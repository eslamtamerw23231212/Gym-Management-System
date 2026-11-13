using Gym_Management_System.DbContexts;
using GymManangement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManangement.DAL.Repositories.Interfaces.Classes
{
    public class MemberShipRepository : GenericRepository<MemberShip>, IMemberShipRepository
    {
        private readonly GymDbContext dbContext;

        public MemberShipRepository(GymDbContext dbContext):base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<MemberShip>> GetAllMembershipsWithMemberAndPlanAsync(Expression<Func<MemberShip, bool>>? predicate = null, CancellationToken ct = default)
        {
            IQueryable<MemberShip> query = dbContext.MemberShips.AsNoTracking().Include(m => m.Plan).Include(m => m.Memeber);
            if (predicate != null) query = query.Where(predicate);
            return await query.ToListAsync();
        }
    }
}
