using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using Componentes.GenericController;
using Componentes.GenericModel;
using Componentes.Utils;
using Componentes.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using log4net;
using System.IO;
using System.Net;

namespace FundacaoFisk.SGF.Web.Controllers
{
    public class FileManagerController : ComponentesMVCController
    {
        private const string SENHA = "CR%wVG0lGiG0QPvgr6S2$fcc7HuzZ";
        private static readonly ILog logger = LogManager.GetLogger(typeof(FileManagerController));

        public ActionResult Index()
        {
            return View();
        }

        public RenderJsonActionResult getArquivosSearch(string caminho, string senha)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (senha != SENHA)
                    throw new Exception("Sem autorização");
                List<FileManager> listaArquivos = new List<FileManager>();
                DirectoryInfo diretorio = new DirectoryInfo(caminho);
                FileInfo[] arquivos = diretorio.GetFiles("*.*");
                DirectoryInfo[] diretorios = diretorio.GetDirectories();
                int id = 0;
                foreach (DirectoryInfo directoryInfo in diretorios)
                {
                    FileManager fileManager = new FileManager();
                    fileManager.cd_arquivo = id++;
                    fileManager.no_arquivo = directoryInfo.Name;
                    fileManager.no_tipo = "Diretório";
                    fileManager.dt_ultima_modificacao = directoryInfo.LastWriteTime;
                    fileManager.no_caminho = directoryInfo.FullName;
                    listaArquivos.Add(fileManager);
                }

                foreach (FileInfo fileInfo in arquivos)
                {
                    FileManager fileManager = new FileManager();
                    id++;
                    fileManager.no_arquivo = fileInfo.Name;
                    fileManager.no_tipo = "Arquivo";
                    fileManager.tamanho = fileInfo.Length;
                    fileManager.dt_ultima_modificacao = fileInfo.LastWriteTime;
                    fileManager.no_caminho = fileInfo.FullName;
                    fileManager.somente_leitura = fileInfo.IsReadOnly;
                    listaArquivos.Add(fileManager);
                }

                retorno.retorno = listaArquivos;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                ex = new ControllerException(ex.Message, ex);
                retorno.AddMensagem(Messages.msgRegBuscError, ex.Message + ex.StackTrace, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return new RenderJsonActionResult { Result = new { erro = retorno } };
            }
        }

        public ActionResult getArquivoDiretorio(string urlPath, string senha, string no_arquivo)
        {
            try
            {
                if (senha != SENHA)
                    throw new Exception("Sem autorização");

                urlPath = urlPath.Replace("\\", "//");
                if (System.IO.File.Exists(urlPath))
                {
                    string extensao = Path.GetExtension(urlPath);
                    if (extensao == ".dll" || extensao == ".exe")
                        throw new SecretariaBusinessException("Usuário não tem acesso para realizar esta operação.", null, SecretariaBusinessException.TipoErro.ERRO_NAO_EXISTE_LAYOUT_NOME_CONTRATO, false);
                    FileStream st = new FileStream(urlPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    byte[] buffer = new byte[st.Length];
                    st.Read(buffer, 0, (int)st.Length);
                    //return this.Content(fs.ToArray().ToString(), "application/pdf");
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                    Response.AddHeader("content-disposition", "attachment; filename=" + no_arquivo);
                    Response.ContentEncoding = System.Text.Encoding.Unicode;
                    Response.Buffer = true;
                    Response.Charset = "unicode";
                    Response.Clear();
                    Response.OutputStream.Write(buffer, 0, buffer.Length);
                    Response.OutputStream.Flush();
                    Response.End();

                    st.Close();

                    //return this.Content(buffer.ToString(), "application/vnd.openxmlformats-officedocument.wordprocessingml.template");
                    return new FileStreamResult(Response.OutputStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.template");
                }
                else
                    throw new Exception("Não existe caminho para esse arquivo");
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                Session["Erro"] = "Ocorreu um erro ao tentar baixar o arquivo.";
                Session["StackTrace"] = ex.Message + ex.StackTrace + ex.InnerException;
                return Redirect("~/Erro/Index");
            }
        }

        [AllowAnonymous]
        public ActionResult UploadArquivo()
        {
            ReturnResult retorno = new ReturnResult();
            //try
            //{
            //Arquivo que o PlUpload envia.
            var fileUpload = Request.Files[0];
            //Local onde vai ficar as fotos enviadas.
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                // extract only the fielname
                var fileName = Path.GetFileName(fileUpload.FileName);
                //DirectoryInfo di = new DirectoryInfo(pathContratosEscola);
                //if (!di.Exists)
                //    di.Create();
                //var path = Path.Combine(pathContratosEscola + "/", fileName);
                //salva o arquivo carregado.
                //fileUpload.SaveAs(path);
            }
            return new RenderJsonActionResult { Result = fileUpload.FileName + ".dotx" };
            //}
            //catch (Exception ex)
            //{
            //    return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotUploadImage, retorno, logger, ex);
            //}
        }
        [AllowAnonymous]
        public ActionResult getMostrarCacheAplicacao()
        {
            var PhysicalMemoryLimit = HttpContext.Cache.EffectivePercentagePhysicalMemoryLimit;
            var PrivateBytesLimit = HttpContext.Cache.EffectivePrivateBytesLimit;
            var itensCache = (from System.Collections.DictionaryEntry dict in HttpContext.Cache
                               //let key = dict.Key
                               select new { chave = dict.Key, valor = dict.Value }).ToList();
            var itensSession = (from  Object dict in HttpContext.Session
                               //let key = dict.Key
                                select dict).ToList();

            //var itensCache = (new System.Linq.SystemCore_EnumerableDebugView(HttpContext.Cache)).Items;
            return new RenderJsonActionResult { Result = new { ItensCache = itensCache, itensSession = itensSession } };
        }
    }

    public class FileManager : TO
    {
        public int cd_arquivo { get; set; }
        public string no_arquivo { get; set; }
        public double tamanho { get; set; }
        public string no_tipo { get; set; }
        public DateTime dt_ultima_modificacao { get; set; }
        public string no_caminho { get; set; }
        public bool somente_leitura { get; set; }
    }
}