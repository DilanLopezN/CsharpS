using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess
{
    public class AtividadeDataAccess : GenericRepository<Atividade>, IAtividadeDataAccess 
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<Atividade> getAllListAtividades(string searchText, int natureza, bool? status)
        {
            try{
                var sql = from atividade in db.Atividade
                          where atividade.id_natureza_atividade == natureza
                          && atividade.no_atividade.Contains(searchText)
                          select atividade;

                if (status == null)
                {
                    sql = from c in sql
                          select c;
                }
                else
                {
                    sql = from c in sql
                          where (c.id_atividade_ativa == status)
                          select c;
                }
                return sql;
                    //db.AtividadePessoa.AsEnumerable().Where(a => a.id_natureza_atividade == natureza && a.no_atividade.Contains(searchText));
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Atividade> GetAtividadeSearch(SearchParameters parametros, string descricao, bool inicio, bool? status, int natureza, string cnae)
        {
            try{
                IEntitySorter<Atividade> sorter = EntitySorter<Atividade>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Atividade> sql;
                if (status == null)
                {
                    sql = from c in db.Atividade.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.Atividade.AsNoTracking()
                          where (c.id_atividade_ativa == status)
                          select c;
                }     

                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.no_atividade.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.no_atividade.Contains(descricao)
                                  select c;

                if (natureza.Equals(1))
                    retorno = from c in retorno
                              where c.id_natureza_atividade == 1
                              select c;
                else
                    if(natureza.Equals(2))
                        retorno = from c in retorno
                                  where c.id_natureza_atividade == 2
                                  select c;
                    else
                        if (natureza.Equals(3))
                            retorno = from c in retorno
                                      where c.id_natureza_atividade == 3
                                      select c;
                if (!String.IsNullOrEmpty(cnae))
                    retorno = from c in retorno
                              where c.cd_cnae_atividade.StartsWith(cnae)
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
        public bool deleteAll(List<Atividade> atividades)
        {
            try{
                string strAtividade = "";
                if (atividades != null && atividades.Count > 0)
                    foreach (Atividade e in atividades)
                        strAtividade += e.cd_atividade + ",";

                // Remove o último ponto e virgula:
                if (strAtividade.Length > 0)
                    strAtividade = strAtividade.Substring(0, strAtividade.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_atividade where cd_atividade in(" + strAtividade + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
