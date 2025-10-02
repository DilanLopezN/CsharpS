using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    using Componentes.GenericModel;
    public class FuncionarioSearchUI : TO
    {
        public FuncionarioSearchUI() { }
        public int cd_funcionario { get; set; }
        public int cd_pessoa_funcionario { get; set; }
        public string no_pessoa { get; set; }
        public System.DateTime dt_cadastramento { get; set; }
        public string dc_reduzido_pessoa { get; set; }
        public Nullable<int> nm_atividade_principal { get; set; }
        public Nullable<int> nm_endereco_principal { get; set; }
        public Nullable<int> nm_telefone_principal { get; set; }
        public Nullable<byte> nm_natureza_pessoa { get; set; }
        public string dc_num_pessoa { get; set; }
        public string nm_cpf_cgc { get; set; }
        public bool id_funcionario_ativo { get; set; }
        public string ext_img_pessoa { get; set; }
        public string no_atividade { get; set; }
        public bool id_professor { get; set; } 

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
               // retorno.Add(new DefinicaoRelatorio("cd_pessoa", "Código", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("no_pessoa", "Nome", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("dc_reduzido_pessoa", "Nome Reduzido"));
                retorno.Add(new DefinicaoRelatorio("nm_cpf_cgc", "CPF\\CNPJ", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("id_ativo", "Ativo", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("professor_ativo", "Professor", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("dta_cadastro", "Data Cadastro"));

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

        public string id_ativo
        {
            get
            {
                return this.id_funcionario_ativo ? "Sim" : "Não";
            }
        }

        public string professor_ativo
        {
            get
            {
                return this.id_professor ? "Sim" : "Não";
            }
        }
    }
}
