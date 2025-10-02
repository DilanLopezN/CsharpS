using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using Componentes.GenericDataAccess.Comum;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

    public interface ITransacaoFinanceiraDataAccess : IGenericRepository<TransacaoFinanceira>
    {
        TransacaoFinanceira getTransacaoFinanceira(int cd_tran_finan, int cd_pessoa_empresa);
        TransacaoFinanceira getTransacaoBaixaTitulo(int cd_titulo, int cd_pessoa_empresa);
        TransacaoFinanceira getTransacaoFinanceira(int cd_titulo, int cd_contrato, int cd_pessoa_empresa);
        TransacaoFinanceira getTransacaoBaixaTituloBolsaReajusteAnual(int cd_titulo, int cd_contrato, int cd_pessoa_empresa);
    }

}
