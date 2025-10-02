using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess
{
    public interface IControleFaltasDataAccess : IGenericRepository<ControleFaltas>
    {
        IEnumerable<ControleFaltasUI> getControleFaltasSearch(Componentes.Utils.SearchParameters parametros, string desc, int cd_turma, int cd_aluno, ControleFaltasDataAccess.AssinaturaControleFaltas assinatura, DateTime? dtInicial, DateTime? dtFinal, int cdEscola);
        ControleFaltasUI getControleFaltasUIbyId(int cd_controle_faltas);
        ControleFaltas getControleFaltasEdit(int cd_controle_faltas);
        List<sp_RptControleFaltas_Result> getReportControleFaltasResults(Nullable<int> cd_tipo, int cd_escola,
            Nullable<int> cd_curso, Nullable<int> cd_nivel,
            Nullable<int> cd_produto, Nullable<int> cd_professor, Nullable<int> cd_turma, Nullable<int> cd_sit_turma,
            string cd_sit_aluno, string dt_inicial,
            string dt_final, bool quebrarpagina);
    }
}
