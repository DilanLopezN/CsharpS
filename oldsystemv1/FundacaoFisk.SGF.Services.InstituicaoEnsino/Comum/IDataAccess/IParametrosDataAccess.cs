using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IDataAccess
{
    public interface IParametrosDataAccess : IGenericRepository<Parametro>
    {
        Parametro getParametrosByEscola(int cdEscola);
        Parametro getParametrosMatricula(int cdEscola);
        Parametro getParametrosBaixa(int cd_escola);
        int? getLocalMovto(int cd_escola);
        Parametro getParametrosBiblioteca(int cd_empresa);
        Parametro getParametrosOpcaoPagamento(int cd_escola);
        Parametro getParametrosMovimento(int cd_escola);
        bool getIdBloquearVendasSemEstoque(int cd_empresa);
        bool getIdBloquearliqTituloAnteriorAberto(int cd_empresa);
        byte? getParametroNiviesPlanoConta(int cdEscola);
        int? getParametroNiveisPlanoContas(int cd_escola);
        int getParametroMovimentoRetroativo(int cd_escola);
        bool getParametroBloquearMovtoRetroativoEst(int cd_escola);
        List<Parametro> getParametroNiviesPlanoEscola(int[] cdEscolas);
        byte? getParametrosPrevDevolucao(int cd_escola);
        bool getParametroNumeracaoAutoNF(int cd_escola);
        Parametro getNumeroESerieNFPorTipo(Movimento.TipoMovimentoEnum tipo, int cd_emprea);
        Parametro getParametrosPlanoTxMatricula(int cdEscola);
        byte? getParametroNmAulasSemMaterial(int cd_escola);
        bool getEmitirNFServico(int cd_empresa);
        byte getParametroRegimeTrib(int cd_escola);
        bool existeParametroTpNF(int cd_tipo_nf);
        int getParametroNmFaltasAluno(int cd_escola);
        bool getImprimir3BoletosPagina(int cd_escola);
        byte? getTipoNumeroContrato(int cd_empresa);
        byte getParametroNmDiasTitulosAbertos(int cd_escola);
        bool getParametroHabilitacaoProfessor(int cd_escola);
    }
}
