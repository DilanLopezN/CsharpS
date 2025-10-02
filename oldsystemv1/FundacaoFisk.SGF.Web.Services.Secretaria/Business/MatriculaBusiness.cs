using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
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
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAcess;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using System.Globalization;
using System;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
using System.Data.Entity;
using System.IO;
using Newtonsoft.Json;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;


namespace FundacaoFisk.SGF.Web.Services.Secretaria.Business
{

    public class MatriculaBusiness : IMatriculaBusiness
    {
        public IMatriculaDataAccess DataAccessMatricula { get; set; }
        public IFinanceiroBusiness BusinessFinanceiro { get; set; }
        public IDescontoContratoDataAccess DataAccessDescontoContrato { get; set; }
        public ITaxaMatriculaDataAccess DataAccessTaxaMatricula { get; set; }
        public IAditamentoDataAccess DataAccessAditamento { get; set; }
        public ISecretariaBusiness BusinessSecretaria { get; set; }
        public IProspectDataAccess DataAccessProspect { get; set; }
        public IAditamentoBolsaDataAccess DataAccessAditamentoBolsa { get; set; }
        public ICursoContratoDataAccess DataAccessCursoContrato { get; set; }
        public IAlunoTurmaDataAccess DataAccessAlunoTurma { get; set; }
        const string SUCESSO = "sucesso";
        public MatriculaBusiness(IMatriculaDataAccess dataAccessMatricula, IFinanceiroBusiness businessFinanceiro,
                                 IDescontoContratoDataAccess dataAccessDescontoContrato, ITaxaMatriculaDataAccess dataAccessTaxaMatricula,
                                 IAditamentoDataAccess dataAccessAditamento, ISecretariaBusiness businessSecretaria, IProspectDataAccess dataAccessProspect,
                                 IAditamentoBolsaDataAccess dataAccessAditamentoBolsa,
                                 ICursoContratoDataAccess dataAccessCursoContrato,
                                 IAlunoTurmaDataAccess dataAccessAlunoTurma)
        {
            if (dataAccessMatricula == null || businessFinanceiro == null
                || dataAccessDescontoContrato == null || dataAccessTaxaMatricula == null
                || dataAccessAditamento == null || businessSecretaria == null || dataAccessProspect == null
                || dataAccessCursoContrato == null || dataAccessAlunoTurma == null)
                throw new ArgumentNullException("repository");
            this.DataAccessMatricula = dataAccessMatricula;
            this.BusinessFinanceiro = businessFinanceiro;
            this.DataAccessDescontoContrato = dataAccessDescontoContrato;
            this.DataAccessTaxaMatricula = dataAccessTaxaMatricula;
            this.DataAccessAditamento = dataAccessAditamento;
            this.BusinessSecretaria = businessSecretaria;
            this.DataAccessProspect = dataAccessProspect;
            this.DataAccessAditamentoBolsa = dataAccessAditamentoBolsa;
            this.DataAccessCursoContrato = dataAccessCursoContrato;
            this.DataAccessAlunoTurma = dataAccessAlunoTurma;
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa)
        {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.DataAccessMatricula.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessDescontoContrato.DB()).IdUsuario = ((SGFWebContext)this.DataAccessTaxaMatricula.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessAditamento.DB()).IdUsuario = ((SGFWebContext)this.DataAccessProspect.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessAditamentoBolsa.DB()).IdUsuario = ((SGFWebContext)this.DataAccessCursoContrato.DB()).IdUsuario = 
            ((SGFWebContext)this.DataAccessAlunoTurma.DB()).IdUsuario = cdUsuario;

            ((SGFWebContext)this.DataAccessMatricula.DB()).cd_empresa =
           ((SGFWebContext)this.DataAccessDescontoContrato.DB()).cd_empresa = ((SGFWebContext)this.DataAccessTaxaMatricula.DB()).cd_empresa =
           ((SGFWebContext)this.DataAccessAditamento.DB()).cd_empresa =  ((SGFWebContext)this.DataAccessProspect.DB()).cd_empresa =
           ((SGFWebContext)this.DataAccessAditamentoBolsa.DB()).cd_empresa = ((SGFWebContext)this.DataAccessCursoContrato.DB()).cd_empresa = 
           ((SGFWebContext)this.DataAccessAlunoTurma.DB()).cd_empresa = cd_empresa;
            BusinessFinanceiro.configuraUsuario(cdUsuario, cd_empresa);
            BusinessSecretaria.configuraUsuario(cdUsuario, cd_empresa);
        }

        public void sincronizarContextos(DbContext dbContext)
        {
            //this.DataAccessMatricula.sincronizaContexto(dbContext);
            //this.DataAccessDescontoContrato.sincronizaContexto(dbContext);
            //this.DataAccessTaxaMatricula.sincronizaContexto(dbContext);
            //this.DataAccessAditamento.sincronizaContexto(dbContext);
            //this.DataAccessProspect.sincronizaContexto(dbContext);
            //BusinessFinanceiro.sincronizarContextos(dbContext);
            //BusinessSecretaria.sincronizarContextos(dbContext);
        }


        #region Matricula

        public int getUltimoNroMatricula(int? nm_ultimo_matricula, int cd_escola)
        {
            return DataAccessMatricula.getUltimoNroMatricula(nm_ultimo_matricula, cd_escola);
        }

        public bool getVerificarNroContrato(int cd_empresa, int nm_contrato)
        {
            return DataAccessMatricula.getVerificarNroContrato(cd_empresa, nm_contrato);
        }

        public int getNroUltimoContrato(int? nm_ultimo_contrato, int cd_escola)
        {
            this.sincronizarContextos(DataAccessMatricula.DB());
            return DataAccessMatricula.getNroUltimoContrato(nm_ultimo_contrato, cd_escola);
        }

        public IEnumerable<Contrato> getMatriculaSearch(SearchParameters parametros, string descAluno, string descTurma, bool inicio,
                                                     bool semTurma, int situacaoTurma, int nmContrato, int tipo, DateTime? dtaInicio,
                                                     DateTime? dtaFim, bool filtraMat, bool filtraDtaInicio, bool filtraDtaFim, int cdEscola, bool renegocia,
                                                            bool transf, bool retornoEsc, int? cdNomeContrato, int nm_matricula, int? cd_ano_escolar, 
                                                    int? cdContratoAnterior, byte tipoC, bool? status, int vinculado)
        {
            IEnumerable<Contrato> retorno = new List<Contrato>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dtMatriculaContrato", "dt_matricula_contrato");
                parametros.sort = parametros.sort.Replace("dtInicialContrato", "dt_inicial_contrato");
                parametros.sort = parametros.sort.Replace("dtFinalContrato", "dt_final_contrato");
                retorno = DataAccessMatricula.getMatriculaSearch(parametros, descAluno, descTurma, inicio, semTurma, situacaoTurma, nmContrato, tipo,
                                                           dtaInicio, dtaFim, filtraMat, filtraDtaInicio, filtraDtaFim, cdEscola, renegocia, transf, retornoEsc, 
                                                           cdNomeContrato, DataAccessAditamento, nm_matricula, cd_ano_escolar, cdContratoAnterior, tipoC, status, vinculado);
                transaction.Complete();
            }
            return retorno;
        }

        public List<ContratoComboUI> getContratosSemTurmaByAlunoSearch(int cd_aluno,
            bool semTurma, int situacaoTurma, int nmContrato, int tipo, int cdEscola, byte tipoC, bool? status)
        {
            List<ContratoComboUI> retorno = new List<ContratoComboUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessMatricula.getContratosSemTurmaByAlunoSearch(cd_aluno, semTurma, situacaoTurma, nmContrato, tipo,
                     cdEscola, tipoC, status).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public Contrato GetMatriculaById(int id, int cdEscola)
        {
            return DataAccessMatricula.getMatriculaById(id, cdEscola);

        }

        public Contrato getMatriculaByIdGeral(int id, int cdEscola)
        {
            return DataAccessMatricula.getMatriculaByIdGeral(id, cdEscola);

        }

        public Contrato GetMatriculaByIdPesq(int id, int cdEscola)
        {
            Contrato retorno = new Contrato();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessMatricula.getMatriculaById(id, cdEscola);
                transaction.Complete();
            }
            return retorno;
        }

       
//        public bool DeleteMatricula(List<Contrato> contratos, int cd_pessoa_escola)
//        {
//            bool deleted = false;
//            List<Contrato> listaDelete = new List<Contrato>();
//            sincronizarContextos(DataAccessMatricula.DB());
//            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
//            {
//                foreach (Contrato contrato in contratos)
//                {
//
//                    //Aluno turma
//                    bool alunoTurma = BusinessSecretaria.existsAlunoTurmaByContratoEscola(contrato.cd_contrato, cd_pessoa_escola);
//                    if (alunoTurma)
//                        throw new MatriculaBusinessException(String.Format(Messages.msgNotDeletedAlunoContrato), null, MatriculaBusinessException.TipoErro.ERRO_ALUNO_TURMA_CONTRATO, false);
//
//                   //cheque
//                    Cheque cheque = BusinessFinanceiro.getChequeByContrato(contrato.cd_contrato);
//                    if (cheque != null && cheque.cd_cheque > 0)
//                        BusinessFinanceiro.excluirCheque(cheque);
//
//                    //Taxa Matricula
//                    TaxaMatricula taxaMatricula = DataAccessTaxaMatricula.getTaxaMatriculaByIdContrato(contrato.cd_contrato, cd_pessoa_escola);
//                    if (taxaMatricula != null && taxaMatricula.cd_taxa_matricula > 0)
//                        DataAccessTaxaMatricula.delete(taxaMatricula, false);
//
//                    //Desconto contrato
//                    IEnumerable<DescontoContrato> descontoContratos = DataAccessDescontoContrato.getDescontoContrato(contrato.cd_contrato, cd_pessoa_escola);
//                    if (descontoContratos != null && descontoContratos.Count() > 0)
//                        foreach (var item in descontoContratos)
//                        {
//                            DescontoContrato descontoContrato = DataAccessDescontoContrato.findById(item.cd_desconto_contrato, false);
//                            if (descontoContrato != null && descontoContrato.cd_desconto_contrato > 0)
//                                DataAccessDescontoContrato.delete(descontoContrato, false);
//                        }
//
//                    //Aditamento
//                    IEnumerable<Aditamento> aditamentos = DataAccessAditamento.getAditamentosByContrato(contrato.cd_contrato, cd_pessoa_escola);
//                    if (aditamentos != null && aditamentos.ToList().Count > 0)
//                        foreach (var item in aditamentos)
//                        {
//                            Aditamento aditamento = DataAccessAditamento.findById(item.cd_aditamento, false);
//                            if (aditamento != null && aditamento.cd_aditamento > 0)
//                            {
//                                IEnumerable<DescontoContrato> descontoContratosAdt = DataAccessDescontoContrato.getDescontoAditamento(contrato.cd_contrato, aditamento.cd_aditamento, cd_pessoa_escola).ToList();
//                                if (descontoContratosAdt != null && descontoContratosAdt.Count() > 0)
//                                    foreach (DescontoContrato descAdt in descontoContratosAdt)
//                                    {
//                                        DescontoContrato descontoContrato = DataAccessDescontoContrato.findById(descAdt.cd_desconto_contrato, false);
//                                        if (descontoContrato != null && descontoContrato.cd_desconto_contrato > 0)
//                                            DataAccessDescontoContrato.deleteContext(descontoContrato, false);
//                                    }
//                                DataAccessAditamento.deleteContext(aditamento, false);
//                            }
//                        }
//
//                    Contrato contratoDel = DataAccessMatricula.findById(contrato.cd_contrato, false);
//                    if (contratoDel != null && contratoDel.cd_contrato > 0)
//                    {
//                        bool existeBolsaContrato = false;
//                        if (contratoDel.pc_desconto_bolsa > 0)
//                            existeBolsaContrato = true;
//                        BusinessFinanceiro.deleteAllTitulo(contratoDel.cd_contrato, contratoDel.cd_pessoa_escola, existeBolsaContrato);
//                        deleted = DataAccessMatricula.delete(contratoDel, false);
//                    }
//
//                }
//                DataAccessAditamento.saveChanges(false);
//                transaction.Complete();
//           }
//            return deleted;
//        }

        public string postDeleteMatricula(Nullable<int> cd_contrato, Nullable<int> cd_usuario, Nullable<int> fuso)
        {
            string retorno = null;

            //LBM Transação será controlada na procedure para evitar locks que estão ocorrendo na tabela Aluno Turma
            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            //{
                retorno = DataAccessMatricula.postDeleteMatricula(cd_contrato, cd_usuario, fuso);
            //    transaction.Complete();
            //}
            return retorno;
        }

        private void criaHistoricoAluno(Contrato contrato, int cdTurma, DateTime? dtaInicioTurma, int? situacaoAlunoTurma, int cdUsuario, int fusoHorario)
        {
            AlunoTurma existeAlunoTurma = BusinessSecretaria.findAlunoTurma(contrato.cd_aluno, cdTurma,  contrato.cd_pessoa_escola);
            if (existeAlunoTurma != null)
            {
                List<NotasVendaMaterialUI> Notas = new List<NotasVendaMaterialUI>();
                if (contrato.CursoContrato != null && contrato.CursoContrato.Count() > 0)
                    foreach (CursoContrato d in contrato.CursoContrato.Where(x=>x.cd_curso == existeAlunoTurma.Turma.Curso.cd_curso).ToList())
                    {
                        int existeNota = BusinessFinanceiro.findNotaAluno(contrato.cd_aluno, d.cd_curso);
                        //Se existir Nota já gerada em outra matrícula não gera novamente
                        if (existeNota == 0 || existeNota == 2 && (int)existeAlunoTurma.Turma.cd_regime == 2) {
                            if (contrato.notas_material_didatico != null && contrato.notas_material_didatico.Count() > 0)
                                Notas = contrato.notas_material_didatico.Where(x => x.cd_curso == d.cd_curso && !x.id_venda_futura).ToList();
                            if (Notas.Count() == 0 || existeNota == 2)
                            {

                                contrato.msgProcedureGerarNota = criaNotadeVendaMaterial(contrato.cd_contrato, d.cd_curso, cdUsuario, fusoHorario, (int)existeAlunoTurma.Turma.cd_regime);

                                if (!contrato.msgProcedureGerarNota.Contains(SUCESSO))
                                    throw new MatriculaBusinessException(
                                        contrato.msgProcedureGerarNota, null, MatriculaBusinessException.TipoErro.ERRO_PROCEDURE_GERAR_NOTA, false);
                            } 
                        }
                    }
            }
            byte segHistorico = (byte)BusinessSecretaria.retunMaxSequenciaHistoricoAluno(contrato.cd_produto_atual, contrato.cd_pessoa_escola, contrato.cd_aluno);

            DateTime dtInicioTurma = dtaInicioTurma != null ? Convert.ToDateTime(dtaInicioTurma) : new DateTime();
            DateTime dtMatriculaContrato = contrato.dt_matricula_contrato != null ? Convert.ToDateTime(contrato.dt_matricula_contrato) : new DateTime();
            DateTime dtaHistorico = dtMatriculaContrato > dtInicioTurma ? dtMatriculaContrato.Date : dtInicioTurma.Date;
            byte tipo = Convert.ToInt32(AlunoTurma.SituacaoAlunoTurma.Ativo) == Convert.ToInt32(situacaoAlunoTurma) ? (byte)HistoricoAluno.TipoMovimento.MATRICULA : (byte)HistoricoAluno.TipoMovimento.REMATRICULA;

            HistoricoAluno hist = new HistoricoAluno
            {
                cd_aluno = contrato.cd_aluno,
                cd_turma = cdTurma,
                cd_usuario = contrato.cd_usuario,
                nm_sequencia = ++segHistorico,
                cd_produto = contrato.cd_produto_atual,
                cd_contrato = contrato.cd_contrato,
                dt_historico = dtaHistorico,
                dt_cadastro = DateTime.UtcNow,
                id_tipo_movimento = tipo,
                cd_desistencia = null,
                id_desistencia = false,
                dt_saida_historico = null,
                id_situacao_historico = Convert.ToByte(situacaoAlunoTurma)
            };
            BusinessSecretaria.addHistoricoAluno(hist);
        }
        public string criaNotadeVendaMaterial(int cd_contrato, int cd_curso, int cdUsuario, int fusoHorario, int cd_regime)
        {

            string ret = postGerarVenda(cd_contrato, cd_curso, cdUsuario, fusoHorario, false, cd_regime);

            return ret;

        }

