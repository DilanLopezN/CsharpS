using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using System.Data.SqlClient;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using Componentes.GenericDataAccess.GenericException;
using System.Data.Entity.Core.Objects;
using System.Data.Objects.SqlClient;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class PlanoTituloDataAccess : GenericRepository<PlanoTitulo>, IPlanoTituloDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }
        public PlanoTitulo getPlanoTituloByTitulo(int cdTitulo, int cdPlanoConta, int cdEscola)
        {
            try
            {
                PlanoTitulo sql = (from pt in db.PlanoTitulo
                                   where pt.cd_titulo == cdTitulo &&
                                   pt.cd_plano_conta == cdPlanoConta &&
                                   pt.PlanoConta.cd_pessoa_empresa == cdEscola
                                   select pt).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PlanoTitulo> getPlanoTituloByTitulo(int cdTitulo, int cd_escola) {
            try {
                var sql = (from pt in db.PlanoTitulo
                          where pt.cd_titulo == cdTitulo &&
                                   pt.PlanoConta.cd_pessoa_empresa == cd_escola
                          select new {
                                    cd_plano_conta = pt.cd_plano_conta,
                                    vl_plano_titulo = pt.vl_plano_titulo,
                                    cd_plano_titulo = pt.cd_plano_titulo,
                                    cd_titulo = pt.cd_titulo
                               }).ToList().Select(x => new PlanoTitulo {
                                   cd_plano_conta = x.cd_plano_conta,
                                   vl_plano_titulo = x.vl_plano_titulo,
                                   cd_plano_titulo = x.cd_plano_titulo,
                                   cd_titulo = x.cd_titulo
                               });
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PlanoTitulo> getPlanoTituloByTitulos(int[] cdTitulos, int cd_empresa)
        {
            try
            {
                var sql = from pt in db.PlanoTitulo
                                   where cdTitulos.Contains(pt.cd_titulo ) &&
                                   pt.PlanoConta.cd_pessoa_empresa == cd_empresa
                                   select pt;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<RptPlanoTitulo> getPlanosContaPosicaoFinanceira(int cd_titulo) {
            try {
                IEnumerable<RptPlanoTitulo> sql = (from pt in db.PlanoTitulo
                                                   where pt.cd_titulo == cd_titulo
                                                   select new {
                                                       no_subgrupo_conta = pt.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta,
                                                       vl_plano_titulo = pt.vl_plano_titulo
                                                   }).ToList().Select(x => new RptPlanoTitulo {
                                                       no_subgrupo_conta = x.no_subgrupo_conta,
                                                       vl_plano_titulo = x.vl_plano_titulo
                                                   });
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public PlanoTitulo getPlanoTituloByCdTitulo(int cd_titulo, int cd_pessoa_empresa)
        {
            try
            {
                PlanoTitulo sql = (from pt in db.PlanoTitulo
                                   where pt.cd_titulo == cd_titulo &&
                                   pt.PlanoConta.cd_pessoa_empresa == cd_pessoa_empresa
                                   select pt).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}