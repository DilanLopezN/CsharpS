using System;

namespace BoletoNet
{
    #region Enumerado

    public enum EnumEspecieDocumento_Sicoob
    {
        Cheque = 1,
        DuplicataMercantil = 2,
        DuplicataMercantilIndicao = 3,
        DuplicataServico = 4,
        DuplicataServicoIndicacao = 5,
        DuplicataRural = 6,
        LetraCambio = 7,
        NotaCreditoComercial = 8,
        //Warrant = 9,
        NotaCreditoExportacao = 9,
        NotaCreditoIndustrial = 10,
        NotaCreditoRural = 11,
        NotaPromissoria = 12,
        NotaPromissoriaRural = 13,
        TriplicataMercantil = 14,
        TriplicataServico = 15,
        NotaSeguro = 16,
        Recibo = 17,
        Fatura = 18,
        NotaDebito = 19,
        ApoliceSeguro = 20,
        MensalidadeEscolar = 21,
        ParcelaConsorcio = 22,
        NotaFiscal = 23,
        DocumentoDivida = 24,
        CedulaProdutoRural = 25,
        CartaoCredito = 31,
        BoletoProposta = 32,
        Outros = 99,
    }

    #endregion

    public class EspecieDocumento_Sicoob : AbstractEspecieDocumento, IEspecieDocumento
    {
        #region Construtores

