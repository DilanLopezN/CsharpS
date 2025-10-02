using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess
{
    public class PapelDataAccess : GenericRepository<PapelSGF>, IPapelDataAccess
    {
         

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<PapelSGF> GetAllPapel()
        {
            try{
                var sql = from c in db.PapelSGF
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PapelSGF> getPapelByTipo(int[] tipo) 
        {
            try{
                var sql = (from c in db.PapelSGF.Include(p => p.PapeisPais)
                           where (tipo).Contains(c.nm_tipo_papel)
                           select new
                           {
                               PapeisPais = c.PapeisPais,
                               cd_papel = c.cd_papel,
                               no_papel = c.no_papel
                           }).ToList().Select(x => new PapelSGF
                           {
                               cd_papel = x.cd_papel,
                               no_papel = x.no_papel,
                               PapeisPais = x.PapeisPais.Select(y => new PapelSGF {
                                   cd_papel = y.cd_papel
                               }).ToList()
                           });

                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<QualifRelacionamento> getAllQualifRelacByPapel(int codPapel)
        {
            try{
                var sql = from qualif in db.QualifRelacionamento
                          where qualif.cd_papel == codPapel
                          select qualif;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PapelSGF getPapelById(int cd_papel) {
            try{
                var sql = (from papel in db.PapelSGF.Include(p => p.PapeisPais).Include(p => p.PapeisFilhos)
                          where papel.cd_papel == cd_papel
                          select papel).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        
    }
}
