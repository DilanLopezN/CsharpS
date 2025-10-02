using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    public interface IRelacionamentoDataAccess : IGenericRepository<RelacionamentoSGF>
    {
        bool addRelacionamentoResponsavelAluno(RelacionamentoSGF pessoaRelac);
    }
}
