using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using System.Data.SqlClient;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using Componentes.GenericDataAccess.GenericException;
using System.Data.Entity.Core.Objects;
using System.Data.Objects.SqlClient;
using System.Web.Mvc.Html;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class TituloDataAccess : GenericRepository<Titulo>, ITituloDataAccess
    {

        public enum TipoConsultaTituloEnum
        {
            HAS_EDIT_TITULO_VIEW = 0,
            HAS_EDIT_TITULO = 1,
            HAS_TITULO_GRADE =2,
            TITULO_ABERTO = 3,
            TITULO_FECHADO = 4,
            TITULO_CANCELADO = 5,
            TITULO_BAIXA_CNAB = 6,
            HAS_TITULO_MOVIMENTO_NF = 7,
            HAS_SIMULACAO = 8,
            HAS_TITULOS_ABERTO_SIMULACAO = 9
        }

        public enum EnumTipoCKMatricula 
        {
            NORMAL = 0,
            MULTIPLA = 1,
            PROFISSIONALIZANTE=2,
            INFORMATICA= 3
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<RptReceberPagar> receberPagarStoreProcedure(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pForn, DateTime pDtaBase, byte pNatureza, int pPlanoContas, int ordem, bool cSegura, int cdTpLiq, int cdTpFinan, int tipo, string situacoes, int cdTurma)
        {
            try {

                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                string[] situacao = situacoes.Split('|');
                List<int> cdsSituacoesList = new List<int>();
                for (int i = 0; i < situacao.Count(); i++)
                    cdsSituacoesList.Add(Int32.Parse(situacao[i]));

                IEnumerable<RptReceberPagar> sql = new List<RptReceberPagar>();
                //IEnumerable<st_RptReceberPagar_Result> sql = (from st in db.st_RptReceberPagar(cdEscola, pDtaI, pDtaF, pForn, pDtaBase, pNatureza, pPlanoContas)
                //                                              select st);
                var sqlTitulo = from t in db.Titulo
                                where t.cd_pessoa_empresa == cdEscola
                                    && t.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO
                                    && t.id_natureza_titulo == pNatureza
                                    && DbFunctions.TruncateTime(t.dt_vcto_titulo) >= pDtaI.Date && DbFunctions.TruncateTime(t.dt_vcto_titulo) <= pDtaF.Date
                                    && (t.cd_pessoa_titulo == pForn || pForn == 0)
                                    && (t.PlanoTitulo.Where(tpt => tpt.cd_plano_conta == pPlanoContas).Any() || pPlanoContas == 0)
                                    && (t.cd_tipo_financeiro == cdTpFinan || cdTpFinan == 0)
                                    && ((t.BaixaTitulo.Any() && t.BaixaTitulo.Where(bt => bt.cd_tipo_liquidacao == cdTpLiq).Any()) || cdTpLiq == 0)
                                select t;
                if (!cSegura)
                    sqlTitulo = from s in sqlTitulo
                                where s.PlanoTitulo.Where(p => p.PlanoConta.id_conta_segura == false).Any()
                                select s;

                if (tipo == (int) Titulo.TipoMovimento.RECEBER)
                {
                    if (cdTurma > 0)
                    {

                        sqlTitulo = from t in sqlTitulo
                            where (from h in db.HistoricoAluno
                                where h.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo &&
                                      h.cd_contrato == t.cd_origem_titulo &&
                                      t.id_origem_titulo == cd_origem &&
                                      h.cd_turma == cdTurma &&
                                      h.dt_historico <= pDtaBase &&
                                      h.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_aluno == h.cd_aluno && han.cd_contrato == h.cd_contrato
                                                                                                                  && DbFunctions.TruncateTime(han.dt_historico) <= pDtaBase
                                      ).Max(x => x.nm_sequencia)
                                select h.cd_turma).Any()
                            select t;
                    }

                    if (!cdsSituacoesList.Contains(100) && cdsSituacoesList.Count() > 0)
                    {
                        sqlTitulo = from t in sqlTitulo
                            where cdsSituacoesList.Contains((from histA in db.HistoricoAluno 
                                where histA.Aluno.cd_pessoa_escola == cdEscola && 
                                      histA.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo && 
                                      histA.dt_historico < pDtaBase 
                                orderby histA.nm_sequencia descending 
                                select histA.id_situacao_historico).FirstOrDefault())
                                //c.HistoricosAluno.Where(histA => histA.Aluno.cd_pessoa_escola == cdEscola && histA.Aluno. == t.cd_pessoa).OrderByDescending(n => n.nm_sequencia).FirstOrDefault().id_situacao_historico)
                            select t;
                    }

                }

                if(ordem == 2) //Plano de contas
                    sql = (from t in sqlTitulo
                           join plano_titulo in db.PlanoTitulo on t.cd_titulo equals plano_titulo.cd_titulo into Inners
                           from pt in Inners.DefaultIfEmpty()
                           where (pt.cd_plano_conta == pPlanoContas || pPlanoContas == 0)
                           select new {
                               nm_escola = t.Empresa.no_pessoa,
                               cd_titulo = t.cd_titulo,
                               nm_titulo = t.nm_titulo,
                               nm_parcela_titulo = t.nm_parcela_titulo,
                               dt_emissao_titulo = t.dt_emissao_titulo,
                               dt_vcto_titulo = t.dt_vcto_titulo,
                               id_origem_titulo = t.id_origem_titulo,
                               cd_origem_titulo = t.cd_origem_titulo,
                               cd_tipo_financeiro = t.cd_tipo_financeiro,
                               dc_tipo_financeiro = t.TipoFinanceiro.dc_tipo_financeiro,
                               t.dc_tipo_titulo,
                               cd_pessoa = t.Pessoa.cd_pessoa,
                               no_pessoa = t.Pessoa.no_pessoa,
                               nm_natureza_pessoa = t.Pessoa.nm_natureza_pessoa,
                               nm_cgc_cpf = t.Pessoa.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                   //db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                   //   db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                       db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                               db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                               nm_rg = t.Pessoa.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                   //db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                   //   db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_doc_identidade :
                                       db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_doc_identidade :
                               db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_insc_estadual,
                               dt_nascimento = t.Pessoa.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                   //db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                   //   db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.dt_nascimento :
                                       db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().dt_nascimento :
                               db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dt_registro_junta_comercial,
                               telefoneSGF = t.Pessoa.TelefonePessoa.Where(tp => tp.cd_tipo_telefone == (int) TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE || tp.cd_tipo_telefone == (int) TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault(),//.Pessoa.Telefone.dc_fone_mail,
                               //dc_endereco
                               no_tipo_logradouro = t.Pessoa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro,
                               no_localidade = t.Pessoa.EnderecoPrincipal.Logradouro.no_localidade,
                               dc_num_endereco = t.Pessoa.EnderecoPrincipal.dc_num_endereco,
                               dc_compl_endereco = t.Pessoa.EnderecoPrincipal.dc_compl_endereco,

                               no_bairro_aluno = t.Pessoa.EnderecoPrincipal.Bairro.no_localidade,
                               nm_cep_aluno = t.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep,

                               //dc_cidade_uf_aluno
                               no_localidade_cidade = t.Pessoa.EnderecoPrincipal.Cidade.no_localidade,
                               sg_estado = t.Pessoa.EnderecoPrincipal.Estado.Estado.sg_estado,

                               cd_responsavel = t.PessoaResponsavel.cd_pessoa,
                               nm_responsavel = t.PessoaResponsavel.no_pessoa,
                               nm_natureza_resp = t.PessoaResponsavel.nm_natureza_pessoa,
                               nm_cgc_cpf_resp = t.PessoaResponsavel.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                       db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                               db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                               nm_rg_resp = t.PessoaResponsavel.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                       db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_doc_identidade :
                               db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_insc_estadual,
                               dt_nascimento_resp = t.PessoaResponsavel.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                       db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().dt_nascimento :
                               db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dt_registro_junta_comercial,
                               telefoneRespSGF = t.PessoaResponsavel.TelefonePessoa.Where(tp => tp.cd_tipo_telefone == (int) TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE || tp.cd_tipo_telefone == (int) TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault(),//.Pessoa.Telefone.dc_fone_mail,
                               
                                //dc_endereco_resp
                               no_tipo_logradouro_resp = t.PessoaResponsavel.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro,
                               no_localidade_resp = t.PessoaResponsavel.EnderecoPrincipal.Logradouro.no_localidade,
                               dc_num_endereco_resp = t.PessoaResponsavel.EnderecoPrincipal.dc_num_endereco,
                               dc_compl_endereco_resp = t.PessoaResponsavel.EnderecoPrincipal.dc_compl_endereco,

                               no_bairro_resp = t.PessoaResponsavel.EnderecoPrincipal.Bairro.no_localidade,
                               nm_cep_resp = t.PessoaResponsavel.EnderecoPrincipal.Logradouro.dc_num_cep,

                               //dc_cidade_uf_resp
                               no_localidade_cidade_resp = t.PessoaResponsavel.EnderecoPrincipal.Cidade.no_localidade,
                               sg_estado_resp = t.PessoaResponsavel.EnderecoPrincipal.Estado.Estado.sg_estado,
                               vl_saldo_titulo = t.vl_saldo_titulo,
                               qtd_subgrupo_conta = t.PlanoTitulo.Count(),
                               vl_plano_titulo = pt.vl_plano_titulo,
                               cd_subgrupo_conta = pt.PlanoConta != null ? pt.PlanoConta.PlanoContaSubgrupo.cd_subgrupo_conta : 0,
                               no_subgrupo_conta = pt.PlanoConta != null ? pt.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta : "",
                               cd_turma = (tipo == (int)Titulo.TipoMovimento.RECEBER && cdTurma > 0) ? 
                                   (from h in db.HistoricoAluno
                                   where h.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo &&
                                         h.cd_contrato == t.cd_origem_titulo &&
                                         t.id_origem_titulo == cd_origem &&
                                         h.cd_turma == cdTurma &&
                                         h.dt_historico <= pDtaBase &&
                                         h.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_aluno == h.cd_aluno && han.cd_contrato == h.cd_contrato
                                                                                                                     && DbFunctions.TruncateTime(han.dt_historico) <= pDtaBase
                                         ).Max(x => x.nm_sequencia)
                                   select h.cd_turma).FirstOrDefault() : 0,
                                   
                           }).ToList().Select(x => new RptReceberPagar {
                               dc_tipo_titulo = x.dc_tipo_titulo,
                               nm_escola = x.nm_escola,
                               nm_titulo = x.nm_titulo,
                               cd_titulo = x.cd_titulo,
                               nm_parcela_titulo = x.nm_parcela_titulo,
                               dt_emissao_titulo = x.dt_emissao_titulo,
                               dt_vcto_titulo = x.dt_vcto_titulo.Date,
                               id_origem_titulo = x.id_origem_titulo,
                               cd_origem_titulo = x.cd_origem_titulo,
                               cd_tipo_financeiro = x.cd_tipo_financeiro,
                               dc_tipo_financeiro = x.dc_tipo_financeiro,
                               cd_pessoa = x.cd_pessoa,
                               no_pessoa = x.no_pessoa,
                               nm_natureza_aluno = x.nm_natureza_pessoa,
                               nm_cgc_cpf = x.nm_cgc_cpf,
                               nm_rg = x.nm_rg,
                               dt_nascimento = x.dt_nascimento,
                               nm_telefone_aluno = x.telefoneSGF != null ? x.telefoneSGF.dc_fone_mail : "",
                               dc_endereco = x.no_tipo_logradouro + ' ' + x.no_localidade + (String.IsNullOrEmpty(x.dc_num_endereco) ? "" : " nº " + x.dc_num_endereco)
                                           + (String.IsNullOrEmpty(x.dc_compl_endereco) ? "" : "/" + x.dc_compl_endereco),

                               no_bairro_aluno = x.no_bairro_aluno,
                               nm_cep_aluno = x.nm_cep_aluno,

                               dc_cidade_uf_aluno = (String.IsNullOrEmpty(x.no_localidade) ? "" : x.no_localidade + (String.IsNullOrEmpty(x.sg_estado) ? "" : " - " + x.sg_estado)),

                               cd_responsavel = x.cd_responsavel,
                               nm_responsavel = x.nm_responsavel,
                               nm_natureza_resp = x.nm_natureza_resp,
                               nm_cgc_cpf_resp = x.nm_cgc_cpf_resp,
                               nm_rg_resp = x.nm_rg_resp,
                               dt_nascimento_resp = x.dt_nascimento_resp,
                               nm_telefone_resp = x.telefoneRespSGF != null ? x.telefoneRespSGF.dc_fone_mail : "",

                               dc_endereco_resp = x.no_tipo_logradouro_resp + ' ' + x.no_localidade_resp + (String.IsNullOrEmpty(x.dc_num_endereco_resp) ? "" : " nº " + x.dc_num_endereco_resp)
                                           + (String.IsNullOrEmpty(x.dc_num_endereco_resp) ? "" : "/" + x.dc_compl_endereco_resp),

                               no_bairro_resp = x.no_bairro_resp,
                               nm_cep_resp = x.nm_cep_resp,

                               dc_cidade_uf_resp = (String.IsNullOrEmpty(x.no_localidade_cidade_resp) ? "" : x.no_localidade_cidade_resp + (String.IsNullOrEmpty(x.sg_estado_resp) ? "" : " - " + x.sg_estado_resp)),
                               vl_saldo_titulo = x.vl_saldo_titulo,
                               qtd_subgrupo_conta = x.qtd_subgrupo_conta,
                               vl_plano_titulo = x.vl_plano_titulo,
                               cd_subgrupo_conta = x.cd_subgrupo_conta,
                               no_subgrupo_conta = x.no_subgrupo_conta,
                               no_turma = x.cd_turma > 0 ? (from t in db.Turma where (t.cd_turma == x.cd_turma || t.cd_turma_ppt == x.cd_turma) select t.no_turma).FirstOrDefault() : "",
                               cd_turma = x.cd_turma,
                           });
                else
                    sql = (from t in sqlTitulo
                                        select new {
                                            nm_escola  = t.Empresa.no_pessoa,
                                            cd_titulo = t.cd_titulo,
                                            nm_titulo = t.nm_titulo,
                                            nm_parcela_titulo = t.nm_parcela_titulo,
                                            dt_emissao_titulo = t.dt_emissao_titulo,
                                            dt_vcto_titulo = t.dt_vcto_titulo,
                                            id_origem_titulo = t.id_origem_titulo,
                                            cd_origem_titulo = t.cd_origem_titulo,
                                            cd_tipo_financeiro = t.cd_tipo_financeiro,
                                            dc_tipo_financeiro = t.TipoFinanceiro.dc_tipo_financeiro,
                                            t.dc_tipo_titulo,
                                            cd_pessoa = t.Pessoa.cd_pessoa,
                                            no_pessoa = t.Pessoa.no_pessoa,
                                            nm_natureza_pessoa = t.Pessoa.nm_natureza_pessoa,
                                            nm_cgc_cpf = t.Pessoa.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                            //db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                            //   db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                                    db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                            db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                                            nm_rg = t.Pessoa.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                            //db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                            //   db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_doc_identidade :
                                                    db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_doc_identidade :
                                            db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_insc_estadual,
                                            dt_nascimento = t.Pessoa.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                            //db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                            //   db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.dt_nascimento :
                                                    db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().dt_nascimento :
                                            db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dt_registro_junta_comercial,
                                            telefoneSGF = t.Pessoa.TelefonePessoa.Where(tp => tp.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE || tp.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault(),//.Pessoa.Telefone.dc_fone_mail,
                                            //dc_endereco
                                            no_tipo_logradouro = t.Pessoa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro,
                                            no_localidade = t.Pessoa.EnderecoPrincipal.Logradouro.no_localidade,
                                            dc_num_endereco = t.Pessoa.EnderecoPrincipal.dc_num_endereco,
                                            dc_compl_endereco = t.Pessoa.EnderecoPrincipal.dc_compl_endereco,
                                                                   
                                            no_bairro_aluno = t.Pessoa.EnderecoPrincipal.Bairro.no_localidade,
                                            nm_cep_aluno = t.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep,

                                            //dc_cidade_uf_aluno
                                            no_localidade_cidade = t.Pessoa.EnderecoPrincipal.Cidade.no_localidade,
                                            sg_estado = t.Pessoa.EnderecoPrincipal.Estado.Estado.sg_estado,

                                            cd_responsavel = t.PessoaResponsavel.cd_pessoa,
                                            nm_responsavel = t.PessoaResponsavel.no_pessoa,
                                            nm_natureza_resp = t.PessoaResponsavel.nm_natureza_pessoa,
                                            nm_cgc_cpf_resp = t.PessoaResponsavel.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                                    db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                            db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                                            nm_rg_resp = t.PessoaResponsavel.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                                    db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_doc_identidade :
                                            db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_insc_estadual,
                                            dt_nascimento_resp = t.PessoaResponsavel.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                                    db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().dt_nascimento :
                                            db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dt_registro_junta_comercial,
                                            telefoneRespSGF = t.PessoaResponsavel.TelefonePessoa.Where(tp => tp.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE || tp.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault(),//.Pessoa.Telefone.dc_fone_mail,
                                            //dc_endereco_resp
                                            no_tipo_logradouro_resp = t.PessoaResponsavel.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro,
                                            no_localidade_resp = t.PessoaResponsavel.EnderecoPrincipal.Logradouro.no_localidade,
                                            dc_num_endereco_resp= t.PessoaResponsavel.EnderecoPrincipal.dc_num_endereco,
                                            dc_compl_endereco_resp = t.PessoaResponsavel.EnderecoPrincipal.dc_compl_endereco,
                                                                   
                                            no_bairro_resp = t.PessoaResponsavel.EnderecoPrincipal.Bairro.no_localidade,
                                            nm_cep_resp = t.PessoaResponsavel.EnderecoPrincipal.Logradouro.dc_num_cep,

                                            //dc_cidade_uf_resp
                                            no_localidade_cidade_resp = t.PessoaResponsavel.EnderecoPrincipal.Cidade.no_localidade,
                                            sg_estado_resp = t.PessoaResponsavel.EnderecoPrincipal.Estado.Estado.sg_estado,
                                            vl_saldo_titulo = t.vl_saldo_titulo,
                                            qtd_subgrupo_conta = t.PlanoTitulo.Count(),
                                            cd_turma = (tipo == (int) Titulo.TipoMovimento.RECEBER && cdTurma > 0) ?
                                                        (from h in db.HistoricoAluno
                                                            where h.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo &&
                                                                  h.cd_contrato == t.cd_origem_titulo &&
                                                                  t.id_origem_titulo == cd_origem &&
                                                                  h.cd_turma == cdTurma &&
                                                                  h.dt_historico <= pDtaBase &&
                                                                  h.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_aluno == h.cd_aluno && han.cd_contrato == h.cd_contrato
                                                                                                                                              && DbFunctions.TruncateTime(han.dt_historico) <= pDtaBase
                                                                  ).Max(x => x.nm_sequencia)
                                                            select h.cd_turma).FirstOrDefault() : 0,
                                        }).ToList().Select(x => new RptReceberPagar {
                                            nm_escola = x.nm_escola,
                                            nm_titulo = x.nm_titulo,
                                            cd_titulo = x.cd_titulo,
                                            nm_parcela_titulo = x.nm_parcela_titulo,
                                            dt_emissao_titulo = x.dt_emissao_titulo,
                                            dt_vcto_titulo = x.dt_vcto_titulo.Date,
                                            id_origem_titulo = x.id_origem_titulo,
                                            cd_origem_titulo = x.cd_origem_titulo,
                                            cd_tipo_financeiro = x.cd_tipo_financeiro,
                                            dc_tipo_financeiro = x.dc_tipo_financeiro,
                                            dc_tipo_titulo = x.dc_tipo_titulo,
                                            cd_pessoa = x.cd_pessoa,
                                            no_pessoa = x.no_pessoa,
                                            nm_natureza_aluno = x.nm_natureza_pessoa,
                                            nm_cgc_cpf = x.nm_cgc_cpf,
                                            nm_rg = x.nm_rg,
                                            dt_nascimento = x.dt_nascimento,
                                            nm_telefone_aluno = x.telefoneSGF != null ? x.telefoneSGF.dc_fone_mail : "",
                                            dc_endereco = x.no_tipo_logradouro + ' ' + x.no_localidade + (String.IsNullOrEmpty(x.dc_num_endereco) ? "" : " nº " + x.dc_num_endereco)
                                                        + (String.IsNullOrEmpty(x.dc_compl_endereco) ? "" : "/" + x.dc_compl_endereco),
                                                                 
                                            no_bairro_aluno = x.no_bairro_aluno,
                                            nm_cep_aluno = x.nm_cep_aluno,

                                            dc_cidade_uf_aluno = (String.IsNullOrEmpty(x.no_localidade) ? "" : x.no_localidade + (String.IsNullOrEmpty(x.sg_estado) ? "" : " - " + x.sg_estado)),

                                            cd_responsavel = x.cd_responsavel,
                                            nm_responsavel = x.nm_responsavel,
                                            nm_natureza_resp = x.nm_natureza_resp,
                                            nm_cgc_cpf_resp = x.nm_cgc_cpf_resp,
                                            nm_rg_resp = x.nm_rg_resp,
                                            dt_nascimento_resp = x.dt_nascimento_resp,
                                            nm_telefone_resp = x.telefoneRespSGF != null ? x.telefoneRespSGF.dc_fone_mail : "", 
                                                                 
                                            dc_endereco_resp =  x.no_tipo_logradouro_resp + ' ' + x.no_localidade_resp + (String.IsNullOrEmpty(x.dc_num_endereco_resp) ? "" : " nº " + x.dc_num_endereco_resp)
                                                        + (String.IsNullOrEmpty(x.dc_num_endereco_resp) ? "" : "/" + x.dc_compl_endereco_resp),

                                            no_bairro_resp = x.no_bairro_resp,
                                            nm_cep_resp = x.nm_cep_resp,

                                            dc_cidade_uf_resp = (String.IsNullOrEmpty(x.no_localidade_cidade_resp) ? "" : x.no_localidade_cidade_resp + (String.IsNullOrEmpty(x.sg_estado_resp) ? "" : " - " + x.sg_estado_resp)),
                                            vl_saldo_titulo = x.vl_saldo_titulo,
                                            qtd_subgrupo_conta = x.qtd_subgrupo_conta,
                                            no_turma = x.cd_turma > 0 ? (from t in db.Turma where (t.cd_turma == x.cd_turma || t.cd_turma_ppt == x.cd_turma) select t.no_turma).FirstOrDefault() : "",
                                            cd_turma = x.cd_turma,
                                        });
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<RptRecebidaPaga> recebidaPagaStoreProcedure(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pForn, byte pNatureza, int pPlanoContas, bool cSegura, int cdTpLiq, int cdTpFinan, int tipo, string situacoes, int cdTurma, bool? ckCancelamento, int cdLocal)
        {
            try
            {
                //IEnumerable<st_RptRecebidaPaga_Result> sql = (from st in db.st_RptRecebidaPaga(cdEscola, pDtaI, pDtaF, pForn, pNatureza, pPlanoContas)
                //                                              select st);

                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                string[] situacao = situacoes.Split('|');
                List<int> cdsSituacoesList = new List<int>();
                for (int i = 0; i < situacao.Count(); i++)
                    cdsSituacoesList.Add(Int32.Parse(situacao[i]));

                var cd_conta_material = (from p in db.Parametro where p.cd_pessoa_escola == cdEscola select p.cd_plano_conta_material).FirstOrDefault();
                var cd_conta_txbco = (from p in db.Parametro where p.cd_pessoa_escola == cdEscola select p.cd_plano_conta_taxbco).FirstOrDefault();

                IQueryable<BaixaTitulo> sqlBaixas;
                IEnumerable<RptRecebidaPaga> sqlTitulos = null;


                if (tipo == (int)Titulo.TipoMovimento.RECEBIDAS && cdTpLiq == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO && ckCancelamento == true)
                {
                    sqlBaixas = from b in db.BaixaTitulo
                                where b.Titulo.cd_pessoa_empresa == cdEscola
                                      && b.Titulo.id_natureza_titulo == pNatureza
                                      //&& DbFunctions.TruncateTime(b.dt_baixa_titulo) >= pDtaI.Date && DbFunctions.TruncateTime(b.dt_baixa_titulo) <= pDtaF.Date
                                      && b.dt_baixa_titulo >= pDtaI.Date && b.dt_baixa_titulo <= pDtaF.Date
                                      && (b.Titulo.cd_pessoa_titulo == pForn || pForn == 0)
                                      && (b.Titulo.PlanoTitulo.Where(tpt => tpt.cd_plano_conta == pPlanoContas).Any() || pPlanoContas == 0)
                                      && ((!cSegura && b.Titulo.PlanoTitulo.Where(p => p.PlanoConta.id_conta_segura == false).Any()) || cSegura)
                                      && (b.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO)
                                      && (b.Titulo.cd_tipo_financeiro == cdTpFinan || cdTpFinan == 0)
                                      && (b.cd_tipo_liquidacao == cdTpLiq)
                                      && (b.cd_local_movto == cdLocal || cdLocal == 0)

                                select b;
                }
                else
                {
                    sqlBaixas = from b in db.BaixaTitulo
                                where b.Titulo.cd_pessoa_empresa == cdEscola
                                      && b.Titulo.id_natureza_titulo == pNatureza
                                      //&& DbFunctions.TruncateTime(b.dt_baixa_titulo) >= pDtaI.Date && DbFunctions.TruncateTime(b.dt_baixa_titulo) <= pDtaF.Date
                                      && b.dt_baixa_titulo >= pDtaI.Date && b.dt_baixa_titulo <= pDtaF.Date
                                      && (b.Titulo.cd_pessoa_titulo == pForn || pForn == 0)
                                      //&& (b.Titulo.PlanoTitulo.Where(tpt => tpt.cd_plano_conta == pPlanoContas).Any() || pPlanoContas == 0)
                                      //&& ((!cSegura && b.Titulo.PlanoTitulo.Where(p => p.PlanoConta.id_conta_segura == false).Any()) || cSegura)
                                      && (db.ContaCorrente.Where(tpt => tpt.cd_baixa_titulo == b.cd_baixa_titulo && tpt.cd_plano_conta == pPlanoContas).Any() || pPlanoContas == 0)
                                      && (!cSegura && db.ContaCorrente.Where(tpt => tpt.cd_baixa_titulo == b.cd_baixa_titulo && tpt.PlanoContas.id_conta_segura == false).Any() || cSegura)
                                      && (b.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO &&
                                          b.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                          b.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.TROCA_FINANCEIRA &&
                                          b.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                                      && (b.Titulo.cd_tipo_financeiro == cdTpFinan || cdTpFinan == 0)
                                      && (b.cd_tipo_liquidacao == cdTpLiq || cdTpLiq == 0)
                                      && (b.cd_local_movto == cdLocal || cdLocal == 0)

                                select b;
                }

                if (tipo == (int)Titulo.TipoMovimento.RECEBIDAS)
                {
                    if (cdTurma > 0)
                    {

                        sqlBaixas = from b in sqlBaixas
                                    where
                                     (from h in db.HistoricoAluno
                                      where h.Aluno.cd_pessoa_aluno == b.Titulo.cd_pessoa_titulo &&
                                           h.cd_contrato == b.Titulo.cd_origem_titulo &&
                                           b.Titulo.id_origem_titulo == cd_origem &&
                                           h.dt_historico <= pDtaF &&
                                           h.cd_turma == cdTurma &&
                                           h.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_aluno == h.cd_aluno && han.cd_contrato == h.cd_contrato
                                                                                                                       && DbFunctions.TruncateTime(han.dt_historico) <= pDtaF
                                           ).Max(x => x.nm_sequencia)
                                      select h.cd_turma).Any()
                                    select b;
                    }

                    if (!cdsSituacoesList.Contains(100) && cdsSituacoesList.Count() > 0)
                    {
                        sqlBaixas = from b in sqlBaixas
                                    where cdsSituacoesList.Contains((from histA in db.HistoricoAluno
                                                                     where histA.Aluno.cd_pessoa_escola == cdEscola &&
                                                                           histA.Aluno.cd_pessoa_aluno == b.Titulo.cd_pessoa_titulo &&
                                                                           histA.dt_historico <= pDtaF
                                                                     orderby histA.nm_sequencia descending
                                                                     select histA.id_situacao_historico).FirstOrDefault())
                                    select b;
                    }

                }

                if (tipo == (int)Titulo.TipoMovimento.RECEBIDAS && cdTpLiq == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO && ckCancelamento == true)
                {
                    sqlTitulos = (from b in sqlBaixas
                                  select new
                                  {
                                      cd_pessoa_empresa = b.Titulo.cd_pessoa_empresa,
                                      cd_baixa_titulo = b.cd_baixa_titulo,
                                      nm_escola = b.Titulo.Empresa.no_pessoa,
                                      nm_titulo = b.Titulo.nm_titulo,
                                      nm_parcela_titulo = b.Titulo.nm_parcela_titulo,
                                      dt_emissao_titulo = b.Titulo.dt_emissao_titulo,
                                      dt_vcto_titulo = b.Titulo.dt_vcto_titulo,
                                      dc_tipo_financeiro = b.Titulo.TipoFinanceiro.dc_tipo_financeiro,
                                      cd_pessoa = b.Titulo.Pessoa.cd_pessoa,
                                      no_pessoa = b.Titulo.Pessoa.no_pessoa,

                                      cd_responsavel = b.Titulo.PessoaResponsavel.cd_pessoa,
                                      nm_responsavel = b.Titulo.PessoaResponsavel.no_pessoa,
                                      cpf_responsavel = (b.Titulo.PessoaResponsavel.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                                                      db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                                                          db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf + "" :
                                                                          (db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf != null ?
                                                                              db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf : "") : ""),

                                      vl_saldo_titulo = b.Titulo.vl_saldo_titulo,

                                      cd_subgrupo_conta = 0,
                                      no_subgrupo_conta = "",
                                      vl_plano_titulo = b.vl_liquidacao_baixa,
                                      dc_origem = b.Titulo.Origem.dc_origem,
                                      vl_titulo = b.Titulo.vl_titulo,
                                      dt_baixa_titulo = b.dt_baixa_titulo,
                                      vl_principal_baixa = b.vl_principal_baixa,
                                      vl_juros_baixa = (b.vl_juros_baixa - b.vl_desc_juros_baixa),
                                      vl_multa_baixa = (b.vl_multa_baixa - b.vl_desc_multa_baixa),
                                      vl_desconto_baixa = b.vl_desconto_baixa,
                                      vl_liquidacao_baixa = b.vl_liquidacao_baixa,
                                      pc_taxa_cartao = b.Titulo.pc_taxa_cartao,
                                      nm_dias_cartao = b.Titulo.nm_dias_cartao,
                                      dc_tipo_liquidacao = b.TipoLiquidacao.dc_tipo_liquidacao,
                                      nm_recibo = b.nm_recibo,
                                      cd_turma =
                                                                        (tipo == (int)Titulo.TipoMovimento.RECEBIDAS && cdTurma > 0) ?
                                                                             (from h in db.HistoricoAluno
                                                                              where h.Aluno.cd_pessoa_aluno == b.Titulo.cd_pessoa_titulo &&
                                                                                    h.cd_contrato == b.Titulo.cd_origem_titulo &&
                                                                                    b.Titulo.id_origem_titulo == cd_origem &&
                                                                                    h.cd_turma == cdTurma &&
                                                                                    h.dt_historico <= pDtaF &&
                                                                                    h.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_aluno == h.cd_aluno && han.cd_contrato == h.cd_contrato
                                                                                                                                                                && DbFunctions.TruncateTime(han.dt_historico) <= pDtaF
                                                                                    ).Max(x => x.nm_sequencia)
                                                                              select h.cd_turma).FirstOrDefault() : 0,
                                      no_usuario_baixa = (tipo == (int)Titulo.TipoMovimento.RECEBIDAS && cdTpLiq == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO && ckCancelamento == true) ? b.SysUsuario.no_login : "",
                                      tx_obs_baixa = b.tx_obs_baixa,
                                      nm_tipo_local = b.LocalMovimento != null ? b.LocalMovimento.nm_tipo_local : 0,
                                      no_local_movto = b.LocalMovimento != null ? b.LocalMovimento.no_local_movto : "",
                                      nm_agencia = b.LocalMovimento != null ? b.LocalMovimento.nm_agencia : "",
                                      nm_conta_corrente = b.LocalMovimento != null ? b.LocalMovimento.nm_conta_corrente : "",
                                      nm_digito_conta_corrente = b.LocalMovimento != null ? b.LocalMovimento.nm_digito_conta_corrente : ""
                                  }).ToList().Select(x => new RptRecebidaPaga
                                  {
                                      cd_pessoa_empresa = x.cd_pessoa_empresa,
                                      cd_baixa_titulo = x.cd_baixa_titulo,
                                      nm_escola = x.nm_escola,
                                      nm_titulo = x.nm_titulo,
                                      nm_parcela_titulo = x.nm_parcela_titulo,
                                      dt_emissao_titulo = x.dt_emissao_titulo,
                                      dt_vcto_titulo = x.dt_vcto_titulo.Date,
                                      dc_tipo_financeiro = x.dc_tipo_financeiro,
                                      cd_pessoa = x.cd_pessoa,
                                      no_pessoa = x.no_pessoa,
                                      pc_taxa_cartao = x.pc_taxa_cartao,
                                      nm_dias_cartao = x.nm_dias_cartao,

                                      cd_responsavel = x.cd_responsavel,
                                      nm_responsavel = x.nm_responsavel,
                                      cpf_responsavel = x.cpf_responsavel,
                                      vl_saldo_titulo = x.vl_saldo_titulo,
                                      cd_subgrupo_conta = x.cd_subgrupo_conta,
                                      no_subgrupo_conta = x.no_subgrupo_conta,
                                      vl_plano_titulo = x.vl_plano_titulo,

                                      dc_origem = x.dc_origem,
                                      vl_titulo = x.vl_plano_titulo,
                                      dt_baixa_titulo = x.dt_baixa_titulo.Date,
                                      vl_principal_baixa = x.vl_principal_baixa,
                                      vl_juros_baixa = Math.Round(x.vl_juros_baixa, 2, MidpointRounding.AwayFromZero),
                                      vl_multa_baixa = Math.Round(x.vl_multa_baixa, 2, MidpointRounding.AwayFromZero),
                                      vl_desconto_baixa = Math.Round(x.vl_desconto_baixa, 2, MidpointRounding.AwayFromZero),
                                      vl_liquidacao_baixa = x.vl_liquidacao_baixa,
                                      dc_tipo_liquidacao = x.dc_tipo_liquidacao,
                                      nm_recibo = x.nm_recibo,
                                      no_turma = x.cd_turma > 0 ? (from t in db.Turma where (t.cd_turma == x.cd_turma || t.cd_turma_ppt == x.cd_turma) select t.no_turma).FirstOrDefault() : "",
                                      cd_turma = x.cd_turma,
                                      no_usuario_baixa = x.no_usuario_baixa,
                                      tx_obs_baixa = x.tx_obs_baixa,
                                      no_local = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto
                                  }).AsQueryable();

                }
                if (cdTpLiq != (int)TipoLiquidacao.TipoLiqui.DESCONTO_FOLHA_PAGAMENTO && !(cdTpLiq == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO && ckCancelamento == true))
                {
                    sqlTitulos = (from b in sqlBaixas
                                      //join plano_titulo in db.ContaCorrente on b.cd_baixa_titulo equals plano_titulo.cd_baixa_titulo into Inners
                                      //from pt in Inners.DefaultIfEmpty()
                                  join pt in db.ContaCorrente on b.cd_baixa_titulo equals pt.cd_baixa_titulo
                                  where ((pt.cd_plano_conta == pPlanoContas || pPlanoContas == 0) &&
                                       !db.ContaCorrente.Any(cc => cc.cd_baixa_titulo == b.cd_baixa_titulo && cc.cd_plano_conta == cd_conta_material && b.Titulo.id_origem_titulo == cd_origem) &&
                                       (cdTpFinan != 5 || (pt.cd_plano_conta != cd_conta_txbco)))
                                  select new
                                  {
                                      cd_pessoa_empresa = b.Titulo.cd_pessoa_empresa,
                                      cd_baixa_titulo = b.cd_baixa_titulo,
                                      nm_escola = b.Titulo.Empresa.no_pessoa,
                                      nm_titulo = b.Titulo.nm_titulo,
                                      nm_parcela_titulo = b.Titulo.nm_parcela_titulo,
                                      dt_emissao_titulo = b.Titulo.dt_emissao_titulo,
                                      dt_vcto_titulo = b.Titulo.dt_vcto_titulo,
                                      dc_tipo_financeiro = b.Titulo.TipoFinanceiro.dc_tipo_financeiro,
                                      cd_pessoa = b.Titulo.Pessoa.cd_pessoa,
                                      no_pessoa = b.Titulo.Pessoa.no_pessoa,

                                      cd_responsavel = b.Titulo.PessoaResponsavel.cd_pessoa,
                                      nm_responsavel = b.Titulo.PessoaResponsavel.no_pessoa,
                                      cpf_responsavel = (b.Titulo.PessoaResponsavel.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                          db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                              db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf + "" :
                                              (db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf != null ?
                                                  db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf : "") : ""),

                                      vl_saldo_titulo = b.Titulo.vl_titulo > 0 && pt.vl_conta_corrente > 0 ? b.Titulo.vl_saldo_titulo / b.Titulo.vl_titulo * (decimal)pt.vl_conta_corrente : b.Titulo.vl_saldo_titulo,

                                      cd_subgrupo_conta = pt.PlanoContas != null ? pt.PlanoContas.PlanoContaSubgrupo.cd_subgrupo_conta : 0,
                                      no_subgrupo_conta = pt.PlanoContas != null ? pt.PlanoContas.PlanoContaSubgrupo.no_subgrupo_conta : "",
                                      vl_plano_titulo = pt.vl_conta_corrente,
                                      dc_origem = b.Titulo.Origem.dc_origem,
                                      vl_titulo = b.Titulo.vl_titulo,
                                      dt_baixa_titulo = b.dt_baixa_titulo,
                                      vl_principal_baixa = pt.cd_plano_conta == cd_conta_txbco ? pt.vl_conta_corrente : b.vl_principal_baixa - b.Titulo.vl_material_titulo,
                                      vl_juros_baixa = pt.cd_plano_conta == cd_conta_txbco ? 0 : b.Titulo.vl_titulo > 0 && pt.vl_conta_corrente > 0 ? (b.vl_juros_baixa - b.vl_desc_juros_baixa) * ((decimal)pt.vl_conta_corrente / b.vl_liquidacao_baixa) : (b.vl_juros_baixa - b.vl_desc_juros_baixa),
                                      vl_multa_baixa = pt.cd_plano_conta == cd_conta_txbco ? 0 : b.Titulo.vl_titulo > 0 && pt.vl_conta_corrente > 0 ? (b.vl_multa_baixa - b.vl_desc_multa_baixa) * ((decimal)pt.vl_conta_corrente / b.vl_liquidacao_baixa) : (b.vl_multa_baixa - b.vl_desc_multa_baixa),
                                      vl_desconto_baixa = pt.cd_plano_conta == cd_conta_txbco ? 0 : b.Titulo.vl_titulo > 0 && pt.vl_conta_corrente > 0 ? b.vl_desconto_baixa * ((decimal)pt.vl_conta_corrente / b.vl_liquidacao_baixa) : b.vl_desconto_baixa,
                                      vl_liquidacao_baixa = (decimal)pt.vl_conta_corrente, //b.Titulo.vl_titulo > 0 && pt.vl_conta_corrente > 0 ? b.vl_liquidacao_baixa / b.Titulo.vl_titulo * (decimal)pt.vl_conta_corrente : b.vl_liquidacao_baixa,
                                      pc_taxa_cartao = b.Titulo.pc_taxa_cartao,
                                      nm_dias_cartao = b.Titulo.nm_dias_cartao,
                                      dc_tipo_liquidacao = b.TipoLiquidacao.dc_tipo_liquidacao,
                                      nm_recibo = b.nm_recibo,
                                      cd_turma =
                                            (tipo == (int)Titulo.TipoMovimento.RECEBIDAS && cdTurma > 0) ?
                                                 (from h in db.HistoricoAluno
                                                  where h.Aluno.cd_pessoa_aluno == b.Titulo.cd_pessoa_titulo &&
                                                        h.cd_contrato == b.Titulo.cd_origem_titulo &&
                                                        b.Titulo.id_origem_titulo == cd_origem &&
                                                        h.cd_turma == cdTurma &&
                                                        h.dt_historico <= pDtaF &&
                                                        h.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_aluno == h.cd_aluno && han.cd_contrato == h.cd_contrato
                                                                                                                                    && DbFunctions.TruncateTime(han.dt_historico) <= pDtaF
                                                        ).Max(x => x.nm_sequencia)
                                                  select h.cd_turma).FirstOrDefault() : 0,
                                      no_usuario_baixa = (tipo == (int)Titulo.TipoMovimento.RECEBIDAS && cdTpLiq == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO && ckCancelamento == true) ? b.SysUsuario.no_login : "",
                                      tx_obs_baixa = (tipo == (int)Titulo.TipoMovimento.RECEBIDAS && cdTpLiq == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO && ckCancelamento == true) ? b.tx_obs_baixa : pt.dc_obs_conta_corrente,
                                      nm_tipo_local = b.LocalMovimento.nm_tipo_local,
                                      no_local_movto = b.LocalMovimento.no_local_movto,
                                      nm_agencia = b.LocalMovimento.nm_agencia,
                                      nm_conta_corrente = b.LocalMovimento.nm_conta_corrente,
                                      nm_digito_conta_corrente = b.LocalMovimento.nm_digito_conta_corrente

                                  }).ToList().Select(x => new RptRecebidaPaga
                                  {
                                      cd_pessoa_empresa = x.cd_pessoa_empresa,
                                      cd_baixa_titulo = x.cd_baixa_titulo,
                                      nm_escola = x.nm_escola,
                                      nm_titulo = x.nm_titulo,
                                      nm_parcela_titulo = x.nm_parcela_titulo,
                                      dt_emissao_titulo = x.dt_emissao_titulo,
                                      dt_vcto_titulo = x.dt_vcto_titulo.Date,
                                      dc_tipo_financeiro = x.dc_tipo_financeiro,
                                      cd_pessoa = x.cd_pessoa,
                                      no_pessoa = x.no_pessoa,
                                      pc_taxa_cartao = x.pc_taxa_cartao,
                                      nm_dias_cartao = x.nm_dias_cartao,

                                      cd_responsavel = x.cd_responsavel,
                                      nm_responsavel = x.nm_responsavel,
                                      cpf_responsavel = x.cpf_responsavel,
                                      vl_saldo_titulo = x.vl_saldo_titulo,
                                      cd_subgrupo_conta = x.cd_subgrupo_conta,
                                      no_subgrupo_conta = x.no_subgrupo_conta,
                                      vl_plano_titulo = x.vl_plano_titulo,

                                      dc_origem = x.dc_origem,
                                      vl_titulo = x.vl_plano_titulo,
                                      dt_baixa_titulo = x.dt_baixa_titulo.Date,
                                      vl_principal_baixa = x.vl_principal_baixa,
                                      vl_juros_baixa = Math.Round(x.vl_juros_baixa, 2, MidpointRounding.AwayFromZero),
                                      vl_multa_baixa = Math.Round(x.vl_multa_baixa, 2, MidpointRounding.AwayFromZero),
                                      vl_desconto_baixa = Math.Round(x.vl_desconto_baixa, 2, MidpointRounding.AwayFromZero),
                                      vl_liquidacao_baixa = x.vl_liquidacao_baixa,
                                      dc_tipo_liquidacao = x.dc_tipo_liquidacao,
                                      nm_recibo = x.nm_recibo,
                                      no_turma = x.cd_turma > 0 ? (from t in db.Turma where (t.cd_turma == x.cd_turma || t.cd_turma_ppt == x.cd_turma) select t.no_turma).FirstOrDefault() : "",
                                      cd_turma = x.cd_turma,
                                      no_usuario_baixa = x.no_usuario_baixa,
                                      tx_obs_baixa = x.tx_obs_baixa,
                                      no_local = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto
                                  }).AsQueryable().Union(

                                                 (from b in sqlBaixas
                                                      //join plano_titulo in db.PlanoTitulo on b.cd_titulo equals plano_titulo.cd_titulo into Inners
                                                      //from pt in Inners.DefaultIfEmpty()
                                                  join pt in db.ContaCorrente on b.cd_baixa_titulo equals pt.cd_baixa_titulo //into Inners
                                                                                                                             //from pt in Inners.DefaultIfEmpty()
                                                  where (pt.cd_plano_conta == pPlanoContas || pPlanoContas == 0) &&
                                                       db.ContaCorrente.Any(cc => cc.cd_baixa_titulo == b.cd_baixa_titulo && cc.cd_plano_conta == cd_conta_material) &&
                                                       b.Titulo.id_origem_titulo == cd_origem
                                                  select new
                                                  {
                                                      cd_pessoa_empresa = b.Titulo.cd_pessoa_empresa,
                                                      cd_baixa_titulo = b.cd_baixa_titulo,
                                                      nm_escola = b.Titulo.Empresa.no_pessoa,
                                                      nm_titulo = b.Titulo.nm_titulo,
                                                      nm_parcela_titulo = b.Titulo.nm_parcela_titulo,
                                                      dt_emissao_titulo = b.Titulo.dt_emissao_titulo,
                                                      dt_vcto_titulo = b.Titulo.dt_vcto_titulo,
                                                      dc_tipo_financeiro = b.Titulo.TipoFinanceiro.dc_tipo_financeiro,
                                                      cd_pessoa = b.Titulo.Pessoa.cd_pessoa,
                                                      no_pessoa = b.Titulo.Pessoa.no_pessoa,

                                                      cd_responsavel = b.Titulo.PessoaResponsavel.cd_pessoa,
                                                      nm_responsavel = b.Titulo.PessoaResponsavel.no_pessoa,
                                                      cpf_responsavel = (b.Titulo.PessoaResponsavel.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                                          db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                                              db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf + "" :
                                                              (db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf != null ?
                                                                  db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf : "") : ""),

                                                      vl_saldo_titulo = b.Titulo.vl_saldo_titulo,

                                                      cd_subgrupo_conta = pt.PlanoContas != null ? pt.PlanoContas.PlanoContaSubgrupo.cd_subgrupo_conta : 0,
                                                      no_subgrupo_conta = pt.PlanoContas != null ? pt.PlanoContas.PlanoContaSubgrupo.no_subgrupo_conta : "",
                                                      vl_plano_titulo = pt.vl_conta_corrente,

                                                      dc_origem = b.Titulo.Origem.dc_origem,
                                                      vl_titulo = b.Titulo.vl_titulo,
                                                      dt_baixa_titulo = b.dt_baixa_titulo,
                                                      vl_principal_baixa = pt.cd_plano_conta == cd_conta_material ? b.Titulo.vl_material_titulo : b.vl_principal_baixa - b.Titulo.vl_material_titulo,
                                                      vl_juros_baixa = pt.cd_plano_conta == cd_conta_material ? 0 : b.vl_juros_baixa - b.vl_desc_juros_baixa,
                                                      vl_multa_baixa = pt.cd_plano_conta == cd_conta_material ? 0 : b.vl_multa_baixa - b.vl_desc_multa_baixa,
                                                      vl_desconto_baixa = pt.cd_plano_conta == cd_conta_material ? 0 : b.vl_desconto_baixa,
                                                      vl_liquidacao_baixa = (decimal)pt.vl_conta_corrente,
                                                      pc_taxa_cartao = b.Titulo.pc_taxa_cartao,
                                                      nm_dias_cartao = b.Titulo.nm_dias_cartao,
                                                      dc_tipo_liquidacao = b.TipoLiquidacao.dc_tipo_liquidacao,
                                                      nm_recibo = b.nm_recibo,
                                                      cd_turma = (tipo == (int)Titulo.TipoMovimento.RECEBIDAS && cdTurma > 0) ?
                                                                 (from h in db.HistoricoAluno
                                                                  where h.Aluno.cd_pessoa_aluno == b.Titulo.cd_pessoa_titulo &&
                                                                           h.cd_contrato == b.Titulo.cd_origem_titulo &&
                                                                           b.Titulo.id_origem_titulo == cd_origem &&
                                                                           h.cd_turma == cdTurma &&
                                                                           h.dt_historico <= pDtaF &&
                                                                           h.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_aluno == h.cd_aluno && han.cd_contrato == h.cd_contrato
                                                                                                                                                       && DbFunctions.TruncateTime(han.dt_historico) <= pDtaF
                                                                           ).Max(x => x.nm_sequencia)
                                                                  select h.cd_turma).FirstOrDefault() : 0,
                                                      no_usuario_baixa = (tipo == (int)Titulo.TipoMovimento.RECEBIDAS && cdTpLiq == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO && ckCancelamento == true) ? b.SysUsuario.no_login : "",
                                                      tx_obs_baixa = (tipo == (int)Titulo.TipoMovimento.RECEBIDAS && cdTpLiq == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO && ckCancelamento == true) ? b.tx_obs_baixa : pt.dc_obs_conta_corrente,
                                                      nm_tipo_local = b.LocalMovimento.nm_tipo_local,
                                                      no_local_movto = b.LocalMovimento.no_local_movto,
                                                      nm_agencia = b.LocalMovimento.nm_agencia,
                                                      nm_conta_corrente = b.LocalMovimento.nm_conta_corrente,
                                                      nm_digito_conta_corrente = b.LocalMovimento.nm_digito_conta_corrente

                                                  }).ToList().Select(x => new RptRecebidaPaga
                                                  {
                                                      cd_pessoa_empresa = x.cd_pessoa_empresa,
                                                      cd_baixa_titulo = x.cd_baixa_titulo,
                                                      nm_escola = x.nm_escola,
                                                      nm_titulo = x.nm_titulo,
                                                      nm_parcela_titulo = x.nm_parcela_titulo,
                                                      dt_emissao_titulo = x.dt_emissao_titulo,
                                                      dt_vcto_titulo = x.dt_vcto_titulo.Date,
                                                      dc_tipo_financeiro = x.dc_tipo_financeiro,
                                                      cd_pessoa = x.cd_pessoa,
                                                      no_pessoa = x.no_pessoa,
                                                      pc_taxa_cartao = x.pc_taxa_cartao,
                                                      nm_dias_cartao = x.nm_dias_cartao,

                                                      cd_responsavel = x.cd_responsavel,
                                                      nm_responsavel = x.nm_responsavel,
                                                      cpf_responsavel = x.cpf_responsavel,
                                                      vl_saldo_titulo = x.vl_saldo_titulo,
                                                      cd_subgrupo_conta = x.cd_subgrupo_conta,
                                                      no_subgrupo_conta = x.no_subgrupo_conta,
                                                      vl_plano_titulo = x.vl_plano_titulo,

                                                      dc_origem = x.dc_origem,
                                                      vl_titulo = x.vl_plano_titulo,
                                                      dt_baixa_titulo = x.dt_baixa_titulo.Date,
                                                      vl_principal_baixa = x.vl_principal_baixa,
                                                      vl_juros_baixa = x.vl_juros_baixa,
                                                      vl_multa_baixa = x.vl_multa_baixa,
                                                      vl_desconto_baixa = x.vl_desconto_baixa,
                                                      vl_liquidacao_baixa = x.vl_liquidacao_baixa,
                                                      dc_tipo_liquidacao = x.dc_tipo_liquidacao,
                                                      nm_recibo = x.nm_recibo,
                                                      no_turma = x.cd_turma > 0 ? (from t in db.Turma where (t.cd_turma == x.cd_turma || t.cd_turma_ppt == x.cd_turma) select t.no_turma).FirstOrDefault() : "",
                                                      cd_turma = x.cd_turma,
                                                      no_usuario_baixa = x.no_usuario_baixa,
                                                      tx_obs_baixa = x.tx_obs_baixa,
                                                      no_local = x.nm_tipo_local == (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO ? x.no_local_movto + " | ag.:" + x.nm_agencia + " | c/c:" + x.nm_conta_corrente + "-" + x.nm_digito_conta_corrente : x.no_local_movto
                                                  }));
                }
                if (cdTpLiq == (int)TipoLiquidacao.TipoLiqui.DESCONTO_FOLHA_PAGAMENTO)
                {
                    sqlTitulos = (from b in sqlBaixas
                                  select new
                                  {
                                      cd_pessoa_empresa = b.Titulo.cd_pessoa_empresa,
                                      cd_baixa_titulo = b.cd_baixa_titulo,
                                      nm_escola = b.Titulo.Empresa.no_pessoa,
                                      nm_titulo = b.Titulo.nm_titulo,
                                      nm_parcela_titulo = b.Titulo.nm_parcela_titulo,
                                      dt_emissao_titulo = b.Titulo.dt_emissao_titulo,
                                      dt_vcto_titulo = b.Titulo.dt_vcto_titulo,
                                      dc_tipo_financeiro = b.Titulo.TipoFinanceiro.dc_tipo_financeiro,
                                      cd_pessoa = b.Titulo.Pessoa.cd_pessoa,
                                      no_pessoa = b.Titulo.Pessoa.no_pessoa,

                                      cd_responsavel = b.Titulo.PessoaResponsavel.cd_pessoa,
                                      nm_responsavel = b.Titulo.PessoaResponsavel.no_pessoa,
                                      cpf_responsavel = (b.Titulo.PessoaResponsavel.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                          db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                              db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf + "" :
                                              (db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf != null ?
                                                  db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == b.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf : "") : ""),

                                      vl_saldo_titulo = b.Titulo.vl_saldo_titulo,

                                      cd_subgrupo_conta = 0,
                                      no_subgrupo_conta = "",
                                      vl_plano_titulo = b.Titulo.vl_titulo,
                                      dc_origem = b.Titulo.Origem.dc_origem,
                                      vl_titulo = b.Titulo.vl_titulo,
                                      dt_baixa_titulo = b.dt_baixa_titulo,
                                      vl_principal_baixa = b.vl_principal_baixa - b.Titulo.vl_material_titulo,
                                      vl_juros_baixa = (b.vl_juros_baixa - b.vl_desc_juros_baixa),
                                      vl_multa_baixa = (b.vl_multa_baixa - b.vl_desc_multa_baixa),
                                      vl_desconto_baixa = b.vl_desconto_baixa,
                                      vl_liquidacao_baixa = b.vl_liquidacao_baixa,
                                      pc_taxa_cartao = 0,
                                      nm_dias_cartao = 0,
                                      dc_tipo_liquidacao = b.TipoLiquidacao.dc_tipo_liquidacao,
                                      nm_recibo = b.nm_recibo,
                                      cd_turma =
                                            (tipo == (int)Titulo.TipoMovimento.RECEBIDAS && cdTurma > 0) ?
                                                 (from h in db.HistoricoAluno
                                                  where h.Aluno.cd_pessoa_aluno == b.Titulo.cd_pessoa_titulo &&
                                                        h.cd_contrato == b.Titulo.cd_origem_titulo &&
                                                        b.Titulo.id_origem_titulo == cd_origem &&
                                                        h.cd_turma == cdTurma &&
                                                        h.dt_historico <= pDtaF &&
                                                        h.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_aluno == h.cd_aluno && han.cd_contrato == h.cd_contrato
                                                                                                                                    && DbFunctions.TruncateTime(han.dt_historico) <= pDtaF
                                                        ).Max(x => x.nm_sequencia)
                                                  select h.cd_turma).FirstOrDefault() : 0,
                                      no_usuario_baixa = (tipo == (int)Titulo.TipoMovimento.RECEBIDAS && cdTpLiq == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO && ckCancelamento == true) ? b.SysUsuario.no_login : "",
                                      tx_obs_baixa = b.tx_obs_baixa,
                                      nm_tipo_local = 0,
                                      no_local_movto = b.LocalMovimento.no_local_movto,
                                      nm_agencia = b.LocalMovimento.nm_agencia,
                                      nm_conta_corrente = b.LocalMovimento.nm_conta_corrente,
                                      nm_digito_conta_corrente = b.LocalMovimento.nm_digito_conta_corrente

                                  }).ToList().Select(x => new RptRecebidaPaga
                                  {
                                      cd_pessoa_empresa = x.cd_pessoa_empresa,
                                      cd_baixa_titulo = x.cd_baixa_titulo,
                                      nm_escola = x.nm_escola,
                                      nm_titulo = x.nm_titulo,
                                      nm_parcela_titulo = x.nm_parcela_titulo,
                                      dt_emissao_titulo = x.dt_emissao_titulo,
                                      dt_vcto_titulo = x.dt_vcto_titulo.Date,
                                      dc_tipo_financeiro = x.dc_tipo_financeiro,
                                      cd_pessoa = x.cd_pessoa,
                                      no_pessoa = x.no_pessoa,
                                      pc_taxa_cartao = x.pc_taxa_cartao,
                                      nm_dias_cartao = x.nm_dias_cartao,

                                      cd_responsavel = x.cd_responsavel,
                                      nm_responsavel = x.nm_responsavel,
                                      cpf_responsavel = x.cpf_responsavel,
                                      vl_saldo_titulo = x.vl_saldo_titulo,
                                      cd_subgrupo_conta = x.cd_subgrupo_conta,
                                      no_subgrupo_conta = x.no_subgrupo_conta,
                                      vl_plano_titulo = x.vl_plano_titulo,

                                      dc_origem = x.dc_origem,
                                      vl_titulo = x.vl_plano_titulo,
                                      dt_baixa_titulo = x.dt_baixa_titulo.Date,
                                      vl_principal_baixa = x.vl_principal_baixa,
                                      vl_juros_baixa = Math.Round(x.vl_juros_baixa, 2, MidpointRounding.AwayFromZero),
                                      vl_multa_baixa = Math.Round(x.vl_multa_baixa, 2, MidpointRounding.AwayFromZero),
                                      vl_desconto_baixa = Math.Round(x.vl_desconto_baixa, 2, MidpointRounding.AwayFromZero),
                                      vl_liquidacao_baixa = x.vl_liquidacao_baixa,
                                      dc_tipo_liquidacao = x.dc_tipo_liquidacao,
                                      nm_recibo = x.nm_recibo,
                                      no_turma = x.cd_turma > 0 ? (from t in db.Turma where (t.cd_turma == x.cd_turma || t.cd_turma_ppt == x.cd_turma) select t.no_turma).FirstOrDefault() : "",
                                      cd_turma = x.cd_turma,
                                      no_usuario_baixa = x.no_usuario_baixa,
                                      tx_obs_baixa = x.tx_obs_baixa,
                                      no_local = x.no_local_movto
                                  });
                }
                    return sqlTitulos;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ObservacaoBaixaUI> getObservacoesBaixaCancelamento(int cdEscola, int cd_baixa_titulo)
        {
            try
            {
               
                 IEnumerable<ObservacaoBaixaUI> sql = (from b in db.BaixaTitulo
                        where b.cd_baixa_titulo == cd_baixa_titulo &&
                              b.Titulo.cd_pessoa_empresa == cdEscola
                        select new
                        {
                            tx_obs_baixa = b.tx_obs_baixa,
                            tx_no_pessoa_obs_baixa = b.Titulo.Pessoa.no_pessoa,
                            dt_vencimento_obs_baixa = b.Titulo.dt_vcto_titulo,
                            numero_titulo_obs_baixa = b.Titulo.nm_titulo,
                            numero_parcela_titulo_obs_baixa = b.Titulo.nm_parcela_titulo
                            
                        }).ToList().Select(x => new ObservacaoBaixaUI
                        {
                            tx_obs_baixa = x.tx_obs_baixa,
                            tx_no_pessoa_obs_baixa = x.tx_no_pessoa_obs_baixa,
                            dt_vencimento_obs_baixa = x.dt_vencimento_obs_baixa,
                            numero_titulo_obs_baixa = x.numero_titulo_obs_baixa,
                            numero_parcela_titulo_obs_baixa = x.numero_parcela_titulo_obs_baixa
                        });
               

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Titulo getTituloAbertoByAditamento(int cd_contrato)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                Titulo sql = ((from i in db.Titulo
                               join t in db.TituloAditamento on i.cd_titulo equals t.cd_titulo 
                               where //i.cd_origem_titulo == cd_contrato &&
                                    t.cd_aditamento == cd_contrato && 
                                    i.id_origem_titulo == cd_origem &&
                                     //i.nm_parcela_titulo == 1  //LBM  liberando para 
                                     (i.dc_tipo_titulo == "AA" || i.dc_tipo_titulo == "AD")
                               select new
                               {
                                   cd_titulo = i.cd_titulo,
                                   vl_titulo = i.vl_titulo,
                                   id_origem_titulo = i.id_origem_titulo,
                                   id_natureza_titulo = i.id_natureza_titulo,
                                   nm_parcela_titulo = i.nm_parcela_titulo,
                                   dt_vcto_titulo = i.dt_vcto_titulo,
                                   cd_pessoa_empresa = i.cd_pessoa_empresa,
                                   vl_saldo_titulo = i.vl_saldo_titulo,
                                   dc_tipo_titulo = i.dc_tipo_titulo,
                                   i.vl_material_titulo
                               }).ToList().Select(x => new Titulo
                               {
                                   cd_titulo = x.cd_titulo,
                                   vl_titulo = x.vl_titulo,
                                   id_natureza_titulo = x.id_natureza_titulo,
                                   id_origem_titulo = x.id_origem_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   cd_pessoa_empresa = x.cd_pessoa_empresa,
                                   vl_saldo_titulo = x.vl_saldo_titulo,
                                   dc_tipo_titulo = x.dc_tipo_titulo,
                                   vl_material_titulo = x.vl_material_titulo
                               })).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Titulo getTituloByContrato(int cd_contrato, int nro_parcela) {
            try {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                Titulo sql = ((from i in db.Titulo
                               where i.cd_origem_titulo == cd_contrato &&
                                     i.id_origem_titulo == cd_origem &&
                                     i.nm_parcela_titulo == nro_parcela
                                     && i.dc_tipo_titulo != "TM" && i.dc_tipo_titulo != "TA"
                                     && i.dc_tipo_titulo != "AA" && i.dc_tipo_titulo != "AD"
                               select new
                               {
                                   cd_titulo = i.cd_titulo,
                                   vl_titulo = i.vl_titulo,
                                   id_origem_titulo = i.id_origem_titulo,
                                   id_natureza_titulo = i.id_natureza_titulo,
                                   nm_parcela_titulo = i.nm_parcela_titulo,
                                   dt_vcto_titulo = i.dt_vcto_titulo,
                                   cd_pessoa_empresa = i.cd_pessoa_empresa,
                                   vl_saldo_titulo = i.vl_saldo_titulo,
                                   dc_tipo_titulo = i.dc_tipo_titulo,
                                   i.vl_material_titulo,
                                   i.vl_taxa_cartao,
                                   i.pc_taxa_cartao,
                                   i.pc_bolsa,
                                   db.Contrato.Where(x => x.cd_pessoa_escola == i.cd_pessoa_empresa && x.cd_contrato == i.cd_origem_titulo && i.id_origem_titulo == cd_origem).FirstOrDefault().pc_desconto_bolsa,

                               }).ToList().Select(x => new Titulo {
                                   cd_titulo = x.cd_titulo,
                                   vl_titulo = x.vl_titulo,
                                   id_natureza_titulo = x.id_natureza_titulo,
                                   id_origem_titulo = x.id_origem_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   cd_pessoa_empresa = x.cd_pessoa_empresa,
                                   vl_saldo_titulo = x.vl_saldo_titulo,
                                   dc_tipo_titulo = x.dc_tipo_titulo,
                                   vl_material_titulo = x.vl_material_titulo,
                                   pc_bolsa = x.pc_bolsa,
                                   vl_taxa_cartao = x.vl_taxa_cartao,
                                   pc_taxa_cartao = x.pc_taxa_cartao
                               })).FirstOrDefault();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public bool getTituloBaixadoContrato(int cd_contrato, int cdEscola, int cdTitulo)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                IEnumerable<Titulo> sql = from i in db.Titulo
                                          where i.cd_origem_titulo == cd_contrato &&
                                                i.id_origem_titulo == cd_origem &&
                                                i.vl_liquidacao_titulo > 0  &&
                                                i.cd_pessoa_empresa == cdEscola
                                          select i;
                if (cdTitulo > 0)
                    sql = from t in sql
                          where t.cd_titulo == cdTitulo
                          select t;
                return sql.Count() > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaTituloEnviadoCnabOuContrato(int cd_contrato, int cdEscola, List<int> cdsTitulo)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                IEnumerable<Titulo> sql = from i in db.Titulo
                                          where i.cd_origem_titulo == cd_contrato &&
                                                i.id_origem_titulo == cd_origem &&
                                                i.TitulosCnab.Any() &&
                                                i.cd_pessoa_empresa == cdEscola
                                          select i;
                if (cdsTitulo != null && cdsTitulo.Count() > 0)
                    sql = from t in sql
                          where cdsTitulo.Contains(t.cd_titulo)
                          select t;
                return sql.Count() > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaTituloEnviadoBoletoOuContrato(int cd_contrato, int cdEscola, List<int> cdsTitulo)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                IEnumerable<Titulo> sql = from i in db.Titulo
                                          where i.cd_origem_titulo == cd_contrato &&
                                                i.id_origem_titulo == cd_origem &&
                                                i.id_cnab_contrato &&
                                                i.cd_pessoa_empresa == cdEscola
                                          select i;
                if (cdsTitulo != null && cdsTitulo.Count() > 0)
                    sql = from t in sql
                          where cdsTitulo.Contains(t.cd_titulo)
                          select t;
                return sql.Count() > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaTituloOrContratoBaixaEfetuada(int cd_contrato, int cdEscola, int cdTitulo)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = from t in db.Titulo
                                          where t.cd_origem_titulo == cd_contrato &&
                                                t.id_origem_titulo == cd_origem &&
                                                t.vl_liquidacao_titulo > 0 &&
                                                t.cd_pessoa_empresa == cdEscola &&
                                                t.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA && 
                                                                       x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                                          select new { cd_titulo = t.cd_titulo };
                if (cdTitulo > 0)
                    sql = from t in sql
                          where t.cd_titulo == cdTitulo
                          select t;
                return sql.Any();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Titulo> getTitulosContrato(int cd_contrato)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                List<Titulo> sql = ((from i in db.Titulo
                                     where i.cd_origem_titulo == cd_contrato &&
                                           i.id_origem_titulo == cd_origem
                                           && i.dc_tipo_titulo != Titulo.TipoTitulo.TA.ToString() && i.dc_tipo_titulo != Titulo.TipoTitulo.TM.ToString()
                                     select new
                                     {
                                         i.nm_parcela_titulo,
                                         i.dt_vcto_titulo,
                                         i.vl_titulo,
                                         i.vl_saldo_titulo,
                                         i.vl_material_titulo
                                     }).ToList().Select(x => new Titulo
                                     {
                                         nm_parcela_titulo = x.nm_parcela_titulo,
                                         dt_vcto_titulo = x.dt_vcto_titulo,
                                         vl_titulo = x.vl_titulo,
                                         vl_saldo_titulo = x.vl_saldo_titulo,
                                         vl_material_titulo = x.vl_material_titulo
                                     })).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Titulo> getTitulosByContrato(int cdContrato, int cdEscola)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                List<Titulo> sql = ((from i in db.Titulo//.Include(l => l.LocalMovto)
                                     where i.cd_origem_titulo == cdContrato &&
                                           i.id_origem_titulo == cd_origem &&
                                           i.cd_pessoa_empresa == cdEscola
                                     select new
                                     {
                                        cd_titulo = i.cd_titulo,
                                        cd_origem_titulo = i.cd_origem_titulo,
                                        cd_pessoa_empresa = i.cd_pessoa_empresa,
                                        cd_pessoa_titulo = i.cd_pessoa_titulo,
                                        cd_pessoa_responsavel = i.cd_pessoa_responsavel,
                                        nomeResponsavel = i.PessoaResponsavel.no_pessoa,
                                        nomeAluno = i.Pessoa.no_pessoa,
                                        tipoDoc = i.TipoFinanceiro.dc_tipo_financeiro,
                                        cd_tipo_financeiro = i.cd_tipo_financeiro,
                                        dt_emissao_titulo = i.dt_emissao_titulo,
                                        dt_vcto_titulo = i.dt_vcto_titulo,
                                        vl_saldo_titulo = i.vl_saldo_titulo,
                                        vl_titulo = i.vl_titulo,
                                        dc_tipo_titulo = i.dc_tipo_titulo,
                                        nm_parcela_titulo = i.nm_parcela_titulo,
                                        dc_num_documento_titulo = i.dc_num_documento_titulo,
                                        nm_titulo = i.nm_titulo,
                                        vl_liquidacao_titulo = i.vl_liquidacao_titulo,
                                        cd_local_movto = i.cd_local_movto,
                                        //tituloBaixado = i.BaixaTitulo.Any(),
                                        nm_banco = i.LocalMovto.nm_banco,
                                        no_local_movto = i.LocalMovto.no_local_movto,
                                        nm_agencia = i.LocalMovto.nm_agencia,
                                        nm_conta_corrente = i.LocalMovto.nm_conta_corrente,
                                        nm_tipo_local = i.LocalMovto.nm_tipo_local,
                                        nm_digito_conta_corrente = i.LocalMovto.nm_digito_conta_corrente,
                                        id_origem_titulo = i.id_origem_titulo,
                                        id_natureza_titulo = i.id_natureza_titulo,
                                        pc_juros_titulo = i.pc_juros_titulo,
                                        pc_multa_titulo = i.pc_multa_titulo,
                                        id_status_cnab = i.id_status_cnab,
                                        id_status_titulo = i.id_status_titulo,
                                        id_carteira_registrada = i.LocalMovto.CarteiraCnab == null ? false : i.LocalMovto.CarteiraCnab.id_registrada,
                                        i.vl_material_titulo,
                                        possuiBaixa = i.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA && 
                                                                        x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO),
                                        possuiBaixaBolsa = i.BaixaTitulo.Any(x => x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA ||
                                                                             x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO),
                                        i.vl_desconto_contrato,
                                        vl_taxa_cartao = i.vl_taxa_cartao,
                                        pc_taxa_cartao = i.pc_taxa_cartao,
                                        nm_dias_cartao = i.nm_dias_cartao,
                                        id_cnab_contrato = i.id_cnab_contrato
                                     }).ToList().Select(x =>  new Titulo {
                                        cd_titulo = x.cd_titulo,
                                        cd_origem_titulo = x.cd_origem_titulo,
                                        cd_pessoa_empresa = x.cd_pessoa_empresa,
                                        cd_pessoa_titulo = x.cd_pessoa_titulo,
                                        cd_pessoa_responsavel = x.cd_pessoa_responsavel,
                                        nomeResponsavel = x.nomeResponsavel,
                                        nomeAluno = x.nomeAluno,
                                        tipoDoc = x.tipoDoc,
                                        cd_tipo_financeiro = x.cd_tipo_financeiro,
                                        dt_emissao_titulo = x.dt_emissao_titulo,
                                        dt_vcto_titulo = x.dt_vcto_titulo,
                                        vl_saldo_titulo = x.vl_saldo_titulo,
                                        vl_titulo = x.vl_titulo,
                                        dc_tipo_titulo = x.dc_tipo_titulo,
                                        nm_parcela_titulo = x.nm_parcela_titulo,
                                        dc_num_documento_titulo = x.dc_num_documento_titulo,
                                        nm_titulo = x.nm_titulo,
                                        tituloEdit = false,
                                        id = x.cd_titulo,
                                        vl_liquidacao_titulo = x.vl_liquidacao_titulo,
                                        cd_local_movto = x.cd_local_movto,
                                        id_carteira_registrada_localMvto = x.id_carteira_registrada,
                                        id_status_cnab = x.id_status_cnab,
                                        id_status_titulo = x.id_status_titulo,
                                        LocalMovto = new LocalMovto{
                                            no_local_movto = x.no_local_movto,
                                            nm_agencia = x.nm_agencia,
                                            nm_conta_corrente = x.nm_conta_corrente,
                                            nm_tipo_local = x.nm_tipo_local,
                                            nm_digito_conta_corrente = x.nm_digito_conta_corrente
                                        },
                                        //tituloBaixado = x.tituloBaixado,
                                        possuiBaixa = x.possuiBaixa,
                                        possuiBaixaBolsa = x.possuiBaixaBolsa,
                                        id_origem_titulo = x.id_origem_titulo,
                                        id_natureza_titulo = x.id_natureza_titulo,
                                        pc_juros_titulo = x.pc_juros_titulo,
                                        pc_multa_titulo = x.pc_multa_titulo,
                                        vl_material_titulo = x.vl_material_titulo,
                                        vl_desconto_contrato = x.vl_desconto_contrato,
                                        vl_taxa_cartao = x.vl_taxa_cartao,
                                        pc_taxa_cartao = x.pc_taxa_cartao,
                                        nm_dias_cartao = x.nm_dias_cartao,
                                        id_cnab_contrato = x.id_cnab_contrato
                                     })).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int getQtdContratoNaoMultiploDiferenteCartaoCheque(int cdContrato, int cdEscola)
        {
            try
            {
                var existeContratoNaoMultiploDiferenteCartaoCheque = (from c in db.Contrato //.Include(l => l.LocalMovto)
                                                                      where c.cd_contrato == cdContrato && c.cd_pessoa_escola == cdEscola
                                                                            && c.id_tipo_contrato != (int)Contrato.TipoCKMatricula.MULTIPLA &&
                                                                            (c.cd_tipo_financeiro != (int)TipoFinanceiro.TiposFinanceiro.CARTAO && c.cd_tipo_financeiro != (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)
                                                                      select c).Count();



                return existeContratoNaoMultiploDiferenteCartaoCheque;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int getQtdMovimentoDiferenteCartaoCheque(int cd_movimento, int cdEscola)
        {
            try
            {
                var existeMovimentoDiferenteCartaoCheque = (from m in db.Movimento //.Include(l => l.LocalMovto)
                                                                      where m.cd_movimento == cd_movimento && m.cd_pessoa_empresa == cdEscola &&
                                                                          (m.cd_tipo_financeiro != (int)TipoFinanceiro.TiposFinanceiro.CARTAO &&
                                                                           m.cd_tipo_financeiro != (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)
                                                                    select m).Count();
                return existeMovimentoDiferenteCartaoCheque;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public int getQtdTitulosSemBaixaTipoCartaoOuCheque(int cdContrato, int cdEscola)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                int sql = (from i in db.Titulo //.Include(l => l.LocalMovto)
                                        where i.cd_origem_titulo == cdContrato &&
                                              i.id_origem_titulo == cd_origem &&
                                              i.cd_pessoa_empresa == cdEscola &&
                                              (!i.BaixaTitulo.Any()) &&
                                              (i.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CARTAO || i.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)
                                              select i
                                     ).Count();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Titulo> getTitulosByContratoEscola(int cdContrato, int cdEscola, List<int> cds_titulos)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                List<Titulo> sql = (from i in db.Titulo //.Include(l => l.LocalMovto)
                           where i.cd_origem_titulo == cdContrato &&
                                 i.id_origem_titulo == cd_origem &&
                                 i.cd_pessoa_empresa == cdEscola &&
                                 cds_titulos.Contains(i.cd_titulo)
                           select i
                    ).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int getQtdTitulosMovimentoSemBaixaTipoCartaoOuCheque(int cdMovimento, int cdEscola)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Movimento"].ToString());

                int sql = (from i in db.Titulo //.Include(l => l.LocalMovto)
                           where i.cd_origem_titulo == cdMovimento &&
                              i.id_origem_titulo == cd_origem &&
                              i.cd_pessoa_empresa == cdEscola &&
                              (!i.BaixaTitulo.Any()) &&
                              (i.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CARTAO || i.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)
                        select i
                    ).Count();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Titulo> getTitulosByRenegociacao(int cdContrato, int cdAluno,int cdEscola, int cdProduto)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                List<Titulo> sql = ((from i in db.Titulo
                                     where 
                                       i.cd_pessoa_titulo == cdAluno &&
                                       i.cd_origem_titulo != cdContrato &&
                                       i.id_origem_titulo == cd_origem &&
                                       i.cd_pessoa_empresa == cdEscola &&
                                       i.vl_saldo_titulo == i.vl_titulo &&
                                       db.Contrato.Where(c => c.cd_contrato == i.cd_origem_titulo && c.cd_produto_atual == cdProduto).Any()
                                     select new
                                     {
                                         cd_titulo = i.cd_titulo,
                                         cd_origem_titulo = i.cd_origem_titulo,
                                         cd_pessoa_empresa = i.cd_pessoa_empresa,
                                         cd_pessoa_titulo = i.cd_pessoa_titulo,
                                         cd_pessoa_responsavel = i.cd_pessoa_responsavel,
                                         nomeResponsavel = i.PessoaResponsavel.no_pessoa,
                                         nomeAluno = i.Pessoa.no_pessoa,
                                         tipoDoc = i.TipoFinanceiro.dc_tipo_financeiro,
                                         cd_tipo_financeiro = i.cd_tipo_financeiro,
                                         dt_emissao_titulo = i.dt_emissao_titulo,
                                         dt_vcto_titulo = i.dt_vcto_titulo,
                                         vl_saldo_titulo = i.vl_saldo_titulo,
                                         vl_titulo = i.vl_titulo,
                                         dc_tipo_titulo = i.dc_tipo_titulo,
                                         nm_parcela_titulo = i.nm_parcela_titulo,
                                         dc_num_documento_titulo = i.dc_num_documento_titulo,
                                         nm_titulo = i.nm_titulo,
                                         tituloEdit = false,
                                         id = i.cd_titulo,
                                         vl_liquidacao_titulo = i.vl_liquidacao_titulo,
                                         cd_local_movto = i.cd_local_movto,
                                         tituloBaixado = i.BaixaTitulo.Any(),
                                         nm_banco = i.LocalMovto.nm_banco,
                                         no_local_movto = i.LocalMovto.no_local_movto,
                                         nm_agencia = i.LocalMovto.nm_agencia,
                                         nm_conta_corrente = i.LocalMovto.nm_conta_corrente,
                                         nm_tipo_local = i.LocalMovto.nm_tipo_local,
                                         id_origem_titulo = i.id_origem_titulo,
                                         id_natureza_titulo = i.id_natureza_titulo
                                     }).ToList().Select(x => new Titulo
                                     {
                                         cd_titulo = x.cd_titulo,
                                         cd_origem_titulo = x.cd_origem_titulo,
                                         cd_pessoa_empresa = x.cd_pessoa_empresa,
                                         cd_pessoa_titulo = x.cd_pessoa_titulo,
                                         cd_pessoa_responsavel = x.cd_pessoa_responsavel,
                                         nomeResponsavel = x.nomeResponsavel,
                                         nomeAluno = x.nomeAluno,
                                         tipoDoc = x.tipoDoc,
                                         cd_tipo_financeiro = x.cd_tipo_financeiro,
                                         dt_emissao_titulo = x.dt_emissao_titulo,
                                         dt_vcto_titulo = x.dt_vcto_titulo,
                                         vl_saldo_titulo = x.vl_saldo_titulo,
                                         vl_titulo = x.vl_titulo,
                                         dc_tipo_titulo = x.dc_tipo_titulo,
                                         nm_parcela_titulo = x.nm_parcela_titulo,
                                         dc_num_documento_titulo = x.dc_num_documento_titulo,
                                         nm_titulo = x.nm_titulo,
                                         tituloEdit = x.tituloEdit,
                                         id = x.id,
                                         vl_liquidacao_titulo = x.vl_liquidacao_titulo,
                                         cd_local_movto = x.cd_local_movto,
                                         LocalMovto = new LocalMovto
                                         {
                                             no_local_movto = x.no_local_movto,
                                             nm_agencia = x.nm_agencia,
                                             nm_conta_corrente = x.nm_conta_corrente,
                                             nm_tipo_local = x.nm_tipo_local
                                         },
                                         tituloBaixado = x.tituloBaixado,
                                         id_origem_titulo = x.id_origem_titulo,
                                         id_natureza_titulo = x.id_natureza_titulo
                                     })).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getTitulosByContratoTodosDados(int cdContrato, int cdEscola)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = from i in db.Titulo
                          where i.cd_origem_titulo == Math.Abs(cdContrato) &&
                                (
                                    (i.vl_saldo_titulo > 0 && cdContrato > 0) || 
                                    ((i.vl_saldo_titulo == 0 || (i.vl_saldo_titulo == i.vl_titulo)) && cdContrato < 0) ||
                                    (i.vl_saldo_titulo == 0 && i.vl_titulo == 0)
                                ) &&
                                i.id_origem_titulo ==  cd_origem &&
                                i.cd_pessoa_empresa == cdEscola
                          select i;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public decimal getValorParcela(int cdContrato, int cdEscola, bool primeira_parcela) {
            IQueryable<decimal> valor;
            try {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = from t in db.Titulo
                           where t.cd_origem_titulo == cdContrato
                              && t.cd_pessoa_empresa == cdEscola
                              && t.id_origem_titulo == cd_origem
                              && t.dc_tipo_titulo != "TM" && t.dc_tipo_titulo != "TA"
                              && t.dc_tipo_titulo != "AA" && t.dc_tipo_titulo != "AD"
                           select t;
                if (primeira_parcela)
                    valor = from t in sql
                            where t.nm_parcela_titulo == 1
                            select t.vl_titulo;
                else
                    valor = from t in sql
                            where
                            t.nm_parcela_titulo == 2
                            select t.vl_titulo;
                return valor.ToList().Count > 0 ? valor.Sum() : 0;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getTitulosContrato(List<int> cdTitulos, int cd_escola)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = from t in db.Titulo.Include(x => x.PlanoTitulo)
                          where cdTitulos.Contains(t.cd_titulo) &&
                          t.cd_pessoa_empresa == cd_escola &&
                          t.id_origem_titulo == cd_origem
                          select t;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public bool deleteAllTitulo(List<Titulo> titulos)
        {
            try
            {
                var strTitulo = "";
                foreach (Titulo tipo in titulos)
                    strTitulo += tipo.cd_titulo + ",";

                if (strTitulo.Length > 0)
                    strTitulo = strTitulo.Substring(0, strTitulo.Length - 1);

                int retono = db.Database.ExecuteSqlCommand("delete from t_titulo where cd_titulo in(" + strTitulo + ")");
                return retono > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public string delTitulosContrato(int cd_contrato, bool existeBolsaContrato, string Json)
        {
            try
            {

                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = @"sp_excluir_titulo_contrato";
                command.CommandTimeout = 180;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_contrato", cd_contrato));

                sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@id_bolsa", existeBolsaContrato));

                sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@Json", Json));

                var parameterm = new System.Data.SqlClient.SqlParameter("@mensagem", System.Data.SqlDbType.VarChar, 1024);
                parameterm.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(parameterm);

                var parameter = new System.Data.SqlClient.SqlParameter("@result", System.Data.SqlDbType.Int);
                parameter.Direction = System.Data.ParameterDirection.ReturnValue;

                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();

                //var retunvalue = Convert.ToString((int)command.Parameters["@result"].Value);
                string retunvalue = null;
                retunvalue = (string)command.Parameters["@mensagem"].Value;

                db.Database.Connection.Close();
                return retunvalue;
            }
            catch (System.Data.SqlClient.SqlException exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
            catch (Exception exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> searchTitulo(SearchParameters parametros, int cd_pessoa_empresa, int cd_pessoa, bool responsavel, bool inicio, int locMov, int natureza, int status, int? numeroTitulo,
                                               int? parcelaTitulo, string valorTitulo, DateTime? dtInicial, DateTime? dtFinal, bool emissao, bool vencimento, bool baixa, int locMovBaixa,
                                               int cdTipoLiquidacao, bool contaSegura, byte tipoTitulo, string nossoNumero, int cnabStatus, int? nro_recibo, int? cd_turma, List<int> cd_situacoes_aluno,
                                               int? cd_tipo_financeiro)
        {
            try
            {
                SGFWebContext dbContext = new SGFWebContext();
                int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                IEntitySorter<Titulo> sorter = EntitySorter<Titulo>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Titulo> sql;
                sql = from t in db.Titulo.AsNoTracking()
                      where t.cd_pessoa_empresa == cd_pessoa_empresa
                      select t;
                if (tipoTitulo > 0)
                {
                    string filtro = Titulo.convertTipoTitulo(tipoTitulo);
                    sql = from t in sql
                          where t.dc_tipo_titulo == filtro
                          select t;
                }
                if (cnabStatus > -1) 
                {
                    sql = from t in sql
                          where t.id_status_cnab == cnabStatus
                          select t;
                }
                if (cd_pessoa > 0)
                    if (responsavel)
                        sql = from t in sql
                              where t.cd_pessoa_responsavel == cd_pessoa
                              select t;
                    else
                        sql = from t in sql
                              where t.cd_pessoa_titulo == cd_pessoa
                              select t;
                if (locMov > 0)
                    sql = from t in sql
                          where t.cd_local_movto == locMov
                          select t;
                //Título possui baixa com esse local de movimento
                if (locMovBaixa > 0)
                    sql = from t in sql
                          where t.BaixaTitulo.Where(bt => bt.cd_local_movto == locMovBaixa).Any()
                          select t;
                //Título que possui baixa com esse tipo de liquidação
                if (cdTipoLiquidacao > 0)
                    sql = from t in sql
                          where t.BaixaTitulo.Where(bt => bt.cd_tipo_liquidacao == cdTipoLiquidacao).Any()
                          select t;
                if (natureza > 0)
                    sql = from t in sql
                          where t.id_natureza_titulo == natureza
                          select t;
                if (status > 0)
                    sql = from t in sql
                          where t.id_status_titulo == status
                          select t;

                if (numeroTitulo != null && numeroTitulo > 0)
                    if(inicio) {
                        string numero = numeroTitulo.ToString();
                        sql = from t in sql
                              where System.Data.Entity.SqlServer.SqlFunctions.StringConvert((decimal)t.nm_titulo).TrimStart().StartsWith(numero)
                              select t;
                    }
                    else
                        sql = from t in sql
                              where t.nm_titulo == numeroTitulo
                              select t;

                if (parcelaTitulo != null && parcelaTitulo > 0)
                    sql = from t in sql
                          where t.nm_parcela_titulo == parcelaTitulo
                          select t;

                if (!string.IsNullOrEmpty(valorTitulo))
                {

                    if(inicio) {
                        valorTitulo = valorTitulo.Replace(".", "").Replace(",", "");
                        sql = from t in sql
                              where System.Data.Entity.SqlServer.SqlFunctions.StringConvert(t.vl_titulo * 100).TrimStart().StartsWith(valorTitulo)
                              select t;
                    }
                    else {
                        decimal vl_titulo = decimal.Parse(valorTitulo);
                        sql = from t in sql
                              where t.vl_titulo == vl_titulo
                              select t;
                    }
                }

                if (emissao)
                {
                    if (dtInicial.HasValue)
                    {
                        var dtInicialP = dtInicial.Value;
                        sql = from t in sql
                              where DbFunctions.TruncateTime(t.dt_emissao_titulo) >= dtInicialP.Date
                              select t;
                    }

                    if (dtFinal.HasValue)
                    {
                        var dtFinalP = dtFinal.Value;
                        sql = from t in sql
                              where DbFunctions.TruncateTime(t.dt_emissao_titulo) <= dtFinalP.Date
                              select t;
                    }
                }

                if (vencimento)
                {
                    if (dtInicial.HasValue)
                        sql = from t in sql
                              where t.dt_vcto_titulo >= dtInicial
                              select t;

                    if (dtFinal.HasValue)
                        sql = from t in sql
                              where t.dt_vcto_titulo <= dtFinal
                              select t;
                }
                if (baixa)
                {
                    if (dtInicial.HasValue && dtFinal.HasValue)
                    {
                        var dtInicialP = dtInicial.Value;
                        var dtFinalP = dtFinal.Value;
                        sql = from t in sql
                              where t.BaixaTitulo.Where(bt => DbFunctions.TruncateTime(bt.dt_baixa_titulo) >= dtInicialP.Date &&
                                                              DbFunctions.TruncateTime(bt.dt_baixa_titulo) <= dtFinalP.Date).Any()
                              select t;
                    }
                    else
                        if (dtInicial.HasValue)
                        {
                            var dtInicialP = dtInicial.Value;
                            sql = from t in sql
                                  where t.BaixaTitulo.Where(bt => DbFunctions.TruncateTime(bt.dt_baixa_titulo) >= dtInicialP.Date).Any()
                                  select t;
                        }
                        else
                            if (dtFinal.HasValue)
                            {
                                var dtFinalP = dtFinal.Value;
                                sql = from t in sql
                                      where t.BaixaTitulo.Where(bt => DbFunctions.TruncateTime(bt.dt_baixa_titulo) <= dtFinalP.Date).Any()
                                      select t;
                            }
                }
                //Usuário não possui permissão de conta segura
                if (!contaSegura)
                    sql = from t in sql
                          where !t.PlanoTitulo.Any(m => m.PlanoConta.id_conta_segura == true)
                          select t;

                if (!string.IsNullOrEmpty(nossoNumero))
                    if (inicio)
                        sql = from t in sql
                              where t.dc_nosso_numero.StartsWith(nossoNumero)
                              select t;
                    else
                        sql = from t in sql
                              where t.dc_nosso_numero.Contains(nossoNumero)
                              select t;
                
                if (nro_recibo != null && nro_recibo > 0)
                    if (inicio)
                    {
                        string recibo = nro_recibo.ToString();
                        sql = from t in sql
                              where t.BaixaTitulo.Any(x => System.Data.Entity.SqlServer.SqlFunctions.StringConvert((decimal)x.nm_recibo).TrimStart().StartsWith(recibo))
                              select t;
                    }
                    else
                        sql = from t in sql
                              where t.BaixaTitulo.Any(x => x.nm_recibo == nro_recibo)
                              select t;

                if (cd_situacoes_aluno.Count > 0)
                {
                    if (cd_turma == null)
                        cd_turma = 0;
                    sql = from t in sql
                          where db.Contrato.Where(c =>
                                        c.cd_contrato == t.cd_origem_titulo &&
                                        t.id_origem_titulo == cd_origem &&
                                        c.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo &&
                                        c.HistoricosAluno.Where(h =>
                                                (cd_turma == 0 || h.cd_turma == cd_turma) &&
                                                h.cd_produto == c.cd_produto_atual &&
                                                h.dt_historico <= t.dt_vcto_titulo &&
                                                cd_situacoes_aluno.Contains(h.id_situacao_historico) &&
                                                h.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo).Any()).Any()
                          select t;
                }
                else
                {
                    if (cd_turma > 0)
                        sql = from t in sql
                              where db.Contrato.Where(c =>
                                            c.cd_contrato == t.cd_origem_titulo &&
                                            t.id_origem_titulo == cd_origem &&
                                            c.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo &&
                                            c.HistoricosAluno.Where(h =>
                                                    h.dt_historico <= t.dt_vcto_titulo &&
                                                    h.cd_produto == c.cd_produto_atual &&
                                                    h.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo && h.cd_turma == cd_turma).Any()).Any()
                              select t;
                }

                if (cd_tipo_financeiro != null)
                {
                    sql = from t in sql
                          where t.cd_tipo_financeiro == cd_tipo_financeiro
                          select t;
                }

                sql = sorter.Sort(sql);
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                var retorno = (from t in sql
                               select new
                               {
                                   cd_titulo = t.cd_titulo,
                                   id_natureza_titulo = t.id_natureza_titulo,
                                   nomeResponsavel = t.PessoaResponsavel.no_pessoa,
                                   nm_parcela_titulo = t.nm_parcela_titulo,
                                   nm_titulo = t.nm_titulo,
                                   dt_emissao_titulo = t.dt_emissao_titulo,
                                   dt_vcto_titulo = t.dt_vcto_titulo,
                                   vl_titulo = t.vl_titulo,
                                   vl_saldo_titulo = t.vl_saldo_titulo,
                                   cd_pessoa_empresa = t.cd_pessoa_empresa,
                                   id_origem_titulo = t.id_origem_titulo,
                                   cd_origem_titulo = t.cd_origem_titulo,
                                   id_status_titulo = t.id_status_titulo,
                                   possuiBaixa = t.BaixaTitulo.Any(),
                                   pc_juros_titulo = t.pc_juros_titulo,
                                   pc_multa_titulo = t.pc_multa_titulo,
                                   id_status_cnab = t.id_status_cnab,
                                   id_local_carteira_registrada = t.LocalMovto != null && t.LocalMovto.CarteiraCnab != null ? t.LocalMovto.CarteiraCnab.id_registrada : false,
                                   dc_tipo_titulo = t.dc_tipo_titulo,
                                   t.TipoFinanceiro.dc_tipo_financeiro,
                                   t.vl_material_titulo,
                                   t.cd_tipo_financeiro,
                                   t.dc_num_documento_titulo,
                                   cd_local_movto = t.LocalMovto.cd_local_movto,
                                   nm_tipo_local = t.LocalMovto.nm_tipo_local,
                                   cd_local_banco = t.LocalMovto.cd_local_banco,
                                   pc_taxa_cartao = t.pc_taxa_cartao,
                                   nm_dias_cartao = t.nm_dias_cartao,
                                   vl_taxa_cartao = t.vl_taxa_cartao
                               }).ToList().Select(x => new Titulo
                               {
                                   cd_titulo = x.cd_titulo,
                                   nomeResponsavel = x.nomeResponsavel,
                                   dt_emissao_titulo = x.dt_emissao_titulo,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   vl_saldo_titulo = x.vl_saldo_titulo,
                                   vl_titulo = x.vl_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   nm_titulo = x.nm_titulo,
                                   id_natureza_titulo = x.id_natureza_titulo,
                                   cd_pessoa_empresa = x.cd_pessoa_empresa,
                                   id_origem_titulo = x.id_origem_titulo,
                                   cd_origem_titulo = x.cd_origem_titulo,
                                   id_status_titulo = x.id_status_titulo,
                                   possuiBaixa = x.possuiBaixa,
                                   pc_juros_titulo = x.pc_juros_titulo,
                                   pc_multa_titulo = x.pc_multa_titulo,
                                   id_status_cnab = x.id_status_cnab,
                                   id_carteira_registrada_localMvto = x.id_local_carteira_registrada,
                                   dc_tipo_titulo = x.dc_tipo_titulo,
                                   vl_material_titulo = x.vl_material_titulo,
                                   tipoDoc = x.dc_tipo_financeiro,
                                   cd_tipo_financeiro = x.cd_tipo_financeiro,
                                   dc_num_documento_titulo = x.dc_num_documento_titulo,
                                   cd_local_movto = x.cd_local_movto,
                                   nm_tipo_local = x.nm_tipo_local,
                                   cd_local_banco = x.cd_local_banco,
                                   pc_taxa_cartao = x.pc_taxa_cartao,
                                   nm_dias_cartao = x.nm_dias_cartao,
                                   vl_taxa_cartao = x.vl_taxa_cartao
                               });


                retorno = OrderRetorno(parametros, retorno);
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        private static IEnumerable<Titulo> OrderRetorno(SearchParameters parametros, IEnumerable<Titulo> retorno)
        {
            if ("cd_tipo_financeiro".Equals(parametros.sort))
            {
                if (parametros.sortOrder == Componentes.Utils.SortDirection.Ascending)
                    retorno = retorno.OrderBy(x => x.tipoDoc);
                else
                    retorno = retorno.OrderByDescending(x => x.tipoDoc);
            }

            if ("id_status_titulo".Equals(parametros.sort))
            {
                if (parametros.sortOrder == Componentes.Utils.SortDirection.Ascending)
                    retorno = retorno.OrderBy(x => x.id_status_titulo);
                else
                    retorno = retorno.OrderByDescending(x => x.id_status_titulo);
            }
            return retorno;
        }

        public IEnumerable<Titulo> searchTituloCnab(TituloUI titulo)
        {
            try
            {
                SGFWebContext dbContext = new SGFWebContext();
                int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                IQueryable<Titulo> sql;

                List<int> cd_turma_ppt = (from tu in db.Turma where tu.id_turma_ppt == false && tu.cd_turma_ppt == titulo.cd_turma select tu.cd_turma).ToList();

                sql = from t in db.Titulo
                      where t.cd_pessoa_empresa == titulo.cd_pessoa_empresa && 
                            t.TipoFinanceiro.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.BOLETO &&
                            t.vl_titulo > 0
                      select t;
                 switch (titulo.id_tipo_cnab)
                {
                    case (int)Cnab.TipoCnab.GERAR_BOLETOS:
                         sql = from t in sql
                               where (t.id_status_cnab == (int)Titulo.StatusCnabTitulo.INICIAL /*|| t.id_status_cnab == (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA*/ ) &&
                              t.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO && !t.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                                               x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                              select t;
                         break;
                    case (int)Cnab.TipoCnab.CANCELAR_BOLETOS:
                         sql = from t in sql
                               where t.id_status_cnab == (int)Titulo.StatusCnabTitulo.ENVIADO_GERADO
                                     && t.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO && !t.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                                                         x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                               select t;
                         break;
                    case (int)Cnab.TipoCnab.PEDIDO_BAIXA:
                         int[] cdStatusTituloCnab = new int[2];
                         cdStatusTituloCnab[0] = (int)Titulo.StatusCnabTitulo.BAIXA_MANUAL;
                         cdStatusTituloCnab[1] = (int)Titulo.StatusCnabTitulo.CONFIRMADO_ENVIO;
                         sql = from t in sql
                               where cdStatusTituloCnab.Contains(t.id_status_cnab)
                               select t;
                         break;
                }
                 if (titulo.cd_pessoa > 0)
                     //if (titulo.responsavel)
                     sql = from t in sql
                           where t.cd_pessoa_responsavel == titulo.cd_pessoa
                           select t;
                 if (titulo.cd_pessoa_cliente > 0)
                     sql = from t in sql
                           where t.cd_pessoa_titulo == titulo.cd_pessoa_cliente
                           select t;
                 if (titulo.locMov > 0)
                    sql = from t in sql
                          where t.cd_local_movto == titulo.locMov
                          select t;
                if (titulo.natureza > 0)
                    sql = from t in sql
                          where t.id_natureza_titulo == titulo.natureza
                          select t;

                if (titulo.numeroTitulo != null && titulo.numeroTitulo > 0)
                    sql = from t in sql
                          where t.nm_titulo == titulo.numeroTitulo
                          select t;

                if (titulo.parcelaTitulo != null && titulo.parcelaTitulo > 0)
                    sql = from t in sql
                          where t.nm_parcela_titulo == titulo.parcelaTitulo
                          select t;
                if (titulo.emissao)
                {
                    if (titulo.dtInicial.HasValue)
                    {
                        var dtInicialP = titulo.dtInicial.Value;
                        sql = from t in sql
                              where DbFunctions.TruncateTime(t.dt_emissao_titulo) >= dtInicialP.Date
                              select t;
                    }

                    if (titulo.dtFinal.HasValue)
                    {
                        var dtFinalP = titulo.dtFinal.Value;
                        sql = from t in sql
                              where DbFunctions.TruncateTime(t.dt_emissao_titulo) <= dtFinalP.Date
                              select t;
                    }
                }
                if (titulo.vencimento)
                {
                    if (titulo.dtInicial.HasValue)
                    {
                        var dtInicialP = titulo.dtInicial.Value;
                        sql = from t in sql
                              where DbFunctions.TruncateTime(t.dt_vcto_titulo) >= dtInicialP.Date
                              select t;
                    }

                    if (titulo.dtFinal.HasValue)
                    {
                        var dtFinalP = titulo.dtFinal.Value;
                        sql = from t in sql
                              where DbFunctions.TruncateTime(t.dt_vcto_titulo) <= dtFinalP.Date
                              select t;
                    }
                }

                if (titulo.todosLocais && titulo.cdLocais != null && titulo.cdLocais.Count() > 0)
                    sql = from t in sql
                          where titulo.cdLocais.Contains(t.cd_local_movto)
                          select t;
                else
                    if (titulo.cdLocaisEscolhidos != null && titulo.cdLocaisEscolhidos.Count() > 0)
                        sql = from t in sql
                              where titulo.cdLocaisEscolhidos.Contains(t.cd_local_movto)
                              select t;
                if(titulo.cdTituloExcludeSelect != null && titulo.cdTituloExcludeSelect.Count() > 0)
                    sql = from t in sql
                          where !titulo.cdTituloExcludeSelect.Contains(t.cd_titulo)
                          select t;

                //Filtros view Cnab
                if (titulo.cd_aluno > 0)
                    sql = from t in sql
                          where db.Aluno.Where(a => a.cd_aluno == titulo.cd_aluno && t.cd_pessoa_titulo == a.cd_pessoa_aluno).Any()
                          select t;
                if (titulo.nro_contrato > 0)
                    sql = from t in sql
                          where db.Contrato.Where(c =>
                               c.cd_contrato == t.cd_origem_titulo &&
                               t.id_origem_titulo == cd_origem && 
                               c.nm_contrato == titulo.nro_contrato).Any() 
                          select t;
                if (titulo.cd_turma > 0)
                    sql = from t in sql
                          where  db.Contrato.Where(c => 
                                        c.cd_contrato == t.cd_origem_titulo && 
                                        t.id_origem_titulo == cd_origem && 
                                        c.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo && (cd_turma_ppt.Count == 0 ||  (cd_turma_ppt.Count > 0 && c.Aluno.AlunoTurma.Any(at => at.cd_contrato == c.cd_contrato && c.cd_aluno == at.cd_aluno && cd_turma_ppt.Contains(at.cd_turma)))) &&
                                        c.HistoricosAluno.Where(h =>
                                                h.dt_historico <= t.dt_vcto_titulo &&  
                                                h.cd_produto == c.cd_produto_atual &&
                                                h.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo && (h.cd_turma == titulo.cd_turma || (cd_turma_ppt.Count > 0 && cd_turma_ppt.Contains(h.cd_turma)))).Any()).Any()
                          select t;
                if (titulo.cd_produto > 0)
                    sql = from t in sql
                          where db.Contrato.Where(c =>
                                        c.cd_contrato == t.cd_origem_titulo &&
                                        t.id_origem_titulo == cd_origem &&
                                        c.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo &&
                                        c.cd_produto_atual == titulo.cd_produto).Any()
                          select t;
                var retorno = (from t in sql
                               select new
                               {
                                   cd_titulo = t.cd_titulo,
                                   cd_pessoa_titulo = t.cd_pessoa_titulo,
                                   no_pessoa = t.Pessoa.no_pessoa,
                                   id_natureza_titulo = t.id_natureza_titulo,
                                   nomeResponsavel = t.PessoaResponsavel.no_pessoa,
                                   nm_parcela_titulo = t.nm_parcela_titulo,
                                   nm_titulo = t.nm_titulo,
                                   dt_emissao_titulo = t.dt_emissao_titulo,
                                   dt_vcto_titulo = t.dt_vcto_titulo,
                                   vl_titulo = t.vl_titulo,
                                   t.vl_saldo_titulo,
                                   id_status_titulo = t.id_status_titulo,
                                   id_status_cnab = t.id_status_cnab,
                                   no_local_movto = t.LocalMovto.no_local_movto,
                                   nm_agencia = t.LocalMovto.nm_agencia,
                                   nm_conta_corrente = t.LocalMovto.nm_conta_corrente,
                                   nm_digito_conta_corrente = t.LocalMovto.nm_digito_conta_corrente != null ? t.LocalMovto.nm_digito_conta_corrente : null,
                                   nm_tipo_local = t.LocalMovto.nm_tipo_local,
                                   cd_local_movto = t.cd_local_movto,
                                   dc_tipo_titulo = t.dc_tipo_titulo,
                                   id_cnab_contrato = t.id_cnab_contrato
                               }).ToList().Select(x => new Titulo
                               {
                                   cd_titulo = x.cd_titulo,
                                   cd_pessoa_titulo = x.cd_pessoa_titulo,
                                   nomeResponsavel = x.nomeResponsavel,
                                   nomeCliente = x.no_pessoa,
                                   dt_emissao_titulo = x.dt_emissao_titulo,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   vl_titulo = x.vl_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   nm_titulo = x.nm_titulo,
                                   id_natureza_titulo = x.id_natureza_titulo,
                                   id_status_cnab = x.id_status_cnab,
                                   cd_local_movto = x.cd_local_movto,
                                   vl_saldo_titulo = x.vl_saldo_titulo,
                                   LocalMovto = new LocalMovto
                                   {
                                       no_local_movto = x.no_local_movto,
                                       nm_agencia = x.nm_agencia,
                                       nm_conta_corrente = x.nm_conta_corrente,
                                       nm_digito_conta_corrente = x.nm_digito_conta_corrente,
                                       nm_tipo_local = x.nm_tipo_local
                                   },
                                   dc_tipo_titulo = x.dc_tipo_titulo,
                                   id_cnab_contrato = x.id_cnab_contrato
                               }).OrderBy(x => x.nm_titulo).ThenBy(x=> x.nm_parcela_titulo);

                var todosTitulos = retorno.Where(t => t.id_cnab_contrato == false).ToList();
                if (titulo.locaisEscolhidos != null && titulo.locaisEscolhidos.Count() > 0 &&
                    titulo.locaisEscolhidos[0].cd_local_movto > 0 && titulo.id_cnab_tipo == 0
                    )
                {
                    var titulosComMesmoLocalMovtoView = retorno.Where(t => t.id_cnab_contrato == true
                                            && titulo.cdLocaisEscolhidos.Contains(t.cd_local_movto));
                    todosTitulos.AddRange(titulosComMesmoLocalMovtoView);
                }

                return todosTitulos;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<TituloCnab> searchTituloCnabGrade(TituloUI titulo)
        {
            try
            {
                if (titulo != null)
                {
                    var sql = from t in titulo.titulosGrade
                              select t;
                    if (titulo.cd_pessoa > 0)
                        sql = from t in sql
                              where t.cd_pessoa_titulo == titulo.cd_pessoa
                              select t;
                    if (titulo.cd_aluno > 0)
                        sql = from t in sql
                              where t.cd_aluno == titulo.cd_aluno
                              select t;
                    if (titulo.cd_produto > 0)
                        sql = from t in sql
                              where t.cd_produto == titulo.cd_produto
                              select t;
                    if (titulo.nro_contrato > 0)
                        sql = from t in sql
                              where t.nro_contrato == titulo.nro_contrato
                              select t;
                    if (titulo.cd_turma > 0)
                        sql = from t in sql
                              where (t.cd_turma_titulo == titulo.cd_turma || t.cd_turma_PPT == titulo.cd_turma)
                              select t;
                    if (titulo.cdLocaisEscolhidos != null && titulo.cdLocaisEscolhidos.Count() > 0)
                        sql = from t in sql
                              where titulo.cdLocaisEscolhidos.Contains(t.cd_local_movto_titulo)
                              select t;
                    //if (titulo.emissao)
                    //{
                    //    if (titulo.dtInicial.HasValue)
                    //    {
                    //        var dtInicialP = titulo.dtInicial.Value;
                    //        sql = from t in sql
                    //              where DbFunctions.TruncateTime(t.dt_emissao_titulo) >= dtInicialP.Date
                    //              select t;
                    //    }

                    //    if (titulo.dtFinal.HasValue)
                    //    {
                    //        var dtFinalP = titulo.dtFinal.Value;
                    //        sql = from t in sql
                    //              where DbFunctions.TruncateTime(t.dt_emissao_titulo) <= dtFinalP.Date
                    //              select t;
                    //    }
                    //}

                    //if (titulo.vencimento)
                    //{
                    //    if (titulo.dtInicial.HasValue)
                    //        sql = from t in sql
                    //              where t.dt_vcto_titulo >= titulo.dtInicial
                    //              select t;

                    //    if (titulo.dtFinal.HasValue)
                    //        sql = from t in sql
                    //              where t.dt_vcto_titulo <= titulo.dtFinal
                    //              select t;
                    //}


                    return sql.ToList();
                }

                return null;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Titulo> getTituloBaixaFinanSimulacao(List<int> cdsTitulo, int cd_pessoa_empresa, int? cd_registro_origem, TipoConsultaTituloEnum tipoConsulta)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                List<Titulo> retorno = new List<Titulo>();
                int[] statusCnabTitulo = new int[] { (int)Titulo.StatusCnabTitulo.INICIAL, (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA };
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = from t in db.Titulo
                       where t.cd_pessoa_empresa == cd_pessoa_empresa
                       select t;
                switch (tipoConsulta)
                {
                    case TipoConsultaTituloEnum.HAS_SIMULACAO:
                        retorno = (from t in db.Titulo
                                   where cdsTitulo.Contains(t.cd_titulo)
                                   select new
                                   {
                                       cd_titulo = t.cd_titulo,
                                       vl_saldo_titulo = t.vl_saldo_titulo,
                                       cd_origem_titulo = t.cd_origem_titulo,
                                       nm_titulo = t.nm_titulo,
                                       nm_parcela_titulo = t.nm_parcela_titulo,
                                       id_natureza_titulo = t.id_natureza_titulo,
                                       id_origem_titulo = t.id_origem_titulo,
                                       dt_vcto_titulo = t.dt_vcto_titulo,
                                       cd_pessoa_empresa = t.cd_pessoa_empresa,
                                       vl_material_titulo = t.vl_material_titulo,
                                       dc_tipo_titulo = t.dc_tipo_titulo
                                   }).ToList().Select(x => new Titulo
                               {
                                   cd_titulo = x.cd_titulo,
                                   vl_saldo_titulo = x.vl_saldo_titulo,
                                   cd_origem_titulo = x.cd_origem_titulo,
                                   nm_titulo = x.nm_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   id_natureza_titulo = x.id_natureza_titulo,
                                   id_origem_titulo = x.id_origem_titulo,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   cd_pessoa_empresa = x.cd_pessoa_empresa,
                                   vl_material_titulo = x.vl_material_titulo,
                                   dc_tipo_titulo = x.dc_tipo_titulo
                               }).ToList();
                        break;
                    case TipoConsultaTituloEnum.HAS_TITULOS_ABERTO_SIMULACAO:
                        retorno = (from t in db.Titulo
                                   where t.cd_origem_titulo == cd_registro_origem &&
                                         t.id_origem_titulo == cd_origem &&
                                         statusCnabTitulo.Contains(t.id_status_cnab) &&
                                         (t.dc_tipo_titulo != "TM" && t.dc_tipo_titulo != "TA") &&
                                         !t.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                            x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                                   select new
                                   {
                                       cd_titulo = t.cd_titulo,
                                       vl_saldo_titulo = t.vl_saldo_titulo,
                                       cd_origem_titulo = t.cd_origem_titulo,
                                       nm_titulo = t.nm_titulo,
                                       nm_parcela_titulo = t.nm_parcela_titulo,
                                       id_natureza_titulo = t.id_natureza_titulo,
                                       id_origem_titulo = t.id_origem_titulo,
                                       dt_vcto_titulo = t.dt_vcto_titulo,
                                       cd_pessoa_empresa = t.cd_pessoa_empresa,
                                       vl_material_titulo = t.vl_material_titulo,
                                       vl_titulo = t.vl_titulo,
                                       dc_tipo_titulo = t.dc_tipo_titulo,
                                       id_status_cnab = t.id_status_cnab,
                                       id_status_titulo = t.id_status_titulo
                                   }).ToList().Select(x => new Titulo
                                   {
                                       cd_titulo = x.cd_titulo,
                                       vl_saldo_titulo = x.vl_saldo_titulo,
                                       cd_origem_titulo = x.cd_origem_titulo,
                                       nm_titulo = x.nm_titulo,
                                       nm_parcela_titulo = x.nm_parcela_titulo,
                                       id_natureza_titulo = x.id_natureza_titulo,
                                       id_origem_titulo = x.id_origem_titulo,
                                       dt_vcto_titulo = x.dt_vcto_titulo,
                                       cd_pessoa_empresa = x.cd_pessoa_empresa,
                                       vl_material_titulo = x.vl_material_titulo,
                                       vl_titulo = x.vl_titulo,
                                       dc_tipo_titulo = x.dc_tipo_titulo,
                                       id_status_cnab = x.id_status_cnab,
                                       id_status_titulo = x.id_status_titulo
                                   }).ToList();
                        break;
                }
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Titulo getTituloBaixaFinan(string dc_nosso_numero, int cd_pessoa_empresa, int cd_local_movto)
        {
            try {
                string nosso_numero = long.Parse(dc_nosso_numero) + "";
                Titulo sql = (from t in db.Titulo
                              where t.dc_nosso_numero == nosso_numero
                              && t.cd_pessoa_empresa == cd_pessoa_empresa
                              && t.cd_local_movto == cd_local_movto
                              select t).FirstOrDefault();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }


        public List<Titulo> getTitulosBaixaFinan(List<string> dc_nosso_numero, int cd_pessoa_empresa, int cd_local_movto)
        {
            try
            {
                List<string> listaNossoNumero = dc_nosso_numero.Select(x => long.Parse(x) + "").ToList();
                //string nosso_numero = long.Parse(dc_nosso_numero) + "";
                List<Titulo> sql = (from t in db.Titulo
                                    where listaNossoNumero.Contains(t.dc_nosso_numero) 
                                    && t.cd_pessoa_empresa == cd_pessoa_empresa
                                    && t.cd_local_movto == cd_local_movto
                              select new
                              {

                                  cd_titulo = t.cd_titulo,
                                  cd_pessoa_empresa = t.cd_pessoa_empresa,
                                  cd_pessoa_titulo = t.cd_pessoa_titulo,
                                  cd_pessoa_responsavel = t.cd_pessoa_responsavel,
                                  cd_local_movto = t.cd_local_movto,
                                  dt_emissao_titulo = t.dt_emissao_titulo,
                                  cd_origem_titulo = t.cd_origem_titulo,
                                  dt_vcto_titulo = t.dt_vcto_titulo,
                                  dh_cadastro_titulo = t.dh_cadastro_titulo,
                                  vl_titulo = t.vl_titulo,
                                  dt_liquidacao_titulo = t.dt_liquidacao_titulo,
                                  dc_codigo_barra = t.dc_codigo_barra,
                                  dc_tipo_titulo = t.dc_tipo_titulo,
                                  dc_nosso_numero = t.dc_nosso_numero,
                                  dc_num_documento_titulo = t.dc_num_documento_titulo,
                                  vl_saldo_titulo = t.vl_saldo_titulo,
                                  nm_titulo = t.nm_titulo,
                                  nm_parcela_titulo = t.nm_parcela_titulo,
                                  cd_tipo_financeiro = t.cd_tipo_financeiro,
                                  id_status_titulo = t.id_status_titulo,
                                  id_status_cnab = t.id_status_cnab,
                                  id_origem_titulo = t.id_origem_titulo,
                                  id_natureza_titulo = t.id_natureza_titulo,
                                  vl_multa_titulo = t.vl_multa_titulo,
                                  vl_juros_titulo = t.vl_juros_titulo,
                                  vl_desconto_titulo = t.vl_desconto_titulo,
                                  vl_liquidacao_titulo = t.vl_liquidacao_titulo,
                                  vl_multa_liquidada = t.vl_multa_liquidada,
                                  vl_juros_liquidado = t.vl_juros_liquidado,
                                  vl_desconto_juros = t.vl_desconto_juros,
                                  vl_desconto_multa = t.vl_desconto_multa,
                                  pc_juros_titulo = t.pc_juros_titulo,
                                  pc_multa_titulo = t.pc_multa_titulo,
                                  cd_plano_conta_tit = t.cd_plano_conta_tit,
                                  vl_material_titulo = t.vl_material_titulo,
                                  vl_abatimento = t.vl_abatimento,
                                  vl_desconto_contrato = t.vl_desconto_contrato 
                              }).ToList().Select(x=> new Titulo()
                                {
                                    cd_titulo = x.cd_titulo,
                                    cd_pessoa_empresa = x.cd_pessoa_empresa,
                                    cd_pessoa_titulo = x.cd_pessoa_titulo,
                                    cd_pessoa_responsavel = x.cd_pessoa_responsavel,
                                    cd_local_movto = x.cd_local_movto,
                                    dt_emissao_titulo = x.dt_emissao_titulo,
                                    cd_origem_titulo = x.cd_origem_titulo,
                                    dt_vcto_titulo = x.dt_vcto_titulo,
                                    dh_cadastro_titulo = x.dh_cadastro_titulo,
                                    vl_titulo = x.vl_titulo,
                                    dt_liquidacao_titulo = x.dt_liquidacao_titulo,
                                    dc_codigo_barra = x.dc_codigo_barra,
                                    dc_tipo_titulo = x.dc_tipo_titulo,
                                    dc_nosso_numero = x.dc_nosso_numero,
                                    dc_num_documento_titulo = x.dc_num_documento_titulo,
                                    vl_saldo_titulo = x.vl_saldo_titulo,
                                    nm_titulo = x.nm_titulo,
                                    nm_parcela_titulo = x.nm_parcela_titulo,
                                    cd_tipo_financeiro = x.cd_tipo_financeiro,
                                    id_status_titulo = x.id_status_titulo,
                                    id_status_cnab = x.id_status_cnab,
                                    id_origem_titulo = x.id_origem_titulo,
                                    id_natureza_titulo = x.id_natureza_titulo,
                                    vl_multa_titulo = x.vl_multa_titulo,
                                    vl_juros_titulo = x.vl_juros_titulo,
                                    vl_desconto_titulo = x.vl_desconto_titulo,
                                    vl_liquidacao_titulo = x.vl_liquidacao_titulo,
                                    vl_multa_liquidada = x.vl_multa_liquidada,
                                    vl_juros_liquidado = x.vl_juros_liquidado,
                                    vl_desconto_juros = x.vl_desconto_juros,
                                    vl_desconto_multa = x.vl_desconto_multa,
                                    pc_juros_titulo = x.pc_juros_titulo,
                                    pc_multa_titulo = x.pc_multa_titulo,
                                    cd_plano_conta_tit = x.cd_plano_conta_tit,
                                    vl_material_titulo = x.vl_material_titulo,
                                    vl_abatimento = x.vl_abatimento,
                                    vl_desconto_contrato = x.vl_desconto_contrato 
                                }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool updateNossoNumeroTitulo(int numeroDocumento, int cd_pessoa_empresa, int cd_local_movto, string nossoNumero)
        {
            try
            {
                string comand = "update t set " +
                                "t.dc_nosso_numero = @dc_nosso_numero " +
                                "from t_titulo t " +
                                "where " +
                                "t.cd_titulo = @numeroDocumento and " +
                                "t.cd_pessoa_empresa = @cd_pessoa_empresa and " +
                                "t.cd_local_movto = @cd_local_movto";
                int retorno = db.Database.ExecuteSqlCommand(comand, 
                    new SqlParameter("dc_nosso_numero", nossoNumero), 
                    new SqlParameter("numeroDocumento", numeroDocumento), 
                    new SqlParameter("cd_pessoa_empresa", cd_pessoa_empresa), 
                    new SqlParameter("cd_local_movto", cd_local_movto));

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Titulo getTituloBaixaFinanMovimentoNF(int cd_baixa_titulo, int cd_pessoa_empresa)
        {
            try
            {
                Titulo sql = (from b in db.BaixaTitulo
                              where b.Titulo.cd_pessoa_empresa == cd_pessoa_empresa
                              && b.cd_baixa_titulo == cd_baixa_titulo
                               select new
                               {
                                   cd_titulo = b.Titulo.cd_titulo,
                                   cd_pessoa_responsavel = b.Titulo.cd_pessoa_responsavel,
                                   nomeResponsavel = b.Titulo.PessoaResponsavel.no_pessoa,
                                   id_origem_titulo = b.Titulo.id_origem_titulo,
                                   dc_tipo_titulo = b.Titulo.dc_tipo_titulo,
                                   vl_titulo = b.Titulo.vl_titulo,
                                   cd_plano_conta_tit = b.Titulo.cd_plano_conta_tit,
                                   dc_plano_conta = b.Titulo.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta,
                                   nm_titulo = b.Titulo.nm_titulo,
                                   nm_parcela_titulo = b.Titulo.nm_parcela_titulo
                               }).ToList().Select(x => new Titulo
                               {
                                   cd_titulo = x.cd_titulo,
                                   cd_pessoa_responsavel = x.cd_pessoa_responsavel,
                                   nomeResponsavel = x.nomeResponsavel,
                                   id_origem_titulo = x.id_origem_titulo,
                                   dc_tipo_titulo = x.dc_tipo_titulo,
                                   vl_titulo = x.vl_titulo,
                                   cd_plano_conta_tit = x.cd_plano_conta_tit,
                                   dc_plano_conta = x.dc_plano_conta,
                                   nm_titulo = x.nm_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo
                               }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Titulo getTituloBaixaFinan(int cd_titulo, int cd_pessoa_empresa, TipoConsultaTituloEnum tipoConsulta)
        {
            try
            {
                Titulo sql = null;
                switch (tipoConsulta)
                {
                    case TipoConsultaTituloEnum.HAS_EDIT_TITULO_VIEW:
                        sql = (from t in db.Titulo
                               where t.cd_titulo == cd_titulo
                               && t.cd_pessoa_empresa == cd_pessoa_empresa
                               select new
                               {
                                   cd_titulo = t.cd_titulo,
                                   cd_pessoa_titulo = t.cd_pessoa_responsavel,
                                   no_pessoa_titulo = t.PessoaResponsavel.no_pessoa,
                                   cd_tipo_financeiro = t.cd_tipo_financeiro,
                                   tipoDoc = t.TipoFinanceiro.dc_tipo_financeiro,
                                   vl_titulo = t.vl_titulo,
                                   nm_titulo = t.nm_titulo,
                                   nm_parcela_titulo = t.nm_parcela_titulo,
                                   dt_emissao_titulo = t.dt_emissao_titulo,
                                   dt_vcto_titulo = t.dt_vcto_titulo,
                                   cd_local_movto = t.cd_local_movto,
                                   no_local_movto = t.LocalMovto.no_local_movto,
                                   vl_saldo_titulo = t.vl_saldo_titulo,
                                   pc_juros_titulo = t.pc_juros_titulo,
                                   pc_multa_titulo = t.pc_multa_titulo,
                                   //compos de plano de contas
                                   no_plano_contas = t.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta,
                                   cd_plano_conta_tit = t.cd_plano_conta_tit,
                                   id_natureza_titulo = t.id_natureza_titulo,
                                   vl_juros_titulo = t.vl_juros_titulo,
                                   vl_multa_titulo = t.vl_multa_titulo,
                                   vl_desconto_titulo = t.vl_desconto_titulo,
                                   dt_liquidacao_titulo = t.dt_liquidacao_titulo,
                                   vl_liquidacao_titulo = t.vl_liquidacao_titulo,
                                   vl_desconto_juros = t.vl_desconto_juros,
                                   vl_desconto_multa = t.vl_desconto_multa,
                                   vl_juros_liquidado = t.vl_juros_liquidado,
                                   vl_multa_liquidada = t.vl_multa_liquidada,
                                   id_status_titulo = t.id_status_titulo,
                                   dh_cadastro_titulo = t.dh_cadastro_titulo,
                                   id_origem_titulo = t.id_origem_titulo,
                                   cd_origem_titulo = t.cd_origem_titulo,
                                   dc_nosso_numero = t.dc_nosso_numero,
                                   id_status_cnab = t.id_status_cnab,
                                   pc_taxa_cartao = t.pc_taxa_cartao,
                                   nm_dias_cartao = t.nm_dias_cartao,
                                   vl_taxa_cartao = t.vl_taxa_cartao,
                                   cd_pessoa_empresa = t.cd_pessoa_empresa
                               }).ToList().Select(x => new Titulo
                                       {
                                           cd_titulo = x.cd_titulo,
                                           cd_pessoa_titulo = x.cd_pessoa_titulo,
                                           nomeResponsavel = x.no_pessoa_titulo,
                                           tipoDoc = x.tipoDoc,
                                           vl_titulo = x.vl_titulo,
                                           nm_titulo = x.nm_titulo,
                                           nm_parcela_titulo = x.nm_parcela_titulo,
                                           dt_emissao_titulo = x.dt_emissao_titulo,
                                           dt_vcto_titulo = x.dt_vcto_titulo,
                                           cd_local_movto = x.cd_local_movto,
                                           vl_saldo_titulo = x.vl_saldo_titulo,
                                           pc_juros_titulo = x.pc_juros_titulo,
                                           pc_multa_titulo = x.pc_multa_titulo,
                                           id_natureza_titulo = x.id_natureza_titulo,
                                           vl_juros_titulo = x.vl_juros_titulo,
                                           vl_multa_titulo = x.vl_multa_titulo,
                                           vl_desconto_titulo = x.vl_desconto_titulo,
                                           dt_liquidacao_titulo = x.dt_liquidacao_titulo,
                                           vl_liquidacao_titulo = x.vl_liquidacao_titulo,
                                           vl_desconto_juros = x.vl_desconto_juros,
                                           vl_desconto_multa = x.vl_desconto_multa,
                                           vl_juros_liquidado = x.vl_juros_liquidado,
                                           vl_multa_liquidada = x.vl_multa_liquidada,
                                           id_status_titulo = x.id_status_titulo,
                                           dh_cadastro_titulo = x.dh_cadastro_titulo,
                                           cd_tipo_financeiro = x.cd_tipo_financeiro,
                                           cd_plano_conta_tit = x.cd_plano_conta_tit,
                                           dc_plano_conta = x.no_plano_contas,
                                           id_origem_titulo = x.id_origem_titulo,
                                           cd_origem_titulo = x.cd_origem_titulo,
                                           dc_nosso_numero = x.dc_nosso_numero,
                                           id_status_cnab = x.id_status_cnab,
                                           pc_taxa_cartao = x.pc_taxa_cartao,
                                           nm_dias_cartao = x.nm_dias_cartao,
                                           vl_taxa_cartao = x.vl_taxa_cartao,
                                           cd_pessoa_empresa = x.cd_pessoa_empresa
                                       }).FirstOrDefault();

                        sql.BaixaTitulo = (from b in db.BaixaTitulo
                                           where sql.cd_titulo == b.cd_titulo
                                           select new
                                           {
                                               cd_tipo_liquidacao = b.cd_tipo_liquidacao
                                           }).ToList().Select(x => new BaixaTitulo
                                           {
                                               cd_tipo_liquidacao = x.cd_tipo_liquidacao
                                           }).ToList();
                        break;
                    case TipoConsultaTituloEnum.HAS_EDIT_TITULO:
                        sql = sql = (from t in db.Titulo
                                       where t.cd_titulo == cd_titulo
                                       && t.cd_pessoa_empresa == cd_pessoa_empresa
                                       select t).FirstOrDefault();
                        break;
                    case TipoConsultaTituloEnum.HAS_TITULO_GRADE:
                        sql = (from t in db.Titulo
                               where t.cd_titulo == cd_titulo
                               && t.cd_pessoa_empresa == cd_pessoa_empresa
                               select new
                                     {
                                         cd_titulo = t.cd_titulo,
                                         id_natureza_titulo = t.id_natureza_titulo,
                                         nomeResponsavel = t.PessoaResponsavel.no_pessoa,
                                         nm_parcela_titulo = t.nm_parcela_titulo,
                                         nm_titulo = t.nm_titulo,
                                         dt_emissao_titulo = t.dt_emissao_titulo,
                                         dt_vcto_titulo = t.dt_vcto_titulo,
                                         vl_titulo = t.vl_titulo,
                                         vl_saldo_titulo = t.vl_saldo_titulo,
                                         cd_pessoa_empresa = t.cd_pessoa_empresa,
                                         id_origem_titulo = t.id_origem_titulo,
                                         cd_origem_titulo = t.cd_origem_titulo,
                                         id_status_titulo = t.id_status_titulo,
                                         pc_juros_titulo = t.pc_juros_titulo,
                                         pc_multa_titulo = t.pc_multa_titulo
                                     }).ToList().Select(x => new Titulo
                                      {
                                          cd_titulo = x.cd_titulo,
                                          nomeResponsavel = x.nomeResponsavel,
                                          dt_emissao_titulo = x.dt_emissao_titulo,
                                          dt_vcto_titulo = x.dt_vcto_titulo,
                                          vl_saldo_titulo = x.vl_saldo_titulo,
                                          vl_titulo = x.vl_titulo,
                                          nm_parcela_titulo = x.nm_parcela_titulo,
                                          nm_titulo = x.nm_titulo,
                                          id_natureza_titulo = x.id_natureza_titulo,
                                          cd_pessoa_empresa = x.cd_pessoa_empresa,
                                          id_origem_titulo = x.id_origem_titulo,
                                          cd_origem_titulo = x.cd_origem_titulo,
                                          id_status_titulo = x.id_status_titulo,
                                          pc_juros_titulo = x.pc_juros_titulo,
                                          pc_multa_titulo = x.pc_multa_titulo
                                      }).FirstOrDefault();
                        break;
                    case TipoConsultaTituloEnum.TITULO_BAIXA_CNAB:
                        sql = sql = (from t in db.Titulo
                                     where t.cd_titulo == cd_titulo
                                     && t.cd_pessoa_empresa == cd_pessoa_empresa
                                     select t).FirstOrDefault();
                        break;
                    case TipoConsultaTituloEnum.HAS_TITULO_MOVIMENTO_NF:
                        sql = (from t in db.Titulo
                               where t.cd_titulo == cd_titulo
                               && t.cd_pessoa_empresa == cd_pessoa_empresa
                               select new
                               {
                                   cd_titulo = t.cd_titulo,
                                   cd_pessoa_responsavel = t.cd_pessoa_responsavel,
                                   nomeResponsavel = t.PessoaResponsavel.no_pessoa,
                                   id_origem_titulo = t.id_origem_titulo,
                                   dc_tipo_titulo = t.dc_tipo_titulo,
                                   vl_titulo = t.vl_titulo,
                                   cd_plano_conta_tit = t.cd_plano_conta_tit,
                                   dc_plano_conta = t.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta,
                                   nm_titulo = t.nm_titulo,
                                   nm_parcela_titulo = t.nm_parcela_titulo
                               }).ToList().Select(x => new Titulo
                               {
                                   cd_titulo = x.cd_titulo,
                                   cd_pessoa_responsavel = x.cd_pessoa_responsavel,
                                   nomeResponsavel = x.nomeResponsavel,
                                   id_origem_titulo = x.id_origem_titulo,
                                   dc_tipo_titulo = x.dc_tipo_titulo,
                                   vl_titulo = x.vl_titulo,
                                   cd_plano_conta_tit = x.cd_plano_conta_tit,
                                   dc_plano_conta = x.dc_plano_conta,
                                   nm_titulo = x.nm_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo
                               }).FirstOrDefault();
                        break;
                        
                    
                }
                
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getTituloByPessoa(SearchParameters parametros, int cd_pessoa, int cd_escola, TipoConsultaTituloEnum tipo, bool contaSeg)
        {
            try{
                IEntitySorter<Titulo> sorter = EntitySorter<Titulo>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Titulo> sql;
                sql = from t in db.Titulo.AsNoTracking()
                      where t.cd_pessoa_titulo == cd_pessoa
                      && t.cd_pessoa_empresa == cd_escola
                      select t;
                
                switch(tipo){
                    case TipoConsultaTituloEnum.TITULO_ABERTO:
                        sql = (from t in sql
                                   where t.id_status_titulo == (int) Titulo.StatusTitulo.ABERTO
                                   select t);
                        break;
                    case TipoConsultaTituloEnum.TITULO_FECHADO:
                        sql = (from t in sql
                                   where t.id_status_titulo == (int) Titulo.StatusTitulo.FECHADO
                                         && (!t.BaixaTitulo.Where(b => b.TransacaoFinanceira.cd_tipo_liquidacao == (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO).Any() ||
                                                ((from b in db.BaixaTitulo //tem baixa que não é de cancelamento e
                                                     where b.cd_titulo == t.cd_titulo &&
                                                           b.cd_tipo_liquidacao != (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO
                                                     select b).Any() &&
                                                 (from b in db.BaixaTitulo //tem baixa de cancelamento 
                                                     where b.cd_titulo == t.cd_titulo &&
                                                           b.cd_tipo_liquidacao == (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO
                                                     select b).Any())
                                            )
                                   select t);
                        break;
                    case TipoConsultaTituloEnum.TITULO_CANCELADO:
                        sql = (from t in sql
                                   where t.BaixaTitulo.Where(b => b.TransacaoFinanceira.cd_tipo_liquidacao == (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO).Any()
                                   select t);
                        break;
                }
                if (!contaSeg)
                    sql = from t in sql
                          where t.PlanoTitulo.Where(pt => pt.PlanoConta.id_conta_segura == false).Any()
                          select t;
                sql = sorter.Sort(sql);
                
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                var retorno = (from t in sql
                               select new {
                                   cd_titulo = t.cd_titulo,
                                   id_status_titulo = t.id_status_titulo,
                                   id_natureza_titulo = t.id_natureza_titulo,
                                   id_origem_titulo = t.id_origem_titulo,
                                   cd_origem_titulo = t.cd_origem_titulo,
                                   //cd_tipo_financeiro = t.cd_tipo_financeiro,
                                   tipoDoc = t.TipoFinanceiro.dc_tipo_financeiro,
                                   nm_titulo = t.nm_titulo,
                                   nm_parcela_titulo = t.nm_parcela_titulo,
                                   dt_vcto_titulo = t.dt_vcto_titulo,
                                   vl_saldo_titulo = t.vl_saldo_titulo,
                                   vl_liquidacao_titulo = t.vl_liquidacao_titulo,
                                   dt_liquidacao_titulo = t.dt_liquidacao_titulo,
                                   LocalMovto = t.LocalMovto,
                                   dc_tipo_liquidacao = (tipo == TipoConsultaTituloEnum.TITULO_CANCELADO) ? t.BaixaTitulo.Where(c => c.TransacaoFinanceira.cd_tipo_liquidacao == (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO).OrderByDescending(bt => bt.dt_baixa_titulo).FirstOrDefault().TransacaoFinanceira.TipoLiquidacao.dc_tipo_liquidacao :
                                                        (tipo == TipoConsultaTituloEnum.TITULO_FECHADO)? t.BaixaTitulo.Where(c => c.TransacaoFinanceira.cd_tipo_liquidacao != (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO).OrderByDescending(bt => bt.dt_baixa_titulo).FirstOrDefault().TransacaoFinanceira.TipoLiquidacao.dc_tipo_liquidacao :
                                                         t.BaixaTitulo.OrderByDescending(bt => bt.dt_baixa_titulo).FirstOrDefault().TransacaoFinanceira.TipoLiquidacao.dc_tipo_liquidacao,
                                   
                                   vl_liquidacao_baixa = (tipo == TipoConsultaTituloEnum.TITULO_CANCELADO && t.BaixaTitulo.Count > 0 ) ? t.BaixaTitulo.Where(z =>  z.TransacaoFinanceira.cd_tipo_liquidacao == (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO).Sum(bt => bt.vl_liquidacao_baixa ):
                                       (tipo == TipoConsultaTituloEnum.TITULO_FECHADO && t.BaixaTitulo.Count > 0) ? t.BaixaTitulo.Where(z => z.TransacaoFinanceira.cd_tipo_liquidacao != (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO).Sum(bt => bt.vl_liquidacao_baixa) :
                                       (tipo == TipoConsultaTituloEnum.TITULO_ABERTO && t.BaixaTitulo.Count > 0) ? t.BaixaTitulo.Where(z => z.TransacaoFinanceira.cd_tipo_liquidacao != (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO).Sum(bt => bt.vl_liquidacao_baixa) :
                                       //opção todos -> (HAS_EDIT_TITULO_VIEW)
                                       (tipo == TipoConsultaTituloEnum.HAS_EDIT_TITULO_VIEW && t.BaixaTitulo.Count > 0 && t.BaixaTitulo.Where(b => b.TransacaoFinanceira.cd_tipo_liquidacao == (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO).Any()) ? t.BaixaTitulo.Where(z => z.TransacaoFinanceira.cd_tipo_liquidacao == (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO).Sum(bt => bt.vl_liquidacao_baixa) :
                                       (tipo == TipoConsultaTituloEnum.HAS_EDIT_TITULO_VIEW && t.BaixaTitulo.Count > 0 && t.id_status_titulo == (int)Titulo.StatusTitulo.FECHADO && t.BaixaTitulo.Where(z => z.TransacaoFinanceira.cd_tipo_liquidacao != (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO).Any()) ? t.BaixaTitulo.Where(z => z.TransacaoFinanceira.cd_tipo_liquidacao != (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO).Sum(bt => bt.vl_liquidacao_baixa) :
                                       (tipo == TipoConsultaTituloEnum.HAS_EDIT_TITULO_VIEW && t.BaixaTitulo.Count > 0 && t.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO && t.BaixaTitulo.Where(z => z.TransacaoFinanceira.cd_tipo_liquidacao != (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO).Any()) ? t.BaixaTitulo.Where(z => z.TransacaoFinanceira.cd_tipo_liquidacao != (int)TransacaoFinanceira.TipoCancelamento.CANCELAMENTO).Sum(bt => bt.vl_liquidacao_baixa) :
                                        0,
                                   dc_tipo_titulo = t.dc_tipo_titulo,
                                   vl_material_titulo = t.vl_material_titulo,
                                   vl_titulo = t.vl_titulo
                               }).ToList().Select(x => new Titulo {
                                   cd_titulo = x.cd_titulo,
                                   id_status_titulo = x.id_status_titulo,
                                   id_natureza_titulo = x.id_natureza_titulo,
                                   id_origem_titulo = x.id_origem_titulo,
                                   cd_origem_titulo = x.cd_origem_titulo,
                                   //cd_tipo_financeiro = x.cd_tipo_financeiro,
                                   tipoDoc = x.tipoDoc,
                                   nm_titulo = x.nm_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   vl_saldo_titulo = x.vl_saldo_titulo,
                                   vl_liquidacao_titulo = x.vl_liquidacao_titulo,
                                   dt_liquidacao_titulo = x.dt_liquidacao_titulo,
                                   LocalMovto = x.LocalMovto,
                                   dc_tipo_liquidacao = x.dc_tipo_liquidacao,
                                   vl_liquidacao_baixa = x.vl_liquidacao_baixa,
                                   dc_tipo_titulo = x.dc_tipo_titulo,
                                   vl_material_titulo = x.vl_material_titulo,
                                   vl_titulo = x.vl_titulo
                               });
                return retorno; 
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getTituloByPessoaResponsavel(int cd_pessoa_titulo, int cd_escola, int cd_contrato, bool contaSeg)
        {
            try
            {
                IQueryable<Titulo> sql;
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                if (contaSeg)
                    sql = from t in db.Titulo
                          where t.cd_pessoa_titulo == cd_pessoa_titulo
                          && t.cd_pessoa_empresa == cd_escola
                          && t.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO
                          && t.vl_saldo_titulo > 0
                          && t.cd_origem_titulo == cd_contrato
                          && t.id_origem_titulo == cd_origem
                          select t;
                else
                    sql = from t in db.Titulo                           
                          where t.cd_pessoa_titulo == cd_pessoa_titulo
                          && t.cd_pessoa_empresa == cd_escola
                          && t.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO
                          && t.vl_saldo_titulo > 0
                          && t.cd_origem_titulo == cd_contrato
                          && t.id_origem_titulo == cd_origem
                          && t.PlanoTitulo.Where(pt => pt.PlanoConta.id_conta_segura == false).Any()
                          select t;

                var retorno = (from t in sql
                               select new
                               {
                                   cd_pessoa_empresa = t.cd_pessoa_empresa,
                                   cd_pessoa_titulo = t.cd_pessoa_titulo,
                                   cd_pessoa_responsavel = t.cd_pessoa_responsavel,
                                   cd_titulo = t.cd_titulo,
                                   cd_tipo_financeiro = t.cd_tipo_financeiro,
                                   id_status_titulo = t.id_status_titulo,
                                   tipoDoc = t.TipoFinanceiro.dc_tipo_financeiro,
                                   nm_titulo = t.nm_titulo,
                                   nm_parcela_titulo = t.nm_parcela_titulo,
                                   dt_vcto_titulo = t.dt_vcto_titulo,                                   
                                   vl_saldo_titulo = t.vl_saldo_titulo,
                                   vl_liquidacao_titulo = t.vl_liquidacao_titulo,
                                   dt_liquidacao_titulo = t.dt_liquidacao_titulo,
                                   dt_emissao_titulo = t.dt_emissao_titulo,
                                   vl_titulo = t.vl_titulo,
                                   pc_juros_titulo = t.pc_juros_titulo,
                                   pc_multa_titulo = t.pc_multa_titulo,
                                   dc_tipo_titulo = t.dc_tipo_titulo,
                                   nomeResponsavel = t.PessoaResponsavel.no_pessoa,
                                   nomeAluno = t.Pessoa.no_pessoa,
                                   cheques = db.Cheque.Where(x => x.cd_contrato == cd_contrato).FirstOrDefault(),
                                   dc_num_documento_titulo = t.dc_num_documento_titulo
                               }).ToList().Select(x => new Titulo
                               {
                                   cd_pessoa_empresa = x.cd_pessoa_empresa,
                                   cd_pessoa_titulo = x.cd_pessoa_titulo,
                                   cd_pessoa_responsavel = x.cd_pessoa_responsavel,
                                   cd_titulo = x.cd_titulo,
                                   cd_tipo_financeiro = x.cd_tipo_financeiro,
                                   id_status_titulo = x.id_status_titulo,
                                   tipoDoc = x.tipoDoc,
                                   nm_titulo = x.nm_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   vl_saldo_titulo = x.vl_saldo_titulo,
                                   vl_liquidacao_titulo = x.vl_liquidacao_titulo,
                                   dt_liquidacao_titulo = x.dt_liquidacao_titulo,
                                   dt_emissao_titulo = x.dt_emissao_titulo,
                                   vl_titulo = x.vl_titulo,
                                   pc_juros_titulo = x.pc_juros_titulo,
                                   pc_multa_titulo = x.pc_multa_titulo,
                                   dc_tipo_titulo = x.dc_tipo_titulo,
                                   nomeResponsavel = x.nomeResponsavel,
                                   nomeAluno = x.nomeAluno,
                                   Cheque = x.cheques,
                                   dc_num_documento_titulo = x.dc_num_documento_titulo
                               });
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getTitulosByTransacaoFinanceira(int cd_pessoa_empresa, int cd_tran_finan)
        {
            try
            {
               var sql = (from t in db.Titulo
                         where t.cd_pessoa_empresa == cd_pessoa_empresa &&
                         t.BaixaTitulo.Where(b => b.cd_tran_finan == cd_tran_finan).Any()
                         select new
                            {
                                cd_titulo = t.cd_titulo,
                                id_natureza_titulo = t.id_natureza_titulo,
                                nomeResponsavel = t.PessoaResponsavel.no_pessoa,
                                nm_parcela_titulo = t.nm_parcela_titulo,
                                nm_titulo = t.nm_titulo,
                                dt_emissao_titulo = t.dt_emissao_titulo,
                                dt_vcto_titulo = t.dt_vcto_titulo,
                                vl_titulo = t.vl_titulo,
                                vl_saldo_titulo = t.vl_saldo_titulo,
                                cd_pessoa_empresa = t.cd_pessoa_empresa,
                                id_origem_titulo = t.id_origem_titulo,
                                cd_origem_titulo = t.cd_origem_titulo,
                                id_status_titulo = t.id_status_titulo,
                                possuiBaixa = t.BaixaTitulo.Any(),
                                t.vl_material_titulo
                            }).ToList().Select(x => new Titulo
                            {
                                cd_titulo = x.cd_titulo,
                                nomeResponsavel = x.nomeResponsavel,
                                dt_emissao_titulo = x.dt_emissao_titulo,
                                dt_vcto_titulo = x.dt_vcto_titulo,
                                vl_saldo_titulo = x.vl_saldo_titulo,
                                vl_titulo = x.vl_titulo,
                                nm_parcela_titulo = x.nm_parcela_titulo,
                                nm_titulo = x.nm_titulo,
                                id_natureza_titulo = x.id_natureza_titulo,
                                cd_pessoa_empresa = x.cd_pessoa_empresa,
                                id_origem_titulo = x.id_origem_titulo,
                                cd_origem_titulo = x.cd_origem_titulo,
                                id_status_titulo = x.id_status_titulo,
                                possuiBaixa = x.possuiBaixa,
                                vl_material_titulo = x.vl_material_titulo
                            });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Titulo> getTitulosGridByMovimento(int cd_movto, int cd_empresa, int? cd_aluno)
        {
            try
                
            {
                SGFWebContext dbComp = new SGFWebContext();
                var nmAluno = db.Aluno.Where(a => a.cd_aluno == cd_aluno).Include(x => x.AlunoPessoaFisica).FirstOrDefault();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Movimento"].ToString());
                List<Titulo> sql = ((from i in db.Titulo
                                     where i.cd_origem_titulo == cd_movto && i.id_origem_titulo == cd_origem && 
                                       i.cd_pessoa_empresa == cd_empresa
                                     select new
                                     {
                                         cd_titulo = i.cd_titulo,
                                         cd_origem_titulo = i.cd_origem_titulo,
                                         //cd_pessoa_empresa = i.cd_pessoa_empresa,
                                         cd_pessoa_titulo = i.cd_pessoa_titulo,
                                         cd_pessoa_responsavel = i.cd_pessoa_responsavel,
                                         nomeResponsavel = i.PessoaResponsavel.no_pessoa,
                                         tipoDoc = i.TipoFinanceiro.dc_tipo_financeiro,
                                         cd_tipo_financeiro = i.cd_tipo_financeiro,
                                         dt_emissao_titulo = i.dt_emissao_titulo,
                                         dt_vcto_titulo = i.dt_vcto_titulo,
                                         vl_saldo_titulo = i.vl_saldo_titulo,
                                         vl_titulo = i.vl_titulo,
                                         dc_tipo_titulo = i.dc_tipo_titulo,
                                         nm_parcela_titulo = i.nm_parcela_titulo,
                                         dc_num_documento_titulo = i.dc_num_documento_titulo,
                                         nm_titulo = i.nm_titulo,
                                         tituloEdit = false,
                                         id = i.cd_titulo,
                                         //vl_liquidacao_titulo = i.vl_liquidacao_titulo,
                                         cd_local_movto = i.cd_local_movto,
                                         nm_banco = i.LocalMovto.nm_banco,
                                         no_local_movto = i.LocalMovto.no_local_movto,
                                         nm_agencia = i.LocalMovto.nm_agencia,
                                         nm_conta_corrente = i.LocalMovto.nm_conta_corrente,
                                         nm_tipo_local = i.LocalMovto.nm_tipo_local,
                                         id_status_cnab = i.id_status_cnab,
                                         possuiBaixa = i.BaixaTitulo.Any(),
                                         cheques = db.Cheque.Where(x => x.cd_movimento == cd_movto).FirstOrDefault(),
                                         //id_origem_titulo = i.id_origem_titulo,
                                         id_natureza_titulo = i.id_natureza_titulo,
                                         pc_taxa_cartao = i.pc_taxa_cartao,
                                         nm_dias_cartao = i.nm_dias_cartao,
                                         vl_taxa_cartao = i.vl_taxa_cartao
                                     }).ToList().Select(x => new Titulo
                                     {
                                         cd_titulo = x.cd_titulo,
                                         cd_origem_titulo = x.cd_origem_titulo,
                                         //cd_pessoa_empresa = x.cd_pessoa_empresa,
                                         cd_pessoa_titulo = x.cd_pessoa_titulo,
                                         cd_pessoa_responsavel = x.cd_pessoa_responsavel,
                                         nomeResponsavel = x.nomeResponsavel,
                                         nomeAluno = (nmAluno != null && nmAluno.AlunoPessoaFisica != null) ? nmAluno.AlunoPessoaFisica.no_pessoa : x.nomeResponsavel,
                                         tipoDoc = x.tipoDoc,
                                         cd_tipo_financeiro = x.cd_tipo_financeiro,
                                         dt_emissao_titulo = x.dt_emissao_titulo,
                                         dt_vcto_titulo = x.dt_vcto_titulo,
                                         vl_saldo_titulo = x.vl_saldo_titulo,
                                         vl_titulo = x.vl_titulo,
                                         dc_tipo_titulo = x.dc_tipo_titulo,
                                         nm_parcela_titulo = x.nm_parcela_titulo,
                                         dc_num_documento_titulo = x.dc_num_documento_titulo,
                                         nm_titulo = x.nm_titulo,
                                         possuiBaixa = x.possuiBaixa,
                                         id = x.id,
                                         //vl_liquidacao_titulo = x.vl_liquidacao_titulo,
                                         cd_local_movto = x.cd_local_movto,
                                         LocalMovto = new LocalMovto
                                         {
                                             no_local_movto = x.no_local_movto,
                                             nm_agencia = x.nm_agencia,
                                             nm_conta_corrente = x.nm_conta_corrente,
                                             nm_tipo_local = x.nm_tipo_local
                                         },
                                         id_status_cnab = x.id_status_cnab,
                                         Cheque = x.cheques,
                                         //id_origem_titulo = x.id_origem_titulo,
                                         id_natureza_titulo = x.id_natureza_titulo,
                                         pc_taxa_cartao = x.pc_taxa_cartao,
                                         nm_dias_cartao = x.nm_dias_cartao,
                                         vl_taxa_cartao = x.vl_taxa_cartao
                                     })).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getTitulosByMovimento(int cd_movto, int cd_empresa)
        {
            SGFWebContext dbComp = new SGFWebContext();
            try
            {
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Movimento"].ToString());
                var sql = from i in db.Titulo
                          where i.cd_origem_titulo == cd_movto && i.id_origem_titulo == cd_origem &&
                            i.cd_pessoa_empresa == cd_empresa
                          select i;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificarSeExisteTitulosAnterioresAberto(List<int> cdTitulos, int cd_empresa)
        {
            {
                try
                {
                    bool existe = (from t in db.Titulo
                                     where  t.cd_pessoa_empresa == cd_empresa && 
                                            t.id_status_titulo == (byte)Titulo.StatusTitulo.ABERTO &&
                                            t.dc_tipo_titulo != "TM" && t.dc_tipo_titulo != "TA" &&  
                                            !cdTitulos.Contains(t.cd_titulo) && 
                                     db.Titulo.Where(x => x.id_origem_titulo == t.id_origem_titulo && x.cd_origem_titulo == t.cd_origem_titulo && x.cd_pessoa_empresa == cd_empresa &&
                                       t.dt_vcto_titulo < x.dt_vcto_titulo && cdTitulos.Contains(x.cd_titulo) && x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA").Any()
                                     select t.cd_titulo).Any();
                    return existe;
                }
                catch (Exception exe)
                {
                    throw new DataAccessException(exe);
                }
            }
        }

        public bool verificarTituloContaSegura(int cd_titulo, int cd_empresa)
        {
            {
                try
                {
                    var contaSegura = (from i in db.Titulo
                                      join pt in db.PlanoTitulo
                                      on i.cd_titulo equals pt.cd_titulo
                                      join pc in db.PlanoConta
                                      on pt.cd_plano_conta equals pc.cd_plano_conta
                                      where i.cd_pessoa_empresa == cd_empresa
                                              && i.cd_titulo == cd_titulo
                                      select pc.id_conta_segura).FirstOrDefault();
                    return contaSegura;
                }
                catch (Exception exe)
                {
                    throw new DataAccessException(exe);
                }
            }
        }

        public IEnumerable<Titulo> getTitulosByOrigem(int cdOrigemTitulo, int idOrigemTitulo, int cd_empresa)
        {
            try
            {
                var sql = from i in db.Titulo
                          where i.cd_origem_titulo == cdOrigemTitulo && i.id_origem_titulo == idOrigemTitulo &&
                            i.cd_pessoa_empresa == cd_empresa
                          select i;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //TO DO: Deivid (refatorar o metodo para verificações que não seja proveniente do incluir)
        public bool verificarStatusCnabTitulo(int[] cdTituls, int cd_empresa, int cd_cnab, byte id_tipo_cnab)
        {
            try
            {
                int[] cdStatusTituloCnab = null;
                switch (id_tipo_cnab)
                {
                    case (int)Cnab.TipoCnab.GERAR_BOLETOS:
                        cdStatusTituloCnab = new int[] { (int)Titulo.StatusCnabTitulo.INICIAL, (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA };
                        break;
                    case (int)Cnab.TipoCnab.CANCELAR_BOLETOS:
                        cdStatusTituloCnab = new int[1];
                        cdStatusTituloCnab[0] = (int)Titulo.StatusCnabTitulo.ENVIADO_GERADO;
                        break;
                    case (int)Cnab.TipoCnab.PEDIDO_BAIXA:
                        cdStatusTituloCnab = new int[3];
                        cdStatusTituloCnab[0] = (int)Titulo.StatusCnabTitulo.BAIXA_MANUAL;
                        cdStatusTituloCnab[1] = (int)Titulo.StatusCnabTitulo.CONFIRMADO_ENVIO;
                        cdStatusTituloCnab[2] = (int)Titulo.StatusCnabTitulo.BAIXA_MANUAL_CONFIRMADO;
                        break;
                }
                if (cdStatusTituloCnab == null)
                {
                    cdStatusTituloCnab = new int[1];
                    cdStatusTituloCnab[0] = (int)Titulo.StatusCnabTitulo.INICIAL;
                }
                var sql = from t in db.Titulo
                          where t.cd_pessoa_empresa == cd_empresa && cdTituls.Contains(t.cd_titulo) &&
                               t.TitulosCnab.Any(x => x.cd_cnab != cd_cnab && x.Cnab.id_status_cnab == (int)Cnab.StatusCnab.ABERTO) &&
                               cdStatusTituloCnab.Contains(t.id_status_cnab)
                          select t;
                return sql.Count() > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getDadosAdicionaisTituloParaCnab(int[] cdTitulos, int cd_empresa)
        {
            try
            {
                SGFWebContext dbContext = new SGFWebContext();
                int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var retorno = (from t in db.Titulo
                               where t.cd_pessoa_empresa == cd_empresa && cdTitulos.Contains(t.cd_titulo)
                               select new
                               {
                                   cd_titulo = t.cd_titulo,
                                   cd_pessoa_responsavel = t.cd_pessoa_responsavel,
                                   cd_pessoa_titulo = t.cd_pessoa_titulo,
                                   nomeAluno = t.Pessoa.no_pessoa,
                                   vl_saldo_titulo = t.vl_saldo_titulo,
                                   id_origem_titulo = t.id_origem_titulo,
                                   cd_origem_titulo = t.cd_origem_titulo,
                                   pc_juros_titulo = t.pc_juros_titulo,
                                   pc_multa_titulo = t.pc_multa_titulo,
                                   id_status_cnab = t.id_status_cnab,
                                   nosso_numero = t.dc_nosso_numero,
                                   vl_material_titulo = t.vl_material_titulo,
                                   vl_pago_bolsa = t.BaixaTitulo.Any(x=> x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA || 
                                                                         x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO) ?
                                                   t.BaixaTitulo.Where(x => x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA ||
                                                                            x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO
                                                   ).Sum(x=> x.vl_baixa_saldo_titulo) : 0,
                                   //Verificação onde pega o código da turma, só existe turma quando o contrato tem uma turma e o título é de origem do contrato, por isso os "case when"
                                   cd_aluno = t.id_origem_titulo == cd_origem ? db.Contrato.Where(c => 
                                        c.cd_contrato == t.cd_origem_titulo && 
                                        t.id_origem_titulo == cd_origem && 
                                        c.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo).Any() ?
                                                  db.Contrato.Where(c => 
                                                      c.cd_contrato == t.cd_origem_titulo && 
                                                      t.id_origem_titulo == cd_origem && 
                                                      c.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo).FirstOrDefault().Aluno.cd_aluno : 0 : 0,
                                   //Turma
                                   cd_turma_titulo = t.id_origem_titulo == cd_origem ?
                                   db.AlunoTurma.Where(a =>
                                                                      a.cd_contrato == t.cd_origem_titulo &&
                                                                      t.id_origem_titulo == cd_origem &&
                                                                      a.Contrato.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo &&
                                                                      db.HistoricoAluno.Where(h =>
                                                                              h.cd_turma == a.cd_turma &&
                                                                              h.cd_aluno == a.cd_aluno &&
                                                                              h.cd_contrato == a.cd_contrato &&
                                                                              h.dt_historico <= t.dt_vcto_titulo &&
                                                                              h.cd_produto == a.Contrato.cd_produto_atual &&
                                                                              h.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_aluno == a.cd_aluno && han.cd_contrato == a.cd_contrato
                                                                                           && DbFunctions.TruncateTime(han.dt_historico) <= t.dt_vcto_titulo
                                                                                           ).Max(x => x.nm_sequencia)).Any()).Any() ?
                                                                db.AlunoTurma.Where(a =>
                                                                      a.cd_contrato == t.cd_origem_titulo &&
                                                                      t.id_origem_titulo == cd_origem &&
                                                                      a.Contrato.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo &&
                                                                      db.HistoricoAluno.Where(h =>
                                                                              h.cd_turma == a.cd_turma &&
                                                                              h.cd_aluno == a.cd_aluno &&
                                                                              h.cd_contrato == a.cd_contrato &&
                                                                              h.dt_historico <= t.dt_vcto_titulo &&
                                                                              h.cd_produto == a.Contrato.cd_produto_atual &&
                                                                              h.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_aluno == a.cd_aluno && han.cd_contrato == a.cd_contrato
                                                                                           && DbFunctions.TruncateTime(han.dt_historico) <= t.dt_vcto_titulo
                                                                                           ).Max(x => x.nm_sequencia)).Any()).FirstOrDefault().cd_turma : 0 : 0,
                                   no_turma_titulo = t.id_origem_titulo == cd_origem ?
                                              db.AlunoTurma.Where(a =>
                                                                      a.cd_contrato == t.cd_origem_titulo &&
                                                                      t.id_origem_titulo == cd_origem &&
                                                                      a.Contrato.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo &&
                                                                      db.HistoricoAluno.Where(h =>
                                                                              h.cd_turma == a.cd_turma &&
                                                                              h.cd_aluno == a.cd_aluno &&
                                                                              h.cd_contrato == a.cd_contrato &&
                                                                              h.dt_historico <= t.dt_vcto_titulo &&
                                                                              h.cd_produto == a.Contrato.cd_produto_atual &&
                                                                              h.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_aluno == a.cd_aluno && han.cd_contrato == a.cd_contrato
                                                                                           && DbFunctions.TruncateTime(han.dt_historico) <= t.dt_vcto_titulo
                                                                                           ).Max(x => x.nm_sequencia)).Any()).Any() ?
                                                        db.AlunoTurma.Where(a =>
                                                                      a.cd_contrato == t.cd_origem_titulo &&
                                                                      t.id_origem_titulo == cd_origem &&
                                                                      a.Contrato.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo &&
                                                                      db.HistoricoAluno.Where(h =>
                                                                              h.cd_turma == a.cd_turma &&
                                                                              h.cd_aluno == a.cd_aluno &&
                                                                              h.cd_contrato == a.cd_contrato &&
                                                                              h.dt_historico <= t.dt_vcto_titulo &&
                                                                              h.cd_produto == a.Contrato.cd_produto_atual &&
                                                                              h.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_aluno == a.cd_aluno && han.cd_contrato == a.cd_contrato
                                                                                           && DbFunctions.TruncateTime(han.dt_historico) <= t.dt_vcto_titulo
                                                                                           ).Max(x => x.nm_sequencia)).Any()).FirstOrDefault().Turma.no_turma : "" : "",
                                   cd_produto = t.id_origem_titulo == cd_origem ?
                                                 db.Contrato.Where(c =>
                                                    c.cd_contrato == t.cd_origem_titulo &&
                                                    t.id_origem_titulo == cd_origem &&
                                                    c.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo).Any() ? db.Contrato.Where(c =>
                                                    c.cd_contrato == t.cd_origem_titulo &&
                                                    t.id_origem_titulo == cd_origem &&
                                                    c.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo).FirstOrDefault().cd_produto_atual : 0 : 0,
                                   nro_contrato = t.id_origem_titulo == cd_origem ?
                                                 db.Contrato.Where(c =>
                                                    c.cd_contrato == t.cd_origem_titulo &&
                                                    t.id_origem_titulo == cd_origem &&
                                                    c.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo).Any() ? db.Contrato.Where(c =>
                                                    c.cd_contrato == t.cd_origem_titulo &&
                                                    t.id_origem_titulo == cd_origem &&
                                                    c.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo).FirstOrDefault().nm_contrato : 0 : 0
                               }).ToList().Select(x => new Titulo
                               {
                                   cd_titulo = x.cd_titulo,
                                   cd_pessoa_responsavel = x.cd_pessoa_responsavel,
                                   cd_pessoa_titulo = x.cd_pessoa_titulo,
                                   cd_turma_titulo = x.cd_turma_titulo,
                                   cd_produto = x.cd_produto,
                                   cd_aluno = x.cd_aluno,
                                   no_turma_titulo = x.no_turma_titulo,
                                   nomeResponsavel = x.nomeAluno,
                                   nomeAluno = x.nomeAluno,
                                   nm_contrato = x.nro_contrato,
                                   vl_saldo_titulo = x.vl_saldo_titulo,
                                   id_origem_titulo = x.id_origem_titulo,
                                   cd_origem_titulo = x.cd_origem_titulo,
                                   pc_juros_titulo = x.pc_juros_titulo,
                                   pc_multa_titulo = x.pc_multa_titulo,
                                   id_status_cnab = x.id_status_cnab,
                                   dc_nosso_numero = x.nosso_numero,
                                   vl_material_titulo = x.vl_material_titulo,
                                   vl_pago_bolsa = x.vl_pago_bolsa
                               });
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getTitulosByCnab(int cdCnab, int cd_empresa)
        {
            try
            {
                var sql = from t in db.Titulo
                          where t.cd_pessoa_empresa == cd_empresa &&
                                 t.TitulosCnab.Where(tc => tc.cd_cnab == cdCnab).Any()
                          select t;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getTitulosByCnab(int[] cdCnabs, int cd_empresa)
        {
            try
            {
                var sql = from t in db.Titulo
                          where t.TitulosCnab.Where(tc => cdCnabs.Contains(tc.cd_cnab) ).Any() &&
                          t.cd_pessoa_empresa == cd_empresa
                          select t;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool existCnabTitulo(int cd_titulo, int cd_empresa)
        {
            try
            {
                bool sql = (from t in db.Titulo
                            where t.TitulosCnab.Where(tc => tc.cd_titulo == cd_titulo).Any() &&
                          t.cd_pessoa_empresa == cd_empresa
                          select t.cd_titulo).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunosSemTituloGeradoUI> GetAlunosSemTituloGerado(int vl_mes, int ano, int cd_turma, string situacoes, int cd_escola)
        {
           
            SGFWebContext dbComp = new SGFWebContext();
            int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
            try
            {
                IEnumerable<AlunosSemTituloGeradoUI> sql2;
                List<int> sit = situacoes.Split('|').Select(item => int.Parse(item)).ToList();

                

                 if ((sit.Count > 0 && !sit.Contains(100)))
                {
                    if (cd_turma == 0)
                    {

                        sql2 = (from at in db.AlunoTurma
                                where sit.Contains((int)at.cd_situacao_aluno_turma) &&
                                    at.Aluno.cd_pessoa_escola == cd_escola &&

                                     !(from t in db.Titulo
                                       where at.cd_contrato == t.cd_origem_titulo &&
                                               t.id_origem_titulo == cd_origem &&
                                               at.Aluno.cd_pessoa_escola == cd_escola &&
                                               t.dt_vcto_titulo.Year == ano &&
                                               t.dt_vcto_titulo.Month == vl_mes
                                       select t).Any() &&

                                            db.HistoricoAluno.Where(h =>
                                               h.cd_contrato == at.cd_contrato &&
                                               h.dt_historico.Year == ano &&
                                               h.dt_historico.Month == vl_mes &&
                                               h.cd_aluno == at.cd_aluno).OrderByDescending(o => o.nm_sequencia).Any() &&

                                            (((at.Turma.id_turma_ppt == false && at.Turma.cd_turma_ppt == null)) ||
                                            ((at.Turma.id_turma_ppt == false && at.Turma.cd_turma_ppt != null && at.Turma != null)))

                                select new
                                {
                                    nome_aluno = at.Aluno.AlunoPessoaFisica.no_pessoa,
                                    nome_turma = at.Turma.no_turma
                                }).ToList().Select(x => new AlunosSemTituloGeradoUI
                                {
                                    nome_aluno = x.nome_aluno,
                                    nome_turma = x.nome_turma
                                });
                    }
                    else
                    {
                        sql2 = (from at in db.AlunoTurma
                                where sit.Contains((int)at.cd_situacao_aluno_turma) &&
                                    at.Aluno.cd_pessoa_escola == cd_escola &&

                                     !(from t in db.Titulo
                                       where at.cd_contrato == t.cd_origem_titulo &&
                                               t.id_origem_titulo == cd_origem &&
                                               at.Aluno.cd_pessoa_escola == cd_escola &&
                                               t.dt_vcto_titulo.Year == ano &&
                                               t.dt_vcto_titulo.Month == vl_mes
                                       select t).Any() &&

                                            db.HistoricoAluno.Where(h =>
                                               h.cd_contrato == at.cd_contrato &&
                                               h.dt_historico.Year == ano &&
                                               h.dt_historico.Month == vl_mes &&
                                               h.cd_aluno == at.cd_aluno).OrderByDescending(o => o.nm_sequencia).Any() &&

                                            (((at.Turma.id_turma_ppt == false && at.Turma.cd_turma_ppt == null && at.Turma.cd_turma == cd_turma)) ||
                                            ((at.Turma.id_turma_ppt == false && at.Turma.cd_turma_ppt == cd_turma && at.Turma != null) || (at.Turma.id_turma_ppt == false && at.Turma.cd_turma_ppt != null && at.Turma.cd_turma == cd_turma)))

                                select new
                                {
                                    nome_aluno = at.Aluno.AlunoPessoaFisica.no_pessoa,
                                    nome_turma = at.Turma.no_turma
                                }).ToList().Select(x => new AlunosSemTituloGeradoUI
                                {
                                    nome_aluno = x.nome_aluno,
                                    nome_turma = x.nome_turma
                                });
                    }

                }
                else
                {
                    if (cd_turma == 0)
                    {

                        sql2 = (from at in db.AlunoTurma
                                where at.Aluno.cd_pessoa_escola == cd_escola &&
                                    !(from t in db.Titulo
                                      where at.cd_contrato == t.cd_origem_titulo &&
                                              t.id_origem_titulo == cd_origem &&
                                              at.Aluno.cd_pessoa_escola == cd_escola &&
                                              t.dt_vcto_titulo.Year == ano &&
                                              t.dt_vcto_titulo.Month == vl_mes
                                      select t).Any() &&

                                    db.HistoricoAluno.Where(h =>
                                       h.cd_contrato == at.cd_contrato &&
                                       h.dt_historico.Year == ano &&
                                       h.dt_historico.Month == vl_mes &&
                                       h.cd_aluno == at.cd_aluno).OrderByDescending(o => o.nm_sequencia).Any() &&

                                    ((((at.Turma.id_turma_ppt == false && at.Turma.cd_turma_ppt == null)) ||
                                    ((at.Turma.id_turma_ppt == false && at.Turma.cd_turma_ppt != null && at.Turma != null))))

                                select new
                                {
                                    nome_aluno = at.Aluno.AlunoPessoaFisica.no_pessoa,
                                    nome_turma = at.Turma.no_turma
                                }).ToList().Select(x => new AlunosSemTituloGeradoUI
                                {
                                    nome_aluno = x.nome_aluno,
                                    nome_turma = x.nome_turma
                                }).ToList();
                    }
                    else
                    {
                        sql2 = (from at in db.AlunoTurma
                                where at.Aluno.cd_pessoa_escola == cd_escola &&
                                    !(from t in db.Titulo
                                      where at.cd_contrato == t.cd_origem_titulo &&
                                              t.id_origem_titulo == cd_origem &&
                                              at.Aluno.cd_pessoa_escola == cd_escola &&
                                              t.dt_vcto_titulo.Year == ano &&
                                              t.dt_vcto_titulo.Month == vl_mes
                                      select t).Any() &&

                                    db.HistoricoAluno.Where(h =>
                                       h.cd_contrato == at.cd_contrato &&
                                       h.dt_historico.Year == ano &&
                                       h.dt_historico.Month == vl_mes &&
                                       h.cd_aluno == at.cd_aluno).OrderByDescending(o => o.nm_sequencia).Any() &&

                                    (((at.Turma.id_turma_ppt == false && at.Turma.cd_turma_ppt == null && at.Turma.cd_turma == cd_turma)) ||
                                    ((at.Turma.id_turma_ppt == false && at.Turma.cd_turma_ppt == cd_turma && at.Turma != null) || (at.Turma.id_turma_ppt == false && at.Turma.cd_turma_ppt != null && at.Turma.cd_turma == cd_turma)))

                                select new
                                {
                                    nome_aluno = at.Aluno.AlunoPessoaFisica.no_pessoa,
                                    nome_turma = at.Turma.no_turma
                                }).ToList().Select(x => new AlunosSemTituloGeradoUI
                                {
                                    nome_aluno = x.nome_aluno,
                                    nome_turma = x.nome_turma
                                }).ToList();
                    }
                }
                

                return sql2;
                
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existTituloRetornoCnab(int cd_titulo)
        {
            try
            {
                //select 1 from t_titulo_retorno_cnab where cd_titulo = titulo.cd_titulo and id_tipo_retorno = 2
                bool sql = (from t in db.TituloRetornoCNAB
                            where t.cd_titulo == cd_titulo && t.id_tipo_retorno == 2 // 
                            select t.cd_titulo).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public string getResponsavelTitulo(int cd_titulo, int cd_pessoa_empresa) {
            try {
                var sql = (from t in db.Titulo
                          where t.cd_titulo == cd_titulo &&
                          t.cd_pessoa_empresa == cd_pessoa_empresa
                          select t.PessoaResponsavel.no_pessoa).FirstOrDefault();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getTitulosAbertoContrato(int cd_empresa, int cd_contrato)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                List<Titulo> sql = (from i in db.Titulo
                                    where
                                      i.cd_origem_titulo == cd_contrato &&
                                      i.id_origem_titulo == cd_origem &&
                                      i.cd_pessoa_empresa == cd_empresa &&
                                      !i.BaixaTitulo.Any()
                                    select new
                                    {
                                        cd_titulo = i.cd_titulo,
                                        cd_origem_titulo = i.cd_origem_titulo,
                                        cd_pessoa_empresa = i.cd_pessoa_empresa,
                                        cd_pessoa_titulo = i.cd_pessoa_titulo,
                                        cd_pessoa_responsavel = i.cd_pessoa_responsavel,
                                        nomeResponsavel = i.PessoaResponsavel.no_pessoa,
                                        nomeAluno = i.Pessoa.no_pessoa,
                                        tipoDoc = i.TipoFinanceiro.dc_tipo_financeiro,
                                        cd_tipo_financeiro = i.cd_tipo_financeiro,
                                        dt_emissao_titulo = i.dt_emissao_titulo,
                                        dt_vcto_titulo = i.dt_vcto_titulo,
                                        vl_saldo_titulo = i.vl_saldo_titulo,
                                        vl_titulo = i.vl_titulo,
                                        dc_tipo_titulo = i.dc_tipo_titulo,
                                        nm_parcela_titulo = i.nm_parcela_titulo,
                                        dc_num_documento_titulo = i.dc_num_documento_titulo,
                                        nm_titulo = i.nm_titulo,
                                        tituloEdit = false,
                                        id = i.cd_titulo,
                                        vl_liquidacao_titulo = i.vl_liquidacao_titulo,
                                        cd_local_movto = i.cd_local_movto,
                                        tituloBaixado = i.BaixaTitulo.Any(),
                                        nm_banco = i.LocalMovto.nm_banco,
                                        no_local_movto = i.LocalMovto.no_local_movto,
                                        nm_agencia = i.LocalMovto.nm_agencia,
                                        nm_conta_corrente = i.LocalMovto.nm_conta_corrente,
                                        nm_tipo_local = i.LocalMovto.nm_tipo_local,
                                        nm_digito_conta_corrente = i.LocalMovto.nm_digito_conta_corrente,
                                        id_origem_titulo = i.id_origem_titulo,
                                        id_natureza_titulo = i.id_natureza_titulo,
                                        pc_juros_titulo = i.pc_juros_titulo,
                                        pc_multa_titulo = i.pc_multa_titulo,
                                        id_status_cnab = i.id_status_cnab,
                                        id_carteira_registrada = i.LocalMovto.CarteiraCnab == null ? false : i.LocalMovto.CarteiraCnab.id_registrada
                                    }).ToList().Select(x => new Titulo
                                    {
                                        cd_titulo = x.cd_titulo,
                                        cd_origem_titulo = x.cd_origem_titulo,
                                        cd_pessoa_empresa = x.cd_pessoa_empresa,
                                        cd_pessoa_titulo = x.cd_pessoa_titulo,
                                        cd_pessoa_responsavel = x.cd_pessoa_responsavel,
                                        nomeResponsavel = x.nomeResponsavel,
                                        nomeAluno = x.nomeAluno,
                                        tipoDoc = x.tipoDoc,
                                        cd_tipo_financeiro = x.cd_tipo_financeiro,
                                        dt_emissao_titulo = x.dt_emissao_titulo,
                                        dt_vcto_titulo = x.dt_vcto_titulo,
                                        vl_saldo_titulo = x.vl_saldo_titulo,
                                        vl_titulo = x.vl_titulo,
                                        dc_tipo_titulo = x.dc_tipo_titulo,
                                        nm_parcela_titulo = x.nm_parcela_titulo,
                                        dc_num_documento_titulo = x.dc_num_documento_titulo,
                                        nm_titulo = x.nm_titulo,
                                        tituloEdit = x.tituloEdit,
                                        id = x.id,
                                        vl_liquidacao_titulo = x.vl_liquidacao_titulo,
                                        cd_local_movto = x.cd_local_movto,
                                        id_carteira_registrada_localMvto = x.id_carteira_registrada,
                                        id_status_cnab = x.id_status_cnab,
                                        LocalMovto = new LocalMovto
                                        {
                                            no_local_movto = x.no_local_movto,
                                            nm_agencia = x.nm_agencia,
                                            nm_conta_corrente = x.nm_conta_corrente,
                                            nm_tipo_local = x.nm_tipo_local,
                                            nm_digito_conta_corrente = x.nm_digito_conta_corrente
                                        },
                                        tituloBaixado = x.tituloBaixado,
                                        id_origem_titulo = x.id_origem_titulo,
                                        id_natureza_titulo = x.id_natureza_titulo,
                                        pc_juros_titulo = x.pc_juros_titulo,
                                        pc_multa_titulo = x.pc_multa_titulo
                                    }).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Decimal getSaldoTitulosMatricula(int cd_empresa, int cd_contrato)
        {
            SGFWebContext dbComp = new SGFWebContext();
            int[] statusCnabTitulo = new int[] { (int)Titulo.StatusCnabTitulo.INICIAL, (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA };
            int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
            var sql = (from t in db.Titulo
                       where t.cd_origem_titulo == cd_contrato && 
                             t.id_origem_titulo == cd_origem &&
                             statusCnabTitulo.Contains(t.id_status_cnab) &&
                             (t.dc_tipo_titulo != "TM" && t.dc_tipo_titulo != "TA") &&
                             !t.BaixaTitulo.Any() 
                       select t).Sum(x => x.vl_titulo);
            return sql;
        }
        public Decimal getValorBaixasOutras(int cd_titulo)
        {
            var sql = !(from b in db.BaixaTitulo
                       where b.cd_titulo == cd_titulo &&
                             (b.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA ||
                              b.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO ||
                              b.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO ||
                              b.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.DESCONTO_FOLHA_PAGAMENTO)
                       select b).Any() ? 0 : (from b in db.BaixaTitulo
                                              where b.cd_titulo == cd_titulo &&
                                                    (b.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA ||
                                                     b.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO ||
                                                     b.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO ||
                                                     b.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.DESCONTO_FOLHA_PAGAMENTO)
                                              select b).Sum(x => x.vl_baixa_saldo_titulo);
            return sql;
        }
        public IEnumerable<CarneUI> getCarnePorContrato(int cdContrato, int cdEscola, int parcIniCarne, int parcFimCarne)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = from i in db.Titulo
                           where i.cd_origem_titulo == cdContrato &&
                                 i.id_origem_titulo == cd_origem &&
                                 i.cd_pessoa_empresa == cdEscola
                                 //i.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CARNE
                           select i;               

                //Realiza a contagem total dos titulos.
                var qtdTaxa = 0;
                var qtdMatricula = 0;
                var qtdAditamento = 0;
                var qtdMaterial = 0;
                qtdTaxa = (from s in sql
                           where s.dc_tipo_titulo == "TM" || s.dc_tipo_titulo == "TA" || s.dc_tipo_titulo == "TX"
                           select s).Count();
                qtdMatricula = (from s in sql
                                where s.dc_tipo_titulo == "MM" || s.dc_tipo_titulo == "MA" || s.dc_tipo_titulo == "ME"
                                select s).Count();
                qtdAditamento = (from s in sql
                                 where s.dc_tipo_titulo == "AD" || s.dc_tipo_titulo == "AA"
                                 select s).Count();
                qtdMaterial = (from s in sql
                                 where s.dc_tipo_titulo == "MT" 
                                 select s).Count();

                if (qtdMatricula == null) qtdMatricula = 0;
                if (qtdAditamento == null) qtdAditamento = 0;
                if (qtdMaterial == null) qtdMaterial = 0;

                //Após realizar a contagem total dos titulos, filtra apenas os carnês;
                sql = from i in sql where i.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CARNE select i;

                var sql1 = (from i in sql
                            select new
                                       {
                                           cd_origem_titulo = i.cd_origem_titulo,
                                           cd_pessoa_empresa = i.cd_pessoa_empresa,
                                           no_pessoa_empresa = i.Empresa.no_pessoa,
                                           cd_titulo = i.cd_titulo,
                                           sg_tipo_logradouro = i.Empresa.EnderecoPrincipal.TipoLogradouro.sg_tipo_logradouro,
                                           no_bairro = i.Empresa.EnderecoPrincipal.Bairro.no_localidade,
                                           no_cidade = i.Empresa.EnderecoPrincipal.Cidade.no_localidade,
                                           dc_num_cep = i.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep,
                                           sg_estado = i.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                                           no_localidade = i.Empresa.EnderecoPrincipal.Logradouro.no_localidade,
                                           dc_num_endereco = i.Empresa.EnderecoPrincipal.dc_num_endereco,

                                           dc_num_cgc = i.Empresa.dc_num_cgc,
                                           
                                           dc_mail_escola = i.Empresa.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                                           dc_fone_escola = i.Empresa.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).Any() ? i.Empresa.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).FirstOrDefault().dc_fone_mail : i.Empresa.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail,
                                           no_pessoa = i.Pessoa.no_pessoa,
                                           nm_titulo = i.nm_titulo,
                                           nm_parcela_titulo = i.nm_parcela_titulo,
                                           dt_vcto_titulo = i.dt_vcto_titulo,
                                           vl_saldo_titulo = i.vl_saldo_titulo,
                                           nm_matricula_contrato = db.Contrato.Where(c => c.cd_contrato == cdContrato).FirstOrDefault().nm_matricula_contrato,
                                           pc_juros_titulo = i.pc_juros_titulo,
                                           pc_multa_titulo = i.pc_multa_titulo,
                                           pc_juros_dia = i.Empresa.EscolaParametro.pc_juros_dia,
                                           pc_multa = i.Empresa.EscolaParametro.pc_multa,
                                           no_curso = db.Contrato.Where(c => c.cd_contrato == cdContrato).FirstOrDefault().Curso.no_curso,
                                           is_multiple_contract = db.Contrato.Where(c => c.cd_contrato == cdContrato && c.id_tipo_contrato == (byte)EnumTipoCKMatricula.MULTIPLA).Any(),
                                           no_cursos_contrato = (from cc in db.CursoContrato
                                                                 from co in db.Curso
                                                                 from ct in db.Contrato
                                                                 where cc.cd_curso == co.cd_curso &&
                                                                       cc.cd_contrato == cdContrato &&
                                                                       cc.cd_contrato == ct.cd_contrato &&
                                                                       ct.id_tipo_contrato == (byte)EnumTipoCKMatricula.MULTIPLA
                                                                 select co.no_curso
                                               ),
                                            nm_max_titulo = i.dc_tipo_titulo == "TM" || i.dc_tipo_titulo == "TA" || i.dc_tipo_titulo == "TX" ? qtdTaxa :
                                                            i.dc_tipo_titulo == "MM" || i.dc_tipo_titulo == "MA" || i.dc_tipo_titulo == "ME" ? (qtdAditamento + qtdMatricula) :
                                                            i.dc_tipo_titulo == "AD" || i.dc_tipo_titulo == "AA" ? (qtdAditamento + qtdMatricula) :
                                                            i.dc_tipo_titulo == "MT" ? qtdMaterial : 0,
                                           dc_tipo_titulo = i.dc_tipo_titulo == "TM" || i.dc_tipo_titulo == "TA" || i.dc_tipo_titulo == "TX" ? "Taxa de Matrícula" :
                                                            i.dc_tipo_titulo == "MM" || i.dc_tipo_titulo == "MA" || i.dc_tipo_titulo == "ME" ? "Mensalidade" :
                                                            i.dc_tipo_titulo == "AD" || i.dc_tipo_titulo == "AA" ? "Aditamento" :
                                                            i.dc_tipo_titulo == "MT" ? "Material" : ""
                                                                
                                       }).ToList().Select(x => new CarneUI
                                    {
                                        endereco = new EnderecoSGF()
                                        {
                                            TipoLogradouro = new TipoLogradouroSGF()
                                            {
                                                sg_tipo_logradouro = x.sg_tipo_logradouro
                                            },
                                            Bairro = new LocalidadeSGF()
                                            {
                                                no_localidade = x.no_bairro
                                            },
                                            Cidade = new LocalidadeSGF()
                                            {
                                                no_localidade = x.no_cidade
                                            },
                                            Logradouro = new LocalidadeSGF()
                                            {
                                                dc_num_cep = x.dc_num_cep,
                                                no_localidade = x.no_localidade
                                            },
                                            Estado = new LocalidadeSGF()
                                            {
                                                Estado = new EstadoSGF()
                                                {
                                                    sg_estado = x.sg_estado
                                                }
                                            },
                                            dc_num_endereco = x.dc_num_endereco
                                        },
                                        dc_num_cgc = x.dc_num_cgc,
                                        cd_titulo = x.cd_titulo,
                                        cd_origem_titulo = x.cd_origem_titulo.Value,
                                        cd_pessoa_empresa = x.cd_pessoa_empresa,
                                        no_pessoa_empresa = x.no_pessoa_empresa,
                                        dc_mail_escola = x.dc_mail_escola,
                                        dc_fone_escola = x.dc_fone_escola,
                                        no_pessoa = x.no_pessoa,
                                        nm_titulo = x.nm_titulo.Value,
                                        nm_parcela_titulo = x.nm_parcela_titulo.Value,
                                        dt_vcto_titulo = x.dt_vcto_titulo,
                                        dt_venc_corrigido = x.dt_vcto_titulo,
                                        vl_titulo = x.vl_saldo_titulo,
                                        nm_matricula_contrato = x.nm_matricula_contrato,
                                        pc_juros_titulo = x.pc_juros_titulo,
                                        pc_multa_titulo = x.pc_multa_titulo,
                                        pc_juros_dia = x.pc_juros_dia,
                                        pc_multa = x.pc_multa,
                                        no_curso = x.is_multiple_contract ? String.Join("/ ", x.no_cursos_contrato.ToArray()).ToString() : x.no_curso,
                                        nm_max_titulo = x.nm_max_titulo,
                                        dc_tipo_titulo = x.dc_tipo_titulo
                                    });

                if (parcIniCarne > 0)
                {
                    sql1 = (from s in sql1
                           where s.nm_parcela_titulo >= parcIniCarne
                           select s);
                }

                if (parcFimCarne > 0)
                {
                    sql1 = (from s in sql1
                           where s.nm_parcela_titulo <= parcFimCarne
                           select s);
                }

                return sql1.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> searchTituloFaturamento(SearchParameters parametros, int cd_pessoa_empresa, int cd_pessoa, bool responsavel, bool inicio, int? numeroTitulo,
                                               int? parcelaTitulo, string valorTitulo, DateTime? dtInicial, DateTime? dtFinal, bool contaSegura, byte tipoTitulo)
        {
            try
            {
                IEntitySorter<Titulo> sorter = EntitySorter<Titulo>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Titulo> sql;
                SGFWebContext dbContext = new SGFWebContext();
                int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                int cd_origemTit = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Titulo"].ToString());
                int cd_origemBaixa = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["BaixaTitulo"].ToString());
                sql = from t in db.Titulo.AsNoTracking()
                      where t.cd_pessoa_empresa == cd_pessoa_empresa &&
                            t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER &&
                            t.id_origem_titulo == cd_origem &&
                            !db.Movimento.Where(m => m.cd_pessoa_empresa == t.cd_pessoa_empresa && m.cd_origem_movimento == t.cd_titulo && m.id_origem_movimento == cd_origemTit).Any() &&
                            !t.BaixaTitulo.Where(b => db.Movimento.Where(mv => mv.cd_pessoa_empresa == t.cd_pessoa_empresa && mv.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO && mv.cd_origem_movimento == b.cd_baixa_titulo && mv.id_origem_movimento == cd_origemBaixa).Any()).Any()
                            
                      select t;
                if (tipoTitulo > 0)
                {
                    string filtro = Titulo.convertTipoTitulo(tipoTitulo);
                    sql = from t in sql
                          where t.dc_tipo_titulo == filtro
                          select t;
                }
                if (cd_pessoa > 0)
                    if (responsavel)
                        sql = from t in sql
                              where t.cd_pessoa_responsavel == cd_pessoa
                              select t;
                    else
                        sql = from t in sql
                              where t.cd_pessoa_titulo == cd_pessoa
                              select t;


                if (numeroTitulo != null && numeroTitulo > 0)
                {
                    if(inicio) {
                        string numero = numeroTitulo.ToString();
                        sql = from t in sql
                              where System.Data.Entity.SqlServer.SqlFunctions.StringConvert((decimal)t.nm_titulo).TrimStart().StartsWith(numero)
                              select t;
                    }
                    else
                        sql = from t in sql
                              where t.nm_titulo == numeroTitulo
                              select t;
                }
                if (parcelaTitulo != null && parcelaTitulo > 0)
                {
                        sql = from t in sql
                              where t.nm_parcela_titulo == parcelaTitulo
                              select t;
                }

                if (!string.IsNullOrEmpty(valorTitulo)){
                    if(inicio) {
                        valorTitulo = valorTitulo.Replace(".","").Replace(",","");
                        sql = from t in sql
                              where System.Data.Entity.SqlServer.SqlFunctions.StringConvert(t.vl_titulo * 100).TrimStart().StartsWith(valorTitulo)
                              select t;
                    }
                    else {
                        decimal vl_titulo = decimal.Parse(valorTitulo);
                        sql = from t in sql
                              where t.vl_titulo == vl_titulo
                              select t;
                    }
                }

                if (dtInicial.HasValue)
                    sql = from t in sql
                          where t.dt_vcto_titulo >= dtInicial
                          select t;

                if (dtFinal.HasValue)
                    sql = from t in sql
                          where t.dt_vcto_titulo <= dtFinal
                          select t;
                
                //Usuário não possui permissão de conta segura
                if (!contaSegura)
                    sql = from t in sql
                          where t.PlanoTitulo.Where(pt => pt.PlanoConta.id_conta_segura == false).Any()
                          select t;

                sql = sorter.Sort(sql);
                int limite = sql.Select(x=>x.cd_titulo).Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                var retorno = (from t in sql
                               select new
                               {
                                   cd_titulo = t.cd_titulo,
                                   id_natureza_titulo = t.id_natureza_titulo,
                                   nomeResponsavel = t.PessoaResponsavel.no_pessoa,
                                   nm_parcela_titulo = t.nm_parcela_titulo,
                                   nm_titulo = t.nm_titulo,
                                   dt_emissao_titulo = t.dt_emissao_titulo,
                                   dt_vcto_titulo = t.dt_vcto_titulo,
                                   vl_titulo = t.vl_titulo,
                                   vl_saldo_titulo = t.vl_saldo_titulo,
                                   cd_pessoa_empresa = t.cd_pessoa_empresa,
                                   id_origem_titulo = t.id_origem_titulo,
                                   cd_origem_titulo = t.cd_origem_titulo,
                                   id_status_titulo = t.id_status_titulo,
                                   possuiBaixa = t.BaixaTitulo.Any(),
                                   pc_juros_titulo = t.pc_juros_titulo,
                                   pc_multa_titulo = t.pc_multa_titulo,
                                   id_status_cnab = t.id_status_cnab,
                                   id_local_carteira_registrada = t.LocalMovto.CarteiraCnab == null ? false : t.LocalMovto.CarteiraCnab.id_registrada,
                                   dc_tipo_titulo = t.dc_tipo_titulo
                               }).ToList().Select(x => new Titulo
                               {
                                   cd_titulo = x.cd_titulo,
                                   nomeResponsavel = x.nomeResponsavel,
                                   dt_emissao_titulo = x.dt_emissao_titulo,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   vl_saldo_titulo = x.vl_saldo_titulo,
                                   vl_titulo = x.vl_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   nm_titulo = x.nm_titulo,
                                   id_natureza_titulo = x.id_natureza_titulo,
                                   cd_pessoa_empresa = x.cd_pessoa_empresa,
                                   id_origem_titulo = x.id_origem_titulo,
                                   cd_origem_titulo = x.cd_origem_titulo,
                                   id_status_titulo = x.id_status_titulo,
                                   possuiBaixa = x.possuiBaixa,
                                   pc_juros_titulo = x.pc_juros_titulo,
                                   pc_multa_titulo = x.pc_multa_titulo,
                                   id_status_cnab = x.id_status_cnab,
                                   id_carteira_registrada_localMvto = x.id_local_carteira_registrada,
                                   dc_tipo_titulo = x.dc_tipo_titulo
                               });
                //int limite = retorno.Count();
                //parametros.ajustaParametrosPesquisa(limite);
                //retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                //parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Titulo> getTitulosAbertosImpressaoAdt(int cdContrato, int cdEscola)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                List<Titulo> sql = (from i in db.Titulo
                           where i.cd_origem_titulo == cdContrato &&
                                 i.id_origem_titulo == cd_origem &&
                                 i.cd_pessoa_empresa == cdEscola &&
                                 i.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO
                           select new {
                               dt_vcto_titulo = i.dt_vcto_titulo
                           }).ToList().Select(x => new Titulo{
                               dt_vcto_titulo = x.dt_vcto_titulo
                           }).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Titulo> getTitulosAbertosAdicionarParcImpressaoAdt(int cdContrato, int cdEscola, int qtd_titulos_adt)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                List<Titulo> sql = (from i in db.Titulo
                                    where i.cd_origem_titulo == cdContrato &&
                                          i.id_origem_titulo == cd_origem &&
                                          i.cd_pessoa_empresa == cdEscola &&
                                          i.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                          (i.dc_tipo_titulo == "AD" || i.dc_tipo_titulo == "AA")
                                    orderby i.nm_titulo descending
                                    select new
                                    {
                                        dt_vcto_titulo = i.dt_vcto_titulo
                                    }).Take(qtd_titulos_adt).ToList().Select(x => new Titulo
                                    {
                                        dt_vcto_titulo = x.dt_vcto_titulo
                                    }).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<CarneUI> getCarnePorMovimentos(int cdMovimento, int cdEscola)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Movimento"].ToString());
                var sql = from i in db.Titulo
                          where i.cd_origem_titulo == cdMovimento &&
                                i.id_origem_titulo == cd_origem &&
                                i.cd_pessoa_empresa == cdEscola &&
                                i.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CARNE
                          select i;

                List<String> itens = (from im in db.ItemMovimento
                                                   where im.cd_movimento == cdMovimento &&
                                                         im.Movimento.cd_pessoa_empresa == cdEscola
                                                   select im.dc_item_movimento).ToList();
                string nomeItens = "";
                foreach (string nome in itens)
                {
                    if (nomeItens != "")
                        nomeItens = nomeItens + " | " + nome;
                    else
                        nomeItens = nome;
                }
               
                var sql1 = (from i in sql
                            select new
                            {
                                cd_origem_titulo = i.cd_origem_titulo,
                                cd_pessoa_empresa = i.cd_pessoa_empresa,
                                no_pessoa_empresa = i.Empresa.no_pessoa,
                                cd_titulo = i.cd_titulo,
                                sg_tipo_logradouro = i.Empresa.EnderecoPrincipal.TipoLogradouro.sg_tipo_logradouro,
                                no_bairro = i.Empresa.EnderecoPrincipal.Bairro.no_localidade,
                                no_cidade = i.Empresa.EnderecoPrincipal.Cidade.no_localidade,
                                dc_num_cep = i.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep,
                                sg_estado = i.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                                no_localidade = i.Empresa.EnderecoPrincipal.Logradouro.no_localidade,
                                dc_num_endereco = i.Empresa.EnderecoPrincipal.dc_num_endereco,

                                dc_mail_escola = i.Empresa.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                                dc_fone_escola = i.Empresa.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).Any() ? i.Empresa.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).FirstOrDefault().dc_fone_mail : i.Empresa.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail,
                                no_pessoa = i.Pessoa.no_pessoa,
                                nm_titulo = i.nm_titulo,
                                nm_parcela_titulo = i.nm_parcela_titulo,
                                dt_vcto_titulo = i.dt_vcto_titulo,
                                vl_titulo = i.vl_titulo,
                                pc_juros_titulo = i.pc_juros_titulo,
                                pc_multa_titulo = i.pc_multa_titulo,
                                pc_juros_dia = i.Empresa.EscolaParametro.pc_juros_dia,
                                pc_multa = i.Empresa.EscolaParametro.pc_multa,
                                nm_max_titulo = sql.Count(),
                                itens = nomeItens

                            }).ToList().Select(x => new CarneUI
                            {
                                endereco = new EnderecoSGF()
                                {
                                    TipoLogradouro = new TipoLogradouroSGF()
                                    {
                                        sg_tipo_logradouro = x.sg_tipo_logradouro
                                    },
                                    Bairro = new LocalidadeSGF()
                                    {
                                        no_localidade = x.no_bairro
                                    },
                                    Cidade = new LocalidadeSGF()
                                    {
                                        no_localidade = x.no_cidade
                                    },
                                    Logradouro = new LocalidadeSGF()
                                    {
                                        dc_num_cep = x.dc_num_cep,
                                        no_localidade = x.no_localidade
                                    },
                                    Estado = new LocalidadeSGF()
                                    {
                                        Estado = new EstadoSGF()
                                        {
                                            sg_estado = x.sg_estado
                                        }
                                    },
                                    dc_num_endereco = x.dc_num_endereco
                                },

                                cd_titulo = x.cd_titulo,
                                cd_origem_titulo = x.cd_origem_titulo.Value,
                                cd_pessoa_empresa = x.cd_pessoa_empresa,
                                no_pessoa_empresa = x.no_pessoa_empresa,
                                dc_mail_escola = x.dc_mail_escola,
                                dc_fone_escola = x.dc_fone_escola,
                                no_pessoa = x.no_pessoa,
                                nm_titulo = x.nm_titulo.Value,
                                nm_parcela_titulo = x.nm_parcela_titulo.Value,
                                dt_vcto_titulo = x.dt_vcto_titulo,
                                dt_venc_corrigido = x.dt_vcto_titulo,
                                vl_titulo = x.vl_titulo,
                                pc_juros_titulo = x.pc_juros_titulo,
                                pc_multa_titulo = x.pc_multa_titulo,
                                pc_juros_dia = x.pc_juros_dia,
                                pc_multa = x.pc_multa,
                                nm_max_titulo = x.nm_max_titulo,
                                itens = x.itens
                            }).ToList();

                return sql1;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ChequeUI> getRptChequesAbertos(int cd_empresa, int cd_pessoa_aluno, int cd_banco, string emitente, bool liquidados, string nm_cheque, string vl_titulo, int nm_agencia,
            int nm_ccorrente, DateTime? dt_ini_bPara, DateTime? dt_fim_bPara, DateTime? dt_ini, DateTime? dt_fim, bool emissao, bool liquidacao, byte natureza)
        {
            try{
                SGFWebContext dbContext = new SGFWebContext();
                int cd_origem_contrato = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                int cd_origem_movimento = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Movimento"].ToString());
                int cd_origem_baixa_titulo = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["BaixaTitulo"].ToString());
                var sql = from t in db.Titulo
                      where t.cd_pessoa_empresa == cd_empresa && t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE
                      select t;
                if (cd_pessoa_aluno > 0)
                    sql = from t in sql
                          where t.cd_pessoa_responsavel == cd_pessoa_aluno
                          select t;
                if (cd_banco > 0)
                    sql = from t in sql
                          where (t.id_origem_titulo == cd_origem_contrato && db.Cheque.Any(x => x.cd_contrato == t.cd_origem_titulo && x.cd_banco == cd_banco)) ||
                                (t.id_origem_titulo == cd_origem_movimento && db.Cheque.Any(x => x.cd_movimento == t.cd_origem_titulo && x.cd_banco == cd_banco))
                          select t;
                if(!string.IsNullOrEmpty(emitente))
                    sql = from t in sql
                          where (t.id_origem_titulo == cd_origem_contrato && db.Cheque.Any(x => x.cd_contrato == t.cd_origem_titulo && x.no_emitente_cheque == emitente)) ||
                                (t.id_origem_titulo == cd_origem_movimento && db.Cheque.Any(x => x.cd_movimento == t.cd_origem_titulo && x.no_emitente_cheque == emitente))
                          select t;

                if (dt_ini_bPara.HasValue)
                    sql = from t in sql
                          where t.dt_vcto_titulo >= dt_ini_bPara
                          select t;

                if (dt_fim_bPara.HasValue)
                    sql = from t in sql
                          where t.dt_vcto_titulo <= dt_fim_bPara
                          select t;

                if (liquidados)
                {
                    sql = from t in sql
                          where t.BaixaTitulo.Any()
                          select t;
                    if (liquidacao)
                    {
                        if (dt_ini.HasValue)
                            sql = from t in sql
                                  where t.BaixaTitulo.Where(x=> x.dt_baixa_titulo >= dt_ini).Any()
                                  select t;

                        if (dt_fim.HasValue)
                            sql = from t in sql
                                  where t.BaixaTitulo.Where(x => x.dt_baixa_titulo <= dt_fim).Any()
                                  select t;
                    }
                }
                if (emissao)
                {
                    if (dt_ini.HasValue)
                        sql = from t in sql
                              where t.dt_emissao_titulo >= dt_ini
                              select t;

                    if (dt_fim.HasValue)
                        sql = from t in sql
                              where t.dt_emissao_titulo <= dt_fim
                              select t;
                }
                if (!string.IsNullOrEmpty(nm_cheque))
                    sql = from t in sql
                          where t.dc_num_documento_titulo == nm_cheque
                          select t;
                if (nm_agencia > 0)
                    sql = from t in sql
                          where (t.id_origem_titulo == cd_origem_contrato && db.Cheque.Any(x => x.cd_contrato == t.cd_origem_titulo && x.nm_agencia_cheque == nm_agencia)) ||
                                (t.id_origem_titulo == cd_origem_movimento && db.Cheque.Any(x => x.cd_movimento == t.cd_origem_titulo && x.nm_agencia_cheque == nm_agencia))
                          select t;
                if (nm_ccorrente > 0)
                    sql = from t in sql
                          where (t.id_origem_titulo == cd_origem_contrato && db.Cheque.Any(x => x.cd_contrato == t.cd_origem_titulo && x.nm_conta_corrente_cheque == nm_ccorrente)) ||
                                (t.id_origem_titulo == cd_origem_movimento && db.Cheque.Any(x => x.cd_movimento == t.cd_origem_titulo && x.nm_conta_corrente_cheque == nm_ccorrente))
                          select t;
                if (!string.IsNullOrEmpty(vl_titulo) && decimal.Parse(vl_titulo) > 0)
                {
                    decimal valor = decimal.Parse(vl_titulo);
                    sql = from t in sql
                          where t.vl_titulo == valor
                          select t;
                }
                if (natureza > 0)
                    sql = from t in sql
                          where t.id_natureza_titulo == natureza
                          select t;

                var retorno = (from t in sql
                               select new
                               {
                                   t.cd_pessoa_titulo,
                                   t.Pessoa.no_pessoa,
                                   nm_cpf_cgc = t.Pessoa.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                           db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                           db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                                   Cheque = t.id_origem_titulo == cd_origem_contrato ? db.Cheque.Where(x => x.cd_contrato == t.cd_origem_titulo).FirstOrDefault() :
                                                                                    db.Cheque.Where(x => x.cd_movimento == t.cd_origem_titulo).FirstOrDefault(),
                                   no_banco = t.id_origem_titulo == cd_origem_contrato ? db.Cheque.Where(x => x.cd_contrato == t.cd_origem_titulo).FirstOrDefault().Banco.no_banco :
                                                                                   db.Cheque.Where(x => x.cd_movimento == t.cd_origem_titulo).FirstOrDefault().Banco.no_banco,
                                   t.dt_vcto_titulo,
                                   t.vl_titulo,
                                   t.dt_emissao_titulo,
                                   nm_nota = t.id_origem_titulo == cd_origem_contrato ? db.Movimento.Where(x => x.cd_pessoa_empresa == cd_empresa && 
                                                                                                                ((x.id_origem_movimento == cd_origem_baixa_titulo && t.BaixaTitulo.Any(b=> b.cd_titulo == t.cd_titulo && b.cd_baixa_titulo == x.cd_origem_movimento)) ||
                                                                                                                 (x.id_origem_movimento == cd_origem_contrato && x.cd_origem_movimento == t.cd_origem_titulo) )
                                                                                                           ).FirstOrDefault().nm_movimento :
                                                                                        db.Movimento.Where(x => x.cd_pessoa_empresa == cd_empresa && x.id_origem_movimento == null && x.cd_origem_movimento == null &&
                                                                                                           x.cd_movimento == t.cd_origem_titulo && t.id_origem_titulo == cd_origem_movimento).FirstOrDefault().nm_movimento,
                                   nm_serie = t.id_origem_titulo == cd_origem_contrato ? db.Movimento.Where(x => x.cd_pessoa_empresa == cd_empresa &&
                                                                                                                 ((x.id_origem_movimento == cd_origem_baixa_titulo && t.BaixaTitulo.Any(b => b.cd_titulo == t.cd_titulo && b.cd_baixa_titulo == x.cd_origem_movimento)) ||
                                                                                                                 (x.id_origem_movimento == cd_origem_contrato && x.cd_origem_movimento == t.cd_origem_titulo))
                                                                                                                 ).FirstOrDefault().dc_serie_movimento :
                                                                                         db.Movimento.Where(x => x.cd_pessoa_empresa == cd_empresa && x.id_origem_movimento == null && x.cd_origem_movimento == null &&
                                                                                                            x.cd_movimento == t.cd_origem_titulo && t.id_origem_titulo == cd_origem_movimento).FirstOrDefault().dc_serie_movimento,
                                   nm_recibo = t.BaixaTitulo.Any() ? t.BaixaTitulo.FirstOrDefault().nm_recibo : 0,
                                   dt_baixa_titulo = t.BaixaTitulo.Any() ? t.BaixaTitulo.FirstOrDefault().dt_baixa_titulo : DateTime.MinValue,
                                   t.dc_num_documento_titulo

                               }).ToList().Select(x => new ChequeUI
                               {
                                   cd_pessoa_titulo = x.cd_pessoa_titulo,
                                   no_pessoa_titulo = x.no_pessoa,
                                   nm_cpf_cgc = x.nm_cpf_cgc,
                                   no_banco = x.no_banco,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   vl_titulo = x.vl_titulo,
                                   dt_emissao_titulo = x.dt_emissao_titulo,
                                   nm_nota = x.nm_nota,
                                   nm_serie = x.nm_serie,
                                   nm_recibo = x.nm_recibo > 0 ? x.nm_recibo +  "" : "",
                                   nm_agencia = x.Cheque != null ? x.Cheque.nm_agencia_cheque: 0,
                                   nm_conta_corrente = x.Cheque != null ? x.Cheque.nm_conta_corrente_cheque : 0,
                                   no_emitente = x.Cheque != null ? x.Cheque.no_emitente_cheque : "",
                                   nm_cheque = x.dc_num_documento_titulo,
                                   dt_baixa_titulo = x.dt_baixa_titulo
                               });

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ChequeUI> getRptChequesLiquidados(int cd_empresa, int cd_pessoa_aluno, int cd_banco, string emitente, bool liquidados, string nm_cheque, string vl_titulo, int nm_agencia,
    int nm_ccorrente, DateTime? dt_ini_bPara, DateTime? dt_fim_bPara, DateTime? dt_ini, DateTime? dt_fim, bool emissao, bool liquidacao, byte natureza)
        {
            try
            {
                SGFWebContext dbContext = new SGFWebContext();
                int cd_origem_contrato = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                int cd_origem_movimento = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Movimento"].ToString());
                int cd_origem_baixa_titulo = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["BaixaTitulo"].ToString());
                var sqlChequestrans = from t in db.Titulo
                                      join b in db.BaixaTitulo on t.cd_titulo equals b.cd_titulo
                                      join tf in db.TransacaoFinanceira on b.cd_tran_finan equals tf.cd_tran_finan
                                      join ctf in db.ChequeTransacao on tf.cd_tran_finan equals ctf.cd_tran_finan
                                      join c in db.Cheque on ctf.cd_cheque equals c.cd_cheque
                                      where t.cd_pessoa_empresa == cd_empresa &&
                                           (cd_banco == 0 || c.cd_banco == cd_banco) &&
                                           (string.IsNullOrEmpty(emitente) || c.no_emitente_cheque.Contains(emitente)) &&
                                           (string.IsNullOrEmpty(nm_cheque) || ctf.nm_cheque == nm_cheque) &&
                                           (nm_agencia == 0 || c.nm_agencia_cheque == nm_agencia) &&
                                           (nm_ccorrente == 0 || c.nm_conta_corrente_cheque == nm_ccorrente) &&
                                           //(!dt_fim_bPara.HasValue || ctf.dt_bom_para <= dt_fim_bPara) &&
                                           (!dt_ini.HasValue || b.dt_baixa_titulo >= dt_ini) &&
                                           (!dt_fim.HasValue || b.dt_baixa_titulo <= dt_fim)
                                      select t;
                var sqlChequesBaixas = from t in db.Titulo
                                       join b in db.BaixaTitulo on t.cd_titulo equals b.cd_titulo
                                       join cb in db.ChequeBaixa on b.cd_baixa_titulo equals cb.cd_baixa_titulo
                                       join c in db.Cheque on cb.cd_cheque equals c.cd_cheque
                                       where t.cd_pessoa_empresa == cd_empresa &&
                                             (cd_banco == 0 || c.cd_banco == cd_banco) &&
                                           (string.IsNullOrEmpty(emitente) || c.no_emitente_cheque.Contains(emitente)) &&
                                           (string.IsNullOrEmpty(nm_cheque) || cb.nm_cheque == nm_cheque) &&
                                           (nm_agencia == 0 || c.nm_agencia_cheque == nm_agencia) &&
                                           (nm_ccorrente == 0 || c.nm_conta_corrente_cheque == nm_ccorrente) &&
                                           (!dt_fim_bPara.HasValue || cb.dt_bom_para <= dt_fim_bPara) &&
                                           (!dt_ini.HasValue || b.dt_baixa_titulo >= dt_ini) &&
                                           (!dt_fim.HasValue || b.dt_baixa_titulo <= dt_fim)
                                       select t;

                if (cd_pessoa_aluno > 0)
                {
                    sqlChequestrans = from t in sqlChequestrans
                          where t.cd_pessoa_responsavel == cd_pessoa_aluno
                          select t;
                    sqlChequesBaixas = from t in sqlChequesBaixas
                                      where t.cd_pessoa_responsavel == cd_pessoa_aluno
                                      select t;
                }

                if (dt_ini_bPara.HasValue)
                {
                    sqlChequestrans = sqlChequestrans.Where(x =>
                        x.BaixaTitulo.Any(s => db.ChequeTransacao.Any(t => t.cd_tran_finan == s.cd_tran_finan &&
                                                                           t.dt_bom_para >= dt_ini_bPara)));

                    sqlChequesBaixas = sqlChequesBaixas.Where(x =>
                        x.BaixaTitulo.Any(s => db.ChequeTransacao.Any(t => t.cd_tran_finan == s.cd_tran_finan &&
                                                                           t.dt_bom_para >= dt_ini_bPara)));
                }

                if (dt_fim_bPara.HasValue)
                {
                    sqlChequestrans = sqlChequestrans.Where(x =>
                        x.BaixaTitulo.Any(s => db.ChequeTransacao.Any(t => t.cd_tran_finan == s.cd_tran_finan &&
                                                                           t.dt_bom_para <= dt_fim_bPara)));

                    sqlChequesBaixas = sqlChequesBaixas.Where(x =>
                        x.BaixaTitulo.Any(s => db.ChequeTransacao.Any(t => t.cd_tran_finan == s.cd_tran_finan &&
                                                                           t.dt_bom_para <= dt_fim_bPara)));
                }

                if (emissao)
                {
                    if (dt_ini.HasValue)
                    {
                        sqlChequestrans = from t in sqlChequestrans
                                          where t.dt_emissao_titulo >= dt_ini
                                          select t;
                        sqlChequesBaixas = from t in sqlChequesBaixas
                                           where t.dt_emissao_titulo >= dt_ini
                                           select t;
                    }
                    if (dt_fim.HasValue)
                    {
                        sqlChequestrans = from t in sqlChequestrans
                                          where t.dt_emissao_titulo <= dt_fim
                                          select t;
                        sqlChequesBaixas = from t in sqlChequesBaixas
                                           where t.dt_emissao_titulo >= dt_ini
                                           select t;
                    }
                }

                if (!string.IsNullOrEmpty(vl_titulo) && decimal.Parse(vl_titulo) > 0)
                {
                    decimal valor = decimal.Parse(vl_titulo);
                    sqlChequestrans = from t in sqlChequestrans
                          where t.vl_titulo == valor
                          select t;
                    sqlChequesBaixas = from t in sqlChequesBaixas
                                      where t.vl_titulo == valor
                                      select t;
                }
                if (natureza > 0)
                {
                    sqlChequestrans = from t in sqlChequestrans
                                      where t.id_natureza_titulo == natureza
                                      select t;
                    sqlChequesBaixas = from t in sqlChequesBaixas
                                       where t.id_natureza_titulo == natureza
                                       select t;
                }

                var retorno = (from t in sqlChequestrans
                               join b in db.BaixaTitulo on t.cd_titulo equals b.cd_titulo
                               join tf in db.TransacaoFinanceira on b.cd_tran_finan equals tf.cd_tran_finan
                               join ctf in db.ChequeTransacao on tf.cd_tran_finan equals ctf.cd_tran_finan
                               join c in db.Cheque on ctf.cd_cheque equals c.cd_cheque
                               select new
                               {
                                   t.cd_pessoa_titulo,
                                   t.Pessoa.no_pessoa,
                                   nm_cpf_cgc = t.Pessoa.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                           db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                           db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                                   Cheque = c,
                                   no_banco = c.Banco.no_banco,
                                   dt_vcto_titulo =ctf.dt_bom_para,
                                   t.vl_titulo,
                                   t.dt_emissao_titulo,
                                   nm_nota = t.id_origem_titulo == cd_origem_contrato ? 
                                               db.Movimento.Where(x => x.cd_pessoa_empresa == cd_empresa &&
                                              ((x.id_origem_movimento == cd_origem_baixa_titulo && 
                                                b.cd_baixa_titulo == x.cd_origem_movimento) ||
                                                (x.id_origem_movimento == cd_origem_contrato && x.cd_origem_movimento == t.cd_origem_titulo)
                                              )).FirstOrDefault().nm_movimento :
                                              db.Movimento.Where(x => x.cd_pessoa_empresa == cd_empresa && x.id_origem_movimento == null && x.cd_origem_movimento == null &&
                                                                 x.cd_movimento == t.cd_origem_titulo && t.id_origem_titulo == cd_origem_movimento).FirstOrDefault().nm_movimento,
                                   nm_serie = t.id_origem_titulo == cd_origem_contrato ? db.Movimento.Where(x => x.cd_pessoa_empresa == cd_empresa &&
                                                                                         ((x.id_origem_movimento == cd_origem_baixa_titulo && b.cd_baixa_titulo == x.cd_origem_movimento) || 
                                                                                          (x.id_origem_movimento == cd_origem_contrato && x.cd_origem_movimento == t.cd_origem_titulo)
                                                                                         )).FirstOrDefault().dc_serie_movimento :
                                                                                         db.Movimento.Where(x => x.cd_pessoa_empresa == cd_empresa && x.id_origem_movimento == null && 
                                                                                                            x.cd_origem_movimento == null && x.cd_movimento == t.cd_origem_titulo && 
                                                                                                            t.id_origem_titulo == cd_origem_movimento).FirstOrDefault().dc_serie_movimento,
                                   nm_recibo = b.nm_recibo,
                                   dt_baixa_titulo = b.dt_baixa_titulo,
                                   dc_num_documento_titulo = ctf.nm_cheque

                               }).Union(from t in sqlChequesBaixas
                                        join b in db.BaixaTitulo on t.cd_titulo equals b.cd_titulo
                                        join cb in db.ChequeBaixa on b.cd_baixa_titulo equals cb.cd_baixa_titulo
                                        join c in db.Cheque on cb.cd_cheque equals c.cd_cheque
                               select new
                               {
                                   t.cd_pessoa_titulo,
                                   t.Pessoa.no_pessoa,
                                   nm_cpf_cgc = t.Pessoa.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                           db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                           db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                                   Cheque = c,
                                   no_banco = c.Banco.no_banco,
                                   dt_vcto_titulo = cb.dt_bom_para,
                                   t.vl_titulo,
                                   t.dt_emissao_titulo,
                                   nm_nota = t.id_origem_titulo == cd_origem_contrato ? 
                                               db.Movimento.Where(x => x.cd_pessoa_empresa == cd_empresa &&
                                              ((x.id_origem_movimento == cd_origem_baixa_titulo && 
                                                b.cd_baixa_titulo == x.cd_origem_movimento) ||
                                                (x.id_origem_movimento == cd_origem_contrato && x.cd_origem_movimento == t.cd_origem_titulo)
                                              )).FirstOrDefault().nm_movimento :
                                              db.Movimento.Where(x => x.cd_pessoa_empresa == cd_empresa && x.id_origem_movimento == null && x.cd_origem_movimento == null &&
                                                                 x.cd_movimento == t.cd_origem_titulo && t.id_origem_titulo == cd_origem_movimento).FirstOrDefault().nm_movimento,
                                   nm_serie = t.id_origem_titulo == cd_origem_contrato ? db.Movimento.Where(x => x.cd_pessoa_empresa == cd_empresa &&
                                                                                         ((x.id_origem_movimento == cd_origem_baixa_titulo && b.cd_baixa_titulo == x.cd_origem_movimento) || 
                                                                                          (x.id_origem_movimento == cd_origem_contrato && x.cd_origem_movimento == t.cd_origem_titulo)
                                                                                         )).FirstOrDefault().dc_serie_movimento :
                                                                                         db.Movimento.Where(x => x.cd_pessoa_empresa == cd_empresa && x.id_origem_movimento == null && 
                                                                                                            x.cd_origem_movimento == null && x.cd_movimento == t.cd_origem_titulo && 
                                                                                                            t.id_origem_titulo == cd_origem_movimento).FirstOrDefault().dc_serie_movimento,
                                   nm_recibo = b.nm_recibo,
                                   dt_baixa_titulo = b.dt_baixa_titulo,
                                   dc_num_documento_titulo = cb.nm_cheque

                               }) .ToList().Select(x => new ChequeUI
                               {
                                   cd_pessoa_titulo = x.cd_pessoa_titulo,
                                   no_pessoa_titulo = x.no_pessoa,
                                   nm_cpf_cgc = x.nm_cpf_cgc,
                                   no_banco = x.no_banco,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   vl_titulo = x.vl_titulo,
                                   dt_emissao_titulo = x.dt_emissao_titulo,
                                   nm_nota = x.nm_nota,
                                   nm_serie = x.nm_serie,
                                   nm_recibo = x.nm_recibo > 0 ? x.nm_recibo + "" : "",
                                   nm_agencia = x.Cheque != null ? x.Cheque.nm_agencia_cheque : 0,
                                   nm_conta_corrente = x.Cheque != null ? x.Cheque.nm_conta_corrente_cheque : 0,
                                   no_emitente = x.Cheque != null ? x.Cheque.no_emitente_cheque : "",
                                   nm_cheque = x.dc_num_documento_titulo,
                                   dt_baixa_titulo = x.dt_baixa_titulo
                               });

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public bool verificaTituloContratoAjusteManual(List<int> titulos, int cd_escola, bool aditivo)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                bool retorno = false;
                if (!aditivo)
                    retorno = (from c in db.Contrato
                               where c.cd_pessoa_escola == cd_escola && (!c.id_ajuste_manual || (c.id_ajuste_manual && c.pc_desconto_bolsa <= 0)) &&
                                     db.Titulo.Any(t => titulos.Contains(t.cd_titulo) &&
                                                        t.cd_pessoa_empresa == cd_escola &&
                                                        t.id_origem_titulo == cd_origem &&
                                                        t.cd_origem_titulo == c.cd_contrato)
                               select c.id_ajuste_manual).Any();
                else
                retorno = (from c in db.Contrato
                           where c.cd_pessoa_escola == cd_escola && 
                                 (!c.id_ajuste_manual || (c.id_ajuste_manual && 
                                                          !c.Aditamento.Any(ad => ad.id_tipo_aditamento != null && ad.id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADITIVO_BOLSA))) &&
                                 db.Titulo.Any(t => titulos.Contains(t.cd_titulo) &&
                                                    t.cd_pessoa_empresa == cd_escola &&
                                                    t.id_origem_titulo == cd_origem &&
                                                    t.cd_origem_titulo == c.cd_contrato)
                           select c.id_ajuste_manual).Any();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaTituloVencido(int cdPessoa, DateTime dataHoje, int cd_escola)
        {
            try{
            bool sql = (from c in db.Titulo
                       where c.cd_pessoa_empresa == cd_escola && 
                       c.vl_saldo_titulo > 0 &&
                       c.dt_vcto_titulo < dataHoje &&
                       (c.cd_pessoa_titulo == cdPessoa || c.cd_pessoa_responsavel == cdPessoa)
                       select c).Any();
            return sql;
                }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getEditTitulosReajusteAnual(int cd_escola, int cd_reajuste_anual)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = from t in db.Titulo
                             where t.id_origem_titulo == cd_origem && t.ReajustesTitulos.Any(r => r.cd_reajuste_anual == cd_reajuste_anual)
                             select t;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getTitulosReajusteAnual(int cd_escola, int cd_reajuste_anual, DateTime dt_ini_venc, DateTime? dt_fim_venc)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                IEnumerable<Titulo> retorno;
                bool existsFiltroInformado = db.ReajusteAnual.Any(r=> r.cd_reajuste_anual == cd_reajuste_anual &&
                                                                      (r.ReajustesCursos.Any() || r.ReajustesTurmas.Any() || r.ReajustesAlunos.Any()));
                if (existsFiltroInformado)
                {
                    var sqlCurso = (from t in db.Titulo.AsNoTracking()
                                    join c in db.Contrato on t.cd_origem_titulo equals c.cd_contrato
                                    where t.id_origem_titulo == cd_origem && t.cd_pessoa_empresa == cd_escola &&
                                          db.AlunoTurma.Any(at => at.cd_aluno == c.cd_aluno && at.cd_contrato == c.cd_contrato) &&
                                          (t.id_status_cnab == (int)Titulo.StatusCnabTitulo.INICIAL || t.id_status_cnab == (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA) &&
                                          t.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO && !t.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                                                           x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO) &&
                                          t.dt_vcto_titulo >= dt_ini_venc.Date && (dt_fim_venc.HasValue && t.dt_vcto_titulo <= dt_fim_venc.Value) &&
                                          db.ReajusteCurso.Any(cur => cur.cd_reajuste_anual == cd_reajuste_anual && cur.cd_curso == c.cd_curso_atual)
                                    select new
                                    {
                                        cd_titulo = t.cd_titulo,
                                        cd_pessoa_titulo = t.cd_pessoa_titulo,
                                        cd_origem_titulo = t.cd_origem_titulo,
                                        cd_curso = c.cd_curso_atual
                                    }).ToList().Select(x => new Titulo
                               {
                                   cd_titulo = x.cd_titulo,
                                   cd_pessoa_titulo = x.cd_pessoa_titulo,
                                   cd_origem_titulo = x.cd_origem_titulo,
                                   cd_curso = x.cd_curso
                               });

                    var sqlTurma = (from t in db.Titulo.AsNoTracking()
                                    join c in db.Contrato on t.cd_origem_titulo equals c.cd_contrato
                                    join h in db.HistoricoAluno on c.cd_aluno equals h.cd_aluno
                                    join tm in db.Turma on h.cd_turma equals tm.cd_turma
                                    where t.id_origem_titulo == cd_origem && t.cd_pessoa_empresa == cd_escola &&
                                          h.Aluno.cd_pessoa_aluno == t.cd_pessoa_titulo && h.cd_produto == c.cd_produto_atual &&
                                          db.AlunoTurma.Any(at => at.cd_aluno == c.cd_aluno && at.cd_contrato == c.cd_contrato) &&
                                          db.ReajusteTurma.Any(rt => rt.cd_reajuste_anual == cd_reajuste_anual && (rt.cd_turma == h.cd_turma || tm.cd_turma_ppt == rt.cd_turma)) &&
                                          (t.id_status_cnab == (int)Titulo.StatusCnabTitulo.INICIAL || t.id_status_cnab == (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA) &&
                                          t.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO && !t.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                                                           x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO) &&
                                          t.dt_vcto_titulo >= dt_ini_venc.Date && (dt_fim_venc.HasValue && t.dt_vcto_titulo <= dt_fim_venc.Value) &&
                                          h.dt_historico <= t.dt_vcto_titulo && h.nm_sequencia == db.HistoricoAluno.Where(h1 => h1.cd_aluno == c.cd_aluno && h1.cd_produto == c.cd_produto_atual && h.dt_historico <= t.dt_vcto_titulo).Max(x => x.nm_sequencia)
                                    select new
                                    {
                                        cd_titulo = t.cd_titulo,
                                        cd_pessoa_titulo = t.cd_pessoa_titulo,
                                        cd_origem_titulo = t.cd_origem_titulo,
                                        h.cd_turma
                                    }).ToList().Select(x => new Titulo
                                 {
                                     cd_titulo = x.cd_titulo,
                                     cd_pessoa_titulo = x.cd_pessoa_titulo,
                                     cd_origem_titulo = x.cd_origem_titulo,
                                     cd_turma_titulo = x.cd_turma
                                 });

                    var sqlAluno = (from t in db.Titulo.AsNoTracking()
                                    join c in db.Contrato on t.cd_origem_titulo equals c.cd_contrato
                                    where t.id_origem_titulo == cd_origem && t.cd_pessoa_empresa == cd_escola &&
                                          db.AlunoTurma.Any(at => at.cd_aluno == c.cd_aluno && at.cd_contrato == c.cd_contrato) &&
                                          (t.id_status_cnab == (int)Titulo.StatusCnabTitulo.INICIAL || t.id_status_cnab == (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA) &&
                                          t.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO && !t.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                                                      x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO) &&
                                          t.dt_vcto_titulo >= dt_ini_venc.Date && (dt_fim_venc.HasValue && t.dt_vcto_titulo <= dt_fim_venc.Value) &&
                                          db.ReajusteAluno.Any(ra => ra.cd_reajuste_anual == cd_reajuste_anual && ra.cd_aluno == c.cd_aluno)
                                    select new
                                    {
                                        cd_titulo = t.cd_titulo,
                                        cd_pessoa_titulo = t.cd_pessoa_titulo,
                                        cd_origem_titulo = t.cd_origem_titulo,
                                        c.cd_aluno
                                    }).ToList().Select(x => new Titulo
                                 {
                                     cd_titulo = x.cd_titulo,
                                     cd_pessoa_titulo = x.cd_pessoa_titulo,
                                     cd_origem_titulo = x.cd_origem_titulo,
                                     cd_aluno = x.cd_aluno
                                 });
                    retorno = sqlCurso.Union(sqlTurma).Union(sqlAluno).ToList();
                }
                else
                {
                    retorno = (from t in db.Titulo.AsNoTracking()
                                    join c in db.Contrato on t.cd_origem_titulo equals c.cd_contrato
                                    where t.id_origem_titulo == cd_origem && t.cd_pessoa_empresa == cd_escola &&
                                          //db.AlunoTurma.Any(at => at.cd_aluno == c.cd_aluno && at.cd_contrato == c.cd_contrato) &&
                                          (t.id_status_cnab == (int)Titulo.StatusCnabTitulo.INICIAL || t.id_status_cnab == (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA) &&
                                          t.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO && !t.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                                                           x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO) &&
                                          t.dt_vcto_titulo >= dt_ini_venc.Date && (dt_fim_venc.HasValue && t.dt_vcto_titulo <= dt_fim_venc.Value)
                                    select new
                                    {
                                        cd_titulo = t.cd_titulo,
                                        cd_pessoa_titulo = t.cd_pessoa_titulo,
                                        cd_origem_titulo = t.cd_origem_titulo
                                    }).ToList().Select(x => new Titulo
                                    {
                                        cd_titulo = x.cd_titulo,
                                        cd_pessoa_titulo = x.cd_pessoa_titulo,
                                        cd_origem_titulo = x.cd_origem_titulo
                                    });
                }
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public decimal getSaldoContratoParaReajusteAnual(int cd_escola, decimal pc_bolsa, int cd_contrato, List<int> cdsTitulos)
        {
            try
            {
                SGFWebContext dbContext = new SGFWebContext();
                int[] statusCnabTitulo = new int[] { (int)Titulo.StatusCnabTitulo.INICIAL, (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA };
                int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                decimal saldo = 0;
                var sql = from t in db.Titulo
                          where t.cd_origem_titulo == cd_contrato &&
                              t.id_origem_titulo == cd_origem &&
                              !cdsTitulos.Contains(t.cd_titulo) &&
                              statusCnabTitulo.Contains(t.id_status_cnab) &&
                              (t.dc_tipo_titulo != "TM" && t.dc_tipo_titulo != "TA") &&
                              t.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                              !t.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA && x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                          select t;
                if (sql.Any())
                    saldo = sql.Sum(x => x.dc_tipo_titulo == "ME" ? x.vl_saldo_titulo + ((pc_bolsa / 100) * x.vl_titulo) - x.vl_material_titulo : (x.vl_saldo_titulo - x.vl_material_titulo));
                return saldo;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getTitulosReajusteAnual(int cd_escola, int cd_reajuste_anual)
        {

            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = from t in db.Titulo.Include(x=> x.PlanoTitulo)
                          where t.cd_pessoa_empresa == cd_escola && 
                                t.id_origem_titulo == cd_origem &&
                                t.ReajustesTitulos.Any(x=> x.cd_reajuste_anual == cd_reajuste_anual)
                           select t;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<int> getAlunosQuePossuemTitulosAbertoMes(List<int> cdPessoaAlunos, int cd_empresa, DateTime dt_diario)
        {
            try
            {
                DateTime dt_ini = new DateTime(dt_diario.Year, dt_diario.Month, 1);
                DateTime dt_fim = dt_ini.AddMonths(1);
                dt_fim = dt_fim.AddDays(-1);
                var sql = from t in db.Titulo
                          where t.cd_pessoa_empresa == cd_empresa && cdPessoaAlunos.Contains(t.cd_pessoa_titulo) &&
                          t.dt_vcto_titulo >= dt_ini && t.dt_vcto_titulo <= dt_fim &&
                          (!t.BaixaTitulo.Any() || t.BaixaTitulo.Any(x=> x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO))
                          select t.cd_pessoa_titulo;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getTitulosForBaixaAutomatica(SearchParameters parametros, TituloUI titulo)
        {
            try
            {
                //var ORIGEMMATRICULA = 22;
                //var ORIGEMMOMIMENTO = 69;
                IEntitySorter<Titulo> sorter = EntitySorter<Titulo>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Titulo> sql;
                 sql = from t in db.Titulo.Include("LocalMovto").AsNoTracking()
                          select t;

               // var localMovtoSelecionado = db.LocalMovto.Where(t => t.cd_local_movto == titulo.cd_local_movto_cartao || t.cd_local_movto == titulo.cd_local_movto_banco).FirstOrDefault();
                
                if (titulo.dtInicial != null)
                    sql = from t in sql
                          where t.dt_vcto_titulo >= titulo.dtInicial
                          select t;
                
                if (titulo.dtFinal != null)
                    sql = from t in sql
                          where t.dt_vcto_titulo <= titulo.dtFinal
                          select t;


                if (titulo.cd_local_movto_cartao > 0 && titulo.cd_local_movto_banco > 0)
                {
                    sql = from t in sql
                        join l in db.LocalMovto on t.cd_local_movto equals l.cd_local_movto 
                        where l.cd_local_banco == titulo.cd_local_movto_cartao &&
                              t.cd_local_movto == titulo.cd_local_movto_banco
                        select t;
                }
                else if (titulo.cd_local_movto_cartao > 0 && titulo.cd_local_movto_banco == 0)
                {
                    sql = from t in sql
                            join l in db.LocalMovto on t.cd_local_movto equals l.cd_local_movto
                          where (l.cd_local_banco == titulo.cd_local_movto_cartao ||
                                l.cd_local_movto == titulo.cd_local_movto_cartao)
                            //&& !l.cd_local_banco.HasValue 
                        select t;
                }

               //if (titulo.cd_local_movto_cartao == 0)
                //{
                //    sql = from t in sql
                //          where db.LocalMovto.Any(loc => loc.cd_local_movto == t.cd_local_movto)
                //          select t;
                //} else if (titulo.cd_local_movto_cartao > 0 && (localMovtoSelecionado != null && localMovtoSelecionado.nm_tipo_local ==
                //    (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.BANCO))
                //{
                //    if (titulo.cd_local_movto_banco > 0)
                //    {
                //        sql = from t in sql
                //              where t.cd_local_movto == titulo.cd_local_movto_banco
                //              select t;
                //    }
                //    else
                //    {
                //        sql = from t in sql
                //              where t.LocalMovto.cd_local_banco == titulo.cd_local_movto_cartao
                //              select t;
                //    }
                //}
                //else if (titulo.cd_local_movto_cartao > 0)
                //{
                //    sql = from t in sql
                //          where t.cd_local_movto == titulo.cd_local_movto_cartao
                //          select t;
                //}


                sql = (from t in sql
                             where t.cd_pessoa_empresa == titulo.cd_pessoa_empresa &&
                                   t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CARTAO &&
                                   t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER &&
                                   t.vl_titulo == t.vl_saldo_titulo //&& Pode ser de qualquer origem a receber com cartão
                                   //t.id_origem_titulo == ORIGEMMATRICULA
                             select new
                             {
                                 cd_titulo = t.cd_titulo,
                                 no_local_movimento = t.LocalMovto.no_local_movto,
                                 id_natureza_titulo = t.id_natureza_titulo,
                                 nomeResponsavel = t.PessoaResponsavel.no_pessoa,
                                 no_pessoa = t.Pessoa.no_pessoa,
                                 nm_parcela_titulo = t.nm_parcela_titulo,
                                 nm_titulo = t.nm_titulo,
                                 dt_emissao_titulo = t.dt_emissao_titulo,
                                 dt_vcto_titulo = t.dt_vcto_titulo,
                                 vl_titulo = t.vl_titulo,
                                 nm_dias_cartao = t.nm_dias_cartao,
                                 pc_taxa_cartao = t.pc_taxa_cartao,
                                 vl_taxa = ((double)t.vl_titulo * t.pc_taxa_cartao) / 100,
                                 vl_saldo_titulo = t.vl_saldo_titulo,
                                 cd_pessoa_empresa = t.cd_pessoa_empresa,
                                 id_origem_titulo = t.id_origem_titulo,
                                 cd_origem_titulo = t.cd_origem_titulo,
                                 id_status_titulo = t.id_status_titulo,
                                 possuiBaixa = t.BaixaTitulo.Any(),
                                 pc_juros_titulo = t.pc_juros_titulo,
                                 pc_multa_titulo = t.pc_multa_titulo,
                                 id_status_cnab = t.id_status_cnab,
                                 id_local_carteira_registrada = t.LocalMovto != null && t.LocalMovto.CarteiraCnab != null ? t.LocalMovto.CarteiraCnab.id_registrada : false,
                                 dc_tipo_titulo = t.dc_tipo_titulo,
                                 t.TipoFinanceiro.dc_tipo_financeiro,
                                 t.vl_material_titulo,
                                 t.cd_tipo_financeiro,
                                 t.dc_num_documento_titulo,
                                 cd_local_movto = t.LocalMovto.cd_local_movto,
                                 nm_tipo_local = t.LocalMovto.nm_tipo_local,
                                 cd_local_banco = t.LocalMovto.cd_local_banco
                             }).Distinct().ToList().Select(x => new Titulo
                             {
                                 cd_titulo = x.cd_titulo,
                                 no_local_movimento = x.no_local_movimento,
                                 nomeResponsavel = x.nomeResponsavel,
                                 no_pessoa = x.no_pessoa,
                                 dt_emissao_titulo = x.dt_emissao_titulo,
                                 dt_vcto_titulo = x.dt_vcto_titulo,
                                 vl_saldo_titulo = x.vl_saldo_titulo,
                                 vl_titulo = x.vl_titulo,
                                 nm_parcela_titulo = x.nm_parcela_titulo,
                                 nm_titulo = x.nm_titulo,
                                 nm_dias_cartao = x.nm_dias_cartao,
                                 pc_taxa_cartao = x.pc_taxa_cartao,
                                 vl_taxa = x.vl_taxa,
                                 id_natureza_titulo = x.id_natureza_titulo,
                                 cd_pessoa_empresa = x.cd_pessoa_empresa,
                                 id_origem_titulo = x.id_origem_titulo,
                                 cd_origem_titulo = x.cd_origem_titulo,
                                 id_status_titulo = x.id_status_titulo,
                                 possuiBaixa = x.possuiBaixa,
                                 pc_juros_titulo = x.pc_juros_titulo,
                                 pc_multa_titulo = x.pc_multa_titulo,
                                 id_status_cnab = x.id_status_cnab,
                                 id_carteira_registrada_localMvto = x.id_local_carteira_registrada,
                                 dc_tipo_titulo = x.dc_tipo_titulo,
                                 vl_material_titulo = x.vl_material_titulo,
                                 tipoDoc = x.dc_tipo_financeiro,
                                 cd_tipo_financeiro = x.cd_tipo_financeiro,
                                 dc_num_documento_titulo = x.dc_num_documento_titulo,
                                 cd_local_movto = x.cd_local_movto,
                                 nm_tipo_local = x.nm_tipo_local,
                                 cd_local_banco = x.cd_local_banco
                             }).AsQueryable().Distinct();

                sql = sorter.Sort(sql);
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> getTitulosForBaixaAutomaticaCheque(TituloChequeUI titulo)
        {
            try
            {   SGFWebContext dbComp = new SGFWebContext();
            var ORIGEMMATRICULA = 22;
            var ORIGEMMOMIMENTO = 69;
            var ORIGEMCHEQUE = 129;
            var sql = (from t in db.Titulo.Include("LocalMovto").AsNoTracking()
                       join c in db.Cheque on t.cd_origem_titulo equals c.cd_contrato into leftj
                       from lfcheque in leftj.DefaultIfEmpty() //left join
                       where t.cd_pessoa_empresa == titulo.cd_pessoa_empresa &&
                             t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE &&
                             t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER &&
                             t.vl_titulo == t.vl_saldo_titulo &&
                             t.id_origem_titulo == ORIGEMMATRICULA
                       //&& !t.BaixaTitulo.Any()
                       select new
                       {
                           dc_num_documento_titulo = t.dc_num_documento_titulo,
                           no_pessoa = t.Pessoa.no_pessoa,
                           no_emissor = lfcheque.no_emitente_cheque,
                           cd_local_movto = t.cd_local_movto,
                           cd_titulo = t.cd_titulo,
                           id_natureza_titulo = t.id_natureza_titulo,
                           nm_parcela_titulo = t.nm_parcela_titulo,
                           nm_titulo = t.nm_titulo,
                           dt_emissao_titulo = t.dt_emissao_titulo,
                           dt_vcto_titulo = t.dt_vcto_titulo,
                           vl_titulo = t.vl_titulo,
                           vl_saldo_titulo = t.vl_saldo_titulo,
                           cd_pessoa_empresa = t.cd_pessoa_empresa,
                           id_origem_titulo = t.id_origem_titulo,
                           cd_origem_titulo = t.cd_origem_titulo,
                           id_status_titulo = t.id_status_titulo,
                           possuiBaixa = t.BaixaTitulo.Any(),
                           pc_juros_titulo = t.pc_juros_titulo,
                           pc_multa_titulo = t.pc_multa_titulo,
                           id_status_cnab = t.id_status_cnab,
                           //id_local_carteira_registrada = t.LocalMovto.CarteiraCnab.id_registrada == null ? false : t.LocalMovto.CarteiraCnab.id_registrada,
                           dc_tipo_titulo = t.dc_tipo_titulo
                       }).Distinct().ToList().Select(x => new Titulo
                       {
                           dc_num_documento_titulo = x.dc_num_documento_titulo,
                           cd_titulo = x.cd_titulo,
                           no_pessoa = x.no_pessoa,
                           no_emissor = x.no_emissor,
                           cd_local_movto = x.cd_local_movto,
                           dt_emissao_titulo = x.dt_emissao_titulo,
                           dt_vcto_titulo = x.dt_vcto_titulo,
                           vl_saldo_titulo = x.vl_saldo_titulo,
                           vl_titulo = x.vl_titulo,
                           nm_parcela_titulo = x.nm_parcela_titulo,
                           nm_titulo = x.nm_titulo,
                           id_natureza_titulo = x.id_natureza_titulo,
                           cd_pessoa_empresa = x.cd_pessoa_empresa,
                           id_origem_titulo = x.id_origem_titulo,
                           cd_origem_titulo = x.cd_origem_titulo,
                           id_status_titulo = x.id_status_titulo,
                           possuiBaixa = x.possuiBaixa,
                           pc_juros_titulo = x.pc_juros_titulo,
                           pc_multa_titulo = x.pc_multa_titulo,
                           id_status_cnab = x.id_status_cnab,
                           //id_carteira_registrada_localMvto = x.id_local_carteira_registrada,
                           dc_tipo_titulo = x.dc_tipo_titulo
                       }).AsQueryable()
                .Union(
                    (from t in db.Titulo.Include("LocalMovto").AsNoTracking()
                     join c in db.Cheque on t.cd_origem_titulo equals c.cd_movimento
                     where t.cd_pessoa_empresa == titulo.cd_pessoa_empresa &&
                           t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE &&
                           t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER &&
                           t.vl_titulo == t.vl_saldo_titulo &&
                           t.id_origem_titulo == ORIGEMMOMIMENTO
                     //&& !t.BaixaTitulo.Any() &&
                     select new
                     {
                         dc_num_documento_titulo = t.dc_num_documento_titulo,
                         no_pessoa = t.Pessoa.no_pessoa,
                         no_emissor = c.no_emitente_cheque,
                         cd_local_movto = t.cd_local_movto,
                         cd_titulo = t.cd_titulo,
                         id_natureza_titulo = t.id_natureza_titulo,
                         nm_parcela_titulo = t.nm_parcela_titulo,
                         nm_titulo = t.nm_titulo,
                         dt_emissao_titulo = t.dt_emissao_titulo,
                         dt_vcto_titulo = t.dt_vcto_titulo,
                         vl_titulo = t.vl_titulo,
                         vl_saldo_titulo = t.vl_saldo_titulo,
                         cd_pessoa_empresa = t.cd_pessoa_empresa,
                         id_origem_titulo = t.id_origem_titulo,
                         cd_origem_titulo = t.cd_origem_titulo,
                         dc_tipo_titulo = t.dc_tipo_titulo
                     }).Distinct().ToList().Select(x => new Titulo
                     {
                         dc_num_documento_titulo = x.dc_num_documento_titulo,
                         cd_titulo = x.cd_titulo,
                         no_pessoa = x.no_pessoa,
                         no_emissor = x.no_emissor,
                         cd_local_movto = x.cd_local_movto,
                         dt_emissao_titulo = x.dt_emissao_titulo,
                         dt_vcto_titulo = x.dt_vcto_titulo,
                         vl_saldo_titulo = x.vl_saldo_titulo,
                         vl_titulo = x.vl_titulo,
                         nm_parcela_titulo = x.nm_parcela_titulo,
                         nm_titulo = x.nm_titulo,
                         id_natureza_titulo = x.id_natureza_titulo,
                         cd_pessoa_empresa = x.cd_pessoa_empresa,
                         id_origem_titulo = x.id_origem_titulo,
                         cd_origem_titulo = x.cd_origem_titulo,
                         dc_tipo_titulo = x.dc_tipo_titulo
                     }).AsQueryable()).Union(
                    (from t in db.Titulo.Include("LocalMovto").AsNoTracking()
                     join c in db.ChequeTransacao on t.cd_origem_titulo equals c.cd_tran_finan
                     join ch in db.Cheque on c.cd_cheque equals ch.cd_cheque
                     where t.cd_pessoa_empresa == titulo.cd_pessoa_empresa &&
                           t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE &&
                           t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER &&
                           t.vl_titulo == t.vl_saldo_titulo &&
                           t.id_origem_titulo == ORIGEMCHEQUE
                     //&& !t.BaixaTitulo.Any() &&
                     select new
                     {
                         dc_num_documento_titulo = t.dc_num_documento_titulo,
                         no_pessoa = t.Pessoa.no_pessoa,
                         no_emissor = ch.no_emitente_cheque,
                         cd_local_movto = t.cd_local_movto,
                         cd_titulo = t.cd_titulo,
                         id_natureza_titulo = t.id_natureza_titulo,
                         nm_parcela_titulo = t.nm_parcela_titulo,
                         nm_titulo = t.nm_titulo,
                         dt_emissao_titulo = t.dt_emissao_titulo,
                         dt_vcto_titulo = t.dt_vcto_titulo,
                         vl_titulo = t.vl_titulo,
                         vl_saldo_titulo = t.vl_saldo_titulo,
                         cd_pessoa_empresa = t.cd_pessoa_empresa,
                         id_origem_titulo = t.id_origem_titulo,
                         cd_origem_titulo = t.cd_origem_titulo,
                         dc_tipo_titulo = t.dc_tipo_titulo
                     }).Distinct().ToList().Select(x => new Titulo
                     {
                         dc_num_documento_titulo = x.dc_num_documento_titulo,
                         cd_titulo = x.cd_titulo,
                         no_pessoa = x.no_pessoa,
                         no_emissor = x.no_emissor,
                         cd_local_movto = x.cd_local_movto,
                         dt_emissao_titulo = x.dt_emissao_titulo,
                         dt_vcto_titulo = x.dt_vcto_titulo,
                         vl_saldo_titulo = x.vl_saldo_titulo,
                         vl_titulo = x.vl_titulo,
                         nm_parcela_titulo = x.nm_parcela_titulo,
                         nm_titulo = x.nm_titulo,
                         id_natureza_titulo = x.id_natureza_titulo,
                         cd_pessoa_empresa = x.cd_pessoa_empresa,
                         id_origem_titulo = x.id_origem_titulo,
                         cd_origem_titulo = x.cd_origem_titulo,
                         dc_tipo_titulo = x.dc_tipo_titulo
                     }).AsQueryable()                     
                     )
                     .Distinct(); 
                        
                if (titulo.dtInicial != null)
                    sql = from t in sql
                          where t.dt_vcto_titulo >= titulo.dtInicial
                          select t;

                if (titulo.dtFinal != null)
                    sql = from t in sql
                          where t.dt_vcto_titulo <= titulo.dtFinal
                          select t;

                if (titulo.cd_local_movto_cheque > 0 && titulo.troca_local == false)
                {
                    sql = from t in sql
                          where t.cd_local_movto == titulo.cd_local_movto_cheque
                          select t;
                }
                
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        


        public int gerarBaixaAutomaticaProcedure(int cd_baixa_automatica, int cd_usuario)
        {
            try
            {
                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"sp_gerar_baixa_automatica";
                command.CommandTimeout = 180;

                var sqlParameters = new List<SqlParameter>();

                sqlParameters.Add(new SqlParameter("@cd_baixa_automatica", cd_baixa_automatica));

                sqlParameters.Add(new SqlParameter("@cd_usuario", cd_usuario));


                var parameter = new SqlParameter("@result", SqlDbType.Int);
                parameter.Direction = ParameterDirection.ReturnValue;
                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();
                var ret = (int)command.Parameters["@result"].Value;
                db.Database.Connection.Close();
                return ret;
            }
            catch (SqlException exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
            catch (Exception exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
        }
        public int excluirAditamentoProcedure(int cd_aditamento)
        {
            try
            {
                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"sp_excluir_aditamento";
                command.CommandTimeout = 180;

                var sqlParameters = new List<SqlParameter>();

                sqlParameters.Add(new SqlParameter("@cd_aditamento", cd_aditamento));

                var parameter = new SqlParameter("@result", SqlDbType.Int);
                parameter.Direction = ParameterDirection.ReturnValue;
                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();
                var ret = (int)command.Parameters["@result"].Value;
                db.Database.Connection.Close();
                return ret;
            }
            catch (SqlException exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
            catch (Exception exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
        }
        public List<Titulo> getTitulosByTituloAditamento(int cd_escola, int cd_aditamento)
        {
            List<Titulo> titulos = new List<Titulo>();
            try
            {

                titulos = (from t in db.Titulo
                    join ta in db.TituloAditamento on t.cd_titulo equals ta.cd_titulo
                    where t.cd_pessoa_empresa == cd_escola &&
                          ta.cd_aditamento == cd_aditamento
                    select t).ToList();
                return titulos;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteTitulosByTituloAditamento(int cd_escola, string cds_titulos)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete b from t_titulo t " +
                                              "inner join T_BAIXA_TITULO b on b.cd_titulo = t.cd_titulo " +
                                              "where b.cd_tipo_liquidacao in (100, 102) and " +
                                              "((select count(*) from T_BAIXA_TITULO bx where bx.cd_titulo = t.cd_titulo) = 1) and " +
                                              "t.cd_titulo in(" + cds_titulos + ") and t.cd_pessoa_empresa = " + cd_escola);

                int retorno = db.Database.ExecuteSqlCommand("delete t from t_titulo t where t.cd_titulo in(" + cds_titulos + ") and t.cd_pessoa_empresa = " + cd_escola);

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Titulo> getTitulosSemBaixa(int cd_escola, List<Titulo> titulos)
        {
            try
            {
               


                string strIdsTitulos = "";

                if (titulos != null && titulos.Count > 0)
                {
                    foreach (Titulo e in titulos)
                    {
                        strIdsTitulos += e.cd_titulo + ",";
                    }
                }



                if (strIdsTitulos.Length > 0)
                    strIdsTitulos = strIdsTitulos.Substring(0, strIdsTitulos.Length - 1);


                List<Titulo> titulosSemBaixa = db.Database.SqlQuery<Titulo>(@"select * " +
                                                       "from t_titulo t " +
                                                        "where " +
                                                            "t.cd_pessoa_empresa = " + cd_escola + " and " +
                                                            "t.vl_saldo_titulo > 0 and " +
                                                            "t.id_status_cnab = 0 and " +
                                                            "t.cd_titulo in(" + strIdsTitulos + ") and " +
                                                           "case " +
                                                                "when exists (select 1 from T_BAIXA_TITULO b " +
                                                                    "where b.cd_tipo_liquidacao in (100, 102) and " +
                                                                    "((select count(*) from T_BAIXA_TITULO bx " +
                                                                    "where bx.cd_titulo = t.cd_titulo) = 1)) then 1 " +
                                                                "when not exists (select 1 from T_BAIXA_TITULO b " +
                                                                    "where b.cd_titulo = t.cd_titulo) then 1 " +
                                                                 "else 0 " +
                                                           "End = 1 ").ToList();
               



                return titulosSemBaixa;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Titulo> getTitulosbyTranFinan(int cd_escola, int cd_tran_finan)
        {
            try
            {
                int TRANSACAO_FINANCEIRA = 129;
                var sql = (from t in db.Titulo
                    join tr in db.TransacaoFinanceira on t.cd_origem_titulo equals tr.cd_tran_finan
                    where t.cd_pessoa_empresa == cd_escola &&
                          t.id_origem_titulo == TRANSACAO_FINANCEIRA &&
                          t.cd_origem_titulo == cd_tran_finan &&
                          tr.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.TROCA_FINANCEIRA
                    select t).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<BaixaTitulo> getBaixasByCdTitulo(int cd_escola, int cd_titulo, int cd_tran_finan)
        {
            try
            {
                int TRANSACAO_FINANCEIRA = 129;
                List<BaixaTitulo> baixasTitulo = (from b in db.BaixaTitulo
                    join t in db.Titulo on b.cd_titulo equals t.cd_titulo
                           join tr in db.TransacaoFinanceira on t.cd_origem_titulo equals tr.cd_tran_finan
                    where t.cd_pessoa_empresa == cd_escola &&
                          t.id_origem_titulo == TRANSACAO_FINANCEIRA &&
                          t.cd_origem_titulo == cd_tran_finan &&
                          tr.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.TROCA_FINANCEIRA &&
                          t.cd_titulo == cd_titulo
                    select b).ToList();

                return baixasTitulo;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Cheque> getChequeTransacaoTrocaFinanceira(int cd_titulo, int cdEscola)
        {
            
            try
            {

                int TROCA_FINANCEIRA = 129;
                TransacaoFinanceira transacao = (from t in db.Titulo
                    join tr in db.TransacaoFinanceira on t.cd_origem_titulo equals tr.cd_tran_finan
                        where t.cd_pessoa_empresa == cdEscola &&
                          t.id_origem_titulo == TROCA_FINANCEIRA &&
                          tr.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.TROCA_FINANCEIRA &&
                          t.cd_titulo == cd_titulo
                    select tr).FirstOrDefault();

                if (transacao != null && transacao.cd_tran_finan > 0)
                {
                    IEnumerable<Cheque> chequesTransacaoTroca = (from ch in db.Cheque.Include("ChequeTransacaoFinanceira")
                                                                 join c in db.ChequeTransacao on ch.cd_cheque equals c.cd_cheque
                                                                 join tf in db.TransacaoFinanceira on c.cd_tran_finan equals tf.cd_tran_finan
                                                                 where tf.cd_tran_finan == transacao.cd_tran_finan
                                                                 select new 
                                                                 {
                                                                     cd_cheque = ch.cd_cheque,
                                                                     cd_contrato = ch.cd_contrato,
                                                                     no_emitente_cheque = ch.no_emitente_cheque,
                                                                     no_agencia_cheque = ch.no_agencia_cheque,
                                                                     nm_agencia_cheque = ch.nm_agencia_cheque,
                                                                     nm_digito_agencia_cheque = ch.nm_digito_agencia_cheque,
                                                                     nm_conta_corrente_cheque = ch.nm_conta_corrente_cheque,
                                                                     nm_digito_cc_cheque = ch.nm_digito_cc_cheque,
                                                                     nm_primeiro_cheque = ch.nm_primeiro_cheque,
                                                                     cd_banco = ch.cd_banco,
                                                                     cd_movimento = ch.cd_movimento,
                                                                     Banco = ch.Banco,
                                                                     dt_bom_para = c.dt_bom_para,
                                                                     cd_cheque_trans = c.cd_cheque_trans,
                                                                     cd_tran_finan = c.cd_tran_finan,
                                                                     nm_cheque = c.nm_cheque
                                                                 }).ToList().Select(x => new Cheque()
                                                                    {
                                                                        cd_cheque = x.cd_cheque,
                                                                        cd_contrato = x.cd_contrato,
                                                                        no_emitente_cheque = x.no_emitente_cheque,
                                                                        no_agencia_cheque = x.no_agencia_cheque,
                                                                        nm_agencia_cheque = x.nm_agencia_cheque,
                                                                        nm_digito_agencia_cheque = x.nm_digito_agencia_cheque,
                                                                        nm_conta_corrente_cheque = x.nm_conta_corrente_cheque,
                                                                        nm_digito_cc_cheque = x.nm_digito_cc_cheque,
                                                                        nm_primeiro_cheque = x.nm_primeiro_cheque,
                                                                        cd_banco = x.cd_banco,
                                                                        cd_movimento = x.cd_movimento,
                                                                        Banco = x.Banco,
                                                                        dt_bom_para = x.dt_bom_para,
                                                                        cd_tran_finan = x.cd_tran_finan,
                                                                        nm_cheque = x.nm_cheque
                                                                    }).AsEnumerable();
                    return chequesTransacaoTroca;
                }
                else
                {
                    return null;
                }

                
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public ReciboConfirmacaoUI getReciboConfirmacaoByContrato(int cd_contrato, int cd_empresa)
        {
            try
            {

                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                ReciboConfirmacaoUI sql = (from c in db.Contrato
                        where c.cd_contrato == cd_contrato &&
                              c.cd_pessoa_escola == cd_empresa
                           select new
                           {
                               sg_tipo_logradouro = c.Escola.EnderecoPrincipal.TipoLogradouro.sg_tipo_logradouro,
                               no_bairro = c.Escola.EnderecoPrincipal.Bairro.no_localidade,
                               no_cidade = c.Escola.EnderecoPrincipal.Cidade.no_localidade,
                               dc_num_cep = c.Escola.EnderecoPrincipal.Logradouro.dc_num_cep,
                               sg_estado = c.Escola.EnderecoPrincipal.Estado.Estado.sg_estado,
                               no_localidade = c.Escola.EnderecoPrincipal.Logradouro.no_localidade,
                               dc_num_endereco = c.Escola.EnderecoPrincipal.dc_num_endereco,

                               dc_telefone_escola = c.Escola.Telefone.dc_fone_mail,
                               dc_num_cgc = c.Escola.dc_num_cgc,
                               dc_num_insc_estadual = c.Escola.dc_num_insc_estadual,
                               dc_cidade_estado = c.Escola.EnderecoPrincipal.Cidade.no_localidade + " - " + c.Escola.EnderecoPrincipal.Estado.Estado.sg_estado,
                               //dt_baixa_titulo = bt.dt_baixa_titulo,
                               //tx_obs_baixa = bt.tx_obs_baixa,
                               cd_contrato = c.nm_contrato,
                               no_pessoa = db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.Aluno.cd_pessoa_aluno).FirstOrDefault<PessoaFisicaSGF>().no_pessoa,
                               no_pessoa_responsavel = db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().no_pessoa,
                               cpf_pessoa = (c.PessoaResponsavel.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                    db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                       db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf + "" :
                                          (db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf != null ?
                                          db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf : "") : ""),

                               titulos = (from i in db.Titulo //.Include(l => l.LocalMovto)
                                          where i.cd_origem_titulo == cd_contrato &&
                                             i.id_origem_titulo == cd_origem &&
                                             i.cd_pessoa_empresa == cd_empresa &&
                                             (!i.BaixaTitulo.Any()) &&
                                             (i.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CARTAO || i.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)
                                       select i).ToList(),
                               no_pessoa_usuario = c.SysUsuario != null && c.SysUsuario.PessoaFisica != null ? c.SysUsuario.PessoaFisica.no_pessoa : ""
                           }).ToList().Select(x => new ReciboConfirmacaoUI
                           {
                               endereco = new EnderecoSGF()
                               {
                                   TipoLogradouro = new TipoLogradouroSGF()
                                   {
                                       sg_tipo_logradouro = x.sg_tipo_logradouro
                                   },
                                   Bairro = new LocalidadeSGF()
                                   {
                                       no_localidade = x.no_bairro
                                   },
                                   Cidade = new LocalidadeSGF()
                                   {
                                       no_localidade = x.no_cidade
                                   },
                                   Logradouro = new LocalidadeSGF()
                                   {
                                       dc_num_cep = x.dc_num_cep,
                                       no_localidade = x.no_localidade
                                   },
                                   Estado = new LocalidadeSGF()
                                   {
                                       Estado = new EstadoSGF()
                                       {
                                           sg_estado = x.sg_estado
                                       }
                                   },
                                   dc_num_endereco = x.dc_num_endereco
                               },
                               titulos = x.titulos,
                               dc_telefone_escola = x.dc_telefone_escola,
                               dc_num_cgc = x.dc_num_cgc,
                               dc_num_insc_estadual = x.dc_num_insc_estadual,
                               dc_cidade_estado = x.dc_cidade_estado,
                               //dt_baixa_titulo = x.dt_baixa_titulo,
                               //txt_obs_baixa = x.tx_obs_baixa,
                               cd_contrato_movimento = (x.cd_contrato != null)? (int)x.cd_contrato : 0,
                               no_pessoa = x.no_pessoa,
                               no_pessoa_responsavel = x.no_pessoa_responsavel,
                               cpf_pessoa = x.cpf_pessoa,
                               //dc_tipo_liquidacao = x.dc_tipo_liquidacao,
                               //total_titulo = x.total_titulo,
                               dc_tipo_titulo_referente = "Mensalidade",
                               no_pessoa_usuario = x.no_pessoa_usuario,
                           }).FirstOrDefault();

                
                return sql;

               
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ReciboConfirmacaoUI getReciboConfirmacaoByMovimento(int cd_movimento, int cd_empresa)
        {
            try
            {

                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Movimento"].ToString());

                ReciboConfirmacaoUI sql = (from m in db.Movimento
                                           where m.cd_movimento == cd_movimento &&
                                                 m.cd_pessoa_empresa == cd_empresa
                                           select new
                                           {
                                               sg_tipo_logradouro = m.Empresa.EnderecoPrincipal.TipoLogradouro.sg_tipo_logradouro,
                                               no_bairro = m.Empresa.EnderecoPrincipal.Bairro.no_localidade,
                                               no_cidade = m.Empresa.EnderecoPrincipal.Cidade.no_localidade,
                                               dc_num_cep = m.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep,
                                               sg_estado = m.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                                               no_localidade = m.Empresa.EnderecoPrincipal.Logradouro.no_localidade,
                                               dc_num_endereco = m.Empresa.EnderecoPrincipal.dc_num_endereco,

                                               dc_telefone_escola = m.Empresa.Telefone.dc_fone_mail,
                                               dc_num_cgc = m.Empresa.dc_num_cgc,
                                               dc_num_insc_estadual = m.Empresa.dc_num_insc_estadual,
                                               dc_cidade_estado = m.Empresa.EnderecoPrincipal.Cidade.no_localidade + " - " + m.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                                               cd_contrato = m.nm_movimento,
                                               no_pessoa = db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == m.Aluno.cd_pessoa_aluno).FirstOrDefault<PessoaFisicaSGF>().no_pessoa,
                                               no_pessoa_responsavel = db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == m.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().no_pessoa,
                                               cpf_pessoa = (m.Pessoa.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                                    db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == m.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                                       db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == m.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf + "" :
                                                          (db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == m.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf != null ?
                                                          db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == m.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf : "") : ""),

                                               titulos = (from i in db.Titulo 
                                                          where i.cd_origem_titulo == cd_movimento &&
                                                             i.id_origem_titulo == cd_origem &&
                                                             i.cd_pessoa_empresa == cd_empresa &&
                                                             (!i.BaixaTitulo.Any()) &&
                                                             (i.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CARTAO || i.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)
                                                          select i).ToList()
                                           }).ToList().Select(x => new ReciboConfirmacaoUI
                                           {
                                               endereco = new EnderecoSGF()
                                               {
                                                   TipoLogradouro = new TipoLogradouroSGF()
                                                   {
                                                       sg_tipo_logradouro = x.sg_tipo_logradouro
                                                   },
                                                   Bairro = new LocalidadeSGF()
                                                   {
                                                       no_localidade = x.no_bairro
                                                   },
                                                   Cidade = new LocalidadeSGF()
                                                   {
                                                       no_localidade = x.no_cidade
                                                   },
                                                   Logradouro = new LocalidadeSGF()
                                                   {
                                                       dc_num_cep = x.dc_num_cep,
                                                       no_localidade = x.no_localidade
                                                   },
                                                   Estado = new LocalidadeSGF()
                                                   {
                                                       Estado = new EstadoSGF()
                                                       {
                                                           sg_estado = x.sg_estado
                                                       }
                                                   },
                                                   dc_num_endereco = x.dc_num_endereco
                                               },
                                               titulos = x.titulos,
                                               dc_telefone_escola = x.dc_telefone_escola,
                                               dc_num_cgc = x.dc_num_cgc,
                                               dc_num_insc_estadual = x.dc_num_insc_estadual,
                                               dc_cidade_estado = x.dc_cidade_estado,
                                               cd_contrato_movimento = (x.cd_contrato != null)? (int)x.cd_contrato : 0,
                                               no_pessoa = x.no_pessoa,
                                               no_pessoa_responsavel = x.no_pessoa_responsavel,
                                               cpf_pessoa = x.cpf_pessoa,
                                               dc_tipo_titulo_referente = "Notas",
                                           }).FirstOrDefault();


                return sql;


            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<ReciboConfirmacaoParcelasUI> getParcelasReciboConfirmacaoByContrato(int cd_contrato, int cd_empresa)
        {
            try
            {

                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                ParcelasReciboConfirmacaoResultUI sql = (from c in db.Contrato
                    where c.cd_contrato == cd_contrato &&
                          c.cd_pessoa_escola == cd_empresa
                    select new
                    {
                        titulos = (from i in db.Titulo //.Include(l => l.LocalMovto)
                            where i.cd_origem_titulo == cd_contrato &&
                                  i.id_origem_titulo == cd_origem &&
                                  i.cd_pessoa_empresa == cd_empresa &&
                                  (!i.BaixaTitulo.Any()) &&
                                  (i.cd_tipo_financeiro == (int) TipoFinanceiro.TiposFinanceiro.CARTAO || i.cd_tipo_financeiro == (int) TipoFinanceiro.TiposFinanceiro.CHEQUE)
                            select i).ToList(),
                    }).ToList().Select(x => new ParcelasReciboConfirmacaoResultUI()
                    {
                        titulos = x.titulos

                    }).FirstOrDefault();

                List<ReciboConfirmacaoParcelasUI> parcelasFormatadas = new List<ReciboConfirmacaoParcelasUI>();
                if (sql != null && sql.parcelas != null && sql.parcelas.Count > 0)
                {
                    foreach (var sqlParcela in sql.parcelas)
                    {
                        ReciboConfirmacaoParcelasUI parcelaReciboFormatada = new ReciboConfirmacaoParcelasUI();
                        parcelaReciboFormatada.parcela = sqlParcela;
                        parcelasFormatadas.Add(parcelaReciboFormatada);
                    }
                }

                return parcelasFormatadas;


            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<ReciboConfirmacaoParcelasUI> getParcelasReciboConfirmacaoByMovimento(int cd_movimento, int cd_empresa)
        {
            try
            {

                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Movimento"].ToString());

                ParcelasReciboConfirmacaoResultUI sql = (from c in db.Movimento
                                                         where c.cd_movimento == cd_movimento &&
                                                               c.cd_pessoa_empresa == cd_empresa
                                                         select new
                                                         {
                                                             titulos = (from i in db.Titulo //.Include(l => l.LocalMovto)
                                                                        where i.cd_origem_titulo == cd_movimento &&
                                                                              i.id_origem_titulo == cd_origem &&
                                                                              i.cd_pessoa_empresa == cd_empresa &&
                                                                              (!i.BaixaTitulo.Any()) &&
                                                                              (i.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CARTAO || i.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)
                                                                        select i).ToList(),
                                                         }).ToList().Select(x => new ParcelasReciboConfirmacaoResultUI()
                                                         {
                                                             titulos = x.titulos

                                                         }).FirstOrDefault();

                List<ReciboConfirmacaoParcelasUI> parcelasFormatadas = new List<ReciboConfirmacaoParcelasUI>();
                if (sql != null && sql.parcelas != null && sql.parcelas.Count > 0)
                {
                    foreach (var sqlParcela in sql.parcelas)
                    {
                        ReciboConfirmacaoParcelasUI parcelaReciboFormatada = new ReciboConfirmacaoParcelasUI();
                        parcelaReciboFormatada.parcela = sqlParcela;
                        parcelasFormatadas.Add(parcelaReciboFormatada);
                    }
                }

                return parcelasFormatadas;


            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
