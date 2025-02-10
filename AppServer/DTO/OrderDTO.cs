namespace AppServer.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? BakerId { get; set; }

        public DateOnly? OrderDate { get; set; }
        public DateOnly? ArrivelDate { get; set; }

        public string Adress { get; set; }

        public double? TotalPrice { get; set; }
        public int? StatusCode { get; set; }

        public OrderDTO() { }

        public OrderDTO(Models.Order modelsOrder)
        {
            this.Id = modelsOrder.OrderId;
            this.UserId = modelsOrder.UserId;
            this.BakerId = modelsOrder.BakerId;
            this.OrderDate=modelsOrder.OrderDate;
            this.ArrivelDate = modelsOrder.ArrivalDate;
            this.Adress = modelsOrder.Adress;
            this.TotalPrice= modelsOrder.TotalPrice;
            this.StatusCode = modelsOrder.StatusCode;
        }
    }
}
