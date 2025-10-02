﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Comum
{
    public interface IUsuarioEmpresaDataAccess : IGenericRepository<UsuarioEmpresaSGF>
    {
        IEnumerable<UsuarioEmpresaSGF> findAllUsuarioEmpresaByUsuario(int cd_usuario);
        List<int> findAllUsuarioEmpresaByUsuarioIds(int cd_usuario);
    }
}
