using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersMatcher.Models;

namespace UsersMatcher.Logic
{
    public static class UsersMatcherMain
    {
        public static string TargetUserName { get; set; }

        private static Task<IEnumerable<Album>> AlbumsOfTarget => Client.GetTopNAlbumsAsync(TargetUserName, 100);

        private static Task<IEnumerable<User>> Friends => Client.GetAllFriendsAsync(TargetUserName);

        public static Func<IEnumerable<Album>, IEnumerable<Album>, double> Metric { get; set; }

        private static LastFmClient Client => new LastFmClient(apiKey: "48cdcb584772ebb3935f8f6289dc9d0b");

        public static async Task<UsersMatchResult> GetSimilarity()
        {
            var friends = await Friends;
            var batchSize = 10;
            var batchesCount = friends.Count() / batchSize + 1;

            IEnumerable<KeyValuePair<User, double>> similarityResult = new Dictionary<User, double>();
            for (int b = 0; b < batchesCount; b++)
            {
                var batch = friends.Skip(b * batchSize).Take(batchSize);
                similarityResult = similarityResult.Concat(await ProcessOneBatch(batch));
            }

            // remove redundant KeyValuePairs produced by GetSimilarityByAlbumsAsync()
            return new UsersMatchResult
            {
                UserName = TargetUserName,
                SimilarityResult = similarityResult.Where(pair => pair.Key != null)
            };               
        }

        private static async Task<IEnumerable<KeyValuePair<User, double>>> ProcessOneBatch(IEnumerable<User> batch)
        {
            var tasks = batch.Select(friend => GetSimilarityByAlbumsAsync(friend));
            return await Task.WhenAll(tasks);
        }

        // similarity with one friend
        private static async Task<KeyValuePair<User, double>> GetSimilarityByAlbumsAsync(User friend)
        {
            try
            {
                var albumsOfFriend = await Client.GetTopNAlbumsAsync(friend.Name, 100);
                return new KeyValuePair<User, double>(friend, Metric(await AlbumsOfTarget, albumsOfFriend));
            }
            catch(LastFmApiError err)
            {
                if (err.StatusCode == 404)
                    return new KeyValuePair<User, double>();
                else throw;
            }
        }
    }
}
