using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _storageRoot;
        private const string VirtualPathPrefix = "/uploads";

        public LocalFileStorageService(IWebHostEnvironment env)
        {
            _storageRoot = Path.Combine(env.ContentRootPath, "..", "ExternalUploads");

            if (!Directory.Exists(_storageRoot))
                Directory.CreateDirectory(_storageRoot);
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subFolder, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0) return "/images/no-image.png";

            var ext = Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowedExtensions.Contains(ext))
                throw new Exception("Недопустимый формат файла. Допускаются только изображения.");

            var uniqueFileName = $"{Guid.NewGuid()}{ext}";
            var targetFolder = Path.Combine(_storageRoot, subFolder);

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            var filePath = Path.Combine(targetFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            return $"{VirtualPathPrefix}/{subFolder}/{uniqueFileName}";
        }

        public void DeleteFile(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl) || fileUrl.Contains("no-image.png")) return;

            var relativePath = fileUrl.Replace(VirtualPathPrefix, "").Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(_storageRoot, relativePath.TrimStart(Path.DirectorySeparatorChar));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}