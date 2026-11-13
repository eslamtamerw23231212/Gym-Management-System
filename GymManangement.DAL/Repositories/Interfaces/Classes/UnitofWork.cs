using Gym_Management_System.DbContexts;
using GymManangement.DAL.Models;
using GymManangement.DAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GymManangement.DAL.Repositories.Interfaces.Classes
{
    public class UnitofWork : IUnitofWork
    {
        private readonly GymDbContext dbContext;
        private readonly Dictionary<string, object> _repositories = [];

        public SessionRepository sessionRepository { get; }

        public MemberShipRepository memberShipRepository { get; }

        public UnitofWork(
            GymDbContext dbContext,
            SessionRepository sessionRepository)
        {
            this.dbContext = dbContext;
            this.sessionRepository = sessionRepository;
        }

        public IGenericRepository<TEntity> GetReposioty<TEntity>()
            where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name;

            if (_repositories.ContainsKey(typeName))
            {
                return (IGenericRepository<TEntity>)_repositories[typeName];
            }

            var repository = new GenericRepository<TEntity>(dbContext);

            _repositories.Add(typeName, repository);

            return repository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await dbContext.SaveChangesAsync(ct);
        }
    }
}