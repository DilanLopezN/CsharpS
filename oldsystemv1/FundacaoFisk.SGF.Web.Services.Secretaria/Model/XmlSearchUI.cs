using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class XmlSearchUI: TO
    {
        public string cd_importacao_XML { get; set; }
        public bool? id_resolvido { get; set; }
        public string no_arquivo_XML { get; set; }
        public string dc_path_arquivo { get; set; }
        public string dc_mensagem_XML { get; set; }
        public string no_pessoa { get; set; }
        public string no_escola { get; set; }
        public int? inicio { get; set; }
        public string nm_nota_fiscal { get; set; }
        public DateTime? dt_emissao_nf { get; set; }
        public string itemExistente { get; set; }
        public DateTime? data_ini { get; set; }
        public DateTime? dataFim { get; set; }
    }
}
