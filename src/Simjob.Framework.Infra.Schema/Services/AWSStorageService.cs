using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Infra.Schemas.Models;

namespace Simjob.Framework.Infra.Schemas.Services
{
    public class AWSStorageService : IAWSStorageService
    {
        public bool Delete(string path)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Upload(AWSModel config, string filename, Stream file,string contentType)
        {
            try
            {
                var regions = RegionEndpoint.EnumerableAllRegions;
                IAmazonS3 client = new AmazonS3Client(config.AccessKey, config.SecretAccessKey, regions.Where(x => x.SystemName == config.RegionEndPoint).FirstOrDefault());
                PutObjectRequest request = new PutObjectRequest()
                {
                    InputStream = file,
                    BucketName = config.Name,
                    Key = filename,
                    ContentType = contentType
                };
                
                PutObjectResponse response = await client.PutObjectAsync(request);
                return "https://" + config.Name + ".s3." + config.RegionEndPoint + ".amazonaws.com/" + filename;
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
