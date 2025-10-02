using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Business
{
    public class MatriculaBusinessException : BusinessException{
        public enum TipoErro {
            ERRO_APENAS_UM_ALUNO_TURMA_AGUARDANDO = 1,
            ERRO_ALUNO_TURMA_CONTRATO = 2,
            ERRO_VALOR_CONTRATO_ZERO = 3,
            ERRO_VALOR_MATRICULA_ZERO = 4,
            ERRO_VALOR_CURSO_LIQUIDO = 5,
            ERRO_VALOR_PARCELA_MAIOR_PRIMEIRA = 6,
            ERRO_NAO_EXISTE_LAYOUT_NOME_CONTRATO = 7,
            ERRO_SUM_TITULOS_MENS_DIFERENTE_FATURAR = 8,
            ERRO_DIA_VENC_ADITA_NAO_INFORMADO = 9,
            ERRO_NRO_CONTRATO_NULO = 10,
            ERRO_TURMA_ENCERRADA = 11,
            ERRO_EXISTE_MAT_PRODUTO = 12,
            ERRO_DIFERENCA_TITULOS_CURSO_MAIOR_UMREAL = 13,
            ERRO_MIN_DATE_MATRICULA = 14,
            ERRO_CONFIGURACAO_DIA_IMPRESSAO_CONTRATO = 15,
            MATRICULA_NAO_ENCONTRADA = 16,
            ERRO_MATERIAL_MENSALIDADES = 17,
		    ERRO_CALCULOS_ADITAMENTO = 18,
            ERRO_VALIDACAO_ADITAMENTO = 19,
            ERRO_EXISTE_ADITAMENTO_BAIXO = 20,
            ERRO_NAO_PERMITE_EXCLUIR_ADITAMENTO_DIFERENTE_ADICIONAR_PARCELA = 21,
            ERRO_EXCLUSAO_REGISTRO = 22,
            ERRO_CURSOS_NAO_VINCULADOS_MATRICULA_COM_TURMA = 23,
            ERRO_EXTENSAO_CONTRATRO_DIGITALIZADO_NAO_VALIDA = 24,
            ERRO_TIPO_FINANCEIRO_DIFERENTE_CARTAO_CHEQUE = 25,
            ERRO_TITULOS_RECIBO_CONFIRMACAO_NOT_FOUND = 26,
            ERRO_PROCEDURE_GERAR_NOTA = 27
        }

        public TipoErro tipoErro;

        public MatriculaBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}
