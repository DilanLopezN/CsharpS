using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface IItemMovimentoDataAccess : IGenericRepository<ItemMovimento>
    {
        IEnumerable<ItemMovimento> getItensMovimentoByMovimento(int cd_movimento, int cd_empresa);
        IEnumerable<ItemMovimento> getItensMovimento(int cd_movimento, int cd_empresa);
        IEnumerable<ItemMovimento> getItensByAluno(SearchParameters parametros, int cd_pessoa, int cd_aluno, int cd_escola);
        List<ItemMovimento> getItensMaterialAluno(List<int> cdAlunos, int cd_empresa, int cd_turma);
        List<ItemMovimento> getItensMvto(int cd_movimento, int cd_escola);
        IEnumerable<ItemMovimento> getItensMovimentoRecibo(int cd_movimento, int cd_empresa);
        IEnumerable<ItemMovimento> getSomatorioValoresItensMovimentoDevolucao(int cd_movimento, int cd_empresa);
    }
}
