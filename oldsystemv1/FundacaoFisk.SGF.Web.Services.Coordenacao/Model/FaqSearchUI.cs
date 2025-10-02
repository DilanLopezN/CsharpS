using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class FaqSearchUI : TO
    {
        public string dc_faq_pergunta { get; set; }
        public bool dc_faq_pergunta_inicio { get; set; }
        public string menu { get; set; }
    }
}
