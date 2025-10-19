using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simjob.Framework.Services.Api.Models.User
{
    public class UpdatePasswordRequest
    {
        public string? UserId { get; set; }
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
