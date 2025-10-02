using System;
using System.Collections.Generic;
using System.Linq;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using Componentes.GenericModel;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria;
using System.Data.Entity;
using Componentes.Utils;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class MotivoCancelamentoBolsaDataAccess : GenericRepository<MotivoCancelamentoBolsa>, IMotivoCancelamentoBolsaDataAccess
    {
       

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<MotivoCancelamentoBolsa> GetMotivoCancelBolsaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            try{
                IEntitySorter<MotivoCancelamentoBolsa> sorter = EntitySorter<MotivoCancelamentoBolsa>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<MotivoCancelamentoBolsa> sql;

                if (ativo == null)
                {
                    sql = from c in db.MotivoCancelamentoBolsa.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.MotivoCancelamentoBolsa.AsNoTracking()
                          where (c.id_motivo_cancelamento_bolsa_ativo == ativo)
                          select c;
                }
                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_motivo_cancelamento_bolsa.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_motivo_cancelamento_bolsa.Contains(descricao)
                                  select c;

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
        
        public bool deleteAll(List<MotivoCancelamentoBolsa> motivosCancelamento) {
            try{
                string strMotivo = "";
                if(motivosCancelamento != null && motivosCancelamento.Count > 0)
                    foreach(MotivoCancelamentoBolsa e in motivosCancelamento)
                        strMotivo += e.cd_motivo_cancelamento_bolsa + ",";

                // Remove o último ponto e virgula:
                if(strMotivo.Length > 0)
                    strMotivo = strMotivo.Substring(0, strMotivo.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_motivo_cancelamento_bolsa where cd_motivo_cancelamento_bolsa in(" + strMotivo + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<MotivoCancelamentoBolsa> getMotivoCancelamentoBolsa(bool? status, int? cd_motivo_cancelamento_bolsa) {
            try{
                IEnumerable<MotivoCancelamentoBolsa> sql;

                if(status == null) 
                    sql = from c in db.MotivoCancelamentoBolsa
                          select c;
                
                else 
                    sql = from c in db.MotivoCancelamentoBolsa
                          where c.id_motivo_cancelamento_bolsa_ativo == status || (cd_motivo_cancelamento_bolsa.HasValue && c.cd_motivo_cancelamento_bolsa == cd_motivo_cancelamento_bolsa.Value)
                          select c;
                
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
