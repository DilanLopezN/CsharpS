using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class Contrato {
        public enum TipoMatricula
        {
            MATRICULA = 1,
            REMATRICULA = 2
        }

        public enum TipoCKMatricula
        {
            NORMAL = 0,
            MULTIPLA = 1,
            PROFISSIONALIZANTE = 2,
            INFORMATICA = 3
        }

        public bool gerarTitulos { get; set; }
        public ICollection<Titulo> titulos { get; set; }
        public List<TituloDescontoParcela> titulosDescontoParcela { get; set; }
        public List<TituloTaxaParcela> titulosTaxaParcela { get; set; }
        public int cd_pessoa_aluno { get; set; }
        public string no_responsavel { get; set; }
        public string e_mail_responsavel { get; set; }
        public string celular_responsavel { get; set; }
        public string no_pessoa_cpf { get; set; }
        public Hashtable hashtableHorarios { get; set; }
        public double qtd_minutos_turma { get; set; }
        public List<Produto> produtos { get; set; }
        public List<Regime> regimes { get; set; }
        public List<Duracao> duracoes { get; set; }
        public List<NomeContrato> nomesContrato { get; set; }
        public List<TipoLiquidacao> tipoLiquidacoes { get; set; }
        public List<Banco> bancos { get; set; }
        public int cd_local_movto { get; set; }
        public List<LocalMovto> localMovto { get; set; }
        public List<AnoEscolar> anosEscolares { get; set; }
        public List<MotivoBolsa> motivosBolsa { get; set; }
        public double? perc_descontoParametros_maximo { get; set; }
        public Nullable<int> cd_situacao_aluno_turma { get; set; }
        public bool alterouTitulos { get; set; }
        public string no_produto { get; set; }
        public string no_curso { get; set; }
        public string dc_ano_escolar { get; set; }
        public decimal? valorSaldoMatricula { get; set; }
        public int qtdTitulosAbertos { get; set; }
        public int? id_tipo_aditamento { get; set; }
        public Aditamento aditamentoMaxData { get; set; }
        public DateTime? dt_fim_turma { get; set; }
        public DateTime? dt_nasc_responsavel { get; set; }
        public int? cd_turma_atual { get; set; }
        public int? cd_cidade_aluno { get; set; }
        public int? cd_uf_aluno { get; set; }
        public PessoaFisicaSGF pessoaFisicaResponsavel {get; set;}
        public PessoaJuridicaSGF pessoajuridicaResponsavel { get; set; }
        private string _no_aluno;
        public string dc_tipo_financeiro_taxa { get; set; }
        public string dc_regime_turma { get; set; }
        public int cd_politica_comercial { get; set; }
        public string nm_arquivo_digitalizado_temporario { get; set; }
        public bool alterou_responsavel { get; set; }
        public bool alterou_dt_vcto { get; set; }
        public bool? id_venda_futura { get; set; }
        public List<NotasVendaMaterialUI> notas_material_didatico { get; set; }
        public string msgProcedureGerarNota { get; set; }
        public string no_pessoa {
            get {
                if (this.Aluno != null && this.Aluno.AlunoPessoaFisica != null && this.Aluno.AlunoPessoaFisica.no_pessoa != null)
                    return this.Aluno.AlunoPessoaFisica.no_pessoa;
                return _no_aluno;
            }
            set {
                _no_aluno = value;
            }
        }
        public string desc_descontos_contrato { get; set; }

        private string _no_turma;
        public string no_turma {
            get {
                //if (this.AlunoTurma != null && this.AlunoTurma.Where(a => a.Turma.no_turma != null).Any())
                //    return this.AlunoTurma.Select(a => a.Turma.no_turma);
                return _no_turma;
            }
            set {
                _no_turma = value;
            }
        }

        private string _tipo_matricula;
        public string dc_tipo_matricula
        {
            get
            {
                return _tipo_matricula;
            }
            set
            {
                _tipo_matricula = value;
            }
        }

        private string _situacaoTurma;
        public string dc_situacao_turma
        {
            get {
                return _situacaoTurma;
            }
            set {
                _situacaoTurma = value;
            }
         }

        public string dtMatriculaContrato
        {
            get
            {
                if (dt_matricula_contrato != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_matricula_contrato.Value);
                else
                    return String.Empty;
            }
        }

        public string dtInicialContrato
        {
            get
            {
                if (dt_inicial_contrato != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_inicial_contrato.Value);//ToLocalTime());
                else
                    return String.Empty;
            }
        }

        public string dtFinalContrato
        {
            get
            {
                if (dt_final_contrato != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_final_contrato.Value);
                else
                    return String.Empty;
            }
        }

        public string dtNascResponsavel
        {
            get
            {
                if (dt_nasc_responsavel != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_nasc_responsavel.Value);
                else
                    return String.Empty;
            }
        }
        
        public string vlCursoContrato
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_curso_contrato);
            }
        }

        public string vlMatriculaContrato
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_matricula_contrato);
            }
        }
        public string vlParcelaContrato
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_parcela_contrato);
            }
        }
        public string vlDescontoContrato
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_desconto_contrato);
            }
        }
        public string vlDividaContrato
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_divida_contrato);
            }
        }
        public string vlDescPrimeiraParcela
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_desc_primeira_parcela);
            }
        }

        public string vlParcelaLiquida
        {
            get
            {
                decimal retorno =  0;
                if(vl_liquido_contrato > 0 && nm_parcelas_mensalidade > 0)
                    retorno = decimal.Round(vl_liquido_contrato / nm_parcelas_mensalidade, 2);
                return string.Format("{0:#,0.00}", retorno);
            }
        }

        

        public decimal vl_parcela_contrato_sem_desconto { get; set; }

        public string vlParcelaContratoSemDesconto {
            get {
                return string.Format("{0:#,0.00}", this.vl_parcela_contrato_sem_desconto);
            }
        }

        public string dtaCorrenteExtenso
        {
            get
            {
                var retorno = "";
                    CultureInfo culture = new CultureInfo("pt-BR");
                    DateTimeFormatInfo dtfi = culture.DateTimeFormat;
                    DateTime dtaCorrente = DateTime.Now;
                    retorno = dtaCorrente.Day + " de " + culture.TextInfo.ToTitleCase(dtfi.GetMonthName(dtaCorrente.Month)) + " de " + dtaCorrente.Year;
                return retorno;
            }
        }

        public string valor_material
        {
            get
            {
                if (this.vl_material_contrato == 0)
                    return "";
                return string.Format("{0:#,0.00}", this.vl_material_contrato);
            }
        }
        
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_curso", "Código"));
                retorno.Add(new DefinicaoRelatorio("nm_contrato", "Contrato", AlinhamentoColuna.Center, "0.9000in"));
                if (nm_contrato != nm_matricula_contrato)
                    retorno.Add(new DefinicaoRelatorio("nm_matricula_contrato", "Matrícula", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("no_pessoa", "Aluno", AlinhamentoColuna.Left, "1.7000in"));
                retorno.Add(new DefinicaoRelatorio("no_turma", "Turma", AlinhamentoColuna.Left, "1.7000in"));
                retorno.Add(new DefinicaoRelatorio("dtMatriculaContrato", "Data Matrícula", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("dtInicialContrato", "Data Inicial", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("dtFinalContrato", "Data Final", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("dc_tipo_matricula", "Tipo", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("desc_descontos_contrato", "Tipo de Desconto", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("dc_situacao_turma", "Situação", AlinhamentoColuna.Center, "0.9000in"));

                return retorno;
            }
        }

        //public string desc_descontos_contrato { get; set; }
        //{
        //    get
        //    {
        //        string retorno = "";
        //        if (DescontoContrato != null && DescontoContrato.Count() > 0)
        //            foreach (DescontoContrato d in DescontoContrato)
        //                if (retorno == "")
        //                    retorno += d.dc_tipo_desconto;
        //                else
        //                    retorno += ", " + d.dc_tipo_desconto;
        //        return retorno;
        //    }
        //}

        public static bool testchangeValuesContrato(Contrato contratoContext, Contrato contratoView)
        {
            return(
            contratoContext.dt_inicial_contrato != contratoView.dt_inicial_contrato ||
            contratoContext.dt_final_contrato != contratoView.dt_final_contrato ||
            contratoContext.dt_matricula_contrato != contratoView.dt_matricula_contrato ||
            contratoContext.cd_produto_atual != contratoView.cd_produto_atual ||
            contratoContext.cd_curso_atual != contratoView.cd_curso_atual ||
            contratoContext.cd_duracao_atual != contratoView.cd_duracao_atual ||
            contratoContext.cd_regime_atual != contratoView.cd_regime_atual ||
            contratoContext.id_tipo_contrato != contratoView.id_tipo_contrato);
        }
        
        public static void changeValuesContrato(Contrato contratoContext, Contrato contratoView)
        {
            contratoContext.cd_aluno = contratoView.cd_aluno;
            contratoContext.nm_contrato = contratoView.nm_contrato;
            contratoContext.dc_serie_contrato = contratoView.dc_serie_contrato;
            contratoContext.nm_matricula_contrato = contratoView.nm_matricula_contrato;
            contratoContext.dt_inicial_contrato = contratoView.dt_inicial_contrato;
            contratoContext.dt_final_contrato = contratoView.dt_final_contrato;
            contratoContext.dt_matricula_contrato = contratoView.dt_matricula_contrato;
            contratoContext.id_ajuste_manual = contratoView.id_ajuste_manual;
            contratoContext.id_contrato_aula = contratoView.id_contrato_aula;
            contratoContext.cd_plano_conta = contratoView.cd_plano_conta;
            contratoContext.cd_tipo_financeiro = contratoView.cd_tipo_financeiro;
            contratoContext.cd_pessoa_responsavel = contratoView.cd_pessoa_responsavel;
            contratoContext.pc_responsavel_contrato = contratoView.pc_responsavel_contrato;
            contratoContext.cd_produto_atual = contratoView.cd_produto_atual;
            contratoContext.cd_curso_atual = contratoView.cd_curso_atual;
            contratoContext.cd_regime_atual = contratoView.cd_regime_atual;
            contratoContext.cd_duracao_atual = contratoView.cd_duracao_atual;
            contratoContext.nm_dia_vcto = contratoView.nm_dia_vcto;
            contratoContext.nm_mes_vcto = contratoView.nm_mes_vcto;
            contratoContext.nm_ano_vcto = contratoView.nm_ano_vcto;
            contratoContext.nm_parcelas_mensalidade = contratoView.nm_parcelas_mensalidade;
            contratoContext.vl_curso_contrato = contratoView.vl_curso_contrato;
            contratoContext.vl_matricula_contrato = contratoView.vl_matricula_contrato;
            contratoContext.vl_parcela_contrato = contratoView.vl_parcela_contrato;
            contratoContext.vl_desconto_contrato = contratoView.vl_desconto_contrato;
            contratoContext.pc_desconto_contrato = contratoView.pc_desconto_contrato;
            contratoContext.vl_divida_contrato = contratoView.vl_divida_contrato;
            contratoContext.id_divida_primeira_parcela = contratoView.id_divida_primeira_parcela;
            contratoContext.vl_desc_primeira_parcela = contratoView.vl_desc_primeira_parcela;
            contratoContext.id_nf_servico = contratoView.id_nf_servico;
            contratoContext.id_tipo_matricula = contratoView.id_tipo_matricula;
            contratoContext.id_renegociacao = contratoView.id_renegociacao;
            contratoContext.id_transferencia = contratoView.id_transferencia;
            contratoContext.id_liberar_certificado = contratoView.id_liberar_certificado;
            contratoContext.id_retorno = contratoView.id_retorno;
            contratoContext.id_venda_pacote = contratoView.id_venda_pacote;
            contratoContext.cd_mala_direta = contratoView.cd_mala_direta;
            contratoContext.pc_desconto_bolsa = contratoView.pc_desconto_bolsa;
            contratoContext.vl_pre_matricula = contratoView.vl_pre_matricula;
            contratoContext.cd_ano_escolar = contratoView.cd_ano_escolar;
            contratoContext.vl_liquido_contrato = contratoView.vl_liquido_contrato;
            contratoContext.id_tipo_contrato = contratoView.id_tipo_contrato;
            contratoContext.nm_mes_curso_inicial = contratoView.nm_mes_curso_inicial;
            contratoContext.nm_ano_curso_inicial = contratoView.nm_ano_curso_inicial;
            contratoContext.nm_mes_curso_final = contratoView.nm_mes_curso_final;
            contratoContext.nm_ano_curso_final = contratoView.nm_ano_curso_final;
            if (contratoContext.nm_arquivo_digitalizado != contratoView.nm_arquivo_digitalizado)
            {
                contratoContext.nm_arquivo_digitalizado = contratoView.nm_arquivo_digitalizado;
            }
            contratoContext.vl_material_contrato = contratoView.vl_material_contrato;
            contratoContext.id_valor_incluso = contratoView.id_valor_incluso;
            contratoContext.id_incorporar_valor_material = contratoView.id_incorporar_valor_material;
            contratoContext.nm_parcelas_material = contratoView.nm_parcelas_material;
            contratoContext.vl_parcela_material = contratoView.vl_parcela_material;
            contratoContext.vl_parcela_liq_material = contratoView.vl_parcela_liq_material;
            contratoContext.pc_bolsa_material = contratoView.pc_bolsa_material;
                
        }


        public static void changeValuesDocumentoDigitalizado(Contrato contratoContext, DocumentoDigitalizadoEditUI contratoView)
        {
            if (!String.IsNullOrEmpty(contratoView.nm_arquivo_digitalizado) && contratoContext.nm_arquivo_digitalizado != contratoView.nm_arquivo_digitalizado)
            {
                contratoContext.nm_arquivo_digitalizado = contratoView.nm_arquivo_digitalizado;
            }
        }

        public static void changeValuesPacoteCertificado(Contrato contratoContext, PacoteCertificadoUI contratoView)
        {
            contratoContext.id_venda_pacote = contratoView.id_venda_pacote;
            contratoContext.id_liberar_certificado = contratoView.id_liberar_certificado;
        }

        public static DateTime gerarDataVencimentoMatricula(int dia, int mes, int ano)
        {
            DateTime data_vencimento = new DateTime();
             bool encontrou_dia = false;
             for (int k = 0; k < 3 && !encontrou_dia; k++)
             {
                 try
                 {
                     data_vencimento = new DateTime(ano, mes, dia - k);
                     encontrou_dia = true;
                 }
                 catch (System.ArgumentOutOfRangeException)
                 {
                     encontrou_dia = false;
                 }
             }
             return data_vencimento;
        }

        public class TituloDescontoParcela
        {
            public byte nm_parcela_titulo { get; set; }
            public int nm_parcela_ini_desconto { get; set; }
            public int nm_parcela_fim_desconto { get; set; }
            public decimal vl_parcela { get; set; }
            public decimal vl_parcela_desconto { get; set; }
            public decimal vl_desconto_separado { get; set; }
            public decimal vl_total_desconto_aplicado { get; set; }
            public decimal pc_total_desconto_aplicado { get; set; }
            public string dc_tipo_titulo { get; set; }
            public bool aplicar_percentual_sem_desconto { get; set; }
            public static Titulo buscarSetarValorParcelaDescontoTitulo(Titulo titulo, List<TituloDescontoParcela> titulosDescontoParcela)
            {
                if (titulo.dc_tipo_titulo == "ME")
                {
                    TituloDescontoParcela tituloParcela = titulosDescontoParcela.Where(x => x.nm_parcela_titulo == titulo.nm_parcela_titulo).FirstOrDefault();
                    if (tituloParcela != null && tituloParcela.pc_total_desconto_aplicado > 0)
                    {
                        titulo.vl_titulo = tituloParcela.vl_parcela_desconto;
                        titulo.vl_saldo_titulo = tituloParcela.vl_parcela_desconto;
                        titulo.vl_desconto_contrato = tituloParcela.vl_desconto_separado;
                    }
                }
                return titulo;
            }
        }

        public class TituloTaxaParcela
        {
            public byte nm_parcela_titulo { get; set; }
            public string dc_tipo_titulo { get; set; }
            public decimal vl_taxa_cartao { get; set; }
            public double pc_taxa_cartao { get; set; }

        }
    }
    }
