using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Utils;
using Componentes.Utils.Messages;
using log4net;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using Componentes.GenericController;
using System.Configuration;
using Componentes.GenericBusiness.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;


namespace FundacaoFisk.SGF.Web.Controllers
{
    public class AlunoController : ComponentesMVCController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(AlunoController));

        public AlunoController()
        {
        }
        
        //[HttpGet]
        //[MvcComponentesAuthorize(Roles = "alu")]
        //public ActionResult GetAluno()
        //{
        //    ReturnResult retorno = new ReturnResult();
        //    try
        //    {
        //        int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
        //        IEnumerable<AlunoSearchUI> alunos = BusinessAluno.findAluno(cdEscola);
        //        return new RenderJsonActionResult { Result = alunos };
        //    }
        //    catch (Exception ex)
        //    {
        //        return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
        //    }
        //}

        [MvcComponentesAuthorize(Roles = "alu")]
        public ActionResult getAlunosDisponiveisFaixaHorario(string nome, string email, int status, string cpf, bool inicio, int sexo, int cdTurma, int cd_produto, 
            bool id_turma_PPT, string dta_final_aula, int cd_curso, int cd_duracao)
        {
            List<Horario> horariosTurma = null;
            ReturnResult retorno = new ReturnResult();
            int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
            try
            {
                ISecretariaBusiness BusinessSec = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IAlunoBusiness BusinessAluno = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                horariosTurma = ((List<Horario>)Session["HorariosTurma"]);
                DateTime? dt_final_aula = string.IsNullOrEmpty(dta_final_aula) ? null : (DateTime?)Convert.ToDateTime(dta_final_aula);
                if (horariosTurma == null && cdTurma > 0)
                    horariosTurma = BusinessSec.getHorarioByEscolaForRegistro(cdEscola, cdTurma, Horario.Origem.TURMA).ToList();
                if (horariosTurma != null)
                    foreach (Horario h in horariosTurma)
                        if (h.cd_horario == 0 && h.endTime != null && h.startTime != null)
                        {
                            h.dt_hora_ini = new TimeSpan(h.startTime.Hour, h.startTime.Minute, h.startTime.Second);
                            h.dt_hora_fim = new TimeSpan(h.endTime.Hour, h.endTime.Minute, h.endTime.Second);
                        }
                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                var alunos = BusinessAluno.getAlunosDisponiveisFaixaHorario(parametros, nome, email, inicio, getStatus(status), cpf, sexo, horariosTurma, cdEscola, cdTurma, cd_produto, id_turma_PPT, dt_final_aula, cd_curso, cd_duracao);
                if (alunos == null)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                var retRender = new RenderJsonActionResult { Result = alunos, parameters = parametros };
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

    }
}