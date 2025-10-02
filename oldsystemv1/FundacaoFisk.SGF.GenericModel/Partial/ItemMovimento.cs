using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class ItemMovimento
    {
        public string dc_plano_conta { get; set; }
        public int cd_pessoa_cliente { get; set; }
        public bool planoSugerido { get; set; }
        public string no_item { get; set; }
        public short nm_cfop { get; set; }
        public int cd_grupo_estoque {get;set; }
        public int qt_item_movimento_dev { get; set; }
        public int qtd_item_devolvido { get; set; }
        public IEnumerable<ItemMovimentoKit> itemsMovimentoKit { get; set; }
        public int id { get; set; }
        public int cd_item_kit { get; set; }

        public int cd_tipo_item { get; set; }
        public bool id_material_didatico { get; set; }
        public bool id_voucher_carga { get; set; }
        public string vlUnitarioItem
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_unitario_item);
            }
        }
        public string vlTotalItem
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_total_item);
            }
        }
        public string vlLiquidoItem
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_liquido_item);
            }
        }
        public string vlAcrescimoItem
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_acrescimo_item);
            }
        }
        public string vlDescontoItem
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_desconto_item);
            }
        }

        public string pcDescontoItem
        {
            get
            {
                return string.Format("{0:#,0.00}", this.pc_desconto_item);
            }
        }

        public bool id_nf_item { get; set; }

        //Para o histórico do aluno:
        public System.DateTime dt_emissao_movimento { get; set; }
        public Nullable<int> nm_movimento { get; set; }
        public string dc_nm_movimento { get; set; }
        public string dta_emissao_movimento {
            get {
                if(dt_emissao_movimento != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_emissao_movimento);
                else
                    return String.Empty;
            }
        }

        public string vlr_liquido_item {
            get {
                return string.Format("{0:#,0.00}", this.vl_liquido_item);
            }
        }

        public static ItemMovimento changeValueItemMovimento(ItemMovimento itemMovtoContext, ItemMovimento itemMovtoView , int id_tipo_movimento)
        {
            itemMovtoContext.cd_item = itemMovtoView.cd_item;
            itemMovtoContext.dc_item_movimento = itemMovtoView.dc_item_movimento;
            itemMovtoContext.qt_item_movimento = itemMovtoView.qt_item_movimento;
            itemMovtoContext.vl_unitario_item = itemMovtoView.vl_unitario_item;
            itemMovtoContext.vl_total_item = itemMovtoView.vl_total_item;
            itemMovtoContext.vl_liquido_item = itemMovtoView.vl_liquido_item;
            itemMovtoContext.vl_acrescimo_item = itemMovtoView.vl_acrescimo_item;
            itemMovtoContext.vl_desconto_item = itemMovtoView.vl_desconto_item;
            itemMovtoContext.pc_desconto_item = itemMovtoView.pc_desconto_item;
            if (itemMovtoView.cd_plano_conta == null || itemMovtoView.cd_plano_conta > 0)
                itemMovtoContext.cd_plano_conta = itemMovtoView.cd_plano_conta;
            switch (id_tipo_movimento)
            {
                //case (int)Movimento.TipoMovimentoEnum.COMPRA:
                //case (int)Movimento.TipoMovimentoEnum.VENDAPRODUTO:
                //    break;
                case (int)Movimento.TipoMovimentoEnum.SERVICO:
                    itemMovtoContext.vl_base_calculo_ISS_item = decimal.Round(itemMovtoView.vl_base_calculo_ISS_item, 2);
                    itemMovtoContext.pc_aliquota_ISS = itemMovtoView.pc_aliquota_ISS;
                    itemMovtoContext.vl_ISS_item = decimal.Round(itemMovtoView.vl_ISS_item, 2);
                    itemMovtoContext.vl_aproximado = decimal.Round(itemMovtoView.vl_aproximado, 2);
                    itemMovtoContext.pc_aliquota_aproximada = itemMovtoView.pc_aliquota_aproximada;
                    break;
                case (int)Movimento.TipoMovimentoEnum.ENTRADA:
                case (int)Movimento.TipoMovimentoEnum.SAIDA:
                    itemMovtoContext.dc_cfop = itemMovtoView.dc_cfop;
                    itemMovtoContext.cd_cfop = itemMovtoView.cd_cfop;
                    itemMovtoContext.cd_situacao_tributaria_ICMS = itemMovtoView.cd_situacao_tributaria_ICMS;
                    itemMovtoContext.cd_situacao_tributaria_PIS = itemMovtoView.cd_situacao_tributaria_PIS;
                    itemMovtoContext.cd_situacao_tributaria_COFINS = itemMovtoView.cd_situacao_tributaria_COFINS;
                    itemMovtoContext.vl_base_calculo_ICMS_item = decimal.Round(itemMovtoView.vl_base_calculo_ICMS_item, 2);
                    itemMovtoContext.vl_base_calculo_COFINS_item = decimal.Round(itemMovtoView.vl_base_calculo_COFINS_item, 2);
                    itemMovtoContext.vl_base_calculo_PIS_item = decimal.Round(itemMovtoView.vl_base_calculo_PIS_item, 2);
                    itemMovtoContext.vl_base_calculo_IPI_item = decimal.Round(itemMovtoView.vl_base_calculo_IPI_item, 2);
                    itemMovtoContext.vl_ICMS_item = decimal.Round(itemMovtoView.vl_ICMS_item, 2);
                    itemMovtoContext.vl_PIS_item = decimal.Round(itemMovtoView.vl_PIS_item, 2);
                    itemMovtoContext.vl_COFINS_item = decimal.Round(itemMovtoView.vl_COFINS_item, 2);
                    itemMovtoContext.vl_IPI_item = decimal.Round(itemMovtoView.vl_IPI_item, 2);
                    itemMovtoContext.pc_aliquota_ICMS = itemMovtoView.pc_aliquota_ICMS;
                    itemMovtoContext.pc_aliquota_PIS = itemMovtoView.pc_aliquota_PIS;
                    itemMovtoContext.pc_aliquota_COFINS = itemMovtoView.pc_aliquota_COFINS;
                    itemMovtoContext.pc_aliquota_IPI = itemMovtoView.pc_aliquota_IPI;
                    itemMovtoContext.vl_aproximado = decimal.Round(itemMovtoView.vl_aproximado, 2);
                    itemMovtoContext.pc_aliquota_aproximada = itemMovtoView.pc_aliquota_aproximada;
                    break;
            }
            return itemMovtoContext;
        }

        // Propriedades para a nota fiscal de produto:
        public string vlUnitarioItemInvariante {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.vl_unitario_item);
            }
        }
        public string qtItemMovimento {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.0000}", this.qt_item_movimento);
            }
        }
        public string vlBaseCalculoICMSItemInvariante {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.vl_base_calculo_ICMS_item);
            }
        }
        public string pcAliquotaICMSInvariante {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.0000}", this.pc_aliquota_ICMS);
            }
        }
        public string vlICMSItemInvariante {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.vl_ICMS_item);
            }
        }
        public string vlAproximado {
            get {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.vl_aproximado);
            }
        }
    }
}
