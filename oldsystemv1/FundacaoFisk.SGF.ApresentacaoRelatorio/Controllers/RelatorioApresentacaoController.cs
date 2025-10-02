using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using BoletoNet;
using Componentes.GenericController;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.CNAB.Business;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.CNAB.DataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using log4net;
using Newtonsoft.Json;
using FundacaoFisk.SGF.Utils;
using Componentes.GenericBusiness.Comum;
using System.Runtime.InteropServices;
using DALC4NET;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using Microsoft.Reporting.WebForms;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using WebGrease.Css.Extensions;
using System.Collections;
using System.Security.Policy;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.ApresentacaoRelatorio.Controllers
{
    [AllowAnonymous]
    public class RelatorioApresentacaoController : ComponentesMVCController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(RelatorioApresentacaoController));
        DataTable dtHistoricoTurma;
        DataTable dtHistoricoTurmaConceito;
        DataTable dtHistoricoEstagioConceito;
        DataTable dtHistoricoTurmaAvaliacao;
        DataTable dtHistoricoAvaliacaoTurma;
        DataTable dtHistoricoEstagio;
        DataTable dtHistoricoAvaliacaoEstagio;
        DataTable dtHistoricoEventoAula1;
        DataTable dtHistoricoEventoAula2;
        DataTable dtHistoricoTitulo;
        DataTable dtHistoricoObs;
        DataTable dtHistoricoAtividade;
        DataTable dtHistoricoFollow;
        DataTable dtHistoricoItem;
        private IBoletoBusiness BusinessBoleto { get; set; }
        private IFiscalBusiness BusinessFiscal { get; set; }
        private ICnabBusiness BusinessCnab { get; set; }
        private IPessoaCartaDataAccess PessoaCartaDataAccess { get; set; }
        private List<ParcelasReciboAgrupadoUI> parcelasReciboAgrupado = new List<ParcelasReciboAgrupadoUI>();


        public RelatorioApresentacaoController(IBoletoBusiness businessBoleto, ICnabBusiness businessCnab, IFiscalBusiness businessFiscal, IPessoaCartaDataAccess pessoaCartaDataAccess)
        {
            if (businessBoleto == null || businessCnab == null || pessoaCartaDataAccess == null)
                throw new ArgumentNullException("business");
            BusinessBoleto = businessBoleto;
            BusinessCnab = businessCnab;
            BusinessFiscal = businessFiscal;
            PessoaCartaDataAccess = pessoaCartaDataAccess;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult postGerarPedidoBaixa(string parametros)
        {
            return postGerarRemessas(parametros, true);
        }

        [AllowAnonymous]
        public ActionResult postGerarRemessa(string parametros)
        {
            return postGerarRemessas(parametros, false);
        }

        [AllowAnonymous]
        public ActionResult emitirNFS(string parametros)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cd_escola = 0;
                List<int> cd_movimentos = new List<int>();
                byte id_tipo_movimento = 0;
                DateTime? dtHoraAtualServAplic = null;
                string parms = HttpUtility.UrlDecode(parametros, System.Text.Encoding.UTF8).Replace(" ", "+");
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if ("cd_escola".Equals(parametrosHash[0]))
                            cd_escola = int.Parse(parametrosHash[1] + "");
                        else if ("cd_movimentos".Equals(parametrosHash[0]))
                            cd_movimentos = JsonConvert.DeserializeObject<List<int>>(parametrosHash[1]);
                        else if ("id_tipo_movimento".Equals(parametrosHash[0]))
                            id_tipo_movimento = byte.Parse(parametrosHash[1] + "");
                        else if ("PDataHoraAtual".Equals(parametrosHash[0]))
                            dtHoraAtualServAplic = DateTime.Parse(parametrosHash[1] + "");
                    }
                }

                //Verifica se o relatório já expirou:
                DateTime dtHoraAtualRelat = DateTime.Now;
                TimeSpan t = dtHoraAtualRelat - dtHoraAtualServAplic.Value;
                int timeout = int.Parse(ConfigurationManager.AppSettings["timeoutRelatorio"] + "");
                if (t.TotalSeconds > timeout)
                {
                    string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoSessaoRelatExp) + "&stackTrace=");
                }

                int nm_nota_fiscal = 0;
                XmlDocument xmlDoc = BusinessFiscal.emitirNFS(cd_escola, cd_movimentos, ref nm_nota_fiscal);

                if (xmlDoc != null)
                {
                    retorno.AddMensagem(Messages.msgProccessNFSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                    MemoryStream stream = new MemoryStream();
                    xmlDoc.Save(stream);
                    stream.Position = 0;

                    //return this.Content(fs.ToArray().ToString(), "application/pdf");
                    Response.ContentType = "application/xml";
                    Response.AddHeader("content-disposition", "attachment;filename=XML-" + nm_nota_fiscal + "-" + String.Format("{0:dd_MM_yyyy_HH_mm_ss}", DateTime.Now) + ".xml");
                    Response.Buffer = true;
                    Response.Clear();
                    byte[] buf = stream.ToArray();
                    Response.OutputStream.Write(buf, 0, buf.Length);
                    Response.OutputStream.Flush();
                    Response.End();

                    //Deleta o arquivo:
                    //System.IO.File.Delete(caminho_relatorios + "/" + file_name + ".pdf");

                    return this.Content(stream.ToArray().ToString(), "application/xml");
                    //return new RenderJsonActionResult { Result = retorno };
                }
                else
                {
                    retorno.AddMensagem(Messages.msgErrorProccessNF, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                    return new RenderJsonActionResult { Result = retorno };
                }
            }
            catch (FiscalBusinessException exe)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];

                logger.Warn(Messages.msgErrorProccessNF, exe);
                if (exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_PROCESSAR_NF)
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(exe.Message));
                else
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(Messages.msgErrorProccessNF));
            }
            catch (Exception ex)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                string stackTrace = HttpUtility.UrlEncode(ex.Message + ex.StackTrace + ex.InnerException);

                //TODO: Melhorar o código para um método post, já que método get tem limitação de caracteres. Enquanto isso, visualizar o erro através do log de relatorios configurado no web.config.
                //Trunca o tamanho da url para o método get:
                if (stackTrace.Length > 1450)
                    stackTrace = stackTrace.Substring(0, 1250);
                logger.Error(Messages.msgErrorProccessNF, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(Messages.msgErrorProccessNF) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
            }
        }

        [AllowAnonymous]
        public ActionResult emitirNF(string parametros)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cd_escola = 0;
                int cd_movimento = 0;
                byte id_tipo_movimento = 0;
                DateTime? dtHoraAtualServAplic = null;
                string parms = HttpUtility.UrlDecode(parametros, System.Text.Encoding.UTF8).Replace(" ", "+");
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if ("cd_escola".Equals(parametrosHash[0]))
                            cd_escola = int.Parse(parametrosHash[1] + "");
                        else if ("cd_movimento".Equals(parametrosHash[0]))
                            cd_movimento = int.Parse(parametrosHash[1] + "");
                        else if ("id_tipo_movimento".Equals(parametrosHash[0]))
                            id_tipo_movimento = byte.Parse(parametrosHash[1] + "");
                        else if ("PDataHoraAtual".Equals(parametrosHash[0]))
                            dtHoraAtualServAplic = DateTime.Parse(parametrosHash[1] + "");
                    }
                }

                //Verifica se o relatório já expirou:
                DateTime dtHoraAtualRelat = DateTime.Now;
                TimeSpan t = dtHoraAtualRelat - dtHoraAtualServAplic.Value;
                int timeout = int.Parse(ConfigurationManager.AppSettings["timeoutRelatorio"] + "");
                if (t.TotalSeconds > timeout)
                {
                    string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoSessaoRelatExp) + "&stackTrace=");
                }

                int nm_nota_fiscal = 0;
                XmlDocument xmlDoc = BusinessFiscal.emitirNF(cd_escola, cd_movimento, id_tipo_movimento, ref nm_nota_fiscal);

                if (xmlDoc != null)
                {
                    retorno.AddMensagem(Messages.msgProccessNFSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                    MemoryStream stream = new MemoryStream();
                    xmlDoc.Save(stream);
                    stream.Position = 0;

                    //return this.Content(fs.ToArray().ToString(), "application/pdf");
                    Response.ContentType = "application/xml";
                    Response.AddHeader("content-disposition", "attachment;filename=XML-" + nm_nota_fiscal + "-" + String.Format("{0:dd_MM_yyyy_HH_mm_ss}", DateTime.Now) + ".xml");
                    Response.Buffer = true;
                    Response.Clear();
                    byte[] buf = stream.ToArray();
                    Response.OutputStream.Write(buf, 0, buf.Length);
                    Response.OutputStream.Flush();
                    Response.End();

                    //Deleta o arquivo:
                    //System.IO.File.Delete(caminho_relatorios + "/" + file_name + ".pdf");

                    return this.Content(stream.ToArray().ToString(), "application/xml");
                    //return new RenderJsonActionResult { Result = retorno };
                }
                else
                {
                    retorno.AddMensagem(Messages.msgErrorProccessNF, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                    return new RenderJsonActionResult { Result = retorno };
                }
            }
            catch (FiscalBusinessException exe)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];

                logger.Warn(Messages.msgErrorProccessNF, exe);
                if (exe.tipoErro == FiscalBusinessException.TipoErro.ERRO_PROCESSAR_NF)
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(exe.Message));
                else
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(Messages.msgErrorProccessNF));
            }
            catch (Exception ex)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                string stackTrace = HttpUtility.UrlEncode(ex.Message + ex.StackTrace + ex.InnerException);

                //TODO: Melhorar o código para um método post, já que método get tem limitação de caracteres. Enquanto isso, visualizar o erro através do log de relatorios configurado no web.config.
                //Trunca o tamanho da url para o método get:
                if (stackTrace.Length > 1450)
                    stackTrace = stackTrace.Substring(0, 1250);
                logger.Error(Messages.msgErrorProccessNF, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(Messages.msgErrorProccessNF) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
            }
        }

        [AllowAnonymous]
        public ActionResult postGerarRemessas(string parametros, bool pedido_baixa)
        {
            ReturnResult retornoErrors = new ReturnResult();
            try
            {
               
                int cd_escola = 0;
                int cd_carteira_cnab = 0;
                int cd_cnab = 0;
                DateTime? dtHoraAtualServAplic = null;
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if ("cd_escola".Equals(parametrosHash[0]))
                            cd_escola = int.Parse(parametrosHash[1] + "");
                        else if ("cd_cnab".Equals(parametrosHash[0]))
                            cd_cnab = int.Parse(parametrosHash[1] + "");
                        else if ("cd_carteira_cnab".Equals(parametrosHash[0]))
                            cd_carteira_cnab = int.Parse(parametrosHash[1] + "");
                        else if ("PDataHoraAtual".Equals(parametrosHash[0]))
                            dtHoraAtualServAplic = DateTime.Parse(parametrosHash[1] + "");
                    }
                }

                //Verifica se o relatório já expirou:
                DateTime dtHoraAtualRelat = DateTime.Now;
                TimeSpan t = dtHoraAtualRelat - dtHoraAtualServAplic.Value;
                int timeout = int.Parse(ConfigurationManager.AppSettings["timeoutRelatorio"] + "");
                if (t.TotalSeconds > timeout)
                {
                    string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoSessaoRelatExp) + "&stackTrace=");
                }

                BusinessCnab.verificarCarteiraRegistrada(cd_escola, cd_cnab, cd_carteira_cnab, retornoErrors);

                //Se não expirou, gera a remessa:
                //Verifica se todos os cnabs foram gerados:
                Int32[] codigos = new Int32[1];
                codigos[0] = cd_cnab;

                Int32[] tipos_cnab = new Int32[2];
                tipos_cnab[0] = (byte)Cnab.TipoCnab.GERAR_BOLETOS;
                tipos_cnab[1] = (byte)Cnab.TipoCnab.PEDIDO_BAIXA;

                BusinessCnab.verificarGerouCnab(cd_escola, codigos, tipos_cnab, (byte)Cnab.StatusCnab.FECHADO, false, retornoErrors);
                BusinessCnab.verificarCdContratoCnab(cd_escola, cd_cnab, retornoErrors);

                FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController relatorioController = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();
                Cnab cnab = BusinessCnab.getGerarRemessa(cd_escola, cd_cnab);
                List<TituloCnab> titulos = cnab.TitulosCnab.ToList();
                Boletos boletos = new Boletos();

                short nro_banco = short.Parse(cnab.LocalMovimento.Banco.nm_banco);
                ConfiguradorBoletoBancario configBoleto = new ConfiguradorBoletoBancario(nro_banco);

                Cedente cedente = configBoleto.gerarCedenteDefault(cnab);
                //if(nro_banco == (int)Bancos.Unicred)
                IBanco banco = new BoletoNet.Banco(nro_banco);
                for (int i = 0; i < titulos.Count; i++)
                {

                    switch ((Bancos)nro_banco)
                    {
                        #region Itau
                        case Bancos.Itau:
                            EspecieDocumento_Itau espDocItau = new EspecieDocumento_Itau();
                            IEspecieDocumento especieDocumentoItau = new EspecieDocumento_Itau(espDocItau.getCodigoEspecieByEnum(EnumEspecieDocumento_Itau.DuplicataMercantil));
                            Boleto boleto = configBoleto.gerarBoletoDefault(titulos[i], especieDocumentoItau, (int)Bancos.Itau, cedente, true, null, retornoErrors);
                            //Configura as instruções:
                            boleto.Instrucoes = new List<IInstrucao>();
                            Instrucao item = new Instrucao((int)Bancos.Itau);
                            if (!pedido_baixa && titulos[i].nm_dias_protesto > 0)
                            {
                                Instrucao_Itau instrucao = new Instrucao_Itau((int)EnumInstrucoes_Itau.ProtestarAposNDiasCorridos, titulos[i].nm_dias_protesto);
                                instrucao.Codigo = (int)EnumInstrucoes_Itau.Protestar;
                                instrucao.QuantidadeDias = titulos[i].nm_dias_protesto;
                                boleto.Instrucoes.Add(instrucao);
                            }
                            else
                            {

                                item.Codigo = 94; //MENSAGENS NOS BOLETOS COM 40 POSIÇÕES
                                boleto.Instrucoes.Add(item);
                                boleto.Instrucoes[0].QuantidadeDias = 10;
                            }
                            item = new Instrucao((int)Bancos.Itau);
                            item.Codigo = 94; //MENSAGENS NOS BOLETOS COM 40 POSIÇÕES
                            boleto.Instrucoes.Add(item);
                            boleto.Instrucoes[1].QuantidadeDias = 10;
                            boleto.NumeroDocumento = titulos[i].cd_titulo + "";
                            boleto.Banco = banco;

                            // Configurações de pedido de baixa:
                            if (pedido_baixa)
                            {
                                boleto.Remessa = new Remessa();
                                boleto.Remessa.CodigoOcorrencia = "02";

                                boleto.ValorBoleto = 0;
                                boleto.JurosMora = 0;
                                boleto.PercMulta = 0;
                                boleto.PercJurosMora = 0;
                                boleto.ValorDesconto = 0;
                            }
                            boleto.DataMulta = boleto.DataVencimento;

                            boletos.Add(boleto);
                            break;
                        #endregion
                        #region Inter
                        case Bancos.Inter:
                            EspecieDocumento_Inter espDocInter = new EspecieDocumento_Inter();
                            IEspecieDocumento especieDocumentoInter = new EspecieDocumento_Inter(espDocInter.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.DuplicataMercantil));
                             boleto = configBoleto.gerarBoletoDefault(titulos[i], especieDocumentoInter, (int)Bancos.Inter, cedente, true, null, retornoErrors);
                            //Configura as instruções:
                            boleto.Instrucoes = new List<IInstrucao>();
                             item = new Instrucao((int)Bancos.Inter);
                            if (!pedido_baixa && titulos[i].nm_dias_protesto > 0)
                            {
                                Instrucao_Inter instrucao = new Instrucao_Inter((int)EnumInstrucoes_Inter.ProtestarAposNDiasCorridos, titulos[i].nm_dias_protesto);
                                instrucao.Codigo = (int)EnumInstrucoes_Inter.Protestar;
                                instrucao.QuantidadeDias = titulos[i].nm_dias_protesto;
                                boleto.Instrucoes.Add(instrucao);
                            }
                            else
                            {

                                item.Codigo = 94; //MENSAGENS NOS BOLETOS COM 40 POSIÇÕES
                                boleto.Instrucoes.Add(item);
                                boleto.Instrucoes[0].QuantidadeDias = 10;
                            }
                            item = new Instrucao((int)Bancos.Inter);
                            item.Codigo = 94; //MENSAGENS NOS BOLETOS COM 40 POSIÇÕES
                            boleto.Instrucoes.Add(item);
                            boleto.Instrucoes[1].QuantidadeDias = 10;
                            boleto.NumeroDocumento = titulos[i].cd_titulo + "";
                            boleto.Banco = banco;

                            // Configurações de pedido de baixa:
                            if (pedido_baixa)
                            {
                                boleto.Remessa = new Remessa();
                                boleto.Remessa.CodigoOcorrencia = "02";

                                boleto.ValorBoleto = 0;
                                boleto.JurosMora = 0;
                                boleto.PercMulta = 0;
                                boleto.PercJurosMora = 0;
                                boleto.ValorDesconto = 0;
                            }
                            // DATA DA MULTA: Durante a homologação com o banco inter, foi solicitado a utilização da data de vencimento + 1
                            if (!boleto.DataMulta.Equals(DateTime.MinValue))
                                boleto.DataMulta = boleto.DataVencimento.AddDays(1);

                            boletos.Add(boleto);
                            break;
                        #endregion
                        #region Santander
                        case Bancos.Santander:
                            EspecieDocumento_Santander espDocSantander = new EspecieDocumento_Santander();
                            IEspecieDocumento especieDocumentoSantander = new EspecieDocumento_Santander(espDocSantander.getCodigoEspecieByEnum(EnumEspecieDocumento_Santander.DuplicataMercantil));
                            boleto = configBoleto.gerarBoletoDefault(titulos[i], especieDocumentoSantander, (int)Bancos.Santander, cedente, true, null, retornoErrors);
                            boleto.Sacado.Nome = titulos[i].Titulo.Pessoa.no_pessoa;
                            boleto.Instrucoes = new List<IInstrucao>();
                            boleto.JurosMora = Decimal.Round(boleto.JurosMora, 2);
                            if (!pedido_baixa && titulos[i].nm_dias_protesto > 0)
                            {
                                Instrucao_Santander instrucao = new Instrucao_Santander(titulos[i].nm_dias_protesto);
                                instrucao.Codigo = (int)EnumInstrucoes_Santander.Protestar;
                                instrucao.QuantidadeDias = titulos[i].nm_dias_protesto;
                                boleto.Instrucoes.Add(instrucao);
                            }
                            if (titulos[i].tx_mensagem_cnab != null)
                            {
                                foreach (string s in titulos[i].tx_mensagem_cnab.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    boleto.Instrucoes.Add(new Instrucao_Santander() { Descricao = s });
                                }
                            }

                            boleto.Banco = banco;
                            boleto.Remessa = new Remessa() { CodigoOcorrencia = "01" };
                            if (pedido_baixa)
                            {
                                boleto.Remessa = new Remessa();
                                boleto.Remessa.CodigoOcorrencia = "02";

                                boleto.ValorBoleto = 0;
                                boleto.JurosMora = 0;
                                boleto.PercMulta = 0;
                                boleto.PercJurosMora = 0;
                                boleto.ValorDesconto = 0;
                            }

                            // DATA DA MULTA: Caso seja invalida ou não informada, será assumida a data do vencimento.Se informada uma data válida, a incidência da multa, será após o dia informado no campo multa. ”
                            boleto.DataMulta = boleto.DataVencimento;

                            boletos.Add(boleto);
                            break;
                        #endregion
                        #region HSBC
                        case Bancos.HSBC:
                            EspecieDocumento_HSBC espDocHSBC = new EspecieDocumento_HSBC();
                            IEspecieDocumento especieDocumentoHSBC;
                            if (cnab.LocalMovimento.CarteiraCnab.id_impressao == (byte)CarteiraCnab.TipoImpressao.BANCO)
                                especieDocumentoHSBC = new EspecieDocumento_HSBC(espDocHSBC.getCodigoEspecieByEnum(EnumEspecieDocumento_HSBC.DuplicataMercantil));
                            else
                                especieDocumentoHSBC = new EspecieDocumento_HSBC(espDocHSBC.getCodigoEspecieByEnum(EnumEspecieDocumento_HSBC.CobrancaEmissaoCliente));
                            boleto = configBoleto.gerarBoletoDefault(titulos[i], especieDocumentoHSBC, (int)Bancos.HSBC, cedente, true, null, retornoErrors);
                            boleto.Banco = banco;
                            boleto.Remessa = new Remessa() { CodigoOcorrencia = "01" };
                            if (pedido_baixa)
                            {
                                boleto.Remessa = new Remessa();
                                boleto.Remessa.CodigoOcorrencia = "02";
                            }
                            boletos.Add(boleto);
                            break;
                        #endregion
                        #region Unicred/Bradesco
                        case Bancos.Unicred:
                        case Bancos.Bradesco:
                            EspecieDocumento_Bradesco espDocBradesco = new EspecieDocumento_Bradesco();
                            IEspecieDocumento especieDocumentoBradesco = new EspecieDocumento_Bradesco(espDocBradesco.getCodigoEspecieByEnum(EnumEspecieDocumento_Bradesco.DuplicataMercantil));
                            if (nro_banco == (int)Bancos.Unicred)
                                cedente = configBoleto.gerarCedenteDefaultPersonalizado(titulos[i].LocalMovimento, false);
                            boleto = configBoleto.gerarBoletoDefault(titulos[i], especieDocumentoBradesco, (int)Bancos.Bradesco, cedente, true, null, retornoErrors);
                            boleto.NumeroDocumento = titulos[i].cd_titulo + "";
                            if (nro_banco == (int)Bancos.Unicred)
                                boleto.NumeroDocumento = titulos[i].nm_parcela_e_titulo;

                            boleto.Instrucoes = new List<IInstrucao>();

                            if (!pedido_baixa && titulos[i].nm_dias_protesto > 0)
                            {
                                Instrucao instrucao = new Instrucao((int)Bancos.Bradesco);
                                instrucao.Codigo = (int)EnumInstrucoes_Bradesco.ProtestarAposNDiasCorridos;
                                instrucao.Descricao = (new Instrucao_Bradesco((int)EnumInstrucoes_Bradesco.NaoReceberAposNDias, titulos[i].nm_dias_protesto)).Descricao;
                                instrucao.QuantidadeDias = titulos[i].nm_dias_protesto;
                                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.Bradesco, ref instrucao, cnab.LocalMovimento.CarteiraCnab.nm_colunas);
                                boleto.Instrucoes.Add(instrucao);
                            }
                            if (titulos[i].tx_mensagem_cnab != null)
                            {
                                foreach (string s in titulos[i].tx_mensagem_cnab.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    boleto.Instrucoes.Add(new Instrucao((int)Bancos.Bradesco) { Descricao = s });
                                }
                            }
                            boleto.Banco = banco;
                            boleto.Remessa = new Remessa() { CodigoOcorrencia = "01" };
                            // Configurações de pedido de baixa:
                            if (pedido_baixa)
                            {
                                boleto.Remessa = new Remessa();
                                boleto.Remessa.CodigoOcorrencia = "02";

                                boleto.ValorBoleto = 0;
                                boleto.JurosMora = 0;
                                boleto.PercMulta = 0;
                                boleto.PercJurosMora = 0;
                                boleto.ValorDesconto = 0;
                            }
                            boletos.Add(boleto);
                            boleto.NumeroControle = titulos[i].cd_titulo + "";
                            if (!String.IsNullOrEmpty(titulos[i].Titulo.Pessoa.EnderecoPrincipal.enderecoCompletoBoleto))
                            {
                                string retorno = "";
                                EnderecoSGF endrc = titulos[i].Titulo.Pessoa.EnderecoPrincipal;
                                if (endrc.Logradouro != null)
                                {
                                    if (endrc.TipoLogradouro != null && !String.IsNullOrEmpty(endrc.TipoLogradouro.no_tipo_logradouro))
                                        retorno += endrc.TipoLogradouro.no_tipo_logradouro + " ";
                                    retorno += endrc.Logradouro.no_localidade;
                                    if (!String.IsNullOrEmpty(endrc.dc_num_endereco))
                                        retorno += "  " + endrc.dc_num_endereco;
                                    if (!String.IsNullOrEmpty(endrc.dc_compl_endereco))
                                        retorno += " " + endrc.dc_compl_endereco;
                                    boleto.Sacado.Endereco.End = retorno;
                                }
                            }
                            break;
                        #endregion
                        #region DinariPay
                        case Bancos.DinariPay:
                            EspecieDocumento_DinariPay espDocDinariPay = new EspecieDocumento_DinariPay();
                            IEspecieDocumento especieDocumentoDinariPay = new EspecieDocumento_DinariPay(espDocDinariPay.getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.DuplicataMercantil));
                            if (nro_banco == (int)Bancos.Unicred)
                                cedente = configBoleto.gerarCedenteDefaultPersonalizado(titulos[i].LocalMovimento, false);
                            boleto = configBoleto.gerarBoletoDefault(titulos[i], especieDocumentoDinariPay, (int)Bancos.DinariPay, cedente, true, null, retornoErrors);
                            boleto.NumeroDocumento = titulos[i].cd_titulo + "";
                            if (nro_banco == (int)Bancos.Unicred)
                                boleto.NumeroDocumento = titulos[i].nm_parcela_e_titulo;

                            boleto.Instrucoes = new List<IInstrucao>();

                            if (!pedido_baixa && titulos[i].nm_dias_protesto > 0)
                            {
                                Instrucao instrucao = new Instrucao((int)Bancos.DinariPay);
                                instrucao.Codigo = (int)EnumInstrucoes_DinariPay.ProtestarAposNDiasCorridos;
                                instrucao.Descricao = (new Instrucao_DinariPay((int)EnumInstrucoes_DinariPay.NaoReceberAposNDias, titulos[i].nm_dias_protesto)).Descricao;
                                instrucao.QuantidadeDias = titulos[i].nm_dias_protesto;
                                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.DinariPay, ref instrucao, cnab.LocalMovimento.CarteiraCnab.nm_colunas);
                                boleto.Instrucoes.Add(instrucao);
                            }
                            if (titulos[i].tx_mensagem_cnab != null)
                            {
                                foreach (string s in titulos[i].tx_mensagem_cnab.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    boleto.Instrucoes.Add(new Instrucao((int)Bancos.DinariPay) { Descricao = s });
                                }
                            }
                            boleto.Banco = banco;
                            boleto.Remessa = new Remessa() { CodigoOcorrencia = "01" };
                            // Configurações de pedido de baixa:
                            if (pedido_baixa)
                            {
                                boleto.Remessa = new Remessa();
                                boleto.Remessa.CodigoOcorrencia = "02";

                                boleto.ValorBoleto = 0;
                                boleto.JurosMora = 0;
                                boleto.PercMulta = 0;
                                boleto.PercJurosMora = 0;
                                boleto.ValorDesconto = 0;
                            }
                            boletos.Add(boleto);
                            boleto.NumeroControle = titulos[i].cd_titulo + "";
                            if (!String.IsNullOrEmpty(titulos[i].Titulo.Pessoa.EnderecoPrincipal.enderecoCompletoBoleto))
                            {
                                string retorno = "";
                                EnderecoSGF endrc = titulos[i].Titulo.Pessoa.EnderecoPrincipal;
                                if (endrc.Logradouro != null)
                                {
                                    if (endrc.TipoLogradouro != null && !String.IsNullOrEmpty(endrc.TipoLogradouro.no_tipo_logradouro))
                                        retorno += endrc.TipoLogradouro.no_tipo_logradouro + " ";
                                    retorno += endrc.Logradouro.no_localidade;
                                    if (!String.IsNullOrEmpty(endrc.dc_num_endereco))
                                        retorno += "  " + endrc.dc_num_endereco;
                                    if (!String.IsNullOrEmpty(endrc.dc_compl_endereco))
                                        retorno += " " + endrc.dc_compl_endereco;
                                    boleto.Sacado.Endereco.End = retorno;
                                }
                            }
                            break;
                        #endregion
                        #region CrediSIS
                        case Bancos.CrediSIS:
                            EspecieDocumento_CrediSIS espDocCrediSIS = new EspecieDocumento_CrediSIS();
                            IEspecieDocumento especieDocumentoCrediSIS;
                            especieDocumentoCrediSIS = new EspecieDocumento_CrediSIS(espDocCrediSIS.getCodigoEspecieByEnum(EnumEspecieDocumento_CrediSIS.DuplicataServicoIndicacao));
                            boleto = configBoleto.gerarBoletoDefault(titulos[i], especieDocumentoCrediSIS, (int)Bancos.CrediSIS, cedente, true, null, retornoErrors);
                            boleto.DataLimitePagamento = boleto.DataVencimento.AddDays(30);
                            boleto.Sacado.Endereco.Numero = titulos[i].Titulo.Pessoa.EnderecoPrincipal.dc_num_endereco + "";
                            boleto.Sacado.Endereco.End = titulos[i].Titulo.Pessoa.EnderecoPrincipal.enderecoBoletoSemNumero + "";
                            boleto.Sacado.Nome = boleto.Sacado.Nome.Replace("\t", "");
                            boleto.Banco = banco;
                            boleto.Remessa = new Remessa() { CodigoOcorrencia = "01" };
                            boleto.NumeroDocumento = titulos[i].cd_titulo + "";
                            boleto.NumeroParcela = titulos[i].Titulo != null && titulos[i].Titulo.nm_parcela_titulo.HasValue ? (int)titulos[i].Titulo.nm_parcela_titulo : 0;
                            if (!pedido_baixa && titulos[i].nm_dias_protesto > 0)
                            {
                                Instrucao instrucao = new Instrucao((int)Bancos.CrediSIS);
                                instrucao.Codigo = (int)EnumInstrucoes_CrediSIS.ProtestarAposNDiasCorridos;
                                instrucao.Descricao = (new Instrucao_CrediSIS((int)EnumInstrucoes_CrediSIS.ProtestarAposNDiasCorridos, titulos[i].nm_dias_protesto)).Descricao;
                                instrucao.QuantidadeDias = titulos[i].nm_dias_protesto;
                                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.CrediSIS, ref instrucao, cnab.LocalMovimento.CarteiraCnab.nm_colunas);
                                boleto.Instrucoes.Add(instrucao);
                            }
                            if (pedido_baixa)
                            {
                                boleto.Remessa = new Remessa();
                                boleto.Remessa.CodigoOcorrencia = "02";
                                boleto.ValorBoleto = 0;
                                boleto.JurosMora = 0;
                                boleto.PercMulta = 0;
                                boleto.PercJurosMora = 0;
                                boleto.ValorDesconto = 0;
                            }
                            boletos.Add(boleto);
                            break;
                        case Bancos.BancodoBrasil:
                            if (string.IsNullOrEmpty(cedente.ContaBancaria.DigitoAgencia))
                                cedente.ContaBancaria.DigitoAgencia = "1";
                            EspecieDocumento_BancoBrasil espDoc = new EspecieDocumento_BancoBrasil();
                            IEspecieDocumento especieDocumento = new EspecieDocumento_BancoBrasil(espDoc.getCodigoEspecieByEnum(EnumEspecieDocumento_BancoBrasil.DuplicataMercantil));
                            boleto = configBoleto.gerarBoletoDefault(titulos[i], especieDocumento, (int)Bancos.BancodoBrasil, cedente, true, null, retornoErrors);
                            boleto.Banco = banco;
                            if (!pedido_baixa && titulos[i].nm_dias_protesto > 0)
                            {
                                Instrucao instrucao = new Instrucao((int)Bancos.BancodoBrasil);
                                instrucao.Codigo = (int)EnumInstrucoes_BancoBrasil.ProtestarAposNDiasCorridos;
                                instrucao.Descricao = (new Instrucao_BancoBrasil((int)EnumInstrucoes_BancoBrasil.NaoReceberAposNDias, titulos[i].nm_dias_protesto)).Descricao;
                                instrucao.QuantidadeDias = titulos[i].nm_dias_protesto;
                                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.BancodoBrasil, ref instrucao, cnab.LocalMovimento.CarteiraCnab.nm_colunas);
                                boleto.Instrucoes.Add(instrucao);
                            }
                            if (pedido_baixa)
                            {
                                boleto.Remessa = new Remessa();
                                boleto.Remessa.CodigoOcorrencia = "02";

                                boleto.ValorBoleto = 0;
                                boleto.JurosMora = 0;
                                boleto.PercMulta = 0;
                                boleto.PercJurosMora = 0;
                                boleto.ValorDesconto = 0;
                            }
                            boletos.Add(boleto);
                            break;
                        #endregion CrediSIS
                        #region Caixa
                        case Bancos.Caixa:
                            EspecieDocumento_Caixa espDocCaixa = new EspecieDocumento_Caixa();
                            IEspecieDocumento especieDocumentoCaixa = new EspecieDocumento_Caixa(espDocCaixa.getCodigoEspecieByEnum(EnumEspecieDocumento_Caixa.DuplicataMercantil));
                            boleto = configBoleto.gerarBoletoDefault(titulos[i], especieDocumentoCaixa, (int)Bancos.Caixa, cedente, true, null, retornoErrors);
                            boleto.Sacado.Nome = titulos[i].Titulo.Pessoa.no_pessoa;
                            boleto.Remessa = new Remessa();
                            boleto.Remessa.CodigoOcorrencia = "01";
                            boleto.Banco = banco;
                            boletos.Add(boleto);
                            if (!pedido_baixa && titulos[i].nm_dias_protesto > 0)
                            {
                                Instrucao instrucao = new Instrucao((int)Bancos.Caixa);
                                instrucao.Codigo = (int)EnumInstrucoes_Caixa.Protestar;
                                instrucao.Descricao = (new Instrucao_Caixa((int)EnumInstrucoes_Caixa.NaoReceberAposNDias, titulos[i].nm_dias_protesto)).Descricao;
                                instrucao.QuantidadeDias = titulos[i].nm_dias_protesto;
                                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.Caixa, ref instrucao, cnab.LocalMovimento.CarteiraCnab.nm_colunas);
                                boleto.Instrucoes.Add(instrucao);
                            }
                            if (pedido_baixa)
                            {
                                //boleto.Remessa = new Remessa();
                                boleto.Remessa.CodigoOcorrencia = "02";

                                boleto.ValorBoleto = 0;
                                boleto.JurosMora = 0;
                                boleto.PercMulta = 0;
                                boleto.PercJurosMora = 0;
                                boleto.ValorDesconto = 0;
                            }

                            // DATA DA MULTA: Durante a homologação com o banco caixa, foi solicitado a utilização da data de vencimento + 1
                            if (!boleto.DataMulta.Equals(DateTime.MinValue))
                                boleto.DataMulta = boleto.DataVencimento.AddDays(1);

                            break;
                        #endregion
                        #region Sicred
                        case Bancos.Sicred:
                            EspecieDocumento_Sicredi espDocSicredi = new EspecieDocumento_Sicredi();
                            IEspecieDocumento especieDocumentoSicredi = new EspecieDocumento_Sicredi(espDocSicredi.getCodigoEspecieByEnum(EnumEspecieDocumento_Sicredi.DuplicataMercantilIndicacao));
                            boleto = configBoleto.gerarBoletoDefault(titulos[i], especieDocumentoSicredi, (int)Bancos.Sicred, cedente, true, null, retornoErrors);
                            boleto.Sacado.Nome = titulos[i].Titulo.Pessoa.no_pessoa;
                            boleto.Remessa = new Remessa();
                            boleto.Remessa.CodigoOcorrencia = "01";
                            boleto.Remessa.TipoDocumento = "A"; // A = 'A' - SICREDI com Registro //      C1 = 'C' - SICREDI sem Registro Impressão Completa pelo Sicredi //      C2 = 'C' - SICREDI sem Registro Pedido de bloquetos pré-impressos
                            boleto.Banco = banco;
                            // Configurações de pedido de baixa:
                            if (!pedido_baixa && titulos[i].nm_dias_protesto > 0)
                            {
                                Instrucao instrucao = new Instrucao((int)Bancos.Sicred);
                                instrucao.Codigo = (int)EnumInstrucoes_Sicredi.PedidoProtesto;
                                instrucao.QuantidadeDias = titulos[i].nm_dias_protesto;
                                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.Sicred, ref instrucao, cnab.LocalMovimento.CarteiraCnab.nm_colunas);
                                boleto.Instrucoes.Add(instrucao);
                            }
                            if (pedido_baixa)
                            {
                                boleto.Remessa.CodigoOcorrencia = "02";

                                boleto.ValorBoleto = 0;
                                boleto.JurosMora = 0;
                                boleto.PercMulta = 0;
                                boleto.PercJurosMora = 0;
                                boleto.ValorDesconto = 0;
                            }

                            //Configuração do nosso número. Na tela do local de movimento o usuário deve configurar apenas o byte de geração + o número sequencial de 5 posições.
                            boleto.NossoNumero = DateTime.UtcNow.ToString("yy") + Utils.Utils.FormatCode(boleto.NossoNumero, 5);

                            boleto.Cedente.ContaBancaria.OperacaConta = Utils.Utils.FormatCode(titulos[i].LocalMovimento.nm_op_conta + "", 2);
                            boletos.Add(boleto);
                            break;
                        #endregion
                        #region Sicoob
                        case Bancos.Sicoob:
                            titulos[i] = TituloCnab.toUperCaseDadosRemessa(titulos[i]);
                            if (cedente.Codigo.Length < 7)
                                cedente.Codigo = cedente.Codigo.PadLeft(7, '0');
                            EspecieDocumento_Sicoob espDocSicoob = new EspecieDocumento_Sicoob();
                            IEspecieDocumento especieDocumentoSicoob = new EspecieDocumento_Sicoob(espDocSicoob.getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.DuplicataServicoIndicacao));
                            boleto = configBoleto.gerarBoletoDefault(titulos[i], especieDocumentoSicoob, (int)Bancos.Sicoob, cedente, true, null, retornoErrors);
                            boleto.NumeroParcela = titulos[i].Titulo != null && titulos[i].Titulo.nm_parcela_titulo.HasValue ? (int)titulos[i].Titulo.nm_parcela_titulo : 0;
                            boleto.Sacado.Nome = titulos[i].Titulo.Pessoa.no_pessoa;
                            boleto.Banco = banco;
                            boleto.Remessa = new Remessa() { CodigoOcorrencia = "01" };
                            if (boleto.PercJurosMora > 0)
                                boleto.PercJurosMora = decimal.Round(boleto.PercJurosMora * 30, 2);
                            boleto.ApenasRegistrar = true;
                            //c.DigitoCedente = tituloCnab.LocalMovimento.nm_digito_cedente.Value;
                            if (!pedido_baixa && titulos[i].nm_dias_protesto > 0)
                            {
                                Instrucao instrucao = new Instrucao((int)Bancos.Sicoob);
                                instrucao.Codigo = (int)EnumInstrucoes_Sicoob.Protestar;
                                instrucao.Descricao = (new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.Protestar, titulos[i].nm_dias_protesto)).Descricao;
                                instrucao.QuantidadeDias = titulos[i].nm_dias_protesto;
                                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.Sicoob, ref instrucao, cnab.LocalMovimento.CarteiraCnab.nm_colunas);
                                boleto.Instrucoes.Add(instrucao);
                            }
                            boleto.Cedente.DigitoCedente = 0;
                            if (pedido_baixa)
                            {
                                boleto.Remessa = new Remessa();
                                boleto.Remessa.CodigoOcorrencia = "02";
                            }
                            
                            if (cnab.LocalMovimento.CarteiraCnab.nm_colunas == (int)CarteiraCnab.TipoColunas.CNAB240)
                            {
                                boleto.NossoNumero = Utils.Utils.FormatCode(boleto.NossoNumero + getDigitoNossoNumeroSicoob(boleto) + "", 10) + Utils.Utils.FormatCode(titulos[i].Titulo.nm_parcela_titulo + "", 2) +
                                                     Utils.Utils.FormatCode("01", 2) + "4" + Utils.Utils.FormatCode("", 5);
                            }
                            boletos.Add(boleto);
                            break;
                        #endregion
                        #region Uniprime
                        case Bancos.Uniprime:
                            EspecieDocumento_Uniprime espDocUniprime = new EspecieDocumento_Uniprime();
                            IEspecieDocumento especieDocumentoUniprime = new EspecieDocumento_Uniprime(espDocUniprime.getCodigoEspecieByEnum(EnumEspecieDocumento_Uniprime.DuplicataMercantil));
                            boleto = configBoleto.gerarBoletoDefault(titulos[i], especieDocumentoUniprime, (int)Bancos.Uniprime, cedente, true, null, retornoErrors);
                            //boleto.NumeroDocumento = titulos[i].cd_titulo + "";
                            boleto.NumeroDocumento = titulos[i].nm_parcela_e_titulo;
                            boleto.Instrucoes = new List<IInstrucao>();

                            if (!pedido_baixa && titulos[i].nm_dias_protesto > 0)
                            {
                                Instrucao instrucao = new Instrucao((int)Bancos.Uniprime);
                                instrucao.Codigo = (int)EnumInstrucoes_Uniprime.ProtestarAposNDiasCorridos;
                                instrucao.Descricao = (new Instrucao_Uniprime((int)EnumInstrucoes_Uniprime.NaoReceberAposNDias, titulos[i].nm_dias_protesto)).Descricao;
                                instrucao.QuantidadeDias = titulos[i].nm_dias_protesto;
                                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.Uniprime, ref instrucao, cnab.LocalMovimento.CarteiraCnab.nm_colunas);
                                boleto.Instrucoes.Add(instrucao);
                            }
                            if (titulos[i].tx_mensagem_cnab != null)
                            {
                                foreach (string s in titulos[i].tx_mensagem_cnab.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    boleto.Instrucoes.Add(new Instrucao((int)Bancos.Uniprime) { Descricao = s });
                                }
                            }
                            boleto.Banco = banco;
                            boleto.Remessa = new Remessa() { CodigoOcorrencia = "01" };
                            // Configurações de pedido de baixa:
                            if (pedido_baixa)
                            {
                                boleto.Remessa = new Remessa();
                                boleto.Remessa.CodigoOcorrencia = "02";

                                boleto.ValorBoleto = 0;
                                boleto.JurosMora = 0;
                                boleto.PercMulta = 0;
                                boleto.PercJurosMora = 0;
                                boleto.ValorDesconto = 0;
                            }
                            boletos.Add(boleto);
                            boleto.NumeroControle = titulos[i].cd_titulo + "";
                            if (!String.IsNullOrEmpty(titulos[i].Titulo.Pessoa.EnderecoPrincipal.enderecoCompletoBoleto))
                            {
                                string retorno = "";
                                EnderecoSGF endrc = titulos[i].Titulo.Pessoa.EnderecoPrincipal;
                                if (endrc.Logradouro != null)
                                {
                                    if (endrc.TipoLogradouro != null && !String.IsNullOrEmpty(endrc.TipoLogradouro.no_tipo_logradouro))
                                        retorno += endrc.TipoLogradouro.no_tipo_logradouro + " ";
                                    retorno += endrc.Logradouro.no_localidade;
                                    if (!String.IsNullOrEmpty(endrc.dc_num_endereco))
                                        retorno += "  " + endrc.dc_num_endereco;
                                    if (!String.IsNullOrEmpty(endrc.dc_compl_endereco))
                                        retorno += " " + endrc.dc_compl_endereco;
                                    boleto.Sacado.Endereco.End = retorno;
                                }
                            }
                            break;
                        #endregion
                        default:
                            retornoErrors.AddMensagem("Não existe suporte de remessa para este banco de número " + nro_banco + ".", null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                            break;
                            //throw new Exception("Não existe suporte de remessa para este banco de número " + nro_banco + ".");
                    }
                }

                System.IO.MemoryStream ms;
                if (cnab.LocalMovimento.CarteiraCnab.nm_colunas == (int)CarteiraCnab.TipoColunas.CNAB240)
                    ms = configBoleto.gerarArquivoCNAB240(banco, cedente, boletos, retornoErrors);
                else
                    ms = configBoleto.gerarArquivoCNAB400(banco, cedente, boletos, retornoErrors);

                if (retornoErrors.MensagensWeb != null && retornoErrors.MensagensWeb.Count > 0)
                {
                    string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    string stackTrace = HttpUtility.UrlEncode(string.Join("<br>", retornoErrors.MensagensWeb.Select(x => x.mensagem).ToArray()));

                    if (stackTrace.Length > 1450)
                        stackTrace = stackTrace.Substring(0, 1250);
                    logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroGerarRemessa, null);
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroGerarRemessa) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
                }

                //return this.Content(fs.ToArray().ToString(), "application/pdf");
                Response.ContentType = "text/plain";
                Response.AddHeader("content-disposition", "attachment;filename=" + cnab.no_arquivo_remessa);
                Response.Buffer = true;
                Response.Clear();
                byte[] buf = ms.ToArray();
                Response.OutputStream.Write(buf, 0, buf.Length);
                Response.OutputStream.Flush();
                Response.End();

                //Deleta o arquivo:
                //System.IO.File.Delete(caminho_relatorios + "/" + file_name + ".pdf");

                return this.Content(ms.ToArray().ToString(), "text/plain");
                //return new RenderJsonActionResult { Result = retorno };
            }
            catch (CnabBusinessException exm)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];

                if (retornoErrors.MensagensWeb != null && retornoErrors.MensagensWeb.Count > 0)
                {
                   string stackTrace = HttpUtility.UrlEncode(string.Join("<br>", retornoErrors.MensagensWeb.Select(x => x.mensagem).ToArray()));

                   if (stackTrace.Length > 1450)
                       stackTrace = stackTrace.Substring(0, 1250);
                   logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroGerarRemessa, exm);
                   return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroGerarRemessa) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
                }
                else
                {
                    logger.Warn(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exm);
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(exm.Message));
                }

                
            }
            catch (Exception ex)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                string stackTrace = "";
                if (retornoErrors.MensagensWeb != null && retornoErrors.MensagensWeb.Count > 0)
                {
                     stackTrace = HttpUtility.UrlEncode(string.Join("<br>", retornoErrors.MensagensWeb.Select(x => x.mensagem).ToArray()));

                }
                stackTrace += HttpUtility.UrlEncode(((retornoErrors.MensagensWeb != null && retornoErrors.MensagensWeb.Count > 0) ? "<br>" : "") + ex.Message + ex.StackTrace + ex.InnerException);




                //TODO: Melhorar o código para um método post, já que método get tem limitação de caracteres. Enquanto isso, visualizar o erro através do log de relatorios configurado no web.config.
                //Trunca o tamanho da url para o método get:
                if (stackTrace.Length > 1450)
                    stackTrace = stackTrace.Substring(0, 1250);
                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroGerarRemessa, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroGerarRemessa) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
            }
        }

        [AllowAnonymous]
        public ActionResult getImprimirContrato(string parametros)
        {
            string caminho_relatorios = ConfigurationManager.AppSettings["caminhoContent"];
            string file_name = Guid.NewGuid().ToString();
            bool abortou_requisicao = false;
            try
            {
                int cd_escola = 0;
                int cd_contrato = 0;
                bool conta_segura = false;
                DateTime? dtHoraAtualServAplic = null;
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if ("cd_escola".Equals(parametrosHash[0]))
                            cd_escola = int.Parse(parametrosHash[1] + "");
                        else if ("cd_contrato".Equals(parametrosHash[0]))
                            cd_contrato = int.Parse(parametrosHash[1] + "");
                        else if ("conta_segura".Equals(parametrosHash[0]))
                            conta_segura = Boolean.Parse(parametrosHash[1] + "");
                        else if ("PDataHoraAtual".Equals(parametrosHash[0]))
                            dtHoraAtualServAplic = DateTime.Parse(parametrosHash[1] + "");
                    }
                }

                //Verifica se o relatório já expirou:
                DateTime dtHoraAtualRelat = DateTime.Now;
                TimeSpan t = dtHoraAtualRelat - dtHoraAtualServAplic.Value;
                int timeout = int.Parse(ConfigurationManager.AppSettings["timeoutRelatorio"] + "");
                if (t.TotalSeconds > timeout)
                {
                    string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoSessaoRelatExp) + "&stackTrace=");
                }

                //Se não expirou, imprime o relatório:
                FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController relatorioController = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();
                int? nm_cliente_integracao = null;
                int? nm_contrato = null;
                string no_aluno = string.Empty;

                //System.IO.MemoryStream ms = relatorioController.postImprimirContrato(cd_escola, cd_contrato, conta_segura, caminho_relatorios, file_name, ref nm_cliente_integracao, ref nm_contrato, ref no_aluno);
                System.IO.MemoryStream ms = relatorioController.gerarDocWordOpenXml(cd_escola, cd_contrato, conta_segura, caminho_relatorios, file_name, ref nm_cliente_integracao, ref nm_contrato, ref no_aluno);

                string nome_arquivo = no_aluno;
                if (nm_contrato.HasValue)
                    nome_arquivo = nm_contrato.Value + " - " + nome_arquivo;
                if (nm_cliente_integracao.HasValue)
                    nome_arquivo = nm_cliente_integracao.Value + " - " + nome_arquivo;

                //return this.Content(fs.ToArray().ToString(), "application/pdf");
                Response.ContentType = "application/word";
                Response.AddHeader("content-disposition", "attachment;filename=" + nome_arquivo + ".docx");
                Response.Buffer = true;
                Response.Clear();
                byte[] buf = ms.ToArray();
                Response.OutputStream.Write(buf, 0, buf.Length);
                abortou_requisicao = true;
                Response.OutputStream.Flush();
                Response.End();
                ms.Dispose();
                //Deleta o arquivo:
                //System.IO.File.Delete(caminho_relatorios + "/" + file_name + ".pdf");

                return this.Content(ms.ToArray().ToString(), "application/word");
                //return new RenderJsonActionResult { Result = retorno };
            }

            catch (COMException com)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                
                string stackTrace = HttpUtility.UrlEncode(com.Message + com.StackTrace + com.InnerException);

                if (stackTrace.Length > 1450)
                    stackTrace = stackTrace.Substring(0, 1250);
                
                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgErrorCOMInteropWordContrato, com);

                if (com.ErrorCode == -2147417846)
                {
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErrorCOMInteropWordContrato) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
                }
                else
                {
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(com.Message) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
                }
                
            }
            catch (MatriculaBusinessException exm)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];

                logger.Warn(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exm);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(exm.Message));
            }
            catch (SecretariaBusinessException exs)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];

                logger.Warn(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exs);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(exs.Message));
            }
            catch (FinanceiroBusinessException exf)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];

                logger.Warn(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exf);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(exf.Message));
            }
            catch (CoordenacaoBusinessException excc)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];

                logger.Warn(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, excc);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(excc.Message));
            }
            catch (Exception ex)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                string stackTrace = HttpUtility.UrlEncode(ex.Message + ex.StackTrace + ex.InnerException);

                //TODO: Melhorar o código para um método post, já que método get tem limitação de caracteres. Enquanto isso, visualizar o erro através do log de relatorios configurado no web.config.
                //Trunca o tamanho da url para o método get:
                if (stackTrace.Length > 1450)
                    stackTrace = stackTrace.Substring(0, 1250);
                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado, ex);
                if (!abortou_requisicao)
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
                else
                    return null;
            }
            finally
            {
                //Remove o arquivo PDF:
                ManipuladorArquivo.removerArquivo(caminho_relatorios + "/" + file_name + ".pdf");
            }
        }

        [AllowAnonymous]
        public ActionResult VisualizadorBoleto(string parametros)
        {
            try
            {
                int cd_escola = 0;
                Int32[] cd_cnab = new Int32[1];
                bool id_mostrar_3_boletos = false;
                DateTime? dtHoraAtualServAplic = null;
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");

                logger.Debug("INICIOU VizualizadorBoleto");
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if ("cd_escola".Equals(parametrosHash[0]))
                            cd_escola = int.Parse(parametrosHash[1] + "");
                        else if ("cd_cnab".Equals(parametrosHash[0]))
                            cd_cnab = Array.ConvertAll((parametrosHash[1] + "").Split(';'), s => Int32.Parse(s));
                        else if ("id_mostrar_3_boletos".Equals(parametrosHash[0]))
                            id_mostrar_3_boletos = bool.Parse(parametrosHash[1]);
                        else if ("PDataHoraAtual".Equals(parametrosHash[0]))
                            dtHoraAtualServAplic = DateTime.Parse(parametrosHash[1] + "");
                    }
                }

                //Verifica se o boleto já expirou:
                DateTime dtHoraAtualRelat = DateTime.Now;
                TimeSpan t = dtHoraAtualRelat - dtHoraAtualServAplic.Value;
                int timeout = int.Parse(ConfigurationManager.AppSettings["timeoutRelatorio"] + "");
                if (t.TotalSeconds > timeout)
                {
                    string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoSessaoRelatExp) + "&stackTrace=");
                }

                logger.Debug("início getTitulosCnabBoletoByCnabs");
                List<TituloCnab> titulos = BusinessBoleto.getTitulosCnabBoletoByCnabs(cd_escola, cd_cnab, null).ToList();
                logger.Debug("início getTitulosCnabBoletoByCnabs");

                gerarBoletos(cd_escola, cd_cnab, titulos, false, id_mostrar_3_boletos);

                logger.Debug("FINALIZOU VizualizadorBoleto");
                return View();
            }
            catch (CnabBusinessException cex)
            {
                string url = "";
                url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                logger.Warn(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroImprimirBoletos, cex.InnerException);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(cex.Message) + "&stackTrace=");
            }
            catch (Exception ex)
            {
                string url = "";
                if (ex.InnerException != null && "NotImplementedException".Equals(ex.InnerException.GetType().Name))
                {
                    url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    logger.Warn(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroImprimirBoletos, ex.InnerException);
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(ex.InnerException.Message) + "&stackTrace=");
                }

                url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                string stackTrace = HttpUtility.UrlEncode(ex.Message + ex.StackTrace + ex.InnerException);

                //TODO: Melhorar o código para um método post, já que método get tem limitação de caracteres. Enquanto isso, visualizar o erro através do log de relatorios configurado no web.config.
                //Trunca o tamanho da url para o método get:
                if (stackTrace.Length > 1450)
                    stackTrace = stackTrace.Substring(0, 1250);
                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroImprimirBoletos, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroImprimirBoletos) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
            }
        }

        [AllowAnonymous]
        public ActionResult VisualizadorTitulosBoleto(string parametros)
        {
            try
            {
                int cd_escola = 0;
                Int32[] cd_titulo_cnab = new Int32[1];
                DateTime? dtHoraAtualServAplic = null;
                bool id_mostrar_3_boletos = false;
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if ("cd_escola".Equals(parametrosHash[0]))
                            cd_escola = int.Parse(parametrosHash[1] + "");
                        else if ("cd_titulos_cnab".Equals(parametrosHash[0]))
                            cd_titulo_cnab = Array.ConvertAll((parametrosHash[1] + "").Split(';'), s => Int32.Parse(s));
                        else if ("id_mostrar_3_boletos".Equals(parametrosHash[0]))
                            id_mostrar_3_boletos = bool.Parse(parametrosHash[1]);
                        else if ("PDataHoraAtual".Equals(parametrosHash[0]))
                            dtHoraAtualServAplic = DateTime.Parse(parametrosHash[1] + "");
                    }
                }

                //Verifica se o boleto já expirou:
                DateTime dtHoraAtualRelat = DateTime.Now;
                TimeSpan t = dtHoraAtualRelat - dtHoraAtualServAplic.Value;
                int timeout = int.Parse(ConfigurationManager.AppSettings["timeoutRelatorio"] + "");
                if (t.TotalSeconds > timeout)
                {
                    string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoSessaoRelatExp) + "&stackTrace=");
                }
                List<TituloCnab> titulos = BusinessBoleto.getTitulosCnabBoletoByTitulosCnab(cd_escola, cd_titulo_cnab).ToList();

                gerarBoletos(cd_escola, cd_titulo_cnab, titulos, true, id_mostrar_3_boletos);

                return View();
            }
            catch (CnabBusinessException cex)
            {
                string url = "";
                url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                logger.Warn(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroImprimirBoletos, cex.InnerException);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(cex.Message) + "&stackTrace=");
            }
            catch (Exception ex)
            {
                string url = "";
                if (ex.InnerException != null && "NotImplementedException".Equals(ex.InnerException.GetType().Name))
                {
                    url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    logger.Warn(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroImprimirBoletos, ex.InnerException);
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(ex.InnerException.Message) + "&stackTrace=");
                }
                url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                string stackTrace = HttpUtility.UrlEncode(ex.Message + ex.StackTrace + ex.InnerException);

                //TODO: Melhorar o código para um método post, já que método get tem limitação de caracteres. Enquanto isso, visualizar o erro através do log de relatorios configurado no web.config.
                //Trunca o tamanho da url para o método get:
                if (stackTrace.Length > 1450)
                    stackTrace = stackTrace.Substring(0, 1250);
                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroImprimirBoletos, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroImprimirBoletos) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
            }
        }

        private List<ConfiguradorBoletoBancario> gerarBoletos(int cd_escola, Int32[] cd_titulo_cnab, List<TituloCnab> titulos, bool is_titulo, bool mostrar_3_boletos_pagina)
        {
            List<ConfiguradorBoletoBancario> retorno = new List<ConfiguradorBoletoBancario>();

            //Se não expirou, imprime o boleto:
            //Verifica se todos os cnabs foram gerados:
            Int32[] tipos_cnab = new Int32[2];
            tipos_cnab[0] = (byte)Cnab.TipoCnab.GERAR_BOLETOS;
            logger.Debug("início verificarGerouCnab");
            BusinessCnab.verificarGerouCnab(cd_escola, cd_titulo_cnab, tipos_cnab, (byte)Cnab.StatusCnab.FECHADO, is_titulo, null);
            logger.Debug("fim verificarGerouCnab");
            StringBuilder sbBoleto = new StringBuilder();

            FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController relatorioController = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();
            for (int i = 0; i < titulos.Count; i++)
            {
                titulos[i].id_mostrar_3_boletos_pagina = mostrar_3_boletos_pagina;
                short nro_banco = short.Parse(titulos[i].LocalMovimento.Banco.nm_banco);
                ConfiguradorBoletoBancario configBoleto = new ConfiguradorBoletoBancario(nro_banco);

                if (titulos[i].id_mostrar_3_boletos_pagina)
                {
                    if (i % 3 == 0)
                        sbBoleto.Append("<div style='page-break-before: always'>");
                }
                else
                    sbBoleto.Append("<div style='page-break-before: always'>");
                switch ((Bancos)nro_banco)
                {
                    case Bancos.HSBC:
                        sbBoleto.Append(configBoleto.HSBC(titulos[i]));
                        break;
                    case Bancos.Caixa:
                        sbBoleto.Append(configBoleto.Caixa(titulos[i]));
                        break;
                    case Bancos.Unicred:
                    case Bancos.Bradesco:
                        if (nro_banco == (int)Bancos.Unicred)
                        {
                            configBoleto = new ConfiguradorBoletoBancario((int)Bancos.Unicred);
                            nro_banco = (int)Bancos.Unicred;
                            sbBoleto.Append(configBoleto.BradescoUnicred(titulos[i]));
                        }
                        else
                            sbBoleto.Append(configBoleto.Bradesco(titulos[i]));

                        break;
                    case Bancos.DinariPay:
                        sbBoleto.Append(configBoleto.DinariPay(titulos[i]));
                        break;
                    case Bancos.Santander:
                        sbBoleto.Append(configBoleto.Santander(titulos[i]));
                        break;
                    case Bancos.BancodoBrasil:
                        sbBoleto.Append(configBoleto.BancodoBrasil(titulos[i]));
                        break;
                    case Bancos.CrediSIS:
                        sbBoleto.Append(configBoleto.CrediSIS(titulos[i]));
                        break;
                    case Bancos.Itau:
                        sbBoleto.Append(configBoleto.Itau(titulos[i]));
                        break;
                    case Bancos.Inter:
                        sbBoleto.Append(configBoleto.Inter(titulos[i]));
                        break;
                    case Bancos.Sicoob:
                        sbBoleto.Append(configBoleto.Sicoob(titulos[i]));
                        break;
                    case Bancos.Sicred:
                        sbBoleto.Append(configBoleto.Sicred(titulos[i]));
                        break;
                    case Bancos.Uniprime:
                        sbBoleto.Append(configBoleto.Uniprime(titulos[i]));
                        break;
                    /*case Bancos.Banrisul:
                        ViewBag.Boleto += configBoleto.Banrisul();
                        break;
                    case Bancos.Basa:
                        ViewBag.Boleto += configBoleto.Basa();
                        break;
                    case Bancos.BRB:
                        ViewBag.Boleto += configBoleto.BRB();
                        break;
                    case Bancos.Real:
                        ViewBag.Boleto += configBoleto.Real();
                        break;
                    case Bancos.Safra:
                        ViewBag.Boleto += configBoleto.Safra();
                        break;
                    case Bancos.Sudameris:
                        ViewBag.Boleto += configBoleto.Sudameris();
                        break;
                    case Bancos.Unibanco:
                        ViewBag.Boleto += configBoleto.Unibanco();
                        break;*/
                    default:
                        sbBoleto.Append("Não existe suporte de boleto para este banco de número " + nro_banco + ".");
                        break;
                }
                retorno.Add(configBoleto);
                sbBoleto.Append("<div/>");
                if (titulos[i].id_mostrar_3_boletos_pagina)
                {
                    sbBoleto = sbBoleto.Replace("<table class=\"ctN w666\">", "<table class=\"ctN w666\" style=\"display:none\">");
                    sbBoleto = sbBoleto.Replace("ebc{width:4px;height:440px;", "ebc{width:4px;height:385px;");
                    sbBoleto = sbBoleto.Replace("<tr class=\"h13\"><td colspan=\"3\" /></tr>", "<tr class=\"h13\" style=\"display:none\"><td colspan=\"3\" /></tr>");
                    sbBoleto = sbBoleto.Append("<div style=\"width: 731px;border: 1px solid;border-style: dashed;margin-left: 45px;margin:3px 0px 3px 56px\"></div>");
                }
                ViewBag.Boleto = sbBoleto.ToString();
            }
            logger.Debug("fim geração dos boletos");
            return retorno;
        }

        [AllowAnonymous]
        public ActionResult getProcessarRetornos(string parametros)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                RetornoCNAB retornoCNAB = new RetornoCNAB();
                
                DateTime? dtHoraAtualServAplic = null;
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");

                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if ("cd_escola".Equals(parametrosHash[0]))
                            retornoCNAB.cd_pessoa_empresa = int.Parse(parametrosHash[1] + "");
                        if ("cd_usuario".Equals(parametrosHash[0]))
                            retornoCNAB.cd_usuario = int.Parse(parametrosHash[1] + "");
                        else if ("cd_retorno_cnab".Equals(parametrosHash[0]))
                            retornoCNAB.cd_retorno_cnab = int.Parse(parametrosHash[1] + "");
                        else if ("nro_banco".Equals(parametrosHash[0]))
                        {
                            retornoCNAB.LocalMovto = new LocalMovto();
                            retornoCNAB.LocalMovto.nm_banco = parametrosHash[1];
                        }
                        else if ("no_arquivo_retorno".Equals(parametrosHash[0]))
                            retornoCNAB.no_arquivo_retorno = parametrosHash[1];
                        else if ("PDataHoraAtual".Equals(parametrosHash[0]))
                            dtHoraAtualServAplic = DateTime.Parse(parametrosHash[1] + "");
                        else if ("cd_local_movto".Equals(parametrosHash[0]))
                            retornoCNAB.LocalMovto.cd_local_movto = int.Parse(parametrosHash[1]);
                    }
                    configuraBusiness(new List<IGenericBusiness>() { BusinessBoleto, BusinessCnab, BusinessFiscal}, retornoCNAB.cd_usuario, retornoCNAB.cd_pessoa_empresa );
                    //Verifica se o boleto já expirou:
                    DateTime dtHoraAtualRelat = DateTime.Now;
                    TimeSpan t = dtHoraAtualRelat - dtHoraAtualServAplic.Value;
                    int timeout = int.Parse(ConfigurationManager.AppSettings["timeoutRelatorio"] + "");
                    if (t.TotalSeconds > timeout)
                    {
                        retorno.AddMensagem(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoSessaoRelatExp, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                        return new RenderJsonActionResult { Result = JsonConvert.SerializeObject(retorno) };
                    }

                    retornoCNAB.DespesasTituloCnab = new List<DespesaTituloCnab>();
                    retornoCNAB.LocalMovto = BusinessCnab.findLocalMovtoComCarteira(retornoCNAB.cd_pessoa_empresa, retornoCNAB.LocalMovto.cd_local_movto);
                    lerArquivoRetorno(retornoCNAB);

                    BusinessCnab.postProcessarRetornos(retornoCNAB);
                    retorno.retorno = retornoCNAB;
                    retorno.AddMensagem(Messages.msgArquivoRetornoProcessado, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                    //Se houve um erro em particular no processamento dos títulos, adiciona mensagens de aviso:
                    foreach (TituloRetornoCNAB tituloRetornoCNAB in retornoCNAB.TitulosRetornoCNAB.ToList())
                    {
                        if (tituloRetornoCNAB.id_tipo_retorno == (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO)
                            retorno.AddMensagem(string.Format(Messages.msgAvisoTituloCNAB, string.IsNullOrEmpty(tituloRetornoCNAB.dc_nosso_numero) ? tituloRetornoCNAB.cd_titulo + "" : tituloRetornoCNAB.dc_nosso_numero) + tituloRetornoCNAB.tx_mensagem_retorno, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                        tituloRetornoCNAB.RetornoCNAB = null;
                        tituloRetornoCNAB.Titulo = null;
                    }
                    retornoCNAB.LocalMovto = null;
                    if (retornoCNAB.TitulosRetornoCNAB != null)
                    {
                        foreach (TituloRetornoCNAB tituloRetornoCnab in retornoCNAB.TitulosRetornoCNAB)
                        {
                            tituloRetornoCnab.TransacaoFinanceira = null;
                            if (tituloRetornoCnab.DespesaTituloCnab != null && tituloRetornoCnab.DespesaTituloCnab.Any())
                            {
                                tituloRetornoCnab.DespesaTituloCnab.ForEach(x => x.TituloRetornoCNAB = null);
                            }
                        }
                    }
                        

                    return new RenderJsonActionResult { Result = JsonConvert.SerializeObject(retorno) };
                }
                retorno.AddMensagem(Messages.msgNotProcessReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return new RenderJsonActionResult { Result = JsonConvert.SerializeObject(retorno) };
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotProcessReg, retorno, logger, ex);
                }

                //return new RenderJsonActionResult { Result = JsonConvert.SerializeObject(retorno) };
            }
        }

        private void lerArquivoRetorno(RetornoCNAB retornoCNAB)
        {
            short nro_banco = short.Parse(retornoCNAB.LocalMovto.nm_banco);
            ConfiguradorBoletoBancario configBoleto = new ConfiguradorBoletoBancario(nro_banco);
            IBanco banco = new BoletoNet.Banco(nro_banco);
            Cedente cedente = new Cedente();

            cedente.Codigo = retornoCNAB.LocalMovto.dc_num_cliente_banco;
            cedente.Convenio = long.Parse(cedente.Codigo + retornoCNAB.LocalMovto.nm_digito_cedente);

            string caminho_retorno = ConfigurationManager.AppSettings["caminhoContent"] + "/Retornos/" + retornoCNAB.cd_pessoa_empresa + "/" + retornoCNAB.no_arquivo_retorno;
            Stream arquivo = null;

            try
            {
                if (ManipuladorArquivo.existeArquivo(@caminho_retorno))
                    arquivo = ManipuladorArquivo.lerArquivo(@caminho_retorno);
                else
                    throw new CnabBusinessException(Componentes.Utils.Messages.Messages.msgArquivoNaoEncontrado + " Caminho: " + caminho_retorno, null, CnabBusinessException.TipoErro.ERRO_PROCESSAR_ARQUIVO_RETORNO, false);

                if (retornoCNAB.LocalMovto.CarteiraCnab.nm_colunas == (int)CarteiraCnab.TipoColunas.CNAB240)
                {
                    ArquivoRetornoCNAB240 cnab240 = new ArquivoRetornoCNAB240();
                    cnab240.LerArquivoRetorno(banco, arquivo);
                    if (cnab240 == null)
                        throw new CnabBusinessException(Messages.msgErroArquivoRetornoNaoProcessado, null, CnabBusinessException.TipoErro.ERRO_PROCESSAR_ARQUIVO_RETORNO, false);

                    foreach (DetalheRetornoCNAB240 detalhe in cnab240.ListaDetalhes)
                    {
                        TituloRetornoCNAB tituloRetorno = new TituloRetornoCNAB();
                        int cd_titulo_retorno_cnab;
                        Int32.TryParse(detalhe.SegmentoT.NumeroDocumento, out cd_titulo_retorno_cnab);
                        tituloRetorno.cd_titulo_retorno_cnab = cd_titulo_retorno_cnab;
                        tituloRetorno.dt_baixa_retorno = detalhe.SegmentoU.DataOcorrencia;
                        tituloRetorno.dt_banco_retorno = detalhe.SegmentoU.DataOcorrencia;
                        tituloRetorno.vl_baixa_retorno = detalhe.SegmentoU.ValorPagoPeloSacado;
                        tituloRetorno.vl_multa_retorno = detalhe.SegmentoU.JurosMultaEncargos; //Juros de mora e multa são juntos no Itaú. Campo 267 do arquivo de retorno.
                        tituloRetorno.vl_desconto_titulo = detalhe.SegmentoU.ValorDescontoConcedido;
                        tituloRetorno.tx_mensagem_retorno = "";
                        tituloRetorno.dc_nosso_numero = detalhe.SegmentoT.NossoNumero;

                        especializarRetornoBanco(nro_banco, cedente, new DetalheRetorno() { CodigoOcorrencia = Convert.ToInt32(detalhe.SegmentoU.CodigoOcorrenciaSacado), ValorAbatimento = detalhe.SegmentoU.ValorAbatimentoConcedido }, tituloRetorno, (int)CarteiraCnab.TipoColunas.CNAB240);
                        retornoCNAB.TitulosRetornoCNAB.Add(tituloRetorno);
                    }
                    retornoCNAB.nm_linhas_retorno = (short)cnab240.ListaDetalhes.Count;
                }
                else if (retornoCNAB.LocalMovto.CarteiraCnab.nm_colunas == (int)CarteiraCnab.TipoColunas.CNAB400)
                {
                    ArquivoRetornoCNAB400 cnab400 = new ArquivoRetornoCNAB400();
                    cnab400.LerArquivoRetorno(banco, arquivo);
                    if (cnab400 == null)
                        throw new CnabBusinessException(Messages.msgErroArquivoRetornoNaoProcessado, null, CnabBusinessException.TipoErro.ERRO_PROCESSAR_ARQUIVO_RETORNO, false);

                    foreach (DetalheRetorno detalhe in cnab400.ListaDetalhe)
                    {
                        TituloRetornoCNAB tituloRetorno = new TituloRetornoCNAB();
                        int cd_titulo_retorno_cnab;

                        Int32.TryParse(detalhe.NumeroDocumento, out cd_titulo_retorno_cnab);
                        tituloRetorno.cd_titulo_retorno_cnab = cd_titulo_retorno_cnab;
                        if (detalhe.DataCredito.Year > 2000)
                            tituloRetorno.dt_baixa_retorno = detalhe.DataCredito;
                        else if (detalhe.DataLiquidacao.Year > 200)
                            tituloRetorno.dt_baixa_retorno = detalhe.DataCredito;
                        tituloRetorno.dt_banco_retorno = detalhe.DataOcorrencia;
                        tituloRetorno.vl_baixa_retorno = detalhe.ValorPago;
                        tituloRetorno.vl_juros_retorno = detalhe.JurosMora; //Juros de mora e multa são juntos no Itaú. Campo 267 do arquivo de retorno.
                        tituloRetorno.vl_multa_retorno = detalhe.ValorMulta;
                        tituloRetorno.vl_desconto_titulo = detalhe.Descontos;
                        tituloRetorno.tx_mensagem_retorno = detalhe.Erros;
                        tituloRetorno.dc_nosso_numero = detalhe.NossoNumero;
                        
                        //Despesa CNAB
                        tituloRetorno.codigoOcorrencia  = detalhe.CodigoOcorrencia;
                        tituloRetorno.valorDespesa = detalhe.ValorDespesa;

                        especializarRetornoBanco(nro_banco, cedente, detalhe, tituloRetorno, (int)CarteiraCnab.TipoColunas.CNAB400);

                        // Despesas Retorno CNAB;
                        DespesaTituloCnab despesa = addDespesaTituloCnab(cd_titulo_retorno_cnab, tituloRetorno);

                        if ((tituloRetorno.codigoOcorrencia == (int)TituloRetornoCNAB.CodigoOcorrenciaCnab.ENTRADA_CONFIRMADA || tituloRetorno.codigoOcorrencia == (int)TituloRetornoCNAB.CodigoOcorrenciaCnab.DEBITO_TARIFAS)
                            || (despesa.cd_despesa == null))
                        {
                            retornoCNAB.TitulosRetornoCNAB.Add(tituloRetorno);
                        }

                        if ((despesa.cd_despesa != null) && (tituloRetorno.valorDespesa > 0))
                        {
                             TituloRetornoCNAB tituloRetornoCnabAchado = retornoCNAB.TitulosRetornoCNAB.Where(x => x.dc_nosso_numero == tituloRetorno.dc_nosso_numero).FirstOrDefault();
                             if (tituloRetornoCnabAchado != null)
                             {
                                 //Estamos partindo do principio que a linha de despesa sempre vem junto com titulo baixado
                                 //No futuro se vier uma linha de despesa sem o titulo, teremos que procurar o tituloRetornoAchado no banco de dados
                                 despesa.TituloRetornoCNAB = tituloRetornoCnabAchado;
                                 despesa.vl_despesa = detalhe.ValorDespesa;
                                 //if (tituloRetornoCnabAchado.valorDespesa == null) tituloRetornoCnabAchado.valorDespesa = 0;
                                 if (tituloRetornoCnabAchado.tx_mensagem_retorno == null) tituloRetornoCnabAchado.tx_mensagem_retorno = "";
                                 tituloRetornoCnabAchado.valorDespesa = tituloRetornoCnabAchado.valorDespesa + detalhe.ValorDespesa;
                                 tituloRetornoCnabAchado.tx_mensagem_retorno = tituloRetornoCnabAchado.tx_mensagem_retorno + " " + detalhe.Erros;
                                 tituloRetornoCnabAchado.DespesaTituloCnab.Add(despesa);
                                 
                                 /*Processo - achar despesa pelo objeto
                                  -achar tituloRetornoOriginal
                                  -despesa.tituloRetorno = achado
                                  -despesa.vl_despesa = detalhe.ValorDespesa
                                  -tituloRetornoAchado.valorDespesa = detalhe.ValorDespesa
                                  -tituloRetornoAchado.DespesaTituloCnab.Add(despesa);*/

                             }
                             
                        }

                        
                        //retornoCNAB.DespesasTituloCnab.Add(addDespesaTituloCnab(cd_titulo_retorno_cnab, tituloRetorno));
                    }
                    retornoCNAB.nm_linhas_retorno = (short)cnab400.ListaDetalhe.Count;
                }
            }
            catch (CnabBusinessException ce)
            {
                throw ce;
            }
            catch (Exception e)
            {
                throw new Exception("Número do banco: " + nro_banco + ", caminho_retorno: " + caminho_retorno, e);
            }
            finally
            {
                ManipuladorArquivo.fecharArquivo(arquivo);
            }
        }

        private DespesaTituloCnab addDespesaTituloCnab(int cd_titulo_retorno_cnab, TituloRetornoCNAB tituloRetorno)
        {
            try
            {
                var despesa = new DespesaTituloCnab();
                despesa.cd_titulo_retorno_cnab = cd_titulo_retorno_cnab;

                if (tituloRetorno.codigoOcorrencia == (int)TituloRetornoCNAB.CodigoOcorrenciaCnab.DEBITO_TARIFAS)
                {
                    despesa.cd_despesa = "0" + ((int)TituloRetornoCNAB.MotivoDespesaCnab.TAXA_BOLETO);
                    despesa.dc_despesa = "Taxa de Boleto(Registro)";
                    despesa.vl_despesa = tituloRetorno.valorDespesa;
                }
                if (tituloRetorno.codigoOcorrencia == (int)TituloRetornoCNAB.CodigoOcorrenciaCnab.ENTRADA_CONFIRMADA)
                {
                    despesa.cd_despesa = "0" + ((int)TituloRetornoCNAB.MotivoDespesaCnab.CUSTOS_PROTESTO);
                    despesa.dc_despesa = "Custas de Protesto";
                    despesa.vl_despesa = tituloRetorno.valorDespesa;
                }
                
                return despesa;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void especializarRetornoBanco(short nro_banco, Cedente cedente, DetalheRetorno detalhe, TituloRetornoCNAB tituloRetorno, int tipo_colunas)
        {
            switch ((Bancos)nro_banco)
            {
                case Bancos.Itau:
                    #region Ocorrência Itau
                    switch (detalhe.CodigoOcorrencia)
                    {
                        case 2: //02-Entrada Confirmada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_TITULO;
                            break;
                        case 9: //09-Baixa simples
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_PEDIDO_BAIXA;
                            break;
                        case 6: //06-Liquidação normal
                        case 8: //08-Liquidação em cartório
                        case 10: //10-Baixa por ter sido liquidado
                        case 76: //76-Cheque Compensado
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.GERAR_BAIXA;
                            break;
                        case 19: //19-Confirma Recebimento Instrução de Protesto
                        case 20: //20-Confirma Recebimento Instrução Sustação de Protesto/Tarifa
                        case 21: //21-Confirma Recebimento Instrução de Não Protestar
                        case 23: //23-Título enviado a Cartório/Tarifa
                        case 24: //24-Instrução de Protesto Rejeitada/Sustada/Pendente
                        case 32: //32-Baixa por ter sido Protestado
                        case 33: //33-Custas de Protesto
                        case 34: //34-Custas de Sustação
                        case 35: //35-Custas de Cartório Distribuidor
                        case 45: //45-Débito Mensal de Tarifas-Protesto
                        case 46: //56-Débito Mensal de Tarifas-Sustação de Protesto
                        case 47: //47-Baixa com Transferência para Protesto
                        case 48: //48-Custas de Sustação Judicial
                        case 56: //56-Débito Mensal de Tarifas-Sustação de Protesto
                        case 63: //63-Título Sustado Judicialmente
                            switch (tituloRetorno.id_tipo_retorno)
                            {
                                case 19: tituloRetorno.tx_mensagem_retorno += " 19-Confirma Recebimento Instrução de Protesto"; break;
                                case 20: tituloRetorno.tx_mensagem_retorno += " 20-Confirma Recebimento Instrução Sustação de Protesto/Tarifa"; break;
                                case 21: tituloRetorno.tx_mensagem_retorno += " 21-Confirma Recebimento Instrução de Não Protestar"; break;
                                case 23: tituloRetorno.tx_mensagem_retorno += " 23-Título enviado a Cartório/Tarifa"; break;
                                case 24: tituloRetorno.tx_mensagem_retorno += " 24-Instrução de Protesto Rejeitada/Sustada/Pendente"; break;
                                case 32: tituloRetorno.tx_mensagem_retorno += " 32-Baixa por ter sido Protestado"; break;
                                case 33: tituloRetorno.tx_mensagem_retorno += " 33-Custas de Protesto"; break;
                                case 34: tituloRetorno.tx_mensagem_retorno += " 34-Custas de Sustação"; break;
                                case 35: tituloRetorno.tx_mensagem_retorno += " 35-Custas de Cartório Distribuidor"; break;
                                case 45: tituloRetorno.tx_mensagem_retorno += " 45-Débito Mensal de Tarifas-Protesto"; break;
                                case 46: tituloRetorno.tx_mensagem_retorno += " 56-Débito Mensal de Tarifas-Sustação de Protesto"; break;
                                case 47: tituloRetorno.tx_mensagem_retorno += " 47-Baixa com Transferência para Protesto"; break;
                                case 48: tituloRetorno.tx_mensagem_retorno += " 48-Custas de Sustação Judicial"; break;
                                case 56: tituloRetorno.tx_mensagem_retorno += " 56-Débito Mensal de Tarifas-Sustação de Protesto"; break;
                                case 63: tituloRetorno.tx_mensagem_retorno += " 63-Título Sustado Judicialmente"; break;
                            }
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.RETORNO_PROTESTO;
                            break;
                        case 3: //03-Entrada Rejeitada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 03-Entrada Rejeitada";
                            break;
                        case 4: //04-Alteração de Dados-Nova entrada ou Alteração/Exclusõa de dados acatada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 04-Alteração de Dados-Nova entrada ou Alteração/Exclusõa de dados acatada";
                            break;
                        case 5: //05-Alteração de dados-Baixa
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 05-Alteração de dados-Baixa";
                            break;
                        case 11: //11-Em Ser (Só no retorno mensal)
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 11-Em Ser (Só no retorno mensal)";
                            break;
                        case 12: //12-Abatimento Concedido
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 12-Abatimento Concedido";
                            break;
                        case 13: //13-Abatimento Cancelado
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 13-Abatimento Cancelado";
                            break;
                        case 14: //14-Vencimento Alterado
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 14-Vencimento Alterado";
                            break;
                        case 15: //15-Baixas rejeitadas
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 15-Baixas rejeitadas";
                            break;
                        case 16: //16-Instruções rejeitadas
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 16-Instruções rejeitadas";
                            break;
                        case 17: //17-Alteração/Exclusão de dados rejeitados
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 17-Alteração/Exclusão de dados rejeitados";
                            break;
                        case 18: //18-Cobrança contratual-Instruções/Alterações rejeitadas/pendentes
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 18-Cobrança contratual-Instruções/Alterações rejeitadas/pendentes";
                            break;
                        case 25: //25-Alegações do Sacado
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 25-Alegações do Sacado";
                            break;
                        case 26: //26-Tarifa de Aviso de Cobrança
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 26-Tarifa de Aviso de Cobrança";
                            break;
                        case 27: //27-Tarifa de Extrato Posição
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 27-Tarifa de Extrato Posição";
                            break;
                        case 28: //28-Tarifa de Relação das Liquidações
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 28-Tarifa de Relação das Liquidações";
                            break;
                        case 29: //29-Tarifa de Manutenção de Títulos Vencidos
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 29-Tarifa de Manutenção de Títulos Vencidos";
                            break;
                        case 30: //30-Débito Mensal de Tarifas (Para Entradas e Baixas)
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 30-Débito Mensal de Tarifas (Para Entradas e Baixas)";
                            break;
                        case 36: //36-Custas de Edital
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 36-Custas de Edital";
                            break;
                        case 37: //37-Tarifa de Emissão de Boleto/Tarifa de Envio de Duplicata
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 37-Tarifa de Emissão de Boleto/Tarifa de Envio de Duplicata";
                            break;
                        case 38: //38-Tarifa de Instrução
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 38-Tarifa de Instrução";
                            break;
                        case 39: //39-Tarifa de Ocorrências
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 39-Tarifa de Ocorrências";
                            break;
                        case 40: //40-Tarifa Mensal de Emissão de Boleto/Tarifa Mensal de Envio de Duplicata
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 40-Tarifa Mensal de Emissão de Boleto/Tarifa Mensal de Envio de Duplicata";
                            break;
                        case 41: //41-Débito Mensal de Tarifas-Extrato de Posição(B4EP/B4OX)
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 41-Débito Mensal de Tarifas-Extrato de Posição(B4EP/B4OX)";
                            break;
                        case 42: //42-Débito Mensal de Tarifas-Outras Instruções
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 42-Débito Mensal de Tarifas-Outras Instruções";
                            break;
                        case 43: //43-Débito Mensal de Tarifas-Manutenção de Títulos Vencidos
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 43-Débito Mensal de Tarifas-Manutenção de Títulos Vencidos";
                            break;
                        case 44: //44-Débito Mensal de Tarifas-Outras Ocorrências
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 44-Débito Mensal de Tarifas-Outras Ocorrências";
                            break;
                        case 51: //51-Tarifa Mensal Ref a Entradas Bancos Correspondentes na Carteira
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 51-Tarifa Mensal Ref a Entradas Bancos Correspondentes na Carteira";
                            break;
                        case 52: //52-Tarifa Mensal Baixas na Carteira
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 52-Tarifa Mensal Baixas na Carteira";
                            break;
                        case 53: //53-Tarifa Mensal Baixas em Bancos Correspondentes na Carteira
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 53-Tarifa Mensal Baixas em Bancos Correspondentes na Carteira";
                            break;
                        case 54: //54-Tarifa Mensal de Liquidações na Carteira
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 54-Tarifa Mensal de Liquidações na Carteira";
                            break;
                        case 55: //55-Tarifa Mensal de Liquidações em Bancos Correspondentes na Carteira
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 55-Tarifa Mensal de Liquidações em Bancos Correspondentes na Carteira";
                            break;
                        case 57: //57-Instrução Cancelada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 57-Instrução Cancelada";
                            break;
                        case 59: //59-Baixa por Crédito em C/C Através do SISPAG
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 59-Baixa por Crédito em C/C Através do SISPAG";
                            break;
                        case 60: //60-Entrada Rejeitada Carnê
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 60-Entrada Rejeitada Carnê";
                            break;
                        case 61: //61-Tarifa Emissão Aviso de Movimentação de Títulos
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 61-Tarifa Emissão Aviso de Movimentação de Títulos";
                            break;
                        case 62: //62-Débito Mensal de Tarifa-Aviso de Movimentação de Títulos
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 62-Débito Mensal de Tarifa-Aviso de Movimentação de Títulos";
                            break;
                        case 64: //64-Entrada Confirmada com Rateio de Crédito
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 64-Entrada Confirmada com Rateio de Crédito";
                            break;
                        case 69: //69-Cheque Devolvido
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 69-Cheque Devolvido";
                            break;
                        case 71: //71-Entrada Registrada-Aguardando Avaliação
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 71-Entrada Registrada-Aguardando Avaliação";
                            break;
                        case 72: //72-Baixa por Crédito em C/C Através do SISPAG sem Título Correspondente
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 72-Baixa por Crédito em C/C Através do SISPAG sem Título Correspondente";
                            break;
                        case 73: //73-Confirmação de Entrada na Cobrança Simples-Entrada Não Aceita na Cobrança Contratual
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 73-Confirmação de Entrada na Cobrança Simples-Entrada Não Aceita na Cobrança Contratual";
                            break;
                        default:
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " Ocorrência não mapeada.";
                            break;
                    }
                    #endregion
                    break;
                case Bancos.Inter:
                    #region Ocorrência Inter
                    switch (detalhe.CodigoOcorrencia)
                    {
                        case 2: //Entrada Confirmada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_TITULO;
                            break;
                        case 3: //Registro Recusado 
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 03-Entrada Rejeitada ";
                            if (!string.IsNullOrEmpty(detalhe.MotivosRejeicao))
                                tituloRetorno.tx_mensagem_retorno += detalhe.MotivosRejeicao;
                            break;

                        case 6: //Retorno Liquidado
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.GERAR_BAIXA;
                            break;
                        case 7: //09-Baixa simples
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_PEDIDO_BAIXA;
                            break;
                        default:
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            if (!string.IsNullOrEmpty(detalhe.MotivosRejeicao))
                                tituloRetorno.tx_mensagem_retorno += detalhe.MotivosRejeicao;
                            else
                                tituloRetorno.tx_mensagem_retorno += " Ocorrência não mapeada.";
                            break;
                    }
                    #endregion
                    break;
                case Bancos.HSBC:
                    #region Ocorrência HSBC
                    switch (detalhe.CodigoOcorrencia)
                    {
                        case 2: //02-Entrada Confirmada
                        case 9: //09-Baixa automática
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_TITULO;
                            break;
                        case 3: //03-Entrada rejeitada ou Instrução rejeitada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += detalhe.MotivosRejeicao;
                            break;
                        case 6: //06-Liquidação normal em dinheiro
                        case 10://10-Baixado conforme instruções
                        case 7: //07-Liquidação por conta em dinheiro
                        case 15://15-Liquidação em cartório em dinheiro
                        case 16://16-Liquidação - baixado/devolvido em data anterior dinheiro
                        case 31://31-Liquidação normal em cheque/compensação/banco correspondente
                        case 32://32-Liquidação em cartório em cheque
                        case 33://33-Liquidação por conta em cheque
                        case 36://36-Liquidação - baixado/devolvido em data anterior em cheque
                        case 38://38-Liquidação de título não registrado - em dinheiro
                        case 39://39-Liquidação de título não registrado - em cheque
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.GERAR_BAIXA;
                            break;
                        default:
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " Ocorrência não mapeada.";
                            break;
                    }
                    #endregion
                    break;
                case Bancos.Santander:
                    #region Ocorrência Santander
                    tituloRetorno.vl_desconto_titulo += detalhe.ValorAbatimento;
                    if (tipo_colunas == (int)CarteiraCnab.TipoColunas.CNAB240)
                        tituloRetorno.dc_nosso_numero = tituloRetorno.dc_nosso_numero.Substring(0, tituloRetorno.dc_nosso_numero.Length - 1);
                    switch (detalhe.CodigoOcorrencia)
                    {
                        case 2: //02-Confirmação de entrada de título
                        case 44://44-Título Pago com cheque devolvido
                        case 50://50-Título pago com cheque pendente de compensação
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_TITULO;
                            break;
                        case 5: //05-Liquidação sem registro
                        case 6: //06-Liquidação normal
                        case 7: //07-Liquidação por conta
                        case 8: //08-Liquidação por saldo
                        case 15://15-Liquidação em cartório
                        case 17: //liquidação após baixa ou liquidação título não registrado
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.GERAR_BAIXA;
                            break;
                        case 9: //09-Baixa simples
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_PEDIDO_BAIXA;
                            break;
                        case 19: //19-Confirma Recebimento Instrução de Protesto
                        case 20: //20-Confirmação recebimento instrução de sustação/Não Protestar
                        case 23: //23-Remessa a cartorio ( aponte em cartorio)
                        case 24: //24-Retirada de cartorio e manutenção em carteira
                        case 25: //25-Protestado e baixado ( baixa por ter sido protestado) 
                        case 28: //28-Debito de tarifas/custas
                        case 47: //47-Baixa com Transferência para Protesto
                            switch (tituloRetorno.id_tipo_retorno)
                            {
                                case 19: tituloRetorno.tx_mensagem_retorno += " 19-Confirma Recebimento Instrução de Protesto"; break;
                                case 20: tituloRetorno.tx_mensagem_retorno += " 20-Confirmação recebimento instrução de sustação/Não Protestar"; break;
                                case 23: tituloRetorno.tx_mensagem_retorno += " 23-Remessa a cartorio ( aponte em cartorio)"; break;
                                case 24: tituloRetorno.tx_mensagem_retorno += " 24-Retirada de cartorio e manutenção em carteira"; break;
                                case 25: tituloRetorno.tx_mensagem_retorno += " 25-Protestado e baixado ( baixa por ter sido protestado)"; break;
                                case 28: tituloRetorno.tx_mensagem_retorno += " 28-Debito de tarifas/custas"; break;
                                case 47: tituloRetorno.tx_mensagem_retorno += " 47-Baixa com Transferência para Protesto"; break;
                            }
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.RETORNO_PROTESTO;
                            break;
                        default:
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            if (!string.IsNullOrEmpty(detalhe.MotivosRejeicao))
                                tituloRetorno.tx_mensagem_retorno += detalhe.MotivosRejeicao;
                            else
                                tituloRetorno.tx_mensagem_retorno += detalhe.CodigoOcorrencia.ToString() + " - Ocorrência não mapeada." + " Motivo rejeição:" + detalhe.MotivosRejeicao;
                            break;
                    }
                    #endregion
                    break;
                case Bancos.Unicred:
                case Bancos.Bradesco:
                    #region Ocorrência Bradesco
                    switch (detalhe.CodigoOcorrencia)
                    {
                        case 2: //Entrada Confirmada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_TITULO;
                            break;
                        case 6: //Liquidação normal
                        case 15: //Liquidação em Cartório
                        case 17: //Liquidação após baixa ou Título não registrado (sem motivo)
                        case 18: //Acerto de Depositária (sem motivo)
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.GERAR_BAIXA;
                            break;
                        case 9:  //09-Baixa
                        case 10: //Baixado conforme instruções da Agência
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_PEDIDO_BAIXA;
                            break;
                        case 19: //Confirmação Receb. Inst. de Protesto
                        case 20: //Confirmação Recebimento Instrução Sustação de Protesto
                        case 23: //Entrada do Título em Cartório
                        case 25: //Confirmação Receb.Inst.de Protesto Falimentar
                        //case 28: //Débito de tarifas/custas
                        case 34: //Retirado de Cartório e Manutenção Carteira
                            switch (tituloRetorno.id_tipo_retorno)
                            {
                                case 19: tituloRetorno.tx_mensagem_retorno += " 19-Confirma Recebimento Instrução de Protesto"; break;
                                case 20: tituloRetorno.tx_mensagem_retorno += " 20-Confirma Recebimento Instrução Sustação de Protesto"; break;
                                case 23: tituloRetorno.tx_mensagem_retorno += " 23-Entrada do Título em Cartório"; break;
                                case 25: tituloRetorno.tx_mensagem_retorno += " 25-Protestado e baixado ( baixa por ter sido protestado)"; break;
                                //case 28: tituloRetorno.tx_mensagem_retorno += " 28-Débito de tarifas/custas"; break;
                                case 34: tituloRetorno.tx_mensagem_retorno += " 34-Retirado de Cartório e Manutenção Carteira"; break;
                            }
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.RETORNO_PROTESTO;
                            break;
                        default:
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            if (!string.IsNullOrEmpty(detalhe.MotivosRejeicao))
                                tituloRetorno.tx_mensagem_retorno += detalhe.MotivosRejeicao;
                            else
                                tituloRetorno.tx_mensagem_retorno += " Ocorrência não mapeada.";
                            break;
                    }
                    #endregion
                    break;
                case Bancos.DinariPay:
                    #region Ocorrência DinariPay
                    switch (detalhe.CodigoOcorrencia)
                    {
                        case 2: //Entrada Confirmada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_TITULO;
                            break;
                        case 6: //Liquidação normal
                        case 15: //Liquidação em Cartório
                        case 17: //Liquidação após baixa ou Título não registrado (sem motivo)
                        case 18: //Acerto de Depositária (sem motivo)
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.GERAR_BAIXA;
                            break;
                        case 9:  //09-Baixa
                        case 10: //Baixado conforme instruções da Agência
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_PEDIDO_BAIXA;
                            break;
                        case 19: //Confirmação Receb. Inst. de Protesto
                        case 20: //Confirmação Recebimento Instrução Sustação de Protesto
                        case 23: //Entrada do Título em Cartório
                        case 25: //Confirmação Receb.Inst.de Protesto Falimentar
                        //case 28: //Débito de tarifas/custas
                        case 34: //Retirado de Cartório e Manutenção Carteira
                            switch (tituloRetorno.id_tipo_retorno)
                            {
                                case 19: tituloRetorno.tx_mensagem_retorno += " 19-Confirma Recebimento Instrução de Protesto"; break;
                                case 20: tituloRetorno.tx_mensagem_retorno += " 20-Confirma Recebimento Instrução Sustação de Protesto"; break;
                                case 23: tituloRetorno.tx_mensagem_retorno += " 23-Entrada do Título em Cartório"; break;
                                case 25: tituloRetorno.tx_mensagem_retorno += " 25-Protestado e baixado ( baixa por ter sido protestado)"; break;
                                //case 28: tituloRetorno.tx_mensagem_retorno += " 28-Débito de tarifas/custas"; break;
                                case 34: tituloRetorno.tx_mensagem_retorno += " 34-Retirado de Cartório e Manutenção Carteira"; break;
                            }
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.RETORNO_PROTESTO;
                            break;
                        default:
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            if (!string.IsNullOrEmpty(detalhe.MotivosRejeicao))
                                tituloRetorno.tx_mensagem_retorno += detalhe.MotivosRejeicao;
                            else
                                tituloRetorno.tx_mensagem_retorno += " Ocorrência não mapeada.";
                            break;
                    }
                    #endregion
                    break;
                case Bancos.BancodoBrasil:
                    #region Ocorrência Banco do Brasil
                    //Removendo a parte do convênio do nosso número:
                    tituloRetorno.dc_nosso_numero = tituloRetorno.dc_nosso_numero.Replace(cedente.Convenio + "", "");

                    //Removendo o dígito verificador do nosso número:
                    if (tituloRetorno.dc_nosso_numero.Length < 10)
                        tituloRetorno.dc_nosso_numero = tituloRetorno.dc_nosso_numero.Substring(0, tituloRetorno.dc_nosso_numero.Length - 2);
                    tituloRetorno.dc_nosso_numero = long.Parse(tituloRetorno.dc_nosso_numero) + "";

                    switch (detalhe.CodigoOcorrencia)
                    {
                        case 2: //Entrada Confirmada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_TITULO;
                            break;
                        case 3: //03-Entrada rejeitada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 03-Entrada rejeitada.";
                            break;
                        case 4: // 04 –Transferência de Carteira/Entrada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += "  04 –Transferência de Carteira/Entrada.";
                            break;
                        case 5: // Transferência de Carteira/Baixa
                        case 6: // Liquidação
                        case 17: //Liquidação Após Baixa ou Liquidação Título Não Registrado
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.GERAR_BAIXA;
                            break;
                        case 9: //Baixa
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_PEDIDO_BAIXA;
                            break;
                        case 26: //  26 – Instrução Rejeitada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += "   26 – Instrução Rejeitada.";
                            break;
                        case 30: //  30 – Alteração de Dados Rejeitada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 30 – Alteração de Dados Rejeitada.";
                            break;
                        case 44: //  44 –  Título pago com cheque devolvido
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 44 –  Título pago com cheque devolvido.";
                            break;
                        case 50: //  50 – Título pago com cheque pendente de compensação
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 50 – Título pago com cheque pendente de compensação.";
                            break;
                        case 19: //Confirmação Recebimento Instrução de Protesto
                        case 20: //Confirmação Recebimento Instrução de Sustação/Cancelamento de Protesto
                        case 23: //Remessa a Cartório (Aponte em Cartório)
                        case 24: //Retirada de Cartório e Manutenção em Carteira,
                        case 25: //Protestado e Baixado (Baixa por ter sido Protestado)
                            switch (tituloRetorno.id_tipo_retorno)
                            {
                                case 19: tituloRetorno.tx_mensagem_retorno += " 19 – Confirmação Recebimento Instrução de Protesto"; break;
                                case 20: tituloRetorno.tx_mensagem_retorno += " 20 – Confirmação Recebimento Instrução de Sustação/Cancelamento de Protesto"; break;
                                case 23: tituloRetorno.tx_mensagem_retorno += " 23 - Remessa a Cartório (Aponte em Cartório)"; break;
                                case 24: tituloRetorno.tx_mensagem_retorno += " 24 - Retirada de Cartório e Manutenção em Carteira,"; break;
                                case 25: tituloRetorno.tx_mensagem_retorno += " 25 – Protestado e Baixado (Baixa por ter sido Protestado)"; break;
                            }
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.RETORNO_PROTESTO;
                            break;
                        default:
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            if (!string.IsNullOrEmpty(detalhe.MotivosRejeicao))
                                tituloRetorno.tx_mensagem_retorno += detalhe.MotivosRejeicao;
                            else
                                tituloRetorno.tx_mensagem_retorno += " Ocorrência não mapeada.";
                            break;
                    }
                    #endregion
                    break;
                case Bancos.Caixa:
                    tituloRetorno.dc_nosso_numero = detalhe.NossoNumeroComDV.Substring(2, detalhe.NossoNumeroComDV.Length - 2);
                    #region Ocorrência Caixa
                    if (tipo_colunas == (int)CarteiraCnab.TipoColunas.CNAB240)
                        #region 240 posições
                        switch (detalhe.CodigoOcorrencia)
                        {
                            case 2: //Entrada Confirmada
                                tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_TITULO;
                                break;
                            case 3: //03-Entrada rejeitada
                                tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                                tituloRetorno.tx_mensagem_retorno += " 03-Entrada rejeitada.";
                                break;
                            case 4: // 04 –Transferência de Carteira/Entrada
                                tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                                tituloRetorno.tx_mensagem_retorno += "  04 –Transferência de Carteira/Entrada.";
                                break;
                            case 5: // Transferência de Carteira/Baixa
                            case 6: // Liquidação 
                                tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.GERAR_BAIXA;
                                break;
                            case 9: //Baixa
                                tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_PEDIDO_BAIXA;
                                break;
                            case 19: //Confirmação Recebimento Instrução de Protesto
                            case 20: //Confirmação Recebimento Instrução de Sustação/Cancelamento de Protesto
                            case 23: //Remessa a Cartório
                            case 24: //Retirada de Cartório
                            case 25: //Protestado e Baixado (Baixa por Ter Sido Protestado) 
                            case 28: //Débito de Tarifas/Custas
                                switch (tituloRetorno.id_tipo_retorno)
                                {
                                    case 19: tituloRetorno.tx_mensagem_retorno += " 19 – Confirmação Recebimento Instrução de Protesto"; break;
                                    case 20: tituloRetorno.tx_mensagem_retorno += " 20 – Confirmação Recebimento Instrução de Sustação/Cancelamento de Protesto"; break;
                                    case 24: tituloRetorno.tx_mensagem_retorno += " 24 – Retirada de Cartório"; break;
                                    case 25: tituloRetorno.tx_mensagem_retorno += " 25 – Protestado e Baixado (Baixa por Ter Sido Protestado) "; break;
                                    case 28: tituloRetorno.tx_mensagem_retorno += " //Débito de Tarifas/Custas"; break;
                                }
                                tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.RETORNO_PROTESTO;
                                break;
                            case 26: //  26 – Instrução Rejeitada
                                tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                                tituloRetorno.tx_mensagem_retorno += "   26 – Instrução Rejeitada.";
                                break;
                            case 30: //  30 – Alteração de Dados Rejeitada
                                tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                                tituloRetorno.tx_mensagem_retorno += " 30 – Alteração de Dados Rejeitada.";
                                break;
                            case 39: //  39 – Manutenção de Sacado Rejeitada
                                tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                                tituloRetorno.tx_mensagem_retorno += " 39 – Manutenção de Sacado Rejeitada.";
                                break;
                            case 40: //  40 – Entrada de Título via Banco de Sacado Rejeitada 
                                tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                                tituloRetorno.tx_mensagem_retorno += " 40 – Entrada de Título via Banco de Sacado Rejeitada.";
                                break;
                            case 41: //  41 – Manutenção de Banco de Sacado Rejeitada  
                                tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                                tituloRetorno.tx_mensagem_retorno += " 41 – Manutenção de Banco de Sacado Rejeitada.";
                                break;
                            case 44: //  44 - Estorno de Baixa / Liquidação
                                tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                                tituloRetorno.tx_mensagem_retorno += " 44 - Estorno de Baixa / Liquidação.";
                                break;
                            default:
                                tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                                if (!string.IsNullOrEmpty(detalhe.MotivosRejeicao))
                                    tituloRetorno.tx_mensagem_retorno += detalhe.MotivosRejeicao;
                                else
                                    tituloRetorno.tx_mensagem_retorno += detalhe.CodigoOcorrencia + " - Ocorrência não mapeada.";
                                break;
                        }
                        #endregion
                    #region 400 Posições
                    switch (detalhe.CodigoOcorrencia)
                    {
                        case 1: //Entrada Confirmada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_TITULO;
                            break;
                        case 2: // Baixa Manual Confirmada 
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_PEDIDO_BAIXA;
                            break;
                        case 3: //03-Abatimento Concedido 
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 03- Abatimento Concedido .";
                            break;
                        case 4: // 04 –Abatimento Cancelado 
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += "  04 – Abatimento Cancelado .";
                            break;
                        case 21: // Liquidação 
                        case 22: // Liquidação em Cartório 
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.GERAR_BAIXA;
                            switch (tituloRetorno.id_tipo_retorno)
                            {
                                case 21: tituloRetorno.tx_mensagem_retorno += " 21 – Liquidação"; break;
                                case 22: tituloRetorno.tx_mensagem_retorno += " 22 – Liquidação em Cartório "; break;
                            }
                            break;
                        case 25: //Baixa por Protesto
                        case 26: //Título enviado para Cartório 
                        case 27: //Sustação de Protesto
                        case 28: // Estorno de Protesto
                        case 29: // Estorno de Sustação de Protesto 
                            switch (tituloRetorno.id_tipo_retorno)
                            {
                                case 25: tituloRetorno.tx_mensagem_retorno += " 25 – Baixa por Protesto"; break;
                                case 26: tituloRetorno.tx_mensagem_retorno += " 26 – Baixa por Protesto"; break;
                                case 27: tituloRetorno.tx_mensagem_retorno += " 27 – Título enviado para Cartório"; break;
                                case 28: tituloRetorno.tx_mensagem_retorno += " 28 – Estorno de Protestoo"; break;
                                case 29: tituloRetorno.tx_mensagem_retorno += "29 - Estorno de Sustação de Protesto "; break;
                            }
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.RETORNO_PROTESTO;
                            break;
                        case 30: //  30 – Alteração de Dados Rejeitada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 30 – Alteração de Dados Rejeitada.";
                            break;
                        case 39: //  39 – Manutenção de Sacado Rejeitada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 39 – Manutenção de Sacado Rejeitada.";
                            break;
                        case 40: //  40 – Entrada de Título via Banco de Sacado Rejeitada 
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 40 – Entrada de Título via Banco de Sacado Rejeitada.";
                            break;
                        case 41: //  41 – Manutenção de Banco de Sacado Rejeitada  
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 41 – Manutenção de Banco de Sacado Rejeitada.";
                            break;
                        case 44: //  44 - Estorno de Baixa / Liquidação
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 44 - Estorno de Baixa / Liquidação.";
                            break;
                        default:
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            if (!string.IsNullOrEmpty(detalhe.MotivosRejeicao))
                                tituloRetorno.tx_mensagem_retorno += detalhe.MotivosRejeicao;
                            else
                                tituloRetorno.tx_mensagem_retorno += detalhe.CodigoOcorrencia + " - Ocorrência não mapeada.";
                            break;
                    }
                    #endregion
                    #endregion
                    break;
                case Bancos.Uniprime:
                case Bancos.Sicoob:
                    #region Ocorrência Sicoob
                    if (tipo_colunas == (int)CarteiraCnab.TipoColunas.CNAB240)
                        tituloRetorno.dc_nosso_numero = tituloRetorno.dc_nosso_numero.Substring(0, 9);
                    if (tipo_colunas == (int)CarteiraCnab.TipoColunas.CNAB400)
                        tituloRetorno.vl_juros_retorno = detalhe.ValorOutrasDespesas;
                    switch (detalhe.CodigoOcorrencia)
                    {
                        case 2: //Entrada Confirmada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_TITULO;
                            break;
                        case 3: //03-Entrada rejeitada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 03-Entrada rejeitada.";
                            break;
                        case 4: // 04 –Transferência de Carteira/Entrada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += "  04 –Transferência de Carteira/Entrada.";
                            break;
                        case 9: //Baixa
                        case 10:
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_PEDIDO_BAIXA;
                            if (detalhe.CodigoOcorrencia == 9)
                                tituloRetorno.tx_mensagem_retorno += "  04 –Transferência de Carteira/Entrada.";
                            else
                                tituloRetorno.tx_mensagem_retorno += "  10 - Baixa – Pedido Beneficiário.";
                            break;
                        case 26: //  26 – Instrução Rejeitada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 26 – Instrução Rejeitada.";
                            break;
                        case 30: //  30 – Alteração de Dados Rejeitada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 30 – Alteração de Dados Rejeitada.";
                            break;
                        case 37: //  37 - Envio de e-mail/SMS rejeitado
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 37 - Envio de e-mail/SMS rejeitado.";
                            break;
                        case 44: //  44 - Título pago com cheque devolvido
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 44 - Título pago com cheque devolvido.";
                            break;
                        case 5: // Transferência de Carteira/Baixa
                        case 6: // Liquidação
                        case 17: //17 - Liquidação Após Baixa ou Liquidação Título Não Registrado
                        case 45: //45 - Título pago com cheque compensado
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.GERAR_BAIXA;
                            break;
                        case 19: //19 - Confirmação Recebimento Instrução de Protesto
                        case 20: //20 - Confirmação Recebimento Instrução de Sustação/Cancelamento de Protesto
                        case 23: //23 - Remessa a Cartório (Aponte em Cartório)
                        case 24: //24 - Retirada de Cartório e Manutenção em Carteira
                        case 46: //46 - Instrução para cancelar protesto confirmada
                        case 47: //47 - Instrução para protesto para fins falimentares confirmada
                            switch (tituloRetorno.id_tipo_retorno)
                            {
                                case 19: tituloRetorno.tx_mensagem_retorno += " 19 – Confirmação Recebimento Instrução de Protesto"; break;
                                case 20: tituloRetorno.tx_mensagem_retorno += " 20 – Confirmação Recebimento Instrução de Sustação/Cancelamento de Protesto"; break;
                                case 23: tituloRetorno.tx_mensagem_retorno += " 23 - Remessa a Cartório (Aponte em Cartório)"; break;
                                case 24: tituloRetorno.tx_mensagem_retorno += " 24 – Protestado e Baixado (Baixa por ter sido Protestado)"; break;
                                case 46: tituloRetorno.tx_mensagem_retorno += " 46 - Instrução para cancelar protesto confirmada"; break;
                                case 47: tituloRetorno.tx_mensagem_retorno += " 47 - Instrução para protesto para fins falimentares confirmada"; break;
                            }
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.RETORNO_PROTESTO;
                            break;
                        default:
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            if (!string.IsNullOrEmpty(detalhe.MotivosRejeicao))
                                tituloRetorno.tx_mensagem_retorno += detalhe.MotivosRejeicao;
                            else
                                tituloRetorno.tx_mensagem_retorno += " Ocorrência não mapeada.";
                            break;
                    }
                    #endregion
                    break;
                case Bancos.Sicred:
                    //Removendo dígito verificador e ano do nosso número:
                    if (tituloRetorno.dc_nosso_numero.Length == 9 || tituloRetorno.dc_nosso_numero.Length == 8)
                        tituloRetorno.dc_nosso_numero = tituloRetorno.dc_nosso_numero.Substring(2, 6);
                    #region Ocorrência Sicred
                    switch (detalhe.CodigoOcorrencia)
                    {
                        case 2: //Entrada Confirmada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_TITULO;
                            break;
                        case 9: // 9 - Baixado automaticamente via arquivo
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_PEDIDO_BAIXA;
                            break;
                        case 3: //03-Entrada rejeitada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 03-Entrada rejeitada.";
                            break;
                        case 24: // 24 - Entrada rejeitada por CEP irregular.
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 24 - Entrada rejeitada por CEP irregular.";
                            break;
                        case 27: // 27 - Baixa rejeitada.
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 27 - Baixa rejeitada.";
                            break;
                        case 28: // 28 – Tarifa
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 28 – Tarifa.";
                            break;
                        case 29: // 29 - Rejeição do pagador
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 29 - Rejeição do pagador";
                            break;
                        case 30: // 30 – Alteração rejeitada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 30 – Alteração rejeitada";
                            break;
                        case 32: // 32 - Instrução rejeitada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 32 - Instrução rejeitada";
                            break;
                        case 34: // 34 - Retirado de cartório e manutenção em carteira
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 34 - Retirado de cartório e manutenção em carteira";
                            break;
                        case 6: // 6 - Liquidação Normal
                        case 10: // 10 - Baixado conforme instruções da cooperativa de crédito
                        case 15: //15 - Liquidação em cartório
                        case 17: //17 - Liquidação após baixa
                        case 35: //35 - Aceite do pagador
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.GERAR_BAIXA;
                            break;
                        case 19: //19 - Confirmação Recebimento Instrução de Protesto
                        case 20: //20 - Confirmação Recebimento Instrução de Sustação/Cancelamento de Protesto
                        case 23: //23 - Remessa a Cartório (Aponte em Cartório)
                            switch (tituloRetorno.id_tipo_retorno)
                            {
                                case 19: tituloRetorno.tx_mensagem_retorno += " 19 – Confirmação Recebimento Instrução de Protesto"; break;
                                case 20: tituloRetorno.tx_mensagem_retorno += " 20 – Confirmação Recebimento Instrução de Sustação/Cancelamento de Protesto"; break;
                                case 23: tituloRetorno.tx_mensagem_retorno += " 23 - Entrada de título em cartório"; break;
                            }
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.RETORNO_PROTESTO;
                            break;
                        default:
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            if (!string.IsNullOrEmpty(detalhe.MotivosRejeicao))
                                tituloRetorno.tx_mensagem_retorno += detalhe.MotivosRejeicao;
                            else
                                tituloRetorno.tx_mensagem_retorno += " Ocorrência não mapeada.";
                            break;
                    }
                    #endregion
                    break;
                case Bancos.CrediSIS:
                    #region Ocorrência CrediSIS
                    switch (detalhe.CodigoOcorrencia)
                    {
                        case 01: //Entrada Confirmada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_TITULO;
                            break;
                        case 02: //02 - Liquidação 
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.GERAR_BAIXA;
                            break;
                        case 03:
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_PEDIDO_BAIXA;
                            tituloRetorno.tx_mensagem_retorno += "  03 - Baixa manual.";
                            break; //03 - Baixa manual
                        case 06: //  06 – Inclusão do Boleto Rejeitado 
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 06 – Inclusão do Boleto Rejeitado .";
                            break;
                        case 08: //  08 – Alteração de Dados Rejeitada
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            tituloRetorno.tx_mensagem_retorno += " 08 – Alteração Rejeitada.";
                            break;
                        default:
                            tituloRetorno.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                            if (!string.IsNullOrEmpty(detalhe.MotivosRejeicao))
                                tituloRetorno.tx_mensagem_retorno += detalhe.MotivosRejeicao;
                            else
                                tituloRetorno.tx_mensagem_retorno += " Ocorrência não mapeada.";
                            break;
                    }
                    #endregion
                    break;
            }
        }

        [AllowAnonymous]
        public ActionResult getEnvioBoletoEmail(string parametros)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                //Preparando parâmetros para montagem do título
                int cd_escola = 0;
                int cd_carteira_cnab = 0;
                int cd_cnab = 0;
                DateTime? dtHoraAtualServAplic = null;
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                SendEmail sendEmail = new SendEmail();
                string host = "";
                int porta = 0;
                bool ssl = false;
                string remetente = "";
                string password = "";
                string userName = "";
                string dominio = "";
                string no_escola = "";
                bool id_mostrar_3_boletos = false;
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if ("cd_escola".Equals(parametrosHash[0]))
                            cd_escola = int.Parse(parametrosHash[1] + "");
                        else if ("no_escola".Equals(parametrosHash[0]))
                            no_escola = parametrosHash[1] + "";
                        else if ("cd_cnab".Equals(parametrosHash[0]))
                            cd_cnab = int.Parse(parametrosHash[1] + "");
                        else if ("cd_carteira_cnab".Equals(parametrosHash[0]))
                            cd_carteira_cnab = int.Parse(parametrosHash[1] + "");
                        else if ("PDataHoraAtual".Equals(parametrosHash[0]))
                            dtHoraAtualServAplic = DateTime.Parse(parametrosHash[1] + "");
                        else if ("host".Equals(parametrosHash[0]))
                            host = (parametrosHash[1] + "");
                        else if ("porta".Equals(parametrosHash[0]))
                            porta = int.Parse(parametrosHash[1] + "");
                        else if ("ssl".Equals(parametrosHash[0]))
                            ssl = bool.Parse(parametrosHash[1] + "");
                        else if ("remetente".Equals(parametrosHash[0]))
                            remetente = (parametrosHash[1] + "");
                        else if ("password".Equals(parametrosHash[0]))
                            password = (parametrosHash[1] + "");
                        else if ("userName".Equals(parametrosHash[0]))
                            userName = (parametrosHash[1] + "");
                        else if ("dominio".Equals(parametrosHash[0]))
                            dominio = (parametrosHash[1] + "");
                        else if ("id_mostrar_3_boletos".Equals(parametrosHash[0]))
                            id_mostrar_3_boletos = bool.Parse(parametrosHash[1]);
                    }
                }
                Cnab cnab = BusinessCnab.getGerarRemessa(cd_escola, cd_cnab);
                short nro_banco = short.Parse(cnab.LocalMovimento.Banco.nm_banco);
                ConfiguradorBoletoBancario configBoleto = new ConfiguradorBoletoBancario(nro_banco);
                Cedente cedente = configBoleto.gerarCedenteDefault(cnab);
                List<TituloCnab> titulos = cnab.TitulosCnab.ToList();

                sendEmail.host = host;
                sendEmail.porta = porta;
                sendEmail.ssl = ssl;
                sendEmail.remetente = remetente;
                sendEmail.password = password;
                sendEmail.userName = userName;
                sendEmail.dominio = dominio;
                sendEmail.assunto = "Dados para pagamento - " + no_escola;
                string nrBoletos = "";
                //Enviar um emailor título
                for (int i = 0; i < titulos.Count; i++)
                {
                    try
                    {
                        //Não depende de banco
                        Boletos boletos = new Boletos();
                        IBanco banco = new BoletoNet.Banco(nro_banco);

                        if (titulos[i].emailPessoaTitulo == null || titulos[i].emailPessoaTitulo == "")
                            retorno.AddMensagem(string.Format(Messages.msgErroEnvioBoleto, titulos[i].Titulo.Pessoa.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                        else
                        {
                            sendEmail.destinatario = titulos[i].emailPessoaTitulo;
                            Boleto boleto = new Boleto();
                            Int32[] codigos = new Int32[1];

                            codigos[0] = cd_cnab;
                            BoletoBancario boletoBanco = gerarBoletos(cd_escola, codigos, titulos.Where(t => t.cd_titulo == titulos[i].cd_titulo).ToList(), false, false).FirstOrDefault().boletoBancario;
                            sendEmail.PDFBoleto = boletoBanco.MontarPDF();
                            bool enviado = SendEmail.EnviarEmail(sendEmail);
                            if (!enviado)
                                retorno.AddMensagem(string.Format(Messages.msgErroEnvioEmailBoletoAluno, titulos[i].Titulo.Pessoa.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                            if (nrBoletos == "")
                                nrBoletos = " " + titulos[i].nm_parcela_e_titulo;
                            else
                                nrBoletos = nrBoletos + ", " + titulos[i].nm_parcela_e_titulo;
                        }
                    }
                    catch (Exception e)
                    {
                        retorno.AddMensagem(string.Format(Messages.msgErroEnvioEmailBoletoAluno, titulos[i].Titulo.Pessoa.no_pessoa), e.Message + e.StackTrace, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    }
                }
                if (nrBoletos != "")
                    retorno.AddMensagem(string.Format(Messages.msgEnvioBoleto, nrBoletos), null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return new RenderJsonActionResult { Result = JsonConvert.SerializeObject(retorno) };
            }
            catch (Exception ex)
            {
                gerarLogException(Messages.msgNotProcessReg, retorno, logger, ex);
                return new RenderJsonActionResult { Result = JsonConvert.SerializeObject(retorno) };
            }
        }

        [AllowAnonymous]
        public ActionResult getEnvioBoletoEmailTitulosCnab(string parametros)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                //Preparando parâmetros para montagem do título
                int cd_escola = 0;
                int cd_cnab = 0;
                Int32[] cd_titulos_cnab = new Int32[1];
                DateTime? dtHoraAtualServAplic = null;
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                SendEmail sendEmail = new SendEmail();
                string host = "";
                int porta = 0;
                bool ssl = false;
                string remetente = "";
                string password = "";
                string userName = "";
                string dominio = "";
                string no_escola = "";
                bool id_mostrar_3_boletos = false;
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if ("cd_escola".Equals(parametrosHash[0]))
                            cd_escola = int.Parse(parametrosHash[1] + "");
                        else if ("no_escola".Equals(parametrosHash[0]))
                            no_escola = parametrosHash[1] + "";
                        else if ("cd_titulos_cnab".Equals(parametrosHash[0]))
                            cd_titulos_cnab = Array.ConvertAll((parametrosHash[1] + "").Split(';'), s => Int32.Parse(s));
                        else if ("PDataHoraAtual".Equals(parametrosHash[0]))
                            dtHoraAtualServAplic = DateTime.Parse(parametrosHash[1] + "");
                        else if ("host".Equals(parametrosHash[0]))
                            host = (parametrosHash[1] + "");
                        else if ("porta".Equals(parametrosHash[0]))
                            porta = int.Parse(parametrosHash[1] + "");
                        else if ("ssl".Equals(parametrosHash[0]))
                            ssl = bool.Parse(parametrosHash[1] + "");
                        else if ("remetente".Equals(parametrosHash[0]))
                            remetente = (parametrosHash[1] + "");
                        else if ("password".Equals(parametrosHash[0]))
                            password = (parametrosHash[1] + "");
                        else if ("userName".Equals(parametrosHash[0]))
                            userName = (parametrosHash[1] + "");
                        else if ("dominio".Equals(parametrosHash[0]))
                            dominio = (parametrosHash[1] + "");
                        else if ("id_mostrar_3_boletos".Equals(parametrosHash[0]))
                            id_mostrar_3_boletos = bool.Parse(parametrosHash[1]);
                    }
                }

                List<TituloCnab> titulos = BusinessBoleto.getTitulosCnabBoletoByCnabs(cd_escola, null, cd_titulos_cnab).ToList();
                if (titulos.Count > 0)
                    cd_cnab = titulos.FirstOrDefault().cd_cnab;
                Cnab cnab = BusinessCnab.getCnabByRemessa(cd_escola, cd_cnab);
                short nro_banco = short.Parse(cnab.LocalMovimento.Banco.nm_banco);
                ConfiguradorBoletoBancario configBoleto = new ConfiguradorBoletoBancario(nro_banco);
                Cedente cedente = configBoleto.gerarCedenteDefault(cnab);
                //List<TituloCnab> titulos = cnab.TitulosCnab.ToList();

                sendEmail.host = host;
                sendEmail.porta = porta;
                sendEmail.ssl = ssl;
                sendEmail.remetente = remetente;
                sendEmail.password = password;
                sendEmail.userName = userName;
                sendEmail.dominio = dominio;
                sendEmail.assunto = "Boleto Fisk - " + no_escola;

                string nrBoletos = "";
                //Enviar um emailor título
                for (int i = 0; i < titulos.Count; i++)
                {
                    try
                    {
                        //Não depende de banco
                        Boletos boletos = new Boletos();
                        IBanco banco = new BoletoNet.Banco(nro_banco);

                        if (titulos[i].emailPessoaTitulo == null || titulos[i].emailPessoaTitulo == "")
                            retorno.AddMensagem(string.Format(Messages.msgErroEnvioBoleto, titulos[i].Titulo.Pessoa.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                        else
                        {
                            sendEmail.destinatario = titulos[i].emailPessoaTitulo;

                            Boleto boleto = new Boleto();
                            Int32[] codigos = new Int32[1];

                            codigos[0] = cd_cnab;
                            BoletoBancario boletoBanco = gerarBoletos(cd_escola, codigos, titulos.Where(t => t.cd_titulo == titulos[i].cd_titulo).ToList(), false, false).FirstOrDefault().boletoBancario;
                            List<AlternateView> corpoEmail = new List<AlternateView>();
                            corpoEmail.Add(boletoBanco.HtmlBoletoParaEnvioEmail());

                            sendEmail.PDFBoleto = boletoBanco.MontarPDF();
                            //bool enviado = SendEmail.EnviarEmail(sendEmail);
                            bool enviado = SendEmail.EnviarEmail(sendEmail, corpoEmail);
                            if (!enviado)
                                retorno.AddMensagem(string.Format(Messages.msgErroEnvioEmailBoletoAluno, titulos[i].Titulo.Pessoa.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                            if (nrBoletos == "")
                                nrBoletos = " " + titulos[i].nm_parcela_e_titulo;
                            else
                                nrBoletos = nrBoletos + ", " + titulos[i].nm_parcela_e_titulo;
                        }
                    }
                    catch (Exception e)
                    {
                        retorno.AddMensagem(string.Format(Messages.msgErroEnvioEmailBoletoAluno, titulos[i].Titulo.Pessoa.no_pessoa), e.Message + e.StackTrace, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    }
                }
                if (nrBoletos != "")
                    retorno.AddMensagem(string.Format(Messages.msgEnvioBoleto, nrBoletos), null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return new RenderJsonActionResult { Result = JsonConvert.SerializeObject(retorno) };
            }
            catch (Exception ex)
            {
                gerarLogException(Messages.msgNotProcessReg, retorno, logger, ex);
                return new RenderJsonActionResult { Result = JsonConvert.SerializeObject(retorno) };
            }
        }

        [AllowAnonymous]
        public ActionResult baixarPDFBoletosCNAB(string parametros)
        {
            bool abortou_requisicao = false;
            try
            {
                int cd_escola = 0;
                Int32[] cd_cnab = new Int32[1];
                DateTime? dtHoraAtualServAplic = null;
                bool id_mostrar_3_boletos = false;
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                logger.Debug("INICIOU ImprimirBoletos");
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if ("cd_escola".Equals(parametrosHash[0]))
                            cd_escola = int.Parse(parametrosHash[1] + "");
                        else if ("cd_cnab".Equals(parametrosHash[0]))
                            cd_cnab = Array.ConvertAll((parametrosHash[1] + "").Split(';'), s => Int32.Parse(s));
                        else if ("PDataHoraAtual".Equals(parametrosHash[0]))
                            dtHoraAtualServAplic = DateTime.Parse(parametrosHash[1] + "");
                        else if ("id_mostrar_3_boletos".Equals(parametrosHash[0]))
                            id_mostrar_3_boletos = bool.Parse(parametrosHash[1]);
                    }
                }

                //Verifica se o boleto já expirou:
                DateTime dtHoraAtualRelat = DateTime.Now;
                TimeSpan t = dtHoraAtualRelat - dtHoraAtualServAplic.Value;
                int timeout = int.Parse(ConfigurationManager.AppSettings["timeoutRelatorio"] + "");
                if (t.TotalSeconds > timeout)
                {
                    string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoSessaoRelatExp) + "&stackTrace=");
                }

                logger.Debug("início getTitulosCnabBoletoByCnabs");
                List<TituloCnab> titulos = BusinessBoleto.getTitulosCnabBoletoByCnabs(cd_escola, cd_cnab, null).ToList();
                logger.Debug("fim getTitulosCnabBoletoByCnabs");
                byte[] buf = this.GerarBoletosEMergePDFs(titulos, cd_escola, cd_cnab, false, id_mostrar_3_boletos);
                logger.Debug("fim  criação do pdf");
                Response.ContentType = "application/pdf";
                Cnab cnab = BusinessCnab.getGerarRemessa(cd_escola, cd_cnab[0]);

                if (!string.IsNullOrEmpty(cnab.LocalMovimento.Banco.nm_banco) && cnab.LocalMovimento.Banco.nm_banco == Utils.Utils.FormatCode((int)Cnab.Bancos.Inter + "", 3))
                {
                    bool exiteTituloSemNossoNumero = titulos.Where(x => x.dc_nosso_numero == null).Any();
                    if (exiteTituloSemNossoNumero)
                    {
                        string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                        return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroInterBoletoCnabSemNossoNumero) + "&stackTrace=");
                    }

                }
                logger.Debug(cnab.cd_contrato);
                string nome_arquivo = (cnab.cd_contrato == null || cnab.cd_contrato == 0) ? "boletos.pdf" : "CTR" + cnab.nro_contrato + ".pdf";
                logger.Debug(nome_arquivo);

                Response.AddHeader("content-disposition", "attachment;filename=" + nome_arquivo);
                Response.Buffer = true;
                Response.Clear();
                Response.OutputStream.Write(buf, 0, buf.Length);
                abortou_requisicao = true;
                Response.OutputStream.Flush();
                Response.End();

                logger.Debug("FINALIZOU ImprimirBoletos");
                return this.Content(buf.ToArray().ToString(), "application/pdf");
            }
            catch (CnabBusinessException cex)
            {
                string url = "";
                url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                logger.Warn(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarPDFBoleto, cex.InnerException);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(cex.Message) + "&stackTrace=");
            }
            catch (Exception ex)
            {
                string url = "";
                if (ex.InnerException != null && "NotImplementedException".Equals(ex.InnerException.GetType().Name))
                {
                    url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    logger.Warn(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarPDFBoleto, ex.InnerException);
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(ex.InnerException.Message) + "&stackTrace=");
                }

                url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                string stackTrace = HttpUtility.UrlEncode(ex.Message + ex.StackTrace + ex.InnerException);

                //TODO: Melhorar o código para um método post, já que método get tem limitação de caracteres. Enquanto isso, visualizar o erro através do log de relatorios configurado no web.config.
                //Trunca o tamanho da url para o método get:
                if (stackTrace.Length > 1250)
                    stackTrace = stackTrace.Substring(0, 1250);
                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarPDFBoleto, ex);
                if (!abortou_requisicao)
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarPDFBoleto) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
                else
                    return null;

            }
        }

        [AllowAnonymous]
        public ActionResult baixarPDFTitulosBoleto(string parametros)
        {
            return baixarPDFTitulosBoletosGenerica(parametros, Componentes.Utils.MD5CryptoHelper.KEY);
        }

        private ActionResult baixarPDFTitulosBoletosGenerica(string parametros, string key)
        {
            bool abortou_requisicao = false;
            try
            {
                int cd_escola = 0;
                Int32[] cd_titulo_cnab = new Int32[1];
                DateTime? dtHoraAtualServAplic = null;
                bool id_mostrar_3_boletos = false;
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, key);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if ("cd_escola".Equals(parametrosHash[0]))
                            cd_escola = int.Parse(parametrosHash[1] + "");
                        else if ("cd_titulos_cnab".Equals(parametrosHash[0]))
                            cd_titulo_cnab = Array.ConvertAll((parametrosHash[1] + "").Split(';'), s => Int32.Parse(s));
                        else if ("PDataHoraAtual".Equals(parametrosHash[0]))
                            dtHoraAtualServAplic = DateTime.Parse(parametrosHash[1] + "");
                        else if ("id_mostrar_3_boletos".Equals(parametrosHash[0]))
                            id_mostrar_3_boletos = bool.Parse(parametrosHash[1]);
                    }
                }

                //Verifica se o boleto já expirou:
                DateTime dtHoraAtualRelat = DateTime.Now;
                TimeSpan t = dtHoraAtualRelat - dtHoraAtualServAplic.Value;
                int timeout = int.Parse(ConfigurationManager.AppSettings["timeoutRelatorio"] + "");
                if (t.TotalSeconds > timeout)
                {
                    string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoSessaoRelatExp) + "&stackTrace=");
                }
                List<TituloCnab> titulos = BusinessBoleto.getTitulosCnabBoletoByTitulosCnab(cd_escola, cd_titulo_cnab).ToList();
                byte[] buf = this.GerarBoletosEMergePDFs(titulos, cd_escola, cd_titulo_cnab, true, id_mostrar_3_boletos);

                logger.Debug("início criação do pdf");
                logger.Debug("fim  criação do pdf");
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=boletos.pdf");
                Response.Buffer = true;
                Response.Clear();
                Response.OutputStream.Write(buf, 0, buf.Length);
                abortou_requisicao = true;
                Response.OutputStream.Flush();
                Response.End();

                logger.Debug("FINALIZOU ImprimirBoletos");
                return this.Content(buf.ToArray().ToString(), "application/pdf");
            }
            catch (CnabBusinessException cex)
            {
                string url = "";
                url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                logger.Warn(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarPDFBoleto, cex.InnerException);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(cex.Message) + "&stackTrace=");
            }
            catch (Exception ex)
            {
                string url = "";
                if (ex.InnerException != null && "NotImplementedException".Equals(ex.InnerException.GetType().Name))
                {
                    url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    logger.Warn(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarPDFBoleto, ex.InnerException);
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(ex.InnerException.Message) + "&stackTrace=");
                }

                url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                string stackTrace = HttpUtility.UrlEncode(ex.Message + ex.StackTrace + ex.InnerException);

                //TODO: Melhorar o código para um método post, já que método get tem limitação de caracteres. Enquanto isso, visualizar o erro através do log de relatorios configurado no web.config.
                //Trunca o tamanho da url para o método get:
                if (stackTrace.Length > 1250)
                    stackTrace = stackTrace.Substring(0, 1250);
                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarPDFBoleto, ex);
                if (!abortou_requisicao)
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroBaixarPDFBoleto) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
                else
                    return null;

            }
        }

        private bool ConverterEGerarPDFHtmlBoleto(string html, string outPut, bool id_mostrar_3_boletos)
        {
            MemoryStream ms = new MemoryStream();
            Utils.Utils.MontarPDF(html, true, id_mostrar_3_boletos).CopyTo(ms);
            byte[] buf;
            FileStream fs;
            using (fs = new FileStream(outPut, FileMode.Create))
            {
                buf = ms.ToArray();
                fs.Write(buf, 0, buf.Length);
                fs.Dispose();
                fs.Close();
            }
            return true;
        }

        private Byte[] GerarBoletosEMergePDFs(List<TituloCnab> titulos, int cd_escola, Int32[] cd_cnab, bool is_titulo, bool id_mostrar_3_boletos)
        {
            string serverUploadPath = "";
            try
            {
                var uploadPath = ConfigurationManager.AppSettings["caminhoContent"];
                serverUploadPath = uploadPath + "\\Boletos\\" + Guid.NewGuid();
                DirectoryInfo di = new DirectoryInfo(serverUploadPath);
                if (!di.Exists)
                    di.Create();
                string htmlBoleto = "";
                MemoryStream ms1 = new MemoryStream();
                int repeticoes = (titulos.Count() / 20) + 1;
                for (int i = 0; i < repeticoes; i++)
                {
                    List<TituloCnab> titulosSelecionados = titulos.Skip(i * 20).Take(20).ToList();
                    if (titulosSelecionados != null && titulosSelecionados.Count() > 0)
                    {
                        gerarBoletos(cd_escola, cd_cnab, titulosSelecionados, is_titulo, id_mostrar_3_boletos);
                        htmlBoleto = ViewBag.Boleto;
                        ViewBag.Boleto = null;
                        var uploadedFilePath = Path.Combine(serverUploadPath, Utils.Utils.geradorNomeAleatorio(36) + ".pdf");
                        if (!string.IsNullOrEmpty(htmlBoleto))
                            ConverterEGerarPDFHtmlBoleto(htmlBoleto, uploadedFilePath, id_mostrar_3_boletos);
                    }
                }
                DirectoryInfo diretorio = new DirectoryInfo(serverUploadPath);
                FileInfo[] arquivos = diretorio.GetFiles("*.pdf*");
                List<string> listDiretorios = arquivos.Select(x => x.FullName).ToList();
                string nomeDocumento = serverUploadPath + "\\merge.pdf";
                PDFMerge.CombineMultiplePDFs(listDiretorios, nomeDocumento);
                FileStream st = new FileStream(serverUploadPath + "\\merge.pdf", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                arquivos = diretorio.GetFiles("*.pdf*");
                listDiretorios = arquivos.Select(x => x.FullName).ToList();
                byte[] buffer = new byte[st.Length];
                st.Read(buffer, 0, (int)st.Length);
                st.Close();
                foreach (var aq in arquivos)
                    aq.Delete();
                di = new DirectoryInfo(serverUploadPath);
                if (di.Exists)
                    di.Delete();
                return buffer;
            }
            catch (CnabBusinessException ex)
            {
                deletarArquivosEPastaBoleto(serverUploadPath);
                throw new CnabBusinessException(ex.Message, ex, ex.tipoErro, ex.MostraStackTrace);
            }
            catch (Exception exe)
            {
                deletarArquivosEPastaBoleto(serverUploadPath);
                throw new Exception(exe.Message, exe);
            }
        }

        private void deletarArquivosEPastaBoleto(string serverUploadPath)
        {
            DirectoryInfo diretorio = new DirectoryInfo(serverUploadPath);
            FileInfo[] arquivos = diretorio.GetFiles("*.pdf*");
            List<string> listDiretorios = arquivos.Select(x => x.FullName).ToList();
            foreach (var aq in arquivos)
                aq.Delete();
            diretorio = new DirectoryInfo(serverUploadPath);
            if (diretorio.Exists)
                diretorio.Delete();
        }

        [AllowAnonymous]
        public ActionResult baixarPDFTitulosBoletoPortal(string parametros)
        {
            return baixarPDFTitulosBoletosGenerica(parametros, Componentes.Utils.MD5CryptoHelper.KEY_ECOMMERCE);
        }

        [AllowAnonymous]
        public ActionResult renovarSessao()
        {
            Session["KeepSessionAlive"] = "KeepAlive!";
            return null;
        }

       
        [AllowAnonymous]
        public ActionResult GerarRecibo(string parametros)
        {
            try
            {
               
                Hashtable parametrosPesquisa = new Hashtable();
                Hashtable parametrosRelatorio = new Hashtable();
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");

                int cdBaixaTitulo = 0;
                int cdEscola = 0;
                int cdOrigemTitulo = 0;
                LocalReport AppReportViewer = new LocalReport();
                AppReportViewer.DataSources.Clear();
                AppReportViewer.EnableExternalImages = false;
                FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();


                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                //configurações da página ex: margin, top, left ...
                string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutputFormat>PDF</OutputFormat>" +
                    //"<MarginTop>0.5in</MarginTop>" +
                    "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] bytes;
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if (parametrosHash[0].Equals("@pCodBaixaTitulo"))
                            cdBaixaTitulo = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pEscola"))
                            cdEscola = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pOrigemTitulo"))
                            cdOrigemTitulo = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].StartsWith("@"))
                            parametrosPesquisa.Add(parametrosHash[0].Substring(1, parametrosHash[0].Length - 1), parametrosHash[1]);
                        else
                            parametrosRelatorio.Add(parametrosHash[0], parametrosHash[1]);
                    }

                    //Verifica se passou o tempo estipulado no web.config para espirar a sessão do usuario.
                    if (parametrosRelatorio != null && parametrosRelatorio["PDataHoraAtual"] != null)
                    {
                        DateTime dtHoraAtualServAplic = DateTime.Parse(parametrosRelatorio["PDataHoraAtual"].ToString());
                        DateTime dtHoraAtualRelat = DateTime.Now;
                        TimeSpan t = dtHoraAtualRelat - dtHoraAtualServAplic;
                        int timeout = int.Parse(ConfigurationManager.AppSettings["timeoutRelatorio"] + "");
                        if (t.TotalSeconds > timeout)
                        {
                            //url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                            string url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(parametros);
                            Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoSessaoRelatExp) + "&stackTrace=");
                        }
                    }

                    dynamic recibo = null;
                    string path = string.Empty;

                    if (cdOrigemTitulo != (int)FundacaoFisk.SGF.GenericModel.Titulo.OrigemTitulo.CONTRATO)
                    {
                        recibo = apresentadorRelatorio.GetSourceReciboBaixa(cdBaixaTitulo, cdEscola);
                        path = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_RECIBO;
                    }
                    else
                    {
                        parametrosRelatorio.Remove("PNomeUsuario");
                        recibo = apresentadorRelatorio.getReciboPagemento(cdBaixaTitulo, cdEscola);
                        path = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_RECIBO_PAGAMENTO;
                    }
                    var caminhoRelatorio = string.Format("{0}{1}.rdlc", ConfigurationManager.AppSettings["caminhoRelatorio"], path);
                    AppReportViewer.ReportPath = caminhoRelatorio;

                List<Componentes.GenericModel.TO> sourceTO = new List<Componentes.GenericModel.TO>();
                    sourceTO.Add(recibo);

                    if (sourceTO != null && sourceTO.ToList<Componentes.GenericModel.TO>().Count > 0)
                    {
                        // relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", path, sourceTO, parametrosRelatorio);
                    }

                AppReportViewer.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter("PDataHoraAtual", parametrosRelatorio["PDataHoraAtual"].ToString()));
                AppReportViewer.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter("PEmpresa", parametrosRelatorio["PEmpresa"].ToString()));
                if (cdOrigemTitulo != (int)FundacaoFisk.SGF.GenericModel.Titulo.OrigemTitulo.CONTRATO)
                    AppReportViewer.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter("PNomeUsuario", parametrosRelatorio["PNomeUsuario"].ToString()));

                var dtReportData = sourceTO.ToList<Componentes.GenericModel.TO>();
                    AppReportViewer.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetReport", dtReportData));
                    AppReportViewer.Refresh();
                    bytes = AppReportViewer.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                    return File(bytes, mimeType);

            } catch (Exception ex) {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                //var url = HttpUtility.UrlDecode(Request.Url.ToString(), System.Text.Encoding.UTF8);

                url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                string msgErro = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.Error(ex.Message, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(msgErro) + "&stackTrace=");
            }
           
        }


        [AllowAnonymous]
        public ActionResult GerarReciboAgrupado(string parametros)
        {
            try
            {

                Hashtable parametrosPesquisa = new Hashtable();
                Hashtable parametrosRelatorio = new Hashtable();
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");

                string cdsTitulosSelecionados = "";
                int cdEscola = 0;
                int cdOrigemTitulo = 0;
                bool renderizarRelatorio = true;

                LocalReport AppReportViewer = new LocalReport();
                AppReportViewer.DataSources.Clear();
                AppReportViewer.EnableExternalImages = false;
                FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();


                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                //configurações da página ex: margin, top, left ...
                string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutputFormat>PDF</OutputFormat>" +
                    //"<MarginTop>0.5in</MarginTop>" +
                    "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] bytes;
                parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                string[] parametrosGet = parms.Split('&');

                for (int i = 0; i < parametrosGet.Length; i++)
                {
                    string[] parametrosHash = parametrosGet[i].Split('=');

                    if (parametrosHash[0].Equals("@pCdsTitulosSelecionados"))
                        cdsTitulosSelecionados = parametrosHash[1];
                    if (parametrosHash[0].Equals("@pEscola"))
                        cdEscola = int.Parse(parametrosHash[1]);
                    if (parametrosHash[0].StartsWith("@"))
                        parametrosPesquisa.Add(parametrosHash[0].Substring(1, parametrosHash[0].Length - 1), parametrosHash[1]);
                    else
                        parametrosRelatorio.Add(parametrosHash[0], parametrosHash[1]);
                }
                //Verifica se passou o tempo estipulado no web.config para espirar a sessão do usuario.
                if (parametrosRelatorio != null && parametrosRelatorio["PDataHoraAtual"] != null)
                {
                    DateTime dtHoraAtualServAplic = DateTime.Parse(parametrosRelatorio["PDataHoraAtual"].ToString());
                    DateTime dtHoraAtualRelat = DateTime.Now;
                    TimeSpan t = dtHoraAtualRelat - dtHoraAtualServAplic;
                    int timeout = int.Parse(ConfigurationManager.AppSettings["timeoutRelatorio"] + "");
                    if (t.TotalSeconds > timeout)
                    {
                        string url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(parametros);
                        Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoSessaoRelatExp) + "&stackTrace=");
                    }
                }


                ReciboAgrupadoUI recibo = null;
                string path = string.Empty;

                parametrosRelatorio.Remove("PNomeUsuario");
                recibo = apresentadorRelatorio.GetSourceReciboAgrupado(cdsTitulosSelecionados, cdEscola);
                path = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_RECIBO_PAGAMENTO_AGRUPADO;
                this.parcelasReciboAgrupado = recibo.parcelas;

                List<Componentes.GenericModel.TO> sourceTO = new List<Componentes.GenericModel.TO>();
                sourceTO.Add(recibo);

                var caminhoRelatorio = string.Format("{0}{1}.rdlc", ConfigurationManager.AppSettings["caminhoRelatorio"], path);
                AppReportViewer.ReportPath = caminhoRelatorio;


                if (sourceTO != null && sourceTO.ToList<Componentes.GenericModel.TO>().Count > 0)
                {
                    //  relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", path, sourceTO, parametrosRelatorio);
                }

                AppReportViewer.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter("PDataHoraAtual", parametrosRelatorio["PDataHoraAtual"].ToString()));
                AppReportViewer.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter("PEmpresa", parametrosRelatorio["PEmpresa"].ToString()));

                var dtReportData = sourceTO.ToList<Componentes.GenericModel.TO>();
                AppReportViewer.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetReport", dtReportData));
                AppReportViewer.SubreportProcessing += new SubreportProcessingEventHandler(renderizaSubRptReciboAgrupado);

                AppReportViewer.Refresh();
                bytes = AppReportViewer.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                return File(bytes, mimeType);

            }
            catch (Exception ex)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                //var url = HttpUtility.UrlDecode(Request.Url.ToString(), System.Text.Encoding.UTF8);

                url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                string msgErro = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.Error(ex.Message, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(msgErro) + "&stackTrace=");
            }

        }
        private void renderizaSubRptReciboAgrupado(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e)
        {
            try
            {
                FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();

                e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetSubReportReciboPagamentoAgrupado",
                    this.parcelasReciboAgrupado));
            }
            catch (Exception exe)
            {
                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
            }
        }




        #region Carne
        [AllowAnonymous]
        public ActionResult GerarCarne(string parametros)
        {
            try
            {
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");

                // Decode parametros
                var parametrosEmBytes = Convert.FromBase64String(parms); //parametros
                var parametrosDecode = Encoding.ASCII.GetString(parametrosEmBytes);
                ParametrosCarneUI carne = JsonConvert.DeserializeObject<ParametrosCarneUI>(parametrosDecode);

                ICoordenacaoBusiness businessCoordenacao = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IEscolaBusiness businessEscola = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();

                Parametro parametro = businessEscola.getParametrosBaixa(carne.cd_escola);

                LocalReport AppReportViewer = new LocalReport();
                AppReportViewer.DataSources.Clear();
                AppReportViewer.EnableExternalImages = false;

                var caminhoRelatorio = string.Format("{0}{1}.rdlc", ConfigurationManager.AppSettings["caminhoRelatorio"], FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_CARNE);
                AppReportViewer.ReportPath = caminhoRelatorio;
                AppReportViewer.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter("PImprimirCapaCarne", carne.imprimirCapaCarne.ToString()));

                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                //configurações da página ex: margin, top, left ...
                string deviceInfo =
                "<DeviceInfo>" +
                "<OutputFormat>PDF</OutputFormat>" +
                //"<MarginTop>0.5in</MarginTop>" +
                "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] bytes;

                var dtReportData = businessCoordenacao.getCarnePorContrato(carne.cd_contrato, carne.cd_escola, parametro, carne.contaSegura, carne.parcIniCarne, carne.parcFimCarne);

                if (dtReportData.Count == 0)
                    throw new Exception(Messages.msgRegNotEnc);

                AppReportViewer.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetReport", dtReportData));
                AppReportViewer.SubreportProcessing += new SubreportProcessingEventHandler(renderizaSubRptCarne);

                AppReportViewer.Refresh();
                bytes = AppReportViewer.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

                return File(bytes, mimeType);
            }
            catch (Exception ex)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                //var url = HttpUtility.UrlDecode(Request.Url.ToString(), System.Text.Encoding.UTF8);

                url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                string msgErro = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.Error(ex.Message, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(msgErro) + "&stackTrace=");
            }
        }

        private void renderizaSubRptCarne(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e)
        {
            try
            {
                IEscolaBusiness businessEscola = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();

                int cd_titulo = Convert.ToInt32(e.Parameters["cd_titulo"].Values[0]);
                int cd_pessoa_empresa = Convert.ToInt32(e.Parameters["cd_pessoa_empresa"].Values[0]);
                bool conta_segura = Convert.ToBoolean(e.Parameters["conta_segura"].Values[0]);

                e.DataSources.Add(new ReportDataSource("DataSetSubReport",
                    businessEscola.getTituloCarnePorContratoSubReport(cd_titulo, cd_pessoa_empresa, conta_segura)));
            }
            catch (Exception exe)
            {
                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
            }
        }


        #endregion

        #region Percentual de Faltas Grupo Avançado
        [AllowAnonymous]
        public ActionResult findTurmaPercentualFaltaGrupoAvancado(string parametros)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                // Decode parametros
                var parametrosEmBytes = Convert.FromBase64String(parametros);
                var parametrosDecode = Encoding.ASCII.GetString(parametrosEmBytes);
                ParametrosPercentualFaltaGrupoAvancadoUI percentualFaltaGrupoAvancadoUI = JsonConvert.DeserializeObject<ParametrosPercentualFaltaGrupoAvancadoUI>(parametrosDecode);

                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                Turma result = turmaBiz.findTurmaPercentualFaltaGrupoAvancado(percentualFaltaGrupoAvancadoUI.cd_escola, percentualFaltaGrupoAvancadoUI.cd_turma, percentualFaltaGrupoAvancadoUI.cd_turma_ppt, percentualFaltaGrupoAvancadoUI.id_turma_ppt);

                //se a turma for Personalizada pai
                if (percentualFaltaGrupoAvancadoUI.id_turma_ppt == true && percentualFaltaGrupoAvancadoUI.cd_turma_ppt == null)
                {
                    throw new TurmaBusinessException(Messages.msgTurmaPPTPai, null, 0, false);

                }

                //Verifica se o produtos é diferente de Ingles e Espanhol
                var produtos = new int[]
                {
                    (int) Produto.TipoProduto.INGLES,
                    (int) Produto.TipoProduto.ESPANHOL, 
                };

                if (!produtos.Contains(result.Curso.Produto.cd_produto))
                {
                    throw new TurmaBusinessException(Messages.msgProdutosExecute, null, 0, false);

                }


                //se a turma não for de nível avançado
                if (result.Curso.Nivel.cd_nivel != (int)Nivel.SituacaoHistorico.AVANCADO)
                {
                    throw new TurmaBusinessException(Messages.msgProdutosExecute, null, 0, false);

                }




                LocalReport AppReportViewer = new LocalReport();
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
                string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;
                //AppReportViewer.Reset();
                AppReportViewer.DataSources.Clear();
                AppReportViewer.EnableExternalImages = false;

                var nomeRelatorio = string.Format("{0}{1}.rdlc", ConfigurationManager.AppSettings["caminhoRelatorio"], FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_CARTA_AVISO);
                AppReportViewer.ReportPath = nomeRelatorio;

                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                //configurações da página ex: margin, top, left ...
                string deviceInfo =
                "<DeviceInfo>" +
                "<OutputFormat>PDF</OutputFormat>" +
                "</DeviceInfo>";


                Warning[] warnings;
                string[] streams;
                byte[] bytes;
                //string filenameParams = string.Format("{0}{1}", "Etiqueta_", cd_mala);

                DataTable dtReportData = new DataTable();

                DBHelper dbSql = new DBHelper(connectionString, providerName);

                DBParameter param1 = new DBParameter("@cd_turma", percentualFaltaGrupoAvancadoUI.cd_turma, DbType.Int32);

                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(param1);

                dtReportData = dbSql.ExecuteDataTable("sp_RptTurmaControleFalta", paramCollection, CommandType.StoredProcedure);

                AppReportViewer.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataRelatorio", dtReportData));
                AppReportViewer.Refresh();
                bytes = AppReportViewer.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

                turmaBiz.editTurmaControleFalta(percentualFaltaGrupoAvancadoUI.cd_turma);

                return File(bytes, mimeType);


            }
            catch (Exception ex)
            {
                //var url = HttpUtility.UrlDecode(Request.Url.ToString(), System.Text.Encoding.UTF8);
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];

                url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                string msgErro = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.Error(ex.Message, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(msgErro) + "&stackTrace=");
            }
        }

        [AllowAnonymous]
        public ActionResult GerarHistorico(string Parametros)
        {
            Hashtable parametrosPesquisa = new Hashtable();
            Hashtable parametrosRelatorio = new Hashtable();
            string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
            int cdAluno = 0;
            string noAluno = null;
            string produtos = null;
            string statustitulo = null;

            if (parms != null)
            {
                parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                string[] parametrosGet = parms.Split('&');

                for (int i = 0; i < parametrosGet.Length; i++)
                {
                    string[] parametrosHash = parametrosGet[i].Split('=');

                    if (parametrosHash[0].Equals("@cd_aluno"))
                        cdAluno = int.Parse(parametrosHash[1]);
                    if (parametrosHash[0].StartsWith("@"))
                        parametrosPesquisa.Add(parametrosHash[0].Substring(1, parametrosHash[0].Length - 1), parametrosHash[1]);
                    else
                    {
                        if (parametrosHash[0].Equals("no_aluno"))
                        {
                            noAluno = parametrosHash[1];
                        }
                        else
                        {
                            if (parametrosHash[0].StartsWith("Pmostrar"))
                            {
                                parametrosRelatorio.Add(parametrosHash[0], Boolean.Parse(parametrosHash[1]));
                            }
                            else
                            {
                                parametrosRelatorio.Add(parametrosHash[0], parametrosHash[1]);
                            }

                            if (parametrosHash[0].Equals("produtos"))
                            {
                                produtos = parametrosHash[1];
                            }

                            if (parametrosHash[0].Equals("PTitulos"))
                            {
                                statustitulo = parametrosHash[1];
                            }
                        }

                    }
                }
            }
            FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();

            parametrosRelatorio["PEscola"] = parametrosRelatorio["PEmpresa"];
            parametrosRelatorio.Remove("PEmpresa");
            parametrosRelatorio.Remove("PTipoRelatorio");
            parametrosRelatorio.Remove("produtos");

            parametrosRelatorio.Add("PMostrarSubHTurma", true);
            parametrosRelatorio.Add("PMostrarSubHTurmaAvaliacao", true);
            parametrosRelatorio.Add("PMostrarSubHAvaliacaoTurma", true);
            parametrosRelatorio.Add("PMostrarSubHEstagio", true);
            parametrosRelatorio.Add("PMostrarSubHEstagioAvaliacao", true);
            parametrosRelatorio.Add("PMostrarHSubHEventoAluno", true);
            parametrosRelatorio.Add("PMostrarHSubTitulos", false);
            parametrosRelatorio.Add("PMostrarHSubObs", true);
            parametrosRelatorio.Add("PMostrarHSubAtividades", true);
            parametrosRelatorio.Add("PMostrarHSubFollow", true);
            parametrosRelatorio.Add("PMostrarHSubItem", true);
            parametrosRelatorio.Add("PMostrarSubHTurmaConceito", true);
            parametrosRelatorio.Add("PMostrarSubHEstagioConceito", true);

            //Vai ser possível faze isto por termos somente 1 aluno por vez
            dtHistoricoTurma = apresentadorRelatorio.GetAlunos(cdAluno, 1, produtos, statustitulo);
            setReportParameter(dtHistoricoTurma, "PMostrarSubHTurma", false, parametrosRelatorio);
            dtHistoricoTurmaAvaliacao = apresentadorRelatorio.GetAlunos(cdAluno, 2, produtos, statustitulo);
            setReportParameter(dtHistoricoTurmaAvaliacao, "PMostrarSubHTurmaAvaliacao", false, parametrosRelatorio);
            dtHistoricoAvaliacaoTurma = apresentadorRelatorio.GetAlunos(cdAluno, 3, produtos, statustitulo);
            setReportParameter(dtHistoricoAvaliacaoTurma, "PMostrarSubHAvaliacaoTurma", false, parametrosRelatorio);
            dtHistoricoEstagio = apresentadorRelatorio.GetAlunos(cdAluno, 4, produtos, statustitulo);
            setReportParameter(dtHistoricoEstagio, "PMostrarSubHEstagio", false, parametrosRelatorio);
            dtHistoricoAvaliacaoEstagio = apresentadorRelatorio.GetAlunos(cdAluno, 5, produtos, statustitulo);
            setReportParameter(dtHistoricoAvaliacaoEstagio, "PMostrarSubHEstagioAvaliacao", false, parametrosRelatorio);
            dtHistoricoEventoAula1 = apresentadorRelatorio.GetAlunos(cdAluno, 6, produtos, statustitulo);
            //setReportParameter(dtHistoricoTurma, "PMostrarSubHEventoAluno", false, parametrosRelatorio);
            dtHistoricoEventoAula2 = apresentadorRelatorio.GetAlunos(cdAluno, 7, produtos, statustitulo);
            setReportParameter(dtHistoricoEventoAula2, "PMostrarHSubHEventoAluno", false, parametrosRelatorio);
            dtHistoricoTitulo = apresentadorRelatorio.GetAlunos(cdAluno, 8, produtos, statustitulo);
            setReportParameter(dtHistoricoTitulo, "PMostrarHSubTitulos", false, parametrosRelatorio);
            dtHistoricoObs = apresentadorRelatorio.GetAlunos(cdAluno, 9, produtos, statustitulo);
            setReportParameter(dtHistoricoObs, "PMostrarHSubObs", false, parametrosRelatorio);
            dtHistoricoAtividade = apresentadorRelatorio.GetAlunos(cdAluno, 10, produtos, statustitulo);
            setReportParameter(dtHistoricoAtividade, "PMostrarHSubAtividades", false, parametrosRelatorio);
            dtHistoricoFollow = apresentadorRelatorio.GetAlunos(cdAluno, 11, produtos, statustitulo);
            setReportParameter(dtHistoricoFollow, "PMostrarHSubFollow", false, parametrosRelatorio);
            dtHistoricoItem = apresentadorRelatorio.GetAlunos(cdAluno, 12, produtos, statustitulo);
            setReportParameter(dtHistoricoItem, "PMostrarHSubItem", false, parametrosRelatorio);
            dtHistoricoTurmaConceito = apresentadorRelatorio.GetAlunos(cdAluno, 13, produtos, statustitulo);
            setReportParameter(dtHistoricoTurmaConceito, "PMostrarSubHTurmaConceito", false, parametrosRelatorio);
            dtHistoricoEstagioConceito = apresentadorRelatorio.GetAlunos(cdAluno, 14, produtos, statustitulo);
            setReportParameter(dtHistoricoEstagioConceito, "PMostrarSubHEstagioConceito", false, parametrosRelatorio);


            Hashtable sourceHash = new Hashtable();

            List<sp_RptHistoricoAlunoM_Result> dtReportData = apresentadorRelatorio.GetRtpHistoricoAlunoM(cdAluno);
            List<Componentes.GenericModel.TO> sourceTO = null;
            if (dtReportData.Count() > 0)
            {
                sourceTO = dtReportData.ToList<Componentes.GenericModel.TO>();
            }

            LocalReport AppReportViewer = new LocalReport();
            AppReportViewer.DataSources.Clear();
            AppReportViewer.EnableExternalImages = false;

            var nomeRelatorio = string.Format("{0}{1}.rdlc", ConfigurationManager.AppSettings["caminhoRelatorio"], FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_HISTORICO_ALUNO);
            AppReportViewer.ReportPath = nomeRelatorio;

            foreach (string key in parametrosRelatorio.Keys)
            {
                Microsoft.Reporting.WebForms.ReportParameter reportParameter = new Microsoft.Reporting.WebForms.ReportParameter(key, (parametrosRelatorio[key] != null) ? parametrosRelatorio[key].ToString() : null);
                AppReportViewer.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter[1] { reportParameter });
            }


            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //configurações da página ex: margin, top, left ...
            string deviceInfo =
            "<DeviceInfo>" +
            "<OutputFormat>PDF</OutputFormat>" +
            "</DeviceInfo>";


            Warning[] warnings;
            string[] streams;
            byte[] bytes;

            if (sourceTO != null && sourceTO.ToList<Componentes.GenericModel.TO>().Count > 0)
            {
                try
                {
                    sourceHash["DataHistoricoAluno"] = dtReportData;
                    AppReportViewer.DataSources.Add(new ReportDataSource("DataHistoricoAluno", dtReportData));
                    AppReportViewer.SubreportProcessing += new SubreportProcessingEventHandler(HTurmaSubReportProcessing);
                    AppReportViewer.Refresh();
                    AppReportViewer.DisplayName = noAluno; //+ "(" + DateTime.Now.ToShortDateString() + ")";
                    bytes = AppReportViewer.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                    string FileName = AppReportViewer.DisplayName + ".pdf";
                    FileName = FileName.Replace(" ", "");
                    FileName = ConfigurationManager.AppSettings["caminhoContent"] + "\\TempContratos\\" + FileName;
                    //using (FileStream fs = new FileStream(ConfigurationManager.AppSettings["caminhoContent"] + "\\TempContratos\\testede arquivo nome(grande)" + ".pdf", FileMode.Create))
                    using (FileStream fs = new FileStream(FileName, FileMode.Create))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    var base64Data = "data:application / pdf; base64," + Convert.ToBase64String(bytes);
                    //Vai excluir o arquivo depois
                    //File.Delete(FileName);

                    return null; //File(bytes, mimeType);
                }
                catch (Exception e)
                {
                    string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(e.Message) + "&stackTrace=");
                }
            }
            else
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(Messages.msgRegNotEnc) + "&stackTrace=");
            }
        }
        private void setReportParameter(DataTable dt, string name, Boolean value, Hashtable parametrosRelatorio)
        {
            if (dt.Rows.Count == 0)
            {
                parametrosRelatorio[name] = value;
            }
        }

        private void HTurmaSubReportProcessing(object sender, SubreportProcessingEventArgs e)
        {

            int cd_atividade = int.Parse(e.Parameters["cd_aluno"].Values[0].ToString());
            if (e.ReportPath.Contains("HistoricoTurmas"))
            {
                DataTable dtRptSub = dtHistoricoTurma; // GetAlunos(cd_atividade, 1);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistorico", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoTurmaAvaliacao"))
            {
                DataTable dtRptSub = dtHistoricoTurmaAvaliacao; //GetAlunos(cd_atividade, 2);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataTurmaAvaliacao", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoAvaliacaoTurma"))
            {
                DataTable dtRptSub = dtHistoricoAvaliacaoTurma; // GetAlunos(cd_atividade, 3);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataAvaliacaoTurma", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath == "HistoricoEstagio")
            {
                DataTable dtRptSub = dtHistoricoEstagio; // GetAlunos(cd_atividade, 4);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataEstagio", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoAvaliacaoEstagio"))
            {
                DataTable dtRptSub = dtHistoricoAvaliacaoEstagio; // GetAlunos(cd_atividade, 5);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataAvaliacaoEstagio", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoEventoAula"))
            {
                DataTable dtRptSub = dtHistoricoEventoAula1; // GetAlunos(cd_atividade, 6);
                Microsoft.Reporting.WebForms.ReportDataSource ds6 = new Microsoft.Reporting.WebForms.ReportDataSource("DataAlunoEvento", dtRptSub);
                e.DataSources.Add(ds6);
                DataTable dtRptSub7 = dtHistoricoEventoAula2; // GetAlunos(cd_atividade, 7);
                Microsoft.Reporting.WebForms.ReportDataSource ds7 = new Microsoft.Reporting.WebForms.ReportDataSource("DataAlunoAula", dtRptSub7);
                e.DataSources.Add(ds7);
            }
            else if (e.ReportPath.Contains("HistoricoTitulo"))
            {
                DataTable dtRptSub = dtHistoricoTitulo; // GetAlunos(cd_atividade, 8);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoTitulo", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoObs"))
            {
                DataTable dtRptSub = dtHistoricoObs; // GetAlunos(cd_atividade, 9);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoObs", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoAtividade"))
            {
                DataTable dtRptSub = dtHistoricoAtividade; // GetAlunos(cd_atividade, 10);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoAtividade", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoFollow"))
            {
                DataTable dtRptSub = dtHistoricoFollow; // GetAlunos(cd_atividade, 11);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoFollow", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoItem"))
            {
                DataTable dtRptSub = dtHistoricoItem; // GetAlunos(cd_atividade, 12);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoItem", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoTurmaConceito"))
            {
                DataTable dtRptSub = dtHistoricoTurmaConceito; // GetAlunos(cd_atividade, 13);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoTurmaConceito", dtRptSub);
                e.DataSources.Add(ds);
            }
            else if (e.ReportPath.Contains("HistoricoEstagioConceito"))
            {
                DataTable dtRptSub = dtHistoricoEstagioConceito; // GetAlunos(cd_atividade, 14);
                Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoEstagioConceito", dtRptSub);
                e.DataSources.Add(ds);
            }
        }

        [AllowAnonymous]
        public ActionResult GerarCartas(string Parametros)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
                Dictionary<string, AlunoCartaEmail> filesAlunoEmail = new Dictionary<string, AlunoCartaEmail>();

                int cdEscola = 0;
                int ano = 0;
                String cdPessoa = "0";

                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                SendEmail sendEmail = new SendEmail();
                string host = "";
                int porta = 0;
                bool ssl = false;
                string remetente = "";
                string password = "";
                string userName = "";
                string dominio = "";
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if ("cdEscola".Equals(parametrosHash[0]))
                            cdEscola = int.Parse(parametrosHash[1] + "");
                        else if ("cdPessoa".Equals(parametrosHash[0]))
                            cdPessoa = (parametrosHash[1] + "");
                        else if ("ano".Equals(parametrosHash[0]))
                            ano = int.Parse(parametrosHash[1] + "");
                        else if ("host".Equals(parametrosHash[0]))
                            host = (parametrosHash[1] + "");
                        else if ("porta".Equals(parametrosHash[0]))
                            porta = int.Parse(parametrosHash[1] + "");
                        else if ("ssl".Equals(parametrosHash[0]))
                            ssl = bool.Parse(parametrosHash[1] + "");
                        else if ("remetente".Equals(parametrosHash[0]))
                            remetente = (parametrosHash[1] + "");
                        else if ("password".Equals(parametrosHash[0]))
                            password = (parametrosHash[1] + "");
                        else if ("userName".Equals(parametrosHash[0]))
                            userName = (parametrosHash[1] + "");
                        else if ("dominio".Equals(parametrosHash[0]))
                            dominio = (parametrosHash[1] + "");
                    }
                }

                List<int> cdsPessoas = new List<int>();
                if (!String.IsNullOrEmpty(cdPessoa))
                {
                    cdsPessoas = cdPessoa.Split(new char[] { '|' }).Select(x => int.Parse(x)).ToList();

                }

                bool existePessoaJaCadastradaLista = false;
                //Preenche a lista com os arquivos do zip e do email
                if (cdsPessoas != null && cdsPessoas.Count > 0)
                {
                    foreach (int idPessoa in cdsPessoas)
                    {
                        PessoaCarta pessoaCarta = PessoaCartaDataAccess.findByIdPessoaAndAno(idPessoa,  ano);
                        if (pessoaCarta == null)
                        {
                            Dictionary<string, AlunoCartaEmail> fileNovaCarta = novaCarta(cdEscola, idPessoa, ano);
                            if (fileNovaCarta != null && fileNovaCarta.Count > 0) //verifica se é nulo
                            {
                                foreach (KeyValuePair<string, AlunoCartaEmail> file in fileNovaCarta)
                                {
                                    files.Add(file.Key, file.Value.arquivo);
                                    if (!String.IsNullOrEmpty(file.Value.email))//email => somente para quem tem email cadastrado
                                    {
                                        filesAlunoEmail.Add(file.Key, file.Value);
                                    }

                                }


                            }
                        }
                        else
                        {
                            existePessoaJaCadastradaLista = true;
                        }
                        
                    }
                }

                //se veio pessoas que ja foi enviado email e não vai enviar nenhum retorn a mensagem
                if (existePessoaJaCadastradaLista && filesAlunoEmail.Count == 0)
                {
                    string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErrorEmailCartaQuitacaoJaEnviada) + "&stackTrace=");
                }

                //envia os emails
                if(filesAlunoEmail != null && filesAlunoEmail.Count > 0)
                {
                    enviarEmailCartasGeradas(filesAlunoEmail, ano, host, porta, ssl, remetente, password, userName, dominio);
                }

                //retorna o zip
                return File(ZipFiles(files), "application/x-zip-compressed", String.Format("Cartas_{0}_{1}_{2}.zip", cdEscola, ano, Guid.NewGuid().ToString()));


                //return File(bytes, mimeType);
            }
            catch (Exception ex)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];

                url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                string msgErro = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.Error(ex.Message, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(msgErro) + "&stackTrace=");
            }
        }

        private Dictionary<string, AlunoCartaEmail> novaCarta(int cdEscola, int idPessoa, int ano)
        {


            LocalReport AppReportViewer = new LocalReport();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
            string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;
            //AppReportViewer.Reset();
            AppReportViewer.DataSources.Clear();
            AppReportViewer.EnableExternalImages = false;

            var nomeRelatorio = string.Format("{0}{1}.rdlc", ConfigurationManager.AppSettings["caminhoRelatorio"], FundacaoFisk.SGF.Utils.ReportParameter.CARTA_QUITACAO);
            AppReportViewer.ReportPath = nomeRelatorio;

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //configurações da página ex: margin, top, left ...
            string deviceInfo =
                "<DeviceInfo>" +
                "<OutputFormat>PDF</OutputFormat>" +
                "</DeviceInfo>";


            Warning[] warnings;
            string[] streams;
            byte[] bytes;
            //string filenameParams = string.Format("{0}{1}", "Etiqueta_", cd_mala);

            DataTable dtReportData = new DataTable();

            DBHelper dbSql = new DBHelper(connectionString, providerName);

            DBParameter param1 = new DBParameter("@cd_escola", cdEscola, DbType.Int32);
            DBParameter param2 = new DBParameter("@ano", ano, DbType.Int32);
            DBParameter param3 = new DBParameter("@cd_pessoa", idPessoa, DbType.Int32);

            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param1);
            paramCollection.Add(param2);
            paramCollection.Add(param3);

            dtReportData = dbSql.ExecuteDataTable("sp_RptCartaQuitacao", paramCollection, CommandType.StoredProcedure);

            AppReportViewer.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataReport", dtReportData));
            AppReportViewer.Refresh();
            bytes = AppReportViewer.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            

            Dictionary<string, AlunoCartaEmail> lista = new Dictionary<string, AlunoCartaEmail>();

            if (dtReportData.Rows != null)
            {
                foreach (DataRow row in dtReportData.Rows)
                {
                    if (row != null)
                    {

                        string valueId = row["cd_pessoa"].ToString();
                        string valueNoResponsavel = row["no_responsavel"].ToString();
                        string valueEmail = row["dc_fone_mail"].ToString();
                        
                        if (!String.IsNullOrEmpty(valueId) && !String.IsNullOrEmpty(valueNoResponsavel))
                        {
                            lista.Add(valueNoResponsavel + ".pdf",  new AlunoCartaEmail
                            {
                                cd_responsavel = valueId,
                                no_responsavel = valueNoResponsavel,
                                email = valueEmail,
                                arquivo = bytes
                            });

                        }
                    }
                }
            }
            return lista;
        }

        public class AlunoCartaEmail
        {
           public string cd_responsavel { get; set; }
           public string no_responsavel { get; set; }
           public string email { get; set; }
           public byte[] arquivo { get; set; }

        }

        public static byte[] ZipFiles(Dictionary<string, byte[]> files)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Update))
                {
                    foreach (var file in files)
                    {
                        ZipArchiveEntry orderEntry = archive.CreateEntry(file.Key); //create a file with this name
                        using (BinaryWriter writer = new BinaryWriter(orderEntry.Open()))
                        {
                            writer.Write(file.Value); //write the binary data
                        }
                    }
                }
                //ZipArchive must be disposed before the MemoryStream has data
                return ms.ToArray();
            }
        }


        public List<String> enviarEmailCartasGeradas(Dictionary<string, AlunoCartaEmail> filesAlunoEmail, int ano,
        string host ,int porta, bool ssl, string remetente,string password,string userName,string dominio)
        {
            List<String> retorno = new List<string>();

            SendEmail sendEmail = new SendEmail();
            sendEmail.assunto = " Carta de Quitação Anual de Débitos";
            sendEmail.host = host;
            sendEmail.porta = porta;
            sendEmail.ssl = ssl;
            sendEmail.remetente = remetente;
            sendEmail.password = password;
            sendEmail.userName = userName;
            sendEmail.dominio = dominio;
            //SendEmail.configurarEmailSection(sendEmail);


                if (filesAlunoEmail != null && filesAlunoEmail.Count > 0)
                {
                    foreach (var fileAlunoCartaEmail in filesAlunoEmail)
                    {
                        sendEmail.destinatario = fileAlunoCartaEmail.Value.email;

                        sendEmail.mensagem = Utils.Utils.GetTemplateCartaQuitacao("Carta de Quitação Anual de Débitos.", fileAlunoCartaEmail.Value.no_responsavel, ano);
                        Dictionary<string, Stream> anexos = new Dictionary<string, Stream>();
                        anexos.Add("Carta_Quitacao.pdf", new MemoryStream(fileAlunoCartaEmail.Value.arquivo));
                        sendEmail.Anexos =  anexos; 
                        
                        bool enviado = SendEmail.EnviarEmail(sendEmail);
                        if (!enviado)
                        {
                            retorno.Add(string.Format(Messages.msgErroSendEmailCartaQuitacao, fileAlunoCartaEmail.Value.no_responsavel, fileAlunoCartaEmail.Value.email));
                        }
                        else
                        {
                            PessoaCarta pessoaCarta = new PessoaCarta()
                            {
                                cd_pessoa = int.Parse(fileAlunoCartaEmail.Value.cd_responsavel),
                                nm_ano_carta = (short)ano
                            };
                            PessoaCartaDataAccess.add(pessoaCarta, false);
                        }
                        
                    }
                }

            

            return retorno;
        }

        /**
            * Calcula e retorna o digito do nosso número para a remessa do banco Sicoob / Chamado:270815 
        */
        public string getDigitoNossoNumeroSicoob(Boleto boleto)
        {
            //Variaveis
            int resultado = 0;
            int dv = 0;
            int resto = 0;
            String constante = "319731973197319731973";
            String cooperativa = boleto.Cedente.ContaBancaria.Agencia;
            String codigo = (boleto.Cedente.Codigo.Substring(0, 6).PadLeft(6, '0') ) + (boleto.Cedente.Codigo.Length == 7 ? Convert.ToInt32(boleto.Cedente.Codigo.Substring(6)).ToString(): boleto.Cedente.DigitoCedente.ToString());
            String nossoNumero = boleto.NossoNumero;
            StringBuilder seqValidacao = new StringBuilder();

            /*
             * Preenchendo com zero a esquerda
             */
            //Tratando cooperativa
            for (int i = 0; i < 4 - cooperativa.Length; i++)
            {
                seqValidacao.Append("0");
            }
            seqValidacao.Append(cooperativa);
            //Tratando cliente
            for (int i = 0; i < 10 - codigo.Length; i++)
            {
                seqValidacao.Append("0");
            }
            seqValidacao.Append(codigo);
            //Tratando nosso número
            for (int i = 0; i < 7 - nossoNumero.Length; i++)
            {
                seqValidacao.Append("0");
            }
            seqValidacao.Append(nossoNumero);

            /*
             * Multiplicando cada posição por sua respectiva posição na constante.
             */
            for (int i = 0; i < 21; i++)
            {
                resultado = resultado + (Convert.ToInt16(seqValidacao.ToString().Substring(i, 1)) * Convert.ToInt16(constante.Substring(i, 1)));
            }
            //Calculando mod 11
            resto = resultado % 11;
            //Verifica resto
            if (resto == 1 || resto == 0)
            {
                dv = 0;
            }
            else
            {
                dv = 11 - resto;
            }
            //Montando nosso número
            //boleto.NossoNumero = boleto.NossoNumero + "-" + dv.ToString();
            //boleto.DigitoNossoNumero = dv.ToString();
            return dv.ToString();
        }

        
        #endregion

        #region Etiqueta
        [AllowAnonymous]
        public ActionResult GerarEtiqueta(string parametros)
        {
            try
            {
                // Decode parametros
                var parametrosEmBytes = Convert.FromBase64String(parametros);
                var parametrosDecode = Encoding.ASCII.GetString(parametrosEmBytes);
                int cd_mala_direta = JsonConvert.DeserializeObject<int>(parametrosDecode);

                IEmailMarketingBusiness BusinessEmailMarketing = (IEmailMarketingBusiness)base.instanciarBusiness<IEmailMarketingBusiness>();

                LocalReport AppReportViewer = new LocalReport();
                AppReportViewer.DataSources.Clear();
                AppReportViewer.EnableExternalImages = true;

                var caminhoRelatorio = string.Format("{0}{1}.rdlc", ConfigurationManager.AppSettings["caminhoRelatorio"], FundacaoFisk.SGF.Utils.ReportParameter.ETIQUETA);
                AppReportViewer.ReportPath = caminhoRelatorio;

                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                //configurações da página ex: margin, top, left ...
                string deviceInfo =
                "<DeviceInfo>" +
                "<OutputFormat>PDF</OutputFormat>" +
                "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] bytes;

                var dtReportData = BusinessEmailMarketing.gerarEtiqueta(cd_mala_direta);
                var contadorLinhas = 0;

                foreach (DataRow row in dtReportData.Rows)
                {
                    foreach (DataColumn column in dtReportData.Columns)
                    {
                        if (column.ToString() == "no_pessoa")
                        {
                            if (row[column] == DBNull.Value)
                                break;
                            else
                                contadorLinhas++;
                        }
                    }
                }

                if (contadorLinhas == 0)
                    throw new Exception(Messages.msgRegNotEnc);

                AppReportViewer.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataEtiqueta", dtReportData));
                AppReportViewer.Refresh();
                bytes = AppReportViewer.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

                return File(bytes, mimeType);
            }
            catch (Exception ex)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                //var url = HttpUtility.UrlDecode(Request.Url.ToString(), System.Text.Encoding.UTF8);

                url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                string msgErro = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.Error(ex.Message, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(msgErro) + "&stackTrace=");
            }
        }

        #endregion

        //Return View
        [AllowAnonymous]
        public ActionResult EnvioBoletoEmail() { return View(); }
        [AllowAnonymous]
        public ActionResult NaoInscrito() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioAtividadeExtra() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioAulaReposicaoView() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioAulaPersonalizada() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioAulaReposicao() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioBalanceteMensal() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioBolsistas() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioCarne() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioCarneMovto() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioCheques() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioComissaoSecretarias() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioContaCorrente() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioControleSala() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioContVendasMaterial() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioCopiaEspelhoMovimento() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioDinamico() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioEspelhoMovimento() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioEstoque() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioEvento() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioFechamentoCaixaSint() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioHistoricoAluno() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioControleFaltas() { return View(); }
        [AllowAnonymous]
        public ActionResult ReportGestaoAtividadeExtra() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioListagemAniversariantes() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioListagemEnderecosMMK() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioMatriculaAnalitico() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioMediaAlunos() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioPagamentoProfessores() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioPercentualTerminoEstagio() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioPosicaoFinanceira() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioProgramacaoAulasTurma() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioProspectAtendido() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioRecibo() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioReciboAgrupado() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioReciboConfirmacao() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioReciboMovimento() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioReciboProspect() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioRetornoTitulosCNAB() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioSaldoFinanceiro() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioTitulosCNAB() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioTurma() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioTurmaMatriculaMaterial() { return View(); }
        [AllowAnonymous]
        public ActionResult VisualizadorBoleto() { return View(); }
        [AllowAnonymous]
        public ActionResult VisualizadorTitulosBoleto() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioAlunosSemTituloGerado() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioFaixaEtaria() { return View(); } 
        [AllowAnonymous]
        public ActionResult RelatorioFollowUp() { return View(); }
        public ActionResult RelatorioInventario() { return View(); }
        public ActionResult RelatorioAlunoRestricao() { return View(); }
        public ActionResult RelatorioAvaliacao() { return View(); }
        [AllowAnonymous]
        public ActionResult RelatorioMatriculaOutros() { return View(); }
        public ActionResult RelatorioLoginEscola() { return View(); }
        public ActionResult RelatorioAlunoCliente() { return View(); }


    }
}