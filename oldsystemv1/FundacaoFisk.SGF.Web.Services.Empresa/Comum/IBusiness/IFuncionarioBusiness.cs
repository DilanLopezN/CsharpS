using System;
using System.Collections.Generic;
using Componentes.GenericBusiness.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness {
    using System.Data.Entity;
    using FundacaoFisk.SGF.Web.Services.Empresa.Model;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
    using FundacaoFisk.SGF.Web.Services.Usuario.Model;

    public interface IFuncionarioBusiness : IGenericBusiness
    {
        void sincronizaContextoFuncionarioComponentes(DbContext db);
        //Funcionario
        IEnumerable<FuncionarioSearchUI> getSearchFuncionario(SearchParameters parametros, string nome, string apelido, bool? status, string cpf, bool inicio, byte tipo, int cdEscola, int sexo,int cdAtividade);
        IEnumerable<FuncionarioSearchUI> getSearchFuncionarioComAtividadeExtra(SearchParameters parametros, string nome, string apelido, bool? status, string cpf, bool inicio, byte tipo, int cdEscola, int sexo, int cdAtividade);
        PessoaFisicaSearchUI ExistFuncionarioOrPessoaFisicaByCpf(string cpf, int cdEscola);
        FuncionarioSGF editFuncionario(FuncionarioSGF funcionarioView, PessoaFisicaUI pessoaFisicaUI, List<RelacionamentoSGF> relacionamentos);
        FuncionarioSearchUI getFuncionarioSearchUIById(int cd_funcionario, int cd_empresa);
        bool addEmpresaPessoa(PessoaEscola pessoaEmpresa);
        FuncionarioSGF addFuncionario(FuncionarioSGF funcionario);
        void saveFuncionario(bool dispose);
        IEnumerable<FuncionarioSearchUI> getFuncionarios(int cd_pessoa_empresa, int? cd_funcionario, FuncionarioSGF.TipoConsultaFuncionarioEnum tipo);
        int getFuncionarioByIdPessoa(int cd_pessoa);

        FuncionarioCyberBdUI findFuncionarioByCdFuncionario(int cd_funcionario, int cd_empresa);
    }
}
