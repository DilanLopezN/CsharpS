using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using Componentes.Utils;
using System.Data.SqlClient;
using Componentes.GenericDataAccess.GenericException;
using System.Data;
using System.Data.Common;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using System.Data.Entity.Core.Objects;
using System.Net.Sockets;
using System.Security.Policy;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class TurmaEscolaDataAccess : GenericRepository<TurmaEscola>, ITurmaEscolaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

       public IEnumerable<TurmaEscolaSearchUI> getTurmasEscolatWithTurma(int cd_turma)
        {
            try
            {
                var sql = from turmaEscola in db.TurmaEscola
                          where turmaEscola.cd_turma == cd_turma
                          orderby turmaEscola.Escola.no_pessoa
                    select new TurmaEscolaSearchUI()
                    {
                        cd_turma_escola = turmaEscola.cd_turma_escola,
                        cd_turma = turmaEscola.cd_turma,
                        cd_escola = turmaEscola.cd_escola,
                        dc_reduzido_pessoa = turmaEscola.Escola.dc_reduzido_pessoa
                    };

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

       public IEnumerable<TurmaEscola> getTurmasEscolatByTurma(int cd_turma)
       {
           try
           {
               var sql = from turmaEscola in db.TurmaEscola
                   where turmaEscola.cd_turma == cd_turma
                   orderby turmaEscola.Escola.no_pessoa
                   select turmaEscola;

               return sql;
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
       }

       public IEnumerable<TurmaEscola> getTurmasEscolatByIdAndTurma(int cd_turma, int cd_escola, int cd_turma_escola)
       {
           try
           {
               var sql = from turmaEscola in db.TurmaEscola
                   where turmaEscola.cd_turma == cd_turma &&
                         turmaEscola.cd_escola == cd_escola &&
                         turmaEscola.cd_turma_escola == cd_turma_escola
                   select turmaEscola;

               return sql;
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
       }
    }
}