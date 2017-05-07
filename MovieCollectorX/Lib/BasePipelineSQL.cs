using System;
using System.Collections.Generic;
using System.Text;
using DotnetSpider.Core;
using MySql.Data.MySqlClient;

namespace MovieCollector
{
    public abstract class BasePipelineSQL : DotnetSpider.Core.Pipeline.BasePipeline
    {
        protected MySqlConnection Connection = new MySqlConnection("server=localhost;database=movie;port=3306;Uid=root;Pwd=;");
        
    }
}
