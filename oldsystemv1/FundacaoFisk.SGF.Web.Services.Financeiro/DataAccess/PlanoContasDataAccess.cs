using System;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class PlanoContasDataAccess: GenericRepository<PlanoConta>, IPlanoContasDataAccess
    {

        public enum TipoPlanoContaConsulta
        {
            DISPONIVEIS = 0,
            MEU_PLANO_CONTA = 1
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }
        

        public IEnumerable<PlanoConta> getPlanoContasSearch(int cd_pessoa_empresa) {
          try{
            var sql = from planoContas in db.PlanoConta.Include(p => p.PlanoContaSubgrupo).Include(p => p.PlanoContaSubgrupo.SubgrupoContaGrupo)
                      where planoContas.cd_pessoa_empresa == cd_pessoa_empresa
                      select planoContas;
            return sql;
          }
          catch (Exception exe)
          {
              throw new DataAccessException(exe);
          } 
        }

        public PlanoConta confirmSubGrupoHasPlanoByIdSubgrupo(int cd_sub_grupo, int cd_pessoa_empresa)
        {
            try
            {
                var sql = (from plano in db.PlanoConta
                           where plano.cd_subgrupo_conta == cd_sub_grupo
                           && plano.cd_pessoa_empresa == cd_pessoa_empresa
                           select plano).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            } 
        
        }

        public String getDescPlanoContaByEscola(int cd_pessoa_empresa, int cd_plano_conta) {

            try
            {
                var sql = (from plano in db.PlanoConta
                           join sbGrupo in db.SubgrupoConta on plano.cd_subgrupo_conta equals sbGrupo.cd_subgrupo_conta
                           where plano.cd_plano_conta == cd_plano_conta
                              && plano.cd_pessoa_empresa == cd_pessoa_empresa
                           select sbGrupo.no_subgrupo_conta).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            } 
        
        
        }
        public List<PlanoConta> getPlanoContaNiveis(int? cd_subGrupoN1, int? cd_subGrupoN2, int[] cdEscolas)
        {
            try
            {
                var sql = (from plano in db.PlanoConta
                           where (plano.cd_subgrupo_conta == cd_subGrupoN1 || plano.cd_subgrupo_conta == cd_subGrupoN2)
                           && cdEscolas.Contains(plano.cd_pessoa_empresa)
                           && plano.id_ativo == true
                           select new {
                               plano.cd_pessoa_empresa,
                               plano.cd_plano_conta
                           }).ToList().Select(x => new PlanoConta
                           {
                               cd_pessoa_empresa = x.cd_pessoa_empresa,
                               cd_plano_conta = x.cd_plano_conta
                           }).ToList(); ;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }
    }
}
