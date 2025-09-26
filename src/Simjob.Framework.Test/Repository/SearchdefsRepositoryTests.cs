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
    public class SearchdefsRepositoryTests
    {
        private static Fixture fixture;
        private Mock<IMongoCollection<User>> _mockCollection;
        private Mock<IUserHelper> _userHelperMock;
        private TDDContext _mockContext;
        private User _user;
        public static List<User> _list = new();
        private SearchdefsRepository searchdefsRepository;

        public SearchdefsRepositoryTests()
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
            searchdefsRepository = new SearchdefsRepository(_mockContext, _userHelperMock.Object, "Searchdefs");
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
            var searchdefs = fixture.Build<Searchdefs>().With(x => x.IsDeleted, false).Create();

            //Act
            searchdefsRepository.Insert(searchdefs);
            var insertSearchdef = searchdefsRepository.GetById(searchdefs.Id);
            //Assert
            Assert.NotNull(insertSearchdef);
            Assert.Equal(insertSearchdef.Id.ToString(), searchdefs.Id.ToString());
        }

        [Fact]
        public void InsertShouldReturnNull()
        {
            //Arrange
            var searchdefs = fixture.Build<Searchdefs>().With(x => x.IsDeleted, true).With(x => x.Id,"").Create();

            //Act
            searchdefsRepository.Insert(searchdefs);
            var insertSearchdef = searchdefsRepository.GetById(searchdefs.Id);
            //Assert
            Assert.Null(insertSearchdef);
        }

        [Fact]
        public void DeleteShouldReturnNull()
        {
            //Arrange
            var searchdefs = fixture.Build<Searchdefs>().With(x => x.IsDeleted, false).Create();
            searchdefsRepository.Insert(searchdefs);
            //Act
            searchdefsRepository.Delete(searchdefs.Id);
            var deleteSearchdef = searchdefsRepository.GetById(searchdefs.Id);
            //Assert
            Assert.Null(deleteSearchdef);
        }

        [Fact]
        public void DeleteShouldReturnResult()
        {
            //Arrange
            var searchdefs = fixture.Build<Searchdefs>().With(x => x.IsDeleted, false).Create();
            searchdefsRepository.Insert(searchdefs);
            //Act
            searchdefsRepository.Delete("1234as");
            var deleteSearchdef = searchdefsRepository.GetById(searchdefs.Id);
            //Assert
            Assert.NotNull(deleteSearchdef);

        }

        [Fact]
        public void ExistsShouldReturnTrue()
        {
            //Arrange
            var searchdefs = fixture.Build<Searchdefs>().With(x => x.IsDeleted, false).Create();
            searchdefsRepository.Insert(searchdefs);
            //Act
            var existSearchdef = searchdefsRepository.Exists(x => x.Id == searchdefs.Id);
            //Assert
            Assert.True(existSearchdef);
        }

        [Fact]
        public void ExistsShouldReturnFalse()
        {
            //Arrange
            var searchdefs = fixture.Build<Searchdefs>().With(x => x.IsDeleted, false).Create();
            searchdefsRepository.Insert(searchdefs);
            //Act
            var existSearchdef = searchdefsRepository.Exists(x => x.Id == "t34412d");
            //Assert
            Assert.False(existSearchdef);
        }

        [Fact]
        public void GetAllShouldReturnList()
        {
            //Arrange
            var searchdefs = fixture.Build<Searchdefs>().With(x => x.IsDeleted, false).Create();
            searchdefsRepository.Insert(searchdefs);
            //Act
            var listSearchdef = searchdefsRepository.GetAll();
            Assert.NotEmpty(listSearchdef);
            Assert.IsType<List<Searchdefs>>(listSearchdef);
        }

        [Fact]
        public void UpdateShouldReturnObjectUpdated()
        {
            //Arrange
            var insertSearchdefs = fixture.Build<Searchdefs>().With(x => x.IsDeleted, false).Create();
            searchdefsRepository.Insert(insertSearchdefs);
            var UpdateSearchdefs = fixture.Build<Searchdefs>().With(x => x.IsDeleted, false).With(x => x.Id, insertSearchdefs.Id).Create();
            //Act
            searchdefsRepository.Update(insertSearchdefs.Id, UpdateSearchdefs);
            var GetUpdateSearchdefs = searchdefsRepository.GetById(insertSearchdefs.Id);
            //Assert
            Assert.NotSame(insertSearchdefs.Defs, GetUpdateSearchdefs.Defs);
        }

        [Fact]
        public void UpdateShouldReturnNull()
        {
            //Arrange
            var insertSearchdefs = fixture.Build<Searchdefs>().With(x => x.IsDeleted, false).Create();
            searchdefsRepository.Insert(insertSearchdefs);
            var UpdateSearchdefs = fixture.Build<Searchdefs>().With(x => x.IsDeleted, false).With(x => x.Id, insertSearchdefs.Id).Create();
            //Act
            searchdefsRepository.Update("", UpdateSearchdefs);
            var GetUpdateSearchdefs = searchdefsRepository.GetById(insertSearchdefs.Id);
            //Assert
            Assert.NotSame(insertSearchdefs.Defs, GetUpdateSearchdefs.Defs);
        }

        [Fact]
        public void GetByIdShouldReturnResult()
        {
            //Arrange
            var searchdefs = fixture.Build<Searchdefs>().With(x => x.IsDeleted, false).Create();
            searchdefsRepository.Insert(searchdefs);
            //Act
            var searchdefsObj = searchdefsRepository.GetById(searchdefs.Id);
            //Assert
            Assert.NotNull(searchdefsObj);
            Assert.IsType<Searchdefs>(searchdefsObj);
        }

        [Fact]
        public void GetByIdShouldReturnNull()
        {
            //Arrange
            var searchdefs = fixture.Build<Searchdefs>().With(x => x.IsDeleted, true).Create();
            searchdefsRepository.Insert(searchdefs);
            //Act
            var searchdefsObj = searchdefsRepository.GetById(searchdefs.Id);
            //Assert
            Assert.Null(searchdefsObj);
            Assert.IsNotType<Searchdefs>(searchdefsObj);
        }
    }
}