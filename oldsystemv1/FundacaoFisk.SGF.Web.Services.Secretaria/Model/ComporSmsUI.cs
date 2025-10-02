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
    public class ComporSmsUI : TO {
        public Nullable<int> cd_escola { get; set;}
        public Nullable<int> motivo { get; set;}
        public string mensagem { get; set; }
        public DateTime? dt_cadastro { get; set; }
        public DateTime? dt_ultima_altracao { get; set; }
    }
}


