using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class SmsParametroUI : TO {
        public int? cd_escola { get; set; }
        public string num_usu { get; set; }
        public string senha { get; set; }
        public string seu_num { get; set; }
        public string url_servico { get; set; }
        public Nullable<int> cod_param { get; set; }
        public int id_automatico_devedores { get; set; }
        public int id_automatico_aniversario { get; set;}
    }
}


