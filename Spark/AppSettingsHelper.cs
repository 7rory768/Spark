namespace Spark
{
    /// <summary>
    /// This class provides access to the variables in the appsettings.json file.
    /// </summary>
    public class AppSettingsHelper
    {

        #region Constants

        private const string FIELD__DEFAULT_DATABASE = "Default";
        private const string SECTION__CONNECTION_STRINGS = "ConnectionStrings";

        #endregion

        #region Initialization

        /// <summary>
        /// Reference to the appsettings.json configuration file.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor called by the service provider.
        /// Using injection to get the parameters.
        /// </summary>
        public AppSettingsHelper(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        #region Database

        /// <summary>
        /// Database connection string.
        /// </summary>
        public string DATABASE_CONNECTION_STRING
        {
            get
            {
                return Configuration.GetSection(SECTION__CONNECTION_STRINGS).GetValue<string>(FIELD__DEFAULT_DATABASE);
            }
        }

        #endregion

    }
}
