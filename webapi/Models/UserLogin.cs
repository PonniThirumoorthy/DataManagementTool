using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
