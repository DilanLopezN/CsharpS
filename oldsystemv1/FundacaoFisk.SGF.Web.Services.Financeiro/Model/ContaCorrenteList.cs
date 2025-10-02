using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class ContaCorrenteList
    {
       public ICollection<MovimentacaoFinanceira> movimentacaoFinanceira { get; set; }
       public ICollection<LocalMovto> localMovimentoOrigem { get; set; }
       public ICollection<LocalMovto> localMovimentoDestino { get; set; }
       public List<TipoLiquidacao> tiposLiquidacao { get; set; }
       public Parametro parametro { get; set; }
       public string planoConta { get; set; }
       public int? cd_plano_conta { get; set; }
    }
}
