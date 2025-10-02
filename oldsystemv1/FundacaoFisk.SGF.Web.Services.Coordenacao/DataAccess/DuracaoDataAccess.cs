using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class DuracaoDataAccess : GenericRepository<Duracao>, IDuracaoDataAccess
    {
        public enum TipoConsultaDuracaoEnum
        {
            HAS_ATIVO = 0,
            HAS_TURMA = 1,
            HAS_CARGA_PROFESSOR = 2
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }


        public IEnumerable<Duracao> getDuracaoDesc(SearchParameters parametros, string desc, bool inicio, bool? ativo)
        {
            try
            {
                IQueryable<Duracao> sql;
                IEntitySorter<Duracao> sorter = EntitySorter<Duracao>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);

                if (ativo == null)
                {
                    sql = from duracao in db.Duracao.AsNoTracking()
                          select duracao;
                }
                else
                {
                    sql = from duracao in db.Duracao.AsNoTracking()
                          where duracao.id_duracao_ativa == ativo
                          select duracao;
                }
                sql = sorter.Sort(sql);
                var retorno = from duracao in sql
                              select duracao;
                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        retorno = from duracao in sql
                                  where duracao.dc_duracao.StartsWith(desc)
                                  select duracao;
                    }//end if
                    else
                    {
                        retorno = from duracao in sql
                                  where duracao.dc_duracao.Contains(desc)
                                  select duracao;
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

        public bool deleteAllDuracao(List<Duracao> duracao)
        {
            try
            {
                string strDuracao = "";
                if (duracao != null && duracao.Count > 0)
                    foreach (Duracao e in duracao)
                        strDuracao += e.cd_duracao + ",";

                // Remove o último ponto e virgula:
                if (strDuracao.Length > 0)
                    strDuracao = strDuracao.Substring(0, strDuracao.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_duracao where cd_duracao in(" + strDuracao + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Duracao> getDuracoes(TipoConsultaDuracaoEnum hasDependente, int? cd_duracao,int? cd_escola)
        {
            try
            {
                IQueryable<Duracao> sql = null;
                switch (hasDependente)
                {
                    case TipoConsultaDuracaoEnum.HAS_ATIVO:
                        sql = from duracao in db.Duracao
                              where duracao.id_duracao_ativa == true || (cd_duracao.HasValue && duracao.cd_duracao == cd_duracao.Value)
                              orderby duracao.dc_duracao
                              select duracao;
                        break;
                    case TipoConsultaDuracaoEnum.HAS_TURMA:
                        sql = from duracao in db.Duracao
                              where duracao.Turma.Where(x => x.cd_pessoa_escola == (int)cd_escola).Any()
                              orderby duracao.dc_duracao
                              select duracao;
                        break;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Duracao> getDuracaoTabelaPreco()
        {
            try{
                IQueryable<Duracao> sql;

                sql = (from duracao in db.Duracao
                      join tabela in db.TabelaPreco
                      on duracao.cd_duracao equals tabela.cd_duracao
                      select duracao).Distinct().OrderBy(d => d.dc_duracao);

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Duracao> getDuracaoProgramacao()
        {
            try
            {
                var sql = (from prog in db.ProgramacaoCurso
                           join duracao in db.Duracao
                           on prog.cd_duracao equals duracao.cd_duracao
                           select duracao).Distinct().OrderBy(d => d.dc_duracao);
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}
