
using Gym_Management_System.DbContexts;
using GymManangement.DAL.Models;
using GymManangement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GymManangement.DAL.Repositories.Interfaces.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext gymDbContext;

        public GenericRepository(GymDbContext gymDbContext)
        {
            this.gymDbContext = gymDbContext;
        }

        public void AddAsync(TEntity entity)
        {
            gymDbContext.Set<TEntity>().Add(entity);
        }

        public void UpdateAsync(TEntity entity)
        {
            gymDbContext.Set<TEntity>().Update(entity);
        }

        public void DeleteAsync(TEntity entity)
        {
            gymDbContext.Set<TEntity>().Remove(entity);
        }

        public async Task<bool> AnyAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken ct = default)
        {
            return await gymDbContext.Set<TEntity>()
                .AnyAsync(predicate, ct);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool withTracking = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = gymDbContext.Set<TEntity>();

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(
                predicate, cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAll(
            bool withTracking = false,
            CancellationToken ct = default)
        {
            IQueryable<TEntity> query = gymDbContext.Set<TEntity>();

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync(ct);
        }

        public async Task<TEntity?> GetByIdAsync(
            int id,
            CancellationToken ct = default)
        {
            return await gymDbContext.Set<TEntity>()
                .FindAsync(new object[] { id }, ct);
        }
    }
}
