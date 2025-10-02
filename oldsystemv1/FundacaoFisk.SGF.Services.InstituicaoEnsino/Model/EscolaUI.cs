using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Services.InstituicaoEnsino.Model
{
    public class EscolaUI : TO
    {
        public bool id_baixa_automatica_cheque;

        //Campos obrigatórios para ordenar na grade
        public int cd_pessoa { get; set; }
        public string no_pessoa { get; set; }
        public String dc_fone_email { get; set; }
        public bool id_pessoa_ativa { get; set; }
        public bool id_empresa_propria { get; set; }
        public System.DateTime dt_cadastramento { get; set; }
        public String dc_num_cgc { get; set; }
        public String dc_reduzido_pessoa { get; set; }
        public Nullable<int> cd_empresa { get; set; }

        //Campos para retornar no select
        public double? pc_juros_dia { get; set; }
        public double? pc_multa { get; set; }
        public double? pc_taxa_dia_biblioteca { get; set; }
        public int? cd_operadora { get; set; }
        public Nullable<int> nm_cliente_integracao { get; set; }
        public Nullable<int> nm_escola_integracao { get; set; }
        public Nullable<System.TimeSpan> hr_inicial { get; set; }
        public Nullable<System.TimeSpan> hr_final { get; set; }
        public string ext_img_pessoa { get; set; }
        public decimal nm_investimento { get; set; }
        public decimal nm_patrimonio { get; set; }

        public String dc_plano_mat { get; set; }
        public String dc_plano_taxa { get; set; }
        public String dc_plano_juros { get; set; }
        public String dc_plano_multa { get; set; }
        public String dc_plano_desconto { get; set; }
        public String dc_plano_taxa_bco { get; set; }
        public String dc_plano_material { get; set; }
        public List<LocalMovto> localMovto { get; set; }

        public Nullable<System.DateTime> dt_inicio { get; set; }
        public Nullable<System.DateTime> dt_abertura { get; set; }

        public String dc_item_taxa_mat { get; set; }
        public String dc_item_mensalidade { get; set; }
        public String dc_item_biblioteca { get; set; }
        public String dc_tpnf_biblioteca { get; set; }
        public String dc_tpnf_taxa_mensalidade { get; set; }
        public String dc_tpnf_material { get; set; }
        public String dc_tpnf_material_saida { get; set; }
        public String dc_pol_comercial { get; set; }

        public Nullable<byte> nm_dia_gerar_nfs { get; set; }
        public string nome_temp_assinatura_certificado { get; set; }
        public string nome_assinatura_certificado_anterior { get; set; }
        public string nome_assinatura_certificado { get; set; }
        public Nullable<int> cd_empresa_coligada { get; set; }
        public string dc_item_servico { get; set; }
        public string dc_tp_nf_servico { get; set; }
        public string dc_plano_contas_servico { get; set; }
        public string dc_pol_comercial_servico { get; set; }
        public Nullable<byte> nm_dia_gerar_nf_servico { get; set; }
        public bool id_empresa_internacional { get; set; }



        public IEnumerable<GrupoEstoque> gruposEstoques { get; set; }

        public string dtaInicio {
            get {
                if(dt_inicio.HasValue)
                    return String.Format("{0:dd/MM/yyyy}", dt_inicio);
                else
                    return String.Empty;
            }
        }

        public string dtaAbertura {
            get {
                if(dt_abertura.HasValue)
                    return String.Format("{0:dd/MM/yyyy}", dt_abertura);
                else
                    return String.Empty;
            }
        }

        public Parametro parametro { get; set; }      
        public PessoaJuridicaUI pessoaJuridica { get; set; }
        public List<EmpresaValorServico> empresaValorServico { get; set; }
        public int? id_empresa_valor_servico { get; set; }

   
        public static EscolaUI fromEscolaUI(EscolaUI escola, string telefone, Parametro parametro, double? juros, double? multa, double? taxaBiblioteca)
        {
            EscolaUI escolaUI = new EscolaUI
            {
                cd_pessoa = escola.cd_pessoa,
                no_pessoa = escola.no_pessoa,
                dt_cadastramento = escola.dt_cadastramento,
                dc_fone_email = telefone,
                dc_num_cgc = escola.dc_num_cgc,
                id_pessoa_ativa = escola.id_pessoa_ativa,
                parametro = parametro,
                pc_juros_dia = juros,
                pc_multa = multa,
                pc_taxa_dia_biblioteca = taxaBiblioteca,
                hr_inicial = escola.hr_inicial,
                hr_final = escola.hr_final,
                ext_img_pessoa = escola.ext_img_pessoa,
                dc_reduzido_pessoa = escola.dc_reduzido_pessoa,
                dt_abertura = escola.dt_abertura,
                dt_inicio = escola.dt_inicio,
                cd_empresa = escola.cd_empresa,
                nm_cliente_integracao = escola.nm_cliente_integracao,
                nm_escola_integracao = escola.nm_escola_integracao,                
                dc_plano_taxa = escola.dc_plano_taxa,
                dc_plano_mat = escola.dc_plano_mat,
                dc_plano_desconto = escola.dc_plano_desconto,
                dc_plano_taxa_bco = escola.dc_plano_taxa_bco,
                dc_plano_juros = escola.dc_plano_juros,
                dc_plano_multa = escola.dc_plano_multa,
                nm_patrimonio = escola.nm_patrimonio,
                nm_investimento = escola.nm_investimento
            };
            return escolaUI;
        }

        // Métodos auxiliares para formatação
        public string escola_ativa
        {
            get
            {
                return this.id_pessoa_ativa ? "Sim" : "Não";
            }
        }

        public string juros
        {
            get
            {
                if (this.pc_juros_dia == null)
                    return "";
                return string.Format("{0,00}", this.pc_juros_dia);
            }
        }
        public string multa
        {
            get
            {
                if (this.pc_multa == null)
                    return "";
                return string.Format("{0,00}", this.pc_multa);
            }
        }

        public string taxaBiblioteca
        {
            get
            {
                if (this.pc_taxa_dia_biblioteca == null)
                    return "";
                return string.Format("{0,00}", this.pc_taxa_dia_biblioteca);
            }
        }

        public string dt_cadastro
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_cadastramento);
            }
        }

        public string hr_cadastro
        {
            get
            {
                return String.Format("{0:HH:mm:ss}", dt_cadastramento.ToLocalTime());
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_pessoa", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_pessoa", "Nome", AlinhamentoColuna.Left, "2.6000in"));
                retorno.Add(new DefinicaoRelatorio("dc_reduzido_pessoa", "Nome fantasia", AlinhamentoColuna.Left, "1.8000in"));
                retorno.Add(new DefinicaoRelatorio("dc_num_cgc", "CNPJ", AlinhamentoColuna.Center, "1.6000in"));
                retorno.Add(new DefinicaoRelatorio("dt_cadastramento", "Data cadasdro", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("dc_fone_email", "Telefone", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("escola_ativa", "Ativo", AlinhamentoColuna.Center));
                return retorno;
            }
        }

        public string hr_inicial_escola {
            get {
                return String.Format(@"{0:hh\:mm}", hr_inicial);
            }
        }

        public string hr_final_escola {
            get {
                return String.Format(@"{0:hh\:mm}", hr_final);
            }
        }

        public bool id_baixa_automatica_cartao { get; set; }
    }

    public class EscolaGrupos
    {
        public int cd_pessoa { get; set; }
        public string no_pessoa { get; set; }
        public ICollection<SysGrupo> SysGrupo { get; set; }
    }

    public class GruposEscolasUsuario
    {
        public List<Escola> empresas { get; set; }
        public List<SysGrupo> sysGrupoSGF {get;set;} 
    }

    public class PesquisaHorarioTurma
    {
        public int cd_turma { get; set; }
        public int cd_turma_PPT { get; set; }
        public int cd_sala { get; set; }
        public int[] professores { get; set; }
        public DateTime dt_inicio { get; set; }
        public DateTime? dt_final { get; set; }
        public int cd_duracao { get; set; }
        public int cd_curso { get; set; }
    }
}
