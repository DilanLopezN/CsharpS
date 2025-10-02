using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class ProspectSearchUI : TO
    {
        public enum TipoPesquisaEnum
        {
            PROSPECT = 1,
            ALUNO = 2
        }

        public string no_pessoa { get; set; }
        public string email { get; set; }
        public string dc_email_escola { get; set; }
        public string telefone { get; set; }
        public string celular { get; set; }
        public System.DateTime dt_cadastramento { get; set; }
        public Nullable<System.DateTime> dt_matricula { get; set; }
        public byte? nm_sexo { get; set; }
        public int cd_prospect { get; set; }
        public int cd_pessoa_fisica { get; set; }
        public int cd_midia { get; set; }
        public string no_midia { get; set; }
        public Nullable<int> cd_pessoa_escola { get; set; }
        public int cd_usuario { get; set; }
        public string no_usuario { get; set; }
        public byte id_periodo { get; set; }
        public Nullable<int> cd_operadora { get; set; }
        public byte id_dia_semana { get; set; }
        public bool id_prospect_ativo { get; set; }
        public Nullable<decimal> vl_matricula { get; set; }
        public bool gerar_baixa { get; set; }
        public Nullable<int> cd_local_movto { get; set; }
        public Nullable<int> cd_plano_conta_tit { get; set; }
        public Nullable<int> cd_tipo_liquidacao { get; set; }
        public string desc_plano_conta { get; set; }
        public string no_escola { get; set; }
        public byte? id_faixa_etaria { get; set; }
        public Nullable<byte> cd_motivo_inativo { get; set; }
        public decimal vl_abatimento { get; set; }
        public Nullable<double> pc_acerto_teste { get; set; }
        public string dc_acerto_teste { get; set; }
        public Nullable<byte> id_tipo_online { get; set; }
        public Nullable<int> id_teste_online { get; set; }
        public string dc_reduzido_pessoa_escola { get; set; }

        public virtual EnderecoSGF endereco { get; set; }
        public virtual EnderecoUI enderecoUI { get; set; }
        public virtual ICollection<Midia> midias { get; set; }
        public virtual ICollection<ProspectProduto> produtos { get; set; }
        public virtual ICollection<ProspectDia> dias { get; set; }
        public virtual ICollection<ProspectPeriodo> periodos { get; set; }
        public virtual ICollection<FollowUp> listaFollowUp { get; set; }
        public virtual ICollection<ProspectMotivoNaoMatricula> listaMotivos { get; set; }
        public virtual ICollection<Operadora> operadoras { get; set; }
        public virtual ICollection<TelefoneUI> TelefonePessoa { get; set; }
        public Parametro parametro { get; set; }
        public System.DateTime? dt_nascimento_prospect { get; set; }

        public string dta_cadastramento_prospect
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_cadastramento);
            }

        }
        public string dta_matricula_prospect
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_matricula);
            }

        }
        public string dta_nascimento_prospect
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_nascimento_prospect);
            }

        }

        public string prospect_ativo
        {
            get
            {
                return this.id_prospect_ativo ? "Sim" : "Não";
            }
        }

        public string vlAbatimento
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_abatimento > 0 ? vl_abatimento : 0);
            }
        }

        public static ProspectSearchUI fromProspectUI(ProspectSearchUI prospect, int cd_prospect, DateTime dt_cadastramento)
        {
            ProspectSearchUI retornProspect = new ProspectSearchUI
            {
                cd_prospect = cd_prospect,
                email = prospect.email,
                celular = prospect.celular,
                telefone = prospect.telefone,
                dt_cadastramento = dt_cadastramento,
                no_pessoa = prospect.no_pessoa,
                cd_midia = prospect.cd_midia,
                id_periodo = prospect.id_periodo,
                nm_sexo = prospect.nm_sexo,
                cd_pessoa_fisica = prospect.cd_pessoa_fisica,
                no_usuario = prospect.no_usuario,
                cd_usuario = prospect.cd_usuario,
                id_prospect_ativo = prospect.id_prospect_ativo,
                cd_operadora = prospect.cd_operadora,
                id_dia_semana = prospect.id_dia_semana,
                dias = prospect.dias,
                periodos = prospect.periodos,
                produtos = prospect.produtos,
                endereco = prospect.endereco,
                vl_matricula = prospect.vl_matricula,
                dt_matricula = prospect.dt_matricula,
                cd_plano_conta_tit = prospect.cd_plano_conta_tit,
                gerar_baixa = prospect.gerar_baixa,
                cd_local_movto = prospect.cd_local_movto,
                cd_tipo_liquidacao = prospect.cd_tipo_liquidacao,
                desc_plano_conta = prospect.desc_plano_conta,
                no_escola = prospect.no_escola,
                id_faixa_etaria = prospect.id_faixa_etaria,
                cd_motivo_inativo = prospect.cd_motivo_inativo,
                vl_abatimento =  prospect.vl_abatimento,
                pc_acerto_teste = prospect.pc_acerto_teste,
                dc_acerto_teste = prospect.dc_acerto_teste,
                id_tipo_online = prospect.id_tipo_online,
                id_teste_online = prospect.id_teste_online,
                dt_nascimento_prospect = prospect.dt_nascimento_prospect
            };
            return retornProspect;
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                retorno.Add(new DefinicaoRelatorio("no_pessoa", "Nome Prospect", AlinhamentoColuna.Left, "2.2000in"));
                retorno.Add(new DefinicaoRelatorio("email", "E-mail", AlinhamentoColuna.Left, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("dta_cadastramento_prospect", "Data Cadastro", AlinhamentoColuna.Left, "1.3000in"));
                retorno.Add(new DefinicaoRelatorio("telefone", "Telefone", AlinhamentoColuna.Left, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("celular", "Celular", AlinhamentoColuna.Left, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("prospect_ativo", "Ativo", AlinhamentoColuna.Left));
                return retorno;
            }
        }
   }
}
