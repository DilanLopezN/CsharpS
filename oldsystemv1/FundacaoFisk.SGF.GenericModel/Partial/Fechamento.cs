using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Fechamento
    {
        public virtual ICollection<Item> itensFechamento { get; set; } //Itens que são incluídos para geração de estoque, na tela de fechamento de estoque.
        public string desAnoMes {
            get
            {
                if (nm_ano_fechamento == 0 || nm_mes_fechamento == 0)
                    return "";
                else
                    return "Ano: " + nm_ano_fechamento + " " + " Mês: " + nm_mes_fechamento;
            }
        }

        public string balanco
        {
            get
            {
                return this.id_balanco ? "Sim" : "Não";
            }
        }

        public string no_usuario { get; set; }
        public int qtd_estoque { get; set; }
        public string dtaHr_fechamento
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy HH:mm:ss}", dh_fechamento.ToLocalTime());
            }
        }

        public string dta_fechamento
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dh_fechamento.ToLocalTime());
            }
        }

        public string dtf_fechamento
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_fechamento);
            }
        }

        private string _hr_fechamento;
        public string hr_fechamento
        {
            get
            {
                if (_hr_fechamento == null)
                    return String.Format("{0:HH:mm:ss}", dh_fechamento.ToLocalTime());
                else
                    return _hr_fechamento;
            }
            set
            {
                _hr_fechamento = value;
            }
        }

        public static Fechamento changeValuesFechamento(Fechamento fechamento, Fechamento fechamentoBase)
        {
            fechamentoBase.nm_ano_fechamento = fechamento.nm_ano_fechamento;
            fechamentoBase.nm_mes_fechamento = fechamento.nm_mes_fechamento;
            fechamentoBase.dt_fechamento = fechamento.dt_fechamento;
            fechamentoBase.id_balanco = fechamento.id_balanco;
            fechamentoBase.dh_fechamento = fechamento.dh_fechamento;
            fechamentoBase.tx_obs_fechamento = fechamento.tx_obs_fechamento;
            fechamentoBase.SaldosItens = null;
            return fechamentoBase;
        }

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_evento", "Código"));
                retorno.Add(new DefinicaoRelatorio("dt_fechamento", "Data", AlinhamentoColuna.Center));
//                retorno.Add(new DefinicaoRelatorio("nm_mes_fechamento", "Mês", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("balanco", "Balanço", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("dtaHr_fechamento", "Data Hora", AlinhamentoColuna.Left, "1.5000in"));
                retorno.Add(new DefinicaoRelatorio("no_usuario", "Usuário", AlinhamentoColuna.Left, "1.5000in"));

                return retorno;
            }
        }
    }
}
