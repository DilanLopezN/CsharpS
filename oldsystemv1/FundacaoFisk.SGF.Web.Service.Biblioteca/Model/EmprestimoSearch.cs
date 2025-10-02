using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Service.Biblioteca.Model {
    public class EmprestimoSearch : TO {
        public int cd_biblioteca { get; set; }
        public int cd_pessoa { get; set; }
        public int cd_item { get; set; }
        public System.DateTime dt_emprestimo { get; set; }
        public System.DateTime dt_prevista_devolucao { get; set; }
        public Nullable<System.DateTime> dt_devolucao { get; set; }
        public decimal vl_taxa_emprestimo { get; set; }
        public Nullable<decimal> vl_multa_emprestimo { get; set; }
        public string no_pessoa { get; set; }
        public string no_item { get; set; }
        public int cd_plano_conta { get; set; }
        public string desc_plano_conta { get; set; }
    }
}
