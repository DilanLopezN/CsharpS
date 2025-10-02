using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class SubgrupoConta {


        public enum TipoNivelConsulta
        {
            TODOS = 0,
            UM_NIVEL = 1,
            DOIS_NIVEIS = 2
        }
        public string grupo_conta { get; set; }
        public string subgrupo_2_conta { get; set; }

        /* Propriedades do Relatório de Balancete */
        private decimal _vl_saldo_anterior = 0;
        public decimal vl_saldo_anterior {
            get {
                return _vl_saldo_anterior;
            }
            set {
                this._vl_saldo_anterior = value;
            }
        }
        public decimal vl_saldo {
            get {
                return vl_saldo_anterior + vl_debito_conta - vl_credito_conta;
            }
        }
        public decimal vl_debito_conta {
            get {
                decimal retorno = 0;
                if(this.SubgrupoPlanoConta != null && this.SubgrupoPlanoConta.Count() > 0)
                    foreach(PlanoConta pc in this.SubgrupoPlanoConta)
                        retorno += pc.vl_debito_conta;
                else
                    if(this.SubgruposFilhos != null)
                        foreach(SubgrupoConta sc in this.SubgruposFilhos)
                            retorno += sc.vl_debito_conta;
                return retorno;
            }
        }
        public decimal vl_credito_conta {
            get {
                decimal retorno = 0;
                if(this.SubgrupoPlanoConta != null && this.SubgrupoPlanoConta.Count() > 0)
                    foreach(PlanoConta pc in this.SubgrupoPlanoConta)
                        retorno += pc.vl_credito_conta;
                else
                    if(this.SubgruposFilhos != null)
                        foreach(SubgrupoConta sc in this.SubgruposFilhos)
                            retorno += sc.vl_credito_conta;
                return retorno;
            }
        }

/// <summary>
/// Função que monta as colunos no relatório do Grupo de Estoque 
/// </summary>
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_grupo_estoque", "Código"));
                retorno.Add(new DefinicaoRelatorio("grupo_conta", "Grupo de Contas", AlinhamentoColuna.Left, "2.8000in"));
                retorno.Add(new DefinicaoRelatorio("no_subgrupo_conta", "Subgrupo 1 de Contas", AlinhamentoColuna.Left, "2.8000in"));
                retorno.Add(new DefinicaoRelatorio("subgrupo_2_conta", "Subgrupo 2 de Contas", AlinhamentoColuna.Left, "2.8000in"));
                retorno.Add(new DefinicaoRelatorio("nm_ordem_subgrupo", "Ordem", AlinhamentoColuna.Center, "2.8000in"));

                return retorno;
            }
        }

        public static SubgrupoConta.TipoNivelConsulta parseTipoNivel(int tipoNivel){
            TipoNivelConsulta tipo = new TipoNivelConsulta();
            if (tipoNivel == 0)
                tipo = TipoNivelConsulta.TODOS;
            if (tipoNivel == 1)
                tipo = TipoNivelConsulta.UM_NIVEL;
            if (tipoNivel == 2)
                tipo = TipoNivelConsulta.DOIS_NIVEIS;
            return tipo;
        }
        
    }
}
