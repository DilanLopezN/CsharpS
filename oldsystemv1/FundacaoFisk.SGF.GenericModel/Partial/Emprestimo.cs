using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class Emprestimo {
        public int nm_dias_biblioteca { get; set; }
        public Nullable<byte> id_bloquear_alt_dta_biblio { get; set; }

        public string dta_emprestimo
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_emprestimo);
            }
        }

        public string dta_prevista_devolucao
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_prevista_devolucao);
            }
        }

        public string dta_devolucao
        {
            get
            {
                if(dt_devolucao.HasValue)
                    return String.Format("{0:dd/MM/yyyy}", dt_devolucao);
                else
                    return String.Empty;
            }
        }

        public string vlTaxaEmprestimo {
            get {
                return string.Format("{0:#,0.00}", this.vl_taxa_emprestimo);
            }
        }

        public string vlMultaEmprestimo {
            get {
                if(this.vl_multa_emprestimo.HasValue)
                    return string.Format("{0:#,0.00}", this.vl_multa_emprestimo.Value);
                else
                    return string.Empty;
            }
        }

        public string no_pessoa {
            get {
                if(this.Pessoa != null)
                    return this.Pessoa.no_pessoa;
                else
                    return String.Empty;
            }
            set {
                if(this.Pessoa == null)
                    this.Pessoa = new PessoaSGF();
                this.Pessoa.no_pessoa = value;
            }
        }

        public string no_item {
            get {
                if(this.Item != null)
                    return this.Item.no_item;
                else
                    return String.Empty;
            }
            set {
                if(this.Item == null)
                    this.Item = new Item();
                this.Item.no_item = value;
            }
        }

        public bool existe_devolucao { get; set; }
        public string str_dt_emprestimo { get; set; }
        public string str_dt_prevista_devolucao { get; set; }
        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_duracao", "Código"));
                retorno.Add(new DefinicaoRelatorio("dta_emprestimo", "Data", AlinhamentoColuna.Left));
                retorno.Add(new DefinicaoRelatorio("no_pessoa", "Pessoa", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("no_item", "Item", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("dta_prevista_devolucao", "Previsão", AlinhamentoColuna.Left));
                retorno.Add(new DefinicaoRelatorio("dta_devolucao", "Devolução", AlinhamentoColuna.Left));
                retorno.Add(new DefinicaoRelatorio("vl_taxa_emprestimo", "Taxa", AlinhamentoColuna.Right));
                retorno.Add(new DefinicaoRelatorio("vl_multa_emprestimo", "Multa", AlinhamentoColuna.Right));

                return retorno;
            }
        }
    }
}
