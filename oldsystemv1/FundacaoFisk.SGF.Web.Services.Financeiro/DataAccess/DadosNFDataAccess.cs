using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class DadosNFDataAccess : GenericRepository<DadosNF>, IDadosNFDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<DadosNF> getDadosNFSearch(SearchParameters parametros, int cdCidade, string natOp, double? aliquota, byte id_regime)
        {
            try
            {
                IEntitySorter<DadosNF> sorter = EntitySorter<DadosNF>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<DadosNF> sql;
                sql = from c in db.DadosNF.AsNoTracking()
                      orderby c.cd_cidade ascending
                      select c;

                if (cdCidade > 0)
                    sql = from s in sql
                          where s.cd_cidade == cdCidade
                          select s;
                if (!String.IsNullOrEmpty(natOp))
                    sql = from c in sql
                          where c.dc_natureza_operacao.Contains(natOp)
                          select c;

                if (aliquota != null)
                    sql = from s in sql
                          where s.pc_aliquota_iss == aliquota
                          select s;

                if(id_regime > 0)
                    sql = from s in sql
                          where s.id_regime_tributario == id_regime
                          select s; 
                sql = sorter.Sort(sql);
                IEnumerable<DadosNF> sql1 = (from s in sql
                                             select new
                                             {
                                                 cd_cidade = s.cd_cidade,
                                                 cd_dados_nf = s.cd_dados_nf,
                                                 dc_item_servico = s.dc_item_servico,
                                                 dc_natureza_operacao = s.dc_natureza_operacao,
                                                 dc_tributacao_municipio = s.dc_tributacao_municipio,
                                                 no_cidade = s.Localidade.no_localidade,
                                                 pc_aliquota_iss = s.pc_aliquota_iss,
                                                 id_regime_tributario = s.id_regime_tributario
                                             }).ToList().Select(x => new DadosNF
                                             {
                                                 cd_cidade = x.cd_cidade,
                                                 cd_dados_nf = x.cd_dados_nf,
                                                 dc_item_servico = x.dc_item_servico,
                                                 dc_natureza_operacao = x.dc_natureza_operacao,
                                                 dc_tributacao_municipio = x.dc_tributacao_municipio,
                                                 Localidade = new LocalidadeSGF { no_localidade = x.no_cidade },
                                                 pc_aliquota_iss = x.pc_aliquota_iss,
                                                 id_regime_tributario = x.id_regime_tributario
                                             });
                int limite = sql1.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql1 = sql1.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sql1;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public DadosNF getDadosNFById(int cdAliquota)
        {
            try
            {
                DadosNF sql = (from s in db.DadosNF
                               where s.cd_dados_nf == cdAliquota
                               select new
                               {
                                   cd_cidade = s.cd_cidade,
                                   cd_dados_nf = s.cd_dados_nf,
                                   dc_item_servico = s.dc_item_servico,
                                   dc_natureza_operacao = s.dc_natureza_operacao,
                                   dc_tributacao_municipio = s.dc_tributacao_municipio,
                                   no_cidade = s.Localidade.no_localidade,
                                   pc_aliquota_iss = s.pc_aliquota_iss,
                                   id_regime_tributario = s.id_regime_tributario
                               }).ToList().Select(x => new DadosNF
                                {
                                    cd_cidade = x.cd_cidade,
                                    cd_dados_nf = x.cd_dados_nf,
                                    dc_item_servico = x.dc_item_servico,
                                    dc_natureza_operacao = x.dc_natureza_operacao,
                                    dc_tributacao_municipio = x.dc_tributacao_municipio,
                                    Localidade = new LocalidadeSGF { no_localidade = x.no_cidade },
                                    pc_aliquota_iss = x.pc_aliquota_iss,
                                    id_regime_tributario = x.id_regime_tributario
                                }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool getDadosCidade(int cdCidade)
        {
            try
            {
                DadosNF sql = (from l in db.DadosNF
                               where l.cd_cidade == cdCidade
                               select l).FirstOrDefault();
                return sql != null && sql.cd_dados_nf > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public double? getISSEscola(int cdEscola)
        {
            try
            {
                double? aliquotaISS = null;
                DadosNF dados = (from l in db.DadosNF
                                 where db.PessoaSGF.Where(p => p.cd_pessoa == cdEscola && p.EnderecoPrincipal.cd_loc_cidade == l.cd_cidade).Any()
                                 select l).FirstOrDefault();
                if (dados != null)
                    aliquotaISS = dados.pc_aliquota_iss;
                return aliquotaISS;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        
    }
}
