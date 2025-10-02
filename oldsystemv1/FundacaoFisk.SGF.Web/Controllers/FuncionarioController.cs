using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericController;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using log4net;


namespace FundacaoFisk.SGF.Web.Controllers
{
    public class FuncionarioController : ComponentesMVCController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PessoaController));

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Funcionario()
        {
            return View();
        }

        //Método construtor
        public FuncionarioController()
        {
        }

        [MvcComponentesAuthorize(Roles = "func")]
        public ActionResult getProfessoresDisponiveisFaixaHorario(string desc, string nomeRed, bool inicio, bool? status, string cpf, int sexo, int cd_turma, int cd_curso, bool PPT_pai, int cd_produto,
             string dt_Inicio, string dt_Final, int cd_duracao)
        {
            List<Horario> horariosTurma = null;
            ReturnResult retorno = new ReturnResult();
            int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
            DateTime dtInicio = (!String.IsNullOrEmpty(dt_Inicio) && dt_Inicio != "null") ? (DateTime) Convert.ToDateTime(dt_Inicio) : DateTime.MinValue;

            DateTime? dtFinal = (!String.IsNullOrEmpty(dt_Final) && dt_Final != "null") ? (DateTime)Convert.ToDateTime(dt_Final)  : (DateTime?)null;

            try
            {
                ISecretariaBusiness SecretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IProfessorBusiness ProfBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                horariosTurma = ((List<Horario>)Session["HorariosTurma"]);
                if (horariosTurma == null && cd_turma > 0)
                    horariosTurma = SecretariaBiz.getHorarioByEscolaForRegistro(cdEscola, cd_turma, Horario.Origem.TURMA).ToList();
                if (horariosTurma != null)
                    foreach (Horario h in horariosTurma)
                    {
                        if (h.cd_horario == 0 && h.endTime != null && h.startTime != null)
                        {
                            h.dt_hora_ini = new TimeSpan(h.startTime.Hour, h.startTime.Minute, h.startTime.Second);
                            h.dt_hora_fim = new TimeSpan(h.endTime.Hour, h.endTime.Minute, h.endTime.Second);
                        }
                    }
                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                List<FuncionarioSearchUI> listProfessor = ProfBiz.getProfessoresDisponiveisFaixaHorario(parametros, desc, nomeRed, inicio, status, cpf, sexo, horariosTurma, 
                    cdEscola, cd_turma, cd_curso, PPT_pai, cd_produto, dtInicio, dtFinal, cd_duracao).ToList();
                if (listProfessor == null)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                var retRender = new RenderJsonActionResult { Result = listProfessor, parameters = parametros };
                return retRender;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
            finally
            {
                Session["HorariosTurma"] = null;
                Session.Remove("HorariosTurma");
            }
        }

        [MvcComponentesAuthorize(Roles = "func")]
        public ActionResult pesquisaProfessorHorarioPorHorariosTurmaFilha(int cd_turma_PPT)
        {
            List<Horario> horariosTurma = null;
            ReturnResult retorno = new ReturnResult();
            int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
            try
            {
                ISecretariaBusiness SecretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IProfessorBusiness ProfBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                horariosTurma = ((List<Horario>)Session["HorariosTurma"]);
                if (horariosTurma == null && cd_turma_PPT > 0)
                    horariosTurma = SecretariaBiz.getHorarioByEscolaForRegistro(cdEscola, cd_turma_PPT, Horario.Origem.TURMA).ToList();
                if (horariosTurma != null)
                    foreach (Horario h in horariosTurma)
                    {
                        if (h.cd_horario == 0 && h.endTime != null && h.startTime != null)
                        {
                            h.dt_hora_ini = new TimeSpan(h.startTime.Hour, h.startTime.Minute, h.startTime.Second);
                            h.dt_hora_fim = new TimeSpan(h.endTime.Hour, h.endTime.Minute, h.endTime.Second);
                        }
                    }
                List<FuncionarioSearchUI> listProfessor = ProfBiz.getProfessoresTurmaPPTPorFaixaHorariosTurmaFilha(horariosTurma, cdEscola, cd_turma_PPT).ToList();
                retorno.retorno = listProfessor;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
            finally
            {
                Session["HorariosTurma"] = null;
                Session.Remove("HorariosTurma");
            }

        }

    }
}
