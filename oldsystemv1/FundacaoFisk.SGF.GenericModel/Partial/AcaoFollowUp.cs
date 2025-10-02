using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class AcaoFollowUp
    {
        public string acao_ativa
        {
            get {
                return this.id_acao_ativa ? "Sim" : "Não";
            }
        }
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_motivo_cancelamento_bolsa", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_acao_follow_up", "Ação Follow-up", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("acao_ativa", "Ativa", AlinhamentoColuna.Center));

                return retorno;
            }
        }
    }
}
