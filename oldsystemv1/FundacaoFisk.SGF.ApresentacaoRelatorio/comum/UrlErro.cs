using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace FundacaoFisk.SGF.ApresentacaoRelatorio.comum
{
    public static class UrlErro
    {
        public static string ObterUrlErroRelatorio(string url)
        {
            //var enderecoWebErroRelatorio = ConfigurationManager.AppSettings["enderecoWebErroRelatorio"];
            //var url = enderecoWebErroRelatorio != null ? enderecoWebErroRelatorio + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
            //url = !url.Contains("http") ? "http://" + url : url;
            //return url;
            bool sgfNew = false;

            string prefixo = "http://";
            url = url.Replace(prefixo, "");

            prefixo = "https://";
            url = url.Replace(prefixo, "");

            string[] url_part = url.Split(Char.Parse("/"));
            for (int i = 0; i < url_part.Length; i++)
            {
                if (url_part[i] == "SGFNEW") sgfNew = true;

            }
            var enderecoWebErroRelatorio = "";
            if (sgfNew)
                enderecoWebErroRelatorio = ConfigurationManager.AppSettings["enderecoWebErroRelatorioNew"];
            else
                enderecoWebErroRelatorio = ConfigurationManager.AppSettings["enderecoWebErroRelatorio"];

            string[] enderecoWebPart = enderecoWebErroRelatorio.Split(Char.Parse(";"));
            for (int i = 0; i < enderecoWebPart.Length; i++)
            {

                if (enderecoWebPart[i].Contains(url_part[0]) &&
                    (enderecoWebPart[i].Contains("apps") && url_part[0].Contains("apps")))
                {
                    enderecoWebErroRelatorio = enderecoWebPart[i];
                }
                else if (enderecoWebPart[i].Contains(url_part[0]))
                {
                    enderecoWebErroRelatorio = enderecoWebPart[i];
                }
            }

            if(sgfNew)
                url = enderecoWebErroRelatorio != null ? enderecoWebErroRelatorio : ConfigurationManager.AppSettings["enderecoRetornoErro"];
            else
                url = enderecoWebErroRelatorio != null ? enderecoWebErroRelatorio + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
            url = !url.Contains("http") ? "http://" + url : url;
            return url;
        }
    }
}