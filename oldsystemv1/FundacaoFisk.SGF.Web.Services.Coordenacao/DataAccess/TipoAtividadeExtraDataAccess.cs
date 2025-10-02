using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using System.Data.Entity;
using Componentes.Utils;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class TipoAtividadeExtraDataAccess : GenericRepository<TipoAtividadeExtra>, ITipoAtividadeExtraDataAccess
    {
        public enum TipoConsultaAtivExtraEnum
        {
            HAS_ATIVO = 1,
            HAS_DIARIO_AULA = 2
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }



        public IEnumerable<TipoAtividadeExtra> getAtividadeDesc(SearchParameters parametros, string desc, bool inicio, bool? ativo)
        {
            try{
                IQueryable<TipoAtividadeExtra> sql;
                IEntitySorter<TipoAtividadeExtra> sorter = EntitySorter<TipoAtividadeExtra>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);

                if (ativo == null)
                {
                    sql = from atividadeExtra in db.TipoAtividadeExtra.AsNoTracking()
                          select atividadeExtra;
                }
                else
                {
                    sql = from atividadeExtra in db.TipoAtividadeExtra.AsNoTracking()
                          where atividadeExtra.id_tipo_atividade_extra_ativa == ativo
                          select atividadeExtra;
                }

                sql = sorter.Sort(sql);

                var retorno = from atividadeExtra in sql
                              select atividadeExtra;

                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        retorno = from atividadeExtra in sql
                                  where atividadeExtra.no_tipo_atividade_extra.StartsWith(desc)
                                  select atividadeExtra;
                    }//end if
                    else
                    {
                        retorno = from atividadeExtra in sql
                                  where atividadeExtra.no_tipo_atividade_extra.Contains(desc)
                                  select atividadeExtra;
                    }
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

        public bool deleteAllTipoAtividade(List<TipoAtividadeExtra> tiposAtividades)
        {
            try{
                string strAtividades = "";
                if (tiposAtividades != null && tiposAtividades.Count > 0)
                    foreach (TipoAtividadeExtra e in tiposAtividades)
                        strAtividades += e.cd_tipo_atividade_extra + ",";

                // Remove o último ponto e virgula:
                if (strAtividades.Length > 0)
                    strAtividades = strAtividades.Substring(0, strAtividades.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_tipo_atividade_extra where cd_tipo_atividade_extra in(" + strAtividades + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TipoAtividadeExtra> getTipoAtividade()
        {
            try
            {
                var sql = from tipoAtividade in db.TipoAtividadeExtra
                          where tipoAtividade.id_tipo_atividade_extra_ativa == true
                          select tipoAtividade;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TipoAtividadeExtra> getTipoAtividade(bool? status, int? cdTipoAtividadeExtra, int? cd_pessoa_escola, TipoConsultaAtivExtraEnum tipoConsulta)
        {
            try
            {
                var sql = from tipoAtividade in db.TipoAtividadeExtra
                          orderby tipoAtividade.no_tipo_atividade_extra ascending
                          select tipoAtividade;

                switch (tipoConsulta)
                {
                    case TipoConsultaAtivExtraEnum.HAS_ATIVO:
                        if (status != null)
                            sql = from tipoAtividade in sql
                                  where tipoAtividade.id_tipo_atividade_extra_ativa == status || (cdTipoAtividadeExtra.HasValue && tipoAtividade.cd_tipo_atividade_extra == cdTipoAtividadeExtra)
                                  orderby tipoAtividade.no_tipo_atividade_extra ascending
                                  select tipoAtividade;
                        break;
                    case TipoConsultaAtivExtraEnum.HAS_DIARIO_AULA:
                        sql = from tipoAtividade in sql
                              where tipoAtividade.id_tipo_atividade_extra_ativa == status
                              orderby tipoAtividade.no_tipo_atividade_extra ascending
                              select tipoAtividade;
                        if (cd_pessoa_escola != null)
                            sql = from tipoAtividade in db.TipoAtividadeExtra
                                  where tipoAtividade.DiarioAula.Where(da => da.cd_pessoa_empresa == cd_pessoa_escola).Any()
                                  orderby tipoAtividade.no_tipo_atividade_extra ascending
                                  select tipoAtividade;
                        if (cdTipoAtividadeExtra != null)
                            sql = from tipoAtividade in db.TipoAtividadeExtra
                                  where tipoAtividade.cd_tipo_atividade_extra == cdTipoAtividadeExtra
                                  orderby tipoAtividade.no_tipo_atividade_extra ascending
                                  select tipoAtividade;
                        break;
                }

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //retorna o primeiro registro ou o default
        public TipoAtividadeExtra firstOrDefault()
        {
            try{
                var sql = (from tipoAtividade in db.TipoAtividadeExtra
                           select tipoAtividade).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        
        //Retorna quantidade n de registros
        public List<TipoAtividadeExtra> findTopRegisters(int qtd) {
            try{
                var sql = (from tipoAtividade in db.TipoAtividadeExtra
                           select tipoAtividade).Take(qtd);
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //TO DO fazer o teste.
        public IEnumerable<TipoAtividadeExtra> getTipoAtividadeWhitAtividadeExtra(int cd_pessoa_escola)
        {
            try
            {
                var sql = (from tipoAtividade in db.TipoAtividadeExtra
                          join atividadeExtra in db.AtividadeExtra on tipoAtividade.cd_tipo_atividade_extra equals atividadeExtra.cd_tipo_atividade_extra
                          where (atividadeExtra.cd_pessoa_escola == cd_pessoa_escola ||
                            (from te in db.AtividadeEscolaAtividade
                             where te.cd_atividade_extra == atividadeExtra.cd_atividade_extra &&
                                 te.cd_escola == cd_pessoa_escola
                             select te).Any())

                          select tipoAtividade).Distinct().OrderBy(t => t.no_tipo_atividade_extra);
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}
