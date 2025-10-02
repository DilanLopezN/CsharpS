using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class Contas
    {
        public enum Hierarquia
        {
            PAI = 1,
            FILHO = 2
        }

        public Contas()
        {
            children = new List<Contas>();
        }

        public int id { get; set; }
        public int pai { get; set; }
        public int cd_conta { get; set; }
        public int cd_subgrupo_conta { get; set; }
        public int cd_plano_conta { get; set; }
        public int? cd_subgrupo_pai { get; set; }
        public int? cd_grupo_conta { get; set; }
        public byte? id_tipo_conta { get; set; }
        public bool id_ativo { get; set; }
        public bool id_conta_segura { get; set; }
        public bool marcado { get; set; }
        public string dc_conta { get; set; }

        public ICollection<Contas> children { get; set; }

    }
}
