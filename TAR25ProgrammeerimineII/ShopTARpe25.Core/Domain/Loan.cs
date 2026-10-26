namespace ShopTARpe25.Core.Domain
{
    public class Loan
    {
        public int Id { get; set; }

        // Võõrvõtmed
        public int BookId { get; set; }
        public int ReaderId { get; set; }

        public DateTime LoanDate { get; set; }

        // Nullable (?), sest tagastamise kuupäev puudub, kuni raamat on tagastamata
        public DateTime? ReturnDate { get; set; }

        // Navigatsiooniomadused
        public Book Book { get; set; } = null!;
        public Reader Reader { get; set; } = null!;
    }
}
