using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.GenericController;
using FundacaoFisk.SGF.Utils.Messages;
using log4net;

namespace FundacaoFisk.SGF.Web.Controllers
{
    public class UtilController : ComponentesMVCController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PessoaController));
        //
        // GET: /Util/

        public ActionResult Index()
        {
            return View();
        }

        public string getDataCorrente(bool trazerHora) {
            if (trazerHora)
                return String.Format("{0:dd/MM/yyyy H:mm:ss}", DateTime.Now);
            return String.Format("{0:dd/MM/yyyy}", DateTime.Now);
        }

        public ActionResult PostDataHoraCorrente()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                DateTime dataCorrente = DateTime.UtcNow;
                int IdFusoHorario = !string.IsNullOrEmpty(Session["IdFusoHorario"] + "") ? int.Parse(Session["IdFusoHorario"] + "") : 0;
                bool IdHorarioVerao = !string.IsNullOrEmpty(Session["IdHorarioVerao"] + "") ? bool.Parse(Session["IdHorarioVerao"] + "") : false;
                var dataCorrenteLocalTime = SGF.Utils.ConversorUTC.ToLocalTime(dataCorrente, IdFusoHorario, IdHorarioVerao);
                if (dataCorrenteLocalTime == null)
                    dataCorrenteLocalTime = dataCorrente.ToLocalTime();
                DataCorrente dataRetorno = new DataCorrente
                {
                    dataPortugues = String.Format("{0:dd/MM/yyyy H:mm:ss}", dataCorrenteLocalTime),
                    dataIngles = String.Format("{0:yyyy/MM/dd H:mm:ss}", dataCorrenteLocalTime),
                    data = dataCorrente

                };
                retorno.retorno = dataRetorno;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        public ActionResult PostDataInicialEFinalMes()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                DateTime dataCorrenteInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime dataCorrenteFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dataCorrenteFinal = dataCorrenteFinal.AddMonths(1);
                dataCorrenteFinal = dataCorrenteFinal.AddDays(-1);
                retorno.retorno = new { dt_ini_mes = dataCorrenteInicial, dt_fim_mes = dataCorrenteFinal };
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        public ActionResult postGerarLog(string stack) {
            ReturnResult retorno = new ReturnResult();
            return gerarLogException(Messages.msgRegBuscError, retorno, logger, new Exception("Erro WEB: " + stack));
        }
    }

    public class DataCorrente
    {
        public string dataIngles { get; set; }
        public string dataPortugues { get; set; }
        public DateTime data { get; set; }
    }
}
