using System.Collections.Generic;
using System;

namespace Simjob.Framework.Services.Api.ViewModels.Asap
{
    public class AsapTypeModel
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal? Frequency { get; set; }
        public string Icon { get; set; }
        public List<string> Platform { get; set; }
        public string? ExecType { get; set; }
        public string? Action { get; set; }
        public bool? Default { get; set; }
        public bool? AllowManual { get; set; }
        public List<string> AsapRules { get; set; }
        public string Id { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? DeleteAt { get; set; }
        public string? DeleteBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}