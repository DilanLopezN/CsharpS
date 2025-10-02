using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{

    public partial class TituloRetornoCNAB
    {
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
        public enum TipoRetornoCNAB {
            GERAR_BAIXA = 1,
            CONFIRMAR_TITULO = 2,
            CONFIRMAR_PEDIDO_BAIXA = 3,
            ERRO_TITULO = 4,
            RETORNO_PROTESTO = 5
        }
        public enum CodigoOcorrenciaCnab
        {
            ENTRADA_CONFIRMADA = 2,
            DEBITO_TARIFAS = 28
        }
        public enum MotivoDespesaCnab
        {
            TAXA_BOLETO = 1,
            CUSTOS_PROTESTO = 8,
        }
        //Campos TituloRetornoCNAB
        public string nomeResponsavel { get; set; }
        public string nomePessoaTitulo { get; set; }
        public string emailPessoaTitulo { get; set; }
        public string no_turma_titulo { get; set; }
        public int codigoOcorrencia { get; set; }
        public decimal valorDespesa { get; set; }

        public string tipoRetornoCNAB {
            get {
                string retorno = "";
                switch (id_tipo_retorno)
                {
                    case (int)TipoRetornoCNAB.GERAR_BAIXA: retorno = "Título Baixado";
                        break;
                    case (int)TipoRetornoCNAB.CONFIRMAR_TITULO: retorno = "Título Confirmado";
                        break;
                    case (int)TipoRetornoCNAB.CONFIRMAR_PEDIDO_BAIXA: retorno = "Pedido de Baixa Confirmado";
                        break;
                    case (int)TipoRetornoCNAB.ERRO_TITULO: retorno = "Título com Erro";
                        break;
                    case (int)TipoRetornoCNAB.RETORNO_PROTESTO: retorno = "Protesto";
                        break;
                }
                return retorno;
            }
        }
        public string vlTitulo
        {
            get
            {
                return this.Titulo != null ? this.Titulo.vlTitulo : "";
            }
        }
        public string vlJuros
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_juros_retorno);
            }
        }
        public string vlBaixa
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_baixa_retorno);
            }
        }
        public string vlMulta
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_multa_retorno);
            }
        }
        public string vlDesconto
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_desconto_titulo);
            }
        }
        private string _dt_baixa = "";
        public string dt_baixa
        {
            get
            {
                if (String.IsNullOrEmpty(_dt_baixa))
                    return String.Format("{0:dd/MM/yyyy}", this.dt_baixa_retorno);
                else
                    return _dt_baixa;
            }
            set
            {
                this._dt_baixa = value;
            }

        }
        private string _dt_banco = "";
        public string dt_banco
        {
            get
            {
                if (String.IsNullOrEmpty(_dt_banco))
                    return String.Format("{0:dd/MM/yyyy}", this.dt_banco_retorno);
                else
                    return _dt_banco;
            }
            set
            {
                this._dt_banco = value;
            }

        }

        private string _descLocalMovtoTituloCnab = "";
        public String descLocalMovtoTitulo
        {
            get
            {
                if (this.Titulo != null && this.Titulo.LocalMovto != null && this.Titulo.LocalMovto.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO)
                    return this.Titulo.LocalMovto.no_local_movto + " | ag.:" + this.Titulo.LocalMovto.nm_agencia + " | c/c:"
                        + this.Titulo.LocalMovto.nm_conta_corrente + "-" + this.Titulo.LocalMovto.nm_digito_conta_corrente;
                else if (this.Titulo != null && this.Titulo.LocalMovto != null)
                    return this.Titulo.LocalMovto.no_local_movto;
                else
                    return _descLocalMovtoTituloCnab;
            }
            set
            {
                this._descLocalMovtoTituloCnab = value;
            }
        }
        private string _dt_vcto = "";
        public string dt_vcto
        {
            get
            {
                if (String.IsNullOrEmpty(_dt_vcto) && this.Titulo != null)
                    return this.Titulo.dt_vcto;
                else
                    return _dt_vcto;
            }
            set
            {
                this._dt_vcto = value;
            }

        }
        private string _dt_emissao = "";
        public string dt_emissao
        {
            get
            {
                if (String.IsNullOrEmpty(_dt_emissao) && this.Titulo != null)
                    return this.Titulo.dt_emissao;
                else
                    return _dt_emissao;
            }
            set
            {
                this._dt_emissao = value;
            }
        }
        public byte id_status_titulo_cnab { get; set; }
        public bool id_alterou_txt_cnab { get; set; }
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
        //Campos grade view título Cnab
        public Nullable<int> nro_contrato { get; set; }
        public int cd_produto { get; set; }
        public int? cd_turma { get; set; }
        public int cd_aluno { get; set; }
        public int cd_pessoa_responsavel { get; set; }
    }
}

