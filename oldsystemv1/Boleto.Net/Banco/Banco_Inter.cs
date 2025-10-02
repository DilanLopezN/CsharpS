using System;
using System.Web.UI;
using Microsoft.VisualBasic;
using System.Text;
using BoletoNet.Util;

[assembly: WebResource("BoletoNet.Imagens.077.jpg", "image/jpg")]
namespace BoletoNet
{
    /// <summary>
    /// Classe referente ao banco Itaú
    /// </summary>
    internal class Banco_Inter : AbstractBanco, IBanco
    {

        #region Variáveis

        private int _dacBoleto = 0;
        private int _dacNossoNumero = 0;

        #endregion

        #region Construtores

        internal Banco_Inter()
        {
            try
            {
                this.Codigo = 077;
                this.Digito = "9";
                this.Nome = "INTER";
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao instanciar objeto.", ex);
            }
        }

        #endregion

        #region Métodos de Instância

        /// <summary>
        /// Validações particulares do banco Inter
        /// </summary>
        public override void ValidaBoleto(Boleto boleto)
        {
            try
            {
                //Carteiras válidas
                int[] cv = new int[] { 112 };
                bool valida = false;

                foreach (int c in cv)
                    if (Utils.ToString(boleto.Carteira) == Utils.ToString(c))
                        valida = true;

                if (!valida)
                {
                    StringBuilder carteirasImplementadas = new StringBuilder(100);

                    carteirasImplementadas.Append(". Carteiras implementadas: ");
                    foreach (int c in cv)
                    {
                        carteirasImplementadas.AppendFormat(" {0}", c);
                    }
                    throw new NotImplementedException("Carteira não implementada: " + boleto.Carteira + carteirasImplementadas.ToString());
                }
                //Verifica se o tamanho para o NossoNumero são 8 dígitos
                if (Convert.ToInt32(boleto.NossoNumero).ToString().Length > 11)
                    throw new NotImplementedException("A quantidade de dígitos do nosso número para a carteira " + boleto.Carteira + ", são 11 números.");
                else if (!String.IsNullOrEmpty(boleto.NossoNumero) && Convert.ToInt32(boleto.NossoNumero).ToString().Length < 11)
                    boleto.NossoNumero = Utils.FormatCode(boleto.NossoNumero, 11);

                
                //if (Utils.ToInt32(boleto.NumeroDocumento) == 0){
                //        throw new NotImplementedException("O número do documento não pode ser igual a zero.");
                //}

                //Formato o número do documento 
                if (Utils.ToInt32(boleto.NumeroDocumento) > 0)
                    boleto.NumeroDocumento = Utils.FormatCode(boleto.NumeroDocumento, 7);

                

                // Calcula o DAC da Conta Corrente
                boleto.Cedente.ContaBancaria.DigitoConta = Mod10(boleto.Cedente.ContaBancaria.Agencia + boleto.Cedente.ContaBancaria.Conta).ToString();
                //Atribui o nome do banco ao local de pagamento
               // boleto.LocalPagamento += Nome + ". Após o vencimento, somente no ITAÚ";

                //Verifica se o nosso número é válido
                //if (Utils.ToInt64(boleto.NossoNumero) == 0)
                //    throw new NotImplementedException("Banco " + boleto.Banco.Nome + " Nosso Número não Informado");

                //Verifica se data do processamento é valida
                //if (boleto.DataProcessamento.ToString("dd/MM/yyyy") == "01/01/0001")
                if (boleto.DataProcessamento == DateTime.MinValue) // diegomodolo (diego.ribeiro@nectarnet.com.br)
                    boleto.DataProcessamento = DateTime.Now;

                //Verifica se data do documento é valida
                //if (boleto.DataDocumento.ToString("dd/MM/yyyy") == "01/01/0001")
                if (boleto.DataDocumento == DateTime.MinValue) // diegomodolo (diego.ribeiro@nectarnet.com.br)
                    boleto.DataDocumento = DateTime.Now;

                boleto.FormataCampos();
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao validar boletos.", e);
            }
        }

        # endregion

        # region Métodos de formatação do boleto

