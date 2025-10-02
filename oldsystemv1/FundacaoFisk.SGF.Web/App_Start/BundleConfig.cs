using System.Web;
using System;
using System.Configuration;
using System.Web.Optimization;

namespace FundacaoFisk.SGF.Web {
    public class BundleConfig {
        public static void RegisterBundles(BundleCollection bundles) {
            BundleTable.EnableOptimizations = false;

            //_______________=============== SCRIPTS ===============_______________
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/util").Include(
                        "~/Scripts/util_v31.js",
                         "~/Scripts/apresentadorMensagem_v31.js"
            ));

            //_______________=============== CSS ===============_______________

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/screen.css",
                         "~/Content/dojoSGF.css",
                         "~/Content/jquerySGF.css",
                         "~/Content/themes/base/dojo1.8.5r/dojo/resources/dojo.css",
                         "~/Content/themes/base/dojo1.8.5r/dojox/grid/resources/Grid.css",
                         "~/Content/themes/base/dojo1.8.5r/dojox/grid/resources/claroGrid.css",
                         "~/Content/themes/base/dojo1.8.5r/dojox/grid/enhanced/resources/claro/EnhancedGrid.css",
                         "~/Content/themes/base/dojo1.8.5r/dojox/grid/enhanced/resources/EnhancedGrid_rtl.css"
            ));

            bundles.Add(new StyleBundle("~/Content/css/themes").Include(
                        "~/Content/themes/base/dojo1.8.5r/dijit/themes/tundra/tundra.css",
                        "~/Content/themes/base/dojo1.8.5r/dijit/themes/claro/claro.css"
            ));
            RegisterBundlesSGF(bundles);
        }

        public static void RegisterBundlesSGF(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/aluno").Include(
                        "~/Scripts/SGF/aluno-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/alunoFK").Include(
                        "~/Scripts/SGF/alunoFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/atividadeExtra").Include(
                        "~/Scripts/SGF/atividadeExtra-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/aulaPersonalizada").Include(
                        "~/Scripts/SGF/aulaPersonalizada-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/auxiliaresPessoa").Include(
                        "~/Scripts/SGF/auxiliaresPessoa-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/auxiliaresPessoaFK").Include(
                        "~/Scripts/SGF/auxiliaresPessoaFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/avaliacao").Include(
                        "~/Scripts/SGF/avaliacao-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/avaliacaoParticipacao").Include(
                        "~/Scripts/SGF/avaliacaoParticipacao-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/avaliacaoTurma").Include(
                        "~/Scripts/SGF/avaliacaoTurma-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/baixaFinanceira").Include(
                        "~/Scripts/SGF/baixaFinanceira-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/baixaFinanceiraAux").Include(
                        "~/Scripts/SGF/baixaFinanceiraAux-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/biblioteca").Include(
                        "~/Scripts/SGF/biblioteca-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/cadBaixaTituloPartial").Include(
                        "~/Scripts/SGF/cadBaixaTituloPartial-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/cadDesistenciaPartial").Include(
                        "~/Scripts/SGF/cadDesistenciaPartial-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/cadDiarioAulaPartial").Include(
                        "~/Scripts/SGF/cadDiarioAulaPartial-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/cadEnderecoAux").Include(
                        "~/Scripts/SGF/cadEnderecoAux-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/cadFollowUpPartial").Include(
                        "~/Scripts/SGF/cadFollowUpPartial-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/cadMailMarketing").Include(
                        "~/Scripts/SGF/cadMailMarketing-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/cadMatriculaPartial").Include(
                        "~/Scripts/SGF/cadMatriculaPartial-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/cadPessoaPartial").Include(
                        "~/Scripts/SGF/cadPessoaPartial-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/CFOPFK").Include(
                        "~/Scripts/SGF/CFOPFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/chequeFK").Include(
                        "~/Scripts/SGF/chequeFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/cidadeFk").Include(
                        "~/Scripts/SGF/cidadeFk-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/cnabBoleto").Include(
                        "~/Scripts/SGF/cnabBoleto-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/configuracaoSistema").Include(
                        "~/Scripts/SGF/configuracaoSistema-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/contaCorrente").Include(
                        "~/Scripts/SGF/contaCorrente-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/coordenacao").Include(
                        "~/Scripts/SGF/coordenacao-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/criteriosavaliacoes").Include(
                        "~/Scripts/SGF/criteriosavaliacoes-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/curso").Include(
                        "~/Scripts/SGF/curso-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/cursoFK").Include(
                        "~/Scripts/SGF/cursoFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/desistencia").Include(
                        "~/Scripts/SGF/desistencia-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/diarioAula").Include(
                        "~/Scripts/SGF/diarioAula-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/escola").Include(
                        "~/Scripts/SGF/escola-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/faturamento").Include(
                        "~/Scripts/SGF/faturamento-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/fechamentoCaixa").Include(
                        "~/Scripts/SGF/fechamentoCaixa-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/fechamentoEstoque").Include(
                        "~/Scripts/SGF/fechamentoEstoque-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/fileManager").Include(
                        "~/Scripts/SGF/fileManager-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/financeiro").Include(
                        "~/Scripts/SGF/financeiro-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/fiscal").Include(
                        "~/Scripts/SGF/fiscal-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/followUp").Include(
                        "~/Scripts/SGF/followUp-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/funcionario").Include(
                        "~/Scripts/SGF/funcionario-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/grupo").Include(
                        "~/Scripts/SGF/grupo-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/historicoAluno").Include(
                        "~/Scripts/SGF/historicoAluno-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/item").Include(
                        "~/Scripts/SGF/item-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/itemFK").Include(
                        "~/Scripts/SGF/itemFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/localidades").Include(
                        "~/Scripts/SGF/localidades-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/localMovto").Include(
                        "~/Scripts/SGF/localMovto-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                        "~/Scripts/SGF/login-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/logradouroFK").Include(
                        "~/Scripts/SGF/logradouroFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/mail_marketing").Include(
                        "~/Scripts/SGF/mail_marketing-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/mail_marketingFK").Include(
                        "~/Scripts/SGF/mail_marketingFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/matricula").Include(
                        "~/Scripts/SGF/matricula-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/modelo_programacao_turma").Include(
                        "~/Scripts/SGF/modelo_programacao_turma-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/motivoMatricula").Include(
                        "~/Scripts/SGF/motivoMatricula-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/motivoNaoMatricula").Include(
                        "~/Scripts/SGF/motivoNaoMatricula-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/movimentoFK").Include(
                        "~/Scripts/SGF/movimentoFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/movimentos").Include(
                        "~/Scripts/SGF/movimentos-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/mudancaInterna").Include(
                        "~/Scripts/SGF/mudancaInterna-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/nomeContrato").Include(
                        "~/Scripts/SGF/nomeContrato-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/pessoa").Include(
                        "~/Scripts/SGF/pessoa-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/pessoaFK").Include(
                        "~/Scripts/SGF/pessoaFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/pessoaTurmaFK").Include(
                "~/Scripts/SGF/pessoaTurmaFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/pessoaRelFK").Include(
                        "~/Scripts/SGF/pessoaRelFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/planoConta").Include(
                        "~/Scripts/SGF/planoConta-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/planoContasFK").Include(
                        "~/Scripts/SGF/planoContasFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/politicaComercial").Include(
                        "~/Scripts/SGF/politicaComercial-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/politicaComercialFk").Include(
                        "~/Scripts/SGF/politicaComercialFk-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/politicaDesconto").Include(
                        "~/Scripts/SGF/politicaDesconto-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/professorFK").Include(
                        "~/Scripts/SGF/professorFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/programacaocurso").Include(
                        "~/Scripts/SGF/programacaocurso-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/prospect").Include(
                        "~/Scripts/SGF/prospect-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/prospectFK").Include(
                        "~/Scripts/SGF/prospectFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reajusteAnual").Include(
                        "~/Scripts/SGF/reajusteAnual-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportAlunoCliente").Include(
                        "~/Scripts/SGF/reportAlunoCliente-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportAulaPersonalizada").Include(
                        "~/Scripts/SGF/reportAulaPersonalizada-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportBalanceteMensal").Include(
                        "~/Scripts/SGF/reportBalanceteMensal-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportBolsistas").Include(
                        "~/Scripts/SGF/reportBolsistas-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportCheques").Include(
                        "~/Scripts/SGF/reportCheques-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportComissaoSecreataria").Include(
                        "~/Scripts/SGF/reportComissaoSecreataria-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportContaCorrente").Include(
                        "~/Scripts/SGF/reportContaCorrente-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportControleSala").Include(
                        "~/Scripts/SGF/reportControleSala-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportDiarioAula").Include(
                        "~/Scripts/SGF/reportDiarioAula-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportEstoque").Include(
                        "~/Scripts/SGF/reportEstoque-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportListagemAniversariantes").Include(
                        "~/Scripts/SGF/reportListagemAniversariantes-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportMatricula").Include(
                        "~/Scripts/SGF/reportMatricula-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportMediasAluno").Include(
                        "~/Scripts/SGF/reportMediasAluno-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportPagamentoProfessores").Include(
                        "~/Scripts/SGF/reportPagamentoProfessores-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/ReportPercentualTerminoEstagio").Include(
                        "~/Scripts/SGF/ReportPercentualTerminoEstagio-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportPosicaoFinanceira").Include(
                        "~/Scripts/SGF/reportPosicaoFinanceira-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportProgramacaoAulasTurma").Include(
                        "~/Scripts/SGF/reportProgramacaoAulasTurma-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportProspect").Include(
                        "~/Scripts/SGF/reportProspect-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportSaldoFinanceiro").Include(
                        "~/Scripts/SGF/reportSaldoFinanceiro-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportTurma").Include(
                        "~/Scripts/SGF/reportTurma-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportTurmaMatriculaMaterial").Include(
                        "~/Scripts/SGF/reportTurmaMatriculaMaterial-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/retornoCNAB").Include(
                        "~/Scripts/SGF/retornoCNAB-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/secretaria").Include(
                        "~/Scripts/SGF/secretaria-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/subgrupoContasFK").Include(
                        "~/Scripts/SGF/subgrupoContasFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/tabelaPreco").Include(
                        "~/Scripts/SGF/tabelaPreco-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/tipoDescontoFK").Include(
                        "~/Scripts/SGF/tipoDescontoFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/tipoNFFk").Include(
                        "~/Scripts/SGF/tipoNFFk-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/tiposAvaliacoes").Include(
                        "~/Scripts/SGF/tiposAvaliacoes-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/tituloFK").Include(
                        "~/Scripts/SGF/tituloFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/turma").Include(
                        "~/Scripts/SGF/turma-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/turmaFK").Include(
                        "~/Scripts/SGF/turmaFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/turmaPersonalizadaFK").Include(
                        "~/Scripts/SGF/turmaPersonalizadaFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/usuario").Include(
                        "~/Scripts/SGF/usuario-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/usuarioFK").Include(
                        "~/Scripts/SGF/usuarioFK-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/usuarioSenha").Include(
                        "~/Scripts/SGF/usuarioSenha-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportControleVendasMaterial").Include(
                        "~/Scripts/SGF/reportControleVendasMaterial-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/calendarioEvento").Include(
                        "~/Scripts/SGF/calendarioEvento-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/calendarioAcademico").Include(
                        "~/Scripts/SGF/calendarioAcademico-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/circulares").Include(
                        "~/Scripts/SGF/circulares-{version}.js"));
 			bundles.Add(new ScriptBundle("~/bundles/video").Include(
                        "~/Scripts/SGF/video-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/videoDetail").Include(
                        "~/Scripts/SGF/videoDetail-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportAtividadeExtra").Include(
                        "~/Scripts/SGF/reportAtividadeExtra-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportHistoricoAluno").Include(
                "~/Scripts/SGF/reportHistoricoAluno-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/kitVendas").Include(
                "~/Scripts/SGF/kitVendas-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/malaDireta").Include(
                        "~/Scripts/SGF/mala_direta-{version}.js"));
 			bundles.Add(new ScriptBundle("~/bundles/controleFalta").Include(
                "~/Scripts/SGF/controleFalta-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportControleFaltas").Include(
                "~/Scripts/SGF/reportControleFaltas-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/baixaAutomaticaCartao").Include(
                "~/Scripts/SGF/baixaAutomaticaCartao-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/baixaAutomaticaCheque").Include(
                "~/Scripts/SGF/baixaAutomaticaCheque-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/smsGestao").Include(
                "~/Scripts/SGF/smsGestao-{version}.js"));
			bundles.Add(new ScriptBundle("~/bundles/geracaoNotasXml").Include(
				"~/Scripts/SGF/geracaoNotasXML-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/aulasReposicao").Include(
                "~/Scripts/SGF/aulasReposicao-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportAulaReposicao").Include(
                        "~/Scripts/SGF/reportAulaReposicao-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportAlunosSemTituloGerado").Include(
                "~/Scripts/SGF/reportAlunosSemTituloGerado-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/cadMatriculaAux").Include(
                "~/Scripts/SGF/cadMatriculaAux-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/faq").Include(
                       "~/Scripts/SGF/faq-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/faqVideoDetail").Include(
                       "~/Scripts/SGF/faqVideoDetail-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportFaixaEtaria").Include(
                "~/Scripts/SGF/reportFaixaEtaria-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportInventario").Include(
                "~/Scripts/SGF/reportInventario-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportAlunoRestricao").Include(
                "~/Scripts/SGF/reportAlunoRestricao-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportFollowUp").Include(
                "~/Scripts/SGF/reportFollowUp-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportAvaliacao").Include(
                "~/Scripts/SGF/reportAvaliacao-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportMatriculaOutros").Include(
                        "~/Scripts/SGF/reportMatriculaOutros-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportCartaQuitacao").Include(
                        "~/Scripts/SGF/reportCartaQuitacao-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/alunosemAula").Include(
                "~/Scripts/SGF/alunosemAula-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/enviarTransferencia").Include(
                        "~/Scripts/SGF/enviarTransferencia-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/receberTransferencia").Include(
                        "~/Scripts/SGF/receberTransferencia-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/professorsemdiario").Include(
                "~/Scripts/SGF/professorsemdiario-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/alunoscargahoraria").Include(
                "~/Scripts/SGF/alunoscargahoraria-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/consultarCargasHorarias").Include(
                "~/Scripts/SGF/consultarCargasHorarias-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/consultarRafsemDiario").Include(
                "~/Scripts/SGF/consultarRafsemDiario-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportLoginEscola").Include(
                "~/Scripts/SGF/reportLoginEscola-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/perdaMaterial").Include(
                "~/Scripts/SGF/perdaMaterial-{version}.js"));

        }
    }
}

