using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using VinteR.Configuration;
using VinteR.Model;


namespace VinteR.OutputAdapter
{
    /*
     * This class work for write mocapframes as records to a csv file, by using CsvHelper.
     * There is a model class at the end of this page, to mapping attributes of mocapframe to csv headers.
     * There is also a buffer to collect incomming frames till 100 frames, then write them once into the csv file.
     * Because it seems that people must open output stream every time before writing. And the connection will be closed automatically
     * after each works. To avoid too frequent Usage of I/O service, a buffer will be used.
     * When an Event received, the mocapframe will be added to the Buffer. And checked if there are more than 100 items, if yes, running the
     * out put process, and then clear the buffer.
     * Further 
     */
    public class FileOutputAdapter : IOutputAdapter
    {
        private  CsvWriter _writer;
        private static readonly List<MocapFrame> Frames = new List<MocapFrame>();
        private readonly string _homeDir;
        private static bool _isAppend = false;

        public FileOutputAdapter(IConfigurationService configurationService)
        {
            _homeDir = configurationService.GetConfiguration().HomeDir;
        }


        public void OnDataReceived(MocapFrame mocapFrame)
        {
            Frames.Add(mocapFrame);

            
                if (Frames.Count > 150)
                {
                    WriterToCsv();
                    Frames.Clear();
                }



        }

        /*
         * Start Method to this thread, to execute output.
         * 
         */
        public void Start()
        {

            


        }

        public void Stop()
        {
            if (Frames != null && Frames.Count > 0)
            {
                WriterToCsv();
                Frames.Clear();
            }
            
        }

        public void WriterToCsv()
        {
            string filePath = this._homeDir+@"\output.csv";
            // Steamwriter seems closed every time after output. so need new every time. Stranger.
            using (TextWriter writer = new StreamWriter(filePath, true))
            {
               
                _writer = new CsvWriter(writer);
               
                
                _writer.Configuration.RegisterClassMap<CsvDataModel>();

                //poor way to delete unrequired titel when appending new records;

                if (!_isAppend)
                {
                    _isAppend = true;
                }
                else
                {
                    _writer.Configuration.HasHeaderRecord = false;
                }




                //_writer.WriteRecord(mocapFrame);
                _writer.WriteRecords(Frames);
                _writer.Flush();


            }

        }
    }

    public sealed class CsvDataModel : ClassMap<MocapFrame>
    {

        /*
         * Mapping attributes of mocapframe to attributes of csv files.
         * each row of Map() means one Column.
         */
        public CsvDataModel()
        {
            // 1. column
            Map(m => m.SourceId);
            // 2. colunm
            Map(m => m.AdapterType);
            // 3. column and so on.
            Map(m => m.ElapsedMillis);
            Map(m => m.Gesture);
            Map(m => m.Latency);

            /*
             * Bodies is a list. If we take mocapframe as unit to one Tuple.
             * This column may have some problems.
             * I can't get real mocapFrames for now. So I just write it like this.
             * Futher optimization is possible.
             * m.Bodies[0] is the way to get first Object in the list.
             */
            Map(m => m.Bodies);


        }

    }
}
