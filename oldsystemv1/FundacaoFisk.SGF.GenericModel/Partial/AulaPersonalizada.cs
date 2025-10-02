using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class AulaPersonalizada {
        public string no_produto { get; set; }
        public string no_turma_personalizada { get; set; }
        public string no_sala { get; set; }
        public string no_professor { get; set; }
        public string no_aluno { get; set; }
        public int? cd_aula_personalizada_aluno { get; set; }
        public bool? id_participou { get; set; }
        public int cd_usuario { get; set; }
        public DateTime dt_cadastro_aula { get; set; }
        public int cd_aluno { get; set; }
        

        public ICollection<Produto> produtos { get; set; }
        public ICollection<Sala> salas { get; set; }
        public ICollection<FuncionarioSGF> professores { get; set; }

        public string dta_aula_personalizada
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_aula_personalizada);
            }
        }
        public string participou
        {
            get
            {
                string retorno = "Não";
                if (this.id_participou.HasValue && this.id_participou.Value)
                    retorno = "Sim";
                else
                    if (!this.id_participou.HasValue)
                        retorno = "";
                return retorno;
            }
        }
    }
}
