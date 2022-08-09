using System.ComponentModel.DataAnnotations.Schema;
namespace TESTWebApi1.Models
{
    
    public class RegisterModel
    {
        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Token { get; set; }

        public int Id { get; set; }

    }
}
