﻿using System;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class FollowUpRptUI: TO
    {
        public int cd_follow_up { get; set; }
        public System.DateTime dt_follow_up { get; set; }
        public Nullable<System.DateTime> dt_proximo_contato { get; set; }
        public bool id_follow_resolvido { get; set; }
        public bool id_follow_lido { get; set; }
        public byte id_tipo_follow { get; set; }
        public byte id_tipo_follow_pesq { get; set; }
        public int cd_usuario_origem { get; set; }
        public string no_usuario_origem { get; set; }
        public string no_usuario_destino { get; set; }
        public string no_prospect_aluno { get; set; }
        public string email { get; set; }
        public string telefone { get; set; }
        public string celular { get; set; }
        public string dc_assunto { get; set; }
        public string dc_acao_follow_up { get; set; }


        public string desc_tipo
        {
            get
            {
                string retorno = "";
                switch (id_tipo_follow)
                {
                    case (int)FundacaoFisk.SGF.GenericModel.FollowUp.TipoFollowUp.INTERNO:
                        retorno = "Interno";
                        break;
                    case (int)FundacaoFisk.SGF.GenericModel.FollowUp.TipoFollowUp.PROSPECT_ALUNO:
                        retorno = "Prospect/Aluno";
                        break;
                    case (int)FundacaoFisk.SGF.GenericModel.FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL:
                        retorno = "Administração Geral";
                        break;
                }
                return retorno;
            }
        }
        public string dta_data
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy HH:mm:ss}", dt_follow_up.ToLocalTime());
            }
        }
        public string dta_proximo_contato
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_proximo_contato);
            }
        }
        public string lido
        {
            get
            {
                return id_follow_lido ? "Sim" : "Não";
            }
        }
        public string resolvido
        {
            get
            {
                return id_follow_resolvido ? "Sim" : "Não";
            }
        }
    }
}