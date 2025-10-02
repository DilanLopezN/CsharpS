using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using System.Transactions;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAcess;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using System.Data.Entity;
using System.Collections;
using System.Configuration;
using System.Data;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
using System.IO;
using FundacaoFisk.SGF.Utils.Messages;
using System.Data.Entity.Infrastructure;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericController;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Utils;
using System.Web;
using Microsoft.Reporting.WebForms;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Business
{
    public class SecretariaBusiness : ISecretariaBusiness
    {
        DataTable dtHistoricoTurma;
        DataTable dtHistoricoTurmaConceito;
        DataTable dtHistoricoEstagioConceito;
        DataTable dtHistoricoTurmaAvaliacao;
        DataTable dtHistoricoAvaliacaoTurma;
        DataTable dtHistoricoEstagio;
        DataTable dtHistoricoAvaliacaoEstagio;
        DataTable dtHistoricoEventoAula1;
        DataTable dtHistoricoEventoAula2;
        DataTable dtHistoricoTitulo;
        DataTable dtHistoricoObs;
        DataTable dtHistoricoAtividade;
        DataTable dtHistoricoFollow;
        DataTable dtHistoricoItem;
        public IEscolaridadeDataAccess DataAccessEscolaridade { get; set; }
        public IMidiaDataAccess DataAccessMidia { get; set; }
        public ITipoContatoDataAccess DataAccessTipoContato { get; set; }
        public IMotivoMatriculaDataAccess DataAccessMotivoMatricula { get; set; }
        public IMotivoNaoMatriculaDataAccess DataAccessMotivoNaoMatricula { get; set; }
        public IMotivoBolsaDataAccess DataAccessMotivoBolsa { get; set; }
        public IMotivoCancelamentoBolsaDataAccess DataAccessMotivoCancelBolsa { get; set; }
        public ILocalidadeBusiness BusinessLoc { get; set; }
        public IPessoaBusiness BusinessPessoa { get; set; }
        public IJsonTesteDataAccess DataAccessJsonTeste { get; set; }
        public IProspectDataAccess DataAccessProspect { get; set; }
        public IProspectDiaDataAccess DataAccessProspectDia { get; set; }
        public IProspectProdutoDataAccess DataAccessProspectProduto { get; set; }
        public IProspectPeriodoDataAccess DataAccessProspectPeriodo { get; set; }
        public IFollowUpDataAccess DataAccessFollowUp { get; set; }
        public IProspectMotivoNaoMatriculaDataAccess DataAccessProspectMotivoNaoMatricula { get; set; }
        public IEmpresaBusiness BusinessEmpresa { get; set; }
        public INomeContratoDataAccess DataAccessNomeContrato { get; set; }
        public IHistoricoAlunoDataAccess DataAccessHistoricoAluno { get; set; }
        public IDesistenciaDataAccess DataAccessDesistencia { get; set; }
        public IAlunoBusiness BusinessAluno { get; set; }
        public IFinanceiroBusiness BusinessFinanceiro { get; set; }
        public IAcaoFollowupDataAccess DataAccessAcaoFollowup { get; set; }
        public IFollowUpEscolaDataAccess DataAccessFollowUpEscola { get; set; }
        public IFollowUpUsuarioDataAccess DataAccessFollowUpUsuario { get; set; }
        public IAnoEscolarDataAccess DataAccessAnoEscolar { get; set; }
        public IMatriculaDataAccess DataAccessMatricula { get; set; }
        public IGeraNotasXmlDataAccess DataAccessGeraNotasXml { get; set; }
        //public ISmsDataAcccess DataAccessSms { get; set; }
        //public ISmsMensagempadraoDataAccess DataAccessMensagemPadraoSms { get; set; }
        public IAlunoDataAccess DataAccessAluno { get; set; }
        public IApiNewCyberAlunoBusiness BusinessApiNewCyberAluno { get; set; }
        public IOrgaoFinanceiroDataAccess DataAccessOrgaoFinanceiro { get; set; }
        public IAlunoRestricaoDataAccess DataAccessAlunoRestricao { get; set; }
        public ITransferenciaAlunoDataAccess DataAccessTransferenciaAluno { get; set; }

        public IMotivoTransferenciaDataAccess DataAccessMotivoTransferencia { get; set; }

        public IApiPromocaoIntercambioProspectBussiness BusinessApiPromocaoIntercambioProspect { get; set; }
        public IPessoaPromocaoDataAccess DataAccessPessoaPromocao { get; set; }

        const int ADD = 1, EDIT = 2;
        const int TELEFONE = 1;// int EMAIL = 4; int SITE = 5; int CELULAR = 3;

        public SecretariaBusiness(IEscolaridadeDataAccess dataAccessEscolaridade,
                                  IMidiaDataAccess dataAccessMidia, ITipoContatoDataAccess dataAccessTipoContato,
                                  IMotivoMatriculaDataAccess dataAccessMotivoMatricula, IMotivoNaoMatriculaDataAccess dataAccessMotivoNaoMatricula,
                                  IMotivoBolsaDataAccess dataAccessMotivoBolsa, IMotivoCancelamentoBolsaDataAccess dataAccessMotivoCancelBolsa,
                                  ILocalidadeBusiness businessLoc, IPessoaBusiness businessPessoa, IJsonTesteDataAccess dataAccessJsonTeste,
                                  IProspectDataAccess dataAccessProspect, IProspectDiaDataAccess dataAccessProspectDia, IProspectProdutoDataAccess dataAccessProspectProduto, IProspectPeriodoDataAccess dataAccessProspectPeriodo, IFollowUpDataAccess dataAccessFollowUp,
                                  IProspectMotivoNaoMatriculaDataAccess dataAccessProspectMotivoNaoMatricula, IEmpresaBusiness businessEmpresa, INomeContratoDataAccess dataAccessNoCont,
                                  IHistoricoAlunoDataAccess dataAccessHistoricoAluno, IDesistenciaDataAccess dataAccessDesistencia, IAlunoBusiness businessAluno, IFinanceiroBusiness businessFinanceiro,
                                  IAcaoFollowupDataAccess dataAccessAcaoFollowup, IFollowUpEscolaDataAccess dataAccessFollowUpEscola, IFollowUpUsuarioDataAccess dataAccessFollowUpUsuario, IAnoEscolarDataAccess dataAccessAnoEscolar,
                                  IMatriculaDataAccess dataAcsesssMatricula, IGeraNotasXmlDataAccess dataAccessGeraNotasXml, 
                                  //ISmsDataAcccess dataAccessSms, ISmsMensagempadraoDataAccess dataAccessSmsMensagemPadrao,
                                  IAlunoDataAccess dataAccessAluno, IOrgaoFinanceiroDataAccess dataAccessOrgaoFinanceiro,
                                  IAlunoRestricaoDataAccess dataAccessAlunoRestricao,IApiNewCyberAlunoBusiness businessApiNewCyberAluno,
                                  IMotivoTransferenciaDataAccess dataAccessMotivoTransferencia, ITransferenciaAlunoDataAccess dataAccessTransferenciaAluno,
                                    IApiPromocaoIntercambioProspectBussiness businessApiPromocaoIntercambioProspect, IPessoaPromocaoDataAccess dataAccessPessoaPromocao
                                  )
        {
            if (dataAccessEscolaridade == null || dataAccessMidia == null || dataAccessTipoContato == null || dataAccessMotivoMatricula == null
                || dataAccessMotivoNaoMatricula == null || dataAccessMotivoBolsa == null || dataAccessMotivoCancelBolsa == null
                || businessLoc == null || businessPessoa == null || dataAccessJsonTeste == null
                || dataAccessProspect == null || dataAccessProspectDia == null || dataAccessProspectProduto == null || dataAccessProspectPeriodo == null
                || dataAccessFollowUp == null || dataAccessProspectMotivoNaoMatricula == null || businessEmpresa == null
                || dataAccessNoCont == null || dataAccessHistoricoAluno == null || dataAccessDesistencia == null || businessAluno == null || businessFinanceiro == null
                || dataAccessAcaoFollowup == null || dataAccessFollowUpEscola == null || dataAccessFollowUpUsuario == null || dataAccessAnoEscolar == null
                || dataAcsesssMatricula == null || dataAccessGeraNotasXml == null 
                 || dataAccessAluno == null || dataAccessOrgaoFinanceiro == null ||
                dataAccessAlunoRestricao == null || businessApiNewCyberAluno == null || dataAccessMotivoTransferencia == null || dataAccessTransferenciaAluno == null ||
            businessApiPromocaoIntercambioProspect == null || dataAccessPessoaPromocao == null)
            {
                throw new ArgumentNullException("repository");
            }
//|| dataAccessSms == null || dataAccessSmsMensagemPadrao == null
            this.DataAccessEscolaridade = dataAccessEscolaridade;
            this.DataAccessMidia = dataAccessMidia;
            this.DataAccessTipoContato = dataAccessTipoContato;
            this.DataAccessMotivoMatricula = dataAccessMotivoMatricula;
            this.DataAccessMotivoNaoMatricula = dataAccessMotivoNaoMatricula;
            this.DataAccessMotivoBolsa = dataAccessMotivoBolsa;
            this.DataAccessMotivoCancelBolsa = dataAccessMotivoCancelBolsa;
            this.DataAccessJsonTeste = dataAccessJsonTeste;
            this.DataAccessProspect = dataAccessProspect;
            this.DataAccessProspectDia = dataAccessProspectDia;
            this.DataAccessProspectProduto = dataAccessProspectProduto;
            this.DataAccessProspectPeriodo = dataAccessProspectPeriodo;
            this.DataAccessFollowUp = dataAccessFollowUp;
            this.DataAccessProspectMotivoNaoMatricula = dataAccessProspectMotivoNaoMatricula;
            this.DataAccessNomeContrato = dataAccessNoCont;
            this.DataAccessMotivoTransferencia = dataAccessMotivoTransferencia;
            BusinessLoc = businessLoc;
            BusinessPessoa = businessPessoa;
            BusinessEmpresa = businessEmpresa;
            this.DataAccessHistoricoAluno = dataAccessHistoricoAluno;
            this.DataAccessDesistencia = dataAccessDesistencia;
            BusinessAluno = businessAluno;
            BusinessFinanceiro = businessFinanceiro;
            this.DataAccessAcaoFollowup = dataAccessAcaoFollowup;
            this.DataAccessFollowUpEscola = dataAccessFollowUpEscola;
            this.DataAccessFollowUpUsuario = dataAccessFollowUpUsuario;
            this.DataAccessAnoEscolar = dataAccessAnoEscolar;
            this.DataAccessMatricula = dataAcsesssMatricula;
            this.DataAccessGeraNotasXml = dataAccessGeraNotasXml;
            //this.DataAccessSms = dataAccessSms;
            //this.DataAccessMensagemPadraoSms = dataAccessSmsMensagemPadrao;
            this.DataAccessAluno = dataAccessAluno;
            this.DataAccessOrgaoFinanceiro = dataAccessOrgaoFinanceiro;
            this.DataAccessAlunoRestricao = dataAccessAlunoRestricao;
            this.BusinessApiNewCyberAluno = businessApiNewCyberAluno;
            this.DataAccessTransferenciaAluno = dataAccessTransferenciaAluno;
            this.BusinessApiPromocaoIntercambioProspect = businessApiPromocaoIntercambioProspect;
            this.DataAccessPessoaPromocao = dataAccessPessoaPromocao;
        }

        public IEscolaridadeDataAccess getDataAccessEscolaridade()
        {
            return DataAccessEscolaridade;
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa)
        {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.DataAccessEscolaridade.DB()).IdUsuario = ((SGFWebContext)this.DataAccessMidia.DB()).IdUsuario = ((SGFWebContext)this.DataAccessTipoContato.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessMotivoMatricula.DB()).IdUsuario = ((SGFWebContext)this.DataAccessMotivoNaoMatricula.DB()).IdUsuario = ((SGFWebContext)this.DataAccessMotivoBolsa.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessMotivoCancelBolsa.DB()).IdUsuario = 
            ((SGFWebContext)this.DataAccessProspect.DB()).IdUsuario = ((SGFWebContext)this.DataAccessJsonTeste.DB()).IdUsuario = ((SGFWebContext)this.DataAccessFollowUp.DB()).IdUsuario = ((SGFWebContext)this.DataAccessHistoricoAluno.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessDesistencia.DB()).IdUsuario = 
            ((SGFWebContext)this.DataAccessAcaoFollowup.DB()).IdUsuario = ((SGFWebContext)this.DataAccessFollowUpEscola.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessFollowUpUsuario.DB()).IdUsuario = 
            //((SGFWebContext)this.DataAccessSms.DB()).IdUsuario = 
            //((SGFWebContext)this.DataAccessMensagemPadraoSms.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessAluno.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessOrgaoFinanceiro.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessMotivoTransferencia.DB()).IdUsuario = 
            ((SGFWebContext)this.DataAccessTransferenciaAluno.DB()).IdUsuario = 
            ((SGFWebContext)this.DataAccessPessoaPromocao.DB()).IdUsuario = cdUsuario;

            ((SGFWebContext)this.DataAccessEscolaridade.DB()).cd_empresa = ((SGFWebContext)this.DataAccessMidia.DB()).cd_empresa = ((SGFWebContext)this.DataAccessTipoContato.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessMotivoMatricula.DB()).cd_empresa = ((SGFWebContext)this.DataAccessMotivoNaoMatricula.DB()).cd_empresa = ((SGFWebContext)this.DataAccessMotivoBolsa.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessMotivoCancelBolsa.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessProspect.DB()).cd_empresa = ((SGFWebContext)this.DataAccessJsonTeste.DB()).cd_empresa = ((SGFWebContext)this.DataAccessProspectDia.DB()).cd_empresa = ((SGFWebContext)this.DataAccessFollowUp.DB()).cd_empresa = ((SGFWebContext)this.DataAccessHistoricoAluno.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessDesistencia.DB()).cd_empresa = ((SGFWebContext)this.DataAccessAcaoFollowup.DB()).cd_empresa = ((SGFWebContext)this.DataAccessFollowUpEscola.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessFollowUpUsuario.DB()).cd_empresa = ((SGFWebContext)this.DataAccessAnoEscolar.DB()).cd_empresa =
            //((SGFWebContext)this.DataAccessGeraNotasXml.DB()).cd_empresa = 
            //((SGFWebContext)this.DataAccessSms.DB()).cd_empresa = ((SGFWebContext)this.DataAccessMensagemPadraoSms.DB()).cd_empresa = 
            ((SGFWebContext)this.DataAccessAluno.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessOrgaoFinanceiro.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessMotivoTransferencia.DB()).cd_empresa = 
            ((SGFWebContext)this.DataAccessTransferenciaAluno.DB()).cd_empresa = 
            ((SGFWebContext)this.DataAccessPessoaPromocao.DB()).cd_empresa = cd_empresa;


            this.BusinessPessoa.configuraUsuario(cdUsuario, cd_empresa);
            this.BusinessLoc.configuraUsuario(cdUsuario, cd_empresa);
            this.BusinessAluno.configuraUsuario(cdUsuario, cd_empresa);
            this.BusinessFinanceiro.configuraUsuario(cdUsuario, cd_empresa);
            this.BusinessEmpresa.configuraUsuario(cdUsuario, cd_empresa);
        }

        public void sincronizarContextos(DbContext dbContext)
        {
            //this.DataAccessHistoricoAluno.sincronizaContexto(dbContext);
            //this.DataAccessDesistencia.sincronizaContexto(dbContext);
            //this.DataAccessTipoContato.sincronizaContexto(dbContext);
            //this.DataAccessEscolaridade.sincronizaContexto(dbContext);
            //this.DataAccessMidia.sincronizaContexto(dbContext);
            //this.DataAccessMotivoMatricula.sincronizaContexto(dbContext);
            //this.DataAccessMotivoNaoMatricula.sincronizaContexto(dbContext);
            //this.DataAccessMotivoBolsa.sincronizaContexto(dbContext);
            //this.DataAccessMotivoCancelBolsa.sincronizaContexto(dbContext);
            //this.DataAccessProspect.sincronizaContexto(dbContext);
            //this.DataAccessProspectProduto.sincronizaContexto(dbContext);
            //this.DataAccessProspectPeriodo.sincronizaContexto(dbContext);
            //this.DataAccessFollowUp.sincronizaContexto(dbContext);
            //this.DataAccessProspectMotivoNaoMatricula.sincronizaContexto(dbContext);
            //this.DataAccessNomeContrato.sincronizaContexto(dbContext);
            //BusinessAluno.sincronizaContexto(dbContext);
            //this.BusinessFinanceiro.sincronizarContextos(dbContext);
            //this.DataAccessAcaoFollowup.sincronizaContexto(dbContext);
            //this.DataAccessFollowUpEscola.sincronizaContexto(dbContext);
            //this.DataAccessFollowUpUsuario.sincronizaContexto(dbContext);
            //this.DataAccessAnoEscolar.sincronizaContexto(dbContext);
        }

        #region Escolaridade
        public Escolaridade GetEscolaridadeById(int id)
        {
            return DataAccessEscolaridade.findById(id, false);
        }

        public Escolaridade PostEscolaridade(Escolaridade escolaridade)
        {
            DataAccessEscolaridade.add(escolaridade, false);
            return escolaridade;
        }

        public Escolaridade PutEscolaridade(Escolaridade escolaridade)
        {
            DataAccessEscolaridade.editE(escolaridade);
            return escolaridade;
        }

        public bool DeleteEscolaridade(List<Escolaridade> escolaridades)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = DataAccessEscolaridade.deleteAll(escolaridades);
                transaction.Complete();
            }
            return deleted;
        }

        public IEnumerable<Escolaridade> GetEscolaridadeSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            IEnumerable<Escolaridade> retorno = new List<Escolaridade>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_escolaridade";
                parametros.sort = parametros.sort.Replace("escolaridade_ativa", "id_escolaridade_ativa");

                retorno = DataAccessEscolaridade.GetEscolaridadeSearch(parametros, descricao, inicio, ativo);
                transaction.Complete();
            }
            return retorno;
        }
        public IEnumerable<Escolaridade> getEscolaridade(bool? status)
        {
            return DataAccessEscolaridade.getEscolaridade(status);
        }

        #endregion

        #region Midia
        public IEnumerable<Midia> getMidia(bool? status, MidiaDataAccess.TipoConsultaMidiaEnum tipo, int? cd_empresa)
        {
            return DataAccessMidia.getMidias(status, tipo, cd_empresa);
        }

        public string getNomeAtendente(int cdUsuario, int cdEscola)
        {
            return DataAccessProspect.getNomeAtendente(cdUsuario, cdEscola);
        }

        //Midia
        public IEnumerable<Midia> GetMidiaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            IEnumerable<Midia> retorno = new List<Midia>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_midia";
                parametros.sort = parametros.sort.Replace("midia_ativa", "id_midia_ativa");

                retorno = DataAccessMidia.GetMidiaSearch(parametros, descricao, inicio, ativo);
                transaction.Complete();
            }
            return retorno;
        }

        public Midia GetMidiaById(int id)
        {
            return DataAccessMidia.findById(id, false);
        }

        public Midia PostMidia(Midia midia)
        {
            DataAccessMidia.add(midia, false);
            return midia;
        }

        public Midia PutMidia(Midia midia)
        {
            DataAccessMidia.edit(midia, false);
            return midia;
        }

        public bool DeleteMidia(List<Midia> midia)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = DataAccessMidia.deleteAll(midia);
                transaction.Complete();
            }
            return deleted;
        }

        #endregion

        #region Tipo Contato
        //Tipo Contato
        public IEnumerable<TipoContato> GetTipoContatoSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            IEnumerable<TipoContato> retorno = new List<TipoContato>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "dc_tipo_contato";
                parametros.sort = parametros.sort.Replace("tipo_contato_ativo", "id_tipo_contato_ativo");

                retorno = DataAccessTipoContato.GetTipoContatoSearch(parametros, descricao, inicio, ativo);
                transaction.Complete();
            }
            return retorno;
        }

        public TipoContato GetTipoContatoById(int id)
        {
            return DataAccessTipoContato.findById(id, false);
        }

        public TipoContato PostTipoContato(TipoContato tipocontato)
        {
            DataAccessTipoContato.add(tipocontato, false);
            return tipocontato;
        }

        public TipoContato PutTipoContato(TipoContato tipocontato)
        {
            DataAccessTipoContato.edit(tipocontato, false);
            return tipocontato;
        }

        public bool DeleteTipoContato(List<TipoContato> tipocontato)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = DataAccessTipoContato.deleteAll(tipocontato);
                transaction.Complete();
            }
            return deleted;
        }
        #endregion

        #region Motivo Matricula
        //Motivo Matricula
        public IEnumerable<MotivoMatricula> GetMotivoMatriculaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            IEnumerable<MotivoMatricula> retorno = new List<MotivoMatricula>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "dc_motivo_matricula";
                parametros.sort = parametros.sort.Replace("motivo_matricula_ativo", "id_motivo_matricula_ativo");

                retorno = DataAccessMotivoMatricula.GetMotivoMatriculaSearch(parametros, descricao, inicio, ativo);
                transaction.Complete();
            }
            return retorno;
        }

        public MotivoMatricula GetMotivoMatriculaById(int id)
        {
            return DataAccessMotivoMatricula.findById(id, false);
        }

        public MotivoMatricula PostMotivoMatricula(MotivoMatricula motivomatricula)
        {
            DataAccessMotivoMatricula.add(motivomatricula, false);
            return motivomatricula;
        }

        public MotivoMatricula PutMotivoMatricula(MotivoMatricula motivomatricula)
        {
            DataAccessMotivoMatricula.edit(motivomatricula, false);
            return motivomatricula;
        }

        public bool DeleteMotivoMatricula(List<MotivoMatricula> motivomatricula)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = DataAccessMotivoMatricula.deleteAll(motivomatricula);
                transaction.Complete();
            }
            return deleted;
        }

        #endregion

        #region Motivo Não Matricula
        //Motivo Não Matricula
        public IEnumerable<MotivoNaoMatricula> GetMotivoNaoMatriculaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            IEnumerable<MotivoNaoMatricula> retorno = new List<MotivoNaoMatricula>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "dc_motivo_nao_matricula";
                parametros.sort = parametros.sort.Replace("motivo_nao_matricula_ativo", "id_motivo_nao_matricula_ativo");

                retorno = DataAccessMotivoNaoMatricula.GetMotivoNaoMatriculaSearch(parametros, descricao, inicio, ativo);
                transaction.Complete();
            }
            return retorno;
        }

        public MotivoNaoMatricula GetMotivoNaoMatriculaById(int id)
        {
            return DataAccessMotivoNaoMatricula.findById(id, false);
        }

        public MotivoNaoMatricula PostMotivoNaoMatricula(MotivoNaoMatricula motivonaomatricula)
        {
            DataAccessMotivoNaoMatricula.add(motivonaomatricula, false);
            return motivonaomatricula;
        }

        public MotivoNaoMatricula PutMotivoNaoMatricula(MotivoNaoMatricula motivonaomatricula)
        {
            DataAccessMotivoNaoMatricula.edit(motivonaomatricula, false);
            return motivonaomatricula;
        }

        public IEnumerable<MotivoNaoMatricula> getMotivoNaoMatriculaProspect(int cdProspect)
        {
            return DataAccessMotivoNaoMatricula.getMotivoNaoMatriculaProspect(cdProspect);

        }

        public bool DeleteMotivoNaoMatricula(List<MotivoNaoMatricula> motivonaomatricula)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = DataAccessMotivoNaoMatricula.deleteAll(motivonaomatricula);
                transaction.Complete();
            }
            return deleted;
        }

        public IEnumerable<MotivoNaoMatricula> getMotivosNaoMatricula()
        {
            return DataAccessMotivoNaoMatricula.getMotivosNaoMatricula();
        }

        #endregion

        #region Motivo Bolsa

        //Motivo Bolsa
        public IEnumerable<MotivoBolsa> GetMotivoBolsaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            IEnumerable<MotivoBolsa> retorno = new List<MotivoBolsa>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "dc_motivo_bolsa";
                parametros.sort = parametros.sort.Replace("motivo_bolsa_ativo", "id_motivo_bolsa_ativo");

                retorno = DataAccessMotivoBolsa.GetMotivoBolsaSearch(parametros, descricao, inicio, ativo);
                transaction.Complete();
            }
            return retorno;
        }

        public MotivoBolsa GetMotivoBolsaById(int id)
        {
            return DataAccessMotivoBolsa.findById(id, false);
        }

        public MotivoBolsa PostMotivoBolsa(MotivoBolsa motivobolsa)
        {
            DataAccessMotivoBolsa.add(motivobolsa, false);
            return motivobolsa;
        }

        public MotivoBolsa PutMotivoBolsa(MotivoBolsa motivobolsa)
        {
            DataAccessMotivoBolsa.edit(motivobolsa, false);
            return motivobolsa;
        }

        public bool DeleteMotivoBolsa(List<MotivoBolsa> motivobolsa)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = DataAccessMotivoBolsa.deleteAll(motivobolsa);
                transaction.Complete();
            }
            return deleted;
        }

        public IEnumerable<MotivoBolsa> getMotivoBolsa(bool? status, int? cd_motivo_bolsa)
        {
            return DataAccessMotivoBolsa.getMotivoBolsa(status, cd_motivo_bolsa);
        }

        #endregion

        #region Motivo Cancelamento Bolsa
        //Motivo Cancelamento Bolsa
        public IEnumerable<MotivoCancelamentoBolsa> GetMotivoCancelBolsaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            IEnumerable<MotivoCancelamentoBolsa> retorno = new List<MotivoCancelamentoBolsa>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "dc_motivo_cancelamento_bolsa";
                parametros.sort = parametros.sort.Replace("motivo_cancelamento_bolsa_ativo", "id_motivo_cancelamento_bolsa_ativo");

                retorno = DataAccessMotivoCancelBolsa.GetMotivoCancelBolsaSearch(parametros, descricao, inicio, ativo);
                transaction.Complete();
            }
            return retorno;
        }

        public MotivoCancelamentoBolsa GetMotivoCancelBolsaById(int id)
        {
            return DataAccessMotivoCancelBolsa.findById(id, false);
        }

        public MotivoCancelamentoBolsa PostMotivoCancelBolsa(MotivoCancelamentoBolsa motivocancelamentobolsa)
        {
            DataAccessMotivoCancelBolsa.add(motivocancelamentobolsa, false);
            return motivocancelamentobolsa;
        }

        public MotivoCancelamentoBolsa PutMotivoCancelBolsa(MotivoCancelamentoBolsa motivocancelamentobolsa)
        {
            DataAccessMotivoCancelBolsa.edit(motivocancelamentobolsa, false);
            return motivocancelamentobolsa;
        }

        public bool DeleteMotivoCancelBolsa(List<MotivoCancelamentoBolsa> motivocancelamentobolsa)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = DataAccessMotivoCancelBolsa.deleteAll(motivocancelamentobolsa);
                transaction.Complete();
            }
            return deleted;
        }

        public IEnumerable<MotivoCancelamentoBolsa> getMotivoCancelamentoBolsa(bool? status, int? cd_motivo_cancelamento_bolsa)
        {
            return DataAccessMotivoCancelBolsa.getMotivoCancelamentoBolsa(status, cd_motivo_cancelamento_bolsa);
        }
        #endregion

        #region Pessoa

        public PessoaFisicaSGF postInsertPessoaFisica(PessoaFisicaUI pessoaFisicaUI, List<RelacionamentoSGF> relacionamentos, int cdEscola)
        {
            PessoaFisicaSGF pessoaFisica = new PessoaFisicaSGF();
            FundacaoFisk.SGF.Web.Services.Pessoa.Model.PessoaSearchUI pessoaSearch = new FundacaoFisk.SGF.Web.Services.Pessoa.Model.PessoaSearchUI();
            PessoaEscola pessoaEsc = new PessoaEscola();
            SGFWebContext db = BusinessPessoa.DBPessoa();
            BusinessPessoa.sincronizaContexto(db);
            BusinessEmpresa.sincronizaContexto(db);
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (pessoaFisicaUI.pessoaFisica.cd_pessoa <= 0)
                {
                    pessoaFisicaUI.pessoaFisica.nm_natureza_pessoa = 1;
                    pessoaFisica = BusinessPessoa.postInsertPessoaFisica(pessoaFisicaUI, relacionamentos,false);
                }
                else
                    pessoaFisica = BusinessPessoa.postUpdatePessoaFisica(pessoaFisicaUI, relacionamentos,false, true);
                pessoaEsc = new PessoaEscola
                {
                    cd_escola = cdEscola,
                    cd_pessoa = pessoaFisica.cd_pessoa
                };
                BusinessEmpresa.postEmpresaPessoa(pessoaEsc);
                if (relacionamentos != null)
                {
                    List<RelacionamentoSGF> rel = relacionamentos;
                    if (relacionamentos.Count() > 0)
                        for (int i = 0; i < relacionamentos.Count(); i++)
                        {
                            pessoaEsc = new PessoaEscola
                            {
                                cd_escola = cdEscola,
                                cd_pessoa = rel[i].cd_pessoa_filho
                            };
                            BusinessEmpresa.postEmpresaPessoa(pessoaEsc);
                        }
                }
                transaction.Complete();
            }
            return pessoaFisica;

        }

        public PessoaJuridicaSGF postInsertPessoaJuridica(PessoaJuridicaUI pessoaJuridicaUI, List<RelacionamentoSGF> relacionamentos, int cdEscola)
        {
            PessoaJuridicaSGF pessoaJuridica = new PessoaJuridicaSGF();
            FundacaoFisk.SGF.Web.Services.Pessoa.Model.PessoaSearchUI pessoaSearch = new FundacaoFisk.SGF.Web.Services.Pessoa.Model.PessoaSearchUI();
            PessoaEscola pessoaEsc = new PessoaEscola();
            SGFWebContext db = BusinessPessoa.DBPessoa();
            BusinessPessoa.sincronizaContexto(db);
            BusinessEmpresa.sincronizaContexto(db);
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (pessoaJuridicaUI.pessoaJuridica.cd_pessoa <= 0)
                {
                    pessoaJuridicaUI.pessoaJuridica.nm_natureza_pessoa = 2;
                    pessoaJuridica = BusinessPessoa.postInsertPessoaJuridica(pessoaJuridicaUI, relacionamentos,false);
                }
                else
                    pessoaJuridica = BusinessPessoa.postUpdatePessoaJuridica(pessoaJuridicaUI, relacionamentos,false);
                //
                pessoaEsc = new PessoaEscola
                {
                    cd_escola = cdEscola,
                    cd_pessoa = pessoaJuridica.cd_pessoa
                };
                BusinessEmpresa.postEmpresaPessoa(pessoaEsc);
                if (relacionamentos != null)
                {
                    List<RelacionamentoSGF> rel = relacionamentos.ToList();
                    if (relacionamentos.Count() > 0)
                        for (int i = 0; i < relacionamentos.Count(); i++)
                        {
                            pessoaEsc = new PessoaEscola
                            {
                                cd_escola = cdEscola,
                                cd_pessoa = rel[i].cd_pessoa_filho
                            };
                            BusinessEmpresa.postEmpresaPessoa(pessoaEsc);
                        }
                }

                transaction.Complete();
            }
            return pessoaJuridica;

        }

        public PessoaFisicaSGF postUpdatePessoaFisica(PessoaFisicaUI pessoaFisicaUI, List<RelacionamentoSGF> relacionamentos, int cdEscola)
        {
            PessoaFisicaSGF pessoaFisica = new PessoaFisicaSGF();
            PessoaEscola pessoaEsc = new PessoaEscola();
            SGFWebContext db = BusinessPessoa.DBPessoa();
            BusinessPessoa.sincronizaContexto(db);
            BusinessEmpresa.sincronizaContexto(db);
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                pessoaFisica = BusinessPessoa.postUpdatePessoaFisica(pessoaFisicaUI, relacionamentos,false, false);

                //
                pessoaEsc = new PessoaEscola
                {
                    cd_escola = cdEscola,
                    cd_pessoa = pessoaFisicaUI.pessoaFisica.cd_pessoa
                };
                BusinessEmpresa.postEmpresaPessoa(pessoaEsc);
                if (relacionamentos != null)
                {
                    List<RelacionamentoSGF> rel = relacionamentos;
                    if (relacionamentos.Count() > 0)
                        for (int i = 0; i < relacionamentos.Count(); i++)
                        {
                            pessoaEsc = new PessoaEscola
                            {
                                cd_escola = cdEscola,
                                cd_pessoa = rel[i].cd_pessoa_filho
                            };
                            BusinessEmpresa.postEmpresaPessoa(pessoaEsc);
                        }
                }

                transaction.Complete();
            }
            return pessoaFisica;

        }

        public PessoaJuridicaSGF postUpdatePessoaJuridica(PessoaJuridicaUI pessoaJuridicaUI, List<RelacionamentoSGF> relacionamentos, int cdEscola)
        {

            PessoaJuridicaSGF pessoaJuridica = new PessoaJuridicaSGF();
            PessoaEscola pessoaEsc = new PessoaEscola();
            SGFWebContext db = BusinessPessoa.DBPessoa();
            BusinessPessoa.sincronizaContexto(db);
            BusinessEmpresa.sincronizaContexto(db);
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                pessoaJuridica = BusinessPessoa.postUpdatePessoaJuridica(pessoaJuridicaUI, relacionamentos,false);

                //
                pessoaEsc = new PessoaEscola
                {
                    cd_escola = cdEscola,
                    cd_pessoa = pessoaJuridicaUI.pessoaJuridica.cd_pessoa
                };
                BusinessEmpresa.postEmpresaPessoa(pessoaEsc);
                if (relacionamentos != null)
                {
                    List<RelacionamentoSGF> rel = relacionamentos.ToList();
                    if (relacionamentos.Count() > 0)
                        for (int i = 0; i < relacionamentos.Count(); i++)
                        {
                            pessoaEsc = new PessoaEscola
                            {
                                cd_escola = cdEscola,
                                cd_pessoa = rel[i].cd_pessoa_filho
                            };
                            BusinessEmpresa.postEmpresaPessoa(pessoaEsc);
                        }
                }
                transaction.Complete();
            }
            return pessoaJuridica;

        }

        public PessoaFisicaSGF verificarPessoaFisicaEmail(string email)
        {
            return DataAccessProspect.verificarPessoaFisicaEmail(email);
        }

        public PessoaFisicaSGF verificarPessoaByEmail(string email)
        {
            return BusinessPessoa.verificarPessoaByEmail(email);
        }

        #endregion

        #region Horario

        public IEnumerable<Horario> getHorarioByEscolaForRegistroUncommited(int cdEscola, int cdRegistro, Horario.Origem origem)
        {
            return BusinessAluno.getHorarioByEscolaForRegistroUncommited(cdEscola, cdRegistro, origem).ToList();
        }

        public IEnumerable<Horario> getHorarioByEscolaForRegistro(int cdEscola, int cdRegistro, Horario.Origem origem)
        {
            return BusinessAluno.getHorarioByEscolaForRegistro(cdEscola, cdRegistro, origem).ToList();
        }

        public bool deleteHorario(Horario horario)
        {
            return BusinessAluno.deleteHorario(horario);
        }

        public Horario addHorario(Horario horario)
        {
            return BusinessAluno.addHorario(horario);
        }

        public Horario editHorarioContext(Horario horarioContext, Horario horarioView)
        {
            return BusinessAluno.editHorarioContext(horarioContext, horarioView);
        }

        public IEnumerable<Horario> getHorarioOcupadosForTurma(int cdEscola, int cdRegistro, int[] cdProfessores, int cd_turma, 
            int cd_duracao, int cd_curso, DateTime dt_inicio, DateTime? dt_final, HorarioDataAccess.TipoConsultaHorario tipoCons)
        {
            return BusinessAluno.getHorarioOcupadosForTurma(cdEscola, cdRegistro, cdProfessores, cd_turma, cd_duracao, cd_curso, dt_inicio, dt_final, tipoCons);
        }

        public IEnumerable<Horario> getHorarioOcupadosForSala(Turma turma, int cdEscola, HorarioDataAccess.TipoConsultaHorario tipoCons)
        {
            return BusinessAluno.getHorarioOcupadosForSala(turma, cdEscola, tipoCons);
        }

        public int countHorariosUsuario(int cd_empresa, int cd_usuario)
        {
            return BusinessAluno.getHorarioByEscolaForRegistro(cd_empresa, cd_usuario, Horario.Origem.USUARIO).Count();
        }

        public bool getHorarioByHorario(int cdEscola, int cdRegistro, Horario.Origem origem, TimeSpan hr_servidor, int diaSemanaAtual)
        {
            return BusinessAluno.getHorarioByHorario(cdEscola, cdRegistro, origem, hr_servidor, diaSemanaAtual);
        }

        public void verificaHorarioUsuario(int cdEscola, int cdRegistro, TimeSpan hr_servidor, int diaSemanaAtual)
        {
            bool ok;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                ok = BusinessAluno.getHorarioByHorario(cdEscola, cdRegistro, Horario.Origem.USUARIO, hr_servidor, diaSemanaAtual);
                transaction.Complete();
            }
            if (!ok)
                throw new SecretariaBusinessException(Utils.Messages.Messages.msgHorarioAcessoUsuario, null, SecretariaBusinessException.TipoErro.ERRO_HORARIO_LOGIN, false);
        }

        public string retornaDescricaoHorarioOcupado(int cd_empresa, TimeSpan hr_ini, TimeSpan hr_fim)
        {
            return BusinessAluno.retornaDescricaoHorarioOcupado(cd_empresa, hr_ini, hr_fim);
        }
        #endregion

        #region Prospect

        public Recibo getReciboByProspect(int cd_prospect, int cd_empresa)
        {
            Recibo retorno = new Recibo();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                int? cd_baixa = DataAccessProspect.getBaixaFinanceira(cd_prospect, cd_empresa);
                if (cd_baixa == null || cd_baixa <= 0)
                    throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroReciboSemBaixa, null, SecretariaBusinessException.TipoErro.ERRO_RECIBO_SEM_BAIXA, false);
                retorno = BusinessFinanceiro.getReciboByBaixa(cd_baixa.Value, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public List<ProspectSiteUI> getProspectSite(int cd_prospect, int tipo)
        {
            List<ProspectSiteUI> retorno;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                List<ProspectSiteUI> prospectSite = DataAccessProspect.getProspectSite(cd_prospect, tipo);
                retorno = prospectSite;
                transaction.Complete();
            }
            return retorno;
        }

        public ProspectSearchUI insertProspect(ProspectSearchUI prospect)
        {
            ProspectSearchUI retornProspect = new ProspectSearchUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessProspect.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                this.sincronizarContextos(DataAccessProspect.DB());
                Prospect newProspect = new Prospect();
                PessoaFisicaSGF pessoaFisicaProspect = new PessoaFisicaSGF();
                PessoaSGF pessoaFisica = new PessoaSGF();

                //Verifica se já existe pessoa cadastrada para a escola com o mesmo telefone/celular:
                ProspectSearchUI prospectWithTelfone = DataAccessProspect.verificaExistenciaProspect(prospect.telefone, prospect.celular, prospect.cd_pessoa_escola.Value, 0, prospect.no_pessoa);
                if (prospectWithTelfone != null)
                    throw new PessoaBusinessException(string.Format(Utils.Messages.Messages.msgPessoaExistenteProspect, prospectWithTelfone.no_pessoa), null, PessoaBusinessException.TipoErro.ERRO_TELEFONEJAEXISTENTE, false);

                newProspect.copy(prospect);
                newProspect.dt_matricula_prospect = prospect.dt_matricula.HasValue ? prospect.dt_matricula.Value.Date : prospect.dt_matricula;
                newProspect.vl_matricula_prospect = prospect.vl_matricula;
                newProspect.id_gerar_baixa = prospect.gerar_baixa;
                newProspect.cd_plano_conta = prospect.cd_plano_conta_tit > 0 ? prospect.cd_plano_conta_tit : null;
                newProspect.cd_local_movimento = prospect.cd_local_movto > 0 ? prospect.cd_local_movto : null;
                newProspect.cd_tipo_liquidacao = prospect.cd_tipo_liquidacao > 0 ? prospect.cd_tipo_liquidacao : null;

                var existProspectEmailBases = DataAccessProspect.getExistsProspectEmailENome(prospect.email, (int)prospect.cd_pessoa_escola, prospect.no_pessoa);
                if (existProspectEmailBases != null && existProspectEmailBases.cd_prospect > 0)
                    throw new PessoaBusinessException(string.Format(Utils.Messages.Messages.msgExistsEmailProspect, existProspectEmailBases.no_pessoa), null, FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_EMAILJAEXISTENTE, false);
                else// verifica se existe e-mail para o aluno
                {
                    // to do formatar os retornos para view
                    //var existAlunoEmailBase = BusinessAluno.verificarAlunoExistEmail(prospect.email, (int)prospect.cd_pessoa_escola,0);
                    //if (existAlunoEmailBase != null && existAlunoEmailBase.cd_aluno > 0)
                    //    throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgExistsEmailAluno, existAlunoEmailBase.no_pessoa), null, AlunoBusinessException.TipoErro.ERRO_EMAIL_JA_EXITE, false);
                    //else
                    //{
                         var existePessoaFisicaEmailBase = DataAccessProspect.verificarPessoaFisicaEmailCadProspect(prospect.email);
                         if (existePessoaFisicaEmailBase != null && existePessoaFisicaEmailBase.cd_pessoa > 0)
                         {
                                newProspect.cd_pessoa_fisica = existePessoaFisicaEmailBase.cd_pessoa;
                         }
                         else
                         {
                             pessoaFisicaProspect = new PessoaFisicaSGF
                             {
                                 no_pessoa = prospect.no_pessoa,
                                 dt_cadastramento = Utils.Utils.truncarMilissegundo(prospect.dt_cadastramento.ToUniversalTime()),
                                 nm_sexo = prospect.nm_sexo,
                                 dt_nascimento = prospect.dt_nascimento_prospect
                             };
                             BusinessPessoa.sincronizaContexto(BusinessPessoa.DBPessoa());
                             pessoaFisica = BusinessPessoa.addNewPessoaFisica(pessoaFisicaProspect, prospect.endereco);
                             newProspect.cd_pessoa_fisica = pessoaFisica.cd_pessoa;

                             BusinessPessoa.addEditTipoContato(prospect.telefone, pessoaFisica.cd_pessoa, ADD, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE, null, null);
                             BusinessPessoa.addEditTipoContato(prospect.email, pessoaFisica.cd_pessoa, ADD, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL, null, null);
                             BusinessPessoa.addEditTipoContato(prospect.celular, pessoaFisica.cd_pessoa, ADD, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR, prospect.cd_operadora, null);
                         }

                         
                    //}
                }
                List<FollowUp> listaFollowUp = new List<FollowUp>();
                if(prospect.listaFollowUp != null){
                    listaFollowUp = prospect.listaFollowUp.ToList();
                    foreach(FollowUp followUp in listaFollowUp) {
                        followUp.cd_aluno = null;
                        followUp.cd_escola = prospect.cd_pessoa_escola;
                        followUp.cd_usuario = prospect.cd_usuario;
                        followUp.dt_follow_up = followUp.dt_follow_up.ToUniversalTime();
                        followUp.id_tipo_follow = (byte)FollowUp.TipoFollowUp.PROSPECT_ALUNO;
                    }
                }

                newProspect.ProspectFollowUp = listaFollowUp;
                newProspect = DataAccessProspect.add(newProspect, false);

                
                persistirMotivosNaoMatricula(prospect, newProspect.cd_prospect);
                perstirProdutoProspect(prospect, newProspect.cd_prospect);
                perstirPeriodoProspect(prospect, newProspect.cd_prospect);
                persisitirDiaProspect(prospect, newProspect.cd_prospect);
                prospect.cd_pessoa_fisica = newProspect.cd_pessoa_fisica;
                retornProspect = ProspectSearchUI.fromProspectUI(prospect, newProspect.cd_prospect, pessoaFisicaProspect.dt_cadastramento);
                retornProspect.listaFollowUp = listaFollowUp;

                // Geração do título da pré-matrícula:
                if (prospect.vl_matricula > 0)
                {
                    Titulo titulo = gerarTituloPreMatricula(prospect, newProspect);
                    if (prospect.gerar_baixa)
                        gerarBaixaPreMatricula(titulo, prospect.cd_local_movto.Value, prospect.cd_tipo_liquidacao.Value, prospect.no_pessoa, prospect.cd_usuario);
                }

                if (BusinessApiPromocaoIntercambioProspect.aplicaApiPromocao())
                {

                    //monta o objeto
                    PromocaoIntercambioParams prospectPromocaoIntercambioCurrent = new PromocaoIntercambioParams();
                    prospectPromocaoIntercambioCurrent = DataAccessProspect.findProspectApiPromocaoIntercambio(newProspect.cd_prospect);

                    BusinessApiPromocaoIntercambioProspect.ValidaParametros(prospectPromocaoIntercambioCurrent);
                    string codigo_promocao = BusinessApiPromocaoIntercambioProspect.postExecutaRequestPromocaoIntercambio(prospectPromocaoIntercambioCurrent);

                    if (!string.IsNullOrEmpty(codigo_promocao))
                    {
                    
                        PessoaPromocao pessoaPromocao = new PessoaPromocao();
                        pessoaPromocao.cd_pessoa = newProspect.cd_prospect;
                        pessoaPromocao.id_tipo_pessoa = 2;
                        pessoaPromocao.dc_promocao = codigo_promocao;
                        DataAccessPessoaPromocao.add(pessoaPromocao, false);
                        

                    }

                }


                transaction.Complete();
            }
            return retornProspect;
        }

        public List<int> getProspectDia(int cd_prospect)
        {
            //Busca a lista de ProspectDia 
            IEnumerable<ProspectDia> prospectDias = new List<ProspectDia>();
            List<int> prospectDiasIds = new List<int>();
             prospectDias = DataAccessProspectDia.searchProspectDia(cd_prospect);
             foreach (var item in prospectDias)
             {
                 prospectDiasIds.Add((int)item.id_dia_semana);
             }
             return prospectDiasIds;
        }

        public bool enviarEmailProspectAndUpdIdEmail(ProspectSearchUI prospect, int cdEscola, ICollection<FollowUp> listaFollowUp)
        {
            SendEmail sendEmail = new SendEmail();
            SendEmail.configurarEmailSection(sendEmail);
            sendEmail.destinatario = prospect.email;
            sendEmail.assunto = "E-mail Fisk";            
            var followUpsCtx = DataAccessFollowUp.getFollowUpProspect(prospect.cd_prospect, cdEscola).ToList();

            if (listaFollowUp != null && listaFollowUp.Count > 0)
            {
                foreach (var followUp in listaFollowUp)
                {
                    if (followUp.id_email_enviado_view)
                    {
                        var followUpCtx = followUpsCtx.Where(f => f.cd_follow_up == followUp.cd_follow_up).FirstOrDefault();
                        sendEmail.mensagem = followUp.dc_assunto;

                        var id_email_enviado = SendEmail.EnviarEmail(sendEmail);
                        followUpCtx.id_email_enviado = id_email_enviado;

                        if (!id_email_enviado)
                        {
                            throw new SecretariaBusinessException(string.Format(Messages.msgErroSendEmailProspectAluno, prospect.email), null, SecretariaBusinessException.TipoErro.ERRO_LOCAL_MOVIMENTO_AUSENTE_PRE_MATRICULA, false);
                        }
                    }
                }
                DataAccessFollowUp.saveChanges(false);
            }
            
            
            return true;
        }

        private void gerarBaixaPreMatricula(Titulo titulo, int cd_local_movto, int cd_tipo_liquidacao, string nome, int cd_usuario)
        {
            TransacaoFinanceira transacao = new TransacaoFinanceira();
            BaixaTitulo baixa = new BaixaTitulo();
            List<BaixaTitulo> listaBaixas = new List<BaixaTitulo>();

            transacao.dt_tran_finan = titulo.dt_emissao_titulo.Date;
            transacao.cd_pessoa_empresa = titulo.cd_pessoa_empresa;
            transacao.cd_local_movto = cd_local_movto;
            transacao.cd_tipo_liquidacao = cd_tipo_liquidacao;

            titulo.nomeResponsavel = nome;

            baixa.cd_titulo = titulo.cd_titulo;
            baixa.dt_baixa_titulo = transacao.dt_tran_finan.Value;
            baixa.id_baixa_parcial = false;
            baixa.vl_liquidacao_baixa = titulo.vl_titulo;
            baixa.vl_principal_baixa = titulo.vl_titulo;
            baixa.tx_obs_baixa = "Baixa gerada automaticamente para o prospect " + nome + ".";
            baixa.Titulo = titulo;
            baixa.cd_usuario = cd_usuario;
            listaBaixas.Add(baixa);

            transacao.Baixas = listaBaixas;
            BusinessFinanceiro.postIncluirTransacao(transacao, false);
        }

        private Titulo gerarTituloPreMatricula(ProspectSearchUI prospect, Prospect newProspect)
        {
            Titulo titulo = new Titulo();
            SGFWebContext dbComp = new SGFWebContext();

            titulo.cd_pessoa_empresa = prospect.cd_pessoa_escola.Value;
            titulo.cd_pessoa_titulo = newProspect.cd_pessoa_fisica;
            titulo.cd_pessoa_responsavel = newProspect.cd_pessoa_fisica;
            if (prospect.gerar_baixa)
                titulo.cd_local_movto = prospect.cd_local_movto.Value;
            else
            {
                if (!prospect.parametro.cd_local_movto.HasValue)
                    throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroLocalMovimentoAusentePreMatricula, null, SecretariaBusinessException.TipoErro.ERRO_LOCAL_MOVIMENTO_AUSENTE_PRE_MATRICULA, false);
                titulo.cd_local_movto = prospect.parametro.cd_local_movto.Value;
            }
            titulo.dt_emissao_titulo = prospect.dt_matricula.Value.Date;
            titulo.cd_origem_titulo = newProspect.cd_prospect;
            titulo.dt_vcto_titulo = prospect.dt_matricula.Value.Date;
            titulo.dh_cadastro_titulo = DateTime.UtcNow;
            titulo.vl_titulo = prospect.vl_matricula.Value;
            if (prospect.gerar_baixa)
                titulo.dt_liquidacao_titulo = prospect.dt_matricula;
            titulo.dc_tipo_titulo = "PP";
            titulo.vl_saldo_titulo = prospect.vl_matricula.Value;
            titulo.nm_titulo = newProspect.cd_prospect;
            titulo.nm_parcela_titulo = 1;
            titulo.cd_tipo_financeiro = (int)TipoFinanceiro.TiposFinanceiro.TITULO;
            titulo.id_status_titulo = prospect.gerar_baixa ? (int)Titulo.StatusTitulo.FECHADO : (int)Titulo.StatusTitulo.ABERTO;
            titulo.id_status_cnab = 0;
            titulo.id_origem_titulo = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Prospect"].ToString());
            titulo.id_natureza_titulo = (int)Titulo.NaturezaTitulo.RECEBER;
            titulo.cd_plano_conta_tit = prospect.cd_plano_conta_tit;

            List<Titulo> listaTitulos = new List<Titulo>();
            listaTitulos.Add(titulo);
            listaTitulos = BusinessFinanceiro.addTitulos(listaTitulos);

            return listaTitulos.FirstOrDefault();
        }

        private void persistirMotivosNaoMatricula(ProspectSearchUI prospect, int cdProspect)
        {
            if (prospect.listaMotivos != null && prospect.listaMotivos.Count() > 0)
            {
                List<MotivoNaoMatricula> MotivoNaoMatriculas = new List<MotivoNaoMatricula>();
                List<ProspectMotivoNaoMatricula> novosMotivosNaoMatricula = prospect.listaMotivos.ToList();
                for (int i = 0; i < novosMotivosNaoMatricula.Count; i++)
                    DataAccessProspectMotivoNaoMatricula.add(new ProspectMotivoNaoMatricula()
                    {
                        cd_prospect = cdProspect,
                        cd_motivo_nao_matricula = novosMotivosNaoMatricula[i].cd_motivo_nao_matricula
                    }, false);
            }
        }

        private void perstirProdutoProspect(ProspectSearchUI prospect, int idProspect)
        {
            if (prospect.produtos != null && prospect.produtos.ToArray().Length > 0)
            {
                foreach (var item in prospect.produtos)
                {
                    ProspectProduto prospectProduto = new ProspectProduto
                    {
                        cd_produto = item.cd_produto,
                        cd_prospect = idProspect
                    };
                    DataAccessProspectProduto.add(prospectProduto, false);
                }
            }
        }

        private void perstirPeriodoProspect(ProspectSearchUI prospect, int idProspect)
        {
            if (prospect.periodos != null && prospect.periodos.ToArray().Length > 0)
            {
                foreach (var item in prospect.periodos)
                {
                    ProspectPeriodo prospectPeriodo = new ProspectPeriodo
                    {
                        id_periodo = item.id_periodo,
                        cd_prospect = idProspect
                    };
                    DataAccessProspectPeriodo.add(prospectPeriodo, false);
                }
            }
        }

        private void persisitirDiaProspect(ProspectSearchUI prospect, int idProspect)
        {
            //Incluir ProspectDia
            if (prospect.dias != null && prospect.dias.ToArray().Length > 0)
            {
                foreach (var item in prospect.dias)
                {
                    ProspectDia prospectDia = new ProspectDia
                    {
                        id_dia_semana = item.id_dia_semana,
                        cd_prospect = idProspect
                    };
                    DataAccessProspectDia.add(prospectDia, false);
                }
            }
        }

        private void setProspectMotivoNaoMatricula(IEnumerable<ProspectMotivoNaoMatricula> listaMotivos, int cdProspect, int cd_escola)
        {
            List<ProspectMotivoNaoMatricula> novosMotivosNaoMatricula = listaMotivos != null ? listaMotivos.ToList() : null;
            IEnumerable<MotivoNaoMatricula> antigos = DataAccessMotivoNaoMatricula.getProspectMotivoNaoMatricula(cdProspect, cd_escola);
            var antigosMotivoNaoMatricula = antigos.ToList();

            List<ProspectMotivoNaoMatricula> MotivoNaoMatriculaView = new List<ProspectMotivoNaoMatricula>();
            MotivoNaoMatricula motivoNaoMatricula = new MotivoNaoMatricula();
            DataAccessMotivoNaoMatricula.sincronizaContexto(DataAccessProspect.DB());
            
            MotivoNaoMatriculaView = listaMotivos != null ? listaMotivos.ToList() : null;
            IEnumerable<ProspectMotivoNaoMatricula> MotivoNaoProspectComCodigo;
            IEnumerable<ProspectMotivoNaoMatricula> motivoNaoProspectDeleted = new List<ProspectMotivoNaoMatricula>();

            if (MotivoNaoMatriculaView != null && MotivoNaoMatriculaView.Count() > 0)
            {
                MotivoNaoProspectComCodigo = from hpts in MotivoNaoMatriculaView
                                             where hpts.cd_motivo_nao_matricula != 0
                                             select hpts;
                //motivoNaoProspectDeleted = motivoNaoMatriculaContext.Where(tc => !MotivoNaoProspectComCodigo.Any(tv => tc.cd_motivo_nao_matricula == tv.cd_motivo_nao_matricula));
            }
            if (motivoNaoProspectDeleted.Count() > 0)
            {
                foreach (var item in motivoNaoProspectDeleted)
                    if (item != null)
                        DataAccessProspectMotivoNaoMatricula.delete(item, false);
            }
            else
                if (MotivoNaoMatriculaView == null)
                    foreach (var item in antigosMotivoNaoMatricula)
                        if (item != null)
                        {
                            ProspectMotivoNaoMatricula motivoNaoMatriculaProsp = DataAccessProspectMotivoNaoMatricula.getProspectMotivoNaoMatriculaEsc(cdProspect, cd_escola, item.cd_motivo_nao_matricula);
                            DataAccessProspectMotivoNaoMatricula.delete(motivoNaoMatriculaProsp, false);
                        }

            if (MotivoNaoMatriculaView != null)
                foreach (var item in MotivoNaoMatriculaView)
                    if (item.cd_prospect_motivo_nao_matricula == 0)
                        DataAccessProspectMotivoNaoMatricula.add(new ProspectMotivoNaoMatricula() { cd_prospect = cdProspect, cd_motivo_nao_matricula = item.cd_motivo_nao_matricula }, false);

        }

        private void setProspectFollowUp(IEnumerable<FollowUp> listaFollowUp, int cdProspect, int cd_usuario_atendente, int cd_escola)
        {
            List<FollowUp> followUpView = new List<FollowUp>();
            FollowUp followUp = new FollowUp();
            IEnumerable<FollowUp> followUpContext = DataAccessFollowUp.getFollowUpProspect(cdProspect, cd_escola).ToList();
            followUpView = listaFollowUp != null ? listaFollowUp.ToList() : null;
            IEnumerable<FollowUp> followUpProspectComCodigo;
            IEnumerable<FollowUp> followUpProspectDeleted = new List<FollowUp>();
            if (followUpView != null && followUpView.Count() > 0)
            {
                followUpProspectComCodigo = from hpts in followUpView
                                            where hpts.cd_follow_up != 0
                                            select hpts;
                followUpProspectDeleted = followUpContext.Where(tc => !followUpProspectComCodigo.Any(tv => tc.cd_follow_up == tv.cd_follow_up));
            }
            if (followUpProspectDeleted.Count() > 0)
            {
                foreach (var item in followUpProspectDeleted)
                    if (item != null)
                    {
                        if (cd_usuario_atendente != item.cd_usuario)
                        {
                            throw new AlunoBusinessException(Messages.msgErroPermissaoAlterarFollowUp, null, AlunoBusinessException.TipoErro.ERRO_FOLLOW_UP_DE_OUTRO_ATENDENTE, false);
                        }

                        var itemDelete = DataAccessFollowUp.findById(item.cd_follow_up, false);
                        if (itemDelete != null)
                        {
                            DataAccessFollowUp.deleteContext(itemDelete, false);
                        }

                    }
            }
            else
            if (followUpView == null)
                foreach (var item in followUpContext)
                    if (item != null)
                    {
                        if (cd_usuario_atendente != item.cd_usuario)
                        {
                            throw new AlunoBusinessException(Messages.msgErroPermissaoAlterarFollowUp, null, AlunoBusinessException.TipoErro.ERRO_FOLLOW_UP_DE_OUTRO_ATENDENTE, false);
                        }

                        var itemDel = DataAccessFollowUp.findById(item.cd_follow_up, false);
                        if (itemDel != null)
                        {
                            DataAccessFollowUp.deleteContext(itemDel, false);
                        }

                    }
            if (followUpView != null){
                foreach (var item in followUpView)
                {
                    if (item.cd_follow_up.Equals(null) || item.cd_follow_up == 0)
                    {
                        item.cd_usuario = cd_usuario_atendente;
                        item.cd_prospect = cdProspect;
                        item.cd_escola = cd_escola;
                        item.dt_follow_up = item.dt_follow_up.ToUniversalTime();
                        item.id_tipo_follow = (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO;
                        DataAccessFollowUp.add(item, false);
                    }
                    else if(item.id_alterado)
                    {
                        var followUpProspect = (from hp in followUpContext where hp.cd_follow_up == item.cd_follow_up select hp).FirstOrDefault();
                        FollowUp followDb = DataAccessFollowUp.findById(followUpProspect.cd_follow_up, false);
                        if (followUpProspect != null && followUpProspect.cd_follow_up > 0 && followDb != null && followDb.cd_follow_up > 0)
                        {
                            followDb.dc_assunto = item.dc_assunto;
                            followDb.id_follow_lido = item.id_follow_lido;
                            followDb.id_follow_resolvido = item.id_follow_resolvido;
                            followDb.dt_proximo_contato = item.dt_proximo_contato;
                            followDb.cd_acao_follow_up = item.cd_acao_follow_up;
                            followDb.dt_follow_up = item.dt_follow_up.ToUniversalTime();
                            followDb.id_tipo_atendimento = item.id_tipo_atendimento;
                            followDb.cd_turma = item.cd_turma;
                            if ((DataAccessFollowUp.DB().Entry(followDb).State == System.Data.Entity.EntityState.Modified) && cd_usuario_atendente != item.cd_usuario)
                                throw new AlunoBusinessException(Messages.msgErroPermissaoAlterarFollowUp, null, AlunoBusinessException.TipoErro.ERRO_FOLLOW_UP_DE_OUTRO_ATENDENTE, false);
                        }
                    }
                }
                DataAccessFollowUp.saveChanges(false);
            }

        }

        public IEnumerable<ProspectSearchUI> GetProspectSearch(SearchParameters parametros, string nome, bool inicio, string email, int cdEscola, DateTime? dataIni, DateTime? dataFim, bool? ativo, bool aluno, int testeClassificacaoMatriculaOnline)
        {
            IEnumerable<ProspectSearchUI> retorno = new List<ProspectSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessProspect.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastramento_prospect", "dt_cadastramento");
                retorno = DataAccessProspect.GetProspectSearch(parametros, nome, inicio, email, cdEscola, dataIni, dataFim, ativo, aluno, testeClassificacaoMatriculaOnline).ToList();
                transaction.Complete();
            }
            return retorno;
        }


        public JsonTeste postJsonTeste(JsonTeste jsonTeste)
        {
            JsonTeste retorno = null;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                retorno = DataAccessJsonTeste.add(jsonTeste, false);
                transaction.Complete();
            }
            return retorno;
        }

        public void editJsonTeste(int idJsonTeste, byte statusProcedureErro, string msgErro)
        {
            
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                JsonTeste jsonTeste =  DataAccessJsonTeste.findById(idJsonTeste, false);
                jsonTeste.id_erro = statusProcedureErro;
                jsonTeste.dc_erro = msgErro;
                DataAccessJsonTeste.saveChanges(false);
                transaction.Complete();
            }
           
        }

        public ProspectIntegracaoRetornoUI postProspectIntegracao(Nullable<int> nm_integracao, Nullable<byte> id_tipo, Nullable<int> id_teste, string no_pessoa, string email, string fone, string cep, string day_week, string periodo, Nullable<System.DateTime> dt_cadastro, string sexo, Nullable<double> hit, string phase, string courseId)
        {

            ProspectIntegracaoRetornoUI retorno = new ProspectIntegracaoRetornoUI();
            retorno.cd_prospect = 0;
            retorno.retunvalue = Convert.ToBoolean((int)ProspectIntegracaoUI.StatusProcedure.SUCESSO_EXECUCAO_PROCEDURE);

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                retorno = DataAccessProspect.postProspectIntegracao(nm_integracao, id_tipo, id_teste, no_pessoa, email, fone, cep, day_week, periodo, dt_cadastro, sexo, hit, phase, courseId);
                transaction.Complete();
            }

            return retorno;
        }

        public ProspectGeradoIntegracaoRetornoUI postGetProspectsGeradosSendPromocao()
        {

            ProspectGeradoIntegracaoRetornoUI retorno = new ProspectGeradoIntegracaoRetornoUI();
            retorno.cd_prospect = null;
            retorno.retunvalue = Convert.ToBoolean((int)ProspectIntegracaoUI.StatusProcedure.SUCESSO_EXECUCAO_PROCEDURE);

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                retorno = DataAccessProspect.postGetProspectsGeradosSendPromocao();
                transaction.Complete();
            }

            return retorno;
        }



        public IEnumerable<ProspectSearchUI> getProspectFKSearch(SearchParameters parametros, int cdEscola, string nome, bool inicio, string email, string telefone , ProspectDataAccess.TipoConsultaEnum tipo)
        {
            // Reajusta os parâmetros de ordenação da pesquisa:
            IEnumerable<ProspectSearchUI> retorno = new List<ProspectSearchUI>();

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                retorno = DataAccessProspect.getProspectFKSearch(parametros, cdEscola, nome, inicio, email, telefone ,tipo).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public bool deleteAllProspect(List<Prospect> prospects, int cd_escola)
        {
            bool deletedProspect = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                Prospect prospect = new Prospect();
                PessoaFisicaSGF pessoaFisica = new PessoaFisicaSGF();
                PessoaSGF pessoa = new PessoaSGF();
                if (prospects != null)
                {
                    for (int i = 0; i < prospects.Count(); i++)
                    {
                        var listFollowUpAluno = DataAccessFollowUp.getFollowUpProspect(prospects[i].cd_prospect, cd_escola).ToList();
                        if (listFollowUpAluno != null && listFollowUpAluno.Count > 0)
                        {
                            foreach (FollowUp followUp in listFollowUpAluno)
                            {
                                var listFollowUpDb = DataAccessFollowUp.findById(followUp.cd_follow_up, false);
                                if (listFollowUpDb != null)
                                {
                                    DataAccessFollowUp.deleteContext(listFollowUpDb, false);
                                }
                            }
                        }
                        
                            

                        //deletando prospect
                        prospect = DataAccessProspect.findById(prospects[i].cd_prospect, false);
                        if (prospect != null)
                        {
                            ///
                            SGFWebContext dbComp = new SGFWebContext();
                            Titulo tituloProspect = BusinessFinanceiro.getTitulosByOrigem(prospect.cd_prospect, Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Prospect"].ToString()), prospect.cd_pessoa_escola.Value).FirstOrDefault();
                            if (tituloProspect != null && tituloProspect.id_status_cnab != (int)Titulo.StatusCnabTitulo.INICIAL && tituloProspect.id_status_cnab != (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA)
                                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgNotUpdateTituloEnviadoCNAB, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB, false);
                            //Verificar se já existe baixa para o título gerado
                            BaixaTitulo baixasTitulo = new BaixaTitulo();
                            if (tituloProspect != null && tituloProspect.cd_titulo > 0)
                                baixasTitulo = BusinessFinanceiro.getBaixaTituloByIdTitulo(tituloProspect.cd_titulo, prospect.cd_pessoa_escola.Value).FirstOrDefault();

                            if (baixasTitulo != null && baixasTitulo.cd_baixa_titulo > 0)
                            {
                                if (!prospect.id_gerar_baixa)
                                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroExcluiTituloBaixa, null, FinanceiroBusinessException.TipoErro.ERRO_ALTERAR_PREMATRICULA_BAIXA_MANUAL, false);
                                TransacaoFinanceira transacaoFinanc = BusinessFinanceiro.getTransacaoFinanceira(baixasTitulo.cd_tran_finan, prospect.cd_pessoa_escola.Value);
                                if (transacaoFinanc != null && transacaoFinanc.cd_tran_finan > 0)
                                    BusinessFinanceiro.deleteTransFinanBaixa(transacaoFinanc);
                            }
                            if (tituloProspect != null && tituloProspect.cd_titulo > 0)
                                BusinessFinanceiro.deletarTitulo(tituloProspect, prospect.cd_pessoa_escola.Value);
                            deletedProspect = DataAccessProspect.delete(prospect, false);
                        }
                    }
                }
                transaction.Complete();
            }
            if (prospects != null && prospects.Count() > 0)
                foreach (var p in prospects)
                {
                    try
                    {
                        BusinessPessoa.deletePessoa(p.cd_pessoa_fisica);
                    }
                    catch
                    {
                        //Execption não tratado pois a exclusão e condicional (se existir ligações com a pessoa, ela não poderá ser excluida).
                    }
                }
            return deletedProspect;
        }

        public ProspectSearchUI editProspect(ProspectSearchUI prospect, int cd_usuario_atendente)
        {
            this.sincronizarContextos(DataAccessProspect.DB());
            ProspectSearchUI retornProspect = new ProspectSearchUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                Prospect newProspect = new Prospect();
                PessoaFisicaSGF pessoaFisicaProspect = new PessoaFisicaSGF();
                bool gerouBaixa = false;

                //Alterar títulos
                Prospect prospectEditado = DataAccessProspect.findById(prospect.cd_prospect, false);
                SGFWebContext dbComp = new SGFWebContext();
                //Se alterar data ou valor de pré matricula, gerar os títulos novamente.
                Prospect prospectOri = DataAccessProspect.getProspectAllData(prospect.cd_prospect, prospect.cd_pessoa_escola.Value);

                Titulo tituloProspect = BusinessFinanceiro.getTitulosByOrigem(prospectOri.cd_prospect, Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Prospect"].ToString()), prospectOri.cd_pessoa_escola.Value).FirstOrDefault();

                List<Titulo> listTitulo = new List<Titulo>();
                //Verificando se altreou data, valor de pré matricula ou plano de contas
                if ((prospect.vl_matricula != prospectOri.vl_matricula_prospect) || (prospect.dt_matricula != prospectOri.dt_matricula_prospect) || (prospect.cd_plano_conta_tit != prospectOri.cd_plano_conta))
                {
                    if (prospect.vl_matricula.HasValue && prospect.vl_matricula.Value <= 0)
                        throw new FinanceiroBusinessException(Messages.msgErroNaoExisteSaldoTitulo, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_SALDO_TITULO, false);

                    //Verificar se já existe baixa para o título gerado
                    BaixaTitulo baixasTitulo = new BaixaTitulo();
                    if (tituloProspect != null && tituloProspect.cd_titulo > 0)
                        baixasTitulo = BusinessFinanceiro.getBaixaTituloByIdTitulo(tituloProspect.cd_titulo, prospect.cd_pessoa_escola.Value).FirstOrDefault();
                    //Se tem baixa e não foi gerada pelo id_gerar_baixa, significa que foi feito baixa manual. Baixa manual não pode alterar
                    if (baixasTitulo != null && baixasTitulo.cd_titulo > 0 && !prospectOri.id_gerar_baixa)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgTituloBaixado, null, FinanceiroBusinessException.TipoErro.ERRO_ALTERAR_PREMATRICULA_BAIXA_MANUAL, false);

                    if (baixasTitulo != null && baixasTitulo.cd_baixa_titulo > 0)
                    {
                        if (!prospectOri.id_gerar_baixa)
                            throw new FinanceiroBusinessException(Utils.Messages.Messages.msgTituloBaixado, null, FinanceiroBusinessException.TipoErro.ERRO_ALTERAR_PREMATRICULA_BAIXA_MANUAL, false);
                        TransacaoFinanceira transacaoFinanc = BusinessFinanceiro.getTransacaoFinanceira(baixasTitulo.cd_tran_finan, prospect.cd_pessoa_escola.Value);
                        if (transacaoFinanc != null && transacaoFinanc.cd_tran_finan > 0)
                            BusinessFinanceiro.deleteTransFinanBaixa(transacaoFinanc);

                    }
                    //Edita o titulo se já existir titulo e existir valor da matricula no prospect 

                    if (tituloProspect != null && tituloProspect.cd_titulo > 0 && prospect.vl_matricula != null)
                    {
                        //Editando título
                        DateTime? dt_liquidacao_titulo = null;
                        if (prospect.gerar_baixa)
                            dt_liquidacao_titulo = prospect.dt_matricula;

                        tituloProspect.vl_titulo = prospect.vl_matricula.Value;
                        tituloProspect.dt_emissao_titulo = prospect.dt_matricula.Value.Date;
                        tituloProspect.dt_vcto_titulo = prospect.dt_matricula.Value.Date;
                        tituloProspect.dh_cadastro_titulo = DateTime.UtcNow;
                        tituloProspect.dt_liquidacao_titulo = dt_liquidacao_titulo;
                        tituloProspect.vl_saldo_titulo = prospect.vl_matricula.Value;
                        tituloProspect.vl_liquidacao_titulo = 0;
                        tituloProspect.cd_plano_conta_tit = prospect.cd_plano_conta_tit;
                        tituloProspect.tituloEdit = true;
                        listTitulo.Add(tituloProspect);
                        List<Titulo> listTituloEditado = BusinessFinanceiro.editTitulos(listTitulo);
                        if (prospect.gerar_baixa)
                        {
                            gerarBaixaPreMatricula(listTituloEditado[0], prospect.cd_local_movto.Value, prospect.cd_tipo_liquidacao.Value, prospect.no_pessoa, cd_usuario_atendente);
                            gerouBaixa = true;
                        }
                    }
                    else
                    {
                        //Não tinha titulo e incluiu
                        if (tituloProspect == null && prospect.vl_matricula != null)
                        {
                            Titulo titulo = null;

                            if(prospect.vl_matricula.HasValue && prospect.vl_matricula.Value > 0)
                                titulo = gerarTituloPreMatricula(prospect, prospectEditado);

                            if (prospect.gerar_baixa)
                            {
                                if (prospect.vl_matricula.HasValue && prospect.vl_matricula.Value <= 0)
                                    throw new FinanceiroBusinessException(Messages.msgErroNaoExisteSaldoTitulo, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_SALDO_TITULO, false);

                                gerarBaixaPreMatricula(titulo, prospect.cd_local_movto.Value, prospect.cd_tipo_liquidacao.Value, prospect.no_pessoa, cd_usuario_atendente);
                                gerouBaixa = true;
                            }
                        }
                        else
                            //Tinha titulo e tirou
                            if (tituloProspect != null && tituloProspect.cd_titulo > 0 && prospectOri.vl_matricula_prospect != null)
                            {
                                if (tituloProspect.id_status_cnab != (int)Titulo.StatusCnabTitulo.INICIAL && tituloProspect.id_status_cnab != (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA)
                                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgNotUpdateTituloEnviadoCNAB, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB, false);
                                BusinessFinanceiro.deletarTitulo(tituloProspect, prospect.cd_pessoa_escola.Value);
                            }
                    }
                }
                //Verificando se gerar baixa, local de movimento ou tipo de liquidação
                if ((prospect.gerar_baixa != prospectOri.id_gerar_baixa) || (prospect.cd_local_movto != prospectOri.cd_local_movimento) || (prospect.cd_tipo_liquidacao != prospectOri.cd_tipo_liquidacao))
                    //Se desmarcou gerar baixa automatica
                    if (prospectOri.id_gerar_baixa && !prospect.gerar_baixa)
                    {
                        //Verificar se já existe baixa para o título gerado
                        BaixaTitulo baixasTitulo = new BaixaTitulo();
                        if (tituloProspect != null && tituloProspect.cd_titulo > 0)
                            baixasTitulo = BusinessFinanceiro.getBaixaTituloByIdTitulo(tituloProspect.cd_titulo, prospect.cd_pessoa_escola.Value).FirstOrDefault();
                        //Se tem baixa e não foi gerada pelo id_gerar_baixa, significa que foi feito baixa manual. Baixa manual não pode alterar
                        if (baixasTitulo != null && baixasTitulo.cd_baixa_titulo > 0)
                        {
                            if (!prospectOri.id_gerar_baixa)
                                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgTituloBaixado, null, FinanceiroBusinessException.TipoErro.ERRO_ALTERAR_PREMATRICULA_BAIXA_MANUAL, false);
                            TransacaoFinanceira transacaoFinanc = BusinessFinanceiro.getTransacaoFinanceira(baixasTitulo.cd_tran_finan, prospect.cd_pessoa_escola.Value);
                            if (transacaoFinanc != null && transacaoFinanc.cd_tran_finan > 0)
                                BusinessFinanceiro.deleteTransFinanBaixa(transacaoFinanc);
                        }
                    }
                    else
                        //Se marcou para gerar automatico
                        if (!prospectOri.id_gerar_baixa && prospect.gerar_baixa && tituloProspect != null && tituloProspect.cd_titulo > 0 && !gerouBaixa)
                        {
                            BaixaTitulo baixasTitulo = new BaixaTitulo();
                            if (tituloProspect != null && tituloProspect.cd_titulo > 0)
                                baixasTitulo = BusinessFinanceiro.getBaixaTituloByIdTitulo(tituloProspect.cd_titulo, prospect.cd_pessoa_escola.Value).FirstOrDefault();
                            if (baixasTitulo != null && baixasTitulo.cd_baixa_titulo > 0)
                                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgTituloBaixado, null, FinanceiroBusinessException.TipoErro.ERRO_ALTERAR_PREMATRICULA_BAIXA_MANUAL, false);
                            gerarBaixaPreMatricula(tituloProspect, prospect.cd_local_movto.Value, prospect.cd_tipo_liquidacao.Value, prospect.no_pessoa, cd_usuario_atendente);
                            gerouBaixa = true;
                        }
                        else
                            if (prospectOri.id_gerar_baixa && ((prospect.cd_local_movto != prospectOri.cd_local_movimento) || (prospect.cd_tipo_liquidacao != prospectOri.cd_tipo_liquidacao)))
                            {
                                TransacaoFinanceira transacaoFinanc = BusinessFinanceiro.getTransacaoBaixaTitulo(tituloProspect.cd_titulo, prospect.cd_pessoa_escola.Value);

                                //altera a baixa
                                if (transacaoFinanc != null && transacaoFinanc.cd_tran_finan > 0)
                                {
                                    transacaoFinanc.cd_local_movto = prospect.cd_local_movto;
                                    transacaoFinanc.cd_tipo_liquidacao = prospect.cd_tipo_liquidacao;
                                    BusinessFinanceiro.editTransacao(transacaoFinanc,false);
                                }
                            }
                
                /////////////
                var existProspectEmailBases = DataAccessProspect.getExistsProspectEmailENome(prospect.email, (int)prospect.cd_pessoa_escola, prospect.no_pessoa);
                if (existProspectEmailBases != null && existProspectEmailBases.cd_prospect != prospect.cd_prospect)
                    throw new PessoaBusinessException(string.Format(Utils.Messages.Messages.msgExistsEmailProspect, existProspectEmailBases.no_pessoa), null, FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_EMAILJAEXISTENTE, false);
                else
                {
                    //var existAlunoEmailBase = BusinessAluno.verificarAlunoExistEmail(prospect.email, (int)prospect.cd_pessoa_escola, prospect.cd_prospect);
                    //if (existAlunoEmailBase != null && existAlunoEmailBase.cd_aluno > 0)
                    //    throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgExistsEmailAluno, existAlunoEmailBase.no_pessoa), null, AlunoBusinessException.TipoErro.ERRO_EMAIL_JA_EXITE, false);
                    //else
                    //{
                    //    var existePessoaFisicaEmailBase = DataAccessProspect.verificarPessoaFisicaEmail(prospect.email);
                    //    if (existePessoaFisicaEmailBase != null && existePessoaFisicaEmailBase.cd_pessoa > 0)
                    //    {
                    //        newProspect.copy(prospect);
                    //        newProspect.cd_prospect = 0;
                    //        newProspect.cd_usuario = prospect.cd_usuario;
                    //        newProspect.cd_pessoa_fisica = existePessoaFisicaEmailBase.cd_pessoa;
                    //        DataAccessProspect.add(newProspect, false);
                    //    }
                    //    else
                    //    {
                            pessoaFisicaProspect = new PessoaFisicaSGF
                            {
                                cd_pessoa = prospect.cd_pessoa_fisica,
                                no_pessoa = prospect.no_pessoa,
                                //dt_cadastramento = Utils.Utils.truncarMilissegundo(prospect.dt_cadastramento.ToUniversalTime().Date),
                                dt_cadastramento = prospect.dt_cadastramento,
                                nm_sexo = prospect.nm_sexo,
                                EnderecoPrincipal = prospect.endereco,
                                dt_nascimento = prospect.dt_nascimento_prospect
                            };
                            BusinessPessoa.editPessoaFisica(pessoaFisicaProspect);
                            newProspect = DataAccessProspect.findById(prospect.cd_prospect, false);
                            newProspect.cd_midia = prospect.cd_midia;
                            newProspect.id_periodo = prospect.id_periodo;
                            newProspect.cd_pessoa_escola = prospect.cd_pessoa_escola;
                            newProspect.cd_pessoa_fisica = prospect.cd_pessoa_fisica;
                            newProspect.cd_pessoa_fisica = prospect.cd_pessoa_fisica;
                            newProspect.id_dia_semana = prospect.id_dia_semana;
                            newProspect.id_prospect_ativo = prospect.id_prospect_ativo;
                            newProspect.no_escola = prospect.no_escola;
                            newProspect.id_faixa_etaria = prospect.id_faixa_etaria;
                            newProspect.cd_motivo_inativo = prospect.cd_motivo_inativo;
                            int? cdLocalBaixa = null;
                            if (prospect.cd_local_movto > 0)
                                cdLocalBaixa = prospect.cd_local_movto;

                            int? cdTipoLiquidacao = null;
                            if (prospect.cd_tipo_liquidacao > 0)
                                cdTipoLiquidacao = prospect.cd_tipo_liquidacao;

                            newProspect.dt_matricula_prospect = prospect.dt_matricula.HasValue ? prospect.dt_matricula.Value.Date : prospect.dt_matricula;
                            newProspect.vl_matricula_prospect = prospect.vl_matricula;
                            newProspect.cd_plano_conta = prospect.cd_plano_conta_tit > 0 ? prospect.cd_plano_conta_tit : null;
                            newProspect.id_gerar_baixa = prospect.gerar_baixa;
                            newProspect.cd_local_movimento = cdLocalBaixa;
                            newProspect.cd_tipo_liquidacao = cdTipoLiquidacao;
                            DataAccessProspect.saveChanges(false);
                        //}
                    //}
                }

                // telefone
                TelefoneSGF telefoneExists = new TelefoneSGF();
                BusinessPessoa.sincronizaContexto(BusinessPessoa.DBPessoa());
                telefoneExists = BusinessPessoa.FindTypeTelefonePrincipal(prospect.cd_pessoa_fisica, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE);
                BusinessPessoa.addEditTipoContato(prospect.telefone, prospect.cd_pessoa_fisica, EDIT, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE, null, telefoneExists);
                
                //Verifica se já existe pessoa cadastrada para a escola com o mesmo telefone/celular:
                ProspectSearchUI prospectWithTelfone = DataAccessProspect.verificaExistenciaProspect(prospect.telefone, prospect.celular, prospect.cd_pessoa_escola.Value, prospect.cd_prospect, prospect.no_pessoa);
                if (prospectWithTelfone != null)
                    throw new PessoaBusinessException(string.Format(Utils.Messages.Messages.msgPessoaExistenteProspect, prospectWithTelfone.no_pessoa), null, PessoaBusinessException.TipoErro.ERRO_TELEFONEJAEXISTENTE, false);

                //email
                TelefoneSGF emailExists = new TelefoneSGF();
                emailExists = BusinessPessoa.FindTypeTelefonePrincipal(prospect.cd_pessoa_fisica, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL);
                BusinessPessoa.addEditTipoContato(prospect.email, prospect.cd_pessoa_fisica, EDIT, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL, null, emailExists);
                
                //celular
                TelefoneSGF celularExists = new TelefoneSGF();
                celularExists = BusinessPessoa.FindTypeTelefonePrincipal(prospect.cd_pessoa_fisica, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR);
                BusinessPessoa.addEditTipoContato(prospect.celular, prospect.cd_pessoa_fisica, EDIT, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR, prospect.cd_operadora, celularExists);
                
                setProspectMotivoNaoMatricula(prospect.listaMotivos, newProspect.cd_prospect, (int)prospect.cd_pessoa_escola);
                setProspectFollowUp(prospect.listaFollowUp, newProspect.cd_prospect, cd_usuario_atendente, (int) prospect.cd_pessoa_escola);

                removerProspectProduto(prospect);
                removerProspectPeriodo(prospect);
                removerProspectDia(prospect);
                perstirProdutoProspect(prospect, newProspect.cd_prospect);
                perstirPeriodoProspect(prospect, newProspect.cd_prospect);
                persisitirDiaProspect(prospect, newProspect.cd_prospect);

                retornProspect = ProspectSearchUI.fromProspectUI(prospect, newProspect.cd_prospect, pessoaFisicaProspect.dt_cadastramento);
                retornProspect.listaFollowUp = prospect.listaFollowUp;
                transaction.Complete();
            }
            return retornProspect;
        }

        private void removerProspectProduto(ProspectSearchUI prospect)
        {
            List<ProspectProduto> deleteProdutos = DataAccessProspectProduto.getProdutoProspect(prospect.cd_prospect);
            if (deleteProdutos != null)
                for (int i = deleteProdutos.Count() - 1; i >= 0; i--)
                    DataAccessProspectProduto.delete(deleteProdutos[i], false);
        }

        private void removerProspectPeriodo(ProspectSearchUI prospect)
        {
            List<ProspectPeriodo> deletePeriodos = DataAccessProspectPeriodo.getPeriodoProspect(prospect.cd_prospect);
            if (deletePeriodos != null)
                for (int i = deletePeriodos.Count() - 1; i >= 0; i--)
                    DataAccessProspectPeriodo.delete(deletePeriodos[i], false);
        }

        private void removerProspectDia(ProspectSearchUI prospect)
        {
            List<ProspectDia> deleteDias = DataAccessProspectDia.searchProspectDia(prospect.cd_prospect);
            if (deleteDias != null)
                for (int i = deleteDias.Count() - 1; i >= 0; i--)
                    DataAccessProspectDia.delete(deleteDias[i], false);
        }

        public IEnumerable<MotivoNaoMatricula> getProspectMotivoNaoMatricula(int cdProspect, int cd_escola)
        {
            return DataAccessMotivoNaoMatricula.getProspectMotivoNaoMatricula(cdProspect, cd_escola);
        }
        public ProspectMotivoNaoMatricula getProspectMotivoNaoMatriculaEsc(int cdProspect, int cd_escola, int cd_motivo)
        {
            return DataAccessProspectMotivoNaoMatricula.getProspectMotivoNaoMatriculaEsc(cdProspect, cd_escola, cd_motivo);
        }
        public ProspectSearchUI getExistsProspectEmail(string email, int cdEscola, int cdProspect)
        {
            return DataAccessProspect.getExistsProspectEmail(email, cdEscola, cdProspect);
        }

        public Prospect getProspectForEdit(int cdProspect, int cdEscola, string email)
        {
            return DataAccessProspect.getProspectForEdit(cdProspect, cdEscola, email);
        }

        public Prospect getProspectPorEmail(int cdEscola, string email)
        {
            Prospect retorno = new Prospect();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessProspect.getProspectPorEmail(cdEscola, email);
                transaction.Complete();
            }
            return retorno;
        }

        public bool deleteProspect(Prospect prospect)
        {
            return DataAccessProspect.delete(prospect, false);
        }

        public Prospect findProspectById(int cdProspect)
        {
            return DataAccessProspect.findById(cdProspect, false);
        }

        public Prospect getProspectAllData(int cdProspect, int cdEscola)
        {
            return DataAccessProspect.getProspectAllData(cdProspect, cdEscola);
        }

        public Prospect getProspectForAluno(int cdProspect, int cdEscola)
        {
            return DataAccessProspect.getProspectForAluno(cdProspect, cdEscola);
        }

        public IEnumerable<ReportProspect> getProspectAtendido(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos, int cd_faixa_etaria)
        {
            IEnumerable<ReportProspect> retorno = new List<ReportProspect>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessProspect.getProspectAtendido(cd_escola, cdMotivoNaoMatricula, cFuncionario, cdProduto, pDtaI, pDtaF, cd_midia, periodos, cd_faixa_etaria);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<ReportProspect> getProspectAtendidoMatricula(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos)
        {
            IEnumerable<ReportProspect> retorno = new List<ReportProspect>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessProspect.getProspectAtendidoMatricula(cd_escola, cdMotivoNaoMatricula, cFuncionario, cdProduto, pDtaI, pDtaF, cd_midia, periodos);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<ReportProspect> getComparativoProspectAtendido(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos)
        {
            IEnumerable<ReportProspect> retorno = new List<ReportProspect>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessProspect.getComparativoProspectAtendido(cd_escola, cdMotivoNaoMatricula, cFuncionario, cdProduto, pDtaI, pDtaF, cd_midia, periodos);
                transaction.Complete();
            }
            return retorno;
        }


        public bool existeProspectNaoConsultado(int cd_escola)
        {
            bool retorno = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessProspect.existeProspectNaoConsultado(cd_escola);
                transaction.Complete();
            }
            return retorno;
        }

       

        public void setProspectsConsultado(int cd_escola)
        {
            IEnumerable<ProspectSearchUI> prospectNaoLidos = new List<ProspectSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                #region pesquisa prospects não lidos
                    prospectNaoLidos = getProspectsNaoLidos(cd_escola, prospectNaoLidos);              
                #endregion

                #region seta lido para cada prospect
                    setaLidoProspects(prospectNaoLidos);
                #endregion 
                transaction.Complete();
            }
            
        }

        private void setaLidoProspects(IEnumerable<ProspectSearchUI> prospectNaoLidos)
        {
            foreach (var item in prospectNaoLidos)
            {
                Prospect prospectEditado = DataAccessProspect.findById(item.cd_prospect, false);
                prospectEditado.id_consultado = Convert.ToBoolean((int)Prospect.TipoStatusConsultado.CONSULTADO);
                DataAccessProspect.saveChanges(false);
            }
        }

        private IEnumerable<ProspectSearchUI> getProspectsNaoLidos(int cd_escola, IEnumerable<ProspectSearchUI> prospectNaoLidos)
        {
            SearchParameters parametrosSearch = new SearchParameters(0, int.MaxValue, int.MaxValue);

            parametrosSearch.sort = "no_pessoa";
            parametrosSearch.sortOrder = (int)SortDirection.Ascending;

            prospectNaoLidos = DataAccessProspect.GetProspectSearch(parametrosSearch, "", false, "", cd_escola, null, null, true, false, 3).ToList();
            return prospectNaoLidos;
        }
        #endregion

        #region Follow Up

        public bool existeFollowNaoResolvido(int cd_usuario, int cd_escola, bool usuario_login_master)
        {
            bool retorno = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessFollowUp.existeFollowNaoResolvido(cd_usuario, cd_escola, usuario_login_master);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<FollowUpSearchUI> getFollowUpSearch(SearchParameters parametros, int cdEscola, byte id_tipo_follow, int cd_usuario_org, int cd_usuario_destino, int cd_prospect_aluno,
                                                       int cd_acao, int resolvido, int lido, bool data, bool proximo_contato, DateTime? dt_inicial, DateTime? dt_final, bool id_usuario_adm,
                                                       int cd_usuario_logado, int cd_aluno, bool usuario_login_master)
        {
            IEnumerable<FollowUpSearchUI> retorno = new List<FollowUpSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dt_follow_up";
                parametros.sort = parametros.sort.Replace("desc_tipo", "id_tipo_follow");
                parametros.sort = parametros.sort.Replace("dta_data", "dt_follow_up");
                parametros.sort = parametros.sort.Replace("dta_proximo_contato", "dt_proximo_contato");
                parametros.sort = parametros.sort.Replace("lido", "id_follow_lido");
                parametros.sort = parametros.sort.Replace("resolvido", "id_follow_resolvido");
                retorno = DataAccessFollowUp.getFollowUpSearch(parametros,cdEscola,id_tipo_follow,cd_usuario_org, cd_usuario_destino,cd_prospect_aluno,cd_acao,resolvido,lido,data,
                                                            proximo_contato, dt_inicial, dt_final, id_usuario_adm, cd_usuario_logado, cd_aluno, usuario_login_master);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<FollowUp> getFollowUpByAluno(int cdAluno, int cd_escola)
        {
            List<FollowUp> retorno = DataAccessFollowUp.getFollowUpByAluno(cdAluno, cd_escola).OrderByDescending(f => f.dt_follow_up).ToList();

            int i = retorno.Count;
            //Ordena por data descrescente:
            foreach(FollowUp followUp in retorno) {
                followUp.nroOrdem = i;
                i -= 1;
            }

            return retorno;
        }

        public IEnumerable<FollowUp> getFollowUpProspect(int cd_prospect, int cd_escola)
        {
            List<FollowUp> retorno = DataAccessFollowUp.getFollowUpProspect(cd_prospect, cd_escola).OrderByDescending(f => f.dt_follow_up).ToList();

            int i = retorno.Count;
            //Ordena por data descrescente:
            foreach(FollowUp followUp in retorno) {
                followUp.nroOrdem = i;
                i -= 1;
            }

            return retorno;
        }

        public bool deleteFollowUp(FollowUp followUp)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = DataAccessFollowUp.delete(followUp, false);
                transaction.Complete();
                return deleted;
            }
        }

        public FollowUp getFollowEditView(int cd_follow_up, int cd_escola, int id_tipo_follow)
        {
            return DataAccessFollowUp.getFollowEditView(cd_follow_up, cd_escola,id_tipo_follow);
        }

        public List<FollowUpRptUI> GetRtpFollowUp(byte id_tipo_follow, int cd_usuario_org, string no_usuario_org, int resolvido, int lido, DateTime? dtaIni, DateTime? dtaFinal, int cd_escola)
        {
            return DataAccessFollowUp.GetRtpFollowUp(id_tipo_follow, cd_usuario_org, no_usuario_org, resolvido, lido, dtaIni, dtaFinal, cd_escola);
        }

        public FollowUpSearchUI addFollowUp(FollowUp followUp)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //if (followUp.dc_assunto.Count() > 3000) // Limite aumentado devido formatação html do campo textarea editor.
                //    throw new SecretariaBusinessException(Messages.msgErroLimiteAssuntoFollowUp, null, SecretariaBusinessException.TipoErro.ERRO_FOLLOW_LIMITE_DC_ASSUNTO, false);

                bool userMasterGeral = BusinessEmpresa.VerificarMasterGeral(followUp.cd_usuario);
                followUp = FollowUp.setarCamposDefaultPorTipo(followUp, userMasterGeral);
                DataAccessFollowUp.add(followUp, false);

                //Se for resposta, deve-se marcar o follow-up original como lido e respondido:
                if(followUp.cd_follow_up_origem.HasValue && followUp.cd_follow_up_pai.HasValue) {
                    FollowUp followUpOriginal = DataAccessFollowUp.findById(followUp.cd_follow_up_pai.Value, false);

                    followUpOriginal.id_follow_lido = true;
                    followUpOriginal.id_follow_resolvido = true;
                    followUpOriginal.cd_follow_up_origem = followUp.cd_follow_up_origem;
                    DataAccessFollowUp.saveChanges(false);
                }

                if (followUp.escolas != null)
                    crudEscolasFollowUp(followUp.escolas.ToList(), followUp.cd_follow_up);
                transaction.Complete();
            }
            return DataAccessFollowUp.getFollowUpGrid(followUp.cd_escola, followUp.cd_follow_up);
        }

        public FollowUpSearchUI editFollowUp(FollowUp followUp, int cd_usuario, bool isMaster)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                followUp = FollowUp.setarCamposDefaultPorTipo(followUp,false);
                FollowUp followUpContext = DataAccessFollowUp.findById(followUp.cd_follow_up, false);

                //if (followUp.dc_assunto.Count() > 3000) // Limite aumentado devido formatação html do campo textarea editor.
                //throw new SecretariaBusinessException(Messages.msgErroLimiteAssuntoFollowUp, null, SecretariaBusinessException.TipoErro.ERRO_FOLLOW_LIMITE_DC_ASSUNTO, false);

                /*--Solicitação -> 307427--*/
                if (!isMaster)
                {
                    if (followUpContext.cd_usuario != cd_usuario)
                        throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroAlterarFollowUpDeOutroUsuario, null,
                            SecretariaBusinessException.TipoErro.ERRO_FOLLOW_UP_DE_OUTRO_USUARIO, false);
                }
                
                followUpContext = FollowUp.changeValuesFollowUp(followUpContext, followUp);
                DataAccessFollowUp.saveChanges(false);
                if (followUp.escolas != null)
                    crudEscolasFollowUp(followUp.escolas.ToList(), followUp.cd_follow_up);
                transaction.Complete();
            }
            return DataAccessFollowUp.getFollowUpGrid(followUp.cd_escola, followUp.cd_follow_up);
        }

        public bool deleteFollowUps(List<int> codigosFollowUps, int cd_usuario_origem)
        {
            bool retorno = false;
            SGFWebContext cdb = new SGFWebContext();

            if (codigosFollowUps != null && codigosFollowUps.Count() > 0)
            {
                if (DataAccessFollowUp.verificaExisteFollowUPOutroUsuario(codigosFollowUps, cd_usuario_origem))
                    throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroDeleteFollowUpOutroUsuario, null,
                                                                          SecretariaBusinessException.TipoErro.ERRO_FOLLOW_UP_DE_OUTRO_USUARIO, false);
                List<FollowUp> followUpsContext = DataAccessFollowUp.getFollowUps(codigosFollowUps, cd_usuario_origem).ToList();
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    foreach (var f in followUpsContext)
                    {
                        if(f.id_follow_lido)
                            throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroNaoPermitidoExcluirFollowUpOutroUsuario, null,
                                                                           SecretariaBusinessException.TipoErro.ERRO_FOLLOW_UP_LIDO, false);
                        if (f.id_tipo_follow == (int)FollowUp.TipoFollowUp.INTERNO && DataAccessFollowUp.existeRespostaFollowUp(f.cd_follow_up))
                            throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroNaoExcluirFollowUpComResposta, null,
                                                                           SecretariaBusinessException.TipoErro.ERRO_FOLLOW_UP_USER_ORIGEM_DIFERENTE, false);
                        //crudItensMovimento(new List<ItemMovimento>(), m, false);
                        //crudTitulosMovimento(new List<Titulo>(), m);
                        retorno = DataAccessFollowUp.delete(f, false);
                    }
                    transaction.Complete();
                }
            }
            return retorno;
        }

        public IEnumerable<FollowUpEscola> getFollowUpEscola(int cd_follow_up)
        {
            return DataAccessFollowUpEscola.getFollowUpEscola(cd_follow_up);
        }

        public void crudEscolasFollowUp(List<FollowUpEscola> escolasView, int cd_follow_up)
        {
            this.sincronizarContextos(DataAccessFollowUpEscola.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                Horario horario = new Horario();
                List<FollowUpEscola> followUpEscolaContext = DataAccessFollowUpEscola.getFollowUpEscola(cd_follow_up).ToList();
                IEnumerable<FollowUpEscola> followUpEscolaComCodigo = from hpts in escolasView
                                                                      where hpts.cd_follow_escola != 0
                                                                      select hpts;
                IEnumerable<FollowUpEscola> followUpEscolaDeleted = followUpEscolaContext.Where(tc => !followUpEscolaComCodigo.Any(tv => tc.cd_follow_escola == tv.cd_follow_escola));

                if (followUpEscolaDeleted != null)
                    foreach (var item in followUpEscolaDeleted)
                        if (item != null)
                            DataAccessFollowUpEscola.deleteContext(item, false);

                foreach (var item in escolasView)
                {
                    FollowUpEscola retorno = null;
                    if (item.cd_follow_escola == 0)
                    {
                        item.cd_follow_up = cd_follow_up;
                        retorno = DataAccessFollowUpEscola.addContext(item, false);
                    }
                }
                DataAccessFollowUpEscola.saveChanges(false);
                transaction.Complete();
            }
        }

        public bool marcarFollowUpComoLido(FollowUp followUp, int cd_usuario_login)
        {
            this.sincronizarContextos(DataAccessFollowUpUsuario.DB());
            FollowUp followUpContext = DataAccessFollowUp.findById(followUp.cd_follow_up, false);
            if (followUpContext != null)
            {
                switch (followUpContext.id_tipo_follow)
                {
                    case (int)FollowUp.TipoFollowUp.INTERNO:
                    case (int)FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL:
                        if (!followUpContext.cd_usuario_destino.HasValue)
                        {
                            if (followUp.id_follow_lido)
                            {
                                if (!DataAccessFollowUpUsuario.usuarioLeuFollowUp(followUpContext.cd_follow_up, cd_usuario_login))
                                    DataAccessFollowUpUsuario.addContext(new FollowUpUsuario
                                    {
                                        cd_usuario = cd_usuario_login,
                                        cd_follow_up = followUpContext.cd_follow_up,
                                        dt_follow_usuario = DateTime.Now
                                    }, false);
                            }
                            else
                            {
                                FollowUpUsuario followUpUsuario = DataAccessFollowUpUsuario.getFollowUpUsuario(followUpContext.cd_follow_up, cd_usuario_login);
                                if (followUpUsuario != null)
                                    DataAccessFollowUpUsuario.deleteContext(followUpUsuario, false);
                            }
                        }
                        else
                            followUpContext.id_follow_lido = followUp.id_follow_lido;
                        break;
                }
                DataAccessFollowUp.saveChanges(false);
                DataAccessFollowUpUsuario.saveChanges(false);
            }
            return true;
        }

        #endregion

        #region Nome Contrato

        public IEnumerable<NomeContrato> getSearchNoContrato(SearchParameters parametros, string desc, string layout, bool inicio, bool? status, int cdEscola, int cdUsuario)
        {
            IEnumerable<NomeContrato> retorno = new List<NomeContrato>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_contrato";
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(cdUsuario);
                parametros.sort = parametros.sort.Replace("tipoNomeContrato", "cd_pessoa_escola");

                retorno = DataAccessNomeContrato.getSearchNoContrato(parametros, desc, layout, inicio, status, cdEscola, masterGeral);
                transaction.Complete();
            }
            return retorno;
        }

        public NomeContrato addNomeContrato(NomeContrato nomeCont, string pathContratosEscola, int cdUsuario)
        {
            string documentoContratoTemp = "";
            bool masterGeral = BusinessEmpresa.VerificarMasterGeral(cdUsuario);
            try
            {
                HashSet<char> invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());
                HashSet<char> invalidPathChars = new HashSet<char>(Path.GetInvalidPathChars());
                if (masterGeral)
                {
                    nomeCont.cd_pessoa_escola = null;
                    if (nomeCont.cd_nome_contrato_pai != null && nomeCont.cd_nome_contrato_pai > 0)
                        throw new SecretariaBusinessException(Utils.Messages.Messages.msgUsuaioNotEspecializarNoCont, null, SecretariaBusinessException.TipoErro.ERRO_USUARIO_NAO_PODE_ESPECIALIZAR_LAYOUT_CONTRATO, false);
                }
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    DataAccessNomeContrato.add(nomeCont, false);
                    if (!String.IsNullOrEmpty(nomeCont.relatorioTemporario))
                    {
                        string documentoContrato = "";
                        if (masterGeral)
                        {
                            documentoContrato = pathContratosEscola + "/" + nomeCont.no_relatorio;
                            documentoContratoTemp = pathContratosEscola + "/" + nomeCont.relatorioTemporario;
                        }
                        else
                        {
                            documentoContrato = pathContratosEscola + "/" + nomeCont.cd_pessoa_escola + "/" + nomeCont.no_relatorio;
                            documentoContratoTemp = pathContratosEscola + "/" + nomeCont.cd_pessoa_escola + "/" + nomeCont.relatorioTemporario;
                        }
                        if (nomeCont.no_relatorio.Any(c => invalidFileNameChars.Contains(c)))
                            throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosNomeArquivo + nomeCont.no_relatorio, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
                        if (pathContratosEscola.Any(c => invalidPathChars.Contains(c)))
                            throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosPathArquivo + nomeCont.no_relatorio, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
                        //string pathContratosEscola = caminho_relatorios + "/Contratos/" + cdEscola;
                        if (System.IO.File.Exists(documentoContrato))
                            throw new SecretariaBusinessException(Utils.Messages.Messages.msgNoLayoutJaCadastrado, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
                        //System.IO.File.Delete(documentoContrato);
                        System.IO.File.Move(documentoContratoTemp, documentoContrato);
                        if (System.IO.File.Exists(documentoContratoTemp))
                            System.IO.File.Delete(documentoContratoTemp);
                    }
                    transaction.Complete();
                }
                return nomeCont;
            }
            catch (Exception exe)
            {
                if (masterGeral)
                    documentoContratoTemp = pathContratosEscola + "/" + nomeCont.relatorioTemporario;
                else
                    documentoContratoTemp = pathContratosEscola + "/" + nomeCont.cd_pessoa_escola + "/" + nomeCont.relatorioTemporario;

                if (System.IO.File.Exists(documentoContratoTemp))
                    System.IO.File.Delete(documentoContratoTemp);

                throw exe;
            }

        }

        public NomeContrato editNomeContrato(NomeContrato nomeCont, string pathContratosEscola, int cdUsuario)
        {
            string documentoContratoTemp = "";
            bool masterGeral = BusinessEmpresa.VerificarMasterGeral(cdUsuario);
            try
            {
                HashSet<char> invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());
                HashSet<char> invalidPathChars = new HashSet<char>(Path.GetInvalidPathChars());
                if (masterGeral)
                    nomeCont.cd_pessoa_escola = null;
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    NomeContrato noContCopy = new NomeContrato();
                    NomeContrato noContContext = new NomeContrato();
                    noContContext = DataAccessNomeContrato.getNomeContratoById(nomeCont.cd_pessoa_escola, nomeCont.cd_nome_contrato);
                    noContCopy.copy(noContContext);
                    if (noContContext.cd_pessoa_escola == null && !masterGeral)
                        throw new SecretariaBusinessException(Utils.Messages.Messages.msgSemPemissaoDeletarNoCont, null, SecretariaBusinessException.TipoErro.ERRO_SEM_PERMISAO_DELETAR_NOME_CONTRATO, false);
                    noContContext = NomeContrato.changeValuesNomeContrato(noContContext, nomeCont);
                    DataAccessNomeContrato.saveChanges(false);
                    if (!String.IsNullOrEmpty(nomeCont.relatorioTemporario))
                    {
                        string documentoContrato = "";
                        string pathDocAntigo = "";
                        if (masterGeral)
                        {
                            pathDocAntigo = pathContratosEscola + "/" + noContCopy.no_relatorio;
                            documentoContrato = pathContratosEscola + "/" + nomeCont.no_relatorio;
                            documentoContratoTemp = pathContratosEscola + "/" + nomeCont.relatorioTemporario;
                        }
                        else
                        {
                            pathDocAntigo = pathContratosEscola + "/" + nomeCont.cd_pessoa_escola + "/" + noContCopy.no_relatorio;
                            documentoContrato = pathContratosEscola + "/" + nomeCont.cd_pessoa_escola + "/" + nomeCont.no_relatorio;
                            documentoContratoTemp = pathContratosEscola + "/" + nomeCont.cd_pessoa_escola + "/" + nomeCont.relatorioTemporario;
                        }
                        //string pathContratosEscola = caminho_relatorios + "/Contratos/" + cdEscola;
                        if (nomeCont.no_relatorio.Any(c => invalidFileNameChars.Contains(c)))
                            throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosNomeArquivo + nomeCont.no_relatorio, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
                        if (pathContratosEscola.Any(c => invalidPathChars.Contains(c)))
                            throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosPathArquivo + nomeCont.no_relatorio, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
                        //string pathContratosEscola = caminho_relatorios + "/Contratos/" + cdEscola;
                        if (System.IO.File.Exists(documentoContrato))
                        {
                            if (noContCopy.no_relatorio != noContContext.no_relatorio)
                                throw new SecretariaBusinessException(Utils.Messages.Messages.msgNoLayoutJaCadastrado, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
                            else
                                if (System.IO.File.Exists(pathDocAntigo))
                                    System.IO.File.Delete(pathDocAntigo);
                        }
                        else
                            if (System.IO.File.Exists(pathDocAntigo))
                                System.IO.File.Delete(documentoContrato);

                        System.IO.File.Move(documentoContratoTemp, documentoContrato);
                        if (System.IO.File.Exists(documentoContratoTemp))
                            System.IO.File.Delete(documentoContratoTemp);
                    }
                    transaction.Complete();
                    return nomeCont;
                }

            }
            catch (Exception exe)
            {
                if (masterGeral)
                    documentoContratoTemp = pathContratosEscola + "/" + nomeCont.relatorioTemporario;
                else
                    documentoContratoTemp = pathContratosEscola + "/" + nomeCont.cd_pessoa_escola + "/" + nomeCont.relatorioTemporario;

                if (System.IO.File.Exists(documentoContratoTemp))
                    System.IO.File.Delete(documentoContratoTemp);
                throw exe;
            }
        }

        public bool deleteNomesContratos(int[] cdNomesContratos, string pathContratosEscola, int cdEscola, int cdUsuario)
        {
            bool retorno = false;
            bool masterGeral = BusinessEmpresa.VerificarMasterGeral(cdUsuario);
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (cdNomesContratos != null && cdNomesContratos.Count() > 0)
                {

                    List<NomeContrato> nomesContratosContext = DataAccessNomeContrato.getNomesContratosByListaCodigos(cdNomesContratos, cdEscola, masterGeral).ToList();
                    foreach (var noCont in nomesContratosContext)
                    {
                        if (noCont.cd_pessoa_escola == null && !masterGeral)
                            throw new SecretariaBusinessException(Utils.Messages.Messages.msgSemPemissaoDeletarNoCont, null, SecretariaBusinessException.TipoErro.ERRO_SEM_PERMISAO_DELETAR_NOME_CONTRATO, false);
                        retorno = DataAccessNomeContrato.delete(noCont, false);
                        if (!String.IsNullOrEmpty(noCont.no_relatorio))
                        {
                            string documentoContrato = "";
                            if (masterGeral)
                                documentoContrato = pathContratosEscola + "/" + noCont.no_relatorio;
                            else
                                if (cdEscola > 0)
                                    documentoContrato = pathContratosEscola + "/" + cdEscola + "/" + noCont.no_relatorio;
                            //string pathContratosEscola = caminho_relatorios + "/Contratos/" + cdEscola;
                            if (System.IO.File.Exists(documentoContrato))
                                System.IO.File.Delete(documentoContrato);

                        }
                    }
                }
                transaction.Complete();
            }
            return retorno;
        }

        public NomeContrato getNomeContratoById(int? cdEscola, int cdNomeContrato)
        {
            return DataAccessNomeContrato.getNomeContratoById(cdEscola, cdNomeContrato);
        }

        public string getNomeContratoDigitalizadoByCdContrato(int cdEscola, int cdContrato, string nomeArquivo)
        {
            return DataAccessNomeContrato.getNomeContratoDigitalizadoByCdContrato(cdEscola,  cdContrato, nomeArquivo);
        }

        public string getNomeContratoDigitalizadoByEscolaAndCdContrato(int cdEscola, int cdContrato)
        {
            return DataAccessNomeContrato.getNomeContratoDigitalizadoByEscolaAndCdContrato(cdEscola, cdContrato);
        }

        public IEnumerable<NomeContrato> getNomeContratoMat(int? cdEscola)
        {
            return DataAccessNomeContrato.getNomeContratoMat(cdEscola);
        }

        public IEnumerable<NomeContrato> getNomesContrato(NomeContratoDataAccess.TipoConsultaNomeContratoEnum hasDependente, int? cd_nome_contrato, int? cd_escola)
        {
            IEnumerable<NomeContrato> retorno = new List<NomeContrato>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessNomeContrato.getNomesContrato(hasDependente, cd_nome_contrato, cd_escola).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public NomeContrato getNomeContratoAditamentoMatricula(int cd_contrato, int cd_escola)
        {
            NomeContrato retorno = new NomeContrato();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessNomeContrato.getNomeContratoAditamentoMatricula(cd_contrato, cd_escola);
                transaction.Complete();
            }
            return retorno;
        }

        #endregion

        #region Histórico Aluno

        public IEnumerable<HistoricoAluno> returnHistoricoSitacaoAlunoTurma(int cd_turma, int cd_pessoa_escola)
        {
            return DataAccessHistoricoAluno.returnHistoricoSitacaoAlunoTurma(cd_turma, cd_pessoa_escola);
        }

        public HistoricoAluno postHistoricoAluno(HistoricoAluno historico)
        {
            return DataAccessHistoricoAluno.add(historico, false);
        }

        public HistoricoAluno editHistoricoAluno(HistoricoAluno historico)
        {
            return DataAccessHistoricoAluno.edit(historico, false);
        }

        public int saveHistoricoAluno()
        {
            return DataAccessHistoricoAluno.saveChanges(false);
        }

        public bool deleteHistoricoAluno(HistoricoAluno historico)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = DataAccessHistoricoAluno.delete(historico, false);
                transaction.Complete();
                return deleted;

            }
        }

        public int getUltimoHistoricoAluno(int cd_pessoa_escola, int cd_aluno, int cd_produto)
        {
            return DataAccessHistoricoAluno.GetUltimoHistoricoAluno(cd_pessoa_escola, cd_aluno, cd_produto);
        }

        public HistoricoAluno getUltimoHistoricoAlunoPorCodTurma(int cd_aluno, int cd_escola, int cd_turma)
        {
            return DataAccessHistoricoAluno.getUltimoHistoricoAlunoPorCodTurma(cd_aluno, cd_escola, cd_turma);
        }

        public HistoricoAluno GetHistoricoAlunoMovido(int cdEscola, int cdAluno, int cdTurma, int cdContrato)
        {
            return DataAccessHistoricoAluno.GetHistoricoAlunoMovido(cdEscola, cdAluno, cdTurma, cdContrato);
        }

        public void addHistoricoAluno(HistoricoAluno historicoAluno)
        {
            DataAccessHistoricoAluno.add(historicoAluno, false);
        }

        public int retunMaxSequenciaHistoricoAluno(int cd_turma, int cd_pessoa_escola, int cd_aluno)
        {
            return DataAccessHistoricoAluno.retunMaxSequenciaHistoricoAluno(cd_turma, cd_pessoa_escola, cd_aluno);
        }

        public HistoricoAluno GetHistoricoAlunoById(int cdEscola, int cdAluno, int cdTurma, int cdContrato)
        {
            return DataAccessHistoricoAluno.GetHistoricoAlunoById(cdEscola, cdAluno, cdTurma, cdContrato);
        }

        public HistoricoAluno getHistoricoAlunoByMatricula(int cdEscola, int cdAluno, int cdTurma, int cdContrato)
        {
            return DataAccessHistoricoAluno.getHistoricoAlunoByMatricula(cdEscola, cdAluno, cdTurma, cdContrato);
        }

        public IEnumerable<HistoricoAluno> GetHistoricosAlunoById(int cdEscola, int cdAluno, int cdTurma, int cdContrato)
        {
            return DataAccessHistoricoAluno.GetHistoricosAlunoById(cdEscola, cdAluno, cdTurma, cdContrato);
        }

        public HistoricoAluno GetHistoricoAlunoPrimeiraAula(int cdEscola, int cdAluno, int cdTurma, int cdContrato, DateTime dataDiario)
        {
            return DataAccessHistoricoAluno.GetHistoricoAlunoPrimeiraAula(cdEscola, cdAluno, cdTurma, cdContrato, dataDiario);
        }
        public HistoricoAluno getSituacaoAlunoCancelEncerramento(int cd_aluno, int cd_turma, DateTime dt_historico)
        {
            return DataAccessHistoricoAluno.getSituacaoAlunoCancelEncerramento(cd_aluno, cd_turma, dt_historico);
        }


        public IEnumerable<FollowUp> getFollowAluno(SearchParameters parametros, int cd_aluno, int cd_escola)
        {
            IEnumerable<FollowUp> retorno = new List<FollowUp>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dt_follow_up";
                parametros.sort = parametros.sort.Replace("dta_follow_up", "dt_follow_up");
                parametros.sort = parametros.sort.Replace("dta_proximo_contato", "dt_proximo_contato");
            
                retorno = DataAccessFollowUp.getFollowAluno(parametros, cd_aluno, cd_escola);
                transaction.Complete();
            }
            return retorno;
        }

        public List<Produto> getHistoricoTurmas(int cd_aluno, int cd_escola)
        {
            List<Produto> listProduto = new List<Produto>();
            List<HistoricoAluno> listaHistorico = DataAccessHistoricoAluno.getHistoricoTurmas(cd_aluno, cd_escola).ToList();

            for (int i = 0; i < listaHistorico.Count; i++)
                //Se não tem produto na lista, inclui ele:
                if (!listProduto.Where(p => p.cd_produto == listaHistorico[i].Produto.cd_produto).Any())
                {
                    Produto produto = new Produto();
                    produto.cd_produto = listaHistorico[i].Produto.cd_produto;
                    produto.no_produto = listaHistorico[i].Produto.no_produto;
                    produto.Turma = new List<Turma>();

                    Turma turma = new Turma();
                    turma.HistoricoAluno = new List<HistoricoAluno>();
                    turma.HistoricoAluno.Add(listaHistorico[i]);
                    turma.cd_turma = listaHistorico[i].Turma.cd_turma;
                    turma.no_turma = listaHistorico[i].Turma.no_turma;
                    produto.Turma.Add(turma);

                    listProduto.Add(produto);
                }
                else
                {
                    //Pega o produto respectivo ao histórico:
                    Produto produto = listProduto.Where(p => p.cd_produto == listaHistorico[i].Produto.cd_produto).First();
                    Turma turmaHist = listaHistorico[i].Turma;

                    //for(int j=0; j<listaTurma.Count; j++){
                    //Se não tem turma na lista da turma no produto, inclui ela:
                    if (!produto.Turma.Where(t => t.cd_turma == turmaHist.cd_turma).Any())
                    {
                        Turma turma = new Turma();
                        turma.HistoricoAluno = new List<HistoricoAluno>();
                        turma.HistoricoAluno.Add(listaHistorico[i]);
                        turma.cd_turma = turmaHist.cd_turma;
                        turma.no_turma = turmaHist.no_turma;
                        produto.Turma.Add(turma);
                    }
                    else
                    {
                        Turma turma = produto.Turma.Where(t => t.cd_turma == turmaHist.cd_turma).First();
                        if (turma.HistoricoAluno == null)
                            turma.HistoricoAluno = new List<HistoricoAluno>();
                        turma.HistoricoAluno.Add(listaHistorico[i]);
                    }
                }
            return listProduto;
        }

        public HistoricoAluno GetHistoricoAlunoPorDesistencia(int cdDesistencia)
        {
            return DataAccessHistoricoAluno.GetHistoricoAlunoPorDesistencia(cdDesistencia);
        }

        public DateTime? buscarDataHistoricoDesistenciaAlteriorCancelamento(int cd_aluno, int cd_turma, DateTime dt_historico, byte nm_sequencia)
        {
            return DataAccessHistoricoAluno.buscarDataHistoricoDesistenciaAlteriorCancelamento(cd_aluno, cd_turma, dt_historico, nm_sequencia);
        }

        public DataTable getAlunos(int cd_aluno, int Tipo, string produtos, string statustitulo)
        {
            return DataAccessHistoricoAluno.getAlunos(cd_aluno, Tipo, produtos, statustitulo);
        }

        public List<sp_RptHistoricoAlunoM_Result> getRtpHistoricoAlunoM(int cdAluno)
        {
            return DataAccessHistoricoAluno.getRtpHistoricoAlunoM(cdAluno);
        }

        public List<st_RptFaixaEtaria_Result> getRtpFaixaEtaria(int cd_escola, int tipo, int idade, int idade_max, int cd_turma)
        {
            return DataAccessHistoricoAluno.getRtpFaixaEtaria(cd_escola, tipo, idade, idade_max, cd_turma);
        }

        public DataTable getRtpFaixaEtariaDT(int cd_escola, int tipo, int idade, int idade_max, int cd_turma)
        {
            return DataAccessHistoricoAluno.getRtpFaixaEtariaDT(cd_escola, tipo, idade, idade_max, cd_turma);
        }

        public List<ProdutoHistoricoSeachUI> getProdutosComHistorico(int cd_escola)
        {
            return DataAccessHistoricoAluno.getProdutosComHistorico(cd_escola);
        }

        #endregion

        #region Desistência

        public IEnumerable<DesistenciaUI> getDesistenciaSearchUI(SearchParameters parametros, int? cd_turma, int? cd_aluno, int? cd_pessoa_escola, int? cd_motivo_desistencia, int cd_tipo, DateTime? dta_ini,
            DateTime? dta_fim, int cd_produto, int cd_professor, List<int> cdsCurso)
        {
            IEnumerable<DesistenciaUI> retorno = new List<DesistenciaUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_turma";
                parametros.sort = parametros.sort.Replace("dtaDesistencia", "dt_desistencia");
                retorno = DataAccessDesistencia.getDesistenciaSearchUI(parametros, cd_turma, cd_aluno, cd_pessoa_escola, cd_motivo_desistencia, cd_tipo, dta_ini, dta_fim, cd_produto, cd_professor, cdsCurso);
                transaction.Complete();
            }
            return retorno;
        }

        public DesistenciaUI addDesistencia(DesistenciaUI desistencia, int cd_pessoa_escola, int qtd_diario, int qtd_faltas, bool chamarApiCyber)
        {
            BusinessAluno.sincronizaContexto(DataAccessDesistencia.DB());
            DataAccessHistoricoAluno.sincronizaContexto(DataAccessDesistencia.DB());

            Desistencia newDesistencia = new Desistencia();

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessDesistencia.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                AlunoTurma alunoTurma = BusinessAluno.findAlunoTurma(desistencia.cd_aluno, desistencia.cd_turma, cd_pessoa_escola);
                LivroAlunoApiCyberBdUI livroAlunoTurmaApiCyber = null;
                if (BusinessApiNewCyberAluno.aplicaApiCyber())
                {
                    livroAlunoTurmaApiCyber = BusinessAluno.findLivroAlunoTurmaApiCyber(desistencia.cd_aluno, desistencia.cd_turma, cd_pessoa_escola);
                }

                if (alunoTurma != null && alunoTurma.cd_aluno_turma > 0)
                {
                    desistencia.cd_aluno_turma = alunoTurma.cd_aluno_turma;
                    DateTime? data_encerramento = alunoTurma.Turma.dt_termino_turma;

                    if (desistencia.id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA && 
                        data_encerramento != null && DateTime.Compare((DateTime)data_encerramento, DateTime.Parse(desistencia.dtaDesistencia)) == -1)
                        throw new SecretariaBusinessException(string.Format(Utils.Messages.Messages.msgErroDesistenciaTurmaEnc, data_encerramento) + " ", null, SecretariaBusinessException.TipoErro.ERRO_DESISTENCIA_TURMA_ENCERRADA, false);

                    newDesistencia.copy(desistencia, false);

                    validarInsersaoDesistencia(desistencia, cd_pessoa_escola);


                    newDesistencia.dt_desistencia = desistencia.dt_desistencia.ToLocalTime().Date;
                    newDesistencia = DataAccessDesistencia.add(newDesistencia, false);
                    desistencia.cd_desistencia = newDesistencia.cd_desistencia;

                    //Ativando ou desativando cadastro de aluno
                    BusinessAluno.editStatusAluno(desistencia.cd_aluno, desistencia.id_aluno_ativo, cd_pessoa_escola, desistencia.cd_turma);

                    //Montar Histórico do Aluno
                    if (alunoTurma.cd_contrato.HasValue && alunoTurma.cd_contrato.Value > 0)
                        gerarHistoricoAluno(desistencia, cd_pessoa_escola, newDesistencia.cd_desistencia, alunoTurma.cd_contrato, alunoTurma.cd_situacao_aluno_origem);
                    if (desistencia.id_tipo_desistencia == (int) DesistenciaDataAccess.tipoDesistencia.DESISTENCIA)
                    {
                        alterarAlunoTurma(desistencia, cd_pessoa_escola, qtd_diario, qtd_faltas);

                        if (BusinessApiNewCyberAluno.aplicaApiCyber())
                        {
                            if (livroAlunoTurmaApiCyber != null && chamarApiCyber == true && existeLivroAlunoByCodAluno(livroAlunoTurmaApiCyber.codigo_aluno, livroAlunoTurmaApiCyber.codigo_grupo, livroAlunoTurmaApiCyber.codigo_livro))
                            {
                                //chama a api cyber com o comanco (DELETA_LIVRO_ALUNO)
                                deletaLivroAlunoApiCyber(livroAlunoTurmaApiCyber, ApiCyberComandosNames.DELETA_LIVROALUNO);
                            }
                        }

                    }
                       
                }
                else
                    throw new SecretariaBusinessException(string.Format(Messages.msgRegNotEnc), null, SecretariaBusinessException.TipoErro.ERRO_DESISTENCIA_POSTERIOR, false);
                transaction.Complete();
            }

            return DataAccessDesistencia.getDesistenciaGridView(newDesistencia.cd_desistencia);
        }

        public bool existeLivroAlunoByCodAluno(int codigo_aluno, int codigo_grupo, int codigo_livro)
        {
            return BusinessApiNewCyberAluno.verificaRegistroLivroAlunos(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.VISUALIZA_LIVROALUNO, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo_aluno, codigo_grupo, codigo_livro);
        }

        private void deletaLivroAlunoApiCyber(LivroAlunoApiCyberBdUI livroAlunoCyberCurrent, string comando)
        {

            string parametros = "";

            //valida e retorna os parametros para a requisicao cyber
            parametros = validaParametrosCyberDeletaLivroAluno(livroAlunoCyberCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");

            string result = BusinessApiNewCyberAluno.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);

        }

        private string validaParametrosCyberDeletaLivroAluno(LivroAlunoApiCyberBdUI entity, string url, string comando, string parametros)
        {


            if (entity == null)
            {
                throw new ApiNewCyberAlunoException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberAlunoException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }


            if (entity.codigo_aluno <= 0)
            {
                throw new ApiNewCyberAlunoException(string.Format(Utils.Messages.Messages.ErroApiCyberCdAlunoMenorIgualZero, url, comando, parametros), null, ApiNewCyberAlunoException.TipoErro.ERRO_COD_ALUNO_MENOR_IGUAL_ZERO, false);
            }

            if (entity.codigo_grupo <= 0)
            {
                throw new ApiNewCyberAlunoException(string.Format(Utils.Messages.Messages.ErroApiCyberCodigoGrupoMenorIgualZero, url, comando, parametros), null, ApiNewCyberAlunoException.TipoErro.ERRO_CODIGO_GRUPO_MENOR_IGUAL_ZERO, false);
            }


            string listaParams = "";
            listaParams = string.Format("codigo_aluno={0},codigo_grupo={1}", entity.codigo_aluno, entity.codigo_grupo);
            return listaParams;
        }

        private void gerarHistoricoAluno(DesistenciaUI desistencia, int cd_pessoa_escola, int cd_desistencia, int? cd_contrato, byte? cd_situacao_origem)
        {
            int cdContrato = (int)cd_contrato;
            int cdProduto = BusinessAluno.findAlunoTurmaProduto(desistencia.cd_aluno, desistencia.cd_turma, cd_pessoa_escola);
            byte segHistorico = (byte)DataAccessHistoricoAluno.retunMaxSequenciaHistoricoAluno(cdProduto, cd_pessoa_escola, desistencia.cd_aluno);
            byte situacaoAluno = desistencia.id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA ? Convert.ToByte(FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Desistente) : (byte)cd_situacao_origem;
            byte tipo = desistencia.id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA ? (byte)HistoricoAluno.TipoMovimento.DESISTENCIA : (byte)HistoricoAluno.TipoMovimento.CANCELAR_DESISTENCIA;


            HistoricoAluno historico = new HistoricoAluno
            {
                cd_aluno = desistencia.cd_aluno,
                cd_turma = desistencia.cd_turma,
                cd_contrato = cdContrato,
                id_tipo_movimento = tipo,
                dt_cadastro = DateTime.UtcNow,
                dt_historico = desistencia.dt_desistencia.ToLocalTime().Date,
                cd_produto = cdProduto,
                nm_sequencia = ++segHistorico,
                id_situacao_historico = situacaoAluno,
                cd_usuario = desistencia.cd_usuario,
                cd_desistencia = cd_desistencia,
                id_desistencia = desistencia.id_tipo_desistencia == (byte)HistoricoAluno.TipoMovimento.DESISTENCIA ? true : false
            };
            addHistoricoAluno(historico);
        }

        public DesistenciaUI editDesistencia(DesistenciaUI desistenciaUI, int cd_pessoa_escola, int qtd_diario, bool chamaApiCyber)
        {
            Desistencia editDesistencia = new Desistencia();
            DesistenciaUI desistenciaEdit = new DesistenciaUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                AlunoTurma alunoTurma = BusinessAluno.findAlunoTurma(desistenciaUI.cd_aluno, desistenciaUI.cd_turma, cd_pessoa_escola);
                LivroAlunoApiCyberBdUI livroAlunoTurmaApiCyber = null;
                if (BusinessApiNewCyberAluno.aplicaApiCyber())
                {
                    livroAlunoTurmaApiCyber = BusinessAluno.findLivroAlunoTurmaApiCyber(desistenciaUI.cd_aluno, desistenciaUI.cd_turma, cd_pessoa_escola);
                }

                desistenciaUI.dt_desistencia = desistenciaUI.dt_desistencia.ToLocalTime().Date;
                BusinessAluno.sincronizaContexto(DataAccessDesistencia.DB());
                DataAccessHistoricoAluno.sincronizaContexto(DataAccessDesistencia.DB());

                editDesistencia = DataAccessDesistencia.findById(desistenciaUI.cd_desistencia, false);

                DateTime? data_encerramento = alunoTurma.Turma.dt_termino_turma;

                if (data_encerramento != null && DateTime.Compare((DateTime)data_encerramento, desistenciaUI.dt_desistencia) == -1)
                    throw new SecretariaBusinessException(string.Format(Utils.Messages.Messages.msgErroDesistenciaTurmaEnc, data_encerramento) + " ", null, SecretariaBusinessException.TipoErro.ERRO_DESISTENCIA_TURMA_ENCERRADA, false);
                
                validarEdicaoDesistencia(desistenciaUI, cd_pessoa_escola, editDesistencia.dt_desistencia);

                //Alterar data do Histórico quando alterar a data de Desistência  
                if (DateTime.Compare(editDesistencia.dt_desistencia, desistenciaUI.dt_desistencia) != 0)
                {
                    HistoricoAluno historicoAlterar = DataAccessHistoricoAluno.GetHistoricoAlunoPorDesistencia(desistenciaUI.cd_desistencia);
                    if (historicoAlterar != null)
                    {
                        historicoAlterar.dt_historico = desistenciaUI.dt_desistencia;
                        historicoAlterar.id_desistencia = desistenciaUI.id_tipo_desistencia == (byte)HistoricoAluno.TipoMovimento.DESISTENCIA ? true : false;
                        DataAccessHistoricoAluno.saveChanges(false);
                    }
                }
                //Ativando ou desativando cadastro de aluno
                BusinessAluno.editStatusAluno(desistenciaUI.cd_aluno, desistenciaUI.id_aluno_ativo, cd_pessoa_escola, desistenciaUI.cd_turma);


                editDesistencia.copy(desistenciaUI, false);
                editDesistencia.dt_desistencia = editDesistencia.dt_desistencia.ToLocalTime().Date;
                desistenciaUI.dt_desistencia = desistenciaUI.dt_desistencia.ToUniversalTime();

                DataAccessDesistencia.saveChanges(false);

                alterarAlunoTurma(desistenciaUI, cd_pessoa_escola, qtd_diario, 0);

                if (BusinessApiNewCyberAluno.aplicaApiCyber())
                {
                    //desistenciaEdit = DesistenciaUI.fromDesistenciaUI(desistenciaUI);
                    if (desistenciaUI.id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA)
                    {
                        if (livroAlunoTurmaApiCyber != null && chamaApiCyber == true &&
                            existeLivroAlunoByCodAluno(livroAlunoTurmaApiCyber.codigo_aluno, livroAlunoTurmaApiCyber.codigo_grupo, livroAlunoTurmaApiCyber.codigo_livro))
                        {
                            //chama a api cyber com o comanco (DELETA_LIVRO_ALUNO)
                            deletaLivroAlunoApiCyber(livroAlunoTurmaApiCyber, ApiCyberComandosNames.DELETA_LIVROALUNO);
                        }

                    }
                }

                transaction.Complete();
            }
            desistenciaEdit = DataAccessDesistencia.getDesistenciaGridView(desistenciaUI.cd_desistencia);
            return desistenciaEdit;
        }

        public void alterarAlunoTurma(DesistenciaUI desistenciaUI, int cd_pessoa_escola, int qtd_diario, int qtd_faltas)
        {
            int[] alunos = { desistenciaUI.cd_aluno };
            AlunoTurma alunoTurma = BusinessAluno.findAlunosTurma(desistenciaUI.cd_turma, cd_pessoa_escola, alunos).FirstOrDefault();

            alunoTurma.nm_aulas_dadas = qtd_diario;
            alunoTurma.nm_faltas -= byte.Parse(qtd_faltas.ToString());

            //verifincado se é desitencia, caso verdadeiro alterar o status do aluno e a data de situação. Se for cancelamento muda o status do aluno para ativo
            if (desistenciaUI.id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA)
            {
                //altera a situação do aluno na turma                    
                if (alunoTurma.cd_situacao_aluno_turma == (byte)AlunoTurma.SituacaoAlunoTurma.Ativo
                      || alunoTurma.cd_situacao_aluno_turma == (byte)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                    alunoTurma.cd_situacao_aluno_origem = (byte)alunoTurma.cd_situacao_aluno_turma;

                alunoTurma.cd_situacao_aluno_turma = (int)AlunoTurma.SituacaoAlunoTurma.Desistente;
            }
            else
                //altera a situação do aluno na turma
                alunoTurma.cd_situacao_aluno_turma = (byte)alunoTurma.cd_situacao_aluno_origem;//(int)AlunoTurma.SituacaoAlunoTurma.Ativo;

            BusinessAluno.saveChagesAlunoTurma(alunoTurma);
        }

        public DesistenciaUI getDesistenciaAlunoTurma(int cd_aluno_turma, int cd_pessoa_escola)
        {
            return DataAccessDesistencia.getDesistenciaAlunoTurma(cd_aluno_turma, cd_pessoa_escola);
        }

        public bool getExisteDesistenciaPorAlunoTurma(int cd_aluno, int cd_turma, int cd_pessoa_escola)
        {
            return DataAccessDesistencia.getExisteDesistenciaPorAlunoTurma(cd_aluno, cd_turma, cd_pessoa_escola);
        }

        public void validarInsersaoDesistencia(DesistenciaUI desistenciaUI, int cd_pessoa_escola)
        {
            Desistencia ultimaDesistencia = DataAccessDesistencia.retornaDesistenciaMax(desistenciaUI, cd_pessoa_escola);

            // verifica se o primeiro registro do usuario, o usário so pode inserir uma desistência, caso ainda não exista registro.
            if ((ultimaDesistencia == null) && (desistenciaUI.id_tipo_desistencia == (byte)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTO || 
                                                desistenciaUI.id_tipo_desistencia == (byte)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTONAOREMATRICULA))
                throw new AlunoBusinessException(Utils.Messages.Messages.msgAlunoNaoDesistente + " ", null, AlunoBusinessException.TipoErro.ERRO_ALUNO_STATUS_NAO_DESISTENTE, false);

            if ((ultimaDesistencia != null) && (desistenciaUI.id_tipo_desistencia != (byte)DesistenciaDataAccess.tipoDesistencia.TODOS))
            {
                //Não é permitido incluir duas desistências
                if (ultimaDesistencia.id_tipo_desistencia == desistenciaUI.id_tipo_desistencia)
                    exceptionDesistenciaByTipo(desistenciaUI);
                else
                    exceptionDataDesistencia(desistenciaUI, ultimaDesistencia);
            }
        }

        private bool getMaiorDataAposDataDesistencia(DesistenciaUI desistenciaUI, int cd_pessoa_escola)
        {
            return DataAccessDesistencia.getMaiorDataAposDataDesistencia(desistenciaUI, cd_pessoa_escola);
        }

        private DateTime? getMenorDataAposDataDesistencia(DesistenciaUI desistenciaUI, int cd_pessoa_escola)
        {
            return DataAccessDesistencia.getMenorDataAposDataDesistencia(desistenciaUI, cd_pessoa_escola);
        }

        //Exeções

        private void validarEdicaoDesistencia(DesistenciaUI desistenciaUI, int cd_pessoa_escola, DateTime dtaDesistencia)
        {
            DateTime dataEditada = desistenciaUI.dt_desistencia.ToLocalTime().Date;

            //Substitui a data modificada pela original para fazer a pesquisa, e retornar a mairo data difente da mesma.
            desistenciaUI.dt_desistencia = dtaDesistencia;
            bool maiorDataDesistencia = getMaiorDataAposDataDesistencia(desistenciaUI, cd_pessoa_escola);
            DateTime? menorDataDesistecia = getMenorDataAposDataDesistencia(desistenciaUI, cd_pessoa_escola);

            if (maiorDataDesistencia)
                throw new SecretariaBusinessException(Utils.Messages.Messages.msgDataDesistenciaForaIntervalo + " ", null, SecretariaBusinessException.TipoErro.ERRO_DATA_FORA_INTERVALO_VALIDO, false);

            if (menorDataDesistecia != null)
                if ((DateTime.Compare(menorDataDesistecia.Value, dataEditada) >= 0))
                    throw new SecretariaBusinessException(Utils.Messages.Messages.msgDataDesistenciaForaIntervalo + " ", null, SecretariaBusinessException.TipoErro.ERRO_DATA_FORA_INTERVALO_VALIDO, false);

            desistenciaUI.dt_desistencia = dataEditada;
        }

        private static void exceptionDataDesistencia(DesistenciaUI desistenciaUI, Desistencia ultimaDesistencia)
        {
            int resultData = DateTime.Compare(ultimaDesistencia.dt_desistencia, desistenciaUI.dt_desistencia);
            //o usuario vai efetuar um cancelamento e data é menor que a data existente
            if (resultData > 0)
                throw new SecretariaBusinessException(Utils.Messages.Messages.msgDataMenorDesistencia + " " + String.Format("{0:dd/MM/yyyy}", ultimaDesistencia.dt_desistencia.ToLocalTime()) + ".", null, SecretariaBusinessException.TipoErro.ERRO_DATA_MENOR_DESISTENCIA, false);
        }

        private static void exceptionDesistenciaByTipo(DesistenciaUI desistenciaUI)
        {
            switch (desistenciaUI.id_tipo_desistencia)
            {
                case (byte)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA:
                    throw new AlunoBusinessException(Utils.Messages.Messages.msgAlunoDesistente + " ", null, AlunoBusinessException.TipoErro.ERRO_ALUNO_ESTA_DESISTENTE, false);

                case (byte)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTO:
                    throw new AlunoBusinessException(Utils.Messages.Messages.msgAlunoCancelado + " ", null, AlunoBusinessException.TipoErro.ERRO_ALUNO_ESTA_CANCELADO, false);

                case (byte)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTONAOREMATRICULA:
                    throw new AlunoBusinessException(Utils.Messages.Messages.msgAlunoCancelado + " ", null, AlunoBusinessException.TipoErro.ERRO_ALUNO_ESTA_CANCELADO, false);
                default: throw new Exception();
            };
        }

        public DateTime? getMaiorDataDesistencia(DesistenciaUI desistenciaUI, int cd_pessoa_escola)
        {
            return DataAccessDesistencia.getMaiorDataDesistencia(desistenciaUI, cd_pessoa_escola);
        }

        public int retornaQuantidadeDesistencia(int cd_turma, int cd_aluno, int cd_pessoa_escola, int cd_aluno_turma)
        {
            return DataAccessDesistencia.retornaQuantidadeDesistencia(cd_turma, cd_aluno, cd_pessoa_escola, cd_aluno_turma);
        }

        public Desistencia findByIdDesistencia(int cd_desistencia)
        {
            return DataAccessDesistencia.findById(cd_desistencia, false);
        }

        public bool deleteDesistencia(Desistencia desistencia)
        {
            return DataAccessDesistencia.delete(desistencia, false);
        }

        public Desistencia retornaDesistenciaMax(DesistenciaUI desistenciaUI, int cd_pessoa_escola)
        {
            return DataAccessDesistencia.retornaDesistenciaMax(desistenciaUI, cd_pessoa_escola);
        }

        #endregion

        #region Ação Follow-up
        public IEnumerable<AcaoFollowUp> GetAcaoFollowUpSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            IEnumerable<AcaoFollowUp> retorno = new List<AcaoFollowUp>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "dc_acao_follow_up";
                parametros.sort = parametros.sort.Replace("acao_ativa", "id_acao_ativa");

                retorno = DataAccessAcaoFollowup.GetAcaoFollowUpSearch(parametros, descricao, inicio, ativo);
                transaction.Complete();
            }
            return retorno;
        }

        public AcaoFollowUp GetAcaoFollowUpById(int id)
        {
            return DataAccessAcaoFollowup.findById(id, false);
        }

        public AcaoFollowUp PostAcaoFollowUp(AcaoFollowUp acaoFollowUp)
        {
            DataAccessAcaoFollowup.add(acaoFollowUp, false);
            return acaoFollowUp;
        }

        public AcaoFollowUp PutAcaoFollowUp(AcaoFollowUp acaoFollowUp)
        {
            DataAccessAcaoFollowup.edit(acaoFollowUp, false);
            return acaoFollowUp;
        }

        public bool DeleteAcaoFollowUp(List<AcaoFollowUp> acoesFollowUp)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (AcaoFollowUp af in acoesFollowUp)
                {
                    AcaoFollowUp acao = DataAccessAcaoFollowup.findById(af.cd_acao_follow_up, false);
                    deleted = DataAccessAcaoFollowup.delete(acao, false);
                }
                transaction.Complete();
                return deleted;
            }
        }

        public IEnumerable<AcaoFollowUp> getAcaoFollowUp(AcaoFollowupDataAccess.TipoPesquisaAcaoEnum tipo, int cd_acao_follow_up)
        {
            return DataAccessAcaoFollowup.getAcaoFollowUp(tipo, cd_acao_follow_up);
        }
        #endregion

        #region Aluno

        public AlunoSearchUI addAluno(AlunoUI alunoUI)
        {
            Aluno aluno;
            sincronizarContextos(DataAccessAcaoFollowup.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessAcaoFollowup.DB()))
            {
                aluno = BusinessAluno.addAluno(alunoUI);
                if (alunoUI.followUpUI != null && alunoUI.followUpUI.Count() > 0)
                {
                    List<FollowUp> listFollowUp = new List<FollowUp>();
                    List<FollowUp> novalistFollowUp = alunoUI.followUpUI.ToList();
                    for (int i = 0; i < novalistFollowUp.Count; i++)
                        DataAccessFollowUp.add(new FollowUp()
                        {
                            cd_aluno = aluno.cd_aluno,
                            cd_prospect = null,
                            cd_usuario =  novalistFollowUp[i].cd_usuario > 0 ? novalistFollowUp[i].cd_usuario : alunoUI.cdUsuarioFollowUp,
                            dc_assunto = novalistFollowUp[i].dc_assunto,
                            dt_follow_up = novalistFollowUp[i].dt_follow_up,
                            id_tipo_follow = (byte)FollowUp.TipoFollowUp.PROSPECT_ALUNO,
                            cd_acao_follow_up = novalistFollowUp[i].cd_acao_follow_up,
                            cd_escola = aluno.cd_pessoa_escola,
                            dt_proximo_contato = novalistFollowUp[i].dt_proximo_contato != null ? novalistFollowUp[i].dt_proximo_contato.Value.Date : novalistFollowUp[i].dt_proximo_contato
                        }, false);
                }

                if (alunoUI.alunosRestricaoUI != null && alunoUI.alunosRestricaoUI.Count() > 0)
                {
                    List<AlunoRestricao> listFollowUp = new List<AlunoRestricao>();
                    List<AlunoRestricao> novalistaAlunoRestricao = alunoUI.alunosRestricaoUI.ToList();
                    for (int i = 0; i < novalistaAlunoRestricao.Count; i++)
                        DataAccessAlunoRestricao.add(new AlunoRestricao()
                        {
                            cd_aluno = aluno.cd_aluno,
                            cd_aluno_resticao = novalistaAlunoRestricao[i].cd_aluno_resticao,
                            cd_orgao_financeiro = novalistaAlunoRestricao[i].cd_orgao_financeiro,
                            cd_usuario = novalistaAlunoRestricao[i].cd_usuario,
                            dt_inicio_restricao = novalistaAlunoRestricao[i].dt_inicio_restricao.Date,
                            dt_final_restricao = novalistaAlunoRestricao[i].dt_final_restricao?.Date,
                            dt_cadastro = DateTime.UtcNow
                }, false);
                }

                transaction.Complete();
            }
            return BusinessAluno.getAlunoByCodForGrid(aluno.cd_pessoa_aluno, aluno.cd_pessoa_escola);
        }

        public AlunoSearchUI editAluno(AlunoUI alunoUI)
        {
            sincronizarContextos(DataAccessFollowUp.DB());
            List<RelacionamentoSGF> relacionamentos = new List<RelacionamentoSGF>();
            Aluno aluno;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessFollowUp.DB()))
            {
                aluno = BusinessAluno.editAluno(alunoUI);
                if (alunoUI.atualizaFollowUp)
                    setFollowUpAluno(alunoUI.followUpUI != null ? alunoUI.followUpUI.ToList() : new List<FollowUp>(), aluno.cd_aluno, alunoUI.cdUsuarioFollowUp, alunoUI.aluno.cd_pessoa_escola);
                
                //Alualiza o alunoRestricao
                if (alunoUI.alunosRestricaoUI != null && alunoUI.alunosRestricaoUI.Count > 0)
                {
                    crudUpdateAlunoRestricao(alunoUI.alunosRestricaoUI.ToList(), aluno.cd_aluno);
                }
                transaction.Complete();
            }
            AlunoSearchUI alunoGrid = BusinessAluno.getAlunoByCodForGrid(aluno.cd_pessoa_aluno, aluno.cd_pessoa_escola);
            

            return alunoGrid;
        }

        public bool deletarAlunos(List<Aluno> alunos, int cd_escola)
        {
            this.sincronizarContextos(DataAccessFollowUp.DB());
            bool retorno = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessFollowUp.DB()))
            {
                if (alunos != null && alunos.Count() > 0)
                    foreach (var alunoView in alunos)
                    {
                        var aluno = BusinessAluno.findAlunoById(alunoView.cd_aluno, cd_escola);
                        alunoView.cd_pessoa_aluno = aluno.cd_pessoa_aluno;
                        if (aluno != null)
                        {
                            var listFollowUpAluno = DataAccessFollowUp.getFollowUpByAlunoAllData(alunoView.cd_aluno, cd_escola).ToList();
                            if (listFollowUpAluno != null && listFollowUpAluno.Count() > 0)
                                for (int i = 0; i < listFollowUpAluno.Count(); i++)
                                    DataAccessFollowUp.delete(listFollowUpAluno[i], false);
                            //Remove as Restrições Financeiras
                            var listAlunoRestricao = DataAccessAlunoRestricao.getAlunoRestricaoEditByCdAluno(alunoView.cd_aluno).ToList();
                            if (listAlunoRestricao != null && listAlunoRestricao.Count() > 0)
                            {
                                for (int i = 0; i < listAlunoRestricao.Count(); i++)
                                {
                                    DataAccessAlunoRestricao.delete(listAlunoRestricao[i], false);
                                }
                                    
                            }

                            BusinessAluno.deleteFichaAluno(aluno.cd_aluno);
                               
                            BusinessAluno.setHorarioAluno(new List<Horario>(), alunoView.cd_aluno, cd_escola);
                            BusinessAluno.findByIdAndDeleteAlunoBolsa(aluno.cd_aluno);
                            BusinessAluno.deletarAluno(aluno);
                            retorno = true;
                        }
                    }
                transaction.Complete();
            }
            if (retorno && alunos != null && alunos.Count() > 0)
                foreach (var alunoView in alunos)
                {
                    try
                    {
                        BusinessPessoa.deletePessoa(alunoView.cd_pessoa_aluno);
                    }
                    catch
                    {
                        //Execption não tratado pois a exclusão e condicional (se existir ligações com a pessoa, ela não poderá ser excluida).
                    }
                }

            if (BusinessApiNewCyberAluno.aplicaApiCyber())
            {
                //Chama a api Cyber para os alunos deletados
                foreach (Aluno aluno in alunos)
                {
                    //Se o aluno existe na api Cyber
                    if (aluno != null && aluno.cd_aluno > 0 && existeAluno(aluno.cd_aluno))
                    {
                        //chama a api cyber com o comando(INATIVA_ALUNO)
                        executaCyberInativaAluno(aluno.cd_aluno);
                    }

                }
            }


            return retorno;
        }

        public void executaCyberInativaAluno(int codigo)
        {
            string result = BusinessApiNewCyberAluno.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.INATIVA_ALUNO, ConfigurationManager.AppSettings["chaveApiNewCyber"], String.Format("codigo={0}", codigo));
        }

        public bool existeAluno(int codigo)
        {
            return BusinessApiNewCyberAluno.verificaRegistro(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.VISUALIZA_ALUNO, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo);
        }

        private void setFollowUpAluno(List<FollowUp> followUpUI, int cdAluno, int cdUsuarioAtend, int cd_escola)
        {
            List<FollowUp> followUpView = followUpUI;
            FollowUp followUp = new FollowUp();
            DataAccessFollowUp.sincronizaContexto(DataAccessFollowUp.DB());
            IEnumerable<FollowUp> followUpContext = DataAccessFollowUp.getFollowUpByAlunoAllData(cdAluno, cd_escola).ToList();
            IEnumerable<FollowUp> followUpAlunoComCodigo = followUpView.Where(hpts => hpts.cd_follow_up != 0);
            IEnumerable<FollowUp> followUpAlunoDeleted = followUpContext.Where(tc => !followUpAlunoComCodigo.Any(tv => tc.cd_follow_up == tv.cd_follow_up));

            if (followUpAlunoDeleted.Count() > 0)
                foreach (var item in followUpAlunoDeleted)
                    if (item != null)
                    {
                        if (cdUsuarioAtend != item.cd_usuario)
                            throw new AlunoBusinessException(Messages.msgErroPermissaoAlterarFollowUp, null, AlunoBusinessException.TipoErro.ERRO_FOLLOW_UP_DE_OUTRO_ATENDENTE, false);
                        DataAccessFollowUp.delete(item, false);
                    }

            foreach (var item in followUpView)
            {
                if (item.cd_follow_up.Equals(null) || item.cd_follow_up == 0)
                {
                    //if (item.dc_assunto.Count() > 3000) // Limite aumentado devido formatação html do campo textarea editor.
                        //throw new AlunoBusinessException(Messages.msgErroLimiteAssuntoFollowUp, null, AlunoBusinessException.TipoErro.ERRO_FOLLOW_LIMITE_DC_ASSUNTO, false);

                    item.cd_usuario = cdUsuarioAtend;
                    item.cd_aluno = cdAluno;
                    item.cd_escola = cd_escola;
                    item.id_tipo_follow = (byte)FollowUp.TipoFollowUp.PROSPECT_ALUNO;
                    DataAccessFollowUp.add(item, false);
                }
                else if (item.id_alterado)
                {
                    var followUpAluno = followUpContext.Where(hp => hp.cd_follow_up == item.cd_follow_up).FirstOrDefault();
                    if (followUpAluno != null && followUpAluno.cd_follow_up > 0)
                    {
                        //if (item.dc_assunto.Count() > 3000) // Limite aumentado devido formatação html do campo textarea editor.
                            //throw new AlunoBusinessException(Messages.msgErroLimiteAssuntoFollowUp, null, AlunoBusinessException.TipoErro.ERRO_FOLLOW_LIMITE_DC_ASSUNTO, false);

                        followUpAluno.dc_assunto = item.dc_assunto;
                        followUpAluno.id_follow_lido = item.id_follow_lido;
                        followUpAluno.id_follow_resolvido = item.id_follow_resolvido;
                        followUpAluno.dt_proximo_contato = item.dt_proximo_contato != null ? item.dt_proximo_contato.Value.Date : item.dt_proximo_contato;
                        followUpAluno.cd_acao_follow_up = item.cd_acao_follow_up;
                        followUpAluno.id_tipo_atendimento = item.id_tipo_atendimento;
                        followUpAluno.cd_usuario_destino = item.cd_usuario_destino;
                        if ((DataAccessFollowUp.DB().Entry(followUpAluno).State == System.Data.Entity.EntityState.Modified) && cdUsuarioAtend != followUpAluno.cd_usuario)
                            throw new AlunoBusinessException(Messages.msgErroPermissaoAlterarFollowUp, null, AlunoBusinessException.TipoErro.ERRO_FOLLOW_UP_DE_OUTRO_ATENDENTE, false);

                    }
                }
            }
            DataAccessFollowUp.saveChanges(false);
        }


        private void crudUpdateAlunoRestricao(List<AlunoRestricao> alunoRestricaoUI, int cdAluno)
        {
            List<AlunoRestricao> alunoRestricaoView = alunoRestricaoUI;
            AlunoRestricao followUp = new AlunoRestricao();
            DataAccessAlunoRestricao.sincronizaContexto(DataAccessAlunoRestricao.DB());
            IEnumerable<AlunoRestricao> alunoRestricaoContext = DataAccessAlunoRestricao.getAlunoRestricaoEditByCdAluno(cdAluno).ToList();
            IEnumerable<AlunoRestricao> alunoRestricaoComCodigo = alunoRestricaoView.Where(arv => arv.cd_aluno_resticao != 0);
            IEnumerable<AlunoRestricao> alunoRestricaoDeleted = alunoRestricaoContext.Where(tc => !alunoRestricaoComCodigo.Any(tv => tc.cd_aluno_resticao == tv.cd_aluno_resticao));

            if (alunoRestricaoDeleted.Count() > 0)
                foreach (var item in alunoRestricaoDeleted)
                    if (item != null)
                    {
                        DataAccessAlunoRestricao.delete(item, false);
                    }

            foreach (var item in alunoRestricaoView)
            {
                if (item.cd_aluno_resticao.Equals(null) || item.cd_aluno_resticao == 0)
                {
                    DataAccessAlunoRestricao.add(new AlunoRestricao()
                    {
                        cd_aluno = cdAluno,
                        cd_aluno_resticao = item.cd_aluno_resticao,
                        cd_orgao_financeiro = item.cd_orgao_financeiro,
                        cd_usuario = item.cd_usuario,
                        dt_inicio_restricao = item.dt_inicio_restricao.Date,
                        dt_final_restricao = item.dt_final_restricao?.Date,
                        dt_cadastro = DateTime.UtcNow
                    }, false);

                    
                }else 
                {
                    var alunoRestricaoEdit = alunoRestricaoContext.Where(ar => ar.cd_aluno_resticao == item.cd_aluno_resticao).FirstOrDefault();
                    if (alunoRestricaoEdit != null && alunoRestricaoEdit.cd_aluno_resticao > 0)
                    {

                        alunoRestricaoEdit.cd_aluno = item.cd_aluno;
                        alunoRestricaoEdit.cd_aluno_resticao = item.cd_aluno_resticao;
                        alunoRestricaoEdit.cd_orgao_financeiro = item.cd_orgao_financeiro;
                        alunoRestricaoEdit.cd_usuario = item.cd_usuario;
                        alunoRestricaoEdit.dt_inicio_restricao = item.dt_inicio_restricao.Date;
                        alunoRestricaoEdit.dt_final_restricao = item.dt_final_restricao?.Date;
                        alunoRestricaoEdit.dt_cadastro = DateTime.UtcNow;

                    }
                }
            }
            DataAccessAlunoRestricao.saveChanges(false);
        }

        #endregion

        #region Aluno Turma

        public IEnumerable<AlunoTurma> findAlunosTurmaPorTurmaEscola(int cd_turma, int cd_escola)
        {
            return BusinessAluno.findAlunosTurmaPorTurmaEscola(cd_turma, cd_escola);
        }

        public void deletarAlunoTurma(AlunoTurma alunoTurma){
            BusinessAluno.deletarAlunoTurma(alunoTurma); 
        }
        public AlunoTurma addAlunoTurma(AlunoTurma alunoTurma)
        {
            return BusinessAluno.addAlunoTurma(alunoTurma);
        }
        public List<AlunoTurma> findAlunosTurmaForEncerramento(int cd_turma, int cd_escola)
        {
            return BusinessAluno.findAlunosTurmaForEncerramento(cd_turma, cd_escola);
        }

        public IEnumerable<AlunoTurma> findAlunosTurma(int cd_turma, int cd_escola, int[] cdAlunos)
        {
            return BusinessAluno.findAlunosTurma(cd_turma, cd_escola, cdAlunos);
        }

        public bool deleteAlunoAguardandoTurma(int cdProduto, int cdEscola, int cdContrato, int cdAluno, int cd_turma)
        {
            return BusinessAluno.deleteAlunoAguardandoTurma(cdProduto, cdEscola, cdContrato, cdAluno, cd_turma);
        }

        public AlunoTurma findAlunoTurma(int cd_aluno, int cd_turma, int cd_escola)
        {
            return BusinessAluno.findAlunoTurma(cd_aluno, cd_turma, cd_escola);
        }

        public AlunoTurma findAlunoTurmaByCdCursoContrato(int cd_curso_contrato, int cd_escola)
        {
            return BusinessAluno.findAlunoTurmaByCdCursoContrato(cd_curso_contrato, cd_escola);
        }

        public List<AlunoTurma> existsAlunosTurmaInTurmaDestino(List<int> cdsAlunosTurma, int cdTurmaDestino)
        {
            return BusinessAluno.existsAlunosTurmaInTurmaDestino(cdsAlunosTurma, cdTurmaDestino);
        }

        public bool existsAlunoTurmaByContratoEscola(int cd_contrato, int cd_pessoa_escola)
        {
            return BusinessAluno.existsAlunoTurmaByContratoEscola(cd_contrato, cd_pessoa_escola);
        }

        public IEnumerable<AlunoTurma> findAlunosTurmaHist(int cd_turma, int cd_escola, int[] cdAlunos)
        {
            return BusinessAluno.findAlunosTurmaHist(cd_turma, cd_escola, cdAlunos);
        }

        public AlunoTurma findAlunoTurmaById(int id)
        {
            return BusinessAluno.findAlunoTurmaById(id);
        }

        #endregion

        public bool verificarMasterGeral(string login)
        {
            return BusinessEmpresa.VerificarMasterGeral(login);
        }

        public bool verificarMasterGeral(int cdUsuario)
        {
            return BusinessEmpresa.VerificarMasterGeral(cdUsuario);
        }

        #region Ano Escolar
        public IEnumerable<AnoEscolar> GetAnoEscolarSearch(SearchParameters parametros, int? cdEscolaridade, string descricao, bool inicio, bool? ativo)
        {
            IEnumerable<AnoEscolar> retorno = new List<AnoEscolar>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_escolaridade";
                parametros.sort = parametros.sort.Replace("ano_escolar_ativo", "id_ativo");

                retorno = DataAccessAnoEscolar.GetAnoEscolarSearch(parametros, cdEscolaridade, descricao, inicio, ativo);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Escolaridade> getEscolaridadePossuiAnoEscolar()
        {
            IEnumerable<Escolaridade> retorno = new List<Escolaridade>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessAnoEscolar.GetEscolaridadePossuiAnoEscolar();
                transaction.Complete();
            }
            return retorno;
        }

        public AnoEscolar GetAnoEscolarById(int id)
        {
            return DataAccessAnoEscolar.GetAnoEscolarById(id);
        }

        public AnoEscolar PostAnoEscolar(AnoEscolar anoEscolar)
        {
            if (anoEscolar.nm_ordem <= 0)
                anoEscolar.nm_ordem = (GetUltimoNmOrdem(anoEscolar.cd_escolaridade) + 1);

            DataAccessAnoEscolar.add(anoEscolar, false);
            return GetAnoEscolarById(anoEscolar.cd_ano_escolar);
        }

        private int GetUltimoNmOrdem(int cd_escolaridade)
        {
            return DataAccessAnoEscolar.GetUltimoNmOrdem(cd_escolaridade);
        }

        public AnoEscolar PutAnoEscolar(AnoEscolar anoEscolar)
        {
            if (anoEscolar.nm_ordem <= 0)
                anoEscolar.nm_ordem = (GetUltimoNmOrdem(anoEscolar.cd_escolaridade) + 1);

            DataAccessAnoEscolar.edit(anoEscolar, false);
            return GetAnoEscolarById(anoEscolar.cd_ano_escolar); 
        }

        public bool DeleteAnoEscolar(List<AnoEscolar> anoEscolar)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (AnoEscolar anesc in anoEscolar)
                {
                    AnoEscolar esc = DataAccessAnoEscolar.findById(anesc.cd_ano_escolar, false);
                    deleted = DataAccessAnoEscolar.delete(esc, false);
                }
                transaction.Complete();
                return deleted;
            }
        }

        public IEnumerable<AnoEscolar> getAnoEscolaresAtivos(int? cdAnoEscolar)
        {
            return DataAccessAnoEscolar.getAnoEscolaresAtivos(cdAnoEscolar);
        }

        #endregion

        // da procedure sp_excluir_contrato 
        public string postDeleteMatricula(int cd_contrato, int cd_usuario, int fuso)
        {
            string retorno = null;

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                retorno = DataAccessMatricula.postDeleteMatricula(cd_contrato, cd_usuario, fuso);
                transaction.Complete();
            }
            return retorno;
        }

        #region XML Gerar
        public IEnumerable<ImportacaoXML> getListaXmlGerados(SearchParameters parametros, XmlSearchUI notatualizaUI)
        {
            IEnumerable<ImportacaoXML> retorno = new List<ImportacaoXML>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))            
            {
                if (parametros.sort == null)
                    parametros.sort = "cd_importacao_XML";

                retorno = DataAccessGeraNotasXml.getListaXmlGerados(parametros, notatualizaUI);
                transaction.Complete();
            }
            return retorno;
        }

        // chamada da procedure sp_dir_xml
        public int abrirGerarMXL(int cd_usuario)
        {
            int retorno = 0;

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.READ_COMMITED))
            {
                retorno = DataAccessGeraNotasXml.abrirGerarXML(cd_usuario);
                transaction.Complete();
            }
            return retorno;
        }

        // chama a procedure sp_import_xml ao clicar no botão "Gerar"
        public int postGerarXmlProc(int cd_usuario) 
        {
            int retorno = 0;

            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.READ_COMMITED, DataAccessGeraNotasXml.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            //{
                retorno = DataAccessGeraNotasXml.postGerarXmlProc(cd_usuario);
            //    transaction.Complete();
            //}
            return retorno;
        }

        public IEnumerable<ImportacaoXML> buscarGerarXML(int cd_usuario)
        {
            return DataAccessGeraNotasXml.buscarGerarXML(cd_usuario);
        }

        public List<int> setAtualizaXML(List<int> cdImportXML)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                List<ImportacaoXML> listaImports = DataAccessGeraNotasXml.setAtualizaXML(cdImportXML).ToList();

                foreach (var item in listaImports)
                {
                    item.id_resolvido = Convert.ToBoolean(1 - Convert.ToInt32(item.id_resolvido));
                    DataAccessGeraNotasXml.edit(item, false);
                }
                DataAccessGeraNotasXml.saveChanges(false);
                transaction.Complete();
            }
            return cdImportXML;
        }

        #endregion

        #region Usuario

        public IEnumerable<Escola> findAllEmpresaByCdUsuario(int codUsuario)
        {
            return BusinessEmpresa.findAllEmpresaByCdUsuario(codUsuario);
        }

        #endregion

        #region Enviar SMS

        //public IEnumerable<SmsParametroUI> verificaParametosEmpresaSms(int cdEscola)
        //{
        //    return DataAccessSms.verificaParametros(cdEscola);
        //}
        
        
        //public IEnumerable<SmsParametrosEscola> getListaEscolaComParametro(int cdEscola)
        //{   
        //    IEnumerable<SmsParametrosEscola> retorno = DataAccessSms.getListaEscolaComParametro(cdEscola);
        //    return retorno;
        //}


        //public SmsParametrosEscola postParamSmsEscola(SmsParametrosEscola smsParametroUi)
        //{
        //    SmsParametrosEscola retorno = new SmsParametrosEscola();
        //    using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
        //    {
        //        retorno = DataAccessSms.add(smsParametroUi, false);
        //        transaction.Complete();
        //    }
        //    return retorno;
        //}

        //public SmsParametrosEscola editParamSmsEscola(SmsParametrosEscola smsParametrosEscola, int cdEscola)
        //{
        //    SmsParametrosEscola retorno = new SmsParametrosEscola();
        //    using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
        //    {
        //        //smsParametrosEscola = DataAccessSms.getListaEscolaComParametro(cdEscola, false);
        //        SmsParametrosEscola atualParametro =  DataAccessSms.GetParametrosEscolaById(smsParametrosEscola);

        //        if (atualParametro != null)
        //        {
        //            atualParametro.senha = smsParametrosEscola.senha;
        //            atualParametro.url_servico = smsParametrosEscola.url_servico;
        //            atualParametro.num_usu = smsParametrosEscola.num_usu;
        //            atualParametro.seu_num = smsParametrosEscola.seu_num;
        //            atualParametro.id_automatico_devedores = smsParametrosEscola.id_automatico_devedores;
        //            atualParametro.id_automatico_aniversario = smsParametrosEscola.id_automatico_aniversario;
        //        }

        //        retorno = DataAccessSms.edit(atualParametro, false);
        //        transaction.Complete();
        //    }

        //    return retorno;
        //}
            

        //public bool deletarParamEscolarSms(int cdEscola)
        //{
        //    bool retorno = false;
        //    using (var transaction =
        //        TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
        //    {
        //        var listaDeletaParamSMS = DataAccessSms.getParamEscolaSms(cdEscola);
               
        //         retorno =   DataAccessSms.delete(listaDeletaParamSMS, false);
                
        //    transaction.Complete();
        //    }
        //    return retorno;
        //}

        //// CRUD PARA COMPOR MENSAGENS PADRAO
        //public SmSComporMensagemPadrao postNovaMensagemPadrao(SmSComporMensagemPadrao smsComporMensagem)
        //{
        //    SmSComporMensagemPadrao retorno = new SmSComporMensagemPadrao();
        //    using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
        //    {
        //        retorno = DataAccessMensagemPadraoSms.add(smsComporMensagem, false);
        //        transaction.Complete();
        //    }
        //    return retorno;
        //}

        //public IEnumerable<SmSComporMensagemPadrao> getListaMensagensPadraobyEscola(int cdEscola)
        //{
        //    IEnumerable<SmSComporMensagemPadrao> retorno =
        //        DataAccessMensagemPadraoSms.getListMensagensPadraoByEscola(cdEscola);

        //    return retorno;
        //}

        //public bool deletaMensagemPadrao(int cd_escola, int motivo)
        //{
        //    bool retorno = false;
        //    using (var transaction =
        //        TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
        //    {
        //        var listaDeletaMensagemSMS = DataAccessMensagemPadraoSms.getParamMensagemSms(cd_escola, motivo);
               
        //        retorno = DataAccessMensagemPadraoSms.delete(listaDeletaMensagemSMS, false);
                
        //        transaction.Complete();
        //    }
        //    return retorno;
        //}


        //public SmSComporMensagemPadrao editMensagemPadraoSms(SmSComporMensagemPadrao smsComporMensagem)
        //{
        //    SmSComporMensagemPadrao retorno = new SmSComporMensagemPadrao();
        //    using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
        //    {
        //        var atualParametro=  DataAccessMensagemPadraoSms.getParamMensagemSmsById(smsComporMensagem);

        //        if (atualParametro != null)
        //        {
        //            atualParametro.cd_escola = smsComporMensagem.cd_escola;
        //            atualParametro.motivo = smsComporMensagem.motivo;
        //            atualParametro.mensagem = smsComporMensagem.mensagem;
        //            atualParametro.dt_ultima_alteracao = smsComporMensagem.dt_ultima_alteracao;
        //        }

        //        retorno = DataAccessMensagemPadraoSms.edit(atualParametro, false);
        //        transaction.Complete();
        //    }

        //    return retorno;
        //}

        // lista aniversariantes por periodo - sms
        //public IEnumerable<PessoaSGF> getListaAniversariosPeriodo(int cdEscola)
        //{
        //    IEnumerable<PessoaSGF> retorno = DataAccessAluno.getListaAniversariantesSms(cdEscola);

        //    return retorno;
        //}


        #endregion

        #region AlunoRestricao

        public IEnumerable<OrgaoFinanceiro> getOrgaoFinanceiro(bool? status)
        {
            return DataAccessOrgaoFinanceiro.getOrgaoFinanceiro(status);
        }

        public IEnumerable<AlunoRestricao> getAlunoRestricaoByCdAluno(int cd_aluno)
        {
            return DataAccessAlunoRestricao.getAlunoRestricaoByCdAluno(cd_aluno);
        }

        #endregion
        #region Motivo Transferencia Aluno
        public IEnumerable<MotivoTransferenciaAluno> GetMotivoTransferenciaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            IEnumerable<MotivoTransferenciaAluno> retorno = new List<MotivoTransferenciaAluno>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "dc_motivo_transferencia_aluno";
                parametros.sort = parametros.sort.Replace("motivo_transferencia_ativo", "id_motivo_transferencia_ativo");

                retorno = DataAccessMotivoTransferencia.GetMotivoTransferenciaSearch(parametros, descricao, inicio, ativo);
                transaction.Complete();
            }
            return retorno;
        }

        public MotivoTransferenciaAluno GetMotivoTransferenciaById(int id)
        {
            return DataAccessMotivoTransferencia.findById(id, false);
        }

        public MotivoTransferenciaAluno PostMotivoTransferencia(MotivoTransferenciaAluno motivotransferencia)
        {
            DataAccessMotivoTransferencia.add(motivotransferencia, false);
            return motivotransferencia;
        }

        public MotivoTransferenciaAluno PutMotivoTransferencia(MotivoTransferenciaAluno motivotransferencia)
        {
            DataAccessMotivoTransferencia.edit(motivotransferencia, false);
            return motivotransferencia;
        }

        public bool DeleteMotivoTransferencia(List<MotivoTransferenciaAluno> motivotransferencia)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = DataAccessMotivoTransferencia.deleteAll(motivotransferencia);
                transaction.Complete();
            }
            return deleted;
        }

        public IEnumerable<MotivoTransferenciaAluno> getMotivosTransferencia()
        {
            return DataAccessMotivoTransferencia.getMotivosTransferencia();
        }

        #endregion

        #region TransferenciaAluno
        public TransferenciaAluno getArquivoHistorico(TransferenciaAluno transferenciaAluno, string parametro)
        {
            Hashtable parametrosPesquisa = new Hashtable();
            Hashtable parametrosRelatorio = new Hashtable();
            string parms = HttpUtility.UrlDecode(parametro, System.Text.Encoding.UTF8).Replace(" ", "+");
            int cdAluno = 0;
            string noAluno = null;
            string produtos = null;
            string statustitulo = null;

            if (parms != null)
            {
                parms = MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                string[] parametrosGet = parms.Split('&');

                for (int i = 0; i < parametrosGet.Length; i++)
                {
                    string[] parametrosHash = parametrosGet[i].Split('=');

                    if (parametrosHash[0].Equals("@cd_aluno"))
                        cdAluno = int.Parse(parametrosHash[1]);
                    if (parametrosHash[0].StartsWith("@"))
                        parametrosPesquisa.Add(parametrosHash[0].Substring(1, parametrosHash[0].Length - 1), parametrosHash[1]);
                    else
                    {
                        if (parametrosHash[0].Equals("no_aluno"))
                        {
                            noAluno = parametrosHash[1];
                        }
                        else
                        {
                            if (parametrosHash[0].StartsWith("Pmostrar"))
                            {
                                parametrosRelatorio.Add(parametrosHash[0], Boolean.Parse(parametrosHash[1]));
                            }
                            else
                            {
                                parametrosRelatorio.Add(parametrosHash[0], parametrosHash[1]);
                            }

                            if (parametrosHash[0].Equals("produtos"))
                            {
                                produtos = parametrosHash[1];
                            }

                            if (parametrosHash[0].Equals("PTitulos"))
                            {
                                statustitulo = parametrosHash[1];
                            }
                        }

                    }
                }
            }

            parametrosRelatorio["PEscola"] = parametrosRelatorio["PEmpresa"];
            parametrosRelatorio.Remove("PEmpresa");
            parametrosRelatorio.Remove("PTipoRelatorio");
            parametrosRelatorio.Remove("produtos");

            parametrosRelatorio.Add("PMostrarSubHTurma", true);
            parametrosRelatorio.Add("PMostrarSubHTurmaAvaliacao", true);
            parametrosRelatorio.Add("PMostrarSubHAvaliacaoTurma", true);
            parametrosRelatorio.Add("PMostrarSubHEstagio", true);
            parametrosRelatorio.Add("PMostrarSubHEstagioAvaliacao", true);
            parametrosRelatorio.Add("PMostrarHSubHEventoAluno", true);
            parametrosRelatorio.Add("PMostrarHSubTitulos", false);
            parametrosRelatorio.Add("PMostrarHSubObs", true);
            parametrosRelatorio.Add("PMostrarHSubAtividades", true);
            parametrosRelatorio.Add("PMostrarHSubFollow", true);
            parametrosRelatorio.Add("PMostrarHSubItem", true);
            parametrosRelatorio.Add("PMostrarSubHTurmaConceito", true);
            parametrosRelatorio.Add("PMostrarSubHEstagioConceito", true);

            //Vai ser possível faze isto por termos somente 1 aluno por vez
            dtHistoricoTurma = getAlunos(cdAluno, 1, produtos, statustitulo);
            setReportParameter(dtHistoricoTurma, "PMostrarSubHTurma", false, parametrosRelatorio);
            dtHistoricoTurmaAvaliacao = getAlunos(cdAluno, 2, produtos, statustitulo);
            setReportParameter(dtHistoricoTurmaAvaliacao, "PMostrarSubHTurmaAvaliacao", false, parametrosRelatorio);
            dtHistoricoAvaliacaoTurma = getAlunos(cdAluno, 3, produtos, statustitulo);
            setReportParameter(dtHistoricoAvaliacaoTurma, "PMostrarSubHAvaliacaoTurma", false, parametrosRelatorio);
            dtHistoricoEstagio = getAlunos(cdAluno, 4, produtos, statustitulo);
            setReportParameter(dtHistoricoEstagio, "PMostrarSubHEstagio", false, parametrosRelatorio);
            dtHistoricoAvaliacaoEstagio = getAlunos(cdAluno, 5, produtos, statustitulo);
            setReportParameter(dtHistoricoAvaliacaoEstagio, "PMostrarSubHEstagioAvaliacao", false, parametrosRelatorio);
            dtHistoricoEventoAula1 = getAlunos(cdAluno, 6, produtos, statustitulo);
            //setReportParameter(dtHistoricoTurma, "PMostrarSubHEventoAluno", false, parametrosRelatorio);
            dtHistoricoEventoAula2 = getAlunos(cdAluno, 7, produtos, statustitulo);
            setReportParameter(dtHistoricoEventoAula2, "PMostrarHSubHEventoAluno", false, parametrosRelatorio);
            dtHistoricoTitulo = getAlunos(cdAluno, 8, produtos, statustitulo);
            setReportParameter(dtHistoricoTitulo, "PMostrarHSubTitulos", false, parametrosRelatorio);
            dtHistoricoObs = getAlunos(cdAluno, 9, produtos, statustitulo);
            setReportParameter(dtHistoricoObs, "PMostrarHSubObs", false, parametrosRelatorio);
            dtHistoricoAtividade = getAlunos(cdAluno, 10, produtos, statustitulo);
            setReportParameter(dtHistoricoAtividade, "PMostrarHSubAtividades", false, parametrosRelatorio);
            dtHistoricoFollow = getAlunos(cdAluno, 11, produtos, statustitulo);
            setReportParameter(dtHistoricoFollow, "PMostrarHSubFollow", false, parametrosRelatorio);
            dtHistoricoItem = getAlunos(cdAluno, 12, produtos, statustitulo);
            setReportParameter(dtHistoricoItem, "PMostrarHSubItem", false, parametrosRelatorio);
            dtHistoricoTurmaConceito = getAlunos(cdAluno, 13, produtos, statustitulo);
            setReportParameter(dtHistoricoTurmaConceito, "PMostrarSubHTurmaConceito", false, parametrosRelatorio);
            dtHistoricoEstagioConceito = getAlunos(cdAluno, 14, produtos, statustitulo);
            setReportParameter(dtHistoricoEstagioConceito, "PMostrarSubHEstagioConceito", false, parametrosRelatorio);


            Hashtable sourceHash = new Hashtable();

            List<sp_RptHistoricoAlunoM_Result> dtReportData = getRtpHistoricoAlunoM(cdAluno);
            List<Componentes.GenericModel.TO> sourceTO = null;
            if (dtReportData.Count() > 0)
            {
                sourceTO = dtReportData.ToList<Componentes.GenericModel.TO>();
            }

            LocalReport AppReportViewer = new LocalReport();
            AppReportViewer.DataSources.Clear();
            AppReportViewer.EnableExternalImages = false;

            var nomeRelatorio = string.Format("{0}{1}.rdlc", ConfigurationManager.AppSettings["caminhoRelatorio"], FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_HISTORICO_ALUNO);
            AppReportViewer.ReportPath = nomeRelatorio;

            foreach (string key in parametrosRelatorio.Keys)
            {
                Microsoft.Reporting.WebForms.ReportParameter reportParameter = new Microsoft.Reporting.WebForms.ReportParameter(key, (parametrosRelatorio[key] != null) ? parametrosRelatorio[key].ToString() : null);
                AppReportViewer.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter[1] { reportParameter });
            }


            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //configurações da página ex: margin, top, left ...
            string deviceInfo =
            "<DeviceInfo>" +
            "<OutputFormat>PDF</OutputFormat>" +
            "</DeviceInfo>";


            Warning[] warnings;
            string[] streams;
            byte[] bytes;

            if (sourceTO != null && sourceTO.ToList<Componentes.GenericModel.TO>().Count > 0)
            {
                try
                {
                    sourceHash["DataHistoricoAluno"] = dtReportData;
                    AppReportViewer.DataSources.Add(new ReportDataSource("DataHistoricoAluno", dtReportData));
                    AppReportViewer.SubreportProcessing += new SubreportProcessingEventHandler(HTurmaSubReportProcessing);
                    AppReportViewer.Refresh();
                    AppReportViewer.DisplayName = noAluno.Replace("/", "-"); //+ "(" + DateTime.Now.ToShortDateString() + ")";
                    bytes = AppReportViewer.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                    string FileName = AppReportViewer.DisplayName + ".pdf";
                    string nomeArquivo = FileName;
                    FileName = FileName.Replace(" ", "");
                    FileName = FileName.Replace(":", "");
                    FileName = Utils.Utils.SubstituiCaracteresEspeciais(FileName);
                    FileName = ConfigurationManager.AppSettings["caminhoUploads"] + "\\TempContratos\\" + FileName;
                    //using (FileStream fs = new FileStream(ConfigurationManager.AppSettings["caminhoContent"] + "\\TempContratos\\testede arquivo nome(grande)" + ".pdf", FileMode.Create))
                    using (FileStream fs = new FileStream(FileName, FileMode.Create))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    var base64Data = "data:application / pdf; base64," + Convert.ToBase64String(bytes);

                    transferenciaAluno.no_arquivo_historico = nomeArquivo;
                    transferenciaAluno.pdf_historico = base64Data;
                    //Vai excluir o arquivo depois
                    File.Delete(FileName);
                    return transferenciaAluno; //File(bytes, mimeType);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                //throw new SecretariaBusinessException("Não existem registros de histórico", null, 0, false);
                return transferenciaAluno;
            }


        }

        public void enviaPromocao(int retCdProspect)
        {
            if (BusinessApiPromocaoIntercambioProspect.aplicaApiPromocao())
            {

                //monta o objeto
                PromocaoIntercambioParams prospectPromocaoIntercambioCurrent = new PromocaoIntercambioParams();
                prospectPromocaoIntercambioCurrent = DataAccessProspect.findProspectApiPromocaoIntercambio(retCdProspect);

                BusinessApiPromocaoIntercambioProspect.ValidaParametros(prospectPromocaoIntercambioCurrent);
                string codigo_promocao = BusinessApiPromocaoIntercambioProspect.postExecutaRequestPromocaoIntercambio(prospectPromocaoIntercambioCurrent);

                if (!string.IsNullOrEmpty(codigo_promocao))
                {
                    using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessPessoaPromocao.DB()))
                    {
                        PessoaPromocao pessoaPromocao = new PessoaPromocao();
                        pessoaPromocao.cd_pessoa = retCdProspect;
                        pessoaPromocao.id_tipo_pessoa = 2;
                        pessoaPromocao.dc_promocao = codigo_promocao;
                        DataAccessPessoaPromocao.add(pessoaPromocao, false);
                        transaction.Complete();
                    }
                    
                }



            }

        }

        public void addPessoaPromocao(PessoaPromocao pessoaPromocao)
        {
            DataAccessPessoaPromocao.add(pessoaPromocao, false);
        }



        private void setReportParameter(DataTable dt, string name, Boolean value, Hashtable parametrosRelatorio)
        {
            if (dt.Rows.Count == 0)
            {
                parametrosRelatorio[name] = value;
            }
        }

        private void HTurmaSubReportProcessing(object sender, SubreportProcessingEventArgs e)
        {

            int cd_atividade = int.Parse(e.Parameters["cd_aluno"].Values[0].ToString());
            if (e.ReportPath.Contains("HistoricoTurmas"))
            {
                DataTable dtRptSub = dtHistoricoTurma; // GetAlunos(cd_atividade, 1);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistorico", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoTurmaAvaliacao"))
            {
                DataTable dtRptSub = dtHistoricoTurmaAvaliacao; //GetAlunos(cd_atividade, 2);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataTurmaAvaliacao", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoAvaliacaoTurma"))
            {
                DataTable dtRptSub = dtHistoricoAvaliacaoTurma; // GetAlunos(cd_atividade, 3);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataAvaliacaoTurma", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath == "HistoricoEstagio")
            {
                DataTable dtRptSub = dtHistoricoEstagio; // GetAlunos(cd_atividade, 4);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataEstagio", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoAvaliacaoEstagio"))
            {
                DataTable dtRptSub = dtHistoricoAvaliacaoEstagio; // GetAlunos(cd_atividade, 5);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataAvaliacaoEstagio", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoEventoAula"))
            {
                DataTable dtRptSub = dtHistoricoEventoAula1; // GetAlunos(cd_atividade, 6);
                Microsoft.Reporting.WebForms.ReportDataSource ds6 = new Microsoft.Reporting.WebForms.ReportDataSource("DataAlunoEvento", dtRptSub);
                e.DataSources.Add(ds6);
                DataTable dtRptSub7 = dtHistoricoEventoAula2; // GetAlunos(cd_atividade, 7);
                Microsoft.Reporting.WebForms.ReportDataSource ds7 = new Microsoft.Reporting.WebForms.ReportDataSource("DataAlunoAula", dtRptSub7);
                e.DataSources.Add(ds7);
            }
            else if (e.ReportPath.Contains("HistoricoTitulo"))
            {
                DataTable dtRptSub = dtHistoricoTitulo; // GetAlunos(cd_atividade, 8);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoTitulo", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoObs"))
            {
                DataTable dtRptSub = dtHistoricoObs; // GetAlunos(cd_atividade, 9);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoObs", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoAtividade"))
            {
                DataTable dtRptSub = dtHistoricoAtividade; // GetAlunos(cd_atividade, 10);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoAtividade", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoFollow"))
            {
                DataTable dtRptSub = dtHistoricoFollow; // GetAlunos(cd_atividade, 11);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoFollow", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoItem"))
            {
                DataTable dtRptSub = dtHistoricoItem; // GetAlunos(cd_atividade, 12);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoItem", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoTurmaConceito"))
            {
                DataTable dtRptSub = dtHistoricoTurmaConceito; // GetAlunos(cd_atividade, 13);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoTurmaConceito", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoEstagioConceito"))
            {
                DataTable dtRptSub = dtHistoricoEstagioConceito; // GetAlunos(cd_atividade, 14);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoEstagioConceito", dtRptSub);
                e.DataSources.Add(ds);
            }
        }

        public IEnumerable<TransferenciaAlunoUI> getEnviarTransferenciaAlunoSearch(SearchParameters parametros, int cd_escola_logada, int? cd_unidade_destino, int cd_aluno, string nm_raf, string cpf, int status_transferencia, DateTime? dataIni, DateTime? dataFim)
        {
            IEnumerable<TransferenciaAlunoUI> retorno = new List<TransferenciaAlunoUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "cd_aluno_origem";
                //parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                retorno = DataAccessTransferenciaAluno.getEnviarTransferenciaAlunoSearch(parametros, cd_escola_logada, cd_unidade_destino, cd_aluno, nm_raf, cpf, status_transferencia,  dataIni, dataFim);
                transaction.Complete();
            }
            return retorno;
        }

        public EnviarTransferenciaComponentesCadParams getComponentesEnviarTransferenciaCad(int cdEscola)
        {
            return DataAccessTransferenciaAluno.getComponentesEnviarTransferenciaCad(cdEscola);
        }

        public string getEmailUnidade(int cdEscola)
        {
            return DataAccessTransferenciaAluno.getEmailUnidade(cdEscola);
        }

        public string getRafByAluno(int cdAluno)
        {
            return DataAccessTransferenciaAluno.getRafByAluno(cdAluno);
        }

        public TransferenciaAluno postInsertEnviarTransferenciaAluno(TransferenciaAluno transferenciaAlunoView)
        {
            TransferenciaAluno retorno;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessTransferenciaAluno.DB()))
            {
                retorno = DataAccessTransferenciaAluno.add(transferenciaAlunoView, false);
                transaction.Complete();
            }

            return DataAccessTransferenciaAluno.getTransferenciaAlunoByCodForGrid(retorno.cd_transferencia_aluno);
        }

        public TransferenciaAluno getTransferenciaAlunoByCodForGrid(int cd_transferencia_aluno)
        {
            return DataAccessTransferenciaAluno.getTransferenciaAlunoByCodForGrid(cd_transferencia_aluno);
        }

        public TransferenciaAluno getTransferenciaAlunoById(int cd_transferencia_aluno)
        {
            return DataAccessTransferenciaAluno.findById(cd_transferencia_aluno, false);
        }
        
        public TransferenciaAluno getEnviarTransferenciaAlunoForEdit(int cd_transferencia_aluno)
        {
            return DataAccessTransferenciaAluno.getEnviarTransferenciaAlunoForEdit(cd_transferencia_aluno);
        }

        public TransferenciaAluno postEditEnviarTransferenciaAluno(TransferenciaAluno transferenciaAlunoView)
        {
            TransferenciaAluno retorno = null;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessTransferenciaAluno.DB()))
            {
                TransferenciaAluno transferenciaAlunoBd = DataAccessTransferenciaAluno.findById(transferenciaAlunoView.cd_transferencia_aluno, false);
                retorno = TransferenciaAluno.changeValueTransferenciaAluno(transferenciaAlunoBd, transferenciaAlunoView);
                DataAccessAluno.saveChanges(false);
                transaction.Complete();
            }

            return DataAccessTransferenciaAluno.getTransferenciaAlunoByCodForGrid(retorno.cd_transferencia_aluno);
        }

        public bool deletarTransferenciaAlunos(List<TransferenciaAluno> transferenciaAluno)
        {
            bool retorno = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessTransferenciaAluno.DB()))
            {
                if (transferenciaAluno != null && transferenciaAluno.Count() > 0)
                    foreach (var transferencia in transferenciaAluno)
                    {
                        var transferenciaAlunoBd = DataAccessTransferenciaAluno.findById(transferencia.cd_transferencia_aluno, false);
                        if (transferenciaAlunoBd != null)
                        {

                            DataAccessTransferenciaAluno.delete(transferenciaAlunoBd, false);
                            retorno = true;
                        }
                    }
                transaction.Complete();
            }

            return retorno;
        }

        public List<string> sendEmailSolicitaTransferenciaAluno(TransferenciaAluno transferenciaAluno)
        {
            List<String> retorno = new List<string>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                SendEmail sendEmail = new SendEmail();
                sendEmail.assunto = " Solicitação de Transferência de Aluno";
                SendEmail.configurarEmailSection(sendEmail);
                TransferenciaAluno transferenciaAlunoBd = DataAccessTransferenciaAluno.findById(transferenciaAluno.cd_transferencia_aluno, false);
                TransferenciaAluno transferenciaAlunoWithFieldsPartial = DataAccessTransferenciaAluno.getTransferenciaAlunoByCodForGrid(transferenciaAluno.cd_transferencia_aluno);
                if (transferenciaAlunoBd == null )
                {
                    throw new SecretariaBusinessException(Messages.msgErroTransferenciaAlunoNuloEnviarEmail, null,
                        SecretariaBusinessException.TipoErro.ERRO_TRANSFERENCIA_ALUNO_ENVIAR_EMAIL_NULO, false);
                }

                if (transferenciaAluno.id_status_transferencia != (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.CADASTRADA &&
                    transferenciaAluno.id_status_transferencia != (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.SOLICITADA )
                {
                    throw new SecretariaBusinessException(Messages.msgErroEmailSolicitacaoStatusDiferenteCriadaSolicitada, null,
                        SecretariaBusinessException.TipoErro.ERRO_ENVIAR_EMAIL_SOLICITACAO_STATUS_DIFERENTE_CRIADA_SOLICITADA, false);
                }

                sendEmail.destinatario = transferenciaAlunoWithFieldsPartial.dc_email_destino;

                /*“Solicitamos autorização para transferirmos o aluno   xxxxxxxxxxxx desta unidade, Fisk ORIGEM para a unidade Fisk DESTINO. Favor enviar e-mail confirmando esta solicitação feita em DD/MM/YYYY”(Data de solicitação)*/

                StringBuilder mensagem = new StringBuilder();
                mensagem.Append("<meta content=\"text/html; charset=windows-1252\" http-equiv=\"Content-Type\" />");
                mensagem.Append("<meta content=\"Microsoft Word 15 (filtered)\" name=\"Generator\" />");
                mensagem.Append("<title>Fisk Centro de Ensino</title>");
                mensagem.Append("<style></style>");
                mensagem.Append("<div class=\"WordSection1\">");
                mensagem.Append("<p class=\"MsoNormal\"><br /></p><p class=\"MsoNormal\">Solicitamos autorização para transferirmos o(a) aluno(a) #nomealuno# desta unidade, #nomeunidadeorigem# para a unidade #nomeunidadedestino#. Favor enviar e-mail confirmando esta solicitação feita em #datasolicitacao# . </p>"
                    .Replace("#nomealuno#", transferenciaAlunoWithFieldsPartial.no_aluno)
                    .Replace("#nomeunidadeorigem#", transferenciaAlunoWithFieldsPartial.no_unidade_origem)
                    .Replace("#nomeunidadedestino#", transferenciaAlunoWithFieldsPartial.no_unidade_destino)
                    .Replace("#datasolicitacao#", String.Format("{0:dd/MM/yyyy}", DateTime.Now))
                );

                mensagem.Append("<p class=\"MsoNormal\"><br /></p>");
                mensagem.Append("<p class=\"MsoNormal\"><br /></p>");
                mensagem.Append("</div>");

                sendEmail.mensagem = mensagem.ToString();
                bool enviado = SendEmail.EnviarEmail(sendEmail);
                if (!enviado)
                {
                    retorno.Add(string.Format(Messages.msgErroEnvioEmailTransferenciaAluno, transferenciaAlunoWithFieldsPartial.no_aluno, transferenciaAlunoWithFieldsPartial.dc_email_destino, transferenciaAlunoWithFieldsPartial.no_unidade_destino));
                }
                else
                {
                    transferenciaAlunoBd.dt_solicitacao_transferencia = DateTime.Now;
                    transferenciaAlunoBd.id_email_origem = true;
                    transferenciaAlunoBd.id_status_transferencia = (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.SOLICITADA;
                    DataAccessTransferenciaAluno.saveChanges(false);
                }

                transaction.Complete();
                return retorno;

            }
        }

        public List<string> transferirAluno(TransferenciaAluno transferenciaAluno, int cd_usuario)
        {
            List<String> retorno = new List<string>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                if (transferenciaAluno == null)
                {
                    throw new SecretariaBusinessException(Messages.msgErroTransferenciaAlunoNuloEnviarEmail, null,
                        SecretariaBusinessException.TipoErro.ERRO_TRANSFERENCIA_ALUNO_ENVIAR_EMAIL_NULO, false);
                }

                TransferenciaAluno transferenciaAlunoBd = DataAccessTransferenciaAluno.findById(transferenciaAluno.cd_transferencia_aluno, false);
                TransferenciaAluno transferenciaAlunoWithFieldsPartial = DataAccessTransferenciaAluno.getTransferenciaAlunoByCodForGrid(transferenciaAluno.cd_transferencia_aluno);

                if (transferenciaAluno.id_status_transferencia != (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.APROVADA)
                {
                    throw new SecretariaBusinessException(Messages.msgErroTransferiAlunoStatusDiferenteAprovado, null,
                        SecretariaBusinessException.TipoErro.ERRO_TRANSFERIR_ALUNO_STATUS_DIFERENTE_APROVADO, false);
                }
                Aluno alunoBdA = DataAccessAluno.getAlunoById(transferenciaAlunoBd.cd_aluno_origem);
                var cpf = string.IsNullOrEmpty(alunoBdA.AlunoPessoaFisica.nm_cpf) ? alunoBdA.AlunoPessoaFisica.PessoaSGFQueUsoOCpf.nm_cpf : alunoBdA.AlunoPessoaFisica.nm_cpf;
                var nome = alunoBdA.AlunoPessoaFisica.cd_pessoa_cpf == null ? null : alunoBdA.AlunoPessoaFisica.PessoaSGFQueUsoOCpf.no_pessoa;
                /*Aluno alunoDest = DataAccessAluno.getAlunoByCpf(cpf, null, nome, (int)(alunoBdA.AlunoPessoaFisica.cd_pessoa_cpf == null ? 0: alunoBdA.AlunoPessoaFisica.cd_pessoa_cpf), transferenciaAlunoBd.cd_escola_destino);
                if(alunoDest != null && alunoDest.cd_aluno > 0)
                    throw new SecretariaBusinessException(Messages.msgErroTransferenciaAlunoDestino, null,
                        SecretariaBusinessException.TipoErro.ERRO_TRANSFERENCIA_ALUNO_EXISTE_DESTINO, false);*/
                //será atribuído status de Transferido em todas as turmas que o Aluno escolhido se encontra ativo
                List<AlunoTurma> alunoTurmaList = DataAccessTransferenciaAluno.getAlunoTurmasBdTransferenciaAluno(transferenciaAlunoBd.cd_aluno_origem, transferenciaAlunoBd.cd_escola_origem);

                if (alunoTurmaList != null && alunoTurmaList.Count > 0)
                {
                    foreach (AlunoTurma alunoTurma in alunoTurmaList)
                    {
                        //muda o status no aluno turma
                        AlunoTurma alunoTurmaBd = DataAccessTransferenciaAluno.getAlunoTurmaBdEditTransferenciaAluno(transferenciaAlunoBd.cd_escola_origem, alunoTurma.cd_aluno_turma);
                        alunoTurmaBd.cd_situacao_aluno_turma = (int)AlunoTurma.SituacaoAlunoTurma.Transferido;
                        DataAccessTransferenciaAluno.saveChanges(false);

                        //gera o historico de transferencia
                        int cdContrato = (int)alunoTurmaBd.cd_contrato;
                        int cdProduto = BusinessAluno.findAlunoTurmaProduto(alunoTurmaBd.cd_aluno, alunoTurmaBd.cd_turma, transferenciaAlunoBd.cd_escola_origem);
                        byte segHistorico = (byte)DataAccessHistoricoAluno.retunMaxSequenciaHistoricoAluno(cdProduto, transferenciaAlunoBd.cd_escola_origem, alunoTurmaBd.cd_aluno);

                        Contrato contrato = DataAccessMatricula.findMatriculaByTurmaAluno(alunoTurmaBd.cd_turma, alunoTurmaBd.cd_aluno);


                        HistoricoAluno historico = new HistoricoAluno
                        {
                            cd_aluno = alunoTurmaBd.cd_aluno,
                            cd_turma = alunoTurmaBd.cd_turma,
                            cd_contrato = cdContrato,
                            id_tipo_movimento = (byte)HistoricoAluno.TipoMovimento.TRANSFERENCIA,
                            dt_cadastro = DateTime.UtcNow,
                            dt_historico = DateTime.UtcNow,
                            cd_produto = contrato.cd_produto_atual,
                            nm_sequencia = ++segHistorico,
                            id_situacao_historico = (byte)AlunoTurma.SituacaoAlunoTurma.Transferido,
                            cd_usuario = cd_usuario
                            
                        };

                        addHistoricoAluno(historico);

                    }
                }



                //desabilitar Raf
                PessoaRaf pessoaRafBd = DataAccessTransferenciaAluno.getRafBdEditByAluno(transferenciaAlunoBd.cd_aluno_origem);
                pessoaRafBd.id_raf_liberado = false;
                DataAccessTransferenciaAluno.saveChanges(false);

                
                //gravar um aluno(t_aluno) na escola de destino
                Aluno alunoBd = DataAccessAluno.findById(transferenciaAlunoBd.cd_aluno_origem, false);
                Aluno alunoEscolaDestino = DataAccessAluno.findAlunoByCdPessoaAlunoAndCdEscolaDestino(alunoBd.cd_pessoa_aluno, transferenciaAlunoBd.cd_escola_destino);

                Aluno alunoSave = null;
                //Caso esteja retornando a escola de origem com outra transferência e o aluno já exista na escola de destino
                if (alunoEscolaDestino != null)
                {
                    alunoSave = alunoEscolaDestino;
                }
                else
                {
                    Aluno newAlunoEscolaDestino = new Aluno();
                    newAlunoEscolaDestino.cd_pessoa_aluno = alunoBd.cd_pessoa_aluno;
                    newAlunoEscolaDestino.cd_pessoa_escola = transferenciaAlunoBd.cd_escola_destino;
                    newAlunoEscolaDestino.cd_midia = alunoBd.cd_midia;
                    newAlunoEscolaDestino.cd_escolaridade = alunoBd.cd_escolaridade;
                    newAlunoEscolaDestino.cd_usuario_atendente = alunoBd.cd_usuario_atendente;
                    newAlunoEscolaDestino.id_aluno_ativo = alunoBd.id_aluno_ativo;
                    newAlunoEscolaDestino.cd_prospect = alunoBd.cd_prospect;

                     alunoSave = DataAccessAluno.add(newAlunoEscolaDestino, false);
                }

                    
                //DataAccessAluno.saveChanges(false);

                SendEmail sendEmail = new SendEmail();
                sendEmail.assunto = " Transferência de Aluno";
                SendEmail.configurarEmailSection(sendEmail);
                


                sendEmail.destinatario = transferenciaAlunoWithFieldsPartial.dc_email_destino;

                /*“Solicitamos autorização para transferirmos o aluno   xxxxxxxxxxxx desta unidade, Fisk ORIGEM para a unidade Fisk DESTINO. Favor enviar e-mail confirmando esta solicitação feita em DD/MM/YYYY”(Data de solicitação)*/

                StringBuilder mensagem = new StringBuilder();
                mensagem.Append("<meta content=\"text/html; charset=windows-1252\" http-equiv=\"Content-Type\" />");
                mensagem.Append("<meta content=\"Microsoft Word 15 (filtered)\" name=\"Generator\" />");
                mensagem.Append("<title>Fisk Centro de Ensino</title>");
                mensagem.Append("<style></style>");
                mensagem.Append("<div class=\"WordSection1\">");
                mensagem.Append("<p class=\"MsoNormal\"><br /></p><p class=\"MsoNormal\">Comunicamos que efetuamos transferência do(a) aluno(a) #nomealuno# desta unidade #nomeunidadeorigem# para sua unidade, #nomeunidadedestino#. Código RAF do(a) aluno(a) transferido(a) #codigorafaluno#. Data da Transferencia: #datasolicitacao# . </p>"
                    .Replace("#nomealuno#", transferenciaAlunoWithFieldsPartial.no_aluno)
                    .Replace("#nomeunidadeorigem#", transferenciaAlunoWithFieldsPartial.no_unidade_origem)
                    .Replace("#nomeunidadedestino#", transferenciaAlunoWithFieldsPartial.no_unidade_destino)
                    .Replace("#codigorafaluno#", transferenciaAlunoWithFieldsPartial.nm_raf)
                    .Replace("#datasolicitacao#", String.Format("{0:dd/MM/yyyy}", DateTime.Now))
                );

                if (
                    (!String.IsNullOrEmpty(transferenciaAlunoBd.pdf_historico) && !String.IsNullOrEmpty(transferenciaAlunoBd.no_arquivo_historico)) ||
                    (!String.IsNullOrEmpty(transferenciaAluno.pdf_historico) && !String.IsNullOrEmpty(transferenciaAluno.no_arquivo_historico))
                   )
                {
                    mensagem.Append("<p class=\"MsoNormal\"><br /></p><p class=\"MsoNormal\">Em anexo o histórico do(a) aluno(a) na nossa unidade.<br /></p>");
                }

                mensagem.Append("<p class=\"MsoNormal\"><br /></p>");
                mensagem.Append("<p class=\"MsoNormal\"><br /></p>");
                mensagem.Append("</div>");

                if (!String.IsNullOrEmpty(transferenciaAlunoBd.pdf_historico) && !String.IsNullOrEmpty(transferenciaAlunoBd.no_arquivo_historico))
                {
                    string tipo = transferenciaAlunoBd.pdf_historico.Substring(transferenciaAlunoBd.pdf_historico.IndexOf(",") + 1);
                    Byte[] buffer = Convert.FromBase64String(tipo);
                    Dictionary<string, Stream> anexos = new Dictionary<string, Stream>();
                    anexos.Add(transferenciaAlunoBd.no_arquivo_historico, new MemoryStream(buffer));
                    sendEmail.Anexos = anexos;
                }
                else
                if (!String.IsNullOrEmpty(transferenciaAluno.pdf_historico) && !String.IsNullOrEmpty(transferenciaAluno.no_arquivo_historico))
                {
                    string tipo = transferenciaAluno.pdf_historico.Substring(transferenciaAluno.pdf_historico.IndexOf(",") + 1);
                    Byte[] buffer = Convert.FromBase64String(tipo);
                    Dictionary<string, Stream> anexos = new Dictionary<string, Stream>();
                    anexos.Add(transferenciaAluno.no_arquivo_historico, new MemoryStream(buffer));
                    sendEmail.Anexos = anexos;
                }

                sendEmail.mensagem = mensagem.ToString();
                bool enviado = SendEmail.EnviarEmail(sendEmail);
                if (!enviado)
                {
                    throw new SecretariaBusinessException(string.Format(Messages.msgErroEnvioEmailTransferenciaAluno, transferenciaAlunoWithFieldsPartial.no_aluno, transferenciaAlunoWithFieldsPartial.dc_email_destino, transferenciaAlunoWithFieldsPartial.no_unidade_destino), null,
                        SecretariaBusinessException.TipoErro.ERRO_TRANSFERENCIA_ALUNO_ENVIAR_EMAIL_NULO, false);
                }
                else
                {
                    transferenciaAlunoBd = DataAccessTransferenciaAluno.findById(transferenciaAluno.cd_transferencia_aluno, false);
                    int cd_aluno_save = alunoSave.cd_aluno;
                    transferenciaAlunoBd.dt_transferencia = DateTime.Now;
                    transferenciaAlunoBd.id_email_origem = true;
                    transferenciaAlunoBd.cd_aluno_destino = cd_aluno_save;
                    transferenciaAlunoBd.id_status_transferencia = (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.EFETUADA;
                    if (transferenciaAluno.pdf_historico != null)
                    {
                        transferenciaAlunoBd.no_arquivo_historico = transferenciaAluno.no_arquivo_historico;
                        transferenciaAlunoBd.pdf_historico = transferenciaAluno.pdf_historico;
                    }
                    DataAccessTransferenciaAluno.saveChanges(false);
                }





                transaction.Complete();
                return retorno;

            }
        }



        #endregion

        #region Receber Transferencia

        public IEnumerable<TransferenciaAlunoUI> getReceberTransferenciaAlunoSearch(SearchParameters parametros, int cdEscola, int? cdUnidadeOrigem, string noAluno, string nmRaf, string cpf, int statusTransferencia, DateTime? dtInicial, DateTime? dtFinal)
        {
            IEnumerable<TransferenciaAlunoUI> retorno = new List<TransferenciaAlunoUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "cd_aluno_origem";
                //parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                retorno = DataAccessTransferenciaAluno.getReceberTransferenciaAlunoSearch(parametros, cdEscola, cdUnidadeOrigem, noAluno, nmRaf, cpf, statusTransferencia, dtInicial, dtFinal);
                transaction.Complete();
            }
            return retorno;
        }

        public TransferenciaAluno postEditReceberTransferenciaAluno(TransferenciaAluno transferenciaAlunoView)
        {
            TransferenciaAluno retorno = null;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessTransferenciaAluno.DB()))
            {
                TransferenciaAluno transferenciaAlunoBd = DataAccessTransferenciaAluno.findById(transferenciaAlunoView.cd_transferencia_aluno, false);
                retorno = TransferenciaAluno.changeValueTransferenciaAlunoReceber(transferenciaAlunoBd, transferenciaAlunoView);
                DataAccessAluno.saveChanges(false);
                transaction.Complete();
            }

            return DataAccessTransferenciaAluno.getTransferenciaAlunoByCodForGrid(retorno.cd_transferencia_aluno);
        }

        public List<string> sendEmailAprovarRecusarTransferenciaAluno(TransferenciaAluno transferenciaAluno)
        {
            List<String> retorno = new List<string>();
            using (var transaction =
                TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                TransferenciaAluno transferenciaAlunoBd = DataAccessTransferenciaAluno.findById(transferenciaAluno.cd_transferencia_aluno, false);
                TransferenciaAluno transferenciaAlunoWithFieldsPartial = DataAccessTransferenciaAluno.getTransferenciaAlunoByCodForGrid(transferenciaAluno.cd_transferencia_aluno);
                if (transferenciaAlunoBd == null)
                {
                    throw new SecretariaBusinessException(Messages.msgErroTransferenciaAlunoNuloEnviarEmail, null,
                        SecretariaBusinessException.TipoErro.ERRO_TRANSFERENCIA_ALUNO_ENVIAR_EMAIL_NULO, false);
                }

                if (transferenciaAlunoBd.id_status_transferencia != (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.APROVADA &&
                    transferenciaAlunoBd.id_status_transferencia != (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.RECUSADA)
                {
                    throw new SecretariaBusinessException(Messages.msgErroEnvioEmailStatusDiferenteAprovadoRecusado, null,
                        SecretariaBusinessException.TipoErro.ERRO_ENVIO_EMAIL_STATUS_DIRERENTE_APROVADO_RECUSADO, false);
                }

                SendEmail sendEmail = new SendEmail();
                if (transferenciaAlunoBd.id_status_transferencia == (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.APROVADA)
                {
                    sendEmail.assunto = " Confirmação de Transferência de Aluno";
                }else if (transferenciaAlunoBd.id_status_transferencia == (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.RECUSADA)
                {
                    sendEmail.assunto = " Recusa de Transferência de Aluno";
                }

                SendEmail.configurarEmailSection(sendEmail);
                

                sendEmail.destinatario = transferenciaAlunoWithFieldsPartial.dc_email_origem;

                /*“Solicitamos autorização para transferirmos o aluno   xxxxxxxxxxxx desta unidade, Fisk ORIGEM para a unidade Fisk DESTINO. Favor enviar e-mail confirmando esta solicitação feita em DD/MM/YYYY”(Data de solicitação)*/

                StringBuilder mensagem = new StringBuilder();
                mensagem.Append("<meta content=\"text/html; charset=windows-1252\" http-equiv=\"Content-Type\" />");
                mensagem.Append("<meta content=\"Microsoft Word 15 (filtered)\" name=\"Generator\" />");
                mensagem.Append("<title>Fisk Centro de Ensino</title>");
                mensagem.Append("<style></style>");
                mensagem.Append("<div class=\"WordSection1\">");
                if (transferenciaAlunoBd.id_status_transferencia == (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.APROVADA)
                {
                    mensagem.Append("<p class=\"MsoNormal\"><br /></p><p class=\"MsoNormal\">Confirmamos a transferência do aluno #nomealuno# para nossa unidade #nomeunidadedestino# efetuada através da unidade  #nomeunidadeorigem#. </p>"
                        .Replace("#nomealuno#", transferenciaAlunoWithFieldsPartial.no_aluno)
                        .Replace("#nomeunidadeorigem#", transferenciaAlunoWithFieldsPartial.no_unidade_origem)
                        .Replace("#nomeunidadedestino#", transferenciaAlunoWithFieldsPartial.no_unidade_destino)
                    );
                }
                else if (transferenciaAlunoBd.id_status_transferencia == (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.RECUSADA)
                {
                    mensagem.Append("<p class=\"MsoNormal\"><br /></p><p class=\"MsoNormal\">Recusamos a transferência do aluno #nomealuno# para nossa unidade #nomeunidadedestino# solicitada através da unidade  #nomeunidadeorigem#. </p>"
                        .Replace("#nomealuno#", transferenciaAlunoWithFieldsPartial.no_aluno)
                        .Replace("#nomeunidadeorigem#", transferenciaAlunoWithFieldsPartial.no_unidade_origem)
                        .Replace("#nomeunidadedestino#", transferenciaAlunoWithFieldsPartial.no_unidade_destino)
                    );
                }
                

                mensagem.Append("<p class=\"MsoNormal\"><br /></p>");
                mensagem.Append("<p class=\"MsoNormal\"><br /></p>");
                mensagem.Append("</div>");

                sendEmail.mensagem = mensagem.ToString();
                bool enviado = SendEmail.EnviarEmail(sendEmail);
                if (!enviado)
                {
                    retorno.Add(string.Format(Messages.msgErroEnvioEmailTransferenciaAluno, transferenciaAlunoWithFieldsPartial.no_aluno, transferenciaAlunoWithFieldsPartial.dc_email_origem, transferenciaAlunoWithFieldsPartial.no_unidade_origem));
                }
                else
                {
                    

                    transferenciaAlunoBd.id_email_destino = true;
                    if (transferenciaAlunoBd.id_status_transferencia == (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.APROVADA)
                    {
                        transferenciaAlunoBd.dt_confirmacao_transferencia = DateTime.Now;
                        transferenciaAlunoBd.id_status_transferencia = (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.APROVADA;
                    }
                    else if (transferenciaAluno.id_status_transferencia == (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.RECUSADA)
                    {
                        transferenciaAlunoBd.dt_confirmacao_transferencia = DateTime.Now;
                        transferenciaAlunoBd.id_status_transferencia = (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.RECUSADA;
                    }
                    
                    DataAccessTransferenciaAluno.saveChanges(false);
                }

                transaction.Complete();
                return retorno;

            }
        }

        #endregion



    }
}
