using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ShopTARpe25.Core.Domain;
using ShopTARpe25.Core.Dto;
using ShopTARpe25.Core.ServiceInterface;
using ShopTARpe25.Data;

namespace ShopTARpe25.ApplicationServices.Services
{
    public class FileServices : IFileServices
    {
        private readonly ShopTARpe25Context _context;
        private readonly IHostEnvironment _webHost;

        public FileServices(ShopTARpe25Context context, IHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        // Spaceshipi piltide salvestamine – sama ülesehitus nagu algses Spaceshipi lahenduses.
        public void FilesToApi(SpaceshipDto dto, Spaceship domain)
        {
            if (dto.Files == null || dto.Files.Count == 0) return;

            string uploadsFolder = Path.Combine(_webHost.ContentRootPath, "wwwroot", "multipleFileUpload");
            Directory.CreateDirectory(uploadsFolder);

            foreach (var file in dto.Files)
            {
                if (file == null || file.Length == 0) continue;
                string uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    file.CopyTo(fileStream);

                var path = new FileToApi
                {
                    Id = Guid.NewGuid(),
                    ExistingFilePath = uniqueFileName,
                    SpaceshipId = domain.Id
                };
                _context.FileToApis.Add(path);
            }
            _context.SaveChanges();
        }

        // Kindergarteni piltide salvestamine on tehtud sama mustri järgi nagu Spaceshipis.
        public void FilesToApi(KindergartenDto dto, Kindergarten domain)
        {
            if (dto.Files == null || dto.Files.Count == 0) return;

            string uploadsFolder = Path.Combine(_webHost.ContentRootPath, "wwwroot", "multipleFileUpload");
            Directory.CreateDirectory(uploadsFolder);

            foreach (var file in dto.Files)
            {
                if (file == null || file.Length == 0) continue;
                string uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    file.CopyTo(fileStream);

                var path = new FileToApi
                {
                    Id = Guid.NewGuid(),
                    ExistingFilePath = uniqueFileName,
                    KindergartenId = domain.Id
                };
                _context.FileToApis.Add(path);
            }
            _context.SaveChanges();
        }

        public async Task<FileToApi?> RemoveImageFromApi(FileToApiDto dto)
        {
            var image = await _context.FileToApis.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (image == null) return null;

            DeleteImageFile(image.ExistingFilePath);
            _context.FileToApis.Remove(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<List<FileToApi>> RemoveImagesFromApi(FileToApiDto[] dto)
        {
            var removed = new List<FileToApi>();

            foreach (var item in dto)
            {
                var image = await _context.FileToApis.FirstOrDefaultAsync(x => x.Id == item.Id);
                if (image == null) continue;

                DeleteImageFile(image.ExistingFilePath);
                _context.FileToApis.Remove(image);
                removed.Add(image);
            }

            await _context.SaveChangesAsync();
            return removed;
        }

        private void DeleteImageFile(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;

            string uploadsFolder = Path.Combine(_webHost.ContentRootPath, "wwwroot", "multipleFileUpload");
            string filePath = Path.Combine(uploadsFolder, Path.GetFileName(fileName));

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
