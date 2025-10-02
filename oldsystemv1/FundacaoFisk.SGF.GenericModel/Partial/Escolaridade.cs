using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class Escolaridade : TO{
        public string escolaridade_ativa {
            get {
                return this.id_escolaridade_ativa ? "Sim" : "Não";
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_escolaridade", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_escolaridade", "Escolaridade", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("escolaridade_ativa", "Ativa", AlinhamentoColuna.Center));

                return retorno;
            }
        }
    }
}
