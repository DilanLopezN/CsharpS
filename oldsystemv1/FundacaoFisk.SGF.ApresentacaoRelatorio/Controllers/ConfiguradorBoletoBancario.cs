using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BoletoNet;
using System.Web.Mvc;
using Componentes.GenericController;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.CNAB.Business;
using FundacaoFisk.SGF.Web.Services.CNAB.DataAccess;

namespace FundacaoFisk.SGF.ApresentacaoRelatorio.Controllers {
    public enum Bancos {
        BancodoBrasil = 1,
        Banrisul = 41,
        Basa = 3,
        Bradesco = 237,
        DinariPay = 238,
        BRB = 70,
        Caixa = 104,
        HSBC = 399,
        Itau = 341,
        Inter = 077,
        Real = 356,
        Safra = 422,
        Santander = 33,
        Sicoob = 756,
        Sicred = 748,
        Sudameris = 347,
        Unibanco = 409,
        CrediSIS = 97,
        Unicred = 136,
        Uniprime = 84,
    }
    
    public class ConfiguradorBoletoBancario {
        public ConfiguradorBoletoBancario(int Banco)
        {
            boletoBancario = new BoletoBancario();
            boletoBancario.CodigoBanco = (short)Banco;
        }
        public BoletoBancario boletoBancario { get; set; }

        public Cedente gerarCedenteDefault(TituloCnab tituloCnab) {
            Cedente c = new Cedente(tituloCnab.LocalMovimento.Empresa.dc_num_cgc + "", tituloCnab.LocalMovimento.Empresa.beneficiario_cnab, tituloCnab.LocalMovimento.nm_agencia, tituloCnab.LocalMovimento.nm_conta_corrente, tituloCnab.LocalMovimento.nm_digito_conta_corrente + "");
            if (tituloCnab.LocalMovimento.nm_digito_cedente.HasValue)
                c.DigitoCedente = tituloCnab.LocalMovimento.nm_digito_cedente.Value;
            c.Codigo = tituloCnab.LocalMovimento.dc_num_cliente_banco;
            if (!string.IsNullOrEmpty(tituloCnab.LocalMovimento.nm_digito_agencia))
                c.ContaBancaria.DigitoAgencia = tituloCnab.LocalMovimento.nm_digito_agencia;
            c.Convenio = Convert.ToInt64(c.Codigo + tituloCnab.LocalMovimento.nm_digito_cedente);
            return c;
        }

        public Cedente gerarCedenteDefaultPersonalizado(LocalMovto LocalMovimentoPessoa, bool id_mostrar_3_boletos_pagina)
        {
            string no_cedente = LocalMovimentoPessoa.PessoaSGFBanco.beneficiario_cnab;
            if (id_mostrar_3_boletos_pagina)
                no_cedente += " Insc. Estadual: " + LocalMovimentoPessoa.PessoaSGFBanco.dc_num_insc_estadual_PJ + ",  " + "  " + LocalMovimentoPessoa.PessoaSGFBanco.EnderecoPrincipal.enderecoCompletoSimplificadoBoleto;
            Cedente c = new Cedente(LocalMovimentoPessoa.PessoaSGFBanco.nm_cpf_cgc + "", no_cedente, LocalMovimentoPessoa.nm_agencia, 
                LocalMovimentoPessoa.nm_conta_corrente, LocalMovimentoPessoa.nm_digito_conta_corrente + "");
            if (LocalMovimentoPessoa.nm_digito_cedente.HasValue)
                c.DigitoCedente = LocalMovimentoPessoa.nm_digito_cedente.Value;
            c.Codigo = LocalMovimentoPessoa.dc_num_cliente_banco;
            if (!string.IsNullOrEmpty(LocalMovimentoPessoa.nm_digito_agencia))
                c.ContaBancaria.DigitoAgencia = LocalMovimentoPessoa.nm_digito_agencia;
            c.Convenio = Convert.ToInt64(c.Codigo + LocalMovimentoPessoa.nm_digito_cedente);
            return c;
        }

        public Cedente gerarCedenteDefault(Cnab cnab)
        {
            Cedente c = new Cedente(cnab.LocalMovimento.PessoaSGFBanco.nm_cpf_cgc + "", cnab.LocalMovimento.PessoaSGFBanco.no_pessoa, cnab.LocalMovimento.nm_agencia, cnab.LocalMovimento.nm_conta_corrente, cnab.LocalMovimento.nm_digito_conta_corrente + "");

            c.Codigo = cnab.LocalMovimento.dc_num_cliente_banco;
            c.Convenio = long.Parse(c.Codigo + cnab.LocalMovimento.nm_digito_cedente);
            c.CodigoTransmissao = cnab.LocalMovimento.nm_transmissao;
            c.NumeroSequencial = cnab.nm_sequencia_remessa != null ? (int)cnab.nm_sequencia_remessa : 1;
            c.Carteira = cnab.LocalMovimento.CarteiraCnab.nm_carteira.ToString();
            if (!string.IsNullOrEmpty(cnab.LocalMovimento.nm_digito_agencia))
                c.ContaBancaria.DigitoAgencia = cnab.LocalMovimento.nm_digito_agencia;
            return c;
        }

