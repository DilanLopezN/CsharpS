using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Evento
    {

        public enum TiposEvento
        {
            FALTA = 1
        }

        public string eventoAtivo
        {
            get
            {
                return this.id_evento_ativo ? "Sim" : "Não";
            }
        }

        public List<Aluno> alunosEvento { get; set; }

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_evento", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_evento", "Evento", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("eventoAtivo", "Ativo", AlinhamentoColuna.Center));

                return retorno;
            }
        }
    }
}
