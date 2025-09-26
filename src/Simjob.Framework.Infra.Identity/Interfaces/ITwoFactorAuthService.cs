using Simjob.Framework.Domain.Models;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Schemas.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface ITwoFactorAuthService
    {
        void Register(UserTwoFactorAuth a2f);
        //UserTwoFactorAuth GerenerateAuth2F(UserTwoFactorAuth userId);
        Task<UserTwoFactorAuth> SendCodeVerification(UserTwoFactorAuth obj);
        Task<string> getToken();
        Task<string> getTokenToSendEmail(string token);
        UserTwoFactorAuth CodeIsValid(string id, string code, string hash, string ip);
    }
}
