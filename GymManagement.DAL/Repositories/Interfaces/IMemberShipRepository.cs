using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IMemberShipRepository : IGenericRepository<MemberShip>
    {
        Task<List<MemberShip>> GetAllMemberShipsWithMembersAndPlansAsync(Expression<Func<MemberShip, bool>>? expression, CancellationToken ct = default);

    }
}
