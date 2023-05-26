using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi.EF;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private CustomerContext _customerContext;

        public UsersController(CustomerContext customerContext)
        {
            _customerContext = customerContext;
        }

        [HttpPost("/register")]
        public ResponseContext SignUp([FromBody] RegisterModel registerModel)
        {
            ResponseContext responseContext = new ResponseContext();
            try
            {

                if (registerModel != null)
                {
                    bool isUserNew = !_customerContext.UserLogin.Any(u => u.Email == registerModel.Email);
                    if (isUserNew)
                    {
                        registerModel.LastAccessedDate = DateTime.Now;
                        registerModel.PasswordHash = registerModel.PasswordHash;
                        _customerContext.UserLogin.Add(registerModel);
                        _customerContext.SaveChanges();
                        responseContext.IsSuccess = true;
                        responseContext.ErrorMessage = string.Empty;
                    }
                    else
                    {
                        responseContext.IsSuccess = false;
                        responseContext.ErrorMessage = "User already exists!";
                    }
                }
                else
                {
                    responseContext.IsSuccess = false;
                    responseContext.ErrorMessage = "Please provide all details to proceed!";
                }
            }
            catch (Exception ex)
            {
                responseContext.IsSuccess = false;
            }
            return responseContext;
        }

        [HttpPost("/signin")]
        public ResponseContext SignIn([FromBody] UserLogin loginModel)
        {
            ResponseContext responseContext = new ResponseContext();
            if (!string.IsNullOrEmpty(loginModel.Email))
            {
                bool isValidUser = _customerContext.UserLogin.Any(x => x.Email == loginModel.Email);
                bool isValidCreds = false;
                if (!isValidUser)
                {
                    responseContext.IsSuccess = false;
                    responseContext.ErrorMessage = "Invalid user!";
                }
                else
                {
                    isValidCreds = _customerContext.UserLogin.
                                    Any(user => user.Email == loginModel.Email
                                    && user.PasswordHash == loginModel.Password);
                    if (isValidCreds)
                    {
                        responseContext.IsSuccess = true;
                        responseContext.ErrorMessage = string.Empty;
                    }
                    else
                    {
                        responseContext.IsSuccess = false;
                        responseContext.ErrorMessage = "Bad credentials.";
                    }
                }

            }
            else
            {
                responseContext.IsSuccess = false;
                responseContext.ErrorMessage = "Please enter your email to proceed!";
            }
            return responseContext;
        }
    }
}
