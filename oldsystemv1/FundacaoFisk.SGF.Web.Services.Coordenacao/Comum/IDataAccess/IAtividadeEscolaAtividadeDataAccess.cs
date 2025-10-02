using System.Collections.Generic;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAtividadeEscolaAtividadeDataAccess : IGenericRepository<AtividadeEscolaAtividade>
    {
        IEnumerable<AtividadeEscolaAtividadeSearchUI> getAtividadeEscolatWithAtividade(int cd_atividade_extra);

        IEnumerable<AtividadeEscolaAtividade> getAtividadesEscolatByAtividade(int cd_atividade_extra);

        IEnumerable<AtividadeEscolaAtividade> getAtividadesEscolatByIdAndAtividade(int cd_atividade_extra, int cd_escola, int cd_atividade_escola);
    }
}