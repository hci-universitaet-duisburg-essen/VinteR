using MongoDB.Driver;
using System.Collections.Generic;
using VinteR.Model;
using VinteR.Mongo;

namespace VinteR.Input
{
    public class MongoQueryService : IQueryService
    {
        private IMongoClient client;

        public IList<Session> GetSessions()
        {
            throw new System.NotImplementedException();
        }

        public Session GetSession(string name, int startTimestamp = 0, int endTimestamp = -1)
        {
            throw new System.NotImplementedException();
        }

        public MongoQueryService(IVinterMongoDBClient client)
        {
            this.client = client.getMongoClient();
        }
    }
}