using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface IMovimentoDataAccess : IGenericRepository<Movimento>
    {
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
                                                           Movimento.TipoVinculoMovimento tipoVinc, bool comEstoque, int? id_natureza_TPNF, bool kit, bool contaSegura, int? cd_movimento, int? cd_aluno, DateTime? dt_inicial, DateTime? dt_final, bool? vinculado_curso, int? cd_curso_material_didatico);
        IEnumerable<ItemUI> getItemMovimentoSearchPerdaMaterial(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int? tipoItem, int? cdGrupoItem, int cdEscola, int id_tipo_movto,
            Movimento.TipoVinculoMovimento tipoVinc, bool comEstoque, int? id_natureza_TPNF, bool kit, bool contaSegura, int? cd_movimento, int origem, int? cd_aluno, DateTime? dt_inicial, DateTime? dt_final, bool? vinculado_curso, int? cd_curso_material_didatico);
        MovimentoUI getMovimentoReturnGrade(int cd_movimento, int cd_empresa);
        IEnumerable<Movimento> getMovimentos(int[] cdMvotos, int cd_empresa);
        Movimento getMovimentoEditView(int cd_movimento, int cd_empresa);
        Movimento getMovimentoEditViewNF(int cd_movimento, int cd_empresa, int id_tipo_movimento);
        Movimento getMovimento(int cd_movimento, int cd_empresa);
        Movimento getMovimentoComCheque(int cd_movimento, int cd_empresa);
        int? getMaxMovimento(int tpMovto, int cd_empresa);
        Espelho getEspelhoMovimento(int cd_movimento, int cd_empresa);
        List<Espelho> getSourceCopiaEspelhoMovimento(int cd_movimento, int cd_empresa);
        int getUltimoNroRecibo(int? nm_ultimo_Recibo, int cd_empresa);
        bool existeDadosNFNota(int cdCidade);
        bool existeICMSEstadoNota(int cdEstadoOri, int cdEstadoDes);
        Movimento getMovimentoEditOrigem(int cd_origem, int id_origem_movto, int cd_empresa, int id_tipo_movimento);
        bool existeMovimentoByOrigem(int cdOrigem, int id_origem_movimento);
        List<Movimento> getMovimentosNotaServico(List<int> cd_movimentos);
        Movimento getMovimentosNotaProduto(int cd_movimento);
        bool existeTituloBaixadoByMovimento(int cd_movimento);
        bool existeMovimentoForTit(int cd_pessoa_empresa, List<int> titulos);
        bool existeMovimentoTpNF(int cd_tipo_nota_fiscal);
        bool existeItemNoMovimento(int cd_movimento);
        bool existeItemZeradoNoMovimento(int cd_movimento);
        bool existeMovimentoComDataSuperior(int id_tipo_movimento, DateTime dta_movimento, int cd_escola);
        bool existeMovimentosAbertosComDataAnterior(int id_tipo_movimento, DateTime dta_movimento, int cd_escola);
        Movimento getRetMovimentoDevolucao(int cd_movimento, int cd_empresa, int id_tipo_movimento);
        bool spEnviarMasterSaf(int? cd_movimento);
        bool existeMovimentoEscola(int cd_movimento, int cd_escola);
        bool notaFiscalComNFS(int id_tipo_movimento, int cd_movimento, int cd_escola);
        bool notaFiscalDevolucaoComProtocolo(int id_tipo_movimento, int cd_movimento, int cd_escola);
        bool notaFiscalDevolvidaComChaveAcesso(int id_tipo_movimento, int cd_movimento, int cd_escola);
        bool notaFiscalComProtocolo(int id_tipo_movimento, int cd_movimento, int cd_escola);
        bool verificaSeItensNotaDevolucaoExtrapolouNotaOrig(int cd_movimento_dev, int cd_movimento, int cd_escola);
        List<int> VerificaNFESemDataAutorizacao(int id_tipo_movimento, int cd_empresa, bool nota_fiscal);
        Movimento getMovimentoWithItens(int cd_movimento, int cd_empresa);
        string getMovimentoWithItensName(int cd_movimento, int cd_empresa);

        // Relatorio Cont Vendas Material
        IEnumerable<RptContVendasMaterial> getRptContVendasMaterial(int cd_escola, int cd_aluno, int cd_item, DateTime dt_inicial, DateTime dt_final, int cd_turma, bool semmaterial);
		bool verificaTipoFinanceiroMovimento(int cd_titulo, string dc_tipo_titulo, int cd_pessoa_empresa);

        IEnumerable<ContratoComboUI> getContratosSemTurmaByAlunoMovimentoSearch(int cd_aluno,
            bool semTurma, int situacaoTurma, int nmContrato, int tipo, int cdEscola, byte tipoC, bool? status);
        int findNotaAluno(int cd_aluno, int cd_curso);
        List<Movimento> getMovimentosbyOrigem(int cd_origem, int id_origem_movto, int cd_empresa);
        IEnumerable<ItemMovimento> getItensMovimentoByCdMovimento(int cd_movimento);
    }
}