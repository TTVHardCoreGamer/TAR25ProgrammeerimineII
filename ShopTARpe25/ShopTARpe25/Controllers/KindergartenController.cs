using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopTARpe25.Models.Kindergarten;
using ShopTARpe25.Core.Dto;
using ShopTARpe25.Core.ServiceInterface;
using ShopTARpe25.Data;

namespace ShopTARpe25.Controllers
{
    public class KindergartenController : Controller
    {
        private readonly IKindergartenServices _kindergartenService;
        private readonly ShopTARpe25Context _context;
        private readonly IFileServices _fileService;

        public KindergartenController(IKindergartenServices kindergartenService,
            ShopTARpe25Context context, IFileServices fileService)
        {
            _kindergartenService = kindergartenService;
            _context = context;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            var result = _context.Kindergartens.Select(x => new KindergartenIndexViewModel
            {
                Id = x.Id, GroupName = x.GroupName, ChildrenCount = x.ChildrenCount,
                KindergartenName = x.KindergartenName, TeacherName = x.TeacherName
            });
            return View(result);
        }

        [HttpGet]
        public IActionResult Create() => View(new KindergartenCreateViewModel());

        [HttpPost]
        public async Task<IActionResult> Create(KindergartenCreateViewModel vm)
        {
            var dto = new KindergartenDto
            {
                Id = vm.Id == Guid.Empty ? Guid.NewGuid() : vm.Id,
                GroupName = vm.GroupName, ChildrenCount = vm.ChildrenCount,
                KindergartenName = vm.KindergartenName, TeacherName = vm.TeacherName,
                Files = vm.Files
            };
            await _kindergartenService.Create(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var kindergarten = await _kindergartenService.DetailsAsync(id);
            if (kindergarten == null) return NotFound();

            var vm = new KindergartenDetailsViewModel
            {
                Id = kindergarten.Id, GroupName = kindergarten.GroupName,
                ChildrenCount = kindergarten.ChildrenCount, KindergartenName = kindergarten.KindergartenName,
                TeacherName = kindergarten.TeacherName, CreatedAt = kindergarten.CreatedAt,
                UpdatedAt = kindergarten.UpdatedAt
            };
            vm.Images.AddRange(await GetImages(id));
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var kindergarten = await _kindergartenService.DetailsAsync(id);
            if (kindergarten == null) return NotFound();

            var vm = new KindergartenUpdateViewModel
            {
                Id = kindergarten.Id, GroupName = kindergarten.GroupName,
                ChildrenCount = kindergarten.ChildrenCount, KindergartenName = kindergarten.KindergartenName,
                TeacherName = kindergarten.TeacherName, CreatedAt = kindergarten.CreatedAt,
                UpdatedAt = kindergarten.UpdatedAt
            };
            vm.Images.AddRange(await GetImages(id));
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(KindergartenUpdateViewModel vm)
        {
            var dto = new KindergartenDto
            {
                Id = vm.Id, GroupName = vm.GroupName, ChildrenCount = vm.ChildrenCount,
                KindergartenName = vm.KindergartenName, TeacherName = vm.TeacherName,
                CreatedAt = vm.CreatedAt, UpdatedAt = vm.UpdatedAt, Files = vm.Files,
                FileToApiDtos = vm.Images.Select(image => new FileToApiDto
                {
                    Id = image.ImageId, ExistingFilePath = image.FilePath,
                    KindergartenId = image.KindergartenId
                }).ToArray()
            };
            await _kindergartenService.Update(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var kindergarten = await _kindergartenService.DetailsAsync(id);
            if (kindergarten == null) return NotFound();

            var vm = new KindergartenDeleteViewModel
            {
                Id = kindergarten.Id, GroupName = kindergarten.GroupName,
                ChildrenCount = kindergarten.ChildrenCount, KindergartenName = kindergarten.KindergartenName,
                TeacherName = kindergarten.TeacherName, CreatedAt = kindergarten.CreatedAt,
                UpdatedAt = kindergarten.UpdatedAt
            };
            vm.Images.AddRange(await GetImages(id));
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmation(Guid id)
        {
            await _kindergartenService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveImage(ImageViewModel vm, string returnAction = "Update")
        {
            var image = await _fileService.RemoveImageFromApi(new FileToApiDto { Id = vm.ImageId });
            if (image == null) return NotFound();
            if (returnAction == "Delete") return RedirectToAction(nameof(Delete), new { id = image.KindergartenId });
            if (returnAction == "Details") return RedirectToAction(nameof(Details), new { id = image.KindergartenId });
            return RedirectToAction(nameof(Update), new { id = image.KindergartenId });
        }

        private async Task<ImageViewModel[]> GetImages(Guid id)
        {
            return await _context.FileToApis.Where(x => x.KindergartenId == id)
                .Select(x => new ImageViewModel
                {
                    FilePath = x.ExistingFilePath, ImageId = x.Id, KindergartenId = x.KindergartenId
                }).ToArrayAsync();
        }
    }
}
