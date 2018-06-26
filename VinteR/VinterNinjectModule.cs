﻿using Ninject.Modules;
using VinteR.Adapter;
using VinteR.Adapter.Kinect;
using VinteR.Adapter.LeapMotion;
using VinteR.Adapter.OptiTrack;
using VinteR.Configuration;
using VinteR.Datamerge;
using VinteR.Input;
using VinteR.MainApplication;
using VinteR.OutputAdapter;
using VinteR.OutputAdapter.Rest;
using VinteR.OutputManager;
using VinteR.Serialization;
using VinteR.Tracking;
using VinteR.Transform;

namespace VinteR
{
    public class VinterNinjectModule : NinjectModule
    {

        public override void Load()
        {
            Bind<IMainApplication>().To<MainApplication.MainApplication>().InSingletonScope();
            Bind<IConfigurationService>().To<VinterConfigurationService>().InSingletonScope();

            Bind<IInputAdapter>().To<LeapMotionAdapter>().Named(LeapMotionAdapter.AdapterTypeName);
            Bind<IInputAdapter>().To<KinectAdapter>().Named(KinectAdapter.AdapterTypeName);
            Bind<IInputAdapter>().To<OptiTrackAdapter>().Named(OptiTrackAdapter.AdapterTypeName);

            Bind<ITransformator>().To<Transformator>();
            Bind<IAdapterTracker>().To<OptiTrackAdapterTracker>().InSingletonScope();
            Bind<IOptiTrackClient>().To<OptiTrackClient>().InSingletonScope();

            Bind<IDataMerger>().To<LeapMotionMerger>().Named(LeapMotionAdapter.AdapterTypeName);
            Bind<IDataMerger>().To<KinectMerger>().Named(KinectAdapter.AdapterTypeName);
            Bind<IDataMerger>().To<OptiTrackMerger>().Named(OptiTrackAdapter.AdapterTypeName);

            Bind<IOutputManager>().To<OutputManager.OutputManager>().InThreadScope();
            Bind<IOutputAdapter>().To<ConsoleOutputAdapter>().InThreadScope();
            Bind<IOutputAdapter>().To<JsonFileOutputAdapter>().InSingletonScope();
            Bind<IOutputAdapter>().To<MongoOutputAdapter>().InSingletonScope();
            // bind network servers in singleton as they use specific ports
            Bind<IOutputAdapter>().To<UdpSender>().InSingletonScope();
            Bind<IOutputAdapter>().To<VinterRestServer>().InSingletonScope();

            Bind<ISerializer>().To<Serializer>();
            Bind<ISessionNameGenerator>().To<SessionNameGenerator>();

            Bind<IQueryService>().To<MongoDbClient>();
            Bind<IQueryService>().To<JsonStorage>();

            Bind<IVinterRestRoute>().To<OutputAdapter.Rest.Sessions.Get>();
            Bind<IVinterRestRoute>().To<OutputAdapter.Rest.Session.Get>();
        }
    }
}