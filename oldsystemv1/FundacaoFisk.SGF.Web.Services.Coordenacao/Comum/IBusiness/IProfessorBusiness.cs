using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericBusiness.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness
{
    public interface IProfessorBusiness : IGenericBusiness
    {
        void sincronizarContextos(DbContext dbContext);
        //Professor
        FuncionarioSearchUI addFuncionario(FuncionarioUI funcionarioUI, List<RelacionamentoSGF> relacionamentos, string fullPath);
        Professor getFuncionarioEditById(int cdFuncionario, int cdEscola,ProfessorDataAccess.TipoConsultaProfessorEnum tipo);
        FuncionarioSearchUI editFuncionario(FuncionarioUI funcionarioUI, List<RelacionamentoSGF> relacionamentos, bool permissaoSalario, string fullPath);
        IEnumerable<ProfessorUI> getProfessorReturnProfUI(ProfessorDataAccess.TipoConsultaProfessorEnum hasDependente, int cdEscola, int? cdProfessor);
        IEnumerable<ProfessorUI> getProfessorTurma(int cd_escola, int cd_turma);
        IEnumerable<ProfessorUI> getProfessorTurmaLogado(int cd_escola, int cd_turma, int cd_usuario);
        IEnumerable<ProfessorUI> getFuncionariosByEscola(int cdEscola, int? cdProfessor, bool? status);
        IEnumerable<ProfessorUI> getFuncionariosByEscolaAtividade(int cdEscola, int? cdProfessor, int? cd_atividade_extra, bool? status);
        IEnumerable<FuncionarioSearchUI> getProfessoresDisponiveisFaixaHorario(SearchParameters parametros, string desc, string nomeRed, bool inicio, bool? status, string cpf, int sexo, List<Horario> horariosTurma, 
            int cd_escola, int cd_turma, int cd_curso, bool PPT_pai, int cd_produto, DateTime dtInicio, DateTime? dtFinal, int cd_duracao);
        FuncionarioSGF getProfLogByCodPessoaUsuario(int cd_pessoa_usuario, int cd_pessoa_empresa);
        FuncionarioSGF getProfLogByCodPessoaUsuario(int cd_pessoa_usuario, int cd_pessoa_empresa, TransactionScopeBuilder.TransactionType TransactionType);
        IEnumerable<Professor> getProfessoresDisponiveisFaixaHorarioTurma(TurmaAlunoProfessorHorario turmaProfessorHorario, int cdEscola);
        IEnumerable<Professor> getRetProfessoresDisponiveisFaixaHorarioTurma(TurmaAlunoProfessorHorario turmaProfessorHorario, int cdEscola);
        ProfessorUI verificaRetornaSeUsuarioLogadoEProfessor(int cd_pessoa_usuario, int cd_pessoa_empresa);
        IEnumerable<ProfessorUI> getProfessorAvaliacaoTurma(int cd_pessoa_empresa);
        IEnumerable<ProfessorUI> getFuncionariosByEscolaWithAtividadeExtra(int cd_pessoa_escola, bool status);
        IEnumerable<FuncionarioSearchUI> professoresDisponiveisFaixaHorarioTurmaFilhaPPT(HorariosTurmasHorariosProfessores HorariosTurmaFilhaEPai,int cd_escola);
        IEnumerable<FuncionarioSearchUI> getProfessoresTurmaPPTPorFaixaHorariosTurmaFilha(List<Horario> horariosTurmaFilha, int cd_escola, int cd_turma_PPT);
        IEnumerable<ProfessorUI> getProfHorariosProgTurma(int cd_turma, int cd_escola, byte diaSemana, TimeSpan horaIni, TimeSpan horaFim);
        IEnumerable<FuncionarioSearchUI> getProfessoresByEmpresa(int cd_pessoa_empresa, int? cd_turma);
        bool deleteFuncionarios(List<FuncionarioSGF> funcionarios, int cd_pessoa_empresa);
        FuncionarioSearchUI getProfessoresById(int cdProf, int cd_escola);
        IEnumerable<FuncionarioSGF> getFuncionariosByAulaPers(int cdEscola);
        List<ProdutoFuncionario> getProdutoFuncionarioByFuncionario(int cdFuncionario, int cdEscola);

        //HorarioProfessorTurma
        void deleteProfessorHorario(HorarioProfessorTurma itemProf);
        HorarioProfessorTurma addProfessorHorario(HorarioProfessorTurma itemProf);
        HorarioProfessorTurma editProfessorHorario(HorarioProfessorTurma professorHorario);
        List<HorarioProfessorTurma> getHorarioProfessorByHorario(int cd_horario);
        IEnumerable<HorarioProfessorTurma> getHorarioProfessorByProfessorTurmaPPT(int cd_professor, int cd_turma_ppt);
        IEnumerable<ReportDiarioAula> getRelatorioDiarioAula(int cd_escola, int cd_turma, int cd_professor, DateTime dt_inicial, DateTime dt_final);
        IEnumerable<FuncionarioSearchUI> getSearchFuncionario(SearchParameters parametros, string nome, string apelido, bool? status, string cpf, bool inicio, byte tipo, int cdEscola, int sexo, int cdAtividade, int coordenador, int colaborador_cyber);
        PessoaFisicaSearchUI ExistFuncionarioOrPessoaFisicaByCpf(string cpf, int cdEmpresa);

        //Pagamento Professor
        IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessores(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim);
        IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessoresFaltas(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim);
        IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessoresObs(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim);

        //relatório Comissão das Secretárias
        IEnumerable<FuncionarioComissao> getRptComissaoSecretarias(int cd_professor, int cd_produto, int cd_empresa, DateTime? dt_ini, DateTime? dt_fim);

        IEnumerable<FuncionarioSearchUI> getProfessoresAulaReposicao(int cd_escola);
        IEnumerable<ProfessorUI> getFuncionariosByEscolaAulaReposicao(int cdEscola, int? cdProfessor, bool? status);
        FuncionarioCyberBdUI findFuncionarioByCdFuncionario(int cd_funcionario, int cd_empresa);
        IEnumerable<ProfsemDiarioUI> getProfessorsemDiario(SearchParameters parametros, int cd_turma, int cd_professor, int cdEscola, bool idLiberado);
        string postLiberarProfessor(int cd_professor, int cd_usuario, int fusoHorario);
    }
}
