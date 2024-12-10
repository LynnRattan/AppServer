namespace AppServer.DTO
{
    public class BakerDTO
    {
        public int BakerId { get; set; }
        public double? HighestPrice { get; set; }

        public int? ConfectioneryTypeId { get; set; }

        public int StatusCode {  get; set; }

        public BakerDTO() { }

        public BakerDTO(Models.Baker modelBaker)
        {
            this.BakerId = modelBaker.BakerId;
            this.HighestPrice = HighestPrice;
            this.ConfectioneryTypeId = ConfectioneryTypeId;
        }
    }
}
