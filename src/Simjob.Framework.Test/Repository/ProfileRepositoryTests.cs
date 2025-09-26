using AutoFixture;
using MongoDB.Driver;
using Moq;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Repositories;
using Simjob.Framework.Test.Services.Api;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Xunit;

namespace Simjob.Framework.Test.Repository
{
    public class ProfileRepositoryTests
    {
        private static Fixture fixture;
        private Mock<IMongoCollection<User>> _mockCollection;
        private Mock<IUserHelper> _userHelperMock;
        private TDDContext _mockContext;
        private User _user;
        public static List<User> _list = new();
        private ProfileRepository profileRepository;

        public ProfileRepositoryTests()
        {
            fixture = new Fixture();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            List<Claim> listClaim = new();
            _user = fixture.Build<User>().With(x => x.IsDeleted, false).With(x => x.Claims, listClaim).Create();
            _mockCollection = new Mock<IMongoCollection<User>>();
            _mockCollection.Object.InsertOne(_user);
            _userHelperMock = new Mock<IUserHelper>();
            _mockContext = new TDDContext(_userHelperMock.Object);
            _mockContext.GetUserCollection("user");
            profileRepository = new ProfileRepository(_mockContext, _userHelperMock.Object, "profile");
            _list.Add(_user);

            Mock<IAsyncCursor<User>> _Cursor = new();
            _Cursor.Setup(_ => _.Current).Returns(_list);
            _Cursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            //Mock FindSync
            _mockCollection.Setup(op => op.FindSync(It.IsAny<FilterDefinition<User>>(),
            It.IsAny<FindOptions<User, User>>(),
             It.IsAny<CancellationToken>())).Returns(_Cursor.Object);
        }

        [Fact]
        public void InsertShouldReturnObjectInserted()
        {
            //Arrange
            var profileObj = fixture.Build<Profile>().With(x => x.IsDeleted, false).Create();

            //Act
            profileRepository.Insert(profileObj);
            var insertprofile = profileRepository.GetById(profileObj.Id);
            //Assert
            Assert.NotNull(insertprofile);
            Assert.Equal(insertprofile.Id.ToString(), profileObj.Id.ToString());
        }

        [Fact]
        public void InsertShouldReturnNull()
        {
            //Arrange
            var profileObj = fixture.Build<Profile>().With(x => x.IsDeleted, true).With(x => x.Id,"").Create();

            //Act
            profileRepository.Insert(profileObj);
            var insertprofile = profileRepository.GetById(profileObj.Id);
            //Assert
            Assert.Null(insertprofile);
        }

        [Fact]
        public void GetByIdShouldReturnResult()
        {
            //Arrange
            var profileObj = fixture.Build<Profile>().With(x => x.IsDeleted, false).Create();
            profileRepository.Insert(profileObj);

            //Act
            var getProfile = profileRepository.GetById(profileObj.Id);
            //Assert
            Assert.NotNull(getProfile);
            Assert.IsType<Profile>(getProfile);
        }

        [Fact]
        public void GetByIdShouldReturnNull()
        {
            //Arrange
            var profileObj = fixture.Build<Profile>().With(x => x.IsDeleted, false).Create();
            profileRepository.Insert(profileObj);

            //Act
            var getProfile = profileRepository.GetById("ab3142");
            //Assert
            Assert.Null(getProfile);
            Assert.IsNotType<Profile>(getProfile);
        }

        [Fact]
        public void ExistsShouldReturnTrue()
        {
            //Arrange
            var profileObj = fixture.Build<Profile>().With(x => x.IsDeleted, false).Create();
            profileRepository.Insert(profileObj);
            //Act
            var existProfile = profileRepository.Exists(x => x.Id == profileObj.Id);
            //Assert
            Assert.True(existProfile);
        }

        [Fact]
        public void ExistsShouldReturnfalse()
        {
            //Arrange
            var profileObj = fixture.Build<Profile>().With(x => x.IsDeleted, false).Create();
            profileRepository.Insert(profileObj);
            //Act
            var existProfile = profileRepository.Exists(x => x.Id == "T432EA");
            //Assert
            Assert.False(existProfile);
        }

        [Fact]
        public void DeleteShouldReturnNull()
        {
            //Arrange
            var profileObj = fixture.Build<Profile>().With(x => x.IsDeleted, false).Create();
            profileRepository.Insert(profileObj);
            //Act
            profileRepository.Delete(profileObj.Id);
            var profileDel = profileRepository.GetById(profileObj.Id);
            //Assert
            Assert.Null(profileDel);
        }

        [Fact]
        public void DeleteShouldReturnResult()
        {
            //Arrange
            var profileObj = fixture.Build<Profile>().With(x => x.IsDeleted, false).Create();
            profileRepository.Insert(profileObj);
            //Act
            profileRepository.Delete("T46146");
            var profileDel = profileRepository.GetById(profileObj.Id);
            //Assert
            Assert.NotNull(profileDel);
        }

        [Fact]
        public void UpdateShouldReturnObjectUpdated()
        {
            //Arrange
            var profileObj = fixture.Build<Profile>().With(x => x.IsDeleted, false).Create();
            profileRepository.Insert(profileObj);
            var UpdateProfile = fixture.Build<Profile>().With(x => x.IsDeleted, false).With(x => x.Id, profileObj.Id).Create();
            //Act
            profileRepository.Update(profileObj.Id, UpdateProfile);
            var GetUpdateProfile = profileRepository.GetById(profileObj.Id);
            //Assert
            Assert.NotSame(profileObj.Tenanty, GetUpdateProfile.Tenanty);            
        }

        [Fact]
        public void UpdateShouldReturnNull()
        {
            //Arrange
            var profileObj = fixture.Build<Profile>().With(x => x.IsDeleted, false).Create();
            profileRepository.Insert(profileObj);
            var UpdateProfile = fixture.Build<Profile>().With(x => x.IsDeleted, false).With(x => x.Id, profileObj.Id).Create();
            //Act
            profileRepository.Update("", UpdateProfile);
            var GetUpdateProfile = profileRepository.GetById(profileObj.Id);
            //Assert
            Assert.NotSame(profileObj.Tenanty, GetUpdateProfile.Tenanty);
        }

        [Fact]
        public void GetByTenantyShouldReturnResult() {
           //Arrange
           var tenanty = "Accist";
           var profileObj = fixture.Build<Profile>().With(x => x.IsDeleted, false).With(x => x.Tenanty,tenanty).Create();
           profileRepository.Insert(profileObj);
           //Act
           var getProfile =  profileRepository.GetByTenanty(tenanty);
            //Assert
            Assert.NotNull(getProfile);
            Assert.IsType<Profile>(getProfile);
        }
        [Fact]
        public void GetByTenantyShouldReturnNull()
        {
            //Arrange
            var tenanty = "Accist";
            var profileObj = fixture.Build<Profile>().With(x => x.IsDeleted, false).With(x => x.Tenanty, tenanty).Create();
            profileRepository.Insert(profileObj);
            //Act
            var getProfile = profileRepository.GetByTenanty("NewTenanty");
            //Assert
            Assert.Null(getProfile);
            Assert.IsNotType<Profile>(getProfile);
        }


    }
}