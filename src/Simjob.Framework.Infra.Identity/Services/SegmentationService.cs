using MongoDB.Driver;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Services
{
    public class SegmentationService : ISegmentationService
    {
        protected readonly SegmentationContext Context;
        protected readonly IMongoCollection<Segmentation> Collection;
        
        private readonly IRepository<SegmentationContext, Segmentation> _segmentationRepository;
        
        public SegmentationService(SegmentationContext context, IRepository<SegmentationContext, Segmentation> segmentationRepository)
        {
            Context = context;
            Collection = context.GetUserCollection();
            _segmentationRepository = segmentationRepository;
            

        }

        public void DeleteSegmentation(string id)
        {
            _segmentationRepository.Delete(id);
        }

        public async Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false)
        {
            return _segmentationRepository.GetAll(page, limit, sortField, sortDesc, false);
        }

        public Segmentation GetSegmentationByFields(string userId, string schema, string field)
        {
            var filterId = Builders<Segmentation>.Filter.Eq(u => u.UserId, userId);
            var filterSchema = Builders<Segmentation>.Filter.Eq(u => u.SchemaName, schema);
            var filterField = Builders<Segmentation>.Filter.Eq(u => u.Field, field);
            var filterIsDeleted = Builders<Segmentation>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterSchema & filterField & filterIsDeleted).FirstOrDefault();
        }

        public Segmentation GetSegmentationById(string id)
        {
            var filterId = Builders<Segmentation>.Filter.Eq(u => u.Id, id);
            var filterIsDeleted = Builders<Segmentation>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public Segmentation GetSegmentationByUserId(string userId)
        {
            var filterId = Builders<Segmentation>.Filter.Eq(u => u.UserId, userId);
            var filterIsDeleted = Builders<Segmentation>.Filter.Eq(u => u.IsDeleted, false);
            return Collection.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public void Register(Segmentation segmentation)
        {
            _segmentationRepository.Insert(segmentation);
        }

        public void UpdateSegmentation(Segmentation segmentation, string id)
        {
            var segmentationIdFilter = Builders<Segmentation>.Filter.Eq(u => u.Id, id);
            var segmentationUpdate = Builders<Segmentation>.Update.Set(u => u.SchemaName, segmentation.SchemaName)
                                                          .Set(u => u.Field, segmentation.Field)
                                                          .Set(u => u.Values, segmentation.Values)
                                                          .Set(u => u.UserId, segmentation.UserId);

            Collection.UpdateOne(segmentationIdFilter, segmentationUpdate);
        }
        public List<string> GetFieldsAndValues(SchemaModel segbool, string userId, string schemaName)
        {
            List<string> fieldProp = new List<string>();
            foreach (var prop in segbool.Properties)
            {


                if (prop.Value.seg == true)
                {
                    fieldProp.Add(prop.Key);
                }


            }
            List<string[]> values = new List<string[]>();
            var segmentation = new Segmentation();
            foreach (var field in fieldProp)
            {
                segmentation = GetSegmentationByFields(userId, schemaName, field);

                if (segmentation != null)
                {
                    values.Add(segmentation.Values);
                }

            }
            if (values.Count > 0)
            {
                var sFields = string.Join(",", fieldProp);
                var valoresVirgula = new List<string>();


                foreach (var v in values)
                {
                    var vjoin = string.Join(",", v);
                    valoresVirgula.Add(vjoin);

                }
                var sValues = string.Join(";", valoresVirgula);

                var list = new List<string> {sFields, sValues};

                return list;
            }

            return null;
        }
    }
}
