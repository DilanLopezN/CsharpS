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

namespace FundacaoFisk.SGF.Web.Services.CNAB.DataAccess
{
    public class TituloCnabDataAccess : GenericRepository<TituloCnab>, ITituloCnabDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<TituloCnab> getTitulosCnabBoletoByCnabs(int cd_escola, Int32[] cd_cnab, Int32[] cd_titulo_cnab, bool eh_responsavel)
        {
            try
            {
                var sql = from c in db.TituloCnab.AsNoTracking()
                          where c.Titulo.cd_pessoa_empresa == cd_escola
                          select c;
                if (cd_cnab != null && cd_cnab.Length > 0)
                    sql = from c in db.TituloCnab
                          where cd_cnab.Contains(c.cd_cnab)
                          select c;
                else if (cd_titulo_cnab != null && cd_titulo_cnab.Length > 0)
                    sql = from c in db.TituloCnab
                          where cd_titulo_cnab.Contains(c.cd_titulo_cnab)
                          select c;

                var retorno = (from c in sql
                               join cn in db.Cnab on c.cd_cnab equals cn.cd_cnab
                               join t in db.Titulo on c.cd_titulo equals t.cd_titulo
                               join l in db.LocalMovto on t.cd_local_movto equals l.cd_local_movto
                               join pr in db.PessoaSGF on t.cd_pessoa_responsavel equals pr.cd_pessoa
                               join pt in db.PessoaSGF on t.cd_pessoa_titulo equals pt.cd_pessoa
                               join er in db.EnderecoSGF on pr.cd_endereco_principal equals er.cd_endereco into InnerC
                               from er in InnerC.DefaultIfEmpty()
                               join et in db.EnderecoSGF on pt.cd_endereco_principal equals et.cd_endereco into InnersD
                               from et in InnersD.DefaultIfEmpty()
                               //join pb in db.PessoaSGF on l.cd_pessoa_banco equals pb.cd_pessoa
                               //join eb in db.EnderecoSGF on pb.cd_endereco_principal equals eb.cd_endereco
                               join pessoa_banco in db.PessoaSGF on l.cd_pessoa_banco equals pessoa_banco.cd_pessoa into Inners
                               from pb in Inners.DefaultIfEmpty()
                               join end_banco in db.EnderecoSGF on pb.cd_endereco_principal equals end_banco.cd_endereco into InnersB
                               from eb in InnersB.DefaultIfEmpty()

                               select new
                               {
                                   cd_cnab = c.cd_cnab,
                                   cd_titulo = c.cd_titulo,
                                   no_turma = c.Turma.TurmaPai != null ? c.Turma.TurmaPai.no_turma : c.Turma.no_turma,
                                   dc_nosso_numero_titulo = c.dc_nosso_numero_titulo,
                                   pc_juros_titulo = c.pc_juros_titulo,
                                   pc_multa_titulo = c.pc_multa_titulo,
                                   //vl_desconto_titulo = c.vl_desconto_titulo,
                                   tx_mensagem_cnab = c.tx_mensagem_cnab,
                                   dt_emissao_titulo = t.dt_emissao_titulo,
                                   dt_vencimento_titulo = t.dt_vcto_titulo,
                                   nm_banco = l.Banco.nm_banco,
                                   no_pessoa_banco = l.PessoaSGFBanco.no_pessoa,
                                   no_pessoa_escola = c.LocalMovimento.Empresa.no_pessoa,
                                   nm_agencia = l.nm_agencia,
                                   l.nm_digito_agencia,
                                   nm_conta_corrente = l.nm_conta_corrente,
                                   nm_digito_conta_corrente = l.nm_digito_conta_corrente,
                                   nm_cpf_cgc_empresa = c.LocalMovimento.Empresa.dc_num_cgc,
                                   nm_cpf_cgc_pessoa_banco = l.PessoaSGFBanco.nm_natureza_pessoa == 1 ?
                                      db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == l.PessoaSGFBanco.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                          db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa ==l.PessoaSGFBanco.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                                   nm_inscricao_pessoa_banco = l.PessoaSGFBanco.nm_natureza_pessoa == 2 ?
                                          db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == l.PessoaSGFBanco.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_insc_estadual : "",
                                   nm_carteira = l.CarteiraCnab.nm_carteira,
                                   id_registrada = l.CarteiraCnab != null ? c.Titulo.LocalMovto.CarteiraCnab.id_registrada : false,
                                   nm_colunas = l.CarteiraCnab.nm_colunas,
                                   dc_num_cliente_banco = l.dc_num_cliente_banco,
                                   nm_digito_cedente = l.nm_digito_cedente,
                                   nm_op_conta = l.nm_op_conta,
                                   vl_titulo = t.vl_titulo,
                                   vl_desconto_bolsa = t.vl_liquidacao_titulo > 0 && t.BaixaTitulo.Any(x => x.cd_titulo == c.cd_titulo && x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA) ?
                                                   t.BaixaTitulo.Where(x => x.cd_titulo == c.cd_titulo && x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA).FirstOrDefault().vl_baixa_saldo_titulo : 0,
                                   nm_cpf = t.PessoaResponsavel.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                      db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == pr.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                         db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == pr.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                            db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == pr.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                                db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == pr.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                                   no_bairro = eh_responsavel ? er.Bairro.no_localidade : et.Bairro.no_localidade,
                                   no_cidade = eh_responsavel ? er.Cidade.no_localidade : et.Cidade.no_localidade,
                                   dc_num_cep = eh_responsavel ? er.Logradouro.dc_num_cep : et.Logradouro.dc_num_cep,
                                   sg_estado = eh_responsavel ? er.Estado.Estado.sg_estado : et.Estado.Estado.sg_estado,
                                   no_localidade = eh_responsavel ? er.Logradouro.no_localidade : et.Logradouro.no_localidade,
                                   dc_compl_endereco = eh_responsavel ? er.dc_compl_endereco : et.dc_compl_endereco,
                                   no_tipo_logradouro = eh_responsavel ? er.TipoLogradouro.no_tipo_logradouro : et.TipoLogradouro.no_tipo_logradouro,
                                   dc_num_endereco = eh_responsavel ? er.dc_num_endereco : et.dc_num_endereco,

                                   dc_num_endereco_cedente = c.LocalMovimento.Empresa.EnderecoPrincipal.dc_num_endereco,
                                   no_tipo_logradouro_cedente = c.LocalMovimento.Empresa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro,
                                   no_bairro_cedente = c.LocalMovimento.Empresa.EnderecoPrincipal.Bairro.no_localidade,
                                   sg_estado_cedente = c.LocalMovimento.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                                   no_localidade_cidade_cedente = c.LocalMovimento.Empresa.EnderecoPrincipal.Cidade.no_localidade,
                                   dc_num_cep_cedente = c.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep,
                                   no_localidade_cedente = c.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade,

                                   nm_parcela_titulo = t.nm_parcela_titulo,
                                   nm_titulo = t.nm_titulo,
                                   no_pessoa = eh_responsavel ? pr.no_pessoa : pt.no_pessoa,
                                   no_pessoa_ordem = pt.no_pessoa,
                                   no_aluno = eh_responsavel ? "   - Aluno(a): " + pt.no_pessoa : "",
                                   email_resp = (from tp in db.TelefoneSGF where pr.cd_pessoa == tp.cd_pessoa && tp.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL select tp.dc_fone_mail).FirstOrDefault() != null ?
                                                (from tp in db.TelefoneSGF where pr.cd_pessoa == tp.cd_pessoa && tp.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL select tp.dc_fone_mail).FirstOrDefault() :
                                                (from tp in db.TelefoneSGF where pt.cd_pessoa == tp.cd_pessoa && tp.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL select tp.dc_fone_mail).FirstOrDefault(),
                                   cd_pessoa = t.cd_pessoa_titulo, //Campo para compor o código do pagador no boleto do HSBC,
                                   DescontoTituloCNAB = c.DescontoTituloCNAB,
                                   cn.nm_dias_protesto,
                                   //Endereço pessoa banco
                                   dc_num_endereco_localMMvto = eb.dc_num_endereco,
                                   no_tipo_logradouro_localMMvto = eb.TipoLogradouro.no_tipo_logradouro,
                                   no_bairro_localMMvto = eb.Bairro.no_localidade,
                                   sg_estado_localMMvto = eb.Estado.Estado.sg_estado,
                                   no_localidade_cidade_localMMvto = eb.Cidade.no_localidade,
                                   dc_num_cep_localMMvto = eb.Logradouro.dc_num_cep,
                                   no_localidade_localMMvto = eb.Logradouro.no_localidade,
                                   dt_emissao_cnab = cn.dt_emissao_cnab
                               }).ToList().Select(x => new TituloCnab
                               {
                                   cd_cnab = x.cd_cnab,
                                   cd_titulo = x.cd_titulo,
                                   dc_nosso_numero_titulo = x.dc_nosso_numero_titulo,
                                   nomePessoaTitulo = x.no_pessoa_ordem,
                                   no_turma_titulo = x.no_turma,
                                   pc_juros_titulo = x.pc_juros_titulo,
                                   pc_multa_titulo = x.pc_multa_titulo,
                                   DescontoTituloCNAB = x.DescontoTituloCNAB.Select(b => new DescontoTituloCNAB
                                   {
                                       cd_desconto_titulo_cnab = b.cd_desconto_titulo_cnab,
                                       cd_titulo_cnab = b.cd_titulo_cnab,
                                       dt_desconto = b.dt_desconto,
                                       vl_desconto = b.vl_desconto,

                                   }).ToList(),
                                   //vl_desconto_titulo = x.vl_desconto_titulo,
                                   tx_mensagem_cnab = x.tx_mensagem_cnab,
                                   dt_vencimento_titulo = x.dt_vencimento_titulo,
                                   LocalMovimento = new LocalMovto()
                                   {
                                       Banco = new Banco()
                                       {
                                           nm_banco = x.nm_banco
                                       },
                                       PessoaSGFBanco = new PessoaSGF()
                                       {
                                           dc_num_insc_estadual_PJ = x.nm_inscricao_pessoa_banco,
                                           nm_cpf_cgc = x.nm_cpf_cgc_pessoa_banco,
                                           no_pessoa = x.no_pessoa_banco,
                                           EnderecoPrincipal = new EnderecoSGF()
                                           {
                                               dc_num_endereco = x.dc_num_endereco_localMMvto,
                                               TipoLogradouro = new TipoLogradouroSGF()
                                               {
                                                   no_tipo_logradouro = x.no_tipo_logradouro_localMMvto
                                               },
                                               Bairro = new LocalidadeSGF()
                                               {
                                                   no_localidade = x.no_bairro_localMMvto
                                               },
                                               Estado = new LocalidadeSGF()
                                               {
                                                   Estado = new EstadoSGF()
                                                   {
                                                       sg_estado = x.sg_estado_localMMvto
                                                   }
                                               },
                                               Cidade = new LocalidadeSGF()
                                               {
                                                   no_localidade = x.no_localidade_cidade_localMMvto
                                               },
                                               Logradouro = new LocalidadeSGF()
                                               {
                                                   dc_num_cep = x.dc_num_cep_localMMvto,
                                                   no_localidade = x.no_localidade_localMMvto
                                               }
                                           }
                                       },
                                       CarteiraCnab = new CarteiraCnab()
                                       {
                                           nm_carteira = x.nm_carteira,
                                           id_registrada = x.id_registrada,
                                           nm_colunas = x.nm_colunas
                                       },
                                       Empresa = new Escola()
                                       {
                                           no_pessoa = x.no_pessoa_escola,
                                           dc_num_cgc = x.nm_cpf_cgc_empresa,
                                           nm_cpf_cgc = x.nm_cpf_cgc_empresa,
                                           EnderecoPrincipal = new EnderecoSGF()
                                           {
                                               dc_num_endereco = x.dc_num_endereco_cedente,
                                               TipoLogradouro = new TipoLogradouroSGF()
                                               {
                                                   no_tipo_logradouro = x.no_tipo_logradouro_cedente
                                               },
                                               Bairro = new LocalidadeSGF()
                                               {
                                                   no_localidade = x.no_bairro_cedente
                                               },
                                               Estado = new LocalidadeSGF()
                                               {
                                                   Estado = new EstadoSGF()
                                                   {
                                                       sg_estado = x.sg_estado_cedente
                                                   }
                                               },
                                               Cidade = new LocalidadeSGF()
                                               {
                                                   no_localidade = x.no_localidade_cidade_cedente
                                               },
                                               Logradouro = new LocalidadeSGF()
                                               {
                                                   dc_num_cep = x.dc_num_cep_cedente,
                                                   no_localidade = x.no_localidade_cedente
                                               }
                                           }
                                       },
                                       nm_agencia = x.nm_agencia,
                                       nm_conta_corrente = x.nm_conta_corrente,
                                       nm_digito_conta_corrente = x.nm_digito_conta_corrente,
                                       dc_num_cliente_banco = x.dc_num_cliente_banco,
                                       nm_digito_cedente = x.nm_digito_cedente,
                                       nm_op_conta = x.nm_op_conta,
                                       nm_digito_agencia = x.nm_digito_agencia
                                   },
                                   Titulo = new Titulo()
                                   {
                                       Pessoa = new PessoaSGF()
                                       {
                                           cd_pessoa = x.cd_pessoa,
                                           no_pessoa = x.no_pessoa,
                                           no_aluno = x.no_aluno,
                                           nm_cpf_cgc = x.nm_cpf,
                                           EnderecoPrincipal = new EnderecoSGF()
                                           {
                                               TipoLogradouro = new TipoLogradouroSGF()
                                               {
                                                   no_tipo_logradouro = x.no_tipo_logradouro
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
                                               dc_num_endereco = x.dc_num_endereco,
                                               dc_compl_endereco = x.dc_compl_endereco
                                           }
                                       },
                                       vl_titulo = x.vl_desconto_bolsa > 0 ? x.vl_titulo - x.vl_desconto_bolsa : x.vl_titulo,
                                       nm_parcela_titulo = x.nm_parcela_titulo,
                                       nm_titulo = x.nm_titulo,
                                       dt_emissao_titulo = x.dt_emissao_titulo,
                                       dt_emissao_cnab = x.dt_emissao_cnab
                                   },
                                   emailPessoaTitulo = x.email_resp,
                                   nm_dias_protesto = x.nm_dias_protesto.HasValue ? (int)x.nm_dias_protesto.Value : 0
                               });

                return retorno.OrderBy(x => x.no_turma_titulo).ThenBy(x => x.nomePessoaTitulo);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TituloCnab> getTitulosCnabBoletoByTitulosCnab(int cd_escola, Int32[] cd_titulo_cnab, bool eh_responsavel) {
            try {
                var sql = (from c in db.TituloCnab
                           where cd_titulo_cnab.Contains(c.cd_titulo_cnab)
                                 && c.Titulo.cd_pessoa_empresa == cd_escola
                           select new {
                               cd_titulo = c.cd_titulo,
                               cd_cnab = c.cd_cnab,
                               dc_nosso_numero_titulo = c.dc_nosso_numero_titulo,
                               no_turma = c.Turma.TurmaPai != null ? c.Turma.TurmaPai.no_turma : c.Turma.no_turma,
                               pc_juros_titulo = c.pc_juros_titulo,
                               pc_multa_titulo = c.pc_multa_titulo,
                               //vl_desconto_titulo = c.vl_desconto_titulo,
                               tx_mensagem_cnab = c.tx_mensagem_cnab,
                               dt_emissao_titulo = c.Titulo.dt_emissao_titulo,
                               dt_vencimento_titulo = c.Titulo.dt_vcto_titulo,
                               nm_banco = c.Titulo.LocalMovto.Banco.nm_banco,
                               //Pessoa banco
                               no_pessoa_banco = c.Titulo.LocalMovto.PessoaSGFBanco.no_pessoa,
                               nm_cpf_cgc_pessoa_banco = c.Titulo.LocalMovto.PessoaSGFBanco.nm_natureza_pessoa == 1 ?
                               db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.Titulo.LocalMovto.PessoaSGFBanco.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                   db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.Titulo.LocalMovto.PessoaSGFBanco.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,

                               nm_agencia = c.Titulo.LocalMovto.nm_agencia,
                               nm_conta_corrente = c.Titulo.LocalMovto.nm_conta_corrente,
                               nm_digito_conta_corrente = c.Titulo.LocalMovto.nm_digito_conta_corrente,c,
                               //Escola
                               no_pessoa_escola = c.LocalMovimento.Empresa.no_pessoa,
                               nm_cpf_cgc_empresa = c.LocalMovimento.Empresa.dc_num_cgc,

                               nm_carteira = c.Titulo.LocalMovto.CarteiraCnab.nm_carteira,
                               id_registrada = c.Titulo.LocalMovto.CarteiraCnab != null ? c.Titulo.LocalMovto.CarteiraCnab.id_registrada : false,
                               nm_colunas = c.Titulo.LocalMovto.CarteiraCnab.nm_colunas,
                               dc_num_cliente_banco = c.Titulo.LocalMovto.dc_num_cliente_banco,
                               nm_digito_cedente = c.Titulo.LocalMovto.nm_digito_cedente,
                               nm_op_conta = c.Titulo.LocalMovto.nm_op_conta,
                               c.Titulo.LocalMovto.nm_digito_agencia,
                               vl_titulo = c.Titulo.vl_titulo,
                               vl_desconto_bolsa = c.Titulo.vl_liquidacao_titulo > 0 && c.Titulo.BaixaTitulo.Any(x => x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                                                      x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO) ?
                                                   c.Titulo.BaixaTitulo.Where(x => x.cd_titulo == c.cd_titulo && x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                                                 x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO).FirstOrDefault().vl_baixa_saldo_titulo : 0,
                               nm_cpf = c.Titulo.PessoaResponsavel.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                  db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                     db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                            db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                               no_bairro = eh_responsavel ? c.Titulo.PessoaResponsavel.EnderecoPrincipal.Bairro.no_localidade : c.Titulo.Pessoa.EnderecoPrincipal.Bairro.no_localidade,
                               no_cidade = eh_responsavel ? c.Titulo.PessoaResponsavel.EnderecoPrincipal.Cidade.no_localidade : c.Titulo.Pessoa.EnderecoPrincipal.Cidade.no_localidade,
                               dc_num_cep = eh_responsavel ? c.Titulo.PessoaResponsavel.EnderecoPrincipal.Logradouro.dc_num_cep : c.Titulo.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep,
                               sg_estado = eh_responsavel ? c.Titulo.PessoaResponsavel.EnderecoPrincipal.Estado.Estado.sg_estado : c.Titulo.Pessoa.EnderecoPrincipal.Estado.Estado.sg_estado,
                               no_localidade = eh_responsavel ? c.Titulo.PessoaResponsavel.EnderecoPrincipal.Logradouro.no_localidade : c.Titulo.Pessoa.EnderecoPrincipal.Logradouro.no_localidade,
                               dc_compl_endereco = eh_responsavel ? c.Titulo.PessoaResponsavel.EnderecoPrincipal.dc_compl_endereco : c.Titulo.Pessoa.EnderecoPrincipal.dc_compl_endereco,
                               no_tipo_logradouro = eh_responsavel ? c.Titulo.PessoaResponsavel.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro : c.Titulo.Pessoa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro,
                               dc_num_endereco = eh_responsavel ? c.Titulo.PessoaResponsavel.EnderecoPrincipal.dc_num_endereco : c.Titulo.Pessoa.EnderecoPrincipal.dc_num_endereco,

                               nm_inscricao_pessoa_banco = c.Titulo.LocalMovto.PessoaSGFBanco.nm_natureza_pessoa == 2 ?
       db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.Titulo.LocalMovto.PessoaSGFBanco.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_insc_estadual : "",
                               dc_num_endereco_cedente = c.LocalMovimento.Empresa.EnderecoPrincipal.dc_num_endereco,
                               no_tipo_logradouro_cedente = c.LocalMovimento.Empresa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro,
                               no_bairro_cedente = c.LocalMovimento.Empresa.EnderecoPrincipal.Bairro.no_localidade,
                               sg_estado_cedente = c.LocalMovimento.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                               no_localidade_cidade_cedente = c.LocalMovimento.Empresa.EnderecoPrincipal.Cidade.no_localidade,
                               dc_num_cep_cedente = c.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep,
                               no_localidade_cedente = c.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade,

                               //Endereço pessoa banco
                               dc_num_endereco_localMMvto = c.Titulo.LocalMovto.PessoaSGFBanco.EnderecoPrincipal.dc_num_endereco,
                               no_tipo_logradouro_localMMvto = c.Titulo.LocalMovto.PessoaSGFBanco.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro,
                               no_bairro_localMMvto = c.Titulo.LocalMovto.PessoaSGFBanco.EnderecoPrincipal.Bairro.no_localidade,
                               sg_estado_localMMvto = c.Titulo.LocalMovto.PessoaSGFBanco.EnderecoPrincipal.Estado.Estado.sg_estado,
                               no_localidade_cidade_localMMvto = c.Titulo.LocalMovto.PessoaSGFBanco.EnderecoPrincipal.Cidade.no_localidade,
                               dc_num_cep_localMMvto = c.Titulo.LocalMovto.PessoaSGFBanco.EnderecoPrincipal.Logradouro.dc_num_cep,
                               no_localidade_localMMvto = c.Titulo.LocalMovto.PessoaSGFBanco.EnderecoPrincipal.Logradouro.no_localidade,

                               nm_parcela_titulo = c.Titulo.nm_parcela_titulo,
                               nm_titulo = c.Titulo.nm_titulo,
                               no_pessoa = eh_responsavel ? c.Titulo.PessoaResponsavel.no_pessoa : c.Titulo.Pessoa.no_pessoa,
                               no_pessoa_ordem = c.Titulo.Pessoa.no_pessoa,
                               no_aluno = eh_responsavel ? "   - Aluno(a): " + c.Titulo.Pessoa.no_pessoa : "",
                               email_resp = c.Titulo.PessoaResponsavel.TelefonePessoa.Where(tp => tp.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail != null ?
                                                c.Titulo.PessoaResponsavel.TelefonePessoa.Where(tp => tp.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail :
                                                c.Titulo.Pessoa.TelefonePessoa.Where(tp => tp.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                               cd_pessoa = c.Titulo.cd_pessoa_titulo, //Campo para compor o código do pagador no boleto do HSBC,
                               c.DescontoTituloCNAB,
                               c.Cnab.nm_dias_protesto,
                               dt_emissao_cnab = c.Cnab.dt_emissao_cnab
                           }).ToList().Select(x => new TituloCnab
                           {
                               cd_cnab = x.cd_cnab,
                               cd_titulo = x.cd_titulo,
                               dc_nosso_numero_titulo = x.dc_nosso_numero_titulo,
                               nomePessoaTitulo = x.no_pessoa_ordem,
                               no_turma_titulo = x.no_turma,
                               pc_juros_titulo = x.pc_juros_titulo,
                               pc_multa_titulo = x.pc_multa_titulo,
                               DescontoTituloCNAB = x.DescontoTituloCNAB,
                               //vl_desconto_titulo = x.vl_desconto_titulo,
                               tx_mensagem_cnab = x.tx_mensagem_cnab,
                               dt_vencimento_titulo = x.dt_vencimento_titulo,
                               LocalMovimento = new LocalMovto()
                               {
                                   Banco = new Banco()
                                   {
                                       nm_banco = x.nm_banco
                                   },
                                   PessoaSGFBanco = new PessoaSGF()
                                   {
                                       dc_num_insc_estadual_PJ = x.nm_inscricao_pessoa_banco,
                                       nm_cpf_cgc = x.nm_cpf_cgc_pessoa_banco,
                                       no_pessoa = x.no_pessoa_banco,
                                       EnderecoPrincipal = new EnderecoSGF()
                                       {
                                           dc_num_endereco = x.dc_num_endereco_localMMvto,
                                           TipoLogradouro = new TipoLogradouroSGF()
                                           {
                                               no_tipo_logradouro = x.no_tipo_logradouro_localMMvto
                                           },
                                           Bairro = new LocalidadeSGF()
                                           {
                                               no_localidade = x.no_bairro_localMMvto
                                           },
                                           Estado = new LocalidadeSGF()
                                           {
                                               Estado = new EstadoSGF()
                                               {
                                                   sg_estado = x.sg_estado_localMMvto
                                               }
                                           },
                                           Cidade = new LocalidadeSGF()
                                           {
                                               no_localidade = x.no_localidade_cidade_localMMvto
                                           },
                                           Logradouro = new LocalidadeSGF()
                                           {
                                               dc_num_cep = x.dc_num_cep_localMMvto,
                                               no_localidade = x.no_localidade_localMMvto
                                           }
                                       }
                                   },
                                   CarteiraCnab = new CarteiraCnab()
                                   {
                                       nm_carteira = x.nm_carteira,
                                       id_registrada = x.id_registrada,
                                       nm_colunas = x.nm_colunas
                                   },
                                   Empresa = new Escola()
                                   {
                                       no_pessoa = x.no_pessoa_escola,
                                       dc_num_cgc = x.nm_cpf_cgc_empresa,
                                       nm_cpf_cgc = x.nm_cpf_cgc_empresa,
                                       EnderecoPrincipal = new EnderecoSGF()
                                       {
                                           dc_num_endereco = x.dc_num_endereco_cedente,
                                           TipoLogradouro = new TipoLogradouroSGF()
                                           {
                                               no_tipo_logradouro = x.no_tipo_logradouro_cedente
                                           },
                                           Bairro = new LocalidadeSGF()
                                           {
                                               no_localidade = x.no_bairro_cedente
                                           },
                                           Estado = new LocalidadeSGF()
                                           {
                                               Estado = new EstadoSGF()
                                               {
                                                   sg_estado = x.sg_estado_cedente
                                               }
                                           },
                                           Cidade = new LocalidadeSGF()
                                           {
                                               no_localidade = x.no_localidade_cidade_cedente
                                           },
                                           Logradouro = new LocalidadeSGF()
                                           {
                                               dc_num_cep = x.dc_num_cep_cedente,
                                               no_localidade = x.no_localidade_cedente
                                           }
                                       }
                                   },
                                   nm_agencia = x.nm_agencia,
                                   nm_conta_corrente = x.nm_conta_corrente,
                                   nm_digito_conta_corrente = x.nm_digito_conta_corrente,
                                   dc_num_cliente_banco = x.dc_num_cliente_banco,
                                   nm_digito_cedente = x.nm_digito_cedente,
                                   nm_op_conta = x.nm_op_conta,
                                   nm_digito_agencia = x.nm_digito_agencia
                               },
                               Titulo = new Titulo()
                               {
                                   Pessoa = new PessoaSGF()
                                   {
                                       cd_pessoa = x.cd_pessoa,
                                       no_pessoa = x.no_pessoa,
                                       no_aluno = x.no_aluno,
                                       nm_cpf_cgc = x.nm_cpf,
                                       EnderecoPrincipal = new EnderecoSGF()
                                       {
                                           TipoLogradouro = new TipoLogradouroSGF()
                                           {
                                               no_tipo_logradouro = x.no_tipo_logradouro
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
                                           dc_num_endereco = x.dc_num_endereco,
                                           dc_compl_endereco = x.dc_compl_endereco
                                       }
                                   },
                                   vl_titulo = x.vl_desconto_bolsa > 0 ? x.vl_titulo - x.vl_desconto_bolsa : x.vl_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   nm_titulo = x.nm_titulo,
                                   dt_emissao_titulo = x.dt_emissao_titulo,
                                   dt_emissao_cnab = x.dt_emissao_cnab
                               },
                               emailPessoaTitulo = x.email_resp,
                               nm_dias_protesto = x.nm_dias_protesto.HasValue ? (int)x.nm_dias_protesto.Value : 0
                           });

                return sql.OrderBy(x => x.no_turma_titulo).ThenBy(x => x.nomePessoaTitulo);
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public TituloCnab getTituloCnabEditView(int cd_cnab, int cd_titulo_cnab, int cdEmpresa)
        {
            try
            {
                var sql = (from tc in db.TituloCnab
                           where tc.Cnab.LocalMovimento.cd_pessoa_empresa == cdEmpresa && tc.Cnab.cd_cnab == cd_cnab && tc.cd_titulo_cnab == cd_titulo_cnab
                           select new
                           {
                               cd_titulo_cnab = tc.cd_titulo_cnab,
                               no_local_movto = tc.LocalMovimento.no_local_movto,
                               nm_agencia = tc.LocalMovimento.nm_agencia,
                               nm_conta_corrente = tc.LocalMovimento.nm_conta_corrente,
                               nm_digito_conta_corrente = tc.LocalMovimento.nm_digito_conta_corrente != null ? tc.LocalMovimento.nm_digito_conta_corrente : null,
                               nm_tipo_local = tc.LocalMovimento.nm_tipo_local,
                               nomePessoaTitulo = tc.Titulo.Pessoa.no_pessoa,
                               nm_titulo = tc.Titulo.nm_titulo,
                               nm_parcela_titulo = tc.Titulo.nm_parcela_titulo,
                               id_status_cnab_titulo = tc.id_status_cnab_titulo,
                               juros = tc.pc_juros_titulo,
                               multa = tc.pc_multa_titulo,
                               //desconto = tc.vl_desconto_titulo,
                               dt_vencimento_titulo = tc.dt_vencimento_titulo,
                               txt_mensagen_cnab = tc.tx_mensagem_cnab,
                               no_turma = tc.Turma.no_turma,
                               dc_nosso_numero_titulo = tc.dc_nosso_numero_titulo,
                               tc.DescontoTituloCNAB
                           }).ToList().Select(x => new TituloCnab
                                   {
                                       cd_titulo_cnab = x.cd_titulo_cnab,
                                       nomePessoaTitulo = x.nomePessoaTitulo,
                                       id_status_cnab_titulo = x.id_status_cnab_titulo,
                                       pc_juros_titulo = x.juros,
                                       pc_multa_titulo = x.multa,
                                       //vl_desconto_titulo = x.desconto,
                                       dt_vencimento_titulo = x.dt_vencimento_titulo,
                                       tx_mensagem_cnab = x.txt_mensagen_cnab,
                                       no_turma_titulo = x.no_turma,
                                       dc_nosso_numero_titulo =x.dc_nosso_numero_titulo,
                                       Titulo = new Titulo
                                       {
                                           nm_titulo = x.nm_titulo,
                                           nm_parcela_titulo = x.nm_parcela_titulo
                                       },
                                       LocalMovimento = new LocalMovto
                                       {
                                           //cd_local_movto = x.cd_local_movto,
                                           no_local_movto = x.no_local_movto,
                                           nm_agencia = x.nm_agencia,
                                           nm_conta_corrente = x.nm_conta_corrente,
                                           nm_digito_conta_corrente = x.nm_digito_conta_corrente,
                                           nm_tipo_local = x.nm_tipo_local
                                       },
                                       DescontoTituloCNAB = x.DescontoTituloCNAB
                                   }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TituloCnab> getAllTituloCnabByCnab(int cd_escola, int cd_cnab)
        {
            try
            {
                var sql = from tc in db.TituloCnab
                          where  tc.cd_cnab == cd_cnab &&  tc.Titulo.cd_pessoa_empresa == cd_escola
                          select tc;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public string getNossoNumeroTitulo(int cd_titulo) {
            try {
                var sql = (from tc in db.TituloCnab
                           where tc.cd_titulo == cd_titulo
                           select tc.dc_nosso_numero_titulo).FirstOrDefault();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public decimal somaValorTodosTitulosCnab(int cd_escola, int cd_cnab)
        {
            try
            {
                var sql = (from tc in db.TituloCnab
                          where tc.cd_cnab == cd_cnab && tc.Titulo.cd_pessoa_empresa == cd_escola
                           select tc).Sum(s => s.Titulo.vl_liquidacao_titulo > 0 && s.Titulo.BaixaTitulo.Any(x => x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                                                                                  x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO) ?
                                               s.Titulo.vl_titulo - s.Titulo.BaixaTitulo.Where(x => x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA ||
                                                                                                    x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO).FirstOrDefault().vl_baixa_saldo_titulo :
                                               s.Titulo.vl_titulo);
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Titulo> existeTituloCnabComStatusDiferenteEviado(int cd_cnab, int cd_empresa)
        {
            try
            {
                var retorno = from t in db.TituloCnab
                           where t.cd_cnab == cd_cnab && t.Titulo.cd_pessoa_empresa == cd_empresa
                           select t.Titulo;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}
