using Simjob.Framework.Domain.Models;
using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IExternalTokensService
    {
        public TokenResponse GerenerateToken(string userId);
        public Tokens Register(Tokens tokens);

        public Tokens Update(string id,Tokens tokens);

        public Tokens GetById(string id);
        public Tokens GetByUserId(string userId);
        public Tokens GetByUserToken(string userToken);

        List<Tokens> GetAllTokens();
        public void DeleteToken(string id);
    }
}
