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

    public interface IItemDataAccess : IGenericRepository<Item>
    {
        IEnumerable<ItemUI> getItemSearch(SearchParameters parametros, String descricao, Boolean inicio, Boolean? ativo, int? tipoItem, int? cdGrupoItem, int? cdEscola, bool isMaster, bool estoque,
            bool biblioteca, bool comEstoque, int categoria, bool todas_escolas, bool contaSegura);
        IEnumerable<ItemUI> getItemSearchAlunosemAula(SearchParameters parametros, String descricao, Boolean inicio, Boolean? ativo, int? tipoItem, int? cdGrupoItem, int? cdEscola, bool isMaster, bool estoque,
            bool biblioteca, bool comEstoque, int categoria, bool todas_escolas, bool contaSegura);
        IEnumerable<ItemKitUI> getKitSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int? tipoItem, int? cdGrupoItem, int? cdEscola, bool isMasterGeral,
            bool estoque, bool biblioteca, bool comEstoque, int categoria, bool todas_escolas, bool contaSegura);
        IEnumerable<Item> getItensByIds(List<Item> itens);
        bool deleteItemByCurso(int cd_curso);
        bool deleteAllItem(List<Item> itens);
        IEnumerable<ItemUI> getItemCurso(int cdCurso, int? cdEscola);
        Item getItemEdit(int cd_item);
        List<KitUI> getItensKit(int idKit);
        ItemUI getItemUIbyId(int cd_item, int cdEscola);
        IEnumerable<ItemUI> getItemSearch(SearchParameters parametros, String descricao, Boolean inicio, Boolean? ativo, int tipoItem, int cdGrupoItem, int cdEscola, bool comEstoque, int categoria);
        IEnumerable<ItemUI> getItemSearchEstoque(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int tipoItem, int cdGrupoItem, int cdEscola, bool comEstoque, List<int> cdItens, bool isMaster, int ano, int mes);
        IEnumerable<RptItemFechamento> rptItemWithSaldoItem(int cd_pessoa_escola, int cd_item, int cd_grupo, byte cd_tipo, int ano, int mes);
        IEnumerable<RptItemFechamento> rptItemSaldoBiblioteca(int cd_pessoa_escola, int cd_item, int cd_grupo, byte cd_tipo, DateTime dt_kardex);
        IEnumerable<Item> listItensWithEstoque(int cd_pessoa_escola, int cd_item, int cd_grupo, int cd_tipo);
        IEnumerable<ItemUI> listItensMaterial(int cd_curso, int cd_escola);
        int quantidadeItensMaterialCurso(int cd_turma, int cd_escola);
        bool getItemNomeEsc(string noItem, int cdEscola);
        bool getItemNome(string noItem);
        bool getexisteParametroItem(int cd_item);
        //string getPlanoContaByItem(int cd_item);
        IEnumerable<ItemUI> obterListaItemMovItemsKit(ItemMovimento item, int cdEscola);
        IEnumerable<ItemUI> obterListaItemsKit(int cd_item_kit, int cdEscola);
        IEnumerable<ItemUI> obterListaItemsKitMov(int cd_item_kit, int cdEscola, int id_tipo_movto, int? id_natureza_TPNF);
    }
}
