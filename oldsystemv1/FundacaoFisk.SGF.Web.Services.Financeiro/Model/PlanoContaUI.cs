using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class PlanoContaUI
    {
        public ICollection<GrupoConta> gruposContas { get; set; }
        public ICollection<PlanoConta> planosContasEmpresa { get; set; }
        public ICollection<PlanoConta> planosContasDisponiveis { get; set; }
        public int? nivel_plano_conta { get; set; }
        public int tipoPlano { get; set; }
        public bool hasGrupoSubGrupoDisponivel { get; set; }
        public int cd_plano_conta { get; set; }
        public int cd_subgrupo_conta { get; set; }
        public bool id_ativo { get; set; }
        public bool id_conta_segura { get; set; }
        public byte id_tipo_conta{ get; set; }
    }
}
