using System.Collections.Generic;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Model
{
    public class UserAreaRestritaDetalheUI
    {
        public int? id { get; set; }
        public string type_access { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string id_fisk_sgf { get; set; }
        public string id_fisk_franchisee { get; set; }
        public string menus { get; set; }
        public List<string> menusConvertidos { get; set; }
        public string status { get; set; }
    }
}