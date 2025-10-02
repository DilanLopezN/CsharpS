using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Model
{
    using Componentes.GenericModel;
    public class FuncionarioSearchUI : TO
    {
        public FuncionarioSearchUI() { }
        public int cd_funcionario { get; set; }
        public int cd_pessoa_funcionario { get; set; }
        public int cd_pessoa { get; set; }
        public string no_pessoa { get; set; }
        public string no_fantasia { get; set; }
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
        public int? cd_atividade { get; set; }
        public string no_atividade { get; set; }
        public bool id_professor { get; set; }
        public string no_pessoa_dependente { get; set; }
        public int? cd_cargo { get; set; }
        public string des_cargo { get; set; }
        public IEnumerable<int> cursosHabilitacao { get; set; }
        public int cd_pessoa_escola { get; set; }
        public byte tipoFuncionario { get; set; }
        public byte? nm_sexo { get; set; }

        public string nm_cpf_dependente
        {
            get
            {
                string retorno = "";
                if (!String.IsNullOrEmpty(nm_cpf_cgc))
                {
                    if (!string.IsNullOrEmpty(no_pessoa_dependente))
                        retorno = nm_cpf_cgc + "(" + no_pessoa_dependente + ")";
                    else
                        retorno = nm_cpf_cgc;
                }
                return retorno;
            }
        }

        public FuncionarioSGF funcionario { get; set; }
        public PessoaFisicaUI pessoaFisicaUI { get; set; }

        public static FuncionarioSGF changeValueFuncionarioViewToContext(FuncionarioSGF funcionarioContext, FuncionarioSGF funcionarioView)
        {
            funcionarioContext.dt_admissao = funcionarioView.dt_admissao;
            funcionarioContext.dt_demissao = funcionarioView.dt_demissao;
            funcionarioContext.cd_pessoa_empresa = funcionarioView.cd_pessoa_empresa;
            funcionarioContext.id_professor = funcionarioView.id_professor;
            funcionarioContext.vl_salario = funcionarioView.vl_salario;
            funcionarioContext.id_comissionado = funcionarioView.id_comissionado;
            funcionarioContext.id_colaborador_cyber = funcionarioView.id_colaborador_cyber;
            funcionarioContext.id_funcionario_ativo = funcionarioView.id_funcionario_ativo;
            funcionarioContext.cd_cargo = funcionarioView.cd_cargo;
            //Salvando o nome do arquivo anterior para que seja deletado.
            if (funcionarioContext.nome_assinatura_certificado != funcionarioView.nome_assinatura_certificado)
                funcionarioContext.nome_assinatura_certificado_anterior = funcionarioContext.nome_assinatura_certificado;
            funcionarioContext.nome_assinatura_certificado = funcionarioView.nome_assinatura_certificado;
            return funcionarioContext;
        }

        public static FuncionarioSearchUI changeValueViewFuncionario(FuncionarioSGF funcionario)
        {
            FuncionarioSearchUI funcionarioSearchUI = new FuncionarioSearchUI();
            if (funcionario != null && funcionario.FuncionarioPessoaFisica != null)
            {
                funcionarioSearchUI.cd_funcionario = funcionario.cd_funcionario;
                funcionarioSearchUI.cd_pessoa_funcionario = funcionario.FuncionarioPessoaFisica.cd_pessoa;
                funcionarioSearchUI.no_pessoa = funcionario.FuncionarioPessoaFisica.no_pessoa;
                funcionarioSearchUI.dt_cadastramento = funcionario.FuncionarioPessoaFisica.dt_cadastramento;
                funcionarioSearchUI.dc_reduzido_pessoa = funcionario.FuncionarioPessoaFisica.dc_reduzido_pessoa;
                funcionarioSearchUI.nm_atividade_principal = funcionario.FuncionarioPessoaFisica.cd_atividade_principal;
                funcionarioSearchUI.nm_endereco_principal = funcionario.FuncionarioPessoaFisica.cd_endereco_principal;
                funcionarioSearchUI.nm_telefone_principal = funcionario.FuncionarioPessoaFisica.cd_telefone_principal;
                funcionarioSearchUI.nm_natureza_pessoa = funcionario.FuncionarioPessoaFisica.nm_natureza_pessoa;
                funcionarioSearchUI.dc_num_pessoa = funcionario.FuncionarioPessoaFisica.dc_num_pessoa;
                funcionarioSearchUI.nm_cpf_cgc = funcionario.FuncionarioPessoaFisica.nm_cpf;
                funcionarioSearchUI.id_funcionario_ativo = funcionario.id_funcionario_ativo;
                funcionarioSearchUI.ext_img_pessoa = funcionario.FuncionarioPessoaFisica.ext_img_pessoa;
                funcionarioSearchUI.no_atividade = funcionario.FuncionarioPessoaFisica.Atividade != null ? funcionario.FuncionarioPessoaFisica.Atividade.no_atividade : "";
                funcionarioSearchUI.id_professor = funcionario.id_professor;
            }
            return funcionarioSearchUI;
        }
      
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
               // retorno.Add(new DefinicaoRelatorio("cd_pessoa", "Código", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("no_pessoa", "Nome", AlinhamentoColuna.Left, "2.5000in"));
                retorno.Add(new DefinicaoRelatorio("nm_cpf_cgc", "CPF\\CNPJ", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("dc_reduzido_pessoa", "Nome Reduzido", AlinhamentoColuna.Left, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("dta_cadastro", "Data Cadastro", AlinhamentoColuna.Center, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("no_atividade", "Profissão", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("professor_ativo", "Professor", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("id_ativo", "Ativo", AlinhamentoColuna.Center, "0.8000in"));
                

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
