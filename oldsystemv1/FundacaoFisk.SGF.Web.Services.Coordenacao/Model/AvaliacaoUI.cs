using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class AvaliacaoUI : TO
    {
        public int cd_tipo_avaliacao { get; set; }
        public string dc_tipo_avaliacao { get; set; }
        public int cd_criterio_avaliacao { get; set; }
        public string dc_criterio_avaliacao { get; set; }
        public string dc_criterio_abreviado { get; set; }
        public Nullable<byte> vl_nota { get; set; }
        public Nullable<byte> nm_ordem_avaliacao { get; set; }
        public Nullable<decimal> nm_peso_avaliacao { get; set; }
        public int? valorTotalNota{ get; set; }
        public bool id_avaliacao_ativa { get; set; }
        public int cd_avaliacao { get; set; }
        
        public string peso
        {
            get
            {
                if (this.nm_peso_avaliacao == null)
                    return "";
                return string.Format("{0,00}", this.nm_peso_avaliacao);
            }
        }

        public string ordem
        {
            get
            {
                return this.nm_ordem_avaliacao == null ? "-" : this.nm_ordem_avaliacao.ToString();
            }
        }

        public string avaliacao_ativa
        {
            get
            {
                return this.id_avaliacao_ativa ? "Sim" : "Não";
            }
        }

        public static AvaliacaoUI fromAvaliacao(Avaliacao avaliacao, String tipoAvaliacao, String criterio)
        {
            AvaliacaoUI avaliacaoUI = new AvaliacaoUI
            {
              cd_avaliacao = avaliacao.cd_avaliacao,
              cd_criterio_avaliacao = avaliacao.cd_criterio_avaliacao,
              cd_tipo_avaliacao = avaliacao.cd_tipo_avaliacao,
              dc_criterio_avaliacao = criterio,
              //dc_criterio_abreviado = abreviatura,
              dc_tipo_avaliacao = tipoAvaliacao,
              nm_ordem_avaliacao = avaliacao.nm_ordem_avaliacao,
              vl_nota = avaliacao.vl_nota,
              nm_peso_avaliacao = avaliacao.nm_peso_avaliacao,
              id_avaliacao_ativa = avaliacao.id_avaliacao_ativa
            };
            return avaliacaoUI;
        }

        //Método para relatório
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                retorno.Add(new DefinicaoRelatorio("dc_tipo_avaliacao", "Tipo Avaliacao", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("dc_criterio_avaliacao", "Critério", AlinhamentoColuna.Left, "3.8000in"));
                //retorno.Add(new DefinicaoRelatorio("dc_criterio_abreviado", "Abreviatura", AlinhamentoColuna.Left));
                retorno.Add(new DefinicaoRelatorio("vl_nota", "Nota", AlinhamentoColuna.Right));
                retorno.Add(new DefinicaoRelatorio("nm_peso_avaliacao", "Peso", AlinhamentoColuna.Right));
                retorno.Add(new DefinicaoRelatorio("nm_ordem_avaliacao", "Ordem", AlinhamentoColuna.Right));
                return retorno;
            }
        }
    }    
}
