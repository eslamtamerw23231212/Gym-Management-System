
using GymManagement.BLL.Services.Interfaces.Attachement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class AttachemntService : IAttachementService
    {
        private readonly long maxFileSize = 5 * 1024 * 1024;
        private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<AttachemntService> logger;

        public AttachemntService(
            IWebHostEnvironment webHostEnvironment,
            ILogger<AttachemntService> logger)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
        }

        private string GetFolderPath(string folderName)
        {
            return Path.Combine(
                webHostEnvironment.ContentRootPath,
                folderName);
        }

        public async Task<string?> UploadAsync(
            Stream fileStram,
            string fileName,
            string folderName,
            CancellationToken ct = default)
        {
            if (fileStram == null || !fileStram.CanRead)
                return null;

            if (fileStram.Length == 0 || fileStram.Length > maxFileSize)
                return null;

            if (string.IsNullOrWhiteSpace(fileName) ||
                string.IsNullOrWhiteSpace(folderName))
                return null;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return null;

            try
            {
                var uploadsFolder = GetFolderPath(folderName);

                Directory.CreateDirectory(uploadsFolder);

                var storedFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, storedFileName);

                await using var fileStream = new FileStream(
                    filePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None);

                await fileStram.CopyToAsync(fileStream, ct);

                return storedFileName;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to upload attachment {FileName}", fileName);
                return null;
            }
        }

        public bool Delete(string fileName, string folderName)
        {
            if (string.IsNullOrWhiteSpace(fileName) ||
                string.IsNullOrWhiteSpace(folderName) ||
                Path.GetFileName(fileName) != fileName)
                return false;

            try
            {
                var fullPath = Path.Combine(GetFolderPath(folderName), fileName);

                if (!File.Exists(fullPath))
                    return false;

                File.Delete(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete attachment {File}", fileName);
                return false;
            }
        }

        public (Stream Stream, string ContentType)? GetFile(
            string fileName,
            string folderName)
        {
            if (string.IsNullOrWhiteSpace(fileName) ||
                string.IsNullOrWhiteSpace(folderName) ||
                Path.GetFileName(fileName) != fileName)
                return null;

            try
            {
                var fullPath = Path.Combine(GetFolderPath(folderName), fileName);

                if (!File.Exists(fullPath))
                    return null;

                var stream = new FileStream(
                    fullPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);

                var contentType = Path.GetExtension(fileName).ToLowerInvariant() switch
                {
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };

                return (stream, contentType);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get attachment {File}", fileName);
                return null;
            }
        }
    }
}
