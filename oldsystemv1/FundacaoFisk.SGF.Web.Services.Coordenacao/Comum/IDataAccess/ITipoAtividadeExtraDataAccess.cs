using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface ITipoAtividadeExtraDataAccess : IGenericRepository<TipoAtividadeExtra>
    {
         IEnumerable<TipoAtividadeExtra> getAtividadeDesc(SearchParameters parametros, String desc, Boolean inicio, Boolean? ativo);
         Boolean deleteAllTipoAtividade(List<TipoAtividadeExtra> tiposAtividades);
         TipoAtividadeExtra firstOrDefault();
         IEnumerable<TipoAtividadeExtra> getTipoAtividade();
         List<TipoAtividadeExtra> findTopRegisters(int qtd);
         IEnumerable<TipoAtividadeExtra> getTipoAtividade(bool? status, int? cdTipoAtividadeExtra, int? cd_pessoa_escola,TipoAtividadeExtraDataAccess.TipoConsultaAtivExtraEnum tipoConsulta);
         IEnumerable<TipoAtividadeExtra> getTipoAtividadeWhitAtividadeExtra(int cd_pessoa_escola);
    }
}
