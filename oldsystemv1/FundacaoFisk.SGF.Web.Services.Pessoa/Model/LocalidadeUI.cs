using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public class LocalidadeUI : LocalidadeSGF
    {
        public LocalidadeSGF localidade { get; set; }
        public int cd_pais { get; set; }
        public string pais { get; set; }
        public int cd_estado { get; set; }
        public string estado { get; set; }
        public int cd_cidade { get; set; }
        public string no_cidade { get; set; }
        public int? nm_cidade { get; set; }
    }
}
