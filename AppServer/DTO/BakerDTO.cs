namespace AppServer.DTO
{
    public class BakerDTO
    {
        public int BakerId { get; set; }
        public double? HighestPrice { get; set; }

        public int? ConfectioneryTypeId { get; set; }

        public int StatusCode { get; set; }

        public double Profits { get; set; }
        public BakerDTO() { }

        public BakerDTO(Models.Baker modelBaker)
        {
            this.BakerId = modelBaker.BakerId;
            this.HighestPrice = modelBaker.HighestPrice;
            this.ConfectioneryTypeId = modelBaker.ConfectioneryTypeId;
            this.StatusCode = modelBaker.StatusCode;
            this.Profits = modelBaker.Profits;
        }

        public Models.Baker GetModels()
        {
            Models.Baker modelsBaker = new Models.Baker()
            {
                BakerId = this.BakerId,
                HighestPrice = this.HighestPrice,
                ConfectioneryTypeId = this.ConfectioneryTypeId,
                StatusCode = this.StatusCode,
                Profits = this.Profits
            };

            return modelsBaker;
        }

    }
}
