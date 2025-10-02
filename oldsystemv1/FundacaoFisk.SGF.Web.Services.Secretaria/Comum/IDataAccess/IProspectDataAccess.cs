using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IProspectDataAccess : IGenericRepository<Prospect>
    {
        IEnumerable<ProspectSearchUI> GetProspectSearch(SearchParameters parametros, string nome, bool inicio, string email, int cdEscola, DateTime? dataIni, DateTime? dataFim, bool? ativo, bool aluno, int testeClassificacaoMatriculaOnline);
        bool deleteAll(List<Prospect> prospects);
        ProspectSearchUI getExistsProspectEmail(string email, int cdEscola, int cdProspect);
        ProspectSearchUI verificaExistenciaProspect(string telefone, string celular, int cd_escola, int cd_prospect, string no_pessoa);
        Prospect getProspectForEdit(int cdProspect, int cdEscola, string email);
        Prospect getProspectAllData(int cdProspect, int cdEscola);
        PessoaFisicaSGF verificarPessoaFisicaEmail(string email);
        PessoaFisicaSGF verificarPessoaFisicaEmailCadProspect(string email);
        Prospect getProspectForAluno(int cdProspect, int cdEscola);
        IEnumerable<ProspectSearchUI> getProspectFKSearch(SearchParameters parametros, int cdEscola, string nome, bool inicio, string email, string telefone, ProspectDataAccess.TipoConsultaEnum tipo);
        ProspectIntegracaoRetornoUI postProspectIntegracao(Nullable<int> nm_integracao, Nullable<byte> id_tipo, Nullable<int> id_teste, string no_pessoa, string email, string fone, string cep, string day_week, string periodo, Nullable<System.DateTime> dt_cadastro, string sexo, Nullable<double> hit, string phase, string courseId);

        ProspectGeradoIntegracaoRetornoUI postGetProspectsGeradosSendPromocao();
    //    bool PostDeleteMatricula(Nullable<int> cd_contrato, Nullable<int> cd_usuario);
        int? getBaixaFinanceira(int cd_prospect, int cd_empresa);
        bool existeProspectNaoConsultado(int cd_escola);
        IEnumerable<ReportProspect> getProspectAtendido(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos, int cd_faixa_etaria);
        IEnumerable<ReportProspect> getProspectAtendidoMatricula(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos);
        Prospect getProspectPorEmail(int cdEscola, string email);
        List<ProspectSiteUI> getProspectSite(int cd_prospect, int tipo);
        IEnumerable<ReportProspect> getComparativoProspectAtendido(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos);
        string getNomeAtendente(int cdUsuario, int cdEscola);
        ProspectSearchUI getExistsProspectEmailENome(string email, int cdEscola, string nomme);
        Prospect getProspectByPessoaFisica(int cd_pessoa_fisica);
        PromocaoIntercambioParams findProspectApiPromocaoIntercambio(int prospectCdProspect);
    }
}
