using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public partial class PessoaSearchUI : TO
    {
        public PessoaSearchUI (){}

        public int cd_pessoa { get; set; }
        public string no_pessoa { get; set; }
        public System.DateTime dt_cadastramento { get; set; }
        public string dc_reduzido_pessoa { get; set; }
        public Nullable<int> nm_atividade_principal { get; set; }
        public Nullable<int> nm_endereco_principal { get; set; }
        public Nullable<int> nm_telefone_principal { get; set; }
        public bool id_pessoa_empresa { get; set; }
        public Nullable<byte> nm_natureza_pessoa { get; set; }
        public string dc_num_pessoa { get; set; }
        public Nullable<bool> id_exportado { get; set; }
        public Nullable<int> cd_papel_principal { get; set; }
        public string nm_cpf_cgc { get; set; }
        public byte[] img_pessoa { get; set; }
        public string ext_img_pessoa { get; set; }
        public string no_pessoa_dependente { get; set; }
        public string txt_obs_pessoa { get; set; }
        public string papel { get; set; }
        public IEnumerable<PapelSGF> papeisFilhos { get; set; }
        public IEnumerable<PapelSGF> papeisPai { get; set; }
        public ICollection<RelacionamentoSGF> relacionamentos { get; set; }
        public Nullable<byte> nm_sexo { get; set; }
        public string email { get; set; }
        public bool existeAluno { get; set; }
        public int? cd_aluno { get; set; }
        public string telefone { get; set; }

        public string nm_cpf_cgc_dependente
        {
            get
            {
                string retorno = "";
                if (!String.IsNullOrEmpty(nm_cpf_cgc)){
                    if (!string.IsNullOrEmpty(no_pessoa_dependente))
                    retorno = nm_cpf_cgc + "("+ no_pessoa_dependente+ ")" ;
                    else
                        retorno = nm_cpf_cgc;
                }
                return retorno;
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                // retorno.Add(new DefinicaoRelatorio("cd_pessoa", "Código", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("no_pessoa", "Nome", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("dc_reduzido_pessoa", "Nome Reduzido"));
                retorno.Add(new DefinicaoRelatorio("nm_cpf_cgc", "CPF\\CNPJ", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("pessoa_ativa", "Ativo", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("natureza_pessoa", "Natureza", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("dta_cadastro", "Data Cadastro"));

                return retorno;
            }
        }

        public static PessoaSearchUI fromPessoaForPessoaSearchUI(PessoaJuridicaSGF pessoaJuridicaSGF)
        {
            PessoaSearchUI pessoaSearchUI = new PessoaSearchUI();

            pessoaSearchUI.cd_pessoa = pessoaJuridicaSGF.cd_pessoa;
            pessoaSearchUI.no_pessoa = pessoaJuridicaSGF.no_pessoa;
            pessoaSearchUI.dc_num_pessoa = pessoaJuridicaSGF.dc_num_pessoa;
            pessoaSearchUI.nm_natureza_pessoa = pessoaJuridicaSGF.nm_natureza_pessoa;
            pessoaSearchUI.dc_reduzido_pessoa = pessoaJuridicaSGF.dc_reduzido_pessoa;
            pessoaSearchUI.dt_cadastramento = pessoaJuridicaSGF.dt_cadastramento;
            pessoaSearchUI.id_pessoa_empresa = pessoaJuridicaSGF.id_pessoa_empresa;
            pessoaSearchUI.nm_cpf_cgc = pessoaJuridicaSGF.dc_num_cgc;
            pessoaSearchUI.img_pessoa = pessoaJuridicaSGF.img_pessoa;
            pessoaSearchUI.ext_img_pessoa = pessoaJuridicaSGF.ext_img_pessoa;
            if (pessoaJuridicaSGF.PessoaFilhoRelacionamento != null && pessoaJuridicaSGF.PessoaFilhoRelacionamento.Count() > 0)
            {
                List<PapelSGF> papeisFilhoRelac = new List<PapelSGF>();
                foreach (var rel in pessoaJuridicaSGF.PessoaFilhoRelacionamento)
                {
                    if (rel.RelacionamentoFilhoPapel != null)
                        papeisFilhoRelac.Add(new PapelSGF { no_papel = rel.RelacionamentoFilhoPapel.no_papel, nm_tipo_papel = rel.RelacionamentoFilhoPapel.nm_tipo_papel, cd_papel = rel.RelacionamentoFilhoPapel.cd_papel });
                }
                if (papeisFilhoRelac.Count() > 0)
                    pessoaSearchUI.papeisFilhos = papeisFilhoRelac;
                pessoaJuridicaSGF.PessoaFilhoRelacionamento = null;
            }
            if (pessoaJuridicaSGF.PessoaPaiRelacionamento != null && pessoaJuridicaSGF.PessoaPaiRelacionamento.Count() > 0)
            {
                List<PapelSGF> papeisPaiRelac = new List<PapelSGF>();
                foreach (var rel in pessoaJuridicaSGF.PessoaPaiRelacionamento)
                {
                    if (rel.RelacionamentoPaiPapel != null)
                        papeisPaiRelac.Add(new PapelSGF { no_papel = rel.RelacionamentoPaiPapel.no_papel, nm_tipo_papel = rel.RelacionamentoPaiPapel.nm_tipo_papel, cd_papel = rel.RelacionamentoPaiPapel.cd_papel });
                }
                if (papeisPaiRelac.Count() > 0)
                    pessoaSearchUI.papeisPai = papeisPaiRelac;
                pessoaJuridicaSGF.PessoaPaiRelacionamento = null;
            }
            if (pessoaSearchUI.papeisFilhos != null && pessoaSearchUI.papeisFilhos.Count() > 0 ||
                pessoaSearchUI.papeisPai != null && pessoaSearchUI.papeisPai.Count() > 0)
            {

                string p = "";
                List<PapelSGF> papeisPessoa = new List<PapelSGF>();

                if (pessoaSearchUI.papeisFilhos != null && pessoaSearchUI.papeisFilhos.Count() > 0)
                    if (pessoaSearchUI.papeisPai != null)
                        papeisPessoa = pessoaSearchUI.papeisFilhos.Union(pessoaSearchUI.papeisPai).ToList();
                    else
                        papeisPessoa = pessoaSearchUI.papeisFilhos.ToList();
                else if (pessoaSearchUI.papeisPai != null)
                    papeisPessoa = pessoaSearchUI.papeisPai.ToList();

                if (papeisPessoa.Count > 0)
                {
                    papeisPessoa = (from c in papeisPessoa select c).Distinct().ToList();
                    foreach (PapelSGF rel in papeisPessoa)
                        p += rel.no_papel + " | ";
                }
                if (p.Length > 3)
                    p = p.Substring(0, p.Length - 3);
                pessoaSearchUI.papel = p;
            }
            return pessoaSearchUI;
        }

        public static PessoaSearchUI fromPessoaForPessoaSearchUI(PessoaFisicaSGF pessoaFisicaSGF)
        {
            string cpf = "";
            if (pessoaFisicaSGF.cd_pessoa_cpf > 0 && pessoaFisicaSGF.PessoaSGFQueUsoOCpf != null && pessoaFisicaSGF.PessoaSGFQueUsoOCpf.cd_pessoa > 0)
                cpf = pessoaFisicaSGF.PessoaSGFQueUsoOCpf.nm_cpf;
            else
                cpf = pessoaFisicaSGF.nm_cpf;
            PessoaSearchUI pessoaSearchUI = new PessoaSearchUI();
            pessoaSearchUI.cd_pessoa = pessoaFisicaSGF.cd_pessoa;
            pessoaSearchUI.no_pessoa = pessoaFisicaSGF.no_pessoa;
            pessoaSearchUI.dc_num_pessoa = pessoaFisicaSGF.dc_num_pessoa;
            pessoaSearchUI.nm_natureza_pessoa = pessoaFisicaSGF.nm_natureza_pessoa;
            pessoaSearchUI.dc_reduzido_pessoa = pessoaFisicaSGF.dc_reduzido_pessoa;
            pessoaSearchUI.dt_cadastramento = pessoaFisicaSGF.dt_cadastramento;
            pessoaSearchUI.id_pessoa_empresa = pessoaFisicaSGF.id_pessoa_empresa;
            pessoaSearchUI.nm_cpf_cgc = cpf;
            pessoaSearchUI.img_pessoa = pessoaFisicaSGF.img_pessoa;
            pessoaSearchUI.ext_img_pessoa = pessoaFisicaSGF.ext_img_pessoa;
            pessoaSearchUI.no_pessoa_dependente = (pessoaFisicaSGF.PessoaSGFQueUsoOCpf != null) ? pessoaFisicaSGF.PessoaSGFQueUsoOCpf.no_pessoa : "";
            //pessoaSearchUI.papeisFilhos = pessoaFisica.PessoaRelacionamento.Select(r => r.PapelFilho);
            //pessoaSearchUI.papeisPai = pessoaFisica.PessoaRelacionamentoPai.Select(r => r.PapelPai);
            if (pessoaFisicaSGF.PessoaFilhoRelacionamento != null && pessoaFisicaSGF.PessoaFilhoRelacionamento.Count() > 0){
                List<PapelSGF> papeisFilhoRelac = new List<PapelSGF>();
                foreach (var rel in pessoaFisicaSGF.PessoaFilhoRelacionamento)
                {
                    if (rel.RelacionamentoFilhoPapel != null)
                        papeisFilhoRelac.Add(new PapelSGF { no_papel = rel.RelacionamentoFilhoPapel.no_papel, nm_tipo_papel = rel.RelacionamentoFilhoPapel.nm_tipo_papel, cd_papel = rel.RelacionamentoFilhoPapel.cd_papel });
                }
                if(papeisFilhoRelac.Count() > 0)
                    pessoaSearchUI.papeisFilhos = papeisFilhoRelac;
                 pessoaFisicaSGF.PessoaFilhoRelacionamento = null;
            }
            if (pessoaFisicaSGF.PessoaPaiRelacionamento != null && pessoaFisicaSGF.PessoaPaiRelacionamento.Count() > 0)
            {
                List<PapelSGF> papeisPaiRelac = new List<PapelSGF>();
                foreach (var rel in pessoaFisicaSGF.PessoaPaiRelacionamento)
                {
                    if (rel.RelacionamentoPaiPapel != null)
                        papeisPaiRelac.Add(new PapelSGF { no_papel = rel.RelacionamentoPaiPapel.no_papel, nm_tipo_papel = rel.RelacionamentoPaiPapel.nm_tipo_papel, cd_papel = rel.RelacionamentoPaiPapel.cd_papel });
                }
                if (papeisPaiRelac.Count() > 0)
                    pessoaSearchUI.papeisPai = papeisPaiRelac;
                pessoaFisicaSGF.PessoaPaiRelacionamento = null;
            }
            if(pessoaSearchUI.papeisFilhos != null && pessoaSearchUI.papeisFilhos.Count() > 0 || 
                pessoaSearchUI.papeisPai != null && pessoaSearchUI.papeisPai.Count() > 0){

                string p = "";
                List<PapelSGF> papeisPessoa = new List<PapelSGF>();

                if (pessoaSearchUI.papeisFilhos != null && pessoaSearchUI.papeisFilhos.Count() > 0)
                    if (pessoaSearchUI.papeisPai != null)
                        papeisPessoa = pessoaSearchUI.papeisFilhos.Union(pessoaSearchUI.papeisPai).ToList();
                    else
                        papeisPessoa = pessoaSearchUI.papeisFilhos.ToList();
                else if (pessoaSearchUI.papeisPai != null)
                    papeisPessoa = pessoaSearchUI.papeisPai.ToList();

                if (papeisPessoa.Count > 0)
                {
                    papeisPessoa = (from c in papeisPessoa select c).Distinct().ToList();
                    foreach (PapelSGF rel in papeisPessoa)
                        p += rel.no_papel + " | ";
                }
                if (p.Length > 3)
                    p = p.Substring(0, p.Length - 3);
                pessoaSearchUI.papel = p;
            }
            
            return pessoaSearchUI;
        }
    }
}
