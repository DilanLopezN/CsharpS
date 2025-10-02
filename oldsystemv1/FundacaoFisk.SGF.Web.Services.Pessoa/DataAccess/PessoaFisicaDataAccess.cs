using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.Utils;
using System.Data.Entity;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess
{
    using Componentes.GenericDataAccess;
    using Componentes.GenericDataAccess.GenericRepository;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
    using log4net;
    using Componentes.GenericDataAccess.GenericException;

    public class PessoaFisicaDataAccess : GenericRepository<PessoaFisicaSGF>, IPessoaFisicaDataAccess
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PessoaDataAccess));

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

    }
}
