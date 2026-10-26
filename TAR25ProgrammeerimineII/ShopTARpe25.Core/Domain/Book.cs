namespace ShopTARpe25.Core.Domain
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        public bool IsAvailable { get; set; }

        // Üks raamat -> palju laenutusi (1 : N)
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
