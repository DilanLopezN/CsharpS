using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    using Componentes.GenericModel;
    public partial class PessoaSGF : TO
    {
        public enum TipoRelatorioAniversariantes
        {
            ALUNOS_ATIVOS = 1,
            ALUNOS_DESISTENTES = 2,
            EX_ALUNOS = 3,
            CLIENTES = 4,
            FUNCIONÁRIOS_PROFESSOR = 5
        }
        public enum TipoPessoa { FISICA = 1, JURIDICA = 2, TODOS = 0 }
        public enum Sexo { MASCULINO = 2, FEMININO = 1, NAO_BINARIO = 3, PREFIRO_NAO_RESPONDER_NEUTRO = 4 }
        public enum TipoPesquisa { TODAS_PESSOAS = 0, FILTRAR_EMPRESA = 1 }

        //Propriedades de tela:
        public bool ehSelecionado = false;

        public string nm_cpf_cgc { get; set; }
        public string nm_doc_identidade { get; set; }
        public string dta_cadastro
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_cadastramento);
            }
        }
        private string _hr_cadastro;
        public string hr_cadastro
        {
            get
            {
                if(_hr_cadastro == null)
                    return String.Format("{0:HH:mm:ss}", dt_cadastramento);
                else
                    return _hr_cadastro;
            }
            set {
                _hr_cadastro = value;
            }
        }

        public string natureza_pessoa
        {
            get
            {
                return this.nm_natureza_pessoa == 1 ? "Física" : "Jurídica";
            }
        }

        public bool ehImgUpload { get; set; }

        public string beneficiario_cnab{
            get{
                return this.no_pessoa + " - " + this.nm_cpf_cgc;
            }
        }
        
        public static PessoaFisicaSGF castPessoaFisicaComponetnesToPessoaFisicaSGF(PessoaFisicaSGF pessoafisica)
        {
            PessoaFisicaSGF pessoaFisSgf = new PessoaFisicaSGF();
            pessoaFisSgf.cd_pessoa = pessoafisica.cd_pessoa;
            pessoaFisSgf.cd_pessoa_cpf = pessoafisica.cd_pessoa_cpf;
            pessoaFisSgf.cd_telefone_principal = pessoafisica.cd_telefone_principal;
            pessoaFisSgf.cd_tratamento = pessoafisica.cd_tratamento;
            pessoaFisSgf.dc_carteira_motorista = pessoafisica.dc_carteira_motorista;
            pessoaFisSgf.dc_carteira_trabalho = pessoafisica.dc_carteira_trabalho;
            pessoaFisSgf.dc_num_certidao_nascimento = pessoafisica.dc_num_certidao_nascimento;
            pessoaFisSgf.dc_num_crc = pessoafisica.dc_num_crc;
            pessoaFisSgf.dc_num_insc_inss = pessoafisica.dc_num_insc_inss;
            pessoaFisSgf.dc_num_pessoa = pessoafisica.dc_num_pessoa;
            pessoaFisSgf.dc_num_titulo_eleitor =  pessoafisica.dc_num_titulo_eleitor;
            pessoaFisSgf.dc_reduzido_pessoa = pessoafisica.dc_reduzido_pessoa;
            pessoaFisSgf.dt_cadastramento = pessoafisica.dt_cadastramento;
            pessoaFisSgf.dt_casamento = pessoafisica.dt_casamento;         
            pessoaFisSgf.dt_emis_expedidor = pessoafisica.dt_emis_expedidor;
            pessoaFisSgf.dt_nascimento = pessoafisica.dt_nascimento =
            pessoaFisSgf.dt_venc_habilitacao = pessoafisica.dt_venc_habilitacao;
            pessoaFisSgf.ext_img_pessoa = pessoafisica.ext_img_pessoa;
            pessoaFisSgf.id_exportado = pessoafisica.id_exportado;
            pessoaFisSgf.id_pessoa_empresa = pessoafisica.id_pessoa_empresa;
            pessoaFisSgf.img_pessoa = pessoafisica.img_pessoa;
            pessoaFisSgf.nm_cpf = pessoafisica.nm_cpf;
            pessoaFisSgf.nm_doc_identidade = pessoafisica.nm_doc_identidade;
            pessoaFisSgf.nm_natureza_pessoa = pessoafisica.nm_natureza_pessoa;
            pessoaFisSgf.nm_sexo = pessoafisica.nm_sexo;
            pessoaFisSgf.no_pessoa = pessoafisica.no_pessoa;
            return pessoaFisSgf;
        }

        //Propriedades para emissão de nota fiscal de produto:
        public string dc_num_insc_estadual_PJ { get; set; }

        //Propriedade parra emissão do boleto no nome do aluno:
        public string no_aluno { get; set; }
    }
}
