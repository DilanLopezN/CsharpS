using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using System.Data.Entity;
using System.Web.Mvc.Html;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class MovimentoDataAccess : GenericRepository<Movimento>, IMovimentoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public enum OrigemPerdaMaterialEnum
        {
            ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL = 1,
            ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL = 2,
          
        }

        public enum OrigemItemMovimentoPerdaMaterialEnum
        {
            ORIGEM_ITEM_SEARCH_PERDA_MATERIAL = 44,
            ORIGEM_ITEM_FK_MOVIMENTO = 45,
            ORIGEM_ITEM_CAD_FK_MOVIMENTO = 46,

        }


        public IEnumerable<MovimentoUI> searchMovimento(SearchParameters parametros, int id_tipo_movimento, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie, int cd_empresa,
                                                       bool emissao, bool movimento, DateTime? dtInicial, DateTime? dtFinal, bool nota_fiscal, int statusNF, bool contaSegura, int isImportXML, 
                                                       bool? id_material_didatico, bool? id_venda_futura)
        {
            try
            {
                int? cdEmpresaLig = (from empresa in db.PessoaSGF.OfType<Escola>() where empresa.cd_pessoa == cd_empresa select empresa.cd_empresa_coligada).FirstOrDefault();
                cd_empresa = (int)(cdEmpresaLig == null ? cd_empresa : cdEmpresaLig);
                int cd_origem = Int32.Parse(new SGFWebContext().LISTA_ORIGEM_LOGS["Movimento"].ToString());
                IEntitySorter<MovimentoUI> sorter = EntitySorter<MovimentoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Movimento> sql;
                sql = from m in db.Movimento.AsNoTracking()
                      where m.cd_pessoa_empresa == cd_empresa && m.id_tipo_movimento == id_tipo_movimento && m.id_nf == nota_fiscal
                      select m;

                //if (isimportxml == 0)
                //    sql = from m in sql
                //          where m.id_importacao_xml == false // convert.toboolean((int)movimento.tipoimportacaoxml.inativo)
                //          select m;

                if (isImportXML == 1)
                    sql = from m in sql
                          where m.id_importacao_xml == true //Convert.ToBoolean((int)Movimento.TipoImportacaoXML.ATIVO)
                          select m;

                if (cd_pessoa > 0)
                    sql = from da in sql
                          where da.cd_pessoa == cd_pessoa
                          select da;
                if (numero > 0)
                    sql = from da in sql
                          where da.nm_movimento == numero
                          select da;
                if (!string.IsNullOrEmpty(serie))
                    sql = from da in sql
                          where da.dc_serie_movimento == serie
                          select da;

                if (cd_item > 0)
                    sql = from m in sql
                          where (from im in m.ItensMovimento where im.cd_item == cd_item select im.cd_item).Any()
                          select m;

                if (cd_plano_conta > 0)
                    sql = from m in sql
                          where (from im in m.ItensMovimento where im.cd_plano_conta == cd_plano_conta select im.cd_plano_conta).Any()
                          select m;

                if (dtInicial.HasValue)
                {
                    if (emissao)
                        sql = from t in sql
                              where t.dt_emissao_movimento >= dtInicial
                              select t;
                    else
                        sql = from t in sql
                              where t.dt_mov_movimento >= dtInicial
                              select t;
                }

                if (dtFinal.HasValue)
                {
                    if (emissao)
                        sql = from t in sql
                              where t.dt_emissao_movimento <= dtFinal
                              select t;
                    else
                        sql = from t in sql
                              where t.dt_mov_movimento <= dtFinal
                              select t;
                }
                if (nota_fiscal && statusNF > 0)
                    sql = from t in sql
                          where t.id_status_nf == statusNF
                          select t;

                if (!contaSegura)
                    sql = from t in sql
                          where !t.ItensMovimento.Any(m => m.PlanoConta.id_conta_segura == true)
                          select t;

                if (id_material_didatico == true)
                {
                    sql = from mm in sql
                          where mm.id_material_didatico == true
                          select mm;
                }

                if (id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA && id_venda_futura == true)
                    sql = from m in sql
                          where m.id_venda_futura == true //Convert.ToBoolean((int)Movimento.TipoImportacaoXML.ATIVO)
                          select m;

                if (id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA && id_material_didatico == false)
                {
                    sql = from mm in sql
                          where mm.id_material_didatico == false
                          select mm;
                }

                var sqlSearch = (from t in sql
                                 select new MovimentoUI
                                 {
                                     nm_movimento = t.nm_movimento,
                                     dc_serie_movimento = t.dc_serie_movimento,
                                     dt_emissao_movimento = t.dt_emissao_movimento,
                                     dt_vcto_movimento = t.dt_vcto_movimento,
                                     dt_mov_movimento = t.dt_mov_movimento,
                                     id_tipo_movimento = t.id_tipo_movimento,
                                     id_status_nf = t.id_status_nf,
                                     cd_movimento = t.cd_movimento,
                                     no_pessoa = t.Pessoa.no_pessoa,
                                     dc_politica_comercial = t.PoliticaComercial.dc_politica_comercial,
                                     dc_tipo_financeiro = t.TipoFinanceiro.dc_tipo_financeiro,
                                     id_nf = t.id_nf,
                                     dc_meio_pagamento = (t.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA) ? t.dc_meio_pagamento : null,
                                     id_tipo_mvto_nf_dev = t.MovimentoDevolucao != null && t.MovimentoDevolucao.cd_movimento > 0 ? t.MovimentoDevolucao.id_tipo_movimento : (byte)0,
                                     id_natureza_mvto_tp_nf = t.TipoNF.id_natureza_movimento,
                                     nm_movimento_dev = t.MovimentoDevolucao != null && t.MovimentoDevolucao.cd_movimento > 0 ? t.MovimentoDevolucao.nm_movimento : 0,
                                     qtd_total_geral = t.ItensMovimento.Sum(x => x.vl_liquido_item),
                                     //datas_vencimento_titulo = db.Titulo.Where(i => i.cd_pessoa_empresa == cd_empresa && i.id_origem_titulo == cd_origem && i.cd_origem_titulo == t.cd_movimento).Select(x => x.dt_vcto_titulo)
                                     datas_vencimento_titulo = from i in db.Titulo
                                                   where i.cd_pessoa_empresa == cd_empresa && i.id_origem_titulo == cd_origem && i.cd_origem_titulo == t.cd_movimento
                                                   select i.dt_vcto_titulo
                                 });

                sqlSearch = sorter.Sort(sqlSearch);
                int limite = sqlSearch.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sqlSearch = sqlSearch.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                var retorno = sqlSearch.ToList();
                //foreach (var movt in retorno)
                //    movt.datas_vencimento_titulo = from i in db.Titulo
                //                                   where i.cd_pessoa_empresa == cd_empresa && i.id_origem_titulo == cd_origem && i.cd_origem_titulo == movt.cd_movimento
                //                                   select i.dt_vcto_titulo;

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ItemUI> getItemMovimentoSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int? tipoItem, int? cdGrupoItem, int cdEscola,
            int id_tipo_movto, Movimento.TipoVinculoMovimento tipoVinc, bool comEstoque, int? id_natureza_TPNF, bool kit, bool contaSegura, int? cd_movimento, int? cd_aluno, DateTime? dt_inicial, DateTime? dt_final, bool? vinculado_curso, int? cd_curso_material_didatico)
        {
            try
            {
                cdGrupoItem = cdGrupoItem == 0 ? null : cdGrupoItem;
                tipoItem = tipoItem == 0 ? null : tipoItem;
                IEntitySorter<ItemUI> sorter = EntitySorter<ItemUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Item> sql;

                sql = from item in db.Item.Include("ItensMovimento.PlanoConta").AsNoTracking()
                      where (cdGrupoItem == null || item.cd_grupo_estoque == cdGrupoItem)
                         && (tipoItem == null || item.cd_tipo_item == tipoItem)
                         && (item.ItemEscola.Where(e => e.cd_pessoa_escola == cdEscola).Any())
                      select item;
                int tipoSubgrupo = 0;

                if (vinculado_curso == true && cd_curso_material_didatico > 0)
                {
                    sql = from itm in sql
                          where itm.Cursos.Any(x => x.cd_curso == cd_curso_material_didatico) 
                          select itm;
                }

                if (cd_aluno != null && cd_aluno > 0)
                {
                    sql = from itm in sql
                          where itm.ItensMovimento.Any(mov => mov.Movimento.cd_aluno == cd_aluno)
                          select itm;
                }

                if (dt_inicial != null && dt_final != null)
                {
                    sql = from itm in sql
                          where itm.ItensMovimento.Any(mov => DbFunctions.TruncateTime(mov.Movimento.dt_emissao_movimento) >= dt_inicial.Value && 
                              DbFunctions.TruncateTime(mov.Movimento.dt_emissao_movimento) <= dt_final.Value)
                          select itm;
                }

                switch (id_tipo_movto)
                {

                    case (int)Movimento.TipoMovimentoEnum.ENTRADA:
                        {
                            tipoSubgrupo = (int)ItemSubgrupo.TipoSubgGrupo.ENTRADA;
                            break;
                        }
                    case (int)Movimento.TipoMovimentoEnum.SAIDA:
                        {
                            tipoSubgrupo = (int)ItemSubgrupo.TipoSubgGrupo.SAIDA;
                            break;
                        }
                    case (int)Movimento.TipoMovimentoEnum.DESPESA:
                        {
                            tipoSubgrupo = (int)ItemSubgrupo.TipoSubgGrupo.DESPESA;
                            break;
                        }
                    case (int)Movimento.TipoMovimentoEnum.SERVICO:
                        {
                            if (id_natureza_TPNF == (int)Movimento.TipoMovimentoEnum.ENTRADA)
                                tipoSubgrupo = (int)ItemSubgrupo.TipoSubgGrupo.SERVICO_ENTRADA;
                            else
                                if (id_natureza_TPNF == (int)Movimento.TipoMovimentoEnum.SAIDA)
                                    tipoSubgrupo = (int)ItemSubgrupo.TipoSubgGrupo.SERVICO_SAIDA;
                            break;
                        }
                    case (int)Movimento.TipoMovimentoEnum.DEVOLUCAO:
                        {
                            if (id_natureza_TPNF == (int)Movimento.TipoMovimentoEnum.ENTRADA)
                                tipoSubgrupo = (int)ItemSubgrupo.TipoSubgGrupo.DEVOLUCAO_ENTRADA;
                            else
                                if (id_natureza_TPNF == (int)Movimento.TipoMovimentoEnum.SAIDA)
                                    tipoSubgrupo = (int)ItemSubgrupo.TipoSubgGrupo.DEVOLUCAO_SAIDA;
                            break;
                        }
                }
                // Verificar permissão "Conta Segura".
                if (!contaSegura)
                    sql = from t in sql
                          where
                           (!t.ItemSubgrupos.Any() || db.PlanoConta.Where(pc => t.ItemSubgrupos.Where(isg => isg.cd_item == t.cd_item && pc.cd_subgrupo_conta == isg.cd_subgrupo_conta).Any() &&
                                                                       t.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).Any() && pc.cd_pessoa_empresa == cdEscola && !pc.id_conta_segura).Any())
                          select t;
                //regra para cadastro movimento
                if (tipoVinc == Movimento.TipoVinculoMovimento.PESQUISA_ITEM_CADASTRO_MOVIMENTO && (tipoItem == null || tipoItem == 0))
                    switch (id_tipo_movto)
                    {
                        case (int)Movimento.TipoMovimentoEnum.SAIDA:
                        case (int)Movimento.TipoMovimentoEnum.ENTRADA:
                            sql = from i in sql
                                  where i.TipoItem.cd_tipo_item != (int)TipoItem.TipoItemEnum.SERVICO &&
                                        i.TipoItem.cd_tipo_item != (int)TipoItem.TipoItemEnum.CUSTOSDESPESAS
                                  select i;
                            break;
                        case (int)Movimento.TipoMovimentoEnum.DESPESA:
                            sql = from i in sql
                                  where ((i.TipoItem.cd_tipo_item == (int)TipoItem.TipoItemEnum.SERVICO) ||
                                        (i.TipoItem.cd_tipo_item == (int)TipoItem.TipoItemEnum.CUSTOSDESPESAS))
                                  select i;
                            break;
                        case (int)Movimento.TipoMovimentoEnum.SERVICO:
                            sql = from i in sql
                                  where i.TipoItem.cd_tipo_item == (int)TipoItem.TipoItemEnum.SERVICO
                                  select i;
                            break;
                        case (int)Movimento.TipoMovimentoEnum.DEVOLUCAO:
                            if (cd_movimento != null && cd_movimento > 0)
                            {
                                sql = from i in sql
                                      where i.ItensMovimento.Where(x => x.cd_movimento == cd_movimento).Any()
                                      select i;
                            }
                            break;
                    }

                if (!kit)
                {
                    if (id_tipo_movto > 0)
                    {
                        switch (tipoVinc)
                        {
                            case Movimento.TipoVinculoMovimento.PESQUISA_ITEM_MOVIMENTO:
                                sql = from item in sql
                                      where item.ItensMovimento.Where(m => m.Movimento.id_tipo_movimento == id_tipo_movto && m.Movimento.cd_pessoa_empresa == cdEscola).Any()
                                      select item;
                                break;
                            case Movimento.TipoVinculoMovimento.HAS_PESQ_NOTA_MATERIAL:
                                SGFWebContext dbComp = new SGFWebContext();
                                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                                sql = from i in sql
                                      where i.ItensMovimento.Any(im => im.Movimento.id_origem_movimento == cd_origem)
                                      select i;
                                break;
                        }
                    }
                }

                if (comEstoque)
                    sql = from item in sql
                          where item.ItemEscola.Where(ie => ie.qt_estoque > 0 && ie.cd_pessoa_escola == cdEscola).Any()
                          select item;

                if (kit)
                {
                    sql = from c in sql
                          where (c.id_kit == true)
                          select c;
                }
                else
                {
                    sql = from c in sql
                        where (c.id_kit == false)
                        select c;
                }

                if (ativo != null)
                    sql = from c in sql
                          where (c.id_item_ativo == ativo)
                          select c;

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql = from c in sql
                              where c.no_item.StartsWith(descricao)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_item.Contains(descricao)
                              select c;

                var retorno = from item in sql
                              select new ItemUI
                              {
                                  cd_grupo_estoque = item.cd_grupo_estoque,
                                  cd_item = item.cd_item,
                                  cd_tipo_item = item.cd_tipo_item,
                                  no_item = item.no_item,
                                  qt_estoque = item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).FirstOrDefault().qt_estoque,
                                  //vl_custo = item.ItemEscola.Where(i => i.cd_pessoa_escola == id_escola).FirstOrDefault().vl_custo,
                                  vl_item = item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).FirstOrDefault().vl_item,
                                  dc_tipo_item = item.TipoItem.dc_tipo_item,
                                  no_grupo_estoque = item.GrupoEstoque.no_grupo_estoque,
                                  id_item_ativo = item.id_item_ativo,
                                  cd_plano_conta = (from pc in db.PlanoConta
                                                    where item.ItemSubgrupos.Where(isg => isg.cd_item == item.cd_item && isg.id_tipo_movimento == tipoSubgrupo && pc.cd_subgrupo_conta == isg.cd_subgrupo_conta).Any() &&
                                                                                  item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).Any() &&
                                                                                  pc.cd_pessoa_empresa == cdEscola
                                                    select pc.cd_plano_conta).FirstOrDefault(),
                                  desc_plano_conta = db.PlanoConta.Where(pc => item.ItemSubgrupos.Where(isg => isg.cd_item == item.cd_item && isg.id_tipo_movimento == tipoSubgrupo && pc.cd_subgrupo_conta == isg.cd_subgrupo_conta).Any() &&
                                                                               item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).Any() && pc.cd_pessoa_empresa == cdEscola).Select(pl => pl.PlanoContaSubgrupo.no_subgrupo_conta).FirstOrDefault(),
                                  cd_curso = (item.Cursos.Any() ? item.Cursos.Select(x => x.cd_curso).FirstOrDefault() : 0),
                                  id_material_didatico = item.id_material_didatico,
                                  id_voucher_carga = item.id_voucher_carga
                              };

                retorno = sorter.Sort(retorno);

                int limite = retorno.Count();
                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public IEnumerable<ItemUI> getItemMovimentoSearchPerdaMaterial(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int? tipoItem, int? cdGrupoItem, int cdEscola,
            int id_tipo_movto, Movimento.TipoVinculoMovimento tipoVinc, bool comEstoque, int? id_natureza_TPNF, bool kit, bool contaSegura, int? cd_movimento, int origem, int? cd_aluno, DateTime? dt_inicial, DateTime? dt_final, bool? vinculado_curso, int? cd_curso_material_didatico)
        {
            try
            {
                cdGrupoItem = cdGrupoItem == 0 ? null : cdGrupoItem;
                tipoItem = tipoItem == 0 ? null : tipoItem;
                IEntitySorter<ItemUI> sorter = EntitySorter<ItemUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Item> sql;

                sql = from item in db.Item.Include("ItensMovimento.PlanoConta").AsNoTracking()
                      where (cdGrupoItem == null || item.cd_grupo_estoque == cdGrupoItem)
                         && (tipoItem == null || item.cd_tipo_item == tipoItem)
                         && (item.ItemEscola.Where(e => e.cd_pessoa_escola == cdEscola).Any())
                      select item;
                int tipoSubgrupo = 0;

                if (vinculado_curso == true && cd_curso_material_didatico > 0)
                {
                    sql = from itm in sql
                          where itm.Cursos.Any(x => x.cd_curso == cd_curso_material_didatico)
                          select itm;
                }

                if (cd_aluno != null && cd_aluno > 0)
                {
                    sql = from itm in sql
                          where itm.ItensMovimento.Any(mov => mov.Movimento.cd_aluno == cd_aluno)
                          select itm;
                }

                if (dt_inicial != null && dt_final != null)
                {
                    sql = from itm in sql
                          where itm.ItensMovimento.Any(mov => DbFunctions.TruncateTime(mov.Movimento.dt_emissao_movimento) >= dt_inicial.Value &&
                              DbFunctions.TruncateTime(mov.Movimento.dt_emissao_movimento) <= dt_final.Value)
                          select itm;
                }

                switch (id_tipo_movto)
                {

                    case (int)Movimento.TipoMovimentoEnum.ENTRADA:
                        {
                            tipoSubgrupo = (int)ItemSubgrupo.TipoSubgGrupo.ENTRADA;
                            break;
                        }
                    case (int)Movimento.TipoMovimentoEnum.SAIDA:
                        {
                            tipoSubgrupo = (int)ItemSubgrupo.TipoSubgGrupo.SAIDA;
                            break;
                        }
                    case (int)Movimento.TipoMovimentoEnum.DESPESA:
                        {
                            tipoSubgrupo = (int)ItemSubgrupo.TipoSubgGrupo.DESPESA;
                            break;
                        }
                    case (int)Movimento.TipoMovimentoEnum.SERVICO:
                        {
                            if (id_natureza_TPNF == (int)Movimento.TipoMovimentoEnum.ENTRADA)
                                tipoSubgrupo = (int)ItemSubgrupo.TipoSubgGrupo.SERVICO_ENTRADA;
                            else
                                if (id_natureza_TPNF == (int)Movimento.TipoMovimentoEnum.SAIDA)
                                tipoSubgrupo = (int)ItemSubgrupo.TipoSubgGrupo.SERVICO_SAIDA;
                            break;
                        }
                    case (int)Movimento.TipoMovimentoEnum.DEVOLUCAO:
                        {
                            if (id_natureza_TPNF == (int)Movimento.TipoMovimentoEnum.ENTRADA)
                                tipoSubgrupo = (int)ItemSubgrupo.TipoSubgGrupo.DEVOLUCAO_ENTRADA;
                            else
                                if (id_natureza_TPNF == (int)Movimento.TipoMovimentoEnum.SAIDA)
                                tipoSubgrupo = (int)ItemSubgrupo.TipoSubgGrupo.DEVOLUCAO_SAIDA;
                            break;
                        }
                }
                // Verificar permissão "Conta Segura".
                if (!contaSegura)
                    sql = from t in sql
                          where
                           (!t.ItemSubgrupos.Any() || db.PlanoConta.Where(pc => t.ItemSubgrupos.Where(isg => isg.cd_item == t.cd_item && pc.cd_subgrupo_conta == isg.cd_subgrupo_conta).Any() &&
                                                                       t.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).Any() && pc.cd_pessoa_empresa == cdEscola && !pc.id_conta_segura).Any())
                          select t;
                //regra para cadastro movimento
                if (tipoVinc == Movimento.TipoVinculoMovimento.PESQUISA_ITEM_CADASTRO_MOVIMENTO && (tipoItem == null || tipoItem == 0))
                    switch (id_tipo_movto)
                    {
                        case (int)Movimento.TipoMovimentoEnum.SAIDA:
                        case (int)Movimento.TipoMovimentoEnum.ENTRADA:
                            sql = from i in sql
                                  where i.TipoItem.cd_tipo_item != (int)TipoItem.TipoItemEnum.SERVICO &&
                                        i.TipoItem.cd_tipo_item != (int)TipoItem.TipoItemEnum.CUSTOSDESPESAS
                                  select i;
                            break;
                        case (int)Movimento.TipoMovimentoEnum.DESPESA:
                            sql = from i in sql
                                  where ((i.TipoItem.cd_tipo_item == (int)TipoItem.TipoItemEnum.SERVICO) ||
                                        (i.TipoItem.cd_tipo_item == (int)TipoItem.TipoItemEnum.CUSTOSDESPESAS))
                                  select i;
                            break;
                        case (int)Movimento.TipoMovimentoEnum.SERVICO:
                            sql = from i in sql
                                  where i.TipoItem.cd_tipo_item == (int)TipoItem.TipoItemEnum.SERVICO
                                  select i;
                            break;
                        case (int)Movimento.TipoMovimentoEnum.DEVOLUCAO:
                            if (cd_movimento != null && cd_movimento > 0)
                            {
                                sql = from i in sql
                                      where i.ItensMovimento.Where(x => x.cd_movimento == cd_movimento).Any()
                                      select i;
                            }
                            break;
                    }

                if (!kit)
                {
                    if (id_tipo_movto > 0)
                    {
                        switch (tipoVinc)
                        {
                            case Movimento.TipoVinculoMovimento.PESQUISA_ITEM_MOVIMENTO:
                                sql = from item in sql
                                      where item.ItensMovimento.Where(m => m.Movimento.id_tipo_movimento == id_tipo_movto && m.Movimento.cd_pessoa_empresa == cdEscola).Any()
                                      select item;
                                break;
                            case Movimento.TipoVinculoMovimento.HAS_PESQ_NOTA_MATERIAL:
                                SGFWebContext dbComp = new SGFWebContext();
                                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                                sql = from i in sql
                                      where i.ItensMovimento.Any(im => im.Movimento.id_origem_movimento == cd_origem)
                                      select i;
                                break;
                        }
                    }
                }

                if (comEstoque)
                    sql = from item in sql
                          where item.ItemEscola.Where(ie => ie.qt_estoque > 0 && ie.cd_pessoa_escola == cdEscola).Any()
                          select item;

                if (kit)
                {
                    sql = from c in sql
                          where (c.id_kit == true)
                          select c;
                }
                else
                {
                    sql = from c in sql
                          where (c.id_kit == false)
                          select c;
                }

                if (origem == (int)OrigemItemMovimentoPerdaMaterialEnum.ORIGEM_ITEM_SEARCH_PERDA_MATERIAL)
                {
                    sql = from c in sql
                          where  (from pm in db.PerdaMaterial 
                                  from im in db.ItemMovimento 
                                  where im.cd_item == c.cd_item && 
                                        im.Movimento.cd_pessoa_empresa == cdEscola &&
                                        pm.cd_movimento == im.cd_movimento && 
                                        im.Movimento.id_material_didatico == true
                                  select pm).Any()
                        select c;
                }

                if (ativo != null)
                    sql = from c in sql
                          where (c.id_item_ativo == ativo)
                          select c;

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql = from c in sql
                              where c.no_item.StartsWith(descricao)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_item.Contains(descricao)
                              select c;

                var retorno = from item in sql
                              select new ItemUI
                              {
                                  cd_grupo_estoque = item.cd_grupo_estoque,
                                  cd_item = item.cd_item,
                                  cd_tipo_item = item.cd_tipo_item,
                                  no_item = item.no_item,
                                  qt_estoque = item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).FirstOrDefault().qt_estoque,
                                  //vl_custo = item.ItemEscola.Where(i => i.cd_pessoa_escola == id_escola).FirstOrDefault().vl_custo,
                                  vl_item = item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).FirstOrDefault().vl_item,
                                  dc_tipo_item = item.TipoItem.dc_tipo_item,
                                  no_grupo_estoque = item.GrupoEstoque.no_grupo_estoque,
                                  id_item_ativo = item.id_item_ativo,
                                  cd_plano_conta = (from pc in db.PlanoConta
                                                    where item.ItemSubgrupos.Where(isg => isg.cd_item == item.cd_item && isg.id_tipo_movimento == tipoSubgrupo && pc.cd_subgrupo_conta == isg.cd_subgrupo_conta).Any() &&
                                                                                  item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).Any() &&
                                                                                  pc.cd_pessoa_empresa == cdEscola
                                                    select pc.cd_plano_conta).FirstOrDefault(),
                                  desc_plano_conta = db.PlanoConta.Where(pc => item.ItemSubgrupos.Where(isg => isg.cd_item == item.cd_item && isg.id_tipo_movimento == tipoSubgrupo && pc.cd_subgrupo_conta == isg.cd_subgrupo_conta).Any() &&
                                                                               item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).Any() && pc.cd_pessoa_empresa == cdEscola).Select(pl => pl.PlanoContaSubgrupo.no_subgrupo_conta).FirstOrDefault(),
                                  cd_curso = (item.Cursos.Any() ? item.Cursos.Select(x => x.cd_curso).FirstOrDefault() : 0),
                                  id_material_didatico = item.id_material_didatico,
                                  id_voucher_carga = item.id_voucher_carga
                              };

                retorno = sorter.Sort(retorno);

                int limite = retorno.Count();
                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public MovimentoUI getMovimentoReturnGrade(int cd_movimento, int cd_empresa)
        {
            try
            {
                int cd_origem = Int32.Parse(new SGFWebContext().LISTA_ORIGEM_LOGS["Movimento"].ToString());
                var sql = (from t in db.Movimento
                           where t.cd_movimento == cd_movimento && t.cd_pessoa_empresa == cd_empresa
                           select new MovimentoUI
                           {
                               cd_movimento = t.cd_movimento,
                               dc_politica_comercial = t.PoliticaComercial.dc_politica_comercial,
                               dc_tipo_financeiro = t.TipoFinanceiro.dc_tipo_financeiro,
                               no_pessoa = t.Pessoa.no_pessoa,
                               nm_movimento = t.nm_movimento,
                               dc_serie_movimento = t.dc_serie_movimento,
                               dt_emissao_movimento = t.dt_emissao_movimento,
                               dt_vcto_movimento = t.dt_vcto_movimento,
                               dt_mov_movimento = t.dt_mov_movimento,
                               id_tipo_movimento = t.id_tipo_movimento,
                               id_status_nf = t.id_status_nf,
                               id_nf = t.id_nf,
                               id_natureza_mvto_tp_nf = t.TipoNF.id_natureza_movimento,
                               qtd_total_geral = t.ItensMovimento.Any() ? t.ItensMovimento.Sum(x => x.vl_liquido_item) : 0,
                               datas_vencimento_titulo = db.Titulo.Where(i => i.cd_origem_titulo == t.cd_movimento && i.id_origem_titulo == cd_origem && i.cd_pessoa_empresa == cd_empresa).Select(x => x.dt_vcto_titulo)
                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        

        
        public List<int> VerificaNFESemDataAutorizacao(int id_tipo_movimento, int cd_empresa, bool nota_fiscal)
        {
            try
            {
                DateTime data = Convert.ToDateTime(System.Configuration.ConfigurationManager.AppSettings["dtsemautorizacao"]);
                int cd_origem = Int32.Parse(new SGFWebContext().LISTA_ORIGEM_LOGS["Movimento"].ToString());
                var sql = (from t in db.Movimento
                    orderby t.nm_movimento
                    where t.cd_pessoa_empresa == cd_empresa && t.id_tipo_movimento == id_tipo_movimento &&
                          t.id_nf == nota_fiscal && t.Empresa.id_empresa_propria
                          && t.dt_autorizacao_nfe == null && t.nm_movimento != null &&
                          t.id_status_nf != 3 &&
                          t.dt_emissao_movimento >= data
                           select (int)t.nm_movimento);

                return sql.Take(5).ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Movimento> getMovimentos(int[] cdMvotos, int cd_empresa)
        {
            try
            {
                var sql = from t in db.Movimento
                          where t.cd_pessoa_empresa == cd_empresa && cdMvotos.Contains(t.cd_movimento)
                          select t;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Movimento getMovimentoEditViewNF(int cd_movimento, int cd_empresa, int id_tipo_movimento)
        {
            int? cdEmpresaLig = (from empresa in db.PessoaSGF.OfType<Escola>() where empresa.cd_pessoa == cd_empresa select empresa.cd_empresa_coligada).FirstOrDefault();
            cd_empresa = (int)(cdEmpresaLig == null ? cd_empresa : cdEmpresaLig);
            try
            {
                Movimento movimento = new Movimento();
                switch (id_tipo_movimento)
                {
                    #region Serviço
                    case (int)Movimento.TipoMovimentoEnum.SERVICO:
                        movimento = (from t in db.Movimento
                                     where t.cd_pessoa_empresa == cd_empresa && t.cd_movimento == cd_movimento
                                     select new
                                     {
                                         cd_movimento = t.cd_movimento,
                                         cd_pessoa = t.cd_pessoa,
                                         cd_aluno = t.cd_aluno,
                                         cd_pessoa_aluno = t.cd_aluno != null ? t.Aluno.cd_pessoa_aluno : 0,
                                         no_aluno = t.Aluno.AlunoPessoaFisica.no_pessoa,
                                         cd_politica_comercial = t.cd_politica_comercial,
                                         cd_tipo_financeiro = t.cd_tipo_financeiro,
                                         nm_movimento = t.nm_movimento,
                                         dc_serie_movimento = t.dc_serie_movimento,
                                         dt_emissao_movimento = t.dt_emissao_movimento,
                                         dt_vcto_movimento = t.dt_vcto_movimento,
                                         dt_mov_movimento = t.dt_mov_movimento,
                                         pc_acrescimo = t.pc_acrescimo,
                                         vl_acrescimo = t.vl_acrescimo,
                                         pc_desconto = t.pc_desconto,
                                         vl_desconto = t.vl_desconto,
                                         tx_obs_movimento = t.tx_obs_movimento,
                                         id_nf = t.id_nf,
                                         id_nf_escola = t.id_nf_escola,
                                         cd_tipo_nota_fiscal = t.cd_tipo_nota_fiscal,
                                         cd_sit_trib_ICMS = t.TipoNF.cd_situacao_tributaria,
                                         id_status_nf = t.id_status_nf,
                                         vl_base_calculo_ISS_nf = t.vl_base_calculo_ISS_nf,
                                         dc_cfop_nf = t.dc_cfop_nf,
                                         nm_cfop = t.CFOP != null ? t.CFOP.nm_cfop : 0,
                                         vl_ISS_nf = t.vl_ISS_nf,
                                         no_pessoa = t.Pessoa.no_pessoa,
                                         dc_politica_comercial = t.PoliticaComercial.dc_politica_comercial,
                                         dc_tipo_nota = t.TipoNF.dc_tipo_nota_fiscal,
                                         pc_reduzido_nf = t.TipoNF.pc_reducao,
                                         id_tipo_movimento = t.id_tipo_movimento,
                                         cheque = t.Cheques.FirstOrDefault(),
                                         cd_cfop_nf = t.cd_cfop_nf,
                                         tx_obs_fiscal = t.tx_obs_fiscal,
                                         id_regime_tributario = t.TipoNF.id_regime_tributario,
                                         vl_aproximado = t.vl_aproximado,
                                         pc_aliquota_aproximada = t.pc_aliquota_aproximada,
                                         t.TipoNF.id_natureza_movimento,
                                         t.nm_nfe,
                                         t.ds_protocolo_nfe,
                                         t.dt_autorizacao_nfe,
                                         t.dc_mensagem_retorno,
                                         t.dc_url_nf,
                                         t.dt_nfe_cancel,
                                         t.dc_protocolo_cancel
                                     }).ToList().Select(x => new Movimento
                                     {
                                         cd_movimento = x.cd_movimento,
                                         cd_pessoa = x.cd_pessoa,
                                         cd_aluno = x.cd_aluno,
                                         no_aluno = x.no_aluno,
                                         cd_pessoa_aluno = x.cd_pessoa_aluno,
                                         cd_politica_comercial = x.cd_politica_comercial,
                                         cd_tipo_financeiro = x.cd_tipo_financeiro,
                                         cd_tipo_nota_fiscal = x.cd_tipo_nota_fiscal,
                                         cd_sit_trib_ICMS = x.cd_sit_trib_ICMS,
                                         nm_movimento = x.nm_movimento,
                                         dc_serie_movimento = x.dc_serie_movimento,
                                         dt_emissao_movimento = x.dt_emissao_movimento,
                                         dt_vcto_movimento = x.dt_vcto_movimento,
                                         dt_mov_movimento = x.dt_mov_movimento,
                                         pc_acrescimo = x.pc_acrescimo,
                                         vl_acrescimo = x.vl_acrescimo,
                                         pc_desconto = x.pc_desconto,
                                         vl_desconto = x.vl_desconto,
                                         tx_obs_movimento = x.tx_obs_movimento,
                                         id_nf_escola = x.id_nf_escola,
                                         id_nf = x.id_nf,
                                         id_status_nf = x.id_status_nf,
                                         vl_base_calculo_ISS_nf = x.vl_base_calculo_ISS_nf,
                                         dc_cfop_nf = x.dc_cfop_nf,
                                         nm_cfop = (short)x.nm_cfop,
                                         vl_ISS_nf = x.vl_ISS_nf,
                                         no_pessoa = x.no_pessoa,
                                         dc_politica_comercial = x.dc_politica_comercial,
                                         dc_tipo_nota = x.dc_tipo_nota,
                                         pc_reduzido_nf = x.pc_reduzido_nf,
                                         id_tipo_movimento = x.id_tipo_movimento,
                                         cheque = x.cheque,
                                         cd_cfop_nf = x.cd_cfop_nf,
                                         tx_obs_fiscal = x.tx_obs_fiscal,
                                         nm_nfe = x.nm_nfe,
                                         ds_protocolo_nfe = x.ds_protocolo_nfe,
                                         dt_autorizacao_nfe = x.dt_autorizacao_nfe,
                                         dc_mensagem_retorno = x.dc_mensagem_retorno,
                                         dc_url_nf = x.dc_url_nf,
                                         dt_nfe_cancel = x.dt_nfe_cancel,
                                         dc_protocolo_cancel = x.dc_protocolo_cancel,
                                         TipoNF = new TipoNotaFiscal
                                         {
                                             id_regime_tributario = x.id_regime_tributario,
                                             id_natureza_movimento = x.id_natureza_movimento
                                         },
                                         vl_aproximado = x.vl_aproximado,
                                         pc_aliquota_aproximada = x.pc_aliquota_aproximada
                                     }).FirstOrDefault();
                        if (movimento != null && movimento.cd_movimento > 0)
                            movimento.ItensMovimento = (from im in db.ItemMovimento
                                                        where im.cd_movimento == movimento.cd_movimento
                                                        select new
                                                        {
                                                            cd_item_movimento = im.cd_item_movimento,
                                                            cd_movimento = im.cd_movimento,
                                                            cd_item = im.cd_item,
                                                            dc_item_movimento = im.dc_item_movimento,
                                                            qt_item_movimento = im.qt_item_movimento,
                                                            vl_unitario_item = im.vl_unitario_item,
                                                            vl_total_item = im.vl_total_item,
                                                            vl_liquido_item = im.vl_liquido_item,
                                                            vl_acrescimo_item = im.vl_acrescimo_item,
                                                            vl_desconto_item = im.vl_desconto_item,
                                                            cd_plano_conta = im.cd_plano_conta,
                                                            no_plano_conta = im.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta,
                                                            planoSugerido = db.ItemSubgrupo.Where(pc => pc.cd_item == im.cd_item &&
                                                                (pc.id_tipo_movimento + 1) == movimento.id_tipo_movimento &&
                                                                 im.Item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cd_empresa).Any()).Any(),
                                                            pc_desconto = im.pc_desconto_item,
                                                            //Fiscal
                                                            vl_base_calculo_ISS_item = im.vl_base_calculo_ISS_item,
                                                            pc_aliquota_ISS = im.pc_aliquota_ISS,
                                                            vl_ISS_item = im.vl_ISS_item,
                                                            id_nf_item = im.Movimento.id_nf,
                                                            cd_cfop = im.cd_cfop,
                                                            dc_cfop = im.dc_cfop,
                                                            nm_cfop = im.CFOP != null ? im.CFOP.nm_cfop : 0,
                                                            vl_aproximado = im.vl_aproximado,
                                                            pc_aliquota_aproximada = im.pc_aliquota_aproximada,
                                                            id_voucher_carga = im.Item.id_voucher_carga
                                                            //no_item = im.Item.no_item
                                                        }).ToList().Select(x => new ItemMovimento
                                                        {
                                                            cd_item_movimento = x.cd_item_movimento,
                                                            cd_movimento = x.cd_movimento,
                                                            cd_item = x.cd_item,
                                                            dc_item_movimento = x.dc_item_movimento,
                                                            qt_item_movimento = x.qt_item_movimento,
                                                            vl_unitario_item = x.vl_unitario_item,
                                                            vl_total_item = x.vl_total_item,
                                                            vl_liquido_item = x.vl_liquido_item,
                                                            vl_acrescimo_item = x.vl_acrescimo_item,
                                                            vl_desconto_item = x.vl_desconto_item,
                                                            cd_plano_conta = x.cd_plano_conta,
                                                            dc_plano_conta = x.no_plano_conta,
                                                            planoSugerido = x.planoSugerido,
                                                            pc_desconto_item = x.pc_desconto,
                                                            vl_base_calculo_ISS_item = x.vl_base_calculo_ISS_item,
                                                            pc_aliquota_ISS = x.pc_aliquota_ISS,
                                                            vl_ISS_item = x.vl_ISS_item,
                                                            id_nf_item = x.id_nf_item,
                                                            cd_cfop = x.cd_cfop,
                                                            dc_cfop = x.dc_cfop,
                                                            nm_cfop = (short)x.nm_cfop,
                                                            vl_aproximado = x.vl_aproximado,
                                                            pc_aliquota_aproximada = x.pc_aliquota_aproximada,
                                                            id_voucher_carga = x.id_voucher_carga
                                                            //no_item = x.no_item
                                                        }).ToList();
                        
                        break;
                    #endregion
                    #region Entrasa/Saida
                    case (int)Movimento.TipoMovimentoEnum.ENTRADA:
                    case (int)Movimento.TipoMovimentoEnum.SAIDA:
                        movimento = (from t in db.Movimento
                                     where t.cd_pessoa_empresa == cd_empresa && t.cd_movimento == cd_movimento
                                     select new
                                     {
                                         cd_movimento = t.cd_movimento,
                                         cd_pessoa = t.cd_pessoa,
                                         cd_aluno = t.cd_aluno,
                                         cd_pessoa_aluno = t.cd_aluno != null ? t.Aluno.cd_pessoa_aluno : 0,
                                         no_aluno = t.Aluno.AlunoPessoaFisica.no_pessoa,
                                         cd_politica_comercial = t.cd_politica_comercial,
                                         cd_tipo_financeiro = t.cd_tipo_financeiro,
                                         nm_movimento = t.nm_movimento,
                                         dc_serie_movimento = t.dc_serie_movimento,
                                         dt_emissao_movimento = t.dt_emissao_movimento,
                                         dt_vcto_movimento = t.dt_vcto_movimento,
                                         dt_mov_movimento = t.dt_mov_movimento,
                                         pc_acrescimo = t.pc_acrescimo,
                                         vl_acrescimo = t.vl_acrescimo,
                                         pc_desconto = t.pc_desconto,
                                         vl_desconto = t.vl_desconto,
                                         tx_obs_movimento = t.tx_obs_movimento,
                                         id_nf = t.id_nf,
                                         id_nf_escola = t.id_nf_escola,
                                         cd_tipo_nota_fiscal = t.cd_tipo_nota_fiscal,
                                         id_status_nf = t.id_status_nf,
                                         cd_cfop_nf = t.cd_cfop_nf,
                                         dc_cfop_nf = t.dc_cfop_nf,
                                         nm_cfop = t.CFOP != null ? t.CFOP.nm_cfop : 0,
                                         vl_base_calculo_ICMS_nf = t.vl_base_calculo_ICMS_nf,
                                         vl_ICMS_nf = t.vl_ICMS_nf,
                                         vl_base_calculo_PIS_nf = t.vl_base_calculo_PIS_nf,
                                         vl_PIS_nf = t.vl_PIS_nf,
                                         vl_base_calculo_COFINS_nf = t.vl_base_calculo_COFINS_nf,
                                         vl_COFINS_nf = t.vl_COFINS_nf,
                                         vl_base_calculo_IPI_nf = t.vl_base_calculo_IPI_nf,
                                         vl_IPI_nf = t.vl_IPI_nf,
                                         no_pessoa = t.Pessoa.no_pessoa,
                                         dc_politica_comercial = t.PoliticaComercial.dc_politica_comercial,
                                         dc_tipo_nota = t.TipoNF.dc_tipo_nota_fiscal,
                                         pc_reduzido_nf = t.TipoNF.pc_reducao,
                                         id_tipo_movimento = t.id_tipo_movimento,
                                         cheque = t.Cheques.FirstOrDefault(),
                                         tx_obs_fiscal = t.tx_obs_fiscal,
                                         cd_sit_trib_ICMS = t.TipoNF.cd_situacao_tributaria,
                                         id_regime_tributario = t.TipoNF.id_regime_tributario,
                                         vl_aproximado = t.vl_aproximado,
                                         pc_aliquota_aproximada = t.pc_aliquota_aproximada,
                                         t.ds_protocolo_nfe,
                                         t.dt_autorizacao_nfe,
                                         t.dc_mensagem_retorno,
                                         t.dc_key_nfe,
                                         t.dt_nfe_cancel,
                                         t.dc_protocolo_cancel,
                                         id_material_didatico = t.id_material_didatico,
                                         dc_meio_pagamento = t.dc_meio_pagamento,
                                         cd_origem_movimento = t.cd_origem_movimento,
                                         nm_contrato = (t.id_material_didatico == true && t.cd_origem_movimento != null ? (from cc in db.Contrato where  cc.cd_contrato == t.cd_origem_movimento select cc.nm_contrato).FirstOrDefault() : null),
                                         nm_matricula_contrato = (t.id_material_didatico == true && t.cd_origem_movimento != null ? (from cc in db.Contrato where cc.cd_contrato == t.cd_origem_movimento select cc.nm_matricula_contrato).FirstOrDefault() : null),
                                         cd_curso = (t.id_material_didatico == true) ? t.cd_curso : null,
                                         no_curso = (t.id_material_didatico == true && t.cd_curso != null ? (from c in db.Curso where t.cd_curso == c.cd_curso select c.no_curso).FirstOrDefault() : null),
                                         id_venda_futura = t.id_venda_futura
                }).ToList().Select(x => new Movimento
                                     {
                                         cd_movimento = x.cd_movimento,
                                         cd_pessoa = x.cd_pessoa,
                                         cd_aluno = x.cd_aluno,
                                         no_aluno = x.no_aluno,
                                         cd_pessoa_aluno = x.cd_pessoa_aluno,
                                         cd_politica_comercial = x.cd_politica_comercial,
                                         cd_tipo_financeiro = x.cd_tipo_financeiro,
                                         cd_tipo_nota_fiscal = x.cd_tipo_nota_fiscal,
                                         nm_movimento = x.nm_movimento,
                                         dc_serie_movimento = x.dc_serie_movimento,
                                         dt_emissao_movimento = x.dt_emissao_movimento,
                                         dt_vcto_movimento = x.dt_vcto_movimento,
                                         dt_mov_movimento = x.dt_mov_movimento,
                                         pc_acrescimo = x.pc_acrescimo,
                                         vl_acrescimo = x.vl_acrescimo,
                                         pc_desconto = x.pc_desconto,
                                         vl_desconto = x.vl_desconto,
                                         tx_obs_movimento = x.tx_obs_movimento,
                                         id_nf = x.id_nf,
                                         id_nf_escola = x.id_nf_escola,
                                         id_status_nf = x.id_status_nf,
                                         cd_cfop_nf = x.cd_cfop_nf,
                                         dc_cfop_nf = x.dc_cfop_nf,
                                         nm_cfop = (short)x.nm_cfop,
                                         vl_base_calculo_ICMS_nf = x.vl_base_calculo_ICMS_nf,
                                         vl_ICMS_nf = x.vl_ICMS_nf,
                                         vl_base_calculo_PIS_nf = x.vl_base_calculo_PIS_nf,
                                         vl_PIS_nf = x.vl_PIS_nf,
                                         vl_base_calculo_COFINS_nf = x.vl_base_calculo_COFINS_nf,
                                         vl_COFINS_nf = x.vl_COFINS_nf,
                                         vl_base_calculo_IPI_nf = x.vl_base_calculo_IPI_nf,
                                         vl_IPI_nf = x.vl_IPI_nf,
                                         no_pessoa = x.no_pessoa,
                                         dc_politica_comercial = x.dc_politica_comercial,
                                         dc_tipo_nota = x.dc_tipo_nota,
                                         pc_reduzido_nf = x.pc_reduzido_nf,
                                         id_tipo_movimento = x.id_tipo_movimento,
                                         cheque = x.cheque,
                                         tx_obs_fiscal = x.tx_obs_fiscal,
                                         cd_sit_trib_ICMS = x.cd_sit_trib_ICMS,
                                         TipoNF = new TipoNotaFiscal
                                         {
                                             id_regime_tributario = x.id_regime_tributario,
                                         },
                                         vl_aproximado = x.vl_aproximado,
                                         pc_aliquota_aproximada = x.pc_aliquota_aproximada,
                                         ds_protocolo_nfe = x.ds_protocolo_nfe,
                                         dt_autorizacao_nfe = x.dt_autorizacao_nfe,
                                         dc_mensagem_retorno = x.dc_mensagem_retorno,
                                         dc_key_nfe = x.dc_key_nfe,
                                         dt_nfe_cancel = x.dt_nfe_cancel,
                                         dc_protocolo_cancel = x.dc_protocolo_cancel,
                                         dc_meio_pagamento = x.dc_meio_pagamento,
                                         id_material_didatico = x.id_material_didatico,
                                         cd_origem_movimento = x.cd_origem_movimento,
                                         cd_curso = x.cd_curso,
                                         no_curso = x.no_curso,
                                         nm_contrato = x.nm_contrato,
                                         nm_matricula_contrato = x.nm_matricula_contrato,
                                         id_venda_futura = x.id_venda_futura
                }).FirstOrDefault();
                        if (movimento != null && movimento.cd_movimento > 0)
                            movimento.ItensMovimento = (from im in db.ItemMovimento
                                                        where im.cd_movimento == movimento.cd_movimento
                                                        select new
                                                        {
                                                            cd_item_movimento = im.cd_item_movimento,
                                                            cd_movimento = im.cd_movimento,
                                                            cd_item = im.cd_item,
                                                            cd_item_kit = im.ItemMovItemKit.Where(imov => imov.cd_item_movimento == im.cd_item_movimento).FirstOrDefault() != null ? im.ItemMovItemKit.Where(imov => imov.cd_item_movimento == im.cd_item_movimento).FirstOrDefault().cd_item_kit : 0,
                                                            dc_item_movimento = im.dc_item_movimento,
                                                            qt_item_movimento = im.qt_item_movimento,
                                                            vl_unitario_item = im.vl_unitario_item,
                                                            vl_total_item = im.vl_total_item,
                                                            vl_liquido_item = im.vl_liquido_item,
                                                            cd_grupo_estoque = im.Item.cd_grupo_estoque,
                                                            vl_acrescimo_item = im.vl_acrescimo_item,
                                                            vl_desconto_item = im.vl_desconto_item,
                                                            cd_plano_conta = im.cd_plano_conta,
                                                            no_plano_conta = im.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta,
                                                            planoSugerido = db.ItemSubgrupo.Where(pc => pc.cd_item == im.cd_item &&
                                                                (pc.id_tipo_movimento + 1) == movimento.id_tipo_movimento &&
                                                                 im.Item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cd_empresa).Any()).Any(),
                                                            pc_desconto = im.pc_desconto_item,
                                                            //Fiscal
                                                            cd_situacao_tributaria_ICMS = im.cd_situacao_tributaria_ICMS,
                                                            cd_situacao_tributaria_PIS = im.cd_situacao_tributaria_PIS,
                                                            cd_situacao_tributaria_COFINS = im.cd_situacao_tributaria_COFINS,
                                                            vl_base_calculo_ICMS_item = im.vl_base_calculo_ICMS_item,
                                                            pc_aliquota_ICMS = im.pc_aliquota_ICMS,
                                                            vl_ICMS_item = im.vl_ICMS_item,
                                                            vl_base_calculo_PIS_item = im.vl_base_calculo_PIS_item,
                                                            pc_aliquota_PIS = im.pc_aliquota_PIS,
                                                            vl_PIS_item = im.vl_PIS_item,
                                                            vl_base_calculo_COFINS_item = im.vl_base_calculo_COFINS_item,
                                                            pc_aliquota_COFINS = im.pc_aliquota_COFINS,
                                                            vl_COFINS_item = im.vl_COFINS_item,
                                                            vl_base_calculo_IPI_item = im.vl_base_calculo_IPI_item,
                                                            pc_aliquota_IPI = im.pc_aliquota_IPI,
                                                            vl_IPI_item = im.vl_IPI_item,
                                                            cd_cfop_item = im.cd_cfop,
                                                            dc_cfop_item = im.dc_cfop,
                                                            nm_cfop = im.CFOP != null ? im.CFOP.nm_cfop : 0,
                                                            id_nf_item = im.Movimento.id_nf,
                                                            vl_aproximado = im.vl_aproximado,
                                                            pc_aliquota_aproximada = im.pc_aliquota_aproximada,
                                                            cd_tipo_item = im.Item.cd_tipo_item,
                                                            id_material_didatico = im.Item.id_material_didatico,
                                                            id_voucher_carga = im.Item.id_voucher_carga
                                                        }).ToList().Select(x => new ItemMovimento
                                                        {
                                                            cd_item_movimento = x.cd_item_movimento,
                                                            cd_movimento = x.cd_movimento,
                                                            cd_item = x.cd_item,
                                                            cd_item_kit = x.cd_item_kit,
                                                            dc_item_movimento = x.dc_item_movimento,
                                                            qt_item_movimento = x.qt_item_movimento,
                                                            vl_unitario_item = x.vl_unitario_item,
                                                            vl_total_item = x.vl_total_item,
                                                            vl_liquido_item = x.vl_liquido_item,
                                                            cd_grupo_estoque = x.cd_grupo_estoque,
                                                            vl_acrescimo_item = x.vl_acrescimo_item,
                                                            vl_desconto_item = x.vl_desconto_item,
                                                            cd_plano_conta = x.cd_plano_conta,
                                                            dc_plano_conta = x.no_plano_conta,
                                                            planoSugerido = x.planoSugerido,
                                                            pc_desconto_item = x.pc_desconto,
                                                            cd_situacao_tributaria_ICMS = x.cd_situacao_tributaria_ICMS,
                                                            cd_situacao_tributaria_PIS = x.cd_situacao_tributaria_PIS,
                                                            cd_situacao_tributaria_COFINS = x.cd_situacao_tributaria_COFINS,
                                                            vl_base_calculo_ICMS_item = x.vl_base_calculo_ICMS_item,
                                                            pc_aliquota_ICMS = x.pc_aliquota_ICMS,
                                                            vl_ICMS_item = x.vl_ICMS_item,
                                                            vl_base_calculo_PIS_item = x.vl_base_calculo_PIS_item,
                                                            pc_aliquota_PIS = x.pc_aliquota_PIS,
                                                            vl_PIS_item = x.vl_PIS_item,
                                                            vl_base_calculo_COFINS_item = x.vl_base_calculo_COFINS_item,
                                                            pc_aliquota_COFINS = x.pc_aliquota_COFINS,
                                                            vl_COFINS_item = x.vl_COFINS_item,
                                                            vl_base_calculo_IPI_item = x.vl_base_calculo_IPI_item,
                                                            pc_aliquota_IPI = x.pc_aliquota_IPI,
                                                            vl_IPI_item = x.vl_IPI_item,
                                                            cd_cfop = x.cd_cfop_item,
                                                            dc_cfop = x.dc_cfop_item,
                                                            id_nf_item = x.id_nf_item,
                                                            nm_cfop = (short)x.nm_cfop,
                                                            vl_aproximado = x.vl_aproximado,
                                                            pc_aliquota_aproximada = x.pc_aliquota_aproximada,
                                                            cd_tipo_item = x.cd_tipo_item,
                                                            id_material_didatico = x.id_material_didatico,
                                                            id_voucher_carga = x.id_voucher_carga
                                                            //no_item = x.no_item
                                                        }).ToList();
                        if (movimento != null && movimento.cd_movimento > 0)
                            movimento.ItemMovimentoKit = (from imk in db.ItemMovimentoKit
                                                        where imk.cd_movimento == movimento.cd_movimento
                                                        select new
                                                        {
                                                            cd_item_movimento_kit = imk.cd_item_movimento_kit,
                                                            cd_movimento = imk.cd_movimento,
                                                            cd_item_kit = imk.cd_item_kit,
                                                            qt_item_kit = imk.qt_item_kit,
                                                            no_item_kit = imk.Item.no_item,
                                                            cd_grupo_estoque = imk.Item.cd_grupo_estoque

                                                        }).ToList().Select(x => new ItemMovimentoKit
                                                        {
                                                            cd_item_movimento_kit = x.cd_item_movimento_kit,
                                                            cd_movimento = x.cd_movimento,
                                                            cd_item_kit = x.cd_item_kit,
                                                            qt_item_kit = x.qt_item_kit,
                                                            no_item_kit = x.no_item_kit,
                                                            cd_grupo_estoque = x.cd_grupo_estoque
                                                        }).ToList();
                        break;
                    #endregion
                    #region Devolução
                    case (int)Movimento.TipoMovimentoEnum.DEVOLUCAO:
                        movimento = (from t in db.Movimento
                                     where t.cd_pessoa_empresa == cd_empresa && t.cd_movimento == cd_movimento
                                     select new
                                     {
                                         cd_movimento = t.cd_movimento,
                                         cd_pessoa = t.cd_pessoa,
                                         cd_politica_comercial = t.cd_politica_comercial,
                                         cd_tipo_financeiro = t.cd_tipo_financeiro,
                                         nm_movimento = t.nm_movimento,
                                         dc_serie_movimento = t.dc_serie_movimento,
                                         dt_emissao_movimento = t.dt_emissao_movimento,
                                         dt_vcto_movimento = t.dt_vcto_movimento,
                                         dt_mov_movimento = t.dt_mov_movimento,
                                         pc_acrescimo = t.pc_acrescimo,
                                         vl_acrescimo = t.vl_acrescimo,
                                         pc_desconto = t.pc_desconto,
                                         vl_desconto = t.vl_desconto,
                                         tx_obs_movimento = t.tx_obs_movimento,
                                         id_nf = t.id_nf,
                                         id_nf_escola = t.id_nf_escola,
                                         cd_tipo_nota_fiscal = t.cd_tipo_nota_fiscal,
                                         id_status_nf = t.id_status_nf,
                                         cd_cfop_nf = t.cd_cfop_nf,
                                         dc_cfop_nf = t.dc_cfop_nf,
                                         nm_cfop = t.CFOP != null ? t.CFOP.nm_cfop : 0,
                                         vl_base_calculo_ICMS_nf = t.vl_base_calculo_ICMS_nf,
                                         vl_ICMS_nf = t.vl_ICMS_nf,
                                         vl_base_calculo_PIS_nf = t.vl_base_calculo_PIS_nf,
                                         vl_PIS_nf = t.vl_PIS_nf,
                                         vl_base_calculo_COFINS_nf = t.vl_base_calculo_COFINS_nf,
                                         vl_COFINS_nf = t.vl_COFINS_nf,
                                         vl_base_calculo_IPI_nf = t.vl_base_calculo_IPI_nf,
                                         vl_IPI_nf = t.vl_IPI_nf,
                                         no_pessoa = t.Pessoa.no_pessoa,
                                         dc_politica_comercial = t.PoliticaComercial.dc_politica_comercial,
                                         dc_tipo_nota = t.TipoNF.dc_tipo_nota_fiscal,
                                         pc_reduzido_nf = t.TipoNF == null ? 0 : t.TipoNF.pc_reducao,
                                         id_tipo_movimento = t.id_tipo_movimento,
                                         cheque = t.Cheques.FirstOrDefault(),
                                         tx_obs_fiscal = t.tx_obs_fiscal,
                                         cd_sit_trib_ICMS = t.TipoNF.cd_situacao_tributaria,
                                         id_regime_tributario = t.TipoNF == null ? 0 : t.TipoNF.id_regime_tributario,
                                         vl_aproximado = t.vl_aproximado,
                                         pc_aliquota_aproximada = t.pc_aliquota_aproximada,
                                         cd_nota_fiscal = t.cd_nota_fiscal,
                                         tipo_nf_devolucao = t.MovimentoDevolucao != null ? t.MovimentoDevolucao.id_tipo_movimento : 0,
                                         nm_NF_devolver = t.MovimentoDevolucao.nm_movimento,
                                         serie_NF_devolver = t.MovimentoDevolucao.dc_serie_movimento,
                                         id_natureza_movimento = t.MovimentoDevolucao != null ? 3-t.MovimentoDevolucao.id_tipo_movimento : t.TipoNF == null ? 0 : t.TipoNF.id_natureza_movimento,
                                         t.ds_protocolo_nfe,
                                         t.dt_autorizacao_nfe,
                                         t.dc_mensagem_retorno,
                                         t.dc_key_nfe,
                                         t.dt_nfe_cancel,
                                         t.dc_protocolo_cancel
                                     }).ToList().Select(x => new Movimento
                                     {
                                         cd_movimento = x.cd_movimento,
                                         cd_pessoa = x.cd_pessoa,
                                         cd_politica_comercial = x.cd_politica_comercial,
                                         cd_tipo_financeiro = x.cd_tipo_financeiro,
                                         cd_tipo_nota_fiscal = x.cd_tipo_nota_fiscal,
                                         nm_movimento = x.nm_movimento,
                                         dc_serie_movimento = x.dc_serie_movimento,
                                         dt_emissao_movimento = x.dt_emissao_movimento,
                                         dt_vcto_movimento = x.dt_vcto_movimento,
                                         dt_mov_movimento = x.dt_mov_movimento,
                                         pc_acrescimo = x.pc_acrescimo,
                                         vl_acrescimo = x.vl_acrescimo,
                                         pc_desconto = x.pc_desconto,
                                         vl_desconto = x.vl_desconto,
                                         tx_obs_movimento = x.tx_obs_movimento,
                                         id_nf = x.id_nf,
                                         id_nf_escola = x.id_nf_escola,
                                         id_status_nf = x.id_status_nf,
                                         cd_cfop_nf = x.cd_cfop_nf,
                                         dc_cfop_nf = x.dc_cfop_nf,
                                         nm_cfop = (short)x.nm_cfop,
                                         vl_base_calculo_ICMS_nf = x.vl_base_calculo_ICMS_nf,
                                         vl_ICMS_nf = x.vl_ICMS_nf,
                                         vl_base_calculo_PIS_nf = x.vl_base_calculo_PIS_nf,
                                         vl_PIS_nf = x.vl_PIS_nf,
                                         vl_base_calculo_COFINS_nf = x.vl_base_calculo_COFINS_nf,
                                         vl_COFINS_nf = x.vl_COFINS_nf,
                                         vl_base_calculo_IPI_nf = x.vl_base_calculo_IPI_nf,
                                         vl_IPI_nf = x.vl_IPI_nf,
                                         no_pessoa = x.no_pessoa,
                                         dc_politica_comercial = x.dc_politica_comercial,
                                         dc_tipo_nota = x.dc_tipo_nota,
                                         pc_reduzido_nf = x.pc_reduzido_nf,
                                         id_tipo_movimento = x.id_tipo_movimento,
                                         cheque = x.cheque,
                                         tx_obs_fiscal = x.tx_obs_fiscal,
                                         cd_sit_trib_ICMS = x.cd_sit_trib_ICMS,
                                         TipoNF = new TipoNotaFiscal
                                         {
                                             id_regime_tributario = (byte)x.id_regime_tributario,
                                             id_natureza_movimento = (byte)x.id_natureza_movimento
                                         },
                                         cd_nota_fiscal = x.cd_nota_fiscal,
                                         MovimentoDevolucao = new Movimento
                                         {
                                             nm_movimento = x.nm_NF_devolver,
                                             dc_serie_movimento = x.serie_NF_devolver,
                                             id_tipo_movimento = (byte)x.tipo_nf_devolucao
                                         },
                                         vl_aproximado = x.vl_aproximado,
                                         pc_aliquota_aproximada = x.pc_aliquota_aproximada,
                                         ds_protocolo_nfe = x.ds_protocolo_nfe,
                                         dt_autorizacao_nfe = x.dt_autorizacao_nfe,
                                         dc_mensagem_retorno = x.dc_mensagem_retorno,
                                         dc_key_nfe = x.dc_key_nfe,
                                         dt_nfe_cancel = x.dt_nfe_cancel,
                                         dc_protocolo_cancel = x.dc_protocolo_cancel
                                     }).FirstOrDefault();
                        if (movimento != null && movimento.cd_movimento > 0)
                            movimento.ItensMovimento = (from im in db.ItemMovimento
                                                        where im.cd_movimento == movimento.cd_movimento
                                                        select new
                                                        {
                                                            cd_item_movimento = im.cd_item_movimento,
                                                            cd_movimento = im.cd_movimento,
                                                            cd_item = im.cd_item,
                                                            dc_item_movimento = im.dc_item_movimento,
                                                            qt_item_movimento = im.qt_item_movimento,
                                                            vl_unitario_item = im.vl_unitario_item,
                                                            vl_total_item = im.vl_total_item,
                                                            vl_liquido_item = im.vl_liquido_item,
                                                            vl_acrescimo_item = im.vl_acrescimo_item,
                                                            vl_desconto_item = im.vl_desconto_item,
                                                            cd_plano_conta = im.cd_plano_conta,
                                                            no_plano_conta = im.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta,
                                                            planoSugerido = db.ItemSubgrupo.Where(pc => pc.cd_item == im.cd_item &&
                                                                (pc.id_tipo_movimento + 1) == movimento.id_tipo_movimento &&
                                                                 im.Item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cd_empresa).Any()).Any(),
                                                            pc_desconto = im.pc_desconto_item,
                                                            //Fiscal
                                                            cd_situacao_tributaria_ICMS = im.cd_situacao_tributaria_ICMS,
                                                            cd_situacao_tributaria_PIS = im.cd_situacao_tributaria_PIS,
                                                            cd_situacao_tributaria_COFINS = im.cd_situacao_tributaria_COFINS,
                                                            vl_base_calculo_ICMS_item = im.vl_base_calculo_ICMS_item,
                                                            pc_aliquota_ICMS = im.pc_aliquota_ICMS,
                                                            vl_ICMS_item = im.vl_ICMS_item,
                                                            vl_base_calculo_PIS_item = im.vl_base_calculo_PIS_item,
                                                            pc_aliquota_PIS = im.pc_aliquota_PIS,
                                                            vl_PIS_item = im.vl_PIS_item,
                                                            vl_base_calculo_COFINS_item = im.vl_base_calculo_COFINS_item,
                                                            pc_aliquota_COFINS = im.pc_aliquota_COFINS,
                                                            vl_COFINS_item = im.vl_COFINS_item,
                                                            vl_base_calculo_IPI_item = im.vl_base_calculo_IPI_item,
                                                            pc_aliquota_IPI = im.pc_aliquota_IPI,
                                                            vl_IPI_item = im.vl_IPI_item,
                                                            cd_cfop_item = im.cd_cfop,
                                                            dc_cfop_item = im.dc_cfop,
                                                            nm_cfop = im.CFOP != null ? im.CFOP.nm_cfop : 0,
                                                            id_nf_item = im.Movimento.id_nf,
                                                            vl_aproximado = im.vl_aproximado,
                                                            pc_aliquota_aproximada = im.pc_aliquota_aproximada,
                                                            cd_tipo_item = im.Item.cd_tipo_item,
                                                            id_material_didatico = im.Item.id_material_didatico
                                                        }).ToList().Select(x => new ItemMovimento
                                                        {
                                                            cd_item_movimento = x.cd_item_movimento,
                                                            cd_movimento = x.cd_movimento,
                                                            cd_item = x.cd_item,
                                                            dc_item_movimento = x.dc_item_movimento,
                                                            qt_item_movimento = x.qt_item_movimento,
                                                            qt_item_movimento_dev = x.qt_item_movimento,
                                                            vl_unitario_item = x.vl_unitario_item,
                                                            vl_total_item = x.vl_total_item,
                                                            vl_liquido_item = x.vl_liquido_item,
                                                            vl_acrescimo_item = x.vl_acrescimo_item,
                                                            vl_desconto_item = x.vl_desconto_item,
                                                            cd_plano_conta = x.cd_plano_conta,
                                                            dc_plano_conta = x.no_plano_conta,
                                                            planoSugerido = x.planoSugerido,
                                                            pc_desconto_item = x.pc_desconto,
                                                            cd_situacao_tributaria_ICMS = x.cd_situacao_tributaria_ICMS,
                                                            cd_situacao_tributaria_PIS = x.cd_situacao_tributaria_PIS,
                                                            cd_situacao_tributaria_COFINS = x.cd_situacao_tributaria_COFINS,
                                                            vl_base_calculo_ICMS_item = x.vl_base_calculo_ICMS_item,
                                                            pc_aliquota_ICMS = x.pc_aliquota_ICMS,
                                                            vl_ICMS_item = x.vl_ICMS_item,
                                                            vl_base_calculo_PIS_item = x.vl_base_calculo_PIS_item,
                                                            pc_aliquota_PIS = x.pc_aliquota_PIS,
                                                            vl_PIS_item = x.vl_PIS_item,
                                                            vl_base_calculo_COFINS_item = x.vl_base_calculo_COFINS_item,
                                                            pc_aliquota_COFINS = x.pc_aliquota_COFINS,
                                                            vl_COFINS_item = x.vl_COFINS_item,
                                                            vl_base_calculo_IPI_item = x.vl_base_calculo_IPI_item,
                                                            pc_aliquota_IPI = x.pc_aliquota_IPI,
                                                            vl_IPI_item = x.vl_IPI_item,
                                                            cd_cfop = x.cd_cfop_item,
                                                            dc_cfop = x.dc_cfop_item,
                                                            id_nf_item = x.id_nf_item,
                                                            nm_cfop = (short)x.nm_cfop,
                                                            vl_aproximado = x.vl_aproximado,
                                                            pc_aliquota_aproximada = x.pc_aliquota_aproximada,
                                                            cd_tipo_item = x.cd_tipo_item,
                                                            id_material_didatico = x.id_material_didatico
                                                            //no_item = x.no_item
                                                        }).ToList();
                        break;
                    #endregion
                }

                return movimento;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public IEnumerable<ContratoComboUI> getContratosSemTurmaByAlunoMovimentoSearch(int cd_aluno,
            bool semTurma, int situacaoTurma, int nmContrato, int tipo, int cdEscola, byte tipoC, bool? status)
        {
            try
            {

                IQueryable<vi_contrato> sql;
                sql = from contrato in db.vi_contrato.AsNoTracking()
                      where contrato.cd_pessoa_escola == cdEscola
                      orderby contrato.no_pessoa ascending
                      select contrato;


                if (cd_aluno > 0)
                {
                    sql = from c in sql
                          where c.cd_aluno == cd_aluno
                          select c;
                }


                if (tipoC != 4)
                {
                    sql = from c in sql
                          where c.id_tipo_contrato == tipoC
                          select c;
                }




                if (semTurma)
                    sql = from c in sql
                          where c.nm_turma == 0
                          select c;

                if (situacaoTurma > 0 && !semTurma)
                    sql = from c in sql
                          where c.id_situacao_turma == situacaoTurma
                          select c;

                if (tipo > 0)
                    sql = from c in sql
                          where c.id_tipo_matricula == tipo
                          select c;

                if (nmContrato > 0)
                    sql = from c in sql
                          where c.nm_contrato == nmContrato
                          select c;


                if (status != null)
                    sql = from c in sql
                          where db.Aluno.Any(a => a.cd_aluno == c.cd_aluno && a.id_aluno_ativo == status)
                          select c;

                int limite = sql.Select(x => x.cd_contrato).ToList().Count();

                var retorno = (from x in sql
                               select new ContratoComboUI
                               {
                                   cd_contrato = x.cd_contrato,
                                   nm_contrato = x.nm_contrato,
                                   nm_matricula_contrato = x.nm_matricula_contrato,

                               }).ToList();



                //foreach (var d in convertContrato)
                //    d.DescontoContrato = DataAccessAditamento.getDescontosAplicadosAditamento(d.cd_contrato, cdEscola).ToList();

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }



        public Movimento getMovimentoEditView(int cd_movimento, int cd_empresa)
        {
            int? cdEmpresaLig = (from empresa in db.PessoaSGF.OfType<Escola>() where empresa.cd_pessoa == cd_empresa select empresa.cd_empresa_coligada).FirstOrDefault();
            cd_empresa = (int)(cdEmpresaLig == null ? cd_empresa : cdEmpresaLig);
            try
            {
                Movimento movimento = (from t in db.Movimento
                                       where t.cd_pessoa_empresa == cd_empresa && t.cd_movimento == cd_movimento
                                       select new
                                       {
                                           cd_movimento = t.cd_movimento,
                                           cd_pessoa = t.cd_pessoa,
                                           cd_aluno = t.cd_aluno,
                                           cd_pessoa_aluno = t.cd_aluno != null ? t.Aluno.cd_pessoa_aluno : 0,
                                           no_aluno = t.Aluno.AlunoPessoaFisica.no_pessoa,
                                           cd_politica_comercial = t.cd_politica_comercial,
                                           cd_tipo_financeiro = t.cd_tipo_financeiro,
                                           nm_movimento = t.nm_movimento,
                                           dc_serie_movimento = t.dc_serie_movimento,
                                           dt_emissao_movimento = t.dt_emissao_movimento,
                                           dt_vcto_movimento = t.dt_vcto_movimento,
                                           dt_mov_movimento = t.dt_mov_movimento,
                                           pc_acrescimo = t.pc_acrescimo,
                                           vl_acrescimo = t.vl_acrescimo,
                                           pc_desconto = t.pc_desconto,
                                           vl_desconto = t.vl_desconto,
                                           tx_obs_movimento = t.tx_obs_movimento,
                                           no_pessoa = t.Pessoa.no_pessoa,
                                           dc_politica_comercial = t.PoliticaComercial.dc_politica_comercial,
                                           id_tipo_movimento = t.id_tipo_movimento,
                                           cheque = t.Cheques.FirstOrDefault(),
                                           cd_origem_movimento = t.cd_origem_movimento,
                                           nm_contrato = (t.id_material_didatico == true && t.cd_origem_movimento != null ? (from cc in db.Contrato where cc.cd_contrato == t.cd_origem_movimento select cc.nm_contrato).FirstOrDefault() : null),
                                           nm_matricula_contrato = (t.id_material_didatico == true && t.cd_origem_movimento != null ? (from cc in db.Contrato where cc.cd_contrato == t.cd_origem_movimento select cc.nm_matricula_contrato).FirstOrDefault() : null),
                                           cd_curso = (t.id_material_didatico == true) ? t.cd_curso : null,
                                           no_curso = (t.id_material_didatico == true && t.cd_curso != null ? (from c in db.Curso where t.cd_curso == c.cd_curso select c.no_curso).FirstOrDefault() : null),
                                           id_venda_futura = t.id_venda_futura
                                       }).ToList().Select(x => new Movimento
                                       {
                                           cd_movimento = x.cd_movimento,
                                           cd_pessoa = x.cd_pessoa,
                                           cd_aluno = x.cd_aluno,
                                           no_aluno = x.no_aluno,
                                           cd_pessoa_aluno = x.cd_pessoa_aluno,
                                           cd_politica_comercial = x.cd_politica_comercial,
                                           cd_tipo_financeiro = x.cd_tipo_financeiro,
                                           nm_movimento = x.nm_movimento,
                                           dc_serie_movimento = x.dc_serie_movimento,
                                           dt_emissao_movimento = x.dt_emissao_movimento,
                                           dt_vcto_movimento = x.dt_vcto_movimento,
                                           dt_mov_movimento = x.dt_mov_movimento,
                                           pc_acrescimo = x.pc_acrescimo,
                                           vl_acrescimo = x.vl_acrescimo,
                                           pc_desconto = x.pc_desconto,
                                           vl_desconto = x.vl_desconto,
                                           tx_obs_movimento = x.tx_obs_movimento,
                                           no_pessoa = x.no_pessoa,
                                           dc_politica_comercial = x.dc_politica_comercial,
                                           id_tipo_movimento = x.id_tipo_movimento,
                                           cheque = x.cheque,
                                           cd_origem_movimento = x.cd_origem_movimento,
                                           cd_curso = x.cd_curso,
                                           no_curso = x.no_curso,
                                           nm_contrato = x.nm_contrato,
                                           nm_matricula_contrato = x.nm_matricula_contrato,
                                           id_venda_futura = x.id_venda_futura
                                       }).FirstOrDefault();
                if (movimento != null && movimento.cd_movimento > 0)
                {
                    movimento.ItensMovimento = (from im in db.ItemMovimento
                                                where im.cd_movimento == movimento.cd_movimento
                                                select new
                                                {
                                                    cd_item_movimento = im.cd_item_movimento,
                                                    cd_movimento = im.cd_movimento,
                                                    cd_item = im.cd_item,
                                                    cd_item_kit = im.ItemMovItemKit.Where(imov => imov.cd_item_movimento == im.cd_item_movimento).FirstOrDefault() != null ? im.ItemMovItemKit.Where(imov => imov.cd_item_movimento == im.cd_item_movimento).FirstOrDefault().cd_item_kit : 0,
                                                    dc_item_movimento = im.dc_item_movimento,
                                                    qt_item_movimento = im.qt_item_movimento,
                                                    vl_unitario_item = im.vl_unitario_item,
                                                    vl_total_item = im.vl_total_item,
                                                    vl_liquido_item = im.vl_liquido_item,
                                                    vl_acrescimo_item = im.vl_acrescimo_item,
                                                    vl_desconto_item = im.vl_desconto_item,
                                                    cd_plano_conta = im.cd_plano_conta,
                                                    cd_grupo_estoque = im.Item.cd_grupo_estoque,
                                                    vl_ICMS_item = im.vl_ICMS_item,
                                                    cd_situacao_tributaria_ICMS = im.cd_situacao_tributaria_ICMS,
                                                    cd_situacao_tributaria_PIS = im.cd_situacao_tributaria_PIS,
                                                    cd_situacao_tributaria_COFINS = im.cd_situacao_tributaria_COFINS,
                                                    no_plano_conta = im.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta,
                                                    planoSugerido = db.ItemSubgrupo.Where(pc => pc.cd_item == im.cd_item && (pc.id_tipo_movimento + 1) == movimento.id_tipo_movimento &&
                                                                                          im.Item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cd_empresa).Any()).Any(),
                                                    pc_desconto_item = im.pc_desconto_item,
                                                    cd_tipo_item = im.Item.cd_tipo_item,
                                                    id_material_didatico = im.Item.id_material_didatico,
                                                    id_voucher_carga = im.Item.id_voucher_carga
                                                    //no_item = im.Item.no_item
                                                }).ToList().Select(x => new ItemMovimento
                                                {
                                                    cd_item_movimento = x.cd_item_movimento,
                                                    cd_movimento = x.cd_movimento,
                                                    cd_item = x.cd_item,
                                                    cd_item_kit = x.cd_item_kit,
                                                    dc_item_movimento = x.dc_item_movimento,
                                                    qt_item_movimento = x.qt_item_movimento,
                                                    vl_unitario_item = x.vl_unitario_item,
                                                    cd_grupo_estoque = x.cd_grupo_estoque,
                                                    vl_total_item = x.vl_total_item,
                                                    vl_liquido_item = x.vl_liquido_item,
                                                    vl_ICMS_item = x.vl_ICMS_item,
                                                    cd_situacao_tributaria_ICMS = x.cd_situacao_tributaria_ICMS,
                                                    cd_situacao_tributaria_PIS = x.cd_situacao_tributaria_PIS,
                                                    cd_situacao_tributaria_COFINS = x.cd_situacao_tributaria_COFINS,
                                                    vl_acrescimo_item = x.vl_acrescimo_item,
                                                    vl_desconto_item = x.vl_desconto_item,
                                                    cd_plano_conta = x.cd_plano_conta,
                                                    dc_plano_conta = x.no_plano_conta,
                                                    planoSugerido = x.planoSugerido,
                                                    pc_desconto_item = x.pc_desconto_item,
                                                    cd_tipo_item = x.cd_tipo_item,
                                                    id_material_didatico = x.id_material_didatico,
                                                    id_voucher_carga = x.id_voucher_carga
                                                    //no_item = x.no_item
                                                }).ToList();

                    movimento.ItemMovimentoKit = (from im in db.ItemMovimentoKit
                                                where im.cd_movimento == movimento.cd_movimento
                                                select new
                                                {
                                                    cd_item_movimento_kit = im.cd_item_movimento_kit,
                                                    cd_movimento = im.cd_movimento,
                                                    cd_item_kit = im.cd_item_kit,
                                                    qt_item_kit = im.qt_item_kit,
                                                    no_item_kit = im.Item.no_item
                                                }).ToList().Select(x => new ItemMovimentoKit
                                                {
                                                    cd_item_movimento_kit = x.cd_item_movimento_kit,
                                                    cd_movimento = x.cd_movimento,
                                                    cd_item_kit = x.cd_item_kit,
                                                    qt_item_kit = x.qt_item_kit,
                                                    no_item_kit = x.no_item_kit
                                                }).ToList();
                }
                return movimento;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Movimento getMovimento(int cd_movimento, int cd_empresa)
        {
            try
            {
                var sql = (from t in db.Movimento
                           where t.cd_pessoa_empresa == cd_empresa && t.cd_movimento == cd_movimento
                           select t).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Movimento getMovimentoWithItens(int cd_movimento, int cd_empresa)
        {
            try
            {
                var sql = (from t in db.Movimento
                        .Include("ItensMovimento")
                        .Include("Cheques")
                        .Include("ItensMovimento.Item")
                    where t.cd_pessoa_empresa == cd_empresa && t.cd_movimento == cd_movimento
                    select t).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public string getMovimentoWithItensName(int cd_movimento, int cd_empresa)
        {
            try
            {
                var sql = (from t in db.Movimento
                        .Include("Pessoa")
                    where t.cd_pessoa_empresa == cd_empresa && t.cd_movimento == cd_movimento
                    select t.Pessoa.no_pessoa).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Movimento getMovimentoComCheque(int cd_movimento, int cd_empresa)
        {
            try
            {
                var sql = (from t in db.Movimento.Include(x => x.Cheques)
                           where t.cd_pessoa_empresa == cd_empresa && t.cd_movimento == cd_movimento
                           select t).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int? getMaxMovimento(int tpMovto, int cd_empresa)
        {
            try
            {
                var sql = (from t in db.Movimento
                           where t.cd_pessoa_empresa == cd_empresa &&
                           t.id_tipo_movimento == tpMovto &&
                           !t.id_nf
                           select t.nm_movimento).Max(c => c);
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Espelho getEspelhoMovimento(int cd_movimento, int cd_empresa)
        {
            try
            {
                var sql = (from t in db.Movimento
                           where t.cd_pessoa_empresa == cd_empresa && t.cd_movimento == cd_movimento
                           select new Espelho
                           {
                               cd_movimento = t.cd_movimento,
                               no_pessoa = t.Aluno.cd_aluno > 0 ? t.Aluno.AlunoPessoaFisica.no_pessoa : t.Pessoa.no_pessoa,
                               nm_movimento = t.nm_movimento,
                               dc_serie_movimento = t.dc_serie_movimento,
                               dt_emissao_movimento = t.dt_emissao_movimento,
                               dt_vcto_movimento = t.dt_vcto_movimento,
                               dt_mov_movimento = t.dt_mov_movimento,
                               dc_tipo_financeiro = "NF",
                               dc_politica_comercial = t.PoliticaComercial.dc_politica_comercial,
                               pc_acrescimo = t.pc_acrescimo,
                               vl_acrescimo = t.vl_acrescimo,
                               pc_desconto = t.pc_desconto,
                               vl_desconto = t.vl_desconto,
                               vl_total_itens = t.ItensMovimento.Sum(x => x.vl_total_item),
                               vl_total_liq = t.ItensMovimento.Sum(x => x.vl_liquido_item),
                               tx_obs_movimento = t.tx_obs_movimento,
                               id_tipo_movimento = t.id_tipo_movimento
                           }).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Espelho> getSourceCopiaEspelhoMovimento(int cd_movimento, int cd_empresa)
        {
            try
            {
                int site = (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.SITE;
                int telefone = (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE;
                int email = (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL;
                var sql = (from it in db.ItemMovimento
                           where it.Movimento.cd_pessoa_empresa == cd_empresa && it.Movimento.cd_movimento == cd_movimento
                           select new
                           {
                               cd_movimento = it.Movimento.cd_movimento,
                               cd_pessoa_empresa = it.Movimento.cd_pessoa_empresa,

                               no_tipo_logradouro = it.Movimento.Empresa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro,
                               no_localidade = it.Movimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade,
                               dc_num_endereco = it.Movimento.Empresa.EnderecoPrincipal.dc_num_endereco,
                               dc_compl_endereco = it.Movimento.Empresa.EnderecoPrincipal.dc_compl_endereco,

                               dc_bairro_escola = it.Movimento.Empresa.EnderecoPrincipal.Bairro.no_localidade,
                               dc_num_cep_escola = it.Movimento.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep,
                               dc_cidade_escola = it.Movimento.Empresa.EnderecoPrincipal.Cidade.no_localidade,
                               sg_estado_escola = it.Movimento.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                               dc_mail_escola = it.Movimento.Empresa.TelefonePessoa.Where(tp => tp.cd_tipo_telefone == site).FirstOrDefault().dc_fone_mail,
                               dc_fone_escola = it.Movimento.Empresa.TelefonePessoa.Where(tp => tp.cd_tipo_telefone == telefone).FirstOrDefault().dc_fone_mail,

                               num_cnpj = it.Movimento.Empresa.dc_num_cgc,
                               dc_num_insc_estadual = it.Movimento.Empresa.dc_num_insc_estadual,
                               dc_num_insc_municipal = it.Movimento.Empresa.dc_num_insc_municipal,

                               cd_pessoa = it.Movimento.cd_pessoa,
                               no_pessoa = it.Movimento.Pessoa.no_pessoa,
                               no_pessoa_empresa = it.Movimento.Empresa.no_pessoa,

                               no_tipo_logradouro_pes = it.Movimento.Pessoa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro,
                               no_localidade_pes = it.Movimento.Pessoa.EnderecoPrincipal.Logradouro.no_localidade,
                               dc_num_endereco_pes = it.Movimento.Pessoa.EnderecoPrincipal.dc_num_endereco,
                               dc_num_cep_pessoa = it.Movimento.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep,
                               dc_compl_endereco_pes = it.Movimento.Pessoa.EnderecoPrincipal.dc_compl_endereco,

                               dc_bairro_pessoa = it.Movimento.Pessoa.EnderecoPrincipal.Bairro.no_localidade,
                               dc_cidade_pessoa = it.Movimento.Pessoa.EnderecoPrincipal.Cidade.no_localidade,
                               sg_estado_pessoa = it.Movimento.Pessoa.EnderecoPrincipal.Estado.Estado.sg_estado,
                               dc_mail_pessoa = it.Movimento.Pessoa.TelefonePessoa.Where(tp => tp.cd_tipo_telefone == email).FirstOrDefault().dc_fone_mail,
                               dc_fone_pessoa = it.Movimento.Pessoa.TelefonePessoa.Where(tp => tp.cd_tipo_telefone == telefone).FirstOrDefault().dc_fone_mail,
                               cpf_pessoa = it.Movimento.Pessoa.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                           db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == it.Movimento.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                              db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == it.Movimento.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                                   db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == it.Movimento.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                                   db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == it.Movimento.Pessoa.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,

                               nm_movimento = it.Movimento.nm_movimento,
                               dc_serie_movimento = it.Movimento.dc_serie_movimento,
                               dt_emissao_movimento = it.Movimento.dt_emissao_movimento,
                               dt_vcto_movimento = it.Movimento.dt_vcto_movimento,
                               dt_mov_movimento = it.Movimento.dt_mov_movimento,
                               dc_tipo_financeiro = it.Movimento.TipoFinanceiro.dc_tipo_financeiro,
                               dc_politica_comercial = it.Movimento.PoliticaComercial.dc_politica_comercial,
                               vl_acrescimo_item = it.vl_acrescimo_item,
                               //pc_desconto = it.Movimento.pc_desconto,
                               vl_desconto_item = it.vl_desconto_item,

                               id_tipo_movimento = it.Movimento.id_tipo_movimento,
                               tx_obs_movimento = it.Movimento.tx_obs_movimento,
                               dc_item_movimento = it.dc_item_movimento,
                               qt_item_movimento = it.qt_item_movimento,
                               vl_unitario_item = it.vl_unitario_item,
                               vl_liquido_item = it.vl_liquido_item
                           }).ToList().Select(x => new Espelho
                           {
                               cd_movimento = x.cd_movimento,
                               cd_pessoa_empresa = x.cd_pessoa_empresa,

                               enderecoEscola = new EnderecoSGF()
                               {
                                   TipoLogradouro = new TipoLogradouroSGF()
                                   {
                                       no_tipo_logradouro = x.no_tipo_logradouro
                                   },
                                   Logradouro = new LocalidadeSGF()
                                   {
                                       no_localidade = x.no_localidade
                                   },
                                   Estado = new LocalidadeSGF()
                                   {
                                       Estado = new EstadoSGF()
                                       {
                                           sg_estado = x.sg_estado_escola
                                       }
                                   },
                                   dc_num_endereco = x.dc_num_endereco,
                                   dc_compl_endereco = x.dc_compl_endereco
                               },
                               sg_estado_escola = x.sg_estado_escola,

                               dc_bairro_escola = x.dc_bairro_escola,
                               dc_num_cep_escola = x.dc_num_cep_escola,
                               dc_cidade_escola = x.dc_cidade_escola,
                               dc_mail_escola = x.dc_mail_escola,
                               dc_fone_escola = x.dc_fone_escola,

                               num_cnpj = x.num_cnpj,
                               dc_num_insc_estadual = x.dc_num_insc_estadual,
                               dc_num_insc_municipal = x.dc_num_insc_municipal,

                               cd_pessoa = x.cd_pessoa,
                               no_pessoa = x.no_pessoa,
                               no_pessoa_empresa = x.no_pessoa_empresa,
                               enderecoPessoa = new EnderecoSGF()
                               {
                                   TipoLogradouro = new TipoLogradouroSGF()
                                   {
                                       no_tipo_logradouro = x.no_tipo_logradouro_pes
                                   },
                                   Logradouro = new LocalidadeSGF()
                                   {
                                       no_localidade = x.no_localidade_pes
                                   },
                                   Estado = new LocalidadeSGF()
                                   {
                                       Estado = new EstadoSGF()
                                       {
                                           sg_estado = x.sg_estado_pessoa
                                       }
                                   },
                                   dc_num_endereco = x.dc_num_endereco_pes,
                                   dc_compl_endereco = x.dc_compl_endereco_pes
                               },
                               sg_estado_pessoa = x.sg_estado_pessoa,
                               dc_bairro_pessoa = x.dc_bairro_pessoa,
                               dc_num_cep_pessoa = x.dc_num_cep_pessoa,
                               dc_cidade_pessoa = x.dc_cidade_pessoa,
                               dc_mail_pessoa = x.dc_mail_pessoa,
                               dc_fone_pessoa = x.dc_fone_pessoa,
                               cpf_pessoa = x.cpf_pessoa,

                               nm_movimento = x.nm_movimento,
                               dc_serie_movimento = x.dc_serie_movimento,
                               dt_emissao_movimento = x.dt_emissao_movimento,
                               dt_vcto_movimento = x.dt_vcto_movimento,
                               dt_mov_movimento = x.dt_mov_movimento,
                               dc_tipo_financeiro = x.dc_tipo_financeiro,
                               dc_politica_comercial = x.dc_politica_comercial,
                               vl_acrescimo_item = x.vl_acrescimo_item,
                               vl_desconto_item = x.vl_desconto_item,
                               vl_liquido_item = x.vl_liquido_item,
                               id_tipo_movimento = x.id_tipo_movimento,
                               tx_obs_movimento = x.tx_obs_movimento,
                               dc_item_movimento = x.dc_item_movimento,
                               qt_item = x.qt_item_movimento,
                               vl_unitario = x.vl_unitario_item
                           });

                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int getUltimoNroRecibo(int? nm_ultimo_Recibo, int cd_empresa)
        {
            try
            {
                int ultimo = 0;
                int? ultimo_recibo = null;

                if (nm_ultimo_Recibo.HasValue)
                {

                    ultimo_recibo = (from c in db.BaixaTitulo
                                     where c.nm_recibo == nm_ultimo_Recibo.Value
                                        && c.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa
                                     select c.nm_recibo).FirstOrDefault();

                    if (ultimo_recibo.HasValue)
                        //Pesquisa a primeira matrícula que possui número de matrícula maior ou igual que a do parâmetro, mas que não tenha a próxima matrícula (nro + 1):
                        ultimo_recibo = (from c in db.BaixaTitulo
                                         where !(from c2 in db.BaixaTitulo
                                                 where c2.nm_recibo == c.nm_recibo + 1
                                                 && c2.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa
                                                 select c2.nm_recibo).Any()
                                         where c.nm_recibo >= nm_ultimo_Recibo.Value - 1
                                            && c.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa
                                         select c.nm_recibo).Min();
                    else
                        ultimo_recibo = nm_ultimo_Recibo - 1;
                }
                else
                {
                    ultimo_recibo = (from c in db.BaixaTitulo
                                     where c.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa && c.nm_recibo > 0
                                     orderby c.nm_recibo descending
                                     select c.nm_recibo).FirstOrDefault();
                }
                if (ultimo_recibo.HasValue)
                    ultimo = ultimo_recibo.Value;
                return ultimo;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeDadosNFNota(int cdCidade)
        {
            try
            {
                Movimento sql = (from l in db.Movimento
                                 where l.id_nf &&
                                       l.id_tipo_movimento == (int)FundacaoFisk.SGF.GenericModel.Movimento.TipoMovimentoEnum.SERVICO &&
                                       l.Empresa.EnderecoPrincipal.cd_loc_cidade == cdCidade
                                 select l).FirstOrDefault();
                return sql != null && sql.cd_movimento > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeICMSEstadoNota(int cdEstadoOri, int cdEstadoDes)
        {
            try
            {
                Movimento sql = (from l in db.Movimento
                                 where l.id_nf &&
                                       (l.id_tipo_movimento == (int)FundacaoFisk.SGF.GenericModel.Movimento.TipoMovimentoEnum.SAIDA ||
                                       l.id_tipo_movimento == (int)FundacaoFisk.SGF.GenericModel.Movimento.TipoMovimentoEnum.ENTRADA) &&
                                       l.Empresa.EnderecoPrincipal.cd_loc_estado == cdEstadoOri &&
                                       l.Pessoa.EnderecoPrincipal.cd_loc_estado == cdEstadoDes
                                 select l).FirstOrDefault();
                return sql != null && sql.cd_movimento > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Movimento> getMovimentosbyOrigem(int cd_origem, int id_origem_movto, int cd_empresa)
        {
            int? cdEmpresaLig = (from empresa in db.PessoaSGF.OfType<Escola>() where empresa.cd_pessoa == cd_empresa select empresa.cd_empresa_coligada).FirstOrDefault();
            cd_empresa = (int)(cdEmpresaLig == null ? cd_empresa : cdEmpresaLig);
            try
            {
                List<Movimento> sql = new List<Movimento>();
                sql = (from t in db.Movimento
                       where t.cd_pessoa_empresa == cd_empresa &&
                             t.cd_origem_movimento == cd_origem &&
                             t.id_origem_movimento == id_origem_movto
                       select new
                       {
                           cd_movimento = t.cd_movimento,
                           id_venda_futura = t.id_venda_futura,
                           id_material_didatico = t.id_material_didatico
                       }).ToList().Select(x => new Movimento
                       {
                           cd_movimento = x.cd_movimento,
                           id_venda_futura = x.id_venda_futura,
                           id_material_didatico = x.id_material_didatico
                       }).ToList();
                return sql;
            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }
        public Movimento getMovimentoEditOrigem(int cd_origem, int id_origem_movto, int cd_empresa, int id_tipo_movimento)
        {
            int? cdEmpresaLig = (from empresa in db.PessoaSGF.OfType<Escola>() where empresa.cd_pessoa == cd_empresa select empresa.cd_empresa_coligada).FirstOrDefault();
            cd_empresa = (int)(cdEmpresaLig == null ? cd_empresa : cdEmpresaLig); 
            try
            {
                Movimento movimento = new Movimento();
                switch (id_tipo_movimento)
                {
                    case (int)Movimento.TipoMovimentoEnum.ENTRADA:
                    case (int)Movimento.TipoMovimentoEnum.SAIDA:
                        movimento = (from t in db.Movimento
                                     where 
                                           ((cd_origem < 0 && t.cd_movimento == Math.Abs(cd_origem)) ||
                                            (cd_origem > 0 &&
                                             t.cd_pessoa_empresa == cd_empresa &&
                                             t.cd_origem_movimento == cd_origem &&
                                             t.id_origem_movimento == id_origem_movto)
                                           )
                                     select new
                                     {
                                         cd_movimento = t.cd_movimento,
                                         cd_pessoa = t.cd_pessoa,
                                         cd_politica_comercial = t.cd_politica_comercial,
                                         cd_tipo_financeiro = t.cd_tipo_financeiro,
                                         nm_movimento = t.nm_movimento,
                                         dc_serie_movimento = t.dc_serie_movimento,
                                         dt_emissao_movimento = t.dt_emissao_movimento,
                                         dt_vcto_movimento = t.dt_vcto_movimento,
                                         dt_mov_movimento = t.dt_mov_movimento,
                                         pc_acrescimo = t.pc_acrescimo,
                                         vl_acrescimo = t.vl_acrescimo,
                                         pc_desconto = t.pc_desconto,
                                         vl_desconto = t.vl_desconto,
                                         tx_obs_movimento = t.tx_obs_movimento,
                                         no_pessoa = t.Pessoa.no_pessoa,
                                         dc_politica_comercial = t.PoliticaComercial.dc_politica_comercial,
                                         id_tipo_movimento = t.id_tipo_movimento,
                                         cheque = t.Cheques.FirstOrDefault(),
                                         id_nf = t.id_nf,
                                         id_nf_escola = t.id_nf_escola,
                                         cd_tipo_nota_fiscal = t.cd_tipo_nota_fiscal,
                                         id_status_nf = t.id_status_nf,
                                         vl_base_calculo_ICMS_nf = t.vl_base_calculo_ICMS_nf,
                                         vl_base_calculo_PIS_nf = t.vl_base_calculo_PIS_nf,
                                         vl_base_calculo_COFINS_nf = t.vl_base_calculo_COFINS_nf,
                                         vl_base_calculo_IPI_nf = t.vl_base_calculo_IPI_nf,
                                         vl_base_calculo_ISS_nf = t.vl_base_calculo_ISS_nf,
                                         vl_ICMS_nf = t.vl_ICMS_nf,
                                         vl_PIS_nf = t.vl_PIS_nf,
                                         vl_COFINS_nf = t.vl_COFINS_nf,
                                         vl_IPI_nf = t.vl_IPI_nf,
                                         vl_ISS_nf = t.vl_ISS_nf,
                                         tx_obs_fiscal = t.tx_obs_fiscal,
                                         dc_justificativa_nf = t.dc_justificativa_nf,
                                         id_origem_movimento = t.id_origem_movimento,
                                         dc_cfop_nf = t.dc_cfop_nf,
                                         cd_origem_movimento = t.cd_origem_movimento,
                                         cd_nota_fiscal = t.cd_nota_fiscal,
                                         cd_aluno = t.cd_aluno,
                                         nm_contrato = (t.id_material_didatico == true && t.cd_origem_movimento != null ? (from cc in db.Contrato where cc.cd_contrato == t.cd_origem_movimento select cc.nm_contrato).FirstOrDefault() : null),
                                         nm_matricula_contrato = (t.id_material_didatico == true && t.cd_origem_movimento != null ? (from cc in db.Contrato where cc.cd_contrato == t.cd_origem_movimento select cc.nm_matricula_contrato).FirstOrDefault() : null),
                                         cd_curso = t.cd_curso,
                                         no_curso = (t.id_material_didatico == true && t.cd_curso != null ? (from c in db.Curso where t.cd_curso == c.cd_curso select c.no_curso).FirstOrDefault() : null),
                                         id_venda_futura = t.id_venda_futura,
                                         id_material_didatico = t.id_material_didatico

                                     }).ToList().Select(x => new Movimento
                                     {
                                         cd_movimento = x.cd_movimento,
                                         cd_pessoa = x.cd_pessoa,
                                         cd_politica_comercial = x.cd_politica_comercial,
                                         cd_tipo_financeiro = x.cd_tipo_financeiro,
                                         nm_movimento = x.nm_movimento,
                                         dc_serie_movimento = x.dc_serie_movimento,
                                         dt_emissao_movimento = x.dt_emissao_movimento,
                                         dt_vcto_movimento = x.dt_vcto_movimento,
                                         dt_mov_movimento = x.dt_mov_movimento,
                                         pc_acrescimo = x.pc_acrescimo,
                                         vl_acrescimo = x.vl_acrescimo,
                                         pc_desconto = x.pc_desconto,
                                         vl_desconto = x.vl_desconto,
                                         tx_obs_movimento = x.tx_obs_movimento,
                                         no_pessoa = x.no_pessoa,
                                         dc_politica_comercial = x.dc_politica_comercial,
                                         id_tipo_movimento = x.id_tipo_movimento,
                                         cheque = x.cheque,
                                         id_nf = x.id_nf,
                                         id_nf_escola = x.id_nf_escola,
                                         cd_tipo_nota_fiscal = x.cd_tipo_nota_fiscal,
                                         id_status_nf = x.id_status_nf,
                                         vl_base_calculo_ICMS_nf = x.vl_base_calculo_ICMS_nf,
                                         vl_base_calculo_PIS_nf = x.vl_base_calculo_PIS_nf,
                                         vl_base_calculo_COFINS_nf = x.vl_base_calculo_COFINS_nf,
                                         vl_base_calculo_IPI_nf = x.vl_base_calculo_IPI_nf,
                                         vl_base_calculo_ISS_nf = x.vl_base_calculo_ISS_nf,
                                         vl_ICMS_nf = x.vl_ICMS_nf,
                                         vl_PIS_nf = x.vl_PIS_nf,
                                         vl_COFINS_nf = x.vl_COFINS_nf,
                                         vl_IPI_nf = x.vl_IPI_nf,
                                         vl_ISS_nf = x.vl_ISS_nf,
                                         tx_obs_fiscal = x.tx_obs_fiscal,
                                         dc_justificativa_nf = x.dc_justificativa_nf,
                                         id_origem_movimento = x.id_origem_movimento,
                                         dc_cfop_nf = x.dc_cfop_nf,
                                         cd_origem_movimento = x.cd_origem_movimento,
                                         cd_nota_fiscal = x.cd_nota_fiscal,
                                         cd_aluno = x.cd_aluno,
                                         cd_curso = x.cd_curso,
                                         no_curso = x.no_curso,
                                         nm_contrato = x.nm_contrato,
                                         nm_matricula_contrato = x.nm_matricula_contrato,
                                         id_venda_futura = x.id_venda_futura,
                                         id_material_didatico = x.id_material_didatico
                                     }).FirstOrDefault();
                        if (movimento != null && movimento.cd_movimento > 0)
                            movimento.ItensMovimento = (from im in db.ItemMovimento
                                                        where im.cd_movimento == movimento.cd_movimento
                                                        select new
                                                        {
                                                            cd_item_movimento = im.cd_item_movimento,
                                                            cd_movimento = im.cd_movimento,
                                                            cd_item = im.cd_item,
                                                            dc_item_movimento = im.dc_item_movimento,
                                                            qt_item_movimento = im.qt_item_movimento,
                                                            vl_unitario_item = im.vl_unitario_item,
                                                            vl_total_item = im.vl_total_item,
                                                            vl_liquido_item = im.vl_liquido_item,
                                                            vl_acrescimo_item = im.vl_acrescimo_item,
                                                            vl_desconto_item = im.vl_desconto_item,
                                                            cd_plano_conta = im.cd_plano_conta,
                                                            no_plano_conta = im.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta,
                                                            planoSugerido = db.ItemSubgrupo.Where(pc => pc.cd_item == im.cd_item &&
                                                                            (pc.id_tipo_movimento + 1) == movimento.id_tipo_movimento &&
                                                                            im.Item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cd_empresa).Any()).Any(),
                                                            pc_desconto = im.pc_desconto_item,
                                                            vl_desconto = im.vl_desconto_item,
                                                            //Fiscal
                                                            cd_situacao_tributaria_ICMS = im.cd_situacao_tributaria_ICMS,
                                                            cd_situacao_tributaria_PIS = im.cd_situacao_tributaria_PIS,
                                                            cd_situacao_tributaria_COFINS = im.cd_situacao_tributaria_COFINS,
                                                            vl_base_calculo_ICMS_item = im.vl_base_calculo_ICMS_item,
                                                            vl_base_calculo_PIS_item = im.vl_base_calculo_PIS_item,
                                                            vl_base_calculo_COFINS_item = im.vl_base_calculo_COFINS_item,
                                                            vl_base_calculo_IPI_item = im.vl_base_calculo_IPI_item,
                                                            vl_ICMS_item = im.vl_ICMS_item,
                                                            vl_PIS_item = im.vl_PIS_item,
                                                            vl_COFINS_item = im.vl_COFINS_item,
                                                            vl_IPI_item = im.vl_IPI_item,
                                                            pc_aliquota_ICMS = im.pc_aliquota_ICMS,
                                                            pc_aliquota_PIS = im.pc_aliquota_PIS,
                                                            pc_aliquota_COFINS = im.pc_aliquota_COFINS,
                                                            pc_aliquota_IPI = im.pc_aliquota_IPI,
                                                            cd_tipo_item = im.Item.cd_tipo_item,
                                                        }).ToList().Select(x => new ItemMovimento
                                                        {
                                                            cd_item_movimento = x.cd_item_movimento,
                                                            cd_movimento = x.cd_movimento,
                                                            cd_item = x.cd_item,
                                                            dc_item_movimento = x.dc_item_movimento,
                                                            qt_item_movimento = x.qt_item_movimento,
                                                            vl_unitario_item = x.vl_unitario_item,
                                                            vl_total_item = x.vl_total_item,
                                                            vl_liquido_item = x.vl_liquido_item,
                                                            vl_acrescimo_item = x.vl_acrescimo_item,
                                                            vl_desconto_item = x.vl_desconto_item,
                                                            cd_plano_conta = x.cd_plano_conta,
                                                            dc_plano_conta = x.no_plano_conta,
                                                            planoSugerido = x.planoSugerido,
                                                            pc_desconto_item = x.pc_desconto,
                                                            //Fiscal
                                                            cd_situacao_tributaria_ICMS = x.cd_situacao_tributaria_ICMS,
                                                            cd_situacao_tributaria_PIS = x.cd_situacao_tributaria_PIS,
                                                            cd_situacao_tributaria_COFINS = x.cd_situacao_tributaria_COFINS,
                                                            vl_base_calculo_ICMS_item = x.vl_base_calculo_ICMS_item,
                                                            vl_base_calculo_PIS_item = x.vl_base_calculo_PIS_item,
                                                            vl_base_calculo_COFINS_item = x.vl_base_calculo_COFINS_item,
                                                            vl_base_calculo_IPI_item = x.vl_base_calculo_IPI_item,
                                                            vl_ICMS_item = x.vl_ICMS_item,
                                                            vl_PIS_item = x.vl_PIS_item,
                                                            vl_COFINS_item = x.vl_COFINS_item,
                                                            vl_IPI_item = x.vl_IPI_item,
                                                            pc_aliquota_ICMS = x.pc_aliquota_ICMS,
                                                            pc_aliquota_PIS = x.pc_aliquota_PIS,
                                                            pc_aliquota_COFINS = x.pc_aliquota_COFINS,
                                                            pc_aliquota_IPI = x.pc_aliquota_IPI,
                                                            cd_tipo_item = x.cd_tipo_item
                                                        }).ToList();
                        break;
                    case (int)Movimento.TipoMovimentoEnum.SERVICO:
                        int cd_titulo = db.BaixaTitulo.Where(b => b.cd_baixa_titulo == cd_origem && b.Titulo.cd_pessoa_empresa == cd_empresa).FirstOrDefault() != null ? db.BaixaTitulo.Where(b => b.cd_baixa_titulo == cd_origem && b.Titulo.cd_pessoa_empresa == cd_empresa).FirstOrDefault().cd_titulo : 0;
                        movimento = (from t in db.Movimento
                                     where t.cd_pessoa_empresa == cd_empresa &&
                                           ((t.cd_origem_movimento == cd_origem && t.id_origem_movimento == id_origem_movto) ||
                                            (cd_titulo > 0 && db.BaixaTitulo.Any(b => t.cd_origem_movimento == b.cd_baixa_titulo && t.id_origem_movimento == id_origem_movto &&
                                             b.cd_titulo == cd_titulo && b.Titulo.cd_pessoa_empresa == cd_empresa)))
                                     select new
                                     {
                                         cd_movimento = t.cd_movimento,
                                         cd_pessoa = t.cd_pessoa,
                                         cd_politica_comercial = t.cd_politica_comercial,
                                         cd_tipo_financeiro = t.cd_tipo_financeiro,
                                         nm_movimento = t.nm_movimento,
                                         dc_serie_movimento = t.dc_serie_movimento,
                                         dt_emissao_movimento = t.dt_emissao_movimento,
                                         dt_vcto_movimento = t.dt_vcto_movimento,
                                         dt_mov_movimento = t.dt_mov_movimento,
                                         pc_acrescimo = t.pc_acrescimo,
                                         vl_acrescimo = t.vl_acrescimo,
                                         pc_desconto = t.pc_desconto,
                                         vl_desconto = t.vl_desconto,
                                         tx_obs_movimento = t.tx_obs_movimento,
                                         id_nf = t.id_nf,
                                         cd_tipo_nota_fiscal = t.cd_tipo_nota_fiscal,
                                         id_status_nf = t.id_status_nf,
                                         vl_base_calculo_ISS_nf = t.vl_base_calculo_ISS_nf,
                                         dc_cfop_nf = t.dc_cfop_nf,
                                         vl_ISS_nf = t.vl_ISS_nf,
                                         no_pessoa = t.Pessoa.no_pessoa,
                                         dc_politica_comercial = t.PoliticaComercial.dc_politica_comercial,
                                         dc_tipo_nota = t.TipoNF.dc_tipo_nota_fiscal,
                                         id_tipo_movimento = t.id_tipo_movimento,
                                         cheque = t.Cheques.FirstOrDefault(),
                                         id_nf_escola = t.id_nf_escola
                                     }).ToList().Select(x => new Movimento
                                     {
                                         cd_movimento = x.cd_movimento,
                                         cd_pessoa = x.cd_pessoa,
                                         cd_politica_comercial = x.cd_politica_comercial,
                                         cd_tipo_financeiro = x.cd_tipo_financeiro,
                                         cd_tipo_nota_fiscal = x.cd_tipo_nota_fiscal,
                                         nm_movimento = x.nm_movimento,
                                         dc_serie_movimento = x.dc_serie_movimento,
                                         dt_emissao_movimento = x.dt_emissao_movimento,
                                         dt_vcto_movimento = x.dt_vcto_movimento,
                                         dt_mov_movimento = x.dt_mov_movimento,
                                         pc_acrescimo = x.pc_acrescimo,
                                         vl_acrescimo = x.vl_acrescimo,
                                         pc_desconto = x.pc_desconto,
                                         vl_desconto = x.vl_desconto,
                                         tx_obs_movimento = x.tx_obs_movimento,
                                         id_nf = x.id_nf,
                                         id_nf_escola = x.id_nf_escola,
                                         id_status_nf = x.id_status_nf,
                                         vl_base_calculo_ISS_nf = x.vl_base_calculo_ISS_nf,
                                         dc_cfop_nf = x.dc_cfop_nf,
                                         vl_ISS_nf = x.vl_ISS_nf,
                                         no_pessoa = x.no_pessoa,
                                         dc_politica_comercial = x.dc_politica_comercial,
                                         dc_tipo_nota = x.dc_tipo_nota,
                                         id_tipo_movimento = x.id_tipo_movimento,
                                         cheque = x.cheque
                                     }).FirstOrDefault();
                        if (movimento != null && movimento.cd_movimento > 0)
                            movimento.ItensMovimento = (from im in db.ItemMovimento
                                                        where im.cd_movimento == movimento.cd_movimento
                                                        select new
                                                        {
                                                            cd_item_movimento = im.cd_item_movimento,
                                                            cd_movimento = im.cd_movimento,
                                                            cd_item = im.cd_item,
                                                            dc_item_movimento = im.dc_item_movimento,
                                                            qt_item_movimento = im.qt_item_movimento,
                                                            vl_unitario_item = im.vl_unitario_item,
                                                            vl_total_item = im.vl_total_item,
                                                            vl_liquido_item = im.vl_liquido_item,
                                                            vl_acrescimo_item = im.vl_acrescimo_item,
                                                            vl_desconto_item = im.vl_desconto_item,
                                                            cd_plano_conta = im.cd_plano_conta,
                                                            no_plano_conta = im.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta,
                                                            planoSugerido = db.ItemSubgrupo.Where(pc => pc.cd_item == im.cd_item &&
                                                                (pc.id_tipo_movimento + 1) == movimento.id_tipo_movimento &&
                                                                 im.Item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cd_empresa).Any()).Any(),
                                                            pc_desconto = im.pc_desconto_item,
                                                            vl_desconto = im.vl_desconto_item,
                                                            //Fiscal
                                                            vl_base_calculo_ISS_item = im.vl_base_calculo_ISS_item,
                                                            pc_aliquota_ISS = im.pc_aliquota_ISS,
                                                            vl_ISS_item = im.vl_ISS_item
                                                            //no_item = im.Item.no_item
                                                        }).ToList().Select(x => new ItemMovimento
                                                        {
                                                            cd_item_movimento = x.cd_item_movimento,
                                                            cd_movimento = x.cd_movimento,
                                                            cd_item = x.cd_item,
                                                            dc_item_movimento = x.dc_item_movimento,
                                                            qt_item_movimento = x.qt_item_movimento,
                                                            vl_unitario_item = x.vl_unitario_item,
                                                            vl_total_item = x.vl_total_item,
                                                            vl_liquido_item = x.vl_liquido_item,
                                                            vl_acrescimo_item = x.vl_acrescimo_item,
                                                            vl_desconto_item = x.vl_desconto_item,
                                                            cd_plano_conta = x.cd_plano_conta,
                                                            dc_plano_conta = x.no_plano_conta,
                                                            planoSugerido = x.planoSugerido,
                                                            pc_desconto_item = x.pc_desconto,
                                                            vl_base_calculo_ISS_item = x.vl_base_calculo_ISS_item,
                                                            pc_aliquota_ISS = x.pc_aliquota_ISS,
                                                            vl_ISS_item = x.vl_ISS_item
                                                            //no_item = x.no_item
                                                        }).ToList();
                        break;
                }

                return movimento;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeMovimentoByOrigem(int cdOrigem, int id_origem_movimento)
        {
            try
            {
                int sql = (from l in db.Movimento
                           where l.id_origem_movimento == id_origem_movimento &&
                           l.cd_origem_movimento == cdOrigem
                           select l.cd_movimento).FirstOrDefault();
                return sql > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeTituloBaixadoByMovimento(int cd_movimento)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Movimento"].ToString());
                bool existe = (from t in db.Titulo
                               where t.cd_origem_titulo == cd_movimento
                               && t.id_origem_titulo == cd_origem
                               && (t.vl_liquidacao_titulo > 0
                                  || (t.id_status_cnab != (int)Titulo.StatusCnabTitulo.INICIAL && t.id_status_cnab != (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA))
                               select t).Any();
                return existe;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Movimento> getMovimentosNotaServico(List<int> cd_movimentos)
        {
            try
            {
                List<Movimento> movimentos = (from m in db.Movimento
                                              where cd_movimentos.Contains(m.cd_movimento)
                                              select new
                                              {
                                                  no_empresa = m.Empresa.no_pessoa,
                                                  dc_num_cgc = m.Empresa.dc_num_cgc,
                                                  dc_num_insc_municipal = m.Empresa.dc_num_insc_municipal,
                                                  dc_serie_movimento = m.dc_serie_movimento,
                                                  nm_movimento = m.nm_movimento,
                                                  id_regime_tributario = m.TipoNF.id_regime_tributario,
                                                  pc_aliquota_aproximada = m.pc_aliquota_aproximada,
                                                  vl_aproximado = m.vl_aproximado,
                                                  dt_emissao_movimento = m.dt_emissao_movimento,
                                                  id_status_nf = m.id_status_nf == (byte)Movimento.StatusNFEnum.FECHADO ? (byte)1 : (byte)2,
                                                  dc_tributacao_municipio = m.Empresa.EnderecoPrincipal.Cidade.DadosNF.FirstOrDefault().dc_tributacao_municipio,
                                                  dc_item_servico = m.Empresa.EnderecoPrincipal.Cidade.DadosNF.FirstOrDefault().dc_item_servico,
                                                  vl_liquido_itens = (decimal?)m.ItensMovimento.Sum(im => im.vl_liquido_item),
                                                  pc_aliquota_iss = (double?)m.Empresa.EnderecoPrincipal.Cidade.DadosNF.FirstOrDefault().pc_aliquota_iss,
                                                  vl_ISS_nf = m.vl_ISS_nf,
                                                  nm_cpf_cgc = m.Pessoa.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                                    db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == m.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                                       db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == m.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                                            db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == m.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                                                db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == m.Pessoa.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                                                  no_pessoa = m.Pessoa.no_pessoa,
                                                  no_tipo_logradouro = m.Pessoa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro,
                                                  no_localidade = m.Pessoa.EnderecoPrincipal.Logradouro.no_localidade,
                                                  dc_num_endereco = m.Pessoa.EnderecoPrincipal.dc_num_endereco,
                                                  dc_compl_endereco = m.Pessoa.EnderecoPrincipal.dc_compl_endereco,
                                                  no_bairro = m.Pessoa.EnderecoPrincipal.Bairro.no_localidade,
                                                  no_cidade = m.Pessoa.EnderecoPrincipal.Cidade.no_localidade,
                                                  sg_estado = m.Pessoa.EnderecoPrincipal.Estado.Estado.sg_estado,
                                                  dc_num_cep = m.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep,
                                                  dc_fone_mail = m.Pessoa.TelefonePessoa.Where(telefone => telefone.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && telefone.id_telefone_principal).FirstOrDefault().dc_fone_mail,
                                                  tx_obs_fiscal = m.tx_obs_fiscal
                                              }).ToList().Select(x => new Movimento
                                              {
                                                  Empresa = new Escola()
                                                  {
                                                      no_pessoa = x.no_empresa,
                                                      dc_num_cgc = x.dc_num_cgc,
                                                      dc_num_insc_municipal = x.dc_num_insc_municipal,
                                                      EnderecoPrincipal = new EnderecoSGF()
                                                      {
                                                          Cidade = new LocalidadeSGF()
                                                          {
                                                              DadosNFCidade = new DadosNF()
                                                              {
                                                                  dc_tributacao_municipio = x.dc_tributacao_municipio,
                                                                  dc_item_servico = x.dc_item_servico,
                                                                  pc_aliquota_iss = x.pc_aliquota_iss == null ? 0 : x.pc_aliquota_iss.Value
                                                              }
                                                          }
                                                      }
                                                  },
                                                  TipoNF = new GenericModel.TipoNotaFiscal()
                                                  {
                                                      id_regime_tributario = x.id_regime_tributario
                                                  },
                                                  pc_aliquota_aproximada = x.pc_aliquota_aproximada,
                                                  vl_aproximado = x.vl_aproximado,
                                                  vl_liquido_itens = x.vl_liquido_itens,
                                                  dc_serie_movimento = x.dc_serie_movimento,
                                                  nm_movimento = x.nm_movimento,
                                                  dt_emissao_movimento = x.dt_emissao_movimento,
                                                  id_status_nf = x.id_status_nf,
                                                  vl_ISS_nf = x.vl_ISS_nf,
                                                  Pessoa = new PessoaSGF()
                                                  {
                                                      nm_cpf_cgc = x.nm_cpf_cgc.TrimEnd(),
                                                      no_pessoa = x.no_pessoa,
                                                      EnderecoPrincipal = new EnderecoSGF()
                                                      {
                                                          TipoLogradouro = new TipoLogradouroSGF()
                                                          {
                                                              no_tipo_logradouro = x.no_tipo_logradouro
                                                          },
                                                          Logradouro = new LocalidadeSGF()
                                                          {
                                                              no_localidade = x.no_localidade,
                                                              dc_num_cep = x.dc_num_cep
                                                          },
                                                          Bairro = new LocalidadeSGF()
                                                          {
                                                              no_localidade = x.no_bairro
                                                          },
                                                          Cidade = new LocalidadeSGF()
                                                          {
                                                              no_localidade = x.no_cidade
                                                          },
                                                          Estado = new LocalidadeSGF()
                                                          {
                                                              Estado = new EstadoSGF()
                                                              {
                                                                  sg_estado = x.sg_estado
                                                              }
                                                          },
                                                          dc_num_endereco = x.dc_num_endereco,
                                                          dc_compl_endereco = x.dc_compl_endereco
                                                      },
                                                      Telefone = new TelefoneSGF()
                                                      {
                                                          dc_fone_mail = x.dc_fone_mail,
                                                      }
                                                  },
                                                  tx_obs_fiscal = x.tx_obs_fiscal
                                              }).ToList();
                return movimentos;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Movimento getMovimentosNotaProduto(int cd_movimento)
        {
            try
            {
                Movimento movimento = (from m in db.Movimento
                                       where m.cd_movimento == cd_movimento
                                       select new
                                       {
                                           sg_estado_emp = m.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                                           cd_movimento = m.cd_movimento,
                                           dc_natureza_operacao = m.TipoNF.dc_natureza_operacao,
                                           dc_serie_movimento = m.dc_serie_movimento,
                                           nm_movimento = m.nm_movimento,
                                           dt_emissao_movimento = m.dt_emissao_movimento,
                                           dt_mov_movimento = m.dt_mov_movimento,
                                           id_natureza_movimento = m.TipoNF.id_natureza_movimento,
                                           nm_municipio_emp = m.Empresa.EnderecoPrincipal.Cidade.nm_municipio,
                                           id_tipo_movimento = m.id_tipo_movimento,
                                           dc_num_cgc = m.Empresa.dc_num_cgc,
                                           no_empresa = m.Empresa.no_pessoa,
                                           dc_reduzido_pessoa = m.Empresa.dc_reduzido_pessoa,

                                           no_localidade_emp = m.Empresa.EnderecoPrincipal.Logradouro.no_localidade,
                                           dc_num_endereco_emp = m.Empresa.EnderecoPrincipal.dc_num_endereco,
                                           no_bairro_emp = m.Empresa.EnderecoPrincipal.Bairro.no_localidade,
                                           //nm_municipio = m.Empresa.EnderecoPrincipal.Cidade.nm_municipio,
                                           no_cidade_emp = m.Empresa.EnderecoPrincipal.Cidade.no_localidade,
                                           //sg_estado = m.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                                           dc_num_cep_emp = m.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep,
                                           dc_num_pais_emp = m.Empresa.EnderecoPrincipal.Pais.Pais.dc_num_pais,
                                           dc_pais_emp = m.Empresa.EnderecoPrincipal.Pais.Pais.dc_pais,

                                           no_pessoa = m.Pessoa.no_pessoa,
                                           no_localidade = m.Pessoa.EnderecoPrincipal.Logradouro.no_localidade,
                                           dc_num_endereco = m.Pessoa.EnderecoPrincipal.dc_num_endereco,
                                           no_bairro = m.Pessoa.EnderecoPrincipal.Bairro.no_localidade,
                                           nm_municipio = m.Pessoa.EnderecoPrincipal.Cidade.nm_municipio,
                                           no_cidade = m.Pessoa.EnderecoPrincipal.Cidade.no_localidade,
                                           sg_estado = m.Pessoa.EnderecoPrincipal.Estado.Estado.sg_estado,
                                           dc_num_cep = m.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep,
                                           dc_num_pais = m.Pessoa.EnderecoPrincipal.Pais.Pais.dc_num_pais,
                                           dc_pais = m.Pessoa.EnderecoPrincipal.Pais.Pais.dc_pais,

                                           nm_cpf_cgc = m.Pessoa.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                                    db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == m.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                                       db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == m.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                                            db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == m.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                                                db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == m.Pessoa.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,

                                           dc_num_insc_estadual_emp = m.Empresa.dc_num_insc_estadual,
                                           dc_num_insc_municipal_emp = m.Empresa.dc_num_insc_municipal,

                                           id_regime_tributario = m.TipoNF.id_regime_tributario,
                                           vl_aproximado = m.vl_aproximado,

                                           nm_natureza_pessoa = m.Pessoa.nm_natureza_pessoa,
                                           dc_num_insc_estadual = m.Pessoa.nm_natureza_pessoa == (byte)PessoaSGF.TipoPessoa.JURIDICA ? db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pf => pf.cd_pessoa == m.Pessoa.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_insc_estadual : null,
                                           dc_fone_mail = m.Pessoa.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,

                                           vl_base_calculo_ICMS_nf = m.vl_base_calculo_ICMS_nf,
                                           vl_ICMS_nf = m.vl_ICMS_nf,
                                           vl_desconto = m.vl_desconto,
                                           vl_IPI_nf = m.vl_IPI_nf,
                                           vl_PIS_nf = m.vl_PIS_nf,
                                           vl_COFINS_nf = m.vl_COFINS_nf,

                                           tx_obs_fiscal = m.tx_obs_fiscal
                                       }).ToList().Select(x => new Movimento
                                       {
                                           Empresa = new Escola()
                                           {
                                               EnderecoPrincipal = new EnderecoSGF()
                                               {
                                                   Estado = new LocalidadeSGF()
                                                   {
                                                       Estado = new EstadoSGF()
                                                       {
                                                           sg_estado = x.sg_estado_emp
                                                       }
                                                   },
                                                   Cidade = new LocalidadeSGF()
                                                   {
                                                       nm_municipio = x.nm_municipio_emp,
                                                       no_localidade = x.no_cidade_emp
                                                   },
                                                   Logradouro = new LocalidadeSGF()
                                                   {
                                                       no_localidade = x.no_localidade_emp,
                                                       dc_num_cep = x.dc_num_cep_emp
                                                   },
                                                   dc_num_endereco = x.dc_num_endereco_emp,
                                                   Bairro = new LocalidadeSGF()
                                                   {
                                                       no_localidade = x.no_bairro_emp
                                                   },
                                                   Pais = new LocalidadeSGF()
                                                   {
                                                       Pais = new PaisSGF()
                                                       {
                                                           dc_num_pais = x.dc_num_pais_emp,
                                                           dc_pais = x.dc_pais_emp
                                                       }
                                                   }
                                               },
                                               dc_num_cgc = x.dc_num_cgc,
                                               no_pessoa = x.no_empresa,
                                               dc_reduzido_pessoa = x.dc_reduzido_pessoa,
                                               dc_num_insc_estadual = x.dc_num_insc_estadual_emp,
                                               dc_num_insc_municipal = x.dc_num_insc_municipal_emp
                                           },
                                           vl_aproximado = x.vl_aproximado,
                                           cd_movimento = x.cd_movimento,
                                           TipoNF = new TipoNotaFiscal()
                                           {
                                               dc_natureza_operacao = x.dc_natureza_operacao,
                                               id_regime_tributario = x.id_regime_tributario,
                                               id_natureza_movimento = x.id_natureza_movimento
                                           },
                                           dc_serie_movimento = x.dc_serie_movimento,
                                           nm_movimento = x.nm_movimento,
                                           dt_emissao_movimento = x.dt_emissao_movimento,
                                           dt_mov_movimento = x.dt_mov_movimento,
                                           id_tipo_movimento = x.id_tipo_movimento,
                                           Pessoa = new PessoaSGF()
                                           {
                                               EnderecoPrincipal = new EnderecoSGF()
                                               {
                                                   Logradouro = new LocalidadeSGF()
                                                   {
                                                       no_localidade = x.no_localidade,
                                                       dc_num_cep = x.dc_num_cep
                                                   },
                                                   dc_num_endereco = x.dc_num_endereco,
                                                   Bairro = new LocalidadeSGF()
                                                   {
                                                       no_localidade = x.no_bairro
                                                   },
                                                   Cidade = new LocalidadeSGF()
                                                   {
                                                       nm_municipio = x.nm_municipio,
                                                       no_localidade = x.no_cidade
                                                   },
                                                   Estado = new LocalidadeSGF()
                                                   {
                                                       Estado = new EstadoSGF()
                                                       {
                                                           sg_estado = x.sg_estado
                                                       }
                                                   },
                                                   Pais = new LocalidadeSGF()
                                                   {
                                                       Pais = new PaisSGF()
                                                       {
                                                           dc_num_pais = x.dc_num_pais,
                                                           dc_pais = x.dc_pais
                                                       }
                                                   }
                                               },
                                               no_pessoa = x.no_pessoa,
                                               nm_cpf_cgc = x.nm_cpf_cgc.TrimEnd(),
                                               nm_natureza_pessoa = x.nm_natureza_pessoa,
                                               dc_num_insc_estadual_PJ = x.dc_num_insc_estadual,
                                               Telefone = new TelefoneSGF()
                                               {
                                                   dc_fone_mail = x.dc_fone_mail
                                               }
                                           },
                                           vl_base_calculo_ICMS_nf = x.vl_base_calculo_ICMS_nf,
                                           vl_ICMS_nf = x.vl_ICMS_nf,
                                           vl_desconto = x.vl_desconto,
                                           vl_IPI_nf = x.vl_IPI_nf,
                                           vl_PIS_nf = x.vl_PIS_nf,
                                           vl_COFINS_nf = x.vl_COFINS_nf,
                                           tx_obs_fiscal = x.tx_obs_fiscal
                                       }).FirstOrDefault();

                movimento.ItensMovimento = (from im in db.ItemMovimento
                                            where im.cd_movimento == movimento.cd_movimento
                                            select new
                                            {
                                                cd_item = im.cd_item,
                                                dc_item_movimento = im.dc_item_movimento,
                                                dc_classificacao_fiscal = im.Item.dc_classificacao_fiscal,
                                                nm_cfop = im.CFOP != null ? im.CFOP.nm_cfop : (short)0,
                                                dc_cfop = im.dc_cfop,
                                                dc_sgl_item = im.Item.dc_sgl_item,
                                                //im.Item.cd_integracao,
                                                qt_item_movimento = im.qt_item_movimento,
                                                vl_unitario_item = im.vl_unitario_item,
                                                vl_ICMS_item = im.vl_ICMS_item,
                                                vl_PIS_item = im.vl_PIS_item,
                                                vl_COFINS_item = im.vl_COFINS_item,
                                                vl_IPI_item = im.vl_IPI_item,
                                                nm_situacao_tributaria = im.SituacaoTribICMS.nm_situacao_tributaria,
                                                vl_base_calculo_ICMS_item = im.vl_base_calculo_ICMS_item,
                                                pc_aliquota_ICMS = im.pc_aliquota_ICMS,
                                                nm_situacao_tributariaPIS = im.SituacaoTribPIS.nm_situacao_tributaria,
                                                nm_situacao_tributariaCOFINS = im.SituacaoTribCOFINS.nm_situacao_tributaria,
                                                vl_total_item = im.vl_total_item, //Campo “Total Itens” do movimento
                                                vl_liquido_item = im.vl_liquido_item, //Campo “Total Geral” do movimento
                                                vl_aproximado = im.vl_aproximado
                                            }).ToList().Select(x2 => new ItemMovimento
                                            {
                                                cd_item = x2.cd_item,
                                                dc_item_movimento = x2.dc_item_movimento,
                                                Item = new Item()
                                                {
                                                    dc_classificacao_fiscal = x2.dc_classificacao_fiscal,
                                                    dc_sgl_item = x2.dc_sgl_item//,
                                                    //cd_integracao = x2.cd_integracao
                                                },
                                                CFOP = new CFOP()
                                                {
                                                    nm_cfop = x2.nm_cfop
                                                },
                                                dc_cfop = x2.dc_cfop,
                                                qt_item_movimento = x2.qt_item_movimento,
                                                vl_unitario_item = x2.vl_unitario_item,
                                                vl_ICMS_item = x2.vl_ICMS_item,
                                                vl_PIS_item = x2.vl_PIS_item,
                                                vl_COFINS_item = x2.vl_COFINS_item,
                                                vl_IPI_item = x2.vl_IPI_item,
                                                SituacaoTribICMS = new SituacaoTributaria()
                                                {
                                                    nm_situacao_tributaria = x2.nm_situacao_tributaria
                                                },
                                                vl_base_calculo_ICMS_item = x2.vl_base_calculo_ICMS_item,
                                                pc_aliquota_ICMS = x2.pc_aliquota_ICMS,
                                                SituacaoTribPIS = new SituacaoTributaria()
                                                {
                                                    nm_situacao_tributaria = x2.nm_situacao_tributariaPIS
                                                },
                                                SituacaoTribCOFINS = new SituacaoTributaria()
                                                {
                                                    nm_situacao_tributaria = x2.nm_situacao_tributariaCOFINS
                                                },
                                                vl_total_item = x2.vl_total_item, //Campo “Total Itens” do movimento
                                                vl_liquido_item = x2.vl_liquido_item, //Campo “Total Geral” do movimento
                                                vl_aproximado = x2.vl_aproximado
                                            }).ToList();

                return movimento;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeMovimentoForTit(int cd_pessoa_empresa, List<int> titulos)
        {
            try
            {
                SGFWebContext dbContext = new SGFWebContext();
                int cd_origemTit = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Titulo"].ToString());
                int cd_origemBaixa = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["BaixaTitulo"].ToString());
                bool sql = (from t in db.Movimento
                            where t.cd_pessoa_empresa == cd_pessoa_empresa &&
                            t.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO &&
                            (((t.cd_origem_movimento.HasValue && titulos.Contains(t.cd_origem_movimento.Value)) &&
                              t.id_origem_movimento == cd_origemTit) ||
                            (db.BaixaTitulo.Where(b => titulos.Contains(b.cd_titulo) && t.cd_origem_movimento == b.cd_baixa_titulo && t.id_origem_movimento == cd_origemBaixa).Any()))
                            select t.cd_movimento).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<MovimentoUI> searchMovimentoFK(SearchParameters parametros, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie, int cd_empresa,
                                                       bool emissao, bool movimento, DateTime? dtInicial, DateTime? dtFinal, int natMovto, bool idNf)
        {
            try
            {
                IEntitySorter<MovimentoUI> sorter = EntitySorter<MovimentoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Movimento> sql;


                sql = from m in db.Movimento.AsNoTracking()
                      where
                        m.cd_pessoa_empresa == cd_empresa &&
                        m.id_nf == idNf &&
                        // Para devoluções sem nota somente será possivel para Entradas ou Saídas
                        (idNf == true ? (m.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA || 
                                m.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA ||
                                m.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DESPESA ||
                                m.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO) :
                                (m.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA ||
                                m.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA)) &&
                        (natMovto == 0 || m.id_tipo_movimento == natMovto) &&
                        (m.id_status_nf == (int)Movimento.StatusNFEnum.FECHADO || !idNf) //&&
                        //LBM Para a consulta ficar rápida fazer a regra quando for escolher o item a devolver
                        //(!m.MovimentoDevolver.Any() ||
                        //m.ItensMovimento.Where(x => db.ItemMovimento.Where(imo => imo.Movimento.cd_pessoa_empresa == m.cd_pessoa_empresa &&
                        //                            imo.Movimento.cd_nota_fiscal == m.cd_movimento && 
                        //                            imo.cd_item == x.cd_item).Sum(s => s.qt_item_movimento) < x.qt_item_movimento).Any()
                        //)
                        //LBM ESTAS EXPRESSOES ABAIXO FORAM TROCADAS PELA DE CIMA, QUE TAMBEM FOI ELIMINADA
                         //m.ItensMovimento.Where(x => (!db.ItemMovimento.Where(imo => imo.Movimento.cd_pessoa_empresa == cd_empresa &&
                         //                                                                               imo.Movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO &&
                         //                                                                               imo.Movimento.cd_nota_fiscal == m.cd_movimento &&
                         //                                                                               imo.Movimento.id_status_nf == (int)Movimento.StatusNFEnum.FECHADO &&
                         //                                                                               imo.cd_item == x.cd_item).Any() ||

                         //                             db.ItemMovimento.Where(imo => imo.Movimento.cd_pessoa_empresa == cd_empresa &&
                         //                                                                               imo.Movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO &&
                         //                                                                               imo.Movimento.cd_nota_fiscal == m.cd_movimento &&
                         //                                                                               imo.Movimento.id_status_nf == (int)Movimento.StatusNFEnum.FECHADO &&
                         //                                                                               imo.cd_item == x.cd_item).Sum(s => s.qt_item_movimento) < x.qt_item_movimento)).Any())

                      select m;

                if (cd_pessoa > 0)
                    sql = from da in sql
                          where da.cd_pessoa == cd_pessoa
                          select da;
                if (numero > 0)
                    sql = from da in sql
                          where da.nm_movimento == numero
                          select da;
                if (!string.IsNullOrEmpty(serie))
                    sql = from da in sql
                          where da.dc_serie_movimento == serie
                          select da;

                if (cd_item > 0)
                    sql = from m in sql
                          where (from im in m.ItensMovimento where im.cd_item == cd_item select im.cd_item).Any()
                          select m;

                if (cd_plano_conta > 0)
                    sql = from m in sql
                          where (from im in m.ItensMovimento where im.cd_plano_conta == cd_plano_conta select im.cd_plano_conta).Any()
                          select m;

                if (dtInicial.HasValue)
                {
                    if (emissao)
                        sql = from t in sql
                              where t.dt_emissao_movimento >= dtInicial
                              select t;
                    else
                        sql = from t in sql
                              where t.dt_mov_movimento >= dtInicial
                              select t;
                }

                if (dtFinal.HasValue)
                {
                    if (emissao)
                        sql = from t in sql
                              where t.dt_emissao_movimento <= dtFinal
                              select t;
                    else
                        sql = from t in sql
                              where t.dt_mov_movimento <= dtFinal
                              select t;
                }


                var sqlSearch = (from t in sql
                                 select new MovimentoUI
                                 {
                                     cd_movimento = t.cd_movimento,
                                     dc_politica_comercial = t.PoliticaComercial.dc_politica_comercial,
                                     dc_tipo_financeiro = t.TipoFinanceiro.dc_tipo_financeiro,
                                     no_pessoa = t.Pessoa.no_pessoa,
                                     nm_movimento = t.nm_movimento,
                                     dc_serie_movimento = t.dc_serie_movimento,
                                     dt_emissao_movimento = t.dt_emissao_movimento,
                                     dt_vcto_movimento = t.dt_vcto_movimento,
                                     dt_mov_movimento = t.dt_mov_movimento,
                                     id_tipo_movimento = t.id_tipo_movimento,
                                     id_status_nf = t.id_status_nf,
                                     id_nf = t.id_nf,
                                     pc_acrescimo = t.pc_acrescimo,
                                     vl_acrescimo = t.vl_acrescimo,
                                     pc_desconto = t.pc_desconto,
                                     vl_desconto = t.vl_desconto
                                 });

                sqlSearch = sorter.Sort(sqlSearch);
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sqlSearch = sqlSearch.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sqlSearch;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<MovimentoPerdaMaterialUI> searchMovimentoFKPerdaMaterial(SearchParameters parametros, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie, int cd_empresa,
                                                       bool emissao, bool movimento, DateTime? dtInicial, DateTime? dtFinal, int natMovto, bool idNf, int origem, int? cd_aluno, int? nm_contrato)
        {
            try
            {
                IEntitySorter<MovimentoPerdaMaterialUI> sorter = EntitySorter<MovimentoPerdaMaterialUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Movimento> sql;

                    
                sql = from m in db.Movimento.AsNoTracking()
                      where
                        m.cd_pessoa_empresa == cd_empresa &&
                        m.id_nf == idNf &&
                        m.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA &&
                        (natMovto == 0 || m.id_tipo_movimento == natMovto)  

                      select m;

                if (cd_pessoa > 0)
                    sql = from da in sql
                          where da.cd_pessoa == cd_pessoa
                          select da;
                if (numero > 0)
                    sql = from da in sql
                          where da.nm_movimento == numero
                          select da;
                if (!string.IsNullOrEmpty(serie))
                    sql = from da in sql
                          where da.dc_serie_movimento == serie
                          select da;

                if (cd_item > 0)
                    sql = from m in sql
                          where (from im in m.ItensMovimento where im.cd_item == cd_item select im.cd_item).Any()
                          select m;

                if (cd_plano_conta > 0)
                    sql = from m in sql
                          where (from im in m.ItensMovimento where im.cd_plano_conta == cd_plano_conta select im.cd_plano_conta).Any()
                          select m;

                if (dtInicial.HasValue)
                {
                    if (emissao)
                        sql = from t in sql
                              where t.dt_emissao_movimento >= dtInicial
                              select t;
                    else
                        sql = from t in sql
                              where t.dt_mov_movimento >= dtInicial
                              select t;
                }

                if (dtFinal.HasValue)
                {
                    if (emissao)
                        sql = from t in sql
                              where t.dt_emissao_movimento <= dtFinal
                              select t;
                    else
                        sql = from t in sql
                              where t.dt_mov_movimento <= dtFinal
                              select t;
                }

                if (origem == (int)OrigemPerdaMaterialEnum.ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL)
                {
                    if (cd_aluno > 0)
                    {
                        sql = from t in sql
                              where t.cd_aluno == cd_aluno
                              select t;
                    }

                    if (nm_contrato > 0)
                    {
                        sql = from t in sql
                              from c in db.Contrato
                              where t.cd_origem_movimento == c.cd_contrato &&
                                    c.nm_contrato == nm_contrato
                              select t;
                    }
                }

                if (origem == (int)OrigemPerdaMaterialEnum.ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL)
                {
                    sql = from t in sql
                          where (from pm in db.PerdaMaterial where pm.cd_movimento == t.cd_movimento select pm).Any()
                          select t;
                }

                if (origem == (int)OrigemPerdaMaterialEnum.ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL)
                {
                    sql = from t in sql
                          where t.id_material_didatico == true && 
                                t.id_origem_movimento == 22 &&
                                t.cd_origem_movimento != null &&
                                t.cd_aluno != null &&
                                t.id_venda_futura == false
                          select t;
                }


                var sqlSearch = (from t in sql
                                 select new MovimentoPerdaMaterialUI
                                 {
                                     cd_movimento = t.cd_movimento,
                                     dc_politica_comercial = t.PoliticaComercial.dc_politica_comercial,
                                     dc_tipo_financeiro = t.TipoFinanceiro.dc_tipo_financeiro,
                                     no_pessoa = t.Pessoa.no_pessoa,
                                     nm_movimento = t.nm_movimento,
                                     dc_serie_movimento = t.dc_serie_movimento,
                                     dt_emissao_movimento = t.dt_emissao_movimento,
                                     dt_vcto_movimento = t.dt_vcto_movimento,
                                     dt_mov_movimento = t.dt_mov_movimento,
                                     id_tipo_movimento = t.id_tipo_movimento,
                                     id_status_nf = t.id_status_nf,
                                     id_nf = t.id_nf,
                                     pc_acrescimo = t.pc_acrescimo,
                                     vl_acrescimo = t.vl_acrescimo,
                                     pc_desconto = t.pc_desconto,
                                     vl_desconto = t.vl_desconto,
                                     cd_contrato = t.cd_origem_movimento,
                                     nm_contrato = (from c in db.Contrato where c.cd_contrato == t.cd_origem_movimento && c.cd_pessoa_escola == cd_empresa select c.nm_contrato).FirstOrDefault(),
                                     cd_aluno = t.cd_aluno,
                                     no_aluno = (from al in db.Aluno 
                                                 from pp in db.PessoaSGF
                                                 where 
                                                       al.cd_pessoa_escola == cd_empresa &&
                                                       al.cd_aluno == t.cd_aluno &&  
                                                       al.cd_pessoa_aluno == pp.cd_pessoa
                                                 select pp.no_pessoa).FirstOrDefault()
                                     

                                 });

                sqlSearch = sorter.Sort(sqlSearch);
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sqlSearch = sqlSearch.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sqlSearch;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ItemMovimento> getItensMovimentoByCdMovimento(int cd_movimento)
        {
            try
            {
                var sql = (from t in db.ItemMovimento
                            where t.cd_movimento == cd_movimento 
                            select t);
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<MovimentoUI> searchMovimentoFKVincularMaterial(SearchParameters parametros, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie, int cd_empresa,
                                                       bool emissao, bool movimento, DateTime? dtInicial, DateTime? dtFinal, int natMovto, bool material_didatico_vincular_material, 
                                                       bool nota_fiscal_vincular_material, int cd_curso)
        {
            try
            {
                IEntitySorter<MovimentoUI> sorter = EntitySorter<MovimentoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Movimento> sql;

                sql = from m in db.Movimento.AsNoTracking()
                      where
                        m.cd_pessoa_empresa == cd_empresa && !m.id_venda_futura &&
                        (natMovto == 0 || m.id_tipo_movimento == natMovto) &&
                        m.id_material_didatico == material_didatico_vincular_material &&
                        m.id_origem_movimento == null &&
                        m.id_nf == nota_fiscal_vincular_material
                      select m;

                if (cd_pessoa > 0)
                    sql = from da in sql
                          where da.cd_pessoa == cd_pessoa
                          select da;
                if (numero > 0)
                    sql = from da in sql
                          where da.nm_movimento == numero
                          select da;
                if (!string.IsNullOrEmpty(serie))
                    sql = from da in sql
                          where da.dc_serie_movimento == serie
                          select da;

                if (cd_item > 0)
                    sql = from m in sql
                          where (from im in m.ItensMovimento where im.cd_item == cd_item select im.cd_item).Any()
                          select m;

                if (cd_plano_conta > 0)
                    sql = from m in sql
                          where (from im in m.ItensMovimento where im.cd_plano_conta == cd_plano_conta select im.cd_plano_conta).Any()
                          select m;

                if (cd_curso > 0)
                    sql = from m in sql
                          where (from im in m.ItensMovimento join i in db.Item on im.cd_item equals i.cd_item 
                                 where 
                                 i.cd_tipo_item == (int)TipoItem.TipoItemEnum.MATERIAL_DIDATICO && i.id_material_didatico == true
                                 && i.Cursos.Where(c=>c.cd_curso == cd_curso).Any()
                                 select im.cd_item).Any()
                          select m;

                if (dtInicial.HasValue)
                {
                    if (emissao)
                        sql = from t in sql
                              where t.dt_emissao_movimento >= dtInicial
                              select t;
                    else
                        sql = from t in sql
                              where t.dt_mov_movimento >= dtInicial
                              select t;
                }

                if (dtFinal.HasValue)
                {
                    if (emissao)
                        sql = from t in sql
                              where t.dt_emissao_movimento <= dtFinal
                              select t;
                    else
                        sql = from t in sql
                              where t.dt_mov_movimento <= dtFinal
                              select t;
                }


                var sqlSearch = (from t in sql
                                 select new MovimentoUI
                                 {
                                     cd_movimento = t.cd_movimento,
                                     dc_politica_comercial = t.PoliticaComercial.dc_politica_comercial,
                                     dc_tipo_financeiro = t.TipoFinanceiro.dc_tipo_financeiro,
                                     no_pessoa = t.Pessoa.no_pessoa,
                                     nm_movimento = t.nm_movimento,
                                     dc_serie_movimento = t.dc_serie_movimento,
                                     dt_emissao_movimento = t.dt_emissao_movimento,
                                     dt_vcto_movimento = t.dt_vcto_movimento,
                                     dt_mov_movimento = t.dt_mov_movimento,
                                     id_tipo_movimento = t.id_tipo_movimento,
                                     id_status_nf = t.id_status_nf,
                                     id_nf = t.id_nf,
                                     pc_acrescimo = t.pc_acrescimo,
                                     vl_acrescimo = t.vl_acrescimo,
                                     pc_desconto = t.pc_desconto,
                                     vl_desconto = t.vl_desconto
                                 });

                sqlSearch = sorter.Sort(sqlSearch);
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sqlSearch = sqlSearch.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sqlSearch;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeMovimentoTpNF(int cd_tipo_nota_fiscal)
        {
            try
            {
                bool sql = (from t in db.Movimento
                            where t.cd_tipo_nota_fiscal == cd_tipo_nota_fiscal
                            select t.cd_movimento).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Movimento getRetMovimentoDevolucao(int cd_movimento, int cd_empresa, int id_tipo_movimento)
        {
            try
            {
                Movimento movimento = new Movimento();
                if (id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA ||
                    id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA)
                {
                    movimento = (from t in db.Movimento
                                 where t.cd_pessoa_empresa == cd_empresa && t.cd_movimento == cd_movimento
                                 select new
                                 {
                                     cd_movimento = t.cd_movimento,
                                     cd_pessoa = t.cd_pessoa,
                                     pc_acrescimo = t.pc_acrescimo,
                                     vl_acrescimo = t.vl_acrescimo,
                                     pc_desconto = t.pc_desconto,
                                     vl_desconto = t.vl_desconto,
                                     id_nf_escola = t.id_nf_escola,
                                     vl_base_calculo_ICMS_nf = t.vl_base_calculo_ICMS_nf,
                                     vl_ICMS_nf = t.vl_ICMS_nf,
                                     vl_base_calculo_PIS_nf = t.vl_base_calculo_PIS_nf,
                                     vl_PIS_nf = t.vl_PIS_nf,
                                     vl_base_calculo_COFINS_nf = t.vl_base_calculo_COFINS_nf,
                                     vl_COFINS_nf = t.vl_COFINS_nf,
                                     vl_base_calculo_IPI_nf = t.vl_base_calculo_IPI_nf,
                                     vl_IPI_nf = t.vl_IPI_nf,
                                     no_pessoa = t.Pessoa.no_pessoa,
                                     pc_reduzido_nf = t.TipoNF == null ? 0 : t.TipoNF.pc_reducao,
                                     id_tipo_movimento = t.id_tipo_movimento,
                                     cheque = t.Cheques.FirstOrDefault(),
                                     tx_obs_fiscal = t.tx_obs_fiscal == null ? "" : t.tx_obs_fiscal,
                                     vl_aproximado = t.vl_aproximado,
                                     pc_aliquota_aproximada = t.pc_aliquota_aproximada
                                 }).ToList().Select(x => new Movimento
                                 {
                                     cd_movimento = x.cd_movimento,
                                     cd_pessoa = x.cd_pessoa,
                                     pc_acrescimo = x.pc_acrescimo,
                                     vl_acrescimo = x.vl_acrescimo,
                                     pc_desconto = x.pc_desconto,
                                     vl_desconto = x.vl_desconto,
                                     id_nf_escola = x.id_nf_escola,
                                     vl_base_calculo_ICMS_nf = x.vl_base_calculo_ICMS_nf,
                                     vl_ICMS_nf = x.vl_ICMS_nf,
                                     vl_base_calculo_PIS_nf = x.vl_base_calculo_PIS_nf,
                                     vl_PIS_nf = x.vl_PIS_nf,
                                     vl_base_calculo_COFINS_nf = x.vl_base_calculo_COFINS_nf,
                                     vl_COFINS_nf = x.vl_COFINS_nf,
                                     vl_base_calculo_IPI_nf = x.vl_base_calculo_IPI_nf,
                                     vl_IPI_nf = x.vl_IPI_nf,
                                     no_pessoa = x.no_pessoa,
                                     pc_reduzido_nf = x.pc_reduzido_nf,
                                     id_tipo_movimento = x.id_tipo_movimento,
                                     cheque = x.cheque,
                                     tx_obs_fiscal = x.tx_obs_fiscal,
                                     vl_aproximado = x.vl_aproximado,
                                     pc_aliquota_aproximada = x.pc_aliquota_aproximada
                                 }).FirstOrDefault();
                    if (movimento != null && movimento.cd_movimento > 0)
                        movimento.ItensMovimento = (from im in db.ItemMovimento
                                                    where im.cd_movimento == movimento.cd_movimento &&
                                                     (!db.ItemMovimento.Where(imo => imo.Movimento.cd_pessoa_empresa == cd_empresa &&
                                                                                                        imo.Movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO &&
                                                                                                        imo.Movimento.cd_nota_fiscal == cd_movimento &&
                                                                                                        (imo.Movimento.id_status_nf == (int)Movimento.StatusNFEnum.FECHADO || !imo.Movimento.id_nf) &&
                                                                                                        imo.cd_item == im.cd_item).Any() ||

                                                     db.ItemMovimento.Where(imo => imo.Movimento.cd_pessoa_empresa == cd_empresa &&
                                                                                                        imo.Movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO &&
                                                                                                        imo.Movimento.cd_nota_fiscal == cd_movimento &&
                                                                                                        (imo.Movimento.id_status_nf == (int)Movimento.StatusNFEnum.FECHADO || !imo.Movimento.id_nf) &&
                                                                                                        imo.cd_item == im.cd_item).Sum(s => s.qt_item_movimento) < im.qt_item_movimento)
                                                    select new
                                                    {
                                                        cd_item_movimento = im.cd_item_movimento,
                                                        cd_movimento = im.cd_movimento,
                                                        cd_item = im.cd_item,
                                                        dc_item_movimento = im.dc_item_movimento,
                                                        qt_item_movimento = im.qt_item_movimento,
                                                        qtd_item_devolvido = 0, //!im.Movimento.MovimentoDevolver.Any() ? 0 : db.ItemMovimento.Where(x => x.Movimento.cd_pessoa_empresa == cd_empresa && x.Movimento.cd_nota_fiscal == im.Movimento.cd_movimento && x.cd_item == im.cd_item).Sum(s => s.qt_item_movimento),
                                                        vl_unitario_item = im.vl_unitario_item,
                                                        vl_total_item = im.vl_total_item,
                                                        vl_liquido_item = im.vl_liquido_item,
                                                        vl_acrescimo_item = im.vl_acrescimo_item,
                                                        vl_desconto_item = im.vl_desconto_item,
                                                        cd_plano_conta = im.cd_plano_conta,
                                                        no_plano_conta = im.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta,
                                                        planoSugerido = db.ItemSubgrupo.Where(pc => pc.cd_item == im.cd_item &&
                                                            (pc.id_tipo_movimento + 1) == movimento.id_tipo_movimento &&
                                                             im.Item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cd_empresa).Any()).Any(),
                                                        pc_desconto = im.pc_desconto_item,
                                                        id_material_didatico = im.Item.id_material_didatico,
                                                        //Fiscal
                                                        cd_situacao_tributaria_ICMS = im.cd_situacao_tributaria_ICMS,
                                                        cd_situacao_tributaria_PIS = im.cd_situacao_tributaria_PIS,
                                                        cd_situacao_tributaria_COFINS = im.cd_situacao_tributaria_COFINS,
                                                        vl_base_calculo_ICMS_item = im.vl_base_calculo_ICMS_item,
                                                        pc_aliquota_ICMS = im.pc_aliquota_ICMS,
                                                        vl_ICMS_item = im.vl_ICMS_item,
                                                        vl_base_calculo_PIS_item = im.vl_base_calculo_PIS_item,
                                                        pc_aliquota_PIS = im.pc_aliquota_PIS,
                                                        vl_PIS_item = im.vl_PIS_item,
                                                        vl_base_calculo_COFINS_item = im.vl_base_calculo_COFINS_item,
                                                        pc_aliquota_COFINS = im.pc_aliquota_COFINS,
                                                        vl_COFINS_item = im.vl_COFINS_item,
                                                        vl_base_calculo_IPI_item = im.vl_base_calculo_IPI_item,
                                                        pc_aliquota_IPI = im.pc_aliquota_IPI,
                                                        vl_IPI_item = im.vl_IPI_item,
                                                        cd_cfop_item = im.cd_cfop,
                                                        dc_cfop_item = im.dc_cfop,
                                                        nm_cfop = im.CFOP != null ? im.CFOP.nm_cfop : 0,
                                                        id_nf_item = im.Movimento.id_nf,
                                                        vl_aproximado = im.vl_aproximado,
                                                        pc_aliquota_aproximada = im.pc_aliquota_aproximada,
                                                        cd_tipo_item = im.Item.cd_tipo_item

                                                    }).ToList().Select(x => new ItemMovimento
                                                    {
                                                        cd_item_movimento = x.cd_item_movimento,
                                                        cd_movimento = x.cd_movimento,
                                                        cd_item = x.cd_item,
                                                        dc_item_movimento = x.dc_item_movimento,
                                                        qt_item_movimento = x.qt_item_movimento,
                                                        qtd_item_devolvido = x.qtd_item_devolvido,
                                                        //qt_item_movimento = x.qtd_item_devolvido > 0 ? x.qt_item_movimento - x.qtd_item_devolvido : x.qt_item_movimento,
                                                        qt_item_movimento_dev = x.qt_item_movimento,
                                                        vl_unitario_item = x.vl_unitario_item,
                                                        vl_total_item = x.vl_total_item,
                                                        vl_liquido_item = x.vl_liquido_item,
                                                        vl_acrescimo_item = x.vl_acrescimo_item,
                                                        vl_desconto_item = x.vl_desconto_item,
                                                        cd_plano_conta = x.cd_plano_conta,
                                                        dc_plano_conta = x.no_plano_conta,
                                                        planoSugerido = x.planoSugerido,
                                                        pc_desconto_item = x.pc_desconto,
                                                        id_material_didatico = x.id_material_didatico,
                                                        cd_situacao_tributaria_ICMS = x.cd_situacao_tributaria_ICMS,
                                                        cd_situacao_tributaria_PIS = x.cd_situacao_tributaria_PIS,
                                                        cd_situacao_tributaria_COFINS = x.cd_situacao_tributaria_COFINS,
                                                        vl_base_calculo_ICMS_item = x.vl_base_calculo_ICMS_item,
                                                        pc_aliquota_ICMS = x.pc_aliquota_ICMS,
                                                        vl_ICMS_item = x.vl_ICMS_item,
                                                        vl_base_calculo_PIS_item = x.vl_base_calculo_PIS_item,
                                                        pc_aliquota_PIS = x.pc_aliquota_PIS,
                                                        vl_PIS_item = x.vl_PIS_item,
                                                        vl_base_calculo_COFINS_item = x.vl_base_calculo_COFINS_item,
                                                        pc_aliquota_COFINS = x.pc_aliquota_COFINS,
                                                        vl_COFINS_item = x.vl_COFINS_item,
                                                        vl_base_calculo_IPI_item = x.vl_base_calculo_IPI_item,
                                                        pc_aliquota_IPI = x.pc_aliquota_IPI,
                                                        vl_IPI_item = x.vl_IPI_item,
                                                        cd_cfop = x.cd_cfop_item,
                                                        dc_cfop = x.dc_cfop_item,
                                                        id_nf_item = x.id_nf_item,
                                                        nm_cfop = (short)x.nm_cfop,
                                                        vl_aproximado = x.vl_aproximado,
                                                        pc_aliquota_aproximada = x.pc_aliquota_aproximada,
                                                        cd_tipo_item = x.cd_tipo_item
                                                        //no_item = x.no_item
                                                    }).ToList();
                }

                return movimento;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeItemNoMovimento(int cd_movimento)
        {
            try
            {
                bool sql = (from t in db.ItemMovimento
                            where t.Movimento.cd_movimento == cd_movimento
                            select t.cd_movimento).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeItemZeradoNoMovimento(int cd_movimento)
        {
            try
            {
                bool sql = (from t in db.ItemMovimento
                            where t.Movimento.cd_movimento == cd_movimento
                                && t.vl_total_item <= 0
                            select t.cd_movimento).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeMovimentoComDataSuperior(int id_tipo_movimento, DateTime dta_movimento, int cd_escola)
        {
            try
            {
                bool sql = (from m in db.Movimento
                            where m.dt_mov_movimento > dta_movimento && m.id_tipo_movimento == id_tipo_movimento && m.id_nf && m.cd_pessoa_empresa == cd_escola &&
                            (m.id_status_nf == (int)Movimento.StatusNFEnum.FECHADO || m.id_status_nf == (int)Movimento.StatusNFEnum.CANCELADO)
                            select m.cd_movimento).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeMovimentosAbertosComDataAnterior(int id_tipo_movimento, DateTime dta_movimento, int cd_escola)
        {
            try
            {
                bool sql = (from m in db.Movimento
                            where m.dt_mov_movimento < dta_movimento && m.id_tipo_movimento == id_tipo_movimento && m.id_nf && m.cd_pessoa_empresa == cd_escola &&
                            (m.id_status_nf == (int)Movimento.StatusNFEnum.ABERTO)
                            select m.cd_movimento).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool notaFiscalComNFS(int id_tipo_movimento, int cd_movimento, int cd_escola)
        {
            try
            {
                bool retorno = (from m in db.Movimento
                                where m.cd_pessoa_empresa == cd_escola && m.id_tipo_movimento == id_tipo_movimento &&
                                      m.cd_movimento == cd_movimento && m.nm_nfe == null && m.id_nf
                                select m.cd_movimento).Any();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool notaFiscalComProtocolo(int id_tipo_movimento, int cd_movimento, int cd_escola)
        {
            try
            {
                bool retorno = (from m in db.Movimento
                                where m.cd_pessoa_empresa == cd_escola && m.id_tipo_movimento == id_tipo_movimento &&
                                      m.cd_movimento == cd_movimento && m.ds_protocolo_nfe == null
                                select m.cd_movimento).Any();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool spEnviarMasterSaf(int? cd_movimento)
        {
            try
            {
                var data = context.Database.SqlQuery<int>(@"declare @ret int exec @ret = sp_enviar_mastersaf " + cd_movimento + " select @ret");
                var sql = data.First();
                return sql == 0 ? true : false;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeMovimentoEscola(int cd_movimento, int cd_escola)
        {
            try
            {
                bool retorno = (from l in db.Movimento
                                where l.cd_movimento == cd_movimento && l.cd_pessoa_empresa == cd_escola
                                select l.cd_movimento).Any();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool notaFiscalDevolucaoComProtocolo(int id_tipo_movimento, int cd_movimento, int cd_escola)
        {
            try
            {
                bool retorno = (from m in db.Movimento
                                where m.cd_pessoa_empresa == cd_escola && m.id_tipo_movimento == id_tipo_movimento &&
                                      m.cd_movimento == cd_movimento && m.ds_protocolo_nfe == null && m.id_nf
                                select m.cd_movimento).Any();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool notaFiscalDevolvidaComChaveAcesso(int id_tipo_movimento, int cd_movimento, int cd_escola)
        {
            try
            {
                bool retorno = (from m in db.Movimento
                                where m.cd_pessoa_empresa == cd_escola && m.id_tipo_movimento == id_tipo_movimento &&
                                      m.cd_movimento == cd_movimento && m.dc_key_nfe == null && m.id_nf
                                select m.cd_movimento).Any();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaSeItensNotaDevolucaoExtrapolouNotaOrig(int cd_movimento_dev, int cd_movimento, int cd_escola)
        {
            try
            {
                bool retorno = (from im in db.ItemMovimento
                                where im.cd_movimento == cd_movimento_dev && im.Movimento.cd_pessoa_empresa == cd_escola &&
                                 db.ItemMovimento.Where(imo => imo.Movimento.cd_pessoa_empresa == cd_escola &&
                                                               imo.Movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO &&
                                                               imo.Movimento.cd_nota_fiscal == cd_movimento_dev &&
                                                               (imo.Movimento.id_status_nf == (int)Movimento.StatusNFEnum.FECHADO || imo.Movimento.cd_movimento == cd_movimento) &&
                                                               imo.cd_item == im.cd_item).Sum(s => s.qt_item_movimento) >
                                 db.ItemMovimento.Where(imo => imo.Movimento.cd_pessoa_empresa == cd_escola &&
                                                               imo.Movimento.cd_movimento == cd_movimento_dev &&
                                                               (imo.Movimento.id_status_nf == (int)Movimento.StatusNFEnum.FECHADO || imo.Movimento.cd_movimento == cd_movimento_dev) &&
                                                               imo.cd_item == im.cd_item).Sum(s => s.qt_item_movimento)
                                select im.cd_item_movimento).Any();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<RptContVendasMaterial> getRptContVendasMaterial(int cd_escola, int cd_aluno, int cd_item, DateTime dt_inicial, DateTime dt_final, int cd_turma, bool semmaterial)
        {
            try
            {
                IEnumerable<RptContVendasMaterial> retorno;
                if (!semmaterial)
                {
                     var sql = (from itm in db.ItemMovimento
                                join ct in db.Contrato on itm.Movimento.cd_origem_movimento equals ct.cd_contrato
                                join at in db.AlunoTurma on itm.Movimento.cd_aluno equals at.cd_aluno
                               where itm.Movimento.cd_pessoa_empresa == cd_escola
                               && itm.Movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA
                               && itm.Movimento.id_material_didatico
                               && itm.Movimento.cd_aluno != null
                               && at.cd_contrato == ct.cd_contrato
                               && (cd_turma != 0 || (DbFunctions.TruncateTime(itm.Movimento.dt_emissao_movimento) >= dt_inicial.Date && DbFunctions.TruncateTime(itm.Movimento.dt_emissao_movimento) <= dt_final.Date))
                               && ((cd_turma == 0 || (at.cd_turma == cd_turma
                               && (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                   at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado))))
                               select new {
                                   cd_aluno = itm.Movimento.cd_aluno,
                                   cd_item = itm.cd_item,
                                   no_aluno = itm.Movimento.Aluno.AlunoPessoaFisica.no_pessoa,
                                   no_item = itm.Item.no_item,
                                   no_turma = at.Turma.no_turma,
                                   nm_movimento = itm.Movimento.nm_movimento,
                                   nm_contrato = at.Contrato.nm_contrato
                                   }).Union(
                                (from itm in db.ItemMovimento
                                 join ct in db.Contrato on itm.Movimento.cd_origem_movimento equals ct.cd_contrato
                                 where itm.Movimento.cd_pessoa_empresa == cd_escola
                                 && itm.Movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA
                                 && itm.Movimento.id_material_didatico
                                 && itm.Movimento.cd_aluno != null
                                 && cd_turma == 0
                                 && itm.Movimento.dt_emissao_movimento >= dt_inicial.Date && itm.Movimento.dt_emissao_movimento <= dt_final.Date
                                 //&& T_ITEM_CURSOs.Any(y => y.Cd_item == itm.Cd_item) // && y.Cd_curso == at.T_TURMA.Cd_curso)
                                 && !db.AlunoTurma.Where(at => at.cd_aluno == itm.Movimento.cd_aluno &&
                                                                (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                 at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)).Any()
                                 select new
                                 {
                                     cd_aluno = itm.Movimento.cd_aluno,
                                     cd_item = itm.cd_item,
                                     no_aluno = itm.Movimento.Aluno.AlunoPessoaFisica.no_pessoa,
                                     no_item = itm.Item.no_item,
                                     no_turma = "Sem Turma",
                                     nm_movimento = itm.Movimento.nm_movimento,
                                     nm_contrato = ct.nm_contrato
                                 })

                                );
                    if (cd_aluno > 0)
                    {
                        sql = (from rptvm in sql
                                   where rptvm.cd_aluno == cd_aluno
                                   select rptvm);
                    }

                    if (cd_item > 0)
                    {
                        sql = (from rptvm in sql
                               where rptvm.cd_item == cd_item
                               select rptvm);
                    }

                     retorno = from ret in sql
                                select new RptContVendasMaterial
                                {
                                    no_aluno = ret.no_aluno,
                                    no_turma = ret.no_turma,
                                    no_item = ret.no_item,
                                    nm_contrato = ret.nm_contrato.ToString(),
                                    nm_movimento = ret.nm_movimento == null ? "S/N" : ret.nm_movimento.ToString()
                                };
                }
                else {
	                var sql = (from at in db.AlunoTurma
	                where at.Aluno.cd_pessoa_escola == cd_escola &&
	                (at.cd_turma == cd_turma || (cd_turma == 0 &&
	                (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
	                 at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado))) &&
	                !db.ItemMovimento.Where(itm => itm.Movimento.id_material_didatico 
                                            && itm.Movimento.cd_origem_movimento == at.cd_contrato 
                                            && itm.Movimento.cd_aluno == at.cd_aluno 
                                            && db.Curso.Any(y => y.MateriaisDidaticos.Any(z => z.cd_item == itm.cd_item) && y.cd_curso == at.Turma.cd_curso)).Any() &&

                    !db.ItemMovimento.Where(itm => db.TransferenciaAluno.Where
                                                    (t=> t.cd_aluno_origem == itm.Movimento.cd_aluno &&
                                                     t.cd_aluno_destino == at.cd_aluno &&
                                                    itm.Movimento.id_venda_futura == false &&
                                                    itm.Movimento.id_material_didatico == true &&
                                                    itm.Movimento.id_origem_movimento != null &&
                                                    db.Curso.Any(y => y.MateriaisDidaticos.Any(z => z.cd_item == itm.cd_item) && y.cd_curso == at.Turma.cd_curso)).Any()).Any()

                    select at);
                    if (cd_aluno > 0)
                    {
                        sql = (from rptvm in sql
                                   where rptvm.cd_aluno == cd_aluno
                                   select rptvm);
                    }
                    retorno = (from ret in sql
                    select new {
                        no_turma = ret.Turma.no_turma,
                        no_aluno = ret.Aluno.AlunoPessoaFisica.no_pessoa,
                        no_item = " - ",
                        nm_movimento = " - ",
                        nm_contrato = ret.Contrato.nm_contrato.ToString()
                    }).ToList().Select(x => new RptContVendasMaterial
                    {
                        no_aluno = x.no_aluno,
                        no_item = x.no_item,
                        no_turma = x.no_turma,
                        nm_movimento = x.nm_movimento,
                        nm_contrato = x.nm_contrato.ToString()
                    });
                }

                return retorno;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }

        }
public bool verificaTipoFinanceiroMovimento(int cd_titulo, string dc_tipo_titulo, int cd_pessoa_empresa)
        {
            try
            {
                var TROCA_FINANCEIRA = 129;
                bool existe_cheque = false;
                switch (dc_tipo_titulo)
                {
                    case "TM":
                    case "TA":
                        existe_cheque = (from t in db.Titulo
                                         where t.cd_pessoa_empresa == cd_pessoa_empresa && t.cd_titulo == cd_titulo &&
                                         (db.Contrato.Any(c => c.cd_contrato == t.cd_origem_titulo &&
                                          c.TaxaMatricula.Any(x => x.cd_tipo_financeiro_taxa == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)) ||
                                          t.id_origem_titulo == TROCA_FINANCEIRA && t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)
                                         select t.cd_titulo).Any();
                        break;
                    case "ME":
                    case "MA":
                    case "MM":
                    case "AD":
                    case "AA":
                        existe_cheque = (from t in db.Titulo
                                         where t.cd_pessoa_empresa == cd_pessoa_empresa && t.cd_titulo == cd_titulo &&
                                         (db.Contrato.Any(c => c.cd_contrato == t.cd_origem_titulo && (c.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE || (c.Aditamento.Where(ca => ca.cd_tipo_financeiro  == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)).Any()) ||
                                         (t.id_origem_titulo == TROCA_FINANCEIRA && t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)))
                                         select t.cd_titulo).Any();
                        break;
                    case "NF":
                        existe_cheque = (from t in db.Titulo
                                         where t.cd_pessoa_empresa == cd_pessoa_empresa && t.cd_titulo == cd_titulo &&
                                         (db.Movimento.Any(c => c.cd_movimento == t.cd_origem_titulo && c.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE) ||
                                          t.id_origem_titulo == TROCA_FINANCEIRA && t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)
                                         select t.cd_titulo).Any();
                        break;

                    default:
                        existe_cheque = true;
                        break;
                }

                return existe_cheque;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int findNotaAluno(int cd_aluno, int cd_curso)
        {
            try
            {
                int qt_itens_c = 0;
                int qt_itens_m = 0;
                int retorno = 0;
                List<int> cds = null;
                int cd_aluno_destino = cd_aluno;
                int cd_aluno_aux = cd_aluno_destino;
                int cd_movimento = 0;
                int count = 0;
                int i = 1;

                while ((from t in db.TransferenciaAluno where t.cd_aluno_destino == cd_aluno_destino select t.cd_aluno_destino).Any())
                {
                    count = (from t in db.TransferenciaAluno where t.cd_aluno_destino == cd_aluno_destino select t.cd_aluno_destino).Count();
                    i = 1;
                    if (!(from m in db.Movimento where m.cd_aluno == cd_aluno_destino && m.cd_curso == cd_curso && !m.id_venda_futura && m.id_material_didatico && m.cd_origem_movimento != null select m).Any())
                    {
                        while (i <= count)
                        {
                            if ((from t in db.TransferenciaAluno where t.cd_aluno_destino == cd_aluno_aux && t.cd_aluno_origem != cd_aluno_destino select t).Any())
                                cd_aluno_destino = (from t in db.TransferenciaAluno where t.cd_aluno_destino == cd_aluno_aux && t.cd_aluno_origem != cd_aluno_destino select t.cd_aluno_origem).FirstOrDefault();
                            else break;
                            //if (cd_aluno_destino == 0) break;
                            cd_movimento = (from m in db.Movimento where m.cd_aluno == cd_aluno_destino && m.cd_curso == cd_curso && !m.id_venda_futura && m.id_material_didatico && m.cd_origem_movimento != null select m.cd_movimento).FirstOrDefault();
                            if (cd_movimento > 0)
                                break;
                            i++;
                        }
                        if (cd_movimento == 0)
                            cd_aluno_destino = cd_aluno;
                    }
                    else
                        break;
                    if (cd_aluno_destino == cd_aluno) break;
                    if (cd_movimento > 0) break;
                    if (i == count && cd_movimento == 0)
                        cd_aluno_destino = (from t in db.TransferenciaAluno where t.cd_aluno_destino == cd_aluno_destino && t.cd_aluno_origem != cd_aluno_destino select t.cd_aluno_origem).FirstOrDefault();
                }

                cds = (from m in db.Movimento
                                where m.cd_aluno == cd_aluno && 
                                      m.cd_curso == cd_curso &&
                                      m.id_material_didatico == true &&
                                      !m.id_venda_futura &&
                                      m.id_origem_movimento != null
                                select m.cd_movimento).Union(
                                 (from m in db.Movimento
                                  where m.cd_aluno == cd_aluno_destino &&
                                        m.cd_curso == cd_curso &&
                                        !m.id_venda_futura &&
                                        m.id_material_didatico == true &&
                                        m.id_origem_movimento != null &&
                                        cd_aluno_destino != cd_aluno
                                  select m.cd_movimento)
                                 ).ToList();
                if (cds != null && cds.Count() > 0)
                {
                    qt_itens_c = (from ic in db.ItemCurso where ic.cd_curso == cd_curso select ic).Count();
                    qt_itens_m = (from it in db.ItemMovimento where cds.Contains(it.cd_movimento) && it.Item.id_material_didatico select it).Count();
                    retorno = qt_itens_m < qt_itens_c ? 2 : 1;
                }
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}