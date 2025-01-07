namespace AppServer.DTO
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Mail { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ProfileName { get; set; }

        public int? UserTypeId { get; set; }

        public string ProfileImagePath { get; set; } = "";



        public UserDTO() { }

        public UserDTO(Models.User modelUser)
        {
            this.UserId = modelUser.UserId;
            this.Username = modelUser.Username;
            this.Password = modelUser.Password;
            this.Mail = modelUser.Mail;
            this.ProfileName = modelUser.ProfileName;
            this.UserTypeId = modelUser.UserTypeId;
            
        }

        public Models.User GetModels()
        {
            Models.User modelsUser = new Models.User()
            {
                UserId = this.UserId,
                Username = this.Username,
                Mail = this.Mail,
                Password = this.Password,
                ProfileName= this.ProfileName,
                UserTypeId= this.UserTypeId
            };

            return modelsUser;
        }

    }
}
