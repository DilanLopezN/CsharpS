using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Componentes.GenericController;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Auth.Comum;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using log4net;
using Newtonsoft.Json;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;
using Componentes.GenericBusiness.Comum;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.Data.Entity.Core;
using FundacaoFisk.SGF.Services.Coordenacao.Model;


namespace FundacaoFisk.SGF.Web.Controllers
{
    public class CoordenacaoController : ComponentesMVCController
    {
        //Propriedades
        private static readonly ILog logger = LogManager.GetLogger(typeof(CoordenacaoController));

        //Método construtor
        public CoordenacaoController()
        {
        }

        #region ActionResult

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CriteriosAvaliacoes()
        {
            return View();
        }

        public ActionResult TiposAvaliacoes()
        {
            return View();
        }

        public ActionResult Avaliacao()
        {
            return View();
        }

        public ActionResult ProgramacaoCurso()
        {
            return View();
        }

        public ActionResult AtividadeExtra()
        {
            return View();
        }

        public ActionResult Turma()
        {
            return View();
        }

        public ActionResult DiarioAula()
        {
            return View();
        }

        public ActionResult AvaliacaoTurma()
        {
            return View();
        }

        public ActionResult AulaPersonalizada()
        {
            return View();
        }

        public ActionResult Desistencia()
        {
            return View();
        }

        public ActionResult MudancaInterna()
        {
            return View();
        }

        public ActionResult ModeloProgramacaoTurma()
        {
            return View();
        }

        public ActionResult AvaliacaoParticipacao()
        {
             return View();
        }

        public ActionResult CalendarioEvento()
        {
            return View();
        }

        public ActionResult ControleFalta()
        {
            return View();
        }

        public ActionResult AulasReposicao()
        {
            return View();
        }

        public ActionResult CalendarioAcademico()
        {
            return View();
        }
        public ActionResult AlunosemAula()
        {
            return View();
        }
        public ActionResult ProfessorsemDiario()
        {
            return View();
        }
        public ActionResult AlunosCargaHoraria()
        {
            return View();
        }
        public ActionResult ConsultarCargasHorarias()
        {
            return View();
        }
        public ActionResult ConsultarRafsemDiario()
        {
            return View();
        }
        public ActionResult PerdaMaterial()
        {
            return View();
        }

        #endregion

        //Retorna o código da escola
        private int recoverEscola()
        {
            var codEscola = (int)Session["CodEscolaSelecionada"];
            return codEscola;
        }

        //Retorna um boleano, se o usuario é master
        private bool retornaUserMaster()
        {
            IEmpresaBusiness empresaBiz = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
            string login = Session["loginUsuario"].ToString();
            var isMaster = empresaBiz.VerificarMasterGeral(login);
            return isMaster;
        }


        #region Atividade Extra

