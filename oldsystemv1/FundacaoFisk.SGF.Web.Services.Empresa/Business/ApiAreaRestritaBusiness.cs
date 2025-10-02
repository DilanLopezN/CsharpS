using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using log4net;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Business
{
    public class ApiAreaRestritaBusiness : IApiAreaRestritaBusiness
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PermissaoBusiness));
        public IGrupoDataAccess DataAccessGrupo { get; set; }
        public ISysDireitoGrupoDataAccess DataAccessDireitoGrupo { get; set; }
        public ISysGrupoUsuarioDataAccess DataAccessGrupoUsuario { get; set; }
        string aplicaApiAreaRestritaValue = ConfigurationManager.AppSettings["aplicarApiAreaRestrita"]; 
        string urlApiAreaRestrita = ConfigurationManager.AppSettings["urlApiAreaRestrita"];
        string emailApiAreaRestrita = ConfigurationManager.AppSettings["emailApiAreaRestrita"];
        string senhaApiAreaRestrita = ConfigurationManager.AppSettings["senhaApiAreaRestrita"];

       
        public ApiAreaRestritaBusiness(IMenuDataAccess dataAccessMenu, IGrupoDataAccess grupoDataAccess,
            ISysDireitoGrupoDataAccess dataAccessDireitoGrupo, ISysGrupoUsuarioDataAccess dataAccessGrupoUsuario)
        {
            if (dataAccessMenu == null || grupoDataAccess == null || dataAccessDireitoGrupo == null || dataAccessGrupoUsuario == null)
            {
                throw new ArgumentNullException("repository");
            }
            DataAccessGrupo = grupoDataAccess;
            DataAccessDireitoGrupo = dataAccessDireitoGrupo;
            DataAccessGrupoUsuario = dataAccessGrupoUsuario;
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa)
        {
            // Configura os codigos do usuário para auditorias dos DataAccess:
        }

        public void sincronizaContexto(DbContext db)
        {
            //this.MenuDataAccess.sincronizaContexto(db);
            //this.DataAccessGrupo.sincronizaContexto(db);
            //this.DataAccessDireitoGrupo.sincronizaContexto(db);
        }

        #region GerarTokenUrlAreaRestrita
        public string GerarToken(string userEmail)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = ConfigurationManager.AppSettings["AreaRestritaNewTokenSecret"];
            var emissor = ConfigurationManager.AppSettings["AreaRestritaNewTokenEmissor"];
            var expiracao = ConfigurationManager.AppSettings["AreaRestritaNewTokenExpiracao"];
            var validoEm = ConfigurationManager.AppSettings["AreaRestritaNewTokenValidoEm"];
            var urlRedirect = ConfigurationManager.AppSettings["AreaRestritaNewTokenUrlRedirect"];



            var securityKey = Encoding.ASCII.GetBytes(key);
            var claims = new List<Claim>();

            var identityClaims = ObterClaimsUsuario(userEmail, claims);

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Issuer = emissor,
                //Audience = validoEm,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddMinutes(!String.IsNullOrEmpty(expiracao) ? int.Parse(expiracao) : 5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256Signature)
            });

            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);

            return (urlRedirect + jwt_token);
        }

        public object SendToken(string token)
        {
            var urlRedirect = ConfigurationManager.AppSettings["AreaRestritaNewTokenUrlRedirect"];
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true; // Ignora erros de certificado (não recomendado para produção)
            handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            var client = new HttpClient(handler);
            var baseUrl = String.Format("{0}", urlRedirect);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));


            var formContent = new MultipartFormDataContent
            {
                //{new StringContent("franqueado"),"type_access"},

            };



            HttpResponseMessage response = null;
            try
            {
                response = client.PostAsync(baseUrl, formContent).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2} ) -> {3}", baseUrl, "sendToken", token, String.Format("Erro ao executar requisição: {0}", e.Message)), e, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }

            var result = response.Content.ReadAsStringAsync();

            string jsonString = result.Result;
            object retorno = null;

            var objJson = JObject.Parse(jsonString);
            var status = 0;
            foreach (var e in objJson)
            {
                if (e.Key == "status")
                    status = (int)e.Value;


            }

            if (!String.IsNullOrEmpty(jsonString) && status == 200)
            {
                retorno = deserelializeJsonC<object>(jsonString);
                
            }
            else
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2} ) -> {3}", baseUrl, "sendToken", token, jsonString), null, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }

            if (response.IsSuccessStatusCode)
            {

                if (retorno != null)
                {
                    return retorno;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2} ) -> {3}", baseUrl, "sendToken", token, jsonString), null, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }



        }

        private ClaimsIdentity ObterClaimsUsuario(string userEmail, ICollection<Claim> claims)
        {


            claims.Add(new Claim(JwtRegisteredClaimNames.Email, userEmail));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));


            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            return identityClaims;
        }

        public static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);





        #endregion

        public TokenAreaRestritaUI ObterToken(string acao, string name)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true; // Ignora erros de certificado (não recomendado para produção)
            handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            var client = new HttpClient(handler);

            //var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator }; 
            //var client = new HttpClient(handler);
            var baseUrl = String.Format("{0}/api/token", urlApiAreaRestrita);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "multipart/form-data");
            var formContent = new MultipartFormDataContent
            {
                {new StringContent(emailApiAreaRestrita),"email"},
                {new StringContent(senhaApiAreaRestrita),"password" },
            };

            HttpResponseMessage response = null;
            try
            {
                response = client.PostAsync(baseUrl, formContent).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2} ) -> {3}", baseUrl, "ObterToken -> acao:" + acao, String.Format("name:{0} ", name), String.Format("Erro ao executar requisição: {0}", e.Message)), e, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }
            
            var result = response.Content.ReadAsStringAsync();
            string jsonString = result.Result;
            TokenAreaRestritaUI token = deserelializeJsonB<TokenAreaRestritaUI>(jsonString);

            if (response.IsSuccessStatusCode && token != null)
            {
                return token;
            }
            else
            {

                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2} ) -> {3}", baseUrl, "ObterToken -> acao" + acao, String.Format("name:{0} ", name), jsonString), null, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }
            
            
        }

        public T deserelializeJsonB<T>(string obj)
        {

            //JsonSerializer serializer = new JsonSerializer();

            //using (StringReader stringReader = new StringReader(obj))
            //using (JsonReader jsonReader = new JsonTextReader(stringReader))
            //{
            //    T objRet = serializer.Deserialize<T>(jsonReader);


            //}


            T jsonObj = JsonConvert.DeserializeObject<T>(obj);

            return jsonObj;
        }

        public T deserelializeJsonC<T>(string obj)
        {

            //JsonSerializer serializer = new JsonSerializer();

            /*using (StringReader stringReader = new StringReader(obj))
            using (JsonReader jsonReader = new JsonTextReader(stringReader))
            {
                T objRet = JsonSerializer.Deserialize<T>(jsonReader);


            }*/

            JsonSerializer serializer = new JsonSerializer();
            StringReader stringReader = new StringReader(obj);
            JsonReader jsonReader = new JsonTextReader(stringReader);
            T jsonObj = serializer.Deserialize<T>(jsonReader); 

            return jsonObj;
        }

        public MenusAreaRestritaRetorno ListagemDosMenus(string token)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true; // Ignora erros de certificado (não recomendado para produção)
            handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            var client = new HttpClient(handler);
            var baseUrl = String.Format("{0}/api/menus-main", urlApiAreaRestrita);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            HttpResponseMessage response = null;
            try
            {
                response = client.GetAsync(baseUrl).Result;
            }
            catch (Exception e)
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2} ) -> {3}", baseUrl, "listagemDosMenus", "", String.Format("Erro ao executar requisição: {0}", e.Message)), e, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }

            var result = response.Content.ReadAsStringAsync();

            bool hasData = false; 
            hasData = result.Result.Contains("\"data\":");
            

            
            string jsonString = result.Result;
            MenusAreaRestritaRetorno menus = null;
            if (!String.IsNullOrEmpty(jsonString) && hasData)
            {
                menus = deserelializeJsonC<MenusAreaRestritaRetorno>(jsonString);
            }
            else if((!String.IsNullOrEmpty(jsonString) && !hasData))
            {
                menus = new MenusAreaRestritaRetorno();
                List<MenusAreaRestritaUI> menuSemData = deserelializeJsonC<List<MenusAreaRestritaUI>>(jsonString);
                menus.message = null;
                menus.success = true;
                menus.data = menuSemData;

            }
            

            if (response.IsSuccessStatusCode)
            {
                
                if (menus != null)
                {
                    return menus;
                }
                else
                {
                    return null;
                }
                
            }
            else
            {
                
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2} ) -> {3}", baseUrl, "listagemDosMenus", "", jsonString), null, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
                
                
            }
            
        }

        public LogoutAreaRestritaRetorno Logout(string token, string acao, string name)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true; // Ignora erros de certificado (não recomendado para produção)
            handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            var client = new HttpClient(handler);
            var baseUrl = String.Format("{0}/api/logout", urlApiAreaRestrita);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = null;
            try
            {
                response = client.PostAsync(baseUrl, null).Result;
            }
            catch (Exception e)
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2} ) -> {3}", baseUrl, "logout -> " + acao, String.Format("name:{0}", name), String.Format("Erro ao executar requisição: {0}", e.Message)), e, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }

            var result = response.Content.ReadAsStringAsync();

            string jsonString = result.Result;
            LogoutAreaRestritaRetorno retorno = null;
            if (!String.IsNullOrEmpty(jsonString))
            {
                retorno = deserelializeJsonC<LogoutAreaRestritaRetorno>(jsonString); ;
            }

            if (response.IsSuccessStatusCode)
            {

                if (retorno != null)
                {
                    return retorno;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2} ) -> {3}", baseUrl, "logout" + acao, String.Format("name:{0}", name), jsonString), null, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }

            

        }

        public UserAreaRestritaCreateRetorno criarUsuario(string token, UserAreaRestritaUI usuarioCreate)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true; // Ignora erros de certificado (não recomendado para produção)
            handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            var client = new HttpClient(handler);
            var baseUrl = String.Format("{0}/api/user/create", urlApiAreaRestrita);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));


            var formContent = new MultipartFormDataContent
            {
                {new StringContent("franqueado"),"type_access"},
                {new StringContent(usuarioCreate.name),"name" },
                {new StringContent(usuarioCreate.email),"email" },
                {new StringContent(usuarioCreate.password),"password" },
                {new StringContent(usuarioCreate.id_fisk_sgf),"id_fisk_sgf" },
                {new StringContent(usuarioCreate.id_fisk_franchisee),"id_fisk_franchisee" },
               
            };


            //Adiciona os menus
            for (int i = 0; i < usuarioCreate.menus.Count; i++)
            {
                formContent.Add(new StringContent(usuarioCreate.menus[i].ToString()), String.Format("menus[{0}]", i));
            }

            HttpResponseMessage response = null;
            try
            {
                response = client.PostAsync(baseUrl, formContent).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2} ) -> {3}", baseUrl, "createUsuario", String.Format("name:{0}, email:{1}, id_fisk_sgf:{2}, id_fisk_franchisee:{3}", usuarioCreate.name, usuarioCreate.email, usuarioCreate.id_fisk_sgf, usuarioCreate.id_fisk_franchisee), String.Format("Erro ao executar requisição: {0}", e.Message)), e, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }

            var result = response.Content.ReadAsStringAsync();

            string jsonString = result.Result;
            UserAreaRestritaCreateRetorno retorno = null;

            var objJson = JObject.Parse(jsonString);
            var status = 0;
            foreach (var e in objJson)
            {
                if (e.Key == "status")
                    status = (int)e.Value;
                

            }

            if (!String.IsNullOrEmpty(jsonString) && status == 200)
            {
                retorno = deserelializeJsonC<UserAreaRestritaCreateRetorno>(jsonString);
                if (!String.IsNullOrEmpty(retorno.user.menus) && retorno.user.menus != "null" && retorno.user.menus != "[]")
                {
                    retorno.user.menusConvertidos = deserelializeJsonC<List<string>>(retorno.user.menus);
                }
                else
                {
                    retorno.user.menusConvertidos = new List<string>();
                }
            }
            else
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2} ) -> {3}", baseUrl, "createUsuario", String.Format("name:{0}, email:{1}, id_fisk_sgf:{2}, id_fisk_franchisee:{3}", usuarioCreate.name, usuarioCreate.email, usuarioCreate.id_fisk_sgf, usuarioCreate.id_fisk_franchisee), jsonString), null, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }

            if (response.IsSuccessStatusCode)
            {

                if (retorno != null)
                {
                    return retorno;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2} ) -> {3}", baseUrl, "createUsuario", String.Format("name:{0}, email:{1}, id_fisk_sgf:{2}, id_fisk_franchisee:{3}", usuarioCreate.name, usuarioCreate.email, usuarioCreate.id_fisk_sgf, usuarioCreate.id_fisk_franchisee), jsonString), null, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }



        }

        public UserAreaRestritaUpdateRetorno updateUsuario(string token, int id,  UserAreaRestritaUI usuarioEdit)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true; // Ignora erros de certificado (não recomendado para produção)
            handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            var client = new HttpClient(handler);
            var baseUrl = String.Format("{0}/api/user/update/{1}", urlApiAreaRestrita, id);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Dictionary<string, string> formContent = new Dictionary<string, string>();
            
            if(!String.IsNullOrEmpty(usuarioEdit.name)) { formContent.Add("name", usuarioEdit.name);}
            if (!String.IsNullOrEmpty(usuarioEdit.email)) {formContent.Add("email", usuarioEdit.email );}
            if (!String.IsNullOrEmpty(usuarioEdit.password)) { formContent.Add("password", usuarioEdit.password); }

            formContent.Add("id_fisk_sgf", usuarioEdit.id_fisk_sgf );
            formContent.Add("id_fisk_franchisee", usuarioEdit.id_fisk_franchisee );

            for (int i = 0; i < usuarioEdit.menus.Count; i++)
            {
                formContent.Add(String.Format("menus[{0}]", i), usuarioEdit.menus[i].ToString());
            }

            var requestContent = new FormUrlEncodedContent(formContent);

            HttpResponseMessage response = null;
            try
            {
                response = client.PutAsync(baseUrl, requestContent).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2}) -> {3}", baseUrl, "updateUsuario", String.Format("name:{0}, email:{1}, id_fisk_sgf:{2}, id_fisk_franchisee:{3}", usuarioEdit.name, usuarioEdit.email, usuarioEdit.id_fisk_sgf, usuarioEdit.id_fisk_franchisee), String.Format("Erro ao executar requisição: {0}", e.Message)), e, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }

            var result = response.Content.ReadAsStringAsync();

            string jsonString = result.Result;
            UserAreaRestritaUpdateRetorno retorno = null;
            var objJson = JObject.Parse(jsonString);
            var status = 0;
            foreach (var e in objJson)
            {
                if (e.Key == "status")
                    status = (int)e.Value;
            }

            if (!String.IsNullOrEmpty(jsonString) && status == 200)
            {
                retorno = deserelializeJsonC<UserAreaRestritaUpdateRetorno>(jsonString); ;
            }
            else
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2}) -> {3}", baseUrl, "updateUsuario", String.Format("name:{0}, email:{1}, id_fisk_sgf:{2}, id_fisk_franchisee:{3}", usuarioEdit.name, usuarioEdit.email, usuarioEdit.id_fisk_sgf, usuarioEdit.id_fisk_franchisee), String.Format("Erro ao executar requisição: {0}", jsonString)), null, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }

            if (response.IsSuccessStatusCode)
            {

                if (retorno != null)
                {
                    return retorno;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - {2}) -> {3}", baseUrl, "updateUsuario", String.Format("name:{0}, email:{1}, id_fisk_sgf:{2}, id_fisk_franchisee:{3}", usuarioEdit.name, usuarioEdit.email, usuarioEdit.id_fisk_sgf, usuarioEdit.id_fisk_franchisee), jsonString), null, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }



        }

        public UserAreaRestritaDetalheRetorno getDetalhesUsuario(string token, string id)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true; // Ignora erros de certificado (não recomendado para produção)
            handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            var client = new HttpClient(handler);
            var baseUrl = String.Format("{0}/api/user/view/{1}", urlApiAreaRestrita, id);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = null;
            try
            {
                response = client.GetAsync(baseUrl).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - id: {2}) -> {3}", baseUrl, "getDetalhesUsuario", id, String.Format("Erro ao executar requisição: {0}", e.Message)), e, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }
            var result = response.Content.ReadAsStringAsync();
            string jsonString = result.Result;
            UserAreaRestritaDetalheRetorno retorno = null;

            var objJson = JObject.Parse(jsonString);
            var status = 0;
            foreach (var e in objJson)
            {
                if (e.Key == "status")
                    status = (int)e.Value;
            }

            if (!String.IsNullOrEmpty(jsonString) && status == 200)
            {
                retorno = deserelializeJsonC<UserAreaRestritaDetalheRetorno>(jsonString);
                if (retorno.user.menus != null && retorno.user.menus != "null")
                {
                    retorno.user.menusConvertidos = deserelializeJsonC<List<string>>(retorno.user.menus);
                }
                else
                {
                    retorno.user.menusConvertidos = new List<string>();
                }
            }
            

            if (response.IsSuccessStatusCode)
            {
                if (retorno != null)
                {
                    return retorno;
                }
                else
                {
                    return null;
                    //throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - id: {2}) -> {3}", url, "getDetalhesUsuario", id, "Retorno é nulo ou vazio"), null, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
                }
                
            }
            else
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - id: {2}) -> {3}", baseUrl, "getDetalhesUsuario", id, jsonString ), null, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }


        }


        public UserAreaRestritaUI deleteUsuarioAreaRestrita(string token, string id)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true; // Ignora erros de certificado (não recomendado para produção)
            handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            var client = new HttpClient(handler);
            var baseUrl = String.Format("{0}/api/user/delete/{1}", urlApiAreaRestrita, id);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = null;
            try
            {
                response = client.DeleteAsync(baseUrl).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - id: {2}) -> {3}", baseUrl, "deleteUsuario", id, String.Format("Erro ao executar requisição: {0}", e.Message)), e, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }
            var result = response.Content.ReadAsStringAsync();
            string jsonString = result.Result;

            UserAreaRestritaUI retorno = null;
            if (!String.IsNullOrEmpty(jsonString))
            {
                retorno = deserelializeJsonC<UserAreaRestritaUI>(jsonString); ;
            }

            if (response.IsSuccessStatusCode)
            {
                if (retorno != null)
                {
                    return retorno;
                }
                else
                {
                    throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - id: {2}) -> {3}", baseUrl, "deleteUsuario", id, "Retorno é nulo ou vazio"), null, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
                }

            }
            else
            {
                throw new ApiAreaRestritaBusinessException(string.Format("Erro Api Area Restrita -> (statusErro: Erro Api Area Restrita - Url {0} - Comando: {1} - Parametros - id: {2}) -> {3}", baseUrl, "deleteUsuario", id, jsonString), null, ApiAreaRestritaBusinessException.TipoErro.ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO, false);
            }


        }


        public bool aplicaApiAreaRestrita()
        {
            
            if (string.IsNullOrEmpty(aplicaApiAreaRestritaValue))
            {
                return false;
            }

            return Convert.ToBoolean(Convert.ToInt16(aplicaApiAreaRestritaValue));
        }

        public int aplicaTamanhoSenhaApiAreaRestrita(string aplica)
        {

            if (string.IsNullOrEmpty(aplica))
            {
                return 8;//Default
            }

            return Convert.ToInt16(aplica);
        }
    }
}