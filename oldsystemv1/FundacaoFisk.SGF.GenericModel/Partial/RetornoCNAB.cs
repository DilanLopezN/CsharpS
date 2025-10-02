using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class RetornoCNAB
    {
        
        public enum StatusRetornoCNAB
        {
           	ABERTO = 1,
            FECHADO = 2
        }
        public int cd_carteira_retorno_cnab { get; set; }
        public int cd_pessoa_empresa { get; set; }
        public bool adm { get; set; }
        public short nro_banco { get; set; }
        public string usuarioRetornoCNAB { get; set; }
        public List<UsuarioWebSGF> usuarios { get; set; }
        public List<CarteiraCnab> carteirasRetornoCNAB { get; set; }
        public List<Produto> produtos { get; set; }
        public List<LocalMovto> locaisMvto { get; set; }
        public string retornoTemporario { get; set; }
        public List<DespesaTituloCnab> DespesasTituloCnab { get; set; }
        public string nm_banco { get; set; }

        public string statusRetornoCNAB
        {
            get
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("pt-BR");
                string retorno = "";
                switch (id_status_cnab)
                {
                    case (int)StatusRetornoCNAB.ABERTO: retorno = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(StatusRetornoCNAB.ABERTO.ToString()).ToLower());
                        break;
                    case (int)StatusRetornoCNAB.FECHADO: retorno = culture.TextInfo.ToTitleCase(culture.TextInfo.ToTitleCase(StatusRetornoCNAB.FECHADO.ToString()).ToLower());
                        break;
                }
                return retorno;
            }
        }
        public string carteira_retorno_cnab
        {
            get
            {
                string retorno = "";
                if (LocalMovto != null && LocalMovto.CarteiraCnab != null)
                    retorno = LocalMovto.CarteiraCnab.no_carteira + "-" + LocalMovto.CarteiraCnab.nm_carteira + "(" + LocalMovto.no_local_movto + " | ag.:" +
                                   LocalMovto.nm_agencia + " | c/c:" + LocalMovto.nm_conta_corrente + "-" + LocalMovto.nm_digito_conta_corrente + ")";
                return retorno;
            }
        }
       
        public string dta_cadastro_cnab
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy HH:mm}", dt_cadastro_cnab.ToLocalTime());
            }
        }

        public static RetornoCNAB changeValuesRetCnab(RetornoCNAB retCnabView, RetornoCNAB retCnabContext)
        {
            retCnabContext.cd_local_movto = retCnabView.cd_local_movto;
            retCnabContext.no_arquivo_retorno = retCnabView.no_arquivo_retorno;
            return retCnabContext;
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_produto", "Código"));
                retorno.Add(new DefinicaoRelatorio("carteira_retorno_cnab", "Carteira", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("no_arquivo_retorno", "Retorno", AlinhamentoColuna.Right, "0.8000in"));
                retorno.Add(new DefinicaoRelatorio("usuarioRetornoCNAB", "Usuário", AlinhamentoColuna.Left, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("dta_cadastro_cnab", " Data Cadastro", AlinhamentoColuna.Left, "1.4000in"));
                retorno.Add(new DefinicaoRelatorio("statusRetornoCNAB", "Status", AlinhamentoColuna.Left, "0.8000in"));
                return retorno;
            }
        }
    }
}
