using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppServer.Models;
using System.Runtime.InteropServices.Marshalling;
using AppServer.DTO;
using AppServer.Models;

namespace AppServer.Controllers
{
    [Route("api")]
    [ApiController]
    public class LMBAPIController : ControllerBase
    {
        //a variable to hold a reference to the db context!
        private DBContext context;
        //a variable that hold a reference to web hosting interface (that provide information like the folder on which the server runs etc...)
        private IWebHostEnvironment webHostEnvironment;
        //Use dependency injection to get the db context and web host into the constructor
        public LMBAPIController(DBContext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.webHostEnvironment = env;
        }

        [HttpGet("check")]
        public IActionResult Check()
        {
            try
            {
                return Ok("Works");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] DTO.LoginInfo loginDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Get model user class from DB with matching email. 
                Models.User? modelsUser = context.GetUser(loginDto.Mail);

                //Check if user exist for this email and if password match, if not return Access Denied (Error 403) 
                if (modelsUser == null || modelsUser.Password != loginDto.Password)
                {
                    return Unauthorized();
                }

                //Login suceed! now mark login in session memory!
                HttpContext.Session.SetString("loggedInUser", modelsUser.Mail);

                DTO.UserDTO dtoUser = new DTO.UserDTO(modelsUser);
                //dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.Id);
                return Ok(dtoUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost("signup")]
        public IActionResult Register([FromBody] DTO.UserBakerDTO userBakerDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                DTO.UserDTO userDto = userBakerDto.GetUserModels();

                //Create model user class
                Models.User modelsUser = userDto.GetModels();

                context.Users.Add(modelsUser);
                context.SaveChanges();

                userBakerDto.UserId = modelsUser.UserId;

                if (modelsUser.UserTypeId == 2)
                {
                    DTO.BakerDTO bakerDto = userBakerDto.GetBakerModels();
                    Models.Baker modelsBaker = bakerDto.GetModels();

                    context.Bakers.Add(modelsBaker);
                    context.SaveChanges();
                }

                //User was added!
                DTO.UserDTO dtoUser = new DTO.UserDTO(modelsUser);
                dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.UserId);
                return Ok(dtoUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //[HttpPost("bakersignup")]
        //public IActionResult RegisterBaker([FromBody] DTO.UserBakerDTO userBakerDto)
        //{
        //    try
        //    {
        //        UserDTO userDto = userBakerDto.GetUserModels();
        //        Register(userDto);
        //        //HttpContext.Session.Clear(); ////Logout any previous login attempt
        //        User modelUser = userDto.GetModels();
        //        foreach (User u in context.Users)
        //        {
        //            if (u.UserId == modelUser.UserId)
        //            {
        //                BakerDTO bakerDto = userBakerDto.GetBakerModels();
        //                //Create model user class
        //                Models.Baker modelsBaker = bakerDto.GetModels();

        //                context.Bakers.Add(modelsBaker);
        //                context.SaveChanges();

        //                //User was added!
        //                DTO.BakerDTO dtoBaker = new DTO.BakerDTO(modelsBaker);
        //                return Ok(dtoBaker);
        //            }
        //        }
        //        return BadRequest();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}

        [HttpPost("UploadProfileImage")]
        public async Task<IActionResult> UploadProfileImageAsync(IFormFile file)
        {
            //Check if who is logged in
            string? userMail = HttpContext.Session.GetString("loggedInUser");
            if (string.IsNullOrEmpty(userMail))
            {
                return Unauthorized("User is not logged in");
            }

            //Get model user class from DB with matching email. 
            Models.User? user = context.GetUser(userMail);
            //Clear the tracking of all objects to avoid double tracking
            context.ChangeTracker.Clear();

            if (user == null)
            {
                return Unauthorized("User is not found in the database");
            }


            //Read all files sent
            long imagesSize = 0;

            if (file.Length > 0)
            {
                //Check the file extention!
                string[] allowedExtentions = { ".png", ".jpg" };
                string extention = "";
                if (file.FileName.LastIndexOf(".") > 0)
                {
                    extention = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                }
                if (!allowedExtentions.Where(e => e == extention).Any())
                {
                    //Extention is not supported
                    return BadRequest("File sent with non supported extention");
                }

                //Build path in the web root (better to a specific folder under the web root
                string filePath = $"{this.webHostEnvironment.WebRootPath}\\profileImages\\{user.UserId}{extention}";

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);

                    if (IsImage(stream))
                    {
                        imagesSize += stream.Length;
                    }
                    else
                    {
                        //Delete the file if it is not supported!
                        System.IO.File.Delete(filePath);
                    }

                }

            }

            DTO.UserDTO dtoUser = new DTO.UserDTO(user);
            dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.UserId);
            return Ok(dtoUser);
        }

        //Helper functions

        //this function gets a file stream and check if it is an image
        private static bool IsImage(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            List<string> jpg = new List<string> { "FF", "D8" };
            List<string> bmp = new List<string> { "42", "4D" };
            List<string> gif = new List<string> { "47", "49", "46" };
            List<string> png = new List<string> { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" };
            List<List<string>> imgTypes = new List<List<string>> { jpg, bmp, gif, png };

            List<string> bytesIterated = new List<string>();

            for (int i = 0; i < 8; i++)
            {
                string bit = stream.ReadByte().ToString("X2");
                bytesIterated.Add(bit);

                bool isImage = imgTypes.Any(img => !img.Except(bytesIterated).Any());
                if (isImage)
                {
                    return true;
                }
            }

            return false;
        }

        //this function check which profile image exist and return the virtual path of it.
        //if it does not exist it returns the default profile image virtual path
        private string GetProfileImageVirtualPath(int userId)
        {
            string virtualPath = $"/profileImages/{userId}";
            string path = $"{this.webHostEnvironment.WebRootPath}\\profileImages\\{userId}.png";
            if (System.IO.File.Exists(path))
            {
                virtualPath += ".png";
            }
            else
            {
                path = $"{this.webHostEnvironment.WebRootPath}\\profileImages\\{userId}.jpg";
                if (System.IO.File.Exists(path))
                {
                    virtualPath += ".jpg";
                }
                else
                {
                    virtualPath = $"/profileImages/default.png";
                }
            }

            return virtualPath;
        }

        [HttpPost("adddessert")]
        public IActionResult AddDessert([FromBody] DTO.DessertDTO dessertDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Create model dessert class
                Models.Dessert modelsDessert = dessertDto.GetModels();

                context.Desserts.Add(modelsDessert);
                context.SaveChanges();

                //Dessert was added!
                DTO.DessertDTO dtoDessert = new DTO.DessertDTO(modelsDessert);
                //dtoDessert.DesertImagePath = GetDessertImageVirtualPath(dtoDessert.DessertId);
                return Ok(dtoDessert);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
    }
