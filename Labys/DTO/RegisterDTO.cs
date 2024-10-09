using System.ComponentModel.DataAnnotations;

namespace Labys.DTO
{
    public class RegisterDTO
    {
       
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
