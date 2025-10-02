using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Business
{
    public class EscolaBusinessException : BusinessException{
        public enum TipoErro {
            ERRO_ALUNOMATESCOLA = 1,
            ERRO_NIVEL_PARAMETRO = 2,
            ERRO_DIA_VENCIMENTO_PARAMETRO = 3,
            ERRO_NIVEIS_PLANO_CONTAS = 4,
            ERRO_EXISTE_AVALICAO = 5,
            ERRO_EXISTE_DIARIO_AULA = 6,
            ERRO_USUARIO_SEM_PERMISSAO = 7,
            ERRO_HORARIO_OCUPADO = 8,
            ERRO_CODIGO_FRANQUIA_INEXISTENTE = 9,
            ERRO_CAMPOS_OBRIGATORIOS = 10,
            ERRO_NAO_EXISTE_UF_PESSOA = 11,
            ERRO_NAO_EXISTE_UF_ESCOLA = 12,
            DATA_INVALIDA = 13,
            ERRO_N_EXISTE_CHEQUE = 14,
            ERRO_BAIXA_CARTAO_TIPO_FINANCEIRO = 15,
            ERRO_BAIXA_CARTAO_LOCALMOVT_DIFERENTE = 16,
            ERRO_OPCAO_INVALIDA = 17
        }
        public TipoErro tipoErro;

        public EscolaBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}
