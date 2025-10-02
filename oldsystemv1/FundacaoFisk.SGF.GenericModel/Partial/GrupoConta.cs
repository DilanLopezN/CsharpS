using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class GrupoConta {

         public enum tipoGrupoConta
        {
            ATIVO = 1,
            PASSIVO = 2,
            RECEITA = 3,
            CUSTO = 4,
            DESPESA = 5
        }

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
                 if(this.SubGrupos != null)
                     foreach(SubgrupoConta sc in this.SubGrupos)
                         retorno += sc.vl_debito_conta;
                 return retorno;
             }
         }
         public decimal vl_credito_conta {
             get {
                 decimal retorno = 0;
                 if(this.SubGrupos != null)
                     foreach(SubgrupoConta sc in this.SubGrupos)
                         retorno += sc.vl_credito_conta;
                 return retorno;
             }
         }

         public string tipo
         {
             get
             {
                 string retorno = "";
                 if (this.id_tipo_grupo_conta == (int)tipoGrupoConta.ATIVO)
                     retorno = "Ativo";
                 if (this.id_tipo_grupo_conta == (int)tipoGrupoConta.PASSIVO)
                     retorno = "Passivo";
                 if (this.id_tipo_grupo_conta == (int)tipoGrupoConta.RECEITA)
                     retorno = "Receita";
                 if (this.id_tipo_grupo_conta == (int)tipoGrupoConta.DESPESA)
                     retorno = "Despesa";
                 if (this.id_tipo_grupo_conta == (int)tipoGrupoConta.CUSTO)
                     retorno = "Custo";
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
                retorno.Add(new DefinicaoRelatorio("no_grupo_conta", "Grupo de Contas", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("tipo", "Tipo", AlinhamentoColuna.Left, "2.2000in"));
                retorno.Add(new DefinicaoRelatorio("nm_ordem_grupo", "Ordem", AlinhamentoColuna.Center, "2.2000in"));
                //retorno.Add(new DefinicaoRelatorio("grupo_estoque_ativo", "Ativo", AlinhamentoColuna.Center));
                return retorno;
            }
        }

        public class GrupoContaComparer : IEqualityComparer<GrupoConta> {

            public bool Equals(GrupoConta x, GrupoConta y) {
                return (x.cd_grupo_conta.Equals(y.cd_grupo_conta));
            }

            public int GetHashCode(GrupoConta obj) {
                return obj.cd_grupo_conta;
            }
        }
    }
}
