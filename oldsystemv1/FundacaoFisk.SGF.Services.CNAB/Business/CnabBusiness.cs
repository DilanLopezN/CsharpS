using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.GenericModel;
using System.Transactions;
using Componentes.GenericController;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IDataAccess;
using Componentes.Utils.Messages;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.CNAB.Business
{
    using Componentes.GenericModel;
    using System.Globalization;
    using System.Data.Entity;
    using FundacaoFisk.SGF.Web.Services.CNAB.DataAccess;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
    using System.Data.Entity.Infrastructure;
    using Componentes.GenericBusiness.Comum;
	using System.IO;
    using FundacaoFisk.SGF.Services.CNAB.Comum.IDataAccess;

    public class CnabBusiness : ICnabBusiness
    {

        /// <summary>
        /// Declarações de  interfaces
        /// </summary>
        public ICarteiraCnabDataAccess DaoCarteiraCnab { get; set; }
        public ICnabDataAccess DaoCnab { get; set; }
        public IRetornoCNABDataAccess DaoRetornoCnab { get; set; }
        public ITituloCnabDataAccess DaoTituloCnab { get; set; }
        public ITituloRetornoCnabDataAccess DaoTituloRetornoCnab { get; set; }
        public IFinanceiroBusiness BusinessFinanceiro { get; set; }
        public IDespesaTituloCnabDataAccess DaoDespesaTituloCnab { get; set; }
        public ITituloDataAccess TituloDataAccess { get; set; }

        public CnabBusiness(ICarteiraCnabDataAccess daoCarteiraCnab, ICnabDataAccess daoCnab, IRetornoCNABDataAccess daoRetornoCnab, ITituloCnabDataAccess daoTituloCnab,
                            IFinanceiroBusiness businessFinanceiro, ITituloRetornoCnabDataAccess daoTituloRetornoCnab, IDespesaTituloCnabDataAccess daoDespesaTituloCnab,
                            ITituloDataAccess tituloDataAccess)
        {
            if (daoCarteiraCnab == null || daoCnab == null || daoTituloCnab == null || daoRetornoCnab == null || daoTituloRetornoCnab == null || daoDespesaTituloCnab == null || tituloDataAccess == null)
                throw new ArgumentNullException();
            DaoCarteiraCnab = daoCarteiraCnab;
            DaoCnab = daoCnab;
            DaoRetornoCnab = daoRetornoCnab;
            DaoTituloCnab = daoTituloCnab;
            BusinessFinanceiro = businessFinanceiro;
            DaoTituloRetornoCnab = daoTituloRetornoCnab;
            DaoDespesaTituloCnab = daoDespesaTituloCnab;
            TituloDataAccess = tituloDataAccess;
        }

        // Configura os codigos do usuário para auditorias dos DataAccess
        public void configuraUsuario(int cdUsuario, int cd_empresa)
        {
            ((SGFWebContext)this.DaoCarteiraCnab.DB()).IdUsuario = ((SGFWebContext)this.DaoCnab.DB()).IdUsuario = ((SGFWebContext)this.DaoRetornoCnab.DB()).IdUsuario =
                ((SGFWebContext)this.DaoTituloCnab.DB()).IdUsuario = ((SGFWebContext)this.DaoTituloRetornoCnab.DB()).IdUsuario = ((SGFWebContext)this.TituloDataAccess.DB()).IdUsuario = cdUsuario; ;
            ((SGFWebContext)this.DaoCarteiraCnab.DB()).cd_empresa = ((SGFWebContext)this.DaoCnab.DB()).cd_empresa = ((SGFWebContext)this.DaoRetornoCnab.DB()).cd_empresa =
                ((SGFWebContext)this.DaoTituloCnab.DB()).cd_empresa = ((SGFWebContext)this.DaoTituloRetornoCnab.DB()).cd_empresa =
                ((SGFWebContext)this.DaoDespesaTituloCnab.DB()).cd_empresa = ((SGFWebContext)this.TituloDataAccess.DB()).cd_empresa = cd_empresa; ;

            BusinessFinanceiro.configuraUsuario(cdUsuario, cd_empresa);
        }

        public void sincronizarContextos(DbContext dbContext)
        {
            //this.DaoCarteiraCnab.sincronizaContexto(dbContext);
            //this.DaoCnab.sincronizaContexto(dbContext);
            //this.DaoRetornoCnab.sincronizaContexto(dbContext);
            //this.DaoTituloCnab.sincronizaContexto(dbContext);
            //this.DaoTituloRetornoCnab.sincronizaContexto(dbContext);
            //BusinessFinanceiro.sincronizarContextos(dbContext);
        }

        #region Cnab

        public void verificarCarteiraRegistrada(int cd_escola, int cd_cnab, int cd_carteira_cnab, ReturnResult retornoErrors) {
            CarteiraCnab carteiraCnab = DaoCarteiraCnab.findById(cd_carteira_cnab, false);
            //CarteiraCnab carteiraCnab = DaoCarteiraCnab.getCarteiraByCarteira(cd_carteira_cnab);
            if (!carteiraCnab.id_registrada)
            {
                if (retornoErrors != null)
                {
                    retornoErrors.AddMensagem(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroGerarRemessaCarteiraSemRegistro, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                }
                else
                {
                    throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroGerarRemessaCarteiraSemRegistro, null, CnabBusinessException.TipoErro.ERRO_GERAR_REMESSA_CARTEIRA_SEM_REGISTRO, false);
                }
                
            }
                
        }

        public void verificarGerouCnab(int cd_escola, Int32[] cd_cnab, Int32[] tipos_cnab, byte status_cnab, bool is_titulo, ReturnResult retornoErrors)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (!DaoCnab.verificarGerouCnab(cd_escola, cd_cnab, tipos_cnab, status_cnab, is_titulo))
                {
                    if (retornoErrors != null)
                    {
                        retornoErrors.AddMensagem(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroNaoGerouCnab, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    }
                    else
                    {
                        throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroNaoGerouCnab, null, CnabBusinessException.TipoErro.ERRO_NAO_GEROU_CNAB, false);
                    }
                }
                    
                transaction.Complete();
            }
        }

        public void verificarCdContratoCnab(int cd_escola, int cd_cnab, ReturnResult retornoErrors)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                var cnab = DaoCnab.getCnabFull(cd_cnab, cd_escola);
                if (cnab != null && cnab.cd_contrato.HasValue && cnab.cd_contrato > 0)
                {
                    if (retornoErrors != null)
                    {
                        retornoErrors.AddMensagem(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroGerarRemessaCdContratoCnab, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    }
                    else
                    {
                        throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroGerarRemessaCdContratoCnab, null, CnabBusinessException.TipoErro.ERRO_GERAR_REMESSA_CD_CONTRATO_CNAB, false);
                    }
                }
                    
                transaction.Complete();
            }
        }

        public IEnumerable<Cnab> searchCnab(SearchParameters parametros, int cd_carteira, int cd_usuario, byte tipo_cnab, int status, DateTime? dtInicial,
                                                 DateTime? dtFinal, bool emissao, bool vencimento, string nossoNumero, int? cd_contrato, int cd_empresa,
                                                bool icnab, bool iboleto, int cd_responsavel, int cd_aluno)
        {
            IEnumerable<Cnab> retorno = new List<Cnab>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                {
                    parametros.sort = "dh_cadastro_cnab";
                    parametros.sortOrder = SortDirection.Descending;
                }
                parametros.sort = parametros.sort.Replace("carteira_cnab", "cd_local_movto");
                parametros.sort = parametros.sort.Replace("tipoCnab", "id_tipo_cnab");
                parametros.sort = parametros.sort.Replace("dta_emissao_cnab", "dt_emissao_cnab");
                parametros.sort = parametros.sort.Replace("dta_inicial_vencimento", "dt_inicial_vencimento");
                parametros.sort = parametros.sort.Replace("dta_final_vencimento", "dt_final_vencimento");
                parametros.sort = parametros.sort.Replace("vlTotalCnab", "vl_total_cnab");
                parametros.sort = parametros.sort.Replace("usuarioCnab", "Usuario.no_login");
                parametros.sort = parametros.sort.Replace("dtah_cadastro_cnab", "dh_cadastro_cnab");
                parametros.sort = parametros.sort.Replace("statusCnab", "id_status_cnab");
                retorno = DaoCnab.searchCnab(parametros, cd_carteira, cd_usuario, tipo_cnab, status, dtInicial, dtFinal, emissao, vencimento, nossoNumero, cd_contrato, cd_empresa, icnab, iboleto, cd_responsavel, cd_aluno);
                transaction.Complete();
            }
            return retorno;
        }

        public Cnab postGerarCnab(int cd_escola, int cd_cnab)
        {
            int retorno = 0;
            //Verifica se todos os cnabs foram gerados:
            Int32[] codigos = new Int32[1];
            codigos[0] = cd_cnab;
            Cnab cnab = new Cnab();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoCnab.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                this.sincronizarContextos(DaoCnab.DB());
                cnab = DaoCnab.getCNABFullComTitulosCNAB(cd_cnab, cd_escola);
                atualizarVerificarCamposObrigatoriosCnab(cnab, cd_escola, ref retorno, true);

                //Muda o status do cnab:
                cnab.id_status_cnab = (byte)Cnab.StatusCnab.FECHADO;
                BusinessFinanceiro.trocarStatusCnabTitulos(codigos, cd_escola, Titulo.StatusCnabTitulo.ENVIADO_GERADO, cnab.cd_contrato);
                DaoCnab.saveChanges(false);
                transaction.Complete();
            }
            Cnab cnabGrade = DaoCnab.getCnabReturnGrade(cnab.cd_cnab, cd_escola);
            cnabGrade.cd_carteira_cnab = retorno;
            return cnabGrade;
        }

        public Cnab postGerarPedidoBaixaCnab(int cd_escola, int cd_cnab) {
            int retorno = 0;
            //Verifica se todos os cnabs foram gerados:
            Int32[] codigos = new Int32[1];
            codigos[0] = cd_cnab;
            BusinessFinanceiro.sincronizarContextos(DaoCnab.DB());
            Cnab cnab = DaoCnab.getCNABFullComTitulosCNAB(cd_cnab, cd_escola);
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoCnab.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                atualizarVerificarCamposObrigatoriosCnab(cnab, cd_escola, ref retorno, false);

                cnab.id_status_cnab = (byte) Cnab.StatusCnab.FECHADO;
                BusinessFinanceiro.trocarStatusCnabTitulos(codigos, cd_escola, Titulo.StatusCnabTitulo.PEDIDO_BAIXA);
                DaoCnab.saveChanges(false);
                transaction.Complete();
            }
            Cnab cnabGrade = DaoCnab.getCnabReturnGrade(cnab.cd_cnab, cd_escola);
            cnabGrade.cd_carteira_cnab = retorno;
            return cnabGrade;
        }

        private void atualizarVerificarCamposObrigatoriosCnab(Cnab cnab, int cd_escola, ref int retorno, bool sequencializar_nosso_numero)
        {
            if (cnab.id_status_cnab == (byte)Cnab.StatusCnab.FECHADO)
                throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroCnabJaGerado, null, CnabBusinessException.TipoErro.ERRO_NAO_GEROU_CNAB, false);

            //Gerar o nosso número de cada titulo do cnab:
            LocalMovto local = BusinessFinanceiro.getLocalMovimentoWithPessoaBanco(cnab.cd_local_movto);

            string banco = "";
            if (local.Banco != null && !String.IsNullOrEmpty(local.Banco.nm_banco)) { banco = local.Banco.nm_banco; }
            else if (!String.IsNullOrEmpty(local.nm_banco)) { banco = local.nm_banco; }

            //if(local.PessoaSGFBanco == null)
            //    throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroCnabSemPessoaBancoCNPJ, null, CnabBusinessException.TipoErro.ERRO_CNAB_SEM_PESSOA_BANCO_CNPJ, false);

            if (String.IsNullOrEmpty(local.dc_num_cliente_banco))
                throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroCamposBancoObrigatorios, null, CnabBusinessException.TipoErro.ERRO_CAMPOS_BANCO_OBRIGATORIO, false);

            if (String.IsNullOrEmpty(local.nm_agencia))
                throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroCnabSemAgencia, null, CnabBusinessException.TipoErro.ERRO_CNAB_SEM_AGENCIA, false);

            if (String.IsNullOrEmpty(local.nm_conta_corrente))
                throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroCnabSemContaCorrente, null, CnabBusinessException.TipoErro.ERRO_CNAB_SEM_CONTA_CORRENTE, false);

            if (string.IsNullOrEmpty(local.nm_digito_conta_corrente))
                throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroCnabSemDigito, null, CnabBusinessException.TipoErro.ERRO_CNAB_SEM_DIGITO, false);

            if (String.IsNullOrEmpty(local.Banco.nm_banco))
                throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroCnabSemNroBanco, null, CnabBusinessException.TipoErro.ERRO_CNAB_NRO_BANCO, false);

            if ((local.dc_nosso_numero == null || local.dc_nosso_numero < 0) && banco != Utils.Utils.FormatCode((int)Cnab.Bancos.Inter + "", 3))
                throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroNossoNumeroNaoConfigurado, null, CnabBusinessException.TipoErro.ERRO_NOSSO_NUMERO_NAO_INFORMADO, false);

            if (cnab.TitulosCnab.Any(c => c.Titulo.dt_emissao_titulo > DateTime.Now.Date))
                throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroDtEmissaoMaiorProcessamentoCnab, null, CnabBusinessException.TipoErro.ERRO_DT_EMISSAO_MAIOR_PROCESSAMENTO_CNAB, false);

            //Só gerar remessa se for carteira registrada id_registrada
            if (local.cd_carteira_cnab.HasValue)
            {
                CarteiraCnab carteiraCnab = DaoCarteiraCnab.findById(local.cd_carteira_cnab.Value, false);
                if (carteiraCnab.id_registrada)
                {
                   
                    local.nm_sequencia += 1;
                    cnab.nm_sequencia_remessa = local.nm_sequencia;

                    retorno = carteiraCnab.cd_carteira_cnab;
                }
            }


            long proximo_numero = 0;
            if (banco != Utils.Utils.FormatCode((int)Cnab.Bancos.Inter + "", 3))
            {
                proximo_numero = local.dc_nosso_numero.Value;

                foreach (TituloCnab tituloCnab in cnab.TitulosCnab)
                {

                    //TODO - Uilian Silva
                    //Quando criar novo cnab (SEM NUMERO CONTRATO)
                    //mostrar titulos com id_cnab_contrato = true para 
                    //incluir na grade se for da mesma carteira,
                    //ignorar porem alterar o id_status_cnab = 1 
                    //quando GERAR CNAB pois 
                    //estes titulos ja foram processados;
                    if (sequencializar_nosso_numero)
                    {
                        proximo_numero += 1;
                        if (!tituloCnab.Titulo.id_cnab_contrato)
                        {
                            if (string.IsNullOrEmpty(tituloCnab.Titulo.dc_nosso_numero))
                            {
                                tituloCnab.dc_nosso_numero_titulo = proximo_numero + "";
                                if (cnab.id_tipo_cnab == (byte)Cnab.TipoCnab.GERAR_BOLETOS)
                                    tituloCnab.Titulo.dc_nosso_numero = tituloCnab.dc_nosso_numero_titulo;
                            }
                            else
                            {
                                tituloCnab.dc_nosso_numero_titulo = tituloCnab.Titulo.dc_nosso_numero;
                            }


                        }
                    }
                    else if (!tituloCnab.Titulo.id_cnab_contrato)
                        tituloCnab.dc_nosso_numero_titulo = DaoTituloCnab.getNossoNumeroTitulo(tituloCnab.cd_titulo);
                }

                foreach (TituloCnab tituloCnab in cnab.TitulosCnab)
                {
                    tituloCnab.Titulo.cd_local_movto = cnab.cd_local_movto;
                }
            }
            if (sequencializar_nosso_numero && (banco != Utils.Utils.FormatCode((int)Cnab.Bancos.Inter + "", 3)))
                local.dc_nosso_numero = proximo_numero;
        }

        public Cnab postCancelarCnab(int cd_escola, int cd_cnab)
        {
            int retorno = 0;
            //Verifica se todos os cnabs foram gerados:
            Int32[] codigos = new Int32[1];
            codigos[0] = cd_cnab;
            this.sincronizarContextos(DaoCnab.DB());
            Cnab cnab = DaoCnab.getCNABFullComTitulosCNAB(cd_cnab, cd_escola);
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoCnab.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                if(cnab.id_status_cnab == (byte) Cnab.StatusCnab.FECHADO)
                    throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroPedidoBaixaGerado, null, CnabBusinessException.TipoErro.ERRO_PEDIDO_BAIXA_JA_GERADO, false);
                cnab.id_status_cnab = (byte)Cnab.StatusCnab.FECHADO;
                BusinessFinanceiro.trocarStatusCnabTitulos(codigos, cd_escola, Titulo.StatusCnabTitulo.INICIAL);
                DaoCnab.saveChanges(false);
                transaction.Complete();
            }
            Cnab cnabGrade = DaoCnab.getCnabReturnGrade(cnab.cd_cnab, cd_escola);
            cnabGrade.cd_carteira_cnab = retorno;
            return cnabGrade;
        }

        public Cnab getGerarRemessa(int cd_escola, int cd_cnab) {
            Cnab retorno = new Cnab();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DaoCnab.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                this.sincronizarContextos(DaoCnab.DB());
                bool eh_responsavel = false;
                Int32[] codigos = new Int32[1];
                retorno = DaoCnab.getGerarRemessa(cd_escola, cd_cnab);
                codigos[0] = cd_cnab;
                eh_responsavel = DaoCnab.isResponsavelCNAB(codigos);
                retorno.TitulosCnab = DaoTituloCnab.getTitulosCnabBoletoByCnabs(cd_escola, codigos, null, eh_responsavel).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public Cnab getCnabByRemessa(int cd_escola, int cd_cnab){
            return DaoCnab.getGerarRemessa(cd_escola, cd_cnab);
        }

        public IEnumerable<UsuarioWebSGF> getUsuariosCnab(int cd_empresa, bool adm, int cd_usuario)
        {
            return DaoCnab.getUsuariosCnab(cd_empresa, adm, cd_usuario);
        }

        public IEnumerable<CarteiraCnab> getCarteirasCnab(int cd_empresa)
        {
            return DaoCnab.getCarteirasCnab(cd_empresa);
        }

        public Cnab getCnabEditView(int cd_cnab, int cdEscola)
        {
            return DaoCnab.getCnabEditView(cd_cnab, cdEscola);
        }

        public Cnab addCnab(Cnab cnab, int cd_empresa)
        {
            DateTime dataCorrente = DateTime.Now.Date;

            if (cnab != null && ((cnab.dt_inicial_vencimento != null && DateTime.Compare((DateTime)cnab.dt_inicial_vencimento, new DateTime(1900, 1, 1)) < 0) ||
                (cnab.dt_final_vencimento != null && DateTime.Compare((DateTime)cnab.dt_final_vencimento, new DateTime(1900, 1, 1)) < 0)))
                throw new CnabBusinessException(Utils.Messages.Messages.msgErroMinDateCNAB, null, CnabBusinessException.TipoErro.ERRO_DATA_MIN_MAX_SMALLDATETIME, false);
            if (cnab != null && ((cnab.dt_inicial_vencimento != null && DateTime.Compare((DateTime)cnab.dt_inicial_vencimento, new DateTime(2079, 06, 06)) > 0) ||
                (cnab.dt_final_vencimento != null && DateTime.Compare((DateTime)cnab.dt_final_vencimento, new DateTime(2079, 06, 06)) > 0)))
                throw new CnabBusinessException(Utils.Messages.Messages.msgErroMaxDateCNAB, null, CnabBusinessException.TipoErro.ERRO_DATA_MIN_MAX_SMALLDATETIME, false);


            if (cnab.id_tipo_cnab == (int)Cnab.TipoCnab.GERAR_BOLETOS && DateTime.Compare(dataCorrente.Date, cnab.dt_inicial_vencimento.Date) > 0)
                throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgVenciIniMenorDataAtual), null,
                                                                  CnabBusinessException.TipoErro.ERRO_DATA_INI_MENOR_DATA_ATUAL, false);
            if (cnab.id_tipo_cnab == (int)Cnab.TipoCnab.GERAR_BOLETOS && DateTime.Compare(cnab.dt_inicial_vencimento, cnab.dt_final_vencimento) > 0)
                throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgVencFinalMenorDataIni), null,
                                                                  CnabBusinessException.TipoErro.ERRO_DATA_FINAL_MENOR_DATA_INICIAL, false);
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoCnab.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                this.sincronizarContextos(DaoCnab.DB());
                CarteiraCnab carteiraCnab = DaoCarteiraCnab.findById(cnab.cd_carteira_cnab, false);
                if (cnab.id_tipo_cnab == (int)Cnab.TipoCnab.CANCELAR_BOLETOS && carteiraCnab.id_registrada)
                    throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgCancelamentoCnabCarteiraReg), null,
                                                                 CnabBusinessException.TipoErro.ERRO_TIPO_CNAB_CANCELAMENTO_CARTEIRA_REGISTRADA, false);
                if (cnab.id_tipo_cnab == (int)Cnab.TipoCnab.PEDIDO_BAIXA && !carteiraCnab.id_registrada)
                    throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgPedidoBaixaSoCarteiraRegistrada), null,
                                                                 CnabBusinessException.TipoErro.ERRO_PEDIDO_BAIXA_CARTEIRA_SEM_REGISTRO, false);
                if(carteiraCnab.id_registrada && (cnab.id_tipo_cnab == (byte) Cnab.TipoCnab.GERAR_BOLETOS || cnab.id_tipo_cnab == (byte) Cnab.TipoCnab.PEDIDO_BAIXA)
                        && String.IsNullOrEmpty(cnab.no_arquivo_remessa)) {
                    LocalMovto localMovimento = BusinessFinanceiro.findCodigoClienteForCnab(cd_empresa, cnab.cd_local_movto);
                    cnab.no_arquivo_remessa = gerarNomeArquivoRemessa(cnab, localMovimento, cd_empresa);
                }
                ICollection<TituloCnab> titulosCnab = cnab.TitulosCnab;
                cnab.TitulosCnab = null;
                cnab = DaoCnab.add(cnab, false);
                cnab.cd_pessoa_empresa = cd_empresa;
                if (titulosCnab != null)
                    crudTitulosCnab(titulosCnab.ToList(), cnab);
                cnab.vl_total_cnab = DaoTituloCnab.somaValorTodosTitulosCnab(cd_empresa, cnab.cd_cnab);
                DaoCnab.saveChanges(false);
                transaction.Complete();
            }
            return DaoCnab.getCnabReturnGrade(cnab.cd_cnab, cd_empresa);
        }

        public Cnab editCnab(Cnab cnab, int cd_empresa)
        {
            DateTime dataCorrente = DateTime.Now.Date;
            if (cnab != null && ((cnab.dt_inicial_vencimento != null && DateTime.Compare((DateTime)cnab.dt_inicial_vencimento, new DateTime(1900, 1, 1)) < 0) ||
               (cnab.dt_final_vencimento != null && DateTime.Compare((DateTime)cnab.dt_final_vencimento, new DateTime(1900, 1, 1)) < 0)))
                throw new CnabBusinessException(Utils.Messages.Messages.msgErroMinDateCNAB, null, CnabBusinessException.TipoErro.ERRO_DATA_MIN_MAX_SMALLDATETIME, false);
            if (cnab != null && ((cnab.dt_inicial_vencimento != null && DateTime.Compare((DateTime)cnab.dt_inicial_vencimento, new DateTime(2079, 06, 06)) > 0) ||
                (cnab.dt_final_vencimento != null && DateTime.Compare((DateTime)cnab.dt_final_vencimento, new DateTime(2079, 06, 06)) > 0)))
                throw new CnabBusinessException(Utils.Messages.Messages.msgErroMaxDateCNAB, null, CnabBusinessException.TipoErro.ERRO_DATA_MIN_MAX_SMALLDATETIME, false);

            if (cnab.id_tipo_cnab == (int)Cnab.TipoCnab.GERAR_BOLETOS && DateTime.Compare(dataCorrente.Date, cnab.dt_inicial_vencimento.Date) > 0)
                throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgVenciIniMenorDataAtual), null,
                                                                  CnabBusinessException.TipoErro.ERRO_DATA_INI_MENOR_DATA_ATUAL, false);
            if (cnab.id_tipo_cnab == (int)Cnab.TipoCnab.GERAR_BOLETOS &&  DateTime.Compare(cnab.dt_inicial_vencimento, cnab.dt_final_vencimento) > 0)
                throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgVencFinalMenorDataIni), null,
                                                                  CnabBusinessException.TipoErro.ERRO_DATA_FINAL_MENOR_DATA_INICIAL, false);
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoCnab.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                LocalMovto localMovimento = new LocalMovto();
                this.sincronizarContextos(DaoCnab.DB());
                CarteiraCnab carteiraCnab = DaoCarteiraCnab.findById(cnab.cd_carteira_cnab, false);
                if(carteiraCnab.id_registrada && (cnab.id_tipo_cnab == (byte) Cnab.TipoCnab.GERAR_BOLETOS || cnab.id_tipo_cnab == (byte) Cnab.TipoCnab.PEDIDO_BAIXA)
                        && String.IsNullOrEmpty(cnab.no_arquivo_remessa)) {
                    localMovimento = BusinessFinanceiro.findLocalMovtoById(cd_empresa, cnab.cd_local_movto);
                    if (localMovimento.nm_banco != ((int)Cnab.Bancos.Sicred + ""))
                    {
                        cnab.no_arquivo_remessa = gerarNomeArquivoRemessa(cnab, localMovimento, cd_empresa);
                    }
                    
                }
                
                ICollection<TituloCnab> titulosCnab = cnab.TitulosCnab;
                cnab.TitulosCnab = null;
                Cnab cnabContext = DaoCnab.getCnabFull(cnab.cd_cnab, cd_empresa);
                if(cnabContext.id_status_cnab == (int)Cnab.StatusCnab.FECHADO)
                    throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgErroCnabFechadoNotAlterado), null,
                                                                  CnabBusinessException.TipoErro.ERRO_NAO_ALTERA_CNAB_GERADO, false);
                string old_no_arquivo_remessa = cnabContext.no_arquivo_remessa;
                cnabContext = Cnab.changeValuesCnab(cnab, cnabContext);

                //se for sicredi não deixa alterar o nome do arquivo
                localMovimento = BusinessFinanceiro.findLocalMovtoById(cd_empresa, cnab.cd_local_movto);
                if (localMovimento.nm_banco == ((int) Cnab.Bancos.Sicred + ""))
                {
                    cnabContext.no_arquivo_remessa = old_no_arquivo_remessa;
                }

                DaoCnab.saveChanges(false);
                cnab.cd_pessoa_empresa = cd_empresa;
                if (titulosCnab != null)
                    crudTitulosCnab(titulosCnab.ToList(), cnab);
                cnabContext.vl_total_cnab = DaoTituloCnab.somaValorTodosTitulosCnab(cd_empresa, cnab.cd_cnab);
                DaoCnab.saveChanges(false);
                transaction.Complete();
            }
            return DaoCnab.getCnabReturnGrade(cnab.cd_cnab, cd_empresa);
        }

        public bool deleteCnabs(int[] cdCnabs, int cd_empresa)
        {
            //BusinessFinanceiro.sincronizarContextos(DaoCnab.DB());
            bool retorno = false;
            SGFWebContext cdb = new SGFWebContext();
            if (cdCnabs != null && cdCnabs.Count() > 0)
            {
                List<Cnab> cnabsContext = DaoCnab.getCnabs(cdCnabs, cd_empresa).ToList();
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    foreach (var c in cnabsContext)
                    {
                        c.cd_pessoa_empresa = cd_empresa;
                        if(c.id_status_cnab == (int)Cnab.StatusCnab.FECHADO)
                            throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgNotExcludeCnabFechado), null,
                                                                   CnabBusinessException.TipoErro.ERRO_EXCLUIR_CNAB_FECHADO, false);
                        retorno = DaoCnab.delete(c, false);
                    }
                    transaction.Complete();
                }
            }
            return retorno;
        }

        public bool deleteCnabsRegistrados(Cnab cnab, int cd_empresa, bool masterGeral)
        {
            bool retorno = false;
            SGFWebContext cdb = new SGFWebContext();
            if (!masterGeral && (cnab.cd_contrato == null || cnab.cd_contrato == 0))
                throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgSemPemissaoDeletarNoCont), null,
                                                                   CnabBusinessException.TipoErro.ERRO_ALTERAR_EXCLUIR_CARTEIRA_HOMOLOGADA, false);
            if (cnab != null && cnab.cd_cnab > 0)
            {
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    List<Titulo> titulosCnab = DaoTituloCnab.existeTituloCnabComStatusDiferenteEviado(cnab.cd_cnab, cd_empresa).ToList();
                    if (cnab.cd_contrato == null)
                    {
                        if (titulosCnab != null && titulosCnab.Any(x => x.id_status_cnab != (int)Titulo.StatusCnabTitulo.ENVIADO_GERADO))
                            throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgErroDeleteCnabRegistrado), null,
                                                                           CnabBusinessException.TipoErro.ERRO_ALTERAR_EXCLUIR_CARTEIRA_HOMOLOGADA, false);
                        foreach (Titulo t in titulosCnab)
                            t.id_status_cnab = (int)Titulo.StatusCnabTitulo.INICIAL;
                    }
                    else {
                        foreach (Titulo t in titulosCnab)
                            t.id_cnab_contrato = false;
                    }
                    Cnab cnabContext = DaoCnab.findById(cnab.cd_cnab, false);
                    if (cnabContext.id_status_cnab == (int)Cnab.StatusCnab.ABERTO)
                        throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgNotExcludeCnabFechado), null,
                                                               CnabBusinessException.TipoErro.ERRO_EXCLUIR_CNAB_FECHADO, false);
                    retorno = DaoCnab.delete(cnabContext, false);
                    DaoTituloCnab.saveChanges(false);
                    transaction.Complete();
                }
            }
            return retorno;
        }

        public bool deleteRetornosCnabs(int[] cdCnabs, int cd_escola, string pathRetornosEscola)
        {
            //BusinessFinanceiro.sincronizarContextos(DaoCnab.DB());
            bool retorno = false;
            SGFWebContext cdb = new SGFWebContext();
            if (cdCnabs != null && cdCnabs.Count() > 0)
            {
                List<RetornoCNAB> cnabsContext = DaoRetornoCnab.getRetornosCnabs(cdCnabs, cd_escola).ToList();
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    foreach (var c in cnabsContext)
                    {
                        string documentoRetorno = "";

                        documentoRetorno = pathRetornosEscola + "/" + cd_escola + "/" + c.no_arquivo_retorno;
                        if (System.IO.File.Exists(documentoRetorno))
                            System.IO.File.Delete(documentoRetorno);
                    }
                    foreach (var c in cnabsContext)
                    {
                        c.cd_pessoa_empresa = cd_escola;
                        if (c.id_status_cnab == (int)Cnab.StatusCnab.FECHADO)
                            throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgNotExcludeRetornoCnabFechado), null,
                                                                   CnabBusinessException.TipoErro.ERRO_EXCLUIR_CNAB_FECHADO, false);
                        retorno = DaoRetornoCnab.delete(c, false);
                    }
                    transaction.Complete();
                }
            }
            return retorno;
        }

        public bool deleteCnabRetornosProcessados(int[] cdCnabs, int cd_escola, string pathRetornosEscola, bool isSupervisor, int movimentoRetroativo, bool masterGeral)
        {
            this.sincronizarContextos(DaoCnab.DB());

            bool retorno = false;
            SGFWebContext cdb = new SGFWebContext();

            if (!masterGeral)
                throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgSemPemissaoDeletarNoCont), null,
                                                                   CnabBusinessException.TipoErro.ERRO_PERMISSAO_EXCLUIR_CNAB_REGISTRADO, false);

            if (cdCnabs != null && cdCnabs.Count() > 0)
            {
                List<RetornoCNAB> cnabsContext = DaoRetornoCnab.getRetornosCnabs(cdCnabs, cd_escola).ToList();
                var tituloRetorno = DaoRetornoCnab.getTitulosRetornoCNAB(cdCnabs, cd_escola).ToList();

                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    deleteTransacaoFinanRetornosProcessados(tituloRetorno, cd_escola, isSupervisor, movimentoRetroativo);
                    foreach (var c in cnabsContext)
                    {
                        string documentoRetorno = "";

                        documentoRetorno = pathRetornosEscola + "/" + cd_escola + "/" + c.no_arquivo_retorno;
                        if (System.IO.File.Exists(documentoRetorno))
                            System.IO.File.Delete(documentoRetorno);
                    }
                    foreach (var c in cnabsContext)
                    {
                        c.cd_pessoa_empresa = cd_escola;
                        retorno = DaoRetornoCnab.delete(c, false);
                    }
                    transaction.Complete();
                }
            }
            return retorno;
        }

        private void deleteTransacaoFinanRetornosProcessados(ICollection<TituloRetornoCNAB> tituloRetorno, int cd_escola, bool isSupervisor, int movimentoRetroativo)
        {
            foreach (var tituloRet in tituloRetorno)
            {
                if (tituloRet.cd_tran_finan != null)
                {
                    var tranFinan = new TransacaoFinanceira
                    {
                        cd_tran_finan = (int)tituloRet.cd_tran_finan,
                        cd_pessoa_empresa = cd_escola,
                        isSupervisor = isSupervisor,
                        movimentoRetroativo = movimentoRetroativo
                    };
                    BusinessFinanceiro.deleteTransFinanBaixa(tranFinan);
                }
                DaoTituloRetornoCnab.delete(tituloRet, false);
            }
        }

        private void crudTitulosCnab(List<TituloCnab> titulosCnabView, Cnab cnab)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoCnab.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                this.sincronizarContextos(DaoCnab.DB());
                List<TituloCnab> titulosCnabContext = DaoTituloCnab.getAllTituloCnabByCnab(cnab.cd_pessoa_empresa, cnab.cd_cnab).ToList();
                IEnumerable<TituloCnab> titulosCnabComCodigo = titulosCnabView.Where(x => x.cd_titulo_cnab != 0);
                IEnumerable<TituloCnab> titulosCnabSemCodigo = titulosCnabView.Where(x => x.cd_titulo_cnab <= 0);
                IEnumerable<TituloCnab> titulosDeleted = titulosCnabContext.Where(tc => !titulosCnabComCodigo.Any(tv => tc.cd_titulo_cnab == tv.cd_titulo_cnab));
                //
                int[] cdTiulosCnabSemCodigo = null;
                if (titulosCnabSemCodigo != null && titulosCnabSemCodigo.Count() > 0)
                {
                    int i = 0;
                    int[] cdTiulosCnabSemCodigoCont = new int[titulosCnabSemCodigo.Count()];
                    foreach (var c in titulosCnabSemCodigo)
                    {
                        cdTiulosCnabSemCodigoCont[i] = c.cd_titulo;
                        i++;
                    }
                    cdTiulosCnabSemCodigo = cdTiulosCnabSemCodigoCont;
                }
                if (cdTiulosCnabSemCodigo != null && cdTiulosCnabSemCodigo.Count() > 0 && BusinessFinanceiro.verificarStatusCnabTitulo(cdTiulosCnabSemCodigo, cnab.cd_pessoa_empresa,cnab.cd_cnab, cnab.id_tipo_cnab))
                    throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgErrorTituloCnabExitente, '\u0022' + cnab.tipoCnab + '\u0022'), null,
                                                                  CnabBusinessException.TipoErro.ERRO_TITULO_STATUSCNAB_DIFERENTE_INCIAL, false);
                //remover o titulo e o plano conta correspondente.
                if (titulosDeleted != null && titulosDeleted.Count() > 0)
                    foreach (TituloCnab item in titulosDeleted)
                    {
                        //TO DO: Deivid = Implementar a regra de verificação de não deixar excluir títulos fechados (status cnab título).
                        DaoTituloCnab.deleteContext(item, false);
                    }
                foreach (TituloCnab item in titulosCnabView)
                {
                    // Novos horários da turma:
                    if (item.cd_titulo_cnab == 0)
                    {
                        item.cd_cnab = cnab.cd_cnab;
                        item.Titulo = null;
                        if (item.cd_turma_titulo == 0)
                            item.cd_turma_titulo = null;
                        if (item.tx_mensagem_cnab == null)
                            item.tx_mensagem_cnab = "";
                        DaoTituloCnab.addContext(item, false);
                    }
                    //Alteração dos horários da turma:
                    else
                    {
                        TituloCnab tCnabContext = titulosCnabContext.Where(tc => tc.cd_titulo_cnab == item.cd_titulo_cnab).FirstOrDefault();
                        tCnabContext.tx_mensagem_cnab = item.tx_mensagem_cnab;
                    }
                }
                DaoTituloCnab.saveChanges(false);
                DaoCnab.saveChanges(false);
                transaction.Complete();
            }
        }

        private string gerarNomeArquivoRemessa(Cnab cnab, LocalMovto localMovimento, int cdEmpresa) {
            

            string retorno = "";
            if ((localMovimento.Banco != null && localMovimento.Banco.nm_banco == ((int)Cnab.Bancos.Sicred + "")) || (localMovimento.nm_banco != null && localMovimento.nm_banco == ((int)Cnab.Bancos.Sicred + "")))
            {
                int nm_sequencia_remessa = 0;
                if (localMovimento.Banco != null && localMovimento.Banco.nm_banco == ((int) Cnab.Bancos.Sicred + ""))
                {
                     nm_sequencia_remessa = DaoCnab.getQtdCnabGeradoDia(localMovimento.Banco.nm_banco,
                        localMovimento.dc_num_cliente_banco, cdEmpresa);

                }
                else if ((localMovimento.nm_banco != null && localMovimento.nm_banco == ((int)Cnab.Bancos.Sicred + "")))
                {
                     nm_sequencia_remessa = DaoCnab.getQtdCnabGeradoDia(localMovimento.nm_banco,
                        localMovimento.dc_num_cliente_banco, cdEmpresa);
                }
                

                retorno = localMovimento.dc_num_cliente_banco;

                //regra Codificação dos meses sicred
                string mesDataAtual = DateTime.Now.Month.ToString();
                switch (mesDataAtual)
                {
                    case "10":
                        retorno += "O";
                    break;
                    case "11":
                        retorno += "N";
                    break;
                    case "12":
                        retorno += "D";
                    break;
                    default:
                        retorno += mesDataAtual;
                    break;
                }

                retorno += DateTime.Now.ToString("dd");
                
        
                if (nm_sequencia_remessa > 9)
                {
                    throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroNumeroMaximoArquivoRemessa, null, CnabBusinessException.TipoErro.ERRO_CAMPOS_BANCO_OBRIGATORIO, false);
                }
                switch (nm_sequencia_remessa)
                {
                    case 0:
                        retorno += ".CRM";
                        break;
                    case 9: retorno += ".RM0";
                        break;
                    default:
                        retorno += ".RM" + (nm_sequencia_remessa + 1);
                        break;
                }

            }else if ((localMovimento.Banco != null && localMovimento.Banco.nm_banco == Utils.Utils.FormatCode((int)Cnab.Bancos.Inter + "", 3)) || (localMovimento.nm_banco != null && localMovimento.nm_banco == Utils.Utils.FormatCode((int)Cnab.Bancos.Inter + "", 3)))
            {
                retorno = gerarNomeArquivoRemessaBancoInter(cnab, localMovimento, cdEmpresa);
            }
            else
            {
                retorno = "COB";

                retorno += localMovimento.Banco.nm_banco;
                retorno += localMovimento.dc_num_cliente_banco;
                if (localMovimento.Banco == null || String.IsNullOrEmpty(localMovimento.Banco.nm_banco))
                    throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroCnabSemNroBanco, null, CnabBusinessException.TipoErro.ERRO_CAMPOS_BANCO_OBRIGATORIO, false);
                if (String.IsNullOrEmpty(localMovimento.dc_num_cliente_banco))
                    throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroCamposBancoObrigatorios, null, CnabBusinessException.TipoErro.ERRO_CAMPOS_BANCO_OBRIGATORIO, false);
                if (cnab.id_tipo_cnab == (byte)Cnab.TipoCnab.PEDIDO_BAIXA)
                    retorno += "BX";
                retorno += DateTime.Now.ToString("ddMMyyHHmmss");
                retorno += ".rem";
            }




            return retorno;
        }

        public string gerarNomeArquivoRemessaBancoInter(Cnab cnab, LocalMovto localMovimento, int cdEmpresa)
        {

            string retorno = "CI400_001_";
            int sequencia = cnab.nm_sequencia_remessa == null ? localMovimento.nm_sequencia > 0 ? localMovimento.nm_sequencia + 1 : 
                (int)cnab.nm_sequencia_remessa > 0 ? (int)cnab.nm_sequencia_remessa : 1 : 1;
            retorno += Utils.Utils.FormatCode(sequencia.ToString() , 7);
            retorno += ".REM";

            return retorno;

        }

        public TituloCnab getTituloCnabEditView(int cd_cnab, int cd_titulo_cnab, int cdEmpresa)
        {
            TituloCnab retorno = new TituloCnab();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DaoTituloCnab.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = DaoTituloCnab.getTituloCnabEditView(cd_cnab, cd_titulo_cnab, cdEmpresa);
                transaction.Complete();
            }
            return retorno;

        }
        #endregion

        #region Carteira CNAB
        public IEnumerable<CarteiraCnab> getCarteiraCnabSearch(SearchParameters parametros, string nome, bool inicio, int banco, bool? status)
        {
            IEnumerable<CarteiraCnab> retorno = new List<CarteiraCnab>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_carteira";
                parametros.sort = parametros.sort.Replace("carteira_ativa", "id_carteira_ativa");
                parametros.sort = parametros.sort.Replace("dc_registro", "id_registrada");
                parametros.sort = parametros.sort.Replace("bancoCarteira", "Banco.no_banco");
                IEnumerable<CarteiraCnab> carteira = DaoCarteiraCnab.getCarteiraCnabSearch(parametros, nome, inicio, banco, status);
                retorno = carteira;
                transaction.Complete();
            }
            return retorno;
        }
        public CarteiraCnab postInsertCarteira(CarteiraCnab carteiraCnab)
        {
            return DaoCarteiraCnab.add(carteiraCnab, false);
        }
        public CarteiraCnab putCarteira(CarteiraCnab carteiraCnab)
        {
            CarteiraCnab carteiraContext = DaoCarteiraCnab.findById(carteiraCnab.cd_carteira_cnab, false);
            if (carteiraContext.id_homologado)
                throw new CnabBusinessException(string.Format(Componentes.Utils.Messages.Messages.msgErroRegProp), null, CnabBusinessException.TipoErro.ERRO_ALTERAR_EXCLUIR_CARTEIRA_HOMOLOGADA, false);
            carteiraContext.copy(carteiraCnab);
            DaoCarteiraCnab.saveChanges(false);
            return carteiraContext;
        }
        public bool deleteAllCarteira(List<CarteiraCnab> carteiras)
        {
            bool retorno = true;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (CarteiraCnab b in carteiras)
                {
                    CarteiraCnab carteiraDel = DaoCarteiraCnab.findById(b.cd_carteira_cnab, false);
                    if (carteiraDel.id_homologado)
                        throw new CnabBusinessException(string.Format(Componentes.Utils.Messages.Messages.msgErroRegProp), null, CnabBusinessException.TipoErro.ERRO_ALTERAR_EXCLUIR_CARTEIRA_HOMOLOGADA, false);
                    retorno = DaoCarteiraCnab.delete(carteiraDel, false);
                }
                transaction.Complete();
            }
            return retorno;
        }
        public IEnumerable<CarteiraCnab> getCarteiraByBanco(int? localMovto, int banco)
        {
            return DaoCarteiraCnab.getCarteiraByBanco(localMovto, banco);
        }

        public IEnumerable<CarteiraCnab> getCarteirasCnab(int cdEscola, int cd_carteira_cnab, CarteiraCnabDataAccess.TipoConsultaCarteiraCnab tipoConsulta)
        {
            return DaoCarteiraCnab.getCarteirasCnab(cdEscola, cd_carteira_cnab, tipoConsulta);
        }
        #endregion

        #region RetornoCnab

        public TituloRetornoCNAB getTituloRetornoCnabEditView(int cd_titulo_cnab, int cd_empresa) {
            return DaoTituloRetornoCnab.getTituloRetornoCnabEditView(cd_titulo_cnab, cd_empresa);
        }

        public int buscarTipoCNAB(int cd_retorno_cnab, int cd_pessoa_empresa) {
            return DaoRetornoCnab.buscarTipoCNAB(cd_retorno_cnab, cd_pessoa_empresa);
        }

        public IEnumerable<RetornoCNAB> searchRetornoCNAB(SearchParameters parametros, int cd_carteira, int cd_usuario, int status, string descRetorno,  DateTime? dtInicial,
                                                          DateTime? dtFinal, string nossoNumero, int cd_empresa, int cd_responsavel, int cd_aluno)
        {
            IEnumerable<RetornoCNAB> retorno = new List<RetornoCNAB>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dt_cadastro_cnab";
                parametros.sort = parametros.sort.Replace("carteira_retorno_cnab", "LocalMovto.CarteiraCnab.no_carteira");
                parametros.sort = parametros.sort.Replace("tipoRetornoCNAB", "id_tipo_cnab");
                parametros.sort = parametros.sort.Replace("usuarioRetornoCNAB", "SysUsuario.no_login");
                parametros.sort = parametros.sort.Replace("dta_cadastro_cnab", "dt_cadastro_cnab");
                parametros.sort = parametros.sort.Replace("statusRetornoCNAB", "id_status_cnab");
                retorno = DaoRetornoCnab.searchRetornoCNAB(parametros, cd_carteira, cd_usuario, status, descRetorno, dtInicial, dtFinal, nossoNumero, cd_empresa, cd_responsavel, cd_aluno);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<UsuarioWebSGF> getUsuariosRetCNAB(int cd_empresa)
        {
            return DaoRetornoCnab.getUsuariosRetCNAB(cd_empresa);
        }

        public IEnumerable<CarteiraCnab> getCarteirasRetCNAB(int cd_empresa)
        {
            return DaoRetornoCnab.getCarteirasRetCNAB(cd_empresa);
        }

        public RetornoCNAB addRetornoCNAB(RetornoCNAB retornoCNAB, string pathRetornosEscola, int cd_empresa, bool masterGeral)
        {
            sincronizarContextos(DaoRetornoCnab.DB());
            string documentoRetornoTemp = "";
            string documentoRetorno = "";
            try
            {
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    retornoCNAB = DaoRetornoCnab.add(retornoCNAB, false);
                    if (!String.IsNullOrEmpty(retornoCNAB.retornoTemporario))
                    {
                        //documentoRetorno = pathRetornosEscola + "/" + cd_empresa + "/" + retornoCNAB.no_arquivo_retorno;
                        documentoRetorno = Path.Combine(pathRetornosEscola, cd_empresa.ToString(), retornoCNAB.no_arquivo_retorno);

                        //documentoRetornoTemp = pathRetornosEscola + "/" + cd_empresa + "/" + retornoCNAB.retornoTemporario;
                        documentoRetornoTemp = Path.Combine(pathRetornosEscola, cd_empresa.ToString(), retornoCNAB.retornoTemporario);

                        //string pathRetornosEscola = caminho_relatorios + "/Contratos/" + cdEscola;
                        if (System.IO.File.Exists(documentoRetorno))
                            throw new CnabBusinessException(Utils.Messages.Messages.msgErroNomeRetExistente, null, CnabBusinessException.TipoErro.ERRO_RETORNO_JA_EXISTE, false);
                        //System.IO.File.Delete(documentoRetorno);
                        System.IO.File.Move(documentoRetornoTemp, documentoRetorno);
                        if (System.IO.File.Exists(documentoRetornoTemp))
                            System.IO.File.Delete(documentoRetornoTemp);
                    }
                    transaction.Complete();
                }
                return DaoRetornoCnab.getRetornoCnabReturnGrade(retornoCNAB.cd_retorno_cnab, cd_empresa);
            }
            catch (ArgumentException ex) // Capturar erro System.ArgumentException: Illegal characters in path. (Erro 5393) 
            {
                throw new ArgumentException(string.Format("Documento Retorno: {0} - Documento Retorno Temp: {1} - Erro: {2}", documentoRetorno, documentoRetornoTemp, ex.ToString()));
            }
            catch (Exception)
            {
                if (masterGeral)
                    //documentoRetornoTemp = pathRetornosEscola + "/" + retornoCNAB.retornoTemporario;
                    documentoRetornoTemp = Path.Combine(pathRetornosEscola, retornoCNAB.retornoTemporario);
                else
                    //documentoRetornoTemp = pathRetornosEscola + "/" + retornoCNAB.cd_pessoa_empresa + "/" + retornoCNAB.retornoTemporario;
                    documentoRetornoTemp = Path.Combine(pathRetornosEscola, retornoCNAB.cd_pessoa_empresa.ToString(), retornoCNAB.retornoTemporario);
                if (System.IO.File.Exists(documentoRetornoTemp))
                    System.IO.File.Delete(documentoRetornoTemp);

                throw;
            }
        }
        public RetornoCNAB getRetornoCnabFull(int cd_retorno, int cd_escola) {
            return DaoRetornoCnab.getRetornoCnabFull(cd_retorno, cd_escola); 
        }
        public RetornoCNAB getRetCnabEditView(int cd_cnab, int cdEscola)
        {
            return DaoRetornoCnab.getRetornoCnabEditView(cd_cnab, cdEscola);
        }

        public void postProcessarRetornos(RetornoCNAB retornoCNAB)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED,DaoRetornoCnab.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                BusinessFinanceiro.sincronizarContextos(DaoRetornoCnab.DB());
                //Se tentar processar um retorno já processado:
                RetornoCNAB retornoCnabContext = DaoRetornoCnab.findById(retornoCNAB.cd_retorno_cnab, false);
                
                if(retornoCnabContext.id_status_cnab == (int) RetornoCNAB.StatusRetornoCNAB.FECHADO)
                    throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgRetornoCNABGerado, null, CnabBusinessException.TipoErro.ERRO_RETORNO_JA_FECHADO, false);

                //Se o Banco que estiver no arquivo de retorno não for o mesmo da carteira escolhida:
                if(!retornoCNAB.LocalMovto.nm_banco.Equals(retornoCNAB.LocalMovto.nm_banco))
                    throw new CnabBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroCorrespondenciaBanco, null, CnabBusinessException.TipoErro.ERRO_CORRESPONDENCIA_BANCO, false);

                retornoCNAB.nm_titulos_gerados = 0;
                if (retornoCNAB.LocalMovto.nm_banco == Utils.Utils.FormatCode((int)Cnab.Bancos.Inter + "", 3))
                {
                    atualizaNossoNumeroTitulos(retornoCNAB);
                }
                atualizarTitulosEBaixas(retornoCNAB);
                retornoCnabContext.TitulosRetornoCNAB = retornoCNAB.TitulosRetornoCNAB;
                //salva com os titulos retorno para a trigger poder atuar e atualizar os titulos - 939 - Chamado: 291530 
                DaoRetornoCnab.saveChanges(false);

                //busca o retorno cnab para atualizar os status para 2 (a trigger age neste momento nos titulos)
                retornoCnabContext = DaoRetornoCnab.findById(retornoCNAB.cd_retorno_cnab, false);
                retornoCNAB.id_status_cnab = retornoCnabContext.id_status_cnab = (int) RetornoCNAB.StatusRetornoCNAB.FECHADO;
                retornoCnabContext.nm_linhas_retorno = retornoCNAB.nm_linhas_retorno;
                retornoCnabContext.nm_titulos_gerados = retornoCNAB.nm_titulos_gerados;

                //salvarDespesasCnab(retornoCNAB.DespesasTituloCnab);

                DaoRetornoCnab.saveChanges(false);
                transaction.Complete();
            }
        }

        private void atualizaNossoNumeroTitulos(RetornoCNAB retornoCnab)
        {
            foreach (TituloRetornoCNAB tituloRetornoCnab in retornoCnab.TitulosRetornoCNAB.ToList())
            {
                BusinessFinanceiro.updateNossoNumeroTitulo(tituloRetornoCnab.cd_titulo_retorno_cnab, retornoCnab.cd_pessoa_empresa, retornoCnab.LocalMovto.cd_local_movto, tituloRetornoCnab.dc_nosso_numero);
            }
        }

        private void salvarDespesasCnab(List<DespesaTituloCnab> despesas)
        {
            foreach (var despesa in despesas)
            {
                DaoDespesaTituloCnab.add(despesa, false);
            }
        }

        private void atualizarTitulosEBaixas(RetornoCNAB retornoCNAB) {
            List<TransacaoFinanceira> listaTransacoes = new List<TransacaoFinanceira>();
            List<Titulo> titulos = BusinessFinanceiro.getTitulosBaixaFinan(retornoCNAB.TitulosRetornoCNAB.Select(x => x.dc_nosso_numero).ToList(), retornoCNAB.cd_pessoa_empresa, retornoCNAB.LocalMovto.cd_local_movto);
            foreach (TituloRetornoCNAB tituloRetornoCNAB in retornoCNAB.TitulosRetornoCNAB.ToList())
            {
                
                
                if(!string.IsNullOrEmpty(tituloRetornoCNAB.dc_nosso_numero) && titulos.Count > 0)
                {
                    //titulo = BusinessFinanceiro.getTituloBaixaFinan(tituloRetornoCNAB.dc_nosso_numero, retornoCNAB.cd_pessoa_empresa, retornoCNAB.LocalMovto.cd_local_movto);
                    Titulo titulo = titulos.Where(x => x.dc_nosso_numero == (long.Parse(tituloRetornoCNAB.dc_nosso_numero) + "")).FirstOrDefault();
                    
                    if (titulo != null)
                    {
                        retornoCNAB.nm_titulos_gerados += 1;
                        tituloRetornoCNAB.cd_titulo = titulo.cd_titulo;
                        switch (tituloRetornoCNAB.id_tipo_retorno)
                        {
                            case (byte)TituloRetornoCNAB.TipoRetornoCNAB.GERAR_BAIXA:
                                TransacaoFinanceira transacao = new TransacaoFinanceira();
                                BaixaTitulo baixa = new BaixaTitulo();
                                List<BaixaTitulo> listaBaixas = new List<BaixaTitulo>();

                                bool baixaTituloNormal = false;
                                decimal vl_titulo_saldo = titulo.vl_titulo;
                                //Se o título já tiver sido baixado deverá ser gravado como tipo retorno “Erro Titulo” e a observação a ser gerada será “Título nosso número xxxxxxxxx já foi baixado ZZZZZZZZ”. 
                                //Se o staus cnab estiver 1 ou 3 ZZZZZZZ deverá ser substituído por “por outro retorno” caso contrário deverá ser substituído por “manualmente”. 
                                if (Math.Round(titulo.vl_titulo, 2) != Math.Round(titulo.vl_saldo_titulo, 2))
                                    baixaTituloNormal = true;
                                //Caso tenha baixa do tipo "Motivo bolsa", liberar a regra de liquidação normalmente.
                                //Usar o valor do saldo no lugar do valor do título.
                                if (baixaTituloNormal && !BusinessFinanceiro.verificaTituloOrContratoBaixaEfetuada((int)titulo.cd_origem_titulo, titulo.cd_pessoa_empresa, titulo.cd_titulo))
                                {
                                    if (titulo.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO)
                                        baixaTituloNormal = false;
                                    vl_titulo_saldo = titulo.vl_saldo_titulo;
                                    //    baixaTituloNormal = true; 
                                }
                                if (baixaTituloNormal)
                                {
                                    tituloRetornoCNAB.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                                    tituloRetornoCNAB.tx_mensagem_retorno += " Título de nosso número " + titulo.dc_nosso_numero + " já foi baixado ";
                                    if (titulo.id_status_cnab == (byte)Titulo.StatusCnabTitulo.ENVIADO_GERADO || titulo.id_status_cnab == (byte)Titulo.StatusCnabTitulo.CONFIRMADO_ENVIO)
                                        tituloRetornoCNAB.tx_mensagem_retorno += "por outro retorno.";
                                    else
                                        tituloRetornoCNAB.tx_mensagem_retorno += "manualmente.";
                                }
                                //Se o valor for maior joga a diferença nos juros
                                else if ((tituloRetornoCNAB.vl_baixa_retorno - tituloRetornoCNAB.vl_juros_retorno - tituloRetornoCNAB.vl_multa_retorno) > vl_titulo_saldo)
                                {
                                    //caso tenha mais de um titulo com o mesmo nosso numero para realizar  baixa, seta erro no titulo e coloca a mensagem de erro
                                    bool hastransacaoTitulo = listaTransacoes.Where(x => x.Baixas.Where(b => b.cd_titulo == titulo.cd_titulo).Any()).Any();
                                    if (hastransacaoTitulo)
                                    {
                                        tituloRetornoCNAB.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                                        tituloRetornoCNAB.tx_mensagem_retorno += String.Format(Utils.Messages.Messages.msgErroBaixaDuplicadaArquivoRetorno, titulo.cd_titulo, titulo.dc_nosso_numero);

                                    }
                                    else
                                    {
                                        transacao.dt_tran_finan = tituloRetornoCNAB.dt_banco_retorno;
                                        transacao.cd_pessoa_empresa = retornoCNAB.cd_pessoa_empresa;
                                        transacao.cd_local_movto = retornoCNAB.LocalMovto.cd_local_movto;
                                        transacao.cd_tipo_liquidacao = (int)TipoLiquidacao.TipoLiqui.LIQUIDACAO_BANCARIA;
                                        titulo.nomeResponsavel = BusinessFinanceiro.getResponsavelTitulo(titulo.cd_titulo, retornoCNAB.cd_pessoa_empresa);

                                        baixa.cd_titulo = titulo.cd_titulo;
                                        baixa.dt_baixa_titulo = transacao.dt_tran_finan.Value;
                                        if (tituloRetornoCNAB.vl_desconto_titulo + tituloRetornoCNAB.vl_baixa_retorno < vl_titulo_saldo)
                                            baixa.id_baixa_parcial = true;
                                        else
                                            baixa.id_baixa_parcial = false;
                                        baixa.vl_principal_baixa = vl_titulo_saldo;

                                        //joga nos juros a diferença da baixa e do titulo
                                        tituloRetornoCNAB.vl_juros_retorno = (tituloRetornoCNAB.vl_baixa_retorno - tituloRetornoCNAB.vl_multa_retorno - vl_titulo_saldo);
                                        tituloRetornoCNAB.tx_mensagem_retorno += String.Format("Valor pago a mais");

                                        baixa.vl_desconto_baixa = tituloRetornoCNAB.vl_desconto_titulo;

                                        baixa.vl_liquidacao_baixa = tituloRetornoCNAB.vl_baixa_retorno;
                                        baixa.vl_juros_baixa = tituloRetornoCNAB.vl_juros_retorno;
                                        baixa.vl_multa_baixa = tituloRetornoCNAB.vl_multa_retorno;
                                        baixa.tx_obs_baixa = FundacaoFisk.SGF.Utils.Messages.Messages.msgBaixaAutomaticaCNAB;
                                        baixa.Titulo = titulo;



                                        TransacaoFinanceira transacaoExistente = listaTransacoes.Where(t => t.Baixas.Where(b => b.dt_baixa_titulo.CompareTo(baixa.dt_baixa_titulo) == 0).Any()).FirstOrDefault();

                                        //Se não existe transação com a data dessa baixa, cria a nova transação:
                                        if (transacaoExistente != null)
                                        {
                                            List<BaixaTitulo> listaExistente = transacaoExistente.Baixas.ToList();
                                            listaExistente.Add(baixa);
                                            transacaoExistente.Baixas = listaExistente;
                                        }
                                        else
                                        { //Caso contrário, cria a nova transação:
                                            listaBaixas.Add(baixa);
                                            transacao.Baixas = listaBaixas;
                                            listaTransacoes.Add(transacao);
                                        }
                                    }


                                }
                                else
                                {
                                    //caso tenha mais de um titulo com o mesmo nosso numero para realizar  baixa, seta erro no titulo e coloca a mensagem de erro
                                    bool hastransacaoTitulo = listaTransacoes.Where(x => x.Baixas.Where(b => b.cd_titulo == titulo.cd_titulo).Any()).Any();
                                    if (hastransacaoTitulo)
                                    {
                                        tituloRetornoCNAB.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                                        tituloRetornoCNAB.tx_mensagem_retorno += String.Format(Utils.Messages.Messages.msgErroBaixaDuplicadaArquivoRetorno, titulo.cd_titulo, titulo.dc_nosso_numero);

                                    }
                                    else
                                    {
                                        //Deve ser gerada uma transação financeira para cada data de baixa efetuada e todas as baixas filhas desta transação.
                                        //Criar uma baixa. Serão informadas a data da baixa, valor da baixa, juros, multa e descontos se houverem. 
                                        //Se o Valor da Baixa do Retorno for menor que o valor do título(calculado com os devidos descontos) uma baixa parcial deverá ser gerada. 
                                        transacao.dt_tran_finan = tituloRetornoCNAB.dt_banco_retorno;
                                        transacao.cd_pessoa_empresa = retornoCNAB.cd_pessoa_empresa;
                                        transacao.cd_local_movto = retornoCNAB.LocalMovto.cd_local_movto;
                                        transacao.cd_tipo_liquidacao = (int)TipoLiquidacao.TipoLiqui.LIQUIDACAO_BANCARIA;
                                        titulo.nomeResponsavel = BusinessFinanceiro.getResponsavelTitulo(titulo.cd_titulo, retornoCNAB.cd_pessoa_empresa);

                                        baixa.cd_titulo = titulo.cd_titulo;
                                        baixa.dt_baixa_titulo = transacao.dt_tran_finan.Value;
                                        if (tituloRetornoCNAB.vl_desconto_titulo + tituloRetornoCNAB.vl_baixa_retorno < vl_titulo_saldo)
                                            baixa.id_baixa_parcial = true;
                                        else
                                            baixa.id_baixa_parcial = false;
                                        baixa.vl_principal_baixa = vl_titulo_saldo;

                                        //Caso for pagamento pelo InternetBank ou BankLine:
                                        if (tituloRetornoCNAB.vl_baixa_retorno > vl_titulo_saldo && tituloRetornoCNAB.vl_juros_retorno == 0 && tituloRetornoCNAB.vl_multa_retorno == 0)
                                            tituloRetornoCNAB.vl_juros_retorno = tituloRetornoCNAB.vl_baixa_retorno - vl_titulo_saldo;

                                        //Caso não venha o valor de desconto e o valor pago é menor que o valor do título:
                                        //Caso valor pago é menor que o do título e o desconto vem descriminado, mas a soma dos dois é menor que o valor do título:
                                        if (baixa.id_baixa_parcial)
                                            baixa.vl_desconto_baixa = baixa.vl_principal_baixa - tituloRetornoCNAB.vl_baixa_retorno; // O valor pode ser substituído uma vez que o desconto será desconsiderado na baixa parcial
                                        else
                                            baixa.vl_desconto_baixa = tituloRetornoCNAB.vl_desconto_titulo;

                                        baixa.vl_liquidacao_baixa = tituloRetornoCNAB.vl_baixa_retorno;
                                        baixa.vl_juros_baixa = tituloRetornoCNAB.vl_juros_retorno;
                                        baixa.vl_multa_baixa = tituloRetornoCNAB.vl_multa_retorno;
                                        baixa.tx_obs_baixa = FundacaoFisk.SGF.Utils.Messages.Messages.msgBaixaAutomaticaCNAB;
                                        baixa.Titulo = titulo;


                                        TransacaoFinanceira transacaoExistente = listaTransacoes.Where(t => t.Baixas.Where(b => b.dt_baixa_titulo.CompareTo(baixa.dt_baixa_titulo) == 0).Any()).FirstOrDefault();

                                        //Se não existe transação com a data dessa baixa, cria a nova transação:
                                        if (transacaoExistente != null)
                                        {
                                            List<BaixaTitulo> listaExistente = transacaoExistente.Baixas.ToList();
                                            listaExistente.Add(baixa);
                                            transacaoExistente.Baixas = listaExistente;
                                        }
                                        else
                                        { //Caso contrário, cria a nova transação:
                                            listaBaixas.Add(baixa);
                                            transacao.Baixas = listaBaixas;
                                            listaTransacoes.Add(transacao);
                                        }
                                    }
                                }
                                break;
                            case (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_TITULO:
                                //Quando o status do título for 1 vai alterar o status cnab dos títulos para 3
                                if (titulo.id_status_cnab == (byte)Titulo.StatusCnabTitulo.ENVIADO_GERADO)
                                {
                                    tituloRetornoCNAB.id_status_cnab_retorno = (byte)Titulo.StatusCnabTitulo.CONFIRMADO_ENVIO;
                                    /* Atualiza titulo a titulos - vai ser substituido por procedure
                                    Titulo tituloBd = TituloDataAccess.findById(titulo.cd_titulo, false);
                                    tituloBd.id_status_cnab = (byte)Titulo.StatusCnabTitulo.CONFIRMADO_ENVIO;
                                    titulo.id_status_cnab = (byte)Titulo.StatusCnabTitulo.CONFIRMADO_ENVIO;
                                    TituloDataAccess.saveChanges(false);
                                    //tituloRetornoCNAB.Titulo = titulo;*/
                                }

                                break;
                            case (byte)TituloRetornoCNAB.TipoRetornoCNAB.CONFIRMAR_PEDIDO_BAIXA:
                                //Alterar o status CNAB do título para 6 se o status cnab dos títulos estiver com valor (1 ou 3 ou 5) ou alterar para 4 se o status anterior for 2
                                if (titulo.id_status_cnab == (byte)Titulo.StatusCnabTitulo.ENVIADO_GERADO || titulo.id_status_cnab == (byte)Titulo.StatusCnabTitulo.CONFIRMADO_ENVIO ||
                                titulo.id_status_cnab == (byte)Titulo.StatusCnabTitulo.PEDIDO_BAIXA)
                                {

                                    tituloRetornoCNAB.id_status_cnab_retorno = (byte) Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA;
                                    /* Atualiza titulo a titulos - vai ser substituido por procedure
                                    Titulo tituloBd = TituloDataAccess.findById(titulo.cd_titulo, false);
                                    tituloBd.id_status_cnab = (byte)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA;
                                    titulo.id_status_cnab = (byte)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA;
                                    TituloDataAccess.saveChanges(false);
                                    //tituloRetornoCNAB.Titulo = titulo;*/
                                }

                                if (titulo.id_status_cnab == (byte)Titulo.StatusCnabTitulo.BAIXA_MANUAL)
                                {
                                    tituloRetornoCNAB.id_status_cnab_retorno = (byte)Titulo.StatusCnabTitulo.BAIXA_MANUAL_CONFIRMADO;

                                    /* Atualiza titulo a titulos - vai ser substituido por procedure
                                    Titulo tituloBd = TituloDataAccess.findById(titulo.cd_titulo, false);
                                    tituloBd.id_status_cnab = (byte)Titulo.StatusCnabTitulo.BAIXA_MANUAL_CONFIRMADO;
                                    titulo.id_status_cnab = (byte)Titulo.StatusCnabTitulo.BAIXA_MANUAL_CONFIRMADO;
                                    TituloDataAccess.saveChanges(false);
                                    //tituloRetornoCNAB.Titulo = titulo;*/
                                }

                                break;
                            case (byte)TituloRetornoCNAB.TipoRetornoCNAB.RETORNO_PROTESTO:
                                //Nada é feito, conforme solicitado no requisito.
                                break;
                        }
                    }
                    else
                    {

                        tituloRetornoCNAB.cd_titulo = null;
                        tituloRetornoCNAB.tx_mensagem_retorno += string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroTituloNaoEncot, tituloRetornoCNAB.dc_nosso_numero, tituloRetornoCNAB.tipoRetornoCNAB);
                        tituloRetornoCNAB.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                    }


                }
                else
                {

                    tituloRetornoCNAB.cd_titulo = null;
                    tituloRetornoCNAB.tx_mensagem_retorno += string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroTituloNaoEncot, tituloRetornoCNAB.dc_nosso_numero, tituloRetornoCNAB.tipoRetornoCNAB);
                    tituloRetornoCNAB.id_tipo_retorno = (byte)TituloRetornoCNAB.TipoRetornoCNAB.ERRO_TITULO;
                }
            }

            //Salva as transções:
            foreach (TransacaoFinanceira transacao in listaTransacoes)
            {
                TransacaoFinanceira trans = BusinessFinanceiro.postIncluirTransacao(transacao, false, true);
                if(retornoCNAB.TitulosRetornoCNAB != null && retornoCNAB.TitulosRetornoCNAB.Count() > 0 &&
                    transacao != null && transacao.Baixas != null && transacao.Baixas.Count() > 0)
                    foreach (TituloRetornoCNAB t in retornoCNAB.TitulosRetornoCNAB.ToList())
                        foreach (BaixaTitulo b in transacao.Baixas)
                            if (/*t.Titulo != null &&*/ t.cd_titulo == b.cd_titulo)
                                t.cd_tran_finan = trans.cd_tran_finan;
            }
        }

        public RetornoCNAB editRetornoCNAB(RetornoCNAB retornoCNAB, string pathContratosEscola, int cd_empresa)
        {
            DateTime dataCorrente = DateTime.Now.Date;

            BusinessFinanceiro.sincronizarContextos(DaoCnab.DB());
            DaoCarteiraCnab.sincronizaContexto(DaoCnab.DB());
            string documentoRetTemp = "";
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                RetornoCNAB retCNABCopy = new RetornoCNAB();
                RetornoCNAB retCNABContext = DaoRetornoCnab.getRetornoCnabFull(retornoCNAB.cd_retorno_cnab, cd_empresa);
                retCNABCopy.copy(retCNABContext);
                if (retCNABContext.id_status_cnab == (int)Cnab.StatusCnab.FECHADO)
                    throw new CnabBusinessException(string.Format(Utils.Messages.Messages.msgErroAlterarRetorno), null,
                                                                  CnabBusinessException.TipoErro.ERRO_RETORNO_JA_FECHADO, false);
                retCNABContext = RetornoCNAB.changeValuesRetCnab(retornoCNAB, retCNABContext);
                retornoCNAB.cd_pessoa_empresa = cd_empresa;
                DaoRetornoCnab.saveChanges(false);

                if (!String.IsNullOrEmpty(retornoCNAB.retornoTemporario))
                {
                    string documentoRet = "";
                    string pathDocAntigo = "";
                    pathDocAntigo = pathContratosEscola + "/" + retornoCNAB.cd_pessoa_empresa + "/" + retCNABCopy.no_arquivo_retorno;
                    documentoRet = pathContratosEscola + "/" + retornoCNAB.cd_pessoa_empresa + "/" + retornoCNAB.no_arquivo_retorno;
                    documentoRetTemp = pathContratosEscola + "/" + retornoCNAB.cd_pessoa_empresa + "/" + retornoCNAB.retornoTemporario;
                    //string pathContratosEscola = caminho_relatorios + "/Contratos/" + cdEscola;
                    if (System.IO.File.Exists(documentoRet))
                    {
                        if (retCNABCopy.no_arquivo_retorno != retCNABContext.no_arquivo_retorno)
                            throw new CnabBusinessException(Utils.Messages.Messages.msgErroNomeRetExistente, null, CnabBusinessException.TipoErro.ERRO_RETORNO_JA_EXISTE, false);
                        else
                            if (System.IO.File.Exists(pathDocAntigo))
                                System.IO.File.Delete(pathDocAntigo);
                    }
                    else
                        if (System.IO.File.Exists(pathDocAntigo))
                            System.IO.File.Delete(documentoRet);
                    if (System.IO.File.Exists(pathDocAntigo) && (retCNABCopy.no_arquivo_retorno != retornoCNAB.no_arquivo_retorno))
                        System.IO.File.Delete(pathDocAntigo);

                    System.IO.File.Move(documentoRetTemp, documentoRet);
                    if (System.IO.File.Exists(documentoRetTemp))
                        System.IO.File.Delete(documentoRetTemp);
                }

                transaction.Complete();
            }
            return DaoRetornoCnab.getRetornoCnabReturnGrade(retornoCNAB.cd_retorno_cnab, cd_empresa);
        }
        
        public List<TituloRetornoCNAB> searchTituloCnabGradeRet(TituloUI titulo)
        {
            return DaoTituloRetornoCnab.searchTituloCnabGradeRet(titulo);
                 
        }

        public List<TituloRetornoCNAB> getTituloRetornoCNAB(int cd_retorno_cnab)
        {
            List<TituloRetornoCNAB> retorno = new List<TituloRetornoCNAB>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DaoTituloRetornoCnab.getTituloRetornoCNAB(cd_retorno_cnab);
                transaction.Complete();
            }
            return retorno;
        }

        public List<TituloCnab> getTituloCNAB(int cd_cnab)
        {
            List<TituloCnab> retorno = new List<TituloCnab>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DaoTituloRetornoCnab.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = DaoTituloRetornoCnab.getTituloCNAB(cd_cnab);
                transaction.Complete();
            }
            return retorno;
        }

        public TransacaoFinanceira getTransacaoFinanceiraCNAB(int cd_tran_finan, int cd_pessoa_empresa)
        {
            return BusinessFinanceiro.getTransacaoFinanceira(cd_tran_finan, cd_pessoa_empresa);
        }
        #endregion

        #region Local de Movimento

        public LocalMovto findLocalMovtoComCarteira(int cdEscola, int cdLocalMovto)
        {
            return BusinessFinanceiro.findLocalMovtoComCarteira(cdEscola, cdLocalMovto);
        }

        #endregion

    }
}
