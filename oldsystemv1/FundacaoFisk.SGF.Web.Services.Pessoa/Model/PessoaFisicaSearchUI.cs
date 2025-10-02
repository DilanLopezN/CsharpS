using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public class PessoaFisicaSearchUI
    {
        public PessoaFisicaSGF pessoaFisica { get; set; }
        public ContatosUI contatosUI { get; set; }
        public IEnumerable<EnderecoSGF> enderecos { get; set; }
        public IEnumerable<RelacionamentoUI> relacionamentoUI { get; set; }
        public string no_pessoa_cpf { get; set; }
    }
}
