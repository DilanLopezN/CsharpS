using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Auth.Model;
using FundacaoFisk.SGF.Web.Services.Auth.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using System.Collections.Generic;
using Componentes.Utils.Messages;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using log4net;
using System.Transactions;
using Componentes.GenericBusiness.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Auth.Business
{
    public class AuthBusiness : IAuthBusiness {
        private static readonly ILog logger = LogManager.GetLogger(typeof(AuthBusiness));

        private const int expirationTimeSec = 43200; //Tempo de expiração do cookie, em segundos

        private IPermissaoBusiness PermissaoBusiness { get; set; }
        private IUsuarioBusiness UsuarioBusiness { get; set; }
        private IAesCryptoHelper CryptoHelper { get; set; }

        public AuthBusiness(IUsuarioBusiness usuarioBusiness, IPermissaoBusiness permissaoBusiness, IAesCryptoHelper crypto) {
            if(usuarioBusiness == null) 
                throw new ArgumentNullException("usuarioBusiness");
            if(permissaoBusiness == null)
                throw new ArgumentNullException("permissaoBusiness");
            if(crypto == null) {
                throw new ArgumentNullException("crypto");
            }
            UsuarioBusiness = usuarioBusiness;
            PermissaoBusiness = permissaoBusiness;
            CryptoHelper = crypto;
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa) {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            UsuarioBusiness.configuraUsuario(cdUsuario, cd_empresa);
            PermissaoBusiness.configuraUsuario(cdUsuario, cd_empresa);
        }

        private AuthenticationStatus AuthenticateInStore(UserCredential credentials, out UsuarioWebSGF usuario) {

            var ret = UsuarioBusiness.GetUsuarioAuthenticateByLogin(credentials.Login);
            usuario = ret;
            if(ret == null)
                return AuthenticationStatus.UnknownUser;

            bool senhaValida = true;
            if (ret.dc_senha_usuario != UsuarioBusiness.GeraSenhaHashSHA1(credentials.Password))
            {
                senhaValida = false;
                UsuarioBusiness.incrementaNmTentativa(usuario, credentials.nmMaxTentativas, senhaValida);
                return AuthenticationStatus.IncorrectPassword;
            }

            //Quando senha for invalida, incrementar no número de tentativas no registro desse usuário
            UsuarioBusiness.incrementaNmTentativa(usuario, credentials.nmMaxTentativas, senhaValida);
            return AuthenticationStatus.OK;
        }

        public void verificaHorarioLogin(TimeSpan hr_inicial, TimeSpan hr_final, TimeSpan hr_servidor) {
            if(hr_servidor.CompareTo(hr_inicial) < 1 || hr_servidor.CompareTo(hr_final) > 0)
                throw new AuthBusinessException(Componentes.Utils.Messages.Messages.msgHorarioLoginNaoPermitido, AuthBusinessException.TipoErro.HORARIO_LOGIN_ULTRAPASSADO);
        }

        [Obsolete]
        public AuthenticationResult renovarAutenticacao(UserCredential credentials){
            byte[] encryptedToken, encryptedRefreshToken;
            string permissoes;
            UsuarioWebSGF usuario;
            
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                usuario = UsuarioBusiness.GetUsuarioAuthenticateByLogin(credentials.Login);
                permissoes = PermissaoBusiness.GetFuncionalidadesUsuario(credentials.Login, credentials.EhMasterGeral, credentials.CodEmpresa, usuario.cd_usuario, (usuario.id_master || usuario.id_administrador));
                
                transaction.Complete();
            }
            int codPessoaUsuario = usuario.cd_pessoa != null ? (int)usuario.cd_pessoa : 0;
            bool master = usuario.id_master || usuario.id_administrador;
            string accessToken = DateTime.UtcNow.AddSeconds(expirationTimeSec).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + ";" + codPessoaUsuario + ";" + usuario.cd_usuario + ";" + permissoes + ";" + master + ";" + credentials.HrInicial + ";" + credentials.HrFinal + ";" + credentials.CodEmpresa + ";" + credentials.IdFusoHorario + ";" + credentials.IdHorarioVerao;
            encryptedToken = CryptoHelper.EncryptStringToBytes_Aes(accessToken);
            accessToken = Convert.ToBase64String(encryptedToken);

            var refreshTokenObj = new
            {
                AccessToken = accessToken,
                Credentials = credentials
            };
            string tokenJSON = JsonConvert.SerializeObjectAsync(refreshTokenObj).Result;
            encryptedRefreshToken = CryptoHelper.EncryptStringToBytes_Aes(tokenJSON);
            string refreshToken = Convert.ToBase64String(encryptedRefreshToken);
            
            return new AuthenticationResult()
            {
                CodPessoaUsuario = codPessoaUsuario,
                loginUsuario = credentials.Login,
                IdMaster = (usuario.id_master || usuario.id_administrador) && !usuario.id_admin,
                CdUsuario = usuario.cd_usuario,
                Permissao = permissoes,
                Token = new AccessTokenResponse()
                {
                    access_token = accessToken,
                    expires_in = expirationTimeSec,
                    refresh_token = refreshToken
                }
            };
        }

        [Obsolete]
        public AuthenticationResult Authenticate(UserCredential credentials) {
            UsuarioWebSGF usuario;
            var status = AuthenticateInStore(credentials, out usuario);
            switch (status) {
                case AuthenticationStatus.IncorrectPassword:
                    return new AuthenticationResult() {
                        Status = status,
                        ErrorMessage = Messages.msgInfUserNotEnc
                    };
                case AuthenticationStatus.UnknownUser:
                    return new AuthenticationResult() {
                        Status = status,
                        ErrorMessage = Messages.msgInfPassword
                    };
            }
            string permissao;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                permissao = PermissaoBusiness.GetFuncionalidadesUsuario(credentials.Login, credentials.EhMasterGeral, credentials.CodEmpresa, usuario.cd_usuario, (usuario.id_master || usuario.id_administrador));
                transaction.Complete();
            }
            byte[] encryptedToken, encryptedRefreshToken;
            int codPessoaUsuario = usuario.cd_pessoa != null ? (int)usuario.cd_pessoa : 0;
            string loginUsuario = usuario.no_login;
            bool idMaster = (usuario.id_master || usuario.id_administrador) && !usuario.id_admin;
            int cdUsuario = usuario.cd_usuario;
            int cdEmpresa = credentials.CodEmpresa.HasValue ? credentials.CodEmpresa.Value : 0;
            if (codPessoaUsuario > 0 && !idMaster && cdEmpresa > 0)
            {
                string msg = "";
                if (!UsuarioBusiness.verificarTravaProfessor(codPessoaUsuario, cdEmpresa))
                {
                    msg = "Professor bloqueado de efetuar login, pois existem turmas com diário de aula a lançar. Favor procurar o departamento Administrativo para regularizar sua situação!";
                    throw new AuthBusinessException(msg, AuthBusinessException.TipoErro.AUTORIZACAO_NAO_ENCONTRADA);
                }
            }
            string accessToken = DateTime.UtcNow.AddSeconds(expirationTimeSec).ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + ";" + codPessoaUsuario + ";" + cdUsuario + ";" + permissao + ";" + idMaster + ";" + credentials.HrInicial + ";" + credentials.HrFinal + ";" + cdEmpresa + ";" + credentials.IdFusoHorario + ";" + credentials.IdHorarioVerao;
            encryptedToken = CryptoHelper.EncryptStringToBytes_Aes(accessToken);
            accessToken = Convert.ToBase64String(encryptedToken);

            var refreshTokenObj = new {
                AccessToken = accessToken,
                Credentials = credentials
            };
            string tokenJSON = JsonConvert.SerializeObjectAsync(refreshTokenObj).Result;
            encryptedRefreshToken = CryptoHelper.EncryptStringToBytes_Aes(tokenJSON);
            string refreshToken = Convert.ToBase64String(encryptedRefreshToken);

            return new AuthenticationResult() {
                Status = status,
                CodPessoaUsuario = codPessoaUsuario,
                loginUsuario = loginUsuario,
                IdMaster = idMaster,
                CdUsuario = cdUsuario,
                Permissao = permissao,
                Token = new AccessTokenResponse() {
                    access_token = accessToken,
                    expires_in = expirationTimeSec,
                    refresh_token = refreshToken
                }
            };
        }

        public AuthenticationResult RefreshAccessToken(string refreshToken) {
            byte[] encryptedRefreshToken, encryptedAccessToken;
            string tokenJSON, decryptedAccessToken;

            encryptedRefreshToken = Convert.FromBase64String(refreshToken);
            tokenJSON = CryptoHelper.DecryptStringFromBytes_Aes(encryptedRefreshToken);
            var refreshTokenObj = JsonConvert.DeserializeAnonymousType(tokenJSON, new { AccessToken = string.Empty, Credentials = new UserCredential() });

            encryptedAccessToken = Convert.FromBase64String(refreshTokenObj.AccessToken);
            decryptedAccessToken = CryptoHelper.DecryptStringFromBytes_Aes(encryptedAccessToken);
            try {
                DateTime tokenExpirationTime = DateTime.ParseExact(decryptedAccessToken.Split(';')[0], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                
                if(DateTime.UtcNow < tokenExpirationTime) {
                    return new AuthenticationResult() {
                        Status = AuthenticationStatus.TokenNotExpired,
                        ErrorMessage = Messages.msgInfToken
                    };
                }
            }
            catch(FormatException fe) {
                throw new FormatException("Data incompatível: " + decryptedAccessToken.Split(';')[0], fe);
            }
            catch(Exception fe) {
                throw new Exception("Data: " + decryptedAccessToken.Split(';')[0], fe);
            }

            return Authenticate(refreshTokenObj.Credentials);
        }

        public string GetNomeUsuario(string nome) {
            IEnumerable<UsuarioWebSGF> usuarios = UsuarioBusiness.GetUsuarioByLogin(nome);
            string retorno = "";
            if(usuarios != null) {
                IEnumerator<UsuarioWebSGF> usuariosEnumerator = usuarios.GetEnumerator();
                if(usuariosEnumerator.MoveNext())
                    if(usuariosEnumerator.Current != null && usuariosEnumerator.Current.PessoaFisica != null)
                        retorno = usuariosEnumerator.Current.PessoaFisica.no_pessoa;
            }
            return retorno;
        }

        public UsuarioWebSGF GetNomeCodigoUsuario(string nome)
        {
            IEnumerable<UsuarioWebSGF> usuarios = UsuarioBusiness.GetUsuarioByLogin(nome);
            var usu = from usuario in usuarios
                          select new UsuarioWebSGF
                          {
                              cd_usuario = usuario.cd_usuario,
                              no_login = usuario.no_login
                          };
            return usu.FirstOrDefault();
        }
    }
}
