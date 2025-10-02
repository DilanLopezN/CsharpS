namespace FundacaoFisk.SGF.GenericModel
{
    public class EscolaApiAreaRestritaBdUI
    {
        public int cd_pessoa { get; set; }
        public int codigo { get; set; }
        public int codigo_antigo { get; set; }
        public int? nm_cliente_integracao { get; set; }
        public string nome_unidade { get; set; }
        public string email { get; set; }
        public bool escola_ativa { get; set; }
    }
}