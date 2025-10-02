using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class VideoSearchUI : TO
    {
        public int nm_video { get; set; }
        public string no_video { get; set; }
        public List<byte>  menu { get; set; }
    }
}
