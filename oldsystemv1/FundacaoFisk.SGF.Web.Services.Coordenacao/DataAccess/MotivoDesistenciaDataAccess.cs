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
    public class MotivoDesistenciaDataAccess : GenericRepository<MotivoDesistencia>, IMotivoDesistenciaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<MotivoDesistencia> getMotivoDesistenciaDesc(SearchParameters parametros, string desc, bool inicio, bool? ativo, bool isCancelamento)
        {
            try{
                IQueryable<MotivoDesistencia> sql;
                IEntitySorter<MotivoDesistencia> sorter = EntitySorter<MotivoDesistencia>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);

                if (ativo == null)
                {
                    sql = from motivo in db.MotivoDesistencia.AsNoTracking()
                          select motivo;
                }
                else
                {
                    sql = from motivo in db.MotivoDesistencia.AsNoTracking()
                          where motivo.id_motivo_desistencia_ativo == ativo
                          select motivo;
                }

                if (isCancelamento)
                    sql = from motivo in sql
                          where motivo.id_cancelamento == isCancelamento
                          select motivo;

                sql = sorter.Sort(sql);           
                var retorno = from motivo in sql
                              select motivo;

                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        retorno = from motivo in sql
                                  where motivo.dc_motivo_desistencia.StartsWith(desc)
                                  select motivo;
                    }// end if desc
                    else
                    {
                        retorno = from motivo in sql
                                  where motivo.dc_motivo_desistencia.Contains(desc)
                                  select motivo;

                    }// end else
                }// end if

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
       
        public bool deleteAllMotivoDesistencia(List<MotivoDesistencia> desitencias)
        {
            try{
                string strDesistencia = "";
                if (desitencias != null && desitencias.Count > 0)
                    foreach (MotivoDesistencia e in desitencias)
                        strDesistencia += e.cd_motivo_desistencia + ",";

                // Remove o último ponto e virgula:
                if (strDesistencia.Length > 0)
                    strDesistencia = strDesistencia.Substring(0, strDesistencia.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_motivo_desistencia where cd_motivo_desistencia in(" + strDesistencia + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        
        public IEnumerable<MotivoDesistencia> motivosDesistenciaWhitDesistencia(int cd_pessoa_empresa)
        {
            try
            {
                var sql = from motivo in db.MotivoDesistencia
                          where motivo.Desistencia.Any(d => d.cd_motivo_desistencia == motivo.cd_motivo_desistencia)
                          && motivo.Desistencia.Where(d => d.AlunoTurma.Turma.cd_pessoa_escola == cd_pessoa_empresa).Any()
                          orderby motivo.dc_motivo_desistencia
                          select motivo;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<MotivoDesistencia> getMotivoDesistenciaByCancelamento(bool isCancelamento) {
            try
            {
                var sql = from motivo in db.MotivoDesistencia
                          where motivo.id_cancelamento == isCancelamento
                          orderby motivo.dc_motivo_desistencia
                          select motivo;

                return sql;
            }
            catch (Exception exe)
            {                
                throw new DataAccessException(exe);
            }
        }
    }
}