public Contrato PostMatricula(Contrato contrato, int cdUsuario, int fusoHorario)
        {
            if (contrato.id_tipo_contrato != (byte) Contrato.TipoCKMatricula.NORMAL)
            {
                List<CursoContrato> cursoContratosVinculadosTurma = (contrato.AlunoTurma != null) ? contrato.AlunoTurma.Where(x => x.CursoContrato != null).Select(y => y.CursoContrato).ToList() : new List<CursoContrato>();
                List<CursoContrato> cursosVinculadosGrid = new List<CursoContrato>();
                List<CursoContrato> cursosNaoVinculadosGrid = new List<CursoContrato>();

                foreach (CursoContrato cursoContrato in contrato.CursoContrato.ToList())
                {
                    if (!cursoContratosVinculadosTurma.Any(x => x.cd_curso == cursoContrato.cd_curso))
                    {
                        cursosNaoVinculadosGrid.Add(cursoContrato);
                    }
                    else
                    {
                        cursosVinculadosGrid.Add(cursoContrato);
                    }
                }

                if (cursoContratosVinculadosTurma.Count > 0 && cursoContratosVinculadosTurma.Count != cursosVinculadosGrid.Count)
                {
                    throw new MatriculaBusinessException(String.Format(Messages.msgErroCursosNaoVinculadosMatriculaComTurma), null, MatriculaBusinessException.TipoErro.ERRO_CURSOS_NAO_VINCULADOS_MATRICULA_COM_TURMA, false);
                }
                
                foreach (var cursoVinculadoGrid in cursosVinculadosGrid)
                {
                    
                    //remove os cursos vinculados que vieram da lista do grid
                    contrato.CursoContrato.Remove(cursoVinculadoGrid);
                }
                
            }
            

            AlunoTurma existeAlunoTurma = new AlunoTurma();
            List<AlunoTurma> alunoAguardando = new List<AlunoTurma>();
            List<DescontoContrato> descontoContrato = new List<DescontoContrato>();
            sincronizarContextos(DataAccessMatricula.DB());
            validarMinDateContrato(contrato);
            //regras para aditamento
            if (contrato.aditamentoMaxData != null)
            {
                List<Aditamento> listaAditamento = new List<Aditamento>();
                listaAditamento.Add(contrato.aditamentoMaxData);
                listaAditamento[0].dt_aditamento = DateTime.UtcNow;  // pega data e hora do sistema
                //listaAditamento[0].dt_vencto_inicial = DateTime.UtcNow;
                listaAditamento[0].cd_usuario = contrato.cd_usuario;
           
                contrato.Aditamento = listaAditamento;
            }
            if (contrato.cd_pessoa_responsavel == 0)
                contrato.cd_pessoa_responsavel = contrato.cd_pessoa_aluno;

            int countAguardando = 0;
            int countAtivo = 0;
            bool existeContratoProdAluno;
            bool existeContratoProd;
            string dc_situacao_turma = "";
            if (contrato.AlunoTurma != null)
                foreach (AlunoTurma alunoTurma in contrato.AlunoTurma)
                {
                    if (alunoTurma.Turma == null)
                    {
                        Turma turma = DataAccessMatricula.getTurmaById(alunoTurma.cd_turma);
                        if (turma != null)
                            alunoTurma.Turma = turma;
                    }
                    if (alunoTurma.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando ||
                        alunoTurma.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                        alunoTurma.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                    {
                        //LBM a mensagem deverá ser alterada para pois o Aluno já está matriculado nesse produto através de uma matrícula sem turma
                        existeContratoProd = DataAccessMatricula.existeMatriculaByProdutoAluno(alunoTurma.Turma.cd_produto, contrato.cd_aluno, contrato.cd_pessoa_escola, alunoTurma.Turma.dt_inicio_aula, (int)alunoTurma.Turma.cd_curso, contrato.cd_contrato, alunoTurma.Turma.dt_final_aula, alunoTurma.Turma.cd_duracao);
                        if (existeContratoProd) //Contrato sem turma
                            throw new MatriculaBusinessException(Utils.Messages.Messages.msgErroAlunoMatProd, null, MatriculaBusinessException.TipoErro.ERRO_EXISTE_MAT_PRODUTO, false);
                        existeContratoProdAluno = DataAccessMatricula.existeMatriculaByProduto(alunoTurma.Turma.cd_produto, contrato.cd_aluno, alunoTurma.Turma.dt_inicio_aula, (int)alunoTurma.Turma.cd_curso, alunoTurma.Turma.cd_turma_ppt != null, contrato.cd_contrato, contrato.dt_final_contrato, alunoTurma.Turma.cd_duracao);
                        if (existeContratoProdAluno)  //Contratos com turma e aluno aguardando
                            throw new MatriculaBusinessException(Utils.Messages.Messages.msgErroAlunoMatProd, null, MatriculaBusinessException.TipoErro.ERRO_EXISTE_MAT_PRODUTO, false);
                    }
                    if (alunoTurma.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando)
                    {
                        alunoTurma.dt_matricula = contrato.dt_matricula_contrato;
                        getSituacaoTurma(contrato, alunoTurma, out dc_situacao_turma);
                        alunoAguardando.Add(alunoTurma);
                        countAguardando = countAguardando + 1;
                        //if (alunoAguardando != null && alunoAguardando.Count() > 0)
                        //{
                        //    alunoAguardando[0].Turma = null; //TODO: Karol, verificar se é preciso esta linha de código e, se é preciso, tem como mudar para o Controller.
                        //}
                    }
                    else
                        countAtivo = countAtivo + 1;
                }
                    //LBM Se colocar mais de uma turma aguardando no contrato, não será permitido para matriculas normais
            if (countAguardando > 1 && contrato.id_tipo_contrato == (byte)Contrato.TipoCKMatricula.NORMAL)
                throw new MatriculaBusinessException(Utils.Messages.Messages.msgErroIncluirTurma, null, MatriculaBusinessException.TipoErro.ERRO_APENAS_UM_ALUNO_TURMA_AGUARDANDO, false);
			//Contrato sem turma 
            if (countAguardando == 0 && countAtivo == 0)
            {
                //Vai procurar se já está matriculado no periodo
                if (contrato.CursoContrato != null)
                    foreach (CursoContrato cc in contrato.CursoContrato.ToList())
                    {
                        //Vai procurar se já está matriculado no periodo
                        existeContratoProdAluno = DataAccessMatricula.existeMatriculaByProduto(contrato.cd_produto_atual, contrato.cd_aluno, contrato.dt_inicial_contrato, cc.cd_curso, contrato.cd_regime_atual == null ? false : contrato.cd_regime_atual == 2, contrato.cd_contrato, contrato.dt_final_contrato, cc.cd_duracao);
                        if (existeContratoProdAluno)  //Contratos com turma
                            throw new MatriculaBusinessException(Utils.Messages.Messages.msgErroAlunoMatProd, null, MatriculaBusinessException.TipoErro.ERRO_EXISTE_MAT_PRODUTO, false);
                        existeContratoProd = DataAccessMatricula.existeMatriculaByProdutoAluno(contrato.cd_produto_atual, contrato.cd_aluno, contrato.cd_pessoa_escola, contrato.dt_inicial_contrato, cc.cd_curso, contrato.cd_contrato, contrato.dt_final_contrato, cc.cd_duracao);
                        if (existeContratoProd) //Contrato sem Turma
                            throw new MatriculaBusinessException(Utils.Messages.Messages.msgErroAlunoMatProd, null, MatriculaBusinessException.TipoErro.ERRO_EXISTE_MAT_PRODUTO, false);
                    }
                existeContratoProd = DataAccessMatricula.existeMatriculaByProdutoAluno(contrato.cd_produto_atual, contrato.cd_aluno, contrato.cd_pessoa_escola, contrato.dt_inicial_contrato, contrato.cd_curso_atual, contrato.cd_contrato, contrato.dt_final_contrato, contrato.cd_duracao_atual);
                if (existeContratoProd) //Contrato sem Turma
                    throw new MatriculaBusinessException(Utils.Messages.Messages.msgErroAlunoMatProd, null, MatriculaBusinessException.TipoErro.ERRO_EXISTE_MAT_PRODUTO, false);
            }

            contrato.dc_situacao_turma = dc_situacao_turma == "" ? "Normal" : dc_situacao_turma;

            if (contrato.AlunoTurma != null && contrato.AlunoTurma.Count() > 0)
            {
                AlunoTurma alunoTur = contrato.AlunoTurma.FirstOrDefault();
                AlunoTurma alunoAtivo = BusinessSecretaria.findAlunoTurma(alunoTur.cd_aluno, alunoTur.cd_turma, contrato.cd_pessoa_escola);
                if (alunoAtivo != null && //!alunoAtivo.Turma.Curso.id_permitir_matricula &&
                    ((!contrato.id_retorno && alunoAtivo != null && alunoAtivo.cd_situacao_aluno_turma != (int)Turma.SituacaoAlunoTurma.AGUARDANDO) ||
                     (contrato.id_retorno && alunoAtivo != null && alunoAtivo.cd_situacao_aluno_turma != (int)Turma.SituacaoAlunoTurma.DESISTENTE)))
                    throw new MatriculaBusinessException(
                                string.Format(Utils.Messages.Messages.msgErroContratoExisteAlunoTurma, contrato.no_pessoa, contrato.no_turma), null, MatriculaBusinessException.TipoErro.ERRO_ALUNO_TURMA_CONTRATO, false);
                //LBM TODO Atenção deletar os aguardando apenas de matriculas normais, id_tipo_matricula = 0
                if ((contrato.id_retorno && (alunoAtivo != null && alunoAtivo.cd_situacao_aluno_turma != (int)Turma.SituacaoAlunoTurma.DESISTENTE)) ||
                    (!contrato.id_retorno && alunoAtivo != null && alunoAtivo.cd_situacao_aluno_turma == (int)Turma.SituacaoAlunoTurma.AGUARDANDO) ||
                    !contrato.id_retorno && alunoAtivo == null)
                {
                    BusinessSecretaria.deleteAlunoAguardandoTurma(contrato.cd_produto_atual, contrato.cd_pessoa_escola, contrato.cd_contrato, contrato.cd_aluno, alunoTur.cd_turma);
                    contrato.AlunoTurma = alunoAguardando;
                }
                else
                    contrato.AlunoTurma = null;
            }
            //Verifica os valores informados no contrato
            regraForValores(contrato, descontoContrato);

            List<TaxaMatricula> listaTaxaMatricula = contrato.TaxaMatricula != null ? contrato.TaxaMatricula.ToList() : new List<TaxaMatricula>();
            if (listaTaxaMatricula != null && listaTaxaMatricula.Count() > 0)
                listaTaxaMatricula[0].dt_vcto_taxa = listaTaxaMatricula[0].dt_vcto_taxa.ToLocalTime().Date;

            Contrato novoContrato = DataAccessMatricula.add(contrato, false);
            if (novoContrato.vl_pre_matricula > 0)
            {
                var ctxProspect = DataAccessProspect.getProspectByPessoaFisica(novoContrato.cd_pessoa_aluno);
                if (ctxProspect != null)
                    ctxProspect.vl_abatimento = contrato.vl_pre_matricula;
            }

            if (alunoAguardando != null && alunoAguardando.Count() > 0)
            {
                foreach (AlunoTurma alunoTurma in alunoAguardando)
                {
                    int[] alunos = { alunoTurma.cd_aluno };
                    existeAlunoTurma = BusinessSecretaria.findAlunosTurma(alunoTurma.cd_turma, contrato.cd_pessoa_escola, alunos).FirstOrDefault();

                    if (existeAlunoTurma != null && existeAlunoTurma.cd_aluno_turma > 0)
                    {
                        existeAlunoTurma.cd_contrato = novoContrato.cd_contrato;
                        existeAlunoTurma.cd_situacao_aluno_turma = alunoTurma.cd_situacao_aluno_turma;
                        existeAlunoTurma.dt_matricula = alunoTurma.dt_matricula;
                        existeAlunoTurma.dt_movimento = alunoTurma.dt_movimento;
                        existeAlunoTurma.dt_inicio = alunoTurma.dt_inicio;
                        existeAlunoTurma.nm_matricula_turma = alunoTurma.nm_matricula_turma;
                        if (contrato.id_tipo_contrato == (byte)Contrato.TipoCKMatricula.NORMAL)
                        {
                            var cc = contrato.CursoContrato.ToList();
                            existeAlunoTurma.cd_curso_contrato = cc[0].cd_curso_contrato;
                        }
                        DataAccessMatricula.saveChanges(false);
                    }
                    //Quanto está matriculando um aluno que não e do tipo retorono, ele nem existe na turma.
                    else if (contrato.id_retorno)
                        throw new MatriculaBusinessException(
                            string.Format(Utils.Messages.Messages.msgErroRetornoMatriculaSemAlunoTurma, contrato.no_pessoa, contrato.no_turma), null,
                            MatriculaBusinessException.TipoErro.ERRO_ALUNO_TURMA_CONTRATO, false);
                    criaHistoricoAluno(novoContrato, alunoTurma.cd_turma, alunoTurma.Turma.dt_inicio_aula, alunoTurma.cd_situacao_aluno_turma, cdUsuario, fusoHorario);
                }

            }

            return contrato;
        }

        private void regraForValores(Contrato contrato, List<DescontoContrato> descontoContrato)
        {
            decimal valorFaturar = 0;
            bool aditamento = false;
            decimal vl_material_contrato = 0;
            if (contrato != null)
            {
                if (contrato.vl_curso_contrato <= 0)
                    throw new MatriculaBusinessException(Utils.Messages.Messages.msgValorContratoMenorZero, null, MatriculaBusinessException.TipoErro.ERRO_VALOR_CONTRATO_ZERO, false);
                //calculando valor líquido do curso ou no caso de aditamento, o valor do aditivo.
                valorFaturar = contrato.vl_liquido_contrato;
                if (contrato.Aditamento != null && contrato.Aditamento.Count() > 0)
                {
                    if (contrato.Aditamento.FirstOrDefault().id_tipo_aditamento > 0)
                    {
                        valorFaturar = contrato.Aditamento.FirstOrDefault().vl_aditivo;
                        aditamento = true;
                    }
                }
                if ((bool)contrato.id_incorporar_valor_material)
                    vl_material_contrato = (decimal)contrato.vl_material_contrato;


                if (contrato.alterouTitulos && contrato.titulos != null && contrato.titulos.Count() > 0)
                {
                    decimal vl_abatimento = 0;
                    decimal vl_abatimento_taxa = 0;
                    if (contrato.vl_pre_matricula > 0)
                        vl_abatimento = contrato.vl_pre_matricula;
                    if (contrato.TaxaMatricula != null && contrato.TaxaMatricula.Count() > 0 && contrato.TaxaMatricula.ToList()[0].vl_matricula_taxa > 0)
                    {
                        vl_abatimento_taxa = vl_abatimento;
                        if (contrato.TaxaMatricula.ToList()[0].vl_matricula_taxa <= vl_abatimento)
                        {
                            vl_abatimento -= (decimal)contrato.TaxaMatricula.ToList()[0].vl_matricula_taxa;
                            vl_abatimento_taxa = (decimal)contrato.TaxaMatricula.ToList()[0].vl_matricula_taxa;
                        }
                        else
                            vl_abatimento = 0;
                    }

                     // Verificando se o somatorio dos titulos é igual ao informado na aba "Valores"
                    decimal sumTituloMensalidade = contrato.titulos.Where(tit => tit.dc_tipo_titulo == "ME" || tit.dc_tipo_titulo == "MA" || tit.dc_tipo_titulo == "MM").Sum(t => t.vl_titulo);
                    decimal sumTituloTaxa = contrato.titulos.Where(tit => tit.dc_tipo_titulo == "TM" || tit.dc_tipo_titulo == "TA").Sum(t => t.vl_titulo);
                    decimal sumTituloAdt = contrato.titulos.Where(tit => tit.dc_tipo_titulo == "AD" || tit.dc_tipo_titulo == "AA").Sum(t => t.vl_titulo);
                    decimal valorTaxaFaturar;
                    decimal vlFaturarVerifica = Decimal.Round(valorFaturar - contrato.vl_desc_primeira_parcela, 2);
                    //Não presisa somar, já está incorporado
                    //if (contrato.id_divida_primeira_parcela)
                    //    vlFaturarVerifica = vlFaturarVerifica + contrato.vl_divida_contrato;
                    if (vl_abatimento > 0)
                        vlFaturarVerifica -= vl_abatimento;
                    if (aditamento)
                    {
                        if (Decimal.Round(sumTituloMensalidade + sumTituloAdt, 2) != Decimal.Round(vlFaturarVerifica, 2))
                            throw new MatriculaBusinessException(String.Format(Messages.msgErroValorTituloAditivo), null, MatriculaBusinessException.TipoErro.ERRO_ALUNO_TURMA_CONTRATO, false);
                    }
                    else
                        if (Decimal.Round(sumTituloMensalidade, 2) != Decimal.Round(vlFaturarVerifica + vl_material_contrato, 2))
                            throw new MatriculaBusinessException(String.Format(Messages.msgErroValorTitulo), null, MatriculaBusinessException.TipoErro.ERRO_ALUNO_TURMA_CONTRATO, false);

                    List<TaxaMatricula> listaTaxa = contrato.TaxaMatricula != null ? contrato.TaxaMatricula.ToList() : null;
                    if (listaTaxa != null && listaTaxa.Count() > 0)
                    {

                        valorTaxaFaturar = listaTaxa[0].vl_matricula_taxa == null ? 0 : Convert.ToDecimal(listaTaxa[0].vl_matricula_taxa);
                        valorTaxaFaturar -= vl_abatimento_taxa;
                        if (Decimal.Round(sumTituloTaxa, 2) != Decimal.Round(valorTaxaFaturar, 2))
                            if (contrato.id_ajuste_manual)
                                throw new MatriculaBusinessException(String.Format(Messages.msgErroValorTaxaMatricula), null, MatriculaBusinessException.TipoErro.ERRO_ALUNO_TURMA_CONTRATO, false);
                    }
                }
            }

        }

        private static void getSituacaoTurma(Contrato contrato, AlunoTurma alunoTurma, out string dc_situacao_turma)
        {
            switch (contrato.id_tipo_matricula)
            {
                case 1: alunoTurma.cd_situacao_aluno_turma = (int)Turma.SituacaoAlunoTurma.ATIVO;
                    break;
                case 2: alunoTurma.cd_situacao_aluno_turma = (int)Turma.SituacaoAlunoTurma.REMATRICULADO;
                    break;
                default: alunoTurma.cd_situacao_aluno_turma = (int)Turma.SituacaoAlunoTurma.ATIVO;
                    break;
            }
            dc_situacao_turma = "Normal";
        }

        public Contrato editContrato(Contrato contrato, string pathContratosEscola, int cdUsuario, int fusoHorario)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessMatricula.DB()))
            {
                sincronizarContextos(DataAccessMatricula.DB());
                validarMinDateContrato(contrato);
                Contrato contratoContext = DataAccessMatricula.findById(contrato.cd_contrato, false);
                if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado) && (!contrato.nm_arquivo_digitalizado.Contains(contrato.nm_contrato + "__")))
                {
                    contrato.nm_arquivo_digitalizado = contrato.nm_contrato + "__" + contrato.nm_arquivo_digitalizado;
                }
                string old_nm_arquivo_digitalizado = contratoContext.nm_arquivo_digitalizado;
                List<CursoContrato> cursoContratoContext = DataAccessCursoContrato.getCursosContratoByCdContrato(contrato.cd_contrato);
                if (contrato.CursoContrato != null)
                    crudCursosContrato(contrato, contrato.CursoContrato.ToList(), cursoContratoContext.ToList());
                
                //Desconto contrato
                List<DescontoContrato> descontoContrato = contrato.DescontoContrato != null ? contrato.DescontoContrato.ToList() : new List<DescontoContrato>();
                List<TaxaMatricula> listaTaxa = contrato.TaxaMatricula != null && contrato.TaxaMatricula.Count() > 0 ? contrato.TaxaMatricula.ToList() : new List<TaxaMatricula>();
                //Taxa de Matrícula
                TaxaMatricula taxaContext = getTaxaMatriculaByIdContrato(contrato.cd_contrato, contrato.cd_pessoa_escola);
                //Se existia taxa de matrícula, e tirar ou zerar a taxa de matrícula na view, apagar a taxa de matrícula
                if (taxaContext != null && taxaContext.cd_taxa_matricula > 0 &&
                    (listaTaxa.Count() <= 0 || (listaTaxa.Count() > 0 && (!listaTaxa[0].vl_matricula_taxa.HasValue || listaTaxa[0].vl_matricula_taxa.Value == 0))))
                    DataAccessTaxaMatricula.delete(taxaContext, false);

                //Cheque
                Cheque cheque = new Cheque();
                if (contrato.cd_tipo_financeiro == 4 || (listaTaxa.Count() > 0 && listaTaxa[0].cd_tipo_financeiro_taxa == 4))
                {
                    if (contrato.Cheque != null)
                    {
                        cheque = BusinessFinanceiro.getChequeByContrato(contrato.cd_contrato);
                        if (cheque != null && cheque.cd_cheque > 0)
                        {
                            cheque.copy(contrato.Cheque.ToList()[0]);
                            BusinessFinanceiro.editCheque(cheque);
                        }
                        else
                            BusinessFinanceiro.addCheque(contrato.Cheque.ToList()[0]);
                    }
                }
                else
                {
                    cheque = BusinessFinanceiro.getChequeByContrato(contrato.cd_contrato);
                    if (cheque != null && cheque.cd_cheque > 0)
                        BusinessFinanceiro.deleteCheque(cheque);
                }
                contrato.Cheque = null;

                //Chamada título.
                if (contrato.titulos != null)
                {
                    if (contrato.alterou_responsavel)
                    {
                        List<Titulo> titulosAlterouResponsavel = contrato.titulos.Where(x => x.alterou_responsavel_titulo).ToList();
                        if(titulosAlterouResponsavel != null && titulosAlterouResponsavel.Count > 0)
                        {
                            alterarResponsavelTitulos(contrato.cd_contrato, contrato.cd_pessoa_escola, titulosAlterouResponsavel);
                        }
                        
                    }
                    if (contrato.alterou_dt_vcto)
                    {
                        List<Titulo> titulosAlterouDtVcto = contrato.titulos.Where(x => x.alterou_dt_vcto_titulo).ToList();
                        if (titulosAlterouDtVcto != null && titulosAlterouDtVcto.Count > 0)
                        {
                            alterarDtVctoTitulos(contrato.cd_contrato, contrato.cd_pessoa_escola, titulosAlterouDtVcto);
                        }

                    }
                    else
                    {
                        crudTitulosContrato(contrato, contratoContext);
                    }

                }
                    

                //Taxa Matricula
                if (contrato != null)
                    editTaxaMatriculaContrato(contrato);

                AlunoTurma existeAlunoTurma = new AlunoTurma();
                List<AlunoTurma> alunoAguardando = new List<AlunoTurma>();
                int countAguardando = 0;
                int countAtivo = 0;
                string dc_situacao_turma = "";
                bool existeContratoProdAluno;
                bool existeContratoProd;
                if (contrato.AlunoTurma != null)
                    foreach (AlunoTurma alunoTurma in contrato.AlunoTurma)
                    {
                        if (alunoTurma.Turma == null)
                        {
                            Turma turma = DataAccessMatricula.getTurmaById(alunoTurma.cd_turma);
                            if (turma != null)
                                alunoTurma.Turma = turma;
                        }
                        if ((alunoTurma.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando ||
                            alunoTurma.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                            alunoTurma.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                            Contrato.testchangeValuesContrato(contratoContext, contrato))
                        {
                            //LBM a mensagem deverá ser alterada para pois o Aluno já está matriculado nesse produto através de uma matrícula sem turma
                            existeContratoProd = DataAccessMatricula.existeMatriculaByProdutoAluno(alunoTurma.Turma.cd_produto, contrato.cd_aluno, contrato.cd_pessoa_escola, alunoTurma.Turma.dt_inicio_aula, (int)alunoTurma.Turma.cd_curso, contrato.cd_contrato, alunoTurma.Turma.dt_final_aula, alunoTurma.Turma.cd_duracao);
                            if (existeContratoProd) //Contrato sem turma
                                throw new MatriculaBusinessException(Utils.Messages.Messages.msgErroAlunoMatProd, null, MatriculaBusinessException.TipoErro.ERRO_EXISTE_MAT_PRODUTO, false);
                            existeContratoProdAluno = DataAccessMatricula.existeMatriculaByProduto(alunoTurma.Turma.cd_produto, contrato.cd_aluno, alunoTurma.Turma.dt_inicio_aula, (int)alunoTurma.Turma.cd_curso, alunoTurma.Turma.cd_turma_ppt != null, contrato.cd_contrato, contrato.dt_final_contrato, alunoTurma.Turma.cd_duracao);
                            if (existeContratoProdAluno)  //Contratos com turma e aluno aguardando
                                throw new MatriculaBusinessException(Utils.Messages.Messages.msgErroAlunoMatProd, null, MatriculaBusinessException.TipoErro.ERRO_EXISTE_MAT_PRODUTO, false);
                        }
                        alunoTurma.nm_matricula_turma = contrato.nm_matricula_contrato;
                        if (alunoTurma.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando)
                        {

                            alunoTurma.dt_matricula = contrato.dt_matricula_contrato;
                            getSituacaoTurma(contrato, alunoTurma, out dc_situacao_turma);
                            alunoTurma.cd_situacao_aluno_turma = contrato.id_tipo_matricula;
                            countAguardando = countAguardando + 1;

                            int[] alunos = { alunoTurma.cd_aluno };
                            existeAlunoTurma = DataAccessMatricula.findAlunosTurma(alunoTurma.cd_turma, alunos).FirstOrDefault();

                            if (existeAlunoTurma != null && existeAlunoTurma.cd_aluno > 0)
                            {
                                BusinessSecretaria.deletarAlunoTurma(existeAlunoTurma);
                                BusinessSecretaria.deleteAlunoAguardandoTurma(alunoTurma.Turma.cd_produto, contrato.cd_pessoa_escola, contrato.cd_contrato, contrato.cd_aluno, existeAlunoTurma.cd_turma);
                                //LBM o procedimento SalvarMatricula abaixo vai gravar a aluno turma e historico
                                //getSituacaoTurma(contrato, alunoTurma, out dc_situacao_turma);
                                //BusinessSecretaria.addAlunoTurma(alunoTurma);
                                //existeAlunoTurma = BusinessSecretaria.findAlunoTurma(contrato.cd_aluno, alunoTurma.Turma.cd_turma, contrato.cd_pessoa_escola);
                                //if (existeAlunoTurma != null && existeAlunoTurma.Turma != null)
                                //    criaHistoricoAluno(contrato, existeAlunoTurma.cd_turma, existeAlunoTurma.Turma.dt_inicio_aula, alunoTurma.cd_situacao_aluno_turma);
                            }
                            //else eliminado para gravar e salvar aluno turma e historico na salvarMatricula abaixo
                                if (contrato != null)
                                    alunoAguardando.Add(alunoTurma);
                        }
                        else
                        {
                            countAtivo = countAtivo + 1;
                            HistoricoAluno historico = BusinessSecretaria.getHistoricoAlunoByMatricula(contrato.cd_pessoa_escola, contrato.cd_aluno, alunoTurma.cd_turma, contrato.cd_contrato);
                            int[] alunos = { contrato.cd_aluno };
                            existeAlunoTurma = BusinessSecretaria.findAlunosTurmaHist(alunoTurma.cd_turma, contrato.cd_pessoa_escola, alunos).FirstOrDefault();
                            if (existeAlunoTurma != null && existeAlunoTurma.Turma != null && existeAlunoTurma.cd_turma > 0 && historico != null && historico.cd_historico_aluno > 0)
                            {
                                historico.dt_historico = contrato.dt_matricula_contrato > existeAlunoTurma.Turma.dt_inicio_aula ? Convert.ToDateTime(contrato.dt_matricula_contrato).Date : Convert.ToDateTime(existeAlunoTurma.Turma.dt_inicio_aula).Date;
                                if (contrato.dt_inicial_contrato > historico.dt_historico)
                                    historico.dt_historico = Convert.ToDateTime(contrato.dt_inicial_contrato).Date;
                                historico.id_situacao_historico = Convert.ToInt32(Contrato.TipoMatricula.MATRICULA) == contrato.id_tipo_matricula ? (byte)HistoricoAluno.SituacaoHistorico.ATIVO : (byte)HistoricoAluno.SituacaoHistorico.REMATRICULADO;
                                historico.id_tipo_movimento = Convert.ToInt32(Contrato.TipoMatricula.MATRICULA) == contrato.id_tipo_matricula ? (byte)HistoricoAluno.TipoMovimento.MATRICULA : (byte)HistoricoAluno.TipoMovimento.REMATRICULA;
                                BusinessSecretaria.saveHistoricoAluno();
                            }
                            if (alunoTurma.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo || alunoTurma.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                            {
                                if (existeAlunoTurma != null && existeAlunoTurma.cd_turma > 0)
                                {
                                    AlunoTurma alunoAltera = BusinessSecretaria.findAlunoTurmaById(existeAlunoTurma.cd_aluno_turma);
                                    getSituacaoTurma(contrato, alunoAltera, out dc_situacao_turma);
                                    //DataAccessMatricula.saveChanges(false);
                                    alunoAltera.dt_matricula = contrato.dt_matricula_contrato > alunoAltera.Turma.dt_inicio_aula ? Convert.ToDateTime(contrato.dt_matricula_contrato).Date : Convert.ToDateTime(alunoAltera.Turma.dt_inicio_aula).Date;
                                    DataAccessAlunoTurma.edit(alunoAltera, false);
                                }
                                contrato.dc_situacao_turma = dc_situacao_turma;
                                if (historico == null || historico.cd_historico_aluno <= 0)
                                {
                                    AlunoTurma alunoTurmaContext = BusinessSecretaria.findAlunoTurma(contrato.cd_aluno, alunoTurma.cd_turma, contrato.cd_pessoa_escola);
                                    if (alunoTurmaContext != null && alunoTurmaContext.Turma != null)
                                        criaHistoricoAluno(contrato, alunoTurma.cd_turma, alunoTurmaContext.Turma.dt_inicio_aula, alunoTurma.cd_situacao_aluno_turma, cdUsuario, fusoHorario);
                                }
                            }
                        }
                    }
                //LBM Se colocar mais de uma turma aguardando no contrato, não será permitido para matriculas normais
                if (countAguardando > 1 && contrato.id_tipo_contrato == (byte) Contrato.TipoCKMatricula.NORMAL)
                    throw new MatriculaBusinessException(Utils.Messages.Messages.msgErroIncluirTurma, null, MatriculaBusinessException.TipoErro.ERRO_APENAS_UM_ALUNO_TURMA_AGUARDANDO, false);
                //Contrato sem turma
				if (countAguardando == 0 && countAtivo == 0)
                {
                    if (contrato.CursoContrato != null && contrato.CursoContrato.Count() > 0 && Contrato.testchangeValuesContrato(contratoContext, contrato))
                        foreach (CursoContrato cc in contrato.CursoContrato)
                        {
                            //Vai procurar se já está matriculado no periodo
                            existeContratoProdAluno = DataAccessMatricula.existeMatriculaByProduto(contrato.cd_produto_atual, contrato.cd_aluno, contrato.dt_inicial_contrato, cc.cd_curso, contrato.cd_regime_atual == null ? false : contrato.cd_regime_atual == 2, contrato.cd_contrato, contrato.dt_final_contrato, cc.cd_duracao);
                            if (existeContratoProdAluno)
                                throw new MatriculaBusinessException(Utils.Messages.Messages.msgErroAlunoMatProd, null, MatriculaBusinessException.TipoErro.ERRO_EXISTE_MAT_PRODUTO, false);
                            existeContratoProd = DataAccessMatricula.existeMatriculaByProdutoAluno(contrato.cd_produto_atual, contrato.cd_aluno, contrato.cd_pessoa_escola, contrato.dt_inicial_contrato, cc.cd_curso, contrato.cd_contrato, contrato.dt_final_contrato, cc.cd_duracao);
                            if (existeContratoProd) //Contrato sem turma
                                throw new MatriculaBusinessException(Utils.Messages.Messages.msgErroAlunoMatProd, null, MatriculaBusinessException.TipoErro.ERRO_EXISTE_MAT_PRODUTO, false);
                        }
                    existeContratoProd = Contrato.testchangeValuesContrato(contratoContext, contrato) &&
                        DataAccessMatricula.existeMatriculaByProdutoAluno(contrato.cd_produto_atual, contrato.cd_aluno, contrato.cd_pessoa_escola, contrato.dt_inicial_contrato, contrato.cd_curso_atual, contrato.cd_contrato, contrato.dt_final_contrato, contrato.cd_duracao_atual);
                    if (existeContratoProd) //Contrato sem turma
                        throw new MatriculaBusinessException(Utils.Messages.Messages.msgErroAlunoMatProd, null, MatriculaBusinessException.TipoErro.ERRO_EXISTE_MAT_PRODUTO, false);
                }
                if (countAguardando > 0)
                               salvarAlunoTurma(contrato, alunoAguardando, cdUsuario, fusoHorario);
                //Desconto Contrato
                if (contrato != null)
                {
                    persistirDescontoContrato(contrato);
                    contrato.DescontoContrato = null;
                }

                //Aditamento
                if (contrato.aditamentoMaxData != null)
                    persitirAditamento(contrato);

                // Titulos Aditamento LBM Obter só os que tem saldo
                BusinessFinanceiro.adicionaTituloAditamento(contrato.titulos.Where(t => t.vl_saldo_titulo > 0).ToList(), contrato);

                //regra para valores 
                //foi comentando pois já exise esta verificação nas regras de geração dos títulos.
                //regraForValores(contrato, descontoContrato);
                contrato.titulos = null;
                contrato.TaxaMatricula = null;
                contrato.AlunoTurma = null;
                Contrato.changeValuesContrato(contratoContext, contrato);
                if (contrato.vl_pre_matricula != contratoContext.vl_pre_matricula)
                {
                    var ctxProspect = DataAccessProspect.getProspectByPessoaFisica(contratoContext.cd_pessoa_aluno);
                    if (ctxProspect != null)
                        ctxProspect.vl_abatimento = contrato.vl_pre_matricula;
                }

                
                DataAccessMatricula.saveChanges(false);
                SalvarArquivoContratoDigitalizado(old_nm_arquivo_digitalizado, contratoContext, contrato, pathContratosEscola);
                //Contrato novoContrato = DataAccessMatricula.edit(contrato, false);

                transaction.Complete();
            }
            return contrato;
        }

        public DocumentoDigitalizadoEditUI editDocumentoDigitalizado(DocumentoDigitalizadoEditUI contrato, string pathContratosEscola)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessMatricula.DB()))
            {
                sincronizarContextos(DataAccessMatricula.DB());
                Contrato contratoContext = DataAccessMatricula.findById(contrato.cd_contrato, false);
                if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado) && (!contrato.nm_arquivo_digitalizado.Contains(contrato.nm_contrato + "__")))
                {
                    contrato.nm_arquivo_digitalizado = contrato.nm_contrato + "__" + contrato.nm_arquivo_digitalizado;
                }
                string old_nm_arquivo_digitalizado = contratoContext.nm_arquivo_digitalizado;

                Contrato.changeValuesDocumentoDigitalizado(contratoContext, contrato);

                DataAccessMatricula.saveChanges(false);
                SalvarArquivoDocumentoDigitalizado(old_nm_arquivo_digitalizado, contratoContext, contrato, pathContratosEscola);
                //Contrato novoContrato = DataAccessMatricula.edit(contrato, false);

                transaction.Complete();
            }
            return contrato;
        }

        public PacoteCertificadoUI editPacoteCertificado(PacoteCertificadoUI contrato)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessMatricula.DB()))
            {
                sincronizarContextos(DataAccessMatricula.DB());
                Contrato contratoContext = DataAccessMatricula.findById(contrato.cd_contrato, false);

                Contrato.changeValuesPacoteCertificado(contratoContext, contrato);

                DataAccessMatricula.saveChanges(false);

                if (contrato.CursoContrato != null)
                {
                    foreach (var cursocontratoview in contrato.CursoContrato)
                    {
                        CursoContrato cursocontratoBD = DataAccessCursoContrato.findById(cursocontratoview.cd_curso_contrato, false);
                        cursocontratoBD.id_liberar_certificado = cursocontratoview.id_liberar_certificado;
                        DataAccessCursoContrato.saveChanges(false);
                    }
                }

                transaction.Complete();
            }
            return contrato;
        }

        private void SalvarArquivoDocumentoDigitalizado(string old_nm_arquivo_digitalizado, Contrato contratoContext, DocumentoDigitalizadoEditUI contrato, string pathContratosEscola)
        {

            string documentoContratoTemp = "";
            try
            {
                HashSet<char> invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());
                HashSet<char> invalidPathChars = new HashSet<char>(Path.GetInvalidPathChars());
                if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado_temporario) && !String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado))
                {
                    string documentoContrato = "";
                    string pathDocAntigo = "";


                    pathDocAntigo = pathContratosEscola + "/" + contrato.cd_pessoa_escola + "/" + old_nm_arquivo_digitalizado;
                    documentoContrato = pathContratosEscola + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado;
                    documentoContratoTemp = pathContratosEscola + "Temp" + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado_temporario;

                    if (contrato.nm_arquivo_digitalizado.Any(c => invalidFileNameChars.Contains(c)))
                        throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosNomeArquivo + contrato.nm_arquivo_digitalizado, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
                    if (pathContratosEscola.Any(c => invalidPathChars.Contains(c)))
                        throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosPathArquivo + contrato.nm_arquivo_digitalizado, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);

                    if (System.IO.File.Exists(documentoContrato))
                    {
                        //if (!String.IsNullOrEmpty(old_nm_arquivo_digitalizado) && contrato.nm_arquivo_digitalizado != old_nm_arquivo_digitalizado)
                        //  throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroArquivoDigitalizadoFound, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
                        //else
                        if (System.IO.File.Exists(pathDocAntigo))
                            System.IO.File.Delete(pathDocAntigo);
                    }

                    DirectoryInfo di = new DirectoryInfo(pathContratosEscola + "/" + contrato.cd_pessoa_escola + "/");
                    if (!di.Exists)
                        di.Create();

                    if (System.IO.File.Exists(pathDocAntigo))
                        System.IO.File.Delete(pathDocAntigo);

                    System.IO.File.Move(documentoContratoTemp, documentoContrato);
                    if (System.IO.File.Exists(documentoContratoTemp))
                        System.IO.File.Delete(documentoContratoTemp);


                }

            }
            catch (Exception exe)
            {
                documentoContratoTemp = pathContratosEscola + "Temp" + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado_temporario;

                if (System.IO.File.Exists(documentoContratoTemp))
                    System.IO.File.Delete(documentoContratoTemp);
                throw exe;
            }

        }

        private void SalvarArquivoContratoDigitalizado(string old_nm_arquivo_digitalizado, Contrato contratoContext, Contrato contrato, string pathContratosEscola)
        {

            string documentoContratoTemp = "";
            bool masterGeral = BusinessSecretaria.verificarMasterGeral(contrato.cd_usuario);
            try
            {
                HashSet<char> invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());
                HashSet<char> invalidPathChars = new HashSet<char>(Path.GetInvalidPathChars());
                if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado_temporario) && !String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado))
                {
                    string documentoContrato = "";
                    string pathDocAntigo = "";

                    
                    pathDocAntigo = pathContratosEscola + "/" + contrato.cd_pessoa_escola + "/" + old_nm_arquivo_digitalizado;
                    documentoContrato = pathContratosEscola + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado;
                    documentoContratoTemp = pathContratosEscola + "Temp" + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado_temporario;

                    if (contrato.nm_arquivo_digitalizado.Any(c => invalidFileNameChars.Contains(c)))
                        throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosNomeArquivo + contrato.nm_arquivo_digitalizado, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
                    if (pathContratosEscola.Any(c => invalidPathChars.Contains(c)))
                        throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosPathArquivo + contrato.nm_arquivo_digitalizado, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);

                    if (System.IO.File.Exists(documentoContrato))
                    {
                        //if (!String.IsNullOrEmpty(old_nm_arquivo_digitalizado) && contrato.nm_arquivo_digitalizado != old_nm_arquivo_digitalizado)
                          //  throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroArquivoDigitalizadoFound, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
                        //else
                            if (System.IO.File.Exists(pathDocAntigo))
                                System.IO.File.Delete(pathDocAntigo);
                    }

                    DirectoryInfo di = new DirectoryInfo(pathContratosEscola + "/" + contrato.cd_pessoa_escola + "/");
                    if (!di.Exists)
                        di.Create();

                    if (System.IO.File.Exists(pathDocAntigo))
                        System.IO.File.Delete(pathDocAntigo);

                    System.IO.File.Move(documentoContratoTemp, documentoContrato);
                    if (System.IO.File.Exists(documentoContratoTemp))
                        System.IO.File.Delete(documentoContratoTemp);

                    
                }

            }
            catch (Exception exe)
            {
                documentoContratoTemp = pathContratosEscola + "Temp" + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado_temporario;

                if (System.IO.File.Exists(documentoContratoTemp))
                    System.IO.File.Delete(documentoContratoTemp);
                throw exe;
            }

        }


        public void crudCursosContrato(Contrato contrato,List<CursoContrato> cursosContratoView, List<CursoContrato> cursosContratoContext)
        {
            List<CursoContrato> cursosAdd = new List<CursoContrato>();
            List<CursoContrato> cursosDel = new List<CursoContrato>();
            List<CursoContrato> cursosEdit = new List<CursoContrato>();
            if (contrato.id_tipo_contrato == (byte)Contrato.TipoCKMatricula.NORMAL && cursosContratoContext.Count() > 0)
                cursosContratoView[0].cd_curso_contrato = cursosContratoContext[0].cd_curso_contrato;
            foreach (var cursoContratoView in cursosContratoView)
            {
                if (cursoContratoView.cd_curso_contrato == 0)
                {
                    cursosAdd.Add(cursoContratoView);
                    contrato.CursoContrato.Remove(cursoContratoView);
                    foreach (var d in cursosAdd)
                    {
                        d.cd_contrato = contrato.cd_contrato;
                        DataAccessCursoContrato.addContext(d, false);
                    }
                }
                
            }

            foreach (var cursoContratoContext in cursosContratoContext)
            {
                CursoContrato curso = cursosContratoView.Where(x => x.cd_curso_contrato == cursoContratoContext.cd_curso_contrato).FirstOrDefault();
                if (curso == null)
                {
                    cursosDel.Add(cursoContratoContext);
                }
                else
                {
                    cursosEdit.Add(curso);
                }
            }

            foreach (var cursoDel in cursosDel)
            {
                //desfaz o vinculo
                AlunoTurma alunoTurmaContext = DataAccessAlunoTurma.findAlunoTurmaByCdCursoContrato(cursoDel.cd_curso_contrato, contrato.cd_pessoa_escola);
                if (alunoTurmaContext != null)
                {
                    alunoTurmaContext.cd_curso_contrato = null;
                    DataAccessAlunoTurma.saveChanges(false);  
                }
                
                //deleta o cursoContrato
                CursoContrato cursoContratoContextDel = DataAccessCursoContrato.getCursoContratoById(cursoDel.cd_curso_contrato);
                if (cursoContratoContextDel != null)
                {
                    DataAccessCursoContrato.delete(cursoContratoContextDel, false);
                    
                }
                
            }

            foreach (var cursoEdit in cursosEdit)
            {
                CursoContrato cursoContratoContextEdit = DataAccessCursoContrato.getCursoContratoById(cursoEdit.cd_curso_contrato);
                CursoContrato.changeValuesCursoContrato(cursoContratoContextEdit, cursoEdit);
            }

            DataAccessCursoContrato.saveChanges(false);
        }

        public void alterarResponsavelTitulos(int cd_contrato, int cd_escola, List<Titulo> titulos)
        {
            BusinessFinanceiro.alterarResponsavelTitulos(cd_contrato, cd_escola, titulos);
        }

        public void alterarDtVctoTitulos(int cd_contrato, int cd_escola, List<Titulo> titulos)
        {
            BusinessFinanceiro.alterarDtVctoTitulos(cd_contrato, cd_escola, titulos);
        }

        public void crudTitulosContrato(Contrato contratoView, Contrato contratoOldContext)
        {
            //TITULO
            bool aditamento = false;
            Decimal vl_material_contrato = 0;
            byte? id_tipo_aditamento = null;
            double pc_desconto_bolsa = 0;
            pc_desconto_bolsa = contratoOldContext.pc_desconto_bolsa;
            if (contratoView.vl_curso_contrato <= 0)
                throw new MatriculaBusinessException(Utils.Messages.Messages.msgValorContratoMenorZero, null, MatriculaBusinessException.TipoErro.ERRO_VALOR_CONTRATO_ZERO, false);
            if (contratoView.titulos.Count() > 0 || contratoView.alterouTitulos)
            {
                decimal valorFaturar = contratoView.vl_liquido_contrato;
                if (contratoView.aditamentoMaxData != null)
                {
                    if (contratoView.aditamentoMaxData.id_tipo_aditamento > 0)
                    {
                        valorFaturar = contratoView.aditamentoMaxData.vl_aditivo;
                        aditamento = true;
                        id_tipo_aditamento = contratoView.aditamentoMaxData.id_tipo_aditamento;
                        if (id_tipo_aditamento != null && id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADITIVO_BOLSA &&
                           contratoView.aditamentoMaxData.AditamentoBolsa != null && contratoView.aditamentoMaxData.AditamentoBolsa.Count() > 0)
                            pc_desconto_bolsa = contratoView.aditamentoMaxData.AditamentoBolsa.FirstOrDefault().pc_bolsa;
                        if (id_tipo_aditamento != null && id_tipo_aditamento != (int)Aditamento.TipoAditamento.ADITIVO_BOLSA &&
                            contratoView.aditamentoMaxData.pc_bolsa_anterior_aditamento > 0)
                            pc_desconto_bolsa = contratoView.aditamentoMaxData.pc_bolsa_anterior_aditamento;
                    }
                    else if ((bool)contratoView.id_incorporar_valor_material)
                        vl_material_contrato = (decimal)contratoView.vl_material_contrato;
                }
                if (contratoView.titulos != null)
                {
                    List<Titulo> listaTituloView = contratoView.titulos.ToList();
                    VerificaDelecaoTitulosMatricula(contratoView, aditamento, contratoOldContext, pc_desconto_bolsa);

                    //Atualizando o percentual do desconto de bolsa do aluno, através do valor que vier da view.
                    if (contratoView.aditamentoMaxData != null && !(contratoView.aditamentoMaxData.id_tipo_aditamento > 0) || contratoView.pc_desconto_bolsa != pc_desconto_bolsa)
                        pc_desconto_bolsa = contratoView.pc_desconto_bolsa;

                    if (contratoView.titulos.Count() > 0)
                    {
                        //Configurando valores de abatimento na somatório do valor da taxa.
                        decimal vl_abatimento = 0;
                        decimal vl_abatimento_taxa = 0;
                        if (contratoView.vl_pre_matricula > 0)// && !contratoView.id_ajuste_manual)
                        {
                            vl_abatimento = contratoView.vl_pre_matricula;
                            if (contratoView.TaxaMatricula != null && contratoView.TaxaMatricula.Count() > 0 && contratoView.TaxaMatricula.ToList()[0].vl_matricula_taxa > 0)
                            {
                                vl_abatimento_taxa = vl_abatimento;
                                if (contratoView.TaxaMatricula.ToList()[0].vl_matricula_taxa <= vl_abatimento)
                                {
                                    vl_abatimento -= (decimal)contratoView.TaxaMatricula.ToList()[0].vl_matricula_taxa;
                                    vl_abatimento_taxa = (decimal)contratoView.TaxaMatricula.ToList()[0].vl_matricula_taxa;
                                    //Quando for ajuste manual, abater somente a taxa, já que não se pode lançar taxa manual.
                                    if (contratoView.id_ajuste_manual)
                                        vl_abatimento = 0;
                                }
                                else
                                    vl_abatimento = 0;
                            }
                        }
                        //Verificar somatorias títulos
                        if (vl_material_contrato <= 0 && listaTituloView.Where(x => x.vl_material_titulo > 0).Any())
                            vl_material_contrato = (decimal)contratoView.vl_material_contrato;
                        //No anco de dadods os valores podem ainda não estão lá        
                        //DataAccessAditamento.getValorMaterialImpressaoContrato(contratoView.cd_contrato, contratoView.cd_pessoa_escola);
                        decimal sumTituloMensalidade = listaTituloView.Where(tit => tit.dc_tipo_titulo == "ME" || tit.dc_tipo_titulo == "MA" || tit.dc_tipo_titulo == "MM").Sum(t => t.vl_titulo);
                        decimal sumTituloTaxa = listaTituloView.Where(tit => tit.dc_tipo_titulo == "TM" || tit.dc_tipo_titulo == "TA").Sum(t => t.vl_titulo);
                        decimal sumTituloAdt = listaTituloView.Where(tit => tit.dc_tipo_titulo == "AD" || tit.dc_tipo_titulo == "AA").Sum(t => t.vl_titulo);
                        decimal valorTaxaFaturar;
                        decimal vlFaturarVerifica = Decimal.Round(valorFaturar - contratoView.vl_desc_primeira_parcela, 2);
                        if (aditamento)
                        {
                            sumTituloMensalidade = listaTituloView.Where(tit => tit.dc_tipo_titulo == "ME" || tit.dc_tipo_titulo == "MA" || tit.dc_tipo_titulo == "MM").Sum(t => t.vl_saldo_titulo);
                            sumTituloTaxa = listaTituloView.Where(tit => tit.dc_tipo_titulo == "TM" || tit.dc_tipo_titulo == "TA").Sum(t => t.vl_saldo_titulo);
                            sumTituloAdt = listaTituloView.Where(tit => tit.dc_tipo_titulo == "AD" || tit.dc_tipo_titulo == "AA").Sum(t => t.vl_saldo_titulo);

                            bool aplicarSomaTitulosPagos = true;
                            Aditamento newAditamento = contratoView.aditamentoMaxData;
                            Aditamento aditamentoMaxContext = DataAccessAditamento.getAditamentoByContrato(newAditamento.cd_aditamento, contratoOldContext.cd_contrato, contratoOldContext.cd_pessoa_escola);
                            int[] statusCnabTitulo = new int[] { (int)Titulo.StatusCnabTitulo.INICIAL, (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA };
                            listaTituloView = Titulo.revisarSaldoTituloBolsaContrato(listaTituloView, false, pc_desconto_bolsa,false);
                            decimal sumTituloMensalidadeEAditmaentoPago = listaTituloView.Where(x => (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "MA" || 
                                                                                                      x.dc_tipo_titulo == "MM" || x.dc_tipo_titulo == "AD" || 
                                                                                                      x.dc_tipo_titulo == "AA"
                                                                                                      ) &&
                                                                                                      ((x.vl_titulo != x.vl_saldo_titulo) || 
                                                                                                        !statusCnabTitulo.Contains(x.id_status_cnab)
                                                                                                       ) && 
                                                                                                       x.cd_titulo > 0).Sum(x => x.vl_saldo_titulo);
                            if (listaTituloView.Where(x => x.vl_material_titulo > 0 && x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                                                           ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab))).Any())
                            {
                                decimal vlTotalMaterialFechados = listaTituloView.Where(x => x.vl_material_titulo > 0 && x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                                                           ((x.vl_titulo != x.vl_saldo_titulo) || !statusCnabTitulo.Contains(x.id_status_cnab))).Sum(x => x.vl_material_titulo);
                                vl_material_contrato -= vlTotalMaterialFechados;
                            }
                            //Quando não alterar nada no aditamento e 
                            if (((!contratoOldContext.id_ajuste_manual && contratoView.id_ajuste_manual) || (contratoOldContext.id_ajuste_manual && !contratoView.id_ajuste_manual))
                                && !Aditamento.comparaAlterouAditamento(contratoView.aditamentoMaxData, aditamentoMaxContext))
                                aplicarSomaTitulosPagos = false;

                            var vlTotalAditamento = (sumTituloMensalidade + sumTituloTaxa + sumTituloAdt);
                            if (aplicarSomaTitulosPagos)
                            {
                                vlFaturarVerifica = Decimal.Round(valorFaturar + sumTituloMensalidadeEAditmaentoPago, 2);
                                vlTotalAditamento = Decimal.Round(vlTotalAditamento + sumTituloMensalidadeEAditmaentoPago, 2);
                            }
                            //ADITAMENTO JÁ ESTÁ CONTABILIZANDO O VALOR DE MATERIAL, REMOVENDO O VALOR AQUI PARA NÃO SER CALCULADO DUPLICADO
                            //NO IF ABAIXO. Decimal.Round(vlFaturarVerifica + vl_material_contrato, 2)
                            vlFaturarVerifica -= vl_material_contrato;
                            
                            if (Decimal.Round(vlTotalAditamento, 2) != Decimal.Round(vlFaturarVerifica + vl_material_contrato, 2))
                                throw new MatriculaBusinessException(String.Format(Messages.msgErroValorTituloAditivo), null, MatriculaBusinessException.TipoErro.ERRO_ALUNO_TURMA_CONTRATO, false);
                        }
                        else
                        {
                            if (vl_abatimento > 0)
                                vlFaturarVerifica -= vl_abatimento;
                            //O valor da dívida já está incorporado
                            //if (contratoView.id_divida_primeira_parcela)
                            //    vlFaturarVerifica = vlFaturarVerifica + contratoView.vl_divida_contrato;
                            if (Decimal.Round(sumTituloMensalidade, 2) != Decimal.Round(vlFaturarVerifica + vl_material_contrato, 2))
                                throw new MatriculaBusinessException(String.Format(Messages.msgErroValorTitulo), null, MatriculaBusinessException.TipoErro.ERRO_ALUNO_TURMA_CONTRATO, false);
                        }
                        if (contratoView.TaxaMatricula != null && contratoView.TaxaMatricula.Count() > 0)
                        {
                            sumTituloTaxa = listaTituloView.Where(tit => tit.dc_tipo_titulo == "TM" || tit.dc_tipo_titulo == "TA").Sum(t => t.vl_titulo);
                            valorTaxaFaturar = contratoView.TaxaMatricula.ToList()[0].vl_matricula_taxa == null ? 0 : Convert.ToDecimal(contratoView.TaxaMatricula.ToList()[0].vl_matricula_taxa);
                            if (vl_abatimento_taxa > 0)
                                valorTaxaFaturar -= vl_abatimento_taxa;
                            if (Decimal.Round(sumTituloTaxa, 2) != Decimal.Round(valorTaxaFaturar, 2))
                                throw new MatriculaBusinessException(String.Format(Messages.msgErroValorTaxaMatricula), null, MatriculaBusinessException.TipoErro.ERRO_ALUNO_TURMA_CONTRATO, false);
                        }
                        List<Titulo> titulosAdd = listaTituloView.Where(x => x.cd_titulo == 0).ToList();
                        if (titulosAdd != null && titulosAdd.Count() > 0)
                            foreach (Titulo t in titulosAdd)
                            {
                                t.cd_origem_titulo = contratoOldContext.cd_contrato;
                                t.id_status_titulo = (int)Titulo.StatusTitulo.ABERTO;
                                t.dh_cadastro_titulo = DateTime.Now.ToUniversalTime();
                                t.id_natureza_titulo = (Byte)Titulo.NaturezaTitulo.RECEBER;
                                //t.cd_plano_conta_tit = contratoView.cd_plano_conta; Motivo Comentário: Todos titulos estavam sendo gerados com mesmo plano de contas, excluindo plano de contas "Taxa de Matrícula".
                                t.cd_pessoa_empresa = contratoOldContext.cd_pessoa_escola;
                                if (t.vl_titulo != t.vl_saldo_titulo)
                                    t.vl_saldo_titulo = t.vl_titulo;
                                t.tituloEdit = true;
                            }

                        var listaAdt = new List<Titulo>();
                        if (aditamento)
                        {
                            if (pc_desconto_bolsa >= 0 &&
                                id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADITIVO_BOLSA ||
                                id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                            {
                                if(listaTituloView.Any(tit => !tit.possuiBaixa && tit.possuiBaixaBolsa))
                                    listaAdt = listaTituloView.Where(tit => !tit.possuiBaixa && tit.possuiBaixaBolsa && tit.dc_tipo_titulo == "AD" && tit.vl_saldo_titulo > 0).ToList();                               
                                else
                                    listaAdt = listaTituloView.Where(tit => tit.dc_tipo_titulo == "AD" && tit.vl_saldo_titulo > 0 && tit.vl_titulo == tit.vl_saldo_titulo).ToList(); 

                                //PEGOU APENAS TITULO AD, DURANTE A BAIXA POR MOTIVO BOLSA COM PERCENTUAL 0 DEVE PEGAR TAMBEM MATRICULA
                                if(id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADITIVO_BOLSA)
                                    listaAdt = listaTituloView.Where(tit => !tit.possuiBaixa && tit.possuiBaixaBolsa && (tit.dc_tipo_titulo == "AD" || tit.dc_tipo_titulo == "ME") && tit.vl_saldo_titulo > 0).ToList();
                                var listaEditTitulo = new List<Titulo>();
                                listaEditTitulo = listaTituloView.Where(x => x.tituloEdit).ToList();
                                if(listaEditTitulo.Count() == 0)
                                foreach (var titulo in listaAdt)
                                {
                                    //quando faz um aditamento do tipo bolsa para zerar ou dar bolsa e existem títulos abertos que já foram enviados para CNAB, não permitir que sejam excluidos. 
                                    if (titulo.id_emitido_CNAB == false && (!titulo.id_cnab_contrato && titulo.id_status_cnab == (int)Titulo.StatusCnabTitulo.INICIAL))
                                        titulo.tituloEdit = true;
                                }
                            }
                        }

                        BusinessFinanceiro.editTitulosContrato(listaTituloView.Where(x => x.tituloEdit ).ToList(), contratoView, pc_desconto_bolsa);
                        if ((!aditamento && !contratoView.id_ajuste_manual && pc_desconto_bolsa > 0) ||
                              aditamento && !contratoView.id_ajuste_manual && id_tipo_aditamento != null &&
                              (id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADITIVO_BOLSA || pc_desconto_bolsa > 0))
                        {
                            if (!aditamento)
                            {
                                DateTime dt_baixa_titulo = contratoView.dt_matricula_contrato.HasValue ? contratoView.dt_matricula_contrato.Value : DateTime.Now.Date;
                                BusinessFinanceiro.gerarBaixaParcialBolsaTitulos(listaTituloView, pc_desconto_bolsa, dt_baixa_titulo, false);
                            }
                            else
                                if ((contratoView.aditamentoMaxData.AditamentoBolsa != null && contratoView.aditamentoMaxData.AditamentoBolsa.Count() > 0) ||
                                    pc_desconto_bolsa > 0)
                                {

                                    bool zerarAditamentoBolsa = (contratoView.aditamentoMaxData != null &&
                                                                contratoView.aditamentoMaxData.id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADITIVO_BOLSA && 
                                                                contratoView.pc_desconto_bolsa == 0);

                                    if (listaAdt.Count > 0 && zerarAditamentoBolsa)
                                    {
                                        foreach (var titulo in listaAdt)
                                        {
                                            titulo.tituloEdit = false;
                                        }
                                    }

                                    DateTime dt_baixa_titulo = contratoView.aditamentoMaxData.dt_inicio_aditamento.HasValue ?
                                        contratoView.aditamentoMaxData.dt_inicio_aditamento.Value : DateTime.Now.Date;                                    
                                    bool aditivoBolsa = id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADITIVO_BOLSA;

                                    if (aditivoBolsa)
                                    {
                                        //Será aplicado a perda ou a concessão de descontos do tipo “Bolsa”, para os títulos da matrícula em questão, que tenham data de vencimento maior ou igual a data de vencimento informada no campo “Data de Vencimento Inicial
                                        if (contratoView.aditamentoMaxData.AditamentoBolsa != null && contratoView.aditamentoMaxData.AditamentoBolsa.Count() > 0)
                                        {
                                            var adtBolsa = contratoView.aditamentoMaxData.AditamentoBolsa.FirstOrDefault();
                                            listaTituloView = listaTituloView.Where(d => d.dt_vcto_titulo >= adtBolsa.dt_comunicado_bolsa.Value.Date).ToList();
                                        }
                                    }
                                    BusinessFinanceiro.gerarBaixaParcialBolsaTitulos(listaTituloView, pc_desconto_bolsa, dt_baixa_titulo, false, aditivoBolsa);
                                }
                        }
                    }
                    else
                    {
                        if (aditamento)
                        {
                            //Caso tenha mudado o valor do aditamento e o ajuste manual marcado.
                            List<Titulo> titulosContext = BusinessFinanceiro.getTitulosByContratoTodosDados(contratoView.cd_contrato, contratoView.cd_pessoa_escola).ToList();
                            decimal sumTituloMensalidade = titulosContext.Where(tit => tit.dc_tipo_titulo == "ME" || tit.dc_tipo_titulo == "MA" || tit.dc_tipo_titulo == "MM").Sum(t => t.vl_titulo);
                            decimal sumTituloTaxa = titulosContext.Where(tit => tit.dc_tipo_titulo == "TM" || tit.dc_tipo_titulo == "TA").Sum(t => t.vl_titulo);
                            decimal sumTituloAdt = titulosContext.Where(tit => tit.dc_tipo_titulo == "AD" || tit.dc_tipo_titulo == "AA").Sum(t => t.vl_titulo);
                            if (Decimal.Round(sumTituloMensalidade + sumTituloAdt, 2) != Decimal.Round(valorFaturar, 2))
                                if (contratoView.id_ajuste_manual)
                                    throw new MatriculaBusinessException(String.Format(Messages.msgErroValorTituloAditivo), null, MatriculaBusinessException.TipoErro.ERRO_ALUNO_TURMA_CONTRATO, false);
                        }
                    }
                }
            }
        }

        private void VerificaDelecaoTitulosMatricula(Contrato contratoView, bool aditamento, Contrato contratoOldContext, double pc_desconto_bolsa)
        {
            bool existeBolsaContrato = false;
            if (pc_desconto_bolsa > 0)
                existeBolsaContrato = true;
            // Se era ajuste manual e passou para automatico, deleta todos os títulos e cria novamente
            if (!aditamento)
                if (contratoOldContext.id_ajuste_manual && !contratoView.id_ajuste_manual)
                    BusinessFinanceiro.deleteAllTitulo(contratoView.cd_contrato, contratoView.cd_pessoa_escola, existeBolsaContrato);
                else
                    BusinessFinanceiro.deletarTitulosEdicaoMatricula(contratoView, pc_desconto_bolsa);
        }

        public Contrato updateContrato(Contrato contrato)
        {
            return DataAccessMatricula.edit(contrato, false);
        }

        //Inclusão de um novo aditamento
        private void persitirAditamento(Contrato contrato)
        {
            bool gerarNovoAdt = true;
            Aditamento newAditamento = contrato.aditamentoMaxData;
            Aditamento aditamentoMaxContext = null;
            if (newAditamento.cd_aditamento > 0)
                aditamentoMaxContext = DataAccessAditamento.getAditamentoByContrato(newAditamento.cd_aditamento, contrato.cd_contrato, contrato.cd_pessoa_escola);
            if (Aditamento.comparaAlterouAditamento(contrato.aditamentoMaxData, aditamentoMaxContext))
            {
                //DateTime dataAditamentoEdit = DateTime.UtcNow;

                if (aditamentoMaxContext != null)
                {
                    //if (aditamentoMaxContext.dt_aditamento != null)
                    //    dataAditamentoEdit = (DateTime)aditamentoMaxContext.dt_aditamento;
                    // Ramos
                    if (aditamentoMaxContext.id_tipo_aditamento != null)
                    {
                        if (aditamentoMaxContext.dt_inicio_aditamento != null && newAditamento.dt_inicio_aditamento != null &&
                            aditamentoMaxContext.dt_inicio_aditamento.Value.Date == newAditamento.dt_inicio_aditamento.Value.Date)
                        {
                            if (newAditamento.dt_inicio_aditamento.HasValue)
                                newAditamento.dt_inicio_aditamento = newAditamento.dt_inicio_aditamento.Value.Date;
                            if (newAditamento.dt_vcto_aditamento.HasValue)
                                newAditamento.dt_vcto_aditamento = newAditamento.dt_vcto_aditamento.Value.Date;
                            aditamentoMaxContext = Aditamento.changeValuesAditamento(aditamentoMaxContext, newAditamento);
                            gerarNovoAdt = false;
                            DataAccessAditamento.saveChanges(false);
                            if (newAditamento.Desconto != null)
                                crudDescontoAditamento(newAditamento.Desconto.ToList(), aditamentoMaxContext.cd_aditamento, contrato.cd_contrato, contrato.cd_pessoa_escola);
                        }
                    }
                }
                //Gravando novo registro de aditamento
                // Ramos
                if (gerarNovoAdt)
                {
                    List<DescontoContrato> descontos = null;
                    if (newAditamento.Desconto != null)
                        descontos = newAditamento.Desconto.ToList();
                    if (newAditamento.AditamentoBolsa != null && newAditamento.AditamentoBolsa.Count() > 0 && newAditamento.AditamentoBolsa.FirstOrDefault().dt_comunicado_bolsa != null)
                        newAditamento.AditamentoBolsa.FirstOrDefault().dt_comunicado_bolsa = newAditamento.AditamentoBolsa.FirstOrDefault().dt_comunicado_bolsa.Value.Date;
                    newAditamento.Desconto = null;
                    //String.Format("{0:T}", dataAditamentoEdit);
                    //DateTime novahora = dataAditamentoEdit + new TimeSpan(0, 0, 1, 1);
                    DateTime novahora = DateTime.UtcNow;
                    newAditamento.dt_aditamento = novahora;
                    
                    if (newAditamento.dt_inicio_aditamento.HasValue)
                        newAditamento.dt_inicio_aditamento = newAditamento.dt_inicio_aditamento.Value.Date;
                    if (!newAditamento.dt_vencto_inicial.HasValue)
                    {
                        newAditamento.dt_vencto_inicial = novahora;
                        newAditamento.dt_vencto_inicial = newAditamento.dt_vencto_inicial.Value.Date;
                    }
                       
                    if (newAditamento.dt_vcto_aditamento.HasValue)
                        newAditamento.dt_vcto_aditamento = newAditamento.dt_vcto_aditamento.Value.Date;
                    //newAditamento.cd_contrato = contrato.cd_contrato;
                    newAditamento.cd_usuario = contrato.cd_usuario;
                    DataAccessAditamento.add(newAditamento, false);
                    if (descontos != null && descontos.Count() > 0)
                        crudDescontoAditamento(descontos, newAditamento.cd_aditamento, newAditamento.cd_contrato, contrato.cd_pessoa_escola);
                }
            }
            else
                if (newAditamento.Desconto != null)
                    crudDescontoAditamento(newAditamento.Desconto.ToList(), aditamentoMaxContext.cd_aditamento, contrato.cd_contrato, contrato.cd_pessoa_escola);
            contrato.Aditamento = null;
        }

        private void persistirDescontoContrato(Contrato contrato)
        {
            List<DescontoContrato> descontosContratoContext = DataAccessDescontoContrato.getDescontoContratoAllDados(contrato.cd_contrato, contrato.cd_pessoa_escola).ToList();

            if (contrato.DescontoContrato == null)
            {
                if (descontosContratoContext != null)
                {
                    foreach (var item in descontosContratoContext)
                    {
                        var descontosContratoDelet = DataAccessDescontoContrato.findById(item.cd_desconto_contrato, false);
                        if (descontosContratoDelet != null)
                            DataAccessDescontoContrato.delete(descontosContratoDelet, false);
                    }
                }
            }
            else
            {
                IEnumerable<DescontoContrato> descontoContratoView = contrato.DescontoContrato;
                List<DescontoContrato> descontosContratoComCodigo = descontoContratoView.Where(fd => fd.cd_desconto_contrato != 0).ToList();
                List<DescontoContrato> descontosContratoInsert = descontoContratoView.Where(fd => fd.cd_desconto_contrato <= 0).ToList();
                List<DescontoContrato> descontosContratoEdit = descontoContratoView.Where(fd => fd.cd_desconto_contrato > 0).ToList();
                List<DescontoContrato> descontosContratoDeleted = descontosContratoContext.Where(pc => !descontosContratoComCodigo.Any(pv => pc.cd_desconto_contrato == pv.cd_desconto_contrato)).ToList();

                if (descontosContratoDeleted.Count() > 0)
                {
                    foreach (var item in descontosContratoDeleted)
                        DataAccessDescontoContrato.deleteContext(item, false);
                }

                foreach (var descontoContratoNew in descontosContratoInsert)
                {
                    descontoContratoNew.cd_contrato = contrato.cd_contrato;
                    var newDesconto = descontoContratoNew.cd_desconto_contrato == 0 ? DataAccessDescontoContrato.addContext(descontoContratoNew, false) : null;
                }

                //Edita os registros modificados pelo usuário
                foreach (var descontosEdit in descontosContratoEdit)
                {
                    DescontoContrato descontoContratoContext = descontosContratoContext.Where(x => x.cd_desconto_contrato == descontosEdit.cd_desconto_contrato).FirstOrDefault();
                    if (descontoContratoContext != null)
                    {
                        descontoContratoContext.copy(descontosEdit);
                    }
                }
            }

            DataAccessDescontoContrato.saveChanges(false);
        }

        private void crudDescontoAditamento(List<DescontoContrato> descontosAditamentoView, int cd_aditamento, int cd_contrato, int cd_escola)
        {
            //Pega os descontos do contrato na base de dados
            List<DescontoContrato> descontoContratoContext = DataAccessDescontoContrato.getDescontoAditamento(cd_contrato, cd_aditamento, cd_escola).ToList();
            IEnumerable<DescontoContrato> descontosComCodigo = from d in descontosAditamentoView
                                                               where d.cd_desconto_contrato != 0
                                                               select d;
            IEnumerable<DescontoContrato> descontosDeleted = descontoContratoContext.Where(tc => !descontosComCodigo.Any(tv => tc.cd_desconto_contrato == tv.cd_desconto_contrato));
            //Deleta os registros que estão na base mas não estão na view
            if (descontosDeleted.Count() > 0)
            {
                foreach (var d in descontosDeleted)
                    DataAccessDescontoContrato.deleteContext(d, false);
            }

            foreach (var d in descontosAditamentoView)
            {
                if (d.cd_desconto_contrato == 0)
                {
                    //d.cd_contrato = cd_contrato;
                    d.cd_aditamento = cd_aditamento;
                    DataAccessDescontoContrato.addContext(d, false);
                }
                else
                {
                    DescontoContrato descontosContradoContext = descontoContratoContext.Where(x => x.cd_desconto_contrato == d.cd_desconto_contrato).FirstOrDefault();
                    if (descontosContradoContext != null)
                        DescontoContrato.changeValuesDescontoContratoAditamento(d, descontosContradoContext);
                }
            }
            DataAccessDescontoContrato.saveChanges(false);
        }

        private void editTaxaMatriculaContrato(Contrato contrato)
        {
            TaxaMatricula taxaMatricula = new TaxaMatricula();
            if (contrato.TaxaMatricula != null && contrato.TaxaMatricula.Count() > 0)
            {
                taxaMatricula = DataAccessTaxaMatricula.getTaxaMatriculaByIdContrato(contrato.cd_contrato, contrato.cd_pessoa_escola);
                if (taxaMatricula != null && taxaMatricula.cd_taxa_matricula > 0)
                {
                    taxaMatricula.copy(contrato.TaxaMatricula.ToList()[0]);
                    taxaMatricula.dt_vcto_taxa = taxaMatricula.dt_vcto_taxa.ToLocalTime().Date;
                    DataAccessTaxaMatricula.saveChanges(false);
                }
                else
                {
                    contrato.TaxaMatricula.ToList()[0].dt_vcto_taxa = contrato.TaxaMatricula.ToList()[0].dt_vcto_taxa.ToLocalTime().Date;
                    DataAccessTaxaMatricula.add(contrato.TaxaMatricula.ToList()[0], false);
                }
            }
        }

        private AlunoTurma salvarAlunoTurma(Contrato contrato, List<AlunoTurma> alunoAguardando, int cdUsuario, int fusoHorario)
        {
            AlunoTurma existeAlunoTurma = new AlunoTurma();
            string dc_situacao_turma = "";
            foreach (var alunoTurma in alunoAguardando)
            {
                getSituacaoTurma(contrato, alunoTurma, out dc_situacao_turma);
                existeAlunoTurma.cd_contrato = contrato.cd_contrato;
                existeAlunoTurma.cd_situacao_aluno_turma = alunoTurma.cd_situacao_aluno_turma;
                existeAlunoTurma.dt_matricula = alunoTurma.dt_matricula;
                existeAlunoTurma.dt_movimento = alunoTurma.dt_movimento;
                existeAlunoTurma.cd_aluno = alunoTurma.cd_aluno;
                existeAlunoTurma.cd_turma = alunoTurma.cd_turma;
                existeAlunoTurma.dt_inicio = alunoTurma.dt_inicio;
                existeAlunoTurma.nm_matricula_turma = alunoTurma.nm_matricula_turma;
                if (contrato.id_tipo_contrato == (byte)Contrato.TipoCKMatricula.NORMAL)
                {
                    var cc = contrato.CursoContrato.ToList();
                    if (cc != null && cc.Count > 0)
                    {
                        //Sempre será apenas 1 registro
                        existeAlunoTurma.cd_curso_contrato = cc[0].cd_curso_contrato;
                    }
                    else
                    {
                        List<CursoContrato> cursoContrato = DataAccessCursoContrato.getCursosContratoByCdContrato(contrato.cd_contrato);
                        if (cursoContrato != null && cursoContrato.Count > 0)
                        {
                            existeAlunoTurma.cd_curso_contrato = cursoContrato[0].cd_curso_contrato;
                        }
                    }
                    
                    
                }
                else
                    if (alunoTurma.CursoContrato != null) 
                        existeAlunoTurma.cd_curso_contrato = alunoTurma.CursoContrato.cd_curso_contrato;
                BusinessSecretaria.addAlunoTurma(existeAlunoTurma);
                existeAlunoTurma = BusinessSecretaria.findAlunoTurma(contrato.cd_aluno, alunoTurma.cd_turma, contrato.cd_pessoa_escola);
                criaHistoricoAluno(contrato, alunoTurma.cd_turma, existeAlunoTurma.Turma.dt_inicio_aula, alunoTurma.cd_situacao_aluno_turma, cdUsuario, fusoHorario);
            }

            return existeAlunoTurma;
        }

        public Contrato getMatriculaByTurmaAluno(int cdTurma, int cdAluno)
        {
            return DataAccessMatricula.getMatriculaByTurmaAluno(cdTurma, cdAluno);
        }

        public bool existeMatriculaByProduto(int produto, int cdAluno, DateTime? dt_inicio_aula, int curso, bool id_turma_ppt, int cd_contrato, DateTime? dt_final_aula, int cd_duracao)
        {
            return DataAccessMatricula.existeMatriculaByProduto(produto, cdAluno, dt_inicio_aula, curso, id_turma_ppt, cd_contrato, dt_final_aula, cd_duracao);
        }

        public Contrato getContratoImpressao(int cd_escola, int cd_contrato)
        {
            Contrato contrato = new Contrato();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                contrato = DataAccessMatricula.getContratoImpressao(cd_escola, cd_contrato);
                if (contrato.cd_turma_atual > 0)
                {
                    List<Horario> horariosTurma = BusinessSecretaria.getHorarioByEscolaForRegistroUncommited(cd_escola, contrato.cd_turma_atual.Value, Horario.Origem.TURMA).ToList();
                    //horariosTurma = horariosTurma.OrderByDescending(x => x.id_dia_semana).OrderByDescending(x => x.dt_hora_ini).OrderByDescending(x => x.dt_hora_fim).ToList();
                    double qtd_minutos_turma = 0;

                    contrato.hashtableHorarios = Horario.montaDiaHorario(horariosTurma, ref qtd_minutos_turma);
                    contrato.qtd_minutos_turma = qtd_minutos_turma;
                }
                transaction.Complete();
            }
            return contrato;
        }

        public Contrato getContratoBaixa(int cd_escola, int cd_contrato)
        {
            Contrato contrato = new Contrato();
            contrato = DataAccessMatricula.getContratoBaixa(cd_escola, cd_contrato);
            return contrato;
        }

        public Contrato findMatriculaByTurmaAluno(int cdTurma, int cdAluno)
        {
            return DataAccessMatricula.findMatriculaByTurmaAluno(cdTurma, cdAluno);
        }

        public int? getMatriculaAluno(int cd_aluno, int cd_escola)
        {
            return DataAccessMatricula.getMatriculaAluno(cd_aluno, cd_escola);
        }

        public Contrato getMatriculaByIdVI(int cd_contrato, int cd_pessoa_escola)
        {
            return DataAccessMatricula.getMatriculaByIdVI(cd_contrato, cd_pessoa_escola);
        }

        public DocumentoDigitalizadoEditUI getDocumentoDigitalizadoUpdated(int cd_contrato, int cd_pessoa_escola)
        {
            return DataAccessMatricula.getDocumentoDigitalizadoUpdated(cd_contrato, cd_pessoa_escola);
        }

        public List<Contrato> getMatriculasAluno(int cd_aluno, int cd_escola)
        {
            return DataAccessMatricula.getMatriculasAluno(cd_aluno, cd_escola);
        }

        public void saveChangesMat()
        {
            DataAccessMatricula.saveChanges(false);
        }

        public Contrato getMatriculaCnabBoleto(int cd_empresa, int cd_contrato, byte tipo)
        {
            Contrato contrato = DataAccessMatricula.getMatriculaCnabBoleto(cd_empresa, cd_contrato);
            contrato.titulos = BusinessFinanceiro.getTitulosByContratoLeitura(cd_contrato, cd_empresa);
            
            setSugestaoDataVencimentoCnab(contrato);
            setSugestaoLocalMovtoCnab(contrato);
            setSugestaoTitulosMatricula(contrato, cd_empresa, tipo);

            return contrato;
        }

        private void setSugestaoTitulosMatricula(Contrato contrato, int cd_empresa, byte tipo)
        {
            var locais = new List<LocalMovto>();
            locais.Add(new LocalMovto { cd_local_movto = contrato.titulos.Where(t => t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.BOLETO).First().cd_local_movto });            

            TituloUI titulo = new TituloUI
            {
                cd_pessoa_empresa = cd_empresa,
                natureza = (int)Titulo.NaturezaTitulo.RECEBER,
                emissao = false,
                vencimento = true,
                todosLocais = false,
                id_tipo_cnab = (int)Cnab.TipoCnab.GERAR_BOLETOS,
                dtInicial = contrato.dt_inicial_contrato,
                dtFinal = contrato.dt_final_contrato,
                nro_contrato = contrato.nm_contrato.Value,
                locaisEscolhidos = locais,
                id_cnab_tipo = tipo
            };

            if (locais.Count == 0)
            {
                titulo.todosLocais = true;
                titulo.locais = BusinessFinanceiro.getLocalMovimentoSomenteLeitura(cd_empresa, 0, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_FILTRO_CNAB, 0);
            }

            contrato.titulos = BusinessFinanceiro.searchTituloCnab(titulo).ToList();
        }
        private void setSugestaoLocalMovtoCnab(Contrato contrato)
        {
            var cd_local_movto = contrato.titulos.Where(t => t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.BOLETO ).First().cd_local_movto;
            if (contrato.titulos.Where(t => t.cd_local_movto == cd_local_movto && t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.BOLETO).Count() == 
                contrato.titulos.Where(t => t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.BOLETO ).Count())
            {
                contrato.cd_local_movto = cd_local_movto;
            }
        }

        private void setSugestaoDataVencimentoCnab(Contrato contrato)
        {
            if (contrato.titulos.Where(t => t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.BOLETO).Count() > 1)
            {
                contrato.dt_inicial_contrato = contrato.titulos.Where(t => t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.BOLETO).OrderBy(t => t.nm_parcela_titulo).First().dt_vcto_titulo;
                contrato.dt_final_contrato = contrato.titulos.Where(t => t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.BOLETO).OrderBy(t => t.nm_parcela_titulo).Last().dt_vcto_titulo;
            }
            else
            {
                contrato.dt_inicial_contrato = contrato.titulos.First().dt_vcto_titulo;
                contrato.dt_final_contrato = contrato.dt_inicial_contrato;
            }
        }

        public IEnumerable<MatriculaRel> getMatriculaAnalitico(int cd_empresa, int cd_turma, int cd_aluno, List<int> situacaoAlunoTurma, bool semTurma, bool tranferencia, bool retorno, int situacaoContrato, DateTime? dtIni, DateTime? dtFim, int cdAtendente, bool bolsaCem, bool contratoDigitalizado, int? cdProduto, string noProduto, bool exibirEnderecos, int vinculado)
        {
            IEnumerable<MatriculaRel> retornoRel = new List<MatriculaRel>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retornoRel = DataAccessMatricula.getMatriculaAnalitico(cd_empresa, cd_turma, cd_aluno, situacaoAlunoTurma, semTurma, tranferencia, retorno, situacaoContrato, dtIni, dtFim, cdAtendente, bolsaCem, contratoDigitalizado, cdProduto, noProduto, exibirEnderecos, vinculado);
                transaction.Complete();
            }
            return retornoRel;
        }
        public IEnumerable<MatriculaRel> getMatriculaPorMotivo(int cd_empresa, int cd_turma, int cd_aluno, List<int> situacaoAlunoTurma, bool semTurma, bool tranferencia, bool retorno, int situacaoContrato, DateTime? dtIni, DateTime? dtFim, int vinculado)
        {
            IEnumerable<MatriculaRel> retornoRel = new List<MatriculaRel>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retornoRel = DataAccessMatricula.getMatriculaPorMotivo(cd_empresa, cd_turma, cd_aluno, situacaoAlunoTurma, semTurma, tranferencia, retorno, situacaoContrato, dtIni, dtFim, vinculado);
                transaction.Complete();
            }
            return retornoRel;
        }
        public DataTable getMatriculaOutros(int cd_escola, int cd_produto, DateTime? dta_ini, DateTime? dta_fim, byte qtd_max)
        {
            return DataAccessMatricula.getMatriculaOutros(cd_escola, cd_produto, dta_ini, dta_fim, qtd_max);
        }
        public Contrato getMatriculaForMovimento(int id, int cdEscola)
        {
            return DataAccessMatricula.getMatriculaForMovimento(id, cdEscola);
        }

        public byte getTpMatricula(int cdContrato, int cdEscola)
        {
            return DataAccessMatricula.getTpMatricula(cdContrato, cdEscola);
        }

        public double getBolsaMatricula(int cdContrato, int cdEscola)
        {
            return DataAccessMatricula.getBolsaMatricula(cdContrato, cdEscola);
        }

        public Contrato getSaldoMatricula(int cdContrato, int cdEscola, decimal pc_bolsa)
        {
            return DataAccessMatricula.getSaldoMatricula(cdContrato, cdEscola, pc_bolsa);
        }

        public bool getAjusteManualMatricula(int cdContrato, int cdEscola)
        {
            return DataAccessMatricula.getAjusteManualMatricula(cdContrato, cdEscola);
        }

        public List<CursoContrato> getCursoContrato(int cd_contrato)
        {
            return DataAccessCursoContrato.getCursosContratoByCdContrato(cd_contrato);            
        }
        public string postGerarVenda(int? cd_turma, int? cd_curso, int? cd_usuario,  int? fuso, bool id_futura, int cd_regime)
        {
            string retorno = null;

            //A transacao será controlada na procedure
            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            //{
                retorno = DataAccessMatricula.postGerarVenda(cd_turma, cd_curso, cd_usuario, fuso, id_futura, cd_regime);
                //transaction.Complete();

            //}
            return retorno;
        }

        public string postVincularVendaMaterial(int? cd_movimento, int? cd_turma_origem, int? cd_contrato)
        {
            string retorno = null;

            //A transacao será controlada na procedure
            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            //{
            retorno = DataAccessMatricula.postVincularVendaMaterial(cd_movimento, cd_turma_origem, cd_contrato);
            //transaction.Complete();

            //}
            return retorno;
        }

        #endregion

        #region Desconto Contrato

        public DescontoContrato addDocumentoContrato(DescontoContrato descontoContrato)
        {
            DescontoContrato newDescontoContrato = new DescontoContrato();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                newDescontoContrato = new DescontoContrato
                {
                    cd_contrato = descontoContrato.cd_contrato,
                    cd_tipo_desconto = descontoContrato.cd_tipo_desconto,
                    id_desconto_ativo = descontoContrato.id_desconto_ativo,
                    pc_desconto_contrato = descontoContrato.pc_desconto_contrato,
                    vl_desconto_contrato = descontoContrato.vl_desconto_contrato
                };
                newDescontoContrato = DataAccessDescontoContrato.add(newDescontoContrato, false);
                transaction.Complete();
            }
            return newDescontoContrato;
        }

        public IEnumerable<DescontoContrato> getDescontoContrato(int cd_contrato, int cd_pessoa_escola)
        {
            return DataAccessDescontoContrato.getDescontoContrato(cd_contrato, cd_pessoa_escola);
        }

        public IEnumerable<DescontoContrato> getDescontoContratoPesq(int cd_contrato, int cd_pessoa_escola)
        {
            IEnumerable<DescontoContrato> desconto = new List<DescontoContrato>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                desconto = DataAccessDescontoContrato.getDescontoContrato(cd_contrato, cd_pessoa_escola);
                transaction.Complete();
            }
            return desconto;
        }

        private void validarMinDateContrato(Contrato contrato)
        {
            if ((contrato != null && contrato.dt_inicial_contrato != null && DateTime.Compare((DateTime)contrato.dt_inicial_contrato, new DateTime(1900, 1, 1)) < 0) ||
                (contrato != null && contrato.dt_final_contrato != null && DateTime.Compare((DateTime)contrato.dt_final_contrato, new DateTime(1900, 1, 1)) < 0) ||
                (contrato != null && contrato.dt_matricula_contrato != null && DateTime.Compare((DateTime)contrato.dt_matricula_contrato, new DateTime(1900, 1, 1)) < 0))
                throw new MatriculaBusinessException(Messages.msgErroMinDateContrato, null,
                      MatriculaBusinessException.TipoErro.ERRO_MIN_DATE_MATRICULA, false);
        }
        #endregion

        #region Taxa de Matricula
        public TaxaMatricula getTaxaMatriculaByIdContrato(int cd_contrato, int cd_pessoa_escola)
        {
            return DataAccessTaxaMatricula.getTaxaMatriculaByIdContrato(cd_contrato, cd_pessoa_escola);
        }

        public TaxaMatriculaSearchUI searchTaxaMatricula(int cd_contrato, int cd_pessoa_escola)
        {
            TaxaMatriculaSearchUI retorno = new TaxaMatriculaSearchUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessTaxaMatricula.searchTaxaMatricula(cd_contrato, cd_pessoa_escola);
                transaction.Complete();
            }
            return retorno;
        }
        #endregion

        #region Aditamento

        public Aditamento getAditamentoByContrato(int cd_contrato, int cd_escola)
        {
            return DataAccessAditamento.getAditamentoByContrato(cd_contrato, cd_escola);
        }

        public Aditamento getAditamentoByContratoEData(DateTime? data_aditamento, int cd_contrato, int cd_pessoa_escola)
        {
            return DataAccessAditamento.getAditamentoByContratoEData(data_aditamento, cd_contrato, cd_pessoa_escola);
        }

        public Aditamento getAditamentoByContratoMaxData(int cd_contrato, int cd_escola)
        {
            return DataAccessAditamento.getAditamentoByContratoMaxData(cd_contrato, cd_escola);
        }
        public Aditamento getPenultimoAditamentoByContrato(int cd_contrato, int cd_escola, DateTime dataUltimoAdt)
        {
            return DataAccessAditamento.getPenultimoAditamentoByContrato(cd_contrato, cd_escola, dataUltimoAdt);
        }
        public IEnumerable<Aditamento> getAditamentosByContrato(int cd_contrato, int cd_escola)
        {
            return DataAccessAditamento.getAditamentosByContrato(cd_contrato, cd_escola);
        }

        public Aditamento deleteAditamentoByContrato(int cd_contrato, int cd_aditamento, int cd_escola)
        {
            //Aditamento retornoRel = new Aditamento();
            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            //{
               Aditamento retornoRel =  DataAccessAditamento.getUltimoAditamentoByContrato(cd_contrato, cd_escola);

            //   List<Titulo> titulosAditamento = BusinessFinanceiro.getTitulosByTituloAditamento(cd_escola, cd_aditamento);
               string strIdsTitulos = "";

            //   if (titulosAditamento != null && titulosAditamento.Count > 0)
            //   {
            //       foreach (Titulo e in titulosAditamento)
            //       {
            //           strIdsTitulos += e.cd_titulo + ",";
            //       }
            //   }

            //   if (strIdsTitulos.Length > 0)
            //       strIdsTitulos = strIdsTitulos.Substring(0, strIdsTitulos.Length - 1);
                       

               //if (aditamento != null && aditamento.cd_aditamento == cd_aditamento && aditamento.id_tipo_aditamento == (byte)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
               //{

               //    List<AditamentoBolsa> aditamentoBolsas = DataAccessAditamento.getAditamentoBolsaByAditamento(aditamento.cd_aditamento);
               //    List<DescontoContrato> descontoContratos = DataAccessAditamento.getDescontoContratoByAditamento(aditamento.cd_aditamento);

               //    if (aditamentoBolsas != null && aditamentoBolsas.Count > 0)
               //    {
               //        for (int i = aditamentoBolsas.Count - 1; i >= 0; i--)
               //        {
               //            DataAccessAditamentoBolsa.delete(aditamentoBolsas[i], false);
               //        }
               //    }

               //    if (descontoContratos != null && descontoContratos.Count > 0)
               //    {
               //        for (int i = descontoContratos.Count - 1; i >= 0; i--)
               //        {
               //            DataAccessDescontoContrato.delete(descontoContratos[i], false);
               //        }
               //    }


               //   var deletado = DataAccessAditamento.delete(aditamento, false);
               //   if (!String.IsNullOrEmpty(strIdsTitulos))
               //   {
               //       if (deletado == true)
               //       {
               //           retornoRel = aditamento;

            bool retDeleteTitulo = BusinessFinanceiro.deleteTitulosByTituloAditamento(cd_aditamento, strIdsTitulos); //Estava cd_escola. A string foi mantida para manter a função.
            if (retDeleteTitulo == false)
            {
                throw new MatriculaBusinessException(String.Format(Messages.msgNotDeletedReg), null, MatriculaBusinessException.TipoErro.ERRO_EXCLUSAO_REGISTRO, false);
            }
                  //    }
                  //    else
                  //    {
                  //        throw new MatriculaBusinessException(String.Format(Messages.msgNotDeletedReg), null, MatriculaBusinessException.TipoErro.ERRO_EXCLUSAO_REGISTRO, false);
                  //    }
                  //}
            //   }
            //   else
            //   {
            //       throw new MatriculaBusinessException(String.Format(Messages.msgNotDeleteAditamento), null, MatriculaBusinessException.TipoErro.ERRO_NAO_PERMITE_EXCLUIR_ADITAMENTO_DIFERENTE_ADICIONAR_PARCELA, false);
            //   }

            //    transaction.Complete();
            //}

            return retornoRel;
        }

        public List<DescontoContrato> getDescontosAplicadosAditamento(int cd_contrato, int cd_escola)
        {
            return DataAccessAditamento.getDescontosAplicadosAditamento(cd_contrato, cd_escola).ToList();
        }

        public IEnumerable<DescontoContrato> getDescontosAplicadosContratoOrAditamento(int cd_contrato, int cd_escola)
        {
            return DataAccessAditamento.getDescontosAplicadosContratoOrAditamento(cd_contrato, cd_escola).ToList();
        }

        #endregion

        #region Reajuste Anual

        public Contrato getContratoReajusteAnual(int cdContrato, int cdEscola)
        {
            return DataAccessMatricula.getContratoReajusteAnual(cdContrato, cdEscola);
        }

        public bool verificaAditamentoAposReajusteAnual(int cd_empresa, int cd_reajuste_anual)
        {
            return DataAccessAditamento.verificaAditamentoAposReajusteAnual(cd_empresa, cd_reajuste_anual);
        }

        public bool deletarAditamentosReajusteAnual(int cd_empresa, int cd_reajuste_anual)
        {
            List<Aditamento> aditamentos = DataAccessAditamento.getAditamentosByIdsContrato(cd_empresa, cd_reajuste_anual).ToList();
            foreach (Aditamento ad in aditamentos)
                ad.ReajusteAnual = null;
            DataAccessAditamento.deleteRangeContext(aditamentos, false);
            return true;
        }

        #endregion

        #region Ano Escolar

        public IEnumerable<AnoEscolar> getAnoEscolaresAtivos(int? cdAnoEscolar)
        {
            return BusinessSecretaria.getAnoEscolaresAtivos(cdAnoEscolar);
        }

        #endregion

        #region Motivo Bolsa

        public IEnumerable<MotivoBolsa> getMotivoBolsa(bool? status, int? cd_motivo_bolsa)
        {
            return BusinessSecretaria.getMotivoBolsa(status, cd_motivo_bolsa);
        }

        #endregion
    }
}