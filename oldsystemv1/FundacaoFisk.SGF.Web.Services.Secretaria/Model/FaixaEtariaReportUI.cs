using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class FaixaEtariaReportUI : TO
    {
        public int tipo { get; set; }
        public int idade { get; set; }
        public int idade_max { get; set; }
        public int cd_turma { get; set; }
        public string no_turma { get; set; }
    }
}