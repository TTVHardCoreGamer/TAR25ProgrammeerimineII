using Microsoft.AspNetCore.Http;

namespace ShopTARpe25.Core.Dto
{
    //Dto class vahendab andmeid controlleri ja service classide vahel.
    public class RealEstateDto
    {
        public Guid? Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public string BuildingType { get; set; } = string.Empty;
        public DateTime? BuiltDate { get; set; }
        public int? RoomNumber { get; set; }
        public double? Size { get; set; }

        public List<IFormFile> Files { get; set; }
        public IEnumerable<FileToDatabaseDto> Image { get; set; }
             = new List<FileToDatabaseDto>();

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
