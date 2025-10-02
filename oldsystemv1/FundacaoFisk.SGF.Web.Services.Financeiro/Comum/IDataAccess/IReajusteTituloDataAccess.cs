﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
    public interface IReajusteTituloDataAccess : IGenericRepository<ReajusteTitulo>
    {
        IEnumerable<ReajusteTitulo> getReajusteTitulos(int cd_empresa, int cd_reajuste_anual);
    }
}
