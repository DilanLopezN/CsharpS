using System;
using System.Collections.Generic;
using Componentes.Utils;
using System.Data;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum
{
    using System.Data.Entity;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

    public interface IMatriculaBusiness : IGenericBusiness
    {
        //Contrato
        void sincronizarContextos(DbContext dbContext);

        IEnumerable<Contrato> getMatriculaSearch(SearchParameters parametros, string descAluno, string descTurma, bool inicio, bool semTurma,
                                                           int situacaoTurma, int nmContrato, int tipo, DateTime? dtaInicio, DateTime? dtaFim,
                                                           bool filtraMat, bool filtraDtaInicio, bool filtraDtaFim, int cdEscola, bool renegocia,
                                                            bool transf, bool retornoEsc, int? cdNomeContrato, int nm_matricula, int? cdAnoEscolar, 
                                                            int? cdContratoAnterior, byte tipoC, bool? status, int vinculado);

        List<ContratoComboUI> getContratosSemTurmaByAlunoSearch(int cd_aluno,
            bool semTurma, int situacaoTurma, int nmContrato, int tipo, int cdEscola, byte tipoC, bool? status);
        Contrato GetMatriculaById(int id, int cdEscola);
        Contrato GetMatriculaByIdPesq(int id, int cdEscola);
        Contrato PostMatricula(Contrato contrato, int cdUsuario, int fusoHorario);
        Contrato editContrato(Contrato contrato, string pathContratosEscola, int cdUsuario, int fusoHorario);
        DocumentoDigitalizadoEditUI editDocumentoDigitalizado(DocumentoDigitalizadoEditUI contrato, string pathContratosEscola);
        PacoteCertificadoUI editPacoteCertificado(PacoteCertificadoUI contrato);
        Contrato updateContrato(Contrato contrato);
        int getUltimoNroMatricula(int? nm_ultimo_matricula, int cd_escola);
        int getNroUltimoContrato(int? nm_ultimo_contrato, int cd_escola);
        Contrato getMatriculaByTurmaAluno(int cdTurma, int cdAluno);
        bool existeMatriculaByProduto(int produto, int cdAluno, DateTime? dt_inicio_aula, int curso, bool id_turma_ppt, int cd_contrato, DateTime? dt_final_aula, int cd_duracao);
        Contrato getContratoImpressao(int cd_escola, int cd_contrato);
        Contrato getContratoBaixa(int cd_escoa, int cd_contrato);
        Contrato findMatriculaByTurmaAluno(int cdTurma, int cdAluno);
        Contrato getMatriculaByIdVI(int cd_contrato, int cd_pessoa_escola);
        DocumentoDigitalizadoEditUI getDocumentoDigitalizadoUpdated(int cd_contrato, int cd_pessoa_escola);
        void saveChangesMat();
        List<Contrato> getMatriculasAluno(int cd_aluno, int cd_escola);
        Contrato getMatriculaCnabBoleto(int cd_empresa, int cd_contrato, byte tipo);
        IEnumerable<MatriculaRel> getMatriculaAnalitico(int cd_empresa, int cd_turma, int cd_aluno, List<int> situacaoAlunoTurma, bool semTurma, bool tranferencia, bool retorno, int situacaoContrato, DateTime? dtIni, DateTime? dtFim, int cdAtendente, bool bolsaCem, bool contratoDigitalizado, int? cdProduto, string noProduto, bool exibirEnderecos, int vinculado);
        IEnumerable<MatriculaRel> getMatriculaPorMotivo(int cd_empresa, int cd_turma, int cd_aluno, List<int> situacaoAlunoTurma, bool semTurma, bool tranferencia, bool retorno, int situacaoContrato, DateTime? dtIni, DateTime? dtFim, int vinculado);
        DataTable getMatriculaOutros(int cd_escola, int cd_produto, DateTime? dta_ini, DateTime? dta_fim, byte qtd_max);
        Contrato getMatriculaForMovimento(int id, int cdEscola);
        byte getTpMatricula(int cdContrato, int cdEscola);
        double getBolsaMatricula(int cdContrato, int cdEscola);
        Contrato getSaldoMatricula(int cdContrato, int cdEscola, decimal pc_bolsa);
        Contrato getMatriculaByIdGeral(int id, int cdEscola);
        bool getAjusteManualMatricula(int cdContrato, int cdEscola);

        bool getVerificarNroContrato(int cd_empresa, int nm_contrato);

        //Documento Contrato
        DescontoContrato addDocumentoContrato(DescontoContrato descontoContrato);
        IEnumerable<DescontoContrato> getDescontoContrato(int cd_contrato, int cd_pessoa_escola);
        IEnumerable<DescontoContrato> getDescontoContratoPesq(int cd_contrato, int cd_pessoa_escola);

        // Taxa Matricula
        TaxaMatricula getTaxaMatriculaByIdContrato(int cd_contrato, int cd_pessoa_escola);
        TaxaMatriculaSearchUI searchTaxaMatricula(int cd_contrato, int cd_pessoa_escola);

        //Aditamento
        Aditamento getAditamentoByContrato(int cd_contrato, int cd_escola);
        Aditamento getAditamentoByContratoEData(DateTime? data_aditamento, int cd_contrato, int cd_pessoa_escola);
        Aditamento deleteAditamentoByContrato(int cd_contrato, int cd_aditamento, int cd_escola);
        Aditamento getAditamentoByContratoMaxData(int cd_contrato, int cd_escola);
        IEnumerable<Aditamento> getAditamentosByContrato(int cd_contrato, int cd_escola);
        Aditamento getPenultimoAditamentoByContrato(int cd_contrato, int cd_escola, DateTime dataUltimoAdt);
        List<DescontoContrato> getDescontosAplicadosAditamento(int cd_contrato, int cd_escola);
        IEnumerable<DescontoContrato> getDescontosAplicadosContratoOrAditamento(int cd_contrato, int cd_escola);

        //Aluno
        int? getMatriculaAluno(int cd_aluno, int cd_escola);

        //Reajuste Anual
        Contrato getContratoReajusteAnual(int cdContrato, int cdEscola);
        bool verificaAditamentoAposReajusteAnual(int cd_empresa, int cd_reajuste_anual);
        bool deletarAditamentosReajusteAnual(int cd_empresa, int cd_reajuste_anual);
        IEnumerable<AnoEscolar> getAnoEscolaresAtivos(int? cdAnoEscolar);

        //Motivo Bolsa
        IEnumerable<MotivoBolsa> getMotivoBolsa(bool? status, int? cd_motivo_bolsa);

        string postDeleteMatricula(Nullable<int> cd_contrato, Nullable<int> cd_usuario, Nullable<int> fuso);
        List<CursoContrato> getCursoContrato(int cd_contrato);
        string postGerarVenda(Nullable<int> cd_contrato, Nullable<int> cd_curso, Nullable<int> cd_usuario, Nullable<int> fuso, bool id_futura, int cd_regime);
        string postVincularVendaMaterial(int? cd_movimento, int? cd_turma_origem, int? cd_contrato);
    }
}
