using System;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.CNAB.Comum.IDataAccess
{
    public interface ITituloCnabDataAccess : IGenericRepository<TituloCnab>
    {
        IEnumerable<TituloCnab> getTitulosCnabBoletoByTitulosCnab(int cd_escola, Int32[] cd_titulo_cnab, bool eh_responsavel);
        IEnumerable<TituloCnab> getTitulosCnabBoletoByCnabs(int cd_escola, Int32[] cd_cnab, Int32[] cd_titulo_cnab, bool eh_responsavel);
        TituloCnab getTituloCnabEditView(int cd_cnab, int cd_titulo_cnab, int cdEmpresa);
        IEnumerable<TituloCnab> getAllTituloCnabByCnab(int cd_escola, int cd_cnab);
        decimal somaValorTodosTitulosCnab(int cd_escola, int cd_cnab);
        string getNossoNumeroTitulo(int cd_titulo);
        IEnumerable<Titulo> existeTituloCnabComStatusDiferenteEviado(int cd_cnab, int cd_empresa);
    }
}
