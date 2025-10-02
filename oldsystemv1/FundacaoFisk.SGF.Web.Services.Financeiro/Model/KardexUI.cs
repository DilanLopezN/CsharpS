using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class KardexUI : TO
    {
        public int cd_pessoa_escola { get; set; }
        public string no_pessoa { get; set; }
        public string no_item { get; set; }
        public string no_grupo_estoque { get; set; }
        public DateTime dt_kardex { get; set; }
        public byte? id_tipo_movimento { get; set; }
        public string tx_obs_kardex { get; set; }
        public int qt_entrada { get; set; }
        public int qt_saida { get; set; }
        public double? qt_saldo { get; set; } //era int
        public decimal vl_entrada { get; set; }
        public decimal vl_saida { get; set; }
        public decimal? vl_saldo { get; set; }  //era decimal
        public int qt_inicial { get; set; }
        public decimal? vl_inicial { get; set; }
        public byte id_saldo { get; set; }
        public int cd_kardex { get; set; }
        public int cd_item { get; set; }

        public double? qt_saldo_item { get; set; }
        public decimal? vl_saldo_item { get; set; }
    }
}
