using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class Details
    {
        [Key]
        public int Id { get; set; }
        public int seqno { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FileName { get; set; }
    }
}
