using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class AvaliacaoOrdem : TO
    {
        public Avaliacao avaliacao { get; set; }
        public ICollection<Avaliacao> avaliacoes { get; set; }
        public String criterio { get; set; }
        public String tipoAvaliacao { get; set; }
        public String abreviatura { get; set; }
    }
}
