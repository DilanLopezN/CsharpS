<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Register Assembly="Componentes.ApresentadorRelatorio" Namespace="Componentes.ApresentadorRelatorio" TagPrefix="cc1" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Relatório</title>
</head>
<body>
    <div>
     <script runat="server">
     
         private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioTurma.aspx");

         public IEnumerable<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> ItensTurmasPPT;
         public IEnumerable<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> ItensTurmasRegulares;
         public IEnumerable<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> ItensTurmasNovasRegulares;
         public IEnumerable<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> ItensTurmasNovasPPT;
         DateTime dataFinal, dataFinalTurmasNovas;
         DateTime dataInicial, dataInicialTurmasNovas;
         public int cdEscolaAluno;//Parametro Global para o subreport
            public void Page_Load(object sender, System.EventArgs e)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                bool abortou_requisicao = false;
                try
                {
                    if (!IsPostBack)
                        bindRelatorio(url, ref abortou_requisicao);
                }
                catch (Exception exe)
                {
                    RedirecionaAplicacaoPrincipal(abortou_requisicao, url, exe);
                }
            }

            private void RedirecionaAplicacaoPrincipal(bool abortou_requisicao, string url, Exception exe)
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
                    logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
                    if (!abortou_requisicao)
                        Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroRenderizarRelatorio) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
                }
                catch (Exception e)
                {
                    logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError + " url: " + url, e);
                }
            }

            private void Page_Init(object sender, System.EventArgs e)
            {
                Context.Handler = this.Page;
            }

            private void bindRelatorio(string url, ref bool abortou_requisicao) {
                
                Hashtable parametrosPesquisa = new Hashtable();
                Hashtable parametrosRelatorio = new Hashtable();
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                DateTime dataBase = new DateTime();
                bool renderizarRelatorio = true;
                int cdEscola = 0;
                int tipoTurma = 0;
                int cdCurso = 0;
                int cdDuracao = 0;
                int cdProduto = 0;
                int cdProfessor = 0;
                int prog = 0;
                int situacaoTurma = 0;
                int tipoOnline = 0;
                string situacaoAlunoTurma = "";
                bool turmasFilhas = false;
                bool mostrarTelefone = false;
                bool mostrarResp = false;
                DateTime? pDtaI = null;
                DateTime? pDtaF = null;
                DateTime? pDtaFimI = null;
                DateTime? pDtaFimF = null;
                string descTipTurma = "";
                int? tipoRelatorio = null;
                string dias = "0000000";
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');
                        if (parametrosHash[0].Equals("@cd_escola"))
                            cdEscola = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@DataBase"))
                            dataBase = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@tipoTurma"))
                            tipoTurma = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cdProfessor"))
                            cdProfessor = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cdCurso"))
                            cdCurso = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cdDuracao"))
                            cdDuracao = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cdProduto"))
                            cdProduto = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@prog"))
                            prog = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@situacaoTurma"))
                            situacaoTurma = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@situacaoAlunoTurma"))
                            situacaoAlunoTurma = parametrosHash[1] + "";
                        if (parametrosHash[0].Equals("@tipoOnline"))
                            tipoOnline = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@turmasFilhas"))
                            turmasFilhas = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@mostrarResp"))
                            mostrarResp = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@mostrarTelefone"))
                            mostrarTelefone = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@tipoRelatorio") &&
                            parametrosHash[1] != null && parametrosHash[1] != "")
                            tipoRelatorio = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@dtInicial") && parametrosHash[1] != "")
                            pDtaI = DateTime.Parse(parametrosHash[1] );
                        if (parametrosHash[0].Equals("@dtFinal") && parametrosHash[1] != "")
                            pDtaF = DateTime.Parse(parametrosHash[1] );
                        if (parametrosHash[0].Equals("@dtInicialFim") && parametrosHash[1] != "")
                            pDtaFimI = DateTime.Parse(parametrosHash[1] );
                        if (parametrosHash[0].Equals("@dtFinalFim") && parametrosHash[1] != "")
                            pDtaFimF = DateTime.Parse(parametrosHash[1] );
                        if (parametrosHash[0].Equals("@dias"))
                            dias = parametrosHash[1];
                        if (!parametrosHash[0].StartsWith("@"))
                            parametrosRelatorio.Add(parametrosHash[0], parametrosHash[1]);
                    }
                }
                // Adicionando o periodo 
              
                    if (parametrosRelatorio != null && parametrosRelatorio["PDataHoraAtual"] != null)
                    {
                        this.cdEscolaAluno = cdEscola;
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
                    }//if

                    if (renderizarRelatorio)
                    {
                        if (tipoRelatorio != null)
                        {
                            
                            parametrosRelatorio.Add("PSintetico", tipoRelatorio == 0 ? false : true);
                        }
                        else
                        {
                            parametrosRelatorio.Add("PSintetico", tipoRelatorio);
                        }
                        

                        var caminhoRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_LISTAGEM_TURMAS;
                        if ((tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL) || (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.TODAS))
                        {
                            descTipTurma = "em Andamento";
                            if (situacaoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.SituacaoTurma.TURMASENCERRADAS)
                                descTipTurma = "Encerradas";
                            if (situacaoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.SituacaoTurma.TURMASFORMACAO)
                                descTipTurma = "em Formação";
                        }
                        else
                        {
                            descTipTurma = "Ativas";
                            if (situacaoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.SituacaoTurmaPPT.TURMASINATIVAS)
                                descTipTurma = "Inativas";
                        }                       
                        
                        IEnumerable<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> turmasAlunos = new List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch>();
                        
                        // SITUAÇÃO TURMAS A ENCERRAR
                        IEnumerable<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> turmasRegulares = new List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch>();
                        IEnumerable<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> turmasPPTs = new List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch>();
                        
                        //SITUAÇÃO TURMAS NOVAS
                        IEnumerable<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> turmasNovasRegulares = new List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch>();
                        IEnumerable<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> turmasNovasPPTs = new List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch>();
                        
                        Hashtable sourceHash = new Hashtable();
                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();

                        if (situacaoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.SituacaoTurma.TURMASAENCERRAR)
                        {
                            descTipTurma = "a Encerrar";
                            caminhoRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_TURMAS_ENCERRAR;
                            
                            if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.TODAS)
                            {                                
                                turmasRegulares = apresentadorRelatorio.getRptTurmasAEncerrar((int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL, cdCurso, cdDuracao, cdProduto, cdProfessor, prog, turmasFilhas, pDtaI, pDtaF, pDtaFimI, pDtaFimF, cdEscola, tipoOnline, dias).ToList();
                                montarNomeProfessor(turmasRegulares);
                                
                                turmasPPTs = apresentadorRelatorio.getRptTurmasAEncerrar((int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.PPT, cdCurso, cdDuracao, cdProduto, cdProfessor, prog, turmasFilhas, pDtaI, pDtaF, pDtaFimI, pDtaFimF, cdEscola, tipoOnline, dias).ToList();
                                montarNomeProfessor(turmasPPTs);
                                //turmasPPTs = turmasPPTs.Union(turmasRegulares);
                            }
                            else
                            {
                                if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.PPT)
                                {
                                    turmasFilhas = false;
                                    prog = 0;
                                    turmasPPTs = apresentadorRelatorio.getRptTurmasAEncerrar((int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.PPT, cdCurso, cdDuracao, cdProduto, cdProfessor, prog, turmasFilhas, pDtaI, pDtaF, pDtaFimI, pDtaFimF, cdEscola, tipoOnline, dias).ToList();
                                    montarNomeProfessor(turmasPPTs);                                    
                                }

                                if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL)
                                {
                                    turmasRegulares = apresentadorRelatorio.getRptTurmasAEncerrar((int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL, cdCurso, cdDuracao, cdProduto, cdProfessor, prog, turmasFilhas, pDtaI, pDtaF, pDtaFimI, pDtaFimF, cdEscola, tipoOnline, dias).ToList();
                                    montarNomeProfessor(turmasRegulares);
                                }
                            }

                           
                        } else
                            if (situacaoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.SituacaoTurma.TURMASNOVA)
                            {
                                descTipTurma = "Novas";
                                caminhoRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_TURMAS_NOVAS;
                                
                                if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.TODAS)
                                {
                                    turmasNovasRegulares = apresentadorRelatorio.getRptTurmasNovas((int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL, cdCurso, cdDuracao, cdProduto, cdProfessor, prog, turmasFilhas, pDtaI, pDtaF, cdEscola, tipoOnline, dias).ToList();
                                    montarNomeProfessor(turmasNovasRegulares);

                                    turmasNovasPPTs = apresentadorRelatorio.getRptTurmasNovas((int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.PPT, cdCurso, cdDuracao, cdProduto, cdProfessor, prog, turmasFilhas, pDtaI, pDtaF, cdEscola, tipoOnline, dias).ToList();
                                    montarNomeProfessor(turmasNovasPPTs);
                                }
                                else
                                {
                                    if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.PPT)
                                    {
                                        turmasNovasPPTs = apresentadorRelatorio.getRptTurmasNovas((int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.PPT, cdCurso, cdDuracao, cdProduto, cdProfessor, prog, turmasFilhas, pDtaI, pDtaF, cdEscola, tipoOnline, dias).ToList();
                                        montarNomeProfessor(turmasNovasPPTs);
                                    }

                                    if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL)
                                    {
                                        turmasNovasRegulares = apresentadorRelatorio.getRptTurmasNovas((int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL, cdCurso, cdDuracao, cdProduto, cdProfessor, prog, turmasFilhas, pDtaI, pDtaF, cdEscola, tipoOnline, dias).ToList();
                                        montarNomeProfessor(turmasNovasRegulares);
                                    }
                                }
                            }
                            else
                            {
                                turmasAlunos = apresentadorRelatorio.getRptTurmas(tipoTurma, cdCurso, cdDuracao, cdProduto, cdProfessor, prog, turmasFilhas, pDtaI, pDtaF, pDtaFimI, pDtaFimF, cdEscola, situacaoTurma, situacaoAlunoTurma, tipoOnline, dias).ToList();
                                montarNomeProfessor(turmasAlunos);
                            }
                        
                        string no_produto = "Todos";
                        string no_tipo_turma = "Todas";
                        string situacaoAluno = "";

                        if (situacaoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.SituacaoTurma.TURMASAENCERRAR ||
                             situacaoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.SituacaoTurma.TURMASNOVA)
                        {                            
                            if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.TODAS)
                            {
                                parametrosRelatorio.Add("PExibeTodos", true);
                            }
                            else
                            {
                                if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL)
                                {
                                    no_tipo_turma = "Regular";
                                    parametrosRelatorio.Add("PExibePPT", false);
                                }
                                if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.PPT)
                                {
                                    no_tipo_turma = "Personalizada";
                                    parametrosRelatorio.Add("PExibePPT", true);
                                }
                            }
                            parametrosRelatorio.Add("PTipoTurma", no_tipo_turma);                            
                        }
                        
                        if (turmasAlunos.Count() > 0 || turmasRegulares.Count() > 0 || turmasPPTs.Count() > 0
                            || turmasNovasRegulares.Count() > 0 || turmasNovasPPTs.Count() > 0)
                        {
                            if (cdProduto > 0 && turmasAlunos.Count() > 0)
                                no_produto = turmasAlunos.FirstOrDefault().no_produto;                            

                            if (cdProduto > 0 && turmasRegulares.Count() > 0)
                                no_produto = turmasRegulares.FirstOrDefault().no_produto;

                            if (cdProduto > 0 && turmasPPTs.Count() > 0)
                                no_produto = turmasPPTs.FirstOrDefault().no_produto;
                            
                            if (cdProduto > 0 && turmasNovasRegulares.Count() > 0)
                                no_produto = turmasNovasRegulares.FirstOrDefault().no_produto;

                            if (cdProduto > 0 && turmasNovasPPTs.Count() > 0)
                                no_produto = turmasNovasPPTs.FirstOrDefault().no_produto;
                            
                            List<int> situacao = situacaoAlunoTurma.Split('|').Select(Int32.Parse).ToList();
                            for(int i = 0; i < situacao.Count; i++)
                            {
                                if(!(situacao[i] == 100))
                                {
                                    if(!situacaoAluno.Contains("Ativo,"))
                                    {
                                        if(situacao.Contains(1) && situacao.Contains(8))
                                            situacaoAluno += " Ativo,";
                                        else
                                        {
                                            if(situacao[i] == 1)
                                                situacaoAluno += " Matriculado,";
                                            if(situacao[i] == 8)
                                                situacaoAluno += " Rematriculado,";
                                        }
                                    }

                                    if(situacao[i] == 0)
                                        situacaoAluno += " Movido,";
                                    if(situacao[i] == 2)
                                        situacaoAluno += " Desistente,";
                                    if(situacao[i] == 4)
                                        situacaoAluno += " Encerrado,";
                                    if(situacao[i] == 9)
                                        situacaoAluno += " Aguardando Matrícula,";                                    
                                } 
                            }
                            situacaoAluno = situacaoAluno != "" ? situacaoAluno.Remove(situacaoAluno.Count() - 1) : "";
                        }
                        
                        sourceHash["DataSetReportTurma"] = turmasAlunos;
                        sourceHash["DataSetReportTurmaRegular"] = turmasRegulares;
                        sourceHash["DataSetReportTurmaPersp"] = turmasPPTs;

                        sourceHash["DataSetReportTurmaNovaRegular"] = turmasNovasRegulares;
                        sourceHash["DataSetReportTurmaNovaPersp"] = turmasNovasPPTs;
                        
                        parametrosRelatorio.Add("PProduto", no_produto);
                        parametrosRelatorio.Add("PMostrarResponsavel", mostrarResp);
                        parametrosRelatorio.Add("PMostrarTelefone", mostrarTelefone);
                        parametrosRelatorio.Add("PSituacaoAlunoTurma", situacaoAlunoTurma);
                        parametrosRelatorio.Add("PNoSituacaoAluno", situacaoAluno);                        
                        
                        //sourceHash["DataSetSubReportSubTurma"] = alunos; 
                        if ((pDtaI != null && pDtaI != DateTime.MinValue) || (pDtaF != null && pDtaF != DateTime.MinValue))
                            parametrosRelatorio.Add("PPeriodo", "Período de " + String.Format("{0:dd/MM/yyyy}", pDtaI) + " à " + String.Format("{0:dd/MM/yyyy}", pDtaF));

                        //Filtro Turmas a Encerrar
                        if (situacaoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.SituacaoTurma.TURMASAENCERRAR && 
                            ((pDtaFimI != null && pDtaFimI != DateTime.MinValue) || (pDtaFimF != null && pDtaFimF != DateTime.MinValue)))
                            parametrosRelatorio.Add("PPeriodo", "Período de " + String.Format("{0:dd/MM/yyyy}", pDtaFimI) + " à " + String.Format("{0:dd/MM/yyyy}", pDtaFimF));
                        
                        parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relatório de Turmas - " + descTipTurma);
                        parametrosRelatorio.Add("DataBase", dataBase);
                        if (turmasAlunos.Count() > 0 || turmasRegulares.Count() > 0 || turmasPPTs.Count() > 0
                            || turmasNovasRegulares.Count() > 0 || turmasNovasPPTs.Count() > 0)
                        {
                            IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI> resultadoConsultaTurmasRegulares = null;
                            IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI> resultadoConsultaTurmasPPTs = null;
                            
                            if (situacaoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.SituacaoTurma.TURMASAENCERRAR)
                            {
                                if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.TODAS)
                                {
                                    resultadoConsultaTurmasRegulares = apresentadorRelatorio.getRptAlunosTurmaEncerrar(turmasRegulares.Select(t => t.cd_turma).ToList(), pDtaFimI, pDtaFimF, cdEscola, (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL).ToList();
                                    resultadoConsultaTurmasPPTs = apresentadorRelatorio.getRptAlunosTurmaEncerrar(turmasPPTs.Select(t => t.cd_turma).ToList(), pDtaFimI, pDtaFimF, cdEscola, (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.PPT).ToList();

                                    sourceHash["DataSetReportTurmaAlunosRegular"] = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI>();
                                    parametrosRelatorio.Add("PDevedoresTurmaReg", false);
                                    
                                    if (resultadoConsultaTurmasRegulares != null && resultadoConsultaTurmasRegulares.Count() > 0)
                                    {
                                        sourceHash["DataSetReportTurmaAlunosRegular"] = resultadoConsultaTurmasRegulares;
                                        parametrosRelatorio["PDevedoresTurmaReg"] = true;
                                    }

                                    sourceHash["DataSetReportTurmaAlunosPersp"] = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI>();
                                    parametrosRelatorio.Add("PDevedoresTurmaPersp", false);

                                    if (resultadoConsultaTurmasPPTs != null && resultadoConsultaTurmasPPTs.Count() > 0)
                                    {
                                        sourceHash["DataSetReportTurmaAlunosPersp"] = resultadoConsultaTurmasPPTs;
                                        parametrosRelatorio["PDevedoresTurmaPersp"] = true;
                                    }

                                    parametrosRelatorio.Add("PExibeRegularRegistros", false);
                                    if (turmasRegulares.Count() > 0)
                                    {
                                        parametrosRelatorio["PExibeRegularRegistros"] = true;
                                    }

                                    parametrosRelatorio.Add("PExibePPTRegistros", false);
                                    if (turmasPPTs.Count() > 0)
                                    {
                                        parametrosRelatorio["PExibePPTRegistros"] = true;
                                    }
                                }
                                else
                                {
                                    if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL)
                                    {
                                        resultadoConsultaTurmasRegulares = apresentadorRelatorio.getRptAlunosTurmaEncerrar(turmasRegulares.Select(t => t.cd_turma).ToList(), pDtaFimI, pDtaFimF, cdEscola, (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL).ToList();
                                        sourceHash["DataSetReportTurmaAlunosRegular"] = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI>();
                                        parametrosRelatorio.Add("PDevedoresTurmaReg", false);

                                        if (resultadoConsultaTurmasRegulares != null && resultadoConsultaTurmasRegulares.Count() > 0)
                                        {
                                            sourceHash["DataSetReportTurmaAlunosRegular"] = resultadoConsultaTurmasRegulares;
                                            parametrosRelatorio["PDevedoresTurmaReg"] = true;
                                        }

                                        sourceHash["DataSetReportTurmaAlunosPersp"] = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI>();
                                        parametrosRelatorio.Add("PDevedoresTurmaPersp", false);

                                        if (resultadoConsultaTurmasPPTs != null && resultadoConsultaTurmasPPTs.Count() > 0)
                                        {
                                            sourceHash["DataSetReportTurmaAlunosPersp"] = resultadoConsultaTurmasPPTs;
                                            parametrosRelatorio["PDevedoresTurmaPersp"] = true;
                                        }

                                        parametrosRelatorio.Add("PExibeRegularRegistros", false);
                                        if (turmasRegulares.Count() > 0)
                                        {
                                            parametrosRelatorio["PExibeRegularRegistros"] = true;
                                        }

                                        parametrosRelatorio.Add("PExibePPTRegistros", false);
                                        if (turmasPPTs.Count() > 0)
                                        {
                                            parametrosRelatorio["PExibePPTRegistros"] = true;
                                        }
                                        
                                    }
                                    if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.PPT)
                                    {
                                        resultadoConsultaTurmasPPTs = apresentadorRelatorio.getRptAlunosTurmaEncerrar(turmasPPTs.Select(t => t.cd_turma).ToList(), pDtaFimI, pDtaFimF, cdEscola, (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.PPT).ToList();
                                        sourceHash["DataSetReportTurmaAlunosPersp"] = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI>();
                                        parametrosRelatorio.Add("PDevedoresTurmaPersp", false);

                                        if (resultadoConsultaTurmasPPTs != null || resultadoConsultaTurmasPPTs.Count() > 0)
                                        {
                                            sourceHash["DataSetReportTurmaAlunosPersp"] = resultadoConsultaTurmasPPTs;
                                            parametrosRelatorio["PDevedoresTurmaPersp"] = true;
                                        }

                                        sourceHash["DataSetReportTurmaAlunosRegular"] = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI>();
                                        parametrosRelatorio.Add("PDevedoresTurmaReg", false);

                                        if (resultadoConsultaTurmasRegulares != null && resultadoConsultaTurmasRegulares.Count() > 0)
                                        {
                                            sourceHash["DataSetReportTurmaAlunosRegular"] = resultadoConsultaTurmasRegulares;
                                            parametrosRelatorio["PDevedoresTurmaReg"] = true;
                                        }

                                        parametrosRelatorio.Add("PExibeRegularRegistros", false);
                                        if (turmasRegulares.Count() > 0)
                                        {
                                            parametrosRelatorio["PExibeRegularRegistros"] = true;
                                        }

                                        parametrosRelatorio.Add("PExibePPTRegistros", false);
                                        if (turmasPPTs.Count() > 0)
                                        {
                                            parametrosRelatorio["PExibePPTRegistros"] = true;
                                        }
                                    }
                                }

                                
                            } else
                                if (situacaoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.SituacaoTurma.TURMASNOVA)
                                {
                                    if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.TODAS)
                                    {
                                        resultadoConsultaTurmasRegulares = apresentadorRelatorio.getRptAlunosTurmasNovas(turmasNovasRegulares.Select(t => t.cd_turma).ToList(), pDtaI, pDtaF, cdEscola, (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL).ToList();
                                        resultadoConsultaTurmasPPTs = apresentadorRelatorio.getRptAlunosTurmasNovas(turmasNovasPPTs.Select(t => t.cd_turma).ToList(),  pDtaI, pDtaF, cdEscola, (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.PPT).ToList();
                                        
                                        sourceHash["DataSetReportTurmaNovaAlunosRegular"] = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI>();
                                        parametrosRelatorio.Add("PBolsistasTurmaReg", false);
                                        
                                        if (resultadoConsultaTurmasRegulares != null && resultadoConsultaTurmasRegulares.Count() > 0)
                                        {
                                            sourceHash["DataSetReportTurmaNovaAlunosRegular"] = resultadoConsultaTurmasRegulares;
                                            //parametrosRelatorio.Add("PBolsistasTurmaReg", true);
                                            parametrosRelatorio["PBolsistasTurmaReg"] = true;
                                        }

                                        sourceHash["DataSetReportTurmaNovaAlunosPersp"] = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI>();
                                        parametrosRelatorio.Add("PBolsistasTurmaPersp", false);
                                        
                                        if (resultadoConsultaTurmasPPTs != null && resultadoConsultaTurmasPPTs.Count() > 0)
                                        {
                                            sourceHash["DataSetReportTurmaNovaAlunosPersp"] = resultadoConsultaTurmasPPTs;
                                            //parametrosRelatorio.Add("PBolsistasTurmaPersp", true);
                                            parametrosRelatorio["PBolsistasTurmaPersp"] = true;
                                        }

                                        parametrosRelatorio.Add("PExibeRegularRegistros", false);
                                        if (turmasNovasRegulares.Count() > 0)
                                        {
                                            parametrosRelatorio["PExibeRegularRegistros"] = true;
                                        }

                                        parametrosRelatorio.Add("PExibePPTRegistros", false);
                                        if (turmasNovasPPTs.Count() > 0)
                                        {
                                            parametrosRelatorio["PExibePPTRegistros"] = true;
                                        }
                                        
                                        
                                           
                                    }
                                    else
                                    {
                                        if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL)
                                        {
                                            resultadoConsultaTurmasRegulares = apresentadorRelatorio.getRptAlunosTurmasNovas(turmasNovasRegulares.Select(t => t.cd_turma).ToList(), pDtaI, pDtaF, cdEscola, (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.NORMAL).ToList();
                                            sourceHash["DataSetReportTurmaNovaAlunosRegular"] = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI>();
                                            parametrosRelatorio.Add("PBolsistasTurmaReg", false);

                                            if (resultadoConsultaTurmasRegulares != null && resultadoConsultaTurmasRegulares.Count() > 0)
                                            {
                                                sourceHash["DataSetReportTurmaNovaAlunosRegular"] = resultadoConsultaTurmasRegulares;
                                                //parametrosRelatorio.Add("PBolsistasTurmaReg", true);
                                                parametrosRelatorio["PBolsistasTurmaReg"] = true;
                                            }

                                            sourceHash["DataSetReportTurmaNovaAlunosPersp"] = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI>();
                                            parametrosRelatorio.Add("PBolsistasTurmaPersp", false);

                                            if (resultadoConsultaTurmasPPTs != null && resultadoConsultaTurmasPPTs.Count() > 0)
                                            {
                                                sourceHash["DataSetReportTurmaNovaAlunosPersp"] = resultadoConsultaTurmasPPTs;
                                                //parametrosRelatorio.Add("PBolsistasTurmaPersp", true);
                                                parametrosRelatorio["PBolsistasTurmaPersp"] = true;
                                            }

                                           

                                            parametrosRelatorio.Add("PExibeRegularRegistros", false);
                                            if (turmasNovasRegulares.Count() > 0)
                                            {
                                                parametrosRelatorio["PExibeRegularRegistros"] = true;
                                            }

                                            parametrosRelatorio.Add("PExibePPTRegistros", false);
                                            if (turmasNovasPPTs.Count() > 0)
                                            {
                                                parametrosRelatorio["PExibePPTRegistros"] = true;
                                            }
                                            
                                            
                                        }
                                        if (tipoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.PPT)
                                        {
                                            resultadoConsultaTurmasPPTs = apresentadorRelatorio.getRptAlunosTurmasNovas(turmasNovasPPTs.Select(t => t.cd_turma).ToList(), pDtaI, pDtaF, cdEscola, (int)FundacaoFisk.SGF.GenericModel.Turma.TipoTurma.PPT).ToList();
                                            sourceHash["DataSetReportTurmaNovaAlunosPersp"] = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI>();
                                            parametrosRelatorio.Add("PBolsistasTurmaPersp", false);

                                            if (resultadoConsultaTurmasPPTs != null || resultadoConsultaTurmasPPTs.Count() > 0)
                                            {
                                                sourceHash["DataSetReportTurmaNovaAlunosPersp"] = resultadoConsultaTurmasPPTs;
                                                //parametrosRelatorio.Add("PBolsistasTurmaPersp", true);
                                                parametrosRelatorio["PBolsistasTurmaPersp"] = true;
                                            }

                                            sourceHash["DataSetReportTurmaNovaAlunosRegular"] = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI>();
                                            parametrosRelatorio.Add("PBolsistasTurmaReg", false);

                                            if (resultadoConsultaTurmasRegulares != null && resultadoConsultaTurmasRegulares.Count() > 0)
                                            {
                                                sourceHash["DataSetReportTurmaNovaAlunosRegular"] = resultadoConsultaTurmasRegulares;
                                                //parametrosRelatorio.Add("PBolsistasTurmaReg", true);
                                                parametrosRelatorio["PBolsistasTurmaReg"] = true;
                                            }

                                            parametrosRelatorio.Add("PExibeRegularRegistros", false);
                                            if (turmasNovasRegulares.Count() > 0)
                                            {
                                                parametrosRelatorio["PExibeRegularRegistros"] = true;
                                            }

                                            parametrosRelatorio.Add("PExibePPTRegistros", false);
                                            if (turmasNovasPPTs.Count() > 0)
                                            {
                                                parametrosRelatorio["PExibePPTRegistros"] = true;
                                            }
                                            
                                            
                                        }
                                    }
                                }
                                else
                                {
                                    relatorio.LocalReport.SubreportProcessing += new Microsoft.Reporting.WebForms.SubreportProcessingEventHandler(renderizaSubRelatorioAlunos);
                                }



                            if (situacaoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.SituacaoTurma.TURMASAENCERRAR)
                            {
                                ItensTurmasPPT = turmasPPTs;
                                ItensTurmasRegulares = turmasRegulares;
                                dataInicial = (DateTime)pDtaFimI;
                                dataFinal = (DateTime)pDtaFimF;
                                relatorio.LocalReport.SubreportProcessing += new Microsoft.Reporting.WebForms.SubreportProcessingEventHandler(renderizaSubRelatorioAlunosTurmasEncerrar);
                            }
                            else if (situacaoTurma == (int)FundacaoFisk.SGF.GenericModel.Turma.SituacaoTurma.TURMASNOVA)
                            {

                                ItensTurmasNovasPPT = turmasNovasPPTs;
                                ItensTurmasNovasRegulares = turmasNovasRegulares;
                                dataInicialTurmasNovas = (DateTime)pDtaI;
                                dataFinalTurmasNovas = (DateTime)pDtaF;
                                relatorio.LocalReport.SubreportProcessing += new Microsoft.Reporting.WebForms.SubreportProcessingEventHandler(renderizaSubRelatorioAlunosTurmasNovas);    
                            }
                            

                            
                            abortou_requisicao = true;
                            relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", caminhoRelatorio, sourceHash, parametrosRelatorio);
                        }
                        else
                        {
                            url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                            if (!abortou_requisicao)
                                Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(Componentes.Utils.Messages.Messages.msgRegNotEnc) + "&stackTrace=");
                        }
                    }
                    else
                    {
                        throw new Exception(Componentes.Utils.Messages.Messages.msgSessaoExpirada);
                    }
            }

            private void montarNomeProfessor(IEnumerable<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> turmas)
            {
                if (turmas != null)
                {
                    foreach (var t in turmas)
                    {
                        string nomes_porfessores = "";
                        if (t.pessoasTurma != null)
                        {
                            //t.nro_alunos = t.pessoasTurma.Count();
                            foreach (var p in t.pessoasTurma)
                                nomes_porfessores += p.dc_reduzido_pessoa + ", ";
                        }
                        //else
                        //    t.nro_alunos = 0;
                        nomes_porfessores = nomes_porfessores.ToString().TrimEnd(',', ' ');
                        t.no_professor = nomes_porfessores;
                        t.pessoasTurma = null;
                    }
                }
            }

            private void renderizaSubRelatorioAlunos(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e)
            {
                try{
                    int cd_turma = Convert.ToInt32(e.Parameters["cd_turma"].Values[0]);                    
                    bool id_turma_ppt = Convert.ToBoolean(e.Parameters["id_turma_ppt"].Values[0]);
                    String cd_situacao_aluno_turma = e.Parameters["cd_situacao_aluno_turma"].Values[0] + "";
                    FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();

                    e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetSubReportSubTurma",
                      apresentadorRelatorio.getRptAlunosTurma(cd_turma, id_turma_ppt, cd_situacao_aluno_turma, this.cdEscolaAluno)));
                }
                catch (Exception exe)
                {
                    logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
                }
            }

            private void renderizaSubRelatorioAlunosTurmasEncerrar(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e)
            {
                try
                {
                    int cd_turma = Convert.ToInt32(e.Parameters["cd_turma"].Values[0]);

                    if (e.ReportPath == "SubRptAlunosTurmaEncerrar")
                    {

                        //String cd_situacao_aluno_turma = e.Parameters["cd_situacao_aluno_turma"].Values[0] + "";
                        List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> itemsTurmaReg = ItensTurmasRegulares.Where(x => x.cd_turma == cd_turma).ToList();
                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();
                        foreach (var item in itemsTurmaReg)
                        {
                            IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI> dataTable =  apresentadorRelatorio.getRptAlunosTurmaEncerrar(item.cd_turma, item.id_turma_ppt, DateTime.ParseExact(item.dtaIniAula, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(item.dtaFim, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), this.cdEscolaAluno).ToList();
                            e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetSubReportSubAlunosTurmaEncerrar", dataTable));
                        }
                    }else if (e.ReportPath == "SubRptAlunosTurmaPPTEncerrar")
                    {
                        
                        List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> itemsTurmaPPT = ItensTurmasPPT.Where(x => x.cd_turma == cd_turma).ToList();
                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();
                        foreach (var item in itemsTurmaPPT)
                        {
                            IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI> dataTable = apresentadorRelatorio.getRptAlunosTurmaEncerrar(item.cd_turma, item.id_turma_ppt, dataInicial, dataFinal, cdEscolaAluno).ToList();
                            e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetSubReportSubAlunosTurmaPPTEncerrar", dataTable ));
                        }

                    }
                }
                catch (Exception exe)
                {
                    logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
                }
            }

            private void renderizaSubRelatorioAlunosTurmasNovas(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e)
            {
                try
                {
                    int cd_turma = Convert.ToInt32(e.Parameters["cd_turma"].Values[0]);

                    if (e.ReportPath == "SubRptAlunosTurmaNova")
                    {

                        //String cd_situacao_aluno_turma = e.Parameters["cd_situacao_aluno_turma"].Values[0] + "";
                        List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> itemsTurmaNovaReg = ItensTurmasNovasRegulares.Where(x => x.cd_turma == cd_turma).ToList();
                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();
                        foreach (var item in itemsTurmaNovaReg)
                        {
                            IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI> dataTable = apresentadorRelatorio.getRptAlunosTurmaNova(item.cd_turma, item.id_turma_ppt, DateTime.ParseExact(item.dtaIniAula, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), item.dtaFim.IsEmpty() ? (DateTime?)null : DateTime.ParseExact(item.dtaFim, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), this.cdEscolaAluno).ToList();
                            e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetSubReportSubAlunosTurmaNova", dataTable));
                        }
                    }
                    else if (e.ReportPath == "SubRptAlunosTurmaPPTNova")
                    {

                        List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> itemsTurmaNovaPPT = ItensTurmasNovasPPT.Where(x => x.cd_turma == cd_turma).ToList();
                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();
                        foreach (var item in itemsTurmaNovaPPT)
                        {
                            IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.AlunoSearchUI> dataTable = apresentadorRelatorio.getRptAlunosTurmaNova(item.cd_turma, item.id_turma_ppt, dataInicialTurmasNovas, dataFinalTurmasNovas, this.cdEscolaAluno).ToList();
                            e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetSubReportSubAlunosTurmaPPTNova", dataTable));
                        }

                    }
                }
                catch (Exception exe)
                {
                    logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
                }
            }

            protected void relatorio_DocumentMapNavigation(object sender, Microsoft.Reporting.WebForms.DocumentMapNavigationEventArgs e)
            {
                //bindRelatorio();
            }

            protected void relatorio_Search(object sender, Microsoft.Reporting.WebForms.SearchEventArgs e)
            {
                //bindRelatorio();
            }

            protected void relatorio_Drillthrough(object sender, Microsoft.Reporting.WebForms.DrillthroughEventArgs e)
            {
                //bindRelatorio();
            }

            protected void relatorio_Load(object sender, EventArgs e)
            {
                /*if(!IsPostBack) 
                    bindRelatorio();*/
            }

            protected void relatorio_BookmarkNavigation(object sender, Microsoft.Reporting.WebForms.BookmarkNavigationEventArgs e)
            {
                //bindRelatorio();
            }

            protected void relatorio_PageNavigation(object sender, Microsoft.Reporting.WebForms.PageNavigationEventArgs e)
            {
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
    </div>
    <form id="Form1" runat="server" style="width: 100%; height: 100%;">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cc1:VisualizadorRelatorio ID="relatorio" runat="server" Height="100%" Width="100%" Font-Names="Verdana" DocumentMapCollapsed="false" ShowFindControls="true"
            ShowDocumentMapButton="true" ShowToolBar="true" ShowPrintButton="true" ProcessingMode="Local" Font-Size="8pt" AsyncRendering="false" SizeToReportContent="True"
            OnDocumentMapNavigation="relatorio_DocumentMapNavigation" OnSearch="relatorio_Search" OnDrillthrough="relatorio_Drillthrough" OnLoad="relatorio_Load"
            OnBookmarkNavigation="relatorio_BookmarkNavigation" OnPageNavigation="relatorio_PageNavigation" KeepSessionAlive="false">
        </cc1:VisualizadorRelatorio>
    </form>
</body>
</html>

