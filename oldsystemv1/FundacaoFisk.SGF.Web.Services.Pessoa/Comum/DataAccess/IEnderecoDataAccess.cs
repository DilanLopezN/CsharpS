using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    public interface IEnderecoDataAccess: IGenericRepository<EnderecoSGF>
    {
        IEnumerable<EnderecoSGF> GetAllEnderecoByPessoa(int cdPessoa, int cd_endereco);
        EnderecoSGF getEnderecoByLogradouro(int cd_logradouro, string nm_cep);
        EnderecoSGF getEnderecoResponsavelCPF(int cd_pessoa);
        EnderecoSGF getEnderecoByCdEndereco(int cd_endereco);
    }
}
