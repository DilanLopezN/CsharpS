using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class Espelho : TO
    {
        public int cd_movimento { get; set; }
        public string no_pessoa { get; set; }
        public string no_pessoa_empresa { get; set; }
        public Nullable<int> nm_movimento { get; set; }
        public string dc_serie_movimento { get; set; }
        public DateTime dt_emissao_movimento { get; set; }
        public DateTime dt_vcto_movimento { get; set; }
        public DateTime dt_mov_movimento { get; set; }
        public string dc_tipo_financeiro { get; set; }
        public string dc_politica_comercial { get; set; }
        public decimal pc_acrescimo { get; set; }
        public decimal vl_acrescimo { get; set; }
        public decimal pc_desconto { get; set; }
        public decimal vl_desconto { get; set; }
        public decimal? vl_total_itens { get; set; }
        public decimal? vl_total_liq { get; set; }
        public string tx_obs_movimento { get; set; }
        public byte id_tipo_movimento { get; set; }

        public string dc_tipo_movimento { 
            set{}
            get{
                string retorno ="";
                if(id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA)
                    retorno = "Entradas";
                if(id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DESPESA)
                    retorno = "Despesas";
                if(id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA)
                    retorno = "Saídas";
                if (id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO)
                    retorno = "Serviços";
                return retorno;
            }
        }

        //Propriedades da Cópia do Espelho:
        public string num_cnpj { get; set; }
        public int cd_pessoa { get; set; }
        public string dc_num_insc_estadual { get; set; }
        public string dc_num_insc_municipal { get; set; }
        public int cd_pessoa_empresa { get; set; }
        public string dc_item_movimento { get; set; }
        public string qt_item_movimento {
            get {
                if(qt_item.HasValue)
                    return string.Format("{0:#,0}", this.qt_item);
                else
                    return "";
            }
        }
        public int? qt_item { get; set; }
        public string vl_unitario_item {
            get {
                if(vl_unitario.HasValue)
                    return string.Format("{0:#,0.00}", this.vl_unitario);
                else
                    return "";
            }
        }
        public double? vl_unitario { get; set; }
        public decimal vl_acrescimo_item { get; set; }
        public decimal vl_desconto_item { get; set; }
        public decimal vl_liquido_item { get; set; }

        public string vl_total_item {
            get {
                if(vl_total.HasValue)
                    return string.Format("{0:#,0.00}", this.vl_total);
                else
                    return "";
            }
        }
        public double? vl_total {
            get {
                return this.qt_item * this.vl_unitario;
            }
        }
        public decimal vl_total_acres { get; set; }
        public decimal vl_total_desc { get; set; }
        //public decimal vl_total_liq { get; set; }

        public string dc_endereco_escola { get { return enderecoEscola.enderecoCompletoEspelho; } }
        private string _dc_bairro_escola { get; set; }
        public string dc_bairro_escola {
            get {
                return dc_num_cep_escola + " " + _dc_bairro_escola;
            }
            set { _dc_bairro_escola = value; }
        }
        private string _dc_cidade_escola { get; set; }
        public string dc_cidade_escola {
            get {
                return _dc_cidade_escola + "-" + sg_estado_escola;
            }
            set {
                _dc_cidade_escola = value;
            }
        }
        public string dc_num_cep_escola { get; set; }
        public string sg_estado_escola { get; set; }
        public string dc_mail_escola { get; set; }
        public string dc_fone_escola { get; set; }

        public string dc_endereco_pessoa { get { return enderecoPessoa.enderecoCompletoEspelho; } }
        private string _dc_bairro_pessoa { get; set; }
        public string dc_bairro_pessoa {
            get {
                return dc_num_cep_pessoa + " " + _dc_bairro_pessoa;
            }
            set { _dc_bairro_pessoa = value; }
        }
        private string _dc_cidade_pessoa { get; set; }
        public string dc_cidade_pessoa {
            get {
                return _dc_cidade_pessoa + "-" + sg_estado_pessoa;
            }
            set {
                _dc_cidade_pessoa = value;
            }
        }
        public string dc_num_cep_pessoa { get; set; }
        public string sg_estado_pessoa { get; set; }
        public string dc_mail_pessoa { get; set; }
        public string dc_fone_pessoa { get; set; }
        public string cpf_pessoa { get; set; }

        public EnderecoSGF enderecoEscola { get; set; }
        public EnderecoSGF enderecoPessoa { get; set; }
    }
}
