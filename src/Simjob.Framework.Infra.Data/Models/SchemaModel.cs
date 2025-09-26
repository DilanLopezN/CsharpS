using System.Collections.Generic;

namespace Simjob.Framework.Infra.Data.Models
{
    public class SchemaModel
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public dynamic Type { get; set; }
        public string Module { get; set; }
        public string Description { get; set; }
        public string Descriptor { get; set; }
        public string Group { get; set; }
        public bool Principal { get; set; }
        public bool AllowChanges { get; set; } = true;
        public bool Intern { get; set; }
        public bool MultiCompany { get; set; }
        public Dictionary<string, object> Definitions { get; set; }
        public Dictionary<string, SchemaModelProperty> Properties { get; set; }
        public Dictionary<string, SchemaModelForm> Form { get; set; }
        public string BeforeSave { get; set; }
        public string AfterSave { get; set; }
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
}
