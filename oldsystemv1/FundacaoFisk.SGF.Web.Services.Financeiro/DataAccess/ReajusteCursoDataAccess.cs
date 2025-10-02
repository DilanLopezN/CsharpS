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
    public class ReajusteCursoDataAccess : GenericRepository<ReajusteCurso>, IReajusteCursoDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public Boolean deleteAllCurso(List<ReajusteCurso> reajustes)
        {
            try
            {
                for (int i = reajustes.Count() - 1; i > 0; i--) 
                {
                    ReajusteCurso reajusteContext = this.findById(reajustes[i].cd_reajuste_curso, false);
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
