using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Models
{
    public class AWSModel
    {
        public AWSModel(string name, string typeBucket, string accessKey, string secretAccessKey, string regionEndPoint, string folder)
        {
            Name = name;
            TypeBucket = typeBucket;
            AccessKey = accessKey;
            SecretAccessKey = secretAccessKey;
            RegionEndPoint = regionEndPoint;
            Folder = folder;
        }

        public string Name { get; set; }
        public string TypeBucket { get; set; }
        public string AccessKey { get; set; }
        public string SecretAccessKey { get; set; }
        public string RegionEndPoint { get; set; }
        public string Folder { get; set; }
    }

    public class AzureModel
    {
        public AzureModel(string name, string typeBucket, string azureConnectionString, string azureContainer, string lengthMaxMb)
        {
            Name = name;
            TypeBucket = typeBucket;
            AzureConnectionString = azureConnectionString;
            AzureContainer = azureContainer;
            LengthMaxMb = lengthMaxMb;
        }
        public string Name { get; set; }
        public string TypeBucket { get; set; }
        public string AzureConnectionString { get; set; }
        public string AzureContainer { get; set; }
        public string LengthMaxMb { get; set; }
    }

    public class HuaweiModel
    {
        public HuaweiModel(string name, string typeBucket, string endPoint, string accessKey, string secretAccessKey, string folder)
        {
            Name = name;
            TypeBucket = typeBucket;
            EndPoint = endPoint;
            AccessKey = accessKey;
            SecretAccessKey = secretAccessKey;
            Folder = folder;
        }

        public string Name { get; set; }
        public string TypeBucket { get; set; }
        public string EndPoint { get; set; }
        public string AccessKey { get; set; }
        public string SecretAccessKey { get; set; }
        public string Folder { get; set; }
    }

    public class StreamFileToUpload
    {
        public string FilePath { get; set; }
        public Stream File { get; set; }
        public string ContentType { get; set; }
    }
}
