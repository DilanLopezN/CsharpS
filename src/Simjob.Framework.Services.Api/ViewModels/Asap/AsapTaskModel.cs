using System;
using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.ViewModels.Asap
{
    public class AsapTaskModel
    {
        public string? AsapTypeId { get; set; }
        public string? AsapTypeIcon { get; set; }
        public string Code { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string UserId { get; set; }
        public decimal Priority { get; set; }
        public DateTime SentDate { get; set; }
        public decimal? Status { get; set; }
        public decimal? View { get; set; }
        public string Id { get; set; }
        public DateTime CreateAt { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? DeleteAt { get; set; }
        public string? DeleteBy { get; set; }
        public bool? IsDeleted { get; set; }
        public List<string>? AsapLogSender { get; set; }

        public List<UserLogAsapModel>? UserLogs { get; set; }
    }

    public class UserLogAsapModel
    {
        public string IdAsapLogSender { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public DateTime SentDate { get; set; }
    }
}