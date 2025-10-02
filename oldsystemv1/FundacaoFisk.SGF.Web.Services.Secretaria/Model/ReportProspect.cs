using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class ReportProspect : TO
    {
        public int cd_prospect { get; set; }
        public int cd_produto { get; set; }
        public string no_produto { get; set; }
        public int cd_usuario { get; set; }
        public int cd_pessoa_fisica { get; set; }
        public string no_pessoa { get; set; }
        public string no_usuario_funcionario { get; set; }
        public string email { get; set; }
        public string telefone { get; set; }
        public string celular { get; set; }
        public System.DateTime dt_cadastramento { get; set; }
        public System.DateTime? dt_matricula { get; set; }
        public IEnumerable<Produto> listaProdutos { get; set; }
        public IEnumerable<ProspectPeriodo> listaPeriodos { get; set; }
        public IEnumerable<MotivoNaoMatricula> listaMotivosNaoMatricula { get; set; }
        public int qtdProspectInicial { get; set; }
        public int qtdProspectFinal { get; set; }
        public string porcentagem_diferença { get; set; }
        public string no_faixa_etaria { get; set; }
        public string dta_atendimento
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_cadastramento);
            }

        }

        public string dta_matricula
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_matricula);
            }

        }

        public string produtos_prospect
        {
            get
            {
                string retorno = "";
                if (listaProdutos != null && listaProdutos.Count() > 0)
                {
                    foreach (Produto p in listaProdutos)
                        retorno += " " + p.no_produto + ",";
                    retorno = retorno.ToString().TrimEnd(',', ' ');
                }
                return retorno;
            }
        }

        public string periodos_prospect
        {
            get
            {
                string retorno = "";
                if (listaPeriodos != null && listaPeriodos.Count() > 0)
                {
                    foreach (ProspectPeriodo p in listaPeriodos)
                    {
                        switch (p.id_periodo)
                        {
                            case (int)Prospect.PeriodoProspect.MANHA: retorno += " Manhã,";
                                break;
                            case (int)Prospect.PeriodoProspect.TARDE: retorno += " Tarde,";
                                break;
                            case (int)Prospect.PeriodoProspect.NOITE: retorno += " Noite,";
                                break;
                        }
                    }
                    retorno = retorno.ToString().TrimEnd(',', ' ');
                }
                return retorno;
            }
        }

        public string listaMotivosNMatricula
        {
            get
            {
                string retorno = "";
                if (listaMotivosNaoMatricula != null && listaMotivosNaoMatricula.Count() > 0)
                {
                    foreach (MotivoNaoMatricula m in listaMotivosNaoMatricula)
                        retorno += " " + m.dc_motivo_nao_matricula + ",";
                    retorno = retorno.ToString().TrimEnd(',', ' ');
                }
                return retorno;
            }
        }

        public string situacao
        {
            get
            {
                string retorno = "";
                if (dt_matricula != null)
                    retorno = "Matriculado";
                return retorno;
            }
        }

        public static List<ReportProspect> conversaoDadosComparativoProspets(List<ReportProspect> prospectsPrimeirofiltro, List<ReportProspect> prospectsSegundofiltro)
        {
            List<ReportProspect> prospectsPorProduto = new List<ReportProspect>();
            if(prospectsPrimeirofiltro == null)
                prospectsPrimeirofiltro = new List<ReportProspect>();
            if(prospectsSegundofiltro == null)
                prospectsSegundofiltro = new List<ReportProspect>();
            List<ReportProspect> listTotal = prospectsPrimeirofiltro.Union(prospectsSegundofiltro).ToList();
            listTotal = listTotal.GroupBy(x => new { x.cd_produto, x.no_produto }).Select(g => new ReportProspect { cd_produto = g.Key.cd_produto, no_produto = g.Key.no_produto }).ToList();
            if (listTotal != null && listTotal.Count() > 0)
                foreach (ReportProspect rp in listTotal)
                {
                    int countProspectPeriodo = prospectsPrimeirofiltro.Where(x=> x.cd_produto == rp.cd_produto).Count();
                    int countCompProspectPeriodo = prospectsSegundofiltro.Where(x=> x.cd_produto == rp.cd_produto).Count();
                    string porcentagem = "0";
                    if (countProspectPeriodo > 0 || countCompProspectPeriodo > 0)
                    {
                        if (countProspectPeriodo <= 0)
                            porcentagem = "100";
                        else
                            if (countCompProspectPeriodo <= 0)
                                porcentagem = "-100";
                            else
                            {
                                decimal perc;
                                if (countCompProspectPeriodo > countProspectPeriodo)
                                    perc = (countCompProspectPeriodo * 100) / countProspectPeriodo;
                                else
                                    perc = (countProspectPeriodo * 100) / countCompProspectPeriodo;
                                //if (perc == 1 && ((countCompProspectPeriodo * 100) / countProspectPeriodo) - 100 == 100)
                                //    perc = 0;
                                perc = Decimal.Round(perc, 2);
                                if (countProspectPeriodo > countCompProspectPeriodo)
                                    perc = decimal.Negate(perc);
                                porcentagem = "" + perc;
                            }
                    }

                    prospectsPorProduto.Add(new ReportProspect
                    {
                        cd_produto = rp.cd_produto,
                        no_produto = rp.no_produto,
                        qtdProspectInicial = countProspectPeriodo,
                        qtdProspectFinal = countCompProspectPeriodo,
                        porcentagem_diferença = porcentagem
                    });
                }
            return prospectsPorProduto;
        }
   }
}
