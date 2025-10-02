using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Controllers;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using log4net;
using System.Data.Entity.Infrastructure;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Log.Comum.IBusiness;
using static FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess.ProgramacaoTurmaDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Business
{
    public class TurmaBusiness : ITurmaBusiness
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TurmaBusiness));
        
        public ITurmaDataAccess daoTurma { get; set; }
        public ISecretariaBusiness secretariaBiz { get; set; }
        public IProfessorTurmaDataAccess daoProfessorTurma { get; set; }
        public IProgramacaoTurmaDataAccess daoProgTurma { get; set; }
        private IProfessorBusiness professorBusiness { get; set; }
        public IAvaliacaoTurmaDataAccess daoAvaliacaoTurma { get; set; }
        public IAvaliacaoDataAccess daoAvaliacao { get; set; }
        public IAvaliacaoAlunoParticipacaoDataAccess daoAvaliacaoAlunoParticipacao { get; set; }
        public IAvaliacaoAlunoDataAccess daoAvaliacaoAluno { get; set; }
        public IAlunoBusiness alunoBiz { get; set; }
        public IFeriadoDesconsideradoDataAccess daoFeriadoDesconsiderado { get; set; }
        public IFinanceiroBusiness financeiroBiz { get; set; }
        public IDiarioAulaDataAccess daoDiarioAula { get; set; }
        public IFeriadoDataAccess daoFeriado { get; set; }
        private ILogGeralBusiness logGeralBusiness { get; set; }
        public IApiNewCyberBusiness BusinessApiNewCyber { get; set; }

        public TurmaBusiness(ITurmaDataAccess turmaDataAccess, ISecretariaBusiness secretariaBisiness, IFeriadoDataAccess feriadoDataAccess,
                           IProfessorTurmaDataAccess professorTurmaDataAcess, IProgramacaoTurmaDataAccess programacaoTurmaDataAccess, IDiarioAulaDataAccess diarioAulaDataAccess,
                           IProfessorBusiness professorBusiness, IAvaliacaoTurmaDataAccess daoAvaliacaoTurma, IAvaliacaoDataAccess daoAvaliacao, IAvaliacaoAlunoDataAccess daoAvaliacaoAluno,
                           IAlunoBusiness AlunoBiz, IFeriadoDesconsideradoDataAccess daoFeriadoDesconsiderado, IFinanceiroBusiness FinanceiroBiz,
                           IAvaliacaoAlunoParticipacaoDataAccess daoAvaliacaoAlunoParticipacao, ILogGeralBusiness LogGeralBusiness, IApiNewCyberBusiness businessApiNewCyber)
        {
            if (turmaDataAccess == null || secretariaBisiness == null || professorTurmaDataAcess == null || programacaoTurmaDataAccess == null
                || feriadoDataAccess == null || diarioAulaDataAccess == null || professorBusiness == null || daoAvaliacaoTurma == null || daoAvaliacao == null || daoAvaliacaoAluno == null
                || AlunoBiz == null || daoFeriadoDesconsiderado == null || FinanceiroBiz == null || daoAvaliacaoAlunoParticipacao == null || LogGeralBusiness == null || businessApiNewCyber == null)
                throw new ArgumentNullException("DAO");
            this.daoTurma = turmaDataAccess;
            this.secretariaBiz = secretariaBisiness;
            this.daoFeriado = feriadoDataAccess;
            this.daoProfessorTurma = professorTurmaDataAcess;
            this.daoProgTurma = programacaoTurmaDataAccess;
            this.daoDiarioAula = diarioAulaDataAccess;
            this.professorBusiness = professorBusiness;
            this.daoAvaliacaoTurma = daoAvaliacaoTurma;
            this.daoAvaliacao = daoAvaliacao;
            this.daoAvaliacaoAluno = daoAvaliacaoAluno;
            alunoBiz = AlunoBiz;
            this.daoFeriadoDesconsiderado = daoFeriadoDesconsiderado;
            this.financeiroBiz = FinanceiroBiz;
            this.daoAvaliacaoAlunoParticipacao = daoAvaliacaoAlunoParticipacao;
            this.logGeralBusiness = LogGeralBusiness;
            this.BusinessApiNewCyber = businessApiNewCyber;
        }

        public void sincronizaContexto(DbContext db)
        {
            //this.daoTurma.sincronizaContexto(db);
            //this.daoFeriado.sincronizaContexto(db);
            //this.daoProfessorTurma.sincronizaContexto(db);
            //this.daoProgTurma.sincronizaContexto(db);
            //this.daoDiarioAula.sincronizaContexto(db);
            //this.daoAvaliacaoTurma.sincronizaContexto(db);
            //this.daoAvaliacao.sincronizaContexto(db);
            //this.daoAvaliacaoAluno.sincronizaContexto(db);
            //this.daoFeriadoDesconsiderado.sincronizaContexto(db);
            //this.professorBusiness.sincronizarContextos(db);
            //this.secretariaBiz.sincronizarContextos(db);
            //this.alunoBiz.sincronizaContexto(db);
            //this.financeiroBiz.sincronizarContextos(db);
            //this.daoAvaliacaoAlunoParticipacao.sincronizaContexto(db);

            //logGeralBusiness.sincronizaContexto(db);
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa)
        {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.daoTurma.DB()).IdUsuario = ((SGFWebContext)this.daoFeriado.DB()).IdUsuario = 
            ((SGFWebContext)this.daoProfessorTurma.DB()).IdUsuario = ((SGFWebContext)this.daoProgTurma.DB()).IdUsuario = ((SGFWebContext)this.daoDiarioAula.DB()).IdUsuario =
            ((SGFWebContext)this.daoAvaliacaoTurma.DB()).IdUsuario = ((SGFWebContext)this.daoAvaliacao.DB()).IdUsuario = ((SGFWebContext)this.daoAvaliacaoAluno.DB()).IdUsuario =
            ((SGFWebContext)this.daoFeriadoDesconsiderado.DB()).IdUsuario = ((SGFWebContext)this.daoAvaliacaoAlunoParticipacao.DB()).IdUsuario =

            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.daoTurma.DB()).cd_empresa = ((SGFWebContext)this.daoFeriado.DB()).cd_empresa = 
            ((SGFWebContext)this.daoProfessorTurma.DB()).cd_empresa = ((SGFWebContext)this.daoProgTurma.DB()).cd_empresa = ((SGFWebContext)this.daoDiarioAula.DB()).cd_empresa =
            ((SGFWebContext)this.daoAvaliacaoTurma.DB()).cd_empresa = ((SGFWebContext)this.daoAvaliacao.DB()).cd_empresa = ((SGFWebContext)this.daoAvaliacaoAluno.DB()).cd_empresa =
            ((SGFWebContext)this.daoFeriadoDesconsiderado.DB()).cd_empresa = ((SGFWebContext)this.daoAvaliacaoAlunoParticipacao.DB()).cd_empresa = cd_empresa;

            secretariaBiz.configuraUsuario(cdUsuario, cd_empresa);
            professorBusiness.configuraUsuario(cdUsuario, cd_empresa);
            alunoBiz.configuraUsuario(cdUsuario, cd_empresa);
            financeiroBiz.configuraUsuario(cdUsuario, cd_empresa);
            logGeralBusiness.configuraUsuario(cdUsuario, cd_empresa);
            BusinessApiNewCyber.configuraUsuario(cdUsuario, cd_empresa);

        }

        #region turma

        public IEnumerable<TurmaSearch> searchTurma(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto,
                                                    int situacaoTurma, int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola,
                                                    bool turmasFilhas, int cdAluno, int origemFK, DateTime? dtInicial, DateTime? dtFinal, int? cd_turma_PPT, bool semContrato, int tipoConsulta, 
                                                    DateTime? dt_inicial, DateTime? dt_final, bool ProfTurmasAtuais, int cd_search_sala, int cd_search_sala_online, bool ckSearchSemSala,
                                                    bool ckSearchSemAluno, List<int> cdSituacoesAlunoTurma, int cd_escola_combo_fk, int diaSemanaTurma, int ckOnLine, string dias)
        {
            IEnumerable<TurmaSearch> retorno = new List<TurmaSearch>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, daoTurma.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_turma";
                parametros.sort = parametros.sort.Replace("dtaIniAula", "dt_inicio_aula");

                retorno = daoTurma.searchTurma(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, tipoProg, cdEscola, turmasFilhas, cdAluno, origemFK, dtInicial, dtFinal, cd_turma_PPT, semContrato, tipoConsulta, dt_inicial, dt_final, ProfTurmasAtuais, cd_search_sala, cd_search_sala_online, ckSearchSemSala, ckSearchSemAluno, null, cd_escola_combo_fk, diaSemanaTurma, ckOnLine, dias);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<TurmaSearch> getTurmaSearchAulaReposicaoDestinoFK(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto,
            int situacaoTurma, int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola,
            bool turmasFilhas, int cdAluno, int origemFK, DateTime? dtInicial, DateTime? dtFinal, int? cd_turma_PPT, bool semContrato, int tipoConsulta,
            DateTime? dt_inicial, DateTime? dt_final, bool ProfTurmasAtuais, DateTime? dt_programacao, int cd_estagio, int cd_turma_origem, List<int> cdSituacoesAlunoTurma, bool ckOnLine)
        {
            IEnumerable<TurmaSearch> retorno = new List<TurmaSearch>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_turma";
                parametros.sort = parametros.sort.Replace("dtaIniAula", "dt_inicio_aula");

                retorno = daoTurma.getTurmaSearchAulaReposicaoDestinoFK(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, tipoProg, cdEscola, turmasFilhas, cdAluno, origemFK, dtInicial, dtFinal, cd_turma_PPT, semContrato, tipoConsulta, dt_inicial, dt_final, ProfTurmasAtuais, dt_programacao, cd_estagio, cd_turma_origem, null, ckOnLine);
                transaction.Complete();
            }
            return retorno;
        }



        public IEnumerable<Turma> findTurma(int idTurma, int idEscola)
        {
            return daoTurma.findTurma(idTurma, idEscola);
        }
        public IEnumerable<Turma> getTurmaPoliticaEsc(int cdEscola)
        {
            return daoTurma.getTurmaPoliticaEsc(cdEscola);
        }     
        public IEnumerable<Turma> findTurma(int cdEscola, int? cd_turma, TurmaDataAccess.TipoConsultaTurmaEnum tipo)
        {
            return daoTurma.findTurma(cdEscola, cd_turma, tipo);
        }

        private string verificaSiglaDia(byte dia)
        {
            string sigla = "";
            switch (dia)
            {
                case 1:
                    sigla = "DOM";
                    break;
                case 2:
                    sigla = "SEG";
                    break;
                case 3:
                    sigla = "TER";
                    break;
                case 4:
                    sigla = "QUA";
                    break;
                case 5:
                    sigla = "QUI";
                    break;
                case 6:
                    sigla = "SEX";
                    break;
                case 7:
                    sigla = "SAB";
                    break;
            }
            return sigla;
        }

        public string criarNomeTurma(string inicioNome, List<Horario> listahorario, DateTime inicioAulas)
        {

            //Dias da Semana (Pegar iniciais)
            string diasSemana = null;
            TimeSpan? horaIni = null;
            TimeSpan? horaFim = null;
            TimeSpan dtaHoraIni;
            TimeSpan dtaHoraFim;
            byte[] verDiaSemana = new byte[listahorario.Count];

            //ordenando a lista por dia da semana, para a nomeclatura não ficar bagunçada
            if (listahorario != null)
            {
                List<Horario> lista = listahorario.OrderBy(d => d.id_dia_semana).ToList();
                if (lista.Count() > 0)
                {
                    diasSemana = verificaSiglaDia(lista[0].id_dia_semana);
                    verDiaSemana[0] = lista[0].id_dia_semana;
                    if (lista.Count > 1)
                        for (int i = 1; i < lista.OrderBy(d => d.id_dia_semana).ToList().Count; i++)
                            if (!verDiaSemana.Contains(lista[i].id_dia_semana))
                            {
                                diasSemana = diasSemana + "/" + verificaSiglaDia(lista[i].id_dia_semana);
                                verDiaSemana[i] = lista[i].id_dia_semana;
                            }

                    Horario horario_inicio = lista.Where(h => h.id_dia_semana == lista[0].id_dia_semana).OrderBy(hI => hI.dt_hora_ini).FirstOrDefault();
                    Horario horario_fim = lista.Where(h => h.id_dia_semana == lista[0].id_dia_semana).OrderByDescending(hI => hI.dt_hora_fim).FirstOrDefault();
                    dtaHoraIni = horario_inicio.dt_hora_ini;
                    dtaHoraFim = horario_fim.dt_hora_fim;
                    // Horario
                    horaIni = new TimeSpan(dtaHoraIni.Hours, dtaHoraIni.Minutes, 0);
                    horaFim = new TimeSpan(dtaHoraFim.Hours, dtaHoraFim.Minutes, 0);
                }
            }
            // Semestre (1 ou 2S)
            var semestre = inicioAulas.Month > 6 ? "2S" : "1S";

            // Ano Inicio Turma
            var anoInicio = inicioAulas.Year;
            var semAno = inicioAulas != null ? "-" + semestre + "/" + anoInicio.ToString().Substring(2, 2) : null;
            string horarioNomeTurma = null;

            if (horaIni != null && horaFim != null && horaIni.ToString().Length >= 5 && horaFim.ToString().Length >= 5)
                horarioNomeTurma = diasSemana != "" ? "-" + diasSemana + "-" + horaIni.ToString().Substring(0, 5) + "/" + horaFim.ToString().Substring(0, 5) : "";

            // Sequência (Para quando tiver mais de uma turma com os mesmos dados)
            string nomeTurma = inicioNome + horarioNomeTurma + semAno;
            return nomeTurma;
        }

        public Turma addTurma(Turma turma)
        {
            this.sincronizaContexto(daoTurma.DB());
            var totalAluno = 0;
            if (!turma.id_turma_ppt)
            {
                totalAluno = turma.alunosTurma.ToList().Count();
                consistirVagas(turma, totalAluno);
            }
            turma = daoTurma.add(turma, false);
            return turma;
        }

        //Regra para vagas de alunos no curso - Nota: so deve persistir a turma se tiver vagas conforme o número de vagas no curso.
        private void consistirVagas(Turma turma, int alunos)
        {
            string descricao = "curso.";
            var curso = (int)TurmaDataAccess.TipoConsultaTurmaEnum.CURSO;
            var vagasCuso = 0;
            if (turma.cd_curso != null)
                vagasCuso = daoTurma.getNumeroVagas((int)turma.cd_curso, curso, turma.cd_pessoa_escola);
            if (vagasCuso < alunos)
                throw new TurmaBusinessException(string.Format(Messages.msgNumeroVagas, descricao), null, TurmaBusinessException.TipoErro.ERRO_NUMERO_VAGAS, false);
        }

        public Turma findTurmasByIdAndCdEscola(int cdTurma, int cdEscola)
        {
            return daoTurma.findTurmasByIdAndCdEscola(cdTurma, cdEscola);
        }

        public Turma findTurmaByCdTurmaApiCyber(int cdTurma)
        {
            return daoTurma.findTurmaByCdTurmaApiCyber(cdTurma);
        }


        //public Turma findTurmasByIdAndCdEscola(int cdTurma, int cdEscola)
        //{
        //    Turma retorno = null;
        //    using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, daoTurma.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
        //    {
        //        retorno = daoTurma.findTurmasByIdAndCdEscola(cdTurma, cdEscola);
        //        transaction.Complete();
        //    }
        //    return retorno;
        //}

        public Turma editTurma(Turma turma, Turma turmaContext)
        {
            this.sincronizaContexto(daoTurma.DB());

            if (turma == null)
                throw new TurmaBusinessException(Messages.msgErroTurmaNulo, null, TurmaBusinessException.TipoErro.ERRO_TURMA_NULO, false);

            if (turma.alunosTurmaEscola != null && turma.alunosTurmaEscola.Count() > 0)
            {
                List<AlunoTurma> listaTurmaPPT = turma.alunosTurmaEscola.ToList();
                if (!turma.id_turma_ativa && turma.id_turma_ppt)
                {
                    if (listaTurmaPPT != null && listaTurmaPPT.Count > 0)
                    {
                        bool hasTurmaAndamento = daoTurma.HasTurmasEmAndamento(listaTurmaPPT.Select(x => x.cd_turma).ToList());
                        if (hasTurmaAndamento)
                        {
                            throw new TurmaBusinessException(string.Format(Utils.Messages.Messages.msgErroInativarTurma), null, TurmaBusinessException.TipoErro.ERRO_EXISTETURMAATIVA, false);
                        }
                    }

                }
                    
            }
            //Atualizando data de inicio de aluno turma 

            //Verificar o nr de vagas apenas para alunos ativos/rematrículados
            int qtdAlunoAtivosRemat = 0;
            if (turma.alunosTurmaEscola != null && turma.alunosTurmaEscola.Count() > 0)
                foreach (AlunoTurma at in turma.alunosTurmaEscola)
                    if (at.cd_situacao_aluno_turma == (int)Turma.SituacaoAlunoTurma.REMATRICULADO || at.cd_situacao_aluno_turma == (int)Turma.SituacaoAlunoTurma.ATIVO)
                        qtdAlunoAtivosRemat += 1;

            if (!turma.id_turma_ppt && qtdAlunoAtivosRemat > 0)
                consistirVagas(turma, qtdAlunoAtivosRemat);
            if (turma.alunosTurma != null && turma.alunosTurma.Count() > 0 && turmaContext.dt_inicio_aula != turma.dt_inicio_aula && turmaContext.cd_pessoa_escola == turma.cd_pessoa_escola)
            {
                int j = 0;
                int[] cdAlunos = new int[turma.alunosTurma.Count()];
                foreach (AlunoTurma a in turma.alunosTurma)
                {
                    cdAlunos[j] = a.cd_aluno;
                    j++;
                }
                IEnumerable<AlunoTurma> alunosTurma = secretariaBiz.findAlunosTurma(turma.cd_turma, turma.cd_pessoa_escola, cdAlunos);
                foreach (AlunoTurma at in alunosTurma)
                {
                    if (at.cd_contrato.HasValue)
                    {
                        Contrato contrato = alunoBiz.getMatriculaByTurmaAlunoHistorico(turma.cd_pessoa_escola, at.cd_aluno, (int)at.cd_contrato);
                        if (contrato != null)
                        {
                            HistoricoAluno historico = secretariaBiz.getHistoricoAlunoByMatricula(turma.cd_pessoa_escola, at.cd_aluno, at.cd_turma, (int)at.cd_contrato);
                            if (historico != null)
                            {
                                historico.dt_historico = contrato.dt_matricula_contrato > turma.dt_inicio_aula ? Convert.ToDateTime(contrato.dt_matricula_contrato).Date : Convert.ToDateTime(turma.dt_inicio_aula).Date;
                                if (contrato.dt_inicial_contrato > historico.dt_historico)
                                    historico.dt_historico = Convert.ToDateTime(contrato.dt_inicial_contrato).Date;
                                secretariaBiz.saveHistoricoAluno();
                            }
                        }
                    }
                    at.dt_inicio = turma.dt_inicio_aula.Date;
                }
            }
            if (turmaContext.cd_pessoa_escola == turma.cd_pessoa_escola)
            {
                turmaContext = Turma.changeValueTurma(turmaContext, turma);
                turmaContext.dt_inicio_aula = turmaContext.dt_inicio_aula.Date;
                daoTurma.saveChanges(false);
            }
            return turmaContext;
        }

        public List<TurmaSearch> editTurmaEncerramento(List<Turma> turmas, int cd_usuario, int fuso)
        {
            Turma turma = new Turma();
            List<TurmaSearch> turmasAlteradas = new List<TurmaSearch>();
            List<TurmaApiCyberBdUI> listaTurmaApiCyber = new List<TurmaApiCyberBdUI>();
            var dc_turma = String.Join("|", turmas.Select(t => t.cd_turma));
            this.sincronizaContexto(daoTurma.DB());
            //Chamar procedure
            List<int?> cdTurmas = daoTurma.TurmaEncLista(dc_turma, turmas[0].dt_termino_turma, cd_usuario, fuso);
            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, daoTurma.DB()))
            //{
            foreach (Turma t in turmas)
            {
                turma = daoTurma.findById(t.cd_turma, false);
                //if (turma.dt_termino_turma.HasValue)
                //    throw new TurmaBusinessException(Messages.msgErrorTurmaEncerrado, null, TurmaBusinessException.TipoErro.ERRO_TURMA_ENCERRADA, false);
                //else
                //    if (turma.cd_turma > 0)
                //    {
                //        //Atualiza a programação
                //        List<ProgramacaoTurma> progBd = daoProgTurma.getProgramacaoTurmaEditEncerramentoTurma(t.cd_turma, (DateTime)t.dt_termino_turma);
                //        if (progBd != null && progBd.Count > 0)
                //        {
                //            progBd.ForEach(x => x.id_prog_cancelada = true);
                //            daoProgTurma.saveChanges(false);
                //        }

                //        List<AlunoTurma> alunosTurma = secretariaBiz.findAlunosTurmaForEncerramento(turma.cd_turma, turma.cd_pessoa_escola);
                //        if (alunosTurma != null && alunosTurma.Count() > 0)
                //            foreach (AlunoTurma at in alunosTurma)
                //            {
                //                if (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo || at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                //                {
                //                    at.cd_situacao_aluno_turma = (int)AlunoTurma.SituacaoAlunoTurma.Encerrado;

                //                    //Geração do histórico para o aluno
                //                    if (at.cd_contrato != null && at.cd_contrato > 0)
                //                        insertHistorico(cd_usuario, at.Aluno.cd_pessoa_escola, turma.cd_produto, (DateTime)t.dt_termino_turma, turma.cd_turma, at.cd_aluno, (int)at.cd_contrato);
                //                }
                //                if (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando && (at.cd_contrato == null || at.cd_contrato == 0))
                //                {
                //                    if (turma.cd_turma_ppt > 0){
                //                        throw new TurmaBusinessException(Messages.msgErroTurmaPPTAguard, null, TurmaBusinessException.TipoErro.ERRO_TURMA_ENCERRADA, false);
                //                    }
                //                    else
                //                        secretariaBiz.deletarAlunoTurma(at);
                //                }
                //            }
                //        turma.dt_termino_turma = t.dt_termino_turma;
                //        daoTurma.saveChanges(false);
                //        turmasAlteradas.Add(getTurmaByCodMudancaOuEncerramento(cdTurma));

                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    //Pega a turma encerrada e alimenta a lista para fazer as requisicoes na api cyber
                    getTurmaEncerradaApiCyber(turma, listaTurmaApiCyber);
                }

                //}
            }

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, daoTurma.DB()))
            {
                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    //monta o objeto alunoCyber para chamar apicyber
                    foreach (TurmaApiCyberBdUI turmaApiCyberBdUi in listaTurmaApiCyber)
                    {
                        if (turmaApiCyberBdUi != null && (turmaApiCyberBdUi.id_unidade != null && turmaApiCyberBdUi.id_unidade > 0) &&
                            existeGrupoByCodigoGrupo(turmaApiCyberBdUi.codigo))
                        {
                            //Chama api cyber com o comando (INATIVA_GRUPO)
                            InativaGruposApiCyber(turmaApiCyberBdUi, ApiCyberComandosNames.INATIVA_GRUPO);
                        }

                    }
                }

                transaction.Complete();
            }
            turmasAlteradas = getTurmaByCodigosEncerramento(cdTurmas, turmas[0].cd_pessoa_escola);
            return turmasAlteradas;
        }

        public bool existeGrupoByCodigoGrupo(int codigo_grupo)
        {
            return BusinessApiNewCyber.verificaRegistroGrupos(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.VISUALIZA_GRUPO, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo_grupo);
        }

        private void getTurmaEncerradaApiCyber(Turma turma, List<TurmaApiCyberBdUI> listaTurmaApiCyber)
        {
            TurmaApiCyberBdUI turmaApiCyberCurrent = new TurmaApiCyberBdUI();
            turmaApiCyberCurrent = findTurmaByCdTurmaAndCdEscolaApiCyber(turma.cd_turma, turma.cd_pessoa_escola);

            //se for regular ou ppt filha e tiver nm_integracao 
            if ((turmaApiCyberCurrent.id_turma_ppt == false && turmaApiCyberCurrent.cd_turma_ppt == null && turmaApiCyberCurrent.id_unidade != null) ||
                (turmaApiCyberCurrent.id_turma_ppt == false && (turmaApiCyberCurrent.cd_turma_ppt != null && turmaApiCyberCurrent.cd_turma_ppt > 0) && turmaApiCyberCurrent.id_unidade != null)
                )
            {
                listaTurmaApiCyber.Add(turmaApiCyberCurrent);
            }
        }

        private void InativaGruposApiCyber(TurmaApiCyberBdUI turmaApiCyberCurrent, string comando)
        {
            string parametros = "";
            //Valida e retorna os parametros da requisicao
            parametros = validaParametrosCyberAtivaInativa(turmaApiCyberCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");

            //Chama a apiCyber
            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);

        }

        private void AtivaGruposApiCyber(TurmaApiCyberBdUI turmaApiCyberCurrent, string comando)
        {
            string parametros = "";

            parametros = validaParametrosCyberAtivaInativa(turmaApiCyberCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");

            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);

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


        private void insertHistorico(int cd_usuario, int cd_pessoa_escola, int cd_produto, DateTime dataTermino, int cd_turma, int cd_aluno, int cd_contrato)
        {
            byte sequenciaMax = (byte)secretariaBiz.retunMaxSequenciaHistoricoAluno(cd_produto, cd_pessoa_escola, cd_aluno);
            HistoricoAluno historico = new HistoricoAluno {
                cd_aluno = cd_aluno,
                cd_turma = cd_turma,
                cd_contrato = cd_contrato,
                id_tipo_movimento = (int) HistoricoAluno.TipoMovimento.ENCERRADO,
                dt_cadastro = DateTime.UtcNow,
                dt_historico = dataTermino.Date,
                cd_produto = cd_produto,
                nm_sequencia = ++sequenciaMax,
                id_situacao_historico = (int)HistoricoAluno.SituacaoHistorico.ENCERRADO,
                cd_usuario = cd_usuario
            };
            secretariaBiz.addHistoricoAluno(historico);
        }

        public TurmaSearch getTurmaByCodForGrid(int cdTurma, int cdEscola, bool turmasFilha)
        {
            TurmaSearch turmaCont = new TurmaSearch();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                turmaCont = daoTurma.getTurmaByCodForGrid(cdTurma, cdEscola, turmasFilha);
                transaction.Complete();
            }
            return turmaCont;
        }

        public TurmaSearch getTurmaByCodMudancaOuEncerramento(int cdTurma, int cdEscola)
        {
            TurmaSearch retorno = null;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, daoTurma.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = daoTurma.getTurmaByCodMudancaOuEncerramento(cdTurma, cdEscola);
                transaction.Complete();
            }
            return retorno;
        }
        public List<TurmaSearch> getTurmaByCodigosEncerramento(List<int?> cdTurma, int cdEscola)
        {
            List<TurmaSearch> retorno = null;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, daoTurma.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = daoTurma.getTurmaByCodigosEncerramento(cdTurma, cdEscola);
                transaction.Complete();
            }
            return retorno;
        }

        public bool deleteTurmas(int[] turmas, int cd_escola)
        {
            bool retorno = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED,daoTurma.DB(), TransactionScopeBuilder.TransactionTime.DEFAULT))
            {
                this.sincronizaContexto(daoTurma.DB());
                if (turmas != null && turmas.Count() > 0)
                {
                    List<Turma> turmasContext = daoTurma.getTurmasByCod(turmas, cd_escola);


                    List<TurmaApiCyberBdUI> listaTurmaApiCyber = new List<TurmaApiCyberBdUI>();

                    foreach (var turma in turmasContext)
                    {
                        if (BusinessApiNewCyber.aplicaApiCyber())
                        {
                            preencheListaTurmasDeleteApiCyber(turma, listaTurmaApiCyber);
                        }

                        if (turma.cd_turma_ppt != null && turma.cd_turma_ppt > 0)
                        {
                            AlunoTurma alunoTurma = secretariaBiz.findAlunosTurmaPorTurmaEscola(turma.cd_turma, cd_escola).FirstOrDefault();
                            if (alunoTurma != null && alunoTurma.cd_aluno > 0)
                            {
                                if (alunoTurma.cd_contrato != null && alunoTurma.cd_contrato > 0)
                                    throw new TurmaBusinessException(Messages.msgNotDeletedAlunoTurmaFilhaPPTExisteContrato, null,
                                        TurmaBusinessException.TipoErro.ERRO_DELETAR_TURMA_ALUNO_COM_CONTRATO, false);
                                if (alunoTurma.cd_situacao_aluno_turma == (int)Turma.SituacaoAlunoTurma.AGUARDANDO && alunoTurma.cd_turma_origem > 0)
                                    throw new TurmaBusinessException(string.Format(Utils.Messages.Messages.msgNotExcAlunoTurmaAguardMovido), null, TurmaBusinessException.TipoErro.ERRO_DELETAR_ALUNOTURMA_AGUARDANDO_MOVIDO, false);
                                secretariaBiz.deletarAlunoTurma(alunoTurma);
                            }
                        }
                        if (daoTurma.verificaTurmaSeExisteAlunoComSitacaoDifAguard(turma.cd_turma, turma.cd_pessoa_escola))
                            throw new TurmaBusinessException(Messages.msgNotDeletedAlunoTurmaExisteContrato, null,
                                        TurmaBusinessException.TipoErro.ERRO_DELETAR_TURMA_ALUNO_COM_CONTRATO, false);
                        crudHorariosTurma(new List<Horario>(), turma.cd_turma, turma.cd_pessoa_escola, Turma.TipoTurma.TODAS);
                        retorno = daoTurma.delete(turma, false);

                        
                    }

                    if (BusinessApiNewCyber.aplicaApiCyber())
                    {
                        foreach (TurmaApiCyberBdUI turmaApiCyberBdUi in listaTurmaApiCyber)
                        {
                            if (turmaApiCyberBdUi != null && (turmaApiCyberBdUi.id_unidade != null && turmaApiCyberBdUi.id_unidade > 0) &&
                                existeGrupoByCodigoGrupo(turmaApiCyberBdUi.codigo))
                            {
                                //Chama api cyber com o comando (INATIVA_GRUPO)
                                InativaGruposApiCyber(turmaApiCyberBdUi, ApiCyberComandosNames.INATIVA_GRUPO);
                            }

                        }
                    }
                }
                transaction.Complete();
            }
            return retorno;
        }

        private void preencheListaTurmasDeleteApiCyber(Turma turma, List<TurmaApiCyberBdUI> listaTurmaApiCyber)
        {
            TurmaApiCyberBdUI turmaApiCyberCurrent = new TurmaApiCyberBdUI();
            turmaApiCyberCurrent = findTurmaByCdTurmaAndCdEscolaApiCyber(turma.cd_turma, turma.cd_pessoa_escola);

            //se for regular ou ppt pai e tiver nm_integracao
            if ((turmaApiCyberCurrent.id_turma_ppt == false && turmaApiCyberCurrent.cd_turma_ppt == null && turmaApiCyberCurrent.id_unidade != null) ||
                (turmaApiCyberCurrent.id_turma_ppt == true && turmaApiCyberCurrent.cd_turma_ppt == null && turmaApiCyberCurrent.id_unidade != null))
            {
                listaTurmaApiCyber.Add(turmaApiCyberCurrent);
            }
        }


        public Turma buscarTurmaHorariosEdit(int cdTurma, int cdEscola, TurmaDataAccess.TipoConsultaTurmaEnum tipo)
        {
            Turma retorno = new Turma();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoTurma.buscarTurmaHorariosEdit(cdTurma, cdEscola, tipo);
                transaction.Complete();
            }
            return retorno;
        }
        public Turma buscarTurmaHorariosNovaVirada(int cdTurma, int cdEscola, TurmaDataAccess.TipoConsultaTurmaEnum tipo)
        {
            Turma retorno = new Turma();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoTurma.buscarTurmaHorariosNovaVirada(cdTurma, cdEscola, tipo);
                transaction.Complete();
            }
            return retorno;
        }

        public List<Turma> getTurmasByCod(int[] cdTurmas, int cd_escola)
        {
            return daoTurma.getTurmasByCod(cdTurmas, cd_escola);
        }

        public void crudHorariosTurma(List<Horario> horariosView, int cd_turma, int cd_escola, Turma.TipoTurma tipoTurma)
        {
            this.sincronizaContexto(daoTurma.DB());
            Horario horario = new Horario();
            List<Horario> horarioContext = secretariaBiz.getHorarioByEscolaForRegistro(cd_escola, cd_turma, Horario.Origem.TURMA).ToList();
            IEnumerable<Horario> horariosTurmaComCodigo = from hpts in horariosView
                                                          where hpts.cd_horario != 0
                                                          select hpts;
            IEnumerable<Horario> horariosTurmaDeleted = horarioContext.Where(tc => !horariosTurmaComCodigo.Any(tv => tc.cd_horario == tv.cd_horario));

            if (horariosTurmaDeleted != null)
                foreach (var item in horariosTurmaDeleted)
                {
                    //for(int i=0; horariosTurmaDeleted != null && i<horariosTurmaDeleted.ToList().Count; i++)
                    if (item != null)
                        secretariaBiz.deleteHorario(item);
                }

            foreach (var item in horariosView)
            {
                Horario retorno = null;

                // Novos horários da turma:
                if (item.cd_horario == 0)
                {
                    if (item.endTime != DateTime.MinValue && item.startTime != DateTime.MinValue)
                    {
                        item.endTime = item.endTime.ToLocalTime();
                        item.startTime = item.startTime.ToLocalTime();
                        item.dt_hora_ini = new TimeSpan(item.startTime.Hour, item.startTime.Minute, 0);
                        item.dt_hora_fim = new TimeSpan(item.endTime.Hour, item.endTime.Minute, 0);
                    }
                    item.cd_pessoa_escola = cd_escola;
                    item.cd_registro = cd_turma;
                    item.id_origem = (int)Horario.Origem.TURMA;
                    retorno = secretariaBiz.addHorario(item);

                }
                //Alteração dos horários da turma:
                else
                {
                    var horarioTurma = horarioContext.Where(hc => hc.cd_horario == item.cd_horario).FirstOrDefault();
                    if (horarioTurma != null && horarioTurma.cd_horario > 0)
                    {
                        item.endTime.ToLocalTime();
                        item.startTime.ToLocalTime();
                        item.cd_registro = cd_turma;
                        item.cd_pessoa_escola = cd_escola;
                        item.id_origem = (int)Horario.Origem.TURMA;

                        retorno = secretariaBiz.editHorarioContext(horarioTurma, item);
                        if (retorno.HorariosProfessores != null)
                            crudHorarioProfessorTurma(item, tipoTurma, cd_turma);
                    }
                }
            }
        }

        public void crudAlunosTurma(List<AlunoTurma> alunoTurmaView, Turma turma)
        {
            this.sincronizaContexto(daoTurma.DB());
            List<AlunoTurma> alunoTurmaContext = secretariaBiz.findAlunosTurmaPorTurmaEscola(turma.cd_turma, turma.cd_pessoa_escola).ToList();
            alunoTurmaView.ForEach(x =>
            {
                if (x.cd_aluno_turma == 0)
                {
                    AlunoTurma alunoteste = alunoTurmaContext.Where(t => t.cd_aluno == x.cd_aluno && t.cd_turma == x.cd_turma).FirstOrDefault();
                    if (alunoteste != null)
                    {
                        x.cd_aluno_turma = alunoteste.cd_aluno_turma;
                    }
                }
            });
            IEnumerable<AlunoTurma> alunosTurmaComCodigo = from alunoT in alunoTurmaView
                                                           where alunoT.cd_aluno_turma != 0
                                                           select alunoT;
            IEnumerable<AlunoTurma> alunosTurmaDeleted = alunoTurmaContext.Where(tc => !alunosTurmaComCodigo.Any(tv => tc.cd_aluno_turma == tv.cd_aluno_turma));
            if (alunosTurmaDeleted.Count() > 0)
            {
                foreach (var item in alunosTurmaDeleted)
                {
                    if (item != null)
                    {
                        if (item.cd_situacao_aluno_turma == (int)Turma.SituacaoAlunoTurma.AGUARDANDO && item.cd_turma_origem > 0)
                            throw new TurmaBusinessException(string.Format(Utils.Messages.Messages.msgNotExcAlunoTurmaAguardMovido), null, TurmaBusinessException.TipoErro.ERRO_DELETAR_ALUNOTURMA_AGUARDANDO_MOVIDO, false);

                        if (item.cd_situacao_aluno_turma != (int)Turma.SituacaoAlunoTurma.AGUARDANDO)
                            throw new TurmaBusinessException(String.Format(Messages.msgNotExcAlunoTurmaMat), null, TurmaBusinessException.TipoErro.ERRO_DELETAR_ALUNOTURMA_MATRICULADO_REMATRICULADO, false);
                        bool existDiarioAula = daoDiarioAula.verificaTurmaDiarioAulaLancado(turma.cd_turma, turma.cd_pessoa_escola);
                        if (existDiarioAula && item.cd_situacao_aluno_turma != (int)AlunoTurma.SituacaoAlunoTurma.Aguardando)
                            throw new TurmaBusinessException(string.Format(Utils.Messages.Messages.msgExistsDiarioAulaLancado), null, TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA_TURMA, false);
                        secretariaBiz.deletarAlunoTurma(item);
                    }
                }
            }
            //DateTime? dataInicio = new DateTime();
            foreach (var item in alunoTurmaView)
            {
                // dataInicio = Convert.ToDateTime(turma.dt_inicio_aula);
                if (item.cd_aluno_turma.Equals(null) || item.cd_aluno_turma == 0)
                {
                    item.cd_situacao_aluno_turma = (int)AlunoTurma.SituacaoAlunoTurma.Aguardando;
                    item.cd_turma = turma.cd_turma;
                    item.dt_inicio = ((DateTime)turma.dt_inicio_aula).Date;
                    item.dt_movimento = DateTime.UtcNow;
                    secretariaBiz.addAlunoTurma(item);
                }
            }
        }

        public IEnumerable<ProfessorTurma> findProfessoresTurmaPorTurmaEscola(int cdTurma, int cd_escola)
        {
            return daoProfessorTurma.findProfessoresTurmaPorTurmaEscola(cdTurma, cd_escola);
        }

        public void deleteProfPPTFilhaMudancaInterna(int cd_turma, int cd_escola)
        {
            List<ProfessorTurma> professorTurmaContext = daoProfessorTurma.findProfessoresTurmaPorTurmaEscola(cd_turma, cd_escola).ToList();
            foreach (var item in professorTurmaContext)
                if (item != null)
                {
                    bool existeProg = false;
                    existeProg = daoDiarioAula.verificaDiarioAulaEfetivadoProf(cd_turma, item.cd_professor, cd_escola);
                    if (existeProg)
                        throw new TurmaBusinessException(string.Format(Messages.msgRemoveProfDiarioAula), null, TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA_TURMA, false);
                    daoProfessorTurma.delete(item, false);
                }
        }

        public void crudProfessoresTurma(List<ProfessorTurma> professorTurmaView, Turma turma, IEnumerable<Horario> horarios, bool turmaPPTFilha)
        {
            this.sincronizaContexto(daoProfessorTurma.DB());
            List<ProfessorTurma> professorTurmaContext = daoProfessorTurma.findProfessoresTurmaPorTurmaEscola(turma.cd_turma, turma.cd_pessoa_escola).ToList();
            IEnumerable<ProfessorTurma> professoresTurmaComCodigo = from profT in professorTurmaView
                                                                    where profT.cd_professor_turma != 0
                                                                    select profT;

            if (horarios != null)
            {
                if (professorTurmaContext != null)
                {
                    IEnumerable<ProfessorTurma> professorTurmaDeleted = professorTurmaContext.Where(tc => !professoresTurmaComCodigo.Any(tv => tc.cd_professor_turma == tv.cd_professor_turma)).ToList();

                    if (professorTurmaDeleted != null && professorTurmaDeleted.Count() > 0)
                        foreach (var item in professorTurmaDeleted)
                            if (item != null)
                            {
                                bool existeProg = false;
                                string mensage = "";
                                if (turma.id_turma_ppt)
                                {
                                    existeProg = daoDiarioAula.verificarDiarioAulaEfetivadoProfTurmasFilhasPPT(turma.cd_turma, item.cd_professor, turma.cd_pessoa_escola);
                                    mensage = Messages.msgRemoveProfDiarioAulaFilhasPPT;
                                }
                                else
                                {
                                    existeProg = daoDiarioAula.verificaDiarioAulaEfetivadoProf(turma.cd_turma, item.cd_professor, turma.cd_pessoa_escola);
                                    mensage = Messages.msgRemoveProfDiarioAula;
                                }
                                if (existeProg)
                                    throw new TurmaBusinessException(string.Format(mensage), null, TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA_TURMA, false);
                                daoProfessorTurma.delete(item, false);
                                professorTurmaView.Remove(item);
                            }
                }
                if ((!turmaPPTFilha && turma.id_turma_ppt == false ) ||  (!turmaPPTFilha && turma.id_turma_ppt && turma.id_turma_ativa))
                {
                    //Lista de professores ativos, para verificar se tem horarios disponivel
                    List<ProfessorTurma> listaProf = professorTurmaView.Where(p => p.id_professor_ativo == true && professoresTurmaComCodigo.Any(pCod => pCod.cd_professor == p.cd_professor)).ToList();
                    TurmaAlunoProfessorHorario TurmaProfessorHorario;
                    List<Horario> listaHorario = horarios.ToList();
                    List<Horario> horariosOcupados = listaHorario.Where(h => h.calendar == "Calendar2").ToList();
                    if (listaProf.Count() == 0)
                        listaProf = professorTurmaView.Where(p => p.id_professor_ativo == true ).ToList();
                    if (listaProf.Count() > 0)
                        foreach (Horario h in horariosOcupados)
                        {
                            IEnumerable<Professor> profOK = new List<Professor>();
                            List<HorarioProfessorTurma> listHorariosProfessores = h.HorariosProfessores.ToList();
                            List<int> profsHorario = new List<int>();
                            for (int i = 0; i < listHorariosProfessores.Count(); i++)
                                if (listaProf.Any(c => c.cd_professor == listHorariosProfessores[i].cd_professor))
                                    profsHorario.Add(listHorariosProfessores[i].cd_professor);

                            DateTime? dt_final_carga = turma.dt_final_aula == null ? turma.dt_inicio_aula.AddDays((turma.Curso == null || turma.Duracao == null) ? 120 : turma.Duracao.nm_duracao == 0 ? 0 : (int)(turma.Curso.nm_carga_horaria == null ? 0 : turma.Curso.nm_carga_horaria / (double)turma.Duracao.nm_duracao * 7.0)) : turma.dt_final_aula;

                            TurmaProfessorHorario = new TurmaAlunoProfessorHorario
                            {
                                cd_turma = turma.cd_turma,
                                horario = h,
                                professores = profsHorario.Distinct().ToArray(),
                                dt_inicio = turma.dt_inicio_aula,
                                dt_final = dt_final_carga
                            };
                            if (turma.cd_turma_ppt != null && turma.cd_turma_ppt > 0)
                                TurmaProfessorHorario.cd_turma = (int)turma.cd_turma_ppt;
                            IEnumerable<Professor> profsDisp = professorBusiness.getRetProfessoresDisponiveisFaixaHorarioTurma(TurmaProfessorHorario, turma.cd_pessoa_escola);
                            profOK = profsDisp.Where(p => profsHorario.Distinct().Contains(p.cd_funcionario));
                            if (profsHorario.Distinct().Count() > 0 && profOK.Count() != profsHorario.Distinct().Count())
                                throw new FuncionarioBusinessException(string.Format(Messages.msgProfHorarioDisponivel), null, FuncionarioBusinessException.TipoErro.ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA, false);
                        }
                }
                //Lista de professores ativos para verificar se a operação de insert ou update.
                foreach (var item in professorTurmaView)
                    if (item.cd_professor_turma.Equals(null) || item.cd_professor_turma == 0)
                    {
                        item.cd_turma = turma.cd_turma;
                        daoProfessorTurma.add(item, false);
                    }
                    else if (professorTurmaContext != null)
                        (from hp in professorTurmaContext where hp.cd_professor_turma == item.cd_professor_turma select hp).FirstOrDefault().id_professor_ativo = item.id_professor_ativo;
                daoProfessorTurma.saveChanges(false);
            }
        }

        public TurmaApiCyberBdUI findTurmaApiCyber(int cd_turma, int cd_escola)
        {
            return daoTurma.findTurmaApiCyber(cd_turma, cd_escola);
        }

        public TurmaApiCyberBdUI findTurmaByCdTurmaAndCdEscolaApiCyber(int cd_turma, int cd_escola)
        {
            return daoTurma.findTurmaByCdTurmaAndCdEscolaApiCyber(cd_turma, cd_escola);
        }

        public TurmaApiCyberBdUI findTurmaCancelarEncerramentoApiCyber(int cd_turma)
        {
            return daoTurma.findTurmaCancelarEncerramentoApiCyber(cd_turma);
        }

        public List<ProfessorTurma> findProfessorTurmaByCdTurma(int cd_turma, int cd_escola)
        {
            return daoTurma.findProfessorTurmaByCdTurma(cd_turma, cd_escola).ToList();
        }

        public List<LivroAlunoApiCyberBdUI> findAlunoTurmaAtivosByCdTurma(int cd_turma)
        {
            return daoTurma.findAlunoTurmaAtivosByCdTurma(cd_turma).ToList();
        }

        public List<LivroAlunoApiCyberBdUI> findAlunoTurmaAtivosByCdTurmaPPTPai(int cd_turma)
        {
            return daoTurma.findAlunoTurmaAtivosByCdTurmaPPTPai(cd_turma).ToList();
        }

        public int findNovaTurmaByCdTurmaEncerrada(int cd_turma)
        {
            return daoTurma.findNovaTurmaByCdTurmaEncerrada(cd_turma);
        }


        public void crudFeriadosDesconsiderados(List<FeriadoDesconsiderado> feriadosDesconsiderados, int cd_turma, int cd_escola)
        {
            List<FeriadoDesconsiderado> feriadoDesconsideradoContext = daoFeriadoDesconsiderado.getFeriadoDesconsideradoByTurma(cd_turma, cd_escola).ToList();
            IEnumerable<FeriadoDesconsiderado> feriadoDesconsideradoComCodigo = feriadosDesconsiderados.Where(fd => fd.cd_feriado_desconsiderado != 0);
            IEnumerable<FeriadoDesconsiderado> descFeriadoInsert = feriadosDesconsiderados.Where(fd => fd.cd_feriado_desconsiderado <= 0);
            IEnumerable<FeriadoDesconsiderado> descFeriadoEdit = feriadosDesconsiderados.Where(fd => fd.cd_feriado_desconsiderado > 0);
            IEnumerable<FeriadoDesconsiderado> progTurmaDeleted = feriadoDesconsideradoContext.Where(pc => !feriadoDesconsideradoComCodigo.Any(pv => pc.cd_feriado_desconsiderado == pv.cd_feriado_desconsiderado));

            if (progTurmaDeleted.Count() > 0)
                foreach (var item in progTurmaDeleted)
                {
                    daoProgTurma.atualizaProgramacaoRemovendoDesconsideraFeriado(item.cd_feriado_desconsiderado);
                    daoFeriadoDesconsiderado.delete(item, false);
                }
            foreach (var item in descFeriadoInsert)
            {
                if (item.cd_feriado_desconsiderado.Equals(null) || item.cd_feriado_desconsiderado == 0)
                {
                    item.cd_turma = cd_turma;
                    daoFeriadoDesconsiderado.add(item, false);
                    atualizaProgramacoesTurmaFeriadoDesconsiderado(item);
                }
            }
            foreach (var item in descFeriadoEdit)
            {
                var progTurmaEditContext = (from pt in feriadoDesconsideradoContext
                                            where pt.cd_feriado_desconsiderado == item.cd_feriado_desconsiderado
                                            select pt).FirstOrDefault();
                if (progTurmaEditContext != null)
                {
                    if (progTurmaEditContext.dt_inicial != item.dt_inicial || progTurmaEditContext.dt_final != item.dt_final)
                    {
                        daoProgTurma.atualizaProgramacaoRemovendoDesconsideraFeriado(item.cd_feriado_desconsiderado);
                        atualizaProgramacoesTurmaFeriadoDesconsiderado(item);
                        progTurmaEditContext.dt_inicial = item.dt_inicial;
                        progTurmaEditContext.dt_final = item.dt_final;
                        //progTurmaEditContext.copy(item);
                    }
                }
            }
            daoFeriadoDesconsiderado.saveChanges(false);
        }

        private void atualizaProgramacoesTurmaFeriadoDesconsiderado(FeriadoDesconsiderado feriadoDesc)
        {
            List<ProgramacaoTurma> progTurma = daoProgTurma.getProgramacaoComDesconsideraFeriado(feriadoDesc).ToList();

            if (progTurma != null)
                for (int i = 0; i < progTurma.Count; i++)
                {
                    if (progTurma[i].id_aula_dada)
                        throw new ProgramacaoTurmaBusinessException(Messages.msgErroModificarDesconsideraFeriadoAulaEfetivada, null, ProgramacaoTurmaBusinessException.TipoErro.ERRO_MODIFICAR_DESCONSIDERA_FERIADO_AULA_EFETIVADA, false);
                    progTurma[i].cd_feriado_desconsiderado = feriadoDesc.cd_feriado_desconsiderado;
                }
            daoProgTurma.saveChanges(false);
        }

        public bool crudProgramacaoTurma(List<ProgramacaoTurma> programacaosTurmaView, int cd_turma, int cd_escola)
        {
            List<ProgramacaoTurma> progTurmaContext = daoProgTurma.getProgramacaoTurmaByTurma(cd_turma, cd_escola, DataAccess.ProgramacaoTurmaDataAccess.TipoConsultaProgTurmaEnum.HAS_PROG_TURMA,false).ToList();
            IEnumerable<ProgramacaoTurma> progTurmaComCodigo = from pt in programacaosTurmaView
                                                                where pt.cd_programacao_turma != 0
                                                                select pt;
            IEnumerable<ProgramacaoTurma> progTurmaInsert = from pt in programacaosTurmaView
                                                            where pt.cd_programacao_turma <= 0
                                                            select pt;
            IEnumerable<ProgramacaoTurma> progTurmaEdit = from pt in programacaosTurmaView
                                                            where pt.programacaoTurmaEdit == true && pt.cd_programacao_turma > 0
                                                            select pt;
            IEnumerable<ProgramacaoTurma> progTurmaDeleted = progTurmaContext.Where(pc => !progTurmaComCodigo.Any(pv => pc.cd_programacao_turma == pv.cd_programacao_turma));

            bool id_programacao = false;

            if (progTurmaDeleted != null && progTurmaDeleted.Count() > 0)
            {
                if(daoProgTurma.verificaProgramacoesComDiario(progTurmaDeleted.Select(x=> x.cd_programacao_turma).ToList(), cd_escola))
                    throw new TurmaBusinessException(string.Format(Utils.Messages.Messages.msgNotDeletedProgTurmaDiarioAula),
                            null, TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA_TURMA, false);
                foreach (var item in progTurmaDeleted)
                    if (item.id_aula_dada)
                        throw new TurmaBusinessException(string.Format(Utils.Messages.Messages.msgNotDeletedProgTurmaDiarioAula),
                            null, TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA_TURMA, false);
                    else
                    {
                        daoProgTurma.deleteContext(item, false);
                        id_programacao = true;
                    }
            }
            if(progTurmaInsert != null)
                foreach(var item in progTurmaInsert)
                    if(item.cd_programacao_turma == 0) {
                        item.cd_turma = cd_turma;
                        daoProgTurma.addContext(item, false);
                        id_programacao = true;
                    }
            if (progTurmaEdit != null)
            {
                foreach (var item in progTurmaEdit)
                {
                    var progTurmaEditContext = (from pt in progTurmaContext
                                                where pt.cd_programacao_turma == item.cd_programacao_turma
                                                select pt).FirstOrDefault();
                    if (progTurmaEditContext != null)
                    {
                        progTurmaEditContext.dc_programacao_turma = item.dc_programacao_turma;
                        progTurmaEditContext.nm_aula_programacao_turma = item.nm_aula_programacao_turma;
                        progTurmaEditContext.id_mostrar_calendario = item.id_mostrar_calendario;

                        if (progTurmaEditContext.cd_feriado == null)
                            progTurmaEditContext.id_programacao_manual = item.id_programacao_manual;
                    }
                }
                id_programacao = true;
            }
            daoProgTurma.saveChanges(false);
            return id_programacao;
        }

        public List<TurmaEscola> crudAlunosTurmasPPT(List<Turma> alunosTurmaPPT, Turma turmaPaiPPT, bool alteradoNome, int cd_escola, bool horarioDiferente)
        {
            this.sincronizaContexto(daoTurma.DB());
            List<Horario> listaHorariosTurmaFilha = new List<Horario>();
            //daoTurma.sincronizaContexto(daoAlunoTurma.DB());  
            //LBMPPT Usar esta linha se gravar a turma filha na escola do pai
            List<Turma> turmasContext = daoTurma.findTurma(turmaPaiPPT.cd_pessoa_escola, turmaPaiPPT.cd_turma, TurmaDataAccess.TipoConsultaTurmaEnum.TODAS_PPT_FILHAS).ToList();
            //LBMPPT Usar esta linha se gravar a turma filha na escola destino(o parametro cd_escola contem o destino)
            //List<Turma> turmasContext = daoTurma.findTurma(cd_escola, turmaPaiPPT.cd_turma, TurmaDataAccess.TipoConsultaTurmaEnum.TODAS_PPT_FILHAS).ToList();
            IEnumerable<Turma> alunosTurmaPPTComCodigo = from alunoT in alunosTurmaPPT
                                                         where alunoT.cd_turma != 0
                                                         select alunoT;
            //Em alunosTurmaPPTComCódigo, somente as turmas com alunos da escola estarão disponíveis
            //Em turmasContext todas as turmas estarão disponíveis, tem que filtrar apenas com alunos da mesma escola
            //LBMPPT Usaresta linha mesma escola do pai
            IEnumerable<Turma> alunosTurmaDeleted = turmasContext.Where(tc => !alunosTurmaPPTComCodigo.Any(tv => tc.cd_turma == tv.cd_turma));
            //LBMPPT Usar esta para escola diferente do pai
            //IEnumerable<Turma> alunosTurmaDeleted = turmasContext.Where(tc => !alunosTurmaPPTComCodigo.Any(tv => tc.cd_turma == tv.cd_turma) && tc.cd_pessoa_escola == cd_escola);
            if (alunosTurmaDeleted.Count() > 0)
            {
                foreach (var item in alunosTurmaDeleted)
                {
                    if (item != null)
                    {
                        //LBMPPT Usar esta linha se escola diferente
                        //AlunoTurma alunoTurma = secretariaBiz.findAlunosTurmaPorTurmaEscola(item.cd_turma, item.cd_pessoa_escola).FirstOrDefault();
                        //LBM mesma escola para as turmas
                        AlunoTurma alunoTurma = secretariaBiz.findAlunosTurmaPorTurmaEscola(item.cd_turma, cd_escola).FirstOrDefault();
                        if (alunoTurma != null && daoTurma.getEscolaAluno(alunoTurma.cd_aluno) == cd_escola)
                        {
                            if (alunoTurma.cd_situacao_aluno_turma == (int)Turma.SituacaoAlunoTurma.AGUARDANDO && alunoTurma.cd_turma_origem > 0)
                                throw new TurmaBusinessException(string.Format(Utils.Messages.Messages.msgNotExcAlunoTurmaAguardMovido), null, TurmaBusinessException.TipoErro.ERRO_DELETAR_ALUNOTURMA_AGUARDANDO_MOVIDO, false);
                            if (alunoTurma.cd_contrato != null && alunoTurma.cd_contrato > 0)
                                throw new TurmaBusinessException(Messages.msgNotDeletedAlunoTurmaFilhaPPTExisteContrato, null, TurmaBusinessException.TipoErro.ERRO_DELETAR_TURMA_ALUNO_COM_CONTRATO, false);
                            if (alunoTurma != null && alunoTurma.cd_situacao_aluno_turma == (int)Turma.SituacaoAlunoTurma.REMATRICULADO || alunoTurma.cd_situacao_aluno_turma == (int)Turma.SituacaoAlunoTurma.ATIVO)
                                throw new TurmaBusinessException(string.Format(Utils.Messages.Messages.msgNotExcluirAlunoTurmaMatOuRematriculado), null, TurmaBusinessException.TipoErro.ERRO_DELETAR_ALUNOTURMA_MATRICULADO_REMATRICULADO, false);
                            secretariaBiz.deletarAlunoTurma(alunoTurma);
                            bool existDiarioAula = daoDiarioAula.verificaTurmaDiarioAulaLancado(item.cd_turma, item.cd_pessoa_escola);
                            if (existDiarioAula && alunoTurma.cd_situacao_aluno_turma != (int)AlunoTurma.SituacaoAlunoTurma.Aguardando)
                                throw new TurmaBusinessException(string.Format(Utils.Messages.Messages.msgExistsDiarioAulaLancado), null, TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA_TURMA, false);
                            daoTurma.delete(item, false);
                        }
                    }
                }
            }
            List<TurmaEscola> turmalist = new List<TurmaEscola>();
            foreach (var item in alunosTurmaPPT)
            {

                Turma novaTurma = new Turma();
                TurmaEscola novaTurmaEscola = new TurmaEscola();
                //LBMPPT Deixar a linha comentada pois preciso daoutr escola mais abaixo para a Turma Escola
                //item.cd_pessoa_escola = turmaPaiPPT.cd_pessoa_escola;
                if (item.cd_turma.Equals(null) || item.cd_turma == 0)
                {
                    //LBMPPT Mesma escola da pai
                    novaTurma.cd_pessoa_escola = turmaPaiPPT.cd_pessoa_escola;
                    //LBMPPT Usar esta para escola diferente
                    //novaTurma.cd_pessoa_escola = item.cd_pessoa_escola; 
                    novaTurma.cd_duracao = turmaPaiPPT.cd_duracao;
                    novaTurma.cd_regime = turmaPaiPPT.cd_regime;
                    novaTurma.cd_produto = turmaPaiPPT.cd_produto;
                    novaTurma.dt_inicio_aula = item.dt_inicio_aula.Date;
                    novaTurma.cd_turma = 0;
                    novaTurma.id_turma_ppt = false;
                    novaTurma.cd_curso = item.cd_curso;
                    novaTurma.cd_turma_ppt = turmaPaiPPT.cd_turma;
                    novaTurma.no_apelido = item.no_apelido;
                    novaTurma.cd_sala_online = item.cd_sala_online;

                    int nrProximaTurma = daoTurma.verificaExisteTurma(item.no_turma, novaTurma.cd_pessoa_escola, 0);
                    novaTurma.no_turma = item.no_turma + "-" + nrProximaTurma;
                    novaTurma.nm_turma = nrProximaTurma;
                    while (daoTurma.verificaExisteTurma(novaTurma.no_turma, novaTurma.cd_pessoa_escola, 0) > 1) {
                        nrProximaTurma++;
                        novaTurma.no_turma = item.no_turma + "-" + nrProximaTurma;
                        novaTurma.nm_turma = nrProximaTurma;
                    }
                    if (item.ProgramacaoTurma != null)
                    {
                        List<ProgramacaoTurma> programacoesTurma = new List<ProgramacaoTurma>();
                        programacoesTurma = item.ProgramacaoTurma.ToList();
                        if (programacoesTurma.Count >= 1)
                            novaTurma.dt_final_aula = programacoesTurma[programacoesTurma.Count - 1].dta_programacao_turma.Date;
                        novaTurma.nro_aulas_programadas = (byte)programacoesTurma.Where(pt => (pt.cd_feriado == null) || (pt.cd_feriado != null && pt.cd_feriado_desconsiderado != null)).Count();
                    }

                    novaTurma = daoTurma.add(novaTurma, false);
                    secretariaBiz.addAlunoTurma(new AlunoTurma
                    {
                        cd_turma = novaTurma.cd_turma,
                        cd_aluno = item.alunosTurma.ToList()[0].cd_aluno,
                        dt_inicio = item.dt_inicio_aula.Date,
                        dt_movimento = DateTime.UtcNow,
                        cd_situacao_aluno_turma = (int)AlunoTurma.SituacaoAlunoTurma.Aguardando
                    });


                    if (item.horariosTurma != null)
                        listaHorariosTurmaFilha = Horario.clonarHorariosZerandoMemoria(item.horariosTurma.ToList(), "Calendar2");
                    if (turmaPaiPPT.horariosTurma != null)
                        crudHorariosTurma(listaHorariosTurmaFilha, novaTurma.cd_turma, novaTurma.cd_pessoa_escola, Turma.TipoTurma.NORMAL);
                    var minutosSemana = (from t in item.horariosTurma select (t.dt_hora_fim - t.dt_hora_ini).TotalMinutes).Sum();
                    if (minutosSemana < 100 && horarioDiferente)
                        throw new TurmaBusinessException(Messages.msgErroCargaHorariaMinima, null,
                            TurmaBusinessException.TipoErro.ERRO_CARGA_HORARIA_MINIMA, false);
                    if (item.ProgramacaoTurma != null && item.ProgramacaoTurma.Count() > 0)
                        crudProgramacaoTurma(item.ProgramacaoTurma.ToList(), novaTurma.cd_turma, novaTurma.cd_pessoa_escola);
                    if (item.ProfessorTurma != null)
                        crudProfessoresTurma(ProfessorTurma.clonarProfessorZerandoMemoria(item.ProfessorTurma.ToList(), novaTurma.cd_turma), novaTurma, listaHorariosTurmaFilha, true);
                    if (item.FeriadosDesconsiderados != null)
                        crudFeriadosDesconsiderados(item.FeriadosDesconsiderados.ToList(), novaTurma.cd_turma, novaTurma.cd_pessoa_escola);
                    if (novaTurma.cd_sala_online != null && (item.cd_pessoa_escola != turmaPaiPPT.cd_pessoa_escola))
                    {
                        novaTurmaEscola.cd_turma = novaTurma.cd_turma;
                        novaTurmaEscola.cd_escola = item.cd_pessoa_escola;
                        turmalist.Add(novaTurmaEscola);
                    }
                }
                else
                {
                    Turma turmaFilhaPPT = turmasContext.Where(hc => hc.cd_turma == item.cd_turma).FirstOrDefault();
                    List<AlunoTurma> at = turmaFilhaPPT.TurmaAluno.ToList();
                    //if (at != null && at[0].cd_situacao_aluno_turma == (int)Turma.SituacaoAlunoTurma.REMATRICULADO ||
                    //    at[0].cd_situacao_aluno_turma == (int)Turma.SituacaoAlunoTurma.ATIVO ||
                    //    at[0].cd_situacao_aluno_turma == (int)Turma.SituacaoAlunoTurma.AGUARDANDO)
                    //{
                    //Pesquisa aluno turma para alterar a data de inicio
                    if (turmaFilhaPPT.dt_inicio_aula != item.dt_inicio_aula.Date)
                    {
                        if (at != null && at.Count() > 0)
                            at[0].dt_inicio = item.dt_inicio_aula.Date;
                    }

                    turmaFilhaPPT.dt_inicio_aula = item.dt_inicio_aula.Date;
                    turmaFilhaPPT.cd_duracao = item.cd_duracao;
                    turmaFilhaPPT.no_apelido = item.no_apelido;
                    if (item.ProgramacaoTurma != null)
                    {
                        List<ProgramacaoTurma> programacoesTurma = item.ProgramacaoTurma.ToList();
                        if (programacoesTurma.Count >= 1)
                            turmaFilhaPPT.dt_final_aula = programacoesTurma[programacoesTurma.Count - 1].dta_programacao_turma;
                        else
                            turmaFilhaPPT.dt_final_aula = null;
                    }
                    if (turmaFilhaPPT.dt_final_aula != null)
                        turmaFilhaPPT.dt_final_aula = ((DateTime)turmaFilhaPPT.dt_final_aula).Date;
                    if (item.horariosTurma != null)
                        crudHorariosTurma(item.horariosTurma.ToList(), turmaFilhaPPT.cd_turma, turmaFilhaPPT.cd_pessoa_escola, Turma.TipoTurma.NORMAL);
                    var minutosSemana = (from t in item.horariosTurma select (t.dt_hora_fim - t.dt_hora_ini).TotalMinutes).Sum();
                    if (minutosSemana < 100 && horarioDiferente)
                        throw new TurmaBusinessException(Messages.msgErroCargaHorariaMinima, null,
                            TurmaBusinessException.TipoErro.ERRO_CARGA_HORARIA_MINIMA, false);

                    if (item.ProfessorTurma != null)
                        crudProfessoresTurma(item.ProfessorTurma.ToList(), item, item.horariosTurma, true);
                    if (item.ProgramacaoTurma != null)
                        crudProgramacaoTurma(item.ProgramacaoTurma.ToList(), item.cd_turma, item.cd_pessoa_escola);
                    if (item.FeriadosDesconsiderados != null)
                        crudFeriadosDesconsiderados(item.FeriadosDesconsiderados.ToList(), item.cd_turma, item.cd_pessoa_escola);
                    turmaFilhaPPT.nro_aulas_programadas = (Byte)getQuantidadeAulasProgramadasTurma(turmaFilhaPPT.cd_turma, turmaFilhaPPT.cd_pessoa_escola);
                    
                    //LRS-> para propagar a sala para as turmas filhas
                    turmaFilhaPPT.cd_sala = turmaPaiPPT.cd_sala;
                    

                    //Verificando nomes das  turmas ppts quando alterado
                    if (alteradoNome)
                    {
                        int nrProximaTurma = daoTurma.verificaExisteTurma(item.no_turma, item.cd_pessoa_escola, item.cd_turma);
                        turmaFilhaPPT.no_turma = item.no_turma + "-" + nrProximaTurma;
                        turmaFilhaPPT.nm_turma = nrProximaTurma;
                        
                    }
                    //}
                }
                
                //daoAlunoTurma.saveChanges(false);
                daoTurma.saveChanges(false);
            }
            return turmalist;
        }

        private void crudHorarioProfessorTurma(Horario horario, Turma.TipoTurma tipoTurma, int? cd_turma_ppt)
        {
            this.sincronizaContexto(daoTurma.DB());
            // Atualiza a lista de professores dos horários:
            List<HorarioProfessorTurma> professoresContext = professorBusiness.getHorarioProfessorByHorario(horario.cd_horario);
            IEnumerable<HorarioProfessorTurma> professoresHorariosComCodigo = horario.HorariosProfessores.Where(pr => pr.cd_horario_professor_turma != 0);

            IEnumerable<HorarioProfessorTurma> professoresDeleted = professoresContext.Where(pr => !professoresHorariosComCodigo.Any(prcc => prcc.cd_horario_professor_turma == pr.cd_horario_professor_turma));

            if (professoresDeleted != null)
                foreach (var itemProf in professoresDeleted)
                    if (itemProf != null)
                        //IEnumerable<HorarioProfessorTurma> hProfFilhas = professorBusiness.getHorarioProfessorByProfessorTurmaPPT(itemProf.cd_professor, (int)cd_turma_ppt);
                        //if (hProfFilhas != null)
                        //    foreach (var itemProfFilhas in hProfFilhas)
                        //        if (itemProfFilhas != null)
                        //            professorBusiness.deleteProfessorHorario(itemProfFilhas);
                        professorBusiness.deleteProfessorHorario(itemProf);
                    
            foreach (var itemProf in horario.HorariosProfessores)
                //Novos professores no horário da turma:
                if (itemProf.cd_horario_professor_turma == 0 && itemProf.cd_professor > 0)
                {
                    HorarioProfessorTurma horarioPT = new HorarioProfessorTurma();

                    horarioPT.copy(itemProf);
                    professorBusiness.addProfessorHorario(horarioPT);
                }
        }

        //Programação Turma
        public List<ProgramacaoTurma> criaProgramacaoTurma(ProgramacaoHorarioUI programacao, int cd_escola, ref IEnumerable<Feriado> feriadosEscola)
        {
            List<ProgramacaoTurma> retorno;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, daoFeriado.DB()))
            {
                this.sincronizaContexto(daoFeriado.DB());
                retorno = criaProgramacaoTurma(programacao, cd_escola, null, ref feriadosEscola);
                transaction.Complete();
            }
            return retorno;
        }

        public List<ProgramacaoTurma> criaProgramacaoTurma(ProgramacaoHorarioUI programacao, int cd_escola, string dc_aula_programacao, ref IEnumerable<Feriado> feriadosEscola)
        {
            ProgramacaoTurma retorno = new ProgramacaoTurma();
            List<ProgramacaoTurma> retornoFeriados = new List<ProgramacaoTurma>();
            List<ProgramacaoTurma> programacoes = new List<ProgramacaoTurma>();
            DateTime data_hoje = DateTime.Now;

            if (programacao.horarios == null || programacao.horarios.Count == 0 || programacao.dt_inicio == null)
                throw new ProgramacaoTurmaBusinessException(Messages.msgProgramacaoSemHorario, null, ProgramacaoTurmaBusinessException.TipoErro.ERRO_PROGRAMACAO_SEM_HORARIO_OU_DATA, false);

            // Recupera o próximo número de aula, de forma incremental:
            retorno.nm_aula_programacao_turma = 1;
            if (programacao.programacoes != null && programacao.programacoes.Count > 0)
            {
                programacoes = programacao.programacoes.ToList();
                retorno.nm_aula_programacao_turma = programacoes.LastOrDefault().nm_aula_programacao_turma + 1;
            }

            //Recupera o nome proposto para descrição:
            retorno.dc_programacao_turma = String.IsNullOrEmpty(dc_aula_programacao) ? "Aula " + retorno.nm_aula_programacao_turma : dc_aula_programacao;
            //retorno.dta_cadastro_programacao = data_hoje; Já existe na auditoria

            // Ordena as faixas de horários da turma:
            List<Horario> horarios = programacao.horarios.ToList();

            foreach (Horario h in horarios)
                h.loadHorario();

            Horario.SortList(horarios);

            // Recupera a última data programação realizada. Se não existir, pega a primeira após a data inicial:
            DateTime ultima_data = programacao.dt_inicio.Value.Date;

            int dia_semana_ult = (int)ultima_data.DayOfWeek;

            if (programacoes.Count > 0)
                ultima_data = programacoes[programacoes.Count - 1].dta_programacao_turma.Date;
            else
            {
                //Verifica se o primeiro dia se trata de um feriado:
                Horario proximo_horario = new Horario();
                calculaProximoDiaAposFeriado(dia_semana_ult, true, ref retorno, ref retornoFeriados, ref ultima_data, horarios, programacao, cd_escola, data_hoje, ref proximo_horario, ref feriadosEscola);
            }

            //Pesquisa no mesmo dia da última programação para verificar se existe próximo horário:
            bool existeProximoHorario = false;
            dia_semana_ult = (int)ultima_data.DayOfWeek;
            IEnumerable<Horario> horariosDoDia = null;

            if (programacoes.Count > 0)
                //Pesquisa no mesmo dia da data inicial para verificar se existe horário:
                horariosDoDia = horarios.Where(h => h.id_dia_semana - 1 == dia_semana_ult && h.dt_hora_ini > programacoes[programacoes.Count - 1].hr_inicial_programacao);
            else
                horariosDoDia = horarios.Where(h => h.id_dia_semana - 1 == dia_semana_ult);

            if (programacoes.Count <= 0 || (!programacoes[programacoes.Count - 1].cd_feriado.HasValue || programacoes[programacoes.Count - 1].cd_feriado_desconsiderado.HasValue
                                            || programacoes[programacoes.Count - 1].id_feriado_desconsiderado))
                if (horariosDoDia != null && horariosDoDia.Count() > 0)
                {
                    existeProximoHorario = true;
                    List<Horario> listHorariosDoDia = horariosDoDia.ToList();

                    Horario.SortList(listHorariosDoDia);

                    retorno.hr_inicial_programacao = listHorariosDoDia[0].dt_hora_ini;
                    retorno.hr_final_programacao = listHorariosDoDia[0].dt_hora_fim;
                    retorno.dta_programacao_turma = ultima_data.Date;//ultima_data.ToUniversalTime();
                }

            //Não havendo próximo horário no dia a programação (ou se tratar da primeira programação), 
            //calcula a próxima aula a partir dos dias da semana dos horários (considerando já os feriados);
            if (!existeProximoHorario)
            {
                // Busca o próximo dia da semana:
                Horario proximo_horario = new Horario();

                calculaProximoDiaAposFeriado(dia_semana_ult, false, ref retorno, ref retornoFeriados, ref ultima_data, horarios, programacao, cd_escola, data_hoje, ref proximo_horario, ref feriadosEscola);


                retorno.hr_inicial_programacao = proximo_horario.dt_hora_ini;
                retorno.hr_final_programacao = proximo_horario.dt_hora_fim;

                retorno.dta_programacao_turma = ultima_data.Date;//ultima_data.ToUniversalTime();
            }

            retornoFeriados.Add(retorno);

            return retornoFeriados;
        }

        private void calculaProximoDiaAposFeriado(int dia_semana_ult, bool primeira_data, ref ProgramacaoTurma retorno, ref List<ProgramacaoTurma> retornoFeriados,
                ref DateTime ultima_data, List<Horario> horarios, ProgramacaoHorarioUI programacao, int cd_escola, DateTime data_hoje, ref Horario proximo_horario, ref IEnumerable<Feriado> feriadosEscola)
        {
            Feriado proximo_feriado = null;
            DateTime? proxima_data_feriado = null;
            ProgramacaoTurma programacaoFeriado = null;

            do
            {
                // Inclui as programações que possuem feriados:
                if (programacaoFeriado != null)
                {
                    retorno.nm_aula_programacao_turma += 1;

                    //Atualiza o próximo nome de aula após feriado:
                    if (String.IsNullOrEmpty(retorno.dc_programacao_turma))
                        retorno.dc_programacao_turma = "Aula " + retorno.nm_aula_programacao_turma;

                    retornoFeriados.Add(programacaoFeriado);
                }
                //Quando se tratar da volta de um feriado, a data deve pular o feriado.
                if (proximo_feriado != null)
                    proxima_data_feriado = new DateTime((int)proximo_feriado.aa_feriado_fim, (int)proximo_feriado.mm_feriado_fim, (int)proximo_feriado.dd_feriado_fim);

                List<Horario> proximos_horarios = new List<Horario>();

                dia_semana_ult = (int)ultima_data.DayOfWeek;
                if (!primeira_data)
                    proximos_horarios = horarios.Where(h => h.id_dia_semana - 1 > dia_semana_ult).ToList();
                else
                    proximos_horarios = horarios.Where(h => h.id_dia_semana - 1 >= dia_semana_ult).ToList();

                if (proximos_horarios.Count > 0)
                {
                    Horario.SortList(proximos_horarios);
                    proximo_horario = proximos_horarios[0];
                }
                else
                    proximo_horario = horarios[0];

                //Calcula a diferença dos dias do horario anterior para o proximo horario: (Exemplo: se for 5 e 1 > 3, se for 1 e 5 > 4)
                int diferenca_horarios = 0;
                if (dia_semana_ult < proximo_horario.id_dia_semana - 1)
                    diferenca_horarios = proximo_horario.id_dia_semana - 1 - dia_semana_ult;
                else if (dia_semana_ult > proximo_horario.id_dia_semana - 1)
                    diferenca_horarios = proximo_horario.id_dia_semana - 1 + (7 - dia_semana_ult);
                else if (!primeira_data)
                    diferenca_horarios = 7;
                else
                    diferenca_horarios = 0;

                //Calcula a data através da diferença de horários:
                ultima_data = ultima_data.AddDays(diferenca_horarios);
                primeira_data = false;

                // Caso a data não esteja dentro do desconiderar feriados, verifica feriados:
                var feriados_desconsiderados = programacao.feriados_desconsiderados != null ? programacao.feriados_desconsiderados.ToList() : new List<FeriadoDesconsiderado>();
                if (!feriadoContidoFeriadosDesconsiderados(feriados_desconsiderados, ultima_data))
                {
                    //Recupera a próxima sequência de feriados maior ou igual a data calculada, se tiver dentro do feriado, pula a data:
                    if (!proxima_data_feriado.HasValue || DateTime.Compare(ultima_data, proxima_data_feriado.Value) > 0)
                        proximo_feriado = this.getFeriadosDentroOuAposData(cd_escola, ultima_data, false, ref feriadosEscola);
                }
                else
                    proximo_feriado = null;

                // Configura os feriados para mostrá-lo:
                if (proximo_feriado != null)
                {
                    programacaoFeriado = new ProgramacaoTurma();
                    //programacaoFeriado.dta_cadastro_programacao = data_hoje; Já existe na auditoria
                    programacaoFeriado.dc_programacao_turma = proximo_feriado.dc_feriado;
                    programacaoFeriado.cd_feriado = proximo_feriado.cod_feriado;
                    programacaoFeriado.hr_inicial_programacao = proximo_horario.dt_hora_ini;
                    programacaoFeriado.hr_final_programacao = proximo_horario.dt_hora_fim;
                    programacaoFeriado.dta_programacao_turma = ultima_data.Date;

                    programacaoFeriado.nm_aula_programacao_turma = retorno.nm_aula_programacao_turma;
                }
                //Verifica se a última data está dentro do feriado:
            } while (proximo_feriado != null
                && ((proximo_feriado.aa_feriado.HasValue && proximo_feriado.aa_feriado_fim.HasValue
                        && DateTime.Compare(ultima_data, new DateTime((int)proximo_feriado.aa_feriado, (int)proximo_feriado.mm_feriado, (int)proximo_feriado.dd_feriado)) >= 0
                        && DateTime.Compare(ultima_data, new DateTime((int)proximo_feriado.aa_feriado_fim, (int)proximo_feriado.mm_feriado_fim, (int)proximo_feriado.dd_feriado_fim)) <= 0)
                    ||
                    (!proximo_feriado.aa_feriado.HasValue && !proximo_feriado.aa_feriado_fim.HasValue
                        && DateTime.Compare(ultima_data, new DateTime((int)data_hoje.Year, (int)proximo_feriado.mm_feriado, (int)proximo_feriado.dd_feriado)) >= 0
                        && DateTime.Compare(ultima_data, new DateTime((int)data_hoje.Year, (int)proximo_feriado.mm_feriado_fim, (int)proximo_feriado.dd_feriado_fim)) <= 0)));
        }
        public Feriado getFeriadosDentroOuAposData(int cd_escola, DateTime ultima_data, bool feriado_financeiro, ref IEnumerable<Feriado> feriadosEscola)
        {
            return getFeriadosDentroOuAposData(cd_escola, ultima_data, feriado_financeiro, ref feriadosEscola, true);
        }
        public Feriado getFeriadosDentroOuAposData(int cd_escola, DateTime ultima_data, bool feriado_financeiro, ref IEnumerable<Feriado> feriadosEscola, bool addDias)
        {
            Feriado retorno = null;

            if (feriadosEscola == null)
                feriadosEscola = daoFeriado.getFeriadosEscola(cd_escola, feriado_financeiro).ToList();

            if (feriadosEscola.Count() > 0)
            {
                IEnumerable<Feriado> cloneFeriadosEscola = feriadosEscola.ToList();

                cloneFeriadosEscola = cloneFeriadosEscola.Select(x => new Feriado
                {
                    aa_feriado = x.aa_feriado.HasValue ? x.aa_feriado : short.Parse(ultima_data.Year + ""),
                    aa_feriado_fim = x.aa_feriado_fim.HasValue ? x.aa_feriado_fim : short.Parse(ultima_data.Year + ""),
                    dd_feriado = x.dd_feriado,
                    dd_feriado_fim = x.dd_feriado_fim,
                    mm_feriado = x.mm_feriado,
                    mm_feriado_fim = x.mm_feriado_fim,
                    dc_feriado = x.dc_feriado,
                    cod_feriado = x.cod_feriado
                });

                List<Feriado> listaAuxiliar = new List<Feriado>();
                List<Feriado> listFeriadoSemAno = cloneFeriadosEscola.ToList();
                for (int i = listFeriadoSemAno.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        DateTime data = new DateTime((int)listFeriadoSemAno[i].aa_feriado_fim, (int)listFeriadoSemAno[i].mm_feriado_fim, (int)listFeriadoSemAno[i].dd_feriado_fim);
                        if (addDias)
                        {
                            if (ultima_data.CompareTo(data) <= 0)
                                listaAuxiliar.Add(listFeriadoSemAno[i]);
                        }
                        else
                        {
                            if (ultima_data.CompareTo(data) >= 0)
                                listaAuxiliar.Add(listFeriadoSemAno[i]);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Warn("Erro ao transformar a data - aa_feriado_fim: " + listFeriadoSemAno[i].aa_feriado_fim + "mm_feriado_fim: " + listFeriadoSemAno[i].mm_feriado_fim
                            + "dd_feriado_fim: " + listFeriadoSemAno[i].dd_feriado_fim, e);
                    }
                }

                IEnumerable<Feriado> listaResultante = from feriado in listaAuxiliar
                                                       orderby feriado.aa_feriado, feriado.mm_feriado, feriado.dd_feriado
                                                       select feriado;
                if (addDias)
                    retorno = listaResultante.FirstOrDefault();
                else
                    retorno = listaResultante.LastOrDefault();

            }
            return retorno;
        }

        private bool feriadoContidoFeriadosDesconsiderados(List<FeriadoDesconsiderado> feriados_desconsiderados, DateTime data)
        {
            bool retorno = false;

            if (feriados_desconsiderados != null)
                for (var i = 0; !retorno && i < feriados_desconsiderados.Count; i++)
                {
                    FeriadoDesconsiderado feriado = feriados_desconsiderados[i];
                    if (DateTime.Compare(data.Date, feriado.dt_inicial.Date) >= 0 && DateTime.Compare(data.Date, feriado.dt_final.Date) <= 0)
                        retorno = true;
                }

            return retorno;
        }

        public Boolean atualizaProgramacoesTurma(List<Feriado> feriados, int? cd_escola, bool isMasterGeral, bool inclui_desconsidera_feriado, bool inclusao_feriado, bool delecao_feriado)
        {
            Boolean retorno = false;
            List<int> cd_feriados = new List<int>();

            foreach (Feriado feriado in feriados)
                cd_feriados.Add(feriado.cod_feriado);

            Int32[] cd_feriados_int = cd_feriados.ToArray();

            //Busca as turmas e as programações de turma que possuem programações da turma com os feriados passados como parâmetro, que não possuem desconsiderar feriados, que não possuem diário de aula, 
            //para uma determinada escola
            List<Turma> turmas = new List<Turma>();

            /*if (inclusao_feriado)
                turmas = this.getTurmaPorProgramacaoTurmaComFeriado((Feriado)feriados[0].Clone(), isMaster ? null : cd_escola).ToList();
            else
                turmas = this.getTurmaPorProgramacaoTurmaComFeriado(cd_feriados_int, isMaster ? null : cd_escola).ToList();
            */
            this.sincronizaContexto(daoProgTurma.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.SPECIALCOMMITED, daoTurma.DB(), TransactionScopeBuilder.TransactionTime.HIGHT)){
                for (int f = 0; f < feriados.Count; f++)
                {
                    turmas = this.getTurmaPorProgramacaoTurmaComFeriado((Feriado)feriados[f].Clone(), isMasterGeral ? null : cd_escola, delecao_feriado).ToList();

                    //Para cada turma, pega a última programação de diário de aula efetivado, percorre até a achar o primeiro feriado deletado e refaz as programções das turmas:
                    if (turmas != null)
                        for (int x = 0; x < turmas.Count; x++)
                        {
                            Turma turma = turmas[x];
                            int qtd_programacoes = 0;

                            retorno = false;
                            
                            List<ProgramacaoTurma> programacoes = turma.ProgramacaoTurma.ToList();

                            for (int i = programacoes.Count - 1; i >= 0; i--)
                            {
                                if (programacoes[i].cd_feriado == null && !programacoes[i].id_aula_dada)
                                    qtd_programacoes += 1;
                                if (programacoes[i].id_aula_dada || i == 0)
                                {
                                    if (programacoes[i].id_aula_dada)
                                        i += 1;
                                    for (int j = i; !retorno && j < programacoes.Count; j++)
                                    {
                                        if (programacoes[j].cd_feriado == null && !programacoes[j].id_aula_dada)
                                            qtd_programacoes -= 1;

                                        if (!inclusao_feriado && programacoes[j].cd_feriado.HasValue && cd_feriados.Contains(programacoes[j].cd_feriado.Value))
                                            refazProgramacoesTurma(TransactionScopeBuilder.TransactionType.SPECIALCOMMITED, qtd_programacoes, programacoes, ref turma, inclui_desconsidera_feriado, ref retorno, feriados, j);
                                        else if (existeIntercessaoFeriadoProgramacao((Feriado)feriados[f].Clone(), programacoes[j]))
                                        {
                                            qtd_programacoes += 1;
                                            refazProgramacoesTurma(TransactionScopeBuilder.TransactionType.SPECIALCOMMITED, qtd_programacoes, programacoes, ref turma, inclui_desconsidera_feriado, ref retorno, feriados, j);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                }
                transaction.Complete();
            }
            logger.Warn("Finalizou as reprogramações.");
            return retorno;
        }

        private bool existeFeriadoFinalProgramacoes(List<ProgramacaoTurma> programacao_turma)
        {
            bool retorno = false;

            if (programacao_turma != null)
                if (programacao_turma.Count > 0 && programacao_turma[programacao_turma.Count - 1].cd_feriado != null)
                    retorno = true;

            return retorno;
        }

        private void removeFeriadosFinalProgramacoes(ref List<ProgramacaoTurma> programacao_turma)
        {
            if(programacao_turma != null)
                for (int i = programacao_turma.Count - 1; i >= 0; i--)
                {
                    if (programacao_turma[i].cd_feriado == null)
                        break;
                    else
                    {
                        daoProgTurma.delete(daoProgTurma.findById(programacao_turma[i].cd_programacao_turma, false), false);
                        programacao_turma.Remove(programacao_turma[i]);
                    }
                }
        }

        //Somente tem intercessao entre as datas se a programação da turma ainda não tenha feriado e não é para ser desconsiderado.
        private bool existeIntercessaoFeriadoProgramacao(Feriado feriado, ProgramacaoTurma programacao_turma)
        {
            bool retorno = false;
            DateTime data_hoje = DateTime.Now;

            if (!feriado.aa_feriado.HasValue)
            {
                feriado.aa_feriado = (short)programacao_turma.dta_programacao_turma.Year;
                feriado.aa_feriado_fim = (short)programacao_turma.dta_programacao_turma.Year;
            }

            DateTime data_ini_feriado = new DateTime(feriado.aa_feriado.Value, feriado.mm_feriado, feriado.dd_feriado);
            DateTime data_fim_feriado = new DateTime(feriado.aa_feriado_fim.Value, feriado.mm_feriado_fim.Value, feriado.dd_feriado_fim.Value);

            data_fim_feriado.AddDays(1);

            if ((!programacao_turma.cd_feriado.HasValue && !programacao_turma.cd_feriado_desconsiderado.HasValue)
                    && DateTime.Compare(programacao_turma.dta_programacao_turma.Date, data_ini_feriado) >= 0 && DateTime.Compare(programacao_turma.dta_programacao_turma.Date, data_fim_feriado) <= 0)
                retorno = true;

            return retorno;
        }

        private void refazProgramacoesTurma(TransactionScopeBuilder.TransactionType transactionType, int qtd_programacoes, List<ProgramacaoTurma> programacoes, ref Turma turma, bool inclui_desconsidera_feriado, ref Boolean retorno, List<Feriado> feriados, int posicao)
        {
            List<ProgramacaoTurma> novas_programacoes = new List<ProgramacaoTurma>();
            if (qtd_programacoes > 0)
            {
                //Remove as programações antigas e inclui as novas:
                IEnumerable<Feriado> feriadosEscola = null;
                ProgramacaoHorarioUI programacaoHorarioUI = new ProgramacaoHorarioUI();
                List<ProgramacaoTurma> programacoes_parte = Extensions.cloneList(programacoes).ToList();
                List<string> nomes_programacoes = new List<string>();

                for (int l = programacoes.Count - 1; nomes_programacoes.Count < qtd_programacoes && l >= 0; l--)
                    if (programacoes[l].cd_feriado == null)
                        nomes_programacoes.Add(programacoes[l].dc_programacao_turma);
                nomes_programacoes.Reverse();

                programacoes_parte.RemoveRange(posicao, programacoes_parte.Count - posicao);

                programacaoHorarioUI.cd_curso = turma.cd_curso;
                programacaoHorarioUI.cd_duracao = turma.cd_duracao;
                programacaoHorarioUI.cd_turma = turma.cd_turma;
                programacaoHorarioUI.dt_inicio = turma.dt_inicio_aula;
                programacaoHorarioUI.programacoes = programacoes_parte;
                programacaoHorarioUI.tipo = (int)TipoAtualizacaoProgramacaoTurma.TIPO_REFAZER_PROGRAMACOES_FERIADO;
                programacaoHorarioUI.horarios = turma.horariosTurma;
                if (inclui_desconsidera_feriado)
                    programacaoHorarioUI.feriados_desconsiderados = criaDesconsideraFeriados(feriados, turma.dt_inicio_aula, programacoes_parte);
                for (int l = 0; l < qtd_programacoes; l++)
                {
                    List<ProgramacaoTurma> prog_criadas = this.criaProgramacaoTurma(programacaoHorarioUI, turma.cd_pessoa_escola, nomes_programacoes[l], ref feriadosEscola);
                    List<ProgramacaoTurma> prog_anteriores = programacaoHorarioUI.programacoes.ToList();

                    novas_programacoes.AddRange(prog_criadas);
                    prog_anteriores.AddRange(prog_criadas);
                    programacaoHorarioUI.programacoes = prog_anteriores;
                }
                this.atualizaProgramacoesTurma(transactionType, novas_programacoes, turma.cd_turma, posicao - 1);

                retorno = true;

                //Atualiza a data de fim da turma:
                if (novas_programacoes.Count > 0)
                {
                    Turma turmaContext = daoTurma.findById(turma.cd_turma, false);

                    turmaContext.dt_final_aula = novas_programacoes[novas_programacoes.Count - 1].dta_programacao_turma;
                    daoTurma.saveChanges(false);
                }
            }
            else if (existeFeriadoFinalProgramacoes(programacoes))
                removeFeriadosFinalProgramacoes(ref programacoes);
        }

        private List<FeriadoDesconsiderado> criaDesconsideraFeriados(List<Feriado> feriados, DateTime dt_inicio, List<ProgramacaoTurma> programacoes)
        {
            List<FeriadoDesconsiderado> retorno = new List<FeriadoDesconsiderado>();

            //Pega a data da programação:
            DateTime data = dt_inicio;

            if (programacoes != null && programacoes.Count > 0)
                data = programacoes[programacoes.Count - 1].dta_programacao_turma;

            if (feriados != null)
                foreach (Feriado feriado in feriados)
                {
                    FeriadoDesconsiderado feriado_desconsiderado = new FeriadoDesconsiderado();

                    feriado_desconsiderado.feriado_deletado = true;

                    if (feriado.aa_feriado.HasValue)
                    {
                        feriado_desconsiderado.dt_inicial = new DateTime(feriado.aa_feriado.Value, feriado.mm_feriado, feriado.dd_feriado);
                        if (!feriado.mm_feriado_fim.HasValue)
                            feriado.mm_feriado_fim = feriado.mm_feriado;
                        if (!feriado.aa_feriado_fim.HasValue)
                            feriado.aa_feriado_fim = feriado.aa_feriado;
                        if (!feriado.dd_feriado_fim.HasValue)
                            feriado.dd_feriado_fim = feriado.dd_feriado;

                        feriado_desconsiderado.dt_final = new DateTime(feriado.aa_feriado_fim.Value, feriado.mm_feriado_fim.Value, feriado.dd_feriado_fim.Value);

                        retorno.Add(feriado_desconsiderado);
                    }
                    else
                    {
                        int ano = data.Year;

                        feriado_desconsiderado.dt_inicial = new DateTime(ano, feriado.mm_feriado, feriado.dd_feriado);
                        if (!feriado.mm_feriado_fim.HasValue)
                            feriado.mm_feriado_fim = feriado.mm_feriado;
                        feriado_desconsiderado.dt_final = new DateTime(ano, feriado.mm_feriado_fim.Value, feriado.dd_feriado_fim.Value);

                        retorno.Add(feriado_desconsiderado);

                        feriado_desconsiderado = new FeriadoDesconsiderado();

                        feriado_desconsiderado.feriado_deletado = true;
                        ano = ano + 1; //Faz o segundo ano para garantir a passagem de um ano ao outro.

                        feriado_desconsiderado.dt_inicial = new DateTime(ano, feriado.mm_feriado, feriado.dd_feriado);
                        if (!feriado.mm_feriado_fim.HasValue)
                            feriado.mm_feriado_fim = feriado.mm_feriado;
                        feriado_desconsiderado.dt_final = new DateTime(ano, feriado.mm_feriado_fim.Value, feriado.dd_feriado_fim.Value);

                        retorno.Add(feriado_desconsiderado);
                    }
                }

            return retorno;
        }

        public bool verificarTurmaExisteProgramacaoHorario(int? cd_turma, int? cd_turma_ppt, int cd_horario, int cdEscola)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {

                if (cd_turma_ppt != null && cd_turma_ppt > 0)
                {
                    var existeProg = existeAulaEfetivadaTurma(null, (int)cd_turma_ppt, cdEscola);
                    if (existeProg)
                        throw new TurmaBusinessException(string.Format(Messages.msgCrudProgramacaoEfetuadaFilhaPPT), null, TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA_EFETUADO_PROGRAMACAO, false);
                }
                else
                {
                    var existeProg = existeAulaEfetivadaTurma((int)cd_turma, null, cdEscola);
                    if (existeProg)
                        throw new TurmaBusinessException(string.Format(Messages.msgCrudProgramacaoEfetuada), null, TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA_EFETUADO_PROGRAMACAO, false);
                }
                transaction.Complete();
            }
            return false;
        }

        public bool verificarProgramacaoParaListaHorarios(int cd_turma, int cd_escola, List<Horario> horarios)
        {
            if (horarios != null && horarios.Count() > 0)
                foreach (var h in horarios)
                {
                    //var existeProg = daoTurma.verificarTurmaExisteProgramacaoHorario(cd_turma, h.cd_horario, cd_escola);
                    //if (existeProg)
                    var existeProg = existeAulaEfetivadaTurma(cd_turma, null, cd_escola);
                    if (existeProg)
                        throw new TurmaBusinessException(string.Format(Messages.msgCrudProgramacaoEfetuada), null, TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA_EFETUADO_PROGRAMACAO, false);
                }
            return true;
        }
        public bool cancelarProgramacaoTurma(List<int> cds_programacao_turma, int cd_pessoa_escola)
        {
            bool retorno = false;
            if(cds_programacao_turma != null && cds_programacao_turma.Count > 0)
            {
                foreach (int cd_programacao_turma in cds_programacao_turma)
                {
                    ProgramacaoTurma progContext = daoProgTurma.findById(cd_programacao_turma, false);
                    using (var transaction =
                        TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED,
                            daoProgTurma.DB()))
                    {
                        progContext.id_prog_cancelada = true;
                        daoProgTurma.saveChanges(false);
                        transaction.Complete();
                    }
                }

                retorno = true;
            }
            
            return retorno;
        }

        public bool desfazerCancelarProgramacaoTurma(List<int> cds_programacao_turma, int cd_pessoa_escola)
        {
            bool retorno = false;
            if (cds_programacao_turma != null && cds_programacao_turma.Count > 0)
            {
                foreach (int cd_programacao_turma in cds_programacao_turma)
                {
                    ProgramacaoTurma progContext = daoProgTurma.findById(cd_programacao_turma, false);
                    using (var transaction =
                        TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED,
                            daoProgTurma.DB()))
                    {
                        progContext.id_prog_cancelada = false;
                        daoProgTurma.saveChanges(false);
                        transaction.Complete();
                    }
                }

                retorno = true;
            }

            return retorno;
        }


        public int verificaExisteTurma(string nomeTurma, int cdEscola, int cdTurma)
        {
            return daoTurma.verificaExisteTurma(nomeTurma, cdEscola, cdTurma);
        }

        public int getNumeroVagas(int id, int tipoConsulta, int cd_escola)
        {
            return daoTurma.getNumeroVagas(id, tipoConsulta, cd_escola);
        }

        public IEnumerable<TurmaSearch> searchTurmaAluno(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
                                                  int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas, int cdAluno, int tipoConsulta, bool retorno, int cd_escola_combo_fk)
        {
            IEnumerable<TurmaSearch> retornoPesq = new List<TurmaSearch>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_turma";
                parametros.sort = parametros.sort.Replace("dtaIniAula", "dt_inicio_aula");
                List<Turma> turmasMatRemat = daoTurma.getTurmaAlunoMatRemat(cdEscola, cdAluno);

                retornoPesq = daoTurma.searchTurmaAluno(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, tipoProg, cdEscola, turmasFilhas,
                                                    cdAluno, tipoConsulta, retorno, cd_escola_combo_fk);
                transaction.Complete();
            }
            return retornoPesq;
        }
        public IEnumerable<TurmaSearch> searchTurmasContrato(int cd_contrato, int cd_escola, int cd_aluno)
        {
            IEnumerable<TurmaSearch> turmaCont = new List<TurmaSearch>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                turmaCont = daoTurma.searchTurmasContrato(cd_contrato, cd_escola, cd_aluno).ToList();
                transaction.Complete();
            }
            return turmaCont;
        }

        [Obsolete]
        public bool verifTurmasFilhasDisponiveisHorariosTurmaPPTBD(int cd_turma_PPT, int cd_escola, List<Horario> horariosTurmaPPT)
        {
            bool ocorreuIntersecao = daoTurma.verifTurmasFilhasDisponiveisHorariosTurmaPPTBD(cd_turma_PPT, cd_escola, horariosTurmaPPT);
            if (ocorreuIntersecao)
                throw new TurmaBusinessException(string.Format(Messages.msgIntersecaoHorariosFilhasPPT), null, TurmaBusinessException.TipoErro.ERRO_HORARIOS_FILHAS_INTERCESSAO_HORARIOS_PPT, false);
            return ocorreuIntersecao;
        }

        //Metodo que verifica se todos os horários das turmas filhas estão contidas em algum dos horários da turma PPT.
        public void verifTurmasFilhasDisponiveisHorariosTurmaPPT(List<Horario> horariosTurmaPPT, List<Turma> allTurmasFilhas)
        {
            //allTurmasFilhas = allTurmasFilhas.Where(x => x.cd_situacao_aluno_turma == (int)Turma.SituacaoAlunoTurma.REMATRICULADO ||
            //                                                                   x.cd_situacao_aluno_turma == (int)Turma.SituacaoAlunoTurma.ATIVO ||
            //                                                                   x.cd_situacao_aluno_turma == (int)Turma.SituacaoAlunoTurma.AGUARDANDO).ToList();
            List<Horario> allHorariosTurmasFilhas = new List<Horario>();
            for (int i = 0; i < allTurmasFilhas.Count; i++)
                if (allTurmasFilhas[i].horariosTurma != null && allTurmasFilhas[i].horariosTurma.Count > 0)
                    allHorariosTurmasFilhas = allHorariosTurmasFilhas.Concat(allTurmasFilhas[i].horariosTurma).ToList();

            bool todos_contidos = true;

            if (allHorariosTurmasFilhas != null && allHorariosTurmasFilhas.Count() > 0)
                for (int i = 0; todos_contidos && i < allHorariosTurmasFilhas.Count; i++)
                {
                    Horario turma_filha = allHorariosTurmasFilhas[i];
                    if (horariosTurmaPPT.Where(ht_pai => ht_pai.id_dia_semana == turma_filha.id_dia_semana
                                        && ht_pai.dt_hora_ini <= turma_filha.dt_hora_ini
                                        && ht_pai.dt_hora_fim >= turma_filha.dt_hora_fim).Count() <= 0)
                        todos_contidos = false;
                }
            if (!todos_contidos)
                throw new TurmaBusinessException(string.Format(Messages.msgIntersecaoHorariosFilhasPPT), null, TurmaBusinessException.TipoErro.ERRO_HORARIOS_FILHAS_INTERCESSAO_HORARIOS_PPT, false);
        }

        public Turma getTurmaEHorarios(int cdTurma, int cdEscola)
        {
            return daoTurma.getTurmaEHorarios(cdTurma,cdEscola);
        }

        public IEnumerable<TurmaSearch> searchTurmaComAluno(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto,
                                                   int situacaoTurma, int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola,
                                                   bool turmasFilhas, int cdAluno, DateTime? dtInicial, DateTime? dtFinal, int? cd_turma_PPT, int cdTurmaOri, int opcao, int tipoConsulta, int cd_escola_combo_fk)
        {
            IEnumerable<TurmaSearch> retorno = new List<TurmaSearch>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_turma";
                parametros.sort = parametros.sort.Replace("dtaIniAula", "dt_inicio_aula");

                retorno = daoTurma.searchTurmaComAluno(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, tipoProg, cdEscola, turmasFilhas, cdAluno, dtInicial, dtFinal, cd_turma_PPT, cdTurmaOri, opcao, tipoConsulta, cd_escola_combo_fk);
                transaction.Complete();
            }
            return retorno;
        }

        public Turma getTurmaOrigem(int cdEscola, int cd_turma, int cd_aluno)
        {
            return daoTurma.findTurmaOrigem(cdEscola, cd_turma, cd_aluno);
        }

        public List<Turma> getTurmasOrigem(List<AlunoTurma> alunosTurma, int cdEscola)
        {
            List<Turma> listaTurma = new List<Turma>();
            if (alunosTurma != null && alunosTurma.Count() > 0)
                foreach (AlunoTurma aluno in alunosTurma)
                {
                    Turma turmaOrigem = daoTurma.findTurmaOrigem(cdEscola, aluno.cd_turma, aluno.cd_aluno);
                    listaTurma.Add(turmaOrigem);
                }
            return listaTurma;
        }

        public bool getVerificaDadosMudanca(List<AlunoTurma> alunos, int cdEscola)
        {
            bool retorno = true;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED,daoTurma.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                if (alunos.Count() > 0)
                {
                    TurmaSearch turma = daoTurma.getTurmaByCodMudancaOuEncerramento(alunos[0].cd_turma, cdEscola);
                    if (turma != null && turma.cd_turma > 0)
                    {
                        if (turma.id_turma_ppt)
                        {
                            if (alunos.Count() > 1)
                            {
                                retorno = false;
                                throw new TurmaBusinessException(string.Format(Messages.msgErroVagasTurma), null, TurmaBusinessException.TipoErro.ERRO_NUMERO_VAGAS, false);
                            }
                        }
                        else
                        {

                            if (turma.considera_vagas == true)
                            {
                                int vagasDisp = turma.vagas_disponiveis - turma.nro_alunos;
                                if (vagasDisp < alunos.Count())
                                {
                                    retorno = false;
                                    throw new TurmaBusinessException(string.Format(Messages.msgErroVagasTurma), null, TurmaBusinessException.TipoErro.ERRO_NUMERO_VAGAS, false);
                                }
                            }
                        }
                        retorno = alunoBiz.verificacoesMudanca(alunos, cdEscola);
                    }
                    else
                    {
                        retorno = false;
                        throw new TurmaBusinessException(string.Format(Messages.msgRegNotEnc), null, TurmaBusinessException.TipoErro.ERRO_DATA_NULL, false);
                    }
                }
                else
                {
                    retorno = false;
                    throw new TurmaBusinessException(string.Format(Messages.msgRegNotEnc), null, TurmaBusinessException.TipoErro.ERRO_DATA_NULL, false);
                }
                transaction.Complete();
            }
           return retorno;
        }
        public IEnumerable<TurmaSearch> searchTurmaAlunoDesistente(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
                                               int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas, DateTime? dtInicial,
                                               DateTime? dtFinal, int tipoConsulta)
        {
            IEnumerable<TurmaSearch> retorno = new List<TurmaSearch>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_turma";
                parametros.sort = parametros.sort.Replace("dtaIniAula", "dt_inicio_aula");

                retorno = daoTurma.searchTurmaAlunoDesistente(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, tipoProg, cdEscola, turmasFilhas, dtInicial, dtFinal, tipoConsulta);
                transaction.Complete();
            }
            return retorno;
        }

        public int getTumaAlunoByIdTurma(int cd_turma, int cd_escola)
        {
            return daoTurma.getTumaAlunoByIdTurma(cd_turma, cd_escola);
        }
        public Turma getTurmaAlunoAguard(int cd_pessoa_aluno,DateTime dataHoje, int cd_empresa)
        {
            Turma turmaAluno =  daoTurma.getTurmaAlunoAguard(cd_pessoa_aluno, cd_empresa);
            if(turmaAluno == null || turmaAluno.cd_turma > 0)
               turmaAluno = new Turma();;
            turmaAluno.titulos_atrazados = financeiroBiz.verificaTituloVencido(cd_pessoa_aluno, dataHoje,  cd_empresa);
            return turmaAluno;
        }

        public TurmaSearch searchTurmaComAlunoDesistencia(int cdEscola, int cdAluno, int opcao)
        {
            return daoTurma.searchTurmaComAlunoDesistencia(cdEscola, cdAluno, opcao);
        }

        public IEnumerable<TurmaSearch> getRptTurmas(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int prog, bool turmasFilhas,
           DateTime? pDtaI, DateTime? pDtaF, DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int situacaoTurma, List<int> situacaoAlunoTurma, int tipoOnline, string dias)
        {
            IEnumerable<TurmaSearch> retorno = new List<TurmaSearch>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, daoTurma.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = daoTurma.getRptTurmas(tipoTurma, cdCurso, cdDuracao, cdProduto, cdProfessor, prog, turmasFilhas, pDtaI, pDtaF, pDtaFimI, pDtaFimF, cd_escola, situacaoTurma, situacaoAlunoTurma, tipoOnline, dias);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<TurmaSearch> getRptTurmasAEncerrar(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int tipoProg, bool turmasFilhas,
           DateTime? pDtaI, DateTime? pDtaF, DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int tipoOnline, string dias)
        {
            IEnumerable<TurmaSearch> retorno = new List<TurmaSearch>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED,daoTurma.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = daoTurma.getRptTurmasAEncerrar(tipoTurma, cdCurso, cdDuracao, cdProduto, cdProfessor, tipoProg, turmasFilhas, pDtaI, pDtaF, pDtaFimI, pDtaFimF, cd_escola, tipoOnline, dias);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<TurmaSearch> getRptTurmasNovas(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int tipoProg, bool turmasFilhas,
           DateTime? pDtaI, DateTime? pDtaF, int cd_escola, int tipoOnline, string dias)
        {
            IEnumerable<TurmaSearch> turmasNovas = new List<TurmaSearch>();

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                turmasNovas = daoTurma.getRptTurmasNovas(tipoTurma, cdCurso, cdDuracao, cdProduto, cdProfessor, tipoProg, turmasFilhas, pDtaI, pDtaF, cd_escola, tipoOnline, dias).ToList();
                foreach (var turma in turmasNovas)
                {
                    var dozeHoras = new TimeSpan(12,0,0); 
                    var dezoitoHoras = new TimeSpan(18,0,0); 
                    var zeroHoras = new TimeSpan(23,59,59);
                    var horarios = turma.horarios.ToList();
                    double qtd_minutos_turma = 0;

                    foreach (var horario in horarios)
                    {
                        if (horario.dt_hora_ini < dozeHoras)
                        {
                            turma.turno = (int)Turno.MANHA;
                        }
                        if (horario.dt_hora_ini >= dozeHoras && horario.dt_hora_ini < dezoitoHoras)
                        {
                            turma.turno = (int)Turno.TARDE;
                        }
                        if (horario.dt_hora_ini >= dezoitoHoras && horario.dt_hora_ini < zeroHoras)
                        {
                            turma.turno = (int)Turno.NOITE;
                        }
                    }

                    var hashtableHorarios = Horario.montaDiaHorario(horarios, ref qtd_minutos_turma);
                    turma.dias = Horario.formatDias(hashtableHorarios).Replace("; ","/")
                                                        .Replace("domingo", "DOM").Replace("segunda-feira", "SEG").Replace("terça-feira", "TER")
                                                        .Replace("quarta-feira", "QUA").Replace("quinta-feira", "QUI").Replace("sexta-feira", "SEX")
                                                        .Replace("sábado", "SÁB");

                    turma.horario = Horario.getDescricaoSimplificadaHorarios(hashtableHorarios).Replace(";", "").Replace(" às ", "/").Replace(", ", "\n");

                    //if(turma.horarios.Where(x => x.))
                }
                transaction.Complete();
            }
            return turmasNovas;
        }

        public IEnumerable<TurmaSearch> getRptTurmasProgAula(int cd_turma, int cd_escola, DateTime? pDtaI, DateTime? pDtaF)
        {
            IEnumerable<TurmaSearch> retorno = new List<TurmaSearch>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoTurma.getRptTurmasProgAula(cd_turma, cd_escola, pDtaI, pDtaF);
                transaction.Complete();
            }
            return retorno;
        }

        public int getQuantidadeAulasProgramadasTurma(int cd_turma, int cd_pessoa_escola)
        {

         return daoProgTurma.getQuantidadeAulasProgramadasTurma(cd_turma, cd_pessoa_escola);

        }

        public TurmaSearch postCancelarEncerramento(Turma turma, int cdEscola, int cdUsuario)
        {

            TurmaSearch turmaEncerrada = new TurmaSearch();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED,daoTurma.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                sincronizaContexto(daoTurma.DB());
                if (turma != null && turma.cd_turma > 0)
                {
                    Turma turmaEncerradaContext = daoTurma.findTurmasByIdAndCdEscola(turma.cd_turma, cdEscola);
                    if (turmaEncerradaContext != null && turmaEncerradaContext.dt_termino_turma != null)
                    {
                        //Verifica se existe turma criada a partir da turma encerrada
                        bool existeNovaTurma = daoTurma.getTurmaNovaEnc(turmaEncerradaContext.cd_turma);
                        if (existeNovaTurma)
                        {
                            
                            bool rematriculados = daoTurma.getVerificaContrato(turmaEncerradaContext.cd_turma);
                            if (rematriculados == true) {
                                throw new TurmaBusinessException(Utils.Messages.Messages.msgCancelaEncerraRematricula, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_CONTRATO_COM_CONTRATO_ANTERIOR_IGUAL_CONTRATO_ALUNO_TURMA, false);
                            }

                            //TO DO: Karoline
                            Turma novaTurma = daoTurma.getTurmaNovaEncSituacao(turmaEncerradaContext.cd_turma);
                            if (novaTurma == null)
                                throw new TurmaBusinessException(Utils.Messages.Messages.msgErroCancelEncSitAluno, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_ALUNO_MAT_TURMA_NOVA, false);
                            else
                            {
                                int[] turmas = { novaTurma.cd_turma };

                                //Se existir e todos os alunos estiverem aguardando, deletar a turma (aluno turma, professor e programação)
                                //Deletando professores da turma
                                List<ProfessorTurma> professorTurmaNova = daoProfessorTurma.findProfessoresTurmaPorTurmaEscola(novaTurma.cd_turma, cdEscola).ToList();
                                if (professorTurmaNova.Count() > 0)
                                    foreach (var item in professorTurmaNova)
                                        if (item != null)
                                        {
                                            bool existeProg = false;
                                            string mensage = "";
                                            if (novaTurma.id_turma_ppt)
                                            {
                                                existeProg = daoDiarioAula.verificarDiarioAulaEfetivadoProfTurmasFilhasPPT(turma.cd_turma, item.cd_professor, turma.cd_pessoa_escola);
                                                mensage = Messages.msgRemoveProfDiarioAulaFilhasPPT;
                                            }
                                            else
                                            {
                                                existeProg = daoDiarioAula.verificaDiarioAulaEfetivadoProf(turma.cd_turma, item.cd_professor, turma.cd_pessoa_escola);
                                                mensage = Messages.msgRemoveProfDiarioAula;
                                            }
                                            if (existeProg)
                                                throw new TurmaBusinessException(string.Format(mensage), null, TurmaBusinessException.TipoErro.ERRO_EXISTE_DIARIO_AULA_TURMA, false);
                                            daoProfessorTurma.delete(item, false);
                                        }
                                List<AlunoTurma> alunosNova = alunoBiz.findAlunosTurmaPorTurmaEscola(novaTurma.cd_turma, cdEscola).ToList();

                                if (alunosNova != null && alunosNova.Count() > 0)
                                    foreach (var item in alunosNova)
                                        if (item != null)
                                            secretariaBiz.deletarAlunoTurma(item);
                                deleteTurmas(turmas, cdEscola);
                            }
                        }
                        //Verificando se existe alunos ou professores ocupado com o mesmo horário da turma que deseja retornar
                        alunosProfsOcupado(turmaEncerradaContext.cd_turma, cdEscola, turmaEncerradaContext.cd_turma_ppt, turmaEncerradaContext.cd_produto);
                        situacaoAlunoAntesEncerramento(cdUsuario, cdEscola, turmaEncerradaContext.cd_produto, turmaEncerradaContext.dt_termino_turma.Value, turmaEncerradaContext.cd_turma);
                        turmaEncerradaContext.dt_termino_turma = null;
                        daoTurma.saveChanges(false);

                        if (turmaEncerradaContext.cd_turma > 0)
                        {
                            if (BusinessApiNewCyber.aplicaApiCyber())
                            {
                                //busca a turma e chama a api ciber com o comando para (ATIVAR_GRUPO)
                                buscarTurmaAtivarGrupoApiCyber(turmaEncerradaContext.cd_turma);
                            }
                        }

                    }
                    turmaEncerrada = daoTurma.getTurmaByCodForGrid(turmaEncerradaContext.cd_turma, cdEscola, turmaEncerradaContext.cd_turma_ppt > 0? true: false);
                }
                transaction.Complete();
            }
            return turmaEncerrada;
        }

        private void alunosProfsOcupado(int cdTurma, int cdEscola, int? cdTurmaPPTPai, int cd_produto)
        {
            List<ProfessorTurma> professorTurmaContext = daoProfessorTurma.findProfessoresTurmaPorTurmaEscola(cdTurma, cdEscola).ToList();
            List<Horario> horarios = secretariaBiz.getHorarioByEscolaForRegistro(cdEscola, cdTurma, Horario.Origem.TURMA).ToList();
            if (horarios != null)
            {    //Lista de professores ativos, para verificar se tem horarios disponivel
                List<ProfessorTurma> listaProf = professorTurmaContext.Where(p => p.id_professor_ativo == true).ToList();
                TurmaAlunoProfessorHorario TurmaProfessorHorario;
                foreach (Horario h in horarios)
                {
                    //Verificar se os professores tem horarios disponivel
                    IEnumerable<Professor> profOK = new List<Professor>();
                    int[] profs = {};
                    if (listaProf != null && listaProf.Count() > 0)
                        profs = listaProf.Select(x => x.cd_professor).ToArray();
                    TurmaProfessorHorario = new TurmaAlunoProfessorHorario
                    {
                        cd_turma = cdTurma,
                        horario = h,
                        professores = profs
                    };
                    if (cdTurmaPPTPai != null && cdTurmaPPTPai > 0)
                        TurmaProfessorHorario.cd_turma = cdTurmaPPTPai.Value;
                    IEnumerable<Professor> profsDisp = professorBusiness.getRetProfessoresDisponiveisFaixaHorarioTurma(TurmaProfessorHorario, cdEscola);
                    profOK = profsDisp.Where(p => profs.Contains(p.cd_funcionario));
                    if (listaProf.Count() > 0 && profOK.Count() != listaProf.Count())
                    {
                        int profOcupado;
                        if (profOK != null && profOK.Count() > 0)
                        {
                            var listprofOK = listaProf.Where(l => profOK.Where(p => p.cd_funcionario != l.cd_professor).Any());
                            if (listprofOK != null && listprofOK.Count() > 0)
                                profOcupado = listprofOK.FirstOrDefault().cd_professor;
                            else profOcupado = profOK.FirstOrDefault().cd_funcionario;
                            //profOcupado = listaProf.Where(l => profOK.Where(p => p.cd_funcionario != l.cd_professor).Any()).FirstOrDefault().cd_professor;
                        }
                        else
                            profOcupado = listaProf.FirstOrDefault().cd_professor;
                        FuncionarioSearchUI prof = professorBusiness.getProfessoresById(profOcupado, cdEscola);
                        throw new TurmaBusinessException(string.Format(Messages.msgErroHorarioOcupadoVirada, "professor " + prof.no_pessoa), null, TurmaBusinessException.TipoErro.ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA, false);
                    }
                    //Método de verificação dos alunos que não estavam movidos ou desistentes e que não estejam em outras turmas no mesmo horário.
                    alunoBiz.getAlunosDisponiveisFaixaHorarioCancelEnc(cdTurma, cdTurmaPPTPai, cd_produto, cdEscola, h);
                }
            }
        }

        private void situacaoAlunoAntesEncerramento(int cd_usuario, int cd_pessoa_escola, int cd_produto, DateTime dataTermino, int cd_turma)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                List<AlunoTurma> todosAlunos = secretariaBiz.findAlunosTurmaPorTurmaEscola(cd_turma, cd_pessoa_escola).ToList();
                if (todosAlunos.Count() > 0)
                {
                    foreach (AlunoTurma at in todosAlunos)
                    {
                        if (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado)
                        {
                            HistoricoAluno historicoAnterior = secretariaBiz.getSituacaoAlunoCancelEncerramento(at.cd_aluno, cd_turma, dataTermino);
                            int[] cdAlunos = { at.cd_aluno };
                            if (at.cd_contrato != null && historicoAnterior != null && historicoAnterior.cd_historico_aluno > 0)
                            {
                                byte sequenciaMax = (byte)secretariaBiz.retunMaxSequenciaHistoricoAluno(cd_produto, cd_pessoa_escola, at.cd_aluno);

                                HistoricoAluno historico = new HistoricoAluno
                                {
                                    cd_aluno = at.cd_aluno,
                                    cd_turma = cd_turma,
                                    cd_contrato = historicoAnterior.cd_contrato,
                                    id_tipo_movimento = (int)HistoricoAluno.TipoMovimento.CANCELA_ENCERRAMENTO,
                                    dt_cadastro = DateTime.UtcNow,
                                    dt_historico = DateTime.Now.Date,
                                    cd_produto = cd_produto,
                                    nm_sequencia = ++sequenciaMax,
                                    id_situacao_historico = historicoAnterior.id_situacao_historico,
                                    cd_usuario = cd_usuario
                                };
                                secretariaBiz.addHistoricoAluno(historico);
                                //Voltando a situação do aluno na turma para a anterior ao encerramento
                                at.cd_situacao_aluno_turma = historicoAnterior.id_situacao_historico;
                                alunoBiz.saveChagesAlunoTurma(at);
                            }
                            else
                            {
                                //Voltando a situação do aluno na turma para aguardando
                                at.cd_situacao_aluno_turma = (int)AlunoTurma.SituacaoAlunoTurma.Aguardando;
                                alunoBiz.saveChagesAlunoTurma(at);
                            }
                        }
                    }
                }
                transaction.Complete();
            }
        }

        public Turma getDadosTurmaMud(int cdEscola, int cd_turma)
        {
            return daoTurma.getDadosTurmaMud(cdEscola, cd_turma);
        }

        public IEnumerable<Turma> getTurmasPersonalizadas(int cdProduto, DateTime dtAula, TimeSpan hrIni, TimeSpan hrFim, int cdEscola, int? cd_turma)
        {
            return daoTurma.getTurmasPersonalizadas(cdProduto, dtAula, hrIni, hrFim, cdEscola, cd_turma);
        }

        public List<ReportPercentualTerminoEstagio> getRptPercentualTerminoEstagio(int cd_professor, DateTime? dt_ini, DateTime? dt_fim, int cd_escola)
        {
            List<ReportPercentualTerminoEstagio> retorno = new List<ReportPercentualTerminoEstagio>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoTurma.getRptPercentualTerminoEstagio(cd_professor, dt_ini, dt_fim, cd_escola);
                transaction.Complete();
            }
            return retorno;
        }

        public string postGerarRematricula(string dc_turma, int cd_usuario, Nullable<System.DateTime> dt_inicial, Nullable<System.DateTime> dt_final, Nullable<bool> id_turma_nova, Nullable<int> cd_layout, Nullable<System.DateTime> dt_termino, int fusoHorario)
        {
            string retorno = null;
            List<int> idsTurmaEnc = new List<int>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                
                retorno = daoTurma.postGerarRematricula(dc_turma, cd_usuario, dt_inicial, dt_final, id_turma_nova, cd_layout, dt_termino, fusoHorario);

                transaction.Complete();
             
            }

            if (BusinessApiNewCyber.aplicaApiCyber())
            {
                if (id_turma_nova == true && !String.IsNullOrEmpty(dc_turma))
                {
                    idsTurmaEnc = dc_turma.Split('|').Where(x => !String.IsNullOrEmpty(x)).Select(x => Int32.Parse(x)).ToList();

                    if (idsTurmaEnc != null && idsTurmaEnc.Count > 0)
                    {
                        foreach (int cd_turma_enc in idsTurmaEnc)
                        {
                            //Pega a nova turma
                            int cd_nova_turma = findNovaTurmaByCdTurmaEncerrada(cd_turma_enc);

                            if (cd_nova_turma > 0)
                            {

                                //Turma do banco
                                Turma turmaNova = findTurmaByCdTurmaApiCyber(cd_nova_turma);

                                //Pega os professores ativos da turma no banco(Current)
                                List<ProfessorTurma> listProfessorTurmaBdCurrent = findProfessorTurmaByCdTurma(turmaNova.cd_turma, turmaNova.cd_pessoa_escola);

                                //Percorre a lista de professores e verifica se já existem no cyber,
                                //se não, cadastra o professor no cyber
                                foreach (ProfessorTurma professorTurma in listProfessorTurmaBdCurrent)
                                {
                                    FuncionarioCyberBdUI funcionarioCyberBdCurrent = findFuncionarioByCdFuncionario(professorTurma.cd_professor, turmaNova.cd_pessoa_escola);

                                    if (funcionarioCyberBdCurrent != null && funcionarioCyberBdCurrent.id_unidade != null && funcionarioCyberBdCurrent.id_unidade > 0 &&
                                        funcionarioCyberBdCurrent.funcionario_ativo == true)
                                    {
                                        //Chama a apiCyber de acordo com o tipo de funcionario
                                        verificaTipoFuncionarioPostApiCyber(funcionarioCyberBdCurrent);
                                    }
                                }


                                //monta a lista com professores turma do banco (Current)
                                TurmaApiCyberBdUI turmaApiCyberCurrent = new TurmaApiCyberBdUI();
                                turmaApiCyberCurrent = findTurmaApiCyber(turmaNova.cd_turma, turmaNova.cd_pessoa_escola);

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
                                    sp_verificar_grupo_cyber(turmaApiCyberCurrent.nome_grupo, turmaApiCyberCurrent.id_unidade);

                                    cadastraGruposApiCyber(turmaApiCyberCurrent, ApiCyberComandosNames.CADASTRA_GRUPO);
                                }


                                List<LivroAlunoApiCyberBdUI> alunosAtivosCadastraLivroAluno = findAlunoTurmaAtivosByCdTurma(cd_nova_turma);

                                //se livro existe no bd e tem o codigo_livro -> (aluno e grupo) existe no cyber -> livroAluno não existe no cyber -> chama a apicyber com comando (CADASTRA_LIVROALUNO)
                                if (alunosAtivosCadastraLivroAluno != null && alunosAtivosCadastraLivroAluno.Count > 0)
                                {
                                    foreach (LivroAlunoApiCyberBdUI alunoAtivoCadastroLivroAluno in alunosAtivosCadastraLivroAluno)
                                    {
                                        if (alunoAtivoCadastroLivroAluno != null && alunoAtivoCadastroLivroAluno.codigo_unidade != null && alunoAtivoCadastroLivroAluno.codigo_livro > 0 &&
                                            existeAluno(alunoAtivoCadastroLivroAluno.codigo_aluno) &&
                                            existeGrupoByCodigoGrupo(alunoAtivoCadastroLivroAluno.codigo_grupo) &&
                                            !existeLivroAlunoByCodAluno(alunoAtivoCadastroLivroAluno.codigo_aluno, alunoAtivoCadastroLivroAluno.codigo_grupo, alunoAtivoCadastroLivroAluno.codigo_livro))
                                        {
                                            cadastraLivroAlunoApiCyber(alunoAtivoCadastroLivroAluno, ApiCyberComandosNames.CADASTRA_LIVROALUNO);
                                        }
                                    }
                                }




                            }
                        }

                    }

                }
            }

            return retorno;
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
                if (!existeFuncionario(("P" + funcionarioCyberBd.codigo), ApiCyberComandosNames.VISUALIZA_PROFESSOR))
                {
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
            return BusinessApiNewCyber.verificaRegistroFuncionario(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo);
        }

        private void executaCyberCadastraFuncionario(string parametros, string comando)
        {
            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
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

        public bool existeAluno(int codigo)
        {
            return BusinessApiNewCyber.verificaRegistro(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.VISUALIZA_ALUNO, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo);
        }

        public bool existeLivroAlunoByCodAluno(int codigo_aluno, int codigo_grupo, int codigo_livro)
        {
            return BusinessApiNewCyber.verificaRegistroLivroAlunos(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.VISUALIZA_LIVROALUNO, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo_aluno, codigo_grupo, codigo_livro);
        }

        private void cadastraLivroAlunoApiCyber(LivroAlunoApiCyberBdUI livroAlunoCyberCurrent, string comando)
        {

            string parametros = "";

            parametros = validaParametrosCyberCadastroLivroAluno(livroAlunoCyberCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");

            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
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
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCodigoGrupoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_LIVRO_MENOR_IGUAL_ZERO, false);
            }


            string listaParams = "";
            listaParams = string.Format("codigo_aluno={0},codigo_grupo={1},codigo_livro={2}", entity.codigo_aluno, entity.codigo_grupo, entity.codigo_livro);
            return listaParams;
        }

        private void cadastraGruposApiCyber(TurmaApiCyberBdUI turmaApiCyberCurrent, string comando)
        {

            string parametros = "";

            parametros = validaParametrosCyberCadastro(turmaApiCyberCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");

            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
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

        // NOVO CANCELAR ENCERRAMENTO - ANTES DA PROCEDURE sp_cancelar_rematricula 
        public string postCancelarEncerramento(Nullable<int> cd_turma, Nullable<int> cd_usuario, Nullable<int> fuso)
        {
            string retorno = null;

            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            //{
            retorno = daoTurma.postCancelarEncerramento(cd_turma, cd_usuario, fuso);

            if (cd_turma != null && cd_turma > 0 && retorno == "0")
            {
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    if (BusinessApiNewCyber.aplicaApiCyber())
                    {
                        //busca a turma e chama a api ciber com o comando para (ATIVAR_GRUPO)
                        buscarTurmaAtivarGrupoApiCyber(cd_turma);
                    }

                    transaction.Complete();

                }
            }
            return retorno;
        }
        public string postRefazerProgramacao(int cdTurma)
        {
            string retorno = null;

            retorno = daoTurma.postRefazerProgramacao(cdTurma);

            return retorno;
        }
        public string postRefazerNumeracao(int cdTurma)
        {
            string retorno = null;

            retorno = daoTurma.postRefazerNumeracao(cdTurma);

            return retorno;
        }

        public int postCancelarTurmasEncerramento(List<Turma> turmas, Nullable<int> cd_usuario, Nullable<int> fuso)
        {
            int retorno = 0;
            var dc_turma = String.Join("|", turmas.Select(t => t.cd_turma));

            retorno = daoTurma.postCancelarTurmasEncerramento(dc_turma, cd_usuario, fuso);

            return retorno;

            //Aqui não vai executar Cyber pois estamos voltando um encerramento
        }


        private void buscarTurmaAtivarGrupoApiCyber(int? cd_turma)
        {
            TurmaApiCyberBdUI turmaApiCyberCurrent = new TurmaApiCyberBdUI();
            turmaApiCyberCurrent = findTurmaCancelarEncerramentoApiCyber((int) cd_turma);

            //se for regular ou ppt filha e tiver nm_integracao
            if (((turmaApiCyberCurrent.id_turma_ppt == false && turmaApiCyberCurrent.cd_turma_ppt == null && turmaApiCyberCurrent.id_unidade != null) ||
                (turmaApiCyberCurrent.id_turma_ppt == false && (turmaApiCyberCurrent.cd_turma_ppt != null && turmaApiCyberCurrent.cd_turma_ppt > 0) && turmaApiCyberCurrent.id_unidade != null)) && existeGrupoByCodigoGrupo(turmaApiCyberCurrent.codigo))
            {
                //Chama api cyber com o comando (ATIVA_GRUPO)
                AtivaGruposApiCyber(turmaApiCyberCurrent, ApiCyberComandosNames.ATIVA_GRUPO);
            }
        }

        public List<Horario> verificaDiaSemanaTurmaFollowUp(int cdEscola, int cdTurma, int idDiaSemanaTurma)
        {
            return daoTurma.verificaDiaSemanaTurmaFollowUp(cdEscola, cdTurma, idDiaSemanaTurma);
        }

        public FuncionarioCyberBdUI findFuncionarioByCdFuncionario(int cd_funcionario, int cd_empresa)
        {
            return professorBusiness.findFuncionarioByCdFuncionario(cd_funcionario, cd_empresa);
        }

        public void sp_verificar_grupo_cyber(string no_turma, Nullable<int> id_unidade)
        {
             daoTurma.sp_verificar_grupo_cyber(no_turma, id_unidade);
        }

        #endregion

        #region Programação Turma

        public ProgramacaoTurmaAbaUI getProgramacoesTurma(int cd_turma, int cd_escola)
        {
            ProgramacaoTurmaAbaUI retorno = new ProgramacaoTurmaAbaUI();
            retorno.Programacoes = daoProgTurma.getProgramacaoTurmaByTurma(cd_turma, cd_escola,DataAccess.ProgramacaoTurmaDataAccess.TipoConsultaProgTurmaEnum.HAS_PROG_TURMA,false).ToList();
            retorno.FeriadosDesconsiderados = daoFeriadoDesconsiderado.getFeriadoDesconsideradoByTurma(cd_turma, cd_escola).ToList();
            return retorno;
        }
        public ProgramacaoTurmaAbaUI verificaProgramacaoTurma(int cd_turma, int cd_escola)
        {
            ProgramacaoTurmaAbaUI retorno = new ProgramacaoTurmaAbaUI();
            retorno.Programacoes = daoProgTurma.getProgramacaoTurmaByTurma(cd_turma, cd_escola, DataAccess.ProgramacaoTurmaDataAccess.TipoConsultaProgTurmaEnum.HAS_PROG_TURMA_SEM_LANCAR, false).ToList();
            return retorno;
        }

        public IEnumerable<ProgramacaoTurma> getProgramacoesTurma(int cd_escola, int cd_turma, int cd_professor, DateTime? dt_inicial, DateTime dt_final) {
            IEnumerable<ProgramacaoTurma> retorno = new List<ProgramacaoTurma>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, daoProgTurma.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = daoProgTurma.getProgramacoesTurma(cd_escola, cd_turma, cd_professor, dt_inicial, dt_final);
                transaction.Complete();
            }
            return retorno;
        }

        public ProgramacaoHorarioUI getProgramacaoHorarioTurma(int cd_turma, int cd_escola)
        {
            if (daoTurma.ExisteProgramacaoTurma(cd_turma, cd_escola))
                throw new TurmaBusinessException(Messages.msgErrorExisteProgramacao, null,
                TurmaBusinessException.TipoErro.ERRO_EXISTE_PROGRAMACAO_TURMA, false);

            return daoTurma.getProgramacaoHorarioTurma(cd_turma, cd_escola);
        }

        public void atualizaProgramacoesTurma(TransactionScopeBuilder.TransactionType transactionType, List<ProgramacaoTurma> programacoes, int cd_turma, int? posicao)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(transactionType, daoTurma.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            {
                this.sincronizaContexto(daoTurma.DB());
                Turma turma = daoTurma.getTurmaComProgramacoes(cd_turma);

                if(turma.ProgramacaoTurma == null)
                    turma.ProgramacaoTurma = programacoes;
                else {
                    var programacoesAnteriores = turma.ProgramacaoTurma.ToList();
                    if(posicao.HasValue) {
                        for(int i = programacoesAnteriores.Count - 1; i > posicao.Value; i--) {
                            programacoesAnteriores[i].Turma = null;
                            daoProgTurma.deleteContext(programacoesAnteriores[i], false);
                        }
                        daoProgTurma.saveChanges(false);
                        
                        turma = daoTurma.getTurmaComProgramacoes(cd_turma);
                        programacoesAnteriores = turma.ProgramacaoTurma.ToList();

                        // Ordena as novas programações:
                        int qtd = programacoesAnteriores.Count();

                        if(programacoes != null)
                            foreach(ProgramacaoTurma prog in programacoes) {
                                prog.dta_cadastro_programacao = DateTime.UtcNow;
                                prog.nm_aula_programacao_turma = qtd + 1;
                                qtd += 1;
                            }
                    }
                    programacoesAnteriores.AddRange(programacoes);
                    turma.ProgramacaoTurma = programacoesAnteriores;
                    turma.nro_aulas_programadas = (short)turma.ProgramacaoTurma.Where(pt => (pt.cd_feriado == null) || (pt.cd_feriado != null && pt.cd_feriado_desconsiderado != null)).Count();
                    turma.dt_final_aula = programacoesAnteriores[programacoesAnteriores.Count - 1].dta_programacao_turma;
                }
                daoTurma.DB().SaveChanges();
                transaction.Complete();
            }
        }
        
        private IEnumerable<Turma> getTurmaPorProgramacaoTurmaComFeriado(Feriado feriado, int? cd_escola, bool delecao_feriado)
        {
            return daoTurma.getTurmaPorProgramacaoTurmaComFeriado(feriado, cd_escola, delecao_feriado);
        }

        public bool existeAulaEfetivadaTurma(int? cd_turma, int? cd_turma_ppt, int cd_escola)
        {
            return daoProgTurma.existeAulaEfetivadaTurma(cd_turma, cd_turma_ppt, cd_escola);
        }

        public IEnumerable<ProgramacaoTurma> getProgramacaoTurmaByTurma(Componentes.GenericBusiness.TransactionScopeBuilder.TransactionType transactionType, int cd_turma, int cd_escola, ProgramacaoTurmaDataAccess.TipoConsultaProgTurmaEnum tipoConsulta, bool idMostrarFeriado)
        {
            List<ProgramacaoTurma> listProgramacaoTurma = new List<ProgramacaoTurma>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(transactionType, daoProgTurma.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            {
                listProgramacaoTurma = daoProgTurma.getProgramacaoTurmaByTurma(cd_turma, cd_escola, tipoConsulta, idMostrarFeriado).ToList();
                transaction.Complete();
            }
            return listProgramacaoTurma;
        }

        public string existeProgInsuficiente(Turma turma)
        {
            return daoProgTurma.existeProgInsuficiente(turma);
        }
        public IEnumerable<ReportTurmaMatriculaMaterial> getRptTurmasMatriculaMaterial(int cd_escola, int cd_turma, int cd_aluno, int cd_item, int nm_contrato, DateTime? pDtaI, DateTime? pDtaF)
        {
            List<ReportTurmaMatriculaMaterial> retorno = new List<ReportTurmaMatriculaMaterial>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, daoTurma.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = daoTurma.getRptTurmasMatriculaMaterial(cd_escola, cd_turma, cd_aluno, cd_item, nm_contrato, pDtaI, pDtaF).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public Turma buscarTurmaHorariosEditVirada(int cdTurma, int cdEscola, TurmaDataAccess.TipoConsultaTurmaEnum tipo)
        {
            return daoTurma.buscarTurmaHorariosEditVirada(cdTurma, cdEscola, tipo);
        }

        public Turma getTurmaComCursoDuracao(int cd_turma){
            return daoTurma.getTurmaComCursoDuracao(cd_turma);
        }

        public ProgramacoesTurmaSemDiarioAula verificarAlunosPendenciaMaterialDidaticoCurso(int qtd_aulas_sem_material, int cd_turma, int cd_escola, DateTime dt_programacao_turma)
        {
            ProgramacoesTurmaSemDiarioAula progTDiario = new ProgramacoesTurmaSemDiarioAula();
            if (qtd_aulas_sem_material > 0)
            {
                int qtd_aulas_lancadas = daoDiarioAula.quantidadeDiarioAulaTurma(cd_turma, cd_escola);
                if (qtd_aulas_lancadas > qtd_aulas_sem_material)
                {
                    progTDiario.alunos = alunoBiz.verificarAlunosCompraMaterialDidatico(cd_turma, dt_programacao_turma, cd_escola);
                    if (progTDiario.alunos != null && progTDiario.alunos.Count() > 0)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append("Alunos sem material didático:");
                        builder.Append("<ul>");
                        foreach (Aluno a in progTDiario.alunos)
                        {
                            builder.Append("<p>");
                            builder.Append("    <li>");
                            builder.Append(a.nomeAluno);
                            builder.Append("    </li>");
                            builder.Append("<p>");
                        }
                        builder.Append("</ul>");
                        progTDiario.nomeAlunosPendencia = builder.ToString();
                    }
                }
            }
            return progTDiario;
        }


        private void verificarAlunosSemTitulosAPagar(int nm_dias_titulos_abertos, int cd_turma, int cd_escola, DateTime dt_programacao_turma, ref ProgramacoesTurmaSemDiarioAula progTDiario)
        {
            if (nm_dias_titulos_abertos > 0)
            {
                int qtd_aulas_lancadas = daoDiarioAula.quantidadeDiarioAulaTurma(cd_turma, cd_escola);
                if (qtd_aulas_lancadas >= nm_dias_titulos_abertos)
                {
                    List <Aluno>  alunosM = alunoBiz.verificarAlunosSemTitulos(cd_turma, dt_programacao_turma, cd_escola);
                    if (alunosM != null && alunosM.Count() > 0)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append("Alunos sem títulos abertos:");
                        builder.Append("<ul>");
                        foreach (Aluno a in alunosM)
                        {
                            builder.Append("<p>");
                            builder.Append("    <li>");
                            builder.Append(a.nomeAluno);
                            builder.Append("    </li>");
                            builder.Append("<p>");
                        }
                        builder.Append("</ul>");
                        if (!string.IsNullOrEmpty(progTDiario.nomeAlunosPendencia))
                            progTDiario.nomeAlunosPendencia += "<br/>";
                        progTDiario.nomeAlunosPendencia += builder.ToString();
                        if (progTDiario.alunos != null && progTDiario.alunos.Count() > 0)
                            progTDiario.alunos.AddRange(alunosM);
                        else
                            progTDiario.alunos = alunosM;
                    }
                }
            }
        }

        public IEnumerable<ProgramacaoTurma> getProgramacoesTurmaPorAluno(int cd_escola, int cd_aluno, DateTime dt_inicial, int cd_turma_principal, List<int> listaProgs)
        {
            List<int> turmas = daoTurma.getTurmaPorAlunoProg(cd_escola, cd_aluno, dt_inicial);
            List<ProgramacaoTurma> programacoes = new List<ProgramacaoTurma>();
            foreach (int cd in turmas)
            {
                List<ProgramacaoTurma> progs = daoProgTurma.getProgramacoesTurmaPorAluno(cd_escola, cd_aluno, dt_inicial, cd, cd_turma_principal, listaProgs).ToList();
                if (progs.Count() > 5)
                    for (int p = 0; p < 5; p++)
                        programacoes.Add(progs[p]);
                else
                    if(progs.Count() > 0)
                        for (int pr = 0; pr < progs.Count(); pr++)
                            programacoes.Add(progs[pr]);

            }
            return programacoes;
        }

        public bool verificaSeTurmaEFilhaPersonalizada(int cd_turma, int cdEscola)
        {
            return daoTurma.verificaSeTurmaEFilhaPersonalizada(cd_turma, cdEscola);
        }

        #endregion

        #region Avaliacao Turma

        public IEnumerable<AvaliacaoTurmaUI> searchAvaliacaoTurma(SearchParameters parametros, int idTurma, bool? idTipoAvaliacao, int cdEscola, int cdPessoaUsuario, int cd_tipo_avaliacao, int cd_criterio_avaliacao, int cd_curso, int cd_funcionario, DateTime? dta_inicial, DateTime? dta_final, bool isMaster, int cd_escola_combo)
        {
            IEnumerable<AvaliacaoTurmaUI> retorno = new List<AvaliacaoTurmaUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {

                ProfessorUI prof = professorBusiness.verificaRetornaSeUsuarioLogadoEProfessor(cdPessoaUsuario, cdEscola);
                if (prof != null && prof.cd_pessoa > 0 && !isMaster && !prof.id_coordenador)
                    cd_funcionario = prof.cd_pessoa;
                    
                if (parametros.sort == null) parametros.sort = "no_turma";
                retorno = daoAvaliacaoTurma.searchAvaliacaoTurma(parametros, idTurma, idTipoAvaliacao, cdEscola, cd_tipo_avaliacao, cd_criterio_avaliacao, cd_curso, cd_funcionario, dta_inicial, dta_final, cd_escola_combo);
                transaction.Complete();
            }
            return retorno;
        }            

        public List<AvaliacaoTurma> getAvaliacaoTurmaArvore(int idTurma, int idEscola, bool? idConceito, int cdUsuario, int tipoForm, int cd_tipo_avaliacao)
        {
            List<AvaliacaoTurma> retorno = new List<AvaliacaoTurma>();
            
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                sincronizaContexto(daoAvaliacao.DB());
                Turma turma = daoTurma.findTurmasByIdAndCdEscola(idTurma, idEscola);
                
                List<FuncionarioSearchUI> funcionario = professorBusiness.getProfessoresByEmpresa(turma.cd_pessoa_escola, idTurma).ToList();  //Estava idEscola
                //if (idEscola != turma.cd_pessoa_escola) idEscola = turma.cd_pessoa_escola; 
                //Busca as avaliações do curso para criar as avaliações da turma, caso elas ainda não estejam criadas:
                FuncionarioSGF professor = professorBusiness.getProfLogByCodPessoaUsuario(cdUsuario, turma.cd_pessoa_escola); //idEscola

                //Busca as avaliações da turma:
                var avaliacaoTurma = daoAvaliacao.getAvaliacaoCursoExistsTurmaWithCurso(idTurma, turma.cd_pessoa_escola); //idEscola
                if (avaliacaoTurma <= 0)
                    throw new TurmaBusinessException(Utils.Messages.Messages.msgNotExistNotaLancada, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_NOTA_LANCADA, false);

                var alunosTuma = daoTurma.getTumaAlunoByIdTurma(idTurma, turma.cd_pessoa_escola);
                if (alunosTuma <= 0)
                    throw new TurmaBusinessException(Utils.Messages.Messages.msgNotExitsAlunoTurma, null, TurmaBusinessException.TipoErro.ERRO_NAO_EXISTE_ALUNO_TURMA, false);

                //if (tipoForm == (int)TurmaController.TipoFormException.AVALIACAO_TURMA)// && ((professor is Professor) || (professor is FuncionarioSGF)))  
                //    incluirAvaliacoesTurma(idTurma, null);

                if (tipoForm == (int)TurmaController.TipoFormException.AVALIACAO_TURMA)// && ((professor is Professor) || (professor is FuncionarioSGF))) {
                {
                    idConceito = idConceito ?? false;
                    //Sustituido pela proc em IncluirAvaliaçõesTurma
                    //incluirAvaliacoesAluno(idTurma, idEscola);
                }

                var idProfessor = 0;
                if (professor != null)
                    idProfessor = professor is Professor ? professor.cd_funcionario : 0;

                retorno = daoAvaliacaoTurma.getAvaliacaoTurmaArvore(idTurma, idEscola, idConceito, idProfessor, cd_tipo_avaliacao);

                transaction.Complete();
            }

            return retorno;
        }

        public int incluirAvaliacoesTurma(int idTurma, FuncionarioSGF professor)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                var ret = daoAvaliacaoTurma.gerarTurmasNulas(idTurma, 0);  

                if (ret!=0)
                {
                    throw new TurmaBusinessException(Utils.Messages.Messages.msgErroProcedureGerarTurmasnulas, null, TurmaBusinessException.TipoErro.ERRO_PROCEDURE_GERAR_TURMAS_NULAS, false);
                }

                //List<AvaliacaoTurma> novasAvaliacoes = new List<AvaliacaoTurma>();
                //List<Avaliacao> avaliacoesCurso = daoAvaliacao.getAvaliacoesCursoSemAvaliacaoTurma(idTurma).ToList();
                //if (avaliacoesCurso != null)
                //    for (int i = 0; i < avaliacoesCurso.Count; i++)
                //    {
                //        AvaliacaoTurma avaliacao = new AvaliacaoTurma();

                //        avaliacao.cd_avaliacao = avaliacoesCurso[i].cd_avaliacao;
                //        avaliacao.cd_turma = idTurma;
                //        avaliacao.cd_funcionario = null;//professor.cd_funcionario;
                //        avaliacao.dt_avaliacao_turma = null;
                //        novasAvaliacoes.Add(avaliacao);
                //    }

                //if (novasAvaliacoes != null && novasAvaliacoes.Count > 0)
                //    daoAvaliacaoTurma.incluirAvaliacoesTurma(novasAvaliacoes);

                transaction.Complete();
                return ret;
            }
        }

        public AvaliacaoTurmaUI editAvaliacaoTurma(AvaliacaoTurmaUI avaliacaoAlunosUI, int cdUsuario, int idEscola)
        {
            AvaliacaoTurmaUI avaliacaoTurmaUI = new AvaliacaoTurmaUI();
            AvaliacaoTurma avaliacaoTumaBase = new AvaliacaoTurma();
            DateTime? dataAvaliacao = null;
            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, daoAvaliacaoTurma.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            {
                sincronizaContexto(daoAvaliacaoTurma.DB());

                //FuncionarioSGF professor = professorBusiness.getProfLogByCodPessoaUsuario(cdUsuario, idEscola);
                //if (professor == null)
                //    throw new FuncionarioBusinessException(Utils.Messages.Messages.msgErroNaoEhProfessorNemFuncionario, null, FuncionarioBusinessException.TipoErro.ERRO_USUARIO_NAO_FUNCIONARIO, false);

                if (avaliacaoAlunosUI != null && avaliacaoAlunosUI.avalialcoesTurmas != null && avaliacaoAlunosUI.avalialcoesTurmas.Count() > 0)
                {
                    foreach (var avaliacoesT in avaliacaoAlunosUI.avalialcoesTurmas.Where(x => x.isModified == true))
                    {
                        //TODO: melhorar essa consulta
                        if (avaliacoesT.dt_avaliacao_turma != null)
                        {
                            avaliacoesT.dt_avaliacao_turma = ((DateTime)avaliacoesT.dt_avaliacao_turma).Date;
                            dataAvaliacao = avaliacoesT.dt_avaliacao_turma;

                            //Para cada avaliação da turma, copia as datas para a avaliação do aluno:
                            if(avaliacaoAlunosUI.avalialcoesAlunos != null) {
                                List<AvaliacaoAluno> avalialcoesAlunos = avaliacaoAlunosUI.avalialcoesAlunos.Where(x => x.isModifiedA == true).ToList();

                                for(int i=0; i<avalialcoesAlunos.Count; i++)
                                    if(avaliacoesT.cd_avaliacao_turma == avalialcoesAlunos[i].cd_avaliacao_turma)
                                        avalialcoesAlunos[i].dt_avalicao_aluno = dataAvaliacao;
                                avaliacaoAlunosUI.avalialcoesAlunos = avalialcoesAlunos;
                            }
                        }
                        avaliacaoTumaBase = daoAvaliacaoTurma.findById(avaliacoesT.cd_avaliacao_turma, false);
                        if (avaliacaoTumaBase != null)
                        {
                            avaliacaoTumaBase.nm_nota_total_turma = avaliacoesT.nm_nota_total_turma;
                            avaliacaoTumaBase.tx_obs_aval_turma = avaliacoesT.tx_obs_aval_turma;
                            avaliacaoTumaBase.dt_avaliacao_turma = avaliacoesT.dt_avaliacao_turma;
                            avaliacaoTumaBase.cd_funcionario = avaliacoesT.cd_funcionario == 0 ? null : avaliacoesT.cd_funcionario;
                            if (avaliacaoTumaBase.cd_funcionario == null && avaliacaoTumaBase.nm_nota_total_turma > 0)
                                throw new FuncionarioBusinessException(Utils.Messages.Messages.msgErroFuncionarioObrigatorio, null, FuncionarioBusinessException.TipoErro.ERRO_FUNCIONARIO_AVALIACAO_OBRIGATORIO, false);

                            daoAvaliacaoTurma.saveChanges(false);
                        }
                    }

                    if (avaliacaoAlunosUI.avalialcoesAlunos != null && avaliacaoAlunosUI.avalialcoesAlunos.Count() > 0)
                        editListAvaliacao(avaliacaoAlunosUI.avalialcoesAlunos.Where(x => x.isModifiedA == true).ToList());

                    avaliacaoTurmaUI.cd_avaliacao_turma = avaliacaoAlunosUI.cd_avaliacao_turma;
                    avaliacaoTurmaUI.no_turma = avaliacaoAlunosUI.no_turma;
                    avaliacaoTurmaUI.cd_turma = avaliacaoAlunosUI.cd_turma;
                    avaliacaoTurmaUI.cd_tipo_avaliacao = avaliacaoAlunosUI.cd_tipo_avaliacao;
                    avaliacaoTurmaUI.no_tipo_criterio = avaliacaoAlunosUI.no_tipo_criterio;
                    avaliacaoTurmaUI.dt_avaliacao_turma = avaliacaoAlunosUI.dt_avaliacao_turma;
                    avaliacaoTurmaUI.isConceito = avaliacaoAlunosUI.isConceito;
                    avaliacaoTurmaUI.isInFocus = avaliacaoAlunosUI.isInFocus;
                    AvaliacaoAluno[] avaliacaoAlunoArray = avaliacaoAlunosUI.avalialcoesAlunos.ToArray();
                    if(avaliacaoAlunoArray.Length > 0 && avaliacaoAlunoArray[0].cd_conceito != null && avaliacaoAlunoArray[0].cd_conceito > 0)
                        avaliacaoTurmaUI.isConceito = true;
                }
                transaction.Complete();
            }
            return avaliacaoTurmaUI;
        }

        public List<AvaliacaoTurmaUI> returnAvaliacoesConceitoOrNotaByTurma(int cd_turma, int cdUsuario, int idEscola)
        {
           // FuncionarioSGF professor = professorBusiness.getProfLogByCodPessoaUsuario(cdUsuario, idEscola);

            //Busca as avaliações da turma:
            var avaliacaoTurma = daoAvaliacao.getAvaliacaoCursoExistsTurmaWithCurso(cd_turma, idEscola);
            if (avaliacaoTurma <= 0)
                throw new TurmaBusinessException(Utils.Messages.Messages.msgNotExistAvaliacaoTurma, null, TurmaBusinessException.TipoErro.ERRO_NAO_EXISTE_AVALIACAO_TURMA, false);

            //if (((professor is Professor) || (professor is FuncionarioSGF)))
                incluirAvaliacoesTurma(cd_turma, null);

            var alunosTuma = daoTurma.getTumaAlunoByIdTurma(cd_turma, idEscola);
            if (alunosTuma <= 0)
                throw new TurmaBusinessException(Utils.Messages.Messages.msgNotExitsAlunoTurma, null, TurmaBusinessException.TipoErro.ERRO_NAO_EXISTE_ALUNO_TURMA, false);

            var avaliacoesConceitoNota = daoAvaliacaoTurma.returnAvaliacoesConceitoOrNotaByTurma(cd_turma, idEscola);

            //if (((professor is Professor) || (professor is FuncionarioSGF)))
                incluirAvaliacoesAluno(cd_turma, idEscola); 

            //if (avaliacoesConceitoNota.Count() <= 0)
            //    throw new TurmaBusinessException(Utils.Messages.Messages.msgNotExistNotaLancada, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_NOTA_LANCADA, false);

            if (avaliacoesConceitoNota.Count() > 0)
                avaliacoesConceitoNota[0].is_conceito_nota = avaliacoesConceitoNota.Any(a => (a.isConceito.Equals(true)))
                  && (avaliacoesConceitoNota.Where(a => a.isConceito.Equals(false)).Any());
            return avaliacoesConceitoNota;
        }

        private void incluirAvaliacoesAluno(int cd_turma, int idEscola)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Busca os alunos que não tem avaliação de aluno na turma, mas teve matricula nesta turma: (isso ocorre na primeira vez que a turma é avaliada e quando inclui um novo aluno na turma)
                List<Aluno> alunosTurma = alunoBiz.returnAlunosSemAvalicacaoByTurma(idEscola, cd_turma).ToList();
                List<AvaliacaoAluno> avaliacoesAluno = new List<AvaliacaoAluno>();
                List<AvaliacaoTurma> avaliacoesTuma = new List<AvaliacaoTurma>();

                //Busca as avaliações da turma de uma turma que não existem para o determinado aluno
                if (alunosTurma.Count() > 0)
                    foreach (var item in alunosTurma)
                    {
                        avaliacoesTuma = daoAvaliacaoTurma.returnAvaliacoesTurmaSemAvaliacoeAluno(cd_turma, idEscola, item.cd_pessoa_aluno).ToList();

                        for (int j = 0; j < avaliacoesTuma.Count(); j++)
                        {
                            AvaliacaoAluno avaliacaoAluno = new AvaliacaoAluno();
                            avaliacaoAluno.cd_aluno = item.cd_aluno;
                            avaliacaoAluno.cd_avaliacao_turma = avaliacoesTuma[j].cd_avaliacao_turma;
                            avaliacaoAluno.cd_conceito = null;
                            avaliacoesAluno.Add(avaliacaoAluno);
                        }
                    }

                if (avaliacoesAluno != null && avaliacoesAluno.Count() > 0)
                    daoAvaliacaoAluno.incluirAvaliacoesAluno(avaliacoesAluno);

                transaction.Complete();
            }
        }
     
        //Método utilizado para retornar um booleano caso exista avaliação na data. Esta data é passada na desistência, a regra não vai
        // permitir que desista um aluno com data maior ou igual a data de Avaliação
        public bool existsAvaliacaoTurmaByDesistencia(DateTime data, int cd_pessoa_escola, int cd_aluno, int cd_turma)
        {
           return daoAvaliacaoTurma.existsAvaliacaoTurmaByDesistencia(data, cd_pessoa_escola, cd_aluno, cd_turma);
        }

        public bool deleteAvaliacaoTurma(List<AvaliacaoTurmaUI> avaliacaoTurma, int cd_pessoa_escola, int cd_usuario)
        {
            bool deleted = false;
            if (avaliacaoTurma.Count() > 0){
                if (daoAvaliacaoAluno.existeNotaOuConceitoAvalicoesAluno(avaliacaoTurma[0].cd_turma, cd_pessoa_escola))
                    throw new TurmaBusinessException(Utils.Messages.Messages.msgErroExclusaoAvaliacao, null, TurmaBusinessException.TipoErro.ERRO_DELETAR_NOTA_FUNCIONARIO, false);
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {

                    foreach (var aval in avaliacaoTurma)
                    {

                        FuncionarioSGF professor = professorBusiness.getProfLogByCodPessoaUsuario(cd_usuario, cd_pessoa_escola, TransactionScopeBuilder.TransactionType.COMMITED);

                        if (!((professor is Professor) || (professor is FuncionarioSGF)))
                            throw new TurmaBusinessException(Utils.Messages.Messages.msgFuncionarioDeletarNota, null, TurmaBusinessException.TipoErro.ERRO_DELETAR_NOTA_FUNCIONARIO, false);

                        List<int> list_cd_avaliacoes_turma = new List<int>();
                        IEnumerable<AvaliacaoAluno> avaliacaoAlunos = daoAvaliacaoAluno.getAvaliacaoesAlunoByIdTurmaEscola(aval.cd_turma, cd_pessoa_escola, aval.is_conceito_nota);
                        foreach (var item in avaliacaoAlunos)
                        {
                            AvaliacaoAluno aluno = daoAvaliacaoAluno.findById(item.cd_avaliacao_aluno, false);
                            if (aluno != null)
                                daoAvaliacaoAluno.delete(aluno, false);
                            list_cd_avaliacoes_turma.Add(aluno.cd_avaliacao_turma);
                        }

                        foreach (var item in list_cd_avaliacoes_turma.Distinct())
                        {
                            AvaliacaoTurma avaliacao = daoAvaliacaoTurma.findById(item, false);
                            if (avaliacao != null)
                                daoAvaliacaoTurma.delete(avaliacao, false);
                            deleted = true;
                        }
                    }
                    transaction.Complete();
                }
            }
            return deleted;
        }

        public List<RptBolsistasAval> getAvaliacoesBolsista(int cd_aluno, int cd_pessoa_escola, int cd_turma)
        {
            List<RptBolsistasAval> avalBol = new List<RptBolsistasAval>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                avalBol = daoTurma.getAvaliacoesBolsista(cd_aluno, cd_pessoa_escola, cd_turma);
                foreach (RptBolsistasAval aval in avalBol)
                {
                    MediaAvaliacaoAlunoUI mediaAval = getMediasAvaliacaoAluno(cd_aluno, aval.cd_turma, cd_pessoa_escola);
                    if (mediaAval != null && mediaAval.avaliacoesMedia != null && mediaAval.avaliacoesMedia.Count() > 0)
                    {
                        List<TipoAvaliacao> tipo = mediaAval.avaliacoesMedia.OrderBy(n => n.dc_tipo_avaliacao).ToList();
                        if (tipo.Count() > 0 && tipo[0] != null && tipo[0].cd_tipo_avaliacao > 0)
                        {
                            aval.dc_tipo_avaliacao1 = tipo[0].dc_tipo_avaliacao.Substring(0, 2);
                            aval.nm_nota_aluno1 = tipo[0].vl_media;
                        }
                        if (tipo.Count() > 1 && tipo[1] != null && tipo[1].cd_tipo_avaliacao > 0)
                        {
                            aval.dc_tipo_avaliacao2 = tipo[1].dc_tipo_avaliacao.Substring(0, 2);
                            aval.nm_nota_aluno2 = tipo[1].vl_media;
                        }
                        if (tipo.Count() > 2 && tipo[2] != null && tipo[2].cd_tipo_avaliacao > 0)
                        {
                            aval.dc_tipo_avaliacao3 = tipo[2].dc_tipo_avaliacao.Substring(0, 2);
                            aval.nm_nota_aluno3 = tipo[2].vl_media;
                        }
                    }

                }
                transaction.Complete();
            }
            return avalBol;
        }
        
        #endregion

        #region Avaliacao Aluno
        public void editListAvaliacao(List<AvaliacaoAluno> avaliacoesAlunos)
        {
            AvaliacaoAluno avaliacoesAlunosEdit = new AvaliacaoAluno();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, daoProgTurma.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                foreach (var item in avaliacoesAlunos)
                {
                    if (item.dt_avalicao_aluno != null)
                    {
                        if (item.dt_avalicao_aluno == null && (item.nm_nota_aluno > 0 || item.cd_conceito > 0))
                            throw new TurmaBusinessException(Utils.Messages.Messages.msgDataAvaliacaoTurmaObrigatoria, null, TurmaBusinessException.TipoErro.ERRO_DATA_NULL, false);

                        avaliacoesAlunosEdit = daoAvaliacaoAluno.findById(item.cd_avaliacao_aluno, false);
                        avaliacoesAlunosEdit.nm_nota_aluno = item.nm_nota_aluno;
                        avaliacoesAlunosEdit.nm_nota_aluno_2 = item.nm_nota_aluno_2;
                        avaliacoesAlunosEdit.tx_obs_nota_aluno = item.tx_obs_nota_aluno;
                        avaliacoesAlunosEdit.cd_conceito = item.cd_conceito == 0 ? null : item.cd_conceito;
                        avaliacoesAlunosEdit.id_segunda_prova = item.nm_nota_aluno_2 != null ? true : false;
                        avaliacoesAlunosEdit.dt_avalicao_aluno = item.dt_avalicao_aluno;
                        if (item.AvaliacoesAlunoParticipacao.Count()>0)
                            crudAvaliacoesParticipacoesAluno(item);
                        daoAvaliacaoAluno.saveChanges(false);
                    }
                }

                transaction.Complete();
            }
        }

        private void crudAvaliacoesParticipacoesAluno(AvaliacaoAluno avaliacaoAlunoView)
        {
            List<AvaliacaoAlunoParticipacao> avaliacaoAlunoPartContext = daoAvaliacaoAlunoParticipacao.getAvaliacaoAlunoParticipacaoByAvaliacaoAluno(avaliacaoAlunoView.cd_avaliacao_aluno).ToList();
            IEnumerable<AvaliacaoAlunoParticipacao> avaliacaoAlunoPartEdit = from ap in avaliacaoAlunoView.AvaliacoesAlunoParticipacao
                                                                                  where ap.cd_avaliacao_aluno_participacao != 0
                                                                                  select ap;
            IEnumerable<AvaliacaoAlunoParticipacao> avaliacaoAlunoPartInsert = from ap in avaliacaoAlunoView.AvaliacoesAlunoParticipacao
                                                            where ap.cd_avaliacao_aluno_participacao <= 0
                                                            select ap;
            IEnumerable<AvaliacaoAlunoParticipacao> avaliacaoAlunoPartDeleted = avaliacaoAlunoPartContext.Where(pc => !avaliacaoAlunoPartEdit.Any(pv => pc.cd_avaliacao_aluno_participacao == pv.cd_avaliacao_aluno_participacao));

            if (avaliacaoAlunoPartDeleted != null && avaliacaoAlunoPartDeleted.Count() > 0)
                foreach (var item in avaliacaoAlunoPartDeleted)
                    daoAvaliacaoAlunoParticipacao.deleteContext(item, false);
            if (avaliacaoAlunoPartInsert != null)
                foreach (var item in avaliacaoAlunoPartInsert)
                    if (item.cd_avaliacao_aluno_participacao == 0)
                    {
                        item.cd_avaliacao_aluno = avaliacaoAlunoView.cd_avaliacao_aluno;
                        daoAvaliacaoAlunoParticipacao.addContext(item, false);
                    }
            if (avaliacaoAlunoPartEdit != null)
                foreach (var item in avaliacaoAlunoPartEdit)
                {
                    var avaliacaoAlunoPartEditContext = (from ap in avaliacaoAlunoPartContext
                                                         where ap.cd_avaliacao_aluno_participacao == item.cd_avaliacao_aluno_participacao
                                                            && ap.cd_conceito_participacao != item.cd_conceito_participacao
                                                         select ap).FirstOrDefault();
                    if (avaliacaoAlunoPartEditContext != null)
                        avaliacaoAlunoPartEditContext.cd_conceito_participacao = item.cd_conceito_participacao;
                }
            daoAvaliacaoAlunoParticipacao.saveChanges(false);
        }

        public bool verificaAvalicaoAlunoTurma(int cdTurma, int cdAluno)
        {
            return daoAvaliacaoAluno.verificaAvalicaoAlunoTurma(cdTurma, cdAluno);
        }


        #endregion

        #region Histórico Aluno
        public MediaAvaliacaoAlunoUI getMediasAvaliacaoAluno(int cd_aluno, int cd_turma, int cd_escola) 
        {
            List<AvaliacaoAluno> listaAvaliacaoNota = daoAvaliacaoAluno.getNotasAvaliacaoTurma(cd_aluno, cd_turma, cd_escola).ToList();
            List<TipoAvaliacao> qtd_avaliacoes_curso = daoTurma.getTiposAvaliacaoComQtdAvaliacao(cd_turma, listaAvaliacaoNota.GroupBy(x=> x.AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao).Select(x=> x.Key).ToList());

            var mediaTurmas = processarMediaAvaliacaoTurma(listaAvaliacaoNota, qtd_avaliacoes_curso);
            return mediaTurmas;
        }

        public MediaAvaliacaoAlunoUI getMediasEstagioAvaliacaoAluno(int cd_aluno, int cd_escola, int cd_estagio)
        {
            List<AvaliacaoAluno> listaAvaliacaoNota = daoAvaliacaoAluno.getNotasAvaliacaoTurmaPorEstagio(cd_aluno, cd_estagio, cd_escola).ToList();
            //List<AvaliacaoAluno> listaAvaliacaoComMaiorData = new List<AvaliacaoAluno>();


            //listarAvaliacoesComMaiorData(listaAvaliacaoComMaiorData, listaAvaliacaoNota);
            List<TipoAvaliacao> qtd_avaliacoes_curso = daoTurma.getTiposAvaliacaoComQtdAvaliacao(0, listaAvaliacaoNota.GroupBy(x => x.AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao).Select(x => x.Key).ToList());

            var mediaTurmas = processarMediaAvaliacaoPorEstagio(listaAvaliacaoNota, qtd_avaliacoes_curso, cd_aluno, cd_escola);

            return mediaTurmas;
        }

        private void listarAvaliacoesComMaiorData(List<AvaliacaoAluno> listaAvaliacaoComMaiorData, List<AvaliacaoAluno> listaAvaliacaoNota)
        {
            foreach (var avaliacao in listaAvaliacaoNota)
            {
                if (!listaAvaliacaoComMaiorData.Any(x => x.cd_criterio_avaliacao.Equals(avaliacao.cd_criterio_avaliacao)))
                    listaAvaliacaoComMaiorData.Add(avaliacao);
                else
                {
                    var avListaComMaiorNota = listaAvaliacaoComMaiorData.Where(x => x.cd_criterio_avaliacao.Equals(avaliacao.cd_criterio_avaliacao)).FirstOrDefault();
                    if (avListaComMaiorNota != null && avListaComMaiorNota.dt_avaliacao_turma < avaliacao.dt_avaliacao_turma)
                    {
                        listaAvaliacaoComMaiorData.Remove(avListaComMaiorNota);
                        listaAvaliacaoComMaiorData.Add(avaliacao);
                    }
                }
            }
        }

        private MediaAvaliacaoAlunoUI processarMediaAvaliacaoPorEstagio(List<AvaliacaoAluno> listaAvaliacaoNota, List<TipoAvaliacao> qtd_avaliacoes_curso, int cd_aluno, int cd_escola)
        {
            List<TipoAvaliacao> avaliacoesMedia = new List<TipoAvaliacao>();
            MediaAvaliacaoAlunoUI retorno = new MediaAvaliacaoAlunoUI();
            //Ininicializa as variaveis:
            retorno.vl_maximo = 0;
            retorno.vl_total = 0;
            var nm_total_avaliacoes = 0;
            var qtd_tipo_avaliacao = listaAvaliacaoNota.GroupBy(a => a.AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao).Count();
            var cd_turmas = listaAvaliacaoNota.GroupBy(a => a.cd_turma).Select(t => t.First().cd_turma).ToList();

            for (int i = 0; i < listaAvaliacaoNota.Count; i++)
            {
                TipoAvaliacao tipoAvaliacao = avaliacoesMedia.Where(a => a.cd_tipo_avaliacao == listaAvaliacaoNota[i].AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao).FirstOrDefault();

                if (tipoAvaliacao != null)
                {
                    tipoAvaliacao.vl_soma += listaAvaliacaoNota[i].vl_nota_media.Value;
                }
                else
                {
                    tipoAvaliacao = new TipoAvaliacao();
                    tipoAvaliacao.nm_avaliacoes = qtd_avaliacoes_curso.Where(x => x.cd_tipo_avaliacao == listaAvaliacaoNota[i].AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao).FirstOrDefault().nm_avaliacoes;
                    tipoAvaliacao.vl_soma = listaAvaliacaoNota[i].vl_nota_media.Value;
                    //tipoAvaliacao.nm_avaliacoes = 1;
                    tipoAvaliacao.dc_tipo_avaliacao = listaAvaliacaoNota[i].dc_tipo_avaliacao;
                    tipoAvaliacao.cd_tipo_avaliacao = listaAvaliacaoNota[i].AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao;
                    tipoAvaliacao.vl_total_nota = listaAvaliacaoNota[i].AvaliacaoTurma.Avaliacao.TipoAvaliacao.vl_total_nota;
                    nm_total_avaliacoes += tipoAvaliacao.nm_avaliacoes;
                    if (qtd_tipo_avaliacao == 1)
                        nm_total_avaliacoes = listaAvaliacaoNota[i].nm_total_avaliacao_curso;

                    avaliacoesMedia.Add(tipoAvaliacao);

                    if (tipoAvaliacao.vl_total_nota.HasValue)
                        retorno.vl_maximo += (double)tipoAvaliacao.vl_total_nota;
                }
                retorno.vl_total += listaAvaliacaoNota[i].vl_nota_media.Value;
            }

            retorno.avaliacoesNota = listaAvaliacaoNota;
            retorno.avaliacoesMedia = avaliacoesMedia;
            retorno.avaliacoesTurma = processarMediaAvaliacaoTurma(cd_aluno, cd_turmas, cd_escola);

            if (avaliacoesMedia != null && avaliacoesMedia.Count > 0)
            {
                if (qtd_tipo_avaliacao == 1)
                {
                    retorno.vl_media_final = Math.Round(avaliacoesMedia.First().vl_soma / nm_total_avaliacoes, 1, MidpointRounding.AwayFromZero);
                    retorno.vl_media_parcial = Math.Round(avaliacoesMedia.First().vl_soma / listaAvaliacaoNota.Count, 1, MidpointRounding.AwayFromZero);
                    retorno.vl_aproveitamento_total = Math.Round((retorno.vl_media_final / retorno.vl_media_parcial) * 100, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    //Valores do campo Aproveitamento total do aluno:
                    retorno.vl_media_final = Math.Round(avaliacoesMedia.Sum(x => x.vl_soma) / nm_total_avaliacoes, 1, MidpointRounding.AwayFromZero);
                    //A Média Parcial será calculada pelo número de avaliações realizadas.
                    retorno.vl_media_parcial = Math.Round(avaliacoesMedia.Sum(x => x.vl_soma) / listaAvaliacaoNota.Count, 1, MidpointRounding.AwayFromZero);
                    retorno.vl_aproveitamento_total = Math.Round((retorno.vl_media_final / retorno.vl_media_parcial) * 100, 2, MidpointRounding.AwayFromZero);

                    ////Valores do campo Aproveitamento total do aluno:
                    //retorno.vl_media_final = Math.Round(avaliacoesMedia.Sum(x => x.vl_soma), 2) / nm_total_avaliacoes;
                    ////A Média Parcial será calculada pelo número de avaliações realizadas.
                    //retorno.vl_media_parcial = Math.Round(avaliacoesMedia.Sum(x => x.vl_soma), 2) / listaAvaliacaoNota.Count;
                    //retorno.vl_aproveitamento_total = Math.Round((retorno.vl_media_final / retorno.vl_media_parcial) * 100, 2);
                }
            }
            return retorno;
        }

        private IEnumerable<TipoAvaliacao> processarMediaAvaliacaoTurma(int cd_aluno, List<int> cd_turmas, int cd_escola)
        {
            List<TipoAvaliacao> avaliacoesMedia = new List<TipoAvaliacao>();
            
            foreach (var cd_turma in cd_turmas)
            {
                List<AvaliacaoAluno> listaAvaliacaoNota = daoAvaliacaoAluno.getNotasAvaliacaoTurma(cd_aluno, cd_turma, cd_escola).ToList();
                List<TipoAvaliacao> qtd_avaliacoes_curso = daoTurma.getTiposAvaliacaoComQtdAvaliacao(cd_turma, listaAvaliacaoNota.GroupBy(x => x.AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao).Select(x => x.Key).ToList());

                var t = new TipoAvaliacao();
                avaliacoesMedia.AddRange(processarMediaAvaliacaoTurma(listaAvaliacaoNota, qtd_avaliacoes_curso).avaliacoesMedia);
            }
            return avaliacoesMedia;            
        }

        private MediaAvaliacaoAlunoUI processarMediaAvaliacaoTurma(List<AvaliacaoAluno> listaAvaliacaoNota, List<TipoAvaliacao> qtd_avaliacoes_curso)
        {
            List<TipoAvaliacao> avaliacoesMedia = new List<TipoAvaliacao>();
            MediaAvaliacaoAlunoUI retorno = new MediaAvaliacaoAlunoUI();
            //Ininicializa as variaveis:
            retorno.vl_maximo = 0;
            retorno.vl_total = 0;
            var nm_total_avaliacoes = 0;

            for (int i = 0; i < listaAvaliacaoNota.Count; i++)
            {
                TipoAvaliacao tipoAvaliacao = avaliacoesMedia.Where(a => a.cd_tipo_avaliacao == listaAvaliacaoNota[i].AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao).FirstOrDefault();

                if (tipoAvaliacao != null)
                {
                    tipoAvaliacao.vl_soma += listaAvaliacaoNota[i].vl_nota_media.Value;
                    //tipoAvaliacao.nm_avaliacoes += 1;
                    tipoAvaliacao.vl_soma = Math.Round(tipoAvaliacao.vl_soma, 1, MidpointRounding.AwayFromZero);
                }
                else
                {
                    tipoAvaliacao = new TipoAvaliacao();
                    tipoAvaliacao.nm_avaliacoes = qtd_avaliacoes_curso.Where(x => x.cd_tipo_avaliacao == listaAvaliacaoNota[i].AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao).FirstOrDefault().nm_avaliacoes;
                    tipoAvaliacao.vl_soma = listaAvaliacaoNota[i].vl_nota_media.Value;
                    //tipoAvaliacao.nm_avaliacoes = 1;
                    nm_total_avaliacoes += tipoAvaliacao.nm_avaliacoes;
                    tipoAvaliacao.dc_tipo_avaliacao = listaAvaliacaoNota[i].dc_tipo_avaliacao;
                    tipoAvaliacao.cd_tipo_avaliacao = listaAvaliacaoNota[i].AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao;
                    tipoAvaliacao.vl_total_nota = listaAvaliacaoNota[i].AvaliacaoTurma.Avaliacao.TipoAvaliacao.vl_total_nota;
                    tipoAvaliacao.cd_turma = listaAvaliacaoNota[i].cd_turma;
                    tipoAvaliacao.no_turma = listaAvaliacaoNota[i].no_turma;

                    avaliacoesMedia.Add(tipoAvaliacao);

                    if (tipoAvaliacao.vl_total_nota.HasValue)
                        retorno.vl_maximo += (double)tipoAvaliacao.vl_total_nota;
                }
                retorno.vl_total += listaAvaliacaoNota[i].vl_nota_media.Value;
                retorno.vl_total = Math.Round(retorno.vl_total, 1, MidpointRounding.AwayFromZero);
            }

            retorno.avaliacoesNota = listaAvaliacaoNota;
            retorno.avaliacoesMedia = avaliacoesMedia;
            if (avaliacoesMedia != null && avaliacoesMedia.Count > 0)
            {
                //Valores do campo Aproveitamento total do aluno:
                retorno.vl_media_final = Math.Round(avaliacoesMedia.Sum(x => x.vl_soma) / nm_total_avaliacoes, 1, MidpointRounding.AwayFromZero);  //Estava 2
                //A Média Parcial será calculada pelo número de avaliações realizadas.
                retorno.vl_media_parcial = Math.Round(avaliacoesMedia.Sum(x => x.vl_soma) / listaAvaliacaoNota.Count, 1, MidpointRounding.AwayFromZero);
                retorno.vl_aproveitamento_total = Math.Round(retorno.vl_media_final / retorno.vl_media_parcial * 100, 1, MidpointRounding.AwayFromZero);  //Estava 2

                if (avaliacoesMedia.Count == 1 && (avaliacoesMedia.Sum(x => x.nm_avaliacoes) / 2) == listaAvaliacaoNota.Count)
                    retorno.vl_aproveitamento_total = 50;
                //retorno.vl_media_final = Math.Round(avaliacoesMedia.Sum(x => x.vl_media), 2) / avaliacoesMedia.Count;
                //retorno.vl_aproveitamento_total = Math.Round(retorno.vl_media_final * 10, 2);
            }
            return retorno;
        }

        public IEnumerable<AvaliacaoAluno> getConceitosAvaliacaoAluno(int cd_aluno, int cd_turma, int cd_escola) {
            return daoAvaliacaoAluno.getConceitosAvaliacaoAluno(cd_aluno, cd_turma, cd_escola);
        }

        public IEnumerable<Estagio> getEstagiosHistoricoAluno(int cd_aluno, int cd_escola) 
        {
            var estagios = daoTurma.getEstagiosHistoricoAluno(cd_aluno, cd_escola);
            if (estagios.Count() > 0)
            {
                //DateTime primeira_aula = estagios.Where()
                //    DateTime ultima_aula =
                estagios = estagios.GroupBy(e => new {e.no_estagio, e.no_produto, e.cd_produto})
                    .Select(est => new Estagio
                    {
                        cd_estagio = est.Min(t => t.cd_estagio), //Key.cd_produto,
                        nm_aulas_dadas = est.Sum(t => t.nm_aulas_dadas),
                        nm_faltas = (byte?) est.Sum(t => t.nm_faltas),
                        nm_aulas_contratadas = est.Sum(t => t.nm_aulas_contratadas),
                        no_estagio = est.Key.no_estagio,
                        cd_produto = est.Key.cd_produto,
                        no_produto = est.Key.no_produto,
                        primeira_aula = est.Min(t => t.primeira_aula),
                        ultima_aula = est.Max(t => t.ultima_aula),
                        nm_ordem_estagio = est.Last().nm_ordem_estagio
                    }).OrderBy(z => z.cd_produto).ThenBy(b => b.no_estagio);
               
            }
            return estagios;
        }

        public IEnumerable<Turma> getTurmasAvaliacoes(int cd_aluno, int cd_escola){

            return daoTurma.getTurmasAvaliacoes(cd_aluno, cd_escola);
        }

        #endregion

        #region Evento

        public IEnumerable<ReportDiarioAula> getRelatorioDiarioAula(int cd_escola, int cd_turma, int cd_professor, DateTime dt_inicial, DateTime dt_final)
        {
            IEnumerable<ReportDiarioAula> retorno = new List<ReportDiarioAula>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = professorBusiness.getRelatorioDiarioAula(cd_escola, cd_turma, cd_professor, dt_inicial, dt_final);
                transaction.Complete();
            }
            return retorno;
        }

        public ProgramacoesTurmaSemDiarioAula getProgramacoesTurmasSemDiarioAula(int cd_turma, int cd_escola, int qtd_aulas_sem_material, byte nm_dias_titulos_abertos)
        {
            ProgramacoesTurmaSemDiarioAula progTDiario = new ProgramacoesTurmaSemDiarioAula();
            progTDiario.progsTurma = daoProgTurma.getProgramacaoTurmaByTurma(cd_turma, cd_escola, ProgramacaoTurmaDataAccess.TipoConsultaProgTurmaEnum.HAS_PROG_TURMA_SEM_DIARIOAULA,false).ToList();
            progTDiario.avaliacoesTurma = daoAvaliacao.getAvaliacaoECriterioTurma(cd_turma, cd_escola).ToList();
            if (progTDiario.progsTurma != null && progTDiario.progsTurma.Count() > 0)
            {
                ProgramacaoTurma progTurma = progTDiario.progsTurma.FirstOrDefault();
                byte diaSemana = (byte)(progTurma.dta_programacao_turma.DayOfWeek + 1);
                progTDiario.professoresHorariosTurma = professorBusiness.getProfHorariosProgTurma(progTurma.cd_turma, cd_escola, diaSemana, (TimeSpan)progTurma.hr_inicial_programacao,
                                                                            (TimeSpan)progTurma.hr_final_programacao).ToList();

                //ProgramacoesTurmaSemDiarioAula pTAlunosPendecia = verificarAlunosPendenciaMaterialDidaticoCurso(qtd_aulas_sem_material, cd_turma, cd_escola, progTurma.dta_programacao_turma);
                //if (pTAlunosPendecia != null && pTAlunosPendecia.alunos != null && pTAlunosPendecia.alunos.Count() > 0)
                //{
                //    progTDiario.alunos = pTAlunosPendecia.alunos;
                //    progTDiario.nomeAlunosPendencia = pTAlunosPendecia.nomeAlunosPendencia;
                //}
                verificarAlunosSemTitulosAPagar(nm_dias_titulos_abertos, cd_turma, cd_escola, progTurma.dta_programacao_turma, ref progTDiario);
            }
            return progTDiario;
        }

        #endregion

        #region Professor business

        public IEnumerable<FuncionarioSGF> getFuncionariosByAulaPers(int cdEscola)
        {
            return professorBusiness.getFuncionariosByAulaPers(cdEscola);
        }
        #endregion

        #region Avaliação

        public void persistAvaliacoes(int cd_tipo_avaliacao, List<Avaliacao> avaliacoes, int cdEscola)
        {
            bool existAvaliacaoLancadaTurma = false;
            IEnumerable<Avaliacao> avaliacoesBaseDados = daoAvaliacao.findByIdTipoAvaliacao(cd_tipo_avaliacao).ToList();
            Avaliacao avaliacao = new Avaliacao();
            
            //pega as avaliacões com código, ou seja, que existem na base de dados
            IEnumerable<Avaliacao> avaliacoesComID = avaliacoes.Where(a => a.cd_avaliacao != 0);

            //pega as avaliações que vão ser deletadas
            IEnumerable<Avaliacao> avaliacoesADeletar = avaliacoesBaseDados.Where(av => !avaliacoesComID.Any(avl => avl.cd_avaliacao == av.cd_avaliacao));

            if (cdEscola > 0)
                existAvaliacaoLancadaTurma = daoAvaliacao.existNotaLancadaAvaliacaoTurma(avaliacoes.Select(x=> x.cd_avaliacao).ToList());
                //for (int i = 0; i < avaliacoes.Count(); i++)
                //{
                //    var existAvaliacaoLancadaTurma = daoAvaliacao.existNotaLancadaAvaliacaoTurma(avaliacoes[i].cd_avaliacao, cdEscola);
                //    if (existAvaliacaoLancadaTurma)
                //        throw new TurmaBusinessException(Utils.Messages.Messages.msgExistNotaLancadaParaTurma, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_NOTA_LANCADA, false);
                //}



            if (avaliacoesADeletar.Count() > 0)
            {
                if (existAvaliacaoLancadaTurma)
                    throw new TurmaBusinessException(Utils.Messages.Messages.msgExistNotaLancadaParaTurma, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_NOTA_LANCADA, false);
                foreach (var item in avaliacoesADeletar)
                {
                    var deletar = (from av in avaliacoesBaseDados where av.cd_avaliacao == item.cd_avaliacao select av).FirstOrDefault();
                    var avalTurmaDeletar = daoAvaliacaoTurma.GetAvaliacaoTurmaByIdAvaliacao(item.cd_avaliacao, cdEscola);

                    this.DeletarRelacionamentoAvalicaoTurma(avalTurmaDeletar, cdEscola);

                    if (deletar != null)
                    {
                        daoAvaliacao.delete(deletar, false);
                    }
                }
            }

            //Edita as avaliações que estão com  código
            avaliacoesBaseDados = daoAvaliacao.findByIdTipoAvaliacao(cd_tipo_avaliacao).ToList();
            if (avaliacoesComID != null && avaliacoesComID.Count() > 0)
            {
                foreach (var avalBD in avaliacoesBaseDados)
                {
                    foreach (var avalVW in avaliacoesComID)
                    {
                        if (avalVW.cd_avaliacao == avalBD.cd_avaliacao)
                        {

                            changeValueAvaliacao(avalBD, avalVW);
                            bool alterouOutrosCampos = daoAvaliacao.DB().Entry(avalBD).Property(p => p.id_avaliacao_ativa).IsModified ||
                                                        daoAvaliacao.DB().Entry(avalBD).Property(p => p.cd_criterio_avaliacao).IsModified ||
                                                        daoAvaliacao.DB().Entry(avalBD).Property(p => p.nm_peso_avaliacao).IsModified ||
                                                        daoAvaliacao.DB().Entry(avalBD).Property(p => p.vl_nota).IsModified ? true : false;
                            if (existAvaliacaoLancadaTurma && alterouOutrosCampos)
                                throw new TurmaBusinessException(Utils.Messages.Messages.msgExistNotaLancadaParaTurma, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_NOTA_LANCADA, false);
                            daoAvaliacao.saveChanges(false);
                        }
                    }
                }
            }

            //Insere avaliações sem id
            IEnumerable<Avaliacao> avaliacoesSemID = from aval in avaliacoes
                                                        where aval.cd_avaliacao == 0
                                                        select aval;
            if (avaliacoesSemID != null && avaliacoesSemID.Count() > 0)
            {
                if (existAvaliacaoLancadaTurma)
                    throw new TurmaBusinessException(Utils.Messages.Messages.msgExistNotaLancadaParaTurma, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_NOTA_LANCADA, false);
                foreach (var aval in avaliacoesSemID)
                {
                    avaliacao = new Avaliacao
                    {
                        cd_criterio_avaliacao = aval.cd_criterio_avaliacao,
                        cd_tipo_avaliacao = cd_tipo_avaliacao,
                        id_avaliacao_ativa = aval.id_avaliacao_ativa,
                        nm_ordem_avaliacao = aval.id_avaliacao_ativa == false ? null : aval.nm_ordem_avaliacao,
                        vl_nota = aval.vl_nota,
                        nm_peso_avaliacao = aval.nm_peso_avaliacao
                    };
                    daoAvaliacao.add(avaliacao, false);
                }
            }
        }

        public void DeletarRelacionamentoAvalicaoTurma(IEnumerable<AvaliacaoTurma> avalTurmaDeletar, int cdEscola)
        {
            foreach (var avalTurma in avalTurmaDeletar)
            {
                List<int> list_cd_avaliacoes_turma = new List<int>();

                IEnumerable<AvaliacaoAluno> avaliacaoAlunos = daoAvaliacaoAluno.getAvaliacaoAlunoByIdAvlTurma(avalTurma.cd_avaliacao_turma, cdEscola, avalTurma.cd_turma);

                foreach (var item in avaliacaoAlunos)
                {
                    AvaliacaoAluno aluno = daoAvaliacaoAluno.findById(item.cd_avaliacao_aluno, false);
                    if (aluno != null)
                        daoAvaliacaoAluno.delete(aluno, false);
                    list_cd_avaliacoes_turma.Add(aluno.cd_avaliacao_turma);
                }

                foreach (var item in list_cd_avaliacoes_turma.Distinct())
                {
                    AvaliacaoTurma avaliacao = daoAvaliacaoTurma.findById(item, false);
                    if (avaliacao != null)
                        daoAvaliacaoTurma.delete(avaliacao, false);
                }
            }
        }

        public IEnumerable<AvaliacaoUI> searchAvaliacao(SearchParameters parametros, string descAbreviado, int? idTipoAvaliacao, int? idCriterio, bool inicio, bool? status)
        {
            IEnumerable<AvaliacaoUI> retorno = new List<AvaliacaoUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                //if (parametros.sort == null) 
                parametros.sort = "cd_tipo_avaliacao";
                parametros.sort = parametros.sort.Replace("descAbreviado", "dc_criterio_abreviado");
                parametros.sort = parametros.sort.Replace("peso", "nm_peso_avaliacao");
                parametros.sort = parametros.sort.Replace("ordem", "nm_ordem_avaliacao");
                retorno = daoAvaliacao.searchAvaliacao(parametros, descAbreviado, idTipoAvaliacao, idCriterio, inicio, status);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Avaliacao> getAvaliacaoOrdem(int idTipoAvaliacao, int idCriterio, bool? status)
        {
            return daoAvaliacao.getAvaliacaoOrdem(idTipoAvaliacao, idCriterio, status);
        }

        //função para persistir a Avaliação e a ordem
        public IEnumerable<AvaliacaoUI> editAvaliacao(AvaliacaoOrdem entity, int cdEscola)
        {
            List<Avaliacao> listaNovasAvaliacao = new List<Avaliacao>();
            IEnumerable<Avaliacao> list;
            IEnumerable<AvaliacaoUI> listUI = new List<AvaliacaoUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                var existAvaliacaoLancadaTurma = daoAvaliacao.existNotaLancadaAvaliacaoTurma(entity.avaliacao.cd_avaliacao, cdEscola);
                if (existAvaliacaoLancadaTurma)
                    throw new TurmaBusinessException(Utils.Messages.Messages.msgExistNotaLancadaParaTurma, null, TurmaBusinessException.TipoErro.ERRO_EXISTE_NOTA_LANCADA, false);

                //Busca a lista de avaliações anterior a alteração:
                list = daoAvaliacao.getAvaliacaoOrdem(entity.avaliacao.cd_tipo_avaliacao, entity.avaliacao.cd_criterio_avaliacao, true);
                List<Avaliacao> listaDel = new List<Avaliacao>();
                List<Avaliacao> listaAntigasAvaliacao = list.ToList();

                listaNovasAvaliacao = entity.avaliacoes.ToList();

                //Deletando avaliações que não existem mais
                foreach (var avaliacaoAntiga in listaAntigasAvaliacao)
                    if (listaNovasAvaliacao.Where(p => p.cd_avaliacao == avaliacaoAntiga.cd_avaliacao).Count() <= 0)
                        listaDel.Add(avaliacaoAntiga);
                if (listaDel != null && listaDel.Count > 0)
                    daoAvaliacao.deleteAllAvaliacao(listaDel);

                //Incluindo novas avaliações
                foreach (var avaliacaoNova in listaNovasAvaliacao)
                {
                    if (avaliacaoNova.cd_avaliacao == 0)
                        daoAvaliacao.add(avaliacaoNova, false);
                    else
                    {
                        //Alterando avaliações já existentes
                        Avaliacao avaliacao = daoAvaliacao.findById(avaliacaoNova.cd_avaliacao, false);
                        avaliacao.copy(avaliacaoNova);
                        daoAvaliacao.edit(avaliacao, false);
                    }
                }
                listUI = daoAvaliacao.getAvaliacaoByIdTipoAvaliacao(entity.avaliacao.cd_tipo_avaliacao).ToList();
                transaction.Complete();
            }
            return listUI;
        }

        // Adicionando uma avaliação na base
        public AvaliacaoUI addAvaliacao(Avaliacao entity)
        {
            Avaliacao avaliacao = new Avaliacao();
            AvaliacaoUI avaliacaoUI = new AvaliacaoUI();
            string tipoAvaliacao = "", criterio = "";

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                // Caso o avaliação seja inativada, a ordem dele é -1, ou seja, não existe ordem para ele:
                avaliacao = new Avaliacao
                {
                    cd_criterio_avaliacao = entity.cd_criterio_avaliacao,
                    cd_tipo_avaliacao = entity.cd_tipo_avaliacao,
                    id_avaliacao_ativa = entity.id_avaliacao_ativa,
                    nm_ordem_avaliacao = entity.id_avaliacao_ativa == false ? null : entity.nm_ordem_avaliacao,
                    vl_nota = entity.vl_nota,
                    nm_peso_avaliacao = entity.nm_peso_avaliacao
                };
                avaliacao = daoAvaliacao.add(entity, false);
                transaction.Complete();
            }
            avaliacaoUI = AvaliacaoUI.fromAvaliacao(avaliacao, tipoAvaliacao, criterio);

            return avaliacaoUI;
        }

        public Avaliacao changeValueAvaliacao(Avaliacao avaliacaoContext, Avaliacao avaliacaoView)
        {
            avaliacaoContext.cd_avaliacao = avaliacaoView.cd_avaliacao;
            avaliacaoContext.cd_criterio_avaliacao = avaliacaoView.cd_criterio_avaliacao;
            avaliacaoContext.id_avaliacao_ativa = avaliacaoView.id_avaliacao_ativa;
            avaliacaoContext.nm_ordem_avaliacao = avaliacaoView.nm_ordem_avaliacao;
            avaliacaoContext.nm_peso_avaliacao = avaliacaoView.nm_peso_avaliacao;
            avaliacaoContext.vl_nota = avaliacaoView.vl_nota;
            return avaliacaoContext;
        }

        // deleta varias avaliações e reordena
        public bool deleteAllAvaliacao(List<Avaliacao> avaliacoes)
        {
            //((FundacaoFisk.SGF.Services.Coordenacao.Model.AvaliacaoUI)((new System.Collections.Generic.Mscorlib_CollectionDebugView<FundacaoFisk.SGF.GenericModel.Avaliacao>(avaliacoes)).Items[0]))
            int idTipoAvaliacao = 0;
            int idCriterio = 0;
            bool deleted = false;

            if (avaliacoes.Count > 0)
            {
                idTipoAvaliacao = avaliacoes[0].cd_tipo_avaliacao;
                idCriterio = avaliacoes[0].cd_criterio_avaliacao;
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    deleted = daoAvaliacao.deleteAllAvaliacao(avaliacoes);

                    // Busca os estagios desse produto, reordena e atualiza as ordens:
                    avaliacoes = daoAvaliacao.getAvaliacaoOrdem(idTipoAvaliacao, idCriterio, avaliacoes[0].id_avaliacao_ativa).ToList<Avaliacao>();

                    avaliacoes.Sort(delegate(Avaliacao a1, Avaliacao a2) { return int.Parse(a1.nm_ordem_avaliacao.ToString()).CompareTo(int.Parse(a2.nm_ordem_avaliacao.ToString())); });
                    for (int i = 0; i < avaliacoes.Count; i++)
                        avaliacoes[i].nm_ordem_avaliacao = byte.Parse((i + 1).ToString());
                    this.editOrdemAvaliacao(avaliacoes.ToList<Avaliacao>());
                    transaction.Complete();
                }
            }
            return deleted;
        }

        public byte? getSomatorio(int idTipoAvaliacao, int idCriterio)
        {
            return daoAvaliacao.getSomatorio(idTipoAvaliacao, idCriterio);
        }

        //Edita a ordem conform o registro for persistido ou deletado
        private List<Avaliacao> editOrdemAvaliacao(List<Avaliacao> entity)
        {
            for (var i = 0; i < entity.Count; i++)
                daoAvaliacao.editOrdemAvaliacao(entity[i]);

            return entity;
        }

        public byte? getNotaAvaliacao(int idAvaliacao)
        {
            return daoAvaliacao.getNotaAvaliacao(idAvaliacao);
        }

        public bool existNotaLancadaAvaliacaoTurma(int cd_avaliacao, int cd_escola)
        {
            return daoAvaliacao.existNotaLancadaAvaliacaoTurma(cd_avaliacao, cd_escola);
        }

        public IEnumerable<AvaliacaoUI> getAvaliacaoByIdTipoAvaliacao(int idTipoAvaliacao)
        {
            return daoAvaliacao.getAvaliacaoByIdTipoAvaliacao(idTipoAvaliacao);
        }

        public int getAvaliacaoCursoExistsTurmaWithCurso(int cd_turma, int cd_escola)
        {
            return daoAvaliacao.getAvaliacaoCursoExistsTurmaWithCurso(cd_turma, cd_escola);
        }

        public IEnumerable<Avaliacao> getAvaliacaoECriterioTurma(int cd_turma, int cd_escola)
        {
            return daoAvaliacao.getAvaliacaoECriterioTurma(cd_turma, cd_escola);
        }

        #endregion

        #region Feriado

        public IEnumerable<Feriado> getDescFeriado(SearchParameters parametros, string desc, bool inicio, bool? status, int cdEscola, int Ano, int Mes, int Dia, int AnoFim, int MesFim, int DiaFim, bool? somenteAno, bool idFeriadoAtivo)
        {
            IEnumerable<Feriado> retorno = new List<Feriado>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, daoFeriado.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_feriado";
                parametros.sort = parametros.sort == "isFeriadoFinanceiro" ? parametros.sort.Replace("isFeriadoFinanceiro", "id_feriado_financeiro") : parametros.sort.Replace("TipoFeriado", "cd_pessoa_escola");
                retorno = daoFeriado.getFeriadoDesc(parametros, desc, inicio, status, cdEscola, Ano, Mes, Dia, AnoFim, MesFim, DiaFim, somenteAno, idFeriadoAtivo);
                transaction.Complete();
            }
            return retorno;
        }

        public Feriado findByIdFeriado(int id)
        {
            return daoFeriado.findById(id, false);
        }

        public List<Feriado> getFeriadosPorPeriodo(Feriado feriado, int? cd_escola)
        {
            return daoFeriado.getFeriadosPorPeriodo(feriado, cd_escola);
        }

        public Feriado addFeriado(Feriado entity, int cd_escola, string login, ref Boolean refez_programacoes, int cd_usuario)
        {
            DateTime data_agora = DateTime.UtcNow;


            bool isMasterGeral = secretariaBiz.verificarMasterGeral(login);
            //Regra para feriado passando null para usuario Master Geral
            if (isMasterGeral)
                entity.cd_pessoa_escola = null;
            if (((entity.aa_feriado == null || entity.aa_feriado_fim == null)
                    && DateTime.Compare(new DateTime(data_agora.Year, (int)entity.mm_feriado, (int)entity.dd_feriado), new DateTime(data_agora.Year, (int)entity.mm_feriado_fim, (int)entity.dd_feriado_fim)) > 0)
               || (entity.aa_feriado != null && entity.aa_feriado_fim != null
                    && DateTime.Compare(new DateTime((int)entity.aa_feriado, (int)entity.mm_feriado, (int)entity.dd_feriado), new DateTime((int)entity.aa_feriado_fim, (int)entity.mm_feriado_fim, (int)entity.dd_feriado_fim)) > 0))
                throw new CoordenacaoBusinessException(Messages.msgFeriadoInvalido, null, CoordenacaoBusinessException.TipoErro.VALIDACAO_DATAS_FERIADO, false);

            int? escola = null;
            if (!isMasterGeral)
                escola = cd_escola;
            List<Feriado> feriadoPeriodo = getFeriadosPorPeriodo(entity, escola);
            if (feriadoPeriodo.Count() > 0)
                throw new CoordenacaoBusinessException(Messages.msgErroFeriadoPeriodo, null, CoordenacaoBusinessException.TipoErro.PERIODO_EXISTENTE, false);
            Feriado feriado;
            this.sincronizaContexto(daoFeriado.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.READ_COMMITED, daoFeriado.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            {
                feriado = daoFeriado.add(entity, false);

                //Cria a auditoria do feriado individual:
                SGFWebContext dbComp = new SGFWebContext();
                logGeralBusiness.geraLogIndividual(cd_escola, cd_usuario, Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Feriado"].ToString()), (int)FundacaoFisk.SGF.GenericModel.TipoLog.TipoLogEnum.INCLUSAO, feriado.cod_feriado,
                    feriado, null);
                refez_programacoes = daoFeriado.spRefazerProgramacoesFeriado(feriado.cod_feriado, Feriado.TipoOperacaoSPFeriado.INSERCAO);
                if (!refez_programacoes)
                    throw new CoordenacaoBusinessException(Messages.msgNotIncludReg, null, CoordenacaoBusinessException.TipoErro.FERIDADO_ESCOLA, false);
                //Verifica se o feriado está sendo usado e refaz as programações:
                //List<Feriado> feriados = new List<Feriado>();
                //feriados.Add(entity);
                //refez_programacoes = turmaBusiness.atualizaProgramacoesTurma(feriados, cd_escola, isMasterGeral, false, true, false);
                transaction.Complete();
            }
            return feriado;
        }

        public Feriado editFeriado(Feriado entity, int cd_escola, string login, ref bool refez_programacoes, int cd_usuario)
        {
            Feriado feriado;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.READ_COMMITED, daoFeriado.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            {
                this.sincronizaContexto(daoFeriado.DB());
                bool isMasterGeral = secretariaBiz.verificarMasterGeral(login);
                //Regra para feriado, pois o usuário comum/administrador, só pode alterar feriados da própria escola.
                Feriado feriadoEscola = daoFeriado.findById(entity.cod_feriado, false);
                if ((feriadoEscola.cd_pessoa_escola == null || feriadoEscola.cd_pessoa_escola == 0) && !isMasterGeral)
                    throw new CoordenacaoBusinessException(string.Format(Messages.msgNotEditDeleteFeriado, feriadoEscola.dc_feriado), null, CoordenacaoBusinessException.TipoErro.FERIDADO_ESCOLA, false);

                int? escola = null;
                if (!isMasterGeral)
                    escola = cd_escola;
                List<Feriado> feriadoPeriodo = getFeriadosPorPeriodo(entity, escola);
                if (feriadoPeriodo.Count() > 0)
                    foreach (Feriado f in feriadoPeriodo)
                        if (f.cod_feriado != entity.cod_feriado)
                            throw new CoordenacaoBusinessException(Messages.msgErroFeriadoPeriodo, null, CoordenacaoBusinessException.TipoErro.PERIODO_EXISTENTE, false);

                DateTime data_agora = DateTime.UtcNow;
                //Regra para feriado passando null para usuario Master Geral
                if (isMasterGeral)
                    entity.cd_pessoa_escola = null;
                if (((entity.aa_feriado == null || entity.aa_feriado_fim == null)
                        && DateTime.Compare(new DateTime(data_agora.Year, (int)entity.mm_feriado, (int)entity.dd_feriado), new DateTime(data_agora.Year, (int)entity.mm_feriado_fim, (int)entity.dd_feriado_fim)) > 0)
                   || (entity.aa_feriado != null && entity.aa_feriado_fim != null
                        && DateTime.Compare(new DateTime((int)entity.aa_feriado, (int)entity.mm_feriado, (int)entity.dd_feriado), new DateTime((int)entity.aa_feriado_fim, (int)entity.mm_feriado_fim, (int)entity.dd_feriado_fim)) > 0))
                    throw new CoordenacaoBusinessException(Messages.msgFeriadoInvalido, null, CoordenacaoBusinessException.TipoErro.VALIDACAO_DATAS_FERIADO, false);

                bool inativarFeriado = processaStatusFeriado(entity, feriadoEscola);

                //Cria a auditoria do feriado individual:
                SGFWebContext dbComp = new SGFWebContext();
                /*logGeralBusiness.geraLogIndividual(cd_escola, cd_usuario, Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Feriado"].ToString()), (int)FundacaoFisk.SGF.GenericModel.TipoLog.TipoLogEnum.ALTERACAO, entity.cod_feriado,
                    entity, feriadoEscola);*/

                feriadoEscola.copy(entity);
                feriadoEscola.aa_feriado = entity.aa_feriado;
                feriadoEscola.aa_feriado_fim = entity.aa_feriado_fim;

                if (inativarFeriado)
                {
                    feriado = daoFeriado.edit(feriadoEscola, false);

                    refez_programacoes = daoFeriado.spRefazerProgramacoesFeriado(entity.cod_feriado, Feriado.TipoOperacaoSPFeriado.DELECAO);
                    if (!refez_programacoes)
                        throw new CoordenacaoBusinessException(Messages.msgNotUpReg, null, CoordenacaoBusinessException.TipoErro.FERIDADO_ESCOLA, false);
                }
                else
                {
                    //Verifica se o feriado está sendo usado e refaz as programações:
                    //List<Feriado> feriados = new List<Feriado>();
                    //feriados.Add(entity);

                    //verificarFeriadoEscolas(feriados, isMaster);                   

                    bool alterouCamposQueGeraReprogramacoes = daoFeriado.DB().Entry(feriadoEscola).Property(f => f.id_feriado_ativo).IsModified ||
                                                              daoFeriado.DB().Entry(feriadoEscola).Property(f => f.dd_feriado).IsModified ||
                                                              daoFeriado.DB().Entry(feriadoEscola).Property(f => f.aa_feriado).IsModified ||
                                                              daoFeriado.DB().Entry(feriadoEscola).Property(f => f.mm_feriado).IsModified ||
                                                              daoFeriado.DB().Entry(feriadoEscola).Property(f => f.aa_feriado_fim).IsModified ||
                                                              daoFeriado.DB().Entry(feriadoEscola).Property(f => f.mm_feriado_fim).IsModified ||
                                                              daoFeriado.DB().Entry(feriadoEscola).Property(f => f.dd_feriado_fim).IsModified;

                    if (alterouCamposQueGeraReprogramacoes)
                    {
                        var programacoes = daoDiarioAula.verificaDiarioAposFeriado(entity.cod_feriado, cd_escola);
                        validacaoDiarioAula(programacoes);

                        refez_programacoes = daoFeriado.spRefazerProgramacoesFeriado(entity.cod_feriado, Feriado.TipoOperacaoSPFeriado.DELECAO);
                        if (!refez_programacoes)
                            throw new CoordenacaoBusinessException(Messages.msgNotUpReg, null, CoordenacaoBusinessException.TipoErro.FERIDADO_ESCOLA, false);
                        feriado = daoFeriado.edit(feriadoEscola, false);
                        refez_programacoes = daoFeriado.spRefazerProgramacoesFeriado(feriado.cod_feriado, Feriado.TipoOperacaoSPFeriado.INSERCAO);
                        if (!refez_programacoes)
                            throw new CoordenacaoBusinessException(Messages.msgNotUpReg, null, CoordenacaoBusinessException.TipoErro.FERIDADO_ESCOLA, false);
                    }
                    else
                    {
                        feriado = daoFeriado.edit(feriadoEscola, false);
                    }
                }
                transaction.Complete();
            }
            return feriado;
        }

        private bool processaStatusFeriado(Feriado feriadoView, Feriado feriadoContext)
        {
            if (!feriadoView.id_feriado_ativo && feriadoContext.id_feriado_ativo)
            {
                feriadoContext.copy(feriadoView);
                feriadoContext.aa_feriado = feriadoView.aa_feriado;
                feriadoContext.aa_feriado_fim = feriadoView.aa_feriado_fim;

                bool alterouCamposFeriado = daoFeriado.DB().Entry(feriadoContext).Property(f => f.dc_feriado).IsModified ||
                                            daoFeriado.DB().Entry(feriadoContext).Property(f => f.id_feriado_financeiro).IsModified ||
                                            daoFeriado.DB().Entry(feriadoContext).Property(f => f.dd_feriado).IsModified ||
                                            daoFeriado.DB().Entry(feriadoContext).Property(f => f.aa_feriado).IsModified ||
                                            daoFeriado.DB().Entry(feriadoContext).Property(f => f.mm_feriado).IsModified ||
                                            daoFeriado.DB().Entry(feriadoContext).Property(f => f.aa_feriado_fim).IsModified ||
                                            daoFeriado.DB().Entry(feriadoContext).Property(f => f.mm_feriado_fim).IsModified ||
                                            daoFeriado.DB().Entry(feriadoContext).Property(f => f.dd_feriado_fim).IsModified;

                if (alterouCamposFeriado)
                    throw new CoordenacaoBusinessException(Messages.erroStatusFeriado, null, CoordenacaoBusinessException.TipoErro.ERRO_STATUS_FERIADO, false);

                return true;
            }
            return false;
        }

        public bool deleteAllFeriado(List<Feriado> feriados, int cd_escola, string login, ref Boolean refez_programacoes, int cd_usuario)
        {
            bool retorno;
            this.sincronizaContexto(daoFeriado.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.READ_COMMITED, daoFeriado.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            {
                //Verifica se o usuário é master geral:
                bool isMaster = secretariaBiz.verificarMasterGeral(login);

                //Regra para feriado, pois o usuário comum/administrador, só pode alterar feriados da própria escola.
                verificarFeriadoEscolas(feriados, isMaster);
                retorno = this.deletarFeriados(feriados, ref refez_programacoes, cd_escola);

                //Cria a auditoria do feriado individual:
                SGFWebContext dbComp = new SGFWebContext();
                /*foreach (Feriado feriado in feriados)
                    logGeralBusiness.geraLogIndividual(cd_escola, cd_usuario, Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Feriado"].ToString()), (int)FundacaoFisk.SGF.GenericModel.TipoLog.TipoLogEnum.DELECAO, feriado.cod_feriado,
                        null, feriado);*/

                transaction.Complete();
            }
            return retorno;
        }

        public bool deletarFeriados(List<Feriado> feriados, ref Boolean refez_programacoes, int cd_escola)
        {
            bool retorno = false;
            this.sincronizaContexto(daoFeriado.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.READ_COMMITED, daoFeriado.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            {
                if (feriados != null && feriados.Count() > 0)
                {
                    List<int> listaCodigos = feriados.Select(x => x.cod_feriado).ToList();
                    List<Feriado> feriadosContext = daoFeriado.getAllFeriados(listaCodigos).ToList();
                    foreach (var feriado in feriadosContext)
                    {
                        var programacoes = daoDiarioAula.verificaDiarioAposFeriado(feriado.cod_feriado, cd_escola);
                        validacaoDiarioAula(programacoes);

                        refez_programacoes = daoFeriado.spRefazerProgramacoesFeriado(feriado.cod_feriado, Feriado.TipoOperacaoSPFeriado.DELECAO);
                        if (!refez_programacoes)
                            throw new CoordenacaoBusinessException(Messages.msgNotDeletedReg, null, CoordenacaoBusinessException.TipoErro.FERIDADO_ESCOLA, false);
                        retorno = daoFeriado.delete(feriado, false);
                    }
                }
                transaction.Complete();
            }
            return retorno;
        }

        private void validacaoDiarioAula(List<ProgramacaoTurma> programacoes)
        {
            if (programacoes.Any())
            {
                var turmas = string.Empty;
                foreach (var programacao in programacoes)
                    turmas += programacao.no_turma + ", ";
                throw new CoordenacaoBusinessException(Messages.msgErroDiaroAposDataFeriado + "<br><br>" + "TURMAS: " + turmas, null, CoordenacaoBusinessException.TipoErro.FERIDADO_ESCOLA, false);
            }
        }

        private void verificarFeriadoEscolas(List<Feriado> feriados, bool isMaster)
        {
            Feriado feriado = new Feriado();
            if (!isMaster)
                foreach (var item in feriados)
                {
                    feriado = daoFeriado.findById(item.cod_feriado, false);
                    if (feriado.cd_pessoa_escola == null || feriado.cd_pessoa_escola == 0)
                        throw new CoordenacaoBusinessException(string.Format(Messages.msgNotEditDeleteFeriado, feriado.dc_feriado), null, CoordenacaoBusinessException.TipoErro.FERIDADO_ESCOLA, false);
                }
        }

        #endregion

        #region Horário

        public IEnumerable<Horario> getHorarioByEscolaForRegistro(int cdEscola, int cdRegistro, Horario.Origem origem)
        {
            return secretariaBiz.getHorarioByEscolaForRegistro(cdEscola, cdRegistro, origem);
        }

        #endregion

        #region Nome Contrato

        public IEnumerable<NomeContrato> getNomesContrato(Secretaria.DataAccess.NomeContratoDataAccess.TipoConsultaNomeContratoEnum hasDependente, int? cd_nome_contrato, int? cd_escola)
        {
            IEnumerable<NomeContrato> retorno = new List<NomeContrato>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = secretariaBiz.getNomesContrato(hasDependente, cd_nome_contrato, cd_escola).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        #endregion

        #region Histórico Aluno

        public int retunMaxSequenciaHistoricoAluno(int cd_produto, int cd_pessoa_escola, int cd_aluno)
        {
            return secretariaBiz.retunMaxSequenciaHistoricoAluno(cd_produto, cd_pessoa_escola, cd_aluno);
        }

        public void addHistoricoAluno(HistoricoAluno historicoAluno)
        {
            secretariaBiz.addHistoricoAluno(historicoAluno);
        }

        public HistoricoAluno GetHistoricoAlunoPrimeiraAula(int cdEscola, int cdAluno, int cdTurma, int cdContrato, DateTime dataDiario)
        {
            return secretariaBiz.GetHistoricoAlunoPrimeiraAula(cdEscola, cdAluno, cdTurma, cdContrato, dataDiario);
        }

        public bool deleteHistoricoAluno(HistoricoAluno historico)
        {
            return secretariaBiz.deleteHistoricoAluno(historico);
        }

        public List<FuncionarioSearchUI> getProfessoresByEmpresa(int cd_escola, int cd_turma)
        {
            return professorBusiness.getProfessoresByEmpresa(cd_escola, cd_turma).ToList();
        }


        #endregion

        #region Desistência

        public bool getExisteDesistenciaPorAlunoTurma(int cd_aluno, int cd_turma, int cd_pessoa_escola)
        {
            return secretariaBiz.getExisteDesistenciaPorAlunoTurma(cd_aluno, cd_turma, cd_pessoa_escola);
        }

        #endregion

        #region Diário Aula

        public DiarioAula addDiarioAula(DiarioAula diarioAula){
            return daoDiarioAula.add(diarioAula, false);
        }

        public bool deletarDiarioAula(DiarioAula diarioAula)
        {
            return daoDiarioAula.delete(diarioAula, false);
        }

        public vi_diario_aula getDiarioForGridById(int cd_diario_aula, int cd_pessoa_escola)
        {
            return daoDiarioAula.getDiarioForGridById(cd_diario_aula, cd_pessoa_escola);
        }

        public IEnumerable<DiarioAula> getDiarioAulas(int[] cdDiarios, int cd_escola)
        {
            return daoDiarioAula.getDiarioAulas(cdDiarios, cd_escola);
        }

        public DiarioAula getEditDiarioAula(int cd_diario_aula, int cd_escola)
        {
            return daoDiarioAula.getEditDiarioAula(cd_diario_aula, cd_escola);
        }

        public IEnumerable<DiarioAulaProgramcoesReport> getRelatorioDiarioAulaProgramacoes(int cd_escola, int? cd_turma, bool mais_turma_pagina, DateTime? dt_inicial, DateTime? dt_final, bool lancada)
        {
            List<DiarioAulaProgramcoesReport> listaRetorno = new List<DiarioAulaProgramcoesReport>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, daoDiarioAula.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                listaRetorno = daoDiarioAula.getRelatorioDiarioAulaProgramacoes(cd_escola, cd_turma, mais_turma_pagina, dt_inicial, dt_final, lancada).ToList();
                transaction.Complete();
            }

            return listaRetorno;
        }

        public string getObsDiarioAula(int cd_diario_aula, int cd_escola)
        {
            return daoDiarioAula.getObsDiarioAula(cd_diario_aula, cd_escola);
        }
        public int returnQuantidadeDiarioAulaByTurma(int cd_turma, int cd_escola, int cd_aluno)
        {
            return daoDiarioAula.returnQuantidadeDiarioAulaByTurma(cd_turma, cd_escola, cd_aluno);
        }
        public int returnDiarioByDataDesistencia(DateTime? data_desistencia, int cd_pessoa_escola, int cd_aluno, int cd_turma_aluno, int tipoDesistencia)
        {
            return daoDiarioAula.returnDiarioByDataDesistencia(data_desistencia, cd_pessoa_escola,cd_aluno,cd_turma_aluno,tipoDesistencia);
        }
        public bool verificaIntersecaoTurmaPersonalizada(int cd_turma, int? cd_programacao_turma, TimeSpan hr_inicial_aula, TimeSpan hr_final_aula, DateTime data)
        {
            return daoDiarioAula.verificaIntersecaoTurmaPersonalizada(cd_turma, cd_programacao_turma, hr_inicial_aula, hr_final_aula, data);
        }
        public bool verificaExisteDiarioEfetivoProgramacaoTurma(int cd_turma, int cd_escola, int cd_programacao)
        {
            return daoDiarioAula.verificaExisteDiarioEfetivoProgramacaoTurma(cd_turma, cd_escola, cd_programacao);
        }
        public bool verificaIntersecaoHorarios(int cd_turma, int? cd_programacao_turma, TimeSpan hr_inicial_aula, TimeSpan hr_final_aula, DateTime data)
        {
            return daoDiarioAula.verificaIntersecaoHorarios(cd_turma, cd_programacao_turma, hr_inicial_aula, hr_final_aula, data);
        }
        public DiarioAula getUltimaAulaAluno(int cd_aluno, int cd_turma, int cd_escola)
        {
            return daoDiarioAula.getUltimaAulaAluno(cd_aluno, cd_turma, cd_escola);
        }
        public DiarioAula getUltimaAulaTurma(int cd_turma, int cd_escola)
        {
            return daoDiarioAula.getUltimaAulaTurma(cd_turma, cd_escola);
        }
        
        public IEnumerable<vi_diario_aula> searchDiarioAula(SearchParameters parametros, int cd_turma, string no_professor, int cd_tipo_aula, byte status, byte presProf,
                                                       bool substituto, bool inicio, DateTime? dtInicial, DateTime? dtFinal, int cd_escola, int? cdProf, int cd_escola_combo)
        {
            IEnumerable<vi_diario_aula> retorno = new List<vi_diario_aula>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_turma";
                parametros.sort = parametros.sort.Replace("dta_aula", "dt_aula");
                retorno = daoDiarioAula.searchDiarioAula(parametros, cd_turma, no_professor, cd_tipo_aula, status, presProf, substituto, inicio, dtInicial, dtFinal, cd_escola, cdProf, cd_escola_combo).ToList();
                transaction.Complete();
            }
            return retorno;
        }
        public int getQtdDiarioTurma(int cd_turma, DateTime dt_aula)
        {
            return daoDiarioAula.qtdDiarioAulaTurmaData(cd_turma, dt_aula);
        }

        #endregion

        #region Controle de Faltas
        public IEnumerable<TurmaSearch> getTurmasComPercentualFaltaSearch(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto,
            int situacaoTurma, int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola,
            bool turmasFilhas, int cdAluno, DateTime? dtInicial, DateTime? dtFinal, int? cd_turma_PPT, bool semContrato, int tipoConsulta, DateTime? dt_inicial, DateTime? dt_final, bool ProfTurmasAtuais, bool id_percentual_faltas, List<int> cdSituacoesAlunoTurma = null)
        {
            IEnumerable<TurmaSearch> retorno = new List<TurmaSearch>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_turma";
                parametros.sort = parametros.sort.Replace("dtaIniAula", "dt_inicio_aula");

                retorno = daoTurma.searchTurmaComPercentualFaltas(parametros, descricao, apelido, inicio, tipoTurma, cdCurso, cdDuracao, cdProduto, situacaoTurma, cdProfessor, tipoProg, cdEscola, turmasFilhas, cdAluno, dtInicial, dtFinal, cd_turma_PPT, semContrato, tipoConsulta, dt_inicial, dt_final, ProfTurmasAtuais, id_percentual_faltas,null);
                transaction.Complete();
            }
            return retorno;
        }

        public Turma findTurmaPercentualFaltaGrupoAvancado(int cdEscola, int cd_turma, int? cd_turma_ppt, bool id_turma_ppt)
        {
            return daoTurma.findTurmaPercentualFaltaGrupoAvancado(cdEscola, cd_turma, cd_turma_ppt, id_turma_ppt);
        }

        public void editTurmaControleFalta(int cd_turma)
        {

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                Turma turma = daoTurma.findById(cd_turma, false);
                turma.id_percentual_faltas = true;
                daoTurma.saveChanges(false);
                transaction.Complete();
            }

        }
        #endregion
               
    }
}