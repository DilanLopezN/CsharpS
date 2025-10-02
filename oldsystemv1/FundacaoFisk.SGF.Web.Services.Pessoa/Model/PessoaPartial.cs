using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public partial class PessoaSearchUI
    {
        public string dta_cadastro
         {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_cadastramento);
            }
        }


        public string natureza_pessoa
        {
            get
            {
                return this.nm_natureza_pessoa == 1 ? "Física" : "Jurídica";
            }
        }
    }
}
