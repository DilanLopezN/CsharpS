using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class AlunoTurma
    {
        public enum SituacaoAlunoTurma
        {
             Reprovado = 6, 
             Aguardando = 9, 
             Ativo      = 1, 
             Dependente = 5, 
             Desistente = 2, 
             Encerrado  = 4, 
             Movido     = 0, 
             Remanejado = 7, 
             Rematriculado = 8,
             Transferido   = 3,
             MatriculadosMaterial = 10,
             Cancelado = 11
        }

        public enum FiltroSituacaoAlunoTurma 
        {
            Todos = 0,
            Nao_Encerrado = 1
        }
        public string dc_reduzido_pessoa_escola { get; set; }
        public int cd_pessoa_escola_aluno { get; set; }
        public DateTime dt_cadastramento { get; set; }
        public string dta_cadastro {
            get {
                if(dt_cadastramento != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_cadastramento.ToLocalTime());
                else
                    return String.Empty;
            }
        }
        public string dta_movimento
        {
            get
            {
                if (dt_movimento != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_movimento.Value.ToLocalTime());
                else
                    return String.Empty;
            }
        }
        public int cd_pessoa_aluno { get; set; }
        public string no_aluno { get; set; }
        public bool id_aluno_ativo { get; set; }
        public string dc_situacao { get; set; }
        public int? frequencia { get; set; }
        public Nullable<decimal> pc_bolsa { get; set; }
        public Nullable<decimal> pc_bolsa_material { get; set; }
        public Nullable<DateTime> dt_inicio_bolsa { get; set; }
        public PessoaFisicaSGF PessoaSGFQueUsoOCpf { get; set; }
        public decimal? vl_abatimento_matricula { get; set; }

        public string situacaoAlunoTurma
        {
            get
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("pt-BR");

                string situacaoAlunoT = "";
                switch (cd_situacao_aluno_turma)
                {
                    case (int)SituacaoAlunoTurma.Movido: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Movido.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Ativo: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Ativo.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Desistente: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Desistente.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Transferido: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Transferido.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Encerrado: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Encerrado.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Dependente: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Dependente.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Reprovado: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Reprovado.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Remanejado: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Remanejado.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Rematriculado: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Rematriculado.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Aguardando: situacaoAlunoT = "Aguardando Mat.";//culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Aguardando.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.MatriculadosMaterial:
                        situacaoAlunoT = "Matriculado s/Mat.";//culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Aguardando.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Cancelado:
                        situacaoAlunoT = "Matrícula Cancelada";//culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Aguardando.ToString()).ToLower());
                        break;
                }
                return situacaoAlunoT;
            }
        }

        public static string getSituacaoAlunoTurma(int? cd_situacao_aluno_turma)
        {
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("pt-BR");
            string situacaoAlunoT = "";
            if (cd_situacao_aluno_turma.HasValue)
                switch (cd_situacao_aluno_turma)
                {
                    case (int)SituacaoAlunoTurma.Movido: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Movido.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Ativo: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Ativo.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Desistente: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Desistente.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Transferido: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Transferido.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Encerrado: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Encerrado.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Dependente: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Dependente.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Reprovado: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Reprovado.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Remanejado: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Remanejado.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Rematriculado: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Rematriculado.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Aguardando: situacaoAlunoT = "Aguardando Mat.";//culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Aguardando.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.MatriculadosMaterial:
                        situacaoAlunoT = "Matriculado s/Mat.";//culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Aguardando.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.Cancelado:
                        situacaoAlunoT = "Matrícula Cancelada";//culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.Aguardando.ToString()).ToLower());
                        break;
                }
            return situacaoAlunoT;
        }
    }
}
