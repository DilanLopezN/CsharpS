using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class CriterioAvaliacao {

        public string criterio_ativo
        {
            get
            {
                return this.id_criterio_ativo ? "Sim" : "Não";
            }
        }

        public string participacao_ativo
        {
            get
            {
                return this.id_participacao ? "Sim" : "Não";
            }
        }

        public string conceito
        {
            get
            {
                return this.id_conceito ? "Sim" : "Não";
            }
        }
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_criterio_avaliacao", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_criterio_avaliacao", "Nome", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("dc_criterio_abreviado", "Abreviatura", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("criterio_ativo", "Ativo", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("conceito", "Conceito", AlinhamentoColuna.Center, "0.8000in"));
                retorno.Add(new DefinicaoRelatorio("participacao_ativo", "Participação", AlinhamentoColuna.Center, "0.8000in"));

                return retorno;
            }
        }

    }
}
