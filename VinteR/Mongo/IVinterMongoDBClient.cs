using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinteR.Mongo
{
    public interface IVinterMongoDBClient
    {
        IMongoClient getMongoClient();
        void connect();
    }
}
