using AutoFixture;
using Moq;
using Newtonsoft.Json;
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
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Simjob.Framework.Test.Services
{
    public class UserServiceTests
    {
        private static Fixture fixture;
        private Mock<IMediatorHandler> busMock;
        private Mock<DomainNotificationHandler> notfMock;
        private Mock<IRepository<IdentityContext, User>> userRepoMock;
        private Mock<IRepository<MongoDbContext, Schema>> schemaRepoMock;
        private Mock<ISchemaBuilder> schemaBuilderMock;
        private Mock<IServiceProvider> serviceProviderMock;
        private Mock<IUserService> userServiceMock;
        private Mock<IUserHelper> userHelperMock;
        private IdentityContext dbcontext;
        private UserService userService;

        public UserServiceTests()
        {
            busMock = new Mock<IMediatorHandler>();
            notfMock = new Mock<DomainNotificationHandler>();
            userRepoMock = new Mock<IRepository<IdentityContext, User>>();
            schemaRepoMock = new Mock<IRepository<MongoDbContext, Schema>>();
            schemaBuilderMock = new Mock<ISchemaBuilder>();
            serviceProviderMock = new Mock<IServiceProvider>();
            userServiceMock = new Mock<IUserService>();
            userHelperMock = new Mock<IUserHelper>();
            dbcontext = new IdentityContext(userHelperMock.Object);
            userService = new UserService(dbcontext, notfMock.Object, busMock.Object, userRepoMock.Object, schemaRepoMock.Object, schemaBuilderMock.Object, serviceProviderMock.Object);
            fixture = new Fixture();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            EnvironmentSettings.Flush();
        }

        #region FailAndSuccessTests

        [Fact]
        public void RegisterUserReturnFalse()
        {
            //Arrange

            var id = "a1234567894av";
            var passw = "";
            var tenanty = "accist";
            //var UserName = "akwwn4@gmail.com";
            List<Claim> claims = new List<Claim>();
            var newUser = fixture.Build<User>().With(c => c.Id, id).With(x => x.IsDeleted, false).With(x => x.Tenanty, tenanty).With(x => x.Claims, claims).Create();
            //Act

            var result = userService.Register(newUser, passw);
            //Assert
            Assert.False(result);
        }

        [Fact]
        public void RegisterUserReturnTrue()
        {
            //Arrange

            var id = "a1234567894av";
            var passw = "admin123*";
            var tenanty = "accist";
            //var UserName = "akwwn4@gmail.com";
            List<Claim> claims = new List<Claim>();
            var newUser = fixture.Build<User>().With(c => c.Id, id).With(x => x.IsDeleted, false).With(x => x.Tenanty, tenanty).With(x => x.Claims, claims).Create();
            //Act

            var result = userService.Register(newUser, passw);
            //Assert
            Assert.True(result);
        }

        [Fact]
        public void GetUserByIdValidUser()
        {
            //Arrange

            var id = "1ad3412";
            List<Claim> listClaim = new List<Claim>();
            var userObj = fixture.Build<User>().With(c => c.Id, id).With(x => x.Claims, listClaim).Create();

            userServiceMock.Setup(m => m.GetUserById(id)).Returns(userObj);

            //Act

            var user = userServiceMock.Object.GetUserById(id);

            //Assert

            var obj1Str = JsonConvert.SerializeObject(user);
            var obj2Str = JsonConvert.SerializeObject(userObj);

            Assert.NotNull(user);
            Assert.Equal(obj1Str, obj2Str);
            Assert.IsType<User>(user);
        }

        [Fact]
        public void GetUserByIdInvalidUser()
        {
            //Arrange
            var id = "1ad3412";
            List<Claim> listClaim = new List<Claim>();
            var userObj = fixture.Build<User>().With(c => c.Id, id).With(x => x.Claims, listClaim).Create();

            userServiceMock.Setup(m => m.GetUserById(id)).Returns(userObj);

            //Act

            var user = userServiceMock.Object.GetUserById("a321");

            //Assert

            var obj1Str = JsonConvert.SerializeObject(user);
            var obj2Str = JsonConvert.SerializeObject(userObj);

            Assert.Null(user);
            Assert.NotEqual(obj1Str, obj2Str);
            Assert.IsNotType<User>(user);
        }

        [Fact]
        public void GetAllUserByTenantyValid()
        {
            //Arrange
            var id = "1ad3412";
            var tenanty = "accist";
            List<Claim> listClaim = new List<Claim>();
            List<User> listUser = new List<User>();
            for (int i = 0; i < 4; i++)
            {
                var userObj = fixture.Build<User>().With(c => c.Id, id).With(x => x.Claims, listClaim).With(x => x.Tenanty, tenanty).Create();
                listUser.Add(userObj);
            }
            userServiceMock.Setup(m => m.GetUsersByTenanty(tenanty)).Returns(listUser);

            //Act

            var result = userServiceMock.Object.GetUsersByTenanty(tenanty);

            //Assert

            Assert.False(result.Count == 0);
            Assert.IsType<List<User>>(result);
            Assert.NotEmpty(result);
            Assert.Same(listUser, result);
        }

        [Fact]
        public void GetAllUserByTenantyInvalid()
        {
            //Arrange

            var id = "1ad3412";
            var tenanty = "accist";
            List<Claim> listClaim = new List<Claim>();
            List<User> listUser = new List<User>();
            for (int i = 0; i < 4; i++)
            {
                var userObj = fixture.Build<User>().With(c => c.Id, id).With(x => x.Claims, listClaim).With(x => x.Tenanty, tenanty).Create();
                listUser.Add(userObj);
            }
            userServiceMock.Setup(m => m.GetUsersByTenanty(tenanty)).Returns(listUser);

            //Act

            var result = userServiceMock.Object.GetUsersByTenanty("");

            //Assert

            Assert.Null(result);
            Assert.IsNotType<List<User>>(result);
            Assert.NotSame(listUser, result);
        }

        [Fact]
        public void GetUserByUserNameValid()
        {
            //Arrange
            var tenanty = "deltaducon";
            var userName = "admin@admin.com";

            //Act
            var user = userService.GetUserByUserName(userName, tenanty);

            //Assert
            Assert.IsType<User>(user);
            Assert.NotNull(user);
        }

        [Fact]
        public void GetUserByUserNameInvalid()
        {
            //Arrange
            var tenanty = "accist";
            var userName = "";

            //Act
            var user = userService.GetUserByUserName(userName, tenanty);

            //Assert
            Assert.IsNotType<User>(user);
            Assert.Null(user);
        }

        [Fact]
        public void GetByUserNameValid()
        {
            //Arrange
            var tenanty = "accist";
            var userName = "admin@admin.com";

            //Act
            var user = userService.GetByUserName(tenanty, userName);

            //Assert
            Assert.IsType<User>(user);
            Assert.NotNull(user);
        }

        [Fact]
        public void GetByUserNameInValid()
        {
            //Arrange
            var tenanty = "accist";
            var userName = "";

            //Act
            var user = userService.GetByUserName(tenanty, userName);

            //Assert
            Assert.IsNotType<User>(user);
            Assert.Null(user);
        }

        [Fact]
        public void LoginSucess()
        {
            //Arrange
            var userName = "testes1234a@gmail.com";
            var tenanty = "deltaducon";
            var password = "admin123*";

            //Act
            var login = userService.Login(tenanty, userName, password);

            //Assert
            Assert.True(login);
        }

        [Fact]
        public void LoginFailure()
        {
            //Arrange
            var tenanty = "accist";
            var password = "admin@admin123*";

            List<Claim> listClaim = new List<Claim>();
            var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.Tenanty, tenanty).Create();

            userServiceMock.Setup(x => x.Register(userObj, password)).Returns(true);
            userServiceMock.Setup(x => x.GetByUserName(tenanty, userObj.UserName)).Returns(userObj);
            var result = userServiceMock.Object.Register(userObj, password);
            //Act

            var login = userService.Login(tenanty, userObj.UserName, password);

            //Assert
            Assert.False(login);
        }

        [Fact]
        public void GetUsersByGroupIdValid()
        {
            //Arrange
            var groupId = "295b1e41-737b-4dee-b161-8e452ebb368c";

            //Act
            var listUser = userService.GetUsersByGroupId(groupId);

            //Assert
            Assert.False(listUser.Count == 0);
            Assert.IsType<List<User>>(listUser);
            Assert.NotEmpty(listUser);
            Assert.NotNull(listUser);
        }

        [Fact]
        public void GetUsersByGroupIdInvalid()
        {
            //Arrange
            var groupId = "groupId745as";

            //Act
            var listUser = userService.GetUsersByGroupId(groupId);

            //Assert
            Assert.True(listUser.Count == 0);
            Assert.IsType<List<User>>(listUser);
        }

        #endregion FailAndSuccessTests

        #region FailureTests

        [Fact]
        public void ResetPasswordFailure()
        {
            //Arrange
            var id = "aacd8aff-a24e-4076-ae1e-9701ff4a3383";
            var user = userService.GetUserById(id);
            var newPassw = "admin123*";
            //Act
            userService.ResetPassword(user, newPassw);
            var userEdit = userService.GetUserById(id);

            //Assert
            Assert.True(user.Hash == userEdit.Hash);
        }

        [Fact]
        public void UpdatePasswordFailure()
        {
            //Arrange
            var id = "aacd8aff-a24e-4076-ae1e-9701ff4a3383";
            var user = userService.GetUserById(id);
            var oldPassw = "admin12345*";
            var newPassw = "admin123*";

            //Act
            userService.UpdatePassword(user, oldPassw, newPassw);
            var userEdit = userService.GetUserById(id);

            //Assert
            Assert.True(user.Hash == userEdit.Hash);
        }

        [Fact]
        public void UpdatePasswordAdminFailure()
        {
            //Arrange
            var id = "aacd8aff-a24e-4076-ae1e-9701ff4a3383";
            var user = userService.GetUserById(id);
            var newPassw = "admin123*";

            //Act
            userService.UpdatePasswordAdmin(user, newPassw);
            var userEdit = userService.GetUserById(id);

            //Assert
            Assert.True(user.Hash == userEdit.Hash);
        }

        [Fact]
        public void UpdateUserNameFailure()
        {
            //Arrange
            var id = "aacd8aff-a24e-4076-ae1e-9701ff4a3383";
            var user = userService.GetUserById(id);
            var newUserName = "testes1234a@gmail.com";

            //Act
            userService.UpdateUserName(user, newUserName);
            var userEdit = userService.GetUserById(id);

            Assert.True(user.UserName == userEdit.UserName);
        }

        [Fact]
        public void UpdateRootFailure()
        {
            //Arrange
            var id = "aacd8aff-a24e-4076-ae1e-9701ff4a3383";
            var user = userService.GetUserById(id);
            var root = false;
            //Act
            userService.UpdateRoot(user, root);
            var userEdit = userService.GetUserById(id);
            //Asert

            Assert.True(user.Root == userEdit.Root);
        }

        [Fact]
        public void EnableOrDisableA2FFailure()
        {
            //Arrange
            var id = "aacd8aff-a24e-4076-ae1e-9701ff4a3383";
            var user = userService.GetUserById(id);
            var a2f = 0;
            //Act
            userService.EnableOrDisableA2F(user, a2f);
            var userEdit = userService.GetUserById(id);
            //Assert
            Assert.False(user.A2f != userEdit.A2f);            
        }

        #endregion FailureTests
    }
}