        public override void FormataCodigoBarra(Boleto boleto)
        {
            try
            {

                long fatorVencimento = FatorVencimento(boleto);

                string valorBoleto = boleto.ValorBoleto.ToString("f").Replace(",", "").Replace(".", "");
                valorBoleto = Utils.FormatCode(valorBoleto, 10);

                string numeroOperacao = Utils.FormatCode("0000000".ToString(), 7);//Disponivel na tela de retorno do banco inter
                string agencia = boleto.Cedente.ContaBancaria.Agencia; //Código da Agência(Sem DV)
                string campoLivre = String.Format("{0}{1}{2}{3}",
                    Utils.FormatCode(agencia, 4),
                    Utils.FormatCode(boleto.Carteira, 3),
                    numeroOperacao,
                    Utils.FormatCode(boleto.NossoNumero == null ? "0" : boleto.NossoNumero, 11));

                string codigoBarras = String.Format(
                    "{0}{1}{2}{3}{4}",
                    Utils.FormatCode(Codigo.ToString(), 3), //Identificação do Banco (Sem DV)
                    "9", //Moeda
                    Utils.FormatCode(fatorVencimento.ToString(), 4), //Fator de Vencimento
                    valorBoleto, //Valor nominal do título

                    //Campo Livre (Código da Agência (Sem DV), Número da Carteira do Título, Número da Operação (Disponível na tela de Retorno*), Nosso Número (Com DV) 
                    Utils.FormatCode(campoLivre, 25)

                );


                boleto.CodigoBarra.Codigo = codigoBarras;

                _dacBoleto = Mod11(boleto.CodigoBarra.Codigo, 9, 0);

                boleto.CodigoBarra.Codigo = Strings.Left(boleto.CodigoBarra.Codigo, 4) + _dacBoleto + Strings.Right(boleto.CodigoBarra.Codigo, 39);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao formatar código de barras.", ex);
            }
        }

        public override void FormataLinhaDigitavel(Boleto boleto)
        {
            try
            {
                string banco = Utils.FormatCode(Codigo.ToString(), 3);
                string moeda = boleto.Moeda.ToString();
                string agencia = "0001"; //Código da Agência(Sem DV)
                string numeroOperacao = Utils.FormatCode("0000000", 7);//Disponivel na tela de retorno do banco inter
                string campoLivre = String.Format("{0}{1}{2}{3}",
                    Utils.FormatCode(agencia, 4),
                    Utils.FormatCode(boleto.Carteira, 3),
                    numeroOperacao,
                    Utils.FormatCode(boleto.NossoNumero == null ? "0" : boleto.NossoNumero, 11));


                //Campo1 -> (Identificação do Banco (Sem DV), Moeda, Cinco primeiras posições do campo livre, Dígito verificador do primeiro campo)
                string digitoVerificadorPrimeiroCampo = Mod10(String.Format("{0}{1}{2}", banco, moeda, campoLivre.Substring(0, 5))).ToString();

                string campo1 = String.Empty;

                campo1 = string.Format("{0}{1}{2}.", banco, moeda, campoLivre.Substring(0, 1));
                campo1 += string.Format("{0}{1} ", campoLivre.Substring(1, 4), digitoVerificadorPrimeiroCampo);


                //Campo2 -> (6a a 15a posições do campo livre, Dígito verificador do segundo campo)
                string digitoVerificadorSegundoCampo = Mod10(campoLivre.Substring(5, 10)).ToString(); ;

                string campo2 = String.Empty;

                campo2 = string.Format("{0}.", campoLivre.Substring(5, 5));
                campo2 += string.Format("{0}{1} ", campoLivre.Substring(10, 5), digitoVerificadorSegundoCampo);


                //Campo3 -> (16a a 25a posições do campo livre, Dígito verificador do terceiro campo)
                string digitoVerificadorTerceiroCampo = Mod10(campoLivre.Substring(15, 10)).ToString(); ;

                string campo3 = String.Empty;

                campo3 = string.Format("{0}.", campoLivre.Substring(15, 5));
                campo3 += string.Format("{0}{1}", campoLivre.Substring(20, 5), digitoVerificadorTerceiroCampo);


                //Campo4 -> (Dígito verificador geral)*Posição 5 do código de barras
                string campo4 = String.Empty;
                campo4 = string.Format(" {0} ", boleto.CodigoBarra.Codigo.Substring(4, 1));
                

                //Campo5 -> (Fator de vencimento, Valor nominal do título)* Posicao 6 a 19 do codigo de barras 
                string campo5 = String.Empty;
                campo5 = boleto.CodigoBarra.Codigo.Substring(5, 14);
                

                boleto.CodigoBarra.LinhaDigitavel = campo1 + campo2 + campo3 + campo4 + campo5;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao formatar linha digitável.", ex);
            }
        }

