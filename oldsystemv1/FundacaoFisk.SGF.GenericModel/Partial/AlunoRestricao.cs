using System;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class AlunoRestricao
    {

        public string dc_orgao_financeiro { get; set; }
        public string no_responsavel { get; set; }
        public int id_grid_aluno_restricao { get; set; }

        public string dtaInicioRestricao
        {
            get
            {
                if (dt_inicio_restricao != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_inicio_restricao);
                else
                    return String.Empty;
            }
        }

        public string dtaFinalRestricao
        {
            get
            {
                if (dt_final_restricao != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_final_restricao);
                else
                    return String.Empty;
            }
        }
        public string dtaCadastro
        {
            get
            {
                if (dt_cadastro != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_cadastro);
                else
                    return String.Empty;
            }
        }
        
    }
}