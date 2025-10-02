using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class AlunosCargaHorariaUI: TO
    {
        public AlunosCargaHorariaUI() { }

        public string dc_reduzido_pessoa { get; set; }

        public int cd_pessoa_escola { get; set; }

        public int cd_aluno { get; set; }

        public int cd_pessoa_aluno { get; set; }

        public string no_aluno { get; set; }

        public Nullable<int> cd_curso { get; set; }

        public string no_curso { get; set; }

        public int cd_turma { get; set; }

        public string no_turma { get; set; }

        public int cd_desistencia { get; set; }

        public System.DateTime dt_desistencia { get; set; }

        public Nullable<int> cd_contrato { get; set; }

        public string tx_obs_desistencia { get; set; }
        public string nm_raf { get; set; }
        public string dta_desistencia
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", this.dt_desistencia);
            }
        }
        public int itemVoucher { get; set; }  
    }
}
