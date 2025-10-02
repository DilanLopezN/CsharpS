using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Componentes.GenericController;
using Componentes.Utils;
using DALC4NET;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using log4net;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;

namespace FundacaoFisk.SGF.Web.Controllers
{
    public class TurmaController : ComponentesMVCController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TurmaController));
        
        //Método construtor
         public TurmaController()
        {
        }

         //Retorna o código da escola
         private int recoverEscola()
         {
             var codEscola = (int)Session["CodEscolaSelecionada"];
             return codEscola;
         }

         // POST api/Usuario
         [MvcComponentesAuthorize(Roles = "tur")]
         public ActionResult postHorariosTurma(ICollection<Horario> horariosTurma)
         {
             ReturnResult retorno = new ReturnResult();
             Session["HorariosTurma"] = horariosTurma;
             try
             {
                 return new RenderJsonActionResult { Result = true };
             }
             catch (Exception ex)
             {
                 return gerarLogException(Messages.msgRegNotEnc, retorno, logger, ex);
             }
         }
       
        [HttpGet]
        [MvcComponentesAuthorize(Roles = "tur")]
        public ActionResult GetTurma(int tipo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                int cdEscola = recoverEscola();
                IEnumerable<Turma> turmas = turmaBiz.findTurma(cdEscola, null, (TurmaDataAccess.TipoConsultaTurmaEnum)tipo);
                return new RenderJsonActionResult { Result = turmas };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }


        [HttpGet]
        [HttpComponentesAuthorize(Roles = "percfagruav")]
        public ActionResult findTurmaPercentualFaltaGrupoAvancado(int cd_turma, int? cd_turma_ppt, bool id_turma_ppt)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = recoverEscola();
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                Turma result = turmaBiz.findTurmaPercentualFaltaGrupoAvancado(cdEscola, cd_turma, cd_turma_ppt, id_turma_ppt);

                //se a turma for Personalizada pai
                if (id_turma_ppt == true && cd_turma_ppt == null)
                {
                    throw new TurmaBusinessException(Messages.msgTurmaPPTPai, null, 0, false);
                    
                }

                //Verifica se o produtos é diferente de Ingles e Espanhol
                var produtos = new int[]
                {
                    (int) Produto.TipoProduto.INGLES,
                    (int) Produto.TipoProduto.ESPANHOL, 
                };

                if (!produtos.Contains(result.Curso.Produto.cd_produto))
                {
                    throw new TurmaBusinessException(Messages.msgProdutosExecute, null, 0, false);
                    
                }


                //se a turma não for de nível avançado
                if (result.Curso.Nivel.cd_nivel != (int)Nivel.SituacaoHistorico.AVANCADO)
                {
                    throw new TurmaBusinessException(Messages.msgProdutosExecute, null, 0, false);
                    
                }




                LocalReport AppReportViewer = new LocalReport();
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
                string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;
                //AppReportViewer.Reset();
                AppReportViewer.DataSources.Clear();
                AppReportViewer.EnableExternalImages = true;

                var nomeRelatorio = string.Format("{0}{1}", ConfigurationManager.AppSettings["caminhoRelatorio"], "Coordenacao/ControleFaltas/CartaAviso.rdlc");
                AppReportViewer.ReportPath = nomeRelatorio;

                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                //configurações da página ex: margin, top, left ...
                string deviceInfo =
                "<DeviceInfo>" +
                "<OutputFormat>PDF</OutputFormat>" +
                "</DeviceInfo>";


                Warning[] warnings;
                string[] streams;
                byte[] bytes;
                //string filenameParams = string.Format("{0}{1}", "Etiqueta_", cd_mala);

                DataTable dtReportData = new DataTable();

                DBHelper dbSql = new DBHelper(connectionString, providerName);

                DBParameter param1 = new DBParameter("@cd_turma", cd_turma, DbType.Int32);

                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(param1);

                dtReportData = dbSql.ExecuteDataTable("sp_RptTurmaControleFalta", paramCollection, CommandType.StoredProcedure);

                AppReportViewer.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataRelatorio", dtReportData));
                AppReportViewer.Refresh();
                bytes = AppReportViewer.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

                turmaBiz.editTurmaControleFalta(cd_turma);

                return File(bytes, mimeType);

                
            }
            catch (Exception ex)
            {
                string url = Session["enderecoWeb"] + "/Erro";
                string stackTrace = HttpUtility.UrlEncode(ex.Message.ToString() + ex.StackTrace + ex.InnerException);

                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarArquivo, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(ex.Message.ToString()) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
            }
        }
    }
}
