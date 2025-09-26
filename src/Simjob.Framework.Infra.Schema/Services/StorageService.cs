using Azure.Storage.Blobs;
using MediatR;
using Microsoft.Extensions.Configuration;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Infra.Schemas.Configurations;
using System;
using System.IO;
using System.Net.Http;

namespace Simjob.Framework.Infra.Schemas.Services
{
    public class StorageService
    {
        public StorageService()
        {

        }

        public static Stream Base64ToStream(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            return new MemoryStream(bytes);
        }
    }
}