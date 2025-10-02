using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using System.Data.SqlClient;
using System.Data;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAtividadeExtraDataAccess : IGenericRepository<AtividadeExtra>
    {
        IEnumerable<AtividadeExtraUI> searchAtividadeExtra(SearchParameters parametros, DateTime? dataIni, DateTime? dataFim, TimeSpan? hrInicial, TimeSpan? hrFinal, int? tipoAtividade, int? curso, int? responsavel, int? produto, int? aluno, byte lancada, int cdEscola, int cd_escola_combo = 0);
        bool deleteAllAtividadeExtra(List<AtividadeExtra> atividadesExtras);
        AtividadeExtra findByIdAtividadeExtraFull(int cdAtividadeExtra);
        List<sp_RptAtividadeExtra_Result> getReportAtividadeExtra(Nullable<int> cd_escola, Nullable<System.DateTime> dta_ini, Nullable<System.DateTime> dta_fim, Nullable<int> cd_produto, Nullable<int> cd_curso, Nullable<int> cd_funcionario, Nullable<int> cd_aluno, Nullable<byte> id_participacao, Nullable<byte> id_lancada);
        List<sp_RptAtividadeExtraAluno_Result> getReportAtividadeExtraAluno(Nullable<int> cd_atividade_extra, Nullable<int> cd_aluno, Nullable<byte> id_participou, Nullable<byte> id_lancada, Nullable<int> cd_escola);
        AtividadeExtraUI returnAtividadeExtraUsuarioAtendente(int cd_atividade_extra, int cd_pessoa_escola);
        IEnumerable<AtividadeExtra> getAtividadesAluno(SearchParameters parametros, int cd_aluno, int cd_escola);
        List<AtividadeExtra> getAtividadeExtraByCdAtividadeRecorrencia(int cd_atividade_recorrencia);
    }
}
