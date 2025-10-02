using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class ChequeUI : TO
    {

        public int cd_pessoa_titulo { get; set; }
        public string no_pessoa_titulo { get; set; }
        public string nm_cpf_cgc { get; set; }
        public string no_banco { get; set; }
        public DateTime dt_vcto_titulo { get; set; }
        public decimal vl_titulo { get; set; }
        public DateTime dt_emissao_titulo { get; set; }
        public int? nm_nota { get; set; }
        public string nm_serie { get; set; }
        public string nm_recibo { get; set; }
        public string no_emitente { get; set; }
        public int nm_agencia { get; set; }
        public int nm_conta_corrente { get; set; }
        public string nm_cheque { get; set; }
        public DateTime dt_baixa_titulo { get; set; }
        public string nm_serie_nota
        {
            get
            {
                string retorno = "";
                if (nm_nota != null)
                    retorno = nm_nota + "";
                if (!string.IsNullOrEmpty(nm_serie))
                    retorno = retorno + "-" + nm_serie;
                return retorno;
            }

        }

        public string dt_emissao
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", this.dt_emissao_titulo);
            }
        }
        public string dt_vcto
        {
            get
            {
                return this.dt_vcto_titulo.CompareTo(DateTime.MinValue) == 0 ? "" : String.Format("{0:dd/MM/yyyy}", this.dt_vcto_titulo);
            }
        }

        public string dt_baixa
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", this.dt_baixa_titulo);
            }
        }
        public string vlTitulo
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_titulo);
            }
        }

    }
}
