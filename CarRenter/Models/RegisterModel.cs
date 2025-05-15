using System.ComponentModel.DataAnnotations;

namespace CarRenter.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Nazwa u�ytkownika jest wymagana.")]
        [MinLength(4, ErrorMessage = "Nazwa u�ytkownika musi mie� co najmniej 4 znaki.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Nieprawid�owy format adresu email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Has�o jest wymagane.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^\w\d\s:]).{6,}$", 
            ErrorMessage = "Has�o musi mie� co najmniej 6 znak�w, zawiera� du�� liter�, cyfr� i znak specjalny.")]
        public string Password { get; set; }
    }
}
