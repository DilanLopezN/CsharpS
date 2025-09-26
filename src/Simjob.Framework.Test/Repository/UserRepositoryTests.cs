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
    public class UserRepositoryTests
    {
        static Fixture fixture;
        private readonly Mock<IMongoCollection<User>> _mockCollection;
        private readonly Mock<IUserHelper> _userHelperMock;
        private readonly UserRepository _userRepository;
        private readonly TDDContext _mockContext;
        private readonly User _user;
        private static readonly List<User> users = new();
        public static List<User> _list = users;
        readonly List<Claim> listClaim = new();

        public UserRepositoryTests()
        {
            fixture = new Fixture();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            //List<Claim> listClaim = new List<Claim>();
            _user = fixture.Build<User>().With(x => x.IsDeleted, false).With(x => x.Claims, listClaim).With(x => x.Id,"").Create();
            _mockCollection = new Mock<IMongoCollection<User>>();
            _mockCollection.Object.InsertOne(_user);
            _userHelperMock = new Mock<IUserHelper>();
            _mockContext = new TDDContext(_userHelperMock.Object);
            _mockContext.GetUserCollection("user");
            _userRepository = new UserRepository(_mockContext, _userHelperMock.Object, "test");

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
        public void GetByIdReturnAnyResult()
        {
            // Arrange
            var userGet = fixture.Build<User>().With(x => x.IsDeleted, false).With(x => x.Claims, listClaim).Create();
            _userRepository.Insert(userGet);

            //Act
            var result = _userRepository.GetById(userGet.Id);
            //Assert 
            //Assert.Equal("admin@admin.com", result.UserName);
            Assert.NotNull(result);
            Assert.IsType<User>(result);

            _userRepository.Delete(userGet.Id);
        }

        [Fact]
        public void GetByIdReturnNull()
        {
            // Arrange
            //Act
            var result = _userRepository.GetById("");
            //Assert            
            Assert.Null(result);
        }

        [Fact]
        public void InsertOneShouldReturnUserCreated()
        {
            // Arrange

            //Act
            _userRepository.Insert(_user);

            var result = _userRepository.GetById(_user.Id);

            // Assert

            Assert.Equal(result.Id, _user.Id);

        }

        [Fact]
        public void UpdateShouldReturnObjectUpdated()
        {
           
            // Arrange
            var userObj = fixture.Build<User>().With(x => x.IsDeleted, false).With(x => x.Claims, listClaim).Create();
            _userRepository.Insert(userObj);

            var userUpdate = fixture.Build<User>().With(x => x.IsDeleted, false).With(x => x.Claims, listClaim).With(x => x.Id, userObj.Id).Create();
            //Act
            _userRepository.Update(userObj.Id, userUpdate);

            var result = _userRepository.GetById(userObj.Id);

            // Assert

            Assert.Equal(result.UserName, userUpdate.UserName);
            Assert.Equal(result.Telefone, userUpdate.Telefone);
            Assert.NotSame(userObj.UserName,result.UserName);
            Assert.NotNull(result);

        }

        [Fact]
        public void UpdateShouldReturnNull()
        {

            // Arrange
            var userObj = fixture.Build<User>().With(x => x.IsDeleted, false).With(x => x.Claims, listClaim).Create();
            _userRepository.Insert(userObj);

            var userUpdate = fixture.Build<User>().With(x => x.IsDeleted, false).With(x => x.Claims, listClaim).With(x => x.Id, userObj.Id).Create();
            //Act
            _userRepository.Update("4A73451", userUpdate);

            var result = _userRepository.GetById(userObj.Id);

            // Assert

            Assert.NotEqual(result.UserName, userUpdate.UserName);
            Assert.NotEqual(result.Telefone, userUpdate.Telefone);
            Assert.NotNull(result);

        }

        [Fact]
        public void InsertManyShouldReturnUsersCreated()
        {

            //Arrange
            List<User> listInsertUser = new();
            List<string> IdUsers = new();
            List<User> listSearchUser = new();
            for (int i = 0; i < 3; i++)
            {
                var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
                listInsertUser.Add(userObj);
                IdUsers.Add(userObj.Id);
            }
            //Act
            _userRepository.InsertMany(listInsertUser);
            for (int i = 0; i < 3; i++)
            {
                var resultUser = _userRepository.GetById(IdUsers[i]);
                listSearchUser.Add(resultUser);
            }
            //Assert
            Assert.NotNull(listSearchUser);
            Assert.True(listSearchUser.Count == 3);
            Assert.DoesNotContain(null, listSearchUser);
        }


        [Fact]
        public void InsertManyShouldReturnNull()
        {

            //Arrange
            List<User> listInsertUser = new();
            List<string> IdUsers = new();
            List<User> listSearchUser = new();
            for (int i = 0; i < 3; i++)
            {
                var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).With(x => x.Id, "").Create();
                listInsertUser.Add(userObj);
                IdUsers.Add(userObj.Id);
            }
            //Act
            _userRepository.InsertMany(listInsertUser);
            for (int i = 0; i < 3; i++)
            {
                var resultUser = _userRepository.GetById(IdUsers[i]);
                listSearchUser.Add(resultUser);
            }
            //Assert
            Assert.Contains(null, listSearchUser);
        }

        [Fact]
        public void GetByIdShouldReturnListUser()
        {
            //Arrange
            List<User> listInsertUser = new();
            List<string> IdUsers = new();
            
            for (int i = 0; i < 3; i++)
            {
                var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
                listInsertUser.Add(userObj);
                IdUsers.Add(userObj.Id);
            }
            _userRepository.InsertMany(listInsertUser);
            string[] ids = { IdUsers[0], IdUsers[1], IdUsers[2] };

            //Act
            var listUsers = _userRepository.GetById(ids);

            //Assert
            Assert.True(listUsers.Count > 0);
            Assert.IsType<List<User>>(listUsers);
            Assert.NotEmpty(listUsers);

        }

        [Fact]
        public void GetByIdShouldReturnListEmpty()
        {
            //Arrange
            
            string[] ids = { "rq2415", "4351tt", "78532eecc" };

            //Act
            var listUsers = _userRepository.GetById(ids);

            //Assert
            Assert.False(listUsers.Count > 0);
            Assert.IsType<List<User>>(listUsers);
            Assert.Empty(listUsers);

        }

        [Fact]
        public void DeleteShouldReturnNull()
        {
            //Arrange
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
            _userRepository.Insert(userObj);
            //Act
            _userRepository.Delete(userObj.Id);
            var userDel = _userRepository.GetById(userObj.Id);
            //Assert
            Assert.Null(userDel);
        }

        [Fact]
        public void DeleteShouldReturnObject()
        {
            //Arrange
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
            _userRepository.Insert(userObj);
            //Act
            _userRepository.Delete("4A83420");
            var userDel = _userRepository.GetById(userObj.Id);
            //Assert
            Assert.NotNull(userDel);
        }

        [Fact]
        public void GetByFieldShouldReturnResults()
        {
            //Arrange
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
            _userRepository.Insert(userObj);
            //Act
            var userSearch = _userRepository.GetByField("UserName", userObj.UserName);
            //Assert
            Assert.NotNull(userSearch);
            Assert.IsType<User>(userSearch);

        }

        [Fact]
        public void GetByFieldShouldReturnNull()
        {
            //Arrange
            //Act
            var userSearch = _userRepository.GetByField("UserName", "Username321");
            //Assert
            Assert.Null(userSearch);
            Assert.IsNotType<User>(userSearch);

        }

        [Fact]
        public void SearchFirstByFieldShouldReturnResults()
        {
            //Arrange
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
            _userRepository.Insert(userObj);
            //Act
            var searchUser = _userRepository.SearchFirstByField("UserName", userObj.UserName);          
            //Assert
            Assert.NotNull(searchUser);
            Assert.IsType<User>(searchUser);
        }

        [Fact]
        public void SearchFirstByFieldShouldReturnNull()
        {
            //Arrange
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
            _userRepository.Insert(userObj);
            //Act
            var searchUser = _userRepository.SearchFirstByField("UserName", "userName");
            //Assert
            Assert.Null(searchUser);
            Assert.IsNotType<User>(searchUser);
        }

        [Fact]
        public void SearchRegexByFieldsShouldReturnResults()
        {
            //Arrange
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
            _userRepository.Insert(userObj);
            //Act
            var searchUser = _userRepository.SearchRegexByFields("UserName", userObj.UserName);          
            //Assert
            Assert.NotEmpty(searchUser.Data);

        }

        [Fact]
        public void SearchRegexByFieldsShouldReturnEmptyData()
        {
            //Arrange
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
            _userRepository.Insert(userObj);
            //Act
            var searchUser = _userRepository.SearchRegexByFields("UserName", "username123",1,1,"UserName");            

            //Assert
            Assert.Empty(searchUser.Data);

        }

        [Fact]
        public void GetAllShouldReturnAllUsersWithNoLimit()
        {

            //Arrange
            //Act
            var allUser = _userRepository.GetAll();
            //Assert
            Assert.NotEmpty(allUser.Data);
        }

        [Fact]
        public void GetAllShouldReturnAllUsersWithLimit()
        {

            //Arrange
            //Act
            var allUser = _userRepository.GetAll(1,1,"UserName");
            //Assert
            Assert.NotEmpty(allUser.Data);
        }

        [Fact]
        public void SearchShouldReturnResult()
        {

            //Arrange
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
            _userRepository.Insert(userObj);
            //Act
            var result = _userRepository.Search(x => x.UserName == userObj.UserName);            
            //Assert
            Assert.NotEmpty(result.Data);

        }

        [Fact]
        public void SearchShouldReturnResultWithLimit()
        {

            //Arrange
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
            _userRepository.Insert(userObj);
            //Act
            var result = _userRepository.Search(x => x.UserName == userObj.UserName,1,1,"UserName");
            //Assert
            Assert.NotEmpty(result.Data);

        }

        [Fact]
        public void SearchShouldReturnNull()
        {
            //Arrange
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
            _userRepository.Insert(userObj);
            //Act
            var result = _userRepository.Search(x => x.UserName == "Username332");            
            //Assert
            Assert.Empty(result.Data);
        }

        [Fact]
        public void SearchLikeInFieldAutoCompleteShouldReturnResult() {
            //Arrange
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
            _userRepository.Insert(userObj);
            //Act
            var result = _userRepository.SearchLikeInFieldAutoComplete("UserName", userObj.UserName);            
            //Assert
            Assert.NotNull(result);
            Assert.Equal(userObj.UserName,result[0].UserName);
        }

        [Fact]
        public void SearchLikeInFieldAutoCompleteShouldReturnNull()
        {
            //Arrange
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
            _userRepository.Insert(userObj);
            //Act
            var result = _userRepository.SearchLikeInFieldAutoComplete("UserName", "UserName1235");           
            //Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ExistsShouldReturnTrue() {
            //Arrange
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
            _userRepository.Insert(userObj);
            //Act
            var result = _userRepository.Exists(x => x.UserName == userObj.UserName);            
            //Assert
            Assert.True(result);
        }

        [Fact]
        public void ExistsShouldReturnFalse()
        {
            //Arrange
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.IsDeleted, false).Create();
            _userRepository.Insert(userObj);
            //Act
            var result = _userRepository.Exists(x => x.UserName == "userName531");            
            //Assert
            Assert.False(result);
        }

    }
}