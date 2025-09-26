using Simjob.Framework.Infra.Schemas.Entities;
using System;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Interfaces
{
    public interface IViewService
    {
        Task<object> GetAll(int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null);
        Views GetById(string id);
        bool Insert(Views view);
        bool Update(string id, Views view);
        Task<object> SerachFields(string searchFields, string value, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null);
        bool Delete(string id);
        Views GetViewByName(string name);
        Views GetViewByNameExternal(string name, string tenanty);
        Views GetByIdExternal(string id, string tenanty);
        object GetRepository(Type schemaType);
        object ExecuteByViewId(Views id);
    }
}
