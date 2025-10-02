using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public class PessoaJurdicaSearchUI
    {
        public PessoaJuridicaSGF pessoaJuridica { get; set; }
        public ContatosUI contatosUI { get; set; }
        public IEnumerable<RelacionamentoUI> relacionamentoUI { get; set; }

        public static PessoaSearchUI fromPessoaForPessoaSearchUI(PessoaJuridicaSGF pessoaJuridicaSGF)
        {
            PessoaSearchUI pessoaSearchUI = new PessoaSearchUI
            {
                cd_pessoa = pessoaJuridicaSGF.cd_pessoa,
                no_pessoa = pessoaJuridicaSGF.no_pessoa,
                dc_num_pessoa = pessoaJuridicaSGF.dc_num_pessoa,
                nm_natureza_pessoa = pessoaJuridicaSGF.nm_natureza_pessoa,
                dc_reduzido_pessoa = pessoaJuridicaSGF.dc_reduzido_pessoa,
                dt_cadastramento = pessoaJuridicaSGF.dt_cadastramento,
                id_pessoa_empresa = pessoaJuridicaSGF.id_pessoa_empresa,
                nm_cpf_cgc = pessoaJuridicaSGF.dc_num_cgc,
                img_pessoa = pessoaJuridicaSGF.img_pessoa,
                ext_img_pessoa = pessoaJuridicaSGF.ext_img_pessoa
            };
            return pessoaSearchUI;
        }

    }

}
