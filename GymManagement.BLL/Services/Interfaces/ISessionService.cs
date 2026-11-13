
using GymManagement.BLL.Services.Classes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>?> GetAllSessions(
            CancellationToken cancellationToken = default);
        Task<bool> CreateSessionAsync(CreateSessionViewModel createSessionViewModel, CancellationToken ct = default);
        Task<IEnumerable<TrainerSelectViewModel>> GetTrainerFromDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<CategorySelectViewModel>> GetCategoryFromDropDownAsync(CancellationToken ct = default);
        Task<SessionViewModel?> GetSessionById(int sessionId, CancellationToken ct = default);
        Task<Result> UpdateSessionAsync(int id,UpdateSessionViewModel updateSessionViewModel, CancellationToken cancellationToken = default);
        Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default);
        Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct = default);
    }
}
