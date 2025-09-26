using System.Collections.Generic;

namespace Simjob.Framework.Infra.Domain.Models
{
    public class SchemaModel
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Alias { get; set; }
        public string Source { get; set; }
        public dynamic Type { get; set; }
        public string Module { get; set; }
        public string Description { get; set; }
        public string? Redirect { get; set; }
        public string Descriptor { get; set; }
        public string PrimaryKey { get; set; }
        public string SearchFields { get; set; }
        public string FilterFields { get; set; }
        public string Group { get; set; }
        public bool Principal { get; set; }
        public bool AllowChanges { get; set; } = true;
        public bool Intern { get; set; }
        public bool Multi_Company { get; set; }
        public bool Owner_Schema { get; set; }
        public Dictionary<string, object> Definitions { get; set; }
        public Dictionary<string, SchemaModelProperty> Properties { get; set; }
        public Dictionary<string, SchemaModelForm> Form { get; set; }
        public string BeforeSave { get; set; }
        public string AfterSave { get; set; }
        public Dictionary<string, StatusFlowProperties[]> StatusFlow { get; set; }
        public SegmentationRelationSchemaProperties segmentationRelationSchema { get; set; }
        public string CompanySiteId { get; set; }
        public List<InnerJoinConfig> InnerJoin { get; set; }
        public List<WhereConfig> Where { get; set; }
        //public RelationCondition RelationCondition { get; set; }
    }


    public class SchemaModelProperty
    {
        public bool AutoInc { get; set; }
        public string Mask { get; set; }
        public dynamic Type { get; set; }
        public string Description { get; set; }
        public string RelationSchema { get; set; }
        public string Mirror { get; set; }

        //public RelationCondition RelationCondition { get; set; }
        public string field { get; set; }

        public string condition { get; set; }
        public bool unique { get; set; }
        public bool uniqueCompany { get; set; }
        public bool seg { get; set; }
        public bool statusF { get; set; }
        public string[] @enum { get; set; }
        public object file { get; set; }
        public bool? primaryKey { get; set; }
    }

    public class SchemaModelForm
    {
        public string Label { get; set; }
        public string Tab { get; set; }
        public int Order { get; set; } = 1;
        public bool ReadOnly { get; set; }
        public string Group { get; set; }
        public int Rows { get; set; } = 1;
    }

    public class StatusFlowProperties
    {
        public string status { get; set; }
        public List<string> emailAprov { get; set; }
        public int minAprov { get; set; }

        public string asapName { get; set; }
        public string action { get; set; }
    }

    public class SegmentationRelationSchemaProperties
    {
        public string searchField { get; set; }
        public string fieldSchema { get; set; }
        public string filter { get; set; }
    }
    public class InnerJoinConfig
    {
        public string table { get; set; }
        public List<string> fields { get; set; }
        public string fk { get; set; }
        public string localField { get; set; }
        public string @as { get; set; }
        public string joinTable { get; set; }
    }

    public class WhereConfig
    {
        public string field { get; set; }
        public object value { get; set; }
    }
}