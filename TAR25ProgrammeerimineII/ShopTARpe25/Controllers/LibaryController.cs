using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopTARpe25.Core.Domain;
using ShopTARpe25.Core.Dto;
using ShopTARpe25.Core.ServiceInterface;

namespace ShopTARpe25.Controllers
{
    // Kontroller ei tea andmebaasist midagi, ta kasutab ainult teenuse liidest
    public class LibaryController : Controller
    {
        private readonly IBookServices _bookServices;

        public LibaryController(IBookServices bookServices)
        {
            _bookServices = bookServices;
        }

        // Nimekiri + otsing aasta või pealkirja järgi
        [HttpGet]
        public async Task<IActionResult> Index(int? year, string? title)
        {
            if (year.HasValue)
            {
                return View(await _bookServices.GetBooksByYear(year.Value));
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                return View(await _bookServices.GetBooksByTitle(title));
            }

            return View(await _bookServices.GetAllBooks());
        }

        // Praegu laenutuses olevad raamatud
        [HttpGet]
        public async Task<IActionResult> Loaned()
        {
            ViewData["Heading"] = "Praegu laenutuses";
            return View("Index", await _bookServices.GetCurrentlyLoanedBooks());
        }

        [HttpGet]
        public IActionResult Create() => View(new BookDto());

        [HttpPost]
        public async Task<IActionResult> Create(BookDto dto)
        {
            await _bookServices.Create(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookServices.GetBookById(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // Pealkirja muutmine
        [HttpGet]
        public async Task<IActionResult> UpdateBookTitle(int id)
        {
            var book = await _bookServices.GetBookById(id);
            if (book == null) return NotFound();

            return View(new BookDto { Id = book.Id, Title = book.Title });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBookTitle(BookDto dto)
        {
            await _bookServices.UpdateTitle(dto.Id, dto.Title);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookServices.GetBookById(id);
            if (book == null) return NotFound();

            ViewBag.HasActiveLoan = await _bookServices.HasActiveLoan(id);
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var deleted = await _bookServices.Delete(id);

            if (!deleted)
            {
                TempData["Error"] = "Raamatut ei saa kustutada: see on laenutuses või ei leitud.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult CreateReader() => View(new ReaderDto());

        [HttpPost]
        public async Task<IActionResult> CreateReader(ReaderDto dto)
        {
            await _bookServices.CreateReader(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> LoanBook()
        {
            ViewBag.Books = new SelectList(await _bookServices.GetAvailableBooks(), "Id", "Title");
            ViewBag.Readers = new SelectList(await _bookServices.GetAllReaders(), "Id", "FullName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoanBook(int bookId, int readerId)
        {
            await _bookServices.LoanBook(bookId, readerId);
            return RedirectToAction(nameof(ReaderLoans), new { readerId });
        }

        // Ühe lugeja laenutused (rippmenüüst valitud lugeja)
        [HttpGet]
        public async Task<IActionResult> ReaderLoans(int? readerId)
        {
            ViewBag.Readers = new SelectList(await _bookServices.GetAllReaders(), "Id", "FullName", readerId);

            if (readerId == null)
            {
                return View(new List<Loan>());
            }

            return View(await _bookServices.GetLoansByReaderId(readerId.Value));
        }

        // Raamatu tagastamine
        [HttpPost]
        public async Task<IActionResult> ReturnBook(int loanId, int readerId)
        {
            await _bookServices.ReturnBook(loanId);
            return RedirectToAction(nameof(ReaderLoans), new { readerId });
        }
    }
}
