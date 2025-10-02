using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class AtividadeAlunoDataAccess : GenericRepository<AtividadeAluno>, IAtividadeAlunoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<AtividadeAlunoUI> searchAtividadeAluno(int cdAtividadeExtra, int cdEscola)
        {
            try
            {


                var sql = from atividadeAluno in db.AtividadeAluno
                          join aluno in db.Aluno on atividadeAluno.cd_aluno equals aluno.cd_aluno
                          join pessoa in db.PessoaSGF on aluno.cd_pessoa_aluno equals pessoa.cd_pessoa
                          where atividadeAluno.cd_atividade_extra == cdAtividadeExtra
                          && ((aluno.cd_pessoa_escola == cdEscola))
                          select new AtividadeAlunoUI
                          {
                              cd_aluno = atividadeAluno.cd_aluno,
                              cd_atividade_aluno = atividadeAluno.cd_atividade_aluno,
                              cd_atividade_extra = atividadeAluno.cd_atividade_extra,
                              ind_participacao = atividadeAluno.ind_participacao,
                              no_pessoa = pessoa.no_pessoa,
                              tx_obs_atividade_aluno = atividadeAluno.tx_obs_atividade_aluno,
                              dc_reduzido_pessoa_escola = atividadeAluno.Aluno.EscolaAluno.dc_reduzido_pessoa
                          };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AtividadeAlunoUI> searchAtividadeAlunoReport(int cdAtividadeExtra, int cdEscola)
        {
            try
            {


                var sql = (from atividadeAluno in db.AtividadeAluno
                    join aluno in db.Aluno on atividadeAluno.cd_aluno equals aluno.cd_aluno
                    join pessoa in db.PessoaSGF on aluno.cd_pessoa_aluno equals pessoa.cd_pessoa
                    where atividadeAluno.cd_atividade_extra == cdAtividadeExtra
                          && ((aluno.cd_pessoa_escola == cdEscola))
                    select new AtividadeAlunoUI
                    {
                        cd_aluno = atividadeAluno.cd_aluno,
                        cd_atividade_aluno = atividadeAluno.cd_atividade_aluno,
                        cd_atividade_extra = atividadeAluno.cd_atividade_extra,
                        ind_participacao = atividadeAluno.ind_participacao,
                        no_pessoa = pessoa.no_pessoa,
                        tx_obs_atividade_aluno = atividadeAluno.tx_obs_atividade_aluno,
                        dc_reduzido_pessoa_escola = atividadeAluno.Aluno.EscolaAluno.dc_reduzido_pessoa,
                        isProspect = false
                    }).ToList();

                var sql2 = (from atividadeProspect in db.AtividadeProspect
                    join prospect in db.Prospect on atividadeProspect.cd_prospect equals prospect.cd_prospect
                    where atividadeProspect.cd_atividade_extra == cdAtividadeExtra
                          && (prospect.cd_pessoa_escola == cdEscola)
                           select new AtividadeAlunoUI
                    {
                        cd_aluno = atividadeProspect.cd_prospect,
                        cd_atividade_aluno = atividadeProspect.cd_atividade_prospect,
                        cd_atividade_extra = atividadeProspect.cd_atividade_extra,
                        ind_participacao = atividadeProspect.ind_participacao,
                        no_pessoa = atividadeProspect.Prospect.PessoaFisica.no_pessoa,
                        tx_obs_atividade_aluno = atividadeProspect.txt_obs_atividade_propspect,
                        dc_reduzido_pessoa_escola = atividadeProspect.Prospect.Escola.dc_reduzido_pessoa,
                        isProspect = true
                    }).ToList();

                sql.AddRange(sql2);
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int getNroPessoasAtividade(int cdAtividadeExtra)
        {
            int cAlunos = (from atividadeAluno in db.AtividadeAluno
                           where atividadeAluno.cd_atividade_extra == cdAtividadeExtra
                           select atividadeAluno).Count();
            int cProspect = (from atividadeAluno in db.AtividadeProspect
                             where atividadeAluno.cd_atividade_extra == cdAtividadeExtra
                             select atividadeAluno).Count();

            return(cAlunos + cProspect);	
        }

        public IEnumerable<AtividadeAluno> searchAtividadeAlunoByCdAtividadeExtra(int cdAtividadeExtra, int cdEscola)
        {
            try
            {
                

                var sql = from atividadeAluno in db.AtividadeAluno
                    join aluno in db.Aluno on atividadeAluno.cd_aluno equals aluno.cd_aluno
                    join pessoa in db.PessoaSGF on aluno.cd_pessoa_aluno equals pessoa.cd_pessoa
                    where atividadeAluno.cd_atividade_extra == cdAtividadeExtra
                          && ((aluno.cd_pessoa_escola == cdEscola))
                    select atividadeAluno;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AtividadeAluno> searchAtividadeAlunoByCdAtividadeExtraForRecorrencia(int cdAtividadeExtra, int cdEscola)
        {
            try
            {


                var sql = from atividadeAluno in db.AtividadeAluno
                    join aluno in db.Aluno on atividadeAluno.cd_aluno equals aluno.cd_aluno
                    join pessoa in db.PessoaSGF on aluno.cd_pessoa_aluno equals pessoa.cd_pessoa
                    where atividadeAluno.cd_atividade_extra == cdAtividadeExtra
                    select atividadeAluno;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<String> searchEmailsAtividadeAlunoByCdAtividadeExtra(int cdAtividadeExtra, int cdEscola)
        {
            try
            {
                

                var sql = from atividadeAluno in db.AtividadeAluno
                    join aluno in db.Aluno on atividadeAluno.cd_aluno equals aluno.cd_aluno
                    join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on aluno.cd_pessoa_aluno equals pessoa.cd_pessoa
                    join contato in db.TelefoneSGF on aluno.cd_pessoa_aluno equals contato.cd_pessoa
                    where atividadeAluno.cd_atividade_extra == cdAtividadeExtra
                          && ((aluno.cd_pessoa_escola == cdEscola))
                          && contato.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL
                          select contato.dc_fone_mail;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Retorna um inteiro com a quantidade de alunos com atividade
        public long retornNumbersOfStudents(int idAtividadeExtra, int cdEscola)
        {
            try{
                long sql = (from atividadeAluno in db.AtividadeAluno
                            join atividade in db.AtividadeExtra on atividadeAluno.cd_atividade_extra equals atividade.cd_atividade_extra
                            join aluno in db.Aluno on atividadeAluno.cd_aluno equals aluno.cd_aluno
                            where atividadeAluno.cd_atividade_extra == idAtividadeExtra &&
                                  aluno.cd_pessoa_escola == cdEscola
                            select atividadeAluno.cd_atividade_aluno).Count();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public long retornAllNumbersOfStudents(int idAtividadeExtra)
        {
            try
            {
                long sql = (from atividadeAluno in db.AtividadeAluno
                    join atividade in db.AtividadeExtra on atividadeAluno.cd_atividade_extra equals atividade.cd_atividade_extra
                    where atividadeAluno.cd_atividade_extra == idAtividadeExtra
                    select atividadeAluno.cd_atividade_aluno).Count();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


    }
}
