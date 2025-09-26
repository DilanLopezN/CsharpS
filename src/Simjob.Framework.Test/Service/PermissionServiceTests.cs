using AutoFixture;
using Moq;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Identity.Services;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace Simjob.Framework.Test.Services
{
    public class PermissionServiceTests
    {
        private static Fixture fixture;
        private Mock<IPermissionService> permissionServiceMock;
        private Mock<IUserHelper> userHelperMock;
        private PermissionContext dbcontext;
        private Mock<DomainNotificationHandler> notfMock;
        private Mock<IMediatorHandler> busMock;
        private Mock<IRepository<PermissionContext, Permission>> permiRepoMock;
        private Mock<IRepository<MongoDbContext, Schema>> schemaRepoMock;
        private Mock<ISchemaBuilder> schemaBuilderMock;
        private PermissionService permissionService;
        private Mock<ISegmentationService> segmentationServiceMock;
        public PermissionServiceTests()
        {
            fixture = new Fixture();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            permissionServiceMock = new Mock<IPermissionService>();

            userHelperMock = new Mock<IUserHelper>();
            dbcontext = new PermissionContext(userHelperMock.Object);
            notfMock = new Mock<DomainNotificationHandler>();
            busMock = new Mock<IMediatorHandler>();
            permiRepoMock = new Mock<IRepository<PermissionContext, Permission>>();
            schemaRepoMock = new Mock<IRepository<MongoDbContext, Schema>>();
            schemaBuilderMock = new Mock<ISchemaBuilder>();
            segmentationServiceMock = new Mock<ISegmentationService>();
            permissionService = new PermissionService(dbcontext, notfMock.Object, busMock.Object, permiRepoMock.Object, schemaRepoMock.Object, schemaBuilderMock.Object, segmentationServiceMock.Object);
        }

        [Fact]
        public void GetPermissionByIdValid()
        {
            //Arrange
            var id = "a34qwa";
            var permiObj = fixture.Build<Permission>().With(c => c.Id, id).Create();
            permissionServiceMock.Setup(m => m.GetPermissionById(id)).Returns(permiObj);
            //Act
            var permission = permissionServiceMock.Object.GetPermissionById(id);
            //Assert
            Assert.NotNull(permission);
            Assert.Same(permission, permiObj);
            Assert.IsType<Permission>(permission);
        }

        [Fact]
        public void GetPermissionByIdInvalid()
        {
            //Arrange
            var id = "a34qwa";
            var permiObj = fixture.Build<Permission>().With(c => c.Id, id).Create();
            permissionServiceMock.Setup(m => m.GetPermissionById(id)).Returns(permiObj);
            //Act
            var permission = permissionServiceMock.Object.GetPermissionById("a323b");

            //Assert
            Assert.Null(permission);
            Assert.NotSame(permission, permiObj);
            Assert.IsNotType<Permission>(permission);
        }

        [Fact]
        public void GetPermissionByNameValid()
        {
            //Arrange
            var id = "a34qwa";
            var userId = "a324as";
            var permiObj = fixture.Build<Permission>().With(c => c.Id, id).With(x => x.UserID, userId).Create();
            permissionServiceMock.Setup(m => m.GetPermissionByName(userId)).Returns(permiObj);
            //Act
            var permission = permissionServiceMock.Object.GetPermissionByName(userId);
            //Assert
            Assert.NotNull(permission);
            Assert.Same(permission, permiObj);
            Assert.IsType<Permission>(permission);
        }

        [Fact]
        public void GetPermissionByNameInvalid()
        {
            //Arrange
            var id = "a34qwa";
            var userId = "a324as";
            var permiObj = fixture.Build<Permission>().With(c => c.Id, id).With(x => x.UserID, userId).Create();
            permissionServiceMock.Setup(m => m.GetPermissionByName(userId)).Returns(permiObj);
            //Act
            var permission = permissionServiceMock.Object.GetPermissionByName("aqwe1");
            //Assert
            Assert.Null(permission);
            Assert.NotSame(permission, permiObj);
            Assert.IsNotType<Permission>(permission);
        }

        [Fact]
        public void GetPermissionsValid()
        {
            //Arrange
            var id = "a34qwa";
            List<Permission> listPermission = new List<Permission>();
            for (int i = 0; i < 4; i++)
            {
                var permiObj = fixture.Build<Permission>().With(c => c.Id, id).Create();
                listPermission.Add(permiObj);
            }
            permissionServiceMock.Setup(m => m.GetPermissions()).Returns(listPermission);
            //Act
            var result = permissionServiceMock.Object.GetPermissions();

            //Assert
            Assert.NotNull(result);
            Assert.Same(result, listPermission);
            Assert.IsType<List<Permission>>(result);
        }

        [Fact]
        public void GetPermissionsInvalid()
        {
            //Arrange            
            List<Permission> listPermission = new List<Permission>();
            permissionServiceMock.Setup(m => m.GetPermissions()).Returns(listPermission);
            //Act
            var result = permissionServiceMock.Object.GetPermissions();
            //Assert
            Assert.True(result.Count == 0);
        }

        [Fact]
        public void GetPermissionsByGroupValid()
        {
            //Arrange
            var id = "1ad3412";
            var idPer = "2be4523";
            var tenanty = "accist";
            List<Claim> listClaim = new List<Claim>();
            List<User> listUser = new List<User>();
            for (int i = 0; i < 4; i++)
            {
                var userObj = fixture.Build<User>().With(c => c.Id, id).With(x => x.Claims, listClaim).With(x => x.Tenanty, tenanty).Create();
                listUser.Add(userObj);
            }
            List<Permission> listPermission = new List<Permission>();
            for (int i = 0; i < 4; i++)
            {
                var permiObj = fixture.Build<Permission>().With(c => c.Id, idPer).With(x => x.UserID, id).Create();
                listPermission.Add(permiObj);
            }
            permissionServiceMock.Setup(x => x.GetPermissionsByGroup(listUser)).Returns(listPermission);

            //Act
            var result = permissionServiceMock.Object.GetPermissionsByGroup(listUser);

            //Assert
            Assert.True(result.Count > 0);
            Assert.IsType<List<Permission>>(result);
        }

        [Fact]
        public void GetPermissionsByGroupInvalid()
        {
            //Arrange
            var id = "1ad3412";
            var idPer = "2be4523";
            List<Claim> listClaim = new List<Claim>();
            List<User> listUser = new List<User>();
            List<Permission> listPermission = new List<Permission>();
            for (int i = 0; i < 4; i++)
            {
                var permiObj = fixture.Build<Permission>().With(c => c.Id, idPer).With(x => x.UserID, id).Create();
                listPermission.Add(permiObj);
            }
            permissionServiceMock.Setup(x => x.GetPermissionsByGroup(listUser)).Returns(listPermission);

            //Act
            var result = permissionService.GetPermissionsByGroup(listUser);

            //Assert
            Assert.False(result.Count > 0);
            Assert.IsType<List<Permission>>(result);
        }
    }
}