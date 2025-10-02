using System;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericController;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.CNAB.DataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.CNAB.Comum.IBusiness
{
    public interface ICnabBusiness : IGenericBusiness
    {
        //Cnab
        IEnumerable<Cnab> searchCnab(SearchParameters parametros, int cd_carteira, int cd_usuario, byte tipo_cnab, int status, DateTime? dtInicial,
                                                 DateTime? dtFinal, bool emissao, bool vencimento, string nossoNumero, int? nro_contrato, int cd_empresa,
                                                bool icnab, bool iboleto, int cd_responsavel, int cd_aluno);
        IEnumerable<UsuarioWebSGF> getUsuariosCnab(int cd_empresa, bool adm, int cd_usuario);
        void verificarGerouCnab(int cd_escola, Int32[] cd_cnab, Int32[] tipos_cnab, byte status_cnab, bool is_titulo, ReturnResult retornoErrors);
        Cnab postGerarCnab(int cd_escola, int cd_cnab);
        Cnab postCancelarCnab(int cd_escola, int cd_cnab);
        Cnab postGerarPedidoBaixaCnab(int cd_escola, int cd_cnab);
        Cnab addCnab(Cnab cnab, int cd_empresa);
        Cnab editCnab(Cnab cnab, int cd_empresa);
        Cnab getCnabEditView(int cd_cnab, int cdEscola);
        bool deleteCnabs(int[] cdCnabs,int cd_escola);
        bool deleteCnabsRegistrados(Cnab cnab, int cd_escola, bool masterGeral);
        TituloCnab getTituloCnabEditView(int cd_cnab,int cd_titulo_cnab, int cdEmpresa);
        Cnab getCnabByRemessa(int cd_escola, int cd_cnab);
        void verificarCdContratoCnab(int cd_escola, int cd_cnab, ReturnResult retornoErrors);

        //Carteira Cnab
        IEnumerable<CarteiraCnab> getCarteiraCnabSearch(SearchParameters parametros, string nome, bool inicio, int banco, bool? status);
        CarteiraCnab postInsertCarteira(CarteiraCnab carteiraCnab);
        CarteiraCnab putCarteira(CarteiraCnab carteiraCnab);
        bool deleteAllCarteira(List<CarteiraCnab> carteiras);
        IEnumerable<CarteiraCnab> getCarteiraByBanco(int? localMovto, int banco);
        IEnumerable<CarteiraCnab> getCarteirasCnab(int cd_empresa);
        IEnumerable<CarteiraCnab> getCarteirasCnab(int cdEscola, int cd_carteira_cnab, CarteiraCnabDataAccess.TipoConsultaCarteiraCnab tipoConsulta);
        Cnab getGerarRemessa(int cd_escola, int cd_cnab);

        void verificarCarteiraRegistrada(int cd_escola, int cd_cnab, int cd_carteira_cnab, ReturnResult retornoErrors);

        //RetornoCNAB
        IEnumerable<RetornoCNAB> searchRetornoCNAB(SearchParameters parametros, int cd_carteira, int cd_usuario, int status, string descRetorno, DateTime? dtInicial, DateTime? dtFinal, string nossoNumero, int cd_empresa, int cd_responsavel, int cd_aluno);
        IEnumerable<UsuarioWebSGF> getUsuariosRetCNAB(int cd_empresa);
        IEnumerable<CarteiraCnab> getCarteirasRetCNAB(int cd_empresa);
        RetornoCNAB addRetornoCNAB(RetornoCNAB retornoCNAB, string pathRetornosEscola, int cd_empresa, bool masterGeral);
        RetornoCNAB getRetornoCnabFull(int cd_retorno, int cd_escola);
        RetornoCNAB getRetCnabEditView(int cd_cnab, int cdEscola);
        void postProcessarRetornos(RetornoCNAB retornoCNAB);
        RetornoCNAB editRetornoCNAB(RetornoCNAB retornoCNAB, string pathContratosEscola, int cd_empresa);
        int buscarTipoCNAB(int cd_retorno_cnab, int cd_pessoa_empresa);
        List<TituloRetornoCNAB> searchTituloCnabGradeRet(TituloUI titulo);
        bool deleteRetornosCnabs(int[] cdCnabs, int cd_escola, string pathRetornosEscola);
        bool deleteCnabRetornosProcessados(int[] cnabs, int cd_escola, string pathRetornosEscola, bool isSupervisor, int movimentoRetroativo, bool masterGeral);
        List<TituloRetornoCNAB> getTituloRetornoCNAB(int cd_retorno_cnab);
        List<TituloCnab> getTituloCNAB(int cd_cnab);
        TituloRetornoCNAB getTituloRetornoCnabEditView(int cd_titulo_cnab, int cd_empresa);

        //Local de Movimento
        LocalMovto findLocalMovtoComCarteira(int cdEscola, int cdLocalMovto);
    }
}