        public override void FormataNossoNumero(Boleto boleto)
        {
            try
            {
                boleto.NossoNumero = string.Format("{0}{1}/{2}/{3}-{4}", 
                    boleto.Cedente.ContaBancaria.Agencia,
                    boleto.Cedente.ContaBancaria.DigitoAgencia,
                    boleto.Carteira, 
                    boleto.NossoNumero, 
                    _dacNossoNumero);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao formatar nosso número", ex);
            }
        }

        public override void FormataCodigoCedente(Boleto boleto)
        {
            try
            {
                boleto.Cedente.Codigo = string.Format("{0}{1}/{2}",
                    boleto.Cedente.ContaBancaria.Agencia,
                    boleto.Cedente.ContaBancaria.DigitoAgencia,
                    boleto.Cedente.Codigo);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao formatar nosso número", ex);
            }
        }

        public override void FormataNumeroDocumento(Boleto boleto)
        {
            try
            {
                boleto.NumeroDocumento = string.Format("{0}-{1}", boleto.NumeroDocumento, Mod10(boleto.NumeroDocumento));
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao formatar número do documento.", ex);
            }
        }

        /// <summary>
        /// Verifica o tipo de ocorrência para o arquivo remessa
        /// 02: Result := '02-Entrada Confirmada';
        /// 03: Result := '03-Entrada Rejeitada';
        /// 06: Result := '06-Liquida��o Normal';
        /// 07: Result := '07-Baixa Simples';
        /// </summary>
        public string Ocorrencia(string codigo)
        {
            switch (codigo)
            {
                case "02":
                    return "02-Entrada Confirmada";
                case "03":
                    return "03-Entrada Rejeitada";
                case "06":
                    return "06-Liquidação normal";
                case "07":
                    return "07-Baixa Simples";
                
                default:
                    return "";
            }
        }

        # endregion

        # region Métodos de geração do arquivo remessa

        # region HEADER
        public override string GerarHeaderRemessa(string numeroConvenio, Cedente cendente, TipoArquivo tipoArquivo, int numeroArquivoRemessa, Boleto boletos)
        {
            throw new NotImplementedException("Função não implementada.");
        }
        public override string GerarHeaderRemessa(Cedente cedente, TipoArquivo tipoArquivo, int numeroArquivoRemessa)
        {
            return GerarHeaderRemessa("0", cedente, tipoArquivo, numeroArquivoRemessa);
        }
        /// <summary>
        /// HEADER do arquivo CNAB
        /// Gera o HEADER do arquivo remessa de acordo com o lay-out informado
        /// </summary>
        public override string GerarHeaderRemessa(string numeroConvenio, Cedente cedente, TipoArquivo tipoArquivo, int numeroArquivoRemessa)
        {
            try
            {
                string _header = " ";

                base.GerarHeaderRemessa("0", cedente, tipoArquivo, numeroArquivoRemessa);

                switch (tipoArquivo)
                {

                    case TipoArquivo.CNAB240:
                        _header = GerarHeaderRemessaCNAB240(cedente);
                        break;
                    case TipoArquivo.CNAB400:
                        _header = GerarHeaderRemessaCNAB400(0, cedente, numeroArquivoRemessa);
                        break;
                    case TipoArquivo.Outro:
                        throw new Exception("Tipo de arquivo inexistente.");
                }

                return _header;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do HEADER do arquivo de REMESSA.", ex);
            }
        }

       
        public string GerarHeaderRemessaCNAB240(Cedente cedente)
        {
            throw new NotImplementedException("Função não implementada.");
        }

