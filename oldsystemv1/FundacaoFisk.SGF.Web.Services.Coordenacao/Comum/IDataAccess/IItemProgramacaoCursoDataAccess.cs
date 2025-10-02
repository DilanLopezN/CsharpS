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

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
    public interface IItemProgramacaoCursoDataAccess : IGenericRepository<ItemProgramacao>
    {
        IEnumerable<ItemProgramacao> getCursoProg(int cdCurso, int cdDuracao, int? cd_escola);
        IEnumerable<ItemProgramacao> getItensProgramacaoCursoById(int cdProgramacao);
        bool deleteAllItemProgramacaoCurso(List<ItemProgramacao> itensProgramacao);
        ItemProgramacao addItemProgramacao(ItemProgramacao itens);
        ItemProgramacao editItemProgramacao(ItemProgramacao item);
    }
}
