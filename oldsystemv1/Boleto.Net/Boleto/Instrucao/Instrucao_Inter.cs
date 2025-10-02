using System;
using System.Collections;
using System.Text;

namespace BoletoNet
{
    #region Enumerado

    public enum EnumInstrucoes_Inter
    {
        Protestar = 9,                      // Emite aviso ao sacado após N dias do vencto, e envia ao cartório após 5 dias úteis
        NaoProtestar = 10,                  // Inibe protesto, quando houver instrução permanente na conta corrente
        ImportanciaporDiaDesconto = 30,
        ProtestoFinsFalimentares = 42,
        ProtestarAposNDiasCorridos = 34, //Jéferson (jefhtavares) em 09/09/14 -- Segundo o manual que eu tenho (v. de maio de 2014) não é 81 este código
        ProtestarAposNDiasUteis = 35, //Jéferson (jefhtavares) em 09/09/14 -- Segundo o manual que eu tenho (v. de maio de 2014) não é 82 este código
        NaoReceberAposNDias = 91,
        DevolverAposNDias = 92,
        MsgBoleto40Posicoes = 94,
        MultaVencimento = 997,
        JurosdeMora = 998,
        DescontoporDia = 999,
    }

    #endregion 
    public class Instrucao_Inter : AbstractInstrucao, IInstrucao
    {

        #region Construtores 

        public Instrucao_Inter()
        {
            try
            {
                this.Banco = new Banco(077);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar objeto", ex);
            }
        }

        public Instrucao_Inter(int codigo, int nrDias)
        {
            this.carregar(codigo, nrDias);
        }

        public Instrucao_Inter(int codigo)
        {
            this.carregar(codigo, 0);
        }
        public Instrucao_Inter(int codigo, double valor)
        {
            this.carregar(codigo, valor);
        }
        #endregion

        #region Metodos Privados

        private void carregar(int idInstrucao, int nrDias)
        {
            try
            {
                this.Banco = new Banco_Inter();
                this.Valida();

                switch ((EnumInstrucoes_Inter)idInstrucao)
                {
                    case EnumInstrucoes_Inter.Protestar:
                        this.Codigo = (int)EnumInstrucoes_Inter.Protestar;
                        this.Descricao = "Protestar após 5 dias úteis.";
                        break;
                    case EnumInstrucoes_Inter.NaoProtestar:
                        this.Codigo = (int)EnumInstrucoes_Inter.NaoProtestar;
                        this.Descricao = "Não protestar";
                        break;
                    case EnumInstrucoes_Inter.ImportanciaporDiaDesconto:
                        this.Codigo = (int)EnumInstrucoes_Inter.ImportanciaporDiaDesconto;
                        this.Descricao = "Importância por dia de desconto.";
                        break;
                    case EnumInstrucoes_Inter.ProtestoFinsFalimentares:
                        this.Codigo = (int)EnumInstrucoes_Inter.ProtestoFinsFalimentares;
                        this.Descricao = "Protesto para fins falimentares";
                        break;
                    case EnumInstrucoes_Inter.ProtestarAposNDiasCorridos:
                        this.Codigo = (int)EnumInstrucoes_Inter.ProtestarAposNDiasCorridos;
                        this.Descricao = "Protestar após " + nrDias + " dias corridos do vencimento";
                        break;
                    case EnumInstrucoes_Inter.ProtestarAposNDiasUteis:
                        this.Codigo = (int)EnumInstrucoes_Inter.ProtestarAposNDiasUteis;
                        this.Descricao = "Protestar após " + nrDias + " dias úteis do vencimento";
                        break;
                    case EnumInstrucoes_Inter.NaoReceberAposNDias:
                        this.Codigo = (int)EnumInstrucoes_Inter.NaoReceberAposNDias;
                        this.Descricao = "Não receber após N dias do vencimento";
                        break;
                    case EnumInstrucoes_Inter.DevolverAposNDias:
                        this.Codigo = (int)EnumInstrucoes_Inter.DevolverAposNDias;
                        this.Descricao = "Devolver após N dias do vencimento";
                        break;
                    case EnumInstrucoes_Inter.DescontoporDia:
                        this.Codigo = (int)EnumInstrucoes_Inter.DescontoporDia;
                        this.Descricao = "Conceder desconto de R$ "; // por dia de antecipação
                        break;
                    default:
                        this.Codigo = 0;
                        this.Descricao = "( Selecione )";
                        break;
                }

                this.QuantidadeDias = nrDias;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar objeto", ex);
            }
        }

        private void carregar(int idInstrucao, double valor)
        {
            try
            {
                this.Banco = new Banco_Inter();
                this.Valida();

                switch ((EnumInstrucoes_Inter)idInstrucao)
                {
                    case EnumInstrucoes_Inter.JurosdeMora:
                        this.Codigo = (int)EnumInstrucoes_Inter.JurosdeMora;
                        this.Descricao = "Após vencimento cobrar mora diária de " + valor + " %";
                        break;
                    case EnumInstrucoes_Inter.MultaVencimento:
                        this.Codigo = (int)EnumInstrucoes_Inter.MultaVencimento;
                        this.Descricao = "Após vencimento cobrar multa de " + valor + " %";
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar objeto", ex);
            }
        }

        public override void Valida()
        {
            //base.Valida();
        }

        #endregion

    }
}