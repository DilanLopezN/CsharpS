using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class MotivoNaoMatricula : TO {
        public int cd_prospect_motivo_nao_matricula { get; set; }
        public int qtd_motivo { get; set; }
        public double perc_motivo { get; set; }
        public string motivo_nao_matricula_ativo
        {
            get {
                return this.id_motivo_nao_matricula_ativo ? "Sim" : "Não";
            }
        }
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_motivo_nao_matricula", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_motivo_nao_matricula", "Motivo da Não Matrícula", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("motivo_nao_matricula_ativo", "Ativo", AlinhamentoColuna.Center));

                return retorno;
            }
        }

    }
}
