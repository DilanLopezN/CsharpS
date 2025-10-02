using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public partial class SimulacaoAditamentoUI : TO
    {
        public static SimulacaoAditamentoUI Simulador(Contrato contrato)
        {
            var simulacao = new SimulacaoAditamentoUI 
            {
                id_tipo_aditamento = contrato.aditamentoMaxData.id_tipo_aditamento,
                dt_inicio_aditamento = contrato.aditamentoMaxData.dt_inicio_aditamento,
                nm_titulos_aditamento = contrato.aditamentoMaxData.nm_titulos_aditamento,
                vl_aditivo = contrato.titulos.Where(t => t.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO).Sum(x => x.vl_saldo_titulo),
                vl_parcela_titulo_aditamento = contrato.aditamentoMaxData.vl_parcela_titulo_aditamento,
            };

            return simulacao;
        }
        
        public byte? id_tipo_aditamento { get; set; }
        public DateTime? dt_inicio_aditamento { get; set; }
        public byte nm_titulos_aditamento { get; set; }
        public decimal vl_aditivo { get; set; }
        public decimal vl_parcela_titulo_aditamento { get; set; }
    }
}
