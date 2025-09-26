using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IUserRepository
    {
        PaginationData<User> GetAll(int? page = null, int? limiUser = null, string sortField = null, bool sortDesc = false, bool addIsDeleted = false);
        PaginationData<User> SearchRegexByFields(string fields, string search, int? page = null, int? limiUser = null, string sortField = null, bool sortDesc = false);
        PaginationData<User> Search(Expression<Func<User, bool>> predicate, int? page = null, int? limiUser = null, string sortField = null, bool sortDesc = false);
        User SearchFirstByField(string field, string value, bool isDeleted = false);
        User GetByField(string field, string value);
        List<User> SearchLikeInFieldAutoComplete(string field, string value, int limiUser = 10, Expression<Func<User, bool>> predicate = null);
        List<User> GetById(string[] ids);
        User GetById(string id);
        void Delete(string id);
        void Insert(User obj);
        void InsertMany(List<User> objs);
        void Update(string id, User obj);

        bool Exists(Expression<Func<User, bool>> predicate);
    }
}
