using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public class TurmaEscolaSearchUI : TO
    {
        public int cd_turma_escola { get; set; }
        public int cd_turma { get; set; }
        public int cd_escola { get; set; }
        public string dc_reduzido_pessoa { get; set; }

        public static TurmaEscolaSearchUI FromTurmaEscolaSearchUi(TurmaEscolaSearchUI turmaEscolaSearch)
        {
            TurmaEscolaSearchUI pessoaSearchUI = new TurmaEscolaSearchUI();
            pessoaSearchUI.cd_turma_escola = turmaEscolaSearch.cd_turma_escola;
            pessoaSearchUI.cd_turma = turmaEscolaSearch.cd_turma;
            pessoaSearchUI.cd_escola = turmaEscolaSearch.cd_escola;
            pessoaSearchUI.dc_reduzido_pessoa = turmaEscolaSearch.dc_reduzido_pessoa;

            return pessoaSearchUI;
        }

    }
}