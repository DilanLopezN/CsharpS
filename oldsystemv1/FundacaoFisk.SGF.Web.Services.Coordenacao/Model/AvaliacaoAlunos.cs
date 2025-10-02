using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class AvaliacaoAlunos
    {

        public enum Hierarquia
        {
            PAI = 1,
            FILHO = 0
        }
        public AvaliacaoAlunos()
        {
            children = new List<AvaliacaoAlunos>();
        }

        public int id { get; set; }
        public int idPai { get; set; }
        public int isChildren { get; set; }
        public string dc_avaliacao_turma { get; set; }
        public string dc_nome { get; set; }
        public double? vl_nota { get; set; }
        public string dc_observacao { get; set; }
        public string dc_nome_avaliador { get; set; }
        public int pai { get; set; }
        public bool isConceitoNota { get; set; }
        public double? notaMaxima { get; set; }
        public double? somaNotas { get; set; }
        public double? mediaNotas { get; set; }
        public Nullable<decimal> maximoNotaTurma { get; set; }
        public Nullable<decimal> peso { get; set; }
        public int cd_avaliacao { get; set; }
        public int cd_avaliacao_turma { get; set; }
        public int cd_aluno { get; set; }
        public int cd_avaliacao_aluno { get; set; }
        public Nullable<int> cd_funcionario { get; set; }
        public bool ativo { get; set; }
        public bool isConceito { get; set; }
        public int? cd_conceito { get; set; }
        public int cd_produto { get; set; }
        public int cd_tipo_avaliacao { get; set; }
        public int cd_criterio_avaliacao { get; set; }
        public Nullable<bool> id_segunda_prova { get; set; }
        public string dc_observacao_aux { get; set; }
        public Nullable<System.DateTime> dt_avaliacao_turma { get; set; }
        public Nullable<System.DateTime> dt_cadastro { get; set; }
        public Nullable<System.DateTime> dt_matricula { get; set; }
        public Nullable<System.DateTime> dt_desistencia { get; set; }
        public Nullable<System.DateTime> dt_movimento { get; set; }
        public Nullable<System.DateTime> dt_transferencia { get; set; }
        public Nullable<System.DateTime> dt_termino_turma { get; set; }
        public string dc_conceito { get; set; }
        public bool id_participacao { get; set; }
        public double? vl_nota_2 { get; set; }
        public bool isModifiedA { get; set; }
        public bool isModified { get; set; }
        public ICollection<AvaliacaoAlunos> children { get; set; }
        public ICollection<Participacao> participacoesDisponiveis { get; set; }
        public ICollection<AvaliacaoAlunoParticipacao> participacoesAluno { get; set; }

        public string dta_avaliacao_turma
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_avaliacao_turma);
            }
        }

        public string dta_matricula
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_matricula);
            }
        }

        public string dta_desistencia
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_desistencia);
            }
        }

        public string dta_movimento
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_movimento);
            }
        }

        public string dta_transferencia
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_transferencia);
            }
        }

        public string dta_termino_turma
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_termino_turma);
            }
        }


        public string maxNotaTurma
        {
            get
            {
                return string.Format("{0:#,0.0}", this.maximoNotaTurma);
            }
        }
        public string val_nota
        {
            get
            {
                return string.Format("{0:#,0.0}", this.vl_nota);
            }
        }

        public string val_nota_2
        {
            get
            {
                return string.Format("{0:#,0.0}", this.vl_nota_2);
            }
        }
    }
}
