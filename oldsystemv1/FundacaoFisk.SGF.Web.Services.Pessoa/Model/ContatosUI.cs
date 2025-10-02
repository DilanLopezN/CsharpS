using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public class ContatosUI
    {
        public IEnumerable<TelefoneUI> outrosContatos { get; set; }
        public IEnumerable<TelefoneUI> contatosPrincipais { get; set; }
    }
}
