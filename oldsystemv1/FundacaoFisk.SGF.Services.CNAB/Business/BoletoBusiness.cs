using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.GenericModel;
using System.Transactions;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IDataAccess;
using Componentes.Utils.Messages;
using FundacaoFisk.SGF.Utils.Messages;

namespace FundacaoFisk.SGF.Web.Services.CNAB.Business
{
    using Componentes.GenericModel;
    using System.Globalization;
    using System.Data.Entity;
    using Componentes.GenericBusiness.Comum;

    public class BoletoBusiness : IBoletoBusiness
    {

        /// <summary>
        /// Declarações de  interfaces
        /// </summary>
        public ITituloCnabDataAccess Dao { get; set; }
        public ICnabDataAccess DaoCnab { get; set; }

        public BoletoBusiness(ITituloCnabDataAccess dao, ICnabDataAccess daoCnab)
        {
            if (dao == null || daoCnab == null)
                throw new ArgumentNullException();
            Dao = dao;
            DaoCnab = daoCnab;
        }

        // Configura os codigos do usuário para auditorias dos DataAccess
        public void configuraUsuario(int cdUsuario, int cd_empresa) {
            ((SGFWebContext) this.Dao.DB()).IdUsuario = ((SGFWebContext) this.DaoCnab.DB()).IdUsuario = cdUsuario;
        }

        public void sincronizarContextos(DbContext dbContext)
        {
            //this.Dao.sincronizaContexto(dbContext);
            //DaoCnab.sincronizaContexto(dbContext);
        }

        public IEnumerable<TituloCnab> getTitulosCnabBoletoByCnabs(int cd_escola, Int32[] cd_cnab, Int32[] cd_titulos_cnab)
        {
            IEnumerable<TituloCnab> ret = new List<TituloCnab>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DaoCnab.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                this.sincronizarContextos(DaoCnab.DB());
                bool eh_responsavel = false;
                if(cd_cnab != null)
                    eh_responsavel = DaoCnab.isResponsavelCNAB(cd_cnab);
                else
                    eh_responsavel = DaoCnab.isResponsavelTitulosCNAB(cd_titulos_cnab);

                ret = Dao.getTitulosCnabBoletoByCnabs(cd_escola, cd_cnab, cd_titulos_cnab, eh_responsavel);
                transaction.Complete();
            }
            return ret;
        }

        public IEnumerable<TituloCnab> getTitulosCnabBoletoByTitulosCnab(int cd_escola, Int32[] cd_titulos_cnab) {
            IEnumerable<TituloCnab> ret = new List<TituloCnab>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DaoCnab.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                this.sincronizarContextos(DaoCnab.DB());
                bool eh_responsavel = false;

                eh_responsavel = DaoCnab.isResponsavelTitulosCNAB(cd_titulos_cnab);

                ret = Dao.getTitulosCnabBoletoByTitulosCnab(cd_escola, cd_titulos_cnab, eh_responsavel);
                transaction.Complete();
            }
            return ret;
        }
    }
}
