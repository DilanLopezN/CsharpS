using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
    public interface IFollowUpDataAccess : IGenericRepository<FollowUp>
    {
        IEnumerable<FollowUp> getFollowUpProspect(int cdProspect, int cd_escola);
        bool deleteAllFollowUp(List<FollowUp> follows);
        IEnumerable<FollowUp> getFollowUpByAluno(int cdAluno, int cd_escola);
        IEnumerable<FollowUp> getFollowAluno(SearchParameters parametros, int cd_aluno, int cd_escola);
        IEnumerable<FollowUpSearchUI> getFollowUpSearch(SearchParameters parametros, int cdEscola, byte id_tipo_follow, int cd_usuario_org, int cd_usuario_destino, int cd_prospect_aluno,
                                                       int cd_acao, int resolvido, int lido, bool data, bool proximo_contato, DateTime? dt_inicial, DateTime? dt_final, bool id_usuario_adm,
                                                       int cd_usuario_logado, int cd_aluno, bool usuario_login_master);
        FollowUp getFollowEditView(int cd_follow_up, int cd_escola, int id_tipo_follow);
        FollowUpSearchUI getFollowUpGrid(int? cd_pessoa_empresa, int cd_follow_up);
        IEnumerable<FollowUp> getFollowUps(List<int> codigosFollowUps, int cd_usuario_origem);
        bool verificaExisteFollowUPOutroUsuario(List<int>  codigosFollowUps,int cd_usuario_origem);
        bool existeRespostaFollowUp(int cd_follow_up);
        IEnumerable<FollowUp> getFollowUpByAlunoAllData(int cdAluno, int cd_escola);
        bool existeFollowNaoResolvido(int cd_usuario_logado, int cd_escola, bool usuario_login_master);
        List<FollowUpRptUI> GetRtpFollowUp(byte id_tipo_follow, int cd_usuario_org, string no_usuario_org, int resolvido, int lido, DateTime? dtaIni, DateTime? dtaFinal, int cd_escola);
    }
}
