using System.Collections.Generic;
using VinteR.Model;

namespace VinteR.Input
{
    public class MongoDbClient : IQueryService
    {
        private const string StorageName = "MongoDB";

        public string GetStorageName()
        {
            return StorageName;
        }

        public IList<Session> GetSessions()
        {
            return new List<Session>();
        }

        public Session GetSession(string name, uint startTimestamp = 0, int endTimestamp = -1)
        {
            throw new System.NotImplementedException();
        }
    }
}