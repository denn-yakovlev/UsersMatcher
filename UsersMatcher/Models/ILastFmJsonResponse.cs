using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMatcher.Models
{
    interface ILastFmJsonResponse<T>
    {
        List<T> Content { get; set; }

        Attributes Attributes { get; set; }
    }
}
