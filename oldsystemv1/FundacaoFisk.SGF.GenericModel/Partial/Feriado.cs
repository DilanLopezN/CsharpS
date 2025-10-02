using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Feriado
    {
        public enum TipoOperacaoSPFeriado
        {
            INSERCAO = 0,
            DELECAO = 1
        }

        public DateTime dt_inicio { get; set; }
        public DateTime dt_fim { get; set; }

        public string isFeriadoFinanceiro
        {
            get
            {
                return this.id_feriado_financeiro == true ? "Sim" : "Não";
            }
        }

        public string idFeriadoAtivo
        {
            get
            {
                return this.id_feriado_ativo == true ? "Sim" : "Não";
            }
        }

        public string TipoFeriado {           
            get 
            {
                return this.cd_pessoa_escola == null ? "Não" : "Sim"; 
            }        
        }

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cod_feriado", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_feriado", "Feriado", AlinhamentoColuna.Left, "2.3000in"));
                retorno.Add(new DefinicaoRelatorio("dd_feriado", "Dia Início", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("mm_feriado", "Mês Início", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("aa_feriado", "Ano Início", AlinhamentoColuna.Center, "0.9000in"));

                retorno.Add(new DefinicaoRelatorio("dd_feriado_fim", "Dia Fim", AlinhamentoColuna.Center, "0.7000in"));
                retorno.Add(new DefinicaoRelatorio("mm_feriado_fim", "Mês Fim", AlinhamentoColuna.Center, "0.7000in"));
                retorno.Add(new DefinicaoRelatorio("aa_feriado_fim", "Ano Fim", AlinhamentoColuna.Center, "0.7000in"));

                retorno.Add(new DefinicaoRelatorio("TipoFeriado", "Feriado Escola", AlinhamentoColuna.Center, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("isFeriadoFinanceiro", "Financeiro", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("idFeriadoAtivo", "Ativo", AlinhamentoColuna.Center, "0.9000in"));

                return retorno;
            }
        }
        public string ano
        {
            get
            {
                return this.aa_feriado == null ? "" : this.aa_feriado+"";
                
            }
        }

    }
}
