using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Business
{
    public class CoordenacaoBusinessException : BusinessException
    {
        public enum TipoErro
        {
            VALIDACAO_DATAS_FERIADO = 1,
            SALA_NAO_DISPONIVEL = 2,
            ALUNOS_ATIVIDADE_EXTRA = 3,
            NUMERO_VAGAS_ATIVIDADE = 4,
            FERIDADO_ESCOLA = 5,
            PROGRAMACAO_CURSO_DADOS_OBRIGATORIOS = 6,
            INTERSECAO_HORARIOS_DIARIO_AULA = 7,
            DIARIO_AULA_JA_CANCELADO = 8,
            NAO_EXCLUIR_DIARIO_CANCELADO = 9,
            NAO_POSSIVEL_ALTERACAO_DIARIO_CANCELADO = 10,
            ERRO_ALUNO_COM_AVALIACAO = 11,
            EXISTE_DESISTENCIA = 12,
            PERIODO_EXISTENTE = 13,
            DIARIO_SEM_ALUNO_MATRICULADO = 14,
            ERRO_DIARIO_TURMA_ENCERRADA = 15,
            ERRO_DIARIO_SEM_PROFESSOR = 16,
            ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR =  17,
            ERRO_DATA_VENCIMENTO_TITULO_INFERIOR = 18,
            ERRO_DATA_DIARIO_MAIOR_DATA_ATUAL = 19,
            ERRO_AULA_PERSONALIZADA_SEM_ALUNO = 20,
			ERRO_MUDANCAO_SEM_CURSO = 21,
            ERRO_CONCEITO_MAIOR = 22,
            ERRO_SOMA_CONCEITO_MAIOR = 23,
            ERRO_AVALIACAO_PARTICIPACAO_SEM_VINC = 24,
            ERRO_AVALIACAO_PARTICIPACAO_NOTA = 25,
			ERRO_NAO_INFORMADO_HR_INICIAL_FINAL = 26,
            ERRO_MIN_HR_INICIAL_FINAL = 27,
            ERRO_INTERVALO_HR_INICIAL_FINAL = 28,
            ERRO_IS_PARTICIPACAO_AVALIACAO = 29,
			ERRO_DT_EMISSAO_TITULO_MAIOR_VENCIMENTO = 30,
			ERRO_MOVIMENTO_TURMA_ENCERRADA = 31,
            ERRO_DELETAR_AVALIACAO_PARTICIPACAO = 32,
            ERRO_DT_FIM_MENOR_DT_INICIO = 33,
            ERRO_HH_FIM_MENOR_HH_INICIO = 34,
            ERRO_VIDEO_NAO_ENCONTRADO = 35,
            ERRO_NAO_EXISTE_IMAGEM = 36,
            ERRO_VALIDACAO_CIRCULAR = 37,
            ERRO_STATUS_FERIADO = 38,
            ERRO_CAD_FERIADO_INATIVO = 39,
            ALUNOS_AULA_REPOSICAO = 40,
            ERRO_EXCLUIR_SALA = 41,
            PROSPECT_ATIVIDADE_EXTRA = 42,
            ERRO_ATIVIDADE_EXTRA_RECORRENCIA_NULO = 43,
            ERRO_ATIVIDADE_EXTRA_RECORRENCIA_ID_ZERO = 44,
            ERRO_ATIVIDADE_EXTRA_NOT_FOUND = 45,
            ERRO_ATIVIDADE_RECORRENCIA_NULO = 46,
            ERRO_DATA_ATIVIDADE_EXTRA_MAIOR_DATA_LIMITE_RECORRENCIA = 46,
            ERRO_NENHUM_ALUNO_PROSPECT_ENCONTRADO_SEND_MAIL = 47,
            ERRO_ALTERAR_DIARIO_OUTRA_ESCOLA = 48,
            ERRO_NENHUM_PROSPECT_ENCONTRADO_SEND_MAIL = 49,
            ERRO_ATIVIDADE_EXTRA_ENVIAR_EMAIL_NULO = 50,
            ERRO_ATIVIDADE_RECORRENCIA_ENVIAR_EMAIL_NULO = 51,
            ERRO_EDICAO_PERDA_MATERIAL_FECHADO = 52,
            ERRO_EXCLUSAO_PERDA_MATERIAL_FECHADO = 53,
            ERRO_PROCESSAR_PERDA_MATERIAL_FECHADO = 54,
            ERRO_IMPRIMIR_CONTRATO = 55
        }
        public TipoErro tipoErro;

        public CoordenacaoBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}
