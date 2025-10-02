using System;
using System.Collections.Generic;
using System.Text;

namespace BoletoNet
{
    #region Enumerado

    public enum EnumEspecieDocumento_Uniprime
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

    public class EspecieDocumento_Uniprime : AbstractEspecieDocumento, IEspecieDocumento
    {
        #region Construtores

        public EspecieDocumento_Uniprime()
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar objeto", ex);
            }
        }

        public EspecieDocumento_Uniprime(string codigo)
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

        public string getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime especie)
        {
            switch (especie)
            {
                case EnumEspecieDocumento_Uniprime.DuplicataMercantil: return "1";
                case EnumEspecieDocumento_Uniprime.NotaPromissoria: return "2";
                case EnumEspecieDocumento_Uniprime.NotaSeguro: return "3";
                case EnumEspecieDocumento_Uniprime.CobrancaSeriada: return "4";
                case EnumEspecieDocumento_Uniprime.Recibo: return "5";
                case EnumEspecieDocumento_Uniprime.LetraCambio: return "10";
                case EnumEspecieDocumento_Uniprime.NotaDebito: return "11";
                case EnumEspecieDocumento_Uniprime.DuplicataServico: return "12";
                case EnumEspecieDocumento_Uniprime.Outros: return "99";
                default: return "99";

            }
        }

        public EnumEspecieDocumento_Uniprime getEnumEspecieByCodigo(string codigo)
        {
            switch (codigo)
            {
                case "1": return EnumEspecieDocumento_Uniprime.DuplicataMercantil;
                case "2": return EnumEspecieDocumento_Uniprime.NotaPromissoria;
                case "3": return EnumEspecieDocumento_Uniprime.NotaSeguro;
                case "4": return EnumEspecieDocumento_Uniprime.CobrancaSeriada;
                case "5": return EnumEspecieDocumento_Uniprime.Recibo;
                case "10": return EnumEspecieDocumento_Uniprime.LetraCambio;
                case "11": return EnumEspecieDocumento_Uniprime.NotaDebito;
                case "12": return EnumEspecieDocumento_Uniprime.DuplicataServico;
                case "99": return EnumEspecieDocumento_Uniprime.Outros;
                default: return EnumEspecieDocumento_Uniprime.Outros;
            }
        }

        private void carregar(string idCodigo)
        {
            try
            {
                this.Banco = new Banco_Uniprime();

                switch (getEnumEspecieByCodigo(idCodigo))
                {
                    case EnumEspecieDocumento_Uniprime.DuplicataMercantil:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.DuplicataMercantil);
                        this.Especie = "Duplicata mercantil";
                        this.Sigla = "DM";
                        break;
                    case EnumEspecieDocumento_Uniprime.NotaPromissoria:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.NotaPromissoria);
                        this.Especie = "Nota promissória";
                        this.Sigla = "NP";
                        break;
                    case EnumEspecieDocumento_Uniprime.NotaSeguro:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.NotaSeguro);
                        this.Especie = "Nota de seguro";
                        this.Sigla = "NS";
                        break;
                    case EnumEspecieDocumento_Uniprime.CobrancaSeriada:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.CobrancaSeriada);
                        this.Especie = "Cobrança seriada";
                        this.Sigla = "CS";
                        break;
                    case EnumEspecieDocumento_Uniprime.Recibo:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.Recibo);
                        this.Especie = "Recibo";
                        this.Sigla = "RC";
                        break;
                    case EnumEspecieDocumento_Uniprime.LetraCambio:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.LetraCambio);
                        this.Sigla = "LC";
                        this.Especie = "Letra de câmbio";
                        break;
                    case EnumEspecieDocumento_Uniprime.NotaDebito:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.NotaDebito);
                        this.Sigla = "ND";
                        this.Especie = "Nota de débito";
                        break;
                    case EnumEspecieDocumento_Uniprime.DuplicataServico:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.DuplicataServico);
                        this.Sigla = "DS";
                        this.Especie = "Duplicata de serviço";
                        break;
                    case EnumEspecieDocumento_Uniprime.Outros:
                        this.Codigo = getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.Outros);
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

                EspecieDocumento_Uniprime obj = new EspecieDocumento_Uniprime();

                obj = new EspecieDocumento_Uniprime(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.DuplicataMercantil));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_Uniprime(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.NotaPromissoria));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_Uniprime(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.NotaSeguro));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_Uniprime(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.CobrancaSeriada));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_Uniprime(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.Recibo));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_Uniprime(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.LetraCambio));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_Uniprime(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.NotaDebito));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_Uniprime(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.DuplicataServico));
                alEspeciesDocumento.Add(obj);

                obj = new EspecieDocumento_Uniprime(obj.getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.Outros));
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