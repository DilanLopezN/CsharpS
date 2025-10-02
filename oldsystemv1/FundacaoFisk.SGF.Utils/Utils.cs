using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Utils
{
    public static class Utils
    {
        public static string geradorNomeAleatorio(int qtd_letras)
        {
            string retorno = "";
            Random rand = new Random();
            // Caracteres possíveis dentro de uma
            String letras = "0123456789abcdefghijlkmnopqrstuvxzwy";

            for (int i = 0; i < qtd_letras; i++){
                int indice = rand.Next(36);
                    retorno += letras[indice] + "";
            }
            return retorno;
        }

        public static DateTime truncarMilissegundo(DateTime data)
        {
            return new DateTime(data.Year, data.Month, data.Day, data.Hour, data.Minute, data.Second);
        }

        // uislcs: Acho que a função FormatCode() deveria ser renomeada para Completar().
        /*
         * "Para os registros tipo A (Alfanumérico) preencher com caracteres caixa alta e com espaços à direita
         * preenchendo todo o espaço do campo. Para os registros tipo N (Numérico) preencher com zeros à
         * esquerda preenchendo todo o campo." (p.9)
         * 
         * Disponível em: http://www.sicoobpr.com.br/download/manualcobranca/Manual_Cedentes_Sistema_Proprio.doc
         */

        /// <summary>
        /// Função para completar um string com zeros ou espacos em branco. Pode servir para criar a remessa.
        /// </summary>
        /// <param name="text">O valor recebe os zeros ou espaços em branco</param>
        /// <param name="with">caractere a ser inserido</param>
        /// <param name="size">Tamanho do campo</param>
        /// <param name="left">Indica se caracteres serão inseridos à esquerda ou à direita, o valor default é inicializar pela esquerda (left)</param>
        /// <returns></returns>
        public static string FormatCode(string text, string with, int length, bool left) {
            length -= text.Length;
            if(left) {
                for(int i = 0; i < length; ++i) {
                    text = with + text;
                }
            }
            else {
                for(int i = 0; i < length; ++i) {
                    text += with;
                }
            }
            return text;
        }

        public static string FormatCode(string text, string with, int length) {
            return FormatCode(text, with, length, false);
        }

        public static string FormatCode(string text, int length) {
            return FormatCode(text, "0", length, true);
        }

        public static Stream MontarPDF(string html, bool boleto, bool doisPorPagina)
        {
            var cPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            cPdf.CustomWkHtmlArgs = "--encoding UTF-8";
            if (boleto)
            {
                cPdf.Orientation = NReco.PdfGenerator.PageOrientation.Portrait;
                if (doisPorPagina)
                    cPdf.Zoom = 1.5f;
                else
                    cPdf.Zoom = 2f;
                cPdf.Size = NReco.PdfGenerator.PageSize.A4;
                cPdf.Margins.Bottom = 1;
                cPdf.Margins.Left = 5;
                cPdf.Margins.Right = 5;
                cPdf.Margins.Top = 1;
            }
            var pdfBytes = cPdf.GeneratePdf(html);
            return new MemoryStream(pdfBytes);
        }

        public static bool validarEmail(string email)
        {
            string validEmailPattern = "^[\\w\\.-]+@([\\w\\-]+\\.)+[A-Z]{2,5}$"; 
            Regex ValidEmailRegex = new Regex(validEmailPattern, RegexOptions.IgnoreCase);

            bool isValido = ValidEmailRegex.IsMatch(email);

            return isValido;

        }

        public static string FormataCEP(string cep)
        {
            try
            {
                return string.Format("{0}{1}-{2}", cep.Substring(0, 2), cep.Substring(2, 3), cep.Substring(5, 3));
            }
            catch
            {
                return string.Empty;
            }
        }

        /**
         *Retira as tags Html de uma string (text não pode ser nulo)
         */
        public static string Strip(string text)
        {
            return Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
        }


        public static string GetTemplateCartaQuitacao(string titulo, string no_responsavel, int ano)
        {


           string html = "<div><p class='MsoNormal'><u></u>&nbsp;<u></u></p><p class='MsoNormal'><span style='font-size:10.0pt'><img class='CToWUd' src='https://fisk.com.br/assets/images/logo-Fisk.png'></span><br>" +
                          "<br><span style='font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;'>Olá <b>" + no_responsavel + "</b>! Segue anexa a carta de quitação anual de débitos referente ao ano de <b>" + ano + "</b>." +
                          "<br><br>Atenciosamente,<span style='color:#1f497d'><br><br></span><br><b>FISK - Centro de Ensino</b> <br><a href='tel:0800%20773%203475' value='+558007733475' target='_blank'>0800 773 3475</a> <br>" +
                          "<a href='http://www.fisk.com.br' target='_blank'>www.fisk.com.br</a><span style='color:#1f497d'><u></u><u></u></span></span></p><div class='yj6qo'></div><div class='adL'></div></div>";



            return html;
        }
        public static string innerMessage(Exception ex)
        {
            var message = "";
            if (ex.InnerException != null)
            {
                if (ex.InnerException.InnerException != null)
                {
                    if (ex.InnerException.InnerException.InnerException != null)
                    {
                        if (ex.InnerException.InnerException.InnerException.InnerException != null)
                            message = ex.InnerException.InnerException.InnerException.InnerException.Message;
                        else
                            message = ex.InnerException.InnerException.InnerException.Message;
                    }
                    else
                        message = ex.InnerException.InnerException.Message;
                }
                else
                    message = ex.InnerException.Message;
            }

            if (message.Contains("||"))
            {
                message = message.Substring(0, message.IndexOf("||"));
            }
            return message;
        }

        public static bool ContainsCaseInsensitive(this string source, string substring)
        {
            return source?.IndexOf(substring, StringComparison.OrdinalIgnoreCase) > -1;
        }
        public static string SubstituiCaracteresEspeciais(string strline)
        {
            try
            {
                strline = strline.Replace("ã", "a");
                strline = strline.Replace('Ã', 'A');
                strline = strline.Replace('â', 'a');
                strline = strline.Replace('Â', 'A');
                strline = strline.Replace('á', 'a');
                strline = strline.Replace('Á', 'A');
                strline = strline.Replace('à', 'a');
                strline = strline.Replace('À', 'A');
                strline = strline.Replace('ç', 'c');
                strline = strline.Replace('Ç', 'C');
                strline = strline.Replace('é', 'e');
                strline = strline.Replace('É', 'E');
                strline = strline.Replace('Ê', 'E');
                strline = strline.Replace('ê', 'e');
                strline = strline.Replace('õ', 'o');
                strline = strline.Replace('Õ', 'O');
                strline = strline.Replace('ó', 'o');
                strline = strline.Replace('Ó', 'O');
                strline = strline.Replace('ô', 'o');
                strline = strline.Replace('Ô', 'O');
                strline = strline.Replace('ú', 'u');
                strline = strline.Replace('Ú', 'U');
                strline = strline.Replace('ü', 'u');
                strline = strline.Replace('Ü', 'U');
                strline = strline.Replace('í', 'i');
                strline = strline.Replace('Í', 'I');
                strline = strline.Replace('ª', 'a');
                strline = strline.Replace('º', 'o');
                strline = strline.Replace('°', 'o');
                strline = strline.Replace('&', 'e');
                strline = strline.Replace('Ñ', 'N');

                return strline;
            }
            catch (Exception ex)
            {
                Exception tmpEx = new Exception("Erro ao formatar string.", ex);
                throw tmpEx;
            }
        }

    }
}
