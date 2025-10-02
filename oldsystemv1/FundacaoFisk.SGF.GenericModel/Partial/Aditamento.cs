using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Aditamento
    {
        //“0-Não Informada”, “1-Até 30 dias”, “2-Até 60 dias”, “3-Até 90 dias” e “4-Escolher Data”.
        public string no_contrato { get; set; }
        public string no_usuario { get; set; }
        public bool aditamentoEdit { get; set; }
        public bool possui_descontos { get; set; }
        public decimal vl_soma_ultimo_adt_adic { get; set; }
        public double pc_bolsa_anterior_aditamento { get; set; }
        public decimal vl_aditivo_anterior { get; set; }
        public decimal vl_saldo_aditivo_em_aberto { get; set; }
        public String tipoDoc { get; set; }

        public enum TipoDataInicioEnum
        {
            NAO_INFORMADA = 0,
            ATE_TRINTA_DIAS = 1,
            ATE_SESSENTA_DIAS = 2,
            ATE_NOVENTA_DIAS = 3,
            ESCOLHER_DATA = 4

        }

        //Tipos de Aditamento: “1-Transferência de Turma”, “2-Perda de Desconto”, “3-Concessão de Desconto”, “4-Maioridade” e “5-Renegociação”.
        public enum TipoAditamento
        {
            TRANSFERENCIA_TURMA = 1,
            PERDA_DESCONTO = 2,
            CONCESSAO_DESCONTO = 3,
            MAIORIDADE = 4,
            ADICIONAR_PARCELAS = 5,
            REAJUSTE_ANUAL = 6,
            ADITIVO_BOLSA = 7
        }

        //Tipo de Pagamento das Aulas: “1-Quinzenal” ou “2-Mensal”.
        public enum TipoPagamentoAditamento
        {
            QUINZENAL = 1,
            MENSAL = 2
        }

        public string dc_tipo_data
        {
            get
            {
                var tipo = "";
                switch (id_tipo_data_inicio)
                {
                    case (byte)TipoDataInicioEnum.NAO_INFORMADA: tipo = "Não Informada";
                        break;
                    case (byte)TipoDataInicioEnum.ATE_TRINTA_DIAS: tipo = "Até 30 dias";
                        break;
                    case (byte)TipoDataInicioEnum.ATE_NOVENTA_DIAS: tipo = "Até 90 dias";
                        break;
                    case (byte)TipoDataInicioEnum.ESCOLHER_DATA: tipo = "Escolher Data";
                        break;
                    default: tipo = "";
                        break;
                }
                return tipo;
            }
            set { }
        }

        public string dc_tipo_aditamento
        {
            get
            {
                string tipo = "";
                switch (id_tipo_aditamento)
                {
                    case (byte)TipoAditamento.CONCESSAO_DESCONTO: tipo =  "Concessão de Desconto";
                        break;
                    case (byte)TipoAditamento.MAIORIDADE: tipo = "Maioridade";
                        break;
                    case (byte)TipoAditamento.PERDA_DESCONTO: tipo =  "Perda de Desconto";
                        break;
                    case (byte)TipoAditamento.REAJUSTE_ANUAL: tipo = "Reajuste Anual";
                        break;
                    case (byte)TipoAditamento.ADICIONAR_PARCELAS: tipo = "Adicionar Parcelas";
                        break;
                    case (byte)TipoAditamento.TRANSFERENCIA_TURMA: tipo = "Transferência de Turma";
                        break;
                    case (byte)TipoAditamento.ADITIVO_BOLSA: tipo = "Bolsa";
                        break;
                    default: tipo =  "";
                        break;
                }
                return tipo;
            }
            set { }
        }

        public string dc_tipo_pagamento
        {
            get
            {
                string tipo = "";
                switch (id_tipo_pagamento)
                {
                    case (byte)TipoPagamentoAditamento.QUINZENAL: tipo = "Quinzenal";
                        break;
                    case (byte)TipoPagamentoAditamento.MENSAL: tipo = "Mensal";
                        break;
                    default: tipo = "";
                        break;
                }
                return tipo;
            }

            set { }
        }

        public string dtaVctoAditamento {
            get {
                if(dt_vcto_aditamento != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_vcto_aditamento.Value);
                else
                    return String.Empty;
            }
        }

        public string dtaAditamento
        {
            get
            {
                if (dt_aditamento != null)
                    // return String.Format("{0:dd/MM/yyyy}", dt_aditamento.Value.ToLocalTime());
                    return String.Format("{0:dd/MM/yyyy}", dt_aditamento.Value);
                else
                    return String.Empty;
            }
        }

        public string dtInicioAdtoBolsa
        {
            get
            {
                if (dt_vencto_inicial != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_vencto_inicial.Value);
                else
                    return String.Empty;
            }
        }

        public string dtaInicioAditamento
        {
            get
            {
                if (dt_inicio_aditamento != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_inicio_aditamento.Value);
                else
                    return String.Empty;
            }
        }

        public string val_aula_hora
        {
            get
            {
                if (this.vl_aula_hora == 0)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_aula_hora);
            }
        }

        public string valor_aditivo
        {
            get
            {
                if (this.vl_aditivo == 0)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_aditivo);
            }
        }

        public string vlParcelaTituloAditamento
        {
            get
            {
                if (this.vl_parcela_titulo_aditamento == 0)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_parcela_titulo_aditamento);
            }
        }

        public string dtaHoraAditamento
        {
            get
            {
                if (dt_aditamento != null)
                    return String.Format("{0:dd/MM/yyyy HH:mm:ss}", dt_aditamento.Value.ToLocalTime());
                else
                    return String.Empty;
            }
        }

        public static Aditamento changeValuesAditamento(Aditamento adtContext, Aditamento adtView)
        {
            adtContext.id_tipo_data_inicio = adtView.id_tipo_data_inicio;
            adtContext.dt_inicio_aditamento = adtView.dt_inicio_aditamento;
            adtContext.nm_dia_vcto_desconto = adtView.nm_dia_vcto_desconto;
            adtContext.cd_nome_contrato = adtView.cd_nome_contrato;
            adtContext.id_tipo_aditamento = adtView.id_tipo_aditamento;
            adtContext.nm_previsao_inicial = adtView.nm_previsao_inicial;
            adtContext.vl_aula_hora = adtView.vl_aula_hora;
            adtContext.id_tipo_pagamento = adtView.id_tipo_pagamento;
            adtContext.nm_titulos_aditamento = adtView.nm_titulos_aditamento;
            adtContext.dt_vcto_aditamento = adtView.dt_vcto_aditamento;
            adtContext.vl_aditivo = adtView.vl_aditivo;
            adtContext.tx_obs_aditamento = adtView.tx_obs_aditamento;
            adtContext.vl_parcela_titulo_aditamento = adtView.vl_parcela_titulo_aditamento;
            adtContext.dt_vencto_inicial = adtView.dt_vencto_inicial.HasValue ? adtView.dt_vencto_inicial.Value.Date : adtView.dt_vencto_inicial;

            if (adtView.id_tipo_aditamento != null && adtView.id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADITIVO_BOLSA)
                if ((adtView.AditamentoBolsa != null && adtView.AditamentoBolsa.Count() > 0) || (adtContext.AditamentoBolsa != null && adtContext.AditamentoBolsa.Count() > 0))
                {
                    if (adtContext.AditamentoBolsa != null && adtContext.AditamentoBolsa.Count() > 0)
                    {
                        if (adtView.AditamentoBolsa != null && adtView.AditamentoBolsa.Count() > 0)
                        {
                            var aditamentoBolsa = adtContext.AditamentoBolsa.FirstOrDefault();
                            aditamentoBolsa = FundacaoFisk.SGF.GenericModel.AditamentoBolsa.changeValuesAditamentoBolsa(aditamentoBolsa, adtView.AditamentoBolsa.FirstOrDefault());
                        }
                        else
                            adtContext.AditamentoBolsa = new List<AditamentoBolsa>();
                    }
                    else
                        if (adtView.AditamentoBolsa != null && adtView.AditamentoBolsa.Count() > 0)
                            adtContext.AditamentoBolsa = adtView.AditamentoBolsa;
                }


            return adtContext;
        }

        public static string getDescricaoDataInicioAdt(Aditamento aditamentoMaxData)
        {
            var retorno = "";
            if (aditamentoMaxData != null && aditamentoMaxData.id_tipo_data_inicio > 0)
            {
                switch (aditamentoMaxData.id_tipo_data_inicio)
                {
                    case (int)FundacaoFisk.SGF.GenericModel.Aditamento.TipoDataInicioEnum.ATE_TRINTA_DIAS:
                        retorno = "Até 30 dias";
                        break;
                    case (int)FundacaoFisk.SGF.GenericModel.Aditamento.TipoDataInicioEnum.ATE_SESSENTA_DIAS:
                        retorno = "Até 60 dias";
                        break;
                    case (int)FundacaoFisk.SGF.GenericModel.Aditamento.TipoDataInicioEnum.ATE_NOVENTA_DIAS:
                        retorno = "Até 90 dias";
                        break;
                    case (int)FundacaoFisk.SGF.GenericModel.Aditamento.TipoDataInicioEnum.ESCOLHER_DATA:
                        retorno = aditamentoMaxData.dt_inicio_aditamento != null ? String.Format("{0:dd/MM/yyyy}", (DateTime)aditamentoMaxData.dt_inicio_aditamento) : "";
                        break;
                }
            }
            return retorno;
        }

        public static bool comparaAlterouAditamento(Aditamento adit, Aditamento aditamentoContext)
        {
            bool alterarAdit = false;
            if (aditamentoContext == null || aditamentoContext.cd_aditamento <= 0)
                alterarAdit = true;
            else
            {
                if (adit.id_tipo_data_inicio != aditamentoContext.id_tipo_data_inicio ||
                    adit.nm_dia_vcto_desconto != aditamentoContext.nm_dia_vcto_desconto ||
                    adit.cd_nome_contrato != aditamentoContext.cd_nome_contrato ||
                    adit.id_tipo_aditamento != aditamentoContext.id_tipo_aditamento ||
                    adit.nm_previsao_inicial != aditamentoContext.nm_previsao_inicial ||
                    adit.vl_aula_hora != aditamentoContext.vl_aula_hora ||
                    adit.id_tipo_pagamento != aditamentoContext.id_tipo_pagamento ||
                    adit.nm_titulos_aditamento != aditamentoContext.nm_titulos_aditamento ||
                    adit.dt_vcto_aditamento != aditamentoContext.dt_vcto_aditamento ||
                    adit.vl_aditivo != aditamentoContext.vl_aditivo ||
                    adit.tx_obs_aditamento != aditamentoContext.tx_obs_aditamento ||
                    adit.vl_parcela_titulo_aditamento != aditamentoContext.vl_parcela_titulo_aditamento)
                    alterarAdit = true;
                if (adit.dt_inicio_aditamento != null)
                    adit.dt_inicio_aditamento = adit.dt_inicio_aditamento.Value.Date;
                if (aditamentoContext.dt_inicio_aditamento != null)
                    aditamentoContext.dt_inicio_aditamento = aditamentoContext.dt_inicio_aditamento.Value.Date;
                if (adit.dt_inicio_aditamento != aditamentoContext.dt_inicio_aditamento)
                    alterarAdit = true;
                if (comparaAlterouBolsaAditamento(adit, aditamentoContext))
                    alterarAdit = true;
            }
            return alterarAdit;
        }

        private static bool comparaAlterouBolsaAditamento(Aditamento adit, Aditamento aditamentoContext)
        {
            bool alterarAdit = false;
            if ((adit.AditamentoBolsa != null && adit.AditamentoBolsa.Count() > 0) || (aditamentoContext.AditamentoBolsa != null && aditamentoContext.AditamentoBolsa.Count() > 0))
            {
                if (adit.AditamentoBolsa != null && adit.AditamentoBolsa.Count() > 0 && aditamentoContext.AditamentoBolsa != null && aditamentoContext.AditamentoBolsa.Count() > 0)
                {
                    AditamentoBolsa adtBolsaEdit = adit.AditamentoBolsa.FirstOrDefault();
                    AditamentoBolsa adtBolsaContext = aditamentoContext.AditamentoBolsa.FirstOrDefault();
                    if (adtBolsaContext.cd_motivo_bolsa != adtBolsaEdit.cd_motivo_bolsa ||
                       adtBolsaContext.dc_validade_bolsa != adtBolsaEdit.dc_validade_bolsa ||
                       adtBolsaContext.dt_comunicado_bolsa != adtBolsaEdit.dt_comunicado_bolsa ||
                       adtBolsaContext.pc_bolsa != adtBolsaEdit.pc_bolsa)
                        alterarAdit = true;
                }
                else
                    alterarAdit = true;
            }
            return alterarAdit;
        }
    }
}