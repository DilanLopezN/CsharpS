namespace FundacaoFisk.SGF.GenericModel.Partial
{
    public class FuncionarioCyberBdUI
    {
        public string nome { get; set; }
        public int? id_unidade { get; set; }
        public int codigo { get; set; }
        public string email { get; set; }
        public bool funcionario_ativo { get; set; }
        public byte tipo_funcionario { get; set; }
    }
}