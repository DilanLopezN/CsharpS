using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Componentes.GenericModel;
namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class AtividadeProspectUI : TO
    {
        public string no_pessoa { get; set; }
        public int cd_atividade_prospect { get; set; }
        public int cd_atividade_extra { get; set; }
        public int cd_prospect{ get; set; }
        public bool ind_participacao { get; set; }
        public string txt_obs_atividade_prospect { get; set; }
        public string dc_reduzido_pessoa_escola { get; set; }
        public string dc_tipo_atividade_extra { get; set; }

        //Propriedade do relatório de atividade extra:
        public string participou
        {
            get
            {
                if (this.ind_participacao)
                    return "Sim";
                else
                    return "Não";
            }
        }
    }
}