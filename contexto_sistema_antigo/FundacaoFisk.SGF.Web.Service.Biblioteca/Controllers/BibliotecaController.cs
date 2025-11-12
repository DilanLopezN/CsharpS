using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Resources;
using System.Threading;
using System.Web.Http;
using System.Web.UI.WebControls;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using Newtonsoft.Json;
using FundacaoFisk.SGF.Utils.Messages;
using log4net;
using System.Web;
using Componentes.GenericController;
using FundacaoFisk.SGF.Web.Services.Biblioteca.Comum.IBusiness;
using Componentes.GenericBusiness.Comum;


namespace FundacaoFisk.SGF.Web.Services.Biblioteca.Controllers
{
    public class BibliotecaController : ComponentesApiController
    {
        //Declaração de Atributos
        private static readonly ILog logger = LogManager.GetLogger(typeof(BibliotecaController));



        //Método construtor
        public BibliotecaController()
        {
        }

        // GET api/<controller>/5 
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "bib")]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getPessoaBibliotecaSearch(string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int? papel)
        {
            try {
                if(nome == null)
                    nome = String.Empty;
                if(apelido == null)
                    apelido = String.Empty;
                if(cnpjCpf == null)
                    cnpjCpf = String.Empty;
                //if(papel <= 0)
                //    papel = null;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IBibliotecaBusiness bibliotecaBiz = (IBibliotecaBusiness)base.instanciarBusiness<IBibliotecaBusiness>();
                var retorno = bibliotecaBiz.getPessoaBibliotecaSearch(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, this.ComponentesUser.CodEmpresa.Value);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "bib")]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getPessoaEmprestimoSearch(string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int? papel)
        {
            try {
                if(nome == null)
                    nome = String.Empty;
                if(apelido == null)
                    apelido = String.Empty;
                if(cnpjCpf == null)
                    cnpjCpf = String.Empty;
                //if(papel <= 0)
                //    papel = null;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IBibliotecaBusiness bibliotecaBiz = (IBibliotecaBusiness)base.instanciarBusiness<IBibliotecaBusiness>();
                var retorno = bibliotecaBiz.getPessoaEmprestimoSearch(parametros, nome, apelido, inicio, tipoPessoa, cnpjCpf, sexo, this.ComponentesUser.CodEmpresa.Value);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }


        
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "bib")]
        public HttpResponseMessage getEmprestimoSearch(int? cd_pessoa, int? cd_item, bool? pendentes, string dt_inicial, string dt_final, bool? emprestimos, bool? devolucao) {
            try {
                DateTime? dta_inicial = null;
                DateTime? dta_final = null;

                if(!String.IsNullOrEmpty(dt_inicial))
                    dta_inicial = DateTime.Parse(dt_inicial);
                if(!String.IsNullOrEmpty(dt_final))
                    dta_final = DateTime.Parse(dt_final);

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IBibliotecaBusiness bibliotecaBiz = (IBibliotecaBusiness)base.instanciarBusiness<IBibliotecaBusiness>();
                var retorno = bibliotecaBiz.getEmprestimoSearch(parametros, cd_pessoa, cd_item, pendentes, dta_inicial, dta_final, emprestimos, devolucao, this.ComponentesUser.CodEmpresa.Value);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

       
        [HttpComponentesAuthorize(Roles = "bib")]
        public HttpResponseMessage getUrlRelatorioEmprestimo(string sort, int direction, int? cd_pessoa, int? cd_item, bool? pendentes, string dt_inicial, string dt_final, bool? emprestimos, bool? devolucao) {
            ReturnResult retorno = new ReturnResult();
            try {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cd_pessoa=" + cd_pessoa + "&@cd_escola=" + this.ComponentesUser.CodEmpresa.Value + "&@cd_item=" + cd_item + "&@pendentes=" + pendentes + "&@dt_inicial=" + dt_inicial + "&@dt_final=" + dt_final + "&@emprestimos=" + emprestimos + "&@devolucao=" + devolucao + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Biblioteca&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.BibliotecaSearch;
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
    }
}