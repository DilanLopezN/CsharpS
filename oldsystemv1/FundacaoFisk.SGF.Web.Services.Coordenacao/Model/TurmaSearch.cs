using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    enum SituacaoAlunoTurma : byte
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

    enum Turno : byte
    {
        MANHA = 1,
        TARDE = 2,
        NOITE = 3
    }

    public class TurmaView
    {
        public Turma turma { get; set; }
    }

    public class TurmaSearch : TO
    {
       public int cd_pessoa_escola { get; set; }
       public int cd_turma  {get;set;}
       public int? cd_turma_enc { get; set; }
       public int? cd_turma_ppt { get; set; }
       public Nullable<int> cd_curso { get; set; }
       public Nullable<int> cd_estagio { get; set; }
       public Nullable<int> cd_regime { get; set; }
       public int cd_produto { get; set; }
       public int cd_duracao { get; set; }
       public string no_turma  {get;set;}
       public string no_turma_ppt { get; set; }
       public string no_apelido { get; set; }
       public string  no_curso {get;set;}
       public string no_duracao {get;set;}
       public string no_produto {get;set;}
       public DateTime dt_inicio_aula {get;set;}
       public DateTime? dt_final_aula { get; set; }
       public Nullable<System.DateTime> dta_termino_turma { get; set; }
       public string no_professor {get;set;}
       public int cd_professor { get; set; }
       public bool id_turma_ppt { get; set; }
       public int id_tipo_turma { get; set; }
       public bool? considera_vagas { get; set; }
       public int vagas_disponiveis { get; set; }
       public int nro_alunos { get; set; }
       public int? cd_situacao_aluno_turma { get; set; }
       public string no_aluno { get; set; }
       public bool turmasFilhas { get; set; }
       public bool alunoAguardando { get; set; }
       public bool id_turma_ativa { get; set; }
       public int? cd_aluno_turma { get; set; }
       public int? cd_contrato { get; set; }
       public string no_sala { get; set; }
       public int? cd_sala { get; set; }
       public string ultima_aula {get;set;}
       public bool existe_diario { get; set; }
       public FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI alunoSearchUI { get; set; }
       public IEnumerable<PessoaSGF> pessoasTurma { get; set; }
       public int nm_alunos_turmaPPT { get; set; }
       public int qtd_devedores { get; set; }
       public int qtd_bolsistas { get; set; }
       public int qtd_matriculas { get; set; }
       public IEnumerable<Horario> horarios { get; set; }
       public string horario { get; set; }
       public string dias { get; set; }
       public int turno { get; set; }
       public int? tipo_online { get; set; }
       public DateTime? dt_ultima_aula { get; set; }
       public CursoContratoSearch CursoContrato { get; set; }
       public string dc_escola_origem { get; set; }

        public string desc_turno 
       {
           get
           {
               var _desc = string.Empty;
               if (turno == (int)Turno.MANHA)
                   _desc = "Manhã";
               if (turno == (int)Turno.TARDE)
                   _desc = "Tarde";
               if (turno == (int)Turno.NOITE)
                   _desc = "Noite";

               return _desc;
           }
           set { }
       }

       public int nm_vagas
       {
           get
           {
               int retorno = 0;
               int qtd_alunos = nro_alunos;
               if (turmasFilhas)
                   qtd_alunos = nm_alunos_turmaPPT;
               if (vagas_disponiveis > 0 || qtd_alunos > 0)
               {
                   if (vagas_disponiveis > 0)
                       if (qtd_alunos > 0)
                           retorno = vagas_disponiveis - qtd_alunos;
                       else
                           retorno = vagas_disponiveis;
               }
               return retorno;
           }
           set { }
       }
       public string descTipoTurma
       {
           get
           {
               var  descTypeTurma = "Regular";
               if ((cd_turma_ppt != null && id_turma_ppt == false) || (cd_turma_ppt == null && id_turma_ppt == true))
                   descTypeTurma = "Personalizada";
               return descTypeTurma;
           }
           set { }
       }

       public string situacao
       {
           get
           {
               var situacaoTurma = "";
               if (!id_turma_ppt)
               {
                   if (existe_diario && dta_termino_turma == null)
                      situacaoTurma = "Turma em Andamento";
                   if (!existe_diario && dta_termino_turma == null)
                       situacaoTurma = "Turma em Formação";
                   if (dta_termino_turma != null)
                       situacaoTurma = "Turma Encerrada";
               }
               else
               {
                   situacaoTurma = "Turma ativa";
                   if(!id_turma_ativa)
                       situacaoTurma = "Turma inativa";
               }
               return situacaoTurma;
           }
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

       public string dtaIniAula
       {
           get
           {
               if (dt_inicio_aula != null)
                   return String.Format("{0:dd/MM/yyyy}", dt_inicio_aula);
               else
                   return String.Empty;
           }
       }

       public string dtaUltimaAula
       {
           get
           {
               if (dt_ultima_aula != null)
                   return String.Format("{0:dd/MM/yyyy}", dt_ultima_aula);
               else
                   return String.Empty;
           }
       }

       public string dtaFim
       {
           get
           {
               if (dt_final_aula != null)
                   return String.Format("{0:dd/MM/yyyy}", dt_final_aula);
               else
                   return String.Empty;
           }
       }

       public string dtaTermino
       {
           get
           {
               if (dt_inicio_aula != null)
                   return String.Format("{0:dd/MM/yyyy}", dta_termino_turma);
               else
                   return String.Empty;
           }
       }

       public List<DefinicaoRelatorio> ColunasRelatorio
       {
           get
           {
               List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

               // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
               //retorno.Add(new DefinicaoRelatorio("cd_criterio_avaliacao", "Código"));
               retorno.Add(new DefinicaoRelatorio("no_turma", "Turma", AlinhamentoColuna.Left, "2.5000in"));
               retorno.Add(new DefinicaoRelatorio("no_apelido", "Apelido", AlinhamentoColuna.Left, "1.4000in"));
               if (!turmasFilhas)
                   retorno.Add(new DefinicaoRelatorio("no_curso", "Curso", AlinhamentoColuna.Left, "1.4000in"));
               else
                   retorno.Add(new DefinicaoRelatorio("no_aluno", "Aluno", AlinhamentoColuna.Left, "1.8000in"));
               retorno.Add(new DefinicaoRelatorio("no_duracao", "Carga Horária", AlinhamentoColuna.Left, "1.1000in"));
               retorno.Add(new DefinicaoRelatorio("no_produto", "Produto", AlinhamentoColuna.Left, "1.1000in"));
               retorno.Add(new DefinicaoRelatorio("no_professor", "Professor",AlinhamentoColuna.Left, "1.2000in"));
               retorno.Add(new DefinicaoRelatorio("dtaIniAula", "Data Início", AlinhamentoColuna.Center));
               retorno.Add(new DefinicaoRelatorio("situacao", "Situação", AlinhamentoColuna.Center, "1.5000in"));
               retorno.Add(new DefinicaoRelatorio("nro_alunos", "Nro. Alunos", AlinhamentoColuna.Center, ".9000in"));
               retorno.Add(new DefinicaoRelatorio("dc_ano_escolar", "Ano Escolar", AlinhamentoColuna.Center, ".9000in"));// Analisar em quais relatorios vai aparecer para não mostrar em relatorio diferente do necessario
               return retorno;
           }
       }

    }
}

