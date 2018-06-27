﻿using System.Collections.Generic;
using VinteR.Model;

namespace VinteR.Input
{
    public class JsonStorage : IQueryService
    {
        private const string StorageName = "JsonSessionFiles";

        public string GetStorageName()
        {
            return StorageName;
        }

        public IList<Session> GetSessions()
        {
            return new List<Session>();
        }

        public Session GetSession(string name, int startTimestamp = 0, int endTimestamp = -1)
        {
            throw new System.NotImplementedException();
        }
    }
}