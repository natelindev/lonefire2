using System;

namespace lonefire.Models
{
    interface IEntityDate
    {
        DateTimeOffset CreateTime { set; get; }
        DateTimeOffset EditTime { set; get; }
    }
}
