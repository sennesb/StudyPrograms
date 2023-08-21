using FakeXiecheng.API.Dtos;
using System.Dynamic;
using System.Reflection;

//IEnumable拓展类， 添加了ShapData方法，用来进行列表的数据塑形

namespace FakeXiecheng.API.Helper
{
    public static class IEnumableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapDatas<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if (source == null)throw new ArgumentNullException(nameof(source));
            
            var expandoObjectList = new List<ExpandoObject>();

            //避免在列表中遍历数据，创建属性信息列表
            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource).GetProperties
                    (BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                var fieldsAfterSplit = fields.Split(',');
                foreach ( var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();
                    var propertyInfo = typeof(TSource).GetProperty
                        (propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null) throw new Exception($"属性{propertyName}找不到"+ $"{typeof(TSource)}");
                    propertyInfoList.Add(propertyInfo);
                }
            }
            foreach (TSource sourceObject in source)
            {
                //创建动态类型对象，创建数据塑形对象
                var dataShapedOnject = new ExpandoObject();
                foreach (var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(sourceObject);
                    //要将dataShapedOnject转化为字典类型才能添加键值对数据
                    ((IDictionary<string, object>)dataShapedOnject).Add(propertyInfo.Name, propertyValue);
                }
                expandoObjectList.Add(dataShapedOnject);
            }
            //返回塑形完毕的数据
            return expandoObjectList;
        }
    }
}
