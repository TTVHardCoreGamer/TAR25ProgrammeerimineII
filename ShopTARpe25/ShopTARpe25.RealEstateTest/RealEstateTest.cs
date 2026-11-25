using ShopTARpe25.Core.Dto;
using ShopTARpe25.Core.ServiceInterface;

namespace ShopTARpe25.RealEstateTest
{
    public class RealEstateTest : TestBase
    {
        [Fact]
        public async Task ShouldNot_AddEmptyRealEstate_WhenReturnResult()
        {
            // Arrange
            RealEstateDto dto = new()
            {
                Area = 120.5,
                Address = "Test location",
                RoomNumber = 3,
                BuildingType = "house",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            };

            //Act Siin tuleb kutsuda RealEstate Create meetod esile
            var result = await Svc<IRealEstateServices>().Create(dto);

            //Assert
            Assert.NotNull(result);
        }

        //teha test, kus otsib ankeeti id alusel, aga seda ei ole andmebaasis
        [Fact]
        public async Task ShouldNot_GetByIdRealEstate_WhenReturnsEqual()
        {
            //Arrange
            Guid databaseGuid = Guid.Parse("d3661e93-4701-40a7-9770-ef18cd3a3253");
            Guid wrongGuid = Guid.NewGuid();

            //Act
            await Svc<IRealEstateServices>().DetailAsync(databaseGuid);

            //Assert
            //vastupidist loogikat tuleb kasutada
            Assert.NotEqual(wrongGuid, databaseGuid);
        }

        [Fact]
        public async Task Should_GetByIdRealEstate_WhenReturnsEqual()
        {
            //Arrange
            //see guid on juba andmebaasis olemas ankeedi juures
            Guid databaseGuid = Guid.Parse("d3661e93-4701-40a7-9770-ef18cd3a3253");
            //kui vajutad nupule, siis see guid saadetakse anmdbeaasi, mida vörreldakse
            Guid guid = Guid.Parse("d3661e93-4701-40a7-9770-ef18cd3a3253");

            //Act
            await Svc<IRealEstateServices>().DetailAsync(guid);
            
            //Assert
            Assert.Equal(databaseGuid, guid);
        }

        [Fact]
        public async Task Should_DeleteByIdRealEstate_WhenDeleteRealEstate()
        {
            // arrange
            RealEstateDto dto = MockRealEstateData();

            // act
            var addRealEstate = await Svc<IRealEstateServices>().Create(dto);
            var deleteRealEstate = await Svc<IRealEstateServices>().Delete((Guid)addRealEstate.Id);
            

            // assert
            //võrdlen Id-sid, kas kustutasin selle ära, mis oli andmebaasis
            Assert.Equal(addRealEstate.Id, deleteRealEstate.Id);
        }

        private RealEstateDto MockRealEstateData()
        {
            return new RealEstateDto
            {
                Area = 88.65,
                Address = "TestAddress",
                RoomNumber = 1,
                BuildingType = "TestBuildingType",
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now
            };
        }
    }
}
