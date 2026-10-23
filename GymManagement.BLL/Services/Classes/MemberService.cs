using AutoMapper;
using AutoMapper.Execution;
using GymManagement.BLL.Services.Attachment;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Member = GymManagement.DAL.Models.Member;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork uow , IMapper mapper  , IAttachmentService attachmentService)
        {
            _uow=uow;
            _mapper=mapper;
            _attachmentService=attachmentService;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel memberViewModel,CancellationToken ct = default)
        {
            var memberRepository = _uow.GetRepository<Member>();

            var emailExists = await memberRepository.AnyAsync(
                m => m.Email == memberViewModel.Email);

            var phoneExists = await memberRepository.AnyAsync(
                m => m.Phone == memberViewModel.Phone);

            if (emailExists || phoneExists)
                return false;

            
            if (memberViewModel.PhotoFile == null ||
                memberViewModel.PhotoFile.Length == 0)
            {
                throw new InvalidOperationException(
                    "Please select a photo.");
            }

            string? storedPhotoName;

            using (var stream = memberViewModel.PhotoFile.OpenReadStream())
            {
                storedPhotoName = await _attachmentService.UploadAsync(
                    stream,
                    memberViewModel.PhotoFile.FileName,
                    "MembersPhotos",
                    ct
                );
            }

            if (string.IsNullOrWhiteSpace(storedPhotoName))
            {
                throw new InvalidOperationException(
                    "Photo upload failed. Check file extension and size.");
            }

            var member = _mapper.Map<Member>(memberViewModel);
            member.Photo = storedPhotoName;

            memberRepository.Add(member);

            var result = await _uow.SaveChangesAsync(ct);

            return result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _uow.GetRepository<Member>().GetAllAsync(ct:ct);

            if (!members.Any()) return [];


            var membersViewModels = _mapper.Map<IEnumerable<MemberViewModel>>(members);

            return membersViewModels;
        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int id ,CancellationToken ct = default)
        {
            var member= await _uow.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member is null) return null;

            var vmodel = _mapper.Map<MemberViewModel>(member);


            var activeMembership= await _uow.GetRepository<MemberShip>()
                                            .FirstOrDefaultAsync(m=>m.MemberId == id && m.EndDate>DateTime.Now);


            if(activeMembership is not null)
            {
                vmodel.PlanName=activeMembership.Plan?.Name ?? "No Plan";
                vmodel.MembershipStartDate=activeMembership.CreatedAt.ToString("yyyy-MM-dd");
                vmodel.MembershipEndDate=activeMembership.EndDate.ToString("yyyy-MM-dd");
            }
            return vmodel;  
        }

        public async Task<HealthRecordViewModel?> GetHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var record = await _uow.GetRepository<HealthRecord>().FirstOrDefaultAsync(x=>x.MemberId==memberId , ct);
            if (record is null) return null;

            return _mapper.Map<HealthRecordViewModel>(record);
        }
        public async Task<bool> RemoveMemberAsync(int memberId , CancellationToken ct = default)
        {
            var member = await _uow.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member is null) return false;


            var hasFutureBookings = await _uow.GetRepository<Booking>().AnyAsync(b => b.MemberId == memberId && b.CreatedAt > DateTime.Now, ct);
            if(hasFutureBookings) return false;




            _uow.GetRepository<Member>().Delete(member);
            var res = await _uow.SaveChangesAsync(ct);
            return res > 0;
        }


        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int id, CancellationToken ct = default)
        {
            var member = await _uow.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member is null) return null;

            return _mapper.Map<MemberToUpdateViewModel>(member);
        }

        public async Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel memberVM, CancellationToken ct = default)
        {
      
            var memberRepository = _uow.GetRepository<Member>();

            var member = await memberRepository.GetByIdAsync(id, ct);
            if (member is null) return false;


            var emailExists = await memberRepository.AnyAsync(m => m.Email == memberVM.Email && m.Id != id);
            var phoneExists = await memberRepository.AnyAsync(m => m.Phone == memberVM.Phone && m.Id != id);

            if (emailExists || phoneExists) return false;

            _mapper.Map(memberVM, member);
            member.UpdatedAt= DateTime.Now;


            _uow.GetRepository<Member>().Update(member);
            var res = await _uow.SaveChangesAsync(ct);
            return res > 0;

        }
    }
}
