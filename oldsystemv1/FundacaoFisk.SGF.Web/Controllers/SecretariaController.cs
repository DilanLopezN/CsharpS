using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Utils;
using Componentes.Utils.Messages;
using log4net;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using Componentes.GenericController;
using FundacaoFisk.SGF.Web.Services.Auth.Comum;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using Componentes.GenericBusiness.Comum;
using System.Configuration;
using System.IO;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Business;

namespace FundacaoFisk.SGF.Web.Controllers
{
    public class SecretariaController : ComponentesMVCController
    {
        //
        // GET: /Secretaria/
   
        private static readonly ILog logger = LogManager.GetLogger(typeof(SecretariaController));
        public SecretariaController()
        {
        }

        #region ActionResult

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Prospect()
        {
            return View();
        }

        public ActionResult Aluno()
        {
            return View();
        }

        public ActionResult Pessoa()
        {
            return View();
        }

        public ActionResult NomeContrato()
        {
            return View();
        }

        public ActionResult Movimentos()
        {
            return View();
        }

        public ActionResult FollowUp()
        {
            return View();
        }

        public ActionResult Matricula()
        {
            return View();
        }

        public ActionResult HistoricoAluno()
        {
            return View();
        }

        public ActionResult Faturamento()
        {
            return View();
        }

        public ActionResult TestePortalAluno()
        {
            return View();
        }

        public ActionResult GeracaoNotasXML()
        {
            return View();
        }

        public ActionResult SmsGestao()
        {
            return View();
        }
        public ActionResult EnviarTransferencia()
        {
            return View();
        }

        public ActionResult ReceberTransferencia()
        {
            return View();
        }


        #endregion

        //Retorna o código da escola
        private int recoverEscola()
        {
            var codEscola = (int)Session["CodEscolaSelecionada"];
            return codEscola;
        }

