using System;
using System.Collections.Generic;
using System.Text;

namespace BoletoNet
{
    public enum EnumEspecieDocumento_Inter
    {
        DuplicataMercantil = 1,
        NotaPromissoria = 2,
        NotaSeguro = 3,
        MensalidadeEscolar = 4,
        Recibo = 5,
        Contrato = 6,
        Cosseguros = 7,
        DuplicataServico = 8,
        LetraCambio = 9,
        NotaDebito = 13,
        DocumentoDivida = 15,
        EncargosCondominais = 16,
        Diversos = 99,
    }

    public class EspecieDocumento_Inter : AbstractEspecieDocumento, IEspecieDocumento
    {
        #region Construtores

        public EspecieDocumento_Inter()
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar objeto", ex);
            }
        }

        public EspecieDocumento_Inter(string codigo)
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

        public string getCodigoEspecieByEnum(EnumEspecieDocumento_Inter especie)
        {
            switch (especie)
            {
                case EnumEspecieDocumento_Inter.DuplicataMercantil: return "1";
                case EnumEspecieDocumento_Inter.NotaPromissoria: return "2";
                case EnumEspecieDocumento_Inter.NotaSeguro: return "3";
                case EnumEspecieDocumento_Inter.MensalidadeEscolar: return "4";
                case EnumEspecieDocumento_Inter.Recibo: return "5";
                case EnumEspecieDocumento_Inter.Contrato: return "6";
                case EnumEspecieDocumento_Inter.Cosseguros: return "7";
                case EnumEspecieDocumento_Inter.DuplicataServico: return "8";
                case EnumEspecieDocumento_Inter.LetraCambio: return "9";
                case EnumEspecieDocumento_Inter.NotaDebito: return "13";
                case EnumEspecieDocumento_Inter.DocumentoDivida: return "15";
                case EnumEspecieDocumento_Inter.EncargosCondominais: return "16";
                case EnumEspecieDocumento_Inter.Diversos: return "99";
                default: return "99";

            }
        }

        public EnumEspecieDocumento_Inter getEnumEspecieByCodigo(string codigo)
        {
            switch (codigo)
            {
                case "1": return EnumEspecieDocumento_Inter.DuplicataMercantil;
                case "2": return EnumEspecieDocumento_Inter.NotaPromissoria;
                case "3": return EnumEspecieDocumento_Inter.NotaSeguro;
                case "4": return EnumEspecieDocumento_Inter.MensalidadeEscolar;
                case "5": return EnumEspecieDocumento_Inter.Recibo;
                case "6": return EnumEspecieDocumento_Inter.Contrato;
                case "7": return EnumEspecieDocumento_Inter.Cosseguros;
                case "8": return EnumEspecieDocumento_Inter.DuplicataServico;
                case "9": return EnumEspecieDocumento_Inter.LetraCambio;
                case "13": return EnumEspecieDocumento_Inter.NotaDebito;
                case "15": return EnumEspecieDocumento_Inter.DocumentoDivida;
                case "16": return EnumEspecieDocumento_Inter.EncargosCondominais;
                case "99": return EnumEspecieDocumento_Inter.Diversos;
                default: return EnumEspecieDocumento_Inter.Diversos;
            }
        }

        private void carregar(string idCodigo)
        {
            try
            {
                this.Banco = new Banco_Inter();
                EspecieDocumento_Inter ed = new EspecieDocumento_Inter();
                switch (getEnumEspecieByCodigo(idCodigo))
                {
                    case EnumEspecieDocumento_Inter.DuplicataMercantil:
                        this.Codigo = ed.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.DuplicataMercantil);
                        this.Especie = "Duplicata mercantil";
                        this.Sigla = "DM";
                        break;
                    case EnumEspecieDocumento_Inter.NotaPromissoria:
                        this.Codigo = ed.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.NotaPromissoria);
                        this.Especie = "Nota promissória";
                        this.Sigla = "NP";
                        break;
                    case EnumEspecieDocumento_Inter.NotaSeguro:
                        this.Codigo = ed.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.NotaSeguro);
                        this.Especie = "Nota de seguro";
                        this.Sigla = "NS";
                        break;
                    case EnumEspecieDocumento_Inter.MensalidadeEscolar:
                        this.Codigo = ed.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.MensalidadeEscolar);
                        this.Especie = "Mensalidade escolar";
                        this.Sigla = "ME";
                        break;
                    case EnumEspecieDocumento_Inter.Recibo:
                        this.Codigo = ed.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.Recibo);
                        this.Especie = "Recibo";
                        this.Sigla = "NS";
                        break;
                    case EnumEspecieDocumento_Inter.Contrato:
                        this.Codigo = ed.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.Contrato);
                        this.Sigla = "C";
                        this.Especie = "Contrato";
                        break;
                    case EnumEspecieDocumento_Inter.Cosseguros:
                        this.Codigo = ed.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.Cosseguros);
                        this.Sigla = "CS";
                        this.Especie = "Cosseguros";
                        break;
                    case EnumEspecieDocumento_Inter.DuplicataServico:
                        this.Codigo = ed.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.DuplicataServico);
                        this.Sigla = "DS";
                        this.Especie = "Duplicata de serviço";
                        break;
                    case EnumEspecieDocumento_Inter.LetraCambio:
                        this.Codigo = ed.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.LetraCambio);
                        this.Sigla = "LC";
                        this.Especie = "Letra de câmbio";
                        break;
                    case EnumEspecieDocumento_Inter.NotaDebito:
                        this.Codigo = ed.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.NotaDebito);
                        this.Sigla = "ND";
                        this.Especie = "Nota de débito";
                        break;
                    case EnumEspecieDocumento_Inter.DocumentoDivida:
                        this.Codigo = ed.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.DocumentoDivida);
                        this.Sigla = "DD";
                        this.Especie = "Documento de dívida";
                        break;
                    case EnumEspecieDocumento_Inter.EncargosCondominais:
                        this.Codigo = ed.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.EncargosCondominais);
                        this.Sigla = "EC";
                        this.Especie = "Encargos condominais";
                        break;
                    case EnumEspecieDocumento_Inter.Diversos:
                        this.Codigo = ed.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.Diversos);
                        this.Especie = "Diversos";
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
            EspeciesDocumento especiesDocumento = new EspeciesDocumento();
            EspecieDocumento_Inter ed = new EspecieDocumento_Inter();

            foreach (EnumEspecieDocumento_Inter item in Enum.GetValues(typeof(EnumEspecieDocumento_Inter)))
                especiesDocumento.Add(new EspecieDocumento_Inter(ed.getCodigoEspecieByEnum(item)));

            return especiesDocumento;
        }

        #endregion
    }
}