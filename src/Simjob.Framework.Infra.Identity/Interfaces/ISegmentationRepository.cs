using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface ISegmentationRepository
    {
        void Insert(Segmentation obj);
        void Update(string id, Segmentation obj);
        Segmentation GetById(string id);
        void Delete(string id);
        bool Exists(Expression<Func<Segmentation, bool>> predicate);

    }
}
