using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
   public class LocalMovtoUI : TO
    {
        public int cd_local_movto { get; set; }
        public Nullable<int> cd_pessoa_local { get; set; }
        public string no_pessoa_local { get; set; }
        public string no_local_movto { get; set; }
        public byte nm_tipo_local { get; set; }
        public string nm_banco { get; set; }
        public string nm_agencia { get; set; }
        public string nm_conta_corrente { get; set; }
        public string dc_num_cliente_banco { get; set; }
        public Nullable<int> cd_banco { get; set; }
        public string desc_banco { get; set; }
        public string nm_digito_conta_corrente { get; set; }
        public string dc_pessoa_conjunta { get; set; }
        public bool id_conta_conjunta { get; set; }
        public string dc_cpf_pessoa_conjunta { get; set; }
        public bool id_local_ativo { get; set; }
        public int cd_pessoa_empresa { get; set; }
        public Nullable<int> cd_pessoa_banco { get; set; }
        public string no_pessoa_banco { get; set; }
        public long? dc_nosso_numero { get; set; }
        public string nomeLocal { get; set; }
        public string noCarteira { get; set; }
        public Nullable<int> cd_carteira_cnab { get; set; }
        public int nm_sequencia { get; set; }
        public string nm_transmissao { get; set; }
        public Nullable<short> nm_digito_cedente { get; set; }
        public short nm_op_conta { get; set; }
        public string nm_digito_agencia { get; set; }
        public ICollection<LocalMovto> locaMovto { get; set; }
        public ICollection<TipoLiquidacao> tipoLiquidacao { get; set; }
        public ICollection<Titulo> titulos { get; set; }
        public List<Banco> bancos { get; set; }
        public ICollection<TaxaBancaria> taxaBancaria { get; set; }
        public int? cd_local_banco { get; set; }

        public string local_ativo
        {
            get
            {
                return this.id_local_ativo ? "Sim" : "Não";
            }
        }

        public string conta_conjunta
        {
            get
            {
                return this.id_conta_conjunta ? "Sim" : "Não";
            }
        }

        public string nossoNumero
        {
            get
            {
                if (this.dc_nosso_numero == null)
                    return "";
                return this.dc_nosso_numero.ToString();
            }
        }

        public string desc_tipo_local
        {
            get
            {
                var tipo = "";
                switch (nm_tipo_local)
                {
                    case (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTEIRA: tipo = "Carteira";
                        break;
                    case (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO: tipo = "Banco";
                        break;
                    case (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA: tipo = "Caixa";
                        break;
                    case (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO: tipo = "Cartão de Crédito";
                        break;
                    case (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO: tipo = "Cartão de Débito";
                        break;
                    default: tipo = "";
                        break;
                }
                return tipo;
            }
            set { }
        }


        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_item", "Código"));
                retorno.Add(new DefinicaoRelatorio("desc_tipo_local", "Tipo", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("no_local_movto", "Nome", AlinhamentoColuna.Left, "1.5000in"));
                retorno.Add(new DefinicaoRelatorio("nm_banco", "Banco", AlinhamentoColuna.Left, "0.6000in"));
                retorno.Add(new DefinicaoRelatorio("nm_agencia", "Agência", AlinhamentoColuna.Left, "0.7500in"));
                retorno.Add(new DefinicaoRelatorio("nm_digito_agencia", "Dig. Ag.", AlinhamentoColuna.Left, "0.7000in"));
                retorno.Add(new DefinicaoRelatorio("nm_conta_corrente", "C/C", AlinhamentoColuna.Left, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("nm_digito_conta_corrente", "Dig.", AlinhamentoColuna.Left, "0.4500in"));
                retorno.Add(new DefinicaoRelatorio("no_pessoa_local", "Pessoa", AlinhamentoColuna.Left, "1.5000in"));
                retorno.Add(new DefinicaoRelatorio("local_ativo", "Ativo", AlinhamentoColuna.Center, "0.5000in"));
                retorno.Add(new DefinicaoRelatorio("conta_conjunta", "C.Conjunta", AlinhamentoColuna.Center, "1.0000in"));

                return retorno;
            }
        }
    }
}

