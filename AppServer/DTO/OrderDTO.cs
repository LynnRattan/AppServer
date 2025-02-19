using AppServer.Models;

namespace AppServer.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? BakerId { get; set; }

        public DateOnly? OrderDate { get; set; }
        public DateOnly? ArrivalDate { get; set; }

        public string Adress { get; set; }

        public double? TotalPrice { get; set; }
        public int? StatusCode { get; set; }

        public UserDTO? TheUser { get; set; }
        public BakerDTO? TheBaker { get; set; }

        public OrderDTO() { }

        public OrderDTO(Models.Order modelsOrder)
        {
            this.Id = modelsOrder.OrderId;
            this.UserId = modelsOrder.UserId;
            this.BakerId = modelsOrder.BakerId;
            this.OrderDate=modelsOrder.OrderDate;
            this.ArrivalDate = modelsOrder.ArrivalDate;
            this.Adress = modelsOrder.Adress;
            this.TotalPrice= modelsOrder.TotalPrice;
            this.StatusCode = modelsOrder.StatusCode;
            if (modelsOrder.User != null)
            {
                this.TheUser = new UserDTO(modelsOrder.User);
            }
            if (modelsOrder.Baker != null)
            {
                this.TheBaker = new BakerDTO(modelsOrder.Baker);
            }
        }

        public Models.Order GetModels()
        {
            Models.Order modelsDessert = new Models.Order()
            {
                OrderId = this.Id,
                UserId = this.UserId,
                BakerId = this.BakerId,
                OrderDate= this.OrderDate,
                ArrivalDate = this.ArrivalDate,
                Adress = this.Adress,
                TotalPrice= this.TotalPrice,
                StatusCode = this.StatusCode
            };

            return modelsDessert;
        }
    }
}