        #region prospect
        [MvcComponentesAuthorize(Roles = "pros.i")]
        [MvcComponentesAuthorize(Roles = "prod")]
        [MvcComponentesAuthorize(Roles = "mtnm")]
        [MvcComponentesAuthorize(Roles = "oper")]
        [MvcComponentesAuthorize(Roles = "mid")]
        public RenderJsonActionResult postInsertProspectQuandoEnviarEmail(ProspectSearchUI prospect)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(prospect.email) && !Utils.Utils.validarEmail(prospect.email))
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroEmailInvalido, null,
                       SecretariaBusinessException.TipoErro.ERRO_EMAIL_INVALIDO, false);

                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                ILocalidadeBusiness BusinessLoc = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { BusinessSecretaria });
                int cdEscola = recoverEscola();
                prospect.cd_pessoa_escola = cdEscola;
                prospect.cd_usuario = int.Parse(Session["CodUsuario"] + "");

                ProspectSearchUI insertProspect = BusinessSecretaria.insertProspect(prospect);
                BusinessSecretaria.enviarEmailProspectAndUpdIdEmail(insertProspect, cdEscola, insertProspect.listaFollowUp);

                retorno.retorno = insertProspect;
                if (insertProspect.cd_prospect <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return new RenderJsonActionResult { Result = retorno };
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_EMAILJAEXISTENTE ||
                    ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_TELEFONEJAEXISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (AlunoBusinessException ex)
            {
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_EMAIL_JA_EXITE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (SecretariaBusinessException ex)
            {
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_LOCAL_MOVIMENTO_AUSENTE_PRE_MATRICULA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "pros.i")]
        [MvcComponentesAuthorize(Roles = "prod")]
        [MvcComponentesAuthorize(Roles = "mtnm")]
        [MvcComponentesAuthorize(Roles = "oper")]
        [MvcComponentesAuthorize(Roles = "mid")]
        public RenderJsonActionResult postInsertProspect(ProspectSearchUI prospect)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(prospect.email) && !Utils.Utils.validarEmail(prospect.email))
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroEmailInvalido, null,
                       SecretariaBusinessException.TipoErro.ERRO_EMAIL_INVALIDO, false);

                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                ILocalidadeBusiness BusinessLoc = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { BusinessSecretaria });
                int cdEscola = recoverEscola();
                prospect.cd_pessoa_escola = cdEscola;
                prospect.cd_usuario =   int.Parse(Session["CodUsuario"] + "");
                
                ProspectSearchUI insertProspect = BusinessSecretaria.insertProspect(prospect);
                
                
                retorno.retorno = insertProspect;
                if (insertProspect.cd_prospect <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_EMAILJAEXISTENTE ||
                    ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_TELEFONEJAEXISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (AlunoBusinessException ex)
            {
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_EMAIL_JA_EXITE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (SecretariaBusinessException ex)
            {
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_LOCAL_MOVIMENTO_AUSENTE_PRE_MATRICULA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    SecretariaBusinessException fx = new SecretariaBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }
        }

        [HttpGet]
        [MvcComponentesAuthorize(Roles = "pros")]
        public ActionResult getProspectSearch(string nome, bool inicio, string email, string dataIni, string dataFim, int? status, bool aluno, int testeClassificacaoMatriculaOnline)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (!status.HasValue)
                    status = 0;
                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                DateTime? dataInicial = !String.IsNullOrEmpty(dataIni) ? (DateTime?)Convert.ToDateTime(dataIni) : null;
                DateTime? dataFinal = !String.IsNullOrEmpty(dataFim) ? (DateTime?)Convert.ToDateTime(dataFim) : null;

                int cdEscola = recoverEscola();
                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                var ret = BusinessSecretaria.GetProspectSearch(parametros, nome, inicio, email, cdEscola, dataInicial, dataFinal, getStatus(status.Value), aluno, testeClassificacaoMatriculaOnline);
                var retRender = new RenderJsonActionResult { Result = ret, parameters = parametros };
                return retRender;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "pros.a")]
        [MvcComponentesAuthorize(Roles = "prod")]
        [MvcComponentesAuthorize(Roles = "mtnm")]
        [MvcComponentesAuthorize(Roles = "oper")]
        [MvcComponentesAuthorize(Roles = "mid")]
        public RenderJsonActionResult postAlterarProspectQuandoEnviarEmail(ProspectSearchUI prospect)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(prospect.email) && !Utils.Utils.validarEmail(prospect.email))
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroEmailInvalido, null,
                       SecretariaBusinessException.TipoErro.ERRO_EMAIL_INVALIDO, false);


                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { BusinessSecretaria });
                //Pega na sessão a escola logada
                var cdEscola = recoverEscola();
                int cdUsuario = int.Parse(Session["CodUsuario"] + "");
                prospect.cd_pessoa_escola = cdEscola;

                var prospectEdit = BusinessSecretaria.editProspect(prospect, cdUsuario);
                BusinessSecretaria.enviarEmailProspectAndUpdIdEmail(prospectEdit, cdEscola, prospectEdit.listaFollowUp);

                retorno.retorno = prospectEdit;
                if (prospectEdit.cd_prospect <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_ALTERAR_PREMATRICULA_BAIXA_MANUAL ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_SALDO_NEGATIVO ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_SALDO_TITULO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_EMAILJAEXISTENTE ||
                    ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_TELEFONEJAEXISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (AlunoBusinessException ex)
            {
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_EMAIL_JA_EXITE ||
                    ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_FOLLOW_UP_DE_OUTRO_ATENDENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

            catch (SecretariaBusinessException ex)
            {
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_LOCAL_MOVIMENTO_AUSENTE_PRE_MATRICULA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "pros.a")]
        [MvcComponentesAuthorize(Roles = "prod")]
        [MvcComponentesAuthorize(Roles = "mtnm")]
        [MvcComponentesAuthorize(Roles = "oper")]
        [MvcComponentesAuthorize(Roles = "mid")]
        public RenderJsonActionResult postAlterarProspect(ProspectSearchUI prospect)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(prospect.email) && !Utils.Utils.validarEmail(prospect.email))
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroEmailInvalido, null,
                       SecretariaBusinessException.TipoErro.ERRO_EMAIL_INVALIDO, false);


                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { BusinessSecretaria });
                //Pega na sessão a escola logada
                var cdEscola = recoverEscola();
                int cdUsuario = int.Parse(Session["CodUsuario"] + "");
                prospect.cd_pessoa_escola = cdEscola;

                var prospectEdit = BusinessSecretaria.editProspect(prospect, cdUsuario);
                retorno.retorno = prospectEdit;
                if (prospectEdit.cd_prospect <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_ALTERAR_PREMATRICULA_BAIXA_MANUAL ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_SALDO_NEGATIVO ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_SALDO_TITULO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_EMAILJAEXISTENTE ||
                    ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_TELEFONEJAEXISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (AlunoBusinessException ex)
            {
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_EMAIL_JA_EXITE ||
                    ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_FOLLOW_UP_DE_OUTRO_ATENDENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

            catch (SecretariaBusinessException ex)
            {
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_LOCAL_MOVIMENTO_AUSENTE_PRE_MATRICULA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }
        }


        [MvcComponentesAuthorize(Roles = "pros")]
        public RenderJsonActionResult getExistsProspectEmail(String email, int cdProspect)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IAlunoBusiness BusinessAluno = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                IPessoaBusiness BusinessPessoa = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                ProspectSearchUI prospect;
                var escola = recoverEscola();
                prospect = BusinessSecretaria.getExistsProspectEmail(email, escola, cdProspect);
                if (prospect != null)
                {
                    prospect.cd_pessoa_escola = null;
                    retorno.retorno = prospect;
                    retorno.AddMensagem(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgExistsEmailProspect, prospect.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                else
                {
                    AlunoSearchUI aluno = null;
                    prospect = BusinessSecretaria.getExistsProspectEmail(email, 0, cdProspect);

                    if(prospect != null)
                        aluno = BusinessAluno.verificarAlunoExistEmail(email, (int)prospect.cd_pessoa_escola,0);
                    if (prospect != null)
                    {
                        string mensage = mensage = string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoProspectOutraUnidade, prospect.no_pessoa, "");
                        retorno.retorno = prospect;
                        if (aluno != null)
                        {
                            var pessoaFisica = BusinessPessoa.verificarPessoaFisicaEmail(email);
                            retorno.retorno = new
                            {
                                no_pessoa = prospect.no_pessoa,
                                cd_prospect = prospect.cd_prospect,
                                no_escola = prospect.no_escola,
                                cd_pessoa_escola = prospect.cd_pessoa_escola,
                                pessoaFisica = pessoaFisica
                            };
                            //mensage = string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoProspectUnidade, prospect.no_pessoa, "(" + prospect.no_escola + ")") + "<br>" + FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoProspectAlunoMatriculadoUnidade;
                        }

                        retorno.AddMensagem(mensage, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    }
                    else
                        if (aluno != null)
                        {
                            retorno.retorno = aluno;
                            retorno.AddMensagem(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgExistsEmailAluno, aluno.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                        }
                        else
                        {
                            var pessoaFisica = BusinessSecretaria.verificarPessoaFisicaEmail(email);
                            if (pessoaFisica != null)
                            {
                                retorno.retorno = pessoaFisica;
                                retorno.AddMensagem(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgExistsEmailPessoa, pessoaFisica.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                            }
                        }
                }
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "item")]
        public ActionResult GetUrlRelatorioProspect(string sort, int direction, string nome, string email, bool inicio, string dataIni, string dataFim, int status, bool aluno, int testeClassificacaoMatriculaOnline)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                string cdEscola = Session["CodEscolaSelecionada"] + "";
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@nome=" + nome + "&@email=" + email + "&@inicio=" + inicio + "&@escola=" + cdEscola + "&@dataIni=" + dataIni + "&@dataFim=" + dataFim + "&@ativo=" + status + "&@aluno=" + aluno + "&@testeClassificacaoMatriculaOnline=" + testeClassificacaoMatriculaOnline  + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Prospect&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.Prospect;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                retorno.retorno = parametrosCript;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Nome Contrato

        [HttpPost]
        [MvcComponentesAuthorize(Roles = "nmCt.i,nmCt.a")]
        public ActionResult UploadDocumento()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                //Arquivo que o PlUpload envia.
                var fileUpload = Request.Files[0];
                //Local onde vai ficar as fotos enviadas.
                if (fileUpload.FileName.Substring(fileUpload.FileName.Length - 5) != ".dotx")
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtesaoLayoutNaoValida, null, 
                        SecretariaBusinessException.TipoErro.ERRO_NAO_EXISTE_LAYOUT_NOME_CONTRATO, false);
                if (fileUpload.ContentLength > 500000)
                    throw new PessoaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroLimiteArquivoExcedido, null, PessoaBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                string pathContratosEscola = "";
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                int codUsuario = int.Parse(Session["CodUsuario"] + "");
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(codUsuario);
                HashSet<char> invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());
                HashSet<char> invalidPathChars = new HashSet<char>(Path.GetInvalidPathChars());
                string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
                string file_name = Utils.Utils.geradorNomeAleatorio(36);
                if (masterGeral)
                    pathContratosEscola = caminho_relatorios + "\\Contratos";
                else
                    pathContratosEscola = caminho_relatorios + "\\Contratos\\" + cd_escola;
               string nomeArquivo =  file_name + ".dotx";
               if (nomeArquivo.Any(c => invalidFileNameChars.Contains(c)))
                   throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosNomeArquivo + nomeArquivo, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
               if (pathContratosEscola.Any(c => invalidPathChars.Contains(c)))
                   throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosPathArquivo + pathContratosEscola, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
               
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    DirectoryInfo di = new DirectoryInfo(pathContratosEscola);
                    if (!di.Exists)
                        di.Create();
                    //gera a path completa onde sera salvo o arquivo.
                   
                    var path = Path.Combine(pathContratosEscola + "\\", nomeArquivo);
                    //salva o arquivo carregado.
                    fileUpload.SaveAs(path);
                }

                return new RenderJsonActionResult { Result = file_name + ".dotx" };
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotUploadImage, retorno, logger, ex);
            }
        }


        [HttpPost]
        [HttpComponentesAuthorize(Roles = "mat.i,mat.a")]
        public ActionResult UploadDocumentoDigitalizado()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                //Arquivo que o PlUpload envia.
                var fileUpload = Request.Files[0];
                //Local onde vai ficar as fotos enviadas.
                string extensao = "";
                if (fileUpload.FileName.ToLower().Contains(".png")) { extensao = fileUpload.FileName.ToLower().Substring(fileUpload.FileName.Length - 4); }
                if (fileUpload.FileName.ToLower().Contains(".pdf")) { extensao = fileUpload.FileName.ToLower().Substring(fileUpload.FileName.Length - 4); }
                if (fileUpload.FileName.ToLower().Contains(".jpg")) { extensao = fileUpload.FileName.ToLower().Substring(fileUpload.FileName.Length - 4); }
                if (fileUpload.FileName.ToLower().Contains(".jpeg")) { extensao = fileUpload.FileName.ToLower().Substring(fileUpload.FileName.Length - 5); }
                if (fileUpload.FileName.ToLower().Contains(".dotx")) { extensao = fileUpload.FileName.ToLower().Substring(fileUpload.FileName.Length - 5); }

                if (!String.IsNullOrEmpty(extensao) && extensao != ".png" && extensao != ".pdf" &&
                    extensao != ".jpg" && extensao != ".jpeg" && extensao != ".dotx")
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtesaoContratoDigitalizadoNaoValida, null,
                        SecretariaBusinessException.TipoErro.ERRO_EXTENSAO_CONTRATRO_DIGITALIZADO_NAO_VALIDA, false);
                if (fileUpload.ContentLength > 500000)
                    throw new PessoaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroLimiteArquivoExcedido, null, PessoaBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                string pathContratosEscola = "";
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                int codUsuario = int.Parse(Session["CodUsuario"] + "");
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(codUsuario);
                HashSet<char> invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());
                HashSet<char> invalidPathChars = new HashSet<char>(Path.GetInvalidPathChars());
                string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
                string file_name = Utils.Utils.geradorNomeAleatorio(36);
                string pathContratos = caminho_relatorios + "\\ContratosDigitalizados";
                //if (masterGeral)
                //    pathContratosEscola = caminho_relatorios + "\\ContratosDigitalizadosTemp";
                //else
                    pathContratosEscola = caminho_relatorios + "\\ContratosDigitalizadosTemp\\" + cd_escola;
                string nomeArquivo = file_name + extensao;
                if (nomeArquivo.Any(c => invalidFileNameChars.Contains(c)))
                    throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosNomeArquivo + nomeArquivo, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
                if (pathContratosEscola.Any(c => invalidPathChars.Contains(c)))
                    throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosPathArquivo + pathContratosEscola, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);

                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    DirectoryInfo diContratos = new DirectoryInfo(pathContratos);
                    if (!diContratos.Exists)
                        diContratos.Create();

                    DirectoryInfo di = new DirectoryInfo(pathContratosEscola);
                    if (!di.Exists)
                        di.Create();
                    //gera a path completa onde sera salvo o arquivo.

                    var path = Path.Combine(pathContratosEscola + "\\", nomeArquivo);
                    //salva o arquivo carregado.
                    fileUpload.SaveAs(path);
                }

                return new RenderJsonActionResult { Result = file_name + extensao };
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotUploadImage, retorno, logger, ex);
            }
        }


        [MvcComponentesAuthorize(Roles = "nmCt")]
        public ActionResult getArquivoDocumentoDigitalizado(string noArquivo, int cd_contrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                int codUsuario = int.Parse(Session["CodUsuario"] + "");
                string pathContratosEscola = "";
                string noCont = BusinessSecretaria.getNomeContratoDigitalizadoByCdContrato(cd_escola, cd_contrato, noArquivo);
                string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
                if (noCont != null && !String.IsNullOrEmpty(noCont))
                {
                    pathContratosEscola = caminho_relatorios + "\\ContratosDigitalizados\\" + cd_escola + "\\" + noCont;
                    
                }
                if (System.IO.File.Exists(pathContratosEscola) && !String.IsNullOrEmpty(noCont) && !String.IsNullOrEmpty(pathContratosEscola))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(pathContratosEscola);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.template", noCont);
                }
                else
                {
                    throw new SecretariaBusinessException(Utils.Messages.Messages.msgArquivoDigitalizadoNotFound, null, SecretariaBusinessException.TipoErro.ERRO_NAO_EXISTE_LAYOUT_NOME_CONTRATO, false);
                }
                    
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Utils.Messages.Messages.msgArquivoDigitalizadoNotFound))
                    logger.Warn(ex);
                else
                    logger.Error(ex);

                Session["Erro"] = FundacaoFisk.SGF.Utils.Messages.Messages.msgArquivoDigitalizadoNotFound;
                Session["StackTrace"] = ex.Message + ex.StackTrace + ex.InnerException;
                return Redirect("~/Erro/Index");
            }
        }

        [MvcComponentesAuthorize(Roles = "nmCt")]
        public ActionResult getArquivoNomeDocumento(string noRelatorio, int cdNomeContrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                int codUsuario = int.Parse(Session["CodUsuario"] + "");
                string pathContratosEscola = "";
                NomeContrato noCont = BusinessSecretaria.getNomeContratoById(cd_escola, cdNomeContrato);
                string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
                if (noCont != null && noCont.cd_pessoa_escola == null)
                    pathContratosEscola = caminho_relatorios + "\\Contratos\\" + noRelatorio;
                else
                    pathContratosEscola = caminho_relatorios + "\\Contratos\\" + cd_escola + "\\" + noRelatorio;
                if (System.IO.File.Exists(pathContratosEscola))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(pathContratosEscola);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.template", noRelatorio);
                }
                else
                    throw new SecretariaBusinessException(Utils.Messages.Messages.msgAvisoNaoexisteLayoutNomeContrato, null, SecretariaBusinessException.TipoErro.ERRO_NAO_EXISTE_LAYOUT_NOME_CONTRATO, false);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Utils.Messages.Messages.msgAvisoNaoexisteLayoutNomeContrato))
                    logger.Warn(ex);
                else
                    logger.Error(ex);

                Session["Erro"] = FundacaoFisk.SGF.Utils.Messages.Messages.msgAvisoNaoexisteLayoutNomeContrato;
                Session["StackTrace"] = ex.Message + ex.StackTrace + ex.InnerException;
                return Redirect("~/Erro/Index");
            }
        }

        [HttpGet]
        [MvcComponentesAuthorize(Roles = "envtran")]
        [HttpComponentesAuthorize(Roles = "rectran")]
        public ActionResult getDownloadArquivoHistoricoTransferenciaAluno(int cd_transferencia_aluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();

                TransferenciaAluno transferenciaAlunoBd = BusinessSecretaria.getTransferenciaAlunoById(cd_transferencia_aluno);

                

                if (transferenciaAlunoBd != null && !string.IsNullOrEmpty(transferenciaAlunoBd.no_arquivo_historico) && !String.IsNullOrEmpty(transferenciaAlunoBd.pdf_historico))
                {
                    string tipo = transferenciaAlunoBd.pdf_historico.Substring(transferenciaAlunoBd.pdf_historico.IndexOf(",") + 1);
                    Byte[] buffer = Convert.FromBase64String(tipo);
                    return File(buffer, "application/pdf", transferenciaAlunoBd.no_arquivo_historico);
                }
                else
                    throw new SecretariaBusinessException(Utils.Messages.Messages.msgAvisoNaoexisteLayoutNomeContrato, null, SecretariaBusinessException.TipoErro.ERRO_NAO_EXISTE_LAYOUT_NOME_CONTRATO, false);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Utils.Messages.Messages.msgAvisoNaoexisteLayoutNomeContrato))
                    logger.Warn(ex);
                else
                    logger.Error(ex);

                Session["Erro"] = FundacaoFisk.SGF.Utils.Messages.Messages.msgAvisoNaoexisteLayoutNomeContrato;
                Session["StackTrace"] = ex.Message + ex.StackTrace + ex.InnerException;
                return Redirect("~/Erro/Index");
            }
        }

        [HttpGet]
        [MvcComponentesAuthorize(Roles = "envtran")]
        [HttpComponentesAuthorize(Roles = "rectran")]
        public ActionResult getVisualizarArquivoHistoricoTransferenciaAluno(int cd_transferencia_aluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();

                TransferenciaAluno transferenciaAlunoBd = BusinessSecretaria.getTransferenciaAlunoById(cd_transferencia_aluno);



                if (transferenciaAlunoBd != null && !string.IsNullOrEmpty(transferenciaAlunoBd.no_arquivo_historico) && !String.IsNullOrEmpty(transferenciaAlunoBd.pdf_historico))
                {

                    string tipo = transferenciaAlunoBd.pdf_historico.Substring(transferenciaAlunoBd.pdf_historico.IndexOf(",") + 1);
                    Byte[] buffer = Convert.FromBase64String(tipo);
                    MemoryStream stream = new MemoryStream(buffer);

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "inline;filename=" + transferenciaAlunoBd.no_arquivo_historico + ".pdf");
                    Response.Buffer = true;
                    Response.Clear();
                    byte[] buf = stream.ToArray();
                    Response.OutputStream.Write(buf, 0, buf.Length);
                    Response.OutputStream.Flush();
                    Response.End();
                    stream.Dispose();
                    return this.Content(stream.ToArray().ToString(), "application/pdf");


                    
                }
                else
                    throw new SecretariaBusinessException(Utils.Messages.Messages.msgAvisoNaoexisteLayoutNomeContrato, null, SecretariaBusinessException.TipoErro.ERRO_NAO_EXISTE_LAYOUT_NOME_CONTRATO, false);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Utils.Messages.Messages.msgAvisoNaoexisteLayoutNomeContrato))
                    logger.Warn(ex);
                else
                    logger.Error(ex);

                Session["Erro"] = FundacaoFisk.SGF.Utils.Messages.Messages.msgAvisoNaoexisteLayoutNomeContrato;
                Session["StackTrace"] = ex.Message + ex.StackTrace + ex.InnerException;
                return Redirect("~/Erro/Index");
            }
        }

        

        #endregion

    }    

}
 

