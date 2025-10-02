using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model {
    public class EventoHistoricoUI {
        public ICollection<AlunoEvento> listaAlunoEvento { get; set; }
        public DiarioAula ultimaAulaAluno { get; set; }
        public DiarioAula ultimaAulaTurma { get; set; }
    }
}
