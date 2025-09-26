using System.Collections.Generic;
using System.Security.Claims;
using Simjob.Framework.Domain.Models;

namespace Simjob.Framework.Domain.Interfaces.Users
{
    public interface IUserHelper
    {
        string GetTenanty();
        string GetUserName();
        string GetId();
        bool IsAuthenticated();
        List<Claim> GetClaims();
    }
}
