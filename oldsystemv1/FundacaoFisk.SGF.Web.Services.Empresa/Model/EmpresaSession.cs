using System;
using System.Text;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Model
{
    using Componentes.GenericModel;
    public class EmpresaSession : TO
    {
        public int cd_pessoa { get; set; }
        public string dc_reduzido_pessoa { get; set; }
        public TimeSpan? hr_inicial { get; set; }
        public TimeSpan? hr_final { get; set; }
        public int id_fuso_horario { get; set; }
        public bool id_horario_verao { get; set; }
    }
}
