using Gym_Management_System.Models;
using GymManangement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManangement.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity:BaseEntity,new()
    {
        Task<IEnumerable<TEntity>> GetAll(bool withTracking = false, CancellationToken ct = default);
        //
        Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
        void AddAsync(TEntity entity);
        void UpdateAsync(TEntity entity);
        void DeleteAsync(TEntity entity);
        Task<bool> AnyAsync(Expression<Func<TEntity,bool>> predicate, CancellationToken ct = default);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool withTracking=false, CancellationToken cancellationToken = default);
    }
}
