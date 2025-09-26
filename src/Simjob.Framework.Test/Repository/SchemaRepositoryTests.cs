//using AutoFixture;
//using MongoDB.Driver;
//using Moq;
//using Simjob.Framework.Domain.Interfaces.Repositories;
//using Simjob.Framework.Domain.Interfaces.Users;
//using Simjob.Framework.Domain.Util;
//using Simjob.Framework.Infra.Data.Context;
//using Simjob.Framework.Infra.Data.Repositories;
//using Simjob.Framework.Infra.Identity.Entities;
//using Simjob.Framework.Infra.Identity.Repositories;
//using Simjob.Framework.Infra.Schemas.Entities;
//using Simjob.Framework.Test.Services.Api;
//using System;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using System.Security.Claims;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;
//using static Simjob.Framework.Domain.Models.Searchdefs;

//namespace Simjob.Framework.Test.Repository
//{
//    public class SchemaRepositoryTest
//    {
//        private static Fixture fixture;
//        private Mock<IMongoCollection<User>> _mockCollection;
//        private Mock<IUserHelper> _userHelperMock;
//        private UserRepository _userRepository;
//        private TDDContext _mockContext;
//        private User _user;
//        public static List<User> _list = new();

//        public SchemaRepositoryTest()
//        {
//            fixture = new Fixture();
//            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
//            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
//            List<Claim> listClaim = new();
//            _user = fixture.Build<User>().With(x => x.IsDeleted, false).With(x => x.Claims, listClaim).Create();
//            _mockCollection = new Mock<IMongoCollection<User>>();
//            _mockCollection.Object.InsertOne(_user);
//            _userHelperMock = new Mock<IUserHelper>();
//            _mockContext = new TDDContext(_userHelperMock.Object);
//            _mockContext.GetUserCollection("user");
//            _userRepository = new UserRepository(_mockContext, _userHelperMock.Object, "test");

//            _list.Add(_user);

//            Mock<IAsyncCursor<User>> _Cursor = new();
//            _Cursor.Setup(_ => _.Current).Returns(_list);
//            _Cursor
//                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
//                .Returns(true)
//                .Returns(false);

//            //Mock FindSync
//            _mockCollection.Setup(op => op.FindSync(It.IsAny<FilterDefinition<User>>(),
//            It.IsAny<FindOptions<User, User>>(),
//             It.IsAny<CancellationToken>())).Returns(_Cursor.Object);
//        }

//        [Fact]
//        public async Task RepositoryInsertNewSchemaShouldReturnSchemaCreatedAsync()
//        {
//            var schema = fixture.Build<Schema>().With(x => x.Id, "").With(x => x.IsDeleted, false).Create();
//            var holy = fixture.Build<PaginationData<Schema>>().Create();

//            var ok = fixture.Build<Dictionary<string, object>>().Create();

//            // Arrange
//            var mockRepository = new Mock<IRepository<MongoDbContext, Schema>>();
//            var mockHelper = new Mock<IUserHelper>();
//            mockRepository.Setup(x => x.Insert(schema, null, false)).Returns(schema);
//            //mockRepository.Setup(x => x.Insert(schema)).Returns(schema);

//            // Act

//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");
//            await repository.Insert(schema);

//            var result = repository.GetById(schema.Id);

//            // Assert

//            Assert.NotNull(result);
//            Assert.Equal(schema.Id, result.Id);
//            Assert.Equal(schema.Name, result.Name);
//        }

//        [Fact]
//        public void RepositoryInsertNewSchemaShouldReturnAllResult()
//        {
//            var schema = fixture.Build<Schema>().With(x => x.IsDeleted, false).Create();
//            var holy = fixture.Build<PaginationData<Schema>>().Create();

//            var ok = fixture.Build<Dictionary<string, object>>().Create();

//            // Arrange
//            var mockRepository = new Mock<IRepository<MongoDbContext, Schema>>();
//            var mockHelper = new Mock<IUserHelper>();
//            mockRepository.Setup(x => x.Insert(schema, null, false)).Returns(schema);

//            // Act

//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");

//            var result = repository.GetAll();

//            // Assert

//            var find = false;
//            if (result.Data != null) find = true;

