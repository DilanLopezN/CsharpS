using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Componentes.GenericBusiness.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness
{
    public interface IFiscalBusiness : IGenericBusiness
    {
        void sincronizarContextos(DbContext dbContext);

        //Movimento
        IEnumerable<MovimentoUI> searchMovimento(SearchParameters parametros, int id_tipo_movimento, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie, int cd_empresa,
                                                      bool emissao, bool movimento, DateTime? dtInicial, DateTime? dtFinal, bool nota_fiscal, int statusNF, bool contaSegura, int isImportXML, 
                                                      bool? id_material_didatico, bool? id_venda_futura);
        IEnumerable<MovimentoUI> searchMovimentoFK(SearchParameters parametros, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie, int cd_empresa,
                                                       bool emissao, bool movimento, DateTime? dtInicial, DateTime? dtFinal, int natMovto, bool idNf);
        IEnumerable<MovimentoPerdaMaterialUI> searchMovimentoFKPerdaMaterial(SearchParameters parametros, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie, int cd_empresa,
            bool emissao, bool movimento, DateTime? dtInicial, DateTime? dtFinal, int natMovto, bool idNf, int origem, int? cd_aluno, int? nm_contrato);
        IEnumerable<MovimentoUI> searchMovimentoFKVincularMaterial(SearchParameters parametros, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie, int cd_empresa,
            bool emissao, bool movimento, DateTime? dtInicial, DateTime? dtFinal, int natMovto, bool material_didatico_vincular_material, bool nota_fiscal_vincular_material, int cd_curso);
        IEnumerable<ItemUI> getItemMovimentoSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int? tipoItem, int? cdGrupoItem, int cdEscola, int id_tipo_movto,
            Movimento.TipoVinculoMovimento tipoVinc, bool comEstoque, int? id_natureza_TPNF, bool kit, bool contaSegura, int? cd_movimento, int? cd_aluno = null, DateTime? dt_inicial = null, DateTime? dt_final = null, bool? vinculado_curso = null, int? cd_curso_material_didatico = null);

        IEnumerable<ItemUI> getItemMovimentoSearchPerdaMaterial(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int? tipoItem, int? cdGrupoItem, int cdEscola, int id_tipo_movto,
            Movimento.TipoVinculoMovimento tipoVinc, bool comEstoque, int? id_natureza_TPNF, bool kit, bool contaSegura, int? cd_movimento, int origem, int? cd_aluno = null, DateTime? dt_inicial = null, DateTime? dt_final = null, bool? vinculado_curso = null, int? cd_curso_material_didatico = null);
        Movimento addMovimento(Movimento movimento);
        Movimento editMovimento(Movimento movimento);
        Movimento updateMovimentoOlny(Movimento movimento);
        bool deleteMovimentos(int[] cdMovimentos, int cd_empresa);
        Movimento getMovimentoEditView(int cd_movimento, int cd_empresa);
        Movimento getMovimentoEditViewNF(int cd_movimento, int cd_empresa, int id_tipo_movimento);
        int? getMaxMovimento(int tpMovto, int cd_empresa);
        Espelho getEspelhoMovimento(int cd_movimento, int cd_empresa);
        List<Espelho> getSourceCopiaEspelhoMovimento(int cd_movimento, int cd_empresa);
        IEnumerable<ItemMovimento> getItensMovimentoByMovimento(int cd_movimento, int cd_empresa);
        IEnumerable<ItemMovimento> getItensMovimento(int cd_movimento, int cd_empresa);
        Movimento getMovtoById(int cd_movimento);
        List<ItemMovimento> getItensMaterialAluno(List<int> cdAlunos, int cd_empresa, int cd_turma);
        MovimentoUI getMovimentoReturnGrade(int cd_movimento, int cd_empresa);
        bool existeMovimentoForTit(int cd_pessoa_empresa, List<int> titulos);
        List<ItemMovimento> getItensMvto(int cd_movimento, int cd_escola);
        Movimento getRetMovimentoDevolucao(int cd_movimento, int cd_empresa, int id_tipo_movimento, bool isMaster);
        bool spEnviarMasterSaf(int? cd_movimento);
        IEnumerable<ItemMovimento> getItensMovimentoReciboLeitura(int cd_movimento, int cd_empresa);

        List<Titulo> addTitulosERateoMovimento(List<Titulo> titulos, List<ItemMovimento> itensMovto, int cd_movimento,
            int cd_empresa, int? nm_movimento);

        //Movimentação Financeira
        IEnumerable<ItemMovimento> getItensByAluno(SearchParameters parametros, int cd_pessoa, int cd_aluno, int cd_escola);
        Movimento getMovimentoEditOrigem(int cd_origem, int id_origem_movto, int cd_empresa, int id_tipo_movimento);
        bool existeMovimentoByOrigem(int cdOrigem, int id_origem_movimento);
        bool verificarTipoNotaFiscalPermiteMovimentoFinanceiro(int cd_tipo_nota_fiscal);
        int getUltimoNroRecibo(int? nm_ultimo_Recibo, int cd_empresa);
        bool existeICMSEstadoNota(int cdEstadoOri, int cdEstadoDes);
        bool existeDadosNFNota(int cdCidade);

        //Nota Fiscal
        XmlDocument emitirNF(int cdEscola, int cd_movimento, int id_tipo_movimento, ref int nm_nota_fiscal);
        XmlDocument emitirNFS(int cdEscola, List<int> cd_movimentos, ref int nm_nota_fiscal);
        Movimento processarNF(int cdEscola, int cd_movimento, bool empresaPropria);
        void cancelarNFServico(int cdEscola, int cd_movimento, string dc_justificativa_nfs, bool id_empresa_propria);
        Movimento postVerificarCancelamentoNF(int cd_escola, int cd_movimento, bool id_empresa_propria);
        Movimento getMovimentoById(int cd_movimento);
        bool postReenviarNFMasterSaf(int cd_movimento, int cdEscola, bool empresaPropria);
        IEnumerable<TipoNotaFiscal> getTipoNotaFiscalSearch(SearchParameters parametros, string desc, string natOp, bool inicio, bool? status, int movimento, bool? devolucao, int cdEscola, byte id_regime_trib, bool? id_servico);
        bool deleteAllTpNF(List<TipoNotaFiscal> tpsNF);
        TipoNotaFiscal postTpNF(TipoNotaFiscal tipo);
        TipoNotaFiscal putTpNF(TipoNotaFiscal tipo);
        TipoNotaFiscal getTipoNFById(int cdTpNF);
        byte getTipoMvtoTpNF(int cd_tipo_nota_fiscal);
        bool existeMovimentoTpNF(int cd_tipo_nota_fiscal);
        List<int> VerificaNFESemDataAutorizacao(int id_tipo_movimento, int cd_empresa, bool nota_fiscal);

        //CFOP
        CFOP getCFOPByTpNF(int cd_tipo_nota);
        IEnumerable<CFOP> searchCFOP(SearchParameters parametros, string descricao, bool inicio, int nm_CFOP, byte id_natureza_CFOP);

        //Dados NF
        DadosNF putDadosNF(DadosNF dado);
        bool deleteAllDadosNF(List<DadosNF> dados);

        //Aliquota NF
        bool deleteAllAliquotaUF(List<AliquotaUF> aliquotas);
        AliquotaUF putAliquotaUF(AliquotaUF aliquota);

        List<ContratoComboUI> getContratosSemTurmaByAlunoMovimentoSearch(int cd_aluno,
            bool semTurma, int situacaoTurma, int nmContrato, int tipo, int cdEscola, byte tipoC, bool? status);
        List<Movimento> getMovimentosbyOrigem(int cd_contrato, int id_origem_movto, int cd_escola);
        List<ItemMovimento> getItensMovimentoByCdMovimentoPerdaMaterial(int cdMovimento);
    }
}
