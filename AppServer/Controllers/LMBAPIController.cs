using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppServer.Models;
using System.Runtime.InteropServices.Marshalling;
using AppServer.DTO;
using AppServer.Models;
using Microsoft.Identity.Client;
using System.Threading.Tasks.Sources;

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
                dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.UserId,"profileImages");
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
                dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.UserId,"profileImages");
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
            dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.UserId,"profileImages");
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

        [HttpPost("UploadDessertImage")]
        public async Task<IActionResult> UploadDessertImageAsync(IFormFile file, [FromQuery] int dessertId, int userId)
        {
            //Check if who is logged in
            //string? userMail = HttpContext.Session.GetString("loggedInUser");
            //if (string.IsNullOrEmpty(userMail))
            //{
            //    return Unauthorized("Baker is not logged in");
            //}

            //Get model user class from DB with matching email. 
            Models.User? user = context.GetUser(userId);
            //Clear the tracking of all objects to avoid double tracking
            context.ChangeTracker.Clear();

            if (user == null)
            {
                return Unauthorized("Baker is not found in the database");
            }
            if(user.UserTypeId != 2)
            {
                return Unauthorized("The logged in user is not a baker");
            }

            //Get model dessert class from DB with matching id. 
            Models.Dessert? dessert = context.GetDessert(dessertId);
            //Clear the tracking of all objects to avoid double tracking
            context.ChangeTracker.Clear();

            if (dessert == null)
            {
                return Unauthorized("Dessert is not found in the database");
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
                string filePath = $"{this.webHostEnvironment.WebRootPath}\\dessertImages\\{dessert.DessertId}{extention}";

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

            DTO.DessertDTO dtoDessert = new DTO.DessertDTO(dessert);
            dtoDessert.DessertImagePath = GetDessertImageVirtualPath(dtoDessert.DessertId, "dessertImages");
            return Ok(dtoDessert);
        }

        

       

        //this function check which profile image exist and return the virtual path of it.
        //if it does not exist it returns the default profile image virtual path
        private string GetProfileImageVirtualPath(int Id,string folder)
        {
            string virtualPath = $"/{folder}/{Id}";
            string path = $"{this.webHostEnvironment.WebRootPath}\\{folder}\\{Id}.png";
            if (System.IO.File.Exists(path))
            {
                virtualPath += ".png";
            }
            else
            {
                path = $"{this.webHostEnvironment.WebRootPath}\\{folder}\\{Id}.jpg";
                if (System.IO.File.Exists(path))
                {
                    virtualPath += ".jpg";
                }
                else
                {
                    virtualPath = $"/{folder}/default.png";
                }
            }

            return virtualPath;
        }

        //Same operation for dessert
        private string GetDessertImageVirtualPath(int Id, string folder)
        {
            string virtualPath = $"/{folder}/{Id}";
            string path = $"{this.webHostEnvironment.WebRootPath}\\{folder}\\{Id}.png";
            if (System.IO.File.Exists(path))
            {
                virtualPath += ".png";
            }
            else
            {
                path = $"{this.webHostEnvironment.WebRootPath}\\{folder}\\{Id}.jpg";
                if (System.IO.File.Exists(path))
                {
                    virtualPath += ".jpg";
                }
                else
                {
                    virtualPath = $"/{folder}/defaultD.png";
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
                //dtoDessert.DessertImagePath = GetDessertImageVirtualPath(dtoDessert.DessertId, "dessertImage");
                return Ok(dtoDessert);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("getbakers")]
        public List<BakerDTO> GetBakers()
        {
            try
            {
                List<Baker> bakers =  context.GetBakers();
                List<BakerDTO> newBakers = new();
                foreach (Baker b in bakers)
                {
                    newBakers.Add(new BakerDTO()
                    {
                        BakerId = b.BakerId,
                        ConfectioneryName = b.ConfectioneryName,
                        HighestPrice = b.HighestPrice,
                        ConfectioneryTypeId = b.ConfectioneryTypeId,
                        StatusCode = b.StatusCode,
                        Profits = b.Profits
                    });
                }
                return newBakers;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        [HttpPost("approvebaker")]
        public IActionResult ApproveBaker([FromBody]int bakerId)
        {
            if (context.Bakers.Where<Baker>(b => b.BakerId == bakerId).FirstOrDefault() != null && context.Bakers.Where<Baker>(b => b.BakerId == bakerId).FirstOrDefault().StatusCode == 1)
            {
                try
            {

                Baker baker = context.GetBaker(bakerId);
                baker.StatusCode = 2;
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("declinebaker")]
        public IActionResult DeclineBaker([FromBody]int bakerId)
        {
            if (context.Bakers.Where<Baker>(b => b.BakerId == bakerId).FirstOrDefault() != null && context.Bakers.Where<Baker>(b => b.BakerId == bakerId).FirstOrDefault().StatusCode == 1)
            {
                try
                {
                    Baker baker = context.GetBaker(bakerId);
                    baker.StatusCode = 3;
                    context.SaveChanges();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("getdesserts")]
        public List<DessertDTO> GetDesserts()
        {
            try
            {
                List<Dessert> desserts = context.GetDesserts();
                List<DessertDTO> newDesserts = new();
                foreach (Dessert d in desserts)
                {
                    newDesserts.Add(new DessertDTO()
                    {
                        DessertId = d.DessertId,
                        DessertName = d.DessertName,
                        DessertTypeId = d.DessertTypeId,
                        Price = d.Price,
                        StatusCode = d.StatusCode,
                        BakerId = d.BakerId
                    });
                }
                return newDesserts;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost("approvedessert")]
        public IActionResult ApproveDessert([FromBody] int dessertId)
        {
            if (context.Desserts.Where<Dessert>(b => b.DessertId == dessertId).FirstOrDefault() != null && context.Desserts.Where<Dessert>(b => b.DessertId == dessertId).FirstOrDefault().StatusCode == 1)
            {
                try
                {

                    Dessert dessert = context.GetDessert(dessertId);
                    dessert.StatusCode = 2;
                    context.SaveChanges();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("declinedessert")]
        public IActionResult DeclineDessert([FromBody] int dessertId)
        {
            if (context.Desserts.Where<Dessert>(b => b.DessertId == dessertId).FirstOrDefault() != null && context.Desserts.Where<Dessert>(b => b.DessertId == dessertId).FirstOrDefault().StatusCode == 1)
            {
                try
                {

                    Dessert dessert = context.GetDessert(dessertId);
                    dessert.StatusCode = 3;
                    context.SaveChanges();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpGet("getconfectionerytypes")]
        public List<ConfectioneryTypeDTO> GetConfectioneryTypes()
        {
            try
            {
                List<DTO.ConfectioneryTypeDTO> dtoConfectioneryTypes = new List<DTO.ConfectioneryTypeDTO>();
                List<ConfectioneryType> modelTypes = context.ConfectioneryTypes.ToList();
                foreach (ConfectioneryType type in modelTypes)
                {
                    dtoConfectioneryTypes.Add(new DTO.ConfectioneryTypeDTO()
                    {
                        ConfectioneryTypeId = type.ConfectioneryTypeId,
                        ConfectioneryTypeName = type.ConfectioneryTypeName
                    });
                }
                return dtoConfectioneryTypes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet("getdesserttypes")]
        public List<DessertTypeDTO> GetDessertTypes()
        {
            try
            {
                List<DTO.DessertTypeDTO> dtoDessertTypes = new List<DTO.DessertTypeDTO>();
                List<DessertType> modelTypes = context.DessertTypes.ToList();
                foreach (DessertType type in modelTypes)
                {
                    dtoDessertTypes.Add(new DTO.DessertTypeDTO()
                    {
                        DessertTypeId = type.DessertTypeId,
                        DessertTypeName = type.DessertTypeName
                    });
                }
                return dtoDessertTypes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost("getbakerdesserts")]
        public List<DessertDTO> GetBakerDesserts([FromBody] int bakerId)
        {
            try
            {
                List<DTO.DessertDTO> desserts = new List<DTO.DessertDTO>();
                List<Dessert> modelDesserts = context.GetDesserts();
                foreach (Dessert d in modelDesserts)
                {
                    if (d.BakerId == bakerId)
                    {
                        desserts.Add(new DTO.DessertDTO()
                        {
                            DessertId = d.DessertId,
                            DessertName = d.DessertName,
                            DessertTypeId = d.DessertTypeId,
                            Price = d.Price,
                            StatusCode = d.StatusCode,
                            BakerId = d.BakerId
                        });
                    }
                }
                return desserts;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        [HttpPost("getbaker")]
        public IActionResult GetBaker([FromBody] int bakerId)
        {
            try
            {
               
                Baker modelBaker = context.GetBaker(bakerId);
                if (modelBaker != null)
                {
                    BakerDTO bakerDTO = new BakerDTO()
                    {
                        BakerId = modelBaker.BakerId,
                        ConfectioneryName = modelBaker.ConfectioneryName,
                        HighestPrice = modelBaker.HighestPrice,
                        ConfectioneryTypeId = modelBaker.ConfectioneryTypeId,
                        StatusCode = modelBaker.StatusCode,
                        Profits = modelBaker.Profits

                    };
                    return Ok(bakerDTO);
                }
                else return null;  
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getstatusestypes")]
        public List<StatusDTO> GetStatusesTypes()
        {
            try
            {
                List<DTO.StatusDTO> dtoStatusTypes = new List<DTO.StatusDTO>();
                List<Status> modelTypes = context.Statuses.ToList();
                foreach (Status type in modelTypes)
                {
                    dtoStatusTypes.Add(new DTO.StatusDTO()
                    {
                        StatusCode = type.StatusCode,
                        StatusName = type.StatusName
                    });
                }
                return dtoStatusTypes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost("updateHighestPrice")]
        public IActionResult UpdateHighestPrice([FromBody] BakerDTO bakerDto)
        {

            try
            {
                Baker? baker = context.Bakers.Where<Baker>(b => b.BakerId == bakerDto.BakerId).FirstOrDefault();
                    baker.HighestPrice = bakerDto.HighestPrice;
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getusers")]
        public List<UserDTO> GetUsers()
        {
            try
            {
                List<DTO.UserDTO> dtoUsers = new List<DTO.UserDTO>();
                List<User> modelUsers = context.Users.ToList();
                foreach (User u in modelUsers)
                {
                    dtoUsers.Add(new DTO.UserDTO()
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        Mail = u.Mail,
                        Password = u.Password,
                        ProfileName = u.ProfileName,
                        UserTypeId = u.UserTypeId
                    });
                }
                return dtoUsers;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



    }
}
