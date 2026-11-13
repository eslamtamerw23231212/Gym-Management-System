using GymManangement.DAL.Models;
using GymManangement.DAL.Repositories.Interfaces.Classes;
using System.Threading;
using System.Threading.Tasks;

namespace GymManangement.DAL.Repositories.Interfaces
{
    public interface IUnitofWork
    {
        IGenericRepository<TEntity> GetReposioty<TEntity>()
            where TEntity : BaseEntity, new();

        SessionRepository sessionRepository { get; }
        MemberShipRepository memberShipRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}