using System;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericBusiness.Comum;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.CNAB.Comum.IBusiness
{
    using Componentes.GenericModel;
    public interface IBoletoBusiness : IGenericBusiness
    {
        IEnumerable<TituloCnab> getTitulosCnabBoletoByCnabs(int cd_escola, Int32[] cd_cnab, Int32[] cd_titulos_cnab);
        IEnumerable<TituloCnab> getTitulosCnabBoletoByTitulosCnab(int cd_escola, Int32[] cd_cnab);
    }
}
