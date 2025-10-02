using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using System.Data.SqlClient;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Utils;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class ItemDataAccess : GenericRepository<Item>, IItemDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<ItemUI> getItemSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int? tipoItem, int? cdGrupoItem, int? cdEscola, bool isMasterGeral,
            bool estoque, bool biblioteca, bool comEstoque, int categoria, bool todas_escolas, bool contaSegura)
        {
            int tipo_biblioteca = (int)TipoItem.TipoItemEnum.ITEM_BIBLIOTECA;
            bool idMaterial = false;

            try{
                cdGrupoItem = cdGrupoItem == 0 ? null : cdGrupoItem;

                tipoItem = tipoItem == 0 ? null : tipoItem;
                if (tipoItem < 0)
                {
                    tipoItem = -1 * tipoItem;
                    idMaterial = true;
                }
                
                IEntitySorter<ItemUI> sorter = EntitySorter<ItemUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<ItemUI> sql;
                IQueryable<Item> sql1;

                todas_escolas = todas_escolas && !biblioteca && !estoque;
                
                if(todas_escolas)
                    sql1 = from item in db.Item.AsNoTracking()
                      where (cdGrupoItem == null || item.cd_grupo_estoque == cdGrupoItem) 
                         && (tipoItem == null || item.cd_tipo_item == tipoItem)
                      select item;
                else
                    sql1 = from item in db.Item.AsNoTracking()
                           where (cdGrupoItem == null || item.cd_grupo_estoque == cdGrupoItem)
                              && (tipoItem == null || item.cd_tipo_item == tipoItem)
                              && (item.ItemEscola.Any(e => e.cd_pessoa_escola == cdEscola))
                           select item;


                if(idMaterial)
                    sql1 = from item in sql1
                           where item.id_material_didatico == true
                           select item;

                if (estoque)
                    sql1 = from item in sql1
                           where item.Emprestimos.Where(e => e.cd_pessoa_escola == cdEscola).Any()
                           select item;
                else if(biblioteca)
                    sql1 = from item in sql1
                           where item.TipoItem.cd_tipo_item == tipo_biblioteca
                           select item;

                if(comEstoque)
                    sql1 = from item in sql1
                           where item.ItemEscola.Where(ie => ie.qt_estoque > 0 && ie.cd_pessoa_escola == cdEscola).Any()
                           select item;

                if (ativo != null)
                    sql1 = from item in sql1
                           where (item.id_item_ativo == ativo)
                           select item;
                if (categoria > 0 )
                    sql1 = from item in sql1
                           where item.GrupoEstoque.id_categoria_grupo == categoria
                           select item;


                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql1 = from c in sql1
                               where c.no_item.StartsWith(descricao)
                               select c;
                    else
                        sql1 = from c in sql1
                               where c.no_item.Contains(descricao)
                               select c;


                sql = from item in sql1
                      select new ItemUI {
                              cd_grupo_estoque = item.cd_grupo_estoque,
                              cd_item = item.cd_item,
                              cd_origem_fiscal = item.cd_origem_fiscal,
                              cd_integracao = item.cd_integracao,
                              cd_tipo_item = item.cd_tipo_item,
                              dc_classificacao_fiscal = item.dc_classificacao_fiscal,
                              dc_classificacao = item.dc_classificacao,
                              dc_codigo_barra = item.dc_codigo_barra,
                              dc_sgl_item = item.dc_sgl_item,
                              id_item_ativo = item.id_item_ativo,
                              no_item = item.no_item,
                              pc_aliquota_icms = item.pc_aliquota_icms,
                              pc_aliquota_iss = item.pc_aliquota_iss,
                              qt_estoque = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.qt_estoque).FirstOrDefault(),
                              vl_custo = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.vl_custo).FirstOrDefault(),
                              vl_item = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.vl_item).FirstOrDefault(),
                              no_autor = item.Biblioteca.no_autor,
                              dc_titulo = item.Biblioteca.dc_titulo,
                              dc_assunto = item.Biblioteca.dc_assunto,
                              dc_local = item.Biblioteca.dc_local,
                              dc_tipo_item = item.TipoItem.dc_tipo_item,
                              no_grupo_estoque = item.GrupoEstoque.no_grupo_estoque,
                              id_categoria_grupo = item.GrupoEstoque.id_categoria_grupo,
                              cd_plano_conta  = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.PlanoConta.cd_plano_conta).FirstOrDefault(),
                              desc_plano_conta = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta).FirstOrDefault(),
                              cd_subgrupo_conta = item.cd_subgrupo_conta,
                              cd_subgrupo_conta_2 = item.cd_subgrupo_conta_2,
                              desc_subgrupo_conta = item.SubgrupoContas.no_subgrupo_conta,
                              desc_subgrupo_conta_2 = item.SubGrupoContas2.no_subgrupo_conta,
                              cd_cest = item.cd_cest,
                              id_material_didatico = item.id_material_didatico,
                              id_voucher_carga = item.id_voucher_carga,
                              qt_horas_carga = item.qt_horas_carga
                              //itemEscolas = item.ItemEscola (não pode colocar todos os itens de escola, por questão de segurança, uma escola não pode ver itens da outra).
                          };

                sql = sorter.Sort(sql);

                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ItemUI> getItemSearchAlunosemAula(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int? tipoItem, int? cdGrupoItem, int? cdEscola, bool isMasterGeral,
            bool estoque, bool biblioteca, bool comEstoque, int categoria, bool todas_escolas, bool contaSegura)
        {
            int tipo_biblioteca = (int)TipoItem.TipoItemEnum.ITEM_BIBLIOTECA;
            bool idMaterial = false;

            try
            {
                cdGrupoItem = cdGrupoItem == 0 ? null : cdGrupoItem;

                tipoItem = tipoItem == 0 ? null : tipoItem;
                if (tipoItem < 0)
                {
                    tipoItem = -1 * tipoItem;
                    idMaterial = true;
                }

                IEntitySorter<ItemUI> sorter = EntitySorter<ItemUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<ItemUI> sql;
                IQueryable<Item> sql1;

                todas_escolas = todas_escolas && !biblioteca && !estoque;

                if (todas_escolas)
                    sql1 = from item in db.Item.AsNoTracking()
                           where (cdGrupoItem == null || item.cd_grupo_estoque == cdGrupoItem)
                              && (tipoItem == null || item.cd_tipo_item == tipoItem)
                           select item;
                else
                    sql1 = from item in db.Item.AsNoTracking()
                           where (cdGrupoItem == null || item.cd_grupo_estoque == cdGrupoItem)
                              && (tipoItem == null || item.cd_tipo_item == tipoItem)
                              && (item.ItemEscola.Any(e => e.cd_pessoa_escola == cdEscola))
                           select item;


                if (idMaterial)
                    sql1 = from item in sql1
                           where item.id_material_didatico == true &&
                           (from ta in db.vi_aluno_sem_aula where ta.cd_item == item.cd_item select ta).Any()
                           select item;

                if (estoque)
                    sql1 = from item in sql1
                           where item.Emprestimos.Where(e => e.cd_pessoa_escola == cdEscola).Any()
                           select item;
                else if (biblioteca)
                    sql1 = from item in sql1
                           where item.TipoItem.cd_tipo_item == tipo_biblioteca
                           select item;

                if (comEstoque)
                    sql1 = from item in sql1
                           where item.ItemEscola.Where(ie => ie.qt_estoque > 0 && ie.cd_pessoa_escola == cdEscola).Any()
                           select item;

                if (ativo != null)
                    sql1 = from item in sql1
                           where (item.id_item_ativo == ativo)
                           select item;
                if (categoria > 0)
                    sql1 = from item in sql1
                           where item.GrupoEstoque.id_categoria_grupo == categoria
                           select item;


                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql1 = from c in sql1
                               where c.no_item.StartsWith(descricao)
                               select c;
                    else
                        sql1 = from c in sql1
                               where c.no_item.Contains(descricao)
                               select c;


                sql = from item in sql1
                      select new ItemUI
                      {
                          cd_grupo_estoque = item.cd_grupo_estoque,
                          cd_item = item.cd_item,
                          cd_origem_fiscal = item.cd_origem_fiscal,
                          cd_integracao = item.cd_integracao,
                          cd_tipo_item = item.cd_tipo_item,
                          dc_classificacao_fiscal = item.dc_classificacao_fiscal,
                          dc_classificacao = item.dc_classificacao,
                          dc_codigo_barra = item.dc_codigo_barra,
                          dc_sgl_item = item.dc_sgl_item,
                          id_item_ativo = item.id_item_ativo,
                          no_item = item.no_item,
                          pc_aliquota_icms = item.pc_aliquota_icms,
                          pc_aliquota_iss = item.pc_aliquota_iss,
                          qt_estoque = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.qt_estoque).FirstOrDefault(),
                          vl_custo = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.vl_custo).FirstOrDefault(),
                          vl_item = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.vl_item).FirstOrDefault(),
                          no_autor = item.Biblioteca.no_autor,
                          dc_titulo = item.Biblioteca.dc_titulo,
                          dc_assunto = item.Biblioteca.dc_assunto,
                          dc_local = item.Biblioteca.dc_local,
                          dc_tipo_item = item.TipoItem.dc_tipo_item,
                          no_grupo_estoque = item.GrupoEstoque.no_grupo_estoque,
                          id_categoria_grupo = item.GrupoEstoque.id_categoria_grupo,
                          cd_plano_conta = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.PlanoConta.cd_plano_conta).FirstOrDefault(),
                          desc_plano_conta = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta).FirstOrDefault(),
                          cd_subgrupo_conta = item.cd_subgrupo_conta,
                          cd_subgrupo_conta_2 = item.cd_subgrupo_conta_2,
                          desc_subgrupo_conta = item.SubgrupoContas.no_subgrupo_conta,
                          desc_subgrupo_conta_2 = item.SubGrupoContas2.no_subgrupo_conta,
                          cd_cest = item.cd_cest,
                          id_material_didatico = item.id_material_didatico,
                          id_voucher_carga = item.id_voucher_carga,
                          qt_horas_carga = item.qt_horas_carga
                          //itemEscolas = item.ItemEscola (não pode colocar todos os itens de escola, por questão de segurança, uma escola não pode ver itens da outra).
                      };

                sql = sorter.Sort(sql);

                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<ItemKitUI> getKitSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int? tipoItem, int? cdGrupoItem, int? cdEscola, bool isMasterGeral,
            bool estoque, bool biblioteca, bool comEstoque, int categoria, bool todas_escolas, bool contaSegura)
        {
            int tipo_biblioteca = (int)TipoItem.TipoItemEnum.ITEM_BIBLIOTECA;

            try
            {
                cdGrupoItem = cdGrupoItem == 0 ? null : cdGrupoItem;

                tipoItem = tipoItem == 0 ? null : tipoItem;

                IEntitySorter<ItemKitUI> sorter = EntitySorter<ItemKitUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<ItemKitUI> sql;
                IQueryable<Item> sql1;

                todas_escolas = todas_escolas && !biblioteca && !estoque;

                if (todas_escolas)
                    sql1 = from item in db.Item.AsNoTracking()
                           where (cdGrupoItem == null || item.cd_grupo_estoque == cdGrupoItem)
                              && (tipoItem == null || item.cd_tipo_item == tipoItem)
                           select item;
                else
                    sql1 = from item in db.Item.AsNoTracking()
                           where (cdGrupoItem == null || item.cd_grupo_estoque == cdGrupoItem)
                              && (tipoItem == null || item.cd_tipo_item == tipoItem)
                              && (item.ItemEscola.Any(e => e.cd_pessoa_escola == cdEscola))
                           select item;


                if (estoque)
                    sql1 = from item in sql1
                           where item.Emprestimos.Where(e => e.cd_pessoa_escola == cdEscola).Any()
                           select item;
                else if (biblioteca)
                    sql1 = from item in sql1
                           where item.TipoItem.cd_tipo_item == tipo_biblioteca
                           select item;

                if (comEstoque)
                    sql1 = from item in sql1
                           where item.ItemEscola.Where(ie => ie.qt_estoque > 0 && ie.cd_pessoa_escola == cdEscola).Any()
                           select item;

                if (ativo != null)
                    sql1 = from item in sql1
                           where (item.id_item_ativo == ativo)
                           select item;
                if (categoria > 0)
                    sql1 = from item in sql1
                           where item.GrupoEstoque.id_categoria_grupo == categoria
                           select item;


                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql1 = from c in sql1
                               where c.no_item.StartsWith(descricao)
                               select c;
                    else
                        sql1 = from c in sql1
                               where c.no_item.Contains(descricao)
                               select c;


                sql = from item in sql1
                      select new ItemKitUI
                      {
                          cd_grupo_estoque = item.cd_grupo_estoque,
                          cd_item = item.cd_item,
                          cd_origem_fiscal = item.cd_origem_fiscal,
                          cd_integracao = item.cd_integracao,
                          cd_tipo_item = item.cd_tipo_item,
                          dc_classificacao_fiscal = item.dc_classificacao_fiscal,
                          dc_classificacao = item.dc_classificacao,
                          dc_codigo_barra = item.dc_codigo_barra,
                          dc_sgl_item = item.dc_sgl_item,
                          id_item_ativo = item.id_item_ativo,
                          no_item = item.no_item,
                          pc_aliquota_icms = item.pc_aliquota_icms,
                          pc_aliquota_iss = item.pc_aliquota_iss,
                          qt_estoque = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.qt_estoque).FirstOrDefault(),
                          vl_custo = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.vl_custo).FirstOrDefault(),
                          vl_item = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.vl_item).FirstOrDefault(),
                          no_autor = item.Biblioteca.no_autor,
                          dc_titulo = item.Biblioteca.dc_titulo,
                          dc_assunto = item.Biblioteca.dc_assunto,
                          dc_local = item.Biblioteca.dc_local,
                          dc_tipo_item = item.TipoItem.dc_tipo_item,
                          no_grupo_estoque = item.GrupoEstoque.no_grupo_estoque,
                          id_categoria_grupo = item.GrupoEstoque.id_categoria_grupo,
                          cd_plano_conta = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.PlanoConta.cd_plano_conta).FirstOrDefault(),
                          desc_plano_conta = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta).FirstOrDefault(),
                          cd_subgrupo_conta = item.cd_subgrupo_conta,
                          cd_subgrupo_conta_2 = item.cd_subgrupo_conta_2,
                          desc_subgrupo_conta = item.SubgrupoContas.no_subgrupo_conta,
                          desc_subgrupo_conta_2 = item.SubGrupoContas2.no_subgrupo_conta,
                          cd_cest = item.cd_cest
                          //itemEscolas = item.ItemEscola (não pode colocar todos os itens de escola, por questão de segurança, uma escola não pode ver itens da outra).
                      };

                sql = sorter.Sort(sql);

                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        //public string getPlanoContaByItem(int cd_item) { 
        //     try{
        //         string planoConta = (from itemPlano in db.Item.Include(pp => pp.PlanoConta.PlanoContaSubgrupo).Include(p => p.PlanoConta)
        //                             where itemPlano.cd_item == cd_item
        //                             select itemPlano.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta).FirstOrDefault();
        //         return planoConta;
        //     }
        //     catch (Exception exe)
        //     {
        //         throw new DataAccessException(exe);
        //     }
        //}

        public List<KitUI> getItensKit(int idKit)
        {
            try
            {
                var sql = (from itemKit in db.ItemKit
                          join item in db.Item on itemKit.cd_item equals item.cd_item 
                          where itemKit.cd_item_kit == idKit
                          select new KitUI
                          {
                              cd_item = itemKit.cd_item,
                              no_item = item.no_item,
                              id_item_ativo = item.id_item_ativo,
                              cd_iitem_kit = itemKit.cd_iitem_kit,
                              qt_item_kit = itemKit.qt_item_kit
                          }).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ItemUI> getItemCurso(int cdCurso, int? cdEscola)
        {
            try{
                var sqlInicial = from ic in db.ItemCurso
                                 join it in db.Item on ic.cd_item equals it.cd_item
                                 orderby it.no_item
                                 where ic.cd_curso == cdCurso
                                 select new
                                 {
                                     cd_item_curso = ic.cd_item_curso,
                                     cd_item = ic.cd_item,
                                     cd_curso = ic.cd_curso,
                                     id_ppt = ic.id_ppt,
                                     cd_grupo_estoque = it.cd_grupo_estoque,
                                     cd_origem_fiscal = it.cd_origem_fiscal,
                                     cd_integracao = it.cd_integracao,
                                     cd_tipo_item = it.cd_tipo_item,
                                     dc_classificacao_fiscal = it.dc_classificacao_fiscal,
                                     dc_codigo_barra = it.dc_codigo_barra,
                                     dc_sgl_item = it.dc_sgl_item,
                                     id_item_ativo = it.id_item_ativo,
                                     no_item = it.no_item,
                                     no_autor = it.Biblioteca.no_autor,
                                     dc_titulo = it.Biblioteca.dc_titulo,
                                     dc_assunto = it.Biblioteca.dc_assunto,
                                     dc_local = it.Biblioteca.dc_local,
                                     dc_tipo_item = it.TipoItem.dc_tipo_item,
                                     no_grupo_estoque = it.GrupoEstoque.no_grupo_estoque,
                                     id_material_didatico = it.id_material_didatico,
                                     id_voucher_carga = it.id_voucher_carga,
                                     qt_horas_carga = it.qt_horas_carga,
                                     ItemEscola = it.ItemEscola
                                 };

                if (cdEscola.HasValue)
                    sqlInicial = from item in sqlInicial
                                 where item.ItemEscola.Where(e => e.cd_pessoa_escola == cdEscola).Any()
                                 select item;

                IQueryable<ItemUI> sql;
                sql = from item in sqlInicial
                      select new ItemUI
                      {
                          cd_grupo_estoque = item.cd_grupo_estoque,
                          cd_item = item.cd_item,
                          cd_origem_fiscal = item.cd_origem_fiscal,
                          cd_integracao = item.cd_integracao,
                          cd_tipo_item = item.cd_tipo_item,
                          dc_classificacao_fiscal = item.dc_classificacao_fiscal,
                          dc_codigo_barra = item.dc_codigo_barra,
                          dc_sgl_item = item.dc_sgl_item,
                          id_item_ativo = item.id_item_ativo,
                          no_item = item.no_item,
                          no_autor = item.no_autor,
                          dc_titulo = item.dc_titulo,
                          dc_assunto = item.dc_assunto,
                          dc_local = item.dc_local,
                          dc_tipo_item = item.dc_tipo_item,
                          no_grupo_estoque = item.no_grupo_estoque,
                          id_material_didatico = item.id_material_didatico,
                          id_voucher_carga = item.id_voucher_carga,
                          qt_horas_carga = item.qt_horas_carga,
                          id_ppt = item.id_ppt,
                          cd_curso = item.cd_curso,
                          cd_item_curso = item.cd_item_curso
                      };

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Item> getItensByIds(List<Item> itens) {
            try{
                List<int> idsItens = new List<int>();

                for(int i = 0; i < itens.Count; i++)
                    idsItens.Add(itens[i].cd_item);
                int[] ids = idsItens.ToArray<int>();
                var query = from i in db.Item
                                where ids.Contains(i.cd_item)
                                select i;

                return query;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteItemByCurso(int cd_curso) {
            try{
                int retorno = db.Database.ExecuteSqlCommand("delete from t_item_curso where cd_curso = @cd_curso", new SqlParameter("cd_curso", cd_curso));

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteAllItem(List<Item> itens)
        {
            try{
                string stritem = "";
                if (itens != null && itens.Count > 0)
                    foreach (Item item in itens)
                        stritem += item.cd_item + ",";
                if (stritem.Length > 0)
                    stritem = stritem.Substring(0, stritem.Length - 1);
                db.Database.ExecuteSqlCommand("delete from t_biblioteca where cd_item in(" + stritem + ")");
                int retorno = db.Database.ExecuteSqlCommand("delete from t_item where cd_item in(" + stritem + ")"); 

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Item getItemEdit(int cd_item) {
            try
            {
                Item sql = (from i in db.Item
                            where i.cd_item == cd_item
                            select new
                            {
                                cd_item = i.cd_item,
                                cd_tipo_item = i.cd_tipo_item,
                                cd_grupo_estoque = i.cd_grupo_estoque,
                                no_item = i.no_item,
                                id_item_ativo = i.id_item_ativo,
                                dc_sgl_item = i.dc_sgl_item,
                                cd_origem_fiscal = i.cd_origem_fiscal,
                                dc_classificacao_fiscal = i.dc_classificacao_fiscal,
                                pc_aliquota_icms = i.pc_aliquota_icms,
                                pc_aliquota_iss = i.pc_aliquota_iss,
                                cd_integracao = i.cd_integracao,
                                dc_codigo_barra = i.dc_codigo_barra,
                                ItemEscola = i.ItemEscola,
                                GrupoEstoque = i.GrupoEstoque,
                                cd_subgrupo_conta = i.cd_subgrupo_conta,
                                cd_subgrupo_conta_2 = i.cd_subgrupo_conta_2,
                                desc_subgrupo_conta = i.SubgrupoContas.no_subgrupo_conta,
                                desc_subgrupo_conta_2 = i.SubGrupoContas2.no_subgrupo_conta,
                                id_categoria_grupo = i.GrupoEstoque.id_categoria_grupo,
                                ItemSubgrupos = i.ItemSubgrupos,
                                ItemKit = i.ItemKit,
                                id_material_didatico = i.id_material_didatico,
                                id_voucher_carga = i.id_voucher_carga,
                                qt_horas_carga = i.qt_horas_carga
                            }).ToList().Select(x => new Item
                            {
                                cd_item = x.cd_item,
                                cd_tipo_item = x.cd_tipo_item,
                                cd_grupo_estoque = x.cd_grupo_estoque,
                                no_item = x.no_item,
                                id_item_ativo = x.id_item_ativo,
                                dc_sgl_item = x.dc_sgl_item,
                                cd_origem_fiscal = x.cd_origem_fiscal,
                                dc_classificacao_fiscal = x.dc_classificacao_fiscal,
                                pc_aliquota_icms = x.pc_aliquota_icms,
                                pc_aliquota_iss = x.pc_aliquota_iss,
                                cd_integracao = x.cd_integracao.HasValue ? x.cd_integracao : null,
                                dc_codigo_barra = x.dc_codigo_barra,
                                cd_subgrupo_conta = x.cd_subgrupo_conta,
                                cd_subgrupo_conta_2 = x.cd_subgrupo_conta_2,
                                id_categoria_grupo = x.id_categoria_grupo,
                                ItemEscola = x.ItemEscola,
                                GrupoEstoque = x.GrupoEstoque,
                                ItemSubgrupos = x.ItemSubgrupos,
                                ItemKit = x.ItemKit,
                                id_material_didatico = x.id_material_didatico,
                                id_voucher_carga = x.id_voucher_carga,
                                qt_horas_carga = x.qt_horas_carga
                            }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public ItemUI getItemUIbyId(int cd_item, int cdEscola){
            try
            {

                var sql = (from item in db.Item
                          where item.cd_item == cd_item
                           select new
                           {
                               cd_item = item.cd_item,
                               cd_integracao = item.cd_integracao,
                               no_item = item.no_item,
                               pc_aliquota_icms = item.pc_aliquota_icms,
                               pc_aliquota_iss = item.pc_aliquota_iss,
                               cd_tipo_item = item.cd_tipo_item,
                               dc_tipo_item = item.TipoItem.dc_tipo_item,
                               cd_grupo_estoque = item.cd_grupo_estoque,
                               dc_codigo_barra = item.dc_codigo_barra,
                               id_item_ativo = item.id_item_ativo,
                               dc_sgl_item = item.dc_sgl_item,
                               dc_classificacao_fiscal = item.dc_classificacao_fiscal,
                               qt_estoque = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(i => i.qt_estoque).FirstOrDefault(),
                               vl_custo = item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).Select(i => i.vl_custo).FirstOrDefault(),
                               vl_item = item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).Select(i => i.vl_item).FirstOrDefault(),
                               dc_assunto = item.Biblioteca.dc_assunto,
                               dc_local = item.Biblioteca.dc_local,
                               dc_titulo = item.Biblioteca.dc_titulo,
                               no_autor = item.Biblioteca.no_autor,
                               no_grupo_estoque = item.GrupoEstoque.no_grupo_estoque,
                               id_categoria_grupo = item.GrupoEstoque != null && item.GrupoEstoque.cd_grupo_estoque > 0 ? item.GrupoEstoque.id_categoria_grupo : 0,
                               cd_plano_conta  = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.PlanoConta.cd_plano_conta).FirstOrDefault(),
                               desc_plano_conta = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta).FirstOrDefault(),
                               cd_subgrupo_conta = item.cd_subgrupo_conta,
                               cd_subgrupo_conta_2 = item.cd_subgrupo_conta_2,
                               desc_subgrupo_conta = item.SubgrupoContas.no_subgrupo_conta,
                               desc_subgrupo_conta_2 = item.SubGrupoContas2.no_subgrupo_conta,
                               cd_cest = item.cd_cest,
                               dc_classificacao = item.dc_classificacao,
                               id_material_didatico = item.id_material_didatico,
                               id_voucher_carga = item.id_voucher_carga,
                               qt_horas_carga = item.qt_horas_carga
                           }).ToList().Select(x => new ItemUI
                           {
                               cd_item = x.cd_item,
                               cd_integracao = x.cd_integracao,
                               no_item = x.no_item,
                               pc_aliquota_icms = x.pc_aliquota_icms,
                               pc_aliquota_iss = x.pc_aliquota_iss,
                               cd_tipo_item = x.cd_tipo_item,
                               dc_tipo_item = x.dc_tipo_item,
                               cd_grupo_estoque = x.cd_grupo_estoque,
                               dc_codigo_barra = x.dc_codigo_barra,
                               id_item_ativo = x.id_item_ativo,
                               dc_sgl_item = x.dc_sgl_item,
                               dc_classificacao_fiscal = x.dc_classificacao_fiscal,
                               qt_estoque = x.qt_estoque,
                               vl_custo = x.vl_custo,
                               vl_item = x.vl_item,
                               dc_assunto = x.dc_assunto,
                               dc_local = x.dc_local,
                               dc_titulo = x.dc_titulo,
                               no_autor = x.no_autor,
                               no_grupo_estoque = x.no_grupo_estoque,
                               id_categoria_grupo = x.id_categoria_grupo,
                               cd_plano_conta = x.cd_plano_conta,
                               desc_plano_conta = x.desc_plano_conta,
                               cd_subgrupo_conta = x.cd_subgrupo_conta,
                               cd_subgrupo_conta_2 = x.cd_subgrupo_conta_2,
                               desc_subgrupo_conta = x.desc_subgrupo_conta,
                               desc_subgrupo_conta_2 = x.desc_subgrupo_conta_2,
                               cd_cest = x.cd_cest,
                               dc_classificacao = x.dc_classificacao,
                               id_material_didatico = x.id_material_didatico,
                               id_voucher_carga = x.id_voucher_carga,
                               qt_horas_carga = x.qt_horas_carga
                           }).FirstOrDefault();

                      return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ItemUI> getItemSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int tipoItem, int cdGrupoItem, int cdEscola, bool comEstoque, int categoria)
        {
            try
            {

                IEntitySorter<ItemUI> sorter = EntitySorter<ItemUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<ItemUI> sql;
                IQueryable<Item> sql1;

                sql1 = from item in db.Item.AsNoTracking()
                       where item.ItemEscola.Any(ie => ie.cd_pessoa_escola == cdEscola)
                       select item;

                if (tipoItem > 0)
                    sql1 = from item in sql1
                           where item.TipoItem.cd_tipo_item == tipoItem
                           select item;

                if (cdGrupoItem > 0)
                    sql1 = from item in sql1
                           where item.cd_grupo_estoque == cdGrupoItem
                           select item;

                if (comEstoque)
                    sql1 = from item in sql1
                           where item.ItemEscola.Where(ie => ie.qt_estoque > 0 && ie.cd_pessoa_escola == cdEscola).Any()
                           select item;

                if (ativo != null)
                    sql1 = from item in sql1
                           where (item.id_item_ativo == ativo)
                           select item;

                if (categoria > 0)
                    sql1 = from item in sql1
                           where item.GrupoEstoque.id_categoria_grupo == categoria
                           select item;


                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql1 = from c in sql1
                               where c.no_item.StartsWith(descricao)
                               select c;
                    else
                        sql1 = from c in sql1
                               where c.no_item.Contains(descricao)
                               select c;

                sql = from item in sql1
                      select new ItemUI
                      {
                          cd_grupo_estoque = item.cd_grupo_estoque,
                          cd_item = item.cd_item,
                          cd_origem_fiscal = item.cd_origem_fiscal,
                          cd_integracao = item.cd_integracao,
                          cd_tipo_item = item.cd_tipo_item,
                          dc_classificacao_fiscal = item.dc_classificacao_fiscal,
                          dc_codigo_barra = item.dc_codigo_barra,
                          dc_sgl_item = item.dc_sgl_item,
                          id_item_ativo = item.id_item_ativo,
                          no_item = item.no_item,
                          pc_aliquota_icms = item.pc_aliquota_icms,
                          pc_aliquota_iss = item.pc_aliquota_iss,
                          qt_estoque = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.qt_estoque).FirstOrDefault(),
                          vl_custo = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.vl_custo).FirstOrDefault(),
                          vl_item = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Select(ie => ie.vl_item).FirstOrDefault(),
                          no_autor = item.Biblioteca.no_autor,
                          dc_titulo = item.Biblioteca.dc_titulo,
                          dc_assunto = item.Biblioteca.dc_assunto,
                          dc_local = item.Biblioteca.dc_local,
                          dc_tipo_item = item.TipoItem.dc_tipo_item,
                          no_grupo_estoque = item.GrupoEstoque.no_grupo_estoque,
                          id_categoria_grupo = item.GrupoEstoque.id_categoria_grupo,
                          id_material_didatico = item.id_material_didatico,
                          id_voucher_carga = item.id_voucher_carga,
                          qt_horas_carga = item.qt_horas_carga
                      };

                sql = sorter.Sort(sql);

                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ItemUI> getItemSearchEstoque(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int tipoItem, int cdGrupoItem, int cdEscola, bool comEstoque,
                                                        List<int> cdItens, bool isMaster, int ano, int mes)
        {
            try
            {

                IEntitySorter<ItemUI> sorter = EntitySorter<ItemUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<ItemUI> sql;
                IQueryable<Item> sql1;

                sql1 = from item in db.Item.AsNoTracking()
                       where item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cdEscola).Any()
                       select item;

                if (isMaster)
                    sql1 = from item in sql1
                           where item.GrupoEstoque.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PRIVADA
                           select item;

                if (cdItens.Count() > 0)
                    sql1 = from item in sql1
                           where !cdItens.Contains(item.cd_item)
                           select item;

                if (tipoItem > 0)
                    sql1 = from item in sql1
                           where item.TipoItem.cd_tipo_item == tipoItem
                           select item;

                if (tipoItem < 0)
                    sql1 = from item in sql1
                           where item.TipoItem.id_movimentar_estoque
                           select item;

                if (cdGrupoItem > 0)
                    sql1 = from item in sql1
                           where item.cd_grupo_estoque == cdGrupoItem
                           select item;

                if (comEstoque)
                    sql1 = from item in sql1
                           where item.ItemEscola.Where(ie => ie.qt_estoque > 0 && ie.cd_pessoa_escola == cdEscola).Any()
                           select item;

                if (ativo != null)
                    sql1 = from item in sql1
                           where (item.id_item_ativo == ativo)
                           select item;


                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql1 = from c in sql1
                               where c.no_item.StartsWith(descricao)
                               select c;
                    else
                        sql1 = from c in sql1
                               where c.no_item.Contains(descricao)
                               select c;

                if (ano > 0 && mes > 0)
                    sql1 = from itm in sql1
                           where itm.SaldosItem.Where(x => x.Fechamentos.cd_pessoa_empresa == cdEscola && x.Fechamentos.nm_ano_fechamento == ano && x.Fechamentos.nm_mes_fechamento == mes).Any()
                           select itm;

                sql = from item in sql1
                      select new ItemUI
                      {
                          cd_item = item.cd_item,
                          cd_tipo_item = item.cd_tipo_item,
                          id_item_ativo = item.id_item_ativo,
                          no_item = item.no_item,
                          qt_estoque = item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).FirstOrDefault().qt_estoque,
                          dc_tipo_item = item.TipoItem.dc_tipo_item,
                          cd_grupo_estoque = item.cd_grupo_estoque,
                          no_grupo_estoque = item.GrupoEstoque.no_grupo_estoque,
                          vl_custo = item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).FirstOrDefault().vl_custo,
                          vl_item = item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).FirstOrDefault().vl_item,
                          id_movto_estoque = item.TipoItem.id_movimentar_estoque,
                          id_material_didatico = item.id_material_didatico,
                          id_voucher_carga = item.id_voucher_carga,
                          qt_horas_carga = item.qt_horas_carga
                      };

                sql = sorter.Sort(sql);

                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<RptItemFechamento> rptItemWithSaldoItem(int cd_pessoa_escola, int cd_item, int cd_grupo, byte cd_tipo, int ano, int mes)
        {
            try
            {
                IQueryable<Item> sql1;

                sql1 = from item in db.Item
                       select item;

                if (cd_item > 0)
                    sql1 = from item in sql1
                           where item.cd_item == cd_item
                           select item;

                if (cd_grupo > 0)
                    sql1 = from item in sql1
                           where item.GrupoEstoque.cd_grupo_estoque == cd_grupo
                           select item;

                if (cd_tipo > 0)
                    sql1 = from item in sql1
                           where item.TipoItem.cd_tipo_item == cd_tipo
                           select item;

                var sql = from item in sql1
                          join itemE in db.ItemEscola on item.cd_item equals itemE.cd_item
                          join saldoItem in db.SaldoItem on item.cd_item equals saldoItem.cd_item
                          join fechamento in db.Fechamento on saldoItem.cd_fechamento equals fechamento.cd_fechamento
                          where itemE.cd_pessoa_escola == cd_pessoa_escola
                             && fechamento.cd_pessoa_empresa == cd_pessoa_escola
                             && fechamento.nm_ano_fechamento == ano
                             && fechamento.nm_mes_fechamento == mes
                         
                          orderby item.cd_grupo_estoque,
                                  item.cd_item,
                                  fechamento.nm_mes_fechamento
                          
                          select new RptItemFechamento
                          {
                              cd_fechamento = fechamento.cd_fechamento,
                              cd_grupo_estoque = item.cd_grupo_estoque,
                              cd_item = item.cd_item,
                              cd_tipo_item = item.TipoItem.cd_tipo_item,
                              dc_grupo = item.GrupoEstoque.no_grupo_estoque,
                              dc_item = item.no_item,
                              saldo = saldoItem.qt_saldo_data,
                              saldo_atual = saldoItem.qt_saldo_atual,
                              nm_ano_fechamento = fechamento.nm_ano_fechamento,
                              nm_mes_fechamento = fechamento.nm_mes_fechamento,
                              contado = saldoItem.qt_saldo_fechamento,

                              vl_custo = saldoItem.vl_custo_atual,
                              vl_item = saldoItem.vl_venda_atual,//saldoItem.qt_saldo_atual * itemE.vl_item,                            
                              vl_custo_atual = saldoItem.vl_custo_fechamento,
                              vl_venda_atual = saldoItem.vl_venda_fechamento
                          };
                
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }       
        }

        public IEnumerable<RptItemFechamento> rptItemSaldoBiblioteca(int cd_pessoa_escola, int cd_item, int cd_grupo, byte cd_tipo, DateTime dt_kardex)
        {
            try
            {
                IQueryable<Item> sql1;
                SGFWebContext comp = new SGFWebContext();
                byte origem = (byte)comp.LISTA_ORIGEM_LOGS["Emprestimo"];
                
                sql1 = from item in db.Item
                       where  item.ItemEscola.Where(i => i.cd_pessoa_escola == cd_pessoa_escola).Any()
                       select item;

                if (cd_item > 0)
                    sql1 = from item in sql1
                           where item.cd_item == cd_item                            
                           select item;

                if (cd_grupo > 0)
                    sql1 = from item in sql1
                           where item.GrupoEstoque.cd_grupo_estoque == cd_grupo
                           select item;

                var sql = from item in sql1
                          join itemE in db.ItemEscola on item.cd_item equals itemE.cd_item
                          where item.TipoItem.cd_tipo_item == (int) TipoItem.TipoItemEnum.ITEM_BIBLIOTECA
                          select new RptItemFechamento
                          {
                              cd_grupo_estoque = item.cd_grupo_estoque,
                              cd_item = item.cd_item,
                              cd_tipo_item = item.TipoItem.cd_tipo_item,
                              dc_grupo = item.GrupoEstoque.no_grupo_estoque,
                              dc_item = item.no_item,
                              saldo_atual = itemE.qt_estoque,
                              qt_saida = (from kardex in db.Kardex
                                          where kardex.cd_origem == origem
                                             && kardex.dt_kardex <= dt_kardex
                                             && kardex.cd_pessoa_empresa == cd_pessoa_escola
                                             && kardex.id_tipo_movimento == (int)Kardex.TipoMovimento.SAIDA
                                             && kardex.cd_item == item.cd_item
                                             && !(from kardexE in db.Kardex
                                                  where kardexE.cd_origem == origem
                                                     && kardexE.dt_kardex <= dt_kardex
                                                     && kardexE.cd_pessoa_empresa == cd_pessoa_escola
                                                     && kardexE.id_tipo_movimento == (int)Kardex.TipoMovimento.ENTRADA
                                                     && kardexE.cd_item == item.cd_item
                                                     && kardexE.cd_registro_origem == kardex.cd_registro_origem
                                                  select kardexE.cd_item).Any()
                                          select kardex.qtd_kardex).Sum() 
                          };

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Item> listItensWithEstoque(int cd_pessoa_escola, int cd_item, int cd_grupo, int cd_tipo)
        {
             try
             {
                 IQueryable<Item> sql1;

                sql1 = from item in db.Item
                       select item;

                if (cd_item > 0)
                    sql1 = from item in sql1
                           where item.cd_item == cd_item
                           select item;

                if (cd_grupo > 0)
                    sql1 = from item in sql1
                           where item.GrupoEstoque.cd_grupo_estoque == cd_grupo
                           select item;

                if (cd_tipo > 0)
                    sql1 = from item in sql1
                           where item.TipoItem.cd_tipo_item == cd_tipo
                           select item;

                var sql = (from item in sql1
                           orderby item.cd_grupo_estoque,
                                   item.cd_item
                          where item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cd_pessoa_escola).Any()
                            && item.TipoItem.id_movimentar_estoque
                           select new
                           {
                               cd_grupo_estoque = item.GrupoEstoque.cd_grupo_estoque,
                               cd_item = item.cd_item,
                               cd_tipo_item = item.cd_tipo_item,
                               no_grupo_estoque = item.GrupoEstoque.no_grupo_estoque,
                               no_item = item.no_item,
                               qt_estoque = item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cd_pessoa_escola).FirstOrDefault().qt_estoque
                           }).ToList().Select(x => new Item
                           {
                               cd_item = x.cd_item,
                               cd_tipo_item = x.cd_tipo_item,
                               GrupoEstoque = new GrupoEstoque
                                 {
                                     cd_grupo_estoque = x.cd_grupo_estoque,
                                     no_grupo_estoque = x.no_grupo_estoque
                                 },
                               no_item = x.no_item,
                               qt_estoque = x.qt_estoque
                           });
                return sql;
            }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
        }
        public IEnumerable<ItemUI> listItensMaterial(int cd_curso, int cd_escola)
        {
            try
            {
                IEnumerable<ItemUI> sql = (from item in db.Item
                                           join itemEscola in db.ItemEscola
                                           on item.cd_item equals itemEscola.cd_item
                                         where item.Cursos.Where(c => c.cd_curso == cd_curso).Any() &&
                                               itemEscola.cd_pessoa_escola == cd_escola
                                         select new
                                         {
                                             cd_item = item.cd_item,
                                             no_item = item.no_item,
                                             vl_item = itemEscola.vl_item,
                                             cd_situacao_tributario = item.GrupoEstoque != null? item.GrupoEstoque.cd_situacao_tributaria : 0,
                                             cd_plano_conta = (from pc in db.PlanoConta
                                                              where item.ItemSubgrupos.Where(isg => isg.cd_item == item.cd_item && isg.id_tipo_movimento == (byte)ItemSubgrupo.TipoSubgGrupo.SAIDA && pc.cd_subgrupo_conta == isg.cd_subgrupo_conta).Any() &&
                                                                          item.ItemEscola.Where(i => i.cd_pessoa_escola == cd_escola).Any() &&
                                                                          pc.cd_pessoa_empresa == cd_escola select pc.cd_plano_conta).FirstOrDefault(),
                                             desc_plano_conta = db.PlanoConta.Where(pc => item.ItemSubgrupos.Where(isg => isg.cd_item == item.cd_item && isg.id_tipo_movimento == (byte)ItemSubgrupo.TipoSubgGrupo.SAIDA && pc.cd_subgrupo_conta == isg.cd_subgrupo_conta).Any() && 
                                                                       item.ItemEscola.Where(i => i.cd_pessoa_escola == cd_escola).Any() && pc.cd_pessoa_empresa == cd_escola).FirstOrDefault().PlanoContaSubgrupo.no_subgrupo_conta,
                                             cd_grupo_estoque = item.cd_grupo_estoque,
                                             id_material_didatico = item.id_material_didatico,
                                             id_voucher_carga = item.id_voucher_carga,
                                             qt_horas_carga = item.qt_horas_carga
                                         }).ToList().Select(x => new ItemUI
                                         {
                                             cd_item = x.cd_item,
                                             no_item = x.no_item,
                                             vl_item = x.vl_item,
                                             cd_plano_conta = x.cd_plano_conta,
                                             desc_plano_conta = x.desc_plano_conta,
                                             cd_grupo_estoque = x.cd_grupo_estoque,
                                             id_material_didatico = x.id_material_didatico,
                                             id_voucher_carga = x.id_voucher_carga,
                                             qt_horas_carga = x.qt_horas_carga
                                         });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int quantidadeItensMaterialCurso(int cd_turma, int cd_escola)
        {
            try
            {
                int qtdItem = (from item in db.Item
                               where item.Cursos.Where(e => e.Curso.Turma.Where(t => t.cd_turma == cd_turma && t.cd_pessoa_escola == cd_escola && e.cd_curso == t.cd_curso).Any()).Any()
                                select item).Count();
                return qtdItem;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool getItemNomeEsc(string noItem, int cdEscola)
        {
            try
            {
                bool tpDesc = (from i in db.Item
                               where i.no_item == noItem &&
                               i.ItemEscola.Where(t => t.cd_pessoa_escola == cdEscola).Any()
                               select i.cd_item).Any();


                return tpDesc;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool getItemNome(string noItem)
        {
            try
            {
                bool tpDesc = (from i in db.Item
                               where i.no_item == noItem
                               select i.cd_item).Any();


                return tpDesc;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool getexisteParametroItem(int cd_item)
        {
            try
            {
                bool tpDesc = (from i in db.Item
                               where i.cd_item == cd_item &&
                               (i.ParametroBiblioteca.Any() || i.ParametroMensalidade.Any() || i.ParametroTaxaMens.Any())
                               select i.cd_item).Any();


                return tpDesc;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ItemUI> obterListaItemsKit(int cd_item_kit, int cdEscola)
        {
            try
            {
                IEnumerable<ItemUI> listaItemsKit = (from itemKit in db.ItemKit
                                                     where itemKit.cd_item_kit == cd_item_kit
                                                     select new
                                                     {
                                                         cd_item_kit = itemKit.cd_item_kit,
                                                         cd_item = itemKit.cd_item,
                                                         no_item = itemKit.Item.no_item,
                                                         qt_item_kit = itemKit.qt_item_kit,
                                                         cd_grupo_estoque = itemKit.Item.cd_grupo_estoque,
                                                         no_grupo_estoque = itemKit.Item.GrupoEstoque.no_grupo_estoque,
                                                         vl_item = itemKit.Item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).FirstOrDefault().vl_item
                                                     }).ToList().Select(x => new ItemUI
                                                     {
                                                         cd_item_kit = x.cd_item_kit,
                                                         cd_item = x.cd_item,
                                                         no_item = x.no_item,
                                                         cd_grupo_estoque = x.cd_grupo_estoque,
                                                         no_grupo_estoque = x.no_grupo_estoque,
                                                         qt_item_kit = x.qt_item_kit,
                                                         vl_item = x.vl_item
                                                     });


                return listaItemsKit;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<ItemUI> obterListaItemMovItemsKit(ItemMovimento item, int cdEscola)
        {
            try
            {
                IEnumerable<ItemUI> listaItemsKit = (from itemKit in db.ItemKit
                                                     join itemMovimento in db.ItemMovimento on itemKit.cd_item equals itemMovimento.cd_item
                                                     join itemMovItemKit in db.ItemMovItemKit on  itemMovimento.cd_item_movimento  equals  itemMovItemKit.cd_item_movimento 
                                                     where itemKit.cd_item_kit == item.cd_item_kit //&& itemMovItemKit.cd_item_movimento == item.cd_item_movimento
                                                     select new
                                                     {
                                                         cd_item_kit = itemKit.cd_item_kit,
                                                         cd_item = itemKit.cd_item,
                                                         no_item = itemKit.Item.no_item,
                                                         qt_item_kit = itemKit.qt_item_kit,
                                                         cd_grupo_estoque = itemKit.Item.cd_grupo_estoque,
                                                         no_grupo_estoque = itemKit.Item.GrupoEstoque.no_grupo_estoque,
                                                         vl_item = itemKit.Item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).FirstOrDefault().vl_item
                                                     }).ToList().Select(x => new ItemUI
                                                    {
                                                        cd_item = x.cd_item,
                                                        no_item = x.no_item,
                                                        cd_grupo_estoque = x.cd_grupo_estoque,
                                                        no_grupo_estoque = x.no_grupo_estoque,
                                                        qt_item_kit = x.qt_item_kit,
                                                        vl_item = x.vl_item
                                                    });


                return listaItemsKit;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ItemUI> obterListaItemsKitMov(int cd_item_kit, int cdEscola, int id_tipo_movto, int? id_natureza_TPNF)
        {
            try
            {
                int tipoSubgrupo = 0;
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

                IEnumerable<ItemUI> listaItemsKit = (from itemKit in db.ItemKit
                                                     where itemKit.cd_item_kit == cd_item_kit
                                                     select new
                                                     {
                                                         cd_item_kit = itemKit.cd_item_kit,
                                                         cd_item = itemKit.cd_item,
                                                         no_item = itemKit.Item.no_item,
                                                         qt_item_kit = itemKit.qt_item_kit,
                                                         cd_grupo_estoque = itemKit.Item.cd_grupo_estoque,
                                                         pc_aliquota_icms = itemKit.Item.pc_aliquota_icms,
                                                         no_grupo_estoque = itemKit.Item.GrupoEstoque.no_grupo_estoque,
                                                         vl_item = itemKit.Item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).FirstOrDefault().vl_item,
                                                         cd_plano_conta = (from pc in db.PlanoConta
                                                                           where itemKit.Item.ItemSubgrupos.Where(isg => isg.cd_item == itemKit.Item.cd_item && isg.id_tipo_movimento == tipoSubgrupo && pc.cd_subgrupo_conta == isg.cd_subgrupo_conta).Any() &&
                                                                                 itemKit.Item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).Any() &&
                                                                           pc.cd_pessoa_empresa == cdEscola
                                                                           select pc.cd_plano_conta).FirstOrDefault(),
                                                         desc_plano_conta = db.PlanoConta.Where(pc => itemKit.Item.ItemSubgrupos.Where(isg => isg.cd_item == itemKit.Item.cd_item && isg.id_tipo_movimento == tipoSubgrupo && pc.cd_subgrupo_conta == isg.cd_subgrupo_conta).Any() &&
                                                                                                      itemKit.Item.ItemEscola.Where(i => i.cd_pessoa_escola == cdEscola).Any() && pc.cd_pessoa_empresa == cdEscola).Select(pl => pl.PlanoContaSubgrupo.no_subgrupo_conta).FirstOrDefault()
                                                     }).ToList().Select(x => new ItemUI
                                                     {
                                                         cd_item_kit = x.cd_item_kit,
                                                         cd_item = x.cd_item,
                                                         no_item = x.no_item,
                                                         cd_plano_conta = x.cd_plano_conta,
                                                         pc_aliquota_icms = x.pc_aliquota_icms,
                                                         desc_plano_conta = x.desc_plano_conta,
                                                         cd_grupo_estoque = x.cd_grupo_estoque,
                                                         no_grupo_estoque = x.no_grupo_estoque,
                                                         qt_item_kit = x.qt_item_kit,
                                                         vl_item = x.vl_item
                                                     });


                return listaItemsKit;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}