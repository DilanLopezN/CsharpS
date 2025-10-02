using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class AliquotaUF
    {
        public List<EstadoSGF> estadosOriPes { get; set; }
        public List<EstadoSGF> estadosDesPes { get; set; }

        public string no_estado_origem
        {
            get
            {
                if (this.EstadoOrigem == null || (this.EstadoOrigem != null && this.EstadoOrigem.Localidade == null))
                    return "";
                return this.EstadoOrigem.Localidade.no_localidade;
            }
        }
        public string no_estado_destino
        {
            get
            {
                if (this.EstadoDestino == null || (this.EstadoDestino != null && this.EstadoDestino.Localidade == null))
                    return "";
                return this.EstadoDestino.Localidade.no_localidade;
            }
        }
        public string aliquotaICMS
        {
            get
            {
                return string.Format("{0,00}", this.pc_aliq_icms_padrao);
            }
        }
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_grupo_estoque", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_estado_origem", "Origem", AlinhamentoColuna.Left, "1.8000in"));
                retorno.Add(new DefinicaoRelatorio("no_estado_destino", "Destino", AlinhamentoColuna.Left, "1.8000in"));
                retorno.Add(new DefinicaoRelatorio("aliquotaICMS", "ICMS(%)", AlinhamentoColuna.Center));

                return retorno;
            }
        }
    }
}
