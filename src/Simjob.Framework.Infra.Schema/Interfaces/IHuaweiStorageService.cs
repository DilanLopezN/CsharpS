using Simjob.Framework.Infra.Schemas.Models;
using System.IO;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Interfaces
{
    public interface IHuaweiStorageService
    {
        public bool Delete(string path);
        public string Upload(HuaweiModel config, string path, Stream image);
    }
}
