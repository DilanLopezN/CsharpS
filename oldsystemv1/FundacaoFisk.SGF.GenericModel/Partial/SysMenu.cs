using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    partial class SysMenu
    {
        public enum TipoSysMenu
        {
            CONFIGURACAO = 1,
            ALTERARSENHA = 2,
            PESSOARELACIONADA = 3,
            USUARIO = 4,
            GRUPO = 24,
            EMPRESA = 36
        }
    }
}
