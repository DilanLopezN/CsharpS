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
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class TipoNotaFiscalDataAccess : GenericRepository<TipoNotaFiscal>, ITipoNotaFiscalDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<TipoNotaFiscal> getTipoNotaFiscalSearch(SearchParameters parametros, string desc, string natOp, bool inicio, bool? status, int movimento, bool? devolucao, int cdEscola, 
            byte id_regime_trib, bool? id_servico)
        {
            try
            {
                IEntitySorter<TipoNotaFiscal> sorter = EntitySorter<TipoNotaFiscal>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<TipoNotaFiscal> sql;
                sql = from c in db.TipoNotaFiscal.AsNoTracking()
                      orderby c.dc_tipo_nota_fiscal ascending
                      select c;
                if(cdEscola > 0)
                    sql = from s in sql
                          where db.Parametro.Where(p => p.id_regime_tributario == s.id_regime_tributario && p.cd_pessoa_escola == cdEscola).Any()
                          select s;
                else
                    if (id_regime_trib > 0)
                        sql = from s in sql
                              where s.id_regime_tributario == id_regime_trib
                              select s;
                if (!String.IsNullOrEmpty(desc))
                    if (inicio)
                        sql = from s in sql
                              where s.dc_tipo_nota_fiscal.StartsWith(desc)
                              select s;
                    else
                        sql = from s in sql
                              where s.dc_tipo_nota_fiscal.Contains(desc)
                              select s;
                if (!String.IsNullOrEmpty(natOp))
                    if (inicio)
                        sql = from s in sql
                              where s.dc_natureza_operacao.StartsWith(natOp)
                              select s;
                    else
                        sql = from s in sql
                              where s.dc_natureza_operacao.Contains(natOp)
                              select s;

                if (status != null)
                    sql = from c in sql
                          where (c.id_tipo_ativo == status)
                          select c;
                if (movimento > 0)
                    sql = from c in sql
                          where c.id_natureza_movimento == movimento
                          select c;
                if (devolucao.HasValue)
                    sql = from c in sql
                          where c.id_devolucao == devolucao
                          select c;
                if(id_servico != null)
                     sql = from c in sql
                           where c.id_servico == id_servico
                          select c;
                   
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


        public bool getTpNFUtilizado(int cdNota)
        {
            try
            {
                int sql = (from c in db.TipoNotaFiscal
                                     where c.cd_tipo_nota_fiscal == cdNota &&
                                          (c.ParametroMatricula.Any() ||
                                           c.ParametroBiblioteca.Any() ||
                                           c.TpNFMaterial.Any() ||
                                           c.Movimentos.Any())
                                      select c.cd_tipo_nota_fiscal).FirstOrDefault();

                return sql > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificarTipoNotaFiscalPermiteMovimentoFinanceiro(int cd_tipo_nota_fiscal) {
            try {
                return (from c in db.TipoNotaFiscal
                                      where c.cd_tipo_nota_fiscal == cd_tipo_nota_fiscal
                                           && c.id_movimenta_financeiro
                                      select c.cd_tipo_nota_fiscal).Any();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public bool verificarTipoNotaFiscalPermiteMovimentoEstoque(int cd_movimento) {
            try {
                return (from c in db.Movimento
                        where c.cd_movimento == cd_movimento
                             && c.TipoNF.id_movimenta_estoque
                        select c.cd_tipo_nota_fiscal).Any();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public bool getMovimentaEstoque(int cd_tipo_nota_fiscal) {
            try {
                return (from c in db.TipoNotaFiscal
                        where c.cd_tipo_nota_fiscal == cd_tipo_nota_fiscal
                        select c.id_movimenta_estoque).FirstOrDefault();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }
        public byte getTipoMvtoTpNF(int cd_tipo_nota_fiscal)
        {
            try
            {
                return (from c in db.TipoNotaFiscal
                        where c.cd_tipo_nota_fiscal == cd_tipo_nota_fiscal
                        select c.id_natureza_movimento).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
