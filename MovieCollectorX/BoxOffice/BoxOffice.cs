﻿using System;
using System.Collections.Generic;
using System.Linq;
using DotnetSpider.Core;
using DotnetSpider.Core.Pipeline;
using DotnetSpider.Core.Processor;
using DotnetSpider.Core.Scheduler;
using DotnetSpider.Core.Selector;
using DotnetSpider.Core.Downloader;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;

namespace MovieCollector
{
    public class BoxOffice
    {
        public static void Run()
        {
            var site = new Site { EncodingName = "UTF-8" };
            site.AddStartUrl("http://www.BoxOffice.com/dianying/zuixindianying");

            Spider spider = Spider.Create(site,
                new QueueDuplicateRemovedScheduler(),
                new BoxOfficePageProcessor())
                .AddPipeline(new BoxOfficePipeline())
                .SetDownloader(new HttpClientDownloader())
                .SetThreadNum(1);

            spider.EmptySleepTime = 3000;

            spider.Run();
        }
    }

    public class BoxOfficePipeline : BasePipelineSQL
    {
        public override void Process(ResultItems resultItems)
        {
            var result = resultItems.Results["VideoResult"];
            if (result != null)
            {
                foreach (var m in result as List<BoxOfficeModel>)
                {
                    var query = Connection.Query<BoxOfficeModel>("select * from box_office where name=@name",
                         new { name = m.name });

                    if (query.Count() > 0)
                    {
                        var old = query.First();
                        m.id = old.id;
                        m.creation_time = old.creation_time;

                        Connection.Update(m);
                    }
                    else
                        Connection.Insert(m);
                }
            }
        }
    }

    public class BoxOfficePageProcessor : PageProcessorWithLog
    {
        protected override void Handle(Page page)
        {
            List<BoxOfficeModel> results = new List<BoxOfficeModel>();
            var items = page.Selectable.SelectList(Selectors.XPath("//div[@id='content']/div/div[contains(@class,'movie-cell')]")).Nodes();

            foreach (var item in items)
            {
                var title = item.Select(Selectors.XPath("./h3[@class='movie-cell__title']/a"));
                var titlev = title.GetValue();

                if (string.IsNullOrEmpty(titlev))
                    continue;

                var rateStr = item.Select(Selectors.XPath("./div[@class='movie-cell__detail']/p/strong[@class='rates']")).GetValue(true);
                if (rateStr == null)
                    rateStr = string.Empty;
                rateStr = rateStr.Replace("\r", "").Replace("\n", "").Trim();

                decimal.TryParse(rateStr, out var rate);

                BoxOfficeModel model = new BoxOfficeModel()
                {
                    name = titlev,
                    detail_url = title.Links().GetValue(),
                    img_url = item.Select(Selectors.XPath("./a[@class='movie-cell__cover']/img/@src")).GetValue(),

                };

                results.Add(model);
            }

            if (results.Count > 0)
                page.AddResultItem("VideoResult", results);
        }
    }
}
