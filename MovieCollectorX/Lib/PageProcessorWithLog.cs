using System;
using System.Collections.Generic;
using System.Text;
using DotnetSpider.Core;
using log4net;

namespace MovieCollector
{
    public class PageProcessorWithLog : DotnetSpider.Core.Processor.BasePageProcessor
    {
        protected static ILog logger = LogManager.GetLogger("default", "default");

        protected override void Handle(Page page)
        {
        }
    }
}
