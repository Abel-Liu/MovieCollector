using System;
using System.Collections.Generic;
using System.Text;
using DotnetSpider.Core;
using NLog;

namespace MovieCollector
{
    public class PageProcessorWithLog : DotnetSpider.Core.Processor.BasePageProcessor
    {
        protected Logger logger = LogManager.GetCurrentClassLogger();

        protected override void Handle(Page page)
        {
        }
    }
}
