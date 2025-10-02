using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IDuracaoDataAccess : IGenericRepository<Duracao>
    {
        IEnumerable<Duracao> getDuracaoDesc(SearchParameters parametros, String desc, Boolean inicio, Boolean? ativo);
        Boolean deleteAllDuracao(List<Duracao> duracao);
        IEnumerable<Duracao> getDuracaoTabelaPreco();
        IEnumerable<Duracao> getDuracoes(DataAccess.DuracaoDataAccess.TipoConsultaDuracaoEnum hasDependente, int? cd_duracao,int? cd_escola);
        IEnumerable<Duracao> getDuracaoProgramacao();
    }
}
