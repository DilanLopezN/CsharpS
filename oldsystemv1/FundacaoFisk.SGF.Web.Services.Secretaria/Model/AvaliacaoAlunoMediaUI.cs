using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model {
    public class AvaliacaoAlunoMediaUI
    {
        public IEnumerable<AvaliacaoAluno> avaliacoesNota { get; set; }
        public IEnumerable<TipoAvaliacao>  avaliacoesMedia { get; set; }
        public IEnumerable<TipoAvaliacao>  avaliacoesTurma { get; set; }

        public double vl_maximo { get; set; }
        public double vl_media {
            get {
                double retorno = 0;
                if(vl_maximo != 0)
                    retorno = vl_total * 100 / vl_maximo;
                return retorno;
            }
        }
        public double vl_total { get; set; }

        public double vl_media_final { get; set; }
        public double vl_aproveitamento_total { get; set; }
        public double vl_media_parcial { get; set; }

        public string vlAproveitamentoTotal
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_aproveitamento_total);
            }
        }

        public string vlMediaParcial
        {
            get
            {
                return string.Format("{0:#,0.0}", this.vl_media_parcial);
            }
        }

        public string vlMediaFinal
        {
            get
            {
                return string.Format("{0:#,0.0}", this.vl_media_final);
            }
        }
    }
}
