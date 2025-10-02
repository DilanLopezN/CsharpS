using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericController;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using log4net;
using Newtonsoft.Json;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
//using Componentes.Utils.Messages;
using static Componentes.GenericController.ReturnResult.MensagemWeb;
using static FundacaoFisk.SGF.GenericModel.ContaCorrente;
using System.Collections;
using FundacaoFisk.SGF.Services.Coordenacao.Business;
using System.CodeDom;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Controllers
{
    public class TurmaController : ComponentesApiController
    {
        public enum TipoFormException
        {
            TURMA = 1,
            AVALIACAO_TURMA = 2
        }

        //Declaração de Atributos
        private static readonly ILog logger = LogManager.GetLogger(typeof(TurmaController));

        //Método construtor
        public TurmaController()
        {
        }

        #region Turma

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "func")]
        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getTurmaSearch(string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, int prog, bool turmasFilhas, int cdAluno, string dtInicial, string dtFinal, int? cd_turma_PPT, bool semContrato, bool profTurmasAtuais, int cd_escola_combo, int ckOnLine, string dias,
                                    int cd_search_sala, int cd_search_sala_online, bool ckSearchSemSala, bool ckSearchSemAluno)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                //para não ter que modificar o métodp passar zero para cd_escola_combo
                var retorno = turmaBiz.searchTurma(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, ProgramacaoTurma.parseTipoConsultaProg(prog), cdEscola, turmasFilhas, cdAluno, 0, dtaInicial, dtaFinal, cd_turma_PPT, semContrato, (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA, null, null, profTurmasAtuais, cd_search_sala, cd_search_sala_online, ckSearchSemSala, ckSearchSemAluno, null, cd_escola_combo, 0, ckOnLine, dias);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getTurmaSearchFK(string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, int prog, bool turmasFilhas, int cdAluno, int origemFK, string dtInicial, string dtFinal, int? cd_turma_PPT, bool semContrato, string dataInicial, string dataFinal, int cd_escola_combo_fk, int diaSemanaTurma, int ckOnLine)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                DateTime? dt_inicial = string.IsNullOrEmpty(dataInicial) ? null : (DateTime?)Convert.ToDateTime(dataInicial);
                DateTime? dt_final = string.IsNullOrEmpty(dataFinal) ? null : (DateTime?)Convert.ToDateTime(dataFinal);
                
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();

                int cdUsuario = this.ComponentesUser.CodUsuario;
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();

                ProfessorUI prof = profBiz.verificaRetornaSeUsuarioLogadoEProfessor(cdPessoaUsuario, cdEscola);
                int tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA;
                if (prof != null && prof.cd_pessoa > 0 && !this.ComponentesUser.IdMaster && !prof.id_coordenador)
                    tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_FK_TURMA_PROF;
                if (origemFK == (int)TurmaDataAccess.TipoConsultaTurmaEnum.REPORT_AVALIACAO)
                    tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_REPORT_AVALIACAO;
                if (origemFK == (int)TurmaDataAccess.TipoConsultaTurmaEnum.REPORT_AVALIACAO_CONCEITO)
                    tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_REPORT_AVALIACAO_CONCEITO;
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = turmaBiz.searchTurma(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, ProgramacaoTurma.parseTipoConsultaProg(prog), cdEscola, turmasFilhas, cdAluno, origemFK, dtaInicial, dtaFinal, cd_turma_PPT, semContrato, tipoPesq, dt_inicial, dt_final, false, 0, 0, false, false, null, cd_escola_combo_fk, diaSemanaTurma,ckOnLine);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getTurmaSearchAulaReposicaoDestinoFK(string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, int prog, bool turmasFilhas, int cdAluno, int origemFK, string dtInicial, string dtFinal, int? cd_turma_PPT, bool semContrato, string dataInicial, string dataFinal, bool ckOnline, string dt_programacao, int cd_estagio, int cd_turma_origem)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                DateTime? dt_inicial = string.IsNullOrEmpty(dataInicial) ? null : (DateTime?)Convert.ToDateTime(dataInicial);
                DateTime? dt_final = string.IsNullOrEmpty(dataFinal) ? null : (DateTime?)Convert.ToDateTime(dataFinal);

                DateTime? dt_prog = string.IsNullOrEmpty(dt_programacao) ? null : (DateTime?)Convert.ToDateTime(dt_programacao);

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();

                int cdUsuario = this.ComponentesUser.CodUsuario;
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();

                ProfessorUI prof = profBiz.verificaRetornaSeUsuarioLogadoEProfessor(cdPessoaUsuario, cdEscola);
                int tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA;
                if (prof != null && prof.cd_pessoa > 0 && !this.ComponentesUser.IdMaster && !prof.id_coordenador)
                    tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_FK_TURMA_PROF;
                if (origemFK == 31)
                    tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_REPORT_AVALIACAO;
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = turmaBiz.getTurmaSearchAulaReposicaoDestinoFK(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, ProgramacaoTurma.parseTipoConsultaProg(prog), cdEscola, turmasFilhas, cdAluno, origemFK, dtaInicial, dtaFinal, cd_turma_PPT, semContrato, tipoPesq, dt_inicial, dt_final, false, dt_prog, cd_estagio, cd_turma_origem, null, ckOnline);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage verificaDiaSemanaTurmaFollowUp(int cdTurma, int idDiaSemanaTurma)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();

                var retorno = turmaBiz.verificaDiaSemanaTurmaFollowUp(cdEscola, cdTurma, idDiaSemanaTurma);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getTurmaSearchFKRelTurmaMatriculaMaterial(string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, int prog, bool turmasFilhas, int cdAluno, string dtInicial, string dtFinal, int? cd_turma_PPT, bool semContrato, string dataInicial, string dataFinal, bool semmaterial)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                DateTime? dt_inicial = string.IsNullOrEmpty(dataInicial) ? null : (DateTime?)Convert.ToDateTime(dataInicial);
                DateTime? dt_final = string.IsNullOrEmpty(dataFinal) ? null : (DateTime?)Convert.ToDateTime(dataFinal);
                
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                int tipoPesq = 0;
                if(!semmaterial)
                    tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_NOTA_MATERIAL;
                else
                    tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.TODAS;
                //ProfessorUI prof = profBiz.verificaRetornaSeUsuarioLogadoEProfessor(cdPessoaUsuario, cdEscola);
                //if (prof != null && prof.cd_pessoa > 0 && !this.ComponentesUser.IdMaster && !prof.id_coordenador)
                //    tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_FK_TURMA_PROF;
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = turmaBiz.searchTurma(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, ProgramacaoTurma.parseTipoConsultaProg(prog), cdEscola, turmasFilhas, cdAluno, 0, dtaInicial, dtaFinal, cd_turma_PPT, semContrato, tipoPesq, dt_inicial, dt_final,false,0, 0, false ,false, null);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getTurmaReajusteSearchFK(string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, int prog, bool turmasFilhas, int cdAluno, string dtInicial, string dtFinal, int? cd_turma_PPT, bool semContrato, string dataInicial, string dataFinal)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                DateTime? dt_inicial = string.IsNullOrEmpty(dataInicial) ? null : (DateTime?)Convert.ToDateTime(dataInicial);
                DateTime? dt_final = string.IsNullOrEmpty(dataFinal) ? null : (DateTime?)Convert.ToDateTime(dataFinal);
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                int tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA;
                ProfessorUI prof = profBiz.verificaRetornaSeUsuarioLogadoEProfessor(cdPessoaUsuario, cdEscola);
                if (prof != null && prof.cd_pessoa > 0 && !this.ComponentesUser.IdMaster && !prof.id_coordenador)
                    tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_FK_TURMA_PROF;
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = turmaBiz.searchTurma(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, ProgramacaoTurma.parseTipoConsultaProg(prog), cdEscola, turmasFilhas, cdAluno, 0, dtaInicial, dtaFinal, cd_turma_PPT, semContrato, tipoPesq, dt_inicial, dt_final,false, 0, 0, false, false);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getTurmaSearchComAluno(string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
                                                            int cdProfessor, int prog, bool turmasFilhas, int cdAluno, string dtInicial, string dtFinal,
                                                            int? cd_turma_PPT, int cdTurmaOri, int opcao, int cd_escola_combo_fk)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();

                int cdUsuario = this.ComponentesUser.CodUsuario;
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;
                int tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA;
                ProfessorUI prof = profBiz.verificaRetornaSeUsuarioLogadoEProfessor(cdPessoaUsuario, cdEscola);
                if (prof != null && prof.cd_pessoa > 0 && !this.ComponentesUser.IdMaster && !prof.id_coordenador)
                    tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_FK_TURMA_PROF;
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = turmaBiz.searchTurmaComAluno(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, ProgramacaoTurma.parseTipoConsultaProg(prog), cdEscola, turmasFilhas, cdAluno, dtaInicial, dtaFinal, cd_turma_PPT, cdTurmaOri, opcao, tipoPesq, cd_escola_combo_fk);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getTurmaSearchFKAvaliacao(string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, int prog, bool turmasFilhas, int cdAluno, string dtInicial, string dtFinal, int? cd_turma_PPT, bool semContrato, string dataInicial, string dataFinal, int cd_escola_combo_fk)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                DateTime? dt_inicial = string.IsNullOrEmpty(dataInicial) ? null : (DateTime?)Convert.ToDateTime(dataInicial);
                DateTime? dt_final = string.IsNullOrEmpty(dataFinal) ? null : (DateTime?)Convert.ToDateTime(dataFinal);
                //string[] situacao = cdSituacoesAlunoTurma.Split('|');
                List<int> cdsSituacoes = new List<int>() { (int)AlunoTurma.SituacaoAlunoTurma.Ativo, (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado, (int)AlunoTurma.SituacaoAlunoTurma.Encerrado, (int)AlunoTurma.SituacaoAlunoTurma.Movido };

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;

                int cdUsuario = this.ComponentesUser.CodUsuario;
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();

                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = turmaBiz.searchTurma(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, ProgramacaoTurma.parseTipoConsultaProg(prog), cdEscola, turmasFilhas, cdAluno, 0, dtaInicial, dtaFinal, cd_turma_PPT, semContrato, (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_AVALICAO_TURMA, dt_inicial, dt_final, false, 0, 0, false, false, cdsSituacoes, cd_escola_combo_fk);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }



        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "func")]
        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage componentesPesquisaTurma()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ComponentesTurma componentesTurma = new ComponentesTurma();
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();

                componentesTurma.duracoes = coordenacaoBiz.getDuracoes(DuracaoDataAccess.TipoConsultaDuracaoEnum.HAS_TURMA, null, cdEscola).ToList();
                componentesTurma.produtos = coordenacaoBiz.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_TURMA, null, cdEscola).ToList();
                componentesTurma.cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_TURMA, null, null, cdEscola).ToList();
                ProfessorUI prof = profBiz.verificaRetornaSeUsuarioLogadoEProfessor(cdPessoaUsuario, cdEscola);
                if (prof != null && prof.cd_pessoa > 0 && !this.ComponentesUser.IdMaster && !prof.id_coordenador)
                {
                    componentesTurma.professores = new List<ProfessorUI>();
                    componentesTurma.professores.Add(prof);
                    componentesTurma.usuarioSisProf = true;
                }
                else
                    componentesTurma.professores = profBiz.getProfessorReturnProfUI(ProfessorDataAccess.TipoConsultaProfessorEnum.HAS_TURMA, cdEscola, null).ToList();
                retorno.retorno = componentesTurma;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage componentesPesquisaMediaAlunos()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ComponentesTurma componentesTurma = new ComponentesTurma();
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();

                componentesTurma.produtos = coordenacaoBiz.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_TURMA, null, cdEscola).ToList();
                componentesTurma.cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_TURMA, null, null, cdEscola).ToList();

                retorno.retorno = componentesTurma;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "reg")]
        public HttpResponseMessage componentesNovaTurma(int? cdDuracao, int? cdProduto, int? cdRegime, int? cdCurso)
        {
            ReturnResult retorno = new ReturnResult();
            ComponentesTurma componentesTurma = new ComponentesTurma();
            //int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                componentesTurma.duracoes = coordenacaoBiz.getDuracoes(DuracaoDataAccess.TipoConsultaDuracaoEnum.HAS_ATIVO, cdDuracao, null).ToList();
                componentesTurma.produtos = coordenacaoBiz.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_ATIVO_CURSO, cdProduto, null).ToList();
                componentesTurma.regimes = coordenacaoBiz.getRegimes(RegimeDataAccess.TipoConsultaRegimeEnum.HAS_ATIVO, cdRegime).ToList();
                retorno.retorno = componentesTurma;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "reg")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage getTurmaAndComponentesByTurmaEdit(int cdTurma)
        {
            ReturnResult retorno = new ReturnResult();
            Turma turma = new Turma();
            ComponentesTurma componentesTurma = new ComponentesTurma();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                turma = turmaBiz.buscarTurmaHorariosEdit(cdTurma, cdEscola, TurmaDataAccess.TipoConsultaTurmaEnum.HAS_BUSCA_TURMA);
                if (turma != null)
                {
                    componentesTurma.turma = Turma.changeValueRetornView(turma);
                    if (turma.cd_duracao > 0)
                        componentesTurma.duracoes = coordenacaoBiz.getDuracoes(DuracaoDataAccess.TipoConsultaDuracaoEnum.HAS_ATIVO, turma.cd_duracao, null).ToList();
                    if (turma.cd_produto > 0)
                        componentesTurma.produtos = coordenacaoBiz.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_ATIVO, turma.cd_produto, null).ToList();
                    if (turma.cd_regime > 0)
                        componentesTurma.regimes = coordenacaoBiz.getRegimes(RegimeDataAccess.TipoConsultaRegimeEnum.HAS_ATIVO, turma.cd_regime).ToList();
                    if (turma.cd_curso > 0)
                    {
                        int? cd_produto = null;
                        if (turma.cd_produto > 0)
                            cd_produto = turma.cd_produto;
                        componentesTurma.cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_ATIVOPROD, turma.cd_curso, cd_produto, null).ToList();
                    }
                    else
                    {
                        componentesTurma.cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_ATIVO, null, null, null).ToList();
                    }
                    if (turma.alunosTurma != null && turma.alunosTurma.Count() > 0)
                    {
                        turma.situacoesAluno = turma.alunosTurma.GroupBy(x => new { x.cd_situacao_aluno_turma })
                                                    .Select(x => new AlunoTurma { cd_situacao_aluno_turma = x.Key.cd_situacao_aluno_turma }).ToList().OrderBy(x => x.situacaoAlunoTurma);
                        turma.alunosTurma = turma.alunosTurma.OrderBy(x => x.no_aluno);
                    }
                    if (turma.id_turma_ppt && turma.alunosTurmasPPTSearch != null && turma.alunosTurmasPPTSearch.Count() > 0)
                    {
                        List<AlunoTurma> alunos = turma.alunosTurmasPPTSearch.Where(x=> x.alunoTurma != null).Select(x => x.alunoTurma).ToList();
                        turma.situacoesAluno = alunos.GroupBy(x => new { x.cd_situacao_aluno_turma })
                                                    .Select(x => new AlunoTurma { cd_situacao_aluno_turma = x.Key.cd_situacao_aluno_turma }).ToList().OrderBy(x => x.situacaoAlunoTurma);
                        turma.alunosTurmasPPTSearch = turma.alunosTurmasPPTSearch.OrderBy(x => x.no_aluno);
                    }
                }
                else
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                retorno.retorno = componentesTurma;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "reg")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage getTurmaAndComponentesByTurmaEditVirada(int cdTurma)
        {
            ReturnResult retorno = new ReturnResult();
            Turma turma = new Turma();
            ComponentesTurma componentesTurma = new ComponentesTurma();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                turma = turmaBiz.buscarTurmaHorariosEditVirada(cdTurma, cdEscola, TurmaDataAccess.TipoConsultaTurmaEnum.HAS_BUSCA_TURMA);
                if (turma != null)
                {
                    componentesTurma.turma = Turma.changeValueRetornView(turma);
                    if (turma.cd_duracao > 0)
                        componentesTurma.duracoes = coordenacaoBiz.getDuracoes(DuracaoDataAccess.TipoConsultaDuracaoEnum.HAS_ATIVO, turma.cd_duracao, null).ToList();
                    if (turma.cd_produto > 0)
                        componentesTurma.produtos = coordenacaoBiz.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_ATIVO, turma.cd_produto, null).ToList();
                    if (turma.cd_regime > 0)
                        componentesTurma.regimes = coordenacaoBiz.getRegimes(RegimeDataAccess.TipoConsultaRegimeEnum.HAS_ATIVO, turma.cd_regime).ToList();
                    if (turma.cd_curso > 0)
                    {
                        int? cd_produto = null;
                        if (turma.cd_produto > 0)
                            cd_produto = turma.cd_produto;
                        componentesTurma.cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_ATIVOPROD, turma.cd_curso, cd_produto, null).ToList();
                    }
                    else
                    {
                        componentesTurma.cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_ATIVO, null, null, null).ToList();
                    }
                }
                else
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                retorno.retorno = componentesTurma;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getTurmaEdit(int cdTurma)
        {
            ReturnResult retorno = new ReturnResult();
            Turma turma = new Turma();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                turma = turmaBiz.buscarTurmaHorariosEdit(cdTurma, cdEscola, TurmaDataAccess.TipoConsultaTurmaEnum.HAS_BUSCA_DADOS_PPT);
                retorno.retorno = Turma.changeValueRetornView(turma);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getTurmaPoliticaEsc()
        {
            ReturnResult retorno = new ReturnResult();

            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                IEnumerable<Turma> turma = turmaBiz.getTurmaPoliticaEsc(cdEscola);
                retorno.retorno = turma;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        
        [HttpComponentesAuthorize(Roles = "tur.i")]
        public HttpResponseMessage postInsertTurma(Turma turma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                ISecretariaBusiness secretariaBusiness = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { turmaBiz, coordenacaoBiz, cursoBiz, secretariaBusiness, profBiz, alunoBiz });
                turma.cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;
                int escolaTurma = turma.cd_pessoa_escola;
                if (turma.horariosTurma != null)
                {
                    string ret = turmaBiz.existeProgInsuficiente(turma);

                    if (ret != null && ret != "OK")
                    {
                        throw new TurmaBusinessException(ret, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_PROGRAMACAO_INSUFICIENTE, false);
                    }
                }
                if (!turma.id_turma_ppt)
                {
                    if (turma.FeriadosDesconsiderados != null)
                    {
                        List<FeriadoDesconsiderado> listaDesconsideraFeriados = turma.FeriadosDesconsiderados.ToList();
                        for (int i = 0; i < listaDesconsideraFeriados.Count; i++)
                        {
                            listaDesconsideraFeriados[i].dt_inicial = DateTime.Parse(listaDesconsideraFeriados[i].dta_inicial);
                            listaDesconsideraFeriados[i].dt_final = DateTime.Parse(listaDesconsideraFeriados[i].dta_final);
                        }
                        turma.FeriadosDesconsiderados = listaDesconsideraFeriados;
                    }
                    if (turma.ProgramacaoTurma != null)
                    {
                        List<ProgramacaoTurma> listaProgramacoes = turma.ProgramacaoTurma.ToList();
                        for (int i = 0; i < listaProgramacoes.Count; i++)
                        {
                            listaProgramacoes[i].dta_programacao_turma = DateTime.Parse(listaProgramacoes[i].dt_programacao_turma).Date;
                            if (!listaProgramacoes[i].dta_cadastro_programacao.HasValue)
                                listaProgramacoes[i].dta_cadastro_programacao = Utils.ConversorUTC.ToUniversalTime(DateTime.Now, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                        }
                        turma.ProgramacaoTurma = listaProgramacoes;
                    }
                    //Tirando hora da data de inicio e data de matricula
                    if (turma.alunosTurma != null && turma.alunosTurma.Count() > 0)
                        foreach (AlunoTurma at in turma.alunosTurma)
                        {
                            at.dt_matricula = at.dt_matricula.HasValue ? at.dt_matricula.Value.ToLocalTime().Date : at.dt_matricula;
                            at.dt_inicio = at.dt_inicio.HasValue ? at.dt_inicio.Value.ToLocalTime().Date : at.dt_inicio;
                        }
                }
                else
                {
                    if (turma.alunosTurmasPPT != null)
                    {
                        foreach (Turma t in turma.alunosTurmasPPT)
                        {
                            if (t.FeriadosDesconsiderados != null)
                            {
                                List<FeriadoDesconsiderado> listaDesconsideraFeriados = t.FeriadosDesconsiderados.ToList();
                                for (int i = 0; i < listaDesconsideraFeriados.Count; i++)
                                {
                                    listaDesconsideraFeriados[i].dt_inicial = DateTime.Parse(listaDesconsideraFeriados[i].dta_inicial);
                                    listaDesconsideraFeriados[i].dt_final = DateTime.Parse(listaDesconsideraFeriados[i].dta_final);
                                }
                                t.FeriadosDesconsiderados = listaDesconsideraFeriados;
                            }
                            if (t.ProgramacaoTurma != null)
                            {
                                List<ProgramacaoTurma> listaProgramacoes = t.ProgramacaoTurma.ToList();
                                for (int i = 0; i < listaProgramacoes.Count; i++)
                                {
                                    if (listaProgramacoes[i].cd_programacao_turma == 0)
                                        listaProgramacoes[i].dta_programacao_turma = DateTime.Parse(listaProgramacoes[i].dt_programacao_turma).Date;
                                    if (!listaProgramacoes[i].dta_cadastro_programacao.HasValue)
                                        listaProgramacoes[i].dta_cadastro_programacao = Utils.ConversorUTC.ToUniversalTime(DateTime.Now, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                                }
                                t.ProgramacaoTurma = listaProgramacoes;
                            }
                            //Tirando hora da data de inicio e data de matricula
                            if (t.alunosTurma != null && t.alunosTurma.Count() > 0)
                                foreach (AlunoTurma at in t.alunosTurma)
                                {
                                    at.dt_matricula = at.dt_matricula.HasValue ? at.dt_matricula.Value.ToLocalTime().Date : at.dt_matricula;
                                    at.dt_inicio = at.dt_inicio.HasValue ? at.dt_inicio.Value.ToLocalTime().Date : at.dt_inicio;
                                }
                        }
                    }
                }
                var turmaCad = coordenacaoBiz.addTurma(turma);
                if (turmaCad.cd_pessoa_escola == 0) turmaCad.cd_pessoa_escola = escolaTurma;
                retorno.retorno = turmaCad;
                if (turma.cd_turma <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (TurmaBusinessException ex)
            {
                if (ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA_TURMA)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                else
                    if (ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_HORARIOS_FILHAS_INTERCESSAO_HORARIOS_PPT)
                        return gerarLogException(ex.Message, retorno, logger, ex);
                    else
                      if(ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_EXISTE_PROGRAMACAO_INSUFICIENTE)
                        return gerarLogException(ex.Message, retorno, logger, ex);
                        else
                            return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);



            }
            catch (FuncionarioBusinessException ex)
            {
                if (ex.tipoErro == FuncionarioBusinessException.TipoErro.ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "tur.a")]
        public HttpResponseMessage postUpdateTurma(Turma turma)
        {
            ReturnResult retorno = new ReturnResult();
            bool outraescola;
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { turmaBiz, coordenacaoBiz });
                turma.dt_inicio_aula = turma.dt_inicio_aula.ToLocalTime();
                if (turma.dt_final_aula.HasValue)
                    turma.dt_final_aula = turma.dt_final_aula.Value.ToLocalTime();
                outraescola = turma.cd_pessoa_escola != (int)this.ComponentesUser.CodEmpresa;
                int escolaTurma = turma.cd_pessoa_escola;
                turma.cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa; //Atenção a escola quai pode ter sido trocada, mas neste caso não vamos salvar a turma
                TurmaSearch turmaCad = new TurmaSearch();
                //Tirando hora da data de inicio e data de matricula

                //if (turma.horariosTurma != null)
                //{
                //    string ret = turmaBiz.existeProgInsuficiente(turma);

                //    if (ret != null && ret != "OK")
                //    {
                //        throw new TurmaBusinessException(ret, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_PROGRAMACAO_INSUFICIENTE, false);
                //    }
                //}

                if (turma.alunosTurma != null && turma.alunosTurma.Count() > 0)
                    foreach (AlunoTurma at in turma.alunosTurma)
                    {
                        at.dt_matricula = at.dt_matricula.HasValue ? at.dt_matricula.Value.ToLocalTime().Date : at.dt_matricula;
                        at.dt_inicio = at.dt_inicio.HasValue ? at.dt_inicio.Value.ToLocalTime().Date : at.dt_inicio;
                    }

                if (!turma.id_turma_ppt)
                {
                    if (turma.FeriadosDesconsiderados != null)
                    {
                        List<FeriadoDesconsiderado> listaDesconsideraFeriados = turma.FeriadosDesconsiderados.ToList();
                        for (int i = 0; i < listaDesconsideraFeriados.Count; i++)
                        {
                            listaDesconsideraFeriados[i].dt_inicial = DateTime.Parse(listaDesconsideraFeriados[i].dta_inicial);
                            listaDesconsideraFeriados[i].dt_final = DateTime.Parse(listaDesconsideraFeriados[i].dta_final);
                        }
                        turma.FeriadosDesconsiderados = listaDesconsideraFeriados;
                    }
                    if (turma.ProgramacaoTurma != null)
                    {
                        List<ProgramacaoTurma> listaProgramacoes = turma.ProgramacaoTurma.ToList();
                        for (int i = 0; i < listaProgramacoes.Count; i++)
                        {
                            listaProgramacoes[i].dta_programacao_turma = DateTime.Parse(listaProgramacoes[i].dt_programacao_turma).Date;
                            if (!listaProgramacoes[i].dta_cadastro_programacao.HasValue)
                                listaProgramacoes[i].dta_cadastro_programacao = Utils.ConversorUTC.ToUniversalTime(DateTime.Now, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                        }
                        turma.ProgramacaoTurma = listaProgramacoes;
                    }
                }
                else
                {
                    if (turma.alunosTurmasPPT != null)
                    {
                        foreach (Turma t in turma.alunosTurmasPPT)
                        {
                            if (t.FeriadosDesconsiderados != null)
                            {
                                List<FeriadoDesconsiderado> listaDesconsideraFeriados = t.FeriadosDesconsiderados.ToList();
                                for (int i = 0; i < listaDesconsideraFeriados.Count; i++)
                                {
                                    listaDesconsideraFeriados[i].dt_inicial = DateTime.Parse(listaDesconsideraFeriados[i].dta_inicial);
                                    listaDesconsideraFeriados[i].dt_final = DateTime.Parse(listaDesconsideraFeriados[i].dta_final);
                                }
                                t.FeriadosDesconsiderados = listaDesconsideraFeriados;
                            }
                            if (t.ProgramacaoTurma != null)
                            {
                                List<ProgramacaoTurma> listaProgramacoes = t.ProgramacaoTurma.ToList();
                                for (int i = 0; i < listaProgramacoes.Count; i++)
                                {
                                    if (listaProgramacoes[i].cd_programacao_turma == 0)
                                        listaProgramacoes[i].dta_programacao_turma = DateTime.Parse(listaProgramacoes[i].dt_programacao_turma).Date;
                                    if (!listaProgramacoes[i].dta_cadastro_programacao.HasValue)
                                        listaProgramacoes[i].dta_cadastro_programacao = Utils.ConversorUTC.ToUniversalTime(DateTime.Now, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                                }
                                t.ProgramacaoTurma = listaProgramacoes;
                            }
                        }
                    }
                }
                turmaCad = coordenacaoBiz.editTurma(turma);
                if (turmaCad.cd_pessoa_escola == 0) turmaCad.cd_pessoa_escola = escolaTurma;
                
                retorno.retorno = turmaCad;
                if (turma.cd_turma <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (TurmaBusinessException ex)
            {
                if (ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA_TURMA
                    || ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_HORARIOS_FILHAS_INTERCESSAO_HORARIOS_PPT
                    || ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA_EFETUADO_PROGRAMACAO
                    || ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_DELETAR_ALUNOTURMA_MATRICULADO_REMATRICULADO
                    || ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_TURMA_NULO
                    || ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_TRIGGER_ALTERACAO_TURMA
                    || ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_EXISTE_PROGRAMACAO_INSUFICIENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (FuncionarioBusinessException ex)
            {
                if (ex.tipoErro == FuncionarioBusinessException.TipoErro.ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur.e")]
        public HttpResponseMessage postDeleteTurma(List<Turma> turmas)
        {
            ReturnResult retorno = new ReturnResult();
            var cd_escola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                ISecretariaBusiness secretariaBusiness = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { turmaBiz, coordenacaoBiz, cursoBiz, secretariaBusiness, profBiz, alunoBiz });
                int[] cdTurmas = null;
                int i;
                // Pegando códigos da Turma
                if (turmas != null && turmas.Count() > 0)
                {
                    i = 0;
                    int[] cdTurmasCont = new int[turmas.Count()];
                    foreach (var c in turmas)
                    {
                        cdTurmasCont[i] = c.cd_turma;
                        i++;
                    }
                    cdTurmas = cdTurmasCont;
                }
                var delTurma = turmaBiz.deleteTurmas(cdTurmas, cd_escola);
                retorno.retorno = delTurma;
                if (!delTurma)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (TurmaBusinessException ex)
            {
                if (ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_DELETAR_ALUNOTURMA_AGUARDANDO_MOVIDO ||
                    ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_DELETAR_TURMA_ALUNO_COM_CONTRATO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getHorarioByEscolaForTurma(int cd_turma)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ISecretariaBusiness secretariaBusiness = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var horariosTurma = secretariaBusiness.getHorarioByEscolaForRegistro(cdEscola, cd_turma, Horario.Origem.TURMA);
                retorno.retorno = horariosTurma;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage GeturlrelatorioTurma(string sort, int direction, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
            int cdProfessor, int prog, bool turmasFilhas, int cdAluno, string dtInicial, string dtFinal, int? cd_turma_PPT, bool semContrato, bool ProfTurmasAtuais, int ckOnline, string dias,
            int cd_search_sala, int cd_search_sala_online, bool ckSearchSemSala, bool ckSearchSemAluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cd_escola_combo = cdEscola;
                //Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao +"&@apelido="+ apelido + "&@inicio=" + inicio + "&@tipoTurma=" + tipoTurma + "&@cdCurso=" + cdCurso + "&@cdDuracao=" + cdDuracao + "&@cdProduto=" + cdProduto
                    + "&@situacaoTurma=" + situacaoTurma + "&@cdProfessor=" + cdProfessor + "&@prog=" + prog + "&@cd_escola=" + cdEscola + "&@turmasFilhas=" + turmasFilhas + "&@cdAluno=" + cdAluno
                    + "&@dtInicial=" + dtInicial + "&@dtFinal=" + dtFinal + "&@cdTurmaPPT=" + cd_turma_PPT + "&@semContrato=" + semContrato + "&@ProfTurmasAtuais=" + ProfTurmasAtuais + "&@cd_escola_combo=" + cd_escola_combo + "&@ckOnLine=" + ckOnline + "&@dias=" + dias 
                    + "&@cd_search_sala=" + cd_search_sala + "&@cd_search_sala_online=" + cd_search_sala_online + "&@ckSearchSemSala=" + ckSearchSemSala + "&@ckSearchSemAluno=" + ckSearchSemAluno + "&"
                    + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Turma&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.Turma;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [Obsolete] //Método está sendo mudado para a regra de verificar se a turma existe diário de aula ou não.
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage verificarTurmaExisteProgramacaoHorarioByCodHorario(int cd_turma, int cd_horario)
        {
            ReturnResult retorno = new ReturnResult();

            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                var existsProgHorarios = turmaBiz.verificarTurmaExisteProgramacaoHorario(cd_turma, null, cd_horario, cdEscola);
                retorno.retorno = existsProgHorarios;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (TurmaBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [Obsolete]
        [HttpPost]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage verificarTurmaExisteProgramacaoByArrayCodHorarios(TurmaAlunoProfessorHorario horariosTurma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                var existsProgHorarios = turmaBiz.verificarProgramacaoParaListaHorarios(horariosTurma.cd_turma, cdEscola, horariosTurma.horarios);
                retorno.retorno = existsProgHorarios;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (TurmaBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur.i")]
        public HttpResponseMessage getInsertTurmaEnc(int cdTurma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                ISecretariaBusiness secretariaBusiness = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { turmaBiz, coordenacaoBiz, cursoBiz, secretariaBusiness, profBiz, alunoBiz });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;

                var turmaCad = coordenacaoBiz.postNovaTurmaEnc(cdTurma, cdEscola, TurmaDataAccess.TipoConsultaTurmaEnum.HAS_BUSCA_TURMA);
                retorno.retorno = turmaCad;
                if (turmaCad.cd_turma <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (TurmaBusinessException ex)
            {
                if (ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA_TURMA ||
                    ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_NAO_EXISTE_ALUNO_ATIVO_PPT_FILHO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                else
                    if (ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_HORARIOS_FILHAS_INTERCESSAO_HORARIOS_PPT)
                        return gerarLogException(ex.Message, retorno, logger, ex);
                    else
                        return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (FuncionarioBusinessException ex)
            {
                if (ex.tipoErro == FuncionarioBusinessException.TipoErro.ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage verifProgHorariosEAlunosDispHorarios(TurmaAlunoProfessorHorario turmaAlunoProfHorario)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            List<Aluno> alunos = null;
            int cdTurma = 0;
            Horario horario = null;
            try
            {
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                if (turmaAlunoProfHorario != null)
                {
                    cdTurma = turmaAlunoProfHorario.cd_turma;
                    alunos = turmaAlunoProfHorario.alunos;
                    horario = turmaAlunoProfHorario.horario;
                }
                if (turmaAlunoProfHorario != null && turmaAlunoProfHorario.cd_turma_ppt != null && turmaAlunoProfHorario.cd_turma_ppt > 0)
                {
                    List<Aluno> allAlunosTurmasFilhas = new List<Aluno>();
                    if (turmaAlunoProfHorario.validarProgramacao && turmaAlunoProfHorario.horario != null)
                        turmaBiz.verificarTurmaExisteProgramacaoHorario(null, (int)turmaAlunoProfHorario.cd_turma_ppt, turmaAlunoProfHorario.horario.cd_horario, cdEscola);
                    allAlunosTurmasFilhas = alunoBiz.getAllAlunosTurmasFilhasPPT((int)turmaAlunoProfHorario.cd_turma_ppt, cdEscola).ToList();
                    if (allAlunosTurmasFilhas != null)
                        alunoBiz.verificarAlunosDisponiveisFaixaHorario(0, cdTurma, cdEscola, horario, allAlunosTurmasFilhas);
                }
                else if (turmaAlunoProfHorario != null)
                {
                    if (turmaAlunoProfHorario.validarProgramacao && turmaAlunoProfHorario.horario != null && cdTurma > 0)
                        turmaBiz.verificarTurmaExisteProgramacaoHorario(turmaAlunoProfHorario.cd_turma, null, turmaAlunoProfHorario.horario.cd_horario, cdEscola);
                }
                if (alunos != null && alunos.Count > 0)
                    alunoBiz.verificarAlunosDisponiveisFaixaHorario(cdTurma, null, cdEscola, horario, alunos);

                return Request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (AlunoBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (FuncionarioBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (TurmaBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getTurmasPersonalizadas(int cdProduto, string dtAula, string hrIni, string hrFim, int? cd_turma)
        {
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                DateTime data = (DateTime)Convert.ToDateTime(dtAula);
                TimeSpan horaInicial = TimeSpan.Parse(hrIni);
                TimeSpan horaFinal = TimeSpan.Parse(hrFim);
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                var retorno = turmaBiz.getTurmasPersonalizadas(cdProduto, data, horaInicial, horaFinal, cdEscola, cd_turma);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur.a")]
        public HttpResponseMessage postUpdateTurmaEnc(ICollection<Turma> turmasEnc)
        {
            ReturnResult retorno = new ReturnResult();
            var cd_usuario = ComponentesUser.CodUsuario;
            int fuso = (int)this.ComponentesUser.IdFusoHorario;
            List<Turma> turmas = turmasEnc.ToList();
            ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();

            try
            {
                //List<Turma> turmas = turmasEnc.ToList();

                //ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                ISecretariaBusiness secretariaBusiness = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { turmaBiz, coordenacaoBiz, cursoBiz, secretariaBusiness, profBiz, alunoBiz });
                var cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;
                //var cd_usuario = ComponentesUser.CodUsuario;
                foreach (Turma t in turmas)
                    t.cd_pessoa_escola = cd_pessoa_escola;
                List<TurmaSearch> turmasAlteradas = turmaBiz.editTurmaEncerramento(turmas, cd_usuario, fuso);
                retorno.retorno = turmasAlteradas;
                if (turmasAlteradas.Count() < 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (ApiNewCyberException ex)
            {
                string message = "ERRO ao tentar inativar a Turma no Cyber:" + ex.Message + ". O cancelamento do encerramento será efetuado automaticamente";
                int ret = 0;
                try
                {
                    ret = turmaBiz.postCancelarTurmasEncerramento(turmas, cd_usuario, fuso);
                }
                catch (Exception e)
                {
                    ret = 1;
                    var message2 = Utils.Utils.innerMessage(e);
                    if (message2 != "")
                        message = message + " ATENÇÂO: Não foi possível realizar o cancelamento automatico do encerramento devido ao erro: " + message2;
                    return gerarLogException(message, retorno, logger, ex);
                }
                return gerarLogException(message, retorno, logger, ex);
            }
            catch (TurmaBusinessException ex)
            {
                if (ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_TURMA_ENCERRADA)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    TurmaBusinessException fx = new TurmaBusinessException(message, ex, 0, false);
                    //retorno.AddMensagem(message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
                }
            }
        }

        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "reg")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpGet]
        public HttpResponseMessage getTurmasContrato(int cd_contrato, int cd_aluno)
        {
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                var retorno = turmaBiz.searchTurmasContrato(cd_contrato, cdEscola, cd_aluno);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getTurmaOrigem(int cdTurma, int cdAluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                Turma turma = turmaBiz.getTurmaOrigem(cdEscola, cdTurma, cdAluno);
                retorno.retorno = turma;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage postProcuraTurmasOrigem(List<AlunoTurma> alunosTurma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                List<Turma> turmas = turmaBiz.getTurmasOrigem(alunosTurma, cdEscola);
                retorno.retorno = turmas;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getTurmaAlunoAguard(int cd_pessoa_aluno)
        {
            ReturnResult retorno = new ReturnResult();
            Turma turma = new Turma();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            DateTime dataHoje = DateTime.Now;
            try
            {
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                turma = turmaBiz.getTurmaAlunoAguard(cd_pessoa_aluno, dataHoje, cdEscola);
                retorno.retorno = turma;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getTurmaComAlunoDesistencia(int cdAluno, int opcao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();

                var turma = turmaBiz.searchTurmaComAlunoDesistencia(cdEscola, cdAluno, opcao);
                retorno.retorno = turma;
                if (turma.cd_turma <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getUrlReporTurma(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int prog, bool turmasFilhas,
            string dtInicial, string dtFinal, string dtInicialFim, string dtFinalFim, bool mostrarTelefone, bool mostrarResp, int situacaoTurma, string situacaoAlunoTurma,
            int tipoOnline, int? tipoRelatorio, string dias)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                DateTime? dataIncial = string.IsNullOrEmpty(dtInicial) ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dataFinal = string.IsNullOrEmpty(dtFinal) ? null : (DateTime?)Convert.ToDateTime(dtFinal);

                DateTime? dataIncialFim = string.IsNullOrEmpty(dtInicialFim) ? null : (DateTime?)Convert.ToDateTime(dtInicialFim);
                DateTime? dataFinalFim = string.IsNullOrEmpty(dtFinalFim) ? null : (DateTime?)Convert.ToDateTime(dtFinalFim);
                
                string parametros = "@tipoTurma=" + tipoTurma + "&@cdCurso=" + cdCurso + "&@cdDuracao=" + cdDuracao + "&@cdProduto=" + cdProduto
                    + "&@cdProfessor=" + cdProfessor + "&@prog=" + (int)ProgramacaoTurma.parseTipoConsultaProg(prog) + "&@cd_escola=" + cdEscola + "&@turmasFilhas=" + turmasFilhas
                    + "&@dtInicial=" + dtInicial + "&@dtFinal=" + dtFinal + "&@dtInicialFim=" + dtInicialFim + "&@dtFinalFim=" + dtFinalFim + "&@mostrarTelefone=" + mostrarTelefone
                    + "&@mostrarResp=" + mostrarResp + "&@situacaoTurma=" + situacaoTurma + "&@situacaoAlunoTurma=" + situacaoAlunoTurma + "&@tipoOnline=" + tipoOnline + "&@tipoRelatorio=" + tipoRelatorio + "&@dias=" + dias;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "rpttm")]
        public HttpResponseMessage getUrlReportTurmaMatriculaMaterial(int cd_aluno, int cd_turma, int cd_item, int nm_contrato, string dtInicial, string dtFinal)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                DateTime? dataIncial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dataFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                string parametros = "@cd_aluno=" + cd_aluno + "&@cd_turma=" + cd_turma + "&@cd_item=" + cd_item + "&@cd_escola=" + cdEscola
                    + "&@nm_contrato=" + nm_contrato + "&@dtInicial=" + dtInicial + "&@dtFinal=" + dtFinal;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getUrlReporProgramacaoAulasTurma(int cdTurma, string dtInicial, string dtFinal, bool umaTurmaPorPagina, bool mostrarFerias)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                DateTime? dataIncial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dataFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                string parametros = "@cdTurma=" + cdTurma + "&@cd_escola=" + cdEscola + "&@dtInicial=" + dtInicial + "&@dtFinal=" + dtFinal +
                                    "&@umaTurmaPorPagina=" + umaTurmaPorPagina + "&PidMostrarFeriado=" + mostrarFerias;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // ANTIGO CANCELAR ENCERRAMENTO - ANTES DA PROCEDURE sp_cancelar_rematricula 
        //[httppost]
        //[httpcomponentesauthorize(roles = "tur.a")]
        //public httpresponsemessage postcancelarencerramento(turma turma)
        //{
        //    returnresult retorno = new returnresult();
        //    try
        //    {
        //        int cdescola  = (int)this.componentesuser.codempresa;
        //        int cdusuario = (int)this.componentesuser.codusuario;
        //        iturmabusiness turmabiz = (iturmabusiness)base.instanciarbusiness<iturmabusiness>();
        //        var turmacancelamento = turmabiz.postcancelarencerramento(turma, cdescola, cdusuario);
        //        retorno.retorno = turmacancelamento;
        //        httpresponsemessage response = this.request.createresponse(httpstatuscode.ok, jsonconvert.serializeobject(retorno));
        //        return response;
        //    }
        //    catch (turmabusinessexception ex)
        //    {
        //        if (ex.tipoerro == turmabusinessexception.tipoerro.erro_aluno_turma_enc_ocupado_horario ||
        //            ex.tipoerro == turmabusinessexception.tipoerro.erro_existe_aluno_mat_turma_nova ||
        //            ex.tipoerro == turmabusinessexception.tipoerro.erro_aluno_turma_enc_ocupado_produto ||
        //            ex.tipoerro == turmabusinessexception.tipoerro.erro_horario_fora_intervalo_ocupado_turma ||
        //            ex.tipoerro == turmabusinessexception.tipoerro.erro_existe_contrato_com_contrato_anterior_igual_contrato_aluno_turma)
        //            return gerarlogexception(ex.message, retorno, logger, ex);
        //        return gerarlogexception(messages.msgregbuscerror, retorno, logger, ex);
        //    }
        //    catch (alunobusinessexception ex)
        //    {
        //        if (ex.tipoerro == alunobusinessexception.tipoerro.erro_aluno_turma_enc_ocupado_produto ||
        //            ex.tipoerro == alunobusinessexception.tipoerro.erro_horario_fora_intervalo_ocupado_turma)
        //            return gerarlogexception(ex.message, retorno, logger, ex);
        //        return gerarlogexception(messages.msgregbuscerror, retorno, logger, ex);
        //    }

        //    catch (exception ex)
        //    {
        //        return gerarlogexception(messages.msgregbuscerror, retorno, logger, ex);
        //    }
        //}

        // NOVO CANCELAR ENCERRAMENTO - ANTES DA PROCEDURE sp_cancelar_rematricula 
        [HttpPost]
        public HttpResponseMessage postCancelarEncerramento(TurmaCancelaEncerramentoUI turmaCancelaEncerramentoUI)
        {
            ReturnResult retorno = new ReturnResult();
            ITurmaBusiness Business = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
            try
            {

                if (turmaCancelaEncerramentoUI == null)
                {
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
                    configureHeaderResponse(response, null);
                    return response;
                }

                #region chamaProcedure postCancelarEncerramento
                int cd_usuario = (int)this.ComponentesUser.CodUsuario;
                int fuso = (int)this.ComponentesUser.IdFusoHorario;
                DateTime? dt_termino = turmaCancelaEncerramentoUI.dt_termino;
                string ret = Business.postCancelarEncerramento(turmaCancelaEncerramentoUI.cd_turma, cd_usuario, fuso);
                #endregion

                #region Grava em caso de erro JSonTeste
                if (ret != null)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ret));
                    return response;
                }
                #endregion

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                #region Retorna Json com erro
                
                var message = Utils.Utils.innerMessage(ex);
                if (message == "")
                    message = ex.Message + ", O cancelamento do encerramento foi efetuado, apesar do erro do Cyber";
                    //var msg = ex.InnerException.Message;

                FundacaoFisk.SGF.Utils.ExceptionHandler exceptionHandler = new FundacaoFisk.SGF.Utils.ExceptionHandler(message, "An erro has occurred.", ex.GetType().ToString(), ex.StackTrace);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(message));
                return response;

                #endregion

            }
        }

        [HttpPost]
        public HttpResponseMessage postRefazerProgramacao(Turma turma)
        {
            ReturnResult retorno = new ReturnResult();
            ITurmaBusiness Business = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
            try
            {

                #region chamaProcedure postRefazerProgramacao
                string ret = Business.postRefazerProgramacao(turma.cd_turma);
                #endregion

                #region Grava em caso de erro JSonTeste
                if (ret != null)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ret));
                    return response;
                }
                #endregion

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                #region Retorna Json com erro

                var message = Utils.Utils.innerMessage(ex);
                if (message == "")
                    message = ex.Message + ", A programacao foi refeita apesar do erro";

                FundacaoFisk.SGF.Utils.ExceptionHandler exceptionHandler = new FundacaoFisk.SGF.Utils.ExceptionHandler(message, "An erro has occurred.", ex.GetType().ToString(), ex.StackTrace);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(message));
                return response;

                #endregion

            }
        }

        [HttpPost]
        public HttpResponseMessage postRefazerNumeracao(Turma turma)
        {
            ReturnResult retorno = new ReturnResult();
            ITurmaBusiness Business = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
            try
            {

                #region chamaProcedure postRefazerNumeracao
                string ret = Business.postRefazerNumeracao(turma.cd_turma);
                #endregion

                #region Grava em caso de erro JSonTeste
                if (ret != null)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ret));
                    return response;
                }
                #endregion

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                #region Retorna Json com erro

                var message = Utils.Utils.innerMessage(ex);
                if (message == "")
                    message = ex.Message + ", A programacao foi refeita apesar do erro";

                FundacaoFisk.SGF.Utils.ExceptionHandler exceptionHandler = new FundacaoFisk.SGF.Utils.ExceptionHandler(message, "An erro has occurred.", ex.GetType().ToString(), ex.StackTrace);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(message));
                return response;

                #endregion

            }
        }

        [HttpGet]
        public HttpResponseMessage findSalas()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                List<Sala> salas = coordenacaoBiz.findSalasTurmas(cdEscola, false).ToList();
                List<Sala> salasOnline = coordenacaoBiz.findSalasTurmas(cdEscola, true).ToList();
                retorno.retorno = new { Salas = salas, SalasOnline = salasOnline };
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        #endregion

        #region Programação Turma

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage postIncluirProgramacao(ProgramacaoHorarioUI programacao)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                IEnumerable<Feriado> feriadosEscola = null;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                var horariosTurma = turmaBiz.criaProgramacaoTurma(programacao, this.ComponentesUser.CodEmpresa.Value, ref feriadosEscola);
                retorno.retorno = horariosTurma;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (ProgramacaoTurmaBusinessException ex)
            {
                return gerarLogException(Messages.msgProgramacaoSemHorario, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Componentes.Utils.Messages.Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur.a")]
        [HttpPost]
        public HttpResponseMessage postCancelarProgramacao([FromBody] List<int> cds_programacao_turma)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                bool cancelar = turmaBiz.cancelarProgramacaoTurma(cds_programacao_turma, this.ComponentesUser.CodEmpresa.Value);
                retorno.retorno = true;
                string msg = Messages.msgRegCanSucess;
                if (!cancelar) msg = Messages.msgRegDesCanSuccess;
                retorno.AddMensagem(msg, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (ProgramacaoTurmaBusinessException ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgRegNotCanSucess, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur.a")]
        [HttpPost]
        public HttpResponseMessage postDesfazerCancelarProgramacao([FromBody] List<int> cds_programacao_turma)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                bool cancelar = turmaBiz.desfazerCancelarProgramacaoTurma(cds_programacao_turma, this.ComponentesUser.CodEmpresa.Value);
                retorno.retorno = true;
                string msg = FundacaoFisk.SGF.Utils.Messages.Messages.msgRegCanSucess;
                if (!cancelar) msg = FundacaoFisk.SGF.Utils.Messages.Messages.msgRegDesCanSuccess;
                retorno.AddMensagem(msg, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (ProgramacaoTurmaBusinessException ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgRegNotCanSucess, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getProgramacoesTurma(int cd_turma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                ProgramacaoTurmaAbaUI progTurma = turmaBiz.getProgramacoesTurma(cd_turma, cdEscola);
                retorno.retorno = progTurma;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage existeAulaEfetivadaTurma(int cd_turma, bool turma_PPT)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                if (turma_PPT)
                    retorno.retorno = turmaBiz.existeAulaEfetivadaTurma(null, cd_turma, cdEscola);
                else
                    retorno.retorno = turmaBiz.existeAulaEfetivadaTurma(cd_turma, null, cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpPost]
#pragma warning disable CS0436 // Type conflicts with imported type
        public HttpResponseMessage postGerarRematricula(AlunoTurmaRematricuaUI alunoTurmaRematricuaUI)
#pragma warning restore CS0436 // Type conflicts with imported type
        {
            ReturnResult retorno = new ReturnResult();
            ITurmaBusiness Business = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
            try
            {
                int cd_usuario = (int)this.ComponentesUser.CodUsuario;
                int fusoHorario = this.ComponentesUser.IdFusoHorario;
                if (alunoTurmaRematricuaUI == null)
                {
                    retorno.AddMensagem(FundacaoFisk.SGF.Utils.Messages.Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                    configureHeaderResponse(response, null);
                    return response;
                }
                
                string ret = Business.postGerarRematricula(alunoTurmaRematricuaUI.dc_turma, cd_usuario, alunoTurmaRematricuaUI.dt_inicial, alunoTurmaRematricuaUI.dt_final, alunoTurmaRematricuaUI.id_turma_nova, alunoTurmaRematricuaUI.cd_layout, alunoTurmaRematricuaUI.dt_termino, fusoHorario);
                         
                if (ret != null)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ret));
                    return response;
                }           

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;

                FundacaoFisk.SGF.Utils.ExceptionHandler exceptionHandler = new FundacaoFisk.SGF.Utils.ExceptionHandler(msg, "An erro has occurred.", ex.GetType().ToString(), ex.StackTrace);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(msg));
                return response;

            }
        }

        #endregion

        #region Avaliação da turma

        [HttpComponentesAuthorize(Roles = "avlt")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "func")]
        [HttpComponentesAuthorize(Roles = "esc")]
        [HttpGet]
        public HttpResponseMessage getSearchAvaliacaoTurma(int idTurma, int idTipoAvaliacao, int cd_tipo_avaliacao, int cd_criterio_avaliacao, int cd_curso, int cd_funcionario, string dta_inicial, string dta_final, int cd_escola_combo)
        {
            DateTime? dataInicial;
            DateTime? dataFinal;
            try
            {
                if(dta_inicial != null && !"10/10/1980".Equals(dta_inicial))
                    dataInicial = Convert.ToDateTime(dta_inicial);
                else dataInicial = null;

                if(dta_final != null && !"10/10/2045".Equals(dta_final))
                    dataFinal = Convert.ToDateTime(dta_final);
                else dataFinal = null;

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cdPessoaUsuario = (int)this.ComponentesUser.CodPessoaUsuario;
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                var retorno = turmaBiz.searchAvaliacaoTurma(parametros, idTurma, getStatus(idTipoAvaliacao), cdEscola, cdPessoaUsuario, cd_tipo_avaliacao, cd_criterio_avaliacao, cd_curso, cd_funcionario, dataInicial, dataFinal, isMaster, cd_escola_combo);
                HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "avlt")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "func")]
        [HttpComponentesAuthorize(Roles = "esc")]
        [HttpPost]
        public HttpResponseMessage returnDataAvaliacaoTurma()
        {
            ReturnResult retorno = new ReturnResult();
            AvaliacaoTurmaUI avaliacaoTurmaUI = new AvaliacaoTurmaUI();
            try
            {
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                avaliacaoTurmaUI.turmas = turmaBiz.findTurma(0, cdEscola).ToList();
                avaliacaoTurmaUI.tiposAvaliacoes = coordenacaoBiz.getTipoAvaliacaoAvaliacaoTurma().ToList();
                avaliacaoTurmaUI.cursos = cursoBiz.getCursoAvaliacaoTurma(cdEscola).ToList();
                avaliacaoTurmaUI.funcionarios = profBiz.getProfessorAvaliacaoTurma(cdEscola).ToList();
                retorno.retorno = avaliacaoTurmaUI;
                retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "avlt")]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "func")]
        [HttpGet]
        public HttpResponseMessage getAvaliacaoTurmaArvore(int idTurma, int idConceito, int tipoForm, int cd_escola_combo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                List<AvaliacaoTurma> avaliacaoesTurmaList = new List<AvaliacaoTurma>();
                List<AvaliacaoAlunos> listaAvaliacoes = new List<AvaliacaoAlunos>();
                List<HistoricoAluno> historicos = new List<HistoricoAluno>();
                AvaliacaoAuxiliarUI avaliacaoUI = new AvaliacaoAuxiliarUI();
                ISecretariaBusiness secretariaBusiness = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                historicos = secretariaBusiness.returnHistoricoSitacaoAlunoTurma(idTurma, (int)this.ComponentesUser.CodEmpresa).OrderBy(x => x.cd_aluno).ThenByDescending(h => h.nm_sequencia).ToList();
                tipoForm = tipoForm == (int)TurmaController.TipoFormException.AVALIACAO_TURMA ? (int)TurmaController.TipoFormException.AVALIACAO_TURMA : (int)TurmaController.TipoFormException.TURMA;
                int cdUsuario = (int)this.ComponentesUser.CodPessoaUsuario;
                bool isConceito = idConceito == 1 ? true : false;
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;

                List<TipoAvaliacaoTurma> listaTipos = coordenacaoBiz.tiposAvaliacao(idTurma, cd_escola_combo, isConceito);
                List<Conceito> listConceitosDisponiveis = coordenacaoBiz.getConceitosDisponiveisByProdutoTurma(idTurma);
                if (listaTipos != null)
                {
                    if (tipoForm == (int)TurmaController.TipoFormException.AVALIACAO_TURMA)
                        turmaBiz.incluirAvaliacoesTurma(idTurma, null);
                    for (int i = 0; i < listaTipos.Count; i++)
                    {
                        avaliacaoesTurmaList = turmaBiz.getAvaliacaoTurmaArvore(idTurma, cd_escola, getStatus(idConceito), cdUsuario, tipoForm, listaTipos[i].cd_tipo_avaliacao);
                        listaTipos[i].cd_tipo_avaliacao = listaTipos[i].cd_tipo_avaliacao;
                        listaTipos[i].dc_nome = listaTipos[i].dc_tipo_avaliacao;
                        listaTipos[i].id = listaTipos[i].cd_tipo_avaliacao + idTurma;
                        listaTipos[i].pai = (int)AvaliacaoAlunos.Hierarquia.PAI;
                        listaTipos[i].idPai = listaTipos[i].cd_tipo_avaliacao;
                        listaTipos[i].ativo = false;
                        listaAvaliacoes = new List<AvaliacaoAlunos>();
                        montaNotasAvaliacao(avaliacaoesTurmaList, listaAvaliacoes, cd_escola, cd_escola_combo);
                        listaTipos[i].children = listaAvaliacoes.ToList();

                        // Transforma o objeto de banco em objeto da view:                       
                        setIsConceitoNota(avaliacaoesTurmaList, listaAvaliacoes);
                    }
                }
                avaliacaoUI.funcionarioAvaliador = profBiz.getProfessoresByEmpresa(cd_escola, idTurma).ToList();
                avaliacaoUI.historicoAluno = historicos;
                avaliacaoUI.tipoAvaliacaoTurma = listaTipos;
                avaliacaoUI.conceitosDisponiveis = listConceitosDisponiveis;
                retorno.retorno = avaliacaoUI;
                               
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));              
            }
            catch (TurmaBusinessException ex)
            {
                if (ex.tipoErro.Equals(TurmaBusinessException.TipoErro.ERRO_NAO_EXISTE_AVALIACAO_TURMA)
                    || ex.tipoErro.Equals(TurmaBusinessException.TipoErro.ERRO_NAO_EXISTE_ALUNO_TURMA))
                {
                    retorno.AddMensagem(ex.Message, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                }
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        private static void setIsConceitoNota(List<AvaliacaoTurma> avaliacaoesTurmaList, List<AvaliacaoAlunos> listaAvaliacoes)
        {
            if (avaliacaoesTurmaList.Count() > 0 && listaAvaliacoes.Count() > 0)
                listaAvaliacoes[0].isConceitoNota = avaliacaoesTurmaList.Any(a => (a.Avaliacao.CriterioAvaliacao.id_conceito.Equals(true)))
                   && (avaliacaoesTurmaList.Where(a => a.Avaliacao.CriterioAvaliacao.id_conceito == false).Any());
        }

        private void montaNotasAvaliacao(ICollection<AvaliacaoTurma> avaliacao, ICollection<AvaliacaoAlunos> retorno, int cd_escola, int cd_escola_combo)
        {
            int cd_professor = 0;

            AvaliacaoAlunos avaliacoesAlunos = new AvaliacaoAlunos();
            List<AvaliacaoTurma> avaliacaoList = avaliacao.ToList();
            for (int i = 0; i < avaliacaoList.Count; i++)
            {
                if(avaliacaoList[i].Turma.TurmaProfessorTurma != null)
                for (int j = 0; j < avaliacaoList[i].Turma.TurmaProfessorTurma.Count;)
			    {   
			      cd_professor =  avaliacaoList[i].Turma.TurmaProfessorTurma.ToList()[j].cd_professor;
                  cd_escola_combo = avaliacaoList[i].Turma.cd_pessoa_escola;
                  break;
			    }
                //List<AvaliacaoAluno> listaAvaliacaoAluno = avaliacaoList[i].AvaliacaoAluno.Where(a => cd_escola != cd_escola_combo ? a.Aluno.cd_pessoa_escola == cd_escola : true).ToList();
                List<AvaliacaoAluno> listaAvaliacaoAluno = avaliacaoList[i].AvaliacaoAluno.ToList();

                avaliacoesAlunos = new AvaliacaoAlunos();
                avaliacoesAlunos.id = avaliacaoList[i].cd_avaliacao_turma;
                avaliacoesAlunos.cd_avaliacao_turma = avaliacaoList[i].cd_avaliacao_turma;
                avaliacoesAlunos.pai = (int)AvaliacaoAlunos.Hierarquia.PAI;
                avaliacoesAlunos.isChildren = 1;
                avaliacoesAlunos.idPai = avaliacaoList[i].cd_avaliacao_turma;
                avaliacoesAlunos.dc_nome = avaliacaoList[i].Avaliacao.CriterioAvaliacao.dc_criterio_avaliacao;
                avaliacoesAlunos.vl_nota = listaAvaliacaoAluno.Select(a => a.nm_nota_aluno).FirstOrDefault();
                avaliacoesAlunos.vl_nota_2= listaAvaliacaoAluno.Select(a => a.nm_nota_aluno_2).FirstOrDefault();
                avaliacoesAlunos.peso = avaliacaoList[i].Avaliacao.nm_peso_avaliacao;
                if (avaliacoesAlunos.peso.HasValue)
                {
                    avaliacoesAlunos.vl_nota *= (double)avaliacoesAlunos.peso.Value;
                    avaliacoesAlunos.vl_nota_2 *= (double)avaliacoesAlunos.peso.Value;
                }
                avaliacoesAlunos.dc_observacao = avaliacaoList[i].tx_obs_aval_turma;
                avaliacoesAlunos.dc_nome_avaliador = avaliacaoList[i].Funcionario == null ? "" : avaliacaoList[i].Funcionario.FuncionarioPessoaFisica.no_pessoa;
                avaliacoesAlunos.dc_avaliacao_turma = avaliacaoList[i].Avaliacao.CriterioAvaliacao.dc_criterio_abreviado;
                avaliacoesAlunos.maximoNotaTurma = avaliacaoList[i].Avaliacao.vl_nota * avaliacaoList[i].Avaliacao.nm_peso_avaliacao;
                avaliacoesAlunos.cd_funcionario = avaliacaoList[i].Funcionario == null ? 
                                                  (cd_professor >  0 ? cd_professor : 0) : avaliacaoList[i].Funcionario.cd_funcionario;
                avaliacoesAlunos.dt_avaliacao_turma = avaliacaoList[i].dt_avaliacao_turma;
                avaliacoesAlunos.cd_avaliacao = avaliacaoList[i].cd_avaliacao;
                avaliacoesAlunos.ativo = avaliacaoList[i].Avaliacao.id_avaliacao_ativa;
                avaliacoesAlunos.isConceito = avaliacaoList[i].Avaliacao.CriterioAvaliacao.id_conceito;
                avaliacoesAlunos.cd_produto = avaliacaoList[i].Turma.cd_produto;
                avaliacoesAlunos.id_segunda_prova = listaAvaliacaoAluno.Select(a => a.id_segunda_prova).FirstOrDefault();
                avaliacoesAlunos.dc_observacao_aux = avaliacaoList[i].tx_obs_aval_turma;
                avaliacoesAlunos.dt_cadastro = null;
                avaliacoesAlunos.dt_desistencia = null;
                avaliacoesAlunos.dt_matricula = null;
                avaliacoesAlunos.dt_movimento = null;
                avaliacoesAlunos.dt_transferencia = null;
                avaliacoesAlunos.dt_termino_turma = avaliacaoList[i].Turma.dt_termino_turma;
                avaliacoesAlunos.isModifiedA = false;
                avaliacoesAlunos.isModified = avaliacaoList[i].isModified;
                avaliacoesAlunos.mediaNotas = listaAvaliacaoAluno.Sum(av => (av.nm_nota_aluno_2 != null ? av.nm_nota_aluno_2 : av.nm_nota_aluno) * (double)avaliacaoList[i].Avaliacao.nm_peso_avaliacao) / listaAvaliacaoAluno.Where(aa => aa.nm_nota_aluno != null).Count();

                avaliacoesAlunos.participacoesDisponiveis = this.extraiParticipacoesDisponiveis(avaliacaoList[i]);
                if (avaliacoesAlunos.mediaNotas.HasValue && avaliacoesAlunos.mediaNotas.Value.Equals(double.NaN))
                    avaliacoesAlunos.mediaNotas = null;
                avaliacoesAlunos.somaNotas = listaAvaliacaoAluno.Sum(av => (av.nm_nota_aluno_2 != null ? av.nm_nota_aluno_2 : av.nm_nota_aluno)) * (double)avaliacaoList[i].Avaliacao.nm_peso_avaliacao;
                if(listaAvaliacaoAluno.Count() > 0)
                    montaAlunoNotasAluno(listaAvaliacaoAluno, avaliacoesAlunos.children, avaliacaoList[i], avaliacoesAlunos.cd_produto);
                avaliacoesAlunos.children = avaliacoesAlunos.children.OrderBy(l => l.dc_nome).ToList();
                retorno.Add(avaliacoesAlunos);
            }
        }

        private void montaAlunoNotasAluno(List<AvaliacaoAluno> listaAvaliacaoAluno, ICollection<AvaliacaoAlunos> retorno, AvaliacaoTurma avaliacaoTurma, int cd_produto)
        {
            AvaliacaoAlunos avaliacoesAlunos = new AvaliacaoAlunos();
            for(int u = 0; u < listaAvaliacaoAluno.Count; u++)
            {
                if(listaAvaliacaoAluno[u].Aluno != null && listaAvaliacaoAluno[u].Aluno.AlunoPessoaFisica != null)
                    avaliacoesAlunos = new AvaliacaoAlunos();
                avaliacoesAlunos.id = listaAvaliacaoAluno[u].cd_avaliacao_aluno + ' ' + listaAvaliacaoAluno[u].Aluno.cd_aluno + ' ' + avaliacaoTurma.Avaliacao.cd_tipo_avaliacao;
                avaliacoesAlunos.pai = (int)AvaliacaoAlunos.Hierarquia.FILHO;
                avaliacoesAlunos.isChildren = 1;
                avaliacoesAlunos.idPai = listaAvaliacaoAluno[u].cd_avaliacao_turma;
                avaliacoesAlunos.dc_nome = listaAvaliacaoAluno[u].Aluno.AlunoPessoaFisica.no_pessoa;
                avaliacoesAlunos.vl_nota = listaAvaliacaoAluno[u].nm_nota_aluno;
                avaliacoesAlunos.vl_nota_2 = listaAvaliacaoAluno[u].nm_nota_aluno_2;
                avaliacoesAlunos.peso = avaliacaoTurma.Avaliacao.nm_peso_avaliacao;
                avaliacoesAlunos.dc_observacao = listaAvaliacaoAluno[u].tx_obs_nota_aluno;
                avaliacoesAlunos.dc_avaliacao_turma = listaAvaliacaoAluno[u].Aluno.AlunoPessoaFisica.no_pessoa;
                avaliacoesAlunos.notaMaxima = listaAvaliacaoAluno.Select(nt => (nt.nm_nota_aluno_2 != null ? nt.nm_nota_aluno_2 : nt.nm_nota_aluno) * (double)avaliacoesAlunos.peso).Max();
                avaliacoesAlunos.mediaNotas = listaAvaliacaoAluno.Sum(nt => (nt.nm_nota_aluno_2 != null ? nt.nm_nota_aluno_2 : nt.nm_nota_aluno) * (double)avaliacoesAlunos.peso) / listaAvaliacaoAluno.Where(aa => aa.nm_nota_aluno != null).Count();
                if (avaliacoesAlunos.mediaNotas.HasValue && avaliacoesAlunos.mediaNotas.Value.Equals(double.NaN))
                    avaliacoesAlunos.mediaNotas = null;
                avaliacoesAlunos.somaNotas = listaAvaliacaoAluno.Sum(a => (a.nm_nota_aluno_2 != null ? a.nm_nota_aluno_2 : a.nm_nota_aluno) * (double)avaliacoesAlunos.peso);
                avaliacoesAlunos.cd_aluno = listaAvaliacaoAluno[u].Aluno.cd_aluno;
                avaliacoesAlunos.cd_avaliacao_turma = listaAvaliacaoAluno[u].cd_avaliacao_turma;
                avaliacoesAlunos.cd_avaliacao_aluno = listaAvaliacaoAluno[u].cd_avaliacao_aluno;
                avaliacoesAlunos.cd_conceito = listaAvaliacaoAluno[u].cd_conceito;
                avaliacoesAlunos.cd_produto = cd_produto;
                avaliacoesAlunos.id_segunda_prova = listaAvaliacaoAluno[u].id_segunda_prova;
                avaliacoesAlunos.dc_observacao_aux = listaAvaliacaoAluno[u].tx_obs_nota_aluno;
                //TODO a data de casdastro foi removida do banco e consequentemente do edmx rever esse linha.
                avaliacoesAlunos.dt_cadastro = listaAvaliacaoAluno[u].Aluno.HistoricoAluno.Where(av => av.cd_turma == avaliacaoTurma.cd_turma).Select(av => av.dt_cadastro).FirstOrDefault();
                avaliacoesAlunos.dt_desistencia = listaAvaliacaoAluno[u].Aluno.AlunoTurma.Select(av => av.dt_desistencia).FirstOrDefault();
                avaliacoesAlunos.dt_matricula = listaAvaliacaoAluno[u].Aluno.AlunoTurma.Select(av => av.dt_matricula).FirstOrDefault();
                avaliacoesAlunos.dt_movimento = listaAvaliacaoAluno[u].Aluno.AlunoTurma.Select(av => av.dt_movimento).FirstOrDefault();
                avaliacoesAlunos.dt_transferencia = listaAvaliacaoAluno[u].Aluno.AlunoTurma.Select(av => av.dt_transferencia).FirstOrDefault();
                avaliacoesAlunos.dt_termino_turma = null;
                avaliacoesAlunos.dc_conceito = listaAvaliacaoAluno[u].Conceito == null ? "" : listaAvaliacaoAluno[u].Conceito.no_conceito;
                avaliacoesAlunos.ativo = false;
                avaliacoesAlunos.id_participacao = avaliacaoTurma.Avaliacao.CriterioAvaliacao.id_participacao;

                if (listaAvaliacaoAluno[u].AvaliacoesAlunoParticipacao != null)
                    foreach (AvaliacaoAlunoParticipacao avaAlunoPart in listaAvaliacaoAluno[u].AvaliacoesAlunoParticipacao)
                    {
                        avaAlunoPart.AvaliacaoAluno = null;
                        avaAlunoPart.ParticipacaoAvaliacao = null;
                    }

                avaliacoesAlunos.participacoesAluno = listaAvaliacaoAluno[u].AvaliacoesAlunoParticipacao;
                retorno.Add(avaliacoesAlunos);
            }
        }

        private ICollection<Participacao> extraiParticipacoesDisponiveis(AvaliacaoTurma avaliacaoTurma)
        {
            List<Participacao> retorno = new List<Participacao>();

            if (avaliacaoTurma.Avaliacao != null && avaliacaoTurma.Avaliacao.CriterioAvaliacao != null)
                if (avaliacaoTurma.Avaliacao.CriterioAvaliacao.AvaliacaoParticipacao != null)
                    foreach (AvaliacaoParticipacao avaPart in avaliacaoTurma.Avaliacao.CriterioAvaliacao.AvaliacaoParticipacao)
                        if (avaPart.AvaliacaoParticipacaoVinc != null)
                            foreach (AvaliacaoParticipacaoVinc avaPartVinc in avaPart.AvaliacaoParticipacaoVinc)
                                if (avaPartVinc.ParticipacaoAvaliacao != null 
                                        && ((avaPartVinc.id_avaliacao_participacao_ativa && avaPartVinc.ParticipacaoAvaliacao.id_participacao_ativa) 
                                            || avaPartVinc.ParticipacaoAvaliacao.AvaliacoesAlunoParticipacao.Count > 0))
                                    retorno.Add(new Participacao
                                    {
                                        cd_participacao = avaPartVinc.ParticipacaoAvaliacao.cd_participacao,
                                        no_participacao = avaPartVinc.ParticipacaoAvaliacao.no_participacao,
                                        id_avaliacao_participacao_vinc_ativa = avaPartVinc.id_avaliacao_participacao_ativa
                                    });
            return retorno;
        }

        //Persistência
        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "avlt.a")]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpPost]
        public HttpResponseMessage postAlterarAvaliacaoTurma(AvaliacaoTurmaUI avaliacaoTurmaUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cdUsuario = (int)this.ComponentesUser.CodPessoaUsuario;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                ISecretariaBusiness secretariaBusiness = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();

                configuraBusiness(new List<IGenericBusiness>() { turmaBiz, coordenacaoBiz, cursoBiz, secretariaBusiness, profBiz, alunoBiz });
                var avaliacaoTurma = turmaBiz.editAvaliacaoTurma(avaliacaoTurmaUI, cdUsuario, cdEscola);
                retorno.retorno = avaliacaoTurma;
                if (String.IsNullOrEmpty(avaliacaoTurma.no_turma))
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "avlt")]
        public HttpResponseMessage getUrlRelatorioAvaliacaoTurma(string sort, int direction, int idTurma, int idTipoAvaliacao, int cd_tipo_avaliacao, int cd_criterio_avaliacao, int cd_curso, int cd_funcionario, string dta_inicial, string dta_final, int cd_escola_combo)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cdUsuario = (int)this.ComponentesUser.CodPessoaUsuario;

                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                bool isMaster = ComponentesUser.IdMaster;
                string parametros = strParametrosSort + "@idTurma=" + idTurma + "&@idTipoAvaliacao=" + idTipoAvaliacao + "&@cdEscola=" + cdEscola + "&@cd_tipo_avaliacao=" 
                    + cd_tipo_avaliacao + "&@cd_criterio_avaliacao=" + cd_criterio_avaliacao + "&@cd_curso=" + cd_curso + "&@cdUsuario=" + cdUsuario + "&@cd_funcionario="
                    + cd_funcionario + "&@dataInicial=" + dta_inicial + "&@dataFinal=" + dta_final + "&@cd_escola_combo=" + cd_escola_combo + "&@isMaster=" + isMaster + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório Avaliação Turma&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.AvaliacaoTurmaSearch;

                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "avlt")]
        [HttpComponentesAuthorize(Roles = "conc")]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "esc")]
        [HttpGet]
        public HttpResponseMessage returnAvaliacoesNotaOrConceito(int idTurma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cdUsuario = (int)this.ComponentesUser.CodPessoaUsuario;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();

                List<AvaliacaoTurmaUI> avaliacaoesTurmaList = turmaBiz.returnAvaliacoesConceitoOrNotaByTurma(idTurma, cdUsuario, cdEscola);
                retorno.retorno = avaliacaoesTurmaList;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (TurmaBusinessException ex)
            {
                if (ex.tipoErro.Equals(TurmaBusinessException.TipoErro.ERRO_NAO_EXISTE_AVALIACAO_TURMA))
                {
                    retorno.AddMensagem(ex.Message, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                }
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "avlt.e")]
        public HttpResponseMessage postDeleteAvaliacaoTurma(List<AvaliacaoTurmaUI> avaliacaoTurma)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                var cd_escola = (int)this.ComponentesUser.CodEmpresa;
                int cdUsuario = (int)this.ComponentesUser.CodPessoaUsuario;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { turmaBiz});
                bool delAvaliacao = false;
                if(avaliacaoTurma != null){
                        delAvaliacao = turmaBiz.deleteAvaliacaoTurma(avaliacaoTurma, cd_escola, cdUsuario);
                    retorno.retorno = delAvaliacao;
                    if(!delAvaliacao)
                        retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    else
                        retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (TurmaBusinessException ex)
            {
                if (ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_DELETAR_NOTA_FUNCIONARIO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage componentesPesquisaAvaliacoes()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ComponentesTurma componentesTurma = new ComponentesTurma();
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();

                componentesTurma.produtos = coordenacaoBiz.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_TURMA, null, cdEscola).ToList();
                componentesTurma.cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_TURMA, null, null, cdEscola).ToList();
                componentesTurma.professores = profBiz.getProfessorReturnProfUI(ProfessorDataAccess.TipoConsultaProfessorEnum.HAS_AVALIACAO, cdEscola, null).ToList();

                retorno.retorno = componentesTurma;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Alunos Turma
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpGet]
        public HttpResponseMessage getAlunosTurma(int cd_turma)
        {
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.findAlunosTurma(cd_turma, cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "reg")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getTurmaAlunoSearch(string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, int prog, bool turmasFilhas,
            int cdAluno, bool retorno, int cd_escola_combo_fk)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;
                int tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                ProfessorUI prof = profBiz.verificaRetornaSeUsuarioLogadoEProfessor(cdPessoaUsuario, cdEscola);
                if (prof != null && prof.cd_pessoa > 0 && !this.ComponentesUser.IdMaster && !prof.id_coordenador)
                    tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_FK_TURMA_PROF;
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retornoPesq = turmaBiz.searchTurmaAluno(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, ProgramacaoTurma.parseTipoConsultaProg(prog), cdEscola, turmasFilhas, cdAluno, tipoPesq, retorno, cd_escola_combo_fk);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retornoPesq);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "reg")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getTurmaAlunoDesistente(string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
                                                            int cdProfessor, int prog, bool turmasFilhas, string dtInicial, string dtFinal)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;
                int tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                ProfessorUI prof = profBiz.verificaRetornaSeUsuarioLogadoEProfessor(cdPessoaUsuario, cdEscola);
                if (prof != null && prof.cd_pessoa > 0 && !this.ComponentesUser.IdMaster && !prof.id_coordenador)
                    tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_FK_TURMA_PROF;
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = turmaBiz.searchTurmaAlunoDesistente(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, ProgramacaoTurma.parseTipoConsultaProg(prog), cdEscola, turmasFilhas, dtaInicial, dtaFinal, tipoPesq);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        #endregion

        #region Tabela de Preço

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "reg")]
        public HttpResponseMessage componentesNovaTabelaPreco(int? cdDuracao, int? cdProduto, int? cdRegime, int? cdCurso)
        {
            ReturnResult retorno = new ReturnResult();
            ComponentesTurma componentesTurma = new ComponentesTurma();
            //int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                componentesTurma.duracoes = coordenacaoBiz.getDuracoes(DuracaoDataAccess.TipoConsultaDuracaoEnum.HAS_ATIVO, cdDuracao, null).ToList();
                componentesTurma.regimes = coordenacaoBiz.getRegimes(RegimeDataAccess.TipoConsultaRegimeEnum.HAS_ATIVO, cdRegime).ToList();
                componentesTurma.cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_ATIVO, cdCurso, null, null).ToList();
                retorno.retorno = componentesTurma;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
    }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Controle de Sala

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "rpcs")]
        public HttpResponseMessage componentesPesquisaControleSala()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                List<ProfessorUI> porfs = profBiz.getProfessorReturnProfUI(ProfessorDataAccess.TipoConsultaProfessorEnum.HAS_ATIVO, cdEscola, null).ToList();
                if (porfs != null && porfs.Count() > 0)
                    foreach (var p in porfs)
                        p.no_fantasia = string.IsNullOrEmpty(p.no_fantasia) ? p.no_pessoa : p.no_fantasia;
                List<Sala> salas = coordenacaoBiz.findListSalasTurmas(cdEscola).ToList();
                retorno.retorno = new { Professores = porfs, Salas = salas };
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "rpcqs")]
        public HttpResponseMessage getUrlRelatorioControleSala(int cd_pessoa_professor, int cd_sala, int cd_turma, string hIni, string hFim, string dias, string no_turma, string no_sala)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                TimeSpan? horaInicial;
                TimeSpan? horaFinal;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                if (!string.IsNullOrEmpty(hIni)) horaInicial = TimeSpan.Parse(hIni);
                else horaInicial = null;
                if (!string.IsNullOrEmpty(hFim)) horaFinal = TimeSpan.Parse(hFim);
                else horaFinal = null;
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cd_empresa=" + cdEscola + "&@cd_pessoa_professor=" + cd_pessoa_professor + "&@cd_sala=" + cd_sala + "&@cd_turma=" + cd_turma +
                                    "&@hIni=" + hIni + "&@hFim=" + hFim + "&@dias=" + dias + "&@no_turma=" + no_turma + "&@no_sala=" + no_sala;
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }



        #endregion

        #region ControleFaltas

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "confal")]
        public HttpResponseMessage getTurmasComPercentualFaltaSearch (string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, int prog, bool turmasFilhas, int cdAluno, string dtInicial, string dtFinal, int? cd_turma_PPT, bool semContrato, string dataInicial, string dataFinal, bool id_percentual_faltas)
        {
            try
            {

                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                DateTime? dt_inicial = string.IsNullOrEmpty(dataInicial) ? null : (DateTime?)Convert.ToDateTime(dataInicial);
                DateTime? dt_final = string.IsNullOrEmpty(dataFinal) ? null : (DateTime?)Convert.ToDateTime(dataFinal);

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                int tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA;
                ProfessorUI prof = profBiz.verificaRetornaSeUsuarioLogadoEProfessor(cdPessoaUsuario, cdEscola);
                if (prof != null && prof.cd_pessoa > 0 && !this.ComponentesUser.IdMaster && !prof.id_coordenador)
                    tipoPesq = (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_FK_TURMA_PROF;
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = turmaBiz.getTurmasComPercentualFaltaSearch(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, ProgramacaoTurma.parseTipoConsultaProg(prog), cdEscola, turmasFilhas, cdAluno, dtaInicial, dtaFinal, cd_turma_PPT, semContrato, tipoPesq, dt_inicial, dt_final, false, id_percentual_faltas);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        #endregion
    }
}