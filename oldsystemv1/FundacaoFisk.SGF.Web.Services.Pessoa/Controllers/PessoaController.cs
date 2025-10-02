using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum;
using FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess;
using Newtonsoft.Json.Linq;
using Componentes.GenericController;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using Componentes.Utils.Messages;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Controllers
{
    using System.Net.Http.Headers;
    using log4net;
    using Newtonsoft.Json;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
    using System.Web;
    using Componentes.Utils;
    using System.Configuration;
    using Componentes.GenericBusiness.Comum;
    using System.Collections;
    
    using FundacaoFisk.SGF.GenericModel;
    public class PessoaController : ComponentesApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PessoaController));

        public PessoaController()
        {
        }
        
        #region Pessoa

        public HttpResponseMessage getPessoasResponsavel()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                retorno.retorno = Business.getPessoasResponsavel((int)PapelSGF.TipoPapelSGF.RESPONSAVEL, cdEscola).ToList();

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // GET api/<controller>/5 
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage GetPessoaSearch(string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, byte tipo_pesquisa)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                //if(papel <= 0)
                //    papel = null;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var retorno = Business.GetPessoaSearch(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, tipo_pesquisa);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }

        // GET api/<controller>/5 
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage GetPessoaResponsavelCPFSearch(string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                //if(papel <= 0)
                //    papel = null;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var retorno = Business.GetPessoaResponsavelCPFSearch(parametros, nome, apelido, inicio, cnpjCpf, sexo);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }

        // GET api/<controller>/5 
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage GetPessoaResponsavelSearch(string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int cdPai, int sexo, int papel)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                //if(papel <= 0)
                //    papel = null;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var retorno = Business.GetPessoaResponsavelSearch(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, cdPai, sexo, papel);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }

        //metodo usado para verificar e trazer os dados da pessoa fisica pelo cpf ou código da pessoa.

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage VerificarPessoaByCpf(string cpf)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                PessoaFisicaSGF pessoaFisica = Business.VerificarPessoByCpf(cpf);
                retorno.retorno = pessoaFisica;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getExistPessoaFisicaByCpf(string cpf)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                PessoaFisicaSGF pessoaFisica = Business.VerificarPessoByCpf(cpf);
                if (pessoaFisica != null && pessoaFisica.cd_pessoa > 0)
                {
                    retorno.retorno = pessoaFisica;
                    retorno.AddMensagem(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroCPFJaCadastrado, pessoaFisica.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getExistEmpresaByCnpj(string cnpj)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var pessoaJuridica = Business.ExistsPessoaByCNPJ(cnpj);
                if (pessoaJuridica != null && pessoaJuridica.cd_pessoa > 0)
                {
                    retorno.retorno = pessoaJuridica;
                    retorno.AddMensagem(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroCPFJaCadastrado, pessoaJuridica.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }

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
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage VerificarEmpresaByCnpj(string cnpj)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var empresa = Business.VerificarEmpresaByCnpj(cnpj);
                retorno.retorno = empresa;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgInfFailDeleted, retorno, logger, ex);
            }
        }

        //Metodo usado para trazer pessoa fisica no relacionamento
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage VerificarPessoaByCdPessoaOrCpf(int cdPessoa,string cpf)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                PessoaFisicaSGF pessoaFisica = Business.VerificarPessoByCdPessoa(cdPessoa);
                retorno.retorno = pessoaFisica;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        //Metodo usado para trazer a pessoa Fisica no relacionamento quando Pesquisado CPF

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage ExistsPessoaByCpf(string cpf)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var pessoaFisica = Business.ExistsPessoByCpf(cpf);
                var exist = false;
                var no_pessoa = "";
                if (pessoaFisica != null && pessoaFisica.cd_pessoa > 0)
                {
                    exist = true;
                    no_pessoa = pessoaFisica.no_pessoa;
                }
                retorno.retorno = new { no_pessoa = no_pessoa, exist = exist };
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage ExistsPessoaJuridicaByCnpj(string cnpj)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var pessoaJuridica = Business.ExistsPessoaByCNPJ(cnpj);
                var exist = false;
                var no_pessoa = "";
                if (pessoaJuridica != null && pessoaJuridica.cd_pessoa > 0)
                {
                    exist = true;
                    no_pessoa = pessoaJuridica.no_pessoa;
                }
                retorno.retorno = new { no_pessoa = no_pessoa, exist = exist };
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "pes.e")]
        public HttpResponseMessage PostdeletePessoa(List<PessoaSGF> pessoas)
        {
            ReturnResult retorno = new ReturnResult();
            var cd_empresa = (int)this.ComponentesUser.CodEmpresa;
            //string login = ComponentesUser.Identity.Name;
            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                var delPessoa = Business.PostdeletePessoa(pessoas, cd_empresa);
                retorno.retorno = delPessoa;
                if (!delPessoa)
                {
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgInfFailDeleted, retorno, logger, ex);
            }
        }

         [HttpComponentesAuthorize(Roles = "pes.i")]
        public HttpResponseMessage PostInsertPessoaJuridica(PessoaJuridicaUI pessoaJuridica)
        {
            string fullPath = null;
            string caminho = null;
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (pessoaJuridica.pessoaJuridica.cd_pessoa > 0)
                    throw new Exception("Registro já existe, favor consulta-lo");
                pessoaJuridica.pessoaJuridica.id_pessoa_empresa = false;
                if (pessoaJuridica != null && !string.IsNullOrEmpty(pessoaJuridica.descFoto))
                {
                    fullPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["caminhoUploads"]);
                    caminho = fullPath + "/" + pessoaJuridica.descFoto;
                    pessoaJuridica.pessoaJuridica.img_pessoa = ManipuladorArquivo.getPathPhoto(caminho);
                    pessoaJuridica.pessoaJuridica.ext_img_pessoa = pessoaJuridica.descFoto;
                }
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var insertPessoaJuridica = Business.postInsertPessoaJuridica(pessoaJuridica, new List<RelacionamentoSGF>(), false);

                retorno.retorno = PessoaSearchUI.fromPessoaForPessoaSearchUI(insertPessoaJuridica);
                if (insertPessoaJuridica.cd_pessoa <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    PessoaBusinessException fx = new PessoaBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }
        }

        [HttpComponentesAuthorize(Roles = "pes.i")]
        public HttpResponseMessage PostInsertPessoaFisica(PessoaFisicaUI pessoaFisica)
        {
            string fullPath = null;
            string caminho = null;
            ReturnResult retorno = new ReturnResult();
            try
            {
                pessoaFisica.pessoaFisica.id_pessoa_empresa = false;
                pessoaFisica.pessoaFisica.dt_nascimento = pessoaFisica.pessoaFisica.dt_nascimento.HasValue ? pessoaFisica.pessoaFisica.dt_nascimento.Value.Date : pessoaFisica.pessoaFisica.dt_nascimento;

                if (pessoaFisica != null && !string.IsNullOrEmpty(pessoaFisica.descFoto))
                {

                    fullPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["caminhoUploads"]);
                    caminho = fullPath + "/" + pessoaFisica.descFoto;
                    pessoaFisica.pessoaFisica.img_pessoa = ManipuladorArquivo.getPathPhoto(caminho);
                    pessoaFisica.pessoaFisica.ext_img_pessoa = pessoaFisica.descFoto;
                }
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var insertPessoaFisica = Business.postInsertPessoaFisica(pessoaFisica, new List<RelacionamentoSGF>(), false);
                retorno.retorno = PessoaSearchUI.fromPessoaForPessoaSearchUI(insertPessoaFisica);
                if (insertPessoaFisica.cd_pessoa <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    PessoaBusinessException fx = new PessoaBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }
        }

        [HttpComponentesAuthorize(Roles = "pes.a")]
        public HttpResponseMessage postUpdatePessoaFisica(PessoaFisicaUI pessoaFisica)
        {
            string fullPath = null;
            string caminho = null;
            ReturnResult retorno = new ReturnResult();
            try
            {
                pessoaFisica.pessoaFisica.id_pessoa_empresa = false;
                pessoaFisica.pessoaFisica.dt_nascimento = pessoaFisica.pessoaFisica.dt_nascimento.HasValue ? pessoaFisica.pessoaFisica.dt_nascimento.Value.Date : pessoaFisica.pessoaFisica.dt_nascimento;

                if (pessoaFisica != null && !string.IsNullOrEmpty(pessoaFisica.descFoto))
                {

                    fullPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["caminhoUploads"]);
                    caminho = fullPath + "/" + pessoaFisica.descFoto;
                    pessoaFisica.pessoaFisica.img_pessoa = ManipuladorArquivo.getPathPhoto(caminho);
                    pessoaFisica.pessoaFisica.ext_img_pessoa = pessoaFisica.descFoto;
                }
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var insertPessoaFisica = Business.postUpdatePessoaFisica(pessoaFisica, new List<RelacionamentoSGF>(), false, false);
                retorno.retorno = PessoaSearchUI.fromPessoaForPessoaSearchUI(insertPessoaFisica);
                if (insertPessoaFisica.cd_pessoa <= 0)
                {
                    retorno.AddMensagem(Messages.msgNotUpDateReg, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    PessoaBusinessException fx = new PessoaBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
                }
            }
        }

        [HttpComponentesAuthorize(Roles = "pes.a")]
        public HttpResponseMessage postUpdatePessoaJuridica(PessoaJuridicaUI pessoaJuridica)
        {
            string fullPath = null;
            string caminho = null;
            ReturnResult retorno = new ReturnResult();
            try
            {
                pessoaJuridica.pessoaJuridica.id_pessoa_empresa = false;
                if (pessoaJuridica != null && !string.IsNullOrEmpty(pessoaJuridica.descFoto))
                {

                    fullPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["caminhoUploads"]);
                    caminho = fullPath + "/" + pessoaJuridica.descFoto;
                    pessoaJuridica.pessoaJuridica.img_pessoa = ManipuladorArquivo.getPathPhoto(caminho);
                    pessoaJuridica.pessoaJuridica.ext_img_pessoa = pessoaJuridica.descFoto;
                }
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var insertPessoaJuridica = Business.postUpdatePessoaJuridica(pessoaJuridica, new List<RelacionamentoSGF>(), false);
                retorno.retorno = PessoaSearchUI.fromPessoaForPessoaSearchUI(insertPessoaJuridica);
                if (insertPessoaJuridica.cd_pessoa <= 0)
                {
                    retorno.AddMensagem(Messages.msgNotUpDateReg, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    PessoaBusinessException fx = new PessoaBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
                }
            }
        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "pes")]
        [HttpComponentesAuthorize(Roles = "tpend")]
        [HttpComponentesAuthorize(Roles = "tlog")]
        [HttpComponentesAuthorize(Roles = "estad")]
        [HttpComponentesAuthorize(Roles = "ctele")]
        [HttpComponentesAuthorize(Roles = "ttele")]
        [HttpComponentesAuthorize(Roles = "oper")]
        public HttpResponseMessage ComponentesPessoa(int[] tipo, int? cd_operadora)
        {
            ReturnResult retorno = new ReturnResult();
            ComponentesPessoaUI comPessoaUI = new ComponentesPessoaUI();
            try
            {
                ILocalidadeBusiness BusinessLoc = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();

                
                comPessoaUI.tiposEndereco = BusinessLoc.GetAllTipoEndereco();
                comPessoaUI.tiposLogradouro = BusinessLoc.GetAllTipoLogradouro();
                comPessoaUI.estadosUI = BusinessLoc.GetAllEstado();
                comPessoaUI.classesTelefone = BusinessLoc.GetAllClasseTelefone();
                comPessoaUI.tiposTelefone = BusinessLoc.GetAllTipoTelefone();
                comPessoaUI.orgaosExpedidores = Business.getAllOrgaoExpedidor();
                comPessoaUI.papeis = Business.getPapelByTipo(tipo);
                comPessoaUI.operadoras = BusinessLoc.GetAllOperadorasAtivas(cd_operadora);
                comPessoaUI.qualifRelacionamentos = Business.getAllQualifRelacByPapel((int)PapelSGF.TipoPapelSGF.RESPONSAVEL).ToList();
                comPessoaUI.gruposEstoques = Business.findAllGrupoAtivo(0, true);
                retorno.retorno = comPessoaUI;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAllGrupoAtivo()
        {
            ReturnResult retorno = new ReturnResult();
            
            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                
                retorno.retorno = Business.findAllGrupoAtivo(0, true); 
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "tpend")]
        [HttpComponentesAuthorize(Roles = "tlog")]
        [HttpComponentesAuthorize(Roles = "estad")]
        public HttpResponseMessage ComponentesProspect()
        {
            ReturnResult retorno = new ReturnResult();
            ComponentesPessoaUI comPessoaUI = new ComponentesPessoaUI();
            try
            {
                ILocalidadeBusiness BusinessLoc = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                comPessoaUI.tiposEndereco = BusinessLoc.GetAllTipoEndereco();
                comPessoaUI.tiposLogradouro = BusinessLoc.GetAllTipoLogradouro();
                comPessoaUI.estadosUI = BusinessLoc.GetAllEstado();
                retorno.retorno = comPessoaUI;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getQualifRelacionamento() 
        {
            ReturnResult retorno = new ReturnResult();
            ComponentesPessoaUI comPessoaUI = new ComponentesPessoaUI();

            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();

                comPessoaUI.qualifRelacionamentos = Business.getAllQualifRelacByPapel((int)PapelSGF.TipoPapelSGF.RESPONSAVEL).ToList();
                retorno.retorno = comPessoaUI;

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getTdsPessoaSearchEscolaCadMovimento(string nome, string apelido,bool inicio, int tipoPessoa, string cnpjCpf, int sexo)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var ret = Business.getTdsPessoaSearchEscolaCadMovimento(parametros, nome, apelido,inicio, tipoPessoa, cnpjCpf, sexo, cdEscola).ToList();

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, ret);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        [HttpComponentesAuthorize(Roles = "tpend")]
        [HttpComponentesAuthorize(Roles = "tlog")]
        [HttpComponentesAuthorize(Roles = "estad")]
        public HttpResponseMessage ComponentesCadastroEnderecoPessoa()
        {
            ReturnResult retorno = new ReturnResult();
            ComponentesPessoaUI comPessoaUI = new ComponentesPessoaUI();
            try
            {
                ILocalidadeBusiness BusinessLoc = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                comPessoaUI.tiposEndereco = BusinessLoc.GetAllTipoEndereco();
                comPessoaUI.tiposLogradouro = BusinessLoc.GetAllTipoLogradouro();
                comPessoaUI.estadosUI = BusinessLoc.GetAllEstado();
                comPessoaUI.qualifRelacionamentos = Business.getAllQualifRelacByPapel((int)PapelSGF.TipoPapelSGF.RESPONSAVEL).ToList();
                retorno.retorno = comPessoaUI;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Atividade

        //[HttpGet]
        //[HttpComponentesAuthorize(Roles = "pes")]
        //public HttpResponseMessage getAllAtividades(string search, int natureza, int status)
        //{
        //    logger.Debug("inicio getAllAtividades");
        //    ReturnResult retorno = new ReturnResult();
        //    try
        //    {
        //        var atividades = Business.getAllListAtividades(search, natureza, getStatus(status));
        //        retorno.retorno = atividades;
        //        logger.Debug("fim getAllAtividades");
        //        return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("PessoaController getAllAtividades - Erro: ", ex);
        //        return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
        //    }
        //}

        #endregion

        #region Tratamento Pessoa

        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getAllTratamentoPessoa()
        {
             ReturnResult retorno = new ReturnResult();
            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var tratamentos = Business.getAllTratamentoPessoa();
                retorno.retorno = tratamentos;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }
        #endregion

        #region Estado Civil

        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getAllEstadoCivil()
        {
             ReturnResult retorno = new ReturnResult();
            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var estadosCivil = Business.getAllEstadoCivil();
                retorno.retorno = estadosCivil;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }
        #endregion

        #region Orgao Expedidor

        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getAllOrgaoExpedidor()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var orgaosExpedidores = Business.getAllOrgaoExpedidor();
                retorno.retorno = orgaosExpedidores;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }
        #endregion

        #region Papel
        [HttpPost]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getPapelByTipo(int[] tipo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var allPapel = Business.getPapelByTipo(tipo);
                retorno.retorno = allPapel;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }
        #endregion

        #region Qualif Relacionamento
         [HttpGet]
         [HttpComponentesAuthorize(Roles = "pes")]
         public HttpResponseMessage getAllQualifRelacByPapel(int codPapel)
         {
             ReturnResult retorno = new ReturnResult();
             try
             {
                 IPessoaBusiness Business = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                 var qualifRelacs = Business.getAllQualifRelacByPapel(codPapel);
                 retorno.retorno = qualifRelacs;
                 HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                 return response;
             }
             catch (Exception ex)
             {
                 return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
             }
         }

        #endregion
    }
}