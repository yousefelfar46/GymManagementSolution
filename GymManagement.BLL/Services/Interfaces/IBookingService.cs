using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.BookingViewModels;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using GymManagement.BLL.ViewModels.SesisonViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default);
        Task<IEnumerable<MemberForSessionViewModel>> GetMembersForUpComingSessionByIdAsync(int sessionId, CancellationToken ct = default);
        Task<IEnumerable<MemberForSessionViewModel>> GetMembersForOnComingSessionByIdAsync(int sessionId, CancellationToken ct = default);
        Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropdownAsync(int sessionId, CancellationToken cancellationToken = default);
        Task<Result> CreateNewBookingAsync(CreateBookingViewModel createBookingViewModel, CancellationToken ct = default);
        Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default);
        Task<Result> MarkAsAttend(int memberId, int sessionId, CancellationToken ct = default);
    }
}
