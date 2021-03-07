using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace LiveStreamStore.Lib.Core
{
    public static class Log4NetLoader
    {
        public static void AddLog4NetConfiguration()
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "log4net.config");
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead(configPath));

            var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

        }
    }
}
