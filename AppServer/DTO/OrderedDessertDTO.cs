using AppServer.Models;

namespace AppServer.DTO
{
    public class OrderedDessertDTO
    {
        public int OrderedDessertId {  get; set; }
        public int? DessertId { get; set; }
        public int? OrderId { get; set; }
        public int Quantity { get; set; }
        public int? StatusCode { get; set; }
        public int? UserId { get; set; }
        public int? BakerId { get; set; }

        public double Price { get; set; }

        public DessertDTO TheDessert { get; set; }
        public BakerDTO TheBaker { get; set; }

        public OrderedDessertDTO() { }

        public OrderedDessertDTO(Models.OrderedDessert modelsOrderedDessert)
        {
            this.OrderedDessertId= modelsOrderedDessert.OrderedDessertId;
            this.OrderId= modelsOrderedDessert.OrderId;
            this.DessertId = modelsOrderedDessert.DessertId;
            this.Quantity = modelsOrderedDessert.Quantity;
            this.StatusCode = modelsOrderedDessert.StatusCode;
            this.Price = modelsOrderedDessert.Price;
            this.UserId = modelsOrderedDessert.UserId;
            this.BakerId= modelsOrderedDessert.BakerId;
            if (modelsOrderedDessert.Dessert != null)
            {
                this.TheDessert = new DessertDTO(modelsOrderedDessert.Dessert);
            }
            if (modelsOrderedDessert.Baker!= null)
            {
                this.TheBaker = new BakerDTO(modelsOrderedDessert.Baker);
            }


        }

        public Models.OrderedDessert GetModels()
        {
            Models.OrderedDessert modelsOrderedDessert = new Models.OrderedDessert()
            {
                OrderedDessertId=this.OrderedDessertId,
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
