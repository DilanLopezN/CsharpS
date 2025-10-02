using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class GrupoEstoque {

        public enum CategoriaEnum
        {
            PRIVADA = 1,
            PUBLICA = 2

        }


        public string grupo_estoque_ativo {
            get {
                return this.id_grupo_estoque_ativo ? "Sim" : "Não";
            }
        }

        public string eliminar_inventario
        {
            get
            {
                return this.id_eliminar_inventario ? "Sim" : "Não";
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

/// <summary>
/// Função que monta as colunos no relatório do Grupo de Estoque 
/// </summary>
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_grupo_estoque", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_grupo_estoque", "Grupo de Item", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("categoria_grupo", "Categoria", AlinhamentoColuna.Center, "2.0000in"));
                retorno.Add(new DefinicaoRelatorio("grupo_estoque_ativo", "Ativo", AlinhamentoColuna.Center));
                retorno.Add(new DefinicaoRelatorio("eliminar_inventario", "El.Inventário", AlinhamentoColuna.Center));
                return retorno;
            }
        }
    }
}