        //Retorna listas de objetos para percistência
        [MvcComponentesAuthorize(Roles = "atvex")]
        [MvcComponentesAuthorize(Roles = "tavex")]
        [MvcComponentesAuthorize(Roles = "cur")]
        [MvcComponentesAuthorize(Roles = "func")]
        [MvcComponentesAuthorize(Roles = "alu")]
        [MvcComponentesAuthorize(Roles = "prod")]
        [MvcComponentesAuthorize(Roles = "sala")]
        public RenderJsonActionResult obterRecursosAtividadeExtra(AtividadeExtraPesquisa atividadeExtraPesq)
        {
            ReturnResult retorno = new ReturnResult();
            AtividadeExtraUI atividadeExtraUI = new AtividadeExtraUI();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IProfessorBusiness professorBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                IAuthBusiness authBiz = (IAuthBusiness)base.instanciarBusiness<IAuthBusiness>();

                int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
                var usuario = authBiz.GetNomeCodigoUsuario(User.Identity.Name);
                var ativo = FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess.ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_ATIVO;
                var usuarioAntendente = coordenacaoBiz.returnAtividadeExtraUsuarioAtendente(atividadeExtraPesq.cd_atividade_extra, cdEscola);

                atividadeExtraUI.produtos = coordenacaoBiz.findProduto(ativo, atividadeExtraPesq.cd_produto, null).ToList();
                atividadeExtraUI.professores = professorBiz.getFuncionariosByEscolaAtividade(cdEscola, atividadeExtraPesq.cd_funcionario, atividadeExtraPesq.cd_atividade_extra, true).ToList();

                if (usuarioAntendente != null)
                {
                    atividadeExtraUI.no_usuario = usuarioAntendente.no_usuario;
                    atividadeExtraUI.cd_usuario_atendente = usuarioAntendente.cd_usuario_atendente;
                }
                else
                {
                    atividadeExtraUI.no_usuario = usuario.no_login;
                    atividadeExtraUI.cd_usuario_atendente = usuario.cd_usuario;
                }

                retorno.retorno = atividadeExtraUI;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Retorna listas de objetos para percistência
        [MvcComponentesAuthorize(Roles = "atvex")]
        [MvcComponentesAuthorize(Roles = "tavex")]
        [MvcComponentesAuthorize(Roles = "cur")]
        [MvcComponentesAuthorize(Roles = "func")]
        [MvcComponentesAuthorize(Roles = "alu")]
        [MvcComponentesAuthorize(Roles = "prod")]
        [MvcComponentesAuthorize(Roles = "sala")]
        public RenderJsonActionResult getAtividadeExtraViewOnDbClik(AtividadeExtraPesquisa atividadeExtraPesq)
        {
            ReturnResult retorno = new ReturnResult();
            AtividadeExtraUI atividadeExtraUI = new AtividadeExtraUI();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                IProfessorBusiness professorBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                IAuthBusiness authBiz = (IAuthBusiness)base.instanciarBusiness<IAuthBusiness>();

                var data = atividadeExtraPesq.data == "" ? "01/01/1989" : atividadeExtraPesq.data;
                var dataAtividade = Convert.ToDateTime(data);
                TimeSpan horaInicial;
                TimeSpan horaFinal;

                if (atividadeExtraPesq.hrInicial != "" && atividadeExtraPesq.hrInicial != null) horaInicial = TimeSpan.Parse(atividadeExtraPesq.hrInicial);
                else horaInicial = new TimeSpan(0);
                if (atividadeExtraPesq.hrFinal != "" && atividadeExtraPesq.hrFinal != null) horaFinal = TimeSpan.Parse(atividadeExtraPesq.hrFinal);
                else horaFinal = new TimeSpan(0);
                int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
                var usuario = authBiz.GetNomeCodigoUsuario(User.Identity.Name);
                var usuarioAntendente = coordenacaoBiz.returnAtividadeExtraUsuarioAtendente(atividadeExtraPesq.cd_atividade_extra, cdEscola);
                atividadeExtraUI.tiposAtividadeExtras = coordenacaoBiz.getTipoAtividade(true, atividadeExtraPesq.cd_tipo_ativiade_extra,
                                                                                         null, TipoAtividadeExtraDataAccess.TipoConsultaAtivExtraEnum.HAS_ATIVO).ToList();
                //atividadeExtraUI.professores = professorBiz.getFuncionariosByEscola(cdEscola, atividadeExtraPesq.cd_funcionario, true).ToList();
                //atividadeExtraUI.produtos = coordenacaoBiz.findProduto(ativo, atividadeExtraPesq.cd_produto,null).ToList();
                atividadeExtraUI.cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_PRODUTO, atividadeExtraPesq.cd_curso, atividadeExtraPesq.cd_produto, cdEscola).ToList();

                atividadeExtraUI.salasDisponiveis = coordenacaoBiz.findListSalasDiponiveis(horaInicial, horaFinal, dataAtividade, true, atividadeExtraPesq.cd_sala, atividadeExtraPesq.cd_pessoa_escola, atividadeExtraPesq.cd_atividade_extra).ToList();
                atividadeExtraUI.salas = coordenacaoBiz.findListSalas(true, atividadeExtraPesq.cd_sala, cdEscola).ToList();
                atividadeExtraUI.alunos = alunoBiz.getAlunoByEscola(cdEscola, true, atividadeExtraPesq.cd_aluno).ToList();
                atividadeExtraUI.nm_total_alunos = coordenacaoBiz.getNroPessoasAtividade(atividadeExtraPesq.cd_atividade_extra);


                if (usuarioAntendente != null)
                {
                    atividadeExtraUI.no_usuario = usuarioAntendente.no_usuario;
                    atividadeExtraUI.cd_usuario_atendente = usuarioAntendente.cd_usuario_atendente;
                }
                else
                {
                    atividadeExtraUI.no_usuario = usuario.no_login;
                    atividadeExtraUI.cd_usuario_atendente = usuario.cd_usuario;
                }
                retorno.retorno = atividadeExtraUI;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Retorna listas de objetos para percistência
        [MvcComponentesAuthorize(Roles = "atvex")]
        [MvcComponentesAuthorize(Roles = "tavex")]
        [MvcComponentesAuthorize(Roles = "cur")]
        [MvcComponentesAuthorize(Roles = "func")]
        [MvcComponentesAuthorize(Roles = "alu")]
        [MvcComponentesAuthorize(Roles = "prod")]
        [MvcComponentesAuthorize(Roles = "sala")]
        public RenderJsonActionResult returnDataAtividadeExtra(AtividadeExtraPesquisa atividadeExtraPesq)
        {
            ReturnResult retorno = new ReturnResult();
            AtividadeExtraUI atividadeExtraUI = new AtividadeExtraUI();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                IProfessorBusiness professorBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                IAuthBusiness authBiz = (IAuthBusiness)base.instanciarBusiness<IAuthBusiness>();

                var data = atividadeExtraPesq.data == "" ? "01/01/1989" : atividadeExtraPesq.data;
                var dataAtividade = Convert.ToDateTime(data);
                TimeSpan horaInicial;
                TimeSpan horaFinal;

                if (atividadeExtraPesq.hrInicial != "" && atividadeExtraPesq.hrInicial != null) horaInicial = TimeSpan.Parse(atividadeExtraPesq.hrInicial);
                else horaInicial = new TimeSpan(0);
                if (atividadeExtraPesq.hrFinal != "" && atividadeExtraPesq.hrFinal != null) horaFinal = TimeSpan.Parse(atividadeExtraPesq.hrFinal);
                else horaFinal = new TimeSpan(0);
                int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
                var usuario = authBiz.GetNomeCodigoUsuario(User.Identity.Name);
                var ativo = FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess.ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_ATIVO;
                var usuarioAntendente = coordenacaoBiz.returnAtividadeExtraUsuarioAtendente(atividadeExtraPesq.cd_atividade_extra, cdEscola);
                atividadeExtraUI.tiposAtividadeExtras = coordenacaoBiz.getTipoAtividade(true, atividadeExtraPesq.cd_tipo_ativiade_extra,
                                                                                         null, TipoAtividadeExtraDataAccess.TipoConsultaAtivExtraEnum.HAS_ATIVO).ToList();
                atividadeExtraUI.professores = professorBiz.getFuncionariosByEscola(cdEscola, atividadeExtraPesq.cd_funcionario, true).ToList();
                atividadeExtraUI.produtos = coordenacaoBiz.findProduto(ativo, atividadeExtraPesq.cd_produto,null).ToList();
                atividadeExtraUI.cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_PRODUTO, atividadeExtraPesq.cd_curso, atividadeExtraPesq.cd_produto, cdEscola).ToList();
            
                atividadeExtraUI.salasDisponiveis = coordenacaoBiz.findListSalasDiponiveis(horaInicial, horaFinal, dataAtividade, true, atividadeExtraPesq.cd_sala, cdEscola, atividadeExtraPesq.cd_atividade_extra).ToList();
                atividadeExtraUI.salas = coordenacaoBiz.findListSalas(true, atividadeExtraPesq.cd_sala, cdEscola).ToList();
                atividadeExtraUI.alunos = alunoBiz.getAlunoByEscola(cdEscola, true, atividadeExtraPesq.cd_aluno).ToList();

            
                if (usuarioAntendente != null)
                {
                    atividadeExtraUI.no_usuario = usuarioAntendente.no_usuario;
                    atividadeExtraUI.cd_usuario_atendente = usuarioAntendente.cd_usuario_atendente;
                }
                else
                {
                    atividadeExtraUI.no_usuario = usuario.no_login;
                    atividadeExtraUI.cd_usuario_atendente = usuario.cd_usuario;
                }
                retorno.retorno = atividadeExtraUI;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Retorna listas de objetos para percistência
        [MvcComponentesAuthorize(Roles = "aurepo")]
        [MvcComponentesAuthorize(Roles = "func")]
        [MvcComponentesAuthorize(Roles = "alu")]
        public RenderJsonActionResult returnDataAulaReposicao(AulaReposicaoUI.AulaReposicaoPesquisa aulaReposicaoPesquisa)
        {
            ReturnResult retorno = new ReturnResult();
            AulaReposicaoUI atividadeExtraUI = new AulaReposicaoUI();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                //ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                IProfessorBusiness professorBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                //IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                IAuthBusiness authBiz = (IAuthBusiness)base.instanciarBusiness<IAuthBusiness>();

                var data = aulaReposicaoPesquisa.dt_aula_reposicao == "" ? "01/01/1989" : aulaReposicaoPesquisa.dt_aula_reposicao;
                var dataAtividade = Convert.ToDateTime(data);
                TimeSpan horaInicial;
                TimeSpan horaFinal;

                if (aulaReposicaoPesquisa.dh_inicial_evento != "" && aulaReposicaoPesquisa.dh_inicial_evento != null) horaInicial = TimeSpan.Parse(aulaReposicaoPesquisa.dh_inicial_evento);
                else horaInicial = new TimeSpan(0);
                if (aulaReposicaoPesquisa.dh_final_evento != "" && aulaReposicaoPesquisa.dh_final_evento != null) horaFinal = TimeSpan.Parse(aulaReposicaoPesquisa.dh_final_evento);
                else horaFinal = new TimeSpan(0);
                int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
                var usuario = authBiz.GetNomeCodigoUsuario(User.Identity.Name);
                var usuarioAntendente = coordenacaoBiz.returnAulaReposicaoUsuarioAtendente(aulaReposicaoPesquisa.cd_aula_reposicao, cdEscola);
                atividadeExtraUI.professoresEdit = professorBiz.getFuncionariosByEscolaAulaReposicao(cdEscola, aulaReposicaoPesquisa.cd_professor, true).ToList();
                //atividadeExtraUI.alunos = alunoBiz.getAlunoByEscola(cdEscola, true, aulaReposicaoPesquisa.cd_aluno).ToList();
                atividadeExtraUI.salasDisponiveis = coordenacaoBiz.findListSalasDiponiveis(horaInicial, horaFinal, dataAtividade, true, aulaReposicaoPesquisa.cd_sala, cdEscola, aulaReposicaoPesquisa.cd_aula_reposicao).ToList();
                atividadeExtraUI.salas = coordenacaoBiz.findListSalas(true, aulaReposicaoPesquisa.cd_sala, cdEscola).ToList();


                if (usuarioAntendente != null)
                {
                    atividadeExtraUI.no_usuario = usuarioAntendente.no_usuario;
                    atividadeExtraUI.cd_atendente = usuarioAntendente.cd_atendente;
                }
                else
                {
                    atividadeExtraUI.no_usuario = usuario.no_login;
                    atividadeExtraUI.cd_atendente = usuario.cd_usuario;
                }
                retorno.retorno = atividadeExtraUI;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Retorna listas de objetos para percistência
        [MvcComponentesAuthorize(Roles = "aurepo")]
        [MvcComponentesAuthorize(Roles = "func")]
        [MvcComponentesAuthorize(Roles = "alu")]
        public RenderJsonActionResult getAulaReposicaoViewOnDbClik(AulaReposicaoUI.AulaReposicaoPesquisa aulaReposicaoPesq)
        {
            ReturnResult retorno = new ReturnResult();
            AulaReposicaoUI aulaReposicaoUi = new AulaReposicaoUI();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                //ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                IProfessorBusiness professorBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                //IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                IAuthBusiness authBiz = (IAuthBusiness)base.instanciarBusiness<IAuthBusiness>();

                var data = aulaReposicaoPesq.dt_aula_reposicao == "" ? "01/01/1989" : aulaReposicaoPesq.dt_aula_reposicao;
                var dataAtividade = Convert.ToDateTime(data);
                TimeSpan horaInicial;
                TimeSpan horaFinal;

                if (aulaReposicaoPesq.dh_inicial_evento != "" && aulaReposicaoPesq.dh_inicial_evento != null) horaInicial = TimeSpan.Parse(aulaReposicaoPesq.dh_inicial_evento);
                else horaInicial = new TimeSpan(0);
                if (aulaReposicaoPesq.dh_final_evento != "" && aulaReposicaoPesq.dh_final_evento != null) horaFinal = TimeSpan.Parse(aulaReposicaoPesq.dh_final_evento);
                else horaFinal = new TimeSpan(0);
                int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
                var usuario = authBiz.GetNomeCodigoUsuario(User.Identity.Name);
                var usuarioAntendente = coordenacaoBiz.returnAulaReposicaoUsuarioAtendente(aulaReposicaoPesq.cd_aula_reposicao, cdEscola);
                aulaReposicaoUi.professoresEdit = professorBiz.getFuncionariosByEscolaAulaReposicao(cdEscola, aulaReposicaoPesq.cd_professor, true).ToList();
                //aulaReposicaoUi.alunos = alunoBiz.getAlunoByEscola(cdEscola, true, aulaReposicaoPesq.cd_aluno).ToList();
                aulaReposicaoUi.salasDisponiveis = coordenacaoBiz.findListSalasDiponiveisAulaRep(horaInicial, horaFinal, dataAtividade, true, aulaReposicaoPesq.cd_sala, cdEscola, aulaReposicaoPesq.cd_aula_reposicao, aulaReposicaoPesq.cd_turma).ToList();
                aulaReposicaoUi.salas = coordenacaoBiz.findListSalas(true, aulaReposicaoPesq.cd_sala, cdEscola).ToList();

                if (aulaReposicaoPesq.cd_turma_destino != 0) //Estava nulo mas nunca será
                {
                    aulaReposicaoUi.no_turma = aulaReposicaoPesq.no_turma_destino;
                    aulaReposicaoUi.cd_turma_destino = aulaReposicaoPesq.cd_turma_destino;
                }


                if (usuarioAntendente != null)
                {
                    aulaReposicaoUi.no_usuario = usuarioAntendente.no_usuario;
                    aulaReposicaoUi.cd_atendente = usuarioAntendente.cd_atendente;
                }
                else
                {
                    aulaReposicaoUi.no_usuario = usuario.no_login;
                    aulaReposicaoUi.cd_atendente = usuario.cd_usuario;
                }
                retorno.retorno = aulaReposicaoUi;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Retorna listas de objetos para percistência
        [MvcComponentesAuthorize(Roles = "aurepo")]
        [MvcComponentesAuthorize(Roles = "func")]
        [MvcComponentesAuthorize(Roles = "alu")]
        public RenderJsonActionResult obterRecursosAulaReposicao(AulaReposicaoUI.AulaReposicaoPesquisa aulaReposicaoPesq)
        {
            ReturnResult retorno = new ReturnResult();
            AulaReposicaoUI aulaReposicaoUI = new AulaReposicaoUI();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IProfessorBusiness professorBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                IAuthBusiness authBiz = (IAuthBusiness)base.instanciarBusiness<IAuthBusiness>();
                var data = aulaReposicaoPesq.dt_aula_reposicao == "" ? "01/01/1989" : aulaReposicaoPesq.dt_aula_reposicao;
                var dataAtividade = Convert.ToDateTime(data);
                TimeSpan horaInicial;
                TimeSpan horaFinal;

                if (aulaReposicaoPesq.dh_inicial_evento != "" && aulaReposicaoPesq.dh_inicial_evento != null) horaInicial = TimeSpan.Parse(aulaReposicaoPesq.dh_inicial_evento);
                else horaInicial = new TimeSpan(0);
                if (aulaReposicaoPesq.dh_final_evento != "" && aulaReposicaoPesq.dh_final_evento != null) horaFinal = TimeSpan.Parse(aulaReposicaoPesq.dh_final_evento);
                else horaFinal = new TimeSpan(0);

                int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
                var usuario = authBiz.GetNomeCodigoUsuario(User.Identity.Name);
                var usuarioAntendente = coordenacaoBiz.returnAulaReposicaoUsuarioAtendente(aulaReposicaoPesq.cd_aula_reposicao, cdEscola);

                aulaReposicaoUI.professoresEdit = professorBiz.getFuncionariosByEscolaAulaReposicao(cdEscola, aulaReposicaoPesq.cd_professor, true).ToList();
                aulaReposicaoUI.salasDisponiveis = coordenacaoBiz.findListSalasDiponiveisAulaRep(horaInicial, horaFinal, dataAtividade, true, aulaReposicaoPesq.cd_sala, cdEscola, aulaReposicaoPesq.cd_aula_reposicao, aulaReposicaoPesq.cd_turma).ToList();

                if (usuarioAntendente != null)
                {
                    aulaReposicaoUI.no_usuario = usuarioAntendente.no_usuario;
                    aulaReposicaoUI.cd_atendente = usuarioAntendente.cd_atendente;
                }
                else
                {
                    aulaReposicaoUI.no_usuario = usuario.no_login;
                    aulaReposicaoUI.cd_atendente = usuario.cd_usuario;
                }

                retorno.retorno = aulaReposicaoUI;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        #endregion
    }
}
