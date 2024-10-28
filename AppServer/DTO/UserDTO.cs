namespace AppServer.DTO
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Mail { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public int UserTypeId { get; set; }

        public double? HighestPrice { get; set; }

        public int? ConfectioneryTypeId { get; set; }


        public UserDTO() { }

        public UserDTO(Models.User modelUser)
        {
            this.UserId = modelUser.UserId;
            this.Username = modelUser.Username;
            this.Password = modelUser.Password;
            this.Mail = modelUser.Mail;
            this.Name = modelUser.Name;
            this.UserTypeId = modelUser.UserTypeId;
            
        }
       
    }
}
