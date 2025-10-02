using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericBusiness.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Service.EmailMarketing.Model;
using System.Data;
//using FundacaoFisk.SGF.Web.Service.EmailMarketing.Model;
//using FundacaoFisk.SGF.Web.Services.EmailMarketing.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IBusiness
{
    public interface IEmailMarketingBusiness : IGenericBusiness
    {
        void sincronizarContextos(DbContext dbContext);

        //Lista não inscrito
        IEnumerable<ListaNaoInscrito> getListagemEnderecos(int cd_empresa, string no_pessoa, int status, string email, byte id_tipo_cadastro);
        IEnumerable<ListaNaoInscrito> getListagemEnderecosMalaDireta(int cd_empresa, int cd_mala_direta);
        IEnumerable<ListaNaoInscrito> getListaNaoIncritoEnderecos(int cd_empresa, string no_pessoa, int status, string email, byte id_tipo_cadastro);
        bool crudListaNaoIncritoEndereco(int cd_escola, List<ListaNaoInscrito> ListaEnderecos);
        MalaDireta postComporMensagem(MalaDireta mala_direta);
        MalaDireta visualizarEtiqueta(MalaDireta mala_direta);
        bool postComporMensagemEnviar(MalaDireta mala_direta);
        int salvarMalaDiretaEtiqueta(MalaDireta mala_direta);
        DataTable gerarEtiqueta(int cd_mala_direta);
        IEnumerable<MalaDireta> searchHistoricoMalaDireta(Componentes.Utils.SearchParameters parametros, string dc_assunto, DateTime? dt_mala_direta, int cd_empresa, int id_tipo_mala);
        MalaDireta getEditViewMalaDireta(int cd_mala_direta, int cd_empresa);
        IEnumerable<MalaDireta> getMalaDiretaPorAluno(Componentes.Utils.SearchParameters parametros, int cd_pessoa, int cd_empresa, string assunto, DateTime? dtaIni, DateTime? dtaFim);
        bool retirarEmailListaEndereco(int cd_empresa, int cd_cadastro, int id_cadastro);
        MalaDireta getMalaDiretaForView(int cd_mala_direta, int cd_escola);
        IEnumerable<RptListagemEndereco> getRptListagemEnderecos(int cd_mala_direta, int cd_empresa, string no_pessoa, int status, string email, byte id_tipo_cadastro);
    }
}
