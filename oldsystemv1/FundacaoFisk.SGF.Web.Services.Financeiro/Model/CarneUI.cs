using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public partial class CarneUI : TO
    {
        public String no_pessoa { get; set; }
        public int cd_origem_titulo { get; set; }
        public int cd_titulo { get; set; }
        public int nm_titulo { get; set; }
        public int cd_pessoa_empresa { get; set; }
        public String no_pessoa_empresa { get; set; }
        public int nm_parcela_titulo { get; set; }
        public DateTime dt_vcto_titulo { get; set; }
        public decimal vl_titulo { get; set; }
        public String dc_mail_escola { get; set; }
        public String dc_fone_escola { get; set; }
        public int? nm_matricula_contrato { get; set; }
        public double pc_juros_titulo { get; set; }
        public double pc_multa_titulo { get; set; }
        public double? pc_juros_dia { get; set; }
        public double? pc_multa { get; set; }
        public String no_curso { get; set; }
        public float val_acr { get; set; }
        public DateTime dt_venc_corrigido { get; set; }
        public float vl_desc { get; set; }
        public float vl_desc_pos_politica { get; set; }
        public DateTime? dt_politica { get; set; }
        public float vl_multa { get; set; }
        public EnderecoSGF endereco { get; set; }
        public int nm_max_titulo { get; set; }
        public string dc_tipo_titulo { get; set; }
        public byte nm_dias_carencia { get; set; }
        public string itens { get; set; }
        public bool conta_segura { get; set; }
        public string dc_num_cgc { get; set; }
        public string dc_endereco_escola
        {
            get
            {
                string retorno = "";

                if (endereco.Logradouro != null)
                {
                    if (endereco.TipoLogradouro != null && !String.IsNullOrEmpty(endereco.TipoLogradouro.sg_tipo_logradouro))
                        retorno += endereco.TipoLogradouro.sg_tipo_logradouro;
                    retorno += " " + endereco.Logradouro.no_localidade;
                    if (!String.IsNullOrEmpty(endereco.dc_num_endereco))
                        retorno += " " + endereco.dc_num_endereco;
                    if (!String.IsNullOrEmpty(endereco.Bairro.no_localidade))
                        retorno += ", " + endereco.Bairro.no_localidade;
                }
                return retorno;
            }
        }
        public string dc_bairro_escola
        {
            get
            {
                string retorno = "";

                if (endereco.Logradouro != null)
                {
                    if (!String.IsNullOrEmpty(endereco.Bairro.no_localidade))
                        retorno += " " + endereco.Bairro.no_localidade;
                }
                return retorno;
            }
        }
        public string dc_cidade_escola
        {
            get
            {
                string retorno = "";

                if (endereco.Logradouro != null)
                {
                    retorno += " " + endereco.Cidade.no_localidade;
                    retorno += " - " + endereco.Estado.Estado.sg_estado;
                }
                return retorno;
            }
        }
        public bool possuiPolitica
        {
            get
            {
                bool existe = false;
                if (dt_politica.HasValue && ((dt_politica < dt_venc_corrigido) || (nm_dias_carencia > 0)))
                    existe = true;
                return existe;
            }
        }

        public string dt_politica_desconto
        {
            get
            {
                string retorno = "";
                if (dt_politica == null)
                    retorno = String.Format("{0:dd/MM/yyyy}", dt_venc_corrigido);
                else
                    if (dt_venc_corrigido < dt_politica)
                        retorno = String.Format("{0:dd/MM/yyyy}", dt_venc_corrigido);
                    else
                        retorno = String.Format("{0:dd/MM/yyyy}", dt_politica);
                return retorno;
            }
        }

        public string dt_ini_vencimento
        {
            get
            {
                string retorno = "";
                if (dt_politica == null)
                    retorno = String.Format("{0:dd/MM/yyyy}", dt_venc_corrigido);
                else
                    if (dt_venc_corrigido < dt_politica)
                        retorno = String.Format("{0:dd/MM/yyyy}", dt_venc_corrigido.AddDays(1));
                    else
                        retorno = String.Format("{0:dd/MM/yyyy}", ((DateTime)dt_politica).AddDays(1));
                return retorno;
            }
        }

        public string dt_fim_vencimento
        {
            get
            {
                string retorno = "";
                if (nm_dias_carencia > 0)
                    retorno = String.Format("{0:dd/MM/yyyy}", dt_venc_corrigido.AddDays(nm_dias_carencia));
                else
                    retorno = String.Format("{0:dd/MM/yyyy}", dt_venc_corrigido);
                return retorno;
            }
        }
    }
}
