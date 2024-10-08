namespace AppServer.DTO
{
    public class UserDTO
    {
        public string Mail { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public int UserTypeId { get; set; }

        public UserDTO() { }

        public UserDTO(Models.User modelUser)
        {
            this.Username = modelUser.Username;
            this.Password = modelUser.Password;
            this.Mail = modelUser.Mail;
            this.Name = modelUser.Name;
            this.UserTypeId = modelUser.UserTypeId;
        }
       
    }
}
