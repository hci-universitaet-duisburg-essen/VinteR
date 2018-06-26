using System.Collections.Generic;
using VinteR.Model;

namespace VinteR.Input
{
    public class JsonStorage : IQueryService
    {
        public IList<Session> GetSessions()
        {
            throw new System.NotImplementedException();
        }

        public Session GetSession(string name, int startTimestamp = 0, int endTimestamp = -1)
        {
            throw new System.NotImplementedException();
        }
    }
}