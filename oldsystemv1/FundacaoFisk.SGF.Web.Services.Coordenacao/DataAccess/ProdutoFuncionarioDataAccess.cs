using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.DataAccess
{
    public class ProdutoFuncionarioDataAccess : GenericRepository<ProdutoFuncionario>, IProdutoFuncionarioDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<ProdutoFuncionario> searchProdutosFuncionario(int cdFuncionario, int cdEscola)
        {
            try
            {
                var sql = from pf in db.ProdutoFuncionario
                    join produto in db.Produto on pf.cd_produto equals produto.cd_produto
                    where pf.cd_funcionario == cdFuncionario
                          && pf.Funcionario.cd_pessoa_empresa == cdEscola
                    select pf;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}