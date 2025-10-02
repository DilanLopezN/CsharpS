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
    public class ChequeBaixaDataAccess : GenericRepository<ChequeBaixa>, IChequeBaixaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }
    }
}