using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    public interface IAtividadeDataAccess : IGenericRepository<Atividade>
    {
        IEnumerable<Atividade> getAllListAtividades(string searchText, int natureza, bool? status);
        IEnumerable<Atividade> GetAtividadeSearch(SearchParameters parametros, string descricao, bool inicio, bool? status, int natureza, string cnae);
        bool deleteAll(List<Atividade> atividades);
    }
}