        public string GerarHeaderRemessaCNAB400(int numeroConvenio, Cedente cedente, int numeroArquivoRemessa)
        {
            try
            {
                string complemento = new string(' ', 294);
                string _header;


                _header = "0"; // 001 a 001 Identifica��o do registro
                _header += "1"; // 002 a 002 Identifica��o do arquivo remessa
                _header += "REMESSA"; // 003 a 009 Literal remessa
                _header += "01"; // 010 a 011 C�digo de servi�o
                _header += Utils.FitStringLength("COBRANCA", 15, 15, ' ', 0, true, true, false).ToUpper(); // 012 a 026 Literal servi�o
                _header += new string(' ', 20); // 027 a 046 C�digo da empresa
                _header += Utils.FitStringLength(cedente.Nome, 30, 30, ' ', 0, true, true, false).ToUpper(); // 047 a 076 Nome da empresa
                _header += Utils.FitStringLength(Codigo.ToString(), 3, 3, '0', 0, true, true, true); // 077 a 079 N�mero do Inter na c�mara de compensa��o
                _header += Utils.FitStringLength("INTER", 15, 15, ' ', 0, true, true, false).ToUpper(); // 080 a 094 Nome do banco por extenso
                _header += DateTime.Now.ToString("ddMMyy"); // 095 a 100 Data da grava��o do arquivo
                _header += new string(' ', 10); // 101 a 110 Branco
                _header += Utils.FitStringLength(numeroArquivoRemessa.ToString(), 7, 7, '0', 0, true, true, true); // 111 a 117 N� sequencial de remessa
                _header += new string(' ', 277);// 118 a 394 Branco
                _header += "000001"; // 395 a 400 N� sequencial do registro de um em um

                _header = Utils.SubstituiCaracteresEspeciais(_header);

                return _header;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }

        # endregion

        # region Header do Lote

        public override string GerarHeaderLoteRemessa(string numeroConvenio, Cedente cedente, int numeroArquivoRemessa, TipoArquivo tipoArquivo)
        {
            try
            {
                string header = " ";

                switch (tipoArquivo)
                {

                    case TipoArquivo.CNAB240:
                        header = GerarHeaderLoteRemessaCNAB240(cedente, numeroArquivoRemessa);
                        break;
                    case TipoArquivo.CNAB400:
                        header = GerarHeaderLoteRemessaCNAB400(0, cedente, numeroArquivoRemessa);
                        break;
                    case TipoArquivo.Outro:
                        throw new Exception("Tipo de arquivo inexistente.");
                }

                return header;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do HEADER DO LOTE do arquivo de REMESSA.", ex);
            }
        }

        

        private string GerarHeaderLoteRemessaCNAB240(Cedente cedente, int numeroArquivoRemessa)
        {
            throw new NotImplementedException("Função não implementada.");
        }

        private string GerarHeaderLoteRemessaCNAB400(int numeroConvenio, Cedente cedente, int numeroArquivoRemessa)
        {
            throw new Exception("Função não implementada.");
        }

        #endregion

        # region DETALHE

        /// <summary>
        /// DETALHE do arquivo CNAB
        /// Gera o DETALHE do arquivo remessa de acordo com o lay-out informado
        /// </summary>
        public override string GerarDetalheRemessa(Boleto boleto, int numeroRegistro, TipoArquivo tipoArquivo)
        {
            try
            {
                string _detalhe = " ";

                base.GerarDetalheRemessa(boleto, numeroRegistro, tipoArquivo);

                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        _detalhe = GerarDetalheRemessaCNAB240(boleto, numeroRegistro, tipoArquivo);
                        break;
                    case TipoArquivo.CNAB400:
                        _detalhe = GerarDetalheRemessaCNAB400(boleto, numeroRegistro, tipoArquivo);
                        break;
                    case TipoArquivo.Outro:
                        throw new Exception("Tipo de arquivo inexistente.");
                }

                return _detalhe;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do DETALHE arquivo de REMESSA.", ex);
            }
        }

        
        public string GerarDetalheRemessaCNAB240(Boleto boleto, int numeroRegistro, TipoArquivo tipoArquivo)
        {
            throw new NotImplementedException("Função não implementada.");
        }

