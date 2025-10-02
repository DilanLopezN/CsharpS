using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using System.Data.SqlClient;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class ChequeDataAccess : GenericRepository<Cheque>, IChequeDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public Cheque getChequeByContrato(int id)
        {
            try{
                Cheque sql = (from i in db.Cheque
                             where i.cd_contrato == id
                             select i).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Cheque> getChequesByTitulosContrato(List<int> cdTitulos, int cd_empresa)
        {
            try
            {
                SGFWebContext dbContext = new SGFWebContext();
                int cd_origem_contrato = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                int cd_origem_movimento = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Movimento"].ToString());
                var sql = (from c in db.Cheque
                          where c.Contrato.cd_pessoa_escola == cd_empresa &&
                                db.Titulo.Any(x => x.cd_origem_titulo == c.cd_contrato && x.id_origem_titulo == cd_origem_contrato && cdTitulos.Contains(x.cd_titulo))
                          select c).Distinct().Union(from c in db.Cheque
                           where c.Movimento.cd_pessoa_empresa == cd_empresa &&
                                 db.Titulo.Any(x => x.cd_origem_titulo == c.cd_movimento && x.id_origem_titulo == cd_origem_movimento && cdTitulos.Contains(x.cd_titulo))
                           select c).Distinct().ToList();
                return sql.ToList() ;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Cheque getChequeTransacao(int cd_tran_finan, int cd_empresa)
        {
            try
            {
                Cheque sql = null;
                sql = (from c in db.Cheque
                       join ct in db.ChequeTransacao on c.cd_cheque equals ct.cd_cheque
                       where ct.cd_tran_finan == cd_tran_finan && ct.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa
                       select new
                       {
                           c.cd_cheque,
                           c.no_emitente_cheque,
                           c.no_agencia_cheque,
                           c.nm_agencia_cheque,
                           c.nm_digito_agencia_cheque,
                           c.nm_conta_corrente_cheque,
                           c.nm_digito_cc_cheque,
                           c.nm_primeiro_cheque,
                           c.cd_banco,
                           ct.dt_bom_para,
                           ct.nm_cheque
                       }).ToList().Select(x => new Cheque { 
                          cd_cheque = x.cd_cheque,
                          no_emitente_cheque = x.no_emitente_cheque,
                          no_agencia_cheque = x.no_agencia_cheque,
                          nm_agencia_cheque = x.nm_agencia_cheque,
                          nm_digito_agencia_cheque = x.nm_digito_agencia_cheque,
                          nm_conta_corrente_cheque = x.nm_conta_corrente_cheque,
                          nm_digito_cc_cheque = x.nm_digito_cc_cheque,
                          cd_banco = x.cd_banco,
                          dt_bom_para = x.dt_bom_para,
                          nm_primeiro_cheque = x.nm_cheque
                       }).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


    }
}