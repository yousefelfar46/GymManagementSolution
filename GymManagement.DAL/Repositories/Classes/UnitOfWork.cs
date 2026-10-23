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
    public class UnitOfWork(GymDbContext dbContext, ISessionRepository sessionRepository, IMemberShipRepository memberShipRepository, IBookingRepository bookingRepository) : IUnitOfWork
    {
        private readonly GymDbContext _dbContext = dbContext;
        private readonly Dictionary<string, object> _repositories = [];

        public ISessionRepository SessionRepository { get; } = sessionRepository;
        public IBookingRepository BookingRepository { get; } = bookingRepository;
        public IMemberShipRepository MemberShipRepository { get; } = memberShipRepository;

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name;
            if (_repositories.TryGetValue(typeName, out object? repository))
                return (IGenericRepository<TEntity>)repository!;
            else
            {
                var repo = new GenericRepository<TEntity>(_dbContext);
                _repositories[typeName] = repo;
                return repo;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await _dbContext.SaveChangesAsync(ct);
    }
}