using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericController;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using log4net;
using Newtonsoft.Json;


namespace FundacaoFisk.SGF.Services.Secretaria.Controllers
{
    //[RoutePrefix("Secretaria")]
    public class AlunoController : ComponentesApiController
    {
        //Declaração de Atributos
        private static readonly ILog logger = LogManager.GetLogger(typeof(AlunoController));

        //Método construtor
        public AlunoController()
        {
        }

        #region Aluno

        [HttpComponentesAuthorize(Roles = "meda")]
        public HttpResponseMessage getUrlReporMediasAlunos(int cd_turma, int tipoTurma, bool turmasFilhas, int cdCurso, int cdProduto, int pesOpcao, int pesTipoAluno,
            decimal? vl_media, string dtInicial, string dtFinal, string no_curso, string no_produto, string no_turma, string noTipoTurma, string noPesOpcao, bool mostrarAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                
                string parametros = "@cd_turma=" + cd_turma + "&@tipoTurma=" + tipoTurma + "&@turmasFilhas=" + turmasFilhas 
                    + "&@cdCurso=" + cdCurso + "&@cdProduto=" + cdProduto
                    + "&@pesOpcao=" + pesOpcao 
                    + "&@pesTipoAluno=" + pesTipoAluno
                    + "&@cd_escola=" + cdEscola
                    + "&vl_media=" + vl_media + "&no_produto=" + no_produto + "&no_curso=" + no_curso
                    + "&no_turma=" + no_turma + "&noTipoTurma=" + noTipoTurma + "&noPesOpcao=" + noPesOpcao 
                    + "&@dtInicial=" + dtInicial + "&@dtFinal=" + dtFinal + "&mostrarAvaliacao=" + mostrarAvaliacao;

                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu.e")]
        public HttpResponseMessage PostdeleteAluno(List<Aluno> alunos)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = this.ComponentesUser.CodEmpresa.Value;
            try
            {
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                base.configuraBusiness(new List<IGenericBusiness>() { secretariaBiz });

                var deletado = secretariaBiz.deletarAlunos(alunos, cdEscola);
                retorno.retorno = deletado;
                if (!deletado)
                {
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage GetAlunoPolitica()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var aluno = alunoBiz.getAlunoPolitica(cdEscola);
                retorno.retorno = aluno;
                if (aluno.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

         [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getAlunoTurma(int cdAluno, int cdTurma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var aluno = alunoBiz.findAlunoTurma(cdAluno, cdTurma, cdEscola);
                if(aluno != null && aluno.Turma != null)
                    aluno.Turma = null;
                retorno.retorno = aluno;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        
        [HttpComponentesAuthorize(Roles = "poldes")]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getAlunoSelecionado(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var alunosPol = alunoBiz.getAlunoSelecionado(id, cdEscola);
                retorno.retorno = alunosPol;
                if (alunosPol.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getAlunoPorTurma(int cdTurma, int opcao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var aluno = alunoBiz.getAlunoPorTurma(cdTurma, cdEscola, opcao).ToList();

                if (aluno != null)
                    foreach (Aluno a in aluno)
                        a.AlunoPessoaFisica = null;

                retorno.retorno = aluno;
                if (aluno.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getAlunoPorTurmaSearch(string nome, string email, bool inicio, int origemFK, bool? status, string cpf, int cdSituacao, int sexo,
                                                        int cdTurma, int opcao)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;
                if (email == null)
                    email = String.Empty;
                if (cpf == null)
                    cpf = String.Empty;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getAlunoPorTurmaSearch(parametros, nome, email, inicio, origemFK, status, cdEscola, cpf, cdSituacao, sexo, cdTurma, opcao);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getAlunoPorTurmaSearchAulaReposicao(string nome, string email, bool inicio, int origemFK, bool? status, string cpf, int cdSituacao, int sexo,
            int cdTurma, int opcao)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;
                if (email == null)
                    email = String.Empty;
                if (cpf == null)
                    cpf = String.Empty;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getAlunoPorTurmaSearchAulaReposicao(parametros, nome, email, inicio, origemFK, status, cdEscola, cpf, cdSituacao, sexo, cdTurma, opcao);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getAlunoPorTurmaControleFaltaSearch(string cdAlunos, string nome, string email, bool inicio, bool? status, string cpf, int cdSituacao, int sexo,
            int cdTurma, int opcao, DateTime? dataFinalHistorico)
        {
            try
            {
                var alunos = new List<int>();
                if (!string.IsNullOrEmpty(cdAlunos))
                {
                    string[] listEmpresas = cdAlunos.Split(',');
                    foreach (var cd_aluno in listEmpresas)
                        if (!string.IsNullOrEmpty(cd_aluno))
                            alunos.Add(int.Parse(cd_aluno));
                }


                if (nome == null)
                    nome = String.Empty;
                if (email == null)
                    email = String.Empty;
                if (cpf == null)
                    cpf = String.Empty;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getAlunoPorTurmaControleFaltaSearch(parametros, alunos, nome, email, inicio, status, cdEscola, cpf, cdSituacao, sexo, cdTurma, opcao, dataFinalHistorico);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getAlunoPorTurmaPPTFilha(int cdTurma, int cdTurmaPai, int opcao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var aluno = alunoBiz.getAlunoPorTurmaPPTFilha(cdEscola, cdTurma, cdTurmaPai, opcao);
                retorno.retorno = aluno;
                if (aluno.cd_aluno <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getAlunoFKReajusteSearch(string nome, string email, int status, string cnpjCpf, bool inicio, string cdSituacoes, int sexo, bool semTurma, bool movido, int tipoAluno, string dataInicial, string dataFinal)
        {
            try {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                string[] situacao = cdSituacoes.Split('|');
                List<int> cdsSituacoes = new List<int>();
                for (int i = 0; i < situacao.Count(); i++)
                    cdsSituacoes.Add(Int32.Parse(situacao[i]));

                DateTime? dt_inicial = string.IsNullOrEmpty(dataInicial) ? null : (DateTime?)Convert.ToDateTime(dataInicial);
                DateTime? dt_final = string.IsNullOrEmpty(dataFinal) ? null : (DateTime?)Convert.ToDateTime(dataFinal);
                if (nome == null)
                    nome = String.Empty;
                if(email == null)
                    email = String.Empty;
                if(cnpjCpf == null)
                    cnpjCpf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getAlunoFKReajusteSearch(parametros, nome, email, inicio, getStatus(status), cdEscola, cnpjCpf, cdsSituacoes, sexo, semTurma, movido, tipoAluno, dt_inicial, dt_final);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage GetAlunoSearch(string nome, string email, int status, string cnpjCpf, bool inicio, string cdSituacoes, int sexo, bool semTurma, bool movido, int tipoAluno, bool matriculasem, bool matricula)
        {
            try {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                string[] situacao = cdSituacoes.Split('|');
                List<int> cdsSituacoes = new List<int>();
                for (int i = 0; i < situacao.Count(); i++)
                    cdsSituacoes.Add(Int32.Parse(situacao[i]));

                if(nome == null)
                    nome = String.Empty;
                if(email == null)
                    email = String.Empty;
                if(cnpjCpf == null)
                    cnpjCpf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getAlunoSearch(parametros, nome, email, inicio, getStatus(status), cdEscola, cnpjCpf, cdsSituacoes, sexo, semTurma, movido, tipoAluno, matriculasem, matricula).ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getAlunoSearchFKPesquisasAtividadeExtra(string nome, string email, int status, string cnpjCpf, bool inicio,
            string cdSituacoes, int sexo, bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel, int cd_escola_combo_fk, string cursos, string cdEscolas)
        {
            try
            {
                List<int> cds_escolas_atividade = new List<int>();
                if (cdEscolas != null)
                {
                    string[] escolas = cdEscolas.Split('|');
                    for (int i = 0; i < escolas.Count(); i++)
                        cds_escolas_atividade.Add(Int32.Parse(escolas[i]));
                }

                List<int> listaCursos = new List<int>();
                if (!string.IsNullOrEmpty(cursos))
                {
                     listaCursos = cursos.Split(',').Select(i => Int32.Parse(i.ToString())).ToList();
                }
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                string[] situacao = cdSituacoes.Split('|');
                List<int> cdsSituacoes = new List<int>();
                for (int i = 0; i < situacao.Count(); i++)
                    cdsSituacoes.Add(Int32.Parse(situacao[i]));

                if (nome == null)
                    nome = String.Empty;
                if (email == null)
                    email = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getAlunoSearchFKPesquisasAtividadeExtra(parametros, nome, email, inicio, getStatus(status), cdEscola,
                    cnpjCpf, cdsSituacoes, sexo, semTurma, movido, tipoAluno, cd_pessoa_responsavel, AlunoDataAccess.TipoConsultaAlunoEnum.ALUNO, cd_escola_combo_fk, listaCursos, cds_escolas_atividade).ToList(); 
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }



        [HttpGet]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getAlunoSearchFKPesquisas(string nome, string email, int status, string cnpjCpf, bool inicio, int origemFK,
            string cdSituacoes, int sexo, bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                string[] situacao = cdSituacoes.Split('|');
                List<int> cdsSituacoes = new List<int>();
                for (int i = 0; i < situacao.Count(); i++)
                    cdsSituacoes.Add(Int32.Parse(situacao[i]));

                if (nome == null)
                    nome = String.Empty;
                if (email == null)
                    email = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getAlunoSearchFKPesquisas(parametros, nome, email, inicio, getStatus(status), cdEscola, origemFK,
                    cnpjCpf, cdsSituacoes, sexo, semTurma, movido, tipoAluno, cd_pessoa_responsavel, AlunoDataAccess.TipoConsultaAlunoEnum.ALUNO).ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getAlunoSearchFKPesquisasAtividade(string nome, string email, int status, string cnpjCpf, bool inicio, int origemFK,
            string cdSituacoes, int sexo, bool semTurma, bool movido, int tipoAluno, int cd_pessoa_responsavel, int cd_escola_combo_fk, string cdEscolas)
        {
            try
            {
                List<int> cds_escolas_atividade = new List<int>();
                if (cdEscolas != null)
                {
                    string[] escolas = cdEscolas.Split('|');
                    for (int i = 0; i < escolas.Count(); i++)
                        cds_escolas_atividade.Add(Int32.Parse(escolas[i]));
                }
                

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                string[] situacao = cdSituacoes.Split('|');
                List<int> cdsSituacoes = new List<int>();
                for (int i = 0; i < situacao.Count(); i++)
                    cdsSituacoes.Add(Int32.Parse(situacao[i]));

                if (nome == null)
                    nome = String.Empty;
                if (email == null)
                    email = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getAlunoSearchFKPesquisasAtividade(parametros, nome, email, inicio, getStatus(status), cdEscola, origemFK,
                    cnpjCpf, cdsSituacoes, sexo, semTurma, movido, tipoAluno, cd_pessoa_responsavel, AlunoDataAccess.TipoConsultaAlunoEnum.ALUNO, cd_escola_combo_fk, cds_escolas_atividade).ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getAlunoOrigemMaterial(string nome, string email, int status, string cnpjCpf,
            bool inicio, string cdSituacoes, int sexo, bool semTurma, bool movido, int tipoAluno)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                string[] situacao = cdSituacoes.Split('|');
                List<int> cdsSituacoes = new List<int>();
                for (int i = 0; i < situacao.Count(); i++)
                    cdsSituacoes.Add(Int32.Parse(situacao[i]));

                if (nome == null)
                    nome = String.Empty;
                if (email == null)
                    email = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getAlunoSearchFKPesquisas(parametros, nome, email, inicio, getStatus(status), cdEscola, 0,
                    cnpjCpf, cdsSituacoes, sexo, semTurma, movido, tipoAluno, 0, AlunoDataAccess.TipoConsultaAlunoEnum.HAS_PESQ_NOTA_MATERIAL).ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getAlunoOrigemVendaMaterial(string nome, string email, int status, string cnpjCpf,
            bool inicio, string cdSituacoes, int sexo, bool semTurma, bool movido, int tipoAluno)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                string[] situacao = cdSituacoes.Split('|');
                List<int> cdsSituacoes = new List<int>();
                for (int i = 0; i < situacao.Count(); i++)
                    cdsSituacoes.Add(Int32.Parse(situacao[i]));

                if (nome == null)
                    nome = String.Empty;
                if (email == null)
                    email = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getAlunoSearchFKPesquisas(parametros, nome, email, inicio, getStatus(status), cdEscola, 0,
                    cnpjCpf, cdsSituacoes, sexo, semTurma, movido, tipoAluno, 0, AlunoDataAccess.TipoConsultaAlunoEnum.VENDA_MATERIAL).ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage GetUrlRptContVendasMaterial(int cd_aluno, int cd_item, string dt_inicial, string dt_final, int cd_turma, bool semmaterial, string pNoTurma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                //Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@dt_inicial=" + dt_inicial + "&@dt_final=" + dt_final + "&@cd_aluno=" + cd_aluno + "&@cd_escola=" + cd_escola + "&@cd_item=" + cd_item + "&@cd_turma=" + cd_turma + "&@semmaterial=" + semmaterial + "&@pNoTurma=" + pNoTurma ;
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "rptaltige")]
        public HttpResponseMessage GetUrlRptAlunosSemTituloGerado([FromUri] AlunosSemTituloGeradoParamsUI alunosSemTituloGeradoParams)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                System.Globalization.CultureInfo ptBRCulture = new System.Globalization.CultureInfo("pt-BR");


                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                StringBuilder parametros = new StringBuilder();
                parametros.Append("@cd_escola=").Append(cdEscola)
                    .Append("&@no_mes=").Append(alunosSemTituloGeradoParams.no_mes)
                    .Append("&@vl_mes=").Append(alunosSemTituloGeradoParams.vl_mes)
                    .Append("&@ano=").Append(alunosSemTituloGeradoParams.ano)
                    .Append("&@cd_turma=").Append(alunosSemTituloGeradoParams.cd_turma != null ? alunosSemTituloGeradoParams.cd_turma : 0)
                    .Append("&@situacoes=").Append(alunosSemTituloGeradoParams.situacoes)
                    .Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO).Append("=Relatório de Alunos que não possuem títulos gerados");
                          //.Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO).Append("=").Append(FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.rtpAlunosSemTituloGerado);

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros.ToString(), System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage GetAlunoSearchAulaPer(string nome, string email, int status, string cnpjCpf, bool inicio, int cdSituacao, int sexo, bool semTurma, bool movido, int tipoAluno, string dtaAula)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                if (nome == null)
                    nome = String.Empty;
                if (email == null)
                    email = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                DateTime dataAula = Convert.ToDateTime(dtaAula);
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.GetAlunoSearchAulaPer(parametros, nome, email, inicio, getStatus(status), cdEscola, cnpjCpf, cdSituacao, sexo, semTurma, movido, tipoAluno, dataAula).ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        //[HttpGet]
        //[HttpComponentesAuthorize(Roles = "alu")]
        //public HttpResponseMessage GetAlunoSearchMovimento(string nome, string email, int status, string cnpjCpf, bool inicio, int sexo, int cd_)
        //{
        //    try
        //    {
        //        int cdEscola = this.ComponentesUser.CodEmpresa.Value;
        //        if (nome == null)
        //            nome = String.Empty;
        //        if (email == null)
        //            email = String.Empty;
        //        if (cnpjCpf == null)
        //            cnpjCpf = String.Empty;
        //        var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
        //        IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
        //        var retorno = alunoBiz.getAlunoSearch(parametros, nome, email, inicio, getStatus(status), cdEscola, cnpjCpf, cdsSituacoes, sexo,).ToList();
        //        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
        //        base.configureHeaderResponse(response, parametros);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
        //    }
        //}

        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage GetUrlRelatorioAluno(string sort, int direction, string nome, string email, int status, string cnpjCpf, bool inicio, string cdSituacoes, int sexo, bool semTurma, bool movido, int tipoAluno, bool matriculasem)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = this.ComponentesUser.CodEmpresa.Value;   
            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string PTitulo = "Relatório de Aluno/Cliente";
                string[] situacao = cdSituacoes.Split('|');
                if (situacao[0] != "100")
                    PTitulo = PTitulo + "    Situação: ";
                for (int i = 0; i < situacao.Count(); i++)
                {
                    if (i > 0)
                        PTitulo = PTitulo + ',';
                    string dc_situacao = AlunoTurma.getSituacaoAlunoTurma(Int32.Parse(situacao[i]));
                    PTitulo = PTitulo + " " + dc_situacao;
                }
                
                string parametros = strParametrosSort + "@nome=" + nome + "&@email=" + email + "&@status=" + status + "&@cnpjCpf=" + cnpjCpf + "&@inicio=" + inicio + "&@cdSituacoes=" + cdSituacoes + "&@cdEscola=" + cdEscola + "&@sexo=" + sexo + "&@semTurma=" + semTurma + "&@movido=" + movido + "&@tipoAluno=" + tipoAluno + "&@matriculasem=" + matriculasem + "&" +
                    Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + PTitulo + "&" +
                    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.AlunoSearch;
                var parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = null;
                response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage GetUrlRelatorioAlunosCartaQuitacao(string sort, int direction, int ano, int cdPessoa)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = this.ComponentesUser.CodEmpresa.Value;
            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                

                string parametros = strParametrosSort + "@ano=" + ano + "&@cd_pessoa=" + cdPessoa  +"&@cdEscola=" + cdEscola + "&" +
                    Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Listagem de alunos (Carta Quitação)" + "&" +
                    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.AlunosCartaQuitacao;
                var parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = null;
                response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getAlunoSearchTurma(string nome, string email, int status, string cnpjCpf, bool inicio, int origemFK, int cdSituacao, int sexo)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                if (nome == null)
                    nome = String.Empty;
                if (email == null)
                    email = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();

                IEnumerable<AlunoSearchUI> ret = alunoBiz.getAlunoSearchTurma(parametros, nome, email, inicio, origemFK, getStatus(status), cdEscola, cnpjCpf, cdSituacao, sexo);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, ret);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage getParametroEscolaInternacional()
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();

                bool ret = alunoBiz.getParametroEscolaInternacional(cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, new {escolaInternacional = ret});
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu.i")]
        public HttpResponseMessage PostInsertAluno(AlunoUI alunoUI)
        {
            string fullPath = null;
            string caminho = null;
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(alunoUI.pessoaFisicaUI.pessoaFisica.no_pessoa) && alunoUI.pessoaFisicaUI.pessoaFisica.no_pessoa.IndexOf("*") > -1)
                {
                    throw new AlunoBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroNomeComAsterisco, null,
                        AlunoBusinessException.TipoErro.ERRO_NOME_COM_ASTERISCO, false);
                }
                if (!string.IsNullOrEmpty(alunoUI.pessoaFisicaUI.email) && !Utils.Utils.validarEmail(alunoUI.pessoaFisicaUI.email))
                    throw new AlunoBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroEmailInvalido, null,
                       AlunoBusinessException.TipoErro.ERRO_EMAIL_INVALIDO, false);

                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { secretariaBiz });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cdUsuarioAtend = (int)this.ComponentesUser.CodUsuario;
                // O horário já vem com o UTC do dojo de Brasília, não precisa passar novamente:
                alunoUI.pessoaFisicaUI.pessoaFisica.dt_cadastramento = Utils.Utils.truncarMilissegundo(alunoUI.pessoaFisicaUI.pessoaFisica.dt_cadastramento.ToUniversalTime());
                alunoUI.cdUsuarioFollowUp = cdUsuarioAtend;
                alunoUI.pessoaFisicaUI.pessoaFisica.id_pessoa_empresa = false;
                alunoUI.aluno.cd_usuario_atendente = cdUsuarioAtend;

                // Vincular endereço do aluno para relacionamentos do tipo PAIS.
                if (alunoUI.pessoaFisicaUI.relacionamentosUI != null)
                {
                    foreach (var relacionamento in alunoUI.pessoaFisicaUI.relacionamentosUI)
                    {
                        // RELACIONAMENTO PAIS
                        if (relacionamento.no_papel.ToLower().Equals("pais"))
                        {
                            relacionamento.enderecoRelac = montarEnderecoPrincipalPai(alunoUI.pessoaFisicaUI.endereco);
                        }
                    }
                }

                if (alunoUI.pessoaFisicaUI.pessoaFisica != null && !string.IsNullOrEmpty(alunoUI.pessoaFisicaUI.descFoto))
                {
                    fullPath = ConfigurationManager.AppSettings["caminhoUploads"];
                    caminho = fullPath + "/" + alunoUI.pessoaFisicaUI.descFoto;
                    alunoUI.pessoaFisicaUI.pessoaFisica.img_pessoa = ManipuladorArquivo.getPathPhoto(caminho);
                    alunoUI.pessoaFisicaUI.pessoaFisica.ext_img_pessoa = alunoUI.pessoaFisicaUI.descFoto;
                }
                alunoUI.aluno.cd_pessoa_escola = cdEscola;
                alunoUI.pessoaFisicaUI.endereco.cd_endereco = 0;
                alunoUI.aluno.cd_aluno = 0;
                
                if (alunoUI.followUpUI != null)
                {
                    foreach (var followUp in alunoUI.followUpUI)
                    {
                        followUp.cd_follow_up = 0;
                    }
                }
                

                //Tratamento do fuso horário da data:
                alunoUI.aluno.Bolsa.dt_inicio_bolsa = SGF.Utils.ConversorUTC.Date(alunoUI.aluno.Bolsa.dt_inicio_bolsa, 
                    this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);

                //evitar o erro msgPessoaAutoRelacionamento de forma errada
                if (alunoUI.pessoaFisicaUI.pessoaFisica != null && alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf != null && alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf <= 0)
                {
                    alunoUI.pessoaFisicaUI.pessoaFisica.cd_pessoa_cpf = null;
                }

                AlunoSearchUI aluno = secretariaBiz.addAluno(alunoUI);
                
                aluno.telefone = alunoUI.pessoaFisicaUI.telefone;
                aluno.email = alunoUI.pessoaFisicaUI.email;
                retorno.retorno = aluno;
                if (aluno.cd_aluno <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (AlunoBusinessException ex)
            {
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_EMAIL_JA_EXITE)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    PessoaBusinessException fx = new PessoaBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                    return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_CPFJAEXISTENTE || ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_AUTO_RELACIONAMENTO
                    || ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_MINDATE_SMALLDATETIME)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    PessoaBusinessException fx = new PessoaBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    AlunoBusinessException fx = new AlunoBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                   return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu.a")]
        public HttpResponseMessage PostUpdateAluno(AlunoUI alunoUI)
        {
            string fullPath = null;
            string caminho = null;
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                int login = this.ComponentesUser.CodUsuario;
                bool isMaster = BusinessEmpresa.VerificarMasterGeral(login);
                alunoUI.isMaster = isMaster;
                if (!string.IsNullOrEmpty(alunoUI.pessoaFisicaUI.pessoaFisica.no_pessoa) && alunoUI.pessoaFisicaUI.pessoaFisica.no_pessoa.IndexOf("*") > -1)
                {
                    throw new AlunoBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroNomeComAsterisco, null,
                        AlunoBusinessException.TipoErro.ERRO_NOME_COM_ASTERISCO, false);
                }

                if (!Utils.Utils.validarEmail(alunoUI.pessoaFisicaUI.email))
                    throw new AlunoBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroEmailInvalido, null,
                       AlunoBusinessException.TipoErro.ERRO_EMAIL_INVALIDO, false);

                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { secretariaBiz });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cdUsuarioAtend = (int)this.ComponentesUser.CodUsuario;

                if (alunoUI.pessoaFisicaUI.pessoaFisica.dt_nascimento.HasValue)
                    alunoUI.pessoaFisicaUI.pessoaFisica.dt_nascimento = alunoUI.pessoaFisicaUI.pessoaFisica.dt_nascimento.Value.Date;

                if (alunoUI.pessoaFisicaUI.relacionamentosUI != null)
                {
                    foreach (var relacionamento in alunoUI.pessoaFisicaUI.relacionamentosUI)
                    {
                        // RELACIONAMENTO PAIS
                        if (relacionamento.no_papel.ToLower().Equals("pais"))
                        {
                            relacionamento.enderecoRelac = montarEnderecoPrincipalPai(alunoUI.pessoaFisicaUI.endereco);
                        }
                    }
                }
                
                alunoUI.pessoaFisicaUI.pessoaFisica.id_pessoa_empresa = false;
                alunoUI.cdUsuarioFollowUp = cdUsuarioAtend;
                //alunoUI.aluno.cd_usuario_atendente = cdUsuaioAtend;
                if (alunoUI.pessoaFisicaUI.pessoaFisica != null && !string.IsNullOrEmpty(alunoUI.pessoaFisicaUI.descFoto))
                {
                    fullPath = ConfigurationManager.AppSettings["caminhoUploads"];
                    caminho = fullPath + "/" + alunoUI.pessoaFisicaUI.descFoto;
                    alunoUI.pessoaFisicaUI.pessoaFisica.img_pessoa = ManipuladorArquivo.getPathPhoto(caminho);
                    alunoUI.pessoaFisicaUI.pessoaFisica.ext_img_pessoa = alunoUI.pessoaFisicaUI.descFoto;
                }
                alunoUI.aluno.cd_pessoa_escola = cdEscola;
                
                //Tratamento do fuso horário da data:
                //alunoUI.aluno.Bolsa.dt_inicio_bolsa = SGF.Utils.ConversorUTC.Date(alunoUI.aluno.Bolsa.dt_inicio_bolsa.Date,
                //    this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);

                alunoUI.aluno.Bolsa.dt_inicio_bolsa = alunoUI.aluno.Bolsa.dt_inicio_bolsa.Date;

                var aluno = secretariaBiz.editAluno(alunoUI);
                //retorno.retorno = AlunoSearchUI.fromAluno(aluno);

                aluno.telefone = alunoUI.pessoaFisicaUI.telefone != null ? alunoUI.pessoaFisicaUI.telefone : alunoUI.pessoaFisicaUI.celular;
                aluno.email = alunoUI.pessoaFisicaUI.email;

                retorno.retorno = aluno;
                if (aluno.cd_aluno <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (AlunoBusinessException ex)
            {
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_EMAIL_JA_EXITE)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                else
                {
                    var message = Utils.Utils.innerMessage(ex);
                    if (message != "")
                    { 
                        AlunoBusinessException fx = new AlunoBusinessException(message, ex, 0, false);
                        return gerarLogException(message, retorno, logger, fx);
                    }
                    else
                    {
                            return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
                    }
                }
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_CPFJAEXISTENTE || ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_AUTO_RELACIONAMENTO
                    || ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_MINDATE_SMALLDATETIME)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                else
                {
                    var message = Utils.Utils.innerMessage(ex);
                    if (message != "")
                    {
                        AlunoBusinessException fx = new AlunoBusinessException(message, ex, 0, false);
                        return gerarLogException(message, retorno, logger, fx);
                    }
                    else
                    {
                        return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    AlunoBusinessException fx = new AlunoBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        private EnderecoSGF montarEnderecoPrincipalPai(EnderecoSGF enderecoSGF)
        {
            var endPrincipalPai = new EnderecoSGF 
            {
                cd_loc_bairro = enderecoSGF.cd_loc_bairro,
                cd_loc_cidade = enderecoSGF.cd_loc_cidade,
                cd_loc_distrito = enderecoSGF.cd_loc_distrito,
                cd_loc_estado = enderecoSGF.cd_loc_estado,
                cd_loc_logradouro = enderecoSGF.cd_loc_logradouro,
                cd_loc_pais = enderecoSGF.cd_loc_pais,
                cd_tipo_endereco = enderecoSGF.cd_tipo_endereco,
                cd_tipo_logradouro = enderecoSGF.cd_tipo_logradouro,
                dc_compl_endereco= enderecoSGF.dc_compl_endereco,
                dc_num_endereco = enderecoSGF.dc_num_endereco,
                num_cep = enderecoSGF.num_cep
            };

            return endPrincipalPai;
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getDataAluno(int cdAluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();

                Aluno alunoContext = alunoBiz.getAllDataAlunoById(cdAluno, cdEscola, true);
                retorno.retorno = alunoContext;
                if (alunoContext != null && alunoContext.cd_aluno <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getExistAlunoOrPessoaFisicaByCpf(string cpf)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var pessoaFisicaUI = alunoBiz.ExistAlunoOrPessoaFisicaByCpf(cpf, null, null, 0, cdEscola);
                if (pessoaFisicaUI != null && pessoaFisicaUI.pessoaFisica != null && pessoaFisicaUI.pessoaFisica.cd_pessoa > 0)
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
                    if (pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento.Count() > 0)
                    {
                        pessoaFisicaUI.relacionamentoUI = RelacionamentoUI.fromRelacionamentoforRelacionamentoUI(pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento, pessoaFisicaUI.pessoaFisica.cd_pessoa);
                        pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento = null;
                    }
                    retorno.retorno = pessoaFisicaUI;
                    retorno.AddMensagem(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgAlunocExistReturnData, pessoaFisicaUI.pessoaFisica.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (AlunoBusinessException ex)
            {
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNOJAEXISTE)
                {
                    return gerarLogException(ex.Message, retorno, logger, ex);
                }
                else
                {
                    return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
                }
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getExistAlunoOrPessoaFisicaByCpfAluno(string cpf)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var pessoaFisicaUI = alunoBiz.ExistAlunoOrPessoaFisicaByCpfAluno(cpf, null, null, 0, cdEscola);
                if (pessoaFisicaUI != null && pessoaFisicaUI.pessoaFisica != null && pessoaFisicaUI.pessoaFisica.cd_pessoa > 0)
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
                    if (pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento.Count() > 0)
                    {
                        pessoaFisicaUI.relacionamentoUI = RelacionamentoUI.fromRelacionamentoforRelacionamentoUI(pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento, pessoaFisicaUI.pessoaFisica.cd_pessoa);
                        pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento = null;
                    }
                    retorno.retorno = pessoaFisicaUI;
                    retorno.AddMensagem(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgAlunocExistReturnData, pessoaFisicaUI.pessoaFisica.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (AlunoBusinessException ex)
            {
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNOJAEXISTE)
                {
                    return gerarLogException(ex.Message, retorno, logger, ex);
                }
                else
                {
                    return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
                }
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage GetPessoaSearch(string nome, string apelido, int tipoPessoa, string cnpjCpf, int? papel, int sexo, bool inicio)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                if (papel <= 0)
                    papel = null;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var ret = alunoBiz.GetPessoaSearch(parametros, nome, apelido, tipoPessoa, cnpjCpf, papel, sexo, inicio, cdEscola, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoPessoaEnum.PESSOA_RELACIONADA);
                                
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, ret);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getPessoaTipoGeralSearch(string nome, string apelido, int tipoPessoa, string cnpjCpf, int? papel, int sexo, bool inicio)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                if (papel <= 0)
                    papel = null;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var ret = alunoBiz.GetPessoaSearch(parametros, nome, apelido, tipoPessoa, cnpjCpf, papel, sexo, inicio, cdEscola, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoPessoaEnum.PESSOA_GERAL);
                
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, ret);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage GetUrlRelatorioPessoa(string sort, int direction, string nome, string apelido, int tipoPessoa, string cnpjCpf, int? papel, int sexo, bool inicio)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = this.ComponentesUser.CodEmpresa.Value;
            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                if (papel <= 0)
                    papel = null;
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@nome=" + nome + "&@apelido=" + apelido + "&@tipoPessoa=" + tipoPessoa + "&@cnpjCpf=" + cnpjCpf + "&@papel=" + papel + "&@sexo=" + sexo + "&@inicio=" + inicio + "&@cdEscola=" + cdEscola + "&@tipo=" + FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoPessoaEnum.PESSOA_RELACIONADA + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Pessoa&" +
                    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.PessoaSearch;
                    
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = null;
                response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Pessoa

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getPessoaUsuarioSearch(string nome, string apelido, string cnpjCpf, int sexo, bool inicio)
        {
            try
            {
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getPessoaUsuarioSearch(parametros, nome, apelido, cnpjCpf, sexo, inicio, cdEscola, cod_pessoa_usuario);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage GetPessoaPorEscolaSearch(string nome, string apelido, int tipoPessoa, string cnpjCpf, int? papel, int sexo, bool inicio)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                if (papel <= 0)
                    papel = null;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var ret = alunoBiz.GetPessoaSearch(parametros, nome, apelido, tipoPessoa, cnpjCpf, papel, sexo, inicio, cdEscola, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoPessoaEnum.PESSOA_GERAL);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, ret);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getPessoaPapelSearchWithCPFCNPJ(string nome, string apelido, int tipoPessoa, string cnpjCpf, int? papel, int sexo, bool inicio)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                if (papel <= 0)
                    papel = null;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                IEnumerable<PessoaSGFSearchUI> ret = alunoBiz.getPessoaPapelSearchWithCPFCNPJ(parametros, nome, apelido, tipoPessoa, cnpjCpf, papel, sexo, inicio, cdEscola);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, ret);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getAlunoDesistente(string nome, string email, int status, string cnpjCpf, bool inicio, int cdSituacao, int sexo)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                if (nome == null)
                    nome = String.Empty;
                if (email == null)
                    email = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                IEnumerable<AlunoSearchUI> ret = alunoBiz.getAlunoDesistente(parametros, nome, email, inicio, getStatus(status), cdEscola, cnpjCpf, cdSituacao, sexo);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, ret);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "rpAlu")]
        public HttpResponseMessage GetUrlRelAlunoCliente(string nome, int cdResp, string telefone, string email, int status, string dtaIni, string dtaFinal, int cd_midia,
            string situacaoAlunoTurma, bool exibirEnderecos)
        {
            ReturnResult retorno = new ReturnResult();
            ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
            base.configuraBusiness(new List<IGenericBusiness>() { secretariaBiz });
            int cdEscola = this.ComponentesUser.CodEmpresa.Value;
            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                DateTime? dt_inicio = null;
                if (dtaIni != null && dtaIni != "")
                    dt_inicio = Convert.ToDateTime(dtaIni);

                DateTime? dt_fim = null;
                if (dtaFinal != null && dtaIni != "")
                    dt_fim = Convert.ToDateTime(dtaFinal);
                string PTitulo = "Relatório de Aluno/Cliente";
                string[] situacao = situacaoAlunoTurma.Split('|');
                if (situacao[0] != "100")
                {
                    PTitulo = PTitulo + "    Situação: ";
                    for (int i = 0; i < situacao.Count(); i++)
                    {
                        if (i > 0)
                            PTitulo = PTitulo + ',';
                        string dc_situacao = AlunoTurma.getSituacaoAlunoTurma(Int32.Parse(situacao[i]));
                        PTitulo = PTitulo + " " + dc_situacao;
                    }
                    PTitulo = PTitulo + "\n" + "\n";
                }
                else
                    PTitulo = PTitulo + "\n" + "\n";
                if (cd_midia > 0)
                {
                    Midia md = secretariaBiz.GetMidiaById(cd_midia);
                    PTitulo = PTitulo + "Como conheceu: " + md.no_midia;
                }
                string parametros = "@nome=" + nome + "&@cdResp=" + cdResp + "&@telefone=" + telefone + "&@email=" + email + "&@status=" + status + "&@cdEscola=" +
                    cdEscola + "&@dtaIni=" + dt_inicio + "&@dtaFinal=" + dt_fim + "&@cd_midia=" + cd_midia + "&@situacaoAlunoTurma=" + situacaoAlunoTurma + "&@exibirEnderecos=" + exibirEnderecos + "&" +
                    Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + PTitulo; //+ "&" +
                    //Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.AlunoRel;
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }



        [HttpComponentesAuthorize(Roles = "rpAlu")]
        public HttpResponseMessage GetUrlRelAluno(string nome, int cdResp, string telefone, string email, int status, string dtaIni, string dtaFinal, int cd_midia,
            string situacaoAlunoTurma, bool exibirEnderecos)
        {
            ReturnResult retorno = new ReturnResult();
            ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
            base.configuraBusiness(new List<IGenericBusiness>() { secretariaBiz });
            int cdEscola = this.ComponentesUser.CodEmpresa.Value;
            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                DateTime? dt_inicio = null;
                if (dtaIni != null && dtaIni != "")
                    dt_inicio = Convert.ToDateTime(dtaIni);

                DateTime? dt_fim = null;
                if (dtaFinal != null && dtaIni != "")
                    dt_fim = Convert.ToDateTime(dtaFinal);
                string PTitulo = "Relatório de Aluno/Cliente";
                string[] situacao = situacaoAlunoTurma.Split('|');
                if (situacao[0] != "100") {
                    PTitulo = PTitulo + "    Situação: ";
                    for (int i = 0; i < situacao.Count(); i++)
                    {
                        if (i > 0)
                            PTitulo = PTitulo + ',';
                        string dc_situacao = AlunoTurma.getSituacaoAlunoTurma(Int32.Parse(situacao[i]));
                        PTitulo = PTitulo + " " + dc_situacao; 
                    }
                    PTitulo = PTitulo + "\n" + "\n";
                }
                else
                    PTitulo = PTitulo + "\n" + "\n";
                if (cd_midia > 0)
                {
                    Midia md = secretariaBiz.GetMidiaById(cd_midia);
                    PTitulo = PTitulo + "Como conheceu: " + md.no_midia;
                }
                string parametros = "@orientation=LANDSCAPE&@nome=" + nome + "&@cdResp=" + cdResp + "&@telefone=" + telefone + "&@email=" + email + "&@status=" + status + "&@cdEscola=" +
                    cdEscola + "&@dtaIni=" + dt_inicio + "&@dtaFinal=" + dt_fim + "&@cd_midia=" + cd_midia + "&@situacaoAlunoTurma=" + situacaoAlunoTurma + "&@exibirEnderecos=" + exibirEnderecos + "&" +
                    Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + PTitulo + "&" +
                    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.AlunoRel;
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        [HttpComponentesAuthorize(Roles = "mvtc,mvtd,mvtp,mvts, mvtdv")]
        public HttpResponseMessage getPessoaMovimentoSearch(string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int tipoMovimento)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                //if(papel <= 0)
                //    papel = null;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getPessoaMovimentoSearch(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, tipoMovimento, cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getPessoaTituloSearch(string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, bool responsavel)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                //if(papel <= 0)
                //    papel = null;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getPessoaTituloSearch(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, cdEscola, responsavel);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(parametros.from, parametros.to, parametros.qtd_limite);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getAllPessoaSomenteEscola(string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                //if(papel <= 0)
                //    papel = null;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getPessoaRelacionadaEscola(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(parametros.from, parametros.to, parametros.qtd_limite);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        //Método que pesquisa a pessoa da escola e trás também o dono do CPF/CNPJ.
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getPessoaSearchEscolaWithCPFCNPJ(string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int papel)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                //if(papel <= 0)
                //    papel = null;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getPessoaSearchEscolaWithCPFCNPJ(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, cdEscola, papel);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(parametros.from, parametros.to, parametros.qtd_limite);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getTdsPessoaSearchEscola(string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                //if(papel <= 0)
                //    papel = null;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getTdsPessoaSearchEscola(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(parametros.from, parametros.to, parametros.qtd_limite);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getPessoaResponsavelCPFSearchEscola(string nome, string apelido, string cnpjCpf, int sexo, bool inicio)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cnpjCpf == null)
                    cnpjCpf = String.Empty;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.getPessoaResponsavelCPFSearchEscola(parametros, nome, apelido, inicio, cnpjCpf, sexo, cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        [HttpComponentesAuthorize(Roles = "tpend")]
        [HttpComponentesAuthorize(Roles = "bair")]
        [HttpComponentesAuthorize(Roles = "estad")]
        [HttpComponentesAuthorize(Roles = "cidd")]
        [HttpComponentesAuthorize(Roles = "tlog")]
        public HttpResponseMessage VerificarExistEmpresaByCnpjOrCdEmpresa(string cnpj, int? cdEmpresa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                List<RelacionamentoUI> listaRelacPai = null;
                List<RelacionamentoUI> listaRelacFilho = null;
                List<RelacionamentoUI> unionRelac = null;
                IPessoaBusiness pessoaBiz = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                var pessoaJuridica = pessoaBiz.VerificarExisitsEmpresaByCnpjOrcdEmpresa(cnpj, cdEmpresa);
                if (pessoaJuridica != null && pessoaJuridica.pessoaJuridica != null)
                {
                    pessoaJuridica.pessoaJuridica.dt_cadastramento = 
                          SGF.Utils.ConversorUTC.ToLocalTime(pessoaJuridica.pessoaJuridica.dt_cadastramento, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                    
                    if (pessoaJuridica.pessoaJuridica.TelefonePessoa.Count() > 0)
                    {
                        pessoaJuridica.contatosUI = TelefoneUI.fromTelefoneforTelefoneUI(pessoaJuridica.pessoaJuridica.TelefonePessoa);
                        pessoaJuridica.pessoaJuridica.TelefonePessoa = null;
                    }
                    if (pessoaJuridica.pessoaJuridica.Telefone != null)
                    {
                        pessoaJuridica.pessoaJuridica.Telefone.ClasseTelefone.TelefoneClasse = null;
                        pessoaJuridica.pessoaJuridica.Telefone.TelefonePessoa = null;
                        pessoaJuridica.pessoaJuridica.Telefone.TelefoneTipo.TipoTelefoneRef = null;
                        pessoaJuridica.pessoaJuridica.Telefone.TelefonePessoa = null;
                    }
                    //if (pessoaJuridica.pessoaJuridica.PessoaEndereco.Count() > 0)
                    //    pessoaJuridica.enderecosUI = EnderecoUI.fromEnderecoforEnderecoUI(pessoaJuridica.pessoaJuridica.PessoaEndereco, pessoaJuridica.pessoaJuridica.cd_endereco_principal);
                    if (pessoaJuridica.pessoaJuridica.PessoaPaiRelacionamento != null && pessoaJuridica.pessoaJuridica.PessoaPaiRelacionamento.Count() > 0)
                    {
                        listaRelacPai = RelacionamentoUI.fromRelacionamentoforRelacionamentoUI(pessoaJuridica.pessoaJuridica.PessoaPaiRelacionamento, pessoaJuridica.pessoaJuridica.cd_pessoa).ToList();
                        pessoaJuridica.pessoaJuridica.PessoaFilhoRelacionamento = null;
                    }
                    if (pessoaJuridica.pessoaJuridica.PessoaFilhoRelacionamento != null && pessoaJuridica.pessoaJuridica.PessoaFilhoRelacionamento.Count() > 0)
                    {
                        listaRelacFilho = RelacionamentoUI.fromRelacionamentoforRelacionamentoUI(pessoaJuridica.pessoaJuridica.PessoaFilhoRelacionamento, pessoaJuridica.pessoaJuridica.cd_pessoa).ToList();
                        pessoaJuridica.pessoaJuridica.PessoaFilhoRelacionamento = null;
                    }
                    if (listaRelacPai != null && listaRelacFilho != null)
                        unionRelac = listaRelacPai.Union(listaRelacFilho).ToList();
                    else
                        if (listaRelacPai != null)
                            unionRelac = listaRelacPai.ToList();
                        else if (listaRelacFilho != null)
                            unionRelac = listaRelacFilho.ToList();
                    pessoaJuridica.relacionamentoUI = unionRelac;
                }
                
                retorno.retorno = pessoaJuridica;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        [HttpComponentesAuthorize(Roles = "tpend")]
        [HttpComponentesAuthorize(Roles = "bair")]
        [HttpComponentesAuthorize(Roles = "estad")]
        [HttpComponentesAuthorize(Roles = "cidd")]
        [HttpComponentesAuthorize(Roles = "tlog")]
        public HttpResponseMessage VerificarExistPessoByCpfOrCdPessoa(string cpf, int cdPessoa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                List<RelacionamentoUI> listaRelacPai = null;
                List<RelacionamentoUI> listaRelacFilho = null;
                List<RelacionamentoUI> unionRelac = null;
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                IPessoaBusiness pessoaBiz = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                PessoaFisicaSearchUI pessoaFisicaUI = pessoaBiz.VerificarExisitsPessoByCpfOrCdPessoa(cpf, null,null,0,cdPessoa, cd_escola);
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
                    if (pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento != null && pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento.Count() > 0)
                    {
                        listaRelacPai = RelacionamentoUI.fromRelacionamentoforRelacionamentoUI(pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento, pessoaFisicaUI.pessoaFisica.cd_pessoa).ToList();
                        pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento = null;
                    }
                    if (pessoaFisicaUI.pessoaFisica.PessoaFilhoRelacionamento != null && pessoaFisicaUI.pessoaFisica.PessoaFilhoRelacionamento.Count() > 0)
                    {
                        listaRelacFilho = RelacionamentoUI.fromRelacionamentoforRelacionamentoUI(pessoaFisicaUI.pessoaFisica.PessoaFilhoRelacionamento, pessoaFisicaUI.pessoaFisica.cd_pessoa).ToList();
                        pessoaFisicaUI.pessoaFisica.PessoaFilhoRelacionamento = null;
                    }
                    if (listaRelacPai != null && listaRelacFilho != null)
                        unionRelac = listaRelacPai.Union(listaRelacFilho).ToList();
                    else
                        if (listaRelacPai != null)
                            unionRelac = listaRelacPai.ToList();
                        else if (listaRelacFilho != null)
                            unionRelac = listaRelacFilho.ToList();
                    pessoaFisicaUI.relacionamentoUI = unionRelac;

                    pessoaFisicaUI.pessoaFisica.dt_cadastramento = Utils.ConversorUTC.ToLocalTime(pessoaFisicaUI.pessoaFisica.dt_cadastramento, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                }
                retorno.retorno = pessoaFisicaUI;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage VerificaExistAlunoraf(int cd_aluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                IPessoaBusiness pessoaBiz = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                PessoaFisicaSearchUI pessoaFisicaUI = null;
                Aluno aluno = alunoBiz.getAlunoById(cd_aluno, cd_escola);

                if (aluno != null)
                {
                     pessoaFisicaUI = pessoaBiz.VerificaExisitsAlunoRafMatricula(aluno.cd_pessoa_aluno, cd_escola);
                }

                
                retorno.retorno = pessoaFisicaUI;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        #endregion

        #region Rel Bolsistas
        [HttpComponentesAuthorize(Roles = "rpbol")]
        public HttpResponseMessage getUrlRelatorioBolsistas(int cd_aluno, int cd_turma, bool cancelamento, decimal? per_bolsa, int cd_motivo_bolsa, string dtIniComunicado,
                                                        string dtFimComunicado, string dtIni, string dtFim, bool periodo_ini, bool periodo_cancel)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                DateTime? dataIniCom = string.IsNullOrEmpty(dtIniComunicado) ? null : (DateTime?)Convert.ToDateTime(dtIniComunicado);
                DateTime? dataFinalbCom = string.IsNullOrEmpty(dtFimComunicado) ? null : (DateTime?)Convert.ToDateTime(dtFimComunicado);
                DateTime? dataIncial = string.IsNullOrEmpty(dtIni) ? null : (DateTime?)Convert.ToDateTime(dtIni);
                DateTime? dataFinal = string.IsNullOrEmpty(dtFim) ? null : (DateTime?)Convert.ToDateTime(dtFim);
                //cd_empresa
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cdEscola=" + cdEscola + "&@cd_aluno=" + cd_aluno + "&@cd_turma=" + cd_turma + "&@cancelamento=" + cancelamento +
                                    "&@per_bolsa=" + per_bolsa + "&@cd_motivo_bolsa=" + cd_motivo_bolsa + "&@dtIniComunicado=" + dataIniCom + "&@dtFimComunicado=" + dataFinalbCom + "&@dtIni=" + dataIncial +
                                    "&@dtFim=" + dataFinal + "&@periodo_ini=" + periodo_ini + "&@periodo_cancel=" + periodo_cancel;
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion


        [HttpGet]
        public HttpResponseMessage findAlunoCartaQuitacao(int ano, int cdPessoa)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var retorno = alunoBiz.findAlunoCartaQuitacao(parametros, cdEscola, ano, cdPessoa);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage GetAlunosemAula(int cd_aluno, int cd_item, bool idEscola, string dtInicial, string dtFinal, bool idMovimento, bool idHistorico, byte idSituacao)
        {
            //ReturnResult retorno = new ReturnResult();
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                int cdEscola = idEscola ? 0 : this.ComponentesUser.CodEmpresa.Value;
                if(!idMovimento && !idHistorico) idHistorico = true;
                var alunos = alunoBiz.getAlunosemAula(parametros, cd_aluno, cd_item, cdEscola, dtaInicial, dtaFinal, idMovimento, idHistorico, idSituacao);
                //retorno.retorno = alunos;

                //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                //configureHeaderResponse(response, null);
                //return response;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, alunos);
                base.configureHeaderResponse(response, parametros);
                return response;


            }
            catch (Exception ex)
            {
                //return gerarLogException(Messages.msgRegBuscError, alunos, logger, ex);
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage GetAlunosCargaHoraria(int cd_aluno, int cd_turma, string dtInicial, string dtFinal)
        {
            //ReturnResult retorno = new ReturnResult();
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var alunos = alunoBiz.getAlunoCargaHoraria(parametros, cd_aluno, cd_turma, cdEscola, dtaInicial, dtaFinal);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, alunos);
                base.configureHeaderResponse(response, parametros);
                return response;


            }
            catch (Exception ex)
            {
                //return gerarLogException(Messages.msgRegBuscError, alunos, logger, ex);
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage GetNotasDevolvidas(int cd_aluno, int cd_item, bool idEscola, string dtInicial, string dtFinal, bool idMovimento, bool idHistorico, byte idSituacao)
        {
            //ReturnResult retorno = new ReturnResult();
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                int cdEscola = idEscola ? 0 : this.ComponentesUser.CodEmpresa.Value;
                if (!idMovimento && !idHistorico) idHistorico = true;
                var alunos = alunoBiz.getNotasDevolvidas(parametros, cd_aluno, cd_item, cdEscola, dtaInicial, dtaFinal, idMovimento, idHistorico, idSituacao);
                //retorno.retorno = alunos;

                //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                //configureHeaderResponse(response, null);
                //return response;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, alunos);
                base.configureHeaderResponse(response, parametros);
                return response;


            }
            catch (Exception ex)
            {
                //return gerarLogException(Messages.msgRegBuscError, alunos, logger, ex);
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage PostGerarKardexEntrada(AlunosemAulaUI linha)
        {
            ReturnResult retorno = new ReturnResult();
            IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
            try
            {
                int cd_usuario = (int)this.ComponentesUser.CodUsuario;
                int fusoHorario = this.ComponentesUser.IdFusoHorario;

                string ret = alunoBiz.postGerarKardexEntrada(linha.cd_item_movimento, cd_usuario, fusoHorario);

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
        [HttpPost]
        public HttpResponseMessage PostGerarNotaVoucher(AlunosCargaHorariaUI linha)
        {
            ReturnResult retorno = new ReturnResult();
            IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
            try
            {
                int cd_usuario = (int)this.ComponentesUser.CodUsuario;
                int fusoHorario = this.ComponentesUser.IdFusoHorario;

                string ret = alunoBiz.postGerarNotaVoucher(linha.cd_desistencia, cd_usuario, fusoHorario, linha.itemVoucher);

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


        public HttpResponseMessage GetCargaHoraria(int cd_aluno, int cd_turma, int cd_curso, int cd_professor, bool todasEscolas, int nm_aulas_vencimento)
        {
            try
            {
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var alunos = alunoBiz.getCargaHoraria(parametros, cd_aluno, cd_turma, cd_curso, cdEscola, cd_professor, todasEscolas, nm_aulas_vencimento);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, alunos);
                base.configureHeaderResponse(response, parametros);
                return response;


            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

    }
}