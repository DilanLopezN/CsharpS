using Componentes.GenericBusiness;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericController;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.InstituicaoEnsino.Model;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Controllers;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Business;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Log.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Log.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Web.UI.WebControls;
using FundacaoFisk.SGF.Utils;
using FundacaoFisk.SGF.Web.Services.Usuario.Business;

namespace FundacaoFisk.SGF.Services.InstituicaoEnsino.Controllers
{
    public class EscolaController : ComponentesApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(EscolaController));

        private ReturnResult retorno { get; set; }

        public EscolaController()
        {
        }
        //Retorna um boleano, se o usuario é master
        private bool retornaUserMaster()
        {
            IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
            int login = this.ComponentesUser.CodUsuario;
            var isMaster = BusinessEmpresa.VerificarMasterGeral(login);
            return isMaster;
        }

        #region Usuario

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getAllEscolaByUsuarioLogin()
        {
            ReturnResult retorno = new ReturnResult();
            //string login = Session["UserName"] + "";

            try
            {
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                string login = ComponentesUser.Identity.Name;
                List<Escola> ge = new List<Escola>();
                if(retornaUserMaster())
                    ge = BusinessEmpresa.findAllEmpresa().ToList();
                else
                    ge = BusinessEmpresa.findAllEmpresaByUsuario(login).ToList();
                retorno.retorno = ge;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }

        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getAllEmpresaAnterior(int cdEscola)
        {
            ReturnResult retorno = new ReturnResult();
            //string login = Session["UserName"] + "";

            try
            {
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                int login = this.ComponentesUser.CodUsuario;
                Escola empresa = new Escola();
                empresa.EmpresasAntigas = BusinessEmpresa.findAllEmpresaAnterior(login, retornaUserMaster(), cdEscola).ToList();
                empresa.isMasterGeral = retornaUserMaster();

                retorno.retorno = empresa;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }

        }
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getAllEmpresaColigada(int cdEscola)
        {
            ReturnResult retorno = new ReturnResult();
            //string login = Session["UserName"] + "";

            try
            {
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                List<Escola> empresa = new List<Escola>();
                empresa = BusinessEmpresa.findAllEmpresaColigada(cdEscola).ToList();

                retorno.retorno = empresa;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }

        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getParametrosByEscola()
        {
            ReturnResult retorno = new ReturnResult();
            //string login = Session["UserName"] + "";

            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int emp = (int)this.ComponentesUser.CodEmpresa;
                
                Parametro ge = new Parametro();
                ge = Business.getParametrosByEscola(emp);
                retorno.retorno = ge;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (EscolaBusinessException ex)
            {
                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_NIVEIS_PLANO_CONTAS)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                    else
                        return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getParametrosMatricula()
        {
            ReturnResult retorno = new ReturnResult();
            //string login = Session["UserName"] + "";

            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int emp = (int)this.ComponentesUser.CodEmpresa;

                Parametro ge = new Parametro();
                ge = Business.getParametrosMatricula(emp);
                retorno.retorno = ge;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getParametrosPlanoTxMatricula()
        {
            ReturnResult retorno = new ReturnResult();
            //string login = Session["UserName"] + "";

            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int emp = (int)this.ComponentesUser.CodEmpresa;

                Parametro ge = new Parametro();
                ge = Business.getParametrosPlanoTxMatricula(emp);
                retorno.retorno = ge;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getAllEscolaByUsuarioMasterLogin(string desc, int cdItem)
        {
            try
            {
                if (desc == null)
                {
                    desc = String.Empty;
                }
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                string login = ComponentesUser.Identity.Name;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = BusinessEmpresa.findAllEmpresaByUsuario(parametros, login, desc, cdItem);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "usu")]
        public HttpResponseMessage VerificarMasterGeral()
        {
            ReturnResult retorno = new ReturnResult();
            //string login = Session["UserName"] + "";
            string login = ComponentesUser.Identity.Name;
            try
            {
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                var ge = BusinessEmpresa.VerificarMasterGeral(login);
                retorno.retorno = ge;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "usu")]
        public HttpResponseMessage verificarSysAdmin()
        {
            ReturnResult retorno = new ReturnResult();
            string login = ComponentesUser.Identity.Name;
            try
            {
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                var ge = BusinessUsuario.verificarSysAdmin(login);
                retorno.retorno = ge;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "usu.e")]
        public HttpResponseMessage postDeleteUsuario(List<UsuarioWebSGF> usuariosWeb)
        {
            ReturnResult retorno = new ReturnResult();
            IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
            IPessoaBusiness BusinessPessoa = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
            IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
            IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
            configuraBusiness(new List<IGenericBusiness>() { Business, BusinessPessoa, BusinessUsuario, BusinessEmpresa });
            //string login = ComponentesUser.Identity.Name;
            try
            {
                int emp = (int)this.ComponentesUser.CodEmpresa;
                var delUsuario = Business.DeleteUsuario(usuariosWeb, emp);
                retorno.retorno = delUsuario;
                if (!delUsuario)
                {
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }


        [HttpGet]
        [HttpComponentesAuthorize(Roles = "usu")]
        [HttpComponentesAuthorize(Roles = "esc")]
        [HttpComponentesAuthorize(Roles = "pes")]
        [HttpComponentesAuthorize(Roles = "gru")]
        public HttpResponseMessage getAllDataUsuariobyId(int cdUsuario)
        {
            ReturnResult retorno = new ReturnResult();
            //string login = Session["UserName"] + "";
            //string login = ComponentesUser.Identity.Name;
            try
            {
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                ISecretariaBusiness businessSecre = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IApiAreaRestritaBusiness BusinessAreRestrita = (IApiAreaRestritaBusiness)base.instanciarBusiness<IApiAreaRestritaBusiness>();
                int emp = (int)this.ComponentesUser.CodEmpresa;
                var usuario = BusinessUsuario.getusuarioForEdit(cdUsuario);

                

                if (usuario != null && usuario.cd_usuario > 0 && usuario.id_area_resrtrita != null && BusinessAreRestrita.aplicaApiAreaRestrita() == true)
                {
                    TokenAreaRestritaUI token = BusinessAreRestrita.ObterToken( "GetDetalhesUsuario", usuario.id_area_resrtrita.ToString());

                    UserAreaRestritaDetalheRetorno userAreRestrita = BusinessAreRestrita.getDetalhesUsuario( token.access_token, usuario.id_area_resrtrita.ToString());

                    if (userAreRestrita != null)
                    {
                        usuario.menusAreaRestrita = userAreRestrita.user.menusConvertidos;
                        usuario.emailAreaRestrita = userAreRestrita.user.email;
                        usuario.nameAreaRestrita = userAreRestrita.user.name;
                        usuario.isAddedAreaRestrita = true;

                    }
                    else
                    {
                        usuario.isAddedAreaRestrita = false;
                    }

                     
                }

                if (usuario != null && usuario.cd_usuario > 0)
                    usuario.qtdHorarios = businessSecre.countHorariosUsuario(emp, usuario.cd_usuario);
                retorno.retorno = usuario;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception exe)
            {
                logger.Error(exe);
                retorno.AddMensagem(Messages.msgRegBuscError, exe.Message + exe.StackTrace + exe.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }

        }


        [HttpGet]
        [HttpComponentesAuthorize(Roles = "usu")]
        public HttpResponseMessage geHorarioByForUsuario(int cdUser)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness businessSecre = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                int emp = (int)this.ComponentesUser.CodEmpresa;
                List<Horario> horarios = businessSecre.getHorarioByEscolaForRegistro(emp, cdUser, Horario.Origem.USUARIO).ToList();
                foreach (Horario h in horarios)
                    h.calendar = "Calendar1";
                retorno.retorno = horarios;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception exe)
            {
                logger.Error(exe);
                retorno.AddMensagem(Messages.msgRegBuscError, exe.Message + exe.StackTrace + exe.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getParametrosMovimento()
        {
            ReturnResult retorno = new ReturnResult();
            //string login = Session["UserName"] + "";

            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int emp = (int)this.ComponentesUser.CodEmpresa;

                Parametro ge = new Parametro();
                ge = Business.getParametrosMovimento(emp);
                retorno.retorno = ge;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getNomeUsuario()
        {
            ReturnResult retorno = new ReturnResult();
            //string login = Session["UserName"] + "";

            try
            {
                string login = ComponentesUser.Identity.Name;
                retorno.retorno = login;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        
        #endregion

        #region Escola
        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getEscolaSearch(String desc, bool inicio, int status, string cnpj, string fantasia)
        {
            try {
                if(desc == null) 
                    desc = String.Empty;
                if(cnpj == null) 
                    cnpj = String.Empty;
                if(fantasia == null) 
                    fantasia = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                int login = this.ComponentesUser.CodUsuario;

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                var retorno = Business.getDescEscola(parametros, desc, inicio, getStatus(status), cnpj, fantasia, login);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getEscolaForEdit(int cd_pessoa_empresa)
        {
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                EscolaUI escola = new EscolaUI();               
                escola = Business.getEscolaForEdit(cd_pessoa_empresa);
                escola.localMovto = BusinessFinanceiro.getLocalMovtoByEscola(cd_pessoa_empresa, 0, false);
                var retorno = escola;

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage GetUrlRelatorioEscola(string sort, int direction, String desc, bool inicio, int status, string cnpj, string fantasia)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                desc = String.IsNullOrEmpty(desc) ? String.Empty : desc;
                cnpj = String.IsNullOrEmpty(cnpj) ? String.Empty : cnpj;
                fantasia = String.IsNullOrEmpty(fantasia) ? String.Empty : fantasia;

                int login = this.ComponentesUser.CodUsuario;

                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&@cnpjCpf=" + cnpj + "&@apelido=" + fantasia + "&@cdUsuario=" + login + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Escola&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.EscolaSearch;
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
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage GetUrlRelatorioLoginEscola(string dtAnalise, string hhAnalise, bool idLogin, byte idMatricula)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                int login = this.ComponentesUser.CodUsuario;
                if  (!BusinessEmpresa.VerificarMasterGeral(login))
                    throw new EscolaBusinessException("Opção Disponível apenas para Administradores do Sistema", null, 0, false);

                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@dtAnalise=" + dtAnalise + "&@hhAnalise=" + hhAnalise + "&@idLogin=" + idLogin + "&@idMatricula=" + idMatricula + "&" + 
                    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.RELATORIODEUSO;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                var parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [AllowAnonymous]
        public HttpResponseMessage GetPhoto(string nome)
        {
            try
            {
                var uploadPath = ConfigurationManager.AppSettings["caminhoUploads"];
                var uploadedFilePath = Path.Combine(uploadPath, nome);
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
        
        [AllowAnonymous]
        public HttpResponseMessage GetPhoto(int id)
        {
            IPessoaBusiness BusinessPessoa = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
            PessoaSGF pessoa = BusinessPessoa.getPessoaImage(id);
            var mediaType = ManipuladorArquivo.dealImageExtensionById(id, pessoa.ext_img_pessoa);
            var image = pessoa.img_pessoa;
            if (!string.IsNullOrEmpty(mediaType))
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, image, mediaType);
                }
                catch (Exception)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Foto não encontrada");
                    throw;
                }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "não existe foto para esse arquivo");
            }
        }

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getExistsEscolaOrCNPJ(String cnpj)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IPessoaBusiness BusinessPessoa = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var escola = BusinessEmpresa.existsEmpresaWithCNPJ(cnpj);
                if (escola != null)
                {
                    retorno.retorno = escola;
                    retorno.AddMensagem(string.Format(Messages.msgExisteCNPJEscola, escola.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                else
                {
                    var ExistPessoaJuridicaCnpjBase = BusinessPessoa.VerificarExisitsEmpresaByCnpjOrcdEmpresa(cnpj, null);
                    if (ExistPessoaJuridicaCnpjBase != null && ExistPessoaJuridicaCnpjBase.pessoaJuridica != null && ExistPessoaJuridicaCnpjBase.pessoaJuridica.cd_pessoa > 0)
                    {
                        retorno.retorno = ExistPessoaJuridicaCnpjBase;
                        retorno.AddMensagem(string.Format(Componentes.Utils.Messages.Messages.msgExistPersonCnpjBase, ExistPessoaJuridicaCnpjBase.pessoaJuridica.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    }
                }
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }
    
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getParametrosBaixa()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;
                var parametro = Business.getParametrosBaixa(cd_pessoa_escola);
                retorno.retorno = parametro;
                if (parametro.per_desconto_maximo > 0)
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

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getParametrosNiveisPlanoConta()
        {
            ReturnResult retorno = new ReturnResult();
            //string login = Session["UserName"] + "";

            try
            {
                int emp = (int)this.ComponentesUser.CodEmpresa;

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                Byte? param = Business.getParametroNiviesPlanoConta(emp);
                retorno.retorno = param;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (EscolaBusinessException ex)
            {
                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_NIVEIS_PLANO_CONTAS)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getEscolaSearchFK(String nome, string fantasia, bool inicio,  string cnpj)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;
                if (cnpj == null)
                    cnpj = String.Empty;
                if (fantasia == null)
                    fantasia = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                int login = this.ComponentesUser.CodUsuario;

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                var retorno = Business.getSearchEscolas(parametros, nome, cnpj, fantasia, inicio);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }


        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getEscolaNotWithItem(String nome, string fantasia, string cnpj, int cd_item, bool inicio)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;

                if (cnpj == null)
                    cnpj = String.Empty;
                
                if (fantasia == null)
                    fantasia = String.Empty;
                
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                var retorno = Business.getEscolaNotWithItem(parametros, nome, cnpj, fantasia, cd_item, inicio);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getEscolaNotWithKit(String nome, string cdEmpresas, string fantasia, string cnpj, int cd_item, bool inicio)
        {
            try
            {

                var empresas = new List<int>();
                if (!string.IsNullOrEmpty(cdEmpresas))
                {
                    string[] listEmpresas = cdEmpresas.Split(',');
                    foreach (var cd_empresa in listEmpresas)
                        if (!string.IsNullOrEmpty(cd_empresa))
                            empresas.Add(int.Parse(cd_empresa));
                }

                if (nome == null)
                    nome = String.Empty;

                if (cnpj == null)
                    cnpj = String.Empty;

                if (fantasia == null)
                    fantasia = String.Empty;

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                var retorno = Business.getEscolaNotWithKit(parametros, nome, empresas, cnpj, fantasia, cd_item, inicio);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getEscolatWithItem(int cd_item)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IEnumerable<PessoaSearchUI> escolas = Business.getEscolaHasItem(cd_item);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                retorno.retorno = escolas;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getTurmasEscolatWithTurma(int cd_turma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IEnumerable<TurmaEscolaSearchUI> escolas = Business.getTurmasEscolatWithTurma(cd_turma);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                retorno.retorno = escolas;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getAtividadeEscolatWithAtividade(int cd_atividade_extra)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IEnumerable<AtividadeEscolaAtividadeSearchUI> escolas = Business.getAtividadeEscolatWithAtividade(cd_atividade_extra);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                retorno.retorno = escolas;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getEscolatWithTpDesc(int cdTpDesc)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IEnumerable<PessoaSearchUI> escolas = Business.getEscolaHasTpDesc(cdTpDesc);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                retorno.retorno = escolas;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        #endregion

        #region Plano Contas
        /// <summary>
        /// Este método esta sendo usado aqui, por que arquiteturalmente não é possível ter depência de Instituição de ensino no Financeiro.
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpComponentesAuthorize(Roles = "plct")]
        public HttpResponseMessage getParametrosForPlanoConta()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                var cd_escola = (int)this.ComponentesUser.CodEmpresa;
                FinanceiroController financeiroController = new FinanceiroController();
                var parametro = Business.getParametrosByEscola(cd_escola);
                var retornar = financeiroController.getValuesForSearchDropDown(parametro.nm_niveis_plano_contas);
                retorno.retorno = retornar;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        #endregion

        #region Matricula
        //Estes metodos são utilizados aqui devido a arquitetura, para não gerar um referência circular
        [HttpComponentesAuthorize(Roles = "mat.i")]
        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        public HttpResponseMessage PostMatricula(Contrato contrato)
        {
            string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
            string pathContratosEscola = caminho_relatorios + "\\ContratosDigitalizados";
            string documentoContratoTemp = "";
            
            ReturnResult retorno = new ReturnResult();
            try
            {
               
                if (contrato.CursoContrato != null)
                {

                    int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                    int cdusuario = (int)this.ComponentesUser.CodUsuario;
                    int fusoHorario = this.ComponentesUser.IdFusoHorario;

                    IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                    configuraBusiness(new List<IGenericBusiness>() { Business });
                    contrato.cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                    //string caminho_relatorios = HttpContext.Current.Server.MapPath("caminhoUploads");
                   
                    if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado))
                        if (contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".png" &&
                            contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".pdf" &&
                            contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".jpg" &&
                            contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 5) != ".jpeg" &&
                            contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 5) != ".dotx")
                            throw new MatriculaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtesaoContratoDigitalizadoNaoValida, null,
                                MatriculaBusinessException.TipoErro.ERRO_EXTENSAO_CONTRATRO_DIGITALIZADO_NAO_VALIDA, false);


                    if (contrato.dt_matricula_contrato.HasValue)
                        contrato.dt_matricula_contrato = contrato.dt_matricula_contrato.Value.ToLocalTime().Date;
                    if (contrato.dt_inicial_contrato.HasValue)
                        contrato.dt_inicial_contrato = contrato.dt_inicial_contrato.Value.ToLocalTime().Date;
                    if (contrato.dt_final_contrato.HasValue)
                        contrato.dt_final_contrato = contrato.dt_final_contrato.Value.ToLocalTime().Date;
                    if (contrato.AlunoTurma != null && contrato.AlunoTurma.Count() > 0)
                        foreach (AlunoTurma at in contrato.AlunoTurma)
                        {
                            at.dt_matricula = at.dt_matricula.HasValue ? at.dt_matricula.Value.ToLocalTime().Date : at.dt_matricula;
                            at.dt_inicio = at.dt_inicio.HasValue ? at.dt_inicio.Value.ToLocalTime().Date : at.dt_inicio;
                        }
                    contrato.cd_usuario = (int)this.ComponentesUser.CodUsuario;
                    var cont = Business.PostMatricula(contrato, pathContratosEscola, cdusuario, fusoHorario);
                    if (cont.AlunoTurma != null && cont.AlunoTurma.Count() > 0)
                        foreach (AlunoTurma a in cont.AlunoTurma)
                        {
                            a.Turma = null;
                            a.Contrato = null;
                        }

                    retorno.retorno = cont;
                    if (cont.cd_contrato <= 0)
                        retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    else
                        retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                
                return response;
            }
            catch (MatriculaBusinessException e)
            {
                if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado_temporario))
                {
                    documentoContratoTemp = pathContratosEscola + "Temp" + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado_temporario;

                    if (System.IO.File.Exists(documentoContratoTemp))
                        System.IO.File.Delete(documentoContratoTemp);
                }

                if (e.tipoErro == MatriculaBusinessException.TipoErro.ERRO_MIN_DATE_MATRICULA ||
                    e.tipoErro == MatriculaBusinessException.TipoErro.ERRO_ALUNO_TURMA_CONTRATO ||
                    e.tipoErro == MatriculaBusinessException.TipoErro.ERRO_EXISTE_MAT_PRODUTO ||
                    e.tipoErro == MatriculaBusinessException.TipoErro.ERRO_APENAS_UM_ALUNO_TURMA_AGUARDANDO ||
                    e.tipoErro == MatriculaBusinessException.TipoErro.ERRO_VALOR_CONTRATO_ZERO)
                    return gerarLogException(e.Message, retorno, logger, e);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, e);
            }
                
            catch (Exception ex)
            {
                if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado_temporario))
                {
                    documentoContratoTemp = pathContratosEscola + "Temp" + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado_temporario;
                    if (System.IO.File.Exists(documentoContratoTemp))
                        System.IO.File.Delete(documentoContratoTemp);
                }
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }
        }

        [HttpGet, AllowAnonymous]
        public HttpResponseMessage getSendPromocaoAlunoMatricula(int cd_aluno, int cd_contrato, int id_tipo_matricula)
        {
            ReturnResult retorno = new ReturnResult();
            IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
            int cd_aluno_erro = 0;
            int cd_contrato_erro = 0;
            try
            {
                #region Pesquisa o aluno salvo e grava a promocao

                if (cd_aluno > 0)
                {

                    cd_aluno_erro = cd_aluno;
                    cd_contrato_erro = cd_contrato;
                    Business.enviaPromocaoAlunoMatricula(cd_aluno, cd_contrato, id_tipo_matricula);

                }

                #endregion

                return Request.CreateResponse(HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                #region Retorna Json com erro

                var msg = (ex.InnerException != null ? ex.InnerException.Message : ex.Message) + "cd_aluno_erro: " + cd_aluno_erro + "cd_contrato: " + cd_contrato_erro ;

                ExceptionHandler exceptionHandler = new ExceptionHandler(msg, "An erro has occurred.", ex.GetType().ToString(), ex.StackTrace);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, exceptionHandler);
                return response;

                #endregion

            }
        }


        [HttpComponentesAuthorize(Roles = "mat.a")]
        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        public HttpResponseMessage PostAlterarMatricula(Contrato contrato)
        {
            string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
            string pathContratosEscola = caminho_relatorios + "\\ContratosDigitalizados";
            string documentoContratoTemp = "";

            ReturnResult retorno = new ReturnResult();
            try
            {
                if (contrato.CursoContrato != null)
                {
                    int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                    int cdusuario = (int)this.ComponentesUser.CodUsuario;
                    int fusoHorario = (int)this.ComponentesUser.IdFusoHorario;

                    IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                    configuraBusiness(new List<IGenericBusiness>() { Business });
                    contrato.cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;

                    //string caminho_relatorios = HttpContext.Current.Server.MapPath("caminhoUploads");
                    if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado))
                        if (contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".png" &&
                            contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".pdf" &&
                            contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".jpg" &&
                            contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 5) != ".jpeg" &&
                            contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 5) != ".dotx")
                            throw new MatriculaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtesaoContratoDigitalizadoNaoValida, null,
                                MatriculaBusinessException.TipoErro.ERRO_EXTENSAO_CONTRATRO_DIGITALIZADO_NAO_VALIDA, false);


                    contrato.cd_usuario = (int)this.ComponentesUser.CodUsuario;

                    contrato.dt_matricula_contrato = contrato.dt_matricula_contrato.HasValue ? contrato.dt_matricula_contrato.Value.Date : contrato.dt_matricula_contrato;
                    contrato.dt_inicial_contrato = contrato.dt_inicial_contrato.HasValue ? contrato.dt_inicial_contrato.Value.Date : contrato.dt_inicial_contrato;
                    contrato.dt_final_contrato = contrato.dt_final_contrato.HasValue ? contrato.dt_final_contrato.Value.Date : contrato.dt_final_contrato;
                    if (contrato.AlunoTurma != null && contrato.AlunoTurma.Count() > 0)
                        foreach (AlunoTurma at in contrato.AlunoTurma)
                        {
                            at.dt_matricula = at.dt_matricula.HasValue ? at.dt_matricula.Value.Date : at.dt_matricula;
                            at.dt_inicio = at.dt_inicio.HasValue ? at.dt_inicio.Value.Date : at.dt_inicio;
                        }

                    var cont = Business.postAlterarMatricula(contrato, true, pathContratosEscola, cdusuario, fusoHorario);

                    retorno.retorno = cont;

                    if (cont.cd_contrato <= 0)
                        retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    else
                        retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado_temporario))
                {
                    documentoContratoTemp = pathContratosEscola + "Temp" + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado_temporario;

                    if (System.IO.File.Exists(documentoContratoTemp))
                        System.IO.File.Delete(documentoContratoTemp);
                }

                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_SALDO_TITULO)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (MatriculaBusinessException e)
            {
                if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado_temporario))
                {
                    documentoContratoTemp = pathContratosEscola + "Temp" + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado_temporario;

                    if (System.IO.File.Exists(documentoContratoTemp))
                        System.IO.File.Delete(documentoContratoTemp);
                }

                if (e.tipoErro == MatriculaBusinessException.TipoErro.ERRO_MIN_DATE_MATRICULA)
                    return gerarLogException(e.Message, retorno, logger, e);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, e);
            }
                
            catch (Exception ex)
            {
                if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado_temporario))
                {
                    documentoContratoTemp = pathContratosEscola + "Temp" + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado_temporario;

                    if (System.IO.File.Exists(documentoContratoTemp))
                        System.IO.File.Delete(documentoContratoTemp);
                }
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }
        }

        [HttpComponentesAuthorize(Roles = "mat.a")]
        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        public HttpResponseMessage PostAtualizarDocumentoDigitalizado(DocumentoDigitalizadoEditUI contrato)
        {
            string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
            string pathContratosEscola = caminho_relatorios + "\\ContratosDigitalizados";
            string documentoContratoTemp = "";

            ReturnResult retorno = new ReturnResult();
            try
            {
                if (contrato.cd_contrato > 0)
                {
                    int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                    int cdusuario = (int)this.ComponentesUser.CodUsuario;
                    IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                    configuraBusiness(new List<IGenericBusiness>() { Business });
                    contrato.cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;

                    //string caminho_relatorios = HttpContext.Current.Server.MapPath("caminhoUploads");
                    if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado))
                        if (contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".png" &&
                            contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".pdf" &&
                            contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".jpg" &&
                            contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 5) != ".jpeg" &&
                            contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 5) != ".dotx")
                            throw new MatriculaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtesaoContratoDigitalizadoNaoValida, null,
                                MatriculaBusinessException.TipoErro.ERRO_EXTENSAO_CONTRATRO_DIGITALIZADO_NAO_VALIDA, false);


                    

                    var cont = Business.postAtualizarDocumentoDigitalizado(contrato, pathContratosEscola);

                    retorno.retorno = cont;

                    if (cont.cd_contrato <= 0)
                        retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    else
                        retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado_temporario))
                {
                    documentoContratoTemp = pathContratosEscola + "Temp" + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado_temporario;

                    if (System.IO.File.Exists(documentoContratoTemp))
                        System.IO.File.Delete(documentoContratoTemp);
                }

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat.a")]
        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        public HttpResponseMessage PostAtualizarPacoteCertificado(PacoteCertificadoUI contrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (contrato.cd_contrato > 0)
                {
                    int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                    int cdusuario = (int)this.ComponentesUser.CodUsuario;
                    IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                    configuraBusiness(new List<IGenericBusiness>() { Business });

                    var cont = Business.postAtualizarPacoteCertificado(contrato);

                    retorno.retorno = cont;

                    if (cont.cd_contrato <= 0)
                        retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    else
                        retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        public HttpResponseMessage postGerarTitulosGrid(Contrato contrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (contrato != null && contrato.titulos.Count() > 0)
                    foreach (Titulo t in contrato.titulos)
                        t.cd_pessoa_empresa = this.ComponentesUser.CodEmpresa.Value;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                var tit = Business.gerarTitulosGrid(contrato);
                retorno.retorno = tit;
                if (tit.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_COM_BAIXA ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_LOCALMOVTO_CARTAO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        [HttpPost]
        public HttpResponseMessage alterarLocalMovtoTitulos(Contrato contrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();

                List<Titulo> titulos = contrato.titulos.ToList();
                int nm_parcelas_mensalidade = contrato.nm_parcelas_mensalidade;
                int cd_politica_comercial = contrato.cd_politica_comercial;
                int cd_pessoa_empresa = this.ComponentesUser.CodEmpresa.Value;

                var tit = Business.alterarLocalMovtoTitulos(titulos, nm_parcelas_mensalidade, cd_politica_comercial, cd_pessoa_empresa);
                retorno.retorno = tit;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        [HttpPost]
        public HttpResponseMessage postAlterarLocalMovtoTitulosNFFechada(List<Titulo> titulos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();


                int cd_pessoa_empresa = this.ComponentesUser.CodEmpresa.Value;

                var tit = Business.postAlterarLocalMovtoTitulosNFFechada(cd_pessoa_empresa, titulos);
                retorno.retorno = tit;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        [Obsolete]
        public HttpResponseMessage simuladorAditamento(Contrato contrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                contrato.titulos.FirstOrDefault().cd_pessoa_empresa = this.ComponentesUser.CodEmpresa.Value;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();

                if (contrato.id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                {
                    if (contrato.aditamentoMaxData.nm_titulos_aditamento == 0)
                        throw new MatriculaBusinessException(Messages.msgErroQtdTituloAditamentonaoInformado, null, MatriculaBusinessException.TipoErro.ERRO_VALIDACAO_ADITAMENTO, false);

                    if(contrato.aditamentoMaxData.dt_vcto_aditamento == null)
                        throw new MatriculaBusinessException(Messages.msgErroDtVctoAditamentoNaoInformado, null, MatriculaBusinessException.TipoErro.ERRO_VALIDACAO_ADITAMENTO, false);

                    if (contrato.aditamentoMaxData.vl_aditivo > 0 || contrato.aditamentoMaxData.vl_parcela_titulo_aditamento > 0)
                    {
                        if (contrato.aditamentoMaxData.vl_aditivo > 0 && contrato.aditamentoMaxData.vl_parcela_titulo_aditamento == 0)
                        {
                            if (contrato.valorSaldoMatricula != null && contrato.valorSaldoMatricula > 0)
                                contrato.aditamentoMaxData.vl_parcela_titulo_aditamento = ((contrato.aditamentoMaxData.vl_aditivo - (decimal)contrato.valorSaldoMatricula) / contrato.aditamentoMaxData.nm_titulos_aditamento);
                            else
                                contrato.aditamentoMaxData.vl_parcela_titulo_aditamento = (contrato.aditamentoMaxData.vl_aditivo / contrato.aditamentoMaxData.nm_titulos_aditamento);
                        }

                        contrato.titulos = Business.gerarTitulosAditamento(contrato);
                    }
                }

                if(!(contrato.id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADICIONAR_PARCELAS))
                    contrato.titulos = Business.gerarTitulosAditamento(contrato);
           
                var simulacaoAditamento = SimulacaoAditamentoUI.Simulador(contrato);
                retorno.retorno = simulacaoAditamento;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObjectAsync(retorno).Result);
                configureHeaderResponse(response, null);
                return response;

            }
            catch (MatriculaBusinessException ex)
            {
                if (ex.tipoErro == MatriculaBusinessException.TipoErro.ERRO_VALIDACAO_ADITAMENTO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        public HttpResponseMessage postGerarTitulosAditamento(Contrato contrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                contrato.titulos.FirstOrDefault().cd_pessoa_empresa = this.ComponentesUser.CodEmpresa.Value;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                var tit = Business.gerarTitulosAditamento(contrato);
                //var vl_total_titulos = Contrato.obterValorTotalAditamento(Business.gerarTitulosAditamento(contrato));

                retorno.retorno = tit;
                if (tit.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (MatriculaBusinessException ex)
            {
                if (ex.tipoErro == MatriculaBusinessException.TipoErro.ERRO_DIFERENCA_TITULOS_CURSO_MAIOR_UMREAL)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        [Obsolete]
        public HttpResponseMessage existeAdtAdicionarParcelaBaixado(Contrato contrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                Titulo tituloViewAdt = contrato.titulos.FirstOrDefault();
                tituloViewAdt.cd_pessoa_empresa = this.ComponentesUser.CodEmpresa.Value;

                bool existeTituloAditamentoBaixado = Business.existeAdtAdicionarParcelaBaixado(new List<Titulo>(), tituloViewAdt);

                if (existeTituloAditamentoBaixado)
                    throw new MatriculaBusinessException(Messages.msgExisteAditamentoBaixado, null, MatriculaBusinessException.TipoErro.ERRO_EXISTE_ADITAMENTO_BAIXO, false);

                retorno.retorno = existeTituloAditamentoBaixado;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObjectAsync(retorno).Result);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        [Obsolete]
        public HttpResponseMessage obterValorAditivo(Contrato contrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                contrato.titulos.FirstOrDefault().cd_pessoa_empresa = this.ComponentesUser.CodEmpresa.Value;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();

                contrato.titulos = Business.gerarTitulosAditamento(contrato);
                var simulacaoAditamento = SimulacaoAditamentoUI.Simulador(contrato);

                retorno.retorno = simulacaoAditamento;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObjectAsync(retorno).Result);
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        //Busca as sugestões de dia para opções de pagamento da matrícula, conforme o parâmetro da escola:
        [HttpComponentesAuthorize(Roles = "mat")]
        public HttpResponseMessage getSugestaoDiaOpcoesPgto(string str_data_matricula, int? cd_curso, int? cd_duracao, int? cd_regime)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                DateTime data_matricula = DateTime.Parse(str_data_matricula, new CultureInfo("pt-br", false));
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                OpcoesPagamentoUI opcoes = Business.getSugestaoDiaOpcoesPgto(cd_escola, data_matricula, cd_curso, cd_duracao, cd_regime);                
                retorno.retorno = opcoes;

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "reg")]
        [HttpComponentesAuthorize(Roles = "banc")]
        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpComponentesAuthorize(Roles = "nmCt")]
        [HttpComponentesAuthorize(Roles = "tprec")]
        public HttpResponseMessage getContratoTurma(string str_data_matricula, int cd_curso, int cd_duracao, int cd_produto,  int cd_regime)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                DateTime data_matricula = string.IsNullOrEmpty(str_data_matricula) ? DateTime.Now : DateTime.Parse(str_data_matricula, new CultureInfo("pt-br", false));
                int cd_escola = ComponentesUser.CodEmpresa.Value;

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                retorno.retorno = Business.getContratoTurma(cd_escola, cd_curso, cd_duracao, cd_produto, cd_regime, data_matricula);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat.a")]
        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        public HttpResponseMessage PostAlterarMatriculaALuno(Contrato contrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cdusuario = (int)this.ComponentesUser.CodUsuario;
                int fusoHorario = (int)this.ComponentesUser.IdFusoHorario;
                string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
                //string caminho_relatorios = HttpContext.Current.Server.MapPath("caminhoUploads");
                string pathContratosEscola = caminho_relatorios + "\\ContratosDigitalizados";
                if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado))
                    if (contrato.nm_arquivo_digitalizado.Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".png" &&
                        contrato.nm_arquivo_digitalizado.Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".pdf" &&
                        contrato.nm_arquivo_digitalizado.Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".jpg" &&
                        contrato.nm_arquivo_digitalizado.Substring(contrato.nm_arquivo_digitalizado.Length - 5) != ".jpeg" &&
                        contrato.nm_arquivo_digitalizado.Substring(contrato.nm_arquivo_digitalizado.Length - 5) != ".dotx")
                        throw new MatriculaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtesaoContratoDigitalizadoNaoValida, null,
                            MatriculaBusinessException.TipoErro.ERRO_EXTENSAO_CONTRATRO_DIGITALIZADO_NAO_VALIDA, false);

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business, BusinessFinanceiro });
                contrato.cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                contrato.cd_usuario = (int)this.ComponentesUser.CodUsuario;

                contrato.dt_matricula_contrato = contrato.dt_matricula_contrato.HasValue ? contrato.dt_matricula_contrato.Value.ToLocalTime().Date : contrato.dt_matricula_contrato;
                contrato.dt_inicial_contrato = contrato.dt_inicial_contrato.HasValue ? contrato.dt_inicial_contrato.Value.ToLocalTime().Date : contrato.dt_inicial_contrato;
                contrato.dt_final_contrato = contrato.dt_final_contrato.HasValue ? contrato.dt_final_contrato.Value.ToLocalTime().Date : contrato.dt_final_contrato;
                if (contrato.AlunoTurma != null && contrato.AlunoTurma.Count() > 0)
                    foreach (AlunoTurma at in contrato.AlunoTurma)
                    {
                        at.dt_matricula = at.dt_matricula.HasValue ? at.dt_matricula.Value.ToLocalTime().Date : at.dt_matricula;
                        at.dt_inicio = at.dt_inicio.HasValue ? at.dt_inicio.Value.ToLocalTime().Date : at.dt_inicio;
                    }

                var cont = Business.postAlterarMatricula(contrato,false, pathContratosEscola, cdusuario, fusoHorario);
                retorno.retorno = BusinessMatricula.getMatriculasAluno(contrato.cd_aluno, contrato.cd_pessoa_escola);

                if (cont.cd_contrato <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }
        }


        #endregion

        #region Baixa Financeira

        private HttpResponseMessage postSimularBaixaCnabOrDadosAdicTitulos(SimulacaoBaixaCnab simulacao, bool contaSegura)
        {
            ReturnResult retorno = new ReturnResult();
            IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
            IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
            var cd_escola = (int)this.ComponentesUser.CodEmpresa;
            int[] cdTitulos = null;
            List<TituloCnab> titulosCnab = new List<TituloCnab>();
            if (simulacao.titulos != null && simulacao.titulos.Count() > 0)
            {
                int i = 0;
                int[] cdTitulosCont = new int[simulacao.titulos.Count()];
                foreach (var c in simulacao.titulos)
                {
                    cdTitulosCont[i] = c.cd_titulo;
                    i++;
                }
                cdTitulos = cdTitulosCont;
                List<Titulo> titulos = BusinessFinanceiro.getDadosAdicionaisTituloParaCnab(cdTitulos, cd_escola).ToList();
                foreach (TituloCnab tc in simulacao.titulos)
                {
                    Titulo titulo = titulos.Where(x => x.cd_titulo == tc.cd_titulo).FirstOrDefault();
                    if (titulo != null)
                    {
                        tc.cd_turma_titulo = titulo.cd_turma_titulo;
                        tc.no_turma_titulo = titulo.no_turma_titulo;
                        tc.Titulo.nomeAluno = titulo.nomeAluno;
                        tc.Titulo.vl_saldo_titulo = titulo.vl_saldo_titulo;
                        if (simulacao.id_tipo_cnab == (int)Cnab.TipoCnab.PEDIDO_BAIXA)
                            tc.Titulo.vl_saldo_titulo = tc.Titulo.vl_titulo - titulo.vl_pago_bolsa;
                        tc.Titulo.id_origem_titulo = titulo.id_origem_titulo;
                        tc.Titulo.pc_multa_titulo = titulo.pc_multa_titulo;
                        tc.Titulo.pc_juros_titulo = titulo.pc_juros_titulo;
                        tc.Titulo.cd_origem_titulo = titulo.cd_origem_titulo;
                        tc.Titulo.cd_pessoa_responsavel = titulo.cd_pessoa_responsavel;
                        tc.cd_pessoa_titulo = titulo.cd_pessoa_responsavel;
                        tc.Titulo.cd_pessoa_titulo = titulo.cd_pessoa_titulo;
                        tc.cd_aluno = titulo.cd_aluno;
                        tc.cd_produto = titulo.cd_produto;
                        tc.nro_contrato = titulo.nm_contrato;
                        tc.id_status_cnab_titulo = titulo.id_status_cnab;
                        tc.Titulo.id_status_cnab = titulo.id_status_cnab;
                        tc.dc_nosso_numero_titulo = titulo.dc_nosso_numero;
                        tc.Titulo.dc_nosso_numero = titulo.dc_nosso_numero;
                        tc.Titulo.vl_material_titulo = titulo.vl_material_titulo;
                    }
                }
                if (simulacao.id_tipo_cnab == (int)Cnab.TipoCnab.GERAR_BOLETOS)
                {
                    DateTime data_baixa = String.IsNullOrEmpty(simulacao.dataBaixa) ? DateTime.Now.Date : DateTime.Parse(simulacao.dataBaixa).Date;
                    titulosCnab = Business.simularBaixaTituloCnab(simulacao.titulos.ToList(), data_baixa, cd_escola, contaSegura);
                }
                else
                    titulosCnab = simulacao.titulos.ToList();
            }

            retorno.retorno = titulosCnab;
            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
            return response;
        }

        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage postSimularBaixaCnabOrDadosAdicTitulos(SimulacaoBaixaCnab simulacao) {
            ReturnResult retorno = new ReturnResult();
            try {
                if (simulacao.titulos != null && simulacao.titulos.Count() > 0)
                    foreach (TituloCnab tituloCnab in simulacao.titulos)
                    {
                        //tituloCnab.dt_vencimento_titulo = SGF.Utils.ConversorUTC.ToLocalTime(tituloCnab.dt_vencimento_titulo.Value, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                        //tituloCnab.Titulo.dt_vcto_titulo = SGF.Utils.ConversorUTC.ToLocalTime(tituloCnab.Titulo.dt_vcto_titulo, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                        tituloCnab.dt_vencimento_titulo = tituloCnab.dt_vencimento_titulo.Value.Date;
                        tituloCnab.Titulo.dt_vcto_titulo = tituloCnab.Titulo.dt_vcto_titulo.Date;
                    }

                return this.postSimularBaixaCnabOrDadosAdicTitulos(simulacao, false);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //[HttpComponentesAuthorize(Roles = "ctsg")]
        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage postSimularBaixaCnabOrDadosAdicTitulosGeral(SimulacaoBaixaCnab simulacao) {
            ReturnResult retorno = new ReturnResult();
            try {
                if (simulacao.titulos != null && simulacao.titulos.Count() > 0)
                    foreach (TituloCnab tituloCnab in simulacao.titulos)
                    {
                        //tituloCnab.dt_vencimento_titulo = SGF.Utils.ConversorUTC.ToLocalTime(tituloCnab.dt_vencimento_titulo.Value, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                        //tituloCnab.Titulo.dt_vcto_titulo = SGF.Utils.ConversorUTC.ToLocalTime(tituloCnab.Titulo.dt_vcto_titulo, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                        tituloCnab.dt_vencimento_titulo = tituloCnab.dt_vencimento_titulo.Value.Date;
                        tituloCnab.Titulo.dt_vcto_titulo = tituloCnab.Titulo.dt_vcto_titulo.Date;
                    }
                return this.postSimularBaixaCnabOrDadosAdicTitulos(simulacao, true); 
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage postSimularBaixaTitulos(SimulacaoBaixa simulacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                aplicaRegrasTituloTipoCartao(simulacao.titulos);

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IFinanceiroBusiness BusinessFina = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var cd_escola = (int)this.ComponentesUser.CodEmpresa;
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;
                DateTime data_baixa = String.IsNullOrEmpty(simulacao.dataBaixa) ? DateTime.Now.Date : DateTime.Parse(simulacao.dataBaixa).Date;
                List<BaixaTitulo> baixas = Business.simularBaixaTituloLeitura(simulacao.titulos.ToList(), data_baixa, cd_escola, false);
                List<TipoLiquidacao> tipoLiquidacoes =  BusinessFina.getTipoLiquidacao(TipoLiquidacaoDataAccess.TipoConsultaTipoLiquidacaoEnum.HAS_ATIVO, null).ToList();
                List<LocalMovto> locais = BusinessFina.getLocalMovimentoSomenteLeitura(cd_escola, 0, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_SIMULACAO_BAIXA, cod_pessoa_usuario).ToList();
                List<Banco> bancos = BusinessFina.getAllBanco().ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Baixas = baixas,
                    LocalMovto = locais,
                    TipoLiquidacoes = tipoLiquidacoes,
                    Bancos = bancos,
                    cd_local_movto = baixas.First().Titulo.cd_local_movto,
                    nm_tipo_local = baixas.First().Titulo.nm_tipo_local,
                    cd_tipo_financeiro = baixas.First().Titulo.cd_tipo_financeiro,
                    cd_local_banco = baixas.First().Titulo.cd_local_banco
                });

                return response;
            }
            catch (EscolaBusinessException ex)
            {
                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_BAIXA_CARTAO_TIPO_FINANCEIRO ||
                    ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_BAIXA_CARTAO_LOCALMOVT_DIFERENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //[HttpComponentesAuthorize(Roles = "ctsg")]
        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage postSimularBaixaTitulosGeral(SimulacaoBaixa simulacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                aplicaRegrasTituloTipoCartao(simulacao.titulos);

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IFinanceiroBusiness BusinessFinan = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var cd_escola = (int)this.ComponentesUser.CodEmpresa;
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;
                DateTime data_baixa = String.IsNullOrEmpty(simulacao.dataBaixa) ? DateTime.Now.Date : DateTime.Parse(simulacao.dataBaixa).Date;
                List<BaixaTitulo> baixas = Business.simularBaixaTituloLeitura(simulacao.titulos.ToList(), data_baixa, cd_escola, true);
                List<TipoLiquidacao> tipoLiquidacoes = BusinessFinan.getTipoLiquidacao(TipoLiquidacaoDataAccess.TipoConsultaTipoLiquidacaoEnum.HAS_ATIVO, null).ToList();
                List<LocalMovto> locais = BusinessFinan.getLocalMovimentoSomenteLeitura(cd_escola, 0, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_SIMULACAO_BAIXA, cod_pessoa_usuario).ToList();
                List<Banco> bancos = BusinessFinan.getAllBanco().ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, new 
                { 
                    Baixas = baixas, 
                    LocalMovto = locais, 
                    TipoLiquidacoes = tipoLiquidacoes, 
                    Bancos = bancos,
                    cd_local_movto = baixas.First().Titulo.cd_local_movto,
                    nm_tipo_local = baixas.First().Titulo.nm_tipo_local,
                    cd_tipo_financeiro = baixas.First().Titulo.cd_tipo_financeiro,
                    cd_local_banco = baixas.First().Titulo.cd_local_banco
                });
                return response;
            }
            catch (EscolaBusinessException ex)
            {
                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_BAIXA_CARTAO_TIPO_FINANCEIRO ||
                    ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_BAIXA_CARTAO_LOCALMOVT_DIFERENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage loadChequeTrocaFinanceira(int cd_titulo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IFinanceiroBusiness BusinessFinan = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var cd_escola = (int)this.ComponentesUser.CodEmpresa;
                Cheque cheque = BusinessFinan.getChequeTransacaoTrocaFinanceira(cd_titulo, cd_escola);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, cheque);
                return response;
            }
            catch (EscolaBusinessException ex)
            {
                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_BAIXA_CARTAO_TIPO_FINANCEIRO ||
                    ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_BAIXA_CARTAO_LOCALMOVT_DIFERENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //[HttpComponentesAuthorize(Roles = "ctsg")]
        [HttpComponentesAuthorize(Roles = "bfinan")]
        [HttpGet]
        public HttpResponseMessage getLocaisMovimentoGeral()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IFinanceiroBusiness BusinessFinan = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var cd_escola = (int)this.ComponentesUser.CodEmpresa;
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;
                List<LocalMovto> locais = BusinessFinan.getLocalMovimentoSomenteLeitura(cd_escola, 0, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_SIMULACAO_BAIXA_GERAL_SEM_CARTAO, cod_pessoa_usuario).ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    LocalMovto = locais
                });
                return response;
            }
            catch (EscolaBusinessException ex)
            {
                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_BAIXA_CARTAO_TIPO_FINANCEIRO ||
                    ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_BAIXA_CARTAO_LOCALMOVT_DIFERENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //[HttpComponentesAuthorize(Roles = "ctsg")]
        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage validaTitulosTipoFinanceiroCartao(ICollection<Titulo> titulos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                aplicaRegrasTituloTipoCartao(titulos);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, new { Valido = true });
                return response;
            }
            catch (EscolaBusinessException ex)
            {
                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_BAIXA_CARTAO_TIPO_FINANCEIRO ||
                    ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_BAIXA_CARTAO_LOCALMOVT_DIFERENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        private void aplicaRegrasTituloTipoCartao(ICollection<Titulo> titulos)
        {
            int qtd_total_titulos = titulos.Count;
            IEnumerable<Titulo> titulos_cartao = titulos.Where(t => t.cd_tipo_financeiro == (int)FundacaoFisk.SGF.GenericModel.TipoFinanceiro.TiposFinanceiro.CARTAO);
            
            if (titulos_cartao.Count() > 0)
            {
                //Baixas com titulos tipo CARTÃO não podem ser lançadas com outros tipos.
                // Para isso a regra diz que a quantidade de titulos vindos do filtro deve
                // ser igual a quantidade total de titulo.
                if (titulos_cartao.Count() != qtd_total_titulos)
                    throw new EscolaBusinessException(Utils.Messages.Messages.msgErroBaixaCartaoTipoFinanceiro,
                    null, EscolaBusinessException.TipoErro.ERRO_BAIXA_CARTAO_TIPO_FINANCEIRO, false);

                //Titulos com tipo CARTÃO devem ser todos do mesmo local de movimento.
                Titulo titulo = titulos_cartao.FirstOrDefault();
                if (titulos_cartao.Count(t => t.cd_local_movto == titulo.cd_local_movto) != qtd_total_titulos)
                    throw new EscolaBusinessException(Utils.Messages.Messages.msgErroBaixaCartaoLocalMovtDiferente,
                    null, EscolaBusinessException.TipoErro.ERRO_BAIXA_CARTAO_LOCALMOVT_DIFERENTE, false);
            }
        }
        
        #endregion

        #region Turma
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage postTodosHorariosVinculosTurma(PesquisaHorarioTurma pesquisaHorarioTurma)
        {
            ReturnResult retorno = new ReturnResult();
            var cd_escola = (int)this.ComponentesUser.CodEmpresa;
            TipoHorarios tipohorarios = new TipoHorarios();
            try
            {
                //if (cd_turma != null && cd_turma > 0)
                //    tipohorarios.horarioOcupTurma = businessSecre.getHorarioByEscolaForRegistro(cd_escola, (int)cd_turma, Horario.Origem.TURMA).ToList();
                ISecretariaBusiness businessSecre = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                if (pesquisaHorarioTurma.cd_sala > 0)
                {
                    tipohorarios.horairosOcupSala = businessSecre.getHorarioOcupadosForTurma(cd_escola, (int)pesquisaHorarioTurma.cd_sala, null, (int)pesquisaHorarioTurma.cd_turma,
                                                                                             pesquisaHorarioTurma.cd_duracao, pesquisaHorarioTurma.cd_curso, pesquisaHorarioTurma.dt_inicio,
                                                                                             pesquisaHorarioTurma.dt_final, HorarioDataAccess.TipoConsultaHorario.HAS_HORARIO_SALA_OCUPADO_TURMA).ToList();
                    var hOcupadosSalaAtivExt = businessSecre.getHorarioOcupadosForTurma(cd_escola, (int)pesquisaHorarioTurma.cd_sala, null, 0,
                                                                                        pesquisaHorarioTurma.cd_duracao, pesquisaHorarioTurma.cd_curso, pesquisaHorarioTurma.dt_inicio,
                                                                                        pesquisaHorarioTurma.dt_final, HorarioDataAccess.TipoConsultaHorario.HAS_HORARIO_SALA_ATIVIDA_EXT).ToList();
                    foreach (Horario h in hOcupadosSalaAtivExt)
                        tipohorarios.horairosOcupSala.Add(h);
                }
                if (pesquisaHorarioTurma.professores != null && pesquisaHorarioTurma.professores.Count() > 0)
                {
                    tipohorarios.horariosOcupProf = businessSecre.getHorarioOcupadosForTurma(cd_escola, (int)pesquisaHorarioTurma.cd_turma, pesquisaHorarioTurma.professores, 
                                                                                             (int)pesquisaHorarioTurma.cd_turma,
                                                                                             pesquisaHorarioTurma.cd_duracao, pesquisaHorarioTurma.cd_curso, pesquisaHorarioTurma.dt_inicio,
                                                                                             pesquisaHorarioTurma.dt_final, HorarioDataAccess.TipoConsultaHorario.HAS_HORARIO_PROF_OCUPADO_TURMA).ToList();
                    var hOcupadosProfAtivExt = businessSecre.getHorarioOcupadosForTurma(cd_escola, (int)pesquisaHorarioTurma.cd_turma, pesquisaHorarioTurma.professores, 0,
                                                                                        pesquisaHorarioTurma.cd_duracao, pesquisaHorarioTurma.cd_curso, pesquisaHorarioTurma.dt_inicio,
                                                                                        pesquisaHorarioTurma.dt_final, HorarioDataAccess.TipoConsultaHorario.HAS_HORARIO_PROF_OCUPADO_ATIV_EXTRA).ToList();
                    //*MMC tipohorarios.horariosDispoProf = businessSecre.getHorarioByEscolaForRegistro(cd_escola, (int)cd_professor ,Horario.Origem.PROFESSOR).ToList();
                    foreach (Horario h in hOcupadosProfAtivExt)
                        tipohorarios.horariosOcupProf.Add(h);
                }
                retorno.retorno = tipohorarios;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage postTodosHorariosVinculosTurmaPPTFilha(PesquisaHorarioTurma pesquisaHorarioTurma)
        {
            ReturnResult retorno = new ReturnResult();
            var cd_escola = (int)this.ComponentesUser.CodEmpresa;
            TipoHorarios tipohorarios = new TipoHorarios();
            try
            {
                ISecretariaBusiness businessSecre = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                if (pesquisaHorarioTurma.cd_turma > 0)
                    tipohorarios.horarioOcupTurma = businessSecre.getHorarioByEscolaForRegistro(cd_escola, pesquisaHorarioTurma.cd_turma, Horario.Origem.TURMA).ToList();
                if (pesquisaHorarioTurma.cd_turma_PPT > 0)
                    tipohorarios.horarioOcupTurmaPPT = businessSecre.getHorarioByEscolaForRegistro(cd_escola, pesquisaHorarioTurma.cd_turma_PPT, Horario.Origem.TURMA).ToList();
                retorno.retorno = tipohorarios;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        #endregion

        #region Log

        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage getLogGeralBaixaTitulo(int cd_baixa_titulo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                SGFWebContext dbc = new SGFWebContext();
                ILogGeralBusiness BusinessLogGeral = (ILogGeralBusiness)base.instanciarBusiness<ILogGeralBusiness>();
                int cd_origem = Int32.Parse(dbc.LISTA_ORIGEM_LOGS["BaixaTitulo"].ToString());
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IEnumerable<LogGeral> logGeral = BusinessLogGeral.getLogGeralBaixaTitulo(cd_baixa_titulo, cdEscola, cd_origem);
                List<Atributos> atributos = BusinessLogGeral.getAtributosByOrigem(cd_origem).ToList();
                if (logGeral.Count() > 0)
                {
                    int id = 0;
                    List<LogDetalheUI> log = new List<LogDetalheUI>();
                    List<LogDetalheUI> logDetalhe = new List<LogDetalheUI>();
                    foreach (LogGeral lg in logGeral)
                    {
                        id++;
                        LogDetalheUI l = new LogDetalheUI();
                        l.dt_historico = ((DateTime)lg.dt_log_geral).ToLocalTime();
                        l.descricao = lg.no_login;
                        l.dc_tipo_log = lg.dc_tipo_log;
                        l.id = id;
                        if (lg.DetalhesLogGeral != null && lg.DetalhesLogGeral.Count > 0)
                        {
                            int idPai = id;
                            logDetalhe = new List<LogDetalheUI>();
                            foreach (LogGeralDetalhe lgd in lg.DetalhesLogGeral)
                            {
                                id++;
                                LogDetalheUI lDetalhe = new LogDetalheUI();
                                lDetalhe.pai = idPai;
                                lDetalhe.id = id;
                                lDetalhe.dt_historico = l.dt_historico;
                                lDetalhe.descricao = atributos.Where(x => x.no_coluna == lgd.no_coluna).Any() ?
                                      atributos.Where(x => x.no_coluna == lgd.no_coluna).FirstOrDefault().no_logico : lgd.no_coluna;
                                lDetalhe.dc_valor_antigo = lgd.dc_valor_antigo;
                                lDetalhe.dc_valor_novo = lgd.dc_valor_novo;
                                logDetalhe.Add(lDetalhe);
                            }
                            l.children = logDetalhe;
                        }
                        log.Add(l);
                    }
                    retorno.retorno = log;
                }
                //retorno.retorno = logGeral;
                if (logGeral.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage getLogGeralTitulo(int cd_titulo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                SGFWebContext dbc = new SGFWebContext();
                ILogGeralBusiness BusinessLogGeral = (ILogGeralBusiness)base.instanciarBusiness<ILogGeralBusiness>();
                int cd_origem = Int32.Parse(dbc.LISTA_ORIGEM_LOGS["Titulo"].ToString());
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IEnumerable<LogGeral> logGeral = BusinessLogGeral.getLogGeralBaixaTitulo(cd_titulo, cdEscola, cd_origem);
                List<Atributos> atributos = BusinessLogGeral.getAtributosByOrigem(cd_origem).ToList();
                if (logGeral.Count() > 0)
                {
                    int id = 0;
                    List<LogDetalheUI> log = new List<LogDetalheUI>();
                    List<LogDetalheUI> logDetalhe = new List<LogDetalheUI>();
                    foreach (LogGeral lg in logGeral)
                    {
                        id++;
                        LogDetalheUI l = new LogDetalheUI();
                        l.dt_historico = ((DateTime)lg.dt_log_geral).ToLocalTime();
                        l.dc_tipo_log = lg.dc_tipo_log;
                        l.descricao = lg.no_login;
                        l.id = id;
                        if (lg.DetalhesLogGeral != null && lg.DetalhesLogGeral.Count > 0)
                        {

                            int idPai = id;
                            logDetalhe = new List<LogDetalheUI>();
                            foreach (LogGeralDetalhe lgd in lg.DetalhesLogGeral)
                            {
                                string stringNoLogico = "";
                                stringNoLogico = lgd.no_coluna;
                                Atributos atributo = atributos.Where(x => x.no_coluna == lgd.no_coluna).FirstOrDefault();
                                if (atributo != null)
                                    stringNoLogico = atributo.no_logico;
                                id++;
                                LogDetalheUI lDetalhe = new LogDetalheUI();
                                lDetalhe.pai = idPai;
                                lDetalhe.id = id;
                                lDetalhe.dt_historico = l.dt_historico;
                                lDetalhe.descricao = stringNoLogico;
                                lDetalhe.dc_valor_antigo = lgd.dc_valor_antigo;
                                lDetalhe.dc_valor_novo = lgd.dc_valor_novo;
                                logDetalhe.Add(lDetalhe);
                            }
                            l.children = logDetalhe;
                        }
                        log.Add(l);
                    }
                    retorno.retorno = log;
                }
               // retorno.retorno = logGeral;
                if (logGeral.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "coco")]
        public HttpResponseMessage getLogGeralContaCorrente(int cd_conta_corrente)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                SGFWebContext dbc = new SGFWebContext();
                ILogGeralBusiness BusinessLogGeral = (ILogGeralBusiness)base.instanciarBusiness<ILogGeralBusiness>();
                int cd_origem = Int32.Parse(dbc.LISTA_ORIGEM_LOGS["ContaCorrente"].ToString());
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IEnumerable<LogGeral> logGeral = BusinessLogGeral.getLogGeralBaixaTitulo(cd_conta_corrente, cdEscola, cd_origem);
                List<Atributos> atributos = BusinessLogGeral.getAtributosByOrigem(cd_origem).ToList();
                if (logGeral.Count() > 0)
                {
                    int id = 0;
                    List<LogDetalheUI> log = new List<LogDetalheUI>();
                    List<LogDetalheUI> logDetalhe = new List<LogDetalheUI>();
                    foreach (LogGeral lg in logGeral)
                    {
                        id++;
                        LogDetalheUI l = new LogDetalheUI();
                        l.dt_historico = ((DateTime)lg.dt_log_geral).ToLocalTime();
                        l.dc_tipo_log = lg.dc_tipo_log;
                        l.descricao = lg.no_login;
                        l.id = id;
                        if (lg.DetalhesLogGeral != null && lg.DetalhesLogGeral.Count > 0)
                        {

                            int idPai = id;
                            logDetalhe = new List<LogDetalheUI>();
                            foreach (LogGeralDetalhe lgd in lg.DetalhesLogGeral)
                            {
                                string stringNoLogico = "";
                                stringNoLogico = lgd.no_coluna;
                                Atributos atributo = atributos.Where(x => x.no_coluna == lgd.no_coluna).FirstOrDefault();
                                if (atributo != null)
                                    stringNoLogico = atributo.no_logico;
                                id++;
                                LogDetalheUI lDetalhe = new LogDetalheUI();
                                lDetalhe.pai = idPai;
                                lDetalhe.id = id;
                                lDetalhe.dt_historico = l.dt_historico;
                                lDetalhe.descricao = stringNoLogico;
                                lDetalhe.dc_valor_antigo = lgd.dc_valor_antigo;
                                lDetalhe.dc_valor_novo = lgd.dc_valor_novo;
                                logDetalhe.Add(lDetalhe);
                            }
                            l.children = logDetalhe;
                        }
                        log.Add(l);
                    }
                    retorno.retorno = log;
                }
                // retorno.retorno = logGeral;
                if (logGeral.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region  Transação Financeira
        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        [HttpComponentesAuthorize(Roles = "bfinan.i")]
        public HttpResponseMessage postIncluirTransacao(TransacaoFinanceira transacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                if(transacao.dt_tran_finan.HasValue)
                    transacao.dt_tran_finan = transacao.dt_tran_finan.Value.ToLocalTime().Date;

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business, BusinessFinanceiro, BusinessUsuario });
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                int CodUsuario = this.ComponentesUser.CodUsuario;

                transacao.isSupervisor = BusinessUsuario.VerificarSupervisorByEscola(CodUsuario, cd_escola);
                transacao.cd_pessoa_empresa = cd_escola;
                foreach (var bt in transacao.Baixas)
                {
                    bt.cd_usuario = CodUsuario;
                }

                transacao = Business.postIncluirTransacao(transacao);
                if (transacao.cheque != null)
                    transacao.cheque = null;
                if (transacao.cd_tran_finan <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                transacao.ChequeTransacaoFinanceira = null;
                foreach (BaixaTitulo bt in transacao.Baixas)
                {
                    if (bt.Titulo != null && bt.Titulo.BaixaTitulo != null)
                        bt.Titulo.BaixaTitulo = null;
                    if (bt.TransacaoFinanceira != null)
                        bt.TransacaoFinanceira = null;
                    if (bt.ContaCorrente != null)
                        bt.ContaCorrente = null;
                    if (bt.ChequeBaixa != null)
                        bt.ChequeBaixa = null;
                }
                retorno.retorno = transacao;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_ESCOLA_NOT_EXIST_LOCALMOVTO || 
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_ANTERIOR_ABERTO ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_CALCULO_APLICAR_BAIXA_TITULO ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_BAIXA_BOLSA_CONTRATO ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_DADOS_CHEQUE ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_PLANO_TAXBANC_NULL)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }
        }

        [HttpComponentesAuthorize(Roles = "tit.i, tit.a, tit.e")]
        [HttpComponentesAuthorize(Roles = "bfinan.a")]
        public HttpResponseMessage postUpdateTransacao(TransacaoFinanceira transacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business,  BusinessFinanceiro, BusinessUsuario  });
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                transacao.cd_pessoa_empresa = cd_escola;

                int CodUsuario = this.ComponentesUser.CodUsuario;

                transacao.isSupervisor = BusinessUsuario.VerificarSupervisorByEscola(CodUsuario, cd_escola);
                transacao.movimentoRetroativo = Business.getParametroMovimentoRetroativo(cd_escola);

                if (transacao.Baixas.Count() <= 0)
                {
                    var delTransFinan = BusinessFinanceiro.deleteTransFinanBaixa(transacao);
                    retorno.retorno = delTransFinan;
                    if (!delTransFinan)
                        retorno.AddMensagem(Messages.msgNotDeletedTrans, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    else
                        retorno.AddMensagem(Messages.msgDeleteSucessTrans, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {

                    transacao = Business.editTransacao(transacao);
                    if (transacao.cd_tran_finan <= 0)
                        retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    else
                        retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                retorno.retorno = transacao;
                if (transacao.cheque != null)
                    transacao.cheque = null;
                transacao.ChequeTransacaoFinanceira = null;
                foreach (BaixaTitulo bt in transacao.Baixas)
                {
                    if (bt.Titulo != null && bt.Titulo.BaixaTitulo != null)
                        bt.Titulo.BaixaTitulo = null;
                    if (bt.TransacaoFinanceira != null)
                        bt.TransacaoFinanceira = null;
                    if (bt.ContaCorrente != null)
                        bt.ContaCorrente = null;
                    if (bt.ChequeBaixa != null)
                        bt.ChequeBaixa = null;
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_ESCOLA_NOT_EXIST_LOCALMOVTO ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_ANTERIOR_ABERTO ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_CALCULO_APLICAR_BAIXA_TITULO ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_BAIXA_BOLSA_CONTRATO ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_DADOS_CHEQUE)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }
        }

        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        [HttpComponentesAuthorize(Roles = "bfinan.a")]
        public HttpResponseMessage postUpdateTransacaoReturnTitulos(TransacaoFinanceira transacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business, BusinessFinanceiro, BusinessUsuario });
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                transacao.cd_pessoa_empresa = cd_escola;
                TransacaoFinanceira trasacaoFinan = new TransacaoFinanceira();
                trasacaoFinan.cd_movimento = transacao.cd_movimento;
                int CodUsuario = this.ComponentesUser.CodUsuario;
                transacao.isSupervisor = BusinessUsuario.VerificarSupervisorByEscola(CodUsuario, cd_escola);
                transacao.movimentoRetroativo = Business.getParametroMovimentoRetroativo(cd_escola);
                if (transacao.Baixas.Count() <= 0)
                {
                    
                    var delTransFinan = BusinessFinanceiro.deleteTransFinanBaixa(transacao);
                    if (trasacaoFinan.cd_movimento > 0)
                        trasacaoFinan.titulosBaixa = BusinessFinanceiro.getTitulosGridByMovimento(transacao.cd_movimento, cd_escola, 0);
                    retorno.retorno = trasacaoFinan;
                    if (!delTransFinan)
                        retorno.AddMensagem(Messages.msgNotDeletedTrans, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    else
                        retorno.AddMensagem(Messages.msgDeleteSucessTrans, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    transacao = Business.editTransacao(transacao);
                    if (trasacaoFinan.cd_movimento > 0)
                        trasacaoFinan.titulosBaixa = BusinessFinanceiro.getTitulosGridByMovimento(trasacaoFinan.cd_movimento, cd_escola, 0);
                    retorno.retorno = trasacaoFinan;
                    if (transacao.cd_tran_finan <= 0)
                        retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    else
                        retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                transacao.ChequeTransacaoFinanceira = null;
                foreach (BaixaTitulo bt in transacao.Baixas)
                {
                    if (bt.Titulo != null && bt.Titulo.BaixaTitulo != null)
                        bt.Titulo.BaixaTitulo = null;
                    if (bt.TransacaoFinanceira != null)
                        bt.TransacaoFinanceira = null;
                    if (bt.ContaCorrente != null)
                        bt.ContaCorrente = null;
                    if (bt.ChequeBaixa != null)
                        bt.ChequeBaixa = null;
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_BAIXA_BOLSA_CONTRATO)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);


            }
        }

        [HttpComponentesAuthorize(Roles = "tit.a")]
        [HttpComponentesAuthorize(Roles = "bfinan.e")]
        public HttpResponseMessage postDeleteTransFinanceiraBaixa(TransacaoFinanceira transacaoFinanceira)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business,  BusinessFinanceiro, BusinessUsuario  });
                transacaoFinanceira.cd_pessoa_empresa = cd_escola;
                int CodUsuario = this.ComponentesUser.CodUsuario;

                transacaoFinanceira.isSupervisor = BusinessUsuario.VerificarSupervisorByEscola(CodUsuario, cd_escola);
                transacaoFinanceira.movimentoRetroativo = Business.getParametroMovimentoRetroativo(cd_escola);

                var delTransFinan = BusinessFinanceiro.deleteTransFinanBaixa(transacaoFinanceira);
                retorno.retorno = delTransFinan;
                if (!delTransFinan)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
                }
            }
        }

        #endregion

        #region Desistencia 

        [HttpComponentesAuthorize(Roles = "desi.i")]
        [HttpComponentesAuthorize(Roles = "bfinan")]
        [HttpComponentesAuthorize(Roles = "bfinan")]
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage postBaixarTituloAddDesistencia(DesistenciaUI desistenciaUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                var cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;
                desistenciaUI.cd_usuario = this.ComponentesUser.CodUsuario;

                DateTime dta_desistencia = DateTime.UtcNow;

                if (!String.IsNullOrEmpty(desistenciaUI.dtaDesistencia))
                {
                    //Tratamento do fuso horário da data:
                    dta_desistencia = SGF.Utils.ConversorUTC.Date(Convert.ToDateTime(desistenciaUI.dtaDesistencia),
                        this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao); 
                }

                desistenciaUI.cd_usuario = this.ComponentesUser.CodUsuario;
                desistenciaUI.fuso = this.ComponentesUser.IdFusoHorario;

                if(desistenciaUI.titulos == null)
                    desistenciaUI.titulos = new List<Titulo>();

                DesistenciaUI newDesistencia = Business.baixarTitulosInserirDesistencia(desistenciaUI, desistenciaUI.titulos.ToList(), dta_desistencia, cd_pessoa_escola, (int)desistenciaUI.cd_tipo_liquidacao, (int)desistenciaUI.cd_local_movto, false);

                if (newDesistencia.cd_desistencia <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                
                retorno.retorno = newDesistencia;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;               
            }
            catch (AlunoBusinessException ex)
            {
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_STATUS_NAO_DESISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_ESTA_CANCELADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_ESTA_DESISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (SecretariaBusinessException ex)
            {
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_DATA_MENOR_DESISTENCIA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (EscolaBusinessException ex)
            {
                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_EXISTE_AVALICAO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (TurmaBusinessException ex)
            {
                if (ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_TURMA_ENCERRADA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }   
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }

        }
        [HttpComponentesAuthorize(Roles = "desi.i")]
        [HttpComponentesAuthorize(Roles = "bfinan")]
        [HttpComponentesAuthorize(Roles = "bfinan")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpComponentesAuthorize(Roles = "ctsg")]
        public HttpResponseMessage postBaixarTituloAddDesistenciaGeral(DesistenciaUI desistenciaUI)
        {

            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                var cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;
                DateTime dta_desistencia = DateTime.UtcNow;

                if (!String.IsNullOrEmpty(desistenciaUI.dtaDesistencia))
                {
                    //dta_desistencia = SGF.Utils.ConversorUTC.Date(Convert.ToDateTime(desistenciaUI.dtaDesistencia),
                    //this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);   
                    dta_desistencia = DateTime.Parse(desistenciaUI.dtaDesistencia).Date;
                }

                desistenciaUI.cd_usuario = this.ComponentesUser.CodUsuario;
                desistenciaUI.fuso = this.ComponentesUser.IdFusoHorario;

                if (desistenciaUI.titulos == null)
                    desistenciaUI.titulos = new List<Titulo>();

                DesistenciaUI newDesistencia = Business.baixarTitulosInserirDesistencia(desistenciaUI, desistenciaUI.titulos.ToList(), dta_desistencia, cd_pessoa_escola, (int)desistenciaUI.cd_tipo_liquidacao, (int)desistenciaUI.cd_local_movto, true);

                if (newDesistencia.cd_desistencia <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                retorno.retorno = newDesistencia;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (AlunoBusinessException ex)
            {
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_STATUS_NAO_DESISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_ESTA_CANCELADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_ESTA_DESISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (SecretariaBusinessException ex)
            {
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_DATA_MENOR_DESISTENCIA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_DESISTENCIA_DT_RETROATIVA_MAT)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (EscolaBusinessException ex)
            {
                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_EXISTE_AVALICAO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (TurmaBusinessException ex)
            {
                if (ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_TURMA_ENCERRADA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }   
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }

        }

        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage postMudancaInterna(MudancasInternas mudanca)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                ISecretariaBusiness secBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();

                configuraBusiness(new List<IGenericBusiness>() { Business, alunoBiz, secBiz });
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdUsuario = this.ComponentesUser.CodUsuario;
                mudanca.cd_escola = cdEscola;
                mudanca.cd_usuario = cdUsuario;
                mudanca.fusoHorario = this.ComponentesUser.IdFusoHorario;
                bool contaSegura = this.ComponentesUser.Permissao.Contains("ctsg");

                //Tirando hora da data de inicio e data de matricula

                if(mudanca.id_manter_contrato == false && mudanca.opcao == (int)MudancasInternas.OpcoesMudanca.MudarTurma)
                {
                    List<int> cdsAlunosTurma = mudanca.alunos.Select(x => x.cd_aluno).ToList();
                    List<AlunoTurma> alunosInTurmaDestino = secBiz.existsAlunosTurmaInTurmaDestino(cdsAlunosTurma, mudanca.cd_turma_destino);
                    
                    if(alunosInTurmaDestino != null && alunosInTurmaDestino.Count > 0)
                    {
                        throw new SecretariaBusinessException(Utils.Messages.Messages.ErroAlunosTurmaInTurmaDestino, null, SecretariaBusinessException.TipoErro.ERRO_ALUNOS_TURMA_IN_TURMA_DESTINO, false);
                    }
                }
                if (Business.getQtdDiarioTurma(mudanca.cd_turma_destino, mudanca.dt_movimentacao) > 0)
                        throw new TurmaBusinessException(Utils.Messages.Messages.ErroDiarioAulaposMudanca, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_POS_MUDANCA, false);

                if (Business.getQtdDiarioTurma(mudanca.cd_turma_origem, mudanca.dt_movimentacao) > 0)
                    throw new TurmaBusinessException(Utils.Messages.Messages.ErroDiarioAulaOrigemMudanca, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_POS_MUDANCA, false);

                if (mudanca.id_ppt)
                {
                    if (!mudanca.id_manter_contrato)
                        mudanca.dt_inicio = mudanca.dt_movimentacao.ToLocalTime().Date;
                    else
                        mudanca.dt_inicio = mudanca.dt_inicio.ToLocalTime().Date;
                    mudanca.dt_movimentacao = mudanca.dt_movimentacao.ToLocalTime().Date;
                }
                else
                {
                    mudanca.dt_inicio = mudanca.dt_inicio.ToLocalTime().Date;
                    mudanca.dt_movimentacao = mudanca.dt_movimentacao.ToLocalTime().Date;
                }

                if (mudanca.alunos != null && mudanca.alunos.Count() > 0)
                    foreach (AlunoTurma at in mudanca.alunos)
                    {
                        at.dt_matricula = at.dt_matricula.HasValue ? at.dt_matricula.Value.ToLocalTime().Date : at.dt_matricula;
                        at.dt_inicio = at.dt_inicio.HasValue ? at.dt_inicio.Value.ToLocalTime() : at.dt_inicio;
                        at.cd_turma_origem = mudanca.cd_turma_origem;
                    }
                var existe = Business.postMudancaTurma(mudanca, contaSegura);
                retorno.retorno = existe;
                if (existe.cd_turma_origem <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (TurmaBusinessException ex)
            {
                if (ex.tipoErro == TurmaBusinessException.TipoErro.ERRO_TURMA_ENCERRADA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (SecretariaBusinessException ex)
            {
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_MUDANCA_TURMA_DT_RETROATIVA_MAT)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                    return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "desi.a")]
        [HttpComponentesAuthorize(Roles = "bfinan")]
        [HttpComponentesAuthorize(Roles = "bfinan")]
        [HttpComponentesAuthorize(Roles = "tit.a")]
        public HttpResponseMessage postBaixarTituloEditDesistencia(DesistenciaUI desistenciaUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                var cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;

                DateTime dta_desistencia = DateTime.UtcNow;

                if (!String.IsNullOrEmpty(desistenciaUI.dtaDesistencia))
                {
                    dta_desistencia = Convert.ToDateTime(desistenciaUI.dtaDesistencia);
                    dta_desistencia = Utils.Utils.truncarMilissegundo(dta_desistencia.ToUniversalTime());
                }

                desistenciaUI.cd_usuario = this.ComponentesUser.CodUsuario;

                if (desistenciaUI.titulos == null)
                    desistenciaUI.titulos = new List<Titulo>();

                var newDesistencia = Business.baixarTitulosEditarDesistencia(desistenciaUI, desistenciaUI.titulos.ToList(), dta_desistencia, cd_pessoa_escola, (int)desistenciaUI.cd_tipo_liquidacao, (int)desistenciaUI.cd_local_movto, false);

                if (newDesistencia.cd_desistencia <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                retorno.retorno = newDesistencia;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (AlunoBusinessException ex)
            {
                
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_STATUS_NAO_DESISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_ESTA_CANCELADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_ESTA_DESISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (SecretariaBusinessException ex)
            {
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_DATA_MENOR_DESISTENCIA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (EscolaBusinessException ex)
            {
                
                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_EXISTE_AVALICAO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                
                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "desi.a")]
        [HttpComponentesAuthorize(Roles = "bfinan")]
        [HttpComponentesAuthorize(Roles = "bfinan")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpComponentesAuthorize(Roles = "ctsg")]
        public HttpResponseMessage postBaixarTituloEditDesistenciaGeral(DesistenciaUI desistenciaUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                var cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;

                DateTime dta_desistencia = DateTime.UtcNow;

                if (!String.IsNullOrEmpty(desistenciaUI.dtaDesistencia))
                {
                    dta_desistencia = Convert.ToDateTime(desistenciaUI.dtaDesistencia);
                    dta_desistencia = Utils.Utils.truncarMilissegundo(dta_desistencia.ToUniversalTime());
                }

                desistenciaUI.cd_usuario = this.ComponentesUser.CodUsuario;

                if (desistenciaUI.titulos == null)
                    desistenciaUI.titulos = new List<Titulo>();

                var newDesistencia = Business.baixarTitulosEditarDesistencia(desistenciaUI, desistenciaUI.titulos.ToList(), dta_desistencia, cd_pessoa_escola, (int)desistenciaUI.cd_tipo_liquidacao, (int)desistenciaUI.cd_local_movto, true);

                if (newDesistencia.cd_desistencia <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                retorno.retorno = newDesistencia;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (AlunoBusinessException ex)
            {
                
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_STATUS_NAO_DESISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_ESTA_CANCELADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_ESTA_DESISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (SecretariaBusinessException ex)
            {
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_DATA_MENOR_DESISTENCIA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (EscolaBusinessException ex)
            {
                
                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_EXISTE_AVALICAO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "desi.e")]
        [HttpComponentesAuthorize(Roles = "bfinan")]
        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage PostDeleteDesistencia(List<DesistenciaUI> desistencias)
        {
            IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
            configuraBusiness(new List<IGenericBusiness>() { Business });

            ReturnResult retorno = new ReturnResult();
            try
            {
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                int cd_usuario = this.ComponentesUser.CodUsuario;
                bool deleted = Business.deletarDesistencia(desistencias, cd_pessoa_escola, cd_usuario);

                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (SecretariaBusinessException ex)
            {
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_DESISTENCIA_POSTERIOR)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    SecretariaBusinessException fx = new SecretariaBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
                }
            }
        }
        #endregion

        #region Histórico do Aluno
        [HttpComponentesAuthorize(Roles = "hista")]
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage getTitulosHistoricoAluno(int cd_pessoa, int cd_tipo) {
            ReturnResult retorno = new ReturnResult();
            try {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                TituloDataAccess.TipoConsultaTituloEnum tipo = (TituloDataAccess.TipoConsultaTituloEnum)cd_tipo;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                List<Titulo> lista_titulos = Business.getTituloByPessoa(parametros, cd_pessoa, cd_escola, tipo, false).ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, lista_titulos);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "hista")]
        [HttpComponentesAuthorize(Roles = "tit")]
        //[HttpComponentesAuthorize(Roles = "ctsg")]
        public HttpResponseMessage getTitulosHistoricoAlunoGeral(int cd_pessoa, int cd_tipo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                TituloDataAccess.TipoConsultaTituloEnum tipo = (TituloDataAccess.TipoConsultaTituloEnum)cd_tipo;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                List<Titulo> lista_titulos = Business.getTituloByPessoa(parametros, cd_pessoa, cd_escola, tipo, true).ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, lista_titulos);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }
        #endregion

        #region Movimento

        [HttpComponentesAuthorize(Roles = "tpfin")]
        [HttpComponentesAuthorize(Roles = "banc")]
        public HttpResponseMessage getComponentesNovoMovimento(int tipoMovimento, int idOrigemNF)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                Movimento movimento = new Movimento();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                movimento.tiposFinan = BusinessFinanceiro.getTipoFinanceiroAtivo();
                movimento.bancosCheque = BusinessFinanceiro.getAllBanco().ToList();
                if (idOrigemNF > 0)
                {
                    PoliticaComercial politicaSugerida = BusinessFinanceiro.getPoliticaComercialSugeridaNF(cdEscola);
                    if (politicaSugerida != null)
                    {
                        movimento.dc_politica_comercial = politicaSugerida.dc_politica_comercial;
                        movimento.cd_politica_comercial = politicaSugerida.cd_politica_comercial;
                    }
                }
                retorno.retorno = movimento;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage postComponentesTitulos(Movimento movimento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_empresa = (int)this.ComponentesUser.CodEmpresa;
                movimento.cd_pessoa_empresa = cd_empresa;
                Movimento movto = new Movimento();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IFiscalBusiness BusinessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                movto.bancos = BusinessFinanceiro.getLocalMovtoByEscola(cd_empresa, 0, true);
                movto.tiposFinan = BusinessFinanceiro.getTipoFinanceiroAtivo();

                if (!movimento.cd_tipo_nota_fiscal.HasValue || movimento.cd_tipo_nota_fiscal == 0 || BusinessFiscal.verificarTipoNotaFiscalPermiteMovimentoFinanceiro(movimento.cd_tipo_nota_fiscal.Value))
                    //Gera os títulos
                    if (movimento.cd_movimento == 0 ||  movimento.gerar_titulos)
                    {
                        if (movimento.ItensMovimento != null && movimento.ItensMovimento.Count() > 0)
                        {
                            byte tipoMovimento = movimento.id_tipo_movimento;
                            if ((movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO ||
                                movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO) && movimento.cd_tipo_nota_fiscal.HasValue)
                            {
                                tipoMovimento = BusinessFiscal.getTipoMvtoTpNF(movimento.cd_tipo_nota_fiscal.Value);
                                movimento.id_tipo_movimento = tipoMovimento;
                            }
                            if ((movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA) ||
                                (!movimento.cd_tipo_nota_fiscal.HasValue && movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO && movimento.dc_serie_movimento != "CEC"))
                                movimento.titulos[0].id_natureza_titulo = (int)Titulo.NaturezaTitulo.RECEBER;
                            else
                                movimento.titulos[0].id_natureza_titulo = (int)Titulo.NaturezaTitulo.PAGAR;

                            if (!movimento.id_nf)
                            {
                                if (movimento.cd_tipo_financeiro == (int) TipoFinanceiro.TiposFinanceiro.CHEQUE)
                                {
                                    movimento.titulos[0].dc_num_documento_titulo = movimento.Cheques.FirstOrDefault().nm_primeiro_cheque + "";
                                }
                            }
                            movto.titulos = Business.gerarTitulosMovimento(movimento.titulos[0], movimento);
                        }
                        else
                            movto.titulos = new List<Titulo>();
                    }
                    else //Busca os títulos para edição:
                        movto.titulos = BusinessFinanceiro.getTitulosGridByMovimento(movimento.cd_movimento, cd_empresa, movimento.cd_aluno);
                else
                    movto.titulos = new List<Titulo>();
                retorno.retorno = movto;
                if (movto == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_ESCOLA_NOT_EXIST_LOCALMOVTO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        [HttpComponentesAuthorize(Roles = "mvtc.i,mvtd.i,mvtp.i,mvts.i, mvtdv.i")]
        public HttpResponseMessage postInsertMovimento(Movimento movimento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                movimento.cd_pessoa_empresa = (int)this.ComponentesUser.CodEmpresa;
                movimento.dt_emissao_movimento = movimento.dt_emissao_movimento.ToLocalTime().Date;
                movimento.dt_vcto_movimento = movimento.dt_vcto_movimento.ToLocalTime().Date;
                movimento.dt_mov_movimento = movimento.dt_mov_movimento.ToLocalTime().Date;
                movimento.id_bloquear_venda_sem_estoque = Business.getIdBloquearVendasSemEstoque(movimento.cd_pessoa_empresa);
                var movtoCad = Business.addMovimento(movimento);
                retorno.retorno = movtoCad;
                if (movtoCad.cd_movimento <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO || 
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_MINDATE_SMALLDATETIME)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }

        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tit.i, tit.a, mvtc.i,mvtd.i,mvtp.i,mvts.i, mvtdv.i")]
        public HttpResponseMessage obterListaItemsKitMov(int cd_item_kit, int id_tipo_movto, int? id_natureza_TPNF)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;

                var itemsKit = BusinessFinanceiro.obterListaItemsKitMov(cd_item_kit, cdEscola, id_tipo_movto, id_natureza_TPNF);
                retorno.retorno = itemsKit;

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "tit.i, tit.a, mvtc.i,mvtd.i,mvtp.i,mvts.i, mvtdv.i")]
        public HttpResponseMessage calcularQuantidadeItemKit(ItemUI item) 
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;

                var movimento = BusinessFinanceiro.calcularQuantidadeItemKit(item, cdEscola);
                retorno.retorno = movimento;

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "tit.i, tit.a, mvtc.i,mvtd.i,mvtp.i,mvts.i, mvtdv.i")]
        public HttpResponseMessage excluirKitDoGrid(ItemUI item)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;

                var movimento = BusinessFinanceiro.excluirKitDoGrid(item, cdEscola);
                retorno.retorno = movimento;

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        [HttpComponentesAuthorize(Roles = "mvtc.a,mvtd.a,mvtp.a,mvts.i, mvtdv.i")]
        public HttpResponseMessage postUpdateMovimento(Movimento movimento)
        {
            ReturnResult retorno = new ReturnResult();
            //throw new Exception("sdfgasdfa");
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                movimento.cd_pessoa_empresa = (int)this.ComponentesUser.CodEmpresa;
                movimento.dt_emissao_movimento = movimento.dt_emissao_movimento.ToLocalTime().Date;
                movimento.dt_vcto_movimento = movimento.dt_vcto_movimento.ToLocalTime().Date;
                movimento.dt_mov_movimento = movimento.dt_mov_movimento.ToLocalTime().Date;
                movimento.id_bloquear_venda_sem_estoque = Business.getIdBloquearVendasSemEstoque(movimento.cd_pessoa_empresa);
                var movtoEdit = Business.editMovimento(movimento);
                retorno.retorno = movtoEdit;
                if (movtoEdit.cd_movimento <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB  ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NF_FECHADA_CANCELADA ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_ESTOQUE_NEGATIVO ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_MINDATE_SMALLDATETIME)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mvtp")]
        [HttpComponentesAuthorize(Roles = "esc")]
        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "item")]
        public HttpResponseMessage getMontaNFMaterial(int cd_contrato, bool id_futura)
        {
            
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IFinanceiroBusiness businessFinan = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                List<Movimento> movimentos = Business.getMovimentosbyOrigem(cd_contrato, cdEscola);
                Movimento movimento = new Movimento();
                if(movimentos != null && movimentos.Count() > 0)
                {
                    List<Movimento> newmov = movimentos.Where(x => x.id_material_didatico == true && x.id_venda_futura == id_futura).ToList();
                    if (newmov != null && newmov.Count() > 0)
                    {
                        foreach (Movimento m in newmov)
                        {
                            int cdAux = -1 * m.cd_movimento;
                            movimento = Business.getMontaNFMaterial(cdAux, cdEscola);

                            if (movimento != null && movimento.cd_tipo_nota_fiscal != null)
                            {
                                if (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA ||
                                    movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA ||
                                    movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO ||
                                    movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO)
                                {
                                    List<SituacaoTributaria> situacoes = businessFinan.getSituacaoTributaria(SituacaoTributariaDataAccess.TipoConsultaSitTribEnum.HAS_ATIVO, new List<int>(), movimento.cd_tipo_nota_fiscal.Value).ToList();
                                    if (situacoes != null && situacoes.Count > 0)
                                    {
                                        movimento.situacoesTributariaICMS = situacoes.Where(x => x.id_tipo_imposto == (int)SituacaoTributaria.TipoImpostoEnum.ICMS).OrderBy(a => a.cd_situacao_tributaria).ToList();
                                        movimento.situacoesTributariaPIS = situacoes.Where(x => x.id_tipo_imposto == (int)SituacaoTributaria.TipoImpostoEnum.PIS).ToList();
                                        movimento.situacoesTributariaCOFINS = situacoes.Where(x => x.id_tipo_imposto == (int)SituacaoTributaria.TipoImpostoEnum.CONFINS).ToList();
                                    }
                                }
                            }
                            break;  //Parar no primeito movimento que encontrar
                        }
                    }
                }

                retorno.retorno = movimento;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage getValidaMatriculaHasNFMaterial(int cd_contrato, bool id_futura)
        {

            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                List<Movimento> movimentos = Business.getMovimentosbyOrigem(cd_contrato, cdEscola);

                if (movimentos != null && movimentos.Count() > 0) 
                {
                    List<Movimento> newmov = movimentos.Where(x => x.id_material_didatico == true && x.id_venda_futura == id_futura).ToList();
                    if(newmov != null && newmov.Count() > 0)
                    {
                        retorno.retorno = new { status = true };
                    }
                    else
                    {
                        retorno.retorno = new { status = false };
                    }
                }
                else
                    retorno.retorno = new { status = false };

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        public HttpResponseMessage PostValidaMatriculaHasNFMaterial(Contrato contrato)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IFinanceiroBusiness businessFinan = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                Boolean existe = Business.postMontaNFMaterial(contrato);

                retorno.retorno = new { status = existe };

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mvts")]
        [HttpComponentesAuthorize(Roles = "esc")]
        [HttpComponentesAuthorize(Roles = "item")]
        public HttpResponseMessage getMontaNFBiblioteca(int cd_biblioteca)
        {

            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                Movimento movimento = Business.getMontaNFBiblioteca(cd_biblioteca, cdEscola);
                retorno.retorno = movimento;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_PARAMETROS_BIBLIO)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "fatu")]
        [HttpComponentesAuthorize(Roles = "esc")]
        [HttpComponentesAuthorize(Roles = "item")]
        [Obsolete]
        public HttpResponseMessage postGerarNFFaturamento(List<Titulo> titulos)
        {

            IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
            ReturnResult retorno = new ReturnResult();
            int cd_escola = (int) this.ComponentesUser.CodEmpresa;
            try
            {
                configuraBusiness(new List<IGenericBusiness>() { Business });
                bool empresaPropria = Business.getEmpresaPropria(cd_escola);
                List<Movimento> movimentos = Business.getGerarNFFaturamento(titulos, cd_escola, empresaPropria);

                var parms = "cd_escola=" + cd_escola + "&cd_movimentos=" + JsonConvert.SerializeObjectAsync(movimentos.Select(ms => ms.cd_movimento)).Result +
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                
                retorno.retorno = "relatorioapresentacao/emitirNFS?" + parametros;

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_NF_MAT ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_ESTADO_ESC_PESSOA ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_PARAMETROS_TAXA ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_PARAMETROS_MAT ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NUMERACAO_AUTOMATICA_NF ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_CONFIG_PARAMETRO_NF ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_PLANO_ITEM)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mvts")]
        [HttpComponentesAuthorize(Roles = "esc")]
        [HttpComponentesAuthorize(Roles = "item")]
        public HttpResponseMessage getMontaNFBaixaFinanceira(int cd_baixa_titulo)
        {

            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                Movimento movimento = Business.getMontaNFBaixaFinanceira(cd_baixa_titulo, cdEscola);
                retorno.retorno = movimento;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_NF_MAT ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_ESTADO_ESC_PESSOA ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_PARAMETROS_TAXA ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_PARAMETROS_MAT ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NUMERACAO_AUTOMATICA_NF ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_CONFIG_PARAMETRO_NF)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getVerifcaEstadoEscAluno(int cd_pessoa, int tpMovto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {

                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                string opCFOP = Business.verifcaEstadoEscAluno(cd_escola, cd_pessoa, tpMovto);
                retorno.retorno = opCFOP;

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_ESTADO_ESC_PESSOA)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "item")]
        public HttpResponseMessage getverificarGeracaoNFSBaixa(int cd_baixa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                bool ok = Business.verificarGeracaoNFSBaixa(cd_baixa, cdEscola);
                retorno.retorno = ok;
                if (!ok)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_ORIGEM_TITULO ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_CONFIG_PARAMETRO_NF)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        
        #endregion

        #region Conta Corrente

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        [HttpComponentesAuthorize(Roles = "coco")]
        [HttpComponentesAuthorize(Roles = "mvfin")]
        [HttpComponentesAuthorize(Roles = "locmv")]
        public HttpResponseMessage getCarregarFiltrosContaCorrente()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEmpresa = this.ComponentesUser.CodEmpresa.Value;
               
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

                ContaCorrenteList contaCorrente = new ContaCorrenteList();

                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                contaCorrente.movimentacaoFinanceira = BusinessFinanceiro.getMovimentacaoWithContaCorrente(cdEmpresa).ToList();
                if (isMaster)
                {
                    contaCorrente.localMovimentoDestino = BusinessFinanceiro.getAllLocalMovto(cdEmpresa, false, cod_pessoa_usuario).ToList();
                    contaCorrente.localMovimentoOrigem = BusinessFinanceiro.getAllLocalMovto(cdEmpresa, true, cod_pessoa_usuario).ToList();
                }
                else
                {
                    contaCorrente.localMovimentoDestino = BusinessFinanceiro.getAllLocalMovto(cdEmpresa, false, cod_pessoa_usuario).ToList();
                    contaCorrente.localMovimentoOrigem = BusinessFinanceiro.getAllLocalMovto(cdEmpresa, true, cod_pessoa_usuario).ToList();
                }
                contaCorrente.parametro = Business.getParametrosMatricula(cdEmpresa);


                retorno.retorno = contaCorrente;

                if (contaCorrente == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "coco.i")]
        [HttpComponentesAuthorize(Roles = "mvfin")]
        [HttpComponentesAuthorize(Roles = "locmv")]
        public HttpResponseMessage postIncluirContaCorrente(ContaCorrenteUI contaCorrente)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                int cd_login = this.ComponentesUser.CodUsuario;

                bool isSupervisor = BusinessUsuario.VerificarSupervisorByEscola(cd_login, cd_pessoa_escola);
                int movimento_retroativo = Business.getParametroMovimentoRetroativo(cd_pessoa_escola);

                ContaCorrenteUI newContaCorrente = BusinessFinanceiro.incluirContaCorrente(contaCorrente, cd_pessoa_escola, isSupervisor, movimento_retroativo);
                newContaCorrente.planoConta = contaCorrente.planoConta;
                retorno.retorno = newContaCorrente;
                if (newContaCorrente.cd_conta_corrente <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_CONTA_BAIXA_FINAN)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_DATA_CONTA_CORRENTE_NULA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_DATA_RETROATIVA_CONTA_CORRENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }
        }
        [HttpComponentesAuthorize(Roles = "coco.i")]
        [HttpComponentesAuthorize(Roles = "mvfin")]
        [HttpComponentesAuthorize(Roles = "locmv")]
        public HttpResponseMessage postEditarContaCorrente(ContaCorrenteUI contaCorrente)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                int cd_login = this.ComponentesUser.CodUsuario;

                bool isSupervisor = BusinessUsuario.VerificarSupervisorByEscola(cd_login, cd_pessoa_escola);
                int movimento_retroativo = Business.getParametroMovimentoRetroativo(cd_pessoa_escola);

                ContaCorrenteUI newContaCorrente = BusinessFinanceiro.editarContaCorrente(contaCorrente, isSupervisor, movimento_retroativo);
                newContaCorrente.planoConta = contaCorrente.planoConta;
                retorno.retorno = newContaCorrente;
                if (newContaCorrente.cd_conta_corrente <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_CONTA_BAIXA_FINAN)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_DATA_CONTA_CORRENTE_NULA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_DATA_RETROATIVA_CONTA_CORRENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }
        }

        [HttpComponentesAuthorize(Roles = "tprec.e")]
        public HttpResponseMessage postDeleteContaCorrente(ICollection<ContaCorrenteUI> contaCorrente)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                int cd_login = this.ComponentesUser.CodUsuario;

                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                bool isSupervisor = BusinessUsuario.VerificarSupervisorByEscola(cd_login, cd_pessoa_escola);
                int movimento_retroativo = Business.getParametroMovimentoRetroativo(cd_pessoa_escola);

                configuraBusiness(new List<IGenericBusiness>() { Business });
                bool deletado = BusinessFinanceiro.deleteContaCorrente(contaCorrente, isSupervisor, movimento_retroativo);
                retorno.retorno = deletado;
                if (deletado == false)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_CONTA_BAIXA_FINAN)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_DATA_RETROATIVA_CONTA_CORRENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
                }
            }

        }
        #endregion

        #region Parâmetro

        [HttpComponentesAuthorize(Roles = "esc")]
        [HttpComponentesAuthorize(Roles = "par")]
        public HttpResponseMessage getParametroSearch()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEmpresa = this.ComponentesUser.CodEmpresa.Value;
                Parametro parametro = new Parametro();

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                parametro = Business.getParametros(cdEmpresa);
                if (parametro != null)
                {
                    //TO DO Michelangelo
                    parametro.localMovto = BusinessFinanceiro.getLocalMovtoByEscola(cdEmpresa,0,false).ToList();
                    retorno.retorno = parametro;
                }
                else
                    retorno.retorno = BusinessFinanceiro.getLocalMovtoByEscola(cdEmpresa,0,false).ToList();
                

                if (parametro == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "rpbal")]
        public HttpResponseMessage getParametroNiveisPlanoContas() {
            ReturnResult retorno = new ReturnResult();
            try {
                int cdEmpresa = this.ComponentesUser.CodEmpresa.Value;

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                retorno.retorno = Business.getParametroNiveisPlanoContas(cdEmpresa);
                
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "esc")]
        [HttpComponentesAuthorize(Roles = "par.i")]
        public HttpResponseMessage postAddOrEditParametro(Parametro parametro)
        {

            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                var cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;
                
                parametro.cd_pessoa_escola = cd_pessoa_escola;

                Parametro newParametro = Business.insertParametro(parametro);

                if (newParametro == null)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                retorno.retorno = newParametro;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "tpNF")]
        public HttpResponseMessage getParametroRegimeTrib()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEmpresa = this.ComponentesUser.CodEmpresa.Value;

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                retorno.retorno = Business.getParametroRegimeTrib(cdEmpresa);

                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getTipoNumeroContrato()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEmpresa = this.ComponentesUser.CodEmpresa.Value;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                retorno.retorno = Business.getTipoNumeroContrato(cdEmpresa);

                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Biblioteca

        [HttpComponentesAuthorize(Roles = "bib")]
        public HttpResponseMessage getEmprestimo(int cd_biblioteca)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                Emprestimo emp = Business.getEmprestimo(cd_biblioteca, this.ComponentesUser.CodEmpresa.Value);
                retorno.retorno = emp;
                if (emp == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                {
                    emp.Item.Emprestimos = null;
                    emp.Pessoa.Emprestimos = null;
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }
        [HttpComponentesAuthorize(Roles = "bib")]
        public HttpResponseMessage getParametrosPrevDevolucao(string dataEmprestimo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                var cd_escola = (int)this.ComponentesUser.CodEmpresa;

                DateTime dt;
                if (DateTime.TryParse(dataEmprestimo, out dt))
                {
                    DateTime emprestimo = Convert.ToDateTime(dataEmprestimo);
                    DateTime emp = Convert.ToDateTime(Business.getParametrosPrevDevolucao(cd_escola, emprestimo));
                    retorno.retorno = emp;
                    if (emp == null)
                        retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                    configureHeaderResponse(response, null);
                    return response;
                }
                else
                {
                    throw new EscolaBusinessException(Utils.Messages.Messages.msgRegBuscError, null, EscolaBusinessException.TipoErro.DATA_INVALIDA, false);
                }
            }
            catch (EscolaBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        

        [HttpComponentesAuthorize(Roles = "bib.i")]
        public HttpResponseMessage postEmprestimo(Emprestimo emprestimo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                if (!string.IsNullOrEmpty(emprestimo.str_dt_emprestimo))
                    emprestimo.dt_emprestimo = DateTime.Parse(emprestimo.str_dt_emprestimo);

                if (!string.IsNullOrEmpty(emprestimo.str_dt_prevista_devolucao))
                    emprestimo.dt_prevista_devolucao = DateTime.Parse(emprestimo.str_dt_prevista_devolucao);
                
                int cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;

                emprestimo = Business.addEmprestimo(emprestimo, this.ComponentesUser.CodEmpresa.Value);
                retorno.retorno = emprestimo;
                if (emprestimo.cd_biblioteca <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO || ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "bib.a")]
        public HttpResponseMessage postEditEmprestimo(Emprestimo emprestimo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                //Tratamento do fuso horário da data:
                emprestimo.dt_emprestimo = SGF.Utils.ConversorUTC.Date(emprestimo.dt_emprestimo,
                    this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                emprestimo.dt_prevista_devolucao = SGF.Utils.ConversorUTC.Date(emprestimo.dt_prevista_devolucao,
                    this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                emprestimo.dt_devolucao = SGF.Utils.ConversorUTC.Date(emprestimo.dt_devolucao.Value,
                this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao); 

                emprestimo = Business.postEditEmprestimo(emprestimo, this.ComponentesUser.CodEmpresa.Value);
                retorno.retorno = emprestimo;
                if (emprestimo.cd_biblioteca <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO || exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "bib.a")]
        public HttpResponseMessage postRenovarEmprestimo(Emprestimo emprestimo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                emprestimo.dt_prevista_devolucao = emprestimo.dt_prevista_devolucao.ToLocalTime();
                emprestimo = Business.postRenovarEmprestimo(emprestimo, this.ComponentesUser.CodEmpresa.Value);
                retorno.retorno = emprestimo;
                if (emprestimo.cd_biblioteca <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO || exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "bib.e")]
        public HttpResponseMessage postDeleteEmprestimo(List<Emprestimo> emprestimos)
        {
            ReturnResult retorno = new ReturnResult();
            var cd_escola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                var delEmprestimos = Business.deleteEmprestimos(emprestimos, this.ComponentesUser.CodEmpresa.Value);
                retorno.retorno = delEmprestimos;
                if (!delEmprestimos)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO || exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }
        #endregion

        #region Diário de Aula

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getProgramacoesTurmasSemDiarioAula(int cd_turma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;

                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                ProgramacoesTurmaSemDiarioAula progsTurma = Business.getProgramacoesTurmasSemDiarioAula(cd_turma, cdEscola);
                retorno.retorno = progsTurma;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getAlunosPendenciaMaterialDidaticoCurso(int cd_turma, string dataProgTurma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                DateTime dtaProgTurma = Convert.ToDateTime(dataProgTurma);
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                ProgramacoesTurmaSemDiarioAula progsTurma = Business.verificarAlunosPendenciaMaterialDidaticoCurso(cd_turma, cdEscola, dtaProgTurma);
                retorno.retorno = progsTurma;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "daula")]
        public HttpResponseMessage getNmMaxFaltasAluno()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int nm_faltas_aluno = Business.getParametroNmFaltasAluno(cdEscola);
                retorno.retorno = nm_faltas_aluno;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region FollowUp

        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getEscolasFollowUp(int cd_follow_up)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IEnumerable<PessoaSearchUI> escolas = Business.getEscolasFollowUp(cd_follow_up, cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                retorno.retorno = escolas;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        

        #endregion

        #region Tipo NF
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "tpNF.i")]
        public HttpResponseMessage postTpNF(TipoNotaFiscal tpNF)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                var tpNFRet = Business.postTpNF(tpNF, cd_escola);
                retorno.retorno = tpNFRet;
                if (tpNFRet.cd_tipo_nota_fiscal <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "tpNF.a")]
        public HttpResponseMessage postAlterarTpNF(TipoNotaFiscal tpNF)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                var tpNFRet = Business.putTpNF(tpNF, cd_escola);
                retorno.retorno = tpNFRet;
                if (tpNFRet.cd_tipo_nota_fiscal <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TIPO_NF_UTILIZADO)
                    return gerarLogException(exe.Message, retorno, logger, exe);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        #endregion 

        #region Dados 
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "dadNF.i")]
        public HttpResponseMessage postDadosNF(DadosNF dado)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                var dadoRet = Business.postDadosNF(dado, cd_escola);
                retorno.retorno = dadoRet;
                if (dadoRet.cd_dados_nf <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);


                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "dadNF.i")]
        public HttpResponseMessage postAlterarDadosNF(DadosNF dado)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                var dadoRet = Business.putDadosNF(dado, cd_escola);
                retorno.retorno = dadoRet;
                if (dadoRet.cd_dados_nf <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TIPO_NF_UTILIZADO)
                    return gerarLogException(exe.Message, retorno, logger, exe);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        #endregion 

        #region Funcionário/Professor

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage getPermissaoHabilitacao()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)ComponentesUser.CodEmpresa;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                var parametro = Business.getParametrosByEscola(cdEscola);
                var habil = parametro.id_liberar_habilitacao_professor;
                retorno.retorno = habil;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage verificaRetornaSeUsuarioEProfessor()
        {
            ReturnResult retorno = new ReturnResult();
            ComponentesTurma componentesTurma = new ComponentesTurma();
            int cdPessoaUsuario = (int)this.ComponentesUser.CodPessoaUsuario;
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                ProfessorUI prof = profBiz.verificaRetornaSeUsuarioLogadoEProfessor(cdPessoaUsuario, cdEscola);
                bool id_liberar_habilitacao_professor = Business.getParametroHabilitacaoProfessor(cdEscola);
                retorno.retorno = new { id_liberar_habilitacao_professor = id_liberar_habilitacao_professor, Professor = prof };

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion 

        #region Tipo Desconto
        [HttpComponentesAuthorize(Roles = "tpdes.i")]
        public HttpResponseMessage PostTipoDesconto(TipoDescontoUI tipodesconto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { BusinessFinanceiro });
                int cdEscola = ComponentesUser.CodEmpresa.Value;

                ICollection<Escola> listaEscolaView = tipodesconto.escolas;
                tipodesconto.escolas = null;

                
                ICollection<Escola> listEscola = new List<Escola>();

                var idMaster = (bool)this.ComponentesUser.IdMaster;
                bool isMasterGeral = retornaUserMaster();

                //Se é usuario master geral busca todas as escolas:
                if (idMaster)
                {
                    IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                    ICollection<EmpresaSession> listEmp = BusinessEmpresa.findEmpresaSessionByLogin(ComponentesUser.Identity.Name, idMaster, true);
                    if (listEmp.Count <= 0)
                        listEmp = BusinessEmpresa.findAllEmpresaSession();

                    if (listaEscolaView == null || listaEscolaView.ToList().Count() == 0)
                        for (int i = 0; i < listEmp.Count(); i++)
                        {
                            Escola empresa = new Escola();
                            EmpresaSession empresaSession = listEmp.ToList()[i];
                            empresa.cd_pessoa = empresaSession.cd_pessoa;
                            empresa.dc_reduzido_pessoa = empresaSession.dc_reduzido_pessoa;
                            listEscola.Add(Escola.formEmpresaEscola(empresa));
                        }
                    else
                        listEscola = listaEscolaView;
                }
                else
                {
                    Escola empresa = new Escola();
                    empresa.cd_pessoa = cdEscola;
                    listEscola.Add(Escola.formEmpresaEscola(empresa));
                }
                TipoDescontoUI tpDescNovo = BusinessFinanceiro.postTipoDesconto(tipodesconto, cdEscola, listEscola, isMasterGeral);

                retorno.retorno = tpDescNovo;

                if (tpDescNovo.cd_tipo_desconto <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_TP_DESCONTO_ESC ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_TP_DESCONTO)
                    return gerarLogException(exe.Message, retorno, logger, exe);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "tpdes.a")]
        public HttpResponseMessage PostAlterarTipoDesconto(TipoDescontoUI tipodesconto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { BusinessFinanceiro });
                int cdEscola = ComponentesUser.CodEmpresa.Value;

                ICollection<Escola> listaEscolaView = tipodesconto.escolas;
                tipodesconto.escolas = null;


                ICollection<Escola> listEscola = new List<Escola>();

                var idMaster = (bool)this.ComponentesUser.IdMaster;
                bool isMasterGeral = retornaUserMaster();
                ICollection<EmpresaSession> listEmp = BusinessEmpresa.findEmpresaSessionByLogin(ComponentesUser.Identity.Name, idMaster, true, TransactionScopeBuilder.TransactionType.UNCOMMITED);
                //Se é usuario master geral busca todas as escolas:
                if (idMaster)
                {

                    if (listEmp.Count <= 0)
                        listEmp = BusinessEmpresa.findAllEmpresaSession(TransactionScopeBuilder.TransactionType.UNCOMMITED);

                    if (listaEscolaView == null || listaEscolaView.ToList().Count() == 0)
                        for (int i = 0; i < listEmp.Count(); i++)
                        {
                            Escola empresa = new Escola();
                            EmpresaSession empresaSession = listEmp.ToList()[i];
                            empresa.cd_pessoa = empresaSession.cd_pessoa;
                            empresa.dc_reduzido_pessoa = empresaSession.dc_reduzido_pessoa;
                            listEscola.Add(Escola.formEmpresaEscola(empresa));
                        }
                    else
                        listEscola = listaEscolaView;
                }
                else
                {
                    Escola empresa = new Escola();
                    empresa.cd_pessoa = cdEscola;
                    listEscola.Add(Escola.formEmpresaEscola(empresa));
                }

                var tipoDesc = BusinessFinanceiro.putTipoDesconto(tipodesconto, cdEscola, listEscola, isMasterGeral, listEmp);

                retorno.retorno = tipoDesc;

                if (tipoDesc.cd_tipo_desconto <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM ||
                   ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_MASTER_SEM_ESCOLA_LOGADA_VINCULADA ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TP_DESC_INCLUIDO_USUARIO_COMUM ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TP_DESC_INCLUIDO_MASTER ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TP_DESC_USADO_OUTRAS_ESC)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        #endregion

        #region Item

        [HttpComponentesAuthorize(Roles = "item.i")]
        public HttpResponseMessage postInsertItemServico(ItemUI item)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
