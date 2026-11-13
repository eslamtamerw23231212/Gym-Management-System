using Gym_Management_System.DbContexts;
using GymManangement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManangement.DAL.Repositories.Interfaces.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext dbContext;

        public SessionRepository(GymDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory(
            CancellationToken ct = default)
        {
            var query = dbContext.Sessions
                .AsNoTracking()
                .Include(x => x.trainer)
                .Include(x => x.category);

            return await query.ToListAsync(ct);
        }

        public async Task<int> GetCountofBookedSlotsAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            return await dbContext.Booking.CountAsync(b => b.SessionId == sessionId);
        }
    }
}