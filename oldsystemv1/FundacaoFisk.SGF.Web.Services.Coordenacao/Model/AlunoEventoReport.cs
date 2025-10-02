using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model {
    public class AlunoEventoReport : TO {
        public int cd_aluno { get; set; }
        public int cd_evento { get; set; }
        public int cd_funcionario { get; set; }
        public int cd_turma { get; set; }
        public string no_professor { get; set; }
        public string no_aluno { get; set; }
        public string dc_fone_mail_principal { get; set; }
        public string dc_fone_mail { get; set; }
        public string dc_fone {
            get {
                string retorno = dc_fone_mail;
                if(!string.IsNullOrEmpty(dc_fone_mail_principal) && !string.IsNullOrEmpty(dc_fone_mail))
                    retorno += " / ";
                retorno += dc_fone_mail_principal;
                return retorno;
            }
        }
        public string no_evento { get; set; }
        public string no_turma { get; set; }
        public int qtd_evento { get; set; }
        public int qtd_consecultiva { get; set; }

        public int cd_diario_aula { get; set; }
        public short? nm_aula_turma { get; set; }

        public string dta_aula { get; set; }
        public System.DateTime dt_aula { get; set; }
        public System.TimeSpan hr_inicial_aula { get; set; }
    }
}