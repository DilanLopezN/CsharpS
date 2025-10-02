using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using System.Data;
//using FundacaoFisk.SGF.Web.Service.EmailMarketing.Model;
//using FundacaoFisk.SGF.Web.Services.EmailMarketing.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IDataAccess
{
    public interface IMalaDiretaDataAccess : IGenericRepository<MalaDireta>
    {
        IEnumerable<MalaDireta> searchHistoricoMalaDireta(Componentes.Utils.SearchParameters parametros, string dc_assunto, DateTime? dt_mala_direta, int cd_empresa, int id_tipo_mala);
        MalaDireta getMalaDiretaEditView(int cd_mala_direta, int cd_empresa);
        MalaDireta getMalaDiretaForView(int cd_mala_direta, int cd_empresa);
        IEnumerable<MalaDiretaCurso> getCursosMalaDireta(int cd_mala_direta, int cd_empresa);
        IEnumerable<MalaDiretaProduto> getProdutosMalaDireta(int cd_mala_direta, int cd_empresa);
        IEnumerable<MalaDiretaPeriodo> getPeriodosMalaDireta(int cd_mala_direta, int cd_empresa);
        IEnumerable<MalaDiretaCadastro> getTiposPessoaMalaDireta(int cd_mala_direta, int cd_empresa);
        IEnumerable<MalaDireta> getMalaDiretaPorAluno(Componentes.Utils.SearchParameters parametros, int cd_pessoa, int cd_empresa, string assunto, DateTime? dtaIni, DateTime? dtaFim);
        DataTable gerarEtiqueta(int cd_mala_direta);
    }
}
