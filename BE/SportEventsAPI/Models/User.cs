using System.ComponentModel.DataAnnotations.Schema;

namespace SportEventsAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Actual password property

        // Not stored in database; used for validation
        [NotMapped]
        public string RepeatPassword { get; set; }
    }
}
