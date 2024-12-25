namespace AppServer.DTO
{
    public class UserBakerDTO
    {
        public int UserId {  get; set; }
        public string Mail {  get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ProfileName { get; set; }
        public int UserTypeId { get; set; }
        public string ProfileImagePath { get; set; } = "";
        public double? HighestPrice { get; set; }
        public int? ConfectioneryTypeId {  get; set; }
        public int StatusCode { get; set; }
        public double Profits { get; set; }

        public UserBakerDTO() { }

        public UserBakerDTO(DTO.UserDTO userDto, DTO.BakerDTO bakerDto)
        {
            this.UserId = userDto.UserId;
            this.Username = userDto.Username;
            this.Password = userDto.Password;
            this.Mail = userDto.Mail;
            this.ProfileName = userDto.ProfileName;
            this.UserTypeId = userDto.UserTypeId;
            this.HighestPrice = bakerDto.HighestPrice;
            this.ConfectioneryTypeId = bakerDto.ConfectioneryTypeId;
            this.StatusCode = bakerDto.StatusCode;
            this.Profits = bakerDto.Profits;
        }
        public DTO.UserDTO GetUserModels()
        {
            DTO.UserDTO user = new DTO.UserDTO()
            {
                Username = this.Username,
                Password = this.Password,
                Mail = this.Mail,
                ProfileName = this.ProfileName,
                UserTypeId = this.UserTypeId,
            };
            return user;
        }
        public DTO.BakerDTO GetBakerModels()
        {
            DTO.BakerDTO baker = new DTO.BakerDTO()
            {
                BakerId = this.UserId,
                HighestPrice = this.HighestPrice,
                ConfectioneryTypeId = this.ConfectioneryTypeId,
                StatusCode = this.StatusCode,
                Profits = this.Profits
            };
            return baker;
        }
    }
}
