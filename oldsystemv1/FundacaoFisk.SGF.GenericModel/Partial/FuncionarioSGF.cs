using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class FuncionarioSGF
    {
        public enum TipoConsultaFuncionarioEnum
        {
            HAS_ATIVO = 0,
            HAS_COMISSIONADO = 1
        }
        public string nome_temp_assinatura_certificado { get; set; }
        public string nome_assinatura_certificado_anterior { get; set; }

        public string no_pessoa { get; set; }

        public string dta_admissao
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_admissao);
            }
        }

        public string dta_demissao
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_demissao);
            }
        }

        public string vlSalario
        {
            get
            {
                if (this.vl_salario == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_salario);
            }
        }

        public enum TipoFuncionario
        {
            TODOS = 0,
            FUNCIONARIO = 1,
            PROFESSOR = 2
        }

        public enum TipoAlunoFuncionario
        {
            
            ALUNO = 0,
            PROFESSOR = 1,
            COORDENADOR = 2,
            COLABORADOR = 3,
            NENHUM = 4,
            FUNCIONARIO = 5
        }

        public static FuncionarioSGF formFuncionario(FuncionarioSGF funcionarioSGF)
        {
            FuncionarioSGF funcionario = new FuncionarioSGF();
            funcionario.cd_funcionario = funcionarioSGF.cd_funcionario;
            funcionario.cd_pessoa_funcionario = funcionarioSGF.cd_pessoa_funcionario;
            funcionario.dt_admissao = funcionarioSGF.dt_admissao;
            funcionario.dt_demissao = funcionarioSGF.dt_demissao;
            funcionario.id_comissionado = funcionarioSGF.id_comissionado;
            funcionario.id_professor = funcionarioSGF.id_professor;
            funcionario.id_funcionario_ativo = funcionarioSGF.id_funcionario_ativo;
            funcionario.cd_pessoa_empresa = funcionarioSGF.cd_pessoa_empresa;
            funcionario.FuncionarioPessoaFisica.copy(funcionarioSGF.FuncionarioPessoaFisica);
            funcionario.FuncionarioAtividade.copy(funcionarioSGF.FuncionarioAtividade);
            //funcionario.PessoaFisica = funcionarioSGF.FuncionarioPessoaFisica;
            

        //     public virtual PessoaFisicaSGF FuncionarioPessoaFisica { get; set; }
        //public virtual ICollection<AtividadeExtra> AtividadeExtraFuncionario { get; set; }
        //public virtual ICollection<AvaliacaoTurma> AvaliacaoTurma { get; set; }
        //public virtual Escola Empresa { get; set; }


            return funcionario;
        }
    }
}
