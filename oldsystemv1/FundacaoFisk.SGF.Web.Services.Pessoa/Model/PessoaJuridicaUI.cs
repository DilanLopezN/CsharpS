using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public class PessoaJuridicaUI
    {
        public PessoaJuridicaSGF pessoaJuridica { get; set; }
        public EnderecoSGF endereco { get; set; }
        public String telefone { get; set; }
        public String email { get; set; }
        public String site { get; set; }
        public String celular { get; set; }
        public List<EnderecoSGF> enderecos { get; set; }
        public List<TelefoneSGF> telefones { get; set; }
        public String descFoto { get; set; }
        public int? cd_operadora {get; set;}
        public ICollection<RelacionamentoUI> relacionamentosUI { get; set; }
    }
}
