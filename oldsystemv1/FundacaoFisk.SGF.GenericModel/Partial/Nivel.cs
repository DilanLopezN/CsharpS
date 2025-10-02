using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Nivel
    {
        public enum SituacaoHistorico
        {
            BASICO = 1,
            INTERMEDIARIO = 2,
            AVANCADO = 3,
            NAOAPLICAVEL = 4
            
            
        }
        public string nivelAtivo
        {
            get
            {
                return this.id_ativo ? "Sim" : "Não";
            }
        }
    

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_evento", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_nivel", "Descrição", AlinhamentoColuna.Left, "4.5000in"));
                retorno.Add(new DefinicaoRelatorio("nm_ordem", "Ordem", AlinhamentoColuna.Left, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("nivelAtivo", "Ativo", AlinhamentoColuna.Center,"0.9000in"));

                return retorno;
            }
        }
    }
}
