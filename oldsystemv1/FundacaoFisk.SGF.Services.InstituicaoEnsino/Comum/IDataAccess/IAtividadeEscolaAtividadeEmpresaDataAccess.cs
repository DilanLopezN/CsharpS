using System;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Services.InstituicaoEnsino.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;

namespace FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IDataAccess
{
    public interface IAtividadeEscolaAtividadeEmpresaDataAccess : IGenericRepository<AtividadeEscolaAtividade>
    {
        IEnumerable<AtividadeEscolaAtividadeSearchUI> getAtividadeEscolatWithAtividade(int cd_atividade_extra);
        
    }
}