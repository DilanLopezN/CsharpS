using System;
using System.Collections.Generic;
using System.Text;

namespace BoletoNet
{
    #region Enumerado

    public enum EnumEspecieDocumento_DinariPay
    {
        DuplicataMercantil,
        NotaPromissoria,
        NotaSeguro,
        CobrancaSeriada,
        Recibo,
        LetraCambio,
        NotaDebito,
        DuplicataServico,
        Outros,
    }

    #endregion

    public class EspecieDocumento_DinariPay : AbstractEspecieDocumento, IEspecieDocumento
    {
        #region Construtores

        public EspecieDocumento_DinariPay()
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar objeto", ex);
            }
        }

        public EspecieDocumento_DinariPay(string codigo)
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

        public string getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay especie)
        {
            switch (especie)
            {
                case EnumEspecieDocumento_DinariPay.DuplicataMercantil: return "1";
                case EnumEspecieDocumento_DinariPay.NotaPromissoria: return "2";
                case EnumEspecieDocumento_DinariPay.NotaSeguro: return "3";
                case EnumEspecieDocumento_DinariPay.CobrancaSeriada: return "4";
                case EnumEspecieDocumento_DinariPay.Recibo: return "5";
                case EnumEspecieDocumento_DinariPay.LetraCambio: return "10";
                case EnumEspecieDocumento_DinariPay.NotaDebito: return "11";
                case EnumEspecieDocumento_DinariPay.DuplicataServico: return "12";
                case EnumEspecieDocumento_DinariPay.Outros: return "99";
                default: return "99";

            }
        }

        public EnumEspecieDocumento_DinariPay getEnumEspecieByCodigo(string codigo)
        {
            switch (codigo)
            {
                case "1": return EnumEspecieDocumento_DinariPay.DuplicataMercantil;
                case "2": return EnumEspecieDocumento_DinariPay.NotaPromissoria;
                case "3": return EnumEspecieDocumento_DinariPay.NotaSeguro;
                case "4": return EnumEspecieDocumento_DinariPay.CobrancaSeriada;
                case "5": return EnumEspecieDocumento_DinariPay.Recibo;
                case "10": return EnumEspecieDocumento_DinariPay.LetraCambio;
                case "11": return EnumEspecieDocumento_DinariPay.NotaDebito;
                case "12": return EnumEspecieDocumento_DinariPay.DuplicataServico;
                case "99": return EnumEspecieDocumento_DinariPay.Outros;
                default: return EnumEspecieDocumento_DinariPay.Outros;
            }
        }

        private void carregar(string idCodigo)
        {
            try
            {
                this.Banco = new Banco_DinariPay();

                switch (getEnumEspecieByCodigo(idCodigo))
                {
                    case EnumEspecieDocumento_DinariPay.DuplicataMercantil:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.DuplicataMercantil);
                        this.Especie = "Duplicata mercantil";
                        this.Sigla = "DM";
                        break;
                    case EnumEspecieDocumento_DinariPay.NotaPromissoria:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.NotaPromissoria);
                        this.Especie = "Nota promissória";
                        this.Sigla = "NP";
                        break;
                    case EnumEspecieDocumento_DinariPay.NotaSeguro:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.NotaSeguro);
                        this.Especie = "Nota de seguro";
                        this.Sigla = "NS";
                        break;
                    case EnumEspecieDocumento_DinariPay.CobrancaSeriada:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.CobrancaSeriada);
                        this.Especie = "Cobrança seriada";
                        this.Sigla = "CS";
                        break;
                    case EnumEspecieDocumento_DinariPay.Recibo:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.Recibo);
                        this.Especie = "Recibo";
                        this.Sigla = "RC";
                        break;
                    case EnumEspecieDocumento_DinariPay.LetraCambio:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.LetraCambio);
                        this.Sigla = "LC";
                        this.Especie = "Letra de câmbio";
                        break;
                    case EnumEspecieDocumento_DinariPay.NotaDebito:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.NotaDebito);
                        this.Sigla = "ND";
                        this.Especie = "Nota de débito";
                        break;
                    case EnumEspecieDocumento_DinariPay.DuplicataServico:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.DuplicataServico);
                        this.Sigla = "DS";
                        this.Especie = "Duplicata de serviço";
                        break;
                    case EnumEspecieDocumento_DinariPay.Outros:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.Outros);
                        this.Especie = "Outros";
                        break;
                    default:
                        this.Codigo = "0";
                        this.Especie = "( Selecione )";
                        this.Sigla = "";
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
                EspeciesDocumento alEspeciesDocumento = new EspeciesDocumento();

                EspecieDocumento_DinariPay obj = new EspecieDocumento_DinariPay();

                obj = new EspecieDocumento_DinariPay(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.DuplicataMercantil));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_DinariPay(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.NotaPromissoria));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_DinariPay(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.NotaSeguro));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_DinariPay(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.CobrancaSeriada));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_DinariPay(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.Recibo));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_DinariPay(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.LetraCambio));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_DinariPay(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.NotaDebito));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_DinariPay(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.DuplicataServico));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_DinariPay(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.Outros));
                alEspeciesDocumento.Add(obj);

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