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
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class ReajusteTituloDataAccess : GenericRepository<ReajusteTitulo>, IReajusteTituloDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<ReajusteTitulo> getReajusteTitulos(int cd_empresa, int cd_reajuste_anual)
        {
            try
            {
                var sql = from rt in db.ReajusteTitulo
                          where rt.cd_reajuste_anual == cd_reajuste_anual && rt.Titulo.cd_pessoa_empresa == cd_empresa
                          select rt;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
