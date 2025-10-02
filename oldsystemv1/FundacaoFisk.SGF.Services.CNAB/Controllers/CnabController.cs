using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Componentes.Utils;
using Newtonsoft.Json;
using FundacaoFisk.SGF.Utils.Messages;
using log4net;
using Componentes.GenericModel;
using Componentes.GenericController;
using System.Net.Http.Headers;
using FundacaoFisk.SGF.Web;
using FundacaoFisk.SGF.GenericModel;
using System.Web;
using Componentes.ApresentadorRelatorio;
using System.Threading.Tasks;
using System.IO;
using Componentes.GenericBusiness.Comum;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IBusiness;
using System.Configuration;
using System.Globalization;
using FundacaoFisk.SGF.Web.Services.CNAB.Business;
using FundacaoFisk.SGF.Web.Services.CNAB.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;

namespace FundacaoFisk.SGF.Services.CNAB.Controllers
{
    public class CnabController : ComponentesApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CnabController));

        public CnabController()
        {
        }

        #region Cnab

        [HttpComponentesAuthorize(Roles = "cnabb")]
        [HttpGet]
        public HttpResponseMessage gerarCnab(int cd_cnab, byte id_tipo_cnab)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                Cnab cnabGrade = new Cnab();
                switch (id_tipo_cnab)
                {
                    case (byte)Cnab.TipoCnab.GERAR_BOLETOS: 
                        cnabGrade = Business.postGerarCnab(cdEscola, cd_cnab);
                        break;
                    case (byte)Cnab.TipoCnab.CANCELAR_BOLETOS:
                        cnabGrade = Business.postCancelarCnab(cdEscola, cd_cnab);
                        break;
                    case (byte)Cnab.TipoCnab.PEDIDO_BAIXA:
                        cnabGrade = Business.postGerarPedidoBaixaCnab(cdEscola, cd_cnab);
                        break;
                }
                if (cnabGrade.cd_cnab >= 0)
                    retorno.AddMensagem(Messages.msgCnabSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgCnabError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                retorno.retorno = cnabGrade;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (CnabBusinessException exe)
            {
                if (exe.tipoErro == CnabBusinessException.TipoErro.ERRO_TITULO_STATUSCNAB_DIFERENTE_INCIAL ||
                    exe.tipoErro == CnabBusinessException.TipoErro.ERRO_NAO_GEROU_CNAB ||
                    exe.tipoErro == CnabBusinessException.TipoErro.ERRO_CNAB_SEM_PESSOA_BANCO_CNPJ ||
                    exe.tipoErro == CnabBusinessException.TipoErro.ERRO_CNAB_SEM_AGENCIA ||
                    exe.tipoErro == CnabBusinessException.TipoErro.ERRO_CNAB_SEM_CONTA_CORRENTE ||
                    exe.tipoErro == CnabBusinessException.TipoErro.ERRO_CNAB_SEM_DIGITO ||
                    exe.tipoErro == CnabBusinessException.TipoErro.ERRO_CNAB_NRO_BANCO ||
                    exe.tipoErro == CnabBusinessException.TipoErro.ERRO_NOSSO_NUMERO_NAO_INFORMADO ||
                    exe.tipoErro == CnabBusinessException.TipoErro.ERRO_DT_EMISSAO_MAIOR_PROCESSAMENTO_CNAB)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgCnabError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "cnabb")]
        public HttpResponseMessage getCnabSearch(int cd_carteira, int cd_usuario, byte tipo_cnab, int status, string dtInicial,
                                                string dtFinal, bool emissao, bool vencimento, string nossoNumero, int? nro_contrato,
                                                bool icnab, bool iboleto, int cd_responsavel, int cd_aluno)
        {
            try
            {
                DateTime? dtaInicial = string.IsNullOrEmpty(dtInicial) ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = string.IsNullOrEmpty(dtFinal) ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                //Se receber o usuário com código zero mas não for usuario administrador, pegar o usuário do login
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                var retorno = Business.searchCnab(parametros, cd_carteira, cd_usuario, tipo_cnab, status, dtaInicial, dtaFinal, emissao, vencimento, nossoNumero, nro_contrato, cdEscola, icnab, iboleto, cd_responsavel, cd_aluno);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpComponentesAuthorize(Roles = "usu")]
        [HttpComponentesAuthorize(Roles = "cnabb")]
        public HttpResponseMessage getComponentesPesquisaCnab()
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                Cnab cnab = new Cnab();
                cnab.cd_usuario = (int)this.ComponentesUser.CodUsuario;
                cnab.adm = this.ComponentesUser.IdMaster;
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                cnab.usuarios = Business.getUsuariosCnab(cdEscola, cnab.adm, cnab.cd_usuario).ToList();
                cnab.carteirasCnab = Business.getCarteirasCnab(cdEscola).ToList();

                retorno.retorno = cnab;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpComponentesAuthorize(Roles = "usu")]
        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getComponentesNovoCnab()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cd_pessoa_usuario = 0;
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                ICoordenacaoBusiness BusinessCoordenacao = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IFinanceiroBusiness BusinessFinan = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                Cnab cnab = new Cnab();
                cnab.carteirasCnab = Business.getCarteirasCnab(cdEscola, 0, CarteiraCnabDataAccess.TipoConsultaCarteiraCnab.HAS_ATIVO).ToList();
                cnab.produtos = BusinessCoordenacao.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_ATIVO_CURSO, null, null).ToList();
                cnab.locaisMvto = BusinessFinan.getLocalMovimentoSomenteLeitura(cdEscola, 0, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_FILTRO_CNAB, cd_pessoa_usuario);
                retorno.retorno = cnab;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

       [HttpComponentesAuthorize(Roles = "cnabb")]
       [HttpComponentesAuthorize(Roles = "locmv")]
       [HttpComponentesAuthorize(Roles = "tit")]
       [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getComponentesByCnabEdit(int cd_cnab)
       {
           ReturnResult retorno = new ReturnResult();
           int cdEscola = (int)this.ComponentesUser.CodEmpresa;
           int cd_pessoa_usuario = 0;//MMC
           try
           {
               ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
               ICoordenacaoBusiness BusinessCoordenacao = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
               IFinanceiroBusiness BusinessFinan = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
               Cnab cnab = Business.getCnabEditView(cd_cnab, cdEscola);
               if (cnab != null)
               {
                   int cd_carteira_cnab = 0;
                   if (cnab.cd_local_movto > 0)
                       cd_carteira_cnab = cnab.cd_local_movto;
                   cnab.carteirasCnab = Business.getCarteirasCnab(cdEscola, cd_carteira_cnab, CarteiraCnabDataAccess.TipoConsultaCarteiraCnab.HAS_ATIVO).ToList();
                   cnab.produtos = BusinessCoordenacao.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_ATIVO_CURSO, null, null).ToList();
                   cnab.locaisMvto = BusinessFinan.getLocalMovimentoSomenteLeitura(cdEscola, 0, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_FILTRO_CNAB, cd_pessoa_usuario);
               }else
                   throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgRegNotEnc), null,
                                                                  CnabBusinessException.TipoErro.ERRO_REGISTRO_NAO_ENCONTRADO, false);
               retorno.retorno = cnab;
               HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
               return response;
           }
           catch (Exception ex)
           {
               return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
           }
       }

       [HttpComponentesAuthorize(Roles = "cnabb.i")]
       public HttpResponseMessage postInsertCnab(Cnab cnab)
       {
           ReturnResult retorno = new ReturnResult();
           try
           {
               int cdEscola = (int)this.ComponentesUser.CodEmpresa;
               ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
               configuraBusiness(new List<IGenericBusiness>() { Business });
               cnab.cd_usuario = (int)this.ComponentesUser.CodUsuario;
               cnab.dh_cadastro_cnab = DateTime.UtcNow;
               cnab.dt_emissao_cnab = cnab.dh_cadastro_cnab.Date;
               cnab.dt_inicial_vencimento = cnab.dt_inicial_vencimento.Date;
               cnab.dt_final_vencimento = cnab.dt_final_vencimento.Date;
               foreach (var titulo in cnab.TitulosCnab)
               {
                   titulo.dt_vencimento_titulo = titulo.dt_vencimento_titulo.HasValue ? titulo.dt_vencimento_titulo.Value.Date : titulo.dt_vencimento_titulo;
               }
               var cnabCad = Business.addCnab(cnab, cdEscola);
               retorno.retorno = cnabCad;
               if (cnabCad.cd_cnab <= 0)
                   retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
               else
                   retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

               HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
               configureHeaderResponse(response, null);
               return response;
           }
           catch (CnabBusinessException exe)
           {
               if (exe.tipoErro == CnabBusinessException.TipoErro.ERRO_DATA_INI_MENOR_DATA_ATUAL ||
                   exe.tipoErro == CnabBusinessException.TipoErro.ERRO_DATA_FINAL_MENOR_DATA_INICIAL ||
                   exe.tipoErro == CnabBusinessException.TipoErro.ERRO_TITULO_STATUSCNAB_DIFERENTE_INCIAL ||
                   exe.tipoErro == CnabBusinessException.TipoErro.ERRO_TIPO_CNAB_CANCELAMENTO_CARTEIRA_REGISTRADA ||
                   exe.tipoErro == CnabBusinessException.TipoErro.ERRO_PEDIDO_BAIXA_CARTEIRA_SEM_REGISTRO ||
                   exe.tipoErro == CnabBusinessException.TipoErro.ERRO_DATA_MIN_MAX_SMALLDATETIME)
                   return gerarLogException(exe.Message, retorno, logger, exe); 
               else
                   return gerarLogException(Messages.msgNotIncludReg, retorno, logger, exe);
           }
           catch (Exception ex)
           {
               return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
           }
       }

       [HttpComponentesAuthorize(Roles = "cnabb.a")]
       public HttpResponseMessage postUpdateCnab(Cnab cnab)
       {
           ReturnResult retorno = new ReturnResult();
           //throw new Exception("sdfgasdfa");
           try
           {
               ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
               configuraBusiness(new List<IGenericBusiness>() { Business });
               int cdEscola = (int)this.ComponentesUser.CodEmpresa;
               configuraBusiness(new List<IGenericBusiness>() { Business });
               cnab.cd_usuario = (int)this.ComponentesUser.CodUsuario;
               cnab.dh_cadastro_cnab = DateTime.UtcNow;
               cnab.dt_emissao_cnab = cnab.dh_cadastro_cnab.Date;
               cnab.dt_inicial_vencimento = cnab.dt_inicial_vencimento.Date;
               cnab.dt_final_vencimento = cnab.dt_final_vencimento.Date;
               var cnabEdit = Business.editCnab(cnab, cdEscola);
               retorno.retorno = cnabEdit;
               if (cnabEdit.cd_cnab <= 0)
                   retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
               else
                   retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

               HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
               configureHeaderResponse(response, null);
               return response;
           }
           catch (CnabBusinessException exe)
           {
               if (exe.tipoErro == CnabBusinessException.TipoErro.ERRO_DATA_INI_MENOR_DATA_ATUAL ||
                   exe.tipoErro == CnabBusinessException.TipoErro.ERRO_DATA_FINAL_MENOR_DATA_INICIAL ||
                   exe.tipoErro == CnabBusinessException.TipoErro.ERRO_NAO_ALTERA_CNAB_GERADO ||
                   exe.tipoErro == CnabBusinessException.TipoErro.ERRO_TITULO_STATUSCNAB_DIFERENTE_INCIAL ||
                   exe.tipoErro == CnabBusinessException.TipoErro.ERRO_DATA_MIN_MAX_SMALLDATETIME)
                   return gerarLogException(exe.Message, retorno, logger, exe);
               else
                   return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
           }
           catch (Exception ex)
           {
               return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
           }
       }

       [HttpComponentesAuthorize(Roles = "rtcnb.e")]
       public HttpResponseMessage postDeleteRetornoCnab(List<RetornoCNAB> cnabs) {
           ReturnResult retorno = new ReturnResult();
           try {
               var cd_escola = (int) this.ComponentesUser.CodEmpresa;
               ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
               configuraBusiness(new List<IGenericBusiness>() { Business });
               int[] cdCnabs = null;
               int i;
               // Pegando códigos da Turma
               if(cnabs != null && cnabs.Count() > 0) {
                   i = 0;
                   int[] cdCnabsCont = new int[cnabs.Count()];
                   foreach(var c in cnabs) {
                       cdCnabsCont[i] = c.cd_retorno_cnab;
                       i++;
                   }
                   cdCnabs = cdCnabsCont;
               }
               string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
               string pathRetornosEscola = caminho_relatorios + "/Retornos";
               var delCnabs = Business.deleteRetornosCnabs(cdCnabs, cd_escola, pathRetornosEscola);
               retorno.retorno = delCnabs;
               if(!delCnabs)
                   retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
               else
                   retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

               HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
               configureHeaderResponse(response, null);
               return response;
           }
           catch(CnabBusinessException exe) {
               if(exe.tipoErro == CnabBusinessException.TipoErro.ERRO_EXCLUIR_CNAB_FECHADO)
                   return gerarLogException(exe.Message, retorno, logger, exe);
               else
                   return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, exe);
           }
           catch(Exception ex) {
               return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
           }
       }

       [HttpComponentesAuthorize(Roles = "cnabb.e")]
       public HttpResponseMessage postDeleteCnab(List<Cnab> cnabs)
       {
           ReturnResult retorno = new ReturnResult();
           try
           {
               var cd_escola = (int)this.ComponentesUser.CodEmpresa;
               ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
               configuraBusiness(new List<IGenericBusiness>() { Business });
               int[] cdCnabs = null;
               int i;
               // Pegando códigos da Turma
               if (cnabs != null && cnabs.Count() > 0)
               {
                   i = 0;
                   int[] cdCnabsCont = new int[cnabs.Count()];
                   foreach (var c in cnabs)
                   {
                       cdCnabsCont[i] = c.cd_cnab;
                       i++;
                   }
                   cdCnabs = cdCnabsCont;
               }
               var delCnabs = Business.deleteCnabs(cdCnabs, cd_escola);
               retorno.retorno = delCnabs;
               if (!delCnabs)
                   retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
               else
                   retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

               HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
               configureHeaderResponse(response, null);
               return response;
           }
           catch (CnabBusinessException exe)
           {
               if (exe.tipoErro == CnabBusinessException.TipoErro.ERRO_EXCLUIR_CNAB_FECHADO)
                   return gerarLogException(exe.Message, retorno, logger, exe);
               else
                   return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, exe);
           }
           catch (Exception ex)
           {
               return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
           }
       }

       [HttpComponentesAuthorize(Roles = "locmv")]
       [HttpComponentesAuthorize(Roles = "tit")]
       [HttpComponentesAuthorize(Roles = "cnabb")]
       public HttpResponseMessage getTituloCnabEdit(int cd_titulo_cnab, int cd_cnab)
       {
           ReturnResult retorno = new ReturnResult();
           int cdEscola = (int)this.ComponentesUser.CodEmpresa;
           try
           {
               ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
               TituloCnab tituloCnab = Business.getTituloCnabEditView(cd_cnab, cd_titulo_cnab, cdEscola);
               retorno.retorno = tituloCnab;
               HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
               return response;
           }
           catch (Exception ex)
           {
               return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
           }
       }

       [HttpComponentesAuthorize(Roles = "tit")]
       [HttpComponentesAuthorize(Roles = "rtcnb")]
       public HttpResponseMessage getTituloRetornoCnabEdit(int cd_titulo_retorno_cnab)
       {
           ReturnResult retorno = new ReturnResult();
           try
           {
               int cdEscola = (int) this.ComponentesUser.CodEmpresa;
               ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
               TituloRetornoCNAB tituloCnab = Business.getTituloRetornoCnabEditView(cd_titulo_retorno_cnab, cdEscola);
               retorno.retorno = tituloCnab;
               HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
               return response;
           }
           catch (Exception ex)
           {
               return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
           }
       }

       //TO DO Deivid
       [HttpComponentesAuthorize(Roles = "cnabb")]
       [HttpComponentesAuthorize(Roles = "tit")]
       public HttpResponseMessage postSimulacaoDadosObrigTitulos(List<TituloCnab> titulosCnab)
       {
           ReturnResult retorno = new ReturnResult();
           try
           {
               int cd_empresa = (int)this.ComponentesUser.CodEmpresa;
               //var titulos = Business.getDadosAdicionaisTitulo(titulosCnab, cd_empresa);
               //retorno.retorno = titulos;
               if (null == null)
               {
                   retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
               };

               HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
               configureHeaderResponse(response, null);
               return response;

           }
           catch (Exception ex)
           {
               return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
           }
       }

       [HttpComponentesAuthorize(Roles = "cnabb")]
       public HttpResponseMessage getUrlRelatorioCnab(string sort, int direction, int cd_carteira, int cd_usuario, byte tipo_cnab, int status, string dtInicial,
                                                string dtFinal, bool emissao, bool vencimento, string nossoNumero, int? nro_contrato, bool icnab, bool iboleto, int cd_responsavel, int cd_aluno)
       {
           ReturnResult retorno = new ReturnResult();
           try
           {
               int cdEscola = (int)this.ComponentesUser.CodEmpresa;
               //Pega os parâmetros do usuário para criar a url do relatório:
               string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
               string parametros = strParametrosSort + "@cdEmpresa=" + cdEscola + "&@cd_carteira=" + cd_carteira + "&@cd_usuario=" + cd_usuario + "&@tipo_cnab=" + tipo_cnab
                   + "&@status=" + status + "&@dtInicial=" + dtInicial + "&@dtFinal=" + dtFinal + "&@emissao=" + emissao + "&@vencimento=" + vencimento + "&@nossoNumero=" + nossoNumero + "&@nro_contrato=" + nro_contrato
                   + "&@icnab= " + icnab + "&@iboleto=" + iboleto + "&@cd_responsavel=" + cd_responsavel + "&@cd_aluno=" + cd_aluno + "&"
                   + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório Cnab Boleto&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.CnabBoleto;
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

       [HttpComponentesAuthorize(Roles = "cnabb.e")]
       public HttpResponseMessage postExcluirCNABRegistrado(Cnab cnab)
       {
           ReturnResult retorno = new ReturnResult();
           try
           {
               int cdEscola = (int)this.ComponentesUser.CodEmpresa;
               ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
               IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
               configuraBusiness(new List<IGenericBusiness>() { Business });
               cnab.cd_usuario = (int)this.ComponentesUser.CodUsuario;
               bool masterGeral = BusinessEmpresa.VerificarMasterGeral(cnab.cd_usuario);
               var cnabCad = Business.deleteCnabsRegistrados(cnab, cdEscola, masterGeral);
               retorno.retorno = cnabCad;
               if (!cnabCad)
                   retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
               else
                   retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

               HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
               configureHeaderResponse(response, null);
               return response;
           }
           catch (CnabBusinessException exe)
           {
               if (exe.tipoErro == CnabBusinessException.TipoErro.ERRO_ALTERAR_EXCLUIR_CARTEIRA_HOMOLOGADA)
                   return gerarLogException(exe.Message, retorno, logger, exe);
               else
                   return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, exe);
           }
           catch (Exception ex)
           {
               return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
           }
       }

       [HttpPost]
       [HttpComponentesAuthorize(Roles = "cnabb.e")]
       public HttpResponseMessage deleteCnabRetornosProcessados(List<int> cnabs)
       {
           ReturnResult retorno = new ReturnResult();
           try
           {
               var cd_escola = (int)this.ComponentesUser.CodEmpresa;
               ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
               configuraBusiness(new List<IGenericBusiness>() { Business });
               IEscolaBusiness escolaBusiness = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
               IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
               IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();

               string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
               string pathRetornosEscola = caminho_relatorios + "/Retornos";
               
               var isSupervisor = BusinessUsuario.VerificarSupervisorByEscola(this.ComponentesUser.CodUsuario, cd_escola);
               var movimentoRetroativo = escolaBusiness.getParametroMovimentoRetroativo(cd_escola);
               bool masterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);

               var delCnabs = Business.deleteCnabRetornosProcessados(cnabs.ToArray(), cd_escola, pathRetornosEscola, isSupervisor, movimentoRetroativo, masterGeral);

               retorno.retorno = delCnabs;
               if (!delCnabs)
                   retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
               else
                   retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

               HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
               configureHeaderResponse(response, null);
               return response;
           }
           catch (CnabBusinessException exe)
           {
               if (exe.tipoErro == CnabBusinessException.TipoErro.ERRO_EXCLUIR_CNAB_FECHADO ||
                   exe.tipoErro == CnabBusinessException.TipoErro.ERRO_PERMISSAO_EXCLUIR_CNAB_REGISTRADO)
                   return gerarLogException(exe.Message, retorno, logger, exe);
               else
                   return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, exe);
           }
           catch (Exception ex)
           {
               return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
           }
       }

        #endregion

        #region Carteira CNAB
        [HttpComponentesAuthorize(Roles = "ccnab")]
        public HttpResponseMessage getCarteiraCnabSearch(string nome, bool inicio, int banco, int status)
        {
            try
            {
                if (nome == null)
                {
                    nome = String.Empty;
                }
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                var retorno = Business.getCarteiraCnabSearch(parametros, nome, inicio, banco, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "ccnab")]
        public HttpResponseMessage getUrlRelatorioCarteiraCnab(string sort, int direction, string nome, bool inicio, int banco, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@nome=" + nome + "&@inicio=" + inicio + "&@banco=" + banco + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Banco&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.CarteiraCnabSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }

        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "ccnab.a")]
        public HttpResponseMessage postAlterarCarteira(CarteiraCnab carteiraCnab)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                var carteiraRet = Business.putCarteira(carteiraCnab);
                retorno.retorno = carteiraRet;
                if (carteiraRet.cd_banco <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (CnabBusinessException exe)
            {
                if (exe.tipoErro == CnabBusinessException.TipoErro.ERRO_ALTERAR_EXCLUIR_CARTEIRA_HOMOLOGADA)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "ccnab.i")]
        public HttpResponseMessage postInsertCarteira(CarteiraCnab carteiraCnab)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                var carteiraRet = Business.postInsertCarteira(carteiraCnab);
                retorno.retorno = carteiraRet;
                if (carteiraRet.cd_carteira_cnab <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "ccnab.e")]
        public HttpResponseMessage postDeleteCarteira(List<CarteiraCnab> carteiraCnab)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                var deleted = Business.deleteAllCarteira(carteiraCnab);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (CnabBusinessException exe)
            {
                if (exe.tipoErro == CnabBusinessException.TipoErro.ERRO_ALTERAR_EXCLUIR_CARTEIRA_HOMOLOGADA)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "ccnab")]
        public HttpResponseMessage getCarteiraByBanco(int? cdLocalMovot, int cdBanco)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                var ge = Business.getCarteiraByBanco(cdLocalMovot, cdBanco);
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

        [HttpComponentesAuthorize(Roles = "banc")]
        public HttpResponseMessage getBancobyId(int cdBanco, int? cdLocalMovto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness BusinessFinan = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                var banco = BusinessFinan.getBancobyId(cdBanco);
                banco.CarteirasCnab = Business.getCarteiraByBanco(cdLocalMovto, cdBanco).ToList();
                retorno.retorno = banco;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        #endregion 

        #region Retorno CNAB
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "rtcnb")]
        public HttpResponseMessage getRetornoCNABSearch(int cd_carteira, int cd_usuario, int status, string descRetorno, string dtInicial, string dtFinal, string nossoNumero, int cd_responsavel, int cd_aluno)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                //Se receber o usuário com código zero mas não for usuario administrador, pegar o usuário do login
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                var retorno = Business.searchRetornoCNAB(parametros, cd_carteira, cd_usuario, status, descRetorno, dtaInicial, dtaFinal, nossoNumero, cdEscola, cd_responsavel, cd_aluno);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "rtcnb")]
        public HttpResponseMessage getUrlRelatorioRetornoCNAB(string sort, int direction, int cd_carteira, int cd_usuario, int status, string descRetorno, string dtInicial, string dtFinal, int cd_responsavel, int cd_aluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                //Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cdEmpresa=" + cdEscola + "&@cd_carteira=" + cd_carteira + "&@cd_usuario=" + cd_usuario + "&@status=" + status
                    + "&@descRetorno=" + descRetorno + "&@dtInicial=" + dtInicial + "&@dtFinal=" + dtFinal + "&@cd_responsavel=" + cd_responsavel +"&@cd_aluno=" + cd_aluno + "&"
                    + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório Retorno CNAB Boleto&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.RetornoCNAB;
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
        [HttpComponentesAuthorize(Roles = "rtcnb")]
        public HttpResponseMessage getUrlRelatorioTitulosCNAB(int cd_cnab)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                //Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "cd_cnab=" + cd_cnab.ToString();
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

        [HttpComponentesAuthorize(Roles = "rtcnb")]
        public HttpResponseMessage getUrlRelatorioRetornoTitulosCNAB(int cd_retorno_cnab)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                //Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório Títulos CNAB Boleto&cd_retorno_cnab=" + cd_retorno_cnab.ToString();
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
        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpComponentesAuthorize(Roles = "usu")]
        [HttpComponentesAuthorize(Roles = "rtcnb")]
        public HttpResponseMessage getComponentesPesquisaRetCNAB()
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                Cnab cnab = new Cnab();
                cnab.cd_usuario = (int)this.ComponentesUser.CodUsuario;
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                cnab.usuarios = Business.getUsuariosRetCNAB(cdEscola).ToList();
                cnab.carteirasCnab = Business.getCarteirasRetCNAB(cdEscola).ToList();
                retorno.retorno = cnab;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "rtcnb")]
        public HttpResponseMessage postInsertRetornoCNAB(RetornoCNAB retornoCNAB)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                retornoCNAB.cd_usuario = (int)this.ComponentesUser.CodUsuario;
                retornoCNAB.dt_cadastro_cnab = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(retornoCNAB.no_arquivo_retorno))
                {
                    string extensaoArq = retornoCNAB.no_arquivo_retorno.Substring(retornoCNAB.no_arquivo_retorno.Length - 4);

                    if (!String.IsNullOrEmpty(retornoCNAB.nm_banco) && (retornoCNAB.nm_banco == ((int)Cnab.Bancos.Sicred + "")))
                    {
                        if (extensaoArq.ToLower() != ".txt" && extensaoArq.ToLower() != ".ret" && extensaoArq.ToLower() != ".dat" && extensaoArq.ToLower() != ".crt" && extensaoArq.ToLower() != ".rst")
                            throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtensaoNaoSuportadaRetorno, null,
                                CnabBusinessException.TipoErro.ERRO_TIPO_ARQUIVO_NAO_SUPORTADO, false);
                    }
                    else
                    {
                        if (extensaoArq.ToLower() != ".txt" && extensaoArq.ToLower() != ".ret" && extensaoArq.ToLower() != ".dat" && extensaoArq.ToLower() != ".rst")
                            throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtensaoNaoSuportadaRetorno, null,
                                CnabBusinessException.TipoErro.ERRO_TIPO_ARQUIVO_NAO_SUPORTADO, false);
                    }

                }
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(retornoCNAB.cd_usuario);
                string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
                string pathRetornosEscola = Path.Combine(caminho_relatorios, "Retornos");
                var cnabCad = Business.addRetornoCNAB(retornoCNAB, pathRetornosEscola, cdEscola, masterGeral);
                retorno.retorno = cnabCad;
                if (cnabCad.cd_retorno_cnab <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (CnabBusinessException exe)
            {
                if (exe.tipoErro == CnabBusinessException.TipoErro.ERRO_RETORNO_COM_NOME_INFORMADO ||
                    exe.tipoErro == CnabBusinessException.TipoErro.ERRO_RETORNO_JA_EXISTE)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, exe);
            }
            catch (ArgumentException ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "cnabb")]
        public HttpResponseMessage postUpdateRetornoCNAB(RetornoCNAB retornoCNAB)
        {
            ReturnResult retorno = new ReturnResult();
            //throw new Exception("sdfgasdfa");
            try
            {
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                retornoCNAB.cd_usuario = (int)this.ComponentesUser.CodUsuario;
                if (!string.IsNullOrEmpty(retornoCNAB.no_arquivo_retorno))
                {
                    string extensaoArq = retornoCNAB.no_arquivo_retorno.Substring(retornoCNAB.no_arquivo_retorno.Length - 4);
                    if (!String.IsNullOrEmpty(retornoCNAB.nm_banco) && (retornoCNAB.nm_banco == ((int)Cnab.Bancos.Sicred + "")))
                    {
                        if (extensaoArq.ToLower() != ".txt" && extensaoArq.ToLower() != ".ret" && extensaoArq.ToLower() != ".dat" && extensaoArq.ToLower() != ".crt" && extensaoArq.ToLower() != ".rst")
                            throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtensaoNaoSuportadaRetorno, null,
                                CnabBusinessException.TipoErro.ERRO_TIPO_ARQUIVO_NAO_SUPORTADO, false);
                        
                    }
                    else
                    {
                        if (extensaoArq.ToLower() != ".txt" && extensaoArq.ToLower() != ".ret" && extensaoArq.ToLower() != ".dat" && extensaoArq.ToLower() != ".rst")
                            throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtensaoNaoSuportadaRetorno, null,
                                CnabBusinessException.TipoErro.ERRO_TIPO_ARQUIVO_NAO_SUPORTADO, false);
                    }
                    

                }
                retornoCNAB.dt_cadastro_cnab = DateTime.UtcNow;
                string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
                string pathContratosEscola = caminho_relatorios + "/Retornos";
                var cnabRetEdit = Business.editRetornoCNAB(retornoCNAB, pathContratosEscola, cdEscola);
                retorno.retorno = cnabRetEdit;
                if (cnabRetEdit.cd_retorno_cnab <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (CnabBusinessException exe)
            {
                if (exe.tipoErro == CnabBusinessException.TipoErro.ERRO_RETORNO_JA_EXISTE ||
                    exe.tipoErro == CnabBusinessException.TipoErro.ERRO_RETORNO_COM_NOME_INFORMADO ||
                    exe.tipoErro == CnabBusinessException.TipoErro.ERRO_RETORNO_JA_FECHADO)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "rtcnb")]
        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getComponentesByRetCnabEdit(int cd_retorno)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            int cd_pessoa_usuario = 0;//MMC
            try
            {
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                ICoordenacaoBusiness BusinessCoordenacao = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IFinanceiroBusiness BusinessFinan = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                RetornoCNAB retornoCNAB = Business.getRetCnabEditView(cd_retorno, cdEscola);
                int cd_carteira_cnab = 0;
                if (retornoCNAB.cd_local_movto > 0)
                    cd_carteira_cnab = retornoCNAB.cd_local_movto;
                retornoCNAB.carteirasRetornoCNAB = Business.getCarteirasCnab(cdEscola, cd_carteira_cnab, CarteiraCnabDataAccess.TipoConsultaCarteiraCnab.HAS_ATIVO).ToList();
                retornoCNAB.produtos = BusinessCoordenacao.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_ATIVO_CURSO, null, null).ToList();
                retornoCNAB.locaisMvto = BusinessFinan.getLocalMovimentoSomenteLeitura(cdEscola, 0, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_FILTRO_CNAB, cd_pessoa_usuario);
                retorno.retorno = retornoCNAB;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage postPesquisaTituloCnabRet(TituloUI titulo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICnabBusiness Business = (ICnabBusiness)base.instanciarBusiness<ICnabBusiness>();
                var titulos = Business.searchTituloCnabGradeRet(titulo);
                retorno.retorno = titulos;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        #endregion 
    }
}