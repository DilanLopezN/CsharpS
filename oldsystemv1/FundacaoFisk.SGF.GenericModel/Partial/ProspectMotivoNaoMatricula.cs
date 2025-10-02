using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class ProspectMotivoNaoMatricula : TO
    {
        public string dc_motivo_nao_matricula_prospect
        {
            get
            {
                var ret = "";
                if (this.MotivoNaoMatricula != null && this.MotivoNaoMatricula.cd_motivo_nao_matricula > 0)
                {
                    ret = this.MotivoNaoMatricula.dc_motivo_nao_matricula;
                    this.Prospect = null;
                    this.MotivoNaoMatricula = null;
                }
                return ret;
            }
        }
    }
}
