using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
//用于分页的工厂类

namespace FakeXiecheng.API.Helper
{
    public class PaginationList<T> : List<T>
    {
        public int TotalPages { get;private set; }//页面总量
        public int TotalCount { get; private set; }//数据总量
        public bool HasPrevious => CurrentPage > 1;//判断是否有上一页
        public bool HasNext => CurrentPage < TotalPages;//判断是否有下一页

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public PaginationList(int totalCount, int currentPage, int pageSize, List<T> items)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            AddRange(items);
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        //静态方法不需要实例化就可调用
        public async static Task<PaginationList<T>> CreateAsync(int currentPage, int pageSize, IQueryable<T> result)
        {
            var totalCount = await result.CountAsync();
            //分页
            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip);
            result = result.Take(pageSize);

            var items = await result.ToListAsync();

            return new PaginationList<T>(totalCount, currentPage, pageSize, items);
        }
    }
}
