using System.Collections.Generic;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Model
{
    public class MenusAreaRestritaRetorno
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<MenusAreaRestritaUI> data { get; set; }
    }
}