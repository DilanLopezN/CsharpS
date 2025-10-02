using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface ITipoItemDataAccess : IGenericRepository<TipoItem>
    {
        IQueryable<TipoItem> getTipoItemSearch(int? tipoMovimento);
        IEnumerable<TipoItem> getTipoItemMovimento(TipoItemDataAccess.TipoConsultaTipoItemEnum tipoConsulta);
        IEnumerable<TipoItem> getTipoItemMovimentoWithItem(int cd_pessoa_escola, bool isMaster);
        IEnumerable<TipoItem> getTipoItemMovimentoEstoque();
        bool verificaMovimentarEstoque(int cd_item);
    }
}
