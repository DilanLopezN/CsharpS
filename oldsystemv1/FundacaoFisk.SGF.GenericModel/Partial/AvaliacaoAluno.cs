using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel{
    public partial class AvaliacaoAluno {
        public string dc_tipo_avaliacao { get; set;}
        public string dc_criterio_avaliacao { get; set; }
        public int cd_criterio_avaliacao { get; set; }
        public byte? vl_nota { get; set; }
        public DateTime? dt_avaliacao_turma { get; set; }
        public string dta_avaliacao_turma
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_avaliacao_turma);
            }
        }
        public decimal? nm_peso_avaliacao { get; set; }
        public string no_conceito { get; set; }
        public double? vl_nota_corrigida { 
            get {
                double? retorno = this.nm_nota_aluno;
                if(nm_peso_avaliacao.HasValue && nm_nota_aluno.HasValue)
                    retorno = (double)nm_peso_avaliacao.Value * nm_nota_aluno.Value;
                
                return retorno;
            }
            set { }
        }
        public double? vl_nota_media
        {
            get
            {
                double? retorno = this.nm_nota_aluno;
                if(nm_nota_aluno_2.HasValue) retorno = this.nm_nota_aluno_2;
                if (nm_peso_avaliacao.HasValue) //&& nm_nota_aluno.HasValue)
                    retorno = (double)nm_peso_avaliacao.Value * retorno; //nm_nota_aluno.Value;
                return retorno;
            }
            set { }
        }
        public string vlNota
        {
            get {
                if(this.vl_nota.HasValue)
                    return string.Format("{0:#,0.0}", this.vl_nota);
                else
                    return string.Empty;
            }
        }
        public string vlNotaCorrigida {
            get {
                if(this.vl_nota_corrigida.HasValue)
                return string.Format("{0:#,0.0}", this.vl_nota_corrigida);
                else
                    return string.Empty;
            }
        }
        public string vlNota2
        {
            get
            {
                if (this.nm_nota_aluno_2.HasValue)
                    return string.Format("{0:#,0.0}", this.nm_nota_aluno_2);
                else
                    return string.Empty;
            }
        }

        public int cd_estagio { get; set;}
        public string no_estagio { get; set;}
        public int cd_turma { get; set;}
        public string no_turma { get; set; }
        public int nm_total_avaliacao_curso { get; set; }
        public bool isModifiedA { get; set; }

    }
}
