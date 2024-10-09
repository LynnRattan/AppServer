namespace AppServer.DTO
{
    public class DessertDTO
    {
        public string DessertName { get; set; }

        public int DessertTypeId { get; set; }

        public double Price { get; set; }

        public byte[] DessertImage { get; set; }

        public DessertDTO() { }

        public DessertDTO(Models.Dessert modelDessert)
        {
            this.DessertName = modelDessert.DessertName;
            this.DessertTypeId = modelDessert.DessertTypeId;
            this.Price = modelDessert.Price;
            this.DessertImage = modelDessert.DessertImage;
        }
    }
}
