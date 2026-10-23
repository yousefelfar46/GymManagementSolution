using GymManagement.DAL.DATA.DbContexts;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly GymDbContext _context;

        public BookingRepository(GymDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
            => await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Member)
                .Where(b => b.SessionId == sessionId)
                .ToListAsync(ct);
    }
}
