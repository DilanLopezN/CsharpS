using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.GenericException;


namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class AtividadeCursoDataAccess : GenericRepository<AtividadeCurso>, IAtividadeCursoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }
        

        public IEnumerable<AtividadeCursoUI> searchAtividadeCurso(int cdAtividadeExtra, int cdEscola)
        {
            try
            {
                var sql = from atividadeCurso in db.AtividadeCurso
                          join curso in db.Curso on atividadeCurso.cd_curso equals curso.cd_curso                          
                          where atividadeCurso.cd_atividade_extra == cdAtividadeExtra
                             && atividadeCurso.cd_pessoa_escola == cdEscola
                          select new AtividadeCursoUI
                          {
                              cd_atividade_curso = atividadeCurso.cd_atividade_curso,
                              cd_curso = atividadeCurso.cd_curso,
                              cd_atividade_extra = atividadeCurso.cd_atividade_extra,
                              cd_pessoa_escola = atividadeCurso.cd_pessoa_escola,
                          };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AtividadeCurso> searchAtividadesCurso(int cdAtividadeExtra, int cdEscola)
        {
            try
            {
                var sql = from atividadeCurso in db.AtividadeCurso
                          join curso in db.Curso on atividadeCurso.cd_curso equals curso.cd_curso
                          where atividadeCurso.cd_atividade_extra == cdAtividadeExtra
                             && atividadeCurso.cd_pessoa_escola == cdEscola
                          select new AtividadeCurso
                          {
                              cd_atividade_curso = atividadeCurso.cd_atividade_curso,
                              cd_curso = atividadeCurso.cd_curso,
                              cd_atividade_extra = atividadeCurso.cd_atividade_extra,
                              cd_pessoa_escola = atividadeCurso.cd_pessoa_escola,
                          };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public List<AtividadeCurso> searchAtividadesCursoBycdAtividadExtra(int cdAtividadeExtra, int cdEscola)
        {
            try
            {

                List<AtividadeCurso> sql = (from a in db.AtividadeCurso
                    where a.cd_atividade_extra == cdAtividadeExtra
                          && a.cd_pessoa_escola == cdEscola
                    select a).ToList();

                
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}
