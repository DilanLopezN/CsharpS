using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Resources;
using System.Threading;
using System.Web.Http;
using System.Web.UI.WebControls;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using Newtonsoft.Json;
using FundacaoFisk.SGF.Utils.Messages;
using log4net;
using System.Web;
using Componentes.GenericController;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IBusiness;
using Componentes.GenericBusiness.Comum;
using System.Configuration;
using System.IO;
using FundacaoFisk.SGF.Services.EmailMarketing.Business;
using FundacaoFisk.SGF.Services.EmailMarketing.Model;
using System.Data;


namespace FundacaoFisk.SGF.Web.Services.EmailMarketing.Controllers
{
    public class EmailMarketingController : ComponentesApiController
    {
        //Declaração de Atributos
        private static readonly ILog logger = LogManager.GetLogger(typeof(EmailMarketingController));

        //Método construtor
        public EmailMarketingController()
        {
        }

        [HttpComponentesAuthorize(Roles = "mailm")]
        public HttpResponseMessage getTemaPronto(string tema)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                string uploadPath = ConfigurationManager.AppSettings["caminhoUploads"];
                string serverUploadPath = Path.Combine(uploadPath, "MailMarketing");
                string caminhoTemasProntos = Path.Combine(serverUploadPath, "TemasProntos");
               
                //Faz um checagem se existe diretórios de temas
                Stream st = Componentes.Utils.ManipuladorArquivo.lerArquivo(Path.Combine(caminhoTemasProntos, tema));
                string html = new StreamReader(st, System.Text.Encoding.UTF8).ReadToEnd();

                retorno.retorno = html;
                Componentes.Utils.ManipuladorArquivo.fecharArquivo(st);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUploadImage, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mailm")]
        public HttpResponseMessage getTemasProntos()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                string uploadPath = ConfigurationManager.AppSettings["caminhoUploads"];
                string serverUploadPath = Path.Combine(uploadPath, "MailMarketing");
                string caminhoTemasProntos = Path.Combine(serverUploadPath, "TemasProntos");
               
                //Faz um checagem se existe diretórios de temas
                string[] nomesArquivos = Componentes.Utils.ManipuladorArquivo.listarNomesArquivos(caminhoTemasProntos);

