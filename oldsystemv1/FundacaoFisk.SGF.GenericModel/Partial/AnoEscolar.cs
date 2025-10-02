using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class AnoEscolar
    {
        public string ano_escolar_ativo
        {
            get
            {
                return this.id_ativo ? "Sim" : "Não";
            }
        }

        public string no_escolaridade { get; set; }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_midia", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_escolaridade", "Escolaridade", AlinhamentoColuna.Left, "2.1000in"));
                retorno.Add(new DefinicaoRelatorio("dc_ano_escolar", "Descrição", AlinhamentoColuna.Left, "2.1000in"));
                retorno.Add(new DefinicaoRelatorio("nm_ordem", "N° Ordem", AlinhamentoColuna.Left, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("ano_escolar_ativo", "Ativo", AlinhamentoColuna.Left, "0.8000in"));

                return retorno;
            }
        }
    }
}
