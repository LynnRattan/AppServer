namespace AppServer.DTO
{
    public class DessertDTO
    {
        public int DessertId { get; set; }
        public string DessertName { get; set; }

        public int DessertTypeId { get; set; }

        public double Price { get; set; }

        public byte[] DessertImage { get; set; }

        public int StatusCode { get; set; }

        public DessertDTO() { }

        public DessertDTO(Models.Dessert modelDessert)
        {
            this.DessertId = modelDessert.DessertId;
            this.DessertName = modelDessert.DessertName;
            this.DessertTypeId = modelDessert.DessertTypeId;
            this.Price = modelDessert.Price;
            this.DessertImage = modelDessert.DessertImage;
            this.StatusCode = modelDessert.StatusCode;
        }

        public Models.Dessert GetModels()
        {
            Models.Dessert modelsDessert = new Models.Dessert()
            {
                DessertId = this.DessertId,
                DessertName = this.DessertName,
                DessertTypeId = this.DessertTypeId,
                Price = this.Price,
                DessertImage = this.DessertImage,
                StatusCode = this.StatusCode
            };

            return modelsDessert;
        }
    }
}
