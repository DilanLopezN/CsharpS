using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public class PessoaFisicaUI
    {
        public PessoaFisicaSGF pessoaFisica { get; set; }
        public EnderecoSGF endereco { get; set; }
        public String telefone { get; set; }
        public int? cd_operadora { get; set; }
        public String email { get; set; }
        public String site { get; set; }
        public String celular { get; set; }
        public ICollection<EnderecoSGF> enderecos { get; set; }
        public ICollection<TelefoneSGF> telefones { get; set; }
        public String descFoto { get; set; }
        public ICollection<RelacionamentoUI> relacionamentosUI { get; set; }
    }
}