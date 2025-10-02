using System;
using System.Collections.Generic;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess;
using System.Linq;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class EmpresaValorServicoDataAccess: GenericRepository<EmpresaValorServico>, IEmpresaValorServicoDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<EmpresaValorServico> getEmpresaValorServicoByEscola(int cdEscola)
        {
            try
            {
                var sql = (from evs in db.EmpresaValorServico
                           where evs.cd_pessoa_empresa == cdEscola
                           select evs);
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public EmpresaValorServico getEmpresaValorServicoByIdAndEscola(int CdEmpresaValorServico, int CdPessoaEmpresa)
        {
            try
            {
                var sql = (from evs in db.EmpresaValorServico
                           where evs.cd_empresa_valor_servico == CdEmpresaValorServico && evs.cd_pessoa_empresa == CdPessoaEmpresa
                           select evs).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}