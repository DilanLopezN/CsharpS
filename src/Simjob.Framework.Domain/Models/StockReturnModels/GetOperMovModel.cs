using System;

namespace Simjob.Framework.Domain.Models.StockReturnModels
{
    public class GetOperMovModel
    {
        public string? Id { get; set; }
        public string? CodigoOperacao { get; set; }
        public DateTime? DataReal { get; set; }
        public string? CodigoItem { get; set; }
        public string? DescricaoItem { get; set; }
        public DateTime? DataMovimento { get; set; }
        public double? Quantidade { get; set; }
        public double? Valor { get; set; }
        public string? Unidade { get; set; }
        public string? CodigoVolume { get; set; }
        public double? QtdVolumes { get; set; }
        public string? CodigoLote { get; set; }
        public string? CodigoLocal { get; set; }
        public string? DescricaoLocal { get; set; }

    }
}
