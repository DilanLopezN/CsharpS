using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
   public class ItemUI : TO
    {
        public int cd_iitem_kit { get; set; }
        public int cd_item { get; set; }
        public int? cd_integracao { get; set; }
        public bool id_item_ativo { get; set; }
        public int qt_estoque { get; set; }
        public decimal vl_item { get; set; }
        public decimal vl_custo { get; set; }
        public string no_item { get; set; }
        public string dc_titulo { get; set; }
        public string no_autor { get; set; }
        public string dc_assunto { get; set; }
        public string dc_local { get; set; }
        public string dc_tipo_item { get; set; }
        public string no_grupo_estoque { get; set; }
        public Nullable<decimal> pc_aliquota_icms { get; set; }
        public Nullable<decimal> pc_aliquota_iss { get; set; }
        public int cd_tipo_item { get; set; }
        public int cd_grupo_estoque { get; set; }
        public string dc_codigo_barra { get; set; }
        public string dc_sgl_item { get; set; }
        public string dc_classificacao_fiscal { get; set; }
        public string dc_classificacao { get; set; }
        public Nullable<byte> cd_origem_fiscal { get; set; }
        public int id_categoria_grupo { get; set; }
        public bool hasClickEscola { get; set; }
        public bool id_movto_estoque { get; set; }
        public int? cd_plano_conta { get; set; }
        public string desc_plano_conta { get; set; }
        public int? cd_subgrupo_conta { get; set; }
        public int? cd_subgrupo_conta_2 { get; set; }
        public string desc_subgrupo_conta { get; set; }
        public string desc_subgrupo_conta_2 { get; set; }
        public int? cd_situacao_tributario { get; set; }
        public int? cd_cest { get; set; }
        public bool id_kit { get; set; }
        public int? cd_curso { get; set; }
        public bool id_material_didatico { get; set; }
        public bool id_ppt { get; set; }
        public int cd_item_curso { get; set; }
        public bool id_voucher_carga { get; set; }
        public byte qt_horas_carga { get; set; }

        public ICollection<ItemKit> itemKit { get; set; }
        public ICollection<ItemEscola> itemEscolas { get; set; }
        public ICollection<Escola> escolas { get; set; }
        public ICollection<GrupoEstoque> grupos { get; set; }
        public ICollection<ItemSubgrupo> itemSubgrupo { get; set; }
        public ICollection<TipoItem> tipos { get; set; }
       
       
       public IEnumerable<ItemMovimento> items { get; set; }
       public int cd_item_kit { get; set; }
       public int qt_item_kit { get; set; }
       public int ultimo_valor_kit { get; set; }

       public ItemSubgrupo ItemSubgrupoDoItemNoKit { get; set; }

        public static ItemUI fromItem(Item item, string tipoItem, string grupoEstoque, ItemEscola itemEscola, Biblioteca biblioteca, string descPlanoConta)
        {
            ItemUI itemui = new ItemUI
            {
                cd_item = item.cd_item,
                cd_integracao = item.cd_integracao,
                no_item = item.no_item,
                pc_aliquota_icms = item.pc_aliquota_icms,
                pc_aliquota_iss = item.pc_aliquota_iss,
                cd_tipo_item = item.cd_tipo_item,
                dc_tipo_item = tipoItem,
                cd_grupo_estoque = item.cd_grupo_estoque,
                dc_codigo_barra = item.dc_codigo_barra,
                id_item_ativo = item.id_item_ativo,
                dc_sgl_item = item.dc_sgl_item,
                dc_classificacao_fiscal = item.dc_classificacao_fiscal,
                qt_estoque = itemEscola.qt_estoque,
                vl_custo = itemEscola.vl_custo,
                vl_item =itemEscola.vl_item,
                dc_assunto = biblioteca.dc_assunto,
                dc_local = biblioteca.dc_local,
                dc_titulo = biblioteca.dc_titulo,
                no_autor = biblioteca.no_autor,
                no_grupo_estoque = grupoEstoque,
                id_categoria_grupo =  item.GrupoEstoque != null && item.GrupoEstoque.cd_grupo_estoque > 0?  item.GrupoEstoque.id_categoria_grupo : 0,
                cd_plano_conta = itemEscola.cd_plano_conta,
                desc_plano_conta = descPlanoConta,
                id_kit = item.id_kit,
                itemKit = item.ItemKit
                
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
        public string dc_ppt
        {
            get
            {
                return this.id_ppt ? "Sim" : "Não";
            }
        }

        public string custo
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_custo);
            }
        }

        public string categoria_grupo
        {
            get
            {
                string categoria = "";
                if (this.id_categoria_grupo == 1)
                    categoria = "Privada";
                else
                    if (this.id_categoria_grupo == 2)
                        categoria = "Pública";
                return categoria;
            }
        }


        public string venda
        {
            get
            {
                return string.Format("{0:#,0.00}", this.vl_item);
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_item", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_item", "Descrição", AlinhamentoColuna.Left, "3.3000in"));
                retorno.Add(new DefinicaoRelatorio("no_grupo_estoque", "Grupo", AlinhamentoColuna.Left, "2.0000in"));
                retorno.Add(new DefinicaoRelatorio("dc_tipo_item", "Tipo", AlinhamentoColuna.Left, "1.2000in"));
                retorno.Add(new DefinicaoRelatorio("categoria_grupo", "Categoria", AlinhamentoColuna.Left, "0.8500in"));
                retorno.Add(new DefinicaoRelatorio("qt_estoque", "Qtde.", AlinhamentoColuna.Right, "0.6000in"));
                retorno.Add(new DefinicaoRelatorio("item_ativo", "Ativo", AlinhamentoColuna.Center, "0.6000in"));
                

                return retorno;
            }
        }

        public string iss
        {
            get
            {
                if (this.pc_aliquota_iss == null)
                    return "";
                return string.Format("{0,00}", this.pc_aliquota_iss);
            }
        }
        public string icms
        {
            get
            {
                if (this.pc_aliquota_icms == null)
                    return "";
                return string.Format("{0,00}", this.pc_aliquota_icms);
            }
        }
    }
}

