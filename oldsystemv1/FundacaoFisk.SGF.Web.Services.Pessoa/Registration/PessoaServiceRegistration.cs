using MvcTurbine.ComponentModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Registration
{
    public class PessoaServiceRegistration :IServiceRegistration
    {
        public void Register(IServiceLocator locator)
        {
            locator.Register<IPessoaDataAccess, PessoaDataAccess>();
            locator.Register<IPapelDataAccess, PapelDataAccess>();
            locator.Register<ITipoLogradouroDataAccess, TipoLogradouroDataAccess>();
            locator.Register<ILocalidadeDataAccess, LocalidadeDataAccess>();
            locator.Register<IPaisDataAccess, PaisDataAccess>();
            locator.Register<IEstadoDataAccess, EstadoDataAccess>();
            locator.Register<ITipoEnderecoDataAccess, TipoEnderecoDataAccess>();
            locator.Register<IClasseTelefoneDataAccess, ClasseTelefoneDataAccess>();
            locator.Register<ITipoTelefoneDataAccess, TipoTelefoneDataAccess>();
            locator.Register<IOperadoraDataAccess, OperadoraDataAccess>();
            locator.Register<IPessoaBusiness, PessoaBusiness>();
            locator.Register<ILocalidadeBusiness, LocalidadeBusiness>();
            locator.Register<IPessoaFisicaDataAccess, PessoaFisicaDataAccess>();
            //Telefone
            locator.Register<ITelefoneDataAccess, TelefoneDataAccess>();
            //Endereco
            locator.Register<IEnderecoDataAccess, EnderecoDataAccess>();
            //Atividade
            locator.Register<IAtividadeDataAccess, AtividadeDataAccess>();
            //Estado Civil
            locator.Register<IEstadoCivilDataAccess, EstadoCivilDataAccess>();
            //Orgão Expedidor
            locator.Register<IOrgaoExpedidorDataAccess, OrgaoExpedidorDataAccess>();
            //Tratamento Pessoa
            locator.Register<ITratamentoPessoaDataAccess, TratamentoPessoaDataAccess>();
            //Relacionamento
            locator.Register<IRelacionamentoDataAccess, RelacionamentoDataAccess>();
        }
    }
}
