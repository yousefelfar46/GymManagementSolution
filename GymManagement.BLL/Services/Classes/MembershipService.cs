using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MembershipViewMode;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class MembershipService(IUnitOfWork unitOfWork, IMapper mapper ) : IMembershipService
    {
        public async Task<Result> CreateMembershipAsync(CreateMembershipViewModel model, CancellationToken cancellationToken = default)
        {
            var isMemberExists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Id == model.MemberId, cancellationToken);
            if (!isMemberExists) return Result.NotFound("Member Not Found");

            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(model.PlanId, cancellationToken);
            if (plan is null) return Result.NotFound("Plan Not Found");
            if (!plan.IsActive) return Result.Fail("Plan Not Active");

            var hasActiveMembership = await unitOfWork.MemberShipRepository     
                             .AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now, cancellationToken);

            if (hasActiveMembership) return Result.Fail("Member Already Has an Active Membership");

            var entity = new MemberShip
            {
                MemberId = model.MemberId,
                PlanId = model.PlanId,
                CreatedAt = DateTime.Now,
                EndDate = (model.StartDate ?? DateTime.Now).AddDays(plan.DurationDays)
            };

            unitOfWork.MemberShipRepository.Add(entity);

            var result = await unitOfWork.SaveChangesAsync(cancellationToken);

            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create Membership"); 
             



        }
        public async Task<Result> DeleteActiveMembership(int memberId, CancellationToken cancellationToken = default)
        {
            var active = await unitOfWork.MemberShipRepository.FirstOrDefaultAsync(m => m.MemberId == memberId && m.EndDate > DateTime.Now, cancellationToken);
            if (active is null)
                return Result.NotFound("Member Not Found");

            unitOfWork.MemberShipRepository.Delete(active);
            var result = await unitOfWork.SaveChangesAsync(cancellationToken);

            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete Membership");
        }

        public async Task<IEnumerable<MembershipViewModel>> GetAllMemberShipAsync(CancellationToken cancellationToken = default)
        {
            var memberships = await unitOfWork.MemberShipRepository
               .GetAllMemberShipsWithMembersAndPlansAsync(m => m.EndDate > DateTime.Now, cancellationToken);

            return mapper.Map<IEnumerable<MembershipViewModel>>(memberships);
        }

        public async Task<IEnumerable<PlanSelectListViewModel>> GetPlanSelectListViewsAsync(CancellationToken cancellationToken = default)
        {
            var plans = await unitOfWork.GetRepository<Plan>().GetAllAsync(p => p.IsActive, ct:cancellationToken);
            return mapper.Map<IEnumerable<PlanSelectListViewModel>>(plans);
        }

        public async Task<IEnumerable<MemberSelectListViewModel>> GetSelectListViewsAsync(CancellationToken cancellationToken = default)
        {
            var members = await unitOfWork.GetRepository<Member>().GetAllAsync(ct: cancellationToken);
            return mapper.Map<IEnumerable<MemberSelectListViewModel>>(members);
        }
    }
}
