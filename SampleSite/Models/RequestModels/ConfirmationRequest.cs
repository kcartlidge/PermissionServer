using System.ComponentModel.DataAnnotations;

namespace SampleSite.Models.RequestModels
{
    public class ConfirmationRequest
    {
        [Required(ErrorMessage = "You must enter an email address.")]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "The email address must be between 6 and 200 characters.")]
        public string EmailAddress { get; set; } = "";

        [Required(ErrorMessage = "A confirmation code is required - check your email and try again.")]
        [StringLength(64, MinimumLength = 5, ErrorMessage = "The confirmation code is not in the expected format - check your email and try again.")]
        public string ConfirmationCode { get; set; } = "";
    }
}
