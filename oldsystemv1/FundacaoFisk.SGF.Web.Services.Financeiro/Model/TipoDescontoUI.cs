using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public partial class TipoDescontoUI : TO
    {
        public int cd_tipo_desconto { get; set; }
        public string dc_tipo_desconto { get; set; }
        public int cd_tipo_desconto_escola { get; set; }
        public int cd_pessoa_escola { get; set; }
        public bool id_tipo_desconto_ativo { get; set; }
        public Nullable<decimal> pc_desconto { get; set; }
        public bool id_incide_parcela_1 { get; set; }
        public bool id_incide_baixa { get; set; }
        public bool hasClickEscola { get; set; }
        public ICollection<Escola> escolas { get; set; }
        public ICollection<TipoDescontoEscola> escolasTpDesconto { get; set; }

        public string tipo_desconto_ativo
        {
            get
            {
                string _tp_desc_ativo = "";
                if (this.cd_tipo_desconto_escola > 0)
                    _tp_desc_ativo = this.id_tipo_desconto_ativo ? "Sim" : "Não";
                return _tp_desc_ativo;
            }
        }
        public string incide_parcela_1
        {
            get
            {
                string _incide_parcela_1 = "";
                if (this.cd_tipo_desconto_escola > 0)
                    _incide_parcela_1 = this.id_incide_parcela_1 ? "Sim" : "Não";
                return _incide_parcela_1;
            }
        }
        public string incide_baixa
        {
            get
            {
                string _incide_baixa = "";
                if (this.cd_tipo_desconto_escola > 0)
                    _incide_baixa = this.id_incide_baixa ? "Sim" : "Não";
                return _incide_baixa;
            }
        }
        public string pc_desc
        {
            get
            {
                if (this.pc_desconto == null)
                    return "";
                return string.Format("{0,00}", this.pc_desconto);
            }
        }
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_tipo_desconto", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_tipo_desconto", "Tipo Desconto"));
                retorno.Add(new DefinicaoRelatorio("incide_baixa", "Baixa", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("incide_parcela_1", "1ºParcela", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("pc_desc", "Percentual", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("tipo_desconto_ativo", "Ativo", AlinhamentoColuna.Center));
                return retorno;
            }
        }
    }
}
