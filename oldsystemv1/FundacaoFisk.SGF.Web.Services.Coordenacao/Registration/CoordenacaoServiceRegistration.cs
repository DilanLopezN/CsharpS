using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcTurbine.ComponentModel;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;



namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Registration
{
    public class CoordenacaoServiceRegistration : IServiceRegistration
    {
        public void Register(IServiceLocator locator)
        {
            // Business
            locator.Register<ICoordenacaoBusiness, CoordenacaoBusiness>();
            locator.Register<ICursoBusiness, CursoBusiness>();
            locator.Register<IProfessorBusiness, ProfessorBusiness>();
            locator.Register<ITurmaBusiness, TurmaBusiness>();

            //DataAccess
            locator.Register<ISalaDataAccess, SalaDataAccess>();
            locator.Register<IEventoDataAccess, EventoDataAccess>();
            locator.Register<IProdutoDataAccess, ProdutoDataAccess>();
            locator.Register<IDuracaoDataAccess, DuracaoDataAccess>();
            locator.Register<ITipoAtividadeExtraDataAccess, TipoAtividadeExtraDataAccess>();
            locator.Register<IMotivoDesistenciaDataAccess, MotivoDesistenciaDataAccess>();
            locator.Register<IMotivoFaltaDataAccess, MotivoFaltaDataAccess>();
            locator.Register<IModalidadeDataAccess, ModalidadeDataAccess>();
            locator.Register<IRegimeDataAccess, RegimeDataAccess>();
            locator.Register<IConceitoDataAccess, ConceitoDataAccess>();
            locator.Register<IFeriadoDataAccess, FeriadoDataAccess>();
            locator.Register<IEstagioDataAccess, EstagioDataAccess>();
            locator.Register<ICursoDataAccess, CursoDataAccess>();
            locator.Register<ITurmaDataAccess, TurmaDataAccess>();
            locator.Register<ICriterioAvaliacaoDataAccess, CriterioAvaliacaoDataAccess>();
            locator.Register<ITipoAvaliacaoDataAccess, TipoAvaliacaoDataAccess>();
            locator.Register<IAvaliacaoDataAccess, AvaliacaoDataAccess>();
            locator.Register<IProgramacaoCursoDataAccess, ProgramacaoCursoDataAccess>();
            locator.Register<IItemProgramacaoCursoDataAccess, ItemProgramacaoCursoDataAccess>();
            locator.Register<IProfessorDataAccess, ProfessorDataAccess>();
            locator.Register<IAtividadeExtraDataAccess, AtividadeExtraDataAccess>();
            locator.Register<IAtividadeAlunoDataAccess, AtividadeAlunoDataAccess>();
            locator.Register<IAvaliacaoCursoDataAccess, AvaliacaoCursoDataAccess>();
            locator.Register<IAvaliacaoTurmaDataAccess, AvaliacaoTurmaDataAccess>();
            locator.Register<IAvaliacaoAlunoDataAccess, AvaliacaoAlunoDataAccess>();
            locator.Register<IProfessorTurmaDataAccess, ProfessorTurmaDataAccess>();
            locator.Register<IProgramacaoTurmaDataAccess, ProgramacaoTurmaDataAccess>();
            locator.Register<IDiarioAulaDataAccess, DiarioAulaDataAccess>();
            locator.Register<IHorarioProfessorTurmaDataAccess, HorarioProfessorTurmaDataAccess>();
            locator.Register<IFeriadoDesconsideradoDataAccess, FeriadoDesconsideradoDataAccess>();
            locator.Register<IAlunoEventoDataAccess, AlunoEventoDataAccess>();
            locator.Register<IAulaPersonalizadaDataAccess, AulaPersonalizadaDataAccess>();
            locator.Register<IAulaPersonalizadaAlunoDataAccess, AulaPersonalizadaAlunoDataAccess>();
            locator.Register<IAvaliacaoParticipacaoVincDataAccess, AvaliacaoParticipacaoVincDataAccess>();
            locator.Register<IAvaliacaoParticipacaoDataAccess, AvaliacaoParticipacaoDataAccess>();
            locator.Register<IParticipacaoDataAccess, ParticipacaoDataAccess>();
            locator.Register<IAvaliacaoAlunoParticipacaoDataAccess, AvaliacaoAlunoParticipacaoDataAccess>();
            locator.Register<ICargaProfessorDataAccess, CargaProfessorDataAccess>();
        }
    }
}
