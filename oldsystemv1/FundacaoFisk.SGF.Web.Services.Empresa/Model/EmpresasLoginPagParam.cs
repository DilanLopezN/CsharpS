namespace FundacaoFisk.SGF.Web.Services.Empresa.Model
{
    public class EmpresasLoginPagParam
    {
        public int cd_usuario { get; set; }
        public string cd_empresas { get; set;}
        public string nome { get; set;}
        public string fantasia { get; set;}
        public string cnpj { get; set;}
        public bool inicio { get; set; }
    }
}