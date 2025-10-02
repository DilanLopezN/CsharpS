using System;
using System.Collections;
using System.Text;

namespace BoletoNet
{
    #region Enumerado

    public enum EnumInstrucoes_Uniprime
    {
        Protestar = 9,
        NaoProtestar = 10,
        ProtestoFinsFalimentares = 42,
        ProtestarAposNDiasCorridos = 81,
        ProtestarAposNDiasUteis = 82,
        NaoReceberAposNDias = 91,
        DevolverAposNDias = 92,

        OutrasInstrucoes_ExibeMensagem_MoraDiaria = 900,
        OutrasInstrucoes_ExibeMensagem_MultaVencimento = 901
    }

    #endregion 

    public class Instrucao_Uniprime : AbstractInstrucao, IInstrucao
    {

        #region Construtores 

		public Instrucao_Uniprime()
		{
			try
			{
                this.Banco = new Banco(237);
			}
			catch (Exception ex)
			{
                throw new Exception("Erro ao carregar objeto", ex);
			}
		}

        public Instrucao_Uniprime(int codigo)
        {
            this.carregar(codigo, 0);
        }

        public Instrucao_Uniprime(int codigo, int nrDias)
        {
            this.carregar(codigo, nrDias);
        }
        public Instrucao_Uniprime(int codigo, double valor)
        {
            this.carregar(codigo, valor);
        }
        #endregion Construtores

        #region Metodos Privados

        private void carregar(int idInstrucao, double valor)
        {
            try
            {
                this.Banco = new Banco_Uniprime();
                this.Valida();

                switch ((EnumInstrucoes_Uniprime)idInstrucao)
                {
                    case EnumInstrucoes_Uniprime.OutrasInstrucoes_ExibeMensagem_MoraDiaria:
                        this.Codigo = 0;
                        this.Descricao = "Após vencimento cobrar mora diária de R$ " + valor;
                        break;
                    case EnumInstrucoes_Uniprime.OutrasInstrucoes_ExibeMensagem_MultaVencimento:
                        this.Codigo = 0;
                        this.Descricao = "Após vencimento cobrar multa de " + valor + "%";
                        break;
                    default:
                        this.Codigo = 0;
                        this.Descricao = " (Selecione) ";
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar objeto", ex);
            }
        }

        private void carregar(int idInstrucao, int nrDias)
        {
            try
            {
                this.Banco = new Banco_Uniprime();
                this.Valida();

                switch ((EnumInstrucoes_Uniprime)idInstrucao)
                {
                    case EnumInstrucoes_Uniprime.Protestar:
                        this.Codigo = (int)EnumInstrucoes_Uniprime.Protestar;
                        this.Descricao = "Protestar";
                        break;
                    case EnumInstrucoes_Uniprime.NaoProtestar:
                        this.Codigo = (int)EnumInstrucoes_Uniprime.NaoProtestar;
                        this.Descricao = "Não protestar";
                        break;
                    case EnumInstrucoes_Uniprime.ProtestoFinsFalimentares:
                        this.Codigo = (int)EnumInstrucoes_Uniprime.ProtestoFinsFalimentares;
                        this.Descricao = "Protesto para fins falimentares";
                        break;
                    case EnumInstrucoes_Uniprime.ProtestarAposNDiasCorridos:
                        this.Codigo = (int)EnumInstrucoes_Uniprime.ProtestarAposNDiasCorridos;
                        this.Descricao = "Protestar após " + nrDias + " dias corridos do vencimento";
                        break;
                    case EnumInstrucoes_Uniprime.ProtestarAposNDiasUteis:
                        this.Codigo = (int)EnumInstrucoes_Uniprime.ProtestarAposNDiasUteis;
                        this.Descricao = "Protestar após " + nrDias + " dias úteis do vencimento";
                        break;
                    case EnumInstrucoes_Uniprime.NaoReceberAposNDias:
                        this.Codigo = (int)EnumInstrucoes_Uniprime.NaoReceberAposNDias;
                        this.Descricao = "Não receber após " + nrDias + " dias do vencimento";
                        break;
                    case EnumInstrucoes_Uniprime.DevolverAposNDias:
                        this.Codigo = (int)EnumInstrucoes_Uniprime.DevolverAposNDias;
                        this.Descricao = "Devolver após " + nrDias + " dias do vencimento";
                        break;
                    default:
                        this.Codigo = 0;
                        this.Descricao = " (Selecione) ";
                        break;
                }

                this.QuantidadeDias = nrDias;
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
