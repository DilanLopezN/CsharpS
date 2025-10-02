using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public partial class ConceitoSearchUI : TO
    {
        public int cd_conceito { get; set; }
        public string no_conceito { get; set; }
        public bool id_conceito_ativo { get; set; }
        public Nullable<decimal> pc_inicial_conceito { get; set; }
        public Nullable<decimal> pc_final_conceito { get; set; }
        public int cd_produto { get; set; }
        public string no_produto { get; set; }
        public double vl_nota_participacao { get; set; }

        public string conceito_ativo
        {
            get
            {
                return this.id_conceito_ativo ? "Sim" : "Não";
            }
        }
        public string pc_inicial
        {
            get
            {
                if (this.pc_inicial_conceito == null)
                    return "";
                return string.Format("{0,00}", this.pc_inicial_conceito);
            }
        }
        public string pc_final
        {
            get
            {
                if (this.pc_final_conceito == null)
                    return "";
                return string.Format("{0,00}", this.pc_final_conceito);
            }
        }

        public string val_nota_participacao
        {
            get
            {
                return string.Format("{0,00}", this.vl_nota_participacao);
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
               // retorno.Add(new DefinicaoRelatorio("cd_conceito", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_produto", "Produto", AlinhamentoColuna.Left, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("no_conceito", "Conceito", AlinhamentoColuna.Left, "2.5000in"));
                retorno.Add(new DefinicaoRelatorio("pc_inicial", "Perc. Inicial", AlinhamentoColuna.Right, "1.1000in"));
                retorno.Add(new DefinicaoRelatorio("pc_final", "Perc. Final", AlinhamentoColuna.Right, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("Participação", "val_nota_participacao", AlinhamentoColuna.Right, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("conceito_ativo", "Ativo", AlinhamentoColuna.Center, "0.9000in"));

                return retorno;
            }
        }

        public static ConceitoSearchUI fromConceito(Conceito conceito, String produto)
        {
            ConceitoSearchUI conceitoUI = new ConceitoSearchUI
            {
                cd_conceito = conceito.cd_conceito,
                cd_produto = conceito.cd_produto,
                id_conceito_ativo = conceito.id_conceito_ativo,
                no_conceito = conceito.no_conceito,
                no_produto = produto,
                pc_inicial_conceito = conceito.pc_inicial_conceito,
                pc_final_conceito = conceito.pc_final_conceito,
                vl_nota_participacao = conceito.vl_nota_participacao
            };
            return conceitoUI;
        }
    }
}
