using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Faq : TO
    {
        public List<Children> children { get; set; }
    }

    public class Children
    {
        public Guid cd_faq { get; set; }
        public int id { get; set; }
        public string dc_faq_resposta { get; set; }
        public string no_video_faq { get; set; }
    }
}
