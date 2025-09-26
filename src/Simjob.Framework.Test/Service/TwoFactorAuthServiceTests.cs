using AutoFixture;
using Moq;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Services;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System.Threading.Tasks;
using Xunit;

namespace Simjob.Framework.Test.Services
{
    //public class TwoFactorAuthServiceTests
    //{
    //    private static Fixture fixture;
    //    private TwoFactorAuthContext dbcontext;
    //    private Mock<IUserHelper> userHelperMock;
    //    private Mock<DomainNotificationHandler> notfMock;
    //    private Mock<IMediatorHandler> busMock;
    //    private Mock<IRepository<TwoFactorAuthContext, UserTwoFactorAuth>> twoFactorAuthRepositoryMock;
    //    private Mock<IRepository<MongoDbContext, Schema>> schemaRepoMock;
    //    private Mock<ISchemaBuilder> schemaBuilderMock;
    //    private TwoFactorAuthService twoFactorAuthService;

    //    public TwoFactorAuthServiceTests()
    //    {
    //        fixture = new Fixture();
    //        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
    //        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    //        busMock = new Mock<IMediatorHandler>();
    //        notfMock = new Mock<DomainNotificationHandler>();
    //        dbcontext = new TwoFactorAuthContext(userHelperMock.Object);
    //        twoFactorAuthRepositoryMock = new Mock<IRepository<TwoFactorAuthContext, UserTwoFactorAuth>>();
    //        schemaRepoMock = new Mock<IRepository<MongoDbContext, Schema>>();
    //        schemaBuilderMock = new Mock<ISchemaBuilder>();
    //        twoFactorAuthService = new TwoFactorAuthService(dbcontext, notfMock.Object, busMock.Object, twoFactorAuthRepositoryMock.Object, schemaRepoMock.Object, schemaBuilderMock.Object);
    //    }
    //}
}