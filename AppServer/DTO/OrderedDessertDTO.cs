namespace AppServer.DTO
{
    public class OrderedDessertDTO
    {
        public int DessertId { get; set; }
        public int OrderID { get; set; }
        public int? Quantity { get; set; }
        public int? StatusCode { get; set; }
        
        public double? Price { get; set; }

        public OrderedDessertDTO() { }

        public OrderedDessertDTO(Models.OrderedDessert modelsOrderedDessert)
        {
            this.OrderID= modelsOrderedDessert.OrderId;
            this.DessertId = modelsOrderedDessert.DessertId;
            this.Quantity = modelsOrderedDessert.Quantity;
            this.StatusCode = modelsOrderedDessert.StatusCode;
            this.Price = modelsOrderedDessert.Price;
        }
    }
}
