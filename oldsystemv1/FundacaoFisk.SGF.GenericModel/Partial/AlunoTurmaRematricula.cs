using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class AlunoTurmaRematricuaUI : TO
    {
        public enum StatusProcedure
        {
            SUCESSO_EXECUCAO_PROCEDURE = 0,
            ERRO_EXECUCAO_PROCEDURE = 1
        }

        public string cd_turma { get; set; }
        public string dt_inicial { get; set; }
        public string dt_final { get; set; }
        public string id_turma_nova { get; set; }
        public string cd_layout { get; set; }
    }
}