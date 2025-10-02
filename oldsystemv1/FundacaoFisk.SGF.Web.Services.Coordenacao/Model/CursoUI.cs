using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class CursoUI
    {
        public Curso curso { get; set; }
        public ICollection<ItemCurso> materiaisDidaticos { get; set; }

        public int cd_produto { get; set; }
        public int cd_curso { get; set; }
        public String no_produto { get; set; }
        public String no_curso { get; set; }
    }
}
