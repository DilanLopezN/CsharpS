using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class TurmaCancelaEncerramentoUI : TO
    {
        public enum StatusProcedure
        {
            SUCESSO_EXECUCAO_PROCEDURE = 0,
            ERRO_EXECUCAO_PROCEDURE = 1
        }

        public Nullable<int> cd_turma { get; set; }
        public Nullable<int> cd_usuario { get; set; }
        public Nullable<int> fuso { get; set; }
        public Nullable<DateTime> dt_termino { get; set; }

    }
}
