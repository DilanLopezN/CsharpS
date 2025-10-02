using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class RptItemFechamento : TO
    {
        public int cd_item { get; set; }
        public int cd_grupo_estoque { get; set; }
        public int cd_fechamento { get; set; }
        public int cd_tipo_item { get; set; }
        public String dc_item { get; set; }
        public String dc_grupo { get; set; }
        public int? saldo_atual { get; set; }
        public int saldo { get; set; }
        public decimal? vl_item { get; set; }
        public decimal? vl_custo { get; set; }
        public short nm_ano_fechamento { get; set; }
        public byte nm_mes_fechamento { get; set; }
        public int contado { get; set; }
        public int qt_entrada { get; set; }
        public int? qt_saida { get; set; }
        public decimal? vl_custo_atual { get; set; }
        public decimal? vl_venda_atual { get; set; }

        
        public int total
        {
            get
            {
                if (qt_saida != null)
                    return (int) saldo_atual + (int) qt_saida;
                else
                {
                    int t = (int) saldo_atual;
                    return t;
                }
            }
            set { qt_saida = value; }
        }
        
        public string custo
        {
            get
            {
                if (this.vl_custo == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_custo);
            }
        }

        public string venda
        {
            get
            {
                if (this.vl_item == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_item);
            }
        }

        public string custo_atual
        {
            get
            {
                if (this.vl_custo_atual == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_custo_atual);
            }
        }

        public string venda_atual
        {
            get
            {
                if (this.vl_venda_atual == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_venda_atual);
            }
        }

    }
}
