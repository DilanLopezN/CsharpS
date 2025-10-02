using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class EstagioOrdem
    {
        public Estagio estagio { get; set; }
        public ICollection<Estagio> estagioOrdem { get; set; }
        public String noProduto { get; set; }
    }
}
