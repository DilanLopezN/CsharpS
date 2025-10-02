﻿using System;
using System.Collections;
using System.Text;

namespace BoletoNet
{
    #region Enumerado

    public enum EnumInstrucoes_DinariPay
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
    public class Instrucao_DinariPay : AbstractInstrucao, IInstrucao
    {
        #region Construtores 

        public Instrucao_DinariPay()
        {
            try
            {
                this.Banco = new Banco(238);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar objeto", ex);
            }
        }

        public Instrucao_DinariPay(int codigo)
        {
            this.carregar(codigo, 0);
        }

        public Instrucao_DinariPay(int codigo, int nrDias)
        {
            this.carregar(codigo, nrDias);
        }
        public Instrucao_DinariPay(int codigo, double valor)
        {
            this.carregar(codigo, valor);
        }
        #endregion Construtores

        #region Metodos Privados

        private void carregar(int idInstrucao, double valor)
        {
            try
            {
                this.Banco = new Banco_Sicredi();
                this.Valida();

                switch ((EnumInstrucoes_DinariPay)idInstrucao)
                {
                    case EnumInstrucoes_DinariPay.OutrasInstrucoes_ExibeMensagem_MoraDiaria:
                        this.Codigo = 0;
                        this.Descricao = "Após vencimento cobrar mora diária de R$ " + valor;
                        break;
                    case EnumInstrucoes_DinariPay.OutrasInstrucoes_ExibeMensagem_MultaVencimento:
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
                this.Banco = new Banco_DinariPay();
                this.Valida();

                switch ((EnumInstrucoes_DinariPay)idInstrucao)
                {
                    case EnumInstrucoes_DinariPay.Protestar:
                        this.Codigo = (int)EnumInstrucoes_DinariPay.Protestar;
                        this.Descricao = "Protestar";
                        break;
                    case EnumInstrucoes_DinariPay.NaoProtestar:
                        this.Codigo = (int)EnumInstrucoes_DinariPay.NaoProtestar;
                        this.Descricao = "Não protestar";
                        break;
                    case EnumInstrucoes_DinariPay.ProtestoFinsFalimentares:
                        this.Codigo = (int)EnumInstrucoes_DinariPay.ProtestoFinsFalimentares;
                        this.Descricao = "Protesto para fins falimentares";
                        break;
                    case EnumInstrucoes_DinariPay.ProtestarAposNDiasCorridos:
                        this.Codigo = (int)EnumInstrucoes_DinariPay.ProtestarAposNDiasCorridos;
                        this.Descricao = "Protestar após " + nrDias + " dias corridos do vencimento";
                        break;
                    case EnumInstrucoes_DinariPay.ProtestarAposNDiasUteis:
                        this.Codigo = (int)EnumInstrucoes_DinariPay.ProtestarAposNDiasUteis;
                        this.Descricao = "Protestar após " + nrDias + " dias úteis do vencimento";
                        break;
                    case EnumInstrucoes_DinariPay.NaoReceberAposNDias:
                        this.Codigo = (int)EnumInstrucoes_DinariPay.NaoReceberAposNDias;
                        this.Descricao = "Não receber após " + nrDias + " dias do vencimento";
                        break;
                    case EnumInstrucoes_DinariPay.DevolverAposNDias:
                        this.Codigo = (int)EnumInstrucoes_DinariPay.DevolverAposNDias;
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