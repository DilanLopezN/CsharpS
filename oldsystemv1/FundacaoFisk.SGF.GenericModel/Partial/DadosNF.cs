using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class DadosNF
    {
        
        public string no_cidade
        {
            get
            {
                if (this.Localidade == null)
                    return "";
                return this.Localidade.no_localidade;
            }
        }
        public string aliquotaISS
        {
            get
            {
                return string.Format("{0,00}", this.pc_aliquota_iss);
            }
        }

        //Campos para compor a nota fiscal de serviço:
        public string pcAliquotaISS {
            get {
                return string.Format("{0:0.00}", this.pc_aliquota_iss);
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_grupo_estoque", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_cidade", "Origem", AlinhamentoColuna.Left, "1.8000in"));
                retorno.Add(new DefinicaoRelatorio("dc_natureza_operacao", "Nat. Operação", AlinhamentoColuna.Left, "1.5000in"));
                retorno.Add(new DefinicaoRelatorio("dc_item_servico", "Item Lista", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("dc_tributacao_municipio", "Cód. Tributação", AlinhamentoColuna.Left, "1.5000in"));
                retorno.Add(new DefinicaoRelatorio("aliquotaISS", "ISS(%)", AlinhamentoColuna.Center, "0.8000in"));

                return retorno;
            }
        }
    }
}
