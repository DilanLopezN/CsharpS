using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.CNAB.Business
{
    public class CnabBusinessException : BusinessException{
        public enum TipoErro {
            ERRO_NAO_GEROU_CNAB = 1,
            ERRO_NOSSO_NUMERO_NAO_INFORMADO = 2,
            ERRO_GERAR_REMESSA_CARTEIRA_SEM_REGISTRO = 3,
            ERRO_NAO_ALTERA_CNAB_GERADO = 4,
            ERRO_DATA_INI_MENOR_DATA_ATUAL = 5,
            ERRO_DATA_FINAL_MENOR_DATA_INICIAL = 6,
            ERRO_CAMPOS_BANCO_OBRIGATORIO = 7,
            ERRO_TITULO_STATUSCNAB_DIFERENTE_INCIAL = 8,
            ERRO_EXCLUIR_CNAB_FECHADO = 9,
            ERRO_CNAB_SEM_PESSOA_BANCO_CNPJ = 10,
            ERRO_CNAB_SEM_AGENCIA = 11,
            ERRO_CNAB_SEM_CONTA_CORRENTE = 12,
            ERRO_CNAB_SEM_DIGITO = 13,
            ERRO_CNAB_NRO_BANCO = 14,
            ERRO_TIPO_CNAB_CANCELAMENTO_CARTEIRA_REGISTRADA = 15,
            ERRO_NAO_GEROU_CANCELAMENTO_CNAB = 16,
            ERRO_PEDIDO_BAIXA_CARTEIRA_SEM_REGISTRO = 17,
            ERRO_PEDIDO_BAIXA_JA_GERADO = 18,
            ERRO_ALTERAR_EXCLUIR_CARTEIRA_HOMOLOGADA = 19,
            ERRO_RETORNO_COM_NOME_INFORMADO = 20,
            ERRO_RETORNO_JA_EXISTE = 21,
            ERRO_RETORNO_JA_FECHADO = 22,
            ERRO_CORRESPONDENCIA_BANCO = 23,
            ERRO_PROCESSAR_ARQUIVO_RETORNO = 24,
            ERRO_LIMITE_ARQUIVO_EXCEDIDO = 25,
            ERRO_TIPO_ARQUIVO_NAO_SUPORTADO = 26,
            ERRO_REGISTRO_NAO_ENCONTRADO = 27,
            ERRO_CAMPOS_ENDERECO_OBRIGATORIO = 28,
            ERRO_DATA_MIN_MAX_SMALLDATETIME = 29,
            ERRO_DT_EMISSAO_MAIOR_PROCESSAMENTO_CNAB = 30,
            ERRO_PERMISSAO_EXCLUIR_CNAB_REGISTRADO = 31,
            ERRO_GERAR_REMESSA_CD_CONTRATO_CNAB = 32,
            ERRO_BAIXA_DUPLICADA_ARQUIVO_RETORNO = 33,
            ERRO_CARACTERES_INVALIDOS_NOME_ARQUIVO = 34,
        }
        public TipoErro tipoErro;

        public CnabBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}
