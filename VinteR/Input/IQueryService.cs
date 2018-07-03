using System.Collections.Generic;
using VinteR.Model;

namespace VinteR.Input
{
    /// <summary>
    /// A query service is able to load session and frame data from an input source.
    /// </summary>
    public interface IQueryService
    {
        /// <summary>
        /// Returns the storage name of the query service. This name is used to differ query
        /// services and will be shown to the user.
        /// </summary>
        /// <returns></returns>
        string GetStorageName();

        /// <summary>
        /// Returns a list of sessions that are stored inside the backend. Only
        /// metadata is retrieved and not the complete data from all sessions.
        /// </summary>
        /// <returns>A list with all session metadata or an empty list</returns>
        IList<Session> GetSessions();

        /// <summary>
        /// Returns a session that is specified by target name (See <see cref="Session.Name"/>)
        /// with the data (frames and so on) that is given inside the session and is between
        /// start and end timestamp if set.
        /// </summary>
        /// <param name="name">Name of the session</param>
        /// <param name="startTimestamp">Start time in millis from session start.
        /// Zero for the beginning of the session</param>
        /// <param name="endTimestamp">End time in millis from session start. Minus one for the end
        /// of the session</param>
        /// <returns>The session with all specified data or <code>null</code> id it does not exist.</returns>
        Session GetSession(string name, uint startTimestamp = 0, int endTimestamp = -1);
    }
}