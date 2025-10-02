using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class FichaSaudeDataAccess : GenericRepository<FichaSaude>, IFichaSaudeDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<int> findByAluno(int cdAluno)
        {
            try
            {
                var sql = (from fs in db.FichaSaude
                           where fs.cd_aluno == cdAluno
                           select fs.cd_ficha_saude).ToList();

                return sql;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}