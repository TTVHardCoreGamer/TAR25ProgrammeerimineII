using Microsoft.EntityFrameworkCore;
using ShopTARpe25.Core.Domain;
using ShopTARpe25.Core.Dto;
using ShopTARpe25.Core.ServiceInterface;
using ShopTARpe25.Data;

namespace ShopTARpe25.ApplicationServices.Services
{
    // Kogu andmebaasiga suhtlemine on siin. Kontroller ei näe DbContext'i.
    public class LibaryService : IBookServices
    {
        private readonly ShopTARpe25Context _context;

        public LibaryService(ShopTARpe25Context context)
        {
            _context = context;
        }

        // ================= ÜLESANNE 3: andmete lisamine =================
        public async Task<Book> Create(BookDto dto)
        {
            Book book = new();

            book.Title = dto.Title;
            book.Author = dto.Author;
            book.PublishedYear = dto.PublishedYear;
            book.IsAvailable = true; // uus raamat on alati saadaval

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            return book;
        }

        // ================= ÜLESANNE 4: LINQ päringud =================

        // Kõik raamatud
        public async Task<IEnumerable<Book>> GetAllBooks()
        {
            return await _context.Books.OrderBy(x => x.Title).ToListAsync();
        }

        // Raamatud ilmumisaasta järgi (nt 2000 või 2010)
        public async Task<IEnumerable<Book>> GetBooksByYear(int year)
        {
            return await _context.Books
                .Where(x => x.PublishedYear == year)
                .ToListAsync();
        }

        // Raamat pealkirja järgi (Contains -> SQL LIKE '%title%')
        public async Task<IEnumerable<Book>> GetBooksByTitle(string title)
        {
            return await _context.Books
                .Where(x => x.Title.Contains(title))
                .ToListAsync();
        }

        // Praegu laenutuses olevad raamatud (IsAvailable == false)
        public async Task<IEnumerable<Book>> GetCurrentlyLoanedBooks()
        {
            return await _context.Books
                .Where(x => x.IsAvailable == false)
                .ToListAsync();
        }

        // Ühe lugeja kõik laenutused (Include toob kaasa ka raamatu andmed)
        public async Task<IEnumerable<Loan>> GetLoansByReaderId(int readerId)
        {
            return await _context.Loans
                .Include(x => x.Book)
                .Where(x => x.ReaderId == readerId)
                .OrderByDescending(x => x.LoanDate)
                .ToListAsync();
        }

        public async Task<Book?> GetBookById(int id)
        {
            return await _context.Books.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Book>> GetAvailableBooks()
        {
            return await _context.Books.Where(x => x.IsAvailable).ToListAsync();
        }

        public async Task<IEnumerable<Reader>> GetAllReaders()
        {
            return await _context.Readers.OrderBy(x => x.FullName).ToListAsync();
        }

        // ================= ÜLESANNE 5: muutmine ja kustutamine =================

        // Raamatu pealkirja muutmine
        public async Task<Book?> UpdateTitle(int id, string newTitle)
        {
            var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return null;
            }

            book.Title = newTitle;
            await _context.SaveChangesAsync();

            return book;
        }

        // Laenutamine (vajalik, et tagastamist ja laenutusi üldse oleks)
        public async Task<Loan?> LoanBook(int bookId, int readerId)
        {
            var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == bookId);
            var reader = await _context.Readers.FirstOrDefaultAsync(x => x.Id == readerId);

            if (book == null || reader == null || !book.IsAvailable)
            {
                return null;
            }

            Loan loan = new();
            loan.BookId = bookId;
            loan.ReaderId = readerId;
            loan.LoanDate = DateTime.Now;

            book.IsAvailable = false;

            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        // Raamatu tagastamine: salvestab tagastamise kuupäeva ja teeb raamatu uuesti saadavaks
        public async Task<Loan?> ReturnBook(int loanId)
        {
            var loan = await _context.Loans
                .Include(x => x.Book)
                .FirstOrDefaultAsync(x => x.Id == loanId);

            if (loan == null || loan.ReturnDate != null)
            {
                return null; // laenutust pole või on juba tagastatud
            }

            loan.ReturnDate = DateTime.Now;
            loan.Book.IsAvailable = true;

            await _context.SaveChangesAsync();

            return loan;
        }

        // Kas raamatul on tagastamata laenutus?
        public async Task<bool> HasActiveLoan(int bookId)
        {
            return await _context.Loans
                .AnyAsync(x => x.BookId == bookId && x.ReturnDate == null);
        }

        // Raamatu kustutamine.
        // Mida tuleb enne kontrollida, kui raamat on seotud laenutustega:
        //  1) Kas raamat on olemas (muidu pole midagi kustutada).
        //  2) Kas raamatul on TAGASTAMATA laenutus (ReturnDate == null / IsAvailable == false).
        //     Kui jah, siis kustutada ei tohi, sest raamat on lugeja käes.
        //  3) Kas on tagastatud (ajaloolisi) laenutusi. Foreign key on Restrict, seega
        //     andmebaas ei luba raamatut kustutada, kuni neid kirjeid alles on.
        //     Seetõttu kustutame need enne ise ära (või säilitaksime ajaloo ja kasutaksime
        //     "pehmet kustutamist", nt IsDeleted lipp).
        public async Task<bool> Delete(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return false;
            }

            if (await HasActiveLoan(id))
            {
                return false;
            }

            var oldLoans = await _context.Loans.Where(x => x.BookId == id).ToListAsync();

            _context.Loans.RemoveRange(oldLoans);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return true;
        }

        // Lugejate lisamine
        public async Task<Reader> CreateReader(ReaderDto dto)
        {
            Reader reader = new();

            reader.FullName = dto.FullName;
            reader.Email = dto.Email;

            await _context.Readers.AddAsync(reader);
            await _context.SaveChangesAsync();

            return reader;
        }
    }
}
