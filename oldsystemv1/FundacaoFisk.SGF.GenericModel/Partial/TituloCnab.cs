using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class TituloCnab
    {
        public enum StatusTituloCnab
        {
            INICIAL = 0,
            ENVIADO_GERADO = 1,
            BAIXA_MANUAL = 2,
            CONFIRMADO_ENVIO = 3,
            BAIXA_MANUAL_CONFIRMADO = 4,
            PEDIDO_BAIXA = 5,
            CONFIRMADO_PEDIDO_BAIXA = 6

        }
        //Campos TituloCnab
        public string nomeResponsavel { get; set; }
        public string nomePessoaTitulo { get; set; }
        public string emailPessoaTitulo { get; set; }
        public int? cd_turma_PPT { get; set; }
        public string no_turma_titulo { get; set; }
        public int cd_produto_escola { get; set; }
        public int cd_pessoa_responsavel { get; set; }
        public string vlTitulo
        {
            get
            {
                return  this.Titulo!= null ? string.Format("{0:#,0.00}", this.Titulo.vl_titulo) : "";
            }
        }
        public string vlSaldoTitulo
        {
            get
            {
                return this.Titulo!= null ? string.Format("{0:#,0.00}", this.Titulo.vl_saldo_titulo) : "";
            }
        }
        public string pcJurosCalc {
            get {
                return string.Format("{0:#,0.00}", this.pc_juros_titulo);
            }
        }
        public string pcMultaCalc {
            get {
                return string.Format("{0:#,0.00}", this.pc_multa_titulo);
            }
        }
        private string _dt_vcto = "";
        public string dt_vcto
        {
            get
            {
                if (String.IsNullOrEmpty(_dt_vcto) && this.Titulo != null)
                    return String.Format("{0:dd/MM/yyyy}", this.Titulo.dt_vcto_titulo);
                else
                    return _dt_vcto;
            }
            set { 
                this._dt_vcto = value;
            }

        }
        private string _dt_emissao = "";
        public string dt_emissao
        {
            get
            {
                if (String.IsNullOrEmpty(_dt_emissao) && this.Titulo != null)
                    return String.Format("{0:dd/MM/yyyy}", this.Titulo.dt_emissao_titulo);
                else
                    return _dt_emissao;
            }
            set {
                this._dt_emissao = value;
            }
        }

        public String descLocalMovtoTituloCnab {
            get {
                var retorno = "";
                if(this.LocalMovimento != null && this.LocalMovimento.nm_tipo_local == (byte) FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO)
                    return this.LocalMovimento.no_local_movto + " | ag.:" + this.LocalMovimento.nm_agencia + " | c/c:"
                        + this.LocalMovimento.nm_conta_corrente + "-" + this.LocalMovimento.nm_digito_conta_corrente;
                else if(this.LocalMovimento != null)
                    return this.LocalMovimento.no_local_movto;
                else
                    return retorno;
            }
        }
        
        private string _descLocalMovtoTituloCnab = "";
        public String descLocalMovtoTitulo
        {
            get
            {
                if (this.Titulo != null && this.Titulo.LocalMovto != null && this.Titulo.LocalMovto.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO)
                    return this.Titulo.LocalMovto.no_local_movto + " | ag.:" + this.Titulo.LocalMovto.nm_agencia + " | c/c:"
                        + this.Titulo.LocalMovto.nm_conta_corrente + "-" + this.Titulo.LocalMovto.nm_digito_conta_corrente;
                else if (this.Titulo != null && this.Titulo.LocalMovto != null)
                    return this.Titulo.LocalMovto.no_local_movto;
                else
                    return _descLocalMovtoTituloCnab;
            }
            set {
                this._descLocalMovtoTituloCnab = value;
            }
        }
        public string nm_parcela_e_titulo
        {
            get
            {
                string retorno = "";
                if (this.Titulo != null)
                {
                    if (this.Titulo.nm_titulo.HasValue)
                        retorno += this.Titulo.nm_titulo;
                    if (this.Titulo.nm_parcela_titulo.HasValue)
                        retorno += "-" + this.Titulo.nm_parcela_titulo;
                }

                return retorno;
            }
        }
        public byte id_status_titulo_cnab { get; set; }
        public string dc_nosso_numero { get; set; }
        public bool id_alterou_txt_cnab { get; set; }
        public string statusTituloCnab
        {
            get
            {
                string descStatusCnab = "";
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("pt-BR");
                switch (id_status_cnab_titulo)
                {
                    case (int)StatusTituloCnab.INICIAL: descStatusCnab = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(StatusTituloCnab.INICIAL.ToString()).ToLower());
                        break;
                    case (int)StatusTituloCnab.ENVIADO_GERADO: descStatusCnab = "Envio/Gerado";
                        break;
                    case (int)StatusTituloCnab.BAIXA_MANUAL: descStatusCnab = "Baixa Manual";
                        break;
                    case (int)StatusTituloCnab.CONFIRMADO_ENVIO: descStatusCnab = "Confirmado Envio";
                        break;
                    case (int)StatusTituloCnab.BAIXA_MANUAL_CONFIRMADO: descStatusCnab = "Baixa Manual Confirmado";
                        break;
                    case (int)StatusTituloCnab.PEDIDO_BAIXA: descStatusCnab = "Pedido Baixa";
                        break;
                    case (int)StatusTituloCnab.CONFIRMADO_PEDIDO_BAIXA: descStatusCnab = "Confirmado Pedido Baixa";
                        break;
                }
                return descStatusCnab;
            }
        }
        //Campos grade view título Cnab
        public Nullable<int> nro_contrato { get; set; }
        public int cd_produto { get; set; }
        public int cd_turma { get; set; }
        public int? cd_aluno { get; set; }
        public int cd_pessoa_titulo { get; set; }
        public bool id_mostrar_3_boletos_pagina { get; set; }
        public int nm_dias_protesto { get; set; }

        public static TituloCnab toUperCaseDadosRemessa(TituloCnab t)
        {
            t.tx_mensagem_cnab = !string.IsNullOrEmpty(t.tx_mensagem_cnab) ? t.tx_mensagem_cnab.ToUpper().Replace(".", " ") : "";
            t.emailPessoaTitulo = !string.IsNullOrEmpty(t.emailPessoaTitulo) ? t.emailPessoaTitulo.ToUpper().Replace(".", " ") : "";
            if (t != null && t.LocalMovimento != null)
            {
                if (t.LocalMovimento.Empresa != null)
                {
                    t.LocalMovimento.Empresa.no_pessoa = t.LocalMovimento.Empresa.no_pessoa.ToUpper().Replace(".", " ");
                    t.LocalMovimento.Empresa.nm_cpf_cgc = t.LocalMovimento.Empresa.dc_num_cgc.ToUpper();
                }
                if (t.LocalMovimento.Empresa != null)
                {
                    if (t.LocalMovimento.Empresa.EnderecoPrincipal != null)
                    {
                        if (t.LocalMovimento.Empresa.EnderecoPrincipal.TipoLogradouro != null && !string.IsNullOrEmpty(t.LocalMovimento.Empresa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro))
                            t.LocalMovimento.Empresa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro = t.LocalMovimento.Empresa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro.ToUpper().Replace(".", " ");
                        if (t.LocalMovimento.Empresa.EnderecoPrincipal.Bairro != null && !string.IsNullOrEmpty(t.LocalMovimento.Empresa.EnderecoPrincipal.Bairro.no_localidade))
                            t.LocalMovimento.Empresa.EnderecoPrincipal.Bairro.no_localidade = t.LocalMovimento.Empresa.EnderecoPrincipal.Bairro.no_localidade.ToUpper().Replace(".", " ");
                        if (t.LocalMovimento.Empresa.EnderecoPrincipal.Estado != null && t.LocalMovimento.Empresa.EnderecoPrincipal.Estado.Estado != null &&
                            !string.IsNullOrEmpty(t.LocalMovimento.Empresa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro))
                            t.LocalMovimento.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado = t.LocalMovimento.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado.ToUpper().Replace(".", " ");
                        if (t.LocalMovimento.Empresa.EnderecoPrincipal.Cidade != null && !string.IsNullOrEmpty(t.LocalMovimento.Empresa.EnderecoPrincipal.Cidade.no_localidade))
                            t.LocalMovimento.Empresa.EnderecoPrincipal.Cidade.no_localidade = t.LocalMovimento.Empresa.EnderecoPrincipal.Cidade.no_localidade.ToUpper().Replace(".", " ");
                        if (t.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro != null && !string.IsNullOrEmpty(t.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade))
                            t.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade = t.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade.ToUpper().Replace(".", " ");
                    }
                }
                if (t.Titulo != null && t.Titulo.Pessoa != null)
                {
                    if (!string.IsNullOrEmpty(t.Titulo.Pessoa.no_pessoa))
                        t.Titulo.Pessoa.no_pessoa = t.Titulo.Pessoa.no_pessoa.ToUpper().Replace(".", " ");
                    if (t.Titulo.Pessoa.EnderecoPrincipal != null)
                    {
                        if (t.Titulo.Pessoa.EnderecoPrincipal.TipoLogradouro != null && !string.IsNullOrEmpty(t.Titulo.Pessoa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro))
                            t.Titulo.Pessoa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro = t.Titulo.Pessoa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro.ToUpper().Replace(".", " ");
                        if (t.Titulo.Pessoa.EnderecoPrincipal.Bairro != null && !string.IsNullOrEmpty(t.Titulo.Pessoa.EnderecoPrincipal.Bairro.no_localidade))
                            t.Titulo.Pessoa.EnderecoPrincipal.Bairro.no_localidade = t.Titulo.Pessoa.EnderecoPrincipal.Bairro.no_localidade.ToUpper().Replace(".", " ");
                        if (t.Titulo.Pessoa.EnderecoPrincipal.Estado != null && t.Titulo.Pessoa.EnderecoPrincipal.Estado.Estado != null &&
                            !string.IsNullOrEmpty(t.Titulo.Pessoa.EnderecoPrincipal.Estado.Estado.sg_estado))
                            t.Titulo.Pessoa.EnderecoPrincipal.Estado.Estado.sg_estado = t.Titulo.Pessoa.EnderecoPrincipal.Estado.Estado.sg_estado.ToUpper().Replace(".", " ");
                        if (t.Titulo.Pessoa.EnderecoPrincipal.Cidade != null && !string.IsNullOrEmpty(t.Titulo.Pessoa.EnderecoPrincipal.Cidade.no_localidade))
                            t.Titulo.Pessoa.EnderecoPrincipal.Cidade.no_localidade = t.Titulo.Pessoa.EnderecoPrincipal.Cidade.no_localidade.ToUpper().Replace(".", " ");
                        if (t.Titulo.Pessoa.EnderecoPrincipal.Logradouro != null && !string.IsNullOrEmpty(t.Titulo.Pessoa.EnderecoPrincipal.Logradouro.no_localidade))
                            t.Titulo.Pessoa.EnderecoPrincipal.Logradouro.no_localidade = t.Titulo.Pessoa.EnderecoPrincipal.Logradouro.no_localidade.ToUpper().Replace(".", " ");
                    }
                }
            }
            return t;
        }
    }
}
