using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class ProspectIntegracaoUI : TO
    {
        public enum StatusProcedure
        {
            SUCESSO_EXECUCAO_PROCEDURE = 0,
            ERRO_EXECUCAO_PROCEDURE = 1
        }

        public Nullable<int> unit_fisk_id { get; set; }
        public Nullable<byte> id_tipo { get; set; }
        public Nullable<int> id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string cep { get; set; }
        public string day_week { get; set; }
        public string period { get; set; }
        public Nullable<System.DateTime> dt_cadastro { get; set; }
        public string sexo { get; set; }
        public Nullable<double> hit_percentage { get; set; }
        public string phase { get; set; }
        public string course_id { get; set; }

    }
}
