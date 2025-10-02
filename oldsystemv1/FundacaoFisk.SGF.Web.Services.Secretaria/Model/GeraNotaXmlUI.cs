using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model {
    public partial class GeraNotaXmlUI : TO 
    {
        public enum StatusProcedure
        {
            SUCESSO_EXECUCAO_PROCEDURE = 0,
            ERRO_EXECUCAO_PROCEDURE = 1
        }

        public int cd_importacao_XML { get; set; }
        public string no_arquivo_XML { get; set; }
        public string dc_path_arquivo { get; set; }
        public string dc_mensagem_XML { get; set; }
        public string nm_nota_fiscal { get; set; }
        public System.DateTime dt_emissao_nf { get; set; }
        public string no_escola { get; set; }
        public string no_pessoa { get; set; }
        public string no_item_inexistente { get; set; }
        public byte id_tipo_importacao { get; set; }
        public bool id_resolvido { get; set; }
        public System.DateTime dt_importacao_xml { get; set; }
        public int cd_usuario { get; set; }
    }
}
