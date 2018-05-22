using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace VinteR.Configuration
{
    public class VinterConfigurationService : IConfigurationService
    {
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
            var serializer = new JsonSerializer();
            var json = ReadJson("vinter.config.json");
            using (var jsonReader = new JsonTextReader(new StringReader(json)))
            {
                var validatingReader = new JSchemaValidatingReader(jsonReader) { Schema = _schema };

                this._configuration = JsonConvert.DeserializeObject<Configuration>(json);
            }
        }

        private void LoadSchema()
        {
            var serializer = new JsonSerializer();
            var json = ReadJson("vinter.config.schema.json");

            this._schema = JSchema.Parse(json);
        }

        private static string ReadJson(string file)
        {
            using (var reader = new StreamReader(file))
            {
                return reader.ReadToEnd();
            }
        }
    }
}