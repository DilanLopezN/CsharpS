using Componentes.GenericModel;
using System.Collections.Generic;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class sp_RptCartaQuitacao_Result : TO
    {
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                retorno.Add(new DefinicaoRelatorio("no_responsavel", "Nome", AlinhamentoColuna.Left, "2.3000in"));
                retorno.Add(new DefinicaoRelatorio("dc_fone_mail", "Email", AlinhamentoColuna.Left, "3.0000in"));
                retorno.Add(new DefinicaoRelatorio("nm_cpfcnpj", "Cpf/Cnpj", AlinhamentoColuna.Center));

                return retorno;
            }
        }
    }
}