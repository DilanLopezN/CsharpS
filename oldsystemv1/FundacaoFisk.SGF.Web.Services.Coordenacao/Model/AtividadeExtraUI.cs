using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class AtividadeExtraUI : TO
    {
        public int cd_atividade_extra { get; set; }
        public System.DateTime dt_atividade_extra { get; set; }
        public System.TimeSpan hh_inicial { get; set; }
        public System.TimeSpan hh_final { get; set; }
        public string no_tipo_atividade_extra { get; set; }
        public string no_curso { get; set; }
        public string no_produto { get; set; }
        public string no_responsavel { get; set; }
        public Nullable<int> nm_vagas { get; set; }
        public Nullable<int> nm_alunos { get; set; }
        public int? cd_sala { get; set; }
        public string no_sala { get; set; }
        public int cd_usuario_atendente { get; set; }
        public string no_usuario { get; set; }
        public int cd_escola { get; set; }
        public int cd_tipo_atividade_extra { get; set; }
        public Nullable<int> cd_curso { get; set; }
        public List<int> cd_cursos { get; set; }
        public Nullable<int> cd_produto { get; set; }
        public int cd_funcionario { get; set; }
        public bool ind_carga_horaria { get; set; }
        public bool ind_pagar_professor { get; set; }
        public string tx_obs_atividade { get; set; }
        public int cd_pessoa_escola { get; set; }
        public bool id_calendario_academico { get; set; }
        public Nullable<System.TimeSpan> hr_limite_academico { get; set; }
        public Nullable<int> cd_atividade_recorrencia { get; set; }
        public AtividadeRecorrencia AtividadeRecorrencia { get; set; }
        public bool id_email_enviado { get; set; }
        public Nullable<int> nm_total_alunos { get; set; }
        public int cd_escola_parametro { get; set; }
        
        
        public ICollection<AtividadeAluno> atividadesAluno { get; set; }
        public ICollection<AtividadeProspect> atividadesProspect{ get; set; }
        public ICollection<AtividadeCurso> AtividadeCurso { get; set; }
        public ICollection<AtividadeEscolaAtividade> AtividadeEscolaAtividade { get; set; }
        public ICollection<TipoAtividadeExtra> tiposAtividadeExtras { get; set; }
        public ICollection<Curso> cursos { get; set; }
        public ICollection<ProfessorUI> professores { get; set; }
        public ICollection<Produto> produtos { get; set; }
        public ICollection<Sala> salas { get; set; }
        public ICollection<Sala> salasDisponiveis { get; set; }
        public ICollection<AlunoSearchUI> alunos { get; set; }
   
        public static AtividadeExtraUI fromAtividadeExtra(AtividadeExtra atividadeExtra, string no_tipo_atividade_extra, List<int> cd_cursos, string no_produto, string no_responsavel, string no_sala, string usuario, int? qtd_alunos)
        {
            AtividadeExtraUI atividadeExtraUI = new AtividadeExtraUI
            {
                cd_atividade_extra = atividadeExtra.cd_atividade_extra,
                cd_cursos = cd_cursos,
                //cd_curso = atividadeExtra.cd_curso,
                cd_funcionario = atividadeExtra.cd_funcionario,
                cd_produto = atividadeExtra.cd_produto,
                cd_tipo_atividade_extra = atividadeExtra.cd_tipo_atividade_extra,
                dt_atividade_extra = atividadeExtra.dt_atividade_extra,
                hh_final = atividadeExtra.hh_final,
                hh_inicial = atividadeExtra.hh_inicial,
                ind_carga_horaria = atividadeExtra.ind_carga_horaria,
                ind_pagar_professor = atividadeExtra.ind_pagar_professor,
                nm_vagas = atividadeExtra.nm_vagas,
                tx_obs_atividade = atividadeExtra.tx_obs_atividade,
                cd_sala = atividadeExtra.cd_sala,
                no_tipo_atividade_extra = no_tipo_atividade_extra,
                id_calendario_academico = atividadeExtra.id_calendario_academico,
                hr_limite_academico = atividadeExtra.hr_limite_academico,
                cd_pessoa_escola = atividadeExtra.cd_pessoa_escola,
                //no_curso = no_curso,
                no_produto = no_produto,
                no_responsavel = no_responsavel,
                no_sala = no_sala,
                no_usuario = usuario,
                nm_alunos = qtd_alunos
            };
            return atividadeExtraUI;
        }

        public string dta_atividade_extra
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_atividade_extra);
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_atividade_extra", "Código"));
                retorno.Add(new DefinicaoRelatorio("dt_atividade_extra", "Data", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("hh_inicial", "Hora Inicial", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("hh_final", "Hora Final", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("no_tipo_atividade_extra", "Tipo Atividade", AlinhamentoColuna.Left));
                retorno.Add(new DefinicaoRelatorio("no_sala", "Sala", AlinhamentoColuna.Left, "0.7000in"));
                retorno.Add(new DefinicaoRelatorio("no_curso", "Curso", AlinhamentoColuna.Left, "2.000in"));
                retorno.Add(new DefinicaoRelatorio("no_responsavel", "Responsável", AlinhamentoColuna.Left, "2.0000in"));
                return retorno;
            }
        }
    }

    public class AtividadeExtraPesquisa {
        //Prametros para a pesquisa
        public AtividadeExtraPesquisa()
        {
            this.cd_pessoa_escola = 0;
            this.cd_atividade_extra = 0;
            this.data = "";
            this.cd_tipo_ativiade_extra = 0;
            this.cd_sala = 0;
            this.cd_funcionario = 0;
            this.cd_produto = 0;
            this.cd_escola = 0;
            this.cd_curso = 0;
            this.cd_aluno = 0;
            this.hrFinal = "";
            this.hrInicial = "";
        }
        public int cd_pessoa_escola { get; set; }
        public int cd_atividade_extra { get; set; }
        public string data { get; set; }
        public string hrInicial { get; set; }
        public string hrFinal { get; set; }
        public int cd_escola { get; set; }
        public int cd_produto { get; set; }
        public int cd_curso { get; set; }
        public int cd_funcionario { get; set; }
        public int cd_aluno { get; set; }
        public int cd_tipo_ativiade_extra { get; set; }
        public int cd_sala { get; set; }        
    }
}