//            Assert.NotNull(result);
//            Assert.True(find);
//        }

//        [Fact]
//        public async Task RepositoryGetAllShouldReturnEmptyDataAsync()
//        {
//            var schema = fixture.Build<Schema>().With(x => x.IsDeleted, false).Create();
//            var holy = fixture.Build<PaginationData<Schema>>().Create();

//            var ok = fixture.Build<Dictionary<string, object>>().Create();

//            // Arrange
//            var mockRepository = new Mock<IRepository<MongoDbContext, Schema>>();
//            var mockHelper = new Mock<IUserHelper>();
//            await mockRepository.Setup(x => x.Insert(schema, null, false)).Returns(schema);

//            // Act

//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema123");

//            var result = repository.GetAll(1, 1, "name");

//            Assert.Empty(result.Data);
//        }

//        [Fact]
//        public void RepositorySearchFirstByFieldShouldReturnResults()
//        {
//            var schema = fixture.Build<Schema>().With(x => x.IsDeleted, false).Create();
//            var holy = fixture.Build<PaginationData<Schema>>().Create();

//            var ok = fixture.Build<Dictionary<string, object>>().Create();

//            // Arrange
//            var mockRepository = new Mock<IRepository<MongoDbContext, Schema>>();
//            var mockHelper = new Mock<IUserHelper>();
//            mockRepository.Setup(x => x.SearchFirstByField("name", "Namebc30884f-cf4b-4dee-983e-9c585a2f047e", false))
//                .Returns((Schema)null);

//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");

//            // Act

//            repository.Insert(schema);

//            var result = repository.SearchFirstByField("name", schema.Name, false);

//            // Assert

//            Assert.NotNull(result);
//            Assert.Equal(schema.Name, result.Name);
//        }

//        [Fact]
//        public void RepositorySearchFirstByFieldShouldReturnNotFound()
//        {
//            // Arrange
//            var schema = fixture.Build<Schema>().With(x => x.IsDeleted, false).Create();
//            var holy = fixture.Build<PaginationData<Schema>>().Create();

//            var ok = fixture.Build<Dictionary<string, object>>().Create();

//            var mockRepository = new Mock<IRepository<MongoDbContext, Schema>>();
//            var mockHelper = new Mock<IUserHelper>();
//            mockRepository.Setup(x => x.SearchFirstByField("name", "Namebc30884f-cf4b-4dee-983e-9c585a2f047e", false))
//                .Returns((Schema)null);

//            // Act

//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");

//            var result = repository.SearchFirstByField("name", "nwww123", false);

//            // Assert

//            Assert.Null(result);
//        }

//        [Fact]
//        public async Task RepositoryUpdateShouldReturnObjectUpdatedAsync()
//        {
//            // Arrange
//            var schema = fixture.Build<Schema>().With(x => x.IsDeleted, false).Create();
//            var holy = fixture.Build<PaginationData<Schema>>().Create();

//            var ok = fixture.Build<Dictionary<string, object>>().Create();

//            var mockHelper = new Mock<IUserHelper>();
//            //var repository = new FakeRepository(_mockContext, mockHelper.Object, "schema");
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");
//            var schemaToUpdate = await repository.Insert(schema);

//            var mockRepository = new Mock<IRepository<MongoDbContext, Schema>>();
//            mockRepository.Setup(x => x.Update(schemaToUpdate, null, false))
//                .Returns((Schema)null);

//            // Act

//            schemaToUpdate.Name = "NovoName";

//            var result = repository.Update(schemaToUpdate);

//            // Assert

//            Assert.Equal(schemaToUpdate.Name, result.Name);
//        }

//        [Fact]
//        public void RepositoryUpdateShouldReturnNull()
//        {
//            // Arrange
//            var schema = fixture.Build<Schema>().With(x => x.IsDeleted, true).Create();
//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");
//            var schemaToUpdate = repository.Insert(schema);
//            //Act
//            var result = repository.Update(schema);
//            // Assert
//            Assert.Null(result);
//        }

