using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IDesistenciaDataAccess : IGenericRepository<Desistencia> 
    {
        IEnumerable<DesistenciaUI> getDesistenciaSearchUI(SearchParameters parametros, int? cd_turma, int? cd_aluno, int? cd_pessoa_escola, int? cd_motivo_desistencia, int cd_tipo,
            DateTime? dta_ini, DateTime? dta_fim, int cd_produto, int cd_professor, List<int> cdsCurso);
        bool deleteAllDesistencia(List<DesistenciaUI> desistencia);
        DesistenciaUI getDesistenciaAlunoTurma(int cd_aluno_turma, int cd_pessoa_escola);
        bool getExisteDesistenciaPorAlunoTurma(int cd_aluno, int cd_turma, int cd_pessoa_escola);
        Desistencia retornaDesistenciaMax(DesistenciaUI desistenciaUI, int cd_pessoa_escola);
        bool getMaiorDataAposDataDesistencia(DesistenciaUI desistenciaUI, int cd_pessoa_escola);
        DateTime? getMenorDataAposDataDesistencia(DesistenciaUI desistenciaUI, int cd_pessoa_escola);
        int retornaQuantidadeDesistencia(int cd_turma, int cd_aluno, int cd_pessoa_escola, int cd_aluno_turma);
        DateTime? getMaiorDataDesistencia(DesistenciaUI desistenciaUI, int cd_pessoa_escola);
        DesistenciaUI getDesistenciaGridView(int cd_desistencia);
    }

}
