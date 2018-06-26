using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using VinteR.Configuration;

namespace VinteR.Mongo
{
    class VInterMongoDBClient : IVinterMongoDBClient
    {
        private readonly IConfigurationService _configurationService;

        public IMongoClient getMongoClient()
        {
            return this.client;
        }

        public VInterMongoDBClient(IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
            this.client = new MongoClient(mongoUrl);
        }
    }
}
