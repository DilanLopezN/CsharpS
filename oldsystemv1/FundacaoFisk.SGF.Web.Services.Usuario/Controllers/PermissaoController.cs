using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using Componentes.Utils;
using Componentes.GenericController;
using Newtonsoft.Json;
using Componentes.Utils.Messages;
//using Componentes.ApresentadorRelatorio;
using log4net;

using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericBusiness.Comum;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Controllers
{
    public class PermissaoController : ComponentesApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PermissaoController));

        public PermissaoController()
        {
        }
        
        #region Permissoes
        [HttpComponentesAuthorize(Roles = "usu")]
        public HttpResponseMessage GetFuncionalidadesUsuarioArvore(int? cdUsuario)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPermissaoBusiness Business = (IPermissaoBusiness)base.instanciarBusiness<IPermissaoBusiness>();
                List<SysMenu> permissoesList = Business.GetFuncionalidadesUsuarioArvore(cdUsuario);

                // Transforma o objeto de banco em objeto da view:
                List<Permissao> listaPermissoes = new List<Permissao>();
                MontaPermissoesRecursivo(permissoesList, listaPermissoes, 0);
                retorno.retorno = listaPermissoes;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch(Exception ex) {
                logger.Error(ex);
                retorno.AddMensagem(Messages.msgErrorPermision, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }
        }
       
        private void MontaPermissoesRecursivo(ICollection<SysMenu> menus, ICollection<Permissao> retorno, int pai)
        {
            Permissao permissoes = new Permissao();
            List<SysMenu> menusList = menus.ToList();

            for (int i = 0; i < menusList.Count; i++)
            {
                List<SysDireitoUsuario> listaDireitosUsuario = menusList[i].Direitos.ToList();

                permissoes = new Permissao();
                permissoes.id = menusList[i].cd_menu;
                permissoes.ehPermitidoEditar = menusList[i].id_permissao_visivel;
                permissoes.pai = pai;
                permissoes.visualizar = false;
                permissoes.permissao = menusList[i].no_menu;
                permissoes.incluir = permissoes.excluir = permissoes.alterar = false;
                for (int u = 0; u < listaDireitosUsuario.Count; u++)
                {
                    permissoes.visualizar = true;
                    permissoes.incluir = permissoes.incluir || listaDireitosUsuario[u].id_inserir;
                    permissoes.excluir = permissoes.excluir || listaDireitosUsuario[u].id_deletar;
                    permissoes.alterar = permissoes.alterar || listaDireitosUsuario[u].id_alterar;
                }
                MontaPermissoesRecursivo(menusList[i].MenusFilhos, permissoes.children, permissoes.id);

                retorno.Add(permissoes);
            }
        }

        private void MontaPermissoesRecursivoGrupo(ICollection<SysMenu> menus, ICollection<Permissao> retorno, int pai)
        {
            Permissao permissoes = new Permissao();
            List<SysMenu> menusList = menus.ToList();

            for (int i = 0; i < menusList.Count; i++)
            {

                List<SysDireitoGrupo> listaDireitosGrupo = menusList[i].SysDireitosGrupos.ToList();
                //List<SysDireitoGrupo> listaDireitosGrupo = BusinessPermissao.ListaDireitos(menusList[i]);
                permissoes = new Permissao();
                permissoes.id = menusList[i].cd_menu;
                permissoes.ehPermitidoEditar = menusList[i].id_permissao_visivel;
                permissoes.pai = pai;
                permissoes.visualizar = false;
                permissoes.permissao = menusList[i].no_menu;
                permissoes.incluir = permissoes.excluir = permissoes.alterar = false;
                if (listaDireitosGrupo != null && listaDireitosGrupo.Count > 0)
                    permissoes.cd_direito_grupo = listaDireitosGrupo.Where(s => s.cd_menu == menusList[i].cd_menu).FirstOrDefault().cd_direito_grupo;
                for (int u = 0; u < listaDireitosGrupo.Count; u++)
                {
                    permissoes.visualizar = true;
                    permissoes.incluir = permissoes.incluir || listaDireitosGrupo[u].id_inserir_grupo;
                    permissoes.excluir = permissoes.excluir || listaDireitosGrupo[u].id_excluir_grupo;
                    permissoes.alterar = permissoes.alterar || listaDireitosGrupo[u].id_alterar_grupo;
                    permissoes.cd_direito_grupo = listaDireitosGrupo[u].cd_direito_grupo;
                }
                MontaPermissoesRecursivoGrupo(menusList[i].MenusFilhos, permissoes.children, permissoes.id);

                retorno.Add(permissoes);
            }
        }

        #endregion

        #region Grupo

        [HttpComponentesAuthorize(Roles = "gru")]
        public HttpResponseMessage GetFuncionalidadesGrupoArvore(int? cdGrupo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPermissaoBusiness Business = (IPermissaoBusiness)base.instanciarBusiness<IPermissaoBusiness>();
                List<SysMenu> permissoesList = Business.GetFuncionalidadesGrupoArvore(cdGrupo);
                // Transforma o objeto de banco em objeto da view:
                List<Permissao> listaPermissoes = new List<Permissao>();
                MontaPermissoesRecursivoGrupo(permissoesList, listaPermissoes, 0);
                retorno.retorno = listaPermissoes;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retorno.AddMensagem(Messages.msgErrorPermision, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }
        }

        [HttpComponentesAuthorize(Roles = "gru")]
        [HttpComponentesAuthorize(Roles = "usu")]
        public HttpResponseMessage getUsuarioByGrupo(int cdGrupo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                List<UsuarioWebSGF> usuarioGrupo = BusinessUsuario.findUsuarioByGrupo(cdEscola, cdGrupo);

                retorno.retorno = usuarioGrupo;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retorno.AddMensagem(Messages.msgErrorPermision, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }
        }


        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "gru.e")]
        public HttpResponseMessage PostDeleteGrupo(List<SysGrupo> grupos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IPermissaoBusiness Business = (IPermissaoBusiness)base.instanciarBusiness<IPermissaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                bool deletado = Business.DeleteGrupo(grupos);
                retorno.retorno = deletado;
                if (!deletado)
                    retorno.AddMensagem(Messages.msgNotExcludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                logger.Error("PermissaoController PostDeleteGrupo - Erro: " + ex.Message + ex.StackTrace + ex.InnerException);
                retorno.AddMensagem(Messages.msgNotExcludReg, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }

        }
        #endregion
    }
}