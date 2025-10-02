using System;
using System.Text;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Model
{
    using Componentes.GenericModel;
    public class EmpresaUsuarioSession : TO
    {
        public int cd_pessoa { get; set; }
        public string dc_reduzido_pessoa { get; set; }
    }
}
