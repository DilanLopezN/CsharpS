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
    public class MotivoNaoMatriculaDataAccess : GenericRepository<MotivoNaoMatricula>, IMotivoNaoMatriculaDataAccess
    {
       

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<MotivoNaoMatricula> GetMotivoNaoMatriculaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            try{
                IEntitySorter<MotivoNaoMatricula> sorter = EntitySorter<MotivoNaoMatricula>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<MotivoNaoMatricula> sql;

                if (ativo == null)
                {
                    sql = from c in db.MotivoNaoMatricula.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.MotivoNaoMatricula.AsNoTracking()
                          where (c.id_motivo_nao_matricula_ativo == ativo)
                          select c;
                }
                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_motivo_nao_matricula.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_motivo_nao_matricula.Contains(descricao)
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
        public IEnumerable<MotivoNaoMatricula> getMotivoNaoMatriculaProspect(int cdProspect)
        {
            try
            {
                var sql = (from motivo in db.MotivoNaoMatricula
                           where motivo.MotivoNaoMatriculaProspect.Where(d => d.cd_prospect == cdProspect).Any()
                           select motivo).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }
        public bool deleteAll(List<MotivoNaoMatricula> motivosNMatricula) {
            try{
                string strMotivo = "";
                if(motivosNMatricula != null && motivosNMatricula.Count > 0)
                    foreach(MotivoNaoMatricula e in motivosNMatricula)
                        strMotivo += e.cd_motivo_nao_matricula + ",";

                // Remove o último ponto e virgula:
                if(strMotivo.Length > 0)
                    strMotivo = strMotivo.Substring(0, strMotivo.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_motivo_nao_matricula where cd_motivo_nao_matricula in(" + strMotivo + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<MotivoNaoMatricula> getMotivosNaoMatricula()
        {
            try
            {
                var sql = from m in db.MotivoNaoMatricula
                          where m.id_motivo_nao_matricula_ativo
                          select m;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<MotivoNaoMatricula> getProspectMotivoNaoMatricula(int cdProspect, int cdEscola)
        {
            try
            {
                var sql = (from motivo in db.ProspectMotivoNaoMatricula
                           where motivo.cd_prospect == cdProspect && motivo.Prospect.cd_pessoa_escola == cdEscola
                           select new
                           {
                               motivo.cd_prospect_motivo_nao_matricula,
                               motivo.MotivoNaoMatricula.cd_motivo_nao_matricula,
                               motivo.MotivoNaoMatricula.dc_motivo_nao_matricula,
                               motivo.MotivoNaoMatricula.id_motivo_nao_matricula_ativo
                           }).ToList().Select(x => new MotivoNaoMatricula { 
                               cd_prospect_motivo_nao_matricula = x.cd_prospect_motivo_nao_matricula,
                               cd_motivo_nao_matricula = x.cd_motivo_nao_matricula,
                               dc_motivo_nao_matricula = x.dc_motivo_nao_matricula,
                               id_motivo_nao_matricula_ativo = x.id_motivo_nao_matricula_ativo
                           });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
