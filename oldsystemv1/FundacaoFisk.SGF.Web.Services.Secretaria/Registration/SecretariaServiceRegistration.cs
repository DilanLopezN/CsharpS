using MvcTurbine.ComponentModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.GenericModel;
using System;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAcess;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Registration
{
    public class SecretariaServiceRegistration :IServiceRegistration
    {
        public void Register(IServiceLocator locator)
        {
            //Businness
            locator.Register<ISecretariaBusiness, SecretariaBusiness>();
            locator.Register<IAlunoBusiness, AlunoBusiness>();
            locator.Register<IMatriculaBusiness, MatriculaBusiness>();
            //DataAcces
            locator.Register<IEscolaridadeDataAccess, EscolaridadeDataAccess>();
            locator.Register<IMidiaDataAccess, MidiaDataAccess>();
            locator.Register<ITipoContatoDataAccess, TipoContatoDataAccess>();
            locator.Register<IMotivoMatriculaDataAccess, MotivoMatriculaDataAccess>();
            locator.Register<IMotivoNaoMatriculaDataAccess, MotivoNaoMatriculaDataAccess>();
            locator.Register<IMotivoBolsaDataAccess, MotivoBolsaDataAccess>();
            locator.Register<IMotivoCancelamentoBolsaDataAccess, MotivoCancelamentoBolsaDataAccess>();
            locator.Register<IAlunoDataAccess, AlunoDataAccess>();
            locator.Register<IHorarioDataAccess, HorarioDataAccess>();
            locator.Register<IAlunoMotivoMatriculaDataAccess, AlunoMotivoMatriculaDataAccess>();
            locator.Register<IProspectDataAccess, ProspectDataAccess>();
            locator.Register<IProspectProdutoDataAccess, ProspectProdutoDataAccess>();
            locator.Register<IProspectPeriodoDataAccess, ProspectPeriodoDataAccess>();
            locator.Register<IFollowUpDataAccess, FollowUpDataAccess>();
            locator.Register<IProspectMotivoNaoMatriculaDataAccess, ProspectMotivoNaoMatriculaDataAccess>();
            locator.Register<IMatriculaDataAccess, MatriculaDataAccess>();
            locator.Register<IAlunoTurmaDataAccess, AlunoTurmaDataAccess>();
            locator.Register<INomeContratoDataAccess, NomeContratoDataAccess>();
            locator.Register<IDescontoContratoDataAccess, DescontoContratoDataAccess>();
            locator.Register<ITaxaMatriculaDataAccess, TaxaMatriculaDataAccess>();
            locator.Register<IAditamentoDataAccess, AditamentoDataAccess>();
            locator.Register<IHistoricoAlunoDataAccess, HistoricoAlunoDataAccess>();
            locator.Register<IAlunoBolsaDataAccess, AlunoBolsaDataAccess>();
            locator.Register<IDesistenciaDataAccess, DesistenciaDataAccess>();
            locator.Register<IAcaoFollowupDataAccess, AcaoFollowupDataAccess>();
            locator.Register<IFollowUpEscolaDataAccess, FollowUpEscolaDataAccess>();
            locator.Register<IFollowUpUsuarioDataAccess, FollowUpUsuarioDataAccess>();
            locator.Register<IAnoEscolarDataAccess, AnoEscolarDataAccess>();
        }
    }
}
