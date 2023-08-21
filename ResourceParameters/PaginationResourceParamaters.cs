
//分页相关参数

namespace FakeXiecheng.API.ResourceParameters
{
    public class PaginationResourceParamaters
    {
        //list专用
        public string? All { get; set; }

        private int _pageNumber = 1;
        private int _pageSize = 10;
        const int maxPageSize = 50;
        public int PageNumber
        {
            get { return _pageNumber; }
            set { if (value >= 1) _pageNumber = value; }
        }
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (value >= 1)
                {
                    _pageSize = (value > maxPageSize) ? maxPageSize : value;
                }
            }
        }

    }
}
