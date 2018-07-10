using MongoDB.Driver;
using System.Collections.Generic;
using VinteR.Configuration;
using VinteR.Model;
using VinteR.Mongo;
using MongoDB.Bson;
using System;

namespace VinteR.Input
{
    public class MongoQueryService : IQueryService
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private IMongoClient client;
        private IMongoDatabase database;
        private IMongoCollection<Session> sessionCollection;
        private bool _enabled;

        public MongoQueryService(IConfigurationService configurationService, IVinterMongoDBClient client)
        {
            _enabled = configurationService.GetConfiguration().Mongo.Enabled;
            if (_enabled)
            {
                client.connect();
                this.client = client.getMongoClient();

                // Setup Database
                this.database = this.client.GetDatabase(configurationService.GetConfiguration().Mongo.Database);
                this.sessionCollection = this.database.GetCollection<Session>("Sessions");
                Logger.Debug("MongoQuery Service initialized");
            }
        }

        public IList<Session> GetSessions()
        {
            if (!_enabled)
            {
                Logger.Warn("MongoDB not enabled");
                return new List<Session>();
            }

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

        public Session GetSession(string name, uint startTimestamp = 0, int endTimestamp = -1)
        {
            if (!_enabled)
            {
                Logger.Warn("MongoDB not enabled");
                return null;
            }

            var collectionNameFrames = string.Format("Vinter-{0}-Frames", name);
            var framesCollection = database.GetCollection<MocapFrame>(collectionNameFrames);
            
            if (startTimestamp != 0 && endTimestamp != -1)
            {
                // return Slice
                return getSlice(startTimestamp, endTimestamp, framesCollection, name);

            } else if (startTimestamp == 0 && endTimestamp != -1)
            {
                // slice from DocumentStart to endTimeStamp
                return getDocumentStartTilEnd(endTimestamp, framesCollection, name);

            } else if (startTimestamp != 0 && endTimestamp == -1)
            {
                // slice from startTimestamp to DocumentEnd
                return getStartTilDataEnd(startTimestamp, framesCollection, name);

            } else
            {
                // return everything
                return getFull(framesCollection, name);
            }
        }

        private Session buildSession(IList<MocapFrame> frames, string sessionName)
        {
            // build the Sesssion
            var session = this.sessionCollection.Find((x => x.Name == sessionName)).Single(); // Name is unique, by definition otherwise we have a problem ...
            session.MocapFrames = frames;
            return session;
        }

        private Session getSlice(uint startTimestamp, int endTimestamp, IMongoCollection<MocapFrame> framesCollection, string sessionName)
        {
            var gtFilter = Builders<MocapFrame>.Filter.Gt("ElapsedMillis", startTimestamp);
            var ltFilter = Builders<MocapFrame>.Filter.Lt("ElapsedMillis", endTimestamp);
            var filter = Builders<MocapFrame>.Filter.And(gtFilter, ltFilter);
            List<MocapFrame> frames = null; 

            try
            {
                frames = framesCollection
                                       .Find<MocapFrame>(filter)
                                       .SortBy(e => e.ElapsedMillis)
                                       .ToList();

            } // Sometimes the Sort takes to much RAM!
            catch (MongoDB.Driver.MongoCommandException e)
            {
                Logger.Warn("Mongo command failed due to exception {0}", e.ToString());
                frames = framesCollection
                                    .Find<MocapFrame>(filter)
                                    .ToList();
            }
            catch (Exception e)
            {
                Logger.Error("Severe Error: {0}", e.ToString());
                frames = null;
            }

            return buildSession(frames, sessionName);
          
        }

        private Session getFull(IMongoCollection<MocapFrame> framesCollection, string sessionName)
        {
            List<MocapFrame> frames = null;
            try
            {
                frames = framesCollection
                                .Find<MocapFrame>(_ => true)
                                .SortBy(e => e.ElapsedMillis)
                                .ToList();

            } // Sometimes the Sort takes to much RAM!
            catch (MongoDB.Driver.MongoCommandException e)
            {
                Logger.Warn("Mongo command failed due to exception {0}", e.ToString());
                frames = framesCollection
                                .Find<MocapFrame>(_ => true)
                                .ToList();
            } catch(Exception e)
            {
                Logger.Error("Severe Error: {0}", e.ToString());
                frames = null;
            }
            return buildSession(frames, sessionName);
        }


        private Session getStartTilDataEnd(uint startTimestamp, IMongoCollection<MocapFrame> framesCollection, string sessionName)
        {
            var gtFilter = Builders<MocapFrame>.Filter.Gt("ElapsedMillis", startTimestamp);
            List <MocapFrame> frames = null;
            try
            {
                frames = framesCollection
                        .Find<MocapFrame>(gtFilter)
                        .SortBy(e => e.ElapsedMillis)
                        .ToList();

            } // Sometimes the Sort takes to much RAM!
            catch (MongoDB.Driver.MongoCommandException e)
            {
                Logger.Warn("Mongo command failed due to exception {0}", e.ToString());
                frames = framesCollection
                        .Find<MocapFrame>(gtFilter)
                        .ToList();
            }
            catch (Exception e)
            {
                Logger.Error("Severe Error: {0}", e.ToString());
                frames = null;
            }
            
            return buildSession(frames, sessionName);
        }

        private Session getDocumentStartTilEnd(int endTimestamp, IMongoCollection<MocapFrame> framesCollection, string sessionName)
        {
            var ltFilter = Builders<MocapFrame>.Filter.Lt("ElapsedMillis", endTimestamp);
            List<MocapFrame> frames = null;

            try
            {
                frames = framesCollection
                    .Find<MocapFrame>(ltFilter)
                    .SortBy(e => e.ElapsedMillis)
                    .ToList();

            } // Sometimes the Sort takes to much RAM!
            catch (MongoDB.Driver.MongoCommandException e)
            {
                Logger.Warn("Mongo command failed due to exception {0}", e.ToString());
                frames = framesCollection
                    .Find<MocapFrame>(ltFilter)
                    .ToList();
            }
            catch (Exception e)
            {
                Logger.Error("Severe Error: {0}", e.ToString());
                frames = null;
            }

            return buildSession(frames, sessionName);
        }

        public string GetStorageName()
        {
            return "mongo";
        }
    }
}