using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class EnviarTransferenciaComponentesCadParams
    {
        public string emailOrigem { get; set; }
        public List<MotivoTransferenciaAluno> MotivosTransferencia { get; set; }
    }
}