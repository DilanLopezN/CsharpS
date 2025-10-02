using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using Componentes.Utils;
using Newtonsoft.Json;
//using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using log4net;
using FundacaoFisk.SGF.Utils;
using Componentes.Utils.Messages;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Controllers
{
    using Componentes.ApresentadorRelatorio;
    using System.Net.Http;
    using System.Net;
    using Componentes.GenericController;
    using Componentes.GenericBusiness.Comum;
    using FundacaoFisk.SGF.Web.Services.Usuario.Business;
    using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
    
    using FundacaoFisk.SGF.Web.Services.Empresa.Model;

	public class PermissaoController : ComponentesMVCController
    {
        //Declaração de constantes
        const int CONFIGURACAO = 1, CADASTROSBASICOS = 5;

        private static readonly ILog logger = LogManager.GetLogger(typeof(PermissaoController));
        
        //
        // GET: /Usuario/

        public ActionResult Index()
        {
            return View();
        }
        // GET: /UsuarioSenha/

        public ActionResult UsuarioSenha()
        {
            return View();
        }
        public ActionResult Grupo()
        {
            return View();
        }

        public PermissaoController()
        {
        }

        [HttpGet]
        public RenderJsonActionResult GetMenusUsuario() {
            ReturnResult retorno = new ReturnResult();

            try {
                if (Session["codUsuario"] == null)                {
                    retorno.AddMensagem(Messages.msgSessaoExpirada, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return new RenderJsonActionResult { Result = new { erro = retorno } };
                }
                else
                {
                    IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                    IPermissaoBusiness BusinessPermissao = (IPermissaoBusiness)base.instanciarBusiness<IPermissaoBusiness>();
                    int cd_escola = (int)Session["CodEscolaSelecionada"];
                    bool empresa_propria = BusinessEmpresa.getEmpresaPropria(cd_escola);
                    MenuUI menuUI = (MenuUI)Session["menusUsuario"] != null ? (MenuUI)Session["menusUsuario"] : new MenuUI();
                    // var menus = (IEnumerable<SysMenu>)Session["menusUsuario"];
                    menuUI.empresaPropria = empresa_propria;
                    if (menuUI.menuPrincipal == null && menuUI.menuConfiguracao == null && menuUI.menuCadastrosBasicos == null )
                    {
                        IEnumerable<SysMenuUI> menus = BusinessPermissao.GetMenusUsuario((int?)Session["CodEscolaSelecionada"], (int)Session["codUsuario"], (bool)Session["IdMaster"]);
                        for (int i = 0; i < menus.ToArray().Length; i++)
                        {
                            if (menus.ToList()[i].cd_menu != CADASTROSBASICOS && menus.ToList()[i].cd_menu != CONFIGURACAO)
                                menuUI.menuPrincipal = (from mn in menus
                                                        where mn.cd_menu != CADASTROSBASICOS
                                                              && mn.cd_menu != CONFIGURACAO
                                                        select mn).ToList();
                            if (menus.ToList()[i].cd_menu == CONFIGURACAO)
                                menuUI.menuConfiguracao = (from mn in menus
                                                           where mn.cd_menu == CONFIGURACAO
                                                           select mn).ToList();
                            if (menus.ToList()[i].cd_menu == CADASTROSBASICOS)
                                menuUI.menuCadastrosBasicos = (from mn in menus
                                                               where mn.cd_menu == CADASTROSBASICOS
                                                               select mn).ToList();
                        }
                        Session["menusUsuario"] = menuUI;
                    }

                    menuUI.nomeUsuario = Session["loginUsuario"] + "";
                    menuUI.nomeEscolaLogada = Session["NomeEscolaSelecionada"] + "";
                    List<EmpresaSession> empresas = (List<EmpresaSession>)Session["EscolasUsuario"];
                    List<EmpresaUsuarioSession> empresasUsuario = new List<EmpresaUsuarioSession>();

                    if ((empresas == null || empresas.Count == 0) && Session["EscolasUsuarioMaster"] != null)
                        empresas = (List<EmpresaSession>)Session["EscolasUsuarioMaster"];

                    if(empresas != null && empresas.Count > 1)
                        foreach (EmpresaSession empresaSession in empresas)
                        {
                            EmpresaUsuarioSession empresaUsuarioSession = new EmpresaUsuarioSession();

                            empresaUsuarioSession.copy(empresaSession);
                            empresasUsuario.Add(empresaUsuarioSession);
                        }
                    menuUI.empresasUsuario = empresasUsuario;
                    menuUI.codEscolaLogada = (int) Session["CodEscolaSelecionada"];

                    retorno.retorno = menuUI;

                    if(menuUI.menuPrincipal == null && menuUI.menuConfiguracao == null && menuUI.menuCadastrosBasicos == null)
                        retorno.AddMensagem(Messages.msgUsuarioSemPermissoes, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    else if((menuUI.menuPrincipal != null && menuUI.menuPrincipal.Count() <= 0) && 
                            (menuUI.menuCadastrosBasicos != null && menuUI.menuCadastrosBasicos.Count() <= 0) &&
                            (menuUI.menuConfiguracao != null && menuUI.menuConfiguracao.Count() <= 0))
                        retorno.AddMensagem(Messages.msgUsuarioSemPermissoes, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);

                    return new RenderJsonActionResult { Result = Newtonsoft.Json.JsonConvert.SerializeObject(retorno) };
                }
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgErrorMenu, retorno, logger, ex);
            }
        }
       
        #region Grupo
        [MvcComponentesAuthorize(Roles = "gru")]
        public ActionResult GeturlrelatorioGrupo(string sort, int direction, string descricao, bool inicio, int tipo)
        {
            int cdEsc = int.Parse(Session["CodEscolaSelecionada"] + "");
            ReturnResult retorno = new ReturnResult();
            //FundacaoFisk.SGF.Web.Services.Usuario.Controllers.PermissaoController controller = new FundacaoFisk.SGF.Web.Services.Usuario.Controllers.PermissaoController(BusinessPermissao);
            //controller.ConfiguraSessaoEscola(cdEsc);
            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&@cdEsc=" + cdEsc + "&@tipo=" + tipo +  "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Grupo&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.GrupoSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                var parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                retorno.retorno = parametrosCript;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgErrorMenu, retorno, logger, ex);
            }
        }

        private List<SysDireitoGrupo> MontaDireitosRecursivo(List<SysDireitoGrupo> listaGrupos, ICollection<Permissao> permissoesView, SysGrupo sysGrupo)
        {
            // Transforma os dados de permissões para entidades de negócio:
            var permissoes = permissoesView.ToList();
            for (int i = 0; i < permissoes.Count; i++)
            {
                if (permissoes[i].visualizar)
                {
                    SysDireitoGrupo sysGrupoDireito = new SysDireitoGrupo();
                    sysGrupoDireito.cd_grupo = sysGrupo.cd_grupo;
                    sysGrupoDireito.cd_menu = permissoes[i].id;
                    sysGrupoDireito.id_alterar_grupo = permissoes[i].alterar;
                    sysGrupoDireito.id_inserir_grupo = permissoes[i].incluir;
                    sysGrupoDireito.id_excluir_grupo = permissoes[i].excluir;

                    listaGrupos.Add(sysGrupoDireito);
                }
                if (permissoes[i].children.Count > 0)
                    MontaDireitosRecursivo(listaGrupos, permissoes[i].children, sysGrupo);
            }

            return listaGrupos;
        }

        [HttpPost]
        [MvcComponentesAuthorize(Roles = "gru.i")]
        public RenderJsonActionResult PostGrupo(GrupoPermissao grupoPermissoes)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPermissaoBusiness BusinessPermissao = (IPermissaoBusiness)base.instanciarBusiness<IPermissaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { BusinessPermissao });
                
                //GrupoPermissao grupoPermissoes = (GrupoPermissao) JsonConvert.DeserializeObject(data);
                SysGrupo sysGrupo = grupoPermissoes.grupo;
                List<Permissao> permissoes = grupoPermissoes.permissoes.ToList<Permissao>();
                List<SysDireitoGrupo> listaGrupos = new List<SysDireitoGrupo>();
                sysGrupo.SysDireitoGrupo = MontaDireitosRecursivo(listaGrupos, permissoes, sysGrupo);
                
                //
                if (grupoPermissoes.usuariogrupo != null)
                {
                    ICollection<SysGrupoUsuario> usuariosGrupo = new List<SysGrupoUsuario>();
                    foreach (UsuarioWebSGF u in grupoPermissoes.usuariogrupo)
                    {
                        SysGrupoUsuario usuGrupo = new SysGrupoUsuario
                        {
                            cd_grupo = grupoPermissoes.grupo.cd_grupo,
                            cd_usuario = u.cd_usuario
                        };
                        usuariosGrupo.Add(usuGrupo);
                    }
                    sysGrupo.Usuarios = usuariosGrupo;
                }
                sysGrupo.cd_pessoa_empresa = int.Parse(Session["CodEscolaSelecionada"]+"");
                sysGrupo = BusinessPermissao.PostGrupo(sysGrupo);
                retorno.retorno = sysGrupo;
                if (sysGrupo.cd_grupo <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "gru.a")]
        public RenderJsonActionResult PostAlterarGrupo(GrupoPermissao grupoPermissoes)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPermissaoBusiness BusinessPermissao = (IPermissaoBusiness)base.instanciarBusiness<IPermissaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { BusinessPermissao });
                grupoPermissoes.grupo.cd_pessoa_empresa = int.Parse(Session["CodEscolaSelecionada"] + "");

                SysGrupo sysGrupo = grupoPermissoes.grupo;
                List<Permissao> permissoes = new List<Permissao>();
                if (grupoPermissoes.permissoes != null)
                    permissoes = grupoPermissoes.permissoes.ToList<Permissao>();

                List<SysDireitoGrupo> listaGrupos = new List<SysDireitoGrupo>();

                ICollection<SysGrupoUsuario> usuariosGrupo = new List<SysGrupoUsuario>();
                foreach (UsuarioWebSGF u in grupoPermissoes.usuariogrupo)
                {
                    SysGrupoUsuario usuGrupo = new SysGrupoUsuario
                    {
                        cd_grupo = grupoPermissoes.grupo.cd_grupo,
                        cd_usuario = u.cd_usuario
                    };
                    usuariosGrupo.Add(usuGrupo);

                }
                sysGrupo.Usuarios = usuariosGrupo;


                sysGrupo.SysDireitoGrupo = MontaDireitosRecursivo(listaGrupos, permissoes, sysGrupo);
                sysGrupo.alteraDireito = grupoPermissoes.grupo.alteraDireito;
                BusinessPermissao.PutGrupo(sysGrupo);
                var ge = sysGrupo;
                retorno.retorno = ge;
                if (sysGrupo.cd_grupo <= 0)
                    retorno.AddMensagem(Messages.msgNotUpDateReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }

        [HttpPost]
        [MvcComponentesAuthorize(Roles = "grmtr.i")]
        public RenderJsonActionResult inserirGrupoEscola(GrupoPermissao grupoPermissoes)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                //configuraBusiness(new List<IGenericBusiness>() { BusinessPermissao });

                //monta as permissões para o grupos
                IPermissaoBusiness BusinessPermissao = (IPermissaoBusiness)base.instanciarBusiness<IPermissaoBusiness>();
                List<Permissao> permissoes = grupoPermissoes.permissoes.ToList<Permissao>();
                List<SysDireitoGrupo> listaGrupos = new List<SysDireitoGrupo>();

                SysGrupo sysGrupo = grupoPermissoes.grupo;
           
                sysGrupo.SysDireitoGrupo = MontaDireitosRecursivo(listaGrupos, permissoes, sysGrupo);

                ICollection<SysGrupo> sysGrupoFilho = grupoPermissoes.grupo.SysGrupoFilho;
                
                foreach (var item in sysGrupoFilho) {
                    item.id_atualizar_grupo = true;
                    List<SysDireitoGrupo> listaGrupoFilhos = new List<SysDireitoGrupo>();
                    item.SysDireitoGrupo = MontaDireitosRecursivo(listaGrupoFilhos, permissoes, item);
                }
          
                sysGrupo = BusinessPermissao.postGrupoEscolas(sysGrupo);
                retorno.retorno = sysGrupo;
                if (sysGrupo.cd_grupo <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "grmtr.a")]
        public RenderJsonActionResult alterarGrupoEscola(GrupoPermissao grupoPermissoes)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
               // configuraBusiness(new List<IGenericBusiness>() { BusinessPermissao });
                IPermissaoBusiness BusinessPermissao = (IPermissaoBusiness)base.instanciarBusiness<IPermissaoBusiness>();
                grupoPermissoes.grupo.cd_pessoa_empresa = int.Parse(Session["CodEscolaSelecionada"] + "");
                SysGrupo sysGrupo = grupoPermissoes.grupo;
                List<Permissao> permissoes = new List<Permissao>();

                if (grupoPermissoes.permissoes != null)
                    permissoes = grupoPermissoes.permissoes.ToList<Permissao>();
               
                List<SysDireitoGrupo> listaGrupos = new List<SysDireitoGrupo>();

                sysGrupo.SysDireitoGrupo = MontaDireitosRecursivo(listaGrupos, permissoes, sysGrupo);
                sysGrupo.alteraDireito = grupoPermissoes.grupo.alteraDireito;

                BusinessPermissao.editarGrupoEscola(sysGrupo, permissoes);

                sysGrupo.SysGrupoFilho = null;
                sysGrupo.SysDireitoGrupo = null;

                retorno.retorno = sysGrupo;
                if (sysGrupo.cd_grupo <= 0)
                    retorno.AddMensagem(Messages.msgNotUpDateReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }

        }

        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "grmtr.e")]
        public RenderJsonActionResult deletarGrupoEscola(List<SysGrupo> grupos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPermissaoBusiness BusinessPermissao = (IPermissaoBusiness)base.instanciarBusiness<IPermissaoBusiness>();
                bool deletado = BusinessPermissao.deleteGrupoEscola(grupos);
                retorno.retorno = deletado;
                if (!deletado)
                    retorno.AddMensagem(Messages.msgNotExcludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }

        }

        [MvcComponentesAuthorize(Roles = "gru")]
        public ActionResult GetGrupoSearch(string descricao, bool inicio, int tipoGrupo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPermissaoBusiness BusinessPermissao = (IPermissaoBusiness)base.instanciarBusiness<IPermissaoBusiness>();
                bool masterGeral = bool.Parse(Session["MasterGeral"] + "");
                if (!masterGeral && tipoGrupo == (int)SysGrupo.TipoGrupo.GRUPO_MASTER)
                    throw new PermissaoBusinessException(Messages.msgErrorAutorizacao, null, PermissaoBusinessException.TipoErro.ERRO_PERMISSAO_NEGADA, false);
                if (descricao == null)
                    descricao = String.Empty;

                int cdEsc = int.Parse(Session["CodEscolaSelecionada"] + "");
                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                IEnumerable<SysGrupo> ret = BusinessPermissao.GetGrupoSearch(parametros, descricao, inicio, cdEsc, tipoGrupo);
                return new RenderJsonActionResult { Result = ret, parameters = parametros };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        //Get api para trazer todos grupos da escola selecionado(logado)
        [MvcComponentesAuthorize(Roles = "gru")]
        public ActionResult GetGrupo()
        {
            IPermissaoBusiness BusinessPermissao = (IPermissaoBusiness)base.instanciarBusiness<IPermissaoBusiness>();
            ReturnResult retorno = new ReturnResult();
            try {
                int cdEsc = int.Parse(Session["CodEscolaSelecionada"] + "");
                IEnumerable<SysGrupo> ret = BusinessPermissao.GetGrupoSearch(cdEsc);
                return new RenderJsonActionResult { Result = ret };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion
    }
}