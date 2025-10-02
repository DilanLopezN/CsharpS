using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
//using FundacaoFisk.SGF.Web.Service.EmailMarketing.Model;
//using FundacaoFisk.SGF.Web.Services.EmailMarketing.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IDataAccess
{
    public interface IListaNaoInscritoDataAccess : IGenericRepository<ListaNaoInscrito>
    {
        IEnumerable<ListaNaoInscrito> getListaNaoIncritoEnderecos(int cd_empresa, string no_pessoa, int status, string email, byte id_tipo_cadastro);
        IEnumerable<ListaNaoInscrito> getListaNaoIncritoEscola(int cd_empresa);
        bool jaExisteNaoIncrito(int cd_empresa, int cd_cadastro, int id_cadastro);
    }
}
