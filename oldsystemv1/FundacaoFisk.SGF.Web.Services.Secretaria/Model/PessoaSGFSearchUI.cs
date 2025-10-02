using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public partial class PessoaSGFSearchUI : TO
    {
        public PessoaSGFSearchUI() { }

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
        public byte? nm_sexo { get; set; }
        public string papel {
            get {

                string p = "";
                List<PapelSGF> papeisPessoa = new List<PapelSGF>();

                if (papeisFilhos != null && papeisFilhos.Count() > 0)
                    papeisPessoa = papeisFilhos.Union(papeisPai).ToList();
                else if(papeisPai != null)
                    papeisPessoa = papeisPai.ToList();

                if (papeisPessoa.Count > 0)
                {
                    papeisPessoa = (from c in papeisPessoa select c).Distinct().ToList();
                    foreach (PapelSGF rel in papeisPessoa)
                            p += rel.no_papel + " | ";
                }
                if (p.Length > 3)
                    p = p.Substring(0, p.Length - 3);
                return p;
            }
        }
        public string nm_cpf_cgc_dependente
        {
            get
            {
                string retorno = "";
                if (!String.IsNullOrEmpty(nm_cpf_cgc))
                {
                    if (!string.IsNullOrEmpty(no_pessoa_dependente))
                        retorno = nm_cpf_cgc + "(" + no_pessoa_dependente + ")";
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
              //  retorno.Add(new DefinicaoRelatorio("cd_pessoa", "Código", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("no_pessoa", "Nome", AlinhamentoColuna.Left, "2.2000in"));
                retorno.Add(new DefinicaoRelatorio("dc_reduzido_pessoa", "Nome Reduzido", AlinhamentoColuna.Left, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("nm_cpf_cgc", "CPF\\CNPJ", AlinhamentoColuna.Center, "1.7000in"));
                retorno.Add(new DefinicaoRelatorio("natureza_pessoa", "Natureza", AlinhamentoColuna.Center, "1.1000in"));
                retorno.Add(new DefinicaoRelatorio("dta_cadastro", "Data Cadastro", AlinhamentoColuna.Left, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("papel", "Papel", AlinhamentoColuna.Left, "1.7000in"));

                return retorno;
            }
        }
        public IEnumerable<PapelSGF> papeisFilhos { get; set; }
        public IEnumerable<PapelSGF> papeisPai { get; set; }
        public ICollection<RelacionamentoSGF> relacionamentos { get; set; }
        public ICollection<RelacionamentoSGF> relacionamentosPai { get; set; }


    }
}
