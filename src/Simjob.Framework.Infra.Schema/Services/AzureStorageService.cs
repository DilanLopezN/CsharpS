using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Simjob.Framework.Infra.Schemas.Configurations;
using Simjob.Framework.Infra.Schemas.Interfaces;
using Simjob.Framework.Infra.Schemas.Models;
using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Services
{
    public class AzureStorageService : StorageService, IAzureStorageService
    {
        public bool Delete(string path)
        {
            //if (!AzureIsReadyValidation()) return false;
            //try
            //{
            //    string nomeArquivoBlob = Path.GetFileName(path);
            //    var response = _blobContainerClient.DeleteBlobIfExists(nomeArquivoBlob);
            //    return true;
            //}
            //catch (Exception e)
            //{
            //    //SendDomainNotification( $"Delete Error: {e.Message}", "StorageService");
            //    return false;
            //}
            return default;
        }

        public async Task<string> Upload(Stream stream, string nomeArquivo, AzureModel config,string contentType)
        {
            BlobContainerClient blobContainerClient = new BlobContainerClient(config.AzureConnectionString, config.AzureContainer);
            string path = $"{blobContainerClient.Uri.AbsoluteUri}/{nomeArquivo}";
            try
            {
                blobContainerClient.CreateIfNotExists(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

                var blobClient = blobContainerClient.GetBlobClient(nomeArquivo);
                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                };
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });

                //await blobContainerClient.UploadBlobAsync(nomeArquivo, stream);
                return path;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Status: 409")) return path;
                //SendDomainNotification($"Upload error: {e.Message}", "StorageService");
                return null;
            }
        }

        private byte[] ConvertStreamToByteArray(Stream stream)
        {
            byte[] byteArray = new byte[16 * 1024];
            using (MemoryStream mStream = new MemoryStream())
            {
                int bit;
                while ((bit = stream.Read(byteArray, 0, byteArray.Length)) > 0)
                {
                    mStream.Write(byteArray, 0, bit);
                }
                return mStream.ToArray();
            }
        }

        public byte[] DownloadToBytes(string path)
        {
            //try
            //{
            //    string nomeArquivoBlob = Path.GetFileName(path);
            //    var blobClient = _blobContainerClient.GetBlobClient(nomeArquivoBlob);
            //    var response = blobClient.Download();
            //    Stream streamDownload = response.Value.Content;
            //    var bytesArquivo = ConvertStreamToByteArray(streamDownload);
            //    return bytesArquivo;
            //}
            //catch (Exception e)
            //{
            //    //SendDomainNotification( $"Download Error: {e.Message}", "StorageService");
            //    return null;
            //}
            return default;
        }

        public string DownloadToBase64(string path)
        {
            //try
            //{
            //    string nomeArquivoBlob = Path.GetFileName(path);
            //    var blobClient = _blobContainerClient.GetBlobClient(nomeArquivoBlob);
            //    var response = blobClient.Download();
            //    Stream streamDownload = response.Value.Content;
            //    var bytesArquivo = ConvertStreamToByteArray(streamDownload);
            //    var base64Arquivo = Convert.ToBase64String(bytesArquivo);
            //    return base64Arquivo;
            //}
            //catch (Exception e)
            //{
            //    //SendDomainNotification($"Download Error: {e.Message}", "StorageService");
            //    return null;
            //}
            return default;
        }
    }
}
