using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Model
{
    using Componentes.GenericModel;
    public partial class UsuarioUISearch : TO
    {
        public int cd_usuario { get; set; }
        public int? cd_pessoa { get; set; }
        public string no_login { get; set; }
        public string no_pessoa { get; set; }
        public bool id_master { get; set; }
        public bool id_usuario_ativo { get; set; }
        public bool id_bloqueado { get; set; }
        public bool id_admin { get; set; }
        public bool id_master_geral { get; set; }
        public bool id_administrador { get; set; }
        public string Master {
            get {
                string retorno = "";
                //if (id_admin)
                //    retorno = "Sim";
                //else
                    retorno = this.id_master ? "Sim" : "Não";
                return retorno;
            }
        }
        public string ativo {
            get {
                return this.id_usuario_ativo ? "Sim" : "Não";
            }
        }
        public string sysAdmin
        {
            get
            {
                return this.id_admin ? "Sim" : "Não";
            }
        }
        public string Administrador
        {
            get
            {
                return this.id_administrador ? "Sim" : "Não";
            }
        }

        public Boolean possui_varias_escolas = false;
        public virtual ICollection<Escola> Empresas { get; set; }
        public string dc_empresas {
            get {
                string empresas = String.Empty;

                if(this.Empresas != null && this.Empresas.Count() > 0)
                    foreach (Escola e in this.Empresas)
                    {
                        string nomeEmpresa = e.dc_reduzido_pessoa != null && e.dc_reduzido_pessoa != "" ? e.dc_reduzido_pessoa : e.no_pessoa;
                        empresas += nomeEmpresa + " | ";
                    }

                if(empresas.Length > 0)
                    empresas = empresas.Substring(0, empresas.Length - 3);

                return empresas;
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_usuario", "Código", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("no_login", "Usuário", AlinhamentoColuna.Left, "1.6000in"));
                retorno.Add(new DefinicaoRelatorio("no_pessoa", "Nome", AlinhamentoColuna.Left, "2.4000in"));
                if (possui_varias_escolas)
                    retorno.Add(new DefinicaoRelatorio("dc_empresas", "Escola(s)", AlinhamentoColuna.Left, "2.4000in"));
                retorno.Add(new DefinicaoRelatorio("ativo", "Ativo", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("Master", "Administrador", AlinhamentoColuna.Center, "1.2000in"));
                if (id_master_geral)
                    retorno.Add(new DefinicaoRelatorio("sysAdmin", "SysAdmin", AlinhamentoColuna.Center));
                return retorno;
            }
        }

        public static UsuarioUISearch fromUsuarioWeb(UsuarioWebSGF usuarioWebSGF) {
            UsuarioUISearch retorno = new UsuarioUISearch();

            //retorno.ativo = usuarioWeb.usuario_ativo;
            retorno.cd_pessoa = usuarioWebSGF.cd_pessoa;
            retorno.cd_usuario = usuarioWebSGF.cd_usuario;
            retorno.id_master = usuarioWebSGF.id_master;
            retorno.no_login = usuarioWebSGF.no_login;
            retorno.id_bloqueado = usuarioWebSGF.id_bloqueado;
            retorno.id_admin = usuarioWebSGF.id_admin;
            retorno.id_usuario_ativo = usuarioWebSGF.id_usuario_ativo;
            if (usuarioWebSGF.PessoaFisica != null)
                retorno.no_pessoa = usuarioWebSGF.PessoaFisica.no_pessoa;
            return retorno;
        }
    }
}
