using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface IKardexDataAccess : IGenericRepository<Kardex>
    {
        IEnumerable<Kardex> getKardexByOrigem(int cd_origem, int cd_registro_origem);
        Kardex getKardexByOrigemItem(int cd_origem, int cd_registro_origem, int cd_item, int cd_escola);
        IEnumerable<KardexUI> st_Rptkardex(int cd_pessoa_escola, int cd_item, DateTime dt_ini, DateTime dt_fim, int cd_grupo, byte tipo);
        int getSaldoItem(int cd_item, DateTime dataLimite, int cd_escola);
        DateTime getMaxMovimentoKardex(int cd_pessoa_escola);
        decimal? getSaldoValorItem(int cd_item, DateTime dataLimite, int cd_escola);

        bool existeKardexItemMovimentoByOrigem(int cd_origem, int cd_registro_origem);
        IEnumerable<Kardex> getKardexItensMovimentoNF(int cd_movimento, int cd_pessoa);
        bool existeKardexItem(int cd_item);
        bool existeKardexItemEsc(int cd_item, int cdEscola);
        bool existeKardexItemEscolas(int cd_item, List<int> cdEscolas);
        List<SaldoFinanceiro> getFechamentoKardex(DateTime? dt_kardex, int cd_item, int cd_pessoa_empresa);
        //List<sp_RptInventario_Result> getRptInventario(int? cd_escola, DateTime? dt_analise, byte? id_valor);
        DataTable getRptInventario(int? cd_escola, DateTime? dt_analise, byte? id_valor, string tipoItem);
    }
}
