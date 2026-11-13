using Microsoft.AspNetCore.Mvc;
using ShopTARpe25.Models.RealEstate;
using ShopTARpe25.Core.Dto;
using ShopTARpe25.Core.ServiceInterface;
using ShopTARpe25.Data;
using Microsoft.EntityFrameworkCore;


namespace ShopTARpe25.Controllers
{
    public class RealEstateController : Controller
    {
        private readonly IRealEstateServices _realEstateService;
        private readonly ShopTARpe25Context _context;
        //teha constructor et saaks kasutada teenust, mis on
        //defineeritud IRealEstateServices liideses
        //lisage Context
        public RealEstateController
            (
                IRealEstateServices realEstateService,
                ShopTARpe25Context context
            )
        {
            _realEstateService = realEstateService;
            _context = context;
        }


        public IActionResult Index()
        {
            //loome vaheinstantsi domaini ja viewModeli vahel.
            var result = _context.RealEstates
                .Select(x => new RealEstateIndexViewModel
                {
                    Id = x.Id,
                    Location = x.Location,
                    BuildingType = x.BuildingType,
                    BuiltDate = x.BuiltDate,
                    RoomNumber = x.RoomNumber,
                    Size = x.Size
                });

            return View(result);
        }

        //kui kasutaja klikib "Create" nuppu, siis see meetod käivitatakse
        //tagastab kasutajale vormi, kuhu saab sisestada andmed
        [HttpGet]
        public IActionResult Create()
        {
            RealEstateCreateUpdateViewModel vm = new();

            return View("CreateUpdate", vm);
        }

        //kui oled teinud vormi, siis see meetod käivitatakse
        //saadab andmed serverisse, kus need salvestatakse andmebaasi
        [HttpPost]
        public async Task<IActionResult> Create(RealEstateCreateUpdateViewModel vm)
        {
            //luua vaheinstants, mis sisaldab andmeid, mis on saadud vormist
            //need andmed tuleb edasi saata dto-sse, mis on mõeldud andmebaasi salvestamiseks

            var dto = new RealEstateDto
            {
                Id = vm.Id,
                Location = vm.Location,
                BuildingType = vm.BuildingType,
                BuiltDate = vm.BuiltDate,
                RoomNumber = vm.RoomNumber,
                Size = vm.Size,
                Files = vm.Files,
                Image = vm.Image
                   .Select(x => new FileToDatabaseDto
                   {
                       Id = x.ImageId,
                       ImageData = x.ImageData,
                       ImageTitle = x.ImageTitle,
                       RealEstateId = x.RealEstateId
                   }).ToArray()
            };

            //kutsuda teenuse meetodit, mis salvestab andmed andmebaasi
            var result = await _realEstateService.Create(dto);

            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        //tuleb teha Details meetod
        //see kutsub v'lja interfacest service meetodi

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            //meetodi kutsumine interfacest
            var realEstate = await _realEstateService.DetailsAsync(id);

            //veakäsitlus
            //suunab vaatele NotFound, kui andmeid ei ole
            if (realEstate == null)
            {
                return NotFound();
            }

            RealEstateImageViewModel[] images = await FileFromDatabase(id);

            //tuleb teha viewModel ja see siin välja kutsuda
            //ära map-ida vm ja domain
            var vm = new RealEstateDetailsViewModel();

            vm.Id = realEstate.Id;
            vm.Location = realEstate.Location;
            vm.BuildingType = realEstate.BuildingType;
            vm.BuiltDate = realEstate.BuiltDate;
            vm.RoomNumber = realEstate.RoomNumber;
            vm.Size = realEstate.Size;
            vm.CreatedAt = realEstate.CreatedAt;
            vm.ModifiedAt = realEstate.ModifiedAt;

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var realEstate = await _realEstateService.DetailsAsync(id);

            if (realEstate == null)
            {
                return NotFound();
            }

            var vm = new RealEstateCreateUpdateViewModel();

            vm.Id = realEstate.Id;
            vm.Location = realEstate.Location;
            vm.BuildingType = realEstate.BuildingType;
            vm.BuiltDate = realEstate.BuiltDate;
            vm.RoomNumber = realEstate.RoomNumber;
            vm.Size = realEstate.Size;
            vm.CreatedAt = realEstate.CreatedAt;
            vm.ModifiedAt = realEstate.ModifiedAt;

            return View("CreateUpdate", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(RealEstateUpdateViewModel vm)
        {
            var dto = new RealEstateDto
            {
                Id = vm.Id,
                Location = vm.Location,
                BuildingType = vm.BuildingType,
                BuiltDate = vm.BuiltDate,
                RoomNumber = vm.RoomNumber,
                Size = vm.Size
            };

            var result = await _realEstateService.Update(dto);

            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var realEstate = await _realEstateService.DetailsAsync(id);

            if (realEstate == null)
            {
                return NotFound();
            }

            var vm = new RealEstateDeleteViewModel();

            vm.Id = realEstate.Id;
            vm.Location = realEstate.Location;
            vm.BuildingType = realEstate.BuildingType;
            vm.BuiltDate = realEstate.BuiltDate;
            vm.RoomNumber = realEstate.RoomNumber;
            vm.Size = realEstate.Size;
            vm.CreatedAt = realEstate.CreatedAt;
            vm.ModifiedAt = realEstate.ModifiedAt;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmation(Guid id)
        {
            var result = await _realEstateService.Delete(id);

            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<RealEstateImageViewModel[]> FileFromDatabase(Guid id)
        {
            var result = await _context.FileToDatabases
                .Where(x => x.RealEstateId == id)
                .Select(y => new RealEstateImageViewModel
                {
                    ImageId = y.Id,
                    RealEstateId = y.Id,
                    ImageData = y.ImageData,
                    ImageTitle = y.ImageTitle,
                    Image = string.Format("data:image/gif;base64, {0}", Convert.ToBase64String(y.ImageData))
                }).ToArrayAsync();
            
            return result;
        }
    }
}
