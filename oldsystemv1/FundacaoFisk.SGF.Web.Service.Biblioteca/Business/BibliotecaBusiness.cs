using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Biblioteca.Business;
using FundacaoFisk.SGF.Web.Service.Biblioteca.Model;
using FundacaoFisk.SGF.Web.Services.Biblioteca.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Biblioteca.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Biblioteca.Business
{
    public class BibliotecaBusiness : IBibliotecaBusiness
    {
        /// <summary>
        /// Declaração de Interfaces
        /// </summary>
        public IEmprestimoDataAccess DaoEmprestimo { get; set; }
        public IFinanceiroBusiness BusinessFinanceiro { get; set; }
        
        /// <summary>
        /// Método Construtor do Dao
        /// </summary>
        public BibliotecaBusiness(IFinanceiroBusiness businessFinanceiro, IEmprestimoDataAccess daoEmprestimo)
        {
            if (daoEmprestimo == null || businessFinanceiro == null)
            {
                throw new ArgumentNullException("BibliotecaBusiness");
            }
            DaoEmprestimo = daoEmprestimo;
            BusinessFinanceiro = businessFinanceiro;
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa) {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext) this.DaoEmprestimo.DB()).IdUsuario = cdUsuario;
            ((SGFWebContext) this.DaoEmprestimo.DB()).cd_empresa = cd_empresa;

            BusinessFinanceiro.configuraUsuario(cdUsuario, cd_empresa);
        }

        public void sincronizarContextos(DbContext dbContext)
        {
            //this.DaoEmprestimo.sincronizaContexto(dbContext);
            //BusinessFinanceiro.sincronizarContextos(dbContext);
        }

        public IEnumerable<PessoaSearchUI> getPessoaBibliotecaSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa)
        {
            IEnumerable<PessoaSearchUI> retorno = new List<PessoaSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if(parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_pessoa_ativa");
                parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
                retorno = DaoEmprestimo.getPessoaBibliotecaSearch(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<PessoaSearchUI> getPessoaEmprestimoSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa)
        {
            if(parametros.sort == null)
                parametros.sort = "no_pessoa";
            parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
            parametros.sort = parametros.sort.Replace("id_ativo", "id_pessoa_ativa");
            parametros.sort = parametros.sort.Replace("natureza_pessoa", "nm_natureza_pessoa");
            return DaoEmprestimo.getPessoaEmprestimoSearch(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, cd_empresa);
        }

        public IEnumerable<Emprestimo> getEmprestimoSearch(SearchParameters parametros, int? cd_pessoa, int? cd_item, bool? pendentes, DateTime? dt_inicial, DateTime? dt_final, bool? emprestimos, bool? devolucao, int cd_empresa) {
            IEnumerable<Emprestimo> retorno = new List<Emprestimo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if(parametros.sort == null)
                    parametros.sort = "dt_emprestimo";
                parametros.sort = parametros.sort.Replace("dta_emprestimo", "dt_emprestimo");
                parametros.sort = parametros.sort.Replace("dta_prevista_devolucao", "dt_prevista_devolucao");
                parametros.sort = parametros.sort.Replace("dta_devolucao", "dt_devolucao");
                parametros.sort = parametros.sort.Replace("vlTaxaEmprestimo", "vl_taxa_emprestimo");
                parametros.sort = parametros.sort.Replace("vlMultaEmprestimo", "vl_multa_emprestimo");
            
                retorno = DaoEmprestimo.getEmprestimoSearch(parametros, cd_pessoa, cd_item, pendentes, dt_inicial, dt_final, emprestimos, devolucao, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public bool deleteEmprestimos(List<Emprestimo> emprestimos, int cd_escola) {
            bool retorno = true;
            BusinessFinanceiro.sincronizarContextos(DaoEmprestimo.DB());

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if(emprestimos != null && emprestimos.Count() > 0) 
                    for(int i = emprestimos.Count-1; i >= 0 ; i--) {
                        Emprestimo emprestimo = DaoEmprestimo.findById(emprestimos[i].cd_biblioteca, false);

                        SGFWebContext cdb = new SGFWebContext();
                        //Pega o item e incrementa o estoque:
                        if(!emprestimo.dt_devolucao.HasValue) {
                            ItemEscola itemEscola = BusinessFinanceiro.findItemEscolabyId(emprestimo.cd_item, cd_escola);
    
                            itemEscola.qt_estoque += 1;
                            DaoEmprestimo.saveChanges(false);
                        }

                        //Exclui o kardex
                        List<Kardex> kardex = BusinessFinanceiro.getKardexByOrigem((byte) cdb.LISTA_ORIGEM_LOGS["Emprestimo"], emprestimo.cd_biblioteca).ToList();

                        for(int j=kardex.Count -1; j>=0; j--)
                            retorno = retorno && BusinessFinanceiro.deleteKardex(kardex[j]);

                        retorno = retorno && DaoEmprestimo.delete(emprestimo, false);
                    }
                transaction.Complete();
            }
            return retorno;
        }

        public Emprestimo addEmprestimo(Emprestimo emprestimo, int cd_escola, int saldo) {
            Item item = emprestimo.Item;
            PessoaSGF pessoa = emprestimo.Pessoa;

            if(emprestimo.dt_prevista_devolucao.CompareTo(emprestimo.dt_emprestimo) < 0)
                throw new BibliotecaBusinessException(Utils.Messages.Messages.msgErrorDataPrevDevolucaoMenorEmprestimo, null, BibliotecaBusinessException.TipoErro.DATA_PREV_DEVOLUCAO_MENOR_EMPRESTIMO, false);

            if (saldo <= 0)
                throw new BibliotecaBusinessException(string.Format(Utils.Messages.Messages.msgErroSaldoBiblioteca, item.no_item), null, BibliotecaBusinessException.TipoErro.SEM_SALDO_ESTOQUE, false);

            SGFWebContext cdb = new SGFWebContext();
            emprestimo.Item = null;
            emprestimo.Pessoa = null;
            //BusinessFinanceiro.sincronizarContextos(DaoEmprestimo.DB());

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                emprestimo.cd_pessoa_escola = cd_escola;
                //Inclui o emprestimo:
                emprestimo = DaoEmprestimo.add(emprestimo, false);

                //Pega o item e decrementa o estoque:
                ItemEscola itemEscola = BusinessFinanceiro.findItemEscolabyId(emprestimo.cd_item, cd_escola);
                itemEscola.qt_estoque -= 1;

                DaoEmprestimo.saveChanges(false);

                //Inclui o kardex:
                Kardex kardex = new Kardex();
                kardex.cd_pessoa_empresa = cd_escola;
                kardex.cd_item = emprestimo.cd_item;
                kardex.cd_origem = (byte) cdb.LISTA_ORIGEM_LOGS["Emprestimo"];
                kardex.cd_registro_origem = emprestimo.cd_biblioteca;
                kardex.dt_kardex = emprestimo.dt_emprestimo.Date;
                kardex.id_tipo_movimento = (int) Kardex.TipoMovimento.SAIDA;
                kardex.qtd_kardex = 1;
                kardex.nm_documento = emprestimo.cd_biblioteca + "";
                kardex.tx_obs_kardex = "Empréstimo de " + pessoa.no_pessoa + ".";
                kardex.vl_kardex = itemEscola.vl_custo;
                BusinessFinanceiro.addKardex(kardex);

                transaction.Complete();
            }
            emprestimo.Item = item;
            emprestimo.Pessoa = pessoa;

            return emprestimo;
        }

        public Emprestimo postEditEmprestimo(Emprestimo emprestimo, int cd_escola) {
            Item item = emprestimo.Item;
            PessoaSGF pessoa = emprestimo.Pessoa;
            SGFWebContext cdb = new SGFWebContext();
            this.sincronizarContextos(DaoEmprestimo.DB());
            if(emprestimo.dt_devolucao.Value.CompareTo(emprestimo.dt_emprestimo) < 0)
                throw new BibliotecaBusinessException(Utils.Messages.Messages.msgErrorDataDevolucaoMenorEmprestimo, null, BibliotecaBusinessException.TipoErro.DATA_DEVOLUCAO_MENOR_EMPRESTIMO, false);

            if(emprestimo.dt_prevista_devolucao.CompareTo(emprestimo.dt_emprestimo) < 0)
                throw new BibliotecaBusinessException(Utils.Messages.Messages.msgErrorDataPrevDevolucaoMenorEmprestimo, null, BibliotecaBusinessException.TipoErro.DATA_PREV_DEVOLUCAO_MENOR_EMPRESTIMO, false);
            
            Emprestimo emprestimoContext = DaoEmprestimo.findById(emprestimo.cd_biblioteca, false);
            DateTime? dt_devolucao_context = emprestimoContext.dt_devolucao;
            
            emprestimoContext.copy(emprestimo);
            emprestimoContext.Item = null;
            emprestimoContext.Pessoa = null;

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                emprestimoContext.cd_pessoa_escola = cd_escola;
                emprestimo = DaoEmprestimo.edit(emprestimoContext, false);

                if(emprestimo.dt_devolucao.HasValue) {
                    //Primeira devolução:
                    if(!dt_devolucao_context.HasValue) {
                        //Pega o item e incrementa o estoque:
                        ItemEscola itemEscola = BusinessFinanceiro.findItemEscolabyId(emprestimo.cd_item, cd_escola);
                        itemEscola.qt_estoque += 1;

                        DaoEmprestimo.saveChanges(false);

                        Kardex kardex = new Kardex();
                        kardex.cd_pessoa_empresa = cd_escola;
                        kardex.cd_item = emprestimo.cd_item;
                        kardex.cd_origem = (byte) cdb.LISTA_ORIGEM_LOGS["Emprestimo"];
                        kardex.cd_registro_origem = emprestimo.cd_biblioteca;
                        kardex.dt_kardex = emprestimo.dt_devolucao.Value;
                        kardex.id_tipo_movimento = (int) Kardex.TipoMovimento.ENTRADA;
                        kardex.qtd_kardex = 1;
                        kardex.nm_documento = emprestimo.cd_biblioteca + "";
                        kardex.tx_obs_kardex = "Devolução de " + pessoa.no_pessoa + ".";
                        kardex.vl_kardex = itemEscola.vl_custo;
                        //Inclui o kardex:
                        BusinessFinanceiro.addKardex(kardex);
                    }
                    //Alteração da devolução:
                    else {
                        Kardex kardex = BusinessFinanceiro.getKardexByOrigem((int) (byte) cdb.LISTA_ORIGEM_LOGS["Emprestimo"], emprestimo.cd_biblioteca).FirstOrDefault();
                        if(kardex != null && kardex.dt_kardex.CompareTo(emprestimo.dt_devolucao.Value) != 0) {
                            kardex.dt_kardex = emprestimo.dt_devolucao.Value;
                            BusinessFinanceiro.atualizaKardex(kardex);
                        }
                    }
                }
                transaction.Complete();

                emprestimoContext.Item = item;
                emprestimoContext.Pessoa = pessoa;
            }
            return emprestimoContext;
        }

        public Emprestimo getEmprestimo(Parametro parametro, int cd_biblioteca, int cd_escola){

            Emprestimo emprestimo = DaoEmprestimo.getEmprestimo(cd_biblioteca, cd_escola);
            emprestimo.existe_devolucao = true;
            if (!emprestimo.dt_devolucao.HasValue)
            {
                emprestimo.dt_devolucao = DateTime.UtcNow.Date;
                emprestimo.existe_devolucao = false;

                int diferenca_dias = (emprestimo.dt_devolucao.Value - emprestimo.dt_prevista_devolucao).Days;
                emprestimo.vl_taxa_emprestimo = (decimal)parametro.pc_taxa_dia_biblioteca.Value;

                if (diferenca_dias > 0)
                {
                    emprestimo.vl_multa_emprestimo = Math.Round((decimal)(diferenca_dias * parametro.pc_taxa_dia_biblioteca.Value), 2, MidpointRounding.AwayFromZero);
                }
            }
            emprestimo.nm_dias_biblioteca = parametro.nm_dias_biblioteca.Value;
            emprestimo.id_bloquear_alt_dta_biblio = parametro.id_bloquear_alt_dta_biblio;
            return emprestimo;
        }

        public Emprestimo getEmprestimoById(int cd_emprestimo)
        {
            return DaoEmprestimo.findById(cd_emprestimo, false);
        }
        public EmprestimoSearch getEmprestimoById(int cd_biblioteca, int cd_empresa)
        {
            return DaoEmprestimo.getEmprestimoById(cd_biblioteca, cd_empresa);
        }
        
    }
}
