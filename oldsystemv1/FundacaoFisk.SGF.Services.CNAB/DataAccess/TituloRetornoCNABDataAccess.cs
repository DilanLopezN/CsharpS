using System;
using System.Collections.Generic;
using System.Linq;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using System.Data.Entity;
using Componentes.Utils;
using Componentes.GenericDataAccess;
using System.Data;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.CNAB.DataAccess
{
    public class TituloRetornoCNABDataAccess : GenericRepository<TituloRetornoCNAB>, ITituloRetornoCnabDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public List<TituloRetornoCNAB> searchTituloCnabGradeRet(TituloUI titulo)
        {
            try
            {
                var sql = from t in titulo.titulosGradeRet
                          select t;
                if (titulo.cd_pessoa > 0)
                    sql = from t in sql
                          where t.cd_pessoa_responsavel == titulo.cd_pessoa
                          select t;
                if (titulo.cd_aluno > 0)
                    sql = from t in sql
                          where t.cd_aluno == titulo.cd_aluno
                          select t;
                if (titulo.nro_contrato > 0)
                    sql = from t in sql
                          where t.nro_contrato == titulo.nro_contrato
                          select t;
                if(titulo.id_tipo_retorno > 0)
                    sql = from t in sql
                          where t.id_tipo_retorno == titulo.id_tipo_retorno
                          select t;

                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<TituloRetornoCNAB> getTituloRetornoCNAB(int cd_retorno_cnab)
        {
            try
            {
                var sql = (from tc in db.TituloRetornoCNAB
                           where tc.cd_retorno_cnab == cd_retorno_cnab
                           select new
                           {
                               tc.cd_retorno_cnab,
                               tc.dc_nosso_numero,
                               dt_emissao = (DateTime?)tc.Titulo.dt_emissao_titulo,
                               dt_vcto = (DateTime?)tc.Titulo.dt_vcto_titulo,
                               tc.Titulo.Pessoa.no_pessoa,
                               no_responsavel = tc.Titulo.PessoaResponsavel.no_pessoa,
                               tc.Titulo.nm_parcela_titulo,
                               tc.Titulo.nm_titulo,
                               vl_titulo = (Decimal?)tc.Titulo.vl_titulo,
                               id_tipo_retorno = tc.id_tipo_retorno,
                               dt_liquidacao_titulo = tc.Titulo.dt_liquidacao_titulo,
                               cd_titulo_retorno_cnab = tc.cd_titulo_retorno_cnab,
                               vl_liquidacao_titulo = (decimal?)tc.Titulo.BaixaTitulo.Sum(x => x.vl_liquidacao_baixa) ?? 0,
                               vl_desconto_titulo = (decimal?)tc.Titulo.vl_desconto_titulo ?? 0,
                               vl_saldo_titulo = (decimal?)tc.Titulo.vl_saldo_titulo ?? 0
                           }).ToList().Select(x => new TituloRetornoCNAB
                           {
                               cd_retorno_cnab = x.cd_retorno_cnab,
                               dc_nosso_numero = x.dc_nosso_numero,
                               id_tipo_retorno = x.id_tipo_retorno,
                               DespesaTituloCnab = db.DespesaTituloCnab.Where(d => d.cd_titulo_retorno_cnab == x.cd_titulo_retorno_cnab).ToList(),
                               Titulo = x.nm_titulo != null ? new Titulo
                               {
                                   dt_liquidacao_titulo = x.dt_liquidacao_titulo,
                                   dt_emissao_titulo = x.dt_emissao.Value,
                                   dt_vcto_titulo = x.dt_vcto.Value,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   nm_titulo = x.nm_titulo,
                                   vl_titulo = x.vl_titulo.Value,
                                   Pessoa = new PessoaSGF { 
                                       no_pessoa = x.no_pessoa
                                   },
                                   PessoaResponsavel = new PessoaSGF{
                                       no_pessoa = x.no_responsavel
                                   },
                                   vl_liquidacao_titulo = x.vl_liquidacao_titulo,
                                   vl_desconto_titulo = x.vl_desconto_titulo,
                                   vl_saldo_titulo = x.vl_saldo_titulo
                               } : null
                           });

                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<TituloCnab> getTituloCNAB(int cd_cnab)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = (from t in db.TituloCnab.AsNoTracking()
                           where t.cd_cnab == cd_cnab
                           select new
                            {
                                t.cd_titulo_cnab,
                                t.dc_nosso_numero_titulo,
                                t.Titulo.dt_emissao_titulo,
                                t.Titulo.dt_vcto_titulo,
                                t.Titulo.Pessoa.no_pessoa,
                                no_responsavel = t.Titulo.PessoaResponsavel.no_pessoa,
                                t.Titulo.nm_parcela_titulo,
                                t.Titulo.nm_titulo,
                                t.Titulo.vl_titulo,
                                id_status_titulo_cnab = t.Titulo.id_status_cnab,
                                Descontos = t.DescontoTituloCNAB,
                                vl_desconto_bolsa = t.Titulo.vl_liquidacao_titulo > 0 && 
                                                    t.Titulo.BaixaTitulo.Any(x => (x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA ||
                                                                                  x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)) ?
                                                                                  t.Titulo.BaixaTitulo.Where(x => x.cd_titulo == t.cd_titulo && 
                                                                                                             (x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA ||
                                                                                                              x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)).FirstOrDefault().vl_baixa_saldo_titulo : 0,
                                t.Titulo.LocalMovto.no_local_movto,
                                no_arquivo_remessa = t.Cnab.no_arquivo_remessa,
                                no_turma_titulo = t.Turma != null ? t.Turma.cd_turma_ppt > 0 ? t.Turma.TurmaPai.no_turma : t.Turma.no_turma : "Sem Turma"
                            }).ToList().Select(x => new TituloCnab
                           {
                               cd_titulo_cnab = x.cd_titulo_cnab,
                               dc_nosso_numero_titulo = x.dc_nosso_numero_titulo,
                               id_status_cnab_titulo = x.id_status_titulo_cnab,
                               Titulo = new Titulo
                               {
                                   dt_emissao_titulo = x.dt_emissao_titulo,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   nm_titulo = x.nm_titulo,
                                   vl_titulo = x.vl_desconto_bolsa > 0 ? x.vl_titulo - x.vl_desconto_bolsa : x.vl_titulo,
                                   Pessoa = new PessoaSGF
                                   {
                                       no_pessoa = x.no_pessoa
                                   },
                                   PessoaResponsavel = new PessoaSGF
                                   {
                                       no_pessoa = x.no_responsavel
                                   },
                                   LocalMovto = new LocalMovto 
                                   {
                                       no_local_movto = x.no_local_movto
                                   }
                               },
                               Cnab = new Cnab
                               {
                                   no_arquivo_remessa = x.no_arquivo_remessa
                               },
                               DescontoTituloCNAB = x.Descontos,
                               no_turma_titulo = x.no_turma_titulo
                           });


                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TituloRetornoCNAB getTituloRetornoCnabEditView(int cd_titulo_retorno_cnab, int cd_empresa) {
            try {
                var sql = (from tc in db.TituloRetornoCNAB
                           where tc.RetornoCNAB.LocalMovto.cd_pessoa_empresa == cd_empresa && tc.cd_titulo_retorno_cnab == cd_titulo_retorno_cnab
                           select new
                           {
                               dc_nosso_numero = tc.dc_nosso_numero,
                               cd_titulo_retorno_cnab = tc.cd_titulo_retorno_cnab,
                               no_local_movto = tc.RetornoCNAB.LocalMovto.no_local_movto,
                               nm_agencia = tc.RetornoCNAB.LocalMovto.nm_agencia,
                               nm_conta_corrente = tc.RetornoCNAB.LocalMovto.nm_conta_corrente,
                               nm_digito_conta_corrente = tc.RetornoCNAB.LocalMovto.nm_digito_conta_corrente != null ? tc.RetornoCNAB.LocalMovto.nm_digito_conta_corrente : null,
                               nm_tipo_local = tc.RetornoCNAB.LocalMovto.nm_tipo_local,
                               nomePessoaTitulo = tc.Titulo.Pessoa.no_pessoa,
                               nomeResponsavelTitulo = tc.Titulo.PessoaResponsavel.no_pessoa,
                               nm_titulo = tc.Titulo.nm_titulo,
                               nm_parcela_titulo = tc.Titulo.nm_parcela_titulo,
                               dt_vcto_titulo = tc.Titulo != null ? tc.Titulo.dt_vcto_titulo : DateTime.MinValue,
                               id_tipo_retorno = tc.id_tipo_retorno,
                               juros = tc.vl_juros_retorno,
                               multa = tc.vl_multa_retorno,
                               desconto = tc.vl_desconto_titulo,
                               tx_mensagem_retorno = tc.tx_mensagem_retorno,
                               vl_baixa_retorno = tc.vl_baixa_retorno,
                               dt_banco_retorno = tc.dt_banco_retorno
                           }).ToList().Select(x => new TituloRetornoCNAB
                           {
                               dc_nosso_numero = x.dc_nosso_numero,
                               cd_titulo_retorno_cnab = x.cd_titulo_retorno_cnab,
                               nomePessoaTitulo = x.nomePessoaTitulo,
                               id_tipo_retorno = x.id_tipo_retorno,
                               vl_juros_retorno = x.juros,
                               vl_multa_retorno = x.multa,
                               vl_desconto_titulo = x.desconto,
                               tx_mensagem_retorno = x.tx_mensagem_retorno,
                               dt_banco_retorno = x.dt_banco_retorno,
                               vl_baixa_retorno = x.vl_baixa_retorno,
                               Titulo = new Titulo
                               {
                                   nm_titulo = x.nm_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   PessoaResponsavel = new PessoaSGF
                                   {
                                       no_pessoa = x.nomeResponsavelTitulo
                                   }
                               },
                               RetornoCNAB = new RetornoCNAB
                               {
                                   LocalMovto = new LocalMovto
                                   {
                                       //cd_local_movto = x.cd_local_movto,
                                       no_local_movto = x.no_local_movto,
                                       nm_agencia = x.nm_agencia,
                                       nm_conta_corrente = x.nm_conta_corrente,
                                       nm_digito_conta_corrente = x.nm_digito_conta_corrente,
                                       nm_tipo_local = x.nm_tipo_local
                                   }
                               }
                           }).FirstOrDefault();

                if(sql != null)
                    sql.DespesaTituloCnab = db.DespesaTituloCnab.Where(d => d.cd_titulo_retorno_cnab == sql.cd_titulo_retorno_cnab).ToList();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }
    }
}
