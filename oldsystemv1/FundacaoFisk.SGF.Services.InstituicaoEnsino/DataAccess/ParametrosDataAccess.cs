using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.DataAccess
{

    public class ParametrosDataAccess :GenericRepository<Parametro>, IParametrosDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public Parametro getParametrosByEscola(int cdEscola)
        {
            try{
                return (from p in db.Parametro where p.cd_pessoa_escola == cdEscola select p).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Parametro getParametrosOpcaoPagamento(int cd_escola) {
            try {
                return (from p in db.Parametro
                        where p.cd_pessoa_escola == cd_escola
                        select new {
                            nm_dia_vencimento = p.nm_dia_vencimento,
                            id_alterar_venc_final_semana = p.id_alterar_venc_final_semana,
                            id_dia_util_vencimento = p.id_dia_util_vencimento
                        }).ToList().Select(x => new Parametro {
                            nm_dia_vencimento = x.nm_dia_vencimento,
                            id_alterar_venc_final_semana = x.id_alterar_venc_final_semana,
                            id_dia_util_vencimento = x.id_dia_util_vencimento
                        }).FirstOrDefault();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public Parametro getParametrosMatricula(int cdEscola)
        {
            try
            {
                return (from p in db.Parametro
                        where p.cd_pessoa_escola == cdEscola
                        select new
                        {
                            cd_pessoa_escola = p.cd_pessoa_escola,
                            id_nro_contrato_automatico = p.id_nro_contrato_automatico,
                            nm_ultimo_contrato = p.nm_ultimo_contrato,
                            id_tipo_numero_contrato = p.id_tipo_numero_contrato,
                            nm_ultimo_matricula = p.nm_ultimo_matricula,
                            cd_local_movto = p.cd_local_movto,
                            id_dia_util_vencimento = p.id_dia_util_vencimento,
                            nm_dia_vencimento = p.nm_dia_vencimento,
                            id_requer_plano_contas_mov = p.id_requer_plano_contas_mov,
                            id_alterar_venc_final_semana = p.id_alterar_venc_final_semana,
                            cd_plano_conta_mat = p.cd_plano_conta_mat,
                            cd_plano_conta_tax = p.cd_plano_conta_tax,
                            desc_plano_conta_mat = p.ParametroPlanoContaMatricula.PlanoContaSubgrupo.no_subgrupo_conta,
                            desc_plano_conta_tax = p.ParametroPlanoContaTaxa.PlanoContaSubgrupo.no_subgrupo_conta,
                            id_gerar_financeiro_contrato = p.id_gerar_financeiro_contrato,
                            id_somar_descontos_financeiros = p.id_somar_descontos_financeiros,
                            id_empresa_propria = p.Escola.id_empresa_propria,
                            dc_local_movto = p.LocalMovimento.no_local_movto
                            
                         }).ToList().Select(x => new Parametro {
                             cd_pessoa_escola = x.cd_pessoa_escola,
                             id_nro_contrato_automatico = x.id_nro_contrato_automatico,
                             nm_ultimo_contrato = x.nm_ultimo_contrato,
                             id_tipo_numero_contrato = x.id_tipo_numero_contrato,
                             nm_ultimo_matricula = x.nm_ultimo_matricula,
                             cd_local_movto = x.cd_local_movto,
                             id_dia_util_vencimento = x.id_dia_util_vencimento,
                             nm_dia_vencimento = x.nm_dia_vencimento,
                             id_requer_plano_contas_mov = x.id_requer_plano_contas_mov,
                             id_alterar_venc_final_semana = x.id_alterar_venc_final_semana,
                             cd_plano_conta_mat = x.cd_plano_conta_mat,
                             cd_plano_conta_tax = x.cd_plano_conta_tax,
                             desc_plano_conta_mat = x.desc_plano_conta_mat,
                             desc_plano_conta_tax = x.desc_plano_conta_tax,
                             id_gerar_financeiro_contrato = x.id_gerar_financeiro_contrato,
                             id_somar_descontos_financeiros = x.id_somar_descontos_financeiros,
                             id_empresa_propria = x.id_empresa_propria,
                             dc_local_movto = x.dc_local_movto
                             
                        }).FirstOrDefault();

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Parametro getParametrosPlanoTxMatricula(int cdEscola)
        {
            try
            {
                return (from p in db.Parametro
                        where p.cd_pessoa_escola == cdEscola
                        select new
                        {
                            cd_plano_conta_tax = p.cd_plano_conta_tax,
                            desc_plano_conta_tax = p.ParametroPlanoContaTaxa.PlanoContaSubgrupo.no_subgrupo_conta
                        }).ToList().Select(x => new Parametro
                        {
                            cd_plano_conta_tax = x.cd_plano_conta_tax,
                            desc_plano_conta_tax = x.desc_plano_conta_tax
                        }).FirstOrDefault();

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Parametro getParametrosBiblioteca(int cd_empresa) {
            try {
                return (from p in db.Parametro
                        where p.cd_pessoa_escola == cd_empresa
                        select new {
                            pc_taxa_dia_biblioteca = p.pc_taxa_dia_biblioteca,
                            nm_dias_biblioteca = p.nm_dias_biblioteca,
                            id_bloquear_alt_dta_biblio = p.id_bloquear_alt_dta_biblio
                        }).ToList().Select(x => new Parametro {
                            pc_taxa_dia_biblioteca = x.pc_taxa_dia_biblioteca,
                            nm_dias_biblioteca = x.nm_dias_biblioteca,
                            id_bloquear_alt_dta_biblio = x.id_bloquear_alt_dta_biblio
                        }).FirstOrDefault();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public Parametro getParametrosBaixa(int cd_escola) {
            try {
                return (from p in db.Parametro
                        where p.cd_pessoa_escola == cd_escola
                        select new {
                            id_somar_descontos_financeiros = p.id_somar_descontos_financeiros,
                            nm_dias_carencia = p.nm_dias_carencia,
                            id_cobrar_juros_multa = p.id_cobrar_juros_multa,
                            pc_juros_dia = p.pc_juros_dia,
                            pc_multa = p.pc_multa,
                            per_desconto_maximo = p.per_desconto_maximo,
                            id_permitir_desc_apos_politica = p.id_permitir_desc_apos_politica,
                            id_alterar_venc_final_semana = p.id_alterar_venc_final_semana,
                            id_juros_final_semana = p.id_juros_final_semana
                        }).ToList().Select(x => new Parametro {
                            id_somar_descontos_financeiros = x.id_somar_descontos_financeiros,
                            nm_dias_carencia = x.nm_dias_carencia,
                            id_cobrar_juros_multa = x.id_cobrar_juros_multa,
                            pc_juros_dia = x.pc_juros_dia,
                            pc_multa = x.pc_multa,
                            per_desconto_maximo = x.per_desconto_maximo,
                            id_permitir_desc_apos_politica = x.id_permitir_desc_apos_politica,
                            id_alterar_venc_final_semana = x.id_alterar_venc_final_semana,
                            id_juros_final_semana = x.id_juros_final_semana
                        }).FirstOrDefault();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public int? getLocalMovto(int cd_escola)
        {
            try
            {
                int? cd_local_movto = (from p in db.Parametro
                                      where p.cd_pessoa_escola == cd_escola
                                      select p.cd_local_movto).FirstOrDefault();
                return cd_local_movto;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Parametro getParametrosMovimento(int cd_escola)
         {
            int? cdEmpresaLig = (from empresa in db.PessoaSGF.OfType<Escola>() where empresa.cd_pessoa == cd_escola select empresa.cd_empresa_coligada).FirstOrDefault();
            cd_escola = (int)(cdEmpresaLig == null ? cd_escola : cdEmpresaLig);
            try
            {
                 return (from p in db.Parametro
                         where p.cd_pessoa_escola == cd_escola
                         select new
                         {
                             cd_pessoa_escola = p.cd_pessoa_escola,
                             id_alterar_venc_final_semana = p.id_alterar_venc_final_semana,
                             id_requer_plano_contas_mov = p.id_requer_plano_contas_mov,
                             cd_local_movto = p.cd_local_movto,
                             cd_plano_conta_tax = p.cd_plano_conta_tax,
                             desc_plano_conta_tax = p.ParametroPlanoContaTaxa.PlanoContaSubgrupo.no_subgrupo_conta,
                             id_numero_nf_automatico = p.id_numero_nf_automatico,
                             id_emitir_nf_mercantil = p.id_emitir_nf_mercantil,
                             id_emitir_nf_servico = p.id_emitir_nf_servico,
                             nm_nf_mercantil = p.nm_nf_mercantil,
                             nm_nf_servico = p.nm_nf_servico,
                             cd_tipo_nf_material = p.cd_tipo_nf_material,
                             cd_tipo_nf_biblioteca = p.cd_tipo_nf_biblioteca,
                             cd_tipo_nf_matricula = p.cd_tipo_nf_matricula,
                             cd_item_biblioteca = p.cd_item_biblioteca,
                             cd_item_mensalidade = p.cd_item_mensalidade,
                             cd_item_taxa_matricula = p.cd_item_taxa_matricula,
                             cd_politica_comercial_nf = p.cd_politica_comercial_nf,
                             desc_politica_comercial_nf = p.PoliticaComercial.dc_politica_comercial,
                             desc_item_mensalidade = p.ItemMensalidade != null ? p.ItemMensalidade.no_item : "",
                             desc_item_taxa_matricula = p.ItemTaxaMens != null ? p.ItemTaxaMens.no_item : "",
                             desc_item_biblioteca = p.ItemBiblioteca != null ? p.ItemBiblioteca.no_item : "",
                             pc_aliquota_ap_servico = p.pc_aliquota_ap_servico,
                             pc_aliquota_ap_saida = p.pc_aliquota_ap_saida,
                             p.Escola.id_empresa_propria
                         }).ToList().Select(x => new Parametro
                         {
                             cd_pessoa_escola = x.cd_pessoa_escola,
                             id_alterar_venc_final_semana = x.id_alterar_venc_final_semana,
                             id_requer_plano_contas_mov = x.id_requer_plano_contas_mov,
                             cd_local_movto = x.cd_local_movto,
                             cd_plano_conta_tax = x.cd_plano_conta_tax,
                             desc_plano_conta_tax = x.desc_plano_conta_tax,
                             id_numero_nf_automatico = x.id_numero_nf_automatico,
                             id_emitir_nf_mercantil = x.id_emitir_nf_mercantil,
                             id_emitir_nf_servico = x.id_emitir_nf_servico,
                             nm_nf_mercantil = x.nm_nf_mercantil,
                             nm_nf_servico = x.nm_nf_servico,
                             cd_tipo_nf_material = x.cd_tipo_nf_material,
                             cd_tipo_nf_biblioteca = x.cd_tipo_nf_biblioteca,
                             cd_tipo_nf_matricula = x.cd_tipo_nf_matricula,
                             cd_item_biblioteca = x.cd_item_biblioteca,
                             cd_item_mensalidade = x.cd_item_mensalidade,
                             cd_item_taxa_matricula = x.cd_item_taxa_matricula,
                             cd_politica_comercial_nf = x.cd_politica_comercial_nf,
                             desc_politica_comercial_nf = x.desc_politica_comercial_nf,
                             desc_item_mensalidade = x.desc_item_mensalidade,
                             desc_item_taxa_matricula = x.desc_item_taxa_matricula,
                             desc_item_biblioteca = x.desc_item_biblioteca,
                             pc_aliquota_ap_servico = x.pc_aliquota_ap_servico,
                             pc_aliquota_ap_saida = x.pc_aliquota_ap_saida,
                             Escola = new Escola { id_empresa_propria = x.id_empresa_propria }
                         }).FirstOrDefault();
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

        public bool getIdBloquearVendasSemEstoque(int cd_empresa)
         {
             try
             {
                 return (from p in db.Parametro
                         where p.cd_pessoa_escola == cd_empresa
                         select p.id_bloquear_venda_sem_estoque).FirstOrDefault();
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

        public bool getIdBloquearliqTituloAnteriorAberto(int cd_empresa)
         {
             try
             {
                 return (from p in db.Parametro
                         where p.cd_pessoa_escola == cd_empresa
                         select p.id_liquidacao_tit_ant_aberto).FirstOrDefault();
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

        public byte? getParametroNiviesPlanoConta(int cdEscola)
         {
             try
             {
                 return (from p in db.Parametro where p.cd_pessoa_escola == cdEscola select p.nm_niveis_plano_contas).FirstOrDefault();
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

        public int? getParametroNiveisPlanoContas(int cd_escola) {
             try {
                 return (from p in db.Parametro
                         where p.cd_pessoa_escola == cd_escola
                         select p.nm_niveis_plano_contas).FirstOrDefault();
             }
             catch(Exception exe) {
                 throw new DataAccessException(exe);
             }
         }
        public int getParametroMovimentoRetroativo(int cd_escola)
         {
             try
             {
                 return (from p in db.Parametro
                         where p.cd_pessoa_escola == cd_escola
                         select p.id_retroativo_caixa).FirstOrDefault();
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

        public bool getParametroBloquearMovtoRetroativoEst(int cd_escola)
         {
             try
             {
                 return (from p in db.Parametro
                         where p.cd_pessoa_escola == cd_escola
                         select p.id_bloquear_mov_retr_estoque).FirstOrDefault();
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

        public List<Parametro> getParametroNiviesPlanoEscola(int[] cdEscolas)
        {
            try
            {
                List<Parametro> nivelEscola = (from p in db.Parametro
                                               where cdEscolas.Contains(p.cd_pessoa_escola)
                                               select new
                                               {
                                                   nm_niveis_plano_contas = p.nm_niveis_plano_contas,
                                                   cd_pessoa_escola = p.cd_pessoa_escola
                                               }).ToList().Select(x => new Parametro
                                               {
                                                   nm_niveis_plano_contas = x.nm_niveis_plano_contas,
                                                   cd_pessoa_escola = x.cd_pessoa_escola
                                               }).ToList();
                return nivelEscola;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public byte? getParametrosPrevDevolucao(int cd_escola)
        {
            try
            {
                return (from p in db.Parametro
                        where p.cd_pessoa_escola == cd_escola
                        select p.nm_dias_biblioteca).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool getParametroNumeracaoAutoNF(int cd_escola)
        {
            try
            {
                bool retorno = false;
                retorno = (from p in db.Parametro
                        where p.cd_pessoa_escola == cd_escola
                        select p.id_numero_nf_automatico).FirstOrDefault();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Parametro getNumeroESerieNFPorTipo(Movimento.TipoMovimentoEnum tipo, int cd_emprea)
        {
            Parametro retorno = new Parametro();
            switch (tipo)
            {
                case Movimento.TipoMovimentoEnum.SAIDA:
                    {
                        retorno.nm_nf_mercantil = (from c in db.Parametro
                                                   where c.cd_pessoa_escola == cd_emprea
                                                   select c.nm_nf_mercantil).FirstOrDefault();
                        if (retorno.nm_nf_mercantil == null)
                            retorno.nm_nf_mercantil = 1;
                        else
                            retorno.nm_nf_mercantil += 1;
                        retorno.dc_serie_nf_mercantil = (from p in db.Parametro
                                                         where p.cd_pessoa_escola == cd_emprea
                                                         select p.dc_serie_nf_mercantil).FirstOrDefault();
                        break;
                    }
                case Movimento.TipoMovimentoEnum.SERVICO:
                    {
                        retorno.nm_nf_servico = (from c in db.Parametro
                                                 where c.cd_pessoa_escola == cd_emprea
                                                 select c.nm_nf_servico).FirstOrDefault();
                        if (retorno.nm_nf_servico == null)
                            retorno.nm_nf_servico = 1;
                        else
                            retorno.nm_nf_servico += 1;
                        retorno.dc_serie_nf_servico = (from p in db.Parametro
                                                       where p.cd_pessoa_escola == cd_emprea
                                                       select p.dc_serie_nf_servico).FirstOrDefault();
                        break;
                    }
            }

            return retorno;
        }

        public byte? getParametroNmAulasSemMaterial(int cd_escola)
        {
            try
            {
                return (from p in db.Parametro
                        where p.cd_pessoa_escola == cd_escola
                        select p.nm_aulas_sem_material).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool getEmitirNFServico(int cd_empresa)
        {
            try
            {
                return (from p in db.Parametro
                        where p.cd_pessoa_escola == cd_empresa
                        select p.id_emitir_nf_servico).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public byte getParametroRegimeTrib(int cd_escola)
        {
            try
            {
                var retorno = (from p in db.Parametro
                        where p.cd_pessoa_escola == cd_escola
                        select p.id_regime_tributario).FirstOrDefault();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeParametroTpNF(int cd_tipo_nf)
        {
            try
            {
                return (from p in db.Parametro
                        where p.cd_tipo_nf_matricula == cd_tipo_nf || p.cd_tipo_nf_material == cd_tipo_nf || p.cd_tipo_nf_biblioteca == cd_tipo_nf
                        select p.cd_pessoa_escola).Any();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int getParametroNmFaltasAluno(int cd_escola)
        {
            try
            {
                return (from p in db.Parametro
                        where p.cd_pessoa_escola == cd_escola
                        select p.nm_faltas_aluno).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool getImprimir3BoletosPagina(int cd_escola)
        {
            try
            {
                return (from p in db.Parametro
                        where p.cd_pessoa_escola == cd_escola
                        select p.id_mostrar_3_boletos_pagina).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public byte? getTipoNumeroContrato(int cd_empresa)
        {
            try
            {
                return (from p in db.Parametro
                        where p.cd_pessoa_escola == cd_empresa
                        select p.id_tipo_numero_contrato).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public byte getParametroNmDiasTitulosAbertos(int cd_escola)
        {
            try
            {
                return (from p in db.Parametro
                        where p.cd_pessoa_escola == cd_escola
                        select p.nm_dias_titulos_abertos).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool getParametroHabilitacaoProfessor(int cd_escola)
        {
            try
            {
                bool habilProf = (bool)(from p in db.Parametro
                                        where p.cd_pessoa_escola == cd_escola
                                        select p.id_liberar_habilitacao_professor).FirstOrDefault();
                return habilProf;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        
    }

}