        public System.IO.MemoryStream gerarArquivoCNAB240(IBanco banco, Cedente cedente, Boletos boletos, ReturnResult retornoErrors) {
            ArquivoRemessa arquivo = new ArquivoRemessa(TipoArquivo.CNAB240);
            System.IO.MemoryStream rdr = new System.IO.MemoryStream();
                
            //Valida a Remessa Correspondentes antes de Gerar a mesma...
            string vMsgRetorno = string.Empty;
            bool vValouOK = arquivo.ValidarArquivoRemessa(cedente.Convenio.ToString(), banco, cedente, boletos, 1, out vMsgRetorno);
            if (!vValouOK)
            {
                if (retornoErrors != null)
                {
                    retornoErrors.AddMensagem("Foram localizadas inconsistências na validação da remessa.", null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    if (!string.IsNullOrEmpty(vMsgRetorno))
                    {
                        retornoErrors.AddMensagem(vMsgRetorno, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    }
                    
                }
                else
                {
                    throw new Exception("Foram localizadas inconsistências na validação da remessa.");

                }
            }
            else
            {
                arquivo.GerarArquivoRemessa(cedente.Convenio.ToString(), banco, cedente, boletos, rdr, 1);
            }

            return rdr;
        }

        public System.IO.MemoryStream gerarArquivoCNAB400(IBanco banco, Cedente cedente, Boletos boletos, ReturnResult retornoErrors) {
            ArquivoRemessa arquivo = new ArquivoRemessa(TipoArquivo.CNAB400);
            System.IO.MemoryStream rdr = new System.IO.MemoryStream();

            //Valida a Remessa Correspondentes antes de Gerar a mesma...
            string vMsgRetorno = string.Empty;
            bool vValouOK = arquivo.ValidarArquivoRemessa(cedente.Convenio.ToString(), banco, cedente, boletos, 1, out vMsgRetorno);
            if (!vValouOK)
            {
                string mensagemErro = "Foram localizadas inconsistências na validação da remessa.";
                if (!string.IsNullOrEmpty(vMsgRetorno))
                {
                    mensagemErro += "       " + vMsgRetorno;
                }

                if (retornoErrors != null)
                {
                    retornoErrors.AddMensagem("Foram localizadas inconsistências na validação da remessa.", null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    if (!string.IsNullOrEmpty(vMsgRetorno))
                    {
                        char[] delimiterChars = {'\r','\n'};
                        List<string> listaErros = vMsgRetorno.Split(delimiterChars).ToList();

                        if (listaErros != null && listaErros.Count > 0)
                        {
                            foreach (string listaErro in listaErros)
                            {
                                if (!string.IsNullOrEmpty(listaErro))
                                {
                                    retornoErrors.AddMensagem(listaErro, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                                }
                                
                            }
                        }
                        
                    }

                }
                else
                {
                    throw new Exception(mensagemErro);

                }

                
            }
            else
                arquivo.GerarArquivoRemessa("0", banco, cedente, boletos, rdr, cedente.NumeroSequencial);
            return rdr;
        }

        public Boleto gerarBoletoDefault(TituloCnab tituloCnab, IEspecieDocumento especieDocumento, int instrucao, Cedente c, bool remessa, Boleto b, ReturnResult retornoErrors) {
            DateTime vencimento = tituloCnab.dt_vencimento_titulo.Value.Date;
            if (b == null)
            {
                b = new BoletoNet.Boleto(vencimento, tituloCnab.Titulo.vl_titulo, tituloCnab.LocalMovimento.CarteiraCnab.nm_carteira + "", tituloCnab.dc_nosso_numero_titulo, c);

                b.Sacado = new Sacado(tituloCnab.Titulo.Pessoa.nm_cpf_cgc, tituloCnab.Titulo.pagador_cnab);
                string descRemessaBoleto = remessa ? "a remessa" : "o boleto";

                b.Sacado.Endereco.End = tituloCnab.Titulo.Pessoa.EnderecoPrincipal.enderecoCompletoBoleto + "";
                b.Sacado.Endereco.Bairro = tituloCnab.Titulo.Pessoa.EnderecoPrincipal.Bairro.no_localidade + "";
                b.Sacado.Endereco.Cidade = tituloCnab.Titulo.Pessoa.EnderecoPrincipal.Cidade.no_localidade + "";
                b.Sacado.Endereco.CEP = tituloCnab.Titulo.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep + "";
                b.Sacado.Endereco.UF = tituloCnab.Titulo.Pessoa.EnderecoPrincipal.Estado.Estado.sg_estado + "";
                b.Sacado.Endereco.Complemento = tituloCnab.Titulo.Pessoa.EnderecoPrincipal.dc_compl_endereco + "";
                if (b.Sacado.Endereco.End == null || b.Sacado.Endereco.End == "")
                {
                    if (retornoErrors != null)
                    {
                        retornoErrors.AddMensagem(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, descRemessaBoleto, "o endereço", b.Sacado.Nome), null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    }
                    else
                    {
                        throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, descRemessaBoleto, "o endereço", b.Sacado.Nome), null, CnabBusinessException.TipoErro.ERRO_CAMPOS_ENDERECO_OBRIGATORIO, false);
                    }
                }

                if (tituloCnab.Titulo.Pessoa.EnderecoPrincipal.dc_num_endereco == null || tituloCnab.Titulo.Pessoa.EnderecoPrincipal.dc_num_endereco == "")
                {
                    if (retornoErrors != null)
                    {
                        retornoErrors.AddMensagem(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, descRemessaBoleto, "o número do endereço", b.Sacado.Nome), null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    }
                    else
                    {
                        throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, descRemessaBoleto, "o número do endereço", b.Sacado.Nome), null, CnabBusinessException.TipoErro.ERRO_CAMPOS_ENDERECO_OBRIGATORIO, false);
                    }
                }

                if (b.Sacado.Endereco.CEP == null || b.Sacado.Endereco.CEP == "")
                {
                    if (retornoErrors != null)
                    {
                        retornoErrors.AddMensagem(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, descRemessaBoleto, "o CEP", b.Sacado.Nome), null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    }
                    else
                    {
                        throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, descRemessaBoleto, "o CEP", b.Sacado.Nome), null, CnabBusinessException.TipoErro.ERRO_CAMPOS_ENDERECO_OBRIGATORIO, false);
                    }
                }
                    
            }
            Instrucao item = new Instrucao(instrucao);
            if(tituloCnab.tx_mensagem_cnab != null)
                item.Descricao = tituloCnab.tx_mensagem_cnab.Replace("\n", "<br/>");
            b.Instrucoes.Add(item);
            b.EspecieDocumento = especieDocumento;
            b.NumeroDocumento = tituloCnab.Titulo.nm_parcela_e_titulo;
            b.DataProcessamento = DateTime.Now;
            if(!remessa) {
                if (vencimento < DateTime.Now.Date)
                    b.DataDocumento = vencimento;
                else
                    b.DataDocumento = DateTime.Now;
                if (tituloCnab.id_mostrar_3_boletos_pagina)
                {
                    boletoBancario.FormatoCarne = true;
                    boletoBancario.OcultarInstrucoes = true;
                }
                b.QuantidadeMoeda = 0;
                b.ValorDesconto = 0;
                b.ValorMulta = 0;
                b.JurosMora = 0;
                b.PercMulta = 0;
                b.DataEmissao = tituloCnab.Titulo.dt_emissao_cnab;
            }
            else {
                b.DataEmissao = tituloCnab.Titulo.dt_emissao_cnab;
                b.DataProcessamento = tituloCnab.Titulo.dt_emissao_titulo; // Utilizado nos boletos de 400 posições
                b.DataDocumento = tituloCnab.Titulo.dt_emissao_titulo; // Utilizado nos boletos de 240 posições
                if(tituloCnab.pc_multa_titulo.HasValue) {
                    b.PercMulta = (decimal) tituloCnab.pc_multa_titulo.Value;
                    b.JurosMora = Decimal.Round((decimal) tituloCnab.pc_juros_titulo * tituloCnab.Titulo.vl_titulo / 100, 4);
                    b.DataMulta = b.DataVencimento;
                }
                if(tituloCnab.pc_juros_titulo.HasValue)
                    b.PercJurosMora = (decimal) tituloCnab.pc_juros_titulo.Value;
                if (tituloCnab.DescontoTituloCNAB != null && tituloCnab.DescontoTituloCNAB.Count() > 0)
                {
                    tituloCnab.DescontoTituloCNAB = tituloCnab.DescontoTituloCNAB.OrderBy(x => x.dt_desconto).ToList();
                    b.ValorDesconto = tituloCnab.DescontoTituloCNAB.FirstOrDefault().vl_desconto;
                    if (b.ValorDesconto > 0)
                        b.DataDesconto = tituloCnab.DescontoTituloCNAB.FirstOrDefault().dt_desconto;
                    if (tituloCnab.DescontoTituloCNAB.Count() > 1)
                    {
                        b.OutrosDescontos = tituloCnab.DescontoTituloCNAB.ToList()[1].vl_desconto;
                        b.DataOutrosDescontos = tituloCnab.DescontoTituloCNAB.ToList()[1].dt_desconto;
                    }
                    if (tituloCnab.DescontoTituloCNAB.Count() > 2)
                    {
                        b.OutrosDescontos2 = tituloCnab.DescontoTituloCNAB.ToList()[2].vl_desconto;
                        b.DataOutrosDescontos2 = tituloCnab.DescontoTituloCNAB.ToList()[2].dt_desconto;
                    }
                }
            }

