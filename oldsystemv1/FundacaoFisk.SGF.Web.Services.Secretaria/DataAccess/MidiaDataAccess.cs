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
    public class MidiaDataAccess : GenericRepository<Midia>, IMidiaDataAccess
    {
        public enum TipoConsultaMidiaEnum
        {
            HAS_ATIVO = 0,
            HAS_ALUNO = 1,
            HAS_ATIVO_INATIVO = 2,
            HAS_PROSPECT = 3
        }
       

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<Midia> GetMidiaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            try{
                IEntitySorter<Midia> sorter = EntitySorter<Midia>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Midia> sql;

                if (ativo == null)
                {
                    sql = from c in db.Midia.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.Midia.AsNoTracking()
                          where (c.id_midia_ativa == ativo)
                          select c;
                }     
                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.no_midia.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.no_midia.Contains(descricao)
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
        

        public bool deleteAll(List<Midia> midias) {
            try{
                string strMidias = "";
                if(midias != null && midias.Count > 0)
                    foreach(Midia e in midias)
                        strMidias += e.cd_midia + ",";

                // Remove o último ponto e virgula:
                if(strMidias.Length > 0)
                    strMidias = strMidias.Substring(0, strMidias.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_midia where cd_midia in(" + strMidias + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Midia> getMidias(bool? status, TipoConsultaMidiaEnum tipo, int? cd_empresa) {
            try{
                IEnumerable<Midia> sql;

                sql = from c in db.Midia
                      select c;
                switch (tipo)
                {
                    case TipoConsultaMidiaEnum.HAS_ATIVO_INATIVO:
                        if (status != null)
                            sql = from c in db.Midia
                                  where (c.id_midia_ativa == status)
                                  select c;
                        break;
                    case TipoConsultaMidiaEnum.HAS_ALUNO:
                        if (cd_empresa.HasValue)
                            sql = from m in db.Midia
                                  where m.MidiaAluno.Any(a => a.cd_pessoa_escola == cd_empresa.Value)
                                  select m;
                        break;
                    case TipoConsultaMidiaEnum.HAS_PROSPECT:
                        if (cd_empresa.HasValue)
                            sql = from m in db.Midia
                                  where m.MidiaProspect.Any(a => a.cd_pessoa_escola == cd_empresa.Value)
                                  select m;
                        break;
                }

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
