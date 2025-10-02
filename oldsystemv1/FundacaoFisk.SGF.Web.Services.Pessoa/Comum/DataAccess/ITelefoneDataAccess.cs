using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    public interface ITelefoneDataAccess : IGenericRepository<TelefoneSGF>
    {
        TelefoneSGF retornaTelefonefirstOrDefault();
        TelefoneSGF FindTypeTelefone(int cdPessoa, int tipoTel);
        IEnumerable<TelefoneSGF> GetAllTelefoneByPessoa(int cdPessoa);
        TelefoneSGF FindTypeTelefonePrincipal(int cdPessoa, int tipoTel);
        IEnumerable<TelefoneSGF> getAllTelefonesContatosByPessoa(int cdPessoa);
    }
}
