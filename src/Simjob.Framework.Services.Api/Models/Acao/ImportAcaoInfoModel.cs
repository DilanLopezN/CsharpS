using System;

namespace Simjob.Framework.Services.Api.Models.Acao
{
    public class ImportAcaoInfoModel
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Genero { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string Cpf { get; set; }
        public string Escolaridade { get; set; }
        public DateTime DataNascimento { get; set; }
        public string? Cep { get; set; }
        public string Complemento { get; set; }
        public string Numero { get; set; }
        public string TipoLogradouro { get; set; }
    }
}