        public string GerarDetalheRemessaCNAB400(Boleto boleto, int numeroRegistro, TipoArquivo tipoArquivo)
        {
            try
            {
                string ACodigoMoraJuros,
                    wCarteira,
                    EspecieDoc,
                    pTipoSacado;



                // tipo de sacado
                if (boleto.Sacado.CPFCNPJ.Length <= 11)
                {
                    pTipoSacado = "01";//Fisica
                }else
                {
                    pTipoSacado = "02";//Juridica
                }



                EspecieDoc = boleto.EspecieDocumento.Sigla;

                if (EspecieDoc != "DM")
                {
                    EspecieDoc = "01";
                }
                

                wCarteira = Utils.FitStringLength(boleto.Carteira, 3, 3, '0', 0, true, true, true);

                string _detalhe;

                _detalhe = "1";//001 a 001 Identificação do registro
                _detalhe += new string(' ', 19); //002 a 020 Branco
                _detalhe += ("1120001" + 
                             Utils.FitStringLength(boleto.Cedente.ContaBancaria.Conta.ToString(), 9, 9, '0', 0, true, true, true) +
                             Utils.FitStringLength(boleto.Cedente.ContaBancaria.DigitoConta, 1, 1, '0', 0, true, true, true));//D da conta(1));//021 a 037 Identificação da empresa beneficiária no Inter
                _detalhe += Utils.FitStringLength(boleto.NumeroDocumento, 25, 25, ' ', 0, true, true, false); //038 a 062 Identificação do título na empresa
                _detalhe += new string(' ', 3); //063 a 065 Branco
                /*Se = 0, sem multa
                  Se = 1, valor fixo de multa
                  Se = 2, percentual de multa*/
                var CampoMulta = "0";
                if (boleto.PercMulta > 0)
                {
                    // 2, percentual de multa
                    CampoMulta = "2";
                }
                else if (boleto.ValorMulta > 0)
                {
                    // 1, valor fixo de multa
                    CampoMulta = "1";
                }
                else
                {
                    //0, sem multa
                    CampoMulta = "0";
                }
                _detalhe += CampoMulta;//066 a 066 Campo de multa

                /*Caso 1 no campo 66, preencher valor fixo
                     da multa.Do contrário, preencher com
                 zeros.*/
                if (CampoMulta == "1")
                {
                    _detalhe += Utils.FitStringLength(boleto.ValorMulta.ApenasNumeros(), 13, 13, '0', 0, true, true, true); //067 a 079 Valor da multa
                }
                else
                {
                    _detalhe += Utils.FitStringLength("0", 13, 13, '0', 0, true, true, true); //067 a 079 Valor da multa
                }
                


                /*Caso 2 no campo 66, preencher percentual
                de multa com 2 casas decimais. Do
                    contrário, preencher com zeros.*/
                if (CampoMulta == "2")
                {
                    _detalhe += Utils.FitStringLength(Convert.ToInt32(boleto.PercMulta * 100).ToString(), 4, 4, '0', 0, true, true, true);//080 a 083 Percentual de multa
                }
                else
                {
                    _detalhe += Utils.FitStringLength("0", 4, 4, '0', 0, true, true, true);//080 a 083 Percentual de multa
                }


                /*
                  DDMMAA obrigatório se 1 ou 2 no campo 66. Do contrário, preencher com zeros
                 */
                if (CampoMulta == "1" || CampoMulta == "2")
                {
                    _detalhe += boleto.DataMulta.ToString("ddMMyy");
                }
                else
                {
                    _detalhe += "000000";
                }


                /*
                 * Zeros
                (será preenchido pelo Inter no arquivo
                retorno)
                 */
                _detalhe += Utils.FitStringLength("0", 11, 11, '0', 0, true, true, true);//090 a 100 Identificação do título no banco("Nosso número")


                _detalhe += new string(' ', 8); //101 a 108 Branco

                _detalhe += "01";//109 a 110 Identificação da ocorrência -> 01 (Remessa)

                _detalhe += Utils.FitStringLength(boleto.NumeroDocumento, 10, 10, ' ', 0, true, true, true);//111 a 120 Nº do documento (Seu número)

                _detalhe += boleto.DataVencimento.ToString("ddMMyy");//121 a 126 Data do vencimento do título


                /*
                 * Valor do título (preencher sem ponto e sem
                    vírgula). Considerar 2 (duas) casas decimais.
                 */
                _detalhe += Utils.FitStringLength(boleto.ValorBoleto.ToString("0.00").Replace(",", ""), 13, 13, '0', 0, true, true, true);//127 a 139 Valor do título


                /*Informar “0”, "30" ou "60".Esses são os dias
                    decorridos da data de vencimento do título
                    em que ainda será possível o pagamento*/
                _detalhe += Utils.FitStringLength("30", 2, 2, '0', 0, true, true, true);//140 a 141 Data limite de pagamento

                _detalhe += new string(' ', 6); //142 a 147 Branco

                _detalhe += Utils.FitStringLength("01", 2, 2, '0', 0, true, true, true); //148 a 149 Espécie do título

                _detalhe += "N"; //150 a 150 Identificação

                _detalhe += new string(' ', 6);//151 a 156 Data da emissão do título

                _detalhe += new string(' ', 3);//157 a 159 Branco

                /*Se = 0, sem juros/ mora
                Se = 1, valor fixo de juros/ mora
                Se = 2, percentual de juros / mora*/
                ACodigoMoraJuros = "0";
                 if (boleto.JurosMora > 0)
                {
                    //1, valor fixo de juros/ mora
                    ACodigoMoraJuros = "1";
                }
                 else
                if (boleto.PercJurosMora > 0)
                {
                    //2, percentual de juros / mora*/
                    ACodigoMoraJuros = "2";
                }
                else
                {
                    //0, sem juros/ mora
                    ACodigoMoraJuros = "0";
                }
                _detalhe += ACodigoMoraJuros;//160 a 160 Campo de juros/ mora

                /*Caso 1 no campo 160, preencher valor fixo
                cobrado por dia de atraso. Do contrário,
                preencher com zeros.*/
                if (ACodigoMoraJuros == "1")
                {
                    _detalhe += Utils.FitStringLength(boleto.JurosMora.ToString("0.00").Replace(",", ""), 13, 13, '0', 0, true, true, true); //161 a 173 Valor a ser cobrado por dia deatraso
                }
                else
                {
                    _detalhe += Utils.FitStringLength("0", 13, 13, '0', 0, true, true, true);//161 a 173 Valor a ser cobrado por dia deatraso
                }

                /*
                 * Caso 2 no campo 160, preencher o percentual
                    a ser cobrado por dia de atraso. Do contrário,
                    preencher com zeros.
                 */
                if (ACodigoMoraJuros == "2")
                {
                    _detalhe += Utils.FitStringLength(Convert.ToInt32(boleto.PercJurosMora * 100).ToString(), 4, 4, '0', 0, true, true, true);//174 a 177 Percentual a ser cobrado juros/mora
                }
                else
                {
                    _detalhe += Utils.FitStringLength("0", 4, 4, '0', 0, true, true, true);//174 a 177 Percentual a ser cobrado juros/mora
                }

                /*
                 * DDMMAA - obrigatório se 1 ou 2 no campo
                    160. Do contrário, preencher com zeros
                 */
                if (ACodigoMoraJuros == "1" || ACodigoMoraJuros == "2")
                {
                    string dataJurosMora;
                    if (boleto.PercJurosMora == 0)
                    {
                        dataJurosMora = "000000";
                    }
                    else if (boleto.DataJurosMora > boleto.DataVencimento)
                    {
                        dataJurosMora = Utils.FormatCode(boleto.DataJurosMora.ToString("ddMMyy"), "0", 6, true); //178 a 183 Data da mora
                    }
                    else
                    {
                        dataJurosMora = Utils.FormatCode(boleto.DataVencimento.AddDays(1).ToString("ddMMyy"), "0", 6, true); //178 a 183 Data da mora
                    }
                    
                    _detalhe += dataJurosMora;//178 a 183 Data da mora
                }
                else
                {
                    _detalhe += "000000";// 178 a 183 Data da mora
                }


                /*184 a 184 - Campo de descontos (notas)
                    Se = 0, título não tem desconto
                    Se = 1, valor fixo até a data informada
                    Se = 2, valor por antecipação(dia corrido)
                    Se = 3, valor por antecipação(dia útil)
                    Se = 4, percentual sobre o valor nominal até a data
                        informada
                    Se = 5, percentual sobre o valor nominal(dia corrido)
                    Se = 6, Percentual sobre o valor nominal(dia útil)*/
                string CampoDeDescontos = "0";
                if (boleto.ValorDesconto > 0)
                {
                    CampoDeDescontos = "1";//'1'  =  Valor Fixo Até a Data Informada
                }
                else
                {
                    CampoDeDescontos = "0";//0, título não tem desconto
                } 

                /*
                     * Se = 0, título não tem desconto
                        Para códigos de desconto, vide (notas )
                     */
                _detalhe += CampoDeDescontos; //184 a 184 Campo de descontos

                /*
                 * Se 1, 2 ou 3 no campo 184. Do contrário,
                    preencher com zeros.
                 */
                if (CampoDeDescontos == "1" || CampoDeDescontos == "2" || CampoDeDescontos == "3")
                {
                    _detalhe += Utils.FitStringLength(boleto.ValorDesconto.ToString("0.00").Replace(",", ""), 13, 13, '0', 0, true, true, true);//185 a 197 Valor do desconto 1
                }
                else
                {
                    _detalhe += Utils.FitStringLength("0", 13, 13, '0', 0, true, true, true);//185 a 197 Valor do desconto 1
                }

                /*
                 * Se 4, 5 ou 6 no campo 184. Do contrário,
                    preencher com zeros
                 */
                if (CampoDeDescontos == "4" || CampoDeDescontos == "5" || CampoDeDescontos == "6")
                {
                    _detalhe += Utils.FitStringLength((Convert.ToUInt64(boleto.ValorDesconto) / 100).ToString(), 4, 4, '0', 0, true, true, true);//198 a 201 Percentual de desconto 1
                }
                else
                {
                    _detalhe += Utils.FitStringLength("0", 4, 4, '0', 0, true, true, true); //198 a 201 Percentual de desconto 1
                }

                // Data limite para desconto
                if (CampoDeDescontos != "0")
                {
                    _detalhe += boleto.DataDesconto.ToString("ddMMyy");//202 a 207 Data limite para concessão do desconto 1
                }
                else
                {
                    _detalhe += "000000";//202 a 207 Data limite para concessão do desconto 1
                }

                //Valor do Abatimento a ser concedido ou cancelado (13, N)
                _detalhe += Utils.FitStringLength(boleto.Abatimento.ToString("0.00").Replace(",", ""), 13, 13, '0', 0, true, true, true); //208 a 220 Valor do abatimento a ser  concedido

                /*
                 * 01 - Pessoa Fisica(CPF)
                 * 02 - Pessoa jurídica (CNPJ)
                 */
                _detalhe += pTipoSacado;//221 a 222 Identificação do tipo de inscrição do pagador

                /*
                 * CNPJ/CPF - Completar com zeros à esquerda
                    no caso de CPF
                 */
                _detalhe += Utils.FitStringLength(boleto.Sacado.CPFCNPJ, 14, 14, '0', 0, true, true, true).ToUpper();//223 a 236 Nº Inscrição do pagador


                _detalhe += Utils.FitStringLength(boleto.Sacado.Nome.TrimStart(' '), 40, 40, ' ', 0, true, true, false);//237 a 276 Nome do pagador

                _detalhe += Utils.FitStringLength(boleto.Sacado.Endereco.End.TrimStart(' '), 40, 40, ' ', 0, true, true, false).ToUpper();//277 a 316 Endereço completo

                _detalhe += Utils.FitStringLength(boleto.Sacado.Endereco.CEP, 8, 8, '0', 0, true, true, true).ToUpper();//317 a 321 CEP -- 322 a 324 Sufixo do CEP

                _detalhe += new string(' ', 70); //325 a 394 1ª Mensagem

                _detalhe += Utils.FitStringLength(numeroRegistro.ToString(), 6, 6, '0', 0, true, true, true);//395 a 400 Nº sequencial do registro

                _detalhe = Utils.SubstituiCaracteresEspeciais(_detalhe);

                return _detalhe;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
            }
        }

        

