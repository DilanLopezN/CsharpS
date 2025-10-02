using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;

namespace FundacaoFisk.SGF.Services.InstituicaoEnsino.Model
{
    public class UsuarioUI
    {
        public enum TipoPesquisaUsuarioEnum
        {
            HAS_USUARIO_ORIGEM = 1,
            HAS_USUARIO_DESTINO = 2,
            HAS_USUARIO_MASTER = 3,
            HAS_USUARIOS_ATIVOS_ESCOLA = 4,
            HAS_USUARIOS_CAD_PROSPECT_ALUNO = 10,
        }
        public string nm_pessoa_usuario { get; set; }
        public string nm_cpf { get; set; }
        public Nullable<byte> nm_sexo { get; set; }
        public ICollection<Permissao> permissoes { get; set; }
        public ICollection<Escola> escolas { get; set; }
        public ICollection<SysGrupo> gruposUsuario { get; set; }
        public List<Horario> horarios { get; set; }
        public bool atualizouPermissoes { get; set; }
        public bool atualizouEscolasOrGrupos { get; set; }
        public bool atualizouHorarios { get; set; }
        public UsuarioWebSGF usuarioWeb { get; set; }
        public string email { get; set; }

        //Metdo para gerar os direitos do usúario sysAdmin.
        public static List<SysDireitoUsuario> montarDiretirosSysAdmin(int cd_usuario)
        {
            List<SysDireitoUsuario> retorno = new List<SysDireitoUsuario>();
            int[] cdMenus = new int[] { (int)SysMenu.TipoSysMenu.CONFIGURACAO, (int)SysMenu.TipoSysMenu.ALTERARSENHA, (int)SysMenu.TipoSysMenu.USUARIO,  (int)SysMenu.TipoSysMenu.PESSOARELACIONADA,
            (int)SysMenu.TipoSysMenu.GRUPO, (int)SysMenu.TipoSysMenu.EMPRESA};
            foreach (int cd_menu in cdMenus)
            {
                if (cd_menu != (int)SysMenu.TipoSysMenu.PESSOARELACIONADA && cd_menu != (int)SysMenu.TipoSysMenu.GRUPO && cd_menu != (int)SysMenu.TipoSysMenu.EMPRESA)
                    retorno.Add(new SysDireitoUsuario
                    {
                        cd_menu = cd_menu,
                        cd_usuario = cd_usuario,
                        id_alterar = true,
                        id_deletar = true,
                        id_inserir = true
                    });
                else
                    retorno.Add(new SysDireitoUsuario
                    {
                        cd_menu = cd_menu,
                        cd_usuario = cd_usuario,
                        id_alterar = false,
                        id_deletar = false,
                        id_inserir = false
                    });
            }
            return retorno;
        }

        public UserAreaRestritaUI userAreaRestrita { get; set; }

    }
}
