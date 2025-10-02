using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IMatriculaDataAccess : IGenericRepository<Contrato>
    {
        IEnumerable<Contrato> getMatriculaSearch(SearchParameters parametros, string descAluno, string descTurma, bool inicio, bool semTurma,
                                                            int situacaoTurma, int nmContrato, int tipo, DateTime? dtaInicio, DateTime? dtaFim,
                                                            bool filtraMat, bool filtraDtaInicio, bool filtraDtaFim, int cdEscola, bool renegocia,
                                                            bool transf, bool retornoEsc, int? cdNomeContrato, IAditamentoDataAccess DataAccessAditamento, int nm_matricula,
                                                            int? cd_ano_escolar, int? cdContratoAnterior, byte tipoC, bool? status, int vinculado);

        IEnumerable<ContratoComboUI> getContratosSemTurmaByAlunoSearch(int cd_aluno,
            bool semTurma, int situacaoTurma, int nmContrato, int tipo, int cdEscola, byte tipoC, bool? status);
        Contrato getMatriculaById(int id, int cdEscola);
        bool deleteAll(List<Contrato> contratos);

        int getUltimoNroMatricula(int? nm_ultimo_matricula, int cd_escola);
        int getNroUltimoContrato(int? nm_ultimo_contrato, int cd_escola);
        bool getVerificarNroContrato(int cd_empresa, int nm_contrato);
        Contrato getMatriculaByTurmaAluno(int cdTurma, int cdAluno);
        bool existeMatriculaByProduto(int produto, int cdAluno, DateTime? dt_inicio_aula, int curso, bool id_turma_ppt, int cd_contrato, DateTime? dt_final_aula, int? cd_duracao);
        Contrato getContratoImpressao(int cd_escola, int cd_contrato);
        Contrato getContratoBaixa(int cd_escola, int cd_contrato);
        Contrato findMatriculaByTurmaAluno(int cdTurma, int cdAluno);
        Contrato getMatriculaByIdVI(int id, int cdEscola);
        DocumentoDigitalizadoEditUI getDocumentoDigitalizadoUpdated(int id, int cdEscola);
        List<Contrato> getMatriculasAluno(int cd_aluno, int cd_escola);
        int? getMatriculaAluno(int cd_aluno, int cd_escola);
        bool existeMatriculaByProdutoAluno(int produto, int cdAluno, int cdEscola, DateTime? dt_inicio_aula, int cd_curso, int cd_contrato, DateTime? dt_final_aula, int? cd_duracao);
        Contrato getMatriculaCnabBoleto(int cd_empresa, int cd_contrato);
        IEnumerable<MatriculaRel> getMatriculaAnalitico(int cd_empresa, int cd_turma, int cd_aluno, List<int> situacaoAlunoTurma, bool semTurma, bool tranferencia, bool retorno, int situacaoContrato, DateTime? dtIni, DateTime? dtFim, int cdAtendente, bool bolsaCem, bool contratoDigitalizado, int? cdProduto, string noProduto, bool exibirEnderecos, int vinculado);
        IEnumerable<MatriculaRel> getMatriculaPorMotivo(int cd_empresa, int cd_turma, int cd_aluno, List<int> situacaoAlunoTurma, bool semTurma, bool tranferencia, bool retorno, int situacaoContrato, DateTime? dtIni, DateTime? dtFim, int vinculado);
        DataTable getMatriculaOutros(int cd_escola, int cd_produto, DateTime? dta_ini, DateTime? dta_fim, byte qtd_max);
        Contrato getMatriculaForMovimento(int id, int cdEscola);
        byte getTpMatricula(int cdContrato, int cdEscola);
        double getBolsaMatricula(int cdContrato, int cdEscola);
        Contrato getSaldoMatricula(int cdContrato, int cdEscola, decimal pc_bolsa);
        Contrato getMatriculaByIdGeral(int id, int cdEscola);
        bool getAjusteManualMatricula(int cdContrato, int cdEscola);
        Contrato getContratoReajusteAnual(int cdContrato, int cdEscola);
        string postDeleteMatricula(Nullable<int> cd_contrato, Nullable<int> cd_usuario, Nullable<int> fuso); // sp_excluir_contrato 
        Turma getTurmaById(int cdTurma);
        IEnumerable<AlunoTurma> findAlunosTurma(int cd_turma, int[] cdAlunos);
        string postGerarVenda(Nullable<int> cd_contrato, Nullable<int> cd_curso, Nullable<int> cd_usuario, Nullable<int> fuso, bool id_futura, int cd_regime);
        string postVincularVendaMaterial(Nullable<int> cd_movimento, Nullable<int> cd_turma_origem, Nullable<int> cd_contrato);
    }
}
