using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Business
{
    using System.Transactions;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Comum;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
    using System.Data.Entity;
    using Componentes.Utils.Messages;
    using Componentes.GenericBusiness;
    using FundacaoFisk.SGF.GenericModel;

    public class PessoaBusiness : IPessoaBusiness
    {
        public IPessoaDataAccess DataAccess { get; set; }
        public IPessoaFisicaDataAccess DataAccessPessoaFisica { get; set; }
        public ITelefoneDataAccess DataAccessTelefone { get; set; }
        public IEstadoCivilDataAccess DataAccessEstadoCivil { get; set; }
        public ITratamentoPessoaDataAccess DataAccessTratamentoPessoa { get; set; }
        public IOrgaoExpedidorDataAccess DataAccessOrgaoExpedidor { get; set; }
        public IPapelDataAccess DataAccessPapel { get; set; }
        public IRelacionamentoDataAccess DataAccessRelacionamento { get; set; }
        public ILocalidadeBusiness BusinessLoc { get; set; }

        const int ADD = 1; int EDIT = 2;
        const int TELEFONE = 1; int EMAIL = 4; int SITE = 5; int CELULAR = 3;
        const int COMERCIAL = 1;

        public PessoaBusiness(IPessoaDataAccess dataAccess, ITelefoneDataAccess daoTelefone,
            IEstadoCivilDataAccess daoEstadoCivil, ITratamentoPessoaDataAccess daoTratamentoPessoa, 
            IOrgaoExpedidorDataAccess daoOrgaoExpedidor,IPapelDataAccess dataAccessPapel,
            IRelacionamentoDataAccess daoRelacionamento, ILocalidadeBusiness businessLoc, IPessoaFisicaDataAccess dataAccessPessoaFisica)

        {
            if (dataAccess == null || daoTelefone == null || daoEstadoCivil == null
                || daoTratamentoPessoa == null || daoOrgaoExpedidor == null
                || dataAccessPapel == null || daoRelacionamento == null || businessLoc == null || dataAccessPessoaFisica == null)
            {
                throw new ArgumentNullException("repository");
            }
            DataAccess = dataAccess;
            DataAccessTelefone = daoTelefone;
            DataAccessEstadoCivil = daoEstadoCivil;
            DataAccessTratamentoPessoa = daoTratamentoPessoa;
            DataAccessOrgaoExpedidor = daoOrgaoExpedidor;
            DataAccessPapel = dataAccessPapel;
            DataAccessRelacionamento = daoRelacionamento;
            BusinessLoc = businessLoc;
            DataAccessPessoaFisica = dataAccessPessoaFisica;
        }

        public void configuraUsuario(int cdUsuario,int cd_empresa) {
            // Configura os codigos do usuário para auditorias dos DataAccess:
             ((SGFWebContext)this.DataAccess.DB()).IdUsuario =  ((SGFWebContext)this.DataAccessTelefone.DB()).IdUsuario  =
             ((SGFWebContext)this.DataAccessEstadoCivil.DB()).IdUsuario =  ((SGFWebContext)this.DataAccessTratamentoPessoa.DB()).IdUsuario =  ((SGFWebContext)this.DataAccessOrgaoExpedidor.DB()).IdUsuario =
             ((SGFWebContext)this.DataAccessPapel.DB()).IdUsuario =  ((SGFWebContext)this.DataAccessRelacionamento.DB()).IdUsuario = ((SGFWebContext)this.DataAccessPessoaFisica.DB()).IdUsuario = cdUsuario;
             ((SGFWebContext)this.DataAccess.DB()).cd_empresa = ((SGFWebContext)this.DataAccessTelefone.DB()).cd_empresa  =
             ((SGFWebContext)this.DataAccessEstadoCivil.DB()).cd_empresa = ((SGFWebContext)this.DataAccessTratamentoPessoa.DB()).cd_empresa = ((SGFWebContext)this.DataAccessOrgaoExpedidor.DB()).cd_empresa =
             ((SGFWebContext)this.DataAccessPapel.DB()).cd_empresa = ((SGFWebContext)this.DataAccessRelacionamento.DB()).cd_empresa = ((SGFWebContext)this.DataAccessPessoaFisica.DB()).cd_empresa = cd_empresa;
            
            this.BusinessLoc.configuraUsuario(cdUsuario, cd_empresa);
        }

        public SGFWebContext DBPessoa()
        {
            return (SGFWebContext)DataAccess.DB();
        }

        public PessoaFisicaSGF verificarPessoaByEmail(string email)
        {
            return DataAccess.verificarPessoaByEmail(email);
        }

        /// <summary>
        /// métodos de persistencia da Pessoa
        /// </summary>
        /// <param name="parametros"></param>
        /// <param name="nome"></param>
        /// <param name="apelido"></param>
        /// <param name="status"></param>
        /// <param name="tipoPessoa"></param>
        /// <param name="cnpjCpf"></param>
        /// <returns></returns>
        #region pessoa

        public void sincronizaContexto(DbContext db)
        {

            //DataAccess.sincronizaContexto(db);
            //DataAccessTelefone.sincronizaContexto(db);
            //DataAccessEstadoCivil.sincronizaContexto(db);
            //DataAccessTratamentoPessoa.sincronizaContexto(db);
            //DataAccessOrgaoExpedidor.sincronizaContexto(db);
            //DataAccessPapel.sincronizaContexto(db);
            //DataAccessRelacionamento.sincronizaContexto(db);
            //BusinessLoc.sincronizaContexto(db);
        }

        public IEnumerable<PessoaSearchUI> GetPessoaSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, byte tipo_pesquisa)
        {
            IEnumerable<PessoaSearchUI> retorno = new List<PessoaSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_pessoa_ativa");
                parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
                retorno = DataAccess.GetPessoaSearch(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, tipo_pesquisa);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<PessoaSearchUI> GetPessoaResponsavelCPFSearch(SearchParameters parametros, string nome, string apelido, bool inicio, string cnpjCpf, int sexo)
        {
            IEnumerable<PessoaSearchUI> retorno = new List<PessoaSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_pessoa_ativa");
                parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
                retorno = DataAccess.GetPessoaResponsavelCPFSearch(parametros, nome, apelido, inicio, cnpjCpf, sexo);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<PessoaSearchUI> GetPessoaResponsavelSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int cdPai, int sexo, int papel)
        {
            IEnumerable<PessoaSearchUI> retorno = new List<PessoaSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_pessoa_ativa");
                parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
                retorno = DataAccess.GetPessoaResponsavelSearch(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, cdPai, sexo, papel);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<PessoaSearchUI> GetPessoaSearchRelac(SearchParameters parametros, string nome, string apelido, int tipoPessoa, string cnpjCpf, int cd_pessoa_empresa)
        {
            IEnumerable<PessoaSearchUI> retorno = new List<PessoaSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_pessoa_ativa");
                parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
                retorno = DataAccess.GetPessoaSearchRelac(parametros, nome, apelido, tipoPessoa, cnpjCpf, cd_pessoa_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public void PostNovaPessoa(PessoaSGF pessoa, PessoaFisicaSGF pessoaFisica, string telefone, string email, EnderecoSGF endereco)
        {
            pessoa.dt_cadastramento = DateTime.UtcNow;
            pessoa.nm_natureza_pessoa = 1;
            endereco.cd_loc_pais = (endereco.cd_loc_pais == 0) ? (1) : (endereco.cd_loc_pais);

            pessoaFisica.Nacionalidade = null;
            pessoaFisica.LocalNascimento = null;
           
            pessoa.Enderecos = null;
            pessoa.TelefonePessoa = null;

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                DataAccess.PostNovaPessoa(pessoa);

                DataAccess.PostNovaPessoaFisica(pessoaFisica);

                DataAccess.PostNovoContato(
                    new TelefoneSGF
                    {
                        cd_pessoa = pessoa.cd_pessoa,
                        cd_tipo_telefone = 1,
                        dc_fone_mail = !string.IsNullOrEmpty(telefone) && telefone != "undefined" ? telefone : "",
                        cd_classe_telefone = 3
                    });
               
                DataAccess.PostNovoContato(
                   new TelefoneSGF
                   {
                       cd_pessoa = pessoa.cd_pessoa,
                       cd_tipo_telefone = 4,
                       dc_fone_mail = !string.IsNullOrEmpty(email) && email != "undefined" ? email : "",
                       cd_classe_telefone = 3
                   });
                
                if ((endereco.cd_loc_cidade != 0) && (endereco.cd_loc_estado != 0))
                {
                    endereco.cd_pessoa = pessoa.cd_pessoa;
                    DataAccess.PostNovoEndereco(endereco);
                }
                transaction.Complete();
            }
        }

        public IEnumerable<RelacionamentoSGF> GetRelacionamentoPorCodPessoaFilho(int codPessoaFilho)
        {
            return DataAccess.GetRelacionamentoPorCodPessoaFilho(codPessoaFilho);
        }

        public List<RelacionamentoSGF> GetRelacionamentoPorCodPessoaPai(int codPessoaPai) {
            return DataAccess.GetRelacionamentoPorCodPessoaPai(codPessoaPai).ToList();
        }

        public PessoaSGF FindIdPessoa(int codPessoa)
        {
            return DataAccess.FindIdPessoa(codPessoa);
        }

        public PessoaSGF ManageChangesPessoa(PessoaSGF pessoaBase, PessoaSGF pessoa)
        {
            if (pessoa.no_pessoa != pessoaBase.no_pessoa)
                pessoaBase.no_pessoa = pessoa.no_pessoa;
            if (pessoa.dc_reduzido_pessoa != pessoaBase.dc_reduzido_pessoa)
                pessoaBase.dc_reduzido_pessoa = pessoa.dc_reduzido_pessoa;
            if (pessoa.dt_cadastramento != pessoaBase.dt_cadastramento)
                pessoaBase.dt_cadastramento = pessoa.dt_cadastramento;
            if (pessoa.cd_papel_principal != pessoaBase.cd_papel_principal)
                pessoaBase.cd_papel_principal = pessoa.cd_papel_principal;
            return pessoaBase;
        }
        
        public TelefoneSGF ManageChangesTelefone(TelefoneSGF telefone, string telefoneAtualizado)
        {
            telefone.dc_fone_mail = telefoneAtualizado;
            return telefone;
        }

        public EnderecoSGF ManageChangesEndereco(EnderecoSGF enderecoBase, EnderecoSGF endereco)
        {
            enderecoBase.cd_loc_estado = endereco.cd_loc_estado;
            enderecoBase.cd_loc_cidade = endereco.cd_loc_cidade;
            enderecoBase.cd_tipo_endereco = endereco.cd_tipo_endereco;
            enderecoBase.cd_loc_bairro = endereco.cd_loc_bairro;
            enderecoBase.cd_tipo_logradouro = endereco.cd_tipo_logradouro;
            enderecoBase.cd_loc_logradouro = endereco.cd_loc_logradouro;
            return enderecoBase;
        }

        public void deletePessoa(int codPessoa)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                PessoaSGF pessoaBase = DataAccess.findById(codPessoa,false);
                DataAccess.delete(pessoaBase,false);
                transaction.Complete();
            }
            
        }

        public Boolean PostdeletePessoa(List<PessoaSGF> pessoas, int cd_empresa)
        {
            int[] cdTitulosDelete = null;
            bool retorno = false;
            if (pessoas != null && pessoas.Count() > 0)
            {
                int[] cdPessoas = null;
                int i = 0;
                cdPessoas = new int[pessoas.Count()];
                foreach (var t in pessoas)
                {
                    cdPessoas[i] = t.cd_pessoa;
                    i++;
                }
                cdTitulosDelete = cdPessoas;
            }
           
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                 List<PessoaSGF> pessoasContext = DataAccess.getPessoasByCds(cdTitulosDelete, cd_empresa).ToList();
                 if (pessoasContext != null)
                     foreach (PessoaSGF p in pessoasContext)
                         retorno = DataAccess.delete(p, false);
                transaction.Complete();
            }
            return retorno;
        }

        public PessoaSGF getPessoaImage(int codPessoa)
        {
            return DataAccess.getPessoaImage(codPessoa);
        }

        public IEnumerable<PessoaSearchUI> getTdsPessoaSearchEscolaCadMovimento(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa)
        {
            IEnumerable<PessoaSearchUI> retorno = new List<PessoaSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_pessoa_ativa");
                parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
                parametros.sort = parametros.sort.Replace("nm_cpf_cgc_dependente", "nm_cpf_cgc");
                retorno = DataAccess.getTdsPessoaSearchEscolaCadMovimento(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public PessoaFisicaSGF verificarPessoaFisicaEmail(string email)
        {
            return DataAccess.verificarPessoaFisicaEmail(email);
        }

        public IEnumerable<PessoaSearchUI> getPessoasResponsavel(int papel, int cd_escola)
        {
            return DataAccess.getPessoasResponsavel(papel, cd_escola);
        }
        #endregion

        #region pessoa juridica

        public PessoaJuridicaSGF postInsertPessoaJuridica(PessoaJuridicaUI pessoaJuridicaUI, List<RelacionamentoSGF> relacionamentos, bool especializado)
        {
            this.sincronizaContexto(DataAccess.DB());
            PessoaJuridicaSGF pessoaJuridica = new PessoaJuridicaSGF();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                var ExistPessoaJuridicaCnpjBase = DataAccess.ExistsPessoaByCNPJ(pessoaJuridicaUI.pessoaJuridica.dc_num_cgc);
                if (ExistPessoaJuridicaCnpjBase != null && ExistPessoaJuridicaCnpjBase.cd_pessoa > 0)
                    throw new PessoaBusinessException(Messages.msgExistPersonCnpjBase + " " + ExistPessoaJuridicaCnpjBase.no_pessoa, null, FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_CNPJJAEXISTENTE, false);
                pessoaJuridicaUI.pessoaJuridica.nm_natureza_pessoa = 2;
                pessoaJuridica = DataAccess.postPessoaJuridica(pessoaJuridicaUI.pessoaJuridica);
                //Pega o código da empresa inserida para fazer a persistência do endereço
                if (pessoaJuridicaUI.endereco != null && (pessoaJuridicaUI.endereco.cd_loc_logradouro != 0)
                    && (pessoaJuridicaUI.endereco.cd_loc_cidade != 0) && (pessoaJuridicaUI.endereco.cd_loc_estado != 0))
                {
                    pessoaJuridicaUI.endereco.cd_pessoa = pessoaJuridica.cd_pessoa;
                    var endereco = BusinessLoc.PostEndereco(pessoaJuridicaUI.endereco);
                    //Pega o código do endereço principal e edita a empresa
                    PessoaJuridicaSGF pessoaChanges = pessoaJuridicaUI.pessoaJuridica;
                    pessoaChanges.cd_endereco_principal = endereco.cd_endereco;
                    pessoaJuridica = PessoaJuridicaSGF.ChangeValuesPessoaJuridica(pessoaJuridica, pessoaChanges);
                    DataAccess.saveChanges(false);
                }
                ////Outros endereços
                if (pessoaJuridicaUI.enderecos != null && pessoaJuridicaUI.enderecos.Count() > 0)
                {
                    foreach (EnderecoSGF endereco in pessoaJuridicaUI.enderecos)
                    {
                        endereco.cd_pessoa = pessoaJuridicaUI.pessoaJuridica.cd_pessoa;
                        BusinessLoc.PostEndereco(endereco);
                    }
                }

                if (pessoaJuridicaUI.telefones != null && pessoaJuridicaUI.telefones.Count() > 0)
                {
                    foreach (TelefoneSGF telefone in pessoaJuridicaUI.telefones)
                    {
                        //telefone.cd_pessoa = pessoaJuridicaUI.pessoaJuridica.cd_pessoa;
                        DataAccessTelefone.add(new TelefoneSGF
                        {
                            cd_classe_telefone = telefone.cd_classe_telefone,
                            cd_pessoa = pessoaJuridicaUI.pessoaJuridica.cd_pessoa,
                            cd_tipo_telefone = telefone.cd_tipo_telefone,
                            cd_operadora = telefone.cd_operadora,
                            dc_fone_mail = telefone.dc_fone_mail
                        }, false);
                    }
                }

                ////Contatos Principais
                addEditTipoContato(pessoaJuridicaUI.telefone, pessoaJuridicaUI.pessoaJuridica.cd_pessoa, ADD, TELEFONE, null, null);
                addEditTipoContato(pessoaJuridicaUI.email, pessoaJuridicaUI.pessoaJuridica.cd_pessoa, ADD, EMAIL, null, null);
                addEditTipoContato(pessoaJuridicaUI.site, pessoaJuridicaUI.pessoaJuridica.cd_pessoa, ADD, SITE, null, null);
                addEditTipoContato(pessoaJuridicaUI.celular, pessoaJuridicaUI.pessoaJuridica.cd_pessoa, ADD, CELULAR, pessoaJuridicaUI.cd_operadora, null);
                ////Parâmetros
                //insertParametro(entity);

                // Relacionamento
                if (relacionamentos != null)
                    setRelacionamentos(relacionamentos, pessoaJuridicaUI.pessoaJuridica.cd_pessoa, false);
                else
                    setRelacionamentos(new List<RelacionamentoSGF>(), pessoaJuridicaUI.pessoaJuridica.cd_pessoa, false);
                transaction.Complete();
            }
            if (!especializado)
            {
                pessoaJuridica.PessoaFilhoRelacionamento = GetRelacionamentoPorCodPessoaFilho(pessoaJuridica.cd_pessoa).ToList();
                pessoaJuridica.PessoaPaiRelacionamento = GetRelacionamentoPorCodPessoaPai(pessoaJuridica.cd_pessoa);
            }
            return pessoaJuridica;
        }

        public PessoaJuridicaSGF postUpdatePessoaJuridica(PessoaJuridicaUI pessoaJuridicaUI, List<RelacionamentoSGF> relacionamentos, bool especializado)
        {
            this.sincronizaContexto(DataAccess.DB());
            EnderecoSGF enderecoPrincipal = new EnderecoSGF();
            PessoaJuridicaSGF pessoaJuridica = new PessoaJuridicaSGF();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                DataAccessTelefone.sincronizaContexto(DataAccess.DB());
                var ExistPessoaJuridicaCnpjBase = DataAccess.ExistsPessoaByCNPJ(pessoaJuridicaUI.pessoaJuridica.dc_num_cgc);
                if (ExistPessoaJuridicaCnpjBase != null && ExistPessoaJuridicaCnpjBase.cd_pessoa > 0 && ExistPessoaJuridicaCnpjBase.cd_pessoa != pessoaJuridicaUI.pessoaJuridica.cd_pessoa)
                    throw new PessoaBusinessException(Messages.msgExistPersonCnpjBase + " " + ExistPessoaJuridicaCnpjBase.no_pessoa, null, FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_CNPJJAEXISTENTE, false);
                pessoaJuridica = DataAccess.FindIdPessoaJuridica(pessoaJuridicaUI.pessoaJuridica.cd_pessoa);
                pessoaJuridicaUI.pessoaJuridica.cd_endereco_principal = pessoaJuridica.cd_endereco_principal;
                pessoaJuridica = PessoaJuridicaSGF.ChangeValuesPessoaJuridica(pessoaJuridica, pessoaJuridicaUI.pessoaJuridica);
                DataAccess.saveChanges(false);
                //endereço 
                persistirEndereco(pessoaJuridicaUI.endereco, pessoaJuridicaUI.pessoaJuridica.cd_pessoa, pessoaJuridica.cd_endereco_principal);
                // telefone
                TelefoneSGF TelefoneExists = new TelefoneSGF();
                TelefoneExists = DataAccessTelefone.FindTypeTelefonePrincipal(pessoaJuridicaUI.pessoaJuridica.cd_pessoa, TELEFONE);
                addEditTipoContato(pessoaJuridicaUI.telefone, pessoaJuridicaUI.pessoaJuridica.cd_pessoa, EDIT, TELEFONE, null, TelefoneExists);
                //site
                TelefoneSGF SiteExists = new TelefoneSGF();
                SiteExists = DataAccessTelefone.FindTypeTelefonePrincipal(pessoaJuridicaUI.pessoaJuridica.cd_pessoa, SITE);
                addEditTipoContato(pessoaJuridicaUI.site, pessoaJuridicaUI.pessoaJuridica.cd_pessoa, EDIT, SITE, null, SiteExists);
                //email
                TelefoneSGF EmailExists = new TelefoneSGF();
                EmailExists = DataAccessTelefone.FindTypeTelefonePrincipal(pessoaJuridicaUI.pessoaJuridica.cd_pessoa, EMAIL);
                addEditTipoContato(pessoaJuridicaUI.email, pessoaJuridicaUI.pessoaJuridica.cd_pessoa, EDIT, EMAIL, null, EmailExists);
                //celular
                TelefoneSGF CelularExists = new TelefoneSGF();
                CelularExists = DataAccessTelefone.FindTypeTelefonePrincipal(pessoaJuridicaUI.pessoaJuridica.cd_pessoa, CELULAR);
                addEditTipoContato(pessoaJuridicaUI.celular, pessoaJuridicaUI.pessoaJuridica.cd_pessoa, EDIT, CELULAR, pessoaJuridicaUI.cd_operadora, CelularExists);
                //Outros Contatos
                setOutrosContatos(pessoaJuridicaUI.telefones, pessoaJuridicaUI.pessoaJuridica.cd_pessoa);
                //Outros endereços
                setOutrosEnderecos(pessoaJuridicaUI.enderecos, pessoaJuridicaUI.pessoaJuridica.cd_pessoa, pessoaJuridica.cd_endereco_principal);
                // Relacionamento
                if (relacionamentos != null)
                    setRelacionamentos(relacionamentos, pessoaJuridicaUI.pessoaJuridica.cd_pessoa, false);
                else
                    setRelacionamentos(new List<RelacionamentoSGF>(), pessoaJuridicaUI.pessoaJuridica.cd_pessoa, false);
                
                transaction.Complete();
            }
            if (!especializado)
            {
                pessoaJuridica.PessoaFilhoRelacionamento = GetRelacionamentoPorCodPessoaFilho(pessoaJuridica.cd_pessoa).ToList();
                pessoaJuridica.PessoaPaiRelacionamento = GetRelacionamentoPorCodPessoaPai(pessoaJuridica.cd_pessoa);
            }
            return pessoaJuridica;
        }

        public PessoaJurdicaSearchUI VerificarExisitsEmpresaByCnpjOrcdEmpresa(string cnpj, int? cdEmpresa)
        {
            PessoaJurdicaSearchUI pessoa = DataAccess.VerificarExisitsEmpresaByCnpjOrcdEmpresa(cnpj, cdEmpresa);
            if (pessoa.pessoaJuridica != null && pessoa.pessoaJuridica.EnderecoPrincipal != null && pessoa.pessoaJuridica.EnderecoPrincipal.cd_loc_cidade > 0)
                pessoa.pessoaJuridica.EnderecoPrincipal.bairros = BusinessLoc.getBairroPorCidade(pessoa.pessoaJuridica.EnderecoPrincipal.cd_loc_cidade, 0).ToList();
            return pessoa;
        }

        public PessoaSGF findPessoaById(int id)
        {
            return DataAccess.findById(id, false);
        }

        public bool deletePessoa(PessoaSGF pessoa)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                DataAccess.delete(pessoa, false);
                deleted = true;
                transaction.Complete();
            }
            return deleted;
        }

        public PessoaJuridicaSGF VerificarEmpresaByCnpj(string cnpj)
        {
            return DataAccess.VerificarEmpresaByCnpj(cnpj);
        }

        public PessoaJuridicaSGF ExistsPessoaJuridicaByCnpj(string Cgc)
        {
            return DataAccess.ExistsPessoaJuridicaByCnpj(Cgc);
        }

        public PessoaSGF ExistsPessoaByCNPJ(string Cgc)
        {
            return DataAccess.ExistsPessoaByCNPJ(Cgc);
        }

        #endregion

        #region pessoa física

        public PessoaFisicaSGF ManageChangesPessoaFisica(PessoaFisicaSGF pessoaFisicaBase, PessoaFisicaSGF pessoaFisica)
        {
            pessoaFisicaBase.nm_cpf = pessoaFisica.nm_cpf;
            pessoaFisicaBase.dt_nascimento = pessoaFisica.dt_nascimento;
            pessoaFisicaBase.nm_doc_identidade = pessoaFisica.nm_doc_identidade;
            pessoaFisicaBase.nm_sexo = pessoaFisica.nm_sexo;
            return pessoaFisicaBase;
        }

        public PessoaFisicaSGF postInsertPessoaFisica(PessoaFisicaUI pessoaFisicaUI, List<RelacionamentoSGF> relacionamentos, bool especializado)
        {
            this.sincronizaContexto(DataAccess.DB());
            PessoaFisicaSGF pessoaFisica = new PessoaFisicaSGF();
            //Caso o usuário esta se relacionando com ele mesmo.
            validarMinDatePessoaFisica(pessoaFisicaUI.pessoaFisica);
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (!string.IsNullOrEmpty(pessoaFisicaUI.pessoaFisica.nm_cpf))
                {
                    var ExistPessoaCpfBase = DataAccess.ExistsPessoByCpf(pessoaFisicaUI.pessoaFisica.nm_cpf);
                    if (ExistPessoaCpfBase != null && ExistPessoaCpfBase.cd_pessoa > 0)
                        throw new PessoaBusinessException(Messages.msgExistPersonCpfBase + " " + ExistPessoaCpfBase.no_pessoa, null, FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_CPFJAEXISTENTE, false);
                }
                else
                    pessoaFisicaUI.pessoaFisica.nm_cpf = null;
                pessoaFisicaUI.pessoaFisica.nm_natureza_pessoa = 1;
                if (pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf == 0) pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf = null;
                pessoaFisica = DataAccess.postPessoaFisica(pessoaFisicaUI.pessoaFisica);
                if (pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf > 0 && pessoaFisicaUI.pessoaFisica.cd_pessoa > 0 && pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf == pessoaFisicaUI.pessoaFisica.cd_pessoa)
                    throw new PessoaBusinessException(Messages.msgPessoaAutoRelacionamento, null,
                          FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_AUTO_RELACIONAMENTO, false);
                //Caso o usuário esteja com cpf e usando o cpf de outra pessoa ao mesmo tempo.
                if (pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf > 0 && !string.IsNullOrEmpty(pessoaFisicaUI.pessoaFisica.nm_cpf))
                    throw new PessoaBusinessException(Messages.msgPessoaAutoRelacionamento, null,
                          FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_AUTO_RELACIONAMENTO, false);
                //Pega o código da empresa inserida para fazer a persistência do endereço
                if (pessoaFisicaUI.endereco != null && (pessoaFisicaUI.endereco.cd_loc_logradouro != 0)
                    && (pessoaFisicaUI.endereco.cd_loc_cidade != 0) && (pessoaFisicaUI.endereco.cd_loc_estado != 0))
                {
                    pessoaFisicaUI.endereco.cd_pessoa = pessoaFisica.cd_pessoa;
                    var endereco = BusinessLoc.PostEndereco(pessoaFisicaUI.endereco);
                    //Pega o código do endereço principal e edita a empresa
                    //PessoaJuridica pessoaChanges = pessoaFisicaUI.pessoaFisica;
                    pessoaFisica.cd_endereco_principal = endereco.cd_endereco;
                    //pessoaFisicaUI = PessoaJuridica.ChangeValuesPessoaJuridica(pessoaFisicaUI, pessoaChanges);
                    DataAccess.saveChanges(false);
                }
                ////Outros endereços
                if (pessoaFisicaUI.enderecos != null && pessoaFisicaUI.enderecos.Count() > 0)
                {
                    foreach (EnderecoSGF endereco in pessoaFisicaUI.enderecos)
                    {
                        endereco.cd_pessoa = pessoaFisicaUI.pessoaFisica.cd_pessoa;
                        BusinessLoc.PostEndereco(endereco);
                    }
                }
                ////LBM ESTAVA AQUI
                if (pessoaFisicaUI.telefones != null && pessoaFisicaUI.telefones.Count() > 0)
                {
                    foreach (TelefoneSGF telefone in pessoaFisicaUI.telefones)
                    {
                        //telefone.cd_pessoa = pessoaJuridicaUI.pessoaJuridica.cd_pessoa;
                        DataAccessTelefone.add(new TelefoneSGF
                        {
                            cd_classe_telefone = telefone.cd_classe_telefone,
                            cd_pessoa = pessoaFisicaUI.pessoaFisica.cd_pessoa,
                            cd_tipo_telefone = telefone.cd_tipo_telefone,
                            cd_operadora = telefone.cd_operadora,
                            dc_fone_mail = telefone.dc_fone_mail
                        }, false);
                    }
                }

                ////Contatos Principais
                addEditTipoContato(pessoaFisicaUI.telefone, pessoaFisicaUI.pessoaFisica.cd_pessoa, ADD, TELEFONE, null, null);
                addEditTipoContato(pessoaFisicaUI.site, pessoaFisicaUI.pessoaFisica.cd_pessoa, ADD, SITE, null, null);
                addEditTipoContato(pessoaFisicaUI.celular, pessoaFisicaUI.pessoaFisica.cd_pessoa, ADD, CELULAR, pessoaFisicaUI.cd_operadora, null);
                ////Parâmetros
                //insertParametro(entity);

                // Relacionamento
                if (relacionamentos != null)
                    setRelacionamentos(relacionamentos, pessoaFisicaUI.pessoaFisica.cd_pessoa, false);
                else
                    setRelacionamentos(new List<RelacionamentoSGF>(), pessoaFisicaUI.pessoaFisica.cd_pessoa, false);

                //LBM PASSOU PARA CÁ 
                addEditTipoContato(pessoaFisicaUI.email, pessoaFisicaUI.pessoaFisica.cd_pessoa, ADD, EMAIL, null, null);

                transaction.Complete();
            }
            if (!especializado)
            {
                pessoaFisica.PessoaFilhoRelacionamento = GetRelacionamentoPorCodPessoaFilho(pessoaFisica.cd_pessoa).ToList();
                pessoaFisica.PessoaPaiRelacionamento = GetRelacionamentoPorCodPessoaPai(pessoaFisica.cd_pessoa);
                if (pessoaFisica.cd_pessoa_cpf != null && pessoaFisica.cd_pessoa_cpf > 0)
                {
                    PessoaFisicaSGF pessoaQueUsoCpf = DataAccess.getPessoaQueUsoCpf(pessoaFisica.cd_pessoa_cpf.Value);
                    if (pessoaQueUsoCpf != null)
                    {
                        pessoaFisica.PessoaSGFQueUsoOCpf = new PessoaFisicaSGF();
                        pessoaFisica.PessoaSGFQueUsoOCpf = pessoaQueUsoCpf;
                    }
                }
            }
            return pessoaFisica;

        }

        public PessoaFisicaSGF postUpdatePessoaFisica(PessoaFisicaUI pessoaFisicaUI, List<RelacionamentoSGF> relacionamentos, bool especializado, bool novo)
        {
            this.sincronizaContexto(DataAccess.DB());
            EnderecoSGF enderecoPrincipal = new EnderecoSGF();
            PessoaFisicaSGF pessoaFisica = new PessoaFisicaSGF();
            validarMinDatePessoaFisica(pessoaFisicaUI.pessoaFisica);
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (!string.IsNullOrEmpty(pessoaFisicaUI.pessoaFisica.nm_cpf))
                {
                    var ExistPessoaCpfBase = DataAccess.ExistsPessoByCpf(pessoaFisicaUI.pessoaFisica.nm_cpf);
                    if (ExistPessoaCpfBase != null && ExistPessoaCpfBase.cd_pessoa > 0 && ExistPessoaCpfBase.cd_pessoa != pessoaFisicaUI.pessoaFisica.cd_pessoa)
                        throw new PessoaBusinessException(Messages.msgExistPersonCpfBase + " " + ExistPessoaCpfBase.no_pessoa, null, FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_CPFJAEXISTENTE, false);
                }
                else
                    pessoaFisicaUI.pessoaFisica.nm_cpf = null;
                pessoaFisica = DataAccess.FindIdPessoaFisica(pessoaFisicaUI.pessoaFisica.cd_pessoa);
                //Caso o usuário esteja com cpf e usando o cpf de outra pessoa ao mesmo tempo.
                if (pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf > 0 && pessoaFisicaUI.pessoaFisica.cd_pessoa > 0 && pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf == pessoaFisicaUI.pessoaFisica.cd_pessoa)
                    throw new PessoaBusinessException(Messages.msgPessoaAutoRelacionamento, null,
                         FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_AUTO_RELACIONAMENTO, false);
                pessoaFisicaUI.pessoaFisica.cd_endereco_principal = pessoaFisica.cd_endereco_principal;
                pessoaFisica = PessoaFisicaSGF.changeValuesPessoaFisica(pessoaFisica, pessoaFisicaUI.pessoaFisica);
                DataAccess.saveChanges(false);

                //endereço 
                //if (pessoaFisicaUI.endereco.cd_loc_estado > 0 || pessoaFisicaUI.endereco.cd_loc_cidade > 0)
                persistirEndereco(pessoaFisicaUI.endereco, pessoaFisicaUI.pessoaFisica.cd_pessoa, pessoaFisica.cd_endereco_principal);

                // telefone
                TelefoneSGF TelefoneExists = new TelefoneSGF();
                TelefoneExists = DataAccessTelefone.FindTypeTelefonePrincipal(pessoaFisicaUI.pessoaFisica.cd_pessoa, TELEFONE);
                addEditTipoContato(pessoaFisicaUI.telefone, pessoaFisicaUI.pessoaFisica.cd_pessoa, EDIT, TELEFONE, null, TelefoneExists);
                //site
                TelefoneSGF SiteExists = new TelefoneSGF();
                SiteExists = DataAccessTelefone.FindTypeTelefonePrincipal(pessoaFisicaUI.pessoaFisica.cd_pessoa, SITE);
                addEditTipoContato(pessoaFisicaUI.site, pessoaFisicaUI.pessoaFisica.cd_pessoa, EDIT, SITE, null, SiteExists);
                //email
                TelefoneSGF EmailExists = new TelefoneSGF();
                EmailExists = DataAccessTelefone.FindTypeTelefonePrincipal(pessoaFisicaUI.pessoaFisica.cd_pessoa, EMAIL);
                addEditTipoContato(pessoaFisicaUI.email, pessoaFisicaUI.pessoaFisica.cd_pessoa, EDIT, EMAIL, null, EmailExists);
                //celular
                TelefoneSGF CelularExists = new TelefoneSGF();
                CelularExists = DataAccessTelefone.FindTypeTelefonePrincipal(pessoaFisicaUI.pessoaFisica.cd_pessoa, CELULAR);
                addEditTipoContato(pessoaFisicaUI.celular, pessoaFisicaUI.pessoaFisica.cd_pessoa, EDIT, CELULAR, pessoaFisicaUI.cd_operadora, CelularExists);
                //Outros Contatos
                if (pessoaFisicaUI.telefones != null)
                    setOutrosContatos(pessoaFisicaUI.telefones.ToList(), pessoaFisicaUI.pessoaFisica.cd_pessoa);
                //Outros endereços
                //if (pessoaFisicaUI.enderecos != null)
                    setOutrosEnderecos(pessoaFisicaUI.enderecos, pessoaFisicaUI.pessoaFisica.cd_pessoa, pessoaFisica.cd_endereco_principal);
                // Relacionamento
                if (relacionamentos != null)
                    setRelacionamentos(relacionamentos, pessoaFisicaUI.pessoaFisica.cd_pessoa, !novo);
                else
                    setRelacionamentos(new List<RelacionamentoSGF>(), pessoaFisicaUI.pessoaFisica.cd_pessoa, !novo);
                transaction.Complete();
            }
            if (!especializado)
            {
                pessoaFisica.PessoaFilhoRelacionamento = GetRelacionamentoPorCodPessoaFilho(pessoaFisica.cd_pessoa).ToList();
                pessoaFisica.PessoaPaiRelacionamento = GetRelacionamentoPorCodPessoaPai(pessoaFisica.cd_pessoa);
                if (pessoaFisica.cd_pessoa_cpf != null && pessoaFisica.cd_pessoa_cpf > 0)
                {
                    PessoaFisicaSGF pessoaQueUsoCpf = DataAccess.getPessoaQueUsoCpf(pessoaFisica.cd_pessoa_cpf.Value);
                    if (pessoaQueUsoCpf != null)
                    {
                        pessoaFisica.PessoaSGFQueUsoOCpf = new PessoaFisicaSGF();
                        pessoaFisica.PessoaSGFQueUsoOCpf = pessoaQueUsoCpf;
                    }
                }
            }
            return pessoaFisica;
        }

        public PessoaFisicaSearchUI VerificarExisitsPessoByCpfOrCdPessoa(string cpf, string email, string nome, int cd_pessoa_cpf, int? cdPessoa, int cd_escola)
        {
            PessoaFisicaSearchUI pessoa = DataAccess.VerificarExisitsPessoByCpfOrCdPessoa(cpf, email, nome, cd_pessoa_cpf, cdPessoa, cd_escola);
            if (pessoa.pessoaFisica != null && pessoa.pessoaFisica.EnderecoPrincipal != null && pessoa.pessoaFisica.EnderecoPrincipal.cd_loc_cidade > 0)
                pessoa.pessoaFisica.EnderecoPrincipal.bairros = BusinessLoc.getBairroPorCidade(pessoa.pessoaFisica.EnderecoPrincipal.cd_loc_cidade, 0).ToList();
            return pessoa;
        }

        public PessoaFisicaSearchUI VerificaExisitsAlunoRafMatricula(int? cdPessoa, int cd_escola)
        {
            return DataAccess.VerificaExisitsAlunoRafMatricula(cdPessoa, cd_escola);
        }

        public PessoaFisicaSGF VerificarPessoByCpf(string cpfCgc)
        {
            return DataAccess.VerificarPessoByCpf(cpfCgc);
        }

        public PessoaFisicaSGF ExistsPessoFisicaByCpf(string cpf)
        {
            return DataAccess.ExistsPessoFisicaByCpf(cpf);
        }

        public PessoaSGF ExistsPessoByCpf(string cpf)
        {
            return DataAccess.ExistsPessoByCpf(cpf);
        }

        public PessoaFisicaSGF VerificarPessoByCdPessoa(int cdPessoa)
        {
            return DataAccess.VerificarPessoByCdPessoa(cdPessoa);
        }

        public void PostPessoaEPessoaFisica(PessoaFisicaSGF pessoaFisica)
        {
            pessoaFisica.dt_cadastramento = DateTime.UtcNow;
            pessoaFisica.id_pessoa_empresa = false;
            pessoaFisica.nm_natureza_pessoa = 1;
            pessoaFisica.Enderecos = null;
            DataAccessPessoaFisica.add(pessoaFisica, false);
        }

        public PessoaSGF addNewPessoaFisica(PessoaFisicaSGF pessoaFisicaSGF, EnderecoSGF endereco)
        {
            pessoaFisicaSGF.id_pessoa_empresa = false;
            pessoaFisicaSGF.nm_natureza_pessoa = 1;
            PessoaFisicaSGF pessoaFisica = DataAccessPessoaFisica.add(pessoaFisicaSGF, false);

            EnderecoSGF enderecoProspect = null;
            if (endereco.cd_endereco == 0 && !string.IsNullOrEmpty(endereco.dc_num_cep))
            {
                endereco.cd_pessoa = pessoaFisica.cd_pessoa;
                 enderecoProspect = BusinessLoc.PostEndereco(endereco);
            }

            if (enderecoProspect != null)
            {
                pessoaFisica.cd_endereco_principal = enderecoProspect.cd_endereco;
            }
            DataAccess.saveChanges(false);
            return pessoaFisica;
        }

        public PessoaFisicaSGF editPessoaFisica(PessoaFisicaSGF pessoaFisicaSGF)
        {
            PessoaFisicaSGF editPessoaFisica = new PessoaFisicaSGF();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                editPessoaFisica = DataAccess.FindIdPessoaFisica(pessoaFisicaSGF.cd_pessoa);
                if (editPessoaFisica.EnderecoPrincipal == null && pessoaFisicaSGF.EnderecoPrincipal.cd_endereco == 0 && !string.IsNullOrEmpty(pessoaFisicaSGF.EnderecoPrincipal.dc_num_cep))
                {
                    pessoaFisicaSGF.EnderecoPrincipal.cd_pessoa = editPessoaFisica.cd_pessoa;
                   var endereco = BusinessLoc.PostEndereco(pessoaFisicaSGF.EnderecoPrincipal);
                   editPessoaFisica.cd_endereco_principal = endereco.cd_endereco;
                   PessoaFisicaSGF.changeValuesPessoaFisicaProspect(editPessoaFisica, pessoaFisicaSGF, endereco);
                }
                else if (editPessoaFisica.EnderecoPrincipal != null || pessoaFisicaSGF.dt_nascimento != null)
                {
                    if (pessoaFisicaSGF.EnderecoPrincipal.cd_endereco > 0 )
                    {
                        pessoaFisicaSGF.EnderecoPrincipal.cd_endereco = editPessoaFisica.EnderecoPrincipal.cd_endereco;
                        pessoaFisicaSGF.EnderecoPrincipal.cd_pessoa = editPessoaFisica.EnderecoPrincipal.cd_pessoa;
                        DataAccess.UpdateValuesEnderecoPrincipal(editPessoaFisica, pessoaFisicaSGF);
                        editPessoaFisica =
                            PessoaFisicaSGF.changeValuesPessoaFisicaProspect(editPessoaFisica, pessoaFisicaSGF, null);
                    }
                    else
                    {
                        editPessoaFisica = PessoaFisicaSGF.changeValuesPessoaFisicaProspect(editPessoaFisica, pessoaFisicaSGF, null);

                        //DataAccess.UpdateValuesEnderecoPrincipal(editPessoaFisica, pessoaFisicaSGF);
                        if (editPessoaFisica.EnderecoPrincipal != null)
                        DataAccess.DeleteEndereco(editPessoaFisica.EnderecoPrincipal);
                        editPessoaFisica.EnderecoPrincipal = null;
                    }
                }else if (pessoaFisicaSGF.nm_sexo != editPessoaFisica.nm_sexo || pessoaFisicaSGF.no_pessoa != editPessoaFisica.no_pessoa)
                {
                    editPessoaFisica.nm_sexo = pessoaFisicaSGF.nm_sexo;
                    editPessoaFisica.no_pessoa = pessoaFisicaSGF.no_pessoa;
                }
                
                DataAccess.saveChanges(false);
                transaction.Complete();
            }

            return editPessoaFisica;
        }

        #endregion
        
        /// <summary>
        /// métodos de percistência do telefone
        /// </summary>
        /// <param name="telefone"></param>
        /// <returns></returns>
        #region telefone
        
        public TelefoneSGF addTelefone(TelefoneSGF telefone)
        {
            return DataAccessTelefone.add(telefone, false);
        }

        public TelefoneSGF editTelefone(TelefoneSGF telefone)
        {
            return DataAccessTelefone.edit(telefone, false);
        }

        public TelefoneSGF FindTypeTelefone(int cdPessoa, int tipoTel)
        {
            return DataAccessTelefone.FindTypeTelefone(cdPessoa, tipoTel);
        }

        public TelefoneSGF FindTypeTelefonePrincipal(int cdPessoa, int tipoTel)
        {
            return DataAccessTelefone.FindTypeTelefonePrincipal(cdPessoa, tipoTel);
        }

        public TelefoneSGF saveChangesTelefone(TelefoneSGF telefone)
        {
            DataAccessTelefone.saveChanges(false);
            return telefone;
        }

        public IEnumerable<TelefoneSGF> getAllTelefonesByPessoa(int cdPessoa)
        {
            return DataAccessTelefone.GetAllTelefoneByPessoa(cdPessoa);
        }

        public IEnumerable<TelefoneSGF> getAllTelefonesContatosByPessoa(int cdPessoa) 
        {
            List<TelefoneSGF> listTelefone = new List<TelefoneSGF>();
            listTelefone = DataAccessTelefone.getAllTelefonesContatosByPessoa(cdPessoa).ToList();
            return listTelefone;
        }

        public bool deletedTelefone(TelefoneSGF telefone)
        {
            
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = DataAccessTelefone.delete(telefone, false);
                transaction.Complete();
            }
            return deleted;
        }

        public TelefoneSGF findTelefoneById(int cdTelefone)
        {
            return DataAccessTelefone.findById(cdTelefone, false);
        }
        
        /// <summary>
        /// Método genérico para gravar tipo de contato.
        /// </summary>
        /// <param name="entityContato">É o tipo de contato passado na entidade</param>
        /// <param name="pessoa">Código da pessoa a ser persistido</param>
        /// <param name="desContato">Descrição  do contato</param>
        /// <param name="tipoCrud">O tipo de persistência se  é Insete ou edit</param>
        /// <param name="tipoContato">O tipo de contato a ser persistido Telefone, email, site ou celular</param>
        public void addEditTipoContato(string entityContato, int cdPessoa, int tipoCrud, int tipoContato, int? codOperadora, TelefoneSGF contatoExists)
        {
            this.sincronizaContexto(DataAccess.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                TelefoneSGF telefonePeristido = new TelefoneSGF();
                if (tipoCrud == ADD)
                {
                    if (!String.IsNullOrEmpty(entityContato))
                    {
                        telefonePeristido = returnTelefone(cdPessoa, entityContato, true, tipoContato, codOperadora, COMERCIAL);
                        setTelefonePrincipal(cdPessoa, tipoContato, telefonePeristido);
                    }
                }
                else
                {
                    if (contatoExists == null)
                    {
                        if (!String.IsNullOrEmpty(entityContato))
                        {
                            telefonePeristido = returnTelefone(cdPessoa, entityContato, true, tipoContato, codOperadora, COMERCIAL);
                            setTelefonePrincipal(cdPessoa, tipoContato, telefonePeristido);
                        }
                    }
                    else
                    {
                        if (contatoExists.dc_fone_mail != entityContato || (codOperadora != null && contatoExists.cd_operadora != codOperadora.Value)
                            || (codOperadora == null && contatoExists.cd_operadora != null))
                        {
                            //quando não existe mais o contato
                            if (string.IsNullOrEmpty(entityContato))
                            {
                                if (tipoContato == TELEFONE)
                                    setTelefonePrincipal(cdPessoa, tipoContato, new TelefoneSGF());
                                deletedTelefone(contatoExists);
                            }
                            else
                            {
                                contatoExists.dc_fone_mail = entityContato;
                                contatoExists.cd_operadora = codOperadora;
                                telefonePeristido = editTelefone(contatoExists);
                            }
                        }
                    }
                }
                transaction.Complete();
            }
        }

        private void setTelefonePrincipal(int cdPessoa, int tipoContato, TelefoneSGF telefonePeristido)
        {
            if (tipoContato == TELEFONE)
            {
                var pessoaEdit = DataAccess.findById(cdPessoa, false);
                if (telefonePeristido.cd_telefone > 0)
                {
                    pessoaEdit.cd_telefone_principal = telefonePeristido.cd_telefone;
                    DataAccess.edit(pessoaEdit, false);
                }
                else if (pessoaEdit.cd_telefone_principal != null)
                {
                    pessoaEdit.cd_telefone_principal = null;
                    DataAccess.edit(pessoaEdit, false);
                }
                
                
            }
        }

        #endregion

        #region Tratamento Pessoa
        public IEnumerable<TratamentoPessoa> getAllTratamentoPessoa()
        {
            return DataAccessTratamentoPessoa.getAllTratamentoPessoa();
        }
        #endregion

        #region Estado Civil
        public IEnumerable<EstadoCivilSGF> getAllEstadoCivil()
        {
            return DataAccessEstadoCivil.getAllEstadoCivil();
        }
        #endregion

        #region Orgao Expedidor
        public IEnumerable<OrgaoExpedidor> getAllOrgaoExpedidor()
        {
            return DataAccessOrgaoExpedidor.getAllOrgaoEspedidor();
        }
        #endregion

        #region Papel

        public IEnumerable<PapelSGF> getPapelByTipo(int[] tipo)
        {
            return DataAccessPapel.getPapelByTipo(tipo);
        }

        public PapelSGF GetPapelByCdPapel(int cdPapel)
        {
            return DataAccessPapel.findById(cdPapel,false);
        }

        public IEnumerable<PapelSGF> GetAllPapel()
        {
            return DataAccessPapel.GetAllPapel();
        }
        
        public PapelSGF getPapelById(int cd_papel_filho)
        {
            return DataAccessPapel.getPapelById(cd_papel_filho);
        }

        #endregion

        #region Qualif Relacionamento
        public IEnumerable<QualifRelacionamento> getAllQualifRelacByPapel(int codPapel)
        {
            return DataAccessPapel.getAllQualifRelacByPapel(codPapel);
        }
        #endregion

        #region Relacionamento

        public RelacionamentoSGF addRelacionamento(RelacionamentoSGF relacionamento)
        {
            return DataAccessRelacionamento.add(relacionamento, false);
        }

        public RelacionamentoSGF editRelacionamento(RelacionamentoSGF relacionamento)
        {
            return DataAccessRelacionamento.edit(relacionamento, false);
        }

        public bool deleteRelacioanamento(RelacionamentoSGF relacionamento)
        {
            DataAccessRelacionamento.sincronizaContexto(DataAccess.DB());
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                RelacionamentoSGF relacionamentoOriginal = (RelacionamentoSGF)relacionamento.Clone();
                relacionamento = DataAccess.GetRelacionamentoPorCodPessoaFilho(relacionamento.cd_pessoa_filho).FirstOrDefault();
                RelacionamentoSGF relDelete = DataAccessRelacionamento.findById(relacionamentoOriginal.cd_relacionamento, false);
                try
                {
                    if (relacionamento != null)
                        deleted = DataAccessRelacionamento.delete(relDelete, false);
                }
                catch (Exception e)
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    throw new PessoaBusinessException(String.Empty, e, PessoaBusinessException.TipoErro.ERRO_CONVERSAO_DATA, true, relacionamentoOriginal);
#pragma warning restore CS0612 // Type or member is obsolete
                }
                transaction.Complete();
            }
            return deleted;
        }

        public void setRelacionamentos(List<RelacionamentoSGF> relacionamentos, int cdPessoaPai, bool edicao)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                List<RelacionamentoSGF> relacionamentosAntigos = GetRelacionamentoPorCodPessoaPai(cdPessoaPai);

                // Adiciona os relacionamentos contrários:
                List<RelacionamentoSGF> relacionamentosContrarios = this.GetRelacionamentoPorCodPessoaFilho(cdPessoaPai).ToList();

                if (relacionamentosAntigos != null && relacionamentosContrarios != null && relacionamentosContrarios.Count > 0)
                    foreach (RelacionamentoSGF relac in relacionamentosContrarios)
                    {
                        relac.ehRelacInverso = true;
                        relacionamentosAntigos.Add(relac);
                    }

                List<RelacionamentoSGF> relDelete = new List<RelacionamentoSGF>();
                // Remove os relacionamentos alterados ou excluídos:
                if (relacionamentosAntigos != null && relacionamentosAntigos.Count > 0)
                foreach (var rel in relacionamentosAntigos)
                {
                    RelacionamentoSGF relDel = relacionamentos.Where(x => x.cd_relacionamento == rel.cd_relacionamento).FirstOrDefault();
                    if (relDel == null && edicao)
                    {
                        deleteRelacioanamento(rel);
                        relDelete.Add(rel);//guarda o itens removidos na memoria para posteriormente remover da lista de relacionamentos antigos
                    }
                }

                //se foi removido do bd - também é removido da lista de relacionamentos antigos
                if(edicao)
                relDelete.ForEach(x => { relacionamentosAntigos.Remove(x); });
                

                //if (relacionamentosAntigos != null && relacionamentosAntigos.Count > 0)
                //    foreach (var rel in relacionamentosAntigos)
                //        // Caso o relacionamento antigo não exista mais ou ele foi trocado de papel:
                //        if (relacionamentos != null && relacionamentos.Where(item => item.cd_relacionamento == rel.cd_relacionamento).Count() <= 0)
                //            deleteRelacioanamento(rel);

                // Alterando os relacionamentos de uma pessoa que já existe:
                if (relacionamentos != null && relacionamentos.Count > 0)
                    foreach (var rel in relacionamentos)
                    {
                        RelacionamentoSGF relac = new RelacionamentoSGF();
                        relac = rel;
                        // Caso o relacionamento novo não existia 
                        if (relacionamentosAntigos == null || relacionamentosAntigos.Where(item => item.cd_relacionamento == rel.cd_relacionamento).Count() <= 0)
                        {
                            relac.cd_pessoa_pai = cdPessoaPai;
                            if (relac.cd_papel_pai == 0 || !relac.cd_papel_pai.HasValue)
                            {
                                PapelSGF papelFilho = this.getPapelById(relac.cd_papel_filho);
                                if (papelFilho != null)
                                    if (papelFilho.PapeisPais != null && papelFilho.PapeisPais.Count > 0)
                                        relac.cd_papel_pai = papelFilho.PapeisPais.FirstOrDefault<PapelSGF>().cd_papel;
                                    else if (papelFilho.PapeisFilhos != null && papelFilho.PapeisFilhos.Count > 0)
                                        relac.cd_papel_pai = papelFilho.PapeisFilhos.FirstOrDefault<PapelSGF>().cd_papel;
                            }
                            //1° e 2° caso abordado (se existir as 2 pessoas e o endereço (Incluido ou Alterado)
                            if (relac.cd_pessoa_filho > 0)
                            {
                                if (relac.PessoaFilho != null && relac.PessoaFilho.EnderecoPrincipal != null)
                                    persistirEndereco(relac.PessoaFilho.EnderecoPrincipal, relac.cd_pessoa_filho, 0);
                                relac.PessoaFilho = null;
                                relac.RelacionamentoPaiPapel = null;
                            }
                            else
                                relac = inserirPessoaFromRelacionamento(relac, cdPessoaPai);
                            if ((relac.cd_papel_pai == 0 || !relac.cd_papel_pai.HasValue) || relac.cd_papel_filho == 0)
#pragma warning disable CS0612 // Type or member is obsolete
                                throw new PessoaBusinessException(Messages.msgNaoInfPapelRelacionemto, null,
                         FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_PAPEL_RELACIONAMENTO, false, relac);
#pragma warning restore CS0612 // Type or member is obsolete
                            if (relac.cd_pessoa_pai == relac.cd_pessoa_filho)
#pragma warning disable CS0612 // Type or member is obsolete
                                throw new PessoaBusinessException(Messages.msgNotIncludAutoRelacPessoasRelac, null,
                                 FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_AUTO_RELACIONAMENTO, false, relac);
#pragma warning restore CS0612 // Type or member is obsolete
                            addRelacionamento(relac);
                        }
                    }
                transaction.Complete();
            }
        }

        public RelacionamentoSGF inserirPessoaFromRelacionamento(RelacionamentoSGF relac, int cdPessoaPai)
        {
            if (relac.PessoaFilho != null)
            {
                if (relac.PessoaFilho.nm_natureza_pessoa == 1)
                {
                    PessoaFisicaSGF pessoaFisica = (PessoaFisicaSGF)relac.PessoaFilho;
                    pessoaFisica = addPessoaFisicaRelacionamento(pessoaFisica, relac, cdPessoaPai);
                    relac.PessoaFilho = null;
                    relac.cd_pessoa_filho = pessoaFisica.cd_pessoa;
                }
                else
                {
                    PessoaJuridicaSGF pessoaJuridica = (PessoaJuridicaSGF)relac.PessoaFilho;
                    pessoaJuridica = addPessoaJuridicaRelacionamento(pessoaJuridica, relac, cdPessoaPai);
                    relac.PessoaFilho = null;
                    relac.cd_pessoa_filho = pessoaJuridica.cd_pessoa;
                }
            }
            return relac;
        }

        public PessoaFisicaSGF addPessoaFisicaRelacionamento(PessoaFisicaSGF pessoaFisica, RelacionamentoSGF relac, int? cdPessoaPai)
        {
            EnderecoSGF enderecoRelac = new EnderecoSGF();
            pessoaFisica.dt_cadastramento = DateTime.UtcNow;
            enderecoRelac = pessoaFisica.EnderecoPrincipal;
            string dc_fone_mail = "";
            if (pessoaFisica.Telefone != null && !string.IsNullOrEmpty(pessoaFisica.Telefone.dc_fone_mail))
            {
                dc_fone_mail = pessoaFisica.Telefone.dc_fone_mail;
                pessoaFisica.Telefone = null;
            }
            pessoaFisica.EnderecoPrincipal = null;
            pessoaFisica.cd_pessoa_cpf = null;
            var existPessoaCpfBase = DataAccess.ExistsPessoFisicaByCpf(pessoaFisica.nm_cpf);
            if (relac != null && existPessoaCpfBase != null && existPessoaCpfBase.cd_pessoa == cdPessoaPai)
#pragma warning disable CS0612 // Type or member is obsolete
                throw new PessoaBusinessException(Messages.msgNotIncludAutoRelacPessoasRelac, null,
                 FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_AUTO_RELACIONAMENTO, false, relac);
#pragma warning restore CS0612 // Type or member is obsolete
            if (existPessoaCpfBase != null)
                pessoaFisica = existPessoaCpfBase;
            else
            {
                validarMinDatePessoaFisica(pessoaFisica);

                pessoaFisica = DataAccess.postPessoaFisica(pessoaFisica);
            }
            if (!string.IsNullOrEmpty(dc_fone_mail))
                addEditTipoContato(dc_fone_mail, pessoaFisica.cd_pessoa, ADD, TELEFONE, null, null);
            if (enderecoRelac != null && !string.IsNullOrEmpty(enderecoRelac.num_cep))
            {
                enderecoRelac.cd_pessoa = pessoaFisica.cd_pessoa;
                enderecoRelac = BusinessLoc.PostEndereco(enderecoRelac);
                pessoaFisica.cd_endereco_principal = enderecoRelac.cd_endereco;
                DataAccess.saveChanges(false);
            }
            return pessoaFisica;
        }

        public PessoaJuridicaSGF addPessoaJuridicaRelacionamento(PessoaJuridicaSGF pessoaJuridica,RelacionamentoSGF relac, int? cdPessoaPai)
        {
            EnderecoSGF enderecoRelac = new EnderecoSGF();
            pessoaJuridica.dt_cadastramento = DateTime.UtcNow;
            enderecoRelac = pessoaJuridica.EnderecoPrincipal;
            pessoaJuridica.EnderecoPrincipal = null;
            string dc_fone_mail = "";
            if (pessoaJuridica.Telefone != null && !string.IsNullOrEmpty(pessoaJuridica.Telefone.dc_fone_mail))
            {
                dc_fone_mail = pessoaJuridica.Telefone.dc_fone_mail;
                pessoaJuridica.Telefone = null;
            }
            PessoaJuridicaSGF existPessoaJuridicaCnpjBase = DataAccess.ExistsPessoaJuridicaByCnpj(pessoaJuridica.dc_num_cgc);
            if (existPessoaJuridicaCnpjBase != null && existPessoaJuridicaCnpjBase.cd_pessoa == cdPessoaPai)
#pragma warning disable CS0612 // Type or member is obsolete
                throw new PessoaBusinessException(Messages.msgNotIncludAutoRelacPessoasRelac, null,
                 FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_AUTO_RELACIONAMENTO, false, relac);
#pragma warning restore CS0612 // Type or member is obsolete
            if (existPessoaJuridicaCnpjBase != null && existPessoaJuridicaCnpjBase.cd_pessoa > 0)
                pessoaJuridica = existPessoaJuridicaCnpjBase;
            else
                pessoaJuridica = DataAccess.postPessoaJuridica(pessoaJuridica);
            if (!string.IsNullOrEmpty(dc_fone_mail))
                addEditTipoContato(dc_fone_mail, pessoaJuridica.cd_pessoa, ADD, TELEFONE, null, null);
            if (enderecoRelac != null)
            {
                enderecoRelac.cd_pessoa = pessoaJuridica.cd_pessoa;
                enderecoRelac = BusinessLoc.PostEndereco(enderecoRelac);
                pessoaJuridica.cd_endereco_principal = enderecoRelac.cd_endereco;
                DataAccess.saveChanges(false);
            }
            return pessoaJuridica;
        }

        public bool addRelacionamentoResponsavelAluno(RelacionamentoSGF pessoaRelac)
        {
            return DataAccessRelacionamento.addRelacionamentoResponsavelAluno(pessoaRelac);
        }

        #endregion                    
                
        #region Metodos Adicionais Pessoa

        public EnderecoSGF persistirEndereco(EnderecoSGF enderecoUI, int cdPessoa, int? cd_endereco_principal)
        {
            EnderecoSGF enderecoPrincipal = new EnderecoSGF();
            enderecoUI.cd_pessoa = cdPessoa;

            if ((cd_endereco_principal.Equals(null) || cd_endereco_principal.Equals(0)) &&
                (enderecoUI.cd_endereco.Equals(null) || enderecoUI.cd_endereco.Equals(0)))
            {
                if ((enderecoUI.cd_loc_estado > 0) && (enderecoUI.cd_loc_cidade > 0) && (enderecoUI.cd_loc_logradouro != 0)
                    && (enderecoUI.cd_tipo_endereco > 0))
                {
                    enderecoPrincipal = BusinessLoc.PostEndereco(enderecoUI);
                    setEnderecoPricipal(cdPessoa, enderecoPrincipal);
                }
            }
            else
            {
                enderecoPrincipal = BusinessLoc.FindById((int)cd_endereco_principal);
                if ((enderecoUI.cd_loc_estado > 0) && (enderecoUI.cd_loc_cidade > 0) && (enderecoUI.cd_loc_logradouro != 0)
                    && (enderecoUI.cd_tipo_endereco > 0))
                {
                    enderecoPrincipal = changeValueEndereco(enderecoUI, enderecoPrincipal);
                    BusinessLoc.saveChangesEndereco(enderecoPrincipal);
                    setEnderecoPricipal(cdPessoa, enderecoPrincipal);
                }
                else
                {
                    PessoaSGF pessoaContext = DataAccess.findById(cdPessoa, false);
                    pessoaContext.cd_endereco_principal = null;
                    DataAccess.saveChanges(false);
                    BusinessLoc.deleteEndereco(enderecoPrincipal);
                }
            }
            return enderecoPrincipal;
        }

        private void setEnderecoPricipal(int cdPessoa, EnderecoSGF enderecoPrincipal)
        {
            PessoaSGF pessoaContext = DataAccess.findById(cdPessoa, false);
            pessoaContext.cd_endereco_principal = pessoaContext.cd_endereco_principal == enderecoPrincipal.cd_endereco ? pessoaContext.cd_endereco_principal : enderecoPrincipal.cd_endereco;
            DataAccess.saveChanges(false);
        }

        private  EnderecoSGF changeValueEndereco(EnderecoSGF enderecoUI, EnderecoSGF endereco)
        {
            endereco.cd_pessoa = enderecoUI.cd_pessoa;
            endereco.cd_loc_bairro = enderecoUI.cd_loc_bairro;
            endereco.cd_loc_cidade = enderecoUI.cd_loc_cidade;
            endereco.cd_loc_distrito = enderecoUI.cd_loc_distrito;
            endereco.cd_loc_estado = enderecoUI.cd_loc_estado;
            endereco.cd_loc_logradouro = enderecoUI.cd_loc_logradouro;
            endereco.cd_loc_pais = enderecoUI.cd_loc_pais;
            endereco.cd_tipo_endereco = enderecoUI.cd_tipo_endereco;
            endereco.cd_tipo_logradouro = enderecoUI.cd_tipo_logradouro;
            endereco.dc_compl_endereco = enderecoUI.dc_compl_endereco;
            endereco.num_cep = enderecoUI.num_cep;
            endereco.dc_num_endereco = enderecoUI.dc_num_endereco;
            endereco.dc_num_local_geografico = enderecoUI.dc_num_local_geografico;
            return endereco;
        }

        public void setOutrosContatos(List<TelefoneSGF> telefonesView, int cdPessoa)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                TelefoneSGF telefone = new TelefoneSGF();
                List<TelefoneSGF> telefoneContext = DataAccessTelefone.GetAllTelefoneByPessoa(cdPessoa).ToList();
                if (telefonesView != null)
                {
                    IEnumerable<TelefoneSGF> ContatosComCodigo = from cts in telefonesView
                                                                 where cts.cd_telefone != 0
                                                                 select cts;
                    IEnumerable<TelefoneSGF> telefoneDeleted = telefoneContext.Where(tc => !ContatosComCodigo.Any(tv => tc.cd_telefone == tv.cd_telefone));
                    if (telefoneDeleted.Count() > 0)
                    {
                        foreach (var item in telefoneDeleted)
                        {
                            var deletarContato = DataAccessTelefone.findById(item.cd_telefone, false);
                            if (deletarContato != null && item.id_telefone_principal == false)
                            {
                                DataAccessTelefone.delete(deletarContato, false);
                            }
                        }
                    }
                    foreach (var item in telefonesView)
                    {
                        if (item.cd_telefone.Equals(null) || item.cd_telefone == 0)
                            returnTelefone(cdPessoa, item.dc_fone_mail, false, item.cd_tipo_telefone, item.cd_operadora, item.cd_classe_telefone);
                        else
                        {
                            //telefone = DataAccessTelefone.FindTypeTelefone(cdPessoa, item.cd_tipo_telefone);
                            telefone = telefoneContext.Where(x => x.cd_telefone == item.cd_telefone).FirstOrDefault();
                            if (telefone != null && telefone.id_telefone_principal == false)
                            {
                                //telefone.cd_telefone = item.cd_telefone;
                                telefone.cd_operadora = item.cd_operadora;
                                telefone.cd_classe_telefone = item.cd_classe_telefone;
                                telefone.cd_tipo_telefone = item.cd_tipo_telefone;
                                telefone.dc_fone_mail = item.dc_fone_mail;
                                DataAccessTelefone.saveChanges(false);
                            }
                        }
                    }
                }
                else
                {
                    if (telefoneContext != null)
                    {
                        foreach (var item in telefoneContext)
                        {
                            var deletarContato = DataAccessTelefone.findById(item.cd_telefone, false);
                            if (deletarContato != null && deletarContato.id_telefone_principal == false)
                                DataAccessTelefone.delete(deletarContato, false);
                        }
                    }
                }
                transaction.Complete();
            }
        }

        public void setOutrosEnderecos(ICollection<EnderecoSGF> enderecosUI, int cdPessoa, int? cd_endereco_principal)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                EnderecoSGF endereco = new EnderecoSGF();
                //Pessoa pessoa = DataAccess.findById(cdPessoa, false);
                List<EnderecoSGF> enderecosView = new List<EnderecoSGF>();
                List<EnderecoSGF> enderecosContext = BusinessLoc.GetAllEnderecoByPessoa(cdPessoa, cd_endereco_principal > 0 ?  (int)cd_endereco_principal : 0).ToList();
                if (enderecosUI != null)
                {
                    enderecosView = enderecosUI.ToList();
                    //procedimento para deletar um registro que esta na base de dados
                    IEnumerable<EnderecoSGF> enderecosComCodigo = from ec in enderecosView
                                                               where ec.cd_endereco != 0
                                                               select ec;
                    IEnumerable<EnderecoSGF> enderecoDeleted = enderecosContext.Where(ec => !enderecosComCodigo.Any(ev => ec.cd_endereco == ev.cd_endereco));
                    if (enderecoDeleted.Count() > 0)
                    {
                        foreach (var item in enderecoDeleted)
                        {
                            var deletarEndereco = BusinessLoc.FindById(item.cd_endereco);
                            if (deletarEndereco != null && cd_endereco_principal != item.cd_endereco)
                                BusinessLoc.deleteEndereco(deletarEndereco);
                        }
                    }
                    //Procedimento para enditar ou inserir um registro conforme o código do endereço 
                    foreach (var item in enderecosView)
                    {
                        if (item.cd_endereco != cd_endereco_principal && item.cd_loc_cidade > 0 && item.cd_loc_bairro > 0)
                            item.cd_pessoa = cdPessoa;
                        {
                            if (item.cd_endereco > 0)
                            {
                                //endereco = BusinessLoc.FindById(item.cd_endereco);
                                endereco = enderecosContext.Where(x => x.cd_endereco == item.cd_endereco).FirstOrDefault();

                                if (endereco != null)
                                {
                                    endereco.cd_endereco = item.cd_endereco;
                                    changeValueEndereco(item, endereco);
                                }
                            }
                            if (item.cd_endereco.Equals(null) || item.cd_endereco == 0)
                            {
                                BusinessLoc.PostEndereco(item);
                            }

                            BusinessLoc.saveChangesEndereco(endereco);
                        }
                    }
                }
                else
                {
                    if (enderecosContext != null && enderecosContext.Count() > 0)
                    {
                        foreach (var item in enderecosContext)
                        {
                            var deletarEndereco = BusinessLoc.FindById(item.cd_endereco);

                            if (cd_endereco_principal != deletarEndereco.cd_endereco)
                            {
                                BusinessLoc.deleteEndereco(deletarEndereco);
                            }
                        }
                    }
                }
                transaction.Complete();
            }
        }

        private TelefoneSGF returnTelefone(int cdPessoa, string contato, bool isPrincipal, int tipoContato, int? codOperadora, int? tipoClasseTelefone)
        {
            if(!tipoClasseTelefone.HasValue)
                tipoClasseTelefone = ClasseTelefoneSGF.TIPO_COMERCIAL;
            var telefone = new TelefoneSGF
            {
                cd_pessoa = cdPessoa,
                cd_operadora = codOperadora,
                cd_classe_telefone = tipoClasseTelefone.Value,
                dc_fone_mail = contato,
                id_telefone_principal = isPrincipal,
                cd_tipo_telefone = tipoContato
            };
            return DataAccessTelefone.add(telefone,false);
        }

        public PessoaFisicaSGF getPessoaFisicaBycdPessoa(int cdPessoa, bool dispose)
        {
            return (PessoaFisicaSGF) DataAccess.findById(cdPessoa, dispose);
        }

        private void validarMinDatePessoaFisica(PessoaFisicaSGF pessoaF) {
            if ((pessoaF != null && pessoaF.dt_nascimento != null && DateTime.Compare((DateTime)pessoaF.dt_nascimento, new DateTime(1900,1,1)) < 0) ||
                (pessoaF != null && pessoaF.dt_cadastramento != null && DateTime.Compare((DateTime)pessoaF.dt_cadastramento, new DateTime(1900, 1, 1)) < 0) ||
                (pessoaF != null && pessoaF.dt_casamento != null && DateTime.Compare((DateTime)pessoaF.dt_casamento, new DateTime(1900, 1, 1)) < 0) ||
                (pessoaF != null && pessoaF.dt_venc_habilitacao != null && DateTime.Compare((DateTime)pessoaF.dt_venc_habilitacao, new DateTime(1900, 1, 1)) < 0))
                throw new PessoaBusinessException(Messages.msgErroMinDateDataNascPessoa, null,
                      FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_MINDATE_SMALLDATETIME, false);
        }

        public List<GrupoEstoque> findAllGrupoAtivo(int cd_grupo, bool isMasterGeral)
        {
            return DataAccess.findAllGrupoAtivo(cd_grupo, isMasterGeral).ToList();
        }

        #endregion

    }
}