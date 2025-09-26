using AutoFixture;
using Moq;
using Newtonsoft.Json;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Infra.Schemas.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Simjob.Framework.Test.Services
{
    public class EntityServiceTests
    {
        private static Fixture fixture;

        private Mock<IUserHelper> userHelperMock;
        private Mock<ISchemaBuilder> schemaBuilderMock;
        private Mock<IServiceProvider> serviceProviderMock;
        private Mock<IAzureStorageService> azureStorageMock;
        private Mock<IHuaweiStorageService> huaweiStorageMock;
        private Mock<IAWSStorageService> awsStorageMock;
        private Mock<IRepository<MongoDbContext, Schema>> schemaRepoMock;
        private Mock<DomainNotificationHandler> notfMock;
        private Mock<IMediatorHandler> busMock;
        private EntityService entityService;

        public EntityServiceTests()
        {
            busMock = new Mock<IMediatorHandler>();
            notfMock = new Mock<DomainNotificationHandler>();
            schemaRepoMock = new Mock<IRepository<MongoDbContext, Schema>>();
            schemaBuilderMock = new Mock<ISchemaBuilder>();
            serviceProviderMock = new Mock<IServiceProvider>();
            userHelperMock = new Mock<IUserHelper>();
            azureStorageMock = new Mock<IAzureStorageService>();
            huaweiStorageMock = new Mock<IHuaweiStorageService>();
            awsStorageMock = new Mock<IAWSStorageService>();
            entityService = new EntityService(schemaBuilderMock.Object, serviceProviderMock.Object, schemaRepoMock.Object, busMock.Object, notfMock.Object, azureStorageMock.Object, huaweiStorageMock.Object, awsStorageMock.Object);
            fixture = new Fixture();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            EnvironmentSettings.Flush();
        }

        #region FailAndSuccessTests

        [Fact]
        public void GetSchemaByNameReturnResult()
        {
            //Arrange
            List<Claim> claims = new List<Claim>();
            var schema = fixture.Build<Schema>().With(c => c.Name, "Account").With(x => x.IsDeleted, false).Create();

            schemaRepoMock.Setup(x => x.GetByField("name", schema.Name)).Returns(schema);

            //Act
            var result = entityService.GetSchemaByName(schema.Name);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(JsonConvert.SerializeObject(schema), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public void GetSchemaByNameReturnNull()
        {
            //Arrange
            List<Claim> claims = new List<Claim>();
            var schema = fixture.Build<Schema>().With(c => c.Name, "Account").With(x => x.IsDeleted, false).Create();

            schemaRepoMock.Setup(x => x.GetByField("name", schema.Name)).Returns(schema);

            //Act
            var result = entityService.GetSchemaByName("invalid");

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetRepositoryReturnAnyObject()
        {
            //Arrange
            List<Claim> claims = new List<Claim>();
            var schema = fixture.Build<Schema>().With(c => c.Name, "Account").With(x => x.IsDeleted, false).Create();
            Task<Type> type = Task.Run(() => schema.GetType());
            var ok = schema.GetType();

            var objectT = fixture.Build<object>().Create();

            schemaBuilderMock.Setup(x => x.GetSchemaType(schema.Name, false)).Returns(type);
            var typeRepo = typeof(IRepository<,>).MakeGenericType(typeof(MongoDbContext), ok);
            serviceProviderMock.Setup(x => x.GetService(typeRepo)).Returns(objectT);

            //Act
            var result = entityService.GetRepository(ok);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetRepositoryReturnNull()
        {
            //Arrange
            List<Claim> claims = new List<Claim>();
            var schema = fixture.Build<Schema>().With(c => c.Name, "Account").With(x => x.IsDeleted, false).Create();
            Task<Type> type = Task.Run(() => schema.GetType());
            var ok = schema.GetType();

            var objectT = fixture.Build<object>().Create();

            schemaBuilderMock.Setup(x => x.GetSchemaType(schema.Name, false)).Returns(type);
            var typeRepo = typeof(IRepository<,>).MakeGenericType(typeof(MongoDbContext), ok);
            serviceProviderMock.Setup(x => x.GetService(typeRepo)).Returns((object)null);

            //Act
            var result = entityService.GetRepository(ok);

            //Assert
            Assert.Null(result);
        }

        //[Fact]
        //public async Task GetByFieldName()
        //{
        //    //Arrange
        //    List<Claim> claims = new List<Claim>();
        //    var schema = fixture.Build<Schema>().With(c => c.Name, "Account").With(x => x.IsDeleted, false).Create();
        //    //var schemaType = fixture.Build<Type>().Create();
        //    Task<Type> type = Task.Run(() => schema.GetType());


        //    //schemaRepoMock.Setup(x => x.GetByField("name", schema.Name)).Returns(type);

        //    schemaBuilderMock.Setup(x => x.GetSchemaType(schema.Name, false)).Returns(type);
        //    var typeRepo = typeof(IRepository<,>).MakeGenericType(typeof(MongoDbContext), schema.GetType());
        //    serviceProviderMock.Setup(x => x.GetService(typeRepo));
        //    //Act
        //    var result = entityService.GetByFieldName(schema.Name, "name", schema.Name);

        //    //Assert
        //    Assert.Null(result);
        //}




        #endregion

        //[Fact]
        //public async Task RegisterUserReturnTrue()
        //{
        //    //Arrange

        //    var id = "a1234567894av";
        //    var passw = "admin123*";
        //    var tenanty = "accist";
        //    //var UserName = "akwwn4@gmail.com";
        //    List<Claim> claims = new List<Claim>();
        //    var newUser = fixture.Build<User>().With(c => c.Id, id).With(x => x.IsDeleted, false).With(x => x.Tenanty, tenanty).With(x => x.Claims, claims).Create();
        //    //Act

        //    var result = userService.Register(newUser, passw);
        //    //Assert
        //    Assert.True(result);
        //}

        //[Fact]
        //public async Task GetUserByIdValidUser()
        //{
        //    //Arrange

        //    var id = "1ad3412";
        //    List<Claim> listClaim = new List<Claim>();
        //    var userObj = fixture.Build<User>().With(c => c.Id, id).With(x => x.Claims, listClaim).Create();

        //    userServiceMock.Setup(m => m.GetUserById(id)).Returns(userObj);

        //    //Act

        //    var user = userServiceMock.Object.GetUserById(id);

        //    //Assert

        //    var obj1Str = JsonConvert.SerializeObject(user);
        //    var obj2Str = JsonConvert.SerializeObject(userObj);

        //    Assert.NotNull(user);
        //    Assert.Equal(obj1Str, obj2Str);
        //    Assert.IsType<User>(user);
        //}

        //[Fact]
        //public async Task GetUserByIdInvalidUser()
        //{
        //    //Arrange
        //    var id = "1ad3412";
        //    List<Claim> listClaim = new List<Claim>();
        //    var userObj = fixture.Build<User>().With(c => c.Id, id).With(x => x.Claims, listClaim).Create();

        //    userServiceMock.Setup(m => m.GetUserById(id)).Returns(userObj);

        //    //Act

        //    var user = userServiceMock.Object.GetUserById("a321");

        //    //Assert

        //    var obj1Str = JsonConvert.SerializeObject(user);
        //    var obj2Str = JsonConvert.SerializeObject(userObj);

        //    Assert.Null(user);
        //    Assert.NotEqual(obj1Str, obj2Str);
        //    Assert.IsNotType<User>(user);
        //}

        //[Fact]
        //public async Task GetAllUserByTenantyValid()
        //{
        //    //Arrange
        //    var id = "1ad3412";
        //    var tenanty = "accist";
        //    List<Claim> listClaim = new List<Claim>();
        //    List<User> listUser = new List<User>();
        //    for (int i = 0; i < 4; i++)
        //    {
        //        var userObj = fixture.Build<User>().With(c => c.Id, id).With(x => x.Claims, listClaim).With(x => x.Tenanty, tenanty).Create();
        //        listUser.Add(userObj);
        //    }
        //    userServiceMock.Setup(m => m.GetUsersByTenanty(tenanty)).Returns(listUser);

        //    //Act

        //    var result = userServiceMock.Object.GetUsersByTenanty(tenanty);

        //    //Assert

        //    Assert.False(result.Count == 0);
        //    Assert.IsType<List<User>>(result);
        //    Assert.NotEmpty(result);
        //    Assert.Same(listUser, result);
        //}

        //[Fact]
        //public async Task GetAllUserByTenantyInvalid()
        //{
        //    //Arrange

        //    var id = "1ad3412";
        //    var tenanty = "accist";
        //    List<Claim> listClaim = new List<Claim>();
        //    List<User> listUser = new List<User>();
        //    for (int i = 0; i < 4; i++)
        //    {
        //        var userObj = fixture.Build<User>().With(c => c.Id, id).With(x => x.Claims, listClaim).With(x => x.Tenanty, tenanty).Create();
        //        listUser.Add(userObj);
        //    }
        //    userServiceMock.Setup(m => m.GetUsersByTenanty(tenanty)).Returns(listUser);

        //    //Act

        //    var result = userServiceMock.Object.GetUsersByTenanty("");

        //    //Assert

        //    Assert.Null(result);
        //    Assert.IsNotType<List<User>>(result);
        //    Assert.NotSame(listUser, result);
        //}

        //[Fact]
        //public async Task GetUserByUserNameValid()
        //{
        //    //Arrange
        //    var tenanty = "deltaducon";
        //    var userName = "admin@admin.com";

        //    //Act
        //    var user = userService.GetUserByUserName(userName, tenanty);

        //    //Assert
        //    Assert.IsType<User>(user);
        //    Assert.NotNull(user);
        //}

        //[Fact]
        //public async Task GetUserByUserNameInvalid()
        //{
        //    //Arrange
        //    var tenanty = "accist";
        //    var userName = "";

        //    //Act
        //    var user = userService.GetUserByUserName(userName, tenanty);

        //    //Assert
        //    Assert.IsNotType<User>(user);
        //    Assert.Null(user);
        //}

        //[Fact]
        //public async Task GetByUserNameValid()
        //{
        //    //Arrange
        //    var tenanty = "accist";
        //    var userName = "admin@admin.com";

        //    //Act
        //    var user = userService.GetByUserName(tenanty, userName);

        //    //Assert
        //    Assert.IsType<User>(user);
        //    Assert.NotNull(user);
        //}

        //[Fact]
        //public async Task GetByUserNameInValid()
        //{
        //    //Arrange
        //    var tenanty = "accist";
        //    var userName = "";

        //    //Act
        //    var user = userService.GetByUserName(tenanty, userName);

        //    //Assert
        //    Assert.IsNotType<User>(user);
        //    Assert.Null(user);
        //}

        //[Fact]
        //public async Task LoginSucess()
        //{
        //    //Arrange
        //    var userName = "admin@admin.com";
        //    var tenanty = "accist";
        //    var password = "admin123*";

        //    //Act
        //    var login = userService.Login(tenanty, userName, password);

        //    //Assert
        //    Assert.True(login);
        //}

        //[Fact]
        //public async Task LoginFailure()
        //{
        //    //Arrange
        //    var tenanty = "accist";
        //    var username = "admin@admin.com.br";
        //    var password = "admin@admin123*";

        //    List<Claim> listClaim = new List<Claim>();
        //    var userObj = fixture.Build<User>().With(x => x.Claims, listClaim).With(x => x.Tenanty, tenanty).Create();

        //    userServiceMock.Setup(x => x.Register(userObj, password)).Returns(true);
        //    userServiceMock.Setup(x => x.GetByUserName(tenanty, userObj.UserName)).Returns(userObj);
        //    var result = userServiceMock.Object.Register(userObj, password);
        //    //Act

        //    var login = userService.Login(tenanty, userObj.UserName, password);

        //    //Assert
        //    Assert.False(login);
        //}

        //[Fact]
        //public async Task GetUsersByGroupIdValid()
        //{
        //    //Arrange
        //    var groupId = "295b1e41-737b-4dee-b161-8e452ebb368c";

        //    //Act
        //    var listUser = userService.GetUsersByGroupId(groupId);

        //    //Assert
        //    Assert.False(listUser.Count == 0);
        //    Assert.IsType<List<User>>(listUser);
        //    Assert.NotEmpty(listUser);
        //    Assert.NotNull(listUser);
        //}

        //[Fact]
        //public async Task GetUsersByGroupIdInvalid()
        //{
        //    //Arrange
        //    var groupId = "groupId745as";

        //    //Act
        //    var listUser = userService.GetUsersByGroupId(groupId);

        //    //Assert
        //    Assert.True(listUser.Count == 0);
        //    Assert.IsType<List<User>>(listUser);
        //}
        //#endregion FailAndSuccessTests

        #region FailureTests

        //[Fact]
        //public async Task ResetPasswordFailure()
        //{
        //    //Arrange
        //    var id = "aacd8aff-a24e-4076-ae1e-9701ff4a3383";
        //    var user = userService.GetUserById(id);
        //    var newPassw = "admin123*";

        //    //Act
        //    userService.ResetPassword(user, newPassw);
        //    var userEdit = userService.GetUserById(id);

        //    //Assert
        //    Assert.True(user.Hash == userEdit.Hash);
        //}

        //[Fact]
        //public async Task UpdatePasswordFailure()
        //{
        //    //Arrange
        //    var id = "aacd8aff-a24e-4076-ae1e-9701ff4a3383";
        //    var user = userService.GetUserById(id);
        //    var oldPassw = "admin12345*";
        //    var newPassw = "admin123*";

        //    //Act
        //    userService.UpdatePassword(user, oldPassw, newPassw);
        //    var userEdit = userService.GetUserById(id);

        //    //Assert
        //    Assert.True(user.Hash == userEdit.Hash);
        //}

        //[Fact]
        //public async Task UpdatePasswordAdminFailure()
        //{
        //    //Arrange
        //    var id = "aacd8aff-a24e-4076-ae1e-9701ff4a3383";
        //    var user = userService.GetUserById(id);
        //    var oldPassw = "admin12345*";
        //    var newPassw = "admin123*";

        //    //Act
        //    userService.UpdatePasswordAdmin(user, newPassw);
        //    var userEdit = userService.GetUserById(id);

        //    //Assert
        //    Assert.True(user.Hash == userEdit.Hash);
        //}

        //[Fact]
        //public async Task UpdateUserNameFailure()
        //{
        //    //Arrange
        //    var id = "aacd8aff-a24e-4076-ae1e-9701ff4a3383";
        //    var user = userService.GetUserById(id);
        //    var newUserName = "testes1234a@gmail.com";

        //    //Act
        //    userService.UpdateUserName(user, newUserName);
        //    var userEdit = userService.GetUserById(id);

        //    Assert.True(user.UserName == userEdit.UserName);
        //}

        //[Fact]
        //public async Task UpdateRootFailure()
        //{
        //    //Arrange
        //    var id = "aacd8aff-a24e-4076-ae1e-9701ff4a3383";
        //    var user = userService.GetUserById(id);
        //    var root = false;
        //    //Act
        //    userService.UpdateRoot(user, root);
        //    var userEdit = userService.GetUserById(id);
        //    //Asert

        //    Assert.True(user.Root == userEdit.Root);
        //}

        //[Fact]
        //public async Task EnableOrDisableA2FFailure()
        //{
        //    //Arrange
        //    var id = "aacd8aff-a24e-4076-ae1e-9701ff4a3383";
        //    var user = userService.GetUserById(id);
        //    var a2f = 0;
        //    //Act
        //    userService.EnableOrDisableA2F(user, a2f);
        //    var userEdit = userService.GetUserById(id);
        //    //Assert
        //    Assert.False(user.A2f != userEdit.A2f);
        //}

        #endregion FailureTests
    }
}