//        [Fact]
//        public async Task RepositoryGetByFieldShouldReturnObjectAsync()
//        {
//            // Arrange
//            var schema = fixture.Build<Schema>().With(x => x.IsDeleted, false).Create();
//            var holy = fixture.Build<PaginationData<Schema>>().Create();
//            var ok = fixture.Build<Dictionary<string, object>>().Create();
//            var mockHelper = new Mock<IUserHelper>();
//            var test = new TDDContext(mockHelper.Object);
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");

//            // Act

//            var schemaReturn = await repository.Insert(schema);
//            var result = repository.GetByField("name", schemaReturn.Name);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(schemaReturn.Name, result.Name);
//        }

//        [Fact]
//        public void RepositoryGetByFieldShouldReturnNull()
//        {
//            // Arrange
//            var schema = fixture.Build<Schema>().With(x => x.IsDeleted, false).Create();
//            var holy = fixture.Build<PaginationData<Schema>>().Create();

//            var ok = fixture.Build<Dictionary<string, object>>().Create();

//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");

//            // Act

//            var result = repository.GetByField("name", schema.Name);

//            // Assert
//            Assert.Null(result);
//        }

//        [Fact]
//        public void RepositoryExistsShouldReturnTrue()
//        {
//            // Arrange
//            var schema = fixture.Build<Schema>().With(x => x.IsDeleted, false).Create();
//            var holy = fixture.Build<PaginationData<Schema>>().Create();

//            var ok = fixture.Build<Dictionary<string, object>>().Create();

//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");

//            // Act

//            repository.Insert(schema);

//            Expression<Func<Schema, bool>> predicate = x => x.Name == schema.Name;
//            var result = repository.Exists(predicate);

//            // Assert
//            Assert.True(result);
//        }

//        [Fact]
//        public void RepositoryExistsShouldReturnFalse()
//        {
//            // Arrange
//            var schema = fixture.Build<Schema>().With(x => x.IsDeleted, false).Create();
//            var holy = fixture.Build<PaginationData<Schema>>().Create();

//            var ok = fixture.Build<Dictionary<string, object>>().Create();

//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");

//            // Act

//            Expression<Func<Schema, bool>> predicate = x => x.Name == schema.Name;
//            var result = repository.Exists(predicate);

//            // Assert
//            Assert.False(result);
//        }

//        [Fact]
//        public void RepositorySearchLikeInFieldAutoCompleteShouldReturn()
//        {
//            // Arrange
//            var schema = fixture.Build<Schema>().With(x => x.IsDeleted, false).Create();
//            var holy = fixture.Build<PaginationData<Schema>>().Create();
//            var ok = fixture.Build<Dictionary<string, object>>().Create();

//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");

//            // Act

//            Expression<Func<Schema, bool>> predicate = x => x.Name == schema.Name;
//            var result = repository.SearchLikeInFieldAutoComplete("name", "Name", 10);

//            // Assert
//            Assert.NotEmpty(result);
//        }

//        [Fact]
//        public void RepositorySearchLikeInFieldAutoCompleteShouldReturnEmptyData()
//        {
//            // Arrange
//            //var schema = fixture.Build<Schema>().With(x => x.IsDeleted, false).Create();
//            // var holy = fixture.Build<PaginationData<Schema>>().Create();
//            //var ok = fixture.Build<Dictionary<string, object>>().Create();

//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");

//            // Act

//            var result = repository.SearchLikeInFieldAutoComplete("namee", "Namee", 10);

//            // Assert
//            Assert.Empty(result);
//        }

//        [Fact]
//        public void RepositorySearchByFilterDefinitionShouldReturnResult()
//        {
//            //Arrange
//            var filter = Builders<Schema>.Filter.Eq(e => e.Name, "Account");
//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");
//            //Act
//            var result = repository.SearchByFilterDefinition(filter);
//            //Assert
//            Assert.NotEmpty(result.Data);
//        }

//        [Fact]
//        public void RepositorySearchByFilterDefinitionShouldReturnEmptyData()
//        {
//            //Arrange
//            var filter = Builders<Schema>.Filter.Eq(e => e.Name, "Account22");
//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");
//            //Act
//            var result = repository.SearchByFilterDefinition(filter, 1, 1, "name");
//            //Assert
//            Assert.Empty(result.Data);
//        }

