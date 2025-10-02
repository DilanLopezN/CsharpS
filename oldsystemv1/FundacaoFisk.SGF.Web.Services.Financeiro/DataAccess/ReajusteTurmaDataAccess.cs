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
    public class ReajusteTurmaDataAccess : GenericRepository<ReajusteTurma>, IReajusteTurmaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public Boolean deleteAllTurma(List<ReajusteTurma> reajustes)
        {
            try
            {
                for (int i = reajustes.Count() - 1; i > 0; i--) 
                {
                    ReajusteTurma reajusteContext = this.findById(reajustes[i].cd_reajuste_turma, false);
                    this.deleteContext(reajusteContext, false);
                }
                return this.saveChanges(false) > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
