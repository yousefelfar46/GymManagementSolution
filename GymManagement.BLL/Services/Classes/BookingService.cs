using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookingViewModels;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using GymManagement.BLL.ViewModels.SesisonViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class BookingService(IUnitOfWork unitOfWork, IMapper mapper) : IBookingService
    {
        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var allSessions = await unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(ct);
            var bookings = allSessions.Where(s => s.EndDate >= DateTime.Now);
            if (!bookings.Any()) return null!;

            var mappedSessions = mapper.Map<IEnumerable<SessionViewModel>>(bookings);

            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - await unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            }

            return mappedSessions;
        }
        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForUpComingSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            var bookings = await unitOfWork.BookingRepository.GetSessionByIdAsync(sessionId, ct);
            if (bookings == null) return Enumerable.Empty<MemberForSessionViewModel>();
            return bookings.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.MemberId,
                SessionId = b.SessionId,
                MemberName = b.Member?.Name ?? "Unknown",
                BookingDate = b.CreatedAt.ToString("yyyy-mm-dd HH:mm"),
                IsAttended = b.IsAttended
            }).ToList();
        }
        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForOnComingSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            var bookings = await unitOfWork.BookingRepository.GetSessionByIdAsync(sessionId, ct);
            if (bookings == null || !bookings.Any())
                return Enumerable.Empty<MemberForSessionViewModel>();
            return bookings.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.MemberId,
                SessionId = b.SessionId,
                MemberName = b.Member.Name,
                BookingDate = b.CreatedAt.ToString("yyyy-mm-dd HH:mm"),
            }).ToList();
        }

        public async Task<Result> CreateNewBookingAsync(CreateBookingViewModel createBookingViewModel, CancellationToken ct = default)
        {
            var session = await unitOfWork.SessionRepository.GetByIdAsync(createBookingViewModel.SessionId, ct);
            if (session is null) return Result.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Session Started, Can not be booked");

            var hasActiveMembership = await unitOfWork.MemberShipRepository.AnyAsync(m => m.MemberId == createBookingViewModel.MemberId && m.EndDate > DateTime.Now, ct);
            if (!hasActiveMembership)
                return Result.Fail("Member Doesnt have an Active Membership");

            var isAlreadyBooked = await unitOfWork.BookingRepository.AnyAsync(b => b.SessionId == createBookingViewModel.SessionId && b.MemberId == createBookingViewModel.MemberId, ct);
            if (isAlreadyBooked) return Result.Fail("Session Already Booked!");

            var bookedSessions = await unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(createBookingViewModel.SessionId, ct);
            if (bookedSessions >= session.Capacity)
                return Result.Fail("Session is full");

            unitOfWork.BookingRepository.Add(new Booking
            {
                MemberId = createBookingViewModel.MemberId,
                SessionId = createBookingViewModel.SessionId,
                IsAttended = false,
                CreatedAt = DateTime.Now
            });

            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Book Session");
        }

        public async Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropdownAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            var booking = await unitOfWork.BookingRepository.GetAllAsync(s => s.SessionId == sessionId, ct: cancellationToken);

            var bookedMembers = booking.Select(x => x.MemberId);

            var availableMembers = await unitOfWork.GetRepository<Member>().GetAllAsync(m => !bookedMembers.Contains(m.Id));

            return mapper.Map<IEnumerable<MemberSelectListViewModel>>(availableMembers);
        }

        public async Task<Result> MarkAsAttend(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId, ct);

            if (booking is null) return Result.NotFound("Booking Not Found");
            booking.IsAttended = true;
            booking.UpdatedAt = DateTime.UtcNow;

            unitOfWork.BookingRepository.Update(booking);

            var result = await unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.Ok() : Result.Fail("Failed To Mark As Attend");
        }

        public async Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null) return Result.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cant Canel a booking for a session that already started");

            var booking = await unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.SessionId == sessionId && b.MemberId == memberId, ct);

            if (booking is null) return Result.NotFound("Booking Not Found");

            unitOfWork.BookingRepository.Delete(booking);

            var result = await unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete Booking");
        }




    }
}