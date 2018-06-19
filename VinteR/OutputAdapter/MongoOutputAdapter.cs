using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinteR.Configuration;
using VinteR.Model;

namespace VinteR.OutputAdapter
{
    class MongoOutputAdapter : IOutputAdapter
    {

        private readonly IConfigurationService _configurationService;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private MongoClient client;
        private IMongoDatabase database;
        private IMongoCollection<MocapFrame> frameCollection;

        public MongoOutputAdapter(IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
            this.client = null;
            this.database = null;
            this.frameCollection = null;
        }

        public void OnDataReceived(MocapFrame mocapFrame)
        {
            if (this._configurationService.GetConfiguration().Mongo.Enabled)
            {
                Logger.Debug("Data Received for MongoDB");
                if (this.client != null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        this.frameCollection.InsertOneAsync(mocapFrame);
                        Logger.Debug("Frame Inserted");
                    });
                }
            }
            
        }

        // Build Connection URL
        // mongodb://<dbuser>:<dbpassword>@<domain>:<port>/<database>
        private MongoUrl buildMongoUrl()
        {
            var url =
                "mongodb://"
                + this._configurationService.GetConfiguration().Mongo.User // <dbuser>
                + ":"
                + this._configurationService.GetConfiguration().Mongo.Password // <dbpassword>
                + "@"
                + this._configurationService.GetConfiguration().Mongo.Domain // <domain>
                + ":"
                + this._configurationService.GetConfiguration().Mongo.Port // <port>
                + "/"
                + this._configurationService.GetConfiguration().Mongo.Database; // <database>
            return new MongoUrl(url);
        }

        public void Start()
        {
            if (this._configurationService.GetConfiguration().Mongo.Enabled)
            {
                Logger.Info("MongoDB Output Enabled");

                try
                {
                    var mongoUrl = buildMongoUrl();
                    this.client = new MongoClient(mongoUrl);

                    // Setup Database
                    this.database = this.client.GetDatabase("vinter");
                    this.frameCollection = this.database.GetCollection<MocapFrame>("VinterMergedData");

                    Logger.Debug("Client set");
                }
                catch
                {
                    Logger.Error("Connection to MongoDB Database failed");
                    throw new ApplicationException("Connection to MongoDB failed!");
                }
            } else
            {
                Logger.Info("MongoDB Output Disabled!");
            }
            
        }

        public void Stop()
        {
            // In the currently MongoDB Driver there is no need to close and dispose connections the client shopuld do it automatically

        }
    }
}
