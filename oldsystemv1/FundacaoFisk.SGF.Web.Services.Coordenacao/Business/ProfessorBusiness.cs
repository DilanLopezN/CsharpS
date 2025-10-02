using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using Componentes.GenericBusiness;
using System.IO;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Utils;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Controllers;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using log4net.Repository.Hierarchy;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Business
{
    public class ProfessorBusiness : IProfessorBusiness
    {
        private IPessoaBusiness pessoaBusiness { get; set; }
        private IFuncionarioBusiness funcionarioBusiness { get; set; }
        private IProfessorDataAccess daoProf { get; set; }
        private IProdutoFuncionarioDataAccess daoProdutoFuncionario { get; set; }
        public IProdutoDataAccess daoProduto { get; set; }
        private IProfessorTurmaDataAccess daoProfTurma { get; set; }
        public ISecretariaBusiness secretBiz { get; set; }
        public IHorarioProfessorTurmaDataAccess daoHorarioProfessorTurma { get; set; }

        public IApiNewCyberFuncionarioBusiness businessApiNewCyberFuncionario { get; set; }

        public ProfessorBusiness(IPessoaBusiness PessoaBusiness,
            IProfessorDataAccess DaoProf, ISecretariaBusiness secretBusiness, IFuncionarioBusiness funcionarioBusiness, IHorarioProfessorTurmaDataAccess daoHorarioProfessorTurma, IProfessorTurmaDataAccess DaoProfTurma, IProdutoFuncionarioDataAccess DaoProdutoFuncionario,
            IProdutoDataAccess DaoProduto, IAlunoDataAccess DaoAluno, IFuncionarioDataAccess DaoFunc, IApiNewCyberFuncionarioBusiness BusinessApiNewCyberFuncionario )
        {
            if (PessoaBusiness == null || DaoProf == null || secretBusiness == null || funcionarioBusiness == null || DaoProdutoFuncionario == null || DaoProduto == null || DaoAluno == null || DaoFunc == null || BusinessApiNewCyberFuncionario == null)
            {
                throw new ArgumentNullException("DAO");
            }

            this.pessoaBusiness = PessoaBusiness;
            this.daoProf = DaoProf;
            this.daoProfTurma = DaoProfTurma;
            this.secretBiz = secretBusiness;
            this.funcionarioBusiness = funcionarioBusiness;
            this.daoHorarioProfessorTurma = daoHorarioProfessorTurma;
            this.daoProdutoFuncionario = DaoProdutoFuncionario;
            this.daoProduto = DaoProduto;
            this.businessApiNewCyberFuncionario = BusinessApiNewCyberFuncionario;
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa)
        {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.daoProf.DB()).IdUsuario = cdUsuario;
            ((SGFWebContext)this.daoProf.DB()).cd_empresa = cd_empresa;
            secretBiz.configuraUsuario(cdUsuario, cd_empresa);
            pessoaBusiness.configuraUsuario(cdUsuario, cd_empresa);
            funcionarioBusiness.configuraUsuario(cdUsuario, cd_empresa);

        }

        public void sincronizarContextos(DbContext db)
        {
            //this.daoProf.sincronizaContexto(db);
            //this.secretBiz.sincronizarContextos(db);
            //this.daoHorarioProfessorTurma.sincronizaContexto(db);
        }

        #region Funcionario/Professor

        public FuncionarioSearchUI addFuncionario(FuncionarioUI funcionarioUI, List<RelacionamentoSGF> relacionamentos, string fullPath)
        {
            PessoaFisicaSGF pessoaFisica = new PessoaFisicaSGF();
            FuncionarioSGF funcionario = new FuncionarioSGF();
            FuncionarioSGF funcionarioSGF = new FuncionarioSGF();
            Professor professor = new Professor();
            PessoaEscola pessoaEsc = new PessoaEscola();
            SGFWebContext db = pessoaBusiness.DBPessoa();
            pessoaBusiness.sincronizaContexto(db);
            funcionarioBusiness.sincronizaContextoFuncionarioComponentes(db);
            sincronizarContextos(daoProf.DB());
            if (funcionarioUI.funcionario.id_colaborador_cyber && funcionarioUI.funcionario.id_professor)
                throw new FuncionarioBusinessException(string.Format(Utils.Messages.Messages.msgErroFuncProfECoordCyber), null, FuncionarioBusinessException.TipoErro.ERRO_FUNCIONARIO, false);
            string imagemCertificadoTemp = "";
            string pathCodFuncionario = ""; 
            try
            {
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    if (funcionarioUI.pessoaFisicaUI != null && funcionarioUI.pessoaFisicaUI.pessoaFisica != null && funcionarioUI.pessoaFisicaUI.pessoaFisica.cd_pessoa > 0 &&
                        (!string.IsNullOrEmpty(funcionarioUI.pessoaFisicaUI.pessoaFisica.nm_cpf) || funcionarioUI.pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf > 0))
                    {
                        pessoaFisica = pessoaBusiness.postUpdatePessoaFisica(funcionarioUI.pessoaFisicaUI, relacionamentos, true, true);
                        // pessoaFisica = (PessoaFisica)pessoaBusiness.FindIdPessoa(funcionarioUI.pessoaFisicaUI.pessoaFisica.cd_pessoa);
                    }
                    else
                    {
                        pessoaFisica = pessoaBusiness.postInsertPessoaFisica(funcionarioUI.pessoaFisicaUI, relacionamentos, true);
                    }
                    validarMinDateFuncionario(funcionarioUI.funcionario);
                    if (funcionarioUI.funcionario.id_professor)
                    {
                        professor.copy(funcionarioUI.professor);
                        professor.copy(funcionarioUI.funcionario);
                        professor.cd_pessoa_funcionario = pessoaFisica.cd_pessoa;
                        daoProf.add(professor, false);
                        if (funcionarioUI.habilitacaoProfessor != null)
                            crudHabilitacaoProfessor(funcionarioUI.habilitacaoProfessor.ToList(), professor.cd_funcionario);
                        if (funcionarioUI.horarioSearchUI != null)
                        {
                            crudHorarioProfessor(funcionarioUI.horarioSearchUI.ToList(), professor.cd_funcionario, professor.cd_pessoa_empresa);
                        }
                        funcionario.copy(professor);
                        SetProdutosFuncionario(funcionario, funcionarioUI);
                    }
                    else
                    {
                        funcionario.copy(funcionarioUI.funcionario);
                        funcionario.cd_pessoa_funcionario = pessoaFisica.cd_pessoa;
                        funcionario.FuncionarioPessoaFisica = null;
                        //funcionario = FuncionarioSGF.formFuncionario(funcionarioUI.funcionario);
                        var func = funcionarioBusiness.addFuncionario(funcionario);
                        SetProdutosFuncionario(func, funcionarioUI);
                    }

                    List<RelacionamentoSGF> listaRelacionamento = relacionamentos;
                    if (relacionamentos != null)
                    {
                        pessoaBusiness.setRelacionamentos(listaRelacionamento, pessoaFisica.cd_pessoa, false);
                        for (int i = 0; i < listaRelacionamento.Count; i++)
                        {
                            pessoaEsc = new PessoaEscola
                            {
                                cd_escola = funcionario.cd_pessoa_empresa,
                                cd_pessoa = listaRelacionamento[i].cd_pessoa_filho
                            };
                            funcionarioBusiness.addEmpresaPessoa(pessoaEsc);
                        }
                    }
                    funcionarioBusiness.addEmpresaPessoa(new PessoaEscola
                    {
                        cd_escola = funcionario.cd_pessoa_empresa,
                        cd_pessoa = funcionario.cd_pessoa_funcionario
                    });
                    if (!string.IsNullOrEmpty(funcionario.nome_temp_assinatura_certificado))
                        gravarArquivoCertificadoFuncionario(ref imagemCertificadoTemp, fullPath, ref pathCodFuncionario, funcionario);
                    if (businessApiNewCyberFuncionario.aplicaApiCyber())
                    {
                        //Busca o funcionario Salvo
                        FuncionarioCyberBdUI funcionarioCyberBd = funcionarioBusiness.findFuncionarioByCdFuncionario(funcionario.cd_funcionario, funcionario.cd_pessoa_empresa);

                        if (funcionarioCyberBd != null && funcionarioCyberBd.id_unidade != null && funcionarioCyberBd.id_unidade > 0 &&
                            funcionarioCyberBd.funcionario_ativo == true)
                        {
                            //Chama a apiCyber de acordo com o tipo de funcionario
                            verificaTipoFuncionarioPostApiCyber(funcionarioCyberBd);
                        }
                    }
                    


                    transaction.Complete();
                }
                return funcionarioBusiness.getFuncionarioSearchUIById(funcionario.cd_funcionario, funcionario.cd_pessoa_empresa);
            }
            catch (Exception exe)
            {
                if (!string.IsNullOrEmpty(imagemCertificadoTemp) && System.IO.File.Exists(imagemCertificadoTemp))
                    System.IO.File.Delete(imagemCertificadoTemp);
                if (!string.IsNullOrEmpty(pathCodFuncionario) && System.IO.Directory.Exists(pathCodFuncionario))
                    System.IO.Directory.Delete(pathCodFuncionario);

                throw exe;
            }

        }

        public void verificaTipoFuncionarioPostApiCyber(FuncionarioCyberBdUI funcionarioCyberBd)
        {
            string parametros = "";

            //Professor -> (func.id_professor && db.FuncionarioSGF.OfType<Professor>().Any(x => x.cd_funcionario == func.cd_funcionario && x.id_coordenador == false))
            if (funcionarioCyberBd.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.PROFESSOR && !existeFuncionario(("P" + funcionarioCyberBd.codigo), ApiCyberComandosNames.VISUALIZA_PROFESSOR))
            {
                parametros = validaParametros(funcionarioCyberBd, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.CADASTRA_PROFESSOR, "");

                executaCyberCadastraFuncionario(parametros, ApiCyberComandosNames.CADASTRA_PROFESSOR);
            }
            //Coordenador -> (func.id_professor && db.FuncionarioSGF.OfType<Professor>().Any(x => x.cd_funcionario == func.cd_funcionario && x.id_coordenador == true))
            else if (funcionarioCyberBd.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COORDENADOR && !existeFuncionario(("O" + funcionarioCyberBd.codigo), ApiCyberComandosNames.VISUALIZA_COORDENADOR))
            {
                parametros = validaParametros(funcionarioCyberBd, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.CADASTRA_COORDENADOR, "");

                executaCyberCadastraFuncionario(parametros, ApiCyberComandosNames.CADASTRA_COORDENADOR);
            }
            //Colaborador -> (!func.id_professor && func.id_colaborador_cyber == true)
            else if (funcionarioCyberBd.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COLABORADOR && !existeFuncionario(("C" + funcionarioCyberBd.codigo), ApiCyberComandosNames.VISUALIZA_COLABORADOR))
            {
                parametros = validaParametros(funcionarioCyberBd, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.CADASTRA_COLABORADOR, "");

                executaCyberCadastraFuncionario(parametros, ApiCyberComandosNames.CADASTRA_COLABORADOR);
            }
          
        }

        private void executaCyberCadastraFuncionario(string parametros, string comando)
        {
            string result = businessApiNewCyberFuncionario.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);
        }

        private string validaParametros(FuncionarioCyberBdUI entity, string url, string comando, string parametros)
        {


            //valida codigo funcionario
            if (entity == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }

            if (entity.codigo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCdFuncionarioMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_COD_FUNCIONARIO_MENOR_IGUAL_ZERO, false);
            }


            //Valida id_unidade
            if (entity.id_unidade <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNmIntegracaoNuloOuMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NM_INTEGRACAO_NULO_OU_MENOR_IGUAL_ZERO, false);
            }

            //Valida nome e email

            if (String.IsNullOrEmpty(entity.nome))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNomeFuncionarioNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NOME_FUNCIONARIO_NULO_VAZIO, false);
            }

            if (String.IsNullOrEmpty(entity.email))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberEmailFuncionarioNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_EMAIL_PESSOA_FISICA_NULA_OU_VAZIA, false);
            }


            string listaParams = "";
            listaParams = string.Format("nome={0},id_unidade={1},codigo={2},email={3}", entity.nome, entity.id_unidade, entity.codigo, entity.email);
            return listaParams;
        }

        public void SetProdutosFuncionario(FuncionarioSGF funcionario, FuncionarioUI funcionarioUI)
        {
            string[] cd_produtos_funcionario = funcionarioUI.cd_produtos_funcionario.Split('|');

            List<int> cdsProdutosFuncionario = new List<int>();
            for (int i = 0; i < cd_produtos_funcionario.Count(); i++)
            {
                cdsProdutosFuncionario.Add(Int32.Parse(cd_produtos_funcionario[i]));
            }

            if (cdsProdutosFuncionario.Count() > 0 && !cdsProdutosFuncionario.Contains(100) && !cdsProdutosFuncionario.Contains(0))
            {
                foreach (int produtoId in cdsProdutosFuncionario)
                {
                    ProdutoFuncionario produtoFuncionario = new ProdutoFuncionario();
                    produtoFuncionario.cd_produto_funcionario = 0;
                    produtoFuncionario.cd_produto = produtoId;
                    produtoFuncionario.cd_funcionario = funcionario.cd_funcionario;
                    daoProdutoFuncionario.add(produtoFuncionario, false);
                }
            }
            else if (cdsProdutosFuncionario.Count() > 0 && cdsProdutosFuncionario.Contains(0))
            {
                List<Produto> produtos = daoProduto.findAll(false).ToList();
                List<int> cdsProduto = produtos.Select(x => x.cd_produto).ToList();
                foreach (int produtoId in cdsProduto)
                {
                    ProdutoFuncionario produtoFuncionario = new ProdutoFuncionario();
                    produtoFuncionario.cd_produto_funcionario = 0;
                    produtoFuncionario.cd_produto = produtoId;
                    produtoFuncionario.cd_funcionario = funcionario.cd_funcionario;
                    daoProdutoFuncionario.add(produtoFuncionario, false);
                }
            }

        }


        public FuncionarioSearchUI editFuncionario(FuncionarioUI funcionarioUI, List<RelacionamentoSGF> relacionamentos, bool permissaoSalario, string fullPath)
        {
            PessoaFisicaSGF pessoaFisica = new PessoaFisicaSGF();
            FuncionarioSGF funcionarioSGFContext = new FuncionarioSGF();
            Professor professorContext = new Professor();
            FuncionarioSGF funcionario = new FuncionarioSGF();
            PessoaEscola pessoaEmp = new PessoaEscola();
            PessoaFisicaSGF pessoaFisicaSgf = new PessoaFisicaSGF();
            string imagemCertificadoTemp = "";
            string pathCodFuncionario = "";

            FuncionarioCyberBdUI funcionarioCyberBdOld = null;

            if (funcionarioUI.funcionario.id_colaborador_cyber && funcionarioUI.funcionario.id_professor)
                throw new FuncionarioBusinessException(string.Format(Utils.Messages.Messages.msgErroFuncProfECoordCyber), null, FuncionarioBusinessException.TipoErro.ERRO_FUNCIONARIO, false);

            try
            {
                //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, daoProf.DB(), TransactionScopeBuilder.TransactionTime.DEFAULT))
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    sincronizarContextos(daoProf.DB());
                    //Caso o usuário não tenha permissão para alterar o Salário e o valor do Salário editado for diferente do cadastrado,
                    //voltar o valor do Salário para o cadastrado no banco
                    if (!permissaoSalario && funcionarioUI.funcionario.vl_salario > 0)
                        throw new FuncionarioBusinessException(string.Format(Utils.Messages.Messages.msgNotPermissaoSalario), null, FuncionarioBusinessException.TipoErro.ERRO_PERMISSAO_SALARIO, false);
                    //funcionarioUI.funcionario.vl_salario = funcionarioContext.vl_salario;

                    if (businessApiNewCyberFuncionario.aplicaApiCyber())
                    {
                        //Busca o funcionario Salvo
                        funcionarioCyberBdOld = funcionarioBusiness.findFuncionarioByCdFuncionario(funcionarioUI.funcionario.cd_funcionario, funcionarioUI.funcionario.cd_pessoa_empresa);
                    }

                    validarMinDateFuncionario(funcionarioUI.funcionario);
                    funcionario.copy(funcionarioUI.funcionario);
                    funcionario.FuncionarioPessoaFisica = pessoaFisica;
                    persistirEditProdutosFuncionario(funcionarioUI);
                    funcionario = funcionarioBusiness.editFuncionario(funcionario, funcionarioUI.pessoaFisicaUI, relacionamentos);



                    if (funcionario.id_professor)
                    {
                        professorContext = daoProf.getFuncionarioEditById(funcionario.cd_funcionario, funcionario.cd_pessoa_empresa, ProfessorDataAccess.TipoConsultaProfessorEnum.LOAD_PROFE);
                        if (professorContext != null)
                        {
                            professorContext = FuncionarioUI.changesValueProfessorViewToContext(professorContext, funcionarioUI.professor);
                            daoProf.saveChanges(false);
                            if (funcionarioUI.habilitacaoProfessor != null)
                                crudHabilitacaoProfessor(funcionarioUI.habilitacaoProfessor.ToList(), professorContext.cd_funcionario);
                            if (funcionarioUI.habilitacaoNula == false)
                            {
                                if (funcionarioUI.horarioSearchUI != null)
                                    crudHorarioProfessor(funcionarioUI.horarioSearchUI.ToList(), professorContext.cd_funcionario, professorContext.cd_pessoa_empresa);
                                else
                                    crudHorarioProfessor(new List<Horario>(), professorContext.cd_funcionario, professorContext.cd_pessoa_empresa);
                            }
                        }
                        else
                        {
                            funcionarioUI.professor.cd_funcionario = funcionario.cd_funcionario;
                            daoProf.addProfExistFunc(funcionarioUI.professor);
                            if (funcionarioUI.habilitacaoProfessor != null)
                                crudHabilitacaoProfessor(funcionarioUI.habilitacaoProfessor.ToList(), funcionario.cd_funcionario);
                            if (funcionarioUI.habilitacaoNula == false && funcionario.id_professor)
                            {
                                if (funcionarioUI.horarioSearchUI != null)
                                    crudHorarioProfessor(funcionarioUI.horarioSearchUI.ToList(), funcionario.cd_funcionario, funcionario.cd_pessoa_empresa);
                                else
                                    crudHorarioProfessor(new List<Horario>(), funcionario.cd_funcionario, funcionario.cd_pessoa_empresa);
                            }
                        }
                    }
                    else
                    {
                        Professor existProf = daoProf.getFuncionarioEditById(funcionario.cd_funcionario, funcionario.cd_pessoa_empresa, ProfessorDataAccess.TipoConsultaProfessorEnum.LOAD_PROFE);
                        if (existProf != null && existProf.cd_funcionario > 0 && !funcionario.id_professor)
                        {
                            List<Horario> horarios = new List<Horario>();
                            List<HabilitacaoProfessor> hProf = new List<HabilitacaoProfessor>();
                            crudHorarioProfessor(horarios, existProf.cd_funcionario, existProf.cd_pessoa_empresa);
                            crudHabilitacaoProfessor(hProf, existProf.cd_funcionario);
                            daoProf.deleteProfessor(existProf.cd_funcionario, existProf.cd_pessoa_empresa);
                        }
                    }

                    List<RelacionamentoSGF> listaRelacionamento = relacionamentos;
                    if (relacionamentos != null)
                    {
                        //foi comentando este método por já existir a persistencia do mesmo no componente de pessoa fisica.
                        //pessoaBusiness.setRelacionamentos(listaRelacionamento, pessoaFisica.cd_pessoa, false);
                        for (int i = 0; i < listaRelacionamento.Count; i++)
                        {
                            pessoaEmp = new PessoaEscola
                            {
                                cd_escola = funcionarioUI.funcionario.cd_pessoa_empresa,
                                cd_pessoa = listaRelacionamento[i].cd_pessoa_filho
                            };
                            funcionarioBusiness.addEmpresaPessoa(pessoaEmp);
                        }
                    }
                    funcionarioBusiness.addEmpresaPessoa(new PessoaEscola
                    {
                        cd_escola = funcionario.cd_pessoa_empresa,
                        cd_pessoa = funcionario.cd_pessoa_funcionario
                    });

                    var path = fullPath;
                    string imagemCertificado = (path += "//Arquivos") + "//" + funcionario.cd_pessoa_empresa + "//Funcionario//" + funcionario.cd_funcionario + "//Assinatura//" + funcionario.nome_assinatura_certificado;
                    if (!string.IsNullOrEmpty(funcionario.nome_assinatura_certificado) && !System.IO.File.Exists(imagemCertificado))
                        gravarArquivoCertificadoFuncionario(ref imagemCertificadoTemp, fullPath, ref pathCodFuncionario, funcionario);
                    //nome_assinatura_certificado_anterior

                    if (businessApiNewCyberFuncionario.aplicaApiCyber())
                    {
                        //pega o funcionario atualizado
                        FuncionarioCyberBdUI funcionarioCyberBdCurrent = funcionarioBusiness.findFuncionarioByCdFuncionario(funcionario.cd_funcionario, funcionario.cd_pessoa_empresa);


                        if (funcionarioCyberBdOld != null && funcionarioCyberBdCurrent != null && funcionarioCyberBdCurrent.id_unidade != null && funcionarioCyberBdCurrent.id_unidade > 0)
                        {
                            //Atualiza funcionario
                            verificaAlterouCamposExecutaCyberAtualizaFuncionario(funcionarioCyberBdCurrent, funcionarioCyberBdOld);

                            //se era apenas funcionario e virou professor
                            if (funcionarioCyberBdOld.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.FUNCIONARIO &&
                                funcionarioCyberBdCurrent.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.PROFESSOR &&
                                !existeFuncionario((verificaTipoFuncionarioSigla(funcionarioCyberBdCurrent.tipo_funcionario) + funcionarioCyberBdCurrent.codigo), ApiCyberComandosNames.VISUALIZA_PROFESSOR))
                            {
                                // Cadastrar professor
                                var parametros = validaParametrosEdicaoCadastraFuncionario(funcionarioCyberBdCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.CADASTRA_PROFESSOR, "");
                                executaCyberCadastraFuncionario(parametros, ApiCyberComandosNames.CADASTRA_PROFESSOR);

                            }

                            //se era apenas funcionario e virou colaborador
                            if (funcionarioCyberBdOld.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.FUNCIONARIO &&
                                funcionarioCyberBdCurrent.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COLABORADOR &&
                                !existeFuncionario((verificaTipoFuncionarioSigla(funcionarioCyberBdCurrent.tipo_funcionario) + funcionarioCyberBdCurrent.codigo), ApiCyberComandosNames.VISUALIZA_COLABORADOR))
                            {
                                // Cadastrar colaborador
                                var parametros = validaParametrosEdicaoCadastraFuncionario(funcionarioCyberBdCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.CADASTRA_COLABORADOR, "");
                                executaCyberCadastraFuncionario(parametros, ApiCyberComandosNames.CADASTRA_COLABORADOR);
                            }

                            //se era professor e virou coordenador
                            if (funcionarioCyberBdOld.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.PROFESSOR &&
                                funcionarioCyberBdCurrent.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COORDENADOR &&
                                !existeFuncionario((verificaTipoFuncionarioSigla(funcionarioCyberBdCurrent.tipo_funcionario) + funcionarioCyberBdCurrent.codigo), ApiCyberComandosNames.VISUALIZA_COORDENADOR))
                            {

                                // Cadastrar coordenador
                                var parametros = validaParametrosEdicaoCadastraFuncionario(funcionarioCyberBdCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.CADASTRA_COORDENADOR, "");
                                executaCyberCadastraFuncionario(parametros, ApiCyberComandosNames.CADASTRA_COORDENADOR);
                            }

                            //se inativou funcionario
                            executaCyberInativaFuncionario(funcionarioCyberBdCurrent, funcionarioCyberBdOld);

                            //se ativou funcionario
                            if (funcionarioCyberBdCurrent.funcionario_ativo != funcionarioCyberBdOld.funcionario_ativo && funcionarioCyberBdOld.funcionario_ativo == false)
                            {
                                //chama a api cyber com o comando (ativa_funcionario)
                                executaCyberAtivaFuncionario(funcionarioCyberBdCurrent.codigo, funcionarioCyberBdCurrent.tipo_funcionario);
                            }
                        }
                    }



                    transaction.Complete();
                }
            }
            catch (Exception exe)
            {
                if (!string.IsNullOrEmpty(imagemCertificadoTemp) && System.IO.File.Exists(imagemCertificadoTemp))
                    System.IO.File.Delete(imagemCertificadoTemp);
                if (!string.IsNullOrEmpty(pathCodFuncionario) && System.IO.Directory.Exists(pathCodFuncionario))
                    System.IO.Directory.Delete(pathCodFuncionario);


                if (exe.InnerException != null &&
                    exe.InnerException.InnerException != null &&
                    exe.InnerException.InnerException.InnerException != null &&
                    (exe.InnerException.InnerException.InnerException.Message.Contains("gatilho") ||
                    exe.InnerException.InnerException.InnerException.Message.Contains("trigger")))
                {
                    throw new FuncionarioBusinessException(exe.InnerException.InnerException.InnerException.Message.Replace("A transação foi encerrada no gatilho. O lote foi anulado.", "."),
                        null, FuncionarioBusinessException.TipoErro.ERRO_TRIGGER_FUNCIONARIO_PROFESSOR, false);
                }

                var message = Utils.Utils.innerMessage(exe);
                if (message != "")
                {
                    throw new FuncionarioBusinessException(message, exe, 0, false);
                }


                throw exe;
                

            }
            FuncionarioSearchUI funcionarioSearchGrid = funcionarioBusiness.getFuncionarioSearchUIById(funcionario.cd_funcionario, funcionario.cd_pessoa_empresa);

            return funcionarioSearchGrid;
        }


        public FuncionarioCyberBdUI findFuncionarioByCdFuncionario(int cd_funcionario, int cd_empresa)
        {
            return funcionarioBusiness.findFuncionarioByCdFuncionario(cd_funcionario, cd_empresa);
        }

        public void verificaAlterouCamposExecutaCyberAtualizaFuncionario(FuncionarioCyberBdUI funcionarioCyberBdCurrent, FuncionarioCyberBdUI funcionarioCyberBdOld)
        {

            //Professor -> (func.id_professor && db.FuncionarioSGF.OfType<Professor>().Any(x => x.cd_funcionario == func.cd_funcionario && x.id_coordenador == false))
            if (funcionarioCyberBdCurrent.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.PROFESSOR)
            {
                executaCyberAtualizaFuncionario(funcionarioCyberBdCurrent, funcionarioCyberBdOld, ApiCyberComandosNames.ATUALIZA_PROFESSOR);

                if (funcionarioCyberBdCurrent != null && !existeFuncionario((verificaTipoFuncionarioSigla(funcionarioCyberBdCurrent.tipo_funcionario) + funcionarioCyberBdCurrent.codigo), ApiCyberComandosNames.VISUALIZA_PROFESSOR))
                {
                    var parametros = validaParametrosEdicaoCadastraFuncionario(funcionarioCyberBdCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.CADASTRA_PROFESSOR, "");
                    executaCyberCadastraFuncionario(parametros, ApiCyberComandosNames.CADASTRA_PROFESSOR);
                }
                
            }
            //Coordenador -> (func.id_professor && db.FuncionarioSGF.OfType<Professor>().Any(x => x.cd_funcionario == func.cd_funcionario && x.id_coordenador == true))
            else if (funcionarioCyberBdCurrent.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COORDENADOR)
            {
                executaCyberAtualizaFuncionario(funcionarioCyberBdCurrent, funcionarioCyberBdOld, ApiCyberComandosNames.ATUALIZA_COORDENADOR);

                if (funcionarioCyberBdCurrent != null && !existeFuncionario((verificaTipoFuncionarioSigla(funcionarioCyberBdCurrent.tipo_funcionario) + funcionarioCyberBdCurrent.codigo), ApiCyberComandosNames.VISUALIZA_COORDENADOR))
                {
                    var parametros = validaParametrosEdicaoCadastraFuncionario(funcionarioCyberBdCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.CADASTRA_COORDENADOR, "");
                    executaCyberCadastraFuncionario(parametros, ApiCyberComandosNames.CADASTRA_COORDENADOR);
                }

            }
            //Colaborador -> (!func.id_professor && func.id_colaborador_cyber == true)
            else if (funcionarioCyberBdCurrent.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COLABORADOR)
            {
                executaCyberAtualizaFuncionario(funcionarioCyberBdCurrent, funcionarioCyberBdOld, ApiCyberComandosNames.ATUALIZA_COLABORADOR);

                if (funcionarioCyberBdCurrent != null && !existeFuncionario((verificaTipoFuncionarioSigla(funcionarioCyberBdCurrent.tipo_funcionario) + funcionarioCyberBdCurrent.codigo), ApiCyberComandosNames.VISUALIZA_COLABORADOR))
                {
                    var parametros = validaParametrosEdicaoCadastraFuncionario(funcionarioCyberBdCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.CADASTRA_COLABORADOR, "");
                    executaCyberCadastraFuncionario(parametros, ApiCyberComandosNames.CADASTRA_COLABORADOR);
                }
                    

            }
        }

        private void executaCyberAtualizaFuncionario(FuncionarioCyberBdUI funcionarioCyberBdCurrent, FuncionarioCyberBdUI funcionarioCyberBdOld, string comando)
        {
            string parametrosBd = "";
            string parametrosView = "";

            //Valida os parametros do banco
            parametrosBd = validaParametrosEdicao(funcionarioCyberBdOld, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando , "", true);

            //Valida os parametros da View
            parametrosView = validaParametrosEdicao(funcionarioCyberBdCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "", false);

            //Verifica se modificou chama o executa cyber
            if ((funcionarioCyberBdOld.nome != funcionarioCyberBdCurrent.nome ||
                 funcionarioCyberBdOld.email != funcionarioCyberBdCurrent.email) && existeFuncionario((verificaTipoFuncionarioSigla(funcionarioCyberBdCurrent.tipo_funcionario) + funcionarioCyberBdCurrent.codigo), funcionarioCyberBdCurrent.tipo_funcionario))
            {
                string result = businessApiNewCyberFuncionario.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                    comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametrosView);
            }
        }

        private string verificaTipoFuncionarioSigla(byte tipo_funcionario)
        {
            string retorno = "";
            if (tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.PROFESSOR)
            {
                retorno = "P";
            }
            else if (tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COORDENADOR)
            {
                retorno = "O";
            }
            else if (tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COLABORADOR)
            {
                retorno = "C";
            }

            return retorno;
        }

        public bool existeFuncionario(string codigo, byte tipo_funcionario)
        {
            if (tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.PROFESSOR)
            {
                return existeFuncionario(codigo, ApiCyberComandosNames.VISUALIZA_PROFESSOR);
            }
            else if (tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COORDENADOR)
            {
                return existeFuncionario(codigo, ApiCyberComandosNames.VISUALIZA_COORDENADOR);
            }
            else if (tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COLABORADOR)
            {
                return existeFuncionario(codigo, ApiCyberComandosNames.VISUALIZA_COLABORADOR);
            }

            return false;

        }

        private bool existeFuncionario(string codigo, string comando)
        {
            return businessApiNewCyberFuncionario.verificaRegistroFuncionario(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
              comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo);
        }


        public void executaCyberInativaFuncionario(FuncionarioCyberBdUI funcionarioCyberBdCurrent, FuncionarioCyberBdUI funcionarioCyberBdOld)
        {
            
            
            if (inativaProfessor(funcionarioCyberBdCurrent, funcionarioCyberBdOld))
            {
                executaCiberAtivacaoInativacaoFuncionario(funcionarioCyberBdCurrent.codigo, ApiCyberComandosNames.INATIVA_PROFESSOR);
            }
            else if (inativaCoordenador(funcionarioCyberBdCurrent, funcionarioCyberBdOld))
            {
                executaCiberAtivacaoInativacaoFuncionario(funcionarioCyberBdCurrent.codigo, ApiCyberComandosNames.INATIVA_COORDENADOR);
            }
            else if (inativaColaborador(funcionarioCyberBdCurrent, funcionarioCyberBdOld))
            {
                executaCiberAtivacaoInativacaoFuncionario(funcionarioCyberBdCurrent.codigo, ApiCyberComandosNames.INATIVA_COLABORADOR);
            }
            
        }

        private bool inativaProfessor(FuncionarioCyberBdUI funcionarioCyberBdCurrent, FuncionarioCyberBdUI funcionarioCyberBdOld)
        {
            return (//Sempre que marcar como inativo um professor que estava ativo 
                    (funcionarioCyberBdCurrent.funcionario_ativo != funcionarioCyberBdOld.funcionario_ativo && 
                    funcionarioCyberBdOld.funcionario_ativo == true && existeFuncionario(("P" + funcionarioCyberBdCurrent.codigo), funcionarioCyberBdCurrent.tipo_funcionario) &&
                    funcionarioCyberBdCurrent.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.PROFESSOR && 
                    funcionarioCyberBdOld.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.PROFESSOR) ||
                   //desmarcar como professor um funcionario que era marcado como professor.
                   (funcionarioCyberBdCurrent.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.PROFESSOR &&
                    funcionarioCyberBdOld.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.FUNCIONARIO &&
                    existeFuncionario("P" + funcionarioCyberBdCurrent.codigo, funcionarioCyberBdCurrent.tipo_funcionario)));
        }

        private bool inativaCoordenador(FuncionarioCyberBdUI funcionarioCyberBdCurrent, FuncionarioCyberBdUI funcionarioCyberBdOld)
        {
            return (//Sempre que marcar como inativo um coordenador que estava ativo 
                (funcionarioCyberBdCurrent.funcionario_ativo != funcionarioCyberBdOld.funcionario_ativo &&
                 funcionarioCyberBdOld.funcionario_ativo == true && existeFuncionario(("O" + funcionarioCyberBdCurrent.codigo), funcionarioCyberBdCurrent.tipo_funcionario) &&
                 funcionarioCyberBdCurrent.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COORDENADOR &&
                 funcionarioCyberBdOld.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COORDENADOR ) ||
                //desmarcar como coordenador um professor que era marcado como coordenador.
                (funcionarioCyberBdCurrent.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COORDENADOR &&
                 funcionarioCyberBdOld.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.PROFESSOR &&
                 existeFuncionario(("O" + funcionarioCyberBdCurrent.codigo), funcionarioCyberBdCurrent.tipo_funcionario)) ||
                //desmarcar este como professor
                (funcionarioCyberBdCurrent.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COORDENADOR &&
                 funcionarioCyberBdOld.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.FUNCIONARIO &&
                 existeFuncionario(("O" + funcionarioCyberBdCurrent.codigo), funcionarioCyberBdCurrent.tipo_funcionario)));
        }

        private bool inativaColaborador(FuncionarioCyberBdUI funcionarioCyberBdCurrent, FuncionarioCyberBdUI funcionarioCyberBdOld)
        {
            return (//Sempre que marcar como inativo um colaborador que estava ativo 
                (funcionarioCyberBdCurrent.funcionario_ativo != funcionarioCyberBdOld.funcionario_ativo &&
                 funcionarioCyberBdOld.funcionario_ativo == true && existeFuncionario(("C" + funcionarioCyberBdCurrent.codigo), funcionarioCyberBdCurrent.tipo_funcionario) &&
                 funcionarioCyberBdCurrent.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COLABORADOR &&
                 funcionarioCyberBdOld.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COLABORADOR) ||
                //desmarcar como colaborador cyber um funcionário que o era.
                (funcionarioCyberBdCurrent.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COLABORADOR &&
                 funcionarioCyberBdOld.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.FUNCIONARIO &&
                 existeFuncionario(("C" + funcionarioCyberBdCurrent.codigo), funcionarioCyberBdCurrent.tipo_funcionario)));
        }


        public void executaCyberAtivaFuncionario(int codigo,byte tipo_funcionario)
        {
            if (existeFuncionario(("P" + codigo), tipo_funcionario) && tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.PROFESSOR)
            {
                executaCiberAtivacaoInativacaoFuncionario(codigo, ApiCyberComandosNames.ATIVA_PROFESSOR);
            }
            else if (existeFuncionario(("O" + codigo), tipo_funcionario) && tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COORDENADOR)
            {
                executaCiberAtivacaoInativacaoFuncionario(codigo, ApiCyberComandosNames.ATIVA_COORDENADOR);
            }
            else if (existeFuncionario(("C" + codigo), tipo_funcionario) && tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COLABORADOR)
            {
                executaCiberAtivacaoInativacaoFuncionario(codigo, ApiCyberComandosNames.ATIVA_COLABORADOR);
            }
        }

        private void executaCiberAtivacaoInativacaoFuncionario(int codigo, string comando)
        {
            string result = businessApiNewCyberFuncionario.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], String.Format("codigo={0}", codigo));
        }

        private string validaParametrosEdicao(FuncionarioCyberBdUI entity, string url, string comando, string parametros, bool isOldValue)
        {


            //valida codigo funcionario
            if (entity == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }

            if (entity.codigo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCdFuncionarioMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_COD_FUNCIONARIO_MENOR_IGUAL_ZERO, false);
            }


            //Valida nome e email

            if (String.IsNullOrEmpty(entity.nome))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNomeFuncionarioNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NOME_FUNCIONARIO_NULO_VAZIO, false);
            }

            //Apenas valida o email quando for o valor atual
            if (!isOldValue)
            {
                if (String.IsNullOrEmpty(entity.email))
                {
                    throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberEmailFuncionarioNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_EMAIL_PESSOA_FISICA_NULA_OU_VAZIA, false);
                }
            }


            string listaParams = "";
            listaParams = string.Format("nome={0},codigo={1},email={2}", entity.nome, entity.codigo, entity.email);
            return listaParams;
        }

        private string validaParametrosEdicaoCadastraFuncionario(FuncionarioCyberBdUI funcionarioCyberBdCurrent, string url, string comando, string parametros)
        {


            //valida codigo funcionario
            if (funcionarioCyberBdCurrent == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }

            if (funcionarioCyberBdCurrent.codigo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCdFuncionarioMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_COD_FUNCIONARIO_MENOR_IGUAL_ZERO, false);
            }


            //Valida id_unidade
            if (funcionarioCyberBdCurrent.id_unidade <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNmIntegracaoNuloOuMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NM_INTEGRACAO_NULO_OU_MENOR_IGUAL_ZERO, false);
            }

            //Valida nome e email

            if (String.IsNullOrEmpty(funcionarioCyberBdCurrent.nome))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNomeFuncionarioNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NOME_FUNCIONARIO_NULO_VAZIO, false);
            }

            if (String.IsNullOrEmpty(funcionarioCyberBdCurrent.email))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberEmailFuncionarioNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_EMAIL_PESSOA_FISICA_NULA_OU_VAZIA, false);
            }


            string listaParams = "";
            listaParams = string.Format("nome={0},id_unidade={1},codigo={2},email={3}", funcionarioCyberBdCurrent.nome, funcionarioCyberBdCurrent.id_unidade, funcionarioCyberBdCurrent.codigo, funcionarioCyberBdCurrent.email);
            return listaParams;
        }


        private void persistirEditProdutosFuncionario(FuncionarioUI funcionario)
        {

            if (funcionario != null)
            {
                IEnumerable<ProdutoFuncionario> produtosFuncionarioCTX =
                    daoProdutoFuncionario.searchProdutosFuncionario(funcionario.funcionario.cd_funcionario, funcionario.funcionario.cd_pessoa_empresa);
                foreach (var produto in produtosFuncionarioCTX)
                {
                    var deletarItem = daoProdutoFuncionario.findById(produto.cd_produto_funcionario, false);
                    var del = deletarItem != null ? daoProdutoFuncionario.delete(deletarItem, false) : false;
                }
            }


            SetProdutosFuncionario(funcionario.funcionario, funcionario);

        }

        public void gravarArquivoCertificadoFuncionario(ref string imagemCertificadoTemp, string fullPath, ref string pathCodFuncionario,  FuncionarioSGF funcionario)
        {
            HashSet<char> invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());
            HashSet<char> invalidPathChars = new HashSet<char>(Path.GetInvalidPathChars());
            imagemCertificadoTemp = fullPath + "//" + funcionario.nome_temp_assinatura_certificado;
            fullPath += "//Arquivos";
            string imagemCertificado = fullPath + "//" + funcionario.cd_pessoa_empresa + "//Funcionario//" + funcionario.cd_funcionario + "//Assinatura//" + funcionario.nome_assinatura_certificado;
            pathCodFuncionario = fullPath + "//" + funcionario.cd_pessoa_empresa + "//Funcionario//" + funcionario.cd_funcionario;
            //Caso não exista a pasta com o código da escola, criar.
            if (!System.IO.Directory.Exists(fullPath + "//" + funcionario.cd_pessoa_empresa))
                System.IO.Directory.CreateDirectory(fullPath + "//" + funcionario.cd_pessoa_empresa);
            if (!System.IO.Directory.Exists(fullPath + "//" + funcionario.cd_pessoa_empresa + "//Funcionario"))
                System.IO.Directory.CreateDirectory(fullPath + "//" + funcionario.cd_pessoa_empresa + "//Funcionario");
            //Caso não exista a pasta com o código do professor, criar.
            if (!System.IO.Directory.Exists(pathCodFuncionario))
                System.IO.Directory.CreateDirectory(pathCodFuncionario);
            if (!System.IO.Directory.Exists(fullPath + "//" + funcionario.cd_pessoa_empresa + "//Funcionario//" + funcionario.cd_funcionario + "//Assinatura"))
                System.IO.Directory.CreateDirectory(fullPath + "//" + funcionario.cd_pessoa_empresa + "//Funcionario//" + funcionario.cd_funcionario + "//Assinatura");

            if (funcionario.nome_assinatura_certificado.Any(c => invalidFileNameChars.Contains(c)))
                throw new FuncionarioBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosImagem + funcionario.nome_assinatura_certificado,
                    null, FuncionarioBusinessException.TipoErro.ERRO_CERTIFICADO_JA_EXISTE, false);
            if (fullPath.Any(c => invalidPathChars.Contains(c)))
                throw new FuncionarioBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosPathArquivoImagem + funcionario.nome_assinatura_certificado,
                    null, FuncionarioBusinessException.TipoErro.ERRO_CERTIFICADO_JA_EXISTE, false);
            //string pathContratosEscola = caminho_relatorios + "/Contratos/" + cdEscola;
            if (System.IO.File.Exists(imagemCertificado))
                throw new FuncionarioBusinessException(Utils.Messages.Messages.msgErroImagemJaCadastrado, null, FuncionarioBusinessException.TipoErro.ERRO_CERTIFICADO_JA_EXISTE, false);
            //System.IO.File.Delete(documentoContrato);
            System.IO.File.Move(imagemCertificadoTemp, imagemCertificado);
            if (System.IO.File.Exists(imagemCertificadoTemp))
                System.IO.File.Delete(imagemCertificadoTemp);
            if(!string.IsNullOrEmpty(funcionario.nome_assinatura_certificado_anterior))
                if (System.IO.File.Exists(fullPath + "//" + funcionario.cd_pessoa_empresa + "//Funcionario//" + funcionario.cd_funcionario + "//Assinatura//" + 
                        funcionario.nome_assinatura_certificado_anterior))
                    System.IO.File.Delete(fullPath + "//" + funcionario.cd_pessoa_empresa + "//Funcionario//" + funcionario.cd_funcionario + "//Assinatura//" +
                        funcionario.nome_assinatura_certificado_anterior);
        }

        public Professor getFuncionarioEditById(int cdFuncionario, int cdEscola, ProfessorDataAccess.TipoConsultaProfessorEnum tipo)
        {
            return daoProf.getFuncionarioEditById(cdFuncionario, cdEscola, tipo);
        }

        public List<ProdutoFuncionario> getProdutoFuncionarioByFuncionario(int cdFuncionario, int cdEscola)
        {
            return daoProdutoFuncionario.searchProdutosFuncionario(cdFuncionario, cdEscola).ToList();
        }


        private void crudHabilitacaoProfessor(List<HabilitacaoProfessor> habilitacaoProfessorUI, int cdProfessor)
        {
            List<HabilitacaoProfessor> habilitacaoProfessorView = new List<HabilitacaoProfessor>();
            HabilitacaoProfessor habilitacaoProfessor = new HabilitacaoProfessor();
            List<HabilitacaoProfessor> habilitacaoProfessorContext = daoProf.getAllHabilitacaoProfessorByCdProfessor(cdProfessor);
            if (habilitacaoProfessorUI != null)
            {
                habilitacaoProfessorView = habilitacaoProfessorUI;
                IEnumerable<HabilitacaoProfessor> HabilitacaoProfessorComCodigo = from hpts in habilitacaoProfessorView
                                                                                  where hpts.cd_habilitacao_professor != 0
                                                                                  select hpts;
                IEnumerable<HabilitacaoProfessor> habilitacaoProfessorDeleted = habilitacaoProfessorContext.Where(tc => !HabilitacaoProfessorComCodigo.Any(tv => tc.cd_habilitacao_professor == tv.cd_habilitacao_professor));
                if (habilitacaoProfessorDeleted.Count() > 0)
                {
                    foreach (var item in habilitacaoProfessorDeleted)
                    {
                        var deletarhabilitacaoProfessor = (from hp in habilitacaoProfessorContext where hp.cd_habilitacao_professor == item.cd_habilitacao_professor select hp).FirstOrDefault();
                        if (deletarhabilitacaoProfessor != null)
                        {
                            daoProf.deletarHabilitacaoProfessor(deletarhabilitacaoProfessor);
                        }
                    }
                }
                foreach (var item in habilitacaoProfessorView)
                {
                    if (item.cd_habilitacao_professor.Equals(null) || item.cd_habilitacao_professor == 0)
                    {
                        item.cd_professor = cdProfessor;
                        daoProf.addHabilitacaoProfessor(item);
                    }
                    else
                    {
                        var habilProf = (from hp in habilitacaoProfessorContext where hp.cd_habilitacao_professor == item.cd_habilitacao_professor select hp).FirstOrDefault();
                        if (habilProf != null && habilProf.cd_habilitacao_professor > 0)
                        {
                            habilitacaoProfessor = FuncionarioUI.changesValueHabilProfessor(item, habilProf);
                            funcionarioBusiness.saveFuncionario(false);
                        }
                    }
                }
            }
            else
            {
                if (habilitacaoProfessorContext != null)
                {
                    foreach (var item in habilitacaoProfessorContext)
                    {
                        var deletarhabilitacaoProfessor = (from hp in habilitacaoProfessorContext where hp.cd_habilitacao_professor == item.cd_habilitacao_professor select hp).FirstOrDefault();
                        if (deletarhabilitacaoProfessor != null && deletarhabilitacaoProfessor.cd_habilitacao_professor > 0)
                        {
                            daoProf.deletarHabilitacaoProfessor(deletarhabilitacaoProfessor);
                        }
                    }
                }
            }
        }

        private void crudHorarioProfessor(List<Horario> horariosUI, int cdProfessor, int cdEscola)
        {
            List<Horario> HorariosView = new List<Horario>();
            Horario horario = new Horario();
            //to do Deivid
            int[] cdProf = new int[1];
            cdProf[0] = cdProfessor;
            DateTime dt_inicio = new DateTime(2000,01,01); //Vai analisar período de 01/01/200 a 01/01/2050 pois não temos aqui dados da turma
            DateTime? dt_final = new DateTime(2050, 01, 01);
            List<Horario> horarioContext = secretBiz.getHorarioByEscolaForRegistro(cdEscola, cdProfessor, Horario.Origem.PROFESSOR).ToList();
            if (horariosUI != null)
            {
                if (!verificaHorarioOcupado(horariosUI))
                    throw new FuncionarioBusinessException(string.Format(Utils.Messages.Messages.msgErroDispHorarioProf), null, FuncionarioBusinessException.TipoErro.ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA, false);
                else
                {
                    HorariosView = horariosUI;
                    IEnumerable<Horario> horarioProfessorComCodigo = from hpts in HorariosView
                                                                     where hpts.cd_horario != 0
                                                                     select hpts;
                    List<Horario> horarioProfessorDeleted = horarioContext.Where(tc => !horarioProfessorComCodigo.Any(tv => tc.cd_horario == tv.cd_horario)).ToList();

                    //Verificar se existe horario ocupado 
                    //Se existe horario ocupado, verificar se tem horario disponivel para ele
                    List<Horario> horarioOcupado = secretBiz.getHorarioOcupadosForTurma(cdEscola, cdProfessor, cdProf, 0, 0, 0, dt_inicio, dt_final, 
                        HorarioDataAccess.TipoConsultaHorario.HAS_HORARIO_PROF_OCUPADO_TURMA).ToList();
                    if (horarioOcupado.Count() > 0)
                        foreach (Horario h in horarioOcupado)
                        {
                            Boolean valido = HorariosView.Where(comp => h.dt_hora_ini >= comp.dt_hora_ini && h.dt_hora_ini <= h.dt_hora_fim &&
                                h.dt_hora_fim <= comp.dt_hora_fim && h.id_dia_semana == comp.id_dia_semana && comp.calendar == "Calendar1").Any();
                            if (!valido)
                                throw new FuncionarioBusinessException(string.Format(Utils.Messages.Messages.msgErroDispHorarioProf), null, FuncionarioBusinessException.TipoErro.ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA, false);

                        }


                    if (horarioProfessorDeleted.Count() > 0)
                    {
                        foreach (var item in horarioProfessorDeleted)
                        {
                            //var deletarHorarioProfessor = (from hp in horarioContext where hp.cd_horario == item.cd_horario select hp).FirstOrDefault();
                            if (item != null)
                            {
                                secretBiz.deleteHorario(item);
                            }
                        }
                    }
                    HorariosView = HorariosView.Where(x => x.calendar == "Calendar1").ToList();
                    foreach (var item in HorariosView)
                    {
                        if (item.cd_horario.Equals(null) || item.cd_horario == 0)
                        {
                            item.cd_pessoa_escola = cdEscola;
                            item.cd_registro = cdProfessor;
                            item.endTime = item.endTime.ToLocalTime();
                            item.startTime = item.startTime.ToLocalTime();
                            item.id_origem = (int)Horario.Origem.PROFESSOR;
                            item.dt_hora_ini = new TimeSpan(item.startTime.Hour, item.startTime.Minute, 0);
                            item.dt_hora_fim = new TimeSpan(item.endTime.Hour, item.endTime.Minute, 0);
                            secretBiz.addHorario(item);
                        }
                        else
                        {
                            var horarioProf = (from hp in horarioContext where hp.cd_horario == item.cd_horario select hp).FirstOrDefault();
                            if (horarioProf != null && horarioProf.cd_horario > 0)
                            {
                                item.cd_registro = horarioProf.cd_registro;
                                //horarioProf = Horario.changeValueHorario(horarioProf, item);
                                secretBiz.editHorarioContext(horarioProf, item);
                            }
                        }
                    }
                }
            }
            else
            {
                if (horarioContext != null)
                {
                    foreach (var item in horarioContext)
                    {
                        var deletarHorarioProfessor = (from hp in horarioContext where hp.cd_horario == item.cd_horario select hp).FirstOrDefault();
                        if (deletarHorarioProfessor != null && deletarHorarioProfessor.cd_horario > 0)
                        {
                            secretBiz.deleteHorario(deletarHorarioProfessor);
                        }
                    }
                }


            }
        }

        public IEnumerable<ProfessorUI> getProfessorReturnProfUI(ProfessorDataAccess.TipoConsultaProfessorEnum hasDependente, int cdEscola, int? cdProfessor)
        {
            return daoProf.getProfessorReturnProfUI(hasDependente, cdEscola, cdProfessor);
        }

        public IEnumerable<ProfessorUI> getProfessorTurma(int cd_escola, int cd_turma)
        {
            return daoProf.getProfessorTurma(cd_escola, cd_turma);
        }

        public IEnumerable<ProfessorUI> getProfessorTurmaLogado(int cd_escola, int cd_turma, int cd_usuario)
        {
            return daoProf.getProfessorTurmaLogado(cd_escola, cd_turma, cd_usuario);
        }

        public IEnumerable<ProfessorUI> getFuncionariosByEscola(int cdEscola, int? cdProfessor, bool? status)
        {
            return daoProf.getFuncionariosByEscola(cdEscola, cdProfessor, status);
        }

        public IEnumerable<ProfessorUI> getFuncionariosByEscolaAtividade(int cdEscola, int? cdProfessor, int? cd_atividade_extra, bool? status)
        {
            return daoProf.getFuncionariosByEscolaAtividade(cdEscola, cdProfessor, cd_atividade_extra, status);
        }

        public IEnumerable<ProfessorUI> getFuncionariosByEscolaAulaReposicao(int cdEscola, int? cdProfessor, bool? status)
        {
            return daoProf.getFuncionariosByEscolaAulaReposicao(cdEscola, cdProfessor, status);
        }

        public FuncionarioSGF getProfLogByCodPessoaUsuario(int cd_pessoa_usuario, int cd_pessoa_empresa)
        {
            return getProfLogByCodPessoaUsuario(cd_pessoa_usuario, cd_pessoa_empresa, TransactionScopeBuilder.TransactionType.UNCOMMITED);
        }
        public FuncionarioSGF getProfLogByCodPessoaUsuario(int cd_pessoa_usuario, int cd_pessoa_empresa, TransactionScopeBuilder.TransactionType TransactionType)
        {
            FuncionarioSGF funcionario = new FuncionarioSGF();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionType))
            {
                funcionario = daoProf.getProfLogByCodPessoaUsuario(cd_pessoa_usuario, cd_pessoa_empresa);
                transaction.Complete();
            }
            return funcionario;
        }

        public IEnumerable<ProfessorUI> getProfessorAvaliacaoTurma(int cd_pessoa_empresa)
        {
            return daoProf.getProfessorAvaliacaoTurma(cd_pessoa_empresa);
        }

        public IEnumerable<FuncionarioSearchUI> getProfessoresDisponiveisFaixaHorario(SearchParameters parametros, string desc, string nomeRed, bool inicio, bool? status, string cpf, int sexo, List<Horario> horariosTurma, 
            int cd_escola, int cd_turma, int cd_curso, bool PPT_pai, int cd_produto, DateTime dtInicio, DateTime? dtFinal, int cd_duracao)
        {
            IEnumerable<FuncionarioSearchUI> retorno = new List<FuncionarioSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, daoProf.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                retorno = daoProf.getProfessoresDisponiveisFaixaHorario(parametros, desc, nomeRed, inicio, status, cpf, sexo, horariosTurma, cd_escola, cd_turma, cd_curso, 
                    PPT_pai, cd_produto, dtInicio, dtFinal, cd_duracao);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Professor> getProfessoresDisponiveisFaixaHorarioTurma(TurmaAlunoProfessorHorario turmaProfessorHorario, int cdEscola)
        {
            var totalProfessoresView = turmaProfessorHorario.horario.HorariosProfessores.Count();
            var totalProfessoresBD = daoProf.getProfessoresDisponiveisFaixaHorarioTurma(turmaProfessorHorario, cdEscola).Count();
            if (totalProfessoresView != totalProfessoresBD)
                throw new FuncionarioBusinessException(string.Format(Messages.msgProfHorarioDisponivel), null, FuncionarioBusinessException.TipoErro.ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA, false);
            return daoProf.getProfessoresDisponiveisFaixaHorarioTurma(turmaProfessorHorario, cdEscola);
        }

        public IEnumerable<Professor> getRetProfessoresDisponiveisFaixaHorarioTurma(TurmaAlunoProfessorHorario turmaProfessorHorario, int cdEscola)
        {
            return daoProf.getProfessoresDisponiveisFaixaHorarioTurma(turmaProfessorHorario, cdEscola);
        }

        public ProfessorUI verificaRetornaSeUsuarioLogadoEProfessor(int cd_pessoa_usuario, int cd_pessoa_empresa)
        {
            return daoProf.verificaRetornaSeUsuarioLogadoEProfessor(cd_pessoa_usuario, cd_pessoa_empresa);
        }

        public IEnumerable<ProfessorUI> getFuncionariosByEscolaWithAtividadeExtra(int cd_pessoa_escola, bool status)
        {
            return daoProf.getFuncionariosByEscolaWithAtividadeExtra(cd_pessoa_escola, status);
        }
        public FuncionarioSearchUI getProfessoresById(int cdProf, int cd_escola)
        {
            return daoProf.getProfessoresById(cdProf, cd_escola);
        }

        public IEnumerable<FuncionarioSearchUI> getProfessoresAulaReposicao(int cd_escola)
        {
            return daoProf.getProfessoresAulaReposicao(cd_escola);
        }

        public IEnumerable<FuncionarioSearchUI> professoresDisponiveisFaixaHorarioTurmaFilhaPPT(HorariosTurmasHorariosProfessores horariosTurmaFilhaEPai, int cd_escola)
        {
            IEnumerable<FuncionarioSearchUI> listaFuncionarios = new List<FuncionarioSearchUI>();
            List<HorarioProfessorTurma> horarioProfessorTurma = new List<HorarioProfessorTurma>();
            List<Horario> horariosContidosFilha = new List<Horario>();
            List<HorarioProfessorTurma> horarioProfessor = new List<HorarioProfessorTurma>();
            if (horariosTurmaFilhaEPai.id_liberar_habilitacao_professor)
                horariosTurmaFilhaEPai.horariosTurmaPPT = verificarProfessoresHabilitacaoProfessor(horariosTurmaFilhaEPai.horariosTurmaPPT, horariosTurmaFilhaEPai.cd_curso, cd_escola);
            // Busca os professores disponíveis na faixa de horário (da turma ppt filha) selecionada e que estão na turma PPT:
            if (horariosTurmaFilhaEPai.horariosTurmaFilha != null && horariosTurmaFilhaEPai.horariosTurmaFilha.Count() > 0 &&
                horariosTurmaFilhaEPai.horariosTurmaPPT != null && horariosTurmaFilhaEPai.horariosTurmaPPT.Count() > 0)
            {
                // Faz a relação de horário de turma ppt com ppt filha, pois os horários selecionados são da filha e os professores são da pai:
                foreach (var hf in horariosTurmaFilhaEPai.horariosTurmaFilha)
                    foreach (var ht in horariosTurmaFilhaEPai.horariosTurmaPPT)
                        if (ht.id_dia_semana == hf.id_dia_semana &&
                                hf.dt_hora_ini >= ht.dt_hora_ini && hf.dt_hora_ini < ht.dt_hora_fim &&
                                hf.dt_hora_fim > ht.dt_hora_ini && hf.dt_hora_fim <= ht.dt_hora_fim)
                        {
                            horariosContidosFilha.Add(hf);
                            horarioProfessor = horarioProfessor.Union(ht.HorariosProfessores).ToList();
                        }

                if (horarioProfessor != null && horarioProfessor.Count() > 0)
                {
                    horarioProfessor = horarioProfessor.GroupBy(hp => hp.cd_professor).Select(grp => grp.First()).ToList();
                    foreach (var hp in horarioProfessor)
                    {
                        int countPorfHorario = horariosTurmaFilhaEPai.horariosTurmaPPT.Where(ht => ht.HorariosProfessores.Where(hpt => hpt.cd_professor == hp.cd_professor).Any()).Count();
                        if (countPorfHorario >= horariosContidosFilha.Count())
                            horarioProfessorTurma.Add(hp);
                    }
                }
            }
            if (horarioProfessorTurma.Count() > 0)
            {
                int[] cdProfs = new int[horarioProfessorTurma.Count()];
                for (int i = 0; i < horarioProfessorTurma.Count(); i++)
                {
                    cdProfs[i] = horarioProfessorTurma[i].cd_professor;

                }
                listaFuncionarios = daoProf.getProfessoresByCodigos(cdProfs, cd_escola);
            }
            return listaFuncionarios;
        }

        private List<Horario> verificarProfessoresHabilitacaoProfessor(List<Horario> horariosTurmaPPT, int cd_curso, int cd_escola)
        {
            List<Horario> horarioLiberados = new List<Horario>();
            if (horariosTurmaPPT != null)
            {
                List<int> cdsProfs = new List<int>();
                foreach (Horario h in horariosTurmaPPT)
                    cdsProfs = cdsProfs.Union(h.HorariosProfessores.Select(p => p.cd_professor)).ToList();
                if (cdsProfs != null && cdsProfs.Count > 0)
                {
                    List<int> cdsProfsLiberados = daoProf.getVerificaProfessorHabilitacaoCursos(cdsProfs, cd_curso, cd_escola);
                    foreach (Horario h in horariosTurmaPPT)
                        h.HorariosProfessores = h.HorariosProfessores.Where(x => cdsProfsLiberados.Contains(x.cd_professor)).ToList();
                }
            }
            return horariosTurmaPPT;
        }

        public IEnumerable<FuncionarioSearchUI> getProfessoresTurmaPPTPorFaixaHorariosTurmaFilha(List<Horario> horariosTurmaFilha, int cd_escola, int cd_turma_PPT)
        {
            return daoProf.getProfessoresTurmaPPTPorFaixaHorariosTurmaFilha(horariosTurmaFilha, cd_escola, cd_turma_PPT);
        }

        public IEnumerable<ProfessorUI> getProfHorariosProgTurma(int cd_turma, int cd_escola, byte diaSemana, TimeSpan horaIni, TimeSpan horaFim)
        {
            return daoProf.getProfHorariosProgTurma(cd_turma, cd_escola, diaSemana, horaIni, horaFim);
        }

        private void validarMinDateFuncionario(FuncionarioSGF funcionario)
        {
            if ((funcionario != null && funcionario.dt_admissao != null && DateTime.Compare((DateTime)funcionario.dt_admissao, new DateTime(1900, 1, 1)) < 0) ||
                (funcionario != null && funcionario.dt_demissao != null && DateTime.Compare((DateTime)funcionario.dt_demissao, new DateTime(1900, 1, 1)) < 0))
                throw new PessoaBusinessException(Componentes.Utils.Messages.Messages.msgErroMinDateDataNascPessoa, null,
                      FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_MINDATE_SMALLDATETIME, false);
        }

        public IEnumerable<FuncionarioSearchUI> getProfessoresByEmpresa(int cd_pessoa_empresa, int? cd_turma)
        {
            List<FuncionarioSearchUI> listFuncSearch = new List<FuncionarioSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                listFuncSearch = daoProf.getProfessoresByEmpresa(cd_pessoa_empresa, cd_turma).ToList();
                transaction.Complete();
            }
            return listFuncSearch;
        }

        public bool deleteFuncionarios(List<FuncionarioSGF> funcionarios, int cd_pessoa_empresa)
        {
            this.sincronizarContextos(daoProf.DB());
            bool retorno = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (funcionarios != null && funcionarios.Count() > 0)
                {
                    List<FuncionarioCyberBdUI> listaFuncionarioCyberDelete = new List<FuncionarioCyberBdUI>();

                    foreach (FuncionarioSGF func in funcionarios)
                    {
                        if (businessApiNewCyberFuncionario.aplicaApiCyber())
                        {
                            //preeche a lista dos funcionarios que irão ser deletados no cyber
                            preencheListaFuncionariosCyberDelete(func, listaFuncionarioCyberDelete);
                        }




                        FuncionarioSGF funcionario = daoProf.findByIdFuncionario(func.cd_funcionario, cd_pessoa_empresa);
                        funcionario.cd_pessoa_funcionario = funcionario.cd_pessoa_funcionario;
                        if (funcionario != null)
                        {
                            IEnumerable<ProdutoFuncionario> produtosFuncionarioCTX = daoProdutoFuncionario.searchProdutosFuncionario(funcionario.cd_funcionario, cd_pessoa_empresa);
                            foreach (var produto in produtosFuncionarioCTX)
                            {
                                var deletarItem = daoProdutoFuncionario.findById(produto.cd_produto_funcionario, false);
                                var del = deletarItem != null ? daoProdutoFuncionario.delete(deletarItem, false) : false;
                            }

                            crudHorarioProfessor(null, funcionario.cd_funcionario, cd_pessoa_empresa);
                            daoProf.deleteProfessorContexto(funcionario);
                        }
                    }
                    daoProf.saveChanges(false);

                    if (businessApiNewCyberFuncionario.aplicaApiCyber())
                    {
                        //inativa no cyber os funcionarios deletados 
                        if (listaFuncionarioCyberDelete != null && listaFuncionarioCyberDelete.Count > 0)
                        {
                            foreach (FuncionarioCyberBdUI funcionarioCyberBdUiDelete in listaFuncionarioCyberDelete)
                            {
                                //se o funcionario existe no cyber
                                if (funcionarioCyberBdUiDelete != null && existeFuncionario((verificaTipoFuncionarioSigla(funcionarioCyberBdUiDelete.tipo_funcionario) + funcionarioCyberBdUiDelete.codigo), funcionarioCyberBdUiDelete.tipo_funcionario))
                                {
                                    //Chama a api cyber com comando (INATIVA_PROFESSOR|INATIVA_COORDENADOR|INATIVA_COLABORADOR)
                                    inativaFuncionario(funcionarioCyberBdUiDelete);
                                }
                            }
                        }
                    }

                    retorno = true;


                }


                transaction.Complete();
            }
            if (retorno && funcionarios != null && funcionarios.Count() > 0)
                foreach (FuncionarioSGF f in funcionarios)
                {
                    try
                    {
                        pessoaBusiness.deletePessoa(f.cd_pessoa_funcionario);
                    }
                    catch
                    {
                        //Execption não tratado pois a exclusão e condicional (se existir ligações com a pessoa, ela não poderá ser excluida).
                    }
                }

            return retorno;
        }

        public void inativaFuncionario(FuncionarioCyberBdUI funcionarioCyber)
        {

            if (funcionarioCyber.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.PROFESSOR)
            {
                executaCiberAtivacaoInativacaoFuncionario(funcionarioCyber.codigo, ApiCyberComandosNames.INATIVA_PROFESSOR);
            }
            else if (funcionarioCyber.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COORDENADOR)
            {
                executaCiberAtivacaoInativacaoFuncionario(funcionarioCyber.codigo, ApiCyberComandosNames.INATIVA_COORDENADOR);
            }
            else if (funcionarioCyber.tipo_funcionario == (byte)FuncionarioSGF.TipoAlunoFuncionario.COLABORADOR)
            {
                executaCiberAtivacaoInativacaoFuncionario(funcionarioCyber.codigo, ApiCyberComandosNames.INATIVA_COLABORADOR);
            }

        }

        private void preencheListaFuncionariosCyberDelete(FuncionarioSGF func, List<FuncionarioCyberBdUI> listaFuncionarioCyberDelete)
        {
            if (func != null)
            {
                //pega o funcionario que ira deletar no cyber
                FuncionarioCyberBdUI funcionarioCyberBd = funcionarioBusiness.findFuncionarioByCdFuncionario(func.cd_funcionario, func.cd_pessoa_empresa);
                if (funcionarioCyberBd != null)
                {
                    listaFuncionarioCyberDelete.Add(funcionarioCyberBd);
                }
            }
        }

        public IEnumerable<FuncionarioSearchUI> getSearchFuncionario(SearchParameters parametros, string nome, string apelido, bool? status, string cpf, bool inicio, byte tipo, int cdEscola, int sexo,
            int cdAtividade, int coordenador, int colaborador_cyber)
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
                retorno = daoProf.getSearchFuncionario(parametros, nome, apelido, status, cpf, inicio, tipo, cdEscola, sexo, cdAtividade, coordenador, colaborador_cyber);
                transaction.Complete();
            }
            return retorno;
        }

        public PessoaFisicaSearchUI ExistFuncionarioOrPessoaFisicaByCpf(string cpf, int cdEmpresa)
        {
            return funcionarioBusiness.ExistFuncionarioOrPessoaFisicaByCpf(cpf, cdEmpresa);
        }
        public IEnumerable<FuncionarioSGF> getFuncionariosByAulaPers(int cdEscola)
        {
            return daoProf.getFuncionariosByAulaPers(cdEscola);
        }
        public IEnumerable<ProfsemDiarioUI> getProfessorsemDiario(SearchParameters parametros, int cd_turma, int cd_professor, int cdEscola, bool idLiberado)
        {
            IEnumerable<ProfsemDiarioUI> retorno = new List<ProfsemDiarioUI>();

            if (parametros.sort == null)
            {
                if (cdEscola == 0)
                    parametros.sort = "dc_reduzido_pessoa";
                else
                    parametros.sort = "no_professor";
            }
            parametros.sort = parametros.sort.Replace("liberado", "id_liberado");

            retorno = daoProf.getProfessorsemDiario(parametros, cd_turma, cd_professor, cdEscola, idLiberado);
            return retorno;
        }

        #endregion

        #region Horario - HorarioProfessorTurma
        public IEnumerable<ReportDiarioAula> getRelatorioDiarioAula(int cd_escola, int cd_turma, int cd_professor, DateTime dt_inicial, DateTime dt_final)
        {
            return daoProfTurma.getRelatorioDiarioAula(cd_escola, cd_turma, cd_professor, dt_inicial, dt_final);
        }

        private bool verificaHorarioOcupado(List<Horario> horarios)
        {
            List<Horario> horarioIntervalo = new List<Horario>();
            //Pegando os itens do Calendar2 (Ocupado)
            IEnumerable<Horario> horariosOcupados = horarios.Where(c => c.calendar == "Calendar2");

            //Pegando os itens do Calendar1 (Disponiveis)
            IEnumerable<Horario> horariosDisponiveis = horarios.Where(c => c.calendar == "Calendar1");

            //Para todo horário ocupado deve possuir um horário disponível que contém ele:
            //Verificar se existe algum horário ocupado que não possui um horário disponível que contém ele:
            return !horariosOcupados.Where(ho => !horariosDisponiveis.Any(hd => hd.id_dia_semana == ho.id_dia_semana && hd.dt_hora_ini <= ho.dt_hora_ini && hd.dt_hora_fim >= ho.dt_hora_fim)).Any();
        }

        public void deleteProfessorHorario(HorarioProfessorTurma itemProf)
        {
            daoHorarioProfessorTurma.delete(itemProf, false);
        }

        public HorarioProfessorTurma addProfessorHorario(HorarioProfessorTurma itemProf)
        {
            return daoHorarioProfessorTurma.add(itemProf, false);
        }

        public HorarioProfessorTurma editProfessorHorario(HorarioProfessorTurma professorHorario)
        {
            return daoHorarioProfessorTurma.edit(professorHorario, false);
        }

        public List<HorarioProfessorTurma> getHorarioProfessorByHorario(int cd_horario)
        {
            return daoHorarioProfessorTurma.getHorarioProfessorByHorario(cd_horario).ToList();
        }

        public IEnumerable<HorarioProfessorTurma> getHorarioProfessorByProfessorTurmaPPT(int cd_professor, int cd_turma_ppt)
        {
            return daoHorarioProfessorTurma.getHorarioProfessorByProfessorTurmaPPT(cd_professor, cd_turma_ppt);
        }

        #endregion

        #region Pagamento Professor

        public IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessores(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim)
        {
            return daoProf.getRptPagamentoProfessores(cd_tipo_relatorio, cd_empresa, cd_professor, dt_ini, dt_fim);
        }

        public IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessoresObs(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim)
        {
            return daoProf.getRptPagamentoProfessoresObs(cd_tipo_relatorio, cd_empresa, cd_professor, dt_ini, dt_fim);
        }

        public IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessoresFaltas(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim)
        {
            return daoProf.getRptPagamentoProfessoresFaltas(cd_tipo_relatorio, cd_empresa, cd_professor, dt_ini, dt_fim);
        }

        #endregion

        #region Comissão Secretarias

        public IEnumerable<FuncionarioComissao> getRptComissaoSecretarias(int cd_funcionario, int cd_produto, int cd_empresa, DateTime? dt_ini, DateTime? dt_fim)
        {
            IEnumerable<FuncionarioComissao> retorno = new List<FuncionarioComissao>();
//            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, daoProf.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                retorno = daoProf.getRptComissaoSecretarias(cd_funcionario, cd_produto, cd_empresa, dt_ini, dt_fim);
                transaction.Complete();
            }
            return retorno;
        }

        public string postLiberarProfessor(int cd_professor, int cd_usuario, int fusoHorario)
        {
            string retorno = null;

            retorno = daoProf.postLiberarProfessor(cd_professor, cd_usuario, fusoHorario);
            return retorno;
        }

        #endregion
    }
}
