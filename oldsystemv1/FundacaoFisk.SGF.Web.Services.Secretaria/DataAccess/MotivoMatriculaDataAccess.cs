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
    public class MotivoMatriculaDataAccess : GenericRepository<MotivoMatricula>, IMotivoMatriculaDataAccess
    {
       

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<MotivoMatricula> GetMotivoMatriculaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            try{
                IEntitySorter<MotivoMatricula> sorter = EntitySorter<MotivoMatricula>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<MotivoMatricula> sql;

                if (ativo == null)
                {
                    sql = from c in db.MotivoMatricula.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.MotivoMatricula.AsNoTracking()
                          where (c.id_motivo_matricula_ativo == ativo)
                          select c;
                }
                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_motivo_matricula.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_motivo_matricula.Contains(descricao)
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
        
        public bool deleteAll(List<MotivoMatricula> motivosMatricula) {
            try{
                string strMotivo = "";
                if(motivosMatricula != null && motivosMatricula.Count > 0)
                    foreach(MotivoMatricula e in motivosMatricula)
                        strMotivo += e.cd_motivo_matricula + ",";

                // Remove o último ponto e virgula:
                if(strMotivo.Length > 0)
                    strMotivo = strMotivo.Substring(0, strMotivo.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_motivo_matricula where cd_motivo_matricula in(" + strMotivo + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
