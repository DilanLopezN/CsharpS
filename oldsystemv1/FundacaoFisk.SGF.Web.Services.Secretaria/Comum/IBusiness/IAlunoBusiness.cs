using System;
using System.Collections.Generic;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel.Partial;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum
{
    using System.Data.Entity;
    using Componentes.GenericBusiness.Comum;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
    using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
    using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
    using System.Data;

    public interface IAlunoBusiness : IGenericBusiness
    {
        //void configuraUsuario(int cdUsuario);
        void sincronizaContexto(DbContext db);
        //Aluno
        AlunoTurma findAlunosTurmaById(int cd_aluno_turma);
        IEnumerable<AlunoSearchUI> getAlunoSearchFollowUp(SearchParameters parametros, string desc, bool inicio, int cdEscola, string email, string telefone, AlunoDataAccess.TipoConsultaAlunoEnum tipo);
        IEnumerable<AlunoSearchUI> findAluno(int cdEscola);
        IEnumerable<AlunoSearchUI> getAlunoPolitica(int cdEscola);
        IEnumerable<AlunoSearchUI> getAlunoSearch(SearchParameters parametros, string desc, string email, bool inicio, bool? status, int cdEscola, string cpf, List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, bool matriculasem, bool matricula);
        IEnumerable<AlunoSearchUI> getAlunoSearchFKPesquisas(SearchParameters parametros, string nome, string email, bool inicio, bool? status, int cdEscola, int origemFK, string cpf, List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoConsultaAlunoEnum tipoConsulta);
        IEnumerable<AlunoSearchUI> getAlunoSearchFKPesquisasAtividade(SearchParameters parametros, string nome, string email, bool inicio, bool? status, int cdEscola, int origemFK, string cpf, List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoConsultaAlunoEnum tipoConsulta, int cd_escola_combo_fk, List<int> cds_escolas_atividade = null);
        IEnumerable<AlunoSearchUI> getAlunoSearchFKPesquisasAtividadeExtra(SearchParameters parametros, string nome, string email, bool inicio, bool? status, int cdEscola, string cpf, List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoConsultaAlunoEnum tipoConsulta, int cd_escola_combo_fk, List<int> cursos, List<int> cds_escolas_atividade = null); 
        IEnumerable<AlunoSearchUI> getAlunoFKReajusteSearch(SearchParameters parametros, string desc, string email, bool inicio, bool? status, int cdEscola, string cpf, List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, DateTime? dt_inicial, DateTime? dt_final);
        IEnumerable<AlunoSearchUI> GetAlunoSearchAulaPer(SearchParameters parametros, string desc, string email, bool inicio, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, DateTime dtaAula);
        Aluno addAluno(AlunoUI alunoUI);
        Aluno editAluno(AlunoUI alunoUI);
        Aluno getAllDataAlunoById(int cdAluno, int cdEscola, bool editView);
        Aluno getAlunoById(int cdAluno, int cdEscola);
        PessoaFisicaSearchUI ExistAlunoOrPessoaFisicaByCpf(string cpf, string email, string nome, int cd_pessoa_cpf, int cdEscola);
        PessoaFisicaSearchUI ExistAlunoOrPessoaFisicaByCpfAluno(string cpf, string email, string nome, int cd_pessoa_cpf, int cdEscola);
        IEnumerable<Horario> getHorarioByEscolaForAluno(int cdEscola, int cdAluno);
        IEnumerable<AlunoSearchUI> getAlunoByEscola(int cdEscola, bool? status, int? cd_aluno);
        AlunoSearchUI verificarAlunoExistEmail(string email, int cdEscola, int cd_prospect);
        Aluno buscarAlunoExistEmail(string email, string nome, int cd_pessoa_cpf);

        List<Aluno> getAlunosByCod(int[] cdAlunos, int cdEscola);
        IEnumerable<AlunoSearchUI> getAlunosDisponiveisFaixaHorario(SearchParameters parametros, string desc, string email, bool inicio, bool? status,
                                                                    string cpf, int sexo, List<Horario> horariosTurma, int cd_escola, int cd_turma, int cd_produto, bool id_turma_PPT, DateTime? dt_final_aula, int cd_curso, int cd_duracao);
        void verificarAlunosDisponiveisFaixaHorario(int cd_turma, int? cd_turma_ppt, int cd_escola, Horario h, List<Aluno> alunos);
        void getAlunosDisponiveisFaixaHorarioCancelEnc(int cd_turma, int? cd_turma_ppt,int cd_produto, int cd_escola, Horario h);
        List<int> getAlunosDisponiveisHistCancelEnc(int cd_turma, int cd_escola);
        IEnumerable<AlunoSearchUI> getAlunoSelecionado(int cdPolitica, int cdEscola);
        IEnumerable<Aluno> returnAlunosSemAvalicacaoByTurma(int cd_escola, int cd_turma);
        IEnumerable<Aluno> getAllAlunosTurmasFilhasPPT(int cd_turma_ppt, int cd_escola);
        IEnumerable<AlunoSearchUI> getAlunoSearchTurma(SearchParameters parametros, string nome, string email, bool inicio, int origemFK, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo);
        IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAula(int cd_turma, int cd_pessoa_escola, DateTime dtAula);
        IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAulaCarga(int cd_turma, int cd_pessoa_escola, DateTime dtAula);
        IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAulaDiario(int cd_turma, int cd_pessoa_escola, DateTime dtAula);
        IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAula(int cd_escola, int cd_turma, DateTime? dt_inicial, DateTime dt_final, AlunoTurma.FiltroSituacaoAlunoTurma situacao_aluno = AlunoTurma.FiltroSituacaoAlunoTurma.Todos);
        IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAulaReport(int cd_escola, int cd_turma, DateTime? dt_inicial, DateTime dt_final);
        IEnumerable<Aluno> getAlunoPorTurma(int cdTurma, int cdEscola, int opcao);
        IEnumerable<AlunoSearchUI> getAlunoDesistente(SearchParameters parametros, string desc, string email, bool inicio, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo);
        IEnumerable<AlunoSearchUI> getAlunoPorTurmaSearch(SearchParameters parametros, string desc, string email, bool inicio, int origemFK, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo, int cdTurma, int opcao);
        IEnumerable<AlunoSearchUI> getAlunoPorTurmaSearchAulaReposicao(SearchParameters parametros, string desc, string email, bool inicio, int origemFK, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo, int cdTurma, int opcao);
        bool existeAlunoMatOrRemEscola(int cdEscola);
        AlunoSearchUI getAlunoPorTurmaPPTFilha(int cdEscola, int cdTurma, int cdTurmaPai, int opcao);
        IEnumerable<AlunoRel> getRelAluno(string nome, int cdResp, string telefone, string email, bool? status, int cdEscola, DateTime? dtaIni, DateTime? dtaFinal, int cd_midia, List<int> cdSituacaoA, bool exibirEnderecos);
        IEnumerable<AlunoSearchUI> getRptAlunosTurma(int cd_turma, bool id_turma_ppt, List<int> cd_situacao_aluno_turma, int cdEscolaAluno);
        IEnumerable<AlunoSearchUI> getRptAlunosTurmaEncerrar(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, DateTime dtaFim, int cdEscolaAluno);
        IEnumerable<AlunoSearchUI> getRptAlunosTurmaPPTEncerrar(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, DateTime dtaFim, int cdEscolaAluno);
        IEnumerable<AlunoSearchUI> getRptAlunosTurmaNova(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, Nullable<DateTime> dtaFim, int cdEscolaAluno);
        IEnumerable<AlunoSearchUI> getRptAlunosTurmaPPTNova(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, DateTime dtaFim, int cdEscolaAluno);
        List<int> findAllEmpresasByUsuario(string login, int cdEscola);
        bool existeProdutoAlunoMat(int cd_escola, bool turmaFilha, int cd_produto, List<int> cdAlunos, int cd_turma);
        AlunoSearchUI editStatusAluno(int cdAluno, bool idAtivo, int cdEscola, int cdTurma);
        IEnumerable<ReportPercentualTerminoEstagio> getRptAlunosProximoCurso(int cd_escola, int cd_turma);
        List<Aluno> verificarAlunosSemTitulos(int cd_turma, DateTime dta_programacao_turma, int cd_escola);
        Contrato getMatriculaByTurmaAlunoHistorico(int cd_escola, int cd_aluno, int cd_contrato);
        AlunoSearchUI getAlunoByCodForGrid(int cd_aluno, int cd_empresa);
        Aluno findAlunoById(int cd_aluno, int cd_escola);

        //Aluno Turma
        IEnumerable<AlunoTurma> findAlunosTurmaPorTurmaEscola(int cd_turma, int cd_escola);
        IEnumerable<AlunoTurma> findAlunosTurma(int cd_turma, int cd_escola);
        AlunoTurma addAlunoTurma(AlunoTurma alunoTurma);
        AlunoTurma addAlunoTurmaMudancaInterna(AlunoTurma alunoTurma);
        AlunoTurma editAlunoTurma(AlunoTurma alunoTurma);
        void deletarAlunoTurma(AlunoTurma alunoTurma);
        bool getVerificaAlunosTurma(List<AlunoTurma> alunosTurma);
        bool getverifivaHorarioMudanca(List<AlunoTurma> alunosTurma, int cd_escola);
        bool existsAlunoTurmaByContratoEscola(int cd_contrato, int cd_pessoa_escola);
        List<AlunoTurma> findAlunoTurmasByContratoEscola(int cd_contrato, int cd_pessoa_escola);
        int existAlunoMatriculadoOuRematriculado(int cd_aluno, int cd_escola, int cd_turma);
        IEnumerable<Aluno> getAlunosPorEventoDiario(int cd_turma, int cd_pessoa_escola, DateTime dtAula, int cd_evento, int cd_diario_aula);
        IEnumerable<AlunoTurma> findAlunosTurma(int cd_turma, int cd_escola, int[] cdAlunos);
        void persistirAulaExecutaEFaltaAlunosDiarioAula(List<Aluno> alunosComFalta, int cd_turma, int cd_pessoa_escola, DateTime dtAula, DiarioAula.StatusDiarioAula statusDiarioAula);
        IEnumerable<Aluno> getAlunosPorEvento(int cd_turma, int cd_pessoa_escola, int cd_evento, int cd_diario_aula);
        void incrementarOuRemoverFaltaAlunoTurmaDiario(List<Aluno> alunos, int cd_turma, int cd_pessoa_escola, DiarioAula.StatusDiarioAula statusDiarioAula);
        AlunoTurma findAlunoTurma(int cd_aluno, int cd_turma, int cd_escola);
        LivroAlunoApiCyberBdUI findLivroAlunoTurmaApiCyber(int cd_aluno, int cd_turma, int cd_escola);
        AlunoTurma findAlunoTurmaByCdCursoContrato(int cd_curso_contrato, int cd_escola);
        List<AlunoTurma> existsAlunosTurmaInTurmaDestino(List<int> cdsAlunosTurma, int cdTurmaDestino);
        int findAlunoTurmaProduto(int cd_aluno, int cd_turma, int cd_escola);
        bool verificacoesMudanca(List<AlunoTurma> alunos, int cdEscola);
        AlunoTurma findAlunoTurmaById(int id);
        void saveChagesAlunoTurma(AlunoTurma alunoTurma);
        AlunoTurma findAlunoTurmaContrato(int cd_aluno, int cd_turma, int cd_escola, int cd_contrato);
        List<Aluno> verificarAlunosCompraMaterialDidatico(int cd_turma, DateTime dta_programacao_turma, int cd_escola);
        IEnumerable<AlunoTurma> findAlunosTurmaHist(int cd_turma, int cd_escola, int[] cdAlunos);
        string getObservacaoAluno(int cdAluno, int cdEscola);
        List<AlunoTurma> findAlunosTurmaForEncerramento(int cd_turma, int cd_escola);
        bool deleteAlunoAguardandoTurma(int cdProduto, int cdEscola, int cdContrato, int cdAluno, int cd_turma);
        IEnumerable<AlunoSearchUI> getRptAlunosTurmaEncerrar(List<int> cdTurmas, DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int tipoTurma);
        IEnumerable<AlunoSearchUI> getRptAlunosTurmasNovas(List<int> cdTurmas, DateTime? pDtaI, DateTime? pDtaF, int cd_escola, int tipoTurma);
        List<AlunoTurma> getAlunoTurmaByCdContrato(int cd_contrato);
        List<AlunoTurma> getAlunoTurmaByCdContratoAndCdAluno(int cd_contrato, int cd_aluno);
        AlunoApiCyberBdUI findAlunoApiCyber(int cd_aluno, int cd_turma, int cd_contrato);
        PromocaoIntercambioParams findAlunoApiPromocaoIntercambio(int cd_aluno, int id_tipo_matricula);
        LivroAlunoApiCyberBdUI findLivroAlunoApiCyber(int cd_aluno, int cd_turma, int cd_contrato);
        //Mudanças Internas
        MudancasInternas postMudancaInterna(MudancasInternas mudanca);
        void gerarHistoricoMudanca(AlunoTurma alunoTurmaAnterior, MudancasInternas mudanca, int cdAluno, int situacaoManterContrato);
        int? getStatusAlunoTurma(int cd_aluno, int cd_escola, int cd_turma);
        void deletarAluno(Aluno aluno);
        void setHorarioAluno(List<Horario> horariosUI, int cdAluno, int cdEscola);
        //Pessoa
        IEnumerable<PessoaSGFSearchUI> getPessoaMovimentoSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int tipoMovimento, int cd_empresa);
        IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> GetPessoaSearch(SearchParameters parametros, string nome, string apelido, int tipoPessoa, string cnpjCpf, int? papel, int sexo, bool inicio, int cdEscola, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoPessoaEnum tipo);
        IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> getPessoaUsuarioSearch(SearchParameters parametros, string nome, string apelido, string cnpjCpf, int sexo, bool inicio, int cdEscola, int cd_pessoa_usuario);
        IEnumerable<PessoaSearchUI> getPessoaRelacionadaEscola(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa);
        IEnumerable<PessoaSGFSearchUI> getPessoaTituloSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa, bool responsavel);
        IEnumerable<PessoaSearchUI> getPessoaSearchEscolaWithCPFCNPJ(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa, int papel);
        IEnumerable<PessoaSearchUI> getPessoaResponsavelCPFSearchEscola(SearchParameters parametros, string nome, string apelido, bool inicio, string cnpjCpf, int sexo, int cd_empresa);
        IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> getPessoaPapelSearchWithCPFCNPJ(SearchParameters parametros, string nome, string apelido, int tipoPessoa, string cnpjCpf, int? papel, int sexo, bool inicio, int cdEscola);
        PessoaFisicaSearchUI existePessoaEscolaOrByCpf(string cpf, int cdEscola);
        PessoaJurdicaSearchUI existePessoaJuridicaEscolaOrByCNPJ(string cnpj, int cdEscola);
        PessoaSGF getPessoaById(int cd_pessoa);
        List<PessoaSGF> getListaAniversariantes(int cd_escola, int tipo, int cd_turma, int mes, int dia);
        IEnumerable<PessoaSearchUI> getTdsPessoaSearchEscola(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa);
        //Aluno Bolsa
        IEnumerable<RptBolsistas> getBolsistas(int cdEscola, int cd_aluno, int cd_turma, bool cancelamento, decimal? per_bolsa, int cd_motivo_bolsa, DateTime? dtIniComunicado,
                                                        DateTime? dtFimComunicado, DateTime? dtIni, DateTime? dtFim, bool periodo_ini, bool periodo_cancel);

        DataTable getRptMediaAlunos(int cd_escola, int cd_turma, int tipoTurma, int cdCurso, int cdProduto, int pesOpcao, int pesTipoAluno,
                decimal? vl_media, DateTime dtInicial, DateTime dtFinal);
        void findByIdAndDeleteAlunoBolsa(int cd_aluno);

        //Horário
        IEnumerable<Horario> getHorarioByEscolaForRegistroUncommited(int cdEscola, int cdRegistro, Horario.Origem origem);
        IEnumerable<Horario> getHorarioByEscolaForRegistro(int cdEscola, int cdRegistro, Horario.Origem origem);
        bool deleteHorario(Horario horario);
        Horario addHorario(Horario horario);
        Horario editHorarioContext(Horario horarioContext, Horario horarioView);
        IEnumerable<Horario> getHorarioOcupadosForTurma(int cdEscola, int cdRegistro, int[] cdProfessores, int cd_turma,
            int cd_duracao, int cd_curso, DateTime dt_inicio, DateTime? dt_final, HorarioDataAccess.TipoConsultaHorario tipoCons);
        IEnumerable<Horario> getHorarioOcupadosForSala(Turma turma, int cdEscola, HorarioDataAccess.TipoConsultaHorario tipoCons);
        int countHorariosUsuario(int cd_empresa, int cd_usuario);
        bool getHorarioByHorario(int cdEscola, int cdRegistro, Horario.Origem origem, TimeSpan hr_servidor, int diaSemanaAtual);
        string retornaDescricaoHorarioOcupado(int cd_empresa, TimeSpan hr_ini, TimeSpan hr_fim);

        //ControleFaltas
        IEnumerable<AlunoSearchUI> getAlunoPorTurmaControleFaltaSearch(SearchParameters parametros, List<int> cdAlunos, string nome, string email, bool inicio, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo, int cdTurma,  int opcao, DateTime? dataFinalHistorico);
        List<sp_RptCartaQuitacao_Result> findAlunoCartaQuitacao(SearchParameters parametros, int cdEscola, int ano, int cdPessoa);
        IEnumerable<AlunosemAulaUI> getAlunosemAula(SearchParameters parametros, int cd_aluno, int cd_item, int cdEscola, DateTime? dtaInicial, DateTime? dtaFinal, bool idMovimento, bool idHistorico, byte idSituacao);
        IEnumerable<AlunosemAulaUI> getNotasDevolvidas(SearchParameters parametros, int cd_aluno, int cd_item, int cdEscola, DateTime? dtaInicial, DateTime? dtaFinal, bool idMovimento, bool idHistorico, byte idSituacao);
        string postGerarKardexEntrada(int cd_item_movimento, int? cd_usuario, int? fuso);
        IEnumerable<AlunosCargaHorariaUI> getAlunoCargaHoraria(SearchParameters parametros, int cd_aluno, int cd_turma, int cdEscola, DateTime? dtaInicial, DateTime? dtaFinal);
        IEnumerable<CargaHorariaUI> getCargaHoraria(SearchParameters parametros, int cd_aluno, int cd_turma, int cd_curso, int cdEscola, int cd_professor, bool todasEscolas, int nm_carga);
        string postGerarNotaVoucher(int cd_desistencia, int cd_usuario, int fusoHorario, int itemVoucher);
        bool getParametroEscolaInternacional(int cdEscola);
        ProfessorCargaHorariaMaximaResultUI getExisteCargaHorariaProximaMaxima(int cdUsuario, int cdEscola);
    
        void deleteFichaAluno(int alunoCdAluno);
    }
}
