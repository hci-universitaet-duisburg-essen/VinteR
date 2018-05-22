namespace VinteR.Configuration
{
    /// <summary>
    /// An instance of <code>IConfigurationService</code> is able to
    /// load the application config and provide it through simple
    /// objects. The application config is located inside the
    /// <code>vinter.config.json</code> file. System specific
    /// properties can be overwritten with the <code>vinter.config.local.json</code>
    /// file.
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Tries to load, validate and return the application configuration.
        /// </summary>
        /// <returns>The <see cref="Configuration"/> or None</returns>
        Configuration GetConfiguration();
    }
}