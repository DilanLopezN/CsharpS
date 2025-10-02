using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess
{
    public class RelacionamentoDataAccess :  GenericRepository<RelacionamentoSGF>, IRelacionamentoDataAccess
    {
        public enum TipoRelacionamento
        {
            RESPONSAVEL_FINANCEIRO = 3,
            ALUNO_RESPONSAVEL = 9,
            COORDENADOR = 14
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public bool addRelacionamentoResponsavelAluno(RelacionamentoSGF pessoaRelac)
        {
            try
            {
                int ret = 0;
                var exist = (from r in db.RelacionamentoSGF
                            where
                              r.cd_pessoa_pai == pessoaRelac.cd_pessoa_pai &&
                              r.cd_pessoa_filho == pessoaRelac.cd_pessoa_filho &&
                              r.cd_papel_filho == (int)PapelSGF.TipoPapelSGF.RESPONSAVEL &&
                              r.cd_papel_pai == (int)PapelSGF.TipoPapelSGF.ALUNORESPONSAVEL 
                            select r.cd_relacionamento).Any();
                if (!exist)
                {
                    db.RelacionamentoSGF.Add(pessoaRelac);
                    ret = db.SaveChanges();
                }

                return ret > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
