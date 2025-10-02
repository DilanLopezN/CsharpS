using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Business {
    public class FuncionarioBusinessException : BusinessException{
        public enum TipoErro {
            ERRO_FUNCIONARIOJAEXISTE = 1,
            ERRO_USUARIO_NAO_FUNCIONARIO = 2,
            ERRO_HORARIO_FORA_INTERVALO_OCUPADO_TURMA = 3,
            ERRO_PERMISSAO_SALARIO = 4,
            ERRO_FUNCIONARIO_AVALIACAO_OBRIGATORIO = 5,
            ERRO_FUNCIONARIO = 6,
            ERRO_CERTIFICADO_JA_EXISTE = 7,
            ERRO_TRIGGER_FUNCIONARIO_PROFESSOR = 8,
            ERRO_NOME_COM_ASTERISCO = 9

        }
        public TipoErro tipoErro;

        public FuncionarioBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}
