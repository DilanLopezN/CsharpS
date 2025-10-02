using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Aluno
    {
        public string no_aluno { get; set; }
        public DateTime? dt_nascimento { get; set; }
        public enum SituacaoAluno
        {
            AguardMatricula = 1,
            Matriculado = 2,
            Desistente = 3,
            Encerrado = 4,
            ExAluno = 5
        }

        public int? cd_pessoa_cpf { get; set; }
        public string no_prospect { get; set; }

        public string txt_obs_pessoa { get; set; }
        public int cd_turma { get; set; }
        public int qtd_material_curso { get; set; }
        public PessoaFisicaSGF pessoaFisica { get; set; }
        public bool selecionadoAluno = false;
        public string nomeAluno { get; set; }
        public int? cd_contrato { get; set; }
        public DateTime? aluno_dt_nascimento { get; set; }

        public FichaSaude fichaSaudeAluno { get; set; }

        public int? idade
        {
            get
            {
                int? retorna = null;
                if (this.aluno_dt_nascimento != null)
                    retorna = DateTime.Now.Year - this.aluno_dt_nascimento.Value.Year;
                if (this.aluno_dt_nascimento != null &&
                    (DateTime.Now.Month < this.aluno_dt_nascimento.Value.Month ||
                    (DateTime.Now.Month == this.aluno_dt_nascimento.Value.Month && DateTime.Now.Day < this.aluno_dt_nascimento.Value.Day)))
                    return retorna - 1;
                else
                    return retorna;
            }
        }
        
        //Propriedades do relatório de diário de aula:
        public string idade_16
        {
            get
            {
                string retorno = null;
                if (this.aluno_dt_nascimento != null)
                {
                    int diferenca_idade = DateTime.UtcNow.Year - this.aluno_dt_nascimento.Value.Year;
                    if(diferenca_idade < 16)
                        retorno = "X";
                }
                return retorno;
            }
        }
        public byte? nm_faltas { get; set; }
        public decimal? pc_bolsa {
            get {
                if(this.Bolsa != null)
                    return this.Bolsa.pc_bolsa;
                else
                    return null;
            }
            set{}
        }
        public double? nm_nota_aluno { get; set; }
        public int? cd_tipo_avaliacao { get; set; }
        public String dc_tipo_avaliacao { get; set; }

        public ICollection<Curso> Cursos { get; set; }
        public ICollection<MotivoMatricula> MotivosMatricula { get; set; }
        public ICollection<MotivoNaoMatricula> MotivosNaoMatricula { get; set; }
        public ICollection<Horario> Horarios { get; set; }
        public string no_pessoa { get; set; }
        public bool id_prospect_ativo { get; set; }
        public Nullable<double> nm_carga { get; set; }
        public Nullable<int> nm_maxima { get; set; }

    }
}