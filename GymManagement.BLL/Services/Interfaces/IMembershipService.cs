using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.MembershipViewMode;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMembershipService
    {
        Task<IEnumerable<MembershipViewModel>> GetAllMemberShipAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<PlanSelectListViewModel>> GetPlanSelectListViewsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<MemberSelectListViewModel>> GetSelectListViewsAsync(CancellationToken cancellationToken = default);
        Task<Result> CreateMembershipAsync(CreateMembershipViewModel model,  CancellationToken cancellationToken = default);
        Task<Result> DeleteActiveMembership(int memberId, CancellationToken cancellationToken = default);
    }
}
