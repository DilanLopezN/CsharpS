using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class FuncionarioComissao
    {
        public string no_professor { get; set; }
        public string no_produto { get; set; }
        public int qtd_matriculas { get; set; }
        public int qtd_rematriculas { get; set; }
        public string vl_total_geral { get; set; }
    }
}
    