using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public partial class TabelaPrecoUI : TO
    {
        public int cd_tabela_preco { get; set; }
        public int cd_curso { get; set; }
        public String no_curso { get; set; }
        public String no_produto { get; set; }
        public int cd_duracao { get; set; }
        public String dc_duracao { get; set; }
        public int? cd_regime { get; set; }
        public String no_regime { get; set; }
        public DateTime dta_tabela_preco { get; set; }
        public int? nm_parcelas { get; set; }
        public decimal? vl_parcela { get; set; }
        public decimal? vl_matricula { get; set; }
        public decimal? vl_total { get; set; }
        public decimal? vl_aula { get; set; }
        public byte? ordem { get; set; }
        
        //Propriedades para a search:
        
        public string dt_tabela
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dta_tabela_preco);
            }
        }

        public string vl_parc
        {
            get
            {
                if (this.vl_parcela == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_parcela);
            }
        }
        public string vl_ttl
        {
            get
            {
                if (this.vl_total == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_total);
            }
        }

        public string vlAula
        {
            get
            {
                if (this.vl_aula == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_aula);
            }
        }

        public string vl_mat
        {
            get
            {
                if (this.vl_matricula == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_matricula);
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_tabela_preco", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_curso", "Curso", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("no_produto", "Produto", AlinhamentoColuna.Left, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("dc_duracao", "Carga Horária", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("no_regime", "Modalidade", AlinhamentoColuna.Left, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("dt_tabela", "Data", AlinhamentoColuna.Center, "1.000in"));
                retorno.Add(new DefinicaoRelatorio("nm_parcelas", "Parcelas", AlinhamentoColuna.Center, "0.800in"));
                retorno.Add(new DefinicaoRelatorio("vl_parcela", "Unitário", AlinhamentoColuna.Center, "0.800in"));
                retorno.Add(new DefinicaoRelatorio("vl_total", "Total", AlinhamentoColuna.Center, "0.800in"));
                retorno.Add(new DefinicaoRelatorio("vl_matricula", "Matrícula", AlinhamentoColuna.Center, "0.800in"));
                retorno.Add(new DefinicaoRelatorio("vlAula", "Aula", AlinhamentoColuna.Center, "0.600in"));

                return retorno;
            }
        }



        public static TabelaPrecoUI fromTabelaPreco(TabelaPreco tabela)
        {
            TabelaPrecoUI tabelaUI = new TabelaPrecoUI();
            tabelaUI.cd_tabela_preco = tabela.cd_tabela_preco;
            tabelaUI.cd_regime = tabela.cd_regime;
            tabelaUI.no_regime = tabela.RegimeTabelaPreco.no_regime;
            tabelaUI.cd_curso = tabela.cd_curso;
            tabelaUI.no_curso = tabela.CursoTabelaPreco.no_curso;
            tabelaUI.no_produto = tabela.CursoTabelaPreco.Produto.no_produto;
            tabelaUI.cd_duracao = tabela.cd_duracao;
            tabelaUI.dc_duracao = tabela.DuracaoTabelaPreco.dc_duracao;
            tabelaUI.nm_parcelas = tabela.nm_parcelas;
            tabelaUI.vl_parcela = tabela.vl_parcela;
            tabelaUI.vl_matricula = tabela.vl_matricula;
            tabelaUI.vl_aula = tabela.vl_aula;
            tabelaUI.vl_total = (tabela.nm_parcelas * tabela.vl_parcela) ;
            return tabelaUI;
        }
    }

    public class Valores {
        public int? nm_parcelas { get; set; }
        public decimal? vl_parcela { get; set; }
        public decimal? vl_matricula { get; set; }
        public decimal? vl_total { get; set; }
        public decimal? vl_aula { get; set; }

        public string val_parcela
        {
            get
            {
                if (this.vl_parcela == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_parcela);
            }
        }

        public string val_matricula
        {
            get
            {
                if (this.vl_matricula == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_matricula);
            }
        }

        public string val_aula
        {
            get
            {
                if (this.vl_aula == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_aula);
            }
        }

        public string val_total
        {
            get
            {
                if (this.vl_total == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_total);
            }
        }
    }
}
