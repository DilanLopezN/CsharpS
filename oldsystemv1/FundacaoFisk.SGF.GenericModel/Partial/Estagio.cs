using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Estagio
    {
        public string estagio_ativo
        {
            get
            {
                return this.id_estagio_ativo ? "Sim" : "Não";
            }
        }

        public string no_produto { get; set; }
        public int nm_aulas_dadas { get; set; }
        public byte? nm_faltas { get; set; }
        public int nm_aulas_contratadas { get; set; }
        public DateTime primeira_aula { get; set; }
        public DateTime? ultima_aula { get; set; }
    }
}
