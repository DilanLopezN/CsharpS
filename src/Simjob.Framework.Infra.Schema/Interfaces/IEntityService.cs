using Amazon.S3.Model;
using Simjob.Framework.Domain.Core.Entities;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Domain.Models.WeeklyScheduleModels;
using Simjob.Framework.Infra.Schemas.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Interfaces
{
    public interface IEntityService
    {
        Task<object> GetAll(string schemaName, int? page, int? limit, string sortField = null, bool sortDesc = false, string ids = null);
        Task<dynamic> GetAllPicking(string schemaName, string? codigo, string? localDestDescription, string? status, DateTime? dtInicio, DateTime? dtFinal, string sortField = null, bool sortDesc = false, string ids = null);

        Task<dynamic> GetAllItems(string schemaName, string? idItem, string? codigoItem);
        Task<dynamic> GetAllStockBalance(string? schemaName, string? idItem, string? idLocal, string? codLocal, DateTime? dataInicioSaldo, DateTime? dataFinalSaldo);
        Task<dynamic> GetAllStockBalanceForItemRepo(string? schemaName, List<string?> itemIds);
        Task<dynamic> GetStockBalanceByLocalCodes(string? schemaName, string[] localCodes);
        Task<dynamic> GetStockBalanceByItemId(string schemaName, string itemId);
        Task<dynamic> GetAllNf(string schemaName, string? nota, DateTime? dtInicio, DateTime? dtFinal, string? status, string? sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null);

        Task<dynamic> GetByIdsPickingItem(string schemaName, List<string> ids, string? lote);
        Task<dynamic> GetByIdsList(string schemaName, List<string> ids);
        Task<dynamic> GetByIdStockOperation(string schemaName, string id, string? code, string? status);
        Task<dynamic> GetByNfConfirmacaoEntrega(string schemaName, string? numero, DateTime? dtInicio, DateTime? dtFinal, string? status);
        Task<dynamic> GetByIdsStockMov(string schemaName, List<string> ids, string? codeExt, string? volCode);

        Task<object> GetRelationProperty(string schemaName, string id, string property, int? page, int? limit, string sortField = null, bool sortDesc = false);
        Task<object> GetById(string schemaName, string id);
        Task<Entity> Insert(string schemaName, Dictionary<string, object> data);

        Task<List<Entity>> InsertMany(string schemaName, List<Dictionary<string, object>> datas);

        Task<List<Entity>> InsertManyBalance(string schemaName, List<Dictionary<string, object>> datas);
        Task<List<Entity>> UpdateMany(string schemaName, List<Dictionary<string, object>> datas);
        Task<Entity> Update(string schemamName, Dictionary<string, object> data);
        Task<object> SerachFields(string userId, string schemaName, string searchFields, string value, string mode, int? page, int? limit, List<Framework.Domain.Models.Searchdefs.Filter> defs, List<bool> sortDesc, string sortField = null, string groupBy = null, string ids = null, string companySiteIds = null);

        Task<object> SerachFieldsJoin(string schemaName, JoinsModel model, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false);

        Task<string> GetListSearchFields(string schemaName);
        Task Delete(string id, string schemamName);

        Task DeleteAll(string schemaName);
        Schema GetSchemaByName(string schemaName);
        object GetRepository(Type schemaType);
        Task<bool> GetByFieldName(string schemaName, string field, string value);
        Task<object> GetByFieldNameCompany(string schemaName, string field, string value, string companySiteId);
        Task<Dictionary<string, string>> GetAutoinc(string schemaName, string[] fieldAutoInc);
        //Task<List<string>> UploadAttachs(List<string> data);
        Task<List<string>> UploadAttachs(List<dynamic> data, string bucketName);
        Task<string> GetItems(string schemaName, string arrayField, string id = null);
        Task<object> GetItemsJoin(JoinsModel model, dynamic data);
        Task<dynamic> GetByField(string schemaName, string field, string value);
        Task<dynamic> GetListByField(string schemaName, string field, string value);

        Task<List<WeeklyScheduleReturnModel>> GetAllWeeklySchedule(string? schemaName, string? fornecedorId,string? ownerId, DateTime? dataInicio, DateTime? dataFim, string sortField = null, bool sortDesc = false);
        Task<Dictionary<string, object>> AddMirrorsSql(string schemaName, Dictionary<string, object> obj);

    }
}
