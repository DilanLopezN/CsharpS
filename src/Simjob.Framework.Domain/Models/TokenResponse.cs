using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Simjob.Framework.Domain.Models
{
    public class TokenResponse
    {
        public TokenResponse(string tenanty, string userId, string username, string user,
            List<Claim> claims, DateTime created, DateTime expiration, string accessToken, int a2f, string groupId, string telefone, bool root, bool controlAccess, string version, bool firstLogin, string revendaId, string nivelId, string decryptedKey, string companySiteIdDefault, string companySiteIds, string cd_pessoa, string cd_usuario, bool usuarioMatriz)
        {
            Tenanty = tenanty;
            UserId = userId;
            Username = username;
            User = user;
            Telefone = telefone;
            Claims = claims;
            Created = created.ToString("yyyy-MM-ddHH:mm:ss");
            Expiration = expiration.ToString("yyyy-MM-ddHH:mm:ss");
            AccessToken = accessToken;
            A2f = a2f;
            GroupId = groupId;
            Root = root;
            ControlAccess = controlAccess;
            Version = version;
            FirstLogin = firstLogin;
            RevendaId = revendaId;
            NivelId = nivelId;
            ApiKey = decryptedKey;
            CompanySiteIdDefault = companySiteIdDefault;
            CompanySiteIds = companySiteIds;
            Cd_pessoa = cd_pessoa;
            Cd_usuario = cd_usuario;
            UsuarioMatriz = usuarioMatriz;
        }
        public TokenResponse(string userId, DateTime created, DateTime expiration, string accessToken)
        {
            UserId = userId;
            Created = created.ToString("yyyy-MM-ddHH:mm:ss");
            Expiration = expiration.ToString("yyyy-MM-ddHH:mm:ss");
            AccessToken = accessToken;
        }
        public string Tenanty { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string User { get; set; }
        public string Telefone { get; set; }
        public List<Claim> Claims { get; set; }
        public object Modules { get; set; }
        public string Created { get; set; }
        public string Expiration { get; set; }
        public string AccessToken { get; set; }
        public int A2f { get; set; }
        public string GroupId { get; set; }
        public bool Root { get; set; }
        public bool ControlAccess { get; set; }
        public string Version { get; set; }
        public bool FirstLogin { get; set; }
        public string RevendaId { get; set; }
        public string NivelId { get; set; }
        public string CompanySiteIdDefault { get; set; }
        public string CompanySiteIds { get; set; }
        public string ApiKey { get; set; }
        public string Cd_pessoa { get; set; }
        public string Cd_usuario { get; set; }
        public bool UsuarioMatriz { get; set; }

    }
}
