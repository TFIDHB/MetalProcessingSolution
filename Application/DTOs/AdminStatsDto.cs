namespace Application.DTOs
{
    public class AdminStatsDto
    {
        public int ServicesCount { get; set; }
        public int UnliquidsCount { get; set; }
        public int OurProductsCount { get; set; }
        public int GeneralGoodsCount { get; set; }
        public decimal TotalUnliquidsValue { get; set; }
        public decimal TotalOurProductsValue { get; set; }
        public decimal TotalGeneralGoodsValue { get; set; }
    }
}