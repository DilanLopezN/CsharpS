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
using System.IO;
using System.Configuration;


namespace FundacaoFisk.SGF.Web.Controllers
{
    public class InformacaoController : ComponentesMVCController
    {
        //Propriedades
        private static readonly ILog logger = LogManager.GetLogger(typeof(CoordenacaoController));

       

        public ActionResult Video()
        {
            return View();
        }

        public ActionResult VideoDetail()
        {
            return View();
        }

        public ActionResult Circular()
        {
            return View();
        } 
        
        public ActionResult Faq()
        {
            return View();
        }

        public ActionResult FaqVideoDetail()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult getDownloadArquivo(int cd_video)
        {
            try {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();

                var videoAux = coordenacaoBiz.obterVideoPorID(cd_video);

                if (videoAux == null)
                {
                    throw new CoordenacaoBusinessException(Messages.msgRegNotEnc, null, CoordenacaoBusinessException.TipoErro.ERRO_VIDEO_NAO_ENCONTRADO, false);
                }

                string caminho_video = ConfigurationManager.AppSettings["caminhoUploads"];
                byte[] data = System.IO.File.ReadAllBytes(caminho_video + "\\Arquivos\\Videos\\" + videoAux.no_arquivo_video);
                MemoryStream stream = new MemoryStream(data);



                //return this.Content(fs.ToArray().ToString(), "application/pdf");
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("content-disposition", "attachment;filename=" + videoAux.no_arquivo_video);
                Response.Buffer = true;
                Response.Clear();
                byte[] buf = stream.ToArray();
                Response.OutputStream.Write(buf, 0, buf.Length);
                Response.OutputStream.Flush();
                Response.End();
                stream.Dispose();
                return this.Content(stream.ToArray().ToString(), "application/octet-stream");
            }
            catch (Exception ex)
            {
                string url = Session["enderecoWeb"] + "/Erro";
                string stackTrace = HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarArquivo + ex.StackTrace + ex.InnerException);

                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarArquivo, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarArquivo) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
            }
        }

        [AllowAnonymous]
        public ActionResult getDownloadCircular(int cd_circular)
        {
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var circular = coordenacaoBiz.obterCircularPorID(cd_circular);

                if (circular == null)
                    throw new CoordenacaoBusinessException(Messages.msgRegNotEnc, null, CoordenacaoBusinessException.TipoErro.ERRO_VIDEO_NAO_ENCONTRADO, false);


                string caminho_circular = ConfigurationManager.AppSettings["caminhoUploads"];
                byte[] data = System.IO.File.ReadAllBytes(caminho_circular + "\\Arquivos\\Circulares\\" + circular.no_arquivo_circular);
                MemoryStream stream = new MemoryStream(data);

                Response.ContentType = "application/octet-stream";
                Response.AddHeader("content-disposition", "attachment;filename=" + circular.no_arquivo_circular);
                Response.Buffer = true;
                Response.Clear();
                byte[] buf = stream.ToArray();
                Response.OutputStream.Write(buf, 0, buf.Length);
                Response.OutputStream.Flush();
                Response.End();
                stream.Dispose();
                return this.Content(stream.ToArray().ToString(), "application/octet-stream");
            }
            catch (Exception ex)
            {
                string url = Session["enderecoWeb"] + "/Erro";
                string stackTrace = HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarArquivo + ex.StackTrace + ex.InnerException);

                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarArquivo, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarArquivo) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
            }
        }

        [MvcComponentesAuthorize(Roles = "impcont")]
        public ActionResult getDownloadImpressosContingencia()
        {
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();



                string caminho_impressos_contingencia = ConfigurationManager.AppSettings["caminhoUploads"];
                byte[] data = System.IO.File.ReadAllBytes(caminho_impressos_contingencia + "\\Arquivos\\Impressos\\Impressos-contingencia.zip");
                MemoryStream stream = new MemoryStream(data);

                Response.ContentType = "application/octet-stream";
                Response.AddHeader("content-disposition", "attachment;filename=Impressos-contingencia.zip" );
                Response.Buffer = true;
                Response.Clear();
                byte[] buf = stream.ToArray();
                Response.OutputStream.Write(buf, 0, buf.Length);
                Response.OutputStream.Flush();
                Response.End();
                stream.Dispose();
                return this.Content(stream.ToArray().ToString(), "application/octet-stream");
            }
            catch (Exception ex)
            {
                string url = Session["enderecoWeb"] + "/Erro";
                string stackTrace = HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarArquivo + ex.StackTrace + ex.InnerException);

                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarArquivo, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarArquivo) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
            }
        }
    }
}
