using System;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.CNAB.Comum.IDataAccess
{
    public interface ITituloRetornoCnabDataAccess : IGenericRepository<TituloRetornoCNAB>
    {
        List<TituloRetornoCNAB> searchTituloCnabGradeRet(TituloUI titulo);
        List<TituloRetornoCNAB> getTituloRetornoCNAB(int cd_retorno_cnab);
        List<TituloCnab> getTituloCNAB(int cd_cnab);
        TituloRetornoCNAB getTituloRetornoCnabEditView(int cd_titulo_cnab, int cd_empresa);
    }
}
