using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class PoliticaComercial
    {
        public enum TipoPeriodoIntervalo {
            DIAS = 1,
            MES = 2
        }

        public string venc_fixo
        {
            get
            {
                return this.id_vencimento_fixo ? "Sim" : "Não";
            }
        }
        public string parc_iguais
        {
            get
            {
                return this.id_parcela_igual ? "Sim" : "Não";
            }
        }
        public string pol_ativa
        {
            get
            {
                return this.id_politica_ativa ? "Sim" : "Não";
            }
        }
        public string periodo_intervalo
        {
            get
            {
                if (this.id_parcela_igual)
                    if(this.nm_periodo_intervalo == (byte)TipoPeriodoIntervalo.DIAS)
                        return "Dia";
                    else
                        if(this.nm_periodo_intervalo == (byte)TipoPeriodoIntervalo.MES)
                            return "Mês";
                        else
                            return "";
                else
                    return "";
            }
        }

        public static PoliticaComercial changeValuePolCom(PoliticaComercial polContext, PoliticaComercial polView)
        {
            polContext.dc_politica_comercial = polView.dc_politica_comercial;
            polContext.id_politica_ativa = polView.id_politica_ativa;
            polContext.id_parcela_igual = polView.id_parcela_igual;
            polContext.id_vencimento_fixo = polView.id_vencimento_fixo;
            polContext.nm_intervalo_parcela = polView.nm_intervalo_parcela;
            polContext.nm_parcelas = polView.nm_parcelas;
            polContext.nm_periodo_intervalo = polView.nm_periodo_intervalo;
            return polContext;
        }

           public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15

                retorno.Add(new DefinicaoRelatorio("dc_politica_comercial", "Descrição", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("venc_fixo", "Vencimento", AlinhamentoColuna.Center, "1.0500in"));
                retorno.Add(new DefinicaoRelatorio("parc_iguais", "Parcelas Iguais", AlinhamentoColuna.Center, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("nm_parcelas", "Nro Parcelas", AlinhamentoColuna.Center, "1.1300in"));
                retorno.Add(new DefinicaoRelatorio("nm_intervalo_parcela", "Intervalo", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("periodo_intervalo", "Período", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("pol_ativa", "Ativo", AlinhamentoColuna.Center, "1.0000in"));
                return retorno;
            }
        }
    }
}
