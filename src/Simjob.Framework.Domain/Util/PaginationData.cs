using System;
using System.Collections.Generic;

namespace Simjob.Framework.Domain.Util
{
    public class PaginationData<T>
    {
        public PaginationData(IEnumerable<T> data, int? page, int? limit, long? total)
        {
            Data = data;
            Page = page ?? 1;
            Limit = limit ?? 30;
            Total = total ?? 1;
            if (total > 1)
            {
                var pages = Math.Ceiling((decimal.Parse(total.ToString()) / decimal.Parse(Limit.ToString())));
                Pages = int.Parse(pages.ToString());
            }
            else
            {
                Page = page ?? 1;
            }

        }

        public IEnumerable<T> Data { get; set; }
        public long Total { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; }
        public int Pages { get; set; } = 1;
    }

}

