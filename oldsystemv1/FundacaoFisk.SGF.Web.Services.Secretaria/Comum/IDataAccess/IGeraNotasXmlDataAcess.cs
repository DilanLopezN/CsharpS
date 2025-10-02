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
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IGeraNotasXmlDataAccess : IGenericRepository<ImportacaoXML>
    {
        IEnumerable<ImportacaoXML> getListaXmlGerados(SearchParameters parametros, XmlSearchUI notatualizaUI);
        int abrirGerarXML(int cd_usuario);
        int postGerarXmlProc(int cd_usuario);
        IEnumerable<ImportacaoXML> buscarGerarXML(int cd_usuario);
        IEnumerable<ImportacaoXML> setAtualizaXML(List<int> cdImportXML);
    }

}