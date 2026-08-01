using System.Configuration;

namespace Go8_SQL_Deployment
{
    internal static class AppConfig
    {
        public static List<SQLServer> SQLServers => ConfigurationManager.AppSettings.AllKeys
                                                        .Where(X => X != null && X.StartsWith("Server_"))
                                                        .Select(X => new SQLServer(ConfigurationManager.AppSettings[X]))
                                                        .ToList();
    }
}
