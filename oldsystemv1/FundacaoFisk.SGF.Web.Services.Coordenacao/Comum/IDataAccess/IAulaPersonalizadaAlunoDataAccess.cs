using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAulaPersonalizadaAlunoDataAccess : IGenericRepository<AulaPersonalizadaAluno>
    {
        IEnumerable<AulaPersonalizadaAluno> searchAulaPersonalizadaAlunoByAulaAluno(int cdAulaPersonalizada, int cdEscola, int cd_aluno);
        IEnumerable<AulaPersonalizadaAluno> searchAulaPersonalizadaAlunoByAula(int cdAulaPersonalizada, int cdEscola);
    }
}
