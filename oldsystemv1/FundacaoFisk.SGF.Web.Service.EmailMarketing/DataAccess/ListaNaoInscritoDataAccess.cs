using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Service.EmailMarketing.DataAccess
{
    public class ListaNaoInscritoDataAccess: GenericRepository<ListaNaoInscrito>, IListaNaoInscritoDataAccess
    {

        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<ListaNaoInscrito> getListaNaoIncritoEnderecos(int cd_empresa, string no_pessoa, int status, string email, byte id_tipo_cadastro)
        {
            try
            {
                IEnumerable<ListaNaoInscrito> retorno;
                var sql = from l in db.ListaNaoInscrito
                          where l.cd_escola == cd_empresa
                          select l;
                if (!string.IsNullOrEmpty(no_pessoa))
                    sql = from l in sql
                          where db.PessoaSGF.Any(x => x.no_pessoa.Contains(no_pessoa) && x.cd_pessoa == l.cd_cadastro)
                          select l;
                if (!String.IsNullOrEmpty(email))
                    sql = from l in sql
                          where db.PessoaSGF.Any(x => x.cd_pessoa == l.cd_cadastro && x.TelefonePessoa.Any(t => t.dc_fone_mail.Contains(email) && t.id_telefone_principal))
                          select l;
                if (id_tipo_cadastro > 0)

                    sql = from l in sql
                          where l.id_cadastro == id_tipo_cadastro
                          select l;
                retorno = (from l in sql
                           select new
                                 {
                                     cd_cadastro = l.cd_cadastro,
                                     no_pessoa = db.PessoaSGF.Where(x => x.cd_pessoa == l.cd_cadastro).FirstOrDefault().no_pessoa,
                                     l.id_cadastro,
                                     l.cd_lista_nao_inscrito
                                     
                                     //email = db.TelefoneSGF.Where(t => t.cd_pessoa == l.cd_cadastro && t.cd_tipo_telefone == (int)TipoTelefone.TipoTelefoneEnum.EMAIL &&
                                     //                                  t.id_telefone_principal).FirstOrDefault().dc_fone_mail//,
                                     //inscrito = db.ListaNaoInscrito.Where(x => x.cd_escola == cd_empresa && x.cd_cadastro == p.cd_pessoa && x.id_cadastro == id_tipo_cadastro).Any()
                                 }).ToList().Select(x => new ListaNaoInscrito
                                 {
                                     cd_lista_nao_inscrito = x.cd_lista_nao_inscrito,
                                     cd_cadastro = x.cd_cadastro,
                                     no_pessoa = x.no_pessoa,
                                     //email = x.email,
                                     id_cadastro = x.id_cadastro//,
                                     //id_inscrito = x.inscrito
                                 }).OrderBy(o => o.no_pessoa);
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ListaNaoInscrito> getListaNaoIncritoEscola(int cd_empresa)
        {
            try
            {
                var sql = from l in db.ListaNaoInscrito
                          where l.cd_escola == cd_empresa
                          select l;
                return sql;
      
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool jaExisteNaoIncrito(int cd_empresa, int cd_cadastro, int id_cadastro)
        {
            try
            {
                var sql = (from l in db.ListaNaoInscrito
                          where l.cd_escola == cd_empresa && l.cd_cadastro == cd_cadastro && l.id_cadastro == id_cadastro
                          select l.cd_lista_nao_inscrito).Any();
                return sql;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}
