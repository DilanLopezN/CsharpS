using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class ItemSubgrupo
    {
        public enum TipoSubgGrupo
        {
            ENTRADA = 1,
            SAIDA = 2,
            DESPESA = 3,
            SERVICO_SAIDA = 4,
            DEVOLUCAO_ENTRADA = 5,
            DEVOLUCAO_SAIDA = 6,
            SERVICO_ENTRADA = 7
        }

        public string no_tipo
        {
            get
            {
                var tipo = "";
                switch (id_tipo_movimento)
                {
                    case (byte)TipoSubgGrupo.ENTRADA: tipo = "Entrada";
                        break;
                    case (byte)TipoSubgGrupo.SAIDA: tipo = "Saída";
                        break;
                    case (byte)TipoSubgGrupo.DESPESA: tipo = "Despesa";
                        break;
                    case (byte)TipoSubgGrupo.SERVICO_ENTRADA: tipo = "Entrada de Serviço";
                        break;
                    case (byte)TipoSubgGrupo.SERVICO_SAIDA: tipo = "Saída de Serviço";
                        break;
                    case (byte)TipoSubgGrupo.DEVOLUCAO_ENTRADA: tipo = "Devolução de Entrada";
                        break;
                    case (byte)TipoSubgGrupo.DEVOLUCAO_SAIDA: tipo = "Devolução de Saída";
                        break;
                    default: tipo = "";
                        break;
                }
                return tipo;
            }
            set { }
        }

        public int cd_grupo_estoque;
        public int cd_plano_conta;
        public string no_subgrupo;
    }
}
