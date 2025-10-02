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
using FundacaoFisk.SGF.Web.Services.Auth.Comum;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using Componentes.GenericBusiness.Comum;
using System.Configuration;
using System.IO;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.CNAB.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IBusiness;


namespace FundacaoFisk.SGF.Web.Controllers
{
    public class CnabController : ComponentesMVCController
    {
        //
        // GET: /Secretaria/
   
        private static readonly ILog logger = LogManager.GetLogger(typeof(CnabController));
        public CnabController()
        {
        }

        #region ActionResult

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Prospect()
        {
            return View();
        }

        public ActionResult Aluno()
        {
            return View();
        }

        public ActionResult Pessoa()
        {
            return View();
        }

        public ActionResult NomeContrato()
        {
            return View();
        }

        public ActionResult Movimentos()
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

        #region Retorno CNAB

        [HttpPost]
        [MvcComponentesAuthorize(Roles = "rtcnb.i")]
        public ActionResult UploadRetorno()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                //Arquivo que o PlUpload envia.
                string nm_banco = Request.Form["nm_banco"].ToString();
                var fileUpload = Request.Files[0];
                string extensaoArq = fileUpload.FileName.Substring(fileUpload.FileName.Length - 4);

                if (fileUpload.FileName.Contains("=") || fileUpload.FileName.Contains("&"))
                {
                    throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErrorCaracteresInvalidosNomeArquivo, null, CnabBusinessException.TipoErro.ERRO_CARACTERES_INVALIDOS_NOME_ARQUIVO, false);
                }

                //Local onde vai ficar as fotos enviadas.
                if (fileUpload.ContentLength > 6145000) // Aumentamos para 6MB devido ao chamado 275915
                    throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroLimiteArquivoRetornoExcedido, null, CnabBusinessException.TipoErro.ERRO_LIMITE_ARQUIVO_EXCEDIDO, false);
                if (!String.IsNullOrEmpty(nm_banco) && nm_banco == ((int)Cnab.Bancos.Sicred + ""))
                {
                    if (extensaoArq.ToLower() != ".txt" && extensaoArq.ToLower() != ".ret" && extensaoArq.ToLower() != ".dat" && extensaoArq.ToLower() != ".crt" && extensaoArq.ToLower() != ".rst")
                        throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtensaoNaoSuportadaRetorno, null,
                            CnabBusinessException.TipoErro.ERRO_TIPO_ARQUIVO_NAO_SUPORTADO, false);
                }
                else
                {
                    if (extensaoArq.ToLower() != ".txt" && extensaoArq.ToLower() != ".ret" && extensaoArq.ToLower() != ".dat" && extensaoArq.ToLower() != ".rst")
                        throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtensaoNaoSuportadaRetorno, null,
                            CnabBusinessException.TipoErro.ERRO_TIPO_ARQUIVO_NAO_SUPORTADO, false);
                }
               
                string pathRetornosEscola = "";
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                int codUsuario = int.Parse(Session["CodUsuario"] + "");
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(codUsuario);
                string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
                //string file_name = Guid.NewGuid().ToString();
                string file_name = Utils.Utils.geradorNomeAleatorio(36);
                pathRetornosEscola = caminho_relatorios + "/Retornos/" + cd_escola;
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    // extract only the fielname
                    //var fileName = Path.GetFileName(fileUpload.FileName);
                    DirectoryInfo di = new DirectoryInfo(pathRetornosEscola);
                    if (!di.Exists)
                        di.Create();
                    //gera a path completa onde sera salvo o arquivo.
                    string nomeArquivo =  file_name + ".txt";
                    var path = Path.Combine(pathRetornosEscola + "/", nomeArquivo);
                    //salva o arquivo carregado.
                    fileUpload.SaveAs(path);
                    fileUpload.InputStream.Dispose();
                }
                return new RenderJsonActionResult { Result = file_name + ".txt" };
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotUploadImage, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "rtcnb")]
        public ActionResult getArquivoRetorno(string noRetorno, int cdRetorno)
        {
            try
            {
                ICnabBusiness BusinessCnab = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                int codUsuario = int.Parse(Session["CodUsuario"] + "");
                string pathRetorno = "";
                RetornoCNAB retCNAB = BusinessCnab.getRetornoCnabFull(cdRetorno, cd_escola);
                string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
                pathRetorno = caminho_relatorios + "/Retornos/" + cd_escola + "/" + noRetorno;
                if (System.IO.File.Exists(pathRetorno))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(pathRetorno);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, noRetorno);
                }
                else
                    throw new CnabBusinessException(Utils.Messages.Messages.msgAvisoNaoexisteRetorno, null, CnabBusinessException.TipoErro.ERRO_RETORNO_COM_NOME_INFORMADO, false);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Session["Erro"] = FundacaoFisk.SGF.Utils.Messages.Messages.msgAvisoNaoexisteRetorno;
                Session["StackTrace"] = ex.Message + ex.StackTrace + ex.InnerException;
                return Redirect("~/Erro/Index");
            }
        }

        #endregion

    }    

}
