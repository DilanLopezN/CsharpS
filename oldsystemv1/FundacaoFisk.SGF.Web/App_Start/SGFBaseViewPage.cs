using System.Web;
using System.Web.Mvc;
using FundacaoFisk.SGF.Web.Services.Auth.Model;
using System;
using System.Configuration;

namespace FundacaoFisk.SGF.Web {
    public abstract class SGFBaseViewPage : SGFBaseViewPage<object> {}
    [System.Web.Mvc.OutputCache(VaryByParam = "none", Duration = 1)]
    public abstract class SGFBaseViewPage<T> : WebViewPage<T> {
        public IHtmlString ApiAccessToken {
            get {
                var accessTokenResponse = Cache["AccessToken" + ";" + User.Identity.Name] as AccessTokenResponse;
                return accessTokenResponse == null ? Html.Raw("") : Html.Raw(accessTokenResponse.access_token);
            }
        }

        public IHtmlString Permissoes {
            get {
                return Html.Raw(Session["Permissoes"]);
            }
        }

        public IHtmlString EnderecoRelativoWeb {
            get {
                return Html.Raw(ConfigurationManager.AppSettings["enderecoRelativoWeb"]);
            }
        }

        public IHtmlString EnderecoRelatorioWeb {
            get {
                string[] enderecoRelatorioWeb = ConfigurationManager.AppSettings["enderecoRelatorioWeb"].Split(Char.Parse(";"));
                return Html.Raw(Session["enderecoRelatorioWeb"] != null ? Session["enderecoRelatorioWeb"] : enderecoRelatorioWeb[0]);
            }
        }

        public IHtmlString EnderecoWeb
        {
            get
            {
                string[] enderecoWeb = ConfigurationManager.AppSettings["enderecoWeb"].Split(Char.Parse(";"));
                return Html.Raw(Session["enderecoWeb"] != null ? Session["enderecoWeb"] : enderecoWeb[0]);
            }
        }

        public IHtmlString idMaster
        {
            get {
                return Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Session["IdMaster"]));
            }
        }

        public IHtmlString idFechConsolidado
        {
            get
            {
                if (Session["Permissoes"] != null)
                    return Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Session["Permissoes"].ToString().Contains("fcc")));
                return Html.Raw("");
            }
        }

        public IHtmlString masterGeral
        {
            get {
                return Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Session["MasterGeral"]));
            }
        }

        public IHtmlString EscolasUsuario
        {
            get
            {
                return Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Session["EscolasUsuario"]));
            }
        }

        public IHtmlString EscolasUsuarioInativas
        {
            get
            {
                return Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Session["EscolasUsuarioInativas"]));
            }
        }
        public IHtmlString EscolaSelecionada
        {
            get
            {
                return Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Session["CodEscolaSelecionada"]));
            }
        }

        public IHtmlString NomeEscolaSelecionada
        {
            get
            {
                return Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Session["NomeEscolaSelecionada"]));
            }
        }
        public IHtmlString LoginUsuario
        {
            get
            {
                return Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Session["loginUsuario"]));
            }
        }
        public IHtmlString AmbienteTeste
        {
            get
            {
                string ambienteTeste = ConfigurationManager.AppSettings["ambienteTeste"];
                return Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(ambienteTeste));
            }
        }
        public IHtmlString AppVersion
        {
            get
            {
                string appversion = "Branch45 - sprint 45-E";
                return Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(appversion));
            }
        }
    }
}