using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class AlunoStatusUI : TO
    {
        public int cd_aluno { get; set; }
        public int cd_pessoa_aluno { get; set; }
        public int cd_pessoa_escola { get; set; }
        public int cd_pessoa { get; set; }
    }
}