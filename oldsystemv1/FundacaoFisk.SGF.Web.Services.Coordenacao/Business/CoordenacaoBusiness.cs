using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
using System.Data.Entity;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using Componentes.GenericBusiness.Excepion;
using Newtonsoft.Json;
using System.Data.Entity.Infrastructure;
using FundacaoFisk.SGF.Web.Services.Log.Comum.IBusiness;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Business
{
    public class CoordenacaoBusiness : ICoordenacaoBusiness
    {
        const int INCLUIR = 1, EDITAR = 2;

        /// <summary>
        /// Declaração de Interfaces
        /// </summary>
        public IEstagioDataAccess daoEstagio { get; set; }

        public IConceitoDataAccess daoConceito { get; set; }
        public IDuracaoDataAccess daoDuracao { get; set; }
        public IEventoDataAccess daoEvento { get; set; }
        public IModalidadeDataAccess daoModalidade { get; set; }
        public IMotivoDesistenciaDataAccess daoMotivoDes { get; set; }
        public IMotivoFaltaDataAccess daoMotivoFalta { get; set; }
        public IProdutoDataAccess daoProduto { get; set; }
        public IRegimeDataAccess daoRegime { get; set; }
        public ISalaDataAccess daoSala { get; set; }
        public IControleFaltasDataAccess daoControleFaltas { get; set; }
        public IControleFaltasAlunoDataAccess daoControleFaltasAluno { get; set; }
        public ITipoAtividadeExtraDataAccess daoTipoAtv { get; set; }
        public ICriterioAvaliacaoDataAccess daoCriterio { get; set; }
        public ITipoAvaliacaoDataAccess daoTipoAvaliacao { get; set; }
        public IProgramacaoCursoDataAccess daoProgramacaoCurso { get; set; }
        public IAtividadeExtraDataAccess daoAtividadeExtra { get; set; }
        public IAtividadeAlunoDataAccess daoAtividadeAluno { get; set; }
        public IAtividadeCursoDataAccess daoAtividadeCurso { get; set; }
        public IAvaliacaoCursoDataAccess daoAvalCurso { get; set; }
        public ITurmaBusiness turmaBusiness { get; set; }
        public IMatriculaBusiness matriculaBusiness { get; set; }
        public ICursoBusiness cursoBiz { get; set; }
        public ISecretariaBusiness secretariaBiz { get; set; }
        public IAlunoEventoDataAccess daoAlunoEvento { get; set; }
        public IProgramacaoTurmaDataAccess daoProgramacaoTurma { get; set; }
        public IItemProgramacaoCursoDataAccess daoItemProgramacaoCurso { get; set; }
        public IAulaPersonalizadaDataAccess daoAulaPersonalizada { get; set; }
        public IAulaPersonalizadaAlunoDataAccess daoAulaPersonalizadaAluno { get; set; }
        public IAvaliacaoParticipacaoDataAccess daoAvaliacaoParticipacao { get; set; }
        public IAvaliacaoParticipacaoVincDataAccess daoAvaliacaoParticipacaoVinc { get; set; }
        public IParticipacaoDataAccess daoParticipacao { get; set; }
        public INivelDataAccess daoNivel { get; set; }
        public IVideoDataAccess daoVideo { get; set; }
        public IFaqDataAccess daoFaq { get; set; }
        public ICargaProfessorDataAccess daoCargaProfessor { get; set; }
        public ICalendarioEvento daoCalendarioEvento { get; set; }
        public ICalendarioAcademico daoCalendarioAcademico { get; set; }
        public IAvaliacaoAlunoParticipacaoDataAccess daoAvaliacaoAlunoParticipacao { get; set; }
        public IAvaliacaoDataAccess daoAvaliacao { get; set; }
        public IAvaliacaoTurmaDataAccess daoAvaliacaoTurma { get; set; }
        public ITurmaEscolaDataAccess DataAccessTurmaEscola { get; set; }

        //Depência com a Instituição de Ensino.
        //Depência com a Pessoa Compononetes.
        private IFinanceiroBusiness financeiroBusiness { get; set; }
        private IAlunoBusiness alunoBusiness { get; set; }
        public ICircular daoCircular { get; set; }
        public ITituloAditamento DataAccessTituloAditamento { get; set; }
        public IAulaReposicaoDataAccess daoAulaReposicao { get; set; }
        public IAlunoAulaReposicaoDataAccess daoAlunoAulaReposicao { get; set; }
        public ITurmaEscolaDataAccess daoTurmaEscola { get; set; }
        public IAtividadeEscolaAtividadeDataAccess daoAtividadeEscola { get; set; }
        public IAtividadeProspectDataAccess daoAtividadeProspect { get; set; }
        public IAtividadeRecorrenciaDataAccess daoAtividadeRecorrencia { get; set; }
        public IMensagemAvaliacaoDataAccess daoMensagemAvaliacao { get; set; }
        public IMensagemAvaliacaoAlunoDataAccess daoMensagemAvaliacaoAluno { get; set; }

        public IApiNewCyberBusiness businessApiNewCyber { get; set; }
        public IPerdaMaterialDataAccess daoPerdaMaterial { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DaoEstagio"></param>
        /// <param name="DaoConceito"></param>
        /// <param name="DaoDuracao"></param>
        /// <param name="DaoEvento"></param>
        /// <param name="DaoModalidade"></param>
        /// <param name="DaoMotivoDes"></param>
        /// <param name="DaoMotivoFalta"></param>
        /// <param name="DaoProduto"></param>
        /// <param name="DaoRegime"></param>
        /// <param name="DaoSala"></param>
        /// <param name="DaoControleFaltas"></param>
        /// <param name="DaoControleFaltasAluno"></param>
        /// <param name="DaoTipoAtv"></param>
        /// <param name="DaoFeriado"></param>
        /// <param name="DaoCriterio"></param>
        /// <param name="DaoTipoAvaliacao"></param>
        /// <param name="DaoNivel"></param>
        /// <param name="DaoProgramacaoCurso"></param>
        /// <param name="DaoAtividadeExtra"></param>
        /// <param name="DaoAtividadeCurso"></param>
        /// <param name="DaoAtividadeAluno"></param>
        /// <param name="DaoAvalCurso"></param>
        /// <param name="FinanceiroBusiness"></param>
        /// <param name="AlunoBusiness"></param>
        /// <param name="TurmaBusiness"></param>
        /// <param name="ProfessorBusiness"></param>
        /// <param name="CursoBiz"></param>
        /// <param name="SecretariaBiz"></param>
        /// <param name="DaoDiarioAula"></param>
        /// <param name="AlunoEventoDataAccess"></param>
        /// <param name="ProgramacaoTurmaDataAccess"></param>
        /// <param name="matriculaBusiness"></param>
        public CoordenacaoBusiness
        (
            IEstagioDataAccess DaoEstagio, IConceitoDataAccess DaoConceito, IDuracaoDataAccess DaoDuracao,
            IEventoDataAccess DaoEvento,
            IModalidadeDataAccess DaoModalidade, IMotivoDesistenciaDataAccess DaoMotivoDes,
            IMotivoFaltaDataAccess DaoMotivoFalta,
            IProdutoDataAccess DaoProduto, IRegimeDataAccess DaoRegime, ISalaDataAccess DaoSala,
            IControleFaltasDataAccess DaoControleFaltas, IControleFaltasAlunoDataAccess DaoControleFaltasAluno,
            ITipoAtividadeExtraDataAccess DaoTipoAtv,
            ICriterioAvaliacaoDataAccess DaoCriterio, ITipoAvaliacaoDataAccess DaoTipoAvaliacao,
            IProgramacaoCursoDataAccess DaoProgramacaoCurso, IAvaliacaoTurmaDataAccess daoAvaliacaoTurma,
            IAtividadeExtraDataAccess DaoAtividadeExtra, IAtividadeAlunoDataAccess DaoAtividadeAluno,
            IAtividadeCursoDataAccess DaoAtividadeCurso, IAvaliacaoCursoDataAccess DaoAvalCurso,
            IFinanceiroBusiness FinanceiroBusiness, IAlunoBusiness AlunoBusiness,
            ITurmaBusiness TurmaBusiness, ICursoBusiness CursoBiz, ISecretariaBusiness SecretariaBiz,
            IAvaliacaoDataAccess daoAvaliacao,
            IAlunoEventoDataAccess AlunoEventoDataAccess, IProgramacaoTurmaDataAccess ProgramacaoTurmaDataAccess,
            IMatriculaBusiness matriculaBusiness, IItemProgramacaoCursoDataAccess DaoItemProgramacaoCurso,
            IAulaPersonalizadaDataAccess DaoAulaPersonalizada,
            IAulaPersonalizadaAlunoDataAccess DaoAulaPersonalizadaAluno,
            IAvaliacaoParticipacaoDataAccess DaoAvaliacaoParticipacao,
            IAvaliacaoParticipacaoVincDataAccess DaoAvaliacaoParticipacaoVinc, IParticipacaoDataAccess DaoParticipacao,
            IVideoDataAccess daoVideo, INivelDataAccess daoNivel, IFaqDataAccess daoFaq,
            ICalendarioEvento DaoCalendarioEvento, ICalendarioAcademico DaoCalendarioAcademico,
            ICargaProfessorDataAccess DaoCargaProfessor,
            IAvaliacaoAlunoParticipacaoDataAccess daoAvaliacaoAlunoParticipacao,
            ICircular DaoCircular, ITituloAditamento dataAccessTituloAditamento,
            IAulaReposicaoDataAccess DaoAulaReposicao,
            IAlunoAulaReposicaoDataAccess DaoAlunoAulaReposicao,
            ITurmaEscolaDataAccess DaoTurmaEscola,
            IAtividadeEscolaAtividadeDataAccess DaoAtividadeEscola,
            IAtividadeProspectDataAccess DaoAtividadeProspect,
            IAtividadeRecorrenciaDataAccess DaoAtividadeRecorrencia,
            IApiNewCyberBusiness BusinessApiNewCyber,
            IMensagemAvaliacaoDataAccess DaoMensagemAvaliacao,
            IMensagemAvaliacaoAlunoDataAccess DaoMensagemAvaliacaoAluno,
            IPerdaMaterialDataAccess DaoPerdaMaterial

        )
        {
            if (DaoEstagio == null || DaoConceito == null || DaoDuracao == null || DaoEvento == null ||
                DaoModalidade == null ||
                DaoMotivoDes == null || DaoMotivoFalta == null || DaoProduto == null || DaoRegime == null ||
                DaoControleFaltas == null || DaoControleFaltasAluno == null || DaoTipoAtv == null ||
                DaoCriterio == null || DaoTipoAvaliacao == null || daoAvaliacaoTurma == null ||
                DaoProgramacaoCurso == null || DaoAtividadeExtra == null || DaoAtividadeAluno == null ||
                DaoAtividadeCurso == null ||
                DaoAvalCurso == null || FinanceiroBusiness == null || AlunoBusiness == null ||
                TurmaBusiness == null || CursoBiz == null || SecretariaBiz == null || daoAvaliacao == null ||
                AlunoEventoDataAccess == null || ProgramacaoTurmaDataAccess == null || matriculaBusiness == null ||
                DaoItemProgramacaoCurso == null ||
                DaoAulaPersonalizada == null || DaoAulaPersonalizadaAluno == null || DaoAvaliacaoParticipacao == null ||
                DaoAvaliacaoParticipacaoVinc == null || DaoParticipacao == null || daoVideo == null ||
                daoNivel == null || daoFaq == null ||
                DaoCargaProfessor == null || DaoCalendarioEvento == null || DaoCalendarioAcademico == null ||
                daoAvaliacaoAlunoParticipacao == null || DaoCircular == null || dataAccessTituloAditamento == null ||
                DaoAulaReposicao == null || DaoAlunoAulaReposicao == null || DaoTurmaEscola == null || DaoAtividadeEscola == null ||
                DaoAtividadeProspect == null || DaoAtividadeRecorrencia == null || BusinessApiNewCyber == null ||
                DaoMensagemAvaliacao == null || DaoMensagemAvaliacaoAluno == null || DaoPerdaMaterial == null)
            {
                throw new ArgumentNullException("DAO");
            }

            daoEstagio = DaoEstagio;
            daoConceito = DaoConceito;
            daoDuracao = DaoDuracao;
            daoEvento = DaoEvento;
            daoModalidade = DaoModalidade;
            daoMotivoDes = DaoMotivoDes;
            daoMotivoFalta = DaoMotivoFalta;
            daoProduto = DaoProduto;
            daoRegime = DaoRegime;
            daoSala = DaoSala;
            daoControleFaltas = DaoControleFaltas;
            daoControleFaltasAluno = DaoControleFaltasAluno;
            daoTipoAtv = DaoTipoAtv;
            daoCriterio = DaoCriterio;
            daoTipoAvaliacao = DaoTipoAvaliacao;
            daoProgramacaoCurso = DaoProgramacaoCurso;
            daoAtividadeExtra = DaoAtividadeExtra;
            daoAtividadeAluno = DaoAtividadeAluno;
            daoAtividadeCurso = DaoAtividadeCurso;
            this.daoAvalCurso = DaoAvalCurso;
            financeiroBusiness = FinanceiroBusiness;
            alunoBusiness = AlunoBusiness;
            turmaBusiness = TurmaBusiness;
            cursoBiz = CursoBiz;
            secretariaBiz = SecretariaBiz;
            daoAlunoEvento = AlunoEventoDataAccess;
            daoProgramacaoTurma = ProgramacaoTurmaDataAccess;
            daoAulaPersonalizada = DaoAulaPersonalizada;
            daoAulaPersonalizadaAluno = DaoAulaPersonalizadaAluno;
            daoAvaliacaoParticipacao = DaoAvaliacaoParticipacao;
            daoAvaliacaoParticipacaoVinc = DaoAvaliacaoParticipacaoVinc;
            daoParticipacao = DaoParticipacao;
            this.daoVideo = daoVideo;
            this.daoNivel = daoNivel;
            this.daoFaq = daoFaq;
            daoCargaProfessor = DaoCargaProfessor;
            daoCalendarioEvento = DaoCalendarioEvento;
            daoCalendarioAcademico = DaoCalendarioAcademico;
            this.matriculaBusiness = matriculaBusiness;
            this.daoItemProgramacaoCurso = DaoItemProgramacaoCurso;
            this.daoAvaliacaoAlunoParticipacao = daoAvaliacaoAlunoParticipacao;
            this.daoAvaliacao = daoAvaliacao;
            this.daoAvaliacaoTurma = daoAvaliacaoTurma;
            daoCircular = DaoCircular;
            this.DataAccessTituloAditamento = dataAccessTituloAditamento;
            daoAulaReposicao = DaoAulaReposicao;
            daoAlunoAulaReposicao = DaoAlunoAulaReposicao;
            daoTurmaEscola = DaoTurmaEscola;
            daoAtividadeEscola = DaoAtividadeEscola;
            daoAtividadeProspect = DaoAtividadeProspect;
            daoAtividadeRecorrencia = DaoAtividadeRecorrencia;
            businessApiNewCyber = BusinessApiNewCyber;
            daoMensagemAvaliacao = DaoMensagemAvaliacao;
            daoMensagemAvaliacaoAluno = DaoMensagemAvaliacaoAluno;
            daoPerdaMaterial = DaoPerdaMaterial;
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa)
        {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.daoEstagio.DB()).IdUsuario = ((SGFWebContext)this.daoConceito.DB()).IdUsuario =
                ((SGFWebContext)this.daoDuracao.DB()).IdUsuario =
                ((SGFWebContext)this.daoEvento.DB()).IdUsuario = ((SGFWebContext)this.daoModalidade.DB()).IdUsuario =
                ((SGFWebContext)this.daoMotivoDes.DB()).IdUsuario =
                ((SGFWebContext)this.daoMotivoFalta.DB()).IdUsuario =
                ((SGFWebContext)this.daoProduto.DB()).IdUsuario =
                ((SGFWebContext)this.daoAtividadeAluno.DB()).IdUsuario =
                ((SGFWebContext)this.daoRegime.DB()).IdUsuario = ((SGFWebContext)this.daoSala.DB()).IdUsuario =
                ((SGFWebContext)this.daoControleFaltas.DB()).IdUsuario =
                ((SGFWebContext)this.daoControleFaltasAluno.DB()).IdUsuario =
                ((SGFWebContext)this.daoProgramacaoCurso.DB()).IdUsuario =
                ((SGFWebContext)this.daoTipoAtv.DB()).IdUsuario = ((SGFWebContext)this.daoCriterio.DB()).IdUsuario =
                ((SGFWebContext)this.daoAtividadeExtra.DB()).IdUsuario =
                ((SGFWebContext)this.daoTipoAvaliacao.DB()).IdUsuario =
                ((SGFWebContext)this.daoAvalCurso.DB()).IdUsuario =
                ((SGFWebContext)this.daoProgramacaoTurma.DB()).IdUsuario =
                ((SGFWebContext)this.daoItemProgramacaoCurso.DB()).IdUsuario =
                ((SGFWebContext)this.daoAulaPersonalizada.DB()).IdUsuario =
                ((SGFWebContext)this.daoAulaPersonalizadaAluno.DB()).IdUsuario =
                ((SGFWebContext)this.daoAvaliacao.DB()).IdUsuario =
                ((SGFWebContext)this.daoAvaliacaoParticipacao.DB()).IdUsuario =
                ((SGFWebContext)this.daoAvaliacaoParticipacaoVinc.DB()).IdUsuario =
                ((SGFWebContext)this.daoAvaliacaoTurma.DB()).IdUsuario =
                ((SGFWebContext)this.daoParticipacao.DB()).IdUsuario =
                ((SGFWebContext)this.daoCargaProfessor.DB()).IdUsuario =
                ((SGFWebContext)this.daoCalendarioEvento.DB()).IdUsuario =
                ((SGFWebContext)this.daoVideo.DB()).IdUsuario =
                ((SGFWebContext)this.daoNivel.DB()).IdUsuario =
                ((SGFWebContext)this.daoFaq.DB()).IdUsuario =
                ((SGFWebContext)this.daoAvaliacaoAlunoParticipacao.DB()).IdUsuario =
                ((SGFWebContext)this.daoCircular.DB()).IdUsuario =
                ((SGFWebContext)DataAccessTituloAditamento.DB()).IdUsuario =
                ((SGFWebContext)this.daoAulaReposicao.DB()).IdUsuario =
                ((SGFWebContext)this.daoAlunoAulaReposicao.DB()).IdUsuario =
                ((SGFWebContext)this.daoTurmaEscola.DB()).IdUsuario =
                ((SGFWebContext)this.daoAtividadeEscola.DB()).IdUsuario =
                ((SGFWebContext)this.daoAtividadeProspect.DB()).IdUsuario =
                ((SGFWebContext)this.daoAtividadeRecorrencia.DB()).IdUsuario = 
                ((SGFWebContext)this.daoMensagemAvaliacao.DB()).IdUsuario = 
                ((SGFWebContext)this.daoMensagemAvaliacaoAluno.DB()).IdUsuario  =
                ((SGFWebContext)this.daoPerdaMaterial.DB()).IdUsuario = cdUsuario;

            ((SGFWebContext)this.daoEstagio.DB()).cd_empresa = ((SGFWebContext)this.daoConceito.DB()).cd_empresa =
                ((SGFWebContext)this.daoDuracao.DB()).cd_empresa =
                ((SGFWebContext)this.daoEvento.DB()).cd_empresa =
                ((SGFWebContext)this.daoModalidade.DB()).cd_empresa =
                ((SGFWebContext)this.daoMotivoDes.DB()).cd_empresa =
                ((SGFWebContext)this.daoMotivoFalta.DB()).cd_empresa =
                ((SGFWebContext)this.daoProduto.DB()).cd_empresa =
                ((SGFWebContext)this.daoAtividadeAluno.DB()).cd_empresa =
                ((SGFWebContext)this.daoRegime.DB()).cd_empresa = ((SGFWebContext)this.daoSala.DB()).cd_empresa =
                ((SGFWebContext)this.daoControleFaltas.DB()).cd_empresa =
                ((SGFWebContext)this.daoControleFaltasAluno.DB()).cd_empresa =
                ((SGFWebContext)this.daoProgramacaoCurso.DB()).cd_empresa =
                ((SGFWebContext)this.daoTipoAtv.DB()).cd_empresa = ((SGFWebContext)this.daoCriterio.DB()).cd_empresa =
                ((SGFWebContext)this.daoAtividadeExtra.DB()).cd_empresa =
                ((SGFWebContext)this.daoTipoAvaliacao.DB()).cd_empresa =
                ((SGFWebContext)this.daoAvaliacao.DB()).cd_empresa =
                ((SGFWebContext)this.daoAvalCurso.DB()).cd_empresa =
                ((SGFWebContext)this.daoProgramacaoTurma.DB()).cd_empresa =
                ((SGFWebContext)this.daoItemProgramacaoCurso.DB()).cd_empresa =
                ((SGFWebContext)this.daoAulaPersonalizada.DB()).cd_empresa =
                ((SGFWebContext)this.daoAulaPersonalizadaAluno.DB()).cd_empresa =
                ((SGFWebContext)this.daoAvaliacaoParticipacao.DB()).cd_empresa =
                ((SGFWebContext)this.daoAvaliacaoParticipacaoVinc.DB()).cd_empresa =
                ((SGFWebContext)this.daoAvaliacaoTurma.DB()).cd_empresa =
                ((SGFWebContext)this.daoParticipacao.DB()).cd_empresa =
                ((SGFWebContext)this.daoCargaProfessor.DB()).cd_empresa =
                ((SGFWebContext)this.daoVideo.DB()).cd_empresa =
                ((SGFWebContext)this.daoNivel.DB()).cd_empresa =
                ((SGFWebContext)this.daoFaq.DB()).cd_empresa =
                ((SGFWebContext)this.daoCalendarioEvento.DB()).cd_empresa =
                ((SGFWebContext)this.daoAvaliacaoAlunoParticipacao.DB()).cd_empresa =
                ((SGFWebContext)this.daoCircular.DB()).cd_empresa =
                ((SGFWebContext)DataAccessTituloAditamento.DB()).cd_empresa =
                ((SGFWebContext)this.daoAulaReposicao.DB()).cd_empresa =
                ((SGFWebContext)this.daoAlunoAulaReposicao.DB()).cd_empresa =
                ((SGFWebContext)this.daoTurmaEscola.DB()).cd_empresa =
                ((SGFWebContext)this.daoAtividadeEscola.DB()).cd_empresa =
                ((SGFWebContext)this.daoAtividadeProspect.DB()).cd_empresa =
                ((SGFWebContext)this.daoAtividadeRecorrencia.DB()).cd_empresa = 
                ((SGFWebContext)this.daoMensagemAvaliacao.DB()).cd_empresa = 
                ((SGFWebContext)this.daoMensagemAvaliacaoAluno.DB()).cd_empresa =
                ((SGFWebContext)this.daoPerdaMaterial.DB()).cd_empresa = cd_empresa;

            financeiroBusiness.configuraUsuario(cdUsuario, cd_empresa);
            alunoBusiness.configuraUsuario(cdUsuario, cd_empresa);
            turmaBusiness.configuraUsuario(cdUsuario, cd_empresa);
            cursoBiz.configuraUsuario(cdUsuario, cd_empresa);
            secretariaBiz.configuraUsuario(cdUsuario, cd_empresa);
            matriculaBusiness.configuraUsuario(cdUsuario, cd_empresa);
        }

        public void sincronizarContextos(DbContext dbContext)
        {
            //this.daoAlunoEvento.sincronizaContexto(dbContext);
            //this.daoEstagio.sincronizaContexto(dbContext);
            //this.daoConceito.sincronizaContexto(dbContext);
            //this.daoDuracao.sincronizaContexto(dbContext);
            //this.daoEvento.sincronizaContexto(dbContext);
            //this.daoModalidade.sincronizaContexto(dbContext);
            //this.daoMotivoDes.sincronizaContexto(dbContext);
            //this.daoMotivoFalta.sincronizaContexto(dbContext);
            //this.daoProduto.sincronizaContexto(dbContext);
            //this.daoSala.sincronizaContexto(dbContext);
            //this.daoRegime.sincronizaContexto(dbContext);
            //this.daoTipoAtv.sincronizaContexto(dbContext);
            //this.daoCriterio.sincronizaContexto(dbContext);
            //this.daoTipoAvaliacao.sincronizaContexto(dbContext);
            //this.daoProgramacaoCurso.sincronizaContexto(dbContext);
            //this.daoAtividadeExtra.sincronizaContexto(dbContext);
            //this.daoAvalCurso.sincronizaContexto(dbContext);
            //this.daoAtividadeAluno.sincronizaContexto(dbContext);
            //this.daoItemProgramacaoCurso.sincronizaContexto(dbContext);
            //this.daoProgramacaoTurma.sincronizaContexto(dbContext);
            //this.financeiroBusiness.sincronizarContextos(dbContext);
            //this.alunoBusiness.sincronizaContexto(dbContext);
            //this.turmaBusiness.sincronizaContexto(dbContext);
            //this.cursoBiz.sincronizarContexto(dbContext);
            //this.secretariaBiz.sincronizarContextos(dbContext);
            //this.matriculaBusiness.sincronizarContextos(dbContext);
            //this.daoAulaPersonalizada.sincronizaContexto(dbContext);
            //this.daoAulaPersonalizadaAluno.sincronizaContexto(dbContext);
            //this.daoAvaliacaoParticipacao.sincronizaContexto(dbContext);
            //this.daoAvaliacaoParticipacaoVinc.sincronizaContexto(dbContext);
            //this.daoParticipacao.sincronizaContexto(dbContext);
            //this.daoCargaProfessor.sincronizaContexto(dbContext);
            //this.daoAvaliacaoAlunoParticipacao.sincronizaContexto(dbContext);
            //this.daoAvaliacao.sincronizaContexto(dbContext);
            //this.daoAvaliacaoTurma.sincronizaContexto(dbContext);
        }

        #region ProgramacaoTurmaCurso

        public List<ItemProgramacao> geraModeloProgramacoesTurma(ProgramacaoHorarioUI programacaoUI, int cd_escola)
        {
            List<ItemProgramacao> retorno = new List<ItemProgramacao>();
            this.sincronizarContextos(daoItemProgramacaoCurso.DB());

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.COMMITED, daoItemProgramacaoCurso.DB(),
                TransactionScopeBuilder.TransactionTime.HIGHT))
            {
                if (programacaoUI != null && programacaoUI.programacoes != null)
                    retorno = this.geraModeloProgramacoesTurma(programacaoUI.cd_turma.Value, cd_escola,
                        programacaoUI.programacoes.ToList());
                daoItemProgramacaoCurso.saveChanges(false);
                transaction.Complete();
            }

            return retorno;
        }

        public void geraModeloProgramacoesTurma(List<int> cd_turmas, int cd_escola)
        {
            this.sincronizarContextos(daoItemProgramacaoCurso.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.COMMITED, daoItemProgramacaoCurso.DB(),
                TransactionScopeBuilder.TransactionTime.HIGHT))
            {
                //Pesquisa as programações das turmas:
                foreach (int cd_turma in cd_turmas)
                {
                    List<ProgramacaoTurma> programacoesTurma = turmaBusiness
                        .getProgramacaoTurmaByTurma(TransactionScopeBuilder.TransactionType.COMMITED, cd_turma,
                            cd_escola, ProgramacaoTurmaDataAccess.TipoConsultaProgTurmaEnum.HAS_PROG_TURMA, false)
                        .ToList();

                    if (programacoesTurma.Count <= 0)
                        throw new TurmaBusinessException(Messages.msgErroTurmaSemProgramacaoTurma, null,
                            TurmaBusinessException.TipoErro.ERRO_TURMA_SEM_PROGRAMACAO_TURMA, false);

                    this.geraModeloProgramacoesTurma(cd_turma, cd_escola, programacoesTurma);
                }

                daoItemProgramacaoCurso.saveChanges(false);
                transaction.Complete();
            }
        }

        private List<ItemProgramacao> geraModeloProgramacoesTurma(int cd_turma, int cd_escola,
            List<ProgramacaoTurma> programacoesTurma)
        {
            ProgramacaoCurso programacaoCurso = new ProgramacaoCurso();
            if (programacoesTurma.Count > 0)
            {
                //Se já há programação de modelo na turma emite erro:
                if (daoProgramacaoCurso.existeModeloProgramacaoByTurma(cd_turma, cd_escola))
                    //daoItemProgramacaoCurso.deleteAllItemProgramacaoCurso(programacaoCurso.ItemProgramacao.ToList());
                    throw new TurmaBusinessException(Messages.msgErroModeloProgramacaoTurmaExistente, null,
                        TurmaBusinessException.TipoErro.ERRO_MODELO_PROGRAMACAO_TURMA_EXISTENTE, false);
                else
                {
                    //Cria nova programacao do curso:
                    programacaoCurso = new ProgramacaoCurso();
                    programacaoCurso.cd_escola = cd_escola;

                    Turma turma = turmaBusiness.getTurmaComCursoDuracao(cd_turma);

                    if (turma == null || !turma.cd_curso.HasValue)
                        throw new TurmaBusinessException(Messages.msgErroModeloProgramacaoTurmaPPTPai, null,
                            TurmaBusinessException.TipoErro.ERRO_MODELO_PROGRAMACAO_TURMA_PPT_PAI, false);
                    programacaoCurso.cd_curso = turma.cd_curso.Value;
                    programacaoCurso.cd_duracao = turma.cd_duracao;

                    daoProgramacaoCurso.add(programacaoCurso, false);
                }

                //Cria novamente:
                List<ItemProgramacao> novosModelosProgramacao = new List<ItemProgramacao>();
                byte nm_aula_programacao_turma = 1;
                foreach (ProgramacaoTurma programacaoTurma in programacoesTurma)
                {
                    ItemProgramacao novoModeloProgramacao = new ItemProgramacao();

                    if (!programacaoTurma.cd_feriado.HasValue || programacaoTurma.cd_feriado_desconsiderado.HasValue)
                    {
                        novoModeloProgramacao.cd_programacao_curso = programacaoCurso.cd_programacao_curso;
                        novoModeloProgramacao.dc_aula_programacao = programacaoTurma.dc_programacao_turma;
                        novoModeloProgramacao.nm_aula_programacao = nm_aula_programacao_turma;

                        //daoItemProgramacaoCurso.addItemProgramacao(novoModeloProgramacao);
                        programacaoCurso.ItemProgramacao.Add(novoModeloProgramacao);

                        nm_aula_programacao_turma += 1;
                    }
                }
            }

            return programacaoCurso.ItemProgramacao.ToList();
        }

        public void criaProgramacoesTurmaCurso(List<int> cd_turmas, int cd_escola)
        {
            criaProgramacoesTurmaCurso(cd_turmas, cd_escola, false);
        }

        public void criaProgramacoesTurmaCurso(List<int> cd_turmas, int cd_escola, bool modelo)
        {
            ProgramacaoHorarioUI programacao = new ProgramacaoHorarioUI();

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.COMMITED, daoProgramacaoCurso.DB(),
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                this.sincronizarContextos(daoProgramacaoCurso.DB());
                for (int i = 0; i < cd_turmas.Count; i++)
                {
                    Turma turmaContext = turmaBusiness.findTurmasByIdAndCdEscola(cd_turmas[i], cd_escola);
                    if (turmaContext.dt_termino_turma.HasValue)
                        throw new TurmaBusinessException(Messages.msgErrorProgramacaoTurmaEncerrada, null,
                            TurmaBusinessException.TipoErro.ERRO_TURMA_ENCERRADA, false);

                    programacao = turmaBusiness.getProgramacaoHorarioTurma(cd_turmas[i], cd_escola);
                    if (programacao != null)
                        turmaBusiness.atualizaProgramacoesTurma(TransactionScopeBuilder.TransactionType.COMMITED,
                            criaProgramacaoTurmaCurso(programacao, cd_escola, modelo), cd_turmas[i], null);
                }

                transaction.Complete();
            }
        }

        public List<ProgramacaoTurma> criaProgramacaoTurmaCurso(ProgramacaoHorarioUI programacao, int cd_escola,
            bool? modelo)
        {
            //Busca as programações do curso:
            List<ProgramacaoTurma> retorno = new List<ProgramacaoTurma>();

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.COMMITED, daoProgramacaoCurso.DB(),
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                this.sincronizarContextos(daoProgramacaoCurso.DB());
                if (!programacao.cd_curso.HasValue || !programacao.cd_duracao.HasValue)
                    throw new ProgramacaoTurmaBusinessException(Messages.msgNaoExisteCurso, null,
                        ProgramacaoTurmaBusinessException.TipoErro.ERRO_NAO_TEM_PROGRAMACAO_CURSO, false);

                ProgramacaoCurso programacoes = daoProgramacaoCurso.getProgramacao(programacao.cd_curso.Value,
                    programacao.cd_duracao.Value, (modelo.HasValue && modelo.Value) ? (int?)cd_escola : null);

                if (programacoes != null && programacoes.ItemProgramacao != null &&
                    programacoes.ItemProgramacao.Count > 0)
                {
                    IEnumerable<Feriado> feriadosEscola = null;
                    programacoes.ItemProgramacao =
                        programacoes.ItemProgramacao.OrderBy(p => p.nm_aula_programacao).ToList();

                    if (programacao.programacoes == null)
                        programacao.programacoes = new List<ProgramacaoTurma>();
                    foreach (ItemProgramacao p in programacoes.ItemProgramacao)
                    {
                        var novas_programacoes = turmaBusiness.criaProgramacaoTurma(programacao, cd_escola,
                            p.dc_aula_programacao, ref feriadosEscola);

                        foreach (ProgramacaoTurma pt in novas_programacoes)
                        {
                            pt.dta_cadastro_programacao = DateTime.UtcNow;
                            if (modelo.HasValue && modelo.Value)
                                pt.id_programacao_manual =
                                    (byte)ProgramacaoTurma.TipoProgramacaoManual.PROGRAMACAO_GERADA_MODELO;
                            else
                                pt.id_programacao_manual =
                                    (byte)ProgramacaoTurma.TipoProgramacaoManual.PROGRAMACAO_GERADA_CURSO;
                        }

                        retorno.AddRange(novas_programacoes);

                        var programacoesAnteriores = programacao.programacoes.ToList();
                        programacoesAnteriores.AddRange(novas_programacoes);
                        programacao.programacoes = programacoesAnteriores;
                    }
                }
                else
                    throw new ProgramacaoTurmaBusinessException(
                        (modelo.HasValue && modelo.Value)
                            ? Messages.msgErroNaoPossuiModeloProgramacao
                            : Messages.msgErroNaoPossuiProgramacaoCurso, null,
                        ProgramacaoTurmaBusinessException.TipoErro.ERRO_NAO_TEM_PROGRAMACAO_CURSO, false);

                transaction.Complete();
            }

            return retorno;
        }
        public void criaProgramacoesTurmaTurma(int cd_turma_origem, int cd_turma_destino, int cd_escola, bool cancelar)
        {
            ProgramacaoHorarioUI programacao = new ProgramacaoHorarioUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.COMMITED, daoProgramacaoTurma.DB(),
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                Turma turmaContext = turmaBusiness.findTurmasByIdAndCdEscola(cd_turma_origem, cd_escola);
                if (turmaContext.dt_termino_turma.HasValue)
                    throw new TurmaBusinessException(Messages.msgErrorProgramacaoTurmaEncerrada, null,
                        TurmaBusinessException.TipoErro.ERRO_TURMA_ENCERRADA, false);

                // Esta Programação seria como se fosse a programação do curso
                List<ProgramacaoTurma> programacoesTurma = turmaBusiness
                .getProgramacaoTurmaByTurma(TransactionScopeBuilder.TransactionType.COMMITED, cd_turma_origem,
                    cd_escola, ProgramacaoTurmaDataAccess.TipoConsultaProgTurmaEnum.HAS_PROG_TURMA, false).ToList();

                programacao = turmaBusiness.getProgramacaoHorarioTurma(cd_turma_destino, cd_escola);

                if (programacoesTurma != null && programacoesTurma.Count() > 0 && programacao != null)
                    turmaBusiness.atualizaProgramacoesTurma(TransactionScopeBuilder.TransactionType.COMMITED,
                        criaProgramacaoTurmaTurma(programacoesTurma, programacao, cd_escola, cancelar), cd_turma_destino, null);

                transaction.Complete();
            }
        }
        public List<ProgramacaoTurma> criaProgramacaoTurmaTurma(List<ProgramacaoTurma> programacaoTurma, ProgramacaoHorarioUI programacao, int cd_escola, bool cancelar)
        {
            //Retorna a programação da Turma:
            List<ProgramacaoTurma> retorno = new List<ProgramacaoTurma>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.COMMITED, daoProgramacaoTurma.DB(),
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                IEnumerable<Feriado> feriadosEscola = null;
                foreach (var item in programacaoTurma)
                {
                    if (item.cd_feriado == null)
                    {
                        var novas_programacoes = turmaBusiness.criaProgramacaoTurma(programacao, cd_escola,
                            item.dc_programacao_turma, ref feriadosEscola);

                        foreach (ProgramacaoTurma pt in novas_programacoes)
                        {
                            pt.id_prog_cancelada = cancelar && (item.id_aula_dada || item.id_prog_cancelada);
                            pt.dta_cadastro_programacao = DateTime.UtcNow;
                            pt.id_programacao_manual =
                                (byte)ProgramacaoTurma.TipoProgramacaoManual.PROGRAMACAO_GERADA_CURSO;
                        }

                        retorno.AddRange(novas_programacoes);

                        var programacoesAnteriores = programacao.programacoes.ToList();
                        programacoesAnteriores.AddRange(novas_programacoes);
                        programacao.programacoes = programacoesAnteriores;
                    }
                }
                transaction.Complete();
            }
            return retorno;
        }
        public List<ProgramacaoTurma> atualizaProgramacaoTurmaAposDiarioAula(List<ProgramacaoTurma> programacoes,
            ProgramacaoHorarioUI programacao, int cd_escola, ref Boolean refez_programacoes)
        {
            List<ProgramacaoTurma> retorno = new List<ProgramacaoTurma>();
            List<ProgramacaoTurma> programacoes_atuais =
                programacao.programacoes != null ? programacao.programacoes.ToList() : programacoes;

            //Aponta para a última + 1 programação com diário de aula:
            List<ProgramacaoTurma> programacoes_com_diario_aula = new List<ProgramacaoTurma>();
            List<ProgramacaoTurma> programacoes_sem_diario_aula = new List<ProgramacaoTurma>();
            int j;
            if (programacao.horarios != null && programacao.horarios.Count > 0)
            {
                for (j = programacoes_atuais.Count - 1; j >= 0; j--)
                    if (!programacoes_atuais[j].id_aula_dada)
                        programacoes_sem_diario_aula.Add(programacoes_atuais[j]);
                    else
                        break;
                programacoes_atuais.RemoveRange(j + 1, programacoes_atuais.Count - 1 - j);
                programacoes_com_diario_aula = programacoes_atuais;
                programacoes_atuais = programacoes_sem_diario_aula;

                List<ProgramacaoTurma> qtd_programacoes = programacoes_sem_diario_aula
                    .Where(s => s.cd_feriado == null || s.cd_feriado_desconsiderado != null)
                    .ToList(); // || s.id_feriado_desconsiderado

                retorno = programacoes_com_diario_aula;

                if (programacoes_sem_diario_aula.Count > 0)
                {
                    refez_programacoes = true;
                    //Refaz a partir da última programação com diário de aula:
                    programacao.programacoes = programacoes_com_diario_aula;
                    IEnumerable<Feriado> feriadosEscola = null;

                    for (int i = qtd_programacoes.Count - 1; i >= 0; i--)
                    {
                        string nome_programacao = qtd_programacoes[i].dc_programacao_turma;

                        List<ProgramacaoTurma> novas_programacoes =
                            turmaBusiness.criaProgramacaoTurma(programacao, cd_escola, nome_programacao,
                                ref feriadosEscola);
                        var programacoesAnteriores = retorno;

                        for (int l = 0; l < novas_programacoes.Count(); l++)
                        {
                            novas_programacoes[l].id_programacao_manual = qtd_programacoes[i].id_programacao_manual;
                            novas_programacoes[l].id_mostrar_calendario = qtd_programacoes[i].id_mostrar_calendario;
                        }
                        programacoesAnteriores.AddRange(novas_programacoes);
                        programacao.programacoes = programacoesAnteriores;
                    }
                }
            }

            //programacoes_com_diario_aula.AddRange(retorno);
            //retorno = programacoes_com_diario_aula;

            return retorno;
        }

        public List<ProgramacaoTurma> atualizaProgramacaoTurma(List<ProgramacaoTurma> programacoes,
            ProgramacaoHorarioUI programacao, int cd_escola)
        {
            List<ProgramacaoTurma> retorno = new List<ProgramacaoTurma>();

            if (!programacao.cd_curso.HasValue || !programacao.cd_duracao.HasValue)
                throw new ProgramacaoTurmaBusinessException(Messages.msgNaoExisteCurso, null,
                    ProgramacaoTurmaBusinessException.TipoErro.ERRO_NAO_TEM_PROGRAMACAO_CURSO, false);

            List<ProgramacaoTurma> qtd_programacoes = programacao.programacoes != null
                ? programacao.programacoes.Where(s =>
                        s.cd_feriado == null || s.cd_feriado_desconsiderado.HasValue || s.id_feriado_desconsiderado)
                    .ToList()
                : programacoes.Where(s =>
                        s.cd_feriado == null || s.cd_feriado_desconsiderado.HasValue || s.id_feriado_desconsiderado)
                    .ToList();

            List<ProgramacaoTurma> antigas_programacoes = programacao.programacoes != null
                ? programacao.programacoes.ToList()
                : new List<ProgramacaoTurma>();
            if (qtd_programacoes.Count > 0)
            {
                //Zera as programações para iniciar da nova data inicial:
                programacao.programacoes = new List<ProgramacaoTurma>();
                IEnumerable<Feriado> feriadosEscola = null;
                for (int i = 0; i < qtd_programacoes.Count; i++)
                {
                    string nome_programacao = qtd_programacoes[i].dc_programacao_turma;

                    var novas_programacoes = turmaBusiness.criaProgramacaoTurma(programacao, cd_escola,
                        nome_programacao, ref feriadosEscola);
                    var programacoesAnteriores = retorno;

                    for (int l = 0; l < novas_programacoes.Count(); l++)
                        novas_programacoes[l].id_programacao_manual = qtd_programacoes[i].id_programacao_manual;

                    programacoesAnteriores.AddRange(novas_programacoes);
                    programacao.programacoes = programacoesAnteriores;
                }
            }

            return retorno;
        }

        public string criarNomeTurma(bool idTurmaPPT, int? cdTurmaPPT, int? codRegime, int cdProduto, int? codCurso,
            List<Horario> listahorario, DateTime inicioAulas)
        {
            string ehPPT = null;
            ehPPT = idTurmaPPT == true && (cdTurmaPPT == null || cdTurmaPPT == 0) ? "P" : "F";
            ehPPT = idTurmaPPT == false && (cdTurmaPPT == null || cdTurmaPPT == 0) ? null : ehPPT;
            //Regime Abreviado
            int cdRegime = codRegime == null ? 0 : (int)codRegime;
            var regime = daoRegime.findById(cdRegime, false);

            string regimeAbrev = regime != null
                ? regime.no_regime_abreviado != null ? ehPPT != null ? regime.no_regime_abreviado.Trim() + ehPPT + "/" :
                regime.no_regime_abreviado.Trim() + "/" : null
                : null;

            //Estagio Abreviado
            int cdCurso = codCurso == null ? 0 : (int)codCurso;
            Curso curso = new Curso();
            Estagio estagio = new Estagio();

            Produto produto = new Produto();

            string abrevEstagioProd = String.Empty;

            //Se for turma PPT Pai a nomenclatura deve conter o Produto e não Curso
            if (idTurmaPPT == true && (cdTurmaPPT == null || cdTurmaPPT == 0))
            {
                produto = daoProduto.findById(cdProduto, false);
                abrevEstagioProd = produto != null && produto.no_produto_abreviado != null
                    ? produto.no_produto_abreviado
                    : String.Empty;
            }
            else
            {
                curso = cursoBiz.findCursoById(cdCurso);
                if (curso != null)
                {
                    estagio = daoEstagio.findById(curso.cd_estagio, false);
                    abrevEstagioProd = estagio != null && estagio.no_estagio_abreviado != null
                        ? estagio.no_estagio_abreviado
                        : String.Empty;
                }
            }

            string retorno = regimeAbrev + abrevEstagioProd.Trim();

            retorno = turmaBusiness.criarNomeTurma(retorno, listahorario, inicioAulas);

            return retorno;
        }

        #endregion

        #region EstagioBusiness

        public IEnumerable<GenericModel.Estagio> findAllEstagio()
        {
            return daoEstagio.findAll(false).ToList();
        }

        public IEnumerable<Web.Services.Coordenacao.Model.EstagioSearchUI> getDescEstagio(SearchParameters parametros,
            string desc, string abrev, bool inicio, bool? status, int codP)
        {
            IEnumerable<EstagioSearchUI> retorno = new List<EstagioSearchUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_estagio";
                parametros.sort = parametros.sort.Replace("estagio_ativo", "id_estagio_ativo");
                parametros.sort = parametros.sort.Replace("ordem", "nm_ordem_estagio");

                retorno = daoEstagio.getEstagioDesc(parametros, desc, abrev, inicio, status, codP);
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<GenericModel.Estagio> getOrdemEstagio(int codP, int? cd_estagio,
            EstagioDataAccess.TipoConsultaEstagioEnum? tipoConsulta)
        {
            return daoEstagio.getEstagioOrdem(codP, cd_estagio, tipoConsulta);
        }

        public Estagio findByIdEstagio(int id)
        {
            return daoEstagio.findById(id, false);
        }

        public EstagioSearchUI addEstagio(GenericModel.Estagio entity)
        {
            Estagio estagio = new Estagio();
            EstagioSearchUI estagioUI = new EstagioSearchUI();
            string produto = "";
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                // Caso o estágio seja inativado, a ordem dele é -1, ou seja, não existe ordem para ele:
                if (!entity.id_estagio_ativo)
                    entity.nm_ordem_estagio = null;

                estagio = daoEstagio.add(entity, false);
                transaction.Complete();
            }

            estagioUI = EstagioSearchUI.fromEstagio(estagio, produto);

            return estagioUI;
        }

        public IEnumerable<Estagio> editEstagio(EstagioOrdem entity)
        {
            List<Estagio> listaNovosEstagios = new List<Estagio>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //Busca a lista de programações anterior a alteração:
                IEnumerable<Estagio> list = daoEstagio.getEstagioOrdem(entity.estagio.cd_produto, null,
                    EstagioDataAccess.TipoConsultaEstagioEnum.ALL);
                List<Estagio> listaDel = new List<Estagio>();
                List<Estagio> listaAntigosEstagios = list.ToList();

                listaNovosEstagios = entity.estagioOrdem.ToList();


                //Deletando estagios que não existem mais
                foreach (var estagioAntigo in listaAntigosEstagios)
                    if (listaNovosEstagios.Where(p => p.cd_estagio == estagioAntigo.cd_estagio).Count() <= 0)
                        listaDel.Add(estagioAntigo);
                if (listaDel != null && listaDel.Count > 0)
                    daoEstagio.deleteAllEstagio(listaDel);

                //Incluindo novos estagios
                foreach (var estagioNovo in listaNovosEstagios)
                {
                    if (estagioNovo.cd_estagio == 0)
                        daoEstagio.add(estagioNovo, false);
                    else
                    {
                        //Alterando estagios já existentes
                        Estagio estagio = daoEstagio.findById(estagioNovo.cd_estagio, false);
                        estagio.copy(estagioNovo);
                        daoEstagio.edit(estagio, false);
                    }
                }

                transaction.Complete();
            }

            return listaNovosEstagios;
        }

        public Estagio changeValueEstagio(Estagio estagioContext, Estagio estagioView)
        {
            if (estagioContext.id_estagio_ativo != estagioView.id_estagio_ativo)
                estagioContext.id_estagio_ativo = estagioView.id_estagio_ativo;
            if (estagioContext.cd_produto != estagioView.cd_produto)
                estagioContext.cd_produto = estagioView.cd_produto;
            if (estagioContext.no_estagio != estagioView.no_estagio)
                estagioContext.no_estagio = estagioView.no_estagio;
            estagioContext.cor_legenda = estagioView.cor_legenda;
            return estagioContext;
        }

        public List<GenericModel.Estagio> editOrdem(List<GenericModel.Estagio> entity)
        {
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                for (var i = 0; i < entity.Count; i++)
                    daoEstagio.editOrdem(entity[i]);
                transaction.Complete();

            }

            return entity;
        }

        public bool deleteEstagio(Estagio entity)
        {
            Estagio estagio = daoEstagio.findById(entity.cd_estagio, false);
            bool deleted = false;

            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = daoEstagio.delete(estagio, false);

                // Busca os estagios desse produto, reordena e atualiza as ordens:
                List<Estagio> estagios = daoEstagio
                    .getEstagioOrdem(entity.cd_produto, null, EstagioDataAccess.TipoConsultaEstagioEnum.HAS_ATIVO)
                    .ToList<Estagio>();

                estagios.Sort(delegate(Estagio e1, Estagio e2)
                {
                    return int.Parse(e1.nm_ordem_estagio.ToString())
                        .CompareTo(int.Parse(e2.nm_ordem_estagio.ToString()));
                });
                for (int i = 0; i < estagios.Count; i++)
                    estagios[i].nm_ordem_estagio = byte.Parse((i + 1).ToString());
                this.editOrdem(estagios.ToList<Estagio>());
                transaction.Complete();
            }

            return deleted;
        }

        public bool deleteAllEstagio(List<Estagio> estagios)
        {
            int idProduto = 0;
            bool deleted = false;

            if (estagios.Count > 0)
            {
                idProduto = estagios[0].cd_produto;
                using (var transaction =
                    TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    deleted = daoEstagio.deleteAllEstagio(estagios);

                    // Busca os estagios desse produto, reordena e atualiza as ordens:
                    estagios = daoEstagio
                        .getEstagioOrdem(idProduto, null, EstagioDataAccess.TipoConsultaEstagioEnum.HAS_ATIVO)
                        .ToList<Estagio>();

                    estagios.Sort(delegate(Estagio e1, Estagio e2)
                    {
                        return int.Parse(e1.nm_ordem_estagio.ToString())
                            .CompareTo(int.Parse(e2.nm_ordem_estagio.ToString()));
                    });
                    for (int i = 0; i < estagios.Count; i++)
                        estagios[i].nm_ordem_estagio = byte.Parse((i + 1).ToString());
                    this.editOrdem(estagios.ToList<Estagio>());
                    transaction.Complete();
                }
            }

            return deleted;
        }

        public IEnumerable<Estagio> getAllEstagioByProduto(int cdProduto)
        {
            return daoEstagio.getAllEstagioByProduto(cdProduto);
        }

        #endregion

        #region ConceitoBusiness

        public List<Conceito> getConceitosDisponiveisByProdutoTurma(int cd_turma)
        {
            List<Conceito> listaConceitos = new List<Conceito>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                listaConceitos = daoConceito.getConceitosDisponiveisByProdutoTurma(cd_turma);
                transaction.Complete();
            }

            return listaConceitos;
        }

        public IEnumerable<GenericModel.Conceito> findAllConceito()
        {
            return daoConceito.findAll(false).ToList();
        }

        public IEnumerable<Web.Services.Coordenacao.Model.ConceitoSearchUI> getDescConceito(SearchParameters parametros,
            string desc, bool inicio, bool? status, int codP)
        {
            IEnumerable<ConceitoSearchUI> retorno = new List<ConceitoSearchUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_conceito";
                parametros.sort = parametros.sort.Replace("pc_inicial", "pc_inicial_conceito");
                parametros.sort = parametros.sort.Replace("pc_final", "pc_final_conceito");
                parametros.sort = parametros.sort.Replace("conceito_ativo", "id_conceito_ativo");
                parametros.sort = parametros.sort.Replace("val_nota_participacao", "vl_nota_participacao");

                retorno = daoConceito.getConceitoDesc(parametros, desc, inicio, status, codP);
                transaction.Complete();
            }

            return retorno;
        }

        public Conceito findByIdConceito(int id)
        {
            return daoConceito.findById(id, false);
        }

        public ConceitoSearchUI addConceito(ConceitoSearchUI entity, int cdEscola)
        {
            Conceito conceito = new Conceito();
            ConceitoSearchUI conceitoUI = new ConceitoSearchUI();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                conceito.copy(entity);
                string produto = entity.no_produto;
                verificaConceitoParticipacao(entity.cd_produto, cdEscola, entity.vl_nota_participacao, null);
                var conceitoNew = daoConceito.add(conceito, false);
                conceitoUI = ConceitoSearchUI.fromConceito(conceitoNew, produto);
                transaction.Complete();
            }

            return conceitoUI;
        }

        public ConceitoSearchUI editConceito(ConceitoSearchUI entity, int cdEscola)
        {
            Conceito conceito = new Conceito();
            ConceitoSearchUI conceitoUI = new ConceitoSearchUI();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                conceito = daoConceito.findById(entity.cd_conceito, false);
                conceito.copy(entity);
                verificaConceitoParticipacao(entity.cd_produto, cdEscola, entity.vl_nota_participacao,
                    conceito.cd_conceito);
                daoConceito.saveChanges(false);

                string produto = entity.no_produto;
                conceitoUI = ConceitoSearchUI.fromConceito(conceito, produto);
                transaction.Complete();
            }

            return conceitoUI;
        }

        public bool deleteConceito(Conceito entity)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                Conceito conceito = daoConceito.findById(entity.cd_conceito, false);
                deleted = daoConceito.delete(conceito, false);
                transaction.Complete();
            }

            return deleted;
        }


        public bool deleteAllConceito(List<Conceito> conceitos)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = daoConceito.deleteAllConceito(conceitos);
                ;
                transaction.Complete();
            }

            return deleted;
        }


        public IEnumerable<Conceito> findConceitosAtivos(int idProduto, int idConceito)
        {
            return daoConceito.findConceitosAtivos(idProduto, idConceito);
        }

        public void verificaConceitoParticipacao(int idProduto, int cdEscola, double vlParticipacao,
            int? cdConceitoAlterado)
        {
            int quantidade = verificaMaiorQntParticipacao(idProduto, cdEscola);
            quantidade = quantidade > 0 ? quantidade : 4;
            if ((vlParticipacao * quantidade) > 10)
                throw new CoordenacaoBusinessException(Messages.msgErroParticipacaoMaior, null,
                    CoordenacaoBusinessException.TipoErro.ERRO_CONCEITO_MAIOR, false);
            double maiorConeito = daoConceito.somaParticipacaoPorConceito(idProduto, cdConceitoAlterado);
            if (vlParticipacao > maiorConeito)
                maiorConeito = vlParticipacao;
            if ((maiorConeito * quantidade) != 10)
                throw new CoordenacaoBusinessException(Messages.msgErroSomaParticipacaoMaior, null,
                    CoordenacaoBusinessException.TipoErro.ERRO_SOMA_CONCEITO_MAIOR, false);


        }

        #endregion

        #region DuraçãoBusiness

        public IEnumerable<GenericModel.Duracao> findAllDuracao()
        {
            return daoDuracao.findAll(false).ToList();
        }

        public IEnumerable<Duracao> getDescDuracao(SearchParameters parametros, string desc, bool inicio, bool? status)
        {
            IEnumerable<Duracao> retorno = new List<Duracao>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "dc_duracao";
                parametros.sort = parametros.sort.Replace("duracaoAtiva", "id_duracao_ativa");
                retorno = daoDuracao.getDuracaoDesc(parametros, desc, inicio, status);
                transaction.Complete();
            }

            return retorno;
        }

        public Duracao findByIdDuracao(int id)
        {
            return daoDuracao.findById(id, false);
        }

        public GenericModel.Duracao addDuracao(GenericModel.Duracao entity)
        {
            return daoDuracao.add(entity, false);
        }

        public GenericModel.Duracao editDuracao(GenericModel.Duracao entity)
        {
            return daoDuracao.edit(entity, false);
        }

        public bool deleteDuracao(Duracao entity)
        {
            Duracao duracao = daoDuracao.findById(entity.cd_duracao, false);
            var deleted = daoDuracao.delete(duracao, false);
            return deleted;
        }

        public bool deleteAllDuracao(List<Duracao> duracoes)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = daoDuracao.deleteAllDuracao(duracoes);
                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<Duracao> getDuracoes(DuracaoDataAccess.TipoConsultaDuracaoEnum hasDependente,
            int? cd_duracao, int? cd_escola)
        {
            IEnumerable<Duracao> ret = new List<Duracao>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                ret = daoDuracao.getDuracoes(hasDependente, cd_duracao, cd_escola).ToList();
                transaction.Complete();
            }

            return ret;
        }

        public IEnumerable<Duracao> getDuracaoProgramacao()
        {
            return daoDuracao.getDuracaoProgramacao();
        }

        public IEnumerable<Duracao> getDuracaoTabelaPreco()
        {
            return daoDuracao.getDuracaoTabelaPreco();
        }

        #endregion

        #region EventoBusiness

        public IEnumerable<DiarioAulaProgramcoesReport> getRelatorioDiarioAulaProgramacoes(int cd_escola, int? cd_turma,
            bool mais_turma_pagina, DateTime? dt_inicial, DateTime? dt_final, bool lancada)
        {
            return turmaBusiness
                .getRelatorioDiarioAulaProgramacoes(cd_escola, cd_turma, mais_turma_pagina, dt_inicial, dt_final, lancada)
                .ToList();
        }

        public IEnumerable<AlunoEventoReport> getRelatorioEventos(int cd_escola, int? cd_turma, int? cd_professor,
            int? cd_evento, int? qtd_faltas, bool falta_consecultiva, bool mais_turma_pagina,
            DateTime? dt_inicial, DateTime? dt_final)
        {
            List<AlunoEventoReport> listaRetorno = new List<AlunoEventoReport>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED,
                    daoEvento.DB()))
            {
                List<AlunoEventoReport> listaAlunoEvento = daoEvento.getRelatorioEventos(cd_escola, cd_turma,
                    cd_professor, cd_evento, qtd_faltas, dt_inicial, dt_final).ToList();
                listaRetorno = new List<AlunoEventoReport>();

                //Calcula as quantidade e quantidades consecultivas:
                if (listaAlunoEvento.Count > 0)
                {
                    short? nm_aula_anterior = listaAlunoEvento[0].nm_aula_turma;
                    int qtd_evento_consecultivo = 1;
                    int maior_qtd_evento_consecultivo = 1;
                    int qtd_evento = 1;
                    
                    listaAlunoEvento = listaAlunoEvento.OrderBy(a => a.cd_funcionario).ThenBy(a => a.cd_turma)
                        .ThenBy(a => a.cd_aluno).ThenBy(a => a.cd_evento)
                        .ThenBy(a => a.dt_aula).ThenBy(a => a.hr_inicial_aula).ToList();

                    string dta_aula = String.Format("{0:dd/MM/yyyy} ", listaAlunoEvento[0].dt_aula);
                    if (listaAlunoEvento.Count == 1 && (!qtd_faltas.HasValue || qtd_faltas == 1))
                    {
                        listaAlunoEvento[0].dta_aula =
                            dta_aula; //+ String.Format("{0:dd/MM/yyyy} ", listaAlunoEvento[0].dt_aula);
                        listaAlunoEvento[0].qtd_evento = 1;
                        listaAlunoEvento[0].qtd_consecultiva = 1;
                        listaRetorno = listaAlunoEvento;
                    }

                    for (int i = 1; i < listaAlunoEvento.Count; i++)
                    {
                        if (listaAlunoEvento[i - 1].cd_evento != listaAlunoEvento[i].cd_evento
                            || listaAlunoEvento[i - 1].cd_aluno != listaAlunoEvento[i].cd_aluno
                            || listaAlunoEvento[i - 1].cd_turma != listaAlunoEvento[i].cd_turma
                            || listaAlunoEvento[i - 1].cd_funcionario != listaAlunoEvento[i].cd_funcionario)
                        {
                            //Quando se trata de um novo evento para o aluno:

                            nm_aula_anterior = listaAlunoEvento[i].nm_aula_turma;

                            //Adiciona na lista para apresentar no relatório:
                            if ((!qtd_faltas.HasValue || qtd_faltas.Value == qtd_evento)
                                && (!falta_consecultiva ||
                                    (maior_qtd_evento_consecultivo == qtd_faltas.Value ||
                                     qtd_evento_consecultivo == qtd_faltas.Value)))
                            {
                                listaAlunoEvento[i - 1].qtd_evento = qtd_evento;
                                listaAlunoEvento[i - 1].dta_aula = dta_aula;

                                maior_qtd_evento_consecultivo = maior_qtd_evento_consecultivo > qtd_evento_consecultivo
                                    ? maior_qtd_evento_consecultivo
                                    : qtd_evento_consecultivo;
                                listaAlunoEvento[i - 1].qtd_consecultiva = maior_qtd_evento_consecultivo;
                                listaRetorno.Add(listaAlunoEvento[i - 1]);
                                //Caso não exista mais elementos, inclui o último também:
                                if (i + 1 == listaAlunoEvento.Count)
                                {
                                    listaAlunoEvento[i].qtd_evento = 1;
                                    listaAlunoEvento[i].dta_aula =
                                        String.Format("{0:dd/MM/yyyy} ", listaAlunoEvento[i].dt_aula);
                                    listaAlunoEvento[i].qtd_consecultiva = 1;
                                    listaRetorno.Add(listaAlunoEvento[i]);
                                    break;
                                }
                            }

                            //Reinicia as variáveis:
                            qtd_evento_consecultivo = 1;
                            qtd_evento = 1;
                            dta_aula = String.Format("{0:dd/MM/yyyy} ", listaAlunoEvento[i].dt_aula);
                            maior_qtd_evento_consecultivo = 1;
                        }
                        else
                        {
                            if (!nm_aula_anterior.HasValue || !listaAlunoEvento[i].nm_aula_turma.HasValue ||
                                nm_aula_anterior.Value == listaAlunoEvento[i].nm_aula_turma.Value - 1
                            ) //Continua consecultivo
                                qtd_evento_consecultivo++;
                            else
                            {
                                maior_qtd_evento_consecultivo = maior_qtd_evento_consecultivo > qtd_evento_consecultivo
                                    ? maior_qtd_evento_consecultivo
                                    : qtd_evento_consecultivo;
                                qtd_evento_consecultivo = 1;
                            }

                            if (listaAlunoEvento[i].nm_aula_turma.HasValue)
                                nm_aula_anterior = listaAlunoEvento[i].nm_aula_turma;
                            dta_aula += String.Format("{0:dd/MM/yyyy} ", listaAlunoEvento[i].dt_aula);
                            qtd_evento++;
                        }

                        //Caso não exista mais elementos, inclui o último também:
                        if (i + 1 == listaAlunoEvento.Count &&
                            (!qtd_faltas.HasValue || qtd_faltas.Value == qtd_evento) &&
                            (!falta_consecultiva || qtd_evento_consecultivo == qtd_faltas.Value))
                        {
                            listaAlunoEvento[i].qtd_evento = qtd_evento;
                            listaAlunoEvento[i].dta_aula = dta_aula;
                            maior_qtd_evento_consecultivo = maior_qtd_evento_consecultivo > qtd_evento_consecultivo
                                ? maior_qtd_evento_consecultivo
                                : qtd_evento_consecultivo;
                            listaAlunoEvento[i].qtd_consecultiva = maior_qtd_evento_consecultivo;
                            listaRetorno.Add(listaAlunoEvento[i]);
                            break;
                        }
                    }
                }

                transaction.Complete();
            }

            return listaRetorno;
        }

        public IEnumerable<GenericModel.Evento> findAllEvento()
        {
            return daoEvento.findAll(false).ToList();
        }

        public IEnumerable<Evento> getDescEvento(SearchParameters parametros, string desc, bool inicio, bool? status)
        {
            IEnumerable<Evento> retorno = new List<Evento>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_evento";
                parametros.sort = parametros.sort.Replace("eventoAtivo", "id_evento_ativo");
                retorno = daoEvento.getEventoDesc(parametros, desc, inicio, status);
                transaction.Complete();
            }

            return retorno;
        }

        public Evento findByIdEvento(int id)
        {
            return daoEvento.findById(id, false);
        }

        public Evento addEvento(Evento entity)
        {
            return daoEvento.add(entity, false);
        }

        public Evento editEvento(Evento entity)
        {
            if (entity.cd_evento >= 1 && entity.cd_evento <= 2)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp,
                    null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            return daoEvento.edit(entity, false);
        }

        public bool deleteEvento(Evento entity)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (entity.cd_evento >= 1 && entity.cd_evento <= 2)
                    throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp,
                        null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                Evento evento = daoEvento.findById(entity.cd_evento, false);
                deleted = daoEvento.delete(evento, false);
                transaction.Complete();
            }

            return deleted;
        }

        public bool deleteAllEvento(List<Evento> eventos)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (Evento e in eventos)
                    if (e.cd_evento >= 1 && e.cd_evento <= 2)
                        throw new RegistroProprietarioBusinessException(
                            Componentes.Utils.Messages.Messages.msgErroRegProp, null,
                            RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                deleted = daoEvento.deleteAllEvento(eventos);
                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<Evento> getEventos(int cd_evento, EventoDataAccess.TipoConsultaEventoEnum tipoConsulta)
        {
            return daoEvento.getEventos(cd_evento, tipoConsulta);
        }

        #endregion

        #region ModalidadeBusiness

        public IEnumerable<Modalidade> findAllModalidade()
        {
            return daoModalidade.findAll(false).ToList();
        }

        public IEnumerable<Modalidade> getModalidades(ModalidadeDataAccess.TipoConsultaModalidadeEnum? tipoConsulta,
            int? cd_modalidade)
        {
            return daoModalidade.getModalidades(tipoConsulta, cd_modalidade);
        }

        public IEnumerable<Modalidade> getDescModalidade(SearchParameters parametros, string desc, bool inicio,
            bool? status)
        {
            IEnumerable<Modalidade> retorno = new List<Modalidade>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_modalidade";
                parametros.sort = parametros.sort.Replace("modalidadeAtiva", "id_modalidade_ativa");
                retorno = daoModalidade.getModalidadeDesc(parametros, desc, inicio, status);
                transaction.Complete();
            }

            return retorno;
        }

        public Modalidade findByIdModalidade(int id)
        {
            return daoModalidade.findById(id, false);
        }

        public Modalidade addModalidade(Modalidade entity)
        {
            return daoModalidade.add(entity, false);
        }

        public Modalidade editModalidade(Modalidade entity)
        {
            return daoModalidade.edit(entity, false);
        }

        public bool deleteModalidade(Modalidade entity)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                Modalidade duracao = daoModalidade.findById(entity.cd_modalidade, false);
                deleted = daoModalidade.delete(duracao, false);
                ;
                transaction.Complete();
            }

            return deleted;
        }

        public bool deleteAllModalidade(List<Modalidade> modalidades)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = daoModalidade.deleteAllModalidade(modalidades);
                ;
                transaction.Complete();
            }

            return deleted;
        }

        #endregion

        #region MotivoDesistenciaBusiness

        public IEnumerable<MotivoDesistencia> findAllMotivoDesistencia()
        {
            return daoMotivoDes.findAll(false).ToList();
        }

        public IEnumerable<MotivoDesistencia> getDescMotivoDesistencia(SearchParameters parametros, string desc,
            bool inicio, bool? status, bool isCancelamento)
        {
            IEnumerable<MotivoDesistencia> retorno = new List<MotivoDesistencia>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_motivo_desistencia";
                parametros.sort = parametros.sort.Replace("motivoDesistenciaAtivo", "id_motivo_desistencia_ativo");
                parametros.sort = parametros.sort.Replace("isCancelamento", "id_cancelamento");
                retorno = daoMotivoDes.getMotivoDesistenciaDesc(parametros, desc, inicio, status, isCancelamento);
                transaction.Complete();
            }

            return retorno;
        }

        public MotivoDesistencia findByIdMotivoDesistencia(int id)
        {
            return daoMotivoDes.findById(id, false);
        }

        public MotivoDesistencia addMotivoDesistencia(MotivoDesistencia entity)
        {
            return daoMotivoDes.add(entity, false);
        }

        public MotivoDesistencia editMotivoDesistencia(MotivoDesistencia entity)
        {
            if (entity.cd_motivo_desistencia >= 1 && entity.cd_motivo_desistencia <= 1)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp,
                    null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            return daoMotivoDes.edit(entity, false);
        }

        public bool deleteMotivoDesistencia(MotivoDesistencia entity)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (entity.cd_motivo_desistencia >= 1 && entity.cd_motivo_desistencia <= 1)
                    throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp,
                        null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                MotivoDesistencia motivo = daoMotivoDes.findById(entity.cd_motivo_desistencia, false);
                deleted = daoMotivoDes.delete(motivo, false);
                ;
                transaction.Complete();
            }

            return deleted;
        }

        public bool deleteAllMotivoDesistencia(List<MotivoDesistencia> desitencias)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (MotivoDesistencia e in desitencias)
                    if (e.cd_motivo_desistencia >= 1 && e.cd_motivo_desistencia <= 1)
                        throw new RegistroProprietarioBusinessException(
                            Componentes.Utils.Messages.Messages.msgErroRegProp, null,
                            RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                deleted = daoMotivoDes.deleteAllMotivoDesistencia(desitencias);
                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<MotivoDesistencia> motivosDesistenciaWhitDesistencia(int cd_pessoa_empresa)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, daoMotivoDes.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                IEnumerable<MotivoDesistencia> motivos = daoMotivoDes.motivosDesistenciaWhitDesistencia(cd_pessoa_empresa);
                transaction.Complete();
                return motivos;
            }
        }

        public IEnumerable<MotivoDesistencia> getMotivoDesistenciaByCancelamento(bool isCancelamento)
        {
            return daoMotivoDes.getMotivoDesistenciaByCancelamento(isCancelamento);
        }

        #endregion

        #region Motivo Falta

        public IEnumerable<MotivoFalta> findAllMotivoFalta()
        {
            return daoMotivoFalta.findAll(false).ToList();
        }

        public IEnumerable<MotivoFalta> getDescMotivoFalta(SearchParameters parametros, string desc, bool inicio,
            bool? status)
        {
            IEnumerable<MotivoFalta> retorno = new List<MotivoFalta>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_motivo_falta";
                parametros.sort = parametros.sort.Replace("motivoFaltaAtiva", "id_motivo_falta_ativa");
                retorno = daoMotivoFalta.getMotivoFaltaDesc(parametros, desc, inicio, status);
                transaction.Complete();
            }

            return retorno;
        }

        public MotivoFalta findByIdMotivoFalta(int id)
        {
            return daoMotivoFalta.findById(id, false);
        }

        public MotivoFalta addMotivoFalta(MotivoFalta entity)
        {
            return daoMotivoFalta.add(entity, false);
        }

        public MotivoFalta editMotivoFalta(MotivoFalta entity)
        {
            return daoMotivoFalta.edit(entity, false);
        }

        public bool deleteMotivoFalta(MotivoFalta entity)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                MotivoFalta motivo = daoMotivoFalta.findById(entity.cd_motivo_falta, false);
                deleted = daoMotivoFalta.delete(motivo, false);
                transaction.Complete();
            }

            return deleted;
        }

        public bool deleteAllMotivoFalta(List<MotivoFalta> motivos)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = daoMotivoFalta.deleteAllMotivoFalta(motivos);
                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<MotivoFalta> getMotivoFaltaAtivo(int cdDiario)
        {
            return daoMotivoFalta.getMotivoFaltaAtivo(cdDiario);
        }

        #endregion

        #region ProdutoBusiness

        public IEnumerable<Produto> findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum hasDependente,
            int? cd_produto, int? cd_escola)
        {
            IEnumerable<Produto> retorno = new List<Produto>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoProduto.findProduto(hasDependente, cd_produto, cd_escola).ToList();
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<Produto> findProdutoAulaPersonalizada(int cd_aluno, int cd_escola)
        {
            IEnumerable<Produto> retorno = new List<Produto>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoProduto.findProdutoAulaPersonalizada(cd_aluno, cd_escola).ToList();
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<Produto> findProdutoTabela(int cdEscola)
        {
            return daoProduto.findProdutoTabela(cdEscola);
        }

        public IEnumerable<Produto> findAllProduto()
        {
            return daoProduto.findAll(false);
        }

        public IEnumerable<Produto> getDescProduto(SearchParameters parametros, string desc, string abrev, bool inicio,
            bool? status)
        {
            IEnumerable<Produto> retorno = new List<Produto>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_produto";
                parametros.sort = parametros.sort.Replace("produtoAtivo", "id_produto_ativo");
                retorno = daoProduto.getProdutoDesc(parametros, desc, abrev, inicio, status);
                transaction.Complete();
            }

            return retorno;
        }

        public Produto findByIdProduto(int id)
        {
            return daoProduto.findById(id, false);
        }

        public Produto addProduto(Produto entity)
        {
            daoProduto.add(entity, false);
            return entity;
        }

        public Produto editProduto(Produto entity)
        {
            if (entity.cd_produto >= 1 && entity.cd_produto <= 4)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp,
                    null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            daoProduto.edit(entity, false);
            return entity;
        }

        public bool deleteProduto(Produto entity)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (entity.cd_produto >= 1 && entity.cd_produto <= 4)
                    throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp,
                        null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                Produto produto = daoProduto.findById(entity.cd_produto, false);
                deleted = daoProduto.delete(produto, false);
                transaction.Complete();
            }

            return deleted;

        }

        public bool deleteAllProduto(List<Produto> produtos)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (Produto e in produtos)
                    if (e.cd_produto >= 1 && e.cd_produto <= 4)
                        throw new RegistroProprietarioBusinessException(
                            Componentes.Utils.Messages.Messages.msgErroRegProp, null,
                            RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                deleted = daoProduto.deleteAllProduto(produtos);
                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<Produto> getProdutosWithAtividadeExtra(int cd_pessoa_escola, bool isAtivo)
        {
            return daoProduto.getProdutosWithAtividadeExtra(cd_pessoa_escola, isAtivo);

        }

        #endregion

        #region RegimeBusines

        public IEnumerable<Regime> findAllRegime()
        {
            return daoRegime.findAll(false);
        }

        public IEnumerable<Regime> getRegimeTabelaPreco()
        {
            return daoRegime.getRegimeTabelaPreco();
        }

        public IEnumerable<Regime> getDescRegime(SearchParameters parametros, string desc, string abrev, bool inicio,
            bool? status)
        {
            IEnumerable<Regime> retorno = new List<Regime>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_regime";
                parametros.sort = parametros.sort.Replace("regimeAtivo", "id_regime_ativo");
                retorno = daoRegime.getRegimeDesc(parametros, desc, abrev, inicio, status);
                transaction.Complete();
            }

            return retorno;
        }

        public Regime findByIdRegime(int id)
        {
            return daoRegime.findById(id, false);
        }

        public Regime addRegime(Regime entity)
        {
            daoRegime.add(entity, false);
            return entity;
        }

        public Regime editRegime(Regime entity)
        {
            if (entity.cd_regime >= 1 && entity.cd_regime <= 4)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp,
                    null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            daoRegime.edit(entity, false);
            return entity;
        }

        public bool deleteRegime(Regime entity)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (entity.cd_regime >= 1 && entity.cd_regime <= 4)
                    throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp,
                        null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                Regime regime = daoRegime.findById(entity.cd_regime, false);
                deleted = daoRegime.delete(regime, false);
                ;
                transaction.Complete();
            }

            return deleted;

        }

        public bool deleteAllRegime(List<Regime> regimes)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (Regime e in regimes)
                    if (e.cd_regime >= 1 && e.cd_regime <= 4)
                        throw new RegistroProprietarioBusinessException(
                            Componentes.Utils.Messages.Messages.msgErroRegProp, null,
                            RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                deleted = daoRegime.deleteAllRegime(regimes);
                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<Regime> getRegimes(RegimeDataAccess.TipoConsultaRegimeEnum hasDependente, int? cd_regime)
        {
            IEnumerable<Regime> retorno = new List<Regime>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoRegime.getRegimes(hasDependente, cd_regime).ToList();
                transaction.Complete();
            }

            return retorno;
        }

        #endregion

        #region SalaBusines

        public IEnumerable<Sala> findAllSala()
        {
            return daoSala.findAll(false);
        }

        public IEnumerable<Sala> getSalasDisponiveisPorHorarios(Turma turma, int cd_escola)
        {
            IEnumerable<Sala> retorno = new List<Sala>();
            Duracao duracao = daoDuracao.findById(turma.cd_duracao, false);
            turma.Duracao = duracao;
            Curso curso = cursoBiz.findCursoById((int)turma.cd_curso);
            turma.Curso = curso;
            DateTime? dt_final_carga = turma.dt_final_aula == null ? turma.dt_inicio_aula.AddDays((turma.Curso == null || turma.Duracao == null) ? 120 : turma.Duracao.nm_duracao == 0 ? 0 : (int)(turma.Curso.nm_carga_horaria == null ? 0 : turma.Curso.nm_carga_horaria / (double)turma.Duracao.nm_duracao * 7.0)) : turma.dt_final_aula;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.UNCOMMITED, daoSala.DB(),
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = daoSala.getSalasDisponiveisPorHorarios(
                    turma.horariosTurma != null ? turma.horariosTurma.ToList() : new List<Horario>(), turma.cd_turma,
                    cd_escola, turma.cd_sala, turma.dt_inicio_aula.Date, dt_final_carga);
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<Sala> getSalasDisponiveisPorHorariosByModalidadeOnline(Turma turma, int cd_escola)
        {
            IEnumerable<Sala> retorno = new List<Sala>();
            Duracao duracao = daoDuracao.findById(turma.cd_duracao, false);
            turma.Duracao = duracao;
            Curso curso = cursoBiz.findCursoById((int)turma.cd_curso);
            turma.Curso = curso;
            DateTime? dt_final_carga = turma.dt_final_aula == null ? turma.dt_inicio_aula.AddDays((turma.Curso == null || turma.Duracao == null) ? 120 : turma.Duracao.nm_duracao == 0 ? 0 : (int)(turma.Curso.nm_carga_horaria == null ? 0 : turma.Curso.nm_carga_horaria / (double)turma.Duracao.nm_duracao * 7.0)) : turma.dt_final_aula;

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.UNCOMMITED, daoSala.DB(),
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = daoSala.getSalasDisponiveisPorHorariosByModalidadeOnline(
                    turma.horariosTurma != null ? turma.horariosTurma.ToList() : new List<Horario>(), turma.cd_turma,
                    cd_escola, turma.cd_sala, turma.dt_inicio_aula.Date, dt_final_carga);
                transaction.Complete();
            }

            return retorno;
        }

        private IEnumerable<Sala> getSalasDisponiveisPorHorariosEscrita(Turma turma, int cd_escola)
        {
            IEnumerable<Sala> retorno = new List<Sala>();
            Duracao duracao = daoDuracao.findById(turma.cd_duracao, false);
            turma.Duracao = duracao;
            if (turma.cd_curso != null)
            {
                Curso curso = cursoBiz.findCursoById((int)turma.cd_curso);
                turma.Curso = curso;
            }
            DateTime? dt_final_carga = turma.dt_final_aula == null ? turma.dt_inicio_aula.AddDays((turma.Curso == null || turma.Duracao == null) ? 120 : turma.Duracao.nm_duracao == 0 ? 0 : (int)(turma.Curso.nm_carga_horaria == null ? 0 : turma.Curso.nm_carga_horaria / (double)turma.Duracao.nm_duracao * 7.0)) : turma.dt_final_aula;
            if (turma.cd_sala_online == null)
                retorno = daoSala.getSalasDisponiveisPorHorarios(
                    turma.horariosTurma != null ? turma.horariosTurma.ToList() : new List<Horario>(), turma.cd_turma,
                    cd_escola, turma.cd_sala, turma.dt_inicio_aula.Date, dt_final_carga);
            else
                retorno = daoSala.getSalasDisponiveisPorHorariosByModalidadeOnline(
                    turma.horariosTurma != null ? turma.horariosTurma.ToList() : new List<Horario>(), turma.cd_turma,
                    cd_escola, turma.cd_sala_online, turma.dt_inicio_aula.Date, dt_final_carga);
            return retorno;
        }

        public IEnumerable<Web.Services.Coordenacao.Model.SalaSearchUI> getDescSala(SearchParameters parametros,
            string desc, bool inicio, bool? status, int cd_escola, bool online)
        {
            IEnumerable<SalaSearchUI> retorno = new List<SalaSearchUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_sala";
                parametros.sort = parametros.sort.Replace("salaAtiva", "id_sala_ativa");
                retorno = daoSala.getSalaDesc(parametros, desc, inicio, status, cd_escola, online);
                transaction.Complete();
            }

            return retorno;
        }

        public Sala findByIdSala(int id)
        {
            return daoSala.findById(id, false);
        }

        public Sala addSala(Sala entity)
        {
            daoSala.add(entity, false);
            return entity;
        }

        public Sala editSala(Sala entity)
        {
            daoSala.edit(entity, false);
            return entity;
        }

        public bool deleteSala(Sala entity)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                Sala sala = daoSala.findById(entity.cd_sala, false);
                deleted = daoSala.delete(sala, false);
                transaction.Complete();
            }

            return deleted;

        }

        public bool deleteAllSala(List<Sala> salas)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = daoSala.deleteAllSala(salas);
                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<Sala> findListSalasDiponiveis(TimeSpan horaIni, TimeSpan horaFim, DateTime data,
            bool? status, int? cdSala, int cdEscola, int? cd_atividade_extra)
        {
            List<Sala> listaSalas = new List<Sala>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.UNCOMMITED, daoSala.DB(),
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                IEnumerable<Sala> salas = daoSala.findListSalasDiponiveis(horaIni, horaFim, data, status, cdSala,
                    cdEscola, cd_atividade_extra);
                foreach (var salasDisponiveis in salas)
                {
                    salasDisponiveis.SalaAtividadeExtra = null;
                    listaSalas.Add(salasDisponiveis);

                }

                transaction.Complete();
            }

            return listaSalas;
        }

        public IEnumerable<Sala> findListSalasDiponiveisAulaRep(TimeSpan horaIni, TimeSpan horaFim, DateTime data,
            bool? status, int? cdSala, int cdEscola, int? cd_aula_reposicao, int? cd_turma)
        {
            List<Sala> listaSalas = new List<Sala>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.UNCOMMITED, daoSala.DB(),
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                IEnumerable<Sala> salas = daoSala.findListSalasDiponiveisAulaRep(horaIni, horaFim, data, status, cdSala,
                    cdEscola, cd_aula_reposicao, cd_turma);
                foreach (var salasDisponiveis in salas)
                {
                    salasDisponiveis.AulaReposicao = null;
                    listaSalas.Add(salasDisponiveis);

                }

                transaction.Complete();
            }

            return listaSalas;
        }

        public IEnumerable<Sala> findListSalas(bool? status, int? cdSala, int cdEscola)
        {
            IEnumerable<Sala> salas = daoSala.findListSalas(status, cdSala, cdEscola);
            return salas;
        }

        public IEnumerable<Sala> findListSalasTurmas(int cdEscola)
        {
            return daoSala.findListSalasTurmas(cdEscola);
        }

        public IEnumerable<Sala> findSalasTurmas(int cdEscola, bool online)
        {
            return daoSala.findSalasTurmas(cdEscola, online);
        }

        public IEnumerable<Sala> getSalas(int cd_sala, int cd_escola,
            SalaDataAccess.TipoConsultaDuracaoEnum tipoConsulta)
        {
            List<Sala> listSala = new List<Sala>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                listSala = daoSala.getSalas(cd_sala, cd_escola, tipoConsulta).ToList();
                transaction.Complete();
            }

            return listSala;
        }

        public IEnumerable<Sala> getSalasAulaReposicao(int cd_escola,
            SalaDataAccess.TipoConsultaDuracaoEnum tipoConsulta)
        {
            //List<SalaSearchUI> listSala = new List<SalaSearchUI>();
            //using (var transaction =
            //    TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            //{
            //listSala = daoSala.getSalasAulaReposicao(cd_escola, tipoConsulta).ToList();
            //transaction.Complete();
            //}

            return daoSala.getSalasAulaReposicao(cd_escola, tipoConsulta); //listSala;
        }

        public List<ReportControleSala> getHorariosRptControleSala(TimeSpan? hIni, TimeSpan? hFim, int cd_turma,
            int cd_professor, int cd_sala, List<int> diasSemana, int cd_escola)
        {
            List<ReportControleSala> listControleSala = new List<ReportControleSala>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                listControleSala = daoSala.getHorariosRptControleSala(hIni, hFim, cd_turma, cd_professor, cd_sala,
                    diasSemana, cd_escola);
                transaction.Complete();
            }

            return listControleSala;
        }

        public IEnumerable<Sala> findListSalasAulaPer(int cdEscola)
        {
            return daoSala.findListSalasAulaPer(cdEscola);
        }

        #endregion

        #region TipoAtividadeExtraBusiness

        public IEnumerable<TipoAtividadeExtra> findAllTipoAtv()
        {
            return daoTipoAtv.findAll(false).ToList();
        }

        public IEnumerable<TipoAtividadeExtra> getDescTipoAtv(SearchParameters parametros, string desc, bool inicio,
            bool? status)
        {
            IEnumerable<TipoAtividadeExtra> retorno = new List<TipoAtividadeExtra>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_tipo_atividade_extra";
                parametros.sort = parametros.sort.Replace("atividadeExtraAtiva", "id_tipo_atividade_extra_ativa");
                retorno = daoTipoAtv.getAtividadeDesc(parametros, desc, inicio, status);
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<TipoAtividadeExtra> getTipoAtividade()
        {
            return daoTipoAtv.getTipoAtividade();
        }

        public TipoAtividadeExtra findByIdTipoAtv(int id)
        {
            return daoTipoAtv.findById(id, false);
        }

        public TipoAtividadeExtra addTipoAtv(TipoAtividadeExtra entity)
        {
            return daoTipoAtv.add(entity, false);
        }

        public TipoAtividadeExtra editTipoAtv(TipoAtividadeExtra entity)
        {
            if (entity.cd_tipo_atividade_extra >= 1 && entity.cd_tipo_atividade_extra <= 1)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp,
                    null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            return daoTipoAtv.edit(entity, false);
        }

        public bool deleteTipoAtv(TipoAtividadeExtra entity)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (entity.cd_tipo_atividade_extra >= 1 && entity.cd_tipo_atividade_extra <= 1)
                    throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp,
                        null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                TipoAtividadeExtra atividade = daoTipoAtv.findById(entity.cd_tipo_atividade_extra, false);
                deleted = daoTipoAtv.delete(atividade, false);
                transaction.Complete();
            }

            return deleted;
        }

        public List<AtividadeExtra> gerarRecorrenciaAtividadeExtra(AtividadeExtraRecorrenciaUI atividade, int idEscola)
        {
            List<AtividadeExtra> atividadesCadastradas = new List<AtividadeExtra>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //Validações
                if (atividade == null)
                {
                    throw new CoordenacaoBusinessException(Utils.Messages.Messages.msgErroAtividadeExtraRecorrenciaNulo,
                            null, CoordenacaoBusinessException.TipoErro.ERRO_ATIVIDADE_EXTRA_RECORRENCIA_NULO, false);
                }
                else if (atividade.cd_atividade_extra <= 0)
                {
                    throw new CoordenacaoBusinessException(Utils.Messages.Messages.msgErroAtividadeExtraRecorrenciaIdZero,
                        null, CoordenacaoBusinessException.TipoErro.ERRO_ATIVIDADE_EXTRA_RECORRENCIA_ID_ZERO, false);
                }

                AtividadeExtra atividadeContext = daoAtividadeExtra.findById(atividade.cd_atividade_extra, false);
                if (atividadeContext == null)
                {
                    throw new CoordenacaoBusinessException(Utils.Messages.Messages.msgErroAtividadeExtraNotFound,
                        null, CoordenacaoBusinessException.TipoErro.ERRO_ATIVIDADE_EXTRA_NOT_FOUND, false);
                }

                if (atividade.AtividadeRecorrencia == null)
                {
                    throw new CoordenacaoBusinessException(Utils.Messages.Messages.msgErroAtividadeRecorrenciaNulo,
                        null, CoordenacaoBusinessException.TipoErro.ERRO_ATIVIDADE_RECORRENCIA_NULO, false);
                }
                else
                {
                    if (atividade.AtividadeRecorrencia.dt_limite != null && (DateTime.Compare((DateTime)atividadeContext.dt_atividade_extra, (DateTime)atividade.AtividadeRecorrencia.dt_limite) > 0))
                    {
                        throw new CoordenacaoBusinessException(Utils.Messages.Messages.msgErroDataAtividadeExtraMaiorDataLimiteRecorrencia,
                            null, CoordenacaoBusinessException.TipoErro.ERRO_DATA_ATIVIDADE_EXTRA_MAIOR_DATA_LIMITE_RECORRENCIA, false);
                    }
                }
                //Fim Validações

                //Preenche a atividadeRecorrencia
                AtividadeRecorrencia atividadeRecorrencia = new AtividadeRecorrencia();
                atividadeRecorrencia.cd_atividade_extra = atividadeContext.cd_atividade_extra;
                atividadeRecorrencia.dt_limite = atividade.AtividadeRecorrencia.dt_limite;
                atividadeRecorrencia.cd_atividade_recorrencia = atividade.AtividadeRecorrencia.cd_atividade_recorrencia;
                atividadeRecorrencia.id_tipo_recorrencia = atividade.AtividadeRecorrencia.id_tipo_recorrencia;
                atividadeRecorrencia.nm_eventos = atividade.AtividadeRecorrencia.nm_eventos;
                atividadeRecorrencia.nm_frequencia = atividade.AtividadeRecorrencia.nm_frequencia;

                //Pega os relacionamentos
                List<AtividadeCurso> atividadeCurso = daoAtividadeCurso.searchAtividadesCursoBycdAtividadExtra(atividadeContext.cd_atividade_extra, idEscola);
                List<AtividadeEscolaAtividade> atividadeEscola = daoAtividadeEscola.getAtividadesEscolatByAtividade(atividadeContext.cd_atividade_extra).ToList();
                List<AtividadeAluno> atividadeAlunos = daoAtividadeAluno.searchAtividadeAlunoByCdAtividadeExtraForRecorrencia(atividadeContext.cd_atividade_extra, idEscola).ToList();
                List<AtividadeProspect> atividadeProspects = daoAtividadeProspect.searchAtividadeProspectByCdAtividadeExtraForRecorrencia(atividadeContext.cd_atividade_extra, idEscola).ToList();

                //limpa os ids de relacionamentos para poder ser usado nas atividades criadas
                List<AtividadeCurso> atividadeCursoInserir = (atividadeCurso != null) ? preencheAtividadeCurso(atividadeCurso) : null;
                List<AtividadeEscolaAtividade> atividadeEscolaInserir = (atividadeEscola != null) ? preencheAtividadeEscolaAtividade(atividadeEscola) : null;
                List<AtividadeAluno> atividadeAlunosInserir = (atividadeAlunos != null) ? preencheAlunos(atividadeAlunos) : null;
                List<AtividadeProspect> atividadeProspectsInserir = (atividadeProspects != null) ? preencheAtividadeProspect(atividadeProspects) : null;

                List<AtividadeExtra> insertedNewAtividades = new List<AtividadeExtra>();


                //gera as novas atividades
                switch (atividadeRecorrencia.id_tipo_recorrencia)
                {

                    case (int)AtividadeRecorrencia.EnumTipoRecorrencia.DIARIAMENTE:
                        GerarRecorenciasDiarias(atividade, atividadeRecorrencia, atividadeContext, insertedNewAtividades);
                        break;
                    case (int)AtividadeRecorrencia.EnumTipoRecorrencia.SEMANALMENTE:
                        GerarRecorenciasSemanais(atividade, atividadeRecorrencia, atividadeContext, insertedNewAtividades);
                        break;
                    case (int)AtividadeRecorrencia.EnumTipoRecorrencia.QUINZENALMENTE:
                        GerarRecorenciasQuinzenais(atividade, atividadeRecorrencia, atividadeContext, insertedNewAtividades);
                        break;
                    case (int)AtividadeRecorrencia.EnumTipoRecorrencia.MENSALMENTE:
                        GerarRecorenciasMensais(atividade, atividadeRecorrencia, atividadeContext, insertedNewAtividades);
                        break;
                    case (int)AtividadeRecorrencia.EnumTipoRecorrencia.ANUALMENTE:
                        GerarRecorenciasAnuais(atividade, atividadeRecorrencia, atividadeContext, insertedNewAtividades);
                        break;
                }

                //Persiste as atividades geradas
                if (insertedNewAtividades.Count() > 0)
                {
                    AtividadeRecorrencia atividadeRecorrenciaInserted = daoAtividadeRecorrencia.add(atividadeRecorrencia, false);
                    persistirRecorrencias(insertedNewAtividades, atividadeCursoInserir, atividadeEscolaInserir, atividadeAlunosInserir, atividadeProspectsInserir, atividadeRecorrenciaInserted, atividadesCadastradas);
                    //atividadeContext.cd_atividade_recorrrencia = atividadeRecorrenciaInserted.cd_atividade_recorrencia;
                }

                daoAtividadeExtra.saveChanges(false);


                transaction.Complete();
            }

            return atividadesCadastradas;
        }

        private void persistirRecorrencias(List<AtividadeExtra> insertedNewAtividades, List<AtividadeCurso> atividadeCurso, List<AtividadeEscolaAtividade> atividadeEscola, List<AtividadeAluno> atividadeAlunos, List<AtividadeProspect> atividadeProspects, AtividadeRecorrencia atividadeRecorrencia, List<AtividadeExtra> atividadesCadastradas)
        {
            if (atividadeRecorrencia != null && atividadeRecorrencia.cd_atividade_recorrencia > 0)
            {
                if (insertedNewAtividades != null)
                {
                    if (insertedNewAtividades.Count > 0)
                    {

                        for (int i = 0; i < insertedNewAtividades.Count; i++)
                        {
                            AtividadeExtra atividadePersitir = preencheAtividadeExtra(insertedNewAtividades[i]);
                            List<AtividadeCurso> newAtividadesCurso = new List<AtividadeCurso>(atividadeCurso);
                            List<AtividadeEscolaAtividade> newAtividadeEscolaAtividade = new List<AtividadeEscolaAtividade>(atividadeEscola);
                            List<AtividadeAluno> newAtividadeAluno = new List<AtividadeAluno>(atividadeAlunos);
                            List<AtividadeProspect> newAtividadeProspect = new List<AtividadeProspect>(atividadeProspects);

                            atividadePersitir.cd_atividade_recorrrencia = atividadeRecorrencia.cd_atividade_recorrencia;
                            AtividadeExtra newAtividadeExtra = daoAtividadeExtra.add(atividadePersitir, false);

                            if (newAtividadeExtra != null)
                            {
                                if (newAtividadesCurso != null) { persistirRegistrosAtividadesCurso(newAtividadesCurso, newAtividadeExtra.cd_atividade_extra); }
                                if (newAtividadeEscolaAtividade != null) { persisitirRegistrosAtividadeEscolaAtividade(newAtividadeEscolaAtividade, newAtividadeExtra.cd_atividade_extra); }
                                if (newAtividadeAluno != null) { persisitirRegistrosAtividadeAluno(newAtividadeAluno, newAtividadeExtra.cd_atividade_extra); }
                                if (newAtividadeProspect != null) { persisitirRegistrosAtividadeProspect(newAtividadeProspect, newAtividadeExtra.cd_atividade_extra); }
                            }
                            else
                            {
                                throw new CoordenacaoBusinessException(Utils.Messages.Messages.msgErroAtividadeExtraNotFound,
                                    null, CoordenacaoBusinessException.TipoErro.ERRO_ATIVIDADE_EXTRA_NOT_FOUND, false);
                            }

                            daoAtividadeExtra.saveChanges(false);
                            if (newAtividadeExtra != null)
                            {
                                atividadesCadastradas.Add(newAtividadeExtra);
                            }

                        }
                    }

                }
                else
                {
                    throw new CoordenacaoBusinessException(Utils.Messages.Messages.msgErroAtividadeExtraNotFound,
                        null, CoordenacaoBusinessException.TipoErro.ERRO_ATIVIDADE_EXTRA_NOT_FOUND, false);
                }
            }
            else
            {
                throw new CoordenacaoBusinessException(Utils.Messages.Messages.msgErroAtividadeRecorrenciaNulo,
                    null, CoordenacaoBusinessException.TipoErro.ERRO_ATIVIDADE_RECORRENCIA_NULO, false);
            }

        }

        private void persisitirRegistrosAtividadeProspect(List<AtividadeProspect> newAtividadeProspect, int cdAtividadeExtra)
        {
            if (newAtividadeProspect.Count > 0)
            {
                //seta o id da atividade salva e persiste os registros
                newAtividadeProspect.ForEach(x => { x.cd_atividade_extra = cdAtividadeExtra; });
                daoAtividadeProspect.addRange(newAtividadeProspect, false);
            }
        }

        private void persisitirRegistrosAtividadeAluno(List<AtividadeAluno> newAtividadeAluno, int cdAtividadeExtra)
        {
            if (newAtividadeAluno.Count > 0)
            {
                //seta o id da atividade salva e persiste os registros
                newAtividadeAluno.ForEach(x => { x.cd_atividade_extra = cdAtividadeExtra; });
                daoAtividadeAluno.addRange(newAtividadeAluno, false);
            }
        }

        private void persisitirRegistrosAtividadeEscolaAtividade(List<AtividadeEscolaAtividade> newAtividadeEscolaAtividade, int cdAtividadeExtra)
        {
            if (newAtividadeEscolaAtividade.Count > 0)
            {
                //seta o id da atividade salva e persiste os registros
                newAtividadeEscolaAtividade.ForEach(x => { x.cd_atividade_extra = cdAtividadeExtra; });
                daoAtividadeEscola.addRange(newAtividadeEscolaAtividade, false);
            }
        }

        private void persistirRegistrosAtividadesCurso(List<AtividadeCurso> newAtividadesCurso, int cdAtividadeExtra)
        {
            if (newAtividadesCurso.Count > 0)
            {
                //seta o id da atividade salva e persiste os cursos
                newAtividadesCurso.ForEach(x => { x.cd_atividade_extra = cdAtividadeExtra; });
                daoAtividadeCurso.addRange(newAtividadesCurso, false);
            }
        }

        private void GerarRecorenciasDiarias(AtividadeExtraRecorrenciaUI atividade, AtividadeRecorrencia atividadeRecorrencia, AtividadeExtra atividadeContext, List<AtividadeExtra> insertedNewAtividades)
        {   //check pela data marcado
            if (atividadeRecorrencia.dt_limite != null)
            {
                DateTime dataInicial = atividadeContext.dt_atividade_extra;

                int qtdEventos = 1;

                dataInicial = dataInicial.AddDays(1);
                if (dataInicial.DayOfWeek == DayOfWeek.Sunday)
                {
                    dataInicial = dataInicial.AddDays(1);
                }

                while ((DateTime.Compare((DateTime)atividade.AtividadeRecorrencia.dt_limite, (DateTime)dataInicial) > 0) ||
                (DateTime.Compare((DateTime)atividade.AtividadeRecorrencia.dt_limite, (DateTime)dataInicial) == 0))
                {

                    AtividadeExtra newAtividade = new AtividadeExtra();

                    newAtividade = preencheAtividadeExtra(atividadeContext);
                    newAtividade.cd_atividade_extra = 0;
                    newAtividade.dt_atividade_extra = dataInicial;
                    insertedNewAtividades.Add(newAtividade);

                    qtdEventos++;

                    dataInicial = dataInicial.AddDays(1);
                    if (dataInicial.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dataInicial = dataInicial.AddDays(1);
                    }
                }

            }
            else if (atividadeRecorrencia.nm_eventos > 0)//check por numero de eventos marcado
            {
                DateTime dataInicial = atividadeContext.dt_atividade_extra;

                int qtdEventos = 1;

                dataInicial = dataInicial.AddDays(1);
                if (dataInicial.DayOfWeek == DayOfWeek.Sunday)
                {
                    dataInicial = dataInicial.AddDays(1);
                }

                while (qtdEventos <= atividadeRecorrencia.nm_eventos)
                {




                    AtividadeExtra newAtividade = new AtividadeExtra();

                    newAtividade = preencheAtividadeExtra(atividadeContext);
                    newAtividade.cd_atividade_extra = 0;
                    newAtividade.dt_atividade_extra = dataInicial;
                    insertedNewAtividades.Add(newAtividade);

                    qtdEventos++;

                    dataInicial = dataInicial.AddDays(1);
                    if (dataInicial.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dataInicial = dataInicial.AddDays(1);
                    }
                }
            }

        }

        private void GerarRecorenciasSemanais(AtividadeExtraRecorrenciaUI atividade, AtividadeRecorrencia atividadeRecorrencia, AtividadeExtra atividadeContext, List<AtividadeExtra> insertedNewAtividades)
        {   //check pela data marcado
            if (atividadeRecorrencia.dt_limite != null)
            {
                DateTime dataInicial = atividadeContext.dt_atividade_extra;

                int qtdEventos = 1;

                while ((DateTime.Compare((DateTime)atividade.AtividadeRecorrencia.dt_limite, (DateTime)dataInicial) > 0) ||
                (DateTime.Compare((DateTime)atividade.AtividadeRecorrencia.dt_limite, (DateTime)dataInicial) == 0))
                {

                    dataInicial = dataInicial.AddDays(7);
                    if (dataInicial.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dataInicial = dataInicial.AddDays(1);
                    }

                    qtdEventos++;

                    AtividadeExtra newAtividade = new AtividadeExtra();

                    newAtividade = preencheAtividadeExtra(atividadeContext);
                    newAtividade.cd_atividade_extra = 0;
                    newAtividade.dt_atividade_extra = dataInicial;
                    insertedNewAtividades.Add(newAtividade);
                }

            }
            else if (atividadeRecorrencia.nm_eventos > 0)//check por numero de eventos marcado
            {
                DateTime dataInicial = atividadeContext.dt_atividade_extra;

                int qtdEventos = 1;

                while (qtdEventos <= atividadeRecorrencia.nm_eventos)
                {
                    dataInicial = dataInicial.AddDays(7);
                    if (dataInicial.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dataInicial = dataInicial.AddDays(1);
                    }

                    qtdEventos++;

                    AtividadeExtra newAtividade = new AtividadeExtra();

                    newAtividade = preencheAtividadeExtra(atividadeContext);
                    newAtividade.cd_atividade_extra = 0;
                    newAtividade.dt_atividade_extra = dataInicial;
                    insertedNewAtividades.Add(newAtividade);
                }
            }

        }

        private void GerarRecorenciasQuinzenais(AtividadeExtraRecorrenciaUI atividade, AtividadeRecorrencia atividadeRecorrencia, AtividadeExtra atividadeContext, List<AtividadeExtra> insertedNewAtividades)
        {   //check pela data marcado
            if (atividadeRecorrencia.dt_limite != null)
            {
                DateTime dataInicial = atividadeContext.dt_atividade_extra;

                int qtdEventos = 1;

                while ((DateTime.Compare((DateTime)atividade.AtividadeRecorrencia.dt_limite, (DateTime)dataInicial) > 0) ||
                (DateTime.Compare((DateTime)atividade.AtividadeRecorrencia.dt_limite, (DateTime)dataInicial) == 0))
                {
                    dataInicial = dataInicial.AddDays(15);
                    if (dataInicial.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dataInicial = dataInicial.AddDays(1);
                    }

                    qtdEventos++;

                    AtividadeExtra newAtividade = new AtividadeExtra();

                    newAtividade = preencheAtividadeExtra(atividadeContext);
                    newAtividade.cd_atividade_extra = 0;
                    newAtividade.dt_atividade_extra = dataInicial;
                    insertedNewAtividades.Add(newAtividade);
                }

            }
            else if (atividadeRecorrencia.nm_eventos > 0)//check por numero de eventos marcado
            {
                DateTime dataInicial = atividadeContext.dt_atividade_extra;

                int qtdEventos = 1;

                while (qtdEventos <= atividadeRecorrencia.nm_eventos)
                {
                    dataInicial = dataInicial.AddDays(15);
                    if (dataInicial.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dataInicial = dataInicial.AddDays(1);
                    }

                    qtdEventos++;

                    AtividadeExtra newAtividade = new AtividadeExtra();

                    newAtividade = preencheAtividadeExtra(atividadeContext);
                    newAtividade.cd_atividade_extra = 0;
                    newAtividade.dt_atividade_extra = dataInicial;
                    insertedNewAtividades.Add(newAtividade);
                }
            }

        }

        private void GerarRecorenciasMensais(AtividadeExtraRecorrenciaUI atividade, AtividadeRecorrencia atividadeRecorrencia, AtividadeExtra atividadeContext, List<AtividadeExtra> insertedNewAtividades)
        {   //check pela data marcado
            if (atividadeRecorrencia.dt_limite != null)
            {
                DateTime dataInicial = atividadeContext.dt_atividade_extra;

                int qtdEventos = 1;

                while ((DateTime.Compare((DateTime)atividade.AtividadeRecorrencia.dt_limite, (DateTime)dataInicial) > 0) ||
                (DateTime.Compare((DateTime)atividade.AtividadeRecorrencia.dt_limite, (DateTime)dataInicial) == 0))
                {

                    dataInicial = dataInicial.AddMonths(1);
                    if (dataInicial.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dataInicial = dataInicial.AddDays(1);
                    }

                    qtdEventos++;

                    AtividadeExtra newAtividade = new AtividadeExtra();

                    newAtividade = preencheAtividadeExtra(atividadeContext);
                    newAtividade.cd_atividade_extra = 0;
                    newAtividade.dt_atividade_extra = dataInicial;
                    insertedNewAtividades.Add(newAtividade);
                }

            }
            else if (atividadeRecorrencia.nm_eventos > 0)//check por numero de eventos marcado
            {
                DateTime dataInicial = atividadeContext.dt_atividade_extra;

                int qtdEventos = 1;

                while (qtdEventos <= atividadeRecorrencia.nm_eventos)
                {
                    dataInicial = dataInicial.AddMonths(1);
                    if (dataInicial.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dataInicial = dataInicial.AddDays(1);
                    }

                    qtdEventos++;

                    AtividadeExtra newAtividade = new AtividadeExtra();

                    newAtividade = preencheAtividadeExtra(atividadeContext);
                    newAtividade.cd_atividade_extra = 0;
                    newAtividade.dt_atividade_extra = dataInicial;
                    insertedNewAtividades.Add(newAtividade);
                }
            }

        }

        private void GerarRecorenciasAnuais(AtividadeExtraRecorrenciaUI atividade, AtividadeRecorrencia atividadeRecorrencia, AtividadeExtra atividadeContext, List<AtividadeExtra> insertedNewAtividades)
        {   //check pela data marcado
            if (atividadeRecorrencia.dt_limite != null)
            {
                DateTime dataInicial = atividadeContext.dt_atividade_extra;

                int qtdEventos = 1;

                while ((DateTime.Compare((DateTime)atividade.AtividadeRecorrencia.dt_limite, (DateTime)dataInicial) > 0) ||
                (DateTime.Compare((DateTime)atividade.AtividadeRecorrencia.dt_limite, (DateTime)dataInicial) == 0))
                {
                    dataInicial = dataInicial.AddYears(1);
                    if (dataInicial.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dataInicial = dataInicial.AddDays(1);
                    }

                    qtdEventos++;

                    AtividadeExtra newAtividade = new AtividadeExtra();

                    newAtividade = preencheAtividadeExtra(atividadeContext);
                    newAtividade.cd_atividade_extra = 0;
                    newAtividade.dt_atividade_extra = dataInicial;
                    insertedNewAtividades.Add(newAtividade);
                }

            }
            else if (atividadeRecorrencia.nm_eventos > 0)//check por numero de eventos marcado
            {
                DateTime dataInicial = atividadeContext.dt_atividade_extra;

                int qtdEventos = 1;

                while (qtdEventos <= atividadeRecorrencia.nm_eventos)
                {
                    dataInicial = dataInicial.AddYears(1);
                    if (dataInicial.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dataInicial = dataInicial.AddDays(1);
                    }

                    qtdEventos++;

                    AtividadeExtra newAtividade = new AtividadeExtra();

                    newAtividade = preencheAtividadeExtra(atividadeContext);
                    newAtividade.cd_atividade_extra = 0;
                    newAtividade.dt_atividade_extra = dataInicial;
                    insertedNewAtividades.Add(newAtividade);
                }
            }

        }

        public AtividadeExtra preencheAtividadeExtra(AtividadeExtra atividadeExtra)
        {
            AtividadeExtra newAtividadeExtra = new AtividadeExtra
            {
                cd_atividade_extra = atividadeExtra.cd_atividade_extra,
                cd_tipo_atividade_extra = atividadeExtra.cd_tipo_atividade_extra,
                dt_atividade_extra = atividadeExtra.dt_atividade_extra,
                hh_inicial = atividadeExtra.hh_inicial,
                hh_final = atividadeExtra.hh_final,
                nm_vagas = atividadeExtra.nm_vagas,
                cd_produto = atividadeExtra.cd_produto,
                cd_funcionario = atividadeExtra.cd_funcionario,
                ind_carga_horaria = atividadeExtra.ind_carga_horaria,
                ind_pagar_professor = atividadeExtra.ind_pagar_professor,
                tx_obs_atividade = atividadeExtra.tx_obs_atividade,
                cd_usuario_atendente = atividadeExtra.cd_usuario_atendente,
                cd_sala = atividadeExtra.cd_sala,
                cd_pessoa_escola = atividadeExtra.cd_pessoa_escola,
                id_calendario_academico = atividadeExtra.id_calendario_academico,
                hr_limite_academico = atividadeExtra.hr_limite_academico,
                id_email_enviado = atividadeExtra.id_email_enviado,
                cd_atividade_recorrrencia = atividadeExtra.cd_atividade_recorrrencia
            };
            return newAtividadeExtra;
        }

        private List<AtividadeAluno> preencheAlunos(List<AtividadeAluno> atividadeAlunos)
        {
            List<AtividadeAluno> newAtividadeAlunos = new List<AtividadeAluno>();
            foreach (AtividadeAluno atividadeAluno in atividadeAlunos)
            {
                AtividadeAluno newAtividadeAluno = new AtividadeAluno
                {
                    cd_atividade_aluno = 0,
                    cd_atividade_extra = 0,
                    cd_aluno = atividadeAluno.cd_aluno,
                    ind_participacao = false,//atividadeAluno.ind_participacao,
                    tx_obs_atividade_aluno = atividadeAluno.tx_obs_atividade_aluno,
                    id_participacao = atividadeAluno.id_participacao
                };

                newAtividadeAlunos.Add(newAtividadeAluno);
            }

            return newAtividadeAlunos;
        }

        private List<AtividadeCurso> preencheAtividadeCurso(List<AtividadeCurso> atividadeCursos)
        {
            List<AtividadeCurso> newAtividadeCursos = new List<AtividadeCurso>();
            foreach (AtividadeCurso atividadeCurso in atividadeCursos)
            {
                AtividadeCurso newAtividadeCurso = new AtividadeCurso
                {
                    cd_atividade_curso = 0,
                    cd_atividade_extra = 0,
                    cd_curso = atividadeCurso.cd_curso,
                    cd_pessoa_escola = atividadeCurso.cd_pessoa_escola
                };

                newAtividadeCursos.Add(newAtividadeCurso);
            }

            return newAtividadeCursos;
        }

        private List<AtividadeEscolaAtividade> preencheAtividadeEscolaAtividade(List<AtividadeEscolaAtividade> atividadeEscolaAtividades)
        {
            List<AtividadeEscolaAtividade> newAtividadeEscolaAtividades = new List<AtividadeEscolaAtividade>();
            foreach (AtividadeEscolaAtividade atividadeEscolaAtividade in atividadeEscolaAtividades)
            {
                AtividadeEscolaAtividade newAtividadeEscolaAtividade = new AtividadeEscolaAtividade
                {
                    cd_atividade_escola = 0,
                    cd_atividade_extra = 0,
                    cd_escola = atividadeEscolaAtividade.cd_escola
                };

                newAtividadeEscolaAtividades.Add(newAtividadeEscolaAtividade);
            }

            return newAtividadeEscolaAtividades;
        }

        private List<AtividadeProspect> preencheAtividadeProspect(List<AtividadeProspect> atividadeProspects)
        {
            List<AtividadeProspect> newAtividadeProspects = new List<AtividadeProspect>();
            foreach (AtividadeProspect atividadeProspect in atividadeProspects)
            {
                AtividadeProspect newAtividadeProspect = new AtividadeProspect
                {
                    cd_atividade_prospect = 0,
                    cd_atividade_extra = 0,
                    cd_prospect = atividadeProspect.cd_prospect,
                    ind_participacao = false,//atividadeProspect.ind_participacao,
                    txt_obs_atividade_propspect = atividadeProspect.txt_obs_atividade_propspect,
                    id_participacao = atividadeProspect.id_participacao
                };

                newAtividadeProspects.Add(newAtividadeProspect);
            }

            return newAtividadeProspects;
        }

        public bool deleteAllTipoAtividade(List<TipoAtividadeExtra> tiposAtividades)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (TipoAtividadeExtra e in tiposAtividades)
                    if (e.cd_tipo_atividade_extra >= 1 && e.cd_tipo_atividade_extra <= 1)
                        throw new RegistroProprietarioBusinessException(
                            Componentes.Utils.Messages.Messages.msgErroRegProp, null,
                            RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                deleted = daoTipoAtv.deleteAllTipoAtividade(tiposAtividades);
                ;
                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<TipoAtividadeExtra> getTipoAtividade(bool? status, int? cdTipoAtividadeExtra,
            int? cd_pessoa_escola, TipoAtividadeExtraDataAccess.TipoConsultaAtivExtraEnum tipoConsulta)
        {
            return daoTipoAtv.getTipoAtividade(status, cdTipoAtividadeExtra, cd_pessoa_escola, tipoConsulta);
        }

        public IEnumerable<TipoAtividadeExtra> getTipoAtividadeWhitAtividadeExtra(int cd_pessoa_escola)
        {
            return daoTipoAtv.getTipoAtividadeWhitAtividadeExtra(cd_pessoa_escola);
        }

        #endregion

        #region Feriado

        public IEnumerable<Feriado> getDescFeriado(SearchParameters parametros, string desc, bool inicio, bool? status, int cdEscola, int Ano, int Mes, int Dia, int AnoFim, int MesFim, int DiaFim, bool? somenteAno, bool idFeriadoAtivo)
        {
            return turmaBusiness.getDescFeriado(parametros, desc, inicio, status, cdEscola, Ano, Mes, Dia, AnoFim, MesFim, DiaFim, somenteAno, idFeriadoAtivo);
        }

        public Feriado findByIdFeriado(int id)
        {
            return turmaBusiness.findByIdFeriado(id);
        }

        public List<Feriado> getFeriadosPorPeriodo(Feriado feriado, int? cd_escola)
        {
            return turmaBusiness.getFeriadosPorPeriodo(feriado, cd_escola);
        }

        public Feriado editFeriado(Feriado entity, int cd_escola, string login, ref bool refez_programacoes,
            int cd_usuario)
        {
            return turmaBusiness.editFeriado(entity, cd_escola, login, ref refez_programacoes, cd_usuario);
        }

        public bool deleteAllFeriado(List<Feriado> feriados, int cd_escola, string login,
            ref Boolean refez_programacoes, int cd_usuario)
        {
            return turmaBusiness.deleteAllFeriado(feriados, cd_escola, login, ref refez_programacoes, cd_usuario);
        }

        public bool deletarFeriados(List<Feriado> feriados, ref Boolean refez_programacoes, int cd_escola)
        {
            return turmaBusiness.deletarFeriados(feriados, ref refez_programacoes, cd_escola);
        }

        public Feriado addFeriado(Feriado entity, int cd_escola, string login, ref Boolean refez_programacoes,
            int cd_usuario)
        {
            return turmaBusiness.addFeriado(entity, cd_escola, login, ref refez_programacoes, cd_usuario);
        }

        public OpcoesPagamentoUI calculaSugestaoOpcoesPgto(Parametro parametro, DateTime data_matricula, int cd_escola,
            int? cd_curso, int? cd_duracao, int? cd_regime)
        {
            OpcoesPagamentoUI retorno = new OpcoesPagamentoUI();

            retorno.mes = data_matricula.Month;
            retorno.ano = data_matricula.Year;

            //Cria a sugestão de dia ano e mês das opções de pagamento:
            if (parametro.nm_dia_vencimento.HasValue && parametro.nm_dia_vencimento.Value > 0)
                // A tabela foi colocada com um default de 0 para esse campo:
                if (parametro.id_dia_util_vencimento.HasValue)
                    retorno.dia = parametro.nm_dia_vencimento.Value;

            //Cria a sugestão de nro parcela:
            int? nro_parcelas = financeiroBusiness.getNroParcelas(cd_escola, cd_curso.Value, cd_regime.Value,
                cd_duracao.Value, data_matricula);
            if (nro_parcelas.HasValue)
                retorno.nro_parcelas = nro_parcelas.Value;

            return retorno;
        }

        public void pulaFeriadoEFinalSemana(ref DateTime data_opcao, int cd_escola,
            ref IEnumerable<Feriado> feriadosEscola)
        {
            pulaFeriadoEFinalSemana(ref data_opcao, cd_escola, ref feriadosEscola, true);
        }

        public void pulaFeriadoEFinalSemana(ref DateTime data_opcao, int cd_escola,
            ref IEnumerable<Feriado> feriadosEscola, bool addDias)
        {
            Feriado proximo_feriado = null;
            do
            {
                //Pula a data de feriado não financeiro:
                if (proximo_feriado != null)
                {

                    if (addDias)
                    {
                        data_opcao = new DateTime(proximo_feriado.aa_feriado_fim.Value,
                            proximo_feriado.mm_feriado_fim.Value,
                            proximo_feriado.aa_feriado_fim.HasValue
                                ? proximo_feriado.dd_feriado_fim.Value
                                : data_opcao.Year);
                        data_opcao = data_opcao.AddDays(1);
                    }
                    else
                    {
                        data_opcao = new DateTime(proximo_feriado.aa_feriado.Value, proximo_feriado.mm_feriado,
                            proximo_feriado.aa_feriado.HasValue ? proximo_feriado.dd_feriado : data_opcao.Year);
                        data_opcao = data_opcao.AddDays(-1);
                    }
                }

                proximo_feriado =
                    turmaBusiness.getFeriadosDentroOuAposData(cd_escola, data_opcao, true, ref feriadosEscola, addDias);
                // Enquanto tiver interceção da data com o feriado financeiro:
            } while (proximo_feriado != null
                     && ((proximo_feriado.aa_feriado.HasValue && proximo_feriado.aa_feriado_fim.HasValue
                                                              && DateTime.Compare(data_opcao,
                                                                  new DateTime((int)proximo_feriado.aa_feriado,
                                                                      (int)proximo_feriado.mm_feriado,
                                                                      (int)proximo_feriado.dd_feriado)) >= 0
                                                              && DateTime.Compare(data_opcao,
                                                                  new DateTime((int)proximo_feriado.aa_feriado_fim,
                                                                      (int)proximo_feriado.mm_feriado_fim,
                                                                      (int)proximo_feriado.dd_feriado_fim)) <= 0)
                         ||
                         (!proximo_feriado.aa_feriado.HasValue && !proximo_feriado.aa_feriado_fim.HasValue
                                                               && DateTime.Compare(data_opcao,
                                                                   new DateTime((int)data_opcao.Year,
                                                                       (int)proximo_feriado.mm_feriado,
                                                                       (int)proximo_feriado.dd_feriado)) >= 0
                                                               && DateTime.Compare(data_opcao,
                                                                   new DateTime((int)data_opcao.Year,
                                                                       (int)proximo_feriado.mm_feriado_fim,
                                                                       (int)proximo_feriado.dd_feriado_fim)) <= 0)));

            if (data_opcao.DayOfWeek == DayOfWeek.Saturday || data_opcao.DayOfWeek == DayOfWeek.Sunday)
            {
                while (data_opcao.DayOfWeek == DayOfWeek.Saturday || data_opcao.DayOfWeek == DayOfWeek.Sunday)
                    if (addDias)
                        data_opcao = data_opcao.AddDays(1);
                    else
                        data_opcao = data_opcao.AddDays(-1);
                pulaFeriadoEFinalSemana(ref data_opcao, cd_escola, ref feriadosEscola, addDias);
            }
        }

        #endregion

        #region Critérios de Avaliações

        public CriterioAvaliacao getCriterioAvaliacaoById(int id)
        {
            return daoCriterio.findById(id, false);
        }

        public CriterioAvaliacao postCriterioAvaliacao(CriterioAvaliacao criterioAvaliacao)
        {
            daoCriterio.add(criterioAvaliacao, false);
            return criterioAvaliacao;
        }

        public CriterioAvaliacao putCriterioAvaliacao(CriterioAvaliacao criterioAvaliacao)
        {
            var IsParticipacao = false;
            // Se desmarcar checkbox "Participação" verificar avaliacao participacao do criterio.
            if (!criterioAvaliacao.id_participacao)
            {
                IsParticipacao =
                    daoAvaliacaoParticipacao.verificaAvaliacaoParticipacaoByCriterio(criterioAvaliacao
                        .cd_criterio_avaliacao);
                if (IsParticipacao)
                    throw new CoordenacaoBusinessException(Messages.msgErroAvaliacaoParticipacao, null,
                        CoordenacaoBusinessException.TipoErro.ERRO_IS_PARTICIPACAO_AVALIACAO, false);
            }
            else
            {
                // Se marcar checkbox "Participação" verificar avaliação do Aluno. 
                IsParticipacao =
                    daoAvaliacaoAlunoParticipacao.verificaAvaliacaoAlunoParticipacaoByCriterio(criterioAvaliacao
                        .cd_criterio_avaliacao);
                if (IsParticipacao)
                    throw new CoordenacaoBusinessException(Messages.msgErroAvaliacaoParticipacaoAluno, null,
                        CoordenacaoBusinessException.TipoErro.ERRO_IS_PARTICIPACAO_AVALIACAO, false);
            }

            daoCriterio.edit(criterioAvaliacao, false);
            return criterioAvaliacao;
        }

        public bool deleteAllCriterioAvaliacao(List<CriterioAvaliacao> criteriosAvaliacao)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = daoCriterio.deleteAllCriterioAvaliacao(criteriosAvaliacao);
                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<CriterioAvaliacao> getCriterioAvaliacaoSearch(SearchParameters parametros, string descricao,
            string abrev, bool inicio, bool? status, bool? conceito, bool IsParticipacao)
        {
            IEnumerable<CriterioAvaliacao> retorno = new List<CriterioAvaliacao>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_criterio_avaliacao";
                parametros.sort = parametros.sort.Replace("criterio_ativo", "id_criterio_ativo");
                parametros.sort = parametros.sort.Replace("conceito", "id_conceito");
                parametros.sort = parametros.sort.Replace("participacao_ativo", "id_participacao");

                retorno = daoCriterio.GetCriterioAvaliacaoSearch(parametros, descricao, abrev, inicio, status, conceito,
                    IsParticipacao);
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<CriterioAvaliacao> getAllCriteriosAtivos(bool? ativo, int? cdCriterio)
        {
            var criteriosAvaliacao = daoCriterio.findAll(false);
            criteriosAvaliacao = ativo != null
                ? ativo == true
                    ? criteriosAvaliacao.Where(c =>
                        c.id_criterio_ativo == true || (cdCriterio.HasValue && c.cd_criterio_avaliacao == cdCriterio))
                    : criteriosAvaliacao.Where(c =>
                        c.id_criterio_ativo == false || (cdCriterio.HasValue && c.cd_criterio_avaliacao == cdCriterio))
                : criteriosAvaliacao;
            return criteriosAvaliacao;
        }

        public List<CriterioAvaliacao> getAvaliacaoCriterio(int? cd_tipo_avaliacao, int? cd_criterio_avaliacao)
        {
            return daoCriterio.getAvaliacaoCriterio(cd_tipo_avaliacao, cd_criterio_avaliacao);
        }

        public IEnumerable<CriterioAvaliacao> getNomesAvaliacao()
        {
            return daoCriterio.getNomesAvaliacao();
        }

        public IEnumerable<CriterioAvaliacao> getNomesAvaliacao(int cd_tipo_avaliacao)
        {
            return daoCriterio.getNomesAvaliacao(cd_tipo_avaliacao);
        }

        public IEnumerable<CriterioAvaliacao> getCriteriosPorAvalPart(int cdEscola)
        {
            return daoCriterio.getCriteriosPorAvalPart(cdEscola);
        }

        public IEnumerable<CriterioAvaliacao> getNomesAvaliacaoByAval(int? cdCriterio)
        {
            return daoCriterio.getNomesAvaliacaoByAval(cdCriterio);
        }

        #endregion

        #region Tipos de Avaliações

        public TipoAvaliacao getTipoAvaliacaoById(int id)
        {
            return daoTipoAvaliacao.findById(id, false);
        }

        public TipoAvaliacao postTipoAvaliacao(TipoAvaliacao tipoAvaliacao)
        {
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                List<AvaliacaoCurso> avalCursos = new List<AvaliacaoCurso>();
                List<Avaliacao> avaliacoes = new List<Avaliacao>();

                if (tipoAvaliacao != null & tipoAvaliacao.AvaliacaoCurso != null)
                    avalCursos = tipoAvaliacao.AvaliacaoCurso.ToList();
                if (tipoAvaliacao != null && tipoAvaliacao.Avaliacao != null)
                    avaliacoes = tipoAvaliacao.Avaliacao.ToList();
                var isNotaTotalMaior =
                    validaNotaAvaliacao(tipoAvaliacao.vl_total_nota, tipoAvaliacao.Avaliacao.ToList());
                if (!isNotaTotalMaior)
                    throw new TurmaBusinessException(Utils.Messages.Messages.msgSomatoriaMaximaError, null,
                        TurmaBusinessException.TipoErro.ERRO_SOMATORIO_NOTA_MAXIMA, false);

                tipoAvaliacao.AvaliacaoCurso = null;
                tipoAvaliacao.Avaliacao = null;
                //persiste o tipo de avaliação
                tipoAvaliacao = daoTipoAvaliacao.add(tipoAvaliacao, false);
                bool notaValida = true;
                if (notaValida == true)
                    daoTipoAvaliacao.saveChanges(true);
                else
                    throw new Exception(Messages.msgNotaAvalCurso);

                if (avalCursos != null && avalCursos.Count > 0)
                    setCursosAvaliacao(avalCursos, tipoAvaliacao.cd_tipo_avaliacao);
                if (avaliacoes != null && avaliacoes.Count > 0)
                    turmaBusiness.persistAvaliacoes(tipoAvaliacao.cd_tipo_avaliacao, avaliacoes, 0);

                transaction.Complete();
            }

            return tipoAvaliacao;
        }

        public bool validaNotaAvaliacao(int? valorTotalNota, List<Avaliacao> avaliacoes)
        {
            int totalNotasNomes = 0;
            valorTotalNota = valorTotalNota == null ? 0 : valorTotalNota;
            if (avaliacoes != null)
            {
                foreach (var item in avaliacoes)
                {
                    totalNotasNomes += (int)item.vl_nota;
                }
            }

            if (totalNotasNomes > valorTotalNota)
                return false;
            else return true;
        }

        public TipoAvaliacao putTipoAvaliacao(TipoAvaliacao tipoAvaliacao, int cdEscola)
        {
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED,
                    daoTipoAvaliacao.DB()))
            {
                this.sincronizarContextos(daoTipoAvaliacao.DB());
                List<AvaliacaoCurso> avalCursos = new List<AvaliacaoCurso>();
                List<Avaliacao> avaliacoes = new List<Avaliacao>();

                if (tipoAvaliacao != null && tipoAvaliacao.AvaliacaoCurso != null)
                    avalCursos = tipoAvaliacao.AvaliacaoCurso.ToList();
                if (tipoAvaliacao != null && tipoAvaliacao.Avaliacao != null)
                {
                    avaliacoes = tipoAvaliacao.Avaliacao.ToList();
                    if (cdEscola > 0)
                        for (int i = 0; i < avaliacoes.Count(); i++)
                        {
                            var isNotaTotalMaior = validaNotaAvaliacao(tipoAvaliacao.vl_total_nota,
                                tipoAvaliacao.Avaliacao.ToList());
                            if (!isNotaTotalMaior)
                                throw new TurmaBusinessException(Utils.Messages.Messages.msgSomatoriaMaximaError, null,
                                    TurmaBusinessException.TipoErro.ERRO_SOMATORIO_NOTA_MAXIMA, false);

                            //var existAvaliacaoLancadaTurma = turmaBusiness.existNotaLancadaAvaliacaoTurma(avaliacoes[i].cd_avaliacao, cdEscola);
                            //if (existAvaliacaoLancadaTurma)
                            //    throw new TurmaBusinessException(Utils.Messages.Messages.msgExistNotaLancadaParaTurma, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_NOTA_LANCADA, false);
                        }
                }

                tipoAvaliacao.AvaliacaoCurso = null;
                tipoAvaliacao.Avaliacao = null;

                tipoAvaliacao = daoTipoAvaliacao.edit(tipoAvaliacao, false);
                bool notaValida = true;
                if (notaValida == true)
                    daoTipoAvaliacao.saveChanges(false);
                else
                    throw new Exception(Messages.msgNotaAvalCurso);
                if (avalCursos != null)
                    setCursosAvaliacao(avalCursos, tipoAvaliacao.cd_tipo_avaliacao);

                if (avaliacoes != null)
                    turmaBusiness.persistAvaliacoes(tipoAvaliacao.cd_tipo_avaliacao, avaliacoes, cdEscola);

                transaction.Complete();
            }

            return tipoAvaliacao;
        }

        public bool verificaNotaCurso(List<Curso> listCurso, int? nota)
        {
            bool notaMaior = true;
            for (int i = 0; i < listCurso.Count(); i++)
                if (listCurso[i].nm_total_nota < nota)
                {
                    notaMaior = false;
                    break;
                }

            return notaMaior;
        }

        public bool deleteAllTipoAvaliacao(List<TipoAvaliacao> TiposAvaliacao, int cdEscola)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED,
                    daoAvaliacao.DB()))
            {
                sincronizarContextos(daoAvaliacao.DB());

                foreach (var itemAvaliacao in TiposAvaliacao)
                {
                    IEnumerable<Avaliacao> avaliacoesADeletar =
                        daoAvaliacao.findByIdTipoAvaliacao(itemAvaliacao.cd_tipo_avaliacao).ToList();

                    if (avaliacoesADeletar != null)
                    {
                        foreach (var item in avaliacoesADeletar)
                        {
                            var deletar =
                                (from av in avaliacoesADeletar where av.cd_avaliacao == item.cd_avaliacao select av)
                                .FirstOrDefault();
                            var avalTurmaDeletar =
                                daoAvaliacaoTurma.GetAvaliacaoTurmaByIdAvaliacao(item.cd_avaliacao, cdEscola);

                            turmaBusiness.DeletarRelacionamentoAvalicaoTurma(avalTurmaDeletar, cdEscola);
                        }
                    }
                }

                deleted = daoTipoAvaliacao.deleteAllTipoAvaliacao(TiposAvaliacao);
                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<AvaliacaoCurso> getCursoTipoAvaliacao(int cdTipoAvaliacao)
        {
            return daoAvalCurso.getAllAvaliacaoCursoByAvalicao(cdTipoAvaliacao);
        }

        public IEnumerable<TipoAvaliacao> getTipoAvaliacaoSearch(SearchParameters parametros, string descricao,
            bool inicio, bool? status, int? cd_tipo_avaliacao, int? cd_criterio_avaliacao, int cdCurso, int cdProduto)
        {
            IEnumerable<TipoAvaliacao> retorno = new List<TipoAvaliacao>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_Tipo_avaliacao";
                parametros.sort = parametros.sort.Replace("tipo_ativo", "id_tipo_ativo");
                parametros.sort = parametros.sort.Replace("oral", "id_oral");
                retorno = daoTipoAvaliacao.GetTipoAvaliacaoSearch(parametros, descricao, inicio, status,
                    cd_tipo_avaliacao, cd_criterio_avaliacao, cdCurso, cdProduto);
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<TipoAvaliacao> getTipoAvaliacao(bool ativo, int idTipoAvaliacao)
        {
            var tiposAvaliacoes = daoTipoAvaliacao.getTipoAvaliacao(ativo, idTipoAvaliacao);
            return tiposAvaliacoes;
        }

        public int? getTotalNotaTipoAvaliacao(int idtipoAvaliacao)
        {
            return daoTipoAvaliacao.getTotalNotaTipoAvaliacao(idtipoAvaliacao);
        }

        public List<TipoAvaliacao> getTipoAvaliacao()
        {
            return daoTipoAvaliacao.getTipoAvaliacao();
        }

        public IEnumerable<TipoAvaliacao> getTipoAvaliacao(bool? ativo, int idTipoAvaliacao)
        {
            return daoTipoAvaliacao.getTipoAvaliacao(ativo, idTipoAvaliacao);
        }

        private void setCursosAvaliacao(List<AvaliacaoCurso> avalCursosUI, int cdTipoAtividade)
        {
            AvaliacaoCurso avalCurso = new AvaliacaoCurso();
            List<AvaliacaoCurso> avalCursosView = new List<AvaliacaoCurso>();
            IEnumerable<AvaliacaoCurso> avalCursosContext =
                daoAvalCurso.getAllAvaliacaoCursoByAvalicao(cdTipoAtividade);
            //Pessoa pessoa = DataAccess.findById(cdPessoa, false);
            if (avalCursosUI != null)
            {
                avalCursosView = avalCursosUI;
                //procedimento para deletar um registro que esta na base de dados
                IEnumerable<AvaliacaoCurso> avalCursosComCodigo =
                    avalCursosView.Where(av => av.cd_avaliacao_curso != 0);

                IEnumerable<AvaliacaoCurso> avalCursosDeleted = avalCursosContext.Where(ec =>
                    !avalCursosComCodigo.Any(ev => ec.cd_avaliacao_curso == ev.cd_avaliacao_curso));
                if (avalCursosDeleted.Count() > 0)
                    foreach (var item in avalCursosDeleted)
                    {
                        var deletarAvalCurso =
                            (from ac in avalCursosContext
                             where ac.cd_avaliacao_curso == item.cd_avaliacao_curso
                             select ac).FirstOrDefault();
                        if (deletarAvalCurso != null)
                            daoAvalCurso.delete(deletarAvalCurso, false);
                    }

                //Procedimento para inserir um registro conforme o código
                foreach (var item in avalCursosView)
                {
                    if (item.cd_avaliacao_curso.Equals(null) || item.cd_avaliacao_curso == 0)
                    {
                        item.cd_tipo_avaliacao = cdTipoAtividade;
                        daoAvalCurso.add(item, false);
                    }
                }
            }
        }

        public IEnumerable<TipoAvaliacao> getTipoAvaliacaoAvaliacaoTurma()
        {
            return daoTipoAvaliacao.getTipoAvaliacaoAvaliacaoTurma();
        }

        public List<TipoAvaliacaoTurma> tiposAvaliacao(int cd_turma, int cd_escola, bool id_conceito)
        {
            return daoTipoAvaliacao.tiposAvaliacao(cd_turma, cd_escola, id_conceito);
        }
        public DataTable getRptAvaliacao(int cd_turma, int cdCurso, int cdProduto, int cdEscola, int cdFuncionario, int tipoTurma, byte sitTurma, DateTime? pDtIni, DateTime? pDtFim, bool isConceito)
        {
            return daoAvaliacao.getRptAvaliacao(cd_turma, cdCurso, cdProduto, cdEscola, cdFuncionario, tipoTurma, sitTurma, pDtIni, pDtFim, isConceito);
        }
        public DataTable getRptAvaliacaoTurma(int cd_turma)
        {
            return daoAvaliacao.getRptAvaliacaoTurma(cd_turma);
        }

        public DataTable getRptAvaliacaoTurmaConceito(int cd_turma)
        {
            return daoAvaliacao.getRptAvaliacaoTurmaConceito(cd_turma);
        }
        #endregion

        #region Programacao

        public ProgramacaoCursoUI postProgramacaoCurso(Programacao programacao)
        {
            ProgramacaoCurso prog = new ProgramacaoCurso();
            ItemProgramacao itemProg = new ItemProgramacao();
            ProgramacaoCursoUI programacaoUI = new ProgramacaoCursoUI();
            IEnumerable<ItemProgramacao> itensDel = new List<ItemProgramacao>();
            this.sincronizarContextos(daoItemProgramacaoCurso.DB());
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                // verificando se já existe a ProgramaçãoCurso
                int cdRegime = 0;
                if (programacao.cd_regime != null)
                    cdRegime = Convert.ToInt32(programacao.cd_regime);
                ProgramacaoCurso existeProgramacao = daoProgramacaoCurso.getProgramacao(programacao.cd_curso,
                    programacao.cd_duracao, programacao.cd_escola);

                if (existeProgramacao != null && existeProgramacao.cd_programacao_curso > 0)
                    itensDel = daoItemProgramacaoCurso.getItensProgramacaoCursoById(existeProgramacao
                        .cd_programacao_curso);

                List<ItemProgramacao> listDel = itensDel.ToList();

                //deletar o que já existe
                if (listDel != null && listDel.Count() > 0)
                    daoItemProgramacaoCurso.deleteAllItemProgramacaoCurso(listDel);

                //Deletando Programação Curso caso não tenha nenhum item na Lista
                if (existeProgramacao != null)
                {
                    ProgramacaoCurso progCurso =
                        daoProgramacaoCurso.findById(existeProgramacao.cd_programacao_curso, false);
                    if (progCurso != null && progCurso.cd_programacao_curso > 0)
                    {
                        List<ProgramacaoCurso> listProgCurso = new List<ProgramacaoCurso>();
                        listProgCurso.Add(progCurso);
                        if (programacao.itens.Count() == 0)
                            daoProgramacaoCurso.deleteAllProgramacoesCursos(listProgCurso);
                    }
                }

                //Incluindo Itens
                int cdProgramacao = 0;
                if (existeProgramacao != null)
                {
                    cdProgramacao = existeProgramacao.cd_programacao_curso;
                    for (int i = 0; i < programacao.itens.Count(); i++)
                    {
                        programacao.itens[i].cd_programacao_curso = cdProgramacao;
                        //Chamar Função para Incluir os Itens na ProgramaçãoCurso já existente
                        daoItemProgramacaoCurso.addItemProgramacao(programacao.itens[i]);
                    }
                }
                else
                {
                    prog = new ProgramacaoCurso
                    {
                        cd_curso = programacao.cd_curso,
                        cd_duracao = programacao.cd_duracao,
                        ItemProgramacao = programacao.itens,
                        cd_escola = programacao.cd_escola
                    };
                    daoProgramacaoCurso.add(prog, false);

                }

                if (prog.cd_programacao_curso != 0)
                    programacaoUI = daoProgramacaoCurso.GetProgramacaoById(prog.cd_programacao_curso);
                else if (programacao.itens.Count() > 0)
                    programacaoUI = daoProgramacaoCurso.GetProgramacaoById(programacao.itens[0].cd_programacao_curso);
                transaction.Complete();
            }

            return programacaoUI;
        }

        public ProgramacaoCursoUI putProgramacaoCurso(Programacao programacao)
        {
            ProgramacaoCursoUI progs = new ProgramacaoCursoUI();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //Busca a lista de programações anterior a alteração:
                IEnumerable<ItemProgramacao> lista =
                    daoItemProgramacaoCurso.getItensProgramacaoCursoById(programacao.cd_programacao_curso);
                List<ItemProgramacao> listaProgramacaoAnterior = lista.ToList();

                //Deleta as programações que não existem mais:
                if (listaProgramacaoAnterior != null && listaProgramacaoAnterior.Count > 0)
                {
                    IEnumerable<ItemProgramacao> progDeletar = listaProgramacaoAnterior.Where(p =>
                        !programacao.itens.Where(pc => pc.cd_item_programacao == p.cd_item_programacao).Any());

                    if (progDeletar != null)
                        daoItemProgramacaoCurso.deleteAllItemProgramacaoCurso(progDeletar.ToList());
                }

                //Inclui as programações:
                for (int i = 0; i < programacao.itens.Count; i++)
                    if (programacao.itens[i].cd_item_programacao == 0)
                    {
                        programacao.itens[i].cd_programacao_curso = programacao.cd_programacao_curso;
                        daoItemProgramacaoCurso.addItemProgramacao(programacao.itens[i]);
                    }
                    else
                        //Altera as programações:
                        daoItemProgramacaoCurso.editItemProgramacao(programacao.itens[i]);

                progs = daoProgramacaoCurso.GetProgramacaoById(programacao.cd_programacao_curso);
                transaction.Complete();
            }

            return progs;
        }

        public bool deleteAllProgramacaoCurso(List<ProgramacaoCurso> programacoesCurso)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //Deleta os itens de programação:
                foreach (ProgramacaoCurso programacaoCurso in programacoesCurso)
                {
                    IEnumerable<ItemProgramacao> listaItens =
                        daoItemProgramacaoCurso.getItensProgramacaoCursoById(programacaoCurso.cd_programacao_curso);
                    daoItemProgramacaoCurso.deleteAllItemProgramacaoCurso(listaItens.ToList());
                }

                deleted = daoProgramacaoCurso.deleteAllProgramacoesCursos(programacoesCurso);
                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<ProgramacaoCursoUI> getProgramacaoCursoSearch(SearchParameters parametros, int? cdCurso,
            int? cdDuracao, int? cd_escola)
        {
            IEnumerable<ProgramacaoCursoUI> retorno = new List<ProgramacaoCursoUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_curso";
                retorno = daoProgramacaoCurso.getProgramacaoCursoSearch(parametros, cdCurso, cdDuracao, cd_escola);
                transaction.Complete();
            }

            return retorno;
        }

        public ProgramacaoCurso GetProgramacaoCursoById(int cdProgramacao)
        {
            return daoProgramacaoCurso.findById(cdProgramacao, false);
        }


        #endregion
        #region Item da Programação

        public IEnumerable<ItemProgramacao> getCursoProg(int cdCurso, int cdDuracao, int? cd_escola)
        {
            return daoItemProgramacaoCurso.getCursoProg(cdCurso, cdDuracao, cd_escola);
        }

        public IEnumerable<ItemProgramacao> getItensProgramacaoCursoById(int cdProgramacao)
        {
            return daoItemProgramacaoCurso.getItensProgramacaoCursoById(cdProgramacao);
        }

        #endregion

        #region Atividade Extra

        public IEnumerable<AtividadeExtraUI> searchAtividadeExtra(SearchParameters parametros, DateTime? dataIni,
            DateTime? dataFim, TimeSpan? hrInicial, TimeSpan? hrFinal, int? tipoAtividade, int? curso, int? responsavel,
            int? produto, int? aluno, byte lancada, int cdEscola, int cd_escola_combo)
        {
            IEnumerable<AtividadeExtraUI> retorno = new List<AtividadeExtraUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null) parametros.sort = "dt_atividade_extra";
                parametros.sort = parametros.sort.Replace("dta_atividade_extra", "dt_atividade_extra");
                retorno = daoAtividadeExtra.searchAtividadeExtra(parametros, dataIni, dataFim, hrInicial, hrFinal,
                    tipoAtividade, curso, responsavel, produto, aluno, lancada, cdEscola, cd_escola_combo);
                transaction.Complete();
            }

            return retorno;
        }

        /// <summary>
        /// Método de Inserção para atividade extra
        /// </summary>
        /// <param name="atividadeExtra"></param>
        /// <returns></returns>
        public AtividadeExtraUI addAtividadadeExtra(AtividadeExtraUI atividadeExtra)
        {
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                var newAtividadeExtra = setNewAtividadeExtra(atividadeExtra);
                var dataAtv = atividadeExtra.dta_atividade_extra;
                newAtividadeExtra.dt_atividade_extra = Convert.ToDateTime(dataAtv);

                //if (atividadeExtra != null && 
                //    ((atividadeExtra.atividadesAluno != null && atividadeExtra.atividadesAluno.ToList().Count() > 0) &&
                //    (atividadeExtra.atividadesProspect != null && atividadeExtra.atividadesProspect.ToList().Count() > 0)))
                //{
                //    consistirVagas(atividadeExtra, (atividadeExtra.atividadesAluno.ToList().Count() + atividadeExtra.atividadesProspect.ToList().Count()));
                //}else if (atividadeExtra != null &&
                //         ((atividadeExtra.atividadesAluno == null) &&
                //          (atividadeExtra.atividadesProspect != null && atividadeExtra.atividadesProspect.ToList().Count() > 0)))
                //{
                //    consistirVagas(atividadeExtra, (atividadeExtra.atividadesProspect.ToList().Count()));
                //}else if (atividadeExtra != null &&
                //         ((atividadeExtra.atividadesAluno != null && atividadeExtra.atividadesAluno.ToList().Count() > 0) &&
                //          (atividadeExtra.atividadesProspect == null)))
                //{
                //    consistirVagas(atividadeExtra, (atividadeExtra.atividadesAluno.ToList().Count()));
                //}

                var atividadeExtraBD = daoAtividadeExtra.add(newAtividadeExtra, false);

                //Incluir Atividade do curso
                List<AtividadeCurso> atividadesCursosViewGenerateList = new List<AtividadeCurso>();
                IEnumerable<AtividadeCurso> atividadesCursosView = new List<AtividadeCurso>();
                List<int> atividadesCursosIds = new List<int>();
                atividadesCursosIds = atividadeExtra.cd_cursos;

                if (atividadesCursosIds != null)
                {
                    foreach (var atividadeCursoId in atividadesCursosIds)
                    {
                        if (atividadeCursoId > 0)
                        {
                            AtividadeCurso atividadeCurso = new AtividadeCurso();
                            //atividadeCurso.cd_atividade_curso = 0;
                            atividadeCurso.cd_atividade_extra = atividadeExtraBD.cd_atividade_extra;
                            atividadeCurso.cd_curso = atividadeCursoId;
                            atividadeCurso.cd_pessoa_escola = atividadeExtra.cd_pessoa_escola;
                            daoAtividadeCurso.add(atividadeCurso, false);

                        }
                    }
                }

                //Incluir Atividade do aluno
                if (atividadeExtra.atividadesAluno != null)
                {
                    foreach (var atividadeAluno in atividadeExtra.atividadesAluno)
                    {
                        atividadeAluno.cd_atividade_extra = atividadeExtraBD.cd_atividade_extra;
                        daoAtividadeAluno.add(atividadeAluno, false);
                    }
                }

                if (atividadeExtra.atividadesProspect != null)
                {
                    foreach (var atividadeProspect in atividadeExtra.atividadesProspect)
                    {
                        atividadeProspect.cd_atividade_extra = atividadeExtraBD.cd_atividade_extra;
                        daoAtividadeProspect.add(atividadeProspect, false);
                    }
                }

                atividadeExtra.cd_escola_parametro = atividadeExtra.cd_pessoa_escola;
                consistirVagas(atividadeExtra, daoAtividadeAluno.getNroPessoasAtividade(atividadeExtraBD.cd_atividade_extra));

                var idAtividadeExtra = atividadeExtraBD.cd_atividade_extra;
                atividadeExtra.cd_atividade_extra = atividadeExtraBD.cd_atividade_extra;
                int nm_alunos_students = (int)daoAtividadeAluno.retornNumbersOfStudents(idAtividadeExtra, atividadeExtraBD.cd_pessoa_escola);
                int nm_prospect_students = (int)daoAtividadeProspect.retornNumbersOfStudents(idAtividadeExtra, atividadeExtraBD.cd_pessoa_escola);
                atividadeExtra.nm_alunos = (nm_alunos_students + nm_prospect_students);

                atividadeExtra = AtividadeExtraUI.fromAtividadeExtra(atividadeExtraBD,
                    atividadeExtra.no_tipo_atividade_extra, atividadeExtra.cd_cursos, atividadeExtra.no_produto,
                    atividadeExtra.no_responsavel, atividadeExtra.no_sala, atividadeExtra.no_usuario,
                    atividadeExtra.nm_alunos);
                transaction.Complete();
            }

            return atividadeExtra;
        }


        private void consistirVagas(AtividadeExtraUI atividadeExtra, int qtdAlunosProspects)
        {
            var vagasAtividade = atividadeExtra.nm_vagas;
            string descricao = "";
            var sala = (int)TurmaDataAccess.TipoConsultaTurmaEnum.SALA;
            var vagasSala = 0;
            var vagas = 0;
            bool erro = false;

            if (atividadeExtra.cd_sala != null)
                vagasSala = turmaBusiness.getNumeroVagas((int)atividadeExtra.cd_sala, sala, atividadeExtra.cd_escola_parametro);

            //verifica a vaga que é menor
            if (vagasAtividade != null && vagasAtividade <= vagasSala)
            {
                vagas = (int)vagasAtividade;
                descricao = "atividade extra.";
                erro = true;
            }

            if (vagasSala < vagasAtividade)
            {
                vagas = vagasSala;
                descricao = "sala.";
                erro = true;
            }

            if (vagas < qtdAlunosProspects && erro)
                throw new CoordenacaoBusinessException(string.Format(Messages.msgNumeroVagas, descricao), null,
                    CoordenacaoBusinessException.TipoErro.NUMERO_VAGAS_ATIVIDADE, false);
        }

        private static AtividadeExtra setNewAtividadeExtra(AtividadeExtraUI atividadeExtra)
        {
            AtividadeExtra newAtividadeExtra = new AtividadeExtra
            {
                //cd_curso = atividadeExtra.cd_curso,
                cd_funcionario = atividadeExtra.cd_funcionario,
                cd_produto = atividadeExtra.cd_produto,
                cd_tipo_atividade_extra = atividadeExtra.cd_tipo_atividade_extra,
                dt_atividade_extra = atividadeExtra.dt_atividade_extra,
                hh_final = atividadeExtra.hh_final,
                hh_inicial = atividadeExtra.hh_inicial,
                ind_carga_horaria = atividadeExtra.ind_carga_horaria,
                ind_pagar_professor = atividadeExtra.ind_pagar_professor,
                id_calendario_academico = atividadeExtra.id_calendario_academico,
                hr_limite_academico = atividadeExtra.hr_limite_academico,
                nm_vagas = atividadeExtra.nm_vagas,
                tx_obs_atividade = atividadeExtra.tx_obs_atividade,
                cd_usuario_atendente = atividadeExtra.cd_usuario_atendente,
                cd_sala = atividadeExtra.cd_sala,
                cd_pessoa_escola = atividadeExtra.cd_pessoa_escola,
                AtividadeEscolaAtividade = atividadeExtra.AtividadeEscolaAtividade
            };
            return newAtividadeExtra;
        }

        public int getNroPessoasAtividade(int CdAtividadeExtra)
        {
            return daoAtividadeAluno.getNroPessoasAtividade(CdAtividadeExtra);
        }

        public AtividadeExtraUI editAtividadeExtraOutraEscola(AtividadeExtraUI atividadeExtra, int cdEscola)
        {
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //Persitir atividade aluno
                persistirAtividadesAluno(atividadeExtra, cdEscola);
                persistirAtividadesProspect(atividadeExtra, cdEscola);

                consistirVagas(atividadeExtra, getNroPessoasAtividade(atividadeExtra.cd_atividade_extra));

                int nm_alunos_students = (int)daoAtividadeAluno.retornNumbersOfStudents(atividadeExtra.cd_atividade_extra, cdEscola);
                int nm_prospect_students = (int)daoAtividadeProspect.retornNumbersOfStudents(atividadeExtra.cd_atividade_extra, cdEscola);
                atividadeExtra.nm_alunos = (nm_alunos_students + nm_prospect_students);
                transaction.Complete();
            }
            return atividadeExtra;
        }

        public AtividadeExtraUI editAtividadeExtra(AtividadeExtraUI atividadeExtra)
        {
            AtividadeExtra atividadeExtraBD = new AtividadeExtra();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //if (atividadeExtra != null &&
                //    ((atividadeExtra.atividadesAluno != null && atividadeExtra.atividadesAluno.ToList().Count() > 0) &&
                //     (atividadeExtra.atividadesProspect != null && atividadeExtra.atividadesProspect.ToList().Count() > 0)))
                //{
                //    consistirVagas(atividadeExtra, (atividadeExtra.atividadesAluno.ToList().Count() + atividadeExtra.atividadesProspect.ToList().Count()));
                //}
                //else if (atividadeExtra != null &&
                //         ((atividadeExtra.atividadesAluno == null || (atividadeExtra.atividadesAluno != null && atividadeExtra.atividadesAluno.ToList().Count() == 0)) &&
                //          (atividadeExtra.atividadesProspect != null && atividadeExtra.atividadesProspect.ToList().Count() > 0)))
                //{
                //    consistirVagas(atividadeExtra, (atividadeExtra.atividadesProspect.ToList().Count()));
                //}
                //else if (atividadeExtra != null &&
                //         (atividadeExtra.atividadesAluno != null &&  atividadeExtra.atividadesAluno.ToList().Count() > 0) &&
                //          (atividadeExtra.atividadesProspect == null || (atividadeExtra.atividadesProspect != null && atividadeExtra.atividadesProspect.ToList().Count() == 0)))
                //{
                //    consistirVagas(atividadeExtra, (atividadeExtra.atividadesAluno.ToList().Count()));
                //}

                //Persitir atividade aluno
                persistirAtividadesAluno(atividadeExtra, atividadeExtra.cd_escola);

                //Persitir atividade prospect
                persistirAtividadesProspect(atividadeExtra, atividadeExtra.cd_escola);

                //Persitir atividade curso
                persistirAtividadesCurso(atividadeExtra, atividadeExtra.cd_escola);

                if (daoSala.verificaSalaOnline(atividadeExtra.cd_sala, atividadeExtra.cd_pessoa_escola))
                {
                    List<AtividadeEscolaAtividade> atividadeEscolasContext = daoAtividadeEscola.getAtividadesEscolatByAtividade(atividadeExtra.cd_atividade_extra).ToList();
                    List<AtividadeEscolaAtividade> atividadeEscolasView = atividadeExtra.AtividadeEscolaAtividade.ToList();
                    atividadeExtra.AtividadeEscolaAtividade = null;
                    crudAtividadeEscola(atividadeEscolasContext, atividadeEscolasView);
                }

                //Edita a ativiadade extra
                atividadeExtraBD = daoAtividadeExtra.findById(atividadeExtra.cd_atividade_extra, false);

                consistirVagas(atividadeExtra, getNroPessoasAtividade(atividadeExtraBD.cd_atividade_extra));

                atividadeExtraBD.copy(atividadeExtra);
                atividadeExtraBD.cd_produto = atividadeExtra.cd_produto;
                daoAtividadeExtra.saveChanges(false);

                var idAtividadeExtra = atividadeExtraBD.cd_atividade_extra;
                int nm_alunos_students = (int)daoAtividadeAluno.retornNumbersOfStudents(idAtividadeExtra, atividadeExtraBD.cd_pessoa_escola);
                int nm_prospect_students = (int)daoAtividadeProspect.retornNumbersOfStudents(idAtividadeExtra, atividadeExtraBD.cd_pessoa_escola);
                atividadeExtra.nm_alunos = (nm_alunos_students + nm_prospect_students);

                atividadeExtra = AtividadeExtraUI.fromAtividadeExtra(atividadeExtraBD,
                    atividadeExtra.no_tipo_atividade_extra, atividadeExtra.cd_cursos, atividadeExtra.no_produto,
                    atividadeExtra.no_responsavel, atividadeExtra.no_sala, atividadeExtra.no_usuario,
                    atividadeExtra.nm_alunos);

                transaction.Complete();
            }

            return atividadeExtra;
        }


        public AtividadeExtra findAtividadeExtraById(int cd_atividade_extra)
        {
            AtividadeExtra atividadeExtraBD = new AtividadeExtra();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {

                atividadeExtraBD = daoAtividadeExtra.findById(cd_atividade_extra, false);

                transaction.Complete();
            }

            return atividadeExtraBD;
        }

        private void crudAtividadeEscola(List<AtividadeEscolaAtividade> atividadeEscolasContext, List<AtividadeEscolaAtividade> atividadeEscolasView)
        {
            List<int> cdsEscolasBase = atividadeEscolasContext.Select(x => x.cd_escola).ToList();
            List<int> cdsEscolasView = atividadeEscolasView.Select(x => x.cd_escola).ToList();

            IEnumerable<int> deletarAtividadeEscolas = cdsEscolasBase.Except(cdsEscolasView);
            IEnumerable<int> addAtividadeEscolas = cdsEscolasView.Except(cdsEscolasBase);

            if (addAtividadeEscolas != null && addAtividadeEscolas.Count() > 0)
            {
                foreach (int addAtividadeEscola in addAtividadeEscolas)
                {
                    AtividadeEscolaAtividade atividadeEscola = atividadeEscolasView.Where(x => x.cd_escola == addAtividadeEscola).FirstOrDefault();
                    if (atividadeEscola != null)
                    {
                        daoAtividadeEscola.addContext(atividadeEscola, false);
                    }
                }
                daoAtividadeEscola.saveChanges(false);
            }


            //Deletar as turma escolas que estão no banco e não estão na view.
            if (deletarAtividadeEscolas != null && deletarAtividadeEscolas.Count() > 0)
            {
                foreach (int delAtividade in deletarAtividadeEscolas)
                {
                    AtividadeEscolaAtividade atividadeEscola = atividadeEscolasContext.Where(x => x.cd_escola == delAtividade).FirstOrDefault();
                    if (atividadeEscola != null)
                    {
                        AtividadeEscolaAtividade atividadeEscolaDel = daoAtividadeEscola.getAtividadesEscolatByIdAndAtividade(atividadeEscola.cd_atividade_extra, atividadeEscola.cd_escola, atividadeEscola.cd_atividade_escola).FirstOrDefault();
                        if (atividadeEscolaDel != null)
                        {
                            daoAtividadeEscola.deleteContext(atividadeEscolaDel, false);
                        }
                    }

                }
                daoAtividadeEscola.saveChanges(false);
            }
        }

        public bool verificaSalaOnline(int? cdSala, int cdEscola)
        {
            bool retorno = false;

            retorno = daoSala.verificaSalaOnline(cdSala, cdEscola);
            return retorno;
        }

        private void persistirAtividadesCurso(AtividadeExtraUI atividadeExtra, int cdEscola)
        {
            //Pega as atividades do aluno na base de dados
            IEnumerable<AtividadeCursoUI> atividadesCursosCTX =
                daoAtividadeCurso.searchAtividadeCurso(atividadeExtra.cd_atividade_extra, cdEscola);
            //Pega as atividades da view
            List<AtividadeCurso> atividadesCursosViewGenerateList = new List<AtividadeCurso>();
            IEnumerable<AtividadeCurso> atividadesCursosView = new List<AtividadeCurso>();
            List<int> atividadesCursosIds = new List<int>();
            atividadesCursosIds = atividadeExtra.cd_cursos;

            //monta as atividades curso da view
            if (atividadesCursosIds != null)
            {
                foreach (var atividadeCursoId in atividadesCursosIds)
                {
                    if (atividadeCursoId > 0)
                    {
                        AtividadeCurso atividadeCurso = new AtividadeCurso();
                        atividadeCurso.cd_atividade_curso = 0;
                        atividadeCurso.cd_atividade_extra = atividadeExtra.cd_atividade_extra;
                        atividadeCurso.cd_curso = atividadeCursoId;
                        atividadeCurso.cd_pessoa_escola = atividadeExtra.cd_pessoa_escola;
                        atividadesCursosViewGenerateList.Add(atividadeCurso);

                    }
                }
            }

            atividadesCursosView = atividadesCursosViewGenerateList.AsEnumerable();


            List<int> idsCursoatividadesCursosCTX = new List<int>();

            foreach (var idCursoXTX in atividadesCursosCTX)
            {
                idsCursoatividadesCursosCTX.Add(idCursoXTX.cd_curso);

            }

            if (atividadesCursosView != null)
            {
                //Incluir Atividade do aluno
                foreach (var atividadeCurso in atividadesCursosView)
                {
                    atividadeCurso.cd_atividade_extra = atividadeExtra.cd_atividade_extra;
                    if (!idsCursoatividadesCursosCTX.Contains(atividadeCurso.cd_curso))
                    {
                        var atvCurso = atividadeCurso.cd_atividade_curso == 0
                            ? daoAtividadeCurso.add(atividadeCurso, false)
                            : null;
                    }
                }

                // Filtra as atividade  aluno que possuem código
                var atividadesComCodigo = atividadesCursosView.Where(at => at.cd_atividade_curso != 0);
                var atividadesDeleted = atividadesCursosCTX.Where(atv =>
                    !atividadesComCodigo.Any(a => atv.cd_atividade_curso == a.cd_atividade_curso));


                //Deleta o registro que esta na base mas não esta na view.
                if (atividadesDeleted.Count() > 0)
                {
                    foreach (var item in atividadesDeleted)
                    {
                        if (!atividadesCursosIds.Contains(item.cd_curso))
                        {
                            var deletarAtividadeCurso = daoAtividadeCurso.findById(item.cd_atividade_curso, false);
                            var deleted = deletarAtividadeCurso != null
                                ? daoAtividadeCurso.delete(deletarAtividadeCurso, false)
                                : false;
                        }

                    }
                }

                //Edita os registros da atividade do curso
                foreach (var colecaoCursosVI in atividadesCursosView)
                {
                    foreach (var atividadesBD in atividadesCursosCTX)
                    {
                        if (colecaoCursosVI.cd_atividade_curso == atividadesBD.cd_atividade_curso)
                        {
                            var atividadesCursoBD = daoAtividadeCurso.findById(atividadesBD.cd_atividade_curso, false);
                        }
                    }
                }

                daoAtividadeCurso.saveChanges(false);
            }
            else
            {
                if (atividadesCursosCTX != null)
                {
                    foreach (var item in atividadesCursosCTX)
                    {
                        var deletarAtividadeCurso = daoAtividadeCurso.findById(item.cd_atividade_curso, false);
                        var deleted = deletarAtividadeCurso != null
                            ? daoAtividadeCurso.delete(deletarAtividadeCurso, false)
                            : false;
                    }
                }
            }
        }

        private void persistirAtividadesAluno(AtividadeExtraUI atividadeExtra, int cdEscola)
        {
            //Pega as atividades do aluno na base de dados
            IEnumerable<AtividadeAlunoUI> atividadesAlunosCTX =
                daoAtividadeAluno.searchAtividadeAluno(atividadeExtra.cd_atividade_extra, cdEscola);
            //Pega as atividades da view
            IEnumerable<AtividadeAluno> atividadesAlunosView = new List<AtividadeAluno>();
            atividadesAlunosView = atividadeExtra.atividadesAluno;

            if (atividadesAlunosView != null)
            {
                //Incluir Atividade do aluno
                foreach (var atividadeAluno in atividadesAlunosView)
                {
                    atividadeAluno.cd_atividade_extra = atividadeExtra.cd_atividade_extra;
                    var atvAluno = atividadeAluno.cd_atividade_aluno == 0
                        ? daoAtividadeAluno.add(atividadeAluno, false)
                        : null;
                }

                // Filtra as atividade  aluno que possuem código
                var atividadesComCodigo = atividadesAlunosView.Where(at => at.cd_atividade_aluno != 0);
                var atividadesDeleted = atividadesAlunosCTX.Where(atv =>
                    !atividadesComCodigo.Any(a => atv.cd_atividade_aluno == a.cd_atividade_aluno));

                //Deleta o registro que esta na base mas não esta na view.
                if (atividadesDeleted.Count() > 0)
                {
                    foreach (var item in atividadesDeleted)
                    {
                        var deletarAtividadeAluno = daoAtividadeAluno.findById(item.cd_atividade_aluno, false);
                        var deleted = deletarAtividadeAluno != null
                            ? daoAtividadeAluno.delete(deletarAtividadeAluno, false)
                            : false;
                    }
                }

                //Edita os registros da atividade do aluno
                foreach (var colecaoAlunosVI in atividadesAlunosView)
                {
                    foreach (var atividadesBD in atividadesAlunosCTX)
                    {
                        if (colecaoAlunosVI.cd_atividade_aluno == atividadesBD.cd_atividade_aluno)
                        {
                            var atividadesAlunoBD = daoAtividadeAluno.findById(atividadesBD.cd_atividade_aluno, false);
                            atividadesAlunoBD.ind_participacao =
                                atividadesAlunoBD.ind_participacao == colecaoAlunosVI.ind_participacao
                                    ? atividadesAlunoBD.ind_participacao
                                    : colecaoAlunosVI.ind_participacao;
                            atividadesAlunoBD.tx_obs_atividade_aluno =
                                atividadesAlunoBD.tx_obs_atividade_aluno == colecaoAlunosVI.tx_obs_atividade_aluno
                                    ? atividadesAlunoBD.tx_obs_atividade_aluno
                                    : colecaoAlunosVI.tx_obs_atividade_aluno;
                        }
                    }
                }

                daoAtividadeAluno.saveChanges(false);
            }
            else
            {
                if (atividadesAlunosCTX != null)
                {
                    foreach (var item in atividadesAlunosCTX)
                    {
                        var deletarAtividadeAluno = daoAtividadeAluno.findById(item.cd_atividade_aluno, false);
                        var deleted = deletarAtividadeAluno != null
                            ? daoAtividadeAluno.delete(deletarAtividadeAluno, false)
                            : false;
                    }
                }
            }
        }

        private void persistirAtividadesProspect(AtividadeExtraUI atividadeExtra, int cdEscola)
        {
            //Pega as atividades do prospect na base de dados
            IEnumerable<AtividadeProspectUI> atividadesProspectCTX =
                daoAtividadeProspect.searchAtividadeProspect(atividadeExtra.cd_atividade_extra, cdEscola);
            //Pega as atividades da view
            IEnumerable<AtividadeProspect> atividadesProspectsView = new List<AtividadeProspect>();
            atividadesProspectsView = atividadeExtra.atividadesProspect;

            if (atividadesProspectsView != null)
            {
                //Incluir Atividade do prospect
                foreach (var atividadeProspect in atividadesProspectsView)
                {
                    atividadeProspect.cd_atividade_extra = atividadeExtra.cd_atividade_extra;
                    var atvProspect = atividadeProspect.cd_atividade_prospect == 0
                        ? daoAtividadeProspect.add(atividadeProspect, false)
                        : null;
                }

                // Filtra as atividade  prospect que possuem código
                var atividadesComCodigo = atividadesProspectsView.Where(at => at.cd_atividade_prospect != 0);
                var atividadesDeleted = atividadesProspectCTX.Where(atv =>
                    !atividadesComCodigo.Any(a => atv.cd_atividade_prospect == a.cd_atividade_prospect));

                //Deleta o registro que esta na base mas não esta na view.
                if (atividadesDeleted.Count() > 0)
                {
                    foreach (var item in atividadesDeleted)
                    {
                        var deletarAtividadeProspect = daoAtividadeProspect.findById(item.cd_atividade_prospect, false);
                        var deleted = deletarAtividadeProspect != null
                            ? daoAtividadeProspect.delete(deletarAtividadeProspect, false)
                            : false;
                    }
                }

                //Edita os registros da atividade do aluno
                foreach (var colecaoProspectVI in atividadesProspectsView)
                {
                    foreach (var atividadesBD in atividadesProspectCTX)
                    {
                        if (colecaoProspectVI.cd_atividade_prospect == atividadesBD.cd_atividade_prospect)
                        {
                            var atividadesProspectBD = daoAtividadeProspect.findById(atividadesBD.cd_atividade_prospect, false);
                            atividadesProspectBD.ind_participacao =
                                atividadesProspectBD.ind_participacao == colecaoProspectVI.ind_participacao
                                    ? atividadesProspectBD.ind_participacao
                                    : colecaoProspectVI.ind_participacao;
                            atividadesProspectBD.txt_obs_atividade_propspect =
                                atividadesProspectBD.txt_obs_atividade_propspect == colecaoProspectVI.txt_obs_atividade_propspect
                                    ? atividadesProspectBD.txt_obs_atividade_propspect
                                    : colecaoProspectVI.txt_obs_atividade_propspect;
                        }
                    }
                }

                daoAtividadeProspect.saveChanges(false);
            }
            else
            {
                if (atividadesProspectCTX != null)
                {
                    foreach (var item in atividadesProspectCTX)
                    {
                        var deletarAtividadeProspect = daoAtividadeProspect.findById(item.cd_atividade_prospect, false);
                        var deleted = deletarAtividadeProspect != null
                            ? daoAtividadeProspect.delete(deletarAtividadeProspect, false)
                            : false;
                    }
                }
            }
        }

        public bool deleteAllAtividadeExtra(List<AtividadeExtra> atividadesExtras, int cd_escola)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                long alunos = 0;
                long prospects = 0;
                foreach (var item in atividadesExtras)
                {
                    alunos = daoAtividadeAluno.retornAllNumbersOfStudents(item.cd_atividade_extra);
                    if (alunos > 0)
                        throw new CoordenacaoBusinessException(Messages.msgAlunosComAtividadeExtra, null,
                            CoordenacaoBusinessException.TipoErro.ALUNOS_ATIVIDADE_EXTRA, false);

                    prospects = daoAtividadeProspect.retornAllNumbersOfStudents(item.cd_atividade_extra);
                    if (prospects > 0)
                        throw new CoordenacaoBusinessException(Messages.msgProspectComAtividadeExtra, null,
                            CoordenacaoBusinessException.TipoErro.PROSPECT_ATIVIDADE_EXTRA, false);

                    var atividadeExtra = daoAtividadeExtra.findById(item.cd_atividade_extra, false);
                    if (atividadeExtra != null)
                    {

                        IEnumerable<AtividadeCursoUI> atividadesCursosCTX =
                            daoAtividadeCurso.searchAtividadeCurso(atividadeExtra.cd_atividade_extra, cd_escola);
                        foreach (var atividade in atividadesCursosCTX)
                        {
                            var deletarItem = daoAtividadeCurso.findById(atividade.cd_atividade_curso, false);
                            var del = deletarItem != null ? daoAtividadeCurso.delete(deletarItem, false) : false;
                        }

                        deleted = daoAtividadeExtra.delete(atividadeExtra, false);
                    }


                }

                transaction.Complete();
            }

            return deleted;
        }

        public bool deleteRecorrencias(AtividadeExtra atividadeExtra, int cd_escola)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                AtividadeRecorrencia atividadeRecorrencia = daoAtividadeRecorrencia.searchAtividadeRecorrenciaByCdAtividadeExtra(atividadeExtra.cd_atividade_extra, cd_escola);


                if (atividadeRecorrencia != null)
                {
                    List<AtividadeExtra> atividadesExtraDeleted = daoAtividadeExtra.getAtividadeExtraByCdAtividadeRecorrencia(atividadeRecorrencia.cd_atividade_recorrencia);
                    foreach (AtividadeExtra atividadeExtraDel in atividadesExtraDeleted)
                    {
                        List<AtividadeCurso> atividadeCurso = daoAtividadeCurso.searchAtividadesCursoBycdAtividadExtra(atividadeExtraDel.cd_atividade_extra, cd_escola);
                        List<AtividadeEscolaAtividade> atividadeEscola = daoAtividadeEscola.getAtividadesEscolatByAtividade(atividadeExtraDel.cd_atividade_extra).ToList();
                        List<AtividadeAluno> atividadeAlunos = daoAtividadeAluno.searchAtividadeAlunoByCdAtividadeExtra(atividadeExtraDel.cd_atividade_extra, cd_escola).ToList();
                        List<AtividadeProspect> atividadeProspects = daoAtividadeProspect.searchAtividadeProspectByCdAtividadeExtra(atividadeExtraDel.cd_atividade_extra, cd_escola).ToList();

                        if (atividadeCurso != null && atividadeCurso.Count > 0)
                        {
                            daoAtividadeCurso.deleteRange(atividadeCurso, false);
                        }
                        if (atividadeEscola != null && atividadeEscola.Count > 0)
                        {
                            daoAtividadeEscola.deleteRange(atividadeEscola, false);
                        }
                        if (atividadeAlunos != null && atividadeAlunos.Count > 0)
                        {
                            daoAtividadeAluno.deleteRange(atividadeAlunos, false);
                        }
                        if (atividadeProspects != null && atividadeProspects.Count > 0)
                        {
                            daoAtividadeProspect.deleteRange(atividadeProspects, false);
                        }

                    }

                    deleted = daoAtividadeRecorrencia.delete(atividadeRecorrencia, false);
                    transaction.Complete();
                }
                else
                {
                    throw new CoordenacaoBusinessException(Messages.msgErroAtividadeExtraRecorrenciaNulo, null,
                        CoordenacaoBusinessException.TipoErro.ERRO_ATIVIDADE_RECORRENCIA_NULO, false);
                }


            }

            return deleted;
        }

        public List<String> enviarEmailRecorrencias(AtividadeExtra atividadeExtra, int cd_escola)
        {
            List<String> retorno = new List<string>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                SendEmail sendEmail = new SendEmail();
                sendEmail.assunto = " Agendamento aula Experimental";
                SendEmail.configurarEmailSection(sendEmail);

                if (atividadeExtra == null)
                {
                    throw new CoordenacaoBusinessException(Messages.msgErroAtividadeExtraEnviarEmailNulo, null,
                        CoordenacaoBusinessException.TipoErro.ERRO_ATIVIDADE_EXTRA_ENVIAR_EMAIL_NULO, false);
                }

                AtividadeRecorrencia atividadeRecorrencia = daoAtividadeRecorrencia.searchAtividadeRecorrenciaByCdAtividadeExtra(atividadeExtra.cd_atividade_extra, cd_escola);

                if (atividadeRecorrencia == null)
                {
                    throw new CoordenacaoBusinessException(Messages.msgErroAtividadeExtraRecorrenciaEnviarEmailNulo, null,
                        CoordenacaoBusinessException.TipoErro.ERRO_ATIVIDADE_RECORRENCIA_ENVIAR_EMAIL_NULO, false);

                }


                List<AtividadeExtra> atividadesExtraGeradas = daoAtividadeExtra.getAtividadeExtraByCdAtividadeRecorrencia(atividadeRecorrencia.cd_atividade_recorrencia);
                List<String> atividadeAlunosEmails = new List<string>();
                List<String> atividadeProspectsEmails = new List<string>();
                List<String> prospectsAlunosEmailsSend = new List<string>();

                foreach (AtividadeExtra atividadeExtraGerada in atividadesExtraGeradas)
                {
                    //List<String> atividadeAlunos = daoAtividadeAluno.searchEmailsAtividadeAlunoByCdAtividadeExtra(atividadeExtraGerada.cd_atividade_extra, cd_escola).ToList();
                    List<String> atividadeProspects = daoAtividadeProspect.searchAtividadeEmailsProspectByCdAtividadeExtra(atividadeExtraGerada.cd_atividade_extra, cd_escola).ToList();

                    //if (atividadeAlunos != null && atividadeAlunos.Count > 0)
                    //{
                    //    atividadeAlunosEmails.AddRange(atividadeAlunos);
                    //}

                    if (atividadeProspects != null && atividadeProspects.Count > 0)
                    {
                        atividadeProspectsEmails.AddRange(atividadeProspects);
                    }
                }

                //prospectsAlunosEmailsSend = atividadeAlunosEmails.Union(atividadeProspectsEmails).Distinct().ToList();
                prospectsAlunosEmailsSend = atividadeProspectsEmails.Distinct().ToList();

                if (prospectsAlunosEmailsSend != null && prospectsAlunosEmailsSend.Count > 0)
                {
                    foreach (var prospectAlunoEmail in prospectsAlunosEmailsSend)
                    {
                        sendEmail.destinatario = prospectAlunoEmail;


                        StringBuilder mensagem = new StringBuilder();
                        mensagem.Append("<meta content=\"text/html; charset=windows-1252\" http-equiv=\"Content-Type\" />");
                        mensagem.Append("<meta content=\"Microsoft Word 15 (filtered)\" name=\"Generator\" />");
                        mensagem.Append("<title>Fisk Centro de Ensino</title>");
                        mensagem.Append("<style></style>");
                        mensagem.Append("<div class=\"WordSection1\">");
                        //mensagem.Append("<p class="MsoNormal"><br /></p><p class="MsoNormal">Prezado(a) #nomecompleto#</p>");
                        mensagem.Append("<p class=\"MsoNormal\"><br /></p><p class=\"MsoNormal\">Seguem Agendamento da(s) seguinte(s) aula(s) Exprimental(is):<br /></p>");
                        foreach (AtividadeExtra atividadeExtraRecorrenciaGerada in atividadesExtraGeradas)
                        {
                            string mensagemAtividade = "<p class=\"MsoNormal\">Dia #data# das #horaInicial# ás #horaFinal#<br /></p>"
                                .Replace("#data#", atividadeExtraRecorrenciaGerada.dta_atividade_extra.ToString())
                                .Replace("#horaInicial#", atividadeExtraRecorrenciaGerada.hh_inicial.ToString())
                                .Replace("#horaFinal#", atividadeExtraRecorrenciaGerada.hh_final.ToString());
                            mensagem.Append(mensagemAtividade);
                        }

                        mensagem.Append("<p class=\"MsoNormal\"><br /></p>");
                        mensagem.Append("<p class=\"MsoNormal\"><br /></p>");
                        mensagem.Append("</div>");

                        sendEmail.mensagem = mensagem.ToString();
                        bool enviado = SendEmail.EnviarEmail(sendEmail);
                        if (!enviado)
                        {
                            retorno.Add(string.Format(Messages.msgErroSendEmailProspect, "", prospectAlunoEmail));
                        }
                        else
                        {

                            foreach (var atividadeGerada in atividadesExtraGeradas)
                            {
                                AtividadeProspect atividadeProspectCtx = daoAtividadeProspect.searchAtividadeProspectByCdAtividadeExtraAndEmailProspect(atividadeGerada.cd_atividade_extra, prospectAlunoEmail);

                                if (atividadeProspectCtx != null && atividadeProspectCtx.id_email_enviado == false)
                                {
                                    atividadeProspectCtx.id_email_enviado = true;
                                    daoAtividadeProspect.saveChanges(false);
                                }
                                else if (atividadeProspectCtx != null && atividadeProspectCtx.id_email_enviado == true)
                                {
                                    retorno.Add(string.Format(Messages.msgErroEmailJaEnviadoAtividadeExtra, (atividadeGerada.no_tipo_atividade_extra + " - " + atividadeGerada.dta_atividade_extra + "(" + atividadeGerada.hh_inicial.ToString(@"hh\:mm") + "/" + atividadeGerada.hh_final.ToString(@"hh\:mm") + ")")));
                                }

                            }

                        }
                    }

                    //if (retorno.Count == 0)
                    //{
                    //    //atualizarEmail
                    //    AtividadeExtra atividadeExtraContext = daoAtividadeExtra.findById(atividadeExtra.cd_atividade_extra, false);
                    //    atividadeExtraContext.id_email_enviado = true;

                    //    atividadesExtraGeradas.ForEach(x => { x.id_email_enviado = true;});

                    //    daoAtividadeExtra.saveChanges(false);
                    //}

                    transaction.Complete();
                    return retorno;

                }

            }

            return retorno;
        }


        public List<String> enviarEmailProspectsAcaoRelacionada(List<AtividadeExtra> atividadeExtra, int cd_escola)
        {
            List<String> retorno = new List<string>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                SendEmail sendEmail = new SendEmail();
                sendEmail.assunto = " Agendamento aula Experimental";
                SendEmail.configurarEmailSection(sendEmail);

                if (atividadeExtra == null || (atividadeExtra != null && atividadeExtra.Count == 0))
                {
                    throw new CoordenacaoBusinessException(Messages.msgErroAtividadeExtraEnviarEmailNulo, null,
                        CoordenacaoBusinessException.TipoErro.ERRO_ATIVIDADE_EXTRA_ENVIAR_EMAIL_NULO, false);
                }


                foreach (var atividade in atividadeExtra)
                {
                    if (atividade.cd_tipo_atividade_extra == (int)AtividadeExtra.EnumTipoAtividade.AULAEXPERIMENTAL && atividade.id_email_enviado == false)
                    {
                        List<ContatoProspectAtividadeExtraUI> contatosProspect = daoAtividadeProspect.searchContatoProspectByCdAtividadeExtra(atividade.cd_atividade_extra, cd_escola).ToList();

                        if (contatosProspect == null || (contatosProspect != null && contatosProspect.Count == 0))
                        {
                            retorno.Add(string.Format(Messages.msgAvisoEmailProspect));
                        }

                        foreach (var contatoProspect in contatosProspect)
                        {
                            if (contatoProspect != null)
                            {

                                AtividadeProspect atividadeProspectCtx = daoAtividadeProspect.searchAtividadeProspectByCdAtividadeExtraAndEmailProspect(atividade.cd_atividade_extra, contatoProspect.email);

                                if (atividadeProspectCtx != null && atividadeProspectCtx.id_email_enviado == false)
                                {



                                    sendEmail.destinatario = contatoProspect.email;

                                    StringBuilder mensagem = new StringBuilder();
                                    mensagem.Append("<meta content=\"text/html; charset=windows-1252\" http-equiv=\"Content-Type\" />");
                                    mensagem.Append("<meta content=\"Microsoft Word 15 (filtered)\" name=\"Generator\" />");
                                    mensagem.Append("<title>Fisk Centro de Ensino</title>");
                                    mensagem.Append("<style></style>");
                                    mensagem.Append("<div class=\"WordSection1\">");
                                    mensagem.Append("<p class=\"MsoNormal\"><br /></p><p class=\"MsoNormal\">Prezado(a) #nomecompleto#</p>"
                                        .Replace("#nomecompleto#", contatoProspect.no_pessoa));
                                    mensagem.Append("<p class=\"MsoNormal\"><br /></p><p class=\"MsoNormal\">Seguem Agendamento da(s) seguinte(s) aula(s) Exprimental(is):<br /></p>");

                                    string mensagemAtividade = "<p class=\"MsoNormal\">Dia #data# das #horaInicial# ás #horaFinal#<br /></p>"
                                        .Replace("#data#", atividade.dta_atividade_extra.ToString())
                                        .Replace("#horaInicial#", atividade.hh_inicial.ToString())
                                        .Replace("#horaFinal#", atividade.hh_final.ToString());
                                    mensagem.Append(mensagemAtividade);


                                    mensagem.Append("<p class=\"MsoNormal\"><br /></p>");
                                    mensagem.Append("<p class=\"MsoNormal\"><br /></p>");
                                    mensagem.Append("</div>");

                                    sendEmail.mensagem = mensagem.ToString();
                                    bool enviado = SendEmail.EnviarEmail(sendEmail);
                                    if (!enviado)
                                    {
                                        retorno.Add(string.Format(Messages.msgErroSendEmailProspect, contatoProspect.no_pessoa, contatoProspect.email));
                                    }
                                    else
                                    {
                                        atividadeProspectCtx.id_email_enviado = true;
                                        daoAtividadeProspect.saveChanges(false);
                                    }

                                }
                                else if (atividadeProspectCtx != null && atividadeProspectCtx.id_email_enviado == true)
                                {
                                    retorno.Add(string.Format(Messages.msgErroEmailJaEnviadoAtividadeExtra, (atividade.no_tipo_atividade_extra + " - " + atividade.dta_atividade_extra + "(" + atividade.hh_inicial.ToString(@"hh\:mm") + "/" + atividade.hh_final.ToString(@"hh\:mm") + ")")));
                                }

                            }

                        }


                        //if (retorno.Count == 0)
                        //{
                        //    //atualizarEmail
                        //    AtividadeExtra atividadeExtraContext = daoAtividadeExtra.findById(atividade.cd_atividade_extra, false);
                        //    atividadeExtraContext.id_email_enviado = true;

                        //    daoAtividadeExtra.saveChanges(false);
                        //}
                    }
                    //else if(atividade.cd_tipo_atividade_extra == (int)AtividadeExtra.EnumTipoAtividade.AULAEXPERIMENTAL && atividade.id_email_enviado == true)
                    //{
                    //    retorno.Add(string.Format(Messages.msgErroEmailJaEnviadoAtividadeExtra, (atividade.no_tipo_atividade_extra + " - " + atividade.dta_atividade_extra + "(" + atividade.hh_inicial.ToString(@"hh\:mm") + "/" + atividade.hh_final.ToString(@"hh\:mm") + ")")));
                    //}
                }

                transaction.Complete();
                return retorno;

            }

        }



        public AtividadeExtra findByIdAtividadeExtraFull(int cdAtividadeExtra)
        {
            AtividadeExtra retorno = new AtividadeExtra();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.UNCOMMITED, daoSala.DB(),
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = daoAtividadeExtra.findByIdAtividadeExtraFull(cdAtividadeExtra);
                transaction.Complete();
            }

            return retorno;
        }

        public List<sp_RptAtividadeExtra_Result> getReportAtividadeExtra(Nullable<int> cd_escola,
            Nullable<System.DateTime> dta_ini, Nullable<System.DateTime> dta_fim, Nullable<int> cd_produto,
            Nullable<int> cd_curso, Nullable<int> cd_funcionario, Nullable<int> cd_aluno,
            Nullable<byte> id_participacao, Nullable<byte> id_lancada)
        {
            List<sp_RptAtividadeExtra_Result> retorno = new List<sp_RptAtividadeExtra_Result>();

            retorno = daoAtividadeExtra.getReportAtividadeExtra(cd_escola, dta_ini, dta_fim, cd_produto, cd_curso,
                cd_funcionario, cd_aluno, id_participacao, id_lancada);
            return retorno;
        }

        public List<sp_RptControleFaltas_Result> getReportControleFaltasResults(Nullable<int> cd_tipo, int cd_escola, Nullable<int> cd_curso, Nullable<int> cd_nivel,
            Nullable<int> cd_produto, Nullable<int> cd_professor, Nullable<int> cd_turma, Nullable<int> cd_sit_turma, string cd_sit_aluno, string dt_inicial,
            string dt_final, bool quebrarpagina)
        {
            List<sp_RptControleFaltas_Result> retorno = new List<sp_RptControleFaltas_Result>();

            retorno = daoControleFaltas.getReportControleFaltasResults(cd_tipo, cd_escola, cd_curso, cd_nivel, cd_produto, cd_professor, cd_turma, cd_sit_turma, cd_sit_aluno, dt_inicial, dt_final, quebrarpagina);
            return retorno;
        }

        public List<sp_RptAtividadeExtraAluno_Result> getReportAtividadeExtraAluno(Nullable<int> cd_atividade_extra,
            Nullable<int> cd_aluno, Nullable<byte> id_participou, Nullable<byte> id_lancada, Nullable<int> cd_escola)
        {
            List<sp_RptAtividadeExtraAluno_Result> retorno = new List<sp_RptAtividadeExtraAluno_Result>();

            retorno = daoAtividadeExtra.getReportAtividadeExtraAluno(cd_atividade_extra, cd_aluno, id_participou, id_lancada, cd_escola);
            return retorno;
        }

        public AtividadeExtraUI returnAtividadeExtraUsuarioAtendente(int cd_atividade_extra, int cd_pessoa_escola)
        {
            return daoAtividadeExtra.returnAtividadeExtraUsuarioAtendente(cd_atividade_extra, cd_pessoa_escola);
        }

        #endregion

        #region Atividade Aluno

        public IEnumerable<AtividadeAlunoUI> searchAtividadeAluno(int cdAtividadeExtra, int cdEscola)
        {
            IEnumerable<AtividadeAlunoUI> retorno = new List<AtividadeAlunoUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoAtividadeAluno.searchAtividadeAluno(cdAtividadeExtra, cdEscola);
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<AtividadeAlunoUI> searchAtividadeAlunoReport(int cdAtividadeExtra, int cdEscola)
        {
            IEnumerable<AtividadeAlunoUI> retorno = new List<AtividadeAlunoUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoAtividadeAluno.searchAtividadeAlunoReport(cdAtividadeExtra, cdEscola);
                transaction.Complete();
            }

            return retorno;
        }

        public long retornNumbersOfStudents(int idAtividadeExtra, int cdEscola)
        {
            return daoAtividadeAluno.retornNumbersOfStudents(idAtividadeExtra, cdEscola);
        }

        #endregion

        #region Atividade Prospect

        public IEnumerable<AtividadeProspectUI> searchAtividadeProspect(int cdAtividadeExtra, int cdEscola)
        {
            IEnumerable<AtividadeProspectUI> retorno = new List<AtividadeProspectUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoAtividadeProspect.searchAtividadeProspect(cdAtividadeExtra, cdEscola);
                transaction.Complete();
            }

            return retorno;
        }

        public long retornNumbersOfStudentsProspect(int idAtividadeExtra, int cdEscola)
        {
            return daoAtividadeProspect.retornNumbersOfStudents(idAtividadeExtra, cdEscola);
        }

        public IEnumerable<AtividadeProspectUI> searchAtividadeProspectByCdProspect(int cdProspect, int cdEscola)
        {
            IEnumerable<AtividadeProspectUI> retorno = new List<AtividadeProspectUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoAtividadeProspect.searchAtividadeProspectByCdProspect(cdProspect, cdEscola);
                transaction.Complete();
            }

            return retorno;
        }

        #endregion

        #region Desconto por Antecipação

        public PoliticaDescontoUI postPoliticaDesconto(PoliticaDesconto politicaDesconto)
        {
            PoliticaDescontoUI retorno = new PoliticaDescontoUI();

            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                retorno = financeiroBusiness.postPoliticaDesconto(politicaDesconto);
                transaction.Complete();
            }

            return retorno;
        }

        #endregion

        #region Turma

        public List<Turma> criarNomeTurmaFilhas(List<Turma> alunosTurmaPPT, Turma turmaPaiPPT, bool alterarNomePPTFilha, int cd_escola)
        {

            foreach (var item in alunosTurmaPPT)
            {
                Turma novaTurma = new Turma();
                //LBMPPT Usar esta linha se for a mesma escola
                //item.cd_pessoa_escola = turmaPaiPPT.cd_pessoa_escola;
                //LBMPPT Escola diferente
                item.cd_pessoa_escola = cd_escola; // turmaPaiPPT.cd_pessoa_escola;
                if (item.cd_turma.Equals(null) || item.cd_turma == 0 || alterarNomePPTFilha)
                {

                    List<Horario> listaHor = turmaPaiPPT.horariosTurma != null
                        ? turmaPaiPPT.horariosTurma.ToList()
                        : new List<Horario>();
                    var nomeTurma = criarNomeTurma(item.id_turma_ppt, 1, turmaPaiPPT.cd_regime, turmaPaiPPT.cd_produto,
                        item.cd_curso, listaHor, turmaPaiPPT.dt_inicio_aula);

                    item.no_turma = nomeTurma;
                }

            }

            return alunosTurmaPPT;
        }

        public TurmaSearch addTurma(Turma turma)
        {
            List<ProgramacaoTurma> programacoesNovas = new List<ProgramacaoTurma>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.READ_COMMITED, daoProgramacaoTurma.DB(),
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                this.sincronizarContextos(daoProgramacaoTurma.DB());
                ICollection<ProgramacaoTurma> programacaoTurma = turma.ProgramacaoTurma;
                ICollection<FeriadoDesconsiderado> feriadosDesconsiderados = turma.FeriadosDesconsiderados;
                List<ProgramacaoTurma> programacoesTurma = new List<ProgramacaoTurma>();
                if (businessApiNewCyber.aplicaApiCyber())
                {
                    //Turma do banco
                    Turma turmaBdOld = turmaBusiness.findTurmaByCdTurmaApiCyber(turma.cd_turma);

                    //Pega os professores ativos da turma no banco(Old)
                    List<ProfessorTurma> listProfessorTurmaBdOld = turmaBusiness.findProfessorTurmaByCdTurma(turma.cd_turma, turma.cd_pessoa_escola);

                    //monta a lista com professores turma do banco (old)
                    TurmaApiCyberBdUI turmaApiCyberOld = new TurmaApiCyberBdUI();
                    turmaApiCyberOld = turmaBusiness.findTurmaApiCyber(turma.cd_turma, turma.cd_pessoa_escola);

                    if (turmaApiCyberOld != null && listProfessorTurmaBdOld != null && listProfessorTurmaBdOld.Count > 0)
                    {
                        //converte a lista de ids em uma string com delimitador (",")
                        turmaApiCyberOld.codigo_professor = string.Join("|", listProfessorTurmaBdOld.Select(x => x.cd_professor.ToString()).ToArray());

                    }
                }


                List<Horario> listaHor =
                    turma.horariosTurma != null ? turma.horariosTurma.ToList() : new List<Horario>();
                turma.dt_inicio_aula = turma.dt_inicio_aula.Date;

                var nomeTurma = criarNomeTurma(turma.id_turma_ppt, turma.cd_turma_ppt, turma.cd_regime,
                    turma.cd_produto, turma.cd_curso, listaHor, turma.dt_inicio_aula);
                int nrProximaTurma = turmaBusiness.verificaExisteTurma(nomeTurma, turma.cd_pessoa_escola, 0);

                //Verifica se existe algum horário da turma que interceda com o horário da sala:
                if (!turma.cd_turma_ppt.HasValue && turma.cd_sala != null &&
                    !getSalasDisponiveisPorHorariosEscrita(turma, turma.cd_pessoa_escola).ToList()
                        .Where(t => t.cd_sala == turma.cd_sala).Any())
                    throw new TurmaBusinessException(Messages.msgErroHorariosDisponiveisTurmaSala, null,
                        TurmaBusinessException.TipoErro.ERRO_HORARIOS_DISPONIVEIS_TURMA_SALA, false);

                nomeTurma = nomeTurma + "-" + nrProximaTurma;

                turma.no_turma = nomeTurma;
                turma.nm_turma = nrProximaTurma;

                // Atualiza a data final com a data da programação da turma:

                if (turma.ProgramacaoTurma != null)
                {
                    programacoesTurma = turma.ProgramacaoTurma.ToList();
                    if (programacoesTurma.Count >= 1)
                        turma.dt_final_aula = programacoesTurma[programacoesTurma.Count - 1].dta_programacao_turma.Date;
                    programacoesNovas = turma.ProgramacaoTurma.Where(p=>p.cd_programacao_turma == 0).ToList();

                }

                turma.dt_inicio_aula = turma.dt_inicio_aula.Date;
                if (turma.dt_final_aula != null)
                    turma.dt_final_aula = ((DateTime)turma.dt_final_aula).Date;
                turma.nro_aulas_programadas = (byte)programacoesTurma.Where(pt =>
                    (pt.cd_feriado == null) || (pt.cd_feriado != null && pt.cd_feriado_desconsiderado != null)).Count();
                Turma turmaContext = turmaBusiness.addTurma(turma);

                if (turma.cd_turma_ppt == null || turma.cd_turma_ppt == 0)
                {
                    if (turma.horariosTurma != null)
                        turmaBusiness.crudHorariosTurma(turma.horariosTurma.ToList(), turma.cd_turma,
                            turma.cd_pessoa_escola, Turma.TipoTurma.NORMAL);
                    if (turma.id_turma_ppt && turma.alunosTurmasPPT != null)
                    {
                        Turma turmaPai = turmaBusiness.getTurmaEHorarios(turma.cd_turma, turma.cd_pessoa_escola);
                        turmaBusiness.crudAlunosTurmasPPT(
                            criarNomeTurmaFilhas(turma.alunosTurmasPPT.ToList(), turmaPai, true, turma.cd_pessoa_escola), turmaPai, true, turma.cd_pessoa_escola, true);
                    }

                    if (!turma.id_turma_ppt && turma.alunosTurma != null)
                        turmaBusiness.crudAlunosTurma(turma.alunosTurma.ToList(), turma);
                    if (turma.ProfessorTurma != null)
                        turmaBusiness.crudProfessoresTurma(turma.ProfessorTurma.ToList(), turma, turma.horariosTurma,
                            false);
                }
                else
                {
                    if (turma.horariosTurma != null)
                        turmaBusiness.crudHorariosTurma(
                            Horario.clonarHorariosZerandoMemoria(turma.horariosTurma.ToList(), "Calendar2"),
                            turma.cd_turma, turma.cd_pessoa_escola, Turma.TipoTurma.NORMAL);
                    else
                        turmaBusiness.crudHorariosTurma(new List<Horario>(), turma.cd_turma, turma.cd_pessoa_escola,
                            Turma.TipoTurma.NORMAL);
                    if (!turma.id_turma_ppt && turma.alunosTurma != null)
                        turmaBusiness.crudAlunosTurma(turma.alunosTurma.ToList(), turma);
                    if (turma.ProfessorTurma != null)
                        turmaBusiness.crudProfessoresTurma(
                            ProfessorTurma.clonarProfessorZerandoMemoria(turma.ProfessorTurma.ToList(), turma.cd_turma),
                            turma, turma.horariosTurma, false);

                }
                // query em produtos com cd_produto = turma.cd_produto
                var produto = daoProduto.findById(turma.cd_produto,false);
                var limite = 100;
                if (produto.no_produto == "Empreendedorismo") limite = 60;
                var minutosSemana = (from t in turma.horariosTurma select (t.dt_hora_fim - t.dt_hora_ini).TotalMinutes).Sum();
                if (minutosSemana < limite)
                    throw new TurmaBusinessException(string.Format(Utils.Messages.Messages.msgErroCargaHorariaMinima, limite), null,
                        TurmaBusinessException.TipoErro.ERRO_CARGA_HORARIA_MINIMA, false);

                //turmaBusiness.atualizarAulasProgramadasTurma(turmaContext);

                if (businessApiNewCyber.aplicaApiCyber())
                {
                    //Turma do banco
                    Turma turmaBdCurrent = turmaBusiness.findTurmaByCdTurmaApiCyber(turma.cd_turma);

                    //Pega os professores ativos da turma no banco(Current)
                    List<ProfessorTurma> listProfessorTurmaBdCurrent = turmaBusiness.findProfessorTurmaByCdTurma(turma.cd_turma, turma.cd_pessoa_escola);

                    //Percorre a lista de professores e verifica se já existem no cyber,
                    //se não, cadastra o professor no cyber
                    foreach (ProfessorTurma professorTurma in listProfessorTurmaBdCurrent)
                    {
                        FuncionarioCyberBdUI funcionarioCyberBdCurrent = turmaBusiness.findFuncionarioByCdFuncionario(professorTurma.cd_professor, turma.cd_pessoa_escola);

                        if (funcionarioCyberBdCurrent != null && funcionarioCyberBdCurrent.id_unidade != null && funcionarioCyberBdCurrent.id_unidade > 0 &&
                            funcionarioCyberBdCurrent.funcionario_ativo == true)
                        {
                            //Chama a apiCyber de acordo com o tipo de funcionario
                            verificaTipoFuncionarioPostApiCyber(funcionarioCyberBdCurrent);
                        }
                    }


                    //monta a lista com professores turma do banco (Current)
                    TurmaApiCyberBdUI turmaApiCyberCurrent = new TurmaApiCyberBdUI();
                    turmaApiCyberCurrent = turmaBusiness.findTurmaApiCyber(turma.cd_turma, turma.cd_pessoa_escola);

                    if (turmaApiCyberCurrent != null && listProfessorTurmaBdCurrent != null && listProfessorTurmaBdCurrent.Count > 0)
                    {
                        //converte a lista de ids em uma string com delimitador (",")
                        turmaApiCyberCurrent.codigo_professor = string.Join("|", listProfessorTurmaBdCurrent.Select(x => x.cd_professor.ToString()).ToArray());
                    }



                    //se inseriu professor ativo chama api cyber com comando (cadastra grupo)
                    if (turmaApiCyberCurrent != null &&
                        turmaApiCyberCurrent.codigo_professor != null && !string.IsNullOrEmpty(turmaApiCyberCurrent.codigo_professor) &&
                        (turmaApiCyberCurrent.id_unidade != null && turmaApiCyberCurrent.id_unidade > 0) &&
                        !existeGrupoByCodigoGrupo(turmaApiCyberCurrent.codigo))
                    {
                        turmaBusiness.sp_verificar_grupo_cyber(turmaApiCyberCurrent.nome_grupo, turmaApiCyberCurrent.id_unidade);

                        cadastraGruposApiCyber(turmaApiCyberCurrent, ApiCyberComandosNames.CADASTRA_GRUPO);
                    }
                }


                transaction.Complete();
            }
            if(programacoesNovas.Count() > 0)
            {
                turmaBusiness.postRefazerProgramacao(turma.cd_turma);
            }
            return turmaBusiness.getTurmaByCodForGrid(turma.cd_turma, turma.cd_pessoa_escola,
                turma.cd_turma_ppt > 0 ? true : false);
        }

        public void verificaTipoFuncionarioPostApiCyber(FuncionarioCyberBdUI funcionarioCyberBd)
        {
            string parametros = "";

            if (funcionarioCyberBd.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.PROFESSOR && !existeFuncionario(("P" + funcionarioCyberBd.codigo), ApiCyberComandosNames.VISUALIZA_PROFESSOR))
            {
                parametros = validaParametrosCadastraProfessor(funcionarioCyberBd, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.CADASTRA_PROFESSOR, "");

                executaCyberCadastraFuncionario(parametros, ApiCyberComandosNames.CADASTRA_PROFESSOR);
            }

            if (funcionarioCyberBd.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COORDENADOR)
            {
                if(!existeFuncionario(("P" + funcionarioCyberBd.codigo), ApiCyberComandosNames.VISUALIZA_PROFESSOR)) {
                    parametros = validaParametrosCadastraProfessor(funcionarioCyberBd, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.CADASTRA_PROFESSOR, "");

                    executaCyberCadastraFuncionario(parametros, ApiCyberComandosNames.CADASTRA_PROFESSOR);
                }

                if (!existeFuncionario(("O" + funcionarioCyberBd.codigo), ApiCyberComandosNames.VISUALIZA_COORDENADOR))
                {
                    parametros = validaParametrosCadastraCoordenador(funcionarioCyberBd, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.CADASTRA_COORDENADOR, "");

                    executaCyberCadastraFuncionario(parametros, ApiCyberComandosNames.CADASTRA_COORDENADOR);
                }
            }

        }

        private bool existeFuncionario(string codigo, string comando)
        {
            return businessApiNewCyber.verificaRegistroFuncionario(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo);
        }

        private void executaCyberCadastraFuncionario(string parametros, string comando)
        {
            string result = businessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);
        }

        private string validaParametrosCadastraProfessor(FuncionarioCyberBdUI entity, string url, string comando, string parametros)
        {


            //valida codigo funcionario
            if (entity == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }

            if (entity.codigo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCdFuncionarioMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_PROFESSOR_MENOR_IGUAL_ZERO, false);
            }


            //Valida id_unidade
            if (entity.id_unidade <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNmIntegracaoNuloOuMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NM_INTEGRACAO_NULO_OU_MENOR_IGUAL_ZERO, false);
            }

            //Valida nome e email

            if (String.IsNullOrEmpty(entity.nome))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNomeFuncionarioNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NOME_FUNCIONARIO_NULO_VAZIO, false);
            }

            if (String.IsNullOrEmpty(entity.email))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberEmailFuncionarioNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_EMAIL_PESSOA_FISICA_NULA_OU_VAZIA, false);
            }


            string listaParams = "";
            listaParams = string.Format("nome={0},id_unidade={1},codigo={2},email={3}", entity.nome, entity.id_unidade, entity.codigo, entity.email);
            return listaParams;
        }

        private string validaParametrosCadastraCoordenador(FuncionarioCyberBdUI entity, string url, string comando, string parametros)
        {


            //valida codigo funcionario
            if (entity == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }

            if (entity.codigo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCdFuncionarioMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_COD_FUNCIONARIO_MENOR_IGUAL_ZERO, false);
            }


            //Valida id_unidade
            if (entity.id_unidade <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNmIntegracaoNuloOuMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NM_INTEGRACAO_NULO_OU_MENOR_IGUAL_ZERO, false);
            }

            //Valida nome e email

            if (String.IsNullOrEmpty(entity.nome))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNomeFuncionarioNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NOME_FUNCIONARIO_NULO_VAZIO, false);
            }

            if (String.IsNullOrEmpty(entity.email))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberEmailFuncionarioNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_EMAIL_PESSOA_FISICA_NULA_OU_VAZIA, false);
            }


            string listaParams = "";
            listaParams = string.Format("nome={0},id_unidade={1},codigo={2},email={3}", entity.nome, entity.id_unidade, entity.codigo, entity.email);
            return listaParams;
        }

        public TurmaSearch editTurma(Turma turma)
        {
            try
            {
                bool id_programacao = false;
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                    TransactionScopeBuilder.TransactionType.READ_COMMITED, daoProduto.DB(),
                    TransactionScopeBuilder.TransactionTime.MODEREATE))
                {
                    ICollection<ProgramacaoTurma> programacaoTurma = turma.ProgramacaoTurma;
                    ICollection<FeriadoDesconsiderado> feriadosDesconsiderados = turma.FeriadosDesconsiderados;
                    Turma turmaContext = new Turma();


                    Turma turmaBdOld = null;
                    TurmaApiCyberBdUI turmaApiCyberOld = new TurmaApiCyberBdUI();
                    List<int> listIdsProfessorTurmaBdOld = new List<int>();
                    List<ProfessorTurma> listProfessorTurmaBdOld = new List<ProfessorTurma>();
                    if (businessApiNewCyber.aplicaApiCyber())
                    {
                        //Turma do banco
                        turmaBdOld = turmaBusiness.findTurmaByCdTurmaApiCyber(turma.cd_turma);

                        //Pega os professores ativos da turma no banco(Old)
                        listProfessorTurmaBdOld = turmaBusiness.findProfessorTurmaByCdTurma(turma.cd_turma, turma.cd_pessoa_escola);
                        

                        //monta a lista com professores turma do banco (old)
                        turmaApiCyberOld = turmaBusiness.findTurmaByCdTurmaAndCdEscolaApiCyber(turma.cd_turma, turma.cd_pessoa_escola);

                        if (turmaApiCyberOld != null && listProfessorTurmaBdOld != null && listProfessorTurmaBdOld.Count > 0)
                        {
                            //converte a lista de ids em uma string com delimitador (",")
                            turmaApiCyberOld.codigo_professor = string.Join("|", listProfessorTurmaBdOld.Select(x => x.cd_professor.ToString()).ToArray());
                            listIdsProfessorTurmaBdOld = listProfessorTurmaBdOld.Select(x => x.cd_professor).ToList();
                        }

                    }

                    //Sincroniza contextos:
                    this.sincronizarContextos(daoProduto.DB());
                    turmaContext = turmaBusiness.findTurmasByIdAndCdEscola(turma.cd_turma, turma.cd_pessoa_escola);
                    bool outraescola = turma.cd_pessoa_escola != turmaContext.cd_pessoa_escola;
                    if (turmaContext.dt_termino_turma.HasValue)
                        throw new TurmaBusinessException(Messages.msgErrorTurmaEncerrado, null,
                            TurmaBusinessException.TipoErro.ERRO_TURMA_ENCERRADA, false);
                    //Verificando alteração de horario
                    bool horarioDiferente;
                    List<Horario> horarioContext = turmaBusiness
                        .getHorarioByEscolaForRegistro(turmaContext.cd_pessoa_escola, turma.cd_turma, Horario.Origem.TURMA)
                        .OrderBy(p => p.id_dia_semana).ToList();
                    Horario primeiroHorarioContext = horarioContext.OrderBy(d => d.id_dia_semana).OrderBy(d => d.dt_hora_ini).FirstOrDefault();

                    ////Horario horarioAlterado = new Horario();
                    horarioDiferente =
                        (turma.horariosTurma != null && primeiroHorarioContext == null) ||
                        (turma.horariosTurma == null && primeiroHorarioContext != null)
                            ? true
                            : false;
                    if (turma.horariosTurma != null && turma.horariosTurma.Count() > 0 && primeiroHorarioContext != null)
                    {
                        byte[] idDiasContext = horarioContext.OrderBy(d => d.id_dia_semana).Select(d => d.id_dia_semana)
                            .ToArray();
                        byte[] idDias = turma.horariosTurma.OrderBy(d => d.id_dia_semana).Select(d => d.id_dia_semana)
                            .ToArray();

                        horarioDiferente = idDias.Length != idDiasContext.Length ? true : horarioDiferente;
                        if (idDias.Length == idDiasContext.Length)
                            for (int i = 0; i < idDiasContext.Length; i++)
                                if (!idDiasContext.Contains(idDias[i]))
                                {
                                    horarioDiferente = true;
                                    break;
                                }

                        Horario primeiroHorario = turma.horariosTurma.OrderBy(d => d.id_dia_semana).OrderBy(d => d.dt_hora_ini).FirstOrDefault();
                        horarioDiferente = (primeiroHorarioContext.id_dia_semana != primeiroHorario.id_dia_semana ||
                                            primeiroHorarioContext.dt_hora_ini != primeiroHorario.dt_hora_ini ||
                                            primeiroHorario.dt_hora_fim != primeiroHorarioContext.dt_hora_fim)
                            ? true
                            : horarioDiferente;
                    }
                    if(!turma.cd_turma_ppt.HasValue && outraescola && horarioDiferente) 
                        throw new TurmaBusinessException(Messages.msgErroAlterarHorario, null,
                            TurmaBusinessException.TipoErro.ERRO_ALTERAR_HORARIO, false);

                    //Verifica se existe algum horário da turma que interceda com o horário da sala:
                    if (!turma.cd_turma_ppt.HasValue && turma.cd_sala != null && turma.cd_sala_online == null &&
                        !getSalasDisponiveisPorHorariosEscrita(turma, turmaContext.cd_pessoa_escola).ToList()
                            .Where(t => t.cd_sala == turma.cd_sala).Any())
                        throw new TurmaBusinessException(Messages.msgErroHorariosDisponiveisTurmaSala, null,
                            TurmaBusinessException.TipoErro.ERRO_HORARIOS_DISPONIVEIS_TURMA_SALA, false);

                    //Verifica se existe algum horário da turma que interceda com o horário da sala:
                    if (!turma.cd_turma_ppt.HasValue && turma.cd_sala_online != null &&
                        !getSalasDisponiveisPorHorariosEscrita(turma, turmaContext.cd_pessoa_escola).ToList()
                            .Where(t => t.cd_sala == turma.cd_sala_online).Any())
                        throw new TurmaBusinessException(Messages.msgErroHorariosDisponiveisTurmaSala, null,
                            TurmaBusinessException.TipoErro.ERRO_HORARIOS_DISPONIVEIS_TURMA_SALA, false);

                    if (turmaContext.dt_inicio_aula != turma.dt_inicio_aula ||
                        turmaContext.cd_produto != turma.cd_produto || turmaContext.cd_curso != turma.cd_curso ||
                        horarioDiferente == true)
                    {
                        List<Horario> listaHor = turma.horariosTurma != null
                            ? turma.horariosTurma.ToList()
                            : new List<Horario>();
                        var nomeTurma = criarNomeTurma(turma.id_turma_ppt, turma.cd_turma_ppt, turma.cd_regime,
                            turma.cd_produto, turma.cd_curso, listaHor, turma.dt_inicio_aula);
                        int nrProximaTurma =
                            turmaBusiness.verificaExisteTurma(nomeTurma, turma.cd_pessoa_escola, turma.cd_turma);
                        nomeTurma = nomeTurma + "-" + nrProximaTurma;
                        turma.no_turma = nomeTurma;
                        turma.nm_turma = nrProximaTurma;
                        //  daoTurma.saveChanges(false);
                    }

                    // Atualiza a data final com a data da programação da turma:
                    List<ProgramacaoTurma> programacoesTurma = new List<ProgramacaoTurma>();
                    if (programacaoTurma != null)
                    {
                        programacoesTurma = programacaoTurma.ToList();
                        if (programacoesTurma.Count >= 1)
                            turma.dt_final_aula = programacoesTurma[programacoesTurma.Count - 1].dta_programacao_turma;
                        else
                            turma.dt_final_aula = null;
                    }

                    if (turma.dt_final_aula != null)
                        turma.dt_final_aula = ((DateTime)turma.dt_final_aula).Date;
                    if (programacaoTurma != null)
                        id_programacao = turmaBusiness.crudProgramacaoTurma(programacoesTurma, turma.cd_turma, turmaContext.cd_pessoa_escola);
                    turma.nro_aulas_programadas =
                        (Byte)turmaBusiness.getQuantidadeAulasProgramadasTurma(turmaContext.cd_turma,
                            turmaContext.cd_pessoa_escola);
                    turmaBusiness.editTurma(turma, turmaContext);
                    var alterarNomePPTFilha = false;
                    if (turmaContext.cd_turma_ppt == null || turmaContext.cd_turma_ppt <= 0)
                    {
                        if (turma.horariosTurma != null)
                        {
                            if (turma.alunosTurmasPPT != null && turma.id_turma_ativa)
                                turmaBusiness.verifTurmasFilhasDisponiveisHorariosTurmaPPT(turma.horariosTurma.ToList(),
                                    turma.alunosTurmasPPT.ToList());
                            //turmaBusiness.verifTurmasFilhasDisponiveisHorariosTurmaPPTBD(turmaContext.cd_turma, turmaContext.cd_pessoa_escola, turma.horariosTurma.ToList());
                            //if (!outraescola)
                            turmaBusiness.crudHorariosTurma(turma.horariosTurma.ToList(), turma.cd_turma,
                                turmaContext.cd_pessoa_escola, Turma.TipoTurma.PPT);
                        }
                        else
                            turmaBusiness.crudHorariosTurma(new List<Horario>(), turma.cd_turma, turmaContext.cd_pessoa_escola,
                                Turma.TipoTurma.PPT);

                        if (!turma.id_turma_ppt && turma.alunosTurma != null)
                            turmaBusiness.crudAlunosTurma(turma.alunosTurma.ToList(), turma);
                        if (turma.id_turma_ppt && turma.alunosTurmasPPT != null)
                        {
                            Turma turmaPai = turmaBusiness.getTurmaEHorarios(turma.cd_turma, turmaContext.cd_pessoa_escola);
                            //turmaBusiness.verifTurmasFilhasDisponiveisHorariosTurmaPPT(turma.horariosTurma.ToList(), turma.alunosTurmasPPT.ToList());
                            if (turmaContext.dt_inicio_aula != turma.dt_inicio_aula ||
                                turmaContext.cd_regime != turma.cd_regime || horarioDiferente == true)
                                alterarNomePPTFilha = true;
                            List<TurmaEscola> turmalist = turmaBusiness.crudAlunosTurmasPPT(
                                criarNomeTurmaFilhas(turma.alunosTurmasPPT.ToList(), turmaPai, alterarNomePPTFilha, turma.cd_pessoa_escola),
                                turmaPai, alterarNomePPTFilha, turma.cd_pessoa_escola, horarioDiferente);
                            if (turmalist.Count() > 0)
                                daoTurmaEscola.addRange(turmalist, false);
                        }
                    }
                    else
                    {
                        if (turma.horariosTurma != null) //&& !outraescola)
                            turmaBusiness.crudHorariosTurma(turma.horariosTurma.ToList(), turma.cd_turma,
                                turmaContext.cd_pessoa_escola, Turma.TipoTurma.NORMAL);
                        else
                            turmaBusiness.crudHorariosTurma(new List<Horario>(), turma.cd_turma, turmaContext.cd_pessoa_escola,
                                Turma.TipoTurma.NORMAL);
                    }

                    // query em produtos com cd_produto = turma.cd_produto
                    var produto = daoProduto.findById(turma.cd_produto, false);
                    var limite = 100;
                    if (produto.no_produto == "Empreendedorismo") limite = 60;

                    var minutosSemana = (from t in turma.horariosTurma select (t.dt_hora_fim - t.dt_hora_ini).TotalMinutes).Sum();
                    if (minutosSemana < 100 && horarioDiferente)
                        throw new TurmaBusinessException(string.Format(Utils.Messages.Messages.msgErroCargaHorariaMinima, limite), null,
                            TurmaBusinessException.TipoErro.ERRO_CARGA_HORARIA_MINIMA, false);
                    if (turma.horariosTurma != null && horarioDiferente)
                    {
                        string ret = turmaBusiness.existeProgInsuficiente(turma);
                        if (ret != null && ret != "OK")
                        {
                            throw new TurmaBusinessException(ret, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_PROGRAMACAO_INSUFICIENTE, false);
                        }

                    }


                    //if (daoSala.verificaSalaOnline(turma.cd_sala, turma.cd_pessoa_escola))
                    if (turmaContext.cd_sala_online != null && turma.TurmaEscola != null && turma.TurmaEscola.Count >= 0 && !outraescola)
                    {
                        List<TurmaEscola> turmaEscolasContext = daoTurmaEscola.getTurmasEscolatByTurma(turma.cd_turma).ToList();
                        List<TurmaEscola> turmaEscolasView = turma.TurmaEscola.ToList();
                        turma.TurmaEscola = null;
                        crudTurmaEscola(turmaEscolasContext, turmaEscolasView);
                    }

                    if (turma.ProfessorTurma != null) //&& !outraescola)
                        turmaBusiness.crudProfessoresTurma(turma.ProfessorTurma.ToList(), turmaContext, turma.horariosTurma,
                            false);

                    if (feriadosDesconsiderados != null) //&& !outraescola)
                        turmaBusiness.crudFeriadosDesconsiderados(feriadosDesconsiderados.ToList(), turma.cd_turma,
                            turma.cd_pessoa_escola);

                    if (businessApiNewCyber.aplicaApiCyber())
                    {
                        //Turma do banco
                        Turma turmaBdCurrent = turmaBusiness.findTurmaByCdTurmaApiCyber(turma.cd_turma);

                        //Pega os professores ativos da turma no banco(Current)
                        List<ProfessorTurma> listProfessorTurmaBdCurrent = turmaBusiness.findProfessorTurmaByCdTurma(turma.cd_turma, turma.cd_pessoa_escola);
                        List<int> listIdsProfessorTurmaBdCurrent = new List<int>();

                        //monta a lista com professores turma do banco (Current)
                        TurmaApiCyberBdUI turmaApiCyberCurrent = new TurmaApiCyberBdUI();
                        turmaApiCyberCurrent = turmaBusiness.findTurmaApiCyber(turma.cd_turma, turma.cd_pessoa_escola);


                        //Percorre a lista de professores e verifica se já existem no cyber,
                        //se não, cadastra o professor no cyber
                        foreach (ProfessorTurma professorTurma in listProfessorTurmaBdCurrent)
                        {
                            FuncionarioCyberBdUI funcionarioCyberBdCurrent = turmaBusiness.findFuncionarioByCdFuncionario(professorTurma.cd_professor, turma.cd_pessoa_escola);

                            if (funcionarioCyberBdCurrent != null && funcionarioCyberBdCurrent.id_unidade != null && funcionarioCyberBdCurrent.id_unidade > 0 &&
                                funcionarioCyberBdCurrent.funcionario_ativo == true)
                            {
                                //Chama a apiCyber de acordo com o tipo de funcionario
                                verificaTipoFuncionarioPostApiCyber(funcionarioCyberBdCurrent);
                            }
                        }



                        //se a lista de professores ativos for > 0, adiciona os ids no campo codigo_professor
                        if (turmaApiCyberCurrent != null && listProfessorTurmaBdCurrent != null && listProfessorTurmaBdCurrent.Count > 0)
                        {
                            //converte a lista de ids em uma string com delimitador (",")
                            turmaApiCyberCurrent.codigo_professor = string.Join("|", listProfessorTurmaBdCurrent.Select(x => x.cd_professor.ToString()).ToArray());
                            listIdsProfessorTurmaBdCurrent = listProfessorTurmaBdCurrent.Select(x => x.cd_professor).ToList();
                        }
                        //TODO Adicionar condição -> caso não tinha nm_cliente_integração e agora tem

                        //se a tuma tem o id_integracao e já exite no  banco e no cyber
                        if ((turmaApiCyberOld != null) &&
                            turmaApiCyberCurrent != null &&
                            (turmaApiCyberCurrent.id_unidade != null && turmaApiCyberCurrent.id_unidade > 0) &&
                            existeGrupoByCodigoGrupo(turmaApiCyberCurrent.codigo))
                        {
                            //turma ppt pai e ativou turma
                            if ((turmaApiCyberCurrent.cd_turma_ppt == null && turmaApiCyberCurrent.id_turma_ppt == true) &&
                                turmaApiCyberOld.id_turma_ativa != turmaApiCyberCurrent.id_turma_ativa && turmaApiCyberOld.id_turma_ativa == false)
                            {
                                AtivaGruposApiCyber(turmaApiCyberCurrent, ApiCyberComandosNames.ATIVA_GRUPO);
                            }

                            //turma ppt pai e inativou turma
                            if ((turmaApiCyberCurrent.cd_turma_ppt == null && turmaApiCyberCurrent.id_turma_ppt == true) &&
                                turmaApiCyberOld.id_turma_ativa != turmaApiCyberCurrent.id_turma_ativa && turmaApiCyberOld.id_turma_ativa == true)
                            {
                                InativaGruposApiCyber(turmaApiCyberCurrent, ApiCyberComandosNames.INATIVA_GRUPO);
                            }

                            //Chama o cadastraLivroAluno(para vincular a turma e aluno - Reunião Fisk 12/01/2023)
                            //turma ppt pai e ativou turma
                            if ((turmaApiCyberCurrent.cd_turma_ppt == null && turmaApiCyberCurrent.id_turma_ppt == true))
                            {
                                List<LivroAlunoApiCyberBdUI> alunosAtivosCadastraLivroAluno = turmaBusiness.findAlunoTurmaAtivosByCdTurmaPPTPai(turma.cd_turma);
                                if (alunosAtivosCadastraLivroAluno != null && alunosAtivosCadastraLivroAluno.Count > 0)
                                {
                                    foreach (LivroAlunoApiCyberBdUI alunoAtivoUpdate in alunosAtivosCadastraLivroAluno)
                                    {
                                        if (alunoAtivoUpdate != null && alunoAtivoUpdate.codigo_unidade != null && alunoAtivoUpdate.codigo_livro > 0 &&
                                            existeAluno(alunoAtivoUpdate.codigo_aluno) &&
                                            existeGrupoByCodigoGrupo(alunoAtivoUpdate.codigo_grupo) &&
                                            !existeLivroAlunoByCodAluno(alunoAtivoUpdate.codigo_aluno, alunoAtivoUpdate.codigo_grupo, alunoAtivoUpdate.codigo_livro))
                                        {
                                            cadastraLivroAlunoApiCyber(alunoAtivoUpdate, ApiCyberComandosNames.CADASTRA_LIVROALUNO);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                List<LivroAlunoApiCyberBdUI> alunosAtivosCadastraLivroAluno = turmaBusiness.findAlunoTurmaAtivosByCdTurma(turma.cd_turma);
                                if (alunosAtivosCadastraLivroAluno != null && alunosAtivosCadastraLivroAluno.Count > 0)
                                {
                                    foreach (LivroAlunoApiCyberBdUI alunoAtivoUpdate in alunosAtivosCadastraLivroAluno)
                                    {
                                        if (alunoAtivoUpdate != null && alunoAtivoUpdate.codigo_unidade != null && alunoAtivoUpdate.codigo_livro > 0 &&
                                            existeAluno(alunoAtivoUpdate.codigo_aluno) &&
                                            existeGrupoByCodigoGrupo(alunoAtivoUpdate.codigo_grupo) &&
                                            !existeLivroAlunoByCodAluno(alunoAtivoUpdate.codigo_aluno, alunoAtivoUpdate.codigo_grupo, alunoAtivoUpdate.codigo_livro))
                                        {
                                            cadastraLivroAlunoApiCyber(alunoAtivoUpdate, ApiCyberComandosNames.CADASTRA_LIVROALUNO);
                                        }
                                    }
                                }
                            }

                            


                            //se for ppt filha
                            if (turmaBdCurrent.cd_turma_ppt != null && turmaBdCurrent.cd_turma_ppt > 0 && turmaBdCurrent.id_turma_ppt == false)
                            {

                                if ((listIdsProfessorTurmaBdCurrent != null && listIdsProfessorTurmaBdCurrent.Count > 0) &&
                                    (listIdsProfessorTurmaBdOld != null && listIdsProfessorTurmaBdOld.Count > 0))
                                {

                                    IEnumerable<int> professoresAtivosDeletados = listIdsProfessorTurmaBdOld.Except(listIdsProfessorTurmaBdCurrent);
                                    IEnumerable<int> professoresAtivosAdd = listIdsProfessorTurmaBdCurrent.Except(listIdsProfessorTurmaBdOld);

                                    //se alterou o nome da turma pai ou alterou a lista de professores ativos
                                    if (turmaBdOld.TurmaPai.no_turma != turmaBdCurrent.TurmaPai.no_turma || (professoresAtivosDeletados.Count() > 0 || professoresAtivosAdd.Count() > 0))
                                    {
                                        //atualiza o grupo
                                        atualizaGruposApiCyber(turmaApiCyberCurrent, ApiCyberComandosNames.ATUALIZA_GRUPO);
                                    }
                                }

                            }
                            else
                            {
                                //Se não for ppt filha
                                if ((listIdsProfessorTurmaBdCurrent != null && listIdsProfessorTurmaBdCurrent.Count > 0) &&
                                    (listIdsProfessorTurmaBdOld != null && listIdsProfessorTurmaBdOld.Count > 0))
                                {

                                    IEnumerable<int> professoresAtivosDeletados = listIdsProfessorTurmaBdOld.Except(listIdsProfessorTurmaBdCurrent);
                                    IEnumerable<int> professoresAtivosAdd = listIdsProfessorTurmaBdCurrent.Except(listIdsProfessorTurmaBdOld);

                                    //se alterou o nome da turma ou alterou a lista de professores ativos
                                    if (turmaBdOld.no_turma != turmaBdCurrent.no_turma || (professoresAtivosDeletados.Count() > 0 || professoresAtivosAdd.Count() > 0))
                                    {
                                        //atualiza o grupo
                                        atualizaGruposApiCyber(turmaApiCyberCurrent, ApiCyberComandosNames.ATUALIZA_GRUPO);
                                    }
                                }
                            }
                        }
                        //se a tuma tem o id_integracao e já exite no banco e não existe no cyber 
                        else if ((turmaApiCyberOld != null) &&
                                 turmaApiCyberCurrent != null &&
                                 (turmaApiCyberCurrent.id_unidade != null && turmaApiCyberCurrent.id_unidade > 0) &&
                                 !existeGrupoByCodigoGrupo(turmaApiCyberCurrent.codigo))
                        {

                            turmaBusiness.sp_verificar_grupo_cyber(turmaApiCyberCurrent.nome_grupo, turmaApiCyberCurrent.id_unidade);

                            //Se tiver professores ativos
                            if (!string.IsNullOrEmpty(turmaApiCyberCurrent.codigo_professor))
                            {
                                cadastraGruposApiCyber(turmaApiCyberCurrent, ApiCyberComandosNames.CADASTRA_GRUPO);
                            }


                            //Chama o atualiza updateLivroAluno(para vincular a turma e aluno - Reunião Fisk 12/01/2023)
                            List<LivroAlunoApiCyberBdUI> alunosAtivosCadastraLivroAluno = turmaBusiness.findAlunoTurmaAtivosByCdTurma(turma.cd_turma);
                            if (alunosAtivosCadastraLivroAluno != null && alunosAtivosCadastraLivroAluno.Count > 0)
                            {
                                foreach (LivroAlunoApiCyberBdUI alunoAtivoUpdate in alunosAtivosCadastraLivroAluno)
                                {
                                    if (alunoAtivoUpdate != null && alunoAtivoUpdate.codigo_unidade != null && alunoAtivoUpdate.codigo_livro > 0 &&
                                        existeAluno(alunoAtivoUpdate.codigo_aluno) &&
                                        existeGrupoByCodigoGrupo(alunoAtivoUpdate.codigo_grupo) &&
                                        !existeLivroAlunoByCodAluno(alunoAtivoUpdate.codigo_aluno, alunoAtivoUpdate.codigo_grupo, alunoAtivoUpdate.codigo_livro))
                                    {
                                        cadastraLivroAlunoApiCyber(alunoAtivoUpdate, ApiCyberComandosNames.CADASTRA_LIVROALUNO);
                                    }
                                }
                            }

                        }

                        //se a tuma tem o id_integracao e não exite no banco e não existe no cyber 
                        else if ((turmaApiCyberOld == null) &&
                                 turmaApiCyberCurrent != null &&
                                 (turmaApiCyberCurrent.id_unidade != null && turmaApiCyberCurrent.id_unidade > 0) &&
                                 !existeGrupoByCodigoGrupo(turmaApiCyberCurrent.codigo))
                        {

                            turmaBusiness.sp_verificar_grupo_cyber(turmaApiCyberCurrent.nome_grupo, turmaApiCyberCurrent.id_unidade);

                            //Se tiver professores ativos
                            if (!string.IsNullOrEmpty(turmaApiCyberCurrent.codigo_professor))
                            {
                                cadastraGruposApiCyber(turmaApiCyberCurrent, ApiCyberComandosNames.CADASTRA_GRUPO);
                            }
                        }

                        
                    }

                    transaction.Complete();
                }
                //Fazer fora da transação
                if (id_programacao) turmaBusiness.postRefazerProgramacao(turma.cd_turma);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    (ex.InnerException.InnerException.Message.Contains("gatilho") ||
                    ex.InnerException.InnerException.Message.Contains("trigger")))
                {
                    throw new TurmaBusinessException(ex.InnerException.InnerException.Message.Replace("A transação foi encerrada no gatilho. O lote foi anulado.", "."),
                        null, TurmaBusinessException.TipoErro.ERRO_TRIGGER_ALTERACAO_TURMA, false);
                }
                throw ex;
            }

            return turmaBusiness.getTurmaByCodForGrid(turma.cd_turma, turma.cd_pessoa_escola,
                turma.cd_turma_ppt > 0 ? true : false);
        }


        private void cadastraGruposApiCyber(TurmaApiCyberBdUI turmaApiCyberCurrent, string comando)
        {
            
            string parametros = "";
          
            parametros = validaParametrosCyberCadastro(turmaApiCyberCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");
            
            string result = businessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);
            
        }

        public bool existeGrupoByCodigoGrupo(int codigo_grupo)
        {
            return businessApiNewCyber.verificaRegistroGrupos(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.VISUALIZA_GRUPO, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo_grupo);
        }

        private void atualizaGruposApiCyber(TurmaApiCyberBdUI turmaApiCyberCurrent, string comando)
        {
            string parametros = "";

            parametros = validaParametrosCyberAtualiza(turmaApiCyberCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");

            string result = businessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);

        }

        private void InativaGruposApiCyber(TurmaApiCyberBdUI turmaApiCyberCurrent, string comando)
        {
                string parametros = "";
                
                parametros = validaParametrosCyberAtivaInativa(turmaApiCyberCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");


                string result = businessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                    comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);

            
        }

        private void AtivaGruposApiCyber(TurmaApiCyberBdUI turmaApiCyberCurrent, string comando)
        {
            string parametros = "";

            parametros = validaParametrosCyberAtivaInativa(turmaApiCyberCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");

            string result = businessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);

        }


        private string validaParametrosCyberCadastro(TurmaApiCyberBdUI entity, string url, string comando, string parametros)
        {

           
            if (entity == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }

            //valida codigo do grupo
            if (entity.codigo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCodigoGrupoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_GRUPO_MENOR_IGUAL_ZERO, false);
            }
            //valida codigo do professor
            //if (entity.codigo_professor <= 0)
            //{
            //    throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCodigoProfessorMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_PROFESSOR_MENOR_IGUAL_ZERO, false);

            //}
            //valida id_unidade
            if (entity.id_unidade == null || entity.id_unidade <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNmIntegracaoNuloOuMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NM_INTEGRACAO_NULO_OU_MENOR_IGUAL_ZERO, false);
            }
            //valida nome do grupo
            if (String.IsNullOrEmpty(entity.nome_grupo))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNomeGrupoNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NOME_GRUPO_NULO_VAZIO, false);
            }


            string listaParams = "";
            listaParams = string.Format("nome_grupo={0},codigo_professor={1},id_unidade={2},codigo={3}", entity.nome_grupo, entity.codigo_professor, entity.id_unidade, entity.codigo);
            return listaParams;
        }

        private void updateLivroAlunoApiCyber(LivroAlunoUpdateApiCyberBdUI entity, string comando)
        {

            string parametros = "";

            parametros = validaParametrosCyberUpdateLivroAluno(entity, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");

            string result = businessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);

        }

        private string validaParametrosCyberUpdateLivroAluno(LivroAlunoUpdateApiCyberBdUI entity, string url, string comando, string parametros)
        {


            if (entity == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }

            //valida id_unidade
            if (entity.codigo_aluno <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCdAlunoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_COD_ALUNO_MENOR_IGUAL_ZERO, false);
            }

            //valida codigo do grupo
            if (entity.codigo_grupo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCdGrupoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_GRUPO_MENOR_IGUAL_ZERO, false);
            }

            //valida codigo antigo
            if (entity.codigo_antigo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCdGrupoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_GRUPO_MENOR_IGUAL_ZERO, false);
            }


            string listaParams = "";
            listaParams = string.Format("codigo_aluno={0},codigo_grupo={1},codigo_antigo={2}", entity.codigo_aluno, entity.codigo_grupo, entity.codigo_antigo);
            return listaParams;
        }

        private string validaParametrosCyberAtualiza(TurmaApiCyberBdUI entity, string url, string comando, string parametros)
        {


            if (entity == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }

            //valida codigo do grupo
            if (entity.codigo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCodigoGrupoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_GRUPO_MENOR_IGUAL_ZERO, false);
            }
            //valida codigo do professor
            //if (entity.codigo_professor <= 0)
            //{
            //    throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCodigoProfessorMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_PROFESSOR_MENOR_IGUAL_ZERO, false);

            //}
            
            //valida nome do grupo
            if (String.IsNullOrEmpty(entity.nome_grupo))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNomeGrupoNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NOME_GRUPO_NULO_VAZIO, false);
            }


            string listaParams = "";
            listaParams = string.Format("nome_grupo={0},codigo_professor={1},codigo={2}", entity.nome_grupo, entity.codigo_professor, entity.codigo);
            return listaParams;
        }

        private string validaParametrosCyberAtivaInativa(TurmaApiCyberBdUI entity, string url, string comando, string parametros)
        {

            if (entity == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }

            //valida codigo do grupo
            if (entity.codigo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCodigoGrupoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_GRUPO_MENOR_IGUAL_ZERO, false);
            }


            string listaParams = "";
            listaParams = string.Format("codigo={0}", entity.codigo);
            return listaParams;
        }



        private void crudTurmaEscola(List<TurmaEscola> turmaEscolasContext, List<TurmaEscola> turmaEscolasView)
        {

            List<int> cdsEscolasBase = turmaEscolasContext.Select(x => x.cd_escola).ToList();
            List<int> cdsEscolasView = turmaEscolasView.Select(x => x.cd_escola).ToList();

            IEnumerable<int> deletarTurmaEscolas = cdsEscolasBase.Except(cdsEscolasView);
            IEnumerable<int> addTurmaEscolas = cdsEscolasView.Except(cdsEscolasBase);

            if (addTurmaEscolas != null && addTurmaEscolas.Count() > 0)
            {
                foreach (int addTurmaEscola in addTurmaEscolas)
                {
                    TurmaEscola turmaEscola = turmaEscolasView.Where(x => x.cd_escola == addTurmaEscola).FirstOrDefault();
                    if (turmaEscola != null)
                    {
                        daoTurmaEscola.addContext(turmaEscola, false);
                    }
                }
                daoTurmaEscola.saveChanges(false);
            }


            //Deletar as turma escolas que estão no banco e não estão na view.
            if (deletarTurmaEscolas != null && deletarTurmaEscolas.Count() > 0)
            {
                foreach (int delTurma in deletarTurmaEscolas)
                {
                    TurmaEscola turmaEscola = turmaEscolasContext.Where(x => x.cd_escola == delTurma).FirstOrDefault();
                    if (turmaEscola != null)
                    {
                        TurmaEscola turmaEscolaDel = daoTurmaEscola.getTurmasEscolatByIdAndTurma(turmaEscola.cd_turma, turmaEscola.cd_escola, turmaEscola.cd_turma_escola).FirstOrDefault();
                        if (turmaEscolaDel != null)
                        {
                            daoTurmaEscola.deleteContext(turmaEscolaDel, false);
                        }
                    }

                }
                daoTurmaEscola.saveChanges(false);
            }
        }

        public TurmaSearch postNovaTurmaEnc(int cdTurma, int cdEscola, TurmaDataAccess.TipoConsultaTurmaEnum tipo)
        {
            var turmaEncerrada = turmaBusiness.buscarTurmaHorariosNovaVirada(cdTurma, cdEscola, tipo);
            if(turmaEncerrada == null)
            {
                throw new TurmaBusinessException(Messages.msgTurmaNotFound, null,
                    TurmaBusinessException.TipoErro.ERRO_TURMA_NAO_ENCONTRADA, false);
            }
            if (turmaEncerrada.cd_turma_ppt > 0 && turmaEncerrada.alunosTurma.Count() == 0)
                throw new TurmaBusinessException(Messages.msgErroViradaPPTFilhaAlunoNDisponivel, null,
                    TurmaBusinessException.TipoErro.ERRO_NAO_EXISTE_ALUNO_ATIVO_PPT_FILHO, false);
            if (turmaEncerrada.cd_curso.HasValue && turmaEncerrada.cd_curso > 0)
            {
                turmaEncerrada.cd_curso = cursoBiz.findProxCurso(turmaEncerrada.cd_curso.Value);
                if (!turmaEncerrada.cd_curso.HasValue)
                    throw new TurmaBusinessException(Messages.msgErroProxCurso, null,
                        TurmaBusinessException.TipoErro.ERRO_NAO_EXISTE_PROX_CURSO, false);
            }

            turmaEncerrada.cd_turma_enc = turmaEncerrada.cd_turma;
            turmaEncerrada.cd_turma = 0;
            turmaEncerrada.no_turma = null;
            if (turmaEncerrada.dt_termino_turma.HasValue)
                turmaEncerrada.dt_inicio_aula = turmaEncerrada.dt_termino_turma.Value.AddDays(1);
            turmaEncerrada.dt_final_aula = null;
            turmaEncerrada.dt_termino_turma = null;
            turmaEncerrada.FeriadosDesconsiderados = null;
            turmaEncerrada.ProgramacaoTurma = null;
            List<ProfessorTurma> listProfTurma = new List<ProfessorTurma>();
            List<AlunoTurma> listAlunoTurma = new List<AlunoTurma>();
            List<Horario> listHorario = new List<Horario>();
            List<TurmaEscola> listTurmaEscola = new List<TurmaEscola>();

            foreach (ProfessorTurma p in turmaEncerrada.ProfessorTurma)
            {
                if (p.id_professor_ativo)
                {
                    ProfessorTurma professor = new ProfessorTurma();
                    professor.cd_professor = p.cd_professor;
                    professor.id_professor_ativo = p.id_professor_ativo;
                    listProfTurma.Add(professor);
                }
            }

            foreach (AlunoTurma a in turmaEncerrada.alunosTurma)
            {
                if (a.cd_situacao_aluno_turma == (byte)SituacaoAlunoTurma.AGUARDANDO ||
                    a.cd_situacao_aluno_turma == (byte)SituacaoAlunoTurma.ATIVO ||
                    a.cd_situacao_aluno_turma == (byte)SituacaoAlunoTurma.REMATRICULADO ||
                    a.cd_situacao_aluno_turma == (byte)SituacaoAlunoTurma.ENCERRADO)
                {
                    AlunoTurma aluno = new AlunoTurma();
                    aluno.cd_aluno = a.cd_aluno;
                    aluno.cd_pessoa_aluno = a.cd_pessoa_aluno;
                    aluno.cd_situacao_aluno_origem = null;
                    aluno.cd_situacao_aluno_turma = (byte)SituacaoAlunoTurma.AGUARDANDO;
                    aluno.cd_turma_origem = null;
                    listAlunoTurma.Add(aluno);
                }
            }

            foreach (Horario a in turmaEncerrada.horariosTurma)
            {
                Horario horario = new Horario();
                horario.copy(a);
                horario.cd_horario = 0;
                horario.cd_registro = 0;
                horario.HorariosProfessores = null;
                foreach (HorarioProfessorTurma p in a.HorariosProfessores)
                {
                    if (horario.HorariosProfessores == null)
                        horario.HorariosProfessores = new List<HorarioProfessorTurma>();
                    horario.HorariosProfessores.Add(new HorarioProfessorTurma
                    {
                        cd_professor = p.cd_professor
                    });
                }

                listHorario.Add(horario);
            }
            
            foreach (TurmaEscola a in turmaEncerrada.TurmaEscola)
            {
                TurmaEscola tEscola = new TurmaEscola();
                tEscola.cd_turma = a.cd_turma;
                tEscola.cd_escola = a.cd_escola;
                listTurmaEscola.Add(tEscola);
            }
            
            turmaEncerrada.alunosTurma = listAlunoTurma;
            turmaEncerrada.ProfessorTurma = listProfTurma;
            turmaEncerrada.horariosTurma = listHorario;
            turmaEncerrada.TurmaEscola = listTurmaEscola;
            TurmaSearch novaTurma = addTurma(turmaEncerrada);
            return novaTurma;
        }

        #endregion

        #region Plano de contas

        /// <summary>
        /// Colocamos esse método aqui por causa da arquitetura pois a depêndecia vem da seguinte forma: financeiro -> coordenação -> escola
        /// </summary>
        /// <param name="cd_pessoa_escola"></param>
        /// <returns></returns>
        public IEnumerable<PlanoConta> getPlanoContaByIdEscola(int cd_pessoa_escola)
        {
            return financeiroBusiness.getPlanoContasSearch(cd_pessoa_escola);
        }

        #endregion

        #region Titulo

        public int? isNumber(string nmTitulo)
        {
            try
            {
                int teste = Convert.ToInt32(nmTitulo);
                return teste;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Titulo> alterarLocalMovtoTitulos(List<Titulo> titulosAlterarLocalMovto, int nm_parcelas_mensalidade)
        {
            if (titulosAlterarLocalMovto != null && titulosAlterarLocalMovto[0].dt_emissao_titulo != null)
            {
                DateTime dataVencimentoTituloCartao = titulosAlterarLocalMovto[0].dt_emissao_titulo;
                foreach (var titulo in titulosAlterarLocalMovto)
                {
                    // Se Tipo financeiro for Cartão Crédito ou Débito, aplicar taxa bancária;
                    if (titulo.cd_tipo_financeiro == (int)FundacaoFisk.SGF.GenericModel.TipoFinanceiro.TiposFinanceiro.CARTAO)
                    {
                        aplicarTaxaBancaria(titulo, nm_parcelas_mensalidade, ref dataVencimentoTituloCartao, null);
                    }
                }
            }
            return titulosAlterarLocalMovto;
        }

        private List<Titulo> criaTitulos(Titulo titulo, bool? diaUtil, byte? nmDiaParam, int idNew,
            bool id_alterar_venc_final_semana, bool calcularProximaData,
            int nm_parcela_fechadas, List<Contrato.TituloDescontoParcela> titulosDescontoParcela,
            decimal vl_liquido_total, byte? nm_parcelas_material, decimal? vl_parcela_material, 
            bool? id_incorporar_material, decimal? vl_material_contrato, List<Contrato.TituloTaxaParcela> titulosTaxaParcela)
        {
            int diaSugerido = titulo.diaSugerido;
            Decimal valorTituloSemRound = titulo.vl_titulo;
            nm_parcelas_material = (nm_parcelas_material == null ? 0 : nm_parcelas_material);
            vl_parcela_material = (vl_parcela_material == null ? 0 : vl_parcela_material);
            Decimal totalMaterial = (decimal)(vl_material_contrato == null ? 0 : vl_material_contrato);
            if (!(bool)id_incorporar_material) totalMaterial = 0;
            titulo.vl_titulo = Decimal.Round(titulo.vl_titulo, 2);
            titulo.dh_cadastro_titulo = DateTime.Now.ToUniversalTime();
            List<Titulo> titulos = new List<Titulo>();
            int mesProximoAno = 0;
            DateTime dtaVencTituloAnterior = titulo.dt_vcto_titulo;
            DateTime maxDate = new DateTime(2079, 06, 06);
            DateTime minDate = new DateTime(1900, 01, 01);
            IEnumerable<Feriado> feriadosEscola = null;
            int? sequenciaCheque = null;
            string stringCheque = "";
            if (titulo.dc_num_documento_titulo != null)
            {
                int legDoc = titulo.dc_num_documento_titulo.Length;
                for (int p = 0; p <= legDoc; p++)
                {
                    sequenciaCheque = isNumber(titulo.dc_num_documento_titulo.Substring(p, (legDoc - p)));
                    if (sequenciaCheque.HasValue && sequenciaCheque > 0)
                    {
                        stringCheque = titulo.dc_num_documento_titulo.Substring(0, p);
                        break;
                    }
                }
            }
            if (titulo.dc_tipo_titulo == "ME" || titulo.dc_tipo_titulo == "MA" || titulo.dc_tipo_titulo == "MM")
                if (nm_parcelas_material > titulo.nm_parcela_titulo)
                    throw new MatriculaBusinessException(Messages.msgErroQtdMensaMenorQtdMaterial, null,
                        MatriculaBusinessException.TipoErro.ERRO_MATERIAL_MENSALIDADES, false);

            if (titulo.percentualResp != 0)
            {
                if (titulo.percentualResp == 100)
                {
                    //Pessoa do Título é a pessoa responsável
                    int l = 0;
                    if (calcularProximaData)
                        l = nm_parcela_fechadas;
                    for (int i = l; i < titulo.nm_parcela_titulo; i++)
                    {
                        mesProximoAno = mesProximoAno >= 12 ? 0 : mesProximoAno;
                        Titulo newTitulo = new Titulo();
                        newTitulo.possuiBaixa = false;
                        newTitulo.copy(titulo);
                        newTitulo.descLocalMovto = titulo.descLocalMovto;
                        idNew++;
                        //titulo.id = idNew;
                        if (titulos.Count() <= 0 && !calcularProximaData)
                        {
                            //Alteração refente ao chamado 714 - Chamado: 265215 -> (Aditamento/retirar ToLocalTime()-> data voltando 1 dia): DateTime data_opcao = titulo.dt_vcto_titulo.ToLocalTime().Date; 
                            DateTime data_opcao = titulo.dt_vcto_titulo.Date;

                            if (id_alterar_venc_final_semana)
                                pulaFeriadoEFinalSemana(ref data_opcao, titulo.cd_pessoa_empresa, ref feriadosEscola);
                            if (data_opcao <= minDate)
                                throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior, null,
                                    CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR, false);
                            if (data_opcao >= maxDate)
                                throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                                    CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR, false);

                            if (((newTitulo.dt_vcto_titulo.Month > data_opcao.Month) ||
                                 (data_opcao.Month > newTitulo.dt_vcto_titulo.Month)) && id_alterar_venc_final_semana)
                            {
                                ////Alteração refente ao chamado 714 - Chamado: 265215 -> (Aditamento/retirar ToLocalTime()-> data voltando 1 dia): data_opcao = titulo.dt_vcto_titulo.ToLocalTime().Date;
                                data_opcao = titulo.dt_vcto_titulo.Date;
                                pulaFeriadoEFinalSemana(ref data_opcao, titulo.cd_pessoa_empresa, ref feriadosEscola,
                                    false);
                            }


                            DateTime dataOpcao = data_opcao;

                            int nmDia = dataOpcao.Day > 0 ? dataOpcao.Day : (int)nmDiaParam;

                            // Calcular data de vencimento dos titulos, quando o parâmetro 'Considerar como dia útil' estiver marcado.
                            this.CalcularDiaVencimentoTit(nmDia, data_opcao.Year, data_opcao.Month, ref dataOpcao,
                                newTitulo, titulo, feriadosEscola, diaUtil);

                            newTitulo.dt_vcto_titulo = dataOpcao;

                            if (newTitulo.dt_emissao_titulo.Date > newTitulo.dt_vcto_titulo)
                                throw new CoordenacaoBusinessException(Messages.msgErroDtEmissaoMaiorVencimento, null,
                                    CoordenacaoBusinessException.TipoErro.ERRO_DT_EMISSAO_TITULO_MAIOR_VENCIMENTO,
                                    false);

                            newTitulo.nm_parcela_titulo = (byte)(i + 1);
                            newTitulo.nomeResponsavel = titulo.nomeResponsavel;
                            newTitulo.tipoDoc = titulo.tipoDoc;
                            newTitulo.id = idNew;
                            if (titulosDescontoParcela != null && titulosDescontoParcela.Count() > 0)
                                newTitulo = Contrato.TituloDescontoParcela.buscarSetarValorParcelaDescontoTitulo(
                                    newTitulo, titulosDescontoParcela);
                            if (newTitulo.dc_tipo_titulo == "ME" || newTitulo.dc_tipo_titulo == "MA" || newTitulo.dc_tipo_titulo == "MM")
                                if (
                                    (bool)id_incorporar_material &&
                                    nm_parcelas_material > 0 &&
                                    vl_parcela_material > 0)
                                {
                                    if (i + 1 <= nm_parcelas_material)
                                    {
                                        newTitulo.vl_titulo =
                                            newTitulo.vl_titulo +
                                            decimal.Round((decimal)vl_parcela_material, 2);
                                        newTitulo.vl_saldo_titulo =
                                            newTitulo.vl_saldo_titulo +
                                            decimal.Round((decimal)vl_parcela_material, 2);
                                        newTitulo.vl_material_titulo =
                                            decimal.Round((decimal)vl_parcela_material, 2);
                                    }
                                }
                            titulos.Add(newTitulo);
                        }
                        else
                        {
                            if (titulo.dc_num_documento_titulo != null && sequenciaCheque.HasValue)
                            {
                                //incrementa a parte numerica
                                int proximoCheque = sequenciaCheque.Value + i;
                                int qtd_numeros = titulo.dc_num_documento_titulo.Length - stringCheque.Length;
                                if (qtd_numeros > 0)
                                    newTitulo.dc_num_documento_titulo =
                                        stringCheque + proximoCheque.ToString().PadLeft(qtd_numeros, '0');
                                else
                                    newTitulo.dc_num_documento_titulo = stringCheque + proximoCheque.ToString();
                            }

                            newTitulo.nm_parcela_titulo = (byte)(i + 1);
                            newTitulo.nomeResponsavel = titulo.nomeResponsavel;
                            newTitulo.tipoDoc = titulo.tipoDoc;
                            int contMes = i;
                            DateTime dataAux = new DateTime();
                            if (calcularProximaData && nm_parcela_fechadas > 1)
                                contMes = i - (nm_parcela_fechadas - 1);

                            dataAux = newTitulo.dt_vcto_titulo;
                            int mesProximo = dataAux.AddMonths(i).Month; //dataAux.AddDays((double)1 * 30 * i).Month;

                            int anoProximoVenc = dtaVencTituloAnterior.Month == 12
                                ? dtaVencTituloAnterior.Year + 1
                                : dtaVencTituloAnterior.Year;

                            DateTime dateValue;

                            int diasMes = diaSugerido > 0 ? diaSugerido : newTitulo.dt_vcto_titulo.Day;
                            while (diasMes > 0)
                            {
                                string novaData = diasMes + "/" + mesProximo + "/" + anoProximoVenc;
                                if (DateTime.TryParse(novaData, out dateValue))
                                {
                                    DateTime dt_venc = new DateTime(anoProximoVenc, mesProximo, diasMes);
                                    if (dt_venc <= minDate)
                                        throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior,
                                            null,
                                            CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR,
                                            false);
                                    if (dt_venc >= maxDate)
                                        throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                                            CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR,
                                            false);
                                    newTitulo.dt_vcto_titulo = dt_venc;
                                    diasMes = 0;
                                }
                                else
                                    diasMes--;
                            }

                            DateTime dataOpcao = newTitulo.dt_vcto_titulo;

                            int nmDia = newTitulo.dt_vcto_titulo.Day > 0
                                ? newTitulo.dt_vcto_titulo.Day
                                : (int)nmDiaParam;

                            // Calcular data de vencimento dos titulos, quando o parâmetro 'Considerar como dia útil' estiver marcado.
                            this.CalcularDiaVencimentoTit(nmDia, anoProximoVenc, mesProximo, ref dataOpcao, newTitulo,
                                titulo, feriadosEscola, diaUtil);

                            if (id_alterar_venc_final_semana)
                            {
                                DateTime dataOpcaoAux = dataOpcao;
                                pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola);
                                if ((dataOpcao.Month > newTitulo.dt_vcto_titulo.Month) ||
                                    (newTitulo.dt_vcto_titulo.Month > dataOpcao.Month))
                                {
                                    dataOpcao = dataOpcaoAux;
                                    pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola,
                                        false);
                                }
                            }

                            if (dataOpcao.Date <= minDate)
                                throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior, null,
                                    CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR, false);
                            if (dataOpcao.Date >= maxDate)
                                throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                                    CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR, false);

                            newTitulo.dt_vcto_titulo = dataOpcao.Date;

                            if (newTitulo.dt_emissao_titulo.Date > newTitulo.dt_vcto_titulo)
                                throw new CoordenacaoBusinessException(Messages.msgErroDtEmissaoMaiorVencimento, null,
                                    CoordenacaoBusinessException.TipoErro.ERRO_DT_EMISSAO_TITULO_MAIOR_VENCIMENTO,
                                    false);

                            dtaVencTituloAnterior = newTitulo.dt_vcto_titulo;
                            newTitulo.id = idNew + 1;
                            idNew = idNew + 1;
                            if (titulosDescontoParcela != null && titulosDescontoParcela.Count() > 0)
                                newTitulo = Contrato.TituloDescontoParcela.buscarSetarValorParcelaDescontoTitulo(
                                    newTitulo, titulosDescontoParcela);
                            if (newTitulo.dc_tipo_titulo == "ME" || newTitulo.dc_tipo_titulo == "MA" || newTitulo.dc_tipo_titulo == "MM")
                                if (
                                    (bool)id_incorporar_material &&
                                    nm_parcelas_material > 0 &&
                                    vl_parcela_material > 0)
                                {
                                    if (newTitulo.nm_parcela_titulo <= nm_parcelas_material)
                                    {
                                        newTitulo.vl_titulo =
                                            newTitulo.vl_titulo +
                                            decimal.Round((decimal)vl_parcela_material, 2);
                                        newTitulo.vl_saldo_titulo =
                                            newTitulo.vl_saldo_titulo +
                                            decimal.Round((decimal)vl_parcela_material, 2);
                                        newTitulo.vl_material_titulo =
                                            decimal.Round((decimal)vl_parcela_material, 2);
                                    }
                                }
                            titulos.Add(newTitulo);
                        }
                    }

                }
                else
                {
                    //títulos aluno
                    criarTitulosAlunoContrato(titulo, ref titulos, ref sequenciaCheque, ref stringCheque, ref idNew,
                        diaUtil, nmDiaParam, ref feriadosEscola, ref id_alterar_venc_final_semana, calcularProximaData,
                        nm_parcela_fechadas, diaSugerido, titulosDescontoParcela, nm_parcelas_material, vl_parcela_material, id_incorporar_material,titulosTaxaParcela);
                    //títulos Responsável
                    criarTitulosResponsavelContrato(titulo, ref titulos, ref sequenciaCheque, ref stringCheque,
                        ref idNew, diaUtil, nmDiaParam, ref feriadosEscola, ref id_alterar_venc_final_semana,
                        calcularProximaData, nm_parcela_fechadas, diaSugerido, titulosDescontoParcela, nm_parcelas_material, vl_parcela_material, id_incorporar_material,titulosTaxaParcela);
                }
            }
            else
            {
                //Titulos para o aluno
                int l = 0;
                if (calcularProximaData)
                    l = nm_parcela_fechadas;
                for (int i = l; i < titulo.nm_parcela_titulo; i++)
                {
                    Titulo newTitulo = new Titulo();
                    newTitulo.copy(titulo);
                    newTitulo.descLocalMovto = titulo.descLocalMovto;
                    if (titulos.Count() <= 0 && !calcularProximaData)
                    {
                        newTitulo.nm_parcela_titulo = (byte)(i + 1);
                        newTitulo.cd_pessoa_responsavel = newTitulo.cd_pessoa_titulo;
                        newTitulo.nomeResponsavel = titulo.nomeAluno;
                        newTitulo.tipoDoc = titulo.tipoDoc;
                        newTitulo.id = idNew + 1;
                        idNew = idNew + 1;
                        if (titulosDescontoParcela != null && titulosDescontoParcela.Count() > 0)
                            newTitulo = Contrato.TituloDescontoParcela.buscarSetarValorParcelaDescontoTitulo(newTitulo,
                                titulosDescontoParcela);
                        if (newTitulo.dc_tipo_titulo == "ME" || newTitulo.dc_tipo_titulo == "MA" || newTitulo.dc_tipo_titulo == "MM")
                            if (
                                    (bool)id_incorporar_material &&
                                    nm_parcelas_material > 0 &&
                                    vl_parcela_material > 0)
                            {
                                if (newTitulo.nm_parcela_titulo <= nm_parcelas_material)
                                {
                                    newTitulo.vl_titulo =
                                        newTitulo.vl_titulo +
                                        decimal.Round((decimal)vl_parcela_material, 2);
                                    newTitulo.vl_saldo_titulo =
                                        newTitulo.vl_saldo_titulo +
                                        decimal.Round((decimal)vl_parcela_material, 2);
                                    newTitulo.vl_material_titulo =
                                        decimal.Round((decimal)vl_parcela_material, 2);
                                }
                            }
                        titulos.Add(newTitulo);
                    }
                    else
                    {
                        if (titulo.dc_num_documento_titulo != null && sequenciaCheque.HasValue)
                        {
                            //incrementa a parte numerica
                            int proximoCheque = sequenciaCheque.Value + i;
                            int qtd_numeros = titulo.dc_num_documento_titulo.Length - stringCheque.Length;
                            if (qtd_numeros > 0)
                                titulo.dc_num_documento_titulo =
                                    stringCheque + proximoCheque.ToString().PadLeft(qtd_numeros, '0');
                            else
                                titulo.dc_num_documento_titulo = stringCheque + proximoCheque.ToString();

                        }

                        newTitulo.nm_parcela_titulo = (byte)(i + 1);
                        newTitulo.cd_pessoa_responsavel = newTitulo.cd_pessoa_titulo;
                        newTitulo.nomeResponsavel = titulo.nomeAluno;
                        newTitulo.tipoDoc = titulo.tipoDoc;
                        int contMes = i;
                        if (calcularProximaData && nm_parcela_fechadas > 1)
                            contMes = i - (nm_parcela_fechadas - 1);
                        int mesProximo = (newTitulo.dt_vcto_titulo.Month + contMes) > 12
                            ? mesProximoAno = (newTitulo.dt_vcto_titulo.Month + contMes) - 12
                            : newTitulo.dt_vcto_titulo.Month + contMes;
                        int diaVencimento = diaSugerido > 0 ? diaSugerido : newTitulo.dt_vcto_titulo.Day;
                        int anoProximoVenc = dtaVencTituloAnterior.Month == 12
                            ? dtaVencTituloAnterior.Year + 1
                            : dtaVencTituloAnterior.Year;
                        DateTime dtaVencProx = new DateTime(anoProximoVenc, mesProximo, diaVencimento);
                        if (dtaVencProx <= minDate)
                            throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior, null,
                                CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR, false);
                        if (dtaVencProx >= maxDate)
                            throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                                CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR, false);
                        newTitulo.dt_vcto_titulo = dtaVencProx;
                        DateTime dataOpcao = newTitulo.dt_vcto_titulo;

                        int diaProximo = nmDiaParam != null ? (int)nmDiaParam : newTitulo.dt_vcto_titulo.Day;
                        if (nmDiaParam.HasValue && nmDiaParam.Value > 0)
                            // A tabela foi colocada com um default de 0 para esse campo:
                            if (diaUtil.HasValue)
                                //Não considera dias úteis ou feriados:
                                if (diaUtil.Value)
                                {
                                    diaProximo = 1;
                                    dataOpcao = new DateTime(anoProximoVenc, mesProximo, diaProximo);
                                    //Calcula o próximo dia útil:
                                    for (int j = 0; j < nmDiaParam; j++)
                                    {
                                        pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa,
                                            ref feriadosEscola);

                                        if (j != nmDiaParam - 1)
                                            dataOpcao = dataOpcao.AddDays(1);
                                    }

                                    if (dataOpcao.Month > newTitulo.dt_vcto_titulo.Month)
                                    {
                                        int difMes = newTitulo.dt_vcto_titulo.Month - dataOpcao.Month;
                                        if (dataOpcao.Year != newTitulo.dt_vcto_titulo.Year)
                                            difMes -= 12;
                                        //Se pulou + de um mês, retorna a data para o mês seguinte a data do vencimento
                                        if (difMes > 1)
                                            dataOpcao = dataOpcao.AddMonths(-(difMes - 1));
                                        dataOpcao = dataOpcao.AddDays(-dataOpcao.Day);
                                        pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa,
                                            ref feriadosEscola, false);
                                    }
                                }

                        if (id_alterar_venc_final_semana)
                        {
                            DateTime dataOpcaoAux = dataOpcao;
                            pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola);
                            if (dataOpcao.Month > newTitulo.dt_vcto_titulo.Month)
                            {
                                dataOpcao = dataOpcaoAux;
                                pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola,
                                    false);
                            }
                        }

                        if (dataOpcao.Date <= minDate)
                            throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior, null,
                                CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR, false);
                        if (dataOpcao.Date >= maxDate)
                            throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                                CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR, false);


                        newTitulo.dt_vcto_titulo = dataOpcao.Date;
                        dtaVencTituloAnterior = newTitulo.dt_vcto_titulo;
                        newTitulo.id = idNew + 1;
                        idNew = idNew + 1;
                        if (titulosDescontoParcela != null && titulosDescontoParcela.Count() > 0)
                            newTitulo = Contrato.TituloDescontoParcela.buscarSetarValorParcelaDescontoTitulo(newTitulo,
                                titulosDescontoParcela);
                        if (newTitulo.dc_tipo_titulo == "ME" || newTitulo.dc_tipo_titulo == "MA" || newTitulo.dc_tipo_titulo == "MM")
                            if (
                                    (bool)id_incorporar_material &&
                                    nm_parcelas_material > 0 &&
                                    vl_parcela_material > 0)
                            {
                                if (newTitulo.nm_parcela_titulo <= nm_parcelas_material)
                                {
                                    newTitulo.vl_titulo =
                                        newTitulo.vl_titulo +
                                        decimal.Round((decimal)vl_parcela_material, 2);
                                    newTitulo.vl_saldo_titulo =
                                        newTitulo.vl_saldo_titulo +
                                        decimal.Round((decimal)vl_parcela_material, 2);
                                    newTitulo.vl_material_titulo =
                                        decimal.Round((decimal)vl_parcela_material, 2);
                                }
                            }
                        titulos.Add(newTitulo);
                    }
                }
            }

            if (titulos.Where(x => x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM")
                    .Count() > 0)
            {
                decimal divida = titulo.primeiraParc ? titulo.vl_divida : 0;
                Decimal totalMensalidades = titulos
                    .Where(x => x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM")
                    .Sum(x => x.vl_titulo);

                //Aqui o valor liquido quando o desconto é aplicado por parcela vem sem os descontos
                // precisa descontar os descontos
                if (titulosDescontoParcela != null && titulosDescontoParcela.Count() > 0 &&
                    titulosDescontoParcela.Any(d =>
                        (d.nm_parcela_ini_desconto > 0 || d.nm_parcela_fim_desconto > 0) &&
                        d.pc_total_desconto_aplicado > 0))
                {
                    var vl_total_desconto_contrato = (from td in titulosDescontoParcela
                        where td.pc_total_desconto_aplicado > 0
                        select td.vl_desconto_separado).Sum();
                    vl_liquido_total -= vl_total_desconto_contrato;
                }

                //Calcula a diferença dos arrendondamentos para primeira parcela:
                decimal diferenca = totalMensalidades + divida - totalMaterial - Decimal.Round(vl_liquido_total, 2);

                if (titulo.dc_tipo_titulo == "ME" && (titulo.vl_desc_1parc > 0 || divida > 0))
                    titulos[0].vl_titulo = Decimal.Round(titulos[0].vl_titulo - titulo.vl_desc_1parc + divida, 2);
                if ((diferenca > 0 && diferenca > 1) || (diferenca < 0 && diferenca < -1))
                    throw new MatriculaBusinessException(Messages.msgErroValorTitulo, null,
                        MatriculaBusinessException.TipoErro.ERRO_DIFERENCA_TITULOS_CURSO_MAIOR_UMREAL, false);
                else
                {
                    Titulo t = titulos.FirstOrDefault();
                    if (t != null)
                    {
                        t.vl_titulo -= diferenca;
                        t.vl_saldo_titulo = t.vl_titulo;
                    }
                }

                titulos[0].vl_saldo_titulo = titulos[0].vl_titulo;
            }

            if (titulos.Where(x => x.dc_tipo_titulo == "TM" || x.dc_tipo_titulo == "TA").Count() > 0)
            {
                Decimal totalTaxa = titulos.Where(x => x.dc_tipo_titulo == "TM" || x.dc_tipo_titulo == "TA")
                    .Sum(x => x.vl_titulo);
                decimal diferenca =
                    totalTaxa - Decimal.Round(
                        (valorTituloSemRound * (titulo.nm_parcela_titulo.Value - nm_parcela_fechadas)), 2);
                if ((diferenca > 0 && diferenca > 1) || (diferenca < 0 && diferenca < -1))
                    throw new MatriculaBusinessException(Messages.msgErroValorTitulo, null,
                        MatriculaBusinessException.TipoErro.ERRO_DIFERENCA_TITULOS_CURSO_MAIOR_UMREAL, false);
                else
                {
                    Titulo t = titulos.FirstOrDefault();
                    if (t != null)
                    {
                        t.vl_titulo -= diferenca;
                        t.vl_saldo_titulo = t.vl_titulo;
                    }
                }

                titulos[0].vl_saldo_titulo = titulos[0].vl_titulo;
            }

            // Se Tipo financeiro for Cartão Crédito ou Débito, aplicar taxa bancária;
            if (titulo.cd_tipo_financeiro == (int)FundacaoFisk.SGF.GenericModel.TipoFinanceiro.TiposFinanceiro.CARTAO)
            {
                DateTime dataVencimentoTituloCartao = titulo.dt_emissao_titulo;

                foreach (var objTitulo in titulos)
                {
                    aplicarTaxaBancaria(objTitulo, (int)titulo.nm_parcela_titulo, ref dataVencimentoTituloCartao, titulosTaxaParcela);
                }
            }

            return titulos;
        }



        private List<Titulo> criaTitulosMatriculaMultipla(Titulo titulo, bool? diaUtil, byte? nmDiaParam, int idNew,
            bool id_alterar_venc_final_semana, bool calcularProximaData,
            int nm_parcela_fechadas, List<Contrato.TituloDescontoParcela> titulosDescontoParcela,
            decimal vl_liquido_total, ref int nm_parcela_inicial,
            byte? nm_parcelas_material, decimal? vl_parcela_material, bool? id_incorporar_material, decimal totalMaterial,
            List<Contrato.TituloTaxaParcela> titulosTaxaParcela)
        {
            int diaSugerido = titulo.diaSugerido;
            Decimal valorTituloSemRound = titulo.vl_titulo;
            titulo.vl_titulo = Decimal.Round(titulo.vl_titulo, 2);
            titulo.dh_cadastro_titulo = DateTime.Now.ToUniversalTime();
            nm_parcelas_material = (nm_parcelas_material == null ? 0 : nm_parcelas_material);
            List<Titulo> titulos = new List<Titulo>();
            int mesProximoAno = 0;
            DateTime dtaVencTituloAnterior = titulo.dt_vcto_titulo;
            DateTime maxDate = new DateTime(2079, 06, 06);
            DateTime minDate = new DateTime(1900, 01, 01);
            IEnumerable<Feriado> feriadosEscola = null;
            int? sequenciaCheque = null;
            string stringCheque = "";
            //bool idiferenca = false;
            //Decimal diferencaC = 0;
            if (titulo.dc_num_documento_titulo != null)
            {
                int legDoc = titulo.dc_num_documento_titulo.Length;
                for (int p = 0; p <= legDoc; p++)
                {
                    sequenciaCheque = isNumber(titulo.dc_num_documento_titulo.Substring(p, (legDoc - p)));
                    if (sequenciaCheque.HasValue && sequenciaCheque > 0)
                    {
                        stringCheque = titulo.dc_num_documento_titulo.Substring(0, p);
                        break;
                    }
                }
            }
            //if (
            //    (bool)titulo.CursoContrato.id_incorporar_valor_material &&
            //    titulo.CursoContrato.nm_parcelas_material > 0 &&
            //    titulo.CursoContrato.vl_parcela_material > 0)
            //{
            if (titulo.dc_tipo_titulo == "ME" || titulo.dc_tipo_titulo == "MA" || titulo.dc_tipo_titulo == "MM")
                if (nm_parcelas_material > titulo.CursoContrato.nm_parcelas_mensalidade)
                    throw new MatriculaBusinessException(Messages.msgErroQtdMensaMenorQtdMaterial, null,
                        MatriculaBusinessException.TipoErro.ERRO_MATERIAL_MENSALIDADES, false);
            //    Decimal vlMaterial = (decimal)titulo.CursoContrato.vl_material_contrato;
            //    diferencaC =
            //        decimal.Round((decimal)(decimal.Round((decimal)(decimal.Round((decimal)(vlMaterial /
            //          titulo.CursoContrato.nm_parcelas_material), 2) *
            //         titulo.CursoContrato.nm_parcelas_material), 2) -
            //        titulo.CursoContrato.vl_material_contrato), 2);
            //}

            if (titulo.percentualResp != 0)
            {
                if (titulo.percentualResp == 100)
                {
                    //Pessoa do Título é a pessoa responsável
                    int l = 0;
                    if (calcularProximaData)
                        l = nm_parcela_fechadas;
                    for (int i = l; i < titulo.nm_parcela_titulo; i++)
                    {
                        mesProximoAno = mesProximoAno >= 12 ? 0 : mesProximoAno;
                        Titulo newTitulo = new Titulo();
                        newTitulo.possuiBaixa = false;
                        newTitulo.copy(titulo);
                        newTitulo.descLocalMovto = titulo.descLocalMovto;
                        idNew++;
                        //titulo.id = idNew;
                        if (titulos.Count() <= 0 && !calcularProximaData)
                        {
                            //Alteração refente ao chamado 714 - Chamado: 265215 -> (Aditamento/retirar ToLocalTime()-> data voltando 1 dia): DateTime data_opcao = titulo.dt_vcto_titulo.ToLocalTime().Date; 
                            DateTime data_opcao = titulo.dt_vcto_titulo.Date;

                            if (id_alterar_venc_final_semana)
                                pulaFeriadoEFinalSemana(ref data_opcao, titulo.cd_pessoa_empresa, ref feriadosEscola);
                            if (data_opcao <= minDate)
                                throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior, null,
                                    CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR, false);
                            if (data_opcao >= maxDate)
                                throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                                    CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR, false);

                            if (((newTitulo.dt_vcto_titulo.Month > data_opcao.Month) ||
                                 (data_opcao.Month > newTitulo.dt_vcto_titulo.Month)) && id_alterar_venc_final_semana)
                            {
                                ////Alteração refente ao chamado 714 - Chamado: 265215 -> (Aditamento/retirar ToLocalTime()-> data voltando 1 dia): data_opcao = titulo.dt_vcto_titulo.ToLocalTime().Date;
                                data_opcao = titulo.dt_vcto_titulo.Date;
                                pulaFeriadoEFinalSemana(ref data_opcao, titulo.cd_pessoa_empresa, ref feriadosEscola,
                                    false);
                            }


                            DateTime dataOpcao = data_opcao;

                            int nmDia = dataOpcao.Day > 0 ? dataOpcao.Day : (int)nmDiaParam;

                            // Calcular data de vencimento dos titulos, quando o parâmetro 'Considerar como dia útil' estiver marcado.
                            this.CalcularDiaVencimentoTit(nmDia, data_opcao.Year, data_opcao.Month, ref dataOpcao,
                                newTitulo, titulo, feriadosEscola, diaUtil);

                            newTitulo.dt_vcto_titulo = dataOpcao;

                            if (newTitulo.dt_emissao_titulo.Date > newTitulo.dt_vcto_titulo)
                                throw new CoordenacaoBusinessException(Messages.msgErroDtEmissaoMaiorVencimento, null,
                                    CoordenacaoBusinessException.TipoErro.ERRO_DT_EMISSAO_TITULO_MAIOR_VENCIMENTO,
                                    false);

                            newTitulo.nm_parcela_titulo = (byte)(nm_parcela_inicial + 1);
                            nm_parcela_inicial++;
                            newTitulo.nomeResponsavel = titulo.nomeResponsavel;
                            newTitulo.tipoDoc = titulo.tipoDoc;
                            newTitulo.id = idNew;
                            if (titulosDescontoParcela != null && titulosDescontoParcela.Count() > 0)
                                newTitulo = Contrato.TituloDescontoParcela.buscarSetarValorParcelaDescontoTitulo(
                                    newTitulo, titulosDescontoParcela);
                            if (newTitulo.dc_tipo_titulo == "ME" || newTitulo.dc_tipo_titulo == "MA" || newTitulo.dc_tipo_titulo == "MM")
                                if (
                                    (bool)id_incorporar_material &&
                                    nm_parcelas_material > 0 &&
                                    vl_parcela_material > 0)
                                {
                                    if (i + 1 <= nm_parcelas_material)
                                    {
                                        newTitulo.vl_titulo =
                                            newTitulo.vl_titulo +
                                            decimal.Round((decimal)vl_parcela_material, 2);
                                        newTitulo.vl_saldo_titulo =
                                            newTitulo.vl_saldo_titulo +
                                            decimal.Round((decimal)vl_parcela_material, 2);
                                        newTitulo.vl_material_titulo =
                                            decimal.Round((decimal)vl_parcela_material, 2);

                                        //if (!idiferenca)
                                        //    if ((diferencaC > 0 && diferencaC <= 1) || (diferencaC < 0 && diferencaC >= -1))
                                        //    {
                                        //        idiferenca = true;
                                        //        newTitulo.vl_titulo = newTitulo.vl_titulo - diferencaC;
                                        //        newTitulo.vl_saldo_titulo = newTitulo.vl_saldo_titulo - diferencaC;
                                        //        newTitulo.vl_material_titulo = newTitulo.vl_material_titulo - diferencaC;
                                        //    }
                                    }
                                }
                            titulos.Add(newTitulo);
                        }
                        else
                        {
                            if (titulo.dc_num_documento_titulo != null && sequenciaCheque.HasValue)
                            {
                                //incrementa a parte numerica
                                int proximoCheque = sequenciaCheque.Value + i;
                                int qtd_numeros = titulo.dc_num_documento_titulo.Length - stringCheque.Length;
                                if (qtd_numeros > 0)
                                    newTitulo.dc_num_documento_titulo =
                                        stringCheque + proximoCheque.ToString().PadLeft(qtd_numeros, '0');
                                else
                                    newTitulo.dc_num_documento_titulo = stringCheque + proximoCheque.ToString();
                            }

                            newTitulo.nm_parcela_titulo = (byte)(nm_parcela_inicial + 1); nm_parcela_inicial++;
                            newTitulo.nomeResponsavel = titulo.nomeResponsavel;
                            newTitulo.tipoDoc = titulo.tipoDoc;
                            int contMes = i;
                            DateTime dataAux = new DateTime();
                            if (calcularProximaData && nm_parcela_fechadas > 1)
                                contMes = i - (nm_parcela_fechadas - 1);

                            dataAux = newTitulo.dt_vcto_titulo;
                            int mesProximo = dataAux.AddMonths(i).Month; //dataAux.AddDays((double)1 * 30 * i).Month;

                            int anoProximoVenc = dtaVencTituloAnterior.Month == 12
                                ? dtaVencTituloAnterior.Year + 1
                                : dtaVencTituloAnterior.Year;

                            DateTime dateValue;

                            int diasMes = diaSugerido > 0 ? diaSugerido : newTitulo.dt_vcto_titulo.Day;
                            while (diasMes > 0)
                            {
                                string novaData = diasMes + "/" + mesProximo + "/" + anoProximoVenc;
                                if (DateTime.TryParse(novaData, out dateValue))
                                {
                                    DateTime dt_venc = new DateTime(anoProximoVenc, mesProximo, diasMes);
                                    if (dt_venc <= minDate)
                                        throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior,
                                            null,
                                            CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR,
                                            false);
                                    if (dt_venc >= maxDate)
                                        throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                                            CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR,
                                            false);
                                    newTitulo.dt_vcto_titulo = dt_venc;
                                    diasMes = 0;
                                }
                                else
                                    diasMes--;
                            }

                            DateTime dataOpcao = newTitulo.dt_vcto_titulo;

                            int nmDia = newTitulo.dt_vcto_titulo.Day > 0
                                ? newTitulo.dt_vcto_titulo.Day
                                : (int)nmDiaParam;

                            // Calcular data de vencimento dos titulos, quando o parâmetro 'Considerar como dia útil' estiver marcado.
                            this.CalcularDiaVencimentoTit(nmDia, anoProximoVenc, mesProximo, ref dataOpcao, newTitulo,
                                titulo, feriadosEscola, diaUtil);

                            if (id_alterar_venc_final_semana)
                            {
                                DateTime dataOpcaoAux = dataOpcao;
                                pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola);
                                if ((dataOpcao.Month > newTitulo.dt_vcto_titulo.Month) ||
                                    (newTitulo.dt_vcto_titulo.Month > dataOpcao.Month))
                                {
                                    dataOpcao = dataOpcaoAux;
                                    pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola,
                                        false);
                                }
                            }

                            if (dataOpcao.Date <= minDate)
                                throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior, null,
                                    CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR, false);
                            if (dataOpcao.Date >= maxDate)
                                throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                                    CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR, false);

                            newTitulo.dt_vcto_titulo = dataOpcao.Date;

                            if (newTitulo.dt_emissao_titulo.Date > newTitulo.dt_vcto_titulo)
                                throw new CoordenacaoBusinessException(Messages.msgErroDtEmissaoMaiorVencimento, null,
                                    CoordenacaoBusinessException.TipoErro.ERRO_DT_EMISSAO_TITULO_MAIOR_VENCIMENTO,
                                    false);

                            dtaVencTituloAnterior = newTitulo.dt_vcto_titulo;
                            newTitulo.id = idNew + 1;
                            idNew = idNew + 1;
                            if (titulosDescontoParcela != null && titulosDescontoParcela.Count() > 0)
                                newTitulo = Contrato.TituloDescontoParcela.buscarSetarValorParcelaDescontoTitulo(
                                    newTitulo, titulosDescontoParcela);
                            if (newTitulo.dc_tipo_titulo == "ME" || newTitulo.dc_tipo_titulo == "MA" || newTitulo.dc_tipo_titulo == "MM")
                                if (
                                    (bool)id_incorporar_material &&
                                    nm_parcelas_material > 0 &&
                                    vl_parcela_material > 0)
                                {
                                    if (i + 1 <= nm_parcelas_material)
                                    {
                                        newTitulo.vl_titulo =
                                            newTitulo.vl_titulo +
                                            decimal.Round((decimal)vl_parcela_material, 2);
                                        newTitulo.vl_saldo_titulo =
                                            newTitulo.vl_saldo_titulo +
                                            decimal.Round((decimal)vl_parcela_material, 2);
                                        newTitulo.vl_material_titulo =
                                            decimal.Round((decimal)vl_parcela_material, 2);

                                        //if (!idiferenca)
                                        //    if ((diferencaC > 0 && diferencaC <= 1) || (diferencaC < 0 && diferencaC >= -1))
                                        //    {
                                        //        idiferenca = true;
                                        //        newTitulo.vl_titulo = newTitulo.vl_titulo - diferencaC;
                                        //        newTitulo.vl_saldo_titulo = newTitulo.vl_saldo_titulo - diferencaC;
                                        //        newTitulo.vl_material_titulo = newTitulo.vl_material_titulo - diferencaC;
                                        //    }
                                    }
                                }
                            titulos.Add(newTitulo);
                        }
                    }

                }
                else
                {
                    //títulos aluno
                    criarTitulosAlunoContrato(titulo, ref titulos, ref sequenciaCheque, ref stringCheque, ref idNew,
                        diaUtil, nmDiaParam, ref feriadosEscola, ref id_alterar_venc_final_semana, calcularProximaData,
                        nm_parcela_fechadas, diaSugerido, titulosDescontoParcela, nm_parcelas_material,
                        vl_parcela_material, id_incorporar_material,titulosTaxaParcela);
                    //títulos Responsável
                    criarTitulosResponsavelContrato(titulo, ref titulos, ref sequenciaCheque, ref stringCheque,
                        ref idNew, diaUtil, nmDiaParam, ref feriadosEscola, ref id_alterar_venc_final_semana,
                        calcularProximaData, nm_parcela_fechadas, diaSugerido, titulosDescontoParcela, nm_parcelas_material,
                        vl_parcela_material, id_incorporar_material,titulosTaxaParcela);
                }
            }
            else
            {
                //Titulos para o aluno
                int l = 0;
                if (calcularProximaData)
                    l = nm_parcela_fechadas;
                for (int i = l; i < titulo.nm_parcela_titulo; i++)
                {
                    Titulo newTitulo = new Titulo();
                    newTitulo.copy(titulo);
                    newTitulo.descLocalMovto = titulo.descLocalMovto;
                    if (titulos.Count() <= 0 && !calcularProximaData)
                    {
                        newTitulo.nm_parcela_titulo = (byte)(nm_parcela_inicial + 1); nm_parcela_inicial++;
                        newTitulo.cd_pessoa_responsavel = newTitulo.cd_pessoa_titulo;
                        newTitulo.nomeResponsavel = titulo.nomeAluno;
                        newTitulo.tipoDoc = titulo.tipoDoc;
                        newTitulo.id = idNew + 1;
                        idNew = idNew + 1;
                        if (titulosDescontoParcela != null && titulosDescontoParcela.Count() > 0)
                            newTitulo = Contrato.TituloDescontoParcela.buscarSetarValorParcelaDescontoTitulo(newTitulo,
                                titulosDescontoParcela);
                        if (newTitulo.dc_tipo_titulo == "ME" || newTitulo.dc_tipo_titulo == "MA" || newTitulo.dc_tipo_titulo == "MM")
                            if (
                                (bool)id_incorporar_material &&
                                nm_parcelas_material > 0 &&
                                vl_parcela_material > 0)
                            {
                                if (i + 1 <= nm_parcelas_material)
                                {
                                    newTitulo.vl_titulo =
                                        newTitulo.vl_titulo +
                                        decimal.Round((decimal)vl_parcela_material, 2);
                                    newTitulo.vl_saldo_titulo =
                                        newTitulo.vl_saldo_titulo +
                                        decimal.Round((decimal)vl_parcela_material, 2);
                                    newTitulo.vl_material_titulo =
                                        decimal.Round((decimal)vl_parcela_material, 2);

                                    //if (!idiferenca)
                                    //    if ((diferencaC > 0 && diferencaC <= 1) || (diferencaC < 0 && diferencaC >= -1))
                                    //    {
                                    //        idiferenca = true;
                                    //        newTitulo.vl_titulo = newTitulo.vl_titulo - diferencaC;
                                    //        newTitulo.vl_saldo_titulo = newTitulo.vl_saldo_titulo - diferencaC;
                                    //        newTitulo.vl_material_titulo = newTitulo.vl_material_titulo - diferencaC;
                                    //    }
                                }
                            }
                        titulos.Add(newTitulo);
                    }
                    else
                    {
                        if (titulo.dc_num_documento_titulo != null && sequenciaCheque.HasValue)
                        {
                            //incrementa a parte numerica
                            int proximoCheque = sequenciaCheque.Value + i;
                            int qtd_numeros = titulo.dc_num_documento_titulo.Length - stringCheque.Length;
                            if (qtd_numeros > 0)
                                titulo.dc_num_documento_titulo =
                                    stringCheque + proximoCheque.ToString().PadLeft(qtd_numeros, '0');
                            else
                                titulo.dc_num_documento_titulo = stringCheque + proximoCheque.ToString();

                        }

                        newTitulo.nm_parcela_titulo = (byte)(nm_parcela_inicial + 1); nm_parcela_inicial++;
                        newTitulo.cd_pessoa_responsavel = newTitulo.cd_pessoa_titulo;
                        newTitulo.nomeResponsavel = titulo.nomeAluno;
                        newTitulo.tipoDoc = titulo.tipoDoc;
                        int contMes = i;
                        if (calcularProximaData && nm_parcela_fechadas > 1)
                            contMes = i - (nm_parcela_fechadas - 1);
                        int mesProximo = (newTitulo.dt_vcto_titulo.Month + contMes) > 12
                            ? mesProximoAno = (newTitulo.dt_vcto_titulo.Month + contMes) - 12
                            : newTitulo.dt_vcto_titulo.Month + contMes;
                        int diaVencimento = diaSugerido > 0 ? diaSugerido : newTitulo.dt_vcto_titulo.Day;
                        int anoProximoVenc = dtaVencTituloAnterior.Month == 12
                            ? dtaVencTituloAnterior.Year + 1
                            : dtaVencTituloAnterior.Year;
                        DateTime dtaVencProx = new DateTime(anoProximoVenc, mesProximo, diaVencimento);
                        if (dtaVencProx <= minDate)
                            throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior, null,
                                CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR, false);
                        if (dtaVencProx >= maxDate)
                            throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                                CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR, false);
                        newTitulo.dt_vcto_titulo = dtaVencProx;
                        DateTime dataOpcao = newTitulo.dt_vcto_titulo;

                        int diaProximo = nmDiaParam != null ? (int)nmDiaParam : newTitulo.dt_vcto_titulo.Day;
                        if (nmDiaParam.HasValue && nmDiaParam.Value > 0)
                            // A tabela foi colocada com um default de 0 para esse campo:
                            if (diaUtil.HasValue)
                                //Não considera dias úteis ou feriados:
                                if (diaUtil.Value)
                                {
                                    diaProximo = 1;
                                    dataOpcao = new DateTime(anoProximoVenc, mesProximo, diaProximo);
                                    //Calcula o próximo dia útil:
                                    for (int j = 0; j < nmDiaParam; j++)
                                    {
                                        pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa,
                                            ref feriadosEscola);

                                        if (j != nmDiaParam - 1)
                                            dataOpcao = dataOpcao.AddDays(1);
                                    }

                                    if (dataOpcao.Month > newTitulo.dt_vcto_titulo.Month)
                                    {
                                        int difMes = newTitulo.dt_vcto_titulo.Month - dataOpcao.Month;
                                        if (dataOpcao.Year != newTitulo.dt_vcto_titulo.Year)
                                            difMes -= 12;
                                        //Se pulou + de um mês, retorna a data para o mês seguinte a data do vencimento
                                        if (difMes > 1)
                                            dataOpcao = dataOpcao.AddMonths(-(difMes - 1));
                                        dataOpcao = dataOpcao.AddDays(-dataOpcao.Day);
                                        pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa,
                                            ref feriadosEscola, false);
                                    }
                                }

                        if (id_alterar_venc_final_semana)
                        {
                            DateTime dataOpcaoAux = dataOpcao;
                            pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola);
                            if (dataOpcao.Month > newTitulo.dt_vcto_titulo.Month)
                            {
                                dataOpcao = dataOpcaoAux;
                                pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola,
                                    false);
                            }
                        }

                        if (dataOpcao.Date <= minDate)
                            throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior, null,
                                CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR, false);
                        if (dataOpcao.Date >= maxDate)
                            throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                                CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR, false);


                        newTitulo.dt_vcto_titulo = dataOpcao.Date;
                        dtaVencTituloAnterior = newTitulo.dt_vcto_titulo;
                        newTitulo.id = idNew + 1;
                        idNew = idNew + 1;
                        if (titulosDescontoParcela != null && titulosDescontoParcela.Count() > 0)
                            newTitulo = Contrato.TituloDescontoParcela.buscarSetarValorParcelaDescontoTitulo(newTitulo,
                                titulosDescontoParcela);
                        if (newTitulo.dc_tipo_titulo == "ME" || newTitulo.dc_tipo_titulo == "MA" || newTitulo.dc_tipo_titulo == "MM")
                            if (
                                (bool)id_incorporar_material &&
                                nm_parcelas_material > 0 &&
                                vl_parcela_material > 0)
                            {
                                if (i + 1 <= nm_parcelas_material)
                                {
                                    newTitulo.vl_titulo =
                                        newTitulo.vl_titulo +
                                        decimal.Round((decimal)vl_parcela_material, 2);
                                    newTitulo.vl_saldo_titulo =
                                        newTitulo.vl_saldo_titulo +
                                        decimal.Round((decimal)vl_parcela_material, 2);
                                    newTitulo.vl_material_titulo =
                                        decimal.Round((decimal)vl_parcela_material, 2);

                                    //if (!idiferenca)
                                    //    if ((diferencaC > 0 && diferencaC <= 1) || (diferencaC < 0 && diferencaC >= -1))
                                    //    {
                                    //        idiferenca = true;
                                    //        newTitulo.vl_titulo = newTitulo.vl_titulo - diferencaC;
                                    //        newTitulo.vl_saldo_titulo = newTitulo.vl_saldo_titulo - diferencaC;
                                    //        newTitulo.vl_material_titulo = newTitulo.vl_material_titulo - diferencaC;
                                    //    }
                                }
                            }
                        titulos.Add(newTitulo);
                    }
                }
            }

            if (titulos.Where(x => x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM")
                    .Count() > 0)
            {
                decimal divida = titulo.primeiraParc ? titulo.vl_divida : 0;
                Decimal totalMensalidades = titulos
                    .Where(x => x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM")
                    .Sum(x => x.vl_titulo);

                if (titulosDescontoParcela != null && titulosDescontoParcela.Count() > 0 &&
                    titulosDescontoParcela.Any(d =>
                        (d.nm_parcela_ini_desconto >= 0 || d.nm_parcela_fim_desconto >= 0) &&
                        d.pc_total_desconto_aplicado > 0 && 
                        (d.nm_parcela_titulo >= titulos[0].nm_parcela_titulo &&
                         d.nm_parcela_titulo < titulos[0].nm_parcela_titulo + titulos.Count)))
                {
                    var vl_total_desconto_contrato = (from td in titulosDescontoParcela
                                                      where td.pc_total_desconto_aplicado > 0 &&
                                                      (td.nm_parcela_titulo >= titulos[0].nm_parcela_titulo &&
                                                        td.nm_parcela_titulo < titulos[0].nm_parcela_titulo + titulos.Count)
                                                      select td.vl_desconto_separado).Sum();
                    vl_liquido_total -= vl_total_desconto_contrato;
                }

                //Calcula a diferença dos arrendondamentos para primeira parcela:
                //A divida não está incorporada no valor do titulo
                decimal diferenca = totalMensalidades - totalMaterial - Decimal.Round(vl_liquido_total, 2);

                if (titulo.dc_tipo_titulo == "ME" && titulos[0].nm_parcela_titulo == 1 && (titulo.vl_desc_1parc > 0 || divida > 0))
                    titulos[0].vl_titulo = Decimal.Round(titulos[0].vl_titulo - titulo.vl_desc_1parc + divida, 2);
                if ((diferenca > 0 && diferenca > 1) || (diferenca < 0 && diferenca < -1))
                    throw new MatriculaBusinessException(Messages.msgErroValorTitulo, null,
                        MatriculaBusinessException.TipoErro.ERRO_DIFERENCA_TITULOS_CURSO_MAIOR_UMREAL, false);
                else
                {
                    Titulo t = titulos.FirstOrDefault();
                    if (t != null)
                    {
                        t.vl_titulo -= diferenca;
                        t.vl_saldo_titulo = t.vl_titulo;
                    }
                }

                titulos[0].vl_saldo_titulo = titulos[0].vl_titulo;
            }

            if (titulos.Where(x => x.dc_tipo_titulo == "TM" || x.dc_tipo_titulo == "TA").Count() > 0)
            {
                Decimal totalTaxa = titulos.Where(x => x.dc_tipo_titulo == "TM" || x.dc_tipo_titulo == "TA")
                    .Sum(x => x.vl_titulo);
                decimal diferenca =
                    totalTaxa - Decimal.Round(
                        (valorTituloSemRound * (titulo.nm_parcela_titulo.Value - nm_parcela_fechadas)), 2);
                if ((diferenca > 0 && diferenca > 1) || (diferenca < 0 && diferenca < -1))
                    throw new MatriculaBusinessException(Messages.msgErroValorTitulo, null,
                        MatriculaBusinessException.TipoErro.ERRO_DIFERENCA_TITULOS_CURSO_MAIOR_UMREAL, false);
                else
                {
                    Titulo t = titulos.FirstOrDefault();
                    if (t != null)
                    {
                        t.vl_titulo -= diferenca;
                        t.vl_saldo_titulo = t.vl_titulo;
                    }
                }

                titulos[0].vl_saldo_titulo = titulos[0].vl_titulo;
            }

            return titulos;
        }

        public void aplicarTaxaBancaria(Titulo objTitulo, int nm_parcelas_mensalidade, ref DateTime dataVencimentoTituloCartao, List<Contrato.TituloTaxaParcela> titulosTaxaParcela)
        {
            //List<LocalMovto> locaisMovtoCartao = new List<LocalMovto>();
            List<LocalMovto> locaisMovtoCartaoNmParcelasIguais = new List<LocalMovto>();
            List<LocalMovto> locaisMovtoCartaoNmParcelasDiferentes = new List<LocalMovto>();
            var taxaBancariaAplicar = new TaxaBancaria();

            var taxaBancariaAplicarNmParcelasIguais = new TaxaBancaria();
            var taxaBancariaAplicarNmParcelasDiferentes = new TaxaBancaria();

            var localMovtoAplicar = new LocalMovto();
            var local = new LocalMovtoUI();

            local = financeiroBusiness.getLocalByTitulo(objTitulo.cd_pessoa_empresa, objTitulo.cd_local_movto);

            if (local != null && (local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO))
            {
                taxaBancariaAplicar = local.taxaBancaria.Where(t => t.nm_parcela == nm_parcelas_mensalidade).OrderBy(x => x.cd_taxa_bancaria).FirstOrDefault();
                taxaBancariaAplicarNmParcelasDiferentes = local.taxaBancaria.Where(t => t.nm_parcela == nm_parcelas_mensalidade).OrderBy(x => x.cd_taxa_bancaria).FirstOrDefault();

                localMovtoAplicar.cd_local_movto = local.cd_local_movto;
                localMovtoAplicar.no_local_movto = local.no_local_movto;
                localMovtoAplicar.cd_pessoa_empresa = local.cd_pessoa_empresa;
                //Se não tiver taxa bancaria seta pc_taxa e nm_dia = 0
                if (local.taxaBancaria.Count() == 0)
                {
                    preencheLocalETaxaMovt(ref objTitulo, localMovtoAplicar, 0, 0, local.cd_local_movto, local.no_local_movto);

                }
                else if (local.taxaBancaria.Count() > 0 && taxaBancariaAplicar == null) //Se tiver taxa bancaria e for diferente do nm_parcela, busca a maxima dos valores que são <= ao nm_parcela 
                {
                    var taxaslocalMenorIgualNmParcelas = local.taxaBancaria
                        .Where(t => t.nm_parcela <= nm_parcelas_mensalidade).OrderBy(x => x.cd_taxa_bancaria);
                    if (taxaslocalMenorIgualNmParcelas != null && taxaslocalMenorIgualNmParcelas.Count() > 0)
                    {
                        var valorlocalMaxMenorIgualNmParcelas = taxaslocalMenorIgualNmParcelas.Max(b => b.nm_parcela);
                        if (valorlocalMaxMenorIgualNmParcelas > 0)
                        {
                            taxaBancariaAplicar = local.taxaBancaria.Where(g => g.nm_parcela == valorlocalMaxMenorIgualNmParcelas).FirstOrDefault();
                        }
                        else
                        {
                            taxaBancariaAplicar = null;
                        }

                    }
                    else
                    {
                        taxaBancariaAplicar = null;
                    }



                    if (taxaBancariaAplicar == null) // Se nao encontrou a maxima entre os que são <=, busca a maxima geral
                    {

                        var taxasLocalNmParcelas = local.taxaBancaria
                            .Where(t => t.nm_parcela <= nm_parcelas_mensalidade).OrderBy(x => x.cd_taxa_bancaria);
                        if (taxasLocalNmParcelas != null && taxasLocalNmParcelas.Count() > 0)
                        {
                            var valorLocalMaxNmParcelas = taxasLocalNmParcelas.Max(b => b.nm_parcela);
                            if (valorLocalMaxNmParcelas > 0)
                            {
                                taxaBancariaAplicar = local.taxaBancaria.Where(g => g.nm_parcela == valorLocalMaxNmParcelas).OrderBy(x => x.cd_taxa_bancaria).FirstOrDefault();
                                preencheLocalETaxa(ref objTitulo, localMovtoAplicar, taxaBancariaAplicar.pc_taxa, taxaBancariaAplicar.nm_dias, local.cd_local_movto, local.no_local_movto, ref dataVencimentoTituloCartao, titulosTaxaParcela);

                            }
                            else
                            {
                                taxaBancariaAplicar = null;
                            }
                        }
                        else
                        {
                            taxaBancariaAplicar = null;
                        }


                    }
                    else // Seta a maxima entre os que são <=
                    {
                        preencheLocalETaxa(ref objTitulo, localMovtoAplicar, taxaBancariaAplicar.pc_taxa, taxaBancariaAplicar.nm_dias, local.cd_local_movto, local.no_local_movto, ref dataVencimentoTituloCartao, titulosTaxaParcela);

                    }

                }
                else if (local.taxaBancaria.Count() > 0 && taxaBancariaAplicar != null) //Se tiver taxa bancaria e for igual seta a taxa que encontrou
                {
                    preencheLocalETaxa(ref objTitulo, localMovtoAplicar, taxaBancariaAplicar.pc_taxa, taxaBancariaAplicar.nm_dias, local.cd_local_movto, local.no_local_movto, ref dataVencimentoTituloCartao, titulosTaxaParcela);

                }

            }
            else
            {
                localMovtoAplicar = financeiroBusiness.getAllLocalMovtoCartaoSemPai(objTitulo.cd_pessoa_empresa).OrderBy(l => l.cd_local_movto).FirstOrDefault();
                if (localMovtoAplicar == null)
                {
                    throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroNaoExisteLocalMovtoCartao), null,
                        FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_LOCALMOVTO_CARTAO, false);
                }

                taxaBancariaAplicar = localMovtoAplicar.TaxaBancaria.Where(t => t.nm_parcela == nm_parcelas_mensalidade)
                    .OrderBy(x => x.cd_taxa_bancaria).FirstOrDefault();

                //Se não tiver taxa bancaria seta pc_taxa e nm_dia = 0
                if (localMovtoAplicar.TaxaBancaria.Count() == 0)
                {
                    preencheLocalETaxaMovt(ref objTitulo, localMovtoAplicar, 0, 0, localMovtoAplicar.cd_local_movto, localMovtoAplicar.no_local_movto);

                }
                else if (localMovtoAplicar.TaxaBancaria.Count() > 0 && taxaBancariaAplicar == null) //Se tiver taxa bancaria e for diferente do nm_parcela, busca a maxima dos valores que são <= ao nm_parcela 
                {
                    var taxasMenorIgualNmParcelas = localMovtoAplicar.TaxaBancaria
                        .Where(t => t.nm_parcela <= nm_parcelas_mensalidade).OrderBy(x => x.cd_taxa_bancaria);
                    if (taxasMenorIgualNmParcelas != null && taxasMenorIgualNmParcelas.Count() > 0)
                    {
                        var valorMaxMenorIgualNmParcelas = taxasMenorIgualNmParcelas.Max(b => b.nm_parcela);
                        if (valorMaxMenorIgualNmParcelas > 0)
                        {
                            taxaBancariaAplicar = localMovtoAplicar.TaxaBancaria.Where(g => g.nm_parcela == valorMaxMenorIgualNmParcelas).FirstOrDefault();

                        }
                        else
                        {
                            taxaBancariaAplicar = null;
                        }

                    }
                    else
                    {
                        taxaBancariaAplicar = null;
                    }

                    if (taxaBancariaAplicar == null) // Se nao encontrou a taxa maxima entre os que são <=, busca a maxima geral
                    {
                        var taxasNmParcelas = localMovtoAplicar.TaxaBancaria
                            .Where(t => t.nm_parcela <= nm_parcelas_mensalidade).OrderBy(x => x.cd_taxa_bancaria);
                        if (taxasNmParcelas != null && taxasNmParcelas.Count() > 0)
                        {
                            var valorMaxNmParcelas = taxasNmParcelas.Max(b => b.nm_parcela);
                            if (valorMaxNmParcelas > 0)
                            {
                                taxaBancariaAplicar = localMovtoAplicar.TaxaBancaria.Where(g => g.nm_parcela == valorMaxNmParcelas).OrderBy(x => x.cd_taxa_bancaria).FirstOrDefault();
                                preencheLocalETaxa(ref objTitulo, localMovtoAplicar, taxaBancariaAplicar.pc_taxa, taxaBancariaAplicar.nm_dias, localMovtoAplicar.cd_local_movto, localMovtoAplicar.no_local_movto, ref dataVencimentoTituloCartao, titulosTaxaParcela);
                            }
                            else
                            {
                                taxaBancariaAplicar = null;
                            }
                        }
                        else
                        {
                            taxaBancariaAplicar = null;
                        }

                    }
                    else // Seta a maxima entre os que são <=
                    {
                        preencheLocalETaxa(ref objTitulo, localMovtoAplicar, taxaBancariaAplicar.pc_taxa, taxaBancariaAplicar.nm_dias, localMovtoAplicar.cd_local_movto, localMovtoAplicar.no_local_movto, ref dataVencimentoTituloCartao, titulosTaxaParcela);
                    }

                }
                else if (localMovtoAplicar.TaxaBancaria.Count() > 0 && taxaBancariaAplicar != null) //Se tiver taxa bancaria e for igual nm_parcela seta a taxa que encontrou
                {
                    preencheLocalETaxa(ref objTitulo, localMovtoAplicar, taxaBancariaAplicar.pc_taxa, taxaBancariaAplicar.nm_dias, localMovtoAplicar.cd_local_movto, localMovtoAplicar.no_local_movto, ref dataVencimentoTituloCartao, titulosTaxaParcela);

                }

            }

        }

        private void preencheLocalETaxaMovt(ref Titulo objTitulo, LocalMovto localMovtoAplicar, double pc_taxa, byte nm_dias, int cd_local_movto,
            string no_local_movto)
        {
            localMovtoAplicar.cd_local_movto = localMovtoAplicar.cd_local_movto;
            objTitulo.nm_dias_cartao = nm_dias;
            objTitulo.cd_local_movto = cd_local_movto;
            objTitulo.descLocalMovto = no_local_movto;
            objTitulo.pc_taxa_cartao = pc_taxa;
            objTitulo.vl_taxa_cartao = Math.Round((decimal)objTitulo.pc_taxa_cartao * (objTitulo.vl_titulo / (decimal)100.0), 2, MidpointRounding.AwayFromZero);
        }


      private void preencheLocalETaxa(ref Titulo objTitulo, LocalMovto localMovtoAplicar, double pc_taxa, byte nm_dias, int cd_local_movto,
            string no_local_movto, ref DateTime dataVencimentoTituloCartao, List<Contrato.TituloTaxaParcela> titulosTaxaParcela)
        {
            Titulo tituloBco = null;
            if (objTitulo.cd_origem_titulo != null && objTitulo.id_origem_titulo == (int)Titulo.OrigemTitulo.CONTRATO)
            {
                int cd_contrato = (int)objTitulo.cd_origem_titulo;
                int nm_parcela = (int)objTitulo.nm_parcela_titulo;
                tituloBco = financeiroBusiness.getTituloByContrato(cd_contrato, nm_parcela);
            }
            Decimal vl_taxa = Math.Round((decimal)pc_taxa * (objTitulo.vl_titulo / (decimal)100.0), 2, MidpointRounding.AwayFromZero);
            localMovtoAplicar.cd_local_movto = localMovtoAplicar.cd_local_movto;
            objTitulo.nm_dias_cartao = nm_dias;
            objTitulo.cd_local_movto = cd_local_movto;
            objTitulo.LocalMovto = localMovtoAplicar;
            if (tituloBco == null || pc_taxa > 0)
            {
                if (titulosTaxaParcela == null || titulosTaxaParcela.Count == 0)
                {
                    objTitulo.pc_taxa_cartao = pc_taxa;
                    objTitulo.vl_taxa_cartao = vl_taxa;
                }
                else
                {
                    int nm_parcela = (int)objTitulo.nm_parcela_titulo;
                    Contrato.TituloTaxaParcela tituloParcela = titulosTaxaParcela.Where(x => x.nm_parcela_titulo == nm_parcela).FirstOrDefault();
                    if (tituloParcela != null)
                    {
                        objTitulo.pc_taxa_cartao = tituloParcela.pc_taxa_cartao == 0 ? pc_taxa : tituloParcela.pc_taxa_cartao;
                        objTitulo.vl_taxa_cartao = tituloParcela.vl_taxa_cartao == 0 ? vl_taxa : tituloParcela.vl_taxa_cartao;
                    }
                }
            }
            else
            if (tituloBco.vl_taxa_cartao == 0 || tituloBco.vl_titulo != objTitulo.vl_titulo)
            {
                objTitulo.pc_taxa_cartao = pc_taxa;
                objTitulo.vl_taxa_cartao = vl_taxa;
            }
            else
            {
                objTitulo.pc_taxa_cartao = tituloBco.pc_taxa_cartao;
                objTitulo.vl_taxa_cartao = tituloBco.vl_taxa_cartao;
            }

            objTitulo.dt_vcto_titulo = dataVencimentoTituloCartao.AddDays(objTitulo.nm_dias_cartao);
            dataVencimentoTituloCartao = dataVencimentoTituloCartao.AddDays(objTitulo.nm_dias_cartao);
        }

        private void CalcularDiaVencimentoTit(int nmDia, int ano, int mes, ref DateTime dataOpcao, Titulo newTitulo,
            Titulo titulo, IEnumerable<Feriado> feriadosEscola, bool? diaUtil)
        {
            if (nmDia > 0)
            {
                //Não considera dias úteis ou feriados:
                if (diaUtil.HasValue && diaUtil.Value)
                {
                    int diaProximo = 1;
                    dataOpcao = new DateTime(ano, mes, diaProximo);
                    //Calcula o próximo dia útil:
                    for (int j = 0; j < nmDia; j++)
                    {
                        pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola);

                        if (j != nmDia - 1)
                            dataOpcao = dataOpcao.AddDays(1);
                    }

                    if ((dataOpcao.Month > newTitulo.dt_vcto_titulo.Month) ||
                        (newTitulo.dt_vcto_titulo.Month > dataOpcao.Month))
                    {
                        int difMes = newTitulo.dt_vcto_titulo.Month - dataOpcao.Month;
                        if (dataOpcao.Year != newTitulo.dt_vcto_titulo.Year)
                            difMes -= 12;
                        //Se pulou + de um mês, retorna a data para o mês seguinte a data do vencimento
                        if (difMes > 1)
                            dataOpcao = dataOpcao.AddMonths(-(difMes - 1));
                        dataOpcao = dataOpcao.AddDays(-dataOpcao.Day);
                        pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola, false);
                    }
                }
            }
        }

        private void criarTitulosAlunoContrato(Titulo titulo, ref List<Titulo> titulos, ref int? sequenciaCheque,
            ref string stringCheque, ref int idNew, bool? diaUtil,
            byte? nmDia, ref IEnumerable<Feriado> feriadosEscola, ref bool id_alterar_venc_final_semana,
            bool calcularProximaData,
            int countParcelasFechadasMensAluno, int diaSugerido,
            List<Contrato.TituloDescontoParcela> titulosDescontoParcela, byte? nm_parcelas_material, decimal? vl_parcela_material, bool? id_incorporar_material,
            List<Contrato.TituloTaxaParcela> titulosTaxaPacela)
        {
            DateTime dtaVencTituloAnterior = titulo.dt_vcto_titulo;
            int mesProximoAno = 0;
            DateTime maxDate = new DateTime(2079, 06, 06);
            DateTime minDate = new DateTime(1900, 01, 01);
            int l = 0;
            if (calcularProximaData)
                l = countParcelasFechadasMensAluno;
            //Fazer títulos Aluno
            for (int i = l; i < titulo.nm_parcela_titulo; i++)
            {
                Titulo newTitulo = new Titulo();
                newTitulo.copy(titulo);
                newTitulo.descLocalMovto = titulo.descLocalMovto;
                if (titulos.Count() <= 0 && !calcularProximaData)
                {
                    newTitulo.nm_parcela_titulo = (byte)(i + 1);
                    if (titulosDescontoParcela != null && titulosDescontoParcela.Count() > 0)
                        newTitulo = Contrato.TituloDescontoParcela.buscarSetarValorParcelaDescontoTitulo(newTitulo,
                            titulosDescontoParcela);
                    newTitulo.vl_titulo = (newTitulo.vl_titulo * (100 - titulo.percentualResp)) / 100;
                    newTitulo.cd_pessoa_responsavel = newTitulo.cd_pessoa_titulo;
                    newTitulo.nomeResponsavel = titulo.nomeAluno;
                    newTitulo.tipoDoc = titulo.tipoDoc;
                    newTitulo.id = idNew + 1;
                    newTitulo.vl_saldo_titulo = newTitulo.vl_titulo;
                    idNew = idNew + 1;
                    if (newTitulo.dc_tipo_titulo == "ME" || newTitulo.dc_tipo_titulo == "MA" || newTitulo.dc_tipo_titulo == "MM")
                        if (
                            (bool)id_incorporar_material &&
                            nm_parcelas_material > 0 &&
                            vl_parcela_material > 0)
                        {
                            if (i + 1 <= nm_parcelas_material)
                            {
                                newTitulo.vl_titulo =
                                    newTitulo.vl_titulo +
                                    decimal.Round((decimal)vl_parcela_material * ((100 - titulo.percentualResp) / 100), 2);
                                newTitulo.vl_saldo_titulo =
                                    newTitulo.vl_saldo_titulo +
                                    decimal.Round((decimal)vl_parcela_material * ((100 - titulo.percentualResp) / 100), 2);
                                newTitulo.vl_material_titulo =
                                    decimal.Round((decimal)vl_parcela_material * ((100 - titulo.percentualResp) / 100), 2);

                            }
                        }
                    titulos.Add(newTitulo);
                }
                else
                {
                    if (titulo.dc_num_documento_titulo != null && sequenciaCheque.HasValue)
                    {
                        //incrementa a parte numerica
                        int proximoCheque = sequenciaCheque.Value + i;
                        int qtd_numeros = titulo.dc_num_documento_titulo.Length - stringCheque.Length;
                        if (qtd_numeros > 0)
                            titulo.dc_num_documento_titulo =
                                stringCheque + proximoCheque.ToString().PadLeft(qtd_numeros, '0');
                        else
                            titulo.dc_num_documento_titulo = stringCheque + proximoCheque.ToString();

                    }

                    newTitulo.nm_parcela_titulo = (byte)(i + 1);
                    if (titulosDescontoParcela != null && titulosDescontoParcela.Count() > 0)
                        newTitulo = Contrato.TituloDescontoParcela.buscarSetarValorParcelaDescontoTitulo(newTitulo,
                            titulosDescontoParcela);
                    newTitulo.vl_titulo = (newTitulo.vl_titulo * (100 - titulo.percentualResp)) / 100;
                    newTitulo.vl_saldo_titulo = newTitulo.vl_titulo;
                    newTitulo.cd_pessoa_responsavel = newTitulo.cd_pessoa_titulo;
                    newTitulo.nomeResponsavel = titulo.nomeAluno;
                    newTitulo.tipoDoc = titulo.tipoDoc;
                    int contMes = i;
                    if (calcularProximaData && countParcelasFechadasMensAluno > 1)
                        contMes = i - (countParcelasFechadasMensAluno - 1);
                    int mesProximo = (newTitulo.dt_vcto_titulo.Month + contMes) > 12
                        ? mesProximoAno = (newTitulo.dt_vcto_titulo.Month + contMes) - 12
                        : newTitulo.dt_vcto_titulo.Month + contMes;

                    int anoProximoVenc = dtaVencTituloAnterior.Month == 12
                        ? dtaVencTituloAnterior.Year + 1
                        : dtaVencTituloAnterior.Year;
                    int diaVencimento = diaSugerido > 0 ? diaSugerido : newTitulo.dt_vcto_titulo.Day;
                    DateTime dtaVencProx = new DateTime(anoProximoVenc, mesProximo, diaVencimento);
                    if (dtaVencProx <= minDate)
                        throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior, null,
                            CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR, false);
                    if (dtaVencProx >= maxDate)
                        throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                            CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR, false);
                    newTitulo.dt_vcto_titulo = dtaVencProx;
                    DateTime dataOpcao = newTitulo.dt_vcto_titulo;
                    //pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa.Value);


                    int diaProximo = nmDia != null ? (int)nmDia : newTitulo.dt_vcto_titulo.Day;
                    if (nmDia.HasValue && nmDia.Value > 0)
                        // A tabela foi colocada com um default de 0 para esse campo:
                        if (diaUtil.HasValue)
                            //Não considera dias úteis ou feriados:
                            if (diaUtil.Value)
                            {
                                diaProximo = 1;
                                dataOpcao = new DateTime(anoProximoVenc, mesProximo, diaProximo);
                                //Calcula o próximo dia útil:
                                for (int j = 0; j < nmDia; j++)
                                {
                                    pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa,
                                        ref feriadosEscola);

                                    if (j != nmDia - 1)
                                        dataOpcao = dataOpcao.AddDays(1);
                                }

                                if (dataOpcao.Month > newTitulo.dt_vcto_titulo.Month)
                                {
                                    int difMes = newTitulo.dt_vcto_titulo.Month - dataOpcao.Month;
                                    if (dataOpcao.Year != newTitulo.dt_vcto_titulo.Year)
                                        difMes -= 12;
                                    //Se pulou + de um mês, retorna a data para o mês seguinte a data do vencimento
                                    if (difMes > 1)
                                        dataOpcao = dataOpcao.AddMonths(-(difMes - 1));
                                    dataOpcao = dataOpcao.AddDays(-dataOpcao.Day);
                                    pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola,
                                        false);
                                }
                            }

                    if (id_alterar_venc_final_semana)
                    {
                        DateTime dataOpcaoAux = dataOpcao;
                        pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola);
                        if (dataOpcao.Month > newTitulo.dt_vcto_titulo.Month)
                        {
                            dataOpcao = dataOpcaoAux;
                            pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola, false);
                        }
                    }

                    if (dataOpcao.Date <= minDate)
                        throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior, null,
                            CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR, false);
                    if (dataOpcao.Date >= maxDate)
                        throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                            CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR, false);

                    newTitulo.dt_vcto_titulo = dataOpcao.Date;
                    dtaVencTituloAnterior = newTitulo.dt_vcto_titulo;
                    newTitulo.id = idNew + 1;
                    idNew = idNew + 1;
                    if (newTitulo.dc_tipo_titulo == "ME" || newTitulo.dc_tipo_titulo == "MA" || newTitulo.dc_tipo_titulo == "MM")
                        if (
                            (bool)id_incorporar_material &&
                            nm_parcelas_material > 0 &&
                            vl_parcela_material > 0)
                        {

                            if (i + 1 <= nm_parcelas_material)
                            {
                                newTitulo.vl_titulo =
                                    newTitulo.vl_titulo +
                                    decimal.Round((decimal)vl_parcela_material * ((100 - titulo.percentualResp) / 100), 2);
                                newTitulo.vl_saldo_titulo =
                                    newTitulo.vl_saldo_titulo +
                                    decimal.Round((decimal)vl_parcela_material * ((100 - titulo.percentualResp) / 100), 2);
                                newTitulo.vl_material_titulo =
                                    decimal.Round((decimal)vl_parcela_material * ((100 - titulo.percentualResp) / 100), 2);

                            }
                        }
                    titulos.Add(newTitulo);
                }

            }
        }

        private void criarTitulosResponsavelContrato(Titulo titulo, ref List<Titulo> titulos, ref int? sequenciaCheque,
            ref string stringCheque,
            ref int idNew, bool? diaUtil, byte? nmDia, ref IEnumerable<Feriado> feriadosEscola,
            ref bool id_alterar_venc_final_semana, bool calcularProximaData,
            int countParcelasFechadasMensRespo, int diaSugerido,
            List<Contrato.TituloDescontoParcela> titulosDescontoParcela, byte? nm_parcelas_material, decimal? vl_parcela_material, bool? id_incorporar_material,
            List<Contrato.TituloTaxaParcela> titulosTaxaParcela)
        {
            DateTime dtaVencTituloAnterior = titulo.dt_vcto_titulo;
            int mesProximoAno = 0;
            DateTime maxDate = new DateTime(2079, 06, 06);
            DateTime minDate = new DateTime(1900, 01, 01);
            //Fazer títulos Aluno
            int l = 0;
            if (calcularProximaData)
                l = countParcelasFechadasMensRespo;
            for (int i = l; i < titulo.nm_parcela_titulo; i++)
            {
                Titulo newTitulo = new Titulo();
                newTitulo.copy(titulo);
                newTitulo.descLocalMovto = titulo.descLocalMovto;
                if (titulos.Count() <= 0 && !calcularProximaData)
                {
                    newTitulo.nm_parcela_titulo = (byte)(i + 1);
                    if (titulosDescontoParcela != null && titulosDescontoParcela.Count() > 0)
                        newTitulo = Contrato.TituloDescontoParcela.buscarSetarValorParcelaDescontoTitulo(newTitulo,
                            titulosDescontoParcela);
                    newTitulo.vl_titulo = (newTitulo.vl_titulo * titulo.percentualResp) / 100;
                    newTitulo.nomeResponsavel = titulo.nomeResponsavel;
                    newTitulo.tipoDoc = titulo.tipoDoc;
                    newTitulo.vl_saldo_titulo = newTitulo.vl_titulo;
                    newTitulo.id = idNew + 1;
                    idNew = idNew + 1;
                    if (newTitulo.dc_tipo_titulo == "ME" || newTitulo.dc_tipo_titulo == "MA" || newTitulo.dc_tipo_titulo == "MM")
                        if (
                            (bool)id_incorporar_material &&
                            nm_parcelas_material > 0 &&
                            vl_parcela_material > 0)
                        {
                            if (i + 1 <= nm_parcelas_material)
                            {
                                newTitulo.vl_titulo =
                                    newTitulo.vl_titulo +
                                    decimal.Round((decimal)vl_parcela_material * ((titulo.percentualResp) / 100), 2);
                                newTitulo.vl_saldo_titulo =
                                    newTitulo.vl_saldo_titulo +
                                    decimal.Round((decimal)vl_parcela_material * ((titulo.percentualResp) / 100), 2);
                                newTitulo.vl_material_titulo =
                                    decimal.Round((decimal)vl_parcela_material * ((titulo.percentualResp) / 100), 2);

                            }
                        }
                    titulos.Add(newTitulo);
                }
                else
                {
                    if (titulo.dc_num_documento_titulo != null && sequenciaCheque.HasValue)
                    {
                        //incrementa a parte numerica
                        int proximoCheque = sequenciaCheque.Value + i;
                        int qtd_numeros = titulo.dc_num_documento_titulo.Length - stringCheque.Length;
                        if (qtd_numeros > 0)
                            titulo.dc_num_documento_titulo =
                                stringCheque + proximoCheque.ToString().PadLeft(qtd_numeros, '0');
                        else
                            titulo.dc_num_documento_titulo = stringCheque + proximoCheque.ToString();

                    }

                    newTitulo.nm_parcela_titulo = (byte)(i + 1);
                    newTitulo.nomeResponsavel = titulo.nomeResponsavel;
                    newTitulo.tipoDoc = titulo.tipoDoc;
                    if (titulosDescontoParcela != null && titulosDescontoParcela.Count() > 0)
                        newTitulo = Contrato.TituloDescontoParcela.buscarSetarValorParcelaDescontoTitulo(newTitulo,
                            titulosDescontoParcela);
                    newTitulo.vl_titulo = (newTitulo.vl_titulo * titulo.percentualResp) / 100;
                    newTitulo.vl_saldo_titulo = newTitulo.vl_titulo;
                    int contMes = i;
                    if (calcularProximaData && countParcelasFechadasMensRespo > 1)
                        contMes = i - (countParcelasFechadasMensRespo - 1);
                    int mesProximo = (newTitulo.dt_vcto_titulo.Month + contMes) > 12
                        ? mesProximoAno = (newTitulo.dt_vcto_titulo.Month + contMes) - 12
                        : newTitulo.dt_vcto_titulo.Month + contMes;

                    int anoProximoVenc = dtaVencTituloAnterior.Month == 12
                        ? dtaVencTituloAnterior.Year + 1
                        : dtaVencTituloAnterior.Year;
                    int diaVencimento = diaSugerido > 0 ? diaSugerido : newTitulo.dt_vcto_titulo.Day;
                    DateTime dtaVencProx = new DateTime(anoProximoVenc, mesProximo, diaVencimento);
                    if (dtaVencProx <= minDate)
                        throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior, null,
                            CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR, false);
                    if (dtaVencProx >= maxDate)
                        throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                            CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR, false);
                    newTitulo.dt_vcto_titulo = dtaVencProx;
                    DateTime dataOpcao = newTitulo.dt_vcto_titulo;

                    int diaProximo = nmDia != null ? (int)nmDia : newTitulo.dt_vcto_titulo.Day;
                    if (nmDia.HasValue && nmDia.Value > 0)
                        // A tabela foi colocada com um default de 0 para esse campo:
                        if (diaUtil.HasValue)
                            //Não considera dias úteis ou feriados:
                            if (diaUtil.Value)
                            {
                                diaProximo = 1;
                                dataOpcao = new DateTime(anoProximoVenc, mesProximo, diaProximo);
                                //Calcula o próximo dia útil:
                                for (int j = 0; j < nmDia; j++)
                                {
                                    pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa,
                                        ref feriadosEscola);

                                    if (j != nmDia - 1)
                                        dataOpcao = dataOpcao.AddDays(1);
                                }

                                if (dataOpcao.Month > newTitulo.dt_vcto_titulo.Month)
                                {
                                    int difMes = newTitulo.dt_vcto_titulo.Month - dataOpcao.Month;
                                    if (dataOpcao.Year != newTitulo.dt_vcto_titulo.Year)
                                        difMes -= 12;
                                    //Se pulou + de um mês, retorna a data para o mês seguinte a data do vencimento
                                    if (difMes > 1)
                                        dataOpcao = dataOpcao.AddMonths(-(difMes - 1));
                                    dataOpcao = dataOpcao.AddDays(-dataOpcao.Day);
                                    pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola,
                                        false);
                                }
                            }

                    if (id_alterar_venc_final_semana)
                    {
                        pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola);
                        DateTime dataOpcaoAux = dataOpcao;
                        if (dataOpcao.Month > newTitulo.dt_vcto_titulo.Month)
                        {
                            dataOpcao = dataOpcaoAux;
                            pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola, false);
                        }
                    }

                    if (dataOpcao.Date <= minDate)
                        throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior, null,
                            CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR, false);
                    if (dataOpcao.Date >= maxDate)
                        throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                            CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR, false);

                    newTitulo.dt_vcto_titulo = dataOpcao.Date;
                    dtaVencTituloAnterior = newTitulo.dt_vcto_titulo;
                    newTitulo.id = idNew + 1;
                    idNew = idNew + 1;
                    if (newTitulo.dc_tipo_titulo == "ME" || newTitulo.dc_tipo_titulo == "MA" || newTitulo.dc_tipo_titulo == "MM")
                        if (
                            (bool)id_incorporar_material &&
                            nm_parcelas_material > 0 &&
                            vl_parcela_material > 0)
                        {
                            if (i + 1 <= nm_parcelas_material)
                            {
                                newTitulo.vl_titulo =
                                    newTitulo.vl_titulo +
                                    decimal.Round((decimal)vl_parcela_material * ((titulo.percentualResp) / 100), 2);
                                newTitulo.vl_saldo_titulo =
                                    newTitulo.vl_saldo_titulo +
                                    decimal.Round((decimal)vl_parcela_material * ((titulo.percentualResp) / 100), 2);
                                newTitulo.vl_material_titulo =
                                    decimal.Round((decimal)vl_parcela_material * ((titulo.percentualResp) / 100), 2);
                            }
                        }
                    titulos.Add(newTitulo);
                }

            }
        }

        public List<Titulo> gerarTitulosAditamento(Contrato contrato, Parametro parametros)
        {
            List<Titulo> novosTitulos = new List<Titulo>();
            Titulo tituloViewAdt = contrato.titulos.FirstOrDefault();
            //Ultimo aditamento (atual)
            Aditamento adt = contrato.aditamentoMaxData;
            int[] statusCnabTitulo = new int[] { (int)Titulo.StatusCnabTitulo.INICIAL, (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA };
            List<Titulo> titulosMensalidadeAbertos = new List<Titulo>();
            bool contratoAjusteManualBD =
                matriculaBusiness.getAjusteManualMatricula(tituloViewAdt.cd_origem_titulo.Value,
                    tituloViewAdt.cd_pessoa_empresa);
            decimal valorFaturar = contrato.vl_liquido_contrato;
            switch (adt.id_tipo_aditamento)
            {
                case (int)Aditamento.TipoAditamento.CONCESSAO_DESCONTO:
                case (int)Aditamento.TipoAditamento.MAIORIDADE:
                case (int)Aditamento.TipoAditamento.PERDA_DESCONTO:
                case (int)Aditamento.TipoAditamento.TRANSFERENCIA_TURMA:
                    novosTitulos = calcularAditamentoDiferenteAdicionarParcelas(statusCnabTitulo, tituloViewAdt,
                        contrato, adt, parametros, contratoAjusteManualBD);
                    break;
                case (int)Aditamento.TipoAditamento.ADICIONAR_PARCELAS:
                    novosTitulos = calcularAditamentoAdicionarParcelas(statusCnabTitulo, tituloViewAdt, valorFaturar,
                        contrato, adt, parametros, contratoAjusteManualBD);
                    break;
                case (int)Aditamento.TipoAditamento.ADITIVO_BOLSA:
                    novosTitulos = calcularAditamentoBolsa(statusCnabTitulo, tituloViewAdt, contrato, adt, parametros,
                        contratoAjusteManualBD);
                    break;
            }

            return novosTitulos;
        }

        private List<Titulo> calcularAditamentoDiferenteAdicionarParcelas(int[] statusCnabTitulo, Titulo tituloViewAdt,
            Contrato contrato, Aditamento adt,
            Parametro parametros, bool contratoAjusteManualBD)
        {
            int countTitulosAdicionarParc = 0;
            bool refez_titulo = false;
            bool novoAditamento = false;
            LocalMovtoUI local =
                financeiroBusiness.getLocalByTitulo(tituloViewAdt.cd_pessoa_empresa, tituloViewAdt.cd_local_movto);
            List<Titulo> novosTitulos = new List<Titulo>();
            List<Titulo> titulosMensalidadeAbertos = new List<Titulo>();
            List<Titulo> titulosMensalidades = new List<Titulo>();
            List<Titulo> titulosContratoContext = financeiroBusiness
                .getTitulosByContratoLeitura(tituloViewAdt.cd_origem_titulo.Value, tituloViewAdt.cd_pessoa_empresa)
                .ToList();
            int ultimo_nm_parcela_titulo = 0;
            if (titulosContratoContext.Count() > 0)
            {
                titulosMensalidades = Extensions.cloneList(titulosContratoContext.Where(x =>
                    x.id_origem_titulo == (int)Titulo.OrigemTitulo.CONTRATO &&
                    x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM").ToList()).ToList();
                if (titulosMensalidades.Count() > 0)
                    ultimo_nm_parcela_titulo = (int)titulosMensalidades.OrderBy(x => x.nm_parcela_titulo).Last().nm_parcela_titulo; //LBM antes era cd_titulo
            }
            bool existe_titulo_adt = false;

            //Correção no saldo do título para que seja refeito o título com baixa do tipo "Motivo - Bolsa"
            titulosContratoContext = Titulo.revisarSaldoTituloBolsaContrato(titulosContratoContext, false, 0, false);
            List<Titulo> titulosComMaterial = titulosContratoContext.Where(x =>
                x.vl_material_titulo > 0 &&
                (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM") &&
                x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO && statusCnabTitulo.Contains(x.id_status_cnab) &&
                x.vl_titulo == x.vl_saldo_titulo).ToList();
            Titulo.revisarValorESaldoTituloContratoMaterial(ref titulosContratoContext, statusCnabTitulo, null, false);
            List<Titulo> titulosAdt = titulosContratoContext
                .Where(x => x.dc_tipo_titulo == "AD" || x.dc_tipo_titulo == "AA").ToList();
            Aditamento aditamentoContext =
                matriculaBusiness.getAditamentoByContratoMaxData(tituloViewAdt.cd_origem_titulo.Value,
                    tituloViewAdt.cd_pessoa_empresa);
            if (aditamentoContext != null && aditamentoContext.dt_inicio_aditamento != null &&
                //aditamentoContext.dt_inicio_aditamento.Value.Date == adt.dt_inicio_aditamento.Value.Date)
                adt.cd_aditamento > 0 &&
                aditamentoContext.cd_aditamento == adt.cd_aditamento)
            {
                //Na alteração do atual aditamento, caso o aditivo anterior tenha sido do tipo "adicinar parcela",precisamos do valor adicionado anteriormente para os novos cálculos.
                if (aditamentoContext.id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                {
                    Aditamento penultimoAditamentoContext = matriculaBusiness.getPenultimoAditamentoByContrato(
                        tituloViewAdt.cd_origem_titulo.Value,
                        tituloViewAdt.cd_pessoa_empresa, (DateTime)aditamentoContext.dt_inicio_aditamento);
                    if (penultimoAditamentoContext != null && penultimoAditamentoContext.dt_inicio_aditamento != null
                        /*&& penultimoAditamentoContext.dt_inicio_aditamento.Value.Date !=
                            aditamentoContext.dt_inicio_aditamento.Value.Date*/)
                    {
                        titulosMensalidadeAbertos = Extensions.cloneList(titulosContratoContext.Where(x =>
                            x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                            statusCnabTitulo.Contains(x.id_status_cnab) &&
                            x.vl_titulo == x.vl_saldo_titulo &&
                            x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                            x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA").ToList()).ToList();

                        if (contratoAjusteManualBD && !contrato.id_ajuste_manual)
                        {
                            titulosMensalidadeAbertos = refazerTitulosAnterioresAditamento(contrato, titulosContratoContext,
                                statusCnabTitulo, tituloViewAdt, local, parametros, null, false);
                            refez_titulo = true;
                        }
                        //if (titulosAdt != null)
                        //{
                        //    titulosAdt = titulosAdt.OrderByDescending(x => x.nm_parcela_titulo).ToList();
                        //    for (int i = aditamentoContext.nm_titulos_aditamento - 1; i >= 0; i--)
                        //        titulosAdt.Remove(titulosAdt[i]);
                        //}

                        titulosMensalidadeAbertos = titulosMensalidadeAbertos.Union(titulosAdt.ToList()).ToList();
                    }
                }
                else
                {
                    //Titulo.revisarValorESaldoTituloContratoMaterial(ref titulosContratoContext, statusCnabTitulo, titulosComMaterial, true);
                    if (contratoAjusteManualBD && !contrato.id_ajuste_manual)
                    {
                        titulosMensalidadeAbertos = refazerTitulosAnterioresAditamento(contrato, titulosContratoContext,
                            statusCnabTitulo, tituloViewAdt, local, parametros, null, false);
                        refez_titulo = true;
                    }
                    else
                        titulosMensalidadeAbertos = Extensions.cloneList(titulosContratoContext.Where(x =>
                            x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                            statusCnabTitulo.Contains(x.id_status_cnab) &&
                            x.vl_titulo == x.vl_saldo_titulo &&
                            x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                            x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA").ToList()).ToList();
                }
            }
            else
            {
                novoAditamento = true;
                //Titulo.revisarValorESaldoTituloContratoMaterial(ref titulosContratoContext, statusCnabTitulo, titulosComMaterial, true);
                if (contratoAjusteManualBD && !contrato.id_ajuste_manual)
                {
                    titulosMensalidadeAbertos = refazerTitulosAnterioresAditamento(contrato, titulosContratoContext,
                        statusCnabTitulo, tituloViewAdt, local, parametros, null, false);
                    refez_titulo = true;
                }
                else
                    titulosMensalidadeAbertos = Extensions.cloneList(titulosContratoContext.Where(x =>
                        x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                        statusCnabTitulo.Contains(x.id_status_cnab) &&
                        x.vl_titulo == x.vl_saldo_titulo &&
                        x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                        x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA").ToList()).ToList();
            }

            var titulosAjusteManual = 0;
            if (titulosAdt != null && titulosAdt.Count > 0)
            {
                countTitulosAdicionarParc = titulosAdt.Count();
                titulosAjusteManual = titulosContratoContext.Where(x => x.dc_tipo_titulo == "NN").Count(); //LBM era MM mas não existe ajuste manual mais
                titulosMensalidadeAbertos = titulosMensalidadeAbertos.Union(titulosAdt.Where(x =>
                    x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                    statusCnabTitulo.Contains(x.id_status_cnab) && x.vl_titulo == x.vl_saldo_titulo).ToList()).ToList();
            }

            int countParcelasFechadas = titulosContratoContext.Where(x =>
                (x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA") &&
                ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab))).Count();

            decimal totalPago = titulosContratoContext.Where(x =>
                ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab)) &&
                x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA").Sum(x => x.vl_titulo);

            //Caso seja aditivo sobre aditivo, e o tipo seja "Adicionar Parcelas", os valores rateados, incidiram também nesses titulos.
            if (contrato.pc_responsavel_contrato != 100 && contrato.pc_responsavel_contrato > 0)
                contrato.nm_parcelas_mensalidade = contrato.nm_parcelas_mensalidade * 2;
            if (countTitulosAdicionarParc > 0)
                contrato.nm_parcelas_mensalidade += countTitulosAdicionarParc + titulosAjusteManual;

            //Tratamento para não dar divisão por zero.
            if (contrato.nm_parcelas_mensalidade - countParcelasFechadas > 0)
            {
                Decimal valorParcela = adt.vl_aditivo / (contrato.nm_parcelas_mensalidade - countParcelasFechadas);
                valorParcela = decimal.Round(valorParcela, 2);



                if (valorParcela > 0 && titulosMensalidadeAbertos.Count() > 0)
                {
                    if (!refez_titulo)
                    {
                        List<Titulo> titulosCalcular = null;

                        if (adt != null && adt.id_tipo_aditamento != null &&
                           (adt.id_tipo_aditamento != (int)Aditamento.TipoAditamento.MAIORIDADE ||
                            adt.id_tipo_aditamento != (int)Aditamento.TipoAditamento.TRANSFERENCIA_TURMA))
                        {
                            titulosCalcular = titulosMensalidadeAbertos.Where(x => x.dt_vcto_titulo.Date >= adt.dt_vencto_inicial.Value.Date).ToList();

                            if (titulosCalcular != null)
                            {

                                //    titulosAdt = titulosAdt.OrderByDescending(x => x.nm_parcela_titulo).ToList();


                                gerarNovoValorTituloAditamento(titulosCalcular, contrato, tituloViewAdt, parametros,
                                    adt.vl_aditivo, adt.id_ajuste_manual, novoAditamento, adt.vl_aditivo_anterior);

                                for (int i = titulosCalcular.Count - 1; i >= 0; i--)
                                {
                                    Titulo tituloAlterar = titulosMensalidadeAbertos.Where(z =>
                                        z.cd_titulo == titulosCalcular[i].cd_titulo).FirstOrDefault();
                                    int pos = titulosMensalidadeAbertos.IndexOf(tituloAlterar);
                                    titulosMensalidadeAbertos[pos] = titulosCalcular[i];
                                }

                                foreach (var titulo in titulosMensalidadeAbertos)
                                {

                                    if (contrato.pc_responsavel_contrato == 100 || contrato.pc_responsavel_contrato == 0)
                                        titulo.nm_parcela_titulo = 0;
                                    titulo.tituloEdit = true;
                                }

                                //titulosMensalidadeAbertos = titulosMensalidadeAbertos.Union(titulosCalcular).OrderBy(b => b.cd_titulo).ToList();
                            }
                            else
                            {
                                gerarNovoValorTituloAditamento(titulosMensalidadeAbertos, contrato, tituloViewAdt, parametros,
                                    adt.vl_aditivo, adt.id_ajuste_manual, novoAditamento, adt.vl_aditivo_anterior);
                            }

                        }
                        else
                        {
                            gerarNovoValorTituloAditamento(titulosMensalidadeAbertos, contrato, tituloViewAdt, parametros,
                                adt.vl_aditivo, adt.id_ajuste_manual, novoAditamento, adt.vl_aditivo_anterior);
                        }


                    }
                    else
                    {
                        foreach (var titulo in titulosMensalidadeAbertos)
                        {
                            titulo.vl_saldo_titulo = titulo.vl_titulo;
                            if (tituloViewAdt.pc_bolsa > 0 && titulo.dc_tipo_titulo == "ME")  //LBM Ver se vai incluir MM depois
                                titulo.vl_saldo_titulo =
                                    titulo.vl_titulo -
                                    decimal.Round(titulo.vl_titulo * (decimal)tituloViewAdt.pc_bolsa / 100, 2);
                            //t.tituloEdit = true;
                            if (contrato.pc_responsavel_contrato == 100 || contrato.pc_responsavel_contrato == 0)
                                titulo.nm_parcela_titulo = 0;
                            //if (t.dc_tipo_titulo == "AD")
                            titulo.tituloEdit = true;
                        }
                    }

                    decimal totalMensaAberto = titulosMensalidadeAbertos.Sum(x => x.vl_titulo);
                    decimal totalMaterial = 0; //titulosMensalidadeAbertos.Sum(x => x.vl_material_titulo);
                    //decimal totalCalculadoMatriculaAdt = (totalAPagarMens + totalPago);

                    //if (retorna_apenas_vl_aditamento)
                    adt.vl_aditivo = titulosMensalidadeAbertos.Sum(x => x.vl_titulo);

                    //Verifica se existe diferença de centavos e aplica na 1° parcela.
                    if ((totalMensaAberto - totalMaterial) != adt.vl_aditivo)
                    {
                        decimal diferenca = (totalMensaAberto - totalMaterial) - adt.vl_aditivo;
                        if ((diferenca > 0 && diferenca > 1) || (diferenca < 0 && diferenca < -1))
                            throw new MatriculaBusinessException(Messages.msgDiferencaValorCursoValorTotaTitulos, null,
                                MatriculaBusinessException.TipoErro.ERRO_DIFERENCA_TITULOS_CURSO_MAIOR_UMREAL, false);
                        else
                        {
                            Titulo t = titulosMensalidadeAbertos.FirstOrDefault();
                            if (t != null)
                            {
                                t.vl_titulo -= (totalMensaAberto - totalMaterial) - adt.vl_aditivo;
                                t.vl_saldo_titulo = t.vl_titulo;
                                if (tituloViewAdt.pc_bolsa > 0 && t.dc_tipo_titulo == "ME")  //LBM Verificar MM
                                    t.vl_saldo_titulo =
                                        t.vl_titulo -
                                        decimal.Round(
                                            (t.vl_titulo - t.vl_material_titulo) * (decimal)tituloViewAdt.pc_bolsa /
                                            100, 2);
                            }
                        }
                    }
                }
            }

            //Gerando a numerção dos titulos por tipo (Mensalidade / Aditamento(adicionar parcela))
            List<Titulo> titulosAdtFechados = titulosAdt.Where(x =>
                ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab))).ToList();

            List<Titulo> titulosMensalidadeFechados = titulosContratoContext.Where(x =>
                (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM") &&
                ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab))).ToList();
            List<Titulo> titulosTaxa = titulosContratoContext
                .Where(x => x.dc_tipo_titulo == "TM" || x.dc_tipo_titulo == "TA").ToList();

            if (contrato.pc_responsavel_contrato == 100 || contrato.pc_responsavel_contrato == 0)
            {
                calcularNumeroParcelasMensalidades(titulosMensalidadeFechados,
                    titulosMensalidadeAbertos.Where(x =>
                        (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM")).ToList());
                //calcularNumeroParcelasMensalidades(titulosAdtFechados, titulosMensalidadeAbertos.Where(x => x.dc_tipo_titulo == "AD" || x.dc_tipo_titulo == "AA").ToList());

                //Refazer numero da parcela para titulos de aditamento;
                calcularNumeroParcelasAditamento(titulosAdtFechados,
                    titulosMensalidadeAbertos.Where(x => x.dc_tipo_titulo == "AD" || x.dc_tipo_titulo == "AA").ToList(),
                    ultimo_nm_parcela_titulo, true, existe_titulo_adt);
            }

            if (contrato.pc_desconto_bolsa > 0)
                titulosMensalidadeFechados = Titulo.revisarSaldoTituloBolsaContrato(titulosMensalidadeFechados, true,
                    contrato.pc_desconto_bolsa, false);
            if (titulosAdtFechados != null && titulosAdtFechados.Count() > 0)
                titulosMensalidadeFechados = titulosMensalidadeFechados.Union(titulosAdtFechados).ToList();
            if (titulosMensalidadeFechados != null && titulosMensalidadeFechados.Count() > 0)
                foreach (Titulo t in titulosMensalidadeFechados)
                {
                    if (!titulosMensalidadeAbertos.Any(tit => tit.cd_titulo == t.cd_titulo))
                        titulosMensalidadeAbertos.Add(t);
                }

            if (titulosTaxa != null && titulosTaxa.Count() > 0)
                foreach (Titulo t in titulosTaxa)
                    titulosMensalidadeAbertos.Add(t);

            //List<Titulo> titulosCalculaComDataMaiorOuIgualVencimento = null;

            novosTitulos = titulosMensalidadeAbertos.OrderByDescending(x => x.dc_tipo_titulo)
            .ThenBy(x => x.nm_parcela_titulo).OrderBy(x => x.cd_pessoa_responsavel).ToList();

            //Refazendo os id's dos títulos para não dar erro na grade de títulos.
            for (int i = 0; i < novosTitulos.Count; i++)
                novosTitulos[i].id = (i + 1);
            Titulo.revisarValorESaldoTituloContratoMaterial(ref novosTitulos, statusCnabTitulo, titulosComMaterial,
                true);
            return novosTitulos;
        }

        private List<Titulo> calcularAditamentoAdicionarParcelas(int[] statusCnabTitulo, Titulo tituloViewAdt,
            Decimal valorFaturar, Contrato contrato,
            Aditamento adt, Parametro parametros, bool contratoAjusteManualBD)
        {
            bool novoAditamento = false;
            List<Titulo> novosTitulos = new List<Titulo>();
            List<Titulo> titulosMensalidadeAbertos = new List<Titulo>();
            List<Titulo> titulosComMaterial = new List<Titulo>();
            LocalMovtoUI local =
                financeiroBusiness.getLocalByTitulo(tituloViewAdt.cd_pessoa_empresa, tituloViewAdt.cd_local_movto);
            List<Titulo> titulosContratoContext = financeiroBusiness
                .getTitulosByContratoLeitura(tituloViewAdt.cd_origem_titulo.Value, tituloViewAdt.cd_pessoa_empresa)
                .ToList();
            int ultimo_nm_parcela_titulo = 0;
            if (titulosContratoContext.Count() > 0)
                ultimo_nm_parcela_titulo = (int)titulosContratoContext
                .Where(t =>t.id_origem_titulo == (int)Titulo.OrigemTitulo.CONTRATO &&
                        (((t.dc_tipo_titulo == "ME" || t.dc_tipo_titulo == "MA" || t.dc_tipo_titulo == "MM") && t.dc_tipo_titulo != "AD") || t.dc_tipo_titulo == "AD")).OrderBy(x => x.nm_parcela_titulo).Last() //Antes era cd_titulo e incluido dc_tipo_titulo
                .nm_parcela_titulo;
            bool existe_titulo_adt = false;
            var titulosContext = Extensions.cloneList(titulosContratoContext);

            var existeTituloAditamentoBaixado = existeAdtAdicionarParcelaBaixado(titulosContratoContext, tituloViewAdt);
            if (existeTituloAditamentoBaixado)
            {
                return titulosContratoContext;
            }
            else
            {

                titulosContratoContext =
                    Titulo.revisarSaldoTituloBolsaContrato(titulosContratoContext, false, 0, false);
                titulosMensalidadeAbertos = Extensions.cloneList(titulosContratoContext.Where(x =>
                        x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                        statusCnabTitulo.Contains(x.id_status_cnab) &&
                        x.vl_titulo == x.vl_saldo_titulo &&
                        x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" ).ToList()) //LBM tirei && x.dc_tipo_titulo != "MM"
                    .ToList();
                //Aditamento atual
                Aditamento aditamentoContext =
                    matriculaBusiness.getAditamentoByContratoMaxData(tituloViewAdt.cd_origem_titulo.Value,
                        tituloViewAdt.cd_pessoa_empresa);
                if (aditamentoContext != null && aditamentoContext.id_tipo_aditamento > 0)
                {
                    if (aditamentoContext.id_tipo_aditamento.HasValue)
                        valorFaturar = aditamentoContext.vl_aditivo;
                    else
                        valorFaturar = contrato.vl_liquido_contrato;
                    ////Se a data for igual, irá editar o aditamento.
                    //if (aditamentoContext.dt_inicio_aditamento != null &&
                    //    aditamentoContext.dt_inicio_aditamento.Value.Date == adt.dt_inicio_aditamento.Value.Date)

                    //Ultima atualização aditamento, gerar aditamento com mesma data de inicio.
                    if (adt.cd_aditamento > 0)
                    {
                        //Na alteração do atual aditamento, caso o aditivo anterior tenha sido do tipo "adicinar parcela",precisamos do valor adicionado anteriormente para os novos calculos.
                        if (aditamentoContext.id_tipo_aditamento != (int)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                        {
                            // caso tenha um aditamento antes do atual editado, valtaremos os títulos e o valor do aditivo sera usado como o valor a faturar. Se não tiver sera o valor a faturar do contrato.
                            Aditamento penultimoAditamentoContext = matriculaBusiness.getPenultimoAditamentoByContrato(
                                tituloViewAdt.cd_origem_titulo.Value,
                                tituloViewAdt.cd_pessoa_empresa, (DateTime)aditamentoContext.dt_aditamento);
                            if (penultimoAditamentoContext != null)
                                valorFaturar = penultimoAditamentoContext.vl_aditivo;
                            else
                                valorFaturar = titulosMensalidadeAbertos.Sum(x => x.vl_titulo);
                        }
                        else
                        {
                            Aditamento penultimoAditamentoContext = matriculaBusiness.getPenultimoAditamentoByContrato(
                                tituloViewAdt.cd_origem_titulo.Value,
                                tituloViewAdt.cd_pessoa_empresa, (DateTime)aditamentoContext.dt_aditamento);
                            if (penultimoAditamentoContext != null)
                            {
                                valorFaturar = penultimoAditamentoContext.vl_aditivo;
                                //if (penultimoAditamentoContext.dt_inicio_aditamento != null &&
                                //    penultimoAditamentoContext.dt_inicio_aditamento.Value.Date !=
                                //    aditamentoContext.dt_inicio_aditamento.Value.Date)

                                //Atualização: Pode existir aditamentos com mesma data de inicio.
                                if (penultimoAditamentoContext.dt_inicio_aditamento != null)
                                {
                                    novoAditamento = true;
                                    titulosMensalidadeAbertos = Extensions.cloneList(titulosContratoContext.Where(x =>
                                        x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                        statusCnabTitulo.Contains(x.id_status_cnab) &&
                                        x.vl_titulo == x.vl_saldo_titulo &&
                                        x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                                        x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA").ToList()).ToList();
                                        //&& x.dc_tipo_titulo != "MM"  LBM Tirei
                                    List<Titulo> titulosAdt = titulosContratoContext
                                        .Where(x => x.dc_tipo_titulo == "AD" || x.dc_tipo_titulo == "AA").ToList();
                                    if (titulosAdt != null && titulosAdt.Count() > 0)
                                    {
                                        titulosAdt = titulosAdt.OrderByDescending(x => x.nm_parcela_titulo).ToList();
                                        for (int i = aditamentoContext.nm_titulos_aditamento - 1; i >= 0; i--)
                                            titulosAdt.Remove(titulosAdt[i]);

                                        titulosAdt = titulosAdt.Where(x =>
                                            x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                            statusCnabTitulo.Contains(x.id_status_cnab) &&
                                            x.vl_titulo == x.vl_saldo_titulo).ToList();
                                    }

                                    titulosMensalidadeAbertos =
                                        titulosMensalidadeAbertos.Union(titulosAdt.ToList()).ToList();
                                }
                            }
                            else
                                valorFaturar = titulosMensalidadeAbertos.Sum(x => x.vl_titulo);
                        }

                        if (!novoAditamento)
                        {
                            //Refazendo os títulos de mensalidade. Sempre será refeitos os títulos, por causa de não poder saber se os títulos foram mexidos no ajuste manual.
                            List<Titulo> titulosPagos = titulosContratoContext.Where(x =>
                                ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab)) &&
                                x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA").ToList();
                            List<Titulo> titulosTaxa = titulosContratoContext
                                .Where(x => x.dc_tipo_titulo == "TM" || x.dc_tipo_titulo == "TA").ToList();
                            List<Titulo> titulosAdt = titulosContratoContext
                                .Where(x => x.dc_tipo_titulo == "AD" || x.dc_tipo_titulo == "AA").ToList();
                            if (titulosAdt != null)
                            {
                                titulosAdt = titulosAdt.OrderByDescending(x => x.nm_parcela_titulo).ToList();
                                for (int i = aditamentoContext.nm_titulos_aditamento - 1; i >= 0; i--)
                                    titulosAdt.Remove(titulosAdt[i]);

                            }

                            //Correção no saldo do título para que seja refeito o título com baixa do tipo "Motivo - Bolsa"
                            titulosContratoContext =
                                Titulo.revisarSaldoTituloBolsaContrato(titulosContratoContext, false, 0, false);
                            titulosComMaterial = titulosContratoContext.Where(x =>
                                x.vl_material_titulo > 0 &&
                                (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM") &&
                                x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                statusCnabTitulo.Contains(x.id_status_cnab) &&
                                x.vl_titulo == x.vl_saldo_titulo).ToList();
                            Titulo.revisarValorESaldoTituloContratoMaterial(ref titulosContratoContext,
                                statusCnabTitulo, null, false);
                            if (contratoAjusteManualBD && !contrato.id_ajuste_manual)
                                titulosMensalidadeAbertos = refazerTitulosAnterioresAditamento(contrato,
                                    titulosContratoContext, statusCnabTitulo,
                                    tituloViewAdt, local, parametros, null, true);
                            else
                                titulosMensalidadeAbertos = Extensions.cloneList(titulosContratoContext.Where(x =>
                                    x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                    statusCnabTitulo.Contains(x.id_status_cnab) &&
                                    x.vl_titulo == x.vl_saldo_titulo &&
                                    x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                                    x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA").ToList()).ToList();

                            foreach (var tit in titulosMensalidadeAbertos)
                                tit.nm_parcela_titulo = 0;

                            calcularNumeroParcelasMensalidades(titulosPagos, titulosMensalidadeAbertos);
                            Titulo.revisarValorESaldoTituloContratoMaterial(ref titulosMensalidadeAbertos,
                                statusCnabTitulo, titulosComMaterial, true);
                            titulosContratoContext = titulosMensalidadeAbertos.Union(titulosPagos).ToList();
                            titulosContratoContext = titulosContratoContext.Union(titulosTaxa).ToList();
                            titulosMensalidadeAbertos = titulosMensalidadeAbertos.Union(titulosAdt.ToList()).ToList();

                        }
                    }
                    else
                    {
                        //Quando for aditivo sobre aditivo com adicionar parcelas, não irá recalcular os títulos.
                        novoAditamento = true;
                        if (contratoAjusteManualBD && !contrato.id_ajuste_manual)
                            titulosMensalidadeAbertos = refazerTitulosAnterioresAditamento(contrato,
                                titulosContratoContext, statusCnabTitulo,
                                tituloViewAdt, local, parametros, null, true);
                        else
                            titulosMensalidadeAbertos = Extensions.cloneList(titulosContratoContext.Where(x =>
                                x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                statusCnabTitulo.Contains(x.id_status_cnab) &&
                                x.vl_titulo == x.vl_saldo_titulo &&
                                x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA").ToList()).ToList();
                    }

                }
                else
                    novoAditamento = true;

                decimal totalPago = titulosContratoContext.Where(x =>
                    ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab)) &&
                    x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA").Sum(x => x.vl_titulo);
                //Caso seja aditivo sobre aditivo, e o aditivo atual seja do tipo "adicionar parcela", deveremos usar o valor para os calcumos seguintes.
                Decimal totalAberto = titulosMensalidadeAbertos.Sum(x => x.vl_titulo - x.vl_material_titulo);

                //Caso o usuário depois de baixar algum título ele volte para alterar o aditivo feito. Não podemos considerar mais o valor do aditivo anterior.
                if (totalAberto != valorFaturar)
                    valorFaturar = totalAberto;
                adt.vl_aditivo = valorFaturar + (adt.nm_titulos_aditamento * adt.vl_parcela_titulo_aditamento);

                Decimal totalAdt = ((adt.vl_aditivo - valorFaturar) / adt.nm_titulos_aditamento);
                if (totalAdt < 0)
                    throw new MatriculaBusinessException(Messages.msgErroValorAditivoMenorSaldo, null,
                        MatriculaBusinessException.TipoErro.ERRO_CALCULOS_ADITAMENTO, false);
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                List<Titulo> todosTitulos = new List<Titulo>();
                tituloViewAdt.descLocalMovto = local.nomeLocal;
                tituloViewAdt.diaSugerido = 0;
                //Alteração refente ao chamado 714 - Chamado: 265215 -> (Aditamento/retirar ToLocalTime()-> data voltando 1 dia): tituloViewAdt.dt_vcto_titulo = ((DateTime)adt.dt_vcto_aditamento).ToLocalTime().Date;
                tituloViewAdt.dt_vcto_titulo = ((DateTime)adt.dt_vcto_aditamento).Date;
                tituloViewAdt.dt_emissao_titulo = tituloViewAdt.dt_emissao_titulo.ToLocalTime().Date;
                tituloViewAdt.nm_parcela_titulo = adt.nm_titulos_aditamento;
                tituloViewAdt.cd_tipo_financeiro = adt.cd_tipo_financeiro;
                tituloViewAdt.tipoDoc = adt.tipoDoc;
                tituloViewAdt.id_origem_titulo = cd_origem;
                tituloViewAdt.dc_tipo_titulo = "AD";
                int idNew = 0;
                if (novosTitulos.Count() > 0)
                    idNew = novosTitulos[novosTitulos.Count - 1].id;
                tituloViewAdt.vl_titulo = decimal.Round(totalAdt, 2);
                tituloViewAdt.vl_saldo_titulo = tituloViewAdt.vl_titulo;
                novosTitulos.AddRange(criaTitulos(tituloViewAdt, parametros.id_dia_util_vencimento,
                    parametros.nm_dia_vencimento, idNew, parametros.id_alterar_venc_final_semana, false, 0, null, 0, 0, 0, false, 0, null));
                //Caso seja aditivo sobre aditivo, e o aditivo atual seja do tipo "adicionar parcela", como usamos nos calculos da parcela, devermos retirar o valor do total, para o saldo atual bater, pois
                // já perdemos o valor do aditivo anterior.
                Decimal totalTitulosAdc = novosTitulos.Sum(x => x.vl_titulo);
                if (totalTitulosAdc != (adt.vl_aditivo - valorFaturar))
                {
                    decimal diferenca = totalTitulosAdc - (adt.vl_aditivo - valorFaturar);
                    if ((diferenca > 0 && diferenca > 1) || (diferenca < 0 && diferenca < -1))
                        throw new MatriculaBusinessException(Messages.msgDiferencaValorCursoValorTotaTitulos, null,
                            MatriculaBusinessException.TipoErro.ERRO_DIFERENCA_TITULOS_CURSO_MAIOR_UMREAL, false);
                    else
                    {
                        Titulo t = novosTitulos.FirstOrDefault();
                        if (t != null)
                        {
                            t.vl_titulo -= diferenca;
                            t.vl_saldo_titulo = t.vl_titulo;
                        }

                    }

                    totalTitulosAdc = novosTitulos.Sum(x => x.vl_titulo);
                }

                Decimal totalTitulosContrato = titulosMensalidadeAbertos.Sum(x => x.vl_titulo - x.vl_material_titulo);
                Decimal totalCalculadoMatriculaAdt = totalTitulosContrato + totalTitulosAdc + totalPago;
                if (novoAditamento)
                {
                    List<Titulo> titulosTaxa = titulosContratoContext
                        .Where(x => x.dc_tipo_titulo == "TM" || x.dc_tipo_titulo == "TA").ToList();
                    List<Titulo> titulosPagos = titulosContratoContext.Where(x =>
                        ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab)) &&
                        x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA").ToList();
                    titulosContratoContext = titulosMensalidadeAbertos;
                    titulosContratoContext = titulosContratoContext.Union(titulosTaxa).ToList();
                    titulosContratoContext = titulosContratoContext.Union(titulosPagos).ToList();

                }

                for (int i = 0; i < novosTitulos.Count; i++)
                    novosTitulos[i].nm_parcela_titulo = 0;

                // Se já existir no banco de dados titulos de AD ou AA, inserir número do último titulo de ME. 
                // Para refazer números dos titulos de AD apartir do ultimo valor de ME.
                if (titulosContext.Any(t =>
                    t.id_origem_titulo == (int)Titulo.OrigemTitulo.CONTRATO && t.dc_tipo_titulo.Equals("AD") ||
                    t.dc_tipo_titulo.Equals("AA")))
                {
                    existe_titulo_adt = true;
                    if (titulosContext.Any(t =>
                        t.id_origem_titulo == (int)Titulo.OrigemTitulo.CONTRATO && (t.dc_tipo_titulo == "ME" || t.dc_tipo_titulo == "MA" || t.dc_tipo_titulo == "MM")))
                        ultimo_nm_parcela_titulo = (int)titulosContext.Where(t =>
                            t.id_origem_titulo == (int)Titulo.OrigemTitulo.CONTRATO &&
                            (t.dc_tipo_titulo == "ME" || t.dc_tipo_titulo == "MA" || t.dc_tipo_titulo == "MM"))
                        .OrderBy(x => x.nm_parcela_titulo).Last().nm_parcela_titulo;  //LBM antes era cd_titulo
                    else
                        ultimo_nm_parcela_titulo = 0;
                }

                //calculando a númeração dos títulos de aditamento.
                novosTitulos = calcularNumeroParcelasAditamento(
                    titulosContratoContext.Where(x => x.dc_tipo_titulo == "AD" || x.dc_tipo_titulo == "AA").ToList(),
                    novosTitulos, ultimo_nm_parcela_titulo, true, existe_titulo_adt);

                aplicarDescontosAditamento(novosTitulos, contrato, tituloViewAdt, parametros, adt.vl_aditivo,
                    adt.id_ajuste_manual, novoAditamento, adt.vl_aditivo_anterior);

                //Verificando a diferença, para abater, caso haja.
                if (totalCalculadoMatriculaAdt != (adt.vl_aditivo + totalPago))
                {
                    decimal diferenca = totalCalculadoMatriculaAdt - adt.vl_aditivo;
                    if ((diferenca > 0 && diferenca > 1) || (diferenca < 0 && diferenca < -1))
                        throw new MatriculaBusinessException(Messages.msgDiferencaValorCursoValorTotaTitulos, null,
                            MatriculaBusinessException.TipoErro.ERRO_DIFERENCA_TITULOS_CURSO_MAIOR_UMREAL, false);
                    else
                    {
                        Titulo t = titulosMensalidadeAbertos.FirstOrDefault();
                        Titulo tContext = titulosContratoContext.Where(x =>
                                x.dc_tipo_titulo == t.dc_tipo_titulo && x.nm_parcela_titulo == t.nm_parcela_titulo)
                            .FirstOrDefault();
                        if (tContext != null)
                        {
                            tContext.vl_titulo -= totalCalculadoMatriculaAdt - adt.vl_aditivo;
                            tContext.vl_saldo_titulo = tContext.vl_titulo;
                        }

                    }
                }

                //Se for aditivo sobre aditivo ou aditivo novo, usaremos todos títulos da matrícula, mais os novos criados. caso alteração do atual, perderemos os títulos anteriors, caso o
                // aditamento anterior seja do mesmo tipo.
                if (novoAditamento)
                {
                    //todosTitulos = titulosContratoContext;
                    todosTitulos = titulosContratoContext.OrderByDescending(x => x.dc_tipo_titulo)
                        .ThenBy(x => x.nm_parcela_titulo).ToList();
                }
                else
                {
                    todosTitulos = titulosContratoContext.Where(x =>
                        x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA" ).ToList(); //LBM tirei && x.dc_tipo_titulo != "MM"
                    todosTitulos = todosTitulos.OrderByDescending(x => x.dc_tipo_titulo)
                        .ThenBy(x => x.nm_parcela_titulo).ToList();
                }

                if (contrato.pc_desconto_bolsa > 0)
                    todosTitulos =
                        Titulo.revisarSaldoTituloBolsaContrato(todosTitulos, true, contrato.pc_desconto_bolsa, false);

                if (todosTitulos != null && todosTitulos.Count() > 0)
                {
                    foreach (Titulo t in novosTitulos)
                        todosTitulos.Add(t);
                    novosTitulos = todosTitulos;
                }

                //ATUALIZAR TITULOS QUE JA EXISTEM E TEVE SEUS DADOS ZERADOS PELO METODO CRIATITULOS();
                for (int i = 0; i < novosTitulos.Count; i++)
                {
                    novosTitulos[i].id = (i + 1);
                    foreach (var titContext in titulosContext)
                    {
                        if (titContext.nm_parcela_e_titulo == novosTitulos[i].nm_parcela_e_titulo &&
                            titContext.nm_parcela_titulo == novosTitulos[i].nm_parcela_titulo &&
                            titContext.dc_tipo_titulo == novosTitulos[i].dc_tipo_titulo)
                        {
                            novosTitulos[i].cd_titulo = titContext.cd_titulo;
                            novosTitulos[i].possuiBaixa = titContext.possuiBaixa;
                            novosTitulos[i].possuiBaixaBolsa = titContext.possuiBaixaBolsa;
                        }
                    }
                }
            }

            return novosTitulos;
        }

        public bool existeAdtAdicionarParcelaBaixado(List<Titulo> titulos, Titulo tituloViewAdt)
        {
            //if (titulos.Count == 0)
            //    titulos = financeiroBusiness
            //        .getTitulosByContratoLeitura(tituloViewAdt.cd_origem_titulo.Value, tituloViewAdt.cd_pessoa_empresa)
            //        .ToList();

            if (tituloViewAdt.cd_aditamento > 0)
            {
                var titulosAditamento = DataAccessTituloAditamento.ObterTitulosAditamentoPorId(tituloViewAdt.cd_aditamento);
                titulos = titulos.Where(t => titulosAditamento.Any(ti => ti.cd_titulo == t.cd_titulo)).ToList();

                return titulos.Any(t => t.dc_tipo_titulo == "AD" && (t.possuiBaixa == true || t.possuiBaixaBolsa == true));
                //&& t.dt_emissao_titulo == tituloViewAdt.dt_emissao_titulo.Date);
            }
            else
                return false;
        }

        private void aplicarDescontosAditamento(List<Titulo> titulosAbertos, Contrato contrato, Titulo tituloViewAdt,
            Parametro parametro, decimal vl_aditivo_atual,
            bool ajuste_manual_anterior, bool novoAditamento, decimal valorAnteriorCalc)
        {
            List<DescontoContrato> descontosAditamento =
                contrato.aditamentoMaxData.Desconto != null && contrato.aditamentoMaxData.Desconto != null
                    ? contrato.aditamentoMaxData.Desconto.ToList()
                    : new List<DescontoContrato>();
            List<DescontoContrato> descontoAntigos = new List<DescontoContrato>();
            if (contrato.DescontoContrato != null && contrato.DescontoContrato.Count() > 0)
                descontoAntigos = contrato.DescontoContrato.ToList();
            bool aplicarVoltaDoPercentual = true;

            ////Cálculo quando existe descontos anteriores ou para aplicar.
            //if (descontoAntigos.Count() <= 0 && descontosAditamento.Count() <= 0)
            //{
            //    decimal verif_vl = (vl_aditivo_atual * 100 / (decimal)contrato.valorSaldoMatricula) - 100;
            //    decimal vl_perc_aplicado = vl_aditivo_atual * 100 / (decimal)contrato.valorSaldoMatricula;
            //    if (verif_vl > 0)
            //        descontoAntigos.Add(new DescontoContrato { aplicar_percentual_sem_desconto = true, id_desconto_ativo = true, id_incide_baixa = false, pc_desconto_contrato = Math.Abs(vl_perc_aplicado) });
            //    else
            //        descontosAditamento.Add(new DescontoContrato { id_desconto_ativo = true, id_incide_baixa = false, pc_desconto_contrato = Math.Abs(vl_perc_aplicado - 100) });
            //}
            //else aplicarVoltaDoPercentual = true;



            //VOLTAR VALOR DE DESCONTO APENAS PARA ABA DE "TITULOS GERADOS"
            if (!ajuste_manual_anterior && contrato.gerarTitulos)
                Titulo.aplicarDescontosTituloAditamento(contrato, titulosAbertos,
                    parametro.id_somar_descontos_financeiros, descontoAntigos, aplicarVoltaDoPercentual,
                    valorAnteriorCalc);

            if ((ajuste_manual_anterior && novoAditamento) || (!ajuste_manual_anterior))
                Titulo.aplicarDescontosTituloAditamento(contrato, titulosAbertos,
                    parametro.id_somar_descontos_financeiros, descontosAditamento, false, valorAnteriorCalc);

            foreach (var titulo in titulosAbertos)
            {
                titulo.vl_saldo_titulo = titulo.vl_titulo;
                if (tituloViewAdt.pc_bolsa > 0)
                    titulo.vl_saldo_titulo = titulo.vl_titulo -
                                             decimal.Round(titulo.vl_titulo * (decimal)tituloViewAdt.pc_bolsa / 100,
                                                 2);
                //titulo.tituloEdit = true;
            }
        }

        private List<Titulo> calcularAditamentoBolsa(int[] statusCnabTitulo, Titulo tituloViewAdt, Contrato contrato,
            Aditamento adt, Parametro parametros, bool contratoAjusteManualBD)
        {
            LocalMovtoUI local =
                financeiroBusiness.getLocalByTitulo(tituloViewAdt.cd_pessoa_empresa, tituloViewAdt.cd_local_movto);
            List<Titulo> novosTitulos = new List<Titulo>();
            List<Titulo> titulosMensalidadeAbertos = new List<Titulo>();
            List<Titulo> titulosContratoContext = financeiroBusiness
                .getTitulosByContratoLeitura(tituloViewAdt.cd_origem_titulo.Value, tituloViewAdt.cd_pessoa_empresa)
                .ToList();
            int ultimo_nm_parcela_titulo =
                titulosContratoContext.Where(t =>
                    t.id_origem_titulo == (int)Titulo.OrigemTitulo.CONTRATO &&
                    t.dc_tipo_titulo == "ME" || t.dc_tipo_titulo == "MA" || t.dc_tipo_titulo == "MM").Any() ?
                    (int)titulosContratoContext.Where(t =>
                    t.id_origem_titulo == (int)Titulo.OrigemTitulo.CONTRATO &&
                    t.dc_tipo_titulo == "ME" || t.dc_tipo_titulo == "MA" || t.dc_tipo_titulo == "MM")
                .OrderBy(x => x.nm_parcela_titulo).Last().nm_parcela_titulo :
                    (int)titulosContratoContext.Where(t =>
                    t.id_origem_titulo == (int)Titulo.OrigemTitulo.CONTRATO &&
                    t.dc_tipo_titulo == "AD")
                .OrderBy(x => x.nm_parcela_titulo).Last().nm_parcela_titulo;
                
                ; //LBM antes era cd_titulo
            bool existe_titulo_adt = false;

            //Correção no saldo do título para que seja refeito o título com baixa do tipo "Motivo - Bolsa"
            titulosContratoContext = Titulo.revisarSaldoTituloBolsaContrato(titulosContratoContext, false, 0, true, adt.AditamentoBolsa.FirstOrDefault());
            List<Titulo> titulosComMaterial = titulosContratoContext.Where(x =>
                x.vl_material_titulo > 0 &&
                (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM") &&
                x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO && statusCnabTitulo.Contains(x.id_status_cnab) &&
                x.vl_titulo == x.vl_saldo_titulo).ToList();
            //Titulo.revisarValorESaldoTituloContratoMaterial(ref titulosContratoContext, statusCnabTitulo, null, false);

            if (contratoAjusteManualBD && !contrato.id_ajuste_manual)
            {
                List<Titulo> titulosAdt = titulosContratoContext
                    .Where(x => x.dc_tipo_titulo == "AD" || x.dc_tipo_titulo == "AA").ToList();
                List<Titulo> titulosAdtFechados = titulosAdt.Where(x =>
                    ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab))).ToList();
                List<Titulo> titulosMensalidadeFechados = titulosContratoContext.Where(x =>
                    (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM") &&
                    ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab))).ToList();
                List<Titulo> titulosTaxa = titulosContratoContext
                    .Where(x => x.dc_tipo_titulo == "TM" || x.dc_tipo_titulo == "TA").ToList();
                titulosMensalidadeAbertos = refazerTitulosAnterioresAditamento(contrato, titulosContratoContext,
                    statusCnabTitulo, tituloViewAdt, local, parametros, null, false);
                if (titulosAdt != null && titulosAdt.Count > 0)
                    titulosMensalidadeAbertos = titulosMensalidadeAbertos.Union(titulosAdt.Where(x =>
                            x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                            statusCnabTitulo.Contains(x.id_status_cnab) && x.vl_titulo == x.vl_saldo_titulo).ToList())
                        .ToList();
                if (contrato.pc_responsavel_contrato == 100 || contrato.pc_responsavel_contrato == 0)
                {
                    calcularNumeroParcelasMensalidades(titulosMensalidadeFechados, titulosMensalidadeAbertos.Where(x =>
                        (x.dc_tipo_titulo == "ME" ||
                         x.dc_tipo_titulo == "MA" ||
                         x.dc_tipo_titulo == "MM")).ToList());
                    calcularNumeroParcelasMensalidades(titulosAdtFechados,
                        titulosMensalidadeAbertos.Where(x => x.dc_tipo_titulo == "AD" || x.dc_tipo_titulo == "AA")
                            .ToList());

                    // Se já existir no banco de dados titulos de AD ou AA, inserir número do último titulo de ME. 
                    // Para refazer números dos titulos de AD apartir do ultimo valor de ME.
                    //if (titulosContratoContext.Any(t => t.id_origem_titulo == (int)Titulo.OrigemTitulo.CONTRATO && t.dc_tipo_titulo.Equals("AD") || t.dc_tipo_titulo.Equals("AA")))
                    //    existe_titulo_adt = true;

                    //Refazer numero da parcela para titulos de aditamento;
                    calcularNumeroParcelasAditamento(titulosAdtFechados,
                        titulosMensalidadeAbertos.Where(x => x.dc_tipo_titulo == "AD" || x.dc_tipo_titulo == "AA")
                            .ToList(), ultimo_nm_parcela_titulo, true, existe_titulo_adt);
                }

                if (titulosAdtFechados != null && titulosAdtFechados.Count() > 0)
                    titulosMensalidadeFechados = titulosMensalidadeFechados.Union(titulosAdtFechados).ToList();
                if (titulosMensalidadeFechados != null && titulosMensalidadeFechados.Count() > 0)
                    foreach (Titulo t in titulosMensalidadeFechados)
                        titulosMensalidadeAbertos.Add(t);

                if (titulosTaxa != null && titulosTaxa.Count() > 0)
                    foreach (Titulo t in titulosTaxa)
                        titulosMensalidadeAbertos.Add(t);

            }
            else
                titulosMensalidadeAbertos = titulosContratoContext;

            double pc_bolsa = adt.AditamentoBolsa != null && adt.AditamentoBolsa.Count() > 0
                ? adt.AditamentoBolsa.FirstOrDefault().pc_bolsa
                : 0;
            if (pc_bolsa >= 0)
                titulosMensalidadeAbertos =
                    Titulo.revisarSaldoTituloBolsaContrato(titulosMensalidadeAbertos, true, pc_bolsa, true, adt.AditamentoBolsa.FirstOrDefault());

            novosTitulos = titulosMensalidadeAbertos.OrderByDescending(x => x.dc_tipo_titulo)
                .ThenBy(x => x.nm_parcela_titulo).OrderBy(x => x.cd_pessoa_responsavel).ToList();
            //Refazendo os id's dos títulos para não dar erro na grade de títulos.
            for (int i = 0; i < novosTitulos.Count; i++)
                novosTitulos[i].id = (i + 1);
            //Titulo.revisarValorESaldoTituloContratoMaterial(ref novosTitulos, statusCnabTitulo, titulosComMaterial, true);

            return novosTitulos;
        }

        private List<Titulo> gerarNovosTitulosAditamento(Titulo tituloViewDefaultAdt, decimal valorParcela,
            decimal valorFaturar, Parametro parametros,
            DateTime dtaVencimento, byte nm_parcelas_contrato, int nm_parcelas_fechadas, bool calcularProximadata,
            DateTime? dt_emissao_titulo)
        {
            List<Titulo> novosTitulos = new List<Titulo>();

            bool? diaUtil = parametros.id_dia_util_vencimento;
            byte? nmDia = parametros.nm_dia_vencimento;
            bool id_alterar_venc_final_semana = parametros.id_alterar_venc_final_semana;

            Titulo tituloDefaultAdt = tituloViewDefaultAdt;
            tituloDefaultAdt.dt_vcto_titulo = dtaVencimento.Date;
            tituloDefaultAdt.dt_emissao_titulo = tituloDefaultAdt.dt_emissao_titulo.ToLocalTime().Date;
            tituloDefaultAdt.nm_parcela_titulo = nm_parcelas_contrato;
            int idNew = 0;
            if (novosTitulos.Count() > 0)
                idNew = novosTitulos[novosTitulos.Count - 1].id;
            tituloDefaultAdt.vl_titulo = decimal.Round(valorParcela, 2);
            tituloDefaultAdt.vl_saldo_titulo = tituloDefaultAdt.vl_titulo;
            novosTitulos.AddRange(criaTitulos(tituloDefaultAdt, diaUtil, nmDia, idNew, id_alterar_venc_final_semana,
                calcularProximadata, nm_parcelas_fechadas, null, valorFaturar, 0, 0, false,0,null));
            if (dt_emissao_titulo.HasValue)
                foreach (Titulo t in novosTitulos)
                    t.dt_emissao_titulo = (DateTime)dt_emissao_titulo;
            return novosTitulos;
        }

        private void calcularNumeroParcelasMensalidades(List<Titulo> titulosFechados, List<Titulo> titulosGerados)
        {
            //Números títulos mensalidades
            int qtdTitulos = 0;
            if (titulosGerados.Count() > 0)
            {
                qtdTitulos = titulosGerados.Count();
                if (titulosFechados != null)
                    qtdTitulos += titulosFechados.Count();

                foreach (Titulo t in titulosGerados)
                {
                    byte nm_parcela = 0;
                    for (int i = 0; i < qtdTitulos; i++)
                    {
                        bool numValido = true;
                        if (titulosFechados != null && titulosFechados.Where(X => X.nm_parcela_titulo == (i + 1)).Any())
                            numValido = false;
                        if (titulosGerados != null && titulosGerados.Where(X => X.nm_parcela_titulo == (i + 1)).Any())
                            numValido = false;
                        if (numValido)
                        {
                            nm_parcela = (byte)(i + 1);
                            break;
                        }
                    }

                    t.nm_parcela_titulo = nm_parcela;
                }
            }

            //return titulosGerados;
            //}
        }

        private List<Titulo> calcularNumeroParcelasAditamento(List<Titulo> aditamentosExistentes,
            List<Titulo> titulosAdt, int ultimo_nm_parcela_titulo, bool zerar_nm_parcela_titulo, bool existe_titulo_adt)
        {
            //Números títulos mensalidades
            int qtdTitulos = 0;
            if (titulosAdt.Count() > 0)
            {
                qtdTitulos = titulosAdt.Count();
                if (aditamentosExistentes != null)
                    qtdTitulos += aditamentosExistentes.Count();
                if (zerar_nm_parcela_titulo)
                    foreach (var adt in titulosAdt)
                        adt.nm_parcela_titulo = 0;

                if (existe_titulo_adt && aditamentosExistentes.Count > 0)
                    ultimo_nm_parcela_titulo =
                        (int)aditamentosExistentes.OrderBy(t => t.cd_titulo).Last().nm_parcela_titulo;

                foreach (Titulo t in titulosAdt)
                {
                    byte nm_parcela = 0;
                    for (int i = 0; i < qtdTitulos; i++)
                    {
                        bool numValido = true;
                        if (aditamentosExistentes != null && aditamentosExistentes
                                .Where(X => X.nm_parcela_titulo == (ultimo_nm_parcela_titulo + 1)).Any())
                        {
                            numValido = false;
                            ultimo_nm_parcela_titulo += 1;
                        }

                        if (titulosAdt != null &&
                            titulosAdt.Where(X => X.nm_parcela_titulo == (ultimo_nm_parcela_titulo + 1)).Any())
                        {
                            numValido = false;
                            ultimo_nm_parcela_titulo += 1;
                        }

                        if (numValido)
                        {
                            nm_parcela = (byte)(ultimo_nm_parcela_titulo + 1);
                            break;
                        }
                    }

                    ultimo_nm_parcela_titulo += 1;
                    t.nm_parcela_titulo = nm_parcela;
                }
            }

            return titulosAdt;
        }

        private List<Titulo> gerarTitulosAditamentoPerelascDiferenteAlunoEResponsavel(
            List<Titulo> titulosContratoContext, int[] statusCnabTitulo, Titulo tituloViewAdt,
            Parametro parametros, byte nm_parcelas_contrato, decimal valorFaturar, DateTime? dt_emissao_titulo,
            DateTime ultimaDtVencimento)
        {
            //Gerar as parcelas quando o responsável tiver os 100% ou quando tiver percentual para o aluno e o responsável, mas não foi pago nehuma parcela.

            bool? diaUtil = parametros.id_dia_util_vencimento;
            byte? nmDia = parametros.nm_dia_vencimento;
            bool id_alterar_venc_final_semana = parametros.id_alterar_venc_final_semana;
            DateTime ultimoDtVencimento = ultimaDtVencimento;
            bool calcularProximaData = false;
            IEnumerable<Feriado> feriadosEscola = null;
            int? sequenciaCheque = null;
            string stringCheque = "";
            if (tituloViewAdt.dc_num_documento_titulo != null)
            {
                int legDoc = tituloViewAdt.dc_num_documento_titulo.Length;
                for (int p = 0; p <= legDoc; p++)
                {
                    sequenciaCheque = isNumber(tituloViewAdt.dc_num_documento_titulo.Substring(p, (legDoc - p)));
                    if (sequenciaCheque.HasValue && sequenciaCheque > 0)
                    {
                        stringCheque = tituloViewAdt.dc_num_documento_titulo.Substring(0, p);
                        break;
                    }
                }
            }

            int countParcelasFechadasMensAluno = titulosContratoContext.Where(x =>
                (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM") &&
                x.cd_pessoa_responsavel == x.cd_pessoa_titulo &&
                ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab))).Count();
            //Gerar os títulos com o percentual do aluno
            Titulo ultimoTitulo = titulosContratoContext.Where(x =>
                    (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM") &&  //LBM Coloquei o MM
                    x.cd_pessoa_responsavel == x.cd_pessoa_titulo &&
                    (x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab))
                .OrderByDescending(x => x.nm_parcela_titulo).FirstOrDefault();
            if (ultimoTitulo != null)
            {
                ultimoDtVencimento = ultimoTitulo.dt_vcto_titulo;
                calcularProximaData = true;
            }

            List<Titulo> novosTitulos = new List<Titulo>();
            List<Titulo> titulosRefeitos = new List<Titulo>();
            Titulo tituloDefaultAdt = tituloViewAdt;
            tituloViewAdt.dc_tipo_titulo = "ME";
            tituloDefaultAdt.dt_vcto_titulo = (DateTime)ultimoDtVencimento.Date;
            tituloDefaultAdt.dt_emissao_titulo = tituloDefaultAdt.dt_emissao_titulo.ToLocalTime().Date;
            tituloDefaultAdt.nm_parcela_titulo = nm_parcelas_contrato;
            int idNew = 0;
            if (novosTitulos.Count() > 0)
                idNew = novosTitulos[novosTitulos.Count - 1].id;
            tituloDefaultAdt.vl_titulo = decimal.Round(valorFaturar, 2);
            tituloDefaultAdt.vl_saldo_titulo = tituloDefaultAdt.vl_titulo;
            criarTitulosAlunoContrato(tituloViewAdt, ref novosTitulos, ref sequenciaCheque, ref stringCheque, ref idNew,
                diaUtil, nmDia, ref feriadosEscola, ref id_alterar_venc_final_semana, calcularProximaData,
                countParcelasFechadasMensAluno, 0, null, 0, 0, false, null);
            titulosRefeitos = novosTitulos;
            novosTitulos = new List<Titulo>();
            //Gerar os títulos com o percentual do responásavel
            idNew = 0;
            ultimoDtVencimento = ultimaDtVencimento;
            calcularProximaData = false;
            int countParcelasFechadasResp = titulosContratoContext.Where(x =>
                (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM") &&
                x.cd_pessoa_responsavel != x.cd_pessoa_titulo &&
                ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab))).Count();
            ultimoTitulo = titulosContratoContext.Where(x => (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM") &&  //LBM coloquei o MM
                                                             x.cd_pessoa_responsavel != x.cd_pessoa_titulo &&
                                                             (x.vl_titulo != x.vl_saldo_titulo) ||
                                                             !statusCnabTitulo.Contains(x.id_status_cnab))
                .OrderByDescending(x => x.nm_parcela_titulo).FirstOrDefault();
            if (ultimoTitulo != null)
            {
                ultimoDtVencimento = ultimoTitulo.dt_vcto_titulo;
                calcularProximaData = true;
            }

            tituloDefaultAdt.dt_vcto_titulo = (DateTime)ultimoDtVencimento.Date;
            criarTitulosResponsavelContrato(tituloViewAdt, ref novosTitulos, ref sequenciaCheque, ref stringCheque,
                ref idNew, diaUtil, nmDia, ref feriadosEscola, ref id_alterar_venc_final_semana, calcularProximaData,
                countParcelasFechadasResp, 0, null, 0, 0, false,null);
            titulosRefeitos = titulosRefeitos.Union(novosTitulos).ToList();
            if (dt_emissao_titulo.HasValue)
                foreach (Titulo t in novosTitulos)
                    t.dt_emissao_titulo = (DateTime)dt_emissao_titulo;
            //Caso seja aditivo sobre aditivo, e o tipo seja "Adicionar Parcelas", os valores rateados, incidiram também nesses titulos.
            //tituloViewAdt.dc_tipo_titulo = "ME";
            return titulosRefeitos;
        }

        private void gerarNovoValorTituloAditamento(List<Titulo> titulosAbertos, Contrato contrato,
            Titulo tituloViewAdt, Parametro parametro, decimal vl_aditivo_atual,
            bool ajuste_manual_anterior, bool novoAditamento, decimal valorAnteriorCalc)
        {
            List<DescontoContrato> descontosAditamento =
                contrato.aditamentoMaxData.Desconto != null && contrato.aditamentoMaxData.Desconto != null
                    ? contrato.aditamentoMaxData.Desconto.ToList()
                    : new List<DescontoContrato>();
            List<DescontoContrato> descontoAntigos = new List<DescontoContrato>();
            if (contrato.DescontoContrato != null && contrato.DescontoContrato.Count() > 0)
                descontoAntigos = contrato.DescontoContrato.ToList();
            bool aplicarVoltaDoPercentual = false;

            //Cálculo quando existe descontos anteriores ou para aplicar.
            if (descontoAntigos.Count() <= 0 && descontosAditamento.Count() <= 0)
            {
                decimal verif_vl = (vl_aditivo_atual * 100 / (decimal)contrato.valorSaldoMatricula) - 100;
                decimal vl_perc_aplicado = vl_aditivo_atual * 100 / (decimal)contrato.valorSaldoMatricula;
                if (verif_vl > 0)
                    descontoAntigos.Add(new DescontoContrato
                    {
                        aplicar_percentual_sem_desconto = true,
                        id_desconto_ativo = true,
                        id_incide_baixa = false,
                        pc_desconto_contrato = Math.Abs(vl_perc_aplicado)
                    });
                else
                    descontosAditamento.Add(new DescontoContrato
                    {
                        id_desconto_ativo = true,
                        id_incide_baixa = false,
                        pc_desconto_contrato = Math.Abs(vl_perc_aplicado - 100)
                    });
            }
            else aplicarVoltaDoPercentual = true;

            if (!ajuste_manual_anterior && (descontosAditamento.Count > 0 && descontoAntigos.Count > 0) ||
                (descontoAntigos.Count > 0 && descontosAditamento.Count == 0) || descontosAditamento.Count > 0)
                Titulo.aplicarDescontosTituloAditamento(contrato, titulosAbertos,
                    parametro.id_somar_descontos_financeiros, descontoAntigos, aplicarVoltaDoPercentual,
                    valorAnteriorCalc);
            if ((ajuste_manual_anterior && novoAditamento) || (!ajuste_manual_anterior))
                Titulo.aplicarDescontosTituloAditamento(contrato, titulosAbertos,
                    parametro.id_somar_descontos_financeiros, descontosAditamento, false, valorAnteriorCalc);

            foreach (var titulo in titulosAbertos)
            {
                titulo.vl_saldo_titulo = titulo.vl_titulo;
                if (tituloViewAdt.pc_bolsa > 0)
                    titulo.vl_saldo_titulo = titulo.vl_titulo -
                                             decimal.Round(titulo.vl_titulo * (decimal)tituloViewAdt.pc_bolsa / 100,
                                                 2);
                //t.tituloEdit = true;
                if (contrato.pc_responsavel_contrato == 100 || contrato.pc_responsavel_contrato == 0)
                    titulo.nm_parcela_titulo = 0;
                //if (t.dc_tipo_titulo == "AD")
                titulo.tituloEdit = true;
            }
        }

        private List<Titulo> refazerTitulosAnterioresAditamento(Contrato contrato, List<Titulo> titulosContratoContext,
            int[] statusCnabTitulo, Titulo tituloViewAdt, LocalMovtoUI local,
            Parametro parametros, decimal? valorFaturar, bool adicionar_parcela)
        {
            tituloViewAdt.descLocalMovto = local.nomeLocal;
            SGFWebContext dbComp = new SGFWebContext();
            tituloViewAdt.id_origem_titulo = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

            //Alteração refente ao chamado 714 - Chamado: 265215 -> (Aditamento/retirar ToLocalTime()-> data voltando 1 dia): DateTime ultimoDtVencimento = tituloViewAdt.dt_vcto_titulo.ToLocalTime().Date;
            DateTime ultimoDtVencimento = tituloViewAdt.dt_vcto_titulo.Date;

            var calcularProximaData = false;
            DateTime? dt_emissao_titulo = null;
            if (!adicionar_parcela && titulosContratoContext != null && titulosContratoContext.Count() > 0)
                dt_emissao_titulo = titulosContratoContext.FirstOrDefault().dt_emissao_titulo;
            //Titulo.revisarValorESaldoTituloContratoMaterial(ref titulosContratoContext, statusCnabTitulo, null, true);
            Decimal totalAnteriorAberto = titulosContratoContext.Where(x =>
                (x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                 statusCnabTitulo.Contains(x.id_status_cnab) &&
                 x.vl_titulo == x.vl_saldo_titulo) &&
                (x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                 x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA" //&& x.dc_tipo_titulo != "MM"
                )).Sum(x => x.vl_titulo);
            decimal totalPago = titulosContratoContext.Where(x =>
                    (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM") &&
                    ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab)))
                .Sum(x => x.vl_titulo);
            int countParcelasFechadasMens = titulosContratoContext.Where(x =>
                (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM") &&
                ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab))).Count();
            int countParcelasFechadasResp = titulosContratoContext.Where(x =>
                (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM") &&
                x.cd_pessoa_responsavel != x.cd_pessoa_titulo &&
                ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab))).Count();
            int countParcelasFechadasMensAluno = titulosContratoContext.Where(x =>
                (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM") &&
                x.cd_pessoa_responsavel == x.cd_pessoa_titulo &&
                ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab))).Count();
            List<Titulo> titulosRefeitos = new List<Titulo>();
            if (valorFaturar != null)
                totalAnteriorAberto = valorFaturar.Value;
            if (totalAnteriorAberto > 0)
            {
                Decimal valorParcela =
                    totalAnteriorAberto / (contrato.nm_parcelas_mensalidade - countParcelasFechadasMens);
                valorParcela = decimal.Round(valorParcela, 2);
                if (contrato.pc_responsavel_contrato == 100 ||
                    (contrato.pc_responsavel_contrato != 100 && contrato.pc_responsavel_contrato > 0 &&
                     countParcelasFechadasResp == 0 && countParcelasFechadasMensAluno == 0))
                {
                    //Gerar as parcelas quando o responsável tiver os 100% ou quando tiver percentual para o aluno e o responsável, mas não foi pago nehuma parcela.
                    Titulo ultimoTitulo = titulosContratoContext.Where(x =>
                        (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || x.dc_tipo_titulo == "MM") &&  //LBM coloquei o MM
                        (x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab)
                    ).OrderByDescending(x => x.nm_parcela_titulo).FirstOrDefault();
                    if (ultimoTitulo != null)
                    {
                        DateTime dateValue;
                        DateTime maxDate = new DateTime(2079, 06, 06);
                        DateTime minDate = new DateTime(1900, 01, 01);
                        int diasMes = tituloViewAdt.diaSugerido > 0
                            ? tituloViewAdt.diaSugerido
                            : tituloViewAdt.dt_vcto_titulo.Day;
                        while (diasMes > 0)
                        {
                            string novaData = diasMes + "/" + ultimoTitulo.dt_vcto_titulo.Month + "/" +
                                              ultimoTitulo.dt_vcto_titulo.Year;
                            if (DateTime.TryParse(novaData, out dateValue))
                            {
                                DateTime dt_venc = new DateTime(ultimoTitulo.dt_vcto_titulo.Year,
                                    ultimoTitulo.dt_vcto_titulo.Month, diasMes);
                                if (dt_venc <= minDate)
                                    throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloSuperior, null,
                                        CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR,
                                        false);
                                if (dt_venc >= maxDate)
                                    throw new CoordenacaoBusinessException(Messages.msgErroDtaVencTituloInf, null,
                                        CoordenacaoBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR,
                                        false);
                                ultimoDtVencimento = dt_venc;
                                diasMes = 0;
                            }
                            else
                                diasMes--;
                        }

                        calcularProximaData = true;
                    }

                    //Caso seja aditivo sobre aditivo, e o tipo seja "Adicionar Parcelas", os valores rateados, incidiram também nesses titulos.
                    tituloViewAdt.dc_tipo_titulo = "ME";
                    titulosRefeitos = gerarNovosTitulosAditamento(tituloViewAdt, valorParcela, totalAnteriorAberto,
                        parametros,
                        (DateTime)ultimoDtVencimento, (byte)contrato.nm_parcelas_mensalidade,
                        countParcelasFechadasMens, calcularProximaData, dt_emissao_titulo);
                }
                else
                    titulosRefeitos = gerarTitulosAditamentoPerelascDiferenteAlunoEResponsavel(titulosContratoContext,
                        statusCnabTitulo, tituloViewAdt, parametros, (byte)contrato.nm_parcelas_mensalidade,
                        valorParcela, dt_emissao_titulo, ultimoDtVencimento);

                if (contrato.pc_responsavel_contrato == 100 || contrato.pc_responsavel_contrato == 0)
                    for (int i = 0; i < titulosRefeitos.Count; i++)
                        titulosRefeitos[i].nm_parcela_titulo = 0;
            }

            return titulosRefeitos;
        }


        public List<Titulo> gerarTitulos(Contrato contrato, bool? diaUtil, byte? nmDia,
            bool id_alterar_venc_final_semana)
        {
            List<Titulo> novosTitulos = new List<Titulo>();
            float pc_bolsa = 0;
            bool taxamultipla = false;
            DateTime dt_emissao_titulo = new DateTime().Date;
            if (contrato.titulos.Any(x => x.pc_bolsa > 0))
                pc_bolsa = (float)(decimal)contrato.titulos.Where(x => x.pc_bolsa > 0).FirstOrDefault().pc_bolsa;
            //string no_responsavel = "";
            if (contrato.titulos.Count() > 0)
            {
                //LocalMovtoUI local = new LocalMovtoUI();

                using (var transaction =
                    TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED)
                )
                {
                    int nm_parcela_inicial = 0;
                    foreach (Titulo t in contrato.titulos)
                    {
                        //local = financeiroBusiness.getLocalByTitulo(
                        //    t.cd_pessoa_empresa,
                        //    t.cd_local_movto);

                        //if (local == null)
                        //    local = new LocalMovtoUI();
                        //t.descLocalMovto = local.nomeLocal;

                        //Alteração refente ao chamado 714 - Chamado: 265215 -> (Aditamento/retirar ToLocalTime()-> data voltando 1 dia): t.dt_vcto_titulo = t.dt_vcto_titulo.ToLocalTime().Date;
                        t.dt_vcto_titulo = t.dt_vcto_titulo.Date;

                        t.dt_emissao_titulo = t.dt_emissao_titulo.ToLocalTime().Date;

                        if (t.dc_tipo_titulo != Titulo.TipoTitulo.TM.ToString() &&
                            t.dc_tipo_titulo != Titulo.TipoTitulo.TA.ToString())
                        {

                            if (!verificaMatriculaMultipla(contrato.id_tipo_contrato))
                            {
                                //Obter valor total do curso quando descontos forem aplicados em parcelas especificas.
                                //Exemplo: Desconto de 30% será aplicado apenas nas parcelas 1 a 3 do tatal de 6 parcelas.
                                if (contrato.titulosDescontoParcela.Any(d =>
                                    (d.nm_parcela_ini_desconto > 0 || d.nm_parcela_fim_desconto > 0) &&
                                    d.pc_total_desconto_aplicado > 0))
                                    contrato.vl_liquido_contrato = contrato.vl_curso_contrato;
                                t.vl_titulo =
                                    decimal.Round(((contrato.vl_liquido_contrato - (t.primeiraParc ? t.vl_divida > 0 ? t.vl_divida : 0 : 0)) / contrato.nm_parcelas_mensalidade), 2);
                                t.vl_saldo_titulo = t.vl_titulo;
                            }
                            else
                            {
                                t.vl_titulo =
                                  decimal.Round((t.CursoContrato.vl_curso_liquido / t.CursoContrato.nm_parcelas_mensalidade), 2);
                                t.vl_saldo_titulo = t.vl_titulo; // t.CursoContrato.vl_parcela_contrato;
                            }

                        }

                        int idNew = 0;
                        if (novosTitulos.Count() > 0)
                        {
                            idNew = novosTitulos[novosTitulos.Count - 1].id;
                        }

                        if (!verificaMatriculaMultipla(contrato.id_tipo_contrato) || (t.dc_tipo_titulo == Titulo.TipoTitulo.TM.ToString() ||
                                                                                  t.dc_tipo_titulo == Titulo.TipoTitulo.TA.ToString()))
                        {
                            novosTitulos.AddRange(criaTitulos(t, diaUtil, nmDia, idNew, id_alterar_venc_final_semana, false,
                                0, contrato.titulosDescontoParcela, contrato.vl_liquido_contrato, contrato.nm_parcelas_material, contrato.vl_parcela_liq_material,
                                contrato.id_incorporar_valor_material, contrato.vl_material_contrato, contrato.titulosTaxaParcela));
                        }
                        else
                        {
                            taxamultipla = true;
                            Decimal totalMaterial = 0;
                            byte? nm_parcelas_material = t.CursoContrato.nm_parcelas_material;
                            decimal? vl_material_contrato = t.CursoContrato.vl_parcela_liq_material;
                            bool? id_incorporar_material = t.CursoContrato.id_incorporar_valor_material;
                            totalMaterial = (decimal)contrato.titulos.Where(ct => ct.CursoContrato != null).Sum(x => x.CursoContrato.vl_material_contrato);  //(decimal)(contrato.titulos.Sum(x => x.CursoContrato.vl_material_contrato));
                            if (totalMaterial == 0)  //Vai dividir o total dos materiais pelo número de cursos
                            {
                                nm_parcelas_material = contrato.nm_parcelas_material;
                                vl_material_contrato = decimal.Round((decimal)(contrato.vl_parcela_liq_material / contrato.titulos.Count()), 2);
                                id_incorporar_material = contrato.id_incorporar_valor_material;
                                totalMaterial = decimal.Round((decimal)(contrato.vl_material_contrato / contrato.titulos.Count()), 2);
                            }
                            else
                                totalMaterial = (decimal)(t.CursoContrato.vl_material_contrato);
                            if (!(bool)id_incorporar_material) totalMaterial = 0;
                            novosTitulos.AddRange(criaTitulosMatriculaMultipla(t, diaUtil, nmDia, idNew, id_alterar_venc_final_semana, false,
                                0, contrato.titulosDescontoParcela, t.CursoContrato.vl_curso_liquido, ref nm_parcela_inicial,
                                nm_parcelas_material, vl_material_contrato, id_incorporar_material, totalMaterial, contrato.titulosTaxaParcela));
                        }

                    }

                    transaction.Complete();
                }

                if (novosTitulos != null && novosTitulos.Count > 0)
                {
                    DateTime dataVencimentoTituloCartao = novosTitulos[0].dt_emissao_titulo;
                    foreach (Titulo t in novosTitulos)
                    {
                        t.vl_saldo_titulo = t.vl_titulo;
                        // Se Tipo financeiro for Cartão Crédito ou Débito, aplicar taxa bancária; Foi movido da linha 6536 para matricula multipla
                        if (t.cd_tipo_financeiro == (int)FundacaoFisk.SGF.GenericModel.TipoFinanceiro.TiposFinanceiro.CARTAO && taxamultipla)
                        {
//                            foreach (var objTitulo in novosTitulos)
//                            {
                                aplicarTaxaBancaria(t, (int)novosTitulos.Count, ref dataVencimentoTituloCartao, contrato.titulosTaxaParcela);
//                            }
                        }
                    }
                    List<Titulo> titulosME = novosTitulos.Where(t => t.dc_tipo_titulo == "ME").ToList();
                    if (titulosME != null && titulosME.Count > 0)
                    {
                        if (pc_bolsa > 0)
                            foreach (Titulo t in titulosME)
                                t.vl_saldo_titulo =
                                    t.vl_titulo - decimal.Round((t.vl_titulo - t.vl_material_titulo)* (decimal)pc_bolsa / 100, 2);
                        //if (contrato.aditamentoMaxData != null && contrato.aditamentoMaxData.cd_nome_contrato > 0 &&
                        //    contrato.aditamentoMaxData.id_tipo_aditamento == null &&
                        //    contrato.aditamentoMaxData.id_incorporar_valor_material &&
                        //    contrato.aditamentoMaxData.nm_parcelas_material > 0 &&
                        //    contrato.aditamentoMaxData.vl_parcela_liq_material > 0){
                        //    for (int i = 0; i < contrato.nm_parcelas_material; i++)
                        //    {
                        //        titulosME[i].vl_titulo =
                        //            titulosME[i].vl_titulo +
                        //            decimal.Round((decimal)contrato.vl_parcela_liq_material, 2);
                        //        titulosME[i].vl_saldo_titulo =
                        //            titulosME[i].vl_saldo_titulo +
                        //            decimal.Round((decimal)contrato.vl_parcela_liq_material, 2);
                        //        titulosME[i].vl_material_titulo =
                        //            decimal.Round((decimal)contrato.vl_parcela_liq_material, 2);
                        //    }

                        //    if ((diferenca > 0 && diferenca <= 1) || (diferenca < 0 && diferenca >= -1))
                        //    {
                        //        titulosME.FirstOrDefault().vl_titulo = titulosME.FirstOrDefault().vl_titulo - diferenca;
                        //        titulosME.FirstOrDefault().vl_saldo_titulo =
                        //            titulosME.FirstOrDefault().vl_saldo_titulo - diferenca;
                        //        titulosME.FirstOrDefault().vl_material_titulo =
                        //            titulosME.FirstOrDefault().vl_material_titulo - diferenca;
                        //    }
                        //}
                    }

                    //Aplicar o abatimnento do valor pargo na pré matrícula prospect.
                    if (contrato.vl_pre_matricula > 0)
                        this.aplicarAbatimentoPreMatriculaProspect(ref novosTitulos, contrato.vl_pre_matricula,
                            pc_bolsa);
                }
            }

            return novosTitulos;
        }

        private bool verificaMatriculaMultipla(int id_tipo_contrato)
        {
            return id_tipo_contrato == (int)Contrato.TipoCKMatricula.MULTIPLA;
        }

        private void aplicarAbatimentoPreMatriculaProspect(ref List<Titulo> novosTitulos, decimal vl_saldo_abatimento,
            float pc_bolsa)
        {
            if (vl_saldo_abatimento > 0 && novosTitulos.Count() > 0)
            {
                List<Titulo> removerLista = new List<Titulo>();
                //Abatendo primeiramente na taxa de matrícula.
                if (novosTitulos.Any(x => x.dc_tipo_titulo == Titulo.convertTipoTitulo((byte)Titulo.TipoTitulo.TM)))
                {
                    foreach (Titulo t in novosTitulos.Where(x =>
                        x.dc_tipo_titulo == Titulo.convertTipoTitulo((byte)Titulo.TipoTitulo.TM)).ToList())
                    {
                        if (t.vl_titulo <= vl_saldo_abatimento)
                        {
                            vl_saldo_abatimento -= t.vl_titulo;
                            removerLista.Add(t);
                        }
                        else
                        {
                            t.vl_saldo_titulo -= vl_saldo_abatimento;
                            t.vl_titulo -= vl_saldo_abatimento;
                            vl_saldo_abatimento = 0;
                            break;
                        }
                    }

                    if (removerLista.Count() > 0)
                        foreach (var t in removerLista)
                            novosTitulos.Remove(t);
                    if (novosTitulos.Any(x =>
                        x.dc_tipo_titulo == Titulo.convertTipoTitulo((byte)Titulo.TipoTitulo.TM)))
                    {
                        foreach (Titulo t in novosTitulos.Where(x =>
                            x.dc_tipo_titulo == Titulo.convertTipoTitulo((byte)Titulo.TipoTitulo.TM)).ToList())
                            t.nm_parcela_titulo = 0;
                        calcularNumeroParcelasMensalidades(new List<Titulo>(),
                            novosTitulos.Where(x =>
                                x.dc_tipo_titulo == Titulo.convertTipoTitulo((byte)Titulo.TipoTitulo.TM)).ToList());
                    }
                }

                removerLista = new List<Titulo>();
                //Abatimento mensalidades
                if (vl_saldo_abatimento > 0)
                {
                    novosTitulos = novosTitulos.OrderBy(x => x.nm_parcela_titulo).ToList();
                    foreach (Titulo t in novosTitulos.Where(x =>
                        x.dc_tipo_titulo != Titulo.convertTipoTitulo((byte)Titulo.TipoTitulo.TM)).ToList())
                    {
                        decimal vl_titulo = t.vl_material_titulo > 0
                            ? (t.vl_saldo_titulo - t.vl_material_titulo)
                            : t.vl_saldo_titulo;
                        if (vl_titulo <= vl_saldo_abatimento)
                        {

                            vl_saldo_abatimento -= vl_titulo;
                            if (t.vl_material_titulo > 0)
                            {
                                t.vl_titulo -= vl_titulo;
                                t.vl_saldo_titulo -= vl_titulo;
                                if (t.vl_saldo_titulo < t.vl_material_titulo)
                                    t.vl_saldo_titulo = t.vl_titulo;
                                t.vl_abatimento = vl_titulo;
                            }
                            else
                                removerLista.Add(t);
                        }
                        else
                        {
                            t.vl_titulo -= vl_saldo_abatimento;
                            t.vl_saldo_titulo -= vl_saldo_abatimento;
                            t.vl_abatimento = vl_saldo_abatimento;
                            vl_saldo_abatimento = 0;
                        }

                        if (vl_saldo_abatimento <= 0)
                            break;
                    }

                    if (removerLista.Count() > 0)
                        for (int i = 0; i < removerLista.Count(); i++)
                            novosTitulos.Remove(novosTitulos[0]);
                    if (novosTitulos.Any(x =>
                        x.dc_tipo_titulo != Titulo.convertTipoTitulo((byte)Titulo.TipoTitulo.TM)))
                        foreach (Titulo t in novosTitulos.Where(x =>
                            x.dc_tipo_titulo != Titulo.convertTipoTitulo((byte)Titulo.TipoTitulo.TM)).ToList())
                            t.nm_parcela_titulo = 0;
                    calcularNumeroParcelasMensalidades(new List<Titulo>(),
                        novosTitulos.Where(x =>
                            x.dc_tipo_titulo != Titulo.convertTipoTitulo((byte)Titulo.TipoTitulo.TM)).ToList());

                    
                }

                //Atualiza o vl_taxa_cartao para titulos do tipo cartao
                novosTitulos.ForEach(x =>
                {
                    if (x.cd_tipo_financeiro == (int) TipoFinanceiro.TiposFinanceiro.CARTAO)
                    {
                        x.vl_taxa_cartao = Math.Round((decimal)x.pc_taxa_cartao * (x.vl_titulo / (decimal)100.0),2, MidpointRounding.AwayFromZero);
                    }
                });
            }
        }

        #endregion

        #region Baixas

        public void simularBaixaContrato(Contrato contrato, ref BaixaTitulo baixa, Parametro parametro)
        {
            Titulo titulo = null;
            List<Aditamento> aditamentos =
                contrato.Aditamento != null ? contrato.Aditamento.ToList() : new List<Aditamento>();
            Aditamento aditamento = (aditamentos != null && aditamentos.Count > 0)
                ? aditamentos.OrderBy(a => a.dt_aditamento).Last()
                : new Aditamento();

            //Caso o contrato tenha somente uma parcela, será feito a simulação de baixa do valor do título.
            if ((contrato != null && contrato.titulos != null && contrato.titulos.Count == 1) // Quando há aditamento
                || (contrato.titulos == null && contrato.nm_parcelas_mensalidade == 1)) //Quando não há aditamento
                titulo = financeiroBusiness.getTituloByContrato(contrato.cd_contrato, 1);
            else if (aditamentos.Count <= 0 || !aditamento.id_tipo_aditamento.HasValue
            ) //Caso tenha mais de uma parcela, se não for de aditamento, aparecera a simulação de baixa do valor da segunda parcela. 
                titulo = financeiroBusiness.getTituloByContrato(contrato.cd_contrato, 2);
            else
            {
                //Se for de Aditamento com tipo diferente de Adicionar Parcelas, vai fazer a simulação de baixa no primeiro titulo em aberto.
                if (aditamento.id_tipo_aditamento.Value != (byte)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                    titulo = financeiroBusiness.getTituloByContrato(contrato.cd_contrato, 1);
                else //Se for do Tipo Adicionar Parcelas, será em cima do primeiro titulo em aberto do aditamento.
                    titulo = financeiroBusiness.getTituloAbertoByAditamento(aditamento.cd_aditamento);
            }

            if (titulo != null)
            {
                byte? nm_dia_vcto_desconto = aditamentos.OrderBy(a => a.dt_aditamento).Last().nm_dia_vcto_desconto;

                if (nm_dia_vcto_desconto.HasValue)
                {
                    //try
                    //{
                    //    baixa.dt_baixa_titulo = new DateTime(titulo.dt_vcto_titulo.Year, titulo.dt_vcto_titulo.Month, dia.Value);
                    //}
                    //catch (System.ArgumentOutOfRangeException e)
                    //{
                    //    throw new MatriculaBusinessException(Messages.msgDiaInvalidoImpressaoContrato, e, MatriculaBusinessException.TipoErro.ERRO_CONFIGURACAO_DIA_IMPRESSAO_CONTRATO, false);
                    //}

                    DateTime data_vcto_desconto = new DateTime();
                    //Caso não exista o dia, por exemplo, dia 31, tenta ainda o dia 30, 29 e 28:
                    bool encontrou_dia = false;
                    for (int k = 0; k < 4 && !encontrou_dia; k++)
                    {
                        try
                        {
                            data_vcto_desconto = new DateTime(titulo.dt_vcto_titulo.Year, titulo.dt_vcto_titulo.Month,
                                (int)nm_dia_vcto_desconto - k);
                            encontrou_dia = true;
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {
                            encontrou_dia = false;
                        }
                    }

                    baixa.dt_baixa_titulo = data_vcto_desconto;
                }
                else
                    throw new MatriculaBusinessException(Messages.msgErroDiaVencDescAditaNaoInformado, null,
                        MatriculaBusinessException.TipoErro.ERRO_DIA_VENC_ADITA_NAO_INFORMADO, false);

                this.calcularBaixaTitulo(contrato, titulo, ref baixa, parametro, false, true);
            }
        }

        public void simularBaixaTitulo(Titulo titulo, ref BaixaTitulo baixa, Parametro parametro, int cd_escola,
            bool contaSegura, bool gerarMensagem)
        {
            //Busca o contrato do titulo:
            SGFWebContext db = new SGFWebContext();
            Contrato contrato = null;
            if (!contaSegura)
                financeiroBusiness.verificarTituloContaSegura(titulo.cd_titulo, Math.Abs(cd_escola));

            if (titulo.vl_saldo_titulo > 0)
            {
                if (titulo.cd_origem_titulo.HasValue && titulo.id_origem_titulo ==
                    Int32.Parse(db.LISTA_ORIGEM_LOGS["Contrato"].ToString()))
                    contrato = matriculaBusiness.getContratoBaixa(cd_escola, titulo.cd_origem_titulo.Value);
                //baixa.dt_baixa_titulo = DateTime.UtcNow;
                List<BaixaTitulo> baixasParcial = financeiroBusiness.getBaixasTransacaoFinan(0, 0, titulo.cd_titulo,
                    cd_escola, BaixaTituloDataAccess.TipoConsultaBaixaEnum.HAS_BAIXAS_PARCIAIS_TITULO).ToList();
                baixasParcial = baixasParcial.Where(x =>
                    x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                    x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO).ToList();
                DateTime dtaBaixa = baixa.dt_baixa_titulo;
                List<BaixaTitulo> baixasParcialDia = baixasParcial.Where(b => b.dt_baixa_titulo == dtaBaixa).ToList();
                if (baixasParcial.Count() > 0)
                {
                    if (baixasParcialDia.Count() > 0)
                    {
                        this.calcularBaixaTitulo(contrato, titulo, ref baixa, parametro, gerarMensagem, false);
                        decimal vl_desconto = baixa.vl_desconto_baixa_calculado;
                        baixa.vl_multa_baixa = baixasParcialDia.Sum(p => p.vl_multa_calculada);
                        baixa.vl_juros_baixa = baixasParcialDia.Sum(p => p.vl_juros_calculado);
                        baixa.vl_liquidacao_baixa = (baixa.vl_principal_baixa + baixa.vl_multa_baixa +
                                                     baixa.vl_juros_baixa - vl_desconto);
                        baixa.vl_juros_calculado = 0;
                        baixa.vl_multa_calculada = 0;
                    }
                    else
                    {
                        var dtVctoOri = titulo.dt_vcto_titulo;
                        bool baixaParcAposVenc = false;
                        DateTime dtaPrimeiraBaixaParcial = baixasParcial.OrderBy(d => d.dt_baixa_titulo)
                            .FirstOrDefault().dt_baixa_titulo;
                        if (dtaPrimeiraBaixaParcial > dtVctoOri)
                        {
                            titulo.dt_vcto_titulo = dtaPrimeiraBaixaParcial;
                            baixaParcAposVenc = true;
                        }

                        this.calcularBaixaTitulo(contrato, titulo, ref baixa, parametro, gerarMensagem, false);
                        decimal multa = baixa.vl_multa_calculada;
                        ////Aplicar o desconto quando não existir juros e multa.
                        decimal vl_desconto = baixa.vl_desconto_baixa_calculado;
                        if (baixaParcAposVenc && baixasParcial.Sum(p => p.vl_multa_calculada) > 0)
                            multa = baixasParcial.Sum(p => p.vl_multa_calculada);
                        if (titulo.dt_vcto_titulo > baixa.dt_baixa_titulo)
                        {
                            baixa.vl_multa_baixa = 0;
                            baixa.vl_juros_baixa = 0;
                        }
                        else
                        {
                            baixa.vl_multa_baixa = multa;
                            baixa.vl_juros_baixa =
                                baixa.vl_juros_calculado + baixasParcial.Sum(p => p.vl_juros_calculado);
                        }

                        //  baixa.vl_juros_calculado = 0;
                        //  baixa.vl_multa_calculada = 0;
                        baixa.vl_liquidacao_baixa = (baixa.vl_principal_baixa + baixa.vl_multa_baixa +
                                                     baixa.vl_juros_baixa - vl_desconto);
                        titulo.dt_vcto_titulo = dtVctoOri;
                    }

                }
                else
                {
                    this.calcularBaixaTitulo(contrato, titulo, ref baixa, parametro, gerarMensagem, false);
                    baixa.vl_multa_baixa = baixa.vl_multa_calculada;
                    baixa.vl_juros_baixa = baixa.vl_juros_calculado;
                }
            }
            else
            {
                List<BaixaTitulo> baixas = financeiroBusiness.getBaixasTransacaoFinan(0, 0, titulo.cd_titulo, cd_escola,
                    BaixaTituloDataAccess.TipoConsultaBaixaEnum.HAS_BAIXAS_PARCIAIS_TITULO).ToList();
                if (baixas != null && baixas.Count > 0)
                    foreach (BaixaTitulo b in baixas)
                    {
                        baixa.vl_multa_baixa += b.vl_multa_calculada - b.vl_multa_baixa;
                        baixa.vl_juros_baixa += b.vl_juros_calculado - b.vl_juros_baixa;
                        baixa.vl_liquidacao_baixa += (b.vl_multa_calculada - b.vl_multa_baixa) +
                                                     (b.vl_juros_calculado - b.vl_juros_baixa);
                        baixa.cd_titulo = b.cd_titulo;
                        titulo.cd_titulo = b.cd_titulo;
                    }

                baixa.cd_titulo = titulo.cd_titulo;
                //Fazer a pesquisa da baixa e somar o juros com a multa no lugar do vl_liquidacao_baixa.
                //Mudar o metodo que fecha o titulo para somar o juros e a multa calculado e comparar com a mesma liquidada.
                //baixa.vl_multa_baixa = baixa.vl_multa_calculada;
                //baixa.vl_juros_baixa = baixa.vl_juros_calculado;
            }

            //Copia os atributos do titulo para a baixa, para mostrar na grade:
            baixa.nm_titulo = titulo.nm_titulo;
            baixa.nm_parcela_titulo = titulo.nm_parcela_titulo;
            baixa.dt_vcto_titulo = titulo.dt_vcto;
            baixa.id_natureza_titulo = titulo.id_natureza_titulo;
            baixa.vl_taxa_cartao = titulo.vl_taxa_cartao;
            baixa.Titulo = titulo;
        }

        //Método usado somente na classe de teste, pois se trata de um método privado.
        public void calcularBaixaTituloTeste(Contrato contrato, Titulo titulo, ref BaixaTitulo baixa,
            Parametro parametro)
        {
            this.calcularBaixaTitulo(contrato, titulo, ref baixa, parametro, false, false);
        }

        /* Método que calcula a baixa do título. Os objetos de entrada:
         * contrato.AlunoTurma > com o relacionamento da turma ativa do contrato.
         * contrato.AlunoTurma[0].cd_turma
         * contrato.cd_aluno
         * baixa.dt_baixa_titulo
         */
        private void calcularBaixaTitulo(Contrato contrato, Titulo titulo, ref BaixaTitulo baixa, Parametro parametro,
            bool gerarMensagem, bool usar_valor_titulo)
        {
            SGFWebContext db = new SGFWebContext();
            IEnumerable<Feriado> feriadosEscola = null;
            baixa.diasPoliticaAntecipacao = new List<BaixaTitulo.DiasPoliticaAntecipacao>();
            int cd_turma = 0;

            try
            {
                //1-Verificar se o titulo é de contrato de matrícula
                if (contrato != null &&
                    titulo.id_origem_titulo == Int32.Parse(db.LISTA_ORIGEM_LOGS["Contrato"].ToString()))
                {
                    if (contrato.AlunoTurma != null)
                    {
                        List<AlunoTurma> listaAlunoTurma = contrato.AlunoTurma.ToList();
                        //Se o contrato tiver turma:
                        if (listaAlunoTurma.Count > 0)
                            cd_turma = listaAlunoTurma[0].cd_turma;
                    }
                }
            }
            catch (System.NullReferenceException exe)
            {
                throw new NullReferenceException("1-Verificar se o titulo é de contrato de matrícula", exe);
            }

            //4-Verificar Descontos do Contrato que incidem na baixa e estão ativos:
            decimal soma_valores_desconto = 0;
            decimal percentual_desconto = 0;
            decimal percentual_politica = 0;
            double percentual_juros = 0;
            double percentual_multa = 0;
            decimal percentual_valor = 0;
            decimal vl_liquido = titulo.vl_saldo_titulo;
            if ((titulo.vl_saldo_titulo - titulo.vl_material_titulo) < 0)
                vl_liquido = 0;
            else
                vl_liquido -= titulo.vl_material_titulo;


            try
            {
                baixa.pc_pontualidade = 0;
                if (contrato != null && contrato.DescontoContrato != null && titulo != null &&
                    titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER
                    && !Titulo.TipoTitulo.TM.ToString().Equals(titulo.dc_tipo_titulo) &&
                    !Titulo.TipoTitulo.TA.ToString().Equals(titulo.dc_tipo_titulo))
                {
                    List<DescontoContrato> descontosContrato = contrato.DescontoContrato.ToList();
                    Aditamento aditamento = new Aditamento();
                    if (contrato.Aditamento != null && contrato.Aditamento.Count() > 0) aditamento = contrato.Aditamento.FirstOrDefault();
                    for (int i = 0; i < descontosContrato.Count; i++)
                    {
                        DescontoContrato descontoContrato = descontosContrato[i];
                        descontoContrato = (DescontoContrato)descontoContrato.Clone();
                        if (descontoContrato.id_desconto_ativo && descontoContrato.id_incide_baixa
                        ) // && (titulo.nm_parcela_titulo == 1))
                        {
                            if ((titulo.dt_vcto_titulo >= ((aditamento == null || aditamento.dt_vencto_inicial == null || !aditamento.id_tipo_aditamento.HasValue || (aditamento.id_tipo_aditamento.Value != (byte)Aditamento.TipoAditamento.CONCESSAO_DESCONTO) && aditamento.id_tipo_aditamento.Value != (byte)Aditamento.TipoAditamento.PERDA_DESCONTO) ? titulo.dt_vcto_titulo : aditamento.dt_vencto_inicial.Value.Date)) &&
                                ((descontoContrato.nm_parcela_ini == 0 && descontoContrato.nm_parcela_fim == 0) ||
                                (descontoContrato.nm_parcela_ini > 0 && descontoContrato.nm_parcela_fim == 0 &&
                                 titulo.nm_parcela_titulo >= descontoContrato.nm_parcela_ini) ||
                                (descontoContrato.nm_parcela_ini == 0 && descontoContrato.nm_parcela_fim > 0 &&
                                 titulo.nm_parcela_titulo <= descontoContrato.nm_parcela_fim) ||
                                (titulo.nm_parcela_titulo >= descontoContrato.nm_parcela_ini &&
                                 titulo.nm_parcela_titulo <= descontoContrato.nm_parcela_fim)))
                            {
                                soma_valores_desconto += descontoContrato.vl_desconto_contrato;
                                baixa.des_desconto = baixa.des_desconto + descontoContrato.valor_desconto;
                                percentual_valor = ((descontoContrato.vl_desconto_contrato == 0 || vl_liquido == 0) ? 0 :
                                        (descontoContrato.vl_desconto_contrato / vl_liquido * 100));
                                percentual_valor = percentual_valor == 0 ? descontoContrato.pc_desconto_contrato : percentual_valor;
                                if (parametro != null && parametro.id_somar_descontos_financeiros)
                                    percentual_desconto += percentual_valor;
                                else
                                    percentual_desconto =
                                        100 - ((1 - percentual_valor / 100) *
                                               (1 - percentual_desconto / 100)) * 100;
                            }
                        }
                    }
                }
            }
            catch (System.NullReferenceException exe)
            {
                throw new NullReferenceException(
                    "4-Verificar Descontos do Contrato que incidem na baixa e estão ativos", exe);
            }

            DateTime dataVencOriginal = titulo.dt_vcto_titulo;
            DateTime dataVenc = titulo.dt_vcto_titulo;

            try
            {
                //5-Calcular data de Vencimento:

                if (parametro != null && parametro.id_alterar_venc_final_semana)
                    pulaFeriadoEFinalSemana(ref dataVenc, titulo.cd_pessoa_empresa, ref feriadosEscola);
                titulo.dt_vcto_titulo = dataVenc;
            }
            catch (System.NullReferenceException exe)
            {
                throw new NullReferenceException("5-Calcular data de Vencimento", exe);
            }

            int nro_dias = (baixa.dt_baixa_titulo - titulo.dt_vcto_titulo).Days;

            if (parametro != null && !parametro.id_alterar_venc_final_semana)
                if (!parametro.id_juros_final_semana)
                {
                    pulaFeriadoEFinalSemana(ref dataVenc, titulo.cd_pessoa_empresa, ref feriadosEscola);
                    nro_dias = (baixa.dt_baixa_titulo - dataVenc).Days;
                }
                else
                {
                    pulaFeriadoEFinalSemana(ref dataVenc, titulo.cd_pessoa_empresa, ref feriadosEscola);
                    if ((baixa.dt_baixa_titulo - dataVenc).Days > 0)
                        nro_dias = (baixa.dt_baixa_titulo - dataVencOriginal).Days;
                    else
                        nro_dias = 0;
                }

            //6-Data da Baixa maior que a data de vencimento
            if (baixa.dt_baixa_titulo.CompareTo(titulo.dt_vcto_titulo) > 0)
            {
                try
                {
                    baixa.pc_pontualidade = 0;
                    percentual_desconto = 0;
                    baixa.des_desconto = String.Empty;
                    soma_valores_desconto = 0;
                    double nm_dias_carencia =
                        parametro.nm_dias_carencia.HasValue ? (double)parametro.nm_dias_carencia : 0;
                    if (!(baixa.dt_baixa_titulo.CompareTo(titulo.dt_vcto_titulo.AddDays(nm_dias_carencia)) <= 0)
                        && parametro.id_cobrar_juros_multa.HasValue && parametro.id_cobrar_juros_multa.Value
                        && titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER && nro_dias > 0)
                    {
                        if (parametro.pc_juros_dia.HasValue)
                            percentual_juros = parametro.pc_juros_dia.Value;
                        percentual_juros = nro_dias * percentual_juros;
                        if (parametro.pc_multa.HasValue)
                        {
                            //percentual_juros += parametro.pc_multa.Value;
                            percentual_multa = parametro.pc_multa.Value;
                        }
                    }
                }
                catch (System.NullReferenceException exe)
                {
                    throw new NullReferenceException("6-Data da Baixa maior que a data de vencimento", exe);
                }
            }

            //7-Data da Baixa menor ou igual ao Vencimento do item 4:
            else if (contrato != null && contrato.cd_aluno > 0 &&
                     titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER
                     && !Titulo.TipoTitulo.TM.ToString().Equals(titulo.dc_tipo_titulo) &&
                     !Titulo.TipoTitulo.TA.ToString().Equals(titulo.dc_tipo_titulo))
            {
                try
                {
                    //2-Verificar existência de Desconto por Antecipação:
                    PoliticaDesconto politica = null;
                    politica = financeiroBusiness.getPoliticaDescontoByTurmaAluno(cd_turma, contrato.cd_aluno,
                        titulo.dt_vcto_titulo);
                    if (politica != null && cd_turma > 0) //Desconto do aluno e turma
                        politica = financeiroBusiness.getPoliticaDescontoByTurmaAluno(cd_turma, contrato.cd_aluno,
                            titulo.dt_vcto_titulo);
                    if (politica == null) //Desconto do aluno
                        politica = financeiroBusiness.getPoliticaDescontoByAluno(contrato.cd_aluno,
                            titulo.dt_vcto_titulo);
                    if (politica == null) //Desconto da turma
                        politica = financeiroBusiness.getPoliticaDescontoByTurma(cd_turma, titulo.dt_vcto_titulo);
                    if (politica == null) //Desconto da escola
                        politica = financeiroBusiness.getPoliticaDescontoByEscola(contrato.cd_pessoa_escola,
                            titulo.dt_vcto_titulo);

                    //Guaradando o desconto do contrato para aplicar juntamente com o desconto da politica
                    //Se não achar a política, sempre vai considerar o desconto do contrato.
                    decimal percentual_anterior = percentual_desconto;
                    percentual_politica = percentual_desconto;
                    if (politica != null && politica.DiasPolitica != null)
                    {
                        List<DiasPolitica> dias = politica.DiasPolitica.ToList();
                        bool encontrou_politica = false;
                        //decimal percentual_politica = 0;
                        for (int i = 0; i < dias.Count && (!encontrou_politica || gerarMensagem); i++)
                        {
                            DateTime data_desconto = new DateTime();

                            //Caso não exista o dia, por exemplo, dia 31, tenta ainda o dia 30, 29 e 28:
                            bool encontrou_dia = false;
                            for (int k = 0; k < 3 && !encontrou_dia; k++)
                            {
                                try
                                {
                                    data_desconto = new DateTime(titulo.dt_vcto_titulo.Year,
                                        titulo.dt_vcto_titulo.Month, dias[i].nm_dia_limite_politica - k);
                                    encontrou_dia = true;
                                }
                                catch (System.ArgumentOutOfRangeException)
                                {
                                    encontrou_dia = false;
                                }
                            }

                            if (parametro != null && parametro.id_alterar_venc_final_semana)
                                pulaFeriadoEFinalSemana(ref data_desconto, titulo.cd_pessoa_empresa,
                                    ref feriadosEscola);

                            //Se achar a política com percentual diferente de zero e a data da baixa for menor ou igual a data da política, sempre vai considerar o desconto do contrato e o desconto da política.
                            //Se achar a política com percentual diferente de zero e a data da baixa for maior que a data da política e tiver marcado vai considerar o desconto do contrato.
                            if (dias[i].pc_desconto.HasValue && dias[i].pc_desconto > 0)
                            {
                                //percentual_politica = System.Convert.ToDouble(dias[i].pc_desconto.Value);
                                if (baixa.dt_baixa_titulo.CompareTo(data_desconto) <= 0)
                                {
                                    baixa.pc_pontualidade = System.Convert.ToDouble(dias[i].pc_desconto.Value);
                                    baixa.cd_politica_desconto = politica.cd_politica_desconto;

                                    //Aplica o percentual de pontualidade com o percentual de desconto:
                                    if (parametro.id_somar_descontos_financeiros)
                                        percentual_politica += (decimal)baixa.pc_pontualidade;
                                    else
                                        percentual_politica =
                                            100 - (((1 - percentual_desconto / 100) *
                                                    (1 - ((decimal)baixa.pc_pontualidade) / 100))) * 100;

                                    baixa.des_desconto = baixa.des_desconto + baixa.valor_desconto;

                                    encontrou_politica = true;

                                    if (gerarMensagem)
                                    {
                                        if (!baixa.sl_politicas.Contains(data_desconto))
                                        {
                                            baixa.sl_politicas[data_desconto] = percentual_politica;
                                            baixa.diasPoliticaAntecipacao.Add(new BaixaTitulo.DiasPoliticaAntecipacao
                                            {
                                                cd_politica_desconto = politica.cd_politica_desconto,
                                                Data_politica = politica.dt_inicial_politica,
                                                nm_dia_limite_politica = dias[i].nm_dia_limite_politica,
                                                pc_pontualidade = (decimal)dias[i].pc_desconto.Value,
                                                pc_pontualidade_total = percentual_politica,
                                                cd_titulo = titulo.cd_titulo,
                                                pc_desconto_baixa = percentual_anterior
                                            });
                                        }
                                        percentual_politica = percentual_anterior;
                                    }
                                    
                                }
                                else
                                    //Se encontrar uma política com percentual igual a zero, sempre considerar o desconto do contrato.
                                    //Se achar a política com percentual diferente de zero e a data da baixa for maior que a data da política e tiver desmarcado vai zerar todos os descontos do contrato e da política. 
                                    if ((!encontrou_politica || gerarMensagem) && percentual_politica != 0 &&
                                        !parametro.id_permitir_desc_apos_politica)
                                    {
                                        percentual_politica = 0;
                                        soma_valores_desconto = 0;
                                        baixa.des_desconto = String.Empty;
                                    }
                            }
                        }
                    }
                }
                catch (System.NullReferenceException exe)
                {
                    throw new NullReferenceException("7-Data da Baixa menor ou igual ao Vencimento do item 4", exe);
                }
            }


            try
            {
                percentual_desconto = percentual_politica;
                if (percentual_desconto > 100)
                    percentual_desconto = 100;
                decimal vl_titulo = titulo.vl_saldo_titulo;
                decimal vl_material_titulo = titulo.vl_material_titulo;
                if (usar_valor_titulo)
                    vl_titulo = titulo.vl_titulo -
                                (Math.Round(
                                    (decimal)titulo.pc_bolsa * (titulo.vl_titulo - titulo.vl_material_titulo) / 100,
                                    2, MidpointRounding.AwayFromZero));
                if ((usar_valor_titulo && vl_material_titulo > 0) ||
                    (vl_material_titulo > 0 && percentual_desconto > 0))
                {
                    if ((vl_titulo - vl_material_titulo) < 0)
                        vl_titulo = 0;
                    else
                        vl_titulo -= vl_material_titulo;
                }

                //8-Cálculos Finais:
                baixa.vl_desconto_baixa_calculado =
                    Decimal.Round(percentual_desconto * vl_titulo / 100, 2); //LBM Aqui já consideramos o parametro somar descontos no percentual_valor calculado acima
                //Decimal.Round(percentual_desconto * vl_titulo / 100 + soma_valores_desconto, 2); Aqui está errado pois não está analisando o parametro de somar descontos
                baixa.soma_valores_desconto = soma_valores_desconto;
                if (baixa.vl_desconto_baixa_calculado < 0) //Caso tem 100% + desconto em valor
                    baixa.vl_desconto_baixa_calculado = vl_titulo;

                decimal percentual_aplicado = 0;

                if (vl_titulo > 0)
                {
                    percentual_aplicado = baixa.vl_desconto_baixa_calculado * 100 / vl_titulo;

                    if (parametro.per_desconto_maximo.HasValue &&
                        (decimal)parametro.per_desconto_maximo.Value < percentual_aplicado)
                    {
                        percentual_desconto = (decimal)parametro.per_desconto_maximo.Value;
                        baixa.vl_desconto_baixa_calculado = Decimal.Round(percentual_desconto * vl_titulo / 100, 2);
                    }
                }

                // LBM estava assim if (!usar_valor_titulo && vl_material_titulo > 0 && percentual_desconto > 0)
                if ((usar_valor_titulo && vl_material_titulo > 0) ||
                    (vl_material_titulo > 0 && percentual_desconto > 0))
                {
                    if (vl_titulo > 0)
                    vl_titulo += vl_material_titulo;
                    else
                        vl_titulo = titulo.vl_saldo_titulo;

                }

                decimal vl_acrescimo = 0;
                if ((titulo.pc_multa_titulo > 0 || titulo.pc_juros_titulo > 0) &&
                    baixa.dt_baixa_titulo.CompareTo(titulo.dt_vcto_titulo) > 0)
                {
                    percentual_juros = titulo.pc_juros_titulo * nro_dias;
                    percentual_multa = titulo.pc_multa_titulo;
                }

                if (parametro.id_somar_descontos_financeiros)
                {
                    vl_acrescimo = Decimal.Round((decimal)percentual_juros * vl_titulo / 100, 2);
                    baixa.vl_multa_calculada = Decimal.Round((decimal)percentual_multa * vl_titulo / 100, 2);
                }
                else
                {
                    vl_acrescimo =
                        Decimal.Round(
                            (decimal)percentual_juros * (vl_titulo - baixa.vl_desconto_baixa_calculado) / 100, 2);
                    baixa.vl_multa_calculada =
                        Decimal.Round(
                            (decimal)percentual_multa * (vl_titulo - baixa.vl_desconto_baixa_calculado) / 100, 2);
                }

                baixa.vl_desconto_baixa = baixa.vl_desconto_baixa_calculado;
                baixa.vl_juros_calculado = vl_acrescimo;
                baixa.vl_liquidacao_baixa = vl_titulo + vl_acrescimo + baixa.vl_multa_calculada -
                                            baixa.vl_desconto_baixa_calculado;
                baixa.vl_acr = vl_acrescimo + baixa.vl_multa_calculada;
                baixa.vl_principal_baixa = vl_titulo;
                baixa.cd_titulo = titulo.cd_titulo;
                baixa.pc_juros_calc = percentual_juros;
                baixa.pc_multa_calc = percentual_multa;
                baixa.id_somar_descontos_financeiros = parametro.id_somar_descontos_financeiros;
            }
            catch (System.NullReferenceException exe)
            {
                throw new NullReferenceException("8-Cálculos Finais", exe);
            }
        }

        public decimal getPerceuntualPoliticaBaixa(List<DiasPolitica> listaDias, DateTime dt_baixa_titulo,
            DateTime dt_vcto_titulo)
        {
            //Monta as datas dos dias conforme a data de vencimento do titulo:
            foreach (DiasPolitica dia in listaDias)
                dia.dia_limite = new DateTime(dt_vcto_titulo.Year, dt_vcto_titulo.Month, dia.nm_dia_limite_politica);

            //Pega o percentual de desconto da política:
            for (int i = 0; i < listaDias.Count; i++)
                if ((i == 0 && dt_baixa_titulo <= listaDias[i].dia_limite)
                    || (dt_baixa_titulo <= listaDias[i].dia_limite && dt_baixa_titulo > listaDias[i - 1].dia_limite))
                    return (decimal)(listaDias[i].pc_desconto.HasValue ? listaDias[i].pc_desconto.Value : 0);
            return 0;
        }

        #endregion

        #region Diário de Aula

        public IEnumerable<vi_diario_aula> searchDiarioAula(SearchParameters parametros, int cd_turma,
            string no_professor, int cd_tipo_aula, byte status, byte presProf,
            bool substituto, bool inicio, DateTime? dtInicial, DateTime? dtFinal, int cd_escola, int? cdProf, int cd_escola_combo)
        {
            using (var transaction =
                   TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, daoAlunoEvento.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            {
                IEnumerable<vi_diario_aula> diarios = turmaBusiness.searchDiarioAula(parametros, cd_turma, no_professor, cd_tipo_aula, status, presProf,
                    substituto, inicio, dtInicial, dtFinal, cd_escola, cdProf, cd_escola_combo).ToList();
                transaction.Complete();
                return diarios;
            }
                
        }

        public DiarioAula getEditDiarioAula(int cd_diario_aula, int cd_escola)
        {
            DiarioAula diarioAula = new DiarioAula();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                diarioAula = turmaBusiness.getEditDiarioAula(cd_diario_aula, cd_escola);
                transaction.Complete();
            }

            return diarioAula;
        }

        public vi_diario_aula addDiarioAula(DiarioAula diario, bool aulaPersonalizada)
        {
            this.sincronizarContextos(daoAlunoEvento.DB());


            diario.id_status_aula = (int)DiarioAula.StatusDiarioAula.Efetivada;
            DiarioAula cadDiario = new DiarioAula();
            if (diario.falta_professor)
            {
                if (diario.falta_justificada)
                    diario.id_falta_professor = (int)DiarioAula.PresencaProfesssor.Justificada;
                else
                    diario.id_falta_professor = (int)DiarioAula.PresencaProfesssor.Falta;

            }
            else
                diario.id_falta_professor = (int)DiarioAula.PresencaProfesssor.Presente;

            cadDiario.copy(diario);
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED,
                    daoAlunoEvento.DB()))
            {
                DateTime data_diario = DateTime.Parse(diario.dta_aula);
                if (diario.dt_cadastro_aula.HasValue &&
                    DateTime.Compare(data_diario, diario.dt_cadastro_aula.Value) == 1)
                    throw new CoordenacaoBusinessException(Messages.msgErroDataDiarioMaiorDataAtual, null,
                        CoordenacaoBusinessException.TipoErro.ERRO_DATA_DIARIO_MAIOR_DATA_ATUAL, false);

                if ((diario.cd_professor == null || diario.cd_professor <= 0) &&
                    (diario.cd_professor_substituto <= 0 || diario.cd_professor_substituto == null))
                    throw new CoordenacaoBusinessException(Messages.msgErroDiarioSemProf, null,
                        CoordenacaoBusinessException.TipoErro.ERRO_DIARIO_SEM_PROFESSOR, false);
                List<Aluno> aluno = alunoBusiness
                    .getAlunosTurmaAtivosDiarioAulaDiario(diario.cd_turma, cadDiario.cd_pessoa_empresa, cadDiario.dt_aula)
                    .ToList();
                if (aluno.Count() <= 0)
                    throw new CoordenacaoBusinessException(Messages.msgNotAlunoMatriculado, null,
                        CoordenacaoBusinessException.TipoErro.DIARIO_SEM_ALUNO_MATRICULADO, false);
                //else //LBM eliminado pois não tem sentido em turmas regulares.
                //    if (aluno.Where(a => a.nm_carga >= a.nm_maxima).Any())
                //    throw new CoordenacaoBusinessException("Existem Alunos com carga horaria maior que o máximo permitido. Favor providenciar voucher e tentar novamente", null, 0, false);
                //TODO: Deivid
                DateTime? data_termino_turma = turmaBusiness
                    .findTurmasByIdAndCdEscola(diario.cd_turma, diario.cd_pessoa_empresa).dt_termino_turma;

                if (data_termino_turma != null && DateTime.Compare(data_diario, (DateTime)data_termino_turma) == 1)
                    throw new CoordenacaoBusinessException(
                        String.Format(Messages.msgErroDiarioTurmaEnc, data_termino_turma), null,
                        CoordenacaoBusinessException.TipoErro.ERRO_DIARIO_TURMA_ENCERRADA, false);
                //Verifica a regra: os horários do novo diário de aula não pode coincidir com os horários dos outros diários de aula ou com os horários das outras programações que ainda não possuem diário de aula
                if (!aulaPersonalizada && !turmaBusiness.verificaIntersecaoHorarios(diario.cd_turma,
                        diario.cd_programacao_turma, diario.hr_inicial_aula, diario.hr_final_aula, diario.dt_aula.Date))
                    throw new CoordenacaoBusinessException(Messages.msgIntersecaoHorariosDiarioAula, null,
                        CoordenacaoBusinessException.TipoErro.INTERSECAO_HORARIOS_DIARIO_AULA, false);

                if (cadDiario.cd_programacao_turma != null &&
                    turmaBusiness.verificaExisteDiarioEfetivoProgramacaoTurma(diario.cd_turma, diario.cd_pessoa_empresa,
                        (int)diario.cd_programacao_turma))
                    throw new CoordenacaoBusinessException(Messages.msgExistDiarioParaEssaProgramacao, null,
                        CoordenacaoBusinessException.TipoErro.INTERSECAO_HORARIOS_DIARIO_AULA, false);

                if (diario.cd_programacao_turma > 0)
                {
                    int cdProg = diario.cd_programacao_turma != null ? (int)diario.cd_programacao_turma : 0;
                    ProgramacaoTurma progTurma = daoProgramacaoTurma.findById(cdProg, false);
                    progTurma.id_aula_dada = true;
                    daoProgramacaoTurma.saveChanges(false);
                }

                cadDiario = turmaBusiness.addDiarioAula(cadDiario);
                List<Aluno> alunosComFalta = new List<Aluno>();
                if (diario.eventos != null && diario.eventos.Count() > 0)
                {
                    crudAlunosPorEventos(diario.eventos, cadDiario.cd_diario_aula, cadDiario.cd_turma,
                        cadDiario.cd_pessoa_empresa, cadDiario.dt_aula, cadDiario.cd_usuario);
                    if (diario.eventos.Where(e => e.cd_evento == (int)Evento.TiposEvento.FALTA).Count() > 0)
                    {
                        Evento eventos = diario.eventos.Where(e => e.cd_evento == (int)Evento.TiposEvento.FALTA)
                            .FirstOrDefault();
                        if (eventos.cd_evento > 0 && eventos.alunosEvento != null)
                        {
                            alunosComFalta = eventos.alunosEvento.Where(a => a.selecionadoAluno == true).ToList();
                        }
                    }
                }

                alunoBusiness.persistirAulaExecutaEFaltaAlunosDiarioAula(alunosComFalta, cadDiario.cd_turma,
                    cadDiario.cd_pessoa_empresa, cadDiario.dt_aula, DiarioAula.StatusDiarioAula.Efetivada);
                transaction.Complete();
            }

            if (aulaPersonalizada)
                return new vi_diario_aula() { cd_diario_aula = cadDiario.cd_diario_aula };
            return turmaBusiness.getDiarioForGridById(cadDiario.cd_diario_aula, diario.cd_pessoa_empresa);
        }

        public vi_diario_aula editDiarioAula(DiarioAula diario)
        {
            this.sincronizarContextos(daoAlunoEvento.DB());
            DiarioAula diarioContext = turmaBusiness
                .getDiarioAulas(new int[] { diario.cd_diario_aula }, diario.cd_pessoa_empresa).FirstOrDefault();
            if (diarioContext == null || diarioContext.cd_diario_aula == 0)
                throw new CoordenacaoBusinessException(Messages.msgRegNotEnc, null,
                    CoordenacaoBusinessException.TipoErro.DIARIO_AULA_JA_CANCELADO, false);
            if (diarioContext.id_status_aula == (int)DiarioAula.StatusDiarioAula.Cancelada)
                throw new CoordenacaoBusinessException(Messages.msgNotUpdateDiarioCancelado, null,
                    CoordenacaoBusinessException.TipoErro.NAO_POSSIVEL_ALTERACAO_DIARIO_CANCELADO, false);
            diarioContext.dc_obs_falta = diario.dc_obs_falta;
            diarioContext.tx_obs_aula = diario.tx_obs_aula;
            diarioContext.cd_motivo_falta = diario.cd_motivo_falta;
            diarioContext.cd_sala = diario.cd_sala;
            diarioContext.cd_avaliacao = diario.cd_avaliacao;
            diarioContext.hr_inicial_aula = diario.hr_inicial_aula;
            diarioContext.hr_final_aula = diario.hr_final_aula;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED,
                    daoAlunoEvento.DB()))
            {
                DateTime data_diario = DateTime.Parse(diario.dta_aula);
                if (diario.dt_cadastro_aula.HasValue && DateTime.Compare(data_diario, diario.dt_cadastro_aula.Value) == 1)
                    throw new CoordenacaoBusinessException(Messages.msgErroDataDiarioMaiorDataAtual, null, CoordenacaoBusinessException.TipoErro.ERRO_DATA_DIARIO_MAIOR_DATA_ATUAL, false);

                if ((diario.cd_professor == null || diario.cd_professor <= 0) &&
                    (diario.cd_professor_substituto <= 0 || diario.cd_professor_substituto == null))
                    throw new CoordenacaoBusinessException(Messages.msgErroDiarioSemProf, null,
                        CoordenacaoBusinessException.TipoErro.ERRO_DIARIO_SEM_PROFESSOR, false);
                daoAlunoEvento.saveChanges(false);
                if (diario.eventos != null && diario.eventos.Count() > 0)
                {
                    foreach (Evento eventoView in diario.eventos)
                    {
                        if (eventoView.cd_evento > 0 && eventoView.alunosEvento != null)
                        {
                            List<AlunoEvento> alunosEvento = daoAlunoEvento.getAlunoEvento(eventoView.cd_evento, 0,
                                diarioContext.cd_pessoa_empresa, diarioContext.cd_diario_aula,
                                AlunoEventoDataAccess.TipoConsultaAlunoEventoEnum.HAS_ALUNOS_EVENTO_DIARIO).ToList();
                            List<Aluno> inserirFaltasAlunos = new List<Aluno>();
                            List<Aluno> removerFaltasAlunos = new List<Aluno>();
                            foreach (Aluno alunoEvento in eventoView.alunosEvento)
                            {
                                AlunoEvento alunoEventoContext = alunosEvento
                                    .Where(a => a.cd_aluno == alunoEvento.cd_aluno).FirstOrDefault();
                                if (alunoEvento.selecionadoAluno)
                                {
                                    if (alunoEventoContext == null || alunoEventoContext.cd_evento == 0)
                                    {
                                        if (eventoView.cd_evento == (int)Evento.TiposEvento.FALTA)
                                            inserirFaltasAlunos.Add(alunoEvento);
                                        daoAlunoEvento.add(
                                            new AlunoEvento
                                            {
                                                cd_aluno = alunoEvento.cd_aluno,
                                                cd_evento = eventoView.cd_evento,
                                                cd_diario_aula = diarioContext.cd_diario_aula
                                            }, false);
                                    }
                                }
                                else if (alunoEventoContext != null && alunoEventoContext.cd_evento > 0)
                                {
                                    removerFaltasAlunos.Add(alunoEvento);
                                    daoAlunoEvento.delete(alunoEventoContext, false);
                                    //Deleta o histórico 
                                    deleteHistoricoAlunoComEvento(diario, alunoEvento, diario.dt_aula);
                                }
                            }

                            if (eventoView.cd_evento == (int)Evento.TiposEvento.FALTA)
                            {
                                if (inserirFaltasAlunos.Count() > 0)
                                    alunoBusiness.incrementarOuRemoverFaltaAlunoTurmaDiario(inserirFaltasAlunos,
                                        diarioContext.cd_turma, diarioContext.cd_pessoa_empresa,
                                        DiarioAula.StatusDiarioAula.Efetivada);
                                if (removerFaltasAlunos.Count() > 0)
                                    alunoBusiness.incrementarOuRemoverFaltaAlunoTurmaDiario(removerFaltasAlunos,
                                        diarioContext.cd_turma, diarioContext.cd_pessoa_empresa,
                                        DiarioAula.StatusDiarioAula.Cancelada);
                            }
                        }
                    }
                }

                transaction.Complete();
            }

            return turmaBusiness.getDiarioForGridById(diarioContext.cd_diario_aula, diarioContext.cd_pessoa_empresa);
        }

        private void deleteHistoricoAlunoComEvento(DiarioAula diario, Aluno alunoEvento, DateTime dataDiario)
        {
            //consistência para a remosão do histórico do aluno
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                Contrato contrato = matriculaBusiness.findMatriculaByTurmaAluno(diario.cd_turma, alunoEvento.cd_aluno);
                HistoricoAluno historico = turmaBusiness.GetHistoricoAlunoPrimeiraAula(diario.cd_pessoa_empresa,
                    alunoEvento.cd_aluno, diario.cd_turma, contrato.cd_contrato, dataDiario);
                if (historico != null && historico.cd_historico_aluno > 0)
                    deleted = turmaBusiness.deleteHistoricoAluno(historico);
                transaction.Complete();
            }
        }

        public bool deleteDiarios(int[] cdDiarios, int cd_escola)
        {
            this.sincronizarContextos(daoAlunoEvento.DB());
            bool retorno = false;
            if (cdDiarios != null && cdDiarios.Count() > 0)
            {
                List<DiarioAula> diariosContext = turmaBusiness.getDiarioAulas(cdDiarios, cd_escola).ToList();
                using (var transaction =
                    TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED,
                        daoAlunoEvento.DB()))
                {
                    foreach (var dAula in diariosContext)
                    {
                        if (dAula.id_status_aula != (int)DiarioAula.StatusDiarioAula.Cancelada)
                        {
                            if (dAula.cd_programacao_turma > 0)
                            {
                                int cdProg = dAula.cd_programacao_turma != null ? (int)dAula.cd_programacao_turma : 0;
                                ProgramacaoTurma progTurma = daoProgramacaoTurma.findById(cdProg, false);
                                progTurma.id_aula_dada = false;
                                daoProgramacaoTurma.saveChanges(false);
                            }

                            List<Aluno> alunosComFalta = alunoBusiness.getAlunosPorEvento(dAula.cd_turma,
                                dAula.cd_pessoa_empresa, (int)Evento.TiposEvento.FALTA, dAula.cd_diario_aula).ToList();
                            alunoBusiness.persistirAulaExecutaEFaltaAlunosDiarioAula(alunosComFalta, dAula.cd_turma,
                                cd_escola, dAula.dt_aula, DiarioAula.StatusDiarioAula.Cancelada);
                        }
                        else
                            deleteAllAlunoEvento(dAula.cd_diario_aula, dAula.cd_pessoa_empresa);

                        retorno = turmaBusiness.deletarDiarioAula(dAula);
                    }

                    transaction.Complete();
                }
            }

            return retorno;
        }

        public void cancelarDiarioAula(int cd_diario_aula, int cd_pessoa_escola)
        {
            this.sincronizarContextos(daoAlunoEvento.DB());
            DiarioAula diariosContext = turmaBusiness.getDiarioAulas(new int[] { cd_diario_aula }, cd_pessoa_escola)
                .FirstOrDefault();
            if (diariosContext.id_status_aula == (int)DiarioAula.StatusDiarioAula.Cancelada)
                throw new CoordenacaoBusinessException(Messages.msgNotCancelDiarioCancelado, null,
                    CoordenacaoBusinessException.TipoErro.DIARIO_AULA_JA_CANCELADO, false);
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED,
                    daoAlunoEvento.DB()))
            {
                diariosContext.id_status_aula = (int)DiarioAula.StatusDiarioAula.Cancelada;
                daoAlunoEvento.saveChanges(false);
                List<Aluno> alunosComFalta = alunoBusiness.getAlunosPorEvento(diariosContext.cd_turma, cd_pessoa_escola,
                    (int)Evento.TiposEvento.FALTA, diariosContext.cd_diario_aula).ToList();
                if (alunosComFalta.Count() <= 0)
                    alunosComFalta = new List<Aluno>();
                alunoBusiness.persistirAulaExecutaEFaltaAlunosDiarioAula(alunosComFalta, diariosContext.cd_turma,
                    cd_pessoa_escola, diariosContext.dt_aula, DiarioAula.StatusDiarioAula.Cancelada);
                if (diariosContext.cd_programacao_turma > 0)
                {
                    int cdProg = diariosContext.cd_programacao_turma != null
                        ? (int)diariosContext.cd_programacao_turma
                        : 0;
                    ProgramacaoTurma progTurma = daoProgramacaoTurma.findById(cdProg, false);
                    progTurma.id_aula_dada = false;
                    daoProgramacaoTurma.saveChanges(false);
                }

                transaction.Complete();
            }

        }

        private void crudAlunosPorEventos(List<Evento> eventosAlunos, int cd_diario_aula, int cd_turma,
            int cd_pessoa_escola, DateTime dtaDiarioAula, int cdUsuario)
        {
            foreach (Evento evt in eventosAlunos)
            {
                if (evt.alunosEvento != null)
                {
                    List<Aluno> alunos = evt.alunosEvento.Where(a => a.selecionadoAluno == true).ToList();
                    foreach (Aluno a in alunos)
                    {
                        daoAlunoEvento.add(
                            new AlunoEvento { cd_aluno = a.cd_aluno, cd_evento = evt.cd_evento, cd_diario_aula = cd_diario_aula },
                            false);

                        //int qtdDiario = daoDiarioAula.returnQuantidadeDiarioAulaByTurma(cd_turma, cd_pessoa_escola, a.cd_aluno);
                        //if (qtdDiario == 1)
                        //   insertHistoricoAluno(cd_turma, cd_pessoa_escola, dtaDiarioAula, cdUsuario, a.cd_aluno, qtdDiario);
                    }
                }
            }

            daoAlunoEvento.saveChanges(false);
        }

        private void insertHistoricoAluno(int cd_turma, int cd_pessoa_escola, DateTime dtaDiarioAula, int cdUsuario,
            int cd_aluno, int qtdDiario)
        {
            Contrato contrato = matriculaBusiness.findMatriculaByTurmaAluno(cd_turma, cd_aluno);
            byte sequenciaMax =
                (byte)turmaBusiness.retunMaxSequenciaHistoricoAluno(contrato.cd_produto_atual, cd_pessoa_escola,
                    cd_aluno);

            HistoricoAluno historico = new HistoricoAluno
            {
                cd_aluno = cd_aluno,
                cd_turma = cd_turma,
                cd_contrato = contrato.cd_contrato,
                id_tipo_movimento = (byte)HistoricoAluno.TipoMovimento.PRIMEIRA_AULA,
                dt_cadastro = DateTime.UtcNow,
                dt_historico = dtaDiarioAula.ToLocalTime().Date,
                cd_produto = contrato.cd_produto_atual,
                nm_sequencia = ++sequenciaMax,
                id_situacao_historico = (byte)contrato.cd_situacao_aluno_turma,
                cd_usuario = cdUsuario
            };

            turmaBusiness.addHistoricoAluno(historico);
        }

        public int returnQuantidadeDiarioAulaByTurma(int cd_turma, int cd_escola, int cd_aluno)
        {
            return turmaBusiness.returnQuantidadeDiarioAulaByTurma(cd_turma, cd_escola, cd_aluno);
        }

        public int returnDiarioByDataDesistencia(DateTime? data_desistencia, int cd_pessoa_escola, int cd_aluno,
            int cd_turma_aluno, int tipoDesistencia)
        {
            return turmaBusiness.returnDiarioByDataDesistencia(data_desistencia, cd_pessoa_escola, cd_aluno,
                cd_turma_aluno, tipoDesistencia);
        }

        public string getObsDiarioAula(int cd_diario_aula, int cd_escola)
        {
            string retorno = string.Empty;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = turmaBusiness.getObsDiarioAula(cd_diario_aula, cd_escola);
            }

            return retorno;
        }

        private void editObsDiarioAula(DiarioAula diario)
        {
            this.sincronizarContextos(daoAlunoEvento.DB());
            DiarioAula diarioContext = turmaBusiness
                .getDiarioAulas(new int[] { diario.cd_diario_aula }, diario.cd_pessoa_empresa).FirstOrDefault();
            if (diarioContext.id_status_aula == (int)DiarioAula.StatusDiarioAula.Cancelada)
                throw new CoordenacaoBusinessException(Messages.msgNotUpdateDiarioCancelado, null,
                    CoordenacaoBusinessException.TipoErro.NAO_POSSIVEL_ALTERACAO_DIARIO_CANCELADO, false);
            diarioContext.tx_obs_aula = diario.tx_obs_aula;
            diarioContext.hr_inicial_aula = diario.hr_inicial_aula;
            diarioContext.hr_final_aula = diario.hr_final_aula;
            daoAlunoEvento.saveChanges(false);
        }

        #endregion

        #region Mudança  Interna

        public MudancasInternas postMudancaTurma(MudancasInternas mudanca )
        {
            if (mudanca != null)
            {
                this.sincronizarContextos(daoEstagio.DB());
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                    TransactionScopeBuilder.TransactionType.COMMITED, daoEstagio.DB(),
                    TransactionScopeBuilder.TransactionTime.MODEREATE))
                {
                    List<LivroAlunoApiCyberBdUI> listaLivroAlunoDelete = new List<LivroAlunoApiCyberBdUI>();
                    if (businessApiNewCyber.aplicaApiCyber())
                    {
                        // preenche a lista de livroAluno que irá deletar
                        foreach (AlunoTurma mudancaAluno in mudanca.alunos)
                        {
                            LivroAlunoApiCyberBdUI livroAlunoApiCyber = alunoBusiness
                                .findLivroAlunoTurmaApiCyber(mudancaAluno.cd_aluno, mudanca.cd_turma_origem, mudanca.cd_escola);

                            if (livroAlunoApiCyber != null && existeLivroAlunoByCodAluno(livroAlunoApiCyber.codigo_aluno, livroAlunoApiCyber.codigo_grupo, livroAlunoApiCyber.codigo_livro))
                            {
                                listaLivroAlunoDelete.Add(livroAlunoApiCyber);
                            }
                        }
                    }

                    List<AlunoTurma> alunoTurmaPPTCyber = new List<AlunoTurma>();
                    List<Aluno> listPPT = new List<Aluno>();

                    if (mudanca.opcao == (int)MudancasInternas.OpcoesMudanca.MudarTurma)
                        turmaBusiness.getVerificaDadosMudanca(mudanca.alunos, mudanca.cd_escola);

                    if (mudanca.id_ppt && mudanca.opcao == (int)FundacaoFisk.SGF.Web.Services.Secretaria.Model
                            .MudancasInternas.OpcoesMudanca.MudarTurma)
                    {
                        if (mudanca.cd_curso <= 0)
                            throw new CoordenacaoBusinessException(
                                string.Format(Utils.Messages.Messages.msgErroCursoMudancaInt), null,
                                CoordenacaoBusinessException.TipoErro.ERRO_MUDANCAO_SEM_CURSO, false);
                        #region Turma ppt filha
                        //Criar Turma PPT FILHA
                        Turma turmaPPTPai =
                            turmaBusiness.findTurmasByIdAndCdEscola(mudanca.cd_turma_destino, mudanca.cd_escola);
                        Turma turmaFilha = new Turma();
                        TurmaEscola novaTurmaEscola = new TurmaEscola();
                        List<Turma> turmasPPTFilhas = new List<Turma>();
                        foreach (AlunoTurma a in mudanca.alunos)
                        {
                            if (turmaPPTPai != null)
                            {
                                List<AlunoTurma> alunoPPT = new List<AlunoTurma>();
                                int[] alunos = { a.cd_aluno };
                                AlunoTurma alunoTurmaAnterior = alunoBusiness
                                    .findAlunosTurma(mudanca.cd_turma_origem, mudanca.cd_escola, alunos)
                                    .FirstOrDefault();
                                DateTime? dtMatriculaAnterior = null;

                                if (alunoTurmaAnterior != null)
                                {
                                    if (mudanca.id_manter_contrato && alunoTurmaAnterior.dt_matricula.HasValue)
                                    {
                                        dtMatriculaAnterior = alunoTurmaAnterior.dt_matricula.Value.ToLocalTime();
                                    }

                                    int situacaoAluno = (int)FundacaoFisk.SGF.GenericModel.AlunoTurma
                                        .SituacaoAlunoTurma.Aguardando;
                                    if (mudanca.id_manter_contrato && alunoTurmaAnterior.cd_contrato.HasValue)
                                    {
                                        Contrato contrato =
                                            matriculaBusiness.getMatriculaByIdGeral(
                                                alunoTurmaAnterior.cd_contrato.Value, mudanca.cd_escola);
                                        contrato.cd_curso_atual = mudanca.cd_curso;
                                        contrato.cd_duracao_atual = turmaPPTPai.cd_duracao;
                                        contrato.cd_regime_atual = turmaPPTPai.cd_regime;
                                        situacaoAluno = contrato.id_tipo_matricula == 1
                                            ? (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo
                                            : (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma
                                                .Rematriculado;
                                        matriculaBusiness.updateContrato(contrato);
                                    }

                                    AlunoTurma alunoTurma = new AlunoTurma
                                    {
                                        cd_contrato =
                                            mudanca.id_manter_contrato ? alunoTurmaAnterior.cd_contrato : null,
                                        cd_aluno = a.cd_aluno,
                                        cd_turma = mudanca.cd_turma_destino,
                                        cd_situacao_aluno_turma = situacaoAluno,
                                        dt_inicio = mudanca.dt_inicio,
                                        id_manter_contrato = mudanca.id_manter_contrato,
                                        id_renegociacao = mudanca.id_renegociacao,
                                        cd_turma_origem = mudanca.cd_turma_origem,
                                        cd_situacao_aluno_origem =
                                            Convert.ToByte(alunoTurmaAnterior.cd_situacao_aluno_turma),
                                        dt_matricula = dtMatriculaAnterior,
                                        nm_matricula_turma = mudanca.id_manter_contrato
                                            ? alunoTurmaAnterior.nm_matricula_turma
                                            : null,
                                        id_tipo_movimento =
                                            Convert.ToByte(HistoricoAluno.TipoMovimento.MUDANCA_INTERNA),
                                        nm_faltas = 0
                                    };
                                    List<Horario> horariosTurma = turmaBusiness
                                        .getHorarioByEscolaForRegistro(mudanca.cd_escola, mudanca.cd_turma_destino,
                                            Horario.Origem.TURMA).ToList();
                                    alunoPPT.Add(alunoTurma);
                                    turmaFilha = new Turma
                                    {
                                        alunosTurma = alunoPPT,
                                        cd_curso = mudanca.cd_curso,
                                        cd_duracao = turmaPPTPai.cd_duracao,
                                        cd_pessoa_escola = turmaPPTPai.cd_pessoa_escola,
                                        cd_produto = turmaPPTPai.cd_produto,
                                        cd_regime = turmaPPTPai.cd_regime,
                                        cd_sala = turmaPPTPai.cd_sala,
                                        cd_sala_online = turmaPPTPai.cd_sala_online,
                                        dt_inicio_aula = mudanca.dt_inicio,
                                        horariosTurma = horariosTurma,
                                        cd_turma_ppt = mudanca.cd_turma_destino

                                    };
                                    var nomeTurma = criarNomeTurma(false, mudanca.cd_turma_origem, turmaFilha.cd_regime,
                                        turmaFilha.cd_produto, turmaFilha.cd_curso, horariosTurma,
                                        turmaFilha.dt_inicio_aula);
                                    int nrProximaTurma =
                                        turmaBusiness.verificaExisteTurma(nomeTurma, turmaFilha.cd_pessoa_escola, 0);
                                    nomeTurma = nomeTurma + "-" + nrProximaTurma;

                                    turmaFilha.no_turma = nomeTurma;
                                    turmaFilha.nm_turma = nrProximaTurma;

                                    Turma turmaSearch = turmaBusiness.addTurma(turmaFilha);
                                    turmaFilha.cd_turma = turmaSearch.cd_turma;
                                    if (turmaFilha.horariosTurma != null)
                                    {
                                        turmaFilha.horariosTurma = Horario.clonarHorariosZerandoMemoria(
                                            turmaFilha.horariosTurma.ToList(),
                                            "Calendar2");
                                        turmaBusiness.crudHorariosTurma(turmaFilha.horariosTurma.ToList()
                                            , turmaFilha.cd_turma, turmaFilha.cd_pessoa_escola,
                                            Turma.TipoTurma.NORMAL);
                                    }

                                    List<ProfessorTurma> professorTurmaPai = turmaBusiness
                                        .findProfessoresTurmaPorTurmaEscola(mudanca.cd_turma_destino, mudanca.cd_escola)
                                        .ToList();
                                    if (professorTurmaPai != null && professorTurmaPai.Count() > 0)
                                        turmaBusiness.crudProfessoresTurma(
                                            ProfessorTurma.clonarProfessorZerandoMemoria(professorTurmaPai,
                                                turmaFilha.cd_turma), turmaFilha, turmaFilha.horariosTurma, false);

                                    alunoTurma.cd_turma = turmaSearch.cd_turma;
                                    alunoTurma.cd_turma_origem = mudanca.cd_turma_origem;

                                    AlunoTurma alunoTurmaSaved = alunoBusiness.addAlunoTurma(alunoTurma);
                                    if (businessApiNewCyber.aplicaApiCyber())
                                    {
                                        //preenche a lista de alunos para cadastrar os livrosAlunos
                                        alunoTurmaPPTCyber.Add(alunoTurmaSaved);
                                    }

                                    if (turmaFilha.cd_sala_online != null && (mudanca.cd_escola != turmaPPTPai.cd_pessoa_escola))
                                    {
                                        novaTurmaEscola.cd_turma = alunoTurma.cd_turma;
                                        novaTurmaEscola.cd_escola = mudanca.cd_escola;
                                        daoTurmaEscola.add(novaTurmaEscola, false);
                                    }
                                    if (mudanca.id_manter_contrato)
                                    {
                                        criaProgramacoesTurmaTurma(mudanca.cd_turma_origem, turmaSearch.cd_turma, mudanca.cd_escola, true);
                                    }
                                    #endregion

                                    if (alunoTurmaAnterior.cd_contrato != null)
                                    {
                                        MudancasInternas mud = new MudancasInternas();
                                        mud.copy(mudanca);
                                        mud.cd_turma_destino = turmaSearch.cd_turma;
                                        alunoBusiness.gerarHistoricoMudanca(alunoTurmaAnterior, mud, a.cd_aluno,
                                            situacaoAluno);
                                    }

                                    //Alterando Aluno Turma da turma de Origem
                                    alunoTurmaAnterior.dt_movimento = mudanca.dt_movimentacao;
                                    alunoTurmaAnterior.id_tipo_movimento =
                                        (int)HistoricoAluno.TipoMovimento.MUDANCA_INTERNA;
                                    alunoTurmaAnterior.cd_situacao_aluno_turma = (int)FundacaoFisk.SGF.GenericModel
                                        .AlunoTurma.SituacaoAlunoTurma.Movido;
                                    alunoBusiness.editAlunoTurma(alunoTurmaAnterior);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (AlunoTurma a in mudanca.alunos)
                        {
                            AlunoTurma alunoTurmaAnterior = alunoBusiness.findAlunoTurma(a.cd_aluno,
                                mudanca.cd_turma_origem, mudanca.cd_escola);
                            Turma turmaDestino =
                                turmaBusiness.findTurmasByIdAndCdEscola(mudanca.cd_turma_destino, mudanca.cd_escola);

                            if (turmaDestino != null && turmaDestino.dt_termino_turma != null)
                                throw new CoordenacaoBusinessException(
                                    string.Format(Utils.Messages.Messages.msgErroMovimentoTurmaEncerrada), null,
                                    CoordenacaoBusinessException.TipoErro.ERRO_MOVIMENTO_TURMA_ENCERRADA, false);

                            if (mudanca.id_manter_contrato && alunoTurmaAnterior.cd_contrato.HasValue)
                            {
                                Contrato contrato =
                                    matriculaBusiness.getMatriculaByIdGeral(alunoTurmaAnterior.cd_contrato.Value,
                                        mudanca.cd_escola);
                                contrato.cd_curso_atual =
                                    turmaDestino.cd_curso.HasValue ? turmaDestino.cd_curso.Value : 0;
                                contrato.cd_duracao_atual = turmaDestino.cd_duracao;
                                contrato.cd_regime_atual = turmaDestino.cd_regime;
                                matriculaBusiness.updateContrato(contrato);
                            }
                        }

                        if (mudanca.opcao == (int)FundacaoFisk.SGF.Web.Services.Secretaria.Model.MudancasInternas
                                .OpcoesMudanca.RetornarTurmaOri)
                        {
                            foreach (AlunoTurma a in mudanca.alunos)
                            {
                                bool existDesistencia = turmaBusiness.getExisteDesistenciaPorAlunoTurma(a.cd_aluno,
                                    mudanca.cd_turma_origem, mudanca.cd_escola);
                                if (existDesistencia)
                                    throw new CoordenacaoBusinessException(
                                        string.Format(Utils.Messages.Messages.msgExisteDesistencia), null,
                                        CoordenacaoBusinessException.TipoErro.EXISTE_DESISTENCIA, false);
                                bool existeAvaliacaoTurma =
                                    turmaBusiness.verificaAvalicaoAlunoTurma(a.cd_aluno, mudanca.cd_turma_origem);
                                if (existeAvaliacaoTurma)
                                    throw new CoordenacaoBusinessException(
                                        string.Format(Utils.Messages.Messages.msgErroMudancaExisteAval), null,
                                        CoordenacaoBusinessException.TipoErro.ERRO_ALUNO_COM_AVALIACAO, false);


                            }

                            alunoBusiness.postMudancaInterna(mudanca);
                            TurmaSearch turmaDeletar =
                                turmaBusiness.getTurmaByCodMudancaOuEncerramento(mudanca.cd_turma_origem,
                                    mudanca.cd_escola);

                            if (turmaDeletar.cd_turma_ppt > 0)
                            {
                                int[] turmas = { turmaDeletar.cd_turma };
                                //Deletar professor da turma PPT Filha
                                turmaBusiness.deleteProfPPTFilhaMudancaInterna(turmaDeletar.cd_turma,
                                    mudanca.cd_escola);
                                //DELETAR TURMA PPT FILHA
                                turmaBusiness.deleteTurmas(turmas, mudanca.cd_escola);

                            }
                        }
                        else
                            alunoBusiness.postMudancaInterna(mudanca);
                    }

                    if (turmaBusiness.verificaSeTurmaEFilhaPersonalizada(mudanca.cd_turma_origem, mudanca.cd_escola))
                    {
                        List<Turma> turmas = new List<Turma>();

                        turmas.Add(new Turma { cd_turma = mudanca.cd_turma_origem, dt_termino_turma = DateTime.Now.Date });
                        turmaBusiness.editTurmaEncerramento(turmas, mudanca.cd_usuario, mudanca.fusoHorario);
                    }


                    if (businessApiNewCyber.aplicaApiCyber())
                    {
                        //Se tem registros para deletar no cyber
                        if (listaLivroAlunoDelete != null && listaLivroAlunoDelete.Count > 0)
                        {
                            foreach (LivroAlunoApiCyberBdUI livroAlunoDelete in listaLivroAlunoDelete)
                            {
                                //chama a api cyber com o comanco (DELETA_LIVRO_ALUNO)
                                deletaLivroAlunoApiCyber(livroAlunoDelete, ApiCyberComandosNames.DELETA_LIVROALUNO);
                            }

                        }


                        List<LivroAlunoApiCyberBdUI> listaLivroAlunoAdd = new List<LivroAlunoApiCyberBdUI>();
                        // preenche a lista de livroAluno que irá adicionar
                        if (mudanca.id_ppt)
                        {
                            PreencheListaAlunoPPTAdd(alunoTurmaPPTCyber, mudanca, listaLivroAlunoAdd);
                        }
                        else
                        {
                            PreencheListaAlunoAdd(mudanca, listaLivroAlunoAdd);
                        }


                        //
                        //Se tem registros para adicionar no Cyber
                        if (listaLivroAlunoAdd != null && listaLivroAlunoAdd.Count > 0)
                        {
                            foreach (LivroAlunoApiCyberBdUI livroAlunoAdd in listaLivroAlunoAdd)
                            {
                                //chama a api cyber com o comanco (CADASTRA_LIVRO_ALUNO)
                                cadastraLivroAlunoApiCyber(livroAlunoAdd, ApiCyberComandosNames.CADASTRA_LIVROALUNO);
                            }

                        }
                    }

                    transaction.Complete();
                }
            }

            return mudanca;
        }

        private void cadastraLivroAlunoApiCyber(LivroAlunoApiCyberBdUI livroAlunoCyberCurrent, string comando)
        {

            string parametros = "";

            parametros = validaParametrosCyberCadastroLivroAluno(livroAlunoCyberCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");

            string result = businessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);

        }

        private string validaParametrosCyberCadastroLivroAluno(LivroAlunoApiCyberBdUI entity, string url, string comando, string parametros)
        {


            if (entity == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }


            if (entity.codigo_aluno <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCdAlunoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_COD_ALUNO_MENOR_IGUAL_ZERO, false);
            }

            if (entity.codigo_grupo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCodigoGrupoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_GRUPO_MENOR_IGUAL_ZERO, false);
            }

            if (entity.codigo_livro <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCodigoLivroMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_LIVRO_MENOR_IGUAL_ZERO, false);
            }


            string listaParams = "";
            listaParams = string.Format("codigo_aluno={0},codigo_grupo={1},codigo_livro={2}", entity.codigo_aluno, entity.codigo_grupo, entity.codigo_livro);
            return listaParams;
        }

        private void PreencheListaAlunoAdd(MudancasInternas mudanca, List<LivroAlunoApiCyberBdUI> listaLivroAlunoAdd)
        {
            // preenche a lista de livroAluno que irá adicionar
            foreach (AlunoTurma mudancaAluno in mudanca.alunos)
            {
                LivroAlunoApiCyberBdUI livroAlunoApiCyber = alunoBusiness
                    .findLivroAlunoTurmaApiCyber(mudancaAluno.cd_aluno, mudanca.cd_turma_destino, mudanca.cd_escola);
                // se existe o (aluno e grupo) no cyber e nao existe livroAluno no cyber
                if (livroAlunoApiCyber != null &&
                    existeAluno(livroAlunoApiCyber.codigo_aluno) &&
                    existeGrupoByCodigoGrupo(livroAlunoApiCyber.codigo_grupo) &&
                    !existeLivroAlunoByCodAluno(livroAlunoApiCyber.codigo_aluno, livroAlunoApiCyber.codigo_grupo, livroAlunoApiCyber.codigo_livro))
                {
                    listaLivroAlunoAdd.Add(livroAlunoApiCyber);
                }
            }
        }

        private void PreencheListaAlunoPPTAdd(List<AlunoTurma> listaAlunosPPTAdd, MudancasInternas mudanca, List<LivroAlunoApiCyberBdUI> listaLivroAlunoAdd)
        {
            // preenche a lista de livroAluno que irá adicionar
            foreach (AlunoTurma mudancaAluno in listaAlunosPPTAdd)
            {
                LivroAlunoApiCyberBdUI livroAlunoApiCyber = alunoBusiness
                    .findLivroAlunoTurmaApiCyber(mudancaAluno.cd_aluno, mudancaAluno.cd_turma, mudanca.cd_escola);
                // se existe o (aluno e grupo) no cyber e nao existe livroAluno no cyber
                if (livroAlunoApiCyber != null &&
                    existeAluno(livroAlunoApiCyber.codigo_aluno) &&
                    existeGrupoByCodigoGrupo(livroAlunoApiCyber.codigo_grupo) &&
                    !existeLivroAlunoByCodAluno(livroAlunoApiCyber.codigo_aluno, livroAlunoApiCyber.codigo_grupo, livroAlunoApiCyber.codigo_livro))
                {
                    listaLivroAlunoAdd.Add(livroAlunoApiCyber);
                }
            }
        }

        public bool existeAluno(int codigo)
        {
            return businessApiNewCyber.verificaRegistro(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.VISUALIZA_ALUNO, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo);
        }

        private void deletaLivroAlunoApiCyber(LivroAlunoApiCyberBdUI livroAlunoCyberCurrent, string comando)
        {

            string parametros = "";

            //valida e retorna os parametros para a requisicao cyber
            parametros = validaParametrosCyberDeletaLivroAluno(livroAlunoCyberCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");

            string result = businessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);

        }

        private string validaParametrosCyberDeletaLivroAluno(LivroAlunoApiCyberBdUI entity, string url, string comando, string parametros)
        {


            if (entity == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }


            if (entity.codigo_aluno <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCdAlunoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_COD_ALUNO_MENOR_IGUAL_ZERO, false);
            }

            if (entity.codigo_grupo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCodigoGrupoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_GRUPO_MENOR_IGUAL_ZERO, false);
            }


            string listaParams = "";
            listaParams = string.Format("codigo_aluno={0},codigo_grupo={1}", entity.codigo_aluno, entity.codigo_grupo);
            return listaParams;
        }

        public bool existeLivroAlunoByCodAluno(int codigo_aluno, int codigo_grupo, int codigo_livro)
        {
            return businessApiNewCyber.verificaRegistroLivroAlunos(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.VISUALIZA_LIVROALUNO, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo_aluno, codigo_grupo, codigo_livro);
        }

        #endregion

        #region Histórico do Aluno

        public IEnumerable<Turma> getTurmasAvaliacoes(int cd_aluno, int cd_escola)
        {
            return turmaBusiness.getTurmasAvaliacoes(cd_aluno, cd_escola).ToList();
        }

        public IEnumerable<Estagio> getEstagiosHistoricoAluno(int cd_aluno, int cd_escola)
        {
            return turmaBusiness.getEstagiosHistoricoAluno(cd_aluno, cd_escola);
        }

        public IEnumerable<AtividadeExtra> getAtividadesAluno(SearchParameters parametros, int cd_aluno, int cd_escola)
        {
            IEnumerable<AtividadeExtra> retorno = new List<AtividadeExtra>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null || "dta_atividade_extra".Equals(parametros.sort))
                    parametros.sort = "AtividadeExtra.dt_atividade_extra";
                parametros.sort = parametros.sort.Replace("hh_inicial", "AtividadeExtra.hh_inicial");
                parametros.sort = parametros.sort.Replace("hh_final", "AtividadeExtra.hh_final");
                parametros.sort = parametros.sort.Replace("tx_obs_atividade", "tx_obs_atividade_aluno");

                retorno = daoAtividadeExtra.getAtividadesAluno(parametros, cd_aluno, cd_escola).ToList();
                transaction.Complete();
            }

            return retorno;
        }

        public EventoHistoricoUI getEventosAvaliacaoAluno(int cd_aluno, int cd_turma, int cd_escola)
        {

            EventoHistoricoUI retorno = new EventoHistoricoUI();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                List<AlunoEvento> listaAlunoEvento =
                    daoAlunoEvento.getEventosAvaliacaoAluno(cd_aluno, cd_turma, cd_escola).ToList();
                retorno.listaAlunoEvento = listaAlunoEvento;
                retorno.ultimaAulaAluno = turmaBusiness.getUltimaAulaAluno(cd_aluno, cd_turma, cd_escola);
                retorno.ultimaAulaTurma = turmaBusiness.getUltimaAulaTurma(cd_turma, cd_escola);

                transaction.Complete();
            }

            return retorno;
        }

        #endregion

        #region Evento Aluno

        public IEnumerable<AlunoEvento> getEventosAlunoByDataDesistencia(int cd_aluno, int cd_turma, int cd_escola,
            DateTime dta_Desistencia)
        {
            return daoAlunoEvento.getEventosAlunoByDataDesistencia(cd_aluno, cd_turma, cd_escola, dta_Desistencia);
        }

        public bool deleteAllAlunoEvento(List<AlunoEvento> alunoEventos)
        {
            return daoAlunoEvento.deleteAllAlunoEvento(alunoEventos);
        }

        public bool deleteAllAlunoEvento(int cd_diario, int cd_empresa)
        {
            IEnumerable<AlunoEvento> alunosEvento = daoAlunoEvento.getAllAlunosEvento(cd_diario, cd_empresa).ToList();
            if (alunosEvento != null && alunosEvento.Count() > 0)
                foreach (AlunoEvento alunoEvento in alunosEvento)
                    daoAlunoEvento.delete(alunoEvento, false);
            daoAlunoEvento.saveChanges(false);
            return true;
        }

        #endregion

        #region Contrato

        public void getVerificaReciboConfirmacao(int cd_contrato, int cd_escola)
        {
            try
            {
                //Valida se o tipoFinanceiro do contrato é diferente de cartão e cheque quando o contrato não é multiplo
                var qtdContratoNaoMultiploDiferenteCartaoCheque = financeiroBusiness.getQtdContratoNaoMultiploDiferenteCartaoCheque(cd_contrato, cd_escola);

                if (qtdContratoNaoMultiploDiferenteCartaoCheque > 0)
                {
                    throw new MatriculaBusinessException(Messages.msgErroTipoFinanDifCartaoCheque, null,
                        MatriculaBusinessException.TipoErro.ERRO_TIPO_FINANCEIRO_DIFERENTE_CARTAO_CHEQUE, false);
                }

                var qtdByContratoVerificaReciboConfirmacao = financeiroBusiness.getQtdTitulosSemBaixaTipoCartaoOuCheque(cd_contrato, cd_escola);
                if (qtdByContratoVerificaReciboConfirmacao == 0)
                {
                    throw new MatriculaBusinessException(Messages.msgErroTitulosReciboConfirmacaoNotFound, null,
                        MatriculaBusinessException.TipoErro.ERRO_TITULOS_RECIBO_CONFIRMACAO_NOT_FOUND, false);
                }

            }
            catch (Exception exe)
            {
                throw new MatriculaBusinessException(exe.Message, null,
                    MatriculaBusinessException.TipoErro.ERRO_TIPO_FINANCEIRO_DIFERENTE_CARTAO_CHEQUE, false); ;
            }
        }

        public void getVerificaReciboConfirmacaoMovimento(int cd_movimento, int cd_escola)
        {
            try
            {
                var qtdMovimentoDiferenteCartaoCheque = financeiroBusiness.getQtdMovimentoDiferenteCartaoCheque(cd_movimento, cd_escola);

                if (qtdMovimentoDiferenteCartaoCheque > 0)
                {
                    throw new MatriculaBusinessException(Messages.msgErroTipoFinanDifCartaoCheque, null,
                        MatriculaBusinessException.TipoErro.ERRO_TIPO_FINANCEIRO_DIFERENTE_CARTAO_CHEQUE, false);
                }

                var qtdByMovimentoVerificaReciboConfirmacao = financeiroBusiness.getQtdTitulosMovimentoSemBaixaTipoCartaoOuCheque(cd_movimento, cd_escola);
                if (qtdByMovimentoVerificaReciboConfirmacao == 0)
                {
                    throw new MatriculaBusinessException(Messages.msgErroTitulosReciboConfirmacaoNotFound, null,
                        MatriculaBusinessException.TipoErro.ERRO_TITULOS_RECIBO_CONFIRMACAO_NOT_FOUND, false);
                }

            }
            catch (Exception exe)
            {
                throw new MatriculaBusinessException(exe.Message, null,
                    MatriculaBusinessException.TipoErro.ERRO_TIPO_FINANCEIRO_DIFERENTE_CARTAO_CHEQUE, false); ;
            }
        }

        public ContratoUI GetMatriculaByIdPesq(int id, int cdEscola)
        {
            ContratoUI retorno = new ContratoUI();
            //Retorna o Contrato
            retorno.contrato = matriculaBusiness.GetMatriculaByIdPesq(id, cdEscola);
            if (retorno.contrato != null)
            {
                //Retorna os valores do contrato
                int duracao = 0;
                if (retorno.contrato.cd_duracao_atual.HasValue)
                    duracao = (int)retorno.contrato.cd_duracao_atual;
                int regime = 0;
                if (retorno.contrato.cd_regime_atual.HasValue)
                    regime = (int)retorno.contrato.cd_regime_atual;
                DateTime dtMatricula = new DateTime();
                if (retorno.contrato.dt_matricula_contrato.HasValue)
                    dtMatricula = Convert.ToDateTime(retorno.contrato.dt_matricula_contrato);
                retorno.valores = financeiroBusiness.getValoresForMatricula(cdEscola, retorno.contrato.cd_curso_atual,
                    duracao, regime, dtMatricula);
                //Retorna Turma, quando houver
                retorno.turmas = turmaBusiness.searchTurmasContrato(id, cdEscola, retorno.contrato.cd_aluno);
                //Retorna Taxa de Matricula, quando houver
                retorno.taxa = matriculaBusiness.searchTaxaMatricula(id, cdEscola);
                //Retorna desconto, quando houver
                retorno.descontos = matriculaBusiness.getDescontoContratoPesq(id, cdEscola);
                //Retorna cheque, quando houver
                retorno.cheque = financeiroBusiness.getChequeByContratoPesq(id);
                //Componentes 
                /*Contrato componentes = componentesNovaMatricula(retorno.contrato.cd_duracao_atual,
                    retorno.contrato.cd_produto_atual,
                    retorno.contrato.cd_regime_atual, retorno.contrato.cd_nome_contrato,
                    retorno.contrato.cd_ano_escolar,
                    null, cdEscola);*/
                //retorno.duracoes = componentes.duracoes;
                //retorno.produtos = componentes.produtos;
                //retorno.regimes = componentes.regimes;
                //retorno.nomesContrato = componentes.nomesContrato;
                //retorno.tipoLiquidacoes = componentes.tipoLiquidacoes;
                //retorno.localMovto = componentes.localMovto;
                //retorno.bancos = financeiroBusiness.getAllBanco();
                //retorno.contrato.anosEscolares = componentes.anosEscolares;
                //retorno.contrato.motivosBolsa = componentes.motivosBolsa;
                //Popula Curso
                retorno.cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_ATIVOPROD,
                    retorno.contrato.cd_curso_atual, retorno.contrato.cd_produto_atual, null);
                //Popula Aluno Turma
                if (retorno.turmas != null)
                    foreach (TurmaSearch t in retorno.turmas)
                    {
                        if (t.cd_situacao_aluno_turma == (int)SituacaoAlunoTurma.ATIVO ||
                            t.cd_situacao_aluno_turma == (int)SituacaoAlunoTurma.REMATRICULADO)
                        {
                            retorno.alunoTurma = alunoBusiness.findAlunoTurmaContrato(retorno.contrato.cd_aluno,
                                t.cd_turma, cdEscola, id);
                        }

                        if (t.cd_situacao_aluno_turma == (int)SituacaoAlunoTurma.ATIVO ||
                         t.cd_situacao_aluno_turma == (int)SituacaoAlunoTurma.AGUARDANDO ||
                            t.cd_situacao_aluno_turma == (int)SituacaoAlunoTurma.REMATRICULADO)
                        {
                            AlunoTurma alunoTurma = alunoBusiness.findAlunoTurmaContrato(retorno.contrato.cd_aluno,
                                t.cd_turma, cdEscola, id);

                            if (alunoTurma != null && alunoTurma.CursoContrato != null)
                            {
                                t.CursoContrato = preencheCursoContratoTurma(alunoTurma.CursoContrato);
                            }
                        }

                    }

                Contrato saldoM =
                    matriculaBusiness.getSaldoMatricula(id, cdEscola, (decimal)retorno.contrato.pc_desconto_bolsa);
                retorno.contrato.valorSaldoMatricula = saldoM.valorSaldoMatricula;
                retorno.contrato.qtdTitulosAbertos = saldoM.qtdTitulosAbertos;
                retorno.contrato.titulos = saldoM.titulos;
            }
            else
                throw new MatriculaBusinessException(Messages.msgRegNotEnc, null,
                    MatriculaBusinessException.TipoErro.MATRICULA_NAO_ENCONTRADA, false);

            return retorno;
        }


        public ContratoComponentesUI GetMatriculaByIdComponentesPesq(int id, int cdEscola)
        {
            ContratoComponentesUI retorno = new ContratoComponentesUI();
            //Retorna o Contrato
            retorno.contrato = matriculaBusiness.GetMatriculaByIdPesq(id, cdEscola);
            if (retorno.contrato != null)
            {
                retorno.cd_contrato = retorno.contrato.cd_contrato;
                //Componentes 
                Contrato componentes = componentesNovaMatricula(retorno.contrato.cd_duracao_atual,
                    retorno.contrato.cd_produto_atual,
                    retorno.contrato.cd_regime_atual, retorno.contrato.cd_nome_contrato,
                    retorno.contrato.cd_ano_escolar,
                    null, cdEscola);
                retorno.duracoes = componentes.duracoes.Select(x => new DuracaoUI
                {
                    cd_duracao = x.cd_duracao,
                    dc_duracao = x.dc_duracao,
                    id_duracao_ativa = x.id_duracao_ativa,
                    nm_duracao = x.nm_duracao
                }).ToList();
                retorno.produtos = componentes.produtos;
                retorno.regimes = componentes.regimes;
                retorno.nomesContrato = componentes.nomesContrato;
                retorno.tipoLiquidacoes = componentes.tipoLiquidacoes;
                retorno.localMovto = componentes.localMovto;
                //retorno.bancos = financeiroBusiness.getAllBanco().ToList();
                retorno.anosEscolares = componentes.anosEscolares;
                retorno.motivosBolsa = componentes.motivosBolsa;
                retorno.contrato = null;
            }
            else
                throw new MatriculaBusinessException(Messages.msgRegNotEnc, null,
                    MatriculaBusinessException.TipoErro.MATRICULA_NAO_ENCONTRADA, false);

            return retorno;
        }

        public ContratoComponentesBancoUI GetMatriculaComponentBancoPesq()
        {
            ContratoComponentesBancoUI retorno = new ContratoComponentesBancoUI();
            //Retorna o Contrato
            
                retorno.bancos = financeiroBusiness.getAllBanco().ToList().Select(x =>  new BancoUI
                {
                    cd_banco = x.cd_banco,
                    no_banco = x.no_banco,
                    nm_banco = x.nm_banco
                }).ToList();
                

            return retorno;
        }

        private CursoContratoSearch preencheCursoContratoTurma(CursoContrato alunoTurmaCursoContrato)
        {
            CursoContratoSearch turmaCursoContrato = new CursoContratoSearch();
            turmaCursoContrato.cd_curso_contrato = alunoTurmaCursoContrato.cd_curso_contrato;
            turmaCursoContrato.cd_contrato = alunoTurmaCursoContrato.cd_contrato;
            turmaCursoContrato.cd_curso = alunoTurmaCursoContrato.cd_curso;
            turmaCursoContrato.cd_duracao = alunoTurmaCursoContrato.cd_duracao;
            turmaCursoContrato.cd_tipo_financeiro = alunoTurmaCursoContrato.cd_tipo_financeiro;
            turmaCursoContrato.cd_pessoa_responsavel = alunoTurmaCursoContrato.cd_pessoa_responsavel;
            turmaCursoContrato.nm_dia_vcto = alunoTurmaCursoContrato.nm_dia_vcto;
            turmaCursoContrato.nm_mes_vcto = alunoTurmaCursoContrato.nm_mes_vcto;
            turmaCursoContrato.nm_ano_vcto = alunoTurmaCursoContrato.nm_ano_vcto;
            turmaCursoContrato.nm_parcelas_mensalidade = alunoTurmaCursoContrato.nm_parcelas_mensalidade;
            turmaCursoContrato.vl_curso_contrato = alunoTurmaCursoContrato.vl_curso_contrato;
            turmaCursoContrato.pc_desconto_contrato = alunoTurmaCursoContrato.pc_desconto_contrato;
            turmaCursoContrato.vl_matricula_curso = alunoTurmaCursoContrato.vl_matricula_curso;
            turmaCursoContrato.vl_parcela_contrato = alunoTurmaCursoContrato.vl_parcela_contrato;
            turmaCursoContrato.vl_desconto_contrato = alunoTurmaCursoContrato.vl_desconto_contrato;
            turmaCursoContrato.pc_responsavel_contrato = alunoTurmaCursoContrato.pc_responsavel_contrato;
            turmaCursoContrato.vl_parcela_liquida = alunoTurmaCursoContrato.vl_parcela_liquida;
            turmaCursoContrato.id_liberar_certificado = alunoTurmaCursoContrato.id_liberar_certificado;
            turmaCursoContrato.vl_curso_liquido = alunoTurmaCursoContrato.vl_curso_liquido;
            turmaCursoContrato.no_pessoa_responsavel = alunoTurmaCursoContrato.no_pessoa_responsavel;
            turmaCursoContrato.no_curso = alunoTurmaCursoContrato.no_curso;
            turmaCursoContrato.cd_proximo_curso = alunoTurmaCursoContrato.cd_proximo_curso;
            turmaCursoContrato.no_tipo_financeiro = alunoTurmaCursoContrato.no_tipo_financeiro;
            turmaCursoContrato.nm_mes_curso_inicial = alunoTurmaCursoContrato.nm_mes_curso_inicial;
            turmaCursoContrato.nm_ano_curso_inicial = alunoTurmaCursoContrato.nm_ano_curso_inicial;
            turmaCursoContrato.nm_mes_curso_final = alunoTurmaCursoContrato.nm_mes_curso_final;
            turmaCursoContrato.nm_ano_curso_final = alunoTurmaCursoContrato.nm_ano_curso_final;
            return turmaCursoContrato;
        }

        public Contrato componentesNovaMatricula(int? cdDuracao, int? cdProduto, int? cdRegime, int? cd_nome_contrato,
            int? cdAnoEscolar, int? cd_motivo_bolsa, int cdEscola)
        {
            Contrato contrato = new Contrato();
            contrato.duracoes = getDuracoes(DuracaoDataAccess.TipoConsultaDuracaoEnum.HAS_ATIVO, cdDuracao, null)
                .ToList();
            contrato.produtos = findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_ATIVO, cdProduto, null)
                .ToList();
            contrato.regimes = getRegimes(RegimeDataAccess.TipoConsultaRegimeEnum.HAS_ATIVO, cdRegime).ToList();
            contrato.nomesContrato = turmaBusiness
                .getNomesContrato(NomeContratoDataAccess.TipoConsultaNomeContratoEnum.HAS_ATIVO_MATRICULA,
                    cd_nome_contrato, cdEscola).ToList();
            contrato.localMovto = financeiroBusiness.getLocalMovtoByEscola(cdEscola, 0, true);
            contrato.bancos = financeiroBusiness.getAllBanco().ToList();
            contrato.anosEscolares = matriculaBusiness.getAnoEscolaresAtivos(cdAnoEscolar).ToList();
            contrato.motivosBolsa = matriculaBusiness.getMotivoBolsa(true, cd_motivo_bolsa).ToList();
            return contrato;
        }

        public Turma GerarTurmaFilha(Contrato contrato, int cd_turma_ppt, int cd_curso)
        {
            Turma turmaFilha = new Turma();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.COMMITED, daoEstagio.DB(),
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                List<Aluno> listPPT = new List<Aluno>();
                Turma turmaPPTPai =
                    turmaBusiness.findTurmasByIdAndCdEscola(cd_turma_ppt, contrato.cd_pessoa_escola);
                List<Turma> turmasPPTFilhas = new List<Turma>();
                if (turmaPPTPai != null)
                {
                    List<AlunoTurma> alunoPPT = new List<AlunoTurma>();
                    int[] alunos = { contrato.cd_aluno };
                    int situacaoAluno = (int)FundacaoFisk.SGF.GenericModel.AlunoTurma
                        .SituacaoAlunoTurma.Aguardando;

                    AlunoTurma alunoTurma = new AlunoTurma
                    {
                        //cd_contrato = contrato.cd_contrato,
                        cd_aluno = contrato.cd_aluno,
                        cd_turma = cd_turma_ppt,
                        cd_situacao_aluno_turma = situacaoAluno,
                        dt_inicio = contrato.dt_matricula_contrato > turmaPPTPai.dt_inicio_aula ? contrato.dt_matricula_contrato : turmaPPTPai.dt_inicio_aula,
                        id_manter_contrato = false,
                        id_renegociacao = false,
                        dt_matricula = contrato.dt_matricula_contrato,
                        nm_matricula_turma = contrato.nm_contrato,
                        id_tipo_movimento = contrato.id_tipo_matricula == 1 ?
                        Convert.ToByte(HistoricoAluno.TipoMovimento.MATRICULA) :
                        Convert.ToByte(HistoricoAluno.TipoMovimento.REMATRICULA),
                        nm_faltas = 0
                    };
                    List<Horario> horariosTurma = turmaBusiness
                        .getHorarioByEscolaForRegistro(contrato.cd_pessoa_escola, cd_turma_ppt,
                            Horario.Origem.TURMA).ToList();
                    alunoPPT.Add(alunoTurma);
                    turmaFilha = new Turma
                    {
                        alunosTurma = alunoPPT,
                        cd_curso = cd_curso,
                        cd_duracao = turmaPPTPai.cd_duracao,
                        cd_pessoa_escola = turmaPPTPai.cd_pessoa_escola,
                        cd_produto = turmaPPTPai.cd_produto,
                        cd_regime = turmaPPTPai.cd_regime,
                        cd_sala = turmaPPTPai.cd_sala,
                        cd_sala_online = turmaPPTPai.cd_sala_online,
                        dt_inicio_aula = turmaPPTPai.dt_inicio_aula,
                        horariosTurma = horariosTurma,
                        cd_turma_ppt = cd_turma_ppt
                    };
                    var nomeTurma = criarNomeTurma(false, cd_turma_ppt, turmaFilha.cd_regime,
                        turmaFilha.cd_produto, turmaFilha.cd_curso, horariosTurma,
                        turmaFilha.dt_inicio_aula);
                    int nrProximaTurma =
                        turmaBusiness.verificaExisteTurma(nomeTurma, turmaFilha.cd_pessoa_escola, 0);
                    nomeTurma = nomeTurma + "-" + nrProximaTurma;

                    turmaFilha.no_turma = nomeTurma;
                    turmaFilha.nm_turma = nrProximaTurma;

                    Turma turmaSearch = turmaBusiness.addTurma(turmaFilha);
                    turmaFilha.cd_turma = turmaSearch.cd_turma;
                    if (turmaFilha.horariosTurma != null)
                    {
                        turmaFilha.horariosTurma = Horario.clonarHorariosZerandoMemoria(
                            turmaFilha.horariosTurma.ToList(),
                            "Calendar2");
                        turmaBusiness.crudHorariosTurma(turmaFilha.horariosTurma.ToList()
                            , turmaFilha.cd_turma, turmaFilha.cd_pessoa_escola,
                            Turma.TipoTurma.NORMAL);
                    }

                    List<ProfessorTurma> professorTurmaPai = turmaBusiness
                        .findProfessoresTurmaPorTurmaEscola(cd_turma_ppt, contrato.cd_pessoa_escola)
                        .ToList();
                    if (professorTurmaPai != null && professorTurmaPai.Count() > 0)
                        turmaBusiness.crudProfessoresTurma(
                            ProfessorTurma.clonarProfessorZerandoMemoria(professorTurmaPai,
                                turmaFilha.cd_turma), turmaFilha, turmaFilha.horariosTurma, false);

                    alunoTurma.cd_turma = turmaSearch.cd_turma;

                    alunoBusiness.addAlunoTurma(alunoTurma);
                    TurmaEscola novaTurmaEscola = new TurmaEscola();
                    if (turmaFilha.cd_sala_online != null && (contrato.cd_pessoa_escola != turmaFilha.cd_pessoa_escola))
                    {
                        novaTurmaEscola.cd_turma = turmaSearch.cd_turma;
                        novaTurmaEscola.cd_escola = contrato.cd_pessoa_escola;
                        daoTurmaEscola.add(novaTurmaEscola, false);

                    }
                }
                transaction.Complete();
            }

            return turmaFilha;
        }

        #endregion

        #region Carnê

        public List<CarneUI> getCarnePorContrato(int cdContrato, int cdEscola, Parametro parametro, bool contaSegura,
            int parcIniCarne, int parcFimCarne)
        {
            List<CarneUI> listaCarne = new List<CarneUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                listaCarne = financeiroBusiness.getCarnePorContrato(cdContrato, cdEscola, parcIniCarne, parcFimCarne).ToList();
                foreach (CarneUI carne in listaCarne)
                {
                    carne.cd_pessoa_empresa = cdEscola;
                    carne.conta_segura = contaSegura;
                    if (parametro.pc_multa.HasValue)
                        carne.vl_multa = ((float)carne.vl_titulo / 100) * (float)parametro.pc_multa.Value;
                    if (parametro.pc_juros_dia.HasValue)
                        carne.val_acr = (((float)carne.vl_titulo / 100) * (float)parametro.pc_juros_dia.Value);
                    if (parametro.nm_dias_carencia.HasValue)
                        carne.nm_dias_carencia = parametro.nm_dias_carencia.Value;
                }

                transaction.Complete();
            }

            return listaCarne;
        }

        public List<CarneUI> getCarnePorMovimentos(int cdMovimento, int cdEscola, Parametro parametro, bool contaSegura)
        {
            List<CarneUI> listaCarne = new List<CarneUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                listaCarne = financeiroBusiness.getCarnePorMovimentos(cdMovimento, cdEscola).ToList();
                List<Titulo> titulos = financeiroBusiness.getTituloBaixaFinanSimulacao(
                    listaCarne.Select(x => x.cd_titulo).ToList(), cdEscola, null,
                    TituloDataAccess.TipoConsultaTituloEnum.HAS_SIMULACAO);
                foreach (CarneUI carne in listaCarne)
                {
                    BaixaTitulo baixa = new BaixaTitulo();
                    Titulo titulo = titulos.Where(x => x.cd_titulo == carne.cd_titulo).FirstOrDefault();
                    if (titulo != null && titulo.cd_titulo > 0)
                    {
                        baixa.dt_baixa_titulo = DateTime.Now.Date;
                        this.simularBaixaTitulo(titulo, ref baixa, parametro, cdEscola, contaSegura, true);
                        if (parametro.pc_multa.HasValue)
                            carne.vl_multa = ((float)carne.vl_titulo / 100) * (float)parametro.pc_multa.Value;
                        if (parametro.pc_juros_dia.HasValue)
                            carne.val_acr = (((float)carne.vl_titulo / 100) * (float)parametro.pc_juros_dia.Value);
                        //Caso tenha carência informada nos parâmetros, considerar na regras de datas do relatório.
                        carne.nm_dias_carencia = parametro.nm_dias_carencia != null
                            ? (byte)parametro.nm_dias_carencia
                            : (byte)0;
                    }
                }

                transaction.Complete();
            }

            return listaCarne;
        }

        #endregion

        #region Aula Personalizada

        public AulaPersonalizada getPesqAulaPersonalizada(int cdEscola)
        {
            AulaPersonalizada retorno = new AulaPersonalizada();
            retorno.produtos =
                findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_AULA_PERSONALIZADA, null, cdEscola).ToList();
            retorno.salas = findListSalasAulaPer(cdEscola).ToList();
            retorno.professores = turmaBusiness.getFuncionariosByAulaPers(cdEscola).ToList();
            return retorno;
        }

        public IEnumerable<AulaPersonalizadaUI> searchAulaPersonalizada(SearchParameters parametros, DateTime? dataIni,
            DateTime? dataFim, TimeSpan? hrInicial, TimeSpan? hrFinal, int? cdProduto, int? cdProfessor,
            int? cdSala, int? cdAluno, bool participou, int cdEscola)
        {
            IEnumerable<AulaPersonalizadaUI> retorno = new List<AulaPersonalizadaUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "cd_aula_personalizada";
                parametros.sort = parametros.sort.Replace("dta_aula_personalizada", "dt_aula_personalizada");
                parametros.sort = parametros.sort.Replace("participou", "id_participou");
                retorno = daoAulaPersonalizada.searchAulaPersonalizada(parametros, dataIni, dataFim, hrInicial, hrFinal,
                    cdProduto, cdProfessor, cdSala, cdAluno, participou, cdEscola).ToList();
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<AulaPersonalizada> searchAulaPersonalizadaPesq(int cdAulaPersonalizada, int cdEscola)
        {
            return daoAulaPersonalizada.searchAulaPersonalizadaPesq(cdAulaPersonalizada, cdEscola);
        }

        public IEnumerable<AulaPersonalizada> addAulaPersonalizada(AulaPersonalizada aulaPersonalizada)
        {

            validaHorarioTurma(aulaPersonalizada);
            AulaPersonalizada aulaPersonalizadaBD = new AulaPersonalizada();
            IEnumerable<AulaPersonalizada> aulaPer = new List<AulaPersonalizada>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (aulaPersonalizada.AulaPersonalizadaAlunos == null ||
                    aulaPersonalizada.AulaPersonalizadaAlunos.Count() <= 0)
                    throw new CoordenacaoBusinessException(Messages.msgErroAulaPerSemAluno, null,
                        CoordenacaoBusinessException.TipoErro.ERRO_AULA_PERSONALIZADA_SEM_ALUNO, false);

                aulaPersonalizada.AulaPersonalizadaAlunos = lancarAula(aulaPersonalizada).ToList();
                aulaPersonalizadaBD = daoAulaPersonalizada.add(aulaPersonalizada, false);
                transaction.Complete();
            }

            aulaPer = searchAulaPersonalizadaPesq(aulaPersonalizadaBD.cd_aula_personalizada,
                aulaPersonalizadaBD.cd_escola);
            return aulaPer;
        }

        private void validaHorarioTurma(AulaPersonalizada aulaPersonalizada)
        {
            if (aulaPersonalizada.AulaPersonalizadaAlunos.Where(x =>
                    x.id_aula_dada == true && x.hh_inicial_aluno == null && x.hh_final_aluno == null).Any() ||
                aulaPersonalizada.AulaPersonalizadaAlunos
                    .Where(x => x.id_aula_dada == true && x.hh_inicial_aluno == null).Any() ||
                aulaPersonalizada.AulaPersonalizadaAlunos.Where(x => x.id_aula_dada == true && x.hh_final_aluno == null)
                    .Any())
                throw new CoordenacaoBusinessException(Messages.msgErroNaoInformadoHrInicialFinal, null,
                    CoordenacaoBusinessException.TipoErro.ERRO_NAO_INFORMADO_HR_INICIAL_FINAL, false);

            if (aulaPersonalizada.AulaPersonalizadaAlunos
                .Where(x => x.id_aula_dada == true && x.hh_inicial_aluno > x.hh_final_aluno).Any())
                throw new CoordenacaoBusinessException(Messages.msgErroMinHrInicialFinal, null,
                    CoordenacaoBusinessException.TipoErro.ERRO_MIN_HR_INICIAL_FINAL, false);

            if (aulaPersonalizada.Turma != null && aulaPersonalizada.Turma.horariosTurma != null)
            {

                foreach (var aulaPer in aulaPersonalizada.AulaPersonalizadaAlunos)
                {
                    if ((aulaPer.hh_inicial_aluno != null && aulaPer.hh_final_aluno != null) && !aulaPersonalizada.Turma
                            .horariosTurma.Where(x =>
                                aulaPer.hh_inicial_aluno.Value >= x.dt_hora_ini &&
                                aulaPer.hh_final_aluno.Value <= x.dt_hora_fim).Any())
                        throw new CoordenacaoBusinessException(Messages.msgErroIntervaloHrInicialFinal, null,
                            CoordenacaoBusinessException.TipoErro.ERRO_INTERVALO_HR_INICIAL_FINAL, false);
                }
            }

            aulaPersonalizada.Turma = null;
        }

        public AulaPersonalizada searchAulaPersonalizadaById(int cdAulaPersonalizada, int cdEscola, int cd_aluno)
        {
            AulaPersonalizada aulaPer = daoAulaPersonalizada.searchAulaPersonalizadaById(cdAulaPersonalizada, cdEscola);
            aulaPer.AulaPersonalizadaAlunos = daoAulaPersonalizadaAluno
                .searchAulaPersonalizadaAlunoByAulaAluno(cdAulaPersonalizada, cdEscola, cd_aluno).ToList();
            return aulaPer;
        }

        private void persistirAulaPersonalizada(AulaPersonalizada aulaPersonalizada, int cdEscola)
        {
            //Pega as aulas personalizadas do aluno na base de dados
            IEnumerable<AulaPersonalizadaAluno> aulaPersonalizadaAluno =
                daoAulaPersonalizadaAluno.searchAulaPersonalizadaAlunoByAula(aulaPersonalizada.cd_aula_personalizada,
                    cdEscola);
            //Pega as aulas personalizadas da view
            IEnumerable<AulaPersonalizadaAluno> aulaPersonalizadaAlunoView = new List<AulaPersonalizadaAluno>();
            aulaPersonalizadaAlunoView = aulaPersonalizada.AulaPersonalizadaAlunos;

            if (aulaPersonalizadaAlunoView != null)
            {
                //Incluir aulas personalizadas do aluno
                foreach (AulaPersonalizadaAluno atividadeAluno in aulaPersonalizadaAlunoView)
                {
                    atividadeAluno.cd_aula_personalizada = aulaPersonalizada.cd_aula_personalizada;
                    if (atividadeAluno.cd_aula_personalizada_aluno == 0)
                        daoAulaPersonalizadaAluno.add(atividadeAluno, false);
                }

                // Filtra as aulas personalizadas  aluno que possuem código
                IEnumerable<AulaPersonalizadaAluno> atividadesComCodigo =
                    aulaPersonalizadaAlunoView.Where(at => at.cd_aula_personalizada_aluno != 0);
                IEnumerable<AulaPersonalizadaAluno> atividadesDeleted = aulaPersonalizadaAluno.Where(atv =>
                    !atividadesComCodigo.Any(a => atv.cd_aula_personalizada_aluno == a.cd_aula_personalizada_aluno));

                //Deleta o registro que esta na base mas não esta na view.
                if (atividadesDeleted.Count() > 0)
                {
                    foreach (AulaPersonalizadaAluno item in atividadesDeleted)
                    {
                        var deletarAtividadeAluno =
                            daoAulaPersonalizadaAluno.findById(item.cd_aula_personalizada_aluno, false);
                        //Deletar diário de aula quando a aula foi lançada
                        if (deletarAtividadeAluno != null)
                        {
                            int cd_diario_aula =
                                deletarAtividadeAluno.cd_diario_aula.HasValue &&
                                deletarAtividadeAluno.cd_diario_aula.Value > 0
                                    ? deletarAtividadeAluno.cd_diario_aula.Value
                                    : 0;
                            daoAulaPersonalizadaAluno.delete(deletarAtividadeAluno, false);
                            if (cd_diario_aula > 0)
                                deleteDiarios(new int[] { cd_diario_aula }, cdEscola);
                        }
                    }
                }

                //Edita os registros da aula personalizada
                foreach (AulaPersonalizadaAluno colecaoAlunosVI in aulaPersonalizadaAlunoView)
                {
                    foreach (AulaPersonalizadaAluno atividadesBD in aulaPersonalizadaAluno)
                    {
                        if (colecaoAlunosVI.cd_aula_personalizada_aluno == atividadesBD.cd_aula_personalizada_aluno)
                        {
                            AulaPersonalizadaAluno atividadesAlunoBD =
                                daoAulaPersonalizadaAluno.findById(atividadesBD.cd_aula_personalizada_aluno, false);
                            atividadesAlunoBD.cd_diario_aula =
                                atividadesAlunoBD.cd_diario_aula == colecaoAlunosVI.cd_diario_aula
                                    ? atividadesAlunoBD.cd_diario_aula
                                    : colecaoAlunosVI.cd_diario_aula;
                            //if (atividadesAlunoBD.cd_diario_aula.HasValue && atividadesAlunoBD.tx_obs_aula != colecaoAlunosVI.tx_obs_aula)
                            if (atividadesAlunoBD.cd_diario_aula.HasValue)
                            {
                                DiarioAula diarioAula = new DiarioAula
                                {
                                    cd_diario_aula = atividadesAlunoBD.cd_diario_aula.Value,
                                    tx_obs_aula = colecaoAlunosVI.tx_obs_aula,
                                    hr_inicial_aula = colecaoAlunosVI.hh_inicial_aluno.Value,
                                    hr_final_aula = colecaoAlunosVI.hh_final_aluno.Value,
                                    cd_pessoa_empresa = cdEscola
                                };
                                editObsDiarioAula(diarioAula);
                            }

                            atividadesAlunoBD.id_aula_dada = colecaoAlunosVI.id_aula_dada;
                            atividadesAlunoBD.tx_obs_aula = colecaoAlunosVI.tx_obs_aula;
                            atividadesAlunoBD.hh_inicial_aluno = colecaoAlunosVI.hh_inicial_aluno;
                            atividadesAlunoBD.hh_final_aluno = colecaoAlunosVI.hh_final_aluno;
                        }
                    }
                }
            }
            else
            {
                if (aulaPersonalizadaAluno != null)
                {
                    foreach (AulaPersonalizadaAluno item in aulaPersonalizadaAluno)
                    {
                        AulaPersonalizadaAluno deletarAulaAluno =
                            daoAulaPersonalizadaAluno.findById(item.cd_aula_personalizada_aluno, false);
                        if (deletarAulaAluno != null)
                        {
                            int cd_diario_aula =
                                deletarAulaAluno.cd_diario_aula.HasValue && deletarAulaAluno.cd_diario_aula.Value > 0
                                    ? deletarAulaAluno.cd_diario_aula.Value
                                    : 0;
                            daoAulaPersonalizadaAluno.delete(deletarAulaAluno, false);
                            //Deletar diário de aula quando a aula foi lançada
                            if (cd_diario_aula > 0)
                                deleteDiarios(new int[] { cd_diario_aula }, cdEscola);
                        }
                    }
                }
            }

            daoAulaPersonalizadaAluno.saveChanges(false);
        }

        public IEnumerable<AulaPersonalizada> editAulaPersonalizada(AulaPersonalizada aulaPersonalizada)
        {
            validaHorarioTurma(aulaPersonalizada);

            IEnumerable<AulaPersonalizada> aulaPer = new List<AulaPersonalizada>();
            sincronizarContextos(daoAulaPersonalizada.DB());
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (aulaPersonalizada.AulaPersonalizadaAlunos == null ||
                    aulaPersonalizada.AulaPersonalizadaAlunos.Count() <= 0)
                    throw new CoordenacaoBusinessException(Messages.msgErroAulaPerSemAluno, null,
                        CoordenacaoBusinessException.TipoErro.ERRO_AULA_PERSONALIZADA_SEM_ALUNO, false);

                AulaPersonalizada aula = daoAulaPersonalizada.findById(aulaPersonalizada.cd_aula_personalizada, false);
                aulaPersonalizada.AulaPersonalizadaAlunos = lancarAula(aulaPersonalizada).ToList();

                persistirAulaPersonalizada(aulaPersonalizada, aulaPersonalizada.cd_escola);
                aulaPersonalizada.AulaPersonalizadaAlunos = null;
                aula.copy(aulaPersonalizada);

                daoAulaPersonalizada.saveChanges(false);

                aulaPer = searchAulaPersonalizadaPesq(aula.cd_aula_personalizada, aula.cd_escola);
                transaction.Complete();
            }

            return aulaPer;
        }

        public IEnumerable<AulaPersonalizadaAluno> lancarAula(AulaPersonalizada aulaPer)
        {
            IEnumerable<AulaPersonalizadaAluno> alunos = new List<AulaPersonalizadaAluno>();
            if (aulaPer.AulaPersonalizadaAlunos != null && aulaPer.AulaPersonalizadaAlunos.Count() > 0)
            {
                DiarioAula diarioAula = new DiarioAula();
                List<vi_diario_aula> diariosLancados = new List<vi_diario_aula>();
                foreach (AulaPersonalizadaAluno item in aulaPer.AulaPersonalizadaAlunos)
                {

                    //Verifica interseção de horários
                    if (item.id_aula_dada)
                    {
                        if (!turmaBusiness.verificaIntersecaoTurmaPersonalizada(item.cd_turma,
                            item.cd_programacao_turma, item.hh_inicial_aluno.Value, item.hh_final_aluno.Value,
                            aulaPer.dt_aula_personalizada.Date))
                            throw new CoordenacaoBusinessException(Messages.msgIntersecaoHorariosDiarioAula, null,
                                CoordenacaoBusinessException.TipoErro.INTERSECAO_HORARIOS_DIARIO_AULA, false);
                    }

                    if (item.id_aula_dada && (!item.cd_diario_aula.HasValue || item.cd_diario_aula == 0))
                    {
                        //Verifica se a aula não foi lançada                         
                        if (turmaBusiness.verificaExisteDiarioEfetivoProgramacaoTurma(item.cd_turma, aulaPer.cd_escola,
                            item.cd_programacao_turma))
                            throw new CoordenacaoBusinessException(Messages.msgExistDiarioParaEssaProgramacao, null,
                                CoordenacaoBusinessException.TipoErro.INTERSECAO_HORARIOS_DIARIO_AULA, false);

                        diarioAula = new DiarioAula
                        {
                            cd_programacao_turma = item.cd_programacao_turma,
                            cd_turma = item.cd_turma,
                            cd_professor = item.cd_professor,
                            cd_sala = item.cd_sala_prog,
                            nm_aula_turma = item.nm_aula_programacao_turma,
                            tx_obs_aula = item.tx_obs_aula,
                            cd_pessoa_empresa = aulaPer.cd_escola,
                            cd_tipo_aula = 1,
                            cd_professor_substituto = null,
                            cd_motivo_falta = null,
                            cd_avaliacao = null,
                            dt_aula = aulaPer.dt_aula_personalizada,
                            hr_inicial_aula = item.hh_inicial_aluno.Value,
                            hr_final_aula = item.hh_final_aluno.Value,
                            cd_usuario = aulaPer.cd_usuario,
                            dt_cadastro_aula = aulaPer.dt_cadastro_aula,
                            id_aula_externa = false,
                            falta_professor = false,
                            falta_justificada = false,
                            dc_obs_falta = null
                        };

                        vi_diario_aula diarioNew = addDiarioAula(diarioAula, true);
                        item.cd_diario_aula = diarioNew.cd_diario_aula;
                    }
                }

                alunos = aulaPer.AulaPersonalizadaAlunos;
            }

            return alunos;
        }

        public bool deleteAllAulaPersonalizada(List<AulaPersonalizada> aulasPersonalizadas, int cdEscola)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (var item in aulasPersonalizadas)
                {
                    IEnumerable<AulaPersonalizadaAluno> alunos =
                        daoAulaPersonalizadaAluno.searchAulaPersonalizadaAlunoByAula(item.cd_aula_personalizada,
                            cdEscola);
                    if (alunos.Count() > 0)
                        foreach (AulaPersonalizadaAluno a in alunos)
                        {
                            int cd_diario_aula = a.cd_diario_aula.HasValue && a.cd_diario_aula.Value > 0
                                ? a.cd_diario_aula.Value
                                : 0;
                            AulaPersonalizadaAluno aula =
                                daoAulaPersonalizadaAluno.findById(a.cd_aula_personalizada_aluno, false);
                            daoAulaPersonalizadaAluno.delete(aula, false);
                            if (cd_diario_aula > 0)
                                deleteDiarios(new int[] { cd_diario_aula }, cdEscola);

                        }

                    var aulaPersonalizada = daoAulaPersonalizada.findById(item.cd_aula_personalizada, false);
                    if (aulaPersonalizada != null)
                        deleted = daoAulaPersonalizada.delete(aulaPersonalizada, false);
                }

                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<AulaPersonalizadaReport> getReportAulaPersonalizada(int cd_empresa, int cd_aluno,
            int? cd_produto, int? cd_curso, DateTime? dt_inicial_agend, DateTime? dt_final_agend,
            DateTime? dt_inicial_lanc, DateTime? dt_final_lanc, TimeSpan? hr_inicial_agend, TimeSpan? hr_final_agend,
            TimeSpan? hr_inicial_lanc, TimeSpan? hr_final_lanc)
        {
            IEnumerable<AulaPersonalizadaReport> retorno = new List<AulaPersonalizadaReport>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED,
                    daoAulaPersonalizada.DB()))
            {
                retorno = daoAulaPersonalizada.getReportAulaPersonalizada(cd_empresa, cd_aluno, cd_produto, cd_curso,
                    dt_inicial_agend, dt_final_agend, dt_inicial_lanc,
                    dt_final_lanc, hr_inicial_agend, hr_final_agend, hr_inicial_lanc, hr_final_lanc);
                transaction.Complete();
            }

            return retorno;
        }

        #endregion

        #region Avaliação Participação

        public IEnumerable<AvaliacaoParticipacaoUI> searchAvaliacaoParticipacao(
            Componentes.Utils.SearchParameters parametros, int cdCriterio, int cdParticipacao, int cdProduto,
            bool? ativo, int cdEscola)
        {
            IEnumerable<AvaliacaoParticipacaoUI> retorno = new List<AvaliacaoParticipacaoUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                //if (parametros.sort == null)
                //    parametros.sort = "dc_criterio_avaliacao";
                //parametros.sort = parametros.sort.Replace("dc_criterio_avaliacao", "CriterioAvaliacao.dc_criterio_avaliacao");
                //parametros.sort = parametros.sort.Replace("no_produto", "no_produto");


                retorno = daoAvaliacaoParticipacao.searchAvaliacaoParticipacao(parametros, cdCriterio, cdParticipacao,
                    cdProduto, ativo, cdEscola);
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<AvaliacaoParticipacao> addAvaliacaoParticipacao(AvaliacaoParticipacao avalPart, int cdEscola)
        {
            IEnumerable<AvaliacaoParticipacao> avaliacaoPart = new List<AvaliacaoParticipacao>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                int qtdPart =
                    avalPart.AvaliacaoParticipacaoVinc == null || avalPart.AvaliacaoParticipacaoVinc.Count() == 0
                        ? 0
                        : avalPart.AvaliacaoParticipacaoVinc.Where(x => x.id_avaliacao_participacao_ativa == true)
                            .Count();
                verificaQtdParticipacao(avalPart.cd_produto, qtdPart);
                List<AvaliacaoParticipacaoVinc> participacoes = avalPart.AvaliacaoParticipacaoVinc.ToList();
                if (avalPart.AvaliacaoParticipacaoVinc.Count() > 0)
                    foreach (AvaliacaoParticipacaoVinc a in participacoes)
                    {
                        a.cd_escola = cdEscola;
                    }

                AvaliacaoParticipacao addAval = daoAvaliacaoParticipacao.add(avalPart, false);
                avaliacaoPart =
                    daoAvaliacaoParticipacao.getAvaliacaoParticipacaoById(addAval.cd_avaliacao_participacao, cdEscola);
                transaction.Complete();
            }

            return avaliacaoPart;
        }

        public AvaliacaoParticipacao getAvaliacaoParticipacaoByEdit(int cdAvalPart, int cdEscola)
        {
            AvaliacaoParticipacao avaliacaoPart = new AvaliacaoParticipacao();
            avaliacaoPart = daoAvaliacaoParticipacao.getAvaliacaoParticipacaoByEditar(cdAvalPart, cdEscola);
            avaliacaoPart.AvaliacaoParticipacaoVinc =
                daoAvaliacaoParticipacaoVinc.getAvalPartVinc(cdAvalPart, cdEscola).ToList();
            return avaliacaoPart;
        }

        public IEnumerable<AvaliacaoParticipacao> editAvaliacaoParticipacao(AvaliacaoParticipacao avaliacaoParticipacao,
            int cdEscola)
        {
            IEnumerable<AvaliacaoParticipacao> avaliacaoPart = new List<AvaliacaoParticipacao>();
            sincronizarContextos(daoAvaliacaoParticipacao.DB());
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                int qtdPart =
                    avaliacaoParticipacao.AvaliacaoParticipacaoVinc == null ||
                    avaliacaoParticipacao.AvaliacaoParticipacaoVinc.Count() == 0
                        ? 0
                        : avaliacaoParticipacao.AvaliacaoParticipacaoVinc
                            .Where(x => x.id_avaliacao_participacao_ativa == true).Count();
                verificaQtdParticipacao(avaliacaoParticipacao.cd_produto, qtdPart);
                AvaliacaoParticipacao avalPart =
                    daoAvaliacaoParticipacao.findById(avaliacaoParticipacao.cd_avaliacao_participacao, false);

                persistirAvaliacaoParticipacaoVinc(avaliacaoParticipacao, cdEscola);
                avaliacaoParticipacao.AvaliacaoParticipacaoVinc = null;
                avalPart.copy(avaliacaoParticipacao);

                daoAvaliacaoParticipacao.saveChanges(false);

                avaliacaoPart =
                    daoAvaliacaoParticipacao.getAvaliacaoParticipacaoById(
                        avaliacaoParticipacao.cd_avaliacao_participacao, cdEscola);
                transaction.Complete();
            }

            return avaliacaoPart;
        }


        public bool deleteAllAvaliacaoParticipacao(List<AvaliacaoParticipacao> avaliacoesPart, int cdEscola)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (var item in avaliacoesPart)
                {
                    IEnumerable<AvaliacaoParticipacaoVinc> participacoes =
                        daoAvaliacaoParticipacaoVinc.getAvalPartVinc(item.cd_avaliacao_participacao, cdEscola);
                    if (participacoes.Count() > 0)
                        foreach (AvaliacaoParticipacaoVinc a in participacoes)
                        {
                            AvaliacaoParticipacaoVinc part =
                                daoAvaliacaoParticipacaoVinc.findById(a.cd_avaliacao_participacao_vinc, false);
                            daoAvaliacaoParticipacaoVinc.delete(part, false);
                        }

                    AvaliacaoParticipacao avaliacaoP =
                        daoAvaliacaoParticipacao.findById(item.cd_avaliacao_participacao, false);
                    if (avaliacaoP != null)
                        deleted = daoAvaliacaoParticipacao.delete(avaliacaoP, false);
                }

                transaction.Complete();
            }

            return deleted;
        }

        public void verificaQtdParticipacao(int cdProduto, int qntParticipacao)
        {
            if (qntParticipacao == 0)
                throw new CoordenacaoBusinessException(Messages.msgErroAvalPartSemVinculo, null,
                    CoordenacaoBusinessException.TipoErro.ERRO_AVALIACAO_PARTICIPACAO_SEM_VINC, false);
            double maiorNotaParticipacao = daoConceito.somaParticipacaoPorConceito(cdProduto, null);
            int qtdParticipacaoNecessaria = 4;
            if (maiorNotaParticipacao > 0)
                //    if ((maiorNotaParticipacao * qntParticipacao) != 10)
                qtdParticipacaoNecessaria = (int)(10 / maiorNotaParticipacao);

            if (qntParticipacao != qtdParticipacaoNecessaria)
                throw new CoordenacaoBusinessException(
                    string.Format(Messages.msgErroQntParticipObrigatoria, qtdParticipacaoNecessaria), null,
                    CoordenacaoBusinessException.TipoErro.ERRO_AVALIACAO_PARTICIPACAO_NOTA, false);
        }

        #endregion

        #region Avaliação Participação Vinculo

        public int verificaMaiorQntParticipacao(int idProduto, int cdEscola)
        {
            return daoAvaliacaoParticipacaoVinc.verificaMaiorQntParticipacao(idProduto, cdEscola);
        }


        private void persistirAvaliacaoParticipacaoVinc(AvaliacaoParticipacao avaliacaoParticipacao, int cdEscola)
        {
            //Pega as Avaliações Participação Viculadas na base de dados
            IEnumerable<AvaliacaoParticipacaoVinc> avaliacaoParticipacaoVinc =
                daoAvaliacaoParticipacaoVinc.getAvalPartVinc(avaliacaoParticipacao.cd_avaliacao_participacao, cdEscola);
            //Pega as aulas personalizadas da view
            IEnumerable<AvaliacaoParticipacaoVinc>
                avaliacaoParticipacaoVincView = new List<AvaliacaoParticipacaoVinc>();
            avaliacaoParticipacaoVincView = avaliacaoParticipacao.AvaliacaoParticipacaoVinc;

            if (avaliacaoParticipacaoVincView != null)
            {
                //Incluir participações a avaliação X participação
                foreach (AvaliacaoParticipacaoVinc participacaoVinc in avaliacaoParticipacaoVincView)
                {
                    participacaoVinc.cd_avaliacao_participacao = avaliacaoParticipacao.cd_avaliacao_participacao;
                    participacaoVinc.cd_escola = cdEscola;
                    //participacaoVinc.id_avaliacao_participacao_ativa = avaliacaoParticipacao.id_ativa;
                    if (participacaoVinc.cd_avaliacao_participacao_vinc == 0)
                        daoAvaliacaoParticipacaoVinc.add(participacaoVinc, false);
                }

                // Filtra as participações  avaliação X participação que possuem código
                IEnumerable<AvaliacaoParticipacaoVinc> participacoesComCodigo =
                    avaliacaoParticipacaoVincView.Where(at => at.cd_avaliacao_participacao_vinc != 0);
                IEnumerable<AvaliacaoParticipacaoVinc> participacoesDeleted = avaliacaoParticipacaoVinc.Where(atv =>
                    !participacoesComCodigo.Any(a =>
                        atv.cd_avaliacao_participacao_vinc == a.cd_avaliacao_participacao_vinc));

                //Deleta o registro que esta na base mas não esta na view.
                if (participacoesDeleted.Count() > 0)
                {
                    foreach (AvaliacaoParticipacaoVinc item in participacoesDeleted)
                    {
                        AvaliacaoParticipacaoVinc deletarAvaliacaoParticipacaoVinc =
                            daoAvaliacaoParticipacaoVinc.findById(item.cd_avaliacao_participacao_vinc, false);
                        if (deletarAvaliacaoParticipacaoVinc != null)
                        {
                            var alunoPart =
                                daoAvaliacaoAlunoParticipacao.verificaParticipacaoAvaliacao(
                                    item.cd_participacao_avaliacao);
                            if (alunoPart && !avaliacaoParticipacaoVincView.Any(vinc =>
                                    vinc.cd_avaliacao_participacao_vinc == item.cd_avaliacao_participacao_vinc))
                                throw new CoordenacaoBusinessException(Messages.msgErroDeletarAvaliacaoParticipacao,
                                    null, CoordenacaoBusinessException.TipoErro.ERRO_DELETAR_AVALIACAO_PARTICIPACAO,
                                    false);

                            daoAvaliacaoParticipacaoVinc.delete(deletarAvaliacaoParticipacaoVinc, false);
                        }
                    }
                }
                //Edita os registros do vinculo avaliação X participação

                foreach (AvaliacaoParticipacaoVinc partVincBD in avaliacaoParticipacaoVinc)
                {
                    AvaliacaoParticipacaoVinc participacaoVincBD =
                        daoAvaliacaoParticipacaoVinc.findById(partVincBD.cd_avaliacao_participacao_vinc, false);
                    if (participacaoVincBD != null)
                    {
                        AvaliacaoParticipacaoVinc avPartView = avaliacaoParticipacaoVincView.Where(x =>
                                x.cd_avaliacao_participacao_vinc == partVincBD.cd_avaliacao_participacao_vinc)
                            .FirstOrDefault();
                        participacaoVincBD.id_avaliacao_participacao_ativa = avPartView.id_avaliacao_participacao_ativa;
                        participacaoVincBD.nm_ordem = avPartView.nm_ordem;
                    }
                }
            }
            else
            {
                if (avaliacaoParticipacaoVinc != null)
                {
                    foreach (AvaliacaoParticipacaoVinc item in avaliacaoParticipacaoVinc)
                    {
                        AvaliacaoParticipacaoVinc deletarAvaliacaoParticipacaoVinc =
                            daoAvaliacaoParticipacaoVinc.findById(item.cd_avaliacao_participacao_vinc, false);
                        if (deletarAvaliacaoParticipacaoVinc != null)
                        {
                            daoAvaliacaoParticipacaoVinc.delete(deletarAvaliacaoParticipacaoVinc, false);
                        }
                    }
                }
            }

            daoAvaliacaoParticipacaoVinc.saveChanges(false);
        }

        #endregion

        #region Participação

        public List<Participacao> getParticipacaoAvaliacaoPart(int cdEscola)
        {
            return daoParticipacao.getParticipacaoAvaliacaoPart(cdEscola);
        }

        public List<Participacao> getParticipacaoByAvaliacao(int cdAvalPart, int cdEscola)
        {
            return daoParticipacao.getParticipacaoByAvaliacao(cdAvalPart, cdEscola);
        }

        public List<Participacao> getParticipacoes(string cdsPart)
        {
            List<int> cdsParticipacao = new List<int>();

            if (cdsPart != null && cdsPart != "")
            {
                string[] codigos = cdsPart.Split('|');
                for (int i = 0; i < codigos.Count(); i++)
                    cdsParticipacao.Add(Int32.Parse(codigos[i]));
            }

            return daoParticipacao.getParticipacoes(cdsParticipacao);
        }

        public IEnumerable<Participacao> getParticipacaoSearch(SearchParameters parametros, string desc, bool inicio,
            bool? status)
        {
            IEnumerable<Participacao> retorno = new List<Participacao>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_participacao";
                parametros.sort = parametros.sort.Replace("participacao_ativa", "id_participacao_ativa");
                retorno = daoParticipacao.getParticipacaoSearch(parametros, desc, inicio, status);
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<GenericModel.Participacao> findAllParticipacao()
        {
            return daoParticipacao.findAll(false).ToList();
        }

        public Participacao findByIdParticipacao(int id)
        {
            return daoParticipacao.findById(id, false);
        }

        public Participacao addParticipacao(Participacao entity)
        {
            return daoParticipacao.add(entity, false);
        }

        public Participacao editParticipacao(Participacao entity)
        {
            if (entity.cd_participacao >= 1 && entity.cd_participacao <= 2)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp,
                    null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            return daoParticipacao.edit(entity, false);
        }

        public bool deleteParticipacao(Participacao entity)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (entity.cd_participacao >= 1 && entity.cd_participacao <= 2)
                    throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp,
                        null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                Participacao participacao = daoParticipacao.findById(entity.cd_participacao, false);
                deleted = daoParticipacao.delete(participacao, false);
                transaction.Complete();
            }

            return deleted;
        }

        public bool deleteAllParticipacao(List<Participacao> participacoes)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (Participacao e in participacoes)
                    if (e.cd_participacao >= 1 && e.cd_participacao <= 2)
                        throw new RegistroProprietarioBusinessException(
                            Componentes.Utils.Messages.Messages.msgErroRegProp, null,
                            RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                foreach (Participacao part in participacoes)
                {
                    Participacao partDel = daoParticipacao.findById(part.cd_participacao, false);
                    deleted = daoParticipacao.delete(partDel, false);
                }

                transaction.Complete();
            }

            return deleted;
        }

        #endregion

        #region Nível

        public List<Nivel> getNiveis(string cdsNiv)
        {
            List<int> cdsNivel = new List<int>();

            if (cdsNiv != null && cdsNiv != "")
            {
                string[] codigos = cdsNiv.Split('|');
                for (int i = 0; i < codigos.Count(); i++)
                    cdsNivel.Add(Int32.Parse(codigos[i]));
            }

            return daoNivel.getNiveis(cdsNivel);
        }

        public IEnumerable<Nivel> getNivelSearch(SearchParameters parametros, string desc, bool inicio, bool? status)
        {
            IEnumerable<Nivel> retorno = new List<Nivel>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "nm_ordem";
                parametros.sort = parametros.sort.Replace("nivelAtivo", "id_ativo");
                retorno = daoNivel.getNivelSearch(parametros, desc, inicio, status);
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<Nivel> findNivel(NivelDataAccess.TipoConsultaNivelEnum hasDependente)
        {
            IEnumerable<Nivel> retorno = new List<Nivel>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoNivel.findNivel(hasDependente).ToList();
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<GenericModel.Nivel> findAllNivel()
        {
            return daoNivel.findAll(false).ToList();
        }

        public Nivel findByIdNivel(int id)
        {
            return daoNivel.findById(id, false);
        }

        public Nivel addNivel(Nivel entity)
        {
            if (entity.nm_ordem <= 0)
                entity.nm_ordem = (GetUltimoNmOrdem(entity.cd_nivel) + 1);

            return daoNivel.add(entity, false);
        }

        public int GetUltimoNmOrdem(int cd_nivel)
        {
            return daoNivel.GetUltimoNmOrdem(cd_nivel);
        }


        public Nivel editNivel(Nivel entity)
        {

            if (entity.nm_ordem <= 0)
                entity.nm_ordem = (GetUltimoNmOrdem(entity.cd_nivel) + 1);

            return daoNivel.edit(entity, false);
        }

        public bool deleteNivel(Nivel entity)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                Nivel nivel = daoNivel.findById(entity.cd_nivel, false);
                deleted = daoNivel.delete(nivel, false);
                transaction.Complete();
            }

            return deleted;
        }

        public bool deleteAllNivel(List<Nivel> niveis)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                foreach (Nivel niv in niveis)
                {
                    Nivel nivDel = daoNivel.findById(niv.cd_nivel, false);
                    deleted = daoNivel.delete(nivDel, false);
                }

                transaction.Complete();
            }

            return deleted;
        }


        #endregion

        #region Carga Professor

        public IEnumerable<CargaProfessorSearchUI> getCargaProfessorSearch(SearchParameters parametros,
            int qtd_minutos_duracao, int cd_escola)
        {
            IEnumerable<CargaProfessorSearchUI> retorno = new List<CargaProfessorSearchUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "nm_carga_horaria";

                parametros.sort = parametros.sort.Replace("nm_carga_professor", "nm_carga_professor");
                retorno = daoCargaProfessor.getCargaProfessorSearch(parametros, qtd_minutos_duracao, cd_escola);
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<CargaProfessor> findCargaProfessorAll(int cd_escola)
        {
            return daoCargaProfessor.findCargaProfessorAll(cd_escola);
        }

        public CargaProfessor findCargaProfessorById(int id, int cd_escola)
        {
            return daoCargaProfessor.findCargaProfessorById(id, cd_escola);
        }

        public CargaProfessor addCargaProfessor(CargaProfessor entity, int cd_escola)
        {
            return daoCargaProfessor.add(entity, false);
        }

        public CargaProfessor editCargaProfessor(CargaProfessor entity, int cd_escola)
        {
            return daoCargaProfessor.edit(entity, false);
        }

        public bool deleteCargaProfessor(CargaProfessor entity, int cd_escola)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                CargaProfessor cargaProfessor =
                    daoCargaProfessor.findCargaProfessorById(entity.cd_carga_professor, cd_escola);
                deleted = daoCargaProfessor.delete(cargaProfessor, false);
                transaction.Complete();
            }

            return deleted;
        }

        public bool deleteAllCargaProfessor(List<CargaProfessor> cargaProfessores, int cd_escola)
        {
            bool retorno = false;
            List<int> listaCodigos = cargaProfessores.Where(c => c.cd_carga_professor > 0)
                .Select(x => x.cd_carga_professor).ToList();
            if (listaCodigos.Count() > 0)
            {
                using (var transaction =
                    TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    List<CargaProfessor> cargasProfessorContext =
                        daoCargaProfessor.getCargaProfessorByEscolaAllData(listaCodigos, cd_escola).ToList();
                    foreach (CargaProfessor c in cargasProfessorContext)
                        retorno = daoCargaProfessor.deleteContext(c, false);
                    transaction.Complete();
                }
            }

            daoCargaProfessor.saveChanges(false);
            return retorno;
        }

        #endregion

        #region Calendário Evento

        public CalendarioEvento addCalendarioEvento(CalendarioEvento calendarioEvento)
        {
            var evento = new CalendarioEvento();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                validaçõesCalendarioEvento(calendarioEvento);

                evento = daoCalendarioEvento.add(calendarioEvento, false);
                transaction.Complete();
            }

            return evento;
        }

        public CalendarioEvento editarCalendarioEvento(CalendarioEvento calendarioEvento)
        {
            var evento = new CalendarioEvento();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                validaçõesCalendarioEvento(calendarioEvento);

                evento = daoCalendarioEvento.edit(calendarioEvento, false);
                transaction.Complete();
            }

            return evento;
        }

        public bool deletarCalendarioEvento(List<int> eventos, int cd_escola)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (var cd_evento in eventos)
                {
                    CalendarioEvento calEvento = daoCalendarioEvento.findCalendarioEventoById(cd_evento, cd_escola);
                    deleted = daoCalendarioEvento.delete(calEvento, false);
                }

                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<CalendarioEvento> obterListaCalendarioEventos(int cd_escola)
        {
            var eventos = new List<CalendarioEvento>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                eventos = daoCalendarioEvento.obterListaCalendarioEventos(cd_escola).ToList();
                transaction.Complete();
            }

            return eventos;
        }

        public IEnumerable<CalendarioEvento> obterCalendarioEventosPorFiltros(SearchParameters parametros,
            int cd_escola, string dc_titulo_evento, bool inicio, bool? status, string dt_inicial_evento,
            string dt_final_evento, string hh_inicial_evento, string hh_final_evento)
        {
            IEnumerable<CalendarioEvento> retorno = new List<CalendarioEvento>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "dc_titulo_evento";

                //parametros.sort = parametros.sort.Replace("nm_carga_professor", "nm_carga_professor");
                retorno = daoCalendarioEvento.obterCalendarioEventosPorFiltros(parametros, cd_escola, dc_titulo_evento,
                    inicio, status, dt_inicial_evento,
                    dt_final_evento, hh_inicial_evento, hh_final_evento);
                transaction.Complete();
            }

            return retorno;
        }

        public CalendarioEvento obterCalendarioEventoPorID(int cd_calendario_evento, int cd_escola)
        {
            CalendarioEvento evento = null;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                evento = daoCalendarioEvento.findCalendarioEventoById(cd_calendario_evento, cd_escola);
                transaction.Complete();
            }

            return evento;
        }

        public List<int> findEscolas()
        {
            List<int> retorno = null;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoCalendarioAcademico.findEscolas();
                transaction.Complete();
            }

            return retorno;
        }

        public CalendarioAcademico findCalendarioAcademico(int item, byte tipo)
        {
            CalendarioAcademico retorno = null;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoCalendarioAcademico.findCalendarioAcademico(item, tipo);
                transaction.Complete();
            }

            return retorno;
        }

        private void validaçõesCalendarioEvento(CalendarioEvento calendarioEvento)
        {
            if (calendarioEvento.dt_final_evento < calendarioEvento.dt_inicial_evento)
                throw new CoordenacaoBusinessException(Messages.msgErroDtFimMenorDtInicioEvento, null,
                    CoordenacaoBusinessException.TipoErro.ERRO_DT_FIM_MENOR_DT_INICIO, false);

            if (calendarioEvento.dt_inicial_evento == calendarioEvento.dt_final_evento &&
                (calendarioEvento.hh_final_evento < calendarioEvento.hh_inicial_evento))
                throw new CoordenacaoBusinessException(Messages.msgErrohhFimMenorhhInicioEvento, null,
                    CoordenacaoBusinessException.TipoErro.ERRO_HH_FIM_MENOR_HH_INICIO, false);
        }

        #endregion

        #region Calendário Acadêmico

        public CalendarioAcademico addCalendarioAcademico(CalendarioAcademico calendarioAcademico)
        {
            var calendario = new CalendarioAcademico();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //validaçõesCalendarioEvento(calendarioEvento);

                calendario = daoCalendarioAcademico.add(calendarioAcademico, false);
                transaction.Complete();
            }

            return calendario;
        }

        public CalendarioAcademico editarCalendarioAcademico(CalendarioAcademico calendarioAcademico)
        {
            var calendario = new CalendarioAcademico();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //validaçõesCalendarioEvento(calendarioEvento);

                calendario = daoCalendarioAcademico.edit(calendarioAcademico, false);
                transaction.Complete();
            }

            return calendario;
        }


        public CalendarioAcademico editarCalendarioMaster(List<int> cd_escolas, CalendarioAcademico calendarioAcademico)
        {
            var calendario = new CalendarioAcademico();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (var cd_escola in cd_escolas)
                {
                    CalendarioAcademico calAcademico =
                        daoCalendarioAcademico.findCalendarioAcademico(cd_escola,
                            calendarioAcademico.cd_tipo_calendario);
                    if (calAcademico != null)
                    {
                        calAcademico.cd_tipo_calendario = calendarioAcademico.cd_tipo_calendario;
                        calAcademico.cd_pessoa_escola = cd_escola;
                        calAcademico.dc_desc_calendario = calendarioAcademico.dc_desc_calendario;
                        calAcademico.id_mostrar_todos = calendarioAcademico.id_mostrar_todos;
                        calAcademico.id_ativo = calendarioAcademico.id_ativo;
                        if (calendarioAcademico.cd_pessoa_escola == calAcademico.cd_pessoa_escola)
                        {
                            calendario = daoCalendarioAcademico.edit(calAcademico, false);
                        }
                        else
                        {
                            daoCalendarioAcademico.edit(calAcademico, false);
                        }
                    }
                }

                transaction.Complete();
            }

            return calendario;
        }


        public bool deletarCalendarioAcademicoMaster(CalendarioAcademico calendarioAcademico)
        {
            bool deleted = false;


            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                deleted = daoCalendarioAcademico.delete(calendarioAcademico, false);

                transaction.Complete();
            }

            return deleted;
        }

        /*
                public bool deletarCalendarioAcademico(List<int> calendarios, int cd_escola)
                {
                    bool deleted = false;
                    using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                    {
                        foreach (var cd_calendario in calendarios)
                        {
                            CalendarioAcademico calAcademico = daoCalendarioAcademico.findCalendarioAcademicoById(cd_calendario, cd_escola);
                            deleted = daoCalendarioAcademico.delete(calAcademico, false);
                        }
                        transaction.Complete();
                    }
                    return deleted;
                }
        */
        public IEnumerable<CalendarioAcademico> obterListaCalendarioAcademicos(int cd_escola)
        {
            var calendarios = new List<CalendarioAcademico>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                calendarios = daoCalendarioAcademico.obterListaCalendarioAcademicos(cd_escola).ToList();
                transaction.Complete();
            }

            return calendarios;
        }

        public IEnumerable<CalendarioAcademico> obterCalendarioAcademicosPorFiltros(SearchParameters parametros,
            int cd_escola, int tipo_calendario, bool? status, bool relatorio)
        {
            IEnumerable<CalendarioAcademico> retorno = new List<CalendarioAcademico>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "cd_calendario_academico";

                retorno = daoCalendarioAcademico.obterCalendarioAcademicosPorFiltros(parametros, cd_escola,
                    tipo_calendario, status, relatorio);
                transaction.Complete();
            }

            return retorno;
        }

        public CalendarioAcademico obterCalendarioAcademicoPorID(int cd_calendario_academico, int cd_escola)
        {
            CalendarioAcademico calendario = null;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                calendario = daoCalendarioAcademico.findCalendarioAcademicoById(cd_calendario_academico, cd_escola);
                transaction.Complete();
            }

            return calendario;
        }

        #endregion

        #region Video

        public IEnumerable<Video> getVideoSearch(Componentes.Utils.SearchParameters parametros, string no_video,
            int nm_video, List<byte> menu)
        {
            IEnumerable<Video> retorno = new List<Video>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "nm_video";

                retorno = daoVideo.getVideoSearch(parametros, no_video, nm_video, menu);
                transaction.Complete();
            }

            return retorno;
        }

        public Video addVideo(Video video)
        {
            var videoAux = new Video();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //validaçõesCalendarioEvento(calendarioEvento);

                videoAux = daoVideo.add(video, false);
                transaction.Complete();
            }

            return videoAux;
        }

        public Video editarVideo(Video video)
        {
            var videoAux = new Video();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                videoAux = daoVideo.edit(video, false);
                transaction.Complete();
            }

            return video;
        }

        public bool deletarVideo(List<int> videos)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (var cd_video in videos)
                {
                    Video video = daoVideo.findDeletedVideoById(cd_video);
                    deleted = daoVideo.delete(video, false);
                }

                transaction.Complete();
            }

            return deleted;
        }

        public Video obterVideoPorID(int cd_video)
        {
            Video videoAux = null;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                videoAux = daoVideo.findVideoById(cd_video);
                transaction.Complete();
            }
            return videoAux;
        }

        public Video obterVideoPorNome(string no_video)
        {
            Video videoAux = null;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                videoAux = daoVideo.findVideoByName(no_video);
                transaction.Complete();
            }
            return videoAux;
        }

        public Video obterVideoPorNumeroParte(int nm_video, int nm_parte)
        {
            Video videoAux = null;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                videoAux = daoVideo.findVideoByNumeroParte(nm_video, nm_parte);

                transaction.Complete();
            }
            return videoAux;
        }


        public IEnumerable<Video> obterVideosPorFiltros(Componentes.Utils.SearchParameters parametros, string no_video, int nm_video, List<byte> menu)
        {
            IEnumerable<Video> retorno = new List<Video>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "nm_video";

                retorno = daoVideo.obterVideosPorFiltros(parametros, no_video, nm_video, menu);
                transaction.Complete();
            }

            return retorno;
        }

        #endregion

        #region Circular

        public Circular addCircular(Circular circular)
        {
            var novaCircular = new Circular();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                novaCircular = daoCircular.add(circular, false);
                transaction.Complete();
            }

            return novaCircular;
        }

        public Circular editarCircular(Circular circular)
        {
            var editCircular = new Circular();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                editCircular = daoCircular.edit(circular, false);
                transaction.Complete();
            }

            return editCircular;
        }

        public bool deletarCircular(List<int> circulares)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (var cd_circular in circulares)
                {
                    Circular delCircular = daoCircular.findDeletedCircularById(cd_circular);
                    deleted = daoCircular.delete(delCircular, false);
                }

                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<Circular> obterListaCirculares()
        {
            var circulares = new List<Circular>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                circulares = daoCircular.obterListaCirculares().ToList();
                transaction.Complete();
            }

            return circulares;
        }

        public IEnumerable<Circular> obterCircularesPorFiltros(SearchParameters parametros, short nm_ano_circular,
            List<byte> nm_mes_circular, int nm_circular, string no_circular, List<byte> nm_menu_circular)
        {
            IEnumerable<Circular> retorno = new List<Circular>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "nm_circular";

                //parametros.sort = parametros.sort.Replace("nm_carga_professor", "nm_carga_professor");
                retorno = daoCircular.obterCircularesPorFiltros(parametros, nm_ano_circular, nm_mes_circular,
                    nm_circular, no_circular, nm_menu_circular);
                transaction.Complete();
            }

            return retorno;
        }

        public Circular obterCircularPorID(int cd_circular)
        {
            Circular circular = null;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                circular = daoCircular.findCircularById(cd_circular);
                transaction.Complete();
            }

            return circular;
        }

        #endregion

        #region ControleFaltas

        public IEnumerable<ControleFaltasUI> getControleFaltasSearch(Componentes.Utils.SearchParameters parametros,
            string desc, int cd_turma, int cd_aluno, int assinatura, DateTime? dtInicial, DateTime? dtFinal,
            int cdEscola)
        {
            IEnumerable<ControleFaltasUI> retorno = new List<ControleFaltasUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                var assinatura_aux = ControleFaltasDataAccess.AssinaturaControleFaltas.TODOS;
                switch (assinatura)
                {
                    case ((int)ControleFaltasDataAccess.AssinaturaControleFaltas.TODOS):
                        assinatura_aux = ControleFaltasDataAccess.AssinaturaControleFaltas.TODOS;
                        break;
                    case ((int)ControleFaltasDataAccess.AssinaturaControleFaltas.ASSINOU):
                        assinatura_aux = ControleFaltasDataAccess.AssinaturaControleFaltas.ASSINOU;
                        break;
                    case ((int)ControleFaltasDataAccess.AssinaturaControleFaltas.NAOASSINOU):
                        assinatura_aux = ControleFaltasDataAccess.AssinaturaControleFaltas.NAOASSINOU;
                        break;
                }

                if (parametros.sort == null)
                    parametros.sort = "no_turma";
                retorno = daoControleFaltas.getControleFaltasSearch(parametros, desc, cd_turma, cd_aluno,
                    assinatura_aux, dtInicial, dtFinal, cdEscola);
                transaction.Complete();
            }

            return retorno;
        }



        public IEnumerable<ControleFaltasAlunoUI> getAlunosTurmaControleFalta(int cd_turma, int cd_pessoa_escola,
            DateTime? dt_inicial, DateTime dt_final, int cd_controle_faltas)
        {
            IEnumerable<ControleFaltasAlunoUI> retorno = new List<ControleFaltasAlunoUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoControleFaltasAluno.getAlunosTurmaControleFalta(cd_turma, cd_pessoa_escola, dt_inicial,
                    dt_final, cd_controle_faltas, AlunoTurma.FiltroSituacaoAlunoTurma.Nao_Encerrado);
                transaction.Complete();
            }

            return retorno;
        }

        public ControleFaltasAlunoUI getAlunoControleFalta(int cd_turma, int cd_pessoa_escola, int cd_aluno, DateTime? dt_inicial, DateTime dt_final)
        {
            ControleFaltasAlunoUI retorno = new ControleFaltasAlunoUI();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoControleFaltasAluno.getAlunoControleFalta(cd_turma, cd_pessoa_escola, cd_aluno, dt_inicial,
                    dt_final, AlunoTurma.FiltroSituacaoAlunoTurma.Nao_Encerrado);
                transaction.Complete();
            }

            return retorno;
        }



        public ControleFaltasUI addAlunoControleFaltas(ControleFaltasUI item)
        {
            ControleFaltas novoItem = new ControleFaltas();
            ControleFaltasUI itemUI = new ControleFaltasUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.COMMITED, daoControleFaltas.DB(),
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {

                //Persistir um ControleFalta
                novoItem.copy(item);

                daoControleFaltas.add(novoItem, false);
                itemUI = daoControleFaltas.getControleFaltasUIbyId(novoItem.cd_controle_faltas);
                //itemUI = ItemUI.fromItem(novoItem, tipoItem, grupoEstoque, itemEscola, biblioteca, item.desc_plano_conta);
                transaction.Complete();
            }

            return itemUI;
        }

        public ControleFaltasUI editarAlunoControleFaltas(ControleFaltasUI item)
        {

            ControleFaltas itemEdit = daoControleFaltas.getControleFaltasEdit(item.cd_controle_faltas);
            ControleFaltas novoItem = new ControleFaltas();
            novoItem.copy(itemEdit);
            novoItem.cd_turma = item.cd_turma;
            novoItem.dt_controle_faltas = item.dt_controle_faltas;
            novoItem.ControleFaltasAluno = null;
            daoControleFaltas.edit(novoItem, false);
            daoControleFaltas.saveChanges(false);
            ControleFaltasUI itemUI = new ControleFaltasUI();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {


                foreach (var itemAluno in itemEdit.ControleFaltasAluno)
                {
                    ControleFaltasAluno itemControleFaltasAlunoDeletar =
                        daoControleFaltasAluno.getAlunoControleEstoqueByCdItem(itemAluno.cd_controle_faltas_aluno);
                    daoControleFaltasAluno.deleteContext(itemControleFaltasAlunoDeletar, false);
                }

                daoControleFaltasAluno.saveChanges(false);

                foreach (ControleFaltasAluno controleFaltasAluno in item.ControleFaltasAluno)
                {
                    controleFaltasAluno.cd_controle_faltas = item.cd_controle_faltas;
                    daoControleFaltasAluno.add(controleFaltasAluno, false);
                }


                transaction.Complete();
            }
            itemUI = daoControleFaltas.getControleFaltasUIbyId(item.cd_controle_faltas);
            return itemUI;

        }

        private ControleFaltas changeValuesItem(ControleFaltasUI item)
        {
            ControleFaltas itemBase = daoControleFaltas.findById(item.cd_controle_faltas, false);
            itemBase.cd_turma = item.cd_turma;
            itemBase.no_turma = item.no_turma;
            itemBase.dt_controle_faltas = item.dt_controle_faltas;
            itemBase.no_usuario = item.no_usuario;
            changeItensAlunoControleFaltas(itemBase.ControleFaltasAluno, item.ControleFaltasAluno);

            //itemBase.cd_plano_conta = item.cd_plano_conta;
            daoControleFaltas.saveChanges(false);
            return itemBase;
        }

        private void changeItensAlunoControleFaltas(ICollection<ControleFaltasAluno> itemBase, ICollection<ControleFaltasAluno> item)
        {
            foreach (ControleFaltasAluno i in itemBase)
            {
                foreach (var j in item)
                {
                    if (i.cd_controle_faltas_aluno == j.cd_controle_faltas_aluno)
                    {
                        i.id_assinatura = j.id_assinatura;
                    }
                }
            }
        }

        public ControleFaltas getControleFaltasById(int cd_controle_faltas)
        {
            ControleFaltas retorno = new ControleFaltas();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoControleFaltas.getControleFaltasEdit(cd_controle_faltas);
                transaction.Complete();
            }

            return retorno;
        }


        //Delete all item
        public bool deleteAllControleFaltas(List<int> itens)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                for (int i = 0; i < itens.Count(); i++)
                {
                    ControleFaltas item = daoControleFaltas.getControleFaltasEdit(itens[i]);

                    foreach (var itemAluno in item.ControleFaltasAluno)
                    {
                        ControleFaltasAluno itemControleFaltasAlunoDeletar =
                            daoControleFaltasAluno.getAlunoControleEstoqueByCdItem(itemAluno.cd_controle_faltas_aluno);
                        daoControleFaltasAluno.deleteContext(itemControleFaltasAlunoDeletar, false);
                    }
                    daoControleFaltasAluno.saveChanges(false);

                    item = daoControleFaltas.findById(itens[i], false);
                    daoControleFaltas.delete(item, false);
                }

                deleted = true;
                transaction.Complete();
            }

            return deleted;
        }

        public List<ControleFaltasAlunoUI> getAlunosControleFalta(int cd_controle_faltas)
        {
            List<ControleFaltasAlunoUI> result;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                result = daoControleFaltasAluno.getAlunosControleFalta(cd_controle_faltas);
                transaction.Complete();
                return result;
            }
        }
        #endregion

        #region Aulas de Reposicao

        public IEnumerable<AulaReposicaoUI> searchAulaReposicao(SearchParameters parametros, DateTime? dataIni, DateTime? dataFim, TimeSpan? hrInicial, TimeSpan? hrFinal, int? cd_turma, int? cd_aluno, int? cd_responsavel, int? cd_sala, int cdEscola)
        {
            IEnumerable<AulaReposicaoUI> retorno = new List<AulaReposicaoUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null) parametros.sort = "dt_aula_reposicao";
                parametros.sort = parametros.sort.Replace("dta_aula_reposicao", "dt_aula_reposicao");
                retorno = daoAulaReposicao.searchAulaReposicao(parametros, dataIni,
                    dataFim, hrInicial, hrFinal, cd_turma, cd_aluno, cd_responsavel, cd_sala, cdEscola);
                transaction.Complete();
            }

            return retorno;
        }

        public AulaReposicaoUI addAulaReposicao(AulaReposicaoUI aulaReposicaoUi)
        {
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                var newAulaReposicao = setNewAulaReposicao(aulaReposicaoUi);
                var dataAtv = aulaReposicaoUi.dta_aula_reposicao;
                newAulaReposicao.dt_aula_reposicao = Convert.ToDateTime(dataAtv);

                var aulaReposicaoBD = daoAulaReposicao.add(newAulaReposicao, false);

                //Incluir Aula do aluno
                if (aulaReposicaoUi.AlunoAulaReposicao != null)
                {
                    foreach (var alunoAulaReposicao in aulaReposicaoUi.AlunoAulaReposicao)
                    {
                        alunoAulaReposicao.cd_aula_reposicao = aulaReposicaoBD.cd_aula_reposicao;
                        daoAlunoAulaReposicao.add(alunoAulaReposicao, false);
                    }
                }

                var idAulaReposicao = aulaReposicaoBD.cd_aula_reposicao;
                aulaReposicaoUi.cd_aula_reposicao = aulaReposicaoBD.cd_aula_reposicao;
                aulaReposicaoUi.nm_alunos = (int)daoAlunoAulaReposicao.retornNumbersOfStudents(idAulaReposicao);

                aulaReposicaoUi = AulaReposicaoUI.fromAulaReposicao(aulaReposicaoBD, aulaReposicaoUi.no_responsavel, aulaReposicaoUi.no_usuario,
                    aulaReposicaoUi.nm_alunos, aulaReposicaoUi.no_turma, aulaReposicaoUi.no_sala, aulaReposicaoUi.no_turma_destino);
                transaction.Complete();
            }

            return aulaReposicaoUi;
        }


        public AulaReposicaoUI editAulaReposicao(AulaReposicaoUI aulaReposicao)
        {
            AulaReposicao aulaReposicaoBD = new AulaReposicao();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                //Persitir atividade aluno
                persistirAlunoAulaReposicao(aulaReposicao, aulaReposicao.cd_pessoa_escola);


                //Edita a ativiadade extra
                aulaReposicaoBD = daoAulaReposicao.findById(aulaReposicao.cd_aula_reposicao, false);

                //aulaReposicaoBD.copy(aulaReposicao);
                setEditAulaReposicao(aulaReposicao, aulaReposicaoBD);
                //aulaReposicaoBD.AlunoAulaReposicao = null;
                daoAulaReposicao.saveChanges(false);

                var idAulaReposicao = aulaReposicaoBD.cd_aula_reposicao;
                aulaReposicao.nm_alunos = (int)daoAlunoAulaReposicao.retornNumbersOfStudents(idAulaReposicao);

                aulaReposicao = AulaReposicaoUI.fromAulaReposicao(aulaReposicaoBD, aulaReposicao.no_responsavel, aulaReposicao.no_usuario,
                    aulaReposicao.nm_alunos, aulaReposicao.no_turma, aulaReposicao.no_sala, aulaReposicao.no_turma_destino);
                transaction.Complete();
            }

            return aulaReposicao;
        }


        private void persistirAlunoAulaReposicao(AulaReposicaoUI aulaReposicao, int cdEscola)
        {
            //Pega as atividades do aluno na base de dados
            IEnumerable<AlunoAulaReposicaoUI> alunoAulaReposicaoCTX =
                daoAlunoAulaReposicao.searchAlunoAulaReposicao(aulaReposicao.cd_aula_reposicao, cdEscola);
            //Pega as atividades da view
            IEnumerable<AlunoAulaReposicao> alunoAulaReposicaoView = new List<AlunoAulaReposicao>();
            alunoAulaReposicaoView = aulaReposicao.AlunoAulaReposicao;

            if (alunoAulaReposicaoView != null)
            {
                //Incluir Atividade do aluno
                foreach (var alunoAulaReposicao in alunoAulaReposicaoView)
                {
                    alunoAulaReposicao.cd_aula_reposicao = aulaReposicao.cd_aula_reposicao;
                    var aluno = alunoAulaReposicao.cd_aluno_aula_reposicao == 0
                        ? daoAlunoAulaReposicao.add(alunoAulaReposicao, false)
                        : null;
                }

                // Filtra as atividade  aluno que possuem código
                var alunoAulaReposicaoComCodigo = alunoAulaReposicaoView.Where(at => at.cd_aluno_aula_reposicao != 0);
                var alunoAulaReposicaoDeleted = alunoAulaReposicaoCTX.Where(atv =>
                    !alunoAulaReposicaoComCodigo.Any(a => atv.cd_aluno_aula_reposicao == a.cd_aluno_aula_reposicao));

                //Deleta o registro que esta na base mas não esta na view.
                if (alunoAulaReposicaoDeleted.Count() > 0)
                {
                    foreach (var item in alunoAulaReposicaoDeleted)
                    {
                        var deletarAlunoAulaReposicao = daoAlunoAulaReposicao.findById(item.cd_aluno_aula_reposicao, false);
                        var deleted = deletarAlunoAulaReposicao != null
                            ? daoAlunoAulaReposicao.delete(deletarAlunoAulaReposicao, false)
                            : false;
                    }
                }

                //Edita os registros da atividade do aluno
                foreach (var colecaoAlunosAulaReposicaoVI in alunoAulaReposicaoView)
                {
                    foreach (var alunoAulaBD in alunoAulaReposicaoCTX)
                    {
                        if (colecaoAlunosAulaReposicaoVI.cd_aluno_aula_reposicao == alunoAulaBD.cd_aluno_aula_reposicao)
                        {
                            var alunoAulaReposicaoBD = daoAlunoAulaReposicao.findById(alunoAulaBD.cd_aluno_aula_reposicao, false);
                            alunoAulaReposicaoBD.id_participacao =
                                alunoAulaReposicaoBD.id_participacao == colecaoAlunosAulaReposicaoVI.id_participacao
                                    ? alunoAulaReposicaoBD.id_participacao
                                    : colecaoAlunosAulaReposicaoVI.id_participacao;
                            alunoAulaReposicaoBD.tx_observacao_aluno_aula =
                                alunoAulaReposicaoBD.tx_observacao_aluno_aula == colecaoAlunosAulaReposicaoVI.tx_observacao_aluno_aula
                                    ? alunoAulaReposicaoBD.tx_observacao_aluno_aula
                                    : colecaoAlunosAulaReposicaoVI.tx_observacao_aluno_aula;
                        }
                    }
                }

                daoAlunoAulaReposicao.saveChanges(false);
            }
            else
            {
                if (alunoAulaReposicaoCTX != null)
                {
                    foreach (var item in alunoAulaReposicaoCTX)
                    {
                        var deletarAlunoAulaReposicao = daoAlunoAulaReposicao.findById(item.cd_aluno_aula_reposicao, false);
                        var deleted = deletarAlunoAulaReposicao != null
                            ? daoAlunoAulaReposicao.delete(deletarAlunoAulaReposicao, false)
                            : false;
                    }
                }
            }
        }

        private static void setEditAulaReposicao(AulaReposicaoUI aulaReposicaoUi, AulaReposicao aulaReposicao)
        {

            aulaReposicao.cd_aula_reposicao = aulaReposicaoUi.cd_aula_reposicao;
            aulaReposicao.cd_pessoa_escola = aulaReposicaoUi.cd_pessoa_escola;
            aulaReposicao.cd_atendente = aulaReposicaoUi.cd_atendente;
            aulaReposicao.cd_professor = aulaReposicaoUi.cd_professor;
            aulaReposicao.dt_aula_reposicao = aulaReposicaoUi.dt_aula_reposicao;
            aulaReposicao.dh_inicial_evento = aulaReposicaoUi.dh_inicial_evento;
            aulaReposicao.dh_final_evento = aulaReposicaoUi.dh_final_evento;
            aulaReposicao.id_carga_horaria = aulaReposicaoUi.id_carga_horaria;
            aulaReposicao.id_pagar_professor = aulaReposicaoUi.id_pagar_professor;
            aulaReposicao.tx_observacao_aula = aulaReposicaoUi.tx_observacao_aula;
            aulaReposicao.cd_turma = aulaReposicaoUi.cd_turma;
            aulaReposicao.cd_turma_destino = aulaReposicaoUi.cd_turma_destino;
            aulaReposicao.cd_sala = aulaReposicaoUi.cd_sala;

        }

        private static AulaReposicao setNewAulaReposicao(AulaReposicaoUI aulaReposicaoUi)
        {
            AulaReposicao aulaReposicao = new AulaReposicao
            {
                cd_aula_reposicao = aulaReposicaoUi.cd_aula_reposicao,
                cd_pessoa_escola = aulaReposicaoUi.cd_pessoa_escola,
                cd_atendente = aulaReposicaoUi.cd_atendente,
                cd_professor = aulaReposicaoUi.cd_professor,
                dt_aula_reposicao = aulaReposicaoUi.dt_aula_reposicao,
                dh_inicial_evento = aulaReposicaoUi.dh_inicial_evento,
                dh_final_evento = aulaReposicaoUi.dh_final_evento,
                id_carga_horaria = aulaReposicaoUi.id_carga_horaria,
                id_pagar_professor = aulaReposicaoUi.id_pagar_professor,
                tx_observacao_aula = aulaReposicaoUi.tx_observacao_aula,
                cd_turma = aulaReposicaoUi.cd_turma,
                cd_turma_destino = aulaReposicaoUi.cd_turma_destino,
                cd_sala = aulaReposicaoUi.cd_sala
            };
            return aulaReposicao;
        }

        public bool deleteAllAulaReposicao(List<AulaReposicao> aulaReposicao, int cd_escola)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                long alunos = 0;
                foreach (var item in aulaReposicao)
                {
                    alunos = daoAlunoAulaReposicao.retornNumbersOfStudents(item.cd_aula_reposicao);
                    if (alunos > 0)
                        throw new CoordenacaoBusinessException(Messages.msgAlunosComAulaReposicao, null,
                            CoordenacaoBusinessException.TipoErro.ALUNOS_AULA_REPOSICAO, false);
                    var aulaReposicaoBD = daoAulaReposicao.findById(item.cd_aula_reposicao, false);
                    if (aulaReposicaoBD != null)
                    {
                        deleted = daoAulaReposicao.delete(aulaReposicaoBD, false);
                    }

                }

                transaction.Complete();
            }

            return deleted;
        }

        public AulaReposicao findByIdAulaReposicaoViewFull(int cdAulaReposicao)
        {
            AulaReposicao retorno = new AulaReposicao();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.UNCOMMITED, daoSala.DB(),
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = daoAulaReposicao.findByIdAulaReposicaoViewFull(cdAulaReposicao);
                transaction.Complete();
            }

            return retorno;
        }

        public IEnumerable<AlunoAulaReposicaoUI> searchAlunoAulaReposicao(int cd_aula_reposicao, int cdEscola)
        {
            IEnumerable<AlunoAulaReposicaoUI> retorno = new List<AlunoAulaReposicaoUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoAlunoAulaReposicao.searchAlunoAulaReposicao(cd_aula_reposicao, cdEscola);
                transaction.Complete();
            }

            return retorno;
        }

        public AulaReposicao findByIdAulaReposicaoFull(int cdAulaReposicao)
        {
            AulaReposicao retorno = new AulaReposicao();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(
                TransactionScopeBuilder.TransactionType.UNCOMMITED, daoAulaReposicao.DB(),
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = daoAulaReposicao.findByIdAulaReposicaoFull(cdAulaReposicao);
                transaction.Complete();
            }

            return retorno;
        }

        public List<sp_RptAulaReposicao_Result> getReportAulaReposicao(Nullable<int> cd_escola,
            Nullable<System.DateTime> dta_ini, Nullable<System.DateTime> dta_fim, Nullable<int> cd_turma,
            Nullable<int> cd_funcionario, Nullable<int> cd_aluno,
            Nullable<byte> id_participacao)
        {
            List<sp_RptAulaReposicao_Result> retorno = new List<sp_RptAulaReposicao_Result>();

            retorno = daoAulaReposicao.getReportAulaReposicao(cd_escola, dta_ini, dta_fim, cd_turma,
                cd_funcionario, cd_aluno, id_participacao);
            return retorno;
        }

        public List<sp_RptAulaReposicaoAluno_Result> getReportAulaReposicaoAluno(Nullable<int> cd_aula_reposicao,
            Nullable<int> cd_aluno, Nullable<byte> id_participou)
        {
            List<sp_RptAulaReposicaoAluno_Result> retorno = new List<sp_RptAulaReposicaoAluno_Result>();

            retorno = daoAulaReposicao.getReportAulaReposicaoAluno(cd_aula_reposicao, cd_aluno, id_participou);
            return retorno;
        }

        public AulaReposicaoUI returnAulaReposicaoUsuarioAtendente(int cd_aula_reposicao, int cd_pessoa_escola)
        {
            return daoAulaReposicao.returnAulaReposicaoUsuarioAtendente(cd_aula_reposicao, cd_pessoa_escola);
        }

        public List<TimeSpan?> getHorariosDisponiveisAulaRep(DateTime data, int turma, int professor, int? cdAulaReposicao, List<AlunoAulaReposicaoUI> alunos)
        {
            List<TimeSpan?> retorno = new List<TimeSpan?>();
            retorno = daoAulaReposicao.getHorariosDisponiveisAulaRep(data, turma, professor, cdAulaReposicao, alunos);
            return retorno;
        }

        public int? verificaHorarioAulaRep(TimeSpan horaIni, TimeSpan horaFim, DateTime data, int? cd_aula_reposicao, int cd_turma, int cd_professor, int cd_empresa, List<AlunoAulaReposicaoUI> alunos)
        {
            return daoAulaReposicao.verificaHorarioAulaRep(horaIni, horaFim, data, cd_aula_reposicao, cd_turma, cd_professor, cd_empresa, alunos);
        }
        #endregion

        #region Faq

        public Faq addFaq(Faq faq)
        {
            var faqAux = new Faq();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                faqAux = daoFaq.add(faq, false);
                transaction.Complete();
                criarFaqChildren(faq, faqAux);
            }

            return faqAux;
        }

        public Faq obterFaqPorID(int cd_faq)
        {
            Faq faqAux = null;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                faqAux = daoFaq.findFaqById(cd_faq);
                transaction.Complete();
            }
            return faqAux;
        }

        public Faq editarFaq(Faq faq)
        {
            var faqAux = new Faq();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                faqAux = daoFaq.edit(faq, false);
                transaction.Complete();
                criarFaqChildren(faq, faqAux);
            }

            return faq;
        }

        private static void criarFaqChildren(Faq faq, Faq faqAux)
        {
            faq.children = new List<Children>();
            faq.children.Add(new Children
            {
                id = faqAux.cd_faq,
                cd_faq = Guid.NewGuid(),
                dc_faq_resposta = faqAux.dc_faq_resposta,
                no_video_faq = faqAux.no_video_faq
            });
        }

        public bool deletarFaq(List<int> faqs)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (var cd_faq in faqs)
                {
                    Faq faq = daoFaq.findDeletedFaqById(cd_faq);
                    deleted = daoFaq.delete(faq, false);
                }

                transaction.Complete();
            }

            return deleted;
        }

        public IEnumerable<Faq> obterFaqsPorFiltros(SearchParameters parametros, string dc_faq_pergunta, bool dc_faq_inicio, List<byte> menu)
        {
            IEnumerable<Faq> retorno = new List<Faq>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "dc_faq_pergunta";

                retorno = daoFaq.obterFaqsPorFiltros(parametros, dc_faq_pergunta, dc_faq_inicio, menu).ToList();

                foreach (var faq in retorno)
                {
                    criarFaqChildren(faq, faq);
                }

                transaction.Complete();
            }

            return retorno;
        }

        public Faq obterFaqPorNumeroParte(int nm_faq, int nm_parte)
        {
            throw new NotImplementedException();
        }

        public Faq obterFaqPorNome(string no_faq)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Curso

        public IEnumerable<Curso> findCurso(CursoDataAccess.TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto, int? cd_escola)
        {
            IEnumerable<Curso> retorno = new List<Curso>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = cursoBiz.getCursos(hasDependente, cd_curso, cd_produto, cd_escola).ToList();
                transaction.Complete();
            }

            return retorno;
        }

        #endregion

        #region Mensagem Avaliacao

        public IEnumerable<MensagemAvaliacaoSearchUI> getMensagemAvaliacaoSearch(SearchParameters parametros, string desc, bool inicio, bool? status, int? produto, int? curso)
        {
            IEnumerable<MensagemAvaliacaoSearchUI> retorno = new List<MensagemAvaliacaoSearchUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_produto";
                parametros.sort = parametros.sort.Replace("mensagemAtiva", "id_mensagem_ativa");
                retorno = daoMensagemAvaliacao.getMensagemAvaliacaoSearch(parametros, desc, inicio, status, produto, curso);
                transaction.Complete();
            }

            return retorno;
        }


        public List<MensagemAvaliacaoSearchUI> addMensagemAvaliacao(MensagemAvaliacaoSearchUI mensagemAvaliacao)
        {
            List<MensagemAvaliacao> listaMensagensAdded = new List<MensagemAvaliacao>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                List<MensagemAvaliacao> listaAdd = setNewMensagensAvaliacoes(mensagemAvaliacao);

                if (listaAdd != null && listaAdd.Count > 0)
                {
                    listaMensagensAdded = daoMensagemAvaliacao.addRange(listaAdd, false);
                }

                transaction.Complete();
            }


            List<int> idsMensagems = listaMensagensAdded.Select(x => x.cd_mensagem_avaliacao).ToList();
            List<MensagemAvaliacaoSearchUI> retorno = daoMensagemAvaliacao.getMensagensAvaliacaoByIds(idsMensagems);
            return retorno;
        }


        private static List<MensagemAvaliacao> setNewMensagensAvaliacoes(MensagemAvaliacaoSearchUI mensagemAvaliacao)
        {
            List<MensagemAvaliacao> listaMensagemAvaliacaoAdd = new List<MensagemAvaliacao>();
            if(mensagemAvaliacao.cursos != null && mensagemAvaliacao.cursos.Count > 0)
            {
                foreach (int curso in mensagemAvaliacao.cursos)
                {   
                    if(curso > 0)
                    {
                        MensagemAvaliacao newMensagemAvaliacao = new MensagemAvaliacao()
                        {
                            cd_mensagem_avaliacao = 0,
                            cd_produto = mensagemAvaliacao.cd_produto,
                            cd_curso = curso,
                            id_mensagem_ativa = mensagemAvaliacao.id_mensagem_ativa,
                            tx_mensagem_avaliacao = mensagemAvaliacao.tx_mensagem_avaliacao

                        };
                        listaMensagemAvaliacaoAdd.Add(newMensagemAvaliacao);
                    }
                }
            }

            return listaMensagemAvaliacaoAdd;
        }

        public MensagemAvaliacaoSearchUI editMensagemAvaliacao(MensagemAvaliacao entity)
        {

            MensagemAvaliacao mensagemEditContext = daoMensagemAvaliacao.findById(entity.cd_mensagem_avaliacao, false);

            setValuesMensagemAvaliacaoEdit(entity, mensagemEditContext);

             var edited = daoMensagemAvaliacao.edit(mensagemEditContext, false);

             return daoMensagemAvaliacao.getMensagensAvaliacaoById(edited.cd_mensagem_avaliacao).FirstOrDefault();


        }

        private static void setValuesMensagemAvaliacaoEdit(MensagemAvaliacao entity, MensagemAvaliacao mensagemEditContext)
        {
            mensagemEditContext.id_mensagem_ativa = entity.id_mensagem_ativa;
            mensagemEditContext.tx_mensagem_avaliacao = entity.tx_mensagem_avaliacao;
        }

        public bool deleteAllMensagemAvaliacao(List<MensagemAvaliacao> listaMensagemAvaliacao)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                foreach (MensagemAvaliacao mensagem in listaMensagemAvaliacao)
                {
                    MensagemAvaliacao mensagemDel = daoMensagemAvaliacao.findById(mensagem.cd_mensagem_avaliacao, false);
                    deleted = daoMensagemAvaliacao.delete(mensagemDel, false);
                }

                transaction.Complete();
            }

            return deleted;
        }
        public List<MensagemAvaliacaoAlunoUI> addMensagemAvaliacaoAluno(MensagemAvaliacaoAlunoUI mensagemAvaliacao)
        {
            List<MensagemAvaliacaoAluno> listaMensagensAdded = new List<MensagemAvaliacaoAluno>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                List<MensagemAvaliacaoAluno> listaAdd = setNewMensagensAvaliacoesAluno(mensagemAvaliacao);

                if (listaAdd != null && listaAdd.Count > 0)
                {
                    listaMensagensAdded = daoMensagemAvaliacaoAluno.addRange(listaAdd, false);
                }

                transaction.Complete();
            }


            //List<int> idsMensagems = listaMensagensAdded.Select(x => x.cd_mensagem_avaliacao_aluno).ToList();
            int idsMensagems = listaMensagensAdded.Select(x => x.cd_mensagem_avaliacao_aluno).FirstOrDefault();
            List<MensagemAvaliacaoAlunoUI> retorno = daoMensagemAvaliacaoAluno.getMensagensAvaliacaoAlunoById(idsMensagems);
            return retorno;
        }
        private static List<MensagemAvaliacaoAluno> setNewMensagensAvaliacoesAluno(MensagemAvaliacaoAlunoUI mensagemAvaliacao)
        {
            List<MensagemAvaliacaoAluno> listaMensagemAvaliacaoAdd = new List<MensagemAvaliacaoAluno>();
            MensagemAvaliacaoAluno newMensagemAvaliacao = new MensagemAvaliacaoAluno()
            {
                cd_mensagem_avaliacao_aluno = 0,
                cd_aluno = mensagemAvaliacao.cd_aluno,
                cd_tipo_avaliacao = mensagemAvaliacao.cd_tipo_avaliacao,
                id_mensagem_aluno_ativa = mensagemAvaliacao.id_mensagem_ativa,
                tx_mensagem_avaliacao_aluno = mensagemAvaliacao.tx_mensagem_avaliacao_aluno,
                cd_mensagem_avaliacao = mensagemAvaliacao.cd_mensagem_avaliacao
            };
            listaMensagemAvaliacaoAdd.Add(newMensagemAvaliacao);

            return listaMensagemAvaliacaoAdd;
        }
        public MensagemAvaliacaoAlunoUI editMensagemAvaliacaoAluno(MensagemAvaliacaoAluno entity)
        {

            MensagemAvaliacaoAluno mensagemEditContext = daoMensagemAvaliacaoAluno.findById(entity.cd_mensagem_avaliacao_aluno, false);

            setValuesMensagemAvaliacaoAlunoEdit(entity, mensagemEditContext);

            var edited = daoMensagemAvaliacaoAluno.edit(mensagemEditContext, false);

            return daoMensagemAvaliacaoAluno.getMensagensAvaliacaoAlunoById(edited.cd_mensagem_avaliacao_aluno).FirstOrDefault();


        }

        private static void setValuesMensagemAvaliacaoAlunoEdit(MensagemAvaliacaoAluno entity, MensagemAvaliacaoAluno mensagemEditContext)
        {
            mensagemEditContext.id_mensagem_aluno_ativa = entity.id_mensagem_aluno_ativa;
            mensagemEditContext.tx_mensagem_avaliacao_aluno = entity.tx_mensagem_avaliacao_aluno;
        }
        public bool deleteAllMensagemAvaliacaoAluno(List<MensagemAvaliacaoAluno> listaMensagemAvaliacao)
        {
            bool deleted = false;
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                foreach (MensagemAvaliacaoAluno mensagem in listaMensagemAvaliacao)
                {
                    MensagemAvaliacaoAluno mensagemDel = daoMensagemAvaliacaoAluno.findById(mensagem.cd_mensagem_avaliacao_aluno, false);
                    deleted = daoMensagemAvaliacaoAluno.delete(mensagemDel, false);
                }

                transaction.Complete();
            }

            return deleted;
        }


        public IEnumerable<MensagemAvaliacaoAlunoUI> findMsgAlunobyTipo(int cdTipoAvaliacao, int cdAluno, int cdProduto, int cdCurso)
        {
            IEnumerable<MensagemAvaliacaoAlunoUI> retorno = new List<MensagemAvaliacaoAlunoUI>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                retorno = daoMensagemAvaliacaoAluno.findMsgAlunobyTipo(cdTipoAvaliacao, cdAluno, cdProduto, cdCurso);
                transaction.Complete();
            }

            return retorno;
        }

        public AlunoEvento getEventosRtpDiarioAula(int cd_escola, int cd_aluno, int cd_professor, DateTime dataAula)
        {
            return daoEvento.getEventosRtpDiarioAula(cd_escola, cd_aluno, cd_professor, dataAula).FirstOrDefault();
        }

        public Aluno getAlunoIsTurmaInDate(int cd_turma, int cd_aluno, int cd_pessoa_escola, DateTime dtAula)
        {
            return daoEvento.getAlunoIsTurmaInDate(cd_turma, cd_aluno, cd_pessoa_escola, dtAula).FirstOrDefault();
        }

        #endregion

        #region PerdaMaterial
        public IEnumerable<PerdaMaterialUI> getPerdaMaterialSearch(SearchParameters parametros, int? cd_aluno, int? nm_contrato, int? cd_movimento, int? cd_item, DateTime? dtInicio, DateTime? dtTermino, int status, int cd_escola)
        {
            IEnumerable<PerdaMaterialUI> retorno = new List<PerdaMaterialUI>();
            using (var transaction =
                   TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "id_status_perda";
                parametros.sort = parametros.sort.Replace("dc_nm_movimento", "nm_movimento");
                parametros.sort = parametros.sort.Replace("dta_perda_material", "dt_perda_material");
                parametros.sort = parametros.sort.Replace("dc_status_perda", "id_status_perda");
                retorno = daoPerdaMaterial.getPerdaMaterialSearch(parametros, cd_aluno, nm_contrato, cd_movimento, cd_item, dtInicio, dtTermino, status, cd_escola).ToList();
                transaction.Complete();
            }

            return retorno;
        }

        public PerdaMaterialUI getPerdaMaterialForGrid(int cd_perda_material)
        {
            return daoPerdaMaterial.getPerdaMaterialForGrid(cd_perda_material);

        }

        public PerdaMaterialUI addPerdaMaterial(PerdaMaterial entity)
        {
            PerdaMaterialUI perdaMaterial = null;
            PerdaMaterial perdaSave = null;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                perdaSave = daoPerdaMaterial.add(entity, false);
                transaction.Complete();
            }

            if (perdaSave != null)
            {
                perdaMaterial = daoPerdaMaterial.getPerdaMaterialForGrid(perdaSave.cd_perda_material);
            }

            return perdaMaterial;
        }

        public PerdaMaterialUI editPerdaMaterial(PerdaMaterial entity)
        {
            PerdaMaterialUI perdaMaterial = null;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                PerdaMaterial perdaMaterialBD = daoPerdaMaterial.findById(entity.cd_perda_material, false);
                if (perdaMaterialBD != null && perdaMaterialBD.id_status_perda == (int)PerdaMaterialDataAccess.TipoPerdaMaterialStatus.FECHADO)
                { 
                    throw new CoordenacaoBusinessException(Messages.msgEdicaoPerdaMaterialFechada, null, CoordenacaoBusinessException.TipoErro.ERRO_EDICAO_PERDA_MATERIAL_FECHADO, false);
                    
                }

                perdaMaterialBD.copy(entity, true);
                daoPerdaMaterial.saveChanges(false);
                transaction.Complete();
            }

            
            perdaMaterial = daoPerdaMaterial.getPerdaMaterialForGrid(entity.cd_perda_material);
            

            return perdaMaterial;
        }
        public bool deletePerdaMaterial(List<PerdaMaterial> perdaMaterial)
        {

            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (PerdaMaterial perdaMat in perdaMaterial)
                {
                    PerdaMaterial perdaBd = daoPerdaMaterial.findById(perdaMat.cd_perda_material, false);

                    if (perdaBd != null && perdaBd.id_status_perda == (int)PerdaMaterialDataAccess.TipoPerdaMaterialStatus.FECHADO)
                    {
                        throw new CoordenacaoBusinessException(Messages.msgExclusaoPerdaMaterialFechada, null, CoordenacaoBusinessException.TipoErro.ERRO_EXCLUSAO_PERDA_MATERIAL_FECHADO, false);

                    }

                    
                    daoPerdaMaterial.delete(perdaBd, false);
                }

                deleted = true;

                transaction.Complete();
                return deleted;

            }
        }

        public int processarPerdaMaterial(PerdaMaterial perdaMaterial, int cd_usuario, int fuso)
        {
            return daoPerdaMaterial.processarPerdaMaterial(perdaMaterial, cd_usuario, fuso);
        }

        #endregion

        public ProfessorCargaHorariaMaximaResultUI getExisteCargaHorariaProximaMaxima(int cdPessoaUsuario, int cdEscola)
        {
            return alunoBusiness.getExisteCargaHorariaProximaMaxima(cdPessoaUsuario, cdEscola);
        }

    }

}
