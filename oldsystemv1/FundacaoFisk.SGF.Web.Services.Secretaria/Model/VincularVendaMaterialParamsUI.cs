using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class VincularVendaMaterialParamsUI : TO
    {
        public int? cd_movimento { get; set; }
        public int? cd_turma_origem {get; set;}
        public int? cd_contrato {get; set;}
    }
}