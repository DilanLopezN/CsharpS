using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class MotivoFalta
    {
        public string motivoFaltaAtiva
        {
            get
            {
                return this.id_motivo_falta_ativa ? "Sim" : "Não";
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_motivo_falta", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_motivo_falta", "Motivo Falta", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("motivoFaltaAtiva", "Ativo", AlinhamentoColuna.Center));

                return retorno;
            }
        }
    }
}
