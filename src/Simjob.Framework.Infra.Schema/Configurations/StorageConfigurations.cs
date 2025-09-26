namespace Simjob.Framework.Infra.Schemas.Configurations
{
    public class StorageConfigurations
    {
        public int LengthMaxMb { get; set; } = 10;
        public string AzureConnectionString { get; set; }
        public string AzureContainer { get; set; }
    }
}
