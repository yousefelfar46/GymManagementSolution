using GymManagement.DAL.DATA.DbContexts;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class MemberShipRepository : GenericRepository<MemberShip>, IMemberShipRepository
    {
        private readonly GymDbContext _dbContext;

        public MemberShipRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<MemberShip>> GetAllMemberShipsWithMembersAndPlansAsync(Expression<Func<MemberShip, bool>>? expression, CancellationToken cancellationToken = default)
        {
            IQueryable<MemberShip> qeury = _dbContext.MemberShips.AsNoTracking()
                                           .Include(m => m.Plan)
                                           .Include(m => m.Member);
            if (expression != null)
            {
                qeury = qeury.Where(expression);
            }
            return qeury.ToListAsync(cancellationToken);
        }
    }
}
