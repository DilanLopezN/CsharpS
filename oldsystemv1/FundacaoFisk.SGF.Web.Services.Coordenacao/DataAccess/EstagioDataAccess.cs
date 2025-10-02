using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class EstagioDataAccess : GenericRepository<Estagio>, IEstagioDataAccess
    {  

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public enum TipoConsultaEstagioEnum {
            ALL = 0,
            HAS_ATIVO = 1,
            HAS_CURSO = 2
        }

        public IEnumerable<Model.EstagioSearchUI> getEstagioDesc(Componentes.Utils.SearchParameters parametros, string desc, string abrev, bool inicio, bool? ativo, int CodP)
        {
            try{
                IEntitySorter<EstagioSearchUI> sorter = EntitySorter<EstagioSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<EstagioSearchUI> sql;

                sql = from estagio in db.Estagio.AsNoTracking()
                      where (CodP == 0 || estagio.cd_produto == CodP)
                      select new EstagioSearchUI
                      {
                          cd_estagio = estagio.cd_estagio,
                          no_estagio = estagio.no_estagio,
                          id_estagio_ativo = estagio.id_estagio_ativo,
                          nm_ordem_estagio = estagio.nm_ordem_estagio,
                          cd_produto = estagio.cd_produto,
                          no_produto = estagio.Produto.no_produto,
                          no_estagio_abreviado = estagio.no_estagio_abreviado,
                          cor_legenda = estagio.cor_legenda
                      };


                if (ativo == null)
                {
                    sql = from estagio in sql
                          select estagio;
                }
                else
                {
                    sql = from estagio in sql
                          where estagio.id_estagio_ativo == ativo
                          select estagio;
                }
                sql = sorter.Sort(sql);
                var retorno = from estagio in sql
                              select estagio;
                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        retorno = from estagio in sql
                                  where estagio.no_estagio.StartsWith(desc)
                                  select estagio;
                    }//end if
                    else
                    {
                        retorno = from estagio in sql
                                  where estagio.no_estagio.Contains(desc)
                                  select estagio;
                    }//end else

                }

                if (!String.IsNullOrEmpty(abrev))
                {
                    if (inicio)
                    {
                        retorno = from estagio in retorno
                                  where estagio.no_estagio_abreviado.StartsWith(abrev)
                                  select estagio;
                    }//end if
                    else
                    {
                        retorno = from estagio in retorno
                                  where estagio.no_estagio_abreviado.Contains(abrev)
                                  select estagio;
                    }//end else

                }

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


        public byte? getOrdem(int CodP)
        {
            try{
                return base.findAll(false).Where(e => e.cd_produto == CodP).Max(e => e.nm_ordem_estagio);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public IEnumerable<Estagio> getEstagioOrdem(int CodP, int? cd_estagio, TipoConsultaEstagioEnum? tipoConsulta)
        {
            try{
                IQueryable<Estagio> sql = from estagio in db.Estagio
                      where estagio.cd_produto == CodP
                      orderby estagio.nm_ordem_estagio descending
                      select estagio;
                var retorno = from estagio in sql
                              select estagio;
                if(tipoConsulta.HasValue)
                switch(tipoConsulta){
                    case TipoConsultaEstagioEnum.HAS_ATIVO :
                        retorno = from e in retorno
                                  where e.id_estagio_ativo || (cd_estagio.HasValue && cd_estagio.Value == e.cd_estagio)
                                  select e;
                        break;
                    case TipoConsultaEstagioEnum.HAS_CURSO : 
                        retorno = from r in retorno
                                  where (from e in r.Cursos select e.cd_produto).Contains(r.cd_produto)
                                  select r;
                        break;
                }

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public void editOrdem(Estagio estagio)
        {
            try{
                //db.Entry(estagio).Property(s => s.nm_ordem_estagio).IsModified = true;
                db.Estagio.Attach(estagio);
                db.Entry(estagio).Property(s => s.nm_ordem_estagio).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteAllEstagio(List<Estagio> estagio)
        {
            try{
                string strEstagio = "";
                if (estagio != null && estagio.Count > 0)
                    foreach (Estagio e in estagio)
                        strEstagio += e.cd_estagio + ",";

                // Remove o último ponto e virgula:
                if (strEstagio.Length > 0)
                    strEstagio = strEstagio.Substring(0, strEstagio.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_estagio where cd_estagio in(" + strEstagio + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Estagio> getAllEstagioByProduto(int cdProduto)
        {
            try{
                var sql = from e in db.Estagio
                          where e.cd_produto == cdProduto
                          select e;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
