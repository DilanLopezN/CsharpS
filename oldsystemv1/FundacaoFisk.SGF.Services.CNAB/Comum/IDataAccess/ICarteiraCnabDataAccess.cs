using System;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.CNAB.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.CNAB.Comum.IDataAccess
{
    public interface ICarteiraCnabDataAccess : IGenericRepository<CarteiraCnab>
    {
        IEnumerable<CarteiraCnab> getCarteiraCnabSearch(SearchParameters parametros, string nome, bool inicio, int banco, bool? status);
        IEnumerable<CarteiraCnab> getCarteiraByBanco(int? localMovto, int banco);
        IEnumerable<CarteiraCnab> getCarteirasCnab(int cdEscola, int cd_carteira_cnab,CarteiraCnabDataAccess.TipoConsultaCarteiraCnab tipoConsulta);
        CarteiraCnab getCarteiraByCarteira(int cd_carteira_cnab);
    }
}
