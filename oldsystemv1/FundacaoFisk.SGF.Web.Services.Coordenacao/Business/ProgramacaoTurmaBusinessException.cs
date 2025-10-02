using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Business {
    public class ProgramacaoTurmaBusinessException : BusinessException{
        public enum TipoErro {
            ERRO_PROGRAMACAO_SEM_HORARIO_OU_DATA = 1,
            ERRO_NAO_TEM_PROGRAMACAO_CURSO = 2,
            ERRO_MODIFICAR_DESCONSIDERA_FERIADO_AULA_EFETIVADA = 3
        }
        public TipoErro tipoErro;

        public ProgramacaoTurmaBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}
