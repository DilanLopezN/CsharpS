using System;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class CargaHorariaUI:TO
    {
        public int cd_pessoa_escola { get; set; }

        public string no_escola { get; set; }

        public string nm_raf { get; set; }
        public int cd_aluno { get; set; }

        public string no_aluno { get; set; }

        public Nullable<int> cd_curso { get; set; }

        public string no_curso { get; set; }

        public int cd_turma { get; set; }

        public string no_turma { get; set; }

        public Nullable<double> nm_carga { get; set; }

        public int nm_carga_maxima { get; set; }

        public int qt_voucher { get; set; }

        public Nullable<System.DateTime> dt_ultima_aula { get; set; }

        public string dta_ultima_aula
        {
            get
            {
                if (dt_ultima_aula.HasValue)
                {
                    return String.Format("{0:dd/MM/yyyy}", dt_ultima_aula);
                }

                return "";
            }
        }
        public int? cd_turma_ppt { get; set; }
    }
}