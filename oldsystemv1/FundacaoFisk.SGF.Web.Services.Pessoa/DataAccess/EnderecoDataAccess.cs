using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess
{
    public class EnderecoDataAccess : GenericRepository<EnderecoSGF>, IEnderecoDataAccess
    {
        
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<EnderecoSGF> GetAllEnderecoByPessoa(int cdPessoa, int cd_endereco)
        {
            try{
                var sql = from endereco in db.EnderecoSGF
                          where endereco.cd_pessoa == cdPessoa &&
                          (cd_endereco == 0 || endereco.cd_endereco != cd_endereco)
                          select endereco;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public EnderecoSGF getEnderecoByLogradouro(int cd_logradouro, string nm_cep)
        {
            try
            {
                EnderecoSGF retorno = new EnderecoSGF();
                var sql = from l in db.LocalidadeSGF
                          where l.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.LOGRADOURO
                          select l;
                if (cd_logradouro > 0)
                    sql = from l in sql
                          where l.cd_localidade == cd_logradouro
                          select l;
                else
                    if(!string.IsNullOrEmpty(nm_cep))
                        sql = from l in sql
                              where l.dc_num_cep == nm_cep
                              select l;
               
                try
                {
                    retorno = (from l in sql
                               select new
                               {
                                   cd_loc_estado = l.LocalidadeRelacionada.LocalidadeRelacionada.LocalidadeRelacionada.cd_localidade,
                                   cd_loc_cidade = l.LocalidadeRelacionada.LocalidadeRelacionada.cd_localidade,
                                   noLocCidade = l.LocalidadeRelacionada.LocalidadeRelacionada.no_localidade,
                                   cd_loc_bairro = l.LocalidadeRelacionada.cd_localidade,
                                   noLocBairro = l.LocalidadeRelacionada.no_localidade,
                                   cd_loc_logradouro = l.cd_localidade,
                                   noLocRua = l.no_localidade,
                                   dc_num_cep = l.dc_num_cep
                               }).ToList().Select(x => new EnderecoSGF
                               {
                                   cd_loc_estado =x.cd_loc_estado,
                                   cd_loc_cidade = x.cd_loc_cidade,
                                   noLocCidade = x.noLocCidade,
                                   cd_loc_bairro = x.cd_loc_bairro,
                                   noLocBairro = x.noLocBairro,
                                   cd_loc_logradouro = x.cd_loc_logradouro,
                                   noLocRua = x.noLocRua,
                                   num_cep = x.dc_num_cep
                               }).FirstOrDefault();
                }
                catch (Exception e)
                {
                    throw new PessoaBusinessException(String.Empty, e, PessoaBusinessException.TipoErro.ERRO_CONVERSAO_ENDERECO, true, cd_logradouro, nm_cep);
                }
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public EnderecoSGF getEnderecoResponsavelCPF(int cd_pessoa)
        {

            try
            {
                var sql = (from e in db.EnderecoSGF
                          where e.cd_pessoa == cd_pessoa && e.PessoasEnderecoPrincipal.Any()
                           select new
                           {
                               cd_endereco = e.cd_endereco,
                               cd_loc_estado = e.cd_loc_estado,
                               cd_loc_cidade = e.cd_loc_cidade,
                               cd_loc_bairro = e.cd_loc_bairro,
                               cd_loc_logradouro = e.cd_loc_logradouro,
                               cd_tipo_endereco = e.cd_tipo_endereco,
                               cd_tipo_logradouro = e.cd_tipo_logradouro,
                               noLocCidade = e.Cidade.no_localidade,
                               noLocBairro = e.Bairro.no_localidade,
                               noLocRua = e.Logradouro.no_localidade,
                               dc_num_cep = e.Logradouro.dc_num_cep,
                               numero = e.dc_num_endereco,
                               dc_complemento = e.dc_compl_endereco,
                               cd_loc_pais = e.cd_loc_pais
                           }).ToList().Select(x => new EnderecoSGF
                           {
                               cd_endereco = x.cd_endereco,
                               cd_loc_estado = x.cd_loc_estado,
                               cd_loc_cidade = x.cd_loc_cidade,
                               cd_loc_bairro = x.cd_loc_bairro,
                               cd_loc_logradouro = x.cd_loc_logradouro,
                               cd_tipo_endereco = x.cd_tipo_endereco,
                               cd_tipo_logradouro = x.cd_tipo_logradouro,
                               noLocCidade = x.noLocCidade,
                               noLocBairro = x.noLocBairro,
                               noLocRua = x.noLocRua,
                               num_cep = x.dc_num_cep,
                               dc_num_endereco = x.numero,
                               cd_loc_pais = x.cd_loc_pais,
                               dc_compl_endereco = x.dc_complemento,
                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public EnderecoSGF getEnderecoByCdEndereco(int cd_endereco)
        {

            try
            {
                var sql = (from e in db.EnderecoSGF
                        .Include(x => x.Cidade)
                        .Include(x=> x.Estado)
                        .Include(x => x.Estado.Estado)
                           where e.cd_endereco == cd_endereco
                           select e).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
