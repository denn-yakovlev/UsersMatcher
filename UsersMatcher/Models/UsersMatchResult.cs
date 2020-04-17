using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMatcher.Models
{
    public class UsersMatchResult: IEnumerable<KeyValuePair<User, double>>
    {
        public string UserName { get; set; }

        public int ResultSize => SimilarityResult.Count();

        public IEnumerable<KeyValuePair<User, double>> SimilarityResult { get; set; }

        public IEnumerator<KeyValuePair<User, double>> GetEnumerator()
        {
            return SimilarityResult.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return SimilarityResult.GetEnumerator();
        }
    }
}
