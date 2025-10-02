using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
   public class KitUI : TO
    {
        public int cd_item { get; set; }
        public bool id_item_ativo { get; set; }
        public string no_item { get; set; }
        public int qt_item_kit { get; set; }
        public int cd_tipo_item { get; set; }
        public int cd_grupo_estoque { get; set; }
        public int cd_iitem_kit { get; set; }

        public ICollection<ItemEscola> itemEscolas { get; set; }
        public ICollection<Escola> escolas { get; set; }

        public static KitUI fromItem(Item item, string tipoItem, string grupoEstoque, ItemEscola itemEscola, Biblioteca biblioteca, string descPlanoConta)
        {
            KitUI itemui = new KitUI
            {
                cd_item = item.cd_item,
                no_item = item.no_item,
                cd_tipo_item = item.cd_tipo_item,
                cd_grupo_estoque = item.cd_grupo_estoque,
                id_item_ativo = item.id_item_ativo,
            };
            return itemui;
        }

        public string item_ativo
        {
            get
            {
                return this.id_item_ativo ? "Sim" : "Não";
            }
        }

        

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_item", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_item", "Nome", AlinhamentoColuna.Left, "3.3000in"));
                retorno.Add(new DefinicaoRelatorio("no_grupo_estoque", "Grupo", AlinhamentoColuna.Left, "2.0000in"));
                retorno.Add(new DefinicaoRelatorio("dc_tipo_item", "Tipo", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("categoria_grupo", "Categoria", AlinhamentoColuna.Left, "0.8500in"));
                retorno.Add(new DefinicaoRelatorio("item_ativo", "Ativo", AlinhamentoColuna.Center, "0.6000in"));
                

                return retorno;
            }
        }

       
    }
}

