using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IProfessorDataAccess : IGenericRepository<Professor>
    {
        IEnumerable<ProfessorUI> getProfessorReturnProfUI(ProfessorDataAccess.TipoConsultaProfessorEnum hasDependente, int cdEscola, int? cdProfessor);
        IEnumerable<ProfessorUI> getFuncionariosByEscola(int cdEscola, int? cdProfessor, bool? status);
        IEnumerable<ProfessorUI> getFuncionariosByEscolaAtividade(int cdEscola, int? cdProfessor, int? cd_atividade_extra, bool? status);
        void deletarHabilitacaoProfessor(HabilitacaoProfessor habilitacaoProfessor);
        void addHabilitacaoProfessor(HabilitacaoProfessor habilitacaoProfessor);
        IEnumerable<FuncionarioSearchUI> getProfessoresDisponiveisFaixaHorario(SearchParameters parametros, string desc, string nomeRed, bool inicio, bool? status, string cpf, int sexo, List<Horario> horariosTurma, 
            int cd_escola, int cd_turma, int cd_curso, bool PPT_pai, int cd_produto, DateTime dtInicio, DateTime? dtFinal, int cd_duracao);
        Professor getFuncionarioEditById(int cdFuncionario, int cdEmpresa, ProfessorDataAccess.TipoConsultaProfessorEnum tipo);
        List<HabilitacaoProfessor> getAllHabilitacaoProfessorByCdProfessor(int cdProfessor);
        FuncionarioSGF getProfLogByCodPessoaUsuario(int cd_pessoa_usuario, int cd_pessoa_empresa);
        bool deleteProfessor(int cd_professor, int cd_escola);
        int addProfExistFunc(Professor prof);
        IEnumerable<Professor> getProfessoresDisponiveisFaixaHorarioTurma(TurmaAlunoProfessorHorario turmaProfessorHorario, int cdEscola);
        ProfessorUI verificaRetornaSeUsuarioLogadoEProfessor(int cd_pessoa_usuario, int cd_pessoa_empresa);
        IEnumerable<ProfessorUI> getProfessorAvaliacaoTurma(int cd_pessoa_empresa);
        IEnumerable<ProfessorUI> getFuncionariosByEscolaWithAtividadeExtra(int cd_pessoa_escola, bool status);
		IEnumerable<FuncionarioSearchUI> getProfessoresByCodigos(int[] cdProfs, int cd_escola);
        IEnumerable<FuncionarioSearchUI> getProfessoresTurmaPPTPorFaixaHorariosTurmaFilha(List<Horario> horariosTurmaFilha, int cd_escola, int cd_turma_PPT);
        IEnumerable<ProfessorUI> getProfHorariosProgTurma(int cd_turma, int cd_escola, byte diaSemana, TimeSpan horaIni, TimeSpan horaFim);
        IEnumerable<ProfessorUI> getProfessorTurma(int cd_escola, int cd_turma);
        IEnumerable<ProfessorUI> getProfessorTurmaLogado(int cd_escola, int cd_turma, int cd_usuario);
        IEnumerable<FuncionarioSearchUI> getProfessoresByEmpresa(int cd_pessoa_empresa, int? cd_turma);
        FuncionarioSGF findByIdFuncionario(int cd_funcionario, int cd_pessoa_escola);
        void deleteProfessorContexto(FuncionarioSGF funcionario);
        FuncionarioSearchUI getProfessoresById(int cdProf, int cd_escola);
        IEnumerable<FuncionarioSearchUI> getSearchFuncionario(SearchParameters parametros, string nome, string apelido, bool? status, string cpf, bool inicio, byte tipo, int cdEscola, int sexo,
   int cdAtividade, int coordenador, int colaborador_cyber);
        IEnumerable<FuncionarioSGF> getFuncionariosByAulaPers(int cdEscola);

        //Relatório pagamento professor
        IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessores(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim);
        IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessoresFaltas(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim);
        IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessoresObs(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim);

        //Relatorio de Comissão das Secretarias
        IEnumerable<FuncionarioComissao> getRptComissaoSecretarias(int cd_funcionario, int cd_produto, int cd_empresa, DateTime? dt_ini, DateTime? dt_fim);
        List<int> getVerificaProfessorHabilitacaoCursos(List<int>cdProfs, int cd_curso, int cd_escola);

        IEnumerable<FuncionarioSearchUI> getProfessoresAulaReposicao(int cd_escola);
        IEnumerable<ProfessorUI> getFuncionariosByEscolaAulaReposicao(int cdEscola, int? cdProfessor, bool? status);
        IEnumerable<ProfsemDiarioUI> getProfessorsemDiario(SearchParameters parametros, int cd_turma, int cd_professor, int cdEscola, bool idLiberado);
        string postLiberarProfessor(int cd_professor, int cd_usuario, int fuso);
    }
}
