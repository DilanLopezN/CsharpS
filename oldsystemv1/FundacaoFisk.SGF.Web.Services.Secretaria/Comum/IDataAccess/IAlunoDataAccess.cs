using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using System.Data;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IAlunoDataAccess : IGenericRepository<Aluno>
    {
        IEnumerable<AlunoSearchUI> getAlunoSearchFollowUp(Componentes.Utils.SearchParameters parametros, string desc, bool inicio, int cdEscola, string email, string telefone, 
            AlunoDataAccess.TipoConsultaAlunoEnum tipo);
        IEnumerable<AlunoSearchUI> findAluno(int cdEscola);
        Aluno getAlunoById(int idAluno, int cdEscola);
        IEnumerable<AlunoSearchUI> getAlunoSearch(SearchParameters parametros, string desc, string email, bool inicio, bool? status, int cdEscola, string cpf, List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, bool matriculasem, bool matricula);
        IEnumerable<AlunoSearchUI> getAlunoFKReajusteSearch(SearchParameters parametros, string desc, string email, bool inicio, bool? status, int cdEscola, string cpf, List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, DateTime? dt_inicial, DateTime? dt_final);
        IEnumerable<AlunoSearchUI> GetAlunoSearchAulaPer(SearchParameters parametros, string desc, string email, bool inicio, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, DateTime dtaAula);
        Aluno getAllDataAlunoById(int cdAluno, int cdEscola, bool editView);
        Aluno getAlunoByCpf(string cpf, string email, string nome, int cd_pessoa_cpf, int cdEscola);
        Aluno getAlunoByCpfAluno(string cpf, string email, string nome, int cd_pessoa_cpf, int cdEscola);
        IEnumerable<AlunoSearchUI> getAlunoByEscola(int cdEscola, bool? status, int? cd_aluno);
        AlunoSearchUI verificarAlunoExistEmail(string email, int cdEscola, int cd_prospect);
        Aluno buscarAlunoExistEmail(string email, string nome, int cd_pessoa_cpf);
        
        List<Aluno> getAlunosByCod(int[] cdAlunos, int cdEscola);
        IEnumerable<AlunoSearchUI> getAlunosDisponiveisFaixaHorario(SearchParameters parametros, string desc, string email, bool inicio, bool? status,
                                                                    string cpf, int sexo, List<Horario> horariosTurma, int cd_escola, int cd_turma, int cd_produto, bool id_turma_PPT, 
                                                                    DateTime? dt_final_aula, int cd_curso, int cd_duracao);
        IEnumerable<Aluno> getAlunosDisponiveisFaixaHorario(int cd_turma,int? cd_turma_ppt, int cd_escola, Horario h, List<Aluno> alunos);
        IEnumerable<Aluno> getAlunosMatriculadosRematriculados(int cd_turma, int? cd_turma_ppt, int cd_escola, List<Aluno> alunos);
        IEnumerable<Aluno> getAlunosDisponiveisFaixaHorarioCancelEnc(int cd_turma, int cd_escola, Horario h, List<int> cdAlunos);
        List<int> getAlunosDisponiveisHistCancelEnc(int cd_turma, int cd_escola);
        IEnumerable<Aluno> returnAlunosSemAvalicacaoByTurma(int cd_escola, int cd_turma);
        IEnumerable<AlunoSearchUI> getAlunoPolitica(int cdEscola);
        IEnumerable<AlunoSearchUI> getAlunoSelecionado(int cdPolitica, int cdEscola);
        IEnumerable<Aluno> getAllAlunosTurmasFilhasPPT(int cd_turma_ppt, int cd_escola);
        Aluno findAlunoById(int cd_aluno, int cd_escola);
        Aluno getAlunoById(int cd_pessoa);
        IEnumerable<AlunoSearchUI> getAlunoSearchTurma(SearchParameters parametros, string nome, string email, bool inicio, int origemFK, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo);
        IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAula(int cd_turma, int cd_pessoa_escola, DateTime? dt_inicial, DateTime dt_final, AlunoTurma.FiltroSituacaoAlunoTurma situacao_aluno = AlunoTurma.FiltroSituacaoAlunoTurma.Todos);
        IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAulaDiario(int cd_turma, int cd_pessoa_escola, DateTime? dt_inicial, DateTime dt_final, AlunoTurma.FiltroSituacaoAlunoTurma situacao_aluno = AlunoTurma.FiltroSituacaoAlunoTurma.Todos);
        IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAulaCarga(int cd_turma, int cd_pessoa_escola, DateTime? dt_inicial, DateTime dt_final);
        IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAulaReport(int cd_turma, int cd_pessoa_escola, DateTime? dt_inicial, DateTime dt_final);
        IEnumerable<Aluno> getAlunosPorEventoDiario(int cd_turma, int cd_pessoa_escola, DateTime dtAula, int cd_evento, int cd_diario_aula);
        IEnumerable<Aluno> getAlunosPorEvento(int cd_turma, int cd_pessoa_escola, int cd_evento, int cd_diario_aula);
        IEnumerable<Aluno> getAlunoPorTurma(int cdTurma, int cdEscola, int opcao);
        IEnumerable<AlunoSearchUI> getAlunoDesistente(SearchParameters parametros, string nome, string email, bool inicio, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo);
        IEnumerable<AlunoSearchUI> getAlunoPorTurmaSearch(SearchParameters parametros, string desc, string email, bool inicio, int origemFK, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo, int cdTurma, int opcao);
        IEnumerable<AlunoSearchUI> getAlunoPorTurmaSearchAulaReposicao(SearchParameters parametros, string nome, string email, bool inicio, int origemFK, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo, int cdTurma, int opcao);
        bool existeAlunoMatOrRemEscola(int cdEscola);
        AlunoSearchUI getAlunoByCodForGrid(int cd_aluno, int cd_empresa);
        AlunoSearchUI getAlunoPorTurmaPPTFilha(int cdEscola, int cdTurma, int cdTurmaPai, int opcao);
        IEnumerable<AlunoRel> getRelAluno(string nome, int cdResp, string telefone, string email, bool? status, int cdEscola, DateTime? dtaIni, DateTime? dtaFinal, int cd_midia, List<int> cdSituacaoA, bool exibirEnderecos);
        IEnumerable<AlunoSearchUI> getRptAlunosTurma(int cd_turma, bool id_turma_ppt, List<int> cd_situacao_aluno_turma, int cdEscolaAluno);
        IEnumerable<AlunoSearchUI> getRptAlunosTurmaEncerrar(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, DateTime dtaFim, int cdEscolaAluno);
        IEnumerable<AlunoSearchUI> getRptAlunosTurmaNova(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, Nullable<DateTime> dtaFim, int cdEscolaAluno);
        bool existeProdutoAlunoMat(int cd_escola, bool turmaFilha, int cd_produto, List<int> cdAlunos, int cd_turma);
        IEnumerable<ReportPercentualTerminoEstagio> getRptAlunosProximoCurso(int cd_escola, int cd_turma);
        IEnumerable<AlunoSearchUI> getAlunoSearchFKPesquisas(SearchParameters parametros, string nome, string email, bool inicio, bool? status, int cdEscola, int origemFK, string cpf, 
            List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoConsultaAlunoEnum tipoConsulta);

        IEnumerable<AlunoSearchUI> getAlunoSearchFKPesquisasAtividade(SearchParameters parametros, string nome, string email, bool inicio, bool? status, int cdEscola, int origemFK, string cpf,
            List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoConsultaAlunoEnum tipoConsulta, int cd_escola_combo_fk, List<int> cds_escolas_atividade = null);
        IEnumerable<AlunoSearchUI> getAlunoSearchFKPesquisasAtividadeExtra(SearchParameters parametros, string nome, string email, bool inicio, bool? status, int cdEscola, string cpf,
            List<int> cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoConsultaAlunoEnum tipoConsulta, int cd_escola_combo_fk, List<int> cursos, List<int> cds_escolas_atividade = null);
        Contrato getMatriculaByTurmaAlunoHistorico(int cd_escola, int cd_aluno, int cd_contrato);
        
        string getObservacaoAluno(int cdAluno, int cdEscola);
        //Pessoa
        IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> GetPessoaSearch(SearchParameters parametros, string nome, string apelido, int tipoPessoa, string cnpjCpf, int? papel, int sexo, bool inicio, int cdEscola, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoPessoaEnum tipo);
        IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> getPessoaUsuarioSearch(SearchParameters parametros, string nome, string apelido, string cnpjCpf, int sexo, bool inicio, int cdEscola, int cd_pessoa_usuario);
        IEnumerable<PessoaSGFSearchUI> getPessoaMovimentoSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int tipoMovimento, int cd_empresa);
        IEnumerable<PessoaSearchUI> getPessoaRelacionadaEscola(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa);
        IEnumerable<PessoaSGFSearchUI> getPessoaTituloSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa, bool responsavel);
        IEnumerable<PessoaSearchUI> getPessoaSearchEscolaWithCPFCNPJ(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa, int papel);
        IEnumerable<PessoaSearchUI> getPessoaResponsavelCPFSearchEscola(SearchParameters parametros, string nome, string apelido, bool inicio, string cnpjCpf, int sexo, int cd_empresa);
        IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.PessoaSGFSearchUI> getPessoaPapelSearchWithCPFCNPJ(SearchParameters parametros, string nome, string apelido, int tipoPessoa, string cnpjCpf, int? papel, int sexo, bool inicio, int cdEscola);
        PessoaSGF verificaExistePessoaEmpresaByCpf(string cpf, int cd_empresa);
        PessoaSGF verificaExistePessoaJuridicaEmpresaByCpf(string cnpj, int cd_empresa);
        PessoaSGF getPessoaById(int cd_pessoa);
        List<PessoaSGF> getListaAniversariantes(int cd_escola, int tipo, int cd_turma, int mes, int dia);
        IEnumerable<PessoaSearchUI> getTdsPessoaSearchEscola(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa);

        DataTable getRptMediaAlunos(int cd_escola, int cd_turma, int tipoTurma, int cdCurso, int cdProduto, int pesOpcao, int pesTipoAluno,
                decimal? vl_media, DateTime dtInicial, DateTime dtFinal);

        IEnumerable<AlunoSearchUI> getRptAlunosTurmaEncerrar(List<int> cdTurmas, DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int tipoTurma);
        IEnumerable<AlunoSearchUI> getRptAlunosTurmasNovas(List<int> cdTurmas, DateTime? pDtaI, DateTime? pDtaF, int cd_escola, int tipoTurma);

        IEnumerable<AlunoSearchUI> getAlunoPorTurmaControleFaltaSearch(SearchParameters parametros, List<int> cdAlunos, string desc, string email, bool inicio, bool? status, int cdEscola, string cpf, int cdSituacao, int sexo, int cdTurma, int opcao, DateTime? dataFinalHistorico);

        // SMS
        //List<PessoaSGF> getListaAniversariantesSms(int cd_escola, int tipo, int cd_turma, int mes, int dia);
        //List<PessoaSGF> getListaAniversariantesSms(int cd_escola);

        AlunoApiCyberBdUI FindAlunoByCdAluno(int cd_aluno, int cd_escola);
        IEnumerable<sp_RptCartaQuitacao_Result> findAlunoCartaQuitacao(SearchParameters parametros, int cdEscola, int ano, int cdPessoa);
        IEnumerable<AlunosemAulaUI> getAlunosemAula(SearchParameters parametros, int cd_aluno, int cd_item, int cdEscola, DateTime? dtaInicial, DateTime? dtaFinal, bool idMovimento, bool idHistorico, byte idSituacao);
        IEnumerable<AlunosemAulaUI> getNotasDevolvidas(SearchParameters parametros, int cd_aluno, int cd_item, int cdEscola, DateTime? dtaInicial, DateTime? dtaFinal, bool idMovimento, bool idHistorico, byte idSituacao);
        string postGerarKardexEntrada(int? cd_item_movimento, int? cd_usuario, int? fuso);
        IEnumerable<AlunosCargaHorariaUI> getAlunosCargaHoraria(SearchParameters parametros, int cd_aluno, int cd_turma, int cdEscola, DateTime? dtaInicial, DateTime? dtaFinal);
        IEnumerable<CargaHorariaUI> getCargaHoraria(SearchParameters parametros, int cd_aluno, int cd_turma, int cd_curso, int cdEscola, int cd_professor, bool totasEscolas, int nm_aulas_vencimento);
        string postGerarNotaVoucher(int cd_desistencia, int cd_usuario, int fusoHorario, int itemVoucher);
        bool getParametroEscolaInternacional(int cdEscola);
        Aluno findAlunoByCdPessoaAlunoAndCdEscolaDestino(int alunoBdCdPessoaAluno, int cdEscolaDestino);
        ProfessorCargaHorariaMaximaResultUI getExisteCargaHorariaProximaMaxima(int cdUsuario, int cdEscola);
    }
}
