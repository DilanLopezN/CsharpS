using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Titulo
    {

        public enum NaturezaTitulo
        {
            TODAS = 0,
            RECEBER = 1,
            PAGAR = 2
        }

        public enum StatusTitulo
        {
            ABERTO = 1,
            FECHADO = 2
        }

        public enum StatusCnabTitulo
        {
            INICIAL = 0,
            ENVIADO_GERADO = 1,
            BAIXA_MANUAL = 2,
            CONFIRMADO_ENVIO = 3,
            BAIXA_MANUAL_CONFIRMADO = 4,
            PEDIDO_BAIXA = 5,
            CONFIRMADO_PEDIDO_BAIXA = 6
        }

        public enum TipoTitulo
        {
            Todos = 0,
            PP = 1,
            TM = 2,
            TA = 3,
            ME = 4,
            MA = 5, 
            AD = 6,
            AA = 7,
            MM = 8, 
            NF = 9
        }

        public enum OrigemTitulo
        {
            CONTRATO = 22
        }

        public enum StatusTituloBaixaAut
        {
            GERADAS = 1,
            NAO_GERADAS = 2
        }

        public enum TipoMovimento
        {
            RECEBER = 0,
            PAGAR = 1,
            RECEBIDAS = 2,
            PAGAS = 3
        };

        public List<LocalMovto> bancos { get; set; }
        public List<TipoFinanceiro> tipoDocumentos { get; set; }
        public String nomeResponsavel { get; set; }
        public String nomeCliente { get; set; }
        public String nomeAluno { get; set; }
        //public String descLocalMovto { get; set; }
        public decimal percentualResp { get; set; }
        public String tipoDoc { get; set; }
        public bool tituloEdit { get; set; }
        public bool primeiraParc { get; set; }
        public decimal vl_desc_1parc { get; set; }
        public decimal vl_divida { get; set; }
        public int id { get; set; }
        public bool tituloBaixado { get; set; }
        public string dc_plano_conta { get; set; }
        private String _descLocalMovto = String.Empty;
        public int nm_tipo { get; set; }
        public string dc_tipo_liquidacao { get; set; }
        public decimal? vl_liquidacao_baixa { get; set; }
        public bool possuiBaixa { get; set; }
        public bool possuiBaixaBolsa { get; set; }
        public int? cd_turma_titulo { get; set; }
        public int cd_produto { get; set; }
        public string no_turma_titulo { get; set; }
        public Nullable<int> nm_contrato { get; set; }
        public bool id_carteira_registrada_localMvto { get; set; }
        public int diaSugerido { get; set; }
        public Cheque Cheque { get; set; }
        public bool existe_titulo_transacao { get; set; }
        public bool localMovEdit { get; set; }
        public decimal vl_pago_bolsa { get; set; }
        public int ultimo_nm_parcela_titulo { get; set; }
        public bool alterou_local_movto { get; set; }
        public byte nm_tipo_local { get; set; }
        public int? cd_local_banco { get; set; }
        public int cd_aditamento { get; set; }
        public string no_pessoa { get; set; }
        public string no_emissor { get; set; }
        public string no_local_movimento { get; set; }
        public double vl_taxa { get; set; }
        public bool alterou_responsavel_titulo { get; set; }
        public bool alterou_dt_vcto_titulo { get; set; }
        public CursoContrato CursoContrato { get; set; }

        public decimal vl_saldo_corrigido { get; set; }
        public DateTime? dt_emissao_cnab { get; set; }

        public String descLocalMovto
        {
            get
            {
                if (this.LocalMovto != null && this.LocalMovto.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO)
                    return this.LocalMovto.no_local_movto + " | ag.:" + this.LocalMovto.nm_agencia + " | c/c:"
                        + this.LocalMovto.nm_conta_corrente + "-" + this.LocalMovto.nm_digito_conta_corrente;
                else if (this.LocalMovto != null)
                    return this.LocalMovto.no_local_movto;
                else
                    return _descLocalMovto;
            }
            set
            {
                _descLocalMovto = value;
            }
        }
        public string nm_parcela_e_titulo
        {
            get
            {
                string retorno = "";
                if (this.nm_titulo.HasValue)
                    retorno += this.nm_titulo;
                if (this.nm_parcela_titulo.HasValue)
                    retorno += "-" + this.nm_parcela_titulo;

                return retorno;
            }
        }

        public string dt_emissao
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", this.dt_emissao_titulo);
            }
        }

        public string dt_liquidacao
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", this.dt_liquidacao_titulo);
            }
        }

        public string dt_vcto
        {
            get
            {
                return this.dt_vcto_titulo.CompareTo(DateTime.MinValue) == 0 ? "" : String.Format("{0:dd/MM/yyyy}", this.dt_vcto_titulo);
            }
        }
        private string _nm_atraso = null;
        public string nm_atraso
        {
            get
            {
                int dias = 0;
                if (_nm_atraso == null)
                {
                    TimeSpan diferenca = DateTime.UtcNow - this.dt_vcto_titulo;
                    dias = diferenca.Days;

                    if (dias < 0)
                    {
                        if (diferenca.TotalSeconds > 0)
                            dias = 1;
                        else
                            dias = 0;
                    }
                }
                else
                    return _nm_atraso;
                return dias + "";
            }
            set
            {
                _nm_atraso = value;
            }
        }
        public string vlLiquidacaoBaixa
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_liquidacao_baixa);
            }
        }
        public string vlSaldoTitulo
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_saldo_titulo);
            }
        }

        public string vlSaldoCorrigido
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_saldo_corrigido);
            }
        }


        public string vlTitulo
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_titulo);
            }
        }

        public string vlTaxaCartao
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_taxa_cartao);
            }
        }
        
        public string natureza
        {
            get
            {
                string descNatureza = "";
                if (id_natureza_titulo != null)
                    if (id_natureza_titulo == (int)NaturezaTitulo.RECEBER)
                        descNatureza = "Receber";
                    else
                        descNatureza = "Pagar";
                return descNatureza;
            }
        }
        public string statusTitulo
        {
            get
            {
                string descStatus = "";
                if (id_natureza_titulo != null)
                    if (id_status_titulo == (int)StatusTitulo.ABERTO)
                        descStatus = "Aberto";
                    else
                        descStatus = "Fechado";
                return descStatus;
            }
        }

        public string statusCnabTitulo
        {
            get
            {
                string descStatusCnab = "";
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("pt-BR");
                switch (id_status_cnab)
                {
                    case (int)StatusCnabTitulo.INICIAL: descStatusCnab = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(StatusCnabTitulo.INICIAL.ToString()).ToLower());
                        break;
                    case (int)StatusCnabTitulo.ENVIADO_GERADO: descStatusCnab = "Envio/Gerado";
                        break;
                    case (int)StatusCnabTitulo.BAIXA_MANUAL: descStatusCnab = "Baixa Manual";
                        break;
                    case (int)StatusCnabTitulo.CONFIRMADO_ENVIO: descStatusCnab = "Confirmado Envio";
                        break;
                    case (int)StatusCnabTitulo.BAIXA_MANUAL_CONFIRMADO: descStatusCnab = "Baixa Manual Confirmado";
                        break;
                    case (int)StatusCnabTitulo.PEDIDO_BAIXA: descStatusCnab = "Pedido Baixa";
                        break;
                    case (int)StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA: descStatusCnab = "Confirmado Pedido Baixa";
                        break;
                }
                return descStatusCnab;
            }
        }

        public string hr_cadastro_titulo
        {
            get
            {
                if (dh_cadastro_titulo != null)
                    return String.Format("{0:HH:mm:ss}", ((DateTime)dh_cadastro_titulo).ToLocalTime());
                else
                    return "";
            }
        }

        public string pagador_cnab
        {
            get
            {
                if (this.Pessoa != null)
                    return this.nm_titulo + " - " + this.Pessoa.no_pessoa + (String.IsNullOrEmpty(this.Pessoa.no_aluno) ? "" : this.Pessoa.no_aluno)
                        + (String.IsNullOrEmpty(this.Pessoa.nm_cpf_cgc) ? "" : " - ");
                else
                    return this.nm_titulo + "";
            }
        }

        public bool id_emitido_CNAB
        {
            get
            {
                bool emitidoCNAB = false;
                if (id_status_cnab != (int)StatusCnabTitulo.INICIAL)
                    emitidoCNAB = true;
                return emitidoCNAB;
            }
        }
        public int cd_tipo_titulo
        {
            get
            {
                int descTipo = 0;
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("pt-BR");
                switch (dc_tipo_titulo)
                {
                    case "AA":
                        descTipo = (int)Titulo.TipoTitulo.AA;
                        break;
                    case "AD":
                        descTipo = (int)Titulo.TipoTitulo.AD;
                        break;
                    case "MA":
                        descTipo = (int)Titulo.TipoTitulo.ME;
                        break;
                    case "ME":
                        descTipo = (int)Titulo.TipoTitulo.ME;
                        break;
                    case "MM":
                        descTipo = (int)Titulo.TipoTitulo.ME;
                        break;
                    case "PP":
                        descTipo = (int)Titulo.TipoTitulo.PP;
                        break;
                    case "TA":
                        descTipo = (int)Titulo.TipoTitulo.TA;
                        break;
                    case "TM":
                        descTipo = (int)Titulo.TipoTitulo.TM;
                        break;
                    case "NF":
                        descTipo = (int)Titulo.TipoTitulo.NF;
                        break;
                }
                return descTipo;
            }
        }
        public static Titulo changeValuesTituloEditBaixaFinan(Titulo tituloContext, Titulo tituloView)
        {
            tituloContext.cd_local_movto = tituloView.cd_local_movto;
            tituloContext.cd_plano_conta_tit = tituloView.cd_plano_conta_tit;
            tituloContext.dt_vcto_titulo = tituloView.dt_vcto_titulo;
            tituloContext.pc_juros_titulo = tituloView.pc_juros_titulo;
            tituloContext.pc_multa_titulo = tituloView.pc_multa_titulo;
            tituloContext.pc_taxa_cartao = tituloView.pc_taxa_cartao;
            tituloContext.nm_dias_cartao = tituloView.nm_dias_cartao;
            tituloContext.vl_taxa_cartao = tituloView.vl_taxa_cartao;
            tituloContext.cd_tipo_financeiro = tituloView.cd_tipo_financeiro;
            return tituloContext;
        }

        public static Titulo changeValuesTituloRestoreBaixa(Titulo tituloContext, Titulo tituloView)
        {
            tituloContext.dt_liquidacao_titulo = tituloView.dt_liquidacao_titulo;
            tituloContext.id_status_titulo = tituloView.id_status_titulo;
            tituloContext.vl_saldo_titulo = tituloView.vl_saldo_titulo;
            tituloContext.vl_desconto_titulo = tituloView.vl_desconto_titulo;
            tituloContext.vl_saldo_titulo = tituloView.vl_saldo_titulo;
            tituloContext.vl_liquidacao_titulo = tituloView.vl_liquidacao_titulo;
            tituloContext.vl_multa_titulo = tituloView.vl_multa_titulo;
            tituloContext.vl_juros_titulo = tituloView.vl_juros_titulo;
            tituloContext.vl_multa_liquidada = tituloView.vl_multa_liquidada;
            tituloContext.vl_juros_liquidado = tituloView.vl_juros_liquidado;
            tituloContext.vl_desconto_juros = tituloView.vl_desconto_juros;
            tituloContext.vl_desconto_multa = tituloView.vl_desconto_multa;

            return tituloContext;
        }

        public static Titulo changeValuesTituloEditMovimento(Titulo tituloContext, Titulo tituloView)
        {
            tituloContext.cd_tipo_financeiro = tituloView.cd_tipo_financeiro;
            tituloContext.dc_num_documento_titulo = tituloView.dc_num_documento_titulo;
            tituloContext.dt_emissao_titulo = tituloView.dt_emissao_titulo.Date;
            tituloContext.dt_vcto_titulo = tituloView.dt_vcto_titulo.Date;
            tituloContext.vl_saldo_titulo = tituloView.vl_saldo_titulo;
            tituloContext.vl_titulo = tituloView.vl_titulo;
            tituloContext.cd_local_movto = tituloView.cd_local_movto;
            tituloContext.cd_plano_conta_tit = tituloView.cd_plano_conta_tit;
            tituloContext.nm_titulo = tituloView.nm_titulo;
            tituloContext.pc_taxa_cartao = tituloView.pc_taxa_cartao;
            tituloContext.nm_dias_cartao = tituloView.nm_dias_cartao;
            tituloContext.vl_taxa_cartao = tituloView.vl_taxa_cartao;
            return tituloContext;
        }

        public static Boolean editValuesTituloEditMovimento(Titulo tituloContext, Titulo tituloView)
        {
            return(
            tituloContext.cd_tipo_financeiro != tituloView.cd_tipo_financeiro ||
            tituloContext.dc_num_documento_titulo != tituloView.dc_num_documento_titulo ||
            tituloContext.dt_emissao_titulo.Date != tituloView.dt_emissao_titulo.Date ||
            tituloContext.dt_vcto_titulo.Date != tituloView.dt_vcto_titulo.Date ||
            tituloContext.vl_saldo_titulo != tituloView.vl_saldo_titulo ||
            tituloContext.vl_titulo != tituloView.vl_titulo ||
            tituloContext.cd_local_movto != tituloView.cd_local_movto ||
            tituloContext.nm_titulo != tituloView.nm_titulo ||
            tituloContext.cd_pessoa_titulo != tituloView.cd_pessoa_titulo ||
            tituloContext.cd_pessoa_responsavel != tituloView.cd_pessoa_responsavel ||
            tituloContext.vl_material_titulo != tituloView.vl_material_titulo ||
            tituloContext.vl_desconto_contrato != tituloView.vl_desconto_contrato
            );
        }

        public static Titulo changeValuesTituloEditMatricula(Titulo tituloContext, Titulo tituloView)
        {
            tituloContext.cd_pessoa_responsavel = tituloView.cd_pessoa_responsavel;
            tituloContext.cd_tipo_financeiro = tituloView.cd_tipo_financeiro;
            tituloContext.dc_num_documento_titulo = tituloView.dc_num_documento_titulo;
            tituloContext.dt_emissao_titulo = tituloView.dt_emissao_titulo.Date;
            tituloContext.dt_vcto_titulo = tituloView.dt_vcto_titulo.Date;
            tituloContext.vl_saldo_titulo = tituloView.vl_saldo_titulo;
            tituloContext.vl_titulo = tituloView.vl_titulo;
            tituloContext.cd_local_movto = tituloView.cd_local_movto;
            tituloContext.cd_plano_conta_tit = tituloView.cd_plano_conta_tit;
            tituloContext.nm_titulo = tituloView.nm_titulo;
            tituloContext.vl_material_titulo = tituloView.vl_material_titulo;
            tituloContext.vl_desconto_contrato = tituloView.vl_desconto_contrato;
            tituloContext.dc_tipo_titulo = tituloView.dc_tipo_titulo;
            tituloContext.pc_taxa_cartao = tituloView.pc_taxa_cartao;
            tituloContext.nm_dias_cartao = tituloView.nm_dias_cartao;
            tituloContext.vl_taxa_cartao = tituloView.vl_taxa_cartao;

            return tituloContext;
        }

        public static string convertTipoTitulo(byte tipo)
        {
            string descTipo = "";
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("pt-BR");
            switch (tipo)
            {
                case (int)TipoTitulo.AA: descTipo = Titulo.TipoTitulo.AA.ToString();
                    break;
                case (int)TipoTitulo.AD: descTipo = Titulo.TipoTitulo.AD.ToString();
                    break;
                case (int)TipoTitulo.MA: descTipo = Titulo.TipoTitulo.MA.ToString();
                    break;
                case (int)TipoTitulo.ME: descTipo = Titulo.TipoTitulo.ME.ToString();
                    break;
                case (int)TipoTitulo.MM: descTipo = Titulo.TipoTitulo.MM.ToString();
                    break;
                case (int)TipoTitulo.PP: descTipo = Titulo.TipoTitulo.PP.ToString();
                    break;
                case (int)TipoTitulo.TA: descTipo = Titulo.TipoTitulo.TA.ToString();
                    break;
                case (int)TipoTitulo.TM: descTipo = Titulo.TipoTitulo.TM.ToString();
                    break;
                case (int)TipoTitulo.NF: descTipo = Titulo.TipoTitulo.NF.ToString();
                    break;
            }
            return descTipo;
        }

        public static string concatenarVencimentosTitulo(List<Titulo> titulos)
        {
            string retorno = "";
            if (titulos != null && titulos.Count() > 0)
                foreach (Titulo t in titulos)
                    retorno += ", " + t.dt_vcto;
            if (retorno.Length >= 1)
                retorno = retorno.Substring(2, retorno.Length - 2);
            return retorno;
        }

        public static List<Titulo> revisarSaldoTituloBolsaContrato(List<Titulo> titulos, bool aplicar_bolsa, double pc_bolsa, bool aditivoBolsa, AditamentoBolsa adtBolsa = null)
        {
            if (!aplicar_bolsa)
            {
                List<Titulo> titulosComBaixaBolsa = new List<Titulo>();
                if (aditivoBolsa && (adtBolsa != null && adtBolsa.dt_comunicado_bolsa.HasValue))
                    titulosComBaixaBolsa = titulos.Where(x => !x.possuiBaixa 
                      && x.possuiBaixaBolsa && x.id_emitido_CNAB == false
                      && x.dt_vcto_titulo >= adtBolsa.dt_comunicado_bolsa.Value.Date).ToList();
                else
                    titulosComBaixaBolsa = titulos.Where(x => !x.possuiBaixa
                      && x.possuiBaixaBolsa && x.id_emitido_CNAB == false).ToList();

                foreach (Titulo t in titulosComBaixaBolsa)
                {
                    if (t.id_status_titulo == (int)Titulo.StatusTitulo.FECHADO)
                        t.id_status_titulo = (int)Titulo.StatusTitulo.ABERTO;

                    t.vl_saldo_titulo = t.vl_titulo;
                }
            }
            else
            {
                int[] statusCnabTitulo = new int[] { (int)Titulo.StatusCnabTitulo.INICIAL, (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA };
                List<Titulo> titulosME = new List<Titulo>();
                //if (!aditivoBolsa)
                //    titulosME = titulos.Where(x => x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                //                                                      statusCnabTitulo.Contains(x.id_status_cnab) &&
                //                                                      x.vl_titulo == x.vl_saldo_titulo && x.dc_tipo_titulo == "ME").ToList();
                //else

                if (aditivoBolsa && (adtBolsa != null && adtBolsa.dt_comunicado_bolsa.HasValue))
                {
                    titulosME = titulos.Where(x => x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                                                  x.dt_vcto_titulo >= adtBolsa.dt_comunicado_bolsa.Value.Date &&
                                                                  statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                  x.vl_titulo == x.vl_saldo_titulo && (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "AD")).ToList();
                }
                else
                {
                    titulosME = titulos.Where(x => x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                                                  statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                  x.vl_titulo == x.vl_saldo_titulo && (x.dc_tipo_titulo == "ME" || x.dc_tipo_titulo == "AD")).ToList();
                }

                if (titulosME != null && titulosME.Count > 0)
                    if (pc_bolsa >= 0)
                        foreach (Titulo t in titulosME)
                        {
                            t.vl_saldo_titulo = t.vl_titulo - decimal.Round((t.vl_titulo - t.vl_material_titulo) * (decimal)pc_bolsa / 100, 2);
                            if (aditivoBolsa)
                                t.tituloEdit = true;
                            t.possuiBaixaBolsa = pc_bolsa > 0;
                        }
            }
            return titulos;
        }

        public static void revisarValorESaldoTituloContratoMaterial(ref List<Titulo> titulos, int[] statusCnabTitulo, List<Titulo> titulosComMaterial, bool aplicar_material)
        {
            List<Titulo> titulosME = titulos.Where(x => x.vl_material_titulo > 0 &&
                                                                 x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                                                 statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                 x.vl_titulo == x.vl_saldo_titulo).ToList();
            if (aplicar_material)
            {
                if (titulosComMaterial != null && titulosComMaterial.Count() > 0)
                    foreach (var t in titulosComMaterial)
                    {
                        Titulo titulo = titulos.Where(x => x.nm_titulo == t.nm_titulo && x.nm_parcela_titulo == t.nm_parcela_titulo &&
                                                           x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA").FirstOrDefault();
                        if (titulo != null)
                        {
                            titulo.vl_saldo_titulo += (decimal)t.vl_material_titulo;
                            titulo.vl_titulo += (decimal)t.vl_material_titulo;
                            titulo.vl_material_titulo = (decimal)t.vl_material_titulo;
                            titulo.possuiBaixaBolsa = true;
                        }
                    }
            }
            else
                foreach (Titulo t in titulosME)
                {
                    t.vl_saldo_titulo -= (decimal)t.vl_material_titulo;
                    t.vl_titulo -= (decimal)t.vl_material_titulo;
                }
        }

        public static void aplicarDescontosTituloAditamento(Contrato contrato, List<Titulo> titulosAbertos, bool id_somar_descontos_financeiros, List<DescontoContrato> descontosAditamento,
            bool voltarDescontos, decimal valorAnteriorCalc)
        {
            List<Contrato.TituloDescontoParcela> titulosDescontos = new List<Contrato.TituloDescontoParcela>();
            foreach (var titulo in titulosAbertos)
                titulosDescontos.Add(new Contrato.TituloDescontoParcela
                {
                    dc_tipo_titulo = titulo.dc_tipo_titulo,
                    nm_parcela_titulo = titulo.nm_parcela_titulo.Value,
                    vl_parcela = titulo.vl_titulo,
                    vl_parcela_desconto = titulo.vl_titulo,
                    vl_desconto_separado = 0,
                    vl_total_desconto_aplicado = 0,
                    pc_total_desconto_aplicado = 0
                });

            foreach (var itensDesc in descontosAditamento.Where(x => x.id_desconto_ativo && x.id_incide_baixa == false))
            {
                var lista = new List<Contrato.TituloDescontoParcela>();
                //Quando informado parcela inicial e final, é selecionadas as parcelas no intervalo.
                if (itensDesc.nm_parcela_ini > 0 && itensDesc.nm_parcela_fim > 0)
                    lista = titulosDescontos.Where(x => x.nm_parcela_titulo >= itensDesc.nm_parcela_ini && x.nm_parcela_titulo <= itensDesc.nm_parcela_fim).ToList();
                //Quando informado parcela inicial e não final, é selecionadas as parcelas apartir da parcela inicial
                if (itensDesc.nm_parcela_ini > 0 && itensDesc.nm_parcela_fim <= 0)
                    lista = titulosDescontos.Where(x => x.nm_parcela_titulo >= itensDesc.nm_parcela_ini).ToList();
                //Quando informado parcela finçal e não a inicial, é selecionadas as parcelas com a numerção menor e igual a parcela final.
                if (itensDesc.nm_parcela_ini <= 0 && itensDesc.nm_parcela_fim > 0)
                    lista = titulosDescontos.Where(x => x.nm_parcela_titulo <= itensDesc.nm_parcela_fim).ToList();
                //Quando não informado a parcela inicial e a final, pegara todas as parcelas.
                if (itensDesc.nm_parcela_ini <= 0 && itensDesc.nm_parcela_fim <= 0)
                    lista = titulosDescontos;
                foreach (var titulo in lista)
                {
                    titulo.aplicar_percentual_sem_desconto = itensDesc.aplicar_percentual_sem_desconto;
                    decimal pc_desc = itensDesc.pc_desconto_contrato > 0 ? itensDesc.pc_desconto_contrato : itensDesc.pc_desconto > 0 ? (decimal)itensDesc.pc_desconto : 0;
                    if (pc_desc > 0)
                        titulo.pc_total_desconto_aplicado = DescontoContrato.calcularValoresDescontoPorPecentualOrValor(pc_desc,
                            0, titulo.pc_total_desconto_aplicado, valorAnteriorCalc, id_somar_descontos_financeiros);

                    if (itensDesc.vl_desconto_contrato > 0)
                        titulo.pc_total_desconto_aplicado = DescontoContrato.calcularValoresDescontoPorPecentualOrValor(0, decimal.Parse(itensDesc.vl_desconto),
                            titulo.pc_total_desconto_aplicado, titulo.vl_parcela, id_somar_descontos_financeiros);
                }
            }
            foreach (var tituloG in titulosDescontos)
            {
                Titulo titulo = titulosAbertos.Where(x => x.nm_parcela_titulo == tituloG.nm_parcela_titulo && x.dc_tipo_titulo == tituloG.dc_tipo_titulo).FirstOrDefault();
                if (titulo != null)
                {
                    if (!voltarDescontos)
                    {

                        var vl_calc = decimal.Round(titulo.vl_titulo * (tituloG.pc_total_desconto_aplicado / 100) + tituloG.vl_total_desconto_aplicado, 2);
                        if (!tituloG.aplicar_percentual_sem_desconto)
                        {
                            titulo.vl_titulo = titulo.vl_titulo - vl_calc;
                            titulo.vl_desconto_contrato = vl_calc;
                        }
                        else
                            titulo.vl_titulo = vl_calc;

                    }
                    else
                    {
                        if (titulo.vl_desconto_contrato > 0)
                        {
                            //titulo.vl_titulo = titulo.vl_titulo - titulo.vl_material_titulo + titulo.vl_desconto_contrato;
                            titulo.vl_titulo = titulo.vl_titulo + titulo.vl_desconto_contrato;
                            titulo.vl_desconto_contrato = 0;
                        }
                        else
                        {
                            var vl_titulo_volta = decimal.Round((titulo.vl_titulo / ((100 - tituloG.pc_total_desconto_aplicado) / 100)), 2);
                            titulo.vl_desconto_contrato -= vl_titulo_volta - titulo.vl_titulo;
                            if (titulo.vl_desconto_contrato < 0)
                                titulo.vl_desconto_contrato = 0;
                            titulo.vl_titulo = vl_titulo_volta;
                        }
                        //titulo.vl_desconto_separado = titulo.vl_titulo - tituloG.vl_parcela_desconto;
                    }
                }
                //valorDescontoTotalAplicadoTitulos += vl_desc_calc;
            }
        }

        public static List<Contrato.TituloDescontoParcela> gerarDescontosTituloAditamento(Contrato contrato, List<Titulo> titulosAbertos, bool id_somar_descontos_financeiros, bool voltarDescontos)
        {
            List<Contrato.TituloDescontoParcela> titulosDescontos = new List<Contrato.TituloDescontoParcela>();
            List<DescontoContrato> descontosAditamento = new List<DescontoContrato>();
            foreach (var titulo in titulosAbertos)
                titulosDescontos.Add(new Contrato.TituloDescontoParcela
                {
                    nm_parcela_titulo = titulo.nm_parcela_titulo.Value,
                    vl_parcela = titulo.vl_titulo,
                    vl_parcela_desconto = titulo.vl_titulo,
                    vl_desconto_separado = 0,
                    vl_total_desconto_aplicado = 0,
                    pc_total_desconto_aplicado = 0
                });

            foreach (var itensDesc in descontosAditamento)
            {
                var lista = new List<Contrato.TituloDescontoParcela>();
                //Quando informado parcela inicial e final, é selecionadas as parcelas no intervalo.
                if (itensDesc.nm_parcela_ini > 0 && itensDesc.nm_parcela_fim > 0)
                    lista = titulosDescontos.Where(x => x.nm_parcela_titulo >= itensDesc.nm_parcela_ini && x.nm_parcela_titulo <= itensDesc.nm_parcela_fim).ToList();
                //Quando informado parcela inicial e não final, é selecionadas as parcelas apartir da parcela inicial
                if (itensDesc.nm_parcela_ini > 0 && itensDesc.nm_parcela_fim <= 0)
                    lista = titulosDescontos.Where(x => x.nm_parcela_titulo >= itensDesc.nm_parcela_ini).ToList();
                //Quando informado parcela finçal e não a inicial, é selecionadas as parcelas com a numerção menor e igual a parcela final.
                if (itensDesc.nm_parcela_ini <= 0 && itensDesc.nm_parcela_fim > 0)
                    lista = titulosDescontos.Where(x => x.nm_parcela_titulo <= itensDesc.nm_parcela_fim).ToList();
                //Quando não informado a parcela inicial e a final, pegara todas as parcelas.
                if (itensDesc.nm_parcela_ini <= 0 && itensDesc.nm_parcela_fim <= 0)
                    lista = titulosDescontos;
                foreach (var titulo in lista)
                {
                    if (itensDesc.pc_desconto > 0)
                        titulo.pc_total_desconto_aplicado = DescontoContrato.calcularValoresDescontoPorPecentualOrValor(decimal.Parse(itensDesc.pc_desc),
                            0, titulo.pc_total_desconto_aplicado, 0, id_somar_descontos_financeiros);

                    if (itensDesc.vl_desconto_contrato > 0)
                        titulo.pc_total_desconto_aplicado = DescontoContrato.calcularValoresDescontoPorPecentualOrValor(0, decimal.Parse(itensDesc.vl_desconto),
                            titulo.pc_total_desconto_aplicado, 0, id_somar_descontos_financeiros);
                }
            }
            foreach (var tituloG in titulosDescontos)
            {
                if (!voltarDescontos)
                {
                    var vl_desc_calc = decimal.Round(tituloG.vl_parcela_desconto * (tituloG.pc_total_desconto_aplicado / 100) + tituloG.vl_total_desconto_aplicado, 2);
                    tituloG.vl_parcela_desconto = decimal.Round(tituloG.vl_parcela_desconto - vl_desc_calc, 2);
                    tituloG.vl_desconto_separado = vl_desc_calc;
                }
                else
                {
                    tituloG.vl_parcela_desconto = decimal.Round((tituloG.vl_parcela_desconto / ((100 - tituloG.pc_total_desconto_aplicado) / 100)), 2);
                    tituloG.vl_desconto_separado = decimal.Round(tituloG.vl_parcela - tituloG.vl_parcela_desconto, 2);
                }
                //valorDescontoTotalAplicadoTitulos += vl_desc_calc;
            }
            return titulosDescontos;
        }
    }

}
