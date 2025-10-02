using System;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class BaixaEfetuadaChequeUI
    {
        public int cd_baixa_titulo { get; set; }
        public int cd_baixa_automatica { get; set; }
        public int cd_tran_finan { get; set; }
        public string no_local_movto { get; set; }
        public string no_emitente_cheque { get; set; }
        public int? cd_local_banco { get; set; }
        public int cd_local_mvto { get; set; }
        public string no_pessoa { get; set; }
        public int? nm_titulo { get; set; }
        public int cd_titulo  { get; set; }
        public byte? nm_parcela_titulo { get; set; }
        public DateTime dt_vcto_titulo { get; set; }
        public DateTime dt_baixa_titulo { get; set; }
        public decimal vl_liquidacao_baixa { get; set; }
        public decimal vl_taxa_cartao { get; set; }
        public string dc_cartao_movto { get; set; }
    }
}