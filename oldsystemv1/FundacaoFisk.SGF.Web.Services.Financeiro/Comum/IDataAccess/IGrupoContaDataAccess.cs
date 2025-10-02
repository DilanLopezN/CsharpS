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

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    public interface IGrupoContaDataAccess : IGenericRepository<GrupoConta>
    {
        IEnumerable<GrupoConta> GetGrupoContaSearch(SearchParameters parametros, string descricao, Boolean inicio, int tipoGrupo);
        Boolean deleteAllGrupoConta(List<GrupoConta> grupoConta);
        IEnumerable<GrupoConta> getListaContas(int cd_grupo_conta, string no_subgrupo_conta, bool inicio, int nivel, int tipoPlanoConta, int cd_pessoa_empresa);
        IEnumerable<GrupoConta> getGrupoContasWithPlanoContas(int cd_pessoa_empresa);
        bool getGrupoContasWhitOutPlanoContas(byte nivel, int cd_pessoa_empresa);
        IEnumerable<GrupoConta> getPlanoContasTreeSearch(int cd_escola, bool busca_somente_ativo, bool isDireitoContaSeg, string descricao, bool inicio);
        IEnumerable<GrupoConta> getPlanoContasTreeSearchWhitMovimento(int cd_escola, int tipoMovimento, string descricao, bool inicio);
        IEnumerable<GrupoConta> getPlanoContasTreeSearchWhitContaCorrente(int cd_escola, string descricao, bool inicio);
        IEnumerable<GrupoConta> getSubgrupoContaSearchFK(string descricao, bool inicio, int cdGrupo, SubgrupoConta.TipoNivelConsulta tipo, bool contaSegura, int cdEscola);
    }
}
