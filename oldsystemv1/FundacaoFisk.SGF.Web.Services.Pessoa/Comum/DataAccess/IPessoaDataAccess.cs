using System;
using System.Collections.Generic;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    using System.Data.Entity;
    using Componentes.GenericDataAccess.Comum;
    using FundacaoFisk.SGF.GenericModel;
    public interface IPessoaDataAccess : IGenericRepository<PessoaSGF>
    {
        IEnumerable<PessoaSearchUI> GetPessoaSearch(SearchParameters parametro, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, byte tipo_pesquisa);
        IEnumerable<PessoaSearchUI> GetPessoaResponsavelCPFSearch(SearchParameters parametros, string nome, string apelido, bool inicio, string cnpjCpf, int sexo);
        void PostNovaPessoa(PessoaSGF pessoa);
        PessoaJuridicaSGF postPessoaJuridica(PessoaJuridicaSGF pessoaJuridicaSGF);
        PessoaFisicaSGF postPessoaFisica(PessoaFisicaSGF pessoaFisicaSGF);
        void PostNovaPessoaFisica(PessoaFisicaSGF pessoaFisicaSGF);
        TelefoneSGF PostNovoContato(TelefoneSGF contato);
        IEnumerable<RelacionamentoSGF> GetRelacionamentoPorCodPessoaPai(int codPessoaPai);
        IEnumerable<RelacionamentoSGF> GetRelacionamentoPorCodPessoaFilho(int codPessoaFilho);
        void PostNovoEndereco(EnderecoSGF endereco);
        PessoaSGF FindIdPessoa(int codPessoa);
        PessoaJuridicaSGF FindIdPessoaJuridica(int codPessoa);
        PessoaFisicaSGF FindIdPessoaFisica(int codPessoaFisica);
        void PutPessoa(PessoaSGF pessoa);
        void PutPessoaFisica(PessoaFisicaSGF pessoaFisicaSGF);
        void PutEndereco(EnderecoSGF endereco);
        void PutTelefone(TelefoneSGF telefone);
        void DeletePessoa(PessoaSGF pessoa);
        void DeletePessoaFisica(PessoaFisicaSGF pessoaFisicaSGF);
        void DeleteTelefone(TelefoneSGF telefone);
        void DeleteEndereco(EnderecoSGF endereco);
        PessoaFisicaSearchUI VerificarExisitsPessoByCpfOrCdPessoa(string cpf, string email, string nome, int cd_pessoa_cpf, int? cdPessoa, int cd_escola);
        PessoaFisicaSearchUI VerificaExisitsAlunoRafMatricula(int? cdPessoa, int cd_escola);
        PessoaFisicaSGF VerificarPessoByCpf(string cpfCgc);
        PessoaJurdicaSearchUI VerificarExisitsEmpresaByCnpjOrcdEmpresa(string cnpj, int? cdEmpresa);
        PessoaJuridicaSGF VerificarEmpresaByCnpj(string cnpj);
        PessoaSGF retonaPessoaFirstOrDefault();
        Boolean PostdeletePessoa(List<PessoaSGF> pessoas);
        PessoaSGF getPessoaImage(int codPessoa);
        PessoaFisicaSGF VerificarPessoByCdPessoa(int cdPessoa);
        PessoaFisicaSGF ExistsPessoFisicaByCpf(string cpf);
        PessoaJuridicaSGF ExistsPessoaJuridicaByCnpj(string Cgc);
        PessoaFisicaSGF verificarPessoaByEmail(string email);
        //PessoaJuridicaUI getManyDependPessoaJuridica(int cdPessoaJuridica);
        IEnumerable<PessoaSearchUI> GetPessoaResponsavelSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int cdPai, int sexo, int papel);
        IEnumerable<PessoaSearchUI> GetPessoaSearchRelac(SearchParameters parametros, string nome, string apelido, int tipoPessoa, string cnpjCpf, int cd_pessoa_empresa);
        IEnumerable<PessoaSearchUI> getTdsPessoaSearchEscolaCadMovimento(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa);
        IEnumerable<PessoaSGF> getPessoasByCds(int[] cdPessoas, int cd_empresa);
        PessoaFisicaSGF getPessoaQueUsoCpf(int codPessoa);
        PessoaSGF ExistsPessoByCpf(string cpf);
        PessoaSGF ExistsPessoaByCNPJ(string Cgc);
        PessoaFisicaSGF verificarPessoaFisicaEmail(string email);
        PessoaFisicaSGF UpdateValuesEnderecoPrincipal(PessoaFisicaSGF pessoaFisicaContext, PessoaFisicaSGF pessoaFisicaView);

        IEnumerable<PessoaSearchUI> getPessoasResponsavel(int papel, int cd_escola);
        IEnumerable<GrupoEstoque> findAllGrupoAtivo(int cd_grupo, bool isMasterGeral);
    }
}
