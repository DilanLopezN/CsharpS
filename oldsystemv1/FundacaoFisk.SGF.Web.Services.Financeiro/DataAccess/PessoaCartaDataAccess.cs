using System;
using System.Linq;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class PessoaCartaDataAccess : GenericRepository<PessoaCarta>, IPessoaCartaDataAccess
    {
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }


        public PessoaCarta findByIdPessoaAndAno(int idPessoa, int ano)
        {
            try
            {
                var retorno = (from p in db.PessoaCarta
                               where p.cd_pessoa == idPessoa && p.nm_ano_carta == ano
                               select p).FirstOrDefault();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}