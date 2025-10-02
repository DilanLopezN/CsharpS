using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class ModalidadeDataAccess : GenericRepository<Modalidade>, IModalidadeDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public enum TipoConsultaModalidadeEnum {
            HAS_ATIVO = 1,
            HAS_CURSO = 2
        }

        public IEnumerable<Modalidade> getModalidades(TipoConsultaModalidadeEnum? tipoConsulta, int? cd_modalidade) {
            try{
                IQueryable<Modalidade> retorno = from modalidade in db.Modalidade
                                                 orderby modalidade.no_modalidade
                                                 select modalidade;
                
                if(tipoConsulta.HasValue)
                    switch(tipoConsulta) {
                        case TipoConsultaModalidadeEnum.HAS_ATIVO:
                            retorno = from r in retorno
                                      where r.id_modalidade_ativa || (cd_modalidade.HasValue && cd_modalidade.Value == r.cd_modalidade)
                                      select r;
                            break;
                        case TipoConsultaModalidadeEnum.HAS_CURSO:
                            retorno = from r in retorno
                                      where (from c in r.Cursos select c.cd_modalidade).Contains(r.cd_modalidade)
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

        public IEnumerable<Modalidade> getModalidadeDesc(Componentes.Utils.SearchParameters parametros, string desc, bool inicio, bool? ativo)
        {
            try{
                IQueryable<Modalidade> sql;
                IEntitySorter<Modalidade> sorter = EntitySorter<Modalidade>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);

                if (ativo == null)
                {
                    sql = from modalidade in db.Modalidade.AsNoTracking()
                          select modalidade;

                }//end if
                else
                {
                    sql = from modalidade in db.Modalidade.AsNoTracking()
                          where modalidade.id_modalidade_ativa == ativo
                          select modalidade;
                }           
                sql = sorter.Sort(sql);           
                var retorno = from modalidade in sql
                              select modalidade;

                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        retorno = from modalidade in sql
                                  where modalidade.no_modalidade.StartsWith(desc)
                                  select modalidade;
                    }// if inicio
                    else
                    {
                        retorno = from modalidade in sql
                                  where modalidade.no_modalidade.Contains(desc)
                                  select modalidade;
                    }// else 
                }// id desc

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

        public bool deleteAllModalidade(List<Modalidade> modalidades)
        {
            try{
                string strModalidade = "";
                if (modalidades != null && modalidades.Count > 0)
                    foreach (Modalidade e in modalidades)
                        strModalidade += e.cd_modalidade + ",";

                // Remove o último ponto e virgula:
                if (strModalidade.Length > 0)
                    strModalidade = strModalidade.Substring(0, strModalidade.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_modalidade where cd_modalidade in(" + strModalidade + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        
    }
}
