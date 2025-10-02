using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Cnab
    {
        public enum Bancos
        {
            BancodoBrasil = 1,
            Banrisul = 41,
            Basa = 3,
            Bradesco = 237,
            DinariPay = 238,
            BRB = 70,
            Caixa = 104,
            HSBC = 399,
            Itau = 341,
            Real = 356,
            Safra = 422,
            Santander = 33,
            Sicoob = 756,
            Sicred = 748,
            Sudameris = 347,
            Unibanco = 409,
            CrediSIS = 97,
            Unicred = 136,
            Uniprime = 84,
            Inter = 077,
        }
        public enum TipoCnab
        {
            GERAR_BOLETOS = 1,
			CANCELAR_BOLETOS = 2,
			PEDIDO_BAIXA = 3
        }
        public enum StatusCnab
        {
           	ABERTO = 1,
            FECHADO = 2
        }
        public int cd_carteira_cnab { get; set; }
        public int cd_pessoa_empresa { get; set; }
        public bool adm { get; set; }
        public short nro_banco { get; set; }
        public string usuarioCnab { get; set; }
        public List<UsuarioWebSGF> usuarios { get; set; }
        public List<CarteiraCnab> carteirasCnab { get; set; }
        public List<Produto> produtos { get; set; }
        public List<LocalMovto> locaisMvto { get; set; }
        public byte id_impressao_carteira_cnab { get; set; }
        public int? nro_contrato { get; set; }

        public string tipoCnab
        {
            get
            {
                string retorno = "";
                switch (id_tipo_cnab)
                {
                    case (int)TipoCnab.GERAR_BOLETOS: retorno = "Gerar Boletos";
                        break;
                    case (int)TipoCnab.CANCELAR_BOLETOS: retorno = "Cancelar Boletos";
                        break;
                    case (int)TipoCnab.PEDIDO_BAIXA: retorno = "Pedido Baixa";
                        break;
                }
                return retorno;
            }
        }
        public string statusCnab
        {
            get
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("pt-BR");
                string retorno = "";
                switch (id_status_cnab)
                {
                    case (int)StatusCnab.ABERTO: retorno = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(StatusCnab.ABERTO.ToString()).ToLower());
                        break;
                    case (int)StatusCnab.FECHADO: retorno = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(StatusCnab.FECHADO.ToString()).ToLower());
                        break;
                }
                return retorno;
            }
        }
        public string carteira_cnab
        {
            get
            {
                string retorno = "";
                if (LocalMovimento != null && LocalMovimento.CarteiraCnab != null)
                    retorno = LocalMovimento.CarteiraCnab.no_carteira + "-" + LocalMovimento.CarteiraCnab.nm_carteira + "(" + LocalMovimento.no_local_movto + " | ag.:" +
                                   LocalMovimento.nm_agencia + " | c/c:" + LocalMovimento.nm_conta_corrente + "-" + LocalMovimento.nm_digito_conta_corrente + ")";
                return retorno;
            }
        }
        public string dta_inicial_vencimento
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_inicial_vencimento);
            }
        }
        public string dta_final_vencimento
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_final_vencimento);
            }
        }
        public string dta_emissao_cnab
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_emissao_cnab);
            }
        }
        public string dtah_cadastro_cnab
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy HH:mm}", dh_cadastro_cnab.ToLocalTime());
            }
        }
        public string vlTotalCnab
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_total_cnab);
            }
        }

        public static Cnab changeValuesCnab(Cnab cnabView, Cnab cnabContext)
        {
            cnabContext.cd_local_movto = cnabView.cd_local_movto;
            cnabContext.dt_final_vencimento = cnabView.dt_final_vencimento;
            cnabContext.dt_inicial_vencimento = cnabView.dt_inicial_vencimento;
            cnabContext.no_arquivo_remessa = cnabView.no_arquivo_remessa;
            cnabContext.id_responsavel_cnab = cnabView.id_responsavel_cnab;
            cnabContext.nm_dias_protesto = cnabView.nm_dias_protesto;
            return cnabContext;
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_produto", "Código"));
                retorno.Add(new DefinicaoRelatorio("carteira_cnab", "Carteira", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("tipoCnab", "Tipo", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("dta_emissao_cnab", "Emissão", AlinhamentoColuna.Center ,"0.8000in"));
                retorno.Add(new DefinicaoRelatorio("dta_inicial_vencimento", "Vcto.Inicial", AlinhamentoColuna.Center, "0.8000in"));
                retorno.Add(new DefinicaoRelatorio("dta_final_vencimento", "Vcto. Final", AlinhamentoColuna.Center, "0.8000in"));
                retorno.Add(new DefinicaoRelatorio("vlTotalCnab", "Valor", AlinhamentoColuna.Right, "0.8000in"));
                retorno.Add(new DefinicaoRelatorio("usuarioCnab", "Usuário", AlinhamentoColuna.Left, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("dtah_cadastro_cnab", " Data Cadastro", AlinhamentoColuna.Left, "1.4000in"));
                retorno.Add(new DefinicaoRelatorio("statusCnab", "Status", AlinhamentoColuna.Left, "0.8000in"));
                retorno.Add(new DefinicaoRelatorio("nro_contrato", "Contrato", AlinhamentoColuna.Left, "0.8000in"));
                return retorno;
            }
        }
    }
}
