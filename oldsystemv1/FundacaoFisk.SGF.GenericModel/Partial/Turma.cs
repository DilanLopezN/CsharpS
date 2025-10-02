using System;
using System.Collections.Generic;
using System.Globalization;
using Componentes.GenericModel;
using System.Data.Entity;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Turma : TO
    {
        public ICollection<Horario> horariosTurma { get; set; }
        public IEnumerable<AlunoTurma> alunosTurma { get; set; }
        public IEnumerable<AlunoTurma> alunosTurmaEscola { get; set; }
        public IEnumerable<AlunoTurma> situacoesAluno { get; set; }
        public IEnumerable<ProfessorTurma> ProfessorTurma { get; set; }
        public IEnumerable<Turma> alunosTurmasPPT { get; set; }
        public IEnumerable<AlunoTurmaPPT> alunosTurmasPPTSearch { get; set; }
        public bool atualizaHorarios { get; set; }
        public bool selecionadoTurma = false;
        public string no_sala { get; set; }
        public string no_sala_online { get; set; }
        public int? nm_vagas { get; set; }
        public string no_professor { get; set; }
        public int? cd_professor { get; set; }
        public DateTime? dta_termino_turma { get; set; }
        public byte sequencia { get; set; }
        public string no_produto {
            get {
                if(this.Produto != null)
                    return this.Produto.no_produto;
                else
                    return "";
            }
        }
        public int nm_aulas_dadas { get; set; }
        public byte? nm_faltas { get; set; }
        public int nm_aulas_contratadas { get; set; }
        public string no_turma_ppt { get; set; }
        public int? cd_situacao_aluno_turma { get; set; }
        public string no_curso { get; set; }
        public string no_duracao { get; set; }
        public bool titulos_atrazados { get; set; }
        public bool hasClickEscola { get; set; }
        public String dt_termino
        {
            get
            {
                if(this.dt_termino_turma.HasValue)
                    return String.Format("{0:dd/MM/yyyy}", dt_termino_turma);
                else
                   return "";
            }
            set {
                if (!String.IsNullOrEmpty(value))
                    this.dt_termino_turma = DateTime.Parse(value, new CultureInfo("pt-br", false));
                else
                    this.dta_termino_turma = null;
            }
        }

        public enum TipoTurma
        {
            TODAS = 0,
            NORMAL =1,
            PPT = 3
        }

        public enum TipoOnline
        {
            TODAS = 0,
            PRESENCIAL = 1,
            ONLINE = 2
        }

        public enum SituacaoTurma
        {
            TODAS = 0,
            TURMASEMANDAMENTO = 1,
            TURMASENCERRADAS = 2,
            TURMASFORMACAO = 3,
            TURMASAENCERRAR = 4,
            TURMASNOVA = 5
        }

        public enum SituacaoTurmaPPT {
            TURMASATIVAS = 1,
            TURMASINATIVAS = 2 
        }


        public enum SituacaoAlunoTurma : byte
        {
            MOVIDO = 0,
            ATIVO = 1,
            DESISTENTE = 2,
            TRANSFERIDO = 3,
            ENCERRADO = 4,
            DEPENDENTE = 5,
            REPROVADO = 6,
            REMANEJADO = 7,
            REMATRICULADO = 8,
            AGUARDANDO = 9
        }

        public string situacaoAlunoTurma
        {
            get
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("pt-BR");

                string situacaoAlunoT = "";
                switch (cd_situacao_aluno_turma)
                {
                    case (int)SituacaoAlunoTurma.MOVIDO: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.MOVIDO.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.ATIVO: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.ATIVO.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.DESISTENTE: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.DESISTENTE.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.TRANSFERIDO: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.TRANSFERIDO.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.ENCERRADO: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.ENCERRADO.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.DEPENDENTE: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.DEPENDENTE.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.REPROVADO: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.REPROVADO.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.REMANEJADO: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.REMANEJADO.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.REMATRICULADO: situacaoAlunoT = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.REMATRICULADO.ToString()).ToLower());
                        break;
                    case (int)SituacaoAlunoTurma.AGUARDANDO: situacaoAlunoT = "Aguardando Mat.";//culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(SituacaoAlunoTurma.AGUARDANDO.ToString()).ToLower());
                        break;
                }
                return situacaoAlunoT;
            }
        }

        public string dta_inicio_aula
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_inicio_aula);
            }
        }

        public string dta_final_aula
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_final_aula);
            }
        }

        public static Turma changeValueTurma(Turma turmaContext, Turma turmaView)
        {
            turmaContext.no_turma = turmaView.no_turma;
            turmaContext.no_apelido = turmaView.no_apelido;
            turmaContext.cd_curso = turmaView.cd_curso;
            turmaContext.cd_duracao = turmaView.cd_duracao;
            turmaContext.cd_produto = turmaView.cd_produto;
            turmaContext.cd_regime = turmaView.cd_regime;
            turmaContext.cd_sala = turmaView.cd_sala;
            turmaContext.dt_inicio_aula = turmaView.dt_inicio_aula;
            turmaContext.dt_final_aula = turmaView.dt_final_aula;
            turmaContext.id_turma_ppt = turmaView.id_turma_ppt;
            turmaContext.id_aula_externa = turmaView.id_aula_externa;
            turmaContext.id_turma_ativa = turmaView.id_turma_ativa;
			turmaContext.nro_aulas_programadas = turmaView.nro_aulas_programadas;
            turmaContext.nm_turma = turmaView.nm_turma > 0 ? turmaView.nm_turma : turmaContext.nm_turma;
            turmaContext.cd_turma_enc = turmaView.cd_turma_enc;
            turmaContext.cd_sala_online = turmaView.cd_sala_online;
            return turmaContext;
        }

        public static Turma changeValueRetornView(Turma turmaContext)
        {
            if (turmaContext.horariosTurma != null)
            {
                foreach (Horario h in turmaContext.horariosTurma)
                    if (h.HorariosProfessores != null)
                        foreach (HorarioProfessorTurma hp in h.HorariosProfessores)
                            hp.Horario = null;
            }
            if (turmaContext.alunosTurmasPPTSearch != null)
                foreach (AlunoTurmaPPT at in turmaContext.alunosTurmasPPTSearch)
                    if (at.horariosTurma != null)
                        foreach (Horario h in at.horariosTurma)
                            if (h.HorariosProfessores != null)
                                foreach (HorarioProfessorTurma hp in h.HorariosProfessores)
                                    hp.Horario = null;


            return turmaContext;
        }
    }



    public class AlunoTurmaPPT
    {
        public int cd_turma { get; set; }
        public int cd_duracao { get; set; }
        public int? cd_aluno { get; set; }
        public int? cd_pessoa_aluno { get; set; }
        public int? cd_curso { get; set; }
        public int? cd_regime { get; set; }
        public string no_turma { get; set; }
        public string no_apelido { get; set; }
        public string no_aluno { get; set; }
        public string no_curso { get; set; }
        public string no_regime { get; set; }
        public bool id_aluno_ativo { get; set; }
        public DateTime? dt_termino_turma { get; set; }
        public System.DateTime dt_inicio_aula { get; set; }
        public Nullable<System.DateTime> dt_final_aula { get; set; }
        public bool alterouProgramacaoOuDescFeriado { get; set; }
        public AlunoTurma alunoTurma { get; set; }
        public ICollection<ProgramacaoTurma> ProgramacaoTurma { get; set; }
        public ICollection<FeriadoDesconsiderado> FeriadosDesconsiderados { get; set; }
        public IEnumerable<Horario> horariosTurma { get; set; }
        public IEnumerable<ProfessorTurma> ProfessorTurma { get; set; }
        public DateTime? dt_cadastramento { get; set; }
        public short nro_aulas_programadas { get; set; }
        public Nullable<decimal> pc_bolsa { get; set; }
        public Nullable<decimal> pc_bolsa_material { get; set; }
        public Nullable<DateTime> dt_inicio_bolsa { get; set; }
        public decimal? vl_abatimento_matricula { get; set; }
        public string dc_reduzido_pessoa_escola { get; set; }
        public int cd_pessoa_escola { get; set; }
        public int id { get; set; }
        public string dta_cadastro {
            get {
                if(dt_cadastramento != null && dt_cadastramento.HasValue)
                    return String.Format("{0:dd/MM/yyyy}", dt_cadastramento.Value.ToLocalTime());
                else
                    return String.Empty;
            }
        }
        public string dta_termino_turma
        {
            get
            {
                if (dt_termino_turma != null && dt_termino_turma.HasValue)
                    return String.Format("{0:dd/MM/yyyy}", dt_termino_turma);
                else
                    return String.Empty;
            }
        }
        public string dta_inicio_aula
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_inicio_aula);
            }
        }

        public string dta_final_aula
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_final_aula);
            }
        }

        public string situacaoAlunoTurmaFilhaPPT
        {
            get
            {
                string retorno = "";
                if (this.alunoTurma != null)
                    retorno = this.alunoTurma.situacaoAlunoTurma;
                    return retorno;
            }
        }

    }

    public class AlunoTurmaEscolas
    {
        public int cd_turma { get; set; }
        public int? cd_aluno { get; set; }
        public int? cd_situacao_aluno { get; set; }
        public int cd_pessoa_escola { get; set; }
    }
}
