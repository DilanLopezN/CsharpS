namespace Simjob.Framework.Services.Api.Middleware
{
    public interface IApiKeyValidation
    {
        bool IsValidApiKey(string userApiKey);
    }
}
