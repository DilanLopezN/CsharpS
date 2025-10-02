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
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
    using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
    public interface ILocalMovtoDataAccess : IGenericRepository<LocalMovto>
    {
        List<LocalMovto> getLocalMovtoByEscola(int cdEscola, int cd_local_movto, bool semcarteira);
        List<LocalMovto> getAllLocalMovtoByEscola(int cdEscola, int cd_local_movto);
        List<LocalMovto> getLocalMovtoCdEEsc(int cdEscola, int? cdLocalMovto);
        IEnumerable<LocalMovtoUI> getLocalMovtoSearch(SearchParameters parametros, int cdEscola, string nome, string nmBanco, bool inicio, bool? status, int tipo, string pessoa, int cd_pessoa_usuario);
        LocalMovtoUI getLocalMovtoById(int cdEscola, int cdLocalMovto);
        LocalMovto findLocalMovtoById(int cdEscola, int cdLocalMovto);
        List<LocalMovto> getLocalMovimento(int cdEscola, int cd_loc_mvto, LocalMovtoDataAccess.TipoConsultaLocalMovto tipoConsulta, int cd_pessoa_usuario);
        List<LocalMovto> getLocalMovimentoComFiltrosTrocaFinanceira(int cdEscola, int cd_loc_mvto, int cd_tipo_financeiro, LocalMovtoDataAccess.TipoConsultaLocalMovto tipoConsulta, int cd_pessoa_usuario);
        int findByLocal(int? cd_local_movt, int cd_tipo_liquidacao);
        List<LocalMovto> getLocalMovtoBaixa(int cd_escola, int? cd_loc_mvto, int natureza, int[] listPessoas, int cd_pessoa_usuario);
        IEnumerable<LocalMovto> getAllLocalMovto(int cdEscola, bool isOrigem, int cd_pessoa_usuario);
        IEnumerable<LocalMovto> getLocalMovtoAtivosWithConta(int cdEscola, bool isOrigem, bool isCasdatrar, int cdLocalMovto);
        IEnumerable<LocalMovto> getLocalMovtoAtivosWithContaUsuario(int cdEscola, bool isOrigem, int cdLocalMovto, int cd_pessoa_usuario);
        IEnumerable<LocalMovto> getLocalMovtoAtivosWithCodigo(int cdEscola, bool isOrigem, int cd_local);
        IEnumerable<LocalMovimentoWithContaUI> getLocalMovtoWithContaByEscola(int cdEscola, int cd_pessoa_usuario);
        IEnumerable<LocalMovto> getLocalMovto(int cdEscola, int cdLocalMovto);
        IEnumerable<LocalMovto> getAllLocalMovtoCartao(int cdEscola);
        IEnumerable<LocalMovto> getAllLocalMovtoCartaoSemPai(int cdEscola);
        IEnumerable<LocalMovto> getAllLocalMovtoCartaoComPai(int cdEscola);
        IEnumerable<LocalMovto> getAllLocalMovtoTipoCartao(int cdEscola, int cd_tipo_liquidacao, int cd_local_movto, int cd_pessoa_usuario);
        IEnumerable<LocalMovto> getAllLocalMovtoCheque(int cdEscola);
        LocalMovto findCodigoClienteForCnab(int cd_escola, int cd_local);
        bool verificaCarteiraCnab(int cdCarteira);
        long getNossoNumeroLocalMovimento(int cd_escola, int cd_local_movto);
        LocalMovto getLocalMovimentoWithPessoaBanco(int cd_local_movto);
        bool verificaLocalTituloTemCNAB(int cd_local_movto, int cd_pessoa_empresa);
        LocalMovtoUI getLocalByTitulo(int cdEscola, int cd_local_movto);
        IEnumerable<LocalMovto> getLocalMovtoProspect(int cdEscola, int cd_loc_mvto, int cd_pessoa_usuario);
        LocalMovto findLocalMovtoComCarteira(int cdEscola, int cdLocalMovto);
        IEnumerable<LocalMovto> getAllLocalMovtoBanco(int cdEscola);
    }
}
