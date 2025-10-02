using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class NomeContrato
    {
 
        public string relatorioTemporario { get; set; }
        public string previsao_dias
        {
            get
            {
                return this.id_previsao_dias ? "Sim" : "Não";
            }
        }
        public string valor_hora_aula
        {
            get
            {
                return this.id_valor_hora_aula ? "Sim" : "Não";
            }
        }
        public string motivo_aditamento
        {
            get
            {
                return this.id_motivo_aditamento ? "Sim" : "Não";
            }
        }
        public string tipo_pgto
        {
            get
            {
                return this.id_tipo_pgto ? "Sim" : "Não";
            }
        }
        public string nome_ativo
        {
            get
            {
                return this.id_nome_ativo ? "Sim" : "Não";
            }
        }
        public string tipoNomeContrato
        {
            get
            {
                return this.cd_pessoa_escola != null ? "Especializado" : "Padrão";
            }
        }
        public string reajuste_anual
        {
            get
            {
                return this.id_reajuste_anual ? "Sim" : "Não";
            }
        }

        public static NomeContrato changeValuesNomeContrato(NomeContrato noContContext,NomeContrato noContView)
        {
            noContContext.id_motivo_aditamento = noContView.id_motivo_aditamento;
            noContContext.id_nome_ativo = noContView.id_nome_ativo;
            noContContext.id_previsao_dias = noContView.id_previsao_dias;
            noContContext.id_tipo_pgto = noContView.id_tipo_pgto;
            noContContext.id_valor_hora_aula = noContView.id_valor_hora_aula;
            noContContext.id_valor_material = noContView.id_valor_material;
            noContContext.no_contrato = noContView.no_contrato;
            noContContext.no_relatorio = noContView.no_relatorio;
            noContContext.id_reajuste_anual = noContView.id_reajuste_anual;
            return noContContext;
        }

    }
}
