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
using System.Data;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class EscolaridadeDataAccess : GenericRepository<Escolaridade>, IEscolaridadeDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<Escolaridade> GetEscolaridadeSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            try{
                IEntitySorter<Escolaridade> sorter = EntitySorter<Escolaridade>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Escolaridade> sql;

                if (ativo == null)
                {
                    sql = from c in db.Escolaridade.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.Escolaridade.AsNoTracking()
                          where (c.id_escolaridade_ativa == ativo)
                          select c;
                }     

                sql = sorter.Sort(sql);
                
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                if(inicio) 
                    retorno = from c in sql
                          where c.no_escolaridade.StartsWith(descricao)
                          select c;
                else
                    retorno = from c in sql
                              where c.no_escolaridade.Contains(descricao)
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

        public bool deleteAll(List<Escolaridade> escolaridades) {
            try{
                string strEscolaridades = "";
                if(escolaridades != null && escolaridades.Count > 0)
                    foreach(Escolaridade e in escolaridades)
                        strEscolaridades += e.cd_escolaridade + ",";

                // Remove o último ponto e virgula:
                if(strEscolaridades.Length > 0)
                    strEscolaridades = strEscolaridades.Substring(0, strEscolaridades.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_escolaridade where cd_escolaridade in(" + strEscolaridades + ")");
                
                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool editE(Escolaridade escolaridade) {
            bool retorno = false;
            try {
                Escolaridade escOld = this.findById(escolaridade.cd_escolaridade, false);
                escOld.no_escolaridade = escolaridade.no_escolaridade;
                escOld.id_escolaridade_ativa = escolaridade.id_escolaridade_ativa;
                escOld.cd_escolaridade = escolaridade.cd_escolaridade;
                //db.Entry(escOld).State = EntityState.Modified;
                retorno = db.SaveChanges() > 0;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
            return retorno;
        }

        public IEnumerable<Escolaridade> getEscolaridade(bool? status) {
            try{
                IEnumerable<Escolaridade> sql;

                if(status == null)
                    sql = from c in db.Escolaridade
                          select c;

                else
                    sql = from c in db.Escolaridade
                          where (c.id_escolaridade_ativa == status)
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
