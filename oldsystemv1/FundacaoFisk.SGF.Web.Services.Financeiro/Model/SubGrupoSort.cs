using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class SubGrupoSort
    {
        public int cd_subgrupo_conta { get; set; }
        public byte id_tipo_grupo_conta { get; set; }
        public int? cd_grupo_conta { get; set; }
        public string no_subgrupo_conta { get; set; }
        public Nullable<int> cd_subgrupo_pai { get; set; }
        public virtual GrupoConta SubgrupoContaGrupo { get; set; }
        public virtual ICollection<SubgrupoConta> SubgruposFilhos { get; set; }
        public virtual SubgrupoConta SubgrupoPai { get; set; }
        public string grupo_conta { get; set; }
        public string subgrupo_2_conta { get; set; }
        public int? nm_ordem_subgrupo { get; set; }
        public string dc_cod_integracao_plano { get; set; }

        public static List<SubgrupoConta> parseSubGrupoForSubgrupoContaUI(IEnumerable<SubGrupoSort> subgrupoContas)
        {
            List<SubgrupoConta> subgruposUI = new List<SubgrupoConta>();
            // SubgrupoConta subg = null;
            if (subgrupoContas.Count() > 0)
            {
                foreach (SubGrupoSort s in subgrupoContas)
                    subgruposUI.Add(parseSubgrupoForSubgrupoUI(s));
            }
            return subgruposUI;
        }

        public static SubgrupoConta parseSubgrupoForSubgrupoUI(SubGrupoSort subgrupo)
        {
            SubgrupoConta subg = new SubgrupoConta();
            if (subgrupo.SubgrupoPai != null && subgrupo.SubgrupoPai.cd_subgrupo_conta > 0)
            {
                subg.cd_grupo_conta = subgrupo.cd_grupo_conta;
                subg.cd_subgrupo_conta = subgrupo.cd_subgrupo_conta;
                subg.cd_subgrupo_pai = subgrupo.cd_subgrupo_pai;
                subg.grupo_conta = subgrupo.SubgrupoPai.SubgrupoContaGrupo != null ? subgrupo.SubgrupoPai.SubgrupoContaGrupo.no_grupo_conta : "";
                subg.subgrupo_2_conta = subgrupo.no_subgrupo_conta;
                subg.no_subgrupo_conta = subgrupo.SubgrupoPai.no_subgrupo_conta;
                subg.dc_cod_integracao_plano = subgrupo.dc_cod_integracao_plano;
            }
            else
            {
                subg.cd_grupo_conta = subgrupo.cd_grupo_conta;
                subg.cd_subgrupo_conta = subgrupo.cd_subgrupo_conta;
                subg.grupo_conta = subgrupo.SubgrupoContaGrupo.no_grupo_conta;
                subg.no_subgrupo_conta = subgrupo.no_subgrupo_conta;
                subg.dc_cod_integracao_plano = subgrupo.dc_cod_integracao_plano;
            }
            subg.nm_ordem_subgrupo = subgrupo.nm_ordem_subgrupo;
            return subg;
        }

        public static SubgrupoConta parseSubgrupoForSubgrupoUI(SubgrupoConta subgrupo)
        {
            SubgrupoConta subg = new SubgrupoConta();
            if (subgrupo.SubgrupoPai != null && subgrupo.SubgrupoPai.cd_subgrupo_conta > 0)
            {
                subg.cd_grupo_conta = subgrupo.cd_grupo_conta;
                subg.cd_subgrupo_conta = subgrupo.cd_subgrupo_conta;
                subg.cd_subgrupo_pai = subgrupo.cd_subgrupo_pai;
                subg.grupo_conta = subgrupo.SubgrupoPai.SubgrupoContaGrupo.no_grupo_conta;
                subg.subgrupo_2_conta = subgrupo.no_subgrupo_conta;
                subg.no_subgrupo_conta = subgrupo.SubgrupoPai.no_subgrupo_conta;
            }
            else
            {
                subg.cd_grupo_conta = subgrupo.cd_grupo_conta;
                subg.cd_subgrupo_conta = subgrupo.cd_subgrupo_conta;
                subg.grupo_conta = subgrupo.SubgrupoContaGrupo.no_grupo_conta;
                subg.no_subgrupo_conta = subgrupo.no_subgrupo_conta;
            }
            subg.nm_ordem_subgrupo = subgrupo.nm_ordem_subgrupo;
            return subg;
        }
    }
}
