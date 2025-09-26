using Simjob.Framework.Infra.Schemas.Models;
using System.IO;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Interfaces
{
    public interface IAzureStorageService
    {
        public bool Delete(string path);
        public Task<string> Upload(Stream stream, string nomeArquivo, AzureModel config,string contentType);
    }
}
