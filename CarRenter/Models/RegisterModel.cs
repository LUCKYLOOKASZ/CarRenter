using System.ComponentModel.DataAnnotations;

namespace CarRenter.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Nazwa u¿ytkownika jest wymagana.")]
        [MinLength(4, ErrorMessage = "Nazwa u¿ytkownika musi mieæ co najmniej 4 znaki.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Nieprawid³owy format adresu email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Has³o jest wymagane.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^\w\d\s:]).{6,}$", 
            ErrorMessage = "Has³o musi mieæ co najmniej 6 znaków, zawieraæ du¿¹ literê, cyfrê i znak specjalny.")]
        public string Password { get; set; }
    }
}
