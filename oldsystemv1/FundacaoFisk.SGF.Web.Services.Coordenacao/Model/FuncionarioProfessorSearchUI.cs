using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class FuncionarioProfessorSearchUI
    {
        public FuncionarioSGF funcionarioSGF { get; set; }
        public List<HablitacaoProfessorUI> HablitacoesProfessorUI { get; set; }
        public List<Turma> turmaAtivas { get; set; }
        public static FuncionarioProfessorSearchUI changeValueFuncionaioToFuncionarioProfessorSearchUI(Professor funcionario, List<ProdutoFuncionario> produtosFuncionario)
        {
            FuncionarioProfessorSearchUI funcionarioProfessorSearchUI = new FuncionarioProfessorSearchUI();
            List<HablitacaoProfessorUI> hablitacoesProfessorUI = new List<HablitacaoProfessorUI>();
            funcionarioProfessorSearchUI.funcionarioSGF = funcionario;
            funcionarioProfessorSearchUI.turmaAtivas = funcionario.turmaAtivas;
            if (funcionario != null && funcionario.id_professor == true)
            {
                if (funcionario != null && funcionario.HabilitacaoProfessor != null
                    && funcionario.HabilitacaoProfessor.Count() > 0)
                {
                    foreach (var f in funcionario.HabilitacaoProfessor)
                    {
                        hablitacoesProfessorUI.Add(new HablitacaoProfessorUI
                        {
                            cd_professor = f.cd_professor,
                            cd_produto = f.cd_produto,
                            cd_estagio = f.cd_estagio,
                            no_estagio = f.Estagio != null ? f.Estagio.no_estagio : "",
                            no_produto = f.Produto != null ? f.Produto.no_produto : ""
                        });
                    }
                    funcionario.HabilitacaoProfessor = null;
                }
            }
            if (hablitacoesProfessorUI != null && hablitacoesProfessorUI.Count() > 0)
                funcionarioProfessorSearchUI.HablitacoesProfessorUI = hablitacoesProfessorUI;

            funcionarioProfessorSearchUI.funcionarioSGF.ProdutoFuncionario = produtosFuncionario;


                return funcionarioProfessorSearchUI;
        }
    }

    public class HablitacaoProfessorUI
    {
       public int cd_professor  {get;set;}
       public int cd_produto    {get;set;}
       public int cd_estagio    {get;set;}
       public string no_produto {get;set;}
       public string no_estagio {get;set;}
    }
}
