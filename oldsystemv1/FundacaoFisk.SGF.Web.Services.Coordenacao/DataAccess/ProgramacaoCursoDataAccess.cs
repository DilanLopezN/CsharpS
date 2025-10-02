using System;
using System.Collections.Generic;
using System.Linq;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using System.Data;
using Componentes.GenericDataAccess.GenericException;


namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class ProgramacaoCursoDataAccess : GenericRepository<ProgramacaoCurso>, IProgramacaoCursoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<ProgramacaoCursoUI> getProgramacaoCursoSearch(SearchParameters parametros, int? cdCurso, int? cdDuracao, int? cd_escola)
        {
            try{
                IEntitySorter<ProgramacaoCursoUI> sorter = EntitySorter<ProgramacaoCursoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<ProgramacaoCurso> sql = from c in db.ProgramacaoCurso.AsNoTracking()
                          select c;

                if(!cd_escola.HasValue)
                    sql = from c in sql
                          where c.cd_escola == null
                          select c;
                else
                    sql = from c in sql
                          where c.cd_escola == cd_escola.Value
                          select c;

                IQueryable<ProgramacaoCursoUI> sql1 = from c in sql
                          select new ProgramacaoCursoUI
                          {
                              cd_programacao_curso = c.cd_programacao_curso,
                              cd_curso = c.cd_curso,
                              cd_duracao = c.cd_duracao,
                              no_curso = c.Curso.no_curso,
                              dc_duracao = c.Duracao.dc_duracao,
                              nm_aula_programacao = c.ItemProgramacao.Where(ip => ip.cd_programacao_curso == c.cd_programacao_curso).Max(n => n.nm_aula_programacao)
                          };

                sql1 = sorter.Sort(sql1);

                var retorno = from c in sql1
                              select c;
                //Filtrando por Curso
                if (cdCurso > 0)
                    retorno = from c in retorno
                              where c.cd_curso == cdCurso
                              select c;
                //Filtrando por Regime
                if (cdDuracao > 0)
                    retorno = from c in retorno
                              where c.cd_duracao == cdDuracao
                              select c;

                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteAllProgramacoesCursos(List<ProgramacaoCurso> programacoesCursos)
        {
            try{
                foreach(ProgramacaoCurso programacaoCurso in programacoesCursos) {
                    ProgramacaoCurso programacaoCursoContext = this.findById(programacaoCurso.cd_programacao_curso, false);
                    this.deleteContext(programacaoCursoContext, false);
                }
                return this.saveChanges(false) > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ProgramacaoCursoUI GetProgramacaoById(int cdProgramacao)
        {
            try{
                var sql = (from c in db.ProgramacaoCurso
                          where c.cd_programacao_curso == cdProgramacao
                          select new ProgramacaoCursoUI
                          {
                              cd_programacao_curso = c.cd_programacao_curso,
                              cd_curso = c.cd_curso,
                              cd_duracao = c.cd_duracao,
                              no_curso = c.Curso.no_curso,
                              dc_duracao = c.Duracao.dc_duracao,
                              nm_aula_programacao = c.ItemProgramacao.Where(ip => ip.cd_programacao_curso == c.cd_programacao_curso).Max(n => n.nm_aula_programacao)
                          }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ProgramacaoCurso getProgramacao(int cdCurso, int cdDuracao, int? cd_escola)
        {
            try{
                var sql1 = from prog in db.ProgramacaoCurso.Include(p => p.ItemProgramacao)
                          where
                            prog.cd_curso == cdCurso &&
                            prog.cd_duracao == cdDuracao
                          select prog;

                if(cd_escola.HasValue)
                    sql1 = from p in sql1
                           where p.cd_escola == cd_escola.Value
                           select p;
                else
                    sql1 = from p in sql1
                           where p.cd_escola == null
                           select p;

                return sql1.FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //retorna o primeiro registro ou o default
        public ProgramacaoCurso firstOrDefault()
        {
            try{
                var sql = (from prog in db.ProgramacaoCurso
                           select prog).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeModeloProgramacaoByTurma(int cd_turma, int cd_escola) {
            try {
                var sql = from t in db.Turma
                           where t.cd_pessoa_escola == cd_escola
                           && t.cd_turma == cd_turma
                           && (from pc in db.ProgramacaoCurso 
                               where pc.cd_curso == t.cd_curso 
                                    && pc.cd_duracao == t.cd_duracao
                                    && pc.cd_escola == cd_escola
                               select pc).Any()
                           select t;
                return sql.Any();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }
    }
}
