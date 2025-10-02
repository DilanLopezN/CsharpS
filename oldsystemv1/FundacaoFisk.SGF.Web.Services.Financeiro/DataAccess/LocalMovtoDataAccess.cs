using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class LocalMovtoDataAccess : GenericRepository<LocalMovto>, ILocalMovtoDataAccess
    {

        public enum TipoConsultaLocalMovto
        {
            HAS_ATIVO = 0,
            HAS_TITULOS_NATUREZA_RECEBER = 1,
            HAS_TITULOS_NATUREZA_PAGAR = 2,
            HAS_TODOS_TITULOS = 3,
            HAS_ATIVO_E_BANCOS = 4,
            HAS_ATIVO_BAIXA_TITULO = 5,
            HAS_ATIVO_ESCOLA_SEM_CARTEIRA = 6,
            HAS_BAIXA_NATUREZA_RECEBER = 7,
            HAS_BAIXA_NATUREZA_PAGAR = 8,
            HAS_TODAS_BAIXAS = 9,
            HAS_SIMULACAO_BAIXA = 10,
            HAS_CNAB = 11,
            HAS_FILTRO_CNAB = 12,
            HAS_SIMULACAO_BAIXA_GERAL_SEM_CARTAO = 13,
            HAS_SIMULACAO_BAIXA_REGRAS_TROCA_FINANCEIRA = 14,
            HAS_ATIVO_E_BANCOS_NAO_CARTAO = 15,
            HAS_ATIVO_E_BANCOS_E_CARTEIRAS = 16
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }
        public enum LocalMovtoEnum
        {
            HAS_MATRICULA = 1,
            HAS_ESCOLA = 2
        }

        public List<LocalMovto> getLocalMovtoByEscola(int cdEscola, int cd_local_movto, bool semcarteira)
        {
            try
            {
                var nmLocal = (from l in db.LocalMovto where l.cd_local_movto == cd_local_movto select l.nm_tipo_local).FirstOrDefault();
                if (cd_local_movto == 0) nmLocal = 0;
                var sql = (from l in db.LocalMovto
                           where l.cd_pessoa_empresa == cdEscola &&
                                (
                                    (l.id_local_ativo == true &&
                                    ((nmLocal == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                     nmLocal == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) ?
                                    (l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                     l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) :
                                    ((l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO  ||
                                     (l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTEIRA) && semcarteira == false) &&
                                    l.cd_pessoa_local == null))) || 
                                    (l.cd_local_movto == cd_local_movto)
                                )
                           orderby l.no_local_movto
                           select new
                           {
                               l.cd_local_movto,
                               l.no_local_movto,
                               l.nm_agencia,
                               l.nm_conta_corrente,
                               l.nm_digito_conta_corrente,
                               l.nm_tipo_local
                           }).ToList().Select(x => new LocalMovto
                          {
                              cd_local_movto = x.cd_local_movto,
                              nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                              no_local_movto = x.no_local_movto,
                              nm_tipo_local = x.nm_tipo_local

                          }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<LocalMovto> getAllLocalMovtoByEscola(int cdEscola, int cd_local_movto)
        {
            try
            {
                var sql = (from l in db.LocalMovto
                    where l.cd_pessoa_empresa == cdEscola &&
                          (((l.id_local_ativo == true && (l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ||
                                                         l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTEIRA) &&
                            l.cd_pessoa_local == null) || (l.cd_local_movto == cd_local_movto)
                          )||
                          ((l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                            l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                            l.id_local_ativo == true))
                    orderby l.no_local_movto
                    select new
                    {
                        l.cd_local_movto,
                        l.no_local_movto,
                        l.nm_agencia,
                        l.nm_conta_corrente,
                        l.nm_digito_conta_corrente,
                        l.nm_tipo_local,
                        cd_pessoa_empresa = l.cd_pessoa_empresa,
                        T_LOCAL_MOVTO1 = l.T_LOCAL_MOVTO1,
                        T_LOCAL_MOVTO2 = l.T_LOCAL_MOVTO2,
                        cd_local_banco = l.cd_local_banco,
                        taxaBancaria = l.TaxaBancaria

                    }).ToList().Select(x => new LocalMovto
                {
                    cd_local_movto = x.cd_local_movto,
                    nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                    no_local_movto = x.no_local_movto,
                    nm_tipo_local = x.nm_tipo_local,
                    cd_pessoa_empresa = x.cd_pessoa_empresa,
                    cd_local_banco = x.cd_local_banco,
                    T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                    T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,

                    TaxaBancaria = x.taxaBancaria
                }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalMovto> getAllLocalMovtoBanco(int cdEscola)
        {
            try
            {
                var sql = (from l in db.LocalMovto
                           where l.cd_pessoa_empresa == cdEscola &&
                                ((l.id_local_ativo == true && (l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO)))
                           orderby l.no_local_movto
                           select new
                           {
                               l.cd_local_movto,
                               l.no_local_movto,
                               l.nm_agencia,
                               l.nm_conta_corrente,
                               l.nm_digito_conta_corrente,
                               l.nm_tipo_local
                           }).ToList().Select(x => new LocalMovto
                           {
                               cd_local_movto = x.cd_local_movto,
                               nomeLocal = x.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                               no_local_movto = x.no_local_movto
                           }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

         public int findByLocal(int? cd_local_movt, int cd_tipo_liquidacao)
        {   int result;
            try
            {
                LocalMovto sql = (from l in db.LocalMovto
                           where  l.cd_local_movto == cd_local_movt 
                           select new
                           {
                              cd_local_movto  = l.cd_local_movto ,
                              nm_tipo_local  = l.nm_tipo_local,
                              cd_pessoa_empresa = l.cd_pessoa_empresa
                           }).ToList().Select(x => new LocalMovto
                           {
                               cd_local_movto = x.cd_local_movto,
                               nm_tipo_local  = x.nm_tipo_local,
                               cd_pessoa_empresa = x.cd_pessoa_empresa
                           }).ToList().FirstOrDefault();

                if(sql.nm_tipo_local != (byte)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO && sql.nm_tipo_local != (byte)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) {
                  LocalMovto sqlf = (from l in db.LocalMovto
                           where  l.cd_local_banco == sql.cd_local_movto &&
                                  l.nm_tipo_local == (cd_tipo_liquidacao == (int)LocalMovto.TipoLocalMovtoEnum.BANCO ? (byte)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO : (byte)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO)
                           select new
                           {
                              cd_local_movto  = l.cd_local_movto,
                              nm_tipo_local  = l.nm_tipo_local
                           }).ToList().Select(x => new LocalMovto
                           {
                               cd_local_movto = x.cd_local_movto,
                               nm_tipo_local  = x.nm_tipo_local
                           }).ToList().FirstOrDefault();

                    if (sqlf == null)
                    sqlf = (from l in db.LocalMovto
                           where  l.cd_pessoa_empresa == sql.cd_pessoa_empresa &&
                                  l.nm_tipo_local == (cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CARTAO_CREDITO ? (byte)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO : (byte)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO)
                           select new
                           {
                              cd_local_movto  = l.cd_local_movto,
                              nm_tipo_local  = l.nm_tipo_local
                           }).ToList().Select(x => new LocalMovto
                           {
                               cd_local_movto = x.cd_local_movto,
                               nm_tipo_local  = x.nm_tipo_local
                           }).ToList().FirstOrDefault();
                    result =  (sqlf != null ? sqlf.cd_local_movto : 0);
                }else {
                    result =  (sql != null ? sql.cd_local_movto : 0);
                }

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
            return result;
        }

        public List<LocalMovto> getLocalMovtoCdEEsc(int cdEscola, int? cdLocalMovto)
        {
            try
            {
                var sql = (from l in db.LocalMovto
                           where (l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ||
                               l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTEIRA) &&
                             l.cd_pessoa_local == null &&
                             l.id_local_ativo == true &&
                             l.cd_pessoa_empresa == cdEscola &&
                             (cdLocalMovto == null || l.cd_local_movto == cdLocalMovto)
                           orderby l.no_local_movto
                           select new
                           {
                               cd_local_movto = l.cd_local_movto,
                               nm_banco = l.no_local_movto,
                               nm_agencia = l.nm_agencia,
                               nm_conta_corrente = l.nm_conta_corrente,
                               nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                               nm_tipo_local = l.nm_tipo_local
                           }).ToList().Select(x => new LocalMovto
                           {
                               cd_local_movto = x.cd_local_movto,
                               nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                           }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public LocalMovto findCodigoClienteForCnab(int cd_escola, int cd_local)
        {
            try
            {
                var sql = (from l in db.LocalMovto
                           where
                             l.cd_pessoa_empresa == cd_escola &&
                             l.cd_local_movto == cd_local
                           select new
                           {
                               dc_num_cliente_banco = l.dc_num_cliente_banco,
                               nm_banco = l.Banco.nm_banco,
                               nm_sequencia = l.nm_sequencia
                           }).ToList().Select(x => new LocalMovto
                           {
                               dc_num_cliente_banco = x.dc_num_cliente_banco,
                               Banco = new Banco()
                               {
                                   nm_banco = x.nm_banco
                               },
                               nm_sequencia = x.nm_sequencia
                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalMovtoUI> getLocalMovtoSearch(SearchParameters parametros, int cdEscola, string nome, string nmBanco, bool inicio, bool? status, int tipo, string pessoa, int cd_pessoa_usuario)
        {
            try
            {
                IEntitySorter<LocalMovtoUI> sorter = EntitySorter<LocalMovtoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<LocalMovtoUI> sql;

                if (status == null)
                {
                    if (cd_pessoa_usuario == 0)
                        sql = from local in db.LocalMovto.AsNoTracking()
                              where local.cd_pessoa_empresa == cdEscola
                              select new LocalMovtoUI
                              {
                                  cd_local_movto = local.cd_local_movto,
                                  nm_tipo_local = local.nm_tipo_local,
                                  no_local_movto = local.no_local_movto,
                                  nm_banco = local.Banco.nm_banco,
                                  nm_agencia = local.nm_agencia,
                                  nm_conta_corrente = local.nm_conta_corrente,
                                  nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                                  no_pessoa_local = local.PessoaSGFLocal.no_pessoa,
                                  id_local_ativo = local.id_local_ativo,
                                  id_conta_conjunta = local.id_conta_conjunta,
                                  nm_digito_agencia = local.nm_digito_agencia
                              };

                    else
                        sql = from local in db.LocalMovto.AsNoTracking()
                              where local.cd_pessoa_empresa == cdEscola
                              && (cd_pessoa_usuario > 0 && (local.cd_pessoa_local == cd_pessoa_usuario || local.cd_pessoa_local == null
                                && local.nm_tipo_local == (byte)LocalMovto.TipoLocalMovtoEnum.CAIXA
                                   || local.nm_tipo_local != (byte)LocalMovto.TipoLocalMovtoEnum.CAIXA))
                              select new LocalMovtoUI
                              {
                                  cd_local_movto = local.cd_local_movto,
                                  nm_tipo_local = local.nm_tipo_local,
                                  no_local_movto = local.no_local_movto,
                                  nm_banco = local.Banco.nm_banco,
                                  nm_agencia = local.nm_agencia,
                                  nm_conta_corrente = local.nm_conta_corrente,
                                  nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                                  no_pessoa_local = local.PessoaSGFLocal.no_pessoa,
                                  id_local_ativo = local.id_local_ativo,
                                  id_conta_conjunta = local.id_conta_conjunta,
                                  nm_digito_agencia = local.nm_digito_agencia
                              };
                }

                else
                {
                    if (cd_pessoa_usuario == 0)
                        sql = from local in db.LocalMovto
                              where
                                local.cd_pessoa_empresa == cdEscola &&
                                local.id_local_ativo == status
                              select new LocalMovtoUI
                              {
                                  cd_local_movto = local.cd_local_movto,
                                  nm_tipo_local = local.nm_tipo_local,
                                  no_local_movto = local.no_local_movto,
                                  nm_banco = local.Banco.nm_banco,
                                  nm_agencia = local.nm_agencia,
                                  nm_conta_corrente = local.nm_conta_corrente,
                                  nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                                  no_pessoa_local = local.PessoaSGFLocal.no_pessoa,
                                  id_local_ativo = local.id_local_ativo,
                                  id_conta_conjunta = local.id_conta_conjunta,
                                  nm_digito_agencia = local.nm_digito_agencia
                              };
                    else
                        sql = from local in db.LocalMovto
                              where
                                local.cd_pessoa_empresa == cdEscola &&
                                local.id_local_ativo == status
                                 && (cd_pessoa_usuario > 0 && (local.cd_pessoa_local == cd_pessoa_usuario || local.cd_pessoa_local == null
                                 && local.nm_tipo_local == (byte)LocalMovto.TipoLocalMovtoEnum.CAIXA
                                   || local.nm_tipo_local != (byte)LocalMovto.TipoLocalMovtoEnum.CAIXA))
                              select new LocalMovtoUI
                              {
                                  cd_local_movto = local.cd_local_movto,
                                  nm_tipo_local = local.nm_tipo_local,
                                  no_local_movto = local.no_local_movto,
                                  nm_banco = local.Banco.nm_banco,
                                  nm_agencia = local.nm_agencia,
                                  nm_conta_corrente = local.nm_conta_corrente,
                                  nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                                  no_pessoa_local = local.PessoaSGFLocal.no_pessoa,
                                  id_local_ativo = local.id_local_ativo,
                                  id_conta_conjunta = local.id_conta_conjunta,
                                  nm_digito_agencia = local.nm_digito_agencia
                              };
                }
                sql = sorter.Sort(sql);

                var retorno = from c in sql
                              select c;

                if (!String.IsNullOrEmpty(nome))
                    if (inicio)
                        retorno = from c in retorno
                                  where c.no_local_movto.StartsWith(nome)
                                  select c;
                    else
                        retorno = from c in retorno
                                  where c.no_local_movto.Contains(nome)
                                  select c;

                if (!String.IsNullOrEmpty(nmBanco))
                    if (inicio)
                        retorno = from c in retorno
                                  where c.nm_banco.StartsWith(nmBanco)
                                  select c;
                    else
                        retorno = from c in retorno
                                  where c.nm_banco.Contains(nmBanco)
                                  select c;

                if (tipo > 0)
                    retorno = from c in retorno
                              where c.nm_tipo_local == tipo
                              select c;
                if (!String.IsNullOrEmpty(pessoa))
                    retorno = from c in retorno
                              where c.no_pessoa_local.Contains(pessoa)
                              select c;


                int limite = retorno.Count();
                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public long getNossoNumeroLocalMovimento(int cd_escola, int cd_local_movto)
        {
            try
            {
                var sql = (from local in db.LocalMovto
                           where local.cd_pessoa_empresa == cd_escola &&
                             local.cd_local_movto == cd_escola
                           select local.dc_nosso_numero).FirstOrDefault();
                return sql.Value;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public LocalMovtoUI getLocalMovtoById(int cdEscola, int cdLocalMovto)
        {
            try
            {

                var sql = (from local in db.LocalMovto
                           where local.cd_pessoa_empresa == cdEscola &&
                             local.cd_local_movto == cdLocalMovto
                           select new
                           {
                               cd_local_movto = local.cd_local_movto,
                               nm_tipo_local = local.nm_tipo_local,
                               cd_pessoa_local = local.cd_pessoa_local,
                               cd_banco = local.cd_banco,
                               cd_pessoa_banco = local.cd_pessoa_banco,
                               dc_num_cliente_banco = local.dc_num_cliente_banco,
                               dc_nosso_numero = local.dc_nosso_numero,
                               dc_pessoa_conjunta = local.dc_pessoa_conjunta,
                               dc_cpf_pessoa_conjunta = local.dc_cpf_pessoa_conjunta,
                               no_local_movto = local.no_local_movto,
                               nm_banco = local.Banco.nm_banco,
                               nm_agencia = local.nm_agencia,
                               nm_conta_corrente = local.nm_conta_corrente,
                               nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                               no_pessoa_banco = local.PessoaSGFBanco.no_pessoa,
                               no_pessoa_local = local.PessoaSGFLocal.no_pessoa,
                               id_local_ativo = local.id_local_ativo,
                               id_conta_conjunta = local.id_conta_conjunta,
                               cd_carteira_cnab = local.cd_carteira_cnab,
                               no_carteira = local.CarteiraCnab.no_carteira,
                               nm_sequencia = local.nm_sequencia,
                               nm_transmissao = local.nm_transmissao,
                               nm_digito_cedente = local.nm_digito_cedente,
                               nm_op_conta = local.nm_op_conta,
                               local.nm_digito_agencia,
                               cd_local_banco = local.cd_local_banco
                           }).ToList().Select(x => new LocalMovtoUI
                          {
                              cd_local_movto = x.cd_local_movto,
                              nm_tipo_local = x.nm_tipo_local,
                              cd_pessoa_local = x.cd_pessoa_local,
                              cd_banco = x.cd_banco,
                              cd_pessoa_banco = x.cd_pessoa_banco,
                              dc_num_cliente_banco = x.dc_num_cliente_banco,
                              dc_nosso_numero = x.dc_nosso_numero,
                              dc_pessoa_conjunta = x.dc_pessoa_conjunta,
                              dc_cpf_pessoa_conjunta = x.dc_cpf_pessoa_conjunta,
                              no_local_movto = x.no_local_movto,
                              nm_banco = x.nm_banco,
                              nm_agencia = x.nm_agencia,
                              nm_conta_corrente = x.nm_conta_corrente,
                              nm_digito_conta_corrente = x.nm_digito_conta_corrente,
                              no_pessoa_banco = x.no_pessoa_banco,
                              no_pessoa_local = x.no_pessoa_local,
                              id_local_ativo = x.id_local_ativo,
                              id_conta_conjunta = x.id_conta_conjunta,
                              nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                              cd_carteira_cnab = x.cd_carteira_cnab,
                              noCarteira = x.no_carteira,
                              nm_sequencia = x.nm_sequencia,
                              nm_transmissao = x.nm_transmissao,
                              nm_digito_cedente = x.nm_digito_cedente,
                              nm_op_conta = x.nm_op_conta,
                              nm_digito_agencia = x.nm_digito_agencia,
                              taxaBancaria = db.TaxaBancaria.Where(b => b.cd_local_movto == x.cd_local_movto).ToList(),
                              cd_local_banco = x.cd_local_banco
                          }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public LocalMovto findLocalMovtoComCarteira(int cdEscola, int cdLocalMovto) {
            try {
                var sql = (from local in db.LocalMovto.Include(l => l.CarteiraCnab)
                           where local.cd_pessoa_empresa == cdEscola &&
                             local.cd_local_movto == cdLocalMovto
                           select local).FirstOrDefault();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public LocalMovto findLocalMovtoById(int cdEscola, int cdLocalMovto)
        {
            try
            {
                var sql = (from local in db.LocalMovto
                           where local.cd_pessoa_empresa == cdEscola &&
                             local.cd_local_movto == cdLocalMovto
                           select local).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<LocalMovto> getLocalMovimento(int cdEscola, int cd_loc_mvto, TipoConsultaLocalMovto tipoConsulta, int cd_pessoa_usuario)
        {
            try
            {
                bool isMaster = cd_pessoa_usuario <= 0;
                IEnumerable<LocalMovto> sql = null;
                switch (tipoConsulta)
                {
                    case TipoConsultaLocalMovto.HAS_ATIVO:
                        if (isMaster)
                            sql = from l in db.LocalMovto
                                  where l.cd_pessoa_empresa == cdEscola
                                     && l.id_local_ativo == true
                                     && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                  select l;
                        else
                            sql = from l in db.LocalMovto
                                  where l.cd_pessoa_empresa == cdEscola
                                     && l.id_local_ativo == true
                                     && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                     && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                      && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                      || l.nm_tipo_local != (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)

                                  select l;
                        break;
                    case TipoConsultaLocalMovto.HAS_ATIVO_E_BANCOS:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto < 0 || l.id_local_ativo == true) 
                                      && (cd_loc_mvto <= 0 || l.cd_local_movto == cd_loc_mvto)
                                   orderby l.no_local_movto
                                   select new
                                    {
                                        cd_local_movto = l.cd_local_movto,
                                        nm_banco = l.no_local_movto,
                                        nm_agencia = l.nm_agencia,
                                        nm_conta_corrente = l.nm_conta_corrente,
                                        nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                        nm_tipo_local = l.nm_tipo_local
                                    }).ToList().Select(x => new LocalMovto
                                       {
                                           cd_local_movto = x.cd_local_movto,
                                           nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                           x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                       }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && l.id_local_ativo == true
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                        && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                        || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_ATIVO_E_BANCOS_E_CARTEIRAS:
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto < 0 || l.id_local_ativo == true)
                                      && (cd_loc_mvto <= 0 || l.cd_local_movto == cd_loc_mvto)
                                      && (l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ||
                                          l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTEIRA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                          x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_ATIVO_E_BANCOS_NAO_CARTAO:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto < 0 || l.id_local_ativo == true)
                                      && (cd_loc_mvto <= 0 || l.cd_local_movto == cd_loc_mvto)
                                      && !(l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                           l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO ||
                                           l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA) 

                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                          x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && l.id_local_ativo == true
                                      && (cd_loc_mvto <= 0 || l.cd_local_movto == cd_loc_mvto)
                                      && !(l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                           l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO ||
                                           l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_TITULOS_NATUREZA_RECEBER:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.Titulos.Where(t => t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER).Any()
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                       {
                                           cd_local_movto = x.cd_local_movto,
                                           nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                           x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                       }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.Titulos.Where(t => t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER).Any()
                                      && ( (l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null)
                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA
                                       || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_BAIXA_NATUREZA_RECEBER:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                    && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                    && l.BaixaTitulo.Where(bt => bt.cd_local_movto == l.cd_local_movto
                                    && bt.Titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER).Any()
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                    && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                    && l.BaixaTitulo.Where(bt => bt.cd_local_movto == l.cd_local_movto
                                    && bt.Titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER).Any()
                                    && (((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null)
                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                       || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_TITULOS_NATUREZA_PAGAR:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.Titulos.Where(t => t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.PAGAR).Any()
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.Titulos.Where(t => t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.PAGAR).Any()
                                      && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                       || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_BAIXA_NATUREZA_PAGAR:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                     && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                     && l.BaixaTitulo.Where(bt => bt.cd_local_movto == l.cd_local_movto
                                     && bt.Titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.PAGAR).Any()
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                     && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                     && l.BaixaTitulo.Where(bt => bt.cd_local_movto == l.cd_local_movto
                                                            && bt.Titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.PAGAR).Any()
                                     && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                       || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_TODOS_TITULOS:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.Titulos.Any()
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.Titulos.Any()
                                      && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                        && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                        || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_TODAS_BAIXAS:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.BaixaTitulo.Any()
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.BaixaTitulo.Any()
                                      && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                       || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                    {
                                        cd_local_movto = x.cd_local_movto,
                                        nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                        x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                    }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_ATIVO_BAIXA_TITULO:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && l.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTEIRA
                                      || (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && l.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTEIRA
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                       && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                       || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_ATIVO_ESCOLA_SEM_CARTEIRA:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                     && (l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTEIRA)
                                     && (l.id_local_ativo == true || (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto))
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                     && (l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTEIRA)
                                     && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                     && l.id_local_ativo == true
                                     && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                       || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    //Todos locais de movimento que não sejam carteira e não sejam bancos de cliente.
                    case TipoConsultaLocalMovto.HAS_SIMULACAO_BAIXA:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola &&
                                         ((l.id_local_ativo == true &&
                                          ((l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO && l.cd_pessoa_local == null) ||
                                          l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA ||
                                          l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO || 
                                          l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO)) ||
                                          (l.cd_local_movto == cd_loc_mvto)
                                          )
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                         && ((l.id_local_ativo == true
                                         && ((l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO && l.cd_pessoa_local == null)
                                           || ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null) && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                           || l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO 
                                           || l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO))
                                           || (l.cd_local_movto == cd_loc_mvto))
                                   orderby l.no_local_movto
                                   select new
                                  {
                                      cd_local_movto = l.cd_local_movto,
                                      nm_banco = l.no_local_movto,
                                      nm_agencia = l.nm_agencia,
                                      nm_conta_corrente = l.nm_conta_corrente,
                                      nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                      nm_tipo_local = l.nm_tipo_local
                                  }).ToList().Select(x => new LocalMovto
                                  {
                                      cd_local_movto = x.cd_local_movto,
                                      nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                      x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                  }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_FILTRO_CNAB:
                        sql = (from l in db.LocalMovto
                               where l.cd_pessoa_empresa == cdEscola
                               where (l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                               orderby l.no_local_movto
                               select new
                               {
                                   cd_local_movto = l.cd_local_movto,
                                   nm_banco = l.no_local_movto,
                                   nm_agencia = l.nm_agencia,
                                   nm_conta_corrente = l.nm_conta_corrente,
                                   nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                   nm_tipo_local = l.nm_tipo_local
                               }).ToList().Select(x => new LocalMovto
                               {
                                   cd_local_movto = x.cd_local_movto,
                                   nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                   x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                               }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_SIMULACAO_BAIXA_GERAL_SEM_CARTAO:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola &&
                                         ((l.id_local_ativo == true &&
                                           l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTEIRA &&
                                          ((l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO && l.cd_pessoa_local == null) ||
                                          l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA /*||
                                          l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                          l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO*/)) ||
                                          (l.cd_local_movto == cd_loc_mvto)
                                          )
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola &&
                                         ((l.id_local_ativo == true &&
                                           l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTEIRA &&
                                          ((l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO && l.cd_pessoa_local == null) ||
                                           ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                               && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                              || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA))) ||
                                          (l.cd_local_movto == cd_loc_mvto)
                                          )
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();

                        break;

                }
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<LocalMovto> getLocalMovimentoComFiltrosTrocaFinanceira(int cdEscola, int cd_loc_mvto, int cd_tipo_financeiro, TipoConsultaLocalMovto tipoConsulta, int cd_pessoa_usuario)
        {
            try
            {
                bool isMaster = cd_pessoa_usuario <= 0;
                IEnumerable<LocalMovto> sql = null;
                switch (tipoConsulta)
                {
                    case TipoConsultaLocalMovto.HAS_ATIVO:
                        if (isMaster)
                            sql = from l in db.LocalMovto
                                  where l.cd_pessoa_empresa == cdEscola
                                     && l.id_local_ativo == true
                                     && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                  select l;
                        else
                            sql = from l in db.LocalMovto
                                  where l.cd_pessoa_empresa == cdEscola
                                     && l.id_local_ativo == true
                                     && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                     && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                      && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                      || l.nm_tipo_local != (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)

                                  select l;
                        break;
                    case TipoConsultaLocalMovto.HAS_ATIVO_E_BANCOS:
                        if (isMaster)
                        {
                            switch (cd_tipo_financeiro)
                            {
                                case (int)TipoFinanceiro.TiposFinanceiro.CARTAO:
                                    sql = (from local in db.LocalMovto.Include("TaxaBancaria")
                                           where local.cd_pessoa_empresa == cdEscola &&
                                                 ((local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                                  local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                 local.id_local_ativo == true &&
                                                 (cd_loc_mvto == 0 || local.cd_local_movto == cd_loc_mvto)) 
                                           select new
                                           {
                                               cd_local_movto = local.cd_local_movto,
                                               no_local_movto = local.no_local_movto,
                                               nm_agencia = local.nm_agencia,
                                               nm_conta_corrente = local.nm_conta_corrente,
                                               nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                                               nm_tipo_local = local.nm_tipo_local,
                                               cd_pessoa_empresa = local.cd_pessoa_empresa,
                                               T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                                               T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                                               cd_local_banco = local.cd_local_banco,
                                               taxaBancaria = local.TaxaBancaria
                                           }).ToList().Select(x => new LocalMovto
                                           {
                                               cd_local_movto = x.cd_local_movto,
                                               no_local_movto = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                                               nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                                               nm_tipo_local = x.nm_tipo_local,
                                               cd_pessoa_empresa = x.cd_pessoa_empresa,
                                               cd_local_banco = x.cd_local_banco,
                                               T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                                               T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,
                                               TaxaBancaria = x.taxaBancaria
                                           }).ToList();
                                    break;
                                case (int)TipoFinanceiro.TiposFinanceiro.CHEQUE:
                                        sql = (from l in db.LocalMovto
                                            where l.cd_pessoa_empresa == cdEscola
                                                  && l.id_local_ativo == true
                                                  && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                                  && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO
                                            orderby l.no_local_movto
                                            select new
                                            {
                                                cd_local_movto = l.cd_local_movto,
                                                nm_banco = l.no_local_movto,
                                                nm_agencia = l.nm_agencia,
                                                nm_conta_corrente = l.nm_conta_corrente,
                                                nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                                nm_tipo_local = l.nm_tipo_local
                                            }).ToList().Select(x => new LocalMovto
                                        {
                                            cd_local_movto = x.cd_local_movto,
                                            nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                                x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                        }).ToList();
                                    break;
                                 default:
                                    sql = (from l in db.LocalMovto
                                        where l.cd_pessoa_empresa == cdEscola
                                              && !(l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                                   l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) 
                                              && l.id_local_ativo == true
                                              && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                        orderby l.no_local_movto
                                        select new
                                        {
                                            cd_local_movto = l.cd_local_movto,
                                            nm_banco = l.no_local_movto,
                                            nm_agencia = l.nm_agencia,
                                            nm_conta_corrente = l.nm_conta_corrente,
                                            nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                            nm_tipo_local = l.nm_tipo_local
                                        }).ToList().Select(x => new LocalMovto
                                    {
                                        cd_local_movto = x.cd_local_movto,
                                        nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                            x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                    }).ToList();
                                    break;
                            }
                        }
                        else
                        {
                            switch (cd_tipo_financeiro)
                            {
                                case (int)TipoFinanceiro.TiposFinanceiro.CARTAO:
                                    sql = (from local in db.LocalMovto.Include("TaxaBancaria")
                                           where local.cd_pessoa_empresa == cdEscola &&
                                                 ((local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                                  local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                 local.id_local_ativo == true &&
                                                 (cd_loc_mvto == 0 || local.cd_local_movto == cd_loc_mvto) 
                                                 )
                                           select new
                                           {
                                               cd_local_movto = local.cd_local_movto,
                                               no_local_movto = local.no_local_movto,
                                               nm_agencia = local.nm_agencia,
                                               nm_conta_corrente = local.nm_conta_corrente,
                                               nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                                               nm_tipo_local = local.nm_tipo_local,
                                               cd_pessoa_empresa = local.cd_pessoa_empresa,
                                               T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                                               T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                                               cd_local_banco = local.cd_local_banco,
                                               taxaBancaria = local.TaxaBancaria
                                           }).ToList().Select(x => new LocalMovto
                                           {
                                               cd_local_movto = x.cd_local_movto,
                                               no_local_movto = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                                               nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                                               nm_tipo_local = x.nm_tipo_local,
                                               cd_pessoa_empresa = x.cd_pessoa_empresa,
                                               cd_local_banco = x.cd_local_banco,
                                               T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                                               T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,
                                               TaxaBancaria = x.taxaBancaria
                                           }).ToList();
                                    break;
                                case (int)TipoFinanceiro.TiposFinanceiro.CHEQUE:
                                    sql = (from l in db.LocalMovto
                                           where l.cd_pessoa_empresa == cdEscola
                                                 && l.id_local_ativo == true
                                                 && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                                 &&  ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                                 && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO) || 
                                                    l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO)
                                           orderby l.no_local_movto
                                           select new
                                           {
                                               cd_local_movto = l.cd_local_movto,
                                               nm_banco = l.no_local_movto,
                                               nm_agencia = l.nm_agencia,
                                               nm_conta_corrente = l.nm_conta_corrente,
                                               nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                               nm_tipo_local = l.nm_tipo_local
                                           }).ToList().Select(x => new LocalMovto
                                           {
                                               cd_local_movto = x.cd_local_movto,
                                               nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                                   x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                           }).ToList();

                                    break;
                                    default:
                                        sql = (from l in db.LocalMovto
                                            where l.cd_pessoa_empresa == cdEscola
                                                  && l.id_local_ativo == true
                                                  && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                                  && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                                      || (l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA &&
                                                            !(l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                                            l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO)))
                                            orderby l.no_local_movto
                                            select new
                                            {
                                                cd_local_movto = l.cd_local_movto,
                                                nm_banco = l.no_local_movto,
                                                nm_agencia = l.nm_agencia,
                                                nm_conta_corrente = l.nm_conta_corrente,
                                                nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                                nm_tipo_local = l.nm_tipo_local
                                            }).ToList().Select(x => new LocalMovto
                                        {
                                            cd_local_movto = x.cd_local_movto,
                                            nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                                x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                        }).ToList();
                                    break;
                            }
                        }
                        break;
                    case TipoConsultaLocalMovto.HAS_TITULOS_NATUREZA_RECEBER:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.Titulos.Where(t => t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER).Any()
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.Titulos.Where(t => t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER).Any()
                                      && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null)
                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA
                                       || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_BAIXA_NATUREZA_RECEBER:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                    && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                    && l.BaixaTitulo.Where(bt => bt.cd_local_movto == l.cd_local_movto
                                    && bt.Titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER).Any()
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                    && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                    && l.BaixaTitulo.Where(bt => bt.cd_local_movto == l.cd_local_movto
                                    && bt.Titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER).Any()
                                    && (((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null)
                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                       || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_TITULOS_NATUREZA_PAGAR:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.Titulos.Where(t => t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.PAGAR).Any()
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.Titulos.Where(t => t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.PAGAR).Any()
                                      && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                       || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_BAIXA_NATUREZA_PAGAR:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                     && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                     && l.BaixaTitulo.Where(bt => bt.cd_local_movto == l.cd_local_movto
                                     && bt.Titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.PAGAR).Any()
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                     && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                     && l.BaixaTitulo.Where(bt => bt.cd_local_movto == l.cd_local_movto
                                                            && bt.Titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.PAGAR).Any()
                                     && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                       || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_TODOS_TITULOS:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.Titulos.Any()
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.Titulos.Any()
                                      && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                        && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                        || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_TODAS_BAIXAS:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.BaixaTitulo.Any()
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                      && l.BaixaTitulo.Any()
                                      && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                       || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_ATIVO_BAIXA_TITULO:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && l.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTEIRA
                                      || (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                      && l.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTEIRA
                                      && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                       && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                       || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_ATIVO_ESCOLA_SEM_CARTEIRA:
                       if (isMaster)
                        {
                            switch (cd_tipo_financeiro)
                            {
                                case (int)TipoFinanceiro.TiposFinanceiro.CARTAO:
                                    sql = (from local in db.LocalMovto.Include("TaxaBancaria")
                                           where local.cd_pessoa_empresa == cdEscola &&
                                                 ((local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                                  local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                 local.id_local_ativo == true &&
                                                 (cd_loc_mvto == 0 || local.cd_local_movto == cd_loc_mvto))
                                           select new
                                           {
                                               cd_local_movto = local.cd_local_movto,
                                               no_local_movto = local.no_local_movto,
                                               nm_agencia = local.nm_agencia,
                                               nm_conta_corrente = local.nm_conta_corrente,
                                               nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                                               nm_tipo_local = local.nm_tipo_local,
                                               cd_pessoa_empresa = local.cd_pessoa_empresa,
                                               T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                                               T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                                               cd_local_banco = local.cd_local_banco,
                                               taxaBancaria = local.TaxaBancaria
                                           }).ToList().Select(x => new LocalMovto
                                           {
                                               cd_local_movto = x.cd_local_movto,
                                               no_local_movto = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                                               nm_tipo_local = x.nm_tipo_local,
                                               cd_pessoa_empresa = x.cd_pessoa_empresa,
                                               cd_local_banco = x.cd_local_banco,
                                               T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                                               T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,
                                               TaxaBancaria = x.taxaBancaria
                                           }).ToList().Union((from local in db.LocalMovto.Include("TaxaBancaria")
                                                              where local.cd_pessoa_empresa == cdEscola &&
                                                                    ((local.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO &&
                                                                     local.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                                     local.id_local_ativo == true &&
                                                                  (cd_loc_mvto == 0 || local.cd_local_movto == cd_loc_mvto) && 
                                                                     (from l in db.LocalMovto
                                                                      where l.cd_local_banco == local.cd_local_movto &&
                                                                            l.cd_pessoa_empresa == local.cd_pessoa_empresa &&
                                                                            (l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                                                             l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                                            l.id_local_ativo == true
                                                                      select local).Any())

                                                              select new
                                                              {
                                                                  cd_local_movto = local.cd_local_movto,
                                                                  no_local_movto = local.no_local_movto,
                                                                  nm_tipo_local = local.nm_tipo_local,
                                                                  nm_agencia = local.nm_agencia,
                                                                  nm_conta_corrente = local.nm_conta_corrente,
                                                                  nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                                                                  cd_pessoa_empresa = local.cd_pessoa_empresa,
                                                                  T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                                                                  T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                                                                  cd_local_banco = local.cd_local_banco,

                                                                  taxaBancaria = local.TaxaBancaria
                                                              }).ToList().Select(x => new LocalMovto
                                                              {
                                                                  cd_local_movto = x.cd_local_movto,
                                                                  no_local_movto = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                                                                  nm_tipo_local = x.nm_tipo_local,
                                                                  cd_pessoa_empresa = x.cd_pessoa_empresa,
                                                                  cd_local_banco = x.cd_local_banco,
                                                                  T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                                                                  T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,

                                                                  TaxaBancaria = x.taxaBancaria
                                                              }).ToList());
                                    break;
                                case (int)TipoFinanceiro.TiposFinanceiro.CHEQUE:
                                        sql = (from l in db.LocalMovto
                                            where l.cd_pessoa_empresa == cdEscola
                                                  && l.id_local_ativo == true
                                                  && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                                  && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO
                                            orderby l.no_local_movto
                                            select new
                                            {
                                                cd_local_movto = l.cd_local_movto,
                                                nm_banco = l.no_local_movto,
                                                nm_agencia = l.nm_agencia,
                                                nm_conta_corrente = l.nm_conta_corrente,
                                                nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                                nm_tipo_local = l.nm_tipo_local
                                            }).ToList().Select(x => new LocalMovto
                                        {
                                            cd_local_movto = x.cd_local_movto,
                                            nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                                x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                        }).ToList();

                                    break;
                                 default:
                                    sql = (from l in db.LocalMovto
                                        where l.cd_pessoa_empresa == cdEscola
                                              && l.id_local_ativo == true
                                              && (l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTEIRA)
                                              && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                        orderby l.no_local_movto
                                        select new
                                        {
                                            cd_local_movto = l.cd_local_movto,
                                            nm_banco = l.no_local_movto,
                                            nm_agencia = l.nm_agencia,
                                            nm_conta_corrente = l.nm_conta_corrente,
                                            nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                            nm_tipo_local = l.nm_tipo_local
                                        }).ToList().Select(x => new LocalMovto
                                    {
                                        cd_local_movto = x.cd_local_movto,
                                        nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                            x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                    }).ToList();
                                    break;
                            }
                            
                            
                        }
                        else
                        {


                            switch (cd_tipo_financeiro)
                            {
                                case (int)TipoFinanceiro.TiposFinanceiro.CARTAO:
                                    sql = (from local in db.LocalMovto.Include("TaxaBancaria")
                                           where local.cd_pessoa_empresa == cdEscola &&
                                              ((local.cd_pessoa_local == cd_pessoa_usuario || local.cd_pessoa_local == null) &&
                                                 ((local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                                  local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                 local.id_local_ativo == true &&
                                                 (cd_loc_mvto == 0 || local.cd_local_movto == cd_loc_mvto) &&
                                                 !(from l in db.LocalMovto
                                                   where l.cd_local_banco == local.cd_local_movto &&
                                                         l.cd_pessoa_empresa == local.cd_pessoa_empresa &&
                                                         (l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                                          l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                         l.id_local_ativo == true
                                                   select local).Any()) || ((local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                                                             local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                                            local.id_local_ativo == true &&
                                                                            (cd_loc_mvto == 0 || local.cd_local_movto == cd_loc_mvto) &&
                                                                            !(from l in db.LocalMovto
                                                                                where l.cd_local_banco == local.cd_local_movto &&
                                                                                      l.cd_pessoa_empresa == local.cd_pessoa_empresa &&
                                                                                      (l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                                                                       l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                                                      l.id_local_ativo == true
                                                                                select local).Any()))
                                           select new
                                           {
                                               cd_local_movto = local.cd_local_movto,
                                               no_local_movto = local.no_local_movto,
                                               nm_agencia = local.nm_agencia,
                                               nm_conta_corrente = local.nm_conta_corrente,
                                               nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                                               nm_tipo_local = local.nm_tipo_local,
                                               cd_pessoa_empresa = local.cd_pessoa_empresa,
                                               T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                                               T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                                               cd_local_banco = local.cd_local_banco,

                                               taxaBancaria = local.TaxaBancaria
                                           }).ToList().Select(x => new LocalMovto
                                           {
                                               cd_local_movto = x.cd_local_movto,
                                               no_local_movto = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                                               nm_tipo_local = x.nm_tipo_local,
                                               cd_pessoa_empresa = x.cd_pessoa_empresa,
                                               cd_local_banco = x.cd_local_banco,
                                               T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                                               T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,

                                               TaxaBancaria = x.taxaBancaria
                                           }).ToList().Union((from local in db.LocalMovto.Include("TaxaBancaria")
                                                              where local.cd_pessoa_empresa == cdEscola &&
                                                                     ((local.cd_pessoa_local == cd_pessoa_usuario || local.cd_pessoa_local == null) &&
                                                                    ((local.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO &&
                                                                     local.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                                     local.id_local_ativo == true &&
                                                                  (cd_loc_mvto == 0 || local.cd_local_movto == cd_loc_mvto) &&
                                                                     (from l in db.LocalMovto
                                                                      where l.cd_local_banco == local.cd_local_movto &&
                                                                            l.cd_pessoa_empresa == local.cd_pessoa_empresa &&
                                                                            (l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                                                             l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                                            l.id_local_ativo == true
                                                                      select local).Any()) || ((local.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO &&
                                                                                                local.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                                                               local.id_local_ativo == true &&
                                                                                               (cd_loc_mvto == 0 || local.cd_local_movto == cd_loc_mvto) &&
                                                                                               (from l in db.LocalMovto
                                                                                                   where l.cd_local_banco == local.cd_local_movto &&
                                                                                                         l.cd_pessoa_empresa == local.cd_pessoa_empresa &&
                                                                                                         (l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                                                                                          l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                                                                         l.id_local_ativo == true
                                                                                                   select local).Any()))

                                                              select new
                                                              {
                                                                  cd_local_movto = local.cd_local_movto,
                                                                  no_local_movto = local.no_local_movto,
                                                                  nm_tipo_local = local.nm_tipo_local,
                                                                  nm_agencia = local.nm_agencia,
                                                                  nm_conta_corrente = local.nm_conta_corrente,
                                                                  nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                                                                  cd_pessoa_empresa = local.cd_pessoa_empresa,
                                                                  T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                                                                  T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                                                                  cd_local_banco = local.cd_local_banco,

                                                                  taxaBancaria = local.TaxaBancaria
                                                              }).ToList().Select(x => new LocalMovto
                                                              {
                                                                  cd_local_movto = x.cd_local_movto,
                                                                  no_local_movto = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                                                                  nm_tipo_local = x.nm_tipo_local,
                                                                  cd_pessoa_empresa = x.cd_pessoa_empresa,
                                                                  cd_local_banco = x.cd_local_banco,
                                                                  T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                                                                  T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,

                                                                  TaxaBancaria = x.taxaBancaria
                                                              }).ToList());
                                    break;
                                case (int)TipoFinanceiro.TiposFinanceiro.CHEQUE:
                                    sql = (from l in db.LocalMovto
                                           where l.cd_pessoa_empresa == cdEscola
                                                 && l.id_local_ativo == true
                                                 && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                                 &&  ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                                 && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO) || 
                                                    l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO)
                                           orderby l.no_local_movto
                                           select new
                                           {
                                               cd_local_movto = l.cd_local_movto,
                                               nm_banco = l.no_local_movto,
                                               nm_agencia = l.nm_agencia,
                                               nm_conta_corrente = l.nm_conta_corrente,
                                               nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                               nm_tipo_local = l.nm_tipo_local
                                           }).ToList().Select(x => new LocalMovto
                                           {
                                               cd_local_movto = x.cd_local_movto,
                                               nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                                   x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                           }).ToList();

                                    break;
                                    default:
                                        sql = (from l in db.LocalMovto
                                            where l.cd_pessoa_empresa == cdEscola
                                                  && l.id_local_ativo == true
                                                  && (cd_loc_mvto == 0 || l.cd_local_movto == cd_loc_mvto)
                                                  && (l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTEIRA)
                                                  && ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                                       && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                                      || l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                            orderby l.no_local_movto
                                            select new
                                            {
                                                cd_local_movto = l.cd_local_movto,
                                                nm_banco = l.no_local_movto,
                                                nm_agencia = l.nm_agencia,
                                                nm_conta_corrente = l.nm_conta_corrente,
                                                nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                                nm_tipo_local = l.nm_tipo_local
                                            }).ToList().Select(x => new LocalMovto
                                        {
                                            cd_local_movto = x.cd_local_movto,
                                            nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                                x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                        }).ToList();
                                    break;
                            }


                            
                        }
                        break;
                    //Todos locais de movimento que não sejam carteira e não sejam bancos de cliente.
                    case TipoConsultaLocalMovto.HAS_SIMULACAO_BAIXA:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola &&
                                         ((l.id_local_ativo == true &&
                                          ((l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO && l.cd_pessoa_local == null) ||
                                          l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA ||
                                          l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                          l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO)) ||
                                          (l.cd_local_movto == cd_loc_mvto)
                                          )
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        else
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola
                                         && ((l.id_local_ativo == true
                                         && ((l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO && l.cd_pessoa_local == null)
                                           || ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null) && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                           || l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO
                                           || l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO))
                                           || (l.cd_local_movto == cd_loc_mvto))
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_FILTRO_CNAB:
                        sql = (from l in db.LocalMovto
                               where l.cd_pessoa_empresa == cdEscola
                               where (l.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                               orderby l.no_local_movto
                               select new
                               {
                                   cd_local_movto = l.cd_local_movto,
                                   nm_banco = l.no_local_movto,
                                   nm_agencia = l.nm_agencia,
                                   nm_conta_corrente = l.nm_conta_corrente,
                                   nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                   nm_tipo_local = l.nm_tipo_local
                               }).ToList().Select(x => new LocalMovto
                               {
                                   cd_local_movto = x.cd_local_movto,
                                   nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                   x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                               }).ToList();
                        break;
                    case TipoConsultaLocalMovto.HAS_SIMULACAO_BAIXA_GERAL_SEM_CARTAO:
                        if (isMaster)
                            sql = (from l in db.LocalMovto
                                   where l.cd_pessoa_empresa == cdEscola &&
                                         ((l.id_local_ativo == true &&
                                          ((l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO && l.cd_pessoa_local == null) ||
                                          l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA /*||
                                          l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                          l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO*/)) ||
                                          (l.cd_local_movto == cd_loc_mvto)
                                          )
                                   orderby l.no_local_movto
                                   select new
                                   {
                                       cd_local_movto = l.cd_local_movto,
                                       nm_banco = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local
                                   }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                       x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                   }).ToList();
                        break;
                    

                }
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<LocalMovto> getLocalMovtoBaixa(int cd_escola, int? cd_loc_mvto, int natureza, int[] listPessoas, int cd_pessoa_usuario)
        {
            try
            {
                bool isMaster = cd_pessoa_usuario <= 0;

                IEnumerable<LocalMovto> sql = (from l in db.LocalMovto
                                               where l.cd_pessoa_empresa == cd_escola
                                                 || (!isMaster
                                                 && l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null
                                                 && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                               select new
                                               {
                                                   cd_local_movto = l.cd_local_movto,
                                                   nm_banco = l.no_local_movto,
                                                   nm_agencia = l.nm_agencia,
                                                   nm_conta_corrente = l.nm_conta_corrente,
                                                   nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                                   nm_tipo_local = l.nm_tipo_local
                                               }).ToList().Select(x => new LocalMovto
                                               {
                                                   cd_local_movto = x.cd_local_movto,
                                                   nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                                   x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                                               }).ToList();
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalMovto> getAllLocalMovto(int cdEscola, bool isOrigem, int cd_pessoa_usuario)
        {
            try
            {
                IEnumerable<LocalMovto> sql;
                if (cd_pessoa_usuario <= 0)
                {
                    if (isOrigem)
                        sql = from local in db.LocalMovto
                              where local.ContaCorrenteOrigem.Any(l => l.cd_local_origem == local.cd_local_movto)
                              select local;
                    else
                        sql = from local in db.LocalMovto
                              where local.ContaCorrenteDestino.Any(l => l.cd_local_destino == local.cd_local_movto)
                              select local;
                }
                else
                {
                    if (isOrigem)
                        sql = from local in db.LocalMovto
                              where local.ContaCorrenteOrigem.Any(l => l.cd_local_origem == local.cd_local_movto)
                                && ((local.cd_pessoa_local == cd_pessoa_usuario || local.cd_pessoa_local == null
                                       && local.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                       || local.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                              select local;
                    else
                        sql = from local in db.LocalMovto
                              where local.ContaCorrenteDestino.Any(l => l.cd_local_destino == local.cd_local_movto)
                               && ((local.cd_pessoa_local == cd_pessoa_usuario || local.cd_pessoa_local == null
                                       && local.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                                       || local.nm_tipo_local != (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)
                              select local;
                }

                sql = (from local in sql
                       where local.cd_pessoa_empresa == cdEscola
                       select new
                       {
                           cd_local_movto = local.cd_local_movto,
                           nm_banco = local.no_local_movto,
                           nm_agencia = local.nm_agencia,
                           nm_conta_corrente = local.nm_conta_corrente,
                           nm_digito_conta_corrente = local.nm_digito_conta_corrente != null ? local.nm_digito_conta_corrente : null,
                           nm_tipo_local = local.nm_tipo_local
                       }).ToList().Select(x => new LocalMovto
                       {
                           cd_local_movto = x.cd_local_movto,
                           nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                           x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                       }).ToList();

                return sql.OrderBy(x => x.nomeLocal);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalMovto> getLocalMovtoAtivosWithConta(int cdEscola, bool isOrigem, bool isCadastrar, int cdLocalMovto)
        {
            try
            {
                IEnumerable<LocalMovto> sql;

                if (isOrigem)//seleciona so o locais que são origem
                {
                    if (!isCadastrar)
                    {//seleciona so o locais que são ativos ou que possuem conta corrente na escola logada (valido apenas para a edição)
                        sql = from local in db.LocalMovto
                              where local.cd_pessoa_empresa == cdEscola
                                  && (local.ContaCorrenteOrigem.Any(l => l.cd_local_origem == local.cd_local_movto)
                                  || (local.id_local_ativo == true))
                              select local;
                    }
                    else
                    { // retonar só os locais que são ativos da escola logada do usuário
                        sql = from local in db.LocalMovto
                              where local.cd_pessoa_empresa == cdEscola
                                    && (((local.id_local_ativo == true)
                                    && (local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                                    || (local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.BANCO && local.cd_pessoa_local == null)
                                    || local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO
                                    || local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO))
                                    || local.cd_local_movto == cdLocalMovto)
                              select local;
                    }
                }
                else
                {
                    if (!isCadastrar)
                    {//seleciona so o locais que são ativos ou que possuem conta corrente na escola logada (valido apenas para a edição)
                        sql = from local in db.LocalMovto
                              where local.cd_pessoa_empresa == cdEscola
                                  && (local.ContaCorrenteDestino.Any(l => l.cd_local_destino == local.cd_local_movto)
                                   || local.id_local_ativo == true)
                              select local;
                    }
                    else
                    {
                        sql = from local in db.LocalMovto
                              where local.cd_pessoa_empresa == cdEscola
                                 && (((local.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTEIRA
                                  || local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                                  || local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.BANCO)
                                 && (local.id_local_ativo == true))
                                 || local.cd_local_movto == cdLocalMovto)
                              select local;
                    }
                }

                sql = (from local in sql
                       select new
                       {
                           cd_local_movto = local.cd_local_movto,
                           nm_banco = local.no_local_movto,
                           nm_agencia = local.nm_agencia,
                           nm_conta_corrente = local.nm_conta_corrente,
                           nm_digito_conta_corrente = local.nm_digito_conta_corrente != null ? local.nm_digito_conta_corrente : null,
                           nm_tipo_local = local.nm_tipo_local
                       }).ToList().Select(x => new LocalMovto
                       {
                           cd_local_movto = x.cd_local_movto,
                           nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                           x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                       }).ToList();

                return sql != null && sql.Count() > 0 ? sql.OrderBy(x => x.nomeLocal) : sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalMovto> getLocalMovtoAtivosWithContaUsuario(int cdEscola, bool isOrigem, int cdLocalMovto, int cd_pessoa_usuario)
        {
            try
            {
                IEnumerable<LocalMovto> sql;
                if (isOrigem)//seleciona so o locais que são origem
                {
                    sql = from local in db.LocalMovto
                          where local.cd_pessoa_empresa == cdEscola
                                && (((local.id_local_ativo == true)
                                && ((local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA && (local.cd_pessoa_local == cd_pessoa_usuario || local.cd_pessoa_local == null))
                                || (local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.BANCO && local.cd_pessoa_local == null)))
                                || local.cd_local_movto == cdLocalMovto)
                          select local;
                }
                else
                {
                    sql = from local in db.LocalMovto
                          where local.cd_pessoa_empresa == cdEscola
                             && ((local.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTEIRA
                             && (local.id_local_ativo == true))
                             || local.cd_local_movto == cdLocalMovto)
                          select local;
                }
                sql = (from local in sql
                       select new
                       {
                           cd_local_movto = local.cd_local_movto,
                           nm_banco = local.no_local_movto,
                           nm_agencia = local.nm_agencia,
                           nm_conta_corrente = local.nm_conta_corrente,
                           nm_digito_conta_corrente = local.nm_digito_conta_corrente != null ? local.nm_digito_conta_corrente : null,
                           nm_tipo_local = local.nm_tipo_local
                       }).ToList().Select(x => new LocalMovto
                       {
                           cd_local_movto = x.cd_local_movto,
                           nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                           x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                       }).ToList();

                return sql != null && sql.Count() > 0 ? sql.OrderBy(x => x.nomeLocal) : sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalMovto> getLocalMovtoAtivosWithCodigo(int cdEscola, bool isOrigem, int cd_local)
        {
            try
            {
                IEnumerable<LocalMovto> sql;

                if (isOrigem)//seleciona so o locais que são origem
                    sql = from local in db.LocalMovto
                          where (local.cd_pessoa_empresa == cdEscola)
                            && (!local.cd_pessoa_local.HasValue)
                            && (local.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTEIRA
                             || local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                             || local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.BANCO)
                            && ((local.id_local_ativo == true)
                               || local.cd_local_movto == cd_local)
                          select local;
                else
                    sql = from local in db.LocalMovto
                          where local.cd_pessoa_empresa == cdEscola
                             && (!local.cd_pessoa_local.HasValue)
                             && (local.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTEIRA
                              || local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                              || local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.BANCO)
                             && ((local.id_local_ativo == true)
                              || local.cd_local_movto == cd_local)
                          select local;

                sql = (from local in sql
                       select new
                       {
                           cd_local_movto = local.cd_local_movto,
                           nm_banco = local.no_local_movto,
                           nm_agencia = local.nm_agencia,
                           nm_conta_corrente = local.nm_conta_corrente,
                           nm_digito_conta_corrente = local.nm_digito_conta_corrente != null ? local.nm_digito_conta_corrente : null,
                           nm_tipo_local = local.nm_tipo_local
                       }).ToList().Select(x => new LocalMovto
                       {
                           cd_local_movto = x.cd_local_movto,
                           nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                           x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                       }).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalMovimentoWithContaUI> getLocalMovtoWithContaByEscola(int cdEscola, int cd_pessoa_usuario)
        {
            try// escola = 2 usuario = 74
            {
                IEnumerable<LocalMovto> sql;
                bool isMaster = cd_pessoa_usuario <= 0;

                if (!isMaster)
                    sql = from local in db.LocalMovto
                          where local.cd_pessoa_empresa == cdEscola &&
                          (((local.ContaCorrenteOrigem.Any(l => l.cd_local_origem == local.cd_local_movto)) ||
                           (local.ContaCorrenteDestino.Any(l => l.cd_local_destino == local.cd_local_movto))
                          ) &&
                            (((local.cd_pessoa_local == cd_pessoa_usuario || local.cd_pessoa_local == null && local.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA) ||
                              local.nm_tipo_local != (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)))
                          orderby local.no_local_movto
                          select local;
                else
                    sql = (from local in db.LocalMovto
                          where local.cd_pessoa_empresa == cdEscola && ((local.ContaCorrenteOrigem.Any()) || (local.ContaCorrenteDestino.Any()))
                          //orderby local.no_local_movto
                          select local).ToList();

              var  sql2 = (from local in sql
                       select new
                       {
                           cd_local_movto = local.cd_local_movto,
                           nm_banco = local.no_local_movto,
                           nm_agencia = local.nm_agencia,
                           nm_conta_corrente = local.nm_conta_corrente,
                           nm_digito_conta_corrente = local.nm_digito_conta_corrente ?? null,
                           nm_tipo_local = local.nm_tipo_local
                       }).ToList().Select(x => new LocalMovimentoWithContaUI
                       {
                           cd_local_movto = x.cd_local_movto,
                           nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                           x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                       }).ToList().OrderBy(x => x.nomeLocal).Distinct();

                return sql2;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalMovto> getLocalMovto(int cdEscola, int cdLocalMovto)
        {
            try
            {
                var sql = from local in db.LocalMovto
                          where local.cd_pessoa_empresa == cdEscola &&
                            local.cd_local_movto == cdLocalMovto
                          select local;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public LocalMovto getLocalMovimentoWithPessoaBanco(int cd_local_movto)
        {
            try
            {
                var sql = (from local in db.LocalMovto.Include(l => l.PessoaSGFBanco).Include(l => l.Banco)
                           where local.cd_local_movto == cd_local_movto
                           select local).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaCarteiraCnab(int cdCarteira)
        {
            try
            {
                // Verifica se  está usando a carteira em algum CNAB
                Cnab sql = (from c in db.Cnab
                          join l in db.LocalMovto
                          on c.cd_local_movto equals l.cd_local_movto
                          where l.cd_carteira_cnab == cdCarteira
                          select c).FirstOrDefault();
                return (sql != null && sql.cd_cnab > 0);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaLocalTituloTemCNAB(int cd_local_movto, int cd_pessoa_empresa)
        {
            try
            {
                // Verifica se  está usando a carteira em algum CNAB
                CarteiraCnab sql = (from l in db.LocalMovto
                                  join c in db.CarteiraCnab
                                  on l.cd_carteira_cnab equals c.cd_carteira_cnab
                                  where l.cd_local_movto == cd_local_movto && l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.BANCO
                                  select c).FirstOrDefault();
                return (sql != null && sql.cd_carteira_cnab > 0);

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public LocalMovtoUI getLocalByTitulo(int cdEscola, int cd_local_movto)
        {

            var sql = (from l in db.LocalMovto
                       where l.cd_pessoa_empresa == cdEscola &&
                         l.cd_local_movto == cd_local_movto
                       select new
                       {
                           cd_local_movto = l.cd_local_movto,
                           nm_banco = l.no_local_movto,
                           nm_agencia = l.nm_agencia,
                           nm_conta_corrente = l.nm_conta_corrente,
                           nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                           nm_tipo_local = l.nm_tipo_local,
                           taxaBancaria = l.TaxaBancaria
                       }).ToList().Select(x => new LocalMovtoUI
                              {
                                  cd_local_movto = x.cd_local_movto,
                                  nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                                  x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco,
                                  nm_tipo_local = x.nm_tipo_local,
                                  taxaBancaria = x.taxaBancaria,
                                  no_local_movto = x.nm_banco
                              }).FirstOrDefault();
            return sql;
        }

        public IEnumerable<LocalMovto> getLocalMovtoProspect(int cdEscola, int cd_loc_mvto, int cd_pessoa_usuario)
        {
            IEnumerable<LocalMovto> sql = null;
            if (cd_pessoa_usuario == 0)
                sql = (from l in db.LocalMovto
                       where l.cd_pessoa_empresa == cdEscola &&
                             ((l.id_local_ativo == true &&
                              ((l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO && l.cd_pessoa_local == null) ||
                              (l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA))) ||
                              (l.cd_local_movto == cd_loc_mvto))
                       orderby l.no_local_movto
                       select new
                       {
                           cd_local_movto = l.cd_local_movto,
                           nm_banco = l.no_local_movto,
                           nm_agencia = l.nm_agencia,
                           nm_conta_corrente = l.nm_conta_corrente,
                           nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                           nm_tipo_local = l.nm_tipo_local
                       }).ToList().Select(x => new LocalMovto
                       {
                           cd_local_movto = x.cd_local_movto,
                           nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                           x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                       }).ToList();
            else
                sql = (from l in db.LocalMovto
                       where l.cd_pessoa_empresa == cdEscola
                             && ((l.id_local_ativo == true
                             && ((l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO)
                               || ((l.cd_pessoa_local == cd_pessoa_usuario || l.cd_pessoa_local == null) && l.nm_tipo_local == (int)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA)))
                               || (l.cd_local_movto == cd_loc_mvto))
                            || (from prospect in db.Prospect 
                                where prospect.cd_local_movimento == l.cd_local_movto 
                                   && prospect.cd_pessoa_escola == cdEscola
                                select prospect.cd_local_movimento).Any()
                       orderby l.no_local_movto
                       select new
                      {
                          cd_local_movto = l.cd_local_movto,
                          nm_banco = l.no_local_movto,
                          nm_agencia = l.nm_agencia,
                          nm_conta_corrente = l.nm_conta_corrente,
                          nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                          nm_tipo_local = l.nm_tipo_local
                      }).ToList().Select(x => new LocalMovto
                      {
                          cd_local_movto = x.cd_local_movto,
                          nomeLocal = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ?
                          x.nm_banco + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.nm_banco
                      }).ToList();
            return sql;
        }


        public IEnumerable<LocalMovto> getAllLocalMovtoCartao(int cdEscola)
        {
            try
            {
                IEnumerable<LocalMovto> sql;
                int cd_aux = cdEscola;  //Vai passar valor negativo para mostrar somente conas de cartão filhas.
                cdEscola = Math.Abs(cdEscola);
                // Conta Cartão sem Banco vinculado
                sql = (from local in db.LocalMovto.Include("TaxaBancaria")
                       where local.cd_pessoa_empresa == cdEscola &&
                             ((local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                              local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                             local.id_local_ativo == true

                                  //Alteração Chamado: 360529 -> 05/04/2023 (mudança para vir locais com banco vinculado)
                                  //Leo estou com um problema de trocar parcelas para cartão baixa automática – acontece que o local de movimento de crédito tem banco alocado e não consigo tirar ,
                                  //sei que a regra e se tiver banco alocado no cartão de credito    ai não aparece na busca quando tento editar e trocar de titulo cartão para cartão baixa automática     
                                  
                                  /*&&
                                   !(from l in db.LocalMovto
                                     where l.cd_local_movto == local.cd_local_banco &&
                                           l.cd_pessoa_empresa == local.cd_pessoa_empresa &&
                                           (l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.BANCO) && //||
                                                                                                            //l.nm_tipo_local !=  (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                           l.id_local_ativo == true
                                     select local).Any()*/)
                       select new
                       {
                           cd_local_movto = local.cd_local_movto,
                           no_local_movto = local.no_local_movto,
                           nm_agencia = local.nm_agencia,
                           nm_conta_corrente = local.nm_conta_corrente,
                           nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                           nm_tipo_local = local.nm_tipo_local,
                           cd_pessoa_empresa = local.cd_pessoa_empresa,
                           T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                           T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                           cd_local_banco = local.cd_local_banco,
                           taxaBancaria = local.TaxaBancaria
                       }).ToList().Select(x => new LocalMovto
                       {
                           cd_local_movto = x.cd_local_movto,
                           no_local_movto = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                           nm_tipo_local = x.nm_tipo_local,
                           cd_pessoa_empresa = x.cd_pessoa_empresa,
                           cd_local_banco = x.cd_local_banco,
                           T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                           T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,
                           TaxaBancaria = x.taxaBancaria
                       }).ToList();
                       if(cd_aux > 0)
                        sql = sql
                        .Union((from local in db.LocalMovto//.Include("TaxaBancaria")
                                          where local.cd_pessoa_empresa == cdEscola &&
                                                (local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.BANCO &&
                                                 //(local.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO &&
                                                 //local.nm_tipo_local !=  (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                 local.id_local_ativo == true &&
                                                 (from l in db.LocalMovto
                                                  where l.cd_local_banco == local.cd_local_movto &&
                                                        l.cd_pessoa_empresa == local.cd_pessoa_empresa &&
                                                        (l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                                         l.nm_tipo_local ==  (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                                        l.id_local_ativo == true
                                                  select local).Any())
                                                 
                                          select new
                                          {
                                              cd_local_movto = local.cd_local_movto,
                                              no_local_movto = local.no_local_movto,
                                              nm_tipo_local = local.nm_tipo_local,
                                              nm_agencia = local.nm_agencia,
                                              nm_conta_corrente = local.nm_conta_corrente,
                                              nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                                              cd_pessoa_empresa = local.cd_pessoa_empresa,
                                              T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                                              T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                                              cd_local_banco = local.cd_local_banco,
                                              taxaBancaria = local.TaxaBancaria
                                          }).ToList().Select(x => new LocalMovto
                                          {
                                              cd_local_movto = x.cd_local_movto,
                                              no_local_movto = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                                              nm_tipo_local = x.nm_tipo_local,
                                              cd_pessoa_empresa = x.cd_pessoa_empresa,
                                              cd_local_banco = x.cd_local_banco,
                                              T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                                              T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,
                                              TaxaBancaria = x.taxaBancaria
                                          }).ToList());

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalMovto> getAllLocalMovtoCartaoSemPai(int cdEscola)
        {
            try
            {
                IEnumerable<LocalMovto> sql;
                sql = (from local in db.LocalMovto.Include("TaxaBancaria")
                       where local.cd_pessoa_empresa == cdEscola &&
                             ((local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                              local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                             local.id_local_ativo == true &&
                             (from t in db.TaxaBancaria
                              where t.cd_local_movto == local.cd_local_movto
                              select t).Any() &&
                             !(from l in db.LocalMovto
                               where l.cd_local_banco == local.cd_local_movto &&
                                     l.cd_pessoa_empresa == local.cd_pessoa_empresa &&
                                     (l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                      l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                     l.id_local_ativo == true
                               select local).Any())
                       select new
                       {
                           cd_local_movto = local.cd_local_movto,
                           no_local_movto = local.no_local_movto,
                           nm_agencia = local.nm_agencia,
                           nm_conta_corrente = local.nm_conta_corrente,
                           nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                           nm_tipo_local = local.nm_tipo_local,
                           cd_pessoa_empresa = local.cd_pessoa_empresa,
                           T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                           T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                           cd_local_banco = local.cd_local_banco,

                           taxaBancaria = local.TaxaBancaria
                       }).ToList().Select(x => new LocalMovto
                       {
                           cd_local_movto = x.cd_local_movto,
                           no_local_movto = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                           nm_tipo_local = x.nm_tipo_local,
                           cd_pessoa_empresa = x.cd_pessoa_empresa,
                           cd_local_banco = x.cd_local_banco,
                           T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                           T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,

                           TaxaBancaria = x.taxaBancaria
                       }).ToList();



                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalMovto> getAllLocalMovtoCartaoComPai(int cdEscola)
        {
            try
            {
                IEnumerable<LocalMovto> sql;
                sql = (from local in db.LocalMovto.Include("TaxaBancaria")
                       where local.cd_pessoa_empresa == cdEscola &&
                             ((local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                              local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                             local.id_local_ativo == true &&
                             (from l in db.LocalMovto
                               where l.cd_local_movto == local.cd_local_banco &&
                                     l.cd_pessoa_empresa == local.cd_pessoa_empresa &&
                                     l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.BANCO &&
                                     //(l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                                     // l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                     l.id_local_ativo == true
                               select local).Any())
                       select new
                       {
                           cd_local_movto = local.cd_local_movto,
                           no_local_movto = local.no_local_movto,
                           nm_agencia = local.nm_agencia,
                           nm_conta_corrente = local.nm_conta_corrente,
                           nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                           nm_tipo_local = local.nm_tipo_local,
                           cd_pessoa_empresa = local.cd_pessoa_empresa,
                           T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                           T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                           cd_local_banco = local.cd_local_banco,
                           taxaBancaria = local.TaxaBancaria
                       }).ToList().Select(x => new LocalMovto
                       {
                           cd_local_movto = x.cd_local_movto,
                           no_local_movto = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                           nm_tipo_local = x.nm_tipo_local,
                           cd_pessoa_empresa = x.cd_pessoa_empresa,
                           cd_local_banco = x.cd_local_banco,
                           T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                           T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,
                           TaxaBancaria = x.taxaBancaria
                       }).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalMovto> getAllLocalMovtoTipoCartao(int cdEscola, int cd_tipo_liquidacao, int cd_local_movto, int cd_pessoa_usuario)
        {
            try
            {
                //Na transação Troca Financeira, o local de movimento do titulo não era cartão, portanto os locais terão que ser apenas do mesmo tipo alem do  Caixa
                bool local_cartao = (from l in db.LocalMovto
                                     where
                                         l.cd_pessoa_empresa == cdEscola &&
                                         l.cd_local_movto == cd_local_movto &&
                                         l.nm_tipo_local == (cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CARTAO_CREDITO ?
                                            (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO : (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                         l.id_local_ativo == true
                                     select l).Any();

                int? cd_local_pai = (from l in db.LocalMovto
                                     where l.cd_local_banco != null &&
                                         l.cd_pessoa_empresa == cdEscola &&
                                         l.cd_local_movto == cd_local_movto &&
                                         l.nm_tipo_local == (cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CARTAO_CREDITO ?
                                            (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO :
                                            (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                         l.id_local_ativo == true
                                     select l.cd_local_banco).FirstOrDefault();
                
                IEnumerable<LocalMovto> sql;
                if (!local_cartao)
                    sql = (from local in db.LocalMovto
                           where local.cd_pessoa_empresa == cdEscola &&
                                 local.id_local_ativo == true &&
                                 (local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA && (cd_pessoa_usuario == 0 || (local.cd_pessoa_local == cd_pessoa_usuario || local.cd_pessoa_local == null))) ||
                                 ((local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.BANCO && local.cd_pessoa_local == null) &&
                                 (from l in db.LocalMovto
                                  where l.cd_local_banco == local.cd_local_movto &&
                                        l.cd_pessoa_empresa == cdEscola &&
                                        l.nm_tipo_local == (cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CARTAO_CREDITO ?
                                            (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO :
                                            (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) &&
                                        l.id_local_ativo == true
                                  select l).Any())
                           select new
                           {
                               cd_local_movto = local.cd_local_movto,
                               no_local_movto = local.no_local_movto,
                               nm_agencia = local.nm_agencia,
                               nm_conta_corrente = local.nm_conta_corrente,
                               nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                               nm_tipo_local = local.nm_tipo_local,
                               cd_pessoa_empresa = local.cd_pessoa_empresa,
                               T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                               T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                               cd_local_banco = local.cd_local_banco,
                               taxaBancaria = local.TaxaBancaria
                           }).ToList().Select(x => new LocalMovto
                           {
                               cd_local_movto = x.cd_local_movto,
                               no_local_movto = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                               nm_tipo_local = x.nm_tipo_local,
                               cd_pessoa_empresa = x.cd_pessoa_empresa,
                               cd_local_banco = x.cd_local_banco,
                               T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                               T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,
                               TaxaBancaria = x.taxaBancaria
                           }).ToList().Union(
                                sql = (from local in db.LocalMovto
                                       where local.cd_pessoa_empresa == cdEscola &&
                                             ((local.nm_tipo_local == (cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CARTAO_CREDITO ?
                                                                                            (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO :
                                                                                            (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO))) &&
                                             local.id_local_ativo == true &&
                                             !(from l in db.LocalMovto
                                               where l.cd_local_movto == local.cd_local_banco &&
                                                     l.cd_pessoa_empresa == cdEscola &&
                                                     (l.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.BANCO && local.cd_pessoa_local == null) &&
                                                     l.id_local_ativo == true
                                               select l).Any()
                                       select new
                                       {
                                           cd_local_movto = local.cd_local_movto,
                                           no_local_movto = local.no_local_movto,
                                           nm_agencia = local.nm_agencia,
                                           nm_conta_corrente = local.nm_conta_corrente,
                                           nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                                           nm_tipo_local = local.nm_tipo_local,
                                           cd_pessoa_empresa = local.cd_pessoa_empresa,
                                           T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                                           T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                                           cd_local_banco = local.cd_local_banco,
                                           taxaBancaria = local.TaxaBancaria
                                       }).ToList().Select(x => new LocalMovto
                                   {
                                       cd_local_movto = x.cd_local_movto,
                                       no_local_movto = x.no_local_movto,
                                       nm_tipo_local = x.nm_tipo_local,
                                       cd_pessoa_empresa = x.cd_pessoa_empresa,
                                       cd_local_banco = x.cd_local_banco,
                                       T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                                       T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,
                                       TaxaBancaria = x.taxaBancaria
                                   }).ToList()
                           );
                else
                {
                    // Listando apenas o banco do cartão, portanto não existe Taxas Bancárias
                    if (cd_local_pai != null)
                    {
                        cd_local_movto = (int)cd_local_pai;
                        sql = (from local in db.LocalMovto
                               where local.cd_pessoa_empresa == cdEscola &&
                                       local.id_local_ativo == true &&
                                       ((local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA && (cd_pessoa_usuario == 0 || (local.cd_pessoa_local == cd_pessoa_usuario || local.cd_pessoa_local == null))) ||
                                       (local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.BANCO && local.cd_pessoa_local == null)) &&
                                       (local.cd_local_movto == cd_local_movto || local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA)
                               select new
                               {
                                   cd_local_movto = local.cd_local_movto,
                                   no_local_movto = local.no_local_movto,
                                   nm_agencia = local.nm_agencia,
                                   nm_conta_corrente = local.nm_conta_corrente,
                                   nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                                   nm_tipo_local = local.nm_tipo_local,
                                   cd_pessoa_empresa = local.cd_pessoa_empresa,
                                   T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                                   T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                                   cd_local_banco = local.cd_local_banco,
                                   taxaBancaria = local.TaxaBancaria
                               }).ToList().Select(x => new LocalMovto
                        {
                            cd_local_movto = x.cd_local_movto,
                            no_local_movto = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto,
                            nm_tipo_local = x.nm_tipo_local,
                            cd_pessoa_empresa = x.cd_pessoa_empresa,
                            cd_local_banco = x.cd_local_banco,
                            T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                            T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,
                            TaxaBancaria = x.taxaBancaria
                        }).ToList();
                    }
                    else
                    {
                        sql = (from local in db.LocalMovto
                               where local.cd_pessoa_empresa == cdEscola &&
                                     ((local.nm_tipo_local == (cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CARTAO_CREDITO ?
                                                                                    (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO :
                                                                                    (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO) ||
                                      (local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA && (cd_pessoa_usuario == 0 ||
                                      (local.cd_pessoa_local == cd_pessoa_usuario || local.cd_pessoa_local == null))))) &&
                                     local.id_local_ativo == true &&
                                     (local.cd_local_movto == cd_local_movto || local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA)
                               select new
                               {
                                   cd_local_movto = local.cd_local_movto,
                                   no_local_movto = local.no_local_movto,
                                   nm_agencia = local.nm_agencia,
                                   nm_conta_corrente = local.nm_conta_corrente,
                                   nm_digito_conta_corrente = local.nm_digito_conta_corrente,
                                   nm_tipo_local = local.nm_tipo_local,
                                   cd_pessoa_empresa = local.cd_pessoa_empresa,
                                   T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                                   T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                                   cd_local_banco = local.cd_local_banco,
                                   taxaBancaria = local.TaxaBancaria
                               }).ToList().Select(x => new LocalMovto
                               {
                                   cd_local_movto = x.cd_local_movto,
                                   no_local_movto = x.no_local_movto,
                                   nm_tipo_local = x.nm_tipo_local,
                                   cd_pessoa_empresa = x.cd_pessoa_empresa,
                                   cd_local_banco = x.cd_local_banco,
                                   T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                                   T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,
                                   TaxaBancaria = x.taxaBancaria
                               }).ToList();
                    }
                }
                return sql.Distinct().OrderBy(x => x.no_local_movto);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalMovto> getAllLocalMovtoCheque(int cdEscola)
        {
            try
            {
                IEnumerable<LocalMovto> sql;
                sql = (from local in db.LocalMovto.Include("TaxaBancaria")
                    where local.cd_pessoa_empresa == cdEscola
                        && (local.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO
                        /*|| local.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CAIXA*/) &&
                        !local.cd_pessoa_local.HasValue && local.id_local_ativo == true
                    select new
                    {
                        cd_local_movto = local.cd_local_movto,
                        no_local_movto = local.no_local_movto,
                        nm_tipo_local = local.nm_tipo_local,

                        T_LOCAL_MOVTO1 = local.T_LOCAL_MOVTO1,
                        T_LOCAL_MOVTO2 = local.T_LOCAL_MOVTO2,
                        cd_local_banco = local.cd_local_banco,

                        taxaBancaria = local.TaxaBancaria
                    }).ToList().Select(x => new LocalMovto
                {
                    cd_local_movto = x.cd_local_movto,
                    no_local_movto = x.no_local_movto,
                    nm_tipo_local = x.nm_tipo_local,

                    cd_local_banco = x.cd_local_banco,
                    T_LOCAL_MOVTO1 = x.T_LOCAL_MOVTO1,
                    T_LOCAL_MOVTO2 = x.T_LOCAL_MOVTO2,

                    TaxaBancaria = x.taxaBancaria
                }).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

       
    }
}
