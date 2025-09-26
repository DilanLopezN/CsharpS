namespace Simjob.Framework.Domain.Models
{
    public class ApiResponse
    {
        public ApiResponse(object data, bool success = true)
        {
            Success = success;
            Data = data;
        }

        public bool Success { get; set; }
        public object Data { get; set; }
    }
}
