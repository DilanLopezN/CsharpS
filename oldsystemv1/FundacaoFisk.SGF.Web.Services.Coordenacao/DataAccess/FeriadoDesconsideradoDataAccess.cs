using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
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
    public class FeriadoDesconsideradoDataAccess : GenericRepository<FeriadoDesconsiderado>, IFeriadoDesconsideradoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<FeriadoDesconsiderado> getFeriadoDesconsideradoByTurma(int cd_turma, int cd_escola)
        {
            try
            {
                var result = from fd in db.FeriadoDesconsiderado
                             where fd.cd_turma == cd_turma && fd.Turma.cd_pessoa_escola == cd_escola
                             orderby fd.dt_inicial
                             select fd;
                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
