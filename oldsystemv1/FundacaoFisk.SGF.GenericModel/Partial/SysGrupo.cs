using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class SysGrupo
    {
        public enum TipoGrupo
        {
            GRUPO = 0,
            GRUPO_MASTER = 1
        }
        public bool ehSelecionado = false;
        public bool alteraDireito { get; set; }
        public List<Escola> Empresas { get; set; }

        public static SysDireitoGrupo changeValuesSysDireitoGrupo(SysDireitoGrupo sysDireitoContext, SysDireitoGrupo sysDireitoView)
        {
            sysDireitoContext.cd_menu = sysDireitoView.cd_menu;
            sysDireitoContext.id_alterar_grupo = sysDireitoView.id_alterar_grupo;
            sysDireitoContext.id_excluir_grupo = sysDireitoView.id_excluir_grupo;
            sysDireitoContext.id_inserir_grupo = sysDireitoView.id_inserir_grupo;
            return sysDireitoContext;
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_grupo", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_grupo", "Grupo de Permissões", AlinhamentoColuna.Left, "3.8000in"));
                return retorno;
            }

        }
     
    }
}
