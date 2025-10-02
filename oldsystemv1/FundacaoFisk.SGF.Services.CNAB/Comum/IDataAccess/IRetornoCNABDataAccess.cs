using System;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.CNAB.Comum.IDataAccess
{
    public interface IRetornoCNABDataAccess : IGenericRepository<RetornoCNAB>
    {
        IEnumerable<RetornoCNAB> searchRetornoCNAB(SearchParameters parametros, int cd_carteira, int cd_usuario, int status, string descRetorno, DateTime? dtInicial,
                                                 DateTime? dtFinal, string nossoNumero, int cd_empresa, int cd_responsavel, int cd_aluno);
        IEnumerable<UsuarioWebSGF> getUsuariosRetCNAB(int cd_empresa);
        IEnumerable<CarteiraCnab> getCarteirasRetCNAB(int cd_empresa);
        RetornoCNAB getRetornoCnabFull(int cd_retorno, int cd_escola);
        RetornoCNAB getRetornoCnabEditView(int cd_retorno, int cdEscola);
        RetornoCNAB getRetornoCnabReturnGrade(int cd_retorno, int cd_empresa);
        IEnumerable<RetornoCNAB> getRetornsoCNAB(int[] cdRetornosCnab, int cd_empresa);
        bool verificaStatusRetorno(int cd_retorno_cnab, int cd_escola, int status);
        int buscarTipoCNAB(int cd_retorno_cnab, int cd_pessoa_empresa);
        IEnumerable<RetornoCNAB> getRetornosCnabs(int[] cdCnabs, int cd_empresa);
        IEnumerable<TituloRetornoCNAB> getTitulosRetornoCNAB(int[] cdCnabs, int cd_empresa);
    }
}
