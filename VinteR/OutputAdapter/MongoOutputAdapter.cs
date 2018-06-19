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
            this.frameCollection.InsertOne(mocapFrame);
        }

        public void Start()
        {
            // TBD Get data from Configuration, currently use hardcoded values

            // Setup the client and its connection
            try {
                var mongoUrl = new MongoUrl("mongodb://dbvinter:dbvinter18@ds161710.mlab.com:61710/vinter");
                this.client = new MongoClient(mongoUrl);

                // Setup Database
                this.database = this.client.GetDatabase("vinter");
                this.frameCollection = this.database.GetCollection<MocapFrame>("merged_data");
            } catch
            {
                // In case something wents wrong with the database initialization
                this.client = null;
                Logger.Error("Connection to MongoDB Database failed");
                throw new ApplicationException("Connection with MongoDB failed");
            }
            
        }

        public void Stop()
        {
            // In the currently MongoDB Driver there is no need to close and dispose connections the client shopuld do it automatically

        }
    }
}