        public EspecieDocumento_Sicoob()
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar objeto", ex);
            }
        }

        public EspecieDocumento_Sicoob(string codigo)
        {
            try
            {
                this.carregar(codigo);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar objeto", ex);
            }
        }

        #endregion

        #region Metodos Privados

        public string getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob especie)
        {
            return Convert.ToInt32(especie).ToString("00");
        }

        public EnumEspecieDocumento_Sicoob getEnumEspecieByCodigo(string codigo)
        {
            return (EnumEspecieDocumento_Sicoob) Convert.ToInt32(codigo);
        }

        private void carregar(string idCodigo)
        {
            try
            {
                this.Banco = new Banco_Sicoob();

                switch (getEnumEspecieByCodigo(idCodigo))
                {
                    case EnumEspecieDocumento_Sicoob.Cheque:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.Cheque);
                        this.Sigla = "CH";
                        this.Especie = "Cheque";
                        break;
                    case EnumEspecieDocumento_Sicoob.DuplicataMercantil:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.DuplicataMercantil);
                        this.Sigla = "DM";
                        this.Especie = "Duplicata Mercantil";
                        break;
                    case EnumEspecieDocumento_Sicoob.DuplicataMercantilIndicao:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.DuplicataMercantilIndicao);
                        this.Sigla = "DMI";
                        this.Especie = "Duplicata Mercantil p/ Indicação";
                        break;
                    case EnumEspecieDocumento_Sicoob.DuplicataServico:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.DuplicataServico);
                        this.Sigla = "DS";
                        this.Especie = "Duplicata de Serviço";
                        break;
                    case EnumEspecieDocumento_Sicoob.DuplicataServicoIndicacao:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.DuplicataServicoIndicacao);
                        this.Sigla = "DSI";
                        this.Especie = "Duplicata de Serviço p/ Indicação";
                        break;
                    case EnumEspecieDocumento_Sicoob.DuplicataRural:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.DuplicataRural);
                        this.Sigla = "DR";
                        this.Especie = "Duplicata Rural";
                        break;
                    case EnumEspecieDocumento_Sicoob.LetraCambio:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.LetraCambio);
                        this.Sigla = "LC";
                        this.Especie = "Letra de Câmbio";
                        break;
                    case EnumEspecieDocumento_Sicoob.NotaCreditoComercial:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.NotaCreditoComercial);
                        this.Sigla = "NCC";
                        this.Especie = "Nota de Crédito Comercial";
                        break;
                    /*case EnumEspecieDocumento_Sicoob.Warrant:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.Warrant);
                        this.Sigla = "WR";
                        this.Especie = "Warrant";
                        break;*/
                    case EnumEspecieDocumento_Sicoob.NotaCreditoExportacao:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.NotaCreditoExportacao);
                        this.Sigla = "NCE";
                        this.Especie = "Nota de Crédito a Exportação";
                        break;
                    case EnumEspecieDocumento_Sicoob.NotaCreditoIndustrial:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.NotaCreditoIndustrial);
                        this.Sigla = "NCI";
                        this.Especie = "Nota de Crédito Industrial";
                        break;
                    case EnumEspecieDocumento_Sicoob.NotaCreditoRural:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.NotaCreditoRural);
                        this.Sigla = "NCR";
                        this.Especie = "Nota de Crédito Rural";
                        break;
                    case EnumEspecieDocumento_Sicoob.NotaPromissoria:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.NotaPromissoria);
                        this.Sigla = "NP";
                        this.Especie = "Nota Promissória";
                        break;
                    case EnumEspecieDocumento_Sicoob.NotaPromissoriaRural:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.NotaPromissoriaRural);
                        this.Sigla = "NPR";
                        this.Especie = "Nota Promissória Rural";
                        break;
                    case EnumEspecieDocumento_Sicoob.TriplicataMercantil:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.TriplicataMercantil);
                        this.Sigla = "TM";
                        this.Especie = "Triplicata Mercantil";
                        break;
                    case EnumEspecieDocumento_Sicoob.TriplicataServico:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.TriplicataServico);
                        this.Sigla = "TS";
                        this.Especie = "Triplicata de Serviço";
                        break;
                    case EnumEspecieDocumento_Sicoob.NotaSeguro:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.NotaSeguro);
                        this.Sigla = "NS";
                        this.Especie = "Nota de Seguro";
                        break;
                    case EnumEspecieDocumento_Sicoob.Recibo:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.Recibo);
                        this.Sigla = "RC";
                        this.Especie = "Recibo";
                        break;
                    case EnumEspecieDocumento_Sicoob.Fatura:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.Fatura);
                        this.Sigla = "FAT";
                        this.Especie = "Fatura";
                        break;
                    case EnumEspecieDocumento_Sicoob.NotaDebito:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.NotaDebito);
                        this.Sigla = "ND";
                        this.Especie = "Nota de Débito";
                        break;
                    case EnumEspecieDocumento_Sicoob.ApoliceSeguro:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.ApoliceSeguro);
                        this.Sigla = "AP";
                        this.Especie = "Apólice de Seguro";
                        break;
                    case EnumEspecieDocumento_Sicoob.MensalidadeEscolar:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.MensalidadeEscolar);
                        this.Sigla = "ME";
                        this.Especie = "Mensalidade Escolar";
                        break;
                    case EnumEspecieDocumento_Sicoob.ParcelaConsorcio:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.ParcelaConsorcio);
                        this.Sigla = "PC";
                        this.Especie = "Parcela de Consórcio";
                        break;
                    case EnumEspecieDocumento_Sicoob.NotaFiscal:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.NotaFiscal);
                        this.Sigla = "NF";
                        this.Especie = "Nota Fiscal";
                        break;
                    case EnumEspecieDocumento_Sicoob.DocumentoDivida:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.DocumentoDivida);
                        this.Sigla = "DD";
                        this.Especie = "Documento de Dívida";
                        break;
                    case EnumEspecieDocumento_Sicoob.CedulaProdutoRural:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.CedulaProdutoRural);
                        this.Sigla = "CPR"; // Este campo no Manual CNAB está Vazio ""
                        this.Especie = "Cédula de Produto Rural";
                        break;
                    case EnumEspecieDocumento_Sicoob.CartaoCredito:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.CartaoCredito);
                        this.Sigla = "CC"; // Este campo no Manual CNAB está Vazio ""
                        this.Especie = "Cartão de Crédito";
                        break;
                    case EnumEspecieDocumento_Sicoob.BoletoProposta:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.BoletoProposta);
                        this.Sigla = "BDP";
                        this.Especie = "Boleto de Proposta";
                        break;
                    case EnumEspecieDocumento_Sicoob.Outros:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.Outros);
                        this.Sigla = "OU"; // Este campo no Manual CNAB está Vazio ""
                        this.Especie = "Outros";
                        break;
                    default:
                        this.Codigo = "0";
                        this.Sigla = "";
                        this.Especie = "( Selecione )";
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar objeto", ex);
            }
        }

        public static EspeciesDocumento CarregaTodas()
        {
            try
            {
                var alEspeciesDocumento = new EspeciesDocumento();

                var obj = new EspecieDocumento_Sicoob();

                foreach (var item in Enum.GetValues(typeof (EnumEspecieDocumento_Sicoob)))
                {
                    obj = new EspecieDocumento_Sicoob(obj.getCodigoEspecieByEnum((EnumEspecieDocumento_Sicoob)item));
                    alEspeciesDocumento.Add(obj);
                }

                return alEspeciesDocumento;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao listar objetos", ex);
            }
        }

        #endregion
    }
}