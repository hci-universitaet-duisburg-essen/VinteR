using System.Diagnostics;
using VinteR.Model;
using System;

namespace VinteR.Adapter
{
    public delegate void MocapFrameAvailableEventHandler(IInputAdapter adapter, MocapFrame frame);
    public delegate void ErrorEventHandler(IInputAdapter adapter, Exception e);

    public interface IInputAdapter
    {
        event MocapFrameAvailableEventHandler FrameAvailable;
        event ErrorEventHandler ErrorEvent;

        bool Enabled { get; }

        string Name { get; set; }
        Configuration.Adapter Config { get; set; }

        void Run();

        void Stop();
    }
}
