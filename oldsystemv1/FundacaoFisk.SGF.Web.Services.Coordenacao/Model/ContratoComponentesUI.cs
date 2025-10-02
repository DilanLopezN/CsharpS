using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class ContratoComponentesUI
    {
        public Contrato contrato { get; set; }
        public int cd_contrato { get; set; }
        public List<Produto> produtos { get; set; }
        public List<Regime> regimes { get; set; }
        public List<NomeContrato> nomesContrato { get; set; }
        public List<TipoLiquidacao> tipoLiquidacoes { get; set; }
        public List<DuracaoUI> duracoes { get; set; }
        public List<LocalMovto> localMovto { get; set; }
        public List<AnoEscolar> anosEscolares { get; set; }
        public List<MotivoBolsa> motivosBolsa { get; set; }
    }
}
