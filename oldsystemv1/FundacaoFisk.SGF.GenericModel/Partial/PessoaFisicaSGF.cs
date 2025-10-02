using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class PessoaFisicaSGF
    {
         //Atributos de View:
        public string site {
            get;
            set;
        }
        public string email {
            get;
            set;
        }
        public string celular
        {
            get;
            set;
        }
        public string dtaNascimento
        {
            get {
                if(dt_nascimento != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_nascimento.Value);
                else
                    return String.Empty;
            }
        }
        public string dtaCasamento
        {
            get {
                if(dt_casamento != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_casamento.Value);
                else
                    return string.Empty;
            }
        }
        public string dtaEmisExpedidor
        {
            get {
                if(dt_emis_expedidor != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_emis_expedidor.Value);
                else
                    return string.Empty;
            }
        }

        public static PessoaFisicaSGF changeValuesPessoaFisica(PessoaFisicaSGF pessoaFiscContext, PessoaFisicaSGF pessoaFisicView)
        {

            if (pessoaFiscContext.cd_atividade_principal != pessoaFisicView.cd_atividade_principal)
                pessoaFiscContext.cd_atividade_principal = pessoaFisicView.cd_atividade_principal;
            if (pessoaFiscContext.cd_endereco_principal != pessoaFisicView.cd_endereco_principal)
                pessoaFiscContext.cd_endereco_principal = pessoaFisicView.cd_endereco_principal;
            if (pessoaFiscContext.cd_estado_civil != pessoaFisicView.cd_estado_civil)
                pessoaFiscContext.cd_estado_civil = pessoaFisicView.cd_estado_civil;
            if (pessoaFiscContext.cd_estado_expedidor != pessoaFisicView.cd_estado_expedidor)
                pessoaFiscContext.cd_estado_expedidor = pessoaFisicView.cd_estado_expedidor;
            if (pessoaFiscContext.cd_integracao_folha != pessoaFisicView.cd_integracao_folha)
                pessoaFiscContext.cd_integracao_folha = pessoaFisicView.cd_integracao_folha;
            if (pessoaFiscContext.cd_loc_nacionalidade != pessoaFisicView.cd_loc_nacionalidade)
                pessoaFiscContext.cd_loc_nacionalidade = pessoaFisicView.cd_loc_nacionalidade;
            if (pessoaFiscContext.cd_loc_nascimento != pessoaFisicView.cd_loc_nascimento)
                pessoaFiscContext.cd_loc_nascimento = pessoaFisicView.cd_loc_nascimento;
            if (pessoaFiscContext.cd_orgao_expedidor != pessoaFisicView.cd_orgao_expedidor)
                pessoaFiscContext.cd_orgao_expedidor = pessoaFisicView.cd_orgao_expedidor;
            if (pessoaFiscContext.cd_papel_principal != pessoaFisicView.cd_papel_principal)
                pessoaFiscContext.cd_papel_principal = pessoaFisicView.cd_papel_principal;
            if (pessoaFiscContext.cd_pessoa != pessoaFisicView.cd_pessoa)
                pessoaFiscContext.cd_pessoa = pessoaFisicView.cd_pessoa;

            if (pessoaFiscContext.cd_pessoa_cpf != pessoaFisicView.cd_pessoa_cpf)
            {
                int? cd_pessoa_cpf_view = pessoaFisicView.cd_pessoa_cpf != 0 ? pessoaFisicView.cd_pessoa_cpf : null;
                pessoaFiscContext.cd_pessoa_cpf = cd_pessoa_cpf_view;
            }
            if (pessoaFiscContext.cd_tratamento != pessoaFisicView.cd_tratamento)
                pessoaFiscContext.cd_tratamento = pessoaFisicView.cd_tratamento;
            if (pessoaFiscContext.dc_carteira_motorista != pessoaFisicView.dc_carteira_motorista)
                pessoaFiscContext.dc_carteira_motorista = pessoaFisicView.dc_carteira_motorista;
            if (pessoaFiscContext.dc_carteira_trabalho != pessoaFisicView.dc_carteira_trabalho)
                pessoaFiscContext.dc_carteira_trabalho = pessoaFisicView.dc_carteira_trabalho;
            if (pessoaFiscContext.dc_num_certidao_nascimento != pessoaFisicView.dc_num_certidao_nascimento)
                pessoaFiscContext.dc_num_certidao_nascimento = pessoaFisicView.dc_num_certidao_nascimento;
            if (pessoaFiscContext.dc_num_crc != pessoaFisicView.dc_num_crc)
                pessoaFiscContext.dc_num_crc = pessoaFisicView.dc_num_crc;
            if (pessoaFiscContext.dc_num_insc_inss != pessoaFisicView.dc_num_insc_inss)
                pessoaFiscContext.dc_num_insc_inss = pessoaFisicView.dc_num_insc_inss;
            if (pessoaFiscContext.dc_num_pessoa != pessoaFisicView.dc_num_pessoa)
                pessoaFiscContext.dc_num_pessoa = pessoaFisicView.dc_num_pessoa;
            if (pessoaFiscContext.dc_num_titulo_eleitor != pessoaFisicView.dc_num_titulo_eleitor)
                pessoaFiscContext.dc_num_titulo_eleitor = pessoaFisicView.dc_num_titulo_eleitor;
            if (pessoaFiscContext.dc_reduzido_pessoa != pessoaFisicView.dc_reduzido_pessoa)
                pessoaFiscContext.dc_reduzido_pessoa = pessoaFisicView.dc_reduzido_pessoa;
            if (pessoaFiscContext.dt_casamento != pessoaFisicView.dt_casamento)
                pessoaFiscContext.dt_casamento = pessoaFisicView.dt_casamento;
            if (pessoaFiscContext.dt_emis_expedidor != pessoaFisicView.dt_emis_expedidor)
                pessoaFiscContext.dt_emis_expedidor = pessoaFisicView.dt_emis_expedidor;
            if (pessoaFiscContext.dt_nascimento != pessoaFisicView.dt_nascimento)
                pessoaFiscContext.dt_nascimento = pessoaFisicView.dt_nascimento;
            if (pessoaFiscContext.dt_venc_habilitacao != pessoaFisicView.dt_venc_habilitacao)
                pessoaFiscContext.dt_venc_habilitacao = pessoaFisicView.dt_venc_habilitacao;
            if (pessoaFiscContext.nm_sexo != pessoaFisicView.nm_sexo)
                pessoaFiscContext.nm_sexo = pessoaFisicView.nm_sexo;
            if (pessoaFiscContext.no_pessoa != pessoaFisicView.no_pessoa)
                pessoaFiscContext.no_pessoa = pessoaFisicView.no_pessoa;
            if (pessoaFiscContext.nm_cpf != pessoaFisicView.nm_cpf)
                pessoaFiscContext.nm_cpf = pessoaFisicView.nm_cpf;
            if (pessoaFiscContext.nm_doc_identidade != pessoaFisicView.nm_doc_identidade)
                pessoaFiscContext.nm_doc_identidade = pessoaFisicView.nm_doc_identidade;

            //pessoaFiscContext.dt_cadastramento = pessoaFisicView.dt_cadastramento;

            if (!string.IsNullOrEmpty(pessoaFisicView.ext_img_pessoa))
            {
                if (!string.IsNullOrEmpty(pessoaFiscContext.ext_img_pessoa))
                {
                    if (pessoaFiscContext.ext_img_pessoa != pessoaFisicView.ext_img_pessoa && pessoaFisicView.img_pessoa != null && pessoaFisicView.img_pessoa.Count() > 0)
                    {
                        pessoaFiscContext.ext_img_pessoa = pessoaFisicView.ext_img_pessoa;
                        pessoaFiscContext.img_pessoa = pessoaFisicView.img_pessoa;
                    }
                }
                else
                {
                    pessoaFiscContext.ext_img_pessoa = pessoaFisicView.ext_img_pessoa;
                    pessoaFiscContext.img_pessoa = pessoaFisicView.img_pessoa;
                }

            }
            else
            {
                if (pessoaFiscContext.ext_img_pessoa != null || (pessoaFiscContext.img_pessoa != null && pessoaFiscContext.img_pessoa.Count() > 0))
                {
                    pessoaFiscContext.img_pessoa = null;
                    pessoaFiscContext.ext_img_pessoa = "";
                }
            }
            if (pessoaFiscContext.txt_obs_pessoa != pessoaFisicView.txt_obs_pessoa)
                pessoaFiscContext.txt_obs_pessoa = pessoaFisicView.txt_obs_pessoa;

            //pessoaFiscContext.nm_doc_identidade = pessoaFisicView.nm_doc_identidade;
            return pessoaFiscContext;
        }

        public static PessoaFisicaSGF changeValuesPessoaFisicaProspect(PessoaFisicaSGF pessoaFiscContext, PessoaFisicaSGF pessoaFisicView, EnderecoSGF enderecoSgf)
        {
            if (pessoaFiscContext.cd_pessoa != pessoaFisicView.cd_pessoa)
                pessoaFiscContext.cd_pessoa = pessoaFisicView.cd_pessoa;
            if (pessoaFiscContext.nm_sexo != pessoaFisicView.nm_sexo)
                pessoaFiscContext.nm_sexo = pessoaFisicView.nm_sexo;
            if (pessoaFiscContext.no_pessoa != pessoaFisicView.no_pessoa)
                pessoaFiscContext.no_pessoa = pessoaFisicView.no_pessoa;
            if (pessoaFiscContext.dt_nascimento != pessoaFisicView.dt_nascimento)
                pessoaFiscContext.dt_nascimento = pessoaFisicView.dt_nascimento;
            if (enderecoSgf != null)
            {
                pessoaFiscContext.cd_endereco_principal = enderecoSgf.cd_endereco;
            }
           
            return pessoaFiscContext;
        }
    }
}
