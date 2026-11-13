
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces.Attachement
{
    public interface IAttachementService
    {
        Task<string?> UploadAsync(
            Stream fileStram,
            string fileName,
            string folderName,
            CancellationToken ct = default);

        bool Delete(string fileName, string folderName);

        (Stream Stream, string ContentType)? GetFile(
            string fileName,
            string folderName);
    }
}
