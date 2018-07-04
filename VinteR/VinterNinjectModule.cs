using Ninject.Modules;
using VinteR.Adapter;
using VinteR.Adapter.Kinect;
using VinteR.Adapter.LeapMotion;
using VinteR.Adapter.OptiTrack;
using VinteR.Configuration;
using VinteR.Datamerge;
using VinteR.Input;
using VinteR.MainApplication;
using VinteR.Mongo;
using VinteR.OutputAdapter;
using VinteR.OutputManager;
using VinteR.Rest;
using VinteR.Serialization;
using VinteR.Streaming;
using VinteR.Tracking;
using VinteR.Transform;

namespace VinteR
{
    public class VinterNinjectModule : NinjectModule
    {

        public override void Load()
        {
            Bind<IMainApplication>().To<MainApplication.MainApplication>().InSingletonScope();
            Bind<IRecordService>().To<RecordService>().InSingletonScope();
            Bind<IPlaybackService>().To<PlaybackService>().InSingletonScope();
            Bind<IConfigurationService>().To<VinterConfigurationService>().InSingletonScope();

            Bind<IInputAdapter>().To<LeapMotionAdapter>().Named(HardwareSystems.LeapMotion);
            Bind<IInputAdapter>().To<KinectAdapter>().Named(HardwareSystems.Kinect);
            Bind<IInputAdapter>().To<OptiTrackAdapter>().Named(HardwareSystems.OptiTrack);

            Bind<ITransformator>().To<Transformator>();
            Bind<IAdapterTracker>().To<OptiTrackAdapterTracker>().InSingletonScope();
            Bind<IOptiTrackClient>().To<OptiTrackClient>().InSingletonScope();

            Bind<IDataMerger>().To<LeapMotionMerger>().Named(HardwareSystems.LeapMotion);
            Bind<IDataMerger>().To<KinectMerger>().Named(HardwareSystems.Kinect);
            Bind<IDataMerger>().To<OptiTrackMerger>().Named(HardwareSystems.OptiTrack);

            Bind<IOutputManager>().To<OutputManager.OutputManager>().InThreadScope();
            Bind<IOutputAdapter>().To<ConsoleOutputAdapter>().InThreadScope();
            Bind<IOutputAdapter>().To<JsonFileOutputAdapter>().InSingletonScope();
            Bind<IOutputAdapter>().To<MongoOutputAdapter>().InSingletonScope();

            // bind network servers as singleton as multiple port bindings lead to application errors
            Bind<IStreamingServer>().To<UdpSender>().InSingletonScope();
            Bind<IRestServer>().To<VinterRestServer>().InSingletonScope();

            Bind<ISerializer>().To<Serializer>();
            Bind<ISessionNameGenerator>().To<SessionNameGenerator>();

            Bind<IQueryService>().To<MongoQueryService>();
            Bind<IQueryService>().To<JsonStorage>();

            Bind<IHttpResponseWriter>().To<HttpResponseWriter>();
            Bind<IRestRouter>().To<DefaultRouter>().InSingletonScope();
            Bind<IRestRouter>().To<SessionsRouter>().InSingletonScope();
            Bind<IRestRouter>().To<SessionRouter>().InSingletonScope();

            Bind<ISessionPlayer>().To<SessionPlayer>().InSingletonScope();
            Bind<IVinterMongoDBClient>().To<VinterMongoDBClient>().InSingletonScope();
        }
    }
}