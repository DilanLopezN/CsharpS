using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class EstoqueUI
    {
        public enum relatorioEstoque
        {
            MOVIMENTOS = 0,
            ITENS = 1,
            SALDO_REAL_BIBLIOTECA = 2,
            CONTABIL = 3
        }

        public int relatorio{ get; set; }
        public int tipoItem { get; set; }
        public int grupoItem { get; set; }
        public byte tipoRpt { get; set; } // analítico // sintético
        public int item { get; set; }
        public String dtaInicio { get; set; }
        public String dtaFim { get; set; }
        public String no_pessoa { get; set; }
        public String dc_grupo { get; set; }
        public String dc_item { get; set; }

        public bool isColunaContagem { get; set; }
        public bool isColunaContatos { get; set; }
        public int anoMesContagem { get; set; }
        public int ano { get; set; }
        public int mes { get; set; }       

        public Decimal valor_saldo { get; set; }
        public float quatidade_saldo { get; set; }
        public bool isSemColunaC { get; set; }
        public bool isApenasItensMovimento { get; set; }        
    }
}
