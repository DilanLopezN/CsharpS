using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Model
{
    public class UsuarioEmpresaLoginUI
    {
        public List<EmpresaLoginUI> Empresas { get; set; }
        public UsuarioWebSGF usuario { get; set; }

        public void fromEmpresa(List<EmpresaSession> escolas)
        {
            Empresas = new List<EmpresaLoginUI>();
            for(int i = 0; i < escolas.Count; i++) {
                EmpresaLoginUI e = new EmpresaLoginUI();

                e.cd_pessoa = escolas[i].cd_pessoa;
                e.no_pessoa = escolas[i].dc_reduzido_pessoa != null ? escolas[i].dc_reduzido_pessoa : "";

                Empresas.Add(e);
            }
        }
    }

    public class EmpresaLoginUI
    {
        public int cd_pessoa { get; set; }
        public string no_pessoa { get; set; }
    }
}
