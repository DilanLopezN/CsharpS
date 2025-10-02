using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Service.EmailMarketing.Model
{
    public class RptListagemEndereco : TO
    {
        public string no_pessoa { get; set; }
        public string no_bairro { get; set; }
        public string no_cidade { get; set; }
        public string dc_num_cep { get; set; }
        public string sg_estado { get; set; }
        public string no_localidade { get; set; }
        public string dc_compl_endereco { get; set; }
        public string no_tipo_logradouro { get; set; }
        public string dc_num_endereco { get; set; }
        public string email { get; set; }
        public bool id_inscrito { get; set; }
        public bool inscrito { get; set; }
        public string endereco_pessoa {
            get
            {
                string retorno = "";
                if (!String.IsNullOrEmpty(no_tipo_logradouro))
                    retorno += no_tipo_logradouro;
                if (!String.IsNullOrEmpty(no_localidade))
                    retorno += " " +no_localidade;
                if (!String.IsNullOrEmpty(dc_num_endereco))
                    retorno += ", " + dc_num_endereco;
                if (!String.IsNullOrEmpty(dc_compl_endereco))
                    retorno += ", " + dc_compl_endereco;
                return retorno;
            }
        }
    }
}

