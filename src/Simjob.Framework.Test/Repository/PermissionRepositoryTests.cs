using AutoFixture;
using MongoDB.Driver;
using Moq;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Repositories;
using Simjob.Framework.Test.Services.Api;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Xunit;

namespace Simjob.Framework.Test.Repository
{
    public class PermissionRepositoryTests
    {
        private static Fixture fixture;
        private Mock<IMongoCollection<User>> _mockCollection;
        private Mock<IUserHelper> _userHelperMock;
        private TDDContext _mockContext;
        private User _user;
        public static List<User> _list = new List<User>();
        private PermissionRepository permissionRepository;

        public PermissionRepositoryTests()
        {
            fixture = new Fixture();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            List<Claim> listClaim = new List<Claim>();
            _user = fixture.Build<User>().With(x => x.IsDeleted, false).With(x => x.Claims, listClaim).Create();
            _mockCollection = new Mock<IMongoCollection<User>>();
            _mockCollection.Object.InsertOne(_user);
            _userHelperMock = new Mock<IUserHelper>();
            _mockContext = new TDDContext(_userHelperMock.Object);
            _mockContext.GetUserCollection("user");
            permissionRepository = new PermissionRepository(_mockContext, _userHelperMock.Object, "Searchdefs");
            _list.Add(_user);

            Mock<IAsyncCursor<User>> _Cursor = new Mock<IAsyncCursor<User>>();
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
        public void insertshouldReturnResult() {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();

            //Act
            permissionRepository.Insert(permissionObj);
            var insertPermission = permissionRepository.GetById(permissionObj.Id);
            //Assert
            Assert.NotNull(insertPermission);
        }

        [Fact]
        public void InsertShouldReturnNull()
        {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, true).With(x => x.Id,"").Create();

            //Act
            permissionRepository.Insert(permissionObj);
            var insertPermission = permissionRepository.GetById(permissionObj.Id);
            //Assert
            Assert.Null(insertPermission);
        }

        [Fact]
        public void DeletePermanentshouldReturnNull() {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            //Act
            permissionRepository.DeletePermanent(permissionObj.Id);
            var permissionDel = permissionRepository.GetById(permissionObj.Id);
            //assert
            Assert.Null(permissionDel);
        }

        [Fact]
        public void DeletePermanentshouldReturnResult()
        {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            //Act
            permissionRepository.DeletePermanent("ID21323");
            var permissionDel = permissionRepository.GetById(permissionObj.Id);
            //assert
            Assert.NotNull(permissionDel);
        }

        [Fact]
        public void DeleteshouldReturnNull() {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            //Act
            permissionRepository.Delete(permissionObj.Id);
            var permissionDel = permissionRepository.GetById(permissionObj.Id);
            //assert
            Assert.Null(permissionDel);
        }

        [Fact]
        public void DeleteShouldReturnResult()
        {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            //Act
            permissionRepository.Delete("ID21323");
            var permissionDel = permissionRepository.GetById(permissionObj.Id);
            //assert
            Assert.NotNull(permissionDel);
        }
        [Fact]
        public void GetAllShouldReturnData() {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            //Act
            var permissions = permissionRepository.GetAll(1,1,"Name");
            //Assert
            Assert.NotEmpty(permissions.Data);
        }

        [Fact]
        public void GetAllShouldReturnDataNoLimit()
        {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            //Act
            var permissions = permissionRepository.GetAll();
            //Assert
            Assert.NotEmpty(permissions.Data);
        }

        [Fact]
        public void GetByIdShouldReturnResult() {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            //Act
            var permissions = permissionRepository.GetById(permissionObj.Id);
            //Assert
            Assert.NotNull(permissions);
        }
        [Fact]
        public void GetByIdShouldReturnNull()
        {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            //Act
            var permissions = permissionRepository.GetById("Id2313");
            //Assert
            Assert.Null(permissions);
        }

        [Fact]        
        public void GetByIdShouldReturnResultList()
        {
            //Arrange
            var permissionObj1 = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            var permissionObj2 = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            string[] ids = { permissionObj1.Id, permissionObj2.Id };
            permissionRepository.Insert(permissionObj1);
            permissionRepository.Insert(permissionObj2);
            //Act
            var permissions = permissionRepository.GetById(ids);
            //Assert
            Assert.NotNull(permissions);
        }

        [Fact]
        public void GetByIdShouldReturnListNull()
        {
            //Arrange
            var permissionObj1 = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            var permissionObj2 = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            string[] ids = { "Id43231", "Id53341" };
            permissionRepository.Insert(permissionObj1);
            permissionRepository.Insert(permissionObj2);
            //Act
            var permissions = permissionRepository.GetById(ids);
            //Assert
            Assert.True(permissions.Count ==0);
            Assert.IsType<List<Permission>>(permissions);
        }
        [Fact]
        public void insertManyShouldReturnListObj() {
            //Arrange
            List<Permission> permissions = new List<Permission>();
            var permissionObj1 = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            var permissionObj2 = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            string[] ids = { permissionObj1.Id, permissionObj2.Id };
            permissions.Add(permissionObj1);
            permissions.Add(permissionObj2);
            //Act
            permissionRepository.InsertMany(permissions);
            var getPermissions = permissionRepository.GetById(ids);
            //Assert
            Assert.True(getPermissions.Count == permissions.Count);          

        }