//////              var idUsuario = BusinessEmpresa.getIdUsuario((string)Session["UserName"]);
////                FinanceiroController financeiroController = new FinanceiroController(BusinessFinanceiro, Business, BusinessEmpresa);
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cdEscola = ComponentesUser.CodEmpresa.Value;

                ICollection<Escola> listaEscolaView = item.escolas;
                item.escolas = null;


                ICollection<Escola> listEscola = new List<Escola>();

                var idMaster = (bool)this.ComponentesUser.IdMaster;
                bool isMasterGeral = retornaUserMaster();

                //Se é usuario master geral busca todas as escolas:
                if (idMaster)
                {
                    ICollection<EmpresaSession> listEmp = BusinessEmpresa.findEmpresaSessionByLogin(ComponentesUser.Identity.Name, idMaster, true);
                    if (listEmp.Count <= 0)
                        listEmp = BusinessEmpresa.findAllEmpresaSession();

                    if (listaEscolaView == null || listaEscolaView.ToList().Count() == 0)
                        for (int i = 0; i < listEmp.Count(); i++)
                        {
                            Escola empresa = new Escola();
                            EmpresaSession empresaSession = listEmp.ToList()[i];
                            empresa.cd_pessoa = empresaSession.cd_pessoa;
                            empresa.dc_reduzido_pessoa = empresaSession.dc_reduzido_pessoa;
                            listEscola.Add(Escola.formEmpresaEscola(empresa));
                        }
                    else
                        listEscola = listaEscolaView;
                }
                else
                {
                    Escola empresa = new Escola();
                    empresa.cd_pessoa = cdEscola;
                    listEscola.Add(Escola.formEmpresaEscola(empresa));
                }
               ItemUI itemNovo = BusinessFinanceiro.addItemEstoque(item, cdEscola, listEscola, isMasterGeral);
                retorno.retorno = itemNovo;
                if (itemNovo.cd_item <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM || ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TIPO_SUBGRUPO_ITEM_IGUAIS ||
                     ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_ITEM_ESC || ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_ITEM)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception exe)
            {
                var message = Utils.Utils.innerMessage(exe);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, exe, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, exe);
                }
            }

        }

        public HttpResponseMessage getItensKit(int idKit)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                List<KitUI> itensKit = BusinessFinanceiro.getItensKit(idKit);
                retorno.retorno = itensKit;
                if (itensKit == null || itensKit.Count == 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, retorno);

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "item.a")]
        public HttpResponseMessage postEditItemServico(ItemUI item)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cdEscola = ComponentesUser.CodEmpresa.Value;

                ICollection<Escola> listaEscolaView = item.escolas;
                item.escolas = null;


                ICollection<Escola> listEscola = new List<Escola>();

                var idMaster = (bool)this.ComponentesUser.IdMaster;
                bool isMasterGeral = retornaUserMaster();
                ICollection<EmpresaSession> listEmp = BusinessEmpresa.findEmpresaSessionByLogin(ComponentesUser.Identity.Name, idMaster, true);
                //Se é usuario master geral busca todas as escolas:
                if (idMaster)
                {

                    if (listEmp.Count <= 0)
                        listEmp = BusinessEmpresa.findAllEmpresaSession();

                    if (listaEscolaView == null || listaEscolaView.ToList().Count() == 0)
                        for (int i = 0; i < listEmp.Count(); i++)
                        {
                            Escola empresa = new Escola();
                            EmpresaSession empresaSession = listEmp.ToList()[i];
                            empresa.cd_pessoa = empresaSession.cd_pessoa;
                            empresa.dc_reduzido_pessoa = empresaSession.dc_reduzido_pessoa;
                            listEscola.Add(Escola.formEmpresaEscola(empresa));
                        }
                    else
                        listEscola = listaEscolaView;
                }
                else
                {
                    Escola empresa = new Escola();
                    empresa.cd_pessoa = cdEscola;
                    listEscola.Add(Escola.formEmpresaEscola(empresa));
                }
                ItemUI itemAlterado = BusinessFinanceiro.editarItemEstoque(item, cdEscola, listEscola, isMasterGeral, listEmp);
                retorno.retorno = itemAlterado;
                if (itemAlterado.cd_item <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM || 
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TIPO_SUBGRUPO_ITEM_IGUAIS ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_SUBGRUPO_ITEM_IGUAIS ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_ITEM_USADO_OUTRAS_ESC)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
                }
            }

        }

        [HttpComponentesAuthorize(Roles = "item.a")]
        public HttpResponseMessage postEditKit(ItemUI item)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cdEscola = ComponentesUser.CodEmpresa.Value;

                ICollection<Escola> listaEscolaView = item.escolas;
                item.escolas = null;


                ICollection<Escola> listEscola = new List<Escola>();

                var idMaster = (bool)this.ComponentesUser.IdMaster;
                bool isMasterGeral = retornaUserMaster();
                ICollection<EmpresaSession> listEmp = BusinessEmpresa.findEmpresaSessionByLogin(ComponentesUser.Identity.Name, idMaster, true);
                //Se é usuario master geral busca todas as escolas:
                if (idMaster)
                {

                    if (listEmp.Count <= 0)
                        listEmp = BusinessEmpresa.findAllEmpresaSession();

                    if (listaEscolaView == null || listaEscolaView.ToList().Count() == 0)
                        for (int i = 0; i < listEmp.Count(); i++)
                        {
                            Escola empresa = new Escola();
                            EmpresaSession empresaSession = listEmp.ToList()[i];
                            empresa.cd_pessoa = empresaSession.cd_pessoa;
                            empresa.dc_reduzido_pessoa = empresaSession.dc_reduzido_pessoa;
                            listEscola.Add(Escola.formEmpresaEscola(empresa));
                        }
                    else
                        listEscola = listaEscolaView;
                }
                else
                {
                    Escola empresa = new Escola();
                    empresa.cd_pessoa = cdEscola;
                    listEscola.Add(Escola.formEmpresaEscola(empresa));
                }
                ItemUI itemAlterado = BusinessFinanceiro.editarKitEstoque(item, cdEscola, listEscola, isMasterGeral, listEmp);
                retorno.retorno = itemAlterado;
                if (itemAlterado.cd_item <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TIPO_SUBGRUPO_ITEM_IGUAIS ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_SUBGRUPO_ITEM_IGUAIS ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_ITEM_USADO_OUTRAS_ESC)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        #endregion

        #region Nota Fiscal

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tit.e")]
        [HttpComponentesAuthorize(Roles = "mvtc,mvts")]
        public HttpResponseMessage postVerificarCancelamentoNF(int cd_movimento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IFiscalBusiness BusinessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { BusinessFiscal });
                bool empresaPropria = Business.getEmpresaPropria(cdEscola);
                Movimento movimento = BusinessFiscal.postVerificarCancelamentoNF(cdEscola, cd_movimento, empresaPropria);
                //retorno.retorno = movimento;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (FiscalBusinessException exe)
            {
                if (exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_CANCELAR_NF)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgErrorCancelarNF, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgErrorCancelarNF, retorno, logger, ex);
            }
        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "tit.e")]
        [HttpComponentesAuthorize(Roles = "mvtc,mvts")]
        public HttpResponseMessage postCancelarNF(Movimento movimento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                bool empresaPropria = Business.getEmpresaPropria(cd_escola);
                movimento.cd_pessoa_empresa = cd_escola;
                Business.cancelarNFServico(movimento, empresaPropria);
                retorno.AddMensagem(Messages.msgCancelNFSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (FiscalBusinessException exe)
            {

                if (exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_CANCELAR_NF)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgErrorCancelarNF, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgErrorCancelarNF, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "mvtc,mvts")]
        public HttpResponseMessage postReenviarMovimentoParaMasterSaf(int cd_movimento, int id_tipo_movimento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFiscalBusiness BusinessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { BusinessFiscal });
                bool empresaPropria = Business.getEmpresaPropria(cdEscola);
                bool enviadoMovimento = false;
                if (id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA ||
                   id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO ||
                   id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO)
                    enviadoMovimento = BusinessFiscal.postReenviarNFMasterSaf(cd_movimento, cdEscola, empresaPropria);
                if (enviadoMovimento)
                    retorno.AddMensagem(Messages.msgRegEnviadoSucessoMSF, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgErroEnviarRegistroMSF, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (FiscalBusinessException exe)
            {
                if (exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_CANCELAR_NF)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgErroEnviarRegistroMSF, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgErroEnviarRegistroMSF, retorno, logger, ex);
            }
        }

        #endregion

        #region Mala Direta

        [HttpComponentesAuthorize(Roles = "confs")]
        public HttpResponseMessage getConfigEmailMarketingSysApp()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                SysApp sysApp = Business.getConfigEmailMarketingSysApp();
                retorno.retorno = sysApp;
                if (sysApp == null)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "mailm")]
        public HttpResponseMessage getRodapeSysApp()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                String sysApp = Business.getRodapeSysApp();
                retorno.retorno = sysApp;
                if (sysApp == null)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }

        }


        [HttpComponentesAuthorize(Roles = "confs")]
        public HttpResponseMessage postAlterarConfigEmailMarketing(SysApp sysApp)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                if (BusinessEmpresa.VerificarMasterGeral(ComponentesUser.CodUsuario))
                {
                    var rodape = Business.putConfigEmailMarketingSysApp(sysApp);
                    retorno.retorno = rodape;
                    if (rodape.cd_sys_app <= 0)
                        retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    else
                        retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        #endregion

        #region Reajuste Anual

        [HttpComponentesAuthorize(Roles = "reaa.a")]
        public HttpResponseMessage postAbrirFecharReajusteAnual(ReajusteAnual reajuste)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                reajuste.cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;
                reajuste.cd_usuario = (int)this.ComponentesUser.CodUsuario;
                reajuste.dh_cadastro_reajuste = Utils.ConversorUTC.ToLocalTime(DateTime.UtcNow, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                ReajusteAnual reajusteInserido = Business.abrirFecharReajusteAnual(reajuste, (int)this.ComponentesUser.CodUsuario);
                retorno.retorno = reajusteInserido;

                if (reajusteInserido.cd_reajuste_anual <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_SUM_DIFERENTE_POLCOM)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_PARC_DIF_OBRIGATORIO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_DIA_POL_COMERCIAL_NULL)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        #endregion

        #region GerarTokenUrlAreaRestrita

        [HttpGet]
        public HttpResponseMessage gerarUrlAreaRestrita()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                IApiAreaRestritaBusiness BusinessAreRestrita = (IApiAreaRestritaBusiness)base.instanciarBusiness<IApiAreaRestritaBusiness>();
                int usuario = this.ComponentesUser.CodUsuario;


                var usuarioAreaRestrita = BusinessUsuario.GetEmailUsuario(usuario);
                string urlAreaRestrita = "";

                if (usuarioAreaRestrita != null)
                {
                    if (String.IsNullOrEmpty(usuarioAreaRestrita.email))
                    {
                        throw new UsuarioBusinessException(Messages.msgErrorEmailUsuarioNotFound, null, UsuarioBusinessException.TipoErro.ERRO_USUARIO_EMAIL_NOT_FOUND, false);
                    }

                    urlAreaRestrita = BusinessAreRestrita.GerarToken(usuarioAreaRestrita.email);

                }

                
                retorno.retorno = urlAreaRestrita;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception exe)
            {
                logger.Error(exe);
                retorno.AddMensagem(Messages.msgRegBuscError, exe.Message + exe.StackTrace + exe.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }

        }

        #endregion


    }
}