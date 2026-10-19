using System;
using System.Collections.Generic;
using System.Text;

namespace ShopTARpe25.Core.Dto
{
    //Dto class vahendab andmeid controlleri ja service classide vahel.
    public class SpaceshipDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Classification { get; set; } = string.Empty;
        public DateTime? BuiltDate { get; set; }
        public int? Crew { get; set; }
        public int? EnginePower { get; set; }
        public List<Microsoft.AspNetCore.Http.IFormFile>? Files { get; set; }
        public FileToApiDto[] FileToApiDtos { get; set; } = Array.Empty<FileToApiDto>();

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
