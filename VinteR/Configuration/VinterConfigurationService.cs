using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace VinteR.Configuration
{
    public class VinterConfigurationService : IConfigurationService
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private const string ConfigFileName = "vinter.config.json";
        private const string SchemaFileName = "vinter.config.schema.json";

        private JSchema _schema;
        private Configuration _configuration;

        public Configuration GetConfiguration()
        {
            if (_configuration == null)
            {
                LoadConfiguration();
            }

            return _configuration;
        }

        private void LoadConfiguration()
        {
            LoadSchema();
            var config = ReadJson(ConfigFileName);
            
            if (config.IsValid(_schema))
            {
                // load the config object from 
                this._configuration = JsonConvert.DeserializeObject<Configuration>(config.ToString());
                Logger.Info("VinteR Configuration loaded");
            }
            else
            {
                Logger.Error("Merged configuration is not valid, was {0}", config.ToString());
            }
        }

        private void LoadSchema()
        {
            var json = ReadJson(SchemaFileName);

            this._schema = JSchema.Load(json.CreateReader());
        }

        private static JObject ReadJson(string file)
        {
            JObject obj;
            using (var reader = new StreamReader(file))
            {
                obj = JObject.Load(new JsonTextReader(reader));
                Logger.Info("Loaded config {0}", file);
            }
            return obj;
        }
    }
}