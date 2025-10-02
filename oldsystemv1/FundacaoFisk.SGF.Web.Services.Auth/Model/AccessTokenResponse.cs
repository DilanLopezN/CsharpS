namespace FundacaoFisk.SGF.Web.Services.Auth.Model {
    public class AccessTokenResponse {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
    }
}
