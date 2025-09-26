using AutoFixture;
using Moq;
using Newtonsoft.Json;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Simjob.Framework.Test.Services
{
    public class ProfileServiceTests
    {
        private static Fixture fixture;
        private Mock<IProfileService> profileServiceMock;
        //private ProfileService profileService;
        //private Mock<IMediatorHandler> busMock;
        //private Mock<IWebHostEnvironment> webHostEnvironmentMock;
        //private Mock<IProfileRepository> profileRepositoryMock;
        //private Mock<IUserService> userServiceMock;
        //private Mock<IUserHelper> userHelperMock;
        //private Mock<IRepository<MongoDbContext, Schema>> schemaRepoMock;

        //private Mock<ITokenService> tokenserviceMock;
        //private TokenService tokenservice;

        public ProfileServiceTests()
        {
            fixture = new Fixture();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            profileServiceMock = new Mock<IProfileService>();
            //var notfMock = new Mock<DomainNotificationHandler>();
            //busMock = new Mock<IMediatorHandler>();
            //webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            //profileRepositoryMock = new Mock<IProfileRepository>();
            //userHelperMock = new Mock<IUserHelper>();
            //var dbcontext = new ProfileContext(userHelperMock.Object);
            //schemaRepoMock = new Mock<IRepository<MongoDbContext, Schema>>();
            //profileService = new ProfileService(dbcontext, busMock.Object, profileRepositoryMock.Object, schemaRepoMock.Object);
        }

        [Fact]
        public void GetProfileBytenantyValid()
        {
            //Arrange
            var id = "14325a";
            var tenanty = "accist";
            var profObj = fixture.Build<Profile>().With(c => c.Id, id).With(x => x.Tenanty, tenanty).With(x => x.IsDeleted, false).Create();
            profileServiceMock.Setup(m => m.GetByTenanty(tenanty)).Returns(profObj);
            //Act
            var profile = profileServiceMock.Object.GetByTenanty(tenanty);
            //Assert
            var obj1Str = JsonConvert.SerializeObject(profile);
            var obj2Str = JsonConvert.SerializeObject(profObj);
            Assert.NotNull(profile);
            Assert.Equal(obj1Str, obj2Str);
            Assert.IsType<Profile>(profile);
        }

        [Fact]
        public void GetProfileBytenantyInvalid()
        {
            //Arrange
            var id = "14325a";
            var tenanty = "accist";
            var profObj = fixture.Build<Profile>().With(c => c.Id, id).With(x => x.Tenanty, tenanty).With(x => x.IsDeleted, false).Create();
            profileServiceMock.Setup(m => m.GetByTenanty(tenanty)).Returns(profObj);
            //Act
            var profile = profileServiceMock.Object.GetByTenanty("tenantyTest");
            //Assert
            var obj1Str = JsonConvert.SerializeObject(profile);
            var obj2Str = JsonConvert.SerializeObject(profObj);
            Assert.Null(profile);
            Assert.NotEqual(obj1Str, obj2Str);
            Assert.IsNotType<Profile>(profile);
        }

        [Fact]
        public void GetProfileByIdValid()
        {
            //Arrange
            var id = "14325a";
            var tenanty = "accist";
            var profObj = fixture.Build<Profile>().With(c => c.Id, id).With(x => x.Tenanty, tenanty).With(x => x.IsDeleted, false).Create();
            profileServiceMock.Setup(m => m.GetProfileById(id)).Returns(profObj);
            //Act
            var profile = profileServiceMock.Object.GetProfileById(id);
            //Assert
            var obj1Str = JsonConvert.SerializeObject(profile);
            var obj2Str = JsonConvert.SerializeObject(profObj);
            Assert.NotNull(profile);
            Assert.Equal(obj1Str, obj2Str);
            Assert.IsType<Profile>(profile);
        }

        [Fact]
        public void GetProfileByIdInvalid()
        {
            //Arrange
            var id = "14325a";
            var tenanty = "accist";
            var profObj = fixture.Build<Profile>().With(c => c.Id, id).With(x => x.Tenanty, tenanty).With(x => x.IsDeleted, false).Create();
            profileServiceMock.Setup(m => m.GetProfileById(id)).Returns(profObj);
            //Act
            var profile = profileServiceMock.Object.GetProfileById("153a3");
            //Assert
            var obj1Str = JsonConvert.SerializeObject(profile);
            var obj2Str = JsonConvert.SerializeObject(profObj);
            Assert.Null(profile);
            Assert.NotEqual(obj1Str, obj2Str);
            Assert.IsNotType<Profile>(profile);
        }

        [Fact]
        public void GetProfilesValid()
        {
            //Arrange
            var id = "1ad3412";
            var tenanty = "accist";
            List<Profile> listProfile = new List<Profile>();
            for (int i = 0; i < 4; i++)
            {
                var profObj = fixture.Build<Profile>().With(c => c.Id, id).With(x => x.Tenanty, tenanty).With(x => x.IsDeleted, false).Create();
                listProfile.Add(profObj);
            }
            profileServiceMock.Setup(m => m.GetProfiles()).Returns(listProfile);
            //Act
            var profiles = profileServiceMock.Object.GetProfiles();
            //Assert
            Assert.False(profiles.Count == 0);
            Assert.IsType<List<Profile>>(profiles);
            Assert.NotEmpty(profiles);
            Assert.Same(listProfile, profiles);
        }

        [Fact]
        public void GetProfilesInvalid()
        {
            //Arrange           

            List<Profile> listProfile = new List<Profile>();
            profileServiceMock.Setup(m => m.GetProfiles()).Returns(listProfile);
            //Act
            var profiles = profileServiceMock.Object.GetProfiles();
            //Assert
            Assert.True(profiles.Count == 0);
            Assert.Empty(profiles);
        }
    }
}