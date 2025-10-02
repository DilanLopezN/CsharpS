using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model {
    public class ProgramacaoTurmaAbaUI {
        public ICollection<ProgramacaoTurma> Programacoes { get; set; }
        public ICollection<FeriadoDesconsiderado> FeriadosDesconsiderados { get; set; }
    }
}
