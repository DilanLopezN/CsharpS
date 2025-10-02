using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericBusiness;
using System.Transactions;
using Componentes.Utils;
using Componentes.Utils.Messages;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Business
{
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
    using FundacaoFisk.SGF.Web.Services.Usuario.Model;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
    using FundacaoFisk.SGF.Web.Services.Empresa.Model;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
    using System.Data.Entity;
    public class FuncionarioBusiness : IFuncionarioBusiness
    {
        private IFuncionarioDataAccess daoFunc { get; set; }
        private IPessoaBusiness pessoaBusiness { get; set; }
        private IFuncionarioComissaoDataAccess funcComissDataAccess { get; set; }
        public FuncionarioBusiness(IFuncionarioDataAccess DaoFunc, IPessoaBusiness PessoaBusiness, IFuncionarioComissaoDataAccess FuncComissDataAccess)
        {
            if (DaoFunc == null || PessoaBusiness == null || FuncComissDataAccess == null)
            {
                throw new ArgumentNullException("DAO");
            }

            this.daoFunc = DaoFunc;
            this.pessoaBusiness = PessoaBusiness;
            this.funcComissDataAccess = FuncComissDataAccess;
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa)
        {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.daoFunc.DB()).IdUsuario = cdUsuario;
            ((SGFWebContext)this.daoFunc.DB()).cd_empresa = cd_empresa;

        }

        public void sincronizaContextoFuncionarioComponentes(DbContext db)
        {
            //this.daoFunc.sincronizaContexto(db);
            //this.pessoaBusiness.sincronizaContexto(db);
            //this.funcComissDataAccess.sincronizaContexto(db);
        }


        public IEnumerable<FuncionarioSearchUI> getSearchFuncionario(SearchParameters parametros, string nome, string apelido, bool? status, string cpf, bool inicio, byte tipo, int cdEscola, int sexo, int cdAtividade)
        {
            IEnumerable<FuncionarioSearchUI> retorno = new List<FuncionarioSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("professor_ativo", "id_professor");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_funcionario_ativo");
                parametros.sort = parametros.sort.Replace("desc_atividade", "no_atividade");
                retorno = daoFunc.getSearchFuncionario(parametros, nome, apelido, status, cpf, inicio, tipo, cdEscola, sexo, cdAtividade);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<FuncionarioSearchUI> getSearchFuncionarioComAtividadeExtra(SearchParameters parametros, string nome, string apelido, bool? status, string cpf, bool inicio, byte tipo, int cdEscola, int sexo, int cdAtividade)
        {
            IEnumerable<FuncionarioSearchUI> retorno = new List<FuncionarioSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("professor_ativo", "id_professor");
                parametros.sort = parametros.sort.Replace("id_ativo", "id_funcionario_ativo");
                parametros.sort = parametros.sort.Replace("desc_atividade", "no_atividade");
                retorno = daoFunc.getSearchFuncionarioComAtividadeExtra(parametros, nome, apelido, status, cpf, inicio, tipo, cdEscola, sexo, cdAtividade);
                transaction.Complete();
            }
            return retorno;
        }

        public bool addEmpresaPessoa(PessoaEscola pessoaEmpresa)
        {
            bool retorno = daoFunc.addEmpresaPessoa(pessoaEmpresa);
            return retorno;
        }

        public FuncionarioSGF addFuncionario(FuncionarioSGF funcionario)
        {
            FuncionarioSGF func = daoFunc.add(funcionario, false);
            return func;
        }

        public void saveFuncionario(bool dispose)
        {
            daoFunc.saveChanges(dispose);
        }

        public PessoaFisicaSearchUI ExistFuncionarioOrPessoaFisicaByCpf(string cpf, int cdEmpresa)
        {
            PessoaFisicaSearchUI pFisica = new PessoaFisicaSearchUI();
            var funcionario = daoFunc.getFuncionarioByCpf(cpf, cdEmpresa);
            if (funcionario != null && funcionario.cd_funcionario > 0 && funcionario.FuncionarioPessoaFisica != null)
            {
                throw new FuncionarioBusinessException(string.Format(Messages.msgExistFuncionarioForCpf, funcionario.FuncionarioPessoaFisica.no_pessoa), null, FuncionarioBusinessException.TipoErro.ERRO_FUNCIONARIOJAEXISTE, false);
            }
            else
            {
                pFisica = pessoaBusiness.VerificarExisitsPessoByCpfOrCdPessoa(cpf, null, null, 0, 0, 0);
            }
            return pFisica;
        }

        public FuncionarioSGF editFuncionario(FuncionarioSGF funcionarioView, PessoaFisicaUI pessoaFisicaUI, List<RelacionamentoSGF> relacionamentos)
        {
            FuncionarioSGF funcionarioContext = new FuncionarioSGF();
            PessoaFisicaSGF pessoaFisicaContext = new PessoaFisicaSGF();
            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, daoFunc.DB(), TransactionScopeBuilder.TransactionTime.DEFAULT))
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                sincronizaContextoFuncionarioComponentes(daoFunc.DB());
                pessoaFisicaContext = pessoaBusiness.postUpdatePessoaFisica(pessoaFisicaUI, relacionamentos, true, false);
                funcionarioContext = daoFunc.fidFuncionarioById(funcionarioView.cd_funcionario, funcionarioView.cd_pessoa_empresa);

                if (pessoaFisicaContext == null || funcionarioContext == null)
                    throw new FuncionarioBusinessException(Componentes.Utils.Messages.Messages.msgRegNotEnc, null, FuncionarioBusinessException.TipoErro.ERRO_FUNCIONARIOJAEXISTE, false);
                //funcionarioContext.PessoaFisica = pessoaFisicaContext;
                funcionarioContext.cd_pessoa_funcionario = pessoaFisicaContext.cd_pessoa;
                funcionarioContext = FuncionarioSearchUI.changeValueFuncionarioViewToContext(funcionarioContext, funcionarioView);
                funcionarioContext.nome_temp_assinatura_certificado = funcionarioView.nome_temp_assinatura_certificado;
                if (funcionarioContext.FuncionarioComissao != null)
                    crudFuncionarioComissao(funcionarioView.FuncionarioComissao.ToList(), funcionarioContext.cd_funcionario);

                daoFunc.saveChanges(false);

                transaction.Complete();
            }
            return funcionarioContext;
        }

        public FuncionarioSearchUI getFuncionarioSearchUIById(int cd_funcionario, int cd_empresa)
        {
            return daoFunc.getFuncionarioSearchUIById(cd_funcionario, cd_empresa);
        }

        public FuncionarioCyberBdUI findFuncionarioByCdFuncionario(int cd_funcionario, int cd_empresa)
        {
            return daoFunc.findFuncionarioByCdFuncionario(cd_funcionario, cd_empresa);
        }

        public IEnumerable<FuncionarioSearchUI> getFuncionarios(int cd_pessoa_empresa, int? cd_funcionario, FuncionarioSGF.TipoConsultaFuncionarioEnum tipo)
        {
            IEnumerable<FuncionarioSearchUI> retorno = new List<FuncionarioSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoFunc.getFuncionarios(cd_pessoa_empresa, cd_funcionario, tipo).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        private void crudFuncionarioComissao(List<FuncionarioComissao> funcionarioComissaoView, int cd_funcionario)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                List<FuncionarioComissao> funcionarioComissaoContext = funcComissDataAccess.getFuncionarioComissao(cd_funcionario);

                IEnumerable<FuncionarioComissao> funcionarioComissaoComCodigo = from fc in funcionarioComissaoView
                                                                                where fc.cd_funcionario_comissao != 0
                                                                                select fc;
                IEnumerable<FuncionarioComissao> funcionarioComissaoDeleted = funcionarioComissaoContext.Where(tc => !funcionarioComissaoComCodigo.Any(tv => tc.cd_funcionario_comissao == tv.cd_funcionario_comissao));

                if (funcionarioComissaoDeleted != null)
                    foreach (var item in funcionarioComissaoDeleted)
                        if (item != null)
                            funcComissDataAccess.deleteContext(item, false);

                foreach (var item in funcionarioComissaoView)
                {
                    if (item.cd_funcionario_comissao == 0)
                    {
                        funcComissDataAccess.addContext(item, false);
                    }
                    else
                    {
                        var ctx = funcionarioComissaoContext.Where(x => x.cd_funcionario_comissao == item.cd_funcionario_comissao).FirstOrDefault();
                        ctx.cd_produto = item.cd_produto;
                        ctx.pc_comissao_matricula = item.pc_comissao_matricula;
                        ctx.pc_comissao_rematricula = item.pc_comissao_rematricula;
                        ctx.vl_comissao_matricula = item.vl_comissao_matricula;
                        ctx.vl_comissao_rematricula = item.vl_comissao_rematricula;
                    }
                }
                funcComissDataAccess.saveChanges(false);
                transaction.Complete();
            }
        }

        public int getFuncionarioByIdPessoa(int cd_pessoa)
        {
            int retorno = 0;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = daoFunc.getFuncionarioByIdPessoa(cd_pessoa);
                transaction.Complete();
            }
            return retorno;
        }
    }
}
