using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Business
{
    public class SecretariaBusinessException : BusinessException
    {
         public enum TipoErro {
             ERRO_NAO_EXISTE_LAYOUT_NOME_CONTRATO = 1,
             ERRO_SEM_PERMISAO_DELETAR_NOME_CONTRATO = 2,
             ERRO_LAYOUT_JA_EXISTE = 3,
             ERRO_USUARIO_NAO_PODE_ESPECIALIZAR_LAYOUT_CONTRATO = 4,
             ERRO_DATA_MENOR_DESISTENCIA = 5,
             ERRO_DATA_FORA_INTERVALO_VALIDO = 6,
             ERRO_DESISTENCIA_POSTERIOR = 7,
             ERRO_HORARIO_LOGIN = 8,
             ERRO_LOCAL_MOVIMENTO_AUSENTE_PRE_MATRICULA = 9,
             ERRO_RECIBO_SEM_BAIXA = 10,
             ERRO_DESISTENCIA_TURMA_ENCERRADA = 11,
             ERRO_PARAMETRO_NAO_ECONTRADO = 12,
             ERRO_FOLLOW_UP_LIDO = 13,
             ERRO_FOLLOW_UP_USER_ORIGEM_DIFERENTE = 14,
             ERRO_FOLLOW_UP_DE_OUTRO_USUARIO = 15,
             ERRO_FOLLOW_LIMITE_DC_ASSUNTO = 16,
             ERRO_EMAIL_INVALIDO = 17,
             ERRO_DESISTENCIA_DT_RETROATIVA_MAT = 18,
             ERRO_MUDANCA_TURMA_DT_RETROATIVA_MAT = 19,
             ERRO_EXTENSAO_CONTRATRO_DIGITALIZADO_NAO_VALIDA = 20,
             ERRO_ALUNOS_TURMA_IN_TURMA_DESTINO = 21,
             ERRO_OBJ_TRANFERENCIA_ALUNO_NULL = 22,
             ERRO_CD_TRANSFERENCIA_MENOR_IGUAL_ZERO = 23,
             ERRO_CD_ESCOLA_DESTINO_MENOR_IGUAL_ZERO = 24,
             ERRO_DC_EMAIL_ORIGEM_NULO_OR_EMPTY = 25,
             ERRO_DC_EMAIL_DESTINO_NULO_OR_EMPTY = 26,
             ERRO_ID_STATUS_TRANSFERENCIA_MENOR_IGUAL_ZERO = 27,
             ERRO_CD_MOTIVO_TRANSFERENCIA_MENOR_IGUAL_ZERO = 28,
             ERRO_DELETE_TRANSFERENCIA_EFETUADA = 29,
             ERRO_EDIT_TRANSFERENCIA_EFETUADA = 30,
             ERRO_TRANSFERENCIA_ALUNO_ENVIAR_EMAIL_NULO = 31,
             ERRO_TRANSFERIR_ALUNO_STATUS_DIFERENTE_APROVADO = 32,
             ERRO_ENVIAR_EMAIL_SOLICITACAO_STATUS_DIFERENTE_CRIADA_SOLICITADA = 33,
             ERRO_STATUS_TRANSFERENCIA_NAO_ALTERADO = 34,
             ERRO_ENVIO_EMAIL_STATUS_DIRERENTE_APROVADO_RECUSADO = 35,
             ERRO_TRANSFERENCIA_ALUNO_EXISTE_DESTINO = 36,
             ERRO_EXCLUIR_DESISTENCIA = 37,

        }
        public TipoErro tipoErro;

        public SecretariaBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}
