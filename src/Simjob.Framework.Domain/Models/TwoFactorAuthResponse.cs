using System;
using System.Collections.Generic;
using System.Security.Claims;


namespace Simjob.Framework.Domain.Models
{
    public class TwoFactorAuthResponse
    {
        public TwoFactorAuthResponse(string tenanty, string userId, string hash, int autenticado, DateTime created)
        {
            Tenanty = tenanty;
            UserId = userId;
            Hash = hash;
            Autenticado = autenticado;
            Created = created.ToString("yyyy-MM-ddHH:mm:ss");
        }

        public string Tenanty { get; set; }
        public string UserId { get; set; }
        public string Hash { get; set; }
        public int Autenticado { get; set; }
        public string Created { get; set; }

    }
}
