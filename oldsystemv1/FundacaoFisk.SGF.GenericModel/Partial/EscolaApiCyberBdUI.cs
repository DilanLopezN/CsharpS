using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{ 
    public class EscolaApiCyberBdUI: TO
    {
        //public string nome { get; set; }
        //public string email { get; set; }
        //public string cidade { get; set; }
        //public string estado { get; set; }

        public int cd_pessoa { get; set; }
        public int codigo { get; set; }
        public int codigo_antigo { get; set; }
        public int? nm_cliente_integracao { get; set; }
        public string nome_unidade { get; set; }
        public string email { get; set; }
        public int cd_cidade { get; set; }
        public string cidade { get; set; }
        public int cd_estado { get; set; }
        public string estado { get; set; }
        public bool escola_ativa { get; set; }
    }
}