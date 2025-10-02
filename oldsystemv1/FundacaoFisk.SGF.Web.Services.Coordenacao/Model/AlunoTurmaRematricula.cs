using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model {
    public class AlunoTurmaRematricuaUI : TO {
        public enum StatusProcedure
        {
            SUCESSO_EXECUCAO_PROCEDURE = 0,
            ERRO_EXECUCAO_PROCEDURE = 1
        }

      public string dc_turma { get; set;}
      public Nullable<System.DateTime> dt_inicial { get; set;}
      public Nullable<System.DateTime> dt_final { get; set;}
      public Nullable<bool> id_turma_nova { get; set;}
      public Nullable<int> cd_layout { get; set;}
      public Nullable<System.DateTime> dt_termino { get; set; }
      public int fuso { get; set; }
    }
}

