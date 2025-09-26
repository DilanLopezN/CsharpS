using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Simjob.Framework.Domain.Core.Entities;
using Simjob.Framework.Domain.Data;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Interfaces.Users;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Simjob.Framework.Domain.Models.JoinsModel;
using static Simjob.Framework.Domain.Models.Searchdefs;

namespace Simjob.Framework.Infra.Data.Repositories
{
    public class Repository<TContext, TEntity> : Framework.Domain.Interfaces.Repositories.IRepository<TContext, TEntity>
        where TEntity : Entity
        where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly IMongoCollection<TEntity> _collection;
        protected readonly IUserHelper _userHelper;
        private readonly IHttpContextAccessor _accessor;
        protected bool UserOnly;
        protected FilterDefinition<TEntity> UserOnlyFilter = Builders<TEntity>.Filter.Empty;

        public Repository(TContext context, IUserHelper userHelper, IHttpContextAccessor acessor, string collectionName = null)
        {
            _context = context;
            _collection = _context.GetDatabase().GetCollection<TEntity>(collectionName ?? typeof(TEntity).Name.ToLower());
            _userHelper = userHelper;
            _accessor = acessor;
            var claims = _userHelper.GetClaims();
            if (claims != null && claims.Any()) UserOnly = claims.Exists(c => c.Value == "useronly" && c.Type == typeof(TEntity).Name);
            if (UserOnly)
                UserOnlyFilter = Builders<TEntity>.Filter.Eq("CreateBy", _userHelper.GetUserName());
        }
        [ExcludeFromCodeCoverage]
        public List<TEntity> ViewFilter(string jsonQuery)
        {
            IMongoCollection<TEntity> coll;
            coll = _context.GetDatabase().GetCollection<TEntity>("account");


            var queryDoc = new QueryDocument(BsonSerializer.Deserialize<BsonDocument>(jsonQuery));
            FilterDefinition<TEntity> filters = UserOnlyFilter;

            var ohno = _context.GetDatabase().RunCommand<TEntity>(queryDoc);
            var ok = _collection.Find(filters).ToList();
            return ok;

        }
        [ExcludeFromCodeCoverage]
        public IUserHelper GetUserHelper() => _userHelper;

        public DbContext GetDbContext() => _context;
        [ExcludeFromCodeCoverage]
        protected FilterDefinition<J> GetUsertOnlyFilter<J>(string property = null)
        {
            return UserOnly ? Builders<J>.Filter.Eq(property ?? "CreateBy", _userHelper.GetUserName()) : Builders<J>.Filter.Empty;
        }

        public virtual void Delete(string id)
        {
            var username = _userHelper.GetUserName();

            var entity = this.GetById(id);
            if (entity == null) return;
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);
            var updateIsDeleted = Builders<TEntity>.Update.Set(o => o.IsDeleted, true)
                .Set(o => o.DeleteAt, DateTime.Now)
                .Set(o => o.DeleteBy, username);

