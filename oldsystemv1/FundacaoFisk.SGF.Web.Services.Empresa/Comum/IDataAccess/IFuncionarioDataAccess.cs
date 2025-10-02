using System;
using System.Collections.Generic;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess
{
    
    public interface IFuncionarioDataAccess : IGenericRepository<FuncionarioSGF>
    {
        IEnumerable<FuncionarioSearchUI> getSearchFuncionario(SearchParameters parametros, string nome, string apelido, bool? status, string cpf, bool inicio, byte tipo, int cdEscola, int sexo, int cdAtividade);
        IEnumerable<FuncionarioSearchUI> getSearchFuncionarioComAtividadeExtra(SearchParameters parametros, string nome, string apelido, bool? status, string cpf, bool inicio, byte tipo, int cdEscola, int sexo, int cdAtividade);
        FuncionarioSGF getFuncionarioByCpf(string cpf, int cdEscola);
        bool addEmpresaPessoa(PessoaEscola pessoaEscola);
        FuncionarioSearchUI getFuncionarioSearchUIById(int cd_funcionario, int cd_empresa);
        FuncionarioSGF fidFuncionarioById(int cd_func, int cd_empresa);
        IEnumerable<FuncionarioSearchUI> getFuncionarios(int cd_pessoa_empresa, int? cd_funcionario, FuncionarioSGF.TipoConsultaFuncionarioEnum tipo);
        int getFuncionarioByIdPessoa(int cd_pessoa);

        FuncionarioCyberBdUI findFuncionarioByCdFuncionario(int cd_funcionario, int cd_empresa);
    }
}
