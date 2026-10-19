using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
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
        private readonly IHostEnvironment _environment;

        public FileServices(ShopTARpe25Context context, IHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        private string UploadsFolder => Path.Combine(_environment.ContentRootPath, "wwwroot", "multipleFileUpload");

        public void FilesToApi(SpaceshipDto dto, Spaceship domain) =>
            SaveFiles(dto.Files, domain.Id, null);

        public void FilesToApi(KindergartenDto dto, Kindergarten domain) =>
            SaveFiles(dto.Files, null, domain.Id);

        private void SaveFiles(List<IFormFile>? files, Guid? spaceshipId, Guid? kindergartenId)
        {
            if (files == null || files.Count == 0) return;
            Directory.CreateDirectory(UploadsFolder);

            foreach (var file in files)
            {
                if (file == null || file.Length == 0) continue;
                var safeName = Path.GetFileName(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}_{safeName}";
                var filePath = Path.Combine(UploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    file.CopyTo(stream);

                _context.FileToApis.Add(new FileToApi
                {
                    Id = Guid.NewGuid(),
                    ExistingFilePath = uniqueFileName,
                    SpaceshipId = spaceshipId,
                    KindergartenId = kindergartenId
                });
            }
            _context.SaveChanges();
        }

        public async Task<FileToApi?> RemoveImageFromApi(FileToApiDto dto)
        {
            var image = await _context.FileToApis.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (image == null) return null;

            DeletePhysicalFile(image.ExistingFilePath);
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
                DeletePhysicalFile(image.ExistingFilePath);
                _context.FileToApis.Remove(image);
                removed.Add(image);
            }
            await _context.SaveChangesAsync();
            return removed;
        }

        private void DeletePhysicalFile(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var path = Path.Combine(UploadsFolder, Path.GetFileName(fileName));
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
