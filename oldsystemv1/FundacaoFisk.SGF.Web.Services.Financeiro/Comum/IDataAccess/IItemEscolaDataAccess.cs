using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface IItemEscolaDataAccess : IGenericRepository<ItemEscola>
    {
        ItemEscola findItemEscolabyId(int cdItem, int cdPessoa);
        ICollection<ItemEscola> getItensWithEscola(int cdItem, int cdUsuario);
        IEnumerable<int> getItensWithEscola(int cdItem);
        IEnumerable<ItemEscola> getItensEscolaByItem(int cdItem);
        decimal getValorCusto(int cd_item, int cd_escola);
        int getQtdEstoque(int cd_item, int cd_escola);
        IEnumerable<ItemEscola> getItemEscolaByItem(int cdItem);
        IEnumerable<ItemEscola> getItemComSubgrupoByEscola(int cdSubGrupo, int cdEscola);
        bool getExisteItensEscolaByItem(int cdItem);
    }
}
