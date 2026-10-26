using ShopTARpe25.Core.Domain;
using ShopTARpe25.Core.Dto;

namespace ShopTARpe25.Core.ServiceInterface
{
    // Liides asub Core kihis, et kontroller ei sõltuks otse EF Core'ist
    public interface IBookServices
    {
        // Ülesanne 3 ja 5 (lugeja lisamine)
        Task<Book> Create(BookDto dto);
        Task<Reader> CreateReader(ReaderDto dto);

        // Ülesanne 4
        Task<IEnumerable<Book>> GetAllBooks();
        Task<IEnumerable<Book>> GetBooksByYear(int year);
        Task<IEnumerable<Book>> GetBooksByTitle(string title);
        Task<IEnumerable<Book>> GetCurrentlyLoanedBooks();
        Task<IEnumerable<Loan>> GetLoansByReaderId(int readerId);

        // Abimeetodid vaadetele
        Task<Book?> GetBookById(int id);
        Task<IEnumerable<Book>> GetAvailableBooks();
        Task<IEnumerable<Reader>> GetAllReaders();

        // Ülesanne 5
        Task<Book?> UpdateTitle(int id, string newTitle);
        Task<Loan?> LoanBook(int bookId, int readerId);
        Task<Loan?> ReturnBook(int loanId);
        Task<bool> Delete(int id);
        Task<bool> HasActiveLoan(int bookId);
    }
}
