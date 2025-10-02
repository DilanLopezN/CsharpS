using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using System.Data.SqlClient;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using Componentes.GenericDataAccess.GenericException;
    using System.Data.Objects;
    public class PoliticaAlunoDataAccess : GenericRepository<PoliticaAluno>, IPoliticaAlunoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<PoliticaAluno> getAlunoPolitica(int cdPolitica, int cdEscola)
        {
            try
            {
                var retorno = (from politicaAluno in db.PoliticaAluno
                               where politicaAluno.cd_politica_desconto == cdPolitica &&
                               politicaAluno.PoliticaDesconto.cd_pessoa_escola == cdEscola
                               select new
                               {
                                   politicaAluno.cd_aluno,
                                   politicaAluno.Aluno.AlunoPessoaFisica.no_pessoa,
                                   politicaAluno.cd_politica_aluno,
                                   politicaAluno.cd_politica_desconto
                               }).ToList().Select(x => new PoliticaAluno
                               {
                                   cd_aluno = x.cd_aluno,
                                   no_aluno = x.no_pessoa,
                                   cd_politica_aluno = x.cd_politica_aluno,
                                   cd_politica_desconto = x.cd_politica_desconto

                               });
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<PoliticaAluno> getAlunoPoliticaFull(int cdPolitica, int cdEscola)
        {
            try
            {
                var retorno = from politicaAluno in db.PoliticaAluno
                              where politicaAluno.cd_politica_desconto == cdPolitica &&
                              politicaAluno.PoliticaDesconto.cd_pessoa_escola == cdEscola
                              select politicaAluno;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}