using System.Collections.Generic;
using System.Linq;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class DescontoContrato {
        public string valor_desconto {
            get {
                string retorno = "";

                if(this.cd_desconto != null) {
                    if(this.vl_desconto_contrato == 0) 
                        retorno = "\n" + this.dc_desconto_contrato + " - " + this.pc_desconto_contrato + "%";
                    else
                        retorno = "\n" + this.dc_desconto_contrato + " - " + this.vl_desconto_contrato;
                }
                return retorno;
            }
        }


        public string dc_tipo_desconto { get; set; }
        public bool desc_incluso_valor { get; set; }
        public decimal? pc_desconto { get; set; }
        public string pc_desc
        {
            get
            {
                if (this.pc_desconto == null)
                    return "";
                return string.Format("{0,00}", this.pc_desconto);
            }
        }
        public string vl_desconto
        {
            get
            {
                //if (this.vl_desconto_contrato == null)
                //    return "";
                return string.Format("{0:#,0.00}", this.vl_desconto_contrato);
            }
        }
        public bool aplicar_percentual_sem_desconto { get; set; }
        public static void changeValuesDescontoContratoAditamento(DescontoContrato descView, DescontoContrato descContext){
            descContext.pc_desconto_contrato = descView.pc_desconto_contrato;
            descContext.vl_desconto_contrato = descView.vl_desconto_contrato;
            descContext.id_desconto_ativo = descView.id_desconto_ativo;
            descContext.nm_parcela_ini = descView.nm_parcela_ini;
            descContext.nm_parcela_fim = descView.nm_parcela_fim;
        }

        public static decimal calcularValoresDescontoPorPecentualOrValor(decimal pc_desconto, decimal vl_desconto_contrato,
            decimal pc_total_desconto_aplicado, decimal valorBaseCalc, bool somar_desconto)
        {
            if (pc_desconto > 0)
            {
                if (somar_desconto)
                    pc_total_desconto_aplicado = pc_total_desconto_aplicado + pc_desconto;
                else
                {
                    if (pc_total_desconto_aplicado > 0)
                        pc_total_desconto_aplicado = 100 - ((1 - pc_total_desconto_aplicado / 100) * (1 - pc_desconto / 100)) * 100;
                    else
                        pc_total_desconto_aplicado = 100 - (1 - pc_desconto / 100) * 100;
                }
            }

            if (vl_desconto_contrato > 0)
            {
                var pc_desc = ((vl_desconto_contrato) / valorBaseCalc) * 100;
                if (somar_desconto)
                {
                    pc_total_desconto_aplicado =  pc_total_desconto_aplicado + pc_desc ;
                }
                else
                {
                    if (pc_total_desconto_aplicado > 0)
                        pc_total_desconto_aplicado = 100 - ((1 - pc_total_desconto_aplicado / 100) * (1 - pc_desc / 100)) * 100;
                    else
                        pc_total_desconto_aplicado = 100 - (1 - pc_desc / 100) * 100;
                }
            }
            return pc_total_desconto_aplicado;
        }

        public static string gerarDescDescontosContrato(List<Titulo> titulosAbertos, byte nm_parcela_ini, byte nm_parcela_fim)
        {
            string retorno = "";
            if (titulosAbertos != null && titulosAbertos.Count() > 0)
            {
                List<Titulo> titulosSelecionados = new List<Titulo>();
                if (nm_parcela_ini > 0 && nm_parcela_fim > 0)
                    titulosSelecionados = titulosAbertos.Where(x => x.nm_parcela_titulo >= nm_parcela_ini && x.nm_parcela_titulo <= nm_parcela_fim).ToList();
                else
                    if (nm_parcela_ini > 0 && nm_parcela_fim <= 0)
                        titulosSelecionados = titulosAbertos.Where(x => x.nm_parcela_titulo >= nm_parcela_ini).ToList();
                    else
                        if (nm_parcela_fim > 0)
                            titulosSelecionados = titulosAbertos.Where(x => x.nm_parcela_titulo <= nm_parcela_fim).ToList();
                        else
                            titulosSelecionados = titulosAbertos;
                var titulosOrdenados = titulosSelecionados.OrderBy(x => x.nm_parcela_titulo);
                foreach (Titulo t in titulosOrdenados)
                    if (!string.IsNullOrEmpty(retorno))
                        retorno += "- " + t.nm_parcela_titulo;
                    else
                        retorno += t.nm_parcela_titulo;
            }
            return retorno;
        }
    }
}
