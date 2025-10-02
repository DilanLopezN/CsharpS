using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Service.EmailMarketing.Model;
//using FundacaoFisk.SGF.Web.Service.EmailMarketing.Model;
//using FundacaoFisk.SGF.Web.Services.EmailMarketing.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IDataAccess
{
    public interface IListaEnderecoMalaDataAccess : IGenericRepository<ListaEnderecoMala>
    {
        IEnumerable<ListaEnderecoMala> getListagemEnderecosComporMensagemProspect(MalaDireta mala_direta);
        IEnumerable<ListaEnderecoMala> getListagemEnderecosComporMensagemAluno(MalaDireta mala_direta);
        IEnumerable<ListaEnderecoMala> getListagemEnderecosComporMensagemCliente(MalaDireta mala_direta);
        IEnumerable<ListaEnderecoMala> getListagemEnderecosComporMensagemPessoaRelacionada(MalaDireta mala_direta, bool filtro_empresa = true);
        IEnumerable<ListaEnderecoMala> getListEndComporMsgFuncProfissao(MalaDireta mala_direta);
        IEnumerable<ListaEnderecoMala> getListagemEndAlunosInadimplentes(MalaDireta mala_direta);
        IEnumerable<ListaNaoInscrito> getListagemEnderecosMalaDireta(int cd_empresa, int cd_mala_direta);
        bool existeEmailListagemEscola(int cd_empresa, int cd_cadastro, int id_cadastro);
        IEnumerable<ListaNaoInscrito> getListagemEnderecos(int cd_empresa, string no_pessoa, int status, string email, byte id_tipo_cadastro);
        IEnumerable<RptListagemEndereco> getRptListagemEnderecos(int cd_empresa, string no_pessoa, int status, string email, byte id_tipo_cadastro);
        IEnumerable<RptListagemEndereco> getRptListagemEnderecosMalaDireta(int cd_empresa, int cd_mala_direta);
        bool existEnderecoPrincipalPessoa(int cd_pessoa);
    }
}
