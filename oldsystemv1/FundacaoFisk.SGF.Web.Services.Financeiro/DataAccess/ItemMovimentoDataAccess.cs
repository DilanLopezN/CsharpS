using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class ItemMovimentoDataAccess : GenericRepository<ItemMovimento>, IItemMovimentoDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<ItemMovimento> getItensMovimentoByMovimento(int cd_movimento, int cd_empresa)
        {
            try
            {
                var sql = from i in db.ItemMovimento
                          where i.cd_movimento == cd_movimento && i.Movimento.cd_pessoa_empresa == cd_empresa
                          select i;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ItemMovimento> getItensMovimento(int cd_movimento, int cd_empresa)
        {
            try
            {
                List<ItemMovimento> itens = (from im in db.ItemMovimento
                                             where im.cd_movimento == cd_movimento && im.Movimento.cd_pessoa_empresa == cd_empresa
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
                                                dc_plano_conta = x.no_plano_conta//,
                                                //no_item = x.no_item

                                            }).ToList();
                return itens;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ItemMovimento> getItensByAluno(SearchParameters parametros, int cd_pessoa, int cd_aluno, int cd_escola) {
            try {
                SGFWebContext dbComp = new SGFWebContext();
                int? cdEmpresaLig = (from empresa in db.PessoaSGF.OfType<Escola>() where empresa.cd_pessoa == cd_escola select empresa.cd_empresa_coligada).FirstOrDefault();
                cd_escola = (int)(cdEmpresaLig == null ? cd_escola : cdEmpresaLig);
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                IEntitySorter<ItemMovimento> sorter = EntitySorter<ItemMovimento>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection) (int) parametros.sortOrder);
                IQueryable<ItemMovimento> sql;
                sql = from i in db.ItemMovimento.AsNoTracking()
                      where i.Movimento.cd_pessoa_empresa == cd_escola &&
                                 i.CFOP.nm_cfop != 922 &&
                                 i.Movimento.cd_aluno == cd_aluno
                           select i;

                sql = sorter.Sort(sql);
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                var sql2 = (from i in sql
                       select new {
                           dt_emissao_movimento = i.Movimento.dt_emissao_movimento,
                           nm_movimento = i.Movimento.nm_movimento,
                           dc_item_movimento = i.dc_item_movimento,
                           vl_liquido_item = i.vl_liquido_item,
                           dc_nm_movimento = i.Movimento.nm_movimento.ToString() + (i.Movimento.dc_serie_movimento == null ? "" : "-" + i.Movimento.dc_serie_movimento)
                       }).ToList().Select(x => new ItemMovimento {
                           dt_emissao_movimento = x.dt_emissao_movimento,
                           nm_movimento = x.nm_movimento,
                           dc_item_movimento = x.dc_item_movimento,
                           vl_liquido_item = x.vl_liquido_item,
                           dc_nm_movimento = x.dc_nm_movimento
                       });

                return sql2;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public List<ItemMovimento> getItensMaterialAluno(List<int> cdAlunos, int cd_empresa, int cd_turma)
        {
            try
            {
                var sql = (from it in db.ItemMovimento
                           where it.Movimento.cd_pessoa_empresa == cd_empresa && it.Movimento.cd_aluno != null &&
                                 cdAlunos.Contains((int)it.Movimento.cd_aluno) &&
                                 it.Movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA &&
                                 it.Movimento.id_material_didatico &&
                                 it.Item.Cursos.Where(c => c.Curso.Turma.Where(t => t.cd_turma == cd_turma && t.cd_curso == c.cd_curso).Any()).Any()
                           select new
                           {
                               cd_cliente = it.Movimento.Aluno.cd_pessoa_aluno,
                               cd_item = it.cd_item
                           }).ToList().Select(x => new ItemMovimento
                           {
                               cd_item = x.cd_item,
                               cd_pessoa_cliente = x.cd_cliente
                           }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<ItemMovimento> getItensMvto(int cd_movimento, int cd_escola)
        {
            try
            {
                List<ItemMovimento> itens = (from im in db.ItemMovimento
                                             where im.cd_movimento == cd_movimento &&
                                                im.Movimento.cd_pessoa_empresa == cd_escola
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
                                                 pc_desconto = im.pc_desconto_item,
                                                 id_voucher_carga = im.Item.id_voucher_carga,
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
                                                id_voucher_carga = x.id_voucher_carga
                                            }).ToList();
                return itens;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ItemMovimento> getItensMovimentoRecibo(int cd_movimento, int cd_empresa)
        {
            try
            {
                List<ItemMovimento> itens = (from im in db.ItemMovimento
                                             where im.cd_movimento == cd_movimento && im.Movimento.cd_pessoa_empresa == cd_empresa
                                             select new
                                             {
                                                 cd_item_movimento = im.cd_item_movimento,
                                                 cd_movimento = im.cd_movimento,
                                                 cd_item = im.cd_item,
                                                 dc_item_movimento = im.dc_item_movimento,
                                                 qt_item_movimento = im.qt_item_movimento,
                                             }).ToList().Select(x => new ItemMovimento
                                             {
                                                 cd_item_movimento = x.cd_item_movimento,
                                                 cd_movimento = x.cd_movimento,
                                                 cd_item = x.cd_item,
                                                 dc_item_movimento = x.dc_item_movimento,
                                                 qt_item_movimento = x.qt_item_movimento,
                                             }).ToList();
                return itens;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ItemMovimento> getSomatorioValoresItensMovimentoDevolucao(int cd_movimento, int cd_empresa)
        {
            try
            {
                var sql = (from im in db.ItemMovimento
                           join imOrig in db.ItemMovimento on im.cd_item equals imOrig.cd_item
                           where im.Movimento.cd_nota_fiscal == cd_movimento && 
                                 imOrig.Movimento.cd_movimento == cd_movimento && 
                                 im.Movimento.cd_pessoa_empresa == cd_empresa &&
                                 im.Movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO &&
                                 (im.Movimento.id_status_nf == (int)Movimento.StatusNFEnum.FECHADO || !im.Movimento.id_nf)
                  group im by new
                 {
                     cd_item = im.cd_item,
                     qtd_item_nota_original = imOrig.qt_item_movimento,
                     vl_unitario_item = im.vl_unitario_item,
                     vl_total_item = im.vl_total_item,
                     vl_liquido_item = im.vl_liquido_item,
                     vl_acrescimo_item = im.vl_acrescimo_item,
                     vl_desconto_item = im.vl_desconto_item,
                     vl_base_calculo_ICMS_item = im.vl_base_calculo_ICMS_item,
                     vl_ICMS_item = im.vl_ICMS_item,
                     vl_base_calculo_PIS_item = im.vl_base_calculo_PIS_item,
                     vl_PIS_item = im.vl_PIS_item,
                     vl_base_calculo_COFINS_item = im.vl_base_calculo_COFINS_item,
                     vl_COFINS_item = im.vl_COFINS_item,
                     vl_base_calculo_IPI_item = im.vl_base_calculo_IPI_item,
                     vl_IPI_item = im.vl_IPI_item,
                     vl_aproximado = im.vl_aproximado,
                     pc_aliquota_aproximada = im.pc_aliquota_aproximada,
                     qt_item_movimento = im.qt_item_movimento
                 } into Gim
                 where
                      Gim.Sum(x => x.qt_item_movimento) < Gim.Key.qtd_item_nota_original
                 select new
                 {
                     cd_item = Gim.Key.cd_item,
                     vl_unitario_item = Gim.Sum(x => x.vl_unitario_item),
                     vl_total_item = Gim.Sum(x => x.vl_total_item),
                     vl_liquido_item = Gim.Sum(x => x.vl_liquido_item),
                     vl_acrescimo_item = Gim.Sum(x => x.vl_acrescimo_item),
                     vl_desconto_item = Gim.Sum(x => x.vl_desconto_item),
                     vl_base_calculo_ICMS_item = Gim.Sum(x => x.vl_base_calculo_ICMS_item),
                     vl_ICMS_item = Gim.Sum(x => x.vl_ICMS_item),
                     vl_base_calculo_PIS_item = Gim.Sum(x => x.vl_base_calculo_PIS_item),
                     vl_PIS_item = Gim.Sum(x => x.vl_PIS_item),
                     vl_base_calculo_COFINS_item = Gim.Sum(x => x.vl_base_calculo_COFINS_item),
                     vl_COFINS_item = Gim.Sum(x => x.vl_COFINS_item),
                     vl_base_calculo_IPI_item = Gim.Sum(x => x.vl_base_calculo_IPI_item),
                     vl_IPI_item = Gim.Sum(x => x.vl_IPI_item),
                     vl_aproximado = Gim.Sum(x => x.vl_aproximado),
                     pc_aliquota_aproximada = Gim.Sum(x => x.pc_aliquota_aproximada),
                     qt_item_movimento = Gim.Sum(x => x.qt_item_movimento)
                 }).ToList().Select(x => new ItemMovimento
                 {
                     cd_item = x.cd_item,
                     qt_item_movimento = x.qt_item_movimento,
                     vl_unitario_item = x.vl_unitario_item,
                     vl_total_item = x.vl_total_item,
                     vl_liquido_item = x.vl_liquido_item,
                     vl_acrescimo_item = x.vl_acrescimo_item,
                     vl_desconto_item = x.vl_desconto_item,
                     vl_base_calculo_ICMS_item = x.vl_base_calculo_ICMS_item,
                     vl_ICMS_item = x.vl_ICMS_item,
                     vl_base_calculo_PIS_item = x.vl_base_calculo_PIS_item,
                     vl_PIS_item = x.vl_PIS_item,
                     vl_base_calculo_COFINS_item = x.vl_base_calculo_COFINS_item,
                     vl_COFINS_item = x.vl_COFINS_item,
                     vl_base_calculo_IPI_item = x.vl_base_calculo_IPI_item,
                     vl_IPI_item = x.vl_IPI_item,
                     vl_aproximado = x.vl_aproximado,
                     pc_aliquota_aproximada = x.pc_aliquota_aproximada
                 }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
