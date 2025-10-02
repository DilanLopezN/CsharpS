using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using System.Transactions;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Data.Entity.Infrastructure;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Business
{
    using FundacaoFisk.SGF.GenericModel;
    using System.Data;

    public class AlunoBusiness : IAlunoBusiness
    {
        public IAlunoDataAccess DataAccessAluno { get; set; }
        public IAlunoTurmaDataAccess dataAccessAlunoTurma { get; set; }
        private ILocalidadeBusiness BusinessLoc { get; set; }
        private IPessoaBusiness BusinessPessoa { get; set; }
        private IHorarioDataAccess DataAccessHorario { get; set; }
        private IAlunoMotivoMatriculaDataAccess DataAccessAlunoMotivoMatricula { get; set; }
        private IAlunoBolsaDataAccess DataAccessAlunoBolsa { get; set; }
        public IEmpresaBusiness BusinessEmpresa { get; set; }
        public IHistoricoAlunoDataAccess DataAccessHistoricoAluno { get; set; }
        public IFiscalBusiness BusinessFiscal { get; set; }
        public IFinanceiroBusiness BusinessFinanceiro { get; set; }
        public IMatriculaDataAccess DataAccessMatricula { get; set; }
        public IProspectDataAccess DataAccessProspect { get; set; }
        public IPessoaRafDataAccess DataAccessPessoaRaf { get; set; }
        public IApiNewCyberAlunoBusiness BusinessApiNewCyberAluno { get; set; }
        public IFichaSaudeDataAccess DataAccessFichaSaude { get; set; }
        const int ADD = 1; //int EDIT = 2;
        const int TELEFONE = 1; //int EMAIL = 4; int SITE = 5; int CELULAR = 3;
        const int COMERCIAL = 1;
        const string SUCESSO = "sucesso";

        public AlunoBusiness(IAlunoDataAccess dataAccessAluno, ILocalidadeBusiness businessLoc, IPessoaBusiness businessPessoa,
            IAlunoMotivoMatriculaDataAccess dataAccessAlunoMotivoMatricula,
            IHorarioDataAccess dataAccessHorario, IAlunoTurmaDataAccess DataAccessAlunoTurma,
            IAlunoBolsaDataAccess dataAccessAlunoBolsa, IHistoricoAlunoDataAccess dataAccessHistoricoAluno,
            IEmpresaBusiness businessEmpresa, IFiscalBusiness businessFiscal, IFinanceiroBusiness businessFinanceiro, IMatriculaDataAccess dataAccessMatricula, IProspectDataAccess dataAccessProspect,
            IPessoaRafDataAccess dataAccessPessoaRaf, IApiNewCyberAlunoBusiness businessApiNewCyberAluno, IFichaSaudeDataAccess dataAccessFichaSaude)
        {
            if ((dataAccessAluno == null) || (businessLoc == null) || (businessPessoa == null) 
                || dataAccessAlunoMotivoMatricula == null || dataAccessHorario == null || DataAccessAlunoTurma == null ||
                dataAccessAlunoBolsa == null || dataAccessHistoricoAluno == null || (businessEmpresa == null) ||
                businessFiscal == null || businessFinanceiro == null || dataAccessMatricula == null || dataAccessProspect == null ||
                dataAccessPessoaRaf == null || businessApiNewCyberAluno == null || dataAccessFichaSaude == null)
                throw new ArgumentNullException("repository");
            this.DataAccessAluno = dataAccessAluno;
            this.BusinessLoc = businessLoc;
            this.BusinessPessoa = businessPessoa;
            this.DataAccessHorario = dataAccessHorario;
            this.DataAccessAlunoMotivoMatricula = dataAccessAlunoMotivoMatricula;
            this.dataAccessAlunoTurma = DataAccessAlunoTurma;
            this.DataAccessAlunoBolsa = dataAccessAlunoBolsa;
            this.BusinessEmpresa = businessEmpresa;
            this.BusinessFiscal = businessFiscal;
            this.BusinessFinanceiro = businessFinanceiro;
            this.DataAccessMatricula = dataAccessMatricula;
            this.DataAccessHistoricoAluno = dataAccessHistoricoAluno;
            this.DataAccessProspect = dataAccessProspect;
            this.DataAccessPessoaRaf = dataAccessPessoaRaf;
            this.BusinessApiNewCyberAluno = businessApiNewCyberAluno;
            this.DataAccessFichaSaude = dataAccessFichaSaude;
            ;

        }

        public void configuraUsuario(int cdUsuario, int cd_empresa)
        {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.DataAccessAluno.DB()).IdUsuario = 
            ((SGFWebContext) this.DataAccessAlunoBolsa.DB()).IdUsuario = ((SGFWebContext) this.DataAccessAlunoMotivoMatricula.DB()).IdUsuario  =
            ((SGFWebContext)this.dataAccessAlunoTurma.DB()).IdUsuario = ((SGFWebContext)this.DataAccessFichaSaude.DB()).IdUsuario = cdUsuario;
            ((SGFWebContext)this.DataAccessAluno.DB()).cd_empresa = 
            ((SGFWebContext)this.DataAccessAlunoBolsa.DB()).cd_empresa = ((SGFWebContext)this.DataAccessAlunoMotivoMatricula.DB()).cd_empresa =
            ((SGFWebContext)this.dataAccessAlunoTurma.DB()).cd_empresa = ((SGFWebContext)this.DataAccessMatricula.DB()).cd_empresa = cd_empresa =
            ((SGFWebContext)this.DataAccessPessoaRaf.DB()).cd_empresa = cd_empresa = ((SGFWebContext)this.DataAccessProspect.DB()).cd_empresa = ((SGFWebContext)this.DataAccessFichaSaude.DB()).cd_empresa = cd_empresa;
            this.BusinessLoc.configuraUsuario(cdUsuario, cd_empresa);
            this.BusinessPessoa.configuraUsuario(cdUsuario, cd_empresa);
            this.BusinessFiscal.configuraUsuario(cdUsuario, cd_empresa);
            this.BusinessFinanceiro.configuraUsuario(cdUsuario, cd_empresa);
            this.BusinessEmpresa.configuraUsuario(cdUsuario, cd_empresa);
            this.BusinessApiNewCyberAluno.configuraUsuario(cdUsuario, cd_empresa);
        }

        public void sincronizaContexto(DbContext db)
        {
            //this.DataAccessAluno.sincronizaContexto(db);
            //this.dataAccessAlunoTurma.sincronizaContexto(db);
            //this.DataAccessHorario.sincronizaContexto(db);
            //this.DataAccessAlunoMotivoMatricula.sincronizaContexto(db);
            //this.DataAccessAlunoBolsa.sincronizaContexto(db);
            //this.DataAccessHistoricoAluno.sincronizaContexto(db);
            //this.DataAccessMatricula.sincronizaContexto(db);
            //this.DataAccessProspect.sincronizaContexto(db);
            //BusinessPessoa.sincronizaContexto(db);
            //BusinessLoc.sincronizaContexto(db);
            //BusinessEmpresa.sincronizaContexto(db);
            //BusinessFiscal.sincronizarContextos(db);
            //BusinessFinanceiro.sincronizarContextos(db);
        }

        public IEnumerable<AlunoSearchUI> getAlunoSelecionado(int cdPolitica, int cdEscola)
        {
            return DataAccessAluno.getAlunoSelecionado(cdPolitica, cdEscola);
        }

        #region Aluno
        public string getObservacaoAluno(int cdAluno, int cdEscola)
        {
            return DataAccessAluno.getObservacaoAluno(cdAluno, cdEscola);
        }

        public IEnumerable<AlunoSearchUI> getAlunoSearchFollowUp(SearchParameters parametros, string desc, bool inicio, int cdEscola, string email, string telefone, AlunoDataAccess.TipoConsultaAlunoEnum tipo)
        {
            if (parametros.sort == null)
                parametros.sort = "no_pessoa";
            return DataAccessAluno.getAlunoSearchFollowUp(parametros, desc, inicio, cdEscola, email, telefone, tipo);
        }

        public IEnumerable<AlunoSearchUI> findAluno(int cdEscola)
        {
            return DataAccessAluno.findAluno(cdEscola);
        }
        public IEnumerable<AlunoSearchUI> getAlunoPolitica(int cdEscola)
        {
            return DataAccessAluno.getAlunoPolitica(cdEscola);
        }
        public IEnumerable<AlunoSearchUI> getAlunoFKReajusteSearch(SearchParameters parametros, string desc, string email, bool inicio, bool? status, int cdEscola, string cpf, List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, DateTime? dt_inicial, DateTime? dt_final){
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_aluno_ativo");
                parametros.sort = parametros.sort.Replace("nm_cpf_dependente", "nm_cpf");
                retorno = DataAccessAluno.getAlunoFKReajusteSearch(parametros, desc, email, inicio, status, cdEscola, cpf, cdSituacao, sexo, semTurma, movido, tipoAluno, dt_inicial, dt_final);
                transaction.Complete();
            }
            return retorno;
        }
        public IEnumerable<AlunoSearchUI> getAlunoSearch(SearchParameters parametros, string desc, string email, bool inicio, bool? status, int cdEscola, string cpf, List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, bool matriculasem, bool matricula)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessAluno.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_aluno_ativo");
                parametros.sort = parametros.sort.Replace("nm_cpf_dependente", "nm_cpf");
                retorno = DataAccessAluno.getAlunoSearch(parametros, desc, email, inicio, status, cdEscola, cpf, cdSituacao, sexo, semTurma, movido, tipoAluno, matriculasem, matricula).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<AlunoSearchUI> getAlunoSearchFKPesquisas(SearchParameters parametros, string nome, 
            string email, bool inicio, bool? status, int cdEscola, int origemFK, string cpf, List<int> cdSituacao, int sexo, 
            bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoConsultaAlunoEnum tipoConsulta)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_aluno_ativo");
                parametros.sort = parametros.sort.Replace("nm_cpf_dependente", "nm_cpf");
                retorno = DataAccessAluno.getAlunoSearchFKPesquisas(parametros, nome, email, inicio, status, cdEscola, origemFK, cpf, cdSituacao, sexo, semTurma, movido, tipoAluno, cd_pessoa_responsavel, tipoConsulta).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<AlunoSearchUI> getAlunoSearchFKPesquisasAtividade(SearchParameters parametros, string nome,
            string email, bool inicio, bool? status, int cdEscola, int origemFK, string cpf, List<int> cdSituacao, int sexo,
            bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoConsultaAlunoEnum tipoConsulta, int cd_escola_combo_fk, List<int> cds_escolas_atividade)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_aluno_ativo");
                parametros.sort = parametros.sort.Replace("nm_cpf_dependente", "nm_cpf");
                retorno = DataAccessAluno.getAlunoSearchFKPesquisasAtividade(parametros, nome, email, inicio, status, cdEscola, origemFK, cpf, cdSituacao, sexo, semTurma, movido, tipoAluno, cd_pessoa_responsavel, tipoConsulta, cd_escola_combo_fk, cds_escolas_atividade).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<AlunoSearchUI> getAlunoSearchFKPesquisasAtividadeExtra(SearchParameters parametros, string nome,
            string email, bool inicio, bool? status, int cdEscola, string cpf, List<int> cdSituacao, int sexo,
            bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoConsultaAlunoEnum tipoConsulta, int cd_escola_combo_fk, List<int> cursos, List<int> cds_escolas_atividade)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_aluno_ativo");
                parametros.sort = parametros.sort.Replace("nm_cpf_dependente", "nm_cpf");
                retorno = DataAccessAluno.getAlunoSearchFKPesquisasAtividadeExtra(parametros, nome, email, inicio, status, cdEscola, cpf, cdSituacao, sexo, semTurma, movido, tipoAluno, cd_pessoa_responsavel, tipoConsulta, cd_escola_combo_fk, cursos, cds_escolas_atividade).ToList(); 
                transaction.Complete();
            }
            return retorno;
        }
        
        public IEnumerable<AlunoSearchUI> GetAlunoSearchAulaPer(SearchParameters parametros, string desc, string email, bool inicio, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo,
                                                                bool semTurma, bool movido, int tipoAluno, DateTime dtaAula)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_aluno_ativo");
                parametros.sort = parametros.sort.Replace("nm_cpf_dependente", "nm_cpf");
                retorno = DataAccessAluno.GetAlunoSearchAulaPer(parametros, desc, email, inicio, status, cdEscola, cpf, cdSituacao, sexo, semTurma, movido, tipoAluno, dtaAula).ToList();
                transaction.Complete();
            }
            return retorno;
        }
        public IEnumerable<AlunoSearchUI> getAlunoSearchTurma(SearchParameters parametros, string desc, string email, bool inicio, int origemFK, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo)
        {
            // Reajusta os parâmetros de ordenação da pesquisa:
            if (parametros.sort == null)
                parametros.sort = "no_pessoa";
            parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
            parametros.sort = parametros.sort.Replace("id_ativo", "id_aluno_ativo");
            return DataAccessAluno.getAlunoSearchTurma(parametros, desc, email, inicio, origemFK, status, cdEscola, cpf, cdSituacao, sexo);
        }


        public List<int> findAllEmpresasByUsuario(string login, int cdEscola)
        {
            List<int> cds_empresas_usuario = BusinessEmpresa.findAllEmpresasByUsuario(login, cdEscola);
            return cds_empresas_usuario;
        }


        private bool verificaHorarioOcupado(List<Horario> horarios)
        {
            List<Horario> horarioIntervalo = new List<Horario>();
            IEnumerable<Horario> horariosDisponiveis;
            bool retorno = true;

            //Pegando os itens do Calendar2 (Ocupado)
            IEnumerable<Horario> horariosOcupados = horarios.Where(c => c.calendar == "Calendar2");

            //Pegando os itens do Calendar1 (Disponiveis)
            horariosDisponiveis = horarios.Where(c => c.calendar == "Calendar1");


            //Preencher a lista qnd Horario Ocupado Inicial < Horario Disponivel Inicial e Horario Ocupado Final > Horario Disponivel Inicio
            //OU Horario Ocupado Inicial < Horario Disponivel Inicial e Horario Ocupado Final > Horario Disponivel Final
            //Caso a Lista continue com o mesmo tanto de ocupado, é pq os horarios estão corretos
            foreach (Horario h in horariosOcupados)
                retorno &= horariosDisponiveis.Where(hd => h.dt_hora_fim <= hd.dt_hora_fim && hd.dt_hora_ini <= h.dt_hora_ini && hd.id_dia_semana == h.id_dia_semana).Any();

            return retorno | horariosDisponiveis.ToList().Count() == 0;
        }

        [Obsolete]
        public Aluno addAluno(AlunoUI alunoUI)
        {
            PessoaFisicaSGF pessoaFisica = new PessoaFisicaSGF();
            Aluno aluno = new Aluno();
            PessoaEscola pessoaEsc = new PessoaEscola();
            List<RelacionamentoSGF> relacionamentos = null;
            sincronizaContexto(DataAccessAluno.DB());
            try
            {
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    bool escolaInternacional = DataAccessAluno.getParametroEscolaInternacional(alunoUI.aluno.cd_pessoa_escola);

                    if (escolaInternacional == false && string.IsNullOrEmpty(alunoUI.pessoaFisicaUI.pessoaFisica.nm_cpf))
                    {
                        throw new AlunoBusinessException(Utils.Messages.Messages.msgErroCpfNuloEscolaNãoInternacional, null, AlunoBusinessException.TipoErro.ERRO_CPF_NULO_NAO_ITERNACIONAL, false);
                    }

                    if (alunoUI.pessoaFisicaUI != null && !string.IsNullOrEmpty(alunoUI.pessoaFisicaUI.email))
                    {
                        var existAlunoEmailBase = DataAccessAluno.verificarAlunoExistEmail(alunoUI.pessoaFisicaUI.email, alunoUI.aluno.cd_pessoa_escola,0);
                        if (existAlunoEmailBase != null && existAlunoEmailBase.cd_pessoa > 0)
                            throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgExistsEmailPessoa, existAlunoEmailBase.no_pessoa), null, AlunoBusinessException.TipoErro.ERRO_EMAIL_JA_EXITE, false);
                    }
                    if (alunoUI.pessoaFisicaUI != null && alunoUI.pessoaFisicaUI.pessoaFisica != null && alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa > 0 &&
                        (!string.IsNullOrEmpty(alunoUI.pessoaFisicaUI.pessoaFisica.nm_cpf) || alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf > 0))
                    {
                        //pessoaFisica.copy((PessoaFisicaSGF) DataAccessPessoaSGF.findById(alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa, false));
                        relacionamentos = RelacionamentoUI.toRelacionamentos(alunoUI.pessoaFisicaUI.relacionamentosUI);
                        pessoaFisica = pessoaFisica = BusinessPessoa.postUpdatePessoaFisica(alunoUI.pessoaFisicaUI, relacionamentos,true, true);

                    }
                    else
                        pessoaFisica = BusinessPessoa.postInsertPessoaFisica(alunoUI.pessoaFisicaUI, RelacionamentoUI.toRelacionamentos(alunoUI.pessoaFisicaUI.relacionamentosUI),true);

                    alunoUI.aluno.cd_pessoa_aluno = pessoaFisica.cd_pessoa;
                    if (alunoUI.aluno.cd_pessoa_aluno <= 0)
                    {
                        alunoUI.pessoaFisicaUI.pessoaFisica.TelefonePessoa = null;
                        alunoUI.pessoaFisicaUI.pessoaFisica.Telefone = null;
                        throw new AlunoBusinessException("Favor informar a pessoa.  " + JsonConvert.SerializeObjectAsync(alunoUI.pessoaFisicaUI.pessoaFisica).Result, null, AlunoBusinessException.TipoErro.ERRO_EMAIL_JA_EXITE, false);
                    }
                    var atendente = alunoUI.aluno.Atendente;

                    alunoUI.aluno.Atendente = null;
                    alunoUI.aluno.MotivosMatricula = null;

                    if (alunoUI.aluno.Bolsa.pc_bolsa == null && string.IsNullOrEmpty(alunoUI.aluno.Bolsa.dc_validade_bolsa) &&
                        (alunoUI.aluno.Bolsa.pc_bolsa_material == null || alunoUI.aluno.Bolsa.pc_bolsa_material == 0) &&
                        alunoUI.aluno.Bolsa.dt_comunicado_bolsa == null && alunoUI.aluno.Bolsa.dt_cancelamento_bolsa == null &&
                    alunoUI.aluno.Bolsa.cd_motivo_bolsa == null && alunoUI.aluno.Bolsa.cd_motivo_cancelamento_bolsa == null &&
                    (alunoUI.aluno.Bolsa.dt_inicio_bolsa == null || alunoUI.aluno.Bolsa.dt_inicio_bolsa == DateTime.MinValue))
                        alunoUI.aluno.Bolsa = null;
                    else
                    {
                        if (alunoUI.aluno.Bolsa.dt_inicio_bolsa == DateTime.MinValue)
                            alunoUI.aluno.Bolsa.dt_inicio_bolsa = DateTime.Now.Date;
                        if (alunoUI.aluno.Bolsa.dt_comunicado_bolsa != null)
                            alunoUI.aluno.Bolsa.dt_comunicado_bolsa = alunoUI.aluno.Bolsa.dt_comunicado_bolsa.Value.Date;
                        if (alunoUI.aluno.Bolsa.dt_cancelamento_bolsa != null)
                            alunoUI.aluno.Bolsa.dt_cancelamento_bolsa = alunoUI.aluno.Bolsa.dt_cancelamento_bolsa.Value.Date;
                        if (alunoUI.aluno.Bolsa.pc_bolsa > 100 || alunoUI.aluno.Bolsa.pc_bolsa_material > 100)
                        {
                            string descriBolsa = "";
                            if (alunoUI.aluno.Bolsa.pc_bolsa > 100)
                                descriBolsa = "bolsa";
                            else
                                descriBolsa = "bolsa material";
                            throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgErroPercBolsaLimite, descriBolsa), null, AlunoBusinessException.TipoErro.ERRO_PERCENTUAL_BOLSA_ALUNO, false);
                        }
                        validarMinDateAlunoBolsa(alunoUI.aluno.Bolsa);
                    }
                    //Configura os motivos matrícula:
                    aluno = DataAccessAluno.add(alunoUI.aluno, false);

                    if (alunoUI.fichaSaude != null && alunoUI.fichaSaude.cd_ficha_saude == 0 && validaFichaAluno(alunoUI) == true)
                    {
                        FichaSaude fichaSaudeAluno = populaValuesNovaFichaAluno(alunoUI, aluno);
                        DataAccessFichaSaude.add(fichaSaudeAluno, false);
                    }

                    if (alunoUI.motivosMatriculaUI != null && alunoUI.motivosMatriculaUI.Count() > 0)
                    {
                        List<MotivoMatricula> MotivoMatriculas = new List<MotivoMatricula>();
                        List<MotivoMatricula> novosMotivosMatricula = alunoUI.motivosMatriculaUI.ToList();
                        for (int i = 0; i < novosMotivosMatricula.Count; i++)
                            DataAccessAlunoMotivoMatricula.add(new AlunoMotivoMatricula() { cd_aluno = aluno.cd_aluno, cd_motivo_matricula = novosMotivosMatricula[i].cd_motivo_matricula }, false);
                    }
                    
                    BusinessEmpresa.postEmpresaPessoa(new PessoaEscola
                    {
                        cd_escola = alunoUI.aluno.cd_pessoa_escola,
                        cd_pessoa = alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa
                    });
                    if (relacionamentos != null)
                    {
                        List<RelacionamentoSGF> rel = relacionamentos;
                        if (relacionamentos.Count() > 0)
                            for (int i = 0; i < relacionamentos.Count(); i++)
                            {
                                pessoaEsc = new PessoaEscola
                                {
                                    cd_escola = alunoUI.aluno.cd_pessoa_escola,
                                    cd_pessoa = rel[i].cd_pessoa_filho
                                };
                                BusinessEmpresa.postEmpresaPessoa(pessoaEsc);
                            }
                    }
                    //Quando usar o CPF, se não existir o relacionamento com o responsável, graval-lo.
                    if (alunoUI.pessoaFisicaUI != null && alunoUI.pessoaFisicaUI.pessoaFisica != null &&
                        string.IsNullOrEmpty(alunoUI.pessoaFisicaUI.pessoaFisica.nm_cpf) &&
                        alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf > 0)
                    {
                        List<RelacionamentoSGF> rel = relacionamentos;
                        bool existerelac = false;
                        if (relacionamentos != null)
                            existerelac = rel.Where(r =>
                                  r.cd_pessoa_pai == alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa &&
                                  r.cd_pessoa_filho == alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf &&
                                  r.cd_papel_filho == (int)PapelSGF.TipoPapelSGF.RESPONSAVEL &&
                                  r.cd_papel_pai == (int)PapelSGF.TipoPapelSGF.ALUNORESPONSAVEL).Any();
                        if (!existerelac)
                            BusinessPessoa.addRelacionamentoResponsavelAluno(new RelacionamentoSGF
                            {
                                cd_pessoa_pai = alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa,
                                cd_pessoa_filho = (int)alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf,
                                cd_papel_filho = (int)PapelSGF.TipoPapelSGF.RESPONSAVEL,
                                cd_papel_pai = (int)PapelSGF.TipoPapelSGF.ALUNORESPONSAVEL
                            });
                    }
                    if (alunoUI.atualizaHorarios && alunoUI.horarioSearchUI != null)
                        setHorarioAluno(alunoUI.horarioSearchUI.ToList(), aluno.cd_aluno, aluno.cd_pessoa_escola);

                    //Ao incluir aluno através do prospect, inativar prospect com motivo 1.
                    if (alunoUI.aluno.cd_prospect != null)
                    {
                        var prospectEdit = DataAccessProspect.findById((int)alunoUI.aluno.cd_prospect, false);
                        prospectEdit.id_prospect_ativo = false;
                        prospectEdit.cd_motivo_inativo = (int)Prospect.MotivoInativaProspect.MATRICULA_ESCOLA_CONCORRENTE;

                        DataAccessProspect.saveChanges(false);
                    }

                    transaction.Complete();
                }
            }
            catch (DbUpdateException ex)
            {
                if (alunoUI != null && alunoUI.pessoaFisicaUI.pessoaFisica != null && alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa > 0)
                {
                    if (alunoUI.pessoaFisicaUI.pessoaFisica.TelefonePessoa.Count() > 0)
                        alunoUI.pessoaFisicaUI.pessoaFisica.TelefonePessoa = null;
                    if (alunoUI.pessoaFisicaUI.pessoaFisica.Telefone != null)
                    {
                        alunoUI.pessoaFisicaUI.pessoaFisica.Telefone.ClasseTelefone.TelefoneClasse = null;
                        alunoUI.pessoaFisicaUI.pessoaFisica.Telefone.TelefonePessoa  = null;
                        alunoUI.pessoaFisicaUI.pessoaFisica.Telefone.TelefoneTipo.TipoTelefoneRef = null;
                        alunoUI.pessoaFisicaUI.pessoaFisica.Telefone.TelefonePessoa = null;
                    }
                    if (alunoUI.pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento.Count() > 0)
                        alunoUI.pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento = null;
                }
                throw new Exception("alunoUI: " + JsonConvert.SerializeObjectAsync(alunoUI).Result, ex);
            }
            return aluno;
        }

        [Obsolete]
        public Aluno editAluno(AlunoUI alunoUI)
        {
            PessoaFisicaSGF pessoaFisica = new PessoaFisicaSGF();
            Aluno aluno = new Aluno();
            sincronizaContexto(DataAccessAluno.DB());

            AlunoApiCyberBdUI alunoApiCyberBdOld = new AlunoApiCyberBdUI();
            List<RelacionamentoSGF> relacionamentos = new List<RelacionamentoSGF>();
            try
            {
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    if (BusinessApiNewCyberAluno.aplicaApiCyber())
                    {
                        //AlunoBdApiCyber
                        alunoApiCyberBdOld = DataAccessAluno.FindAlunoByCdAluno(alunoUI.aluno.cd_aluno, alunoUI.aluno.cd_pessoa_escola);
                    }

                    bool escolaInternacional = DataAccessAluno.getParametroEscolaInternacional(alunoUI.aluno.cd_pessoa_escola);

                    if (escolaInternacional == false && string.IsNullOrEmpty(alunoUI.pessoaFisicaUI.pessoaFisica.nm_cpf))
                    {
                        throw new AlunoBusinessException(Utils.Messages.Messages.msgErroCpfNuloEscolaNãoInternacional, null, AlunoBusinessException.TipoErro.ERRO_CPF_NULO_NAO_ITERNACIONAL, false);
                    }

                    if (alunoUI != null && alunoUI.pessoaFisicaUI != null && alunoUI.pessoaFisicaUI.relacionamentosUI != null)
                        relacionamentos = RelacionamentoUI.toRelacionamentos(alunoUI.pessoaFisicaUI.relacionamentosUI);
                    pessoaFisica = BusinessPessoa.postUpdatePessoaFisica(alunoUI.pessoaFisicaUI, relacionamentos,true,false);

                    alunoUI.aluno.cd_pessoa_aluno = pessoaFisica.cd_pessoa;
                    var atendente = alunoUI.aluno.Atendente;
                    alunoUI.aluno.Atendente = null;
                    alunoUI.aluno.AlunoPessoaFisica = null;
                    aluno = DataAccessAluno.findAlunoById(alunoUI.aluno.cd_aluno, alunoUI.aluno.cd_pessoa_escola);


                    //Se o aluno era ativo e está passando para inativo, verificar se ele está matriculado em alguma turma onde a situação seja "Ativo" ou "Rematriculado"
                    if (aluno.id_aluno_ativo && !alunoUI.aluno.id_aluno_ativo)
                    {
                        int alunoTurma = existAlunoMatriculadoOuRematriculado(alunoUI.aluno.cd_aluno, alunoUI.aluno.cd_pessoa_escola, 0);
                        if (alunoTurma > 0)
                            throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgErroDesativarAluno), null, AlunoBusinessException.TipoErro.ERRO_INATIVAR_ALUNO_ATIVO, false);
                    }

                    aluno = AlunoUI.changeValuesAluno(aluno, alunoUI.aluno);

                    if (alunoUI.fichaSaude != null && alunoUI.fichaSaude.cd_ficha_saude == 0 && validaFichaAluno(alunoUI) == true)
                    {
                        FichaSaude fichaSaudeAluno = populaValuesNovaFichaAluno(alunoUI, aluno);
                        DataAccessFichaSaude.add(fichaSaudeAluno, false);
                    }
                    else if (alunoUI.fichaSaude != null && alunoUI.fichaSaude.cd_ficha_saude > 0)
                    {
                        FichaSaude fichaSaudeChangeBd = DataAccessFichaSaude.findById(alunoUI.fichaSaude.cd_ficha_saude, false);
                        if (fichaSaudeChangeBd != null)
                        {
                            fichaSaudeChangeBd = changeValuesFichaSaude(alunoUI, fichaSaudeChangeBd);
                            DataAccessFichaSaude.saveChanges(false);
                        }
                    }

                    //Altera os dados do aluno bolsa:
                    AlunoBolsa alunoBolsa = DataAccessAlunoBolsa.findById(aluno.cd_aluno, false);
                    if (alunoUI.aluno.Bolsa.pc_bolsa == null &&
                        string.IsNullOrEmpty(alunoUI.aluno.Bolsa.dc_validade_bolsa) &&
                       (alunoUI.aluno.Bolsa.pc_bolsa_material == null || alunoUI.aluno.Bolsa.pc_bolsa_material == 0) &&
                        alunoUI.aluno.Bolsa.dt_comunicado_bolsa == null &&
                        alunoUI.aluno.Bolsa.dt_cancelamento_bolsa == null &&
                        alunoUI.aluno.Bolsa.cd_motivo_bolsa == null &&
                        alunoUI.aluno.Bolsa.cd_motivo_cancelamento_bolsa == null &&
                       (alunoUI.aluno.Bolsa.dt_inicio_bolsa == null || alunoUI.aluno.Bolsa.dt_inicio_bolsa == DateTime.MinValue))
                    {
                        if (alunoBolsa != null && alunoBolsa.cd_aluno > 0)
                            DataAccessAlunoBolsa.delete(alunoBolsa, false);
                    }
                    else
                    {
                        if (alunoUI.aluno.Bolsa.dt_inicio_bolsa == DateTime.MinValue)
                            alunoUI.aluno.Bolsa.dt_inicio_bolsa = DateTime.Now.Date;
                        if (alunoUI.aluno.Bolsa.dt_comunicado_bolsa != null)
                            alunoUI.aluno.Bolsa.dt_comunicado_bolsa = alunoUI.aluno.Bolsa.dt_comunicado_bolsa.Value.Date;
                        if (alunoUI.aluno.Bolsa.dt_cancelamento_bolsa != null)
                            alunoUI.aluno.Bolsa.dt_cancelamento_bolsa = alunoUI.aluno.Bolsa.dt_cancelamento_bolsa.Value.Date;
                        validarMinDateAlunoBolsa(alunoUI.aluno.Bolsa);
                        if (alunoUI.aluno.Bolsa.pc_bolsa > 100 || alunoUI.aluno.Bolsa.pc_bolsa_material > 100)
                        {
                            string descriBolsa = "";
                            if (alunoUI.aluno.Bolsa.pc_bolsa > 100)
                                descriBolsa = "bolsa";
                            else
                                descriBolsa = "bolsa material";
                            throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgErroPercBolsaLimite, descriBolsa), null, AlunoBusinessException.TipoErro.ERRO_PERCENTUAL_BOLSA_ALUNO, false);
                        }
                        if (alunoBolsa != null)
                        {
                            alunoBolsa = AlunoBolsa.changeValuesAlunoBolsa(alunoBolsa, alunoUI.aluno.Bolsa);
                            DataAccessAlunoBolsa.saveChanges(false);
                        }
                        else
                        {
                            alunoUI.aluno.Bolsa.cd_aluno = aluno.cd_aluno;
                            DataAccessAlunoBolsa.add(alunoUI.aluno.Bolsa, false);
                        }
                    }
                    DataAccessAluno.saveChanges(false);

                    aluno = DataAccessAluno.getAllDataAlunoById(alunoUI.aluno.cd_aluno, alunoUI.aluno.cd_pessoa_escola, false);


                    if (alunoUI.atualizaHorarios)
                        setHorarioAluno(alunoUI.horarioSearchUI != null ? alunoUI.horarioSearchUI.ToList() : new List<Horario>(), aluno.cd_aluno, aluno.cd_pessoa_escola);
                    //Configura os motivos matrícula:
                    if (alunoUI.motivosMatriculaUI != null)
                    {
                        List<MotivoMatricula> novosMotivosMatricula = alunoUI.motivosMatriculaUI.ToList();
                        List<AlunoMotivoMatricula> antigosMotivoMatricula = aluno.AlunosMotivosMatricula.ToList<AlunoMotivoMatricula>();
                        for (int i = 0; i < antigosMotivoMatricula.Count; i++)
                            if (!novosMotivosMatricula.Where(c => c.cd_motivo_matricula == antigosMotivoMatricula[i].cd_motivo_matricula).Any())
                                DataAccessAlunoMotivoMatricula.delete(antigosMotivoMatricula[i], false);

                        for (int i = 0; i < novosMotivosMatricula.Count; i++)
                            if (!antigosMotivoMatricula.Where(c => c.cd_motivo_matricula == novosMotivosMatricula[i].cd_motivo_matricula).Any())
                                DataAccessAlunoMotivoMatricula.add(new AlunoMotivoMatricula() { cd_aluno = aluno.cd_aluno, cd_motivo_matricula = novosMotivosMatricula[i].cd_motivo_matricula }, false);
                    }

                    // Relacionamento
                    if (relacionamentos != null)
                    {
                        PessoaEscola pessoaEsc = new PessoaEscola();
                        for (int i = 0; i < relacionamentos.Count; i++)
                        {
                            pessoaEsc = new PessoaEscola
                            {
                                cd_escola = aluno.cd_pessoa_escola,
                                cd_pessoa = relacionamentos[i].cd_pessoa_filho
                            };
                            BusinessEmpresa.postEmpresaPessoa(pessoaEsc);
                        }
                    }
                    BusinessEmpresa.postEmpresaPessoa(new PessoaEscola
                    {
                        cd_escola = alunoUI.aluno.cd_pessoa_escola,
                        cd_pessoa = alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa
                    });
                    //Quando usar o CPF, se não existir o relacionamento com o responsável, graval-lo.
                    if (alunoUI.pessoaFisicaUI != null && alunoUI.pessoaFisicaUI.pessoaFisica != null &&
                        string.IsNullOrEmpty(alunoUI.pessoaFisicaUI.pessoaFisica.nm_cpf) &&
                        alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf > 0)
                    {
                        bool existerelac = false;
                        if (relacionamentos != null)
                            existerelac = relacionamentos.Where(r =>
                                  r.cd_pessoa_pai == alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa &&
                                  r.cd_pessoa_filho == alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf &&
                                  r.cd_papel_filho == (int)PapelSGF.TipoPapelSGF.RESPONSAVEL &&
                                  r.cd_papel_pai == (int)PapelSGF.TipoPapelSGF.ALUNORESPONSAVEL).Any();
                        if (!existerelac)
                            BusinessPessoa.addRelacionamentoResponsavelAluno(new RelacionamentoSGF
                            {
                                cd_pessoa_pai = alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa,
                                cd_pessoa_filho = (int)alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf,
                                cd_papel_filho = (int)PapelSGF.TipoPapelSGF.RESPONSAVEL,
                                cd_papel_pai = (int)PapelSGF.TipoPapelSGF.ALUNORESPONSAVEL
                            });
                    }


                    if (alunoUI.pessoaRaf != null && 
                        !String.IsNullOrEmpty(alunoUI.pessoaRaf.nm_raf) && 
                        alunoUI.pessoaRaf.cd_pessoa > 0 &&
                        alunoUI.pessoaRaf.cd_pessoa_raf > 0 && alunoUI.isMaster == true)
                    {
                        PessoaRaf pessoaRafContext = DataAccessPessoaRaf.findById(alunoUI.pessoaRaf.cd_pessoa_raf, false);
                        pessoaRafContext.id_raf_liberado = alunoUI.pessoaRaf.id_raf_liberado;
                        pessoaRafContext.dt_limite_bloqueio = alunoUI.pessoaRaf.dt_limite_bloqueio;
                        DataAccessPessoaRaf.saveChanges(false);
                    }

                    if (BusinessApiNewCyberAluno.aplicaApiCyber())
                    {

                        AlunoApiCyberBdUI alunoApiCyberBdCurrent = DataAccessAluno.FindAlunoByCdAluno(aluno.cd_aluno, aluno.cd_pessoa_escola);
                        //Chama a api cyber com o comando (ATUALIZA_ALUNO)
                        verificaAlterouCamposExecutaCyberAtualizaAluno(alunoApiCyberBdOld, alunoApiCyberBdCurrent);

                        if (alunoApiCyberBdOld.aluno_ativo != alunoApiCyberBdCurrent.aluno_ativo && alunoApiCyberBdOld.aluno_ativo == true && existeAluno(alunoApiCyberBdOld.codigo))
                        {
                            //chama a api cyber com o comando (INATIVA_ALUNO)
                            executaCyberInativaAluno(alunoUI.aluno.cd_aluno);
                        }
                        else if (alunoApiCyberBdOld.aluno_ativo != alunoApiCyberBdCurrent.aluno_ativo && alunoApiCyberBdOld.aluno_ativo == false && existeAluno(alunoApiCyberBdOld.codigo))
                        {
                            //chama a api cyber com o comando (ATIVA_ALUNO)
                            executaCyberAtivaAluno(alunoUI.aluno.cd_aluno);
                        }
                    }


                    transaction.Complete();
                }
            }
            catch (DbUpdateException ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    AlunoBusinessException fx = new AlunoBusinessException(message, ex, 0, false);
                    throw new Exception(message, fx);
                }
                else
                  throw new Exception("alunoUI: " + JsonConvert.SerializeObjectAsync(alunoUI).Result, ex);
            }
            return aluno;
        }

        private static FichaSaude changeValuesFichaSaude(AlunoUI alunoUI, FichaSaude fichaSaudeChangeBd)
        {
            fichaSaudeChangeBd.id_problema_saude = alunoUI.fichaSaude.id_problema_saude;
            fichaSaudeChangeBd.dc_problema_saude = alunoUI.fichaSaude.dc_problema_saude;
            fichaSaudeChangeBd.id_tratamento_medico = alunoUI.fichaSaude.id_tratamento_medico;
            fichaSaudeChangeBd.dc_tratamento_medico = alunoUI.fichaSaude.dc_tratamento_medico;
            fichaSaudeChangeBd.id_uso_medicamento = alunoUI.fichaSaude.id_uso_medicamento;
            fichaSaudeChangeBd.dc_uso_medicamento = alunoUI.fichaSaude.dc_uso_medicamento;
            fichaSaudeChangeBd.id_recomendacao_medica = alunoUI.fichaSaude.id_recomendacao_medica;
            fichaSaudeChangeBd.dc_recomendacao_medica = alunoUI.fichaSaude.dc_recomendacao_medica;
            fichaSaudeChangeBd.id_alergico = alunoUI.fichaSaude.id_alergico;
            fichaSaudeChangeBd.dc_alergico = alunoUI.fichaSaude.dc_alergico;
            fichaSaudeChangeBd.id_alergico_alimento_material = alunoUI.fichaSaude.id_alergico_alimento_material;
            fichaSaudeChangeBd.dc_alergico_alimento_material = alunoUI.fichaSaude.dc_alergico_alimento_material;
            fichaSaudeChangeBd.id_epiletico = alunoUI.fichaSaude.id_epiletico;
            fichaSaudeChangeBd.id_epiletico_tratamento = alunoUI.fichaSaude.id_epiletico_tratamento;
            fichaSaudeChangeBd.id_asmatico = alunoUI.fichaSaude.id_asmatico;
            fichaSaudeChangeBd.id_asmatico_tratamento = alunoUI.fichaSaude.id_asmatico_tratamento;
            fichaSaudeChangeBd.id_diabetico = alunoUI.fichaSaude.id_diabetico;
            fichaSaudeChangeBd.id_depende_insulina = alunoUI.fichaSaude.id_depende_insulina;
            fichaSaudeChangeBd.id_medicacao_especifica = alunoUI.fichaSaude.id_medicacao_especifica;
            fichaSaudeChangeBd.dc_medicacao_especifica = alunoUI.fichaSaude.dc_medicacao_especifica;
            fichaSaudeChangeBd.dt_hora_medicacao_especifica = alunoUI.fichaSaude.dt_hora_medicacao_especifica;
            fichaSaudeChangeBd.tx_informacoes_adicionais = alunoUI.fichaSaude.tx_informacoes_adicionais;
            fichaSaudeChangeBd.id_plano_saude = alunoUI.fichaSaude.id_plano_saude;
            fichaSaudeChangeBd.dc_plano_saude = alunoUI.fichaSaude.dc_plano_saude;
            fichaSaudeChangeBd.dc_nm_carteirinha_plano = alunoUI.fichaSaude.dc_nm_carteirinha_plano;
            fichaSaudeChangeBd.dc_categoria_plano = alunoUI.fichaSaude.dc_categoria_plano;
            fichaSaudeChangeBd.dc_nome_clinica_hospital = alunoUI.fichaSaude.dc_nome_clinica_hospital;
            fichaSaudeChangeBd.dc_endereco_hospital_clinica = alunoUI.fichaSaude.dc_endereco_hospital_clinica;
            fichaSaudeChangeBd.dc_telefone_hospital_clinica = alunoUI.fichaSaude.dc_telefone_hospital_clinica;
            fichaSaudeChangeBd.dc_telefone_fixo_hospital_clinica = alunoUI.fichaSaude.dc_telefone_fixo_hospital_clinica;
            fichaSaudeChangeBd.id_aviso_emergencia = alunoUI.fichaSaude.id_aviso_emergencia;
            fichaSaudeChangeBd.dc_nome_pessoa_aviso_emergencia = alunoUI.fichaSaude.dc_nome_pessoa_aviso_emergencia;
            fichaSaudeChangeBd.dc_parentesco_aviso_emergencia = alunoUI.fichaSaude.dc_parentesco_aviso_emergencia;
            fichaSaudeChangeBd.dc_telefone_residencial_aviso_emergencia = alunoUI.fichaSaude.dc_telefone_residencial_aviso_emergencia;
            fichaSaudeChangeBd.dc_telefone_comercial_aviso_emergencia = alunoUI.fichaSaude.dc_telefone_comercial_aviso_emergencia;
            fichaSaudeChangeBd.dc_telefone_celular_aviso_emergencia = alunoUI.fichaSaude.dc_telefone_celular_aviso_emergencia;
            return fichaSaudeChangeBd;
        }

        private static FichaSaude populaValuesNovaFichaAluno(AlunoUI alunoUI, Aluno aluno)
        {
            FichaSaude fichaSaudeAluno = new FichaSaude();
            fichaSaudeAluno.cd_ficha_saude = 0;
            fichaSaudeAluno.cd_aluno = aluno.cd_aluno;
            fichaSaudeAluno.id_problema_saude = alunoUI.fichaSaude.id_problema_saude;
            fichaSaudeAluno.dc_problema_saude = alunoUI.fichaSaude.dc_problema_saude;
            fichaSaudeAluno.id_tratamento_medico = alunoUI.fichaSaude.id_tratamento_medico;
            fichaSaudeAluno.dc_tratamento_medico = alunoUI.fichaSaude.dc_tratamento_medico;
            fichaSaudeAluno.id_uso_medicamento = alunoUI.fichaSaude.id_uso_medicamento;
            fichaSaudeAluno.dc_uso_medicamento = alunoUI.fichaSaude.dc_uso_medicamento;
            fichaSaudeAluno.id_recomendacao_medica = alunoUI.fichaSaude.id_recomendacao_medica;
            fichaSaudeAluno.dc_recomendacao_medica = alunoUI.fichaSaude.dc_recomendacao_medica;
            fichaSaudeAluno.id_alergico = alunoUI.fichaSaude.id_alergico;
            fichaSaudeAluno.dc_alergico = alunoUI.fichaSaude.dc_alergico;
            fichaSaudeAluno.id_alergico_alimento_material = alunoUI.fichaSaude.id_alergico_alimento_material;
            fichaSaudeAluno.dc_alergico_alimento_material = alunoUI.fichaSaude.dc_alergico_alimento_material;
            fichaSaudeAluno.id_epiletico = alunoUI.fichaSaude.id_epiletico;
            fichaSaudeAluno.id_epiletico_tratamento = alunoUI.fichaSaude.id_epiletico_tratamento;
            fichaSaudeAluno.id_asmatico = alunoUI.fichaSaude.id_asmatico;
            fichaSaudeAluno.id_asmatico_tratamento = alunoUI.fichaSaude.id_asmatico_tratamento;
            fichaSaudeAluno.id_diabetico = alunoUI.fichaSaude.id_diabetico;
            fichaSaudeAluno.id_depende_insulina = alunoUI.fichaSaude.id_depende_insulina;
            fichaSaudeAluno.id_medicacao_especifica = alunoUI.fichaSaude.id_medicacao_especifica;
            fichaSaudeAluno.dc_medicacao_especifica = alunoUI.fichaSaude.dc_medicacao_especifica;
            fichaSaudeAluno.dt_hora_medicacao_especifica = alunoUI.fichaSaude.dt_hora_medicacao_especifica;
            fichaSaudeAluno.tx_informacoes_adicionais = alunoUI.fichaSaude.tx_informacoes_adicionais;
            fichaSaudeAluno.id_plano_saude = alunoUI.fichaSaude.id_plano_saude;
            fichaSaudeAluno.dc_plano_saude = alunoUI.fichaSaude.dc_plano_saude;
            fichaSaudeAluno.dc_nm_carteirinha_plano = alunoUI.fichaSaude.dc_nm_carteirinha_plano;
            fichaSaudeAluno.dc_categoria_plano = alunoUI.fichaSaude.dc_categoria_plano;
            fichaSaudeAluno.dc_nome_clinica_hospital = alunoUI.fichaSaude.dc_nome_clinica_hospital;
            fichaSaudeAluno.dc_endereco_hospital_clinica = alunoUI.fichaSaude.dc_endereco_hospital_clinica;
            fichaSaudeAluno.dc_telefone_hospital_clinica = alunoUI.fichaSaude.dc_telefone_hospital_clinica;
            fichaSaudeAluno.dc_telefone_fixo_hospital_clinica = alunoUI.fichaSaude.dc_telefone_fixo_hospital_clinica;
            fichaSaudeAluno.id_aviso_emergencia = alunoUI.fichaSaude.id_aviso_emergencia;
            fichaSaudeAluno.dc_nome_pessoa_aviso_emergencia = alunoUI.fichaSaude.dc_nome_pessoa_aviso_emergencia;
            fichaSaudeAluno.dc_parentesco_aviso_emergencia = alunoUI.fichaSaude.dc_parentesco_aviso_emergencia;
            fichaSaudeAluno.dc_telefone_residencial_aviso_emergencia = alunoUI.fichaSaude.dc_telefone_residencial_aviso_emergencia;
            fichaSaudeAluno.dc_telefone_comercial_aviso_emergencia = alunoUI.fichaSaude.dc_telefone_comercial_aviso_emergencia;
            fichaSaudeAluno.dc_telefone_celular_aviso_emergencia = alunoUI.fichaSaude.dc_telefone_celular_aviso_emergencia;
            return fichaSaudeAluno;
        }

        private bool validaFichaAluno(AlunoUI alunoUI)
        {

            if (alunoUI.fichaSaude.id_problema_saude == null &&
                alunoUI.fichaSaude.dc_problema_saude == null &&
                alunoUI.fichaSaude.id_tratamento_medico == null &&
                alunoUI.fichaSaude.dc_tratamento_medico == null &&
                alunoUI.fichaSaude.id_uso_medicamento == null &&
                alunoUI.fichaSaude.dc_uso_medicamento == null &&
                alunoUI.fichaSaude.id_recomendacao_medica == null &&
                alunoUI.fichaSaude.dc_recomendacao_medica == null &&
                alunoUI.fichaSaude.id_alergico == null &&
                alunoUI.fichaSaude.dc_alergico == null &&
                alunoUI.fichaSaude.id_alergico_alimento_material == null &&
                alunoUI.fichaSaude.dc_alergico_alimento_material == null &&
                alunoUI.fichaSaude.id_epiletico == null &&
                alunoUI.fichaSaude.id_epiletico_tratamento == null &&
                alunoUI.fichaSaude.id_asmatico == null &&
                alunoUI.fichaSaude.id_asmatico_tratamento == null &&
                alunoUI.fichaSaude.id_diabetico == null &&
                alunoUI.fichaSaude.id_depende_insulina == null &&
                alunoUI.fichaSaude.id_medicacao_especifica == null &&
                alunoUI.fichaSaude.dc_medicacao_especifica == null &&
                alunoUI.fichaSaude.dt_hora_medicacao_especifica == null &&
                alunoUI.fichaSaude.tx_informacoes_adicionais == null &&
                alunoUI.fichaSaude.id_plano_saude == null &&
                alunoUI.fichaSaude.dc_plano_saude == null &&
                alunoUI.fichaSaude.dc_nm_carteirinha_plano == null &&
                alunoUI.fichaSaude.dc_categoria_plano == null &&
                alunoUI.fichaSaude.dc_nome_clinica_hospital == null &&
                alunoUI.fichaSaude.dc_endereco_hospital_clinica == null &&
                alunoUI.fichaSaude.dc_telefone_hospital_clinica == null &&
                alunoUI.fichaSaude.dc_telefone_fixo_hospital_clinica == null &&
                alunoUI.fichaSaude.id_aviso_emergencia == null &&
                alunoUI.fichaSaude.dc_nome_pessoa_aviso_emergencia == null &&
                alunoUI.fichaSaude.dc_parentesco_aviso_emergencia == null &&
                alunoUI.fichaSaude.dc_telefone_residencial_aviso_emergencia == null &&
                alunoUI.fichaSaude.dc_telefone_comercial_aviso_emergencia == null &&
                alunoUI.fichaSaude.dc_telefone_celular_aviso_emergencia == null)
            {
                return false;
            }
            else
            {
                return true;
            }

            
        }

        public void verificaAlterouCamposExecutaCyberAtualizaAluno(AlunoApiCyberBdUI alunoApiCyberBdOld, AlunoApiCyberBdUI alunoApiCyberBdCurrent)
        {

            //Valida os parametros do banco
            //string parametrosBdOld = ValidaParametrosBDEdicao(alunoApiCyberBdOld, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.ATUALIZA_ALUNO, "");

            //Valida os parametros da View
            string parametrosBdCurrent = ValidaParametrosBDEdicao(alunoApiCyberBdCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.ATUALIZA_ALUNO, ""); 

            //Verifica se modificou chama o executa cyber
            if ((alunoApiCyberBdOld.nome != alunoApiCyberBdCurrent.nome ||
                 alunoApiCyberBdOld.email != alunoApiCyberBdCurrent.email) && existeAluno(alunoApiCyberBdCurrent.codigo))
            {
                string result = BusinessApiNewCyberAluno.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                    ApiCyberComandosNames.ATUALIZA_ALUNO, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametrosBdCurrent);
            }


        }

        public bool existeAluno(int codigo)
        {
            return BusinessApiNewCyberAluno.verificaRegistro(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
               ApiCyberComandosNames.VISUALIZA_ALUNO, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo);
        }

        public void executaCyberInativaAluno(int codigo)
        {
            string result = BusinessApiNewCyberAluno.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.INATIVA_ALUNO, ConfigurationManager.AppSettings["chaveApiNewCyber"], String.Format("codigo={0}", codigo));
        }

        public void executaCyberAtivaAluno(int codigo)
        {
            string result = BusinessApiNewCyberAluno.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.ATIVA_ALUNO, ConfigurationManager.AppSettings["chaveApiNewCyber"], String.Format("codigo={0}", codigo));
        }

        private string ValidaParametrosBDEdicao(AlunoApiCyberBdUI entity, string url, string comando, string parametros)
        {

            if (entity == null)
            {
                throw new ApiNewCyberAlunoException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberAlunoException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }

            if (entity.codigo <= 0)
            {
                throw new ApiNewCyberAlunoException(string.Format(Utils.Messages.Messages.ErroApiCyberCdAlunoMenorIgualZero, url, comando, parametros), null, ApiNewCyberAlunoException.TipoErro.ERRO_COD_ALUNO_MENOR_IGUAL_ZERO, false);
            }



            //Valida PessoaJuridica
            if (String.IsNullOrEmpty(entity.nome))
            {
                throw new ApiNewCyberAlunoException(string.Format(Utils.Messages.Messages.ErroApiCyberNomeAlunoNuloVazio, url, comando, parametros), null, ApiNewCyberAlunoException.TipoErro.ERRO_NOME_ALUNO_NULO_VAZIO, false);
            }

            //Valida Email
            if (String.IsNullOrEmpty(entity.email))
            {
                throw new ApiNewCyberAlunoException(string.Format(Utils.Messages.Messages.ErroApiCyberEmailAlunoNuloVazio, url, comando, parametros), null, ApiNewCyberAlunoException.TipoErro.ERRO_EMAIL_PESSOA_FISICA_NULA_OU_VAZIA, false);
            }


            string listaParams = "";


            listaParams = string.Format("nome={0},codigo={1},email={2}", entity.nome, entity.codigo, entity.email);
            return listaParams;
        }


        public bool alterarStatusAluno(AlunoStatusUI alunoStatusUI, bool status)
        {
            bool retorno = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                //Busca o aluno no bd e alterar seu status(ativo ou inativo)
                Aluno aluno = DataAccessAluno.findAlunoById(alunoStatusUI.cd_aluno, alunoStatusUI.cd_pessoa_escola);
                    aluno.id_aluno_ativo = status;
                    aluno = DataAccessAluno.edit(aluno, false);
                    if (aluno != null) retorno = true;

                transaction.Complete();
            }
            return retorno;
        }

        public void setHorarioAluno(List<Horario> horariosUI, int cdAluno, int cdEscola)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                List<Horario> HorariosView = new List<Horario>();
                Horario horario = new Horario();
                List<Horario> horarioContext = DataAccessHorario.getHorarioByEscolaForRegistro(cdEscola, cdAluno, Horario.Origem.ALUNO).ToList();
                DateTime dt_inicio = new DateTime(2000, 01, 01); //Vai analisar período de 01/01/200 a 01/01/2050 pois não temos aqui dados da turma
                DateTime? dt_final = new DateTime(2050, 01, 01);

                if (horariosUI != null)
                {
                    bool regraHorario = verificaHorarioOcupado(horariosUI);
                    if (regraHorario == false)
                        throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgErroDispHorarioAluno), null, AlunoBusinessException.TipoErro.ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA, false);
                    else
                    {
                        HorariosView = horariosUI;
                        IEnumerable<Horario> horarioAlunoComCodigo = from hpts in HorariosView
                                                                     where hpts.cd_horario != 0
                                                                     select hpts;
                        List<Horario> horarioAlunoDeleted = horarioContext.Where(tc => !horarioAlunoComCodigo.Any(tv => tc.cd_horario == tv.cd_horario)).ToList();
                        //Se diferente, não deletou todos horarios 
                        //Verificar se existe horario ocupado 
                        //Se existe horario ocupado, verificar se tem horario disponivel para ele
                        if (horarioAlunoDeleted.ToList().Count() != horarioContext.ToList().Count())
                        {
                            List<Horario> horarioOcupado = DataAccessHorario.getHorarioOcupadosForTurma(cdEscola, cdAluno, new int[0], 0, 0, 0, dt_inicio, dt_final, HorarioDataAccess.TipoConsultaHorario.HAS_HORARIO_ALUNO_OCUPADO_TURMA).ToList();
                            if (horarioOcupado.Count() > 0)
                                foreach (Horario h in horarioOcupado)
                                {
                                    Boolean valido = HorariosView.Where(comp => h.dt_hora_ini >= comp.dt_hora_ini && h.dt_hora_ini <= h.dt_hora_fim &&
                                        h.dt_hora_fim <= comp.dt_hora_fim && h.id_dia_semana == comp.id_dia_semana && comp.calendar == "Calendar1").Any();
                                    if (!valido)
                                        throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgErroDispHorarioAluno), null, AlunoBusinessException.TipoErro.ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA, false);

                                }
                        }
                        if (horarioAlunoDeleted.Count() > 0)
                        {
                            foreach (var item in horarioAlunoDeleted)
                            {
                                //var deletarHorarioProfessor = (from hp in horarioContext where hp.cd_horario == item.cd_horario select hp).FirstOrDefault();
                                if (item != null)
                                {
                                    //DataAccessHorario.sincronizaContexto(DataAccessAluno.DB());
                                    horario = DataAccessHorario.findById(item.cd_horario, false);
                                    DataAccessHorario.delete(horario, false);
                                }
                            }
                        }
                        HorariosView = HorariosView.Where(x => x.calendar == "Calendar1").ToList();
                        foreach (var item in HorariosView)
                        {
                            if (item.cd_horario.Equals(null) || item.cd_horario == 0)
                            {
                                item.cd_pessoa_escola = cdEscola;
                                item.cd_registro = cdAluno;
                                item.endTime = item.endTime.ToLocalTime();
                                item.startTime = item.startTime.ToLocalTime();
                                item.dt_hora_ini = new TimeSpan(item.startTime.Hour, item.startTime.Minute, 0);
                                item.dt_hora_fim = new TimeSpan(item.endTime.Hour, item.endTime.Minute, 0);
                                item.id_origem = (int)Horario.Origem.ALUNO;
                                DataAccessHorario.add(item, false);
                            }
                            else
                            {
                                var horarioAluno = (from hp in horarioContext where hp.cd_horario == item.cd_horario select hp).FirstOrDefault();
                                if (horarioAluno != null && horarioAluno.cd_horario > 0)
                                {
                                    //DataAccessHorario.sincronizaContexto(DataAccessAluno.DB());
                                    item.cd_registro = horarioAluno.cd_registro;
                                    horarioAluno = Horario.changeValueHorario(horarioAluno, item);
                                    DataAccessHorario.saveChanges(false);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (horarioContext != null)
                    {
                        foreach (var item in horarioContext)
                        {
                            var deletarHorarioAluno = (from hp in horarioContext where hp.cd_horario == item.cd_horario select hp).FirstOrDefault();
                            if (deletarHorarioAluno != null && deletarHorarioAluno.cd_horario > 0)
                            {
                                DataAccessHorario.delete(deletarHorarioAluno, false);
                            }
                        }
                    }
                }
                transaction.Complete();
            }
        }

        public Aluno getAllDataAlunoById(int cdAluno, int cdEscola, bool editView)
        {
            return DataAccessAluno.getAllDataAlunoById(cdAluno, cdEscola, editView);
        }

        public Aluno getAlunoById(int cdAluno, int cdEscola)
        {
            return DataAccessAluno.getAlunoById(cdAluno, cdEscola);
        }

        public AlunoSearchUI getAlunoByCodForGrid(int cd_aluno, int cd_empresa)
        {
            AlunoSearchUI retorno = new AlunoSearchUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessAluno.getAlunoByCodForGrid(cd_aluno, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public Aluno findAlunoById(int cd_aluno, int cd_escola)
        {
            return DataAccessAluno.findAlunoById(cd_aluno, cd_escola);
        }

        public PessoaFisicaSearchUI ExistAlunoOrPessoaFisicaByCpf(string cpf, string email, string nome, int cd_pessoa_cpf, int cdEscola)
        {
            PessoaFisicaSearchUI pFisica = new PessoaFisicaSearchUI();
            var aluno = DataAccessAluno.getAlunoByCpf(cpf, email, nome, cd_pessoa_cpf, cdEscola);
            if (aluno != null && aluno.cd_aluno > 0 && aluno.AlunoPessoaFisica != null)
            {
                if(aluno.cd_pessoa_escola != cdEscola)
                    throw new AlunoBusinessException(string.Format(Messages.msgExistAlunoForCPfEscola, aluno.AlunoPessoaFisica.no_pessoa), null, AlunoBusinessException.TipoErro.ERRO_ALUNOJAEXISTE, false);
                if (aluno.id_aluno_ativo)
                    throw new AlunoBusinessException(string.Format(Messages.msgExistAlunoForCPf, aluno.AlunoPessoaFisica.no_pessoa), null, AlunoBusinessException.TipoErro.ERRO_ALUNOJAEXISTE, false);
                else
                    throw new AlunoBusinessException(string.Format(Messages.msgExistAlunoInaticoForCPf, aluno.AlunoPessoaFisica.no_pessoa), null, AlunoBusinessException.TipoErro.ERRO_ALUNOJAEXISTE, false);
            }
            else
                pFisica = BusinessPessoa.VerificarExisitsPessoByCpfOrCdPessoa(cpf, email, nome, cd_pessoa_cpf, null, 0);
            return pFisica;
        }
        public PessoaFisicaSearchUI ExistAlunoOrPessoaFisicaByCpfAluno(string cpf, string email, string nome, int cd_pessoa_cpf, int cdEscola)
        {
            PessoaFisicaSearchUI pFisica = new PessoaFisicaSearchUI();
            var aluno = DataAccessAluno.getAlunoByCpfAluno(cpf, email, nome, cd_pessoa_cpf, cdEscola);
            if (aluno != null && aluno.cd_aluno > 0 && aluno.AlunoPessoaFisica != null)
            {
                if (aluno.cd_pessoa_escola != cdEscola)
                    throw new AlunoBusinessException(string.Format(Messages.msgExistAlunoForCPfEscola, aluno.AlunoPessoaFisica.no_pessoa), null, AlunoBusinessException.TipoErro.ERRO_ALUNOJAEXISTE, false);
                if (aluno.id_aluno_ativo)
                    throw new AlunoBusinessException(string.Format(Messages.msgExistAlunoForCPf, aluno.AlunoPessoaFisica.no_pessoa), null, AlunoBusinessException.TipoErro.ERRO_ALUNOJAEXISTE, false);
                else
                    throw new AlunoBusinessException(string.Format(Messages.msgExistAlunoInaticoForCPf, aluno.AlunoPessoaFisica.no_pessoa), null, AlunoBusinessException.TipoErro.ERRO_ALUNOJAEXISTE, false);
            }
            else
                pFisica = BusinessPessoa.VerificarExisitsPessoByCpfOrCdPessoa(cpf, email, nome, cd_pessoa_cpf, null, 0);
            return pFisica;
        }

        public IEnumerable<Horario> getHorarioByEscolaForAluno(int cdEscola, int cdAluno)
        {
            return DataAccessHorario.getHorarioByEscolaForRegistro(cdEscola, cdAluno, Horario.Origem.ALUNO);
        }

        public IEnumerable<AlunoSearchUI> getAlunoByEscola(int cdEscola, bool? status, int? cd_aluno)
        {
            return DataAccessAluno.getAlunoByEscola(cdEscola, status, cd_aluno);
        }

        public AlunoSearchUI verificarAlunoExistEmail(string email, int cdEscola, int cd_prospect)
        {
            return DataAccessAluno.verificarAlunoExistEmail(email, cdEscola, cd_prospect);
        }

        public Aluno buscarAlunoExistEmail(string email, string nome, int cd_pessoa_cpf)
        {
            return DataAccessAluno.buscarAlunoExistEmail(email, nome, cd_pessoa_cpf);
        }

        public List<Aluno> getAlunosByCod(int[] cdAlunos, int cdEscola)
        {
            return DataAccessAluno.getAlunosByCod(cdAlunos, cdEscola);
        }

        public IEnumerable<AlunoSearchUI> getAlunosDisponiveisFaixaHorario(SearchParameters parametros, string desc, string email, bool inicio, bool? status,
            string cpf, int sexo, List<Horario> horariosTurma, int cd_escola, int cd_turma, int cd_produto, bool id_turma_PPT, DateTime? dt_final_aula,
            int cd_curso, int cd_duracao)
        {
            
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessAluno.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_aluno_ativo");
                retorno = DataAccessAluno.getAlunosDisponiveisFaixaHorario(parametros, desc, email, inicio, status, cpf, sexo, horariosTurma, cd_escola, cd_turma, cd_produto, 
                    id_turma_PPT, dt_final_aula, cd_curso, cd_duracao).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public void verificarAlunosDisponiveisFaixaHorario(int cd_turma, int? cd_turma_ppt, int cd_escola, Horario h, List<Aluno> alunos)
        {
            List<Aluno> alunosDisp = new List<Aluno>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                //var totalAlunosView = alunos.Select(x => x.cd_aluno).Distinct().Count();
                var totalAlunosViewAux = DataAccessAluno.getAlunosMatriculadosRematriculados(cd_turma, cd_turma_ppt, cd_escola, alunos).Distinct().Count();

                var totalAlunosBD = DataAccessAluno.getAlunosDisponiveisFaixaHorario(cd_turma, (cd_turma_ppt != null ? (int?)0 : null), cd_escola, h, alunos).Count();
                if (totalAlunosViewAux != totalAlunosBD)
                    throw new AlunoBusinessException(string.Format(Messages.msgAlunoHorarioDisponivel), null, AlunoBusinessException.TipoErro.ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA, false);
                transaction.Complete();
            }
        }
        public void getAlunosDisponiveisFaixaHorarioCancelEnc(int cd_turma, int? cd_turma_ppt,int cd_produto, int cd_escola, Horario h)
        {
            IEnumerable<Aluno> alunosDisp = new List<Aluno>();
            List<int> alunosDispoVerif = DataAccessAluno.getAlunosDisponiveisHistCancelEnc(cd_turma, cd_escola);
            int totalAlunosDispoVerif = alunosDispoVerif.Count();
            alunosDisp = DataAccessAluno.getAlunosDisponiveisFaixaHorarioCancelEnc(cd_turma, cd_escola, h, alunosDispoVerif);
            var totalAlunosBD = alunosDisp.Count();
            if (totalAlunosDispoVerif != totalAlunosBD)
            {
                List<int> alunoOK = alunosDisp.Where(p => alunosDispoVerif.Contains(p.cd_aluno)).Select(a => a.cd_aluno).ToList();
                var alunoOcupado = alunosDispoVerif.Where(l => !alunoOK.Contains(l)).FirstOrDefault();

                AlunoSearchUI aluno = getAlunoByEscola(cd_escola, null, alunoOcupado).FirstOrDefault();
                throw new AlunoBusinessException(string.Format(Messages.msgErroHorarioOcupadoVirada, "aluno " + aluno.no_pessoa), null, AlunoBusinessException.TipoErro.ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA, false);
            }
            //Verifica se o aluno já está em turma de mesmo produto 
            if (alunosDispoVerif != null && alunosDispoVerif.Count() > 0)
            {
                bool turmaPPTFilha = cd_turma_ppt > 0 ? true : false;
                bool existe = existeProdutoAlunoMat(cd_escola, turmaPPTFilha, cd_produto, alunosDispoVerif, cd_turma);
                if (existe)
                    throw new AlunoBusinessException(string.Format(Messages.msgErroAlunoProdVirada), null, AlunoBusinessException.TipoErro.ERRO_ALUNO_TURMA_ENC_OCUPADO_PRODUTO, false);
            }
        }
        public List<int> getAlunosDisponiveisHistCancelEnc(int cd_turma, int cd_escola) {

            return DataAccessAluno.getAlunosDisponiveisHistCancelEnc(cd_turma, cd_escola);

        }

        public IEnumerable<Aluno> returnAlunosSemAvalicacaoByTurma(int cd_escola, int cd_turma)
        {
            return DataAccessAluno.returnAlunosSemAvalicacaoByTurma(cd_escola, cd_turma);
        }

        public IEnumerable<Aluno> getAllAlunosTurmasFilhasPPT(int cd_turma_ppt, int cd_escola)
        {
            List<Aluno> listAluno = new List<Aluno>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                listAluno =  DataAccessAluno.getAllAlunosTurmasFilhasPPT(cd_turma_ppt, cd_escola).ToList();
                transaction.Complete();
            }
            return listAluno;
        }

        public IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAula(int cd_turma, int cd_pessoa_escola, DateTime dtAula)
        {
            return DataAccessAluno.getAlunosTurmaAtivosDiarioAula(cd_turma, cd_pessoa_escola, null, dtAula);
        }
        public IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAulaDiario(int cd_turma, int cd_pessoa_escola, DateTime dtAula)
        {
            return DataAccessAluno.getAlunosTurmaAtivosDiarioAulaDiario(cd_turma, cd_pessoa_escola, null, dtAula);
        }
        public IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAulaCarga(int cd_turma, int cd_pessoa_escola, DateTime dtAula)
        {
            return DataAccessAluno.getAlunosTurmaAtivosDiarioAulaCarga(cd_turma, cd_pessoa_escola, null, dtAula);
        }

        public IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAula(int cd_escola, int cd_turma, DateTime? dt_inicial, DateTime dt_final, AlunoTurma.FiltroSituacaoAlunoTurma situacao_aluno)
        {
            List<Aluno> listAluno = new List<Aluno>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                listAluno = DataAccessAluno.getAlunosTurmaAtivosDiarioAula(cd_turma, cd_escola, dt_inicial, dt_final, situacao_aluno).ToList();
                transaction.Complete();
            }
            return listAluno;
        }
        public IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAulaReport(int cd_escola, int cd_turma, DateTime? dt_inicial, DateTime dt_final)
        {
            IEnumerable<Aluno> retorno = new List<Aluno>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessAluno.getAlunosTurmaAtivosDiarioAulaReport(cd_turma, cd_escola, dt_inicial, dt_final);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Aluno> getAlunosPorEventoDiario(int cd_turma, int cd_pessoa_escola, DateTime dtAula, int cd_evento, int cd_diario_aula)
        {
            return DataAccessAluno.getAlunosPorEventoDiario(cd_turma, cd_pessoa_escola, dtAula, cd_evento, cd_diario_aula);
        }

        public IEnumerable<Aluno> getAlunoPorTurma(int cdTurma, int cdEscola, int opcao)
        {
            return DataAccessAluno.getAlunoPorTurma(cdTurma, cdEscola, opcao);
        }

        public IEnumerable<AlunoSearchUI> getAlunoDesistente(SearchParameters parametros, string desc, string email, bool inicio, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_aluno_ativo");
                retorno = DataAccessAluno.getAlunoDesistente(parametros, desc, email, inicio, status, cdEscola, cpf, cdSituacao, sexo);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<AlunoSearchUI> getAlunoPorTurmaSearch(SearchParameters parametros, string desc, string email, bool inicio, int origemFK, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo, int cdTurma, int opcao)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_aluno_ativo");
                retorno = DataAccessAluno.getAlunoPorTurmaSearch(parametros, desc, email, inicio, origemFK, status, cdEscola, cpf, cdSituacao, sexo, cdTurma, opcao);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<AlunoSearchUI> getAlunoPorTurmaSearchAulaReposicao(SearchParameters parametros, string desc, string email, bool inicio, int origemFK, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo, int cdTurma, int opcao)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_aluno_ativo");
                retorno = DataAccessAluno.getAlunoPorTurmaSearchAulaReposicao(parametros, desc, email, inicio, origemFK, status, cdEscola, cpf, cdSituacao, sexo, cdTurma, opcao);
                transaction.Complete();
            }
            return retorno;
        }



        public IEnumerable<AlunoSearchUI> getAlunoPorTurmaControleFaltaSearch(SearchParameters parametros, List<int> cdAlunos, string desc, string email, bool inicio, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo, int cdTurma, int opcao, DateTime? dataFinalHistorico)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_aluno_ativo");
                retorno = DataAccessAluno.getAlunoPorTurmaControleFaltaSearch(parametros, cdAlunos, desc, email, inicio, status, cdEscola, cpf, cdSituacao, sexo, cdTurma, opcao, dataFinalHistorico);
                transaction.Complete();
            }
            return retorno;
        }

        public AlunoSearchUI getAlunoPorTurmaPPTFilha(int cdEscola, int cdTurma, int cdTurmaPai, int opcao)
        {
            return DataAccessAluno.getAlunoPorTurmaPPTFilha(cdEscola, cdTurma, cdTurmaPai, opcao);
        }

        public bool existeAlunoMatOrRemEscola(int cdEscola)
        {
            return DataAccessAluno.existeAlunoMatOrRemEscola(cdEscola);
        }

        public IEnumerable<AlunoRel> getRelAluno(string nome, int cdResp, string telefone, string email, bool? status, int cdEscola, DateTime? dtaIni, DateTime? dtaFinal, int cd_midia, List<int> cdSituacaoA, bool exibirEnderecos)
        {
            IEnumerable<AlunoRel> retorno = new List<AlunoRel>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessAluno.getRelAluno(nome, cdResp, telefone, email, status, cdEscola, dtaIni, dtaFinal, cd_midia, cdSituacaoA, exibirEnderecos);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurma(int cd_turma, bool id_turma_ppt, List<int> cd_situacao_aluno_turma, int cdEscolaAluno)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessAluno.getRptAlunosTurma(cd_turma, id_turma_ppt, cd_situacao_aluno_turma, cdEscolaAluno);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmaEncerrar(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, DateTime dtaFim, int cdEscolaAluno)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessAluno.getRptAlunosTurmaEncerrar(cd_turma, id_turma_ppt, dtaIniAula, dtaFim, cdEscolaAluno);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmaPPTEncerrar(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, DateTime dtaFim, int cdEscolaAluno)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessAluno.getRptAlunosTurmaEncerrar(cd_turma, id_turma_ppt, dtaIniAula, dtaFim, cdEscolaAluno);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmaNova(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, Nullable<DateTime> dtaFim, int cdEscolaAluno)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessAluno.getRptAlunosTurmaNova(cd_turma, id_turma_ppt, dtaIniAula, dtaFim, cdEscolaAluno);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmaPPTNova(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, DateTime dtaFim, int cdEscolaAluno)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessAluno.getRptAlunosTurmaNova(cd_turma, id_turma_ppt, dtaIniAula, dtaFim, cdEscolaAluno);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmaEncerrar(List<int> cdTurmas, DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int tipoTurma)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED,
                DataAccessAluno.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = DataAccessAluno.getRptAlunosTurmaEncerrar(cdTurmas, pDtaFimI, pDtaFimF, cd_escola, tipoTurma);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmasNovas(List<int> cdTurmas, DateTime? pDtaI, DateTime? pDtaF, int cd_escola, int tipoTurma)
        {
            IEnumerable<AlunoSearchUI> retorno = new List<AlunoSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessAluno.getRptAlunosTurmasNovas(cdTurmas, pDtaI, pDtaF, cd_escola, tipoTurma);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<PessoaSearchUI> getPessoaSearchEscolaWithCPFCNPJ(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa, int papel)
        {
            IEnumerable<PessoaSearchUI> retorno = new List<PessoaSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
                parametros.sort = parametros.sort.Replace("nm_cpf_cgc_dependente", "nm_cpf_cgc");
                retorno = DataAccessAluno.getPessoaSearchEscolaWithCPFCNPJ(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, cd_empresa, papel).ToList();
                transaction.Complete();
            }
            return retorno;
        }
        public IEnumerable<PessoaSearchUI> getTdsPessoaSearchEscola(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa)
        {
            IEnumerable<PessoaSearchUI> retorno = new List<PessoaSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessAluno.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
                retorno = DataAccessAluno.getTdsPessoaSearchEscola(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, cd_empresa).ToList();
                transaction.Complete();
            }
            return retorno;
        }
        private void validarMinDateAlunoBolsa(AlunoBolsa bolsa)
        {
            if ((bolsa != null && bolsa.dt_inicio_bolsa != null && DateTime.Compare((DateTime)bolsa.dt_inicio_bolsa, new DateTime(1900, 1, 1)) < 0) ||
                (bolsa != null && bolsa.dt_comunicado_bolsa != null && DateTime.Compare((DateTime)bolsa.dt_comunicado_bolsa, new DateTime(1900, 1, 1)) < 0) ||
                (bolsa != null && bolsa.dt_cancelamento_bolsa != null && DateTime.Compare((DateTime)bolsa.dt_cancelamento_bolsa, new DateTime(1900, 1, 1)) < 0))
                throw new PessoaBusinessException(Messages.msgErroDataErroMinDateAlunoBolsa, null,
                      FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_MINDATE_SMALLDATETIME, false);
            if ((bolsa != null && bolsa.dt_inicio_bolsa != null && DateTime.Compare((DateTime)bolsa.dt_inicio_bolsa, new DateTime(2079, 06, 06)) > 0) ||
                (bolsa != null && bolsa.dt_comunicado_bolsa != null && DateTime.Compare((DateTime)bolsa.dt_comunicado_bolsa, new DateTime(2079, 06, 06)) > 0) ||
                (bolsa != null && bolsa.dt_cancelamento_bolsa != null && DateTime.Compare((DateTime)bolsa.dt_cancelamento_bolsa, new DateTime(2079, 06, 06)) > 0))
                throw new PessoaBusinessException(Messages.msgErroDataErroMaxDateAlunoBolsa, null,
                      FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_MINDATE_SMALLDATETIME, false);
        }

        public List<Aluno> verificarAlunosCompraMaterialDidatico(int cd_turma, DateTime dta_programacao_turma, int cd_escola)
        {
            List<Aluno> alunosComPendenciaMaterial = new List<Aluno>();
            List<Aluno> alunosDiario = DataAccessAluno.getAlunosTurmaAtivosDiarioAula(cd_turma, cd_escola, null, dta_programacao_turma).ToList();
            int qtd_material_curso = BusinessFinanceiro.quantidadeItensMaterialCurso(cd_turma, cd_escola);
            if (alunosDiario != null)
            {
                List<int> cdAlunos = alunosDiario.Select(x => x.cd_aluno).ToList();
                List<ItemMovimento> itensMaterial = BusinessFiscal.getItensMaterialAluno(cdAlunos, cd_escola, cd_turma);
                if (itensMaterial != null && itensMaterial.Count() > 0)
                {
                    var listaMaterialPorAluno = itensMaterial.GroupBy(x => new { x.cd_pessoa_cliente, x.cd_item }).Select(g => new ItemMovimento
                        {
                            cd_pessoa_cliente = g.Key.cd_pessoa_cliente,
                            cd_item = g.Key.cd_item
                        });
                    var listaAlunoQtdMaterial = listaMaterialPorAluno.GroupBy(x => new { x.cd_pessoa_cliente }).Select(g => new ItemMovimento
                    {
                        cd_pessoa_cliente = g.Key.cd_pessoa_cliente,
                        qt_item_movimento = g.Count(),
                    });

                    alunosComPendenciaMaterial = alunosDiario.Where(ad => listaAlunoQtdMaterial.Where(qi => qi.cd_pessoa_cliente == ad.cd_pessoa_aluno &&
                                                                                                       qi.qt_item_movimento < qtd_material_curso).Any()).ToList();
                    alunosComPendenciaMaterial = alunosComPendenciaMaterial.Union(alunosDiario.Where(ad => !listaAlunoQtdMaterial.Where(qi => qi.cd_pessoa_cliente == ad.cd_pessoa_aluno).Any()).ToList()).ToList();
                }
                else
                    alunosComPendenciaMaterial = alunosDiario;
            }
            return alunosComPendenciaMaterial;
        }

        public List<Aluno> verificarAlunosSemTitulos(int cd_turma, DateTime dta_programacao_turma, int cd_escola)
        {
            List<Aluno> alunosSemTitulos = new List<Aluno>();
            List<Aluno> alunosDiario = DataAccessAluno.getAlunosTurmaAtivosDiarioAula(cd_turma, cd_escola, null, dta_programacao_turma).ToList();
            List<int> cdAlunos = alunosDiario.Select(x => x.cd_pessoa_aluno).ToList();
            cdAlunos = BusinessFinanceiro.getAlunosQuePossuemTitulosAbertoMes(cdAlunos, cd_escola, dta_programacao_turma);
            if (cdAlunos != null && cdAlunos.Count() > 0)
                alunosSemTitulos = alunosDiario.Where(x => !cdAlunos.Contains(x.cd_pessoa_aluno)).ToList();
            else
                alunosSemTitulos = alunosDiario;
            return alunosSemTitulos;
        }


        public bool existeProdutoAlunoMat(int cd_escola, bool turmaFilha, int cd_produto, List<int> cdAlunos, int cd_turma)
        {
            return DataAccessAluno.existeProdutoAlunoMat(cd_escola, turmaFilha, cd_produto, cdAlunos, cd_turma);
        }

        public PessoaSGF getPessoaById(int cd_pessoa)
        {
           return DataAccessAluno.getPessoaById(cd_pessoa);
        }

        public AlunoSearchUI editStatusAluno(int cdAluno, bool idAtivo, int cdEscola, int cdTurma)
        {
            Aluno aluno = new Aluno();
            sincronizaContexto(DataAccessAluno.DB());
            List<RelacionamentoSGF> relacionamentos = new List<RelacionamentoSGF>();
            
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                aluno = DataAccessAluno.findAlunoById(cdAluno, cdEscola);

                //Se o aluno era ativo e está passando para inativo, verificar se ele está matriculado em alguma turma onde a situação seja "Ativo" ou "Rematriculado"
                if (aluno.id_aluno_ativo && !idAtivo)
                {
                    int alunoTurma = existAlunoMatriculadoOuRematriculado(cdAluno, cdEscola, cdTurma);
                    if (alunoTurma > 0)
                        throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgErroDesativarAluno), null, AlunoBusinessException.TipoErro.ERRO_INATIVAR_ALUNO_ATIVO, false);
                }
                aluno.id_aluno_ativo = idAtivo;
                DataAccessAluno.saveChanges(false);

                transaction.Complete();
            }
            return DataAccessAluno.getAlunoByCodForGrid(aluno.cd_pessoa_aluno, aluno.cd_pessoa_escola);
        }
        //public PessoaFisicaSGF addPessoaRelacionado()
        //{
        //    PessoaFisicaSGF pessoaFisica = new PessoaFisicaSGF();
        //    if (alunoUI.pessoaFisicaUI != null && alunoUI.pessoaFisicaUI.pessoaFisica != null && alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa > 0 &&
        //                    (!string.IsNullOrEmpty(alunoUI.pessoaFisicaUI.pessoaFisica.nm_cpf) || alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf > 0))
        //    {
        //        //pessoaFisica.copy((PessoaFisicaSGF) DataAccessPessoaSGF.findById(alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa, false));
        //        relacionamentos = RelacionamentoUI.toRelacionamentos(alunoUI.pessoaFisicaUI.relacionamentosUI);
        //        pessoaFisica = pessoaFisica = BusinessPessoa.postUpdatePessoaFisica(alunoUI.pessoaFisicaUI, relacionamentos, true);

        //    }
        //    else
        //        pessoaFisica = BusinessPessoa.postInsertPessoaFisica(alunoUI.pessoaFisicaUI, RelacionamentoUI.toRelacionamentos(alunoUI.pessoaFisicaUI.relacionamentosUI), true);
        //}

        public Contrato getMatriculaByTurmaAlunoHistorico(int cd_escola, int cd_aluno, int cd_contrato)
        {
            return DataAccessAluno.getMatriculaByTurmaAlunoHistorico(cd_escola, cd_aluno, cd_contrato);
        }

        public void deletarAluno(Aluno aluno)
        {
            DataAccessAluno.delete(aluno, false);
        }

        #endregion

        #region Pessoa


        public IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> GetPessoaSearch(SearchParameters parametros, string nome,
            string apelido, int tipoPessoa, string cnpjCpf, int? papel, int sexo, bool inicio, int cdEscola, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoPessoaEnum tipo)
        {
            IEnumerable<PessoaSGFSearchUI> retorno = new List<PessoaSGFSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_pessoa_ativa");
                parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
                parametros.sort = parametros.sort.Replace("nm_cpf_cgc_dependente", "nm_cpf_cgc");
                retorno = DataAccessAluno.GetPessoaSearch(parametros, nome, apelido, tipoPessoa, cnpjCpf, papel, sexo, inicio, cdEscola, tipo);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> getPessoaUsuarioSearch(SearchParameters parametros, string nome,
                                                                                    string apelido, string cnpjCpf, int sexo, bool inicio, int cdEscola, int cd_pessoa_usuario)
        {
            if (parametros.sort == null)
                parametros.sort = "no_pessoa";
            parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
            parametros.sort = parametros.sort.Replace("id_ativo", "id_pessoa_ativa");
            parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
            return DataAccessAluno.getPessoaUsuarioSearch(parametros, nome, apelido, cnpjCpf, sexo, inicio, cdEscola, cd_pessoa_usuario);
        }

        public IEnumerable<PessoaSGFSearchUI> getPessoaMovimentoSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int tipoMovimento, int cd_empresa)
        {
            IEnumerable<PessoaSGFSearchUI> retorno = new List<PessoaSGFSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_pessoa_ativa");
                parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
                retorno = DataAccessAluno.getPessoaMovimentoSearch(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, tipoMovimento, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<PessoaSearchUI> getPessoaRelacionadaEscola(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa)
        {
            if (parametros.sort == null)
                parametros.sort = "no_pessoa";
            parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
            parametros.sort = parametros.sort.Replace("id_ativo", "id_pessoa_ativa");
            parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
            return DataAccessAluno.getPessoaRelacionadaEscola(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, cd_empresa);
        }

        public IEnumerable<PessoaSGFSearchUI> getPessoaTituloSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa, bool responsavel)
        {
            IEnumerable<PessoaSGFSearchUI> retorno = new List<PessoaSGFSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_pessoa_ativa");
                parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
                parametros.sort = parametros.sort.Replace("nm_cpf_cgc_dependente", "nm_cpf_cgc");
                retorno = DataAccessAluno.getPessoaTituloSearch(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, cd_empresa, responsavel);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<PessoaSearchUI> getPessoaResponsavelCPFSearchEscola(SearchParameters parametros, string nome, string apelido, bool inicio, string cnpjCpf, int sexo, int cd_empresa)
        {
            IEnumerable<PessoaSearchUI> retorno = new List<PessoaSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_pessoa_ativa");
                parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
                retorno = DataAccessAluno.getPessoaResponsavelCPFSearchEscola(parametros, nome, apelido, inicio, cnpjCpf, sexo, cd_empresa).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> getPessoaPapelSearchWithCPFCNPJ(SearchParameters parametros, string nome, string apelido, int tipoPessoa, string cnpjCpf, int? papel, int sexo, bool inicio, int cdEscola){
            IEnumerable<PessoaSGFSearchUI> retorno = new List<PessoaSGFSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_pessoa_ativa");
                parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
                parametros.sort = parametros.sort.Replace("nm_cpf_cgc_dependente", "nm_cpf_cgc");
                retorno = DataAccessAluno.getPessoaPapelSearchWithCPFCNPJ(parametros, nome, apelido, tipoPessoa, cnpjCpf, papel, sexo, inicio, cdEscola);
                transaction.Complete();
            }
            return retorno;
        }

        public PessoaFisicaSearchUI existePessoaEscolaOrByCpf(string cpf, int cdEscola)
        {
            PessoaFisicaSearchUI pFisica = new PessoaFisicaSearchUI();
            PessoaSGF pessoaFisicaEscola = DataAccessAluno.verificaExistePessoaEmpresaByCpf(cpf, cdEscola);
            if (pessoaFisicaEscola != null && pessoaFisicaEscola.cd_pessoa > 0)
                throw new AlunoBusinessException(string.Format(Messages.msgExistPessoaRelacEscola, pessoaFisicaEscola.no_pessoa), null, AlunoBusinessException.TipoErro.ERRO_PESSOAEMPRESAJAEXISTE, false);
            else
                pFisica = BusinessPessoa.VerificarExisitsPessoByCpfOrCdPessoa(cpf, null, null, 0,null, 0);
            return pFisica;
        }
        public PessoaJurdicaSearchUI existePessoaJuridicaEscolaOrByCNPJ(string cnpj, int cdEscola)
        {
            PessoaJurdicaSearchUI pJuridica = new PessoaJurdicaSearchUI();
            PessoaSGF pessoaJuridicaEscola = DataAccessAluno.verificaExistePessoaJuridicaEmpresaByCpf(cnpj, cdEscola);
            if (pessoaJuridicaEscola != null && pessoaJuridicaEscola.cd_pessoa > 0)
                throw new AlunoBusinessException(string.Format(Messages.msgExistPessoaJuridicaRelacEscola, pessoaJuridicaEscola.no_pessoa), null, AlunoBusinessException.TipoErro.ERRO_PESSOAEMPRESAJAEXISTE, false);
            else
                pJuridica = BusinessPessoa.VerificarExisitsEmpresaByCnpjOrcdEmpresa(cnpj, null);
            return pJuridica;
        }

        public List<PessoaSGF> getListaAniversariantes(int cd_escola, int tipo, int cd_turma, int mes, int dia)
        {
            List<PessoaSGF> retorno = new List<PessoaSGF>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessAluno.getListaAniversariantes(cd_escola, tipo, cd_turma, mes, dia);
                transaction.Complete();
            }
            return retorno;
        }
        #endregion

        #region Aluno Turma
        public AlunoTurma findAlunosTurmaById(int cd_aluno_turma)
        {
            return dataAccessAlunoTurma.findById(cd_aluno_turma, false);
        }
        public IEnumerable<AlunoTurma> findAlunosTurmaPorTurmaEscola(int cd_turma, int cd_escola)
        {
            return dataAccessAlunoTurma.findAlunosTurmaPorTurmaEscola(cd_turma, cd_escola);
        }
        public IEnumerable<AlunoTurma> findAlunosTurma(int cd_turma, int cd_escola)
        {
            return dataAccessAlunoTurma.findAlunosTurma(cd_turma, cd_escola);
        }
        public AlunoTurma findAlunoTurma(int cd_aluno, int cd_turma, int cd_escola)
        {
            return dataAccessAlunoTurma.findAlunoTurma(cd_aluno, cd_turma, cd_escola);
        }

        public LivroAlunoApiCyberBdUI findLivroAlunoTurmaApiCyber(int cd_aluno, int cd_turma, int cd_escola)
        {
            return dataAccessAlunoTurma.findLivroAlunoTurmaApiCyber(cd_aluno, cd_turma, cd_escola);
        }

        public AlunoTurma findAlunoTurmaByCdCursoContrato(int cd_curso_contrato, int cd_escola)
        {
            return dataAccessAlunoTurma.findAlunoTurmaByCdCursoContrato(cd_curso_contrato, cd_escola);
        }
        public List<AlunoTurma> existsAlunosTurmaInTurmaDestino(List<int> cdsAlunosTurma, int cdTurmaDestino)
        {
            return dataAccessAlunoTurma.existsAlunosTurmaInTurmaDestino(cdsAlunosTurma, cdTurmaDestino);
        }

        public int findAlunoTurmaProduto(int cd_aluno, int cd_turma, int cd_escola)
        {
            return dataAccessAlunoTurma.findAlunoTurmaProduto(cd_aluno, cd_turma, cd_escola);
        }
        public AlunoTurma addAlunoTurmaMudancaInterna(AlunoTurma alunoTurma)
        {
            alunoTurma.dt_inicio = alunoTurma.dt_inicio.HasValue ? alunoTurma.dt_inicio.Value.ToLocalTime() : alunoTurma.dt_inicio;
            alunoTurma.dt_matricula = alunoTurma.dt_matricula.HasValue ? alunoTurma.dt_matricula.Value.ToLocalTime() : alunoTurma.dt_matricula;
            return dataAccessAlunoTurma.add(alunoTurma, false);
        }
        public AlunoTurma editAlunoTurma(AlunoTurma alunoTurma)
        {
            alunoTurma.dt_inicio = alunoTurma.dt_inicio.HasValue ? alunoTurma.dt_inicio.Value.ToLocalTime() : alunoTurma.dt_inicio;
            alunoTurma.dt_matricula = alunoTurma.dt_matricula.HasValue ? alunoTurma.dt_matricula.Value.ToLocalTime() : alunoTurma.dt_matricula;
            return dataAccessAlunoTurma.edit(alunoTurma, false);
        }

        public void deletarAlunoTurma(AlunoTurma alunoTurma){
            dataAccessAlunoTurma.delete(alunoTurma, false);
        }
        public AlunoTurma addAlunoTurma(AlunoTurma alunoTurma)
        {
            return dataAccessAlunoTurma.add(alunoTurma, false);
        }
        public bool existsAlunoTurmaByContratoEscola(int cd_contrato, int cd_pessoa_escola)
        {
            return dataAccessAlunoTurma.existsAlunoTurmaByContratoEscola(cd_contrato, cd_pessoa_escola);
        }
        public List<AlunoTurma> findAlunoTurmasByContratoEscola(int cd_contrato, int cd_pessoa_escola)
        {
            return dataAccessAlunoTurma.findAlunoTurmasByContratoEscola(cd_contrato, cd_pessoa_escola);
        }

        public int existAlunoMatriculadoOuRematriculado(int cd_aluno, int cd_escola, int cd_turma)
        {
            return dataAccessAlunoTurma.existAlunoMatriculadoOuRematriculado(cd_aluno, cd_escola, cd_turma);
        }

        public IEnumerable<AlunoTurma> findAlunosTurma(int cd_turma, int cd_escola, int[] cdAlunos)
        {
            return dataAccessAlunoTurma.findAlunosTurma(cd_turma, cd_escola, cdAlunos);
        }

        public IEnumerable<AlunoTurma> findAlunosTurmaHist(int cd_turma, int cd_escola, int[] cdAlunos)
        {
            return dataAccessAlunoTurma.findAlunosTurmaHist(cd_turma, cd_escola, cdAlunos);
        }

        public bool getVerificaAlunosTurma(List<AlunoTurma> alunosTurma)
        {

            List<int> listaAlunos = alunosTurma.Select(x => x.cd_aluno).ToList();
            return dataAccessAlunoTurma.getVerificaAlunosTurma(listaAlunos, alunosTurma[0].cd_turma);
        }

        public bool verificacoesMudanca(List<AlunoTurma> alunos, int cdEscola)
        {
            //VERIFICAR SE O ALUNO JÁ NÃO ESTÁ NA TURMA
            if (getVerificaAlunosTurma(alunos))
                throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgNaoMudarAlunoMsmTurma), null, AlunoBusinessException.TipoErro.ERRO_TURMA_POSSUI_ALUNO, false);

            //VERIFICAR SE TEM HORÁRIO DISPONIVEL
            if (!getverifivaHorarioMudanca(alunos, cdEscola))
                throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgAlunoOcupadoHorario), null, AlunoBusinessException.TipoErro.ERRO_ALUNO_HORARIO_OCUPADO, false);

            return true;
        }

        public bool getverifivaHorarioMudanca(List<AlunoTurma> alunosTurma, int cd_escola)
        {

            List<Aluno> alunos = new List<Aluno>();
            foreach (AlunoTurma at in alunosTurma)
                alunos.Add(new Aluno { cd_aluno = at.cd_aluno });

            List<Horario> listaHorarioTurmaDest = DataAccessHorario.getHorarioByEscolaForRegistro(cd_escola, alunosTurma[0].cd_turma, Horario.Origem.TURMA).ToList();
            bool alunosDisponiveis = true;
            foreach (Horario h in listaHorarioTurmaDest)
            {
                int cdTurmaOrigem = 0;
                if (alunosTurma[0].cd_turma > 0)
                    cdTurmaOrigem = (int)alunosTurma[0].cd_turma_origem;
                IEnumerable<Aluno> disponivel = DataAccessAluno.getAlunosDisponiveisFaixaHorario(cdTurmaOrigem, null, cd_escola, h, alunos);
                if (disponivel.Count() != alunos.Count())
                {
                    alunosDisponiveis = false;
                    break;
                }
            }
            return alunosDisponiveis;
        }

        public void persistirAulaExecutaEFaltaAlunosDiarioAula(List<Aluno> alunosComFalta, int cd_turma, int cd_pessoa_escola, DateTime dtAula, DiarioAula.StatusDiarioAula statusDiarioAula)
        {
            List<AlunoTurma> alunosTurmaDiario = dataAccessAlunoTurma.getAlunosTurmaAtivosDiarioAula(cd_turma, cd_pessoa_escola, dtAula).ToList();
            foreach (AlunoTurma at in alunosTurmaDiario)
            {
                //Retira a aula quando for cancelado/deleção o diário.
                if (statusDiarioAula == DiarioAula.StatusDiarioAula.Efetivada)
                    at.nm_aulas_dadas = at.nm_aulas_dadas + 1;
                else
                    at.nm_aulas_dadas = at.nm_aulas_dadas - 1;
            }
            if (alunosComFalta != null && alunosComFalta.Count() > 0)
            {
                List<int> cdAlunos = alunosComFalta.Select(x => x.cd_aluno).ToList();
                if (cdAlunos != null && cdAlunos.Count() > 0)
                {
                    List<AlunoTurma> alunosTurmaComFalta = alunosTurmaDiario.Where(atd => cdAlunos.Contains(atd.cd_aluno)).ToList();
                    //Ou retira a aula quando for cancelado/deleção o diário.
                    if (statusDiarioAula == DiarioAula.StatusDiarioAula.Efetivada)
                    {
                        if (alunosTurmaComFalta != null && alunosTurmaComFalta.Count() > 0)
                            foreach (var at in alunosTurmaComFalta)
                                at.nm_faltas = Byte.Parse((at.nm_faltas + 1) + "");
                    }
                    else
                        if (alunosTurmaComFalta != null && alunosTurmaComFalta.Count() > 0)
                            foreach (var at in alunosTurmaComFalta)
                                if (at.nm_faltas != 0)
                                    at.nm_faltas = Byte.Parse((at.nm_faltas - 1) + "");

                }
            }
            dataAccessAlunoTurma.saveChanges(false);
        }

        public void incrementarOuRemoverFaltaAlunoTurmaDiario(List<Aluno> alunos, int cd_turma, int cd_pessoa_escola, DiarioAula.StatusDiarioAula statusDiarioAula)
        {
            if (alunos != null && alunos.Count() > 0)
            {
                int i = 0;
                int[] cdAlunos = new int[alunos.Count()];
                foreach (var c in alunos)
                {
                    cdAlunos[i] = c.cd_aluno;
                    i++;
                }
                List<AlunoTurma> alunosTurma = dataAccessAlunoTurma.findAlunosTurma(cd_turma, cd_pessoa_escola, cdAlunos).ToList();
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    if (statusDiarioAula == DiarioAula.StatusDiarioAula.Efetivada)
                    {
                        if (alunosTurma != null && alunosTurma.Count() > 0)
                            foreach (var at in alunosTurma)
                                at.nm_faltas = Byte.Parse((at.nm_faltas + 1) + "");
                    }
                    else
                        if (alunosTurma != null && alunosTurma.Count() > 0)
                            foreach (var at in alunosTurma)
                                if (at.nm_faltas != 0)
                                    at.nm_faltas = Byte.Parse((at.nm_faltas - 1) + "");
                    dataAccessAlunoTurma.saveChanges(false);
                    transaction.Complete();
                }
            }
        }

        public IEnumerable<Aluno> getAlunosPorEvento(int cd_turma, int cd_pessoa_escola, int cd_evento, int cd_diario_aula)
        {
            return DataAccessAluno.getAlunosPorEvento(cd_turma, cd_pessoa_escola, cd_evento, cd_diario_aula);
        }

        public int? getStatusAlunoTurma(int cd_aluno, int cd_escola, int cd_turma)
        {
            return dataAccessAlunoTurma.getStatusAlunoTurma(cd_aluno, cd_escola, cd_turma);
        }

        public AlunoTurma findAlunoTurmaById(int id)
        {
            return dataAccessAlunoTurma.findById(id, false);
        }

        public void saveChagesAlunoTurma(AlunoTurma alunoTurma)
        {
            dataAccessAlunoTurma.saveChanges(false);
        }
        public AlunoTurma findAlunoTurmaContrato(int cd_aluno, int cd_turma, int cd_escola, int cd_contrato)
        {
            AlunoTurma retorno = new AlunoTurma();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = dataAccessAlunoTurma.findAlunoTurmaContrato(cd_aluno, cd_turma, cd_escola, cd_contrato);
                transaction.Complete();
            }
            return retorno;
        }

        public List<AlunoTurma> findAlunosTurmaForEncerramento(int cd_turma, int cd_escola)
        {
            return dataAccessAlunoTurma.findAlunosTurmaForEncerramento(cd_turma, cd_escola);
        }

        public bool deleteAlunoAguardandoTurma(int cdProduto, int cdEscola, int cdContrato, int cdAluno, int cd_turma)
        {
            return dataAccessAlunoTurma.deleteAlunoAguardandoTurma(cdProduto, cdEscola, cdContrato, cdAluno, cd_turma);
        }

        public List<AlunoTurma> getAlunoTurmaByCdContrato(int cd_contrato)
        {
            return dataAccessAlunoTurma.getAlunoTurmaByCdContrato(cd_contrato).ToList();
        }

        public List<AlunoTurma> getAlunoTurmaByCdContratoAndCdAluno(int cd_contrato, int cd_aluno)
        {
            return dataAccessAlunoTurma.getAlunoTurmaByCdContratoAndCdAluno(cd_contrato, cd_aluno).ToList();
        }

        public AlunoApiCyberBdUI findAlunoApiCyber(int cd_aluno, int cd_turma, int cd_contrato)
        {
            return dataAccessAlunoTurma.findAlunoApiCyber(cd_aluno, cd_turma, cd_contrato);
        }

        public PromocaoIntercambioParams findAlunoApiPromocaoIntercambio(int cd_aluno, int id_tipo_matricula)
        {
            return dataAccessAlunoTurma.findAlunoApiPromocaoIntercambio(cd_aluno, id_tipo_matricula);
        }


        public LivroAlunoApiCyberBdUI findLivroAlunoApiCyber(int cd_aluno, int cd_turma, int cd_contrato)
        {
            return dataAccessAlunoTurma.findLivroAlunoApiCyber(cd_aluno, cd_turma, cd_contrato);
        }



        #endregion

        #region Mudanças Internas

        public MudancasInternas postMudancaInterna(MudancasInternas mudanca)
        {

            AlunoTurma alunoTurmaDestino ;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                bool existeAlunoturma = getVerificaAlunosTurma(mudanca.alunos);
                if (mudanca.opcao == (int)FundacaoFisk.SGF.Web.Services.Secretaria.Model.MudancasInternas.OpcoesMudanca.MudarTurma)
                    foreach (AlunoTurma a in mudanca.alunos)
                    {
                        //Alterar data de desistencia no aluno
                        Aluno aluno = getAlunoById(a.cd_aluno, mudanca.cd_escola);
                        DataAccessAluno.saveChanges(false);

                        if (!mudanca.id_ppt)
                        {
                            //AlunoTurma alunoTurmaAnterior = dataAccessAlunoTurma.findAlunoTurma(a.cd_aluno, mudanca.cd_turma_origem, mudanca.cd_escola);
                            int[] alunos = {a.cd_aluno};
                            AlunoTurma alunoTurmaAnterior = dataAccessAlunoTurma.findAlunosTurma(mudanca.cd_turma_origem, mudanca.cd_escola, alunos).FirstOrDefault();

                            //Incluindo novo Aluno Turma para a turma de Destino
                            DateTime? dtaMatricula = null;
                            if (mudanca.id_manter_contrato && alunoTurmaAnterior.dt_matricula.HasValue)
                                dtaMatricula = alunoTurmaAnterior.dt_matricula.Value;
                            int situacaoAlunoTurma = (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Aguardando;
                            if (mudanca.id_manter_contrato && alunoTurmaAnterior.cd_contrato.HasValue && alunoTurmaAnterior.cd_contrato != 0)
                            {
                                int tipo = (int)DataAccessMatricula.getTpMatricula(alunoTurmaAnterior.cd_contrato.Value, mudanca.cd_escola);
                                situacaoAlunoTurma = tipo == 1 ? (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo : (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado;
                            }
                            AlunoTurma alunoMovido = dataAccessAlunoTurma.findAlunoTurmaMovido(a.cd_aluno, mudanca.cd_turma_destino, mudanca.cd_escola);
                            if (alunoMovido != null && alunoMovido.cd_aluno_turma > 0)
                            {
                                alunoTurmaDestino = alunoMovido;
                                alunoTurmaDestino.cd_situacao_aluno_turma = situacaoAlunoTurma;
                                alunoTurmaDestino.dt_inicio = mudanca.dt_inicio.ToLocalTime();
                                alunoTurmaDestino.dt_matricula = dtaMatricula;
                                alunoTurmaDestino.cd_contrato = mudanca.id_manter_contrato ? alunoTurmaAnterior.cd_contrato : null;
                                alunoTurmaDestino.nm_matricula_turma = mudanca.id_manter_contrato ? alunoTurmaAnterior.nm_matricula_turma : null;
                                alunoTurmaDestino.id_manter_contrato = mudanca.id_manter_contrato;
                                alunoTurmaDestino.id_renegociacao = mudanca.id_renegociacao;
                                alunoTurmaDestino.id_tipo_movimento = Convert.ToByte(HistoricoAluno.TipoMovimento.MUDANCA_INTERNA);
                                alunoTurmaDestino.cd_turma_origem = mudanca.cd_turma_origem;
                                alunoTurmaDestino.cd_situacao_aluno_origem = Convert.ToByte(alunoTurmaAnterior.cd_situacao_aluno_turma);
                                alunoTurmaDestino.dt_movimento = mudanca.dt_movimentacao;
                            }
                            else
                            {

                                alunoTurmaDestino = new AlunoTurma
                                {
                                    cd_aluno = a.cd_aluno,
                                    cd_turma = mudanca.cd_turma_destino,
                                    cd_situacao_aluno_turma = situacaoAlunoTurma,
                                    dt_inicio = mudanca.dt_inicio.ToLocalTime(),
                                    dt_matricula = dtaMatricula,
                                    cd_contrato = mudanca.id_manter_contrato ? alunoTurmaAnterior.cd_contrato : null,
                                    nm_matricula_turma = mudanca.id_manter_contrato ? alunoTurmaAnterior.nm_matricula_turma : null,
                                    nm_faltas = 0,
                                    id_manter_contrato = mudanca.id_manter_contrato,
                                    id_renegociacao = mudanca.id_renegociacao,
                                    id_tipo_movimento = Convert.ToByte(HistoricoAluno.TipoMovimento.MUDANCA_INTERNA),
                                    cd_turma_origem = mudanca.cd_turma_origem,
                                    cd_situacao_aluno_origem = Convert.ToByte(alunoTurmaAnterior.cd_situacao_aluno_turma),
                                    dt_movimento = mudanca.dt_movimentacao

                                };
                            }
                            ////historico aluno turma
                            if (alunoTurmaAnterior.cd_contrato != null)
                                gerarHistoricoMudanca(alunoTurmaAnterior, mudanca, a.cd_aluno, situacaoAlunoTurma);
                            //Alterando Aluno Turma da turma de Origem
                            alunoTurmaAnterior.dt_movimento = mudanca.dt_movimentacao;
                            alunoTurmaAnterior.id_tipo_movimento = (int)HistoricoAluno.TipoMovimento.MUDANCA_INTERNA;
                            alunoTurmaAnterior.cd_situacao_aluno_turma = (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Movido;
                            dataAccessAlunoTurma.edit(alunoTurmaAnterior, false);
                            if (alunoMovido != null && alunoMovido.cd_aluno_turma > 0)
                                dataAccessAlunoTurma.saveChanges(false);
                            else
                                dataAccessAlunoTurma.add(alunoTurmaDestino, false);
                        }
                    }

                if (mudanca.opcao == (int)FundacaoFisk.SGF.Web.Services.Secretaria.Model.MudancasInternas.OpcoesMudanca.RetornarTurmaOri)
                    foreach (AlunoTurma a in mudanca.alunos)
                    {
                        
                        int[] alunos = { a.cd_aluno };
                        AlunoTurma alunoTurmaAnterior = dataAccessAlunoTurma.findAlunosTurma(mudanca.cd_turma_origem, mudanca.cd_escola, alunos).FirstOrDefault();

                        if (!alunoTurmaAnterior.id_manter_contrato && alunoTurmaAnterior.cd_contrato > 0)
                            throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgErroRetornarAlunoMatricula), null, AlunoBusinessException.TipoErro.ERRO_ALUNO_COM_DIARIO, false);
                        //Verificar aulas dadas
                        if (alunoTurmaAnterior.nm_aulas_dadas > 0 || alunoTurmaAnterior.nm_faltas > 0)
                            throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgErroMudancaExisteEfento), null, AlunoBusinessException.TipoErro.ERRO_ALUNO_MATRICULADO_TURMA_ATUAL, false);


                        if (alunoTurmaAnterior.cd_contrato > 0)
                        {
                            List<HistoricoAluno> histAluno = DataAccessHistoricoAluno.GetHistoricosAlunoById(mudanca.cd_escola, a.cd_aluno, mudanca.cd_turma_origem, (int)alunoTurmaAnterior.cd_contrato).ToList();
                            if (histAluno != null && histAluno.Count() > 0)
                                foreach (HistoricoAluno hist in histAluno)
                                    DataAccessHistoricoAluno.delete(hist, false);
                        }
                        
                        AlunoTurma alunoTurmaOriginal = dataAccessAlunoTurma.findAlunosTurma((int)alunoTurmaAnterior.cd_turma_origem, mudanca.cd_escola, alunos).FirstOrDefault();
                        if (alunoTurmaOriginal.cd_contrato > 0)
                        {
                            HistoricoAluno histDeletar = DataAccessHistoricoAluno.GetHistoricoAlunoMovido(mudanca.cd_escola, a.cd_aluno, mudanca.cd_turma_destino, (int)alunoTurmaOriginal.cd_contrato);
                            if (histDeletar != null && histDeletar.cd_historico_aluno > 0)
                                DataAccessHistoricoAluno.delete(histDeletar, false);
                        }

                        alunoTurmaOriginal.dt_movimento = alunoTurmaAnterior.dt_movimento;
                        alunoTurmaOriginal.id_tipo_movimento = (int)HistoricoAluno.TipoMovimento.MATRICULA;
                        alunoTurmaOriginal.cd_situacao_aluno_turma = alunoTurmaAnterior.cd_situacao_aluno_origem;
                        if (!mudanca.id_ppt_origem)
                            dataAccessAlunoTurma.delete(alunoTurmaAnterior, false);
                        dataAccessAlunoTurma.edit(alunoTurmaOriginal, false);

                    }

                transaction.Complete();
            }
            return mudanca;

        }

        public void gerarHistoricoMudanca(AlunoTurma alunoTurmaAnterior, MudancasInternas mudanca, int cdAluno, int situacaoManterContrato)
        {
            HistoricoAluno historicoOrigem;
            HistoricoAluno historicoDestino;

            // Valida data de inicio do aluno na turma.
            ValidaData(mudanca.dt_movimentacao);
            if (alunoTurmaAnterior.Contrato != null)
            {
                Contrato contrato = alunoTurmaAnterior.Contrato;
                List<NotasVendaMaterialUI> Notas = new List<NotasVendaMaterialUI>();
                if (contrato.CursoContrato != null && contrato.CursoContrato.Count() > 0)
                    foreach (CursoContrato d in contrato.CursoContrato.Where(x => x.cd_curso == alunoTurmaAnterior.Turma.Curso.cd_curso).ToList())
                    {
                        int existeNota = BusinessFinanceiro.findNotaAluno(contrato.cd_aluno, d.cd_curso);
                        //Se existir Nota já gerada em outra matrícula não gera novamente
                        if (existeNota == 0 || (existeNota == 2 && (mudanca.id_ppt ? 2 : (int)contrato.cd_regime_atual) == 2))
                        {
                            if (contrato.notas_material_didatico != null && contrato.notas_material_didatico.Count() > 0)
                                Notas = contrato.notas_material_didatico.Where(x => x.cd_curso == d.cd_curso && !x.id_venda_futura).ToList();
                            if (Notas.Count() == 0 || existeNota == 2)
                            {
                                contrato.msgProcedureGerarNota = criaNotadeVendaMaterial(contrato.cd_contrato, d.cd_curso, mudanca.cd_usuario, mudanca.fusoHorario, mudanca.id_ppt ? 2 : (int)contrato.cd_regime_atual);

                                if (!contrato.msgProcedureGerarNota.Contains(SUCESSO))
                                    throw new MatriculaBusinessException(
                                        contrato.msgProcedureGerarNota, null, MatriculaBusinessException.TipoErro.ERRO_PROCEDURE_GERAR_NOTA, false);
                            }
                        }
                    }
            }
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //Incluindo o Histórico da SAIDA do aluno da turma de ORIGEM
                byte segHistorico = (byte)DataAccessHistoricoAluno.retunMaxSequenciaHistoricoAluno(mudanca.cd_produto, mudanca.cd_escola, cdAluno);
                int cdContrato = (int)alunoTurmaAnterior.cd_contrato;

                historicoOrigem = new HistoricoAluno
                {
                    cd_aluno = cdAluno,
                    cd_turma = mudanca.cd_turma_origem,
                    nm_sequencia = ++segHistorico,
                    cd_produto = mudanca.cd_produto,
                    cd_contrato = cdContrato,
                    dt_historico = mudanca.dt_movimentacao.Date,
                    dt_cadastro = DateTime.UtcNow,
                    id_tipo_movimento = Convert.ToByte(HistoricoAluno.TipoMovimento.MUDANCA_INTERNA),
                    id_situacao_historico = Convert.ToByte(FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Movido),
                    cd_usuario = mudanca.cd_usuario
                };

                if (mudanca.id_manter_contrato)
                {
                    // DESTINO
                    historicoDestino = new HistoricoAluno
                    {
                        cd_aluno = cdAluno,
                        cd_turma = mudanca.cd_turma_destino,
                        nm_sequencia = Convert.ToByte(segHistorico + 1),
                        cd_produto = mudanca.cd_produto,
                        cd_contrato = cdContrato,
                        dt_historico = mudanca.dt_movimentacao.Date,
                        dt_cadastro = DateTime.UtcNow,
                        id_tipo_movimento = Convert.ToByte(HistoricoAluno.TipoMovimento.MUDANCA_INTERNA),
                        id_situacao_historico = Convert.ToByte(situacaoManterContrato),
                        cd_usuario = mudanca.cd_usuario
                    };

                    DataAccessHistoricoAluno.add(historicoDestino, false);
                }
                DataAccessHistoricoAluno.add(historicoOrigem, false);
                transaction.Complete();
            }
        }
        public string criaNotadeVendaMaterial(int cd_contrato, int cd_curso, int cdUsuario, int fusoHorario, int cd_regime)
        {

            string ret = postGerarVenda(cd_contrato, cd_curso, cdUsuario, fusoHorario, cd_regime);

            return ret;

        }

        public string postGerarVenda(int? cd_turma, int? cd_curso, int? cd_usuario, int? fuso, int cd_regime)
        {
            string retorno = null;

            //A transacao será controlada na procedure
            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            //{
            retorno = DataAccessMatricula.postGerarVenda(cd_turma, cd_curso, cd_usuario, fuso, false, cd_regime);
            //transaction.Complete();

            //}
            return retorno;
        }


        private void ValidaData(DateTime dt_movimentacao)
        {
            DateTime dataMinValida = new DateTime(1900,01,01,0,0,0);
            DateTime dataMaxValida = new DateTime(2079, 06, 06,0,0,0);

            if (dt_movimentacao.Date < dataMinValida || dt_movimentacao.Date > dataMaxValida) 
            {
                if(dt_movimentacao.Year < 1900)
                    throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgErroDataMinInicioAlunoTurma), null, AlunoBusinessException.TipoErro.ERRO_DATA_INICIO_ALUNO_TURMA, false);
                if(dt_movimentacao.Year >= 2079)
                    throw new AlunoBusinessException(string.Format(Utils.Messages.Messages.msgErroDataMaxInicioAlunoTurma), null, AlunoBusinessException.TipoErro.ERRO_DATA_INICIO_ALUNO_TURMA, false);
            }
        }

        #endregion

        #region Bolsa

        public IEnumerable<RptBolsistas> getBolsistas(int cdEscola, int cd_aluno, int cd_turma, bool cancelamento, decimal? per_bolsa, int cd_motivo_bolsa, DateTime? dtIniComunicado,
                                                        DateTime? dtFimComunicado, DateTime? dtIni, DateTime? dtFim, bool periodo_ini, bool periodo_cancel)
        {
            List<RptBolsistas> listBolsistas = new List<RptBolsistas>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                listBolsistas = DataAccessAlunoBolsa.getBolsistas(cdEscola, cd_aluno, cd_turma, cancelamento, per_bolsa, cd_motivo_bolsa, dtIniComunicado, dtFimComunicado, dtIni, dtFim, periodo_ini, periodo_cancel).ToList();
                transaction.Complete();
            }
            return listBolsistas;
        }

        public void findByIdAndDeleteAlunoBolsa(int cd_aluno)
        {
            AlunoBolsa alunoBolsa = DataAccessAlunoBolsa.findById(cd_aluno, false);
            if (alunoBolsa != null)
                DataAccessAlunoBolsa.delete(alunoBolsa, false);
        }


        #endregion 

        #region Relatório de Médias
        public DataTable getRptMediaAlunos(int cd_escola, int cd_turma, int tipoTurma, int cdCurso, int cdProduto, int pesOpcao, int pesTipoAluno,
                decimal? vl_media, DateTime dtInicial, DateTime dtFinal)
        {
            //IEnumerable<sp_getRptMediaAlunos_Result> retorno = new List<sp_getRptMediaAlunos_Result>();
            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessAluno.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            //{
                return DataAccessAluno.getRptMediaAlunos(cd_escola, cd_turma, tipoTurma, cdCurso, cdProduto, pesOpcao, pesTipoAluno, vl_media, dtInicial, dtFinal);
                //transaction.Complete();
            //}
            //return retorno;
        }
        #endregion

        #region Relatório de Percentual de Término de Estágio

        public IEnumerable<ReportPercentualTerminoEstagio> getRptAlunosProximoCurso(int cd_escola, int cd_turma)
        {
            IEnumerable<ReportPercentualTerminoEstagio> retorno = new List<ReportPercentualTerminoEstagio>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessAluno.getRptAlunosProximoCurso(cd_escola, cd_turma);
                transaction.Complete();
            }
            return retorno;
        }

        #endregion        

        #region Horário

        public IEnumerable<Horario> getHorarioByEscolaForRegistroUncommited(int cdEscola, int cdRegistro, Horario.Origem origem)
        {
            List<Horario> listHorario = new List<Horario>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                listHorario = DataAccessHorario.getHorarioByEscolaForRegistro(cdEscola, cdRegistro, origem).ToList();
                transaction.Complete();
            }
            return listHorario;
        }

        public IEnumerable<Horario> getHorarioByEscolaForRegistro(int cdEscola, int cdRegistro, Horario.Origem origem)
        {
            return DataAccessHorario.getHorarioByEscolaForRegistro(cdEscola, cdRegistro, origem).ToList();
        }

        public bool deleteHorario(Horario horario)
        {
            horario = DataAccessHorario.findById(horario.cd_horario,false);
            return DataAccessHorario.delete(horario, false);
        }

        public Horario addHorario(Horario horario)
        {
            return DataAccessHorario.add(horario, false);
        }

        public Horario editHorarioContext(Horario horarioContext, Horario horarioView)
        {
            horarioContext = Horario.changeValueHorario(horarioContext, horarioView);
            DataAccessHorario.saveChanges(false);
            return horarioContext;
        }

        public IEnumerable<Horario> getHorarioOcupadosForTurma(int cdEscola, int cdRegistro, int[] cdProfessores, int cd_turma,
            int cd_duracao, int cd_curso, DateTime dt_inicio, DateTime? dt_final, HorarioDataAccess.TipoConsultaHorario tipoCons)
        {
            return DataAccessHorario.getHorarioOcupadosForTurma(cdEscola, cdRegistro, cdProfessores, cd_turma, cd_duracao, cd_curso, dt_inicio, dt_final, tipoCons);
        }

        public IEnumerable<Horario> getHorarioOcupadosForSala(Turma turma, int cdEscola, HorarioDataAccess.TipoConsultaHorario tipoCons)
        {
            return DataAccessHorario.getHorarioOcupadosForSala(turma, cdEscola, tipoCons);
        }

        public int countHorariosUsuario(int cd_empresa, int cd_usuario)
        {
            return DataAccessHorario.getHorarioByEscolaForRegistro(cd_empresa, cd_usuario, Horario.Origem.USUARIO).Count();
        }

        public bool getHorarioByHorario(int cdEscola, int cdRegistro, Horario.Origem origem, TimeSpan hr_servidor, int diaSemanaAtual)
        {
            return DataAccessHorario.getHorarioByHorario(cdEscola, cdRegistro, origem, hr_servidor, diaSemanaAtual);
        }

        public string retornaDescricaoHorarioOcupado(int cd_empresa, TimeSpan hr_ini, TimeSpan hr_fim)
        {
            return DataAccessHorario.retornaDescricaoHorarioOcupado(cd_empresa, hr_ini, hr_fim);
        }

        #endregion

        public List<sp_RptCartaQuitacao_Result> findAlunoCartaQuitacao(SearchParameters parametros, int cdEscola, int ano, int cdPessoa)
        {
            List<sp_RptCartaQuitacao_Result> retorno = new List<sp_RptCartaQuitacao_Result>();
            if (parametros.sort == null)
                parametros.sort = "no_responsavel";
            retorno = DataAccessAluno.findAlunoCartaQuitacao(parametros, cdEscola, ano, cdPessoa).ToList();
            return retorno;
        }

        public IEnumerable<AlunosemAulaUI> getAlunosemAula(SearchParameters parametros, int cd_aluno, int cd_item, int cdEscola, DateTime? dtaInicial, DateTime? dtaFinal, bool idMovimento, bool idHistorico, byte idSituacao)
        {
            IEnumerable<AlunosemAulaUI> retorno = new List<AlunosemAulaUI>();

            if (parametros.sort == null)
            {
                if (cdEscola == 0)
                    parametros.sort = "dc_reduzido_pessoa";
                else
                    parametros.sort = "no_pessoa";
            }
            parametros.sort = parametros.sort.Replace("dta_movimento", "dt_emissao_movimento");
            parametros.sort = parametros.sort.Replace("dta_historico", "dt_historico");
            parametros.sort = parametros.sort.Replace("Movimentacao", "id_situacao_historico");

            retorno = DataAccessAluno.getAlunosemAula(parametros, cd_aluno, cd_item, cdEscola, dtaInicial, dtaFinal, idMovimento, idHistorico, idSituacao);
            return retorno;
        }
        public IEnumerable<AlunosemAulaUI> getNotasDevolvidas(SearchParameters parametros, int cd_aluno, int cd_item, int cdEscola, DateTime? dtaInicial, DateTime? dtaFinal, bool idMovimento, bool idHistorico, byte idSituacao)
        {
            IEnumerable<AlunosemAulaUI> retorno = new List<AlunosemAulaUI>();

            if (parametros.sort == null)
            {
                if (cdEscola == 0)
                    parametros.sort = "dc_reduzido_pessoa";
                else
                    parametros.sort = "no_pessoa";
            }
            parametros.sort = parametros.sort.Replace("dta_movimento", "dt_emissao_movimento");
            parametros.sort = parametros.sort.Replace("dta_historico", "dt_historico");
            parametros.sort = parametros.sort.Replace("Movimentacao", "id_situacao_historico");

            retorno = DataAccessAluno.getNotasDevolvidas(parametros, cd_aluno, cd_item, cdEscola, dtaInicial, dtaFinal, idMovimento, idHistorico, idSituacao);
            return retorno;
        }
        public string postGerarKardexEntrada(int cd_item_movimento, int? cd_usuario, int? fuso)
        {
            string retorno = null;

            retorno = DataAccessAluno.postGerarKardexEntrada(cd_item_movimento, cd_usuario, fuso);
            return retorno;
        }

        public IEnumerable<AlunosCargaHorariaUI> getAlunoCargaHoraria(SearchParameters parametros, int cd_aluno, int cd_turma, int cdEscola, DateTime? dtaInicial, DateTime? dtaFinal)
        {
            IEnumerable<AlunosCargaHorariaUI> retorno = new List<AlunosCargaHorariaUI>();

            if (parametros.sort == null)
            {
               parametros.sort = "no_aluno";
            }
            parametros.sort = parametros.sort.Replace("dta_desistencia", "dt_desistencia");

            retorno = DataAccessAluno.getAlunosCargaHoraria(parametros, cd_aluno, cd_turma, cdEscola, dtaInicial, dtaFinal);
            return retorno;
        }

        public IEnumerable<CargaHorariaUI> getCargaHoraria(SearchParameters parametros, int cd_aluno, int cd_turma, int cd_curso, int cdEscola, int cd_professor, bool todasEscolas, int nm_aulas_vencimento)
        {
            IEnumerable<CargaHorariaUI> retorno = new List<CargaHorariaUI>();

            if (parametros.sort == null)
            {
                if (todasEscolas == true)
                {
                    parametros.sort = "no_escola";
                    
                }else
                {
                    parametros.sort = "no_aluno";
                }
                
            }
            if (parametros.sort == "dta_ultima_aula")
                parametros.sort = "dt_ultima_aula";

            retorno = DataAccessAluno.getCargaHoraria(parametros, cd_aluno, cd_turma, cd_curso, cdEscola, cd_professor, todasEscolas, nm_aulas_vencimento); ;
            return retorno;
        }

        public string postGerarNotaVoucher(int cd_desistencia, int cd_usuario, int fusoHorario, int itemVoucher)
        {
            string retorno = null;

            retorno = DataAccessAluno.postGerarNotaVoucher(cd_desistencia, cd_usuario, fusoHorario, itemVoucher);
            return retorno;
        }

        public bool getParametroEscolaInternacional(int cdEscola)
        {
            bool retorno = false;

            retorno = DataAccessAluno.getParametroEscolaInternacional(cdEscola);
            return retorno;
        }

        public ProfessorCargaHorariaMaximaResultUI getExisteCargaHorariaProximaMaxima(int cdPessoaUsuario, int cdEscola)
        {
            return DataAccessAluno.getExisteCargaHorariaProximaMaxima(cdPessoaUsuario, cdEscola);
        }


        public void deleteFichaAluno(int cd_aluno)
        {
            List<int> fichasAluno = DataAccessFichaSaude.findByAluno(cd_aluno).ToList();
            if (fichasAluno != null && fichasAluno.Count > 0)
            {
                for (int i = 0; i < fichasAluno.Count; i++)
                {
                    FichaSaude fichaDel = DataAccessFichaSaude.findById(fichasAluno[i], false);
                    if (fichaDel != null)
                    {
                        DataAccessFichaSaude.delete(fichaDel, false);
                    }
                }
            }
        }

    }
}
