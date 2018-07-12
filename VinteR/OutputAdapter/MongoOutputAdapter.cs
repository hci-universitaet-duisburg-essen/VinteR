using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinteR.Configuration;
using VinteR.Model;
using VinteR.Mongo;
using Ninject;

namespace VinteR.OutputAdapter
{
    class MongoOutputAdapter : IOutputAdapter
    {

        private readonly IConfigurationService _configurationService;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly int _bufferSize;
        private IList _buffer;
        private IMongoClient client;
        private IVinterMongoDBClient dbClient;
        private IMongoDatabase database;
        private IMongoCollection<MocapFrame> frameCollection;
        private IMongoCollection<Body> bodyCollection;
        private IMongoCollection<Session> sessionCollection;
        private Session _session;
        private bool Enabled;
        private bool Write;
        private InsertManyOptions _insertOptions;

        public MongoOutputAdapter(IConfigurationService configurationService, IVinterMongoDBClient dbClient)
        {
            this._configurationService = configurationService;
            this.dbClient = dbClient;
            this.database = null;
            this.frameCollection = null;
            this.bodyCollection = null;
            this._buffer = new List<MocapFrame>();
            this._bufferSize = this._configurationService.GetConfiguration().Mongo.MongoBufferSize;
            this.Enabled = this._configurationService.GetConfiguration().Mongo.Enabled;
            this.Write = this._configurationService.GetConfiguration().Mongo.Write;
            this._insertOptions = new InsertManyOptions();
        }

        public void OnDataReceived(MocapFrame mocapFrame)
        {
            if (this.Enabled && this.Write)
            {
                Logger.Debug("Data Received for MongoDB");

                // Buffer Frames
                    if (this._buffer.Count <= this._bufferSize )
                    {
                        this._buffer.Add(mocapFrame);
                        Logger.Debug("MongoDB Frame appended to List");
                    }
                    else
                    {
                        writeToDatabase();
                    }

            }
        }


        public void writeToDatabase()
        {
            if ((this.frameCollection != null))
            {
                this.frameCollection.InsertManyAsync( (IEnumerable<MocapFrame>) this._buffer );
                Logger.Debug("Bulk Insert started, executed asynchroniously");
                this._buffer.Clear();
            }
        }

        public void flush()
        {
            if ((this.frameCollection != null))
            {
                this.frameCollection.InsertMany((IEnumerable<MocapFrame>)this._buffer);
                Logger.Debug("Bulk Insert flush, executed synchroniously");
                this._buffer.Clear();
            }
        }

        public void Start(Session session)
        {
            if (this.Enabled && this.Write)
            {
                Logger.Info("MongoDB Output Enabled");
                // Set the Session
                this._session = session;
                try
                {
                    this.dbClient.connect();
                    this.client = this.dbClient.getMongoClient();
                    var frameCollectionForSession = string.Format("Vinter-{0}-Frames", this._session.Name);

                    // Setup Database
                    this.database = this.client.GetDatabase(this._configurationService.GetConfiguration().Mongo.Database);
                    this.frameCollection = this.database.GetCollection<MocapFrame>(frameCollectionForSession);
                    this.sessionCollection = this.database.GetCollection<Session>("Sessions");
                    this._insertOptions.IsOrdered = false; // Improves performance, order of documents does not matter on bulk insert!

                    Logger.Debug("MongoDB Client initialized");
                }
                catch (Exception e)
                {
                    Logger.Error("Connection to MongoDB Database failed");
                    Logger.Error(e);
                    throw new ApplicationException("Connection to MongoDB Database failed!");
                }
            } else
            {
                Logger.Info("MongoDB Output Disabled!");
            }
            
        }

        public void Stop()
        {
            try
            {
                // Serialize Session Meta in the database
                this.sessionCollection?.InsertOne(this._session);
                flush();
            } catch (Exception e)
            {
                Logger.Error("Could not serialize session in database due to: {0}", e.ToString());
            }
            
        }
    }
}
