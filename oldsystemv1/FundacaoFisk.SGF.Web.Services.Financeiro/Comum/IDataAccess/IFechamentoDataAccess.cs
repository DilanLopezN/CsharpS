using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using Componentes.GenericDataAccess.Comum;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

    public interface IFechamentoDataAccess : IGenericRepository<Fechamento>
    {
        IEnumerable<Fechamento> getFechamentoSearch(SearchParameters parametros, int? ano, int? mes, bool balanco, DateTime? dta_ini, DateTime? dta_fim, int cd_escola);
        Fechamento getFechamentoById(int cd_fechamento, int cd_escola);
        bool existeFechamentoAnoMes(DateTime data, int cd_escola, int cd_fechamento);
        Fechamento getFechById(int cd_fechamento, int cd_escola);
        bool existeFechamentoSuperior(DateTime data, int cd_escola, int cd_fechamento);       
        IEnumerable<Fechamento> fechamentoAnoMes(int cd_escola);
        IEnumerable<Fechamento> getFechamentos(List<int> cdFechamentos, int cd_empresa);
        Fechamento getFechamentoByDta(DateTime data, int cd_empresa);
        Fechamento getUltimoFechamentoItem(DateTime dt_fechamento, int cd_item, int cd_pessoa_empresa);
    }
}
