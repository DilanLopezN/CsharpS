using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Services
{
    /// <summary>
    /// Serviço responsável por validar se a data de baixa não é anterior à data de emissão do título
    /// </summary>
    public static class ValidacaoDataEmissaoService
    {
        /// <summary>
        /// Valida se a data de baixa é posterior ou igual à data de emissão do título
        /// </summary>
        /// <param name="titulo">Dicionário contendo os dados do título</param>
        /// <param name="dataBaixa">Data da baixa que será realizada</param>
        /// <returns>Tupla com sucesso (bool) e mensagem de erro (string)</returns>
        public static (bool sucesso, string mensagemErro) ValidarDataBaixaVsEmissao(
            Dictionary<string, object> titulo,
            DateTime dataBaixa)
        {
            try
            {
                // Verificar se o título possui data de emissão
                if (!titulo.ContainsKey("dt_emissao_titulo") || titulo["dt_emissao_titulo"] == null)
                {
                    // Se não há data de emissão, permite a baixa
                    return (true, null);
                }

                DateTime dataEmissao = Convert.ToDateTime(titulo["dt_emissao_titulo"]);

                // Validar se a data de baixa é anterior à data de emissão
                if (dataBaixa.Date < dataEmissao.Date)
                {
                    string mensagem = $"Não é possível realizar a baixa com data ({dataBaixa:dd/MM/yyyy}) anterior à data de emissão do título ({dataEmissao:dd/MM/yyyy}).";
                    return (false, mensagem);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                // Em caso de erro na validação, registra mas permite a operação
                // para não bloquear o sistema por falha na validação
                Console.WriteLine($"Erro ao validar data de baixa vs emissão: {ex.Message}");
                return (true, null);
            }
        }
    }
}