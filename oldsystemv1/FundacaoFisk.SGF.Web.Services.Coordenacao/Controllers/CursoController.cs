using Componentes.GenericController;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;


namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Controllers
{
    public class CursoController : ComponentesApiController
    {
        //Declaração de Atributos
        private static readonly ILog logger = LogManager.GetLogger(typeof(CursoController));

        //Método construtor
        public CursoController()
        {
        }

       //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage GetUrlRelatorioCurso(string sort, int direction, string desc, bool inicio, int status) {
            ReturnResult retorno = new ReturnResult();

            try {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Curso&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.CursoSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                var parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                
                HttpResponseMessage response = null;
                
                if(parametrosCript == null)
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                else
                    response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
  
                configureHeaderResponse(response, null);
                return response;
            }
            catch(Exception exe) {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getCursoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                var curso = cursoBiz.findCursoById(id);
                retorno.retorno = curso;
                if (curso.cd_curso <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getCursoProduto(string desc, int? cdProd)
        {
           // ReturnResult retorno = new ReturnResult();
            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                if (desc == null) desc = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = cursoBiz.getCursoProduto(parametros,desc, cdProd);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getCursosProdutos(string cd_produtos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                List<Curso> cursos = new List<Curso>();
                if (!String.IsNullOrEmpty(cd_produtos))
                {
                    List<int> cdProdutos = Array.ConvertAll(cd_produtos.Split(','), s => int.Parse(s)).OfType<int>().ToList();
                    cursos = cursoBiz.getCursoProduto(cdProdutos).ToList();
                }
                retorno.retorno = cursos;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getCursoProdutoPorTipoAval(string desc, int? cdProd, int cdTipoAvaliacao)
        {
            // ReturnResult retorno = new ReturnResult();
            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                if (desc == null) desc = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                if (parametros.qtd_limite == 0)
                    parametros.qtd_limite = 5;
                if (parametros.to == 0)
                    parametros.to = 4;
                var retorno = cursoBiz.getCursoProdutoPorTipoAval(parametros, desc, cdProd, cdTipoAvaliacao);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getCursoTabelaPreco()
        {
             ReturnResult retorno = new ReturnResult();
            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var curso = cursoBiz.getCursoTabelaPreco(cdEscola);
                retorno.retorno = curso;
                if (curso.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getCursoFKSearch(String desc, bool inicio, int status, int? produto, int? estagio, int? modalidade, int? nivel, string dataInicial, string dataFinal)
        {
            try
            {
                DateTime? dt_inicial = string.IsNullOrEmpty(dataInicial) ? null : (DateTime?)Convert.ToDateTime(dataInicial);
                DateTime? dt_final = string.IsNullOrEmpty(dataFinal) ? null : (DateTime?)Convert.ToDateTime(dataFinal);

                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                if (desc == null) desc = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = cursoBiz.getCursoSearch(parametros, desc, inicio, getStatus(status), produto, estagio, modalidade, nivel, dt_inicial, dt_final);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getCursoSearch(String desc, bool inicio, int status, int? produto, int? estagio, int? modalidade, int? nivel)
        {
            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                if (desc == null) desc = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = cursoBiz.getCursoSearch(parametros, desc, inicio, getStatus(status), produto, estagio, modalidade, nivel, null, null);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getCursoByContratoSearch(int cd_contrato, String desc, bool inicio, int status, int? produto, int? estagio, int? modalidade, int? nivel)
        {
            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                if (desc == null) desc = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = cursoBiz.getCursoByContratoSearch(parametros, cd_contrato, desc, inicio, getStatus(status), produto, estagio, modalidade, nivel, null, null);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        //Traz os registros pelo status da Entidade
        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getCursos(int? cd_curso) {
            ReturnResult retorno = new ReturnResult();

            try {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                var cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_ATIVO, cd_curso, null, null);
                retorno.retorno = cursos;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getCursosAulaPersonalizada(int cd_aluno)
        {
            ReturnResult retorno = new ReturnResult();
            try {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                var cursos = cursoBiz.getCursosAulaPersonalizada(cd_aluno, cd_escola);
                retorno.retorno = cursos;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getCursosCargaHoraria(bool todasEscolas)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                var cursos = cursoBiz.getCursosCargaHoraria(todasEscolas, cd_escola);
                retorno.retorno = cursos;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "cur.e")]
        public HttpResponseMessage postDeleteCurso(List<Curso> cursos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                var delCurso = cursoBiz.deleteCursos(cursos);
                retorno.retorno = delCurso;
                if(!delCurso)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "cur.i")]
        public HttpResponseMessage postCurso(CursoUI cursoUI) {
            ReturnResult retorno = new ReturnResult();
            try {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                Curso curso = cursoBiz.addCurso(cursoUI.curso, cursoUI.materiaisDidaticos.ToList());
                retorno.retorno = curso;
                if(curso == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else 
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "cur.a")]
        public HttpResponseMessage postEditCurso(CursoUI cursoUI) {
            ReturnResult retorno = new ReturnResult();
            try {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;

                Curso curso = cursoBiz.editCurso(cursoUI.curso, cursoUI.materiaisDidaticos.ToList(), cd_pessoa_escola);
                retorno.retorno = curso;
                if(curso == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        //Traz os registros pelo status da Entidade
        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getCursosPorProduto(int? cd_curso,int cd_produto)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                var cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_ATIVOPROD, cd_curso, cd_produto, null);
                retorno.retorno = cursos;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getProximoCursoPorProdutoSemMatriculaSimultanea(int? cd_curso, int cd_produto)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness) base.instanciarBusiness<ICursoBusiness>();
                var cursos = cursoBiz.getProximoCursoPorProdutoSemMatriculaSimultanea(
                    CursoDataAccess.TipoConsultaCursoEnum.HAS_ATIVOPROD, cd_curso, cd_produto, null);
                retorno.retorno = cursos;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (CursoBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getCursosPorProdutoSemMatriculaSimultanea(int? cd_curso, int cd_produto)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                var cursos = cursoBiz.getCursosSemMatriculaSimultanea(CursoDataAccess.TipoConsultaCursoEnum.HAS_ATIVOPROD, cd_curso, cd_produto, null);
                retorno.retorno = cursos;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage GetCursoOrdem(int cdCurso)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                var curso = cursoBiz.GetCursoOrdem(cdCurso);
                retorno.retorno = curso;
                if (curso <= 0 || curso == null)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        public HttpResponseMessage getCursosFollowUp()
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                var cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_TURMA, null, null, cdEscola);
                retorno.retorno = cursos;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getCursoByProdutoSemMatriculaSimultanea(int? cd_curso, int cd_produto)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                var cursos = cursoBiz.getCursoByProdutoSemMatriculaSimultanea(cd_curso, cd_produto, null);
                retorno.retorno = cursos;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage getPesCursos(CursoDataAccess.TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                int? cdEscola = this.ComponentesUser.CodEmpresa;
                var cursos = cursoBiz.getCursos(hasDependente, cd_curso, cd_produto, cdEscola);
                retorno.retorno = cursos;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "cur")]
        public HttpResponseMessage GetCursoProgramacao()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                var ge = cursoBiz.getCursoProgramacao().ToList();

                for (int i = 0; i < ge.Count; i++)
                    ge[i].ProximoCurso = null;

                retorno.retorno = ge;
                if (ge.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
    }
}