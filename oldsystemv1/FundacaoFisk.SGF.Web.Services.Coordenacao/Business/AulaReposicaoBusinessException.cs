using System;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;

namespace FundacaoFisk.SGF.Services.Coordenacao.Business
{
    public class AulaReposicaoBusinessException : BusinessException
    {

        public enum TipoErro
        {
            ERRO_PROCEDURE_SP_VERIFICA_AULA_REPOSICAO = 1,
        }

        public AulaReposicaoBusinessException.TipoErro tipoErro;

        public AulaReposicaoBusinessException(string msg, Exception ex, AulaReposicaoBusinessException.TipoErro erro, bool stacktrace)
            : base(msg, ex, stacktrace)
        {
            tipoErro = erro;
        }
    }
}