                //Remove os caminhos dos arquivos e extensões:
                List<ReferenciaArquivoUI> listaReferenciaArquivos = new List<ReferenciaArquivoUI>();
                for (int i = 0; i < nomesArquivos.Length; i++)
                {
                    ReferenciaArquivoUI refArquivo = new ReferenciaArquivoUI();
                    string[] pathArr = nomesArquivos[i].Split('\\');
                    refArquivo.id = pathArr.Last();
                    string[] fileArr = pathArr.Last().Split('.');
                    refArquivo.name = fileArr.First().ToString();
                    if(fileArr.Last().IndexOf("html") >= 0)  
                        listaReferenciaArquivos.Add(refArquivo);
                }
                retorno.retorno = listaReferenciaArquivos;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUploadImage, retorno, logger, ex);
            }
        }
        
        [HttpPost]
        [HttpComponentesAuthorize(Roles = "mailm.i,mailm.a")]
        [Obsolete]
        public HttpResponseMessage uploadImage()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];
                if (httpPostedFile == null)
                    throw new EmailMarketingBusinessException(Messages.msgImagemInvalida, null, EmailMarketingBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                var fileUpload = httpPostedFile;
                //limite  500kbytes
                string extensaoArq = fileUpload.FileName.Substring(fileUpload.FileName.Length - 4);
                if (fileUpload.ContentLength > 1000000)
                    throw new EmailMarketingBusinessException(Messages.msgErroImagemExcedeuLimte, null, EmailMarketingBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                if (extensaoArq.ToLower() != ".jpg" && extensaoArq.ToLower() != "jpeg" && extensaoArq.ToLower() != ".gif" && extensaoArq.ToLower() != ".png" && extensaoArq.ToLower() != ".gif")
                    throw new EmailMarketingBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtensaoImagemNaoSuportada, null, EmailMarketingBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                //Local onde vai ficar as fotos enviadas.
                var uploadPath = ConfigurationManager.AppSettings["caminhoUploads"];
                //var enderecoWeb = ConfigurationManager.AppSettings["enderecoWeb"];
                var serverUploadPath = Path.Combine(uploadPath, "MailMarketing");
                //Faz um checagem se o arquivo veio correto.

                string file_name = Utils.Utils.geradorNomeAleatorio(36);
                string[] nomes = fileUpload.FileName.Split('.');
                extensaoArq = nomes[nomes.Length - 1];
                string novo_nome = file_name + "." + extensaoArq;
                string uploadedFilePath = Path.Combine(serverUploadPath, novo_nome);

                //faz o upload literalmetne do arquivo.
                byte[] buffer;
                FileStream fs;
                using (fs = new FileStream(uploadedFilePath, FileMode.Create))
                {
                    buffer = new byte[fileUpload.InputStream.Length];
                    fileUpload.InputStream.Read(buffer, 0, buffer.Length);
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Dispose();
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObjectAsync(novo_nome).Result);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUploadImage, retorno, logger, ex);
            }
        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "mailm.i,mailm.a")]
        public HttpResponseMessage uploadArquivoAnexo()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                // arquivos vindos da página web.
                var fileUpload = HttpContext.Current.Request.Files;

                //Validações, para cada arquivo.
                for (int i = 0; i < fileUpload.Count; i++)
                {
                    if (Path.GetExtension(fileUpload[i].FileName) != ".pdf")
                        throw new EmailMarketingBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtensaoArqAnexoEmailMarketing, null,
                            EmailMarketingBusinessException.TipoErro.ERRO_EXTENSAO_ARQ_ANEXO, false);

                    if (fileUpload[i].ContentLength > 500000)
                        throw new EmailMarketingBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroLimiteArqAnexoExcedidoEmailMarketing, null, EmailMarketingBusinessException.TipoErro.ERRO_LIMITE_ARQ_ANEXO, false);
                }

                // variaveis
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                int codUsuario = this.ComponentesUser.CodUsuario;
                Dictionary<string, string> nome_arquivos_anexo = new Dictionary<string, string>();                

                // criar caminho aonde arquivos serão salvos.
                string caminho_arquivos_anexo = ConfigurationManager.AppSettings["caminhoUploads"];
                string pathArquivoAnexo = caminho_arquivos_anexo + "\\MailMarketing\\Anexos\\" + cd_escola;
                DirectoryInfo di = new DirectoryInfo(pathArquivoAnexo);
                if (!di.Exists) // se diretorio não existir, criar.
                    di.Create();

                for (int i = 0; i < fileUpload.Count; i++)
                {
                    string novo_nome_arquivo = Utils.Utils.geradorNomeAleatorio(36);
                    string nomeArquivo = novo_nome_arquivo + ".pdf";

                    if (fileUpload[i] != null && fileUpload[i].ContentLength > 0)
                    {
                        var path = Path.Combine(pathArquivoAnexo + "\\", nomeArquivo);

                        //salva o arquivo carregado.
                        fileUpload[i].SaveAs(path);

                        if (!nome_arquivos_anexo.ContainsKey(nomeArquivo)) // não permite inserir chaves repetidas no dicionário.
                            nome_arquivos_anexo.Add(nomeArquivo, fileUpload[i].FileName);
                        Thread.Sleep(3000); // aguarda 3 segundos, para criação do arquivo.
                    }
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(nome_arquivos_anexo, Formatting.Indented));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotUploadFile, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "mailm.i,mailm.a")]
        public HttpResponseMessage getHistDocumentos(int cd_mala_direta)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                var cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var path = Path.Combine(ConfigurationManager.AppSettings["caminhoUploads"] + "\\MailMarketing\\Anexos\\" + cd_escola + "\\" + cd_mala_direta);

                if (Directory.Exists(path))
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    FileInfo[] rgFiles = di.GetFiles("*.pdf");

                    var files = rgFiles.Select(fi => new 
                    {
                        fi.Name,
                        fi.Length,
                        fi.Extension,
                        cd_escola = cd_escola
                    });
                    return Request.CreateResponse(HttpStatusCode.OK, files);
                }

                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotHistoryFile, retorno, logger, ex);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DownloadArquivoAnexo(string nome_arquivo, int cd_mala_direta, int cd_escola)
        {
            if (!string.IsNullOrEmpty(nome_arquivo) && cd_mala_direta > 0)
            {
                string fullPath = Path.Combine(ConfigurationManager.AppSettings["caminhoUploads"] + "\\MailMarketing\\Anexos\\" + cd_escola + "\\" + cd_mala_direta + "\\" + nome_arquivo);
                if (File.Exists(fullPath))
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    var fileStream = new FileStream(fullPath, FileMode.Open);
                    response.Content = new StreamContent(fileStream);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = nome_arquivo;
                    return response;
                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [AllowAnonymous]
        public HttpResponseMessage getPhoto(string nome)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                var uploadPath = ConfigurationManager.AppSettings["caminhoUploads"];
                var uploadedFilePath = Path.Combine(Path.Combine(uploadPath, "MailMarketing"), nome);
                byte[] fileBytes = System.IO.File.ReadAllBytes(uploadedFilePath);
                //FileStream fileStream = new FileStream(uploadedFilePath, FileMode.Open, FileAccess.Read);
                //BinaryReader binaryReader = new BinaryReader(fileStream);
                //byte[] image = binaryReader.ReadBytes((int) fileStream.Length);
                //binaryReader.Close();
                //fileStream.Close();

                var mediaType = ManipuladorArquivo.dealImageExtensionById(0, nome);

                if (!string.IsNullOrEmpty(mediaType))
                    try
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, fileBytes, mediaType);
                    }
                    catch (Exception)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Foto não encontrada");
                        throw;
                    }
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound, "não existe foto para esse arquivo");
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "mailm")]
        [Obsolete]
        public HttpResponseMessage getListagemEnderecosEscola(string no_pessoa, int status, string email, byte id_tipo_cadastro)
        {
            try
            {
                var cd_escola = this.ComponentesUser.CodEmpresa.Value;
                IEmailMarketingBusiness emailMarketingBiz = (IEmailMarketingBusiness)base.instanciarBusiness<IEmailMarketingBusiness>();
                var listaEnderecos = emailMarketingBiz.getListagemEnderecos(cd_escola, no_pessoa, status, email, id_tipo_cadastro);
                var listaNaoInscritos = emailMarketingBiz.getListaNaoIncritoEnderecos(cd_escola, no_pessoa, status, email, id_tipo_cadastro);
                if (listaNaoInscritos != null && listaNaoInscritos.Count() > 0 && listaEnderecos != null)
                {
                    listaNaoInscritos = listaNaoInscritos.Where(l => listaEnderecos.Any(e => e.cd_cadastro == l.cd_cadastro && e.id_cadastro == l.id_cadastro)).ToList();
                    listaNaoInscritos = listaNaoInscritos.OrderBy(x => x.cd_cadastro);
                    foreach (ListaNaoInscrito l in listaNaoInscritos)
                        listaEnderecos.Where(e => e.cd_cadastro == l.cd_cadastro && e.id_cadastro == l.id_cadastro).FirstOrDefault().cd_lista_nao_inscrito = l.cd_lista_nao_inscrito;
                }
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObjectAsync(new { Enderecos = listaEnderecos, ListaNaoInscritos = listaNaoInscritos }).Result);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mailm")]
        [Obsolete]
        public HttpResponseMessage getListagemEnderecoMalaDireta(int cd_mala_direta)
        {
            try
            {
                var cd_escola = this.ComponentesUser.CodEmpresa.Value;
                IEmailMarketingBusiness emailMarketingBiz = (IEmailMarketingBusiness)base.instanciarBusiness<IEmailMarketingBusiness>();
                var listaEnderecos = emailMarketingBiz.getListagemEnderecosMalaDireta(cd_escola, cd_mala_direta);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObjectAsync(listaEnderecos).Result);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mailm.i,mailm.a")]
        public HttpResponseMessage visualizarEtiqueta(MalaDireta mala_direta)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                mala_direta.cd_escola = this.ComponentesUser.CodEmpresa.Value;
                if (!String.IsNullOrEmpty(mala_direta.dc_hr_inicial))
                    mala_direta.hh_inicial = TimeSpan.Parse(mala_direta.dc_hr_inicial);

                if (!String.IsNullOrEmpty(mala_direta.dc_hr_fim))
                    mala_direta.hh_final = TimeSpan.Parse(mala_direta.dc_hr_fim);

                if (mala_direta.dt_inicio_turma.HasValue)
                    mala_direta.dt_inicio_turma = mala_direta.dt_inicio_turma.Value.Date;
                if (mala_direta.dt_matricula.HasValue)
                    mala_direta.dt_matricula = mala_direta.dt_matricula.Value;

                IEmailMarketingBusiness emailMarketingBiz = (IEmailMarketingBusiness)base.instanciarBusiness<IEmailMarketingBusiness>();
                var etiqueta = emailMarketingBiz.visualizarEtiqueta(mala_direta);

                if (etiqueta.ListasEnderecoMala.Count == 0)
                    throw new ObjectNotFoundException(Messages.msgRegNotEnc);

                retorno.retorno = salvarMalaDiretaEtiqueta(etiqueta);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (ObjectNotFoundException ex)
            {
                return gerarLogException(Messages.msgRegNotEnc, new ReturnResult(), logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        private int salvarMalaDiretaEtiqueta(MalaDireta mala_direta)
        {
            try
            {
                IEmailMarketingBusiness BusinessEmailMarketing = (IEmailMarketingBusiness)base.instanciarBusiness<IEmailMarketingBusiness>();

                if (!String.IsNullOrEmpty(mala_direta.dc_hr_inicial))
                    mala_direta.hh_inicial = TimeSpan.Parse(mala_direta.dc_hr_inicial);

                if (!String.IsNullOrEmpty(mala_direta.dc_hr_fim))
                    mala_direta.hh_final = TimeSpan.Parse(mala_direta.dc_hr_fim);

                if (mala_direta.ListasEnderecoMala != null)
                {
                    foreach (var endereco in mala_direta.ListasEnderecoMala)
                    {
                        endereco.dc_email_cadastro = string.Empty;
                    }
                }

                mala_direta.cd_usuario = this.ComponentesUser.CodUsuario;
                int cd_mala_direta = BusinessEmailMarketing.salvarMalaDiretaEtiqueta(mala_direta);
                if (cd_mala_direta > 0)
                    return cd_mala_direta;
                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpComponentesAuthorize(Roles = "mailm.i,mailm.a")]
        public HttpResponseMessage postComporMensagem(MalaDireta mala_direta)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                mala_direta.cd_escola = this.ComponentesUser.CodEmpresa.Value;
                if(!String.IsNullOrEmpty(mala_direta.dc_hr_inicial))
                    mala_direta.hh_inicial = TimeSpan.Parse(mala_direta.dc_hr_inicial);

                if (!String.IsNullOrEmpty(mala_direta.dc_hr_fim))
                    mala_direta.hh_final = TimeSpan.Parse(mala_direta.dc_hr_fim);

                if (mala_direta.dt_inicio_turma.HasValue)
                    mala_direta.dt_inicio_turma = mala_direta.dt_inicio_turma.Value.Date;
                if (mala_direta.dt_matricula.HasValue)
                    mala_direta.dt_matricula = mala_direta.dt_matricula.Value;

                IEmailMarketingBusiness emailMarketingBiz = (IEmailMarketingBusiness)base.instanciarBusiness<IEmailMarketingBusiness>();
                retorno.retorno = emailMarketingBiz.postComporMensagem(mala_direta);
                
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mailm.i")]
        public HttpResponseMessage postAtualizarListaNaoInscrito(List<ListaNaoInscrito> listaEnderecos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                //configuraBusiness(new List<IGenericBusiness>() { emailMarketingBiz });
                int cd_pessoa_empresa = (int)this.ComponentesUser.CodEmpresa;
                IEmailMarketingBusiness emailMarketingBiz = (IEmailMarketingBusiness)base.instanciarBusiness<IEmailMarketingBusiness>();
                bool atualizado = emailMarketingBiz.crudListaNaoIncritoEndereco(cd_pessoa_empresa, listaEnderecos);
                retorno.retorno = atualizado;
                if (atualizado)
                    retorno.AddMensagem(Messages.msgAlteracoesSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgNotUpAllReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpAllReg, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "mailm")]
        public HttpResponseMessage searchHistoricoMalaDireta(string dc_assunto, string dta_historico, int id_tipo_mala)
        {
            try
            {
                DateTime? dtaHitorico = dta_historico == null ? null : (DateTime?)Convert.ToDateTime(dta_historico);
                IEmailMarketingBusiness emailMarketingBiz = (IEmailMarketingBusiness)base.instanciarBusiness<IEmailMarketingBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                if (dc_assunto == null)
                    dc_assunto = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = emailMarketingBiz.searchHistoricoMalaDireta(parametros, dc_assunto, dtaHitorico, cdEscola, id_tipo_mala);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mailm.a")]
        public HttpResponseMessage getMalaDiretaEComponentesEdit(int cd_mala_direta)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                IEmailMarketingBusiness emailMarketingBiz = (IEmailMarketingBusiness)base.instanciarBusiness<IEmailMarketingBusiness>();
                MalaDireta mala = emailMarketingBiz.getEditViewMalaDireta(cd_mala_direta, cdEscola);
                if (mala == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                retorno.retorno = mala;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mailm")]
        public HttpResponseMessage getMalaDiretaPorAluno(int cd_pessoa, string assunto, string dtaIni, string dtaFim)
            
        {
            try
            {
                var cd_escola = this.ComponentesUser.CodEmpresa.Value;
                DateTime? dtaInicio = dtaIni == null ? null : (DateTime?)Convert.ToDateTime(dtaIni);
                DateTime? dtaFinal = dtaFim == null ? null : (DateTime?)Convert.ToDateTime(dtaFim);
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                if (assunto == null)
                    assunto = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IEmailMarketingBusiness emailMarketingBiz = (IEmailMarketingBusiness)base.instanciarBusiness<IEmailMarketingBusiness>();
                IEnumerable<MalaDireta> listaMalaDireta = emailMarketingBiz.getMalaDiretaPorAluno(parametros, cd_pessoa, cd_escola, assunto, dtaInicio, dtaFinal);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, listaMalaDireta);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "rpla")]
        public HttpResponseMessage getUrlRelatorioListagemEnderecosMMK(int cd_mala_direta, string no_pessoa, int status, string email, byte id_tipo_cadastro)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                //int cd_mala_direta, int cd_empresa, string no_pessoa, int status, string email, byte id_tipo_cadastro
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cdEscola=" + cd_pessoa_escola + "&@cd_mala_direta=" + cd_mala_direta + "&@no_pessoa=" + no_pessoa + "&@status=" + status + 
                                    "&@email=" + email + "&@id_tipo_cadastro=" + id_tipo_cadastro;
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
    }
}