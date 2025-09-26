using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;

namespace Simjob.Framework.Domain.Core.Utils
{
    public static class Util
    {
        public static dynamic GetUserInfoFromToken(StringValues accesstoken)
        {
            var dic = new Dictionary<string, string>();
            if (accesstoken.Count > 0)
            {
                var token = accesstoken.ToString().Split(" ");
                string onlyToken = "";
                foreach (var itens in token) { if (itens.Length > 10) onlyToken = itens; }
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(onlyToken);
                var claims = jwtSecurityToken.Claims.ToList();

                dic.Add("username", claims.Where(x => x.Type == "unique_name").FirstOrDefault().Value.ToString());
                dic.Add("userid", claims.Where(x => x.Type == "userid").FirstOrDefault().Value.ToString());
                dic.Add("tenanty", claims.Where(x => x.Type == "tenanty").FirstOrDefault().Value.ToString());
                dic.Add("cd_pessoa", claims.Where(x => x.Type == "cd_pessoa").FirstOrDefault()?.Value.ToString()??"");
                dic.Add("companySiteIdDefault", claims.Where(x => x.Type == "companySiteIdDefault").FirstOrDefault().Value.ToString());
                //dic.Add("companySiteIds", claims.Where(x => x.Type == "companySiteIds").FirstOrDefault().Value.ToString());
                dic.Add("root", claims.Where(x => x.Type == "root").FirstOrDefault()?.Value.ToString()??"");
            }

            return dic;
        }

        public static dynamic GetEmailFromToken(StringValues accesstoken)
        {
            var dic = new Dictionary<string, string>();
            if (accesstoken.Count > 0)
            {
                var token = accesstoken.ToString().Split(" ");
                string onlyToken = "";
                foreach (var itens in token) { if (itens.Length > 10) onlyToken = itens; }
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(onlyToken);
                var claims = jwtSecurityToken.Claims.ToList();

                dic.Add("username", claims.Where(x => x.Type == "unique_name").FirstOrDefault().Value.ToString());
            }

            return dic;
        }

        public static bool IsExcelFile(IFormFile file)
        {
            var extensoesPermitidas = new[] { ".xls", ".xlsx" };
            var extensao = Path.GetExtension(file.FileName).ToLowerInvariant();
            return !string.IsNullOrEmpty(extensao) && extensoesPermitidas.Contains(extensao);
        }

        public static ArquivoMemoryStream? GetArquivoMemoryStream(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) return null;

            var contentTypeAceitos = new List<string>
            {
                "image/png",
                "image/jpeg",
                "application/pdf",
                "@file/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };

            try
            {
                var base64Data = base64.Split(',');
                var base64String = base64Data.Length > 1 ? base64Data[1] : base64Data[0];

                byte[] fileBytes = Convert.FromBase64String(base64String);
                var memoryStream = new MemoryStream(fileBytes);
                string contentType = base64Data.Length > 1
                    ? base64Data[0].Replace("data:", "").Split(';')[0]
                    : "application/octet-stream";

                if (!contentTypeAceitos.Contains(contentType)) return null;

                string fileExtension = contentType switch
                {
                    "image/png" => "png",
                    "image/jpeg" => "jpg",
                    "application/pdf" => "pdf",
                    "@file/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => "xlsx",
                    _ => "bin"
                };

                return new ArquivoMemoryStream
                {
                    MemoryStream = memoryStream,
                    ContentType = contentType,
                    FileExtension = fileExtension
                };
            }
            catch
            {
                return null;
            }
        }

        public class ArquivoMemoryStream
        {
            public MemoryStream MemoryStream { get; set; } = null!;
            public string ContentType { get; set; } = null!;
            public string FileExtension { get; set; } = null!;
        }

    }
}