using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class SaldoItem
    {
        public enum StatusProcedure
        {
            SUCESSO_EXECUCAO_PROCEDURE = 0,
            ERRO_EXECUCAO_PROCEDURE = 1
        }

        public enum OperacaoProcedure
        {
            INCLUIR = 0,
            EXCLUIR = 1,
            ALTERAR = 2
        }

        public string no_item { get; set; }
        public int cd_grupo_estoque { get; set; }
        public int cd_tipo_item { get; set; }
        public List<SaldoItem> listaSaldo { get; set; }
        public bool inicio { get; set; }
        public bool id_movto_estoque { get; set; }
        public bool id_material_didatico { get; set; }
        public bool id_voucher_carga { get; set; }
        public bool editado { get; set; }
        public string vlCustoAtual
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_custo_atual);
            }
        }
        public string vlVendaAtual
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_venda_atual);
            }
        }
        
    }
}
