using GymManagement.BLL.Services.Classes;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(
            CancellationToken ct = default);
        Task<bool> CreateMemberAsync(CreateMemberViewModel viewModel, CancellationToken ct = default);
        Task<MemberViewModel?> GetMemberDetailsByIdAsync(int id, CancellationToken ct = default);
        Task<HealthRecordViewModel?> GetHealthRecordDetails(int id, CancellationToken cancellationToken = default);
        Task<MemberToUpdateViewModel?> UpdateMember(int id, CancellationToken ct = default);
        Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel memberToUpdateViewModel, CancellationToken ct = default);
        Task<bool> RemoveMemberAsync(int MemberId, CancellationToken ct = default);
    }
}