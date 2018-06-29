using System.Diagnostics;
using VinteR.Model;
using System;

namespace VinteR.Adapter
{
    /// <summary>
    /// Delegate that must be used to retrieve frames from a <see cref="IInputAdapter"/>
    /// </summary>
    /// <param name="adapter">Source adapter that has detected the frame</param>
    /// <param name="frame"></param>
    public delegate void MocapFrameAvailableEventHandler(IInputAdapter adapter, MocapFrame frame);

    /// <summary>
    /// Delegate that can be used to get information on errors about input adapters.
    /// </summary>
    /// <param name="adapter"></param>
    /// <param name="e"></param>
    public delegate void ErrorEventHandler(IInputAdapter adapter, Exception e);

    /// <summary>
    /// An input adapter is the source for <see cref="MocapFrame"/> data that this
    /// application depends on.
    /// </summary>
    public interface IInputAdapter
    {
        /// <summary>
        /// Event that is called if a frame is available on this adapter.
        /// </summary>
        event MocapFrameAvailableEventHandler FrameAvailable;

        /// <summary>
        /// Event that is calles if an error occured during the use with
        /// the input source.
        /// </summary>
        event ErrorEventHandler ErrorEvent;

        /// <summary>
        /// <code>true</code> if this adapter is enabled. Look at the
        /// "enabled" property inside the app configuration if the adapter
        /// is enabled or not
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Unique identifier of the input adapter. This is espacially used to
        /// track input sources inside this application
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Type of the adapter, primary used to get components related to this instance.
        /// </summary>
        string AdapterType { get; }

        /// <summary>
        /// Configuration of the adapter. This is one json object given in the array
        /// of "adapters" inside the vinter.config.json.
        /// </summary>
        Configuration.Adapter Config { get; set; }

        /// <summary>
        /// Executes this adapter.
        /// </summary>
        void Run();

        /// <summary>
        /// Tries to properly stop this adapter.
        /// </summary>
        void Stop();
    }
}
