using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Regime
    {
        public string regimeAtivo
        {
            get
            {
                return this.id_regime_ativo ? "Sim" : "Não";
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_regime", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_regime", "Modalidade", AlinhamentoColuna.Left, "3.0000in"));
                retorno.Add(new DefinicaoRelatorio("no_regime_abreviado", "Abreviado", AlinhamentoColuna.Left));
                retorno.Add(new DefinicaoRelatorio("regimeAtivo", "Ativo", AlinhamentoColuna.Center));

                return retorno;
            }
        }
    }
}