        public string GerarRegistroDetalhe2(Boleto boleto, int numeroRegistro)
        {
            throw new NotImplementedException("Função não implementada.");
        }

        # endregion DETALHE

        # region TRAILER CNAB240

        public override string GerarTrailerLoteRemessa(int numeroRegistro)
        {
            throw new NotImplementedException("Função não implementada.");
        }

        

        public override string GerarTrailerArquivoRemessa(int numeroRegistro)
        {
            throw new NotImplementedException("Função não implementada.");
        }
        #endregion

        # region TRAILER CNAB400

        /// <summary>
        /// TRAILER do arquivo CNAB
        /// Gera o TRAILER do arquivo remessa de acordo com o lay-out informado
        /// </summary>
        public override string GerarTrailerRemessa(int numeroRegistro, TipoArquivo tipoArquivo, Cedente cedente, decimal vltitulostotal, int qtdBoletos)
        {
            try
            {
                string _trailer = " ";

                base.GerarTrailerRemessa(numeroRegistro, tipoArquivo, cedente, vltitulostotal);

                switch (tipoArquivo)
                {
                    case TipoArquivo.CNAB240:
                        _trailer = GerarTrailerRemessa240();
                        break;
                    case TipoArquivo.CNAB400:
                        _trailer = GerarTrailerRemessa400(numeroRegistro, qtdBoletos);
                        break;
                    case TipoArquivo.Outro:
                        throw new Exception("Tipo de arquivo inexistente.");
                }

                return _trailer;

            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

        public string GerarTrailerRemessa240()
        {
            throw new NotImplementedException("Função não implementada.");
        }

        public string GerarTrailerRemessa400(int numeroRegistro, int qtdBoletos)
        {
            try
            {
                string _trailer;

                _trailer = "9";//001 a 001 Identificação registro
                _trailer += Utils.FitStringLength(qtdBoletos.ToString(), 6, 6, '0', 0, true, true, true);// 002 a 007 Quantidade de boletos
                _trailer += new string(' ', 387);//002 a 394 Branco
                _trailer += Utils.FitStringLength(numeroRegistro.ToString(), 6, 6, '0', 0, true, true, true); // 395 a 400 Número sequencial de registro

                _trailer = Utils.SubstituiCaracteresEspeciais(_trailer);

                return _trailer;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }

        # endregion

        #endregion

        #region Métodos de processamento do arquivo retorno CNAB400

        public override DetalheRetorno LerDetalheRetornoCNAB400(string registro)
        {
            try
            {
                int dataOcorrencia = Utils.ToInt32(registro.Substring(91, 6));
                int dataVencimento = Utils.ToInt32(registro.Substring(118, 6));
                int dataCredito = Utils.ToInt32(registro.Substring(172, 6));

                DetalheRetorno detalhe = new DetalheRetorno(registro);

                detalhe.CodigoInscricao = Utils.ToInt32(registro.Substring(1, 2));
                detalhe.NumeroInscricao = registro.Substring(3, 14);
                detalhe.Agencia = Utils.ToInt32(registro.Substring(23, 4));
                detalhe.Conta = Utils.ToInt32(registro.Substring(27, 9));
                detalhe.DACConta = Utils.ToInt32(registro.Substring(36, 1));
                detalhe.UsoEmpresa = registro.Substring(37, 25);
                //
                detalhe.NossoNumeroComDV = registro.Substring(70, 11);
                detalhe.NossoNumero = registro.Substring(70, 10); //Sem o DV
                detalhe.DACNossoNumero = registro.Substring(80, 1); //DV
                //
                detalhe.Carteira = registro.Substring(86, 3);
                detalhe.CodigoOcorrencia = Utils.ToInt32(registro.Substring(89, 2));

                //Descrição da ocorrência
                detalhe.DescricaoOcorrencia = this.Ocorrencia(registro.Substring(89, 2));

                detalhe.DataOcorrencia = Utils.ToDateTime(dataOcorrencia.ToString("##-##-##"));
                detalhe.NumeroDocumento = registro.Substring(97, 10);
                //
                detalhe.DataVencimento = Utils.ToDateTime(dataVencimento.ToString("##-##-##"));
                decimal valorTitulo = Convert.ToInt64(registro.Substring(124, 13));
                detalhe.ValorTitulo = valorTitulo / 100;
                detalhe.CodigoBanco = Utils.ToInt32(registro.Substring(137, 3));
                detalhe.AgenciaCobradora = Utils.ToInt32(registro.Substring(140, 4));
                detalhe.Especie = Utils.ToInt32(registro.Substring(144, 2));
              

                decimal valorPrincipal = Convert.ToUInt64(registro.Substring(159, 13));

                detalhe.ValorPrincipal = valorPrincipal / 100;

                detalhe.DataCredito = Utils.ToDateTime(dataCredito.ToString("##-##-##"));

                detalhe.NomeSacado = registro.Substring(181, 40);

                detalhe.NumeroInscricao = registro.Substring(226, 14);

                detalhe.Erros = registro.Substring(240, 140);

                detalhe.NumeroSequencial = Utils.ToInt32(registro.Substring(394, 6));
                detalhe.ValorPago = detalhe.ValorPrincipal;

                

                return detalhe;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        #endregion


        /// <summary>
        /// Efetua as Validações dentro da classe Boleto, para garantir a geração da remessa
        /// </summary>
        public override bool ValidarRemessa(TipoArquivo tipoArquivo, string numeroConvenio, IBanco banco, Cedente cedente, Boletos boletos, int numeroArquivoRemessa, out string mensagem)
        {
            bool vRetorno = true;
            string vMsg = string.Empty;
            ////IMPLEMENTACAO PENDENTE...
            mensagem = vMsg;
            return vRetorno;
        }

    }
}