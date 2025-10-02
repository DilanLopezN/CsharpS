using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class RptContVendasMaterial : TO
    {
        public string no_aluno { get; set; }
        public string no_item { get; set; }
        public string no_turma { get; set; }
        public string nm_movimento { get; set; }
        public string nm_contrato { get; set; }
    }
}
