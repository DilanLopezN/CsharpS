using Newtonsoft.Json;
using System;

namespace Simjob.Framework.Domain.Core.Entities
{
    public class Entity
    {
        public Entity()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public DateTime? CreateAt { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? DeleteAt { get; set; }
        public string DeleteBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