            _collection.UpdateOne(filter, updateIsDeleted);
        }


        //public FilterDefinition<TEntity> searchFieldCreate(string[] objSearchOr, string[] objSearchAnd, string[] objValueOr, string[] objValueAnd)
        //{
        //    FilterDefinition<TEntity> orAndFilters = null;
        //    List<FilterDefinition<TEntity>> regexFilters = new();
        //    if (objSearchOr != null)
        //    {

        //        if (objSearchOr.Length == objValueOr.Length)
        //        {
        //            //EXEMPLO
        //            //[code,description]
        //            //[0001,evolution]
        //            for (var i = 0; i < objSearchOr.Length; i++)
        //            {
        //                // FILTER CAMPO[i] -> VALOR[i] DO TIPO AND

        //                regexFilters.Add(Builders<TEntity>.Filter.Regex(objSearchAnd[i], new BsonRegularExpression($"/^.*{objValueAnd[i].Replace(" ","_")}.*$/i")));
        //            }
        //            orAndFilters = Builders<TEntity>.Filter.And(regexFilters);
        //        }
        //        else
        //        {

        //            if (objSearchOr.Length > objValueOr.Length)
        //            {
        //                //EXEMPLO
        //                //[code,description]
        //                //[0001]
        //                for (var i = 0; i < objSearchOr.Length; i++)
        //                {
        //                    // FILTER CAMPO[i] -> VALOR[0] DO TIPO OR 
        //                    regexFilters.Add(Builders<TEntity>.Filter.Regex(objSearchOr[i], new BsonRegularExpression($"/^.*{objValueOr[0].Replace(" ", "_")}.*$/i")));
        //                }
        //                orAndFilters = Builders<TEntity>.Filter.Or(regexFilters);
        //            }
        //            else
        //            {
        //                //EXEMPLO
        //                //[code]
        //                //[0001,0002]
        //                for (var i = 0; i < objValueOr.Length; i++)
        //                {
        //                    // FILTER CAMPO[0] -> VALOR[i] DO TIPO OR
        //                    regexFilters.Add(Builders<TEntity>.Filter.Regex(objSearchOr[0], new BsonRegularExpression($"/^.*{objValueOr[i].Replace(" ", "_")}.*$/i")));
        //                }
        //                orAndFilters = Builders<TEntity>.Filter.Or(regexFilters);
        //            }
        //        }

        //        ////INICIA O AND
        //        //for (var i = 0; i < objSearchAnd.Length; i++)
        //        //{
        //        //    // FILTER CAMPO[i] -> VALOR[i] DO TIPO AND
        //        //    regexFilters.Add(Builders<TEntity>.Filter.Regex(objSearchAnd[i], new BsonRegularExpression($"/^.*{objValueAnd[i]}.*$/i")));
        //        //}
        //        //orAndFilters = Builders<TEntity>.Filter.And(regexFilters);


        //        //// COLOCAR IDS, CASO POSSUA
        //        ////sqlOr += ")) and ids='123123-12-312-312-3'";
        //    }
        //    else
        //    {

        //        for (var i = 0; i < objSearchAnd.Length; i++)
        //        {
        //            // FILTER CAMPO[i] -> VALOR[i] DO TIPO AND
        //            regexFilters.Add(Builders<TEntity>.Filter.Regex(objSearchAnd[i], new BsonRegularExpression($"/^.*{objValueAnd[i].Replace(" ", "_")}.*$/i")));
        //        }
        //        orAndFilters = Builders<TEntity>.Filter.And(regexFilters);
        //    }
        //    return orAndFilters;
        //}



        public static int countSearchFields = 0;
        public static FilterDefinition<TEntity> finish = Builders<TEntity>.Filter.Empty;
        // filter searchfields
        public FilterDefinition<TEntity> Filter(SchemaModel schemaModel, string searchfields, string value, string mode)
        {
            value = value.Replace("[[]]", "[]");
            value = value.Replace("[[", "[");
            value = value.Replace("]]", "]");
            searchfields = searchfields.Replace("[[", "[");
            searchfields = searchfields.Replace("]]", "]");

            var SearchRest = "";
            var ValueRest = "";
            var SearchUserOr = "";
            var ValueUserOr = "";
            List<string> objSearchUserOr = new List<string>();
            List<string> objValueUserOr = new List<string>();


            //TRATAMENTO DO QUE O USUARIO BUSCOU, CASO SEJA MAIS DE UM VALOR
            if (searchfields.Split(']').Length > 0 && searchfields.Contains("["))
            {
                SearchUserOr = searchfields.Split(']')[0].Replace("[", "");

                //pega o index do objeto que o usuario pesquisou e remove
                var indexSearchRest = searchfields.IndexOf(']');
                if (searchfields.Split(']')[1] != "")
                {
                    SearchRest = searchfields.Remove(0, indexSearchRest + 2);
                }
                else
                {
                    SearchRest = "";
                }
                objSearchUserOr = SearchUserOr.Split(',').ToList();

                if (value.Split(']').Length > 0 && value.Contains("["))
                {
                    ValueUserOr = value.Split(']')[0].Replace("[", "");
                    objValueUserOr = ValueUserOr.Split(',').ToList();
                    //pega o index do objeto que o usuario pesquisou e remove
                    var indexValueRest = value.IndexOf(']');
                    if (searchfields.Split(']')[1] != "")
                    {
                        ValueRest = value.Remove(0, indexValueRest + 2);
                    }
                    else
                    {
                        ValueRest = "";
                    }
                }


                finish &= SearchFieldsConditions(schemaModel, objSearchUserOr, objValueUserOr, mode);
                //finish &= SearchFieldsConditions(objSearchUserOr, objValueUserOr);

                if (finish == null) return null;
                if (SearchRest != "")
                {
                    if (SearchRest.Split(']').Length > 0 && SearchRest.Contains("["))
                    {
                        Filter(schemaModel, SearchRest, ValueRest, mode);
                    }
                    else
                    {
                        Console.WriteLine(SearchRest);
                        Console.WriteLine(ValueRest);
                        Filter(schemaModel, SearchRest, ValueRest, mode);
                    }
                }


            }// termina o if do userSearch
            else
            {
                objSearchUserOr.Add(searchfields);
                objValueUserOr.Add(value);
                var searchFieldsConditions = SearchFieldsConditions(schemaModel, objSearchUserOr, objValueUserOr, mode);
                finish &= SearchFieldsConditions(schemaModel, objSearchUserOr, objValueUserOr, mode);
                //finish &= SearchFieldsConditions(objSearchUserOr, objValueUserOr);
            }

            FilterDefinition<TEntity> finalFilters;
            finalFilters = Builders<TEntity>.Filter.And(finish);

            //var res = _collection.Find(finish).Skip((1 - 1) * 10).Limit(10);
            //var count = _collection.CountDocuments(finish);
            //var r = res.ToList();


            return finalFilters;
        }

        public static FilterDefinition<TEntity> SearchFieldsConditions(SchemaModel schemaModel, List<string> objSearchUserOr, List<string> objValueUserOr, string mode)
        {
            FilterDefinition<TEntity> orAndFilters = null;
            List<FilterDefinition<TEntity>> regexFilters = new();
            dynamic valueType = null;
            if (countSearchFields > 0)
            {
                //sqlOr += " and (";
            }
            //TRATANDO CONDIÇÃO AND
            if (objSearchUserOr.Count == objValueUserOr.Count)
            {
                //EXEMPLO
                //[code,description]
                //[0001,evolution]
                for (var i = 0; i < objSearchUserOr.Count; i++)
                {
                    // FILTER CAMPO[i] -> VALOR[i] DO TIPO AND

                    var eNum = schemaModel?.Properties.Where(x => x.Key == objSearchUserOr[i] && x.Value.@enum != null).FirstOrDefault();

                    if (eNum != null && eNum.Value.Key != null) objValueUserOr[i] = objValueUserOr[i].Replace(" ", "_");
                    var searchType = schemaModel?.Properties.Where(x => x.Key == objSearchUserOr[i] || x.Key == objSearchUserOr[i].ToLower()).FirstOrDefault();

                    if (schemaModel != null)
                    {
                        if (searchType.Value.Value != null)
                        {
                            valueType = searchType.Value.Value.Type != null ? searchType.Value.Value.Type : "";
                            if (valueType != "" && valueType == "integer" && objValueUserOr[i] != "")
                            {
                                if (int.TryParse(objValueUserOr[i], out int result))
                                {
                                    regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[i], int.Parse(objValueUserOr[i])));
                                }
                                    
                            }
                            else if (valueType != "" && valueType == "boolean" && objValueUserOr[i] != "")
                            {
                                if (bool.TryParse(objValueUserOr[i], out bool result))
                                {
                                    regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[i], bool.Parse(objValueUserOr[i])));
                                }
                            }
                            else
                            {
                                if (mode != "Equals" && (mode == "Contains" || mode == "" || (mode == "Range" && DateTime.TryParse(objValueUserOr[i], out DateTime result)) == false))
                                {
                                    regexFilters.Add(Builders<TEntity>.Filter.Regex(objSearchUserOr[i], new BsonRegularExpression($"/^.*{objValueUserOr[i]}.*$/i")));
                                }

                                else regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[i], objValueUserOr[i]));
                            }
                        }
                        else if (mode == "Contains" || mode == "") regexFilters.Add(Builders<TEntity>.Filter.Regex(objSearchUserOr[i], new BsonRegularExpression($"/^.*{objValueUserOr[i]}.*$/i")));
                        else regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[i], objValueUserOr[i]));
                    }
                    else if (mode == "Contains" || mode == "") regexFilters.Add(Builders<TEntity>.Filter.Regex(objSearchUserOr[i], new BsonRegularExpression($"/^.*{objValueUserOr[i]}.*$/i")));
                    else regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[i], objValueUserOr[i]));

                }
                orAndFilters = Builders<TEntity>.Filter.And(regexFilters);
            }


            //TRANDO CONDIÇÃO OR QUANDO O CAMPO É UM OBJETO MAIOR QUE O VALOR
            else if ((objSearchUserOr.Count > objValueUserOr.Count) && objValueUserOr.Count == 1)
            {
                //EXEMPLO
                //[code,description]
                //[0001]
                for (var i = 0; i < objSearchUserOr.Count; i++)
                {
                    // FILTER CAMPO[i] -> VALOR[0] DO TIPO OR
                    var eNum = schemaModel?.Properties.Where(x => x.Key == objSearchUserOr[i] && x.Value.@enum != null).FirstOrDefault();

                    if (eNum != null && eNum.Value.Key != null) objValueUserOr[0] = objValueUserOr[0].Replace(" ", "_");

                    var searchType = schemaModel?.Properties.Where(x => x.Key == objSearchUserOr[i] || x.Key == objSearchUserOr[i].ToLower()).FirstOrDefault();

                    if (schemaModel != null)
                    {
                        if (searchType.Value.Value != null)
                        {
                            valueType = searchType.Value.Value.Type != null ? searchType.Value.Value.Type : "";
                            if (valueType != "" && valueType == "integer" && objValueUserOr[0] != "")
                            {
                                if (int.TryParse(objValueUserOr[0], out int result))
                                {
                                    regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[i], int.Parse(objValueUserOr[0])));
                                }
                                    
                            }
                            else if (valueType != "" && valueType == "boolean" && objValueUserOr[0] != "")
                            {
                                if (bool.TryParse(objValueUserOr[0], out bool result))
                                {
                                    regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[i], bool.Parse(objValueUserOr[0])));
                                }
                                    
                            }
                            else
                            {
                                if (mode != "Equals" && (mode == "Contains" || mode == "" || (mode == "Range" && DateTime.TryParse(objValueUserOr[i], out DateTime result)) == false)) regexFilters.Add(Builders<TEntity>.Filter.Regex(objSearchUserOr[i], new BsonRegularExpression($"/^.*{objValueUserOr[0]}.*$/i")));

                                else regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[i], objValueUserOr[0]));
                            }
                        }
                        else if (mode == "Contains" || mode == "") regexFilters.Add(Builders<TEntity>.Filter.Regex(objSearchUserOr[i], new BsonRegularExpression($"/^.*{objValueUserOr[0]}.*$/i")));

                        else regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[i], objValueUserOr[0]));
                    }
                    else if (mode == "Contains" || mode == "") regexFilters.Add(Builders<TEntity>.Filter.Regex(objSearchUserOr[i], new BsonRegularExpression($"/^.*{objValueUserOr[0]}.*$/i")));

                    else regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[i], objValueUserOr[0]));

                }
                orAndFilters = Builders<TEntity>.Filter.Or(regexFilters);
            }

            //TRANDO CONDIÇÃO OR QUANDO O VALOR É UM OBJETO MAIOR QUE O CAMPO
            else if ((objSearchUserOr.Count < objValueUserOr.Count) && objSearchUserOr.Count == 1)
            {
                //EXEMPLO
                //[code]
                //[0001,0002,0003]
                for (var i = 0; i < objValueUserOr.Count; i++)
                {
                    // FILTER CAMPO[0] -> VALOR[i] DO TIPO OR
                    var eNum = schemaModel?.Properties.Where(x => x.Key == objSearchUserOr[0] && x.Value.@enum != null).FirstOrDefault();

                    if (eNum != null && eNum.Value.Key != null) objValueUserOr[i] = objValueUserOr[i].Replace(" ", "_");

                    var searchType = schemaModel?.Properties.Where(x => x.Key == objSearchUserOr[0] || x.Key == objSearchUserOr[0].ToLower()).FirstOrDefault();

                    if (schemaModel != null)
                    {
                        if (searchType.Value.Value != null)
                        {
                            valueType = searchType.Value.Value.Type != null ? searchType.Value.Value.Type : "";
                            if (valueType != "" && valueType == "integer" && objValueUserOr[i] != "")
                            {
                                if (mode == "Range")
                                {
                                    regexFilters.Add(Builders<TEntity>.Filter.AnyGte(objSearchUserOr[0], int.Parse(objValueUserOr[i])));
                                    regexFilters.Add(Builders<TEntity>.Filter.AnyLte(objSearchUserOr[0], int.Parse(objValueUserOr[i + 1])));
                                    break;
                                }
                                else
                                {
                                    regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[0], int.Parse(objValueUserOr[i])));
                                }

                            }
                            else if (valueType == "date")
                            {
                                if (mode == "Range")
                                {
                                    if (objValueUserOr[i] == objValueUserOr[i + 1])
                                    {
                                        regexFilters.Add(Builders<TEntity>.Filter.AnyGte(objSearchUserOr[0], DateTime.Parse(objValueUserOr[i])));
                                        regexFilters.Add(Builders<TEntity>.Filter.AnyLt(objSearchUserOr[0], DateTime.Parse(objValueUserOr[i + 1]).AddDays(1.0)));
                                    }
                                    else
                                    {
                                        regexFilters.Add(Builders<TEntity>.Filter.AnyGte(objSearchUserOr[0], DateTime.Parse(objValueUserOr[i])));
                                        regexFilters.Add(Builders<TEntity>.Filter.AnyLte(objSearchUserOr[0], DateTime.Parse(objValueUserOr[i + 1])));
                                    }

                                    break;
                                }
                            }
                            else if (valueType == "number")
                            {
                                if (mode == "Range")
                                {
                                    regexFilters.Add(Builders<TEntity>.Filter.AnyGte(objSearchUserOr[0], double.Parse(objValueUserOr[i])));
                                    regexFilters.Add(Builders<TEntity>.Filter.AnyLte(objSearchUserOr[0], double.Parse(objValueUserOr[i + 1])));
                                    break;
                                }

                            }
                            else if (valueType != "" && valueType == "boolean" && objValueUserOr[i] != "")
                            {
                                regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[i], bool.Parse(objValueUserOr[i])));
                            }
                            else if (mode == "Contains" || mode == "") regexFilters.Add(Builders<TEntity>.Filter.Regex(objSearchUserOr[0], new BsonRegularExpression($"/^.*{objValueUserOr[i]}.*$/i")));
                            else regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[0], objValueUserOr[i]));

                        }
                        else if (mode == "Contains" || mode == "") regexFilters.Add(Builders<TEntity>.Filter.Regex(objSearchUserOr[0], new BsonRegularExpression($"/^.*{objValueUserOr[i]}.*$/i")));
                        else regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[0], objValueUserOr[i]));
                    }
                    else if (mode == "Contains" || mode == "") regexFilters.Add(Builders<TEntity>.Filter.Regex(objSearchUserOr[0], new BsonRegularExpression($"/^.*{objValueUserOr[i]}.*$/i")));
                    else regexFilters.Add(Builders<TEntity>.Filter.Eq(objSearchUserOr[0], objValueUserOr[i]));

                }

                if (mode == "Range" && (valueType == "date" || valueType == "integer")) orAndFilters = Builders<TEntity>.Filter.And(regexFilters);
                else
                {
                    orAndFilters = Builders<TEntity>.Filter.Or(regexFilters);
                }
            }
            else
            {
                //return vazio
                Console.WriteLine("vazio");
                return null;
            }

            countSearchFields++;

            return orAndFilters;
        }

        public IEnumerable<TEntity> GroupBuilder(IFindFluent<TEntity, TEntity> res, List<TEntity> listRes, string groupBy, bool sortDesc, int page, int limit, string sortField)
        {
            IEnumerable<TEntity> result = null;
            if (sortDesc == true)
            {
                result = listRes.OrderByDescending(x => x.GetType().GetProperty(groupBy).GetValue(x, null)).Skip(((page - 1) * limit)).Take(limit);
            }
            else
            {
                result = listRes.OrderBy(x => x.GetType().GetProperty(groupBy).GetValue(x, null)).Skip(((page - 1) * limit)).Take(limit);
            }

            return result;
        }
        #region schemasJoin
        private List<FilterDefinition<TEntity>> FilterFields(SchemaModel schemaModel, List<WhereCondition> whereFilters)
        {
            List<FilterDefinition<TEntity>> filters = new List<FilterDefinition<TEntity>>();

            foreach (var filter in whereFilters)
            {
                var operation = filter.Operation;
                switch (operation)
                {
                    case "like":
                        filters.Add(Builders<TEntity>.Filter.Regex(filter.Field, new BsonRegularExpression($"/^.*{filter.Value}.*$/i")));
                        break;

                    case "==":
                        if (!schemaModel.Properties.ContainsKey(filter.Field)) filters.Add(Builders<TEntity>.Filter.Eq(filter.Field, filter.Value));
                        else if (schemaModel.Properties.Where(x => x.Key == filter.Field).First().Value.Type == "string") filters.Add(Builders<TEntity>.Filter.Eq(filter.Field, filter.Value));
                        else if (schemaModel.Properties.Where(x => x.Key == filter.Field).First().Value.Type == "number") filters.Add(Builders<TEntity>.Filter.Eq(filter.Field, double.Parse(filter.Value)));
                        else if (schemaModel.Properties.Where(x => x.Key == filter.Field).First().Value.Type == "date") filters.Add(Builders<TEntity>.Filter.Eq(filter.Field, DateTime.Parse(filter.Value)));
                        else filters.Add(Builders<TEntity>.Filter.Eq(filter.Field, filter.Value));

                        break;
                    case "!=":

                        if (!schemaModel.Properties.ContainsKey(filter.Field)) filters.Add(Builders<TEntity>.Filter.Not(Builders<TEntity>.Filter.Eq(filter.Field, filter.Value)));
                        else if (schemaModel.Properties.Where(x => x.Key == filter.Field).First().Value.Type == "string") filters.Add(Builders<TEntity>.Filter.Not(Builders<TEntity>.Filter.Eq(filter.Field, filter.Value)));
                        else if (schemaModel.Properties.Where(x => x.Key == filter.Field).First().Value.Type == "number") filters.Add(Builders<TEntity>.Filter.Not(Builders<TEntity>.Filter.Eq(filter.Field, double.Parse(filter.Value))));
                        else if (schemaModel.Properties.Where(x => x.Key == filter.Field).First().Value.Type == "date") filters.Add(Builders<TEntity>.Filter.Not(Builders<TEntity>.Filter.Eq(filter.Field, DateTime.Parse(filter.Value))));
                        else filters.Add(Builders<TEntity>.Filter.Not(Builders<TEntity>.Filter.Eq(filter.Field, filter.Value)));

                        break;
                    case "<":

                        filters.Add(Builders<TEntity>.Filter.Lt(filter.Field, double.Parse(filter.Value)));
                        break;

                    case ">":

                        filters.Add(Builders<TEntity>.Filter.Gt(filter.Field, double.Parse(filter.Value)));
                        break;

                    case "<=":

                        filters.Add(Builders<TEntity>.Filter.Lte(filter.Field, double.Parse(filter.Value)));
                        break;

                    case ">=":

                        filters.Add(Builders<TEntity>.Filter.Gte(filter.Field, double.Parse(filter.Value)));
                        break;
                    case "between":

                        filters.Add(Builders<TEntity>.Filter.Gte(filter.Field, DateTime.Parse(filter.Value.Split(',')[0])));
                        filters.Add(Builders<TEntity>.Filter.Lte(filter.Field, DateTime.Parse(filter.Value.Split(',')[1])));
                        break;

                    case "in":
                        filters.Add(Builders<TEntity>.Filter.In(filter.Field, filter.Value.Split(',')));
                        break;
                    default:
                        throw new ArgumentException("Invalid operation");
                }
            }
            return filters;
        }
        public PaginationData<TEntity> SearchRegexByFieldsJoin(SchemaModel schemaModel, JoinsModel model, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        {
            SortDefinition<TEntity> sort = null;
            //if (model.Order != null)
            //{
            //    sortDesc = model.Order.Sort == "desc" ? true : false;
            //    sortField = model.Order.Field;
            //}
            List<FilterDefinition<TEntity>> andFilters = new List<FilterDefinition<TEntity>>();
            List<FilterDefinition<TEntity>> orFilters = new List<FilterDefinition<TEntity>>();
            FilterDefinition<TEntity> finalAndFilters = null;
            FilterDefinition<TEntity> finalOrFilters = null;
            if (!model.Where.IsNullOrEmpty())
            {
                model.Where = model.Where.Where(x => !x.Field.Contains(".")).ToList();
                andFilters = FilterFields(schemaModel, model.Where);
                finalAndFilters = Builders<TEntity>.Filter.And(andFilters);
            }

            if (!model.WhereOr.IsNullOrEmpty())
            {
                model.WhereOr = model.WhereOr.Where(x => !x.Field.Contains(".")).ToList();
                orFilters = FilterFields(schemaModel, model.WhereOr);
                finalOrFilters = Builders<TEntity>.Filter.Or(orFilters);
            }
            if (orFilters.Count() == 0) finalOrFilters = Builders<TEntity>.Filter.Empty;
            if (andFilters.Count() == 0) finalAndFilters = Builders<TEntity>.Filter.Empty;
            var filterIsDeleted = Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);

            var filters = filterIsDeleted & finalAndFilters & finalOrFilters & UserOnlyFilter;
            if (!string.IsNullOrEmpty(sortField)) sort = sortDesc ? Builders<TEntity>.Sort.Descending(sortField) : Builders<TEntity>.Sort.Ascending(sortField);
            var count = _collection.CountDocuments(filters);
            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filters).Skip((page - 1) * limit).Limit(limit) : _collection.Find(filters).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<TEntity>(res.ToList(), page, limit, count);
        }
        #endregion
        public PaginationData<TEntity> SearchRegexByFields(string fields, string search, string mode, SchemaModel schemaModel, int? page = null, int? limit = null, string sortField = null, string groupBy = null, bool sortDesc = false, string ids = null, List<string> relationIds = null, string relationField = "", string companySiteIds = "")
        {
            var filterRelationIds = Builders<TEntity>.Filter.Empty;

            if (relationIds != null && relationIds.Count() > 0 && relationField != "") filterRelationIds = Builders<TEntity>.Filter.In(relationField, relationIds);
            countSearchFields = 0;
            finish = Builders<TEntity>.Filter.Empty;

            //if (ids != null && !ids.Any()) return new PaginationData<TEntity>(new List<TEntity>() { }, page, limit, 0);

            //FilterDefinition<TEntity> orAndFilters = null;
            //verificar se search fields é um objeto e tem mais de um valor buscado (ex: [code,description,cpf],status])

            FilterDefinition<TEntity> filters = Builders<TEntity>.Filter.Empty;


            var filterIds = string.IsNullOrEmpty(ids) ? Builders<TEntity>.Filter.Empty : string.IsNullOrEmpty(schemaModel.PrimaryKey) ?Builders<TEntity>.Filter.In("Id", ids.Split(',')) : Builders<TEntity>.Filter.In(schemaModel.PrimaryKey, ids.Split(',')
                                                                                                          .Select(id => int.Parse(id))
                                                                                                          .ToArray());


            var filterCompanyIds = string.IsNullOrEmpty(companySiteIds) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.In("companySiteId", companySiteIds.Split(','));

            SortDefinition<TEntity> sort = null;

            var filterIsDeleted = Builders<TEntity>.Filter.Eq(e => e.IsDeleted, false);
            IEnumerable<TEntity> resGroup = null;
            if (!string.IsNullOrEmpty(sortField)) sort = sortDesc ? Builders<TEntity>.Sort.Descending(sortField) : Builders<TEntity>.Sort.Ascending(sortField);
            else sort = Builders<TEntity>.Sort.Descending("createAt");

            if (fields.Length == 0 && search.Length == 0)
            {
                // return tudo
                filters = filterRelationIds & filterIds & filterCompanyIds & filterIsDeleted;

                //var filterISDeleted = Builders<TEntity>.Filter.Eq(e => e.IsDeleted, false);
                var resAll = _collection.Find(filters).Sort(sort).Skip((page - 1) * limit).Limit(limit);


                var countAll = _collection.CountDocuments(filters);

                if (groupBy != null)
                {
                    List<TEntity> listRes = new List<TEntity>();

                    var grouping = _collection.Find(filters).ToList().GroupBy(x => x?.GetType().GetProperty(groupBy).GetValue(x, null));
                    foreach (var group in grouping) listRes.Add(group.FirstOrDefault());

                    resGroup = GroupBuilder(_collection.Find(filters), listRes, groupBy, sortDesc, (int)page, (int)limit, sortField);
                    return new PaginationData<TEntity>(resGroup, page, limit, listRes.Count());
                }

                return new PaginationData<TEntity>(resAll.ToList(), page, limit, countAll);
            }
            //FilterDefinition<TEntity> filters = Builders<TEntity>.Filter.Empty;
            var filtros = Builders<TEntity>.Filter.Empty;
            if (fields.Contains("callConfig.schemaName"))
            {
                filtros = Builders<TEntity>.Filter.Regex(fields, new BsonRegularExpression($"/^.*{search}.*$/i"));
            }

            else
            {
                filtros = Builders<TEntity>.Filter.And(Filter(schemaModel, fields, search, mode));
            }

            filters = filtros & filterIds & filterCompanyIds & filterRelationIds & filterIsDeleted;

            var count = _collection.CountDocuments(filters);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = _collection.Find(filters).Sort(sort);

                if (groupBy != null)
                {
                    List<TEntity> listRes = new List<TEntity>();

                    var grouping = _collection.Find(filters).ToList().GroupBy(x => x?.GetType().GetProperty(groupBy).GetValue(x, null));
                    foreach (var group in grouping) listRes.Add(group.FirstOrDefault());

                    resGroup = GroupBuilder(_collection.Find(filters), listRes, groupBy, sortDesc, (int)page, (int)limit, sortField);
                    return new PaginationData<TEntity>(resGroup, page, limit, listRes.Count());
                }

                return new PaginationData<TEntity>(resNoLimit.ToList(), page, limit, count);
            }


            var res = _collection.Find(filters).Sort(sort).Skip((page - 1) * limit).Limit(limit);

            if (groupBy != null)
            {
                List<TEntity> listRes = new List<TEntity>();

                var grouping = _collection.Find(filters).ToList().GroupBy(x => x?.GetType().GetProperty(groupBy).GetValue(x, null));
                foreach (var group in grouping) listRes.Add(group.FirstOrDefault());

                resGroup = GroupBuilder(_collection.Find(filters), listRes, groupBy, sortDesc, (int)page, (int)limit, sortField);

                return new PaginationData<TEntity>(resGroup, page, limit, listRes.Count());
            }

            return new PaginationData<TEntity>(res.ToList(), page, limit, count);
        }

        public virtual PaginationData<TEntity> SearchRegexByFieldsDefs(List<Filter> defs, List<bool> sortDesc, string fields, string search, SchemaModel schemaModel, int? page = null, int? limit = null, string sortField = null, string ids = null, string companySiteIds = "")
        {
            countSearchFields = 0;
            finish = Builders<TEntity>.Filter.Empty;
            if (ids != null && !ids.Any()) return new PaginationData<TEntity>(new List<TEntity>() { }, page, limit, 0);

            //SortDefinition<TEntity> sort = null;
            var filterIsDeleted = Builders<TEntity>.Filter.Eq(e => e.IsDeleted, false);

            FilterDefinition<TEntity> filters = Builders<TEntity>.Filter.Empty;

            SortDefinition<TEntity> sort = null;

            var filtros = Builders<TEntity>.Filter.Empty;
            if (fields.Contains("callConfig.schemaName"))
            {
                filtros = Builders<TEntity>.Filter.Regex(fields, new BsonRegularExpression($"/^.*{search}.*$/i"));
            }
            else
            {
                filtros = Builders<TEntity>.Filter.And(Filter(schemaModel, fields, search, ""));
            }


            List<FilterDefinition<TEntity>> newFilterOr = new();
            List<FilterDefinition<TEntity>> newFilterAnd = new();

            foreach (var filtro in defs)
            {
                switch (filtro.@operator)
                {
                    case "greaterThan":
                        if (defs.Where(x => x.field == filtro.field).Count() > 1) newFilterOr.Add(Builders<TEntity>.Filter.AnyGt(filtro.field, filtro.value));

                        else newFilterAnd.Add(Builders<TEntity>.Filter.AnyGt(filtro.field, filtro.value));
                        break;

                    case "equalsTo":
                        if (defs.Where(x => x.field == filtro.field).Count() > 1) newFilterOr.Add(Builders<TEntity>.Filter.Eq(filtro.field, filtro.value));

                        else newFilterAnd.Add(Builders<TEntity>.Filter.Eq(filtro.field, filtro.value));
                        break;

                    case "notEqualsTo":
                        if (defs.Where(x => x.field == filtro.field).Count() > 1) newFilterOr.Add(Builders<TEntity>.Filter.AnyNe(filtro.field, filtro.value));

                        else newFilterAnd.Add(Builders<TEntity>.Filter.AnyNe(filtro.field, filtro.value));
                        break;

                    case "contains":
                        if (defs.Where(x => x.field == filtro.field).Count() > 1) newFilterOr.Add(Builders<TEntity>.Filter.Regex(filtro.field, new BsonRegularExpression($"/^.*{filtro.value}.*$/i")));

                        else newFilterAnd.Add(Builders<TEntity>.Filter.Regex(filtro.field, new BsonRegularExpression($"/^.*{filtro.value}.*$/i")));
                        break;

                    case "notContains":
                        if (defs.Where(x => x.field == filtro.field).Count() > 1) newFilterOr.Add(Builders<TEntity>.Filter.Regex(filtro.field, new BsonRegularExpression($"^((?!{filtro.value}).)*$")));

                        else newFilterAnd.Add(Builders<TEntity>.Filter.Regex(filtro.field, new BsonRegularExpression($"^((?!{filtro.value}).)*$")));
                        break;

                    case "greaterThanOrEqualsTo":
                        if (defs.Where(x => x.field == filtro.field).Count() > 1) newFilterOr.Add(Builders<TEntity>.Filter.AnyGte(filtro.field, filtro.value));
                        else newFilterAnd.Add(Builders<TEntity>.Filter.AnyGte(filtro.field, filtro.value));

                        break;

                    case "lessThan":
                        if (defs.Where(x => x.field == filtro.field).Count() > 1) newFilterOr.Add(Builders<TEntity>.Filter.AnyLt(filtro.field, filtro.value));

                        else newFilterAnd.Add(Builders<TEntity>.Filter.AnyLt(filtro.field, filtro.value));
                        break;

                    case "lessThanOrEqualTo":
                        if (defs.Where(x => x.field == filtro.field).Count() > 1) newFilterOr.Add(Builders<TEntity>.Filter.AnyLte(filtro.field, filtro.value));

                        else newFilterAnd.Add(Builders<TEntity>.Filter.AnyLte(filtro.field, filtro.value));
                        break;
                }
            }

            //if (newFilterOr.Count() == 0) newFilterOr.Add(Builders<TEntity>.Filter.Empty);
            //if (newFilterAnd.Count() == 0) newFilterAnd.Add(Builders<TEntity>.Filter.Empty);

            var filterConcatOr = newFilterOr.Count() == 0 ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Or(newFilterOr);


            var filterConcatAnd = newFilterAnd.Count() == 0 ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.And(newFilterAnd);


            var filterCompanyIds = string.IsNullOrEmpty(companySiteIds) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.In("companySiteId", companySiteIds.Split(','));

            long count = 0;
         
            IFindFluent<TEntity, TEntity> res;
            if (fields.Length == 0 && search.Length == 0)
            {
                filters = filterIsDeleted & UserOnlyFilter & filterConcatAnd & filterConcatOr & filterCompanyIds;
            }
            else
            {
                filters = filterIsDeleted & UserOnlyFilter & filtros & filterConcatAnd & filterConcatOr & filterCompanyIds;
            }
            List<SortDefinition<TEntity>> sortList = new List<SortDefinition<TEntity>>();
            if (!string.IsNullOrEmpty(sortField))
            {
                var fieldSplit = sortField.Split(',');
                for (int i = 0; i < fieldSplit.Length; i++)
                {
                    sort = sortDesc[i] ? Builders<TEntity>.Sort.Descending(fieldSplit[i]) : Builders<TEntity>.Sort.Ascending(fieldSplit[i]);
                    sortList.Add(sort);
                }
                sort = Builders<TEntity>.Sort.Combine(sortList);
            }
            count = _collection.CountDocuments(filters);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filters) : _collection.Find(filters).Sort(sort);
                return new PaginationData<TEntity>(resNoLimit.ToList(), page, limit, count);
            }

            res = string.IsNullOrEmpty(sortField) ? _collection.Find(filters).Skip((page - 1) * limit).Limit(limit) : _collection.Find(filters).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<TEntity>(res.ToList(), page, limit, count);
        }

        #region stock
        public List<TEntity> GetAllPicking(string? codigo, string? localDestDescription, string? status, DateTime? dtInicio, DateTime? dtFinal, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null)
        {
            if (ids != null && !ids.Any()) return new List<TEntity> { };

            SortDefinition<TEntity> sort = null;
            var filterIds = string.IsNullOrEmpty(ids) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.In("Id", ids.Split(','));
            var filterCodigo = string.IsNullOrEmpty(codigo) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("codigo", codigo);
            var filterLocalDest = string.IsNullOrEmpty(localDestDescription) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("localDestDescription", localDestDescription);
            var filterStatus = string.IsNullOrEmpty(status) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("status", status);
            var filterDtInicio = dtInicio == null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Gte("datePicking", dtInicio);
            var filterDtFinal = dtFinal == null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Lte("datePicking", dtFinal);
            var filterIsDeleted = addIsDeleted ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);

            var filters = filterIsDeleted & UserOnlyFilter & filterIds & filterCodigo & filterLocalDest & filterStatus & filterDtInicio & filterDtFinal;
            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<TEntity>.Sort.Descending(sortField) : Builders<TEntity>.Sort.Ascending(sortField);

            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filters) : _collection.Find(filters).Sort(sort);
            return res.ToList();
        }

        public List<TEntity> GetAllItems(string? idItem, string? codigoItem)
        {
            var filterIdItem = string.IsNullOrEmpty(idItem) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("Id", idItem);
            var filterCodigoItem = string.IsNullOrEmpty(codigoItem) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("codeExt", codigoItem);
            var filterIsDeleted = Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);
            var filters = filterIsDeleted & UserOnlyFilter & filterIdItem & filterCodigoItem;
            var res = _collection.Find(filters);
            return res.ToList();
        }

        public List<TEntity> GetAllStockBalance(string? idItem, string? idLocal, string? codLocal, DateTime? dataInicioSaldo, DateTime? dataFinalSaldo)
        {

            var filterIdItem = string.IsNullOrEmpty(idItem) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("itemId", idItem);
            var filterIdLocal = string.IsNullOrEmpty(idLocal) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("stockLocId", idLocal);
            var filterCodLocal = string.IsNullOrEmpty(codLocal) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("stockLocCode", codLocal);
            var filterDtInicio = dataInicioSaldo == null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Gte("date", dataInicioSaldo);
            var filterDtFinal = dataFinalSaldo == null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Lte("date", dataFinalSaldo);
            var filterIsDeleted = Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);
            var filters = filterIsDeleted & UserOnlyFilter & filterIdItem & filterIdLocal;
            var sort = Builders<TEntity>.Sort.Descending("date");
            var res = _collection.Find(filters).Sort(sort);
            return res.ToList();
        }
        public List<TEntity> GetAllStockBalanceForItemRepo(List<string> itemIds)
        {
            var filterIdItem = Builders<TEntity>.Filter.In("itemId", itemIds);
            var filterIsDeleted = Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);
            var filters = filterIsDeleted & UserOnlyFilter & filterIdItem;
            var sort = Builders<TEntity>.Sort.Descending("date");
            var res = _collection.Find(filters).Sort(sort);
            return res.ToList();
        }
        public List<TEntity> GetStockBalanceByLocalCodes(string[] localCodes)
        {
            var filterLocalIds = localCodes.IsNullOrEmpty() ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.In("stockLocCode", localCodes);
            var filterIsDeleted = Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);
            var filters = filterIsDeleted & UserOnlyFilter & filterLocalIds;

            var sort = Builders<TEntity>.Sort.Descending("createAt");
            var res = _collection.Find(filters).Sort(sort);
            return res.ToList();
        }

        public TEntity GetStockBalanceByItemId(string itemId)
        {
            var filterItemId = Builders<TEntity>.Filter.Eq("itemId", itemId);
            var filterIsDeleted = Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);
            var filters = filterIsDeleted & UserOnlyFilter & filterItemId;

            var sort = Builders<TEntity>.Sort.Descending("createAt");
            var res = _collection.Find(filters).Sort(sort);
            return res.FirstOrDefault();
        }
        public List<TEntity> GetAllNf(string? nota, DateTime? dtInicio, DateTime? dtFinal, string? status, string? sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null)
        {
            if (ids != null && !ids.Any()) return new List<TEntity> { };

            SortDefinition<TEntity> sort = null;
            var filterIds = string.IsNullOrEmpty(ids) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.In("Id", ids.Split(','));
            var filterNumero = string.IsNullOrEmpty(nota) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("numero", nota);
            var filterStatus = string.IsNullOrEmpty(status) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("status", status);
            var filterDtInicio = dtInicio == null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Gte("dataEmissao", dtInicio);
            var filterDtFinal = dtFinal == null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Lte("dataEmissao", dtFinal);
            var filterIsDeleted = addIsDeleted ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);

            var filters = filterIsDeleted & UserOnlyFilter & filterIds & filterNumero & filterStatus & filterDtInicio & filterDtFinal;
            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<TEntity>.Sort.Descending(sortField) : Builders<TEntity>.Sort.Ascending(sortField);

            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filters) : _collection.Find(filters).Sort(sort);
            return res.ToList();
        }
        public List<TEntity> GetByIdsPickingItem(List<string> ids, string? lote)
        {
            var filterIds = Builders<TEntity>.Filter.In("Id", ids);
            var filterLote = string.IsNullOrEmpty(lote) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("lote", lote);
            var filterIsDeleted = Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);
            var filters = filterIds & filterLote & filterIsDeleted;
            return _collection.Find(filters).ToList();
        }

        public List<TEntity> GetByIdsList(List<string> ids)
        {
            var filterIds = Builders<TEntity>.Filter.In("Id", ids);
            var filterIsDeleted = Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);
            var filters = filterIds & filterIsDeleted;
            return _collection.Find(filters).ToList();
        }

        public List<TEntity> GetByFieldsIntList(string field, List<int> values)
        {
            var filterIds = Builders<TEntity>.Filter.In(field, values);
            var filterIsDeleted = Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);
            var filters = filterIds & filterIsDeleted;
            return _collection.Find(filters).ToList();
        }
        public TEntity GetByIdStockOperation(string id, string code, string status)
        {
            var filterId = Builders<TEntity>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            var filterCode = string.IsNullOrEmpty(code) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("code", code);
            var filterStatus = string.IsNullOrEmpty(status) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("status", status);
            return _collection.Find(filterId & filterIsDeleted & filterCode & filterStatus & UserOnlyFilter).FirstOrDefault();
        }

        public TEntity GetByNfConfirmacaoEntrega(string numero, DateTime? dtInicio, DateTime? dtFinal, string status)
        {
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            var filterNumero = string.IsNullOrEmpty(numero) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("nf", numero);
            var filterDtInicio = dtInicio == null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Gte("createAt", dtInicio);
            var filterDtFinal = dtFinal == null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Lte("createAt", dtFinal);
            var filterStatus = string.IsNullOrEmpty(status) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("status", status);
            return _collection.Find(filterIsDeleted & filterNumero & filterStatus & filterDtInicio & filterDtFinal & UserOnlyFilter).FirstOrDefault();
        }
        public List<TEntity> GetByIdsStockMov(List<string> ids, string codeExt, string volCode)
        {
            var filterIds = Builders<TEntity>.Filter.In("Id", ids);
            var filterCodeExt = string.IsNullOrEmpty(codeExt) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("codeExt", codeExt);
            var filterVolCode = string.IsNullOrEmpty(volCode) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("stockVolCode", volCode);
            var filterVolCodeOrigem = string.IsNullOrEmpty(volCode) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("stockVolCodeOrigem", volCode);
            var filterIsDeleted = Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);
            var filters = filterIds & filterIsDeleted & filterCodeExt & (filterVolCode | filterVolCodeOrigem);
            return _collection.Find(filters).ToList();
        }
        #endregion


        public List<TEntity> GetWeeklySchedulePaginated(string? fornecedorId ,string? ownerId,DateTime? dataInicio, DateTime? dataFim, bool sortDesc = false, string sortField = null)
        {
            SortDefinition<TEntity> sort = null;
            var filterFornecedor = fornecedorId == null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("fornecedorId", fornecedorId);
            var filterDtInicio = dataInicio == null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Gte("data", dataInicio);
            var filterDtFinal = dataFim == null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Lte("data", dataFim.Value.AddDays(1));
            var filterIsDeleted = Builders<TEntity>.Filter.Where(e => e.IsDeleted == false); 
            var filterOwnerId = string.IsNullOrEmpty(ownerId) ? Builders<TEntity>.Filter.Empty  : Builders<TEntity>.Filter.In("owners",new List<string> { ownerId });
            var filters = filterIsDeleted & UserOnlyFilter  & filterFornecedor & filterDtInicio & filterOwnerId & filterDtFinal;
            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<TEntity>.Sort.Descending(sortField) : Builders<TEntity>.Sort.Ascending(sortField);

            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filters) : _collection.Find(filters).Sort(sort);
            return res.ToList();
        }
        public virtual PaginationData<TEntity> GetAll(int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null)
        {
            if (ids != null && !ids.Any()) return new PaginationData<TEntity>(new List<TEntity>() { }, page, limit, 0);

            SortDefinition<TEntity> sort = null;
            var filterIds = string.IsNullOrEmpty(ids) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.In("Id", ids.Split(','));
            var filterIsDeleted = addIsDeleted ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);

            var filters = filterIsDeleted & UserOnlyFilter & filterIds;
            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<TEntity>.Sort.Descending(sortField) : Builders<TEntity>.Sort.Ascending(sortField);
            var count = _collection.CountDocuments(filters);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filters) : _collection.Find(filters).Sort(sort);
                return new PaginationData<TEntity>(resNoLimit.ToList(), page, limit, count);
            }

            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filters).Skip((page - 1) * limit).Limit(limit) : _collection.Find(filters).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<TEntity>(res.ToList(), page, limit, count);
        }
        public PaginationData<TEntity> GetAllModules(string? module, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null)
        {
            if (ids != null && !ids.Any()) return new PaginationData<TEntity>(new List<TEntity>() { }, page, limit, 0);

            SortDefinition<TEntity> sort = null;
            var filterIds = string.IsNullOrEmpty(ids) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.In("Id", ids.Split(','));
            var filterIsDeleted = addIsDeleted ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);

            //var filterModule = string.IsNullOrEmpty(module) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Regex("jsonValue", new BsonRegularExpression($"/^.*{module}.*$/i"));
            
            var filterModule = string.IsNullOrEmpty(module) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Regex("jsonValue", new BsonRegularExpression($@".*\""module\""\s*:\s*\""{module}\"".*", "i"));



            var filters = filterIsDeleted & filterModule & UserOnlyFilter & filterIds;
            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<TEntity>.Sort.Descending(sortField) : Builders<TEntity>.Sort.Ascending(sortField);
            var count = _collection.CountDocuments(filters);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filters) : _collection.Find(filters).Sort(sort);
                return new PaginationData<TEntity>(resNoLimit.ToList(), page, limit, count);
            }

            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filters).Skip((page - 1) * limit).Limit(limit) : _collection.Find(filters).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<TEntity>(res.ToList(), page, limit, count);
        }

        public PaginationData<TEntity> GetAllModulePermissions(string? userId, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null, string moduleIds = null,string userIds = null,string groupIds = null)
        {
            if (ids != null && !ids.Any()) return new PaginationData<TEntity>(new List<TEntity>() { }, page, limit, 0);

            SortDefinition<TEntity> sort = null;
            var filterIds = string.IsNullOrEmpty(ids) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.In("Id", ids.Split(','));
            var filterModuleIds = string.IsNullOrEmpty(moduleIds) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.In("moduleId", moduleIds.Split(','));

            var filterGroupIds = string.IsNullOrEmpty(groupIds) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.In("groupId", groupIds.Split(','));

            var filterUserIds = string.IsNullOrEmpty(userIds) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.In("userId", userIds.Split(','));

            var filterIsDeleted = addIsDeleted ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);
            var filterUserId = string.IsNullOrEmpty(userId) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Eq("userId", userId);

            var filters = filterIsDeleted & UserOnlyFilter & filterIds & filterUserId & filterModuleIds & filterGroupIds;
            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<TEntity>.Sort.Descending(sortField) : Builders<TEntity>.Sort.Ascending(sortField);
            var count = _collection.CountDocuments(filters);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filters) : _collection.Find(filters).Sort(sort);
                return new PaginationData<TEntity>(resNoLimit.ToList(), page, limit, count);
            }

            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filters).Skip((page - 1) * limit).Limit(limit) : _collection.Find(filters).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<TEntity>(res.ToList(), page, limit, count);
        }

        public virtual async Task<List<TEntity>> GetAllList()
        {
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            //  return _collection.Find(filterField & filterIsDeleted).FirstOrDefault();
            return await _collection.Find(filterIsDeleted & UserOnlyFilter).ToListAsync();
        }
        public PaginationData<TEntity> GetAllRelationSchema(string id, string fieldUser, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false, string ids = null)
        {
            //schemaName = schemaName.Replace(schemaName[0], char.ToLower(schemaName[0]));
            //IMongoCollection<TEntity> coll;
            //coll = _context.GetDatabase().GetCollection<TEntity>(schemaName);
            var filterRelationId = Builders<TEntity>.Filter.Eq(fieldUser, id);

            if (ids != null && !ids.Any()) return new PaginationData<TEntity>(new List<TEntity>() { }, page, limit, 0);

            SortDefinition<TEntity> sort = null;
            var filterIds = string.IsNullOrEmpty(ids) ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.In("Id", ids.Split(','));
            var filterIsDeleted = addIsDeleted ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);

            var filters = filterIsDeleted & UserOnlyFilter & filterIds & filterRelationId;
            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<TEntity>.Sort.Descending(sortField) : Builders<TEntity>.Sort.Ascending(sortField);
            var count = _collection.CountDocuments(filters);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filters) : _collection.Find(filters).Sort(sort);
                return new PaginationData<TEntity>(resNoLimit.ToList(), page, limit, count);
            }

            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filters).Skip((page - 1) * limit).Limit(limit) : _collection.Find(filters).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<TEntity>(res.ToList(), page, limit, count);
        }
        public virtual List<TEntity> GetAllSchedule(bool addIsDeleted = false)
        {

            var filterExecuted = Builders<TEntity>.Filter.Eq("isExecuted", false);
            var filterDate = Builders<TEntity>.Filter.AnyLt("nextActionRun", DateTime.Now);

            var filterIsDeleted = addIsDeleted ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Where(e => e.IsDeleted == false);

            var filtros = Builders<TEntity>.Filter.And(filterExecuted, filterDate, filterIsDeleted);
            return _collection.Find(filtros).ToList();

        }



        [ExcludeFromCodeCoverage]
        public TEntity GetAutoInc(string schemaName, string fieldAutoInc)
        {
            if (schemaName == null) return null;
            var filter = Builders<TEntity>.Filter.Empty;
            var sort = Builders<TEntity>.Sort.Descending(fieldAutoInc);
            var res = _collection.Find(filter).Sort(sort).Limit(1).FirstOrDefault();
            return (TEntity)res;
        }
        public virtual TEntity GetById(string id)
        {
            var filterId = Builders<TEntity>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefault();

        }


        public virtual async Task<TEntity> GetByIdAsync(string id)
        {
            var filterId = Builders<TEntity>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            return await _collection.Find(filterId & filterIsDeleted & UserOnlyFilter).FirstOrDefaultAsync();
        }

        public virtual TEntity GetByIdSchedule(string id)
        {
            var filterExecuted = Builders<TEntity>.Filter.Eq("isExecuted", false);
            var filterId = Builders<TEntity>.Filter.Eq("actionId", id);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterId & filterIsDeleted & filterExecuted & UserOnlyFilter).FirstOrDefault();

        }

        public virtual PaginationData<TEntity> GetByIds(string[] ids, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = true)
        {
            var filterIds = Builders<TEntity>.Filter.In("Id", ids);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);

            SortDefinition<TEntity> sort = null;

            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<TEntity>.Sort.Descending(sortField) : Builders<TEntity>.Sort.Ascending(sortField);
            var count = _collection.CountDocuments(filterIsDeleted & UserOnlyFilter & filterIds);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filterIsDeleted & UserOnlyFilter & filterIds) : _collection.Find(filterIsDeleted & UserOnlyFilter & filterIds).Sort(sort);
                return new PaginationData<TEntity>(resNoLimit.ToList(), page, limit, count);
            }

            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filterIsDeleted & UserOnlyFilter & filterIds).Skip((page - 1) * limit).Limit(limit) : _collection.Find(filterIsDeleted & UserOnlyFilter & filterIds).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<TEntity>(res.ToList(), page, limit, count);
        }

        public virtual void SchemaIndexSync(List<string> indexFields = null)
        {
            var indexes = _collection.Indexes.List().ToList();
            foreach (var index in indexes)
            {
                if (index["name"] != "_id_")
                {
                    _collection.Indexes.DropOne(index["name"].AsString);
                }
            }

            if (indexFields.IsNullOrEmpty()) indexFields = new List<string>() { };
            indexFields.AddRange(new List<string>() { "createAt", "updateAt", "deleteAt", "updateBy", "createBy", "deleteBy", "isDeleted" });

            IndexKeysDefinition<TEntity> indexKeysDefinition = null;
            string indexName = "";
            foreach (var field in indexFields)
            {
                if (field != "isDeleted")
                {
                    indexName = $"_{field}_{"isDeleted"}_";

                    indexKeysDefinition = Builders<TEntity>.IndexKeys
                        .Ascending(field)
                        .Ascending(entity => entity.IsDeleted);
                }
                else
                {
                    indexName = $"_{"isDeleted"}_";

                    indexKeysDefinition = Builders<TEntity>.IndexKeys
                        .Ascending(entity => entity.IsDeleted);
                }
                var options = new CreateIndexOptions { Name = indexName };

                var indexModel = new CreateIndexModel<TEntity>(indexKeysDefinition, options);

                _collection.Indexes.CreateOne(indexModel);
            }
        }

        public virtual TEntity Insert(TEntity obj)
        {
            //if (isSchema) SchemaIndexSync(indexFields);
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            //obj.CreateBy = createBy;
            obj.CreateAt = DateTime.Now;
            _collection.InsertOne(obj);
            return GetById(obj.Id);
        }

        public virtual List<TEntity> InsertManyEntity(List<TEntity> objs)
        {
            objs = objs.Select(obj =>
            {
                obj.CreateAt = DateTime.Now;
                return obj;
            }).ToList();

            _collection.InsertMany(objs);
            return GetByIds(objs.Select(o => o.Id).ToArray())?.Data?.ToList();
        }
        public async Task<TEntity> InsertAsync(TEntity obj)
        {
            //if (isSchema) SchemaIndexSync(indexFields);
            if (string.IsNullOrEmpty(obj.Id)) obj.Id = Guid.NewGuid().ToString();
            //obj.CreateBy = createBy;
            obj.CreateAt = DateTime.Now;
            try
            {
                await _collection.InsertOneAsync(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return GetById(obj.Id);
        }
        public virtual async Task<List<TEntity>> InsertManyAsync(List<TEntity> objs)
        {
            var now = DateTime.Now;
            string username = _userHelper.GetUserName();

            foreach (var e in objs)
            {
                if (string.IsNullOrEmpty(e.Id))
                    e.Id = Guid.NewGuid().ToString();

                e.CreateAt = now;
                e.CreateBy = username;
            }

            await _collection.InsertManyAsync(objs);
            return GetByIds(objs.Select(o => o.Id).ToArray())?.Data?.ToList();
        }

        public virtual List<TEntity> InsertMany(List<TEntity> objs)
        {
            var now = DateTime.Now;
            string username = _userHelper.GetUserName();

            foreach (var e in objs)
            {
                if (string.IsNullOrEmpty(e.Id))
                    e.Id = Guid.NewGuid().ToString();

                e.CreateAt = now;
                e.CreateBy = username;
            }

            _collection.InsertMany(objs);
            return GetByIds(objs.Select(o => o.Id).ToArray())?.Data?.ToList();
        }
        public virtual PaginationData<TEntity> Search(Expression<Func<TEntity, bool>> predicate, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        {
            SortDefinition<TEntity> sort = null;
            var filter = Builders<TEntity>.Filter.Where(predicate);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq(e => e.IsDeleted, false);
            var count = _collection.CountDocuments(filter & filterIsDeleted & UserOnlyFilter);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filter & filterIsDeleted & UserOnlyFilter) : _collection.Find(filter & filterIsDeleted & UserOnlyFilter).Sort(sort);
                return new PaginationData<TEntity>(resNoLimit.ToList(), page, limit, count);
            }

            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<TEntity>.Sort.Descending(sortField) : Builders<TEntity>.Sort.Ascending(sortField);
            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(predicate & filterIsDeleted & UserOnlyFilter).Skip((page - 1) * limit).Limit(limit) : _collection.Find(predicate & filterIsDeleted & UserOnlyFilter).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<TEntity>(res.ToList(), page, limit, count);
        }

        public virtual PaginationData<TEntity> SearchByFilterDefinition(FilterDefinition<TEntity> filterDefinition, int? page = null, int? limit = null, string sortField = null, bool sortDesc = false)
        {
            SortDefinition<TEntity> sort = null;
            var filter = filterDefinition;
            var filterIsDeleted = Builders<TEntity>.Filter.Eq(e => e.IsDeleted, false);
            var count = _collection.CountDocuments(filter & filterIsDeleted & UserOnlyFilter);

            if (!page.HasValue || !limit.HasValue)
            {
                var resNoLimit = string.IsNullOrEmpty(sortField) ? _collection.Find(filter & filterIsDeleted & UserOnlyFilter) : _collection.Find(filter & filterIsDeleted & UserOnlyFilter).Sort(sort);
                return new PaginationData<TEntity>(resNoLimit.ToList(), page, limit, count);
            }

            if (!string.IsNullOrEmpty(sortField))
                sort = sortDesc ? Builders<TEntity>.Sort.Descending(sortField) : Builders<TEntity>.Sort.Ascending(sortField);
            var res = string.IsNullOrEmpty(sortField) ? _collection.Find(filter & filterIsDeleted & UserOnlyFilter).Skip((page - 1) * limit).Limit(limit) : _collection.Find(filter & filterIsDeleted & UserOnlyFilter).Sort(sort).Skip((page - 1) * limit).Limit(limit);
            return new PaginationData<TEntity>(res.ToList(), page, limit, count);
        }

        public List<TEntity> SearchLikeInFieldAutoComplete(string field, string value, int limit = 10, Expression<Func<TEntity, bool>> predicate = null)
        {
            var fieldsSplit = field.Split(',');
            var filters = fieldsSplit.Select(f => Builders<TEntity>.Filter.Regex(f, new BsonRegularExpression($"/^.*{value}.*$/i"))).ToList();
            var filterIsDeleted = Builders<TEntity>.Filter.Eq(u => u.IsDeleted, false);
            var filterAnyEq = filters.Count > 1 ? Builders<TEntity>.Filter.Or(filters) : Builders<TEntity>.Filter.Regex(field, new BsonRegularExpression($"/^.*{value}.*$/i"));
            var filterWhere = predicate == null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Where(predicate);

            return _collection.Find(filterAnyEq & filterIsDeleted & UserOnlyFilter & filterWhere).Limit(limit).ToList();
        }

        //public virtual TEntity Update(string id, TEntity obj)
        //{
        //    var entity = GetById(id);
        //    if (entity == null) return null;

        //    obj.CreateAt = entity.CreateAt;
        //    obj.CreateBy = entity.CreateBy;
        //    obj.DeleteAt = entity.DeleteAt;
        //    obj.DeleteBy = entity.DeleteBy;
        //    obj.UpdateBy = _userHelper.GetUserName();
        //    obj.UpdateAt = DateTime.Now;

        //    var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);
        //    _collection.ReplaceOne<TEntity>(o => o.Id == id, obj);
        //    return GetById(id);
        //}

        public virtual TEntity Update(TEntity obj)
        {
            //if (isSchema) SchemaIndexSync(indexFields);

            var entity = GetById(obj.Id);
            if (entity == null) return null;
            obj.CreateAt = entity.CreateAt;
            obj.CreateBy = entity.CreateBy;
            obj.DeleteAt = entity.DeleteAt;
            obj.DeleteBy = entity.DeleteBy;
            obj.UpdateAt = DateTime.Now;
            //obj.UpdateBy = updateBy;
            //var filter = Builders<TEntity>.Filter.Eq(e => e.Id, obj.Id);
            _collection.ReplaceOne<TEntity>(o => o.Id == obj.Id, obj);
            return GetById(obj.Id);
        }
        public List<TEntity> UpdateManyEntity(List<TEntity> objs)
        {
            var filterIds = Builders<TEntity>.Filter.In(e => e.Id, objs.Select(x => x.Id));

            foreach (var obj in objs)
            {
                _collection.ReplaceOne(o => o.Id == obj.Id, obj);
            }
            return GetByIds(objs.Select(o => o.Id).ToArray())?.Data?.ToList();
        }
        public TEntity SearchFirstByField(string field, string value, bool isDeleted = false)
        {
            var eqIsDeleted = Builders<TEntity>.Filter.Eq(e => e.IsDeleted, isDeleted);
            var eqFilter = Builders<TEntity>.Filter.Eq(field, value);
            return _collection.Find(eqFilter & eqIsDeleted & UserOnlyFilter).FirstOrDefault();
        }

        public TEntity GetByField(string field, string value)
        {
            var filterField = Builders<TEntity>.Filter.Eq(field, value);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            //  return _collection.Find(filterField & filterIsDeleted).FirstOrDefault();
            return _collection.Find(filterField & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }
        public TEntity GetByFieldInt(string field, int value)
        {
            var filterField = Builders<TEntity>.Filter.Eq(field, value);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            //  return _collection.Find(filterField & filterIsDeleted).FirstOrDefault();
            return _collection.Find(filterField & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }
        public List<TEntity> GetListByField(string field, string value)
        {
            var filterField = Builders<TEntity>.Filter.Eq(field, value);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            //  return _collection.Find(filterField & filterIsDeleted).FirstOrDefault();
            return _collection.Find(filterField & filterIsDeleted & UserOnlyFilter).ToList();
        }


        public List<TEntity> GetUsersByField()
        {
            IMongoCollection<TEntity> coll;
            coll = _context.GetDatabaseByTenanty("identity").GetCollection<TEntity>("user");
            var filterField = Builders<TEntity>.Filter.Eq("userName", "admin@admin.com");
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            return coll.Find(filterField & filterIsDeleted & UserOnlyFilter).ToList();
        }

        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            var filterPredicate = Builders<TEntity>.Filter.Where(predicate);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            return _collection.Find(filterPredicate & filterIsDeleted & UserOnlyFilter).CountDocuments() > 0;
        }

        public TEntity GetByFieldExternal(string field, string value, string tenanty)
        {
            IMongoCollection<TEntity> coll;
            //  coll = _context.GetDatabase().GetCollection<TEntity>("views");

            coll = _context.GetDatabaseByTenanty(tenanty).GetCollection<TEntity>("views");
            var filterField = Builders<TEntity>.Filter.Eq(field, value);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            return coll.Find(filterField & filterIsDeleted).FirstOrDefault();
            // return _collection.Find(filterField & filterIsDeleted & UserOnlyFilter).FirstOrDefault();
        }
        public TEntity GetByIdExternal(string id, string tenanty)
        {
            IMongoCollection<TEntity> coll;
            coll = _context.GetDatabaseByTenanty(tenanty).GetCollection<TEntity>("views");
            var filterId = Builders<TEntity>.Filter.Eq(e => e.Id, id);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            return coll.Find(filterId & filterIsDeleted).FirstOrDefault();
        }

        public TEntity GetSchemaByField(string field, string value)
        {
            IMongoCollection<TEntity> coll;
            ClaimsPrincipal claimsPrincipal = (ClaimsPrincipal)(_accessor?.HttpContext.Items["currentClaims"]);
            var tenanty = claimsPrincipal.Claims?.FirstOrDefault(c => c.Type == "tenanty")?.Value;
            coll = _context.GetDatabaseByTenanty(tenanty).GetCollection<TEntity>("schema");
            var filterField = Builders<TEntity>.Filter.Eq(field, value);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            return coll.Find(filterField & filterIsDeleted).FirstOrDefault();
        }

        public void DeleteAll()
        {
            var filter = Builders<TEntity>.Filter.Empty;

            // Delete all documents in the collection
           _collection.DeleteMany(filter);


        }

        public List<TEntity> GetSchemasByFieldContains(string field, string value)
        {
            IMongoCollection<TEntity> coll;
            ClaimsPrincipal claimsPrincipal = (ClaimsPrincipal)(_accessor?.HttpContext.Items["currentClaims"]);
            var tenanty = claimsPrincipal.Claims?.FirstOrDefault(c => c.Type == "tenanty")?.Value;
            coll = _context.GetDatabaseByTenanty(tenanty).GetCollection<TEntity>("schema");
            var filterField = Builders<TEntity>.Filter.Regex(field, new BsonRegularExpression($"/^.*{value}.*$/i"));
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            return coll.Find(filterField & filterIsDeleted).ToList();
        }

        public List<TEntity> GetAllActionsByIds(List<string> ids)
        {
            IMongoCollection<TEntity> coll;
            ClaimsPrincipal claimsPrincipal = (ClaimsPrincipal)(_accessor?.HttpContext.Items["currentClaims"]);
            var tenanty = claimsPrincipal.Claims?.FirstOrDefault(c => c.Type == "tenanty")?.Value;
            coll = _context.GetDatabaseByTenanty(tenanty).GetCollection<TEntity>("action");
            var filterId = Builders<TEntity>.Filter.In(e => e.Id, ids);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            return coll.Find(filterId & filterIsDeleted).ToList();
        }

        public List<TEntity> GetAllActionsByModules(List<string> moduleNames)
        {
            IMongoCollection<TEntity> coll;
            ClaimsPrincipal claimsPrincipal = (ClaimsPrincipal)(_accessor?.HttpContext.Items["currentClaims"]);
            var tenanty = claimsPrincipal.Claims?.FirstOrDefault(c => c.Type == "tenanty")?.Value;
            coll = _context.GetDatabaseByTenanty(tenanty).GetCollection<TEntity>("action");
            var filterId = Builders<TEntity>.Filter.In("Module", moduleNames);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            return coll.Find(filterId & filterIsDeleted).ToList();
        }

        public TEntity GetByFieldCompany(string field, string value, string companySiteId)
        {
            var filterField = Builders<TEntity>.Filter.Eq(field, value);

            var filterCompany = Builders<TEntity>.Filter.Eq("companySiteId", companySiteId);

            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);

            //  return _collection.Find(filterField & filterIsDeleted).FirstOrDefault();

            return _collection.Find(filterField & filterIsDeleted & UserOnlyFilter & filterCompany).FirstOrDefault();
        }

        public List<TEntity> GetSchemasByNames(string field, List<string> names)
        {
            IMongoCollection<TEntity> coll;
            ClaimsPrincipal claimsPrincipal = (ClaimsPrincipal)(_accessor?.HttpContext.Items["currentClaims"]);
            var tenanty = claimsPrincipal.Claims?.FirstOrDefault(c => c.Type == "tenanty")?.Value;
            coll = _context.GetDatabaseByTenanty(tenanty).GetCollection<TEntity>("schema");
            var filterField = Builders<TEntity>.Filter.In(field, names);
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            return coll.Find(filterField & filterIsDeleted).ToList();
        }

        public List<TEntity> GetSchemasByModuleRegex(string moduleName)
        {
            IMongoCollection<TEntity> coll;
            ClaimsPrincipal claimsPrincipal = (ClaimsPrincipal)(_accessor?.HttpContext.Items["currentClaims"]);
            var tenanty = claimsPrincipal.Claims?.FirstOrDefault(c => c.Type == "tenanty")?.Value;
            coll = _context.GetDatabaseByTenanty(tenanty).GetCollection<TEntity>("schema");
            var filterField = Builders<TEntity>.Filter.Regex("jsonValue", new BsonRegularExpression($@".*\""module\""\s*:\s*\""{moduleName}\"".*", "i"));
            var filterIsDeleted = Builders<TEntity>.Filter.Eq("IsDeleted", false);
            return coll.Find(filterField & filterIsDeleted).ToList();
        }
    }
}