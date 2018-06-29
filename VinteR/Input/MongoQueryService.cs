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
                IList<Session> sessions = this.sessionCollection.Find(_ => true).ToList();
                return sessions;
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
                return getSlice(startTimestamp, endTimestamp, framesCollection, bodyCollection, name);

            } else if (startTimestamp == 0 && endTimestamp != -1)
            {
                // slice from DocumentStart to endTimeStamp
                return getDocumentStartTilEnd(endTimestamp, framesCollection, bodyCollection, name);

            } else if (startTimestamp != 0 && endTimestamp == -1)
            {
                // slice from startTimestamp to DocumentEnd
                return getStartTilDataEnd(startTimestamp, framesCollection, bodyCollection, name);

            } else
            {
                // return everything
                return getFull(framesCollection, bodyCollection, name);
            }
        }

        public Session getSlice(int startTimestamp, int endTimestamp, IMongoCollection<MocapFrame> framesCollection, IMongoCollection<Body> bodyCollection, string sessionName)
        {
            var gtFilter = Builders<MocapFrame>.Filter.Gt("ElapsedMillis", startTimestamp);
            var ltFilter = Builders<MocapFrame>.Filter.Lt("ElapsedMillis", endTimestamp);
            var filter = Builders<MocapFrame>.Filter.And(gtFilter, ltFilter);
            var frames = framesCollection.Find<MocapFrame>(filter).ToList();

            return mergeBuild(frames, bodyCollection, sessionName);
          
        }

        public Session getFull(IMongoCollection<MocapFrame> framesCollection, IMongoCollection<Body> bodyCollection, string sessionName)
        {
            var frames = framesCollection.Find<MocapFrame>(_ => true).ToList();
            return mergeBuild(frames, bodyCollection, sessionName);
        }


        public Session getStartTilDataEnd(int startTimestamp, IMongoCollection<MocapFrame> framesCollection, IMongoCollection<Body> bodyCollection, string sessionName)
        {
            var gtFilter = Builders<MocapFrame>.Filter.Gt("ElapsedMillis", startTimestamp);
            var frames = framesCollection.Find<MocapFrame>(gtFilter).ToList();

            return mergeBuild(frames, bodyCollection, sessionName);

        }

        public Session getDocumentStartTilEnd(int endTimestamp, IMongoCollection<MocapFrame> framesCollection, IMongoCollection<Body> bodyCollection, string sessionName)
        {
            var ltFilter = Builders<MocapFrame>.Filter.Lt("ElapsedMillis", endTimestamp);
            var frames = framesCollection.Find<MocapFrame>(ltFilter).ToList();

            return mergeBuild(frames, bodyCollection, sessionName);
        }

        public Session mergeBuild(IList<MocapFrame> frames, IMongoCollection<Body> bodyCollection, string sessionName)
        {
            foreach (MocapFrame mocap in frames)
            {
                mergeFrameBody(mocap, bodyCollection);
            }

            // build the Sesssion
            var session = this.sessionCollection.Find((x => x.Name == sessionName)).Single(); // Name is unique, by definition otherwise we have a problem ...
            session.MocapFrames = frames;

            return session;
        }

        public MocapFrame mergeFrameBody(MocapFrame mocap, IMongoCollection<Body> bodyCollection )
        {
            var bodyFilter = Builders<Body>.Filter.In(x => x._id, mocap._referenceBodies);
            var bodies = bodyCollection.Find<Body>(bodyFilter).ToList();
            mocap.Bodies = bodies;

            return mocap;
        }

        public string GetStorageName()
        {
            return "mongo";
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