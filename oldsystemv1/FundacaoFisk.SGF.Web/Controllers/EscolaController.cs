using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.Utils;
using FundacaoFisk.SGF.Utils.Messages;
using log4net;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
//using Componentes.GenericModel;
using System.Net.Http.Headers;
using FundacaoFisk.SGF.Services.InstituicaoEnsino.Controllers;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.InstituicaoEnsino.Model;
using FundacaoFisk.SGF.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using Componentes.GenericController;
using Componentes.GenericBusiness.Comum;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using System.Configuration;
using FundacaoFisk.SGF.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
using FundacaoFisk.SGF.Web.Services.Usuario.Business;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Business;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;

namespace FundacaoFisk.SGF.Web.Controllers
{
    public class EscolaController : ComponentesMVCController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(EscolaController));

        public EscolaController()
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Escola

        // [HttpPost]
        [MvcComponentesAuthorize(Roles = "esc.i")]
        [MvcComponentesAuthorize(Roles = "bair")]
        [MvcComponentesAuthorize(Roles = "cidd")]
        [MvcComponentesAuthorize(Roles = "dist")]
        [MvcComponentesAuthorize(Roles = "estad")]
        [MvcComponentesAuthorize(Roles = "pais")]
        public RenderJsonActionResult PostInsertEscola(EscolaUI escolaUI)
        {
            string fullPath = ConfigurationManager.AppSettings["caminhoUploads"];
            string caminho = null;
            List<RelacionamentoSGF> relacionamentos = null;
            ReturnResult retorno = new ReturnResult();
            escolaUI.pessoaJuridica.pessoaJuridica.dt_cadastramento = Utils.Utils.truncarMilissegundo(escolaUI.pessoaJuridica.pessoaJuridica.dt_cadastramento.ToUniversalTime());
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness) base.instanciarBusiness<IEscolaBusiness>();
                IPessoaBusiness BusinessPessoa = (IPessoaBusiness) base.instanciarBusiness<IPessoaBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness) base.instanciarBusiness<IEmpresaBusiness>();
                int cdEsc = int.Parse(Session["CodEscolaSelecionada"] + "");

                configuraBusiness(new List<IGenericBusiness>() {Business, BusinessPessoa, BusinessEmpresa});
                if (escolaUI != null && !string.IsNullOrEmpty(escolaUI.pessoaJuridica.descFoto))
                {
                    caminho = fullPath + "/" + escolaUI.pessoaJuridica.descFoto;
                    escolaUI.pessoaJuridica.pessoaJuridica.img_pessoa = ManipuladorArquivo.getPathPhoto(caminho);
                    escolaUI.pessoaJuridica.pessoaJuridica.ext_img_pessoa = escolaUI.pessoaJuridica.descFoto;
                }

                if (escolaUI.pessoaJuridica.relacionamentosUI != null && escolaUI.pessoaJuridica.relacionamentosUI.Count() > 0)
                {
                    relacionamentos = new List<RelacionamentoSGF>();
                    foreach (var item in escolaUI.pessoaJuridica.relacionamentosUI)
                    {
                        RelacionamentoSGF relac = new RelacionamentoSGF();
                        relac = item.relacionamento;
                        if (item.nm_natureza_pessoa == (int) PessoaSGF.TipoPessoa.FISICA)
                        {
                            if (item.pessoaFisicaRelac != null && !string.IsNullOrEmpty(item.pessoaFisicaRelac.no_pessoa) && item.pessoaFisicaRelac.nm_sexo > 0)
                            {
                                item.pessoaFisicaRelac.nm_natureza_pessoa = (int) PessoaSGF.TipoPessoa.FISICA;
                                relac.PessoaFilho = item.pessoaFisicaRelac;
                                if (item.enderecoRelac != null && item.enderecoRelac.cd_loc_cidade > 0 && item.enderecoRelac.cd_loc_bairro > 0 && item.enderecoRelac.cd_loc_estado > 0 &&
                                    item.enderecoRelac.cd_loc_logradouro > 0 && item.enderecoRelac.cd_tipo_logradouro > 0)
                                    relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            }

                            relacionamentos.Add(relac);
                        }
                        else
                        {
                            if (item.pessoaJuridicaRelac != null && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.no_pessoa) && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.dc_num_cgc))
                            {
                                item.pessoaJuridicaRelac.cd_tipo_sociedade = 1;
                                item.pessoaJuridicaRelac.nm_natureza_pessoa = (int) PessoaSGF.TipoPessoa.JURIDICA;
                                relac.PessoaFilho = item.pessoaJuridicaRelac;
                                if (item.enderecoRelac != null && item.enderecoRelac.cd_loc_cidade > 0 && item.enderecoRelac.cd_loc_bairro > 0 && item.enderecoRelac.cd_loc_estado > 0 &&
                                    item.enderecoRelac.cd_loc_logradouro > 0 && item.enderecoRelac.cd_tipo_logradouro > 0)
                                    relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            }

                            relacionamentos.Add(relac);
                        }

                    }
                }

                var insertEscola = Business.addEscola(escolaUI, relacionamentos, cdEsc, fullPath);

                if (insertEscola.cd_pessoa <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                {
                    retorno.retorno = insertEscola;
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                return new RenderJsonActionResult {Result = retorno};
            }
            catch (ApiNewCyberException ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    EscolaBusinessException fx = new EscolaBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }
        }

        [MvcComponentesAuthorize(Roles = "esc.a")]
        [MvcComponentesAuthorize(Roles = "bair")]
        [MvcComponentesAuthorize(Roles = "cidd")]
        [MvcComponentesAuthorize(Roles = "dist")]
        [MvcComponentesAuthorize(Roles = "estad")]
        [MvcComponentesAuthorize(Roles = "pais")]
        public RenderJsonActionResult postEditEscola(EscolaUI escolaUI)
        {
            string fullPath = ConfigurationManager.AppSettings["caminhoUploads"];
            string caminho = null;
            ReturnResult retorno = new ReturnResult();
            List<RelacionamentoSGF> relacionamentos = null;
            if (escolaUI.pessoaJuridica.relacionamentosUI != null && escolaUI.pessoaJuridica.relacionamentosUI.Count() == 0)
                relacionamentos = new List<RelacionamentoSGF>();

            escolaUI.pessoaJuridica.pessoaJuridica.dt_cadastramento = escolaUI.pessoaJuridica.pessoaJuridica.dt_cadastramento.ToUniversalTime();
             try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IPessoaBusiness BusinessPessoa = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                int cdEsc = int.Parse(Session["CodEscolaSelecionada"] + "");

                configuraBusiness(new List<IGenericBusiness>() { Business, BusinessPessoa, BusinessEmpresa });
                if (escolaUI != null && !string.IsNullOrEmpty(escolaUI.pessoaJuridica.descFoto))
                {
                    caminho = fullPath + "/" + escolaUI.pessoaJuridica.descFoto;
                    escolaUI.pessoaJuridica.pessoaJuridica.img_pessoa = ManipuladorArquivo.getPathPhoto(caminho);
                    escolaUI.pessoaJuridica.pessoaJuridica.ext_img_pessoa = escolaUI.pessoaJuridica.descFoto;
                }
                if (escolaUI.pessoaJuridica.relacionamentosUI != null && escolaUI.pessoaJuridica.relacionamentosUI.Count() > 0)
                {
                    relacionamentos = new List<RelacionamentoSGF>();
                    foreach (var item in escolaUI.pessoaJuridica.relacionamentosUI)
                    {
                        RelacionamentoSGF relac = new RelacionamentoSGF();
                        relac = item.relacionamento;
                        if (item.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA)
                        {
                            if (item.pessoaFisicaRelac != null && !string.IsNullOrEmpty(item.pessoaFisicaRelac.no_pessoa) && item.pessoaFisicaRelac.nm_sexo > 0)
                            {
                                item.pessoaFisicaRelac.nm_natureza_pessoa = (int)PessoaSGF.TipoPessoa.FISICA;
                                relac.PessoaFilho = item.pessoaFisicaRelac;
                                if (item.enderecoRelac != null && item.enderecoRelac.cd_loc_cidade > 0 && item.enderecoRelac.cd_loc_bairro > 0 && item.enderecoRelac.cd_loc_estado > 0 &&
                                item.enderecoRelac.cd_loc_logradouro > 0 && item.enderecoRelac.cd_tipo_logradouro > 0)
                                    relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            }
                            relacionamentos.Add(relac);
                        }
                        else
                        {
                            if (item.pessoaJuridicaRelac != null && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.no_pessoa) && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.dc_num_cgc))
                            {
                                item.pessoaJuridicaRelac.cd_tipo_sociedade = 1;
                                item.pessoaJuridicaRelac.nm_natureza_pessoa = (int)PessoaSGF.TipoPessoa.JURIDICA;
                                relac.PessoaFilho = item.pessoaJuridicaRelac;
                                if (item.enderecoRelac != null && item.enderecoRelac.cd_loc_cidade > 0 && item.enderecoRelac.cd_loc_bairro > 0 && item.enderecoRelac.cd_loc_estado > 0 &&
                                item.enderecoRelac.cd_loc_logradouro > 0 && item.enderecoRelac.cd_tipo_logradouro > 0)
                                    relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            }
                            relacionamentos.Add(relac);
                        }

                    }
                }
                bool isMasterGeral = retornaUserMaster();
                var editEscola = Business.editEscola(escolaUI, relacionamentos, cdEsc, isMasterGeral, fullPath);

                if(editEscola != null)
                {
                    if (editEscola.parametro != null)
                    {
                        editEscola.parametro = null;
                        

                    }
                }

                if (editEscola.cd_pessoa <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                {
                    retorno.retorno = editEscola;
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                return new RenderJsonActionResult { Result = retorno };

            }
            catch (EscolaBusinessException ex)
            {
                if (ex.tipoErro == EscolaBusinessException.TipoErro.ERRO_HORARIO_OCUPADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    EscolaBusinessException fx = new EscolaBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
                }
            }
        }

        [HttpGet]
        [MvcComponentesAuthorize(Roles = "usu")]
        public RenderJsonActionResult getEscolaGrupo(int cd_grupo, int tipoGrupo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                //if (Session["EscolasUsuario"] == null)
                //{
                //    retorno.AddMensagem(Componentes.Utils.Messages.Messages.msgSessaoExpirada, "", Componentes.GenericController.ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                //    return new RenderJsonActionResult { Result = new { erro = retorno } };
                //}
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                bool masterGeral = bool.Parse(Session["MasterGeral"] + "");
                if (!masterGeral && tipoGrupo == (int)SysGrupo.TipoGrupo.GRUPO_MASTER)
                    throw new PermissaoBusinessException(Componentes.Utils.Messages.Messages.msgErrorAutorizacao, null, PermissaoBusinessException.TipoErro.ERRO_PERMISSAO_NEGADA, false);
                var escolas = BusinessEmpresa.getEmpresaGrupo(cd_grupo);

                retorno.retorno = escolas;
                return new RenderJsonActionResult { Result = retorno };

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Traz os registros pela descrição da Entidade
        [MvcComponentesAuthorize(Roles = "esc")]
        public RenderJsonActionResult getEscolaSearchFKFollowUp(String nome, string fantasia, bool inicio, string cnpj, int? cd_estado, int? cd_cidade)
        {
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                bool idMasterGeral = false;
                List<EmpresaSession> escolas = new List<EmpresaSession>();
                if (Session["EscolasUsuario"] == null || ((List<EmpresaSession>)Session["EscolasUsuario"]).Count() == 0)
                    idMasterGeral = true;
                else
                    escolas = (List<EmpresaSession>)Session["EscolasUsuario"];
                if (nome == null)
                    nome = String.Empty;
                if (cnpj == null)
                    cnpj = String.Empty;
                if (fantasia == null)
                    fantasia = String.Empty;
                 var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                var retorno = Business.getSearchEscolasFKFollowUp(parametros, nome, cnpj, fantasia, inicio, escolas.Select(x => x.cd_pessoa).ToList(), idMasterGeral, cd_estado, cd_cidade);
                return new RenderJsonActionResult { Result = retorno, parameters = parametros };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        #endregion

        #region Usuario

        [HttpGet]
        [MvcComponentesAuthorize(Roles = "usu")]
        public RenderJsonActionResult getAllUsuarioByEscola(bool? ativo, int? cdGrupo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                var admGeral = false;

                if (Session["EscolasUsuario"] == null)
                {
                    retorno.AddMensagem(Componentes.Utils.Messages.Messages.msgSessaoExpirada, "", Componentes.GenericController.ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return new RenderJsonActionResult { Result = new { erro = retorno } };
                }

                if (((bool)Session["IdMaster"]) && ((List<EmpresaSession>)Session["EscolasUsuario"]).Count == 0)
                    admGeral = true;

                int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
                int codUsuario = int.Parse(Session["CodUsuario"] + "");
                var usuarios = BusinessEmpresa.findUsuarioByEmpresaLogin(cdEscola, codUsuario, admGeral, ativo, cdGrupo);

                retorno.retorno = usuarios;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [MvcComponentesAuthorize(Roles = "usu")]
        public ActionResult GetUsuarioSearch(string descricao, string nome, bool inicio, int status, int escola, bool pesqSysAdmin)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                IPessoaBusiness BusinessPessoa = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                base.configuraBusiness(new List<IGenericBusiness>() { Business, BusinessPessoa });
                bool master = (bool)Session["IdMaster"];
                string login = Session[@"UserName"].ToString();
                List<EmpresaSession> escolasativas = (List<EmpresaSession>)Session["EscolasUsuario"];
                List<EmpresaSession> escolasinativas = (List<EmpresaSession>)Session["EscolasUsuarioInativas"];
                List<EmpresaSession> escolas = escolasativas.Union(escolasinativas).ToList();
                int[] codEscolas = null;
                int i = 0;
                if (escolas.Count() > 0)
                {
                    int[] EscolasUsuario = new int[escolas.Count()];
                    foreach (var c in escolas)
                    {
                        EscolasUsuario[i] = c.cd_pessoa;
                        i++;
                    }
                    codEscolas = EscolasUsuario;
                }
                if (nome == null) nome = String.Empty;
                if (descricao == null) descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                var sysAdmin = BusinessUsuario.verificarSysAdmin(login);
                var usu = BusinessUsuario.GetUsuarioSearch(parametros, descricao, nome, inicio, getStatus(status), login, codEscolas, escola, master, sysAdmin, pesqSysAdmin);

                return new RenderJsonActionResult { Result = usu, parameters = parametros };
            }
            catch (NullReferenceException nre)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, nre);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [MvcComponentesAuthorize(Roles = "usu")]
        public ActionResult getUsuarioSearchFKFollowUp(string descricao, string nome, bool inicio, int tipoPesq)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
                int codUsuario = int.Parse(Session["CodUsuario"] + "");
                string login = Session[@"UserName"].ToString();
                List<EmpresaSession> escolas = (List<EmpresaSession>)Session["EscolasUsuario"];
                int[] codEscolas = null;
                int i = 0;
                if (escolas.Count() > 0)
                {
                    int[] EscolasUsuario = new int[escolas.Count()];
                    foreach (var c in escolas)
                    {
                        EscolasUsuario[i] = c.cd_pessoa;
                        i++;
                    }
                    codEscolas = EscolasUsuario;
                }
                if (nome == null) nome = String.Empty;
                if (descricao == null) descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                var usu = Business.getUsuarioSearchFKFollowUp(parametros, descricao, nome, inicio, cdEscola, codUsuario, tipoPesq, login, codEscolas);

                return new RenderJsonActionResult { Result = usu, parameters = parametros };
            }
            catch (NullReferenceException nre)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, nre);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [MvcComponentesAuthorize(Roles = "usu")]
        public ActionResult getUsuarioSearchFK(string descricao, string nome, bool inicio)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
                if (nome == null) nome = String.Empty;
                if (descricao == null) descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                var usu = BusinessUsuario.getUsuarioSearchFK(parametros, descricao, nome, inicio, cdEscola);

                return new RenderJsonActionResult { Result = usu, parameters = parametros };
            }
            catch (NullReferenceException nre)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, nre);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpGet]
        [MvcComponentesAuthorize(Roles = "usu")]
        public ActionResult getUsuarioSearchGeralFK(string descricao, string nome, bool inicio)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
                if (nome == null) nome = String.Empty;
                if (descricao == null) descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                var usu = BusinessUsuario.getUsuarioSearchGeralFK(parametros, descricao, nome, inicio, cdEscola);

                return new RenderJsonActionResult { Result = usu, parameters = parametros };
            }
            catch (NullReferenceException nre)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, nre);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpGet]
        [MvcComponentesAuthorize(Roles = "usu")]
        public ActionResult getUsuarioSearchAtendenteFK(string descricao, string nome, bool inicio)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
                if (nome == null) nome = String.Empty;
                if (descricao == null) descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                var usu = BusinessUsuario.getUsuarioSearchAtendenteFK(parametros, descricao, nome, inicio, cdEscola);

                return new RenderJsonActionResult { Result = usu, parameters = parametros };
            }
            catch (NullReferenceException nre)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, nre);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "usu")]
        public ActionResult GetUrlRelatorioUsuario(string sort, int direction, string descricao, string nome, bool inicio, int status, int escola, bool pesqSysAdmin)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                List<EmpresaSession> escolas = (List<EmpresaSession>)Session["EscolasUsuario"];
                bool master = (bool)Session["IdMaster"];
                string login = Session[@"UserName"].ToString();
                var sysAdmin = BusinessUsuario.verificarSysAdmin(login);
                //if(Session["IdMaster"] != null && (bool) Session["IdMaster"])
                //    escolas = BusinessEmpresa.findAllEmpresaSession();

                var stringEscolas = "";
                if (escolas.Count() > 0)
                {
                    foreach (var c in escolas)
                    {
                        stringEscolas = stringEscolas + ", " + c.cd_pessoa;
                    }
                    stringEscolas = stringEscolas.Substring(1, stringEscolas.Length - 1);
                }
                // Configura as escolas na sessão do api:
                //FundacaoFisk.SGF.Services.InstituicaoEnsino.Controllers.EscolaController controller = new FundacaoFisk.SGF.Services.InstituicaoEnsino.Controllers.EscolaController(Business, BusinessPessoa);
                //controller.ConfiguraSessaoEscolas(codEscolas);

                //Se o usuário for master, pega as escolas do sistema:

                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@login=" + User.Identity.Name + "&@descricao=" + descricao + "&@nomePessoa=" + nome + "&@inicio=" + inicio + "&@status=" + status + "&@escola=" + escola + "&@cdEsc=" + stringEscolas + "&@isMaster=" + master + "&@sysAdmin=" + sysAdmin + "&@pesqSysAdmin=" + pesqSysAdmin + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Usuário&" +
                    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.UsuarioSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                var parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                retorno.retorno = parametrosCript;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        #endregion

        #region GrupoSGF

        [HttpGet]
        [MvcComponentesAuthorize(Roles = "usu")]
        public RenderJsonActionResult getGruposEscolasUsuario(int cdUsuario)
        {
            ReturnResult retorno = new ReturnResult();
            FundacaoFisk.SGF.Web.Services.Empresa.Model.GruposEscolasUsuario gEscUser = new FundacaoFisk.SGF.Web.Services.Empresa.Model.GruposEscolasUsuario();
            try
            {
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IPermissaoBusiness BusinessPermissoes = (IPermissaoBusiness)base.instanciarBusiness<IPermissaoBusiness>();
                int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
                var escolas = BusinessEmpresa.findAllEmpresaByUsuario(cdUsuario);
                var grupos = BusinessPermissoes.getSysGrupoByCdUsuario(cdUsuario, cdEscola);
                if (escolas.Count() > 0)
                    gEscUser.empresas = escolas.ToList();
                if (grupos.Count() > 0)
                    gEscUser.sysGrupoSGF = grupos.ToList();

                retorno.retorno = gEscUser;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        #endregion

        #region item - utilizamos essa chamada por causa das escolas do usuario master geral, que precisa trazer todas as escolas do sistema.

        //Retorna um boleano, se o usuario é master
        private bool retornaUserMaster()
        {
            IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
            string login = Session["loginUsuario"].ToString();
            var isMaster = BusinessEmpresa.VerificarMasterGeral(login);
            return isMaster;
        }
        #endregion

        #region Movimento

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        [MvcComponentesAuthorize(Roles = "mvtc,mvts")]
        public ActionResult postGerarTitulosProcessarNF(int cd_movimento, byte id_tipo_movimento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                configuraBusiness(new List<IGenericBusiness>() { Business });
                bool empresaPropria = Business.getEmpresaPropria(cd_escola);



                Movimento mov = Business.gerarTitulosMovimentoPostNf(cd_escola, cd_movimento, id_tipo_movimento);


                retorno.retorno = mov;

                return new RenderJsonActionResult { Result = retorno };
            }
            catch (FiscalBusinessException exe)
            {
                if (exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_PROCESSAR_NF ||
                    exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_NOTAS_POSTERIORES_PROCESSADOS ||
                    exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_PROCESSAR_MOVIMENTO_COM_ITEM_ZERADO ||
                    exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_PROCESSAR_MOVIMENTO_SEM_ITEM ||
                    exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_NOTAS_ANTERIORES_EM_ABERTAS ||
                    exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_CHAVE_ACESSO_NF)

                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgErrorProccessNF, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgErrorProccessNF, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit.i, tit.a")]
        [MvcComponentesAuthorize(Roles = "mvtc,mvts")]
        [HttpPost]
        public ActionResult postProcessarNF(Movimento movimento) 
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                configuraBusiness(new List<IGenericBusiness>() { Business });
                bool empresaPropria = Business.getEmpresaPropria(cd_escola);


                //Business.realizaMovimento(movimento);

                MovimentoUI processou = Business.processarNFMovimento(cd_escola, movimento.cd_movimento, empresaPropria, movimento);
                if (processou != null && processou.cd_movimento > 0)
                {
                    var parms = "cd_escola=" + cd_escola + "&cd_movimento=" + movimento.cd_movimento + "&id_tipo_movimento=" + movimento.id_tipo_movimento +
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                    string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                    parametros += "&enderecoWeb=" + Session["enderecoWeb"];
                    processou.url_relatorio = enderecoRelatorioWeb + "relatorio/emitirNF?" + parametros;
                    retorno.AddMensagem(Messages.msgProccessNFSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);


                    if (empresaPropria)
                        if (processou.envio_masterSaf_empresa_propira)
                        {
                            retorno.AddMensagem(Messages.msgRegEnviadoSucessoMSF, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                        }
                        else
                            retorno.AddMensagem(Messages.msgErroEnviarRegistroMSF, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                else
                    retorno.AddMensagem(Messages.msgErrorProccessNF, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                retorno.retorno = processou;

                return new RenderJsonActionResult { Result = retorno };
            }
            catch (FiscalBusinessException exe)
            {
                if (exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_PROCESSAR_NF ||
                    exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_NOTAS_POSTERIORES_PROCESSADOS ||
                    exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_PROCESSAR_MOVIMENTO_COM_ITEM_ZERADO ||
                    exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_PROCESSAR_MOVIMENTO_SEM_ITEM ||
                    exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_NOTAS_ANTERIORES_EM_ABERTAS ||
                    exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_CHAVE_ACESSO_NF)

                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgErrorProccessNF, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgErrorProccessNF, retorno, logger, ex);
            }
        }
        #endregion

        #region Relatórios
        [MvcComponentesAuthorize(Roles = "cnabb")]
        public ActionResult getImprimirTitulosBoletos(string cd_titulos_cnab)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                string enderecoRetorno = Session["enderecoWeb"] + "";
                bool id_mostrar_3_boletos = Business.getImprimir3BoletosPagina(cd_escola);
                var parms = "cd_titulos_cnab=" + cd_titulos_cnab + "&cd_escola=" + cd_escola + "&id_mostrar_3_boletos=" + id_mostrar_3_boletos +
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                parametros += "&enderecoWeb=" + Session["enderecoWeb"];
                return new RenderJsonActionResult { Result = enderecoRelatorioWeb + "RelatorioApresentacao/VisualizadorTitulosBoleto?" + parametros };
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "cnabb")]
        public ActionResult getBaixarPDFTitulosBoletos(string cd_titulos_cnab)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                string enderecoRetorno = Session["enderecoWeb"] + "";
                bool id_mostrar_3_boletos = Business.getImprimir3BoletosPagina(cd_escola);
                var parms = "cd_titulos_cnab=" + cd_titulos_cnab + "&cd_escola=" + cd_escola + "&id_mostrar_3_boletos=" + id_mostrar_3_boletos +
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                parametros += "&enderecoWeb=" + Session["enderecoWeb"];
                return new RenderJsonActionResult { Result = enderecoRelatorioWeb + "RelatorioApresentacao/baixarPDFTitulosBoleto?" + parametros };
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado, retorno, logger, ex);
            }
        }

        [AllowAnonymous]
        public ActionResult getBaixarPDFTitulosBoletosPortalTeste(string cd_titulos_cnab, int cd_escola, bool id_mostrar_3_boletos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                var parms = "cd_titulos_cnab=" + cd_titulos_cnab + "&cd_escola=" + cd_escola + "&id_mostrar_3_boletos=" + id_mostrar_3_boletos +
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY_ECOMMERCE), System.Text.Encoding.UTF8);
                parametros += "&enderecoWeb=" + Session["enderecoWeb"];
                return new RenderJsonActionResult { Result = enderecoRelatorioWeb + "RelatorioApresentacao/baixarPDFTitulosBoletoPortal?" + parametros };
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "cnabb")]
        public ActionResult getVisualizarBoletos(string cd_cnab)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                string enderecoRetorno = Session["enderecoWeb"] + "";
                bool id_mostrar_3_boletos = Business.getImprimir3BoletosPagina(cd_escola);
                var parms = "cd_cnab=" + cd_cnab + "&cd_escola=" + cd_escola + "&id_mostrar_3_boletos=" + id_mostrar_3_boletos +
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                parametros += "&enderecoWeb=" + Session["enderecoWeb"];
                return new RenderJsonActionResult { Result = enderecoRelatorioWeb + "RelatorioApresentacao/VisualizadorBoleto?" + parametros };
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "cnabb")]
        public ActionResult getBaixarPDFBoletosCNAB(string cd_cnab)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                string enderecoRetorno = Session["enderecoWeb"] + "";
                bool id_mostrar_3_boletos = Business.getImprimir3BoletosPagina(cd_escola);
                var parms = "cd_cnab=" + cd_cnab + "&cd_escola=" + cd_escola + "&id_mostrar_3_boletos=" + id_mostrar_3_boletos +
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                parametros += "&enderecoWeb=" + Session["enderecoWeb"];
                return new RenderJsonActionResult { Result = enderecoRelatorioWeb + "RelatorioApresentacao/baixarPDFBoletosCNAB?" + parametros };
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "cnabb")]
        public ActionResult getBoletoEmail(string cd_cnab)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                SendEmail sendEmail = new SendEmail();
                SendEmail.configurarEmailSection(sendEmail);
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                string caminhoRelatorioWeb = "RelatorioApresentacao/getEnvioBoletoEmail?";
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                string no_escola = Session["NomeEscolaSelecionada"] + "";
                bool id_mostrar_3_boletos = Business.getImprimir3BoletosPagina(cd_escola);
                var parms = "cd_cnab=" + cd_cnab + "&cd_escola=" + cd_escola + "&no_escola=" + no_escola + "&host=" + sendEmail.host + "&porta=" + sendEmail.porta + "&ssl=" + sendEmail.ssl + "&remetente=" + sendEmail.remetente +
                    "&password=" + sendEmail.password + "&userName=" + sendEmail.userName + "&dominio=" + sendEmail.dominio + "&id_mostrar_3_boletos=" + id_mostrar_3_boletos +
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                try
                {
                    //using(HttpClient client = new HttpClient()) {
                    enderecoRelatorioWeb = enderecoRelatorioWeb.Replace("www.sgf.datawin.com.br", "localhost");
                    retorno.retorno = enderecoRelatorioWeb + caminhoRelatorioWeb + parametros;
                    /*
                        client.BaseAddress = new Uri(enderecoRelatorioWeb); //Uri("http://localhost:28509/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = client.GetAsync(caminhoRelatorioWeb + parametros).Result;

                        if(response.IsSuccessStatusCode)
                            retorno = (ReturnResult) JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(ReturnResult));
                        else {
                            logger.Error(response.StatusCode + " Url: " + enderecoRelatorioWeb + caminhoRelatorioWeb + parametros);
                            retorno.AddMensagem(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroConexaoRelatorioBoleto, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                        }*/

                    return new RenderJsonActionResult { Result = retorno };
                    //}
                }
                catch (Exception ex)
                {
                    throw new Exception(" Url: " + enderecoRelatorioWeb + caminhoRelatorioWeb + parametros, ex);
                }
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotEmailEnviado, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "cnabb")]
        public ActionResult getBoletoEmailTitulosCnab(string cd_titulos_cnab)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness Business = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                SendEmail sendEmail = new SendEmail();
                SendEmail.configurarEmailSection(sendEmail);
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                string caminhoRelatorioWeb = "RelatorioApresentacao/getEnvioBoletoEmailTitulosCnab?";
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                string no_escola = Session["NomeEscolaSelecionada"] + "";
                bool id_mostrar_3_boletos = Business.getImprimir3BoletosPagina(cd_escola);
                var parms = "cd_titulos_cnab=" + cd_titulos_cnab + "&cd_escola=" + cd_escola + "&no_escola=" + no_escola + "&host=" + sendEmail.host + "&porta=" + sendEmail.porta +
                            "&ssl=" + sendEmail.ssl + "&remetente=" + sendEmail.remetente +
                            "&password=" + sendEmail.password + "&userName=" + sendEmail.userName + "&dominio=" + sendEmail.dominio + "&id_mostrar_3_boletos=" + id_mostrar_3_boletos +
                            "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                try
                {
                    // using (HttpClient client = new HttpClient())
                    //{
                    enderecoRelatorioWeb = enderecoRelatorioWeb.Replace("www.sgf.datawin.com.br", "localhost");
                    retorno.retorno = enderecoRelatorioWeb + caminhoRelatorioWeb + parametros;

                    /*
                    client.BaseAddress = new Uri(enderecoRelatorioWeb); //Uri("http://localhost:28509/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.GetAsync(caminhoRelatorioWeb + parametros).Result;

                    if(response.IsSuccessStatusCode)
                        retorno = (ReturnResult) JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(ReturnResult));
                    else {
                        logger.Error(response.StatusCode + " Url: " + enderecoRelatorioWeb + caminhoRelatorioWeb + parametros);
                        retorno.AddMensagem(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroConexaoRelatorioBoleto, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    }*/

                    return new RenderJsonActionResult { Result = retorno };
                    //   }
                }
                catch (Exception ex)
                {
                    throw new Exception(" Url: " + enderecoRelatorioWeb + caminhoRelatorioWeb + parametros, ex);
                }
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotEmailEnviado, retorno, logger, ex);
            }
        }

        #endregion
    }
}
