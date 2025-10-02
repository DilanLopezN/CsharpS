<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="FundacaoFisk.SGF.Web.Services.Financeiro.Model" %>
<%@ Import Namespace="Componentes.GenericModel" %>
<%@ Register Assembly="Componentes.ApresentadorRelatorio" Namespace="Componentes.ApresentadorRelatorio" TagPrefix="cc1" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <title>Relatório</title>
</head>
<body>
    <div style="Width:100%;height:100%;">
        <script runat="server">
            private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioPosicaoFinanceira.aspx");
            private enum TipoMovimento
            {
                RECEBER = 0,
                PAGAR = 1,
                RECEBIDAS = 2,
                PAGAS = 3
            };
            private enum Ordenacao {
                VENCIMENTO = 0,
                PESSOA = 1,
                PLANO_CONTA = 2
            };

            private enum Situacao {
                AGUARDANDO_MATRÍCULA= 9,
                MATRICULADO= 1,
                REMATRICULADO= 8,
                DESISTENTE= 2,
                ENCERRADO= 4,
                MOVIDO= 0
            };

            int cdEscola = 0;

            public void Page_Load(object sender, System.EventArgs e)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                try {
                    if(!IsPostBack)
                        bindRelatorio(url);
                }
                catch(Exception exe) {
                    RedirecionaAplicacaoPrincipal(url, exe);
                }
            }

            private void RedirecionaAplicacaoPrincipal(string url, Exception exe)
            {
                try
                {
                    //url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                    string stackTrace = HttpUtility.UrlEncode(exe.Message + exe.StackTrace + exe.InnerException);

                    //TODO: Melhorar o código para um método post, já que método get tem limitação de caracteres. Enquanto isso, visualizar o erro através do log de relatorios configurado no web.config.
                    //Trunca o tamanho da url para o método get:
                    if (stackTrace.Length > 1450)
                        stackTrace = stackTrace.Substring(0, 1450);
                    if (exe.Message != Componentes.Utils.Messages.Messages.msgSessaoExpirada)
                        logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
                    Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroRenderizarRelatorio) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
                }
                catch (Exception e)
                {
                    logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError + " url: " + url, e);
                }
            }

            private void Page_Init(object sender, System.EventArgs e) {
                Context.Handler = this.Page;
            }

            private void bindRelatorio(string url){
                Hashtable parametrosPesquisa = new Hashtable();
                Hashtable parametrosRelatorio = new Hashtable();
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                DateTime? pDtaI = null;
                DateTime? pDtaF = null;
                int pForn = 0;
                string situacoes = "100";
                DateTime? pDtaBase = null;
                byte pNatureza = 2;

                int pPlanoContas = 0;
                int tipo = 0;
                int ordem = 0;
                bool renderizarRelatorio = true;
                bool pMostraCCManual = false;
                bool pMostraRecibo = false;
                bool pMostrarCpfResponsavel = false;
                bool cSegura = false;
                int cdTpLiq = 0;
                int cdTpFinan = 0;
                int cdTurma = 0;
                string no_turma = "";
                bool? ckCancelamento = null;
                int cdLocal = 0;


                if(parms != null) {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for(int i = 0; i < parametrosGet.Length; i++) {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if (parametrosHash[0].Equals("@pEscola"))
                            cdEscola = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pDtaI"))
                            pDtaI = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pDtaF"))
                            pDtaF = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pForn"))
                            pForn = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pDtaBase"))
                            pDtaBase = DateTime.Parse(parametrosHash[1]);
                        if(parametrosHash[0].Equals("PNatureza"))
                            pNatureza = byte.Parse(parametrosHash[1]);
                        if(parametrosHash[0].Equals("PMostraCCManual"))
                            pMostraCCManual = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@PMostrarRecibo"))
                            pMostraRecibo = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pPlanoContas"))
                            pPlanoContas = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@tpMovto"))
                            tipo = byte.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@ordem"))
                            ordem = byte.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@ordem"))
                            ordem = byte.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cSegura"))
                            cSegura = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cdTpLiq"))
                            cdTpLiq = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cdLocal"))
                            cdLocal = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pCdTurma"))
                            cdTurma = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pSituacoes"))
                            situacoes = parametrosHash[1] + "";
                        if (parametrosHash[0].Equals("@cdTpFinan"))
                            cdTpFinan = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pMostrarCpfResponsavel"))
                            pMostrarCpfResponsavel = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pNoTurma"))
                            no_turma = parametrosHash[1];
                        if (parametrosHash[0].Equals("@pCancelamento"))
                            ckCancelamento = (parametrosHash[1] != null && parametrosHash[1] != "") ? (bool?)bool.Parse(parametrosHash[1]): null;
                        if (!parametrosHash[0].StartsWith("@")
                            && (((tipo == (int)TipoMovimento.RECEBIDAS || tipo == (int)TipoMovimento.PAGAS)
                               && (!parametrosHash[0].Equals("PMostraDesconto") && !parametrosHash[0].Equals("PMostraContas")))
                               || (tipo != (int)TipoMovimento.RECEBIDAS && tipo != (int)TipoMovimento.PAGAS && !parametrosHash[0].Equals("PMostraCCManual"))))
                            parametrosRelatorio.Add(parametrosHash[0], parametrosHash[1]);
                    }
                    // Adicionando o periodo 
                    if(tipo != (int)TipoMovimento.RECEBIDAS && tipo != (int)TipoMovimento.PAGAS)
                        parametrosRelatorio.Add("PPeriodo", "Período de " + String.Format("{0:dd/MM/yyyy}", pDtaI) + " à " + String.Format("{0:dd/MM/yyyy}", pDtaF) + "        Data base: " + String.Format("{0:dd/MM/yyyy}", pDtaBase));
                    else
                        parametrosRelatorio.Add("PPeriodo", "Período de " + String.Format("{0:dd/MM/yyyy}", pDtaI) + " à " + String.Format("{0:dd/MM/yyyy}", pDtaF));

                    if (parametrosRelatorio != null && parametrosRelatorio["PDataHoraAtual"] != null)
                    {
                        DateTime dtHoraAtualServAplic = DateTime.Parse(parametrosRelatorio["PDataHoraAtual"].ToString());
                        DateTime dtHoraAtualRelat = DateTime.Now;
                        TimeSpan t = dtHoraAtualRelat - dtHoraAtualServAplic;
                        int timeout = int.Parse(ConfigurationManager.AppSettings["timeoutRelatorio"] + "");
                        if (t.TotalSeconds > timeout)
                        {
                            //url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                            url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                            Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoSessaoRelatExp) + "&stackTrace=");
                            renderizarRelatorio = false;
                        }
                    }

                    if (renderizarRelatorio)
                    {

                        parametrosRelatorio.Add("PSituacoes", formataSituacoes(situacoes));


                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();

                        List<Componentes.GenericModel.TO> sourceTO = new List<TO>();
                        List<RptReceberPagar> sourceReceberPagar = new List<RptReceberPagar>();
                        List<RptRecebidaPaga> sourceRecebidaPaga = new List<RptRecebidaPaga>();
                        string no_local = "";
                        //TIPO: RECEBER = 0 - PAGAR = 1 - RECEBIDAS = 2 - PAGAS = 3
                        if (tipo == (int)TipoMovimento.RECEBER || tipo == (int)TipoMovimento.PAGAR)
                            sourceReceberPagar = apresentadorRelatorio.getSourceReceberPagar(cdEscola, pDtaI.Value, pDtaF.Value, pForn, pDtaBase.Value, pNatureza, pPlanoContas, ordem, cSegura, cdTpLiq, cdTpFinan, tipo, situacoes, cdTurma).ToList<RptReceberPagar>();
                        else
                            sourceRecebidaPaga = apresentadorRelatorio.getSourceRecebidaPaga(cdEscola, pDtaI.Value, pDtaF.Value, pForn, pNatureza, pPlanoContas, pMostraCCManual, cSegura, cdTpLiq, cdTpFinan, tipo, situacoes, cdTurma, ckCancelamento, cdLocal).ToList<RptRecebidaPaga>();

                        if (sourceReceberPagar != null && sourceReceberPagar.Count() > 0)
                        {
                            sourceTO = sourceReceberPagar.ToList<Componentes.GenericModel.TO>();
                        }else if (sourceRecebidaPaga != null && sourceRecebidaPaga.Count() > 0)
                        {
                            sourceTO = sourceRecebidaPaga.ToList<Componentes.GenericModel.TO>();
                            no_local = cdLocal == 0 ? "" : "Local Movimento: " + sourceRecebidaPaga[0].no_local;
                        }

                        var nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_TITULO_DATA;
                        string tituloRel = "";
                        switch (ordem)
                        {
                            case (int)Ordenacao.VENCIMENTO:
                                {
                                    if ((tipo == (int)TipoMovimento.RECEBER || tipo == (int)TipoMovimento.RECEBIDAS) && cdTurma > 0)
                                    {
                                        parametrosRelatorio.Add("PNomeTurma", no_turma);
                                    }
                                    else
                                    {
                                        parametrosRelatorio.Add("PNomeTurma", "");
                                    }

                                    if (tipo == (int)TipoMovimento.RECEBER || tipo == (int)TipoMovimento.PAGAR)
                                    {
                                        if (tipo == (int)TipoMovimento.RECEBER)
                                            tituloRel = "Contas a Receber por Data de Vencimento";
                                        else
                                            tituloRel = "Contas a Pagar por Data de Vencimento";

                                        //Adicionando Nome do Relatório
                                        parametrosRelatorio.Add("PNomeRelatorio", "RptTitulosData.rdlc");

                                        nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_TITULO_DATA;
                                        parametrosRelatorio.Add("PLocal", "");
                                    }
                                    else
                                    {
                                        parametrosRelatorio.Add("PLocal", no_local);
                                        if (tipo == (int)TipoMovimento.RECEBIDAS)
                                            tituloRel = "Contas Recebidas por Data de Recebimento";
                                        else
                                            tituloRel = "Contas Pagas por Data de Pagamento";
                                        //Adicionando Nome do Relatório
                                        parametrosRelatorio.Add("PNomeRelatorio", "RptBaixasData.rdlc");
                                        parametrosRelatorio.Add("PMostrarRecibo", pMostraRecibo);
                                        nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_BAIXA_DATA;

                                        if (tipo == (int)TipoMovimento.RECEBIDAS)
                                        {
                                            parametrosRelatorio.Add("pMostrarCpfResponsavel", pMostrarCpfResponsavel);
                                            parametrosRelatorio.Add("ckCancelamento", ckCancelamento);

                                            if (tipo == (int)TipoMovimento.RECEBIDAS && ckCancelamento == true)
                                            {
                                                tituloRel = "Títulos Cancelados";
                                            }
                                        }
                                        else
                                        {
                                            parametrosRelatorio.Add("pMostrarCpfResponsavel", false);
                                            parametrosRelatorio.Add("ckCancelamento", null);
                                        }

                                        if (cdTpFinan == 5)
                                        {
                                            parametrosRelatorio.Add("column_visible", false);
                                        }
                                        else
                                        {
                                            parametrosRelatorio.Add("column_visible", true);
                                        }

                                    }
                                    break;
                                }
                            case (int)Ordenacao.PESSOA:
                                {
                                    if ((tipo == (int)TipoMovimento.RECEBER || tipo == (int)TipoMovimento.RECEBIDAS) && cdTurma > 0)
                                    {
                                        parametrosRelatorio.Add("PNomeTurma", no_turma);
                                    }
                                    else
                                    {
                                        parametrosRelatorio.Add("PNomeTurma", "");
                                    }

                                    if (tipo == (int)TipoMovimento.RECEBER || tipo == (int)TipoMovimento.PAGAR)
                                    {
                                        if (tipo == (int)TipoMovimento.RECEBER)
                                            tituloRel = "Contas a Receber por Cliente";
                                        else
                                            tituloRel = "Contas a Pagar por Fornecedor";
                                        //Adicionando Nome do Relatório
                                        parametrosRelatorio.Add("PNomeRelatorio", "RptTitulosPessoa.rdlc");
                                        nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_TITULO_PESSOA;
                                        parametrosRelatorio.Add("PLocal", "");
                                    }
                                    else
                                    {
                                        parametrosRelatorio.Add("PLocal", no_local);
                                        if (tipo == (int)TipoMovimento.RECEBIDAS)
                                            tituloRel = "Contas Recebidas por Cliente";
                                        else
                                            tituloRel = "Contas Pagas por Fornecedor";
                                        //Adicionando Nome do Relatório
                                        parametrosRelatorio.Add("PNomeRelatorio", "RptBaixasPessoa.rdlc");
                                        parametrosRelatorio.Add("PMostrarRecibo", pMostraRecibo);
                                        nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_BAIXA_PESSOA;

                                        if (tipo == (int)TipoMovimento.RECEBIDAS)
                                        {
                                            parametrosRelatorio.Add("pMostrarCpfResponsavel", pMostrarCpfResponsavel);
                                            parametrosRelatorio.Add("ckCancelamento", ckCancelamento);

                                            if (tipo == (int)TipoMovimento.RECEBIDAS && ckCancelamento == true)
                                            {
                                                tituloRel = "Títulos Cancelados";
                                            }
                                        }
                                        else
                                        {
                                            parametrosRelatorio.Add("pMostrarCpfResponsavel", false);
                                            parametrosRelatorio.Add("ckCancelamento", null);
                                        }

                                        if (cdTpFinan == 5)
                                        {
                                            parametrosRelatorio.Add("column_visible", false);
                                        }
                                        else
                                        {
                                            parametrosRelatorio.Add("column_visible", true);
                                        }
                                    }
                                    break;
                                }
                            case (int)Ordenacao.PLANO_CONTA:
                                {
                                    if ((tipo == (int)TipoMovimento.RECEBER || tipo == (int)TipoMovimento.RECEBIDAS) && cdTurma > 0)
                                    {
                                        parametrosRelatorio.Add("PNomeTurma", no_turma);
                                    }
                                    else
                                    {
                                        parametrosRelatorio.Add("PNomeTurma", "");
                                    }

                                    if (tipo == (int)TipoMovimento.RECEBER || tipo == (int)TipoMovimento.PAGAR)
                                    {
                                        if (tipo == (int)TipoMovimento.RECEBER)
                                            tituloRel = "Contas a Receber por Plano de Contas";
                                        else
                                            tituloRel = "Contas a Pagar por Plano de Contas";
                                        //Adicionando Nome do Relatório
                                        parametrosRelatorio.Add("PNomeRelatorio", "RptTitulosConta.rdlc");
                                        nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_TITULO_CONTA;
                                        parametrosRelatorio.Add("PLocal", "");
                                    }
                                    else
                                    {
                                        parametrosRelatorio.Add("PLocal", no_local);
                                        if (tipo == (int)TipoMovimento.RECEBIDAS)
                                            tituloRel = "Contas Recebidas por Plano de Contas";
                                        else
                                            tituloRel = "Contas Pagas por Plano de Contas";
                                        //Adicionando Nome do Relatório
                                        parametrosRelatorio.Add("PNomeRelatorio", "RptBaixasConta.rdlc");
                                        parametrosRelatorio.Add("PMostrarRecibo", pMostraRecibo);
                                        nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_BAIXA_CONTA;

                                        if (tipo == (int)TipoMovimento.RECEBIDAS)
                                        {
                                            parametrosRelatorio.Add("pMostrarCpfResponsavel", pMostrarCpfResponsavel);
                                            parametrosRelatorio.Add("ckCancelamento", ckCancelamento);

                                            if (tipo == (int)TipoMovimento.RECEBIDAS && ckCancelamento == true)
                                            {
                                                tituloRel = "Títulos Cancelados";
                                            }
                                        }
                                        else
                                        {
                                            parametrosRelatorio.Add("pMostrarCpfResponsavel", false);
                                            parametrosRelatorio.Add("ckCancelamento", null);

                                        }

                                        if (cdTpFinan == 5)
                                        {
                                            parametrosRelatorio.Add("column_visible", false);
                                        }
                                        else
                                        {
                                            parametrosRelatorio.Add("column_visible", true);
                                        }
                                    }
                                    break;
                                }
                        }
                        // Inclui os parâmetros especializados do relatório:   pMostraRecibo
                        parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, tituloRel);

                        if(sourceTO != null && sourceTO.Count > 0) {
                            relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", nomeRelatorio, sourceTO, parametrosRelatorio);
                            if (tipo == (int) TipoMovimento.RECEBER || tipo == (int) TipoMovimento.PAGAR)
                            {
                                relatorio.LocalReport.SubreportProcessing += new Microsoft.Reporting.WebForms.SubreportProcessingEventHandler(renderizaSubRelatorioPlanos);
                            }
                            else if(tipo == (int) TipoMovimento.RECEBIDAS && ckCancelamento == true)
                            {
                                relatorio.LocalReport.SubreportProcessing += new Microsoft.Reporting.WebForms.SubreportProcessingEventHandler(renderizaSubRptBaixaDataObs);
                            }
                            //LBM Coloquei o campo de Observação diretamente. Sé é usado no relatório de Contas Recebidas
                            //else
                            //{
                            //    relatorio.LocalReport.SubreportProcessing += new Microsoft.Reporting.WebForms.SubreportProcessingEventHandler(renderizaSubRptBaixaDataObsCC);
                            //}

                        }
                        else {
                            //url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                            url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                            Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(Componentes.Utils.Messages.Messages.msgRegNotEnc) + "&stackTrace=");
                        }
                    }
                }
                else
                    throw new Exception(Componentes.Utils.Messages.Messages.msgSessaoExpirada);
            }

            private string formataSituacoes(string situacoes)
            {
                string retorno = "";
                string[] situacao = situacoes.Split('|');
                List<int> cdsSituacoesList = new List<int>();
                for (int i = 0; i < situacao.Count(); i++)
                {
                    switch (Int32.Parse(situacao[i]))
                    {
                        case (int)Situacao.AGUARDANDO_MATRÍCULA:
                            if (retorno == "") retorno = "Aguardando Matricula";else retorno = retorno + "," + "Aguardando Matricula";
                            break;
                        case (int)Situacao.DESISTENTE:
                            if (retorno == "") retorno = "Desistente";else retorno = retorno + "," + "Desistente";
                            break;
                        case (int)Situacao.ENCERRADO:
                            if (retorno == "") retorno = "Encerrado";else retorno = retorno + "," + "Encerrado";
                            break;
                        case (int)Situacao.MATRICULADO:
                            if (retorno == "") retorno = "Matriculado";else retorno = retorno + "," + "Matriculado";
                            break;
                        case (int)Situacao.MOVIDO:
                            if (retorno == "") retorno = "Movido";else retorno = retorno + "," + "Movido";
                            break;
                        case (int)Situacao.REMATRICULADO:
                            if (retorno == "") retorno = "Rematriculado";else retorno = retorno + "," + "Rematriculado";
                            break;
                    }

                }

                return retorno;
            }

            private void renderizaSubRelatorioPlanos(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e) {
                try
                {
                    int cd_titulo = Convert.ToInt32(e.Parameters["cd_titulo"].Values[0]);
                    FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();
                    e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetSubReport",
                        apresentadorRelatorio.getPlanosContaPosicaoFinanceira(cd_titulo)));
                }
                catch (Exception exe)
                {
                    logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
                }
            }

            public static class NullableInt
            {
                public static bool TryParse(string text, out int? outValue)
                {
                    int parsedValue;
                    bool success = int.TryParse(text, out parsedValue);
                    outValue = success ? (int?)parsedValue : null;
                    return success;
                }
            }

            private void renderizaSubRptBaixaDataObsCC(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e) {
                try{
                    if (e.Parameters["cd_baixa_titulo"] != null && e.Parameters["cd_conta_corrente"] != null)
                    {
                        int? cd_baixa_titulo = null;
                        int? cd_conta_corrente = null;
                        NullableInt.TryParse(e.Parameters["cd_baixa_titulo"].Values[0] + "", out cd_baixa_titulo);
                        NullableInt.TryParse(e.Parameters["cd_conta_corrente"].Values[0] + "", out cd_conta_corrente);
                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();

                        e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetSubReport",
                            apresentadorRelatorio.getObservacoesCCBaixa(cd_baixa_titulo, cd_conta_corrente)));
                    }

                }
                catch (Exception exe)
                {
                    logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
                }
            }

            private void renderizaSubRptBaixaDataObs(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e)
            {
                try
                {
                    //if (e.Parameters["obs_baixa"] != null && e.Parameters["no_pessoa_obs_baixa"] != null &&
                    //    e.Parameters["dt_vencimento_obs_baixa"] != null && e.Parameters["numero_titulo_obs_baixa"] != null &&
                    //    e.Parameters["numero_parcela_titulo_obs_baixa"] != null)
                    //{
                    //    string tx_obs_baixa = e.Parameters["obs_baixa"].Values[0] + "";
                    //    string tx_no_pessoa_obs_baixa = e.Parameters["no_pessoa_obs_baixa"].Values[0] + "";
                    //    string dt_vencimento_obs_baixa = e.Parameters["dt_vencimento_obs_baixa"].Values[0] + "";
                    //    string numero_titulo_obs_baixa = e.Parameters["numero_titulo_obs_baixa"].Values[0] + "";
                    //    string numero_parcela_titulo_obs_baixa = e.Parameters["numero_parcela_titulo_obs_baixa"].Values[0] + "";
                    //    List<ObservacaoBaixaUI> listaObservacao = new List<ObservacaoBaixaUI>();

                    //    ObservacaoBaixaUI observacao = new ObservacaoBaixaUI();
                    //    observacao.tx_obs_baixa = tx_obs_baixa;
                    //    observacao.tx_no_pessoa_obs_baixa = tx_no_pessoa_obs_baixa;
                    //    observacao.dt_vencimento_obs_baixa = dt_vencimento_obs_baixa;
                    //    observacao.numero_titulo_obs_baixa = numero_titulo_obs_baixa;
                    //    observacao.numero_parcela_titulo_obs_baixa = numero_parcela_titulo_obs_baixa;

                    //    listaObservacao.Add(observacao);
                    if (e.Parameters["cd_baixa_titulo"] != null && e.Parameters["cd_conta_corrente"] == null)
                    {
                        int? cd_baixa_titulo = 0;
                        NullableInt.TryParse(e.Parameters["cd_baixa_titulo"].Values[0] + "", out cd_baixa_titulo);
                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();

                        e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetSubReport",
                            apresentadorRelatorio.getObservacoesBaixaCancelamento(cdEscola, (int) cd_baixa_titulo)));
                    }
                    // }

                }
                catch (Exception exe)
                {
                    logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
                }
            }

            protected void relatorio_DocumentMapNavigation(object sender, Microsoft.Reporting.WebForms.DocumentMapNavigationEventArgs e) {
                //bindRelatorio();
            }

            protected void relatorio_Search(object sender, Microsoft.Reporting.WebForms.SearchEventArgs e) {
                //bindRelatorio();
            }

            protected void relatorio_Drillthrough(object sender, Microsoft.Reporting.WebForms.DrillthroughEventArgs e) {
                //bindRelatorio();
            }

            protected void relatorio_Load(object sender, EventArgs e) {
                /*if(!IsPostBack) 
                    bindRelatorio();*/
            }

            protected void relatorio_BookmarkNavigation(object sender, Microsoft.Reporting.WebForms.BookmarkNavigationEventArgs e) {
                //bindRelatorio();
            }

            protected void relatorio_PageNavigation(object sender, Microsoft.Reporting.WebForms.PageNavigationEventArgs e) {
                //bindRelatorio();
            }

            protected override void OnInit(EventArgs e)
            {
                base.OnInit(e);

                StringBuilder sb = new StringBuilder();
                sb.Append("setInterval(KeepSessionAlive, " + getSessionTimeoutInMs() + ");");
                sb.Append("function KeepSessionAlive() {");
                //sb.Append(string.Format("$.post('{0}', null);", ResolveUrl("~/KeepSessionAlive.ashx")));

                //sb.Append("var xhr = new XMLHttpRequest();");
                //sb.Append(string.Format("xhr.open('GET', '{0}');", ResolveUrl("~/Relatorio/renovarSessao")));
                //sb.Append("xhr.onload = function() {");
                //sb.Append("};");
                //sb.Append("xhr.send();");
                sb.Append("window.location = window.location;");
                sb.Append("};");

                sb.Append("};");

                // register on page
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SessionKeepAlive", sb.ToString(), true);
            }

            private int getSessionTimeoutInMs()
            {
                return (this.Session.Timeout * 60000) / 2; //(this.Session.Timeout * 60000) - 10000
            }
        </script>
        <script>
                 (function (i, s, o, g, r, a, m) {
                     i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                         (i[r].q = i[r].q || []).push(arguments)
                     }, i[r].l = 1 * new Date(); a = s.createElement(o),
                     m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
                 })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

                 ga('create', 'UA-97448037-1', 'auto');
                 ga('send', 'pageview');
        </script>
        <script>
            var _prum = [['id', '570d92daabe53d3930288c12'],
                         ['mark', 'firstbyte', (new Date()).getTime()]];
            (function () {
                var s = document.getElementsByTagName('script')[0]
                  , p = document.createElement('script');
                p.async = 'async';
                p.src = '//rum-static.pingdom.net/prum.min.js';
                s.parentNode.insertBefore(p, s);
            })();
        </script>
        <form id="Form1" runat="server" style="width:100%; height:100%;">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <cc1:VisualizadorRelatorio id="relatorio" runat="server" height="100%" width="100%" Font-Names="Verdana" DocumentMapCollapsed="false"  ShowFindControls="true"
                ShowDocumentMapButton="true" ShowToolBar="true"  ShowPrintButton="true" ProcessingMode="Local" Font-Size="8pt" AsyncRendering="false" SizeToReportContent="True" 
                OnDocumentMapNavigation="relatorio_DocumentMapNavigation" OnSearch="relatorio_Search" OnDrillthrough="relatorio_Drillthrough" OnLoad="relatorio_Load"
                OnBookmarkNavigation="relatorio_BookmarkNavigation" OnPageNavigation="relatorio_PageNavigation" KeepSessionAlive="false"></cc1:VisualizadorRelatorio>
        </form>
    </div>
</body>
</html>