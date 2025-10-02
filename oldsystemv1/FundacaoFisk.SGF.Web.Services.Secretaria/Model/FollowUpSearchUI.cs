using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class FollowUpSearchUI : TO
    {
        public int cd_follow_up { get; set; }
        public System.DateTime dt_follow_up { get; set; }
        public Nullable<System.DateTime> dt_proximo_contato { get; set; }
        public bool id_follow_resolvido { get; set; }
        public bool id_follow_lido { get; set; }
        public byte id_tipo_follow { get; set; }
        public byte id_tipo_follow_pesq { get; set; }
        public string no_usuario_origem { get; set; }
        public string no_usuario_destino { get; set; }
        public string no_prospect_aluno { get; set; }

        public string desc_tipo
        {
            get
            {
                string retorno = "";
                switch (id_tipo_follow)
                {
                    case (int)FundacaoFisk.SGF.GenericModel.FollowUp.TipoFollowUp.INTERNO: retorno = "Interno";
                        break;
                    case (int)FundacaoFisk.SGF.GenericModel.FollowUp.TipoFollowUp.PROSPECT_ALUNO: retorno = "Prospect/Aluno";
                        break;
                    case (int)FundacaoFisk.SGF.GenericModel.FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL: retorno = "Administração Geral";
                        break;
                }
                return retorno;
            }
        }
        public string dta_data
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy HH:mm:ss}", dt_follow_up.ToLocalTime());
            }
        }
        public string dta_proximo_contato
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_proximo_contato);
            }
        }
        public string lido
        {
            get
            {
                return id_follow_lido ? "Sim" : "Não";
            }
        }
        public string resolvido
        {
            get
            {
                return id_follow_resolvido ? "Sim" : "Não";
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_criterio_avaliacao", "Código"));
                retorno.Add(new DefinicaoRelatorio("desc_tipo", "Tipo", AlinhamentoColuna.Left, "1.4000in"));
                retorno.Add(new DefinicaoRelatorio("no_usuario_origem", "Usuário de Origem", AlinhamentoColuna.Left, "1.6000in"));
                if (id_tipo_follow_pesq > 0 && id_tipo_follow_pesq == (int)FundacaoFisk.SGF.GenericModel.FollowUp.TipoFollowUp.PROSPECT_ALUNO)
                  retorno.Add(new DefinicaoRelatorio("no_prospect_aluno", "Prospect/Aluno", AlinhamentoColuna.Left, "1.6000in"));
                else
                  retorno.Add(new DefinicaoRelatorio("no_usuario_destino", "Usuário de Destino", AlinhamentoColuna.Left, "1.6000in"));
                retorno.Add(new DefinicaoRelatorio("dta_data", "Data", AlinhamentoColuna.Left, "1.5000in"));
                retorno.Add(new DefinicaoRelatorio("dta_proximo_contato", "Data Próximo", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("lido", "Lido", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("resolvido", "Resolvido", AlinhamentoColuna.Center, "1.3000in"));
                return retorno;
            }
        }
    }
}
