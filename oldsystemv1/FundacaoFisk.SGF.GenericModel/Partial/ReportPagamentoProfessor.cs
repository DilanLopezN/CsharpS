using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class ReportPagamentoProfessor : TO
    {
        public enum TipoRelatorio
        {
            Todos = 0,
            Aulas_Normais = 1,
            Aulas_Substituicao = 2,
            Atividade_Extra = 3
        }

        public string dta_inicio_aula
        {
            get
            {
                return this.dt_inicio_aula.CompareTo(DateTime.MinValue) == 0 ? "" : String.Format("{0:dd/MM/yyyy}", this.dt_inicio_aula);
            }
        }
        public string dta_dia_falta
        {
            get
            {
                return this.dt_dia_falta.HasValue ? this.dt_dia_falta.Value.CompareTo(DateTime.MinValue) == 0 ? "" : String.Format("{0:dd/MM/yyyy}", this.dt_dia_falta) : "";
            }
        }

        public int cd_tipo_relatorio { get; set; }
        public string tipo_relatorio
        {
            get
            {
                var tipo = "";
                switch (cd_tipo_relatorio)
                {
                    case (byte)TipoRelatorio.Todos: tipo = "Todos";
                        break;
                    case (byte)TipoRelatorio.Aulas_Normais: tipo = "Aulas Normais";
                        break;
                    case (byte)TipoRelatorio.Aulas_Substituicao: tipo = "Aulas Substituição";
                        break;
                    case (byte)TipoRelatorio.Atividade_Extra: tipo = "Atividade Extra";
                        break;
                    default: tipo = "";
                        break;
                }
                return tipo;
            }
            set { }
        }
    }
}
