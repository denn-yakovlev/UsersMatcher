﻿using Microsoft.Extensions.Caching.Memory;
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

        public int TotalFriendsCount { get; set; }

        public int NonEmptyFriendsCount => SimilarityResult.Count();

        public IEnumerable<KeyValuePair<User, double>> SimilarityResult { get; set; }

        public TimeSpan Time { get; set; }

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
