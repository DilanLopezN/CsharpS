using System;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class EmpresaValorServico
    {
        public string dta_inicio_valor_servico
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_inicio_valor);
            }
        }
    }
}