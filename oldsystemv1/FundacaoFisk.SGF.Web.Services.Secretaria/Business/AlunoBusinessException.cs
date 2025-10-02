using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Business
{
    public class AlunoBusinessException : BusinessException{
        public enum TipoErro {
            ERRO_ALUNOJAEXISTE = 1,
            ERRO_EMAIL_JA_EXITE = 2,
            ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA = 3,
            ERRO_ALUNO_COM_DIARIO = 4,
            ERRO_TURMA_POSSUI_ALUNO = 5,
            ERRO_ALUNO_HORARIO_OCUPADO = 6,
            ERRO_ALUNO_MATRICULADO_TURMA_ATUAL = 7,
            ERRO_ALUNO_STATUS_NAO_DESISTENTE = 8,
            ERRO_ALUNO_ESTA_DESISTENTE = 9,
            ERRO_ALUNO_ESTA_CANCELADO = 10,
            ERRO_INATIVAR_ALUNO_ATIVO = 11,
            ERRO_FOLLOW_UP_DE_OUTRO_ATENDENTE = 12,
            ERRO_PESSOAEMPRESAJAEXISTE = 13,
            ERRO_ALUNO_TURMA_ENC_OCUPADO_PRODUTO = 14,
            ERRO_PERCENTUAL_BOLSA_ALUNO = 15,
            ERRO_ALUNO_ESTA_NAO_REMATRICULADO = 16,
            ERRO_DATA_INICIO_ALUNO_TURMA = 17,
            ERRO_FOLLOW_LIMITE_DC_ASSUNTO = 18,
            ERRO_EMAIL_INVALIDO = 19,
            ERRO_NOME_COM_ASTERISCO = 20,
            ERRO_CPF_NULO_NAO_ITERNACIONAL = 21
        }
        public TipoErro tipoErro;

        public AlunoBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}
