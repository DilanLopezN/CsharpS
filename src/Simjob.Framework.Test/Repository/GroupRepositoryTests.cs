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
    public class GroupRepositoryTests
    {
        private static Fixture fixture;
        private Mock<IMongoCollection<User>> _mockCollection;
        private Mock<IUserHelper> _userHelperMock;
        private TDDContext _mockContext;
        private User _user;
        public static List<User> _list = new List<User>();
        private GroupRepository groupRepository;

        public GroupRepositoryTests()
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
            groupRepository = new GroupRepository(_mockContext, _userHelperMock.Object, "group");
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
        public void insertshouldReturnResult()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();

            //Act
            groupRepository.Insert(groupObj);
            var insertGroup = groupRepository.GetById(groupObj.Id);
            //Assert
            Assert.NotNull(insertGroup);
        }

        [Fact]
        public void insertshouldReturnNull()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, true).With(x => x.Id, "").Create();

            //Act
            groupRepository.Insert(groupObj);
            var insertGroup = groupRepository.GetById(groupObj.Id);
            //Assert
            Assert.Null(insertGroup);
        }


        [Fact]
        public void DeleteshouldReturnNull()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            groupRepository.Insert(groupObj);
            //Act
            groupRepository.Delete(groupObj.Id);
            var groupDel = groupRepository.GetById(groupObj.Id);
            //assert
            Assert.Null(groupDel);
        }

        [Fact]
        public void DeleteShouldReturnResult()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            groupRepository.Insert(groupObj);
            //Act
            groupRepository.Delete("ID21323");
            var groupDel = groupRepository.GetById(groupObj.Id);
            //assert
            Assert.NotNull(groupDel);
        }
        [Fact]
        public void GetAllShouldReturnData()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            groupRepository.Insert(groupObj);
            //Act
            var groups = groupRepository.GetAll(1,1,"name");
            //Assert
            Assert.NotEmpty(groups.Data);
        }

        [Fact]
        public void GetAllShouldReturnDataNoLimit()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            groupRepository.Insert(groupObj);
            //Act
            var groups = groupRepository.GetAll();
            //Assert
            Assert.NotEmpty(groups.Data);
        }
        [Fact]
        public void GetByIdShouldReturnResult()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            groupRepository.Insert(groupObj);
            //Act
            var groups = groupRepository.GetById(groupObj.Id);
            //Assert
            Assert.NotNull(groups);
        }
        [Fact]
        public void GetByIdShouldReturnNull()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            groupRepository.Insert(groupObj);
            //Act
            var groups = groupRepository.GetById("Id2313");
            //Assert
            Assert.Null(groups);
        }

        [Fact]
        public void GetByIdShouldReturnResultList()
        {
            //Arrange
            var groupObj1 = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            var groupObj2 = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            string[] ids = { groupObj1.Id, groupObj2.Id };
            groupRepository.Insert(groupObj1);
            groupRepository.Insert(groupObj2);
            //Act
            var groups = groupRepository.GetById(ids);
            //Assert
            Assert.NotNull(groups);
        }

        [Fact]
        public void GetByIdShouldReturnListNull()
        {
            //Arrange
            var groupObj1 = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            var groupObj2 = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            string[] ids = { "Id43231", "Id53341" };
            groupRepository.Insert(groupObj1);
            groupRepository.Insert(groupObj2);
            //Act
            var groups = groupRepository.GetById(ids);
            //Assert
            Assert.True(groups.Count == 0);
            Assert.IsType<List<Group>>(groups);
        }
        [Fact]
        public void insertManyShouldReturnListObj()
        {
            //Arrange
            List<Group> groups = new List<Group>();
            var groupObj1 = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            var groupObj2 = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            string[] ids = { groupObj1.Id, groupObj2.Id };
            groups.Add(groupObj1);
            groups.Add(groupObj2);
            //Act
            groupRepository.InsertMany(groups);
            var getGroups = groupRepository.GetById(ids);
            //Assert
            Assert.True(getGroups.Count == groups.Count);

        }

        [Fact]
        public void insertManyShouldReturnListEmptyWithEmptyId()
        {
            //Arrange
            List<Group> groups = new List<Group>();
            var groupObj1 = fixture.Build<Group>().With(x => x.IsDeleted, false).With(x => x.Id, "").Create();
            var groupObj2 = fixture.Build<Group>().With(x => x.IsDeleted, false).With(x => x.Id, "").Create();
            string[] ids = { groupObj1.Id, groupObj2.Id };
            groups.Add(groupObj1);
            groups.Add(groupObj2);
            //Act
            groupRepository.InsertMany(groups);
            var getGroups = groupRepository.GetById(ids);
            //Assert
            Assert.True(getGroups.Count == 0);

        }

        [Fact]
        public void SearchShouldReturnResult()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            groupRepository.Insert(groupObj);
            //Act
            var getGroup = groupRepository.Search(x => x.Id == groupObj.Id);
            //Assert
            Assert.NotNull(getGroup);
        }
        [Fact]
        public void SearchShouldReturnNull()
        {
            //Arrange            
            //Act
            var getGroup = groupRepository.Search(x => x.GroupName == "ajharte",1,1,"name");
            //Assert
            Assert.Empty(getGroup.Data);
        }
        [Fact]
        public void SearchLikeInFieldAutoCompleteShouldResultData()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            groupRepository.Insert(groupObj);
            //Act
            var getGroup = groupRepository.SearchLikeInFieldAutoComplete("GroupName", groupObj.GroupName);
            //Assert
            Assert.Equal(getGroup[0].Id, groupObj.Id);
        }
        [Fact]
        public void SearchLikeInFieldAutoCompleteShouldResultDataEmpty()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            groupRepository.Insert(groupObj);
            //Act
            var getGroup = groupRepository.SearchLikeInFieldAutoComplete("Name", "NameGroup");
            //Assert
            Assert.True(getGroup.Count == 0);
        }

        [Fact]
        public void UpdateShouldResultObjectUpdated()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            groupRepository.Insert(groupObj);
            var groupUpdate = fixture.Build<Group>().With(x => x.IsDeleted, false).With(x => x.Id, groupObj.Id).Create();
            //Act
            groupRepository.Update(groupObj.Id, groupUpdate);
            var getGroupUpdate = groupRepository.GetById(groupObj.Id);
            //Assert
            Assert.NotEqual(groupObj.GroupName, getGroupUpdate.GroupName);
        }

        [Fact]
        public void UpdateShouldResultNull()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            groupRepository.Insert(groupObj);
            var groupUpdate = fixture.Build<Group>().With(x => x.IsDeleted, false).With(x => x.Id, groupObj.Id).Create();
            //Act
            groupRepository.Update("A43235F3", groupUpdate);
            var getGroupUpdate = groupRepository.GetById(groupObj.Id);
            //Assert
            Assert.Equal(groupObj.GroupName, getGroupUpdate.GroupName);
        }

        [Fact]
        public void SearchFirstByFieldShouldReturnResult()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            groupRepository.Insert(groupObj);
            //Act
            var getGroup = groupRepository.SearchFirstByField("GroupName", groupObj.GroupName);
            //Assert
            Assert.Equal(getGroup.GroupName, groupObj.GroupName);
        }

        [Fact]
        public void SearchFirstByFieldShouldReturnNull()
        {
            //Arrange           
            //Act
            var getGroup = groupRepository.SearchFirstByField("GroupName", "NameGroup");
            //Assert
            Assert.Null(getGroup);
        }
        [Fact]
        public void GetByFieldShouldReturnResult()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            groupRepository.Insert(groupObj);
            //Act
            var getGroup = groupRepository.GetByField("GroupName", groupObj.GroupName);
            //Assert
            Assert.Equal(getGroup.Id, groupObj.Id);
        }

        [Fact]
        public void GetByFieldShouldReturnNull()
        {
            //Arrange           
            //Act
            var getGroup = groupRepository.GetByField("GroupName", "NameGroup");
            //Assert
            Assert.Null(getGroup);
        }

        [Fact]
        public void ExistsShouldReturnTrue()
        {
            //Arrange
            var groupObj = fixture.Build<Group>().With(x => x.IsDeleted, false).Create();
            groupRepository.Insert(groupObj);
            //Act
            var getGroup = groupRepository.Exists(x => x.Id == groupObj.Id);
            //Assert
            Assert.True(getGroup);
        }

        [Fact]
        public void ExistsShouldReturnFalse()
        {
            //Arrange            
            //Act
            var getGroup = groupRepository.Exists(x => x.Id == "Id134512");
            //Assert
            Assert.False(getGroup);
        }
    }
}
