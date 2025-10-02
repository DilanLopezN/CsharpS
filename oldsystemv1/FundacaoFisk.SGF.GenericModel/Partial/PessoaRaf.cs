using System;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class PessoaRaf
    {
        public string dta_expiracao_senha
        {
            get
            {
                if (dt_expiracao_senha != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_expiracao_senha.ToLocalTime());
                else
                    return String.Empty;
            }
        }
        public string dta_limite_bloqueio
        {
            get
            {
                if (dt_limite_bloqueio != null)
                {
                    DateTime dataAux = (DateTime)dt_limite_bloqueio;
                    return String.Format("{0:dd/MM/yyyy}", dataAux.ToLocalTime());
                }
                else
                    return String.Empty;
            }
        }

        public bool hasPassCreated { get; set; }
        
    }
}