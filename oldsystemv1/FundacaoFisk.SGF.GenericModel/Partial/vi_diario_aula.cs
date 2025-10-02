using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class vi_diario_aula
    {
        public string desc_status
        {
            get
            {
                var status = "Efetivado";
                if(id_status_aula == (int) DiarioAula.StatusDiarioAula.Cancelada)
                    status = "Cancelada";
                return status;
            }
        }

        public string desc_presPorf
        {
            get
            {
                var status = "Presente";
                if (id_falta_professor == 1)
                    status = "Falta";
                if (id_falta_professor == 2)
                    status = "Justificada";
                return status;
            }
        }

        public string dta_aula
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_aula);
            }
        }

        public string id_substituto
        {
            get
            {
                string retorno = "Não";
                if ((id_falta_professor == (int )DiarioAula.PresencaProfesssor.Falta || id_falta_professor == (int) DiarioAula.PresencaProfesssor.Justificada) &&
                    String.IsNullOrEmpty(nom_professor) && !String.IsNullOrEmpty(nom_susbtituto))
                    retorno = "Sim";
                return retorno;
            }
        }

        public string no_professor
        {
            get
            {
                string prof = nom_professor;
                if(string.IsNullOrEmpty(nom_professor) && !string.IsNullOrEmpty(nom_susbtituto))
                    prof = nom_susbtituto;
                return prof;
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                retorno.Add(new DefinicaoRelatorio("dta_aula", "Data da Aula", AlinhamentoColuna.Center, "1.1000in"));
                retorno.Add(new DefinicaoRelatorio("no_turma", "Turma", AlinhamentoColuna.Left, "2.3000in"));
                retorno.Add(new DefinicaoRelatorio("no_professor", "Professor", AlinhamentoColuna.Left, "1.6000in"));
                retorno.Add(new DefinicaoRelatorio("id_substituto", "Substituto", AlinhamentoColuna.Center, "1.1000in"));
                retorno.Add(new DefinicaoRelatorio("nm_aula_turma", "Aula", AlinhamentoColuna.Left, "1.1000in"));
                retorno.Add(new DefinicaoRelatorio("no_tipo_atividade_extra", "Tipo Aula", AlinhamentoColuna.Left, "1.5000in"));
                retorno.Add(new DefinicaoRelatorio("desc_status", "Status Aula", AlinhamentoColuna.Center, "1.1000in"));
                return retorno;
            }
        }
    }
}
