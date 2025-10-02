using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class MotivoTransferenciaDataAccess : GenericRepository<MotivoTransferenciaAluno>, IMotivoTransferenciaDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<MotivoTransferenciaAluno> GetMotivoTransferenciaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            try
            {
                IEntitySorter<MotivoTransferenciaAluno> sorter = EntitySorter<MotivoTransferenciaAluno>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<MotivoTransferenciaAluno> sql;

                if (ativo == null)
                {
                    sql = from c in db.MotivoTransferenciaAluno.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.MotivoTransferenciaAluno.AsNoTracking()
                          where (c.id_motivo_transferencia_ativo == ativo)
                          select c;
                }
                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_motivo_transferencia_aluno.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_motivo_transferencia_aluno.Contains(descricao)
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
        public bool deleteAll(List<MotivoTransferenciaAluno> motivosTransferencia)
        {
            try
            {
                string strMotivo = "";
                if (motivosTransferencia != null && motivosTransferencia.Count > 0)
                    foreach (MotivoTransferenciaAluno e in motivosTransferencia)
                        strMotivo += e.cd_motivo_transferencia_aluno + ",";

                // Remove o último ponto e virgula:
                if (strMotivo.Length > 0)
                    strMotivo = strMotivo.Substring(0, strMotivo.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_motivo_transferencia_aluno where cd_motivo_transferencia_aluno in(" + strMotivo + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<MotivoTransferenciaAluno> getMotivosTransferencia()
        {
            try
            {
                var sql = from m in db.MotivoTransferenciaAluno
                          where m.id_motivo_transferencia_ativo
                          select m;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}
