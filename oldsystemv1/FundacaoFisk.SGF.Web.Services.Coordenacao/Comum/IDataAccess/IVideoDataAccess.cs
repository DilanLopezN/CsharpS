using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;

namespace FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess
{
    public interface IVideoDataAccess : IGenericRepository<Video>
    {
        IEnumerable<Video> getVideoSearch(Componentes.Utils.SearchParameters parametros, string no_video, int nm_video, List<byte> menu);
        Video findVideoById(int cd_video);
        Video findDeletedVideoById(int cd_video);
        IEnumerable<Video> obterVideosPorFiltros(Componentes.Utils.SearchParameters parametros, string no_video, int nm_video, List<byte> menu);
        Video findVideoByNumeroParte(int nm_video, int nm_parte);
        Video findVideoByName(string no_video);
    }
}
