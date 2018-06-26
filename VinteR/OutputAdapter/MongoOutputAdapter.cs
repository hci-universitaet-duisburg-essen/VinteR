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
        private IMongoCollection<Session> documentCollection;
        private Session _session;
        private bool Enabled;
        private bool Write; 

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
        }

        public void OnDataReceived(MocapFrame mocapFrame)
        {
            if (this.Enabled && this.Write)
            {
                Logger.Debug("Data Received for MongoDB");

                // buffer frames before init
                if ( (this.frameCollection == null) || (this.bodyCollection == null) )
                {
                    if (this._buffer.Count <= this._bufferSize )
                    {
                        this._buffer.Add(mocapFrame);
                    }
                    else
                    {
                        this._buffer.Clear();
                    }
                }

                if ((this.frameCollection != null) && (this.bodyCollection != null))
                {
                    // empty the buffer
                    if (this._buffer.Count > 0)
                    {
                        foreach (MocapFrame frame in this._buffer)
                        {
                            writeToDatabase(frame);
                        }
                    }
                }

                // write the current Frame
                writeToDatabase(mocapFrame);
            }
        }

        public void writeToDatabase(MocapFrame mocapFrame)
        {
            if ((this.frameCollection != null) && (this.bodyCollection != null))
            {
                mocapFrame._id = new BsonObjectId(ObjectId.GenerateNewId());
                foreach (Body body in mocapFrame.Bodies)
                {
                    body._id = new BsonObjectId(ObjectId.GenerateNewId());
                    mocapFrame._referenceBodies.Add(body._id);
                }

                Task.Factory.StartNew(() =>
                {
                    this.bodyCollection.InsertMany(mocapFrame.Bodies);
                    this.frameCollection.InsertOne(mocapFrame);
                    Logger.Debug("Frame Inserted");
                });
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
                    //var frameCollectionForSession = string.Format("Vinter-{0}-Frames", this._session.Name);
                    //var bodyCollectionForSession = string.Format("Vinter-{0}-Bodies", this._session.Name);

                    // Setup Database
                    this.database = this.client.GetDatabase(this._configurationService.GetConfiguration().Mongo.Database);
                    this.frameCollection = this.database.GetCollection<MocapFrame>("frames");
                    this.bodyCollection = this.database.GetCollection<Body>("bodies");
                    this.documentCollection = this.database.GetCollection<Session>("Sessions");
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
                //this.documentCollection.InsertOne(this._session);
            } catch (Exception e)
            {
               // Logger.Error("Could not serialize session in database due to: {0}", e.ToString());
            }
            
        }
    }
}
