using Simjob.Framework.Domain.Models;

namespace Simjob.Framework.Services.Api.Interfaces
{
    public interface ITokenService
    {
        TokenResponse GerenerateToken(string userId);
        TokenResponse GerenerateTokenAdmin(string userId);
        dynamic GerenerateTokenForEmail(string email);
        bool ValidateToken(string token);
    }
}
