using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using OBS;
using OBS.Model;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Infra.Schemas.Models;

namespace Simjob.Framework.Infra.Schemas.Services
{
    public class HuaweiStorageService : IHuaweiStorageService
    {
        public bool Delete(string path)
        {
            throw new NotImplementedException();
        }

        public string Upload(HuaweiModel config, string path, Stream image)
        {
            var obsConfig = new ObsConfig();
            obsConfig.Endpoint = config.EndPoint;
            var client = new ObsClient(config.AccessKey, config.SecretAccessKey, obsConfig);

            UploadStreamRequest request = new UploadStreamRequest();

            request.ObjectKey = path;
            request.BucketName = config.Name;
            request.UploadStream = image;
   

            try
            {
                var res = client.UploadStream(request);
                return res.ObjectUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
