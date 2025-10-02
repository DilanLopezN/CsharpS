using Componentes.GenericBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Business
{
    public class CursoBusinessException : BusinessException
    {

        public enum TipoErro
        {
            ERRO_EXISTES_CURSO_TURMA = 1,
            ERRO_NOT_EXISTS_PROXIMO_CURSO = 2
        }
        public TipoErro tipoErro;

        public CursoBusinessException(string msg, Exception ex, TipoErro erro,bool stacktrace )
            : base(msg, ex, stacktrace)
        {
            tipoErro = erro;
        }
    }
}
