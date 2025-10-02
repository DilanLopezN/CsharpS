using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Entity;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class TaxaMatriculaDataAccess : GenericRepository<TaxaMatricula>, ITaxaMatriculaDataAccess
    {
      
        

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public TaxaMatricula getTaxaMatriculaByIdContrato(int cd_contrato, int cd_pessoa_escola)
        {
            TaxaMatricula sql;
            try
            {
                sql = ((from taxaMatricula in db.TaxaMatricula
                        where (taxaMatricula.cd_contrato == cd_contrato
                             && taxaMatricula.Contrato.cd_pessoa_escola == cd_pessoa_escola)
                        select taxaMatricula)).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
            return sql;
        }

        public TaxaMatriculaSearchUI searchTaxaMatricula(int cd_contrato, int cd_pessoa_escola)
        {
            TaxaMatriculaSearchUI sql;
            try
            {
                sql = (from taxaMatricula in db.TaxaMatricula
                      where taxaMatricula.cd_contrato == cd_contrato
                           && taxaMatricula.Contrato.cd_pessoa_escola == cd_pessoa_escola
                      select new TaxaMatriculaSearchUI
                      {
                          cd_contrato = taxaMatricula.cd_contrato,
                          cd_pessoa_responsavel_taxa = taxaMatricula.cd_pessoa_responsavel_taxa,
                          cd_plano_conta_taxa = taxaMatricula.cd_plano_conta_taxa,
                          cd_taxa_matricula = taxaMatricula.cd_taxa_matricula,
                          cd_tipo_financeiro_taxa = taxaMatricula.cd_tipo_financeiro_taxa,
                          dt_vcto_taxa = taxaMatricula.dt_vcto_taxa,
                          nm_parcelas_taxa = taxaMatricula.nm_parcelas_taxa,
                          pc_responsavel_taxa = taxaMatricula.pc_responsavel_taxa,
                          vl_matricula_taxa = taxaMatricula.vl_matricula_taxa,
                          no_pessoa_responsavel = (from a in db.PessoaSGF where a.cd_pessoa == taxaMatricula.cd_pessoa_responsavel_taxa select a.no_pessoa).FirstOrDefault(),
                          dc_plano_conta_taxa = db.PlanoConta.Where(p => p.cd_plano_conta == taxaMatricula.cd_plano_conta_taxa).FirstOrDefault().PlanoContaSubgrupo.no_subgrupo_conta
                      }).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
            return sql;
        }
    }
}
