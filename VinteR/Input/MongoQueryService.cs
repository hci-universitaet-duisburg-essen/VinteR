using MongoDB.Driver;
using System.Collections.Generic;
using VinteR.Configuration;
using VinteR.Model;
using VinteR.Mongo;
using MongoDB.Bson;

namespace VinteR.Input
{
    public class MongoQueryService : IQueryService
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private IMongoClient client;
        private IMongoDatabase database;
        private IMongoCollection<Session> sessionCollection;

        public IList<Session> GetSessions()
        {
            // return the Session from the database
            try
            {
                return this.sessionCollection.Find(_ => true).ToList();
            } catch (System.Exception e)
            {
                Logger.Error("GetSessions failed on retriving data due: {0}", e.ToString());
                throw new System.Exception("Database Failure");
            }
            
        }

        public Session GetSession(string name, int startTimestamp = 0, int endTimestamp = -1)
        {
            var collectionNameFrames = string.Format("Vinter-{0}-Frames", name);
            var collectionNameBodies = string.Format("Vinter-{0}-Bodies", name);
            var framesCollection = database.GetCollection<MocapFrame>(collectionNameFrames);
            var bodyCollection = database.GetCollection<Body>(collectionNameBodies);
            
            if (startTimestamp != 0 && endTimestamp != -1)
            {
                // return Slice
                var gtFilter = Builders<MocapFrame>.Filter.Gt("ElapsedMillis", startTimestamp);
                var ltFilter = Builders<MocapFrame>.Filter.Lt("ElapsedMillis", endTimestamp);
                var filter = Builders<MocapFrame>.Filter.And(gtFilter, ltFilter);
                var frames = framesCollection.Find<MocapFrame>(filter).ToList();

                // We might need a more performant solution in the future
                foreach(MocapFrame mocap in frames)
                {
                    var bodyFilter = Builders<Body>.Filter.In(x => x._id, mocap._referenceBodies);
                    var bodies = bodyCollection.Find<Body>(bodyFilter).ToList();
                    mocap.Bodies = bodies;
                }

                // build the Sesssion
                var session = this.sessionCollection.Find((x => x.Name == name)).Single();
                session.MocapFrames = frames;
                return session;

            } else if (startTimestamp == 0 && endTimestamp != -1)
            {
                // slice from DocumentStart to endTimeStamp

            } else if (startTimestamp != 0 && endTimestamp == -1)
            {
                // slice from startTimestamp to DocumentEnd
            } else
            {
                // return everything

            }
        }

        public MongoQueryService(IConfigurationService configurationService, IVinterMongoDBClient client)
        {
            if (configurationService.GetConfiguration().Mongo.Enabled)
            {
                client.connect();
                this.client = client.getMongoClient();

                // Setup Database
                this.database = this.client.GetDatabase(configurationService.GetConfiguration().Mongo.Database);
                this.sessionCollection = this.database.GetCollection<Session>("Sessions");
                Logger.Debug("MongoQuery Service initialized");
            } else
            {
                var exception =  new System.ApplicationException("MongoDB not enabled, but MongoQueryService requested");
                Logger.Error(exception);
                throw exception;
            }
        }
    }
}