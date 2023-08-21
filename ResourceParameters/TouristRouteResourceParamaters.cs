
//旅游路线资源的参数组合
//封装着api的参数以及参数的处理逻辑

using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace FakeXiecheng.API.ResourceParameters
{
    public class TouristRouteResourceParamaters
    {
        //数据塑形参数
        public string? Fields { get; set; }

        //排序参数
        public string? OrderBy { get; set; }

        public string? Keyword { get; set; }

        public string? RatingOperator { get; set; }//Rating的运算类型部分
        public int? RatingValue { get; set; }//Rating的数字部分
        private string _rating;
        public string? Rating //评分筛选条件：小于、大于、等于,这个参数分为两部分：运算类型+数字部分，比如lessThan3
        {
            get { return _rating; }
            set
            {
               
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");//使用正则表达式提取参数的两部分
                    Match match = regex.Match(value);
                    if (match.Success)
                    {
                        RatingOperator = match.Groups[1].Value;
                        RatingValue = Int32.Parse(match.Groups[2].Value);//把字符串转换为数字
                    }
                }
                _rating = value;
            }
        }


    }
}
