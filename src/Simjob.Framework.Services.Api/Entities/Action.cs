using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Simjob.Framework.Domain.Core.Entities;
using System;
using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.Entities
{
    public enum ActionResultType
    {
        Message,
        ModalGrid,
        ModalCustom,
        FullCustom
    }

    public enum ActionCallType
    {
        Schema,
        Separate,
        TabForm,
        EventSchema,
        EventSchemaProperty,
        FromAction,
        Schedule,
        StatusFlow
    }

    public enum ActionParameterType
    {
        String,
        Relation,
        Date,
        Number,
        Boolean
    }

    public class CallConfig
    {
        public string SchemaName { get; set; }
        public string TriggerProperty { get; set; }
    }

    public class ActionParameter
    {
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ActionParameterType DataType { get; set; }


#pragma warning disable CS8632 // A anotação para tipos de referência anuláveis deve ser usada apenas em código em um contexto de anotações '#nullable'.
        public static ActionParameter? ParseFromDynamic(dynamic value)
#pragma warning restore CS8632
        {
            return new ActionParameter
            {
                Name = value.name.ToString(),
                DataType = Enum.Parse<ActionParameterType>(value.dataType.ToString(), true)
            };
        }
#pragma warning disable CS8632 // A anotação para tipos de referência anuláveis deve ser usada apenas em código em um contexto de anotações '#nullable'.
        public static ActionParameter? ParseFromJson(Dictionary<string, object> value)
#pragma warning restore CS8632
        {
            return new ActionParameter
            {
                Name = value["name"].ToString(),
                DataType = Enum.Parse<ActionParameterType>(value["dataType"].ToString(), true)
            };
        }
    }

    public class Action : Entity
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Module { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ActionCallType CallType { get; set; }
        public CallConfig CallConfig { get; set; }
        public string JavascriptCode { get; set; }
        public int Timeout { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ActionResultType ResultType { get; set; }
        public ActionParameter[] Parameters { get; set; }
    }
}
