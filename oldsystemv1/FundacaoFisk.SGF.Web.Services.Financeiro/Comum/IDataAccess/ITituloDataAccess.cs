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
    using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;

    public interface ITituloDataAccess : IGenericRepository<Titulo>
    {
        List<Titulo> getTitulosByContrato(int cdContrato, int cdEscola);
        int getQtdContratoNaoMultiploDiferenteCartaoCheque(int cdContrato, int cdEscola);
        int getQtdMovimentoDiferenteCartaoCheque(int cd_movimento, int cdEscola);
        int getQtdTitulosSemBaixaTipoCartaoOuCheque(int cd_contrato, int cdEscola);
        int getQtdTitulosMovimentoSemBaixaTipoCartaoOuCheque(int cdMovimento, int cdEscola);
        List<Titulo> getTitulosContrato(int cd_contrato);
        bool getTituloBaixadoContrato(int cd_contrato, int cdEscola, int cdTitulo);
        decimal getValorParcela(int cdContrato, int cdEscola, bool primeira_parcela);
        bool deleteAllTitulo(List<Titulo> titulos);
        Titulo getTituloByContrato(int cd_contrato, int nro_parcela);
        Titulo getTituloAbertoByAditamento(int cd_contrato);
        List<Titulo> getTitulosByRenegociacao(int cdContrato, int cdAluno, int cdEscola, int cdProduto);
        IEnumerable<Titulo> getTitulosContrato(List<int> cdTitulos, int cd_escola);
        IEnumerable<Titulo> getTitulosByContratoTodosDados(int cdContrato, int cdEscola);
        IEnumerable<Titulo> searchTitulo(SearchParameters parametros, int cd_pessoa_empresa, int cd_pessoa, bool responsavel, bool inicio, int locMov, int natureza, int status, int? numeroTitulo,
                                               int? parcelaTitulo, string valorTitulo, DateTime? dtInicial, DateTime? dtFinal, bool emissao, bool vencimento, bool baixa, int locMovBaixa,
                                               int cdTipoLiquidacao, bool contaSegura, byte tipoTitulo, string nossoNumero, int cnabStatus, int? nro_recibo, int? cd_turma, List<int> cd_situacoes_aluno,
                                               int? cd_tipo_financeiro);
        string getResponsavelTitulo(int cd_titulo, int cd_pessoa_empresa);
        IEnumerable<Titulo> searchTituloCnab(TituloUI titulo);
        IEnumerable<Titulo> getTitulosForBaixaAutomatica(SearchParameters parametros, TituloUI titulo);
        IEnumerable<Titulo> getTitulosForBaixaAutomaticaCheque(TituloChequeUI titulo);
        int gerarBaixaAutomaticaProcedure(int cd_baixa_automatica, int cd_usuario);
        List<TituloCnab> searchTituloCnabGrade(TituloUI titulo);
        Titulo getTituloBaixaFinan(int cd_titulo, int cd_pessoa_empresa, TituloDataAccess.TipoConsultaTituloEnum tipoConsulta);
        Titulo getTituloBaixaFinan(string dc_nosso_numero, int cd_pessoa_empresa, int cd_local_movto);
        List<Titulo> getTitulosBaixaFinan(List<string> dc_nosso_numero, int cd_pessoa_empresa, int cd_local_movto);
        List<Titulo> getTituloBaixaFinanSimulacao(List<int> cdsTitulo, int cd_pessoa_empresa, int? cd_registro_origem, TituloDataAccess.TipoConsultaTituloEnum tipoConsulta);
        IEnumerable<RptReceberPagar> receberPagarStoreProcedure(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pForn, DateTime pDtaBase, byte pNaturezaa, int pPlanoContas, int ordem, bool cSegura, int cdTpLiq, int cdTpFinan, int tipo, string situacoes, int cdTurma);
        IEnumerable<RptRecebidaPaga> recebidaPagaStoreProcedure(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pForn, byte pNaturezaa, int pPlanoContas, bool cSegura, int cdTpLiq, int cdTpFinan, int tipo, string situacoes, int cdTurma, bool? ckCancelamento, int cdLocal);
        IEnumerable<ObservacaoBaixaUI> getObservacoesBaixaCancelamento(int cdEscola, int cd_baixa_titulo);
        IEnumerable<Titulo> getTituloByPessoa(SearchParameters parametros, int cd_pessoa, int cd_escola, TituloDataAccess.TipoConsultaTituloEnum tipo, bool contaSeg);
        IEnumerable<Titulo> getTituloByPessoaResponsavel(int cd_pessoa_titulo, int cd_escola, int cd_contrato, bool contaSeg);
        IEnumerable<Titulo> getTitulosByTransacaoFinanceira(int cd_pessoa_empresa, int cd_tran_finan);
        List<Titulo> getTitulosGridByMovimento(int cd_movto, int cd_empresa, int? cd_aluno);
        IEnumerable<Titulo> getTitulosByMovimento(int cd_movto, int cd_empresa);
        bool verificarSeExisteTitulosAnterioresAberto(List<int> cdTitulos, int cd_empresa);
        bool verificarTituloContaSegura(int cd_titulo, int cd_empresa);
        IEnumerable<Titulo> getTitulosByOrigem(int cdOrigemTitulo, int idOrigemTitulo, int cd_empresa);
        bool verificarStatusCnabTitulo(int[] cdTituls, int cd_empresa, int cd_cnab, byte id_tipo_cnab);
        IEnumerable<Titulo> getDadosAdicionaisTituloParaCnab(int[] cdTitulos, int cd_empresa);
        IEnumerable<Titulo> getTitulosByCnab(int cd_cnab, int cd_empresa);
        IEnumerable<Titulo> getTitulosByCnab(int[] cdCnabs, int cd_empresa);
        bool existCnabTitulo(int cd_titulo, int cd_empresa);
        IEnumerable<AlunosSemTituloGeradoUI> GetAlunosSemTituloGerado(int vl_mes, int ano, int cd_turma, string situacoes, int cd_escola);
        bool existTituloRetornoCnab(int cd_titulo);
        IEnumerable<Titulo> getTitulosAbertoContrato(int cd_empresa, int cd_contrato);
        Decimal getSaldoTitulosMatricula(int cd_empresa, int cd_contrato);
        IEnumerable<CarneUI> getCarnePorContrato(int cdContrato, int cdEscola, int parcIniCarne, int parcFimCarne);
        IEnumerable<Titulo> searchTituloFaturamento(SearchParameters parametros, int cd_pessoa_empresa, int cd_pessoa, bool responsavel, bool inicio, int? numeroTitulo,
                                               int? parcelaTitulo, string valorTitulo, DateTime? dtInicial, DateTime? dtFinal, bool contaSegura, byte tipoTitulo);
        Titulo getTituloBaixaFinanMovimentoNF(int cd_baixa_titulo, int cd_pessoa_empresa);
        List<Titulo> getTitulosAbertosImpressaoAdt(int cdContrato, int cdEscola);
        List<Titulo> getTitulosAbertosAdicionarParcImpressaoAdt(int cdContrato, int cdEscola, int qtd_titulos_adt);
        IEnumerable<CarneUI> getCarnePorMovimentos(int cdMovimento, int cdEscola);
        IEnumerable<ChequeUI> getRptChequesAbertos(int cd_empresa, int cd_pessoa_aluno, int cd_banco, string emitente, bool liquidados, string nm_cheques, string vl_titulo, int nm_agencia,
            int nm_ccorrente, DateTime? dt_ini_bPara, DateTime? dt_fim_bPara, DateTime? dt_ini, DateTime? dt_fim, bool emissao, bool liquidacao, byte natureza);
        IEnumerable<ChequeUI> getRptChequesLiquidados(int cd_empresa, int cd_pessoa_aluno, int cd_banco, string emitente, bool liquidados, string nm_cheque, string vl_titulo, int nm_agencia,
   int nm_ccorrente, DateTime? dt_ini_bPara, DateTime? dt_fim_bPara, DateTime? dt_ini, DateTime? dt_fim, bool emissao, bool liquidacao, byte natureza);
        bool verificaTituloOrContratoBaixaEfetuada(int cd_contrato, int cdEscola, int cdTitulo);
        bool verificaTituloContratoAjusteManual(List<int> titulos, int cd_escola, bool aditivo);
        bool verificaTituloVencido(int cdPessoa, DateTime dataHoje, int cd_escola);
        bool verificaTituloEnviadoCnabOuContrato(int cd_contrato, int cdEscola, List<int> cdsTitulo);
        bool verificaTituloEnviadoBoletoOuContrato(int cd_contrato, int cdEscola, List<int> cdsTitulo);
        IEnumerable<Titulo> getEditTitulosReajusteAnual(int cd_escola, int cd_reajuste_anual);
        IEnumerable<Titulo> getTitulosReajusteAnual(int cd_escola, int cd_reajuste_anual, DateTime dt_ini_venc, DateTime? dt_fim_venc);
        decimal getSaldoContratoParaReajusteAnual(int cd_escola, decimal pc_bolsa, int cd_contrato, List<int> cdsTitulos);
        IEnumerable<Titulo> getTitulosReajusteAnual(int cd_escola, int cd_reajuste_anual);
        List<int> getAlunosQuePossuemTitulosAbertoMes(List<int> cdPessoaAlunos, int cd_empresa, DateTime dt_diario);
        List<Titulo> getTitulosByTituloAditamento(int cd_escola, int cd_aditamento);
        bool deleteTitulosByTituloAditamento(int cd_escola, string cds_titulos);
        List<Titulo> getTitulosSemBaixa(int cd_escola, List<Titulo> titulos);
        List<Titulo> getTitulosbyTranFinan(int cd_escola, int cd_tran_finan);
        List<BaixaTitulo> getBaixasByCdTitulo(int cd_escola, int cd_titulo, int cd_tran_finan);

        IEnumerable<Cheque> getChequeTransacaoTrocaFinanceira(int cd_titulo, int cdEscola);
        ReciboConfirmacaoUI getReciboConfirmacaoByContrato(int cd_contrato, int cd_empresa);
        ReciboConfirmacaoUI getReciboConfirmacaoByMovimento(int cd_movimento, int cd_empresa);
        List<ReciboConfirmacaoParcelasUI> getParcelasReciboConfirmacaoByContrato(int cd_contrato, int cd_empresa);
        List<ReciboConfirmacaoParcelasUI> getParcelasReciboConfirmacaoByMovimento(int cd_movimento, int cd_empresa);
        Decimal getValorBaixasOutras(int cd_titulo);
        int excluirAditamentoProcedure(int cd_aditamento);

        List<Titulo> getTitulosByContratoEscola(int cdContrato, int cdEscola, List<int> cds_titulos);

        bool updateNossoNumeroTitulo(int numeroDocumento, int cd_pessoa_empresa, int cd_local_movto, string nossoNumero);
        string delTitulosContrato(int cd_contrato, bool existeBolsaContrato, string json);
    }
}
