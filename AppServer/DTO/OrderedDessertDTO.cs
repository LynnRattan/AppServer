namespace AppServer.DTO
{
    public class OrderedDessertDTO
    {
        public int Quantity { get; set; }

        public OrderedDessertDTO() { }

        public OrderedDessertDTO(Models.OrderedDessert modelsOrderedDessert)
        {
            this.Quantity = modelsOrderedDessert.Quantity;
        }
    }
}
