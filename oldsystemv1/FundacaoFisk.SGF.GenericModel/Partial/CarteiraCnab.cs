using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class CarteiraCnab
    {
        public enum TipoColunas { 
            CNAB400 = 400,
            CNAB240 = 240
        }
        public enum TipoImpressao
        {
            TODOS = 0,
            BANCO = 1,
            ESCOLA = 2
        }

        public int cd_localMvto {get;set;}
        public LocalMovto localMovtoCateiraCnab { get; set; }
        public string carteira_ativa
        {
            get {
                return this.id_carteira_ativa ? "Sim" : "Não";
            }
        }
        public string dc_registro
        {
            get
            {
                return this.id_registrada ? "Sim" : "Não";
            }
        }

        public string nomeCarteira
        {
            get
            {
                return this.nm_carteira + " - " + this.no_carteira;
            }
        }

        private string _bancoCarteira;
        public string bancoCarteira
        {
            get
            {
                if (this.Banco != null && this.Banco.no_banco != null)
                    return this.Banco.no_banco;
                return _bancoCarteira;
            }
            set
            {
                _bancoCarteira = value;
            }
        }
        public string no_carteira_completa
        {
            get {
                string retorno = "";
                if (localMovtoCateiraCnab != null)
                    retorno = no_carteira + "-" + nm_carteira + "(" + localMovtoCateiraCnab.no_local_movto + " | ag.:" +
                                   localMovtoCateiraCnab.nm_agencia + " | c/c:" + localMovtoCateiraCnab.nm_conta_corrente + "-" + localMovtoCateiraCnab.nm_digito_conta_corrente + ")";
                return retorno;
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_curso", "Código"));
                retorno.Add(new DefinicaoRelatorio("bancoCarteira", "Banco", AlinhamentoColuna.Left, "3.0000in"));
                retorno.Add(new DefinicaoRelatorio("no_carteira", "Nome", AlinhamentoColuna.Left, "3.0000in"));
                retorno.Add(new DefinicaoRelatorio("nm_carteira", "Número", AlinhamentoColuna.Left));
                retorno.Add(new DefinicaoRelatorio("dc_registro", "Registrada", AlinhamentoColuna.Left));
                retorno.Add(new DefinicaoRelatorio("nm_colunas", "Colunas", AlinhamentoColuna.Left));
                retorno.Add(new DefinicaoRelatorio("carteira_ativa", "Ativa", AlinhamentoColuna.Center));

                return retorno;
            }
        }
    }
}
