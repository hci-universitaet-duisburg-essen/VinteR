using System.Collections.Generic;
using VinteR.Model;
using VinteR.Mongo;

namespace VinteR.Input
{
    public class MongoDbClient : IQueryService
    {

        public IList<Session> GetSessions()
        {
            throw new System.NotImplementedException();
        }

        public Session GetSession(string name, int startTimestamp = 0, int endTimestamp = -1)
        {
            throw new System.NotImplementedException();
        }

        public MongoDbClient(IVinterMongoDBClient client)
        {
            this.client = client;
        }
    }
}