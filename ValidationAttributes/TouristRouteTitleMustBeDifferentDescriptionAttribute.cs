

using FakeXiecheng.API.Dtos;
using System.ComponentModel.DataAnnotations;

namespace FakeXiecheng.API.ValidationAttributes
{
    public class TouristRouteTitleMustBeDifferentDescriptionAttribute : ValidationAttribute
    {
        //重写IsValid自定义数据验证
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var touristRouteDto = (TouristRouteForManipulationDto)validationContext.ObjectInstance;
            if (touristRouteDto.Title == touristRouteDto.Description)
            {
                return new ValidationResult
                    (
                    "路线名称必须与路线描述不同",
                    new[] { "TouristRouteForCreationDto" }
                    );
            }
            return ValidationResult.Success;
        }
    }
}
