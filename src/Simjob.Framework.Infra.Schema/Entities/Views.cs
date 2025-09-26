using Simjob.Framework.Domain.Core.Entities;

namespace Simjob.Framework.Infra.Schemas.Entities
{
    public enum ViewParameterType
    {
        String,
        Date,
        Number,
        Boolean
    }

    public class Views : Entity
    {
        public Views(string name, string description, string query, string schema,string type, ViewParameter[] parameter)
        {
            Name = name;
            Description = description;
            Parameters = parameter;
            Query = query;
            SchemaName = schema;
            Type = type;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public ViewParameter[] Parameters { get; set; }
        //public string[] Params { get; set; }
        public string Query { get; set; }
        public string SchemaName { get; set; }
        public string Type { get; set; }
    }

    public class ViewParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }

        //public static ViewParameter? ParseFromDynamic(dynamic value)
        //{
        //    return new ViewParameter
        //    {
        //        Name = value.Name.ToString(),
        //        Value = value.Value.ToString(),
        //        DataType = Enum.Parse<ViewParameterType>(value.DataType.ToString(), true)
        //    };
        //}

        //public static ViewParameter? ParseFromJson(Dictionary<string, object> value)
        //{
        //    return new ViewParameter
        //    {
        //        Name = value["name"].ToString(),
        //        Value = value["value"].ToString(),
        //        DataType = Enum.Parse<ViewParameterType>(value["dataType"].ToString(), true),
        //    };
        //}
    }

    public static class ViewsFactory
    {
        public static Views Full(string id, string name, string description, string query, string schema,string type, ViewParameter[] parameters) => new Views(name, description, query, schema,type, parameters) { Id = id };
    }
    

}
