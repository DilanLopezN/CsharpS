using Amazon;
using Simjob.Framework.Infra.Schemas.Models;
using System.IO;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Interfaces
{
    public interface IAWSStorageService
    {
        public bool Delete(string path);
        public Task<string> Upload(AWSModel config, string filename, Stream file, string contentType);
    }
}
