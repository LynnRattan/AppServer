namespace AppServer.DTO
{
    public class OrderDTO
    {
        public string Adress { get; set; }

        public OrderDTO() { }

        public OrderDTO(Models.Order modelsOrder)
        {
            this.Adress = modelsOrder.Adress;
        }
    }
}
