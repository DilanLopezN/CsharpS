using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using System.Collections.Generic;
using System;
using System.Linq;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class MovimentoPerdaMaterialUI : TO
    {
        public int cd_movimento { get; set; }
        public string no_pessoa { get; set; }
        public string dc_politica_comercial { get; set; }
        public string dc_tipo_financeiro { get; set; }
        public Nullable<int> nm_movimento { get; set; }
        public Nullable<int> nm_movimento_dev { get; set; }
        public string dc_serie_movimento { get; set; }
        public System.DateTime dt_emissao_movimento { get; set; }
        public System.DateTime dt_vcto_movimento { get; set; }
        public System.DateTime dt_mov_movimento { get; set; }
        public byte id_tipo_movimento { get; set; }
        public byte? id_tipo_mvto_nf_dev { get; set; }
        public byte? id_natureza_mvto_tp_nf { get; set; }
        public byte? id_status_nf { get; set; }
        public string dc_meio_pagamento { get; set; }
        public bool id_nf { get; set; }
        public string url_relatorio { get; set; }
        public decimal pc_acrescimo { get; set; }
        public decimal vl_acrescimo { get; set; }
        public decimal pc_desconto { get; set; }
        public decimal vl_desconto { get; set; }
        public bool envio_masterSaf_empresa_propira { get; set; }
        public decimal? qtd_total_geral { get; set; }
        public IEnumerable<DateTime> datas_vencimento_titulo { get; set; }
        public int? cd_contrato { get; set; }
        public int? nm_contrato { get; set; }
        public int? cd_aluno { get; set; }
        public string no_aluno { get; set; }
        public string dc_datas_titulos
        {
            get
            {
                string retorno = "";
                if (datas_vencimento_titulo != null && datas_vencimento_titulo.Count() > 0)
                {
                    foreach (DateTime d in datas_vencimento_titulo)
                        retorno += String.Format("{0:dd/MM/yyyy}", d) + ", ";
                    retorno = retorno.Substring(0, retorno.Length - 2);
                }
                return retorno;
            }
        }
        public string vl_qtd_total_geral
        {
            get
            {
                string retorno = string.Format("{0:#,0.00}", 0);
                if (this.qtd_total_geral != null)
                    retorno = string.Format("{0:#,0.00}", this.qtd_total_geral);
                return retorno;
            }
        }
        public string dc_numero_serie
        {
            get
            {
                var retorno = "";
                if (nm_movimento != null)
                    retorno += nm_movimento;
                if (!string.IsNullOrEmpty(dc_serie_movimento))
                    if (!string.IsNullOrEmpty(retorno))
                        retorno += "-" + dc_serie_movimento;
                    else
                        retorno += dc_serie_movimento;
                if (nm_movimento_dev > 0)
                    retorno += "     (" + nm_movimento_dev.ToString() + ")";
                return retorno;
            }
        }
        public string dta_emissao_movimento
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_emissao_movimento);
            }
        }
        public string dta_vcto_movimento
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_vcto_movimento);
            }
        }
        public string dta_mov_movimento
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_mov_movimento);
            }
        }

        public string status_nf
        {
            get
            {
                var retorno = "";
                if (id_status_nf != null)
                    switch ((byte)id_status_nf)
                    {
                        case (byte)Movimento.StatusNFEnum.ABERTO:
                            retorno = "aberta";
                            break;
                        case (byte)Movimento.StatusNFEnum.FECHADO:
                            retorno = "fechada";
                            break;
                        case (byte)Movimento.StatusNFEnum.CANCELADO:
                            retorno = "cancelada";
                            break;
                    }

                return retorno;
            }
        }
        public string status_nf_pesq
        {
            get
            {
                var retorno = "";
                if (id_status_nf != null)
                    switch ((byte)id_status_nf)
                    {
                        case (byte)Movimento.StatusNFEnum.ABERTO:
                            retorno = "Aberta";
                            break;
                        case (byte)Movimento.StatusNFEnum.FECHADO:
                            retorno = "Fechada";
                            break;
                        case (byte)Movimento.StatusNFEnum.CANCELADO:
                            retorno = "Cancelada";
                            break;
                    }

                return retorno;
            }
        }
        public string dc_natureza_tipo_nf
        {
            get
            {
                var retorno = "";
                if (id_natureza_mvto_tp_nf != null)
                    switch ((byte)id_natureza_mvto_tp_nf)
                    {
                        case (byte)Movimento.TipoMovimentoEnum.ENTRADA:
                            retorno = "Entrada";
                            break;
                        case (byte)Movimento.TipoMovimentoEnum.SAIDA:
                            retorno = "Saída";
                            break;
                    }

                return retorno;
            }
        }
        public string dc_tipo_movto
        {
            get
            {
                var retorno = "";
                switch ((byte)id_tipo_movimento)
                {
                    case (byte)Movimento.TipoMovimentoEnum.ENTRADA:
                        retorno = "Entrada";
                        break;
                    case (byte)Movimento.TipoMovimentoEnum.SAIDA:
                        retorno = "Saída";
                        break;
                    case (byte)Movimento.TipoMovimentoEnum.DESPESA:
                        retorno = "Despesa";
                        break;
                    case (byte)Movimento.TipoMovimentoEnum.SERVICO:
                        retorno = "Serviço";
                        break;
                    case (byte)Movimento.TipoMovimentoEnum.DEVOLUCAO:
                        retorno = "Devolução";
                        break;
                }

                return retorno;
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                switch (id_tipo_movimento)
                {
                    case (int)Movimento.TipoMovimentoEnum.ENTRADA:
                        retorno.Add(new DefinicaoRelatorio("dc_numero_serie", "Número Entrada", AlinhamentoColuna.Left, "0.9000in"));
                        retorno.Add(new DefinicaoRelatorio("no_pessoa", "Fornecedor", AlinhamentoColuna.Left, "2.4000in"));
                        retorno.Add(new DefinicaoRelatorio("dta_emissao_movimento", "Movimento", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dta_mov_movimento", "Emissão", AlinhamentoColuna.Left));
                        retorno.Add(new DefinicaoRelatorio("dc_datas_titulos", "Vencimentos", AlinhamentoColuna.Left, "1.1000in"));
                        retorno.Add(new DefinicaoRelatorio("vl_qtd_total_geral", "Total Geral", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dc_tipo_financeiro", "Tipo Financeiro", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dc_politica_comercial", "Forma de Pagamento", AlinhamentoColuna.Left, "1.0000in"));
                        if (id_nf)
                            retorno.Add(new DefinicaoRelatorio("status_nf_pesq", "Status NF", AlinhamentoColuna.Center, "0.8000in"));
                        break;
                    case (int)Movimento.TipoMovimentoEnum.SAIDA:
                        retorno.Add(new DefinicaoRelatorio("dc_numero_serie", "Número Saída", AlinhamentoColuna.Left, "0.9000in"));
                        retorno.Add(new DefinicaoRelatorio("no_pessoa", "Cliente", AlinhamentoColuna.Left, "2.4000in"));
                        retorno.Add(new DefinicaoRelatorio("dta_emissao_movimento", "Movimento", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dta_mov_movimento", "Emissão", AlinhamentoColuna.Left));
                        retorno.Add(new DefinicaoRelatorio("dc_datas_titulos", "Vencimentos", AlinhamentoColuna.Left, "1.1000in"));
                        retorno.Add(new DefinicaoRelatorio("vl_qtd_total_geral", "Total Geral", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dc_tipo_financeiro", "Tipo Financeiro", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dc_politica_comercial", "Política de Saída", AlinhamentoColuna.Left, "1.0000in"));
                        if (id_nf)
                            retorno.Add(new DefinicaoRelatorio("status_nf_pesq", "Status NF", AlinhamentoColuna.Center, "0.8000in"));
                        break;
                    case (int)Movimento.TipoMovimentoEnum.SERVICO:
                        retorno.Add(new DefinicaoRelatorio("dc_numero_serie", "Número Saída", AlinhamentoColuna.Left, "0.9000in"));
                        retorno.Add(new DefinicaoRelatorio("no_pessoa", "Cliente", AlinhamentoColuna.Left, "2.3000in"));
                        retorno.Add(new DefinicaoRelatorio("dta_emissao_movimento", "Movimento", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dta_mov_movimento", "Emissão", AlinhamentoColuna.Left));
                        retorno.Add(new DefinicaoRelatorio("dc_datas_titulos", "Vencimentos", AlinhamentoColuna.Left, "1.1000in"));
                        retorno.Add(new DefinicaoRelatorio("vl_qtd_total_geral", "Total Geral", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dc_tipo_financeiro", "Tipo Financeiro", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dc_politica_comercial", "Política de Venda", AlinhamentoColuna.Left, "0.9000in"));
                        if (id_nf)
                            retorno.Add(new DefinicaoRelatorio("status_nf_pesq", "Status NF", AlinhamentoColuna.Center, "0.8000in"));
                        break;
                    case (int)Movimento.TipoMovimentoEnum.DESPESA:
                        retorno.Add(new DefinicaoRelatorio("dc_numero_serie", "Número Despesa", AlinhamentoColuna.Left, "0.9000in"));
                        retorno.Add(new DefinicaoRelatorio("no_pessoa", "Fornecedor", AlinhamentoColuna.Left, "2.4000in"));
                        retorno.Add(new DefinicaoRelatorio("dta_emissao_movimento", "Movimento", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dta_mov_movimento", "Emissão", AlinhamentoColuna.Left));
                        retorno.Add(new DefinicaoRelatorio("dc_datas_titulos", "Vencimentos", AlinhamentoColuna.Left, "1.1000in"));
                        retorno.Add(new DefinicaoRelatorio("vl_qtd_total_geral", "Total Geral", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dc_tipo_financeiro", "Tipo Financeiro", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dc_politica_comercial", "Forma de Pagamento", AlinhamentoColuna.Left, "1.0000in"));
                        break;
                    case (int)Movimento.TipoMovimentoEnum.DEVOLUCAO:
                        retorno.Add(new DefinicaoRelatorio("dc_numero_serie", "Número Saída", AlinhamentoColuna.Left, "0.9000in"));
                        retorno.Add(new DefinicaoRelatorio("no_pessoa", "Cliente", AlinhamentoColuna.Left, "2.3000in"));
                        retorno.Add(new DefinicaoRelatorio("dta_emissao_movimento", "Movimento", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dta_mov_movimento", "Emissão", AlinhamentoColuna.Left));
                        retorno.Add(new DefinicaoRelatorio("dc_datas_titulos", "Vencimentos", AlinhamentoColuna.Left, "1.1000in"));
                        retorno.Add(new DefinicaoRelatorio("vl_qtd_total_geral", "Total Geral", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dc_tipo_financeiro", "Tipo Financeiro", AlinhamentoColuna.Left, "1.0000in"));
                        retorno.Add(new DefinicaoRelatorio("dc_politica_comercial", "Política de Venda", AlinhamentoColuna.Left, "0.9000in"));
                        if (id_nf)
                            retorno.Add(new DefinicaoRelatorio("status_nf_pesq", "Status NF", AlinhamentoColuna.Center, "0.8000in"));
                        break;
                }
                return retorno;
            }
        }
    }
}