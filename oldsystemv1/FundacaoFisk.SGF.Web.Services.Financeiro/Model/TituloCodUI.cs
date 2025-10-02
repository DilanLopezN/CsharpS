using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    internal class TituloCodUI : TO
    {
        public int cd_titulo { get; set; }
        public int? nm_titulo { get; set; }
    }
}
