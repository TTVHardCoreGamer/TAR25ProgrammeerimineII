namespace ShopTARpe25.Models.RealEstate
{
    public class RealEstateDeleteViewModel
    {
        public Guid? Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public string BuildingType { get; set; } = string.Empty;
        public DateTime? BuiltDate { get; set; }
        public int? RoomNumber { get; set; }
        public double? Size { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
