using AutoFixture;
using Moq;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Simjob.Framework.Test.Services
{
    public class GroupServiceTests
    {
        private static Fixture _fixture;
        private Mock<IGroupService> groupServiceMock;
        public GroupServiceTests()
        {
            groupServiceMock = new Mock<IGroupService>();
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());            
        }

        [Fact]
        public void GetGroupsValid()
        {
            //Arrange
            var id = "14325a";
            var groupName = "grouptest";

            List<Group> listGroup = new List<Group>();
            for (int i = 0; i < 4; i++)
            {
                var groupObj = _fixture.Build<Group>().With(c => c.Id, id).With(c => c.GroupName, groupName).With(x => x.IsDeleted, false).Create();
                listGroup.Add(groupObj);
            }
            groupServiceMock.Setup(m => m.GetGroups()).Returns(listGroup);
            //Act
            var groups = groupServiceMock.Object.GetGroups();

            Assert.False(groups.Count == 0);
            Assert.IsType<List<Group>>(groups);
            Assert.NotEmpty(groups);
            Assert.Same(listGroup, groups);
        }

        [Fact]
        public void GetGroupsInvalid()
        {
            //Arrange    
            List<Group> listGroup = new List<Group>();
            groupServiceMock.Setup(m => m.GetGroups()).Returns(listGroup);
            //Act
            var groups = groupServiceMock.Object.GetGroups();
            //Arrange
            Assert.True(groups.Count == 0);
            Assert.Empty(groups);
        }

        [Fact]
        public void GetGroupByIdValid()
        {
            //Arrange
            var id = "14325a";
            var groupObj = _fixture.Build<Group>().With(c => c.Id, id).With(x => x.IsDeleted, false).Create();

            groupServiceMock.Setup(m => m.GetGroupById(id)).Returns(groupObj);

            //Act
            var group = groupServiceMock.Object.GetGroupById(id);

            //Assert

            Assert.Same(group, groupObj);
            Assert.NotNull(group);
            Assert.IsType<Group>(group);
        }

        [Fact]
        public void GetGroupByIdInvalid()
        {
            //Arrange
            var id = "14325a";
            var groupObj = _fixture.Build<Group>().With(c => c.Id, id).With(x => x.IsDeleted, false).Create();

            groupServiceMock.Setup(m => m.GetGroupById(id)).Returns(groupObj);

            //Act
            var group = groupServiceMock.Object.GetGroupById("123a");

            //Assert

            Assert.NotSame(group, groupObj);
            Assert.Null(group);
            Assert.IsNotType<Group>(group);
        }

        [Fact]
        public void GetGroupByNameValid()
        {
            //Arrange
            var id = "14325a";
            var groupName = "grouptest";
            var groupObj = _fixture.Build<Group>().With(c => c.Id, id).With(x => x.IsDeleted, false).With(x => x.GroupName, groupName).Create();

            groupServiceMock.Setup(m => m.GetGroupByName(groupName)).Returns(groupObj);

            //Act
            var group = groupServiceMock.Object.GetGroupByName(groupName);

            //Assert

            Assert.Same(group, groupObj);
            Assert.NotNull(group);
            Assert.IsType<Group>(group);
        }

        [Fact]
        public void GetGroupByNameInvalid()
        {
            //Arrange
            var id = "14325a";
            var groupName = "grouptest";
            var groupObj = _fixture.Build<Group>().With(c => c.Id, id).With(x => x.IsDeleted, false).With(x => x.GroupName, groupName).Create();

            groupServiceMock.Setup(m => m.GetGroupByName(groupName)).Returns(groupObj);

            //Act
            var group = groupServiceMock.Object.GetGroupByName("group");
            //Assert

            Assert.NotSame(group, groupObj);
            Assert.Null(group);
            Assert.IsNotType<Group>(group);
        }

        [Fact]
        public void GetGroupByNamePaginadaValid()
        {
            //Arrange            
            var groupName = "grouptest";
            var groupObj = _fixture.Build<PaginationData<Group>>().Create();
            groupServiceMock.Setup(m => m.GetGroupByNamePaginada(groupName, 1, 1, null)).Returns(groupObj);
            //Act
            var group = groupServiceMock.Object.GetGroupByNamePaginada(groupName, 1, 1);
            //Assert
            Assert.Same(group.Data, groupObj.Data);
            Assert.NotNull(group);
        }

        [Fact]
        public void GetGroupByNamePaginadaInvalid()
        {
            //Arrange            
            var groupName = "grouptest";
            var groupObj = _fixture.Build<PaginationData<Group>>().Create();
            groupServiceMock.Setup(m => m.GetGroupByNamePaginada(groupName, 1, 1,null)).Returns(groupObj);
            //Act
            var group = groupServiceMock.Object.GetGroupByNamePaginada("invalidGroup", 1, 1);
            //Assert            
            Assert.Null(group);
        }
    }
}