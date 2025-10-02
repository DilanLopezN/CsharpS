using System;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Business
{
    public class ApiNewCyberFuncionarioException : BusinessException
    {
        public enum TipoErro
        {
            ERRO_SEND_EXECUTA_CYBER = 1,
            ERRO_PARAMETROS_EXECUTA_CYBER = 2,
            ERRO_COMANDO_EXECUTA_CYBER = 3,
            ERRO_CHAVE_EXECUTA_CYBER = 4,
            ERRO_PARAMETROS_NULOS = 5,
            ERRO_NM_INTEGRACAO_NULO_OU_MENOR_IGUAL_ZERO = 6,
            ERRO_PESSOA_JURIDICA_NULA_OU_VAZIA = 7,
            ERRO_EMAIL_PESSOA_JURIDICA_NULA_OU_VAZIA = 8,
            ERRO_CIDADE_NULA_OU_VAZIA = 9,
            ERRO_ESTADO_NULO_OU_VAZIO = 10,
            ERRO_COD_ALUNO_MENO_IGUAL_ZERO = 11,
            ERRO_EMAIL_PESSOA_FISICA_NULA_OU_VAZIA = 12,
            ERRO_NOME_ALUNO_NULO_VAZIO = 13,



        }

        public TipoErro tipoErro;

        public ApiNewCyberFuncionarioException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}