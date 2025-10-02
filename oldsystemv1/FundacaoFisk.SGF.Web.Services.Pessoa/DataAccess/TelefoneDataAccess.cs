using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess
{
    public class TelefoneDataAccess : GenericRepository<TelefoneSGF>, ITelefoneDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        // retorna um telefone da base
        public TelefoneSGF retornaTelefonefirstOrDefault()
        {
            try{
                var sql = (from telefone in db.TelefoneSGF
                           select telefone).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TelefoneSGF FindTypeTelefone(int cdPessoa, int tipoTel)
        {
            try{
                var sql = (from telefone in db.TelefoneSGF
                           where telefone.cd_pessoa == cdPessoa
                           && telefone.cd_tipo_telefone == tipoTel
                           select telefone).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TelefoneSGF> getAllTelefonesContatosByPessoa(int cdPessoa) {
            try {
                var sql = (from t in db.TelefoneSGF
                           where t.cd_pessoa == cdPessoa
                           select new {
                               dc_fone_mail = t.dc_fone_mail,
                               no_tipo_telefone = t.TelefoneTipo.no_tipo_telefone
                           }).ToList().Select(x => new TelefoneSGF {
                               dc_fone_mail = x.dc_fone_mail,
                               TelefoneTipo = new TipoTelefoneSGF { no_tipo_telefone = x.no_tipo_telefone }
                           });
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TelefoneSGF> GetAllTelefoneByPessoa(int cdPessoa)
        {
            try{
                var sql = from telefones in db.TelefoneSGF
                          where telefones.cd_pessoa == cdPessoa
                          select telefones;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TelefoneSGF FindTypeTelefonePrincipal(int cdPessoa, int tipoTel)
        {
            try{
                var sql = (from telefone in db.TelefoneSGF
                           where telefone.cd_pessoa == cdPessoa
                           && telefone.cd_tipo_telefone == tipoTel && telefone.id_telefone_principal == true
                           select telefone).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
