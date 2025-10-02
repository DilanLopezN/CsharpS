using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using Componentes.Utils;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class TurmaDataAccess : GenericRepository<Turma>, ITurmaDataAccess
    {
        public enum TipoConsultaTurmaEnum
        {
            HAS_ATIVO = 0,
            HAS_POLITICADESCONTO = 1,
            TODAS_PPT_FILHAS = 3,
            TODAS = 4,
            EXCETO_ENCERRADAS = 5,
            HAS_BUSCA_DADOS_PPT = 6,
            HAS_BUSCA_TURMA = 7,
            SALA = 8,
            CURSO = 9,
            DESISTENCIA = 10,
            CANCELA_DESISTENCIA = 11,
            DIARIO_AULA = 12,
            HAS_PESQ_PRINC_TURMA = 13,
            HAS_PESQ_FK_TURMA_PROF = 14,
            HAS_PESQ_NOTA_MATERIAL = 15,
            HAS_PESQ_AVALICAO_TURMA = 16,
            HAS_PESQ_REPORT_AVALIACAO = 17,
            HAS_PESQ_REPORT_AVALIACAO_CONCEITO = 18,
            AULAREPOSICAO = 28,
            AULAREPOSICAOCADASTRO = 29,
            TURMAFOLLOWUP = 30,
            REPORT_AVALIACAO = 31,
            AULAREPOSICAOCADASTROTURMADESTINO = 32,
            REPORT_AVALIACAO_CONCEITO = 33,
            SEM_DIARIO = 40,
            SEM_DIARIO_COM_ESCOLA = 41,
            CARGA_HORARIA = 42,
            CARGA_HORARIA_COM_ESCOLA = 43,
            DESISTENCIA_CARGA = 50
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<TurmaSearchUI> getTurmaDesc(Componentes.Utils.SearchParameters parametros, string desc, bool inicio, int cdEscola,
           TipoConsultaTurmaEnum tipo, int cdPolitica)
        {
            try{
                IEntitySorter<TurmaSearchUI> sorter = EntitySorter<TurmaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<TurmaSearchUI> sql;
                IQueryable<Turma> sql0;
                if(cdPolitica == 0)
                    sql0 = from turma in db.Turma.AsNoTracking()
                           where turma.dt_termino_turma == null && turma.id_turma_ppt == false
                           select turma;
                else
                    sql0 = from turma in db.Turma.AsNoTracking()
                           where (turma.dt_termino_turma == null && turma.id_turma_ppt == false) || turma.PoliticasTurmas.Where(p => p.cd_politica_desconto == cdPolitica).Any()
                           select turma;
                sql = from turma in sql0
                      where turma.cd_pessoa_escola == cdEscola
                      select new TurmaSearchUI
                      {
                          cd_turma = turma.cd_turma,
                          cd_pessoa_escola = turma.cd_pessoa_escola,
                          no_turma = turma.no_turma
                      };

                sql = sorter.Sort(sql);
                var retorno = from turma in sql
                              select turma;
                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        retorno = from turma in sql
                                  where turma.no_turma.StartsWith(desc)
                                  select turma;
                    }//end if
                    else
                    {
                        retorno = from turma in sql
                                  where turma.no_turma.Contains(desc)
                                  select turma;
                    }//end else

                }

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

        public IEnumerable<Turma> findTurma(int cdEscola, int? cd_turma,TipoConsultaTurmaEnum tipo)
        {
            try{
                IQueryable<Turma> retorno = null;

                retorno = from turma in db.Turma
                          where turma.cd_pessoa_escola == cdEscola
                          select turma;

                switch(tipo) {
                    case TipoConsultaTurmaEnum.TODAS:
                        retorno = from turma in retorno
                                  select turma;
                        break;
                    case TipoConsultaTurmaEnum.TODAS_PPT_FILHAS:
                        if (cd_turma != null && cd_turma > 0)
                            retorno = from turma in retorno.Include(x => x.TurmaAluno)
                                      where turma.cd_turma_ppt == cd_turma
                                      select turma;
                        break;
                }

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Turma firstOrDefault(int cdEscola)
        {
            try{
                var sql = (from c in db.Turma
                           where c.cd_pessoa_escola == cdEscola
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TurmaSearch> searchTurma(SearchParameters parametros, string descricao, string apelido,  bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
                                                   int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas, int cdAluno, int origemFK, DateTime? dtInicial,
                                                   DateTime? dtFinal, int? cd_turma_PPT, bool semContrato, int tipoConsulta, DateTime? dt_inicial, DateTime? dt_final, bool ProfTurmasAtuais,
                                                   int cd_search_sala, int cd_search_sala_online, bool ckSearchSemSala, bool ckSearchSemAluno,
                                                   List<int> cdSituacoesAlunoTurma, int cd_escola_combo_fk, int diaSemanaTurma, int ckOnLine, string dias)
        {
            try
            {
                

                IEntitySorter<TurmaSearch> sorter = EntitySorter<TurmaSearch>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Turma> sql;

                sql = from t in db.Turma.AsNoTracking()
                    where (t.cd_pessoa_escola == cdEscola ||
                            (from te in db.TurmaEscola
                                where te.cd_turma == t.cd_turma &&
                                    te.cd_escola == cdEscola
                                select te).Any())
                    select t;

                if (ckOnLine == 1)
                {
                    sql = from t in sql
                          where t.cd_sala_online ==  null
                          select t;
                }
                if (ckOnLine == 2)
                {
                    sql = from t in sql
                          where t.cd_sala_online != null
                          select t;
                }
                if (origemFK == (int)TipoConsultaTurmaEnum.AULAREPOSICAO)
                {
                    sql = from t in sql
                          where t.AulaReposicao.Any() 
                           select t;
                }
                if (origemFK == (int)TipoConsultaTurmaEnum.AULAREPOSICAOCADASTRO || origemFK == (int)TipoConsultaTurmaEnum.TURMAFOLLOWUP) 
                {
                    if (ckOnLine == 0 && origemFK == (int)TipoConsultaTurmaEnum.TURMAFOLLOWUP)
                        sql = from t in sql
                              where t.cd_sala_online != null
                              select t;

                    if (diaSemanaTurma > 0)
                    {
                        sql = from t in sql
                              where db.Horario.Any(h => //h.cd_pessoa_escola == cdEscola && 
                                  h.id_origem == (int)Horario.Origem.TURMA && h.cd_registro == t.cd_turma &&
                                  h.id_dia_semana == diaSemanaTurma)
                              select t;
                    }
                }
                if (origemFK == (int)TipoConsultaTurmaEnum.SEM_DIARIO)
                {
                    sql = from t in sql
                          where db.vi_raf_sem_diario.Where(v => v.cd_turma == t.cd_turma).Any()
                          select t;
                }
                if (origemFK == (int)TipoConsultaTurmaEnum.SEM_DIARIO_COM_ESCOLA)
                {
                    sql = from t in sql
                          where db.vi_raf_sem_diario.Where(v => v.cd_turma == t.cd_turma && v.cd_pessoa_escola == cdEscola).Any()
                          select t;
                }
                if (origemFK == (int)TipoConsultaTurmaEnum.DESISTENCIA_CARGA)
                {
                    sql = from t in sql
                          where db.vi_desistencia_carga.Where(v => v.cd_turma == t.cd_turma).Any()
                          select t;
                }
                if (origemFK == (int)TipoConsultaTurmaEnum.CARGA_HORARIA)
                {
                    sql = from t in sql
                          where db.vi_carga_horaria.Where(v => (v.cd_turma == t.cd_turma) || (v.cd_turma_ppt == t.cd_turma)).Any()
                          select t;
                }
                if (origemFK == (int)TipoConsultaTurmaEnum.CARGA_HORARIA_COM_ESCOLA)
                {
                    sql = from t in sql
                          where db.vi_carga_horaria.Where(v => ((v.cd_turma == t.cd_turma) || (v.cd_turma_ppt == t.cd_turma)) && v.cd_escola == cdEscola).Any()
                          select t;
                }



                if (dias.Contains("1"))
                        sql = from t in sql
                              where db.Horario.Any(h => //h.cd_pessoa_escola == cdEscola && 
                                  h.id_origem == (int)Horario.Origem.TURMA && h.cd_registro == t.cd_turma &&
                                  dias.Substring(h.id_dia_semana-1,1) == "1")
                              select t;

                if (dt_inicial.HasValue && dt_final.HasValue)
                {
                    //os turmas que possuem títulos abertos, que não possuem baixas e nem Cnab emitido, no período selecionado
                    SGFWebContext dbContext = new SGFWebContext();
                    int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                    sql = (from t in sql
                          join at in db.AlunoTurma on t.cd_turma equals at.cd_turma
                          join c in db.Contrato on at.cd_contrato equals c.cd_contrato 
                          where db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                                                    ti.id_origem_titulo == cd_origem &&
                                                    c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&
                                                    ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.ABERTO &&
                                                    ti.id_status_cnab == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                                                    && ti.dt_vcto_titulo >= dt_inicial.Value
                                                    && ti.dt_vcto_titulo <= dt_final.Value
                                                    && !ti.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                                  ).Any()
                                  //&&
                                  //!db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                                  //                  ti.id_origem_titulo == cd_origem &&
                                  //                  c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&
                                                    
                                  //                  ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.FECHADO
                                  //                  && ti.dt_vcto_titulo >= dt_inicial.Value
                                  //                  && ti.dt_vcto_titulo <= dt_final.Value
                                  //).Any()
                                  //&&
                                  //!db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                                  //                  ti.id_origem_titulo == cd_origem &&
                                  //                  c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&
                                                    
                                  //                  ti.id_status_cnab != (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                                  //                  && ti.dt_vcto_titulo >= dt_inicial.Value
                                  //                  && ti.dt_vcto_titulo <= dt_final.Value
                                  //).Any()
                          select t).Distinct();
                }
                else if (dt_inicial.HasValue)
                {
                    //os turmas que possuem títulos abertos, que não possuem baixas e nem Cnab emitido, no período selecionado
                    SGFWebContext dbContext = new SGFWebContext();
                    int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                    sql = (from t in sql
                           join at in db.AlunoTurma on t.cd_turma equals at.cd_turma
                           join c in db.Contrato on at.cd_contrato equals c.cd_contrato
                           where db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                                                    ti.id_origem_titulo == cd_origem &&
                                                    c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&
                                                    ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.ABERTO &&
                                                    ti.id_status_cnab == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                                                    && ti.dt_vcto_titulo >= dt_inicial.Value
                                                    && !ti.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                 x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                                   ).Any()
                                   //&&
                                   //!db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                                   //                  ti.id_origem_titulo == cd_origem &&
                                   //                  c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&

                                   //                  ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.FECHADO
                                   //                  && ti.dt_vcto_titulo >= dt_inicial.Value
                                   //).Any()
                                   //&&
                                   //!db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                                   //                  ti.id_origem_titulo == cd_origem &&
                                   //                  c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&

                                   //                  ti.id_status_cnab != (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                                   //                  && ti.dt_vcto_titulo >= dt_inicial.Value
                                   //).Any()
                           select t).Distinct();
                }
                if (!string.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql = from t in sql
                              where t.no_turma.StartsWith(descricao)
                              select t;
                    else
                        sql = from t in sql
                              where t.no_turma.Contains(descricao)
                              select t;
                if (!string.IsNullOrEmpty(apelido))
                    if (inicio)
                        sql = from t in sql
                              where t.no_apelido.StartsWith(apelido)
                              select t;
                    else
                        sql = from t in sql
                              where t.no_apelido.Contains(apelido)
                              select t;
                if(cdAluno > 0)
                    sql = from t in sql
                          where t.TurmaAluno.Where(a => a.cd_aluno == cdAluno).Any()
                          select t;

                if (tipoConsulta != (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_REPORT_AVALIACAO && tipoConsulta != (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_REPORT_AVALIACAO_CONCEITO)
                {
                    if (dtInicial.HasValue)
                        sql = from t in sql
                              where t.dt_inicio_aula >= dtInicial
                              select t;

                    if (dtFinal.HasValue)
                        sql = from t in sql
                              where t.dt_inicio_aula <= dtFinal
                              select t;
                }
                if (semContrato)
                    sql = from t in sql
                          where db.AlunoTurma.Where(at => at.cd_turma == t.cd_turma && at.cd_contrato == null && at.cd_turma_origem > 0).Any()
                          select t;
                //Busca as turmas filhas aprtir do código da turma pai PPT
                if (cd_turma_PPT != null && cd_turma_PPT > 0)
                {
                    // Para trazer o nome do aluno e não trazer o nome do curso.
                    turmasFilhas = true;
                    sql = from t in sql
                          where t.cd_turma_ppt == cd_turma_PPT
                          select t;
                }
                else
                {
                    if (tipoTurma > 0)
                    {
                        if (tipoTurma == (int)Turma.TipoTurma.PPT)
                        {
                            if (!turmasFilhas && situacaoTurma > 0)
                            {
                                var ativo = true;
                                if (situacaoTurma == 2)
                                    ativo = false;
                                sql = from t in sql
                                      where t.id_turma_ativa == ativo
                                      select t;
                            }
                            if (turmasFilhas)
                            {
                                sql = from t in sql
                                      where t.cd_turma_ppt != null && t.id_turma_ppt == false
                                      select t;
                                if (situacaoTurma > 0)
                                {
                                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                                        sql = from t in sql
                                              where t.dt_termino_turma == null
                                              && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                              select t;
                                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                                        sql = from t in sql
                                              where t.dt_termino_turma != null
                                              select t;
                                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                                        sql = from t in sql
                                              where t.dt_termino_turma == null 
                                              && !t.DiarioAula.Any(x=> x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                              select t;
                                }
                            }
                            else
                                sql = from t in sql
                                      where t.id_turma_ppt == true && t.cd_turma_ppt == null
                                      select t;

                        }
                        if (tipoTurma == (int)Turma.TipoTurma.NORMAL)
                        {
                            sql = from t in sql
                                  where t.id_turma_ppt == false &&
                                     t.cd_turma_ppt == null
                                  select t;
                            if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                                sql = from t in sql
                                      where t.dt_termino_turma == null
                                      && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                      select t;
                            if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                                sql = from t in sql
                                      where t.dt_termino_turma != null
                                      select t;
                            if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                                sql = from t in sql
                                      where t.dt_termino_turma == null
                                      && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                      //&& !t.AvaliacaoTurma.Any()
                                      select t;
                        }
                    }
                    else
                    {
                        sql = from t in sql
                              where !t.id_turma_ppt
                              select t;
                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                        sql = from t in sql
                              where t.dt_termino_turma == null
                              && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                              select t;
                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                        sql = from t in sql
                              where t.dt_termino_turma != null
                              select t;
                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                        sql = from t in sql
                              where t.dt_termino_turma == null
                              && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                              //&& !t.AvaliacaoTurma.Any()
                              select t;
                    }
                }

                if (cdCurso > 0)
                    sql = from t in sql
                          where t.cd_curso == cdCurso
                          select t;
                if (cdDuracao > 0)
                    sql = from t in sql
                          where t.cd_duracao == cdDuracao
                          select t;
                if (cdProduto > 0)
                    sql = from t in sql
                          where t.cd_produto == cdProduto
                          select t;

                switch (tipoProg)
                {
                    case ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_GERADAS:
                        sql = from t in sql
                              where t.ProgramacaoTurma.Any()
                              select t;
                        break;
                    case ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_NAO_GERADAS:
                        sql = from t in sql
                              where !t.ProgramacaoTurma.Any()
                              select t;
                        break;
                }

                if (ckSearchSemAluno)
                {
                    sql = from t in sql
                        where (t.id_turma_ppt == true ? (from turma in db.Turma
                                where turma.cd_pessoa_escola == cdEscola && turma.cd_turma_ppt == t.cd_turma &&
                                      turma.TurmaAluno.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                   at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                   at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial).Any()
                                select turma.cd_turma).Count() :
                            t.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola &&
                                                     (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                      at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                      at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Count()
                                                      ) == 0
                          select t;
                }

                if (ckSearchSemSala)
                {
                    sql = from t in sql
                        where t.cd_sala == null && t.cd_sala_online == null
                        select t;
                }
                else
                {
                    if (cd_search_sala > 0)
                    {
                        sql = from t in sql
                            where t.cd_sala == cd_search_sala 
                            select t;
                    }

                    if (cd_search_sala_online > 0)
                    {
                        sql = from t in sql
                            where t.cd_sala_online == cd_search_sala_online
                              select t;
                    }

                }

                switch (tipoConsulta)
                {
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA:
                        if (cdProfessor > 0)
                            if (ProfTurmasAtuais)
                                sql = from t in sql
                                      where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo && pt.Turma.dt_termino_turma == null).Any()
                                      select t;
                            else
                                sql = from t in sql
                                      where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor).Any()
                                      select t;
                        break;
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_FK_TURMA_PROF:
                        if (cdProfessor > 0)
                            sql = from t in sql
                                  where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo).Any()
                                  select t;
                        break;
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_NOTA_MATERIAL:
                        SGFWebContext dbComp = new SGFWebContext();
                        int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                        sql = from t in sql
                                  where t.TurmaAluno.Any(ta => ta.Aluno.Movimento.Any(m => m.id_origem_movimento == cd_origem)) 
                                  select t;
                        if (cdProfessor > 0)
                            sql = from t in sql
                                  where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo).Any()
                                  select t;
                        break;
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_AVALICAO_TURMA:
                        sql = from t in sql
                                  where !t.AvaliacaoTurma.Any()
                                  select t;
                        if (cdProfessor > 0)
                            sql = from t in sql
                                  where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo).Any()
                                  select t;
                        break;
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_REPORT_AVALIACAO:
                        if (!dtInicial.HasValue && !dtFinal.HasValue)
                            sql = from t in sql
                                  where t.AvaliacaoTurma.Any(av => av.AvaliacaoAluno.Where(aa => aa.nm_nota_aluno != null).Any() && !av.Avaliacao.CriterioAvaliacao.id_conceito)
                                  select t;
                        else
                        {
                            if (dtInicial.HasValue && !dtFinal.HasValue)
                                sql = from t in sql
                                      where t.AvaliacaoTurma.Any(av => av.dt_avaliacao_turma >= dtInicial && av.AvaliacaoAluno.Where(aa => aa.nm_nota_aluno != null).Any() && !av.Avaliacao.CriterioAvaliacao.id_conceito)
                                      select t;
                            if (!dtInicial.HasValue && dtFinal.HasValue)
                                sql = from t in sql
                                      where t.AvaliacaoTurma.Any(av => av.dt_avaliacao_turma <= dtFinal && av.AvaliacaoAluno.Where(aa => aa.nm_nota_aluno != null).Any() && !av.Avaliacao.CriterioAvaliacao.id_conceito)
                                      select t;
                            if (dtInicial.HasValue && dtFinal.HasValue)
                                sql = from t in sql
                                      where t.AvaliacaoTurma.Any(av => av.dt_avaliacao_turma >= dtInicial && av.dt_avaliacao_turma <= dtFinal && av.AvaliacaoAluno.Where(aa => aa.nm_nota_aluno != null).Any() && !av.Avaliacao.CriterioAvaliacao.id_conceito)
                                      select t;
                        }
                        break;
                        case (int)TipoConsultaTurmaEnum.HAS_PESQ_REPORT_AVALIACAO_CONCEITO:
                        if (!dtInicial.HasValue && !dtFinal.HasValue)
                            sql = from t in sql
                                  where t.AvaliacaoTurma.Any(av => av.AvaliacaoAluno.Where(aa => aa.cd_conceito != null).Any() && av.Avaliacao.CriterioAvaliacao.id_conceito)
                                  select t;
                        else
                        {
                            if (dtInicial.HasValue && !dtFinal.HasValue)
                                sql = from t in sql
                                      where t.AvaliacaoTurma.Any(av => av.dt_avaliacao_turma >= dtInicial && av.AvaliacaoAluno.Where(aa => aa.cd_conceito != null).Any() && av.Avaliacao.CriterioAvaliacao.id_conceito)
                                      select t;
                            if (!dtInicial.HasValue && dtFinal.HasValue)
                                sql = from t in sql
                                      where t.AvaliacaoTurma.Any(av => av.dt_avaliacao_turma <= dtFinal && av.AvaliacaoAluno.Where(aa => aa.cd_conceito != null).Any() && av.Avaliacao.CriterioAvaliacao.id_conceito)
                                      select t;
                            if (dtInicial.HasValue && dtFinal.HasValue)
                                sql = from t in sql
                                      where t.AvaliacaoTurma.Any(av => av.dt_avaliacao_turma >= dtInicial && av.dt_avaliacao_turma <= dtFinal && av.AvaliacaoAluno.Where(aa => aa.cd_conceito != null).Any() && av.Avaliacao.CriterioAvaliacao.id_conceito)
                                      select t;
                        }
                        break;

                }

                if (cdSituacoesAlunoTurma != null && cdSituacoesAlunoTurma.Count() > 0)
                    sql = from t in sql
                          where t.TurmaAluno.Any(ta => cdSituacoesAlunoTurma.Contains((int)ta.cd_situacao_aluno_turma))
                          select t;

                

                var sqlSearch = (from t in sql.AsNoTracking()
                                 select new 
                                 {
                                     cd_turma = t.cd_turma,
                                     cd_turma_ppt = t.cd_turma_ppt,
                                     no_turma = t.no_turma,
                                     no_apelido = t.no_apelido,
                                     no_curso = turmasFilhas ? "" : t.Curso.no_curso,
                                     no_duracao = t.Duracao.dc_duracao,
                                     no_produto = t.Produto.no_produto,
                                     cd_estagio = (t.cd_curso != null) ? t.Curso.cd_estagio : (int?)null,
                                     cd_curso = t.cd_curso,
                                     cd_produto = t.cd_produto,
                                     dt_inicio_aula = t.dt_inicio_aula,
                                     id_turma_ppt = t.id_turma_ppt,
                                     id_turma_ativa = t.id_turma_ativa,
                                     dta_termino_turma = t.dt_termino_turma,
                                     cd_pessoa_escola = t.cd_pessoa_escola,
                                     nro_alunos = t.id_turma_ppt == true ? (from turma in db.Turma
                                                                            where turma.cd_pessoa_escola == cdEscola && turma.cd_turma_ppt == t.cd_turma &&
                                                                            turma.TurmaAluno.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                                    at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado).Any()
                                                                            select turma.cd_turma).Count() :
                                     t.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola &&
                                                              (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                               at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                               at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Count(),
                                     no_professor = t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true && p.cd_turma == t.cd_turma).FirstOrDefault().Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                                     no_aluno = turmasFilhas ? t.TurmaAluno.FirstOrDefault().Aluno.AlunoPessoaFisica.no_pessoa : "",
                                     turmasFilhas = turmasFilhas,
                                     cd_turma_enc = t.cd_turma_enc,
                                     existe_diario = t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada),
                                     nm_alunos_turmaPPT = turmasFilhas ? (from turma in db.Turma
                                                                          where turma.cd_turma_ppt == t.cd_turma_ppt &&
                                                                          turma.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                                       at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)).Any()
                                                                          select turma.cd_turma).Count() : 0,
                                     vagas_disponiveis = turmasFilhas ? t.TurmaPai.T_SALA != null ? t.TurmaPai.T_SALA.nm_vaga_sala : t.TurmaPai.Sala != null ? t.TurmaPai.Sala.nm_vaga_sala :
                                                         t.T_SALA != null ? t.Curso != null ? t.T_SALA.nm_vaga_sala <= t.Curso.nm_vagas_curso ? t.T_SALA.nm_vaga_sala : (int)t.Curso.nm_vagas_curso : (int)t.T_SALA.nm_vaga_sala :
                                                         t.Sala != null ? t.Curso != null ? t.Sala.nm_vaga_sala <= t.Curso.nm_vagas_curso ? t.Sala.nm_vaga_sala : (int)t.Curso.nm_vagas_curso : (int)t.Sala.nm_vaga_sala :
                                                         t.Curso != null ? (int)t.Curso.nm_vagas_curso : 0 : 0,
                                     //(turmasFilhas && t.TurmaPai.Sala != null ? t.TurmaPai.Sala.nm_vaga_sala : t.Sala != null ? t.Curso != null ? t.Sala.nm_vaga_sala < t.Curso.nm_vagas_curso ? t.Sala.nm_vaga_sala : 
                                     //(int)t.Curso.nm_vagas_curso :// (int)t.Sala.nm_vaga_sala : t.Curso != null ? (int)t.Curso.nm_vagas_curso : 0
                                     horarios = (from horario in db.Horario
                                                 where //horario.cd_pessoa_escola == cdEscola && 
                                                     horario.id_origem == (int)Horario.Origem.TURMA &&
                                                     horario.cd_registro == t.cd_turma &&
                                                     (diaSemanaTurma == 0 || horario.id_dia_semana == diaSemanaTurma)
                                                 orderby horario.dt_hora_ini
                                                 select horario).ToList(),
                                     dc_escola_origem = t.cd_pessoa_escola != cdEscola ? t.EscolaTurma.dc_reduzido_pessoa : ""
                                 }).ToList().Select(x => new TurmaSearch
                                {
                                    cd_turma = x.cd_turma,
                                    cd_turma_ppt = x.cd_turma_ppt,
                                    no_turma = x.no_turma,
                                    no_apelido = x.no_apelido,
                                    no_curso = x.no_curso,
                                    no_duracao = x.no_duracao,
                                    no_produto = x.no_produto,
                                    cd_estagio = x.cd_estagio,
                                    cd_curso = x.cd_curso,
                                    cd_produto = x.cd_produto,
                                    dt_inicio_aula = x.dt_inicio_aula,
                                    id_turma_ppt = x.id_turma_ppt,
                                    id_turma_ativa = x.id_turma_ativa,
                                    dta_termino_turma = x.dta_termino_turma,
                                    cd_pessoa_escola = x.cd_pessoa_escola,
                                    nro_alunos = x.nro_alunos,
                                    no_professor = x.no_professor,
                                    no_aluno = x.no_aluno,
                                    turmasFilhas = x.turmasFilhas,
                                    cd_turma_enc = x.cd_turma_enc,
                                    existe_diario = x.existe_diario,
                                    nm_alunos_turmaPPT = x.nm_alunos_turmaPPT,
                                    vagas_disponiveis = x.vagas_disponiveis,
                                    horarios = x.horarios,
                                    dc_escola_origem = x.dc_escola_origem
                                 }).AsQueryable(); 

                if (origemFK == 30)
                {
                    sqlSearch = sqlSearch.Where(x => x.horarios.Count() > 0);
                }

                sqlSearch = sorter.Sort(sqlSearch);

                int limite = sqlSearch.Select(x=> x.cd_turma).Count();
                parametros.ajustaParametrosPesquisa(limite);
                sqlSearch = sqlSearch.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                var retorno = sqlSearch;
                

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TurmaSearch> getTurmaSearchAulaReposicaoDestinoFK(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
                                                   int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas, int cdAluno, int origemFK, DateTime? dtInicial,
                                                   DateTime? dtFinal, int? cd_turma_PPT, bool semContrato, int tipoConsulta, DateTime? dt_inicial, DateTime? dt_final, bool ProfTurmasAtuais, DateTime? dt_programacao,
                                                   int cd_estagio, int cd_turma_origem, List<int> cdSituacoesAlunoTurma, bool ckOnLine)
        {
            try
            {


                IEntitySorter<TurmaSearch> sorter = EntitySorter<TurmaSearch>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Turma> sql;

                sql = from t in db.Turma.AsNoTracking()
                      where (t.cd_pessoa_escola == cdEscola ||
                              (from te in db.TurmaEscola
                               where te.cd_turma == t.cd_turma &&
                                 te.cd_escola == cdEscola
                               select te).Any()) && 
                            t.dt_termino_turma == null //não pode ser turma encerrada
                      select t;

                //turma de destino diferente da turma de origem
                if (cd_turma_origem > 0)
                {
                    sql = from t in sql
                        where t.cd_turma != cd_turma_origem
                        select t;
                }

               
                if (ckOnLine == false)
                {
                    sql = from t in sql
                          where t.cd_sala_online == null
                          select t;
                }

                // é sala online
                if (ckOnLine == true)
                {
                    sql = from t in sql
                          where t.cd_sala_online != null
                          select t;
                }
                if (origemFK == 28)
                {
                    sql = from t in sql
                          where t.AulaReposicao.Any()
                          select t;
                }
                if (origemFK == 29 || origemFK == 30) //30 = FollowUp 
                {
                    sql = from t in sql
                          where t.cd_sala_online != null
                          select t;

                    
                }

                

                if (dt_inicial.HasValue && dt_final.HasValue)
                {
                    //os turmas que possuem títulos abertos, que não possuem baixas e nem Cnab emitido, no período selecionado
                    SGFWebContext dbContext = new SGFWebContext();
                    int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                    sql = (from t in sql
                           join at in db.AlunoTurma on t.cd_turma equals at.cd_turma
                           join c in db.Contrato on at.cd_contrato equals c.cd_contrato
                           where db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                                                     ti.id_origem_titulo == cd_origem &&
                                                     c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&
                                                     ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.ABERTO &&
                                                     ti.id_status_cnab == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                                                     && ti.dt_vcto_titulo >= dt_inicial.Value
                                                     && ti.dt_vcto_titulo <= dt_final.Value
                                                     && !ti.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                 x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                                   ).Any()
                           //&&
                           //!db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                           //                  ti.id_origem_titulo == cd_origem &&
                           //                  c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&

                           //                  ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.FECHADO
                           //                  && ti.dt_vcto_titulo >= dt_inicial.Value
                           //                  && ti.dt_vcto_titulo <= dt_final.Value
                           //).Any()
                           //&&
                           //!db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                           //                  ti.id_origem_titulo == cd_origem &&
                           //                  c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&

                           //                  ti.id_status_cnab != (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                           //                  && ti.dt_vcto_titulo >= dt_inicial.Value
                           //                  && ti.dt_vcto_titulo <= dt_final.Value
                           //).Any()
                           select t).Distinct();
                }
                else if (dt_inicial.HasValue)
                {
                    //os turmas que possuem títulos abertos, que não possuem baixas e nem Cnab emitido, no período selecionado
                    SGFWebContext dbContext = new SGFWebContext();
                    int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                    sql = (from t in sql
                           join at in db.AlunoTurma on t.cd_turma equals at.cd_turma
                           join c in db.Contrato on at.cd_contrato equals c.cd_contrato
                           where db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                                                    ti.id_origem_titulo == cd_origem &&
                                                    c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&
                                                    ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.ABERTO &&
                                                    ti.id_status_cnab == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                                                    && ti.dt_vcto_titulo >= dt_inicial.Value
                                                    && !ti.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                 x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                                   ).Any()
                           //&&
                           //!db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                           //                  ti.id_origem_titulo == cd_origem &&
                           //                  c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&

                           //                  ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.FECHADO
                           //                  && ti.dt_vcto_titulo >= dt_inicial.Value
                           //).Any()
                           //&&
                           //!db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                           //                  ti.id_origem_titulo == cd_origem &&
                           //                  c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&

                           //                  ti.id_status_cnab != (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                           //                  && ti.dt_vcto_titulo >= dt_inicial.Value
                           //).Any()
                           select t).Distinct();
                }
                if (!string.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql = from t in sql
                              where t.no_turma.StartsWith(descricao)
                              select t;
                    else
                        sql = from t in sql
                              where t.no_turma.Contains(descricao)
                              select t;
                if (!string.IsNullOrEmpty(apelido))
                    if (inicio)
                        sql = from t in sql
                              where t.no_apelido.StartsWith(apelido)
                              select t;
                    else
                        sql = from t in sql
                              where t.no_apelido.Contains(apelido)
                              select t;
                if (cdAluno > 0)
                    sql = from t in sql
                          where t.TurmaAluno.Where(a => a.cd_aluno == cdAluno).Any()
                          select t;

                if (tipoConsulta != (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_REPORT_AVALIACAO)
                {
                    if (dtInicial.HasValue)
                        sql = from t in sql
                              where t.dt_inicio_aula >= dtInicial
                              select t;

                    if (dtFinal.HasValue)
                        sql = from t in sql
                              where t.dt_inicio_aula <= dtFinal
                              select t;
                }
                if (semContrato)
                    sql = from t in sql
                          where db.AlunoTurma.Where(at => at.cd_turma == t.cd_turma && at.cd_contrato == null && at.cd_turma_origem > 0).Any()
                          select t;
                //Busca as turmas filhas aprtir do código da turma pai PPT
                if (cd_turma_PPT != null && cd_turma_PPT > 0)
                {
                    // Para trazer o nome do aluno e não trazer o nome do curso.
                    turmasFilhas = true;
                    sql = from t in sql
                          where t.cd_turma_ppt == cd_turma_PPT
                          select t;
                }
                else
                {
                    if (tipoTurma > 0)
                    {
                        if (tipoTurma == (int)Turma.TipoTurma.PPT)
                        {
                            if (!turmasFilhas && situacaoTurma > 0)
                            {
                                var ativo = true;
                                if (situacaoTurma == 2)
                                    ativo = false;
                                sql = from t in sql
                                      where t.id_turma_ativa == ativo
                                      select t;
                            }
                            if (turmasFilhas)
                            {
                                sql = from t in sql
                                      where t.cd_turma_ppt != null && t.id_turma_ppt == false
                                      select t;
                                if (situacaoTurma > 0)
                                {
                                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                                        sql = from t in sql
                                              where t.dt_termino_turma == null
                                              && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                              select t;
                                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                                        sql = from t in sql
                                              where t.dt_termino_turma != null
                                              select t;
                                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                                        sql = from t in sql
                                              where t.dt_termino_turma == null
                                              && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                              select t;
                                }
                            }
                            else
                                sql = from t in sql
                                      where t.id_turma_ppt == true && t.cd_turma_ppt == null
                                      select t;

                        }
                        if (tipoTurma == (int)Turma.TipoTurma.NORMAL)
                        {
                            sql = from t in sql
                                  where t.id_turma_ppt == false &&
                                     t.cd_turma_ppt == null
                                  select t;
                            if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                                sql = from t in sql
                                      where t.dt_termino_turma == null
                                      && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                      select t;
                            if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                                sql = from t in sql
                                      where t.dt_termino_turma != null
                                      select t;
                            if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                                sql = from t in sql
                                      where t.dt_termino_turma == null
                                      && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                      //&& !t.AvaliacaoTurma.Any()
                                      select t;
                        }
                    }
                    else
                    {
                        sql = from t in sql
                              where !t.id_turma_ppt
                              select t;
                        if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                            sql = from t in sql
                                  where t.dt_termino_turma == null
                                  && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                  select t;
                        if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                            sql = from t in sql
                                  where t.dt_termino_turma != null
                                  select t;
                        if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                            sql = from t in sql
                                  where t.dt_termino_turma == null
                                  && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                  //&& !t.AvaliacaoTurma.Any()
                                  select t;
                    }
                }

                if (cdCurso > 0)
                    sql = from t in sql
                          where (t.cd_turma_ppt == null && t.id_turma_ppt == true) || t.cd_curso == cdCurso
                          select t;
                if (cdDuracao > 0)
                    sql = from t in sql
                          where t.cd_duracao == cdDuracao
                          select t;
                if (cdProduto > 0)
                    sql = from t in sql
                          where t.cd_produto == cdProduto
                          select t;

                switch (tipoProg)
                {
                    case ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_GERADAS:
                        sql = from t in sql
                              where (t.cd_turma_ppt == null && t.id_turma_ppt == true) || t.ProgramacaoTurma.Any()
                              select t;
                        break;
                    case ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_NAO_GERADAS:
                        sql = from t in sql
                              where !t.ProgramacaoTurma.Any()
                              select t;
                        break;
                }


                // tem aula em determinada data
                if (dt_programacao.HasValue)
                {
                    sql = from t in sql
                        where (t.cd_turma_ppt == null && t.id_turma_ppt == true) || (t.ProgramacaoTurma.Any() && t.ProgramacaoTurma.Any(x => x.dta_programacao_turma == dt_programacao))
                          select t;
                }

                // mesmo estagio
                if (cd_estagio > 0)
                {
                    sql = from t in sql
                        where (t.cd_turma_ppt == null && t.id_turma_ppt == true) || t.Curso.cd_estagio == cd_estagio
                        select t;
                }


                   

                switch (tipoConsulta)
                {
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA:
                        if (cdProfessor > 0)
                            if (ProfTurmasAtuais)
                                sql = from t in sql
                                      where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo && pt.Turma.dt_termino_turma == null).Any()
                                      select t;
                            else
                                sql = from t in sql
                                      where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor).Any()
                                      select t;
                        break;
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_FK_TURMA_PROF:
                        if (cdProfessor > 0)
                            sql = from t in sql
                                  where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo).Any()
                                  select t;
                        break;
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_NOTA_MATERIAL:
                        SGFWebContext dbComp = new SGFWebContext();
                        int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                        sql = from t in sql
                              where t.TurmaAluno.Any(ta => ta.Aluno.Movimento.Any(m => m.id_origem_movimento == cd_origem))
                              select t;
                        if (cdProfessor > 0)
                            sql = from t in sql
                                  where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo).Any()
                                  select t;
                        break;
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_AVALICAO_TURMA:
                        sql = from t in sql
                              where !t.AvaliacaoTurma.Any()
                              select t;
                        if (cdProfessor > 0)
                            sql = from t in sql
                                  where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo).Any()
                                  select t;
                        break;
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_REPORT_AVALIACAO:
                        if (!dtInicial.HasValue && !dtFinal.HasValue)
                            sql = from t in sql
                                  where t.AvaliacaoTurma.Any(av => av.AvaliacaoAluno.Where(aa => aa.nm_nota_aluno != null).Any() && !av.Avaliacao.CriterioAvaliacao.id_conceito)
                                  select t;
                        else
                        {
                            if (dtInicial.HasValue && !dtFinal.HasValue)
                                sql = from t in sql
                                      where t.AvaliacaoTurma.Any(av => av.dt_avaliacao_turma >= dtInicial && av.AvaliacaoAluno.Where(aa => aa.nm_nota_aluno != null).Any() && !av.Avaliacao.CriterioAvaliacao.id_conceito)
                                      select t;
                            if (!dtInicial.HasValue && dtFinal.HasValue)
                                sql = from t in sql
                                      where t.AvaliacaoTurma.Any(av => av.dt_avaliacao_turma <= dtFinal && av.AvaliacaoAluno.Where(aa => aa.nm_nota_aluno != null).Any() && !av.Avaliacao.CriterioAvaliacao.id_conceito)
                                      select t;
                            if (dtInicial.HasValue && dtFinal.HasValue)
                                sql = from t in sql
                                      where t.AvaliacaoTurma.Any(av => av.dt_avaliacao_turma >= dtInicial && av.dt_avaliacao_turma <= dtFinal && av.AvaliacaoAluno.Where(aa => aa.nm_nota_aluno != null).Any() && !av.Avaliacao.CriterioAvaliacao.id_conceito)
                                      select t;
                        }
                        break;

                }

                if (cdSituacoesAlunoTurma != null && cdSituacoesAlunoTurma.Count() > 0)
                    sql = from t in sql
                          where t.TurmaAluno.Any(ta => cdSituacoesAlunoTurma.Contains((int)ta.cd_situacao_aluno_turma)) 
                          select t;

                var sqlSearch = (from t in sql.AsNoTracking()
                                 where t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true && p.cd_turma == t.cd_turma).Any()
                                 select new TurmaSearch
                                 {
                                     cd_turma = t.cd_turma,
                                     cd_turma_ppt = t.cd_turma_ppt,
                                     no_turma = t.no_turma,
                                     no_apelido = t.no_apelido,
                                     no_curso = turmasFilhas ? "" : t.Curso.no_curso,
                                     no_duracao = t.Duracao.dc_duracao,
                                     no_produto = t.Produto.no_produto,
                                     cd_curso = t.cd_curso,
                                     cd_estagio = (t.cd_curso != null) ? t.Curso.cd_estagio: (int?)null,
                                     cd_produto = t.cd_produto,
                                     dt_inicio_aula = t.dt_inicio_aula,
                                     id_turma_ppt = t.id_turma_ppt,
                                     id_turma_ativa = t.id_turma_ativa,
                                     dta_termino_turma = t.dt_termino_turma,
                                     cd_pessoa_escola = t.cd_pessoa_escola,
                                     nro_alunos = t.id_turma_ppt == true ? (from turma in db.Turma
                                                                            where turma.cd_pessoa_escola == cdEscola && turma.cd_turma_ppt == t.cd_turma &&
                                                                            turma.TurmaAluno.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                                    at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado).Any()
                                                                            select turma.cd_turma).Count() :
                                     t.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola &&
                                                              (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                               at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                               at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Count(),
                                     no_professor = t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true && p.cd_turma == t.cd_turma).FirstOrDefault().Professor.FuncionarioPessoaFisica.no_pessoa,
                                     cd_professor = t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true && p.cd_turma == t.cd_turma).Any() ? t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true && p.cd_turma == t.cd_turma).FirstOrDefault().cd_professor : 0,
                                     no_aluno = turmasFilhas ? t.TurmaAluno.FirstOrDefault().Aluno.AlunoPessoaFisica.no_pessoa : "",
                                     turmasFilhas = turmasFilhas,
                                     cd_turma_enc = t.cd_turma_enc,
                                     existe_diario = t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada),
                                     nm_alunos_turmaPPT = turmasFilhas ? (from turma in db.Turma
                                                                          where turma.cd_turma_ppt == t.cd_turma_ppt &&
                                                                          turma.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                                       at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                                       at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Any()
                                                                          select turma.cd_turma).Count() : 0,
                                     vagas_disponiveis = turmasFilhas ? t.TurmaPai.T_SALA != null ? t.TurmaPai.T_SALA.nm_vaga_sala : t.TurmaPai.Sala != null ? t.TurmaPai.Sala.nm_vaga_sala :
                                                         t.T_SALA != null ? t.Curso != null ? t.T_SALA.nm_vaga_sala <= t.Curso.nm_vagas_curso ? t.T_SALA.nm_vaga_sala : (int)t.Curso.nm_vagas_curso : (int)t.T_SALA.nm_vaga_sala :
                                                         t.Sala != null ? t.Curso != null ? t.Sala.nm_vaga_sala <= t.Curso.nm_vagas_curso ? t.Sala.nm_vaga_sala : (int)t.Curso.nm_vagas_curso : (int)t.Sala.nm_vaga_sala :
                                                         t.Curso != null ? (int)t.Curso.nm_vagas_curso : 0 : 0,
                                     //(turmasFilhas && t.TurmaPai.Sala != null ? t.TurmaPai.Sala.nm_vaga_sala : t.Sala != null ? t.Curso != null ? t.Sala.nm_vaga_sala < t.Curso.nm_vagas_curso ? t.Sala.nm_vaga_sala : 
                                     //(int)t.Curso.nm_vagas_curso :// (int)t.Sala.nm_vaga_sala : t.Curso != null ? (int)t.Curso.nm_vagas_curso : 0
                                     horarios = (from horario in db.Horario
                                                 where //horario.cd_pessoa_escola == cdEscola && 
                                                     horario.id_origem == (int)Horario.Origem.TURMA &&
                                                     horario.cd_registro == t.cd_turma 
                                                 orderby horario.dt_hora_ini
                                                 select horario).ToList(),
                                     cd_sala = t.cd_sala_online,
                                     no_sala = t.T_SALA.no_sala
                                 });

                if (origemFK == 30)
                {
                    sqlSearch = sqlSearch.Where(x => x.horarios.Count() > 0);
                }

                sqlSearch = sorter.Sort(sqlSearch);
                int limite = sqlSearch.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sqlSearch = sqlSearch.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                var retorno = sqlSearch.ToList();


                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Turma> findTurma(int idTurma, int idEscola)
        {
            var idAux = idTurma;
            try
            {
                var sql = (from turma in db.Turma
                           where (idTurma == idAux || turma.cd_turma == idTurma)
                              && (turma.cd_pessoa_escola == idEscola)
                           select new
                           {
                               no_turma = turma.no_turma,
                               cd_turma = turma.cd_turma
                           }).ToList().Select(x => new Turma { no_turma = x.no_turma, cd_turma = x.cd_turma });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TurmaApiCyberBdUI findTurmaApiCyber(int cd_turma, int cd_escola)
        {
            try
            {
                TurmaApiCyberBdUI sql = (from turma in db.Turma
                    where (turma.cd_turma == cd_turma) &&
                          (turma.cd_pessoa_escola == cd_escola) 
                          /* //turma filha e tem professores ativos na turma pai
                            ((turma.cd_turma_ppt != null && turma.cd_turma_ppt > 0 && turma.id_turma_ppt == false) &&
                             turma.TurmaPai.turmaPTurmaProfessorTurma.Where(x => x.id_professor_ativo).Any())||*/
                              
                            //turma pai e tem professores ativos na turma

                    select new
                    {
                        nome_grupo = (turma.cd_turma_ppt != null && turma.cd_turma_ppt > 0 && turma.id_turma_ppt == false)? turma.TurmaPai.no_turma : turma.no_turma,
                        codigo = (turma.cd_turma_ppt != null && turma.cd_turma_ppt > 0 && turma.id_turma_ppt == false) ? turma.TurmaPai.cd_turma : turma.cd_turma,
                        id_unidade = (turma.cd_turma_ppt != null && turma.cd_turma_ppt > 0 && turma.id_turma_ppt == false) ?
                            (turma.TurmaPai.EscolaTurma.cd_empresa_coligada == null ? turma.TurmaPai.EscolaTurma.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == turma.TurmaPai.EscolaTurma.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()):
                            (turma.EscolaTurma.cd_empresa_coligada == null ? turma.EscolaTurma.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == turma.EscolaTurma.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()),
                        cd_turma_ppt = turma.cd_turma_ppt,
                        id_turma_ppt = turma.id_turma_ppt,
                        id_turma_ativa = turma.id_turma_ativa
                        //codigo_professor = (turma.TurmaProfessorTurma.Where(x => /*x.id_professor_ativo &&*/ x.cd_professor == cd_professor).Any())? turma.TurmaProfessorTurma.Where(x => /*x.id_professor_ativo &&*/ x.cd_professor == cd_professor).FirstOrDefault().cd_professor_turma : 0,
                        //id_professor_ativo = (turma.TurmaProfessorTurma.Where(x => /*x.id_professor_ativo &&*/ x.cd_professor == cd_professor).Any()) ? turma.TurmaProfessorTurma.Where(x => /*x.id_professor_ativo &&*/ x.cd_professor == cd_professor).FirstOrDefault().id_professor_ativo : false,

                    }).ToList().Select(x => new TurmaApiCyberBdUI
                    {
                        nome_grupo = x.nome_grupo,
                        codigo = x.codigo,
                        id_unidade = x.id_unidade,
                        cd_turma_ppt = x.cd_turma_ppt,
                        id_turma_ppt = x.id_turma_ppt,
                        id_turma_ativa = x.id_turma_ativa
                        //codigo_professor = x.codigo_professor,
                        //id_professor_ativo = x.id_professor_ativo
                    }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public TurmaApiCyberBdUI findTurmaByCdTurmaAndCdEscolaApiCyber(int cd_turma, int cd_escola)
        {
            try
            {
                TurmaApiCyberBdUI sql = (from turma in db.Turma
                                         where (turma.cd_turma == cd_turma) &&
                                               (turma.cd_pessoa_escola == cd_escola) 
                                         select new
                                         {
                                             nome_grupo = (turma.cd_turma_ppt != null && turma.cd_turma_ppt > 0 && turma.id_turma_ppt == false) ? turma.TurmaPai.no_turma : turma.no_turma,
                                             codigo = (turma.cd_turma_ppt != null && turma.cd_turma_ppt > 0 && turma.id_turma_ppt == false) ? turma.TurmaPai.cd_turma : turma.cd_turma,
                                             id_unidade = (turma.cd_turma_ppt != null && turma.cd_turma_ppt > 0 && turma.id_turma_ppt == false) ?
                                                 (turma.TurmaPai.EscolaTurma.cd_empresa_coligada == null ? turma.TurmaPai.EscolaTurma.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == turma.TurmaPai.EscolaTurma.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()) :
                                                 (turma.EscolaTurma.cd_empresa_coligada == null ? turma.EscolaTurma.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == turma.EscolaTurma.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()),
                                             cd_turma_ppt = turma.cd_turma_ppt,
                                             id_turma_ppt = turma.id_turma_ppt,
                                             id_turma_ativa = turma.id_turma_ativa

                                         }).ToList().Select(x => new TurmaApiCyberBdUI
                                         {
                                             nome_grupo = x.nome_grupo,
                                             codigo = x.codigo,
                                             id_unidade = x.id_unidade,
                                             cd_turma_ppt = x.cd_turma_ppt,
                                             id_turma_ppt = x.id_turma_ppt,
                                             id_turma_ativa = x.id_turma_ativa
                                         }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TurmaApiCyberBdUI findTurmaCancelarEncerramentoApiCyber(int cd_turma)
        {
            try
            {
                TurmaApiCyberBdUI sql = (from turma in db.Turma
                                         where (turma.cd_turma == cd_turma) 

                                         select new
                                         {
                                             nome_grupo =  turma.no_turma,
                                             codigo =  turma.cd_turma,
                                             id_unidade = (turma.EscolaTurma.cd_empresa_coligada == null ? turma.EscolaTurma.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == turma.EscolaTurma.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()),
                                             cd_turma_ppt = turma.cd_turma_ppt,
                                             id_turma_ppt = turma.id_turma_ppt,
                                             id_turma_ativa = turma.id_turma_ativa
                                             //codigo_professor = (turma.TurmaProfessorTurma.Where(x => /*x.id_professor_ativo &&*/ x.cd_professor == cd_professor).Any())? turma.TurmaProfessorTurma.Where(x => /*x.id_professor_ativo &&*/ x.cd_professor == cd_professor).FirstOrDefault().cd_professor_turma : 0,
                                             //id_professor_ativo = (turma.TurmaProfessorTurma.Where(x => /*x.id_professor_ativo &&*/ x.cd_professor == cd_professor).Any()) ? turma.TurmaProfessorTurma.Where(x => /*x.id_professor_ativo &&*/ x.cd_professor == cd_professor).FirstOrDefault().id_professor_ativo : false,

                                         }).ToList().Select(x => new TurmaApiCyberBdUI
                                         {
                                             nome_grupo = x.nome_grupo,
                                             codigo = x.codigo,
                                             id_unidade = x.id_unidade,
                                             cd_turma_ppt = x.cd_turma_ppt,
                                             id_turma_ppt = x.id_turma_ppt,
                                             id_turma_ativa = x.id_turma_ativa
                                             //codigo_professor = x.codigo_professor,
                                             //id_professor_ativo = x.id_professor_ativo
                                         }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ProfessorTurma> findProfessorTurmaByCdTurma(int cd_turma, int cd_escola)
        {
            try
            {
                IEnumerable<ProfessorTurma> sql = (from pt in db.ProfessorTurma
                    where (pt.cd_turma == cd_turma) &&
                          (pt.Turma.cd_pessoa_escola == cd_escola) &&
                          pt.id_professor_ativo
                    select pt);
                    
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        
        public IEnumerable<LivroAlunoApiCyberBdUI> findAlunoTurmaAtivosByCdTurma(int cd_turma)
        {
            try
            {
                var sql = (from at in db.AlunoTurma.Include(a => a.Turma).Include(x => x.Aluno)
                           where  at.cd_turma == cd_turma &&
                                 //at.Turma.dt_termino_turma == null && at.Turma.id_turma_ppt == false &&
                                 (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                  at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                  at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)
                           select new
                           {
                               codigo_aluno = at.cd_aluno,
                               codigo_grupo = (at.Turma.cd_turma_ppt != null && at.Turma.cd_turma_ppt > 0 && at.Turma.id_turma_ppt == false) ? at.Turma.TurmaPai.cd_turma : at.Turma.cd_turma,
                               nome_turma = (at.Turma.cd_turma_ppt != null && at.Turma.cd_turma_ppt > 0 && at.Turma.id_turma_ppt == false) ? at.Turma.TurmaPai.no_turma : at.Turma.no_turma,
                               codigo_livro = (from lc in db.LivroCyber where lc.cd_estagio == at.Turma.Curso.Estagio.cd_estagio select lc).Any() ? (from lc in db.LivroCyber where lc.cd_estagio == at.Turma.Curso.Estagio.cd_estagio select lc).FirstOrDefault().cd_livro_cyber : 0,
                               codigo_unidade = (at.Turma.cd_turma_ppt != null && at.Turma.cd_turma_ppt > 0 && at.Turma.id_turma_ppt == false) ?
                                   (at.Turma.TurmaPai.EscolaTurma.cd_empresa_coligada == null ? at.Turma.TurmaPai.EscolaTurma.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == at.Turma.TurmaPai.EscolaTurma.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()) :
                                   (at.Turma.EscolaTurma.cd_empresa_coligada == null ? at.Turma.EscolaTurma.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == at.Turma.EscolaTurma.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault())


                           }).ToList().Select(x => new LivroAlunoApiCyberBdUI()
                           {
                               codigo_aluno = x.codigo_aluno,
                               codigo_grupo = x.codigo_grupo,
                               codigo_livro = x.codigo_livro,
                               codigo_unidade = x.codigo_unidade,
                               nome_turma = x.nome_turma

                           }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LivroAlunoApiCyberBdUI> findAlunoTurmaAtivosByCdTurmaPPTPai(int cd_turma)
        {
            try
            {
                var sql = (from at in db.AlunoTurma.Include(a => a.Turma).Include(x => x.Aluno)
                           where at.Turma.cd_turma_ppt == cd_turma &&
                                 //at.Turma.dt_termino_turma == null && at.Turma.id_turma_ppt == false &&
                                 (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                  at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                  at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)
                           select new
                           {
                               codigo_aluno = at.cd_aluno,
                               codigo_grupo = (at.Turma.cd_turma_ppt != null && at.Turma.cd_turma_ppt > 0 && at.Turma.id_turma_ppt == false) ? at.Turma.TurmaPai.cd_turma : at.Turma.cd_turma,
                               nome_turma = (at.Turma.cd_turma_ppt != null && at.Turma.cd_turma_ppt > 0 && at.Turma.id_turma_ppt == false) ? at.Turma.TurmaPai.no_turma : at.Turma.no_turma,
                               codigo_livro = (from lc in db.LivroCyber where lc.cd_estagio == at.Turma.Curso.Estagio.cd_estagio select lc).Any() ? (from lc in db.LivroCyber where lc.cd_estagio == at.Turma.Curso.Estagio.cd_estagio select lc).FirstOrDefault().cd_livro_cyber : 0,
                               codigo_unidade = (at.Turma.cd_turma_ppt != null && at.Turma.cd_turma_ppt > 0 && at.Turma.id_turma_ppt == false) ?
                                   (at.Turma.TurmaPai.EscolaTurma.cd_empresa_coligada == null ? at.Turma.TurmaPai.EscolaTurma.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == at.Turma.TurmaPai.EscolaTurma.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()) :
                                   (at.Turma.EscolaTurma.cd_empresa_coligada == null ? at.Turma.EscolaTurma.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == at.Turma.EscolaTurma.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault())


                           }).ToList().Select(x => new LivroAlunoApiCyberBdUI()
                           {
                               codigo_aluno = x.codigo_aluno,
                               codigo_grupo = x.codigo_grupo,
                               codigo_livro = x.codigo_livro,
                               codigo_unidade = x.codigo_unidade,
                               nome_turma = x.nome_turma

                           }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int findNovaTurmaByCdTurmaEncerrada(int cd_turma)
        {
            try
            {
                var sql = (from t in db.Turma
                           where  t.cd_turma_enc == cd_turma
                           select t.cd_turma).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        
        public TurmaSearch getTurmaByCodForGrid(int cdTurma, int cdEscola, bool turmasFilha)
        {
            try
            {
                var sqlSearch = (from t in db.Turma
                                 where t.cd_turma == cdTurma //t.cd_pessoa_escola == cdEscola && 
                                 select new TurmaSearch
                                 {
                                     cd_pessoa_escola = t.cd_pessoa_escola,
                                     cd_turma = t.cd_turma,
                                     cd_turma_ppt = t.cd_turma_ppt,
                                     no_turma = t.no_turma,
                                     no_apelido = t.no_apelido,
                                     no_curso = t.Curso.no_curso,
                                     no_duracao = t.Duracao.dc_duracao,
                                     no_produto = t.Produto.no_produto,
                                     dt_inicio_aula = t.dt_inicio_aula,
                                     id_turma_ppt = t.id_turma_ppt,
                                     id_turma_ativa = t.id_turma_ativa,
                                     dta_termino_turma = t.dt_termino_turma,
                                     nro_alunos = t.id_turma_ppt == true ? (from turma in db.Turma
                                                                            where turma.cd_turma_ppt == t.cd_turma &
                                                                            turma.TurmaAluno.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial).Any()
                                                                            select turma.cd_turma).Count() :
                                     t.TurmaAluno.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial).Count(),
                                     no_professor = t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true).FirstOrDefault().Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                                     no_aluno = turmasFilha ? t.TurmaAluno.FirstOrDefault().Aluno.AlunoPessoaFisica.no_pessoa : "",
                                     existe_diario = t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                 }).FirstOrDefault();
                return sqlSearch;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TurmaSearch getTurmaByCodMudancaOuEncerramento(int cdTurma, int cdEscola)
        {
            try
            {
                bool? vagasTurma = (from p in db.Parametro
                                    where p.cd_pessoa_escola == cdEscola
                                    select p.id_bloquear_matricula_vaga).FirstOrDefault();

                var sql = (from t in db.Turma
                           where t.cd_turma == cdTurma //&& t.cd_pessoa_escola == cdEscola
                           select t);

                var sqlSearch = (from t in sql
                                 select new TurmaSearch
                                 {
                                     cd_pessoa_escola = t.cd_pessoa_escola,
                                     cd_turma = t.cd_turma,
                                     cd_turma_ppt = t.cd_turma_ppt,
                                     no_turma = t.no_turma,
                                     no_apelido = t.no_apelido,
                                     no_curso = t.Curso.no_curso,
                                     no_duracao = t.Duracao.dc_duracao,
                                     no_produto = t.Produto.no_produto,
                                     dt_inicio_aula = t.dt_inicio_aula,
                                     id_turma_ppt = t.id_turma_ppt,
                                     id_turma_ativa = t.id_turma_ativa,
                                     dta_termino_turma = t.dt_termino_turma,
                                     nro_alunos = t.id_turma_ppt == true ? (from turma in db.Turma
                                                                            where turma.cd_turma_ppt == t.cd_turma &
                                                                            turma.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola &&
                                                                                (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Any()
                                                                            select turma.cd_turma).Count() :
                                     t.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && 
                                                                (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Count(),
                                     no_professor = t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true).FirstOrDefault().Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                                     considera_vagas = vagasTurma,
                                    vagas_disponiveis =
                                    vagasTurma == true ?
                                        t.T_SALA != null ?
                                            t.Curso != null ?
                                                t.T_SALA.nm_vaga_sala <= t.Curso.nm_vagas_curso ?
                                                    t.T_SALA.nm_vaga_sala :
                                                (int)t.Curso.nm_vagas_curso :
                                            (int)t.T_SALA.nm_vaga_sala :
                                        t.Sala != null ?
                                            t.Curso != null ?
                                                t.Sala.nm_vaga_sala <= t.Curso.nm_vagas_curso ?
                                                    t.Sala.nm_vaga_sala :
                                                (int)t.Curso.nm_vagas_curso :
                                            (int)t.Sala.nm_vaga_sala :
                                        t.Curso != null ?
                                            (int)t.Curso.nm_vagas_curso :
                                        0 :
                                    100,
                                     cd_turma_enc = t.cd_turma_enc
                                 }).FirstOrDefault();
                return sqlSearch;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public List<TurmaSearch> getTurmaByCodigosEncerramento(List<int?> cdTurma, int cdEscola)
        {
            try
            {
                bool? vagasTurma = (from p in db.Parametro
                                    where p.cd_pessoa_escola == cdEscola
                                    select p.id_bloquear_matricula_vaga).FirstOrDefault();

                var sql = (from t in db.Turma
                           where cdTurma.Contains(t.cd_turma) //&& t.cd_pessoa_escola == cdEscola
                           select t);

                var sqlSearch = (from t in sql
                                 select new TurmaSearch
                                 {
                                     cd_pessoa_escola = t.cd_pessoa_escola,
                                     cd_turma = t.cd_turma,
                                     cd_turma_ppt = t.cd_turma_ppt,
                                     no_turma = t.no_turma,
                                     no_apelido = t.no_apelido,
                                     no_curso = t.Curso.no_curso,
                                     no_duracao = t.Duracao.dc_duracao,
                                     no_produto = t.Produto.no_produto,
                                     dt_inicio_aula = t.dt_inicio_aula,
                                     id_turma_ppt = t.id_turma_ppt,
                                     id_turma_ativa = t.id_turma_ativa,
                                     dta_termino_turma = t.dt_termino_turma,
                                     nro_alunos = t.id_turma_ppt == true ? (from turma in db.Turma
                                                                            where turma.cd_turma_ppt == t.cd_turma &
                                                                            turma.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola &&
                                                                                (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Any()
                                                                            select turma.cd_turma).Count() :
                                     t.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola &&
                                                                (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Count(),
                                     no_professor = t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true).FirstOrDefault().Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                                     considera_vagas = vagasTurma,
                                     vagas_disponiveis =
                                    vagasTurma == true ?
                                        t.T_SALA != null ?
                                            t.Curso != null ?
                                                t.T_SALA.nm_vaga_sala <= t.Curso.nm_vagas_curso ?
                                                    t.T_SALA.nm_vaga_sala :
                                                (int)t.Curso.nm_vagas_curso :
                                            (int)t.T_SALA.nm_vaga_sala :
                                        t.Sala != null ?
                                            t.Curso != null ?
                                                t.Sala.nm_vaga_sala <= t.Curso.nm_vagas_curso ?
                                                    t.Sala.nm_vaga_sala :
                                                (int)t.Curso.nm_vagas_curso :
                                            (int)t.Sala.nm_vaga_sala :
                                        t.Curso != null ?
                                            (int)t.Curso.nm_vagas_curso :
                                        0 :
                                    100,
                                     cd_turma_enc = t.cd_turma_enc
                                 }).ToList();
                return sqlSearch;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Turma findTurmasByIdAndCdEscola(int cdTurma, int cdEscola)
        {
            try
            {
                var sql = (from t in db.Turma
                           where t.cd_turma == cdTurma  //LBM&& t.cd_pessoa_escola == cdEscola
                           select t).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public Turma findTurmaByCdTurmaApiCyber(int cdTurma)
        {
            try
            {
                var sql = (from t in db.Turma
                        .Include(x=> x.TurmaPai)
                        .Include(x=> x.EscolaTurma)
                    where t.cd_turma == cdTurma  
                    select t).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Turma> getTurmasByCod(int[] cdTurmas,int cd_escola) {
            try {
                var result = from turma in db.Turma
                             where cdTurmas.Contains(turma.cd_turma) && turma.cd_pessoa_escola == cd_escola
                             select turma;
                return result.ToList();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public Turma buscarTurmaHorariosEdit(int cdTurma, int cdEscola, TipoConsultaTurmaEnum tipo)
        {
            try
            {
                int escola = (from p in db.Turma
                                    where p.cd_turma == cdTurma
                                    select p.cd_pessoa_escola).FirstOrDefault();
                bool? vagasTurma = (from p in db.Parametro
                                    where p.cd_pessoa_escola == escola
                                    select p.id_bloquear_matricula_vaga).FirstOrDefault();
                var sql = (from turma in db.Turma
                           where turma.cd_turma == cdTurma //&&
                                 //turma.cd_pessoa_escola == cdEscola
                           select new
                           {
                               no_turma = turma.no_turma,
                               no_apelido = turma.no_apelido,
                               cd_turma = turma.cd_turma,
                               cd_pessoa_escola = turma.cd_pessoa_escola,
                               cd_turma_ppt = turma.cd_turma_ppt,
                               cd_curso = turma.cd_curso,
                               cd_sala = turma.cd_sala,
                               no_sala = turma.Sala.no_sala,
                               cd_duracao = turma.cd_duracao,
                               cd_regime = turma.cd_regime,
                               dt_inicio_aula = turma.dt_inicio_aula,
                               dt_final_aula = turma.dt_final_aula,
                               id_aula_externa = turma.id_aula_externa,
                               nro_aulas_programadas = turma.nro_aulas_programadas,
                               id_turma_ppt = turma.id_turma_ppt,
                               id_turma_ativa = turma.id_turma_ativa,
                               cd_produto = turma.cd_produto,
                               dta_termino_turma = turma.dt_termino_turma,
                               no_turma_ppt = turma.TurmaPai.no_turma,
                               cd_turma_enc = turma.cd_turma_enc,
                               cd_sala_online = turma.cd_sala_online,
                               no_sala_online = turma.T_SALA.no_sala,
                               vagas_disponiveis =
                                    vagasTurma == true ?
                                        turma.T_SALA != null ?
                                            turma.Curso != null ?
                                                turma.T_SALA.nm_vaga_sala <= turma.Curso.nm_vagas_curso ?
                                                    turma.T_SALA.nm_vaga_sala :
                                                (int)turma.Curso.nm_vagas_curso :
                                            (int)turma.T_SALA.nm_vaga_sala :
                                        turma.Sala != null ?
                                            turma.Curso != null ?
                                                turma.Sala.nm_vaga_sala <= turma.Curso.nm_vagas_curso ?
                                                    turma.Sala.nm_vaga_sala :
                                                (int)turma.Curso.nm_vagas_curso :
                                            (int)turma.Sala.nm_vaga_sala :
                                        turma.Curso != null ?
                                            (int)turma.Curso.nm_vagas_curso :
                                        0 :
                                    100
                           }).ToList().Select(x => new Turma
                           {
                               no_turma = x.no_turma,
                               no_apelido = x.no_apelido,
                               cd_turma = x.cd_turma,
                               cd_pessoa_escola = x.cd_pessoa_escola,
                               cd_turma_ppt = x.cd_turma_ppt,
                               cd_curso = x.cd_curso,
                               cd_sala = x.cd_sala,
                               no_sala = x.no_sala,
                               cd_duracao = x.cd_duracao,
                               cd_regime = x.cd_regime,
                               dt_inicio_aula = x.dt_inicio_aula,
                               dt_final_aula = x.dt_final_aula,
                               id_aula_externa = x.id_aula_externa,
                               nro_aulas_programadas = x.nro_aulas_programadas,
                               id_turma_ppt = x.id_turma_ppt,
                               id_turma_ativa = x.id_turma_ativa,
                               cd_produto = x.cd_produto,
                               dt_termino_turma = x.dta_termino_turma,
                               no_turma_ppt = x.no_turma_ppt,
                               cd_turma_enc = x.cd_turma_enc,
                               cd_sala_online = x.cd_sala_online,
                               no_sala_online = x.no_sala_online,
                               nm_vagas = x.vagas_disponiveis
                           }).FirstOrDefault();
                if (sql != null && sql.cd_turma > 0)
                {
                    sql.horariosTurma = (from horario in db.Horario.Include(p => p.HorariosProfessores)
                                         where //horario.cd_pessoa_escola == cdEscola &&
                                         horario.id_origem == (int)Horario.Origem.TURMA &&
                                         horario.cd_registro == cdTurma
                                         select new
                                         {
                                             horario.cd_horario,
                                             horario.cd_registro,
                                             horario.id_origem,
                                             horario.id_disponivel,
                                             horario.id_dia_semana,
                                             horario.dt_hora_ini,
                                             horario.dt_hora_fim,
                                             horario.HorariosProfessores
                                         }).ToList().Select(x => new Horario { 
                                             cd_horario = x.cd_horario,
                                             cd_registro = x.cd_registro,
                                             id_origem = x.id_origem,
                                             id_disponivel = x.id_disponivel,
                                             id_dia_semana = x.id_dia_semana,
                                             dt_hora_ini = x.dt_hora_ini,
                                             dt_hora_fim = x.dt_hora_fim,
                                             HorariosProfessores = x.HorariosProfessores.ToList().Select(y => new HorarioProfessorTurma()
                                             {
                                                 cd_horario_professor_turma = y.cd_horario_professor_turma,
                                                 cd_horario = y.cd_horario,
                                                 cd_professor = y.cd_professor
                                             }).ToList()
                                         }).ToList();

                    sql.ProfessorTurma = (from professorT in db.ProfessorTurma
                                          where professorT.cd_turma == cdTurma //&& professorT.Turma.cd_pessoa_escola == cdEscola
                                          select new
                                          {
                                              cd_professor_turma = professorT.cd_professor_turma,
                                              cd_turma = professorT.cd_turma,
                                              cd_professor = professorT.cd_professor,
                                              id_professor_ativo = professorT.id_professor_ativo,
                                              no_professor = professorT.Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                                              HorariosProfessores = professorT.Professor.HorariosProfessores.Where(hp => hp.Horario.cd_registro == cdTurma)
                                          }).ToList().Select(x => new ProfessorTurma
                                   {
                                       cd_professor_turma = x.cd_professor_turma,
                                       cd_turma = x.cd_turma,
                                       cd_professor = x.cd_professor,
                                       no_professor = x.no_professor,
                                       id_professor_ativo = x.id_professor_ativo,
                                       HorariosProfessores = x.HorariosProfessores.ToList().Select(y => new HorarioProfessorTurma()
                                       {
                                           cd_horario_professor_turma = y.cd_horario_professor_turma,
                                           cd_horario = y.cd_horario,
                                           cd_professor = y.cd_professor
                                       }).ToList()
                                   });

                    //Pesquisa das turmas PPTs filhas para edição:
                    if (sql.id_turma_ppt && tipo == TipoConsultaTurmaEnum.HAS_BUSCA_TURMA)
                    {
                        sql.alunosTurmasPPTSearch = (from at in db.AlunoTurma
                                                     where at.Turma.cd_turma_ppt == cdTurma && at.Aluno.cd_pessoa_escola == cdEscola
                                                     select new
                                                     {
                                                         at.Turma.cd_turma,
                                                         at.Turma.Curso.cd_curso,
                                                         at.Turma.cd_duracao,
                                                         at.Turma.cd_regime,
                                                         at.Turma.Regime.no_regime,
                                                         at.Turma.no_turma,
                                                         at.Turma.no_apelido,
                                                         at.Turma.Curso.no_curso,
                                                         at.Turma.nro_aulas_programadas,
                                                         no_aluno = at.Aluno.AlunoPessoaFisica.no_pessoa,
                                                         at.Aluno.Bolsa.pc_bolsa,
                                                         at.Aluno.Bolsa.pc_bolsa_material,
                                                         dt_inicio_bolsa = at.Aluno.Bolsa.dt_inicio_bolsa == null ? (DateTime?)at.Aluno.Bolsa.dt_inicio_bolsa : null,
                                                         cd_pessoa_aluno = at.Aluno.AlunoPessoaFisica.cd_pessoa,
                                                         at.Aluno.AlunoPessoaFisica.dt_cadastramento,
                                                         vl_abatimento_matricula = at.Aluno.Contratos.Any(x => x.vl_pre_matricula > 0) && at.Aluno.Prospect != null ? 0 : at.Aluno.Prospect.vl_matricula_prospect,
                                                         at.Turma.dt_termino_turma,
                                                         at.Turma.dt_final_aula,
                                                         at.Turma.dt_inicio_aula,
                                                         horariosTurma = from horario in db.Horario
                                                                         where //horario.cd_pessoa_escola == cdEscola && 
                                                                         horario.id_origem == (int)Horario.Origem.TURMA &&
                                                                         horario.cd_registro == at.cd_turma
                                                                         select horario,
                                                         id_aluno_ativo = at.Aluno.id_aluno_ativo,
                                                         //Campos usados aluno turma
                                                         at.cd_situacao_aluno_turma,
                                                         at.cd_aluno_turma,
                                                         at.cd_aluno,
                                                         at.cd_contrato,
                                                         dc_reduzido_pessoa_escola = at.Aluno.EscolaAluno.dc_reduzido_pessoa,
                                                         cd_pessoa_escola = at.Aluno.EscolaAluno.cd_pessoa
                                                     }).ToList().Select(x => new AlunoTurmaPPT
                                                     {
                                                         cd_turma = x.cd_turma,
                                                         cd_curso = x.cd_curso,
                                                         cd_duracao = x.cd_duracao,
                                                         cd_aluno = x.cd_aluno,
                                                         cd_regime = x.cd_regime,
                                                         no_regime = x.no_regime,
                                                         no_turma = x.no_turma,
                                                         no_apelido = x.no_apelido,
                                                         no_curso = x.no_curso,
                                                         nro_aulas_programadas = x.nro_aulas_programadas,
                                                         no_aluno = x.no_aluno,
                                                         pc_bolsa = x.pc_bolsa,
                                                         pc_bolsa_material = x.pc_bolsa_material,
                                                         dt_inicio_bolsa = x.dt_inicio_bolsa,
                                                         dt_termino_turma = x.dt_termino_turma,
                                                         dt_final_aula = x.dt_final_aula,
                                                         dt_inicio_aula = x.dt_inicio_aula,
                                                         horariosTurma = x.horariosTurma,
                                                         id_aluno_ativo = x.id_aluno_ativo,
                                                         cd_pessoa_aluno = x.cd_pessoa_aluno,
                                                         dt_cadastramento = x.dt_cadastramento,
                                                         vl_abatimento_matricula = x.vl_abatimento_matricula,
                                                         alunoTurma = new AlunoTurma
                                                         {
                                                             cd_situacao_aluno_turma = x.cd_situacao_aluno_turma,
                                                             cd_aluno_turma = x.cd_aluno_turma,
                                                             cd_aluno = x.cd_aluno,
                                                             cd_contrato = x.cd_contrato,
                                                             dc_reduzido_pessoa_escola = x.dc_reduzido_pessoa_escola,
                                                             cd_pessoa_escola_aluno = x.cd_pessoa_escola
                                                         }

                                                     });

                        //TODO DEivid
                        if (sql.alunosTurmasPPTSearch != null && sql.alunosTurmasPPTSearch.Count() > 0)
                        {
                            List<AlunoTurmaPPT> alunosTurmaPPT = sql.alunosTurmasPPTSearch.ToList();
                            for (int i = 0; i < alunosTurmaPPT.Count(); i++)
                            {
                                int cd_turma = alunosTurmaPPT[i].cd_turma;
                                alunosTurmaPPT[i].ProfessorTurma = new List<ProfessorTurma>();
                                alunosTurmaPPT[i].ProfessorTurma = (from professorT in db.ProfessorTurma
                                                                    where professorT.cd_turma == cd_turma //&& professorT.Turma.cd_pessoa_escola == cdEscola
                                                                    select new
                                                                    {
                                                                        cd_professor_turma = professorT.cd_professor_turma,
                                                                        cd_turma = professorT.cd_turma,
                                                                        cd_professor = professorT.cd_professor,
                                                                        id_professor_ativo = professorT.id_professor_ativo,
                                                                        no_professor = professorT.Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                                                                        HorariosProfessores = professorT.Professor.HorariosProfessores.Where(hp => hp.Horario.cd_registro == cd_turma)
                                                                    }).ToList().Select(x => new ProfessorTurma
                                                                    {
                                                                        cd_professor_turma = x.cd_professor_turma,
                                                                        cd_turma = x.cd_turma,
                                                                        cd_professor = x.cd_professor,
                                                                        no_professor = x.no_professor,
                                                                        id_professor_ativo = x.id_professor_ativo,
                                                                        HorariosProfessores = x.HorariosProfessores.ToList().Select(y => new HorarioProfessorTurma()
                                                                        {
                                                                            cd_horario_professor_turma = y.cd_horario_professor_turma,
                                                                            cd_horario = y.cd_horario,
                                                                            cd_professor = y.cd_professor
                                                                        }).ToList()
                                                                    });
                            }

                            sql.alunosTurmasPPTSearch = alunosTurmaPPT;
                        }
                    }
                    else
                    {
                        sql.alunosTurma = (from alunosT in db.AlunoTurma
                                           where alunosT.cd_turma == cdTurma && alunosT.Aluno.cd_pessoa_escola == cdEscola
                                           select new
                                           {
                                               cd_aluno_turma = alunosT.cd_aluno_turma,
                                               cd_turma = alunosT.cd_turma,
                                               cd_aluno = alunosT.cd_aluno,
                                               cd_situacao_aluno_turma = alunosT.cd_situacao_aluno_turma,
                                               id_aluno_ativo = db.Aluno.Where(a => a.cd_aluno == alunosT.cd_aluno && a.id_aluno_ativo == true).Any(),
                                               no_aluno = alunosT.Aluno.AlunoPessoaFisica.no_pessoa,
                                               cd_pessoa_aluno = alunosT.Aluno.cd_pessoa_aluno,
                                               cd_situacao_aluno_origem = alunosT.cd_situacao_aluno_origem,
                                               id_renegociacao = alunosT.id_renegociacao,
                                               dt_cadastramento = alunosT.Aluno.AlunoPessoaFisica.dt_cadastramento,
                                               cd_contrato = alunosT.cd_contrato,
                                               pessoaResposavel = alunosT.Aluno.AlunoPessoaFisica.PessoaSGFQueUsoOCpf,
                                               pc_bolsa = alunosT.Aluno.Bolsa.pc_bolsa,
                                               pc_bolsa_material = alunosT.Aluno.Bolsa.pc_bolsa_material,
                                               dt_inicio_bolsa = (DateTime?)alunosT.Aluno.Bolsa.dt_inicio_bolsa,
                                               vl_abatimento_matricula = alunosT.Aluno.Contratos.Any(x => x.vl_pre_matricula > 0) && alunosT.Aluno.Prospect != null ? 0 : alunosT.Aluno.Prospect.vl_matricula_prospect,
                                               dc_reduzido_pessoa_escola = alunosT.Aluno.EscolaAluno.dc_reduzido_pessoa,
                                               cd_pessoa_escola = alunosT.Aluno.EscolaAluno.cd_pessoa
                                           }).ToList().Select(x => new AlunoTurma
                                           {
                                               cd_aluno_turma = x.cd_aluno_turma,
                                               cd_turma = x.cd_turma,
                                               cd_aluno = x.cd_aluno,
                                               cd_situacao_aluno_turma = x.cd_situacao_aluno_turma,
                                               id_aluno_ativo = x.id_aluno_ativo,
                                               no_aluno = x.no_aluno,
                                               cd_pessoa_aluno = x.cd_pessoa_aluno,
                                               cd_situacao_aluno_origem = x.cd_situacao_aluno_origem,
                                               id_renegociacao = x.id_renegociacao,
                                               dt_cadastramento = x.dt_cadastramento,
                                               cd_contrato = x.cd_contrato,
                                               pc_bolsa = x.pc_bolsa,
                                               pc_bolsa_material = x.pc_bolsa_material,
                                               dt_inicio_bolsa = x.dt_inicio_bolsa,
                                               vl_abatimento_matricula = x.vl_abatimento_matricula,
                                               PessoaSGFQueUsoOCpf = x.pessoaResposavel == null ? null : new PessoaFisicaSGF
                                               {
                                                   cd_pessoa = x.pessoaResposavel.cd_pessoa,
                                                   no_pessoa = x.pessoaResposavel.no_pessoa
                                               },
                                               dc_reduzido_pessoa_escola = x.dc_reduzido_pessoa_escola,
                                               cd_pessoa_escola_aluno = x.cd_pessoa_escola
                                           });
                    }
                    sql.alunosTurmaEscola = (from alunosT in db.AlunoTurma
                                             where ((!sql.id_turma_ppt && alunosT.cd_turma == cdTurma) ||
                                                    (sql.id_turma_ppt && alunosT.Turma.cd_turma_ppt == cdTurma))
                                             select new
                                             {
                                                 cd_turma = alunosT.cd_turma,
                                                 cd_aluno = alunosT.cd_aluno,
                                                 cd_situacao_aluno_turma = alunosT.cd_situacao_aluno_turma,
                                                 cd_pessoa_escola = alunosT.Aluno.cd_pessoa_escola,
                                             }).ToList().Select(x => new AlunoTurma
                                             {
                                                 cd_turma = x.cd_turma,
                                                 cd_aluno = x.cd_aluno,
                                                 cd_situacao_aluno_turma = x.cd_situacao_aluno_turma,
                                                 cd_pessoa_escola_aluno = x.cd_pessoa_escola
                                             }
                                       );
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Turma getTurmaEHorarios(int cdTurma, int cdEscola)
        {
            try
            {
                var sql = (from turma in db.Turma
                           where turma.cd_turma == cdTurma //&&
                                 //turma.cd_pessoa_escola == cdEscola
                           select new
                           {
                               no_turma = turma.no_turma,
                               cd_turma = turma.cd_turma,
                               cd_pessoa_escola = turma.cd_pessoa_escola,
                               cd_turma_ppt = turma.cd_turma_ppt,
                               cd_curso = turma.cd_curso,
                               cd_sala = turma.cd_sala,
                               no_sala = turma.Sala.no_sala,
                               cd_duracao = turma.cd_duracao,
                               cd_regime = turma.cd_regime,
                               dt_inicio_aula = turma.dt_inicio_aula,
                               dt_final_aula = turma.dt_final_aula,
                               id_aula_externa = turma.id_aula_externa,
                               nro_aulas_programadas = turma.nro_aulas_programadas,
                               id_turma_ppt = turma.id_turma_ppt,
                               id_turma_ativa = turma.id_turma_ativa,
                               cd_produto = turma.cd_produto,
                               dta_termino_turma = turma.dt_termino_turma,
                               no_turma_ppt = turma.TurmaPai.no_turma,
                               cd_turma_enc = turma.cd_turma_enc
                           }).ToList().Select(x => new Turma
                           {
                               no_turma = x.no_turma,
                               cd_turma = x.cd_turma,
                               cd_pessoa_escola = x.cd_pessoa_escola,
                               cd_turma_ppt = x.cd_turma_ppt,
                               cd_curso = x.cd_curso,
                               cd_sala = x.cd_sala,
                               no_sala = x.no_sala,
                               cd_duracao = x.cd_duracao,
                               cd_regime = x.cd_regime,
                               dt_inicio_aula = x.dt_inicio_aula,
                               dt_final_aula = x.dt_final_aula,
                               id_aula_externa = x.id_aula_externa,
                               nro_aulas_programadas = x.nro_aulas_programadas,
                               id_turma_ppt = x.id_turma_ppt,
                               id_turma_ativa = x.id_turma_ativa,
                               cd_produto = x.cd_produto,
                               dt_termino_turma = x.dta_termino_turma,
                               no_turma_ppt = x.no_turma_ppt,
                               cd_turma_enc = x.cd_turma_enc
                           }).FirstOrDefault();

                if (sql != null && sql.cd_turma > 0)
                {
                    //to do Deivid
                    sql.horariosTurma = (from horario in db.Horario.Include(p => p.HorariosProfessores)
                                         where //horario.cd_pessoa_escola == cdEscola && 
                                         horario.id_origem == (int)Horario.Origem.TURMA &&
                                         horario.cd_registro == cdTurma
                                         select horario).
                                        ToList();
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool ExisteProgramacaoTurma(int cd_turma, int cd_escola)
        {
            try
            {
                var result = (from  turma in db.Turma
                             where  turma.cd_turma == cd_turma &&
                                    turma.ProgramacaoTurma.Any()
                             select turma.cd_turma).Count();
                return result > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool ExisteAvaliacaoTurma(int cd_turma, int cd_escola)
        {
            try
            {
                var result = (from turma in db.Turma
                            where turma.AvaliacaoTurma.Where(at => at.AvaliacaoAluno.Any()).Any()
                            select turma.cd_turma).Count();
                return result > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificarTurmaExisteProgramacaoHorario(int cd_turma, int cd_horario, int cdEscola)
        {
            try
            {
                bool retorno = (from turma in db.Turma
                          join horario in db.Horario on turma.cd_turma equals horario.cd_registro
                          where turma.cd_turma == cd_turma
                                && (turma.cd_pessoa_escola == cdEscola)
                                && (horario.cd_horario == cd_horario)
                                && (turma.ProgramacaoTurma.Any(t => t.cd_turma == turma.cd_turma
                                                   && t.hr_inicial_programacao == horario.dt_hora_ini
                                                   && t.hr_final_programacao == horario.dt_hora_fim
                                                   && t.id_aula_dada == true))
                          select cd_turma).Any();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Verificar se existe turma com msm estagio/regime/horario/dia da semana/ semeste/ ano / PPT ou normal
        public int verificaExisteTurma(string nomeTurma, int cdEscola, int cdTurma)
        {
            try
            {
                #region Verificação de nome, por base
                ////  (int cdEstagio, int cdRegime, TimeSpan? hrIni, TimeSpan? hrFim, byte[] diaSemana, DateTime dtaIni, string ehPPT)
                //  var sql = from turma in db.Turma
                //            join horario in db.Horario on turma.cd_turma equals horario.cd_registro
                //            into hora from horario in hora.DefaultIfEmpty()
                //            where
                //              turma.Curso.cd_estagio == cdEstagio &&
                //              turma.cd_regime == cdRegime &&
                //              (horario != null && horario.dt_hora_ini == hrIni) &&
                //              (horario != null && horario.dt_hora_fim == hrFim) &&
                //              ((turma.dt_inicio_aula.Month > 6 && dtaIni.Month > 6) || (turma.dt_inicio_aula.Month < 6 && dtaIni.Month < 6)) &&
                //              turma.dt_inicio_aula.Year == dtaIni.Year
                //            select turma;

                //  //pegando o dia da semana das turmas que contem os menos Regime, estagio, horario, semestre e ano
                //  //var hor = from horario in db.Horario
                //  //          join s in sql
                //  //          on horario.cd_registro equals s.cd_turma
                //  //          select horario;

                //  //var listahorario = hor.ToList();
                //  //int diasSemanasConf = 0;
                //  ////Conferindo se existe os dias igual n
                //  //if (listahorario != null)
                //  //    foreach (byte d in listahorario)
                //  //        if (diaSemana.Contains(d))
                //  //            diasSemanasConf = diasSemanasConf + 1;

                //  if (ehPPT == "P")
                //      sql = from turma in sql
                //            where turma.id_turma_ppt == true
                //            select turma;
                //  else
                //      if (ehPPT == "F")
                //          sql = from turma in sql
                //                where turma.id_turma_ppt == true &&
                //                  turma.cd_turma_ppt > 0
                //                select turma;
                //      else
                //          sql = from turma in sql
                //                where
                //                  turma.id_turma_ppt == false &&
                //                  turma.cd_turma_ppt == null
                //                select turma;
                #endregion
                IQueryable<int> sql;
                if (cdTurma > 0)
                    sql = from t in db.Turma
                          where t.no_turma.Contains(nomeTurma) &&
                              t.cd_pessoa_escola == cdEscola &&
                              t.cd_turma != cdTurma
                          select t.nm_turma;

                else
                    sql = from t in db.Turma
                          where t.no_turma.Contains(nomeTurma) &&
                              t.cd_pessoa_escola == cdEscola
                          select t.nm_turma;

                var nr = 0;
                if (sql.Count() == 0)
                    nr = 1;
                else
                    nr = (from t in sql select t).Max() + 1;
                return nr;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //retorna a quantidade de alunos na turma para fazer a consistência na avaliação da turma 
        public int getTumaAlunoByIdTurma(int cd_turma, int cd_escola)
        {
            try
            {
                var sql = (from turma in db.Turma
                           where turma.cd_turma == cd_turma
                            //&& turma.cd_pessoa_escola == cd_escola Turmas Compartilhadas
                            && turma.TurmaAluno.Any(a => a.cd_turma == cd_turma)
                           orderby turma.no_turma
                           select turma).Count();
                return sql;

            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }
        }
        //retorna a quantidade de alunos na turma para fazer a consistência na avaliação da turma 
        public IEnumerable<Turma> getTurmaPoliticaEsc(int cdEscola)
        {
            try
            {
                var sql = (from turma in db.Turma
                           where turma.PoliticasTurmas.Where(p => p.cd_politica_desconto > 0).Any() &&
                           turma.cd_pessoa_escola == cdEscola
                           orderby turma.no_turma
                           select new
                           {
                               no_turma = turma.no_turma,
                               cd_turma = turma.cd_turma
                           }).ToList().Select(x => new Turma { no_turma = x.no_turma, cd_turma = x.cd_turma });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //TO DO Fazer teste
        public int getNumeroVagas(int id, int tipoConsuta, int cd_escola)
        {
            try
            {
                bool? vagasTurma = (from p in db.Parametro
                    where p.cd_pessoa_escola == cd_escola
                        select p.id_bloquear_matricula_vaga).FirstOrDefault();

                int? sql = 0;
                if (vagasTurma != null && (bool)vagasTurma)
                {
                    if (tipoConsuta == (int)TurmaDataAccess.TipoConsultaTurmaEnum.SALA)
                        sql = (from sala in db.Sala
                            where sala.cd_sala == id
                            select sala.nm_vaga_sala).FirstOrDefault();
                    else
                        sql = (int)(from curso in db.Curso
                            where curso.cd_curso == id
                            select curso.nm_vagas_curso).FirstOrDefault();
                    if (!sql.HasValue)
                        sql = 0;
                }
                else
                {
                    sql = 100;
                }
                
                return sql.HasValue ? sql.Value : 0 ;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ProgramacaoHorarioUI getProgramacaoHorarioTurma(int cd_turma, int cd_escola)
        {
            try
            {
                var retorno = from t in db.Turma
                              where t.cd_turma == cd_turma //&& t.cd_pessoa_escola == cd_escola
                              select new ProgramacaoHorarioUI
                              {
                                  cd_curso = t.cd_curso.Value,
                                  cd_duracao = t.cd_duracao,
                                  dt_inicio = t.dt_inicio_aula,
                                  //horarios = t.horariosTurma,
                                  programacoes = t.ProgramacaoTurma
                              };

                var programacaoHorarioUI = retorno.FirstOrDefault();

                if (programacaoHorarioUI != null)
                    programacaoHorarioUI.horarios = (from horario in db.Horario.Include(p => p.HorariosProfessores)
                                                     where horario.id_origem == (int)Horario.Origem.TURMA && //LBM Escola eliminada
                                                     horario.cd_registro == cd_turma
                                                     select horario).
                                                ToList();

                return programacaoHorarioUI;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Turma getTurmaComProgramacoes(int cd_turma)
        {
            try
            {
                var retorno = (from t in db.Turma.Include(p => p.ProgramacaoTurma)
                               where t.cd_turma == cd_turma
                               select t).FirstOrDefault();

                retorno.ProgramacaoTurma.OrderBy(pt => pt.nm_aula_programacao_turma);

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TurmaSearch> searchTurmaAluno(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
                                                  int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas, int cdAluno, int tipoConsulta, bool retorno, int cd_escola_combo_fk)
        {
            try
            {
                bool id_permitir_matricula = (from a in db.Curso where a.cd_curso == cdCurso select a.id_permitir_matricula).FirstOrDefault();
                IEntitySorter<TurmaSearch> sorter = EntitySorter<TurmaSearch>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Turma> sql;
                
                sql = from t in db.Turma.AsNoTracking()
                      where (t.cd_pessoa_escola == cdEscola ||
                           (from te in db.TurmaEscola
                               where te.cd_turma == t.cd_turma &&
                                     te.cd_escola == cdEscola
                               select te).Any()) &&
                        ((t.cd_curso == cdCurso || cdCurso == 0) || (t.id_turma_ppt && tipoTurma == (int)Turma.TipoTurma.PPT)) &&
                        (t.cd_produto == cdProduto || cdProduto == 0) &&
                        (t.cd_duracao == cdDuracao || cdDuracao == 0) &&
                        // Turma pesquisada tem que ter horário
                        db.Horario.Where(h => h.cd_registro == t.cd_turma && h.id_origem == (int)Horario.Origem.TURMA).Any() &&
                        ((t.id_turma_ppt && t.id_turma_ativa && tipoTurma == (int)Turma.TipoTurma.PPT) ||
                         (!t.id_turma_ppt &&
                            t.dt_termino_turma == null &&
                              // Não existe o aluno na turma pesquisada
                            (!t.TurmaAluno.Where(alt => alt.cd_aluno == cdAluno && alt.Turma.cd_turma == t.cd_turma).Any() ||
                              // OU existe porém com status de aguardando em qualquer turma ou desistente com retorno ou Ativo
                             t.TurmaAluno.Where(at => at.cd_aluno == cdAluno && at.Turma.dt_termino_turma == null &&
                                 ((at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando) ||
                                  (retorno && at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Desistente) ||
                                  (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                   at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                   at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial))).Any()))
                        ) 
                      //&&
                      // Se existir turma com ststus ativo, do mesmo produto no mesmo periodo tem que ser simultanea ou aceleração
                      //(db.AlunoTurma.Where(at => at.cd_aluno == cdAluno && //at.Turma.cd_turma == t.cd_turma &&
                      //                         (((at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo || 
                      //                            at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                      //                            at.Turma.cd_produto == t.cd_produto && 
                      //                            at.Turma.dt_termino_turma == null && at.Turma.dt_final_aula.Value > t.dt_inicio_aula &&
                      //                            (((at.Turma.Curso.id_permitir_matricula && !t.Curso.id_permitir_matricula) ||
                      //                             (!at.Turma.Curso.id_permitir_matricula && t.Curso.id_permitir_matricula)) ||
                      //                             //Aceleração somente para personalisadas do mesmo estágio
                      //                            (at.Turma.cd_turma_ppt != null && t.cd_turma_ppt != null && at.Turma.cd_curso == t.cd_curso)) ))).Any() ||
                      //// OU Não existe o aluno em nenhuma turma com status ativo, não encerrada, do mesmo produto e no mesmo período
                      //!db.AlunoTurma.Where(at => at.cd_aluno == cdAluno && //at.Turma.cd_turma == t.cd_turma &&
                      //                         (((at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo || 
                      //                            at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                      //                            at.Turma.cd_produto == t.cd_produto && at.Turma.dt_termino_turma == null &&
                      //                            at.Turma.dt_final_aula.Value > t.dt_inicio_aula))).Any())
                      select t;

                bool? vagasTurma = (from p in db.Parametro.AsNoTracking()
                                    where p.cd_pessoa_escola == ((cd_escola_combo_fk == 0) ? cdEscola : cd_escola_combo_fk)
                                    select p.id_bloquear_matricula_vaga).FirstOrDefault();

                //Busca os horários que o aluno esteja MATRICULADO/REMATRICULADO em outra turma 
                var horariosOcupados = (from ho in db.Horario.AsNoTracking()
                                        join at in db.AlunoTurma.AsNoTracking()
                                        on ho.cd_registro equals at.cd_turma
                                        where 
                                             at.cd_aluno == cdAluno &&
                                             at.cd_contrato != null &&
                                             ho.cd_pessoa_escola == ((cd_escola_combo_fk == 0) ? cdEscola : cd_escola_combo_fk) &&
                                             at.Turma.dt_termino_turma == null &&
                                             ho.id_origem == (int)Horario.Origem.TURMA &&
                                             (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                             at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                             at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)
                                        select new
                                        {
                                            id_dia_semana = ho.id_dia_semana,
                                            dt_hora_ini = ho.dt_hora_ini,
                                            dt_hora_fim = ho.dt_hora_fim,
                                            dt_inicio_aula = at.Turma.dt_inicio_aula,
                                            dt_final_aula = at.Turma.dt_final_aula,
                                            cd_produto = at.Turma.cd_produto,
                                            cd_turma_ppt = at.Turma.cd_turma_ppt,
                                            cd_curso = at.Turma.cd_curso,
                                            id_permitir_matricula = at.Turma.Curso.id_permitir_matricula,
                                            nm_duracao = at.Turma.Duracao.nm_duracao,
                                            nm_carga_horaria = at.Turma.Curso.nm_carga_horaria
                                        });

                var horariosDisponiveisAluno = (from ht in db.Horario
                                                where ht.cd_registro == cdAluno && 
                                                  ht.id_origem == (int)Horario.Origem.ALUNO &&
                                                  ht.cd_pessoa_escola == cdEscola
                                                select new
                                                {
                                                    id_dia_semana = ht.id_dia_semana,
                                                    dt_hora_ini = ht.dt_hora_ini,
                                                    dt_hora_fim = ht.dt_hora_fim
                                                });
                var listaHorariosOcupados = horariosOcupados.ToList();
                var listaHorariosDisponiveisAluno = horariosDisponiveisAluno.ToList();
                if (listaHorariosOcupados != null && listaHorariosOcupados.Count() > 0)
                {
                    foreach (var hOcup in listaHorariosOcupados)
                    {
                        sql =   (from t in sql
                                 join ht in db.Horario on t.cd_turma equals ht.cd_registro
                                 where
                                    ht.id_origem == (int)Horario.Origem.TURMA &&
                                     //ATENÇÃO: Se data final das turmas ocupadas não estiverem definidas será acrescentada 
                                     //o número de dias da carga horária à data inicial da turma.
                                     //A verificação para contratos sem turma será feita no postmatricula
                                    (
                                        (
                                            //Turma ocupada não simultânea e  Turma pesquisada antecipada,  estágios diferentes e período posterior ou anerior
                                            (!hOcup.id_permitir_matricula && !id_permitir_matricula && cdCurso != hOcup.cd_curso) &&
                                            //Turma pesquisada com período posterior
                                            ((!hOcup.dt_final_aula.HasValue && t.dt_inicio_aula > DbFunctions.AddDays(hOcup.dt_inicio_aula, hOcup.nm_duracao == 0 ? 0 : (int)(hOcup.nm_carga_horaria / (double)hOcup.nm_duracao * 7.0)) ||
                                              hOcup.dt_final_aula.HasValue && t.dt_inicio_aula > hOcup.dt_final_aula) ||
                                             (!t.dt_final_aula.HasValue && hOcup.dt_inicio_aula > DbFunctions.AddDays(t.dt_inicio_aula, t.Duracao.nm_duracao == 0 ? 0 : (int)(t.Curso.nm_carga_horaria / (double)t.Duracao.nm_duracao * 7.0)) ||
                                              t.dt_final_aula.HasValue && hOcup.dt_inicio_aula >= t.dt_final_aula)
                                            )
                                        ) ||
                                        // Horários diferentes
                                        (ht.id_dia_semana != hOcup.id_dia_semana || (
                                            !((ht.dt_hora_ini <= hOcup.dt_hora_ini && hOcup.dt_hora_ini < ht.dt_hora_fim)
                                            || (ht.dt_hora_ini < hOcup.dt_hora_fim && hOcup.dt_hora_fim <= ht.dt_hora_fim)
                                            || (ht.dt_hora_ini <= hOcup.dt_hora_ini && hOcup.dt_hora_fim <= ht.dt_hora_fim)
                                            || (ht.dt_hora_ini >= hOcup.dt_hora_ini && hOcup.dt_hora_fim >= ht.dt_hora_fim)))
                                        ) &&
                                        (
                                            // Produtos Diferentes
                                            (t.cd_produto != hOcup.cd_produto) ||
                                            // Mesmo produto
                                            (t.cd_produto == hOcup.cd_produto &&
                                                (
                                                    (!hOcup.dt_final_aula.HasValue && t.dt_inicio_aula <= DbFunctions.AddDays(hOcup.dt_inicio_aula, hOcup.nm_duracao == 0 ? 0 : (int)(hOcup.nm_carga_horaria / (double)hOcup.nm_duracao * 7.0)) ||
                                                        hOcup.dt_final_aula.HasValue && t.dt_inicio_aula <= hOcup.dt_final_aula) &&
                                                    (
                                                        //Turma ocupada não simultânea  e Aceleração só para personalizadas no mesmo período
                                                        (!hOcup.id_permitir_matricula && !id_permitir_matricula && cdCurso == hOcup.cd_curso &&
                                                         t.cd_turma_ppt != null && hOcup.cd_turma_ppt != null) ||
                                                        //Turma ocupada não simultânea  e Turma pesquisada simultânea, tem que estar no período
                                                        (!hOcup.id_permitir_matricula && id_permitir_matricula) ||
                                                        //Turma ocupada regular simultânea, independente da pesquisada tem que estar no mesmo período
                                                        hOcup.id_permitir_matricula || t.id_turma_ppt
                                                    )
                                                ) 
                                            )
                                        )
                                    )
                        select t).Distinct();
                    }
                }

                if (listaHorariosDisponiveisAluno.Count() > 0)
                {
                    var sql2 = (from t in sql
                                where t.cd_pessoa_escola == ((cd_escola_combo_fk == 0) ? cdEscola : cd_escola_combo_fk) &&
                                ((t.id_turma_ppt == true) ||
                                (t.id_turma_ppt == false &&
                                 t.dt_termino_turma == null ))
                                 //&&
                                 //!db.AlunoTurma.Where(at => at.cd_aluno == cdAluno && at.Aluno.cd_pessoa_escola == cdEscola &&
                                 //                    (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo || at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                                 //                     at.Turma.cd_produto == t.cd_produto).Any()))
                                select new
                                {
                                    cd_turma = t.cd_turma,
                                    HorariosTurma = db.Horario.Where(th => th.cd_registro == t.cd_turma && th.id_origem == (int)Horario.Origem.TURMA)
                                }).ToList().Select(x => new Turma
                            {
                                cd_turma = x.cd_turma,
                                horariosTurma = x.HorariosTurma.ToList()
                            });
                    List<Turma> horariosDisponiveisTurma = sql2.ToList();
                    //Buscar apensas as turmas com horario disponivel para o aluno, ou se o aluno não tiver nenhum horário

                    for (int i = horariosDisponiveisTurma.Count - 1; i >= 0; i--)
                    {
                        bool existHorarioAlunoContido = true;
                        foreach (var ht in horariosDisponiveisTurma[i].horariosTurma)
                        {
                            existHorarioAlunoContido &= (from ha in listaHorariosDisponiveisAluno
                                                         where (ha.id_dia_semana == ht.id_dia_semana &&
                                                                   ht.dt_hora_ini >= ha.dt_hora_ini &&
                                                                   ht.dt_hora_fim <= ha.dt_hora_fim)
                                                         select ha).Any();

                        }
                        if (!existHorarioAlunoContido)
                            horariosDisponiveisTurma.Remove(horariosDisponiveisTurma[i]);
                    }
                       
                   if (horariosDisponiveisTurma.Count() > 0)
                   {
                       List<int> cdTurmasCont = horariosDisponiveisTurma.Select(x => x.cd_turma).ToList();
                       //Voltar a pesquisa original.
                       //sql = null;
                       //sql = from t in db.Turma
                       //      where
                       //        t.cd_pessoa_escola == ((cd_escola_combo_fk == 0) ? cdEscola : cd_escola_combo_fk) &&
                       //        ((t.id_turma_ppt == true) ||
                       //         (t.id_turma_ppt == false &&
                       //          t.dt_termino_turma == null &&
                       //          !db.AlunoTurma.Where(at => at.cd_aluno == cdAluno && at.Aluno.cd_pessoa_escola == cdEscola &&
                       //                                 (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo || at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                       //                                  at.Turma.cd_produto == t.cd_produto).Any()))
                       //      select t;

                       if (cdTurmasCont != null && cdTurmasCont.Count > 0)
                           sql = from t in sql
                                 where cdTurmasCont.Contains(t.cd_turma)
                                 select t;
                   }
                }


                if (!string.IsNullOrEmpty(descricao))
                    if (inicio == true)
                        sql = from t in sql
                              where t.no_turma.StartsWith(descricao)
                              select t;
                    else
                        sql = from t in sql
                              where t.no_turma.Contains(descricao)
                              select t;

                if (!string.IsNullOrEmpty(apelido))
                    if (inicio == true)
                        sql = from t in sql
                              where t.no_apelido.StartsWith(apelido)
                              select t;
                    else
                        sql = from t in sql
                              where t.no_apelido.Contains(apelido)
                              select t;

                //Busca as turmas filhas aprtir do código da turma pai PPT
                if (tipoTurma > 0)
                {
                    if (tipoTurma == (int)Turma.TipoTurma.PPT)
                    {
                        if (!turmasFilhas && situacaoTurma > 0)
                        {
                            var ativo = true;
                            if (situacaoTurma == 2)
                                ativo = false;
                            sql = from t in sql
                                  where t.id_turma_ativa == ativo
                                  select t;
                        }
                        if (turmasFilhas)
                        { // Busca somente as turmas em que o aluno já está aguardando matrícula:
                            sql = from t in sql
                                  where t.cd_turma_ppt != null && t.id_turma_ppt == false
                                  select t;

                            if (cdAluno > 0)
                                sql = from t in sql
                                      where t.TurmaAluno.Where(at => at.cd_aluno == cdAluno && at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando).Any()
                                      select t;
                            if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                                sql = from t in sql
                                      where t.dt_termino_turma == null
                                      && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                      select t;
                            if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                                sql = from t in sql
                                      where t.dt_termino_turma != null
                                      select t;
                            if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                                sql = from t in sql
                                      where t.dt_termino_turma == null
                                      && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                      select t;
                        }
                        else
                            sql = from t in sql
                                  where t.id_turma_ppt == true && t.cd_turma_ppt == null
                                  select t;

                    }
                    if (tipoTurma == (int)Turma.TipoTurma.NORMAL)
                        sql = from t in sql
                              where t.id_turma_ppt == false &&
                                 t.cd_turma_ppt == null
                              select t;
                }
                else
                    sql = from t in sql
                          where (!t.id_turma_ppt && t.cd_turma_ppt == null) ||
                                 //Que é turma ppt filha e o aluno está aguardando nela//Que é turma normal
                                (t.cd_turma_ppt != null && !t.id_turma_ppt && 
                                  (t.TurmaAluno.Where(at => at.cd_aluno == cdAluno && at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando).Any() ||
                                  (retorno && t.TurmaAluno.Where(at =>at.cd_aluno == cdAluno && at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Desistente).Any()))
                                 )
                          select t;

                if ((tipoTurma != (int)Turma.TipoTurma.PPT) || (tipoTurma == (int)Turma.TipoTurma.PPT && turmasFilhas))
                {
                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                        sql = from t in sql
                              where t.dt_termino_turma == null
                              && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                              select t;
                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                        sql = from t in sql
                              where t.dt_termino_turma != null
                              select t;
                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                        sql = from t in sql
                              where t.dt_termino_turma == null
                              && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                              select t;
                }

                //Filtros foram deslocados para cima para agilizar
                //if (cdCurso > 0 && tipoTurma != (int)Turma.TipoTurma.PPT)
                //    sql = from t in sql
                //          where t.cd_curso == cdCurso
                //          select t;
                //if (cdDuracao > 0)
                //    sql = from t in sql
                //          where t.cd_duracao == cdDuracao
                //          select t;
                //if (cdProduto > 0)
                //    sql = from t in sql
                //          where t.cd_produto == cdProduto
                //          select t;

                switch (tipoConsulta)
                {
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA:
                        if (cdProfessor > 0)
                            sql = from t in sql
                                  where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor).Any()
                                  select t;
                        break;
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_FK_TURMA_PROF:
                        if (cdProfessor > 0)
                            sql = from t in sql
                                  where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo).Any()
                                  select t;

                        break;
                }

                switch (tipoProg)
                {
                    case ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_GERADAS:
                        sql = from t in sql
                              where t.ProgramacaoTurma.Any()
                              select t;
                        break;
                    case ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_NAO_GERADAS:
                        sql = from t in sql
                              where !t.ProgramacaoTurma.Any()
                              select t;
                        break;
                }
                if (vagasTurma != null && vagasTurma.Value && tipoTurma != (int)Turma.TipoTurma.PPT)
                    sql = from s in sql
                          where (s.Sala != null && s.Curso != null && s.Sala.nm_vaga_sala >= s.Curso.nm_vagas_curso && s.Curso.nm_vagas_curso >= s.TurmaAluno.Where(p => p.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo || p.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado || p.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial).Count()) ||
                                (s.Sala != null && s.Curso != null && s.Sala.nm_vaga_sala <= s.Curso.nm_vagas_curso && s.Sala.nm_vaga_sala >= s.TurmaAluno.Where(p => p.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo || p.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado || p.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial).Count()) ||
                                (s.Sala == null && s.Curso.nm_vagas_curso >= s.TurmaAluno.Where(p => p.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo || p.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado).Count())
                          select s;
                var sqlSearch = (from t in sql
                                 select new TurmaSearch
                                 {
                                     cd_turma = t.cd_turma,
                                     cd_turma_ppt = t.cd_turma_ppt,
                                     no_turma = t.no_turma,
                                     no_apelido = t.no_apelido,
                                     cd_curso = t.cd_curso,
                                     no_curso = t.Curso.no_curso,
                                     cd_duracao = t.cd_duracao,
                                     no_duracao = t.Duracao.dc_duracao,
                                     cd_produto = t.cd_produto,
                                     no_produto = t.Produto.no_produto,
                                     cd_regime = t.cd_regime,
                                     dt_inicio_aula = t.dt_inicio_aula,
                                     id_turma_ppt = t.id_turma_ppt,
                                     id_turma_ativa = t.id_turma_ativa,
                                     dta_termino_turma = t.dt_termino_turma,
                                     nro_alunos = t.id_turma_ppt == true ? (from turma in db.Turma
                                                                            where turma.cd_turma_ppt == t.cd_turma &&
                                                                            turma.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Any()
                                                                            select turma.cd_turma).Count() :
                                    t.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                             at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                             at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Count(),
                                     no_professor = t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true).FirstOrDefault().Professor.FuncionarioPessoaFisica.no_pessoa,
                                     alunoAguardando = t.TurmaAluno.Where(ta => ta.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando && ta.cd_aluno == cdAluno).Any(),
                                     no_aluno = turmasFilhas && t.TurmaAluno.FirstOrDefault() != null ? t.TurmaAluno.FirstOrDefault().Aluno.AlunoPessoaFisica.no_pessoa : "",
                                     considera_vagas = vagasTurma,
                                     existe_diario = t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada),
                                     nm_alunos_turmaPPT = turmasFilhas ? (from turma in db.Turma
                                                                          where turma.cd_turma_ppt == t.cd_turma_ppt &&
                                                                          turma.TurmaAluno.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                                       at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                                       at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial).Any()
                                                                          select turma.cd_turma).Count() : 0,
                                     vagas_disponiveis = turmasFilhas ? t.TurmaPai.T_SALA != null ? t.TurmaPai.T_SALA.nm_vaga_sala : t.TurmaPai.Sala != null ? t.TurmaPai.Sala.nm_vaga_sala :
                                                         t.T_SALA != null ? t.Curso != null ? t.T_SALA.nm_vaga_sala <= t.Curso.nm_vagas_curso ? t.T_SALA.nm_vaga_sala : (int)t.Curso.nm_vagas_curso : (int)t.T_SALA.nm_vaga_sala :
                                                         t.Sala != null ? t.Curso != null ? t.Sala.nm_vaga_sala <= t.Curso.nm_vagas_curso ? t.Sala.nm_vaga_sala : (int)t.Curso.nm_vagas_curso : (int)t.Sala.nm_vaga_sala :
                                                         t.Curso != null ? (int)t.Curso.nm_vagas_curso : 0 : 0
                                 });
                sqlSearch = sorter.Sort(sqlSearch);
                int limite = sqlSearch.Count();
                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                sqlSearch = sqlSearch.Skip(parametros.from).Take(parametros.qtd_limite);
                return sqlSearch.AsEnumerable<TurmaSearch>();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TurmaSearch> searchTurmasContrato(int cd_contrato, int cd_escola, int cd_aluno)
        {
            try
            {
                var sql = from t in db.Turma 
                          where
                             t.HistoricoAluno.Where(histA => t.cd_turma == histA.cd_turma && //histA.Turma.cd_pessoa_escola == cd_escola &&
                                                             histA.cd_contrato == cd_contrato && histA.cd_aluno == cd_aluno).Any()
                          select new TurmaSearch
                                 {
                                     cd_turma = t.cd_turma,
                                     cd_turma_ppt = t.cd_turma_ppt,
                                     no_turma = t.no_turma,
                                     cd_curso = t.cd_curso,
                                     cd_produto = t.cd_produto,
                                     no_curso = t.Curso.no_curso,
                                     no_duracao = t.Duracao.dc_duracao,
                                     no_produto = t.Produto.no_produto,
                                     no_professor = t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true).FirstOrDefault().Professor.FuncionarioPessoaFisica.no_pessoa,
                                     cd_situacao_aluno_turma = t.HistoricoAluno.Where(histA => histA.cd_contrato == cd_contrato && //histA.Turma.cd_pessoa_escola == cd_escola && 
                                                                                               histA.cd_aluno == cd_aluno).OrderByDescending(o => o.nm_sequencia).FirstOrDefault().id_situacao_historico
                                 };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        [Obsolete]
        //Metodo que verifica se todos os horários das turmas filhas estão contidas em algum dos horários da turma PPT. 
        public bool verifTurmasFilhasDisponiveisHorariosTurmaPPTBD(int cd_turma_PPT, int cd_escola, List<Horario> horariosTurmaPPT)
        {
            try
            {
                var retorno = false;
                if (horariosTurmaPPT != null && horariosTurmaPPT.Count() > 0){

                    var sql = from hf in db.Horario
                          join turmas in db.Turma on hf.cd_registro equals turmas.cd_turma
                          where turmas.cd_turma_ppt == cd_turma_PPT && turmas.dt_termino_turma == null
                          && hf.id_origem == (int)Horario.Origem.TURMA && turmas.cd_pessoa_escola == cd_escola
                          select hf;

                    foreach (var hp in horariosTurmaPPT)
                        sql = from h in sql
                              where
                                  (from hf in db.Horario
                                    where hf.cd_horario == h.cd_horario                                    
                                    && hf.id_dia_semana == hp.id_dia_semana
                                        && hf.dt_hora_ini >= hp.dt_hora_ini
                                        && hf.dt_hora_ini < hp.dt_hora_fim
                                        && hf.dt_hora_ini > hp.dt_hora_ini
                                        && hf.dt_hora_fim <= hp.dt_hora_fim
                                    select hf.cd_horario).Any()
                              select h;


                    retorno = sql.Count() > 0 ;
                }
                return retorno;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Busca as turmas e as programações de turma que possuem programações da turma com os feriados passados como parâmetro, que não possuem desconsiderar feriados, que não possuem diário de aula, 
        //para uma determinada escola
        public IEnumerable<Turma> getTurmaPorProgramacaoTurmaComFeriado(Feriado feriado, int? cd_escola, bool delecao_feriado)
        {
            try {
                DateTime data_inicial = DateTime.MinValue;
                DateTime data_final = DateTime.MinValue;
                DateTime data_inicial_posterior = DateTime.MinValue;
                DateTime data_final_posterior = DateTime.MinValue;
                DateTime data_inicial_anterior = DateTime.MinValue;
                DateTime data_final_anterior = DateTime.MinValue;

                DateTime data_agora = DateTime.Now;
                Feriado feriado_ano_posterior = (Feriado) feriado.Clone();
                Feriado feriado_ano_anterior = (Feriado) feriado.Clone();
                var result = from turmas in db.Turma.Include(pt => pt.ProgramacaoTurma)
                             where !turmas.id_turma_ppt
                             select turmas;
                if(cd_escola.HasValue)
                    result = from turmas in result
                             where turmas.cd_pessoa_escola == cd_escola
                             select turmas;

                try {
                    if(!feriado.aa_feriado.HasValue) {
                        feriado.aa_feriado = (short) data_agora.Year;
                        feriado.aa_feriado_fim = (short) data_agora.Year;

                        feriado_ano_posterior.aa_feriado = (short) (data_agora.Year + 1);
                        feriado_ano_posterior.aa_feriado_fim = (short) (data_agora.Year + 1);

                        feriado_ano_anterior.aa_feriado = (short) (data_agora.Year - 1);
                        feriado_ano_anterior.aa_feriado_fim = (short) (data_agora.Year - 1);
                    }

                    data_inicial = new DateTime(feriado.aa_feriado.Value, feriado.mm_feriado, feriado.dd_feriado);
                    data_final = new DateTime(feriado.aa_feriado_fim.Value, feriado.mm_feriado_fim.Value, feriado.dd_feriado_fim.Value);

                    data_inicial_posterior = new DateTime(feriado_ano_posterior.aa_feriado.Value, feriado_ano_posterior.mm_feriado, feriado_ano_posterior.dd_feriado);
                    data_final_posterior = new DateTime(feriado_ano_posterior.aa_feriado_fim.Value, feriado_ano_posterior.mm_feriado_fim.Value, feriado_ano_posterior.dd_feriado_fim.Value);

                    data_inicial_anterior = new DateTime(feriado_ano_anterior.aa_feriado.Value, feriado_ano_anterior.mm_feriado, feriado_ano_anterior.dd_feriado);
                    data_final_anterior = new DateTime(feriado_ano_anterior.aa_feriado_fim.Value, feriado_ano_anterior.mm_feriado_fim.Value, feriado_ano_anterior.dd_feriado_fim.Value);
                }
                catch(Exception exe) {
                    throw new DataAccessException("Erro de conversão das datas posteriores e anteriores.", exe);
                }

                IEnumerable<Turma> result2 = null;
                
                if(!delecao_feriado)
                    result2 = (from turmas in result
                               where //turmas.dt_termino_turma == null &&
                               turmas.ProgramacaoTurma.Where(p => //Verifica se há alguma programação da turma que esteja contida no feriado:
                                                                        //Não posso verificar pela data de fim da turma, pois temos anos nulos nos feriados.
                                                                        (
                                                                            (
                                                                              DbFunctions.TruncateTime(p.dta_programacao_turma) >= data_inicial && data_final <= DbFunctions.TruncateTime(p.dta_programacao_turma)
                                                                            )
                                                                            ||
                                                                            (
                                                                              DbFunctions.TruncateTime(p.dta_programacao_turma) >= data_inicial_posterior && data_final_posterior <= DbFunctions.TruncateTime(p.dta_programacao_turma)
                                                                            )
                                                                            ||
                                                                            (
                                                                              DbFunctions.TruncateTime(p.dta_programacao_turma) >= data_inicial_anterior && data_final_anterior <= DbFunctions.TruncateTime(p.dta_programacao_turma)
                                                                            )
                                                                        ) && !(from program in db.ProgramacaoTurma
                                                                                where program.cd_turma == turmas.cd_turma
                                                                                && program.id_aula_dada
                                                                                && program.nm_aula_programacao_turma > p.nm_aula_programacao_turma
                                                                                select program).Any()
                                                                  ).Any()
                               select new {
                                   no_turma = turmas.no_turma,
                                   cd_turma = turmas.cd_turma,
                                   cd_produto = turmas.cd_produto,
                                   cd_curso = turmas.cd_curso,
                                   cd_duracao = turmas.cd_duracao,
                                   cd_regime = turmas.cd_regime,
                                   cd_turma_ppt = turmas.cd_turma_ppt,
                                   cd_pessoa_escola = turmas.cd_pessoa_escola,
                                   dt_inicio_aula = turmas.dt_inicio_aula,
                                   ProgramacaoTurma = turmas.ProgramacaoTurma/*.Where(p => cd_feriados_int.Contains(p.Feriado.cod_feriado) 
                                                                                      && p.FeriadoDesconsiderado != null
                                                                                      && !(from program in db.ProgramacaoTurma
                                                                                            where program.cd_turma == turmas.cd_turma
                                                                                            && program.id_aula_dada
                                                                                            && program.nm_aula_programacao_turma > p.nm_aula_programacao_turma
                                                                                            select program).Any()
                                                                                )*/
                                                                             .OrderBy(pt => pt.nm_aula_programacao_turma)
                               }).ToList().Select(x => new Turma {
                                   no_turma = x.no_turma,
                                   cd_turma = x.cd_turma,
                                   cd_produto = x.cd_produto,
                                   cd_curso = x.cd_curso,
                                   cd_duracao = x.cd_duracao,
                                   cd_regime = x.cd_regime,
                                   cd_turma_ppt = x.cd_turma_ppt,
                                   cd_pessoa_escola = x.cd_pessoa_escola,
                                   dt_inicio_aula = x.dt_inicio_aula,
                                   ProgramacaoTurma = x.ProgramacaoTurma.ToList()
                               });
                else
                    result2 = (from turmas in result
                               where //turmas.dt_termino_turma == null &&
                               turmas.ProgramacaoTurma.Where(p => p.cd_feriado == feriado.cod_feriado
                                                                        && !(from program in db.ProgramacaoTurma
                                                                             where program.cd_turma == turmas.cd_turma
                                                                             && program.id_aula_dada
                                                                             && program.nm_aula_programacao_turma > p.nm_aula_programacao_turma
                                                                             select program).Any()
                                                                  ).Any()
                               select new
                               {
                                   no_turma = turmas.no_turma,
                                   cd_turma = turmas.cd_turma,
                                   cd_produto = turmas.cd_produto,
                                   cd_curso = turmas.cd_curso,
                                   cd_duracao = turmas.cd_duracao,
                                   cd_regime = turmas.cd_regime,
                                   cd_turma_ppt = turmas.cd_turma_ppt,
                                   cd_pessoa_escola = turmas.cd_pessoa_escola,
                                   dt_inicio_aula = turmas.dt_inicio_aula,
                                   ProgramacaoTurma = turmas.ProgramacaoTurma/*.Where(p => cd_feriados_int.Contains(p.Feriado.cod_feriado) 
                                                                                      && p.FeriadoDesconsiderado != null
                                                                                      && !(from program in db.ProgramacaoTurma
                                                                                            where program.cd_turma == turmas.cd_turma
                                                                                            && program.id_aula_dada
                                                                                            && program.nm_aula_programacao_turma > p.nm_aula_programacao_turma
                                                                                            select program).Any()
                                                                                )*/
                                                                             .OrderBy(pt => pt.nm_aula_programacao_turma)
                               }).ToList().Select(x => new Turma
                               {
                                   no_turma = x.no_turma,
                                   cd_turma = x.cd_turma,
                                   cd_produto = x.cd_produto,
                                   cd_curso = x.cd_curso,
                                   cd_duracao = x.cd_duracao,
                                   cd_regime = x.cd_regime,
                                   cd_turma_ppt = x.cd_turma_ppt,
                                   cd_pessoa_escola = x.cd_pessoa_escola,
                                   dt_inicio_aula = x.dt_inicio_aula,
                                   ProgramacaoTurma = x.ProgramacaoTurma.ToList()
                               });

                if(result2 != null) {
                    var turmas = result2.ToList();
                    //foreach(Turma turma in result2)
                    for(int i = turmas.Count-1; i >= 0; i--) {
                        Turma turma = turmas[i];
                        turma.horariosTurma = (from horario in db.Horario
                                                where horario.cd_pessoa_escola == turma.cd_pessoa_escola && horario.id_origem == (int) Horario.Origem.TURMA &&
                                                horario.cd_registro == turma.cd_turma
                                                select horario).ToList();
                        if(turma.horariosTurma.Count == 0)
                            turmas.Remove(turma);
                    }
                    result2 = turmas;
                }
                return result2;
            }
            catch(Exception exe) {
                throw new DataAccessException(" Ano de início do feriado: " + feriado.aa_feriado + " Mês de início do feriado: " + feriado.mm_feriado + " Dia de inicio do feriado: " + feriado.dd_feriado + 
                                                " Ano do fim do feriado: " + feriado.aa_feriado_fim + " Mês do fim do feriado: " + feriado.mm_feriado_fim + " Dia do fim do feriado: " + feriado.dd_feriado_fim, exe);
            }
        }

        public IEnumerable<TurmaSearch> searchTurmaComAluno(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
                                                   int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas, int cdAluno, DateTime? dtInicial,
                                                   DateTime? dtFinal, int? cd_turma_PPT, int cdTurmaOrigem, int opcao, int tipoConsulta, int cd_escola_combo_fk)
        {
            try
            {
                IEntitySorter<TurmaSearch> sorter = EntitySorter<TurmaSearch>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Turma> sql;


                bool? vagasTurma = (from p in db.Parametro.AsNoTracking()
                                    where p.cd_pessoa_escola == ((cd_escola_combo_fk == 0) ? cdEscola : cd_escola_combo_fk)
                                    select p.id_bloquear_matricula_vaga).FirstOrDefault();

                
                    sql = from t in db.Turma.AsNoTracking()
                          where (t.cd_pessoa_escola == cdEscola ||
                                 (from te in db.TurmaEscola
                                     where te.cd_turma == t.cd_turma &&
                                           te.cd_escola == cdEscola
                                     select te).Any())
                        select t;
               
                    

                if (opcao == (int)FundacaoFisk.SGF.Web.Services.Secretaria.Model.MudancasInternas.OpcoesMudanca.MudarTurma ||
                    opcao == (int)TipoConsultaTurmaEnum.DIARIO_AULA)
                {


                    if (cdTurmaOrigem > 0)
                        sql = from t in sql
                              where t.cd_turma != cdTurmaOrigem
                              select t;
                    else
                        if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                            sql = from t in sql
                                  where t.TurmaAluno.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado).Any()
                                  select t;
                        else
                            sql = from t in sql
                                  where t.TurmaAluno.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial).Any()
                                  select t;
                }

                if (opcao == (int)FundacaoFisk.SGF.Web.Services.Secretaria.Model.MudancasInternas.OpcoesMudanca.RetornarTurmaOri)
                    sql = from t in sql
                          where t.TurmaAluno.Where(at => at.cd_situacao_aluno_origem != 0 &&
                                                          (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                          at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                          at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando ||
                                                          at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Any()
                          select t;

                if (cdAluno > 0)
                    sql = from t in sql
                          where t.TurmaAluno.Where(a => a.cd_aluno == cdAluno).Any()
                          select t;
                if (opcao == (int)TipoConsultaTurmaEnum.CANCELA_DESISTENCIA)
                    sql = from t in db.Turma
                          where t.cd_pessoa_escola == cdEscola &&
                                 t.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Desistente && at.cd_contrato > 0 &&
                                                         (at.cd_aluno == cdAluno || cdAluno == 0)).Any()
                          select t;
                if (opcao == (int)TipoConsultaTurmaEnum.DESISTENCIA)
                    sql = from t in sql
                          where t.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                         at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                         at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial) && at.cd_contrato > 0 &&
                                                         (at.cd_aluno == cdAluno || cdAluno == 0)).Any()
                          select t;

                if (!string.IsNullOrEmpty(descricao))
                    if (inicio == true)
                        sql = from t in sql
                              where t.no_turma.StartsWith(descricao)
                              select t;
                    else
                        sql = from t in sql
                              where t.no_turma.Contains(descricao)
                              select t;

                if (!string.IsNullOrEmpty(apelido))
                    if (inicio == true)
                        sql = from t in sql
                              where t.no_apelido.StartsWith(apelido)
                              select t;
                    else
                        sql = from t in sql
                              where t.no_apelido.Contains(apelido)
                              select t;

                if (dtInicial.HasValue)
                    sql = from t in sql
                          where t.dt_inicio_aula >= dtInicial
                          select t;

                if (dtFinal.HasValue)
                    sql = from t in sql
                          where t.dt_inicio_aula <= dtFinal
                          select t;
                //Busca as turmas filhas aprtir do código da turma pai PPT
                if (cd_turma_PPT != null && cd_turma_PPT > 0)
                {
                    // Para trazer o nome do aluno e não trazer o nome do curso.
                    turmasFilhas = true;
                    sql = from t in sql
                          where t.cd_turma_ppt == cd_turma_PPT
                          select t;
                }
                else
                {
                    if (tipoTurma > 0)
                    {
                        if (tipoTurma == (int)Turma.TipoTurma.PPT)
                        {
                            if (!turmasFilhas && situacaoTurma > 0)
                            {
                                var ativo = true;
                                if (situacaoTurma == 2)
                                    ativo = false;
                                sql = from t in sql
                                      where t.id_turma_ativa == ativo
                                      select t;
                            }
                            if (turmasFilhas)
                            {
                                sql = from t in sql
                                      where t.cd_turma_ppt != null && t.id_turma_ppt == false
                                      select t;
                                if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                                    sql = from t in sql
                                          where t.dt_termino_turma == null
                                          && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                          select t;
                                if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                                    sql = from t in sql
                                          where t.dt_termino_turma != null
                                          select t;
                                if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                                    sql = from t in sql
                                          where t.dt_termino_turma == null
                                          && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                          select t;
                            }
                            else
                                sql = from t in sql
                                      where t.id_turma_ppt == true && t.cd_turma_ppt == null
                                      select t;

                        }
                        if (tipoTurma == (int)Turma.TipoTurma.NORMAL)
                            sql = from t in sql
                                  where t.id_turma_ppt == false &&
                                     t.cd_turma_ppt == null
                                  select t;
                    }
                    else
                        sql = from t in sql
                              where !t.id_turma_ppt
                              select t;
                    if (tipoTurma != (int)Turma.TipoTurma.PPT)
                    {
                        if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                            sql = from t in sql
                                  where t.dt_termino_turma == null
                                  && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                  select t;
                        if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                            sql = from t in sql
                                  where t.dt_termino_turma != null
                                  select t;
                        if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                            sql = from t in sql
                                  where t.dt_termino_turma == null
                                  && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                  select t;
                    }
                }

                if (cdCurso > 0)
                    sql = from t in sql
                          where t.cd_curso == cdCurso
                          select t;
                if (cdDuracao > 0)
                    sql = from t in sql
                          where t.cd_duracao == cdDuracao
                          select t;
                if (cdProduto > 0)
                    sql = from t in sql
                          where t.cd_produto == cdProduto
                          select t;

                switch (tipoConsulta)
                {
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA:
                        if (cdProfessor > 0)
                            sql = from t in sql
                                  where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor).Any()
                                  select t;
                        break;
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_FK_TURMA_PROF:
                        if (cdProfessor > 0)
                            sql = from t in sql
                                  where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo).Any()
                                  select t;

                        break;
                }

                switch (tipoProg)
                {
                    case ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_GERADAS:
                        sql = from t in sql
                              where t.ProgramacaoTurma.Any()
                              select t;
                        break;
                    case ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_NAO_GERADAS:
                        sql = from t in sql
                              where !t.ProgramacaoTurma.Any()
                              select t;
                        break;
                }

                var sqlSearch = (from t in sql
                                 select new TurmaSearch
                                 {
                                     cd_turma = t.cd_turma,
                                     cd_turma_ppt = t.cd_turma_ppt,
                                     cd_pessoa_escola = t.cd_pessoa_escola,
                                     no_turma = t.no_turma,
                                     no_apelido = t.no_apelido,
                                     cd_curso = t.cd_curso,
                                     no_curso =  t.Curso.no_curso,
                                     no_duracao = t.Duracao.dc_duracao,
                                     no_produto = t.Produto.no_produto,
                                     cd_produto = t.cd_produto,
                                     dt_inicio_aula = t.dt_inicio_aula,
                                     id_turma_ppt = t.id_turma_ppt,
                                     id_turma_ativa = t.id_turma_ativa,
                                     dta_termino_turma = t.dt_termino_turma,
                                     nro_alunos = t.id_turma_ppt == true ? (from turma in db.Turma
                                                                            where turma.cd_turma_ppt == t.cd_turma &
                                                                            turma.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Any()
                                                                            select turma.cd_turma).Count() :
                                     t.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Count(),
                                     no_professor = t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true && p.cd_turma == t.cd_turma).FirstOrDefault().Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                                     no_aluno = turmasFilhas ? t.TurmaAluno.FirstOrDefault().Aluno.AlunoPessoaFisica.no_pessoa : "",
                                     turmasFilhas = turmasFilhas,
                                     considera_vagas = vagasTurma,
                                     cd_aluno_turma = cdAluno > 0 ? t.TurmaAluno.Where(at => at.cd_aluno == cdAluno && at.cd_turma == t.cd_turma).FirstOrDefault().cd_aluno_turma : 0,
                                     cd_contrato = cdAluno > 0 ? t.TurmaAluno.Where(at => at.cd_aluno == cdAluno && at.cd_turma == t.cd_turma).FirstOrDefault().cd_contrato : 0,
                                     existe_diario = t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada),
                                     nm_alunos_turmaPPT = turmasFilhas ? (from turma in db.Turma
                                                                          where turma.cd_turma_ppt == t.cd_turma_ppt &&
                                                                          turma.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                                       at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                                       at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Any()
                                                                          select turma.cd_turma).Count() : 0,
                                     vagas_disponiveis = turmasFilhas ? t.TurmaPai.T_SALA != null ? t.TurmaPai.T_SALA.nm_vaga_sala : t.TurmaPai.Sala != null ? t.TurmaPai.Sala.nm_vaga_sala :
                                                         t.T_SALA != null ? t.Curso != null ? t.T_SALA.nm_vaga_sala <= t.Curso.nm_vagas_curso ? t.T_SALA.nm_vaga_sala : (int)t.Curso.nm_vagas_curso : (int)t.T_SALA.nm_vaga_sala :
                                                         t.Sala != null ? t.Curso != null ? t.Sala.nm_vaga_sala <= t.Curso.nm_vagas_curso ? t.Sala.nm_vaga_sala : (int)t.Curso.nm_vagas_curso : (int)t.Sala.nm_vaga_sala :
                                                         t.Curso != null ? (int)t.Curso.nm_vagas_curso : 0 : 0,
                                     dt_ultima_aula = t.DiarioAula.OrderByDescending(d => d.dt_aula).ThenByDescending(d => d.hr_inicial_aula).FirstOrDefault().dt_aula
                                 });
                sqlSearch = sorter.Sort(sqlSearch);
                int limite = sqlSearch.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sqlSearch = sqlSearch.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sqlSearch.AsEnumerable<TurmaSearch>();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Turma findTurmaOrigem(int cdEscola, int cd_turma, int cd_aluno)
        {
            try
            {
                var sql = (from t in db.Turma
                           where db.AlunoTurma.Where(ta => ta.cd_turma == cd_turma && ta.cd_aluno == cd_aluno && ta.cd_turma_origem == t.cd_turma).Any()
                           select t).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TurmaSearch> searchTurmaAlunoDesistente(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso,
                                            int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg,
                                            int cdEscola, bool turmasFilhas,  DateTime? dtInicial,DateTime? dtFinal, int tipoConsulta)
        {
            try
            {

                IEntitySorter<TurmaSearch> sorter = EntitySorter<TurmaSearch>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Turma> sql;

                sql = from t in db.Turma.AsNoTracking()
                      where
                        //t.cd_pessoa_escola == cdEscola &&
                        t.dt_termino_turma == null &&
                        t.id_turma_ppt == false &&
                        db.AlunoTurma.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && at.cd_turma == t.cd_turma &&
                                                 at.Desistencia.Where(d => d.cd_aluno_turma == at.cd_aluno_turma).Any()).Any()
                      select t;


                if (!string.IsNullOrEmpty(descricao))
                    if (inicio == true)
                        sql = from t in sql
                              where t.no_turma.StartsWith(descricao)
                              select t;
                    else
                        sql = from t in sql
                              where t.no_turma.Contains(descricao)
                              select t;

                if (!string.IsNullOrEmpty(apelido))
                    if (inicio == true)
                        sql = from t in sql
                              where t.no_apelido.StartsWith(apelido)
                              select t;
                    else
                        sql = from t in sql
                              where t.no_apelido.Contains(apelido)
                              select t;

                if (dtInicial.HasValue)
                    sql = from t in sql
                          where t.dt_inicio_aula >= dtInicial
                          select t;

                if (dtFinal.HasValue)
                    sql = from t in sql
                          where t.dt_inicio_aula <= dtFinal
                          select t;

                //Busca as turmas filhas aprtir do código da turma pai PPT
                if (tipoTurma > 0)
                {
                    if (tipoTurma == (int)Turma.TipoTurma.PPT)
                    {
                        if (!turmasFilhas && situacaoTurma > 0)
                        {
                            var ativo = true;
                            if (situacaoTurma == 2)
                                ativo = false;
                            sql = from t in sql
                                  where t.id_turma_ativa == ativo
                                  select t;
                        }
                        if (situacaoTurma > 0)
                        {
                            if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                                sql = from t in sql
                                      where t.dt_termino_turma == null
                                      && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                      select t;
                            if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                                sql = from t in sql
                                      where t.dt_termino_turma != null
                                      select t;
                            if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                                sql = from t in sql
                                      where t.dt_termino_turma == null
                                      && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                      select t;
                        }
                        else
                            sql = from t in sql
                                  where t.id_turma_ppt == true && t.cd_turma_ppt == null
                                  select t;

                    }
                    if (tipoTurma == (int)Turma.TipoTurma.NORMAL)
                        sql = from t in sql
                              where t.id_turma_ppt == false &&
                                 t.cd_turma_ppt == null
                              select t;
                }
                else
                    sql = from t in sql
                          where !t.id_turma_ppt
                          select t;
                    

                if ((tipoTurma != (int)Turma.TipoTurma.PPT) || (tipoTurma == (int)Turma.TipoTurma.PPT && turmasFilhas))
                {
                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                        sql = from t in sql
                              where t.dt_termino_turma == null
                              && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                              select t;
                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                        sql = from t in sql
                              where t.dt_termino_turma != null
                              select t;
                    if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                        sql = from t in sql
                              where t.dt_termino_turma == null
                              && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                              select t;
                }

                if (cdCurso > 0)
                    sql = from t in sql
                          where t.cd_curso == cdCurso
                          select t;
                if (cdDuracao > 0)
                    sql = from t in sql
                          where t.cd_duracao == cdDuracao
                          select t;
                if (cdProduto > 0)
                    sql = from t in sql
                          where t.cd_produto == cdProduto
                          select t;

                switch (tipoConsulta)
                {
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA:
                        if (cdProfessor > 0)
                            sql = from t in sql
                                  where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor).Any()
                                  select t;
                        break;
                    case (int)TipoConsultaTurmaEnum.HAS_PESQ_FK_TURMA_PROF:
                        if (cdProfessor > 0)
                            sql = from t in sql
                                  where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo).Any()
                                  select t;

                        break;
                }

                switch (tipoProg)
                {
                    case ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_GERADAS:
                        sql = from t in sql
                              where t.ProgramacaoTurma.Any()
                              select t;
                        break;
                    case ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_NAO_GERADAS:
                        sql = from t in sql
                              where !t.ProgramacaoTurma.Any()
                              select t;
                        break;

                }

                var sqlSearch = (from t in sql
                                 select new TurmaSearch
                                 {
                                     cd_turma = t.cd_turma,
                                     cd_turma_ppt = t.cd_turma_ppt,
                                     no_turma = t.no_turma,
                                     cd_curso = t.cd_curso,
                                     no_curso = t.Curso.no_curso,
                                     cd_duracao = t.cd_duracao,
                                     no_duracao = t.Duracao.dc_duracao,
                                     cd_produto = t.cd_produto,
                                     no_produto = t.Produto.no_produto,
                                     cd_regime = t.cd_regime,
                                     dt_inicio_aula = t.dt_inicio_aula,
                                     id_turma_ppt = t.id_turma_ppt,
                                     id_turma_ativa = t.id_turma_ativa,
                                     dta_termino_turma = t.dt_termino_turma,
                                     nro_alunos = t.id_turma_ppt == true ? (from turma in db.Turma
                                                                            where turma.cd_turma_ppt == t.cd_turma &&
                                                                            turma.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && 
                                                                                (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                            at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)).Any()
                                                                            select turma.cd_turma).Count() :
                                    t.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola &&
                                                            (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                             at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)).Count(),
                                     no_professor = t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true).FirstOrDefault().Professor.FuncionarioPessoaFisica.no_pessoa,
                                     existe_diario = t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada),
                                     nm_alunos_turmaPPT = turmasFilhas ? (from turma in db.Turma
                                                                          where turma.cd_turma_ppt == t.cd_turma_ppt &&
                                                                          turma.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                                       at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                                       at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Any()
                                                                          select turma.cd_turma).Count() : 0,
                                     vagas_disponiveis = turmasFilhas ? t.TurmaPai.T_SALA != null ? t.TurmaPai.T_SALA.nm_vaga_sala : t.TurmaPai.Sala != null ? t.TurmaPai.Sala.nm_vaga_sala :
                                                         t.T_SALA != null ? t.Curso != null ? t.T_SALA.nm_vaga_sala <= t.Curso.nm_vagas_curso ? t.T_SALA.nm_vaga_sala : (int)t.Curso.nm_vagas_curso : (int)t.T_SALA.nm_vaga_sala :
                                                         t.Sala != null ? t.Curso != null ? t.Sala.nm_vaga_sala <= t.Curso.nm_vagas_curso ? t.Sala.nm_vaga_sala : (int)t.Curso.nm_vagas_curso : (int)t.Sala.nm_vaga_sala :
                                                         t.Curso != null ? (int)t.Curso.nm_vagas_curso : 0 : 0
                                 });
                sqlSearch = sorter.Sort(sqlSearch);
                int limite = sqlSearch.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sqlSearch = sqlSearch.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sqlSearch.AsEnumerable<TurmaSearch>();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Estagio> getEstagiosHistoricoAluno(int cd_aluno, int cd_escola)
        {
            try
            {
                var sql = (from h in db.HistoricoAluno
                        join t in db.Turma on h.cd_turma equals t.cd_turma
                        join cr in db.Curso on t.cd_curso equals cr.cd_curso
                        join a in db.Aluno on h.cd_aluno equals a.cd_aluno
                        join ta in db.AlunoTurma on new {h.cd_turma, h.cd_aluno} equals new {ta.cd_turma, ta.cd_aluno}
                        orderby h.cd_aluno, h.cd_produto, h.nm_sequencia
                        where h.cd_aluno == cd_aluno &&
                           h.nm_sequencia == (from hi in db.HistoricoAluno
                                where hi.cd_aluno == h.cd_aluno && /*t.cd_pessoa_escola == cd_escola &&*/
                                      hi.cd_turma == h.cd_turma && hi.cd_produto == h.cd_produto
                                      select hi).Max(x=> x.nm_sequencia)
                                    select new 
                                    {
                                        cd_estagio = cr.cd_estagio,
                                        cd_turma = h.cd_turma,
                                        cd_produto = h.cd_produto,
                                        nm_aulas_dadas = ta.nm_aulas_dadas,
                                        nm_faltas = ta.nm_faltas,
                                        nm_aulas_contratadas = (from pt in db.ProgramacaoTurma
                                                                where pt.cd_turma == t.cd_turma &&
                                                                      (pt.cd_feriado == null || pt.cd_feriado_desconsiderado != null)
                                                                      select pt).Count(),
                                        dt_aula_min = (from d in db.DiarioAula 
                                                        where d.id_status_aula == 0 &&
                                                              d.cd_turma == h.cd_turma
                                                       select d).Min(z => z.dt_aula) != null ? 
                                                        (from d in db.DiarioAula
                                                        where d.id_status_aula == 0 &&
                                                              d.cd_turma == h.cd_turma
                                                        select d).Min(z => z.dt_aula) : default(DateTime),
                                        dt_aula_max = (from d in db.DiarioAula
                                                        where d.id_status_aula == 0 &&
                                                              d.cd_turma == h.cd_turma
                                                              select d).Max(b => b.dt_aula) != null ? 
                                                        (from d in db.DiarioAula
                                                        where d.id_status_aula == 0 &&
                                                              d.cd_turma == h.cd_turma
                                                         select d).Max(b => b.dt_aula) : default(DateTime),
                                        nm_sequencia = h.nm_sequencia
                                    }).ToList().Select(x => new TurmasHistoricoUI
                                    {
                                        cd_estagio = x.cd_estagio,
                                        cd_turma = x.cd_turma,
                                        cd_produto = x.cd_produto,
                                        nm_aulas_dadas = x.nm_aulas_dadas,
                                        nm_aulas_contratadas = x.nm_aulas_contratadas,
                                        nm_faltas = x.nm_faltas,
                                        dt_aula_min = x.dt_aula_min,
                                        dt_aula_max = x.dt_aula_max,
                                        nm_sequencia = x.nm_sequencia
                                    });
                        

                    var sql2 = (from t in sql
                        join e in db.Estagio on t.cd_estagio equals e.cd_estagio
                        join p in db.Produto on t.cd_produto equals p.cd_produto 
                        //orderby t.cd_produto, e.no_estagio, p.no_produto
                        select new
                        {
                                cd_estagio = e.cd_estagio,
                                no_estagio = e.no_estagio,
                                no_estagio_abreviado = e.no_estagio_abreviado,
                                no_produto = p.no_produto,
                                cd_produto = t.cd_produto,
                                nm_aulas_dadas = t.nm_aulas_dadas,
                                nm_faltas = t.nm_faltas,
                                nm_aulas_contratadas = t.nm_aulas_contratadas,
                                primeira_aula = t.dt_aula_min,
                                ultima_aula = t.dt_aula_max,
                                nm_sequencia = t.nm_sequencia

                        }).ToList().Select(k => new Estagio()
                        {
                            cd_estagio = k.cd_estagio,
                            nm_aulas_dadas = k.nm_aulas_dadas,
                            nm_faltas = (byte?)k.nm_faltas,
                            nm_aulas_contratadas = k.nm_aulas_contratadas,
                            no_estagio = string.Format("{0} - {1}", k.no_estagio_abreviado, k.no_estagio),
                            cd_produto = k.cd_produto,
                            no_produto = k.no_produto,
                            primeira_aula = k.primeira_aula != null ? k.primeira_aula : default(DateTime),
                            ultima_aula = k.ultima_aula != null ? k.ultima_aula : default(DateTime),
                            nm_ordem_estagio = k.nm_sequencia
                        });
                

                    return sql2;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Turma> getTurmasAvaliacoes(int cd_aluno, int cd_escola) {
            try
            {
                var sql = (from t in db.Turma
                    join h in db.HistoricoAluno on t.cd_turma equals h.cd_turma
                           where /*t.cd_pessoa_escola == cd_escola &&*/ h.cd_aluno == cd_aluno &&
                          h.nm_sequencia == (from hi in db.HistoricoAluno
                              where hi.cd_aluno == h.cd_aluno /*&& t.cd_pessoa_escola == cd_escola*/ &&
                                    hi.cd_turma == h.cd_turma && hi.cd_produto == h.cd_produto
                              select hi).Max(x => x.nm_sequencia)
                          && t.TurmaAluno.Where(at => at.cd_aluno == cd_aluno && at.cd_contrato > 0).Any()
                    orderby h.cd_produto, h.nm_sequencia, h.cd_aluno
                    select new
                    {
                        no_turma = t.no_turma,
                        cd_turma = t.cd_turma,
                        no_produto = t.Produto.no_produto,
                        no_sala = t.Sala.no_sala,
                        no_professor = t.TurmaProfessorTurma
                            .Where(p => p.id_professor_ativo == true && p.cd_turma == t.cd_turma).FirstOrDefault()
                            .Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                        dt_inicio_aula = t.dt_inicio_aula,
                        dt_final_aula = t.dt_final_aula,
                        dt_termino_turma = t.dt_termino_turma,
                        cd_produto = h.cd_produto,
                        sequencia = h.nm_sequencia,
                        nm_aulas_dadas = t.TurmaAluno.Where(at => at.cd_aluno == cd_aluno).FirstOrDefault()
                            .nm_aulas_dadas,
                        nm_faltas = t.TurmaAluno.Where(at => at.cd_aluno == cd_aluno).FirstOrDefault().nm_faltas,
                        nm_aulas_contratadas = t.ProgramacaoTurma
                            .Where(pt => !pt.cd_feriado.HasValue || pt.cd_feriado_desconsiderado.HasValue).Count()
                    }).Distinct().ToList().Select(x => new Turma
                {
                    no_turma = x.no_turma,
                    cd_turma = x.cd_turma,
                    Produto = new Produto() {no_produto = x.no_produto, cd_produto = x.cd_produto},
                    no_sala = x.no_sala,
                    no_professor = x.no_professor,
                    dt_inicio_aula = x.dt_inicio_aula,
                    dt_final_aula = x.dt_final_aula,
                    dt_termino_turma = x.dt_termino_turma,
                    nm_aulas_dadas = x.nm_aulas_dadas,
                    nm_faltas = x.nm_faltas,
                    nm_aulas_contratadas = x.nm_aulas_contratadas,
                    sequencia = x.sequencia
                }).OrderBy(x => x.Produto.cd_produto).ThenBy(x => x.sequencia);

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

         public bool verificaTurmaSeExisteAlunoComSitacaoDifAguard(int cd_turma, int cd_empresa)
        {
            try
            {

                int cd_turma_context = (from t in db.Turma
                       where t.cd_turma == cd_turma && t.cd_pessoa_escola == cd_empresa &&
                             t.TurmaAluno.Where(at => at.cd_situacao_aluno_turma != (int)AlunoTurma.SituacaoAlunoTurma.Aguardando).Any()
                       select t.cd_turma).FirstOrDefault();
                return cd_turma_context > 0 ? true : false;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

         public Turma getTurmaAlunoAguard(int cd_pessoa_aluno, int cd_empresa)
         {
             try
             {
                 Turma turmaAguardando = new Turma();
                 IEnumerable<Turma> turma = (from t in db.Turma
                                             where t.cd_pessoa_escola == cd_empresa &&
                                                   t.TurmaAluno.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando && at.Aluno.cd_pessoa_aluno == cd_pessoa_aluno).Any()
                                             select new
                                             {
                                                 no_turma = t.no_turma,
                                                 cd_turma = t.cd_turma,
                                                 no_produto = t.Produto.no_produto,
                                                 produto = t.Produto,
                                                 cd_curso = t.cd_curso,
                                                 cd_regime = t.cd_regime,
                                                 cd_duracao = t.cd_duracao,
                                                 no_professor = t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true && p.cd_turma == t.cd_turma).FirstOrDefault().Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,

                                             }).ToList().Select(x => new Turma
                                             {
                                                 no_turma = x.no_turma,
                                                 cd_turma = x.cd_turma,
                                                 Produto = x.produto,
                                                 cd_curso = x.cd_curso,
                                                 cd_regime = x.cd_regime,
                                                 cd_duracao = x.cd_duracao,
                                                 no_professor = x.no_professor

                                             });                                            
                 if (turma.Count() == 1)
                     turmaAguardando = turma.ToList()[0];
                 return turmaAguardando;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

         public TurmaSearch searchTurmaComAlunoDesistencia(int cdEscola, int cdAluno, int opcao)
         {
             try
             {
                 IQueryable<Turma> sql;
                 TurmaSearch sqlSearch = new TurmaSearch();

                 var verififcaTurma = (from at in db.AlunoTurma
                                      where at.Aluno.cd_pessoa_escola == cdEscola &&
                                           at.Aluno.cd_aluno == cdAluno &&
                                           at.cd_contrato > 0
                                      select at.cd_turma).ToList();
                 if (verififcaTurma != null && verififcaTurma.Count() == 1)
                 {

                     sql = from t in db.Turma
                           where verififcaTurma.Contains(t.cd_turma)
                           select t;

                     if (opcao == (int)TipoConsultaTurmaEnum.CANCELA_DESISTENCIA)
                         sql = from t in sql
                               where t.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Desistente).Any()
                               select t;
                     if (opcao == (int)TipoConsultaTurmaEnum.DESISTENCIA)
                         sql = from t in sql
                               where t.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && 
                                                            (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                              at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Any()
                               select t;

                     sqlSearch = (from t in sql
                                  select new TurmaSearch
                                  {   cd_turma = t.cd_turma,
                                      no_turma = t.no_turma
                                  }).FirstOrDefault();
                 }

                 return sqlSearch;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

         public IEnumerable<TurmaSearch> getRptTurmas(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int tipoProg, bool turmasFilhas,
           DateTime? pDtaI, DateTime? pDtaF, DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int situacaoTurma, List<int> situacaoAlunoTurma, int tipoOnline, string dias)
         {
             try
             {
                 SGFWebContext dbContext = new SGFWebContext();
                 int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                 IQueryable<Turma> sql;

                 
                 sql = (from t in db.Turma
                        where (t.cd_pessoa_escola == cd_escola ||
                               (from te in db.TurmaEscola
                                   where te.cd_turma == t.cd_turma &&
                                         te.cd_escola == cd_escola
                                   select te).Any())
                        select t);
                 
                if (dias.Contains("1"))
                        sql = from t in sql
                              where db.Horario.Any(h => //h.cd_pessoa_escola == cdEscola && 
                                  h.id_origem == (int)Horario.Origem.TURMA && h.cd_registro == t.cd_turma &&
                                  dias.Substring(h.id_dia_semana-1,1) == "1")
                              select t;
                

                 if (tipoOnline > 0)
                 {
                     if (tipoOnline == (int)Turma.TipoOnline.PRESENCIAL)
                     {
                         sql = from at in sql
                               where at.cd_sala_online == null
                               select at;
                     }
                     else if (tipoOnline == (int)Turma.TipoOnline.ONLINE)
                     {
                         sql = from at in sql
                               where at.cd_sala_online != null
                               select at;
                     }
                 }

                 if (pDtaI.HasValue)
                     sql = from at in sql
                           where at.dt_inicio_aula >= pDtaI
                           select at;

                 if (pDtaF.HasValue)
                     sql = from at in sql
                           where at.dt_inicio_aula <= pDtaF
                           select at;

                 if (pDtaFimI.HasValue)
                     sql = from at in sql
                           where at.dt_final_aula >= pDtaFimI
                           select at;

                 if (pDtaFimF.HasValue)
                     sql = from at in sql
                           where at.dt_final_aula <= pDtaFimF
                           select at;

                 if (cdCurso > 0)
                     sql = from t in sql
                           where t.cd_curso == cdCurso
                           select t;
                 if (cdDuracao > 0)
                     sql = from t in sql
                           where t.cd_duracao == cdDuracao
                           select t;
                 if (cdProduto > 0)
                     sql = from t in sql
                           where t.cd_produto == cdProduto
                           select t;
                 if (cdProfessor > 0)
                     sql = from t in sql
                           where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor).Any()
                           select t;
                 switch (tipoProg)
                 {
                     case (int)ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_GERADAS:
                         sql = from t in sql
                               where t.ProgramacaoTurma.Any()
                               select t;
                         break;
                     case (int)ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_NAO_GERADAS:
                         sql = from t in sql
                               where !t.ProgramacaoTurma.Any()
                               select t;
                         break;
                 }

                 if (tipoTurma > 0)
                 {
                     if (tipoTurma == (int)Turma.TipoTurma.PPT)
                     {
                         if (!turmasFilhas && situacaoTurma > 0)
                         {
                             var ativo = true;
                             if (situacaoTurma == 2)
                                 ativo = false;
                             sql = from t in sql
                                   where t.id_turma_ativa == ativo
                                   select t;
                         }

                         if (turmasFilhas)
                         {
                             sql = from t in sql
                                   where t.cd_turma_ppt != null && t.id_turma_ppt == false
                                   select t;
                             if (situacaoTurma > 0)
                             {
                                 if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                                     sql = from t in sql
                                           where t.dt_termino_turma == null
                                           && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                           select t;
                                 if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                                     sql = from t in sql
                                           where t.dt_termino_turma != null
                                           select t;
                                 if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                                     sql = from t in sql
                                           where t.dt_termino_turma == null
                                           && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                           select t;
                                 if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASAENCERRAR)
                                     sql = from t in sql
                                           where t.dt_termino_turma == null && (t.dt_final_aula <= pDtaFimF && t.dt_final_aula != null)
                                           && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                           select t;
                             }
                         }
                         else
                             sql = from t in sql
                                   where t.id_turma_ppt == true && t.cd_turma_ppt == null
                                   select t;

                     }
                     if (tipoTurma == (int)Turma.TipoTurma.NORMAL)
                     {
                         sql = from t in sql
                               where t.id_turma_ppt == false &&
                                  t.cd_turma_ppt == null
                               select t;
                         if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                             sql = from t in sql
                                   where t.dt_termino_turma == null
                                   && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                   select t;
                         if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                             sql = from t in sql
                                   where t.dt_termino_turma != null
                                   select t;
                         if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                             sql = from t in sql
                                   where t.dt_termino_turma == null
                                   && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                   select t;
                         if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASAENCERRAR)
                             sql = from t in sql
                                   where t.dt_termino_turma == null && (t.dt_final_aula <= pDtaFimF && t.dt_final_aula != null)
                                   && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                   select t;
                     }
                 }
                 else
                 {
                     sql = from t in sql
                           where !t.id_turma_ppt
                           select t;
                     if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                         sql = from t in sql
                               where t.dt_termino_turma == null
                               && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                               select t;
                     if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                         sql = from t in sql
                               where t.dt_termino_turma != null
                               select t;
                     if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                         sql = from t in sql
                               where t.dt_termino_turma == null
                               && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                               select t;
                     if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASAENCERRAR)
                         sql = from t in sql
                               where t.dt_termino_turma == null && (t.dt_final_aula <= pDtaFimF && t.dt_final_aula != null)
                               && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                               select t;
                 }
                 if (!situacaoAlunoTurma.Contains(100) && situacaoAlunoTurma.Count() > 0)
                     sql = from t in sql
                           where t.TurmaAluno.Where(at => at.cd_situacao_aluno_turma.HasValue && situacaoAlunoTurma.Contains(at.cd_situacao_aluno_turma.Value)).Any() ||
                           (t.id_turma_ppt && t.TurmaFilhasPPT.Where(f => f.TurmaAluno.Where(atp => atp.cd_situacao_aluno_turma.HasValue && situacaoAlunoTurma.Contains(atp.cd_situacao_aluno_turma.Value)).Any()).Any())
                           select t;
                 var sqlSearch = (from t in sql
                                  select new TurmaSearch
                                  {
                                      cd_turma = t.cd_turma,
                                      id_turma_ppt = t.id_turma_ppt,
                                      no_turma = t.no_turma,
                                      no_turma_ppt = t.TurmaPai.no_turma,
                                      no_apelido = t.no_apelido,
                                      no_curso = t.Curso.no_curso,
                                      no_duracao = t.Duracao.dc_duracao,
                                      no_produto = t.Produto.no_produto,
                                      no_sala = (t.cd_sala_online == null)? t.Sala.no_sala:  t.T_SALA.no_sala,
                                      dt_inicio_aula = t.dt_inicio_aula,
                                      dt_final_aula = t.dt_final_aula,
                                      dta_termino_turma = t.dt_termino_turma,
                                      pessoasTurma = db.PessoaSGF.Where(p => db.FuncionarioSGF.Where(pf => pf.cd_pessoa_funcionario == p.cd_pessoa &&
                                                                        t.TurmaProfessorTurma.Where(pt => pt.cd_professor == pf.cd_funcionario).Any()).Any()),
                                      ultima_aula = t.DiarioAula.OrderByDescending(d => d.dt_aula).ThenByDescending(d => d.hr_inicial_aula).FirstOrDefault().tx_obs_aula,
                                      nro_alunos = t.id_turma_ppt == true ? (from turma in db.Turma
                                                                             where turma.cd_pessoa_escola == cd_escola && turma.cd_turma_ppt == t.cd_turma &&
                                                                                   turma.TurmaAluno.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                   at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                   at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial).Any()
                                                                             select turma.cd_turma).Count() :
                                                                             t.TurmaAluno.Where(at => (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                                      at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                                      at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial) && at.Aluno.cd_pessoa_escola == cd_escola).Count(),
                                      qtd_devedores = (from at in db.AlunoTurma.Where(al => t.id_turma_ppt == true ? (al.Turma.cd_turma_ppt == t.cd_turma) : (al.cd_turma == t.cd_turma))
                                                        join c in db.Contrato on at.cd_contrato equals c.cd_contrato
                                                        where db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                                                            ti.id_origem_titulo == cd_origem &&
                                                            c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&
                                                            ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.ABERTO &&
                                                            ti.id_status_cnab == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                                                            && ti.dt_vcto_titulo >= pDtaFimI.Value && ti.dt_vcto_titulo <= pDtaFimF.Value
                                                            && !ti.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                 x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)).Any()
                                                           && at.Aluno.cd_pessoa_escola == cd_escola
                                                       select at).Count(),
                                      tipo_online = (t.cd_sala_online == null) ? 1 : 2,
                                      dt_ultima_aula = t.DiarioAula.OrderByDescending(d => d.dt_aula).ThenByDescending(d => d.hr_inicial_aula).FirstOrDefault().dt_aula
                                  });

                 return sqlSearch;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }       
         
         public IEnumerable<TurmaSearch> getRptTurmasProgAula(int cd_turma, int cd_escola, DateTime? pDtaI, DateTime? pDtaF)
         {
             try
             {
                 var sql = from at in db.Turma
                           where at.cd_pessoa_escola == cd_escola && at.ProgramacaoTurma.Any()
                           select at;
                 if (cd_turma > 0)
                     sql = from at in sql
                           where at.cd_turma == cd_turma
                           select at;
                 else
                 {
                     if (pDtaI.HasValue)
                         sql = from t in sql
                               where t.dt_inicio_aula >= pDtaI
                               select t;

                     if (pDtaF.HasValue)
                         sql = from t in sql
                               where t.dt_inicio_aula <= pDtaF
                               select t;
                 }
                 var sqlSearch = (from t in sql
                                  select new TurmaSearch
                                  {
                                    cd_turma = t.cd_turma,
                                    cd_pessoa_escola = t.cd_pessoa_escola,
                                    no_turma = t.no_turma,
                                    no_apelido = t.no_apelido,
                                    no_curso = t.Curso.no_curso,
                                    no_duracao = t.Duracao.dc_duracao,
                                    no_produto = t.Produto.no_produto,
                                    pessoasTurma = db.PessoaSGF.Where(p => db.FuncionarioSGF.Where(pf => pf.cd_pessoa_funcionario == p.cd_pessoa &&
                                                                        t.TurmaProfessorTurma.Where(pt => pt.cd_professor == pf.cd_funcionario &&
                                                                                                          pt.id_professor_ativo).Any()).Any()),
                                   no_aluno = t.cd_turma_ppt > 0 ? t.TurmaAluno.FirstOrDefault().Aluno.AlunoPessoaFisica.no_pessoa : ""
                                  });
                 return sqlSearch;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

         public IEnumerable<ReportTurmaMatriculaMaterial> getRptTurmasMatriculaMaterial(int cd_escola, int cd_turma, int cd_aluno, int cd_item, int nm_contrato, DateTime? pDtaI, DateTime? pDtaF)
         {
             try
             {
                 /*var sql = from im in db.ItemMovimento
                           where im.Item.ItemEscola.Any(ie => ie.cd_item_escola == cd_escola)
                           select im;
                 if (cd_turma > 0){
                     SGFWebContext dbComp = new SGFWebContext();
                     int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                     
                     sql = from im in sql
                           where im.Movimento.id_origem_movimento == cd_origem &&
                                    db.Contrato.Any(c => c.cd_contrato == im.Movimento.cd_origem_movimento 
                                        && c.AlunoTurma.Any(at => at.cd_turma == cd_turma &&
                                        (at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                            at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                            at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Encerrado))
                                    )
                                                      
                           select im;
                 }
                 if (cd_aluno > 0)
                 {
                     SGFWebContext dbComp = new SGFWebContext();
                     int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                     sql = from im in sql
                           where im.Movimento.id_origem_movimento == cd_origem &&
                                    db.Contrato.Any(c => c.cd_contrato == im.Movimento.cd_origem_movimento
                                        && c.AlunoTurma.Any(at => at.cd_aluno == cd_aluno &&
                                        (at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                            at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                            at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Encerrado))
                                    )

                           select im;
                 }
                 if (nm_contrato > 0)
                 {
                     SGFWebContext dbComp = new SGFWebContext();
                     int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                     sql = from im in sql
                           where im.Movimento.id_origem_movimento == cd_origem &&
                                    db.Contrato.Any(c => c.cd_contrato == im.Movimento.cd_origem_movimento && c.nm_contrato == nm_contrato)

                           select im;
                 }
                 if (cd_item > 0)
                     sql = from im in sql
                           where im.cd_item == cd_item
                           select im;

                 if (pDtaI.HasValue)
                 {
                     SGFWebContext dbComp = new SGFWebContext();
                     int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                     sql = from im in sql
                           where im.Movimento.id_origem_movimento == cd_origem &&
                                    db.Contrato.Any(c => c.cd_contrato == im.Movimento.cd_origem_movimento && c.nm_contrato == nm_contrato && c.dt_matricula_contrato >= pDtaI)
                           select im;
                 }

                 if (pDtaF.HasValue)
                 {
                     SGFWebContext dbComp = new SGFWebContext();
                     int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                     sql = from im in sql
                           where im.Movimento.id_origem_movimento == cd_origem &&
                                    db.Contrato.Any(c => c.cd_contrato == im.Movimento.cd_origem_movimento && c.nm_contrato == nm_contrato && c.dt_matricula_contrato <= pDtaF)
                           select im;
                 }

                 var retorno = (from im in sql
                                  
                                select new ReportTurmaMatriculaMaterial
                                  {
                                      cd_aluno = db.Contrato.Where(c => c.cd_contrato == im.Movimento.cd_origem_movimento
                                        && (c.nm_contrato == nm_contrato)
                                        && c.AlunoTurma.Any(at => (cd_aluno == 0 || at.cd_aluno == cd_aluno) && (cd_turma == 0 || at.cd_turma == cd_turma) &&
                                        (at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                            at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                            at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Encerrado))).FirstOrDefault().cd_aluno
                                  });
                  */
                 SGFWebContext dbComp = new SGFWebContext();
                 int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                 var sql = (from im in db.ItemMovimento
                            from ii in db.Item
                            from m in db.Movimento
                            from c in db.Contrato
                            from at in db.AlunoTurma
                            from a in db.Aluno
                            from t in db.Turma
                            from pl in db.PessoaSGF
                            where 
                                im.cd_movimento == m.cd_movimento &&
                                c.cd_contrato == at.cd_contrato &&
                                at.cd_aluno == a.cd_aluno &&
                                at.cd_turma == t.cd_turma &&
                                a.cd_pessoa_aluno == pl.cd_pessoa &&
                                m.cd_origem_movimento == c.cd_contrato &&
                                m.cd_pessoa_empresa == cd_escola &&
                                ii.cd_item == im.cd_item &&
                                (from ie in db.ItemEscola 
                                 from i in db.Item
                                 where 
                                     i.cd_item == im.cd_item &&
                                     ie.cd_pessoa_escola == cd_escola
                                 select ie).Any() &&
                                 m.id_origem_movimento == cd_origem

                                       //nm_contrato:
                                  && (nm_contrato == 0 || c.nm_contrato == nm_contrato)

                                  //cd_item:
                                  && (cd_item == 0 || im.cd_item == cd_item)

                                  //cd_turma ou cd_aluno:
                                  && (cd_turma == 0 || at.cd_turma == cd_turma) && (cd_aluno == 0 || at.cd_aluno == cd_aluno) &&
                                        (at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                            at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                            at.cd_situacao_aluno_turma == (int)FundacaoFisk.SGF.GenericModel.AlunoTurma.SituacaoAlunoTurma.Encerrado ||
                                            at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)

                                  // data inicial:
                                  && (!pDtaI.HasValue || c.dt_matricula_contrato >= pDtaI)

                                  // data inicial e final:
                                  && (!pDtaI.HasValue || c.dt_matricula_contrato >= pDtaI)
                                  && (!pDtaF.HasValue || c.dt_matricula_contrato <= pDtaF)
                            select new 
                            {
                                cd_aluno = c.cd_aluno,
                                cd_turma = at.cd_turma,
                                dt_matricula = c.dt_matricula_contrato,
                                nm_contrato = c.nm_contrato,
                                no_aluno = at.Aluno.AlunoPessoaFisica.no_pessoa,
                                no_turma = at.Turma.no_turma,
                                no_material = im.Item.no_item,
                                dt_venda = im.Movimento.dt_mov_movimento
                            }).ToList().Select(x => new ReportTurmaMatriculaMaterial
                             {
                                 cd_aluno = x.cd_aluno,
                                 cd_turma = x.cd_turma,
                                 dt_matricula = x.dt_matricula,
                                 nm_contrato = x.nm_contrato,
                                 no_aluno = x.no_aluno,
                                 no_turma = x.no_turma,
                                 no_material = x.no_material,
                                 dt_venda = x.dt_venda
                            });

                 return sql;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

         public Turma buscarTurmaHorariosEditVirada(int cdTurma, int cdEscola, TipoConsultaTurmaEnum tipo)
         {
             try
             {

                 var sql = (from turma in db.Turma
                            where turma.cd_turma_enc == cdTurma &&
                                  turma.cd_pessoa_escola == cdEscola
                            select new
                            {
                                no_turma = turma.no_turma,
                                no_apelido = turma.no_apelido,
                                cd_turma = turma.cd_turma,
                                cd_pessoa_escola = turma.cd_pessoa_escola,
                                cd_turma_ppt = turma.cd_turma_ppt,
                                cd_curso = turma.cd_curso,
                                cd_sala = turma.cd_sala,
                                no_sala = turma.Sala.no_sala,
                                cd_duracao = turma.cd_duracao,
                                cd_regime = turma.cd_regime,
                                dt_inicio_aula = turma.dt_inicio_aula,
                                dt_final_aula = turma.dt_final_aula,
                                id_aula_externa = turma.id_aula_externa,
                                nro_aulas_programadas = turma.nro_aulas_programadas,
                                id_turma_ppt = turma.id_turma_ppt,
                                id_turma_ativa = turma.id_turma_ativa,
                                cd_produto = turma.cd_produto,
                                dta_termino_turma = turma.dt_termino_turma,
                                no_turma_ppt = turma.TurmaPai.no_turma,
                                cd_turma_enc = turma.cd_turma_enc
                            }).ToList().Select(x => new Turma
                            {
                                no_turma = x.no_turma,
                                no_apelido = x.no_apelido,
                                cd_turma = x.cd_turma,
                                cd_pessoa_escola = x.cd_pessoa_escola,
                                cd_turma_ppt = x.cd_turma_ppt,
                                cd_curso = x.cd_curso,
                                cd_sala = x.cd_sala,
                                no_sala = x.no_sala,
                                cd_duracao = x.cd_duracao,
                                cd_regime = x.cd_regime,
                                dt_inicio_aula = x.dt_inicio_aula,
                                dt_final_aula = x.dt_final_aula,
                                id_aula_externa = x.id_aula_externa,
                                nro_aulas_programadas = x.nro_aulas_programadas,
                                id_turma_ppt = x.id_turma_ppt,
                                id_turma_ativa = x.id_turma_ativa,
                                cd_produto = x.cd_produto,
                                dt_termino_turma = x.dta_termino_turma,
                                no_turma_ppt = x.no_turma_ppt,
                                cd_turma_enc = x.cd_turma_enc
                            }).FirstOrDefault();

                 if (sql != null && sql.cd_turma > 0)
                 {
                     int cdTurmaNova = sql.cd_turma;
                     //TODO Deivid
                     sql.horariosTurma = (from horario in db.Horario.Include(p => p.HorariosProfessores)
                                          where horario.cd_pessoa_escola == cdEscola && horario.id_origem == (int)Horario.Origem.TURMA &&
                                          horario.cd_registro == cdTurmaNova
                                          select new
                                          {
                                              horario.cd_horario,
                                              horario.cd_registro,
                                              horario.id_origem,
                                              horario.id_disponivel,
                                              horario.id_dia_semana,
                                              horario.dt_hora_ini,
                                              horario.dt_hora_fim,
                                              horario.HorariosProfessores
                                          }).ToList().Select(x => new Horario
                                          {
                                              cd_horario = x.cd_horario,
                                              cd_registro = x.cd_registro,
                                              id_origem = x.id_origem,
                                              id_disponivel = x.id_disponivel,
                                              id_dia_semana = x.id_dia_semana,
                                              dt_hora_ini = x.dt_hora_ini,
                                              dt_hora_fim = x.dt_hora_fim,
                                              HorariosProfessores = x.HorariosProfessores.ToList().Select(y => new HorarioProfessorTurma()
                                              {
                                                  cd_horario_professor_turma = y.cd_horario_professor_turma,
                                                  cd_horario = y.cd_horario,
                                                  cd_professor = y.cd_professor
                                              }).ToList()
                                          }).ToList();

                     sql.ProfessorTurma = (from professorT in db.ProfessorTurma
                                           where professorT.cd_turma == cdTurmaNova && professorT.Turma.cd_pessoa_escola == cdEscola
                                           select new
                                           {
                                               cd_professor_turma = professorT.cd_professor_turma,
                                               cd_turma = professorT.cd_turma,
                                               cd_professor = professorT.cd_professor,
                                               id_professor_ativo = professorT.id_professor_ativo,
                                               no_professor = professorT.Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                                               HorariosProfessores = professorT.Professor.HorariosProfessores.Where(hp => hp.Horario.cd_registro == cdTurmaNova)
                                           }).ToList().Select(x => new ProfessorTurma
                                           {
                                               cd_professor_turma = x.cd_professor_turma,
                                               cd_turma = x.cd_turma,
                                               cd_professor = x.cd_professor,
                                               no_professor = x.no_professor,
                                               id_professor_ativo = x.id_professor_ativo,
                                               HorariosProfessores = x.HorariosProfessores.ToList().Select(y => new HorarioProfessorTurma()
                                               {
                                                   cd_horario_professor_turma = y.cd_horario_professor_turma,
                                                   cd_horario = y.cd_horario,
                                                   cd_professor = y.cd_professor
                                               }).ToList()
                                           });

                     //Pesquisa das turmas PPTs filhas para edição:
                     if (sql.id_turma_ppt && tipo == TipoConsultaTurmaEnum.HAS_BUSCA_TURMA)
                     {
                         sql.alunosTurmasPPTSearch = (from turmaFilhaPPT in db.Turma
                                                      where turmaFilhaPPT.cd_turma_ppt == cdTurmaNova && turmaFilhaPPT.cd_pessoa_escola == cdEscola
                                                      select new AlunoTurmaPPT
                                                      {
                                                          cd_turma = turmaFilhaPPT.cd_turma,
                                                          cd_curso = turmaFilhaPPT.Curso.cd_curso,
                                                          cd_duracao = turmaFilhaPPT.cd_duracao,
                                                          cd_aluno = turmaFilhaPPT.TurmaAluno.Where(tPTT => tPTT.cd_turma == turmaFilhaPPT.cd_turma).FirstOrDefault().cd_aluno,
                                                          cd_regime = turmaFilhaPPT.cd_regime,
                                                          no_regime = turmaFilhaPPT.Regime.no_regime,
                                                          no_turma = turmaFilhaPPT.no_turma,
                                                          no_apelido = turmaFilhaPPT.no_apelido,
                                                          no_curso = turmaFilhaPPT.Curso.no_curso,
                                                          no_aluno = turmaFilhaPPT.TurmaAluno.Where(tPTT => tPTT.cd_turma == turmaFilhaPPT.cd_turma).FirstOrDefault().Aluno.AlunoPessoaFisica.no_pessoa,
                                                          alunoTurma = turmaFilhaPPT.TurmaAluno.Where(tPTT => tPTT.cd_turma == turmaFilhaPPT.cd_turma).FirstOrDefault(),
                                                          dt_termino_turma = turmaFilhaPPT.dt_termino_turma,
                                                          dt_final_aula = turmaFilhaPPT.dt_final_aula,
                                                          dt_inicio_aula = turmaFilhaPPT.dt_inicio_aula,
                                                          horariosTurma = from horario in db.Horario
                                                                          where horario.cd_pessoa_escola == cdEscola && horario.id_origem == (int)Horario.Origem.TURMA &&
                                                                          horario.cd_registro == turmaFilhaPPT.cd_turma
                                                                          select horario,
                                                          id_aluno_ativo = (from at in db.AlunoTurma
                                                                            join a in db.Aluno
                                                                            on at.cd_aluno equals a.cd_aluno
                                                                            where at.cd_turma == turmaFilhaPPT.cd_turma
                                                                            select a.id_aluno_ativo).FirstOrDefault(),
                                                          cd_pessoa_aluno = turmaFilhaPPT.TurmaAluno.Where(tPTT => tPTT.cd_turma == turmaFilhaPPT.cd_turma).FirstOrDefault().Aluno.AlunoPessoaFisica.cd_pessoa,

                                                          dt_cadastramento = turmaFilhaPPT.TurmaAluno.Where(tPTT => tPTT.cd_turma == turmaFilhaPPT.cd_turma).FirstOrDefault().Aluno.AlunoPessoaFisica.dt_cadastramento
                                                      }).ToList();

                         //TODO DEivid
                         if (sql.alunosTurmasPPTSearch != null && sql.alunosTurmasPPTSearch.Count() > 0)
                         {
                             List<AlunoTurmaPPT> alunosTurmaPPT = sql.alunosTurmasPPTSearch.ToList();
                             for (int i = 0; i < alunosTurmaPPT.Count(); i++)
                             {
                                 int cd_turma = alunosTurmaPPT[i].cd_turma;
                                 alunosTurmaPPT[i].ProfessorTurma = new List<ProfessorTurma>();
                                 alunosTurmaPPT[i].ProfessorTurma = (from professorT in db.ProfessorTurma
                                                                     where professorT.cd_turma == cd_turma && professorT.Turma.cd_pessoa_escola == cdEscola
                                                                     select new
                                                                     {
                                                                         cd_professor_turma = professorT.cd_professor_turma,
                                                                         cd_turma = professorT.cd_turma,
                                                                         cd_professor = professorT.cd_professor,
                                                                         id_professor_ativo = professorT.id_professor_ativo,
                                                                         no_professor = professorT.Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                                                                         HorariosProfessores = professorT.Professor.HorariosProfessores.Where(hp => hp.Horario.cd_registro == cd_turma)
                                                                     }).ToList().Select(x => new ProfessorTurma
                                                                     {
                                                                         cd_professor_turma = x.cd_professor_turma,
                                                                         cd_turma = x.cd_turma,
                                                                         cd_professor = x.cd_professor,
                                                                         no_professor = x.no_professor,
                                                                         id_professor_ativo = x.id_professor_ativo,
                                                                         HorariosProfessores = x.HorariosProfessores.ToList().Select(y => new HorarioProfessorTurma()
                                                                         {
                                                                             cd_horario_professor_turma = y.cd_horario_professor_turma,
                                                                             cd_horario = y.cd_horario,
                                                                             cd_professor = y.cd_professor
                                                                         }).ToList()
                                                                     });
                             }

                             sql.alunosTurmasPPTSearch = alunosTurmaPPT;
                         }
                     }
                     else
                         sql.alunosTurma = (from alunosT in db.AlunoTurma
                                            where alunosT.cd_turma == cdTurmaNova && alunosT.Turma.cd_pessoa_escola == cdEscola
                                            select new
                                            {
                                                cd_aluno_turma = alunosT.cd_aluno_turma,
                                                cd_turma = alunosT.cd_turma,
                                                cd_aluno = alunosT.cd_aluno,
                                                cd_situacao_aluno_turma = alunosT.cd_situacao_aluno_turma,
                                                id_aluno_ativo = db.Aluno.Where(a => a.cd_aluno == alunosT.cd_aluno && a.id_aluno_ativo == true).Any(),
                                                no_aluno = alunosT.Aluno.AlunoPessoaFisica.no_pessoa,
                                                cd_pessoa_aluno = alunosT.Aluno.cd_pessoa_aluno,
                                                cd_situacao_aluno_origem = alunosT.cd_situacao_aluno_origem,
                                                id_renegociacao = alunosT.id_renegociacao,
                                                dt_cadastramento = alunosT.Aluno.AlunoPessoaFisica.dt_cadastramento,
                                                cd_contrato = alunosT.cd_contrato,
                                                pessoaResposavel = alunosT.Aluno.AlunoPessoaFisica.PessoaSGFQueUsoOCpf,
                                            }).ToList().Select(x => new AlunoTurma
                                            {
                                                cd_aluno_turma = x.cd_aluno_turma,
                                                cd_turma = x.cd_turma,
                                                cd_aluno = x.cd_aluno,
                                                cd_situacao_aluno_turma = x.cd_situacao_aluno_turma,
                                                id_aluno_ativo = x.id_aluno_ativo,
                                                no_aluno = x.no_aluno,
                                                cd_pessoa_aluno = x.cd_pessoa_aluno,
                                                cd_situacao_aluno_origem = x.cd_situacao_aluno_origem,
                                                id_renegociacao = x.id_renegociacao,
                                                dt_cadastramento = x.dt_cadastramento,
                                                cd_contrato = x.cd_contrato,
                                               PessoaSGFQueUsoOCpf = x.pessoaResposavel == null ? null : new PessoaFisicaSGF
                                               {
                                                   cd_pessoa = x.pessoaResposavel.cd_pessoa,
                                                   no_pessoa = x.pessoaResposavel.no_pessoa
                                               }
                                            });
                 }
                 return sql;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

         public Turma getTurmaComCursoDuracao(int cd_turma) {
             try{
                 return (from t in db.Turma
                        where t.cd_turma == cd_turma
                        select new {
                            cd_duracao = t.cd_duracao,
                            cd_curso = t.cd_curso
                        }).ToList().Select(x => new Turma {
                            cd_duracao = x.cd_duracao,
                            cd_curso = x.cd_curso
                        }).FirstOrDefault();
             }
             catch(Exception exe) {
                 throw new DataAccessException(exe);
             }
         }
         public bool getTurmaNovaEnc(int cd_turma_enc)
         {
             try
             {
                 return (from t in db.Turma
                         where t.cd_turma_enc == cd_turma_enc
                         select t).Any();
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }
         public bool getVerificaContrato(int cd_turma)
         {
             try
             { 
                 IEnumerable<Contrato> sql;
                 sql = (from c in db.Contrato
                         //join c in db.Contrato on a.cd_contrato equals c.cd_contrato                        
                         where ((from a in db.AlunoTurma 
                                where a.cd_aluno == c.cd_aluno &&
                                      a.cd_contrato == c.cd_contrato_anterior &&
                                      a.cd_turma == cd_turma
                                      select a).Distinct()).Any()
                              select c);

                 return sql.Count() > 0;
                         //SELECT * FROM T_CONTRATO c 
                        // WHERE exists (SELECT 1 from T_ALUNO_TURMA a where a.cd_aluno = c.cd_aluno and
                        // a.cd_contrato = c.cd_contrato_anterior AND a.cd_turma = 1909)
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }
         public Turma getTurmaNovaEncSituacao(int cd_turma_enc)
         {
             try
             {
                 return (from t in db.Turma
                         where t.cd_turma_enc == cd_turma_enc && 
                         !t.TurmaAluno.Where(at => t.cd_turma == at.cd_turma && at.cd_situacao_aluno_turma != (int)AlunoTurma.SituacaoAlunoTurma.Aguardando).Any()
                         select t).FirstOrDefault();
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }
         public Turma getDadosTurmaMud(int cdEscola, int cd_turma)
         {
             try
             {
                 Turma sql = (from turma in db.Turma
                              where turma.cd_turma == cd_turma &&
                              turma.cd_pessoa_escola == cdEscola
                              select new
                              {
                                  turma.cd_turma,
                                  turma.cd_produto,
                                  turma.cd_curso,
                                  turma.cd_duracao

                              }).ToList().Select(x => new Turma
                              {
                                  cd_turma = x.cd_turma,
                                  cd_produto = x.cd_produto,
                                  cd_curso = x.cd_curso,
                                  cd_duracao = x.cd_duracao
                              }).FirstOrDefault();
                 return sql;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }
         public Turma buscarTurmaHorariosNovaVirada(int cdTurma, int cdEscola, TipoConsultaTurmaEnum tipo)
         {
             try
             {
                 var sql = (from turma in db.Turma
                            where turma.cd_turma == cdTurma &&
                                  turma.cd_pessoa_escola == cdEscola
                            select new
                            {
                                no_turma = turma.no_turma,
                                no_apelido = turma.no_apelido,
                                cd_turma = turma.cd_turma,
                                cd_pessoa_escola = turma.cd_pessoa_escola,
                                cd_turma_ppt = turma.cd_turma_ppt,
                                cd_curso = turma.cd_curso,
                                cd_sala = turma.cd_sala,
                                no_sala = turma.Sala.no_sala,
                                cd_duracao = turma.cd_duracao,
                                cd_regime = turma.cd_regime,
                                dt_inicio_aula = turma.dt_inicio_aula,
                                dt_final_aula = turma.dt_final_aula,
                                id_aula_externa = turma.id_aula_externa,
                                nro_aulas_programadas = turma.nro_aulas_programadas,
                                id_turma_ppt = turma.id_turma_ppt,
                                id_turma_ativa = turma.id_turma_ativa,
                                cd_produto = turma.cd_produto,
                                dta_termino_turma = turma.dt_termino_turma,
                                no_turma_ppt = turma.TurmaPai.no_turma,
                                cd_turma_enc = turma.cd_turma_enc,
                                cd_sala_online = turma.cd_sala_online,
                                no_sala_online = turma.T_SALA.no_sala
                            }).ToList().Select(x => new Turma
                            {
                                no_turma = x.no_turma,
                                no_apelido = x.no_apelido,
                                cd_turma = x.cd_turma,
                                cd_pessoa_escola = x.cd_pessoa_escola,
                                cd_turma_ppt = x.cd_turma_ppt,
                                cd_curso = x.cd_curso,
                                cd_sala = x.cd_sala,
                                no_sala = x.no_sala,
                                cd_duracao = x.cd_duracao,
                                cd_regime = x.cd_regime,
                                dt_inicio_aula = x.dt_inicio_aula,
                                dt_final_aula = x.dt_final_aula,
                                id_aula_externa = x.id_aula_externa,
                                nro_aulas_programadas = x.nro_aulas_programadas,
                                id_turma_ppt = x.id_turma_ppt,
                                id_turma_ativa = x.id_turma_ativa,
                                cd_produto = x.cd_produto,
                                dt_termino_turma = x.dta_termino_turma,
                                no_turma_ppt = x.no_turma_ppt,
                                cd_turma_enc = x.cd_turma_enc,
                                cd_sala_online = x.cd_sala_online,
                                no_sala_online = x.no_sala_online
                            }).FirstOrDefault();
                 if (sql != null && sql.cd_turma > 0)
                 {
                     //TODO Deivid
                     sql.horariosTurma = (from horario in db.Horario.Include(p => p.HorariosProfessores)
                                          where horario.cd_pessoa_escola == cdEscola && horario.id_origem == (int)Horario.Origem.TURMA &&
                                          horario.cd_registro == cdTurma
                                          select new
                                          {
                                              horario.cd_horario,
                                              horario.cd_registro,
                                              horario.id_origem,
                                              horario.id_disponivel,
                                              horario.id_dia_semana,
                                              horario.dt_hora_ini,
                                              horario.dt_hora_fim,
                                              horario.HorariosProfessores
                                          }).ToList().Select(x => new Horario
                                          {
                                              cd_horario = x.cd_horario,
                                              cd_registro = x.cd_registro,
                                              id_origem = x.id_origem,
                                              id_disponivel = x.id_disponivel,
                                              id_dia_semana = x.id_dia_semana,
                                              dt_hora_ini = x.dt_hora_ini,
                                              dt_hora_fim = x.dt_hora_fim,
                                              HorariosProfessores = x.HorariosProfessores.ToList().Select(y => new HorarioProfessorTurma()
                                              {
                                                  cd_horario_professor_turma = y.cd_horario_professor_turma,
                                                  cd_horario = y.cd_horario,
                                                  cd_professor = y.cd_professor
                                              }).ToList()
                                          }).ToList();

                     sql.ProfessorTurma = (from professorT in db.ProfessorTurma
                                           where professorT.cd_turma == cdTurma && professorT.Turma.cd_pessoa_escola == cdEscola
                                           select new
                                           {
                                               cd_professor_turma = professorT.cd_professor_turma,
                                               cd_turma = professorT.cd_turma,
                                               cd_professor = professorT.cd_professor,
                                               id_professor_ativo = professorT.id_professor_ativo,
                                               no_professor = professorT.Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                                               HorariosProfessores = professorT.Professor.HorariosProfessores.Where(hp => hp.Horario.cd_registro == cdTurma)
                                           }).ToList().Select(x => new ProfessorTurma
                                           {
                                               cd_professor_turma = x.cd_professor_turma,
                                               cd_turma = x.cd_turma,
                                               cd_professor = x.cd_professor,
                                               no_professor = x.no_professor,
                                               id_professor_ativo = x.id_professor_ativo,
                                               HorariosProfessores = x.HorariosProfessores.ToList().Select(y => new HorarioProfessorTurma()
                                               {
                                                   cd_horario_professor_turma = y.cd_horario_professor_turma,
                                                   cd_horario = y.cd_horario,
                                                   cd_professor = y.cd_professor
                                               }).ToList()
                                           });
                     sql.alunosTurma = (from alunosT in db.AlunoTurma
                                        where alunosT.cd_turma == cdTurma && alunosT.Turma.cd_pessoa_escola == cdEscola &&
                                         alunosT.Aluno.HistoricoAluno.Any(ha => ha.cd_turma == alunosT.cd_turma && DbFunctions.TruncateTime(ha.dt_historico) <= DbFunctions.TruncateTime(ha.Turma.dt_termino_turma) &&
                                                                    (ha.id_situacao_historico == (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.ATIVO ||
                                                                     ha.id_situacao_historico == (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.REMATRICULADO ||
                                                                     ha.id_situacao_historico == (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.MATRICULADOSMATERIAL) &&
                                                                    !db.HistoricoAluno.Any(ht => ht.cd_turma == alunosT.cd_turma && alunosT.cd_aluno == ht.cd_aluno &&
                                                                                              (ht.id_situacao_historico != (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.ATIVO &&
                                                                                               ht.id_situacao_historico != (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.REMATRICULADO &&
                                                                                               ht.id_situacao_historico != (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.ENCERRADO &&
                                                                                               ht.id_situacao_historico != (byte)FundacaoFisk.SGF.GenericModel.HistoricoAluno.SituacaoHistorico.MATRICULADOSMATERIAL
                                                                                               && ht.cd_historico_aluno > ha.cd_historico_aluno && ht.nm_sequencia > ha.nm_sequencia)
                                                                                              ))
                                        select new
                                        {
                                            cd_aluno_turma = alunosT.cd_aluno_turma,
                                            cd_turma = alunosT.cd_turma,
                                            cd_aluno = alunosT.cd_aluno,
                                            cd_situacao_aluno_turma = alunosT.cd_situacao_aluno_turma,
                                            id_aluno_ativo = db.Aluno.Where(a => a.cd_aluno == alunosT.cd_aluno && a.id_aluno_ativo == true).Any(),
                                            no_aluno = alunosT.Aluno.AlunoPessoaFisica.no_pessoa,
                                            cd_pessoa_aluno = alunosT.Aluno.cd_pessoa_aluno,
                                            cd_situacao_aluno_origem = alunosT.cd_situacao_aluno_origem,
                                            id_renegociacao = alunosT.id_renegociacao,
                                            dt_cadastramento = alunosT.Aluno.AlunoPessoaFisica.dt_cadastramento,
                                            cd_contrato = alunosT.cd_contrato,
                                            pessoaResposavel = alunosT.Aluno.AlunoPessoaFisica.PessoaSGFQueUsoOCpf
                                        }).ToList().Select(x => new AlunoTurma
                                        {
                                            cd_aluno_turma = x.cd_aluno_turma,
                                            cd_turma = x.cd_turma,
                                            cd_aluno = x.cd_aluno,
                                            cd_situacao_aluno_turma = x.cd_situacao_aluno_turma,
                                            id_aluno_ativo = x.id_aluno_ativo,
                                            no_aluno = x.no_aluno,
                                            cd_pessoa_aluno = x.cd_pessoa_aluno,
                                            cd_situacao_aluno_origem = x.cd_situacao_aluno_origem,
                                            id_renegociacao = x.id_renegociacao,
                                            dt_cadastramento = x.dt_cadastramento,
                                            cd_contrato = x.cd_contrato,
                                            PessoaSGFQueUsoOCpf = x.pessoaResposavel
                                        });
                    sql.TurmaEscola = (from tescola in db.TurmaEscola
                                       where tescola.cd_turma == cdTurma
                                       select new
                                       {
                                           cd_turma = tescola.cd_turma,
                                           cd_escola = tescola.cd_escola
                                       }).ToList().Select(x => new TurmaEscola
                                       {
                                           cd_turma = x.cd_turma,
                                           cd_escola = x.cd_escola
                                       }).ToList();

                 }
                 return sql;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

         public List<RptBolsistasAval> getAvaliacoesBolsista(int cd_aluno, int cd_pessoa_escola, int cd_turma)
         {
             try
             {
                 SGFWebContext dbComp = new SGFWebContext();
                 int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                 var sql = (from at in db.AlunoTurma 
                            where at.Aluno.cd_pessoa_escola == cd_pessoa_escola &&
                            at.cd_aluno == cd_aluno //&& at.Turma.AvaliacaoTurma.Any()
                            && (cd_turma == 0 || at.cd_turma == cd_turma)
                            select new RptBolsistasAval
                            {
                                cd_aluno = cd_aluno,
                                cd_turma = at.cd_turma,
                                no_turma = at.Turma.no_turma,
                                dt_ini_turma = at.Turma.dt_inicio_aula,
                                nr_faltas = at.nm_faltas,
                                nm_nf = db.Movimento.Where(m => m.id_origem_movimento == cd_origem && m.cd_origem_movimento == at.cd_contrato).Any() ? 
                                         db.Movimento.Where(m => m.id_origem_movimento == cd_origem && m.cd_origem_movimento == at.cd_contrato).FirstOrDefault().nm_movimento : null,
                                vl_parcela = at.Contrato != null ? at.Contrato.vl_parcela_contrato : 0,
                                dt_transferencia = at.Desistencia.Where(d => d.cd_aluno_turma == at.cd_aluno_turma && d.cd_motivo_desistencia == (int)MotivoDesistencia.motivoDesistencia.TRANSFERENCIA).OrderByDescending(d => d.dt_desistencia).FirstOrDefault().dt_desistencia,
                                tx_obs_transferencia = at.Desistencia.Where(d => d.cd_aluno_turma == at.cd_aluno_turma && d.cd_motivo_desistencia == (int)MotivoDesistencia.motivoDesistencia.TRANSFERENCIA).OrderByDescending(d => d.dt_desistencia).FirstOrDefault().tx_obs_desistencia,
                                no_produto = at.Turma.Produto.no_produto
                            }).ToList();
                 return sql;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }
         public IEnumerable<Turma> getTurmasPersonalizadas(int cdProduto, DateTime dtAula, TimeSpan hrIni, TimeSpan hrFim, int cdEscola, int? cd_turma)
         {
             try
             {
                 byte diaSemanaAula = (byte)(dtAula.DayOfWeek + 1);
                 var sql = (from turma in db.Turma
                            orderby turma.no_turma
                            where turma.cd_produto == cdProduto &&
                                  turma.id_turma_ativa &&
                                  turma.id_turma_ppt &&
                                  !turma.cd_turma_ppt.HasValue &&
                                  turma.cd_pessoa_escola == cdEscola &&
                                  turma.dt_inicio_aula <= dtAula.Date &&
                                  db.Horario.Where(th => th.cd_registro == turma.cd_turma && th.id_origem == (int)Horario.Origem.TURMA && th.id_dia_semana == diaSemanaAula &&
                                                         (th.dt_hora_ini >= hrIni && th.dt_hora_ini < hrFim) &&
                                                         (th.dt_hora_fim > hrIni && th.dt_hora_fim <= hrFim)).Any() &&
                                   (turma.Sala == null || (turma.Sala.nm_vaga_sala - turma.TurmaFilhasPPT.Where(tf => tf.TurmaAluno.Any(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando ||
                                                                                                    at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                                    at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                                    at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Count()) > 0)
                            select new
                        {
                            no_turma = turma.no_turma,
                            cd_turma = turma.cd_turma,
                            no_professor = turma.TurmaProfessorTurma.Where(t => t.id_professor_ativo).FirstOrDefault().Professor.FuncionarioPessoaFisica.no_pessoa,
                            no_sala = turma.Sala.no_sala,
                            nm_vagas_sala = turma.Sala != null ? turma.Sala.nm_vaga_sala : 0,
                            nm_qtd_alunos = turma.TurmaFilhasPPT.Where(tf => tf.TurmaAluno.Any(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando ||
                                                                                               at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                               at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                               at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Count(),
                            hr_ini = db.Horario.Where(th => th.cd_registro == turma.cd_turma && th.id_origem == (int)Horario.Origem.TURMA && th.id_dia_semana == diaSemanaAula).Min(x => x.dt_hora_ini),
                            hr_fim = db.Horario.Where(th => th.cd_registro == turma.cd_turma && th.id_origem == (int)Horario.Origem.TURMA && th.id_dia_semana == diaSemanaAula).Max(x => x.dt_hora_fim)
                        }).ToList().Select(x => new Turma
                            {
                                no_turma = x.no_turma,
                                cd_turma = x.cd_turma,
                                no_professor = x.no_professor,
                                no_sala = x.no_sala,
                                nm_vagas = x.nm_vagas_sala > 0 ? x.nm_vagas_sala - x.nm_qtd_alunos : 0,
                                horariosTurma = AddHrTurma(x.hr_ini,x.hr_fim)
                            });
                 
                 if (cd_turma != null && cd_turma > 0)
                     return sql.Where(x => x.cd_turma == cd_turma);

                 return sql;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

         private ICollection<Horario> AddHrTurma(TimeSpan hr_ini, TimeSpan hr_fim)
         {
             var listaHrTurma = new List<Horario>();
             listaHrTurma.Add(new Horario { dt_hora_ini = hr_ini, dt_hora_fim = hr_fim });
             return listaHrTurma;
         }

         public List<int> getTurmaPorAlunoProg(int cd_escola, int cd_aluno, DateTime dt_inicial)
         {
             try
             {
                 List<int> sql = (from t in db.Turma
                                  where t.cd_pessoa_escola == cd_escola &&
                                  t.ProgramacaoTurma.Any(p => p.dta_programacao_turma >= dt_inicial && p.id_aula_dada == false) &&
                                  t.cd_turma_ppt > 0 &&
                                  t.TurmaAluno.Any(ta => ta.cd_aluno == cd_aluno)
                                  select t.cd_turma).ToList();
                 return sql;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }
         public List<Turma> getTurmaAlunoMatRemat(int cdEscola, int cd_aluno)
         {
             try
             {
                 List<Turma> sql = (from turma in db.Turma
                                    where turma.TurmaAluno.Where(ta => ta.cd_aluno == cd_aluno && (ta.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                                   ta.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                                   ta.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Any() &&
                                    turma.cd_pessoa_escola == cdEscola
                                    select new
                                    {
                                        turma.cd_turma,
                                        turma.cd_produto,
                                        turma.dt_final_aula
                                    }).ToList().Select(x => new Turma
                                    {
                                        cd_turma = x.cd_turma,
                                        cd_produto = x.cd_produto,
                                        dt_final_aula = x.dt_final_aula
                                    }).ToList();
                 return sql;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

         public List<ReportPercentualTerminoEstagio> getRptPercentualTerminoEstagio(int cd_professor, DateTime? dt_ini, DateTime? dt_fim, int cd_escola)
         {
             try
             {
                 List<int> situacoes = new List<int>();
                 var sql = from t in db.Turma
                           join professor in db.ProfessorTurma on t.cd_turma equals professor.cd_turma
                           where t.cd_pessoa_escola == cd_escola && t.dt_termino_turma.HasValue
                           select t;
                 if (cd_professor > 0)
                     sql = from t in sql
                           join professor in db.ProfessorTurma on t.cd_turma equals professor.cd_turma
                           where professor.cd_professor == cd_professor
                           select t;
                 if (dt_ini.HasValue)
                     sql = from t in sql
                           where t.dt_termino_turma >= dt_ini
                           select t;
                 if (dt_fim.HasValue)
                     sql = from t in sql
                           where t.dt_termino_turma <= dt_fim
                           select t;
                 var sqlSearch = (from t in sql
                                  join professor in db.ProfessorTurma on t.cd_turma equals professor.cd_turma
                                  where (cd_professor == 0 || professor.cd_professor == cd_professor)
                                  select new
                                  {
                                      cd_turma = t.cd_turma,
                                      no_turma = t.no_turma,
                                      no_professor = professor.Professor.FuncionarioPessoaFisica.no_pessoa,
                                      cd_professor = professor.cd_professor,
                                      pc_termino_estagio = professor.Professor.pc_termino_estagio,
                                      vl_termino_estagio = professor.Professor.vl_termino_estagio,
                                      vl_rematricula = professor.Professor.vl_rematricula,
                                      //professores = t.TurmaProfessorTurma.Select(z => new { no_professor = z.Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa, cd_professor = z.cd_professor, 
                                      //                                                      pc_termino_estagio = z.Professor.pc_termino_estagio, vl_termino_estagio = z.Professor.vl_termino_estagio }),
                                      no_estagio = t.Curso.Estagio.no_estagio,
                                      Horairos = db.Horario.Where(x=> x.cd_registro == t.cd_turma && x.id_origem == (int)Horario.Origem.TURMA && x.cd_pessoa_escola == cd_escola),
                                      dt_inicio_estagio = t.dt_inicio_aula,
                                      dt_termino =  t.dt_termino_turma,
                                      qtd_alunos_1_mes = t.TurmaAluno.Where(at=> db.HistoricoAluno.Where(ha => ha.cd_turma == t.cd_turma && ha.cd_aluno == at.cd_aluno &&
                                                                                                              (DbFunctions.TruncateTime(ha.dt_historico) >= t.dt_inicio_aula &&
                                                                                                               DbFunctions.TruncateTime(ha.dt_historico) <= DbFunctions.AddDays(t.dt_inicio_aula,30) &&
                                                                                                               (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                                                ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                                                ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado ||
                                                                                                                ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial))
                                                                                                              ).Any()).Count(),
                                      qtd_alunos_ini_term_estagio = t.TurmaAluno.Where(at => db.HistoricoAluno.Where(ha => ha.cd_turma == t.cd_turma && ha.cd_aluno == at.cd_aluno &&
                                                                                                              (DbFunctions.TruncateTime(ha.dt_historico) >= t.dt_inicio_aula &&
                                                                                                               DbFunctions.TruncateTime(ha.dt_historico) <= DbFunctions.AddDays(t.dt_inicio_aula, 30) &&
                                                                                                               (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                                                ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                                                ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado ||
                                                                                                                ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)
                                                                                                               && ha.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_turma == t.cd_turma && han.cd_aluno == at.cd_aluno
                                                                                                               && DbFunctions.TruncateTime(han.dt_historico) >= t.dt_inicio_aula &&
                                                                                                                DbFunctions.TruncateTime(han.dt_historico) <= DbFunctions.AddDays(t.dt_inicio_aula, 30)).Max(x => x.nm_sequencia))
                                                                                                              ).Any()
                                                                                                               &&
                                                                                            at.Aluno.HistoricoAluno.Where(ha => ha.cd_turma == t.cd_turma &&
                                                                                                                            ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado
                                                                                                                            && ha.nm_sequencia == at.Aluno.HistoricoAluno.Where(han => han.cd_turma == t.cd_turma).Max(x => x.nm_sequencia)
                                                                                                                            ).Any()
                                                                                       ).Count(),
                                      qtd_alunos_terminaram_estagio = t.TurmaAluno.Where(at => db.HistoricoAluno.Where(ha => ha.cd_turma == t.cd_turma &&  ha.cd_aluno == at.cd_aluno &&
                                                                                                                             ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado &&
                                                                                                                             ha.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_turma == t.cd_turma && han.cd_aluno == at.cd_aluno).Max(x => x.nm_sequencia)
                                                                                                              ).Any()).Count(),
                                      qtd_alunos_matricularam_proximo_estagio = (from a in db.Aluno 
                                                                           where a.cd_pessoa_escola == cd_escola && a.AlunoTurma.Where(at => at.cd_turma == t.cd_turma).Any() &&
                                                                                 a.HistoricoAluno.Where(ha => ha.cd_turma == t.cd_turma && ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado &&
                                                                                 ha.nm_sequencia == a.HistoricoAluno.Where(han => han.cd_turma == t.cd_turma).Max(x => x.nm_sequencia) &&
                                                                                 a.HistoricoAluno.Where(hap => hap.cd_produto == ha.cd_produto && hap.nm_sequencia > ha.nm_sequencia &&
                                                                                 (hap.id_situacao_historico == (int)HistoricoAluno.SituacaoHistorico.ATIVO ||
                                                                                  hap.id_situacao_historico == (int)HistoricoAluno.SituacaoHistorico.REMATRICULADO ||
                                                                                  hap.id_situacao_historico == (int)HistoricoAluno.SituacaoHistorico.MATRICULADOSMATERIAL)).Any()).Any()
                                                                               select a.cd_aluno).Count()
                                  }).ToList().Select(x=> new ReportPercentualTerminoEstagio{
                                      cd_turma = x.cd_turma,
                                      no_turma = x.no_turma,
                                      no_professor = x.no_professor,
                                      cd_professor = x.cd_professor,
                                      pc_termino_estagio = x.pc_termino_estagio,
                                      vl_termino_estagio = x.vl_termino_estagio,
                                      vl_rematricula = x.vl_rematricula,
                                      //listaProfessores = (from lp in x.professores
                                      //                 select new
                                      //                 {
                                      //                     cd_professor = lp.cd_professor,
                                      //                     no_professor = lp.no_professor
                                      //                 }).Select(y => new ProfessorTurma
                                      //                 {
                                      //                     cd_professor = y.cd_professor,
                                      //                     no_professor = y.no_professor
                                      //                 }).ToList(),
                                      no_estagio = x.no_estagio,
                                      horarios = x.Horairos.ToList(),
                                      dt_inicio_estagio = x.dt_inicio_estagio,
                                      dt_termino_estagio = (DateTime)x.dt_termino,
                                      qtd_aluno_1_mes = x.qtd_alunos_1_mes,
                                      qtd_alunos_ini_term_estagio = x.qtd_alunos_ini_term_estagio,
                                      qtd_alunos_terminaram_estagio = x.qtd_alunos_terminaram_estagio,
                                      qtd_alunos_matricularam_proximo_estagio = x.qtd_alunos_matricularam_proximo_estagio
                                  });
                 return sqlSearch.ToList();
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

         public bool verificaSeTurmaEFilhaPersonalizada(int cd_turma, int cdEscola)
         {
             try
             {
                 bool retorno = (from t in db.Turma
                                 where t.cd_turma == cd_turma && t.cd_pessoa_escola == cdEscola && t.cd_turma_ppt > 0
                                 select t.cd_turma).Any();
                 return retorno;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

         public List<TipoAvaliacao> getTiposAvaliacaoComQtdAvaliacao(int cd_turma, List<int> cdsTiposAvaliacao)
         {
             try
             {
                 var retorno = (from t in db.TipoAvaliacao
                                where cdsTiposAvaliacao.Contains(t.cd_tipo_avaliacao)
                                select new
                                {
                                   t.cd_tipo_avaliacao,
                                   qtd_avaliacoes = t.Avaliacao.Count()
                                }).ToList().Select(x => new TipoAvaliacao
                                {
                                    cd_tipo_avaliacao = x.cd_tipo_avaliacao,
                                    nm_avaliacoes = x.qtd_avaliacoes
                                }).ToList();
                 return retorno;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

         public IEnumerable<TurmaSearch> getRptTurmasAEncerrar(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int tipoProg, bool turmasFilhas,
           DateTime? pDtaI, DateTime? pDtaF, DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int tipoOnline, string dias)
         {
             try
             {
                 SGFWebContext dbContext = new SGFWebContext();
                 int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                 var sql = from t in db.Turma
                     where (t.cd_pessoa_escola == cd_escola ||
                            (from te in db.TurmaEscola
                                where te.cd_turma == t.cd_turma &&
                                      te.cd_escola == cd_escola
                                select te).Any())
                           select t;

                if (dias.Contains("1"))
                        sql = from t in sql
                              where db.Horario.Any(h => //h.cd_pessoa_escola == cdEscola && 
                                  h.id_origem == (int)Horario.Origem.TURMA && h.cd_registro == t.cd_turma &&
                                  dias.Substring(h.id_dia_semana-1,1) == "1")
                              select t;

                 if (tipoOnline > 0)
                 {
                     if (tipoOnline == (int)Turma.TipoOnline.PRESENCIAL)
                     {
                         sql = from at in sql
                             where at.cd_sala_online == null
                             select at;
                     }
                     else if (tipoOnline == (int)Turma.TipoOnline.ONLINE)
                     {
                         sql = from at in sql
                             where at.cd_sala_online != null
                             select at;
                     }
                 }

                 if (cdCurso > 0)
                     sql = from t in sql
                           where t.cd_curso == cdCurso
                           select t;
                 if (cdDuracao > 0)
                     sql = from t in sql
                           where t.cd_duracao == cdDuracao
                           select t;
                 if (cdProduto > 0)
                     sql = from t in sql
                           where t.cd_produto == cdProduto
                           select t;
                 if (cdProfessor > 0)
                     sql = from t in sql
                           where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor).Any()
                           select t;

                 if (tipoTurma > 0)
                 {
                     if (tipoTurma == (int)Turma.TipoTurma.PPT)
                     {
                         switch (tipoProg)
                         {
                             case (int)ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_GERADAS:
                                 sql = from t in sql
                                       where t.TurmaFilhasPPT.Any(x => x.ProgramacaoTurma.Any())
                                       select t;
                                 break;
                             case (int)ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_NAO_GERADAS:
                                 sql = from t in sql
                                       where  t.TurmaFilhasPPT.Any(x => !x.ProgramacaoTurma.Any())
                                       select t;
                                 break;
                         }

                         sql = from t in sql
                               where t.id_turma_ppt == true &&
                                  t.cd_turma_ppt == null
                                      && t.TurmaFilhasPPT.Where(tfPPT => tfPPT.dt_termino_turma == null &&
                                      (tfPPT.dt_final_aula != null && tfPPT.dt_final_aula >= pDtaFimI && (pDtaFimF == null || tfPPT.dt_final_aula <= pDtaFimF))).Any()
                               select t;

                     }

                     if (tipoTurma == (int)Turma.TipoTurma.NORMAL)
                     {
                         switch (tipoProg)
                         {
                             case (int)ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_GERADAS:
                                 sql = from t in sql
                                       where t.ProgramacaoTurma.Any()
                                       select t;
                                 break;
                             case (int)ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_NAO_GERADAS:
                                 sql = from t in sql
                                       where !t.ProgramacaoTurma.Any()
                                       select t;
                                 break;
                         }
                         sql = from t in sql
                               where t.id_turma_ppt == false &&
                                  t.cd_turma_ppt == null &&
                                  t.dt_termino_turma == null && (t.dt_final_aula != null && t.dt_final_aula >= pDtaFimI && (pDtaFimF == null || t.dt_final_aula <= pDtaFimF))
                               select t;
                     }
                 }
                 else
                 {
                     sql = from t in sql
                           where
                           ((t.id_turma_ppt == true && t.cd_turma_ppt == null &&
                             t.TurmaFilhasPPT.Where(tfPPT => tfPPT.dt_termino_turma == null && tfPPT.dt_final_aula != null && tfPPT.dt_final_aula >= pDtaFimI && (pDtaFimF == null || tfPPT.dt_final_aula <= pDtaFimF)).Any()) 
                                 ||
                            (t.id_turma_ppt == false && t.cd_turma_ppt == null && t.dt_termino_turma == null && t.dt_final_aula != null && t.dt_final_aula >= pDtaFimI && (pDtaFimF == null || t.dt_final_aula <= pDtaFimF)))
                           select t;

                     switch (tipoProg)
                     {
                         case (int)ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_GERADAS:
                             sql = from t in sql
                                   where ((t.id_turma_ppt == true && t.cd_turma_ppt == null && t.TurmaFilhasPPT.Any(x => x.ProgramacaoTurma.Any())) ||
                                         (t.id_turma_ppt == false && t.cd_turma_ppt == null && t.ProgramacaoTurma.Any()))
                                   select t;
                             break;
                         case (int)ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_NAO_GERADAS:
                             sql = from t in sql
                                   where ((t.id_turma_ppt == true && t.cd_turma_ppt == null && t.TurmaFilhasPPT.Any(x =>! x.ProgramacaoTurma.Any())) ||
                                         (t.id_turma_ppt == false && t.cd_turma_ppt == null && !t.ProgramacaoTurma.Any()))
                                   select t;
                             break;
                     }
                 }

                 var sqlSearch = (from t in sql
                                  select new TurmaSearch
                                  {
                                      cd_turma = t.cd_turma,
                                      id_turma_ppt = t.id_turma_ppt,
                                      no_turma = t.no_turma,
                                      no_apelido = t.no_apelido,
                                      no_curso = t.Curso.no_curso,
                                      no_duracao = t.Duracao.dc_duracao,
                                      no_produto = t.Produto.no_produto,
                                      no_sala = (t.cd_sala_online == null)? t.Sala.no_sala:  t.T_SALA.no_sala,
                                      dt_inicio_aula = t.dt_inicio_aula,
                                      dt_final_aula = t.dt_final_aula,
                                      dta_termino_turma = t.dt_termino_turma,
                                      pessoasTurma = db.PessoaSGF.Where(p => db.FuncionarioSGF.Where(pf => pf.cd_pessoa_funcionario == p.cd_pessoa &&
                                                                        t.TurmaProfessorTurma.Where(pt => pt.cd_professor == pf.cd_funcionario && pt.id_professor_ativo == true).Any()).Any()),
                                      ultima_aula = t.DiarioAula.OrderByDescending(d => d.dt_aula).FirstOrDefault().tx_obs_aula,
                                      nro_alunos = t.id_turma_ppt == true ? (from at in db.AlunoTurma
                                                                             join tf in db.Turma on at.cd_turma equals tf.cd_turma
                                                                             join tPai in db.Turma on tf.cd_turma_ppt equals tPai.cd_turma
                                                                             join a in db.Aluno on at.cd_aluno equals a.cd_aluno
                                                                             join pa in db.PessoaSGF on a.cd_pessoa_aluno equals pa.cd_pessoa
                                                                             where
                                                                                 at.Aluno.cd_pessoa_escola == cd_escola &&
                                                                                 tPai.cd_turma == t.cd_turma &&
                                                                                 (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                 at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                 at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)
                                                                                 && tf.dt_termino_turma == null && (tf.dt_final_aula != null && tf.dt_final_aula >= pDtaFimI && (pDtaFimF == null || tf.dt_final_aula <= pDtaFimF))
                                                                             select at).Count() :
                                                                            t.TurmaAluno.Where(at => (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                           at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                           at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial) && at.Aluno.cd_pessoa_escola == cd_escola).Count(),
                                      qtd_devedores = (from at in db.AlunoTurma
                                                       where
                                                        at.Aluno.cd_pessoa_escola == cd_escola &&
                                                       (t.id_turma_ppt == true ? (at.Turma.cd_turma_ppt == t.cd_turma) : (at.cd_turma == t.cd_turma)) &&
                                                       at.Turma.dt_termino_turma == null && (at.Turma.dt_final_aula != null && at.Turma.dt_final_aula >= pDtaFimI && (pDtaFimF == null || at.Turma.dt_final_aula <= pDtaFimF)) &&
                                                       at.Turma.ProgramacaoTurma.Any() &&
                                                       db.Titulo.Where(ti => at.cd_contrato == ti.cd_origem_titulo &&
                                                            ti.id_origem_titulo == cd_origem &&
                                                            at.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&
                                                            ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.ABERTO &&
                                                            //ti.id_status_cnab == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL &&
                                                            (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                             at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                             at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)
                                                            //&& ti.dt_vcto_titulo >= pDtaFimI.Value && (pDtaFimF == null || ti.dt_vcto_titulo <= pDtaFimF.Value)
                                                            && !ti.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                            x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)).Any()
                                                       select at).Count(),
                                      tipo_online = (t.cd_sala_online == null) ? 1 : 2
                                  });
                 if (sqlSearch != null)
                     sqlSearch = sqlSearch.Where(a => a.nro_alunos > 0);
                 
                 return sqlSearch;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

         public IEnumerable<TurmaSearch> getRptTurmasNovas(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int tipoProg, bool turmasFilhas,
            DateTime? pDtaI, DateTime? pDtaF, int cd_escola, int tipoOnline, string dias)
         {
             try
             {
                 SGFWebContext dbContext = new SGFWebContext();
                 int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                 IQueryable<Turma> sql;

                 
                 sql = (from turma in db.Turma
                        where (turma.cd_pessoa_escola == cd_escola ||
                          (from te in db.TurmaEscola
                           where te.cd_turma == turma.cd_turma &&
                                    te.cd_escola == cd_escola
                              select te).Any())
                        select turma);
                 
                if (dias.Contains("1"))
                        sql = from t in sql
                              where db.Horario.Any(h => //h.cd_pessoa_escola == cdEscola && 
                                  h.id_origem == (int)Horario.Origem.TURMA && h.cd_registro == t.cd_turma &&
                                  dias.Substring(h.id_dia_semana-1,1) == "1")
                              select t;

                 if (tipoOnline > 0)
                 {
                     if (tipoOnline == (int)Turma.TipoOnline.PRESENCIAL)
                     {
                         sql = from at in sql
                               where at.cd_sala_online == null
                               select at;
                     }
                     else if (tipoOnline == (int)Turma.TipoOnline.ONLINE)
                     {
                         sql = from at in sql
                               where at.cd_sala_online != null
                               select at;
                     }
                 }

                 if (cdCurso > 0)
                     sql = from t in sql
                           where t.cd_curso == cdCurso
                           select t;
                 if (cdDuracao > 0)
                     sql = from t in sql
                           where t.cd_duracao == cdDuracao
                           select t;
                 if (cdProduto > 0)
                     sql = from t in sql
                           where t.cd_produto == cdProduto
                           select t;
                 if (cdProfessor > 0)
                     sql = from t in sql
                           where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor).Any()
                           select t;

                 if (tipoTurma > 0)
                 {
                     if (tipoTurma == (int)Turma.TipoTurma.PPT)
                     {
                         switch (tipoProg)
                         {
                             case (int)ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_GERADAS:
                                 sql = from t in sql
                                       where t.TurmaFilhasPPT.Any(x => x.ProgramacaoTurma.Any())
                                       select t;
                                 break;
                             case (int)ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_NAO_GERADAS:
                                 sql = from t in sql
                                       where t.TurmaFilhasPPT.Any(x => !x.ProgramacaoTurma.Any())
                                       select t;
                                 break;
                         }

                         sql = from t in sql
                               where t.id_turma_ppt == true &&
                                  t.cd_turma_ppt == null
                                  && t.TurmaFilhasPPT.Where(tfPPT => tfPPT.dt_termino_turma == null && t.cd_turma_enc == null &&
                                                           !tfPPT.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada) &&
                                                           (tfPPT.dt_inicio_aula != null && tfPPT.dt_inicio_aula >= pDtaI && (pDtaF == null || tfPPT.dt_inicio_aula <= pDtaF))).Any()
                                  select t;
                     }

                     if (tipoTurma == (int)Turma.TipoTurma.NORMAL)
                     {
                         switch (tipoProg)
                         {
                             case (int)ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_GERADAS:
                                 sql = from t in sql
                                       where t.ProgramacaoTurma.Any()
                                       select t;
                                 break;
                             case (int)ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_NAO_GERADAS:
                                 sql = from t in sql
                                       where !t.ProgramacaoTurma.Any()
                                       select t;
                                 break;
                         }
                         sql = from t in sql
                               where t.id_turma_ppt == false && t.cd_turma_enc == null &&
                                  t.cd_turma_ppt == null &&
                                  t.dt_termino_turma == null && (t.dt_inicio_aula >= pDtaI && (pDtaF == null || t.dt_inicio_aula <= pDtaF)) &&
                                    !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                               select t;
                     }
                 }
                 else
                 {
                     sql = from t in sql
                           where
                           ((t.id_turma_ppt == false && t.cd_turma_ppt != null &&
                             t.dt_termino_turma == null && t.cd_turma_enc == null && t.dt_inicio_aula >= pDtaI && (pDtaF == null || t.dt_inicio_aula <= pDtaF) && 
                                 !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada))
                                 ||
                            (t.id_turma_ppt == false && t.cd_turma_ppt == null && t.dt_termino_turma == null && t.cd_turma_enc == null && t.dt_inicio_aula >= pDtaI && (pDtaF == null || t.dt_inicio_aula <= pDtaF) && 
                                !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)))
                           select t;

                     switch (tipoProg)
                     {
                         case (int)ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_GERADAS:
                             sql = from t in sql
                                   where (t.id_turma_ppt == false && t.cd_turma_ppt == null && t.ProgramacaoTurma.Any())
                                   select t;
                             break;
                         case (int)ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_NAO_GERADAS:
                             sql = from t in sql
                                   where (t.id_turma_ppt == false && t.cd_turma_ppt == null && !t.ProgramacaoTurma.Any())
                                   select t;
                             break;
                     }
                 }

                 var sqlSearch = (from t in sql
                                  select new TurmaSearch
                                  {
                                      cd_turma = t.cd_turma,
                                      id_turma_ppt = t.id_turma_ppt,
                                      cd_turma_ppt = t.cd_turma_ppt,
                                      no_turma = t.no_turma,
                                      no_curso = t.Curso.no_curso,
                                      no_produto = t.Produto.no_produto,
                                      dt_inicio_aula = t.dt_inicio_aula,
                                      pessoasTurma = db.PessoaSGF.Where(p => db.FuncionarioSGF.Where(pf => pf.cd_pessoa_funcionario == p.cd_pessoa &&
                                                                        t.TurmaProfessorTurma.Where(pt => pt.cd_professor == pf.cd_funcionario && pt.id_professor_ativo == true).Any()).Any()),
                                      qtd_bolsistas = t.id_turma_ppt == true ? (from at in db.AlunoTurma
                                                                                join tf in db.Turma on at.cd_turma equals tf.cd_turma
                                                                                join tPai in db.Turma on tf.cd_turma_ppt equals tPai.cd_turma
                                                                                join a in db.Aluno on at.cd_aluno equals a.cd_aluno
                                                                                join pa in db.PessoaSGF on a.cd_pessoa_aluno equals pa.cd_pessoa
                                                                                where
                                                                                  at.Aluno.cd_pessoa_escola == cd_escola &&
                                                                                  tPai.cd_turma == t.cd_turma &&
                                                                                  tf.dt_termino_turma == null && tf.cd_turma_enc == null &&
                                                                                  !tf.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada) &&
                                                                                  (at.Contrato != null && at.Contrato.pc_desconto_bolsa > 0) &&
                                                                                  (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                   at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                   at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)
                                                                                  && (at.Turma.dt_inicio_aula >= pDtaI && (pDtaF == null || at.Turma.dt_inicio_aula <= pDtaF))
                                                                                select at).Count() :
                                                      t.TurmaAluno.Where(at => (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                               at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                               at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial) &&
                                                               (at.Contrato != null && at.Contrato.pc_desconto_bolsa > 0) &&
                                                               (at.Turma.dt_inicio_aula >= pDtaI && (pDtaF == null || at.Turma.dt_inicio_aula <= pDtaF))).Count(),
                                      //nro_alunos = t.TurmaAluno.Where(at => (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                      //                         at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                                      //                         (at.Turma.dt_inicio_aula >= pDtaI && (pDtaF == null || at.Turma.dt_inicio_aula <= pDtaF))).Count(),
                                      nro_alunos = t.id_turma_ppt == true ? (from at in db.AlunoTurma
                                                                             join tf in db.Turma on at.cd_turma equals tf.cd_turma
                                                                             join tPai in db.Turma on tf.cd_turma_ppt equals tPai.cd_turma
                                                                             join a in db.Aluno on at.cd_aluno equals a.cd_aluno
                                                                             join pa in db.PessoaSGF on a.cd_pessoa_aluno equals pa.cd_pessoa
                                                                             where
                                                                             at.Aluno.cd_pessoa_escola == cd_escola &&
                                                                             tPai.cd_turma == t.cd_turma &&
                                                                             tf.dt_termino_turma == null && tf.cd_turma_enc == null &&
                                                                             !tf.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada) &&
                                                                             (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                             at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                             at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)
                                                                             && (at.Turma.dt_inicio_aula >= pDtaI && (pDtaF == null || at.Turma.dt_inicio_aula <= pDtaF))
                                                                             select at).Count() :
                                                    t.TurmaAluno.Where(at => (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                    at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                    at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial) && at.Aluno.cd_pessoa_escola == cd_escola).Count(),
                                                    tipo_online = (t.cd_sala_online == null) ? 1 : 2,

                                      horarios = from horario in db.Horario
                                                 where (horario.cd_pessoa_escola == cd_escola) && horario.id_origem == (int)Horario.Origem.TURMA &&
                                                       (horario.cd_registro == t.cd_turma)
                                                 orderby horario.id_dia_semana, horario.dt_hora_ini, horario.dt_hora_fim
                                                 select horario
                                  });

                 return sqlSearch;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

         public IEnumerable<TurmaSearch> searchTurmaComPercentualFaltas(SearchParameters parametros, string descricao,
             string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
             int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas,
             int cdAluno, DateTime? dtInicial, DateTime? dtFinal, int? cd_turma_PPT, bool semContrato, int tipoConsulta, DateTime? dt_inicial,
             DateTime? dt_final, bool ProfTurmasAtuais, bool id_percentual_faltas,List<int> cdSituacoesAlunoTurma)
         {
             try
             {
                 IEntitySorter<TurmaSearch> sorter = EntitySorter<TurmaSearch>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                 IQueryable<Turma> sql;
                 sql = from t in db.Turma.AsNoTracking()
                       where t.cd_pessoa_escola == cdEscola
                       select t;

                 if (dt_inicial.HasValue && dt_final.HasValue)
                 {
                     //os turmas que possuem títulos abertos, que não possuem baixas e nem Cnab emitido, no período selecionado
                     SGFWebContext dbContext = new SGFWebContext();
                     int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                     sql = (from t in sql
                            join at in db.AlunoTurma on t.cd_turma equals at.cd_turma
                            join c in db.Contrato on at.cd_contrato equals c.cd_contrato
                            where db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                                                      ti.id_origem_titulo == cd_origem &&
                                                      c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&
                                                      ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.ABERTO &&
                                                      ti.id_status_cnab == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                                                      && ti.dt_vcto_titulo >= dt_inicial.Value
                                                      && ti.dt_vcto_titulo <= dt_final.Value
                                                      && !ti.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                  x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                                    ).Any()
                            //&&
                            //!db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                            //                  ti.id_origem_titulo == cd_origem &&
                            //                  c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&

                            //                  ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.FECHADO
                            //                  && ti.dt_vcto_titulo >= dt_inicial.Value
                            //                  && ti.dt_vcto_titulo <= dt_final.Value
                            //).Any()
                            //&&
                            //!db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                            //                  ti.id_origem_titulo == cd_origem &&
                            //                  c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&

                            //                  ti.id_status_cnab != (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                            //                  && ti.dt_vcto_titulo >= dt_inicial.Value
                            //                  && ti.dt_vcto_titulo <= dt_final.Value
                            //).Any()
                            select t).Distinct();
                 }
                 else if (dt_inicial.HasValue)
                 {
                     //os turmas que possuem títulos abertos, que não possuem baixas e nem Cnab emitido, no período selecionado
                     SGFWebContext dbContext = new SGFWebContext();
                     int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                     sql = (from t in sql
                            join at in db.AlunoTurma on t.cd_turma equals at.cd_turma
                            join c in db.Contrato on at.cd_contrato equals c.cd_contrato
                            where db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                                                     ti.id_origem_titulo == cd_origem &&
                                                     c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&
                                                     ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.ABERTO &&
                                                     ti.id_status_cnab == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                                                     && ti.dt_vcto_titulo >= dt_inicial.Value
                                                     && !ti.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                  x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                                    ).Any()
                            //&&
                            //!db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                            //                  ti.id_origem_titulo == cd_origem &&
                            //                  c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&

                            //                  ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.FECHADO
                            //                  && ti.dt_vcto_titulo >= dt_inicial.Value
                            //).Any()
                            //&&
                            //!db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                            //                  ti.id_origem_titulo == cd_origem &&
                            //                  c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&

                            //                  ti.id_status_cnab != (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                            //                  && ti.dt_vcto_titulo >= dt_inicial.Value
                            //).Any()
                            select t).Distinct();
                 }

                 if (!string.IsNullOrEmpty(descricao))
                     if (inicio)
                         sql = from t in sql
                               where t.no_turma.StartsWith(descricao)
                               select t;
                     else
                         sql = from t in sql
                               where t.no_turma.Contains(descricao)
                               select t;
                 if (!string.IsNullOrEmpty(apelido))
                     if (inicio)
                         sql = from t in sql
                               where t.no_apelido.StartsWith(apelido)
                               select t;
                     else
                         sql = from t in sql
                               where t.no_apelido.Contains(apelido)
                               select t;
                 if (cdAluno > 0)
                     sql = from t in sql
                           where t.TurmaAluno.Where(a => a.cd_aluno == cdAluno).Any()
                           select t;

                 if (dtInicial.HasValue)
                     sql = from t in sql
                           where t.dt_inicio_aula >= dtInicial
                           select t;

                 if (dtFinal.HasValue)
                     sql = from t in sql
                           where t.dt_inicio_aula <= dtFinal
                           select t;
                 if (semContrato)
                     sql = from t in sql
                           where db.AlunoTurma.Where(at => at.cd_turma == t.cd_turma && at.cd_contrato == null && at.cd_turma_origem > 0).Any()
                           select t;
                 //Busca as turmas filhas aprtir do código da turma pai PPT
                 if (cd_turma_PPT != null && cd_turma_PPT > 0)
                 {
                     // Para trazer o nome do aluno e não trazer o nome do curso.
                     turmasFilhas = true;
                     sql = from t in sql
                           where t.cd_turma_ppt == cd_turma_PPT
                           select t;
                 }
                 else
                 {
                     if (tipoTurma > 0)
                     {
                         if (tipoTurma == (int)Turma.TipoTurma.PPT)
                         {
                             if (!turmasFilhas && situacaoTurma > 0)
                             {
                                 var ativo = true;
                                 if (situacaoTurma == 2)
                                     ativo = false;
                                 sql = from t in sql
                                       where t.id_turma_ativa == ativo
                                       select t;
                             }
                             if (turmasFilhas)
                             {
                                 sql = from t in sql
                                       where t.cd_turma_ppt != null && t.id_turma_ppt == false
                                       select t;
                                 if (situacaoTurma > 0)
                                 {
                                     if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                                         sql = from t in sql
                                               where t.dt_termino_turma == null
                                               && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                               select t;
                                     if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                                         sql = from t in sql
                                               where t.dt_termino_turma != null
                                               select t;
                                     if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                                         sql = from t in sql
                                               where t.dt_termino_turma == null
                                               && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                               select t;
                                 }
                             }
                             else
                                 sql = from t in sql
                                       where t.id_turma_ppt == true && t.cd_turma_ppt == null
                                       select t;

                         }
                         if (tipoTurma == (int)Turma.TipoTurma.NORMAL)
                         {
                             sql = from t in sql
                                   where t.id_turma_ppt == false &&
                                      t.cd_turma_ppt == null
                                   select t;
                             if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                                 sql = from t in sql
                                       where t.dt_termino_turma == null
                                       && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                       select t;
                             if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                                 sql = from t in sql
                                       where t.dt_termino_turma != null
                                       select t;
                             if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                                 sql = from t in sql
                                       where t.dt_termino_turma == null
                                       && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                       //&& !t.AvaliacaoTurma.Any()
                                       select t;
                         }
                     }
                     else
                     {
                         sql = from t in sql
                               where !t.id_turma_ppt
                               select t;
                         if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASEMANDAMENTO)
                             sql = from t in sql
                                   where t.dt_termino_turma == null
                                   && t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                   select t;
                         if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASENCERRADAS)
                             sql = from t in sql
                                   where t.dt_termino_turma != null
                                   select t;
                         if (situacaoTurma == (int)Turma.SituacaoTurma.TURMASFORMACAO)
                             sql = from t in sql
                                   where t.dt_termino_turma == null
                                   && !t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada)
                                   //&& !t.AvaliacaoTurma.Any()
                                   select t;
                     }
                 }

                 if (cdCurso > 0)
                     sql = from t in sql
                           where t.cd_curso == cdCurso
                           select t;
                 if (cdDuracao > 0)
                     sql = from t in sql
                           where t.cd_duracao == cdDuracao
                           select t;
                 if (cdProduto > 0)
                     sql = from t in sql
                           where t.cd_produto == cdProduto
                           select t;

                 switch (tipoProg)
                 {
                     case ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_GERADAS:
                         sql = from t in sql
                               where t.ProgramacaoTurma.Any()
                               select t;
                         break;
                     case ProgramacaoTurma.TipoConsultaProgramacaoEnum.HAS_NAO_GERADAS:
                         sql = from t in sql
                               where !t.ProgramacaoTurma.Any()
                               select t;
                         break;
                 }

                 if (id_percentual_faltas)
                     sql = from t in sql
                         where db.AlunoTurma.Where(at => at.cd_turma == t.cd_turma && at.Turma.id_percentual_faltas == id_percentual_faltas).Any()
                         select t;

                 switch (tipoConsulta)
                 {
                     case (int)TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA:
                         if (cdProfessor > 0)
                             if (ProfTurmasAtuais)
                                 sql = from t in sql
                                       where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo && pt.Turma.dt_termino_turma == null).Any()
                                       select t;
                             else
                                 sql = from t in sql
                                       where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor).Any()
                                       select t;
                         break;
                     case (int)TipoConsultaTurmaEnum.HAS_PESQ_FK_TURMA_PROF:
                         if (cdProfessor > 0)
                             sql = from t in sql
                                   where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo).Any()
                                   select t;
                         break;
                     case (int)TipoConsultaTurmaEnum.HAS_PESQ_NOTA_MATERIAL:
                         SGFWebContext dbComp = new SGFWebContext();
                         int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                         sql = from t in sql
                               where t.TurmaAluno.Any(ta => ta.Aluno.Movimento.Any(m => m.id_origem_movimento == cd_origem))
                               select t;
                         if (cdProfessor > 0)
                             sql = from t in sql
                                   where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo).Any()
                                   select t;
                         break;
                     case (int)TipoConsultaTurmaEnum.HAS_PESQ_AVALICAO_TURMA:
                         sql = from t in sql
                               where !t.AvaliacaoTurma.Any()
                               select t;
                         if (cdProfessor > 0)
                             sql = from t in sql
                                   where t.TurmaProfessorTurma.Where(pt => pt.cd_professor == cdProfessor && pt.id_professor_ativo).Any()
                                   select t;
                         break;

                 }

                 if (cdSituacoesAlunoTurma != null && cdSituacoesAlunoTurma.Count() > 0)
                     sql = from t in sql
                           where t.TurmaAluno.Any(ta => cdSituacoesAlunoTurma.Contains((int)ta.cd_situacao_aluno_turma))
                           select t;

                 var sqlSearch = (from t in sql.AsNoTracking()
                                  select new TurmaSearch
                                  {
                                      cd_turma = t.cd_turma,
                                      cd_turma_ppt = t.cd_turma_ppt,
                                      no_turma = t.no_turma,
                                      no_apelido = t.no_apelido,
                                      no_curso = turmasFilhas ? "" : t.Curso.no_curso,
                                      no_duracao = t.Duracao.dc_duracao,
                                      no_produto = t.Produto.no_produto,
                                      cd_produto = t.cd_produto,
                                      dt_inicio_aula = t.dt_inicio_aula,
                                      id_turma_ppt = t.id_turma_ppt,
                                      id_turma_ativa = t.id_turma_ativa,
                                      dta_termino_turma = t.dt_termino_turma,
                                      nro_alunos = t.id_turma_ppt == true ? (from turma in db.Turma
                                                                             where turma.cd_pessoa_escola == cdEscola && turma.cd_turma_ppt == t.cd_turma &&
                                                                             turma.TurmaAluno.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                               at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                               at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial).Any()
                                                                             select turma.cd_turma).Count() :
                                      t.TurmaAluno.Where(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                               at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                               at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial).Count(),
                                      no_professor = t.TurmaProfessorTurma.Where(p => p.id_professor_ativo == true && p.cd_turma == t.cd_turma).FirstOrDefault().Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                                      no_aluno = turmasFilhas ? t.TurmaAluno.FirstOrDefault().Aluno.AlunoPessoaFisica.no_pessoa : "",
                                      turmasFilhas = turmasFilhas,
                                      cd_turma_enc = t.cd_turma_enc,
                                      existe_diario = t.DiarioAula.Any(x => x.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada),
                                      nm_alunos_turmaPPT = turmasFilhas ? (from turma in db.Turma
                                                                           where turma.cd_turma_ppt == t.cd_turma_ppt &&
                                                                           turma.TurmaAluno.Where(at => at.Aluno.cd_pessoa_escola == cdEscola && (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                                                                        at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                                                        at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)).Any()
                                                                           select turma.cd_turma).Count() : 0,
                                      vagas_disponiveis = turmasFilhas ? t.TurmaPai.T_SALA != null ? t.TurmaPai.T_SALA.nm_vaga_sala : t.TurmaPai.Sala != null ? t.TurmaPai.Sala.nm_vaga_sala :
                                                          t.T_SALA != null ? t.Curso != null ? t.T_SALA.nm_vaga_sala <= t.Curso.nm_vagas_curso ? t.T_SALA.nm_vaga_sala : (int)t.Curso.nm_vagas_curso : (int)t.T_SALA.nm_vaga_sala :
                                                          t.Sala != null ? t.Curso != null ? t.Sala.nm_vaga_sala <= t.Curso.nm_vagas_curso ? t.Sala.nm_vaga_sala : (int)t.Curso.nm_vagas_curso : (int)t.Sala.nm_vaga_sala :
                                                          t.Curso != null ? (int)t.Curso.nm_vagas_curso : 0 : 0
                                  });

                 sqlSearch = sorter.Sort(sqlSearch);
                 int limite = sqlSearch.Count();
                 parametros.ajustaParametrosPesquisa(limite);
                 sqlSearch = sqlSearch.Skip(parametros.from).Take(parametros.qtd_limite);
                 parametros.qtd_limite = limite;

                 var retorno = sqlSearch.ToList();
                 return retorno;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }


         public Turma findTurmaPercentualFaltaGrupoAvancado(int cdEscola, int cd_turma, int? cd_turma_ppt, bool id_turma_ppt)
         {
             try{
                 Turma retorno = null;

                 var sql = from turma in db.Turma.Include("curso")
                         .Include("curso.nivel")
                         .Include("curso.produto")
                     where turma.cd_pessoa_escola == cdEscola 
                     select turma;

               
                     if (id_turma_ppt == true && cd_turma_ppt == null)
                     {

                         sql = from t in sql.Include("curso")
                                 .Include("curso.nivel")
                                 .Include("curso.produto")
                               where t.id_turma_ppt == true && t.cd_turma == cd_turma &&
                                  t.cd_turma_ppt == null
                                  && t.TurmaFilhasPPT.Where(tfPPT => tfPPT.dt_termino_turma == null && t.cd_turma_enc == null).Any()


                               select t;
                     }

                     if (id_turma_ppt == false && cd_turma_ppt == null)
                     {

                         sql = from t in sql.Include("curso")
                                 .Include("curso.nivel")
                                 .Include("curso.produto")
                               where t.id_turma_ppt == false && t.cd_turma_enc == null &&
                                     t.cd_turma == cd_turma &&
                                    t.cd_turma_ppt == null

                               select t;
                     }

                     if (id_turma_ppt == false && cd_turma_ppt != null)
                     {
                         sql = from t in sql.Include("curso")
                                 .Include("curso.nivel")
                                 .Include("curso.produto")
                             where
                                 ((t.id_turma_ppt == false && t.cd_turma_ppt != null && t.cd_turma == cd_turma &&
                                   t.dt_termino_turma == null && t.cd_turma_enc == null ))
                             select t;
                     }
                 
                 

                 retorno = sql.FirstOrDefault();


                 
                 return retorno;
             }
             catch (Exception exe)
             {
                 throw new DataAccessException(exe);
             }
         }

         public string postGerarRematricula(string dc_turma, int cdUsuarioAtend, Nullable<System.DateTime> dt_inicial, Nullable<System.DateTime> dt_final, Nullable<bool> id_turma_nova, Nullable<int> cd_layout, Nullable<System.DateTime> dt_termino, int fusoHorario)
         {
             try
             {

                 db.Database.Connection.Open();
                 var command = db.Database.Connection.CreateCommand();
                 command.CommandType = System.Data.CommandType.StoredProcedure;
                 command.CommandText = @"sp_gerar_matricula";
                command.CommandTimeout = 180;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                 if (dc_turma != null)
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@dc_turma", dc_turma));
                 else
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@dc_turma", DBNull.Value));

                 if (dt_inicial != null)
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@dt_inicial", dt_inicial));
                 else
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@dt_inicial", DBNull.Value));

                 if (dt_final != null)
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@dt_final", dt_final));
                 else
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@dt_final", DBNull.Value));

                 if (id_turma_nova != null)
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@id_turma_nova", id_turma_nova));
                 else
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@id_turma_nova", DBNull.Value));

                 if (cd_layout != null)
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_layout", cd_layout));
                 else
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_layout", DBNull.Value));
                 if (dt_termino != null)
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@dt_termino", dt_termino));
                 else
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@dt_termino", DBNull.Value));
                 if (cdUsuarioAtend > 0)
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", cdUsuarioAtend));
                 else
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", DBNull.Value));
                 if (fusoHorario > 0)
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@fuso", fusoHorario));
                 else
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@fuso", DBNull.Value));

                 var parameterm = new System.Data.SqlClient.SqlParameter("@mensagem", System.Data.SqlDbType.VarChar,255);
                 parameterm.Direction = System.Data.ParameterDirection.Output;

                 sqlParameters.Add(parameterm);

                 var parameter = new System.Data.SqlClient.SqlParameter("@result", System.Data.SqlDbType.Int);
                 parameter.Direction = System.Data.ParameterDirection.ReturnValue;

                 sqlParameters.Add(parameter);

                 command.Parameters.AddRange(sqlParameters.ToArray());
                 command.ExecuteReader();

                 //var retunvalue = command.Parameters["@retorno"].Value;
                 // var retunvalue = Convert.ToBoolean((int)command.Parameters["@result"].Value);
                 string retunvalue = null;
                  retunvalue = (string)command.Parameters["@mensagem"].Value;

                db.Database.Connection.Close();
                return retunvalue;
             }
             catch (System.Data.SqlClient.SqlException exe)
             {
                 db.Database.Connection.Close();
                 throw new DataAccessException(exe);
             }
             catch (Exception exe)
             {
                 db.Database.Connection.Close();
                 throw new DataAccessException(exe);
             }
         }

        // NOVO CANCELAR ENCERRAMENTO
         public string postCancelarEncerramento(Nullable<int> cd_turma, Nullable<int> cd_usuario, Nullable<int> fuso)             
         {
             try
             {

                 db.Database.Connection.Open();
                 var command = db.Database.Connection.CreateCommand();
                 command.CommandType = System.Data.CommandType.StoredProcedure;
                 command.CommandText = @"sp_cancelar_rematricula";
                command.CommandTimeout = 180;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                 if (cd_turma != null)
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_turma", cd_turma));
                 else
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_turma", DBNull.Value));

                 if (cd_usuario != null)
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", cd_usuario));
                 else
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", DBNull.Value));

                 if (fuso != null)
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@fuso", fuso));
                 else
                     sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@fuso", DBNull.Value));

                 var parameter = new System.Data.SqlClient.SqlParameter("@result", System.Data.SqlDbType.Int);
                 parameter.Direction = System.Data.ParameterDirection.ReturnValue;

                 sqlParameters.Add(parameter);

                 command.Parameters.AddRange(sqlParameters.ToArray());
                 command.ExecuteReader();

                 var retunvalue = Convert.ToString((int)command.Parameters["@result"].Value);

                db.Database.Connection.Close();
                return retunvalue;
             }
             catch (System.Data.SqlClient.SqlException exe)
             {
                 db.Database.Connection.Close();
                 throw new DataAccessException(exe);
             }
             catch (Exception exe)
             {
                 db.Database.Connection.Close();
                 throw new DataAccessException(exe);
             }
         }
        public string postRefazerProgramacao(int cdTurma)
        {
            try
            {

                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = @"sp_numeracao_real_programacao";
                command.CommandTimeout = 180;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cdTurma", cdTurma));

                var parameter = new System.Data.SqlClient.SqlParameter("@result", System.Data.SqlDbType.Int);
                parameter.Direction = System.Data.ParameterDirection.ReturnValue;

                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();

                var retunvalue = Convert.ToString((int)command.Parameters["@result"].Value);

                db.Database.Connection.Close();
                return retunvalue;
            }
            catch (System.Data.SqlClient.SqlException exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
            catch (Exception exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
        }
        public string postRefazerNumeracao(int cdTurma)
        {
            try
            {

                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = @"sp_numeracao_programacao";
                command.CommandTimeout = 180;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_turma", cdTurma));

                var parameter = new System.Data.SqlClient.SqlParameter("@result", System.Data.SqlDbType.Int);
                parameter.Direction = System.Data.ParameterDirection.ReturnValue;

                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();

                var retunvalue = Convert.ToString((int)command.Parameters["@result"].Value);

                db.Database.Connection.Close();
                return retunvalue;
            }
            catch (System.Data.SqlClient.SqlException exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
            catch (Exception exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
        }

        public int postCancelarTurmasEncerramento(string dc_turma, Nullable<int> cd_usuario, Nullable<int> fuso)
        {
            try
            {
                return db.sp_cancelar_encerramento (dc_turma, cd_usuario, fuso);
            }
            catch (Exception e)
            {
                throw new DataAccessException(e);
            }
        }

        public List<int?> TurmaEncLista(string dc_turma, Nullable<System.DateTime> dt_termino, Nullable<int> cd_usuario, Nullable<int> fuso)
        {
            try
            {
                return db.sp_encerrar_turmas(dc_turma, dt_termino, cd_usuario, fuso).ToList();
            }
            catch (Exception e)
            {
                throw new DataAccessException(e);
            }
        }
        public List<Horario> verificaDiaSemanaTurmaFollowUp(int cdEscola, int cdTurma, int idDiaSemanaTurma)
         {
             try
             {
                 var contemDiaSemana = db.Horario.Where(h => (h.cd_pessoa_escola == cdEscola ||
                                                              (from te in db.TurmaEscola
                                                               where te.cd_turma == cdTurma &&
                                                                     te.cd_escola == cdEscola
                                                               select te).Any()) &&
                                  h.id_origem == (int)Horario.Origem.TURMA && h.cd_registro == cdTurma &&
                                  h.id_dia_semana == idDiaSemanaTurma).OrderBy(x => x.dt_hora_ini).ToList();
                 return contemDiaSemana;
             }
             catch (Exception)
             {
                 throw;
             }

         }
        
        public int getEscolaAluno(int CdAluno)
        {
            var escola =  (from a in db.Aluno where a.cd_aluno == CdAluno select a.cd_pessoa_escola).FirstOrDefault();
            return escola;
        }

        public bool HasTurmasEmAndamento(List<int> cdsTurmasPpt)
        {
            try
            {
                int qtdTurmas = 0;

                qtdTurmas = (from t in db.Turma
                    where cdsTurmasPpt.Contains(t.cd_turma) &&
                          t.cd_turma_ppt != null && t.id_turma_ppt == false &&
                          t.dt_termino_turma == null
                          && t.TurmaAluno.Any(at => at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                    at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                    at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando ||
                                                    at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.MatriculadosMaterial)
                          //&& t.DiarioAula.Any(x => x.id_status_aula == (int) DiarioAula.StatusDiarioAula.Efetivada)
                    select t).Count();

                return qtdTurmas > 0 ? true : false;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public void sp_verificar_grupo_cyber(string no_turma, Nullable<int> id_unidade)
        {
            try
            {
                var sql = db.sp_verificar_grupo_cyber(no_turma, id_unidade);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
