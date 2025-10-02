using System.Web;
using System.Web.Mvc;
using FundacaoFisk.SGF.Web.Services.Auth.Model;
using System;
using System.Configuration;

namespace FundacaoFisk.SGF.ApresentacaoRelatorio {
    public abstract class SGFBaseViewPage : SGFBaseViewPage<object> {}
    public abstract class SGFBaseViewPage<T> : WebViewPage<T> {
    }
}