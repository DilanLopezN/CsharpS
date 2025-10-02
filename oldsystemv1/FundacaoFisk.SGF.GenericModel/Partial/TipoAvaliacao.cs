using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class TipoAvaliacao {

        public int cd_turma { get; set; }
        public string no_turma { get; set; }

        public string tipo_ativo
        {
            get
            {
                return this.id_tipo_ativo ? "Sim" : "Não";
            }
        }

        public double vl_soma {get; set;}

        public string vlSoma {
            get {
                return string.Format("{0:#,0.0}", this.vl_soma);
            }
        }

        public int nm_avaliacoes {get;set;}
        public double vl_media {
            get {
                double retorno = 0;
                if(nm_avaliacoes != 0)
                    retorno = vl_soma / nm_avaliacoes;                
                return  Math.Round(retorno, 2, MidpointRounding.AwayFromZero);
            }
        }

        public string vlMedia {
            get {
                return string.Format("{0:#,0.0}", this.vl_media);
            }
        }

        public string vlTotal
        {
            get
            {
                return string.Format("{0:#,0.0}", this.vl_total_nota);
            }
        }
        
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_tipo_avaliacao", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_tipo_avaliacao", "Descrição", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("vl_total_nota", "Nota", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("tipo_ativo", "Ativo", AlinhamentoColuna.Center));

                return retorno;
            }
        }

    }
}
