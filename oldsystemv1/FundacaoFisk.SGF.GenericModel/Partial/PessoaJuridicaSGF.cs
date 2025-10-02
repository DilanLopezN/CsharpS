using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class PessoaJuridicaSGF
    {
        public string dtaBaixa
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_baixa);
            }
        }
        public string dtaRJuntaComer
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_registro_junta_comercial);
            }
        }

        public static PessoaJuridicaSGF ChangeValuesPessoaJuridica(PessoaJuridicaSGF pessoaJuridicaContex, PessoaJuridicaSGF pessoaJuridicaView)
        {
            pessoaJuridicaContex.cd_atividade_principal = pessoaJuridicaView.cd_atividade_principal;
            pessoaJuridicaContex.cd_papel_principal = pessoaJuridicaView.cd_papel_principal;
            pessoaJuridicaContex.cd_pessoa = pessoaJuridicaView.cd_pessoa;
            //pessoaJuridicaContex.cd_telefone_principal = pessoaJuridicaView.cd_telefone_principal;
            pessoaJuridicaContex.cd_tipo_sociedade = pessoaJuridicaView.cd_tipo_sociedade;
            pessoaJuridicaContex.dc_num_cgc = pessoaJuridicaView.dc_num_cgc;
            pessoaJuridicaContex.dc_num_cnpj_cnab = pessoaJuridicaView.dc_num_cnpj_cnab;
            pessoaJuridicaContex.dc_num_insc_estadual = pessoaJuridicaView.dc_num_insc_estadual;
            pessoaJuridicaContex.dc_num_insc_municipal = pessoaJuridicaView.dc_num_insc_municipal;
            pessoaJuridicaContex.dc_reduzido_pessoa = pessoaJuridicaView.dc_reduzido_pessoa;
            pessoaJuridicaContex.dc_registro_junta_comercial = pessoaJuridicaView.dc_registro_junta_comercial;
            pessoaJuridicaContex.dt_baixa = pessoaJuridicaView.dt_baixa;
            pessoaJuridicaContex.dt_registro_junta_comercial = pessoaJuridicaView.dt_registro_junta_comercial;
            pessoaJuridicaContex.no_pessoa = pessoaJuridicaView.no_pessoa;
            pessoaJuridicaContex.txt_obs_pessoa = pessoaJuridicaView.txt_obs_pessoa;
            
            if (!string.IsNullOrEmpty(pessoaJuridicaView.ext_img_pessoa))
                if (!string.IsNullOrEmpty(pessoaJuridicaContex.ext_img_pessoa))
                {
                    if (pessoaJuridicaContex.ext_img_pessoa != pessoaJuridicaView.ext_img_pessoa && pessoaJuridicaView.img_pessoa != null && pessoaJuridicaView.img_pessoa.Count() > 0)
                    {
                        pessoaJuridicaContex.ext_img_pessoa = pessoaJuridicaView.ext_img_pessoa;
                        pessoaJuridicaContex.img_pessoa = pessoaJuridicaView.img_pessoa;
                    }
                }
                else
                {
                    pessoaJuridicaContex.ext_img_pessoa = pessoaJuridicaView.ext_img_pessoa;
                    pessoaJuridicaContex.img_pessoa = pessoaJuridicaView.img_pessoa;
                }
            else
            {
                if (pessoaJuridicaContex.ext_img_pessoa != null || (pessoaJuridicaContex.img_pessoa != null && pessoaJuridicaContex.img_pessoa.Count() > 0))
                {
                    pessoaJuridicaContex.img_pessoa = null;
                    pessoaJuridicaContex.ext_img_pessoa = "";
                }
            }
            return pessoaJuridicaContex;
        }
    }
}
