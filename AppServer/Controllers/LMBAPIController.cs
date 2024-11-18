using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppServer.Models;
using System.Runtime.InteropServices.Marshalling;
using AppServer.DTO;
using AppServer.Models;

namespace AppServer.Controllers
{
    [Route("api/[controller]")]
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


        //[HttpPost("Sign Up")]
        //public IActionResult Register([FromBody] DTO.UserDTO userDto)
        //{
        //    try
        //    {
        //        HttpContext.Session.Clear(); //Logout any previous login attempt

        //        //Get model user class from DB with matching email. 
        //        Models.User modelsUser = new User()
        //        {
        //           Username = userDto.Username,
        //           Password = userDto.Password,
        //           Mail = userDto.Mail,
        //           Name = userDto.Name,
        //           UserTypeId = userDto.UserTypeId
        //        };

        //        context.Users.Add(modelsUser);
        //        context.SaveChanges();
               
        //        if (userDto.UserTypeId==2)
        //        {
        //            Models.Baker modelsBaker = new Baker()
        //            {
        //                HighestPrice = userDto.HighestPrice,
        //                ConfectioneryTypeId = userDto.ConfectioneryTypeId
        //            };

        //            context.Bakers.Add(modelsBaker);
        //            context.SaveChanges();
        //        }

        //        //User was added!
        //        DTO.UserDTO dtoUser = new DTO.UserDTO(modelsUser);
        //        //dtoUser.ProfileImagePath = GetProfileImageVirtualPath(dtoUser.UserId);
        //        return Ok(dtoUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}

    }
}
