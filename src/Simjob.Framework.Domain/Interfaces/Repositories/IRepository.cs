using MongoDB.Driver;
using Simjob.Framework.Domain.Core.Entities;
using Simjob.Framework.Domain.Data;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using static Simjob.Framework.Domain.Models.Searchdefs;

namespace Simjob.Framework.Domain.Interfaces.Repositories
{
    public interface IRepository<TContext, TEntity>
        where TEntity : Entity
        where TContext : DbContext
    {
        PaginationData<TEntity> GetAll(int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null);

        PaginationData<TEntity> GetAllModules(string? module,int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null);

        PaginationData<TEntity> GetAllModulePermissions(string? userId,int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null, string moduleIds = null, string userIds = null,string groupIds = null);

        //stock
        List<TEntity> GetAllPicking(string? codigo, string? localDestDescription, string? status, DateTime? dtInicio, DateTime? dtFinal, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null);

        List<TEntity> GetAllItems(string? idItem, string? codigoItem);
        List<TEntity> GetAllActionsByIds(List<string>? ids);
        List<TEntity> GetAllActionsByModules(List<string>? moduleNames);
        List<TEntity> GetAllStockBalance(string? idItem, string? idLocal, string? codLocal, DateTime? dataInicioSaldo, DateTime? dataFinalSaldo);

        List<TEntity> GetAllStockBalanceForItemRepo(List<string?> itemIds);

        List<TEntity> GetStockBalanceByLocalCodes(string[] localCodes);

        TEntity GetStockBalanceByItemId(string itemId);

        List<TEntity> GetByIdsPickingItem(List<string> ids, string? lote);

        TEntity GetByIdStockOperation(string id, string? code, string? status);

        List<TEntity> GetAllNf(string? nota, DateTime? dtInicio, DateTime? dtFinal, string? status, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null);

        TEntity GetByNfConfirmacaoEntrega(string? numero, DateTime? dtInicio, DateTime? dtFinal, string? status);

        List<TEntity> GetByIdsStockMov(List<string> ids, string? codeExt, string? volCode);

        Task<List<TEntity>> GetAllList();

        List<TEntity> GetAllSchedule(bool addIsDeleted = false);

        void SchemaIndexSync(List<string> indexFields = null);

        //PaginationData<TEntity> SearchRegexByFields(string fields, string search, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, string ids = null);
        //PaginationData<TEntity> SearchRegexByFieldsSeg(string fields, string search, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, string ids = null);
        //PaginationData<TEntity> SearchRegexByMultiFields(string fields, string search, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, string ids = null);
        //FilterDefinition<TEntity> searchFieldCreate(string[] objSearchOr, string[] objSearchAnd, string[] objValueOr, string[] objValueAnd);
        PaginationData<TEntity> SearchRegexByFields(string fields, string search, string mode, SchemaModel schemaModel, int? page = null, int? limit = null, string sortField = null, string groupBy = null, bool sortDesc = false, string ids = null, List<string> relationIds = null, string relationField = "", string companySiteIds = "");

        PaginationData<TEntity> SearchRegexByFieldsJoin(SchemaModel schemaModel, JoinsModel model, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false);

        PaginationData<TEntity> SearchRegexByFieldsDefs(List<Filter> defs, List<bool> sortDesc, string fields, string search, SchemaModel schemaModel, int? page = null, int? limit = null, string sortField = null, string ids = null, string companySiteIds = "");

        PaginationData<TEntity> Search(Expression<Func<TEntity, bool>> predicate, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false);

        PaginationData<TEntity> SearchByFilterDefinition(FilterDefinition<TEntity> filterDefinition, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false);

        TEntity SearchFirstByField(string field, string value, bool isDeleted = false);

        TEntity GetByField(string field, string value);
        TEntity GetByFieldInt(string field, int value);
        TEntity GetByFieldCompany(string field, string value, string companySiteId);
        TEntity GetSchemaByField(string field, string value);

        List<TEntity> GetListByField(string field, string value);
        List<TEntity> GetSchemasByFieldContains(string field, string value);
        List<TEntity> GetSchemasByModuleRegex(string moduleName);
        List<TEntity> GetSchemasByNames(string field, List<string> names);
        List<TEntity> GetUsersByField();

        TEntity GetByFieldExternal(string field, string value, string tenanty);

        List<TEntity> SearchLikeInFieldAutoComplete(string field, string value, int limit = 10, Expression<Func<TEntity, bool>> predicate = null);

        PaginationData<TEntity> GetByIds(string[] ids, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = true);

        List<TEntity> GetByIdsList(List<string> ids);
        List<TEntity> GetByFieldsIntList(string field,List<int> values);
        TEntity GetById(string id);

        Task<TEntity> GetByIdAsync(string id);

        PaginationData<TEntity> GetAllRelationSchema(string id, string fieldUser, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null);

        TEntity GetByIdExternal(string id, string tenanty);

        TEntity GetByIdSchedule(string id);

        void Delete(string id);

        void DeleteAll();

        TEntity Insert(TEntity obj);

        Task<TEntity> InsertAsync(TEntity obj);

        Task<List<TEntity>> InsertManyAsync(List<TEntity> objs);

        List<TEntity> InsertMany(List<TEntity> objs);

        List<TEntity> InsertManyEntity(List<TEntity> objs);

        List<TEntity> UpdateManyEntity(List<TEntity> objs);

        TEntity Update(TEntity obj);

        bool Exists(Expression<Func<TEntity, bool>> predicate);

        IUserHelper GetUserHelper();

        DbContext GetDbContext();

        TEntity GetAutoInc(string schemaName, string fieldAutoInc);

        // new
        List<TEntity> ViewFilter(string jsonQuery);

        List<TEntity> GetWeeklySchedulePaginated(string? fornecedorId, string? ownerId, DateTime? dataInicio, DateTime? dataFim, bool sortDesc = false, string sortField = null);
    }
}