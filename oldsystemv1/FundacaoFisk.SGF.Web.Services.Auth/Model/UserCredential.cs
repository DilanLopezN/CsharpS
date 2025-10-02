using System.ComponentModel.DataAnnotations;
using System;

namespace FundacaoFisk.SGF.Web.Services.Auth.Model {
    public class UserCredential {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }

        public TimeSpan? HrInicial { get; set; } // Horário inicial de funcionamento da empresa 
        public TimeSpan? HrFinal { get; set; } // Horário final de funcionamento da empresa 
        public bool EhMasterGeral { get; set; }
        public int? CodEmpresa { get; set; }
        public int nmMaxTentativas { get; set; }
        public int IdFusoHorario { get; set; }
        public bool IdHorarioVerao { get; set; }
        public static string permissoes { get; set; }
    }
}