//        [Fact]
//        public void RepositorySearchShouldReturnResult()
//        {
//            //Arrange
//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");
//            //Act
//            var Result = repository.Search(x => x.Name == "Account");
//            //Assert
//            Assert.NotEmpty(Result.Data);
//        }

//        [Fact]
//        public void RepositorySearchShouldReturnEmptyData()
//        {
//            //Arrange
//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");
//            //Act
//            var Result = repository.Search(x => x.Name == "Account22", 1, 1, "name");
//            //Assert
//            Assert.Empty(Result.Data);
//        }

//        [Fact]
//        public void RepositorySearchRegexByFieldsDefsShouldReturnEmptyData()
//        {
//            //Arrange
//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");

//            var defsFilterGreaterThan = fixture.Build<Filter>().With(x => x.@operator, "greaterThan").Create();
//            var defsFilterEqualsTo = fixture.Build<Filter>().With(x => x.@operator, "equalsTo").Create();
//            var defsFilterNotEqualsTo = fixture.Build<Filter>().With(x => x.@operator, "notEqualsTo").Create();
//            var defsFilterContains = fixture.Build<Filter>().With(x => x.@operator, "contains").Create();
//            var defsFilterNotContains = fixture.Build<Filter>().With(x => x.@operator, "notContains").Create();
//            var defsFiltergreaterThanOrE = fixture.Build<Filter>().With(x => x.@operator, "greaterThanOrEqualTo").Create();
//            var defsFilterlessThan = fixture.Build<Filter>().With(x => x.@operator, "lessThan").Create();
//            var defsFilterlessThanOrEqualTo = fixture.Build<Filter>().With(x => x.@operator, "lessThanOrEqualTo").Create();
//            List<Filter> filtros = new();
//            filtros.Add(defsFilterGreaterThan);
//            filtros.Add(defsFilterEqualsTo);
//            filtros.Add(defsFilterNotEqualsTo);
//            filtros.Add(defsFilterContains);
//            filtros.Add(defsFilterNotContains);
//            filtros.Add(defsFiltergreaterThanOrE);
//            filtros.Add(defsFilterlessThan);
//            filtros.Add(defsFilterlessThanOrEqualTo);
//            //Act
//            //var result = repository.SearchRegexByFieldsDefs(filtros,1,1,"name");
//            //Assert
//            Assert.Empty("");
//        }

//        [Fact]
//        public void RepositorySearchRegexByFieldsDefsShouldReturnData()
//        {
//            //Arrange
//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");

//            var defsFilter = fixture.Build<Filter>().Create();
//            List<Filter> filtros = new();
//            filtros.Add(defsFilter);

//            //Act
//            //var result = repository.SearchRegexByFieldsDefs(filtros, 1, 1, "name");
//            //Assert
//            Assert.NotEmpty("teste");
//        }



//        [Fact]
//        public void RepositorySearchRegexByFieldsShouldReturnResults()
//        {
//            //Arrange
//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");
//            //Act
//            //var result = repository.SearchRegexByFields("Name", "Account");
//            //Assert
//            Assert.Null(null);
//        }

//        [Fact]
//        public void RepositorySearchRegexByFieldsShouldReturnEmptyData()
//        {
//            //Arrange
//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");
//            //Act
//            //var result = repository.SearchRegexByFields("Name", "Accounta",1,1,"Name",false,"5134A34DF");
//            //Assert
//            Assert.Empty("");
//        }

//        [Fact]
//        public void RepositoryDeleteShouldReturnNull()
//        {
//            //Arrange
//            var schema = fixture.Build<Schema>().With(x => x.Id, "").With(x => x.IsDeleted, false).Create();
//            var mockRepository = new Mock<IRepository<MongoDbContext, Schema>>();
//            var mockHelper = new Mock<IUserHelper>();
//            var repository = new Repository<TDDContext, Schema>(_mockContext, mockHelper.Object, null, "schema");
//            repository.Insert(schema);
//            var schemaCreat = repository.GetById(schema.Id);
//            //Act
//            repository.Delete(schema.Id);
//            var schemaDelet = repository.GetById(schema.Id);
//            //assert
//            Assert.Null(schemaDelet);
//        }
//    }
//}