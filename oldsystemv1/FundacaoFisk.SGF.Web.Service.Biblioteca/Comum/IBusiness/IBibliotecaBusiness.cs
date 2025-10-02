using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericBusiness.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Service.Biblioteca.Model;
using FundacaoFisk.SGF.Web.Services.Biblioteca.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Biblioteca.Comum.IBusiness
{
    public interface IBibliotecaBusiness : IGenericBusiness
    {
        void sincronizarContextos(DbContext dbContext);

        IEnumerable<PessoaSearchUI> getPessoaBibliotecaSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa);
        IEnumerable<PessoaSearchUI> getPessoaEmprestimoSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa);
        IEnumerable<Emprestimo> getEmprestimoSearch(SearchParameters parametros, int? cd_pessoa, int? cd_item, bool? pendentes, DateTime? dt_inicial, DateTime? dt_final, bool? emprestimos, bool? devolucao, int cd_empresa);
        Emprestimo addEmprestimo(Emprestimo emprestimo, int cd_escola, int saldo);
        Emprestimo postEditEmprestimo(Emprestimo emprestimo, int cd_escola);
        Emprestimo getEmprestimo(Parametro parametro, int cd_biblioteca, int cd_escola);
        bool deleteEmprestimos(List<Emprestimo> emprestimos, int cd_escola);
        Emprestimo getEmprestimoById(int cd_emprestimo);
        EmprestimoSearch getEmprestimoById(int cd_biblioteca, int cd_empresa);
    }
}
