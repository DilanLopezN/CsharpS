using System;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class ObservacaoBaixaUI : TO
    {
        public string tx_obs_baixa { get; set; }
        public string tx_no_pessoa_obs_baixa { get; set; }
        public DateTime dt_vencimento_obs_baixa { get; set; }
        public int? numero_titulo_obs_baixa { get; set; }
        public byte? numero_parcela_titulo_obs_baixa { get; set; }
    }
}