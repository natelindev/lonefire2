using System.ComponentModel.DataAnnotations;

namespace lonefire.Models.AccountModels
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
