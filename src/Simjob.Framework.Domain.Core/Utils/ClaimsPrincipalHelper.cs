using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Simjob.Framework.Domain.Core.Utils
{
    public static class ClaimsPrincipalHelper
    {
        public static List<Session> Sessions = new List<Session>();

        public static void AddSession(string sessionId, ClaimsPrincipal claimsPrincipal)
        {
            var session = new Session
            {
                SessionId = sessionId,
                CurrentClaimsPrincipal = claimsPrincipal,
            };

            Sessions.Add(session);
        }

        public static ClaimsPrincipal? GetClaimsPrincipal(string sessionId)
        {
            var session = Sessions.FirstOrDefault(s => s.SessionId == sessionId);
            return session?.CurrentClaimsPrincipal;
        }
    }

    public class Session
    {
        public ClaimsPrincipal? CurrentClaimsPrincipal { get; set; }
        public string SessionId { get; set; }
    }
}
