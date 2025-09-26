using System;

namespace Simjob.Framework.Services.Api.Models
{
    public class SendEmailUI
    {
        public string? destinatario { get; set; }
        public string? CcEmail { get; set; }
        public string? FromEmail { get; set; }
        public string? assunto { get; set; }
        public string? mensagem { get; set; }
        public string? PrimaryDomain { get; set; }
        public int? PrimaryPort { get; set; }
        public string? UsernameEmail { get; set; }
        public string? UsernamePassword { get; set; }
        public string? dominio { get; set; }
        public string email { get; set; }
        public string login { get; set; }
        public bool? ssl { get; set; }
        public bool? UseDefaultCredentials { get; set; }
        public bool? IdTrocarSenha { get; set; }
        //public Stream? PDFBoleto { get; set; }
        //public Dictionary<string, Stream>? Anexos { get; set; }

        public static void configurarEmailSection(SendEmailUI sendEmail, EmailConfigUI emailConfig)
        {
            sendEmail.PrimaryDomain = emailConfig.PrimaryDomain;
            sendEmail.PrimaryPort = emailConfig.PrimaryPort;
            sendEmail.ssl = Convert.ToBoolean(emailConfig.ssl);
            sendEmail.UseDefaultCredentials = Convert.ToBoolean(emailConfig.ssl);
            sendEmail.FromEmail = emailConfig.FromEmail;
            sendEmail.UsernamePassword = emailConfig.UsernamePassword;
            sendEmail.UsernameEmail = emailConfig.UsernameEmail;
        }
    }
}