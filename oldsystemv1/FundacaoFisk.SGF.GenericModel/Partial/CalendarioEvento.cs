using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class CalendarioEvento
    {
        public string dt_inicial_formatada
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_inicial_evento);
            }
        }

        public string hh_inicial_formatada
        {
            get
            {
                if (hh_inicial_evento != null)
                    return hh_inicial_evento.ToString(@"hh\:mm");
                return "";
            }
        }

        public string dt_final_formatada
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_final_evento);
            }
        }

        public string hh_final_formatada
        {
            get
            {
                if (hh_final_evento != null)
                    return hh_final_evento.ToString(@"hh\:mm");
                return "";
            }
        }

        public string id_ativo_formatado
        {
            get
            {
                return this.id_ativo ? "Sim" : "Não";
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_evento", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_titulo_evento", "Título", AlinhamentoColuna.Left, "4.5000in"));
                retorno.Add(new DefinicaoRelatorio("dt_inicial_formatada", "Data Inicial", AlinhamentoColuna.Center, "1.0100in"));
                retorno.Add(new DefinicaoRelatorio("hh_inicial_formatada", "Hora Inicial", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("dt_final_formatada", "Data Final", AlinhamentoColuna.Center, "1.0100in"));
                retorno.Add(new DefinicaoRelatorio("hh_final_formatada", "Hora Final", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("id_ativo_formatado", "Ativo", AlinhamentoColuna.Center, "0.9000in"));

                return retorno;
            }
        }
    }
}
