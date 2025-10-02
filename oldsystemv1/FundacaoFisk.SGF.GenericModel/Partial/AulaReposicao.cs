using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class AulaReposicao : TO
    {
        public string no_turma { get; set; }
        public string no_usuario { get; set; }
        public string no_professor { get; set; }
        public string tempo
        {
            get
            {
                return string.Format("{0} - {1}", dh_final_evento.ToString(@"hh\:mm"), dh_final_evento.ToString(@"hh\:mm"));
            }
        }
    }
}