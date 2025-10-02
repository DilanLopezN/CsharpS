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
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class TipoDescontoEscolaDataAccess : GenericRepository<TipoDescontoEscola>, ITipoDescontoEscolaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }
        public IEnumerable<int> getTipoDescontoWithEscola(int cdTpDesc)
        {
            try
            {
                var sql = (from tpDescEscola in db.TipoDescontoEscola
                           where tpDescEscola.cd_tipo_desconto == cdTpDesc
                           select tpDescEscola.cd_pessoa_escola).Distinct();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public TipoDescontoEscola findTpDescEscolabyId(int cdTpDesc, int cdPessoa)
        {
            try
            {
                var sql = (from tpDescEscola in db.TipoDescontoEscola
                           where tpDescEscola.cd_tipo_desconto == cdTpDesc &&
                                 tpDescEscola.cd_pessoa_escola == cdPessoa
                           select tpDescEscola).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<TipoDescontoEscola> getTpDescEscolaByTpDesc(int cdTpDesc)
        {
            try
            {
                var sql = (from tpDescEscola in db.TipoDescontoEscola
                           where tpDescEscola.cd_tipo_desconto == cdTpDesc
                           select tpDescEscola).Distinct();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool existeContatoDesconto(int cdTpDesc, int cdEscola)
        {
            try
            {
                var sql = (from dc in db.DescontoContrato
                           join c in db.Contrato
                           on dc.cd_contrato equals c.cd_contrato
                           where
                           dc.cd_tipo_desconto == cdTpDesc && 
                           (cdEscola == 0 ||c.cd_pessoa_escola == cdEscola)
                           select dc).FirstOrDefault();
                return sql != null && sql.cd_desconto_contrato > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
