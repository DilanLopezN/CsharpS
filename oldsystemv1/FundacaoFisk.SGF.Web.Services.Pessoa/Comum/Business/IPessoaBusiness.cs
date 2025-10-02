using System;
using System.Collections.Generic;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using System.Data.Entity;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business
{
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
    using Componentes.GenericBusiness.Comum;
    public interface IPessoaBusiness : IGenericBusiness
    {
        SGFWebContext DBPessoa();
        void sincronizaContexto(DbContext db);
        // Pessoa
        IEnumerable<PessoaSearchUI> GetPessoaSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, byte tipo_pesquisa);
        IEnumerable<PessoaSearchUI> GetPessoaResponsavelCPFSearch(SearchParameters parametros, string nome, string apelido, bool inicio, string cnpjCpf, int sexo);
        void PostNovaPessoa(PessoaSGF pessoa, PessoaFisicaSGF pessoaFisica,string telefone, string email,EnderecoSGF endereco);
        PessoaSGF FindIdPessoa(int codPessoa);
        void deletePessoa(int codPessoa);
        Boolean PostdeletePessoa(List<PessoaSGF> pessoas, int cd_pessoa);
        PessoaSGF getPessoaImage(int codPessoa);
        IEnumerable<PessoaSearchUI> GetPessoaResponsavelSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int cdPai, int sexo, int papel);
        IEnumerable<PessoaSearchUI> GetPessoaSearchRelac(SearchParameters parametros, string nome, string apelido, int tipoPessoa, string cnpjCpf, int cd_pessoa_empresa);
        IEnumerable<PessoaSearchUI> getTdsPessoaSearchEscolaCadMovimento(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa);
        IEnumerable<PessoaSearchUI> getPessoasResponsavel(int papel, int cd_escola);

        //Relacionamento
        IEnumerable<RelacionamentoSGF> GetRelacionamentoPorCodPessoaFilho(int codPessoaFilho);
        List<RelacionamentoSGF> GetRelacionamentoPorCodPessoaPai(int codPessoaPai);
        RelacionamentoSGF inserirPessoaFromRelacionamento(RelacionamentoSGF relac, int cdPessoaPai);

        //Pessoa Físca
        PessoaFisicaSGF VerificarPessoByCpf(string cpfCgc);
        PessoaFisicaSGF VerificarPessoByCdPessoa(int cdPessoa);
        PessoaFisicaSGF ExistsPessoFisicaByCpf(string cpf);
        PessoaFisicaSGF postInsertPessoaFisica(PessoaFisicaUI pessoaFisica, List<RelacionamentoSGF> relacionamentos, bool especializado);
        PessoaFisicaSGF postUpdatePessoaFisica(PessoaFisicaUI pessoaFisica, List<RelacionamentoSGF> relacionamentos, bool especializado, bool novo);
        void PostPessoaEPessoaFisica(PessoaFisicaSGF pessoaFisica);
        PessoaFisicaSearchUI VerificarExisitsPessoByCpfOrCdPessoa(string cpf, string email, string nome, int cd_pessoa_cpf, int? cdPessoa, int cd_escola);
        PessoaFisicaSearchUI VerificaExisitsAlunoRafMatricula(int? cdPessoa, int cd_escola);
        PessoaFisicaSGF getPessoaFisicaBycdPessoa(int cdPessoa, bool dispose);
        PessoaSGF addNewPessoaFisica(PessoaFisicaSGF pessoaFisicaSGF, EnderecoSGF endereco);
        PessoaFisicaSGF editPessoaFisica(PessoaFisicaSGF pessoaFisica);
        PessoaFisicaSGF verificarPessoaByEmail(string email);
        PessoaFisicaSGF verificarPessoaFisicaEmail(string email);

        //PessoaJuridica
        PessoaJuridicaSGF postInsertPessoaJuridica(PessoaJuridicaUI pessoaJuridica, List<RelacionamentoSGF> relacionamentos, bool especializado);
        PessoaJuridicaSGF postUpdatePessoaJuridica(PessoaJuridicaUI pessoaJuridicaUI, List<RelacionamentoSGF> relacionamentos, bool especializado);
        PessoaJuridicaSGF ExistsPessoaJuridicaByCnpj(string Cgc);
        PessoaJurdicaSearchUI VerificarExisitsEmpresaByCnpjOrcdEmpresa(string cnpj, int? cdEmpresa);
        PessoaJuridicaSGF VerificarEmpresaByCnpj(string cnpj);
     
        //Telefone
        TelefoneSGF addTelefone(TelefoneSGF telefone);
        TelefoneSGF editTelefone(TelefoneSGF telefone);
        TelefoneSGF FindTypeTelefone(int cdPessoa, int tipoTel);
        TelefoneSGF saveChangesTelefone(TelefoneSGF telefone);
        TelefoneSGF FindTypeTelefonePrincipal(int cdPessoa,int tipoTel);
        IEnumerable<TelefoneSGF> getAllTelefonesByPessoa(int cdPessoa);
        IEnumerable<TelefoneSGF> getAllTelefonesContatosByPessoa(int cdPessoa);
        Boolean deletedTelefone(TelefoneSGF telefone);
        TelefoneSGF findTelefoneById(int cdTelefone);
        void addEditTipoContato(string entityContato, int escola, int tipoCrud, int tipoContato, int? codOperadora, TelefoneSGF contatoExists);

        //Atividade
        //IEnumerable<AtividadePessoa> getAllListAtividades(string searchText, int natureza, bool? status);

        //Orgao Expedidor
        IEnumerable<TratamentoPessoa> getAllTratamentoPessoa();

        //Tratamento Pessoa
        IEnumerable<EstadoCivilSGF> getAllEstadoCivil();

        //Estado Civil
        IEnumerable<OrgaoExpedidor> getAllOrgaoExpedidor();

        // papel
        IEnumerable<PapelSGF> GetAllPapel();
        PapelSGF GetPapelByCdPapel(int cdPapel);
        IEnumerable<PapelSGF> getPapelByTipo(int[] tipo);
        
        //Qualif Relacionamento
        IEnumerable<QualifRelacionamento> getAllQualifRelacByPapel(int codPapel);

        //Relacionamento
        RelacionamentoSGF addRelacionamento(RelacionamentoSGF relacionamento);
        RelacionamentoSGF editRelacionamento(RelacionamentoSGF relacionamento);
        Boolean deleteRelacioanamento(RelacionamentoSGF relacionamento);
        void setRelacionamentos(List<RelacionamentoSGF> relacionamentos, int cdPessoaPai, bool edicao);
        bool addRelacionamentoResponsavelAluno(RelacionamentoSGF pessoaRelac);

        //Endereço
        void setOutrosEnderecos(ICollection<EnderecoSGF> enderecosUI, int cdPessoa, int? cd_endereco_principal);
        EnderecoSGF persistirEndereco(EnderecoSGF enderecoUI, int cdPessoa, int? cd_endereco_principal);
     
        //Contatos
        void setOutrosContatos(List<TelefoneSGF> telefonesUI, int cdPessoa);

        PessoaSGF findPessoaById(int id);
        bool deletePessoa(PessoaSGF pessoa);
        PessoaSGF ExistsPessoByCpf(string cpf);
        PessoaSGF ExistsPessoaByCNPJ(string Cgc);
        List<GrupoEstoque> findAllGrupoAtivo(int cd_grupo, bool isMasterGeral);
    }
}
