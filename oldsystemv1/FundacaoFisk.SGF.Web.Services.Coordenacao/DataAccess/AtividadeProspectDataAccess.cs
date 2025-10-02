using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.DataAccess
{
    public class AtividadeProspectDataAccess : GenericRepository<AtividadeProspect>, IAtividadeProspectDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<AtividadeProspectUI> searchAtividadeProspect(int cdAtividadeExtra, int cdEscola)
        {
            try
            {

                var sql = from atividadeProspect in db.AtividadeProspect
                          join prospect in db.Prospect on atividadeProspect.cd_prospect equals prospect.cd_prospect
                          where atividadeProspect.cd_atividade_extra == cdAtividadeExtra
                             && (prospect.cd_pessoa_escola == cdEscola) 
                          select new AtividadeProspectUI
                          {
                              cd_prospect = atividadeProspect.cd_prospect,
                              cd_atividade_prospect = atividadeProspect.cd_atividade_prospect,
                              cd_atividade_extra = atividadeProspect.cd_atividade_extra,
                              ind_participacao = atividadeProspect.ind_participacao,
                              no_pessoa = atividadeProspect.Prospect.PessoaFisica.no_pessoa,
                              txt_obs_atividade_prospect = atividadeProspect.txt_obs_atividade_propspect,
                              dc_reduzido_pessoa_escola = atividadeProspect.Prospect.Escola.dc_reduzido_pessoa
                          };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AtividadeProspectUI> searchAtividadeProspectByCdProspect(int cdProspect, int cdEscola)
        {
            try
            {

                var sql = from atividadeProspect in db.AtividadeProspect
                    join prospect in db.Prospect on atividadeProspect.cd_prospect equals prospect.cd_prospect
                    where prospect.cd_prospect == cdProspect
                          && (prospect.cd_pessoa_escola == cdEscola)
                    select new AtividadeProspectUI
                    {
                        cd_prospect = atividadeProspect.cd_prospect,
                        cd_atividade_prospect = atividadeProspect.cd_atividade_prospect,
                        cd_atividade_extra = atividadeProspect.cd_atividade_extra,
                        ind_participacao = atividadeProspect.ind_participacao,
                        no_pessoa = atividadeProspect.Prospect.PessoaFisica.no_pessoa,
                        txt_obs_atividade_prospect = atividadeProspect.txt_obs_atividade_propspect,
                        dc_reduzido_pessoa_escola = atividadeProspect.Prospect.Escola.dc_reduzido_pessoa,
                        dc_tipo_atividade_extra = atividadeProspect.AtividadeExtra.TipoAtividadeExtra.no_tipo_atividade_extra
                    };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AtividadeProspect> searchAtividadeProspectByCdAtividadeExtra(int cdAtividadeExtra, int cdEscola)
        {
            try
            {

                var sql = from atividadeProspect in db.AtividadeProspect
                    join prospect in db.Prospect on atividadeProspect.cd_prospect equals prospect.cd_prospect
                    where atividadeProspect.cd_atividade_extra == cdAtividadeExtra
                          && (prospect.cd_pessoa_escola == cdEscola)
                    select atividadeProspect;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AtividadeProspect> searchAtividadeProspectByCdAtividadeExtraForRecorrencia(int cdAtividadeExtra, int cdEscola)
        {
            try
            {

                var sql = from atividadeProspect in db.AtividadeProspect
                    join prospect in db.Prospect on atividadeProspect.cd_prospect equals prospect.cd_prospect
                    where atividadeProspect.cd_atividade_extra == cdAtividadeExtra
                    select atividadeProspect;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public AtividadeProspect searchAtividadeProspectByCdAtividadeExtraAndEmailProspect(int cdAtividadeExtra, string email)
        {
            try
            {

                var sql = (from atividadeProspect in db.AtividadeProspect
                    join prospect in db.Prospect on atividadeProspect.cd_prospect equals prospect.cd_prospect
                    where atividadeProspect.cd_atividade_extra == cdAtividadeExtra && 
                          prospect.PessoaFisica.TelefonePessoa.Any(x => x.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && x.dc_fone_mail == email)
                    select atividadeProspect).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<String> searchAtividadeEmailsProspectByCdAtividadeExtra(int cdAtividadeExtra, int cdEscola)
        {
            try
            {

                var sql = from atividadeProspect in db.AtividadeProspect
                          join prospect in db.Prospect on atividadeProspect.cd_prospect equals prospect.cd_prospect
                    where atividadeProspect.cd_atividade_extra == cdAtividadeExtra
                          && (prospect.cd_pessoa_escola == cdEscola)
                          && prospect.PessoaFisica.TelefonePessoa.Any(x => x.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL)
                          select (prospect.PessoaFisica.TelefonePessoa.Where(telefone => telefone.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault() != null) ? prospect.PessoaFisica.TelefonePessoa.Where(telefone => telefone.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail: "" ;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ContatoProspectAtividadeExtraUI> searchContatoProspectByCdAtividadeExtra(int cdAtividadeExtra, int cdEscola)
        {
            try
            {
                var sql = (from atividadeProspect in db.AtividadeProspect
                    join prospect in db.Prospect on atividadeProspect.cd_prospect equals prospect.cd_prospect
                    where atividadeProspect.cd_atividade_extra == cdAtividadeExtra
                          && (prospect.cd_pessoa_escola == cdEscola)
                          && prospect.PessoaFisica.TelefonePessoa.Any(x => x.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL)
                    select prospect).Select(f => new ContatoProspectAtividadeExtraUI()
                    {
                        email = db.TelefoneSGF.Where(t => t.cd_pessoa == f.cd_pessoa_fisica && t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).Select(x => x.dc_fone_mail).FirstOrDefault(),
                        no_pessoa = db.PessoaSGF.Where(z => z.cd_pessoa == f.cd_pessoa_fisica).Select(y=> y.no_pessoa).FirstOrDefault()
                            
                    });
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
            try
            {
                long sql = (from atividadeProspect in db.AtividadeProspect
                            join atividade in db.AtividadeExtra on atividadeProspect.cd_atividade_extra equals atividade.cd_atividade_extra
                            join prospect in db.Prospect on atividadeProspect.cd_prospect equals prospect.cd_prospect
                            where atividadeProspect.cd_atividade_extra == idAtividadeExtra &&
                                  prospect.cd_pessoa_escola == cdEscola
                            select atividadeProspect.cd_atividade_prospect).Count();
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
                long sql = (from atividadeProspect in db.AtividadeProspect
                    join atividade in db.AtividadeExtra on atividadeProspect.cd_atividade_extra equals atividade.cd_atividade_extra
                    where atividadeProspect.cd_atividade_extra == idAtividadeExtra
                    select atividadeProspect.cd_atividade_prospect).Count();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}