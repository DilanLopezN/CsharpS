using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public partial class PoliticaDescontoUI : TO
    {
        public int cd_politica_desconto { get; set; }
        public int cd_pessoa_escola { get; set; }
        public bool id_ativo { get; set; }
        public DateTime? dt_inicial_politica { get; set; }
        public int cd_turma { get; set; }
        public int cd_aluno { get; set; }

        public List<DiasPolitica> diasPolitica { get; set; }
        public IEnumerable<PoliticaAluno> alunosPol { get; set; }
        public IEnumerable<PessoaSGF> pessoasAlunoPol { get; set; }
        public IEnumerable<Turma> turmas { get; set; }
        public IEnumerable<PoliticaTurma> turmasPol { get; set; }

        //Propriedades para a search:
        public string no_turma
        {
            get
            {
                String turmas = "";
                if(this.turmasPol != null && this.turmasPol.Count() > 0)
                {
                    foreach (PoliticaTurma pol in turmasPol)
                        turmas += pol.Turma.no_turma + " | ";
                    return turmas.Substring(0, turmas.Length -3);
                }
                return "";
            }
        }

        public string no_aluno
        {
            get
            {
                String alunos = "";
                if (this.pessoasAlunoPol != null && this.pessoasAlunoPol.Count() > 0)
                {
                    foreach(PessoaSGF aluno in pessoasAlunoPol)
                        alunos += aluno.no_pessoa + " | ";
                    return alunos.Substring(0, alunos.Length - 3);
                }
                return "";
            }
        }

        public string politica_desconto_ativo
        {
            get
            {
                return this.id_ativo ? "Sim" : "Não";
            }
        }
        public string dt_inicial
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_inicial_politica);
            }
        }

        public string dias_percential 
        {
            get
            {
                var i = 0; 
                var perc_aux = string.Empty;
                foreach (var politica in diasPolitica)
                {
                    perc_aux += string.Format("Dia {0}: {1}%{2}", politica.nm_dia_limite_politica, politica.desconto, ++i < diasPolitica.Count ? ", " : "");
                }
                return perc_aux;
            }
            set { }
        }


        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_politica_desconto", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_turma", "Turma(s)", AlinhamentoColuna.Left, "2.0000in"));
                retorno.Add(new DefinicaoRelatorio("no_aluno", "Aluno(s)", AlinhamentoColuna.Left, "2.0000in"));
                retorno.Add(new DefinicaoRelatorio("dt_inicial", "Data Inicial", AlinhamentoColuna.Right, "1.000in"));
                retorno.Add(new DefinicaoRelatorio("politica_desconto_ativo", "Ativo", AlinhamentoColuna.Center));

                return retorno;
            }
        }
        


        public static PoliticaDescontoUI fromPoliticaDesconto(PoliticaDesconto politica)
        {
            PoliticaDescontoUI politicaUI = new PoliticaDescontoUI();
            politicaUI.cd_politica_desconto = politica.cd_politica_desconto;
            politicaUI.turmasPol = politica.PoliticasTurmas;
            politicaUI.alunosPol = politica.PoliticasAlunos;
            politicaUI.dt_inicial_politica = politica.dt_inicial_politica;
            politicaUI.id_ativo = politica.id_ativo;
            return politicaUI;
        }
    }
}