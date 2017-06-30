using System;
using System.Collections.Generic;
using System.Text;
using DotnetSpider.Core;
using MySql.Data.MySqlClient;

namespace MovieCollector
{
    public abstract class BasePipelineSQL : DotnetSpider.Core.Pipeline.BasePipeline
    {
        protected MySqlConnection Connection = new MySqlConnection(Program.Configuration["ConnectionString"]);
    }
}
