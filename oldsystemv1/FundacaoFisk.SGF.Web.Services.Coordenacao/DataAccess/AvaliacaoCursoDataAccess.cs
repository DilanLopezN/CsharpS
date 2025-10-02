using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class AvaliacaoCursoDataAccess : GenericRepository<AvaliacaoCurso>, IAvaliacaoCursoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<AvaliacaoCurso> getAllAvaliacaoCursoByAvalicao(int cdTipoAvaliacao)
        {
            try{
                var sql = from avalCurso in db.AvaliacaoCurso
                          where avalCurso.cd_tipo_avaliacao == cdTipoAvaliacao
                          select avalCurso;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
