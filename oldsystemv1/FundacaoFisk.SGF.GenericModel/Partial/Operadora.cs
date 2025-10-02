using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    using Componentes.GenericModel;
    public partial class Operadora 
    {
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
               // retorno.Add(new DefinicaoRelatorio("cd_operadora", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_operadora", "Operadora", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("operadora_ativa", "Ativa"));
                return retorno;
            }

        }
        public string operadora_ativa
        {
            get
            {
                return this.id_operadora_ativa ? "Sim" : "Não";
            }
        }

   }
}
