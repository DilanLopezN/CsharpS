using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Componentes.GenericController;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using log4net;
using Newtonsoft.Json;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using System.Configuration;
using Componentes.GenericBusiness.Comum;

using FundacaoFisk.SGF.GenericModel;
using System.IO;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Controllers
{
    public class ProfessorController : ComponentesApiController
    {
        //Declaração de Atributos
        private static readonly ILog logger = LogManager.GetLogger(typeof(ProfessorController));

        //Método construtor
        public ProfessorController()
        {
        }

        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage getAllProfessor()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                var listProfessor = profBiz.getProfessorReturnProfUI(ProfessorDataAccess.TipoConsultaProfessorEnum.HAS_ATIVO, cdEscola, null);
                retorno.retorno = listProfessor;
                if (listProfessor.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage getAllProfessorTurma() {
            ReturnResult retorno = new ReturnResult();
            try {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                var listProfessor = profBiz.getProfessorReturnProfUI(ProfessorDataAccess.TipoConsultaProfessorEnum.HAS_TURMA, cdEscola, null);
                retorno.retorno = listProfessor;
                if(listProfessor.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage getAllProfessorTurmasemDiario(bool idEscola)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = idEscola ? 0 : this.ComponentesUser.CodEmpresa.Value;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                var listProfessor = profBiz.getProfessorReturnProfUI(ProfessorDataAccess.TipoConsultaProfessorEnum.SEM_DIARIO, cdEscola, null);
                retorno.retorno = listProfessor;
                if (listProfessor.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage getProfessorCargaHoraria(bool idEscola)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = idEscola ? 0 : this.ComponentesUser.CodEmpresa.Value;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                var listProfessor = profBiz.getProfessorReturnProfUI(ProfessorDataAccess.TipoConsultaProfessorEnum.CARGA_HORARIA, cdEscola, null);
                retorno.retorno = listProfessor;
                if (listProfessor.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage getProfessorTurma(int cd_turma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                var listProfessor = profBiz.getProfessorTurma(cdEscola, cd_turma);
                retorno.retorno = listProfessor;
                if (listProfessor.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage getProfessorTurmaLogado(int cd_turma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdusuario = (int)this.ComponentesUser.CodUsuario;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                var listProfessor = profBiz.getProfessorTurmaLogado(cdEscola, cd_turma, cdusuario);
                retorno.retorno = listProfessor;
                if (listProfessor.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage professoresDisponiveisFaixaHorarioTurma(TurmaAlunoProfessorHorario turmaProfessorHorario)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdTurma = turmaProfessorHorario.cd_turma;
                int[] professores = turmaProfessorHorario.professores;
                Horario horario = turmaProfessorHorario.horario;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                var professoresT = profBiz.getProfessoresDisponiveisFaixaHorarioTurma(turmaProfessorHorario, cdEscola);
                
                retorno.retorno = professoresT;
                if (professoresT == null)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FuncionarioBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }

        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage verificaAlunosEProfessoresDisponiveisFaixaHorario(TurmaAlunoProfessorHorario turmaAlunoProfHorario)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdTurma = turmaAlunoProfHorario.cd_turma;
                List<Aluno> alunos = turmaAlunoProfHorario.alunos;
                Horario horario = turmaAlunoProfHorario.horario;
                int[] professores = turmaAlunoProfHorario.professores;
                IEnumerable<Professor> professoresT = null;
            
                if(alunos != null && alunos.Count > 0)
                    alunoBiz.verificarAlunosDisponiveisFaixaHorario(cdTurma,null, cdEscola, horario, alunos);
                if (professores != null && professores.Count() > 0)
                    professoresT = profBiz.getProfessoresDisponiveisFaixaHorarioTurma(turmaAlunoProfHorario, cdEscola);

                return Request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (AlunoBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (FuncionarioBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }

        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage professoresDisponiveisFaixaHorarioTurmaFilhaPPT(HorariosTurmasHorariosProfessores turmaProfessorHorario)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = this.ComponentesUser.CodEmpresa.Value;
            try
            {
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                List<FuncionarioSearchUI> listProfessor = profBiz.professoresDisponiveisFaixaHorarioTurmaFilhaPPT(turmaProfessorHorario, cdEscola).ToList();
                retorno.retorno = listProfessor;
                if (listProfessor.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FuncionarioBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        } 
        
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage getFuncionarioByEscola()
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = this.ComponentesUser.CodEmpresa.Value;
            try
            {
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                List<FuncionarioSearchUI> funcionarios = profBiz.getProfessoresByEmpresa(cdEscola, null).ToList();
                retorno.retorno = funcionarios;
                if (funcionarios.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FuncionarioBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "func.e")]
        public HttpResponseMessage postDeleteFuncionario(List<FuncionarioSGF> funcionarios)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEmpresa = (int)ComponentesUser.CodEmpresa;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                bool delPessoa = profBiz.deleteFuncionarios(funcionarios, cdEmpresa);
                retorno.retorno = delPessoa;
                if (!delPessoa)
                {
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "func.i")]
        public HttpResponseMessage PostInsertFuncionario(FuncionarioUI funcionarioUI)
        {
            string fullPath = fullPath = ConfigurationManager.AppSettings["caminhoUploads"];
            string caminho = null;
            
            List<RelacionamentoSGF> relacionamentos = null;
            funcionarioUI.pessoaFisicaUI.pessoaFisica.dt_cadastramento = Utils.Utils.truncarMilissegundo(funcionarioUI.pessoaFisicaUI.pessoaFisica.dt_cadastramento.ToUniversalTime());
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(funcionarioUI.pessoaFisicaUI.pessoaFisica.no_pessoa) && funcionarioUI.pessoaFisicaUI.pessoaFisica.no_pessoa.IndexOf("*") > -1)
                {
                    throw new FuncionarioBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroNomeComAsterisco, null,
                        FuncionarioBusinessException.TipoErro.ERRO_NOME_COM_ASTERISCO, false);
                }

                int cdEscola = (int)ComponentesUser.CodEmpresa;
                string permissao = (string)ComponentesUser.Permissao;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { profBiz });
                string[] permissoes = permissao.Split('|');
                funcionarioUI.funcionario.dt_admissao = funcionarioUI.funcionario.dt_admissao.ToLocalTime().Date;
                funcionarioUI.funcionario.dt_demissao = funcionarioUI.funcionario.dt_demissao.HasValue ? funcionarioUI.funcionario.dt_demissao.Value.ToLocalTime().Date : funcionarioUI.funcionario.dt_demissao;

                if (!permissoes.Contains("salfunc") && funcionarioUI.funcionario.vl_salario != null && funcionarioUI.funcionario.vl_salario > 0)
                    throw new FuncionarioBusinessException(string.Format(Utils.Messages.Messages.msgNotPermissaoSalario), null, FuncionarioBusinessException.TipoErro.ERRO_PERMISSAO_SALARIO, false);

                funcionarioUI.pessoaFisicaUI.pessoaFisica.id_pessoa_empresa = false;
                if (funcionarioUI != null && !string.IsNullOrEmpty(funcionarioUI.pessoaFisicaUI.descFoto))
                {
                    caminho = fullPath + "/" + funcionarioUI.pessoaFisicaUI.descFoto;
                    funcionarioUI.pessoaFisicaUI.pessoaFisica.img_pessoa = ManipuladorArquivo.getPathPhoto(caminho);
                    funcionarioUI.pessoaFisicaUI.pessoaFisica.ext_img_pessoa = funcionarioUI.pessoaFisicaUI.descFoto;
                }

                if (funcionarioUI.pessoaFisicaUI.relacionamentosUI != null && funcionarioUI.pessoaFisicaUI.relacionamentosUI.Count() > 0)
                {
                    foreach (var item in funcionarioUI.pessoaFisicaUI.relacionamentosUI)
                    {
                        relacionamentos = new List<RelacionamentoSGF>();
                        RelacionamentoSGF relac = new RelacionamentoSGF();
                        relac = item.relacionamento;
                        if (item.nm_natureza_pessoa == 1)
                        {
                            if (item.pessoaFisicaRelac != null && !string.IsNullOrEmpty(item.pessoaFisicaRelac.no_pessoa) && item.pessoaFisicaRelac.nm_sexo > 0)
                            {
                                item.pessoaFisicaRelac.nm_natureza_pessoa = 1;
                                relac.PessoaFilho = item.pessoaFisicaRelac;
                                if (item.enderecoRelac != null && item.enderecoRelac.cd_loc_cidade > 0 && item.enderecoRelac.cd_loc_bairro > 0 && item.enderecoRelac.cd_loc_estado > 0 &&
                                item.enderecoRelac.cd_loc_logradouro > 0 && item.enderecoRelac.cd_tipo_logradouro > 0)
                                    relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            }
                            relacionamentos.Add(relac);
                        }
                        else
                        {
                            if (item.pessoaJuridicaRelac != null && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.no_pessoa) && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.dc_num_cgc))
                            {
                                item.pessoaJuridicaRelac.cd_tipo_sociedade = 1;
                                item.pessoaJuridicaRelac.nm_natureza_pessoa = 2;
                                relac.PessoaFilho = item.pessoaJuridicaRelac;
                                if (item.enderecoRelac != null && item.enderecoRelac.cd_loc_cidade > 0 && item.enderecoRelac.cd_loc_bairro > 0 && item.enderecoRelac.cd_loc_estado > 0 &&
                                item.enderecoRelac.cd_loc_logradouro > 0 && item.enderecoRelac.cd_tipo_logradouro > 0)
                                    relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            }
                            relacionamentos.Add(relac);
                        }

                    }
                }

                funcionarioUI.funcionario.cd_pessoa_empresa = cdEscola;

                var funcionario = profBiz.addFuncionario(funcionarioUI, relacionamentos, fullPath);
                retorno.retorno = funcionario;
                if (funcionario.cd_funcionario <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (FuncionarioBusinessException ex)
            {
                if (ex.tipoErro == FuncionarioBusinessException.TipoErro.ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA ||
                    ex.tipoErro == FuncionarioBusinessException.TipoErro.ERRO_FUNCIONARIO)
                {
                    return gerarLogException(ex.Message, retorno, logger, ex);
                }
                else
                {
                    return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
                }
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_CPFJAEXISTENTE || ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_AUTO_RELACIONAMENTO
                    || ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_MINDATE_SMALLDATETIME)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FuncionarioBusinessException fx = new FuncionarioBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }

        }

        [HttpComponentesAuthorize(Roles = "func.a")]
        public HttpResponseMessage PostUpdateFuncionario(FuncionarioUI funcionarioUI)
        {
            string fullPath = ConfigurationManager.AppSettings["caminhoUploads"];
            string caminho = null;
            List<RelacionamentoSGF> relacionamentos = null;
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(funcionarioUI.pessoaFisicaUI.pessoaFisica.no_pessoa) && funcionarioUI.pessoaFisicaUI.pessoaFisica.no_pessoa.IndexOf("*") > -1)
                {
                    throw new FuncionarioBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroNomeComAsterisco, null,
                        FuncionarioBusinessException.TipoErro.ERRO_NOME_COM_ASTERISCO, false);
                }

                int cdEscola = (int)ComponentesUser.CodEmpresa;
                string permissao = (string)ComponentesUser.Permissao;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { profBiz });
                funcionarioUI.pessoaFisicaUI.pessoaFisica.id_pessoa_empresa = false;

                funcionarioUI.funcionario.dt_admissao = funcionarioUI.funcionario.dt_admissao.ToLocalTime().Date;
                funcionarioUI.funcionario.dt_demissao = funcionarioUI.funcionario.dt_demissao.HasValue ? funcionarioUI.funcionario.dt_demissao.Value.ToLocalTime().Date : funcionarioUI.funcionario.dt_demissao;

                if (funcionarioUI != null && !string.IsNullOrEmpty(funcionarioUI.pessoaFisicaUI.descFoto))
                {
                    caminho = fullPath + "/" + funcionarioUI.pessoaFisicaUI.descFoto;
                    funcionarioUI.pessoaFisicaUI.pessoaFisica.img_pessoa = ManipuladorArquivo.getPathPhoto(caminho);
                    funcionarioUI.pessoaFisicaUI.pessoaFisica.ext_img_pessoa = funcionarioUI.pessoaFisicaUI.descFoto;
                }

                if (funcionarioUI.pessoaFisicaUI.relacionamentosUI != null && funcionarioUI.pessoaFisicaUI.relacionamentosUI.Count() > 0)
                {
                    foreach (var item in funcionarioUI.pessoaFisicaUI.relacionamentosUI)
                    {
                        relacionamentos = new List<RelacionamentoSGF>();
                        RelacionamentoSGF relac = new RelacionamentoSGF();
                        relac = item.relacionamento;
                        if (item.nm_natureza_pessoa == 1)
                        {
                            if (item.pessoaFisicaRelac != null && !string.IsNullOrEmpty(item.pessoaFisicaRelac.no_pessoa) && item.pessoaFisicaRelac.nm_sexo > 0)
                            {
                                item.pessoaFisicaRelac.nm_natureza_pessoa = 1;
                                relac.PessoaFilho = item.pessoaFisicaRelac;
                                if (item.enderecoRelac != null && item.enderecoRelac.cd_loc_cidade > 0 && item.enderecoRelac.cd_loc_bairro > 0 && item.enderecoRelac.cd_loc_estado > 0 &&
                                item.enderecoRelac.cd_loc_logradouro > 0 && item.enderecoRelac.cd_tipo_logradouro > 0)
                                    relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            }
                            relacionamentos.Add(relac);
                        }
                        else
                        {
                            if (item.pessoaJuridicaRelac != null && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.no_pessoa) && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.dc_num_cgc))
                            {
                                item.pessoaJuridicaRelac.cd_tipo_sociedade = 1;
                                item.pessoaJuridicaRelac.nm_natureza_pessoa = 2;
                                relac.PessoaFilho = item.pessoaJuridicaRelac;
                                if (item.enderecoRelac != null && item.enderecoRelac.cd_loc_cidade > 0 && item.enderecoRelac.cd_loc_bairro > 0 && item.enderecoRelac.cd_loc_estado > 0 &&
                                item.enderecoRelac.cd_loc_logradouro > 0 && item.enderecoRelac.cd_tipo_logradouro > 0)
                                    relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            }
                            relacionamentos.Add(relac);
                        }

                    }
                }

                foreach (var funcComiss in funcionarioUI.funcionario.FuncionarioComissao)
                {
                    funcComiss.cd_funcionario = funcionarioUI.funcionario.cd_funcionario;
                }

                funcionarioUI.funcionario.cd_pessoa_empresa = cdEscola;
                FuncionarioSGF func = new FuncionarioSGF();
                PessoaFisicaSGF pessoaFisica = new PessoaFisicaSGF();
                bool permissaoSalario = permissao.Contains("salfunc") ? true : false;

                var funcionario = profBiz.editFuncionario(funcionarioUI, relacionamentos, permissaoSalario, fullPath);
                retorno.retorno = funcionario;
                //if (insertPessoaFisica.cd_pessoa <= 0)
                if (funcionario.cd_pessoa_funcionario <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (FuncionarioBusinessException ex)
            {
                if (ex.tipoErro == FuncionarioBusinessException.TipoErro.ERRO_TRIGGER_FUNCIONARIO_PROFESSOR)
                {
                    return gerarLogException(ex.Message, retorno, logger, ex);
                }

                if (ex.tipoErro == FuncionarioBusinessException.TipoErro.ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA ||
                    ex.tipoErro == FuncionarioBusinessException.TipoErro.ERRO_FUNCIONARIO)
                {
                    return gerarLogException(ex.Message, retorno, logger, ex);
                }
                else
                {
                    return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
                }
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_CPFJAEXISTENTE || ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_AUTO_RELACIONAMENTO
                    || ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_MINDATE_SMALLDATETIME)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage getDataFuncionario(int cdFunc, bool prof)
        {
            ReturnResult retorno = new ReturnResult();
            Professor professor = new Professor();
            try
            {
                int cdEscola = (int)ComponentesUser.CodEmpresa;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                if (prof)
                    professor = profBiz.getFuncionarioEditById(cdFunc, cdEscola, ProfessorDataAccess.TipoConsultaProfessorEnum.DADOS_PROF_EDIT);
                else
                    professor = profBiz.getFuncionarioEditById(cdFunc, cdEscola, ProfessorDataAccess.TipoConsultaProfessorEnum.DADOS_FUNC_EDIT);

                List<ProdutoFuncionario> produtosFuncionario = profBiz.getProdutoFuncionarioByFuncionario(cdFunc, cdEscola);

                var get = FuncionarioProfessorSearchUI.changeValueFuncionaioToFuncionarioProfessorSearchUI(professor, produtosFuncionario);
                string permissao = (string)ComponentesUser.Permissao;
                string[] permissoes = permissao.Split('|');
                if (!permissoes.Contains("salfunc"))
                    get.funcionarioSGF.vl_salario = null;

                retorno.retorno = get;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage geHorarioByEscolaForProfessor(int cdFunc)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                //to do Deivid
                int[] cdProf = new int[1];
                cdProf[0] = cdFunc;
                int cdEscola = (int)ComponentesUser.CodEmpresa;
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                DateTime dt_inicio = new DateTime(2000, 01, 01); //Vai analisar período de 01/01/200 a 01/01/2050 pois não temos aqui dados da turma
                DateTime? dt_final = new DateTime(2050, 01, 01);
                var horarios = secretariaBiz.getHorarioByEscolaForRegistro(cdEscola, cdFunc, Horario.Origem.PROFESSOR);
                List<Horario> horariosOcupados = secretariaBiz.getHorarioOcupadosForTurma(cdEscola, cdFunc, cdProf, 0, 0, 0, dt_inicio, dt_final, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.HorarioDataAccess.TipoConsultaHorario.HAS_HORARIO_PROF_OCUPADO_TURMA).ToList();
                foreach (Horario h in horarios)
                    h.calendar = "Calendar1";
                foreach (Horario h in horariosOcupados)
                    h.calendar = "Calendar2";

                horarios = horarios.Union(horariosOcupados);


                retorno.retorno = horarios;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // GET api/<controller>/5 
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage GetFuncionarioSearch(string nome, string apelido, int status, string cpf, bool inicio, byte tipo, int sexo, int cdAtividade, int coordenador, int coolaborador_cyber)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)ComponentesUser.CodEmpresa;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cpf == null)
                    cpf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var ret = profBiz.getSearchFuncionario(parametros, nome, apelido, getStatus(status), cpf, inicio, tipo, cdEscola, sexo, cdAtividade, coordenador, coolaborador_cyber);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, ret);
                configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage GetUrlRelatorioFuncionario(string sort, int direction, string nome, string apelido, int status, string cpf, bool inicio, byte tipo, int sexo, int cdAtividade,
            int coordenador, int coolaborador_cyber)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)ComponentesUser.CodEmpresa;
            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@nome=" + nome + "&@apelido=" + apelido + "&@status=" + status + "&@cpf=" + cpf + "&@inicio=" + inicio + "&@tipo=" + tipo + "&@cdEscola=" + cdEscola +
                    "&@sexo=" + sexo + "&@cdAtividade=" + cdAtividade + "&@coordenador=" + coordenador + "&@colaboradorCyber=" + coolaborador_cyber +
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Funcionário&" +
                    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.FuncionarioSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                retorno.retorno = parametrosCript;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage getProfessorByEscola()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)ComponentesUser.CodEmpresa;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                var funcionarios = profBiz.getFuncionariosByEscola(cdEscola, 0, null);
                retorno.retorno = funcionarios;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage ExistFuncionarioOrPessoaFisicaByCpf(string cpf)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)ComponentesUser.CodEmpresa;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                var pessoaFisicaUI = profBiz.ExistFuncionarioOrPessoaFisicaByCpf(cpf, cdEscola);
                if (pessoaFisicaUI != null && pessoaFisicaUI.pessoaFisica != null)
                {
                    if (pessoaFisicaUI.pessoaFisica.TelefonePessoa.Count() > 0)
                    {
                        pessoaFisicaUI.contatosUI = TelefoneUI.fromTelefoneforTelefoneUI(pessoaFisicaUI.pessoaFisica.TelefonePessoa);
                        pessoaFisicaUI.pessoaFisica.TelefonePessoa = null;
                    }
                    if (pessoaFisicaUI.pessoaFisica.Telefone != null)
                    {
                        pessoaFisicaUI.pessoaFisica.Telefone.ClasseTelefone.TelefoneClasse = null;
                        pessoaFisicaUI.pessoaFisica.Telefone.TelefonePessoa = null;
                        pessoaFisicaUI.pessoaFisica.Telefone.TelefoneTipo.TipoTelefoneRef = null;
                        pessoaFisicaUI.pessoaFisica.Telefone.TelefonePessoa = null;
                    }
                    //if (pessoaFisicaUI.pessoaFisica.PessoaEndereco.Count() > 0)
                    //    pessoaFisicaUI.enderecosUI = EnderecoUI.fromEnderecoforEnderecoUI(pessoaFisicaUI.pessoaFisica.PessoaEndereco, pessoaFisicaUI.pessoaFisica.cd_endereco_principal);
                    if (pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento.Count() > 0)
                    {
                        pessoaFisicaUI.relacionamentoUI = RelacionamentoUI.fromRelacionamentoforRelacionamentoUI(pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento, pessoaFisicaUI.pessoaFisica.cd_pessoa);
                        pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento = null;
                    }
                    retorno.retorno = pessoaFisicaUI;
                    retorno.AddMensagem(string.Format(Messages.msgFuncExistReturnData, pessoaFisicaUI.pessoaFisica.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (FuncionarioBusinessException ex)
            {
                if (ex.tipoErro == FuncionarioBusinessException.TipoErro.ERRO_FUNCIONARIOJAEXISTE)
                {
                    retorno.AddMensagem(ex.Message, ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                }
                else
                {
                    retorno.AddMensagem(Messages.msgRegBuscError, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                }
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "func.i,func.a")]
        public HttpResponseMessage uploadCertificadoProfessor()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];
                if (httpPostedFile == null)
                    throw new PessoaBusinessException(Messages.msgImagemInvalida, null, PessoaBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                var fileUpload = httpPostedFile;
                //limite  500kbytes
                string extensaoArq = fileUpload.FileName.Substring(fileUpload.FileName.Length - 4);
                if (fileUpload.ContentLength > 1000000)
                    throw new PessoaBusinessException(Messages.msgErroImagemExcedeuLimte, null, PessoaBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                if (extensaoArq.ToLower() != ".jpg" && extensaoArq.ToLower() != "jpeg" && extensaoArq.ToLower() != ".gif" && extensaoArq.ToLower() != ".png" &&
                    extensaoArq.ToLower() != ".gif" && extensaoArq.ToLower() != ".bmp")
                    throw new PessoaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtensaoImagemNaoSuportada, null, PessoaBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                //Local onde vai ficar as fotos enviadas.
                var uploadPath = ConfigurationManager.AppSettings["caminhoUploads"];
                string file_name = Utils.Utils.geradorNomeAleatorio(36);
                //var enderecoWeb = ConfigurationManager.AppSettings["enderecoWeb"];
                //var serverUploadPath = uploadPath;
                //Faz um checagem se o arquivo veio correto.
                if (extensaoArq.ToLower() == "jpeg")
                    file_name += ".";
                file_name += extensaoArq;

                var uploadedFilePath = Path.Combine(uploadPath, file_name);

                //faz o upload literalmetne do arquivo.
                byte[] buffer;
                FileStream fs;
                using (fs = new FileStream(uploadedFilePath, FileMode.Create))
                {
                    buffer = new byte[fileUpload.InputStream.Length];
                    fileUpload.InputStream.Read(buffer, 0, buffer.Length);
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Dispose();
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, file_name);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUploadImage, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage GetProfessorsemDiario(int cd_turma, int cd_professor, bool idEscola, bool idLiberado)
        {
            try
            {
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                int cdEscola = idEscola ? 0 : this.ComponentesUser.CodEmpresa.Value;
                var professores = profBiz.getProfessorsemDiario(parametros, cd_turma, cd_professor, cdEscola, idLiberado);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, professores);
                base.configureHeaderResponse(response, parametros);
                return response;


            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage PostLiberarProfessor(ProfsemDiarioUI linha)
        {
            ReturnResult retorno = new ReturnResult();
            IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
            try
            {
                int cd_usuario = (int)this.ComponentesUser.CodUsuario;
                int fusoHorario = this.ComponentesUser.IdFusoHorario;

                string ret = profBiz.postLiberarProfessor(linha.cd_professor, cd_usuario, fusoHorario);

                if (ret != null)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ret));
                    return response;
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;

                return gerarLogException(msg, retorno, logger, ex);
            }
        }
    }
}