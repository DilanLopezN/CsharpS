namespace Simjob.Framework.Services.Api.Models.Modules
{
    public class RetornoSubModules
    {
            public string Id { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Type { get; set; }
            public string? Icon { get; set; }
            public bool? Active { get; set; }
            public string? ModuleId { get; set; }
            public decimal? Price { get; set; }
            public string? ActionId { get; set; }
            public string? Path { get; set; }
            public int? Order { get; set; }
            public string? Redirect { get; set; }
            public string? SchemaName { get; set; }
            public dynamic? SubModules { get; set; }




    }
}
