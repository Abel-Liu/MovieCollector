using Dapper.Contrib.Extensions;
using DotnetSpider.Core;
using DotnetSpider.Core.Downloader;
using DotnetSpider.Core.Scheduler;
using DotnetSpider.Core.Selector;
using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;

namespace MovieCollector
{
    /// <summary>
    /// 网络票房
    /// </summary>
    public class BoxOffice
    {
        public static void Run()
        {
            var site = new Site { EncodingName = "UTF-8" };
            site.AddStartUrl("http://58921.com/alltime/wangpiao");

            Spider spider = Spider.Create(site,
                new QueueDuplicateRemovedScheduler(),
                new BoxOfficePageProcessor())
                .AddPipeline(new BoxOfficePipeline())
                .SetDownloader(new HttpClientDownloader())
                .SetThreadNum(1);

            spider.EmptySleepTime = 60000;
            spider.Run();

            DeleteOldData();
        }

        static void DeleteOldData()
        {
            var sql = @"
            UPDATE movie.box_office a 
            SET 
                creation_time = (SELECT 
                        mintime
                    FROM
                        (SELECT 
                            name, MIN(creation_time) AS mintime
                        FROM
                            movie.box_office
                        GROUP BY name) r
                    WHERE
                        r.name = a.name);


            DELETE FROM movie.box_office 
            WHERE
                id IN (SELECT 
                    id
                FROM
                    (SELECT 
                        id
                    FROM
                        movie.box_office a
                    INNER JOIN (SELECT 
                        name, MAX(id) AS tid
                    FROM
                        movie.box_office
                    GROUP BY name
                    HAVING COUNT(*) > 1) r ON a.name = r.name
        
                    WHERE
                        a.id <> r.tid) i);";

            using (MySqlConnection conn = new MySqlConnection(Program.Configuration["ConnectionString"]))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();
            }
        }
    }

    public class BoxOfficePipeline : BasePipelineSQL
    {
        public override void Process(ResultItems resultItems)
        {
            var result = resultItems.Results["VideoResult"];
            if (result != null)
            {
                Connection.Insert(result as IEnumerable<BoxOfficeModel>);
            }
        }
    }

    public class BoxOfficePageProcessor : PageProcessorWithLog
    {
        protected override void Handle(Page page)
        {
            List<BoxOfficeModel> results = new List<BoxOfficeModel>();
            var items = page.Selectable.SelectList(Selectors.XPath("//div[@class='table-responsive']/table//tr")).Nodes();

            foreach (var item in items)
            {
                var title = item.Select(Selectors.XPath("./td[2]/a")).GetValue();
                if (string.IsNullOrEmpty(title))
                    continue;

                BoxOfficeModel model = new BoxOfficeModel()
                {
                    name = title,
                    detail_url = item.Select(Selectors.XPath("./td[2]/a/@href")).GetValue(),
                };

                var people = item.Select(Selectors.XPath("./td[5]")).GetValue().HumanToNumber();
                var price = item.Select(Selectors.XPath("./td[6]")).GetValue().HumanToNumber();
                if (people.HasValue && price.HasValue)
                {
                    model.box_office = (int)(people.Value * price.Value);
                }

                results.Add(model);
            }

            if (results.Count > 0)
                page.AddResultItem("VideoResult", results);

            var next = page.Selectable.Select(Selectors.XPath("//li[@class='pager_next']/a/@href")).GetValue();
            if (!string.IsNullOrEmpty(next))
            {
                page.AddTargetRequest(next);
            }
        }
    }
}
