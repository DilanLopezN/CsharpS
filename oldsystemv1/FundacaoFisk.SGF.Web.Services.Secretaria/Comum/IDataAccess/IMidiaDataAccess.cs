using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum
{
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
    public interface IMidiaDataAccess : IGenericRepository<Midia>
    {
        //Mídia
        IEnumerable<Midia> GetMidiaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        IEnumerable<Midia> getMidias(bool? status, MidiaDataAccess.TipoConsultaMidiaEnum tipo, int? cd_empresa);
        bool deleteAll(List<Midia> midias);
    }
}
