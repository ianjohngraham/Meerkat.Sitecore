using System.Configuration;
using System.Web.Configuration;

namespace Meerkat.Sitecore.Service
{
    public class ServiceConfig
    {
        private const string connectionBase = "metadata=res://*/Tables.csdl|res://*/Tables.ssdl|res://*/Tables.msl;provider=System.Data.SqlClient;provider connection string=\"{0};multipleactiveresultsets=True;application name=EntityFramework\"";

        public static string GetConnectionString(string databaseName, string filePath)
        {
            string entityConnectionString = ConfigurationManager.ConnectionStrings["Entities"].ConnectionString;
            if (!string.IsNullOrEmpty(databaseName) && !string.IsNullOrEmpty(filePath))
            {
                System.Configuration.Configuration rootWebConfig = OpenConfigFile(filePath);
                if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count > 0)
                {
                    entityConnectionString = rootWebConfig.ConnectionStrings.ConnectionStrings[databaseName].ConnectionString;

                }
                entityConnectionString = string.Format(connectionBase, entityConnectionString);
            }
           return entityConnectionString;
        }

        public static Configuration OpenConfigFile(string configPath)
        {
            string dummyVirtualPath = "";
            WebConfigurationFileMap map = new WebConfigurationFileMap();
            map.VirtualDirectories.Add(dummyVirtualPath, new VirtualDirectoryMapping(configPath, true));
            Configuration configuration = WebConfigurationManager.OpenMappedWebConfiguration(map, dummyVirtualPath);
            return configuration;
        }
    }
}