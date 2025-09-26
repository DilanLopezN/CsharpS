using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Schemas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface ISegmentationService
    {
        public void Register(Segmentation segmentation);
        public Segmentation GetSegmentationById(string userId);
        public Segmentation GetSegmentationByUserId(string id);
        public Segmentation GetSegmentationByFields(string userId, string schema, string field);
        public List<string> GetFieldsAndValues(SchemaModel segbool, string userId, string schemaName);
        public void UpdateSegmentation(Segmentation segmentation, string id);
        public void DeleteSegmentation(string id);
        Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false);
    }
}
