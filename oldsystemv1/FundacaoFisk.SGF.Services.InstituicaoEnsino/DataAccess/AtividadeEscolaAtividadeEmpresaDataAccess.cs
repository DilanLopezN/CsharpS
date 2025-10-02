using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;

namespace FundacaoFisk.SGF.Services.InstituicaoEnsino.DataAccess
{
    public class AtividadeEscolaAtividadeEmpresaDataAccess : GenericRepository<AtividadeEscolaAtividade>, IAtividadeEscolaAtividadeEmpresaDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<AtividadeEscolaAtividadeSearchUI> getAtividadeEscolatWithAtividade(int cd_atividade_extra)
        {
            try
            {
                var sql = from atividadeEscola in db.AtividadeEscolaAtividade
                          where atividadeEscola.cd_atividade_extra == cd_atividade_extra
                          orderby atividadeEscola.Escola.no_pessoa
                    select new AtividadeEscolaAtividadeSearchUI()
                    {
                        cd_atividade_escola = atividadeEscola.cd_atividade_escola,
                        cd_atividade_extra = atividadeEscola.cd_atividade_extra,
                        cd_escola = atividadeEscola.cd_escola,
                        dc_reduzido_pessoa = atividadeEscola.Escola.dc_reduzido_pessoa
                    };

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}