namespace FundacaoFisk.SGF.GenericModel.Partial
{
    public class PessoaCoordenadorCyberBdUI
    {
        public string nome { get; set; }
        public int? id_unidade { get; set; }
        public int codigo { get; set; }
        public string email { get; set; }
        public bool pessoa_ativa { get; set; }
        public byte tipo_funcionario { get; set; }
    }
}