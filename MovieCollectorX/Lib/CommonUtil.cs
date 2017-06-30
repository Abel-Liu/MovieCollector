using System;
using System.Collections.Generic;
using System.Text;

namespace MovieCollector
{
    public static class CommonUtil
    {
        /// <summary>
        /// 转换如2.3万为23000，目前包括万、亿
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal? HumanToNumber(this string str)
        {
            try
            {
                decimal r = 0;
                if (string.IsNullOrEmpty(str))
                    return null;
                str = str.Replace(",", "");

                if (str.Contains("万"))
                    r = Convert.ToDecimal(str.Replace("万", "")) * 10000;
                else if (str.Contains("亿"))
                    r = Convert.ToDecimal(str.Replace("亿", "")) * 100000000;
                else
                    r = Convert.ToDecimal(str);

                return r;
            }
            catch
            {
                return null;
            }
        }
    }
}
