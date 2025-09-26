using System;
using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.Models.Matricula
{
    public class MatriculaUpdateAditamentosModel
    {
        public int? cd_aditamento { get; set; }                         
        public int cd_contrato { get; set; }                           
        public int id_tipo_data_inicio { get; set; }                   
        public int vl_aula_hora { get; set; }                          
        public int nm_titulos_aditamento { get; set; }                 
        public int cd_usuario { get; set; }                            
        public decimal vl_aditivo { get; set; }                        
        public decimal vl_parcela_titulo_aditamento { get; set; }      
        public int id_ajuste_manual { get; set; }                      
        public DateTime? dt_aditamento { get; set; }                   
        public DateTime? dt_inicio_aditamento { get; set; }            
        public string? nm_dia_vcto_desconto { get; set; }              
        public int? cd_nome_contrato { get; set; }                      
        public int id_tipo_aditamento { get; set; }                    
        public DateTime? nm_previsao_inicial { get; set; }             
        public int id_tipo_pagamento { get; set; }                     
        public DateTime? dt_vcto_aditamento { get; set; }              
        public string? tx_obs_aditamento { get; set; }                 
        public int cd_reajuste_anual { get; set; }                     
        public DateTime? dt_vencto_inicial { get; set; }               
        public int cd_tipo_financeiro { get; set; }                    
        public decimal vl_saldo_aberto { get; set; }                   
        public decimal vl_anterior { get; set; }                       
        public string? nm_sequencia_aditamento { get; set; }

        // Bolsa
        public int? cd_motivo_bolsa { get; set; }
        public string? dc_validade_bolsa { get; set; }
        public DateTime? dt_comunicado_bolsa { get; set; }
        public double? pc_bolsa { get; set; }


        // Desconto
        public double? pc_desconto_contrato { get; set; }
        public decimal? vl_desconto_contrato { get; set; }

        public List<TituloModelAditamento>? TitulosMensalidade { get; set; }
        public List<TituloModelAditamento>? TitulosMaterial { get; set; }



        public class TituloModelAditamento
        {
            public int? cd_titulo { get; set; }
            public int cd_pessoa_titulo { get; set; }
            public int cd_pessoa_responsavel { get; set; }
            public int cd_local_movto { get; set; }
            public DateTime dt_emissao_titulo { get; set; }
            public int cd_origem_titulo { get; set; }
            public DateTime dt_vcto_titulo { get; set; }
            public decimal vl_titulo { get; set; }
            public decimal vl_saldo_titulo { get; set; }
            public string? dc_tipo_titulo { get; set; }
            public string? dc_num_documento_titulo { get; set; }
            public string? nm_parcela_titulo { get; set; }
            public int cd_tipo_financeiro { get; set; }
            public bool id_status_cnab { get; set; }
            public decimal vl_material_titulo { get; set; }
            public decimal pc_taxa_cartao { get; set; }
            public int nm_dias_cartao { get; set; }
            public bool id_cnab_contrato { get; set; }
            public decimal vl_taxa_cartao { get; set; }
            public int? cd_aluno { get; set; }
            public decimal? pc_responsavel { get; set; }
            public decimal? vl_mensalidade { get; set; }
            public decimal? pc_bolsa { get; set; }
            public decimal? vl_bolsa { get; set; }
            public decimal? pc_desconto_mensalidade { get; set; }
            public decimal? vl_desconto_mensalidade { get; set; }
            public decimal? pc_bolsa_material { get; set; }
            public decimal? vl_bolsa_material { get; set; }
            public decimal? pc_desconto_material { get; set; }
            public decimal? vl_desconto_material { get; set; }
            public decimal? pc_desconto_total { get; set; }
            public decimal? vl_desconto_total { get; set; }
            public string? opcao_venda { get; set; }
            public int? cd_curso { get; set; }
        }
    }
}
