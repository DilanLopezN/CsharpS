using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class AtividadeExtraReportUI : TO
    {

        public int cd_atividade_extra { get; set; }

        public Nullable<int> cd_produto { get; set; }

        public Nullable<int> cd_curso { get; set; }

        public Nullable<int> cd_aluno { get; set; }

        public Nullable<int> cd_funcionario { get; set; }

        public Nullable<byte> id_participacao{ get; set; }

        public bool esconde_obs { get; set; }

        public Nullable<DateTime> dta_ini { get; set; }

        public Nullable<DateTime> dta_fim { get; set; }

        public Nullable<byte> id_lancada { get; set; }

    }
}
