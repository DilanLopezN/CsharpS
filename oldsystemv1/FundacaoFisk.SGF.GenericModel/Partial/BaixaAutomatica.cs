using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class BaixaAutomatica : TO
    {
        public string no_usuario { get; set; }
        public string no_local_movto { get; set; }
        public string dc_cartao_movto { get; set; }
    }
}