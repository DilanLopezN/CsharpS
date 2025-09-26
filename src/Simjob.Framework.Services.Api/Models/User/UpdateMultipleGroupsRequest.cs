using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simjob.Framework.Services.Api.Models.User
{
    public class UpdateMultipleGroupsRequest
    {
        [Required]
        public List<UserGroupUpdateItem> Users { get; set; } = new List<UserGroupUpdateItem>();
    }

    public class UserGroupUpdateItem
    {
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public string GroupId { get; set; }
    }
}
