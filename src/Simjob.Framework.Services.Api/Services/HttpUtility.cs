using System;
using System.Security.Cryptography;
using System.Text;

namespace Simjob.Framework.Services.Api.Services
{
    public class MD5CryptoHelper
    {
        public static string KEY = "7H$cvg19%HzZMa%";
        public static string KEY_RPT = "ChaveRelatorio01";

        //public static string KEY_ECOMMERCE = "ChaveRelatorio01";
        //public static string KEY = "7H$cvg19%HzZMa%";

        public static string criptografaSenha(string senhaCripto, string chave)
        {
            try
            {
                //TripleDESCryptoServiceProvider objcriptografaSenha = new TripleDESCryptoServiceProvider();
                //MD5CryptoServiceProvider objcriptoMd5 = new MD5CryptoServiceProvider();

                TripleDES objcriptografaSenha = TripleDES.Create();
                MD5 objcriptoMd5 = MD5.Create();

                byte[] byteHash, byteBuff;
                string strTempKey = chave;

                byteHash = objcriptoMd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objcriptoMd5 = null;
                objcriptografaSenha.Key = byteHash;
                objcriptografaSenha.Mode = CipherMode.ECB;

                byteBuff = ASCIIEncoding.ASCII.GetBytes(senhaCripto);
                return Convert.ToBase64String(objcriptografaSenha.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch (Exception ex)
            {
                return "Digite os valores Corretamente." + ex.Message;
            }
        }

        public static string descriptografaSenha(string strCriptografada, string chave)
        {
            try
            {
                //TripleDESCryptoServiceProvider objdescriptografaSenha = new TripleDESCryptoServiceProvider();
                //MD5CryptoServiceProvider objcriptoMd5 = new MD5CryptoServiceProvider();

                TripleDES objdescriptografaSenha = TripleDES.Create();
                MD5 objcriptoMd5 = MD5.Create();

                byte[] byteHash, byteBuff;
                string strTempKey = chave;

                byteHash = objcriptoMd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objcriptoMd5 = null;
                objdescriptografaSenha.Key = byteHash;
                objdescriptografaSenha.Mode = CipherMode.ECB;

                byteBuff = Convert.FromBase64String(strCriptografada);
                string strDecrypted = ASCIIEncoding.ASCII.GetString(objdescriptografaSenha.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                objdescriptografaSenha = null;

                return strDecrypted;
            }
            catch (Exception ex)
            {
                return "Digite os valores Corretamente." + ex.Message;
            }
        }

        private const string senhaCaracteresValidos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@#!?";

        public static string gerarSenha(int tamanho)
        {
            if (tamanho > 10)
                tamanho = 10;
            if (tamanho < 6)
                tamanho = 6;
            int valorMaximo = senhaCaracteresValidos.Length;
            Random radom = new Random(DateTime.Now.Millisecond);

            StringBuilder senha = new StringBuilder(tamanho);

            for (int i = 0; i < tamanho; i++)
                senha.Append(senhaCaracteresValidos[radom.Next(0, valorMaximo)]);

            return senha.ToString();
        }

        public static string geraSenhaHashSHA1(string credentialsPassword)
        {
            //SHA1 sha1 = new SHA1CryptoServiceProvider();
            SHA1 sha1 = SHA1.Create();
            credentialsPassword = credentialsPassword + credentialsPassword + credentialsPassword;
            byte[] data = System.Text.Encoding.ASCII.GetBytes(credentialsPassword);
            byte[] hash = sha1.ComputeHash(data);
            StringBuilder strigBuilder = new StringBuilder();
            foreach (var item in hash)
            {
                strigBuilder.Append(item.ToString("X2"));
            }

            return strigBuilder.ToString().ToUpper();
        }
    }
}