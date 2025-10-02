using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public class AtividadeEscolaAtividadeSearchUI : TO
    {

        public int cd_atividade_escola { get; set; }
        public int cd_atividade_extra { get; set; }
        public int cd_escola { get; set; }
        public string dc_reduzido_pessoa { get; set; }

        public static AtividadeEscolaAtividadeSearchUI FromAtividadeEscolaAtividadeSearchUi(AtividadeEscolaAtividadeSearchUI atividadeEscolaAtividadeSearch)
        {
            AtividadeEscolaAtividadeSearchUI pessoaSearchUI = new AtividadeEscolaAtividadeSearchUI();
            pessoaSearchUI.cd_atividade_escola = atividadeEscolaAtividadeSearch.cd_atividade_escola;
            pessoaSearchUI.cd_atividade_extra = atividadeEscolaAtividadeSearch.cd_atividade_extra;
            pessoaSearchUI.cd_escola = atividadeEscolaAtividadeSearch.cd_escola;
            pessoaSearchUI.dc_reduzido_pessoa = atividadeEscolaAtividadeSearch.dc_reduzido_pessoa;

            return pessoaSearchUI;
        }
    }
}