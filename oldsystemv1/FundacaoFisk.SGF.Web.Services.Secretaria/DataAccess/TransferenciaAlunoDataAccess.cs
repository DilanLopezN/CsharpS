using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class TransferenciaAlunoDataAccess : GenericRepository<TransferenciaAluno>, ITransferenciaAlunoDataAccess
    {
        public enum TipoStatusTransferenciaAluno
        {
            CADASTRADA = 0,
            SOLICITADA = 1,
            APROVADA = 2,
            EFETUADA = 3,
            RECUSADA = 9,
            
        }

        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<TransferenciaAlunoUI> getEnviarTransferenciaAlunoSearch(SearchParameters parametros, int cd_escola_logada, int? cd_unidade_destino, int cd_aluno, string nm_raf, string cpf, int status_transferencia, DateTime? dataIni, DateTime? dataFim)
        {
            try
            {
                IEntitySorter<TransferenciaAlunoUI> sorter = EntitySorter<TransferenciaAlunoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<TransferenciaAlunoUI> retorno;
                var sql = from t in db.TransferenciaAluno
                          where t.cd_escola_origem == cd_escola_logada
                          select t;

                if (status_transferencia >= 0)
                {
                    sql = (from t in sql
                           where t.id_status_transferencia == status_transferencia
                           select t);
                }

                if (dataIni.HasValue)
                {
                    switch (status_transferencia)
                    {
                        case (int)TipoStatusTransferenciaAluno.CADASTRADA:
                            sql = (from t in sql
                                   where t.dt_cadastro_transferencia.Date >= dataIni
                                   select t);
                            break;
                        case (int)TipoStatusTransferenciaAluno.SOLICITADA:
                            sql = (from t in sql
                                   where t.dt_solicitacao_transferencia.HasValue && DbFunctions.TruncateTime(t.dt_solicitacao_transferencia) >= dataIni
                                   select t);
                            break;
                        case (int)TipoStatusTransferenciaAluno.APROVADA:
                        case (int)TipoStatusTransferenciaAluno.RECUSADA:
                            sql = (from t in sql
                                   where t.dt_confirmacao_transferencia.HasValue && DbFunctions.TruncateTime(t.dt_confirmacao_transferencia) >= dataIni
                                   select t);
                            break;
                        case (int)TipoStatusTransferenciaAluno.EFETUADA:
                            sql = (from t in sql
                                   where t.dt_transferencia.HasValue && DbFunctions.TruncateTime(t.dt_transferencia) >= dataIni
                                   select t);
                            break;
                    }
                }

                if (dataFim.HasValue)
                {
                    switch (status_transferencia)
                    {
                        case (int)TipoStatusTransferenciaAluno.CADASTRADA:
                            sql = (from t in sql
                                   where t.dt_cadastro_transferencia.Date <= dataFim
                                   select t);
                            break;
                        case (int)TipoStatusTransferenciaAluno.SOLICITADA:
                            sql = (from t in sql
                                   where t.dt_solicitacao_transferencia.HasValue && DbFunctions.TruncateTime(t.dt_solicitacao_transferencia) <= dataFim
                                   select t);
                            break;
                        case (int)TipoStatusTransferenciaAluno.APROVADA:
                        case (int)TipoStatusTransferenciaAluno.RECUSADA:
                            sql = (from t in sql
                                   where t.dt_confirmacao_transferencia.HasValue && DbFunctions.TruncateTime(t.dt_confirmacao_transferencia) <= dataFim
                                   select t);
                            break;
                        case (int)TipoStatusTransferenciaAluno.EFETUADA:
                            sql = (from t in sql
                                   where t.dt_transferencia.HasValue && DbFunctions.TruncateTime(t.dt_transferencia) <= dataFim
                                   select t);
                            break;
                    }
                }

                if (cd_unidade_destino > 0)
                {
                    sql = (from t in sql
                           where t.cd_escola_destino == cd_unidade_destino
                           select t);
                }

                if (cd_aluno > 0)
                {

                    sql = (from t in sql
                           where ((t.cd_aluno_origem == cd_aluno && t.cd_escola_origem == cd_escola_logada))
                           select t);
                }

                if (!String.IsNullOrEmpty(nm_raf))
                {
                    sql = (from t in sql
                           from a in db.Aluno
                           from pr in db.PessoaRaf
                           where t.cd_aluno_origem == a.cd_aluno &&
                                 a.cd_pessoa_aluno == pr.cd_pessoa &&
                                 pr.nm_raf.Contains(nm_raf)
                           select t);
                }

                if (!String.IsNullOrEmpty(cpf))
                {
                    sql = (from t in sql
                           from a in db.Aluno
                           from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                           where
                               t.cd_aluno_origem == a.cd_aluno &&
                                 a.cd_pessoa_aluno == p.cd_pessoa &&
                                 p.nm_cpf.Contains(cpf)
                           select t);
                }



                retorno = (from c in sql
                           select new
                           {
                               cd_transferencia_aluno = c.cd_transferencia_aluno,
                               cd_escola_origem = c.cd_escola_origem,
                               cd_escola_destino = c.cd_escola_destino,
                               cd_aluno_origem = c.cd_aluno_origem,
                               cd_aluno_destino = c.cd_aluno_destino,
                               cd_motivo_transferencia = c.cd_motivo_transferencia,
                               dt_cadastro_transferencia = c.dt_cadastro_transferencia,
                               dt_solicitacao_transferencia = c.dt_solicitacao_transferencia,
                               dt_confirmacao_transferencia = c.dt_confirmacao_transferencia,
                               dt_transferencia = c.dt_transferencia,
                               dc_email_origem = c.dc_email_origem,
                               dc_email_destino = c.dc_email_destino,
                               id_status_transferencia = c.id_status_transferencia,
                               id_email_origem = c.id_email_origem,
                               id_email_destino = c.id_email_destino,
                               cpf = (from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                                      from a in db.Aluno
                                      where a.cd_aluno == c.cd_aluno_origem &&
                                            a.cd_pessoa_aluno == p.cd_pessoa
                                      select p.nm_cpf).FirstOrDefault(),
                               nm_raf = (from pr in db.PessoaRaf
                                         from a in db.Aluno
                                         where a.cd_aluno == c.cd_aluno_origem &&
                                               a.cd_pessoa_aluno == pr.cd_pessoa
                                         select pr.nm_raf).FirstOrDefault(),
                               no_motivo_transferencia_aluno = (from m in db.MotivoTransferenciaAluno where m.cd_motivo_transferencia_aluno == c.cd_motivo_transferencia select m.dc_motivo_transferencia_aluno).FirstOrDefault(),
                               no_aluno = (from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                                           from a in db.Aluno
                                           where a.cd_aluno == c.cd_aluno_origem &&
                                                 a.cd_pessoa_aluno == p.cd_pessoa
                                           select p.no_pessoa).FirstOrDefault(),
                               no_unidade_origem = (from p in db.PessoaSGF
                                                    where p.cd_pessoa == c.cd_escola_origem
                                                    select p.dc_reduzido_pessoa).FirstOrDefault(),
                               no_unidade_destino = (from p in db.PessoaSGF
                                                     where p.cd_pessoa == c.cd_escola_destino
                                                     select p.dc_reduzido_pessoa).FirstOrDefault(),

                           }).ToList().Select(x => new TransferenciaAlunoUI()
                           {
                               cd_transferencia_aluno = x.cd_transferencia_aluno,
                               cd_escola_origem = x.cd_escola_origem,
                               cd_escola_destino = x.cd_escola_destino,
                               cd_aluno_origem = x.cd_aluno_origem,
                               cd_aluno_destino = x.cd_aluno_destino,
                               cd_motivo_transferencia = x.cd_motivo_transferencia,
                               dt_cadastro_transferencia = x.dt_cadastro_transferencia,
                               dt_solicitacao_transferencia = x.dt_solicitacao_transferencia,
                               dt_confirmacao_transferencia = x.dt_confirmacao_transferencia,
                               dt_transferencia = x.dt_transferencia,
                               dc_email_origem = x.dc_email_origem,
                               dc_email_destino = x.dc_email_destino,
                               id_status_transferencia = x.id_status_transferencia,
                               id_email_origem = x.id_email_origem,
                               id_email_destino = x.id_email_destino,
                               cpf = x.cpf,
                               nm_raf = x.nm_raf,
                               no_motivo_transferencia_aluno = x.no_motivo_transferencia_aluno,
                               no_aluno = x.no_aluno,
                               no_unidade_origem = x.no_unidade_origem,
                               no_unidade_destino = x.no_unidade_destino
                           }).AsQueryable();

                retorno = sorter.Sort(retorno);

                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);


                return retorno;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public EnviarTransferenciaComponentesCadParams getComponentesEnviarTransferenciaCad(int cdEscola)
        {
            try
            {
                EnviarTransferenciaComponentesCadParams sql = (from telefone in db.TelefoneSGF

                                                               where telefone.cd_pessoa == cdEscola && telefone.id_telefone_principal == true && telefone.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL
                                                               select new EnviarTransferenciaComponentesCadParams()
                                                               {
                                                                   emailOrigem = telefone.dc_fone_mail,
                                                                   MotivosTransferencia = (from motivos in db.MotivoTransferenciaAluno select motivos).ToList()
                                                               }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public string getEmailUnidade(int cdEscola)
        {
            try
            {
                string sql = (from telefone in db.TelefoneSGF
                               where telefone.cd_pessoa == cdEscola && telefone.id_telefone_principal == true && telefone.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL
                               select telefone.dc_fone_mail).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public string getRafByAluno(int cdAluno)
        {
            try
            {
                string sql = (from r in db.PessoaRaf
                              from a in db.Aluno
                              where a.cd_aluno == cdAluno && a.cd_pessoa_aluno == r.cd_pessoa
                              select r.nm_raf).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public TransferenciaAluno getTransferenciaAlunoByCodForGrid(int cd_transferencia_aluno)
        {

            TransferenciaAluno retorno = (from c in db.TransferenciaAluno
                                          where c.cd_transferencia_aluno == cd_transferencia_aluno
                                          select new
                                          {
                                              cd_transferencia_aluno = c.cd_transferencia_aluno,
                                              cd_escola_origem = c.cd_escola_origem,
                                              cd_escola_destino = c.cd_escola_destino,
                                              cd_aluno_origem = c.cd_aluno_origem,
                                              cd_aluno_destino = c.cd_aluno_destino,
                                              cd_motivo_transferencia = c.cd_motivo_transferencia,
                                              dt_cadastro_transferencia = c.dt_cadastro_transferencia,
                                              dt_solicitacao_transferencia = c.dt_solicitacao_transferencia,
                                              dt_confirmacao_transferencia = c.dt_confirmacao_transferencia,
                                              dt_transferencia = c.dt_transferencia,
                                              dc_email_origem = c.dc_email_origem,
                                              dc_email_destino = c.dc_email_destino,
                                              id_status_transferencia = c.id_status_transferencia,
                                              id_email_origem = c.id_email_origem,
                                              id_email_destino = c.id_email_destino,
                                              cpf = (from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                                                     from a in db.Aluno
                                                     where a.cd_aluno == c.cd_aluno_origem &&
                                                           a.cd_pessoa_aluno == p.cd_pessoa
                                                     select p.nm_cpf).FirstOrDefault(),
                                              nm_raf = (from pr in db.PessoaRaf
                                                        from a in db.Aluno
                                                        where a.cd_aluno == c.cd_aluno_origem &&
                                                              a.cd_pessoa_aluno == pr.cd_pessoa
                                                        select pr.nm_raf).FirstOrDefault(),
                                              no_motivo_transferencia_aluno = (from m in db.MotivoTransferenciaAluno where m.cd_motivo_transferencia_aluno == c.cd_motivo_transferencia select m.dc_motivo_transferencia_aluno).FirstOrDefault(),
                                              no_aluno = (from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                                                          from a in db.Aluno
                                                          where a.cd_aluno == c.cd_aluno_origem &&
                                                                a.cd_pessoa_aluno == p.cd_pessoa
                                                          select p.no_pessoa).FirstOrDefault(),
                                              no_unidade_origem = (from p in db.PessoaSGF
                                                                    where p.cd_pessoa == c.cd_escola_origem
                                                                    select p.dc_reduzido_pessoa).FirstOrDefault(),
                                              no_unidade_destino = (from p in db.PessoaSGF
                                                                    where p.cd_pessoa == c.cd_escola_destino
                                                                    select p.dc_reduzido_pessoa).FirstOrDefault(),


                                          }).ToList().Select(x => new TransferenciaAluno()
                                           {
                                               cd_transferencia_aluno = x.cd_transferencia_aluno,
                                               cd_escola_origem = x.cd_escola_origem,
                                               cd_escola_destino = x.cd_escola_destino,
                                               cd_aluno_origem = x.cd_aluno_origem,
                                               cd_aluno_destino = x.cd_aluno_destino,
                                               cd_motivo_transferencia = x.cd_motivo_transferencia,
                                               dt_cadastro_transferencia = x.dt_cadastro_transferencia,
                                               dt_solicitacao_transferencia = x.dt_solicitacao_transferencia,
                                               dt_confirmacao_transferencia = x.dt_confirmacao_transferencia,
                                               dt_transferencia = x.dt_transferencia,
                                               dc_email_origem = x.dc_email_origem,
                                               dc_email_destino = x.dc_email_destino,
                                               id_status_transferencia = x.id_status_transferencia,
                                               id_email_origem = x.id_email_origem,
                                               id_email_destino = x.id_email_destino,
                                               cpf = x.cpf,
                                               nm_raf = x.nm_raf,
                                               no_motivo_transferencia_aluno = x.no_motivo_transferencia_aluno,
                                               no_aluno = x.no_aluno,
                                               no_unidade_origem = x.no_unidade_origem,
                                               no_unidade_destino = x.no_unidade_destino
                                          }).FirstOrDefault();

            


            return retorno;


        }

        public TransferenciaAluno getEnviarTransferenciaAlunoForEdit(int cd_transferencia_aluno)
        {
            try
            {
                TransferenciaAluno retorno = (from c in db.TransferenciaAluno
                                              where c.cd_transferencia_aluno == cd_transferencia_aluno
                                              select new
                                              {
                                                  cd_transferencia_aluno = c.cd_transferencia_aluno,
                                                  cd_escola_origem = c.cd_escola_origem,
                                                  cd_escola_destino = c.cd_escola_destino,
                                                  cd_aluno_origem = c.cd_aluno_origem,
                                                  cd_aluno_destino = c.cd_aluno_destino,
                                                  cd_motivo_transferencia = c.cd_motivo_transferencia,
                                                  dt_cadastro_transferencia = c.dt_cadastro_transferencia,
                                                  dt_solicitacao_transferencia = c.dt_solicitacao_transferencia,
                                                  dt_confirmacao_transferencia = c.dt_confirmacao_transferencia,
                                                  dt_transferencia = c.dt_transferencia,
                                                  dc_email_origem = c.dc_email_origem,
                                                  dc_email_destino = c.dc_email_destino,
                                                  id_status_transferencia = c.id_status_transferencia,
                                                  id_email_origem = c.id_email_origem,
                                                  id_email_destino = c.id_email_destino,
                                                  cpf = (from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                                                         from a in db.Aluno
                                                         where a.cd_aluno == c.cd_aluno_origem &&
                                                               a.cd_pessoa_aluno == p.cd_pessoa
                                                         select p.nm_cpf).FirstOrDefault(),
                                                  nm_raf = (from pr in db.PessoaRaf
                                                            from a in db.Aluno
                                                            where a.cd_aluno == c.cd_aluno_origem &&
                                                                  a.cd_pessoa_aluno == pr.cd_pessoa
                                                            select pr.nm_raf).FirstOrDefault(),
                                                  no_motivo_transferencia_aluno = (from m in db.MotivoTransferenciaAluno where m.cd_motivo_transferencia_aluno == c.cd_motivo_transferencia select m.dc_motivo_transferencia_aluno).FirstOrDefault(),
                                                  no_aluno = (from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                                                              from a in db.Aluno
                                                              where a.cd_aluno == c.cd_aluno_origem &&
                                                                    a.cd_pessoa_aluno == p.cd_pessoa
                                                              select p.no_pessoa).FirstOrDefault(),
                                                  no_unidade_origem = (from p in db.PessoaSGF
                                                                       where p.cd_pessoa == c.cd_escola_origem
                                                                       select p.dc_reduzido_pessoa).FirstOrDefault(),
                                                  no_unidade_destino = (from p in db.PessoaSGF
                                                                        where p.cd_pessoa == c.cd_escola_destino
                                                                        select p.dc_reduzido_pessoa).FirstOrDefault(),
                                                  no_arquivo_historico = c.no_arquivo_historico,
                                                  pdf_historico = c.pdf_historico

                                              }).ToList().Select(x => new TransferenciaAluno()
                                              {
                                                  cd_transferencia_aluno = x.cd_transferencia_aluno,
                                                  cd_escola_origem = x.cd_escola_origem,
                                                  cd_escola_destino = x.cd_escola_destino,
                                                  cd_aluno_origem = x.cd_aluno_origem,
                                                  cd_aluno_destino = x.cd_aluno_destino,
                                                  cd_motivo_transferencia = x.cd_motivo_transferencia,
                                                  dt_cadastro_transferencia = x.dt_cadastro_transferencia,
                                                  dt_solicitacao_transferencia = x.dt_solicitacao_transferencia,
                                                  dt_confirmacao_transferencia = x.dt_confirmacao_transferencia,
                                                  dt_transferencia = x.dt_transferencia,
                                                  dc_email_origem = x.dc_email_origem,
                                                  dc_email_destino = x.dc_email_destino,
                                                  id_status_transferencia = x.id_status_transferencia,
                                                  id_email_origem = x.id_email_origem,
                                                  id_email_destino = x.id_email_destino,
                                                  cpf = x.cpf,
                                                  nm_raf = x.nm_raf,
                                                  no_motivo_transferencia_aluno = x.no_motivo_transferencia_aluno,
                                                  no_aluno = x.no_aluno,
                                                  no_unidade_origem = x.no_unidade_origem,
                                                  no_unidade_destino = x.no_unidade_destino,
                                                  no_arquivo_historico = x.no_arquivo_historico,
                                                  pdf_historico = x.pdf_historico
                                              }).FirstOrDefault();




                return retorno;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<AlunoTurma> getAlunoTurmasBdTransferenciaAluno(int cd_aluno, int cd_escola)
        {
            try
            {
                List<AlunoTurma> sql = (from at in db.AlunoTurma
                                        from t in db.Turma
                                        from a in db.Aluno
                                        where t.cd_turma == at.cd_turma &&
                                              a.cd_aluno == at.cd_aluno &&
                                              a.cd_pessoa_escola == cd_escola &&
                                              (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                               at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado) &&
                                              a.id_aluno_ativo == true && a.cd_aluno == cd_aluno
                                        select at
                    ).ToList();
                return sql;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public AlunoTurma getAlunoTurmaBdEditTransferenciaAluno(int cd_escola, int cd_aluno_turma)
        {
            try
            {
                AlunoTurma sql = (from at in db.AlunoTurma
                                        from a in db.Aluno
                                        where at.cd_aluno_turma == cd_aluno_turma &&
                                              a.cd_aluno == at.cd_aluno &&
                                              a.cd_pessoa_escola == cd_escola 
                                        select at
                    ).FirstOrDefault();
                return sql;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public PessoaRaf getRafBdEditByAluno(int cdAluno)
        {
            try
            {
                PessoaRaf sql = (from r in db.PessoaRaf
                                 from a in db.Aluno
                                 where a.cd_aluno == cdAluno && a.cd_pessoa_aluno == r.cd_pessoa
                                 select r).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Aluno getAlunoBdEditByAluno(int cdAluno)
        {
            try
            {
                Aluno sql = (from a in db.Aluno
                                 where a.cd_aluno == cdAluno 
                                 select a).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TransferenciaAlunoUI> getReceberTransferenciaAlunoSearch(SearchParameters parametros, int cdEscola, int? cdUnidadeOrigem, string noAluno, string nmRaf, string cpf, int statusTransferencia, DateTime? dtInicial, DateTime? dtFinal)
        {
            try
            {
                IEntitySorter<TransferenciaAlunoUI> sorter = EntitySorter<TransferenciaAlunoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<TransferenciaAlunoUI> retorno;
                var sql = from t in db.TransferenciaAluno
                          where t.cd_escola_destino == cdEscola
                          select t;

                if (statusTransferencia >= 0)
                {
                    sql = (from t in sql
                           where t.id_status_transferencia == statusTransferencia
                           select t);
                }

                if (dtInicial.HasValue)
                {
                    switch (statusTransferencia)
                    {
                        case (int)TipoStatusTransferenciaAluno.CADASTRADA:
                            sql = (from t in sql
                                   where t.dt_cadastro_transferencia.Date >= dtInicial
                                   select t);
                            break;
                        case (int)TipoStatusTransferenciaAluno.SOLICITADA:
                            sql = (from t in sql
                                   where t.dt_solicitacao_transferencia.HasValue && DbFunctions.TruncateTime(t.dt_solicitacao_transferencia) >= dtInicial
                                   select t);
                            break;
                        case (int)TipoStatusTransferenciaAluno.APROVADA:
                        case (int)TipoStatusTransferenciaAluno.RECUSADA:
                            sql = (from t in sql
                                   where t.dt_confirmacao_transferencia.HasValue && DbFunctions.TruncateTime(t.dt_confirmacao_transferencia) >= dtInicial
                                   select t);
                            break;
                        case (int)TipoStatusTransferenciaAluno.EFETUADA:
                            sql = (from t in sql
                                   where t.dt_transferencia.HasValue && DbFunctions.TruncateTime(t.dt_transferencia) >= dtInicial
                                   select t);
                            break;
                    }
                }

                if (dtFinal.HasValue)
                {
                    switch (statusTransferencia)
                    {
                        case (int)TipoStatusTransferenciaAluno.CADASTRADA:
                            sql = (from t in sql
                                   where t.dt_cadastro_transferencia.Date <= dtFinal
                                   select t);
                            break;
                        case (int)TipoStatusTransferenciaAluno.SOLICITADA:
                            sql = (from t in sql
                                   where t.dt_solicitacao_transferencia.HasValue && DbFunctions.TruncateTime(t.dt_solicitacao_transferencia) <= dtFinal
                                   select t);
                            break;
                        case (int)TipoStatusTransferenciaAluno.APROVADA:
                        case (int)TipoStatusTransferenciaAluno.RECUSADA:
                            sql = (from t in sql
                                   where t.dt_confirmacao_transferencia.HasValue && DbFunctions.TruncateTime(t.dt_confirmacao_transferencia) <= dtFinal
                                   select t);
                            break;
                        case (int)TipoStatusTransferenciaAluno.EFETUADA:
                            sql = (from t in sql
                                   where t.dt_transferencia.HasValue && DbFunctions.TruncateTime(t.dt_transferencia) <= dtFinal
                                   select t);
                            break;
                    }
                }

                if (cdUnidadeOrigem > 0)
                {
                    sql = (from t in sql
                           where t.cd_escola_origem == cdUnidadeOrigem
                           select t);
                }

                if (!String.IsNullOrEmpty(noAluno))
                {

                    sql = (from t in sql
                           from a in db.Aluno
                           from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                           where t.cd_aluno_origem == a.cd_aluno &&
                                 a.cd_pessoa_aluno == p.cd_pessoa &&
                                 p.no_pessoa.Contains(noAluno)
                           select t);
                }

                if (!String.IsNullOrEmpty(nmRaf))
                {
                    sql = (from t in sql
                           from a in db.Aluno
                           from pr in db.PessoaRaf
                           where t.cd_aluno_origem == a.cd_aluno &&
                                 a.cd_pessoa_aluno == pr.cd_pessoa &&
                                 pr.nm_raf.Contains(nmRaf)
                           select t);
                }

                if (!String.IsNullOrEmpty(cpf))
                {
                    sql = (from t in sql
                           from a in db.Aluno
                           from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                           where
                               t.cd_aluno_origem == a.cd_aluno &&
                                 a.cd_pessoa_aluno == p.cd_pessoa &&
                                 p.nm_cpf.Contains(cpf)
                           select t);
                }



                retorno = (from c in sql
                           select new
                           {
                               cd_transferencia_aluno = c.cd_transferencia_aluno,
                               cd_escola_origem = c.cd_escola_origem,
                               cd_escola_destino = c.cd_escola_destino,
                               cd_aluno_origem = c.cd_aluno_origem,
                               cd_aluno_destino = c.cd_aluno_destino,
                               cd_motivo_transferencia = c.cd_motivo_transferencia,
                               dt_cadastro_transferencia = c.dt_cadastro_transferencia,
                               dt_solicitacao_transferencia = c.dt_solicitacao_transferencia,
                               dt_confirmacao_transferencia = c.dt_confirmacao_transferencia,
                               dt_transferencia = c.dt_transferencia,
                               dc_email_origem = c.dc_email_origem,
                               dc_email_destino = c.dc_email_destino,
                               id_status_transferencia = c.id_status_transferencia,
                               id_email_origem = c.id_email_origem,
                               id_email_destino = c.id_email_destino,
                               cpf = (from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                                      from a in db.Aluno
                                      where a.cd_aluno == c.cd_aluno_origem &&
                                            a.cd_pessoa_aluno == p.cd_pessoa
                                      select p.nm_cpf).FirstOrDefault(),
                               nm_raf = (from pr in db.PessoaRaf
                                         from a in db.Aluno
                                         where a.cd_aluno == c.cd_aluno_origem &&
                                               a.cd_pessoa_aluno == pr.cd_pessoa
                                         select pr.nm_raf).FirstOrDefault(),
                               no_motivo_transferencia_aluno = (from m in db.MotivoTransferenciaAluno where m.cd_motivo_transferencia_aluno == c.cd_motivo_transferencia select m.dc_motivo_transferencia_aluno).FirstOrDefault(),
                               no_aluno = (from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                                           from a in db.Aluno
                                           where a.cd_aluno == c.cd_aluno_origem &&
                                                 a.cd_pessoa_aluno == p.cd_pessoa
                                           select p.no_pessoa).FirstOrDefault(),
                               no_unidade_origem = (from p in db.PessoaSGF
                                                    where p.cd_pessoa == c.cd_escola_origem
                                                    select p.dc_reduzido_pessoa).FirstOrDefault(),
                               no_unidade_destino = (from p in db.PessoaSGF
                                                     where p.cd_pessoa == c.cd_escola_destino
                                                     select p.dc_reduzido_pessoa).FirstOrDefault(),

                           }).ToList().Select(x => new TransferenciaAlunoUI()
                           {
                               cd_transferencia_aluno = x.cd_transferencia_aluno,
                               cd_escola_origem = x.cd_escola_origem,
                               cd_escola_destino = x.cd_escola_destino,
                               cd_aluno_origem = x.cd_aluno_origem,
                               cd_aluno_destino = x.cd_aluno_destino,
                               cd_motivo_transferencia = x.cd_motivo_transferencia,
                               dt_cadastro_transferencia = x.dt_cadastro_transferencia,
                               dt_solicitacao_transferencia = x.dt_solicitacao_transferencia,
                               dt_confirmacao_transferencia = x.dt_confirmacao_transferencia,
                               dt_transferencia = x.dt_transferencia,
                               dc_email_origem = x.dc_email_origem,
                               dc_email_destino = x.dc_email_destino,
                               id_status_transferencia = x.id_status_transferencia,
                               id_email_origem = x.id_email_origem,
                               id_email_destino = x.id_email_destino,
                               cpf = x.cpf,
                               nm_raf = x.nm_raf,
                               no_motivo_transferencia_aluno = x.no_motivo_transferencia_aluno,
                               no_aluno = x.no_aluno,
                               no_unidade_origem = x.no_unidade_origem,
                               no_unidade_destino = x.no_unidade_destino
                           }).AsQueryable();

                retorno = sorter.Sort(retorno);

                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);


                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }


    
}