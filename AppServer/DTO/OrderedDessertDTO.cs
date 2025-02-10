namespace AppServer.DTO
{
    public class OrderedDessertDTO
    {
        public int DessertId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public int? StatusCode { get; set; }
        public int? UserId { get; set; }
        public int? BakerId { get; set; }

        public double Price { get; set; }

        public OrderedDessertDTO() { }

        public OrderedDessertDTO(Models.OrderedDessert modelsOrderedDessert)
        {
            this.OrderId= modelsOrderedDessert.OrderId;
            this.DessertId = modelsOrderedDessert.DessertId;
            this.Quantity = modelsOrderedDessert.Quantity;
            this.StatusCode = modelsOrderedDessert.StatusCode;
            this.Price = modelsOrderedDessert.Price;
            this.UserId = modelsOrderedDessert.UserId;
            this.BakerId= modelsOrderedDessert.BakerId;
        }

        public Models.OrderedDessert GetModels()
        {
            Models.OrderedDessert modelsOrderedDessert = new Models.OrderedDessert()
            {
                OrderId = this.OrderId,
                DessertId = this.DessertId,
                Quantity = this.Quantity,
                StatusCode = this.StatusCode,
                Price = this.Price,
                UserId = this.UserId,
                BakerId = this.BakerId

            };

            return modelsOrderedDessert;
        }
    }
}
