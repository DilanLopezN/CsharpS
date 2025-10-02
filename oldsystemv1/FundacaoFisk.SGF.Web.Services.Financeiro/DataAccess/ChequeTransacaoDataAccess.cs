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

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class ChequeTransacaoDataAccess : GenericRepository<ChequeTransacao>, IChequeTransacaoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<ChequeTransacao> getChequeTrasacao(int cd_tran_finan)
        {
            try
            {
                var sql = from c in db.ChequeTransacao.Include(x => x.Cheque)
                                       where c.cd_tran_finan == cd_tran_finan
                                       select c;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}