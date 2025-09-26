using AutoFixture;
using MongoDB.Driver;
using Moq;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Repositories;
using Simjob.Framework.Test.Services.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Simjob.Framework.Test.Repository
{
   public class TwoFactorAuthRepositoryTests
    {
        private static Fixture fixture;
        private Mock<IMongoCollection<User>> _mockCollection;
        private Mock<IUserHelper> _userHelperMock;
        private TDDContext _mockContext;
        private User _user;
        public static List<User> _list = new List<User>();
        private TwoFactorAuthRepository twoFactorAuthRepository;
        public TwoFactorAuthRepositoryTests()
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
            twoFactorAuthRepository = new TwoFactorAuthRepository(_mockContext, "TwoFactorAuth");
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
        public void InsertShouldReturnTrue() 
        { 
            //Arrange
            var Tfactor = fixture.Build<UserTwoFactorAuth>().With(x => x.IsDeleted, false).With(x => x.Id, "").Create();
            //Act
            twoFactorAuthRepository.Insert(Tfactor);
            var existTFactor = twoFactorAuthRepository.Exists(x=>x.Id==Tfactor.Id);
            Assert.True(existTFactor);
        }

        [Fact]
        public void ExistsShouldReturnTrue()
        {
            //Arrange
            var Tfactor = fixture.Build<UserTwoFactorAuth>().With(x => x.IsDeleted, true).Create();
            twoFactorAuthRepository.Insert(Tfactor);
            //Act
            var existTFactor = twoFactorAuthRepository.Exists(x => x.Id == Tfactor.Id);
            Assert.True(existTFactor);
        }

        [Fact]
        public void ExistsShouldReturnFalse()
        {
            //Arrange
            var Tfactor = fixture.Build<UserTwoFactorAuth>().With(x => x.IsDeleted, true).Create();
            twoFactorAuthRepository.Insert(Tfactor);
            //Act
            var existTFactor = twoFactorAuthRepository.Exists(x => x.Id == "ID124523");
            Assert.False(existTFactor);
        }

    }
}
