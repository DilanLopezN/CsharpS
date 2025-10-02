using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class HistoricoAlunoReportUI : TO
    {
        public string produtos { get; set; }
        public string turmaAvaliacao { get; set; }
        public string estagioAvaliacao { get; set; }
        public string statusTitulo { get; set; }
        public bool mostrarEstagio { get; set; }
        public bool mostrarAtividade { get; set; }
        public bool mostrarObservacao { get; set; }
        public bool mostrarFollow { get; set; }
        public bool mostrarItem { get; set; }
        public int cd_aluno { get; set; }
        public string no_aluno { get; set; }
    }
}
