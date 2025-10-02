using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public class st_RptEtiqueta_Result : TO
    {
        public string no_pessoa { get; set; }
        public string dc_endereco { get; set; }
        public string dc_bairro { get; set; }
        public string dc_cidade { get; set; }
        public int no_ordem { get; set; }
    }
}
