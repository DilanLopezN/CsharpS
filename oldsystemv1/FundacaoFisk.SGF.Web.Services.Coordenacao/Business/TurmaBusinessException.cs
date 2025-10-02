using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Business
{
    public class TurmaBusinessException : BusinessException
    {
        public enum TipoErro
        {
            ERRO_NAO_EXISTE_ALUNO_TURMA = 1,
            ERRO_NAO_EXISTE_AVALIACAO_TURMA = 2,
            ERRO_EXISTE_DIARIO_AULA_TURMA = 3,
            ERRO_EXISTE_NOTA_LANCADA = 4,
            ERRO_EXISTE_DIARIO_AULA_EFETUADO_PROGRAMACAO = 5,
            ERRO_SOMATORIO_NOTA_MAXIMA = 6,
            ERRO_EXISTETURMAATIVA = 7,
            ERRO_NUMERO_VAGAS = 8,
            ERRO_DATA_NULL = 9,
            ERRO_HORARIOS_FILHAS_INTERCESSAO_HORARIOS_PPT = 10,
            ERRO_TURMA_ENCERRADA = 11,
            ERRO_DELETAR_ALUNOTURMA_MATRICULADO_REMATRICULADO = 12,
            ERRO_DELETAR_ALUNOTURMA_AGUARDANDO_MOVIDO = 13,
            ERRO_DELETAR_TURMA_ALUNO_COM_CONTRATO = 14,
            ERRO_DELETAR_NOTA_FUNCIONARIO = 15,
            ERRO_HORARIOS_DISPONIVEIS_TURMA_SALA = 16,
            ERRO_PROGRAMACOES_EXCEDIDAS = 17,
            ERRO_NAO_EXISTE_PROX_CURSO = 18,
            ERRO_MODELO_PROGRAMACAO_TURMA_PPT_PAI = 19,
            ERRO_MODELO_PROGRAMACAO_TURMA_EXISTENTE = 20,
            ERRO_TURMA_SEM_PROGRAMACAO_TURMA = 21,
            ERRO_EXISTE_ALUNO_MAT_TURMA_NOVA = 22,
            ERRO_ALUNO_TURMA_ENC_OCUPADO_HORARIO = 23,
            ERRO_ALUNO_TURMA_ENC_OCUPADO_PRODUTO = 24,
            ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA =25,
            ERRO_NAO_EXISTE_ALUNO_ATIVO_PPT_FILHO = 26,
            ERRO_PROFESSOR_SEM_HORARIO_OCUPADO_TURMA = 27,
            ERRO_TURMA_NULO = 28,
            ERRO_PROCEDURE_GERAR_TURMAS_NULAS = 29,
            ERRO_EXISTE_PROGRAMACAO_TURMA = 30,
			ERRO_EXISTE_CONTRATO_COM_CONTRATO_ANTERIOR_IGUAL_CONTRATO_ALUNO_TURMA = 31,
            ERRO_TRIGGER_ALTERACAO_TURMA = 32,
            ERRO_ALTERAR_HORARIO = 33,
            ERRO_TURMA_NAO_ENCONTRADA = 34,
            ERRO_CARGA_HORARIA_MINIMA = 35,
            ERRO_EXISTE_DIARIO_POS_MUDANCA = 36,
            ERRO_EXISTE_PROGRAMACAO_INSUFICIENTE = 37
        }
        public TipoErro tipoErro;

        public TurmaBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}
