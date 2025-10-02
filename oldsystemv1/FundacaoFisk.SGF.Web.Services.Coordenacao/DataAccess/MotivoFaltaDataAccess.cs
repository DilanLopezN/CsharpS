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
    public class MotivoFaltaDataAccess : GenericRepository<MotivoFalta>, IMotivoFaltaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<MotivoFalta> getMotivoFaltaDesc(SearchParameters parametros, string desc, bool inicio, bool? ativo)
        {
            try{
                IQueryable<MotivoFalta> sql;
                IEntitySorter<MotivoFalta> sorter = EntitySorter<MotivoFalta>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);

                if (ativo == null)
                {
                    sql = from motivoFtl in db.MotivoFalta.AsNoTracking()
                          select motivoFtl;
                }//end if
                else
                {
                    sql = from motivoFtl in db.MotivoFalta.AsNoTracking()
                          where motivoFtl.id_motivo_falta_ativa == ativo
                          select motivoFtl;
                }//end else
                sql = sorter.Sort(sql);           
                var retorno = from motivoFtl in sql
                              select motivoFtl;

                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        retorno = from motivoftl in sql
                                  where motivoftl.dc_motivo_falta.StartsWith(desc)
                                  select motivoftl;
                    }// end if início
                    else
                    {
                        retorno = from motivoFtl in sql
                                  where motivoFtl.dc_motivo_falta.Contains(desc)
                                  select motivoFtl;
                    }// else
                }//if desc

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

        public bool deleteAllMotivoFalta(List<MotivoFalta> motivos)
        {
            try{
                string strMotivo = "";
                if (motivos != null && motivos.Count > 0)
                    foreach (MotivoFalta e in motivos)
                        strMotivo += e.cd_motivo_falta + ",";

                // Remove o último ponto e virgula:
                if (strMotivo.Length > 0)
                    strMotivo = strMotivo.Substring(0, strMotivo.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_motivo_falta where cd_motivo_falta in(" + strMotivo + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<MotivoFalta> getMotivoFaltaAtivo(int cdDiario)
        {
            try
            {
                var retorno = from motivoFtl in db.MotivoFalta
                              where motivoFtl.id_motivo_falta_ativa == true ||
                              motivoFtl.DiariosAula.Where(d => d.cd_diario_aula == cdDiario).Any()
                              select motivoFtl;

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
