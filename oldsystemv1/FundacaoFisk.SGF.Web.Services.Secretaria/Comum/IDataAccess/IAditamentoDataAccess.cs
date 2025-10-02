using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IAditamentoDataAccess : IGenericRepository<Aditamento>
    {
        Aditamento getAditamentoByContrato(int cd_contrato, int cd_escola);
        Aditamento getAditamentoByContrato(int cd_aditamento, int cd_contrato, int cd_escola);
        IEnumerable<Aditamento> getAditamentosByContrato(int cd_contrato, int cd_escola);
        Aditamento getAditamentoByContratoEData(DateTime? data_aditamento, int cd_contrato, int cd_pessoa_escola);
        Aditamento getAditamentoByContratoMaxData(int cd_contrato, int cd_escola);
        Aditamento getPenultimoAditamentoByContrato(int cd_contrato, int cd_escola, DateTime dataUltimoAdt);
        IEnumerable<Aditamento> getAditamentosByIds(int cd_contrato, int cd_escola, List<int> cdAditamentos);
        IEnumerable<Aditamento> getAllDataAditamentosByContrato(int cd_contrato, int cd_escola);
        IEnumerable<DescontoContrato> getDescontosAplicadosAditamento(int cd_contrato, int cd_escola);
        IEnumerable<DescontoContrato> getDescontosAplicadosContratoOrAditamento(int cd_contrato, int cd_escola);
        Decimal getValorMaterialImpressaoContrato(int cd_contrato, int cd_escola);
        bool verificaAditamentoAposReajusteAnual(int cd_empresa, int cd_reajuste_anual);
        IEnumerable<Aditamento> getAditamentosByIdsContrato(int cd_escola, int cd_reajuste_anual);
        Aditamento getUltimoAditamentoByContrato(int cd_contrato, int cd_pessoa_escola);
        List<AditamentoBolsa> getAditamentoBolsaByAditamento(int cd_aditamento);
        List<DescontoContrato> getDescontoContratoByAditamento(int cd_aditamento);
    }
}
