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
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class AtividadeRecorrenciaDataAccess : GenericRepository<AtividadeRecorrencia>, IAtividadeRecorrenciaDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public AtividadeRecorrencia searchAtividadeRecorrenciaByCdAtividadeExtra(int cdAtividadeExtra, int cdEscola)
        {
            try
            {

                var sql = (from atividadeRecorrencia in db.AtividadeRecorrencia
                    join atividadeExtra in db.AtividadeExtra on atividadeRecorrencia.cd_atividade_extra equals atividadeExtra.cd_atividade_extra 
                          where atividadeRecorrencia.cd_atividade_extra == cdAtividadeExtra
                          && (atividadeExtra.cd_pessoa_escola == cdEscola)
                          select atividadeRecorrencia).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}