namespace FakeXiecheng.API.Dtos
{
    public class TouristRoutesListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public double? DiscountPresent { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        //public ICollection<TouristRoutePictureDto> TouristRoutePictures { get; set; }
    }
}
