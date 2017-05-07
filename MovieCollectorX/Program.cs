using System;
using System.IO;
using log4net;
using log4net.Config;

namespace MovieCollector
{
    class Program
    {
        static void Main(string[] args)
        {
#if NET_CORE
            var baseDirectory = AppContext.BaseDirectory;
#endif

#if !NET_CORE
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif

            var logCfg = new FileInfo(Path.Combine(baseDirectory, "log4net.config"));
            var repo = LogManager.CreateRepository("default");
            XmlConfigurator.ConfigureAndWatch(repo, logCfg);

            Meituan.Run();
        }
    }
}