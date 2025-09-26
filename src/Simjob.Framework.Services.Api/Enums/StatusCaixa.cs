namespace Simjob.Framework.Services.Api.Enums
{
    public enum StatusCaixa
    {
        EmAberto = 0,
        AguardandoValidacao = 1,
        Fechado = 2,
        Rejeitado = 3,
        Reaberto = 4
    }
    
    public static class StatusCaixaExtensions
    {
        public static string GetDescription(this StatusCaixa status)
        {
            return status switch
            {
                StatusCaixa.EmAberto => "Em Aberto",
                StatusCaixa.AguardandoValidacao => "Aguardando Validação",
                StatusCaixa.Fechado => "Fechado",
                StatusCaixa.Rejeitado => "Rejeitado",
                StatusCaixa.Reaberto => "Reaberto",
                _ => "Status Desconhecido"
            };
        }
        
        public static bool IsAberto(this StatusCaixa status)
        {
            return status == StatusCaixa.EmAberto;
        }
        
        public static bool IsFechado(this StatusCaixa status)
        {
            return status == StatusCaixa.Fechado;
        }
    }
}
