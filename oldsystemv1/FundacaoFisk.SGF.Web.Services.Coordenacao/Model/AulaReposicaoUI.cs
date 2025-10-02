using System;
using System.Collections.Generic;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class AulaReposicaoUI: TO
    {

        public int cd_aula_reposicao { get; set; }
        public int cd_pessoa_escola { get; set; }
        public int cd_atendente { get; set; }
        public int cd_professor { get; set; }
        public System.DateTime dt_aula_reposicao { get; set; }
        public System.TimeSpan dh_inicial_evento { get; set; }
        public System.TimeSpan dh_final_evento { get; set; }
        public bool id_carga_horaria { get; set; }
        public bool id_pagar_professor { get; set; }
        public string tx_observacao_aula { get; set; }
        public int cd_turma { get; set; }
        public int cd_sala { get; set; }
        public string no_usuario { get; set; }
        public string no_turma { get; set; }
        public string no_responsavel { get; set; }
        public int nm_alunos { get; set; }
        public string no_sala { get; set; }
        public Nullable<int> cd_turma_destino { get; set; }
        public string no_turma_destino { get; set; }
        public int? cd_produto { get; set; }
        public int? cd_curso { get; set; }
        public int? cd_estagio { get; set; }


        public ICollection<FuncionarioSearchUI> professores { get; set; }
        public ICollection<ProfessorUI> professoresEdit { get; set; }
        public ICollection<TurmaSearch> turmas { get; set; }
        public ICollection<Sala> salas { get; set; }
        public ICollection<Sala> salasDisponiveis { get; set; }

        public ICollection<AlunoAulaReposicao> AlunoAulaReposicao { get; set; }

        public static AulaReposicaoUI fromAulaReposicao(AulaReposicao aulaReposicao, string no_responsavel, string usuario, int? qtd_alunos, string no_turma, string no_sala, string no_turma_destino)
        {
            AulaReposicaoUI aulaReposicaoUI = new AulaReposicaoUI
            {
                cd_aula_reposicao = aulaReposicao.cd_aula_reposicao,
                cd_pessoa_escola = aulaReposicao.cd_pessoa_escola,
                cd_atendente = aulaReposicao.cd_atendente,
                cd_professor = aulaReposicao.cd_professor,
                dt_aula_reposicao = aulaReposicao.dt_aula_reposicao,
                dh_inicial_evento = aulaReposicao.dh_inicial_evento,
                dh_final_evento = aulaReposicao.dh_final_evento,
                id_carga_horaria = aulaReposicao.id_carga_horaria,
                id_pagar_professor = aulaReposicao.id_pagar_professor,
                tx_observacao_aula = aulaReposicao.tx_observacao_aula,
                cd_turma = aulaReposicao.cd_turma,
                cd_turma_destino = aulaReposicao.cd_turma_destino,
                cd_sala = aulaReposicao.cd_sala,
                no_usuario = usuario,
                nm_alunos = (int)qtd_alunos,
                no_responsavel = no_responsavel,
                no_turma = no_turma,
                no_sala = no_sala,
                no_turma_destino = no_turma_destino

            };
            return aulaReposicaoUI;
        }
        
        
        
        public string dta_aula_reposicao
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_aula_reposicao);
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_atividade_extra", "Código"));
                retorno.Add(new DefinicaoRelatorio("dta_aula_personalizada", "Data", AlinhamentoColuna.Left, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("dh_inicial_evento", "Hr. Inicial", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("dh_final_evento", "Hr. Final", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("no_turma", "Turma", AlinhamentoColuna.Left, "1.8000in"));
                retorno.Add(new DefinicaoRelatorio("no_sala", "Sala", AlinhamentoColuna.Left, "0.7000in"));
                retorno.Add(new DefinicaoRelatorio("no_responsavel", "Professor", AlinhamentoColuna.Left, "1.1000in"));
                retorno.Add(new DefinicaoRelatorio("nm_aluno", "Alunos", AlinhamentoColuna.Left, "1.1000in"));
                return retorno;
            }
        }

        public class AulaReposicaoPesquisa
        {
            //Prametros para a pesquisa
            public AulaReposicaoPesquisa()
            {
                cd_aula_reposicao = 0;
                cd_pessoa_escola = 0;
                cd_atendente = 0;
                cd_professor = 0;
                dt_aula_reposicao = "";
                dh_inicial_evento = "";
                dh_final_evento = "";
                id_carga_horária = false;
                id_pagar_professor = false;
                tx_observacao_aula = "";
                cd_turma = 0;
                no_turma = "";
                no_responsavel = "";
                cd_aluno = 0;
                nm_alunos = 0;
                cd_sala = 0;
                no_sala = "";
                cd_turma_destino = 0;
                no_turma_destino = "";
            }


            public int cd_aula_reposicao { get; set; }
            public int cd_pessoa_escola { get; set; }
            public int cd_atendente { get; set; }
            public int cd_professor { get; set; }
            public string dt_aula_reposicao { get; set; }
            public string dh_inicial_evento { get; set; }
            public string dh_final_evento { get; set; }
            public bool id_carga_horária { get; set; }
            public bool id_pagar_professor { get; set; }
            public string tx_observacao_aula { get; set; }
            public int cd_turma { get; set; }
            public int cd_aluno { get; set; }
            public int cd_sala { get; set; }

            public string no_turma { get; set; }
            public string no_sala { get; set; }
            public string no_responsavel { get; set; }
            public int nm_alunos { get; set; }
            public int cd_turma_destino { get; set; }
            public string no_turma_destino { get; set; }

        }
    }
}