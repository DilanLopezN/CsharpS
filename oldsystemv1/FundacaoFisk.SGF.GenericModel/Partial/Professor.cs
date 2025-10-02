using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Professor
    {
        public List<Turma> turmaAtivas { get; set; }
        public string no_professor { get; set; }

        public string vlPagamentoInterno 
        { 
            get 
            {
                if (this.vl_pagamento_interno == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_pagamento_interno);
            } 
        }

        public string vlPagamentoExterno 
        { 
            get 
            {
                if (this.vl_pagamento_externo == null)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_pagamento_externo);
            } 
        }

        public string vlTerminoEstagio
        {
            get 
            {
                return string.Format("{0:#,0.00}", this.vl_termino_estagio);
            }
        }

        public string vlRematricula
        {
            get 
            {
                return string.Format("{0:#,0.00}", this.vl_rematricula);
            }
        }
    }
}
