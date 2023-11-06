using System.ComponentModel.DataAnnotations;

namespace SampleSite.Models.RequestModels
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "You must enter an email address.")]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "The email address must be between 6 and 200 characters.")]
        public string EmailAddress { get; set; } = "";
    }
}
