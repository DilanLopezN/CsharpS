using AutoFixture;
using Moq;
using Newtonsoft.Json;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Infra.Identity.Interfaces;
using System.Threading.Tasks;
using Xunit;

namespace Simjob.Framework.Test.Services
{
    public class SearchdefsServiceTests
    {
        private static Fixture fixture;
        private Mock<ISearchdefsService> searchdefsServiceMock;
        //SearchdefsContext dbcontext;
        //SearchdefsService searchdefsService;
        //Mock<IUserHelper> userHelperMock;
        //Mock<IRepository<SearchdefsContext, Searchdefs>> searchdefRepoMock;
        //Mock<IRepository<MongoDbContext, Schema>> schemaRepoMock;

        public SearchdefsServiceTests()
        {
            fixture = new Fixture();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            searchdefsServiceMock = new Mock<ISearchdefsService>();
            //var busMock = new Mock<IMediatorHandler>();
            //var notfMock = new Mock<DomainNotificationHandler>();
            //userHelperMock = new Mock<IUserHelper>();
            //dbcontext = new SearchdefsContext(userHelperMock.Object);
            //schemaRepoMock = new Mock<IRepository<MongoDbContext, Schema>>();
            //searchdefRepoMock = new Mock<IRepository<SearchdefsContext, Searchdefs>>();
            //searchdefsService = new SearchdefsService(dbcontext,busMock.Object,searchdefRepoMock.Object,schemaRepoMock.Object);
        }

        [Fact]
        public void GetsearchdefsValid()
        {
            //Arrange
            var id = "87642";
            var searchdefsObj = fixture.Build<Searchdefs>().With(c => c.Id, id).With(x => x.IsDeleted, false).Create();
            searchdefsServiceMock.Setup(m => m.GetSearchdefsById(id)).Returns(searchdefsObj);

            //Act
            var searchdefs = searchdefsServiceMock.Object.GetSearchdefsById(id);

            //Assert
            var obj1Str = JsonConvert.SerializeObject(searchdefs);
            var obj2Str = JsonConvert.SerializeObject(searchdefsObj);

            Assert.NotNull(searchdefs);
            Assert.Equal(obj1Str, obj2Str);
        }

        [Fact]
        public void GetsearchdefsInvalid()
        {
            //Arrange
            var id = "87642";
            var searchdefsObj = fixture.Build<Searchdefs>().With(c => c.Id, id).With(x => x.IsDeleted, false).Create();
            searchdefsServiceMock.Setup(m => m.GetSearchdefsById(id)).Returns(searchdefsObj);

            //Act
            var searchdefs = searchdefsServiceMock.Object.GetSearchdefsById("a21r4");

            //Assert
            var obj1Str = JsonConvert.SerializeObject(searchdefs);
            var obj2Str = JsonConvert.SerializeObject(searchdefsObj);

            Assert.Null(searchdefs);
            Assert.NotEqual(obj1Str, obj2Str);
        }
    }
}