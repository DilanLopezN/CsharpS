using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface INomeContratoDataAccess : IGenericRepository<NomeContrato>
    {
        IEnumerable<NomeContrato> getSearchNoContrato(SearchParameters parametros, string desc, string layout, bool inicio, bool? status, int cdEscola,bool masterGeral);
        IEnumerable<NomeContrato> getNomesContratosByListaCodigos(int[] cdNomesContratos,int? cdEscola,bool masterGeral);
        NomeContrato getNomeContratoById(int? cdEscola, int cdNomeContrato);
        IEnumerable<NomeContrato> getNomesContrato(NomeContratoDataAccess.TipoConsultaNomeContratoEnum hasDependente, int? cd_nome_contrato, int? cd_escola);
        NomeContrato getNomeContratoAditamentoMatricula(int cd_contrato, int cd_escola);
        IEnumerable<NomeContrato> getNomeContratoMat(int? cdEscola);
        string getNomeContratoDigitalizadoByCdContrato(int cdEscola, int cdContrato, string nomeArquivo);
        string getNomeContratoDigitalizadoByEscolaAndCdContrato(int cdEscola, int cdContrato);
    }
}
