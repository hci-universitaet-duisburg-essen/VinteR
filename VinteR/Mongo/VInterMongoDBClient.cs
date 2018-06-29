using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using NLog;
using VinteR.Configuration;

namespace VinteR.Mongo
{
    class VinterMongoDBClient : IVinterMongoDBClient
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IConfigurationService _configurationService;
        private IMongoClient client;

        public IMongoClient getMongoClient()
        {
            return this.client;
        }

        public VinterMongoDBClient(IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
        }

        // Build Connection URL
        // mongodb://<dbuser>:<dbpassword>@<domain>:<port>/<database>
        private MongoUrl buildMongoUrl()
        {
            var mongoConfig = _configurationService.GetConfiguration().Mongo;
            var url = string.Format("mongodb://{0}:{1}@{2}:{3}/{4}",
                mongoConfig.User, // <dbuser>
                mongoConfig.Password, // <dbpassword>
                mongoConfig.Domain, // <domain>
                mongoConfig.Port, // <port>
                mongoConfig.Database); // <database>
            return new MongoUrl(url);
        }

        public void connect()
        {
            if (_configurationService.GetConfiguration().Mongo.Enabled && this.client == null)
            {
                try
                {
                    // Create a Mongo Client from Configuration
                    var mongoUrl = buildMongoUrl();
                    this.client = new MongoClient(mongoUrl);
                }
                catch (Exception e)
                {
                    Logger.Error("Connection to the MongoDB Client failed because of: {0}", e.ToString());
                    throw new ApplicationException("MongoDB client can't establish a connection");
                }
            } else if(this.client != null)
            {
                // If connect() was already called, beforehand
                // -> do nothing
            } else
            {
                throw new ApplicationException("MongoDB is disabled or the client has problems");
            }
            
        }
    }
}
