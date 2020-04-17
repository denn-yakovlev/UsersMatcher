using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMatcher.Logic
{
    public static class SimilarityMetrices
    {
        public static double Tanimoto<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            var intersection = first.Intersect(second);
            var intersectionSize = intersection.Count();
            var union = first.Union(second);
            return (double)intersectionSize / (union.Count() - intersectionSize);
        }
    }
}