            return b;
        }

        private string gerarBoletoBancarioDefaut(TituloCnab tituloCnab, IEspecieDocumento especieDocumento, int instrucao){
            boletoBancario.Boleto = gerarBoletoDefault(tituloCnab, especieDocumento, instrucao, gerarCedenteDefault(tituloCnab), false, null, null);
            boletoBancario.Boleto.Valida();
            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string BancodoBrasil(TituloCnab tituloCnab)
        {
            EspecieDocumento_BancoBrasil espDocBancoBrasil = new EspecieDocumento_BancoBrasil();
            IEspecieDocumento especieDocumento = new EspecieDocumento_BancoBrasil(espDocBancoBrasil.getCodigoEspecieByEnum(EnumEspecieDocumento_BancoBrasil.DuplicataMercantil));

            boletoBancario.Boleto = gerarBoletoDefault(tituloCnab, especieDocumento, (int) Bancos.BancodoBrasil, gerarCedenteDefault(tituloCnab), false, null, null);
            //boletoBancario.Boleto.LocalPagamento = "Pagável em qualquer banco até o vencimento. Após, atualize o boleto no site bb.com.br.";
            boletoBancario.Boleto.LocalPagamento = "PAGÁVEL EM QUALQUER BANCO";

            //Para apresentar o endereço no campo beneficiário é preciso marcar o parâmetro "MostrarEnderecoCedente" como true;
            boletoBancario.MostrarEnderecoCedente = true;
            // Preenche o endereço do cedente, o mesmo que será apresentado no boleto.
            boletoBancario.Cedente.Endereco = new Endereco();
            boletoBancario.Cedente.Endereco.Numero = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.dc_num_endereco + "";
            boletoBancario.Cedente.Endereco.End = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro
                                                    + " " + tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade;
            boletoBancario.Cedente.Endereco.Bairro = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Bairro.no_localidade + "";
            boletoBancario.Cedente.Endereco.UF = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado + "";
            boletoBancario.Cedente.Endereco.Cidade = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Cidade.no_localidade + "";
            boletoBancario.Cedente.Endereco.CEP = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep + "";

            boletoBancario.Boleto.Valida();
            if (tituloCnab.nm_dias_protesto > 0)
            {
                Instrucao instrucao = new Instrucao((int)Bancos.BancodoBrasil);
                instrucao.Codigo = (int)EnumInstrucoes_BancoBrasil.ProtestarAposNDiasCorridos;
                instrucao.QuantidadeDias = tituloCnab.nm_dias_protesto;
                instrucao.Descricao = (new Instrucao_BancoBrasil((int)EnumInstrucoes_BancoBrasil.ProtestarAposNDiasCorridos, tituloCnab.nm_dias_protesto)).Descricao;
                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.BancodoBrasil, ref instrucao, tituloCnab.LocalMovimento.CarteiraCnab.nm_colunas);
                boletoBancario.Boleto.Instrucoes.Add(instrucao);
            }

            //Sobscrever valores do campo beneficiário quando estiver marcado "MostrarEnderecoCedente".
            //var retornoHtml = boletoBancario.MontaHtml("/Content/Boletos/", "");
            //retornoHtml = retornoHtml.Replace("<td class=\"w472\">Beneficiário</td>", "<td class=\"w472\"><table><tr><td class=\"ct\" style=\"border-left:0px;\">Beneficiário</td><td class=\"cp h12 rBb\" style=\"border-left:0px;\">" + tituloCnab.LocalMovimento.PessoaSGFBanco.beneficiario_cnab + "</td></tr></table></td>");
            //retornoHtml = retornoHtml.Replace("Agência / Código Beneficiário</td>\r\n        </tr>\r\n        <tr class=\"cp h12 rBb\">\r\n            <td>" + boletoBancario.Cedente.Nome,
            //                                  "Agência / Código Beneficiário</td>\r\n        </tr>\r\n        <tr class=\"cp h12 rBb\">\r\n            <td>" + tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.enderecoCompletoSimplificadoBoleto);


            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string CrediSIS(TituloCnab tituloCnab)
        {
            EspecieDocumento_CrediSIS espDocCrediSIS = new EspecieDocumento_CrediSIS();
            IEspecieDocumento especieDocumento = new EspecieDocumento_CrediSIS(espDocCrediSIS.getCodigoEspecieByEnum(EnumEspecieDocumento_CrediSIS.DuplicataServicoIndicacao));

            boletoBancario.Boleto = gerarBoletoDefault(tituloCnab, especieDocumento, (int)Bancos.CrediSIS, gerarCedenteDefault(tituloCnab), false, null, null);
            boletoBancario.Boleto.Valida();
            if (tituloCnab.nm_dias_protesto > 0)
            {
                Instrucao instrucao = new Instrucao((int)Bancos.CrediSIS);
                instrucao.Codigo = (int)EnumInstrucoes_CrediSIS.ProtestarAposNDiasCorridos;
                instrucao.Descricao = (new Instrucao_CrediSIS((int)EnumInstrucoes_CrediSIS.ProtestarAposNDiasCorridos, tituloCnab.nm_dias_protesto)).Descricao;
                instrucao.QuantidadeDias = tituloCnab.nm_dias_protesto;
                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.CrediSIS, ref instrucao, tituloCnab.LocalMovimento.CarteiraCnab.nm_colunas);
                boletoBancario.Boleto.Instrucoes.Add(instrucao);
            }
            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string Banrisul()
        {
            DateTime vencimento = new DateTime(2008, 02, 07);

            Cedente c = new Cedente("00.000.000/0000-00", "Empresa de Atacado", "1234", "5", "12345678", "9");

            c.Codigo = "00000000504";

            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 45.50m, "18", "12345678901", c);

            b.Sacado = new Sacado("000.000.000-00", "Fulano de Silva");
            b.Sacado.Endereco.End = "SSS 154 Bloco J Casa 23";
            b.Sacado.Endereco.Bairro = "Testando";
            b.Sacado.Endereco.Cidade = "Testelândia";
            b.Sacado.Endereco.CEP = "70000000";
            b.Sacado.Endereco.UF = "DF";

            //Adiciona as instruções ao boleto
            #region Instruções
            //Protestar
            Instrucao_Banrisul item = new Instrucao_Banrisul(9, 10, 0);
            b.Instrucoes.Add(item);
            #endregion Instruções

            b.NumeroDocumento = "12345678901";


            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string Basa()
        {
            DateTime vencimento = new DateTime(2008, 02, 07);

            Cedente c = new Cedente("00.000.000/0000-00", "Empresa de Atacado", "1234", "5", "12345678", "9");

            c.Codigo = "12548";

            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 45.50m, "CNR", "125478", c);

            b.Sacado = new Sacado("000.000.000-00", "Nome do seu Cliente ");
            b.Sacado.Endereco.End = "Endereço do seu Cliente ";
            b.Sacado.Endereco.Bairro = "Bairro";
            b.Sacado.Endereco.Cidade = "Cidade";
            b.Sacado.Endereco.CEP = "00000000";
            b.Sacado.Endereco.UF = "UF";


            b.NumeroDocumento = "12345678901";

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string BradescoUnicred(TituloCnab tituloCnab)
        {
            string retornoHtml = "";
            EspecieDocumento_Bradesco espDocBradesco = new EspecieDocumento_Bradesco();
            IEspecieDocumento especieDocumento = new EspecieDocumento_Bradesco(espDocBradesco.getCodigoEspecieByEnum(EnumEspecieDocumento_Bradesco.DuplicataMercantil));

            Cedente c =  gerarCedenteDefaultPersonalizado(tituloCnab.LocalMovimento, tituloCnab.id_mostrar_3_boletos_pagina);
            DateTime vencimento = tituloCnab.dt_vencimento_titulo.Value.Date;
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, tituloCnab.Titulo.vl_titulo, tituloCnab.LocalMovimento.CarteiraCnab.nm_carteira + "", tituloCnab.dc_nosso_numero_titulo, c);

            b.Sacado = new Sacado(tituloCnab.Titulo.Pessoa.nm_cpf_cgc, tituloCnab.Titulo.pagador_cnab);
            string descRemessaBoleto = "o boleto";

            string enderecoSacado = "";
            string no_sacador_avalista = "Sacador / Avalista " + tituloCnab.LocalMovimento.Empresa.beneficiario_cnab + "  " + tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.enderecoCompletoSimplificadoBoleto;

            
            enderecoSacado = tituloCnab.Titulo.Pessoa.EnderecoPrincipal.enderecoCompletoSimplificadoBoleto;

            b.Sacado = new Sacado("nome???");
            b.Sacado.Endereco.End = "???";
            //b.Sacado.Endereco.End = "";
            b.Sacado.Endereco.Bairro = "";
            b.Sacado.Endereco.Cidade = "";
            b.Sacado.Endereco.CEP = "";
            b.Sacado.Endereco.UF = "";
            b.Sacado.Endereco.Complemento = "";
            if (tituloCnab.Titulo.Pessoa.EnderecoPrincipal.enderecoCompletoBoleto == null || tituloCnab.Titulo.Pessoa.EnderecoPrincipal.enderecoCompletoBoleto == "")
                throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, descRemessaBoleto, "o endereço", b.Sacado.Nome), null, CnabBusinessException.TipoErro.ERRO_CAMPOS_ENDERECO_OBRIGATORIO, false);
            if (tituloCnab.Titulo.Pessoa.EnderecoPrincipal.dc_num_endereco == null || tituloCnab.Titulo.Pessoa.EnderecoPrincipal.dc_num_endereco == "")
                throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, descRemessaBoleto, "o número do endereço", b.Sacado.Nome), null, CnabBusinessException.TipoErro.ERRO_CAMPOS_ENDERECO_OBRIGATORIO, false);
            if (tituloCnab.Titulo.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep == null || tituloCnab.Titulo.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep == "")
                throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, descRemessaBoleto, "o CEP", b.Sacado.Nome), null, CnabBusinessException.TipoErro.ERRO_CAMPOS_ENDERECO_OBRIGATORIO, false);
            boletoBancario.Boleto = gerarBoletoDefault(tituloCnab, especieDocumento, (int)Bancos.Bradesco, c, false, b, null);
            boletoBancario.Boleto.Carteira = Utils.Utils.FormatCode(boletoBancario.Boleto.Carteira, 2);
            boletoBancario.Boleto.Valida();
            boletoBancario.Boleto.LocalPagamento = "PAGÁVEL EM QUALQUER AGÊNCIA BANCÁRIA ATÉ O VENCIMENTO.";
            if (tituloCnab.LocalMovimento.PessoaSGFBanco == null || tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal == null ||
                tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.dc_num_endereco == null)
                throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, "o boleto", "o endereço", "beneficiário/sacacor avalista"), null, CnabBusinessException.TipoErro.ERRO_CNAB_SEM_PESSOA_BANCO_CNPJ, false);

            boletoBancario.Cedente.Endereco = new Endereco();
            boletoBancario.Cedente.Endereco.Numero = tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.dc_num_endereco + "";
            boletoBancario.Cedente.Endereco.End = tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro
                                                    + " " + tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.Logradouro.no_localidade;
            boletoBancario.Cedente.Endereco.Bairro = tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.Bairro.no_localidade + "";
            boletoBancario.Cedente.Endereco.UF = tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.Estado.Estado.sg_estado + "";
            boletoBancario.Cedente.Endereco.Cidade = tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.Cidade.no_localidade + "";
            boletoBancario.Cedente.Endereco.CEP = tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.Logradouro.dc_num_cep + "";
            //boletoBancario.Cedente.Nome = tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.enderecoCompletoBoleto + "";

            //Configurando o sacado
            boletoBancario.MostrarEnderecoCedente = true;
            boletoBancario.Boleto.Carteira = Utils.Utils.FormatCode(boletoBancario.Boleto.Carteira, 3);
            if (tituloCnab.nm_dias_protesto > 0)
            {
                Instrucao instrucao = new Instrucao((int)Bancos.Bradesco);
                instrucao.Codigo = (int)EnumInstrucoes_Bradesco.Protestar;
                instrucao.QuantidadeDias = tituloCnab.nm_dias_protesto;
                instrucao.Descricao = (new Instrucao_Bradesco((int)EnumInstrucoes_Bradesco.ProtestarAposNDiasCorridos, tituloCnab.nm_dias_protesto)).Descricao;
                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.Bradesco, ref instrucao, tituloCnab.LocalMovimento.CarteiraCnab.nm_colunas);
                boletoBancario.Boleto.Instrucoes.Add(instrucao);
            }
            if (tituloCnab.id_mostrar_3_boletos_pagina)
            {
                boletoBancario.FormatoCarne = true;
                //boletoBancario.OcultarInstrucoes = true;
            }
            //if (tituloCnab.id_mostrar_3_boletos_pagina)
            //    enderecoSacado = tituloCnab.Titulo.pagador_cnab + tituloCnab.Titulo.Pessoa.nm_cpf_cgc + " " + enderecoSacado;
            retornoHtml = boletoBancario.MontaHtml("/Content/Boletos/", "");
            retornoHtml = retornoHtml.Replace("<tr class=\"ct h13\">\r\n            <td class=\"w659\">Pagador</td>", "<tr class=\"ct\">\r\n            <td style=\"padding-left: 0px;border-left: 0px;\"><table class=\"w666\"><tr><td class=\"ct\" style=\"width: 5%;\">Pagador</td><td class=\"cpN pL6\" style=\"border-left: 0px;\">" + tituloCnab.Titulo.pagador_cnab + tituloCnab.Titulo.Pessoa.nm_cpf_cgc + "</td></tr></table></td>");
            if (tituloCnab.id_mostrar_3_boletos_pagina)
               retornoHtml = retornoHtml.Replace("nome???\r\n ", tituloCnab.Titulo.pagador_cnab + tituloCnab.Titulo.Pessoa.nm_cpf_cgc + " " + enderecoSacado + "\r\n ");            
            retornoHtml = retornoHtml.Replace("nome???", enderecoSacado);
            retornoHtml = retornoHtml.Replace("???<br /> - /", no_sacador_avalista);
            //retornoHtml = retornoHtml.Replace("<td class=\"At\"></td>", "<td class=\"At\">" + enderecoSacado  + "</td>");
            //retornoHtml = retornoHtml.Replace("<div class=\"cpN pL6\">", "<div class=\"cpN pL6\">" + no_sacador_avalista );
            
            retornoHtml = retornoHtml.Replace("<td class=\"w472\">Beneficiário</td>","<td class=\"w472\"><table><tr><td class=\"ct\" style=\"border-left:0px;\">Beneficiário</td><td class=\"cp h12 rBb\" style=\"border-left:0px;\">"+tituloCnab.LocalMovimento.PessoaSGFBanco.beneficiario_cnab +"</td></tr></table></td>");
            retornoHtml = retornoHtml.Replace("Agência / Código Beneficiário</td>\r\n        </tr>\r\n        <tr class=\"cp h12 rBb\">\r\n            <td>" + boletoBancario.Cedente.Nome,
                                              "Agência / Código Beneficiário</td>\r\n        </tr>\r\n        <tr class=\"cp h12 rBb\">\r\n            <td>" + tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.enderecoCompletoSimplificadoBoleto);
           
            
            return retornoHtml;
        }

        public string Bradesco(TituloCnab tituloCnab)
        {
            string retornoHtml = "";
            EspecieDocumento_Bradesco espDocBradesco = new EspecieDocumento_Bradesco();
            IEspecieDocumento especieDocumento = new EspecieDocumento_Bradesco(espDocBradesco.getCodigoEspecieByEnum(EnumEspecieDocumento_Bradesco.DuplicataMercantil));
            boletoBancario.Boleto = gerarBoletoDefault(tituloCnab, especieDocumento, (int) Bancos.Bradesco, gerarCedenteDefault(tituloCnab), false,null, null);
            //Instrucao_Bradesco instrucao = new Instrucao_Bradesco();
            //boletoBancario.Boleto.Instrucoes.Add(instrucao);
            boletoBancario.Boleto.Carteira = Utils.Utils.FormatCode(boletoBancario.Boleto.Carteira, 2);
            //Configura para mostrar o endereço do cedente:
            boletoBancario.MostrarEnderecoCedente = true;
            if (tituloCnab.LocalMovimento.Empresa == null || tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal == null || tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.dc_num_endereco == null)
                throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, "o boleto", "o endereço", "beneficiário/sacacor avalista"), null, CnabBusinessException.TipoErro.ERRO_CNAB_SEM_PESSOA_BANCO_CNPJ, false);

            boletoBancario.Cedente.Endereco = new Endereco();
            boletoBancario.Cedente.Endereco.Numero = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.dc_num_endereco + "";
            boletoBancario.Cedente.Endereco.End = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro
                                                    + " " + tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade;
            boletoBancario.Cedente.Endereco.Bairro = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Bairro.no_localidade + "";
            boletoBancario.Cedente.Endereco.UF = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado + "";
            boletoBancario.Cedente.Endereco.Cidade = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Cidade.no_localidade + "";
            boletoBancario.Cedente.Endereco.CEP = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep + "";

            boletoBancario.Boleto.Valida();
            boletoBancario.Boleto.Carteira = Utils.Utils.FormatCode(boletoBancario.Boleto.Carteira, 3);
            if (tituloCnab.nm_dias_protesto > 0)
            {
                Instrucao instrucao = new Instrucao((int)Bancos.Bradesco);
                instrucao.Codigo = (int)EnumInstrucoes_Bradesco.Protestar;
                instrucao.QuantidadeDias = tituloCnab.nm_dias_protesto;
                instrucao.Descricao = (new Instrucao_Bradesco((int)EnumInstrucoes_Bradesco.ProtestarAposNDiasCorridos, tituloCnab.nm_dias_protesto)).Descricao;
                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.Bradesco, ref instrucao, tituloCnab.LocalMovimento.CarteiraCnab.nm_colunas);
                boletoBancario.Boleto.Instrucoes.Add(instrucao);
            }
            if (tituloCnab.id_mostrar_3_boletos_pagina)
            {
                boletoBancario.FormatoCarne = true;
                //boletoBancario.OcultarInstrucoes = true;
            }
            
             retornoHtml = boletoBancario.MontaHtml("/Content/Boletos/", "");

             if (boletoBancario.Banco.Codigo == 237 && tituloCnab.id_mostrar_3_boletos_pagina == true)
             {
                 string enderecoCedente = "";


                 
                     if (boletoBancario.Cedente.Endereco == null)
                         throw new ArgumentNullException("Endereço do Cedente");

                     string Numero = !String.IsNullOrEmpty(boletoBancario.Cedente.Endereco.Numero) ? boletoBancario.Cedente.Endereco.Numero + ", " : "";
                     enderecoCedente = string.Concat(boletoBancario.Cedente.Endereco.End, " , ", Numero);

                     if (boletoBancario.Cedente.Endereco.CEP == String.Empty)
                     {
                         enderecoCedente += string.Format("{0} - {1}/{2}", boletoBancario.Cedente.Endereco.Bairro,
                             boletoBancario.Cedente.Endereco.Cidade, boletoBancario.Cedente.Endereco.UF);
                     }
                     else
                     {
                         enderecoCedente += string.Format("{0} - {1}/{2} - CEP: {3}", boletoBancario.Cedente.Endereco.Bairro,
                             boletoBancario.Cedente.Endereco.Cidade, boletoBancario.Cedente.Endereco.UF,
                             Utils.Utils.FormataCEP(boletoBancario.Cedente.Endereco.CEP));
                     }

                     retornoHtml = retornoHtml.Replace("<td class=\"w472\">Beneficiário</td>", "<td class=\"w472\">Beneficiário / Endereço</td>");
                     retornoHtml = retornoHtml.Replace("<tr class=\"cp h12 rBb\">\r\n            <td>" + boletoBancario.Cedente.Nome + "</td>", "<tr class=\"cp h12 rBb\">\r\n            <td>" + boletoBancario.Cedente.Nome + " / " + enderecoCedente + "</td>");
             }
             
             return retornoHtml;

        }

        public string DinariPay(TituloCnab tituloCnab)
        {
            string retornoHtml = "";
            EspecieDocumento_DinariPay espDocDinariPay = new EspecieDocumento_DinariPay();
            IEspecieDocumento especieDocumento = new EspecieDocumento_DinariPay(espDocDinariPay.getCodigoEspecieByEnum(EnumEspecieDocumento_DinariPay.DuplicataMercantil));
            boletoBancario.Boleto = gerarBoletoDefault(tituloCnab, especieDocumento, (int)Bancos.DinariPay, gerarCedenteDefault(tituloCnab), false, null, null);
            //Instrucao_DinariPay instrucao = new Instrucao_DinariPay();
            //boletoBancario.Boleto.Instrucoes.Add(instrucao);
            boletoBancario.Boleto.Carteira = Utils.Utils.FormatCode(boletoBancario.Boleto.Carteira, 2);
            //Configura para mostrar o endereço do cedente:
            boletoBancario.MostrarEnderecoCedente = true;
            if (tituloCnab.LocalMovimento.Empresa == null || tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal == null || tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.dc_num_endereco == null)
                throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, "o boleto", "o endereço", "beneficiário/sacacor avalista"), null, CnabBusinessException.TipoErro.ERRO_CNAB_SEM_PESSOA_BANCO_CNPJ, false);

            boletoBancario.Cedente.Endereco = new Endereco();
            boletoBancario.Cedente.Endereco.Numero = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.dc_num_endereco + "";
            boletoBancario.Cedente.Endereco.End = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro
                                                    + " " + tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade;
            boletoBancario.Cedente.Endereco.Bairro = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Bairro.no_localidade + "";
            boletoBancario.Cedente.Endereco.UF = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado + "";
            boletoBancario.Cedente.Endereco.Cidade = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Cidade.no_localidade + "";
            boletoBancario.Cedente.Endereco.CEP = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep + "";

            boletoBancario.Boleto.Valida();
            boletoBancario.Boleto.Carteira = Utils.Utils.FormatCode(boletoBancario.Boleto.Carteira, 3);
            if (tituloCnab.nm_dias_protesto > 0)
            {
                Instrucao instrucao = new Instrucao((int)Bancos.DinariPay);
                instrucao.Codigo = (int)EnumInstrucoes_DinariPay.Protestar;
                instrucao.QuantidadeDias = tituloCnab.nm_dias_protesto;
                instrucao.Descricao = (new Instrucao_DinariPay((int)EnumInstrucoes_DinariPay.ProtestarAposNDiasCorridos, tituloCnab.nm_dias_protesto)).Descricao;
                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.DinariPay, ref instrucao, tituloCnab.LocalMovimento.CarteiraCnab.nm_colunas);
                boletoBancario.Boleto.Instrucoes.Add(instrucao);
            }
            if (tituloCnab.id_mostrar_3_boletos_pagina)
            {
                boletoBancario.FormatoCarne = true;
                //boletoBancario.OcultarInstrucoes = true;
            }

            retornoHtml = boletoBancario.MontaHtml("/Content/Boletos/", "");

            if (boletoBancario.Banco.Codigo == 238 && tituloCnab.id_mostrar_3_boletos_pagina == true)
            {
                string enderecoCedente = "";



                if (boletoBancario.Cedente.Endereco == null)
                    throw new ArgumentNullException("Endereço do Cedente");

                string Numero = !String.IsNullOrEmpty(boletoBancario.Cedente.Endereco.Numero) ? boletoBancario.Cedente.Endereco.Numero + ", " : "";
                enderecoCedente = string.Concat(boletoBancario.Cedente.Endereco.End, " , ", Numero);

                if (boletoBancario.Cedente.Endereco.CEP == String.Empty)
                {
                    enderecoCedente += string.Format("{0} - {1}/{2}", boletoBancario.Cedente.Endereco.Bairro,
                        boletoBancario.Cedente.Endereco.Cidade, boletoBancario.Cedente.Endereco.UF);
                }
                else
                {
                    enderecoCedente += string.Format("{0} - {1}/{2} - CEP: {3}", boletoBancario.Cedente.Endereco.Bairro,
                        boletoBancario.Cedente.Endereco.Cidade, boletoBancario.Cedente.Endereco.UF,
                        Utils.Utils.FormataCEP(boletoBancario.Cedente.Endereco.CEP));
                }

                retornoHtml = retornoHtml.Replace("<td class=\"w472\">Beneficiário</td>", "<td class=\"w472\">Beneficiário / Endereço</td>");
                retornoHtml = retornoHtml.Replace("<tr class=\"cp h12 rBb\">\r\n            <td>" + boletoBancario.Cedente.Nome + "</td>", "<tr class=\"cp h12 rBb\">\r\n            <td>" + boletoBancario.Cedente.Nome + " / " + enderecoCedente + "</td>");
            }

            return retornoHtml;

        }

        public string Uniprime(TituloCnab tituloCnab)
        {
            string retornoHtml = "";
            EspecieDocumento_Bradesco espDocBradesco = new EspecieDocumento_Bradesco();
            IEspecieDocumento especieDocumento = new EspecieDocumento_Bradesco(espDocBradesco.getCodigoEspecieByEnum(EnumEspecieDocumento_Bradesco.DuplicataMercantil));

            Cedente c = gerarCedenteDefaultPersonalizado(tituloCnab.LocalMovimento, tituloCnab.id_mostrar_3_boletos_pagina);
            DateTime vencimento = tituloCnab.dt_vencimento_titulo.Value.Date;
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, tituloCnab.Titulo.vl_titulo, tituloCnab.LocalMovimento.CarteiraCnab.nm_carteira + "", tituloCnab.dc_nosso_numero_titulo, c);

            b.Sacado = new Sacado(tituloCnab.Titulo.Pessoa.nm_cpf_cgc, tituloCnab.Titulo.pagador_cnab);
            string descRemessaBoleto = "o boleto";

            string enderecoSacado = "";
            //string no_sacador_avalista = "Sacador / Avalista " + tituloCnab.LocalMovimento.Empresa.beneficiario_cnab + "  " + tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.enderecoCompletoSimplificadoBoleto;
            enderecoSacado = tituloCnab.Titulo.Pessoa.EnderecoPrincipal.enderecoCompletoSimplificadoBoleto;
            b.Sacado = new Sacado("nome???");
            b.Sacado.Endereco.End = "???";
            //b.Sacado.Endereco.End = "";
            b.Sacado.Endereco.Bairro = "";
            b.Sacado.Endereco.Cidade = "";
            b.Sacado.Endereco.CEP = "";
            b.Sacado.Endereco.UF = "";
            b.Sacado.Endereco.Complemento = "";
            if (tituloCnab.Titulo.Pessoa.EnderecoPrincipal.enderecoCompletoBoleto == null || tituloCnab.Titulo.Pessoa.EnderecoPrincipal.enderecoCompletoBoleto == "")
                throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, descRemessaBoleto, "o endereço", b.Sacado.Nome), null, CnabBusinessException.TipoErro.ERRO_CAMPOS_ENDERECO_OBRIGATORIO, false);
            if (tituloCnab.Titulo.Pessoa.EnderecoPrincipal.dc_num_endereco == null || tituloCnab.Titulo.Pessoa.EnderecoPrincipal.dc_num_endereco == "")
                throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, descRemessaBoleto, "o número do endereço", b.Sacado.Nome), null, CnabBusinessException.TipoErro.ERRO_CAMPOS_ENDERECO_OBRIGATORIO, false);
            if (tituloCnab.Titulo.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep == null || tituloCnab.Titulo.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep == "")
                throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, descRemessaBoleto, "o CEP", b.Sacado.Nome), null, CnabBusinessException.TipoErro.ERRO_CAMPOS_ENDERECO_OBRIGATORIO, false);
            boletoBancario.Boleto = gerarBoletoDefault(tituloCnab, especieDocumento, (int)Bancos.Bradesco, c, false, b, null);
            boletoBancario.Boleto.Carteira = Utils.Utils.FormatCode(boletoBancario.Boleto.Carteira, 2);
            boletoBancario.Boleto.Valida();
            //boletoBancario.Boleto.LocalPagamento += " OU BRADESCO EXPRESSO";
            if (tituloCnab.LocalMovimento.PessoaSGFBanco == null || tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal == null ||
                tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.dc_num_endereco == null)
                throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, "o boleto", "o endereço", "beneficiário/sacacor avalista"), null, CnabBusinessException.TipoErro.ERRO_CNAB_SEM_PESSOA_BANCO_CNPJ, false);

            boletoBancario.Cedente.Endereco = new Endereco();
            boletoBancario.Cedente.Endereco.Numero = tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.dc_num_endereco + "";
            boletoBancario.Cedente.Endereco.End = tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro
                                                    + " " + tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.Logradouro.no_localidade;
            boletoBancario.Cedente.Endereco.Bairro = tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.Bairro.no_localidade + "";
            boletoBancario.Cedente.Endereco.UF = tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.Estado.Estado.sg_estado + "";
            boletoBancario.Cedente.Endereco.Cidade = tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.Cidade.no_localidade + "";
            boletoBancario.Cedente.Endereco.CEP = tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.Logradouro.dc_num_cep + "";
            //boletoBancario.Cedente.Nome = tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.enderecoCompletoBoleto + "";

            //Configurando o sacado
            boletoBancario.MostrarEnderecoCedente = true;
            boletoBancario.Boleto.Carteira = Utils.Utils.FormatCode(boletoBancario.Boleto.Carteira, 3);
            if (tituloCnab.nm_dias_protesto > 0)
            {
                Instrucao instrucao = new Instrucao((int)Bancos.Bradesco);
                instrucao.Codigo = (int)EnumInstrucoes_Bradesco.Protestar;
                instrucao.QuantidadeDias = tituloCnab.nm_dias_protesto;
                instrucao.Descricao = (new Instrucao_Bradesco((int)EnumInstrucoes_Bradesco.ProtestarAposNDiasCorridos, tituloCnab.nm_dias_protesto)).Descricao;
                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.Bradesco, ref instrucao, tituloCnab.LocalMovimento.CarteiraCnab.nm_colunas);
                boletoBancario.Boleto.Instrucoes.Add(instrucao);
            }
            if (tituloCnab.id_mostrar_3_boletos_pagina)
            {
                boletoBancario.FormatoCarne = true;
                //boletoBancario.OcultarInstrucoes = true;
            }
            //if (tituloCnab.id_mostrar_3_boletos_pagina)
            //    enderecoSacado = tituloCnab.Titulo.pagador_cnab + tituloCnab.Titulo.Pessoa.nm_cpf_cgc + " " + enderecoSacado;
            retornoHtml = boletoBancario.MontaHtml("/Content/Boletos/", "");
            retornoHtml = retornoHtml.Replace("<tr class=\"ct h13\">\r\n            <td class=\"w659\">Pagador</td>", "<tr class=\"ct\">\r\n            <td style=\"padding-left: 0px;border-left: 0px;\"><table class=\"w666\"><tr><td class=\"ct\" style=\"width: 5%;\">Pagador</td><td class=\"cpN pL6\" style=\"border-left: 0px;\">" + tituloCnab.Titulo.pagador_cnab + tituloCnab.Titulo.Pessoa.nm_cpf_cgc + "</td></tr></table></td>");
            if (tituloCnab.id_mostrar_3_boletos_pagina)
                retornoHtml = retornoHtml.Replace("nome???\r\n ", tituloCnab.Titulo.pagador_cnab + tituloCnab.Titulo.Pessoa.nm_cpf_cgc + " " + enderecoSacado + "\r\n ");
            retornoHtml = retornoHtml.Replace("nome???", enderecoSacado);
            retornoHtml = retornoHtml.Replace("???<br /> - /", "");
            //retornoHtml = retornoHtml.Replace("<td class=\"At\"></td>", "<td class=\"At\">" + enderecoSacado  + "</td>");
            //retornoHtml = retornoHtml.Replace("<div class=\"cpN pL6\">", "<div class=\"cpN pL6\">" + no_sacador_avalista );

            //recibo do sacasa, impressão 3 boletos por página.
            retornoHtml = retornoHtml.Replace("<td class=\"w472\">Beneficiário</td>", "<td class=\"w472\"><table><tr><td class=\"ct\" style=\"border-left:0px;\">Beneficiário</td><td class=\"cp h12 rBb\" style=\"border-left:0px;\">" + tituloCnab.LocalMovimento.PessoaSGFBanco.beneficiario_cnab + ", Insc. Estadual: " + tituloCnab.LocalMovimento.PessoaSGFBanco.dc_num_insc_estadual_PJ + "</td></tr></table></td>");
            retornoHtml = retornoHtml.Replace("Agência / Código Beneficiário</td>\r\n        </tr>\r\n        <tr class=\"cp h12 rBb\">\r\n            <td>" + boletoBancario.Cedente.Nome,
                                              "Agência / Código Beneficiário</td>\r\n        </tr>\r\n        <tr class=\"cp h12 rBb\">\r\n            <td>" + tituloCnab.LocalMovimento.PessoaSGFBanco.EnderecoPrincipal.enderecoCompletoSimplificadoBoleto);


            return retornoHtml;
        }
        
        public string BRB()
        {
            DateTime vencimento = new DateTime(2007, 11, 15);

            Cedente c = new Cedente("00.000.000/0000-00", "Empresa de Atacado", "208", "", "010357", "6");
            c.Codigo = "13000";

            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 0.01m, "COB", "119964", c);
            b.NumeroDocumento = "119964";

            b.Sacado = new Sacado("000.000.000-00", "Nome do seu Cliente ");
            b.Sacado.Endereco.End = "Endereço do seu Cliente ";
            b.Sacado.Endereco.Bairro = "Bairro";
            b.Sacado.Endereco.Cidade = "Cidade";
            b.Sacado.Endereco.CEP = "00000000";
            b.Sacado.Endereco.UF = "UF";

            //b.Instrucoes.Add("Não Receber após o vencimento");
            //b.Instrucoes.Add("Após o Vencimento pague somente no Bradesco");
            //b.Instrucoes.Add("Instrução 2");
            //b.Instrucoes.Add("Instrução 3");

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string Caixa(TituloCnab tituloCnab)
        {
            EspecieDocumento_Caixa espDocCaixa = new EspecieDocumento_Caixa();
            IEspecieDocumento especieDocumento = new EspecieDocumento_Caixa(espDocCaixa.getCodigoEspecieByEnum(EnumEspecieDocumento_Caixa.DuplicataMercantil));

            boletoBancario.Boleto = gerarBoletoDefault(tituloCnab, especieDocumento, (int)Bancos.Caixa, gerarCedenteDefault(tituloCnab), false, null, null);

            switch (boletoBancario.Boleto.Carteira) {
                case "24":
                    if(Convert.ToInt64(boletoBancario.Boleto.NossoNumero).ToString().Length < 17)
                        boletoBancario.Boleto.NossoNumero = Utils.Utils.FormatCode(boletoBancario.Boleto.NossoNumero, 17);
                    boletoBancario.Boleto.NossoNumero = "24" + boletoBancario.Boleto.NossoNumero.Substring(2, boletoBancario.Boleto.NossoNumero.Length - 2);
                    break;
                case "14":
                    if(Convert.ToInt64(boletoBancario.Boleto.NossoNumero).ToString().Length < 17)
                        boletoBancario.Boleto.NossoNumero = Utils.Utils.FormatCode(boletoBancario.Boleto.NossoNumero, 17);
                    if (!tituloCnab.LocalMovimento.CarteiraCnab.id_registrada)
                        boletoBancario.Boleto.NossoNumero = "24" + boletoBancario.Boleto.NossoNumero.Substring(2, boletoBancario.Boleto.NossoNumero.Length - 2);
                    else
                        boletoBancario.Boleto.NossoNumero = "14" + boletoBancario.Boleto.NossoNumero.Substring(2, boletoBancario.Boleto.NossoNumero.Length - 2);
                    //Configura para mostrar o endereço do cedente:
                    boletoBancario.MostrarEnderecoCedente = true;
                    if (tituloCnab.LocalMovimento.Empresa == null || tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal == null || tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.dc_num_endereco == null)
                        throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, "o boleto", "o endereço", "beneficiário/sacacor avalista"), null, CnabBusinessException.TipoErro.ERRO_CNAB_SEM_PESSOA_BANCO_CNPJ, false);

                    boletoBancario.Cedente.Endereco = new Endereco();
                    boletoBancario.Cedente.Endereco.Numero = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.dc_num_endereco + "";
                    boletoBancario.Cedente.Endereco.End = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro
                                                            + " " + tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade;
                    boletoBancario.Cedente.Endereco.Bairro = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Bairro.no_localidade + "";
                    boletoBancario.Cedente.Endereco.UF = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado + "";
                    boletoBancario.Cedente.Endereco.Cidade = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Cidade.no_localidade + "";
                    boletoBancario.Cedente.Endereco.CEP = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep + "";
                    break;
                case "1":
                    if(Convert.ToInt64(boletoBancario.Boleto.NossoNumero).ToString().Length < 17)
                        boletoBancario.Boleto.NossoNumero = Utils.Utils.FormatCode(boletoBancario.Boleto.NossoNumero, 17);
                    boletoBancario.Boleto.NossoNumero = "24" + boletoBancario.Boleto.NossoNumero.Substring(2, boletoBancario.Boleto.NossoNumero.Length - 2);
                    break;
            }

            if ("24".Equals(boletoBancario.Boleto.Carteira) || "01".Equals(boletoBancario.Boleto.Carteira) || "1".Equals(boletoBancario.Boleto.Carteira) || "14".Equals(boletoBancario.Boleto.Carteira))
            {
                if(!tituloCnab.LocalMovimento.CarteiraCnab.id_registrada)
                    boletoBancario.Boleto.Carteira = "SR";
                else
                    boletoBancario.Boleto.Carteira = "RG";
            }

            boletoBancario.Boleto.Valida();
            boletoBancario.Boleto.LocalPagamento = "EM TODA A REDE BANCÁRIA E SEUS CORRESPONDENTES ATÉ O VALOR LIMITE";
            if (tituloCnab.nm_dias_protesto > 0)
            {
                Instrucao instrucao = new Instrucao((int)Bancos.Caixa);
                instrucao.Codigo = (int)EnumInstrucoes_Caixa.Protestar;
                instrucao.QuantidadeDias = tituloCnab.nm_dias_protesto;
                instrucao.Descricao = (new Instrucao_Caixa((int)EnumInstrucoes_Caixa.ProtestarAposNDiasCorridos, tituloCnab.nm_dias_protesto)).Descricao;
                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.Caixa, ref instrucao, tituloCnab.LocalMovimento.CarteiraCnab.nm_colunas);
                boletoBancario.Boleto.Instrucoes.Add(instrucao);
            }
            if (boletoBancario.Sacado != null)
            {
                boletoBancario.Sacado.Instrucoes.Add(new Instrucao((int)Bancos.Caixa)
                {
                    Descricao = " </br></br> <b>SAC CAIXA:</b> 0800 726 0101 (informações, reclamações, sugestões e elogios) </br>" +
                         "<b>Para pessoas com deficiência auditiva ou de fala:</b> 0800 726 2492 </br>" +
                         "<b>Ouvidoria:</b> 0800 725 7474 &nbsp&nbsp&nbsp&nbsp&nbsp <a href=''>caixa.gov.br</a>"
                });
            }
            string retorno = boletoBancario.MontaHtml("/Content/Boletos/", "");
            retorno = retorno.Replace("<div class=\"t\">(+) Mora / Multa</div>", "<div class=\"t\">(+) Mora / Multa / Juros</div>");
            retorno = retorno.Replace("<td class=\"w113\">(+) Mora / Multa</td>", "<td class=\"w113\">(+) Mora / Multa / Juros</td>");
            retorno = retorno.Replace(">Instruções<", ">Instruções (Texto de responsabilidade do beneficiário)<");
            return retorno;
        }

        public string HSBC(TituloCnab tituloCnab)
        {
            EspecieDocumento_HSBC espDocHSBC = new EspecieDocumento_HSBC();
            IEspecieDocumento especieDocumento;
            if(tituloCnab.LocalMovimento.CarteiraCnab.id_impressao == (byte)CarteiraCnab.TipoImpressao.BANCO)
                especieDocumento = new EspecieDocumento_HSBC(espDocHSBC.getCodigoEspecieByEnum(EnumEspecieDocumento_HSBC.DuplicataMercantil));
            else
                especieDocumento = new EspecieDocumento_HSBC(espDocHSBC.getCodigoEspecieByEnum(EnumEspecieDocumento_HSBC.CobrancaEmissaoCliente));


            if (!tituloCnab.LocalMovimento.CarteiraCnab.id_registrada)
            {
                tituloCnab.LocalMovimento.dc_num_cliente_banco = tituloCnab.LocalMovimento.dc_num_cliente_banco;
                tituloCnab.LocalMovimento.nm_digito_cedente = tituloCnab.LocalMovimento.nm_digito_cedente;
            }
            else
            {
                tituloCnab.LocalMovimento.dc_num_cliente_banco = tituloCnab.LocalMovimento.nm_conta_corrente;
                tituloCnab.LocalMovimento.nm_digito_cedente = short.Parse(tituloCnab.LocalMovimento.nm_digito_conta_corrente);
            }
            boletoBancario.Boleto = gerarBoletoDefault(tituloCnab, especieDocumento, (int)Bancos.HSBC, gerarCedenteDefault(tituloCnab), false, null, null);
            if(!tituloCnab.LocalMovimento.CarteiraCnab.id_registrada) {
                boletoBancario.Boleto.NossoNumero = tituloCnab.Titulo.Pessoa.cd_pessoa + "";
                boletoBancario.Boleto.Carteira = "CNR";
                boletoBancario.Boleto.EspecieDocumento = null;
            }
            else 
                boletoBancario.Boleto.Carteira = "CSB";
            boletoBancario.Boleto.Valida();
           
            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string Itau(TituloCnab tituloCnab)
        {
            EspecieDocumento_Itau espDocItau = new EspecieDocumento_Itau();
            IEspecieDocumento especieDocumento = new EspecieDocumento_Itau(espDocItau.getCodigoEspecieByEnum(EnumEspecieDocumento_Itau.DuplicataMercantil));

            boletoBancario.Boleto = gerarBoletoDefault(tituloCnab, especieDocumento, (int)Bancos.Itau, gerarCedenteDefault(tituloCnab), false, null, null);

            //Configura para mostrar o endereço do cedente:
            boletoBancario.MostrarEnderecoCedente = true;

            if(tituloCnab.LocalMovimento.Empresa == null || tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal == null
                    || tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro == null || tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade == null)
                throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, "o boleto", "o endereço", "beneficiário/sacacor avalista"), null, CnabBusinessException.TipoErro.ERRO_CNAB_SEM_PESSOA_BANCO_CNPJ, false);

            boletoBancario.Cedente.Endereco = new Endereco();
            boletoBancario.Cedente.Endereco.Numero = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.dc_num_endereco + "";
            boletoBancario.Cedente.Endereco.End = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro
                                                    + " " + tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade;
            boletoBancario.Cedente.Endereco.Bairro = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Bairro.no_localidade + "";
            boletoBancario.Cedente.Endereco.UF = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado + "";
            boletoBancario.Cedente.Endereco.Cidade = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Cidade.no_localidade + "";
            boletoBancario.Cedente.Endereco.CEP = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep + "";

            boletoBancario.Boleto.Valida();
            if (tituloCnab.nm_dias_protesto > 0)
            {
                Instrucao_Itau instrucao = new Instrucao_Itau((int)EnumInstrucoes_Itau.ProtestarAposNDiasCorridos, tituloCnab.nm_dias_protesto);
                instrucao.Codigo = (int)EnumInstrucoes_Itau.Protestar;
                instrucao.QuantidadeDias = tituloCnab.nm_dias_protesto;
                boletoBancario.Boleto.Instrucoes.Add(instrucao);
            }
            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string Inter(TituloCnab tituloCnab)
        {
            EspecieDocumento_Inter espDocInter = new EspecieDocumento_Inter();
            IEspecieDocumento especieDocumento = new EspecieDocumento_Inter(espDocInter.getCodigoEspecieByEnum(EnumEspecieDocumento_Inter.DuplicataMercantil));

            boletoBancario.Boleto = gerarBoletoDefault(tituloCnab, especieDocumento, (int)Bancos.Inter, gerarCedenteDefault(tituloCnab), false, null, null);

            //Configura para mostrar o endereço do cedente:
            boletoBancario.MostrarEnderecoCedente = true;

            if (tituloCnab.LocalMovimento.Empresa == null || tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal == null
                    || tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro == null || tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade == null)
                throw new CnabBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotGeraRemessaSemEnd, "o boleto", "o endereço", "beneficiário/sacacor avalista"), null, CnabBusinessException.TipoErro.ERRO_CNAB_SEM_PESSOA_BANCO_CNPJ, false);

            boletoBancario.Cedente.Endereco = new Endereco();
            boletoBancario.Cedente.Endereco.Numero = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.dc_num_endereco + "";
            boletoBancario.Cedente.Endereco.End = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro
                                                    + " " + tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade;
            boletoBancario.Cedente.Endereco.Bairro = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Bairro.no_localidade + "";
            boletoBancario.Cedente.Endereco.UF = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado + "";
            boletoBancario.Cedente.Endereco.Cidade = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Cidade.no_localidade + "";
            boletoBancario.Cedente.Endereco.CEP = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep + "";

            boletoBancario.Boleto.LocalPagamento = "PAGÁVEL EM QUALQUER BANCO";
            boletoBancario.Boleto.Valida();
            if (tituloCnab.nm_dias_protesto > 0)
            {
                Instrucao_Inter instrucao = new Instrucao_Inter((int)EnumInstrucoes_Inter.ProtestarAposNDiasCorridos, tituloCnab.nm_dias_protesto);
                instrucao.Codigo = (int)EnumInstrucoes_Itau.Protestar;
                instrucao.QuantidadeDias = tituloCnab.nm_dias_protesto;
                boletoBancario.Boleto.Instrucoes.Add(instrucao);
            }
            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string Real()
        {
            DateTime vencimento = new DateTime(2008, 12, 16);

            Cedente c = new Cedente("00.000.000/0000-00", "Coloque a Razão Social da sua empresa aqui", "1234", "12345");
            c.Codigo = "12345";

            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 0.1m, "57", "123456", c, new EspecieDocumento(356, "9"));
            b.NumeroDocumento = "1234567";

            b.Sacado = new Sacado("000.000.000-00", "Nome do seu Cliente ");
            b.Sacado.Endereco.End = "Endereço do seu Cliente ";
            b.Sacado.Endereco.Bairro = "Bairro";
            b.Sacado.Endereco.Cidade = "Cidade";
            b.Sacado.Endereco.CEP = "00000000";
            b.Sacado.Endereco.UF = "UF";

            //b.Instrucoes.Add("Não Receber após o vencimento");
            //b.Instrucoes.Add("Após o Vencimento pague somente no Real");
            //b.Instrucoes.Add("Instrução 2");
            //b.Instrucoes.Add("Instrução 3");
            boletoBancario.Boleto = b;

            EspeciesDocumento ed = EspecieDocumento_Real.CarregaTodas();
            boletoBancario.Boleto.Valida();

            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string Safra()
        {
            DateTime vencimento = new DateTime(2007, 9, 10);

            Cedente c = new Cedente("00.000.000/0000-00", "Empresa de Atacado", "0542", "5413000");

            c.Codigo = "13000";

            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 1642, "198", "02592082835", c);
            b.NumeroDocumento = "1008073";

            b.Sacado = new Sacado("000.000.000-00", "Nome do seu Cliente ");
            b.Sacado.Endereco.End = "Endereço do seu Cliente ";
            b.Sacado.Endereco.Bairro = "Bairro";
            b.Sacado.Endereco.Cidade = "Cidade";
            b.Sacado.Endereco.CEP = "00000000";
            b.Sacado.Endereco.UF = "UF";

            //b.Instrucoes.Add("Não Receber após o vencimento");
            //b.Instrucoes.Add("Após o Vencimento pague somente no Bradesco");
            //b.Instrucoes.Add("Instrução 2");
            //b.Instrucoes.Add("Instrução 3");

            Instrucao_Safra instrucao = new Instrucao_Safra();
            instrucao.Descricao = "Instrução 1";

            b.Instrucoes.Add(instrucao);


            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string Santander(TituloCnab tituloCnab)
        {
            EspecieDocumento_Bradesco espDocCaixa = new EspecieDocumento_Bradesco();
            IEspecieDocumento especieDocumento = new EspecieDocumento_Bradesco(espDocCaixa.getCodigoEspecieByEnum(EnumEspecieDocumento_Bradesco.DuplicataMercantil));

            boletoBancario.Boleto = gerarBoletoDefault(tituloCnab, especieDocumento, (int)Bancos.Bradesco, gerarCedenteDefault(tituloCnab), false, null, null);

            boletoBancario.Cedente.Endereco = new Endereco();
            boletoBancario.Cedente.Endereco.Numero = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.dc_num_endereco + "";
            boletoBancario.Cedente.Endereco.End = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro
                                                    + " " + tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade;
            boletoBancario.Cedente.Endereco.Bairro = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Bairro.no_localidade + "";
            boletoBancario.Cedente.Endereco.UF = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado + "";
            boletoBancario.Cedente.Endereco.Cidade = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Cidade.no_localidade + "";
            boletoBancario.Cedente.Endereco.CEP = tituloCnab.LocalMovimento.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep + "";
            boletoBancario.Boleto.JurosMora = Decimal.Round(boletoBancario.Boleto.JurosMora, 2);
            boletoBancario.MostrarEnderecoCedente = true;

            boletoBancario.Boleto.Valida();

            if (boletoBancario.Boleto.Carteira.Equals(((int)EnumCarteiras_Santander.CobrancaSimplesComRegistro) + ""))
            {
                //Alteração chamado 354499 - 14/02/2023
                //Preciso que ajuste no boleto o nome da carteira para o nome solicitado pelo Santander.  No campo carteira – que esta Cobranca Simples RCR para 101 .
                boletoBancario.Boleto.Carteira = ((int)EnumCarteiras_Santander.CobrancaSimplesComRegistro) + "";
                //boletoBancario.Boleto.Carteira = "Cobrança Simples RCR";
            }
                
            else
                if (boletoBancario.Boleto.Carteira.Equals(((int)EnumCarteiras_Santander.CobrancaSimplesSemRegistro) + ""))
                    boletoBancario.Boleto.Carteira = "Cobrança Simples CSR";
            if (tituloCnab.nm_dias_protesto > 0)
            {
                Instrucao_Santander instrucao = new Instrucao_Santander((int)EnumInstrucoes_Santander.Protestar, tituloCnab.nm_dias_protesto);
                instrucao.Codigo = (int)EnumInstrucoes_Santander.Protestar;
                instrucao.QuantidadeDias = tituloCnab.nm_dias_protesto;
                boletoBancario.Boleto.Instrucoes.Add(instrucao);
            }

            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string Sicoob(TituloCnab tituloCnab)
        {
            EspecieDocumento_Sicoob espDocSicoob = new EspecieDocumento_Sicoob();
            IEspecieDocumento especieDocumento = new EspecieDocumento_Sicoob(espDocSicoob.getCodigoEspecieByEnum(EnumEspecieDocumento_Sicoob.DuplicataServicoIndicacao));
            boletoBancario.Boleto = gerarBoletoDefault(tituloCnab, especieDocumento, (int)Bancos.Sicoob, gerarCedenteDefault(tituloCnab), false, null, null);
            if (tituloCnab.LocalMovimento.CarteiraCnab.id_registrada)
            {
                boletoBancario.Boleto.Cedente.DigitoCedente = 0;
                boletoBancario.Boleto.TipoModalidade = "01";
            }
            else
                boletoBancario.Boleto.TipoModalidade = "02";
            if (boletoBancario.Boleto.Cedente.Codigo.Length < 7)
                boletoBancario.Boleto.Cedente.Codigo = boletoBancario.Boleto.Cedente.Codigo.PadLeft(7, '0');
            boletoBancario.Boleto.NumeroParcela = tituloCnab.Titulo != null && tituloCnab.Titulo.nm_parcela_titulo.HasValue ? (int)tituloCnab.Titulo.nm_parcela_titulo : 0;
            boletoBancario.Boleto.Valida();
            if (tituloCnab.nm_dias_protesto > 0)
            {
                Instrucao instrucao = new Instrucao((int)Bancos.Sicoob);
                instrucao.Codigo = (int)EnumInstrucoes_Sicoob.Protestar;
                instrucao.QuantidadeDias = tituloCnab.nm_dias_protesto;
                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.Sicoob, ref instrucao, tituloCnab.LocalMovimento.CarteiraCnab.nm_colunas);
                boletoBancario.Boleto.Instrucoes.Add(instrucao);
            }
            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string Sicred(TituloCnab tituloCnab)
        {
            EspecieDocumento_Sicredi espDocSicredi = new EspecieDocumento_Sicredi();
            IEspecieDocumento especieDocumento = new EspecieDocumento_Sicredi(espDocSicredi.getCodigoEspecieByEnum(EnumEspecieDocumento_Sicredi.DuplicataMercantilIndicacao));

            boletoBancario.Boleto = gerarBoletoDefault(tituloCnab, especieDocumento, (int)Bancos.Sicred, gerarCedenteDefault(tituloCnab), false, null, null);

            if(boletoBancario.Boleto.NossoNumero.Length != 6)
                throw new NotImplementedException("Nosso número inválido. Nosso número deve possuir o byte de geração e também 5 dígitos para o número sequencial, compondo 6 dígitos.");
            boletoBancario.Boleto.LocalPagamento = "PAGÁVEL PREFERENCIALMENTE NAS COOPERATIVAS DE CRÉDITO DO SICREDI";
            boletoBancario.Boleto.Cedente.ContaBancaria.OperacaConta = Utils.Utils.FormatCode(tituloCnab.LocalMovimento.nm_op_conta + "", 2);
            boletoBancario.Boleto.Valida();
            if (tituloCnab.nm_dias_protesto > 0)
            {
                Instrucao instrucao = new Instrucao((int)Bancos.Sicred);
                instrucao.Codigo = (int)EnumInstrucoes_Sicredi.PedidoProtesto;
                instrucao.QuantidadeDias = tituloCnab.nm_dias_protesto;
                instrucao.Descricao = (new Instrucao_Sicredi((int)EnumInstrucoes_Sicredi.PedidoProtesto, tituloCnab.nm_dias_protesto)).Descricao;
                ConfiguradorBoletoBancario.configurarDiasProtestoBanco((int)Bancos.Sicred, ref instrucao, tituloCnab.LocalMovimento.CarteiraCnab.nm_colunas);
                boletoBancario.Boleto.Instrucoes.Add(instrucao);
            }
            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string Sudameris()
        {
            DateTime vencimento = new DateTime(2007, 9, 10);

            Cedente c = new Cedente("00.000.000/0000-00", "Empresa de Atacado", "0501", "6703255");

            c.Codigo = "13000";

            //Nosso número com 7 dígitos
            string nn = "0003020";
            //Nosso número com 13 dígitos
            //nn = "0000000003025";

            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 1642, "198", nn, c);// EnumEspecieDocumento_Sudameris.DuplicataMercantil);
            b.NumeroDocumento = "1008073";

            b.Sacado = new Sacado("000.000.000-00", "Nome do seu Cliente ");
            b.Sacado.Endereco.End = "Endereço do seu Cliente ";
            b.Sacado.Endereco.Bairro = "Bairro";
            b.Sacado.Endereco.Cidade = "Cidade";
            b.Sacado.Endereco.CEP = "00000000";
            b.Sacado.Endereco.UF = "UF";

            //b.Instrucoes.Add("Não Receber após o vencimento");
            //b.Instrucoes.Add("Após o Vencimento pague somente no Sudameris");
            //b.Instrucoes.Add("Instrução 2");
            //b.Instrucoes.Add("Instrução 3");

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public string Unibanco()
        {
            // ----------------------------------------------------------------------------------------
            // Exemplo 1

            //DateTime vencimento = new DateTime(2001, 12, 31);

            //Cedente c = new Cedente("00.000.000/0000-00", "Next Consultoria", "1234", "5", "123456", "7");
            //c.Codigo = 123456;

            //BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 1000.00, "20", "1803029901", c);
            //b.NumeroDocumento = b.NossoNumero;

            // ----------------------------------------------------------------------------------------
            // Exemplo 2

            DateTime vencimento = new DateTime(2008, 03, 10);

            Cedente c = new Cedente("00.000.000/0000-00", "Next Consultoria Ltda.", "0123", "100618", "9");
            c.Codigo = "203167";

            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 2952.95m, "20", "1803029901", c);
            b.NumeroDocumento = b.NossoNumero;

            // ----------------------------------------------------------------------------------------

            //b.Instrucoes.Add("Não receber após o vencimento");
            //b.Instrucoes.Add("Após o vencimento pague somente no Unibanco");
            //b.Instrucoes.Add("Taxa bancária - R$ 2,95");
            //b.Instrucoes.Add("Emitido pelo componente Boleto.NET");

            // ----------------------------------------------------------------------------------------

            b.Sacado = new Sacado("000.000.000-00", "Nome do seu Cliente ");
            b.Sacado.Endereco.End = "Endereço do seu Cliente ";
            b.Sacado.Endereco.Bairro = "Bairro";
            b.Sacado.Endereco.Cidade = "Cidade";
            b.Sacado.Endereco.CEP = "00000000";
            b.Sacado.Endereco.UF = "UF";

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        public static void configurarDiasProtestoBanco(int banco, ref Instrucao instrucao, int posicoes)
        {
            switch ((Bancos)banco)
            {
                case Bancos.Caixa:
                    if (instrucao != null && instrucao.QuantidadeDias > 0)
                        if (instrucao.QuantidadeDias == 1)
                            instrucao.QuantidadeDias = 2;
                        else if (instrucao.QuantidadeDias > 90)
                            instrucao.QuantidadeDias = 90;
                    break;
                case Bancos.Bradesco:
                case Bancos.DinariPay:
                case Bancos.Unicred:
                case Bancos.Uniprime:
                    if (instrucao != null && instrucao.QuantidadeDias > 0)
                        if (instrucao.QuantidadeDias < 5)
                            instrucao.QuantidadeDias = 5;
                    break;
                case Bancos.Sicred:
                    if (instrucao != null && instrucao.QuantidadeDias > 0)
                        if (instrucao.QuantidadeDias < 3)
                            instrucao.QuantidadeDias = 3;
                    instrucao.Descricao = "Protestar após " + instrucao.QuantidadeDias + " dias corridos do vencimento";
                    break;
                case Bancos.BancodoBrasil:
                    if (instrucao != null && instrucao.QuantidadeDias > 0)
                        if (instrucao.QuantidadeDias < 6)
                            instrucao.QuantidadeDias = 6;
                        else if (instrucao.QuantidadeDias > 29 && instrucao.QuantidadeDias <= 35)
                        {
                            instrucao.QuantidadeDias = 35;
                        }
                        else if (instrucao.QuantidadeDias > 35)
                            instrucao.QuantidadeDias = 40;
                    break;
                case Bancos.Sicoob:
                    if (posicoes == (int)CarteiraCnab.TipoColunas.CNAB400)
                    {
                        if (instrucao != null && instrucao.QuantidadeDias > 0)
                        {
                            if (instrucao.QuantidadeDias <= 3)
                            {
                                instrucao.QuantidadeDias = 3;
                                instrucao.Descricao = (new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.Protestar3DiasUteis)).Descricao;
                            }
                            else if (instrucao.QuantidadeDias == 4)
                            {
                                instrucao.Descricao = (new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.Protestar4DiasUteis)).Descricao;
                            }
                            else if (instrucao.QuantidadeDias == 5)
                            {
                                instrucao.Descricao = (new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.Protestar5DiasUteis)).Descricao;
                            }
                            else if (instrucao.QuantidadeDias > 5 && instrucao.QuantidadeDias <= 10)
                            {
                                instrucao.QuantidadeDias = 10;
                                instrucao.Descricao = (new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.Protestar10DiasUteis)).Descricao;
                            }
                            else if (instrucao.QuantidadeDias > 10 && instrucao.QuantidadeDias <= 15)
                            {
                                instrucao.QuantidadeDias = 15;
                                instrucao.Descricao = (new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.Protestar15DiasUteis)).Descricao;
                            }
                            else if (instrucao.QuantidadeDias > 15)
                            {
                                instrucao.QuantidadeDias = 20;
                                instrucao.Descricao = (new Instrucao_Sicoob((int)EnumInstrucoes_Sicoob.Protestar20DiasUteis)).Descricao;
                            }
                        }
                    }
                    else
                        instrucao.Descricao = "Protestar após " + instrucao.QuantidadeDias + " dias corridos do vencimento";
                        break;
            }
        }
    }
}