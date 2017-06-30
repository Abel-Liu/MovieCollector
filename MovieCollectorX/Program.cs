using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace MovieCollector
{
    class Program
    {
        public static IConfigurationRoot Configuration;

        static void Main(string[] args)
        {
#if NETCOREAPP1_1
            var baseDirectory = AppContext.BaseDirectory;
#endif

#if !NETCOREAPP1_1
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif

            #region app config
            var builder = new ConfigurationBuilder()
                .SetBasePath(baseDirectory)
                .AddJsonFile("appsettings.json", true, true);

            Configuration = builder.Build();

            #endregion

            Meituan.Run();
            BoxOffice.Run();
        }
    }
}