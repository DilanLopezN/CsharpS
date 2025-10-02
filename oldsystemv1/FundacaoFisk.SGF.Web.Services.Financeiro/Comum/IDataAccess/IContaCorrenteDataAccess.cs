using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface IContaCorrenteDataAccess : IGenericRepository<ContaCorrente>
    {
        List<ContaCorrente> getContaCorrenteByBaixa(int cd_baixa_titulo, int cd_pessoa_empresa);
        IEnumerable<ContaCorrenteUI> getContaCorreteSearch(SearchParameters parametros, int cd_pessoa_escola, int cd_origem, int cd_destino, byte entraSaida, int cd_movimento, int cd_plano_conta, DateTime? dta_ini, DateTime? dta_fim, int cd_pessoa_usuario, bool contaSegura);
        IEnumerable<ContaCorrenteUI> rtpContaCorrente(int cd_pessoa_escola, int cd_local_movimento, DateTime dta_inicial, DateTime dta_final, decimal saldo_inicial, int tipoLiquidacao, bool contaSegura, bool isMaster);
        Decimal fcSaldoContaCorrente(int cd_pessoa_escola, int cd_local_movimento, DateTime dta_saldo, int tipoLiquidacao);
        IEnumerable<SubgrupoConta> getGruposSemContaCorrenteN1(DateTime data_inicial, DateTime data_final, int cd_empresa, bool conta_segura);
        IEnumerable<SubgrupoConta> getGruposSemContaCorrenteN2(DateTime data_inicial, DateTime data_final, int cd_empresa, bool conta_segura);
        decimal buscaSaldoAnteriorGrupo(int cd_escola, int cd_grupo_conta, DateTime data_anterior);
        decimal buscaSaldoAnteriorSubGrupo(int cd_escola, int cd_grupo_conta, DateTime data_anterior);
        IEnumerable<ContaCorrente> getObservacoesCCBaixa(int? cd_baixa_titulo, int? cd_conta_corrente);
        IEnumerable<RptRecebidaPaga> recebidaPagaStoreProcedure(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pForn, byte pNaturezaa, int pPlanoContas, bool cSegura, int cdTpLiq, int cdTpFinan, int tipo, string situacoes, int cdTurma, int cdLocal);
        List<SaldoFinanceiro> rtpSaldoFinanceiro(int cd_empresa, DateTime dta_base,byte tipoLocal, bool liquidacao);
        List<SaldoFinanceiro> rtpLocalMovimentoSemCCorrenteDtaBase(int cd_empresa, DateTime dta_base, byte tipoLocal);
        ContaCorrente existAberturaSaldoData(DateTime dtaInicial, int cd_pessoa_escola, int cd_local_movto, int tipoLiquidacao);
        ContaCorrenteUI getContaCorretePlanoConta(int cd_pessoa_escola, int cd_conta_corrente);
        IEnumerable<ContaCorrenteUI> getFechamentoCaixaTpLiquidacao(int cd_pessoa_escola, DateTime dta_fechamento, int cdUsuario, byte tipoLocal);
        IEnumerable<ContaCorrenteUI> getFechamentoCaixaLocalMovto(int cd_pessoa_escola, DateTime dta_fechamento, int tipoLiquidacao, int cdUsuario, byte tipoLocal);

        void postZerarSaldoFinanceiro(int cd_escola, int cd_tipo_liquidacao, Nullable<System.DateTime> dta_base, byte tipo);
        IEnumerable<ContaCorrenteUI> getFechamentoCaixaLocalMovtoRel(int cd_pessoa_escola, DateTime dta_fechamento, int cdUsuario, byte tipoLocal);
    }
}
