using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace MovieCollector
{
    [Table("box_office")]
    public class BoxOfficeModel
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }

        public string description { get; set; } = string.Empty;

        public decimal box_office { get; set; } = 0;

        public string img_url { get; set; } = string.Empty;

        public string detail_url { get; set; } = string.Empty;

        public DateTime creation_time { get; set; } = DateTime.UtcNow.AddHours(8);

        public DateTime update_time { get; set; } = DateTime.UtcNow.AddHours(8);
    }
}