        [Fact]
        public void insertManyShouldReturnListEmpty()
        {
            //Arrange
            List<Permission> permissions = new List<Permission>();
            var permissionObj1 = fixture.Build<Permission>().With(x => x.IsDeleted, false).With(x => x.Id, "").Create();
            var permissionObj2 = fixture.Build<Permission>().With(x => x.IsDeleted, false).With(x => x.Id, "").Create();
            string[] ids = { permissionObj1.Id, permissionObj2.Id };
            permissions.Add(permissionObj1);
            permissions.Add(permissionObj2);
            //Act
            permissionRepository.InsertMany(permissions);
            var getPermissions = permissionRepository.GetById(ids);
            //Assert
            Assert.True(getPermissions.Count == 0);

        }

        [Fact]
        public void SearchShouldReturnResult() {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            //Act
            var getPermission = permissionRepository.Search(x=>x.Id==permissionObj.Id,1,1,"Name");
            //Assert
            Assert.NotNull(getPermission);
        }
        [Fact]
        public void SearchShouldReturnNull()
        {
            //Arrange            
            //Act
            var getPermission = permissionRepository.Search(x => x.Name == "ajharte");
            //Assert
            Assert.Empty(getPermission.Data);
        }
        [Fact]
        public void SearchLikeInFieldAutoCompleteShouldResultData() {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            //Act
            var getPermission = permissionRepository.SearchLikeInFieldAutoComplete("Name",permissionObj.Name);
            //Assert
            Assert.Equal(getPermission[0].Id,permissionObj.Id);
        }
        [Fact]
        public void SearchLikeInFieldAutoCompleteShouldResultDataEmpty()
        {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            //Act
            var getPermission = permissionRepository.SearchLikeInFieldAutoComplete("Name", "NamePermission");
            //Assert
            Assert.True(getPermission.Count==0);
        }

        [Fact]
        public void UpdateShouldResultObjectUpdated() {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            var perssionUpdate = fixture.Build<Permission>().With(x => x.IsDeleted, false).With(x=>x.Id,permissionObj.Id).Create();
            //Act
            permissionRepository.Update(permissionObj.Id, perssionUpdate);
            var getPermUpdate = permissionRepository.GetById(permissionObj.Id);
            //Assert
            Assert.NotEqual(permissionObj.Name, getPermUpdate.Name);            
        }

        [Fact]
        public void UpdateShouldResultNull()
        {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            var perssionUpdate = fixture.Build<Permission>().With(x => x.IsDeleted, false).With(x => x.Id, permissionObj.Id).Create();
            //Act
            permissionRepository.Update("A453627991", perssionUpdate);
            var getPermUpdate = permissionRepository.GetById(permissionObj.Id);
            //Assert
            Assert.Equal(permissionObj.Name, getPermUpdate.Name);
        }

        [Fact]
        public void SearchFirstByFieldShouldReturnResult()
        {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            //Act
            var getPermission = permissionRepository.SearchFirstByField("Name", permissionObj.Name);
            //Assert
            Assert.Equal(getPermission.Id, permissionObj.Id);
        }

        [Fact]
        public void SearchFirstByFieldShouldReturnNull()
        {
            //Arrange           
            //Act
            var getPermission = permissionRepository.SearchFirstByField("Name","NamePermission");
            //Assert
            Assert.Null(getPermission);
        }
        [Fact]
        public void GetByFieldShouldReturnResult() {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            //Act
            var getPermission = permissionRepository.GetByField("Name", permissionObj.Name);
            //Assert
            Assert.Equal(getPermission.Id, permissionObj.Id);
        }

        [Fact]
        public void GetByFieldShouldReturnNull()
        {
            //Arrange           
            //Act
            var getPermission = permissionRepository.GetByField("Name", "NamePermission");
            //Assert
            Assert.Null(getPermission);
        }

        [Fact]
        public void ExistsShouldReturnTrue() {
            //Arrange
            var permissionObj = fixture.Build<Permission>().With(x => x.IsDeleted, false).Create();
            permissionRepository.Insert(permissionObj);
            //Act
            var getPermission = permissionRepository.Exists(x => x.Id == permissionObj.Id);
            //Assert
            Assert.True(getPermission);
        }

        [Fact]
        public void ExistsShouldReturnFalse()
        {
            //Arrange            
            //Act
            var getPermission = permissionRepository.Exists(x => x.Id == "Id134512");
            //Assert
            Assert.False(getPermission);
        }
    }
}
