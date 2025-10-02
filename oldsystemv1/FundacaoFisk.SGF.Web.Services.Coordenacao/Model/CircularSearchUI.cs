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
    public class CircularSearchUI : TO
    {
        public short nm_ano_circular { get; set; }
        public List<byte> nm_mes_circular { get; set; }
        public int nm_circular { get; set; }
        public string no_circular { get; set; }
        public List<byte> nm_menu_circular { get; set; }
    }
}
