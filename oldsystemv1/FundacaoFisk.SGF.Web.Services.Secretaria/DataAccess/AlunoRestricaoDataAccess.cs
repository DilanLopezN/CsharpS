using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class AlunoRestricaoDataAccess : GenericRepository<AlunoRestricao>, IAlunoRestricaoDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<AlunoRestricao> getAlunoRestricaoByCdAluno(int cd_aluno)
        {
            try
            {

                IEnumerable<AlunoRestricao> retorno = (from ar in db.AlunoRestricao
                    where ar.cd_aluno == cd_aluno
                    select new
                    {
                        id_grid_aluno_restricao = 0,
                        cd_aluno_restricao = ar.cd_aluno_resticao,
                        cd_aluno = ar.cd_aluno,
                        dc_orgao_financeiro = ar.OrgaoFinanceiro.dc_orgao_financeiro,
                        no_responsavel = ar.UsuarioWebSgf.no_login,
                        cd_orgao_financeiro = ar.OrgaoFinanceiro.cd_orgao_financeiro,
                        cd_usuario = ar.UsuarioWebSgf.cd_usuario,
                        dt_inicio_restricao = ar.dt_inicio_restricao,
                        dt_final_restricao = ar.dt_final_restricao,
                        dt_cadastro = ar.dt_cadastro

                    }).ToList().Select(x => new AlunoRestricao()
                    {
                        id_grid_aluno_restricao = x.id_grid_aluno_restricao,
                        cd_aluno_resticao = x.cd_aluno_restricao,
                        cd_aluno = x.cd_aluno,
                        dc_orgao_financeiro = x.dc_orgao_financeiro,
                        no_responsavel = x.no_responsavel,
                        cd_orgao_financeiro = x.cd_orgao_financeiro,
                        cd_usuario = x.cd_usuario,
                        dt_inicio_restricao = x.dt_inicio_restricao,
                        dt_final_restricao = x.dt_final_restricao,
                        dt_cadastro = x.dt_cadastro
                    }).AsEnumerable();

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
            
        }

        public IEnumerable<AlunoRestricao> getAlunoRestricaoEditByCdAluno(int cd_aluno)
        {
            try
            {
                IEnumerable<AlunoRestricao> retorno = (from ar in db.AlunoRestricao
                    where ar.cd_aluno == cd_aluno
                    select ar);
              
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }
    }
}