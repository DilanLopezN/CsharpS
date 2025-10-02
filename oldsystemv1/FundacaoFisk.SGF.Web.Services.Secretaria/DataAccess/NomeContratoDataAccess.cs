using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class NomeContratoDataAccess : GenericRepository<NomeContrato>, INomeContratoDataAccess
    {
        public enum TipoConsultaNomeContratoEnum
        {
            HAS_ATIVO = 0,
            HAS_ATIVO_MATRICULA = 1,
            HAS_REAJUSTE_ANUAL = 2
        }

        

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<NomeContrato> getSearchNoContrato(SearchParameters parametros, string desc, string layout, bool inicio, bool? status, int cdEscola, bool masterGeral)
        {
            try
            {
                IEntitySorter<NomeContrato> sorter = EntitySorter<NomeContrato>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<NomeContrato> sql;

                if (masterGeral)
                    sql = from noContrato in db.NomeContrato.AsNoTracking()
                          where (noContrato.cd_pessoa_escola == null)
                          select noContrato;
                else
                    sql = from noContrato in db.NomeContrato.AsNoTracking()
                          where (noContrato.cd_pessoa_escola == null || noContrato.cd_pessoa_escola == cdEscola)
                          select noContrato;

                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                        sql = from c in sql
                              where c.no_contrato.StartsWith(desc)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_contrato.Contains(desc)
                              select c;

                }

                if (!String.IsNullOrEmpty(layout))
                {
                    if (inicio)
                        sql = from c in sql
                              where c.no_relatorio .StartsWith(layout)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_relatorio.Contains(layout)
                              select c;

                }

                if (status != null)
                    sql = from c in sql
                          where c.id_nome_ativo == status
                          select c;

                //var retorno = sorter.Sort(retorno);
                sql = sorter.Sort(sql);

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public IEnumerable<NomeContrato> getNomesContratosByListaCodigos(int[] cdNomesContratos, int? cdEscola,bool masterGeral)
        {
            try
            {
                var result = from noCont in db.NomeContrato
                             where  cdNomesContratos.Contains(noCont.cd_nome_contrato)
                             select noCont;
                if (masterGeral)
                    result = from nc in result
                             where nc.cd_pessoa_escola == null
                             orderby nc.no_contrato
                             select nc;
                else
                    result = from nc in result
                             where
                              ((cdEscola.HasValue && nc.cd_pessoa_escola == cdEscola) || (nc.cd_pessoa_escola == null))
                             orderby nc.no_contrato
                             select nc;

                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public NomeContrato getNomeContratoById(int? cdEscola, int cdNomeContrato)
        {
            try
            {
                var result = (from noCont in db.NomeContrato
                             where noCont.cd_nome_contrato == cdNomeContrato &&
                             ((cdEscola.HasValue && noCont.cd_pessoa_escola == cdEscola) || (noCont.cd_pessoa_escola == null))
                              orderby noCont.no_contrato
                             select noCont).FirstOrDefault();

                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public string getNomeContratoDigitalizadoByCdContrato(int cdEscola, int cdContrato, string nomeArquivo)
        {
            try
            {
                var result = (from c in db.Contrato
                    where c.cd_pessoa_escola == cdEscola &&
                          c.cd_contrato == cdContrato &&
                          c.nm_arquivo_digitalizado == nomeArquivo
                    select c.nm_arquivo_digitalizado).FirstOrDefault();

                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public string getNomeContratoDigitalizadoByEscolaAndCdContrato(int cdEscola, int cdContrato)
        {
            try
            {
                var result = (from c in db.Contrato
                    where c.cd_pessoa_escola == cdEscola &&
                          c.cd_contrato == cdContrato 
                    select c.nm_arquivo_digitalizado).FirstOrDefault();

                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public IEnumerable<NomeContrato> getNomesContrato(TipoConsultaNomeContratoEnum hasDependente, int? cd_nome_contrato, int? cd_escola)
        {
            try
            {
                IQueryable<NomeContrato> sql = null;
                switch (hasDependente)
                {
                    case TipoConsultaNomeContratoEnum.HAS_ATIVO_MATRICULA:
                        sql = (from nc in db.NomeContrato
                              where ((nc.cd_pessoa_escola == (int)cd_escola) ||
                                    (nc.cd_pessoa_escola == null && !nc.NomeContratoFilhas.Any(x => x.cd_pessoa_escola == cd_escola))) &&
                                    (nc.id_nome_ativo || (cd_nome_contrato.HasValue && nc.cd_nome_contrato == cd_nome_contrato.Value))
                              orderby nc.no_contrato
                              select new
                              {
                                 cd_nome_contrato = nc.cd_nome_contrato,
                                 no_contrato = nc.no_contrato,
                                 no_relatorio = nc.no_relatorio,
                                 id_previsao_dias = nc.id_previsao_dias,
                                 id_valor_hora_aula = nc.id_valor_hora_aula,
                                 id_motivo_aditamento = nc.id_motivo_aditamento,
                                 id_tipo_pgto = nc.id_tipo_pgto,
                                 id_nome_ativo = nc.id_nome_ativo,
                                 cd_nome_contrato_pai = nc.cd_nome_contrato_pai,
                                 cd_pessoa_escola = nc.cd_pessoa_escola,
                                 id_valor_material = nc.id_valor_material,
                                 id_reajuste_anual = nc.id_reajuste_anual
  
                              }).ToList().Select(x => new NomeContrato()
                            {
                                cd_nome_contrato = x.cd_nome_contrato,
                                no_contrato = x.no_contrato,
                                no_relatorio = x.no_relatorio,
                                id_previsao_dias = x.id_previsao_dias,
                                id_valor_hora_aula = x.id_valor_hora_aula,
                                id_motivo_aditamento = x.id_motivo_aditamento,
                                id_tipo_pgto = x.id_tipo_pgto,
                                id_nome_ativo = x.id_nome_ativo,
                                cd_nome_contrato_pai = x.cd_nome_contrato_pai,
                                cd_pessoa_escola = x.cd_pessoa_escola,
                                id_valor_material = x.id_valor_material,
                                id_reajuste_anual = x.id_reajuste_anual
                            }).AsQueryable();
                        break;
                    case TipoConsultaNomeContratoEnum.HAS_REAJUSTE_ANUAL:
                        sql = (from nc in db.NomeContrato
                              where nc.id_reajuste_anual == true && ((nc.cd_pessoa_escola == (int)cd_escola) || (nc.cd_pessoa_escola == null && !nc.NomeContratoFilhas.Any(x => x.cd_pessoa_escola == cd_escola))) &&
                                    (nc.id_nome_ativo || (cd_nome_contrato.HasValue && nc.cd_nome_contrato == cd_nome_contrato.Value))
                              orderby nc.no_contrato
                              select new
                              {
                                  cd_nome_contrato = nc.cd_nome_contrato,
                                  no_contrato = nc.no_contrato,
                                  no_relatorio = nc.no_relatorio,
                                  id_previsao_dias = nc.id_previsao_dias,
                                  id_valor_hora_aula = nc.id_valor_hora_aula,
                                  id_motivo_aditamento = nc.id_motivo_aditamento,
                                  id_tipo_pgto = nc.id_tipo_pgto,
                                  id_nome_ativo = nc.id_nome_ativo,
                                  cd_nome_contrato_pai = nc.cd_nome_contrato_pai,
                                  cd_pessoa_escola = nc.cd_pessoa_escola,
                                  id_valor_material = nc.id_valor_material,
                                  id_reajuste_anual = nc.id_reajuste_anual
  
                              }).ToList().Select(x => new NomeContrato()
                                {
                                    cd_nome_contrato = x.cd_nome_contrato,
                                    no_contrato = x.no_contrato,
                                    no_relatorio = x.no_relatorio,
                                    id_previsao_dias = x.id_previsao_dias,
                                    id_valor_hora_aula = x.id_valor_hora_aula,
                                    id_motivo_aditamento = x.id_motivo_aditamento,
                                    id_tipo_pgto = x.id_tipo_pgto,
                                    id_nome_ativo = x.id_nome_ativo,
                                    cd_nome_contrato_pai = x.cd_nome_contrato_pai,
                                    cd_pessoa_escola = x.cd_pessoa_escola,
                                    id_valor_material = x.id_valor_material,
                                    id_reajuste_anual = x.id_reajuste_anual
                                }).AsQueryable();
                        break;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public NomeContrato getNomeContratoAditamentoMatricula(int cd_contrato, int cd_escola)
        {
            try
            {
                //var sql = from nc in db.NomeContrato
                //           where nc.Aditamento.Where(a => a.cd_contrato == cd_contrato && a.Contrato.cd_pessoa_escola == cd_escola && 
                //               !(from ad in db.Aditamento
                //                 where ad.cd_contrato == a.cd_contrato 
                //                 && a.cd_aditamento != ad.cd_aditamento
                //                 && ad.dt_aditamento > a.dt_aditamento
                //                 select ad.dt_aditamento).Any()
                //           ).Any()
                //           select nc;
                //return sql.FirstOrDefault();

                NomeContrato sql = (from nc in db.NomeContrato
                 where nc.Aditamento.Where(a => a.cd_nome_contrato == nc.cd_nome_contrato &&
                     a.cd_contrato == cd_contrato &&
                     a.dt_aditamento == (from ad in db.Aditamento where ad.cd_contrato == a.cd_contrato select ad.dt_aditamento).Max()
                     ).Any()
                 select nc).FirstOrDefault();
                if (sql == null)
                {
                     sql = (from nc in db.NomeContrato
                            where nc.T_CONTRATO.Where(a => a.cd_nome_contrato == nc.cd_nome_contrato &&
                                   a.cd_contrato == cd_contrato).Any() //&&  LBM Aqui só tem um por contrato
                                                                       //a.dt_aditamento == (from ad in db.Aditamento where ad.cd_contrato == a.cd_contrato select ad.dt_aditamento).Max()
                           select nc).FirstOrDefault();
                }

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<NomeContrato> getNomeContratoMat(int? cdEscola)
        {
            try
            {
                var result =  from noCont in db.NomeContrato
                              where noCont.Aditamento.Any() &&
                              ((cdEscola.HasValue && noCont.cd_pessoa_escola == cdEscola) || (noCont.cd_pessoa_escola == null))
                              orderby noCont.no_contrato
                              select noCont;

                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

    }
}
