using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Model {
    public class GrupoPermissao {
        public SysGrupo grupo { get; set; }
        public ICollection<Permissao> permissoes { get; set; }
        public ICollection<UsuarioWebSGF> usuariogrupo { get; set; }


        public static List<SysGrupo> montarGruposPermissoesFilhos(List<SysGrupo> sysGrupoFilhoBase, ICollection<Permissao> permissoes, string nome_grupo_pai)
        {
            List<SysGrupo> sysGrupoFilho = new List<SysGrupo>();
            foreach (var item in sysGrupoFilhoBase)
            {
                SysGrupo grupoFilho = new SysGrupo();
                grupoFilho.copy(item);
                grupoFilho.no_grupo = nome_grupo_pai;
                List<SysDireitoGrupo> listaGrupoFilhos = new List<SysDireitoGrupo>();
                grupoFilho.SysDireitoGrupo = MontaDireitosRecursivo(listaGrupoFilhos, permissoes, grupoFilho);
                sysGrupoFilho.Add(grupoFilho);
            }

            return sysGrupoFilho;
        }

        public static List<SysDireitoGrupo> MontaDireitosRecursivo(List<SysDireitoGrupo> listaGrupos, ICollection<Permissao> permissoesView, SysGrupo sysGrupo)
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

        private static void MontaPermissoesRecursivo(ICollection<SysMenu> menus, ICollection<Permissao> retorno, int pai)
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

    }
}
