﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppServer.Models;
using System.Runtime.InteropServices.Marshalling;
using AppServer.DTO;
using AppServer.Models;
using Microsoft.Identity.Client;
using System.Threading.Tasks.Sources;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        #region check
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
        #endregion
        #region
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
        #endregion
        #region
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
        #endregion
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
        #region
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
        #endregion
        //Helper functions

        //this function gets a file stream and check if it is an image
        #region
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
        #endregion
        #region
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
            dtoDessert.DessertImage = GetDessertImageVirtualPath(dtoDessert.DessertId, "dessertImages");
            dessert = context.Desserts.Where(d => d.DessertId == dtoDessert.DessertId).FirstOrDefault();
            dessert.DessertImage = dtoDessert.DessertImage;
            context.SaveChanges();
            return Ok(dtoDessert);
        }
        #endregion




        //this function check which profile image exist and return the virtual path of it.
        //if it does not exist it returns the default profile image virtual path
        #region
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
        #endregion

        //Same operation for dessert
        #region
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
        #endregion

        #region adddessert
        [HttpPost("adddessert")]
        public IActionResult AddDessert([FromBody] DTO.DessertDTO dessertDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Create model dessert class
                Models.Dessert modelsDessert = dessertDto.GetModels();
                modelsDessert.DessertImage = "/dessertImages/defaultD.png";
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
        #endregion

        #region getbakers
        [HttpGet("getbakers")]
        public List<BakerDTO> GetBakers()
        {
            try
            {
                List<Baker> bakers =  context.GetBakers();
                List<BakerDTO> newBakers = new();
                foreach (Baker b in bakers)
                {
                    BakerDTO bdto = new BakerDTO(b);
                    bdto.UserNavigation.ProfileImagePath = GetProfileImageVirtualPath(bdto.BakerId, "profileImages");
                    newBakers.Add(bdto);
                    
                }
                return newBakers;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region approvebaker
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
        #endregion

        #region declinebaker
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
        #endregion

        #region getdesserts
        [HttpGet("getdesserts")]
        public List<DessertDTO> GetDesserts()
        {
            try
            {
                List<Dessert> desserts = context.GetDesserts();
                List<DessertDTO> newDesserts = new();
                foreach (Dessert d in desserts)
                {
                    newDesserts.Add(new DessertDTO(d));
                }
                return newDesserts;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region approvedessert
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
        #endregion

        #region declinedessert
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
        #endregion

        #region getconfectionerytypes
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
        #endregion

        #region getdesserttypes
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
        #endregion

        #region getbakerdesserts
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
        #endregion

        #region getbaker
        [HttpPost("getbaker")]
        public IActionResult GetBaker([FromBody] int bakerId)
        {
            try
            {
               
                Baker modelBaker = context.GetBaker(bakerId);
                if (modelBaker != null)
                {
                    BakerDTO bakerDTO = new BakerDTO(modelBaker);

                    return Ok(bakerDTO);
                }
                else return null;  
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region getstatusestypes
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
        #endregion

        #region updateHighestPrice
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
        #endregion

        #region getusers
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
        #endregion


        #region updateDessertImage

        [HttpPost("updateDessertImage")]
        public IActionResult UpdateDessertImage([FromBody] DessertDTO dessertDTO)
        {

            try
            {
                Dessert? dessert = context.Desserts.Where(d => d.DessertId == dessertDTO.DessertId).FirstOrDefault();
                dessert.DessertImage = dessertDTO.DessertImage;
                context.SaveChanges();
                return Ok(dessert);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
        #region addordereddessert
        [HttpPost("addordereddessert")]
        public IActionResult AddOrderedDessert([FromBody] DTO.OrderedDessertDTO orderedDessertDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Create model dessert class
                Models.OrderedDessert modelsOrderedDessert = orderedDessertDto.GetModels();
                modelsOrderedDessert.OrderedDessertImage = "/dessertImages/defaultD.png";
                context.OrderedDesserts.Add(modelsOrderedDessert);
                context.SaveChanges();

                //Dessert was added!
                DTO.OrderedDessertDTO dtoOrderedDessert = new DTO.OrderedDessertDTO(modelsOrderedDessert);
                //dtoDessert.DessertImagePath = GetDessertImageVirtualPath(dtoDessert.DessertId, "dessertImage");
                return Ok(dtoOrderedDessert);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        #endregion

        #region getordereddesserts
        [HttpGet("getordereddesserts")]
        public List<OrderedDessertDTO> GetOrderedDesserts()
        {
            try
            {
                List<DTO.OrderedDessertDTO> dtoOrderedDesserts = new List<DTO.OrderedDessertDTO>();
                List<OrderedDessert> modelOrderedDesserts = context.OrderedDesserts
                                                            .Include(o=>o.Dessert)
                                                            .Include(o=>o.Baker).ToList();
                foreach (OrderedDessert d in modelOrderedDesserts)
                {
                    dtoOrderedDesserts.Add(new DTO.OrderedDessertDTO(d));
                }
                return dtoOrderedDesserts;                
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region DeleteOrderedDessert
        [HttpGet("DeleteOrderedDessert")]
        public IActionResult DeleteOrderedDessert(int id)
        {
            try
            {
                OrderedDessert d = context.OrderedDesserts.Where(o => o.OrderedDessertId == id).FirstOrDefault();
                if (d != null)
                {
                    context.OrderedDesserts.Remove(d);
                    context.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest("Ordered desert was not found in database");
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region updateOrderedDessertQuantity
        [HttpPost("updateQuantity")]
        public IActionResult UpdateQuantity([FromBody] OrderedDessertDTO orderedDessertDto, [FromQuery] int quantity)
        {

            try
            {
                OrderedDessert? orderedDessert = context.OrderedDesserts.Where<OrderedDessert>(d => d.OrderedDessertId == orderedDessertDto.OrderedDessertId).FirstOrDefault();
                double onePrice = orderedDessert.Price/ orderedDessert.Quantity;
                orderedDessert.Quantity = quantity;
                orderedDessert.Price = onePrice*quantity;
                context.SaveChanges();
                return Ok(orderedDessert);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Put Desser In Order
       
        [HttpPost("PutInOrder")]
        public IActionResult PutInOrder([FromBody] int oDessertId, [FromQuery] int orderId)
        {
            if (context.OrderedDesserts.Where<OrderedDessert>(b => b.OrderedDessertId == oDessertId).FirstOrDefault() != null)
            {
                try
                {

                    OrderedDessert oDessert = context.GetOrderedDessert(oDessertId);
                    oDessert.OrderId=orderId;
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

        #endregion

        #region addorder
        [HttpPost("addorder")]
        public IActionResult AddOrder([FromBody] DTO.OrderDTO orderDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Create model order class
                Models.Order modelsOrder = orderDto.GetModels();
                
                context.Orders.Add(modelsOrder);
                context.SaveChanges();

                //Order was added!
                DTO.OrderDTO dtoOrder = new DTO.OrderDTO(modelsOrder);
                return Ok(dtoOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        #endregion

        #region UpdateProfits
        [HttpPost("UpdateProfits")]
        public IActionResult UpdateProfits([FromBody] DTO.BakerDTO bakerDto, [FromQuery]double price)
        {
            if (context.Bakers.Where<Baker>(b => b.BakerId == bakerDto.BakerId).FirstOrDefault() != null)
            {
                try
                {

                    Baker baker = context.GetBaker(bakerDto.BakerId);
                    baker.Profits += price;
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
        #endregion
    }


}
