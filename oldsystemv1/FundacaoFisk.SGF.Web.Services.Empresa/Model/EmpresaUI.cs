using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Model
{
    using Componentes.GenericModel;
    public class EmpresaUI : TO
    {
        //Campos obrigatórios para ordenar na grade
        public int cd_pessoa { get; set; }
        public string no_pessoa { get; set; }
        public String dc_fone_email { get; set; }
        public bool id_pessoa_ativa { get; set; }
        public System.DateTime dt_cadastramento { get; set; }
        public String dc_num_cgc { get; set; }

        //Campos para retornar no select
        public double? pc_juros_dia { get; set; }
        public double? pc_multa { get; set; }
        public double? pc_taxa_dia_biblioteca { get; set; }
        public int? cd_operadora { get; set; }
        public Nullable<int> nm_cliente_integracao { get; set; }
        public Nullable<int> nm_escola_integracao { get; set; }
        public Nullable<System.TimeSpan> hr_inicial { get; set; }
        public Nullable<System.TimeSpan> hr_final { get; set; }
        public string ext_img_pessoa { get; set; }

        public PessoaJuridicaUI pessoaJuridica { get; set; }
        public int cd_grupo { get; set; }

        public static EmpresaUI fromEscolaUI(EmpresaUI empresa, string telefone, double? juros, double? multa, double? taxaBiblioteca)
        {
            EmpresaUI empresaUI = new EmpresaUI
            {
                cd_pessoa = empresa.cd_pessoa,
                no_pessoa = empresa.no_pessoa,
                dt_cadastramento = empresa.dt_cadastramento,
                dc_fone_email = telefone,
                dc_num_cgc = empresa.dc_num_cgc,
                id_pessoa_ativa = empresa.id_pessoa_ativa,
                pc_juros_dia = juros,
                pc_multa = multa,
                pc_taxa_dia_biblioteca = taxaBiblioteca,
                hr_inicial = empresa.hr_inicial,
                hr_final = empresa.hr_final,
                ext_img_pessoa = empresa.ext_img_pessoa
            };
            return empresaUI;
        }

        // Métodos auxiliares para formatação
        public string escola_ativa
        {
            get
            {
                return this.id_pessoa_ativa ? "Sim" : "Não";
            }
        }

        public string juros
        {
            get
            {
                if (this.pc_juros_dia == null)
                    return "";
                return string.Format("{0,00}", this.pc_juros_dia);
            }
        }
        public string multa
        {
            get
            {
                if (this.pc_multa == null)
                    return "";
                return string.Format("{0,00}", this.pc_multa);
            }
        }

        public string taxaBiblioteca
        {
            get
            {
                if (this.pc_taxa_dia_biblioteca == null)
                    return "";
                return string.Format("{0,00}", this.pc_taxa_dia_biblioteca);
            }
        }

        public string dt_cadastro
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_cadastramento);
            }
        }

        public string hr_cadastro
        {
            get
            {
                return String.Format("{0:HH:mm:ss}", dt_cadastramento.ToLocalTime());
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_pessoa", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_pessoa", "Nome", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("escola_ativa", "Ativa", AlinhamentoColuna.Center));
                return retorno;
            }
        }
    }

    public class EmpresaUIUsuario
    {
        public int cd_pessoa { get; set; }
        public string dc_reduzido_pessoa { get; set; }
        private String _nomePessoa = String.Empty;
        public string nm_cpf_cgc { get; set; }
        public DateTime dt_cadastramento { get; set; }

        public string no_pessoa { 
            get{
                if (this.dc_reduzido_pessoa != null)
                    return this.dc_reduzido_pessoa;
                else
                    return this._nomePessoa;
            } 
            set{
                _nomePessoa = value;
            } 
        }

        public string nm_cpf_cgc_dependente
        {
            get
            {
                string retorno = "";
                if (!String.IsNullOrEmpty(nm_cpf_cgc))
                    retorno = nm_cpf_cgc;
                return retorno;
            }
        }

        public string dta_cadastro
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_cadastramento);
            }
        }
        
        public ICollection<SysGrupo> Grupos { get; set; }
    }

    public class PesquisaEmpresaUI 
    { 
        public List<int> empresas { get; set; }
        public string nome { get; set; }
        public string fantasia { get; set; }
        public string cnpj { get; set; }
        public bool inicio { get; set; }
    }

    public class GruposEscolasUsuario
    {
        public List<Escola> empresas { get; set; }
        public List<SysGrupo> sysGrupoSGF { get; set; }
    }
}
