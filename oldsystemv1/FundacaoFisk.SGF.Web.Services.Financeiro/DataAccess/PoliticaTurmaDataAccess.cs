using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using System.Data.SqlClient;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using Componentes.GenericDataAccess.GenericException;
    using System.Data.Objects;
    public class PoliticaTurmaDataAccess : GenericRepository<PoliticaTurma>, IPoliticaTurmaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<PoliticaTurma> getTurmaPolitica(int cdPolitica, int cdEscola)
        {
            try
            {
                var retorno = (from politicaTurma in db.PoliticaTurma
                               where politicaTurma.cd_politica_desconto == cdPolitica &&
                               politicaTurma.PolitcaDesconto.cd_pessoa_escola == cdEscola
                               select new
                               {
                                   politicaTurma.cd_turma,
                                   politicaTurma.Turma.no_turma,
                                   politicaTurma.cd_politica_turma,
                                   politicaTurma.cd_politica_desconto
                               }).ToList().Select(x => new PoliticaTurma
                               {
                                   cd_turma = x.cd_turma,
                                   no_turma = x.no_turma,
                                   cd_politica_turma = x.cd_politica_turma,
                                   cd_politica_desconto = x.cd_politica_desconto

                               }); 
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<PoliticaTurma> getTurmaPoliticaFull(int cdPolitica, int cdEscola)
        {
            try
            {
                var retorno = from politicaTurma in db.PoliticaTurma
                              where politicaTurma.cd_politica_desconto == cdPolitica &&
                              politicaTurma.PolitcaDesconto.cd_pessoa_escola == cdEscola
                              select politicaTurma;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public IEnumerable<Feriado> getFeriadosEscola(int cd_escola, bool feriado_financeiro)
        {
            try
            {
                var sql = from feriado in db.Feriado
                          where feriado.id_feriado_ativo == true
                          select feriado;

                if (feriado_financeiro)
                    sql = from feriado in sql
                          where feriado.id_feriado_financeiro
                          select feriado;

                IEnumerable<Feriado> sql1 = (from feriado in sql
                                             where (feriado.cd_pessoa_escola == null || feriado.cd_pessoa_escola == cd_escola)
                                             select new
                                             {
                                                 aa_feriado = feriado.aa_feriado,
                                                 aa_feriado_fim = feriado.aa_feriado_fim,
                                                 dd_feriado = feriado.dd_feriado,
                                                 dd_feriado_fim = feriado.dd_feriado_fim,
                                                 mm_feriado = feriado.mm_feriado,
                                                 mm_feriado_fim = feriado.mm_feriado_fim,
                                                 dc_feriado = feriado.dc_feriado,
                                                 cod_feriado = feriado.cod_feriado
                                             }).ToList().Select(x => new Feriado
                                             {
                                                 aa_feriado = x.aa_feriado,
                                                 aa_feriado_fim = x.aa_feriado_fim,
                                                 dd_feriado = x.dd_feriado,
                                                 dd_feriado_fim = x.dd_feriado_fim,
                                                 mm_feriado = x.mm_feriado,
                                                 mm_feriado_fim = x.mm_feriado_fim,
                                                 dc_feriado = x.dc_feriado,
                                                 cod_feriado = x.cod_feriado
                                             });
                return sql1;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Contrato getContratoBaixa(int cd_escola, int cd_contrato)
        {
            Contrato contrato = new Contrato();
            try
            {

                contrato = ((from c in db.Contrato
                             where c.cd_pessoa_escola == cd_escola && c.cd_contrato == cd_contrato
                             select new
                             {
                                 AlunosTurma = c.AlunoTurma.Where(at => (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo) ||
                                                                         at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                                         at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado),
                                 //DescontoContrato = c.DescontoContrato.Where(dc => dc.id_desconto_ativo),
                                 cd_aluno = c.cd_aluno,
                                 cd_pessoa_escola = c.cd_pessoa_escola,
                                 ultimoAditamento = c.Aditamento.OrderByDescending(x => x.dt_aditamento).FirstOrDefault()
                             }).ToList().Select(x => new Contrato
                             {
                                 AlunoTurma = x.AlunosTurma.ToList(),
                                 //DescontoContrato = x.DescontoContrato.ToList(),
                                 cd_aluno = x.cd_aluno,
                                 cd_pessoa_escola = x.cd_pessoa_escola,
                                 aditamentoMaxData = x.ultimoAditamento
                             })).FirstOrDefault();

                if (contrato != null && contrato.cd_aluno > 0)
                {
                    List<DescontoContrato> descontos = new List<DescontoContrato>();
                    if (db.Aditamento.Any(aditamento => aditamento.cd_contrato == cd_contrato && aditamento.Contrato.cd_pessoa_escola == cd_escola &&
                                          aditamento.id_tipo_aditamento != null && (aditamento.id_tipo_aditamento == (int)Aditamento.TipoAditamento.CONCESSAO_DESCONTO ||
                                          aditamento.id_tipo_aditamento == (int)Aditamento.TipoAditamento.PERDA_DESCONTO)))
                        descontos = (from d in db.DescontoContrato
                                     where d.Aditamento.Contrato.cd_pessoa_escola == cd_escola &&
                                           d.cd_aditamento == db.Aditamento.Where(aditamento => aditamento.cd_contrato == cd_contrato && aditamento.Contrato.cd_pessoa_escola == cd_escola &&
                                                                                  aditamento.id_tipo_aditamento != null && (aditamento.id_tipo_aditamento == (int)Aditamento.TipoAditamento.CONCESSAO_DESCONTO ||
                                                                                                                            aditamento.id_tipo_aditamento == (int)Aditamento.TipoAditamento.PERDA_DESCONTO))
                                                                                  .Max(x => x.cd_aditamento)
                                     select d).ToList();
                    else
                        if (descontos == null || descontos.Count() <= 0)
                        descontos = (from d in db.DescontoContrato
                                     where d.cd_contrato == cd_contrato && d.Contratos.cd_pessoa_escola == cd_escola
                                     select d).ToList();
                    contrato.DescontoContrato = descontos;
                }
                if (contrato.aditamentoMaxData != null)
                {
                    contrato.Aditamento = new List<Aditamento>();
                    contrato.Aditamento.Add(contrato.aditamentoMaxData);
                }
                return contrato;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Parametro getParametrosBaixa(int cd_escola)
        {
            try
            {
                return (from p in db.Parametro
                    where p.cd_pessoa_escola == cd_escola
                    select new
                    {
                        id_somar_descontos_financeiros = p.id_somar_descontos_financeiros,
                        nm_dias_carencia = p.nm_dias_carencia,
                        id_cobrar_juros_multa = p.id_cobrar_juros_multa,
                        pc_juros_dia = p.pc_juros_dia,
                        pc_multa = p.pc_multa,
                        per_desconto_maximo = p.per_desconto_maximo,
                        id_permitir_desc_apos_politica = p.id_permitir_desc_apos_politica,
                        id_alterar_venc_final_semana = p.id_alterar_venc_final_semana,
                        id_juros_final_semana = p.id_juros_final_semana
                    }).ToList().Select(x => new Parametro
                {
                    id_somar_descontos_financeiros = x.id_somar_descontos_financeiros,
                    nm_dias_carencia = x.nm_dias_carencia,
                    id_cobrar_juros_multa = x.id_cobrar_juros_multa,
                    pc_juros_dia = x.pc_juros_dia,
                    pc_multa = x.pc_multa,
                    per_desconto_maximo = x.per_desconto_maximo,
                    id_permitir_desc_apos_politica = x.id_permitir_desc_apos_politica,
                    id_alterar_venc_final_semana = x.id_alterar_venc_final_semana,
                    id_juros_final_semana = x.id_juros_final_semana
                }).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Parametro getParametrosEscola(int cd_escola)
        {
            try
            {
                return (from p in db.Parametro where p.cd_pessoa_escola == cd_escola select p).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}