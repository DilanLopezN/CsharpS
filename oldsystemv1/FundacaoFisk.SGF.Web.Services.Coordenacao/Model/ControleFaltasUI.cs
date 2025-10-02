using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class ControleFaltasUI : TO
    {
        public int cd_controle_faltas { get; set; }
        public int cd_turma { get; set; }
        public DateTime dt_controle_faltas { get; set; }
        public int cd_usuario { get; set; }
        public DateTime dh_controle_faltas { get; set; }
        public string no_turma { get; set; }
        public string no_usuario { get; set; }


        public ICollection<ControleFaltasAluno> ControleFaltasAluno { get; set; }

        public string dta_controle_faltas
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_controle_faltas);
            }
        }

        public string dtah_controle_faltas
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dh_controle_faltas);
            }
        }

        public static ControleFaltasUI fromItem(ControleFaltas controleFaltas)
        {
            ControleFaltasUI controleFaltasUi = new ControleFaltasUI
            {
                cd_controle_faltas = controleFaltas.cd_controle_faltas,
                cd_turma = controleFaltas.cd_turma,
                dt_controle_faltas = controleFaltas.dt_controle_faltas,
                cd_usuario = controleFaltas.cd_usuario,
                dh_controle_faltas = controleFaltas.dh_controle_faltas,
                no_turma = controleFaltas.no_turma,
                no_usuario = controleFaltas.no_usuario,
                ControleFaltasAluno = controleFaltas.ControleFaltasAluno

            };
            return controleFaltasUi;
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_item", "Código"));
                retorno.Add(new DefinicaoRelatorio("dta_controle_faltas", "Data", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("no_turma", "Turma", AlinhamentoColuna.Left, "3.3000in"));
                retorno.Add(new DefinicaoRelatorio("no_usuario", "Usuário", AlinhamentoColuna.Left, "2.0000in"));
                retorno.Add(new DefinicaoRelatorio("dtah_controle_faltas", "Data Cadastro", AlinhamentoColuna.Left, "0.8500in"));


                return retorno;
            }
        }

        public class ComponentesTurmaControleFalta
        {
            public Turma turma { get; set; }
            public List<Produto> produtos { get; set; }
            public List<Nivel> niveis { get; set; }
            public List<Duracao> duracoes { get; set; }
            public List<Curso> cursos { get; set; }
            public List<ProfessorUI> professores { get; set; }
            public bool usuarioSisProf { get; set; }
        }
    }
}