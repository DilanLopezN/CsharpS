using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public class PessoaSearchComboUI : TO
    {
        public int cd_pessoa { get; set; }
        public string no_pessoa { get; set; }
        public string dc_reduzido_pessoa { get; set; }
    }
}