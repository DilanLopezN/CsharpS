using System.Collections.Generic;
using System;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Model
{
    public partial class UserAreaRestritaCreatedUI
    {
        public string type_access { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string id_fisk_sgf { get; set; }
        public string id_fisk_franchisee { get; set; }
        public int id_created_by { get; set; }
        public string menus { get; set; }
        public List<string> menusConvertidos { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public int id { get; set; }
    }
}