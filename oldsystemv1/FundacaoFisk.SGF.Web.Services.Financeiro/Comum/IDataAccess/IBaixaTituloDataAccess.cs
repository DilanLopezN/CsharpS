using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface IBaixaTituloDataAccess : IGenericRepository<BaixaTitulo>
    {
        IEnumerable<BaixaTitulo> getBaixaTituloByIdTitulo(int cd_titulo, int cd_pessoa_empresa);
        IEnumerable<BaixaTitulo> getBaixasTransacaoFinan(int cd_transacao_finan, int cd_baixa,int cd_titulo, int cd_pessoa_empresa, BaixaTituloDataAccess.TipoConsultaBaixaEnum tipoConsulta);
        Recibo getReciboByBaixa(int cd_baixa, int cd_empresa);
        ReciboAgrupadoUI getReciboAgrupado(string cds_titulos_selecionados, int cd_empresa);
        ReciboPagamentoUI getReciboPagamentoByBaixa(int cd_baixa, int cd_empresa);

		ReciboPagamentoUI getVerificaReciboPagamentoByBaixa(int cd_baixa, int cd_empresa);
        bool validaReciboAgrupadoAlunosResponsaveisDiferentes(List<int> cds_titulos_selecionados, int cd_escola, int tipo_validacao);
        bool verificarTituloOrigemMatricula(int cd_baixa, int cd_escola);
        IEnumerable<BaixaTitulo> getBaixaTitulosBolsaContrato(int cd_contrato, int cd_pessoa_empresa);
        IEnumerable<BaixaTitulo> getBaixaTitulosBolsaContrato(int cd_contrato, int cd_pessoa_empresa, List<int> cdTitulos);
        IEnumerable<BaixaEfetuadaChequeUI> getBaixasEfetuadasForBaixaAutomaticaCheque(SearchParameters parametros, BaixaAutomaticaUI automaticaChequeUi);
        IEnumerable<BaixaEfetuadaChequeUI> getBaixasEfetuadasForBaixaAutomaticaCartao(SearchParameters parametros, BaixaAutomaticaUI automaticaChartaoUi);
        bool baixaMotivoBolsaContrato(int cd_baixa, int cd_escola);
        bool verificaBaixaTituloByTurma(int cd_turma, int cd_escola, DateTime dtPol);
        bool verificaBaixaTituloByAluno(int cd_aluno, int cd_escola, DateTime dtPol);
        bool verificaBaixaTituloByPolAluno(int cd_aluno, int cd_escola, int cd_poltica);
        bool verificaBaixaTituloByPolTurma(int cd_turma, int cd_escola, int cd_poltica);
        bool verificaBaixaAposDataPol(int cd_escola, DateTime dtPol);
        int getUltimoNroRecibo(int? nm_ultimo_Recibo, int cd_empresa);
    }
}
