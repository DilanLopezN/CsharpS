using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface IPlanoContasDataAccess : IGenericRepository<PlanoConta>
    {
        IEnumerable<PlanoConta> getPlanoContasSearch(int cd_pessoa_empresa);
        PlanoConta confirmSubGrupoHasPlanoByIdSubgrupo(int cd_sub_grupo, int cd_pessoa_empresa);
        String getDescPlanoContaByEscola(int cd_pessoa_empresa, int cd_plano_conta);
        List<PlanoConta> getPlanoContaNiveis(int? cd_subGrupoN1, int? cd_subGrupoN2, int[] cdEscolas);
    }
}
