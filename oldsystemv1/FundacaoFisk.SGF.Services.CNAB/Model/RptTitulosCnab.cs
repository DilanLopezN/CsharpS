using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
namespace FundacaoFisk.SGF.Web.Services.CNAB.Model
{
    public class RptTitulosCnab: TO
    {
        public enum TipoRetornoCNAB
        {
            GERAR_BAIXA = 1,
            CONFIRMAR_TITULO = 2,
            CONFIRMAR_PEDIDO_BAIXA = 3,
            ERRO_TITULO = 4,
            RETORNO_PROTESTO = 5
        }

        public enum StatusTituloRetornoCNAB
        {
            INICIAL = 0,
            ENVIADO_GERADO = 1,
            BAIXA_MANUAL = 2,
            CONFIRMADO_ENVIO = 3,
            BAIXA_MANUAL_CONFIRMADO = 4,
            PEDIDO_BAIXA = 5,
            CONFIRMADO_PEDIDO_BAIXA = 6

        }

        public string nm_aluno { get; set; }
        public string no_responsavel { get; set; }
        public int nm_titulo { get; set; }
        public int nm_parcela { get; set; }
        public string dt_emissao { get; set; }
        public string dt_vencimento { get; set; }
        public decimal Valor { get; set; }
        public string dc_nosso_numero { get; set; }
        public string dt_liquidacao_titulo { get; set; }
        public byte id_tipo_retorno { get; set; }
        public byte id_status_titulo_cnab { get; set; }
        public decimal vl_desconto_1 { get; set; }
        public decimal vl_desconto_2 { get; set; }
        public string no_local_movto { get; set; }
        public string no_arquivo_remessa { get; set; }
        public string no_turma { get; set; }
        public decimal valorTotalDespesa { get; set; }
        public decimal vl_saldo_titulo { get; set; }
        public decimal vl_liquidacao_titulo { get; set; }
        public decimal vl_desconto_titulo { get; set; }

        public string tipoRetornoCNAB
        {
            get
            {
                string retorno = "";
                switch (id_tipo_retorno)
                {
                    case (int)TipoRetornoCNAB.GERAR_BAIXA: retorno = "Título(s) baixado(s)";
                        break;
                    case (int)TipoRetornoCNAB.CONFIRMAR_TITULO: retorno = "Título(s) confirmado(s)";
                        break;
                    case (int)TipoRetornoCNAB.CONFIRMAR_PEDIDO_BAIXA: retorno = "Pedido(s) de baixa(s) confirmado(s)";
                        break;
                    case (int)TipoRetornoCNAB.ERRO_TITULO: retorno = "Titulo(s) com erro(s)";
                        break;
                }
                return retorno;
            }
        }

        public string statusTituloRetornoCnab
        {
            get
            {
                string descStatusCnab = "";
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("pt-BR");
                switch (id_status_titulo_cnab)
                {
                    case (int)StatusTituloRetornoCNAB.INICIAL: descStatusCnab = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(StatusTituloRetornoCNAB.INICIAL.ToString()).ToLower());
                        break;
                    case (int)StatusTituloRetornoCNAB.ENVIADO_GERADO: descStatusCnab = "Envio/Gerado";
                        break;
                    case (int)StatusTituloRetornoCNAB.BAIXA_MANUAL: descStatusCnab = "Baixa Manual";
                        break;
                    case (int)StatusTituloRetornoCNAB.CONFIRMADO_ENVIO: descStatusCnab = "Confirmado Envio";
                        break;
                    case (int)StatusTituloRetornoCNAB.BAIXA_MANUAL_CONFIRMADO: descStatusCnab = "Baixa Manual Confirmado";
                        break;
                    case (int)StatusTituloRetornoCNAB.PEDIDO_BAIXA: descStatusCnab = "Pedido Baixa";
                        break;
                    case (int)StatusTituloRetornoCNAB.CONFIRMADO_PEDIDO_BAIXA: descStatusCnab = "Confirmado Pedido Baixa";
                        break;
                }
                return descStatusCnab;
            }
        }
    }
}
